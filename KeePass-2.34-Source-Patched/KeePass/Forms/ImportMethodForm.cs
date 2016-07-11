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

using KeePassLib;

namespace KeePass.Forms
{
	public partial class ImportMethodForm : Form
	{
		PwMergeMethod m_mmSelected = PwMergeMethod.CreateNewUuids;

		public PwMergeMethod MergeMethod
		{
			get { return m_mmSelected; }
		}

		public ImportMethodForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);
			try { if(this.Owner == null) this.Owner = Program.MainForm; }
			catch(Exception) { Debug.Assert(false); }

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Folder_Download, KPRes.ImportBehavior,
				KPRes.ImportBehaviorDesc);
			this.Icon = Properties.Resources.KeePass;

			this.Text = KPRes.ImportBehavior;

			m_radioCreateNew.Text = KPRes.CreateNewIDs;
			m_radioKeepExisting.Text = KPRes.KeepExisting;
			m_radioOverwrite.Text = KPRes.OverwriteExisting;
			m_radioOverwriteIfNewer.Text = KPRes.OverwriteIfNewer;
			m_radioSynchronize.Text = KPRes.OverwriteIfNewerAndApplyDel;

			FontUtil.AssignDefaultBold(m_radioCreateNew);
			FontUtil.AssignDefaultBold(m_radioKeepExisting);
			FontUtil.AssignDefaultBold(m_radioOverwrite);
			FontUtil.AssignDefaultBold(m_radioOverwriteIfNewer);
			FontUtil.AssignDefaultBold(m_radioSynchronize);

			m_radioCreateNew.Checked = true;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(m_radioCreateNew.Checked)
				m_mmSelected = PwMergeMethod.CreateNewUuids;
			else if(m_radioKeepExisting.Checked)
				m_mmSelected = PwMergeMethod.KeepExisting;
			else if(m_radioOverwrite.Checked)
				m_mmSelected = PwMergeMethod.OverwriteExisting;
			else if(m_radioOverwriteIfNewer.Checked)
				m_mmSelected = PwMergeMethod.OverwriteIfNewer;
			else if(m_radioSynchronize.Checked)
				m_mmSelected = PwMergeMethod.Synchronize;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}
	}
}