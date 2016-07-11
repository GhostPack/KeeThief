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

namespace KeePass.DataExchange.Formats
{
	// 1.44 & Pro 1.07
	internal sealed class AnyPwCsv144 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Any Password CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }
		
		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_AnyPw; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[]{ '\r', '\n' },
				StringSplitOptions.RemoveEmptyEntries);

			foreach(string strLine in vLines)
			{
				if(strLine.Length > 5) ProcessCsvLine(strLine, pwStorage);
			}
		}

		private static void ProcessCsvLine(string strLine, PwDatabase pwStorage)
		{
			List<string> list = ImportUtil.SplitCsvLine(strLine, ",");
			Debug.Assert((list.Count == 6) || (list.Count == 7));
			if(list.Count < 6) return;
			bool bIsPro = (list.Count >= 7); // Std exports 6 fields only

			PwEntry pe = new PwEntry(true, true);
			pwStorage.RootGroup.AddEntry(pe, true);

			pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectTitle,
				ParseCsvWord(list[0], false)));
			pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectUserName,
				ParseCsvWord(list[1], false)));
			pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectPassword,
				ParseCsvWord(list[2], false)));
			pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectUrl,
				ParseCsvWord(list[3], false)));

			int p = 3;
			if(bIsPro)
				pe.Strings.Set(KPRes.Custom, new ProtectedString(false,
					ParseCsvWord(list[++p], false)));

			pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectNotes,
				ParseCsvWord(list[++p], true)));

			DateTime dt;
			if(DateTime.TryParse(ParseCsvWord(list[++p], false), out dt))
				pe.CreationTime = pe.LastAccessTime = pe.LastModificationTime = dt;
			else { Debug.Assert(false); }
		}

		private static string ParseCsvWord(string strWord, bool bFixCodes)
		{
			string str = strWord.Trim();

			if((str.Length >= 2) && str.StartsWith("\"") && str.EndsWith("\""))
				str = str.Substring(1, str.Length - 2);

			str = str.Replace("\"\"", "\"");

			if(bFixCodes)
			{
				str = str.Replace("<13>", string.Empty);
				str = str.Replace("<10>", "\r\n");
			}

			return str;
		}
	}
}
