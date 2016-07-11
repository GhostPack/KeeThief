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
	// 4.7.35
	internal sealed class DataVaultCsv47 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "DataVault CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }
		
		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_DataVault; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			// Fix broken newlines
			strData = strData.Replace("\r\r\n", "\r\n");

			CsvStreamReader csv = new CsvStreamReader(strData, false);
			while(true)
			{
				string[] v = csv.ReadLine();
				if(v == null) break;
				if(v.Length == 0) continue;

				PwEntry pe = new PwEntry(true, true);
				pwStorage.RootGroup.AddEntry(pe, true);

				pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectTitle, v[0]));

				int p = 1;
				while((p + 1) < v.Length)
				{
					string strMapped = ImportUtil.MapNameToStandardField(v[p], true);
					string strKey = (string.IsNullOrEmpty(strMapped) ? v[p] : strMapped);
					string strValue = v[p + 1];

					p += 2;

					if((strKey.Length == 0) && (strValue.Length == 0)) continue;

					AppendToString(pe, strKey, strValue);
				}

				if((p < v.Length) && !string.IsNullOrEmpty(v[p]))
					AppendToString(pe, PwDefs.NotesField, v[p]);
			}
		}

		private static void AppendToString(PwEntry pe, string strKey, string strValue)
		{
			if(pe.Strings.ReadSafe(strKey).Length > 0)
			{
				pe.Strings.Set(strKey, new ProtectedString(false,
					pe.Strings.ReadSafe(strKey) + ", " + strValue));
			}
			else pe.Strings.Set(strKey, new ProtectedString(false, strValue));
		}
	}
}
