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
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 2013.4.0.10
	internal sealed class NortonIdSafeCsv2013 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Norton Identity Safe CSV"; } }
		public override string DefaultExtension { get { return "csv"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_NortonIdSafe; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Unicode, true);
			string strData = sr.ReadToEnd();
			sr.Close();

			CsvOptions opt = new CsvOptions();
			opt.BackslashIsEscape = false;

			CsvStreamReaderEx csv = new CsvStreamReaderEx(strData, opt);

			while(true)
			{
				string[] v = csv.ReadLine();
				if(v == null) break;
				if(v.Length < 5) continue;

				if(v[0].Equals("url", StrUtil.CaseIgnoreCmp) &&
					v[1].Equals("username", StrUtil.CaseIgnoreCmp) &&
					v[2].Equals("password", StrUtil.CaseIgnoreCmp))
					continue; // Header

				PwGroup pg = pwStorage.RootGroup;
				string strGroup = v[4];
				if(!string.IsNullOrEmpty(strGroup))
					pg = pg.FindCreateGroup(strGroup, true);

				PwEntry pe = new PwEntry(true, true);
				pg.AddEntry(pe, true);

				ImportUtil.AppendToField(pe, PwDefs.UrlField, v[0], pwStorage);
				ImportUtil.AppendToField(pe, PwDefs.UserNameField, v[1], pwStorage);
				ImportUtil.AppendToField(pe, PwDefs.PasswordField, v[2], pwStorage);
				ImportUtil.AppendToField(pe, PwDefs.TitleField, v[3], pwStorage);
			}
		}
	}
}
