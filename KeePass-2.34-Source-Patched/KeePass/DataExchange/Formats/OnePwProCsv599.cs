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
using System.Globalization;
using System.Drawing;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 1Password Pro 5.99 and 1PW 6.15-7.05+
	internal sealed class OnePwProCsv599 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return @"1PW & 1Password Pro CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_OnePwPro; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default, true);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[] { '\r', '\n' },
				StringSplitOptions.RemoveEmptyEntries);

			Dictionary<string, PwGroup> dictGroups = new Dictionary<string, PwGroup>();
			foreach(string strLine in vLines)
			{
				ProcessCsvLine(strLine, pwStorage, dictGroups);
			}
		}

		private static void ProcessCsvLine(string strLine, PwDatabase pwStorage,
			Dictionary<string, PwGroup> dictGroups)
		{
			if(strLine == "\"Bezeichnung\"\t\"User/ID\"\t\"1.Passwort\"\t\"Url/Programm\"\t\"Geändert am\"\t\"Bemerkung\"\t\"2.Passwort\"\t\"Läuft ab\"\t\"Kategorie\"\t\"Eigene Felder\"")
				return;

			string str = strLine;
			if(str.StartsWith("\"") && str.EndsWith("\""))
				str = str.Substring(1, str.Length - 2);
			else { Debug.Assert(false); }

			string[] list = str.Split(new string[]{ "\"\t\"" }, StringSplitOptions.None);

			int iOffset;
			if(list.Length == 11) iOffset = 0; // 1Password Pro 5.99
			else if(list.Length == 10) iOffset = -1; // 1PW 6.15
			else if(list.Length > 11) iOffset = 0; // Unknown extension
			else return;

			string strGroup = list[9 + iOffset];
			PwGroup pg;
			if(dictGroups.ContainsKey(strGroup)) pg = dictGroups[strGroup];
			else
			{
				pg = new PwGroup(true, true, strGroup, PwIcon.Folder);
				pwStorage.RootGroup.AddGroup(pg, true);
				dictGroups[strGroup] = pg;
			}

			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectTitle,
				ParseCsvWord(list[1 + iOffset])));
			pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectUserName,
				ParseCsvWord(list[2 + iOffset])));
			pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectPassword,
				ParseCsvWord(list[3 + iOffset])));
			pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectUrl,
				ParseCsvWord(list[4 + iOffset])));
			pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectNotes,
				ParseCsvWord(list[6 + iOffset])));
			pe.Strings.Set(PwDefs.PasswordField + " 2", new ProtectedString(
				pwStorage.MemoryProtection.ProtectPassword,
				ParseCsvWord(list[7 + iOffset])));

			// 1Password Pro only:
			// Debug.Assert(list[9] == list[0]); // Very mysterious format...

			DateTime dt;
			if(ParseDateTime(list[5 + iOffset], out dt))
			{
				pe.CreationTime = pe.LastAccessTime = pe.LastModificationTime = dt;
			}
			else { Debug.Assert(false); }

			if(ParseDateTime(list[8 + iOffset], out dt))
			{
				pe.Expires = true;
				pe.ExpiryTime = dt;
			}

			AddCustomFields(pe, list[10 + iOffset]);
		}

		private static string ParseCsvWord(string strWord)
		{
			string str = strWord;

			str = str.Replace("\\r", string.Empty);
			str = str.Replace("\\n", "\r\n");

			return str;
		}

		private static bool ParseDateTime(string str, out DateTime dt)
		{
			dt = DateTime.MinValue;
			if(string.IsNullOrEmpty(str)) return false;
			if(str.Trim().Equals("nie", StrUtil.CaseIgnoreCmp)) return false;
			if(str.Trim().Equals("never", StrUtil.CaseIgnoreCmp)) return false;
			if(str.Trim().Equals("morgen", StrUtil.CaseIgnoreCmp))
			{
				dt = DateTime.Now.AddDays(1.0);
				return true;
			}

			string[] list = str.Split(new char[]{ '.', '\r', '\n', ' ', '\t',
				'-', ':' }, StringSplitOptions.RemoveEmptyEntries);

			try
			{
				if(list.Length == 6)
					dt = new DateTime(int.Parse(list[2]), int.Parse(list[1]),
						int.Parse(list[0]), int.Parse(list[3]), int.Parse(list[4]),
						int.Parse(list[5]));
				else if(list.Length == 3)
					dt = new DateTime(int.Parse(list[2]), int.Parse(list[1]),
						int.Parse(list[0]));
				else { Debug.Assert(false); return false; }
			}
			catch(Exception) { Debug.Assert(false); return false; }

			return true;
		}

		private static void AddCustomFields(PwEntry pe, string strCustom)
		{
			string[] vItems = strCustom.Split(new string[] { @"|~#~|" },
				StringSplitOptions.RemoveEmptyEntries);

			foreach(string strItem in vItems)
			{
				string[] vData = strItem.Split(new char[] { '|' },
					StringSplitOptions.None);

				if(vData.Length >= 3)
				{
					string strValue = vData[2];
					for(int i = 3; i < vData.Length; ++i)
						strValue += @"|" + vData[i];

					pe.Strings.Set(vData[1], new ProtectedString(false,
						strValue));
				}
				else { Debug.Assert(false); }
			}
		}
	}
}
