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
	// 3.1.4
	internal sealed class ZdnPwProTxt314 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "ZDNet's Password Pro TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_ZdnPwPro; }
		}

		private const string StrFieldUser = "User Name: ";
		private const string StrFieldPw = "Password: ";
		private const string StrFieldUrl = "Shortcut: ";
		private const string StrFieldExpires = "Expires: ";
		private const string StrFieldType = "Type: ";
		private const string StrFieldNotes = "Comments: ";

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			strData = strData.Replace("\r", string.Empty);
			string[] vLines = strData.Split(new char[] { '\n' });

			if(vLines.Length >= 1)
			{
				Debug.Assert(vLines[0].StartsWith("Contents of: "));
				vLines[0] = string.Empty; // Trigger 'new entry' below
			}

			Dictionary<string, string> dItems = new Dictionary<string, string>();
			bool bInNotes = false;
			DateTime? dtExpire = null;

			for(int i = 0; i < vLines.Length; ++i)
			{
				string strLine = vLines[i];

				if((i + 2) < vLines.Length)
				{
					string strSep = new string('-', vLines[i + 1].Length);

					if((strLine.Length == 0) && (vLines[i + 2] == strSep) &&
						(strSep.Length > 0))
					{
						AddEntry(pwStorage.RootGroup, dItems, ref bInNotes, ref dtExpire);
						dItems.Clear();

						dItems[PwDefs.TitleField] = vLines[i + 1];

						i += 2;
						continue;
					}
				}

				if(bInNotes)
				{
					if(dItems.ContainsKey(PwDefs.NotesField))
						dItems[PwDefs.NotesField] += MessageService.NewLine + strLine;
					else dItems[PwDefs.NotesField] = strLine;
				}
				else if(strLine.StartsWith(StrFieldUser))
					AddField(dItems, PwDefs.UserNameField, strLine.Substring(
						StrFieldUser.Length));
				else if(strLine.StartsWith(StrFieldPw))
					AddField(dItems, PwDefs.PasswordField, strLine.Substring(
						StrFieldPw.Length));
				else if(strLine.StartsWith(StrFieldUrl))
					AddField(dItems, PwDefs.UrlField, strLine.Substring(
						StrFieldUrl.Length));
				else if(strLine.StartsWith(StrFieldType))
					AddField(dItems, "Type", strLine.Substring(StrFieldType.Length));
				else if(strLine.StartsWith(StrFieldExpires))
				{
					string strExp = strLine.Substring(StrFieldExpires.Length);

					DateTime dtExp;
					if(DateTime.TryParse(strExp, out dtExp)) dtExpire = dtExp;
					else { Debug.Assert(false); }
				}
				else if(strLine.StartsWith(StrFieldNotes))
				{
					AddField(dItems, PwDefs.NotesField, strLine.Substring(
						StrFieldNotes.Length));
					bInNotes = true;
				}
				else { Debug.Assert(false); }
			}

			AddEntry(pwStorage.RootGroup, dItems, ref bInNotes, ref dtExpire);
			Debug.Assert(!dtExpire.HasValue);
		}

		private static void AddField(Dictionary<string, string> dItems,
			string strKey, string strValue)
		{
			if(!dItems.ContainsKey(strKey))
			{
				dItems[strKey] = strValue;
				return;
			}

			string strPreValue = dItems[strKey];
			if((strPreValue.Length > 0) && (strValue.Length > 0))
				strPreValue += ", ";

			dItems[strKey] = strPreValue + strValue;
		}

		private static void AddEntry(PwGroup pg, Dictionary<string, string> dItems,
			ref bool bInNotes, ref DateTime? dtExpire)
		{
			if(dItems.Count > 0)
			{
				PwEntry pe = new PwEntry(true, true);
				pg.AddEntry(pe, true);

				foreach(KeyValuePair<string, string> kvp in dItems)
					pe.Strings.Set(kvp.Key, new ProtectedString(false, kvp.Value));

				if(dtExpire.HasValue)
				{
					pe.Expires = true;
					pe.ExpiryTime = dtExpire.Value;
				}
			}

			bInNotes = false;
			dtExpire = null;
		}
	}
}
