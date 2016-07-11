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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.UI;
using KeePass.Resources;
using KeePass.Native;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class EntryListForm : Form
	{
		private string m_strTitle = string.Empty;
		private string m_strDescShort = string.Empty;
		private string m_strDescLong = string.Empty;
		private Image m_imgIcon = null;
		private ImageList m_ilIcons = null;
		private PwObjectList<PwEntry> m_vEntries = null;
		private PwEntry m_peSelected = null;
		private bool m_bEnsureForeground = false;

		public PwEntry SelectedEntry
		{
			get { return m_peSelected; }
		}

		public bool EnsureForeground
		{
			get { return m_bEnsureForeground; }
			set { m_bEnsureForeground = value; }
		}

		public void InitEx(string strTitle, string strDescShort,
			string strDescLong, Image imgIcon, ImageList ilIcons,
			PwObjectList<PwEntry> vEntries)
		{
			m_strTitle = strTitle;
			m_strDescShort = strDescShort;
			m_strDescLong = strDescLong;
			m_imgIcon = imgIcon;
			m_ilIcons = UIUtil.CloneImageList(ilIcons, true);
			m_vEntries = vEntries;
		}

		public EntryListForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_strTitle.Length > 0);
			Debug.Assert(m_imgIcon != null);
			Debug.Assert(m_ilIcons != null);

			GlobalWindowManager.AddWindow(this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				m_imgIcon, m_strTitle, m_strDescShort);
			m_lblText.Text = m_strDescLong;
			this.Text = m_strTitle;
			this.Icon = Properties.Resources.KeePass;

			if(m_ilIcons != null) m_lvEntries.SmallImageList = m_ilIcons;
			else m_ilIcons = new ImageList();

			List<KeyValuePair<string, string>> vColumns =
				new List<KeyValuePair<string, string>>();
			vColumns.Add(new KeyValuePair<string, string>(PwDefs.TitleField, KPRes.Title));
			vColumns.Add(new KeyValuePair<string, string>(PwDefs.UserNameField, KPRes.UserName));
			vColumns.Add(new KeyValuePair<string, string>(PwDefs.UrlField, KPRes.Url));

			UIUtil.CreateEntryList(m_lvEntries, m_vEntries, vColumns, m_ilIcons);

			ProcessResize();

			if(m_bEnsureForeground)
			{
				this.BringToFront();
				this.Activate();
			}

			EnableControlsEx();
		}

		private void EnableControlsEx()
		{
			bool bCond = (m_lvEntries.SelectedIndices.Count > 0);
			bool bCur = m_btnOK.Enabled;
			if(bCond != bCur) m_btnOK.Enabled = bCond;
		}

		private void ProcessResize()
		{
			UIUtil.ResizeColumns(m_lvEntries, true);
		}

		private bool GetSelectedEntry(bool bSetDialogResult)
		{
			ListView.SelectedListViewItemCollection slvic = m_lvEntries.SelectedItems;
			if(slvic.Count == 1)
			{
				m_peSelected = (slvic[0].Tag as PwEntry);

				if(bSetDialogResult) this.DialogResult = DialogResult.OK;
				return true;
			}

			return false;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(GetSelectedEntry(false) == false)
				this.DialogResult = DialogResult.None;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void CleanUpEx()
		{
			if(m_ilIcons != null)
			{
				m_lvEntries.SmallImageList = null; // Detach event handlers
				m_ilIcons.Dispose();
				m_ilIcons = null;
			}
		}

		private void OnEntriesItemActivate(object sender, EventArgs e)
		{
			if(GetSelectedEntry(true))
				m_lvEntries.Enabled = false;
		}

		private void OnEntriesSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();

			// ListView.SelectedIndexCollection lvsic = m_lvEntries.SelectedIndices;
			// if((lvsic != null) && (lvsic.Count == 1))
			//	NativeMethods.EnsureVisible(m_lvEntries, lvsic[0], false);
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}
	}
}
