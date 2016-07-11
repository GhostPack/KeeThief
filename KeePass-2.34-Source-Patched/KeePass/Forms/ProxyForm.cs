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

using KeePass.App.Configuration;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Serialization;

namespace KeePass.Forms
{
	public partial class ProxyForm : Form
	{
		public ProxyForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;

			ProxyServerType pst = Program.Config.Integration.ProxyType;
			if(pst == ProxyServerType.None) m_rbNoProxy.Checked = true;
			else if(pst == ProxyServerType.Manual) m_rbManualProxy.Checked = true;
			else m_rbSystemProxy.Checked = true;

			m_tbAddress.Text = Program.Config.Integration.ProxyAddress;
			m_tbPort.Text = Program.Config.Integration.ProxyPort;

			string strUserName = Program.Config.Integration.ProxyUserName;
			string strPassword = Program.Config.Integration.ProxyPassword;

			ProxyAuthType pat = Program.Config.Integration.ProxyAuthType;
			if(pat == ProxyAuthType.Auto)
			{
				if((strUserName.Length > 0) || (strPassword.Length > 0))
					pat = ProxyAuthType.Manual;
				else pat = ProxyAuthType.Default;
			}

			if(pat == ProxyAuthType.None) m_rbAuthNone.Checked = true;
			else if(pat == ProxyAuthType.Manual) m_rbAuthManual.Checked = true;
			else m_rbAuthDefault.Checked = true;

			m_tbUser.Text = strUserName;
			m_tbPassword.Text = strPassword;

			EnableControlsEx();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			ProxyServerType pst = ProxyServerType.System;
			if(m_rbNoProxy.Checked) pst = ProxyServerType.None;
			else if(m_rbManualProxy.Checked) pst = ProxyServerType.Manual;

			ProxyAuthType pat = ProxyAuthType.Default;
			if(m_rbAuthNone.Checked) pat = ProxyAuthType.None;
			else if(m_rbAuthManual.Checked) pat = ProxyAuthType.Manual;

			AceIntegration ace = Program.Config.Integration;
			ace.ProxyType = pst;
			ace.ProxyAddress = m_tbAddress.Text;
			ace.ProxyPort = m_tbPort.Text;
			ace.ProxyAuthType = pat;
			ace.ProxyUserName = m_tbUser.Text;
			ace.ProxyPassword = m_tbPassword.Text;

			Program.Config.Apply(AceApplyFlags.Proxy);
		}

		private void EnableControlsEx()
		{
			Control[] vAddr = new Control[] {
				m_lblAddress, m_tbAddress, m_lblPort, m_tbPort
			};
			Control[] vAuthType = new Control[] {
				m_rbAuthNone, m_rbAuthDefault, m_rbAuthManual
			};
			Control[] vAuthData = new Control[] {
				m_lblUser, m_tbUser, m_lblPassword, m_tbPassword
			};
			List<Control> lAuthAll = new List<Control>(vAuthType);
			lAuthAll.AddRange(vAuthData);

			bool bAddr = m_rbManualProxy.Checked;
			foreach(Control cAddr in vAddr) { cAddr.Enabled = bAddr; }

			if(m_rbNoProxy.Checked)
			{
				foreach(Control c in lAuthAll) { c.Enabled = false; }
				m_grpAuth.Enabled = false;
			}
			else
			{
				m_grpAuth.Enabled = true;
				if(m_rbAuthManual.Checked)
				{
					foreach(Control c in lAuthAll) { c.Enabled = true; }
				}
				else
				{
					foreach(Control cC in vAuthType) { cC.Enabled = true; }
					foreach(Control cD in vAuthData) { cD.Enabled = false; }
				}
			}

			// if(!m_rbManualProxy.Checked) m_btnOK.Enabled = true;
			// else m_btnOK.Enabled = (m_tbAddress.Text.Length > 0);
		}

		private void OnNoProxyCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnSystemProxyCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnManualProxyCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnAddressTextChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnAuthNoneCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnAuthDefaultCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnAuthManualCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}
	}
}
