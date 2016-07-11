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
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 6.05-6.62+
	internal sealed class CodeWalletTxt605 : FileFormatProvider
	{
		private const string FieldSeparator = "*---------------------------------------------------";

		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "CodeWallet TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_CWallet; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Unicode);
			string strData = sr.ReadToEnd();
			sr.Close();

			string[] vLines = strData.Split(new char[]{ '\r', '\n' });

			bool bDoImport = false;
			PwEntry pe = new PwEntry(true, true);
			bool bInnerSep = false;
			bool bEmptyEntry = true;
			string strLastIndexedItem = string.Empty;
			string strLastLine = string.Empty;

			foreach(string strLine in vLines)
			{
				if(strLine.Length == 0) continue;

				if(strLine == FieldSeparator)
				{
					bInnerSep = !bInnerSep;
					if(bInnerSep && !bEmptyEntry)
					{
						pwStorage.RootGroup.AddEntry(pe, true);

						pe = new PwEntry(true, true);
						bEmptyEntry = true;
					}
					else if(!bInnerSep)
						pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
							pwStorage.MemoryProtection.ProtectTitle,
							strLastLine));

					bDoImport = true;
				}
				else if(bDoImport)
				{
					int nIDLen = strLine.IndexOf(": ");
					if(nIDLen > 0)
					{
						string strIndex = strLine.Substring(0, nIDLen);
						if(PwDefs.IsStandardField(strIndex))
							strIndex = Guid.NewGuid().ToString();

						pe.Strings.Set(strIndex, new ProtectedString(
							false, strLine.Remove(0, nIDLen + 2)));

						strLastIndexedItem = strIndex;
					}
					else if(!bEmptyEntry)
					{
						pe.Strings.Set(strLastIndexedItem, new ProtectedString(
							false, pe.Strings.ReadSafe(strLastIndexedItem) +
							MessageService.NewParagraph + strLine));
					}

					bEmptyEntry = false;
				}

				strLastLine = strLine;
			}
		}
	}
}
