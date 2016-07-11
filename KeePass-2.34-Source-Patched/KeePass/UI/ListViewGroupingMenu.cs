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

using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Collections;

namespace KeePass.UI
{
	public sealed class ListViewGroupingMenu
	{
		private ToolStripMenuItem m_tsmiMenu;
		private MainForm m_mf;

		private Dictionary<AceListGrouping, ToolStripMenuItem> m_dItems =
			new Dictionary<AceListGrouping, ToolStripMenuItem>();

		public ListViewGroupingMenu(ToolStripMenuItem tsmiContainer, MainForm mf)
		{
			if(tsmiContainer == null) throw new ArgumentNullException("tsmiContainer");
			if(mf == null) throw new ArgumentNullException("mf");

			m_tsmiMenu = tsmiContainer;
			m_mf = mf;

			ToolStripMenuItem tsmi = new ToolStripMenuItem(KPRes.On);
			tsmi.Click += this.OnGroupOn;
			m_dItems[AceListGrouping.On] = tsmi;
			m_tsmiMenu.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Auto + " (" + KPRes.RecommendedCmd + ")");
			tsmi.Click += this.OnGroupAuto;
			m_dItems[AceListGrouping.Auto] = tsmi;
			m_tsmiMenu.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Off);
			tsmi.Click += this.OnGroupOff;
			m_dItems[AceListGrouping.Off] = tsmi;
			m_tsmiMenu.DropDownItems.Add(tsmi);

			UpdateUI();
		}

#if DEBUG
		~ListViewGroupingMenu()
		{
			Debug.Assert(m_tsmiMenu == null); // Release should have been called
		}
#endif

		public void Release()
		{
			if(m_tsmiMenu != null)
			{
				m_dItems[AceListGrouping.On].Click -= this.OnGroupOn;
				m_dItems[AceListGrouping.Auto].Click -= this.OnGroupAuto;
				m_dItems[AceListGrouping.Off].Click -= this.OnGroupOff;

				m_dItems.Clear();
				m_tsmiMenu.DropDownItems.Clear();

				m_tsmiMenu = null;
				m_mf = null;
			}
		}

		private void UpdateUI()
		{
			int lgp = (Program.Config.MainWindow.ListGrouping & (int)AceListGrouping.Primary);
			foreach(KeyValuePair<AceListGrouping, ToolStripMenuItem> kvp in m_dItems)
			{
				Debug.Assert(((int)kvp.Key & ~(int)AceListGrouping.Primary) == 0);
				UIUtil.SetRadioChecked(kvp.Value, ((int)kvp.Key == lgp));
			}
		}

		private void SetGrouping(AceListGrouping lgPrimary)
		{
			Debug.Assert(((int)lgPrimary & ~(int)AceListGrouping.Primary) == 0);
			if((int)lgPrimary == (Program.Config.MainWindow.ListGrouping &
				(int)AceListGrouping.Primary))
				return;

			Program.Config.MainWindow.ListGrouping &= ~(int)AceListGrouping.Primary;
			Program.Config.MainWindow.ListGrouping |= (int)lgPrimary;
			Debug.Assert((Program.Config.MainWindow.ListGrouping &
				(int)AceListGrouping.Primary) == (int)lgPrimary);
			UpdateUI();

			if(m_mf == null) { Debug.Assert(false); return; }

			PwDatabase pd = m_mf.ActiveDatabase;
			PwGroup pg = m_mf.GetCurrentEntries();
			if((pd == null) || !pd.IsOpen || (pg == null)) return; // No assert

			PwObjectList<PwEntry> pwl = pg.GetEntries(true);
			if((pwl.UCount > 0) && EntryUtil.EntriesHaveSameParent(pwl))
				m_mf.UpdateUI(false, null, true, pwl.GetAt(0).ParentGroup,
					true, null, false);
			else
			{
				EntryUtil.ReorderEntriesAsInDatabase(pwl, pd); // Requires open DB

				pg = new PwGroup(true, true);
				pg.IsVirtual = true;
				foreach(PwEntry pe in pwl) pg.AddEntry(pe, false);

				m_mf.UpdateUI(false, null, false, null, true, pg, false);
			}
		}

		private void OnGroupOn(object sender, EventArgs e)
		{
			SetGrouping(AceListGrouping.On);
		}

		private void OnGroupAuto(object sender, EventArgs e)
		{
			SetGrouping(AceListGrouping.Auto);
		}

		private void OnGroupOff(object sender, EventArgs e)
		{
			SetGrouping(AceListGrouping.Off);
		}
	}
}
