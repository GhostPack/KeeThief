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

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;

namespace KeePass.Forms
{
	public partial class AutoTypeCtxForm : Form
	{
		private List<AutoTypeCtx> m_lCtxs = null;
		private ImageList m_ilIcons = null;

		private string m_strInitialFormRect = string.Empty;
		private string m_strInitialColWidths = string.Empty;
		private int m_nBannerWidth = -1;
		private bool m_bCanShowPasswords = true;

		private CustomContextMenuStripEx m_ctxTools = null;
		private ToolStripMenuItem m_tsmiColumns = null;

		private AutoTypeCtx m_atcSel = null;
		public AutoTypeCtx SelectedCtx
		{
			get { return m_atcSel; }
		}

		public void InitEx(List<AutoTypeCtx> lCtxs, ImageList ilIcons)
		{
			m_lCtxs = lCtxs;
			m_ilIcons = UIUtil.CloneImageList(ilIcons, true);
		}

		public AutoTypeCtxForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Size sz = this.ClientSize;
			this.MinimumSize = new Size((int)(0.949f * (float)sz.Width),
				(int)(0.824f * (float)sz.Height));

			GlobalWindowManager.AddWindow(this);

			m_lblText.Text = KPRes.AutoTypeEntrySelectionDescLong2;
			this.Text = KPRes.AutoTypeEntrySelection;
			this.Icon = Properties.Resources.KeePass;

			string strRect = Program.Config.UI.AutoTypeCtxRect;
			if(strRect.Length > 0) UIUtil.SetWindowScreenRect(this, strRect);
			m_strInitialFormRect = UIUtil.GetWindowScreenRect(this);

			UIUtil.SetExplorerTheme(m_lvItems, true);

			if(m_ilIcons != null) m_lvItems.SmallImageList = m_ilIcons;
			else { Debug.Assert(false); m_ilIcons = new ImageList(); }

			m_bCanShowPasswords = AppPolicy.Current.UnhidePasswords;

			RecreateEntryList();

			string strColWidths = Program.Config.UI.AutoTypeCtxColumnWidths;
			if(strColWidths.Length > 0) UIUtil.SetColumnWidths(m_lvItems, strColWidths);
			m_strInitialColWidths = UIUtil.GetColumnWidths(m_lvItems);

			ProcessResize();
			this.BringToFront();
			this.Activate();
			UIUtil.SetFocus(m_lvItems, this);
		}

		private void RecreateEntryList()
		{
			long lFlags = Program.Config.UI.AutoTypeCtxFlags;

			if(!m_bCanShowPasswords)
				lFlags &= ~(long)AceAutoTypeCtxFlags.ColPassword;

			UIUtil.CreateEntryList(m_lvItems, m_lCtxs, (AceAutoTypeCtxFlags)lFlags,
				m_ilIcons);
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}

		private void CleanUpEx()
		{
			string strColWidths = UIUtil.GetColumnWidths(m_lvItems);
			if(strColWidths != m_strInitialColWidths)
				Program.Config.UI.AutoTypeCtxColumnWidths = strColWidths;

			string strRect = UIUtil.GetWindowScreenRect(this);
			if(strRect != m_strInitialFormRect)
				Program.Config.UI.AutoTypeCtxRect = strRect;

			DestroyToolsContextMenu();

			if(m_ilIcons != null)
			{
				m_lvItems.SmallImageList = null; // Detach event handlers
				m_ilIcons.Dispose();
				m_ilIcons = null;
			}
		}

		private void ProcessResize()
		{
			if(m_lCtxs == null) return; // TrlUtil or design mode

			string strSub = KPRes.AutoTypeEntrySelectionDescShort;
			int n = m_lCtxs.Count;
			if(n == 1) strSub = KPRes.SearchEntriesFound1 + ".";
			else if(n <= 0)
			{
				strSub = KPRes.SearchEntriesFound + ".";
				strSub = strSub.Replace(@"{PARAM}", "0");
			}

			BannerFactory.UpdateBanner(this, m_bannerImage,
				Properties.Resources.B48x48_KGPG_Key2, KPRes.AutoTypeEntrySelection,
				strSub, ref m_nBannerWidth);
		}

		private bool GetSelectedEntry()
		{
			ListView.SelectedListViewItemCollection slvic = m_lvItems.SelectedItems;
			if(slvic.Count == 1)
			{
				m_atcSel = (slvic[0].Tag as AutoTypeCtx);
				return (m_atcSel != null);
			}

			return false;
		}

		private void ProcessItemSelection()
		{
			if(this.DialogResult == DialogResult.OK) return; // Already closing

			if(GetSelectedEntry()) this.DialogResult = DialogResult.OK;
		}

		private void OnListItemActivate(object sender, EventArgs e)
		{
			ProcessItemSelection();
		}

		private void OnListClick(object sender, EventArgs e)
		{
			ProcessItemSelection();
		}

		private void OnFormResize(object sender, EventArgs e)
		{
			ProcessResize();
		}

		private void DestroyToolsContextMenu()
		{
			if(m_ctxTools == null) return;

			foreach(ToolStripItem tsi in m_tsmiColumns.DropDownItems)
				tsi.Click -= this.OnToggleColumn;

			m_tsmiColumns = null;
			m_ctxTools.Dispose();
			m_ctxTools = null;
		}

		private void RecreateToolsContextMenu()
		{
			DestroyToolsContextMenu();

			m_ctxTools = new CustomContextMenuStripEx();
			m_tsmiColumns = new ToolStripMenuItem(KPRes.Columns);
			m_ctxTools.Items.Add(m_tsmiColumns);

			long lFlags = Program.Config.UI.AutoTypeCtxFlags;

			ToolStripMenuItem tsmi = new ToolStripMenuItem(KPRes.Title);
			UIUtil.SetChecked(tsmi, true);
			tsmi.Tag = AceAutoTypeCtxFlags.ColTitle;
			tsmi.Click += this.OnToggleColumn;
			tsmi.Enabled = false;
			m_tsmiColumns.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.UserName);
			UIUtil.SetChecked(tsmi, ((lFlags & (long)AceAutoTypeCtxFlags.ColUserName) != 0));
			tsmi.Tag = AceAutoTypeCtxFlags.ColUserName;
			tsmi.Click += this.OnToggleColumn;
			m_tsmiColumns.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Password);
			UIUtil.SetChecked(tsmi, (((lFlags & (long)AceAutoTypeCtxFlags.ColPassword) != 0) &&
				m_bCanShowPasswords));
			tsmi.Tag = AceAutoTypeCtxFlags.ColPassword;
			tsmi.Click += this.OnToggleColumn;
			if(!m_bCanShowPasswords) tsmi.Enabled = false;
			m_tsmiColumns.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Url);
			UIUtil.SetChecked(tsmi, ((lFlags & (long)AceAutoTypeCtxFlags.ColUrl) != 0));
			tsmi.Tag = AceAutoTypeCtxFlags.ColUrl;
			tsmi.Click += this.OnToggleColumn;
			m_tsmiColumns.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Notes);
			UIUtil.SetChecked(tsmi, ((lFlags & (long)AceAutoTypeCtxFlags.ColNotes) != 0));
			tsmi.Tag = AceAutoTypeCtxFlags.ColNotes;
			tsmi.Click += this.OnToggleColumn;
			m_tsmiColumns.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Sequence + " - " + KPRes.Comments);
			UIUtil.SetChecked(tsmi, ((lFlags & (long)AceAutoTypeCtxFlags.ColSequenceComments) != 0));
			tsmi.Tag = AceAutoTypeCtxFlags.ColSequenceComments;
			tsmi.Click += this.OnToggleColumn;
			m_tsmiColumns.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Sequence);
			UIUtil.SetChecked(tsmi, ((lFlags & (long)AceAutoTypeCtxFlags.ColSequence) != 0));
			tsmi.Tag = AceAutoTypeCtxFlags.ColSequence;
			tsmi.Click += this.OnToggleColumn;
			m_tsmiColumns.DropDownItems.Add(tsmi);
		}

		private void OnToggleColumn(object sender, EventArgs e)
		{
			ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
			if(tsmi == null) { Debug.Assert(false); return; }

			AceAutoTypeCtxFlags f = (AceAutoTypeCtxFlags)tsmi.Tag;
			long lFlags = Program.Config.UI.AutoTypeCtxFlags;

			lFlags ^= (long)f;
			lFlags |= (long)AceAutoTypeCtxFlags.ColTitle; // Enforce title

			Program.Config.UI.AutoTypeCtxFlags = lFlags;
			RecreateEntryList();
		}

		private void OnBtnTools(object sender, EventArgs e)
		{
			RecreateToolsContextMenu();
			m_ctxTools.Show(m_btnTools, 0, m_btnTools.Height);
		}
	}
}
