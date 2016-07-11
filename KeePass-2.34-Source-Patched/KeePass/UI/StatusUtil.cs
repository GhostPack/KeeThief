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
using KeePass.Native;

using KeePassLib;
using KeePassLib.Interfaces;

namespace KeePass.UI
{
	public static class StatusUtil
	{
		private sealed class StatusProgressFormWrapper : IStatusLogger
		{
			private StatusProgressForm m_dlg;
			public StatusProgressForm Form
			{
				get { return m_dlg; }
			}

			public StatusProgressFormWrapper(Form fParent, string strTitle,
				bool bCanCancel, bool bMarqueeProgress)
			{
				m_dlg = new StatusProgressForm();
				m_dlg.InitEx(strTitle, bCanCancel, bMarqueeProgress, fParent);

				if(fParent != null) m_dlg.Show(fParent);
				else m_dlg.Show();
			}

			public void StartLogging(string strOperation, bool bWriteOperationToLog)
			{
				if(m_dlg == null) { Debug.Assert(false); return; }

				m_dlg.StartLogging(strOperation, false);
			}

			public void EndLogging()
			{
				if(m_dlg == null) { Debug.Assert(false); return; }

				m_dlg.EndLogging();
				m_dlg.Close();
				UIUtil.DestroyForm(m_dlg);
				m_dlg = null;
			}

			public bool SetProgress(uint uPercent)
			{
				if(m_dlg == null) { Debug.Assert(false); return true; }

				return m_dlg.SetProgress(uPercent);
			}

			public bool SetText(string strNewText, LogStatusType lsType)
			{
				if(m_dlg == null) { Debug.Assert(false); return true; }

				return m_dlg.SetText(strNewText, lsType);
			}

			public bool ContinueWork()
			{
				if(m_dlg == null) { Debug.Assert(false); return true; }

				return m_dlg.ContinueWork();
			}
		}

		public static IStatusLogger CreateStatusDialog(Form fParent, out Form fOptDialog,
			string strTitle, string strOp, bool bCanCancel, bool bMarqueeProgress)
		{
			if(string.IsNullOrEmpty(strTitle)) strTitle = PwDefs.ShortProductName;
			if(strOp == null) strOp = string.Empty;

			IStatusLogger sl;
			// if(NativeProgressDialog.IsSupported)
			// {
			//	ProgDlgFlags fl = (ProgDlgFlags.AutoTime | ProgDlgFlags.NoMinimize);
			//	if(!bCanCancel) fl |= ProgDlgFlags.NoCancel;
			//	if(bMarqueeProgress) fl |= ProgDlgFlags.MarqueeProgress;
			//	sl = new NativeProgressDialog((fParent != null) ? fParent.Handle :
			//		IntPtr.Zero, fl);
			//	fOptDialog = null;
			// }
			// else
			// {
				StatusProgressFormWrapper w = new StatusProgressFormWrapper(fParent,
					strTitle, bCanCancel, bMarqueeProgress);
				sl = w;
				fOptDialog = w.Form;
			// }

			sl.StartLogging(strOp, false);
			return sl;
		}
	}
}
