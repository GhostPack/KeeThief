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
using KeePass.Resources;
using KeePass.UI;

namespace KeePass.Forms
{
	public partial class UrlOverrideForm : Form
	{
		private AceUrlSchemeOverride m_ovr = null;

		public void InitEx(AceUrlSchemeOverride ovr)
		{
			m_ovr = ovr;
		}

		public UrlOverrideForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if(m_ovr == null) throw new InvalidOperationException();

			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.UrlOverride;

			m_tbScheme.Text = m_ovr.Scheme;
			m_tbOverride.Text = m_ovr.UrlOverride;
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			m_ovr.Scheme = m_tbScheme.Text;
			m_ovr.UrlOverride = m_tbOverride.Text;
		}
	}
}
