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

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;

namespace KeePass.DataExchange.Formats
{
	// 1.4
	internal sealed class PVaultTxt14 : FileFormatProvider
	{
		private const string InitGroup = "************";
		private const string InitNewEntry = "----------------------";

		private const string InitTitle = "Account:      ";
		private const string InitUser = "User Name:    ";
		private const string InitPassword = "Password:     ";
		private const string InitURL = "Hyperlink:    ";
		private const string InitEMail = "Email:        ";

		private const string InitNotes = "Comments:     ";
		private const string ContinueNotes = "              ";

		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Personal Vault TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_PVault; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[]{ '\r', '\n' });

			PwGroup pg = pwStorage.RootGroup;
			PwEntry pe = new PwEntry(true, true);

			foreach(string strLine in vLines)
			{
				if(strLine.StartsWith(InitGroup))
				{
					string strGroup = strLine.Remove(0, InitGroup.Length);
					if(strGroup.Length > InitGroup.Length)
						strGroup = strGroup.Substring(0, strGroup.Length - InitGroup.Length);

					pg = pwStorage.RootGroup.FindCreateGroup(strGroup, true);

					pe = new PwEntry(true, true);
					pg.AddEntry(pe, true);
				}
				else if(strLine.StartsWith(InitNewEntry))
				{
					pe = new PwEntry(true, true);
					pg.AddEntry(pe, true);
				}
				else if(strLine.StartsWith(InitTitle))
					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectTitle,
						strLine.Remove(0, InitTitle.Length)));
				else if(strLine.StartsWith(InitUser))
					pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectUserName,
						strLine.Remove(0, InitUser.Length)));
				else if(strLine.StartsWith(InitPassword))
					pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectPassword,
						strLine.Remove(0, InitPassword.Length)));
				else if(strLine.StartsWith(InitURL))
					pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectUrl,
						strLine.Remove(0, InitURL.Length)));
				else if(strLine.StartsWith(InitEMail))
					pe.Strings.Set("E-Mail", new ProtectedString(
						false,
						strLine.Remove(0, InitEMail.Length)));
				else if(strLine.StartsWith(InitNotes))
					pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectNotes,
						strLine.Remove(0, InitNotes.Length)));
				else if(strLine.StartsWith(ContinueNotes))
					pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectNotes,
						pe.Strings.ReadSafe(PwDefs.NotesField) + "\r\n" +
						strLine.Remove(0, ContinueNotes.Length)));
			}
		}
	}
}
