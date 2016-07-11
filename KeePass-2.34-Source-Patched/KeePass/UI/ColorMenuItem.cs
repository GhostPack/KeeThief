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
	public sealed class ColorMenuItem : MenuItem
	{
		private Color m_clr;
		private int m_qSize;

		public Color Color
		{
			get { return m_clr; }
		}

		public ColorMenuItem(Color clr, int qSize) : base()
		{
			m_clr = clr;
			m_qSize = qSize;

			Debug.Assert(this.CanRaiseEvents);
			this.ShowShortcut = false;
			this.OwnerDraw = true;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			// base.OnDrawItem(e);

			Graphics g = e.Graphics;
			Rectangle rectBounds = e.Bounds;
			Rectangle rectFill = new Rectangle(rectBounds.Left + 2,
				rectBounds.Top + 2, rectBounds.Width - 4, rectBounds.Height - 4);

			bool bFocused = (((e.State & DrawItemState.Focus) != DrawItemState.None) ||
				((e.State & DrawItemState.Selected) != DrawItemState.None));

			// e.DrawBackground();
			// e.DrawFocusRectangle();
			using(SolidBrush sbBack = new SolidBrush(bFocused ?
				SystemColors.Highlight : SystemColors.Menu))
			{
				g.FillRectangle(sbBack, rectBounds);
			}

			using(SolidBrush sb = new SolidBrush(m_clr))
			{
				g.FillRectangle(sb, rectFill);
			}
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			// base.OnMeasureItem(e);

			e.ItemWidth = m_qSize;
			e.ItemHeight = m_qSize;
		}
	}
}
