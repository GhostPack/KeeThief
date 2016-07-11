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

using KeePassLib;

namespace KeePass.UI
{
	public sealed class ColumnProviderPool : IEnumerable<ColumnProvider>
	{
		private List<ColumnProvider> m_vProviders = new List<ColumnProvider>();

		public int Count
		{
			get { return m_vProviders.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_vProviders.GetEnumerator();
		}

		public IEnumerator<ColumnProvider> GetEnumerator()
		{
			return m_vProviders.GetEnumerator();
		}

		public void Add(ColumnProvider prov)
		{
			Debug.Assert(prov != null); if(prov == null) throw new ArgumentNullException("prov");

			m_vProviders.Add(prov);
		}

		public bool Remove(ColumnProvider prov)
		{
			Debug.Assert(prov != null); if(prov == null) throw new ArgumentNullException("prov");

			return m_vProviders.Remove(prov);
		}

		public string[] GetColumnNames()
		{
			List<string> v = new List<string>();

			foreach(ColumnProvider prov in m_vProviders)
			{
				foreach(string strColumn in prov.ColumnNames)
				{
					if(!v.Contains(strColumn)) v.Add(strColumn);
				}
			}

			return v.ToArray();
		}

		public HorizontalAlignment GetTextAlign(string strColumnName)
		{
			if(strColumnName == null) throw new ArgumentNullException("strColumnName");

			foreach(ColumnProvider prov in m_vProviders)
			{
				if(Array.IndexOf<string>(prov.ColumnNames, strColumnName) >= 0)
					return prov.TextAlign;
			}

			return HorizontalAlignment.Left;
		}

		public string GetCellData(string strColumnName, PwEntry pe)
		{
			if(strColumnName == null) throw new ArgumentNullException("strColumnName");
			if(pe == null) throw new ArgumentNullException("pe");

			foreach(ColumnProvider prov in m_vProviders)
			{
				if(Array.IndexOf<string>(prov.ColumnNames, strColumnName) >= 0)
					return prov.GetCellData(strColumnName, pe);
			}

			return string.Empty;
		}

		public bool SupportsCellAction(string strColumnName)
		{
			if(strColumnName == null) throw new ArgumentNullException("strColumnName");

			foreach(ColumnProvider prov in m_vProviders)
			{
				if(Array.IndexOf<string>(prov.ColumnNames, strColumnName) >= 0)
					return prov.SupportsCellAction(strColumnName);
			}

			return false;
		}

		public void PerformCellAction(string strColumnName, PwEntry pe)
		{
			if(strColumnName == null) throw new ArgumentNullException("strColumnName");

			foreach(ColumnProvider prov in m_vProviders)
			{
				if(Array.IndexOf<string>(prov.ColumnNames, strColumnName) >= 0)
				{
					prov.PerformCellAction(strColumnName, pe);
					break;
				}
			}
		}
	}
}
