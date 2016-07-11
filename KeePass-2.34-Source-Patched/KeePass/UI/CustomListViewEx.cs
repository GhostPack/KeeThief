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
using System.Threading;
using System.Diagnostics;

using KeePass.Native;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public sealed class CustomListViewEx : ListView
	{
		public CustomListViewEx() : base()
		{
			try { this.DoubleBuffered = true; }
			catch(Exception) { Debug.Assert(false); }
		}

		/* private Color m_clrPrev = Color.Black;
		private Point m_ptLast = new Point(-1, -1);
		protected override void OnMouseHover(EventArgs e)
		{
			if((m_ptLast.X >= 0) && (m_ptLast.X < this.Columns.Count) &&
				(m_ptLast.Y >= 0) && (m_ptLast.Y < this.Items.Count))
			{
				this.Items[m_ptLast.Y].SubItems[m_ptLast.X].ForeColor = m_clrPrev;
			}

			ListViewHitTestInfo lh = this.HitTest(this.PointToClient(Cursor.Position));
			if((lh.Item != null) && (lh.SubItem != null))
			{
				m_ptLast = new Point(lh.Item.SubItems.IndexOf(lh.SubItem),
					lh.Item.Index);
				m_clrPrev = lh.SubItem.ForeColor;

				lh.SubItem.ForeColor = Color.LightBlue;
			}

			base.OnMouseHover(e);
		} */

		/* protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			UnfocusGroupInSingleMode();
		}
		private void UnfocusGroupInSingleMode()
		{
			try
			{
				if(!WinUtil.IsAtLeastWindowsVista) return;
				if(KeePassLib.Native.NativeLib.IsUnix()) return;
				if(!this.ShowGroups) return;
				if(this.MultiSelect) return;

				const uint m = (NativeMethods.LVGS_FOCUSED | NativeMethods.LVGS_SELECTED);

				uint uGroups = (uint)this.Groups.Count;
				for(uint u = 0; u < uGroups; ++u)
				{
					int iGroupID;
					if(NativeMethods.GetGroupStateByIndex(this, u, m,
						out iGroupID) == m)
					{
						NativeMethods.SetGroupState(this, iGroupID, m,
							NativeMethods.LVGS_SELECTED);
						return;
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		} */

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(UIUtil.HandleCommonKeyEvent(e, true, this)) return;

			try { if(SkipGroupHeaderIfRequired(e)) return; }
			catch(Exception) { Debug.Assert(false); }

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if(UIUtil.HandleCommonKeyEvent(e, false, this)) return;

			base.OnKeyUp(e);
		}

		private bool SkipGroupHeaderIfRequired(KeyEventArgs e)
		{
			if(!UIUtil.GetGroupsEnabled(this)) return false;
			if(this.MultiSelect) return false;

			if(MonoWorkarounds.IsRequired(836428016)) return false;

			ListViewItem lvi = this.FocusedItem;
			if(lvi != null)
			{
				ListViewGroup g = lvi.Group;
				ListViewItem lviChangeTo = null;

				if((e.KeyCode == Keys.Up) && IsFirstLastItemInGroup(g, lvi, true))
					lviChangeTo = (GetNextLvi(g, true) ?? lvi); // '??' for top item
				else if((e.KeyCode == Keys.Down) && IsFirstLastItemInGroup(g, lvi, false))
					lviChangeTo = (GetNextLvi(g, false) ?? lvi); // '??' for bottom item

				if(lviChangeTo != null)
				{
					foreach(ListViewItem lviEnum in this.Items)
						lviEnum.Selected = false;

					EnsureVisible(lviChangeTo.Index);
					UIUtil.SetFocusedItem(this, lviChangeTo, true);

					UIUtil.SetHandled(e, true);
					return true;
				}
			}

			return false;
		}

		private static bool IsFirstLastItemInGroup(ListViewGroup g,
			ListViewItem lvi, bool bFirst)
		{
			if(g == null) { Debug.Assert(false); return false; }

			ListViewItemCollection c = g.Items;
			if(c.Count == 0) { Debug.Assert(false); return false; }

			return (bFirst ? (c[0] == lvi) : (c[c.Count - 1] == lvi));
		}

		private ListViewItem GetNextLvi(ListViewGroup gBaseExcl, bool bUp)
		{
			if(gBaseExcl == null) { Debug.Assert(false); return null; }

			int i = this.Groups.IndexOf(gBaseExcl);
			if(i < 0) { Debug.Assert(false); return null; }

			if(bUp)
			{
				--i;
				while(i >= 0)
				{
					ListViewGroup g = this.Groups[i];
					if(g.Items.Count > 0) return g.Items[g.Items.Count - 1];

					--i;
				}
			}
			else // Down
			{
				++i;
				int nGroups = this.Groups.Count;
				while(i < nGroups)
				{
					ListViewGroup g = this.Groups[i];
					if(g.Items.Count > 0) return g.Items[0];

					++i;
				}
			}

			return null;
		}
	}
}
