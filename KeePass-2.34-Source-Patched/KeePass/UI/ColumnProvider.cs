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

using KeePassLib;

namespace KeePass.UI
{
	public abstract class ColumnProvider
	{
		/// <summary>
		/// Names of all provided columns.
		/// Querying this property should be fast, i.e. it's recommended that
		/// you cache the returned string array.
		/// </summary>
		public abstract string[] ColumnNames { get; }

		public virtual HorizontalAlignment TextAlign
		{
			get { return HorizontalAlignment.Left; }
		}

		public abstract string GetCellData(string strColumnName, PwEntry pe);

		public virtual bool SupportsCellAction(string strColumnName)
		{
			return false;
		}

		/// <summary>
		/// KeePass calls this method when a user double-clicks onto a cell
		/// of a column provided by the plugin.
		/// This method is only called, if the provider returns <c>true</c>
		/// in the <c>SupportsCellAction</c> method for the specified column.
		/// </summary>
		/// <param name="strColumnName">Name of the active column.</param>
		/// <param name="pe">Entry to which the active cell belongs.</param>
		public virtual void PerformCellAction(string strColumnName, PwEntry pe)
		{
		}
	}
}
