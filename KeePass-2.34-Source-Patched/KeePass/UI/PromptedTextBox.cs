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
using System.ComponentModel;
using System.Drawing;

namespace KeePass.UI
{
	public sealed class PromptedTextBox : TextBox
	{
		private const int WM_PAINT = 0x000F;

		private string m_strPrompt = string.Empty;
		[DefaultValue("")]
		public string PromptText
		{
			get { return m_strPrompt; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");

				m_strPrompt = value;
				this.Invalidate();
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if((m.Msg == WM_PAINT) && !this.Focused && (this.Text.Length == 0) &&
				(m_strPrompt.Length > 0))
			{
				TextFormatFlags tff = (TextFormatFlags.EndEllipsis |
					TextFormatFlags.NoPrefix | TextFormatFlags.Left |
					TextFormatFlags.Top | TextFormatFlags.NoPadding);

				using(Graphics g = this.CreateGraphics())
				{
					Rectangle rect = this.ClientRectangle;
					rect.Offset(1, 1);

					TextRenderer.DrawText(g, m_strPrompt, this.Font,
						rect, SystemColors.GrayText, this.BackColor, tff);
				}
			}
		}
	}
}
