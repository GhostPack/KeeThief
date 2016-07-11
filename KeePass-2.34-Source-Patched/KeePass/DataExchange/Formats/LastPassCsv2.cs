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
using System.Drawing;
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 2.0.2-3.2.40+
	internal sealed class LastPassCsv2 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "LastPass CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_LastPass; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, StrUtil.Utf8, true);
			string strData = sr.ReadToEnd();
			sr.Close();

			CsvOptions opt = new CsvOptions();
			opt.BackslashIsEscape = false;

			CsvStreamReaderEx csr = new CsvStreamReaderEx(strData, opt);

			while(true)
			{
				string[] vLine = csr.ReadLine();
				if(vLine == null) break;

				AddEntry(vLine, pwStorage);
			}
		}

		private static void AddEntry(string[] vLine, PwDatabase pd)
		{
			Debug.Assert((vLine.Length == 0) || (vLine.Length == 7));
			if(vLine.Length < 5) return;

			// Skip header line
			if((vLine[1] == "username") && (vLine[2] == "password") &&
				(vLine[3] == "extra") && (vLine[4] == "name"))
				return;

			PwEntry pe = new PwEntry(true, true);

			PwGroup pg = pd.RootGroup;
			if(vLine.Length >= 6)
			{
				string strGroup = vLine[5];
				if(strGroup.Length > 0)
					pg = pg.FindCreateSubTree(strGroup, new string[1]{ "\\" }, true);
			}
			pg.AddEntry(pe, true);

			ImportUtil.AppendToField(pe, PwDefs.TitleField, vLine[4], pd);
			ImportUtil.AppendToField(pe, PwDefs.UserNameField, vLine[1], pd);
			ImportUtil.AppendToField(pe, PwDefs.PasswordField, vLine[2], pd);

			string strNotes = vLine[3];
			bool bIsSecNote = vLine[0].Equals("http://sn", StrUtil.CaseIgnoreCmp);
			if(bIsSecNote)
			{
				if(strNotes.StartsWith("NoteType:", StrUtil.CaseIgnoreCmp))
					AddNoteFields(pe, strNotes, pd);
				else ImportUtil.AppendToField(pe, PwDefs.NotesField, strNotes, pd);
			}
			else // Standard entry, no secure note
			{
				ImportUtil.AppendToField(pe, PwDefs.UrlField, vLine[0], pd);

				Debug.Assert(!strNotes.StartsWith("NoteType:"));
				ImportUtil.AppendToField(pe, PwDefs.NotesField, strNotes, pd);
			}

			if(vLine.Length >= 7)
			{
				if(StrUtil.StringToBool(vLine[6]))
					pe.AddTag("Favorite");
			}
		}

		private static void AddNoteFields(PwEntry pe, string strNotes,
			PwDatabase pd)
		{
			string strData = StrUtil.NormalizeNewLines(strNotes, false);
			string[] vLines = strData.Split('\n');

			string strFieldName = PwDefs.NotesField;
			bool bNotesFound = false;
			foreach(string strLine in vLines)
			{
				int iFieldLen = strLine.IndexOf(':');
				int iDataOffset = 0;
				if((iFieldLen > 0) && !bNotesFound)
				{
					string strRaw = strLine.Substring(0, iFieldLen).Trim();
					string strField = ImportUtil.MapNameToStandardField(strRaw, false);
					if(string.IsNullOrEmpty(strField)) strField = strRaw;

					if(strField.Length > 0)
					{
						strFieldName = strField;
						iDataOffset = iFieldLen + 1;

						bNotesFound |= (strRaw == "Notes"); // Not PwDefs.NotesField
					}
				}

				bool bSingle = ((strFieldName == PwDefs.TitleField) ||
					(strFieldName == PwDefs.UserNameField) ||
					(strFieldName == PwDefs.PasswordField) ||
					(strFieldName == PwDefs.UrlField));
				string strSep = (bSingle ? ", " : "\r\n");

				ImportUtil.AppendToField(pe, strFieldName, strLine.Substring(
					iDataOffset), pd, strSep, bSingle);
			}
		}
	}
}
