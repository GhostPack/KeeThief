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
using System.IO;
using System.Drawing;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;

namespace KeePass.DataExchange.Formats
{
	// 2.4.1.2-3.0.7.2022+
	internal sealed class SafeWalletXml3 : FileFormatProvider
	{
		private static readonly string[] ElemsVault = new string[] {
			"T37" // 3.0.5
		};
		private static readonly string[] ElemsGroup = new string[] {
			"Folder", // 2.4.1.2
			"T3", // 3.0.5
			"T21" // 3.0.7, special group for web entries
		};
		private static readonly string[] ElemsEntry = new string[] {
			"Card", // 3.0.4
			"T4" // 3.0.5
		};
		private static readonly string[] ElemsProps = new string[] {
			"Property", // 3.0.4
			"T257", "T258", "T259", "T263", "T264", "T265", // 3.0.5
			"T266", "T267" // 3.0.5
		};
		private static readonly string[] ElemsWebEntry = new string[] {
			"T22" // 3.0.7
		};

		private const string AttribCaption = "Caption";

		private const string AttribWebUrl = "URL";
		private const string AttribWebUserName = "Username";
		private const string AttribWebPassword = "Password";

		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "SafeWallet XML"; } }
		public override string DefaultExtension { get { return "xml"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_SafeWallet; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Unicode);
			string strDoc = sr.ReadToEnd();
			sr.Close();

			XmlDocument xd = new XmlDocument();
			xd.LoadXml(strDoc);

			XmlNode xnRoot = xd.DocumentElement;
			Debug.Assert(xnRoot.Name == "SafeWallet");
			foreach(XmlNode xn in xnRoot.ChildNodes)
			{
				if(Array.IndexOf<string>(ElemsGroup, xn.Name) >= 0)
					AddGroup(xn, pwStorage.RootGroup, pwStorage); // 2.4.1.2
				else if(Array.IndexOf<string>(ElemsEntry, xn.Name) >= 0)
					AddEntry(xn, pwStorage.RootGroup, pwStorage); // 3.0.4
				else if(Array.IndexOf<string>(ElemsVault, xn.Name) >= 0)
					ImportVault(xn, pwStorage); // 3.0.5
			}
		}

		private static void AddEntry(XmlNode xnEntry, PwGroup pg, PwDatabase pd)
		{
			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			XmlNode xnTitle = xnEntry.Attributes.GetNamedItem(AttribCaption);
			string strTitle = ((xnTitle != null) ? xnTitle.Value : string.Empty);
			ImportUtil.AppendToField(pe, PwDefs.TitleField, strTitle ?? string.Empty, pd);

			foreach(XmlNode xn in xnEntry.ChildNodes)
			{
				if(Array.IndexOf<string>(ElemsProps, xn.Name) >= 0)
				{
					XmlNode xnField = xn.Attributes.GetNamedItem(AttribCaption);
					string strField = ((xnField != null) ? xnField.Value : null);
					if(string.IsNullOrEmpty(strField)) { Debug.Assert(false); }
					else
					{
						string strMap = ImportUtil.MapNameToStandardField(strField, false);
						if(string.IsNullOrEmpty(strMap)) strMap = strField;

						ImportUtil.AppendToField(pe, strMap,
							XmlUtil.SafeInnerText(xn), pd);
					}
				}
				else { Debug.Assert(false); } // Unknown node
			}
		}

		private static void AddWebEntry(XmlNode xnEntry, PwGroup pg, PwDatabase pd)
		{
			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			XmlNode xn = xnEntry.Attributes.GetNamedItem(AttribCaption);
			string str = ((xn != null) ? xn.Value : string.Empty);
			ImportUtil.AppendToField(pe, PwDefs.TitleField, str ?? string.Empty, pd);

			xn = xnEntry.Attributes.GetNamedItem(AttribWebUrl);
			str = ((xn != null) ? xn.Value : string.Empty);
			ImportUtil.AppendToField(pe, PwDefs.UrlField, str ?? string.Empty, pd);

			xn = xnEntry.Attributes.GetNamedItem(AttribWebUserName);
			str = ((xn != null) ? xn.Value : string.Empty);
			ImportUtil.AppendToField(pe, PwDefs.UserNameField, str ?? string.Empty, pd);

			xn = xnEntry.Attributes.GetNamedItem(AttribWebPassword);
			str = ((xn != null) ? xn.Value : string.Empty);
			ImportUtil.AppendToField(pe, PwDefs.PasswordField, str ?? string.Empty, pd);
		}

		private static void ImportVault(XmlNode xnVault, PwDatabase pd)
		{
			foreach(XmlNode xn in xnVault.ChildNodes)
			{
				if(Array.IndexOf<string>(ElemsGroup, xn.Name) >= 0)
					AddGroup(xn, pd.RootGroup, pd);
				else if(Array.IndexOf<string>(ElemsEntry, xn.Name) >= 0)
					AddEntry(xn, pd.RootGroup, pd);
				else { Debug.Assert(false); } // Unknown node
			}
		}

		private static void AddGroup(XmlNode xnGrp, PwGroup pgParent, PwDatabase pd)
		{
			XmlNode xnName = xnGrp.Attributes.GetNamedItem(AttribCaption);
			string strName = ((xnName != null) ? xnName.Value : null);
			if(string.IsNullOrEmpty(strName)) { Debug.Assert(false); strName = KPRes.Group; }

			PwGroup pg = new PwGroup(true, true);
			pg.Name = strName;

			pgParent.AddGroup(pg, true);

			foreach(XmlNode xn in xnGrp)
			{
				if(Array.IndexOf<string>(ElemsGroup, xn.Name) >= 0)
					AddGroup(xn, pg, pd);
				else if(Array.IndexOf<string>(ElemsEntry, xn.Name) >= 0)
					AddEntry(xn, pg, pd);
				else if(Array.IndexOf<string>(ElemsWebEntry, xn.Name) >= 0)
					AddWebEntry(xn, pg, pd);
				else { Debug.Assert(false); } // Unknown node
			}
		}
	}
}
