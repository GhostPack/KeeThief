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
	// 4.02-5.3+
	internal sealed class SplashIdCsv402 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "SplashID CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }
		
		public override bool ImportAppendsToRootGroupOnly { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_SplashID; }
		}

		private const string StrHeader = "SplashID Export File";

		private static SplashIdMapping[] m_vMappings = null;
		private static SplashIdMapping[] SplashIdMappings
		{
			get
			{
				if(m_vMappings != null) return m_vMappings;

				m_vMappings = new SplashIdMapping[]{
					new SplashIdMapping("Bank Accts", PwIcon.Homebanking,
						new string[]{ PwDefs.TitleField, "Account #", PwDefs.PasswordField,
							PwDefs.UserNameField, "Branch", "Phone #" }),
					new SplashIdMapping("Birthdays", PwIcon.UserCommunication,
						new string[]{ PwDefs.TitleField, "Date" }),
					new SplashIdMapping("Calling Cards", PwIcon.EMail,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField,
							PwDefs.PasswordField }),
					new SplashIdMapping("Clothes Size", PwIcon.UserCommunication,
						new string[]{ PwDefs.TitleField, "Shirt Size", "Pant Size",
							"Shoe Size", "Dress Size" }),
					new SplashIdMapping("Combinations", PwIcon.Key,
						new string[]{ PwDefs.TitleField, PwDefs.PasswordField }),
					new SplashIdMapping("Credit Cards", PwIcon.UserKey,
						new string[]{ PwDefs.TitleField, "Card #", "Expiration Date",
							PwDefs.UserNameField, PwDefs.PasswordField, "Bank" }),
					new SplashIdMapping("Email Accts", PwIcon.EMail,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField,
							PwDefs.PasswordField, "POP3 Host", "SMTP Host" }),
					new SplashIdMapping("Emergency Info", PwIcon.UserCommunication,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField }),
					new SplashIdMapping("Frequent Flyer", PwIcon.PaperQ,
						new string[]{ PwDefs.TitleField, "Number",
							PwDefs.UserNameField, "Date" }),
					new SplashIdMapping("Identification", PwIcon.UserKey,
						new string[]{ PwDefs.TitleField, PwDefs.PasswordField,
							PwDefs.UserNameField, "Date" }),
					new SplashIdMapping("Insurance", PwIcon.ClipboardReady,
						new string[]{ PwDefs.TitleField, PwDefs.PasswordField,
							PwDefs.UserNameField, "Insured", "Date" }),
					new SplashIdMapping("Memberships", PwIcon.UserKey,
						new string[]{ PwDefs.TitleField, PwDefs.PasswordField,
							PwDefs.UserNameField, "Date" }),
					new SplashIdMapping("Phone Numbers", PwIcon.UserCommunication,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField }),
					new SplashIdMapping("Prescriptions", PwIcon.ClipboardReady,
						new string[]{ PwDefs.TitleField, PwDefs.PasswordField,
							PwDefs.UserNameField, "Doctor", "Pharmacy", "Phone #" }),
					new SplashIdMapping("Serial Numbers", PwIcon.Key,
						new string[]{ PwDefs.TitleField, PwDefs.PasswordField,
							"Purchase Date", "Reseller" }),
					new SplashIdMapping("Vehicle Info", PwIcon.PaperReady,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField,
							PwDefs.PasswordField }),
					new SplashIdMapping("Voice Mail", PwIcon.IRCommunication,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField,
							PwDefs.PasswordField }),
					new SplashIdMapping("Web Logins", PwIcon.UserKey,
						new string[]{ PwDefs.TitleField, PwDefs.UserNameField,
							PwDefs.PasswordField, PwDefs.UrlField })
				};
				return m_vMappings;
			}
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[]{ '\r', '\n' });
			SortedDictionary<string, PwGroup> dictGroups =
				new SortedDictionary<string, PwGroup>();

			foreach(string strLine in vLines)
			{
				if(strLine.Length > 0)
					ProcessCsvLine(strLine, pwStorage, dictGroups);
			}
		}

		private static void ProcessCsvLine(string strLine, PwDatabase pwStorage,
			SortedDictionary<string, PwGroup> dictGroups)
		{
			if(strLine == StrHeader) return; // Skip header

			List<string> list = ImportUtil.SplitCsvLine(strLine, ",");
			Debug.Assert(list.Count == 13);
			if(list.Count != 13) return;

			string strType = ParseCsvWord(list[0]);
			string strGroupName = ParseCsvWord(list[12]) + " - " + strType;

			SplashIdMapping mp = null;
			foreach(SplashIdMapping mpFind in SplashIdCsv402.SplashIdMappings)
			{
				if(mpFind.TypeName == strType)
				{
					mp = mpFind;
					break;
				}
			}

			PwIcon pwIcon = ((mp != null) ? mp.Icon : PwIcon.Key);

			PwGroup pg = null;
			if(dictGroups.ContainsKey(strGroupName))
				pg = dictGroups[strGroupName];
			else
			{
				PwIcon pwGroupIcon = ((pwIcon == PwIcon.Key) ?
					PwIcon.FolderOpen : pwIcon);

				pg = new PwGroup(true, true, strGroupName, pwGroupIcon);
				pwStorage.RootGroup.AddGroup(pg, true);
				dictGroups.Add(strGroupName, pg);
			}

			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			pe.IconId = pwIcon;

			for(int iField = 0; iField < 9; ++iField)
			{
				string strData = ParseCsvWord(list[iField + 1]);
				if(strData.Length == 0) continue;

				string strLookup = ((mp != null) ? mp.FieldNames[iField] :
					null);
				string strField = (strLookup ?? ("Field " + (iField + 1).ToString()));

				pe.Strings.Set(strField, new ProtectedString(false,
					strData));
			}

			pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectNotes,
				ParseCsvWord(list[11])));

			DateTime? dt = TimeUtil.ParseUSTextDate(ParseCsvWord(list[10]));
			if(dt.HasValue)
			{
				pe.LastAccessTime = dt.Value;
				pe.LastModificationTime = dt.Value;
			}
		}

		private static string ParseCsvWord(string strWord)
		{
			string str = strWord.Trim(new char[]{ '\"' });

			str = str.Replace("\"\"", "\""); // Unescape quotes

			str = str.Replace((char)0x0B, '\n'); // 0x0B = new line
			str = str.Replace("\r", string.Empty);
			str = str.Replace("\n", "\r\n"); // To Windows new lines

			return str;
		}

		private sealed class SplashIdMapping
		{
			private string m_strTypeName;
			public string TypeName
			{
				get { return m_strTypeName; }
			}

			private PwIcon m_pwIcon;
			public PwIcon Icon
			{
				get { return m_pwIcon; }
			}

			private string[] m_vFieldNames = new string[9];
			public string[] FieldNames
			{
				get { return m_vFieldNames; }
			}

			public SplashIdMapping(string strTypeName, PwIcon pwIcon, string[] vFieldNames)
			{
				Debug.Assert(strTypeName != null);
				if(strTypeName == null) throw new ArgumentNullException("strTypeName");

				Debug.Assert(vFieldNames != null);
				if(vFieldNames == null) throw new ArgumentNullException("vFieldNames");

				m_strTypeName = strTypeName;
				m_pwIcon = pwIcon;

				for(int i = 0; i < Math.Min(m_vFieldNames.Length, vFieldNames.Length); ++i)
					m_vFieldNames[i] = vFieldNames[i];
			}
		}
	}
}
