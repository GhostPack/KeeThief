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

using KeePass.App.Configuration;
using KeePass.Resources;
using KeePass.UI;

namespace KeePass.Forms
{
	public partial class UrlOverridesForm : Form
	{
		private AceUrlSchemeOverrides m_aceOvr = null;
		private AceUrlSchemeOverrides m_aceTmp = null;

		private bool m_bEnfSch = false;
		private bool m_bEnfAll = false;

		private string m_strUrlOverrideAll = string.Empty;
		public string UrlOverrideAll
		{
			get { return m_strUrlOverrideAll; }
		}

		public void InitEx(AceUrlSchemeOverrides aceOvr, string strOverrideAll)
		{
			m_aceOvr = aceOvr;

			Debug.Assert(strOverrideAll != null);
			m_strUrlOverrideAll = (strOverrideAll ?? string.Empty);
		}

		public UrlOverridesForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if(m_aceOvr == null) throw new InvalidOperationException();
			m_aceTmp = m_aceOvr.CloneDeep();

			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.UrlOverrides;

			UIUtil.SetExplorerTheme(m_lvOverrides, false);

			int nWidth = m_lvOverrides.ClientSize.Width - UIUtil.GetVScrollBarWidth();
			m_lvOverrides.Columns.Add(KPRes.Scheme, nWidth / 4);
			m_lvOverrides.Columns.Add(KPRes.UrlOverride, (nWidth * 3) / 4);

			m_bEnfSch = AppConfigEx.IsOptionEnforced(Program.Config.Integration, "UrlSchemeOverrides");
			m_bEnfAll = AppConfigEx.IsOptionEnforced(Program.Config.Integration, "UrlOverride");

			UpdateOverridesList(false, false);

			m_cbOverrideAll.Checked = (m_strUrlOverrideAll.Length > 0);
			m_tbOverrideAll.Text = m_strUrlOverrideAll;
			EnableControlsEx();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void UpdateOverridesList(bool bRestoreView, bool bUpdateState)
		{
			UIScrollInfo s = (bRestoreView ? UIUtil.GetScrollInfo(
				m_lvOverrides, true) : null);

			m_lvOverrides.BeginUpdate();
			m_lvOverrides.Items.Clear();
			m_lvOverrides.Groups.Clear();

			for(int i = 0; i < 2; ++i)
			{
				List<AceUrlSchemeOverride> l = ((i == 0) ?
					m_aceTmp.BuiltInOverrides : m_aceTmp.CustomOverrides);

				ListViewGroup lvg = new ListViewGroup((i == 0) ?
					KPRes.OverridesBuiltIn : KPRes.OverridesCustom);
				m_lvOverrides.Groups.Add(lvg);

				foreach(AceUrlSchemeOverride ovr in l)
				{
					ListViewItem lvi = new ListViewItem(ovr.Scheme);
					lvi.SubItems.Add(ovr.UrlOverride);
					lvi.Tag = ovr; // Set before setting the Checked property

					lvi.Checked = ovr.Enabled;

					m_lvOverrides.Items.Add(lvi);
					lvg.Items.Add(lvi);
				}
			}

			if(bRestoreView) UIUtil.Scroll(m_lvOverrides, s, false);

			m_lvOverrides.EndUpdate();

			if(bUpdateState) EnableControlsEx();
		}

		private void OnOverridesItemChecked(object sender, ItemCheckedEventArgs e)
		{
			AceUrlSchemeOverride ovr = (e.Item.Tag as AceUrlSchemeOverride);
			if(ovr == null) { Debug.Assert(false); return; }

			ovr.Enabled = e.Item.Checked;
		}

		private void EnableControlsEx()
		{
			bool bAll = m_cbOverrideAll.Checked;
			m_cbOverrideAll.Enabled = !m_bEnfAll;
			m_tbOverrideAll.Enabled = (!m_bEnfAll && bAll);

			ListView.SelectedListViewItemCollection lvsc = m_lvOverrides.SelectedItems;
			bool bOne = (lvsc.Count == 1);
			bool bAtLeastOne = (lvsc.Count >= 1);

			bool bBuiltIn = false;
			foreach(ListViewItem lvi in lvsc)
			{
				AceUrlSchemeOverride ovr = (lvi.Tag as AceUrlSchemeOverride);
				if(ovr == null) { Debug.Assert(false); continue; }

				if(ovr.IsBuiltIn) { bBuiltIn = true; break; }
			}

			bool bSch = !m_bEnfSch;
			m_lvOverrides.Enabled = bSch;
			m_btnAdd.Enabled = bSch;
			m_btnEdit.Enabled = (bSch && bOne && !bBuiltIn);
			m_btnDelete.Enabled = (bSch && bAtLeastOne && !bBuiltIn);
		}

		private void OnOverridesSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnBtnAdd(object sender, EventArgs e)
		{
			AceUrlSchemeOverride ovr = new AceUrlSchemeOverride(true, string.Empty,
				string.Empty);

			UrlOverrideForm dlg = new UrlOverrideForm();
			dlg.InitEx(ovr);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
			{
				m_aceTmp.CustomOverrides.Add(ovr);
				UpdateOverridesList(true, true);
				// m_lvOverrides.EnsureVisible(m_lvOverrides.Items.Count - 1);
			}
		}

		private void OnBtnEdit(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvOverrides.SelectedItems;
			if((lvsic == null) || (lvsic.Count != 1)) return;

			AceUrlSchemeOverride ovr = (lvsic[0].Tag as AceUrlSchemeOverride);
			if(ovr == null) { Debug.Assert(false); return; }
			if(ovr.IsBuiltIn) { Debug.Assert(false); return; }

			UrlOverrideForm dlg = new UrlOverrideForm();
			dlg.InitEx(ovr);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
				UpdateOverridesList(true, true);
		}

		private void OnBtnDelete(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvOverrides.SelectedItems;
			if((lvsic == null) || (lvsic.Count == 0)) return;

			foreach(ListViewItem lvi in lvsic)
			{
				AceUrlSchemeOverride ovr = (lvi.Tag as AceUrlSchemeOverride);
				if(ovr == null) { Debug.Assert(false); continue; }
				if(ovr.IsBuiltIn) { Debug.Assert(false); continue; }

				try { m_aceTmp.CustomOverrides.Remove(ovr); }
				catch(Exception) { Debug.Assert(false); }
			}

			UpdateOverridesList(true, true);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			m_aceTmp.CopyTo(m_aceOvr);

			if(m_cbOverrideAll.Checked)
				m_strUrlOverrideAll = m_tbOverrideAll.Text;
			else m_strUrlOverrideAll = string.Empty;
		}

		private void OnOverrideAllCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}
	}
}
