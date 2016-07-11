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

using KeePass.UI;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Delegates;

namespace KeePass.Forms
{
	public partial class EntryReportForm : Form
	{
		private string m_strTitle = string.Empty;
		private PwGroup m_pgDataSource = null;

		public void InitEx(string strTitle, PwGroup pgDataSource)
		{
			m_strTitle = strTitle;
			m_pgDataSource = pgDataSource;
		}

		public EntryReportForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			this.Icon = Properties.Resources.KeePass;

			if(!string.IsNullOrEmpty(m_strTitle)) this.Text = m_strTitle;
			else m_strTitle = PwDefs.ShortProductName;

			m_lvEntries.Columns.Add(KPRes.Title);
			m_lvEntries.Columns.Add(KPRes.UserName);
			m_lvEntries.Columns.Add(string.Empty);

			ListViewGroup lvg = new ListViewGroup(m_pgDataSource.GetFullPath(
				" - ", true));
			m_lvEntries.Groups.Add(lvg);

			GroupHandler gh = delegate(PwGroup pg)
			{
				if(pg.Entries.UCount != 0)
				{
					lvg = new ListViewGroup(pg.GetFullPath(" - ", true));
					m_lvEntries.Groups.Add(lvg);
				}

				return true;
			};

			EntryHandler eh = delegate(PwEntry pe)
			{
				ListViewItem lvi = new ListViewItem(pe.Strings.ReadSafe(PwDefs.TitleField));

				lvi.SubItems.Add(pe.Strings.ReadSafe(PwDefs.UserNameField));
				lvi.SubItems.Add(string.Empty);

				m_lvEntries.Items.Add(lvi);
				lvg.Items.Add(lvi);
				return true;
			};

			m_pgDataSource.TraverseTree(TraversalMethod.PreOrder, gh, eh);

			UIUtil.ResizeColumns(m_lvEntries, new int[] { 2, 1, 2 }, true);
		}
	}
}
