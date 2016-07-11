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
using KeePassLib.Delegates;

namespace KeePass.Forms
{
	public partial class DatabaseOperationsForm : Form, IGwmWindow
	{
		private PwDatabase m_pwDatabase = null;
		private bool m_bModified = false;

		public bool CanCloseWithoutDataLoss { get { return true; } }
		public bool HasModifiedDatabase { get { return m_bModified; } }

		public void InitEx(PwDatabase pwDatabase)
		{
			m_pwDatabase = pwDatabase;
		}

		public DatabaseOperationsForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_pwDatabase != null); if(m_pwDatabase == null) throw new InvalidOperationException();

			GlobalWindowManager.AddWindow(this, this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Package_Settings, KPRes.DatabaseMaintenance,
				KPRes.DatabaseMaintenanceDesc);
			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.DatabaseMaintenance;

			m_numHistoryDays.Value = m_pwDatabase.MaintenanceHistoryDays;

			m_pbStatus.Visible = false;
		}

		private void OnBtnClose(object sender, EventArgs e)
		{
			m_pwDatabase.MaintenanceHistoryDays = (uint)m_numHistoryDays.Value;
		}

		private void EnableStatusMsgEx(bool bEnable)
		{
			if(bEnable)
			{
				m_btnClose.Enabled = m_btnHistoryEntriesDelete.Enabled =
					m_btnRemoveDelObjInfo.Enabled = false;
				if(!m_pbStatus.Visible) m_pbStatus.Visible = true;

				m_pbStatus.Value = 0;
			}
			else
			{
				m_btnClose.Enabled = m_btnHistoryEntriesDelete.Enabled =
					m_btnRemoveDelObjInfo.Enabled = true;

				m_pbStatus.Value = m_pbStatus.Maximum;
			}
		}

		private void OnBtnDelete(object sender, EventArgs e)
		{
			EnableStatusMsgEx(true);

			DateTime dtNow = DateTime.Now;
			TimeSpan tsSpan = new TimeSpan((int)m_numHistoryDays.Value, 0, 0, 0);

			uint uNumGroups, uNumEntries;
			m_pwDatabase.RootGroup.GetCounts(true, out uNumGroups, out uNumEntries);

			uint uCurEntryNumber = 1;
			EntryHandler eh = delegate(PwEntry pe)
			{
				for(uint u = 0; u < pe.History.UCount; ++u)
				{
					PwEntry peHist = pe.History.GetAt(u);

					if((dtNow - peHist.LastModificationTime) >= tsSpan)
					{
						pe.History.Remove(peHist);
						--u;

						m_bModified = true;
					}
				}

				m_pbStatus.Value = (int)((uCurEntryNumber * 100) / uNumEntries);
				++uCurEntryNumber;
				return true;
			};

			m_pwDatabase.RootGroup.TraverseTree(TraversalMethod.PreOrder, null, eh);

			EnableStatusMsgEx(false); // Database is set modified by parent
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnRemoveDelObjInfo(object sender, EventArgs e)
		{
			EnableStatusMsgEx(true);

			if(m_pwDatabase.DeletedObjects.UCount > 0)
			{
				m_pwDatabase.DeletedObjects.Clear();
				m_bModified = true;
			}

			EnableStatusMsgEx(false); // Database is set modified by parent
		}
	}
}
