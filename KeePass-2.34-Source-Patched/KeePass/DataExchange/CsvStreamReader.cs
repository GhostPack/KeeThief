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
using System.IO;
using System.Diagnostics;

using KeePassLib.Utility;

namespace KeePass.DataExchange
{
	public sealed class CsvStreamReader
	{
		private CharStream m_sChars;
		private readonly bool m_bAllowUnquoted;

		[Obsolete]
		public CsvStreamReader(string strData)
		{
			m_sChars = new CharStream(strData);
			m_bAllowUnquoted = false;
		}

		public CsvStreamReader(string strData, bool bAllowUnquoted)
		{
			m_sChars = new CharStream(strData);
			m_bAllowUnquoted = bAllowUnquoted;
		}

		public string[] ReadLine()
		{
			return (m_bAllowUnquoted ? ReadLineUnquoted() : ReadLineQuoted());
		}

		private string[] ReadLineQuoted()
		{
			if(m_sChars.PeekChar() == char.MinValue) return null;

			List<string> v = new List<string>();
			StringBuilder sb = new StringBuilder();
			bool bInField = false;

			while(true)
			{
				char ch = m_sChars.ReadChar();
				if(ch == char.MinValue) break;

				if((ch == '\"') && !bInField) bInField = true;
				else if((ch == '\"') && bInField)
				{
					if(m_sChars.PeekChar() == '\"')
					{
						m_sChars.ReadChar();
						sb.Append('\"');
					}
					else
					{
						v.Add(sb.ToString());
						if(sb.Length > 0) sb.Remove(0, sb.Length);

						bInField = false;
					}
				}
				else if(((ch == '\r') || (ch == '\n')) && !bInField) break;
				else if(bInField) sb.Append(ch);
			}
			Debug.Assert(!bInField);
			Debug.Assert(sb.Length == 0);
			if(sb.Length > 0) v.Add(sb.ToString());

			return v.ToArray();
		}

		private string[] ReadLineUnquoted()
		{
			char chFirst = m_sChars.PeekChar();
			if(chFirst == char.MinValue) return null;
			if((chFirst == '\r') || (chFirst == '\n'))
			{
				m_sChars.ReadChar(); // Advance
				return new string[0];
			}

			List<string> v = new List<string>();
			StringBuilder sb = new StringBuilder();
			bool bInField = false;

			while(true)
			{
				char ch = m_sChars.ReadChar();
				if(ch == char.MinValue) break;

				if((ch == '\"') && !bInField) bInField = true;
				else if((ch == '\"') && bInField)
				{
					if(m_sChars.PeekChar() == '\"')
					{
						m_sChars.ReadChar();
						sb.Append('\"');
					}
					else bInField = false;
				}
				else if(((ch == '\r') || (ch == '\n')) && !bInField) break;
				else if(bInField) sb.Append(ch);
				else if(ch == ',')
				{
					v.Add(sb.ToString());
					if(sb.Length > 0) sb.Remove(0, sb.Length);
				}
				else sb.Append(ch);
			}
			Debug.Assert(!bInField);
			v.Add(sb.ToString());

			return v.ToArray();
		}
	}
}
