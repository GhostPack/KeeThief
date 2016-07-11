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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;
using KeePass.Forms;
using KeePass.Native;

using NativeLib = KeePassLib.Native.NativeLib;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public static class HotKeyManager
	{
		private static Form m_fRecvWnd = null;
		private static Dictionary<int, Keys> m_vRegKeys = new Dictionary<int, Keys>();

		// private static NativeMethods.BindKeyHandler m_hOnHotKey =
		//	new NativeMethods.BindKeyHandler(HotKeyManager.OnHotKey);

		// public static Form ReceiverWindow
		// {
		//	get { return m_fRecvWnd; }
		//	set { m_fRecvWnd = value; }
		// }

		public static bool Initialize(Form fRecvWnd)
		{
			m_fRecvWnd = fRecvWnd;

			// if(NativeLib.IsUnix())
			// {
			//	try { NativeMethods.tomboy_keybinder_init(); }
			//	catch(Exception) { Debug.Assert(false); return false; }
			// }

			return true;
		}

		public static bool RegisterHotKey(int nId, Keys kKey)
		{
			UnregisterHotKey(nId);

			uint uMod = 0;
			if((kKey & Keys.Shift) != Keys.None) uMod |= NativeMethods.MOD_SHIFT;
			if((kKey & Keys.Alt) != Keys.None) uMod |= NativeMethods.MOD_ALT;
			if((kKey & Keys.Control) != Keys.None) uMod |= NativeMethods.MOD_CONTROL;

			uint vkCode = (uint)(kKey & Keys.KeyCode);
			if(vkCode == (uint)Keys.None) return false; // Don't register mod keys only

			try
			{
				if(!NativeLib.IsUnix())
				{
					if(NativeMethods.RegisterHotKey(m_fRecvWnd.Handle, nId, uMod, vkCode))
					{
						m_vRegKeys[nId] = kKey;
						return true;
					}
				}
				else // Unix
				{
					// NativeMethods.tomboy_keybinder_bind(EggAccKeysToString(kKey),
					//	m_hOnHotKey);
					// m_vRegKeys[nId] = kKey;
					// return true;
				}
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		public static bool UnregisterHotKey(int nId)
		{
			if(m_vRegKeys.ContainsKey(nId))
			{
				// Keys k = m_vRegKeys[nId];
				m_vRegKeys.Remove(nId);

				try
				{
					bool bResult;
					if(!NativeLib.IsUnix())
						bResult = NativeMethods.UnregisterHotKey(m_fRecvWnd.Handle, nId);
					else // Unix
					{
						// NativeMethods.tomboy_keybinder_unbind(EggAccKeysToString(k),
						//	m_hOnHotKey);
						// bResult = true;
						bResult = false;
					}

					// Debug.Assert(bResult);
					return bResult;
				}
				catch(Exception) { Debug.Assert(false); }
			}

			return false;
		}

		public static void UnregisterAll()
		{
			List<int> vIDs = new List<int>(m_vRegKeys.Keys);
			foreach(int nID in vIDs) UnregisterHotKey(nID);

			Debug.Assert(m_vRegKeys.Count == 0);
		}

		public static bool IsHotKeyRegistered(Keys kKey, bool bGlobal)
		{
			if(m_vRegKeys.ContainsValue(kKey)) return true;
			if(!bGlobal) return false;

			int nID = AppDefs.GlobalHotKeyId.TempRegTest;
			if(!RegisterHotKey(nID, kKey)) return true;

			UnregisterHotKey(nID);
			return false;
		}

		/* private static void OnHotKey(string strKey, IntPtr lpUserData)
		{
			if(string.IsNullOrEmpty(strKey)) return;
			if(strKey.IndexOf(@"<Release>", StrUtil.CaseIgnoreCmp) >= 0) return;

			if(m_fRecvWnd != null)
			{
				MainForm mf = (m_fRecvWnd as MainForm);
				if(mf == null) { Debug.Assert(false); return; }

				Keys k = EggAccStringToKeys(strKey);
				foreach(KeyValuePair<int, Keys> kvp in m_vRegKeys)
				{
					if(kvp.Value == k) mf.HandleHotKey(kvp.Key);
				}
			}
			else { Debug.Assert(false); }
		}

		private static Keys EggAccStringToKeys(string strKey)
		{
			if(string.IsNullOrEmpty(strKey)) return Keys.None;

			Keys k = Keys.None;

			if(strKey.IndexOf(@"<Alt>", StrUtil.CaseIgnoreCmp) >= 0)
				k |= Keys.Alt;
			if((strKey.IndexOf(@"<Ctl>", StrUtil.CaseIgnoreCmp) >= 0) ||
				(strKey.IndexOf(@"<Ctrl>", StrUtil.CaseIgnoreCmp) >= 0) ||
				(strKey.IndexOf(@"<Control>", StrUtil.CaseIgnoreCmp) >= 0))
				k |= Keys.Control;
			if((strKey.IndexOf(@"<Shft>", StrUtil.CaseIgnoreCmp) >= 0) ||
				(strKey.IndexOf(@"<Shift>", StrUtil.CaseIgnoreCmp) >= 0))
				k |= Keys.Shift;

			string strKeyCode = strKey;
			while(strKeyCode.IndexOf('<') >= 0)
			{
				int nStart = strKeyCode.IndexOf('<');
				int nEnd = strKeyCode.IndexOf('>');
				if((nStart < 0) || (nEnd < 0) || (nEnd <= nStart)) { Debug.Assert(false); break; }

				strKeyCode = strKeyCode.Remove(nStart, nEnd - nStart + 1);
			}
			strKeyCode = strKeyCode.Trim();

			try { k |= (Keys)Enum.Parse(typeof(Keys), strKeyCode, true); }
			catch(Exception) { Debug.Assert(false); }

			return k;
		}

		private static string EggAccKeysToString(Keys k)
		{
			StringBuilder sb = new StringBuilder();

			if((k & Keys.Shift) != Keys.None) sb.Append(@"<Shift>");
			if((k & Keys.Control) != Keys.None) sb.Append(@"<Control>");
			if((k & Keys.Alt) != Keys.None) sb.Append(@"<Alt>");

			sb.Append((k & Keys.KeyCode).ToString());
			return sb.ToString();
		} */
	}
}
