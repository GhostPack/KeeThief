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
	internal sealed class KeePassXml1x : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "KeePass XML (1.x)"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return PwDefs.ShortProductName; } }

		public override bool SupportsUuids { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Binary; }
		}

		private const string ElemRoot = "pwlist";
		private const string ElemEntry = "pwentry";
		private const string ElemGroup = "group";
		private const string ElemTitle = "title";
		private const string ElemUserName = "username";
		private const string ElemUrl = "url";
		private const string ElemPassword = "password";
		private const string ElemNotes = "notes";
		private const string ElemUuid = "uuid";
		private const string ElemImage = "image";
		private const string ElemCreationTime = "creationtime";
		private const string ElemLastModTime = "lastmodtime";
		private const string ElemLastAccessTime = "lastaccesstime";
		private const string ElemExpiryTime = "expiretime";
		private const string ElemAttachDesc = "attachdesc";
		private const string ElemAttachment = "attachment";

		private const string AttribGroupTree = "tree";
		private const string AttribExpires = "expires";

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(sInput);

			XmlNode xmlRoot = xmlDoc.DocumentElement;
			Debug.Assert(xmlRoot.Name == ElemRoot);

			int nNodeCount = xmlRoot.ChildNodes.Count;
			for(int i = 0; i < nNodeCount; ++i)
			{
				XmlNode xmlChild = xmlRoot.ChildNodes[i];

				if(xmlChild.Name == ElemEntry)
					ReadEntry(xmlChild, pwStorage);
				else { Debug.Assert(false); }

				if(slLogger != null)
					slLogger.SetProgress((uint)(((i + 1) * 100) / nNodeCount));
			}
		}

		private static void ReadEntry(XmlNode xmlNode, PwDatabase pwStorage)
		{
			PwEntry pe = new PwEntry(true, true);
			PwGroup pg = pwStorage.RootGroup;

			string strAttachDesc = null, strAttachment = null;

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == ElemGroup)
				{
					string strPreTree = null;
					try
					{
						XmlNode xmlTree = xmlChild.Attributes.GetNamedItem(AttribGroupTree);
						strPreTree = xmlTree.Value;
					}
					catch(Exception) { }

					string strLast = XmlUtil.SafeInnerText(xmlChild);
					string strGroup = ((!string.IsNullOrEmpty(strPreTree)) ?
						strPreTree + "\\" + strLast : strLast);

					pg = pwStorage.RootGroup.FindCreateSubTree(strGroup,
						new string[1]{ "\\" }, true);
				}
				else if(xmlChild.Name == ElemTitle)
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
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemUuid)
					pe.SetUuid(new PwUuid(MemUtil.HexStringToByteArray(
						XmlUtil.SafeInnerText(xmlChild))), false);
				else if(xmlChild.Name == ElemImage)
				{
					int nImage;
					if(int.TryParse(XmlUtil.SafeInnerText(xmlChild), out nImage))
					{
						if((nImage >= 0) && (nImage < (int)PwIcon.Count))
							pe.IconId = (PwIcon)nImage;
						else { Debug.Assert(false); }
					}
					else { Debug.Assert(false); }
				}
				else if(xmlChild.Name == ElemCreationTime)
					pe.CreationTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
				else if(xmlChild.Name == ElemLastModTime)
					pe.LastModificationTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
				else if(xmlChild.Name == ElemLastAccessTime)
					pe.LastAccessTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
				else if(xmlChild.Name == ElemExpiryTime)
				{
					try
					{
						XmlNode xmlExpires = xmlChild.Attributes.GetNamedItem(AttribExpires);
						if(StrUtil.StringToBool(xmlExpires.Value))
						{
							pe.Expires = true;
							pe.ExpiryTime = ParseTime(XmlUtil.SafeInnerText(xmlChild));
						}
						else { Debug.Assert(ParseTime(XmlUtil.SafeInnerText(xmlChild)).Year == 2999); }
					}
					catch(Exception) { Debug.Assert(false); }
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

			pg.AddEntry(pe, true);
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
	}
}
