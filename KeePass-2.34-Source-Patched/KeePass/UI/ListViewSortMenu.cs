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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public delegate void SortCommandHandler(bool bEnableSorting, int nColumn,
		SortOrder? soForce, bool bUpdateEntryList);

	public sealed class ListViewSortMenu
	{
		private ToolStripMenuItem m_tsmiMenu;
		private ListView m_lv;
		private SortCommandHandler m_h;

		private int m_iCurSortColumn = -1;
		private bool m_bCurSortAsc = true;

		private ToolStripMenuItem m_tsmiInitDummy = null;
		private ToolStripMenuItem m_tsmiNoSort = null;
		private ToolStripSeparator m_tssSep0 = null;
		private List<ToolStripMenuItem> m_vColumns = null;
		private ToolStripSeparator m_tssSep1 = null;
		private ToolStripMenuItem m_tsmiAsc = null;
		private ToolStripMenuItem m_tsmiDesc = null;

		public ListViewSortMenu(ToolStripMenuItem tsmiContainer, ListView lvTarget,
			SortCommandHandler h)
		{
			if(tsmiContainer == null) throw new ArgumentNullException("tsmiContainer");
			if(lvTarget == null) throw new ArgumentNullException("lvTarget");
			if(h == null) throw new ArgumentNullException("h");

			m_tsmiMenu = tsmiContainer;
			m_lv = lvTarget;
			m_h = h;

			m_tsmiInitDummy = new ToolStripMenuItem(KPRes.NoSort);
			m_tsmiMenu.DropDownItems.Add(m_tsmiInitDummy);

			m_tsmiMenu.DropDownOpening += this.UpdateMenu;
		}

#if DEBUG
		~ListViewSortMenu()
		{
			Debug.Assert(m_tsmiMenu == null); // Release should have been called
		}
#endif

		public void Release()
		{
			if(m_tsmiMenu != null)
			{
				DeleteMenuItems();
				m_tsmiMenu.DropDownOpening -= this.UpdateMenu;

				m_tsmiMenu = null;
				m_lv = null;
				m_h = null;
			}
		}

		private void DeleteMenuItems()
		{
			if(m_tsmiInitDummy != null)
			{
				m_tsmiMenu.DropDownItems.Remove(m_tsmiInitDummy);
				m_tsmiInitDummy = null;
			}

			if(m_tsmiNoSort == null) return;

			m_tsmiNoSort.Click -= this.OnNoSort;
			foreach(ToolStripMenuItem tsmiCol in m_vColumns)
				tsmiCol.Click -= this.OnSortColumn;
			m_tsmiAsc.Click -= this.OnSortAscDesc;
			m_tsmiDesc.Click -= this.OnSortAscDesc;

			m_tsmiMenu.DropDownItems.Clear();

			m_tsmiNoSort = null;
			m_tssSep0 = null;
			m_vColumns = null;
			m_tssSep1 = null;
			m_tsmiAsc = null;
			m_tsmiDesc = null;
		}

		private void UpdateMenu(object sender, EventArgs e)
		{
			if(m_lv == null) { Debug.Assert(false); return; }

			DeleteMenuItems();

			IComparer icSorter = m_lv.ListViewItemSorter;
			ListSorter ls = ((icSorter != null) ? (icSorter as ListSorter) : null);
			if(ls != null)
			{
				m_iCurSortColumn = ls.Column;
				m_bCurSortAsc = (ls.Order != SortOrder.Descending);
				if((ls.Order == SortOrder.None) || (m_iCurSortColumn >= m_lv.Columns.Count))
					m_iCurSortColumn = -1;
			}
			else m_iCurSortColumn = -1;

			m_tsmiNoSort = new ToolStripMenuItem(KPRes.NoSort);
			if(m_iCurSortColumn < 0) UIUtil.SetRadioChecked(m_tsmiNoSort, true);
			m_tsmiNoSort.Click += this.OnNoSort;
			m_tsmiMenu.DropDownItems.Add(m_tsmiNoSort);

			m_tssSep0 = new ToolStripSeparator();
			m_tsmiMenu.DropDownItems.Add(m_tssSep0);

			m_vColumns = new List<ToolStripMenuItem>();
			foreach(ColumnHeader ch in m_lv.Columns)
			{
				string strText = (ch.Text ?? string.Empty);
				strText = StrUtil.EncodeMenuText(strText);

				ToolStripMenuItem tsmi = new ToolStripMenuItem(strText);
				if(ch.Index == m_iCurSortColumn) UIUtil.SetRadioChecked(tsmi, true);
				tsmi.Click += this.OnSortColumn;

				m_vColumns.Add(tsmi);
				m_tsmiMenu.DropDownItems.Add(tsmi);
			}

			m_tssSep1 = new ToolStripSeparator();
			m_tsmiMenu.DropDownItems.Add(m_tssSep1);

			m_tsmiAsc = new ToolStripMenuItem(KPRes.Ascending);
			if((m_iCurSortColumn >= 0) && m_bCurSortAsc)
				UIUtil.SetRadioChecked(m_tsmiAsc, true);
			m_tsmiAsc.Click += this.OnSortAscDesc;
			if(m_iCurSortColumn < 0) m_tsmiAsc.Enabled = false;
			m_tsmiMenu.DropDownItems.Add(m_tsmiAsc);

			m_tsmiDesc = new ToolStripMenuItem(KPRes.Descending);
			if((m_iCurSortColumn >= 0) && !m_bCurSortAsc)
				UIUtil.SetRadioChecked(m_tsmiDesc, true);
			m_tsmiDesc.Click += this.OnSortAscDesc;
			if(m_iCurSortColumn < 0) m_tsmiDesc.Enabled = false;
			m_tsmiMenu.DropDownItems.Add(m_tsmiDesc);
		}

		private void OnNoSort(object sender, EventArgs e)
		{
			if(m_h == null) { Debug.Assert(false); return; }

			m_h(false, 0, null, true);
		}

		private void OnSortColumn(object sender, EventArgs e)
		{
			if(m_h == null) { Debug.Assert(false); return; }

			ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
			if(tsmi == null) { Debug.Assert(false); return; }

			for(int i = 0; i < m_vColumns.Count; ++i)
			{
				if(m_vColumns[i] == tsmi)
				{
					bool bAsc = m_bCurSortAsc;
					if(i == m_iCurSortColumn) bAsc = !bAsc; // Toggle

					m_h(true, i, bAsc ? SortOrder.Ascending : SortOrder.Descending, true);
					break;
				}
			}
		}

		private void OnSortAscDesc(object sender, EventArgs e)
		{
			if(m_h == null) { Debug.Assert(false); return; }
			if(m_iCurSortColumn < 0) { Debug.Assert(false); return; }

			ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
			if(tsmi == null) { Debug.Assert(false); return; }

			if((tsmi == m_tsmiAsc) && !m_bCurSortAsc)
				m_h(true, m_iCurSortColumn, SortOrder.Ascending, true);
			else if((tsmi == m_tsmiDesc) && m_bCurSortAsc)
				m_h(true, m_iCurSortColumn, SortOrder.Descending, true);
		}
	}
}
