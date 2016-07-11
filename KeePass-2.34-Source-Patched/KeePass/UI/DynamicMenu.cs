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
using System.Drawing;
using System.Diagnostics;

namespace KeePass.UI
{
	public sealed class DynamicMenuEventArgs : EventArgs
	{
		private string m_strItemName = string.Empty;
		private object m_objTag = null;

		public string ItemName
		{
			get { return m_strItemName; }
		}

		public object Tag
		{
			get { return m_objTag; }
		}

		public DynamicMenuEventArgs(string strItemName, object objTag)
		{
			Debug.Assert(strItemName != null);
			if(strItemName == null) throw new ArgumentNullException("strItemName");

			m_strItemName = strItemName;
			m_objTag = objTag;
		}
	}

	public sealed class DynamicMenu
	{
		private ToolStripItemCollection m_tsicHost;
		private List<ToolStripItem> m_vMenuItems = new List<ToolStripItem>();

		public event EventHandler<DynamicMenuEventArgs> MenuClick;

		// Constructor required by plugins
		public DynamicMenu(ToolStripDropDownItem tsmiHost)
		{
			Debug.Assert(tsmiHost != null);
			if(tsmiHost == null) throw new ArgumentNullException("tsmiHost");

			m_tsicHost = tsmiHost.DropDownItems;
		}

		public DynamicMenu(ToolStripItemCollection tsicHost)
		{
			Debug.Assert(tsicHost != null);
			if(tsicHost == null) throw new ArgumentNullException("tsicHost");

			m_tsicHost = tsicHost;
		}

		~DynamicMenu()
		{
			Clear();
		}

		public void Clear()
		{
			for(int i = 0; i < m_vMenuItems.Count; ++i)
			{
				ToolStripItem tsi = m_vMenuItems[m_vMenuItems.Count - i - 1];

				if(tsi is ToolStripMenuItem)
					tsi.Click -= this.OnMenuClick;

				m_tsicHost.Remove(tsi);
			}
			m_vMenuItems.Clear();
		}

		public ToolStripMenuItem AddItem(string strItemText, Image imgSmallIcon)
		{
			return AddItem(strItemText, imgSmallIcon, null);
		}

		public ToolStripMenuItem AddItem(string strItemText, Image imgSmallIcon,
			object objTag)
		{
			Debug.Assert(strItemText != null);
			if(strItemText == null) throw new ArgumentNullException("strItemText");

			ToolStripMenuItem tsmi = new ToolStripMenuItem(strItemText);
			tsmi.Click += this.OnMenuClick;
			tsmi.Tag = objTag;

			if(imgSmallIcon != null) tsmi.Image = imgSmallIcon;

			m_tsicHost.Add(tsmi);
			m_vMenuItems.Add(tsmi);
			return tsmi;
		}

		public void AddSeparator()
		{
			ToolStripSeparator sep = new ToolStripSeparator();

			m_tsicHost.Add(sep);
			m_vMenuItems.Add(sep);
		}

		private void OnMenuClick(object sender, EventArgs e)
		{
			ToolStripItem tsi = (sender as ToolStripItem);
			Debug.Assert(tsi != null); if(tsi == null) return;

			string strText = tsi.Text;
			Debug.Assert(strText != null); if(strText == null) return;

			DynamicMenuEventArgs args = new DynamicMenuEventArgs(strText, tsi.Tag);
			if(this.MenuClick != null) this.MenuClick(sender, args);
		}
	}
}
