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
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

using KeePass.Native;

using KeePassLib.Utility;

namespace KeePass.Util.SendInputExt
{
	internal sealed class SiEngineWin : SiEngineStd
	{
		private static Dictionary<string, SiSendMethod> g_dProcessSendMethods =
			new Dictionary<string, SiSendMethod>();

		private IntPtr m_hklOriginal = IntPtr.Zero;
		private IntPtr m_hklCurrent = IntPtr.Zero;

		private bool m_bInputBlocked = false;

		private SiSendMethod? m_osmEnforced = null;

		private Dictionary<IntPtr, SiWindowInfo> m_dWindowInfos =
			new Dictionary<IntPtr, SiWindowInfo>();

		// private bool m_bThreadInputAttached = false;

		private Keys m_kModCur = Keys.None;

		public override void Init()
		{
			base.Init();

			try
			{
				InitProcessSendMethods();
				InitForEnv();

				// Do not use SendKeys.Flush here, use Application.DoEvents
				// instead; SendKeys.Flush might run into an infinite loop here
				// if a previous auto-type process failed with throwing an
				// exception (SendKeys.Flush is waiting in a loop for an internal
				// queue being empty, however the queue is never processed)
				Application.DoEvents();

				// if(m_uThisThreadID != m_uTargetThreadID)
				// {
				//	m_bThreadInputAttached = NativeMethods.AttachThreadInput(
				//		m_uThisThreadID, m_uTargetThreadID, true);
				//	Debug.Assert(m_bThreadInputAttached);
				// }
				// else { Debug.Assert(false); }

				m_bInputBlocked = NativeMethods.BlockInput(true);

				uint? tLastInput = NativeMethods.GetLastInputTime();
				if(tLastInput.HasValue)
				{
					int iDiff = Environment.TickCount - (int)tLastInput.Value;
					Debug.Assert(iDiff >= 0);
					if(iDiff == 0)
					{
						// Enforce delay after pressing the global auto-type
						// hot key, as a workaround for applications
						// with broken time-dependent message processing;
						// https://sourceforge.net/p/keepass/bugs/1213/
						Thread.Sleep(1);
						Application.DoEvents();
					}
				}

				if(ReleaseModifiers(true) > 0)
				{
					// Enforce delay between releasing modifiers and sending
					// the actual sequence, as a workaround for applications
					// with broken time-dependent message processing;
					// https://sourceforge.net/p/keepass/bugs/1213/
					Thread.Sleep(1);
					Application.DoEvents();
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public override void Release()
		{
			try
			{
				Debug.Assert(m_kModCur == Keys.None);

				if(m_bInputBlocked)
				{
					NativeMethods.BlockInput(false); // Unblock
					m_bInputBlocked = false;
				}

				Debug.Assert(GetActiveKeyModifiers().Count == 0);
				// Do not restore original modifier keys here, otherwise
				// modifier keys are restored even when the user released
				// them while KeePass is auto-typing
				// ActivateKeyModifiers(lRestore, true);
				ReleaseModifiers(false);

				// if(m_bThreadInputAttached)
				//	NativeMethods.AttachThreadInput(m_uThisThreadID,
				//		m_uTargetThreadID, false); // Detach

				Debug.Assert(NativeMethods.GetKeyboardLayout(0) == m_hklCurrent);
				if((m_hklCurrent != m_hklOriginal) && (m_hklOriginal != IntPtr.Zero))
				{
					if(NativeMethods.ActivateKeyboardLayout(m_hklOriginal, 0) !=
						m_hklCurrent)
					{
						Debug.Assert(false);
					}

					m_hklCurrent = m_hklOriginal;
				}

				Application.DoEvents();
			}
			catch(Exception) { Debug.Assert(false); }

			base.Release();
		}

		public override void SendKeyImpl(int iVKey, bool? bExtKey, bool? bDown)
		{
			PrepareSend();

			// Disable IME (only required for sending VKeys, not for chars);
			// https://sourceforge.net/p/keepass/discussion/329221/thread/5da4bd14/
			// using(SiImeBlocker sib = new SiImeBlocker(hWnd))

			if(bDown.HasValue)
			{
				SendVKeyNative(iVKey, bExtKey, bDown.Value);
				return;
			}

			SendVKeyNative(iVKey, bExtKey, true);
			SendVKeyNative(iVKey, bExtKey, false);
		}

		public override void SetKeyModifierImpl(Keys kMod, bool bDown)
		{
			SetKeyModifierImplEx(kMod, bDown, false);
		}

		private void SetKeyModifierImplEx(Keys kMod, bool bDown, bool bRAlt)
		{
			PrepareSend();

			if((kMod & Keys.Shift) != Keys.None)
				SendVKeyNative((int)Keys.ShiftKey, null, bDown);
			if((kMod & Keys.Control) != Keys.None)
				SendVKeyNative((int)Keys.ControlKey, null, bDown);
			if((kMod & Keys.Alt) != Keys.None)
			{
				int vk = (int)(bRAlt ? Keys.RMenu : Keys.Menu);
				SendVKeyNative(vk, null, bDown);
			}

			if(bDown) m_kModCur |= kMod;
			else m_kModCur &= ~kMod;
		}

		public override void SendCharImpl(char ch, bool? bDown)
		{
			SiWindowInfo swi = PrepareSend();

			if(TrySendCharByKeypresses(ch, bDown, swi)) return;

			if(bDown.HasValue)
			{
				SendCharNative(ch, bDown.Value);
				return;
			}

			SendCharNative(ch, true);
			SendCharNative(ch, false);
		}

		private static void InitProcessSendMethods()
		{
			Dictionary<string, SiSendMethod> d = g_dProcessSendMethods;
			if(d.Count > 0) return; // Init once is sufficient

			string[] vEnfUni = new string[] {
				"PuTTY",
				"KiTTY", "KiTTY_Portable", "KiTTY_NoTrans", "KiTTY_NoHyperlink",
				"KiTTY_NoCompress",
				"PuTTYjp",
				// "mRemoteNG", // No effect
				// "PuTTYNG", // No effect

				// SuperPuTTY spawns PuTTY processes whose windows are
				// displayed embedded in the SuperPuTTY window (without
				// window borders), thus the definition "PuTTY" also
				// fixes SuperPuTTY; no "SuperPuTTY" required
				// "SuperPuTTY",

				"MinTTY" // Cygwin window "~"
			};
			foreach(string strEnfUni in vEnfUni)
			{
				d[strEnfUni] = SiSendMethod.UnicodePacket;
			}

			string[] vEnfKey = new string[] {
				"MSTSC", // Remote Desktop Connection client
				"VirtualBox" // VirtualBox does not support VK_PACKET
			};
			foreach(string strEnfKey in vEnfKey)
			{
				d[strEnfKey] = SiSendMethod.KeyEvent;
			}
		}

		private void InitForEnv()
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			try
			{
				m_hklCurrent = NativeMethods.GetKeyboardLayout(0);
				m_hklOriginal = m_hklCurrent;
				Debug.Assert(m_hklOriginal != IntPtr.Zero);

				Process[] vProcesses = Process.GetProcesses();
				foreach(Process p in vProcesses)
				{
					if(p == null) { Debug.Assert(false); continue; }

					try
					{
						string strName = GetProcessName(p);

						// If the Neo keyboard layout is being used, we must
						// send Unicode characters; keypresses are converted
						// and thus do not lead to the expected result
						if(ProcessNameMatches(strName, "Neo20"))
						{
							FileVersionInfo fvi = p.MainModule.FileVersionInfo;
							if(((fvi.ProductName ?? string.Empty).Trim().Length == 0) &&
								((fvi.FileDescription ?? string.Empty).Trim().Length == 0))
								m_osmEnforced = SiSendMethod.UnicodePacket;
							else { Debug.Assert(false); }
						}
						else if(ProcessNameMatches(strName, "KbdNeo_Ahk"))
							m_osmEnforced = SiSendMethod.UnicodePacket;
					}
					catch(Exception) { Debug.Assert(false); }

					try { p.Dispose(); }
					catch(Exception) { Debug.Assert(false); }
				}
			}
			catch(Exception) { Debug.Assert(false); }

#if DEBUG
			sw.Stop();
			Debug.Assert(sw.ElapsedMilliseconds < 100);
#endif
		}

		private static bool SendVKeyNative(int vKey, bool? bExtKey, bool bDown)
		{
			bool bRes = false;

			if(IntPtr.Size == 4)
				bRes = SendVKeyNative32(vKey, bExtKey, null, bDown);
			else if(IntPtr.Size == 8)
				bRes = SendVKeyNative64(vKey, bExtKey, null, bDown);
			else { Debug.Assert(false); }

			// The following does not hold when sending keypresses to
			// key state-consuming windows (e.g. VM windows)
			// if(bDown && (vKey != NativeMethods.VK_CAPITAL))
			// {
			//	Debug.Assert(IsKeyActive(vKey));
			// }

			return bRes;
		}

		private static bool SendCharNative(char ch, bool bDown)
		{
			if(IntPtr.Size == 4)
				return SendVKeyNative32(0, null, ch, bDown);
			else if(IntPtr.Size == 8)
				return SendVKeyNative64(0, null, ch, bDown);
			else { Debug.Assert(false); }

			return false;
		}

		private static bool SendVKeyNative32(int vKey, bool? bExtKey,
			char? optUnicodeChar, bool bDown)
		{
			NativeMethods.INPUT32[] pInput = new NativeMethods.INPUT32[1];

			pInput[0].Type = NativeMethods.INPUT_KEYBOARD;

			if(optUnicodeChar.HasValue && WinUtil.IsAtLeastWindows2000)
			{
				pInput[0].KeyboardInput.VirtualKeyCode = 0;
				pInput[0].KeyboardInput.ScanCode = (ushort)optUnicodeChar.Value;
				pInput[0].KeyboardInput.Flags = ((bDown ? 0 :
					NativeMethods.KEYEVENTF_KEYUP) | NativeMethods.KEYEVENTF_UNICODE);
			}
			else
			{
				if(optUnicodeChar.HasValue)
					vKey = (int)(NativeMethods.VkKeyScan(optUnicodeChar.Value) & 0xFFU);

				pInput[0].KeyboardInput.VirtualKeyCode = (ushort)vKey;
				pInput[0].KeyboardInput.ScanCode =
					(ushort)(NativeMethods.MapVirtualKey((uint)vKey, 0) & 0xFFU);
				pInput[0].KeyboardInput.Flags = GetKeyEventFlags(vKey, bExtKey, bDown);
			}

			pInput[0].KeyboardInput.Time = 0;
			pInput[0].KeyboardInput.ExtraInfo = NativeMethods.GetMessageExtraInfo();

			Debug.Assert(Marshal.SizeOf(typeof(NativeMethods.INPUT32)) == 28);
			if(NativeMethods.SendInput32(1, pInput,
				Marshal.SizeOf(typeof(NativeMethods.INPUT32))) != 1)
				return false;

			return true;
		}

		private static bool SendVKeyNative64(int vKey, bool? bExtKey,
			char? optUnicodeChar, bool bDown)
		{
			NativeMethods.SpecializedKeyboardINPUT64[] pInput = new
				NativeMethods.SpecializedKeyboardINPUT64[1];

			pInput[0].Type = NativeMethods.INPUT_KEYBOARD;

			if(optUnicodeChar.HasValue && WinUtil.IsAtLeastWindows2000)
			{
				pInput[0].VirtualKeyCode = 0;
				pInput[0].ScanCode = (ushort)optUnicodeChar.Value;
				pInput[0].Flags = ((bDown ? 0 : NativeMethods.KEYEVENTF_KEYUP) |
					NativeMethods.KEYEVENTF_UNICODE);
			}
			else
			{
				if(optUnicodeChar.HasValue)
					vKey = (int)(NativeMethods.VkKeyScan(optUnicodeChar.Value) & 0xFFU);

				pInput[0].VirtualKeyCode = (ushort)vKey;
				pInput[0].ScanCode = (ushort)(NativeMethods.MapVirtualKey(
					(uint)vKey, 0) & 0xFFU);
				pInput[0].Flags = GetKeyEventFlags(vKey, bExtKey, bDown);
			}

			pInput[0].Time = 0;
			pInput[0].ExtraInfo = NativeMethods.GetMessageExtraInfo();

			Debug.Assert(Marshal.SizeOf(typeof(NativeMethods.SpecializedKeyboardINPUT64)) == 40);
			if(NativeMethods.SendInput64Special(1, pInput,
				Marshal.SizeOf(typeof(NativeMethods.SpecializedKeyboardINPUT64))) != 1)
				return false;

			return true;
		}

		private static uint GetKeyEventFlags(int vKey, bool? bExtKey, bool bDown)
		{
			uint u = 0;

			if(!bDown) u |= NativeMethods.KEYEVENTF_KEYUP;

			if(bExtKey.HasValue)
			{
				if(bExtKey.Value) u |= NativeMethods.KEYEVENTF_EXTENDEDKEY;
			}
			else if(IsExtendedKeyEx(vKey))
				u |= NativeMethods.KEYEVENTF_EXTENDEDKEY;

			return u;
		}

		private static bool IsExtendedKeyEx(int vKey)
		{
			// http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731.aspx
			// http://www.win.tue.nl/~aeb/linux/kbd/scancodes-1.html
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_LSHIFT, 0) == 0x2AU);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_RSHIFT, 0) == 0x36U);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_SHIFT, 0) == 0x2AU);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_LCONTROL, 0) == 0x1DU);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_RCONTROL, 0) == 0x1DU);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_CONTROL, 0) == 0x1DU);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_LMENU, 0) == 0x38U);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_RMENU, 0) == 0x38U);
			Debug.Assert(NativeMethods.MapVirtualKey((uint)
				NativeMethods.VK_MENU, 0) == 0x38U);
			Debug.Assert(NativeMethods.MapVirtualKey(0x5BU, 0) == 0x5BU);
			Debug.Assert(NativeMethods.MapVirtualKey(0x5CU, 0) == 0x5CU);
			Debug.Assert(NativeMethods.MapVirtualKey(0x5DU, 0) == 0x5DU);
			Debug.Assert(NativeMethods.MapVirtualKey(0x6AU, 0) == 0x37U);
			Debug.Assert(NativeMethods.MapVirtualKey(0x6BU, 0) == 0x4EU);
			Debug.Assert(NativeMethods.MapVirtualKey(0x6DU, 0) == 0x4AU);
			Debug.Assert(NativeMethods.MapVirtualKey(0x6EU, 0) == 0x53U);
			Debug.Assert(NativeMethods.MapVirtualKey(0x6FU, 0) == 0x35U);

			if((vKey >= 0x21) && (vKey <= 0x2E)) return true;
			if((vKey >= 0x5B) && (vKey <= 0x5D)) return true;
			if(vKey == 0x6F) return true; // VK_DIVIDE

			// RShift is separate; no E0
			if(vKey == NativeMethods.VK_RCONTROL) return true;
			if(vKey == NativeMethods.VK_RMENU) return true;

			return false;
		}

		private static int ReleaseModifiers(bool bWithSpecial)
		{
			List<int> lMod = GetActiveKeyModifiers();
			ActivateKeyModifiers(lMod, false);

			if(bWithSpecial) SpecialReleaseModifiers(lMod);

			Debug.Assert(GetActiveKeyModifiers().Count == 0);
			return lMod.Count;
		}

		private static List<int> GetActiveKeyModifiers()
		{
			List<int> lSet = new List<int>();

			AddKeyModifierIfSet(lSet, NativeMethods.VK_LSHIFT);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_RSHIFT);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_SHIFT);

			AddKeyModifierIfSet(lSet, NativeMethods.VK_LCONTROL);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_RCONTROL);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_CONTROL);

			AddKeyModifierIfSet(lSet, NativeMethods.VK_LMENU);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_RMENU);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_MENU);

			AddKeyModifierIfSet(lSet, NativeMethods.VK_LWIN);
			AddKeyModifierIfSet(lSet, NativeMethods.VK_RWIN);

			AddKeyModifierIfSet(lSet, NativeMethods.VK_CAPITAL);

			return lSet;
		}

		private static void AddKeyModifierIfSet(List<int> lList, int vKey)
		{
			if(IsKeyActive(vKey)) lList.Add(vKey);
		}

		private static bool IsKeyActive(int vKey)
		{
			if(vKey == NativeMethods.VK_CAPITAL)
			{
				ushort usCap = NativeMethods.GetKeyState(vKey);
				return ((usCap & 1) != 0);
			}

			ushort usState = NativeMethods.GetAsyncKeyState(vKey);
			return ((usState & 0x8000) != 0);

			// For GetKeyState:
			// if(vKey == NativeMethods.VK_CAPITAL)
			//	return ((usState & 1) != 0);
			// else
			//	return ((usState & 0x8000) != 0);
		}

		private static void ActivateKeyModifiers(List<int> vKeys, bool bDown)
		{
			Debug.Assert(vKeys != null);
			if(vKeys == null) throw new ArgumentNullException("vKeys");

			foreach(int vKey in vKeys)
			{
				if(vKey == NativeMethods.VK_CAPITAL) // Toggle
				{
					SendVKeyNative(vKey, null, true);
					SendVKeyNative(vKey, null, false);
				}
				else SendVKeyNative(vKey, null, bDown);
			}
		}

		private static void SpecialReleaseModifiers(List<int> vKeys)
		{
			// Get out of a menu bar that was focused when only
			// using Alt as hot key modifier
			if(Program.Config.Integration.AutoTypeReleaseAltWithKeyPress &&
				(vKeys.Count == 2) && vKeys.Contains(NativeMethods.VK_MENU))
			{
				if(vKeys.Contains(NativeMethods.VK_LMENU))
				{
					SendVKeyNative(NativeMethods.VK_LMENU, null, true);
					SendVKeyNative(NativeMethods.VK_LMENU, null, false);
				}
				else if(vKeys.Contains(NativeMethods.VK_RMENU))
				{
					SendVKeyNative(NativeMethods.VK_RMENU, null, true);
					SendVKeyNative(NativeMethods.VK_RMENU, null, false);
				}
			}
		}

		private static char[] m_vForcedUniChars = null;
		private bool TrySendCharByKeypresses(char ch, bool? bDown, SiWindowInfo swi)
		{
			if(ch == char.MinValue) { Debug.Assert(false); return false; }

			SiSendMethod sm = GetSendMethod(swi);
			if(sm == SiSendMethod.UnicodePacket) return false;

			if(m_vForcedUniChars == null)
				m_vForcedUniChars = new char[] {
					// All of the following diacritics are spacing / non-combining

					'\u00B4', // Acute accent
					'\u02DD', // Double acute accent
					'\u0060', // Grave accent
					'\u02D8', // Breve
					'\u00B8', // Cedilla
					'\u005E', // Circumflex ^
					'\u00A8', // Diaeresis
					'\u02D9', // Dot above
					'\u00AF', // Macron above, long
					'\u02C9', // Macron above, modifier, short
					'\u02CD', // Macron below, modifier, short
					'\u02DB', // Ogonek

					// E.g. for US-International;
					// https://sourceforge.net/p/keepass/discussion/329220/thread/5708e5ef/
					'\u0027', // Apostrophe
					'\u0022', // Quotation mark
					'\u007E' // Tilde
				};
			if(sm != SiSendMethod.KeyEvent) // If Unicode packets allowed
			{
				if(Array.IndexOf<char>(m_vForcedUniChars, ch) >= 0) return false;
			}

			IntPtr hKL = swi.KeyboardLayout;
			ushort u = ((hKL == IntPtr.Zero) ? NativeMethods.VkKeyScan(ch) :
				NativeMethods.VkKeyScanEx(ch, hKL));
			if(u == 0xFFFFU) return false;

			int vKey = (int)(u & 0xFFU);

			Keys kMod = Keys.None;
			int nMods = 0;
			if((u & 0x100U) != 0U) { ++nMods; kMod |= Keys.Shift; }
			if((u & 0x200U) != 0U) { ++nMods; kMod |= Keys.Control; }
			if((u & 0x400U) != 0U) { ++nMods; kMod |= Keys.Alt; }
			if((u & 0x800U) != 0U) return false; // Hankaku unsupported

			// Do not send a key combination that is registered as hot key;
			// https://sourceforge.net/p/keepass/bugs/1235/
			// Windows shortcut hot keys involve at least 2 modifiers
			if(nMods >= 2)
			{
				Keys kFull = (kMod | (Keys)vKey);
				if(HotKeyManager.IsHotKeyRegistered(kFull, true))
					return false;
			}

			Keys kModDiff = (kMod & ~m_kModCur);
			if(kModDiff != Keys.None)
			{
				// Send RAlt for better AltGr compatibility;
				// https://sourceforge.net/p/keepass/bugs/1475/
				SetKeyModifierImplEx(kModDiff, true, true);

				Thread.Sleep(1);
				Application.DoEvents();
			}

			SendKeyImpl(vKey, null, bDown);

			if(kModDiff != Keys.None)
			{
				Thread.Sleep(1);
				Application.DoEvents();

				SetKeyModifierImplEx(kModDiff, false, true);
			}

			return true;
		}

		private SiSendMethod GetSendMethod(SiWindowInfo swi)
		{
			if(m_osmEnforced.HasValue) return m_osmEnforced.Value;

			return swi.SendMethod;
		}

		private SiWindowInfo GetWindowInfo(IntPtr hWnd)
		{
			SiWindowInfo swi;
			if(m_dWindowInfos.TryGetValue(hWnd, out swi)) return swi;

			swi = new SiWindowInfo(hWnd);

			Process p = null;
			try
			{
				uint uPID;
				uint uTID = NativeMethods.GetWindowThreadProcessId(hWnd, out uPID);

				swi.KeyboardLayout = NativeMethods.GetKeyboardLayout(uTID);

				p = Process.GetProcessById((int)uPID);
				string strName = GetProcessName(p);

				foreach(KeyValuePair<string, SiSendMethod> kvp in g_dProcessSendMethods)
				{
					if(ProcessNameMatches(strName, kvp.Key))
					{
						swi.SendMethod = kvp.Value;
						break;
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
			finally
			{
				try { if(p != null) p.Dispose(); }
				catch(Exception) { Debug.Assert(false); }
			}

			m_dWindowInfos[hWnd] = swi;
			return swi;
		}

		private SiWindowInfo PrepareSend()
		{
			IntPtr hWnd = NativeMethods.GetForegroundWindowHandle();
			SiWindowInfo swi = GetWindowInfo(hWnd);

			EnsureSameKeyboardLayout(swi);

			return swi;
		}

		private void EnsureSameKeyboardLayout(SiWindowInfo swi)
		{
			if(!Program.Config.Integration.AutoTypeAdjustKeyboardLayout) return;

			IntPtr hklTarget = swi.KeyboardLayout;
			Debug.Assert(hklTarget != IntPtr.Zero);
			Debug.Assert(NativeMethods.GetKeyboardLayout(0) == m_hklCurrent);

			if((m_hklCurrent != hklTarget) && (hklTarget != IntPtr.Zero))
			{
				if(NativeMethods.ActivateKeyboardLayout(hklTarget, 0) != m_hklCurrent)
				{
					Debug.Assert(false);
				}
				m_hklCurrent = hklTarget;

				Thread.Sleep(1);
				Application.DoEvents();
			}
		}

		private static string GetProcessName(Process p)
		{
			if(p == null) { Debug.Assert(false); return string.Empty; }

			try { return (p.ProcessName ?? string.Empty).Trim(); }
			catch(Exception) { Debug.Assert(false); }
			return string.Empty;
		}

		private static bool ProcessNameMatches(string strUnk, string strPattern)
		{
			if(strUnk == null) { Debug.Assert(false); return false; }
			if(strPattern == null) { Debug.Assert(false); return false; }
			Debug.Assert(strUnk.Trim() == strUnk);
			Debug.Assert(strPattern.Trim() == strPattern);
			Debug.Assert(!strPattern.EndsWith(".exe", StrUtil.CaseIgnoreCmp));

			return (strUnk.Equals(strPattern, StrUtil.CaseIgnoreCmp) ||
				strUnk.Equals(strPattern + ".exe", StrUtil.CaseIgnoreCmp));
		}
	}

	/* internal sealed class SiImeBlocker : IDisposable
	{
		private IntPtr m_hWnd;
		private IntPtr m_hOrgIme = IntPtr.Zero;

		public SiImeBlocker(IntPtr hWnd)
		{
			m_hWnd = hWnd;
			if(hWnd == IntPtr.Zero) return;

			try { m_hOrgIme = NativeMethods.ImmAssociateContext(hWnd, IntPtr.Zero); }
			catch(Exception) { Debug.Assert(false); }
		}

		~SiImeBlocker()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(m_hOrgIme != IntPtr.Zero)
			{
				try { NativeMethods.ImmAssociateContext(m_hWnd, m_hOrgIme); }
				catch(Exception) { Debug.Assert(false); }

				m_hOrgIme = IntPtr.Zero;
			}
		}
	} */
}
