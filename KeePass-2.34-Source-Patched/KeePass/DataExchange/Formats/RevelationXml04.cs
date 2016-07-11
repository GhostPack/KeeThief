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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Utility;
using KeePassLib.Security;

namespace KeePass.DataExchange.Formats
{
	internal sealed class RevelationXml04 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Revelation XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_Revelation; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.UTF8);
			string strDoc = sr.ReadToEnd();
			sr.Close();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(strDoc);

			ProcessEntries(pwStorage, pwStorage.RootGroup,
				doc.DocumentElement.ChildNodes);
		}

		private static void ProcessEntries(PwDatabase pd, PwGroup pgParent,
			XmlNodeList xlNodes)
		{
			foreach(XmlNode xmlChild in xlNodes)
			{
				if(xmlChild.Name == "entry")
				{
					XmlNode xnType = xmlChild.Attributes.GetNamedItem("type");
					if(xnType == null) { Debug.Assert(false); }
					else
					{
						if(xnType.Value == "folder")
							ImportGroup(pd, pgParent, xmlChild);
						else ImportEntry(pd, pgParent, xmlChild);
					}
				}
			}
		}

		private static void ImportGroup(PwDatabase pd, PwGroup pgParent, XmlNode xmlNode)
		{
			PwGroup pg = new PwGroup(true, true);

			foreach(XmlNode xmlChild in xmlNode.ChildNodes)
			{
				if(xmlChild.Name == "name")
					pg.Name = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == "description")
					pg.Notes = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == "entry") { }
				else if(xmlChild.Name == "updated")
					pg.LastModificationTime = ImportTime(xmlChild);
				else { Debug.Assert(false); }
			}

			pgParent.AddGroup(pg, true);

			ProcessEntries(pd, pg, xmlNode.ChildNodes);
		}

		private static void ImportEntry(PwDatabase pd, PwGroup pgParent, XmlNode xmlNode)
		{
			PwEntry pe = new PwEntry(true, true);
			pgParent.AddEntry(pe, true);

			foreach(XmlNode xmlChild in xmlNode.ChildNodes)
			{
				if(xmlChild.Name == "name")
					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pd.MemoryProtection.ProtectTitle, XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == "description")
					pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
						pd.MemoryProtection.ProtectNotes, XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == "updated")
					pe.LastModificationTime = ImportTime(xmlChild);
				else if(xmlChild.Name == "field")
				{
					XmlNode xnName = xmlChild.Attributes.GetNamedItem("id");
					if(xnName == null) { Debug.Assert(false); }
					else
					{
						KeyValuePair<string, bool> kvp = MapFieldName(xnName.Value, pd);
						pe.Strings.Set(kvp.Key, new ProtectedString(kvp.Value,
							XmlUtil.SafeInnerText(xmlChild)));
					}
				}
				else { Debug.Assert(false); }
			}
		}

		private static KeyValuePair<string, bool> MapFieldName(string strFieldName,
			PwDatabase pdContext)
		{
			switch(strFieldName)
			{
				case "creditcard-cardnumber":
				case "generic-username":
				case "generic-location":
				case "phone-phonenumber":
					return new KeyValuePair<string, bool>(PwDefs.UserNameField,
						pdContext.MemoryProtection.ProtectUserName);
				case "generic-code":
				case "generic-password":
				case "generic-pin":
					return new KeyValuePair<string, bool>(PwDefs.PasswordField,
						pdContext.MemoryProtection.ProtectPassword);
				case "generic-hostname":
				case "generic-url":
					return new KeyValuePair<string, bool>(PwDefs.UrlField,
						pdContext.MemoryProtection.ProtectUrl);
				case "creditcard-cardtype":
					return new KeyValuePair<string, bool>("Card Type", false);
				case "creditcard-expirydate":
					return new KeyValuePair<string, bool>(KPRes.ExpiryTime, false);
				case "creditcard-ccv":
					return new KeyValuePair<string, bool>("CCV Number", false);
				case "generic-certificate":
					return new KeyValuePair<string, bool>("Certificate", false);
				case "generic-keyfile":
					return new KeyValuePair<string, bool>("Key File", false);
				case "generic-database":
					return new KeyValuePair<string, bool>(KPRes.Database, false);
				case "generic-email":
					return new KeyValuePair<string, bool>(KPRes.EMail, false);
				case "generic-port":
					return new KeyValuePair<string, bool>("Port", false);
				case "generic-domain":
					return new KeyValuePair<string, bool>("Domain", false);
				default: Debug.Assert(false); break;
			}

			return new KeyValuePair<string, bool>(strFieldName, false);
		}

		private static DateTime ImportTime(XmlNode xn)
		{
			string str = XmlUtil.SafeInnerText(xn);

			double dtUnix;
			if(!double.TryParse(str, out dtUnix)) { Debug.Assert(false); }
			else return TimeUtil.ConvertUnixTime(dtUnix);

			return DateTime.Now;
		}
	}
}
