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
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

using KeePass.Resources;
using KeePassLib.Utility;

namespace KeePass.UI
{
	public sealed class ListSorter : IComparer
	{
		// Cached version of a string representing infinity
		private readonly string m_strNeverExpires;

		private int m_nColumn = -1;
		[DefaultValue(-1)]
		public int Column
		{
			get { return m_nColumn; }

			/// Only provided for XML serialization, do not use
			set { m_nColumn = value; }
		}

		private SortOrder m_oSort = SortOrder.Ascending;
		public SortOrder Order
		{
			get { return m_oSort; }

			/// Only provided for XML serialization, do not use
			set { m_oSort = value; }
		}

		private bool m_bCompareNaturally = true;
		[DefaultValue(true)]
		public bool CompareNaturally
		{
			get { return m_bCompareNaturally; }

			/// Only provided for XML serialization, do not use
			set { m_bCompareNaturally = value; }
		}

		private bool m_bCompareTimes = false;
		[DefaultValue(false)]
		public bool CompareTimes
		{
			get { return m_bCompareTimes; }

			/// Only provided for XML serialization, do not use
			set { m_bCompareTimes = value; }
		}

		public ListSorter()
		{
			m_strNeverExpires = KPRes.NeverExpires;
		}

		public ListSorter(int nColumn, SortOrder sortOrder, bool bCompareNaturally,
			bool bCompareTimes)
		{
			m_strNeverExpires = KPRes.NeverExpires;

			m_nColumn = nColumn;

			Debug.Assert(sortOrder != SortOrder.None);
			if(sortOrder != SortOrder.None) m_oSort = sortOrder;

			m_bCompareNaturally = bCompareNaturally;
			m_bCompareTimes = bCompareTimes;
		}

		public int Compare(object x, object y)
		{
			bool bSwap = (m_oSort != SortOrder.Ascending);
			ListViewItem lviX = (bSwap ? (ListViewItem)y : (ListViewItem)x);
			ListViewItem lviY = (bSwap ? (ListViewItem)x : (ListViewItem)y);
			string strL, strR;

			Debug.Assert(lviX.SubItems.Count == lviY.SubItems.Count);
			if((m_nColumn <= 0) || (lviX.SubItems.Count <= m_nColumn) ||
				(lviY.SubItems.Count <= m_nColumn))
			{
				strL = lviX.Text;
				strR = lviY.Text;
			}
			else
			{
				strL = lviX.SubItems[m_nColumn].Text;
				strR = lviY.SubItems[m_nColumn].Text;
			}

			if(m_bCompareTimes)
			{
				if((strL == m_strNeverExpires) || (strR == m_strNeverExpires))
					return strL.CompareTo(strR);

				DateTime dtL = TimeUtil.FromDisplayString(strL);
				DateTime dtR = TimeUtil.FromDisplayString(strR);
				return dtL.CompareTo(dtR);
			}

			if(m_bCompareNaturally) return StrUtil.CompareNaturally(strL, strR);
			return string.Compare(strL, strR, true);
		}
	}
}
