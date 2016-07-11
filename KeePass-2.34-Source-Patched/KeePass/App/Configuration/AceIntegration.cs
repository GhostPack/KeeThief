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
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Diagnostics;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Utility;

namespace KeePass.App.Configuration
{
	public sealed class AceIntegration
	{
		private ulong m_hkAutoType = (ulong)(Keys.Control | Keys.Alt | Keys.A);
		public ulong HotKeyGlobalAutoType
		{
			get { return m_hkAutoType; }
			set { m_hkAutoType = value; }
		}

		private ulong m_hkAutoTypeSel = (ulong)Keys.None;
		public ulong HotKeySelectedAutoType
		{
			get { return m_hkAutoTypeSel; }
			set { m_hkAutoTypeSel = value; }
		}

		private ulong m_hkShowWindow = (ulong)(Keys.Control | Keys.Alt | Keys.K);
		public ulong HotKeyShowWindow
		{
			get { return m_hkShowWindow; }
			set { m_hkShowWindow = value; }
		}

		private ulong m_hkEntryMenu = (ulong)Keys.None;
		public ulong HotKeyEntryMenu
		{
			get { return m_hkEntryMenu; }
			set { m_hkEntryMenu = value; }
		}

		private string m_strUrlOverride = string.Empty;
		[DefaultValue("")]
		public string UrlOverride
		{
			get { return m_strUrlOverride; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strUrlOverride = value;
			}
		}

		private AceUrlSchemeOverrides m_vSchemeOverrides = new AceUrlSchemeOverrides();
		public AceUrlSchemeOverrides UrlSchemeOverrides
		{
			get { return m_vSchemeOverrides; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vSchemeOverrides = value;
			}
		}

		private bool m_bSearchKeyFiles = true;
		[DefaultValue(true)]
		public bool SearchKeyFiles
		{
			get { return m_bSearchKeyFiles; }
			set { m_bSearchKeyFiles = value; }
		}

		private bool m_bSearchKeyFilesOnRemovable = false;
		[DefaultValue(false)]
		public bool SearchKeyFilesOnRemovableMedia
		{
			get { return m_bSearchKeyFilesOnRemovable; }
			set { m_bSearchKeyFilesOnRemovable = value; }
		}

		private bool m_bSingleInstance = true;
		[DefaultValue(true)]
		public bool LimitToSingleInstance
		{
			get { return m_bSingleInstance; }
			set { m_bSingleInstance = value; }
		}

		private bool m_bMatchByTitle = true;
		[DefaultValue(true)]
		public bool AutoTypeMatchByTitle
		{
			get { return m_bMatchByTitle; }
			set { m_bMatchByTitle = value; }
		}

		private bool m_bMatchByUrlInTitle = false;
		[DefaultValue(false)]
		public bool AutoTypeMatchByUrlInTitle
		{
			get { return m_bMatchByUrlInTitle; }
			set { m_bMatchByUrlInTitle = value; }
		}

		private bool m_bMatchByUrlHostInTitle = false;
		[DefaultValue(false)]
		public bool AutoTypeMatchByUrlHostInTitle
		{
			get { return m_bMatchByUrlHostInTitle; }
			set { m_bMatchByUrlHostInTitle = value; }
		}

		private bool m_bMatchByTagInTitle = false;
		[DefaultValue(false)]
		public bool AutoTypeMatchByTagInTitle
		{
			get { return m_bMatchByTagInTitle; }
			set { m_bMatchByTagInTitle = value; }
		}

		private bool m_bExpiredCanMatch = false;
		[DefaultValue(false)]
		public bool AutoTypeExpiredCanMatch
		{
			get { return m_bExpiredCanMatch; }
			set { m_bExpiredCanMatch = value; }
		}

		private bool m_bAutoTypeAlwaysShowSelDlg = false;
		[DefaultValue(false)]
		public bool AutoTypeAlwaysShowSelDialog
		{
			get { return m_bAutoTypeAlwaysShowSelDlg; }
			set { m_bAutoTypeAlwaysShowSelDlg = value; }
		}

		private bool m_bPrependInitSeqIE = true;
		[DefaultValue(true)]
		public bool AutoTypePrependInitSequenceForIE
		{
			get { return m_bPrependInitSeqIE; }
			set { m_bPrependInitSeqIE = value; }
		}

		private bool m_bSpecialReleaseAlt = true;
		[DefaultValue(true)]
		public bool AutoTypeReleaseAltWithKeyPress
		{
			get { return m_bSpecialReleaseAlt; }
			set { m_bSpecialReleaseAlt = value; }
		}

		private bool m_bAdjustKeybLayout = true;
		[DefaultValue(true)]
		public bool AutoTypeAdjustKeyboardLayout
		{
			get { return m_bAdjustKeybLayout; }
			set { m_bAdjustKeybLayout = value; }
		}

		private bool m_bAllowInterleaved = false;
		[DefaultValue(false)]
		public bool AutoTypeAllowInterleaved
		{
			get { return m_bAllowInterleaved; }
			set { m_bAllowInterleaved = value; }
		}

		private bool m_bCancelOnWindowChange = false;
		[DefaultValue(false)]
		public bool AutoTypeCancelOnWindowChange
		{
			get { return m_bCancelOnWindowChange; }
			set { m_bCancelOnWindowChange = value; }
		}

		private bool m_bCancelOnTitleChange = false;
		[DefaultValue(false)]
		public bool AutoTypeCancelOnTitleChange
		{
			get { return m_bCancelOnTitleChange; }
			set { m_bCancelOnTitleChange = value; }
		}

		private int m_iInterKeyDelay = -1;
		[DefaultValue(-1)]
		public int AutoTypeInterKeyDelay
		{
			get { return m_iInterKeyDelay; }
			set { m_iInterKeyDelay = value; }
		}

		private ProxyServerType m_pstProxyType = ProxyServerType.System;
		public ProxyServerType ProxyType
		{
			get { return m_pstProxyType; }
			set { m_pstProxyType = value; }
		}

		private string m_strProxyAddr = string.Empty;
		[DefaultValue("")]
		public string ProxyAddress
		{
			get { return m_strProxyAddr; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strProxyAddr = value;
			}
		}

		private string m_strProxyPort = string.Empty;
		[DefaultValue("")]
		public string ProxyPort
		{
			get { return m_strProxyPort; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strProxyPort = value;
			}
		}

		private ProxyAuthType m_pstProxyAuthType = ProxyAuthType.Auto;
		public ProxyAuthType ProxyAuthType
		{
			get { return m_pstProxyAuthType; }
			set { m_pstProxyAuthType = value; }
		}

		private string m_strProxyUser = string.Empty;
		[DefaultValue("")]
		public string ProxyUserName
		{
			get { return m_strProxyUser; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strProxyUser = value;
			}
		}

		private string m_strProxyPassword = string.Empty;
		[DefaultValue("")]
		public string ProxyPassword
		{
			get { return m_strProxyPassword; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strProxyPassword = value;
			}
		}

		public AceIntegration()
		{
		}
	}

	public sealed class AceUrlSchemeOverrides : IDeepCloneable<AceUrlSchemeOverrides>
	{
		private List<AceUrlSchemeOverride> m_lBuiltInOverrides =
			new List<AceUrlSchemeOverride>();
		[XmlIgnore]
		public List<AceUrlSchemeOverride> BuiltInOverrides
		{
			get { return m_lBuiltInOverrides; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_lBuiltInOverrides = value;
			}
		}

		public ulong BuiltInOverridesEnabled
		{
			get { return GetEnabledBuiltInOverrides(); }
			set { SetEnabledBuiltInOverrides(value); }
		}

		private List<AceUrlSchemeOverride> m_lCustomOverrides =
			new List<AceUrlSchemeOverride>();
		[XmlArrayItem("Override")]
		public List<AceUrlSchemeOverride> CustomOverrides
		{
			get { return m_lCustomOverrides; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_lCustomOverrides = value;
			}
		}

		public AceUrlSchemeOverrides()
		{
			MakeBuiltInList();
		}

		private void MakeBuiltInList()
		{
			m_lBuiltInOverrides.Clear();

			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(true, "ssh",
				@"cmd://PuTTY.exe -ssh {USERNAME}@{BASE:RMVSCM}", 0x1));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{INTERNETEXPLORER} \"{BASE}\"", 0x2));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{INTERNETEXPLORER} \"{BASE}\"", 0x4));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{INTERNETEXPLORER} -private \"{BASE}\"", 0x10000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{INTERNETEXPLORER} -private \"{BASE}\"", 0x20000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"microsoft-edge:{BASE}", 0x4000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"microsoft-edge:{BASE}", 0x8000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{FIREFOX} \"{BASE}\"", 0x8));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{FIREFOX} \"{BASE}\"", 0x10));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "chrome",
				"cmd://{FIREFOX} -chrome \"{BASE}\"", 0x20));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{GOOGLECHROME} \"{BASE}\"", 0x100));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{GOOGLECHROME} \"{BASE}\"", 0x200));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{GOOGLECHROME} --incognito \"{BASE}\"", 0x40000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{GOOGLECHROME} --incognito \"{BASE}\"", 0x80000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{OPERA} \"{BASE}\"", 0x40));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{OPERA} \"{BASE}\"", 0x80));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "http",
				"cmd://{SAFARI} \"{BASE}\"", 0x400));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "https",
				"cmd://{SAFARI} \"{BASE}\"", 0x800));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "kdbx",
				"cmd://\"{APPDIR}\\KeePass.exe\" \"{BASE:RMVSCM}\" -pw-enc:\"{PASSWORD_ENC}\"", 0x1000));
			m_lBuiltInOverrides.Add(new AceUrlSchemeOverride(false, "kdbx",
				"cmd://mono \"{APPDIR}/KeePass.exe\" \"{BASE:RMVSCM}\" -pw-enc:\"{PASSWORD_ENC}\"", 0x2000));
			// Free: 0x100000

#if DEBUG
			ulong u = 0;
			foreach(AceUrlSchemeOverride o in m_lBuiltInOverrides)
			{
				Debug.Assert(o.IsBuiltIn);
				ulong f = o.BuiltInFlagID;
				Debug.Assert((f != 0) && ((f & (f - 1)) == 0)); // Check power of 2
				u += f;
			}
			Debug.Assert(u == ((1UL << m_lBuiltInOverrides.Count) - 1UL));
#endif
		}

		public string GetOverrideForUrl(string strUrl)
		{
			if(string.IsNullOrEmpty(strUrl)) return null;

			for(int i = 0; i < 2; ++i)
			{
				List<AceUrlSchemeOverride> l = ((i == 0) ? m_lBuiltInOverrides :
					m_lCustomOverrides);

				foreach(AceUrlSchemeOverride ovr in l)
				{
					if(!ovr.Enabled) continue;

					if(strUrl.StartsWith(ovr.Scheme + ":", StrUtil.CaseIgnoreCmp))
						return ovr.UrlOverride;
				}
			}

			return null;
		}

		public AceUrlSchemeOverrides CloneDeep()
		{
			AceUrlSchemeOverrides ovr = new AceUrlSchemeOverrides();
			CopyTo(ovr);
			return ovr;
		}

		public void CopyTo(AceUrlSchemeOverrides ovrTarget)
		{
			ovrTarget.m_lBuiltInOverrides.Clear();
			foreach(AceUrlSchemeOverride shB in m_lBuiltInOverrides)
			{
				ovrTarget.m_lBuiltInOverrides.Add(shB.CloneDeep());
			}

			ovrTarget.m_lCustomOverrides.Clear();
			foreach(AceUrlSchemeOverride shC in m_lCustomOverrides)
			{
				ovrTarget.m_lCustomOverrides.Add(shC.CloneDeep());
			}
		}

		public ulong GetEnabledBuiltInOverrides()
		{
			ulong u = 0;
			for(int i = 0; i < m_lBuiltInOverrides.Count; ++i)
			{
				if(m_lBuiltInOverrides[i].Enabled)
					u |= m_lBuiltInOverrides[i].BuiltInFlagID;
			}

			return u;
		}

		public void SetEnabledBuiltInOverrides(ulong uFlags)
		{
			for(int i = 0; i < m_lBuiltInOverrides.Count; ++i)
				m_lBuiltInOverrides[i].Enabled = ((uFlags &
					m_lBuiltInOverrides[i].BuiltInFlagID) != 0UL);
		}
	}

	public sealed class AceUrlSchemeOverride : IDeepCloneable<AceUrlSchemeOverride>
	{
		private bool m_bEnabled = true;
		public bool Enabled
		{
			get { return m_bEnabled; }
			set { m_bEnabled = value; }
		}

		private string m_strScheme = string.Empty;
		public string Scheme
		{
			get { return m_strScheme; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strScheme = value;
			}
		}

		private string m_strOvr = string.Empty;
		public string UrlOverride
		{
			get { return m_strOvr; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strOvr = value;
			}
		}

		private ulong m_uBuiltInFlagID = 0;
		[XmlIgnore]
		internal ulong BuiltInFlagID
		{
			get { return m_uBuiltInFlagID; }
		}

		[XmlIgnore]
		public bool IsBuiltIn
		{
			get { return (m_uBuiltInFlagID != 0UL); }
		}

		public AceUrlSchemeOverride()
		{
		}

		public AceUrlSchemeOverride(bool bEnable, string strScheme,
			string strUrlOverride)
		{
			Init(bEnable, strScheme, strUrlOverride, 0);
		}

		internal AceUrlSchemeOverride(bool bEnable, string strScheme,
			string strUrlOverride, ulong uBuiltInFlagID)
		{
			Init(bEnable, strScheme, strUrlOverride, uBuiltInFlagID);
		}

		private void Init(bool bEnable, string strScheme, string strUrlOverride,
			ulong uBuiltInFlagID)
		{
			if(strScheme == null) throw new ArgumentNullException("strScheme");
			if(strUrlOverride == null) throw new ArgumentNullException("strUrlOverride");

			m_bEnabled = bEnable;
			m_strScheme = strScheme;
			m_strOvr = strUrlOverride;
			m_uBuiltInFlagID = uBuiltInFlagID;
		}

		public AceUrlSchemeOverride CloneDeep()
		{
			return new AceUrlSchemeOverride(m_bEnabled, m_strScheme,
				m_strOvr, m_uBuiltInFlagID);
		}
	}
}
