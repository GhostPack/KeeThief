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
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Native;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 2.3.2+
	internal sealed class DashlaneCsv2 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Dashlane CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_Dashlane; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, StrUtil.Utf8, true);
			string str = sr.ReadToEnd();
			sr.Close();

			// All fields are enclosed in '"', however '"' in data is
			// not encoded (broken format)

			str = str.Trim();
			str = StrUtil.NormalizeNewLines(str, false); // To Unix
			char chFieldSep = StrUtil.GetUnusedChar(str);
			str = str.Replace("\",\"", new string(chFieldSep, 1));
			char chRecSep = StrUtil.GetUnusedChar(str);
			str = str.Replace("\"\n\"", new string(chRecSep, 1));
			if(str.StartsWith("\"") && str.EndsWith("\"") && (str.Length >= 2))
				str = str.Substring(1, str.Length - 2);
			else { Debug.Assert(false); }
			if(!NativeLib.IsUnix()) str = StrUtil.NormalizeNewLines(str, true);

			CsvOptions opt = new CsvOptions();
			opt.BackslashIsEscape = false;
			opt.FieldSeparator = chFieldSep;
			opt.RecordSeparator = chRecSep;
			opt.TextQualifier = char.MinValue;

			CsvStreamReaderEx csr = new CsvStreamReaderEx(str, opt);

			while(true)
			{
				string[] vLine = csr.ReadLine();
				if(vLine == null) break;

				AddEntry(vLine, pwStorage);
			}
		}

		private static Regex m_rxIsDate = null;
		private static Regex m_rxIsGuid = null;
		private static void AddEntry(string[] vLine, PwDatabase pd)
		{
			int n = vLine.Length;
			if(n == 0) return;

			PwEntry pe = new PwEntry(true, true);
			pd.RootGroup.AddEntry(pe, true);

			string[] vFields = null;
			if(n == 2)
				vFields = new string[2] { PwDefs.TitleField, PwDefs.UrlField };
			else if(n == 3)
				vFields = new string[3] { PwDefs.TitleField, PwDefs.UrlField,
					PwDefs.UserNameField };
			else if(n == 4)
			{
				if((vLine[2].Length == 0) && (vLine[3].Length == 0))
					vFields = new string[4] { PwDefs.TitleField, PwDefs.UserNameField,
						PwDefs.NotesField, PwDefs.NotesField };
				else
					vFields = new string[4] { PwDefs.TitleField, PwDefs.NotesField,
						PwDefs.UserNameField, PwDefs.NotesField };
			}
			else if(n == 5)
				vFields = new string[5] { PwDefs.TitleField, PwDefs.UrlField,
					PwDefs.UserNameField, PwDefs.PasswordField, PwDefs.NotesField };
			else if(n == 6)
				vFields = new string[6] { PwDefs.TitleField, PwDefs.UrlField,
					PwDefs.UserNameField, PwDefs.UserNameField, PwDefs.PasswordField,
					PwDefs.NotesField };
			else if(n == 7)
				vFields = new string[7] { PwDefs.TitleField, PwDefs.UserNameField,
					PwDefs.NotesField, PwDefs.NotesField, PwDefs.NotesField,
					PwDefs.NotesField, PwDefs.NotesField };

			if(m_rxIsDate == null)
			{
				m_rxIsDate = new Regex(@"^\d{4}-\d+-\d+$");
				m_rxIsGuid = new Regex(
					@"^\{[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\}$");
			}

			if((vLine[0].Length == 0) && (n >= 2) && m_rxIsDate.IsMatch(vLine[1]))
			{
				vFields = null;

				vLine[0] = KPRes.Id;
				for(int i = 1; i < n; ++i)
				{
					string strPart = vLine[i];
					if(strPart.Equals("NO_TYPE", StrUtil.CaseIgnoreCmp) ||
						m_rxIsGuid.IsMatch(strPart))
						vLine[i] = string.Empty;
				}
			}

			for(int i = 0; i < n; ++i)
			{
				string str = vLine[i];
				if(str.Length == 0) continue;
				if(str.Equals("dashlaneappcredential", StrUtil.CaseIgnoreCmp))
					continue;

				string strField = ((vFields != null) ? vFields[i] : null);
				if(strField == null)
				{
					if(i == 0) strField = PwDefs.TitleField;
					else strField = PwDefs.NotesField;
				}

				if((strField == PwDefs.UrlField) && (str.IndexOf('.') >= 0))
					str = ImportUtil.FixUrl(str);

				ImportUtil.AppendToField(pe, strField, str, pd, ((strField ==
					PwDefs.NotesField) ? MessageService.NewLine : ", "), false);
			}
		}
	}
}
