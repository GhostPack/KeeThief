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

namespace KeePass.Util.SendInputExt
{
	internal static class SiObf
	{
		public static void Obfuscate(List<SiEvent> l)
		{
			if(l == null) { Debug.Assert(false); return; }

			int n = l.Count;
			if(n <= 1) return;

			bool[] vValid = new bool[n];
			Keys kMod = Keys.None;
			for(int i = 0; i < n; ++i)
			{
				SiEvent si = l[i];
				if(si.Type == SiEventType.KeyModifier)
				{
					if(si.Down.HasValue)
					{
						if(si.Down.Value)
						{
							Debug.Assert((kMod & si.KeyModifier) == Keys.None);
							kMod |= si.KeyModifier;
						}
						else
						{
							Debug.Assert((kMod & si.KeyModifier) == si.KeyModifier);
							kMod &= ~si.KeyModifier;
						}
					}
					else { Debug.Assert(false); }
				}
				else if((si.Type == SiEventType.Char) && (kMod == Keys.None))
					vValid[i] = true;
			}

			int c = 0;
			for(int i = n - 1; i >= -1; --i)
			{
				if((i == -1) || !vValid[i])
				{
					if(c > 0)
					{
						ReplaceByMixedTransfer(l, i + 1, c);
						c = 0;
					}
				}
				else ++c;
			}
		}

		private static void ReplaceByMixedTransfer(List<SiEvent> l, int iOffset,
			int nCount)
		{
			List<SiEvent> lNew = new List<SiEvent>();
			StringBuilder sbClip = new StringBuilder();

			// The string should be split randomly, but the same each
			// time this function is called. Otherwise an attacker could
			// get information by observing different splittings each
			// time auto-type is performed. Therefore, compute the random
			// seed based on the string to be auto-typed.
			Random r = new Random(GetRandomSeed(l, iOffset, nCount));

			for(int i = 0; i < nCount; ++i)
			{
				char ch = l[iOffset + i].Char;

				SiEvent si = new SiEvent();
				if(r.Next(0, 2) == 0)
				{
					sbClip.Append(ch);

					si.Type = SiEventType.Key;
					si.VKey = (int)Keys.Right;
				}
				else
				{
					si.Type = SiEventType.Char;
					si.Char = ch;
				}

				lNew.Add(si);
			}

			string strClip = sbClip.ToString();

			if(strClip.Length > 0)
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.ClipboardCopy;
				si.Text = strClip;
				lNew.Insert(0, si);

				// Mixed transfer occurs only for text without any modifiers,
				// thus we can press Ctrl+V for pasting here

				si = new SiEvent();
				si.Type = SiEventType.KeyModifier;
				si.KeyModifier = Keys.Control;
				si.Down = true;
				lNew.Insert(1, si);

				// Send the 'v' using a virtual key code, not as char;
				// sending it as char (translated to a keypress) doesn't
				// work with all keyboard layouts (e.g. Russian);
				// https://sourceforge.net/p/keepass/discussion/329220/thread/938d06a7/
				si = new SiEvent();
				si.Type = SiEventType.Key;
				si.VKey = (int)Keys.V;
				lNew.Insert(2, si);

				si = new SiEvent();
				si.Type = SiEventType.KeyModifier;
				si.KeyModifier = Keys.Control;
				si.Down = false;
				lNew.Insert(3, si);

				for(int i = 0; i < strClip.Length; ++i)
				{
					si = new SiEvent();
					si.Type = SiEventType.Key;
					si.VKey = (int)Keys.Left;
					lNew.Insert(4, si);
				}
			}

			l.RemoveRange(iOffset, nCount);
			l.InsertRange(iOffset, lNew);
		}

		private static int GetRandomSeed(List<SiEvent> l, int iOffset, int nCount)
		{
			int nSeed = 3;

			unchecked
			{
				for(int i = 0; i < nCount; ++i)
					nSeed = nSeed * 13 + l[iOffset + i].Char;
			}

			// Prevent overflow (see Random class constructor)
			if(nSeed == int.MinValue) nSeed = 13;
			return nSeed;
		}
	}
}
