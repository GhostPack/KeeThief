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

namespace KeePass.UI
{
	internal sealed class ListViewStateEx
	{
		public readonly int[] ColumnWidths;

		public ListViewStateEx(ListView lv)
		{
			if(lv == null) throw new ArgumentNullException("lv");

			this.ColumnWidths = new int[lv.Columns.Count];
			for(int iColumn = 0; iColumn < lv.Columns.Count; ++iColumn)
				this.ColumnWidths[iColumn] = lv.Columns[iColumn].Width;
		}

		public bool CompareTo(ListView lv)
		{
			if(lv == null) throw new ArgumentNullException("lv");

			if(lv.Columns.Count != this.ColumnWidths.Length) return false;
			for(int iColumn = 0; iColumn < this.ColumnWidths.Length; ++iColumn)
			{
				if(lv.Columns[iColumn].Width != this.ColumnWidths[iColumn])
					return false;
			}

			return true;
		}
	}
}
