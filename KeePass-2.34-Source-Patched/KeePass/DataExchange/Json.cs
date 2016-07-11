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
using System.Globalization;
using System.Diagnostics;

using KeePassLib.Utility;
using KeePassLib.Resources;

namespace KeePass.DataExchange
{
	public sealed class JsonFormatException : Exception
	{
		public override string Message
		{
			get { return KLRes.InvalidDataWhileDecoding; }
		}

		public JsonFormatException()
		{
			Debug.Assert(false);
		}
	}

	public sealed class JsonObject
	{
		private Dictionary<string, JsonValue> m_dict = new Dictionary<string, JsonValue>();
		public Dictionary<string, JsonValue> Items
		{
			get { return m_dict; }
		}

		public JsonObject(CharStream csDataSource)
		{
			if(csDataSource == null) throw new ArgumentNullException("csDataSource");

			char chInit = csDataSource.ReadChar(true);
			if(chInit != '{') throw new JsonFormatException();

			while(true)
			{
				string strName = (new JsonString(csDataSource)).Value;

				char chSeparator = csDataSource.ReadChar(true);
				if(chSeparator != ':') throw new JsonFormatException();

				JsonValue jValue = new JsonValue(csDataSource);

				m_dict[strName] = jValue;

				char chNext = csDataSource.PeekChar(true);
				if(chNext == '}') break;
				else if(chNext == ',') csDataSource.ReadChar(true);
				else throw new JsonFormatException();
			}

			char chTerminator = csDataSource.ReadChar(true);
			if(chTerminator != '}') throw new JsonFormatException();
		}
	}

	public sealed class JsonArray
	{
		private JsonValue[] m_values;
		public JsonValue[] Values
		{
			get { return m_values; }
		}

		public JsonArray(JsonValue[] vValues)
		{
			if(vValues == null) throw new ArgumentNullException("vValues");

			m_values = vValues;
		}

		public JsonArray(CharStream csDataSource)
		{
			if(csDataSource == null) throw new ArgumentNullException("csDataSource");

			char chInit = csDataSource.ReadChar(true);
			if(chInit != '[') throw new JsonFormatException();

			List<JsonValue> lValues = new List<JsonValue>();

			while(true)
			{
				char chNext = csDataSource.PeekChar(true);
				if(chNext == ']') break;

				lValues.Add(new JsonValue(csDataSource));

				chNext = csDataSource.PeekChar(true);
				if(chNext == ',') csDataSource.ReadChar(true);
			}

			char chTerminator = csDataSource.ReadChar(true);
			if(chTerminator != ']') throw new JsonFormatException();

			m_values = lValues.ToArray();
		}
	}

	public sealed class JsonValue
	{
		private object m_value;
		public object Value
		{
			get { return m_value; }
		}

		public JsonValue(CharStream csDataSource)
		{
			if(csDataSource == null) throw new ArgumentNullException("csDataSource");

			char chNext = csDataSource.PeekChar(true);

			if(chNext == '\"') m_value = (new JsonString(csDataSource)).Value;
			else if(chNext == '{') m_value = new JsonObject(csDataSource);
			else if(chNext == '[') m_value = new JsonArray(csDataSource);
			else if(chNext == 't')
			{
				for(int i = 0; i < 4; ++i) csDataSource.ReadChar(true);
				m_value = true;
			}
			else if(chNext == 'f')
			{
				for(int i = 0; i < 5; ++i) csDataSource.ReadChar(true);
				m_value = false;
			}
			else if(chNext == 'n')
			{
				for(int i = 0; i < 4; ++i) csDataSource.ReadChar(true);
				m_value = null;
			}
			else m_value = new JsonNumber(csDataSource);
		}

		public override string ToString()
		{
			if(m_value != null) return m_value.ToString();
			return string.Empty;
		}
	}

	public sealed class JsonString
	{
		private string m_str;
		public string Value
		{
			get { return m_str; }
		}

		public JsonString(string strValue)
		{
			if(strValue == null) throw new ArgumentNullException("strValue");

			m_str = strValue;
		}

		public JsonString(CharStream csDataSource)
		{
			if(csDataSource == null) throw new ArgumentNullException("csDataSource");

			char chInit = csDataSource.ReadChar(true);
			if(chInit != '\"') throw new JsonFormatException();

			StringBuilder sb = new StringBuilder();

			while(true)
			{
				char ch = csDataSource.ReadChar();
				if(ch == char.MinValue) throw new JsonFormatException();

				if(ch == '\"') break; // End of string
				else if(ch == '\\')
				{
					char chNext = csDataSource.ReadChar();
					if(chNext == char.MinValue) throw new JsonFormatException();

					if((chNext == 'b') || (chNext == 'f')) { } // Ignore
					else if(chNext == 'r') sb.Append('\r');
					else if(chNext == 'n') sb.Append('\n');
					else if(chNext == 't') sb.Append('\t');
					else if(chNext == 'u')
					{
						char ch1 = csDataSource.ReadChar();
						char ch2 = csDataSource.ReadChar();
						char ch3 = csDataSource.ReadChar();
						char ch4 = csDataSource.ReadChar();
						if(ch4 == char.MinValue) throw new JsonFormatException();

						byte[] pbUni = MemUtil.HexStringToByteArray(new string(
							new char[] { ch1, ch2, ch3, ch4 }));
						if((pbUni != null) && (pbUni.Length == 2))
							sb.Append((char)(((ushort)pbUni[0] << 8) | (ushort)pbUni[1]));
						else throw new JsonFormatException();
					}
					else sb.Append(chNext);
				}
				else sb.Append(ch);
			}

			m_str = sb.ToString();
		}

		public override string ToString()
		{
			return m_str;
		}
	}

	public sealed class JsonNumber
	{
		private double m_value;
		public double Value
		{
			get { return m_value; }
		}

		public JsonNumber(CharStream csDataSource)
		{
			if(csDataSource == null) throw new ArgumentNullException("csDataSource");

			StringBuilder sb = new StringBuilder();
			while(true)
			{
				char ch = csDataSource.PeekChar(true);

				if(((ch >= '0') && (ch <= '9')) || (ch == 'e') || (ch == 'E') ||
					(ch == '+') || (ch == '-') || (ch == '.'))
				{
					csDataSource.ReadChar(true);
					sb.Append(ch);
				}
				else break;
			}

			const NumberStyles ns = (NumberStyles.Integer | NumberStyles.AllowDecimalPoint |
				NumberStyles.AllowThousands | NumberStyles.AllowExponent);
			if(!double.TryParse(sb.ToString(), ns, NumberFormatInfo.InvariantInfo,
				out m_value)) { Debug.Assert(false); }
		}

		public override string ToString()
		{
			return m_value.ToString(NumberFormatInfo.InvariantInfo);
		}
	}
}
