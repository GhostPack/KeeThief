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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;

namespace KeePass.DataExchange.Formats
{
	// 4.50
	internal sealed class PinsTxt450 : FileFormatProvider
	{
		private const string FirstLine = "\"Category\"\t\"System\"\t\"User\"\t" +
			"\"Password\"\t\"URL/Comments\"\t\"Custom\"\t\"Start date\"\t\"Expires\"\t" +
			"\"More info\"";
		private const string FieldSeparator = "\"\t\"";

		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "PINs TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_PINs; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[]{ '\r', '\n' });

			bool bFirst = true;
			foreach(string strLine in vLines)
			{
				if(bFirst)
				{
					if(strLine != FirstLine)
						throw new FormatException("Format error. First line is invalid. Read the documentation.");

					bFirst = false;
				}
				else if(strLine.Length > 5) ImportLine(strLine, pwStorage);
			}
		}

		private static void ImportLine(string strLine, PwDatabase pwStorage)
		{
			string[] vParts = strLine.Split(new string[] { FieldSeparator },
				StringSplitOptions.None);
			Debug.Assert(vParts.Length == 9);
			if(vParts.Length != 9)
				throw new FormatException("Line:\r\n" + strLine);

			vParts[0] = vParts[0].Remove(0, 1);
			vParts[8] = vParts[8].Substring(0, vParts[8].Length - 1);

			vParts[8] = vParts[8].Replace("||", "\r\n");

			PwGroup pg = pwStorage.RootGroup.FindCreateGroup(vParts[0], true);
			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectTitle, vParts[1]));
			pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectUserName, vParts[2]));
			pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectPassword, vParts[3]));
			pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectUrl, vParts[4]));
			
			if(vParts[5].Length > 0)
				pe.Strings.Set("Custom", new ProtectedString(false, vParts[5]));

			DateTime dt;
			if((vParts[6].Length > 0) && DateTime.TryParse(vParts[6], out dt))
				pe.CreationTime = pe.LastModificationTime = pe.LastAccessTime = dt;

			if((vParts[7].Length > 0) && DateTime.TryParse(vParts[7], out dt))
			{
				pe.ExpiryTime = dt;
				pe.Expires = true;
			}

			pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
				pwStorage.MemoryProtection.ProtectNotes, vParts[8]));
		}
	}
}
