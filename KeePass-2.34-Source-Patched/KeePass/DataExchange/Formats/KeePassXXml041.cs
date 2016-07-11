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
using System.Diagnostics;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 0.4.1
	internal sealed class KeePassXXml041 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "KeePassX XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool SupportsUuids { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_KeePassX; }
		}

		private const string ElemRoot = "database";

		private const string ElemGroup = "group";
		private const string ElemTitle = "title";
		private const string ElemIcon = "icon";

		private const string ElemEntry = "entry";
		private const string ElemUserName = "username";
		private const string ElemUrl = "url";
		private const string ElemPassword = "password";
		private const string ElemNotes = "comment";
		private const string ElemCreationTime = "creation";
		private const string ElemLastModTime = "lastmod";
		private const string ElemLastAccessTime = "lastaccess";
		private const string ElemExpiryTime = "expire";
		private const string ElemAttachDesc = "bindesc";
		private const string ElemAttachment = "bin";

		private const string ValueNever = "Never";

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(sInput);

			XmlNode xmlRoot = xmlDoc.DocumentElement;
			Debug.Assert(xmlRoot.Name == ElemRoot);

			Stack<PwGroup> vGroups = new Stack<PwGroup>();
			vGroups.Push(pwStorage.RootGroup);

			int nNodeCount = xmlRoot.ChildNodes.Count;
			for(int i = 0; i < nNodeCount; ++i)
			{
				XmlNode xmlChild = xmlRoot.ChildNodes[i];

				if(xmlChild.Name == ElemGroup)
					ReadGroup(xmlChild, vGroups, pwStorage);
				else { Debug.Assert(false); }

				if(slLogger != null)
					slLogger.SetProgress((uint)(((i + 1) * 100) / nNodeCount));
			}
		}

		private static PwIcon ReadIcon(XmlNode xmlChild, PwIcon pwDefault)
		{
			int nIcon;
			if(StrUtil.TryParseInt(XmlUtil.SafeInnerText(xmlChild), out nIcon))
			{
				if((nIcon >= 0) && (nIcon < (int)PwIcon.Count)) return (PwIcon)nIcon;
			}
			else { Debug.Assert(false); }

			return pwDefault;
		}

		private static void ReadGroup(XmlNode xmlNode, Stack<PwGroup> vGroups,
			PwDatabase pwStorage)
		{
			if(vGroups.Count == 0) { Debug.Assert(false); return; }
			PwGroup pgParent = vGroups.Peek();

			PwGroup pg = new PwGroup(true, true);
			pgParent.AddGroup(pg, true);
			vGroups.Push(pg);

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == ElemTitle)
					pg.Name = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == ElemIcon)
					pg.IconId = ReadIcon(xmlChild, pg.IconId);
				else if(xmlChild.Name == ElemGroup)
					ReadGroup(xmlChild, vGroups, pwStorage);
				else if(xmlChild.Name == ElemEntry)
					ReadEntry(xmlChild, pg, pwStorage);
				else { Debug.Assert(false); }
			}

			vGroups.Pop();
		}

		private static void ReadEntry(XmlNode xmlNode, PwGroup pgParent,
			PwDatabase pwStorage)
		{
			PwEntry pe = new PwEntry(true, true);
			pgParent.AddEntry(pe, true);

			string strAttachDesc = null, strAttachment = null;

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == ElemTitle)
					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectTitle,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemUserName)
					pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectUserName,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemUrl)
					pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectUrl,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemPassword)
					pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectPassword,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemNotes)
					pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectNotes,
						FilterSpecial(XmlUtil.SafeInnerXml(xmlChild))));
				else if(xmlChild.Name == ElemIcon)
					pe.IconId = ReadIcon(xmlChild, pe.IconId);
				else if(xmlChild.Name == ElemCreationTime)
					pe.CreationTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
				else if(xmlChild.Name == ElemLastModTime)
					pe.LastModificationTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
				else if(xmlChild.Name == ElemLastAccessTime)
					pe.LastAccessTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
				else if(xmlChild.Name == ElemExpiryTime)
				{
					string strDate = XmlUtil.SafeInnerText(xmlChild);
					pe.Expires = (strDate != ValueNever);
					if(pe.Expires) pe.ExpiryTime = ParseTime(strDate);
				}
				else if(xmlChild.Name == ElemAttachDesc)
					strAttachDesc = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == ElemAttachment)
					strAttachment = XmlUtil.SafeInnerText(xmlChild);
				else { Debug.Assert(false); }
			}

			if(!string.IsNullOrEmpty(strAttachDesc) && (strAttachment != null))
			{
				byte[] pbData = Convert.FromBase64String(strAttachment);
				ProtectedBinary pb = new ProtectedBinary(false, pbData);
				pe.Binaries.Set(strAttachDesc, pb);
			}
		}

		private static DateTime ParseTime(string str)
		{
			if(string.IsNullOrEmpty(str)) { Debug.Assert(false); return DateTime.Now; }
			if(str == "0000-00-00T00:00:00") return DateTime.Now;

			DateTime dt;
			if(DateTime.TryParse(str, out dt)) return dt;

			Debug.Assert(false);
			return DateTime.Now;
		}

		private static string FilterSpecial(string strData)
		{
			string str = strData;
			
			str = str.Replace(@"<br/>", MessageService.NewLine);
			str = str.Replace(@"<br />", MessageService.NewLine);

			str = StrUtil.XmlToString(str);
			return str;
		}
	}
}
