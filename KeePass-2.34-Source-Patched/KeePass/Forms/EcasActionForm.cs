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
	public partial class EcasActionForm : Form
	{
		private EcasAction m_actionInOut = null;
		private EcasAction m_action = null; // Working copy

		private bool m_bBlockTypeSelectionHandler = false;

		public void InitEx(EcasAction e)
		{
			m_actionInOut = e;
			m_action = e.CloneDeep();
		}

		public EcasActionForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			this.Text = KPRes.Action;
			this.Icon = Properties.Resources.KeePass;

			m_lblParamHint.Text = KPRes.ParamDescHelp;

			foreach(EcasActionProvider ap in Program.EcasPool.ActionProviders)
			{
				foreach(EcasActionType t in ap.Actions)
					m_cmbActions.Items.Add(t.Name);
			}

			UpdateDataEx(m_action, false, EcasTypeDxMode.Selection);
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private bool UpdateDataEx(EcasAction a, bool bGuiToInternal,
			EcasTypeDxMode dxType)
		{
			m_bBlockTypeSelectionHandler = true;
			bool bResult = EcasUtil.UpdateDialog(EcasObjectType.Action, m_cmbActions,
				m_dgvParams, a, bGuiToInternal, dxType);
			m_bBlockTypeSelectionHandler = false;
			return bResult;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(!UpdateDataEx(m_actionInOut, true, EcasTypeDxMode.Selection))
				this.DialogResult = DialogResult.None;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnActionsSelectedIndexChanged(object sender, EventArgs e)
		{
			if(m_bBlockTypeSelectionHandler) return;

			UpdateDataEx(m_action, true, EcasTypeDxMode.ParamsTag);
			UpdateDataEx(m_action, false, EcasTypeDxMode.None);
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.Triggers, AppDefs.HelpTopics.TriggersActions);
		}
	}
}
