/*
  KeePass Password Safe - The Open-Source Password Manager
  Copyright (C) 2003-2016 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using KeePass.App;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Security;
using KeePassLib.Collections;
using KeePassLib.Utility;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass.Forms
{
	public partial class EditAutoTypeItemForm : Form
	{
		private AutoTypeConfig m_atConfig = null;
		private int m_iAssocIndex = -1;
		private bool m_bEditSequenceOnly = false;
		private string m_strDefaultSeq = string.Empty;
		private ProtectedStringDictionary m_vStringDict = null;

		private object m_objDialogSync = new object();
		private bool m_bDialogClosed = false;
#if DEBUG
		private static Dictionary<string, string> m_dWndTasks =
			new Dictionary<string, string>();
#endif

		// private Color m_clrOriginalForeground = Color.Black;
		// private Color m_clrOriginalBackground = Color.White;
		private List<Image> m_vWndImages = new List<Image>();

		private RichTextBoxContextMenu m_ctxKeySeq = new RichTextBoxContextMenu();
		private RichTextBoxContextMenu m_ctxKeyCodes = new RichTextBoxContextMenu();
		private bool m_bBlockUpdates = false;

		public EditAutoTypeItemForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		public void InitEx(AutoTypeConfig atConfig, int iAssocIndex, bool bEditSequenceOnly,
			string strDefaultSeq, ProtectedStringDictionary vStringDict)
		{
			Debug.Assert(atConfig != null); if(atConfig == null) throw new ArgumentNullException("atConfig");

			m_atConfig = atConfig;
			m_iAssocIndex = iAssocIndex;
			m_bEditSequenceOnly = bEditSequenceOnly;
			m_strDefaultSeq = (strDefaultSeq ?? string.Empty);
			m_vStringDict = (vStringDict ?? new ProtectedStringDictionary());
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_atConfig != null); if(m_atConfig == null) throw new InvalidOperationException();
			Debug.Assert(m_vStringDict != null); if(m_vStringDict == null) throw new InvalidOperationException();

			GlobalWindowManager.AddWindow(this);

			m_ctxKeySeq.Attach(m_rbKeySeq, this);
			m_ctxKeyCodes.Attach(m_rtbPlaceholders, this);

			if(!m_bEditSequenceOnly)
			{
				BannerFactory.CreateBannerEx(this, m_bannerImage,
					Properties.Resources.B48x48_KCMSystem, KPRes.ConfigureAutoTypeItem,
					KPRes.ConfigureAutoTypeItemDesc);
			}
			else // Edit keystrokes only
			{
				BannerFactory.CreateBannerEx(this, m_bannerImage,
					Properties.Resources.B48x48_KCMSystem, KPRes.ConfigureKeystrokeSeq,
					KPRes.ConfigureKeystrokeSeqDesc);
			}

			this.Icon = Properties.Resources.KeePass;

			// FontUtil.AssignDefaultBold(m_lblTargetWindow);
			// FontUtil.AssignDefaultBold(m_rbSeqDefault);
			// FontUtil.AssignDefaultBold(m_rbSeqCustom);

			UIUtil.EnableAutoCompletion(m_cmbWindow, false);

			// m_clrOriginalForeground = m_lblOpenHint.ForeColor;
			// m_clrOriginalBackground = m_cmbWindow.BackColor;
			// m_strOriginalWindowHint = m_lblTargetWindowInfo.Text;

			InitPlaceholdersBox();

			string strInitSeq = m_atConfig.DefaultSequence;
			if(m_iAssocIndex >= 0)
			{
				AutoTypeAssociation asInit = m_atConfig.GetAt(m_iAssocIndex);
				m_cmbWindow.Text = asInit.WindowName;

				if(!m_bEditSequenceOnly) strInitSeq = asInit.Sequence;
			}
			else if(m_bEditSequenceOnly)
				m_cmbWindow.Text = "(" + KPRes.Default + ")";
			else strInitSeq = string.Empty;

			bool bSetDefault = false;
			m_bBlockUpdates = true;
			if(strInitSeq.Length > 0) m_rbSeqCustom.Checked = true;
			else
			{
				m_rbSeqDefault.Checked = true;
				bSetDefault = true;
			}
			m_bBlockUpdates = false;

			if(bSetDefault) m_rbKeySeq.Text = m_strDefaultSeq;
			else m_rbKeySeq.Text = strInitSeq;

			try
			{
				if(NativeLib.IsUnix()) PopulateWindowsListUnix();
				else PopulateWindowsListWin();
			}
			catch(Exception) { Debug.Assert(false); }

			EnableControlsEx();
		}

		private void InitPlaceholdersBox()
		{
			const string VkcBreak = @"<break />";

			string[] vSpecialKeyCodes = new string[] {
				"TAB", "ENTER", "UP", "DOWN", "LEFT", "RIGHT",
				"HOME", "END", "PGUP", "PGDN",
				"INSERT", "DELETE", "SPACE", VkcBreak,
				"BACKSPACE", "BREAK", "CAPSLOCK", "ESC",
				"WIN", "LWIN", "RWIN", "APPS",
				"HELP", "NUMLOCK", "PRTSC", "SCROLLLOCK", VkcBreak,
				"F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
				"F13", "F14", "F15", "F16", VkcBreak,
				"ADD", "SUBTRACT", "MULTIPLY", "DIVIDE",
				"NUMPAD0", "NUMPAD1", "NUMPAD2", "NUMPAD3", "NUMPAD4",
				"NUMPAD5", "NUMPAD6", "NUMPAD7", "NUMPAD8", "NUMPAD9"
			};

			string[] vSpecialPlaceholders = new string[] {
				"GROUP", "GROUP_PATH", "GROUP_NOTES",
				"GROUP_SEL", "GROUP_SEL_PATH", "GROUP_SEL_NOTES",
				"PASSWORD_ENC",
				"URL:RMVSCM", "URL:SCM", "URL:HOST", "URL:PORT", "URL:PATH",
				"URL:QUERY", "URL:USERINFO", "URL:USERNAME", "URL:PASSWORD",
				// "BASE",
				"T-REPLACE-RX:/T/S/R/", "T-CONV:/T/C/",
				"C:Comment", VkcBreak,
				"DELAY 1000", "DELAY=200", "VKEY 13", "VKEY-NX 13", "VKEY-EX 13",
				"PICKCHARS", "PICKCHARS:Password:C=3",
				"NEWPASSWORD", "NEWPASSWORD:/Profile/", "HMACOTP", "CLEARFIELD",
				"APPACTIVATE " + KPRes.Title, "BEEP 800 200", VkcBreak,
				"APPDIR", "DB_PATH", "DB_DIR", "DB_NAME", "DB_BASENAME", "DB_EXT",
				"ENV_DIRSEP", "ENV_PROGRAMFILES_X86", VkcBreak,
				// "INTERNETEXPLORER", "FIREFOX", "OPERA", "GOOGLECHROME",
				// "SAFARI", VkcBreak,
				"DT_SIMPLE", "DT_YEAR", "DT_MONTH", "DT_DAY", "DT_HOUR", "DT_MINUTE",
				"DT_SECOND", "DT_UTC_SIMPLE", "DT_UTC_YEAR", "DT_UTC_MONTH",
				"DT_UTC_DAY", "DT_UTC_HOUR", "DT_UTC_MINUTE", "DT_UTC_SECOND"
			};

			RichTextBuilder rb = new RichTextBuilder();
			rb.AppendLine(KPRes.StandardFields, FontStyle.Bold, null, null, ":", null);

			rb.Append("{" + PwDefs.TitleField + "} ");
			rb.Append("{" + PwDefs.UserNameField + "} ");
			rb.Append("{" + PwDefs.PasswordField + "} ");
			rb.Append("{" + PwDefs.UrlField + "} ");
			rb.Append("{" + PwDefs.NotesField + "}");

			bool bCustomInitialized = false, bFirst = true;
			foreach(KeyValuePair<string, ProtectedString> kvp in m_vStringDict)
			{
				if(!PwDefs.IsStandardField(kvp.Key))
				{
					if(bCustomInitialized == false)
					{
						rb.AppendLine();
						rb.AppendLine();
						rb.AppendLine(KPRes.CustomFields, FontStyle.Bold, null, null, ":", null);
						bCustomInitialized = true;
					}

					if(!bFirst) rb.Append(" ");
					rb.Append("{" + PwDefs.AutoTypeStringPrefix + kvp.Key + "}");
					bFirst = false;
				}
			}

			rb.AppendLine();
			rb.AppendLine();
			rb.AppendLine(KPRes.KeyboardKeyModifiers, FontStyle.Bold, null, null, ":", null);
			rb.Append(KPRes.KeyboardKeyShift + @": +, ");
			rb.Append(KPRes.KeyboardKeyCtrl + @": ^, ");
			rb.Append(KPRes.KeyboardKeyAlt + @": %");

			rb.AppendLine();
			rb.AppendLine();
			rb.AppendLine(KPRes.SpecialKeys, FontStyle.Bold, null, null, ":", null);
			bFirst = true;
			foreach(string strNav in vSpecialKeyCodes)
			{
				if(strNav == VkcBreak) { rb.AppendLine(); rb.AppendLine(); bFirst = true; }
				else
				{
					if(!bFirst) rb.Append(" ");
					rb.Append("{" + strNav + "}");
					bFirst = false;
				}
			}

			rb.AppendLine();
			rb.AppendLine();
			rb.AppendLine(KPRes.OtherPlaceholders, FontStyle.Bold, null, null, ":", null);
			bFirst = true;
			foreach(string strPH in vSpecialPlaceholders)
			{
				if(strPH == VkcBreak) { rb.AppendLine(); rb.AppendLine(); bFirst = true; }
				else
				{
					if(!bFirst) rb.Append(" ");
					rb.Append("{" + strPH + "}");
					bFirst = false;
				}
			}

			if(SprEngine.FilterPlaceholderHints.Count > 0)
			{
				rb.AppendLine();
				rb.AppendLine();
				rb.AppendLine(KPRes.PluginProvided, FontStyle.Bold, null, null, ":", null);
				bFirst = true;
				foreach(string strP in SprEngine.FilterPlaceholderHints)
				{
					if(string.IsNullOrEmpty(strP)) continue;

					if(!bFirst) rb.Append(" ");
					rb.Append(strP);
					bFirst = false;
				}
			}

			rb.Build(m_rtbPlaceholders);

			LinkifyRtf(m_rtbPlaceholders);
		}

		private void OnFormShown(object sender, EventArgs e)
		{
			// Focusing doesn't work in OnFormLoad
			if(m_cmbWindow.Enabled)
				UIUtil.SetFocus(m_cmbWindow, this);
			else if(m_rbKeySeq.Enabled)
				UIUtil.SetFocus(m_rbKeySeq, this);
			else UIUtil.SetFocus(m_btnOK, this);
		}

		private void CleanUpEx()
		{
			lock(m_objDialogSync) { m_bDialogClosed = true; }

			m_cmbWindow.OrderedImageList = null;
			foreach(Image img in m_vWndImages)
			{
				if(img != null) img.Dispose();
			}
			m_vWndImages.Clear();

			m_ctxKeyCodes.Detach();
			m_ctxKeySeq.Detach();

#if DEBUG
			lock(m_dWndTasks) { Debug.Assert(m_dWndTasks.Count == 0); }
#endif
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			EnableControlsEx();
			Debug.Assert(m_btnOK.Enabled); if(!m_btnOK.Enabled) return;

			string strNewSeq = (m_rbSeqCustom.Checked ? m_rbKeySeq.Text : string.Empty);

			if(!m_bEditSequenceOnly)
			{
				AutoTypeAssociation atAssoc;
				if(m_iAssocIndex >= 0) atAssoc = m_atConfig.GetAt(m_iAssocIndex);
				else
				{
					atAssoc = new AutoTypeAssociation();
					m_atConfig.Add(atAssoc);
				}

				atAssoc.WindowName = m_cmbWindow.Text;
				atAssoc.Sequence = strNewSeq;
			}
			else m_atConfig.DefaultSequence = strNewSeq;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.AutoType, null);
		}

		private void EnableControlsEx()
		{
			if(m_bBlockUpdates) return;
			m_bBlockUpdates = true;

			// string strItemName = m_cmbWindow.Text;

			// bool bEnableOK = true;
			// // string strError = string.Empty;

			// if((m_atConfig.Get(strItemName) != null) && !m_bEditSequenceOnly)
			// {
			//	if((m_strOriginalName == null) || !strItemName.Equals(m_strOriginalName))
			//	{
			//		bEnableOK = false;
			//		// strError = KPRes.FieldNameExistsAlready;
			//	}
			// }

			// // if((strItemName.IndexOf('{') >= 0) || (strItemName.IndexOf('}') >= 0))
			// // {
			// //	bEnableOK = false;
			// //	// strError = KPRes.FieldNameInvalid;
			// // }

			// if(bEnableOK)
			// {
			//	// m_lblTargetWindowInfo.Text = m_strOriginalWindowHint;
			//	// m_lblTargetWindowInfo.ForeColor = m_clrOriginalForeground;
			//	m_cmbWindow.BackColor = m_clrOriginalBackground;
			//	m_btnOK.Enabled = true;
			// }
			// else
			// {
			//	// m_lblTargetWindowInfo.Text = strError;
			//	// m_lblTargetWindowInfo.ForeColor = Color.Red;
			//	m_cmbWindow.BackColor = AppDefs.ColorEditError;
			//	m_btnOK.Enabled = false;
			// }

			m_lblTargetWindow.Enabled = !m_bEditSequenceOnly;
			m_cmbWindow.Enabled = !m_bEditSequenceOnly;
			m_lblOpenHint.Enabled = !m_bEditSequenceOnly;
			m_lnkWildcardRegexHint.Enabled = !m_bEditSequenceOnly;

			// Workaround for disabled link render bug (gray too dark)
			m_lnkWildcardRegexHint.Visible = !m_bEditSequenceOnly;

			bool bCustom = m_rbSeqCustom.Checked;
			m_rbKeySeq.Enabled = bCustom;
			m_lblKeySeqInsertInfo.Enabled = bCustom;
			m_rtbPlaceholders.Enabled = bCustom;

			m_bBlockUpdates = false;
		}

		private void ColorizeKeySeq()
		{
			SprContext ctx = new SprContext();
			ctx.EncodeAsAutoTypeSequence = true;

			PwEntry pe = new PwEntry(true, true);
			pe.Strings = m_vStringDict;
			ctx.Entry = pe;

			SprSyntax.Highlight(m_rbKeySeq, ctx);
		}

		/* private void ColorizeKeySeq()
		{
			string strText = m_rbKeySeq.Text;

			int iSelStart = m_rbKeySeq.SelectionStart, iSelLen = m_rbKeySeq.SelectionLength;

			m_rbKeySeq.SelectAll();
			m_rbKeySeq.SelectionBackColor = SystemColors.Window;

			int iStart = 0;
			while(true)
			{
				int iPos = strText.IndexOf('{', iStart);
				if(iPos < 0) break;

				int iEnd = strText.IndexOf('}', iPos + 1);
				if(iEnd < 0) break;

				m_rbKeySeq.Select(iPos, iEnd - iPos + 1);
				m_rbKeySeq.SelectionBackColor = Color.FromArgb(212, 255, 212);
				iStart = iEnd;
			}

			m_rbKeySeq.SelectionStart = iSelStart;
			m_rbKeySeq.SelectionLength = iSelLen;
		} */

		private void OnTextChangedKeySeq(object sender, EventArgs e)
		{
			ColorizeKeySeq();
		}

		private static void LinkifyRtf(RichTextBox rtb)
		{
			Debug.Assert(rtb.HideSelection); // Flicker otherwise

			string str = rtb.Text;

			int iPos = str.IndexOf('{');
			while(iPos >= 0)
			{
				int iEnd = str.IndexOf('}', iPos);
				if(iEnd >= 1)
				{
					rtb.Select(iPos, iEnd - iPos + 1);
					UIUtil.RtfSetSelectionLink(rtb);
				}

				iPos = str.IndexOf('{', iPos + 1);
			}

			rtb.Select(0, 0);
		}

		private void OnPlaceholdersLinkClicked(object sender, LinkClickedEventArgs e)
		{
			if(!m_rbSeqCustom.Checked) m_rbSeqCustom.Checked = true;

			int nSelStart = m_rbKeySeq.SelectionStart;
			int nSelLength = m_rbKeySeq.SelectionLength;
			string strText = m_rbKeySeq.Text;
			string strUrl = e.LinkText;

			if(nSelLength > 0)
				strText = strText.Remove(nSelStart, nSelLength);

			m_rbKeySeq.Text = strText.Insert(nSelStart, strUrl);
			m_rbKeySeq.Select(nSelStart + strUrl.Length, 0);
			UIUtil.SetFocus(m_rbKeySeq, this);
		}

		private void OnWindowTextUpdate(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnWindowSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnWildcardRegexLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.AutoType, AppDefs.HelpTopics.AutoTypeWindowFilters);
		}

		private void OnSeqDefaultCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnSeqCustomCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			CleanUpEx();
		}

		private sealed class PwlwInfo
		{
			public EditAutoTypeItemForm Form { get; private set; }
			public IntPtr WindowHandle { get; private set; }

			public PwlwInfo(EditAutoTypeItemForm f, IntPtr h)
			{
				this.Form = f;
				this.WindowHandle = h;
			}
		}

		private void PopulateWindowsListWin()
		{
			Dictionary<IntPtr, bool> dWnds = new Dictionary<IntPtr, bool>();
			NativeMethods.EnumWindowsProc procEnum = delegate(IntPtr hWnd,
				IntPtr lParam)
			{
				try
				{
					if(hWnd != IntPtr.Zero) dWnds[hWnd] = true;
				}
				catch(Exception) { Debug.Assert(false); }

				return true;
			};
			NativeMethods.EnumWindows(procEnum, IntPtr.Zero);
			GC.KeepAlive(procEnum); // Like in MainWindowFinder.FindMainWindow

			// On Windows 8 and higher, EnumWindows does not return Metro
			// app windows, thus we try to discover these windows using
			// the FindWindowEx function; we do this in addition to EnumWindows,
			// because calling FindWindowEx in a loop is less reliable (and
			// by additionally using EnumWindows we at least get all desktop
			// windows for sure)
			if(WinUtil.IsAtLeastWindows8)
			{
				int nMax = (dWnds.Count * 2) + 2;
				IntPtr h = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero,
					null, null);
				for(int i = 0; i < nMax; ++i)
				{
					if(h == IntPtr.Zero) break;

					dWnds[h] = true;

					h = NativeMethods.FindWindowEx(IntPtr.Zero, h, null, null);
				}
			}

			foreach(KeyValuePair<IntPtr, bool> kvp in dWnds)
				ThreadPool.QueueUserWorkItem(new WaitCallback(
					EditAutoTypeItemForm.EvalWindowProc),
					new PwlwInfo(this, kvp.Key));

			m_cmbWindow.OrderedImageList = m_vWndImages;
		}

		private static void EvalWindowProc(object objState)
		{
#if DEBUG
			string strTaskID = Guid.NewGuid().ToString();
			lock(m_dWndTasks) { m_dWndTasks[strTaskID] = @"<<<UNDEFINED>>>"; }
#endif

			try
			{
				PwlwInfo pInfo = (objState as PwlwInfo);
				IntPtr hWnd = pInfo.WindowHandle;
				if(hWnd == IntPtr.Zero) { Debug.Assert(false); return; }

				uint uSmtoFlags = (NativeMethods.SMTO_NORMAL |
					NativeMethods.SMTO_ABORTIFHUNG);
				IntPtr pLen = IntPtr.Zero;
				IntPtr pSmto = NativeMethods.SendMessageTimeout(hWnd,
					NativeMethods.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero,
					uSmtoFlags, 2000, ref pLen);
				if(pSmto == IntPtr.Zero) return;

				string strName = NativeMethods.GetWindowText(hWnd, true);
				if(string.IsNullOrEmpty(strName)) return;

#if DEBUG
				Debug.Assert(strName.Length <= pLen.ToInt64());
				lock(m_dWndTasks) { m_dWndTasks[strTaskID] = strName; }
#endif

				if((NativeMethods.GetWindowStyle(hWnd) &
					NativeMethods.WS_VISIBLE) == 0) return;
				if(NativeMethods.IsTaskBar(hWnd)) return;

				Image img = UIUtil.GetWindowImage(hWnd, true);

				if(pInfo.Form.InvokeRequired)
					pInfo.Form.Invoke(new AddWindowProcDelegate(
						EditAutoTypeItemForm.AddWindowProc), new object[] {
						pInfo.Form, hWnd, strName, img });
				else AddWindowProc(pInfo.Form, hWnd, strName, img);
			}
			catch(Exception) { Debug.Assert(false); }
#if DEBUG
			finally
			{
				lock(m_dWndTasks) { m_dWndTasks.Remove(strTaskID); }
			}
#endif
		}

		private delegate void AddWindowProcDelegate(EditAutoTypeItemForm f,
			IntPtr h, string strWndName, Image img);
		private static void AddWindowProc(EditAutoTypeItemForm f, IntPtr h,
			string strWndName, Image img)
		{
			if(f == null) { Debug.Assert(false); return; }
			if(h == IntPtr.Zero) { Debug.Assert(false); return; }

			try
			{
				if(!AutoType.IsValidAutoTypeWindow(h, false)) return;

				lock(f.m_objDialogSync)
				{
					if(!f.m_bDialogClosed)
					{
						f.m_vWndImages.Add(img);
						f.m_cmbWindow.Items.Add(strWndName);
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private void PopulateWindowsListUnix()
		{
			string strWindows = NativeMethods.RunXDoTool(
				@"search --onlyvisible --name '.+' getwindowname %@");
			if(string.IsNullOrEmpty(strWindows)) return;

			strWindows = StrUtil.NormalizeNewLines(strWindows, false);
			string[] vWindows = strWindows.Split(new char[]{ '\n' });

			List<string> vListed = new List<string>();
			for(int i = 0; i < vWindows.Length; ++i)
			{
				string str = vWindows[i].Trim();

				bool bValid = true;
				foreach(Form f in Application.OpenForms)
				{
					if(IsOwnWindow(f, str)) { bValid = false; break; }
				}
				if(!bValid) continue;

				if((str.Length > 0) && (vListed.IndexOf(str) < 0))
				{
					m_cmbWindow.Items.Add(str);
					vListed.Add(str);
				}
			}
		}

		private static bool IsOwnWindow(Control cRoot, string strText)
		{
			if(cRoot == null) { Debug.Assert(false); return false; }
			if(cRoot.Text.Trim() == strText) return true;

			foreach(Control cSub in cRoot.Controls)
			{
				if(cSub == cRoot) { Debug.Assert(false); continue; }
				if(IsOwnWindow(cSub, strText)) return true;
			}

			return false;
		}
	}
}
