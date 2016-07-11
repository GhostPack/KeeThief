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
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Drawing;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// KasperskyPwMgrXml50 derives from this

	// 5.0.4.232-8.0.7.78+
	internal class StickyPwXml50 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Sticky Password XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_StickyPw; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			XPathDocument xpDoc = new XPathDocument(sInput);
			XPathNavigator xpNav = xpDoc.CreateNavigator();

			ImportLogins(xpNav, pwStorage);
			ImportMemos(xpNav, pwStorage);
		}

		private static void ImportLogins(XPathNavigator xpNav, PwDatabase pd)
		{
			XPathNodeIterator it = xpNav.Select("/root/Database/Logins/Login");
			while(it.MoveNext())
			{
				PwEntry pe = new PwEntry(true, true);
				pd.RootGroup.AddEntry(pe, true);

				XPathNavigator xpLogin = it.Current;
				pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
					pd.MemoryProtection.ProtectUserName,
					xpLogin.GetAttribute("Name", string.Empty)));
				pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
					pd.MemoryProtection.ProtectPassword,
					xpLogin.GetAttribute("Password", string.Empty)));

				SetTimes(pe, xpLogin);

				string strID = xpLogin.GetAttribute("ID", string.Empty);
				if(string.IsNullOrEmpty(strID)) continue;

				XPathNavigator xpAccLogin = xpNav.SelectSingleNode(
					@"/root/Database/Accounts/Account/LoginLinks/Login[@SourceLoginID='" +
					strID + @"']/../..");
				if(xpAccLogin == null) { Debug.Assert(false); }
				else
				{
					Debug.Assert(xpAccLogin.Name == "Account");

					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pd.MemoryProtection.ProtectTitle,
						xpAccLogin.GetAttribute("Name", string.Empty)));
					pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
						pd.MemoryProtection.ProtectUrl,
						xpAccLogin.GetAttribute("Link", string.Empty)));

					string strNotes = xpAccLogin.GetAttribute("Comments", string.Empty);
					strNotes = strNotes.Replace("/n", Environment.NewLine);
					pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
						pd.MemoryProtection.ProtectNotes, strNotes));
				}
			}
		}

		private static void ImportMemos(XPathNavigator xpNav, PwDatabase pd)
		{
			XPathNodeIterator it = xpNav.Select("/root/Database/SecureMemos/SecureMemo");
			while(it.MoveNext())
			{
				PwEntry pe = new PwEntry(true, true);
				pd.RootGroup.AddEntry(pe, true);

				pe.IconId = PwIcon.PaperNew;

				XPathNavigator xpMemo = it.Current;

				pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
					pd.MemoryProtection.ProtectTitle,
					xpMemo.GetAttribute("Name", string.Empty)));

				SetTimes(pe, xpMemo);

				try
				{
					string strMemoHex = xpMemo.Value;
					byte[] pbMemo = MemUtil.HexStringToByteArray(strMemoHex);
					string strMemoRtf = Encoding.Unicode.GetString(pbMemo);

					pe.Binaries.Set(KPRes.Notes + ".rtf", new ProtectedBinary(
						false, StrUtil.Utf8.GetBytes(strMemoRtf)));
				}
				catch(Exception) { Debug.Assert(false); }
			}
		}

		private static void SetTimes(PwEntry pe, XPathNavigator xpNode)
		{
			DateTime dt;
			string strTime = (xpNode.GetAttribute("CreatedDate", string.Empty));
			if(DateTime.TryParse(strTime, out dt)) pe.CreationTime = dt;
			else { Debug.Assert(false); }

			strTime = (xpNode.GetAttribute("ModifiedDate", string.Empty));
			if(DateTime.TryParse(strTime, out dt)) pe.LastModificationTime = dt;
			else { Debug.Assert(false); }
		}
	}
}
