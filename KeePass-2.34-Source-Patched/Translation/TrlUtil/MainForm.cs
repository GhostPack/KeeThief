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
using System.Reflection;

using KeePass.App;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;
using KeePass.Util.XmlSerialization;

using KeePassLib;
using KeePassLib.Resources;
using KeePassLib.Serialization;
using KeePassLib.Translation;
using KeePassLib.Utility;

namespace TrlUtil
{
	public partial class MainForm : Form
	{
		private const string TrlUtilName = "TrlUtil";

		private KPTranslation m_trl = new KPTranslation();
		private string m_strFile = string.Empty;

		private ImageList m_ilStr = new ImageList();

		private const string m_strFileFilter = "KeePass Translation (*.lngx)|*.lngx|All Files (*.*)|*.*";
		private static readonly string[] m_vEmpty = new string[2] {
			@"<DYN>", @"<>" };

		private KPControlCustomization m_kpccLast = null;

		private const int m_inxWindow = 6;
		private const int m_inxMissing = 1;
		private const int m_inxOk = 4;
		private const int m_inxWarning = 5;

		private bool m_bModified = false;

		private PreviewForm m_prev = new PreviewForm();

		private delegate void ImportFn(KPTranslation trlInto, IOConnectionInfo ioc);

		public MainForm()
		{
			InitializeComponent();
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			this.Text += " " + PwDefs.VersionString;

			m_trl.Forms = FormTrlMgr.CreateListOfCurrentVersion();
			m_rtbUnusedText.SimpleTextOnly = true;

			string strSearchTr = ((WinUtil.IsAtLeastWindowsVista ?
				string.Empty : " ") + "Search in active tab...");
			UIUtil.SetCueBanner(m_tbFind, strSearchTr);

			this.CreateStringTableUI();
			this.UpdateControlTree();

			if(m_tvControls.SelectedNode == null)
				m_tvControls.SelectedNode = m_tvControls.Nodes[0];
			UpdatePreviewForm();
			m_prev.Show();

			try { this.DoubleBuffered = true; }
			catch(Exception) { Debug.Assert(false); }

			UpdateUIState();
		}

		private void CreateStringTableUI()
		{
			int nWidth = m_lvStrings.ClientSize.Width - 20;

			m_ilStr.ColorDepth = ColorDepth.Depth32Bit;
			m_ilStr.ImageSize = new Size(16, 16);
			m_ilStr.Images.Add(Properties.Resources.B16x16_Binary);
			m_ilStr.Images.Add(Properties.Resources.B16x16_KRec_Record);
			m_ilStr.Images.Add(Properties.Resources.B16x16_LedGreen);
			m_ilStr.Images.Add(Properties.Resources.B16x16_LedLightBlue);
			m_ilStr.Images.Add(Properties.Resources.B16x16_LedLightGreen);
			m_ilStr.Images.Add(Properties.Resources.B16x16_LedOrange);
			m_ilStr.Images.Add(Properties.Resources.B16x16_View_Remove);

			m_lvStrings.SmallImageList = m_ilStr;
			m_tvControls.ImageList = m_ilStr;

			m_lvStrings.Columns.Add("ID", nWidth / 5);
			m_lvStrings.Columns.Add("English", (nWidth * 2) / 5);
			m_lvStrings.Columns.Add("Translated", (nWidth * 2) / 5);

			m_trl.StringTables.Clear();
			KPStringTable kpstP = new KPStringTable();
			kpstP.Name = "KeePass.Resources.KPRes";
			m_trl.StringTables.Add(kpstP);
			KPStringTable kpstL = new KPStringTable();
			kpstL.Name = "KeePassLib.Resources.KLRes";
			m_trl.StringTables.Add(kpstL);
			KPStringTable kpstM = new KPStringTable();
			kpstM.Name = "KeePass.Forms.MainForm.m_menuMain";
			m_trl.StringTables.Add(kpstM);
			KPStringTable kpstE = new KPStringTable();
			kpstE.Name = "KeePass.Forms.MainForm.m_ctxPwList";
			m_trl.StringTables.Add(kpstE);
			KPStringTable kpstG = new KPStringTable();
			kpstG.Name = "KeePass.Forms.MainForm.m_ctxGroupList";
			m_trl.StringTables.Add(kpstG);
			KPStringTable kpstT = new KPStringTable();
			kpstT.Name = "KeePass.Forms.MainForm.m_ctxTray";
			m_trl.StringTables.Add(kpstT);
			KPStringTable kpstET = new KPStringTable();
			kpstET.Name = "KeePass.Forms.PwEntryForm.m_ctxTools";
			m_trl.StringTables.Add(kpstET);
			KPStringTable kpstDT = new KPStringTable();
			kpstDT.Name = "KeePass.Forms.PwEntryForm.m_ctxDefaultTimes";
			m_trl.StringTables.Add(kpstDT);
			KPStringTable kpstLO = new KPStringTable();
			kpstLO.Name = "KeePass.Forms.PwEntryForm.m_ctxListOperations";
			m_trl.StringTables.Add(kpstLO);
			KPStringTable kpstPG = new KPStringTable();
			kpstPG.Name = "KeePass.Forms.PwEntryForm.m_ctxPwGen";
			m_trl.StringTables.Add(kpstPG);
			KPStringTable kpstSM = new KPStringTable();
			kpstSM.Name = "KeePass.Forms.PwEntryForm.m_ctxStrMoveToStandard";
			m_trl.StringTables.Add(kpstSM);
			KPStringTable kpstBA = new KPStringTable();
			kpstBA.Name = "KeePass.Forms.PwEntryForm.m_ctxBinAttach";
			m_trl.StringTables.Add(kpstBA);
			KPStringTable kpstTT = new KPStringTable();
			kpstTT.Name = "KeePass.Forms.EcasTriggersForm.m_ctxTools";
			m_trl.StringTables.Add(kpstTT);
			KPStringTable kpstDE = new KPStringTable();
			kpstDE.Name = "KeePass.Forms.DataEditorForm.m_menuMain";
			m_trl.StringTables.Add(kpstDE);
			KPStringTable kpstSD = new KPStringTable();
			kpstSD.Name = "KeePassLib.Resources.KSRes";
			m_trl.StringTables.Add(kpstSD);

			Type tKP = typeof(KPRes);
			ListViewGroup lvg = new ListViewGroup("KeePass Strings");
			m_lvStrings.Groups.Add(lvg);
			foreach(string strKey in KPRes.GetKeyNames())
			{
				PropertyInfo pi = tKP.GetProperty(strKey);
				MethodInfo mi = pi.GetGetMethod();
				if(mi.ReturnType != typeof(string))
				{
					MessageBox.Show(this, "Return type is not string:\r\n" +
						strKey, TrlUtilName + ": Fatal Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				string strEng = (mi.Invoke(null, null) as string);
				if(strEng == null)
				{
					MessageBox.Show(this, "English string is null:\r\n" +
						strKey, TrlUtilName + ": Fatal Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				KPStringTableItem kpstItem = new KPStringTableItem();
				kpstItem.Name = strKey;
				kpstItem.ValueEnglish = strEng;
				kpstP.Strings.Add(kpstItem);

				ListViewItem lvi = new ListViewItem();
				lvi.Group = lvg;
				lvi.Text = strKey;
				lvi.SubItems.Add(strEng);
				lvi.SubItems.Add(string.Empty);
				lvi.Tag = kpstItem;
				lvi.ImageIndex = 0;
				m_lvStrings.Items.Add(lvi);
			}

			Type tKL = typeof(KLRes);
			lvg = new ListViewGroup("KeePass Library Strings");
			m_lvStrings.Groups.Add(lvg);
			foreach(string strLibKey in KLRes.GetKeyNames())
			{
				PropertyInfo pi = tKL.GetProperty(strLibKey);
				MethodInfo mi = pi.GetGetMethod();
				if(mi.ReturnType != typeof(string))
				{
					MessageBox.Show(this, "Return type is not string:\r\n" +
						strLibKey, TrlUtilName + ": Fatal Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				string strEng = (mi.Invoke(null, null) as string);
				if(strEng == null)
				{
					MessageBox.Show(this, "English string is null:\r\n" +
						strLibKey, TrlUtilName + ": Fatal Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				KPStringTableItem kpstItem = new KPStringTableItem();
				kpstItem.Name = strLibKey;
				kpstItem.ValueEnglish = strEng;
				kpstL.Strings.Add(kpstItem);

				ListViewItem lvi = new ListViewItem();
				lvi.Group = lvg;
				lvi.Text = strLibKey;
				lvi.SubItems.Add(strEng);
				lvi.SubItems.Add(string.Empty);
				lvi.Tag = kpstItem;
				lvi.ImageIndex = 0;
				m_lvStrings.Items.Add(lvi);
			}

			lvg = new ListViewGroup("Main Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			KeePass.Forms.MainForm mf = new KeePass.Forms.MainForm();
			TrlAddMenuCommands(kpstM, lvg, mf.MainMenu.Items);

			lvg = new ListViewGroup("Entry Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstE, lvg, mf.EntryContextMenu.Items);

			lvg = new ListViewGroup("Group Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstG, lvg, mf.GroupContextMenu.Items);

			lvg = new ListViewGroup("System Tray Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstT, lvg, mf.TrayContextMenu.Items);

			KeePass.Forms.PwEntryForm ef = new KeePass.Forms.PwEntryForm();
			lvg = new ListViewGroup("Entry Tools Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstET, lvg, ef.ToolsContextMenu.Items);

			lvg = new ListViewGroup("Default Times Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstDT, lvg, ef.DefaultTimesContextMenu.Items);

			lvg = new ListViewGroup("List Operations Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstLO, lvg, ef.ListOperationsContextMenu.Items);

			lvg = new ListViewGroup("Password Generator Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstPG, lvg, ef.PasswordGeneratorContextMenu.Items);

			KeePass.Forms.EcasTriggersForm tf = new KeePass.Forms.EcasTriggersForm();
			lvg = new ListViewGroup("Ecas Trigger Tools Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstTT, lvg, tf.ToolsContextMenu.Items);

			KeePass.Forms.DataEditorForm df = new KeePass.Forms.DataEditorForm();
			lvg = new ListViewGroup("Data Editor Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstDE, lvg, df.MainMenuEx.Items);

			lvg = new ListViewGroup("Standard String Movement Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstSM, lvg, ef.StandardStringMovementContextMenu.Items);

			lvg = new ListViewGroup("Entry Attachments Context Menu Commands");
			m_lvStrings.Groups.Add(lvg);
			TrlAddMenuCommands(kpstBA, lvg, ef.AttachmentsContextMenu.Items);

			Type tSD = typeof(KSRes);
			lvg = new ListViewGroup("KeePassLibSD Strings");
			m_lvStrings.Groups.Add(lvg);
			foreach(string strLibSDKey in KSRes.GetKeyNames())
			{
				PropertyInfo pi = tSD.GetProperty(strLibSDKey);
				MethodInfo mi = pi.GetGetMethod();
				if(mi.ReturnType != typeof(string))
				{
					MessageBox.Show(this, "Return type is not string:\r\n" +
						strLibSDKey, TrlUtilName + ": Fatal Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				string strEng = (mi.Invoke(null, null) as string);
				if(strEng == null)
				{
					MessageBox.Show(this, "English string is null:\r\n" +
						strLibSDKey, TrlUtilName + ": Fatal Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				KPStringTableItem kpstItem = new KPStringTableItem();
				kpstItem.Name = strLibSDKey;
				kpstItem.ValueEnglish = strEng;
				kpstL.Strings.Add(kpstItem);

				ListViewItem lvi = new ListViewItem();
				lvi.Group = lvg;
				lvi.Text = strLibSDKey;
				lvi.SubItems.Add(strEng);
				lvi.SubItems.Add(string.Empty);
				lvi.Tag = kpstItem;
				lvi.ImageIndex = 0;
				m_lvStrings.Items.Add(lvi);
			}
		}

		private void TrlAddMenuCommands(KPStringTable kpst, ListViewGroup grp,
			ToolStripItemCollection tsic)
		{
			foreach(ToolStripItem tsi in tsic)
			{
				if(tsi.Text.Length == 0) continue;
				if(tsi.Text.StartsWith(@"<") && tsi.Text.EndsWith(@">")) continue;

				KPStringTableItem kpstItem = new KPStringTableItem();
				kpstItem.Name = tsi.Name;
				kpstItem.ValueEnglish = tsi.Text;
				kpst.Strings.Add(kpstItem);

				ListViewItem lvi = new ListViewItem();
				lvi.Group = grp;
				lvi.Text = tsi.Name;
				lvi.SubItems.Add(tsi.Text);
				lvi.SubItems.Add(string.Empty);
				lvi.Tag = kpstItem;
				lvi.ImageIndex = 0;
				m_lvStrings.Items.Add(lvi);

				ToolStripMenuItem tsmi = (tsi as ToolStripMenuItem);
				if(tsmi != null) TrlAddMenuCommands(kpst, grp, tsmi.DropDownItems);
			}
		}

		private void UpdateStringTableUI()
		{
			foreach(ListViewItem lvi in m_lvStrings.Items)
			{
				KPStringTableItem kpstItem = (lvi.Tag as KPStringTableItem);
				Debug.Assert(kpstItem != null);
				if(kpstItem == null) continue;

				lvi.SubItems[2].Text = kpstItem.Value;
			}
		}

		private void UpdateControlTree()
		{
			FormTrlMgr.RenderToTreeControl(m_trl.Forms, m_tvControls);
			UpdateStatusImages(null);
		}

		private void UpdateUIState()
		{
			bool bTrlTab = ((m_tabMain.SelectedTab == m_tabStrings) ||
				(m_tabMain.SelectedTab == m_tabDialogs));
			m_menuEditNextUntrl.Enabled = bTrlTab;
			m_tbNextUntrl.Enabled = bTrlTab;
			m_tbFind.Enabled = bTrlTab;
		}

		private void OnLinkLangCodeClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start(@"http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes");
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, TrlUtilName,
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void OnFileOpen(object sender, EventArgs e)
		{
			OpenFileDialogEx ofd = UIUtil.CreateOpenFileDialog("Open KeePass Translation",
				m_strFileFilter, 1, null, false, AppDefs.FileDialogContext.Attachments);

			if(ofd.ShowDialog() != DialogResult.OK) return;

			KPTranslation kpTrl = null;
			try
			{
				XmlSerializerEx xs = new XmlSerializerEx(typeof(KPTranslation));
				kpTrl = KPTranslation.Load(ofd.FileName, xs);
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, TrlUtilName, MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				return;
			}

			m_strFile = ofd.FileName;

			StringBuilder sbUnusedText = new StringBuilder();
			if(kpTrl.UnusedText.Length > 0)
			{
				if(kpTrl.UnusedText.EndsWith("\r") || kpTrl.UnusedText.EndsWith("\n"))
					sbUnusedText.Append(kpTrl.UnusedText);
				else sbUnusedText.AppendLine(kpTrl.UnusedText);
			}

			m_trl.Properties = kpTrl.Properties;
			foreach(KPStringTable kpstNew in kpTrl.StringTables)
			{
				foreach(KPStringTable kpstInto in m_trl.StringTables)
				{
					if(kpstInto.Name == kpstNew.Name)
						MergeInStringTable(kpstInto, kpstNew, sbUnusedText);
				}
			}

			FormTrlMgr.MergeForms(m_trl.Forms, kpTrl.Forms, sbUnusedText);

			m_tbNameEng.Text = m_trl.Properties.NameEnglish;
			m_tbNameLcl.Text = m_trl.Properties.NameNative;
			m_tbLangID.Text = m_trl.Properties.Iso6391Code;
			m_tbAuthorName.Text = m_trl.Properties.AuthorName;
			m_tbAuthorContact.Text = m_trl.Properties.AuthorContact;
			m_cbRtl.Checked = m_trl.Properties.RightToLeft;

			m_rtbUnusedText.Text = sbUnusedText.ToString();

			this.UpdateStringTableUI();
			this.UpdateStatusImages(null);
			this.UpdatePreviewForm();
		}

		private void MergeInStringTable(KPStringTable tbInto, KPStringTable tbSource,
			StringBuilder sbUnusedText)
		{
			foreach(KPStringTableItem kpSrc in tbSource.Strings)
			{
				bool bHasAssigned = false;
				foreach(KPStringTableItem kpDst in tbInto.Strings)
				{
					if(kpSrc.Name == kpDst.Name)
					{
						if(kpSrc.Value.Length > 0)
						{
							kpDst.Value = kpSrc.Value;
							bHasAssigned = true;
						}
					}
				}

				if(!bHasAssigned)
				{
					string strTrimmed = kpSrc.Value.Trim();
					if(strTrimmed.Length > 0) sbUnusedText.AppendLine(strTrimmed);
				}
			}
		}

		private void UpdateInternalTranslation()
		{
			m_trl.Properties.NameEnglish = StrUtil.SafeXmlString(m_tbNameEng.Text);
			m_trl.Properties.NameNative = StrUtil.SafeXmlString(m_tbNameLcl.Text);
			m_trl.Properties.Iso6391Code = StrUtil.SafeXmlString(m_tbLangID.Text);
			m_trl.Properties.AuthorName = StrUtil.SafeXmlString(m_tbAuthorName.Text);
			m_trl.Properties.AuthorContact = StrUtil.SafeXmlString(m_tbAuthorContact.Text);
			m_trl.Properties.RightToLeft = m_cbRtl.Checked;
		}

		private void UpdateStatusImages(TreeNodeCollection vtn)
		{
			if(vtn == null) vtn = m_tvControls.Nodes;

			foreach(TreeNode tn in vtn)
			{
				KPFormCustomization kpfc = (tn.Tag as KPFormCustomization);
				KPControlCustomization kpcc = (tn.Tag as KPControlCustomization);

				if(kpfc != null)
				{
					tn.ImageIndex = m_inxWindow;
					tn.SelectedImageIndex = m_inxWindow;
				}
				else if(kpcc != null)
				{
					int iCurrentImage = tn.ImageIndex, iNewImage;

					if(Array.IndexOf<string>(m_vEmpty, kpcc.TextEnglish) >= 0)
						iNewImage = ((kpcc.Text.Length == 0) ? m_inxOk : m_inxWarning);
					else if((kpcc.TextEnglish.Length > 0) && (kpcc.Text.Length > 0))
						iNewImage = m_inxOk;
					else if((kpcc.TextEnglish.Length > 0) && (kpcc.Text.Length == 0))
						iNewImage = m_inxMissing;
					else if((kpcc.TextEnglish.Length == 0) && (kpcc.Text.Length == 0))
						iNewImage = m_inxOk;
					else if((kpcc.TextEnglish.Length == 0) && (kpcc.Text.Length > 0))
						iNewImage = m_inxWarning;
					else
						iNewImage = m_inxWarning;

					if(iNewImage != iCurrentImage)
					{
						tn.ImageIndex = iNewImage;
						tn.SelectedImageIndex = iNewImage;
					}
				}
				else { Debug.Assert(false); }

				if(tn.Nodes != null) UpdateStatusImages(tn.Nodes);
			}
		}

		private void OnFileSave(object sender, EventArgs e)
		{
			UpdateInternalTranslation();

			if(m_strFile.Length == 0)
			{
				OnFileSaveAs(sender, e);
				return;
			}

			PrepareSave();

			try
			{
				XmlSerializerEx xs = new XmlSerializerEx(typeof(KPTranslation));
				KPTranslation.Save(m_trl, m_strFile, xs);
				m_bModified = false;
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, TrlUtilName, MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}
		}

		private void PrepareSave()
		{
			m_trl.Properties.Application = PwDefs.ProductName;
			m_trl.Properties.ApplicationVersion = PwDefs.VersionString;
			m_trl.Properties.Generator = TrlUtilName;

			PwUuid pwUuid = new PwUuid(true);
			m_trl.Properties.FileUuid = pwUuid.ToHexString();

			m_trl.Properties.LastModified = DateTime.Now.ToString("u");

			m_trl.UnusedText = m_rtbUnusedText.Text;

			try { Validate3Dots(); }
			catch(Exception) { Debug.Assert(false); }

			try
			{
				string strAccel = AccelKeysCheck.Validate(m_trl);
				if(strAccel != null)
				{
					MessageBox.Show(this, "Warning! The following accelerator keys collide:" +
						MessageService.NewParagraph + strAccel + MessageService.NewParagraph +
						"Click [OK] to continue saving.", TrlUtilName, MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private void Validate3Dots()
		{
			if(m_trl.Properties.RightToLeft) return; // Check doesn't support RTL

			foreach(KPStringTable kpst in m_trl.StringTables)
			{
				foreach(KPStringTableItem kpi in kpst.Strings)
				{
					string strEn = kpi.ValueEnglish;
					string strTrl = kpi.Value;
					if(string.IsNullOrEmpty(strEn) || string.IsNullOrEmpty(strTrl)) continue;

					bool bEllEn = (strEn.EndsWith(@"...") || strEn.EndsWith(@"…"));
					bool bEllTrl = (strTrl.EndsWith(@"...") || strTrl.EndsWith(@"…"));

					if(bEllEn && !bEllTrl)
						MessageBox.Show(this, "Warning! The English string" +
							MessageService.NewParagraph + strEn + MessageService.NewParagraph +
							"ends with 3 dots, but the translated string does not:" +
							MessageService.NewParagraph + strTrl, TrlUtilName,
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		private void OnFileExit(object sender, EventArgs e)
		{
			if(m_bModified) OnFileSaveAs(sender, e);

			this.Close();
		}

		private void OnStringsSelectedIndexChanged(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic =
				m_lvStrings.SelectedItems;
			if(lvsic.Count != 1)
			{
				m_tbStrEng.Text = string.Empty;
				m_tbStrTrl.Text = string.Empty;
				return;
			}

			KPStringTableItem kpstItem = (lvsic[0].Tag as KPStringTableItem);
			Debug.Assert(kpstItem != null);
			if(kpstItem == null) return;

			UIUtil.SetMultilineText(m_tbStrEng, lvsic[0].SubItems[1].Text);
			m_tbStrTrl.Text = lvsic[0].SubItems[2].Text;
		}

		private void OnStrKeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
			{
				UIUtil.SetHandled(e, true);

				ListView.SelectedListViewItemCollection lvsic =
					m_lvStrings.SelectedItems;
				if(lvsic.Count != 1) return;

				KPStringTableItem kpstItem = (lvsic[0].Tag as KPStringTableItem);
				if(kpstItem == null)
				{
					Debug.Assert(false);
					return;
				}

				kpstItem.Value = StrUtil.SafeXmlString(m_tbStrTrl.Text);
				this.UpdateStringTableUI();

				int iIndex = lvsic[0].Index;
				if(iIndex < m_lvStrings.Items.Count - 1)
				{
					lvsic[0].Selected = false;
					UIUtil.SetFocusedItem(m_lvStrings, m_lvStrings.Items[
						iIndex + 1], true);
				}

				m_bModified = true;
			}
		}

		private void OnStrKeyUp(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
				UIUtil.SetHandled(e, true);
		}

		private void OnFileSaveAs(object sender, EventArgs e)
		{
			SaveFileDialogEx sfd = UIUtil.CreateSaveFileDialog("Save KeePass Translation",
				m_tbNameEng.Text + ".lngx", m_strFileFilter, 1, "lngx",
				AppDefs.FileDialogContext.Attachments);

			if(sfd.ShowDialog() != DialogResult.OK) return;

			m_strFile = sfd.FileName;

			OnFileSave(sender, e);
		}

		private void OnStrDoubleClick(object sender, EventArgs e)
		{
			UIUtil.SetFocus(m_tbStrTrl, this);
		}

		private void OnCustomControlsAfterSelect(object sender, TreeViewEventArgs e)
		{
			ShowCustomControlProps(e.Node.Tag as KPControlCustomization);
			UpdatePreviewForm();
		}

		private void ShowCustomControlProps(KPControlCustomization kpcc)
		{
			if(kpcc == null) return; // No assert

			m_kpccLast = kpcc;

			UIUtil.SetMultilineText(m_tbCtrlEngText, m_kpccLast.TextEnglish);
			m_tbCtrlTrlText.Text = m_kpccLast.Text;

			m_tbLayoutX.Text = KpccLayout.ToControlRelativeString(m_kpccLast.Layout.X);
			m_tbLayoutY.Text = KpccLayout.ToControlRelativeString(m_kpccLast.Layout.Y);
			m_tbLayoutW.Text = KpccLayout.ToControlRelativeString(m_kpccLast.Layout.Width);
			m_tbLayoutH.Text = KpccLayout.ToControlRelativeString(m_kpccLast.Layout.Height);
		}

		private void OnCtrlTrlTextChanged(object sender, EventArgs e)
		{
			string strText = m_tbCtrlTrlText.Text;
			if((m_kpccLast != null) && (m_kpccLast.Text != strText))
			{
				m_kpccLast.Text = StrUtil.SafeXmlString(m_tbCtrlTrlText.Text);
				m_bModified = true;
			}

			UpdateStatusImages(null);
			UpdatePreviewForm();
		}

		private void OnLayoutXTextChanged(object sender, EventArgs e)
		{
			if(m_kpccLast != null)
			{
				m_kpccLast.Layout.SetControlRelativeValue(
					KpccLayout.LayoutParameterEx.X, m_tbLayoutX.Text);
				m_bModified = true;

				UpdatePreviewForm();
			}
		}

		private void OnLayoutYTextChanged(object sender, EventArgs e)
		{
			if(m_kpccLast != null)
			{
				m_kpccLast.Layout.SetControlRelativeValue(
					KpccLayout.LayoutParameterEx.Y, m_tbLayoutY.Text);
				m_bModified = true;

				UpdatePreviewForm();
			}
		}

		private void OnLayoutWidthTextChanged(object sender, EventArgs e)
		{
			if(m_kpccLast != null)
			{
				m_kpccLast.Layout.SetControlRelativeValue(
					KpccLayout.LayoutParameterEx.Width, m_tbLayoutW.Text);
				m_bModified = true;

				UpdatePreviewForm();
			}
		}

		private void OnLayoutHeightTextChanged(object sender, EventArgs e)
		{
			if(m_kpccLast != null)
			{
				m_kpccLast.Layout.SetControlRelativeValue(
					KpccLayout.LayoutParameterEx.Height, m_tbLayoutH.Text);
				m_bModified = true;

				UpdatePreviewForm();
			}
		}

		private void OnCtrlTrlTextKeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
			{
				UIUtil.SetHandled(e, true);

				TreeNode tn = m_tvControls.SelectedNode;
				if(tn == null) return;

				try
				{
					TreeNode tnNew = tn.NextNode;
					if(tnNew != null) m_tvControls.SelectedNode = tnNew;
				}
				catch(Exception) { Debug.Assert(false); }
			}
		}

		private void OnCtrlTrlTextKeyUp(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
				UIUtil.SetHandled(e, true);
		}

		private void UpdatePreviewForm()
		{
			TreeNode tn = m_tvControls.SelectedNode;
			if(tn == null) return;
			if(tn.Parent != null) tn = tn.Parent;
			string strFormName = tn.Text;

			foreach(KPFormCustomization kpfc in m_trl.Forms)
			{
				if(kpfc.FullName.EndsWith(strFormName))
				{
					UpdatePreviewForm(kpfc);
					break;
				}
			}
		}

		private void UpdatePreviewForm(KPFormCustomization kpfc)
		{
			// bool bResizeEng = (string.IsNullOrEmpty(kpfc.Window.Layout.Width) &&
			//	string.IsNullOrEmpty(kpfc.Window.Layout.Height));

			m_prev.CopyForm(kpfc.FormEnglish);
			kpfc.ApplyTo(m_prev);
		}

		private void OnBtnClearUnusedText(object sender, EventArgs e)
		{
			m_rtbUnusedText.Text = string.Empty;
		}

		private void OnImport1xLng(object sender, EventArgs e)
		{
			PerformImport("lng", "KeePass 1.x LNG File", Import1xLng);
		}

		private static void Import1xLng(KPTranslation trlInto, IOConnectionInfo ioc)
		{
			TrlImport.Import1xLng(trlInto, ioc.Path);
		}

		private void PerformImport(string strFileExt, string strFileDesc, ImportFn f)
		{
			OpenFileDialogEx ofd = UIUtil.CreateOpenFileDialog("Import " + strFileDesc,
				strFileDesc + " (*." + strFileExt + ")|*." + strFileExt +
				"|All Files (*.*)|*.*", 1, strFileExt, false,
				AppDefs.FileDialogContext.Import);

			if(ofd.ShowDialog() != DialogResult.OK) return;

			try { f(m_trl, IOConnectionInfo.FromPath(ofd.FileName)); }
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, TrlUtilName, MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}

			UpdateStringTableUI();
			UpdateControlTree();
			m_tvControls.SelectedNode = m_tvControls.Nodes[0];
			UpdatePreviewForm();
		}

		private void PerformQuickFind()
		{
			string str = m_tbFind.Text;
			if(string.IsNullOrEmpty(str)) return;

			bool bResult = true;
			if(m_tabMain.SelectedTab == m_tabStrings)
				bResult = PerformQuickFindStrings(str);
			else if(m_tabMain.SelectedTab == m_tabDialogs)
				bResult = PerformQuickFindDialogs(str);

			if(!bResult) m_tbFind.BackColor = AppDefs.ColorEditError;
		}

		private bool PerformQuickFindStrings(string strFind)
		{
			int nItems = m_lvStrings.Items.Count;
			if(nItems == 0) return false;

			ListViewItem lviStart = m_lvStrings.FocusedItem;
			int iOffset = ((lviStart != null) ? (lviStart.Index + 1) : 0);

			for(int i = 0; i < nItems; ++i)
			{
				int j = ((iOffset + i) % nItems);
				ListViewItem lvi = m_lvStrings.Items[j];
				foreach(ListViewItem.ListViewSubItem lvsi in lvi.SubItems)
				{
					if(lvsi.Text.IndexOf(strFind, StrUtil.CaseIgnoreCmp) >= 0)
					{
						UIUtil.SetFocusedItem(m_lvStrings, lvi, false);
						m_lvStrings.SelectedItems.Clear();
						lvi.Selected = true;

						m_lvStrings.EnsureVisible(j);
						return true;
					}
				}
			}

			return false;
		}

		private bool PerformQuickFindDialogs(string strFind)
		{
			List<TreeNode> vNodes = new List<TreeNode>();
			List<string> vValues = new List<string>();
			GetControlTreeItems(m_tvControls.Nodes, vNodes, vValues);

			int iOffset = vNodes.IndexOf(m_tvControls.SelectedNode) + 1;

			for(int i = 0; i < vNodes.Count; ++i)
			{
				int j = ((iOffset + i) % vNodes.Count);

				if(vValues[j].IndexOf(strFind, StrUtil.CaseIgnoreCmp) >= 0)
				{
					m_tvControls.SelectedNode = vNodes[j];
					return true;
				}
			}

			return false;
		}

		private void GetControlTreeItems(TreeNodeCollection tnBase,
			List<TreeNode> vNodes, List<string> vValues)
		{
			foreach(TreeNode tn in tnBase)
			{
				KPFormCustomization kpfc = (tn.Tag as KPFormCustomization);
				KPControlCustomization kpcc = (tn.Tag as KPControlCustomization);

				vNodes.Add(tn);
				if(kpfc != null)
					vValues.Add(kpfc.Window.Name + "©" + kpfc.Window.TextEnglish +
						"©" + kpfc.Window.Text);
				else if(kpcc != null)
					vValues.Add(kpcc.Name + "©" + kpcc.TextEnglish + "©" + kpcc.Text);
				else vValues.Add(tn.Text);

				GetControlTreeItems(tn.Nodes, vNodes, vValues);
			}
		}

		private void OnFindKeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
			{
				UIUtil.SetHandled(e, true);
				PerformQuickFind();
				return;
			}
		}

		private void OnFindKeyUp(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
			{
				UIUtil.SetHandled(e, true);
				return;
			}
		}

		private void OnFindTextChanged(object sender, EventArgs e)
		{
			m_tbFind.ResetBackColor();
		}

		private void OnTabMainSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateUIState();
		}

		private void OnImport2xNoChecks(object sender, EventArgs e)
		{
			FormTrlMgr.IgnoreBaseHash = true;
			OnFileOpen(sender, EventArgs.Empty);
			FormTrlMgr.IgnoreBaseHash = false;
		}

		private void OnImportPo(object sender, EventArgs e)
		{
			PerformImport("po", "PO File", ImportPo);
		}

		private static void ImportPo(KPTranslation trlInto, IOConnectionInfo ioc)
		{
			TrlImport.ImportPo(trlInto, ioc.Path);
		}

		private void OnEditNextUntrl(object sender, EventArgs e)
		{
			if(m_tabMain.SelectedTab == m_tabStrings)
			{
				int nItems = m_lvStrings.Items.Count;
				if(nItems == 0) { Debug.Assert(false); return; }

				ListViewItem lviStart = m_lvStrings.FocusedItem;
				int iOffset = ((lviStart != null) ? (lviStart.Index + 1) : 0);

				for(int i = 0; i < nItems; ++i)
				{
					int j = ((iOffset + i) % nItems);
					ListViewItem lvi = m_lvStrings.Items[j];
					KPStringTableItem kpstItem = (lvi.Tag as KPStringTableItem);
					if(kpstItem == null) { Debug.Assert(false); continue; }

					if(string.IsNullOrEmpty(kpstItem.Value) &&
						!string.IsNullOrEmpty(kpstItem.ValueEnglish))
					{
						m_lvStrings.EnsureVisible(j);
						lvi.Selected = true;
						lvi.Focused = true;
						UIUtil.SetFocus(m_tbStrTrl, this);
						return;
					}
				}
			}
			else if(m_tabMain.SelectedTab == m_tabDialogs)
			{
				List<TreeNode> vNodes = new List<TreeNode>();
				List<string> vValues = new List<string>();
				GetControlTreeItems(m_tvControls.Nodes, vNodes, vValues);

				int iOffset = vNodes.IndexOf(m_tvControls.SelectedNode) + 1;

				for(int i = 0; i < vNodes.Count; ++i)
				{
					int j = ((iOffset + i) % vNodes.Count);
					TreeNode tn = vNodes[j];

					string strEng = null, strText = null;
					KPControlCustomization kpcc = (tn.Tag as KPControlCustomization);
					if(kpcc != null)
					{
						strEng = kpcc.TextEnglish;
						strText = kpcc.Text;
					}

					if(string.IsNullOrEmpty(strEng) || (Array.IndexOf<string>(
						m_vEmpty, strEng) >= 0))
						strText = "Dummy";

					if(string.IsNullOrEmpty(strText))
					{
						m_tvControls.SelectedNode = tn;
						UIUtil.SetFocus(m_tbCtrlTrlText, this);
						return;
					}
				}
			}
			else { Debug.Assert(false); return; } // Unsupported tab

			// MessageService.ShowInfo("No untranslated strings found on the current tab page.");
			MessageBox.Show(this, "No untranslated strings found on the current tab page.",
				TrlUtilName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
