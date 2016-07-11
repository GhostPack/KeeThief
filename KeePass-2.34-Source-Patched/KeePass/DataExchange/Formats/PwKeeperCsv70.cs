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
	// 7.0
	internal sealed class PwKeeperCsv70 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Password Keeper CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_Whisper32; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[] { '\r', '\n' });

			foreach(string strLine in vLines)
			{
				if(strLine.Length > 5) ProcessCsvLine(strLine, pwStorage);
			}
		}

		private static void ProcessCsvLine(string strLine, PwDatabase pwStorage)
		{
			List<string> list = ImportUtil.SplitCsvLine(strLine, ",");
			Debug.Assert(list.Count == 5);

			PwEntry pe = new PwEntry(true, true);
			pwStorage.RootGroup.AddEntry(pe, true);

			if(list.Count == 5)
			{
				pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectTitle,
					ProcessCsvWord(list[0])));
				pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectUserName,
					ProcessCsvWord(list[1])));
				pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectPassword,
					ProcessCsvWord(list[2])));
				pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectUrl,
					ProcessCsvWord(list[3])));
				pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectNotes,
					ProcessCsvWord(list[4])));
			}
			else throw new FormatException("Invalid field count!");
		}

		private static string ProcessCsvWord(string strWord)
		{
			if(strWord == null) return string.Empty;
			if(strWord.Length < 2) return strWord;

			if(strWord.StartsWith("\"") && strWord.EndsWith("\""))
				return strWord.Substring(1, strWord.Length - 2);

			return strWord;
		}
	}
}
