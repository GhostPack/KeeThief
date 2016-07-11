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
using System.Diagnostics;

namespace KeePass.UI
{
	public sealed class KblButton
	{
		public string Text = null;
		public string Command = null;

		public Rectangle Position = Rectangle.Empty;

		public KblButton() { }
		public KblButton(string strText, string strCommand)
		{
			this.Text = strText;
			this.Command = strCommand;
		}
	}

	public sealed class KbcLayout
	{
		public List<List<KblButton>> Rows = new List<List<KblButton>>();

		public static KbcLayout Default
		{
			get
			{
				KbcLayout l = new KbcLayout();

				List<KblButton> r = new List<KblButton>();
				r.Add(new KblButton("1", null));
				r.Add(new KblButton("2", null));
				r.Add(new KblButton("3", null));
				r.Add(new KblButton("4", null));
				r.Add(new KblButton("5", null));
				r.Add(new KblButton("6", null));
				r.Add(new KblButton("7", null));
				r.Add(new KblButton("8", null));
				r.Add(new KblButton("9", null));
				r.Add(new KblButton("0", null));
				l.Rows.Add(r);

				return l;
			}
		}
	}

	public sealed class KeyboardControl : Control
	{
		private KbcLayout m_l = null;
		private int m_iLayoutW = -1, m_iLayoutH = -1;

		private CustomToolStripRendererEx m_renderer = new CustomToolStripRendererEx();

		public void InitEx(KbcLayout l)
		{
			m_l = l;
		}

		private void ComputeLayoutEx()
		{
			if(m_l == null) { Debug.Assert(false); return; }

			int w = this.ClientSize.Width, h = this.ClientSize.Height;

			int nRows = m_l.Rows.Count;
			float fBtnH = (float)h / (float)(nRows + 1);
			float fBtnHSkip = ((float)h - (fBtnH * (float)nRows)) /
				(float)(nRows + 1);
			float y = 0.0f;

			for(int iy = 0; iy < nRows; ++iy)
			{
				y += fBtnHSkip;

				int nColumns = m_l.Rows[iy].Count;
				float fBtnW = (float)w / (nColumns + 1);
				float fBtnWSkip = ((float)w - (fBtnW * (float)nColumns)) /
					(float)(nColumns + 1);
				float x = 0.0f;

				for(int ix = 0; ix < nColumns; ++ix)
				{
					x += fBtnWSkip;

					m_l.Rows[iy][ix].Position = new Rectangle((int)(x + 0.00001f),
						(int)(y + 0.00001f), (int)fBtnW, (int)fBtnH);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e); // Required

			if(m_l == null) return;

			Size szCli = this.ClientSize;
			if((m_iLayoutW != szCli.Width) || (m_iLayoutH != szCli.Height))
				ComputeLayoutEx();

			Graphics g = e.Graphics;
			Rectangle rectClip = e.ClipRectangle;
			foreach(List<KblButton> vRow in m_l.Rows)
			{
				foreach(KblButton btn in vRow)
				{
					if(btn.Position.IntersectsWith(rectClip))
						DrawButtonEx(btn, g);
				}
			}
		}

		private void DrawButtonEx(KblButton btn, Graphics g)
		{
			string strText = ((btn.Text ?? btn.Command) ?? string.Empty);

			ToolStripItemRenderEventArgs e = new ToolStripItemRenderEventArgs(g,
				new ToolStripButton(strText));
			m_renderer.DrawMenuItemBackground(e);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			using(SolidBrush brush = new SolidBrush(SystemColors.Control))
			{
				pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
			}
		}
	}
}
*/
