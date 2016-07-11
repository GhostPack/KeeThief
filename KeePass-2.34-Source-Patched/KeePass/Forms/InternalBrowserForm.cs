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

using KeePassLib;

namespace KeePass.Forms
{
	public partial class InternalBrowserForm : Form
	{
		private string m_strInitialUrl = string.Empty;
		// private PwGroup m_pgDataSource = null; // See InitEx

		public void InitEx(string strUrl, PwGroup pgDataSource)
		{
			if(strUrl != null) m_strInitialUrl = strUrl;

			// m_pgDataSource = pgDataSource; // Not used yet
		}

		public InternalBrowserForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;

			if(m_strInitialUrl.Length > 0)
				m_webBrowser.Navigate(m_strInitialUrl);

			ProcessResize();
			UpdateUIState();
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void UpdateUIState()
		{
			m_btnBack.Enabled = m_webBrowser.CanGoBack;
			m_btnForward.Enabled = m_webBrowser.CanGoForward;

			string strTitle = m_webBrowser.DocumentTitle;
			if(strTitle.Length > 0) strTitle += " - ";
			this.Text = strTitle + PwDefs.ShortProductName;
		}

		private void ProcessResize()
		{
			int nWidth = m_toolNav.ClientRectangle.Width;
			int nRight = m_btnGo.Bounds.X + m_btnGo.Bounds.Width;

			Size size = new Size(m_tbUrl.Size.Width, m_tbUrl.Size.Height);
			size.Width += nWidth - nRight - 2;
			m_tbUrl.Size = size;
		}

		private static void DoAutoFill() // Remove static when implementing
		{
		}

		private void OnBtnBack(object sender, EventArgs e)
		{
			m_webBrowser.GoBack();
		}

		private void OnBtnForward(object sender, EventArgs e)
		{
			m_webBrowser.GoForward();
		}

		private void OnBtnReload(object sender, EventArgs e)
		{
			m_webBrowser.Refresh(WebBrowserRefreshOption.Completely);
		}

		private void OnBtnStop(object sender, EventArgs e)
		{
			m_webBrowser.Stop();
		}

		private void OnBtnGo(object sender, EventArgs e)
		{
			m_webBrowser.Navigate(m_tbUrl.Text);
		}

		private void OnWbNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			UpdateUIState();
		}

		private void OnWbDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			m_tbUrl.Text = e.Url.ToString();

			DoAutoFill();
			UpdateUIState();
		}

		private void OnTbUrlKeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
			{
				UIUtil.SetHandled(e, true);

				OnBtnGo(sender, e);
			}
		}

		private void OnFormSizeChanged(object sender, EventArgs e)
		{
			ProcessResize();
		}
	}
}