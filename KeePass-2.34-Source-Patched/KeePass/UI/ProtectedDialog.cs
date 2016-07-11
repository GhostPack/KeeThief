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
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

using KeePass.Native;
using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Utility;

// using KpLibNativeMethods = KeePassLib.Native.NativeMethods;

namespace KeePass.UI
{
	public delegate Form UIFormConstructor(object objParam);
	public delegate object UIFormResultBuilder(Form f);

	public sealed class ProtectedDialog
	{
		private UIFormConstructor m_fnConstruct;
		private UIFormResultBuilder m_fnResultBuilder;

		private enum SecureThreadState
		{
			None = 0,
			ShowingDialog,
			Terminated
		}

		private sealed class SecureThreadInfo
		{
			public List<Bitmap> BackgroundBitmaps = new List<Bitmap>();
			public IntPtr ThreadDesktop = IntPtr.Zero;

			public object FormConstructParam = null;

			public DialogResult DialogResult = DialogResult.None;
			public object ResultObject = null;

			public SecureThreadState State = SecureThreadState.None;
		}

		public ProtectedDialog(UIFormConstructor fnConstruct,
			UIFormResultBuilder fnResultBuilder)
		{
			if(fnConstruct == null) { Debug.Assert(false); throw new ArgumentNullException("fnConstruct"); }
			if(fnResultBuilder == null) { Debug.Assert(false); throw new ArgumentNullException("fnResultBuilder"); }

			m_fnConstruct = fnConstruct;
			m_fnResultBuilder = fnResultBuilder;
		}

		public DialogResult ShowDialog(out object objResult, object objConstructParam)
		{
			objResult = null;

			ProcessMessagesEx();

			// Creating a window on the new desktop spawns a CtfMon.exe child
			// process by default. On Windows Vista, this process is terminated
			// correctly when the desktop is closed. However, on Windows 7 it
			// isn't terminated (probably a bug); creating multiple desktops
			// accumulates CtfMon.exe child processes.
			ChildProcessesSnapshot cpsCtfMons = new ChildProcessesSnapshot(
				"CtfMon.exe");

			ClipboardEventChainBlocker ccb = new ClipboardEventChainBlocker();
			byte[] pbClipHash = ClipboardUtil.ComputeHash();

			SecureThreadInfo stp = new SecureThreadInfo();
			foreach(Screen sc in Screen.AllScreens)
			{
				Bitmap bmpBack = UIUtil.CreateScreenshot(sc);
				if(bmpBack != null) UIUtil.DimImage(bmpBack);
				stp.BackgroundBitmaps.Add(bmpBack);
			}

			DialogResult dr = DialogResult.None;
			try
			{
				uint uOrgThreadId = NativeMethods.GetCurrentThreadId();
				IntPtr pOrgDesktop = NativeMethods.GetThreadDesktop(uOrgThreadId);

				string strName = "D" + Convert.ToBase64String(
					CryptoRandom.Instance.GetRandomBytes(16),
					Base64FormattingOptions.None);
				strName = strName.Replace(@"+", string.Empty);
				strName = strName.Replace(@"/", string.Empty);
				strName = strName.Replace(@"=", string.Empty);
				if(strName.Length > 15) strName = strName.Substring(0, 15);

				NativeMethods.DesktopFlags deskFlags =
					(NativeMethods.DesktopFlags.CreateMenu |
					NativeMethods.DesktopFlags.CreateWindow |
					NativeMethods.DesktopFlags.ReadObjects |
					NativeMethods.DesktopFlags.WriteObjects |
					NativeMethods.DesktopFlags.SwitchDesktop);

				IntPtr pNewDesktop = NativeMethods.CreateDesktop(strName,
					null, IntPtr.Zero, 0, deskFlags, IntPtr.Zero);
				if(pNewDesktop == IntPtr.Zero)
					throw new InvalidOperationException();

				bool bNameSupported = NativeMethods.DesktopNameContains(pNewDesktop,
					strName).GetValueOrDefault(false);
				Debug.Assert(bNameSupported);

				stp.ThreadDesktop = pNewDesktop;
				stp.FormConstructParam = objConstructParam;

				Thread th = new Thread(this.SecureDialogThread);
				th.CurrentCulture = Thread.CurrentThread.CurrentCulture;
				th.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
				th.Start(stp);

				SecureThreadState st = SecureThreadState.None;
				while(st != SecureThreadState.Terminated)
				{
					th.Join(150);

					lock(stp) { st = stp.State; }

					if((st == SecureThreadState.ShowingDialog) && bNameSupported)
					{
						IntPtr hCurDesk = NativeMethods.OpenInputDesktop(0,
							false, NativeMethods.DesktopFlags.ReadObjects);
						if(hCurDesk == IntPtr.Zero) { Debug.Assert(false); continue; }
						if(hCurDesk == pNewDesktop)
						{
							if(!NativeMethods.CloseDesktop(hCurDesk)) { Debug.Assert(false); }
							continue;
						}
						bool? obOnSec = NativeMethods.DesktopNameContains(hCurDesk, strName);
						if(!NativeMethods.CloseDesktop(hCurDesk)) { Debug.Assert(false); }

						lock(stp) { st = stp.State; } // Update; might have changed

						if(obOnSec.HasValue && !obOnSec.Value &&
							(st == SecureThreadState.ShowingDialog))
							HandleUnexpectedDesktopSwitch(pOrgDesktop, pNewDesktop, stp);
					}
				}

				if(!NativeMethods.SwitchDesktop(pOrgDesktop)) { Debug.Assert(false); }
				NativeMethods.SetThreadDesktop(pOrgDesktop);

				th.Join(); // Ensure thread terminated before closing desktop

				if(!NativeMethods.CloseDesktop(pNewDesktop)) { Debug.Assert(false); }
				NativeMethods.CloseDesktop(pOrgDesktop); // Optional

				dr = stp.DialogResult;
				objResult = stp.ResultObject;
			}
			catch(Exception) { Debug.Assert(false); }

			byte[] pbNewClipHash = ClipboardUtil.ComputeHash();
			if((pbClipHash != null) && (pbNewClipHash != null) &&
				!MemUtil.ArraysEqual(pbClipHash, pbNewClipHash))
				ClipboardUtil.Clear();
			ccb.Dispose();

			foreach(Bitmap bmpBack in stp.BackgroundBitmaps)
			{
				if(bmpBack != null) bmpBack.Dispose();
			}
			stp.BackgroundBitmaps.Clear();

			cpsCtfMons.TerminateNewChildsAsync(4100);

			// If something failed, show the dialog on the normal desktop
			if(dr == DialogResult.None)
			{
				Form f = m_fnConstruct(objConstructParam);
				dr = f.ShowDialog();
				objResult = m_fnResultBuilder(f);
				UIUtil.DestroyForm(f);
			}

			return dr;
		}

		private static void ProcessMessagesEx()
		{
			Application.DoEvents();
			Thread.Sleep(5);
			Application.DoEvents();
		}

		private void SecureDialogThread(object oParam)
		{
			SecureThreadInfo stp = (oParam as SecureThreadInfo);
			if(stp == null) { Debug.Assert(false); return; }

			List<BackgroundForm> lBackForms = new List<BackgroundForm>();
			BackgroundForm formBackPrimary = null;
			// bool bLangBar = false;

			try
			{
				if(!NativeMethods.SetThreadDesktop(stp.ThreadDesktop))
				{
					Debug.Assert(false);
					return;
				}

				ProcessMessagesEx();

				// Test whether we're really on the secure desktop
				if(NativeMethods.GetThreadDesktop(NativeMethods.GetCurrentThreadId()) !=
					stp.ThreadDesktop)
				{
					Debug.Assert(false);
					return;
				}

				// Disabling IME is not required anymore; we terminate
				// CtfMon.exe child processes manually
				// try { NativeMethods.ImmDisableIME(0); } // Always false on 2000/XP
				// catch(Exception) { Debug.Assert(!WinUtil.IsAtLeastWindows2000); }

				ProcessMessagesEx();

				Screen[] vScreens = Screen.AllScreens;
				Screen scPrimary = Screen.PrimaryScreen;
				Debug.Assert(vScreens.Length == stp.BackgroundBitmaps.Count);
				int sMin = Math.Min(vScreens.Length, stp.BackgroundBitmaps.Count);
				for(int i = sMin - 1; i >= 0; --i)
				{
					Bitmap bmpBack = stp.BackgroundBitmaps[i];
					if(bmpBack == null) continue;
					Debug.Assert(bmpBack.Size == vScreens[i].Bounds.Size);

					BackgroundForm formBack = new BackgroundForm(bmpBack,
						vScreens[i]);

					lBackForms.Add(formBack);
					if(vScreens[i].Equals(scPrimary))
						formBackPrimary = formBack;

					formBack.Show();
				}
				if(formBackPrimary == null)
				{
					Debug.Assert(false);
					if(lBackForms.Count > 0)
						formBackPrimary = lBackForms[lBackForms.Count - 1];
				}

				ProcessMessagesEx();

				if(!NativeMethods.SwitchDesktop(stp.ThreadDesktop)) { Debug.Assert(false); }

				ProcessMessagesEx();

				Form f = m_fnConstruct(stp.FormConstructParam);
				if(f == null) { Debug.Assert(false); return; }

				if(Program.Config.UI.SecureDesktopPlaySound)
					UIUtil.PlayUacSound();

				// bLangBar = ShowLangBar(true);

				lock(stp) { stp.State = SecureThreadState.ShowingDialog; }
				stp.DialogResult = f.ShowDialog(formBackPrimary);
				stp.ResultObject = m_fnResultBuilder(f);

				UIUtil.DestroyForm(f);
			}
			catch(Exception) { Debug.Assert(false); }
			finally
			{
				// if(bLangBar) ShowLangBar(false);

				foreach(BackgroundForm formBack in lBackForms)
				{
					try
					{
						formBack.Close();
						UIUtil.DestroyForm(formBack);
					}
					catch(Exception) { Debug.Assert(false); }
				}

				lock(stp) { stp.State = SecureThreadState.Terminated; }
			}
		}

		/* private static bool ShowLangBar(bool bShow)
		{
			try
			{
				return KpLibNativeMethods.TfShowLangBar(bShow ?
					KpLibNativeMethods.TF_SFT_SHOWNORMAL :
					KpLibNativeMethods.TF_SFT_HIDDEN);
			}
			catch(Exception) { }

			return false;
		} */

		private static void HandleUnexpectedDesktopSwitch(IntPtr pOrgDesktop,
			IntPtr pNewDesktop, SecureThreadInfo stp)
		{
			NativeMethods.SwitchDesktop(pOrgDesktop);
			NativeMethods.SetThreadDesktop(pOrgDesktop);

			ProcessMessagesEx();

			// Do not use MessageService.ShowWarning, because
			// it uses the top form's thread
			MessageService.ExternalIncrementMessageCount();
			NativeMethods.MessageBoxFlags mbf =
				(NativeMethods.MessageBoxFlags.MB_ICONWARNING |
				NativeMethods.MessageBoxFlags.MB_TASKMODAL |
				NativeMethods.MessageBoxFlags.MB_SETFOREGROUND |
				NativeMethods.MessageBoxFlags.MB_TOPMOST);
			if(StrUtil.RightToLeft)
				mbf |= (NativeMethods.MessageBoxFlags.MB_RTLREADING |
					NativeMethods.MessageBoxFlags.MB_RIGHT);
			NativeMethods.MessageBox(IntPtr.Zero, KPRes.SecDeskOtherSwitched +
				MessageService.NewParagraph + KPRes.SecDeskSwitchBack,
				PwDefs.ShortProductName, mbf);
			MessageService.ExternalDecrementMessageCount();

			SecureThreadState st;
			lock(stp) { st = stp.State; }
			if(st != SecureThreadState.Terminated)
			{
				NativeMethods.SwitchDesktop(pNewDesktop);
				ProcessMessagesEx();
			}
		}

		/* private static void BlockPrintScreen(Form f, bool bBlock)
		{
			if(f == null) { Debug.Assert(false); return; }

			try
			{
				if(bBlock)
				{
					NativeMethods.RegisterHotKey(f.Handle, NativeMethods.IDHOT_SNAPDESKTOP,
						0, NativeMethods.VK_SNAPSHOT);
					NativeMethods.RegisterHotKey(f.Handle, NativeMethods.IDHOT_SNAPWINDOW,
						NativeMethods.MOD_ALT, NativeMethods.VK_SNAPSHOT);
				}
				else
				{
					NativeMethods.UnregisterHotKey(f.Handle, NativeMethods.IDHOT_SNAPWINDOW);
					NativeMethods.UnregisterHotKey(f.Handle, NativeMethods.IDHOT_SNAPDESKTOP);
				}
			}
			catch(Exception) { Debug.Assert(false); }
		} */
	}
}
