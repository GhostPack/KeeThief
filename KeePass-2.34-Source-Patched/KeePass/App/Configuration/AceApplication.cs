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
using System.Diagnostics;

using KeePass.Ecas;

using KeePassLib.Serialization;

namespace KeePass.App.Configuration
{
	public sealed class AceApplication
	{
		public AceApplication()
		{
		}

		private string m_strLanguageFile = string.Empty; // = English
		[DefaultValue("")]
		public string LanguageFile
		{
			get { return m_strLanguageFile; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strLanguageFile = value;
			}
		}

		private bool m_bHelpUseLocal = false;
		[DefaultValue(false)]
		public bool HelpUseLocal
		{
			get { return m_bHelpUseLocal; }
			set { m_bHelpUseLocal = value; }
		}

		// Serialize DateTime with TimeUtil
		private string m_strLastUpdChk = string.Empty;
		[DefaultValue("")]
		public string LastUpdateCheck
		{
			get { return m_strLastUpdChk; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strLastUpdChk = value;
			}
		}

		private IOConnectionInfo m_ioLastDb = null;
		public IOConnectionInfo LastUsedFile
		{
			get
			{
				if(m_ioLastDb == null) m_ioLastDb = new IOConnectionInfo();
				return m_ioLastDb;
			}
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_ioLastDb = value;
			}
		}

		private AceMru m_mru = null;
		public AceMru MostRecentlyUsed
		{
			get
			{
				if(m_mru == null) m_mru = new AceMru();
				return m_mru;
			}
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_mru = value;
			}
		}

		private bool m_bRememberWorkDirs = true;
		[DefaultValue(true)]
		public bool RememberWorkingDirectories
		{
			get { return m_bRememberWorkDirs; }
			set { m_bRememberWorkDirs = value; }
		}

		private Dictionary<string, string> m_dictWorkingDirs =
			new Dictionary<string, string>();

		/// <summary>
		/// For serialization only; use the <c>*WorkingDirectory</c>
		/// methods instead.
		/// </summary>
		[XmlArray("WorkingDirectories")]
		[XmlArrayItem("Item")]
		public string[] WorkingDirectoriesSerialized
		{
			get { return SerializeWorkingDirectories(); }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				DeserializeWorkingDirectories(value);
			}
		}

		private AceStartUp m_su = null;
		public AceStartUp Start
		{
			get
			{
				if(m_su == null) m_su = new AceStartUp();
				return m_su;
			}
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_su = value;
			}
		}

		private AceOpenDb m_fo = null;
		public AceOpenDb FileOpening
		{
			get
			{
				if(m_fo == null) m_fo = new AceOpenDb();
				return m_fo;
			}
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_fo = value;
			}
		}

		private bool m_bVerifyFile = true;
		[DefaultValue(true)]
		public bool VerifyWrittenFileAfterSaving
		{
			get { return m_bVerifyFile; }
			set { m_bVerifyFile = value; }
		}

		private bool m_bTransactedWrites = true;
		[DefaultValue(true)]
		public bool UseTransactedFileWrites
		{
			get { return m_bTransactedWrites; }
			set { m_bTransactedWrites = value; }
		}

		private bool m_bFileTxExtra = false;
		[DefaultValue(false)]
		public bool FileTxExtra
		{
			get { return m_bFileTxExtra; }
			set { m_bFileTxExtra = value; }
		}

		private bool m_bFileLocks = false;
		[DefaultValue(false)]
		public bool UseFileLocks
		{
			get { return m_bFileLocks; }
			set { m_bFileLocks = value; }
		}

		private bool m_bSaveForceSync = false;
		[DefaultValue(false)]
		public bool SaveForceSync
		{
			get { return m_bSaveForceSync; }
			set { m_bSaveForceSync = value; }
		}

		private AceCloseDb m_fc = new AceCloseDb();
		public AceCloseDb FileClosing
		{
			get { return m_fc; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_fc = value;
			}
		}

		private EcasTriggerSystem m_triggers = new EcasTriggerSystem();
		public EcasTriggerSystem TriggerSystem
		{
			get { return m_triggers; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_triggers = value;
			}
		}

		private string m_strPluginCachePath = string.Empty;
		[DefaultValue("")]
		public string PluginCachePath
		{
			get { return m_strPluginCachePath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strPluginCachePath = value;
			}
		}

		public string GetWorkingDirectory(string strContext)
		{
			// strContext may be null

			if(!m_bRememberWorkDirs) return null;

			string str;
			m_dictWorkingDirs.TryGetValue(strContext ?? string.Empty, out str);
			return str;
		}

		public void SetWorkingDirectory(string strContext, string strDir)
		{
			// Both parameters may be null

			// if(!m_bRememberWorkDirs) return;

			if(string.IsNullOrEmpty(strContext)) return;
			m_dictWorkingDirs[strContext] = (strDir ?? string.Empty);
		}

		internal List<string> GetWorkingDirectoryContexts()
		{
			if(!m_bRememberWorkDirs) return new List<string>();

			return new List<string>(m_dictWorkingDirs.Keys);
		}

		private string[] SerializeWorkingDirectories()
		{
			if(!m_bRememberWorkDirs) return new string[0];

			List<string> l = new List<string>();
			foreach(KeyValuePair<string, string> kvp in m_dictWorkingDirs)
				l.Add(kvp.Key + @"@" + kvp.Value);
			return l.ToArray();
		}

		private void DeserializeWorkingDirectories(string[] v)
		{
			// Do not check m_bRememberWorkDirs, because it might not
			// have been deserialized yet

			m_dictWorkingDirs.Clear();

			foreach(string str in v)
			{
				if(str == null) { Debug.Assert(false); continue; }

				int iSep = str.IndexOf('@');
				if(iSep <= 0) { Debug.Assert(false); continue; }

				m_dictWorkingDirs[str.Substring(0, iSep)] = str.Substring(iSep + 1);
			}
		}
	}

	public sealed class AceStartUp
	{
		public AceStartUp()
		{
		}

		private bool m_bOpenLastDb = true;
		[DefaultValue(true)]
		public bool OpenLastFile
		{
			get { return m_bOpenLastDb; }
			set { m_bOpenLastDb = value; }
		}

		private bool m_bCheckForUpdate = false;
		// [DefaultValue(false)] // Avoid user confusion with 'Configured' setting
		public bool CheckForUpdate
		{
			get { return m_bCheckForUpdate; }
			set { m_bCheckForUpdate = value; }
		}

		private bool m_bCheckForUpdateCfg = false;
		[DefaultValue(false)]
		public bool CheckForUpdateConfigured
		{
			get { return m_bCheckForUpdateCfg; }
			set { m_bCheckForUpdateCfg = value; }
		}

		private bool m_bMinimizedAndLocked = false;
		[DefaultValue(false)]
		public bool MinimizedAndLocked
		{
			get { return m_bMinimizedAndLocked; }
			set { m_bMinimizedAndLocked = value; }
		}

		private bool m_bPlgDeleteOld = true;
		[DefaultValue(true)]
		public bool PluginCacheDeleteOld
		{
			get { return m_bPlgDeleteOld; }
			set { m_bPlgDeleteOld = value; }
		}

		private bool m_bClearPlgCache = false;
		[DefaultValue(false)]
		public bool PluginCacheClearOnce
		{
			get { return m_bClearPlgCache; }
			set { m_bClearPlgCache = value; }
		}
	}

	public sealed class AceOpenDb
	{
		public AceOpenDb()
		{
		}

		private bool m_bShowExpiredEntries = false;
		[DefaultValue(false)]
		public bool ShowExpiredEntries
		{
			get { return m_bShowExpiredEntries; }
			set { m_bShowExpiredEntries = value; }
		}

		private bool m_bShowSoonToExpireEntries = false;
		[DefaultValue(false)]
		public bool ShowSoonToExpireEntries
		{
			get { return m_bShowSoonToExpireEntries; }
			set { m_bShowSoonToExpireEntries = value; }
		}
	}

	public sealed class AceCloseDb
	{
		public AceCloseDb()
		{
		}

		private bool m_bAutoSave = false;
		[DefaultValue(false)]
		public bool AutoSave
		{
			get { return m_bAutoSave; }
			set { m_bAutoSave = value; }
		}
	}

	public sealed class AceMru
	{
		public const uint DefaultMaxItemCount = 12;

		public AceMru()
		{
		}

		private uint m_uMaxItems = DefaultMaxItemCount;
		public uint MaxItemCount
		{
			get { return m_uMaxItems; }
			set { m_uMaxItems = value; }
		}

		private List<IOConnectionInfo> m_vItems = new List<IOConnectionInfo>();
		[XmlArrayItem("ConnectionInfo")]
		public List<IOConnectionInfo> Items
		{
			get { return m_vItems; }
			set { m_vItems = value; }
		}
	}
}
