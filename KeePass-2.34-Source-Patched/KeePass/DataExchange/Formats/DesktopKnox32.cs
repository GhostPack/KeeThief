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
using System.Xml;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 3.2+
	internal sealed class DesktopKnox32 : FileFormatProvider
	{
		private const string ElemRoot = "SafeCatalog";

		private const string ElemEntry = "SafeElement";

		private const string ElemCategory = "Category";
		private const string ElemTitle = "Title";
		private const string ElemNotes = "Content";

		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "DesktopKnox XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_DesktopKnox; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, StrUtil.Utf8);
			string strDoc = sr.ReadToEnd();
			sr.Close();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(strDoc);

			XmlElement xmlRoot = doc.DocumentElement;
			Debug.Assert(xmlRoot.Name == ElemRoot);

			Dictionary<string, PwGroup> dictGroups = new Dictionary<string, PwGroup>();
			dictGroups[string.Empty] = pwStorage.RootGroup;

			foreach(XmlNode xmlChild in xmlRoot.ChildNodes)
			{
				if(xmlChild.Name == ElemEntry)
					ImportEntry(xmlChild, pwStorage, dictGroups);
				else { Debug.Assert(false); }
			}
		}

		private static void ImportEntry(XmlNode xmlNode, PwDatabase pwStorage,
			Dictionary<string, PwGroup> dGroups)
		{
			PwEntry pe = new PwEntry(true, true);
			string strGroup = string.Empty;

			foreach(XmlNode xmlChild in xmlNode)
			{
				string strInner = XmlUtil.SafeInnerText(xmlChild);

				if(xmlChild.Name == ElemCategory)
					strGroup = strInner;
				else if(xmlChild.Name == ElemTitle)
					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectTitle, strInner));
				else if(xmlChild.Name == ElemNotes)
					pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectNotes, strInner));
			}

			PwGroup pg;
			dGroups.TryGetValue(strGroup, out pg);
			if(pg == null)
			{
				pg = new PwGroup(true, true);
				pg.Name = strGroup;
				dGroups[string.Empty].AddGroup(pg, true);
				dGroups[strGroup] = pg;
			}
			pg.AddEntry(pe, true);
		}
	}
}
