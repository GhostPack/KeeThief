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
using System.Threading;
using System.Media;
using System.Diagnostics;

using KeePass.Native;
using KeePass.Resources;
using KeePass.Util.SendInputExt;

using KeePassLib.Utility;

namespace KeePass.Util
{
	internal enum SiEventType
	{
		None = 0,
		Key,
		KeyModifier,
		Char,
		Delay,
		SetDefaultDelay,
		ClipboardCopy,
		AppActivate,
		Beep
	}

	internal sealed class SiEvent
	{
		public SiEventType Type = SiEventType.None;

		public int VKey = 0;
		public bool? ExtendedKey = null;

		public Keys KeyModifier = Keys.None;

		public char Char = char.MinValue;

		public bool? Down = null;

		public uint Delay = 0;

		public string Text = null;

#if DEBUG
		// For debugger display
		public override string ToString()
		{
			string str = Enum.GetName(typeof(SiEventType), this.Type);

			string strSub = null;
			switch(this.Type)
			{
				case SiEventType.Key:
					strSub = this.VKey.ToString() + " " + this.ExtendedKey.ToString() +
						" " + this.Down.ToString();
					break;
				case SiEventType.KeyModifier:
					strSub = this.KeyModifier.ToString() + " " + this.Down.ToString();
					break;
				case SiEventType.Char:
					strSub = this.Char.ToString() + " " + this.Down.ToString();
					break;
				case SiEventType.Delay:
				case SiEventType.SetDefaultDelay:
					strSub = this.Delay.ToString();
					break;
				case SiEventType.ClipboardCopy:
				case SiEventType.AppActivate:
				case SiEventType.Beep:
					strSub = this.Text;
					break;
				default: break;
			}

			if(!string.IsNullOrEmpty(strSub)) return (str + ": " + strSub);
			return str;
		}
#endif
	}

	public static class SendInputEx
	{
		private static CriticalSectionEx g_csSending = new CriticalSectionEx();

		private static int m_nCurSending = 0;
		public static bool IsSending
		{
			get { return (m_nCurSending != 0); }
		}

		public static void SendKeysWait(string strKeys, bool bObfuscate)
		{
			if(strKeys == null) { Debug.Assert(false); return; }

			List<SiEvent> l = Parse(strKeys);
			if(l.Count == 0) return;

			if(bObfuscate) SiObf.Obfuscate(l);

			FixEventSeq(l);

			bool bUnix = KeePassLib.Native.NativeLib.IsUnix();
			ISiEngine si;
			if(bUnix) si = new SiEngineUnix();
			else si = new SiEngineWin();

			bool bInter = Program.Config.Integration.AutoTypeAllowInterleaved;
			if(!bInter)
			{
				if(!g_csSending.TryEnter()) return;
			}

			Interlocked.Increment(ref m_nCurSending);
			try
			{
				si.Init();
				Send(si, l);
			}
			finally
			{
				try { si.Release(); }
				catch(Exception) { Debug.Assert(false); }

				Interlocked.Decrement(ref m_nCurSending);

				if(!bInter) g_csSending.Exit();
			}
		}

		private static List<SiEvent> Parse(string strSequence)
		{
			CharStream cs = new CharStream(strSequence);
			List<SiEvent> l = new List<SiEvent>();
			string strError = KPRes.AutoTypeSequenceInvalid;

			Keys kCurKbMods = Keys.None;

			List<Keys> lMods = new List<Keys>();
			lMods.Add(Keys.None);

			while(true)
			{
				char ch = cs.ReadChar();
				if(ch == char.MinValue) break;

				if((ch == '+') || (ch == '^') || (ch == '%'))
				{
					if(lMods.Count == 0) { Debug.Assert(false); break; }
					else if(ch == '+') lMods[lMods.Count - 1] |= Keys.Shift;
					else if(ch == '^') lMods[lMods.Count - 1] |= Keys.Control;
					else if(ch == '%') lMods[lMods.Count - 1] |= Keys.Alt;
					else { Debug.Assert(false); }

					continue;
				}
				else if(ch == '(')
				{
					lMods.Add(Keys.None);
					continue;
				}
				else if(ch == ')')
				{
					if(lMods.Count >= 2)
					{
						lMods.RemoveAt(lMods.Count - 1);
						lMods[lMods.Count - 1] = Keys.None;
					}
					else throw new FormatException(strError);

					continue;
				}

				Keys kEffMods = Keys.None;
				foreach(Keys k in lMods) kEffMods |= k;

				EnsureKeyModifiers(kEffMods, ref kCurKbMods, l);

				if(ch == '{')
				{
					List<SiEvent> lSub = ParseSpecial(cs);
					if(lSub == null) throw new FormatException(strError);

					l.AddRange(lSub);
				}
				else if(ch == '}')
					throw new FormatException(strError);
				else if(ch == '~')
				{
					SiEvent si = new SiEvent();
					si.Type = SiEventType.Key;
					si.VKey = (int)Keys.Enter;

					l.Add(si);
				}
				else
				{
					SiEvent si = new SiEvent();
					si.Type = SiEventType.Char;
					si.Char = ch;

					l.Add(si);
				}

				lMods[lMods.Count - 1] = Keys.None;
			}

			EnsureKeyModifiers(Keys.None, ref kCurKbMods, l);

			return l;
		}

		private static void EnsureKeyModifiers(Keys kReqMods, ref Keys kCurKbMods,
			List<SiEvent> l)
		{
			if(kReqMods == kCurKbMods) return;

			if((kReqMods & Keys.Shift) != (kCurKbMods & Keys.Shift))
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.KeyModifier;
				si.KeyModifier = Keys.Shift;
				si.Down = ((kReqMods & Keys.Shift) != Keys.None);

				l.Add(si);
			}

			if((kReqMods & Keys.Control) != (kCurKbMods & Keys.Control))
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.KeyModifier;
				si.KeyModifier = Keys.Control;
				si.Down = ((kReqMods & Keys.Control) != Keys.None);

				l.Add(si);
			}

			if((kReqMods & Keys.Alt) != (kCurKbMods & Keys.Alt))
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.KeyModifier;
				si.KeyModifier = Keys.Alt;
				si.Down = ((kReqMods & Keys.Alt) != Keys.None);

				l.Add(si);
			}

			kCurKbMods = kReqMods;
		}

		private static List<SiEvent> ParseSpecial(CharStream cs)
		{
			// Skip leading white space
			while(true)
			{
				char ch = cs.PeekChar();
				if(ch == char.MinValue) { Debug.Assert(false); return null; }

				if(!char.IsWhiteSpace(ch)) break;
				cs.ReadChar();
			}

			// First char is *always* part of the name (support for "{{}", etc.)
			char chFirst = cs.ReadChar();
			if(chFirst == char.MinValue) { Debug.Assert(false); return null; }

			int iPart = 0;
			StringBuilder sbName = new StringBuilder(), sbParams =
				new StringBuilder();
			sbName.Append(chFirst);

			while(true)
			{
				char ch = cs.ReadChar();
				if(ch == char.MinValue) { Debug.Assert(false); return null; }
				if(ch == '}') break;

				if(iPart == 0)
				{
					if(char.IsWhiteSpace(ch)) ++iPart;
					else sbName.Append(ch);
				}
				else sbParams.Append(ch);
			}

			string strName = sbName.ToString();
			string strParams = sbParams.ToString().Trim();

			uint? ouParam = null;
			if(strParams.Length > 0)
			{
				uint uParamTry;
				if(uint.TryParse(strParams, out uParamTry)) ouParam = uParamTry;
			}

			List<SiEvent> l = new List<SiEvent>();

			if(strName.Equals("DELAY", StrUtil.CaseIgnoreCmp))
			{
				if(!ouParam.HasValue) { Debug.Assert(false); return null; }

				SiEvent si = new SiEvent();
				si.Type = SiEventType.Delay;
				si.Delay = ouParam.Value;

				l.Add(si);
				return l;
			}
			if(strName.StartsWith("DELAY=", StrUtil.CaseIgnoreCmp))
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.SetDefaultDelay;

				string strDelay = strName.Substring(6).Trim();
				uint uDelay;
				if(uint.TryParse(strDelay, out uDelay))
					si.Delay = uDelay;
				else { Debug.Assert(false); return null; }

				l.Add(si);
				return l;
			}
			if(strName.Equals("VKEY", StrUtil.CaseIgnoreCmp) ||
				strName.Equals("VKEY-NX", StrUtil.CaseIgnoreCmp) ||
				strName.Equals("VKEY-EX", StrUtil.CaseIgnoreCmp))
			{
				if(!ouParam.HasValue) { Debug.Assert(false); return null; }

				SiEvent si = new SiEvent();
				si.Type = SiEventType.Key;
				si.VKey = (int)ouParam.Value;

				if(strName.EndsWith("-NX", StrUtil.CaseIgnoreCmp))
					si.ExtendedKey = false;
				else if(strName.EndsWith("-EX", StrUtil.CaseIgnoreCmp))
					si.ExtendedKey = true;

				l.Add(si);
				return l;
			}
			if(strName.Equals("APPACTIVATE", StrUtil.CaseIgnoreCmp))
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.AppActivate;
				si.Text = strParams;

				l.Add(si);
				return l;
			}
			if(strName.Equals("BEEP", StrUtil.CaseIgnoreCmp))
			{
				SiEvent si = new SiEvent();
				si.Type = SiEventType.Beep;
				si.Text = strParams;

				l.Add(si);
				return l;
			}

			SiCode siCode = SiCodes.Get(strName);

			SiEvent siTmpl = new SiEvent();
			if(siCode != null)
			{
				siTmpl.Type = SiEventType.Key;
				siTmpl.VKey = siCode.VKey;
				siTmpl.ExtendedKey = siCode.ExtKey;
			}
			else if(strName.Length == 1)
			{
				siTmpl.Type = SiEventType.Char;
				siTmpl.Char = strName[0];
			}
			else
			{
				throw new FormatException(KPRes.AutoTypeUnknownPlaceholder +
					MessageService.NewLine + @"{" + strName + @"}");
			}

			uint uRepeat = 1;
			if(ouParam.HasValue) uRepeat = ouParam.Value;

			for(uint u = 0; u < uRepeat; ++u)
			{
				SiEvent si = new SiEvent();
				si.Type = siTmpl.Type;
				si.VKey = siTmpl.VKey;
				si.ExtendedKey = siTmpl.ExtendedKey;
				si.Char = siTmpl.Char;

				l.Add(si);
			}

			return l;
		}

		private static void FixEventSeq(List<SiEvent> l)
		{
			// Convert chars to keys
			// Keys kMod = Keys.None;
			for(int i = 0; i < l.Count; ++i)
			{
				SiEvent si = l[i];
				SiEventType t = si.Type;

				// if(t == SiEventType.KeyModifier)
				// {
				//	if(!si.Down.HasValue) { Debug.Assert(false); continue; }
				//	if(si.Down.Value)
				//	{
				//		Debug.Assert((kMod & si.KeyModifier) == Keys.None);
				//		kMod |= si.KeyModifier;
				//	}
				//	else
				//	{
				//		Debug.Assert((kMod & si.KeyModifier) == si.KeyModifier);
				//		kMod &= ~si.KeyModifier;
				//	}
				// }
				if(t == SiEventType.Char)
				{
					// bool bLightConv = (kMod == Keys.None);
					int iVKey = SiCodes.CharToVKey(si.Char, true);
					if(iVKey > 0)
					{
						si.Type = SiEventType.Key;
						si.VKey = iVKey;
					}
				}
			}
		}

		private static void Send(ISiEngine siEngine, List<SiEvent> l)
		{
			bool bHasClipOp = l.Exists(SendInputEx.IsClipboardOp);
			ClipboardEventChainBlocker cev = null;
			ClipboardContents cnt = null;
			if(bHasClipOp)
			{
				cev = new ClipboardEventChainBlocker();
				cnt = new ClipboardContents(true, true);
			}

			try { SendPriv(siEngine, l); }
			finally
			{
				if(bHasClipOp)
				{
					ClipboardUtil.Clear();
					cnt.SetData();
					cev.Dispose();
				}
			}
		}

		private static bool IsClipboardOp(SiEvent si)
		{
			if(si == null) { Debug.Assert(false); return false; }
			return (si.Type == SiEventType.ClipboardCopy);
		}

		private static void SendPriv(ISiEngine siEngine, List<SiEvent> l)
		{
			// For 2000 alphanumeric characters:
			// * KeePass 1.26: 0:31 min
			// * KeePass 2.24: 1:58 min
			// * New engine of KeePass 2.25 with default delay DD:
			//   * DD =  1: 0:31 min
			//   * DD = 31: 1:03 min
			//   * DD = 32: 1:34 min
			//   * DD = 33: 1:34 min
			//   * DD = 43: 1:34 min
			//   * DD = 46: 1:34 min
			//   * DD = 47: 2:05 min
			//   * DD = 49: 2:05 min
			//   * DD = 59: 2:05 min
			uint uDefaultDelay = 33; // Slice boundary + 1

			// Induced by SiEngineWin.TrySendCharByKeypresses
			uDefaultDelay += 2;

			int iDefOvr = Program.Config.Integration.AutoTypeInterKeyDelay;
			if(iDefOvr >= 0)
			{
				if(iDefOvr == 0) iDefOvr = 1; // 1 ms is minimum
				uDefaultDelay = (uint)iDefOvr;
			}

			bool bFirstInput = true;
			foreach(SiEvent si in l)
			{
				// Also delay key modifiers, as a workaround for applications
				// with broken time-dependent message processing;
				// https://sourceforge.net/p/keepass/bugs/1213/
				if((si.Type == SiEventType.Key) || (si.Type == SiEventType.Char) ||
					(si.Type == SiEventType.KeyModifier))
				{
					if(!bFirstInput)
						siEngine.Delay(uDefaultDelay);

					bFirstInput = false;
				}

				switch(si.Type)
				{
					case SiEventType.Key:
						siEngine.SendKey(si.VKey, si.ExtendedKey, si.Down);
						break;

					case SiEventType.KeyModifier:
						if(si.Down.HasValue)
							siEngine.SetKeyModifier(si.KeyModifier, si.Down.Value);
						else { Debug.Assert(false); }
						break;

					case SiEventType.Char:
						siEngine.SendChar(si.Char, si.Down);
						break;

					case SiEventType.Delay:
						siEngine.Delay(si.Delay);
						break;

					case SiEventType.SetDefaultDelay:
						uDefaultDelay = si.Delay;
						break;

					case SiEventType.ClipboardCopy:
						if(!string.IsNullOrEmpty(si.Text))
							ClipboardUtil.Copy(si.Text, false, false, null,
								null, IntPtr.Zero);
						else if(si.Text != null)
							ClipboardUtil.Clear();
						break;

					case SiEventType.AppActivate:
						AppActivate(si);
						break;

					case SiEventType.Beep:
						Beep(si);
						break;

					default:
						Debug.Assert(false);
						break;
				}

				// Extra delay after tabs
				if(((si.Type == SiEventType.Key) && (si.VKey == (int)Keys.Tab)) ||
					((si.Type == SiEventType.Char) && (si.Char == '\t')))
				{
					if(uDefaultDelay < 100)
						siEngine.Delay(uDefaultDelay);
				}
			}
		}

		private static void AppActivate(SiEvent si)
		{
			try
			{
				if(string.IsNullOrEmpty(si.Text)) return;

				IntPtr h = NativeMethods.FindWindow(si.Text);
				if(h != IntPtr.Zero)
					NativeMethods.EnsureForegroundWindow(h);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private static void Beep(SiEvent si)
		{
			try
			{
				string str = si.Text;
				if(string.IsNullOrEmpty(str))
				{
					SystemSounds.Beep.Play();
					return;
				}

				string[] v = str.Split(new char[] { ' ', '\t' },
					StringSplitOptions.RemoveEmptyEntries);

				int f = 800, d = 200; // Defaults of Console.Beep()
				if(v.Length >= 1) int.TryParse(v[0], out f);
				if(v.Length >= 2) int.TryParse(v[1], out d);

				f = Math.Min(Math.Max(f, 37), 32767);
				if(d <= 0) return;

				Console.Beep(f, d); // Throws on a server
			}
			catch(Exception) { Debug.Assert(false); }
		}
	}
}
