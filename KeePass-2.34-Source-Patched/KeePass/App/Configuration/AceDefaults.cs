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
using System.Xml.Serialization;
using System.ComponentModel;

using KeePass.Util;

using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.App.Configuration
{
	public sealed class AceKeyAssoc
	{
		private string m_strDb = string.Empty;
		public string DatabasePath
		{
			get { return m_strDb; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strDb = value;
			}
		}

		private string m_strKey = string.Empty;
		[DefaultValue("")]
		public string KeyFilePath
		{
			get { return m_strKey; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strKey = value;
			}
		}

		private string m_strProv = string.Empty;
		[DefaultValue("")]
		public string KeyProvider
		{
			get { return m_strProv; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strProv = value;
			}
		}

		private bool m_bUserAcc = false;
		[DefaultValue(false)]
		public bool UserAccount
		{
			get { return m_bUserAcc; }
			set { m_bUserAcc = value; }
		}

		public AceKeyAssoc() { }
	}

	public sealed class AceDefaults
	{
		public AceDefaults()
		{
		}

		private int m_nNewEntryExpireDays = -1;
		[DefaultValue(-1)]
		public int NewEntryExpiresInDays
		{
			get { return m_nNewEntryExpireDays; }
			set { m_nNewEntryExpireDays = value; }
		}

		private uint m_uDefaultOptionsTab = 0;
		public uint OptionsTabIndex
		{
			get { return m_uDefaultOptionsTab; }
			set { m_uDefaultOptionsTab = value; }
		}

		private const string DefaultTanChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
		private string m_strTanChars = DefaultTanChars;
		[DefaultValue(DefaultTanChars)]
		public string TanCharacters
		{
			get { return m_strTanChars; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strTanChars = value;
			}
		}

		private bool m_bExpireTansOnUse = true;
		[DefaultValue(true)]
		public bool TanExpiresOnUse
		{
			get { return m_bExpireTansOnUse; }
			set { m_bExpireTansOnUse = value; }
		}

		private SearchParameters m_searchParams = new SearchParameters();
		public SearchParameters SearchParameters
		{
			get { return m_searchParams; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_searchParams = value;
			}
		}

		private string m_strDbSaveAsPath = string.Empty;
		[DefaultValue("")]
		public string FileSaveAsDirectory
		{
			get { return m_strDbSaveAsPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strDbSaveAsPath = value;
			}
		}

		private bool m_bRememberKeySources = true;
		[DefaultValue(true)]
		public bool RememberKeySources
		{
			get { return m_bRememberKeySources; }
			set { m_bRememberKeySources = value; }
		}

		private List<AceKeyAssoc> m_vKeySources = new List<AceKeyAssoc>();
		[XmlArrayItem("Association")]
		public List<AceKeyAssoc> KeySources
		{
			get { return m_vKeySources; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vKeySources = value;
			}
		}

		private string m_strCustomColors = string.Empty;
		[DefaultValue("")]
		public string CustomColors
		{
			get { return m_strCustomColors; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strCustomColors = value;
			}
		}

		private string m_strWinFavsBaseName = string.Empty;
		[DefaultValue("")]
		public string WinFavsBaseFolderName
		{
			get { return m_strWinFavsBaseName; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strWinFavsBaseName = value;
			}
		}

		private string m_strWinFavsFilePrefix = string.Empty;
		[DefaultValue("")]
		public string WinFavsFileNamePrefix
		{
			get { return m_strWinFavsFilePrefix; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strWinFavsFilePrefix = value;
			}
		}

		private string m_strWinFavsFileSuffix = string.Empty;
		[DefaultValue("")]
		public string WinFavsFileNameSuffix
		{
			get { return m_strWinFavsFileSuffix; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strWinFavsFileSuffix = value;
			}
		}

		private bool m_bCollapseRecycleBin = false;
		[DefaultValue(false)]
		public bool RecycleBinCollapse
		{
			get { return m_bCollapseRecycleBin; }
			set { m_bCollapseRecycleBin = value; }
		}

		private static string GetKeyAssocID(IOConnectionInfo iocDb)
		{
			if(iocDb == null) throw new ArgumentNullException("iocDb");

			string strDb = iocDb.Path;
			if((strDb.Length > 0) && iocDb.IsLocalFile() &&
				!UrlUtil.IsAbsolutePath(strDb))
				strDb = UrlUtil.MakeAbsolutePath(WinUtil.GetExecutable(), strDb);

			return strDb;
		}

		private int GetKeyAssocIndex(string strID)
		{
			for(int i = 0; i < m_vKeySources.Count; ++i)
			{
				if(strID.Equals(m_vKeySources[i].DatabasePath, StrUtil.CaseIgnoreCmp))
					return i;
			}

			return -1;
		}

		public void SetKeySources(IOConnectionInfo iocDb, CompositeKey cmpKey)
		{
			string strID = GetKeyAssocID(iocDb);
			int idx = GetKeyAssocIndex(strID);

			if((cmpKey == null) || !m_bRememberKeySources)
			{
				if(idx >= 0) m_vKeySources.RemoveAt(idx);
				return;
			}

			AceKeyAssoc a = new AceKeyAssoc();
			a.DatabasePath = strID;

			IUserKey kcpFile = cmpKey.GetUserKey(typeof(KcpKeyFile));
			if(kcpFile != null)
			{
				string strKeyFile = ((KcpKeyFile)kcpFile).Path;
				if(!string.IsNullOrEmpty(strKeyFile) && !StrUtil.IsDataUri(strKeyFile))
				{
					if(!UrlUtil.IsAbsolutePath(strKeyFile))
						strKeyFile = UrlUtil.MakeAbsolutePath(WinUtil.GetExecutable(),
							strKeyFile);

					a.KeyFilePath = strKeyFile;
				}
			}

			IUserKey kcpCustom = cmpKey.GetUserKey(typeof(KcpCustomKey));
			if(kcpCustom != null)
				a.KeyProvider = ((KcpCustomKey)kcpCustom).Name;

			IUserKey kcpUser = cmpKey.GetUserKey(typeof(KcpUserAccount));
			a.UserAccount = (kcpUser != null);

			bool bAtLeastOne = ((a.KeyFilePath.Length > 0) ||
				(a.KeyProvider.Length > 0) || a.UserAccount);
			if(bAtLeastOne)
			{
				if(idx >= 0) m_vKeySources[idx] = a;
				else m_vKeySources.Add(a);
			}
			else if(idx >= 0) m_vKeySources.RemoveAt(idx);
		}

		public AceKeyAssoc GetKeySources(IOConnectionInfo iocDb)
		{
			string strID = GetKeyAssocID(iocDb);
			int idx = GetKeyAssocIndex(strID);

			if(!m_bRememberKeySources) return null;

			if(idx >= 0) return m_vKeySources[idx];
			return null;
		}
	}
}
