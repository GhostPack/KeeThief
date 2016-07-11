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

using StrDict = System.Collections.Generic.SortedDictionary<string, string>;

namespace KeePass.Util
{
	public sealed class IniFile
	{
		private SortedDictionary<string, StrDict> m_vSections =
			new SortedDictionary<string, StrDict>();

		public IniFile()
		{
		}

		public static IniFile Read(string strFile, Encoding enc)
		{
			StreamReader sr = null;
			IniFile ini = new IniFile();

			try
			{
				sr = new StreamReader(strFile, enc);

				string strSection = string.Empty;
				while(true)
				{
					string str = sr.ReadLine();
					if(str == null) break; // End of stream

					str = str.Trim();
					if(str.Length == 0) continue;

					if(str.StartsWith("[") && str.EndsWith("]"))
						strSection = str.Substring(1, str.Length - 2);
					else
					{
						int iSep = str.IndexOf('=');
						if(iSep < 0) { Debug.Assert(false); }
						else
						{
							string strKey = str.Substring(0, iSep);
							string strValue = str.Substring(iSep + 1);

							if(!ini.m_vSections.ContainsKey(strSection))
								ini.m_vSections.Add(strSection, new StrDict());
							ini.m_vSections[strSection][strKey] = strValue;
						}
					}
				}
			}
			finally { if(sr != null) sr.Close(); }

			return ini;
		}

		public string Get(string strSection, string strKey)
		{
			if(strSection == null) throw new ArgumentNullException("strSection");
			if(strKey == null) throw new ArgumentNullException("strKey");

			StrDict dict;
			if(!m_vSections.TryGetValue(strSection, out dict)) return null;

			string strValue;
			if(!dict.TryGetValue(strKey, out strValue)) return null;

			return strValue;
		}
	}
}
