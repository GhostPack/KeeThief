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
using KeePassLib.Security;

namespace KeePass.Forms
{
	public partial class TanWizardForm : Form
	{
		private PwDatabase m_pwDatabase = null;
		private PwGroup m_pgStorage = null;

		public void InitEx(PwDatabase pwParent, PwGroup pgStorage)
		{
			m_pwDatabase = pwParent;
			m_pgStorage = pgStorage;
		}

		public TanWizardForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_pwDatabase != null); if(m_pwDatabase == null) throw new InvalidOperationException();
			Debug.Assert(m_pgStorage != null); if(m_pgStorage == null) throw new InvalidOperationException();

			GlobalWindowManager.AddWindow(this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				KeePass.Properties.Resources.B48x48_Wizard, KPRes.TanWizard,
				KPRes.TanWizardDesc);
			
			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.TanWizard;

			if((m_pgStorage.Name != null) && (m_pgStorage.Name.Length > 0))
				m_lblToGroup.Text += ": " + m_pgStorage.Name + ".";
			else
				m_lblToGroup.Text += ".";

			m_tbTanChars.Text = Program.Config.Defaults.TanCharacters;

			EnableControlsEx();
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			ParseTans();
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void CleanUpEx()
		{
			Program.Config.Defaults.TanCharacters = m_tbTanChars.Text;
		}

		private void EnableControlsEx()
		{
			m_numTANsIndex.Enabled = m_cbNumberTans.Checked;
		}

		private void ParseTans()
		{
			StringBuilder sb = new StringBuilder();
			string strText = m_tbTANs.Text;
			int nTanIndex = (int)m_numTANsIndex.Value;
			bool bSetIndex = m_cbNumberTans.Checked;
			string strTanChars = m_tbTanChars.Text;

			for(int i = 0; i < strText.Length; ++i)
			{
				char ch = strText[i];

				if(strTanChars.IndexOf(ch) >= 0)
					sb.Append(ch);
				else
				{
					AddTan(sb.ToString(), bSetIndex, ref nTanIndex);
					sb = new StringBuilder(); // Reset string
				}
			}

			if(sb.Length > 0) AddTan(sb.ToString(), bSetIndex, ref nTanIndex);
		}

		private void AddTan(string strTan, bool bSetIndex, ref int nTanIndex)
		{
			if(strTan.Length == 0) return;

			PwEntry pe = new PwEntry(true, true);
			pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
				m_pwDatabase.MemoryProtection.ProtectTitle, PwDefs.TanTitle));

			pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
				m_pwDatabase.MemoryProtection.ProtectPassword, strTan));

			if(bSetIndex && (nTanIndex >= 0))
			{
				Debug.Assert(PwDefs.TanIndexField == PwDefs.UserNameField);

				pe.Strings.Set(PwDefs.TanIndexField, new ProtectedString(
					m_pwDatabase.MemoryProtection.ProtectUserName, nTanIndex.ToString()));

				++nTanIndex;
			}

			m_pgStorage.AddEntry(pe, true);
		}

		private void OnNumberTANsCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			CleanUpEx();
		}
	}
}
