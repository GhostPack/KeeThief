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
using System.Globalization;
using System.Diagnostics;

namespace KeePass.Util.SendInputExt
{
	internal sealed class SiCode
	{
		public readonly string Code;
		public readonly int VKey;
		public readonly bool? ExtKey = null; // Currently all have default ext state
		public readonly string XKeySym;

		public SiCode(string strCode, int iVKey, string strXKeySym)
		{
			if(string.IsNullOrEmpty(strCode)) { Debug.Assert(false); strCode = " "; }

			this.Code = strCode;
			this.VKey = iVKey;
			this.XKeySym = strXKeySym;
		}

		public SiCode(string strCode, Keys k, string strXKeySym)
		{
			if(string.IsNullOrEmpty(strCode)) { Debug.Assert(false); strCode = " "; }

			this.Code = strCode;
			this.VKey = (int)k;
			this.XKeySym = strXKeySym;
		}
	}

	internal static class SiCodes
	{
		private static List<SiCode> m_l = null;
		public static List<SiCode> KeyCodes
		{
			get
			{
				if(m_l != null) return m_l;

				List<SiCode> l = new List<SiCode>();

				// XKeySym codes from 'keysymdef.h'

				l.Add(new SiCode("BACKSPACE", Keys.Back, "BackSpace"));
				l.Add(new SiCode("BKSP", Keys.Back, "BackSpace"));
				l.Add(new SiCode("BS", Keys.Back, "BackSpace"));
				l.Add(new SiCode("BREAK", Keys.Cancel, "Cancel"));
				l.Add(new SiCode("CAPSLOCK", Keys.CapsLock, "Caps_Lock"));
				l.Add(new SiCode("CLEAR", Keys.Clear, "Clear"));
				l.Add(new SiCode("DEL", Keys.Delete, "Delete"));
				l.Add(new SiCode("DELETE", Keys.Delete, "Delete"));
				l.Add(new SiCode("END", Keys.End, "End"));
				l.Add(new SiCode("ENTER", Keys.Enter, "Return"));
				l.Add(new SiCode("ESC", Keys.Escape, "Escape"));
				l.Add(new SiCode("ESCAPE", Keys.Escape, "Escape"));
				l.Add(new SiCode("HELP", Keys.Help, "Help"));
				l.Add(new SiCode("HOME", Keys.Home, "Home"));
				l.Add(new SiCode("INS", Keys.Insert, "Insert"));
				l.Add(new SiCode("INSERT", Keys.Insert, "Insert"));
				l.Add(new SiCode("NUMLOCK", Keys.NumLock, "Num_Lock"));
				l.Add(new SiCode("PGDN", Keys.PageDown, "Page_Down"));
				l.Add(new SiCode("PGUP", Keys.PageUp, "Page_Up"));
				l.Add(new SiCode("PRTSC", Keys.PrintScreen, "Print"));
				l.Add(new SiCode("SCROLLLOCK", Keys.Scroll, "Scroll_Lock"));
				l.Add(new SiCode("SPACE", Keys.Space, "space"));
				l.Add(new SiCode("TAB", Keys.Tab, "Tab"));

				l.Add(new SiCode("UP", Keys.Up, "Up"));
				l.Add(new SiCode("DOWN", Keys.Down, "Down"));
				l.Add(new SiCode("LEFT", Keys.Left, "Left"));
				l.Add(new SiCode("RIGHT", Keys.Right, "Right"));

				for(int i = 1; i <= 24; ++i)
				{
					string strF = "F" + i.ToString(NumberFormatInfo.InvariantInfo);
					l.Add(new SiCode(strF, (int)Keys.F1 + i - 1, strF));

					Debug.Assert(Enum.IsDefined(typeof(Keys), (int)Keys.F1 + i - 1) &&
						Enum.GetName(typeof(Keys), (int)Keys.F1 + i - 1) == strF);
				}

				l.Add(new SiCode("ADD", Keys.Add, "KP_Add"));
				l.Add(new SiCode("SUBTRACT", Keys.Subtract, "KP_Subtract"));
				l.Add(new SiCode("MULTIPLY", Keys.Multiply, "KP_Multiply"));
				l.Add(new SiCode("DIVIDE", Keys.Divide, "KP_Divide"));

				for(int i = 0; i < 10; ++i)
				{
					string strI = i.ToString(NumberFormatInfo.InvariantInfo);
					l.Add(new SiCode("NUMPAD" + strI, (int)Keys.NumPad0 + i,
						"KP_" + strI));

					Debug.Assert(((int)Keys.NumPad9 - (int)Keys.NumPad0) == 9);
				}

				l.Add(new SiCode("WIN", Keys.LWin, "Super_L"));
				l.Add(new SiCode("LWIN", Keys.LWin, "Super_L"));
				l.Add(new SiCode("RWIN", Keys.RWin, "Super_R"));
				l.Add(new SiCode("APPS", Keys.Apps, "Menu"));

#if DEBUG
				foreach(SiCode si in l)
				{
					// All key codes must be upper-case (for 'Get' method)
					Debug.Assert(si.Code == si.Code.ToUpperInvariant());
				}
#endif

				m_l = l;
				return l;
			}
		}

		private static Dictionary<string, SiCode> m_dictS = null;
		public static SiCode Get(string strCode)
		{
			if(strCode == null) { Debug.Assert(false); return null; }

			if(m_dictS == null)
			{
				Dictionary<string, SiCode> d = new Dictionary<string, SiCode>();
				foreach(SiCode siCp in SiCodes.KeyCodes)
				{
					d[siCp.Code] = siCp;
				}
				Debug.Assert(d.Count == SiCodes.KeyCodes.Count);

				m_dictS = d;
			}

			string strUp = strCode.ToUpperInvariant();
			SiCode si;
			if(m_dictS.TryGetValue(strUp, out si)) return si;
			return null;
		}

		public static SiCode Get(int iVKey, bool? bExtKey)
		{
			foreach(SiCode si in SiCodes.KeyCodes)
			{
				if(si.VKey == iVKey)
				{
					if(!si.ExtKey.HasValue || !bExtKey.HasValue) return si;
					if(si.ExtKey.Value == bExtKey.Value) return si;
				}
			}

			return null;
		}

		// Characters that should be converted to VKeys in special
		// situations (e.g. when a key modifier is active)
		private static Dictionary<char, int> m_dChToVk = null;

		// Characters that should always be converted to VKeys
		// (independent of e.g. whether key modifier is active or not)
		private static Dictionary<char, int> m_dChToVkAlways = null;

		private static void EnsureChToVkDicts()
		{
			if(m_dChToVk != null) return;

			Dictionary<char, int> d = new Dictionary<char, int>();

			// The following characters should *always* be converted
			d['\u0008'] = (int)Keys.Back;
			d['\t'] = (int)Keys.Tab;
			d['\n'] = (int)Keys.LineFeed;
			d['\r'] = (int)Keys.Return;
			d['\u001B'] = (int)Keys.Escape;
			d[' '] = (int)Keys.Space; // For toggling checkboxes
			d['\u007F'] = (int)Keys.Delete; // Different values

			m_dChToVkAlways = new Dictionary<char, int>(d);

			Debug.Assert((int)Keys.D0 == (int)'0');
			for(char ch = '0'; ch <= '9'; ++ch)
				d[ch] = (int)ch - (int)'0' + (int)Keys.D0;
			Debug.Assert(d['9'] == (int)Keys.D9);

			Debug.Assert((int)Keys.A == (int)'A');
			// Do not translate upper-case letters;
			// on Windows, sending VK_A results in 'a', and 'A' with Shift;
			// on some Linux systems, only Ctrl+'v' pastes, not Ctrl+'V':
			// https://sourceforge.net/p/keepass/discussion/329220/thread/bce61102/
			// for(char ch = 'A'; ch <= 'Z'; ++ch)
			//	d[ch] = (int)ch - (int)'A' + (int)Keys.A;
			for(char ch = 'a'; ch <= 'z'; ++ch)
				d[ch] = (int)ch - (int)'a' + (int)Keys.A;
			Debug.Assert(d['z'] == (int)Keys.Z);

			m_dChToVk = d;
			Debug.Assert(m_dChToVk.Count > m_dChToVkAlways.Count); // Independency
		}

		public static int CharToVKey(char ch, bool bLightConv)
		{
			EnsureChToVkDicts();

			Dictionary<char, int> d = (bLightConv ? m_dChToVkAlways : m_dChToVk);

			int iVKey;
			if(d.TryGetValue(ch, out iVKey)) return iVKey;
			return 0;
		}

		public static char VKeyToChar(int iVKey)
		{
			EnsureChToVkDicts();

			foreach(KeyValuePair<char, int> kvp in m_dChToVk)
			{
				if(kvp.Value == iVKey) return kvp.Key;
			}

			return char.MinValue;
		}
	}
}
