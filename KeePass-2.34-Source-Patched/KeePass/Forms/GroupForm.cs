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
using System.Globalization;

using KeePass.UI;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Collections;

namespace KeePass.Forms
{
	public partial class GroupForm : Form
	{
		private PwGroup m_pwGroup = null;
		private bool m_bCreatingNew = false;
		private ImageList m_ilClientIcons = null;
		private PwDatabase m_pwDatabase = null;

		private PwIcon m_pwIconIndex = 0;
		private PwUuid m_pwCustomIconID = PwUuid.Zero;

		private ExpiryControlGroup m_cgExpiry = new ExpiryControlGroup();

		[Obsolete]
		public void InitEx(PwGroup pg, ImageList ilClientIcons, PwDatabase pwDatabase)
		{
			InitEx(pg, false, ilClientIcons, pwDatabase);
		}

		public void InitEx(PwGroup pg, bool bCreatingNew, ImageList ilClientIcons,
			PwDatabase pwDatabase)
		{
			m_pwGroup = pg;
			m_bCreatingNew = bCreatingNew;
			m_ilClientIcons = ilClientIcons;
			m_pwDatabase = pwDatabase;
		}

		public GroupForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_pwGroup != null); if(m_pwGroup == null) throw new InvalidOperationException();
			Debug.Assert(m_pwDatabase != null); if(m_pwDatabase == null) throw new InvalidOperationException();

			GlobalWindowManager.AddWindow(this);

			string strTitle = (m_bCreatingNew ? KPRes.AddGroup : KPRes.EditGroup);
			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Folder_Txt, strTitle,
				(m_bCreatingNew ? KPRes.AddGroupDesc : KPRes.EditGroupDesc));
			this.Icon = Properties.Resources.KeePass;
			this.Text = strTitle;

			UIUtil.SetButtonImage(m_btnAutoTypeEdit,
				Properties.Resources.B16x16_Wizard, true);

			m_pwIconIndex = m_pwGroup.IconId;
			m_pwCustomIconID = m_pwGroup.CustomIconUuid;
			
			m_tbName.Text = m_pwGroup.Name;
			UIUtil.SetMultilineText(m_tbNotes, m_pwGroup.Notes);

			if(!m_pwCustomIconID.Equals(PwUuid.Zero))
				UIUtil.SetButtonImage(m_btnIcon, DpiUtil.GetIcon(
					m_pwDatabase, m_pwCustomIconID), true);
			else
				UIUtil.SetButtonImage(m_btnIcon, m_ilClientIcons.Images[
					(int)m_pwIconIndex], true);

			if(m_pwGroup.Expires)
			{
				m_dtExpires.Value = m_pwGroup.ExpiryTime;
				m_cbExpires.Checked = true;
			}
			else // Does not expire
			{
				m_dtExpires.Value = DateTime.Now.Date;
				m_cbExpires.Checked = false;
			}
			m_cgExpiry.Attach(m_cbExpires, m_dtExpires);

			PwGroup pgParent = m_pwGroup.ParentGroup;
			bool bParentAutoType = ((pgParent != null) ?
				pgParent.GetAutoTypeEnabledInherited() :
				PwGroup.DefaultAutoTypeEnabled);
			UIUtil.MakeInheritableBoolComboBox(m_cmbEnableAutoType,
				m_pwGroup.EnableAutoType, bParentAutoType);
			bool bParentSearching = ((pgParent != null) ?
				pgParent.GetSearchingEnabledInherited() :
				PwGroup.DefaultSearchingEnabled);
			UIUtil.MakeInheritableBoolComboBox(m_cmbEnableSearching,
				m_pwGroup.EnableSearching, bParentSearching);

			m_tbDefaultAutoTypeSeq.Text = m_pwGroup.GetAutoTypeSequenceInherited();

			if(m_pwGroup.DefaultAutoTypeSequence.Length == 0)
				m_rbAutoTypeInherit.Checked = true;
			else m_rbAutoTypeOverride.Checked = true;

			CustomizeForScreenReader();
			EnableControlsEx();
			UIUtil.SetFocus(m_tbName, this);
		}

		private void CustomizeForScreenReader()
		{
			if(!Program.Config.UI.OptimizeForScreenReader) return;

			m_btnIcon.Text = KPRes.PickIcon;
			m_btnAutoTypeEdit.Text = KPRes.ConfigureAutoType;
		}

		private void EnableControlsEx()
		{
			m_tbDefaultAutoTypeSeq.Enabled = m_btnAutoTypeEdit.Enabled =
				!m_rbAutoTypeInherit.Checked;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			m_pwGroup.Touch(true, false);

			m_pwGroup.Name = m_tbName.Text;
			m_pwGroup.Notes = m_tbNotes.Text;
			m_pwGroup.IconId = m_pwIconIndex;
			m_pwGroup.CustomIconUuid = m_pwCustomIconID;

			m_pwGroup.Expires = m_cgExpiry.Checked;
			m_pwGroup.ExpiryTime = m_cgExpiry.Value;

			m_pwGroup.EnableAutoType = UIUtil.GetInheritableBoolComboBoxValue(m_cmbEnableAutoType);
			m_pwGroup.EnableSearching = UIUtil.GetInheritableBoolComboBoxValue(m_cmbEnableSearching);

			if(m_rbAutoTypeInherit.Checked)
				m_pwGroup.DefaultAutoTypeSequence = string.Empty;
			else m_pwGroup.DefaultAutoTypeSequence = m_tbDefaultAutoTypeSeq.Text;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void CleanUpEx()
		{
			m_cgExpiry.Release();
		}

		private void OnBtnIcon(object sender, EventArgs e)
		{
			IconPickerForm ipf = new IconPickerForm();
			ipf.InitEx(m_ilClientIcons, (uint)PwIcon.Count, m_pwDatabase,
				(uint)m_pwIconIndex, m_pwCustomIconID);

			if(ipf.ShowDialog() == DialogResult.OK)
			{
				if(!ipf.ChosenCustomIconUuid.Equals(PwUuid.Zero)) // Custom icon
				{
					m_pwCustomIconID = ipf.ChosenCustomIconUuid;
					UIUtil.SetButtonImage(m_btnIcon, DpiUtil.GetIcon(
						m_pwDatabase, m_pwCustomIconID), true);
				}
				else // Standard icon
				{
					m_pwIconIndex = (PwIcon)ipf.ChosenIconId;
					m_pwCustomIconID = PwUuid.Zero;
					UIUtil.SetButtonImage(m_btnIcon, m_ilClientIcons.Images[
						(int)m_pwIconIndex], true);
				}
			}

			UIUtil.DestroyForm(ipf);
		}

		private void OnAutoTypeInheritCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnBtnAutoTypeEdit(object sender, EventArgs e)
		{
			// string strName = @"(" + KPRes.AutoType + @")";

			AutoTypeConfig atConfig = new AutoTypeConfig();
			atConfig.DefaultSequence = m_tbDefaultAutoTypeSeq.Text;

			EditAutoTypeItemForm dlg = new EditAutoTypeItemForm();
			dlg.InitEx(atConfig, -1, true, atConfig.DefaultSequence, null);

			if(dlg.ShowDialog() == DialogResult.OK)
				m_tbDefaultAutoTypeSeq.Text = atConfig.DefaultSequence;

			UIUtil.DestroyForm(dlg);
			EnableControlsEx();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}
	}
}
