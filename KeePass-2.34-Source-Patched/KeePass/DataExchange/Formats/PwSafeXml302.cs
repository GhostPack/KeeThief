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
using System.Xml;
using System.Diagnostics;
using System.Drawing;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 3.02-3.30+
	internal sealed class PwSafeXml302 : FileFormatProvider
	{
		private const string AttribLineBreak = "delimiter";

		private const string ElemEntry = "entry";
		private const string ElemGroup = "group";
		private const string ElemTitle = "title";
		private const string ElemUserName = "username";
		private const string ElemPassword = "password";
		private const string ElemURL = "url";
		private const string ElemNotes = "notes";
		private const string ElemEMail = "email";

		private const string ElemAutoType = "autotype";
		private const string ElemRunCommand = "runcommand";

		private const string ElemCreationTime = "ctime";
		private const string ElemLastAccessTime = "atime";
		private const string ElemExpireTime = "ltime";
		private const string ElemLastModTime = "pmtime";
		private const string ElemRecordModTime = "rmtime";
		private const string ElemCreationTimeX = "ctimex";
		private const string ElemLastAccessTimeX = "atimex";
		private const string ElemExpireTimeX = "xtimex"; // Yes, inconsistent
		private const string ElemLastModTimeX = "pmtimex";
		private const string ElemRecordModTimeX = "rmtimex";

		private const string ElemEntryHistory = "pwhistory";
		private const string ElemEntryHistoryContainer = "history_entries";
		private const string ElemEntryHistoryItem = "history_entry";
		private const string ElemEntryHistoryItemTime = "changed";
		private const string ElemEntryHistoryItemTimeX = "changedx";
		private const string ElemEntryHistoryItemPassword = "oldpassword";

		private const string ElemTimePartDate = "date";
		private const string ElemTimePartTime = "time";

		private const string XPathUseDefaultUser = "Preferences/UseDefaultUser";
		private const string XPathDefaultUser = "Preferences/DefaultUsername";

		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Password Safe XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_PwSafe; }
		}

		private sealed class DatePasswordPair
		{
			public DateTime Time = DateTime.Now;
			public string Password = string.Empty;
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			MemoryStream ms = new MemoryStream();
			MemUtil.CopyStream(sInput, ms);
			byte[] pbData = ms.ToArray();
			ms.Close();

			try
			{
				string strData = StrUtil.Utf8.GetString(pbData);
				if(strData.StartsWith("<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
					StrUtil.CaseIgnoreCmp) && (strData.IndexOf(
					"WhatSaved=\"Password Safe V3.29\"", StrUtil.CaseIgnoreCmp) >= 0))
				{
					// Fix broken XML exported by Password Safe 3.29;
					// this has been fixed in 3.30
					strData = strData.Replace("<DefaultUsername<![CDATA[",
						"<DefaultUsername><![CDATA[");
					strData = strData.Replace("<DefaultSymbols<![CDATA[",
						"<DefaultSymbols><![CDATA[");

					pbData = StrUtil.Utf8.GetBytes(strData);
				}
			}
			catch(Exception) { Debug.Assert(false); }

			ms = new MemoryStream(pbData, false);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(ms);

			XmlNode xmlRoot = xmlDoc.DocumentElement;

			string strLineBreak = "\n";
			try
			{
				XmlAttributeCollection xac = xmlRoot.Attributes;
				XmlNode xmlBreak = xac.GetNamedItem(AttribLineBreak);
				string strBreak = xmlBreak.Value;

				if(!string.IsNullOrEmpty(strBreak))
					strLineBreak = strBreak;
				else { Debug.Assert(false); }
			}
			catch(Exception) { Debug.Assert(false); }

			foreach(XmlNode xmlChild in xmlRoot.ChildNodes)
			{
				if(xmlChild.Name == ElemEntry)
					ImportEntry(xmlChild, pwStorage, strLineBreak);
			}

			XmlNode xnUse = xmlRoot.SelectSingleNode(XPathUseDefaultUser);
			if(xnUse != null)
			{
				string strUse = XmlUtil.SafeInnerText(xnUse);
				if(StrUtil.StringToBool(strUse))
				{
					XmlNode xn = xmlRoot.SelectSingleNode(XPathDefaultUser);
					if((xn != null) && (pwStorage.DefaultUserName.Length == 0))
					{
						pwStorage.DefaultUserName = XmlUtil.SafeInnerText(xn);
						if(pwStorage.DefaultUserName.Length > 0)
							pwStorage.DefaultUserNameChanged = DateTime.Now;
					}
				}
			}

			ms.Close();
		}

		private static void ImportEntry(XmlNode xmlNode, PwDatabase pwStorage,
			string strLineBreak)
		{
			Debug.Assert(xmlNode != null); if(xmlNode == null) return;

			PwEntry pe = new PwEntry(true, true);
			string strGroupName = string.Empty;

			List<DatePasswordPair> listHistory = null;

			foreach(XmlNode xmlChild in xmlNode.ChildNodes)
			{
				if(xmlChild.Name == ElemGroup)
					strGroupName = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == ElemTitle)
					pe.Strings.Set(PwDefs.TitleField,
						new ProtectedString(pwStorage.MemoryProtection.ProtectTitle,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemUserName)
					pe.Strings.Set(PwDefs.UserNameField,
						new ProtectedString(pwStorage.MemoryProtection.ProtectUserName,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemPassword)
					pe.Strings.Set(PwDefs.PasswordField,
						new ProtectedString(pwStorage.MemoryProtection.ProtectPassword,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemURL)
					pe.Strings.Set(PwDefs.UrlField,
						new ProtectedString(pwStorage.MemoryProtection.ProtectUrl,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemNotes)
					pe.Strings.Set(PwDefs.NotesField,
						new ProtectedString(pwStorage.MemoryProtection.ProtectNotes,
						XmlUtil.SafeInnerText(xmlChild, strLineBreak)));
				else if(xmlChild.Name == ElemEMail)
					pe.Strings.Set("E-Mail", new ProtectedString(false,
						XmlUtil.SafeInnerText(xmlChild)));
				else if(xmlChild.Name == ElemCreationTime)
					pe.CreationTime = ReadDateTime(xmlChild);
				else if(xmlChild.Name == ElemLastAccessTime)
					pe.LastAccessTime = ReadDateTime(xmlChild);
				else if(xmlChild.Name == ElemExpireTime)
				{
					pe.ExpiryTime = ReadDateTime(xmlChild);
					pe.Expires = true;
				}
				else if(xmlChild.Name == ElemLastModTime) // = last mod
					pe.LastModificationTime = ReadDateTime(xmlChild);
				else if(xmlChild.Name == ElemRecordModTime) // = last mod
					pe.LastModificationTime = ReadDateTime(xmlChild);
				else if(xmlChild.Name == ElemCreationTimeX)
					pe.CreationTime = ReadDateTimeX(xmlChild);
				else if(xmlChild.Name == ElemLastAccessTimeX)
					pe.LastAccessTime = ReadDateTimeX(xmlChild);
				else if(xmlChild.Name == ElemExpireTimeX)
				{
					pe.ExpiryTime = ReadDateTimeX(xmlChild);
					pe.Expires = true;
				}
				else if(xmlChild.Name == ElemLastModTimeX) // = last mod
					pe.LastModificationTime = ReadDateTimeX(xmlChild);
				else if(xmlChild.Name == ElemRecordModTimeX) // = last mod
					pe.LastModificationTime = ReadDateTimeX(xmlChild);
				else if(xmlChild.Name == ElemAutoType)
					pe.AutoType.DefaultSequence = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == ElemRunCommand)
					pe.OverrideUrl = XmlUtil.SafeInnerText(xmlChild);
				else if(xmlChild.Name == ElemEntryHistory)
					listHistory = ReadEntryHistory(xmlChild);
			}

			if(listHistory != null)
			{
				string strPassword = pe.Strings.ReadSafe(PwDefs.PasswordField);
				DateTime dtLastMod = pe.LastModificationTime;

				foreach(DatePasswordPair dpp in listHistory)
				{
					pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectPassword,
						dpp.Password));
					pe.LastModificationTime = dpp.Time;

					pe.CreateBackup(null);
				}
				// Maintain backups manually now (backups from the imported file
				// might have been out of order)
				pe.MaintainBackups(pwStorage);

				pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectPassword,
					strPassword));
				pe.LastModificationTime = dtLastMod;
			}

			PwGroup pgContainer = pwStorage.RootGroup;
			if(strGroupName.Length != 0)
				pgContainer = pwStorage.RootGroup.FindCreateSubTree(strGroupName,
					new string[1]{ "." }, true);
			pgContainer.AddEntry(pe, true);
			pgContainer.IsExpanded = true;
		}

		private static DateTime ReadDateTime(XmlNode xmlNode)
		{
			Debug.Assert(xmlNode != null); if(xmlNode == null) return DateTime.Now;

			int[] vTimeParts = new int[6];
			DateTime dtTemp;
			foreach(XmlNode xmlChild in xmlNode.ChildNodes)
			{
				if(xmlChild.Name == ElemTimePartDate)
				{
					if(DateTime.TryParse(XmlUtil.SafeInnerText(xmlChild), out dtTemp))
					{
						vTimeParts[0] = dtTemp.Year;
						vTimeParts[1] = dtTemp.Month;
						vTimeParts[2] = dtTemp.Day;
					}
				}
				else if(xmlChild.Name == ElemTimePartTime)
				{
					if(DateTime.TryParse(XmlUtil.SafeInnerText(xmlChild), out dtTemp))
					{
						vTimeParts[3] = dtTemp.Hour;
						vTimeParts[4] = dtTemp.Minute;
						vTimeParts[5] = dtTemp.Second;
					}
				}
				else { Debug.Assert(false); }
			}

			return new DateTime(vTimeParts[0], vTimeParts[1], vTimeParts[2],
				vTimeParts[3], vTimeParts[4], vTimeParts[5]);
		}

		private static DateTime ReadDateTimeX(XmlNode xmlNode)
		{
			string strDate = XmlUtil.SafeInnerText(xmlNode);
			DateTime dt;
			if(StrUtil.TryParseDateTime(strDate, out dt)) return dt;
			Debug.Assert(false);
			return DateTime.Now;
		}

		private static List<DatePasswordPair> ReadEntryHistory(XmlNode xmlNode)
		{
			List<DatePasswordPair> list = null;

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == ElemEntryHistoryContainer)
					list = ReadEntryHistoryContainer(xmlChild);
			}

			return list;
		}

		private static List<DatePasswordPair> ReadEntryHistoryContainer(XmlNode xmlNode)
		{
			List<DatePasswordPair> list = new List<DatePasswordPair>();

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == ElemEntryHistoryItem)
					list.Add(ReadEntryHistoryItem(xmlChild));
			}

			return list;
		}

		private static DatePasswordPair ReadEntryHistoryItem(XmlNode xmlNode)
		{
			DatePasswordPair dpp = new DatePasswordPair();

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == ElemEntryHistoryItemTime)
					dpp.Time = ReadDateTime(xmlChild);
				else if(xmlChild.Name == ElemEntryHistoryItemTimeX)
					dpp.Time = ReadDateTimeX(xmlChild);
				else if(xmlChild.Name == ElemEntryHistoryItemPassword)
					dpp.Password = XmlUtil.SafeInnerText(xmlChild);
			}

			return dpp;
		}
	}
}
