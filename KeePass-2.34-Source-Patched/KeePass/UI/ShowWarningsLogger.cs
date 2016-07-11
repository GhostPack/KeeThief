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

using KeePass.Forms;

using KeePassLib;
using KeePassLib.Interfaces;

namespace KeePass.UI
{
	/// <summary>
	/// This logger displays the current status in a status bar.
	/// As soon as a warning or error is logged, a dialog is opened
	/// and all consecutive log lines are sent to both loggers
	/// (status bar and dialog).
	/// </summary>
	public sealed class ShowWarningsLogger : IStatusLogger
	{
		private StatusBarLogger m_sbDefault = null;
		private StatusLoggerForm m_slForm = null;
		private Form m_fTaskbarWindow = null;
		private bool m_bStartedLogging = false;
		private bool m_bEndedLogging = false;

		private List<KeyValuePair<LogStatusType, string>> m_vCachedMessages =
			new List<KeyValuePair<LogStatusType, string>>();

		[Obsolete]
		public ShowWarningsLogger(StatusBarLogger sbDefault)
		{
			m_sbDefault = sbDefault;
		}

		public ShowWarningsLogger(StatusBarLogger sbDefault, Form fTaskbarWindow)
		{
			m_sbDefault = sbDefault;
			m_fTaskbarWindow = fTaskbarWindow;
		}

		~ShowWarningsLogger()
		{
			Debug.Assert(m_bEndedLogging);
			if(!m_bEndedLogging) EndLogging();
		}

		public void StartLogging(string strOperation, bool bWriteOperationToLog)
		{
			Debug.Assert(!m_bStartedLogging && !m_bEndedLogging);

			if(m_sbDefault != null)
				m_sbDefault.StartLogging(strOperation, bWriteOperationToLog);
			if(m_slForm != null)
				m_slForm.StartLogging(strOperation, bWriteOperationToLog);
			if(m_fTaskbarWindow != null)
			{
				TaskbarList.SetProgressValue(m_fTaskbarWindow, 0, 100);
				TaskbarList.SetProgressState(m_fTaskbarWindow, TbpFlag.Normal);
			}

			m_bStartedLogging = true;
			
			if(bWriteOperationToLog)
				m_vCachedMessages.Add(new KeyValuePair<LogStatusType, string>(
					LogStatusType.Info, strOperation));
		}

		public void EndLogging()
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);

			if(m_sbDefault != null) m_sbDefault.EndLogging();
			if(m_slForm != null) m_slForm.EndLogging();
			if(m_fTaskbarWindow != null)
				TaskbarList.SetProgressState(m_fTaskbarWindow, TbpFlag.NoProgress);

			m_bEndedLogging = true;
		}

		/// <summary>
		/// Set the current progress in percent.
		/// </summary>
		/// <param name="uPercent">Percent of work finished.</param>
		/// <returns>Returns <c>true</c> if the caller should continue
		/// the current work.</returns>
		public bool SetProgress(uint uPercent)
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);

			bool b = true;
			if(m_sbDefault != null)
			{
				if(!m_sbDefault.SetProgress(uPercent)) b = false;
			}
			if(m_slForm != null)
			{
				if(!m_slForm.SetProgress(uPercent)) b = false;
			}
			if(m_fTaskbarWindow != null)
				TaskbarList.SetProgressValue(m_fTaskbarWindow, uPercent, 100);

			return b;
		}

		/// <summary>
		/// Set the current status text.
		/// </summary>
		/// <param name="strNewText">Status text.</param>
		/// <param name="lsType">Type of the message.</param>
		/// <returns>Returns <c>true</c> if the caller should continue
		/// the current work.</returns>
		public bool SetText(string strNewText, LogStatusType lsType)
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);

			if((m_slForm == null) && ((lsType == LogStatusType.Warning) ||
				(lsType == LogStatusType.Error)))
			{
				m_slForm = new StatusLoggerForm();
				m_slForm.InitEx(false);

				m_slForm.Show();
				m_slForm.BringToFront();

				bool bLoggingStarted = false;
				foreach(KeyValuePair<LogStatusType, string> kvp in m_vCachedMessages)
				{
					if(!bLoggingStarted)
					{
						m_slForm.StartLogging(kvp.Value, true);
						bLoggingStarted = true;
					}
					else m_slForm.SetText(kvp.Value, kvp.Key);
				}
				Debug.Assert(bLoggingStarted);

				m_vCachedMessages.Clear();
			}

			bool b = true;
			if(m_sbDefault != null)
			{
				if(!m_sbDefault.SetText(strNewText, lsType)) b = false;
			}
			if(m_slForm != null)
			{
				if(!m_slForm.SetText(strNewText, lsType)) b = false;
			}

			if(m_slForm == null)
				m_vCachedMessages.Add(new KeyValuePair<LogStatusType, string>(
					lsType, strNewText));

			return b;
		}

		/// <summary>
		/// Check if the user cancelled the current work.
		/// </summary>
		/// <returns>Returns <c>true</c> if the caller should continue
		/// the current work.</returns>
		public bool ContinueWork()
		{
			Debug.Assert(m_bStartedLogging && !m_bEndedLogging);

			bool b = true;
			if(m_slForm != null)
			{
				if(!m_slForm.ContinueWork()) b = false;
			}
			if(m_sbDefault != null)
			{
				if(!m_sbDefault.ContinueWork()) b = false;
			}

			return b;
		}
	}
}
