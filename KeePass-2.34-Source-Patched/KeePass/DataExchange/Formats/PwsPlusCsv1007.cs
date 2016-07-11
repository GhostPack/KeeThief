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
using System.IO;
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	internal sealed class PwsPlusCsv1007 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Passwords Plus CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_PwsPlus; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			CsvStreamReader csv = new CsvStreamReader(strData, true);
			Dictionary<string, PwGroup> dictGroups = new Dictionary<string, PwGroup>();

			while(true)
			{
				string[] vLine = csv.ReadLine();
				if(vLine == null) break;
				if(vLine.Length == 0) continue; // Skip empty line
				if(vLine.Length == 5) continue; // Skip header line
				if(vLine.Length != 34) { Debug.Assert(false); continue; }

				string strType = vLine[0].Trim();
				if(strType.Equals("Is Template", StrUtil.CaseIgnoreCmp)) continue;
				if(strType.Equals("1")) continue; // Skip template

				string strGroup = vLine[2].Trim();
				PwGroup pg;
				if(strGroup.Length == 0) pg = pwStorage.RootGroup;
				else
				{
					if(dictGroups.ContainsKey(strGroup)) pg = dictGroups[strGroup];
					else
					{
						pg = new PwGroup(true, true, strGroup, PwIcon.Folder);
						pwStorage.RootGroup.AddGroup(pg, true);
						dictGroups[strGroup] = pg;
					}
				}

				PwEntry pe = new PwEntry(true, true);
				pg.AddEntry(pe, true);

				string strTitle = vLine[1].Trim();
				if(strTitle.Length > 0)
					ImportUtil.AppendToField(pe, PwDefs.TitleField, strTitle, pwStorage);

				for(int i = 0; i < 10; ++i)
				{
					string strKey = vLine[(i * 3) + 3].Trim();
					string strValue = vLine[(i * 3) + 4].Trim();
					if((strKey.Length == 0) || (strValue.Length == 0)) continue;

					string strMapped = ImportUtil.MapNameToStandardField(strKey, true);
					if(string.IsNullOrEmpty(strMapped)) strMapped = strKey;
					ImportUtil.AppendToField(pe, strMapped, strValue, pwStorage);
				}

				string strNotesPre = pe.Strings.ReadSafe(PwDefs.NotesField);
				string strNotes = vLine[33].Trim();
				if(strNotes.Length > 0)
				{
					if(strNotesPre.Length == 0)
						ImportUtil.AppendToField(pe, PwDefs.NotesField, strNotes, pwStorage);
					else
						pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
							((pwStorage == null) ? false :
							pwStorage.MemoryProtection.ProtectNotes), strNotesPre +
							Environment.NewLine + Environment.NewLine + strNotes));
				}
			}
		}
	}
}
