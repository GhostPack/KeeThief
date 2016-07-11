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
using System.Xml;
using System.Xml.Serialization;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 1.0.4
	internal sealed class PwMemory2008Xml104 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Password Memory 2008 XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_PwMem2008; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			string str = PwMemory2008Xml104.Preprocess(sInput);
			MemoryStream ms = new MemoryStream(StrUtil.Utf8.GetBytes(str), false);

			XmlSerializer xs = new XmlSerializer(typeof(Priv_PwMem2008XmlFile));
			Priv_PwMem2008XmlFile f = (Priv_PwMem2008XmlFile)xs.Deserialize(ms);
			ms.Close();

			if((f == null) || (f.Cells == null)) return;

			Dictionary<string, PwGroup> vGroups = new Dictionary<string, PwGroup>();

			for(int iLine = 2; iLine < f.Cells.Length; ++iLine)
			{
				string[] vCells = f.Cells[iLine];
				if((vCells == null) || (vCells.Length != 6)) continue;
				if((vCells[1] == null) || (vCells[2] == null) ||
					(vCells[3] == null) || (vCells[4] == null)) continue;

				string strGroup = vCells[4];
				PwGroup pg;
				if(strGroup == ".") pg = pwStorage.RootGroup;
				else if(vGroups.ContainsKey(strGroup)) pg = vGroups[strGroup];
				else
				{
					pg = new PwGroup(true, true);
					pg.Name = strGroup;
					pwStorage.RootGroup.AddGroup(pg, true);

					vGroups[strGroup] = pg;
				}

				PwEntry pe = new PwEntry(true, true);
				pg.AddEntry(pe, true);

				if(vCells[1] != ".")
					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectTitle, vCells[1]));
				if(vCells[2] != ".")
					pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectUserName, vCells[2]));
				if(vCells[3] != ".")
					pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectPassword, vCells[3]));
			}
		}

		private static string Preprocess(Stream sInput)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.UTF8);
			string str = sr.ReadToEnd();

			const string strStartTag = "<IMAGE";
			const string strEndTag = "</IMAGE>";

			while(true)
			{
				int nStart = str.IndexOf(strStartTag);
				int nEnd = str.IndexOf(strEndTag);

				if((nStart < 0) || (nEnd < 0)) break;

				str = str.Remove(nStart, nEnd - nStart + strEndTag.Length);
			}

			return str;
		}
	}

	[XmlRoot("CACHE")]
	public sealed class Priv_PwMem2008XmlFile
	{
		[XmlElement("TITLE")]
		public string Title { get; set; }

		[XmlArray("LINES")]
		[XmlArrayItem("LINE")]
		[XmlArrayItem("CELL", NestingLevel = 1)]
		public string[][] Cells { get; set; }
	}
}
