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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Util.Spr
{
	public static class SprSyntax
	{
		private static readonly string[] m_vDynSepPlh = new string[] {
			@"{NEWPASSWORD:", @"{T-REPLACE-RX:", @"{T-CONV:"
		};

		private static readonly SprStyle SprStyleOK = new SprStyle(
			Color.FromArgb(0, 128, 0));
			// Color.FromArgb(212, 255, 212));
		private static readonly SprStyle SprStyleRaw = new SprStyle(
			Color.FromArgb(0, 0, 224));
			// Color.FromArgb(214, 226, 255));
		private static readonly SprStyle SprStyleWarning = new SprStyle(
			Color.FromArgb(255, 106, 0));
			// AppDefs.ColorEditError);
		private static readonly SprStyle SprStyleError = new SprStyle(
			Color.FromArgb(224, 0, 0));
			// AppDefs.ColorEditError);

		private sealed class SprStyle : IEquatable<SprStyle> // Immutable
		{
			private readonly Color? m_clr;
			public Color? Color { get { return m_clr; } }

			public SprStyle(Color? clr)
			{
				m_clr = clr;
			}

			public bool Equals(SprStyle other)
			{
				if(other == null) return false;

				if(m_clr.HasValue != other.m_clr.HasValue) return false;
				if(m_clr.HasValue)
				{
					if(m_clr.Value != other.m_clr.Value) return false;
				}

				return true;
			}
		}

		private sealed class SprPart // Immutable
		{
			private readonly string m_str;
			public string Text { get { return m_str; } }

			private readonly int m_iStart;
			public int Start { get { return m_iStart; } }

			public SprPart(string str, int iStart)
			{
				if(str == null) throw new ArgumentNullException("str");

				m_str = str;
				m_iStart = iStart;
			}

			public SprPart GetPart(int iOffset)
			{
				return GetPart(iOffset, m_str.Length - iOffset);
			}

			public SprPart GetPart(int iOffset, int nLength)
			{
				if(iOffset < 0) throw new ArgumentOutOfRangeException("iOffset");
				if(nLength < 0) throw new ArgumentOutOfRangeException("nLength");
				if((iOffset + nLength) > m_str.Length) throw new ArgumentException();

				SprPart pSub = new SprPart(m_str.Substring(iOffset, nLength),
					m_iStart + iOffset);
				return pSub;
			}
		}

		public static void Highlight(RichTextBox rtb, SprContext ctx)
		{
			try { HighlightPriv(rtb, ctx); }
			catch(Exception) { Debug.Assert(false); }
		}

		private static void HighlightPriv(RichTextBox rtb, SprContext ctx)
		{
			if(rtb == null) { Debug.Assert(false); return; }

			int iSelStart = rtb.SelectionStart;
			int iSelLen = rtb.SelectionLength;

			string strText = rtb.Text;

			rtb.SelectAll();
			// rtb.SelectionBackColor = SystemColors.Window;
			rtb.SelectionColor = SystemColors.ControlText;

			List<SprStyle> l = GetHighlight(strText, ctx);
			l.Add(null); // Sentinel

			SprStyle sFrom = null;
			int pFrom = 0;
			for(int i = 0; i < l.Count; ++i)
			{
				SprStyle s = l[i];

				if(sFrom != null)
				{
					if((s == null) || !sFrom.Equals(s))
					{
						if(sFrom.Color.HasValue)
						{
							rtb.Select(pFrom, i - pFrom);
							rtb.SelectionColor = sFrom.Color.Value;
							// rtb.SelectionBackColor = sFrom.Color.Value;
						}

						sFrom = s;
						pFrom = i;
					}
				}
				else
				{
					sFrom = s;
					pFrom = i;
				}
			}

			rtb.Select(iSelStart, iSelLen);
		}

		private static List<SprStyle> GetHighlight(string str, SprContext ctx)
		{
			if(str == null) { Debug.Assert(false); return new List<SprStyle>(); }

			List<SprStyle> l = new List<SprStyle>(str.Length + 1); // 1 for sentinel
			for(int i = 0; i < str.Length; ++i) l.Add(null);

			Stack<SprPart> sToDo = new Stack<SprPart>();
			sToDo.Push(new SprPart(str, 0));

			while(sToDo.Count > 0)
			{
				SprPart p = sToDo.Pop();

				Debug.Assert(p.Start >= 0);
				Debug.Assert((p.Start + p.Text.Length) <= l.Count);
				Debug.Assert(str.Substring(p.Start, p.Text.Length) == p.Text);

				HighlightPart(p, l, sToDo, ctx);
			}

			return l;
		}

		private static void HighlightPart(SprPart p, List<SprStyle> lStyles,
			Stack<SprPart> sToDo, SprContext ctx)
		{
			if(p.Text.Length == 0) return;

			if(HighlightDynSepPlh(p, lStyles, sToDo)) return;
			if(HighlightRegularPlh(p, lStyles, sToDo, ctx)) return;
			HighlightInvalid(p, lStyles, sToDo, ctx);
		}

		private static bool HighlightDynSepPlh(SprPart pPart, List<SprStyle> lStyles,
			Stack<SprPart> sToDo)
		{
			string str = pPart.Text;

			int iStart = -1, p = -1;
			foreach(string strPlh in m_vDynSepPlh)
			{
				iStart = str.IndexOf(strPlh, StrUtil.CaseIgnoreCmp);
				if(iStart >= 0)
				{
					p = iStart + strPlh.Length;
					break;
				}
			}
			if(iStart < 0) return false;

			try
			{
				if(p >= str.Length) throw new FormatException();

				char chSep = str[p];

				while(true)
				{
					if((p + 1) >= str.Length) throw new FormatException();

					if(str[p + 1] == '}') break;

					int q = str.IndexOf(chSep, p + 1);
					if(q < 0) throw new FormatException();

					p = q;
				}

				Debug.Assert(str[p + 1] == '}');
				if((p + 2) < str.Length)
					sToDo.Push(pPart.GetPart(p + 2));

				SetStyle(lStyles, pPart, iStart, (p + 1) - iStart + 1, SprStyleRaw);
			}
			catch(Exception)
			{
				SetStyle(lStyles, pPart, iStart, str.Length - iStart, SprStyleError);
			}

			if(iStart > 0) sToDo.Push(pPart.GetPart(0, iStart));
			return true;
		}

		private static bool HighlightRegularPlh(SprPart pPart, List<SprStyle> lStyles,
			Stack<SprPart> sToDo, SprContext ctx)
		{
			string str = pPart.Text;

			int iStart = str.IndexOf('{');
			if(iStart < 0) return false;

			if((iStart + 2) >= str.Length)
				SetStyle(lStyles, pPart, iStart, str.Length - iStart, SprStyleError);
			else
			{
				int iOpen = str.IndexOf('{', iStart + 2);
				int iClose = str.IndexOf('}', iStart + 2);

				bool bAT = ((ctx != null) && ctx.EncodeAsAutoTypeSequence);

				if(iClose < 0)
					SetStyle(lStyles, pPart, iStart, str.Length - iStart, SprStyleError);
				else if((iOpen >= 0) && (iOpen < iClose))
				{
					sToDo.Push(pPart.GetPart(iOpen));

					SetStyle(lStyles, pPart, iStart, iOpen - iStart, SprStyleError);
				}
				else if(bAT && ((str[iStart + 1] == '{') || (str[iStart + 1] == '}')) &&
					(iClose != (iStart + 2)))
				{
					sToDo.Push(pPart.GetPart(iClose + 1));

					SetStyle(lStyles, pPart, iStart, iClose - iStart + 1, SprStyleError);
				}
				else
				{
					sToDo.Push(pPart.GetPart(iClose + 1));

					int iErrLvl = 0;

					string strPlh = str.Substring(iStart + 1, iClose - iStart - 1);
					if(strPlh.Length == 0) { Debug.Assert(false); iErrLvl = 2; }
					else if(char.IsWhiteSpace(strPlh[0])) iErrLvl = 2;
					else if(strPlh.StartsWith("S:", StrUtil.CaseIgnoreCmp) &&
						(ctx != null) && (ctx.Entry != null))
					{
						string strField = strPlh.Substring(2);

						List<string> lFields = PwDefs.GetStandardFields();
						lFields.AddRange(ctx.Entry.Strings.GetKeys());

						bool bFound = false;
						foreach(string strAvail in lFields)
						{
							if(strField.Equals(strAvail, StrUtil.CaseIgnoreCmp))
							{
								bFound = true;
								break;
							}
						}

						if(!bFound) iErrLvl = 1;
					}

					SprStyle s = SprStyleOK;
					if(iErrLvl == 1) s = SprStyleWarning;
					else if(iErrLvl > 1) s = SprStyleError;

					SetStyle(lStyles, pPart, iStart, iClose - iStart + 1, s);
				}
			}

			if(iStart > 0) sToDo.Push(pPart.GetPart(0, iStart));
			return true;
		}

		private static bool HighlightInvalid(SprPart pPart, List<SprStyle> lStyles,
			Stack<SprPart> sToDo, SprContext ctx)
		{
			bool bAT = ((ctx != null) && ctx.EncodeAsAutoTypeSequence);
			if(!bAT) return false;

			string str = pPart.Text;
			int p = str.IndexOf('}');
			if(p >= 0)
			{
				Debug.Assert(str.IndexOf('{') < 0); // Must be called after regular

				sToDo.Push(pPart.GetPart(p + 1));

				SetStyle(lStyles, pPart, 0, p + 1, SprStyleError);
				return true;
			}

			return false;
		}

		private static void SetStyle(List<SprStyle> l, SprPart p,
			int iPartOffset, int nLength, SprStyle s)
		{
			int px = p.Start + iPartOffset;

			if(px < 0) { Debug.Assert(false); return; }
			if(nLength < 0) { Debug.Assert(false); return; }
			if((px + nLength) > l.Count) { Debug.Assert(false); return; }

			for(int i = 0; i < nLength; ++i)
			{
				Debug.Assert(l[px + i] == null);
				l[px + i] = s;
			}
		}
	}
}
