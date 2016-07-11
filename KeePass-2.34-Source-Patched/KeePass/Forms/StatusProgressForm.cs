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
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;

namespace KeePass.Forms
{
	public partial class StatusProgressForm : Form, IStatusLogger
	{
		private string m_strTitle = null;
		private bool m_bCanCancel = true;
		private bool m_bMarquee = false;
		private Form m_fOwner = null;

		private volatile bool m_bCancelled = false;
		private bool m_bCanClose = true;

		public bool UserCancelled
		{
			get { return m_bCancelled; }
		}

		public static StatusProgressForm ConstructEx(string strTitle,
			bool bCanCancel, bool bMarqueeProgress, Form fOwner,
			string strInitialOp)
		{
			StatusProgressForm dlg = new StatusProgressForm();
			dlg.InitEx(strTitle, bCanCancel, bMarqueeProgress, fOwner);

			if(fOwner != null) dlg.Show(fOwner);
			else dlg.Show();

			dlg.StartLogging(strInitialOp, false);

			return dlg;
		}

		public static void DestroyEx(StatusProgressForm dlg)
		{
			if(dlg == null) { Debug.Assert(false); return; }

			dlg.EndLogging();
			dlg.Close();
			UIUtil.DestroyForm(dlg);
		}

		public void InitEx(string strTitle, bool bCanCancel, bool bMarqueeProgress,
			Form fOwner)
		{
			m_strTitle = strTitle; // May be null
			m_bCanCancel = bCanCancel;
			m_bMarquee = bMarqueeProgress;
			m_fOwner = fOwner;
		}

		public StatusProgressForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			// Must work without a parent window
			Debug.Assert(this.StartPosition == FormStartPosition.CenterScreen);

			GlobalWindowManager.AddWindow(this);

			m_pbTotal.Minimum = 0;
			m_pbTotal.Maximum = 100;
			m_pbTotal.Value = 0;

			try { if(m_bMarquee) m_pbTotal.Style = ProgressBarStyle.Marquee; }
			catch(Exception) { Debug.Assert(WinUtil.IsWindows9x || WinUtil.IsWindows2000); }

			if(!string.IsNullOrEmpty(m_strTitle)) this.Text = m_strTitle;
			else this.Text = PwDefs.ShortProductName;

			try { if(m_fOwner != null) this.Owner = m_fOwner; }
			catch(Exception) { Debug.Assert(false); } // Throws from other thread

			if(!m_bCanCancel) m_btnCancel.Enabled = false;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.None;
			DoCancel();
		}

		private void DoCancel()
		{
			if(m_bCancelled) return;
			if(!m_bCanCancel) return;

			m_bCancelled = true;
			m_btnCancel.Enabled = false;
		}

		public delegate void Priv_SetProgressInternal(string strText, int nPercent);

		private void SetProgressInternal(string strText, int nPercent)
		{
			Debug.Assert(!m_lblTotal.InvokeRequired);
			Debug.Assert(!m_pbTotal.InvokeRequired);

			if(strText != null) m_lblTotal.Text = strText;

			if((nPercent >= 0) && (nPercent <= 100))
				m_pbTotal.Value = nPercent; // .NET compares with cached value
		}

		private bool SetProgressGlobal(string strText, int nPercent)
		{
			if(this.InvokeRequired)
				this.Invoke(new Priv_SetProgressInternal(this.SetProgressInternal),
					strText, nPercent);
			else SetProgressInternal(strText, nPercent);

			Application.DoEvents();
			return !m_bCancelled;
		}

		public void StartLogging(string strOperation, bool bWriteOperationToLog)
		{
			SetProgressGlobal(strOperation, -1);
			m_bCanClose = false;

			if((m_fOwner != null) && (m_fOwner is MainForm))
				TaskbarList.SetProgressState(m_fOwner, TbpFlag.Indeterminate);
		}

		public void EndLogging()
		{
			if((m_fOwner != null) && (m_fOwner is MainForm))
				TaskbarList.SetProgressState(m_fOwner, TbpFlag.NoProgress);

			m_bCanClose = true;
		}

		public bool SetProgress(uint uPercent)
		{
			return SetProgressGlobal(null, (int)uPercent);
		}

		public bool SetText(string strNewText, LogStatusType lsType)
		{
			return SetProgressGlobal(strNewText, -1);
		}

		public bool ContinueWork()
		{
			Application.DoEvents();
			return !m_bCancelled;
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if(!m_bCanClose)
			{
				Debug.Assert(e.CloseReason == CloseReason.UserClosing);
				e.Cancel = true;
				DoCancel();
			}
		}
	}
}
