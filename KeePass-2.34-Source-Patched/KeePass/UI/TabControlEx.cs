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

/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace KeePass.UI
{
	public sealed class TabControlEx : TabControl
	{
		private Font m_fBold = null;

		public TabControlEx() : base()
		{
			m_fBold = FontUtil.CreateFont(this.Font, FontStyle.Bold);

			this.DrawMode = TabDrawMode.OwnerDrawFixed;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if(disposing) m_fBold.Dispose();
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			DrawItemEventArgs ev = e;

			if(this.SelectedIndex == e.Index)
				ev = new DrawItemEventArgs(e.Graphics, m_fBold, e.Bounds,
					e.Index, e.State);

			e.DrawBackground();

			base.OnDrawItem(ev);
		}
	}
}
*/
