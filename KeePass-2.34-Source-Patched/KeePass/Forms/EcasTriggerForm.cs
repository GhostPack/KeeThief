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
using KeePass.UI;
using KeePass.Resources;
using KeePass.Ecas;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class EcasTriggerForm : Form
	{
		private EcasTrigger m_triggerInOut = null;
		private EcasTrigger m_trigger = null;

		private bool m_bEditing = false;
		private ImageList m_ilIcons = null;

		public void InitEx(EcasTrigger trigger, bool bEditing, ImageList ilIcons)
		{
			m_triggerInOut = trigger;
			m_trigger = trigger.CloneDeep();

			m_bEditing = bEditing;
			m_ilIcons = ilIcons;
		}

		public EcasTriggerForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			string strTitle = (m_bEditing ? KPRes.TriggerEdit : KPRes.TriggerAdd);
			string strDesc = (m_bEditing ? KPRes.TriggerEditDesc : KPRes.TriggerAddDesc);
			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Run, strTitle, strDesc);
			this.Text = strTitle;
			this.Icon = Properties.Resources.KeePass;

			m_lvEvents.SmallImageList = m_ilIcons;
			m_lvConditions.SmallImageList = m_ilIcons;
			m_lvActions.SmallImageList = m_ilIcons;

			Debug.Assert((m_lvEvents.Width == m_lvConditions.Width) &&
				(m_lvEvents.Width == m_lvActions.Width));
			int nColWidth = ((m_lvEvents.ClientSize.Width - UIUtil.GetVScrollBarWidth()) / 2);
			m_lvEvents.Columns.Add(KPRes.Event, nColWidth);
			m_lvEvents.Columns.Add(string.Empty, nColWidth);
			m_lvConditions.Columns.Add(KPRes.Condition, nColWidth);
			m_lvConditions.Columns.Add(string.Empty, nColWidth);
			m_lvActions.Columns.Add(KPRes.Action, nColWidth);
			m_lvActions.Columns.Add(string.Empty, nColWidth);

			m_tbName.Text = m_trigger.Name;
			UIUtil.SetMultilineText(m_tbComments, m_trigger.Comments);
			m_cbEnabled.Checked = m_trigger.Enabled;
			m_cbInitiallyOn.Checked = m_trigger.InitiallyOn;
			m_cbTurnOffAfterAction.Checked = m_trigger.TurnOffAfterAction;

			UpdateListsEx(false);
			EnableControlsEx();
			UIUtil.SetFocus(m_tbName, this);
		}

		private void CleanUpEx()
		{
			// Detach event handlers
			m_lvEvents.SmallImageList = null;
			m_lvConditions.SmallImageList = null;
			m_lvActions.SmallImageList = null;
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			m_triggerInOut.Name = m_tbName.Text;
			m_triggerInOut.Comments = m_tbComments.Text;
			m_triggerInOut.Enabled = m_cbEnabled.Checked;
			m_triggerInOut.InitiallyOn = m_cbInitiallyOn.Checked;
			m_triggerInOut.TurnOffAfterAction = m_cbTurnOffAfterAction.Checked;

			m_triggerInOut.EventCollection = m_trigger.EventCollection;
			m_triggerInOut.ConditionCollection = m_trigger.ConditionCollection;
			m_triggerInOut.ActionCollection = m_trigger.ActionCollection;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnBtnPrev(object sender, EventArgs e)
		{
			if(m_tabMain.SelectedIndex > 0)
				m_tabMain.SelectedIndex = (m_tabMain.SelectedIndex - 1);
		}

		private void OnBtnNext(object sender, EventArgs e)
		{
			if(m_tabMain.SelectedIndex < (m_tabMain.TabCount - 1))
				m_tabMain.SelectedIndex = (m_tabMain.SelectedIndex + 1);
		}

		private void EnableControlsEx()
		{
			int nTab = m_tabMain.SelectedIndex;
			m_btnPrev.Enabled = (nTab > 0);
			m_btnNext.Enabled = (nTab < (m_tabMain.TabCount - 1));

			int nEventsSel = m_lvEvents.SelectedIndices.Count;
			m_btnEventEdit.Enabled = (nEventsSel == 1);
			m_btnEventDelete.Enabled = m_btnEventMoveUp.Enabled =
				m_btnEventMoveDown.Enabled = (nEventsSel > 0);

			int nConditionsSel = m_lvConditions.SelectedIndices.Count;
			m_btnConditionEdit.Enabled = (nConditionsSel == 1);
			m_btnConditionDelete.Enabled = m_btnConditionMoveUp.Enabled =
				m_btnConditionMoveDown.Enabled = (nConditionsSel > 0);

			int nActionsSel = m_lvActions.SelectedIndices.Count;
			m_btnActionEdit.Enabled = (nActionsSel == 1);
			m_btnActionDelete.Enabled = m_btnActionMoveUp.Enabled =
				m_btnActionMoveDown.Enabled = (nActionsSel > 0);
		}

		private void UpdateListsEx(bool bRestoreSelected)
		{
			UpdateEventListEx(bRestoreSelected);
			UpdateConditionListEx(bRestoreSelected);
			UpdateActionListEx(bRestoreSelected);
		}

		private void UpdateEventListEx(bool bRestoreSelected)
		{
			object[] vSelected = (bRestoreSelected ?
				UIUtil.GetSelectedItemTags(m_lvEvents) : null);
			UIScrollInfo s = UIUtil.GetScrollInfo(m_lvEvents, true);
			List<EcasEvent> lToRemove = new List<EcasEvent>();

			m_lvEvents.BeginUpdate();
			m_lvEvents.Items.Clear();
			foreach(EcasEvent e in m_trigger.EventCollection)
			{
				EcasEventType t = Program.EcasPool.FindEvent(e.Type);
				if(t == null) { Debug.Assert(false); lToRemove.Add(e); continue; }

				ListViewItem lvi = m_lvEvents.Items.Add(t.Name);
				lvi.SubItems.Add(EcasUtil.ParametersToString(e, t.Parameters));
				lvi.Tag = e;
				lvi.ImageIndex = (int)t.Icon;
			}

			foreach(EcasEvent e in lToRemove)
				m_trigger.EventCollection.Remove(e);
			if(vSelected != null) UIUtil.SelectItems(m_lvEvents, vSelected);

			UIUtil.Scroll(m_lvEvents, s, true);
			m_lvEvents.EndUpdate();
		}

		private void UpdateConditionListEx(bool bRestoreSelected)
		{
			object[] vSelected = (bRestoreSelected ?
				UIUtil.GetSelectedItemTags(m_lvConditions) : null);
			UIScrollInfo s = UIUtil.GetScrollInfo(m_lvConditions, true);
			List<EcasCondition> lToRemove = new List<EcasCondition>();

			m_lvConditions.BeginUpdate();
			m_lvConditions.Items.Clear();
			foreach(EcasCondition c in m_trigger.ConditionCollection)
			{
				EcasConditionType t = Program.EcasPool.FindCondition(c.Type);
				if(t == null) { Debug.Assert(false); lToRemove.Add(c); continue; }

				ListViewItem lvi = m_lvConditions.Items.Add(t.Name);
				lvi.SubItems.Add(EcasUtil.ParametersToString(c, t.Parameters));
				lvi.Tag = c;
				lvi.ImageIndex = (int)t.Icon;
			}

			foreach(EcasCondition c in lToRemove)
				m_trigger.ConditionCollection.Remove(c);
			if(vSelected != null) UIUtil.SelectItems(m_lvConditions, vSelected);

			UIUtil.Scroll(m_lvConditions, s, true);
			m_lvConditions.EndUpdate();
		}

		private void UpdateActionListEx(bool bRestoreSelected)
		{
			object[] vSelected = (bRestoreSelected ?
				UIUtil.GetSelectedItemTags(m_lvActions) : null);
			UIScrollInfo s = UIUtil.GetScrollInfo(m_lvActions, true);
			List<EcasAction> lToRemove = new List<EcasAction>();

			m_lvActions.BeginUpdate();
			m_lvActions.Items.Clear();
			foreach(EcasAction a in m_trigger.ActionCollection)
			{
				EcasActionType t = Program.EcasPool.FindAction(a.Type);
				if(t == null) { Debug.Assert(false); lToRemove.Add(a); continue; }

				ListViewItem lvi = m_lvActions.Items.Add(t.Name);
				lvi.SubItems.Add(EcasUtil.ParametersToString(a, t.Parameters));
				lvi.Tag = a;
				lvi.ImageIndex = (int)t.Icon;
			}

			foreach(EcasAction a in lToRemove)
				m_trigger.ActionCollection.Remove(a);
			if(vSelected != null) UIUtil.SelectItems(m_lvActions, vSelected);

			UIUtil.Scroll(m_lvActions, s, true);
			m_lvActions.EndUpdate();
		}

		private void OnEventsSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnConditionsSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnActionsSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnEventAdd(object sender, EventArgs e)
		{
			EcasEvent eNew = new EcasEvent();
			eNew.Type = EcasEventIDs.AppLoadPost;

			EcasEventForm dlg = new EcasEventForm();
			dlg.InitEx(eNew);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
			{
				m_trigger.EventCollection.Add(eNew);
				UpdateEventListEx(false);
			}
		}

		private void OnEventEdit(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvEvents.SelectedItems;
			if((lvsic == null) || (lvsic.Count == 0)) return;

			EcasEventForm dlg = new EcasEventForm();
			dlg.InitEx(lvsic[0].Tag as EcasEvent);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
				UpdateEventListEx(true);
		}

		private void OnEventDelete(object sender, EventArgs e)
		{
			UIUtil.DeleteSelectedItems(m_lvEvents, m_trigger.EventCollection);
		}

		private void OnConditionAdd(object sender, EventArgs e)
		{
			EcasCondition eNew = new EcasCondition();
			EcasConditionForm dlg = new EcasConditionForm();
			dlg.InitEx(eNew);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
			{
				m_trigger.ConditionCollection.Add(eNew);
				UpdateConditionListEx(false);
			}
		}

		private void OnConditionEdit(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvConditions.SelectedItems;
			if((lvsic == null) || (lvsic.Count == 0)) return;

			EcasConditionForm dlg = new EcasConditionForm();
			dlg.InitEx(lvsic[0].Tag as EcasCondition);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
				UpdateConditionListEx(true);
		}

		private void OnConditionDelete(object sender, EventArgs e)
		{
			UIUtil.DeleteSelectedItems(m_lvConditions, m_trigger.ConditionCollection);
		}

		private void OnActionAdd(object sender, EventArgs e)
		{
			EcasAction eNew = new EcasAction();
			EcasActionForm dlg = new EcasActionForm();
			dlg.InitEx(eNew);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
			{
				m_trigger.ActionCollection.Add(eNew);
				UpdateActionListEx(false);
			}
		}

		private void OnActionEdit(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvActions.SelectedItems;
			if((lvsic == null) || (lvsic.Count == 0)) return;

			EcasActionForm dlg = new EcasActionForm();
			dlg.InitEx(lvsic[0].Tag as EcasAction);
			if(UIUtil.ShowDialogAndDestroy(dlg) == DialogResult.OK)
				UpdateActionListEx(true);
		}

		private void OnActionDelete(object sender, EventArgs e)
		{
			UIUtil.DeleteSelectedItems(m_lvActions, m_trigger.ActionCollection);
		}

		private void OnBtnEventMoveUp(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvEvents,
				m_trigger.EventCollection, true);
			UpdateEventListEx(true);
		}

		private void OnBtnEventMoveDown(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvEvents,
				m_trigger.EventCollection, false);
			UpdateEventListEx(true);
		}

		private void OnBtnConditionMoveUp(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvConditions,
				m_trigger.ConditionCollection, true);
			UpdateConditionListEx(true);
		}

		private void OnBtnConditionMoveDown(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvConditions,
				m_trigger.ConditionCollection, false);
			UpdateConditionListEx(true);
		}

		private void OnBtnActionMoveUp(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvActions,
				m_trigger.ActionCollection, true);
			UpdateActionListEx(true);
		}

		private void OnBtnActionMoveDown(object sender, EventArgs e)
		{
			UIUtil.MoveSelectedItemsInternalOne(m_lvActions,
				m_trigger.ActionCollection, false);
			UpdateActionListEx(true);
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.Triggers, null);
		}

		private void OnEventsItemActivate(object sender, EventArgs e)
		{
			OnEventEdit(sender, e);
		}

		private void OnConditionsItemActivate(object sender, EventArgs e)
		{
			OnConditionEdit(sender, e);
		}

		private void OnActionsItemActivate(object sender, EventArgs e)
		{
			OnActionEdit(sender, e);
		}

		private void OnTabMainSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}
	}
}
