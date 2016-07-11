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
using KeePass.UI;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class DatabaseSettingsForm : Form
	{
		private bool m_bCreatingNew = false;
		private PwDatabase m_pwDatabase = null;

		private Color m_clr = Color.Empty;

		private ContextMenu m_ctxColor = null;
		private List<ColorMenuItem> m_vColorItems = new List<ColorMenuItem>();

		private string m_strAutoCreateNew = "(" + KPRes.AutoCreateNew + ")";
		private Dictionary<int, PwUuid> m_dictRecycleBinGroups = new Dictionary<int, PwUuid>();

		private Dictionary<int, PwUuid> m_dictEntryTemplateGroups = new Dictionary<int, PwUuid>();

		private bool m_bInitializing = true;

		public DatabaseSettingsForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		public void InitEx(bool bCreatingNew, PwDatabase pwDatabase)
		{
			m_bCreatingNew = bCreatingNew;

			Debug.Assert(pwDatabase != null); if(pwDatabase == null) throw new ArgumentNullException("pwDatabase");
			m_pwDatabase = pwDatabase;
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_pwDatabase != null); if(m_pwDatabase == null) throw new InvalidOperationException();

			m_bInitializing = true;

			GlobalWindowManager.AddWindow(this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Ark, KPRes.DatabaseSettings,
				KPRes.DatabaseSettingsDesc);
			this.Icon = Properties.Resources.KeePass;

			FontUtil.AssignDefaultItalic(m_lblHeaderCpAlgo);
			FontUtil.AssignDefaultItalic(m_lblHeaderCp);
			FontUtil.AssignDefaultItalic(m_lblHeaderPerf);

			FontUtil.AssignDefaultBold(m_rbNone);
			FontUtil.AssignDefaultBold(m_rbGZip);

			m_ttRect.SetToolTip(m_lnkCompute1SecDelay, KPRes.TransformationRounds1SecHint);

			m_tbDbName.PromptText = KPRes.DatabaseNamePrompt;
			m_tbDbDesc.PromptText = KPRes.DatabaseDescPrompt;

			if(m_bCreatingNew) this.Text = KPRes.ConfigureOnNewDatabase;
			else this.Text = KPRes.DatabaseSettings;

			m_tbDbName.Text = m_pwDatabase.Name;
			UIUtil.SetMultilineText(m_tbDbDesc, m_pwDatabase.Description);
			m_tbDefaultUser.Text = m_pwDatabase.DefaultUserName;

			m_clr = m_pwDatabase.Color;
			if(m_clr != Color.Empty)
				UIUtil.SetButtonImage(m_btnColor, PwEntryForm.CreateColorButtonImage(
					m_btnColor, m_clr), false);
			m_cbColor.Checked = (m_clr != Color.Empty);

			for(int inx = 0; inx < CipherPool.GlobalPool.EngineCount; ++inx)
				m_cmbEncAlgo.Items.Add(CipherPool.GlobalPool[inx].DisplayName);

			if(m_cmbEncAlgo.Items.Count > 0)
			{
				int nIndex = CipherPool.GlobalPool.GetCipherIndex(m_pwDatabase.DataCipherUuid);
				m_cmbEncAlgo.SelectedIndex = ((nIndex >= 0) ? nIndex : 0);
			}

			m_numEncRounds.Minimum = ulong.MinValue;
			m_numEncRounds.Maximum = ulong.MaxValue;
			m_numEncRounds.Value = m_pwDatabase.KeyEncryptionRounds;

			// m_lbMemProt.Items.Add(KPRes.Title, m_pwDatabase.MemoryProtection.ProtectTitle);
			// m_lbMemProt.Items.Add(KPRes.UserName, m_pwDatabase.MemoryProtection.ProtectUserName);
			// m_lbMemProt.Items.Add(KPRes.Password, m_pwDatabase.MemoryProtection.ProtectPassword);
			// m_lbMemProt.Items.Add(KPRes.Url, m_pwDatabase.MemoryProtection.ProtectUrl);
			// m_lbMemProt.Items.Add(KPRes.Notes, m_pwDatabase.MemoryProtection.ProtectNotes);

			// m_cbAutoEnableHiding.Checked = m_pwDatabase.MemoryProtection.AutoEnableVisualHiding;
			// m_cbAutoEnableHiding.Checked = false;

			if(m_pwDatabase.Compression == PwCompressionAlgorithm.None)
				m_rbNone.Checked = true;
			else if(m_pwDatabase.Compression == PwCompressionAlgorithm.GZip)
				m_rbGZip.Checked = true;
			else { Debug.Assert(false); }

			InitRecycleBinTab();
			InitAdvancedTab();

			m_bInitializing = false;
			EnableControlsEx();
		}

		private void InitRecycleBinTab()
		{
			m_cbRecycleBin.Checked = m_pwDatabase.RecycleBinEnabled;

			m_cmbRecycleBin.Items.Add(m_strAutoCreateNew);
			m_dictRecycleBinGroups[0] = PwUuid.Zero;

			int iSelect;
			UIUtil.CreateGroupList(m_pwDatabase.RootGroup, m_cmbRecycleBin,
				m_dictRecycleBinGroups, m_pwDatabase.RecycleBinUuid, out iSelect);

			m_cmbRecycleBin.SelectedIndex = Math.Max(0, iSelect);
		}

		private void InitAdvancedTab()
		{
			m_cmbEntryTemplates.Items.Add("(" + KPRes.None + ")");
			m_dictEntryTemplateGroups[0] = PwUuid.Zero;

			int iSelect;
			UIUtil.CreateGroupList(m_pwDatabase.RootGroup, m_cmbEntryTemplates,
				m_dictEntryTemplateGroups, m_pwDatabase.EntryTemplatesGroup, out iSelect);

			m_cmbEntryTemplates.SelectedIndex = Math.Max(0, iSelect);

			m_numHistoryMaxItems.Minimum = 0;
			m_numHistoryMaxItems.Maximum = int.MaxValue;
			bool bHistMaxItems = (m_pwDatabase.HistoryMaxItems >= 0);
			m_numHistoryMaxItems.Value = (bHistMaxItems ? m_pwDatabase.HistoryMaxItems :
				PwDatabase.DefaultHistoryMaxItems);
			m_cbHistoryMaxItems.Checked = bHistMaxItems;

			m_numHistoryMaxSize.Minimum = 0;
			m_numHistoryMaxSize.Maximum = long.MaxValue / (1024 * 1024);
			bool bHistMaxSize = (m_pwDatabase.HistoryMaxSize >= 0);
			m_numHistoryMaxSize.Value = (bHistMaxSize ? m_pwDatabase.HistoryMaxSize :
				PwDatabase.DefaultHistoryMaxSize) / (1024 * 1024);
			m_cbHistoryMaxSize.Checked = bHistMaxSize;

			m_numKeyRecDays.Minimum = 0;
			m_numKeyRecDays.Maximum = long.MaxValue;
			bool bChangeRec = (m_pwDatabase.MasterKeyChangeRec >= 0);
			m_numKeyRecDays.Value = (bChangeRec ? m_pwDatabase.MasterKeyChangeRec : 182);
			m_cbKeyRec.Checked = bChangeRec;

			m_numKeyForceDays.Minimum = 0;
			m_numKeyForceDays.Maximum = long.MaxValue;
			bool bChangeForce = (m_pwDatabase.MasterKeyChangeForce >= 0);
			m_numKeyForceDays.Value = (bChangeForce ? m_pwDatabase.MasterKeyChangeForce : 365);
			m_cbKeyForce.Checked = bChangeForce;
		}

		private void EnableControlsEx()
		{
			if(m_bInitializing) return;

			m_btnColor.Enabled = m_cbColor.Checked;

			m_numHistoryMaxItems.Enabled = m_cbHistoryMaxItems.Checked;
			m_numHistoryMaxSize.Enabled = m_cbHistoryMaxSize.Checked;

			bool bEnableDays = ((Program.Config.UI.UIFlags &
				(ulong)AceUIFlags.DisableKeyChangeDays) == 0);
			m_numKeyRecDays.Enabled = (bEnableDays && m_cbKeyRec.Checked);
			m_numKeyForceDays.Enabled = (bEnableDays && m_cbKeyForce.Checked);
			m_cbKeyRec.Enabled = bEnableDays;
			m_cbKeyForce.Enabled = bEnableDays;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(!m_tbDbName.Text.Equals(m_pwDatabase.Name))
			{
				m_pwDatabase.Name = m_tbDbName.Text;
				m_pwDatabase.NameChanged = DateTime.Now;
			}

			string strNew = m_tbDbDesc.Text;
			string strOrgFlt = StrUtil.NormalizeNewLines(m_pwDatabase.Description, false);
			string strNewFlt = StrUtil.NormalizeNewLines(strNew, false);
			if(!strNewFlt.Equals(strOrgFlt))
			{
				m_pwDatabase.Description = strNew;
				m_pwDatabase.DescriptionChanged = DateTime.Now;
			}

			if(!m_tbDefaultUser.Text.Equals(m_pwDatabase.DefaultUserName))
			{
				m_pwDatabase.DefaultUserName = m_tbDefaultUser.Text;
				m_pwDatabase.DefaultUserNameChanged = DateTime.Now;
			}

			if(!m_cbColor.Checked) m_pwDatabase.Color = Color.Empty;
			else m_pwDatabase.Color = m_clr;

			int nCipher = CipherPool.GlobalPool.GetCipherIndex(m_cmbEncAlgo.Text);
			Debug.Assert(nCipher >= 0);
			if(nCipher >= 0)
				m_pwDatabase.DataCipherUuid = CipherPool.GlobalPool[nCipher].CipherUuid;
			else
				m_pwDatabase.DataCipherUuid = StandardAesEngine.AesUuid;

			m_pwDatabase.KeyEncryptionRounds = (ulong)m_numEncRounds.Value;

			if(m_rbNone.Checked) m_pwDatabase.Compression = PwCompressionAlgorithm.None;
			else if(m_rbGZip.Checked) m_pwDatabase.Compression = PwCompressionAlgorithm.GZip;
			else { Debug.Assert(false); }

			// m_pwDatabase.MemoryProtection.ProtectTitle = UpdateMemoryProtection(0,
			//	m_pwDatabase.MemoryProtection.ProtectTitle, PwDefs.TitleField);
			// m_pwDatabase.MemoryProtection.ProtectUserName = UpdateMemoryProtection(1,
			//	m_pwDatabase.MemoryProtection.ProtectUserName, PwDefs.UserNameField);
			// m_pwDatabase.MemoryProtection.ProtectPassword = UpdateMemoryProtection(2,
			//	m_pwDatabase.MemoryProtection.ProtectPassword, PwDefs.PasswordField);
			// m_pwDatabase.MemoryProtection.ProtectUrl = UpdateMemoryProtection(3,
			//	m_pwDatabase.MemoryProtection.ProtectUrl, PwDefs.UrlField);
			// m_pwDatabase.MemoryProtection.ProtectNotes = UpdateMemoryProtection(4,
			//	m_pwDatabase.MemoryProtection.ProtectNotes, PwDefs.NotesField);

			// m_pwDatabase.MemoryProtection.AutoEnableVisualHiding = m_cbAutoEnableHiding.Checked;

			if(m_cbRecycleBin.Checked != m_pwDatabase.RecycleBinEnabled)
			{
				m_pwDatabase.RecycleBinEnabled = m_cbRecycleBin.Checked;
				m_pwDatabase.RecycleBinChanged = DateTime.Now;
			}
			int iRecBinSel = m_cmbRecycleBin.SelectedIndex;
			if(m_dictRecycleBinGroups.ContainsKey(iRecBinSel))
			{
				if(!m_dictRecycleBinGroups[iRecBinSel].Equals(m_pwDatabase.RecycleBinUuid))
				{
					m_pwDatabase.RecycleBinUuid = m_dictRecycleBinGroups[iRecBinSel];
					m_pwDatabase.RecycleBinChanged = DateTime.Now;
				}
			}
			else
			{
				Debug.Assert(false);
				if(!PwUuid.Zero.Equals(m_pwDatabase.RecycleBinUuid))
				{
					m_pwDatabase.RecycleBinUuid = PwUuid.Zero;
					m_pwDatabase.RecycleBinChanged = DateTime.Now;
				}
			}

			int iTemplSel = m_cmbEntryTemplates.SelectedIndex;
			if(m_dictEntryTemplateGroups.ContainsKey(iTemplSel))
			{
				if(!m_dictEntryTemplateGroups[iTemplSel].Equals(m_pwDatabase.EntryTemplatesGroup))
				{
					m_pwDatabase.EntryTemplatesGroup = m_dictEntryTemplateGroups[iTemplSel];
					m_pwDatabase.EntryTemplatesGroupChanged = DateTime.Now;
				}
			}
			else
			{
				Debug.Assert(false);
				if(!PwUuid.Zero.Equals(m_pwDatabase.EntryTemplatesGroup))
				{
					m_pwDatabase.EntryTemplatesGroup = PwUuid.Zero;
					m_pwDatabase.EntryTemplatesGroupChanged = DateTime.Now;
				}
			}

			if(!m_cbHistoryMaxItems.Checked) m_pwDatabase.HistoryMaxItems = -1;
			else m_pwDatabase.HistoryMaxItems = (int)m_numHistoryMaxItems.Value;

			if(!m_cbHistoryMaxSize.Checked) m_pwDatabase.HistoryMaxSize = -1;
			else m_pwDatabase.HistoryMaxSize = (long)m_numHistoryMaxSize.Value * 1024 * 1024;

			m_pwDatabase.MaintainBackups(); // Apply new history settings

			if(!m_cbKeyRec.Checked) m_pwDatabase.MasterKeyChangeRec = -1;
			else m_pwDatabase.MasterKeyChangeRec = (long)m_numKeyRecDays.Value;

			if(!m_cbKeyForce.Checked) m_pwDatabase.MasterKeyChangeForce = -1;
			else m_pwDatabase.MasterKeyChangeForce = (long)m_numKeyForceDays.Value;
		}

		// private bool UpdateMemoryProtection(int nIndex, bool bOldSetting,
		//	string strFieldID)
		// {
		//	bool bNewProt = m_lbMemProt.GetItemChecked(nIndex);
		//	if(bNewProt != bOldSetting)
		//		m_pwDatabase.RootGroup.EnableStringFieldProtection(strFieldID, bNewProt);
		// #if DEBUG
		//	EntryHandler eh = delegate(PwEntry pe)
		//	{
		//		ProtectedString ps = pe.Strings.Get(strFieldID);
		//		if(ps != null) { Debug.Assert(ps.IsProtected == bNewProt); }
		//		return true;
		//	};
		//	Debug.Assert(m_pwDatabase.RootGroup.TraverseTree(
		//		TraversalMethod.PreOrder, null, eh));
		// #endif
		//	return bNewProt;
		// }

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			string strSubTopic = null;
			if(m_tabMain.SelectedTab == m_tabGeneral)
				strSubTopic = AppDefs.HelpTopics.DbSettingsGeneral;
			else if(m_tabMain.SelectedTab == m_tabSecurity)
				strSubTopic = AppDefs.HelpTopics.DbSettingsSecurity;
			// else if(m_tabMain.SelectedTab == m_tabProtection)
			//	strSubTopic = AppDefs.HelpTopics.DbSettingsProtection;
			else if(m_tabMain.SelectedTab == m_tabCompression)
				strSubTopic = AppDefs.HelpTopics.DbSettingsCompression;

			AppHelp.ShowHelp(AppDefs.HelpTopics.DatabaseSettings, strSubTopic);
		}

		private void OnLinkClicked1SecondDelayRounds(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
				m_numEncRounds.Value = CompositeKey.TransformKeyBenchmark(1000, 3001);
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);

			foreach(ColorMenuItem mi in m_vColorItems)
				mi.Click -= this.HandleColorButtonClicked;
			m_vColorItems.Clear();
		}

		private void OnKeyRecCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnKeyForceCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		// private void OnLinkClickedMemProtHelp(object sender, LinkLabelLinkClickedEventArgs e)
		// {
		//	AppHelp.ShowHelp(AppDefs.HelpTopics.FaqTech, AppDefs.HelpTopics.FaqTechMemProt);
		// }

		private void OnHistoryMaxItemsCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnHistoryMaxSizeCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void HandleColorButtonClicked(object sender, EventArgs e)
		{
			if(sender == null) { Debug.Assert(false); return; }
			ColorMenuItem mi = (sender as ColorMenuItem);
			if(mi == null) { Debug.Assert(false); return; }

			m_clr = mi.Color;
			UIUtil.SetButtonImage(m_btnColor, PwEntryForm.CreateColorButtonImage(
				m_btnColor, m_clr), false);
		}

		private void OnBtnColor(object sender, EventArgs e)
		{
			// Color? clr = UIUtil.ShowColorDialog(m_clr);
			// if(clr.HasValue)
			// {
			//	float h, s, v;
			//	UIUtil.ColorToHsv(clr.Value, out h, out s, out v);
			//	m_clr = UIUtil.ColorFromHsv(h, 1.0f, 1.0f);
			//	UIUtil.SetButtonImage(m_btnColor, PwEntryForm.CreateColorButtonImage(
			//		m_btnColor, m_clr), false);
			// }

			if(m_ctxColor == null)
			{
				m_ctxColor = new ContextMenu();

				int qSize = (int)((20.0f * m_btnColor.Height) / 23.0f + 0.01f);

				const int nMaxColors = 64;
				int nBreakAt = (int)Math.Sqrt(0.1 + nMaxColors);

				// m_ctxColor.LayoutStyle = ToolStripLayoutStyle.Flow;
				// FlowLayoutSettings fls = (m_ctxColor.LayoutSettings as FlowLayoutSettings);
				// if(fls == null) { Debug.Assert(false); return; }
				// fls.FlowDirection = FlowDirection.LeftToRight;

				// m_ctxColor.LayoutStyle = ToolStripLayoutStyle.Table;
				// TableLayoutSettings tls = (m_ctxColor.LayoutSettings as TableLayoutSettings);
				// if(tls == null) { Debug.Assert(false); return; }
				// tls.ColumnCount = nBreakAt;
				// tls.RowCount = nBreakAt;

				// m_ctxColor.SuspendLayout();

				for(int i = 0; i < nMaxColors; ++i)
				{
					float fHue = ((float)i * 360.0f) / (float)nMaxColors;
					Color clr = UIUtil.ColorFromHsv(fHue, 1.0f, 1.0f);

					// Image img = UIUtil.CreateColorBitmap24(16, 16, clr);
					// ToolStripButton btn = new ToolStripButton(string.Empty, img);
					// btn.DisplayStyle = ToolStripItemDisplayStyle.Image;
					// btn.ImageAlign = ContentAlignment.MiddleCenter;
					// btn.AutoSize = true;

					ColorMenuItem mi = new ColorMenuItem(clr, qSize);

					if((i > 0) && ((i % nBreakAt) == 0))
						mi.Break = true;
					//	fls.SetFlowBreak(btn, true);

					mi.Click += this.HandleColorButtonClicked;

					// m_ctxColor.Items.Add(btn);
					m_vColorItems.Add(mi);
				}

				m_ctxColor.MenuItems.AddRange(m_vColorItems.ToArray());

				// m_ctxColor.ResumeLayout(true);
				// this.Controls.Add(m_ctxColor);
				// m_ctxColor.BringToFront();
			}

			// m_ctxColor.Show(m_btnColor, new Point(0, m_btnColor.Height));
			// m_ctxColor.Location = new Point(m_btnColor.Location.X,
			//	m_btnColor.Location.Y - m_btnColor.Height - m_ctxColor.Height);
			// m_ctxColor.Visible = true;
			// m_ctxColor.Show();

			m_ctxColor.Show(m_btnColor, new Point(0, m_btnColor.Height));
		}

		private void OnColorCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}
	}
}