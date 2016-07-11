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
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using KeePass.App;
using KeePass.UI;
using KeePass.Resources;
using KeePass.Ecas;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class EcasTriggersForm : Form
	{
		private EcasTriggerSystem m_triggersInOut = null;
		private EcasTriggerSystem m_triggers = null;

		private ImageList m_ilIcons = null;

		private const string XmlTriggerRootName = "TriggerCollection";

		public ContextMenuStrip ToolsContextMenu
		{
			get { return m_ctxTools; }
		}

		public bool InitEx(EcasTriggerSystem triggers, ImageList ilIcons)
		{
			m_triggersInOut = triggers;
			m_triggers = triggers.CloneDeep();

			m_ilIcons = ilIcons;

			return AppPolicy.Try(AppPolicyId.EditTriggers);
		}

		public EcasTriggersForm()
		{
			InitializeComponent();

			Program.Translation.ApplyTo(this);
			Program.Translation.ApplyTo("KeePass.Forms.EcasTriggersForm.m_ctxTools", m_ctxTools.Items);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_triggers != null); if(m_triggers == null) return;

			GlobalWindowManager.AddWindow(this);
			GlobalWindowManager.CustomizeControl(m_ctxTools);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Make_KDevelop, KPRes.Triggers,
				KPRes.TriggersDesc);
			this.Text = KPRes.Triggers;
			this.Icon = Properties.Resources.KeePass;

			int nWidth = (m_lvTriggers.ClientSize.Width - UIUtil.GetVScrollBarWidth() - 1);
			m_lvTriggers.Columns.Add(KPRes.Triggers, nWidth);

			m_lvTriggers.SmallImageList = m_ilIcons;

			m_cbEnableTriggers.Checked = m_triggers.Enabled;
			UpdateTriggerListEx(false);
		}

		private void CleanUpEx()
		{
			m_lvTriggers.SmallImageList = null; // Detach event handlers
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			m_triggersInOut.Enabled = m_cbEnableTriggers.Checked;
			m_triggersInOut.TriggerCollection = m_triggers.TriggerCollection;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void EnableControlsEx()
		{
			int nSelCount = m_lvTriggers.SelectedIndices.Count;

			bool bCanMove = ((m_lvTriggers.Items.Count >= 2) && (nSelCount >= 1));
			m_btnMoveUp.Enabled = m_btnMoveDown.Enabled = bCanMove;

			m_btnEdit.Enabled = (nSelCount == 1);
			m_btnDelete.Enabled = (nSelCount >= 1);
		}

		private void UpdateTriggerListEx(bool bRestoreSelected)
		{
			object[] vSelected = (bRestoreSelected ?
				UIUtil.GetSelectedItemTags(m_lvTriggers) : null);
			UIScrollInfo s = UIUtil.GetScrollInfo(m_lvTriggers, true);

			m_lvTriggers.BeginUpdate();
			m_lvTriggers.Items.Clear();
			foreach(EcasTrigger t in m_triggers.TriggerCollection)
			{
				ListViewItem lvi = m_lvTriggers.Items.Add(t.Name);
				lvi.SubItems.Add(t.Comments);
				lvi.Tag = t;
				lvi.ImageIndex = (t.Enabled ? (int)PwIcon.Run : (int)PwIcon.Expired);
			}

			if(vSelected != null) UIUtil.SelectItems(m_lvTriggers, vSelected);
			UIUtil.Scroll(m_lvTriggers, s, true);
			m_lvTriggers.EndUpdate();

			EnableControlsEx();
		}

		private void OnBtnAdd(object sender, EventArgs e)
		{
			EcasTrigger tNew = new EcasTrigger(true);
			EcasTriggerForm f = new EcasTriggerForm();
			f.InitEx(tNew, false, m_ilIcons);
			if(UIUtil.ShowDialogAndDestroy(f) == DialogResult.OK)
			{
				m_triggers.TriggerCollection.Add(tNew);
				UpdateTriggerListEx(false);
			}
		}

		private void OnBtnEdit(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvTriggers.SelectedItems;
			if((lvsic == null) || (lvsic.Count == 0)) return;

			EcasTriggerForm dlg = new EcasTriggerForm();
			dlg.InitEx(lvsic[0].Tag as EcasTrigger, true, m_ilIcons);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
				UpdateTriggerListEx(true);
		}

		private void OnBtnDelete(object sender, EventArgs e)
		{
			UIUtil.DeleteSelectedItems(m_lvTriggers, m_triggers.TriggerCollection);
		}

		private void OnBtnMoveUp(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvTriggers, m_triggers.TriggerCollection, true);
			UpdateTriggerListEx(true);
		}

		private void OnBtnMoveDown(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvTriggers, m_triggers.TriggerCollection, false);
			UpdateTriggerListEx(true);
		}

		private void OnBtnTools(object sender, EventArgs e)
		{
			m_ctxTools.Show(m_btnTools, new Point(0, m_btnTools.Size.Height));
		}

		private void OnCtxToolsHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.Triggers, null);
		}

		private void DoCopyTriggers(ListViewItem[] vTriggers)
		{
			if(vTriggers == null) return;

			try
			{
				ClipboardUtil.Clear();
				if(vTriggers.Length == 0) return;

				EcasTriggerContainer v = new EcasTriggerContainer();
				for(int iTrigger = 0; iTrigger < vTriggers.Length; ++iTrigger)
					v.Triggers.Add(vTriggers[iTrigger].Tag as EcasTrigger);

				XmlWriterSettings xws = new XmlWriterSettings();
				xws.Encoding = new UTF8Encoding(false);
				xws.Indent = true;
				xws.IndentChars = "\t";

				MemoryStream ms = new MemoryStream();
				XmlWriter xw = XmlWriter.Create(ms, xws);

				XmlSerializer xmls = new XmlSerializer(typeof(EcasTriggerContainer));
				xmls.Serialize(xw, v);

				ClipboardUtil.Copy(StrUtil.Utf8.GetString(ms.ToArray()), false,
					false, null, null, this.Handle);
				xw.Close();
				ms.Close();
			}
			catch(Exception excp) { MessageService.ShowWarning(excp.Message); }
		}

		private void OnCtxToolsCopyTriggers(object sender, EventArgs e)
		{
			ListViewItem[] v = new ListViewItem[m_lvTriggers.Items.Count];
			m_lvTriggers.Items.CopyTo(v, 0);
			DoCopyTriggers(v);
		}

		private void OnCtxToolsCopySelectedTriggers(object sender, EventArgs e)
		{
			ListViewItem[] v = new ListViewItem[m_lvTriggers.SelectedItems.Count];
			m_lvTriggers.SelectedItems.CopyTo(v, 0);
			DoCopyTriggers(v);
		}

		private void OnCtxToolsPasteTriggers(object sender, EventArgs e)
		{
			try
			{
				string strData = ClipboardUtil.GetText();
				XmlSerializer xmls = new XmlSerializer(typeof(EcasTriggerContainer));

				byte[] pbData = StrUtil.Utf8.GetBytes(strData);
				MemoryStream ms = new MemoryStream(pbData, false);
				EcasTriggerContainer c = (EcasTriggerContainer)xmls.Deserialize(ms);
				ms.Close();

				foreach(EcasTrigger t in c.Triggers)
				{
					if(m_triggers.FindObjectByUuid(t.Uuid) != null)
						t.Uuid = new PwUuid(true);
					
					m_triggers.TriggerCollection.Add(t);
				}
			}
			catch(Exception excp) { MessageService.ShowWarning(excp.Message); }

			UpdateTriggerListEx(true);
		}

		private void OnTriggersItemActivate(object sender, EventArgs e)
		{
			OnBtnEdit(sender, e);
		}

		private void OnTriggersSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}
	}
}
