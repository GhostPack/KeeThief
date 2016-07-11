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
using System.ComponentModel;
using System.Diagnostics;

using KeePass.UI;

namespace KeePass.App.Configuration
{
	[Flags]
	public enum AceKeyUIFlags : ulong
	{
		None = 0,
		EnablePassword = 0x1,
		EnableKeyFile = 0x2,
		EnableUserAccount = 0x4,
		EnableHidePassword = 0x8,
		DisablePassword = 0x100,
		DisableKeyFile = 0x200,
		DisableUserAccount = 0x400,
		DisableHidePassword = 0x800,
		CheckPassword = 0x10000,
		CheckKeyFile = 0x20000,
		CheckUserAccount = 0x40000,
		CheckHidePassword = 0x80000,
		UncheckPassword = 0x1000000,
		UncheckKeyFile = 0x2000000,
		UncheckUserAccount = 0x4000000,
		UncheckHidePassword = 0x8000000
	}

	[Flags]
	public enum AceUIFlags : ulong
	{
		None = 0,

		DisableOptions = 0x1,
		DisablePlugins = 0x2,
		DisableTriggers = 0x4,
		DisableKeyChangeDays = 0x8,
		HidePwQuality = 0x10,
		DisableUpdateCheck = 0x20,

		HideBuiltInPwGenPrfInEntryDlg = 0x10000,
		ShowLastAccessTime = 0x20000
	}

	[Flags]
	public enum AceAutoTypeCtxFlags : long
	{
		None = 0,

		ColTitle = 0x1,
		ColUserName = 0x2,
		ColPassword = 0x4,
		ColUrl = 0x8,
		ColNotes = 0x10,
		ColSequence = 0x20,
		ColSequenceComments = 0x40,

		Default = (ColTitle | ColUserName | ColUrl | ColSequence)
	}

	public sealed class AceUI
	{
		public AceUI()
		{
		}

		private AceTrayIcon m_tray = new AceTrayIcon();
		public AceTrayIcon TrayIcon
		{
			get { return m_tray; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_tray = value;
			}
		}

		private AceHiding m_uiHiding = new AceHiding();
		public AceHiding Hiding
		{
			get { return m_uiHiding; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_uiHiding = value;
			}
		}

		private bool m_bRepeatPwOnlyWhenHidden = true;
		[DefaultValue(true)]
		public bool RepeatPasswordOnlyWhenHidden
		{
			get { return m_bRepeatPwOnlyWhenHidden; }
			set { m_bRepeatPwOnlyWhenHidden = value; }
		}

		private AceFont m_font = new AceFont();
		public AceFont StandardFont
		{
			get { return m_font; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_font = value;
			}
		}

		private AceFont m_fontPasswords = new AceFont(true);
		public AceFont PasswordFont
		{
			get { return m_fontPasswords; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_fontPasswords = value;
			}
		}

		private bool m_bForceSysFont = true;
		[DefaultValue(true)]
		public bool ForceSystemFontUnix
		{
			get { return m_bForceSysFont; }
			set { m_bForceSysFont = value; }
		}

		private BannerStyle m_bannerStyle = BannerStyle.WinVistaBlack;
		public BannerStyle BannerStyle
		{
			get { return m_bannerStyle; }
			set { m_bannerStyle = value; }
		}

		private bool m_bShowImportStatusDlg = true;
		[DefaultValue(true)]
		public bool ShowImportStatusDialog
		{
			get { return m_bShowImportStatusDlg; }
			set { m_bShowImportStatusDlg = value; }
		}

		private bool m_bShowDbMntncResDlg = true;
		[DefaultValue(true)]
		public bool ShowDbMntncResultsDialog
		{
			get { return m_bShowDbMntncResDlg; }
			set { m_bShowDbMntncResDlg = value; }
		}

		private bool m_bShowRecycleDlg = true;
		[DefaultValue(true)]
		public bool ShowRecycleConfirmDialog
		{
			get { return m_bShowRecycleDlg; }
			set { m_bShowRecycleDlg = value; }
		}

		// private bool m_bUseCustomTsRenderer = true;
		// [DefaultValue(true)]
		// public bool UseCustomToolStripRenderer
		// {
		//	get { return m_bUseCustomTsRenderer; }
		//	set { m_bUseCustomTsRenderer = value; }
		// }

		private string m_strToolStripRenderer = string.Empty;
		[DefaultValue("")]
		public string ToolStripRenderer
		{
			get { return m_strToolStripRenderer; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strToolStripRenderer = value;
			}
		}

		private bool m_bOptScreenReader = false;
		[DefaultValue(false)]
		public bool OptimizeForScreenReader
		{
			get { return m_bOptScreenReader; }
			set { m_bOptScreenReader = value; }
		}

		private string m_strDataEditorRect = string.Empty;
		[DefaultValue("")]
		public string DataEditorRect
		{
			get { return m_strDataEditorRect; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strDataEditorRect = value;
			}
		}

		private AceFont m_deFont = new AceFont();
		public AceFont DataEditorFont
		{
			get { return m_deFont; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_deFont = value;
			}
		}

		private bool m_bDeWordWrap = true;
		[DefaultValue(true)]
		public bool DataEditorWordWrap
		{
			get { return m_bDeWordWrap; }
			set { m_bDeWordWrap = value; }
		}

		private string m_strCharPickerRect = string.Empty;
		[DefaultValue("")]
		public string CharPickerRect
		{
			get { return m_strCharPickerRect; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strCharPickerRect = value;
			}
		}

		private string m_strAutoTypeCtxRect = string.Empty;
		[DefaultValue("")]
		public string AutoTypeCtxRect
		{
			get { return m_strAutoTypeCtxRect; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strAutoTypeCtxRect = value;
			}
		}

		private long m_lAutoTypeCtxFlags = (long)AceAutoTypeCtxFlags.Default;
		[DefaultValue((long)AceAutoTypeCtxFlags.Default)]
		public long AutoTypeCtxFlags
		{
			get { return m_lAutoTypeCtxFlags; }
			set { m_lAutoTypeCtxFlags = value; }
		}

		private string m_strAutoTypeCtxColWidths = string.Empty;
		[DefaultValue("")]
		public string AutoTypeCtxColumnWidths
		{
			get { return m_strAutoTypeCtxColWidths; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strAutoTypeCtxColWidths = value;
			}
		}

		private ulong m_uUIFlags = (ulong)AceUIFlags.None;
		public ulong UIFlags
		{
			get { return m_uUIFlags; }
			set { m_uUIFlags = value; }
		}

		private ulong m_uKeyCreationFlags = (ulong)AceKeyUIFlags.None;
		public ulong KeyCreationFlags
		{
			get { return m_uKeyCreationFlags; }
			set { m_uKeyCreationFlags = value; }
		}

		private ulong m_uKeyPromptFlags = (ulong)AceKeyUIFlags.None;
		public ulong KeyPromptFlags
		{
			get { return m_uKeyPromptFlags; }
			set { m_uKeyPromptFlags = value; }
		}

		// private bool m_bEditCancelConfirmation = true;
		// public bool EntryEditCancelConfirmation
		// {
		//	get { return m_bEditCancelConfirmation; }
		//	set { m_bEditCancelConfirmation = value; }
		// }

		private bool m_bSecDeskSound = true;
		[DefaultValue(true)]
		public bool SecureDesktopPlaySound
		{
			get { return m_bSecDeskSound; }
			set { m_bSecDeskSound = value; }
		}
	}

	public sealed class AceHiding
	{
		public AceHiding()
		{
		}

		private bool m_bSepHiding = false;
		[DefaultValue(false)]
		public bool SeparateHidingSettings
		{
			get { return m_bSepHiding; }
			set { m_bSepHiding = value; }
		}

		private bool m_bHideInEntryDialog = true;
		[DefaultValue(true)]
		public bool HideInEntryWindow
		{
			get { return m_bHideInEntryDialog; }
			set { m_bHideInEntryDialog = value; }
		}

		private bool m_bUnhideBtnAlsoUnhidesSec = false;
		[DefaultValue(false)]
		public bool UnhideButtonAlsoUnhidesSource
		{
			get { return m_bUnhideBtnAlsoUnhidesSec; }
			set { m_bUnhideBtnAlsoUnhidesSec = value; }
		}
	}

	public sealed class AceFont
	{
		private Font m_fCached = null;
		private bool m_bCacheValid = false;

		private string m_strFamily = "Microsoft Sans Serif";
		public string Family
		{
			get { return m_strFamily; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strFamily = value;
				m_bCacheValid = false;
			}
		}

		private float m_fSize = 8.25f;
		public float Size
		{
			get { return m_fSize; }
			set { m_fSize = value; m_bCacheValid = false; }
		}

		private GraphicsUnit m_gu = GraphicsUnit.Point;
		public GraphicsUnit GraphicsUnit
		{
			get { return m_gu; }
			set { m_gu = value; m_bCacheValid = false; }
		}

		private FontStyle m_fStyle = FontStyle.Regular;
		public FontStyle Style
		{
			get { return m_fStyle; }
			set { m_fStyle = value; m_bCacheValid = false; }
		}

		private bool m_bOverrideUIDefault = false;
		public bool OverrideUIDefault
		{
			get { return m_bOverrideUIDefault; }
			set { m_bOverrideUIDefault = value; }
		}

		public AceFont()
		{
		}

		public AceFont(Font f)
		{
			if(f == null) throw new ArgumentNullException("f");

			this.Family = f.FontFamily.Name;
			m_fSize = f.Size;
			m_fStyle = f.Style;
			m_gu = f.Unit;
		}

		public AceFont(bool bMonospace)
		{
			if(bMonospace) m_strFamily = "Courier New";
		}

		public Font ToFont()
		{
			if(m_bCacheValid) return m_fCached;

			m_fCached = FontUtil.CreateFont(m_strFamily, m_fSize, m_fStyle, m_gu);
			m_bCacheValid = true;
			return m_fCached;
		}
	}
}
