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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using System.Media;

using Microsoft.Win32;

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI.ToolStripRendering;
using KeePass.Util;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Delegates;
using KeePassLib.Interfaces;
using KeePassLib.Utility;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass.UI
{
	public sealed class UIScrollInfo
	{
		private readonly int m_dx;
		public int ScrollX { get { return m_dx; } }

		private readonly int m_dy;
		public int ScrollY { get { return m_dy; } }

		private readonly int m_idxTop;
		public int TopIndex { get { return m_idxTop; } }

		public UIScrollInfo(int iScrollX, int iScrollY, int iTopIndex)
		{
			m_dx = iScrollX;
			m_dy = iScrollY;
			m_idxTop = iTopIndex;
		}
	}

	public static class UIUtil
	{
		private static bool m_bVistaStyleLists = false;
		public static bool VistaStyleListsSupported
		{
			get { return m_bVistaStyleLists; }
		}

		public static bool IsDarkTheme
		{
			get
			{
				return !IsDarkColor(SystemColors.ControlText);
			}
		}

		public static bool IsHighContrast
		{
			get
			{
				try { return SystemInformation.HighContrast; }
				catch(Exception) { Debug.Assert(false); }
				return false;
			}
		}

		public static void Initialize(bool bReinitialize)
		{
			// bReinitialize is currently not used, but not removed
			// for plugin backward compatibility

			string strUuid = Program.Config.UI.ToolStripRenderer;
			ToolStripRenderer tsr = TsrPool.GetBestRenderer(strUuid);
			if(tsr == null) { Debug.Assert(false); tsr = new ToolStripProfessionalRenderer(); }
			ToolStripManager.Renderer = tsr;

			m_bVistaStyleLists = (WinUtil.IsAtLeastWindowsVista &&
				(Environment.Version.Major >= 2));
		}

		public static void RtfSetSelectionLink(RichTextBox richTextBox)
		{
			IntPtr pCF = IntPtr.Zero;
			try
			{
				NativeMethods.CHARFORMAT2 cf = new NativeMethods.CHARFORMAT2();
				cf.cbSize = (uint)Marshal.SizeOf(cf);

				cf.dwMask = NativeMethods.CFM_LINK;
				cf.dwEffects = NativeMethods.CFE_LINK;

				IntPtr wParam = (IntPtr)NativeMethods.SCF_SELECTION;
				pCF = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
				Marshal.StructureToPtr(cf, pCF, false);

				NativeMethods.SendMessage(richTextBox.Handle,
					NativeMethods.EM_SETCHARFORMAT, wParam, pCF);
			}
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
			finally { if(pCF != IntPtr.Zero) Marshal.FreeCoTaskMem(pCF); }
		}

		private static NativeMethods.CHARFORMAT2 RtfGetCharFormat(RichTextBox rtb)
		{
			NativeMethods.CHARFORMAT2 cf = new NativeMethods.CHARFORMAT2();
			cf.cbSize = (uint)Marshal.SizeOf(cf);

			try
			{
				IntPtr wParam = (IntPtr)NativeMethods.SCF_SELECTION;
				IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
				Marshal.StructureToPtr(cf, lParam, false);

				NativeMethods.SendMessage(rtb.Handle,
					NativeMethods.EM_GETCHARFORMAT, wParam, lParam);

				cf = (NativeMethods.CHARFORMAT2)Marshal.PtrToStructure(lParam,
					typeof(NativeMethods.CHARFORMAT2));
				Marshal.FreeCoTaskMem(lParam);
			}
			catch(Exception) { Debug.Assert(false); }

			return cf;
		}

		public static bool RtfIsFirstCharLink(RichTextBox rtb)
		{
			NativeMethods.CHARFORMAT2 cf = RtfGetCharFormat(rtb);
			return ((cf.dwEffects & NativeMethods.CFE_LINK) != 0);
		}

		public static void RtfLinkifyExtUrls(RichTextBox richTextBox, bool bResetSelection)
		{
			const string strProto = "cmd://";

			try
			{
				string strText = richTextBox.Text;

				int nOffset = 0;
				while(nOffset < strText.Length)
				{
					int nStart = strText.IndexOf(strProto, nOffset, StrUtil.CaseIgnoreCmp);
					if(nStart < 0) break;

					richTextBox.Select(nStart, UrlUtil.GetUrlLength(strText, nStart));
					RtfSetSelectionLink(richTextBox);

					nOffset = nStart + 1;
				}

				if(bResetSelection) richTextBox.Select(0, 0);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void RtfLinkifyText(RichTextBox rtb, string strLinkText,
			bool bResetTempSelection)
		{
			if(rtb == null) throw new ArgumentNullException("rtb");
			if(string.IsNullOrEmpty(strLinkText)) return; // No assert

			try
			{
				string strText = rtb.Text;
				int nStart = strText.IndexOf(strLinkText);

				if(nStart >= 0)
				{
					rtb.Select(nStart, strLinkText.Length);
					RtfSetSelectionLink(rtb);

					if(bResetTempSelection) rtb.Select(0, 0);
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void RtfLinkifyReferences(RichTextBox rtb,
			bool bResetTempSelection)
		{
			try
			{
				string str = rtb.Text;

				int iOffset = 0;
				while(true)
				{
					int iStart = str.IndexOf(SprEngine.StrRefStart, iOffset,
						StrUtil.CaseIgnoreCmp);
					if(iStart < 0) break;
					int iEnd = str.IndexOf(SprEngine.StrRefEnd, iStart + 1,
						StrUtil.CaseIgnoreCmp);
					if(iEnd <= iStart) break;

					string strRef = str.Substring(iStart, iEnd - iStart + 1);
					RtfLinkifyText(rtb, strRef, bResetTempSelection);

					iOffset = iStart + 1;
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		[Obsolete("Use GfxUtil.LoadImage instead.")]
		public static Image LoadImage(byte[] pb)
		{
			return GfxUtil.LoadImage(pb);
		}

		public static Image CreateColorBitmap24(int nWidth, int nHeight, Color color)
		{
			Bitmap bmp = new Bitmap(nWidth, nHeight, PixelFormat.Format24bppRgb);

			using(Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(color);
			}

			return bmp;
		}

		public static ImageList BuildImageListUnscaled(List<Image> lImages,
			int nWidth, int nHeight)
		{
			ImageList imgList = new ImageList();
			imgList.ImageSize = new Size(nWidth, nHeight);
			imgList.ColorDepth = ColorDepth.Depth32Bit;

			if((lImages != null) && (lImages.Count > 0))
				imgList.Images.AddRange(lImages.ToArray());

			return imgList;
		}

		public static ImageList BuildImageList(List<PwCustomIcon> vImages,
			int nWidth, int nHeight)
		{
			ImageList imgList = new ImageList();
			imgList.ImageSize = new Size(nWidth, nHeight);
			imgList.ColorDepth = ColorDepth.Depth32Bit;

			List<Image> lImages = BuildImageListEx(vImages, nWidth, nHeight);
			if((lImages != null) && (lImages.Count > 0))
				imgList.Images.AddRange(lImages.ToArray());

			return imgList;
		}

		public static List<Image> BuildImageListEx(List<PwCustomIcon> vImages,
			int nWidth, int nHeight)
		{
			List<Image> lImages = new List<Image>();

			foreach(PwCustomIcon pwci in vImages)
			{
				Image img = pwci.GetImage(nWidth, nHeight);
				if(img == null)
				{
					Debug.Assert(false);
					img = UIUtil.CreateColorBitmap24(nWidth, nHeight, Color.White);
				}

				if((img.Width != nWidth) || (img.Height != nHeight))
				{
					Debug.Assert(false);
					img = new Bitmap(img, new Size(nWidth, nHeight));
				}

				lImages.Add(img);
			}

			return lImages;
		}

		public static ImageList ConvertImageList24(List<Image> vImages,
			int nWidth, int nHeight, Color clrBack)
		{
			ImageList ilNew = new ImageList();
			ilNew.ImageSize = new Size(nWidth, nHeight);
			ilNew.ColorDepth = ColorDepth.Depth24Bit;

			List<Image> vNewImages = new List<Image>();
			foreach(Image img in vImages)
			{
				Bitmap bmpNew = new Bitmap(nWidth, nHeight, PixelFormat.Format24bppRgb);

				using(Graphics g = Graphics.FromImage(bmpNew))
				{
					g.Clear(clrBack);

					if((img.Width == nWidth) && (img.Height == nHeight))
						g.DrawImageUnscaled(img, 0, 0);
					else
					{
						g.InterpolationMode = InterpolationMode.High;
						g.DrawImage(img, 0, 0, nWidth, nHeight);
					}
				}

				vNewImages.Add(bmpNew);
			}
			ilNew.Images.AddRange(vNewImages.ToArray());

			return ilNew;
		}

		public static ImageList CloneImageList(ImageList ilSource, bool bCloneImages)
		{
			Debug.Assert(ilSource != null); if(ilSource == null) throw new ArgumentNullException("ilSource");

			ImageList ilNew = new ImageList();
			ilNew.ColorDepth = ilSource.ColorDepth;
			ilNew.ImageSize = ilSource.ImageSize;

			foreach(Image img in ilSource.Images)
			{
				if(bCloneImages) ilNew.Images.Add(new Bitmap(img));
				else ilNew.Images.Add(img);
			}

			return ilNew;
		}

		public static bool DrawAnimatedRects(Rectangle rectFrom, Rectangle rectTo)
		{
			bool bResult;

			try
			{
				NativeMethods.RECT rnFrom = new NativeMethods.RECT(rectFrom);
				NativeMethods.RECT rnTo = new NativeMethods.RECT(rectTo);

				bResult = NativeMethods.DrawAnimatedRects(IntPtr.Zero,
					NativeMethods.IDANI_CAPTION, ref rnFrom, ref rnTo);
			}
			catch(Exception) { Debug.Assert(false); bResult = false; }

			return bResult;
		}

		private static void SetCueBanner(IntPtr hWnd, string strText)
		{
			Debug.Assert(strText != null); if(strText == null) throw new ArgumentNullException("strText");

			IntPtr pText = IntPtr.Zero;
			try
			{
				pText = Marshal.StringToHGlobalUni(strText);
				NativeMethods.SendMessage(hWnd, NativeMethods.EM_SETCUEBANNER,
					IntPtr.Zero, pText);
			}
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
			finally { if(pText != IntPtr.Zero) Marshal.FreeHGlobal(pText); }
		}

		public static void SetCueBanner(TextBox tb, string strText)
		{
			SetCueBanner(tb.Handle, strText);
		}

		public static void SetCueBanner(ToolStripTextBox tb, string strText)
		{
			SetCueBanner(tb.TextBox, strText);
		}

		public static void SetCueBanner(ToolStripComboBox tb, string strText)
		{
			try
			{
				NativeMethods.COMBOBOXINFO cbi = new NativeMethods.COMBOBOXINFO();
				cbi.cbSize = Marshal.SizeOf(cbi);

				NativeMethods.GetComboBoxInfo(tb.ComboBox.Handle, ref cbi);

				SetCueBanner(cbi.hwndEdit, strText);
			}
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
		}

		public static Bitmap CreateScreenshot()
		{
			return CreateScreenshot(null);
		}

		public static Bitmap CreateScreenshot(Screen sc)
		{
			try
			{
				Screen s = (sc ?? Screen.PrimaryScreen);
				Bitmap bmp = new Bitmap(s.Bounds.Width, s.Bounds.Height);

				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.CopyFromScreen(s.Bounds.Location, new Point(0, 0),
						s.Bounds.Size);
				}

				return bmp;
			}
			catch(Exception) { } // Throws on Cocoa and Quartz

			return null;
		}

		public static void DimImage(Image bmp)
		{
			if(bmp == null) { Debug.Assert(false); return; }

			using(Brush b = new SolidBrush(Color.FromArgb(192, Color.Black)))
			{
				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.FillRectangle(b, 0, 0, bmp.Width, bmp.Height);
				}
			}
		}

		public static void PrepareStandardMultilineControl(RichTextBox rtb,
			bool bSimpleTextOnly, bool bCtrlEnterAccepts)
		{
			Debug.Assert(rtb != null); if(rtb == null) throw new ArgumentNullException("rtb");

			try
			{
				int nStyle = NativeMethods.GetWindowStyle(rtb.Handle);

				if((nStyle & NativeMethods.ES_WANTRETURN) == 0)
				{
					NativeMethods.SetWindowLong(rtb.Handle, NativeMethods.GWL_STYLE,
						nStyle | NativeMethods.ES_WANTRETURN);

					Debug.Assert((NativeMethods.GetWindowStyle(rtb.Handle) &
						NativeMethods.ES_WANTRETURN) != 0);
				}
			}
			catch(Exception) { }

			CustomRichTextBoxEx crtb = (rtb as CustomRichTextBoxEx);
			if(crtb != null)
			{
				crtb.SimpleTextOnly = bSimpleTextOnly;
				crtb.CtrlEnterAccepts = bCtrlEnterAccepts;
			}
			else { Debug.Assert(!bSimpleTextOnly && !bCtrlEnterAccepts); }
		}

		public static void SetMultilineText(TextBox tb, string str)
		{
			if(tb == null) { Debug.Assert(false); return; }
			if(str == null) str = string.Empty;

			if(!NativeLib.IsUnix())
				str = StrUtil.NormalizeNewLines(str, true);

			tb.Text = str;
		}

		/// <summary>
		/// Fill a <c>ListView</c> with password entries.
		/// </summary>
		/// <param name="lv"><c>ListView</c> to fill.</param>
		/// <param name="vEntries">Entries.</param>
		/// <param name="vColumns">Columns of the <c>ListView</c>. The first
		/// parameter of the key-value pair is the internal string field name,
		/// and the second one the text displayed in the column header.</param>
		public static void CreateEntryList(ListView lv, IEnumerable<PwEntry> vEntries,
			List<KeyValuePair<string, string>> vColumns, ImageList ilIcons)
		{
			if(lv == null) throw new ArgumentNullException("lv");
			if(vEntries == null) throw new ArgumentNullException("vEntries");
			if(vColumns == null) throw new ArgumentNullException("vColumns");
			if(vColumns.Count == 0) throw new ArgumentException();
			// ilIcons may be null

			lv.BeginUpdate();

			lv.Items.Clear();
			lv.Columns.Clear();
			lv.ShowGroups = true;
			lv.SmallImageList = ilIcons;

			foreach(KeyValuePair<string, string> kvp in vColumns)
			{
				lv.Columns.Add(kvp.Value);
			}

			DocumentManagerEx dm = Program.MainForm.DocumentManager;
			ListViewGroup lvg = new ListViewGroup(Guid.NewGuid().ToString());
			DateTime dtNow = DateTime.Now;
			bool bFirstEntry = true;

			foreach(PwEntry pe in vEntries)
			{
				if(pe == null) { Debug.Assert(false); continue; }

				if(pe.ParentGroup != null)
				{
					string strGroup = pe.ParentGroup.GetFullPath(" - ", false);
					if(strGroup != lvg.Header)
					{
						lvg = new ListViewGroup(strGroup, HorizontalAlignment.Left);
						lv.Groups.Add(lvg);
					}
				}

				ListViewItem lvi = new ListViewItem(AppDefs.GetEntryField(pe, vColumns[0].Key));

				if(ilIcons != null)
				{
					if(pe.Expires && (pe.ExpiryTime <= dtNow))
						lvi.ImageIndex = (int)PwIcon.Expired;
					else if(pe.CustomIconUuid == PwUuid.Zero)
						lvi.ImageIndex = (int)pe.IconId;
					else
					{
						lvi.ImageIndex = (int)pe.IconId;

						foreach(PwDocument ds in dm.Documents)
						{
							int nInx = ds.Database.GetCustomIconIndex(pe.CustomIconUuid);
							if(nInx > -1)
							{
								ilIcons.Images.Add(new Bitmap(DpiUtil.GetIcon(
									ds.Database, pe.CustomIconUuid)));
								lvi.ImageIndex = ilIcons.Images.Count - 1;
								break;
							}
						}
					}
				}

				for(int iCol = 1; iCol < vColumns.Count; ++iCol)
					lvi.SubItems.Add(AppDefs.GetEntryField(pe, vColumns[iCol].Key));

				if(!pe.ForegroundColor.IsEmpty)
					lvi.ForeColor = pe.ForegroundColor;
				if(!pe.BackgroundColor.IsEmpty)
					lvi.BackColor = pe.BackgroundColor;

				lvi.Tag = pe;

				lv.Items.Add(lvi);
				lvg.Items.Add(lvi);

				if(bFirstEntry)
				{
					UIUtil.SetFocusedItem(lv, lvi, true);
					bFirstEntry = false;
				}
			}

			int nColWidth = (lv.ClientRectangle.Width - GetVScrollBarWidth()) /
				vColumns.Count;
			foreach(ColumnHeader ch in lv.Columns)
			{
				ch.Width = nColWidth;
			}

			lv.EndUpdate();
		}

		/// <summary>
		/// Fill a <c>ListView</c> with password entries.
		/// </summary>
		public static void CreateEntryList(ListView lv, List<AutoTypeCtx> lCtxs,
			AceAutoTypeCtxFlags f, ImageList ilIcons)
		{
			if(lv == null) throw new ArgumentNullException("lv");
			if(lCtxs == null) throw new ArgumentNullException("lCtxs");

			lv.BeginUpdate();

			lv.Items.Clear();
			lv.Columns.Clear();
			lv.ShowGroups = true;
			lv.SmallImageList = ilIcons;

			Debug.Assert((f & AceAutoTypeCtxFlags.ColTitle) != AceAutoTypeCtxFlags.None);
			f |= AceAutoTypeCtxFlags.ColTitle; // Enforce title

			lv.Columns.Add(KPRes.Title);
			if((f & AceAutoTypeCtxFlags.ColUserName) != AceAutoTypeCtxFlags.None)
				lv.Columns.Add(KPRes.UserName);
			if((f & AceAutoTypeCtxFlags.ColPassword) != AceAutoTypeCtxFlags.None)
				lv.Columns.Add(KPRes.Password);
			if((f & AceAutoTypeCtxFlags.ColUrl) != AceAutoTypeCtxFlags.None)
				lv.Columns.Add(KPRes.Url);
			if((f & AceAutoTypeCtxFlags.ColNotes) != AceAutoTypeCtxFlags.None)
				lv.Columns.Add(KPRes.Notes);
			if((f & AceAutoTypeCtxFlags.ColSequenceComments) != AceAutoTypeCtxFlags.None)
				lv.Columns.Add(KPRes.Sequence + " - " + KPRes.Comments);
			if((f & AceAutoTypeCtxFlags.ColSequence) != AceAutoTypeCtxFlags.None)
				lv.Columns.Add(KPRes.Sequence);

			ListViewGroup lvg = new ListViewGroup(Guid.NewGuid().ToString());
			DateTime dtNow = DateTime.Now;
			Regex rxSeqCmt = null;
			bool bFirstEntry = true;

			foreach(AutoTypeCtx ctx in lCtxs)
			{
				if(ctx == null) { Debug.Assert(false); continue; }
				PwEntry pe = ctx.Entry;
				if(pe == null) { Debug.Assert(false); continue; }
				PwDatabase pd = ctx.Database;
				if(pd == null) { Debug.Assert(false); continue; }

				if(pe.ParentGroup != null)
				{
					string strGroup = pe.ParentGroup.GetFullPath(" - ", true);
					if(strGroup != lvg.Header)
					{
						lvg = new ListViewGroup(strGroup, HorizontalAlignment.Left);
						lv.Groups.Add(lvg);
					}
				}

				SprContext sprCtx = new SprContext(pe, pd, SprCompileFlags.Deref);
				sprCtx.ForcePlainTextPasswords = false;

				ListViewItem lvi = new ListViewItem(SprEngine.Compile(
					pe.Strings.ReadSafe(PwDefs.TitleField), sprCtx));

				if(pe.Expires && (pe.ExpiryTime <= dtNow))
					lvi.ImageIndex = (int)PwIcon.Expired;
				else if(pe.CustomIconUuid == PwUuid.Zero)
					lvi.ImageIndex = (int)pe.IconId;
				else
				{
					int nInx = pd.GetCustomIconIndex(pe.CustomIconUuid);
					if(nInx > -1)
					{
						ilIcons.Images.Add(new Bitmap(DpiUtil.GetIcon(
							pd, pe.CustomIconUuid)));
						lvi.ImageIndex = ilIcons.Images.Count - 1;
					}
					else { Debug.Assert(false); lvi.ImageIndex = (int)pe.IconId; }
				}

				if((f & AceAutoTypeCtxFlags.ColUserName) != AceAutoTypeCtxFlags.None)
					lvi.SubItems.Add(SprEngine.Compile(pe.Strings.ReadSafe(
						PwDefs.UserNameField), sprCtx));
				if((f & AceAutoTypeCtxFlags.ColPassword) != AceAutoTypeCtxFlags.None)
					lvi.SubItems.Add(SprEngine.Compile(pe.Strings.ReadSafe(
						PwDefs.PasswordField), sprCtx));
				if((f & AceAutoTypeCtxFlags.ColUrl) != AceAutoTypeCtxFlags.None)
					lvi.SubItems.Add(SprEngine.Compile(pe.Strings.ReadSafe(
						PwDefs.UrlField), sprCtx));
				if((f & AceAutoTypeCtxFlags.ColNotes) != AceAutoTypeCtxFlags.None)
					lvi.SubItems.Add(StrUtil.MultiToSingleLine(SprEngine.Compile(
						pe.Strings.ReadSafe(PwDefs.NotesField), sprCtx)));
				if((f & AceAutoTypeCtxFlags.ColSequenceComments) != AceAutoTypeCtxFlags.None)
				{
					if(rxSeqCmt == null)
						rxSeqCmt = new Regex("\\{[Cc]:[^\\}]*\\}");

					string strSeqCmt = string.Empty, strImpSeqCmt = string.Empty;
					foreach(Match m in rxSeqCmt.Matches(ctx.Sequence))
					{
						string strPart = m.Value;
						if(strPart == null) { Debug.Assert(false); continue; }
						if(strPart.Length < 4) { Debug.Assert(false); continue; }

						strPart = strPart.Substring(3, strPart.Length - 4).Trim();
						bool bImp = strPart.StartsWith("!"); // Important comment
						if(bImp) strPart = strPart.Substring(1);
						if(strPart.Length == 0) continue;

						if(bImp)
						{
							if(strImpSeqCmt.Length > 0) strImpSeqCmt += " - ";
							strImpSeqCmt += strPart;
						}
						else
						{
							if(strSeqCmt.Length > 0) strSeqCmt += " - ";
							strSeqCmt += strPart;
						}
					}

					lvi.SubItems.Add((strImpSeqCmt.Length > 0) ? strImpSeqCmt :
						strSeqCmt);
				}
				if((f & AceAutoTypeCtxFlags.ColSequence) != AceAutoTypeCtxFlags.None)
					lvi.SubItems.Add(ctx.Sequence);
				Debug.Assert(lvi.SubItems.Count == lv.Columns.Count);

				if(!pe.ForegroundColor.IsEmpty)
					lvi.ForeColor = pe.ForegroundColor;
				if(!pe.BackgroundColor.IsEmpty)
					lvi.BackColor = pe.BackgroundColor;

				lvi.Tag = ctx;

				lv.Items.Add(lvi);
				lvg.Items.Add(lvi);

				if(bFirstEntry)
				{
					UIUtil.SetFocusedItem(lv, lvi, true);
					bFirstEntry = false;
				}
			}

			lv.EndUpdate();

			// Resize columns *after* EndUpdate, otherwise sizing problem
			// caused by the scrollbar
			UIUtil.ResizeColumns(lv, true);
		}

		public static int GetVScrollBarWidth()
		{
			try { return SystemInformation.VerticalScrollBarWidth; }
			catch(Exception) { Debug.Assert(false); }

			return 18; // Default theme on Windows Vista
		}

		/// <summary>
		/// Create a file type filter specification string.
		/// </summary>
		/// <param name="strExtension">Default extension(s), without leading
		/// dot. Multiple extensions must be separated by a '|' (e.g.
		/// "html|htm", having the same description "HTML Files").</param>
		public static string CreateFileTypeFilter(string strExtension, string strDescription,
			bool bIncludeAllFiles)
		{
			StringBuilder sb = new StringBuilder();

			if(!string.IsNullOrEmpty(strExtension) && !string.IsNullOrEmpty(
				strDescription))
			{
				// str += strDescription + @" (*." + strExtension +
				//	@")|*." + strExtension;

				string[] vExts = strExtension.Split(new char[]{ '|' },
					StringSplitOptions.RemoveEmptyEntries);
				if(vExts.Length > 0)
				{
					sb.Append(strDescription);
					sb.Append(@" (*.");

					for(int i = 0; i < vExts.Length; ++i)
					{
						if(i > 0) sb.Append(@", *.");
						sb.Append(vExts[i]);
					}

					sb.Append(@")|*.");

					for(int i = 0; i < vExts.Length; ++i)
					{
						if(i > 0) sb.Append(@";*.");
						sb.Append(vExts[i]);
					}
				}
			}

			if(bIncludeAllFiles)
			{
				if(sb.Length > 0) sb.Append(@"|");
				sb.Append(KPRes.AllFiles);
				sb.Append(@" (*.*)|*.*");
			}

			return sb.ToString();
		}

		public static string GetPrimaryFileTypeExt(string strExtensions)
		{
			if(strExtensions == null) { Debug.Assert(false); return string.Empty; }

			int i = strExtensions.IndexOf('|');
			if(i >= 0) return strExtensions.Substring(0, i);

			return strExtensions; // Single extension
		}

		[Obsolete("Use the overload with the strContext parameter.")]
		public static OpenFileDialog CreateOpenFileDialog(string strTitle, string strFilter,
			int iFilterIndex, string strDefaultExt, bool bMultiSelect, bool bRestoreDirectory)
		{
			return (OpenFileDialog)(CreateOpenFileDialog(strTitle, strFilter,
				iFilterIndex, strDefaultExt, bMultiSelect, string.Empty).FileDialog);
		}

		public static OpenFileDialogEx CreateOpenFileDialog(string strTitle, string strFilter,
			int iFilterIndex, string strDefaultExt, bool bMultiSelect, string strContext)
		{
			OpenFileDialogEx ofd = new OpenFileDialogEx(strContext);
			
			if(!string.IsNullOrEmpty(strDefaultExt))
				ofd.DefaultExt = strDefaultExt;

			if(!string.IsNullOrEmpty(strFilter))
			{
				ofd.Filter = strFilter;

				if(iFilterIndex > 0) ofd.FilterIndex = iFilterIndex;
			}

			ofd.Multiselect = bMultiSelect;

			if(!string.IsNullOrEmpty(strTitle))
				ofd.Title = strTitle;

			return ofd;
		}

		[Obsolete("Use the overload with the strContext parameter.")]
		public static SaveFileDialog CreateSaveFileDialog(string strTitle,
			string strSuggestedFileName, string strFilter, int iFilterIndex,
			string strDefaultExt, bool bRestoreDirectory)
		{
			return (SaveFileDialog)(CreateSaveFileDialog(strTitle, strSuggestedFileName,
				strFilter, iFilterIndex, strDefaultExt, string.Empty).FileDialog);
		}

		[Obsolete("Use the overload with the strContext parameter.")]
		public static SaveFileDialog CreateSaveFileDialog(string strTitle,
			string strSuggestedFileName, string strFilter, int iFilterIndex,
			string strDefaultExt, bool bRestoreDirectory, bool bIsDatabaseFile)
		{
			return (SaveFileDialog)(CreateSaveFileDialog(strTitle, strSuggestedFileName,
				strFilter, iFilterIndex, strDefaultExt, (bIsDatabaseFile ?
				AppDefs.FileDialogContext.Database : string.Empty)).FileDialog);
		}

		public static SaveFileDialogEx CreateSaveFileDialog(string strTitle,
			string strSuggestedFileName, string strFilter, int iFilterIndex,
			string strDefaultExt, string strContext)
		{
			SaveFileDialogEx sfd = new SaveFileDialogEx(strContext);

			if(!string.IsNullOrEmpty(strDefaultExt))
				sfd.DefaultExt = strDefaultExt;

			if(!string.IsNullOrEmpty(strSuggestedFileName))
				sfd.FileName = strSuggestedFileName;

			if(!string.IsNullOrEmpty(strFilter))
			{
				sfd.Filter = strFilter;

				if(iFilterIndex > 0) sfd.FilterIndex = iFilterIndex;
			}

			if(!string.IsNullOrEmpty(strTitle))
				sfd.Title = strTitle;

			if(strContext != null)
			{
				if((strContext == AppDefs.FileDialogContext.Database) &&
					(Program.Config.Defaults.FileSaveAsDirectory.Length > 0))
					sfd.InitialDirectory = Program.Config.Defaults.FileSaveAsDirectory;
			}

			return sfd;
		}

		public static FolderBrowserDialog CreateFolderBrowserDialog(string strDescription)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();

			if((strDescription != null) && (strDescription.Length > 0))
				fbd.Description = strDescription;

			fbd.ShowNewFolderButton = true;

			return fbd;
		}

		private static ColorDialog CreateColorDialog(Color clrDefault)
		{
			ColorDialog dlg = new ColorDialog();

			dlg.AllowFullOpen = true;
			dlg.AnyColor = true;
			if(!clrDefault.IsEmpty) dlg.Color = clrDefault;
			dlg.FullOpen = true;
			dlg.ShowHelp = false;
			// dlg.SolidColorOnly = false;

			try
			{
				string strColors = Program.Config.Defaults.CustomColors;
				if(!string.IsNullOrEmpty(strColors))
				{
					int[] vColors = StrUtil.DeserializeIntArray(strColors);
					if((vColors != null) && (vColors.Length > 0))
						dlg.CustomColors = vColors;
				}
			}
			catch(Exception) { Debug.Assert(false); }

			return dlg;
		}

		private static void SaveCustomColors(ColorDialog dlg)
		{
			if(dlg == null) { Debug.Assert(false); return; }

			try
			{
				int[] vColors = dlg.CustomColors;
				if((vColors == null) || (vColors.Length == 0))
					Program.Config.Defaults.CustomColors = string.Empty;
				else
					Program.Config.Defaults.CustomColors =
						StrUtil.SerializeIntArray(vColors);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static Color? ShowColorDialog(Color clrDefault)
		{
			ColorDialog dlg = CreateColorDialog(clrDefault);

			GlobalWindowManager.AddDialog(dlg);
			DialogResult dr = dlg.ShowDialog();
			GlobalWindowManager.RemoveDialog(dlg);

			SaveCustomColors(dlg);

			Color? clrResult = null;
			if(dr == DialogResult.OK) clrResult = dlg.Color;

			dlg.Dispose();
			return clrResult;
		}

		public static FontDialog CreateFontDialog(bool bEffects)
		{
			FontDialog dlg = new FontDialog();

			dlg.FontMustExist = true;
			dlg.ShowEffects = bEffects;

			return dlg;
		}

		public static void SetGroupNodeToolTip(TreeNode tn, PwGroup pg)
		{
			if((tn == null) || (pg == null)) { Debug.Assert(false); return; }

			string str = GetPwGroupToolTip(pg);
			if(str == null) return;

			try { tn.ToolTipText = str; }
			catch(Exception) { Debug.Assert(false); }
		}

		public static string GetPwGroupToolTip(PwGroup pg)
		{
			if(pg == null) { Debug.Assert(false); return null; }

			StringBuilder sb = new StringBuilder();
			sb.Append(pg.Name);

			string strNotes = pg.Notes.Trim();
			if(strNotes.Length > 0)
			{
				sb.Append(MessageService.NewParagraph);
				sb.Append(strNotes);
			}
			else return null;

			// uint uSubGroups, uEntries;
			// pg.GetCounts(true, out uSubGroups, out uEntries);
			// sb.Append(MessageService.NewParagraph);
			// sb.Append(KPRes.Subgroups); sb.Append(": "); sb.Append(uSubGroups);
			// sb.Append(MessageService.NewLine);
			// sb.Append(KPRes.Entries); sb.Append(": "); sb.Append(uEntries);

			return sb.ToString();
		}

		// public static string GetPwGroupToolTipTN(TreeNode tn)
		// {
		//	if(tn == null) { Debug.Assert(false); return null; }
		//	PwGroup pg = (tn.Tag as PwGroup);
		//	if(pg == null) { Debug.Assert(false); return null; }
		//	return GetPwGroupToolTip(pg);
		// }

		public static Color LightenColor(Color clrBase, double dblFactor)
		{
			if((dblFactor <= 0.0) || (dblFactor > 1.0)) return clrBase;

			unchecked
			{
				byte r = (byte)((double)(255 - clrBase.R) * dblFactor + clrBase.R);
				byte g = (byte)((double)(255 - clrBase.G) * dblFactor + clrBase.G);
				byte b = (byte)((double)(255 - clrBase.B) * dblFactor + clrBase.B);
				return Color.FromArgb((int)r, (int)g, (int)b);
			}
		}

		public static Color DarkenColor(Color clrBase, double dblFactor)
		{
			if((dblFactor <= 0.0) || (dblFactor > 1.0)) return clrBase;

			unchecked
			{
				byte r = (byte)((double)clrBase.R - ((double)clrBase.R * dblFactor));
				byte g = (byte)((double)clrBase.G - ((double)clrBase.G * dblFactor));
				byte b = (byte)((double)clrBase.B - ((double)clrBase.B * dblFactor));
				return Color.FromArgb((int)r, (int)g, (int)b);
			}
		}

		public static void MoveSelectedItemsInternalOne<T>(ListView lv,
			PwObjectList<T> v, bool bUp)
			where T : class, IDeepCloneable<T>
		{
			if(lv == null) throw new ArgumentNullException("lv");
			if(v == null) throw new ArgumentNullException("v");
			if(lv.Items.Count != (int)v.UCount) throw new ArgumentException();

			ListView.SelectedListViewItemCollection lvsic = lv.SelectedItems;
			if(lvsic.Count == 0) return;

			int nStart = (bUp ? 0 : (lvsic.Count - 1));
			int nEnd = (bUp ? lvsic.Count : -1);
			int iStep = (bUp ? 1 : -1);
			for(int i = nStart; i != nEnd; i += iStep)
				v.MoveOne((lvsic[i].Tag as T), bUp);
		}

		public static void DeleteSelectedItems<T>(ListView lv, PwObjectList<T>
			vInternalList)
			where T : class, IDeepCloneable<T>
		{
			if(lv == null) throw new ArgumentNullException("lv");
			if(vInternalList == null) throw new ArgumentNullException("vInternalList");

			ListView.SelectedIndexCollection lvsc = lv.SelectedIndices;
			int n = lvsc.Count; // Getting Count sends a message
			if(n == 0) return;

			// LVSIC: one access by index requires O(n) time, thus copy
			// all to an array (which requires O(1) for each element)
			int[] v = new int[n];
			lvsc.CopyTo(v, 0);

			for(int i = 0; i < n; ++i)
			{
				int nIndex = v[n - i - 1];
				if(vInternalList.Remove(lv.Items[nIndex].Tag as T))
					lv.Items.RemoveAt(nIndex);
				else { Debug.Assert(false); }
			}
		}

		public static object[] GetSelectedItemTags(ListView lv)
		{
			if(lv == null) throw new ArgumentNullException("lv");

			ListView.SelectedListViewItemCollection lvsc = lv.SelectedItems;
			if(lvsc == null) { Debug.Assert(false); return new object[0]; }
			int n = lvsc.Count; // Getting Count sends a message

			object[] p = new object[n];
			int i = 0;
			// LVSLVIC: one access by index requires O(n) time, thus use
			// enumerator instead (which requires O(1) for each element)
			foreach(ListViewItem lvi in lvsc)
			{
				if(i >= n) { Debug.Assert(false); break; }

				p[i] = lvi.Tag;
				++i;
			}
			Debug.Assert(i == n);

			return p;
		}

		public static void SelectItems(ListView lv, object[] vItemTags)
		{
			if(lv == null) throw new ArgumentNullException("lv");
			if(vItemTags == null) throw new ArgumentNullException("vItemTags");

			for(int i = 0; i < lv.Items.Count; ++i)
			{
				if(Array.IndexOf<object>(vItemTags, lv.Items[i].Tag) >= 0)
					lv.Items[i].Selected = true;
			}
		}

		public static void SetWebBrowserDocument(WebBrowser wb, string strDocumentText)
		{
			string strContent = (strDocumentText ?? string.Empty);

			wb.AllowNavigation = true;
			wb.DocumentText = strContent;

			// Wait for document being loaded
			for(int i = 0; i < 50; ++i)
			{
				if(wb.DocumentText == strContent) break;

				Thread.Sleep(20);
				Application.DoEvents();
			}
		}

		public static string GetWebBrowserDocument(WebBrowser wb)
		{
			return wb.DocumentText;
		}

		public static void SetExplorerTheme(IntPtr hWnd)
		{
			if(hWnd == IntPtr.Zero) { Debug.Assert(false); return; }

			try { NativeMethods.SetWindowTheme(hWnd, "explorer", null); }
			catch(Exception) { } // Not supported on older operating systems
		}

		public static void SetExplorerTheme(Control c, bool bUseListFont)
		{
			if(c == null) { Debug.Assert(false); return; }

			SetExplorerTheme(c.Handle);

			if(bUseListFont)
			{
				if(UISystemFonts.ListFont != null)
					c.Font = UISystemFonts.ListFont;
			}
		}

		public static void SetShield(Button btn, bool bSetShield)
		{
			if(btn == null) throw new ArgumentNullException("btn");

			try
			{
				if(btn.FlatStyle != FlatStyle.System)
				{
					Debug.Assert(false);
					btn.FlatStyle = FlatStyle.System;
				}

				IntPtr h = btn.Handle;
				if(h == IntPtr.Zero) { Debug.Assert(false); return; }

				NativeMethods.SendMessage(h, NativeMethods.BCM_SETSHIELD,
					IntPtr.Zero, (IntPtr)(bSetShield ? 1 : 0));
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void Configure(ToolStrip ts)
		{
			if(ts == null) { Debug.Assert(false); return; }
			if(Program.DesignMode) return;

			DpiUtil.Configure(ts);
		}

		// internal static void ConfigureToolStripItem(ToolStripItem ts)
		// {
		//	if(ts == null) { Debug.Assert(false); return; }
		//	if(Program.DesignMode) return;
		//	// Disable separators such that clicking on them
		//	// does not close the menu
		//	ToolStripSeparator tsSep = (ts as ToolStripSeparator);
		//	if(tsSep != null) tsSep.Enabled = false;
		// }

		public static void ConfigureTbButton(ToolStripItem tb, string strText,
			string strTooltip)
		{
			ConfigureTbButton(tb, strText, strTooltip, null);
		}

		private static char[] m_vTbTrim = null;
		public static void ConfigureTbButton(ToolStripItem tb, string strText,
			string strTooltip, ToolStripMenuItem tsmiEquiv)
		{
			if(strText != null) tb.Text = strText;

			if(m_vTbTrim == null)
				m_vTbTrim = new char[] { ' ', '\t', '\r', '\n', '.', '\u2026' };

			string strTip = (strTooltip ?? strText);
			if(strTip == null) return;

			strTip = StrUtil.RemoveAccelerator(strTip);
			strTip = strTip.Trim(m_vTbTrim);

			if((tsmiEquiv != null) && (strTip.Length > 0))
			{
				string strShortcut = tsmiEquiv.ShortcutKeyDisplayString;
				if(!string.IsNullOrEmpty(strShortcut))
					strTip += " (" + strShortcut + ")";
			}

			tb.ToolTipText = strTip;
		}

		public static void CreateGroupList(PwGroup pgContainer, ComboBox cmb,
			Dictionary<int, PwUuid> outCreatedItems, PwUuid uuidToSelect,
			out int iSelectIndex)
		{
			iSelectIndex = -1;

			if(pgContainer == null) { Debug.Assert(false); return; }
			if(cmb == null) { Debug.Assert(false); return; }
			// Do not clear the combobox!

			int iSelectInner = -1;
			GroupHandler gh = delegate(PwGroup pg)
			{
				string str = new string(' ', Math.Abs(8 * ((int)pg.GetLevel() - 1)));
				str += pg.Name;

				if((uuidToSelect != null) && pg.Uuid.Equals(uuidToSelect))
					iSelectInner = cmb.Items.Count;

				if(outCreatedItems != null)
					outCreatedItems[cmb.Items.Count] = pg.Uuid;

				cmb.Items.Add(str);
				return true;
			};

			pgContainer.TraverseTree(TraversalMethod.PreOrder, gh, null);
			iSelectIndex = iSelectInner;
		}

		public static void MakeInheritableBoolComboBox(ComboBox cmb, bool? bSelect,
			bool bInheritedState)
		{
			if(cmb == null) { Debug.Assert(false); return; }

			cmb.Items.Clear();
			cmb.Items.Add(KPRes.InheritSettingFromParent + " (" + (bInheritedState ?
				KPRes.Enabled : KPRes.Disabled) + ")");
			cmb.Items.Add(KPRes.Enabled);
			cmb.Items.Add(KPRes.Disabled);

			if(bSelect.HasValue) cmb.SelectedIndex = (bSelect.Value ? 1 : 2);
			else cmb.SelectedIndex = 0;
		}

		public static bool? GetInheritableBoolComboBoxValue(ComboBox cmb)
		{
			if(cmb == null) { Debug.Assert(false); return null; }

			if(cmb.SelectedIndex == 1) return true;
			if(cmb.SelectedIndex == 2) return false;
			return null;
		}

		public static void SetEnabled(Control c, bool bEnabled)
		{
			if(c == null) { Debug.Assert(false); return; }

			if(c.Enabled != bEnabled) c.Enabled = bEnabled;
		}

		internal static void SetEnabledFast(bool bEnabled, params ToolStripItem[] v)
		{
			if(v == null) { Debug.Assert(false); return; }

			foreach(ToolStripItem c in v)
			{
				if(c == null) { Debug.Assert(false); continue; }

				c.Enabled = bEnabled;
			}
		}

		public static void SetChecked(CheckBox cb, bool bChecked)
		{
			if(cb == null) { Debug.Assert(false); return; }

			if(cb.Checked != bChecked) cb.Checked = bChecked;
		}

		private static Bitmap GetGlyphBitmap(MenuGlyph mg, Color clrFG)
		{
			try
			{
				Size sz = new Size(DpiUtil.ScaleIntX(16), DpiUtil.ScaleIntY(16));

				Bitmap bmp = new Bitmap(sz.Width, sz.Height,
					PixelFormat.Format32bppArgb);
				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.Transparent);
					ControlPaint.DrawMenuGlyph(g, new Rectangle(Point.Empty,
						sz), mg, clrFG, Color.Transparent);
				}

				return bmp;
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		private static Bitmap g_bmpCheck = null;
		private static Bitmap g_bmpCheckLight = null;
		private static Bitmap g_bmpTrans = null;
		public static void SetChecked(ToolStripMenuItem tsmi, bool bChecked)
		{
			if(tsmi == null) { Debug.Assert(false); return; }

			const string strIDCheck = "guid:5EAEA440-02AA-4E62-B57E-724A6F89B1EE";
			const string strIDTrans = "guid:38DDF11D-F101-468A-A006-9810A95F34F4";

			// The image references may change, thus use the Tag instead;
			// https://sourceforge.net/p/keepass/discussion/329220/thread/e1950e60/
			bool bSetImage = false;
			Image imgCur = tsmi.Image;
			if(imgCur == null) bSetImage = true;
			else
			{
				string strID = (imgCur.Tag as string);
				if(strID == null) { } // Unknown image, don't overwrite
				else if((strID == strIDCheck) || (strID == strIDTrans))
					bSetImage = true;
			}

			if(bSetImage)
			{
				Image img = null;

				if(bChecked)
				{
					if(g_bmpCheck == null)
					{
						g_bmpCheck = new Bitmap(Properties.Resources.B16x16_MenuCheck);
						g_bmpCheck.Tag = strIDCheck;
					}

					img = g_bmpCheck;

					Color clrFG = tsmi.ForeColor;
					if(!clrFG.IsEmpty && (ColorToGrayscale(clrFG).R >= 128))
					{
						if(g_bmpCheckLight == null)
						{
							g_bmpCheckLight = GetGlyphBitmap(MenuGlyph.Checkmark,
								Color.White); // Not clrFG, for consistency

							if(g_bmpCheckLight != null)
							{
								Debug.Assert(g_bmpCheckLight.Tag == null);
								g_bmpCheckLight.Tag = strIDCheck;
							}
						}

						if(g_bmpCheckLight != null) img = g_bmpCheckLight;
					}
					else { Debug.Assert(g_bmpCheckLight == null); } // Always or never
				}
				else
				{
					if(g_bmpTrans == null)
					{
						g_bmpTrans = new Bitmap(Properties.Resources.B16x16_Transparent);
						g_bmpTrans.Tag = strIDTrans;
					}

					// Assign transparent image instead of null in order to
					// prevent incorrect menu item heights
					img = g_bmpTrans;
				}

				tsmi.Image = img;
			}

			tsmi.Checked = bChecked;
		}

		private static Bitmap g_bmpRadioLight = null;
		public static void SetRadioChecked(ToolStripMenuItem tsmi, bool bChecked)
		{
			if(tsmi == null) { Debug.Assert(false); return; }

			Debug.Assert(!tsmi.CheckOnClick); // Potential to break image

			if(bChecked)
			{
				Image imgCheck = Properties.Resources.B16x16_MenuRadio;

				Color clrFG = tsmi.ForeColor;
				if(!clrFG.IsEmpty && (ColorToGrayscale(clrFG).R >= 128))
				{
					if(g_bmpRadioLight == null)
						g_bmpRadioLight = GetGlyphBitmap(MenuGlyph.Bullet,
							Color.White); // Not clrFG, for consistency

					if(g_bmpRadioLight != null) imgCheck = g_bmpRadioLight;
				}
				else { Debug.Assert(g_bmpRadioLight == null); } // Always or never

				tsmi.Image = imgCheck;
				tsmi.CheckState = CheckState.Checked;
			}
			else
			{
				// Transparent: see SetChecked method
				tsmi.Image = Properties.Resources.B16x16_Transparent;
				tsmi.CheckState = CheckState.Unchecked;
			}
		}

		public static void ResizeColumns(ListView lv, bool bBlockUIUpdate)
		{
			ResizeColumns(lv, null, bBlockUIUpdate);
		}

		public static void ResizeColumns(ListView lv, int[] vRelWidths,
			bool bBlockUIUpdate)
		{
			if(lv == null) { Debug.Assert(false); return; }
			// vRelWidths may be null

			List<int> lCurWidths = new List<int>();
			foreach(ColumnHeader ch in lv.Columns) { lCurWidths.Add(ch.Width); }

			int n = lCurWidths.Count;
			if(n == 0) return;

			int[] vRels = new int[n];
			int nRelSum = 0;
			for(int i = 0; i < n; ++i)
			{
				if((vRelWidths != null) && (i < vRelWidths.Length))
				{
					if(vRelWidths[i] >= 0) vRels[i] = vRelWidths[i];
					else { Debug.Assert(false); vRels[i] = 0; }
				}
				else vRels[i] = 1; // Unit width 1 is default

				nRelSum += vRels[i];
			}
			if(nRelSum == 0) { Debug.Assert(false); return; }

			int w = lv.ClientSize.Width - 1;
			if(w <= 0) { Debug.Assert(false); return; }

			// The client width might include the width of a vertical
			// scrollbar or not (unreliable; for example the scrollbar
			// width is not subtracted during a Form.Load even though
			// a scrollbar is required); try to detect this situation
			int cwScroll = UIUtil.GetVScrollBarWidth();
			if((lv.Width - w) < cwScroll) // Scrollbar not already subtracted
			{
				int nItems = lv.Items.Count;
				if(nItems > 0)
				{
#if DEBUG
					foreach(ListViewItem lvi in lv.Items)
					{
						Debug.Assert(lvi.Bounds.Height == lv.Items[0].Bounds.Height);
					}
#endif

					if((((long)nItems * (long)lv.Items[0].Bounds.Height) >
						(long)lv.ClientSize.Height) && (w > cwScroll))
						w -= cwScroll;
				}
			}

			double dx = (double)w / (double)nRelSum;

			int[] vNewWidths = new int[n];
			int iFirstVisible = -1, wSum = 0;
			for(int i = 0; i < n; ++i)
			{
				int cw = (int)Math.Floor(dx * (double)vRels[i]);
				vNewWidths[i] = cw;

				wSum += cw;
				if((iFirstVisible < 0) && (cw > 0)) iFirstVisible = i;
			}

			if((iFirstVisible >= 0) && (wSum < w))
				vNewWidths[iFirstVisible] += (w - wSum);

			if(bBlockUIUpdate) lv.BeginUpdate();

			int iCol = 0;
			foreach(ColumnHeader ch in lv.Columns)
			{
				if(iCol >= n) { Debug.Assert(false); break; }

				if(vNewWidths[iCol] != lCurWidths[iCol])
					ch.Width = vNewWidths[iCol];

				++iCol;
			}
			Debug.Assert(iCol == n);

			if(bBlockUIUpdate) lv.EndUpdate();
		}

		public static bool ColorsEqual(Color c1, Color c2)
		{
			return ((c1.R == c2.R) && (c1.G == c2.G) && (c1.B == c2.B) &&
				(c1.A == c2.A));
		}

		public static Color GetAlternateColor(Color clrBase)
		{
			if(ColorsEqual(clrBase, Color.White)) return Color.FromArgb(238, 238, 255);

			float b = clrBase.GetBrightness();
			if(b >= 0.5) return UIUtil.DarkenColor(clrBase, 0.1);
			return UIUtil.LightenColor(clrBase, 0.25);
		}

		public static void SetAlternatingBgColors(ListView lv, Color clrAlternate,
			bool bAlternate)
		{
			if(lv == null) throw new ArgumentNullException("lv");

			Color clrBg = lv.BackColor;

			if(!UIUtil.GetGroupsEnabled(lv) || !bAlternate)
			{
				for(int i = 0; i < lv.Items.Count; ++i)
				{
					ListViewItem lvi = lv.Items[i];
					Debug.Assert(lvi.Index == i);
					Debug.Assert(lvi.UseItemStyleForSubItems);

					if(!bAlternate)
					{
						if(ColorsEqual(lvi.BackColor, clrAlternate))
							lvi.BackColor = clrBg;
					}
					else if(((i & 1) == 0) && ColorsEqual(lvi.BackColor, clrAlternate))
						lvi.BackColor = clrBg;
					else if(((i & 1) == 1) && ColorsEqual(lvi.BackColor, clrBg))
						lvi.BackColor = clrAlternate;
				}
			}
			else // Groups && alternating
			{
				foreach(ListViewGroup lvg in lv.Groups)
				{
					// Within the group the items are not in display order,
					// but the order can be derived from the item indices
					List<ListViewItem> lItems = new List<ListViewItem>();
					foreach(ListViewItem lviEnum in lvg.Items)
						lItems.Add(lviEnum);
					lItems.Sort(UIUtil.LviCompareByIndex);

					for(int i = 0; i < lItems.Count; ++i)
					{
						ListViewItem lvi = lItems[i];
						Debug.Assert(lvi.UseItemStyleForSubItems);

						if(((i & 1) == 0) && ColorsEqual(lvi.BackColor, clrAlternate))
							lvi.BackColor = clrBg;
						else if(((i & 1) == 1) && ColorsEqual(lvi.BackColor, clrBg))
							lvi.BackColor = clrAlternate;
					}
				}
			}
		}

		private static int LviCompareByIndex(ListViewItem a, ListViewItem b)
		{
			return a.Index.CompareTo(b.Index);
		}

		public static bool SetSortIcon(ListView lv, int iColumn, SortOrder so)
		{
			if(lv == null) { Debug.Assert(false); return false; }
			if(NativeLib.IsUnix()) return false;

			try
			{
				IntPtr hHeader = NativeMethods.SendMessage(lv.Handle,
					NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

				bool bUnicode = (WinUtil.IsWindows2000 || WinUtil.IsWindowsXP ||
					WinUtil.IsAtLeastWindowsVista);
				int nGetMsg = (bUnicode ? NativeMethods.HDM_GETITEMW :
					NativeMethods.HDM_GETITEMA);
				int nSetMsg = (bUnicode ? NativeMethods.HDM_SETITEMW :
					NativeMethods.HDM_SETITEMA);

				for(int i = 0; i < lv.Columns.Count; ++i)
				{
					IntPtr pColIndex = new IntPtr(i);

					NativeMethods.HDITEM hdItem = new NativeMethods.HDITEM();
					hdItem.mask = NativeMethods.HDI_FORMAT;

					if(NativeMethods.SendMessageHDItem(hHeader, nGetMsg, pColIndex,
						ref hdItem) == IntPtr.Zero) { Debug.Assert(false); }

					if((i != iColumn) || (so == SortOrder.None))
						hdItem.fmt &= (~NativeMethods.HDF_SORTUP &
							~NativeMethods.HDF_SORTDOWN);
					else
					{
						if(so == SortOrder.Ascending)
						{
							hdItem.fmt &= ~NativeMethods.HDF_SORTDOWN;
							hdItem.fmt |= NativeMethods.HDF_SORTUP;
						}
						else // SortOrder.Descending
						{
							hdItem.fmt &= ~NativeMethods.HDF_SORTUP;
							hdItem.fmt |= NativeMethods.HDF_SORTDOWN;
						}
					}

					Debug.Assert(hdItem.mask == NativeMethods.HDI_FORMAT);
					if(NativeMethods.SendMessageHDItem(hHeader, nSetMsg, pColIndex,
						ref hdItem) == IntPtr.Zero) { Debug.Assert(false); }
				}
			}
			catch(Exception) { Debug.Assert(false); return false; }

			return true;
		}

		public static void SetDisplayIndices(ListView lv, int[] v)
		{
			// Display indices must be assigned in an ordered way (with
			// respect to the display indices, not the column indices),
			// otherwise .NET's automatic adjustments result in
			// different display indices;
			// https://sourceforge.net/p/keepass/discussion/329221/thread/5e00cffe/

			if(lv == null) { Debug.Assert(false); return; }
			if(v == null) { Debug.Assert(false); return; }

			int nCols = lv.Columns.Count;
			int nMin = Math.Min(nCols, v.Length);

			SortedDictionary<int, int> d = new SortedDictionary<int, int>();
			for(int i = 0; i < nMin; ++i)
			{
				int nIdx = v[i];
				if((nIdx >= 0) && (nIdx < nCols)) d[nIdx] = i;
			}

			foreach(KeyValuePair<int, int> kvp in d)
				lv.Columns[kvp.Value].DisplayIndex = kvp.Key;

#if DEBUG
			int[] vNew = new int[nMin];
			for(int i = 0; i < nMin; ++i)
				vNew[i] = lv.Columns[i].DisplayIndex;
			Debug.Assert(StrUtil.SerializeIntArray(vNew) ==
				StrUtil.SerializeIntArray(MemUtil.Mid(v, 0, nMin)));
#endif
		}

		public static Color ColorToGrayscale(Color clr)
		{
			int l = (int)((0.3f * clr.R) + (0.59f * clr.G) + (0.11f * clr.B));
			if(l >= 256) l = 255;
			return Color.FromArgb(l, l, l);
		}

		public static Color ColorTowards(Color clr, Color clrBase, double dblFactor)
		{
			int l = (int)((0.3f * clrBase.R) + (0.59f * clrBase.G) + (0.11f * clrBase.B));

			if(l < 128) return DarkenColor(clr, dblFactor);
			return LightenColor(clr, dblFactor);
		}

		public static Color ColorTowardsGrayscale(Color clr, Color clrBase, double dblFactor)
		{
			return ColorToGrayscale(ColorTowards(clr, clrBase, dblFactor));
		}

		public static bool IsDarkColor(Color clr)
		{
			Color clrLvl = ColorToGrayscale(clr);
			return (clrLvl.R < 128);
		}

		public static Color ColorMiddle(Color clrA, Color clrB)
		{
			return Color.FromArgb(((int)clrA.A + (int)clrB.A) / 2,
				((int)clrA.R + (int)clrB.R) / 2,
				((int)clrA.G + (int)clrB.G) / 2,
				((int)clrA.B + (int)clrB.B) / 2);
		}

		public static GraphicsPath CreateRoundedRectangle(int x, int y, int dx, int dy,
			int r)
		{
			try
			{
				GraphicsPath gp = new GraphicsPath();

				gp.AddLine(x + r, y, x + dx - (r * 2), y);
				gp.AddArc(x + dx - (r * 2), y, r * 2, r * 2, 270.0f, 90.0f);
				gp.AddLine(x + dx, y + r, x + dx, y + dy - (r * 2));
				gp.AddArc(x + dx - (r * 2), y + dy - (r * 2), r * 2, r * 2, 0.0f, 90.0f);
				gp.AddLine(x + dx - (r * 2), y + dy, x + r, y + dy);
				gp.AddArc(x, y + dy - (r * 2), r * 2, r * 2, 90.0f, 90.0f);
				gp.AddLine(x, y + dy - (r * 2), x, y + r);
				gp.AddArc(x, y, r * 2, r * 2, 180.0f, 90.0f);

				gp.CloseFigure();
				return gp;
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		public static Control GetActiveControl(ContainerControl cc)
		{
			if(cc == null) { Debug.Assert(false); return null; }

			try
			{
				Control c = cc.ActiveControl;
				if(c == cc) return c;

				ContainerControl ccSub = (c as ContainerControl);
				if(ccSub != null) return GetActiveControl(ccSub);
				else return c;
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		public static void ApplyKeyUIFlags(ulong aceUIFlags, CheckBox cbPassword,
			CheckBox cbKeyFile, CheckBox cbUserAccount, CheckBox cbHidePassword)
		{
			if((aceUIFlags & (ulong)AceKeyUIFlags.EnablePassword) != 0)
				UIUtil.SetEnabled(cbPassword, true);
			if((aceUIFlags & (ulong)AceKeyUIFlags.EnableKeyFile) != 0)
				UIUtil.SetEnabled(cbKeyFile, true);
			if((aceUIFlags & (ulong)AceKeyUIFlags.EnableUserAccount) != 0)
				UIUtil.SetEnabled(cbUserAccount, true);
			if((aceUIFlags & (ulong)AceKeyUIFlags.EnableHidePassword) != 0)
				UIUtil.SetEnabled(cbHidePassword, true);

			if((aceUIFlags & (ulong)AceKeyUIFlags.CheckPassword) != 0)
				UIUtil.SetChecked(cbPassword, true);
			if((aceUIFlags & (ulong)AceKeyUIFlags.CheckKeyFile) != 0)
				UIUtil.SetChecked(cbKeyFile, true);
			if((aceUIFlags & (ulong)AceKeyUIFlags.CheckUserAccount) != 0)
				UIUtil.SetChecked(cbUserAccount, true);
			if((aceUIFlags & (ulong)AceKeyUIFlags.CheckHidePassword) != 0)
				UIUtil.SetChecked(cbHidePassword, true);

			if((aceUIFlags & (ulong)AceKeyUIFlags.UncheckPassword) != 0)
				UIUtil.SetChecked(cbPassword, false);
			if((aceUIFlags & (ulong)AceKeyUIFlags.UncheckKeyFile) != 0)
				UIUtil.SetChecked(cbKeyFile, false);
			if((aceUIFlags & (ulong)AceKeyUIFlags.UncheckUserAccount) != 0)
				UIUtil.SetChecked(cbUserAccount, false);
			if((aceUIFlags & (ulong)AceKeyUIFlags.UncheckHidePassword) != 0)
				UIUtil.SetChecked(cbHidePassword, false);

			if((aceUIFlags & (ulong)AceKeyUIFlags.DisablePassword) != 0)
				UIUtil.SetEnabled(cbPassword, false);
			if((aceUIFlags & (ulong)AceKeyUIFlags.DisableKeyFile) != 0)
				UIUtil.SetEnabled(cbKeyFile, false);
			if((aceUIFlags & (ulong)AceKeyUIFlags.DisableUserAccount) != 0)
				UIUtil.SetEnabled(cbUserAccount, false);
			if((aceUIFlags & (ulong)AceKeyUIFlags.DisableHidePassword) != 0)
				UIUtil.SetEnabled(cbHidePassword, false);
		}

		public static bool GetGroupsEnabled(ListView lv)
		{
			if(lv == null) { Debug.Assert(false); return false; }

			// Corresponds almost with the internal GroupsEnabled property
			return (lv.ShowGroups && (lv.Groups.Count > 0) && !lv.VirtualMode);
		}

		public static int GetMaxVisibleItemCount(ListView lv)
		{
			if(lv == null) { Debug.Assert(false); return 0; }

			int hClient = lv.ClientSize.Height;
			int hHeader = NativeMethods.GetHeaderHeight(lv);
			if(hHeader > 0)
			{
				if(((lv.Height - hClient) < hHeader) && (hClient > hHeader))
					hClient -= hHeader;
			}

			int dy = lv.Items[0].Bounds.Height;
			if(dy <= 1) { Debug.Assert(false); dy = DpiUtil.ScaleIntY(16) + 1; }

			return (hClient / dy);
		}

		public static int GetTopVisibleItem(ListView lv)
		{
			if(lv == null) { Debug.Assert(false); return -1; }

			// The returned value must be an existing index or -1
			int nRes = -1;
			try
			{
				if(lv.Items.Count == 0) return nRes;

				ListViewItem lvi = null;
				if(!UIUtil.GetGroupsEnabled(lv)) lvi = lv.TopItem;
				else
				{
					// In grouped mode, the TopItem property does not work;
					// http://connect.microsoft.com/VisualStudio/feedback/details/642188/listview-control-bug-topitem-property-doesnt-work-with-groups
					// http://msdn.microsoft.com/en-us/library/windows/desktop/bb761087.aspx

					int dyHeader = NativeMethods.GetHeaderHeight(lv);

					int yMin = int.MaxValue;
					foreach(ListViewItem lviEnum in lv.Items)
					{
						int yEnum = Math.Abs(lviEnum.Position.Y - dyHeader);
						if(yEnum < yMin)
						{
							yMin = yEnum;
							lvi = lviEnum;
						}
					}
				}

				if(lvi != null) nRes = lvi.Index;
			}
			catch(Exception) { Debug.Assert(false); }

			return nRes;
		}

		public static void SetTopVisibleItem(ListView lv, int iIndex)
		{
			SetTopVisibleItem(lv, iIndex, false);
		}

		public static void SetTopVisibleItem(ListView lv, int iIndex,
			bool bEnsureSelectedVisible)
		{
			if(lv == null) { Debug.Assert(false); return; }
			if(iIndex < 0) return; // No assert

			int n = lv.Items.Count;
			if(n <= 0) return;

			if(iIndex >= n) iIndex = n - 1; // No assert

			int iOrgTop = GetTopVisibleItem(lv);

			lv.BeginUpdate(); // Might reduce flicker

			if(iIndex != iOrgTop) // Prevent unnecessary flicker
			{
				// Setting lv.TopItem doesn't work properly
				
				// lv.EnsureVisible(n - 1);
				// if(iIndex != (n - 1)) lv.EnsureVisible(iIndex);

#if DEBUG
				foreach(ListViewItem lvi in lv.Items)
				{
					Debug.Assert(lvi.Bounds.Height == lv.Items[0].Bounds.Height);
				}
#endif

				if(iIndex < iOrgTop)
					lv.EnsureVisible(iIndex);
				else
				{
					int nVisItems = GetMaxVisibleItemCount(lv);

					int iNewLast = nVisItems + iIndex - 1;
					if(iNewLast < 0) { Debug.Assert(false); iNewLast = 0; }
					iNewLast = Math.Min(iNewLast, n - 1);

					lv.EnsureVisible(iNewLast);
					
					int iNewTop = GetTopVisibleItem(lv);
					if(iNewTop > iIndex) // Scrolled too far
					{
						Debug.Assert(false);
						lv.EnsureVisible(iIndex); // Scroll back
					}
					else if(iNewTop == iIndex) { } // Perfect
					else { Debug.Assert(iNewLast == (n - 1)); }

					// int hItem = lv.Items[0].Bounds.Height;
					// if(hItem <= 0) { Debug.Assert(false); hItem = DpiUtil.ScaleIntY(16) + 1; }
					// int hToScroll = (iIndex - iOrgTop) * hItem;
					// NativeMethods.Scroll(lv, 0, hToScroll);
					// int iNewTop = GetTopVisibleItem(lv);
					// if(iNewTop > iIndex) // Scrolled too far
					// {
					//	Debug.Assert(false);
					//	lv.EnsureVisible(iIndex); // Scroll back
					// }
					// else if(iNewTop == iIndex) { } // Perfect
					// else
					// {
					//	lv.EnsureVisible(n - 1);
					//	if(iIndex != (n - 1)) lv.EnsureVisible(iIndex);
					// }
				}
			}

			if(bEnsureSelectedVisible)
			{
				ListView.SelectedIndexCollection lvsic = lv.SelectedIndices;
				int nSel = lvsic.Count;
				if(nSel > 0)
				{
					int[] vSel = new int[nSel];
					lvsic.CopyTo(vSel, 0);

					lv.EnsureVisible(vSel[nSel - 1]);
					if(nSel >= 2) lv.EnsureVisible(vSel[0]);
				}
			}

			lv.EndUpdate();
		}

		public static UIScrollInfo GetScrollInfo(ListView lv,
			bool bForRestoreOnly)
		{
			if(lv == null) { Debug.Assert(false); return null; }

			int scrY = NativeMethods.GetScrollPosY(lv.Handle);
			int idxTop = GetTopVisibleItem(lv);

			// Fix index-based scroll position
			if((scrY == idxTop) && (idxTop > 0))
			{
				// Groups imply pixel position
				Debug.Assert(!UIUtil.GetGroupsEnabled(lv));

				if(!bForRestoreOnly)
				{
					int hSum = 0;
					foreach(ListViewItem lvi in lv.Items)
					{
						if(scrY <= 0) break;

						int hItem = lvi.Bounds.Height;
						if(hItem > 1) hSum += hItem;
						else { Debug.Assert(false); }

						--scrY;
					}

					scrY = hSum;
				}
				else scrY = 0; // Pixels not required for restoration
			}

			return new UIScrollInfo(0, scrY, idxTop);
		}

		public static void Scroll(ListView lv, UIScrollInfo s,
			bool bEnsureSelectedVisible)
		{
			if(lv == null) { Debug.Assert(false); return; }
			if(s == null) { Debug.Assert(false); return; }

			if(UIUtil.GetGroupsEnabled(lv) && !NativeLib.IsUnix())
			{
				// Only works correctly when groups are present
				// (lv.ShowGroups is not sufficient)
				NativeMethods.Scroll(lv, s.ScrollX, s.ScrollY);

				if(bEnsureSelectedVisible)
				{
					Debug.Assert(false); // Unsupported mode combination
					SetTopVisibleItem(lv, GetTopVisibleItem(lv), true);
				}
			}
			else SetTopVisibleItem(lv, s.TopIndex, bEnsureSelectedVisible);
		}

		/// <summary>
		/// Test whether a screen area is at least partially visible.
		/// </summary>
		/// <param name="rect">Area to test.</param>
		/// <returns>Returns <c>true</c>, if the area is at least partially
		/// visible. Otherwise, <c>false</c> is returned.</returns>
		public static bool IsScreenAreaVisible(Rectangle rect)
		{
			try
			{
				foreach(Screen scr in Screen.AllScreens)
				{
					Rectangle scrBounds = scr.Bounds;

					if((rect.Left > scrBounds.Right) || (rect.Right < scrBounds.Left) ||
						(rect.Top > scrBounds.Bottom) || (rect.Bottom < scrBounds.Top)) { }
					else return true;
				}
			}
			catch(Exception) { Debug.Assert(false); return true; }

			return false;
		}

		public static void EnsureInsideScreen(Form f)
		{
			if(f == null) { Debug.Assert(false); return; }

			try
			{
				if(!f.Visible) return; // No assert
				if(f.WindowState != FormWindowState.Normal) return;

				int x = f.Location.X;
				int y = f.Location.Y;
				int w = f.Size.Width;
				int h = f.Size.Height;

				Debug.Assert((x != -32000) && (x != -64000));
				Debug.Assert(x != AppDefs.InvalidWindowValue);
				Debug.Assert((y != -32000) && (y != -64000));
				Debug.Assert(y != AppDefs.InvalidWindowValue);
				Debug.Assert(w != AppDefs.InvalidWindowValue);
				Debug.Assert(h != AppDefs.InvalidWindowValue);

				Rectangle rect = new Rectangle(x, y, w, h);
				if(IsScreenAreaVisible(rect)) return;

				Screen scr = Screen.PrimaryScreen;
				Rectangle rectScr = scr.Bounds;
				BoundsSpecified bs = BoundsSpecified.Location;

				if((w > rectScr.Width) || (h > rectScr.Height))
				{
					w = Math.Min(w, rectScr.Width);
					h = Math.Min(h, rectScr.Height);
					bs |= BoundsSpecified.Size;
				}

				x = rectScr.X + ((rectScr.Width - w) / 2);
				y = rectScr.Y + ((rectScr.Height - h) / 2);

				f.SetBounds(x, y, w, h, bs);				
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static string GetWindowScreenRect(Form f)
		{
			if(f == null) { Debug.Assert(false); return string.Empty; }

			StringBuilder sb = new StringBuilder();

			Point ptLocation = f.Location;
			sb.Append(ptLocation.X);
			sb.Append(", ");
			sb.Append(ptLocation.Y);

			if((f.FormBorderStyle == FormBorderStyle.Sizable) ||
				(f.FormBorderStyle == FormBorderStyle.SizableToolWindow))
			{
				Size szSize = f.Size;
				sb.Append(", ");
				sb.Append(szSize.Width);
				sb.Append(", ");
				sb.Append(szSize.Height);
			}

			return sb.ToString();
		}

		public static void SetWindowScreenRect(Form f, string strScreenRect)
		{
			if((f == null) || (strScreenRect == null)) { Debug.Assert(false); return; }

			string[] v = strScreenRect.Split(new char[] { ',', ' ' },
				StringSplitOptions.RemoveEmptyEntries);

			if(v.Length == 4)
			{
				int x, y, w, h;
				if(int.TryParse(v[0], out x) && int.TryParse(v[1], out y) &&
					int.TryParse(v[2], out w) && int.TryParse(v[3], out h))
				{
					Rectangle rect = new Rectangle(x, y, w, h);
					if(UIUtil.IsScreenAreaVisible(rect))
					{
						f.Location = new Point(x, y);
						f.Size = new Size(w, h);
					}
				}
				else { Debug.Assert(false); }
			}
			else if(v.Length == 2)
			{
				int x, y;
				if(int.TryParse(v[0], out x) && int.TryParse(v[1], out y))
				{
					Size sz = f.Size;
					Rectangle rect = new Rectangle(x, y, sz.Width, sz.Height);
					if(UIUtil.IsScreenAreaVisible(rect))
						f.Location = new Point(x, y);
				}
				else { Debug.Assert(false); }
			}
			else { Debug.Assert(false); }
		}

		public static string GetColumnWidths(ListView lv)
		{
			if(lv == null) { Debug.Assert(false); return string.Empty; }

			int n = lv.Columns.Count;
			int[] vSizes = new int[n];
			for(int i = 0; i < n; ++i) vSizes[i] = lv.Columns[i].Width;

			return StrUtil.SerializeIntArray(vSizes);
		}

		public static void SetColumnWidths(ListView lv, string strSizes)
		{
			if(string.IsNullOrEmpty(strSizes)) return; // No assert

			int[] vSizes = StrUtil.DeserializeIntArray(strSizes);

			int n = lv.Columns.Count;
			Debug.Assert(n == vSizes.Length);

			for(int i = 0; i < Math.Min(n, vSizes.Length); ++i)
				lv.Columns[i].Width = vSizes[i];
		}

		public static void SetButtonImage(Button btn, Image img, bool b16To15)
		{
			if(btn == null) { Debug.Assert(false); return; }
			if(img == null) { Debug.Assert(false); return; }

			if(b16To15 && (btn.Height == 23) && (img.Height == 16))
				btn.Image = GfxUtil.ScaleImage(img, img.Width, 15,
					ScaleTransformFlags.UIIcon);
			else btn.Image = img;
		}

		public static void EnableAutoCompletion(ComboBox cb, bool bAlsoAutoAppend)
		{
			if(cb == null) { Debug.Assert(false); return; }

			Debug.Assert(cb.DropDownStyle != ComboBoxStyle.DropDownList);

			cb.AutoCompleteMode = (bAlsoAutoAppend ? AutoCompleteMode.SuggestAppend :
				AutoCompleteMode.Suggest);
			cb.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		public static void EnableAutoCompletion(ToolStripComboBox cb, bool bAlsoAutoAppend)
		{
			if(cb == null) { Debug.Assert(false); return; }

			Debug.Assert(cb.DropDownStyle != ComboBoxStyle.DropDownList);

			cb.AutoCompleteMode = (bAlsoAutoAppend ? AutoCompleteMode.SuggestAppend :
				AutoCompleteMode.Suggest);
			cb.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		public static void SetFocus(Control c, Form fParent)
		{
			if(c == null) { Debug.Assert(false); return; }
			// fParent may be null

			try
			{
				if(fParent != null) fParent.ActiveControl = c;
			}
			catch(Exception) { Debug.Assert(false); }

			try
			{
				if(c.CanSelect) c.Select();
				if(c.CanFocus) c.Focus();
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void ResetFocus(Control c, Form fParent)
		{
			if(c == null) { Debug.Assert(false); return; }
			// fParent may be null

			try
			{
				Control cPre = null;
				if(fParent != null) cPre = fParent.ActiveControl;

				bool bStdSetFocus = true;
				if(c == cPre)
				{
					// Special reset for password text boxes that
					// can show a Caps Lock balloon tip;
					// https://sourceforge.net/p/keepass/feature-requests/1905/
					TextBox tb = (c as TextBox);
					if((tb != null) && !NativeLib.IsUnix())
					{
						IntPtr h = tb.Handle;
						bool bCapsLock = ((NativeMethods.GetKeyState(
							NativeMethods.VK_CAPITAL) & 1) != 0);

						if((h != IntPtr.Zero) && tb.UseSystemPasswordChar &&
							!tb.ReadOnly && bCapsLock)
						{
							NativeMethods.SendMessage(h, NativeMethods.WM_KILLFOCUS,
								IntPtr.Zero, IntPtr.Zero);
							NativeMethods.SendMessage(h, NativeMethods.WM_SETFOCUS,
								IntPtr.Zero, IntPtr.Zero);

							bStdSetFocus = false;
						}
					}
				}

				if(bStdSetFocus) UIUtil.SetFocus(c, fParent);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		/// <summary>
		/// Show a modal dialog and destroy it afterwards.
		/// </summary>
		/// <param name="f">Form to show and destroy.</param>
		/// <returns>Result from <c>ShowDialog</c>.</returns>
		public static DialogResult ShowDialogAndDestroy(Form f)
		{
			if(f == null) { Debug.Assert(false); return DialogResult.None; }

			DialogResult dr = f.ShowDialog();
			UIUtil.DestroyForm(f);
			return dr;
		}

		/// <summary>
		/// Show a modal dialog. If the result isn't the specified value, the
		/// dialog is disposed and <c>true</c> is returned. Otherwise, <c>false</c>
		/// is returned (without disposing the dialog!).
		/// </summary>
		/// <param name="f">Dialog to show.</param>
		/// <param name="drNotValue">Comparison value.</param>
		/// <returns>See description.</returns>
		public static bool ShowDialogNotValue(Form f, DialogResult drNotValue)
		{
			if(f == null) { Debug.Assert(false); return true; }

			if(f.ShowDialog() != drNotValue)
			{
				UIUtil.DestroyForm(f);
				return true;
			}

			return false;
		}

		public static void DestroyForm(Form f)
		{
			if(f == null) { Debug.Assert(false); return; }

			try
			{
				// f.Close(); // Don't trigger closing events
				f.Dispose();
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static Image ExtractVistaIcon(Icon ico)
		{
			if(ico == null) { Debug.Assert(false); return null; }

			MemoryStream ms = new MemoryStream();
			try
			{
				ico.Save(ms);
				byte[] pb = ms.ToArray();

				return GfxUtil.LoadImage(pb); // Extracts best image from ICO
			}
			catch { Debug.Assert(false); }
			finally { ms.Close(); }

			return null;
		}

		public static void ColorToHsv(Color clr, out float fHue,
			out float fSaturation, out float fValue)
		{
			int nMax = Math.Max(clr.R, Math.Max(clr.G, clr.B));
			int nMin = Math.Min(clr.R, Math.Min(clr.G, clr.B));

			fHue = clr.GetHue(); // In degrees
			fSaturation = ((nMax == 0) ? 0.0f : (1.0f - ((float)nMin / nMax)));
			fValue = (float)nMax / 255.0f;
		}

		public static Color ColorFromHsv(float fHue, float fSaturation, float fValue)
		{
			float d = fHue / 60;
			float fl = (float)Math.Floor(d);
			float f = d - fl;

			fValue *= 255.0f;
			int v = (int)fValue;
			int p = (int)(fValue * (1.0f - fSaturation));
			int q = (int)(fValue * (1.0f - (fSaturation * f)));
			int t = (int)(fValue * (1.0f - (fSaturation * (1.0f - f))));

			try
			{
				int hi = (int)fl % 6;
				if(hi == 0) return Color.FromArgb(255, v, t, p);
				if(hi == 1) return Color.FromArgb(255, q, v, p);
				if(hi == 2) return Color.FromArgb(255, p, v, t);
				if(hi == 3) return Color.FromArgb(255, p, q, v);
				if(hi == 4) return Color.FromArgb(255, t, p, v);

				return Color.FromArgb(255, v, p, q);
			}
			catch(Exception) { Debug.Assert(false); }

			return Color.Transparent;
		}

		public static Icon CreateColorizedIcon(Icon icoBase, Color clr, int qSize)
		{
			if(icoBase == null) { Debug.Assert(false); return null; }

			if(qSize <= 0) qSize = 48; // Large shell icon size

			try
			{
				Bitmap bmp = new Bitmap(qSize, qSize, PixelFormat.Format32bppArgb);
				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.Transparent);

					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.SmoothingMode = SmoothingMode.HighQuality;

					bool bDrawDefault = true;
					if((qSize != 16) && (qSize != 32))
					{
						Image imgIco = ExtractVistaIcon(icoBase);
						if(imgIco != null)
						{
							// g.DrawImage(imgIco, 0, 0, bmp.Width, bmp.Height);
							using(Image imgSc = GfxUtil.ScaleImage(imgIco,
								bmp.Width, bmp.Height, ScaleTransformFlags.UIIcon))
							{
								g.DrawImageUnscaled(imgSc, 0, 0);
							}

							imgIco.Dispose();
							bDrawDefault = false;
						}
					}

					if(bDrawDefault)
					{
						Icon icoSc = null;
						try
						{
							icoSc = new Icon(icoBase, bmp.Width, bmp.Height);
							g.DrawIcon(icoSc, new Rectangle(0, 0, bmp.Width,
								bmp.Height));
						}
						catch(Exception)
						{
							Debug.Assert(false);
							g.DrawIcon(icoBase, new Rectangle(0, 0, bmp.Width,
								bmp.Height));
						}
						finally { if(icoSc != null) icoSc.Dispose(); }
					}

					// IntPtr hdc = g.GetHdc();
					// NativeMethods.DrawIconEx(hdc, 0, 0, icoBase.Handle, bmp.Width,
					//	bmp.Height, 0, IntPtr.Zero, NativeMethods.DI_NORMAL);
					// g.ReleaseHdc(hdc);
				}

				BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width,
					bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				int nBytes = Math.Abs(bd.Stride * bmp.Height);
				byte[] pbArgb = new byte[nBytes];
				Marshal.Copy(bd.Scan0, pbArgb, 0, nBytes);

				float fHue, fSaturation, fValue;
				ColorToHsv(clr, out fHue, out fSaturation, out fValue);

				for(int i = 0; i < nBytes; i += 4)
				{
					if(pbArgb[i + 3] == 0) continue; // Transparent
					if((pbArgb[i] == pbArgb[i + 1]) && (pbArgb[i] == pbArgb[i + 2]))
						continue; // Gray

					Color clrPixel = Color.FromArgb((int)pbArgb[i + 2],
						(int)pbArgb[i + 1], (int)pbArgb[i]); // BGRA

					float h, s, v;
					ColorToHsv(clrPixel, out h, out s, out v);

					Color clrNew = ColorFromHsv(fHue, s, v);

					pbArgb[i] = clrNew.B;
					pbArgb[i + 1] = clrNew.G;
					pbArgb[i + 2] = clrNew.R;
				}

				Marshal.Copy(pbArgb, 0, bd.Scan0, nBytes);
				bmp.UnlockBits(bd);

				IntPtr hIcon = bmp.GetHicon();
				Icon icoBmp = Icon.FromHandle(hIcon);

				Icon icoResult = (Icon)icoBmp.Clone();

				try { NativeMethods.DestroyIcon(hIcon); }
				catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
				bmp.Dispose();

				return icoResult;
			}
			catch(Exception) { Debug.Assert(false); }

			return (Icon)icoBase.Clone();
		}

		public static Bitmap InvertImage(Image img)
		{
			try
			{
				int w = img.Width, h = img.Height;

				Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.Transparent);

					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.SmoothingMode = SmoothingMode.None;

					ColorMatrix cm = new ColorMatrix(new float[][] {
						new float[] { -1, 0, 0, 0, 0 },
						new float[] { 0, -1, 0, 0, 0 },
						new float[] { 0, 0, -1, 0, 0 },
						new float[] { 0, 0, 0, 1, 0 },
						new float[] { 1, 1, 1, 0, 1 }
					});

					ImageAttributes a = new ImageAttributes();
					a.SetColorMatrix(cm);

					g.DrawImage(img, new Rectangle(0, 0, w, h), 0, 0, w, h,
						GraphicsUnit.Pixel, a);
				}

				return bmp;
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		public static Image CreateTabColorImage(Color clr, TabControl cTab)
		{
			if(cTab == null) { Debug.Assert(false); return null; }

			int qSize = cTab.ItemSize.Height - 3;
			if(MonoWorkarounds.IsRequired()) qSize -= 1;
			if(qSize < 4) { Debug.Assert(false); return null; }

			const int dyTrans = 3;
			int yCenter = (qSize - dyTrans) / 2 + dyTrans;
			Rectangle rectTop = new Rectangle(0, dyTrans, qSize, yCenter - dyTrans);
			Rectangle rectBottom = new Rectangle(0, yCenter, qSize, qSize - yCenter);

			Color clrLight = UIUtil.LightenColor(clr, 0.5);
			Color clrDark = UIUtil.DarkenColor(clr, 0.1);

			Bitmap bmp = new Bitmap(qSize, qSize, PixelFormat.Format32bppArgb);
			using(Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.Transparent);

				using(LinearGradientBrush brLight = new LinearGradientBrush(
					rectTop, clrLight, clr, LinearGradientMode.Vertical))
				{
					g.FillRectangle(brLight, rectTop);
				}

				using(LinearGradientBrush brDark = new LinearGradientBrush(
					rectBottom, clr, clrDark, LinearGradientMode.Vertical))
				{
					g.FillRectangle(brDark, rectBottom);
				}
			}

			return bmp;
		}

		public static bool PlayUacSound()
		{
			try
			{
				string strRoot = "HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\.Default\\WindowsUAC\\";

				string strWav = (Registry.GetValue(strRoot + ".Current",
					string.Empty, string.Empty) as string);
				if(string.IsNullOrEmpty(strWav))
					strWav = (Registry.GetValue(strRoot + ".Default",
						string.Empty, string.Empty) as string);
				if(string.IsNullOrEmpty(strWav))
					strWav = @"%SystemRoot%\Media\Windows User Account Control.wav";

				strWav = SprEngine.Compile(strWav, null);

				if(!File.Exists(strWav)) throw new FileNotFoundException();

				NativeMethods.PlaySound(strWav, IntPtr.Zero, NativeMethods.SND_FILENAME |
					NativeMethods.SND_ASYNC | NativeMethods.SND_NODEFAULT);
				return true;
			}
			catch(Exception) { }

			Debug.Assert(NativeLib.IsUnix() || !WinUtil.IsAtLeastWindowsVista);
			// Do not play a standard sound here
			return false;
		}

		public static Image GetWindowImage(IntPtr hWnd, bool bPrefSmall)
		{
			try
			{
				IntPtr hIcon;
				if(bPrefSmall)
				{
					hIcon = NativeMethods.SendMessage(hWnd, NativeMethods.WM_GETICON,
						new IntPtr(NativeMethods.ICON_SMALL2), IntPtr.Zero);
					if(hIcon != IntPtr.Zero) return Icon.FromHandle(hIcon).ToBitmap();
				}

				hIcon = NativeMethods.SendMessage(hWnd, NativeMethods.WM_GETICON,
					new IntPtr(bPrefSmall ? NativeMethods.ICON_SMALL :
					NativeMethods.ICON_BIG), IntPtr.Zero);
				if(hIcon != IntPtr.Zero) return Icon.FromHandle(hIcon).ToBitmap();

				hIcon = NativeMethods.GetClassLongPtr(hWnd, bPrefSmall ?
					NativeMethods.GCLP_HICONSM : NativeMethods.GCLP_HICON);
				if(hIcon != IntPtr.Zero) return Icon.FromHandle(hIcon).ToBitmap();

				hIcon = NativeMethods.SendMessage(hWnd, NativeMethods.WM_GETICON,
					new IntPtr(bPrefSmall ? NativeMethods.ICON_BIG : NativeMethods.ICON_SMALL),
					IntPtr.Zero);
				if(hIcon != IntPtr.Zero) return Icon.FromHandle(hIcon).ToBitmap();

				hIcon = NativeMethods.GetClassLongPtr(hWnd, bPrefSmall ?
					NativeMethods.GCLP_HICON : NativeMethods.GCLP_HICONSM);
				if(hIcon != IntPtr.Zero) return Icon.FromHandle(hIcon).ToBitmap();

				if(!bPrefSmall)
				{
					hIcon = NativeMethods.SendMessage(hWnd, NativeMethods.WM_GETICON,
						new IntPtr(NativeMethods.ICON_SMALL2), IntPtr.Zero);
					if(hIcon != IntPtr.Zero) return Icon.FromHandle(hIcon).ToBitmap();
				}
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		/// <summary>
		/// Set the state of a window. This is a workaround for
		/// https://sourceforge.net/projects/keepass/forums/forum/329221/topic/4610118
		/// </summary>
		public static void SetWindowState(Form f, FormWindowState fws)
		{
			if(f == null) { Debug.Assert(false); return; }

			f.WindowState = fws;

			// If the window state change / resize handler changes
			// the window state again, the property gets out of sync
			// with the real state. Therefore, we now make sure that
			// the property is synchronized with the actual window
			// state.
			try
			{
				// Get the value that .NET currently caches; note
				// this isn't necessarily the real window state
				// due to the bug
				FormWindowState fwsCached = f.WindowState;

				IntPtr hWnd = f.Handle;
				if(hWnd == IntPtr.Zero) { Debug.Assert(false); return; }

				// Get the real state using Windows API functions
				bool bIsRealMin = NativeMethods.IsIconic(hWnd);
				bool bIsRealMax = NativeMethods.IsZoomed(hWnd);

				FormWindowState? fwsFix = null;
				if(bIsRealMin && (fwsCached != FormWindowState.Minimized))
					fwsFix = FormWindowState.Minimized;
				else if(bIsRealMax && (fwsCached != FormWindowState.Maximized) &&
					!bIsRealMin)
					fwsFix = FormWindowState.Maximized;
				else if(!bIsRealMin && !bIsRealMax &&
					(fwsCached != FormWindowState.Normal))
					fwsFix = FormWindowState.Normal;

				if(fwsFix.HasValue)
				{
					// If the window is invisible, no state
					// change / resize handlers are called
					bool bVisible = f.Visible;
					if(bVisible) f.Visible = false;
					f.WindowState = fwsFix.Value;
					if(bVisible) f.Visible = true;
				}
			}
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
		}

		private static KeysConverter m_convKeys = null;
		public static string GetKeysName(Keys k)
		{
			if(m_convKeys == null) m_convKeys = new KeysConverter();
			return m_convKeys.ConvertToString(k);
		}

		/// <summary>
		/// Assign shortcut keys to a menu item. This method uses
		/// custom-translated display strings.
		/// </summary>
		public static void AssignShortcut(ToolStripMenuItem tsmi, Keys k)
		{
			if(tsmi == null) { Debug.Assert(false); return; }

			tsmi.ShortcutKeys = k;

			string str = string.Empty;
			if((k & Keys.Control) != Keys.None)
				str += KPRes.KeyboardKeyCtrl + "+";
			if((k & Keys.Alt) != Keys.None)
				str += KPRes.KeyboardKeyAlt + "+";
			if((k & Keys.Shift) != Keys.None)
				str += KPRes.KeyboardKeyShift + "+";
			str += GetKeysName(k & Keys.KeyCode);

			tsmi.ShortcutKeyDisplayString = str;
		}

		/* public static string ImageToDataUri(Image img)
		{
			if(img == null) { Debug.Assert(false); return string.Empty; }

			MemoryStream ms = new MemoryStream();
			img.Save(ms, ImageFormat.Png);

			byte[] pbImage = ms.ToArray();
			ms.Close();

			return StrUtil.DataToDataUri(pbImage, "image/png");
		} */

		public static void SetFocusedItem(ListView lv, ListViewItem lvi,
			bool bAlsoSelect)
		{
			if((lv == null) || (lvi == null)) { Debug.Assert(false); return; }

			if(bAlsoSelect) lvi.Selected = true;

			try { lv.FocusedItem = lvi; } // .NET
			catch(Exception)
			{
				try { lvi.Focused = true; } // Mono
				catch(Exception) { Debug.Assert(false); }
			}
		}

		public static Image CreateDropDownImage(Image imgBase)
		{
			if(imgBase == null) { Debug.Assert(false); return null; }

			// Height of the arrow without the glow effect
			// int hArrow = (int)Math.Ceiling((double)DpiUtil.ScaleIntY(20) / 8.0);
			// int hArrow = (int)Math.Ceiling((double)DpiUtil.ScaleIntY(28) / 8.0);
			int hArrow = DpiUtil.ScaleIntY(3);

			int dx = imgBase.Width, dy = imgBase.Height;
			if((dx < ((hArrow * 2) + 2)) || (dy < (hArrow + 2)))
				return new Bitmap(imgBase);

			bool bStdClr = !UIUtil.IsDarkTheme;

			Bitmap bmp = new Bitmap(dx, dy, PixelFormat.Format32bppArgb);
			using(Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.Transparent);
				g.DrawImageUnscaled(imgBase, 0, 0);

				// Pen penDark = Pens.Black;
				// g.DrawLine(penDark, dx - 5, dy - 3, dx - 1, dy - 3);
				// g.DrawLine(penDark, dx - 4, dy - 2, dx - 2, dy - 2);
				// // g.DrawLine(penDark, dx - 7, dy - 4, dx - 1, dy - 4);
				// // g.DrawLine(penDark, dx - 6, dy - 3, dx - 2, dy - 3);
				// // g.DrawLine(penDark, dx - 5, dy - 2, dx - 3, dy - 2);
				// using(Pen penLight = new Pen(Color.FromArgb(
				//	160, 255, 255, 255), 1.0f))
				// {
				//	g.DrawLine(penLight, dx - 5, dy - 4, dx - 1, dy - 4);
				//	g.DrawLine(penLight, dx - 6, dy - 3, dx - 4, dy - 1);
				//	g.DrawLine(penLight, dx - 2, dy - 1, dx - 1, dy - 2);
				//	// g.DrawLine(penLight, dx - 7, dy - 5, dx - 1, dy - 5);
				//	// g.DrawLine(penLight, dx - 8, dy - 4, dx - 5, dy - 1);
				//	// g.DrawLine(penLight, dx - 3, dy - 1, dx - 1, dy - 3);
				// }

				g.SmoothingMode = SmoothingMode.None;

				Pen penDark = (bStdClr ? Pens.Black : Pens.White);
				for(int i = 1; i < hArrow; ++i)
					g.DrawLine(penDark, dx - hArrow - i, dy - 1 - i,
						dx - hArrow + i, dy - 1 - i);

				int c = (bStdClr ? 255 : 0);
				using(Pen penLight = new Pen(Color.FromArgb(160, c, c, c), 1.0f))
				{
					g.DrawLine(penLight, dx - (hArrow * 2) + 1,
						dy - hArrow - 1, dx - 1, dy - hArrow - 1);
					g.DrawLine(penLight, dx - (hArrow * 2),
						dy - hArrow, dx - hArrow - 1, dy - 1);
					g.DrawLine(penLight, dx - hArrow + 1, dy - 1,
						dx - 1, dy - hArrow + 1);
				}
			}

			// bmp.SetPixel(dx - 3, dy - 1, Color.Black);
			// // bmp.SetPixel(dx - 4, dy - 1, Color.Black);
			bmp.SetPixel(dx - hArrow, dy - 1, (bStdClr ? Color.Black : Color.White));

			return bmp;
		}

		[Obsolete("Use GfxUtil.ScaleImage instead.")]
		public static Bitmap CreateScaledImage(Image img, int w, int h)
		{
			if(img == null) { Debug.Assert(false); return null; }

			Bitmap bmp = (GfxUtil.ScaleImage(img, w, h) as Bitmap);
			Debug.Assert(bmp != null);
			return bmp;
		}

		/* public static T DgvGetComboBoxValue<T>(DataGridViewComboBoxCell c,
			List<KeyValuePair<T, string>> lItems)
		{
			if((c == null) || (lItems == null)) { Debug.Assert(false); return default(T); }

			string strValue = ((c.Value as string) ?? string.Empty);
			foreach(KeyValuePair<T, string> kvp in lItems)
			{
				if(kvp.Value == strValue) return kvp.Key;
			}

			Debug.Assert(false);
			return default(T);
		}

		public static void DgvSetComboBoxValue<T>(DataGridViewComboBoxCell c,
			T tValue, List<KeyValuePair<T, string>> lItems)
		{
			if((c == null) || (lItems == null)) { Debug.Assert(false); return; }

			foreach(KeyValuePair<T, string> kvp in lItems)
			{
				if(kvp.Key.Equals(tValue))
				{
					c.Value = kvp.Value;
					return;
				}
			}

			Debug.Assert(false);
		}

		public static bool GetChecked(DataGridViewCheckBoxCell c)
		{
			if(c == null) { Debug.Assert(false); return false; }

			object o = c.Value;
			if(o == null) { Debug.Assert(false); return false; }

			return StrUtil.StringToBool(o.ToString());
		}

		public static void SetChecked(DataGridViewCheckBoxCell c, bool bChecked)
		{
			if(c == null) { Debug.Assert(false); return; }

			c.Value = bChecked;
		} */

		public static Image GetFileIcon(string strFilePath, int w, int h)
		{
			Image img = null;
			try
			{
				Icon ico = Icon.ExtractAssociatedIcon(strFilePath);
				if(ico == null) return null;

				img = new Bitmap(w, h, PixelFormat.Format32bppArgb);
				using(Graphics g = Graphics.FromImage(img))
				{
					g.Clear(Color.Transparent);
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.DrawIcon(ico, new Rectangle(0, 0, img.Width, img.Height));
				}

				ico.Dispose();
			}
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }

			return img;
		}

		public static void SetHandled(KeyEventArgs e, bool bHandled)
		{
			if(e == null) { Debug.Assert(false); return; }

			e.Handled = bHandled;
			e.SuppressKeyPress = bHandled;
		}

		public static bool HandleCommonKeyEvent(KeyEventArgs e, bool bDown,
			Control cCtx)
		{
			if(e == null) { Debug.Assert(false); return false; }
			if(cCtx == null) { Debug.Assert(false); return false; }

			// On Windows, all of the following is already supported by .NET
			if(!NativeLib.IsUnix()) return false;

			bool bS = e.Shift;
			bool bC = e.Control;
			bool bA = e.Alt;
			Keys k = e.KeyCode;
			bool bMac = (NativeLib.GetPlatformID() == PlatformID.MacOSX);

			if(((k == Keys.Apps) && !bA) || // Shift and Control irrelevant
				((k == Keys.F10) && bS && !bA) || // Control irrelevant
				(bMac && (k == Keys.D5) && bC && bA) ||
				(bMac && (k == Keys.NumPad5) && bC))
			{
				bool bOp = bDown;
				if(k == Keys.Apps) bOp = !bDown;

				if(bOp)
				{
					ContextMenu cm = cCtx.ContextMenu;
					ContextMenuStrip cms = cCtx.ContextMenuStrip;

					if(cms != null) cms.Show(Cursor.Position);
					else if(cm != null)
					{
						Point pt = cCtx.PointToClient(Cursor.Position);
						cm.Show(cCtx, pt);
					}
				}

				UIUtil.SetHandled(e, true);
				return true;
			}

			return false;
		}

		internal static Size GetSmallIconSize(int wDefault, int hDefault)
		{
			// Throws under Mono 4.2.1 on Mac OS X;
			// https://sourceforge.net/p/keepass/discussion/329221/thread/7c096cfc/
			try { return SystemInformation.SmallIconSize; }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }

			return new Size(wDefault, hDefault);
		}

		/* internal static bool HasClickedSeparator(ToolStripItemClickedEventArgs e)
		{
			if(e == null) { Debug.Assert(false); return false; }

			ToolStripSeparator ts = (e.ClickedItem as ToolStripSeparator);
			if(ts == null) return false;

			Debug.Assert(!(e.ClickedItem is ToolStripMenuItem));
			return true;
		} */
	}
}
