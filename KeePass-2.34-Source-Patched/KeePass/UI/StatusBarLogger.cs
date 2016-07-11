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
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using KeePassLib.Interfaces;

namespace KeePass.UI
{
	public sealed class StatusBarLogger : IStatusLogger
	{
		private ToolStripStatusLabel m_sbText = null;
		private ToolStripProgressBar m_pbProgress = null;
		private bool m_bStartedLogging = false;
		private bool m_bEndedLogging = false;
		private uint m_uLastPercent = 0;

		~StatusBarLogger()
		{
			Debug.Assert(m_bEndedLogging);
			if(!m_bEndedLogging) EndLogging();
		}

		public void SetControls(ToolStripStatusLabel sbText, ToolStripProgressBar pbProgress)
		{
			m_sbText = sbText;
			m_pbProgress = pbProgress;

			if(m_pbProgress != null)
			{
				if(m_pbProgress.Minimum != 0) m_pbProgress.Minimum = 0;
				if(m_pbProgress.Maximum != 100) m_pbProgress.Maximum = 100;
			}
		}

		public void StartLogging(string strOperation, bool bWriteOperationToLog)
		{
			Debug.Assert(!m_bStartedLogging && !m_bEndedLogging);

			m_pbProgress.Value = 0;
			m_uLastPercent = 0;
			m_pbProgress.Visible = true;

			m_bStartedLogging = true;
			this.SetText(strOperation, LogStatusType.Info);
		}

		public void EndLogging()
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);

			if(m_pbProgress != null)
			{
				m_pbProgress.Visible = false;
				m_pbProgress.Value = 100;
			}
			m_uLastPercent = 100;

			m_bEndedLogging = true;
		}

		public bool SetProgress(uint uPercent)
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);

			if((m_pbProgress != null) && (uPercent != m_uLastPercent))
			{
				m_pbProgress.Value = (int)uPercent;
				m_uLastPercent = uPercent;

				Application.DoEvents();
			}

			return true;
		}

		public bool SetText(string strNewText, LogStatusType lsType)
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);
			
			if((m_sbText != null) && (lsType == LogStatusType.Info))
			{
				m_sbText.Text = strNewText;
				Application.DoEvents();
			}

			return true;
		}

		public bool ContinueWork()
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);
			return true;
		}
	}
}
