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
	// 5.0.1+
	internal sealed class TurboPwsCsv5 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "TurboPasswords CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return Properties.Resources.B16x16_Imp_TurboPws; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();
			sInput.Close();

			Dictionary<string, PwGroup> dGroups = new Dictionary<string, PwGroup>();
			dGroups[string.Empty] = pwStorage.RootGroup;

			CsvOptions opt = new CsvOptions();
			opt.BackslashIsEscape = false;

			CsvStreamReaderEx csv = new CsvStreamReaderEx(strData, opt);

			while(true)
			{
				string[] v = csv.ReadLine();
				if(v == null) break;
				if(v.Length == 0) continue;
				if(v[0].StartsWith("TurboPasswords CSV Export File")) continue;
				if(v.Length < 24) { Debug.Assert(false); continue; }
				if((v[0] == "Category") && (v[1] == "Type")) continue;

				PwEntry pe = new PwEntry(true, true);

				PwGroup pg;
				string strGroup = v[0];
				if(!dGroups.TryGetValue(strGroup, out pg))
				{
					pg = new PwGroup(true, true, strGroup, PwIcon.Folder);
					dGroups[string.Empty].AddGroup(pg, true);
					dGroups[strGroup] = pg;
				}
				pg.AddEntry(pe, true);

				string strType = v[1];

				for(int f = 0; f < 6; ++f)
				{
					string strKey = v[2 + (2 * f)];
					string strValue = v[2 + (2 * f) + 1];
					if(strKey.Length == 0) strKey = PwDefs.NotesField;
					if(strValue.Length == 0) continue;

					if(strKey == "Description")
						strKey = PwDefs.TitleField;
					else if(((strType == "Contact") || (strType == "Personal Info")) &&
						(strKey == "Name"))
						strKey = PwDefs.TitleField;
					else if(((strType == "Membership") || (strType == "Insurance")) &&
						(strKey == "Company"))
						strKey = PwDefs.TitleField;
					else if(strKey == "SSN")
						strKey = PwDefs.UserNameField;
					else
					{
						string strMapped = ImportUtil.MapNameToStandardField(strKey, false);
						if(!string.IsNullOrEmpty(strMapped)) strKey = strMapped;
					}

					ImportUtil.AppendToField(pe, strKey, strValue, pwStorage,
						((strKey == PwDefs.NotesField) ? "\r\n" : ", "), false);
				}

				ImportUtil.AppendToField(pe, PwDefs.NotesField, v[20], pwStorage,
					"\r\n\r\n", false);
				if(v[21].Length > 0)
					ImportUtil.AppendToField(pe, "Login URL", v[21], pwStorage, null, true);
			}
		}
	}
}
