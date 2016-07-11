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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using KeePassLib;

namespace KeePass.UI.ToolStripRendering
{
	internal sealed class Win81TsrFactory : TsrFactory
	{
		private PwUuid m_uuid = new PwUuid(new byte[] {
			0xEF, 0x5B, 0x4B, 0xE8, 0x49, 0xD1, 0x5E, 0x71,
			0x65, 0xE0, 0x26, 0x3B, 0x03, 0xBD, 0x8C, 0x3B
		});

		public override PwUuid Uuid
		{
			get { return m_uuid; }
		}

		public override string Name
		{
			get { return "Windows 8.1"; }
		}

		public override bool IsSupported()
		{
			return !UIUtil.IsHighContrast;
		}

		public override ToolStripRenderer CreateInstance()
		{
			return new Win81Tsr();
		}
	}

	internal sealed class Win81Tsr : ProExtTsr
	{
		private sealed class Win81TsrColorTable : ProfessionalColorTable
		{
			private Color m_clrItemActiveBack = Color.FromArgb(184, 216, 249);
			private Color m_clrItemActiveBorder = Color.FromArgb(98, 163, 229);
			private Color m_clrItemSelBack = Color.FromArgb(213, 231, 248);
			private Color m_clrItemSelBorder = Color.FromArgb(122, 177, 232);

			private Color m_clrBarBack = Color.FromArgb(245, 246, 247);
			private Color m_clrMenuBack = Color.FromArgb(240, 240, 240);
			private Color m_clrImageBack = Color.FromArgb(240, 240, 240);

			private Color m_clrSubItemSelBack = Color.FromArgb(209, 226, 242);
			private Color m_clrSubItemSelBorder = Color.FromArgb(120, 174, 229);

			public override Color ButtonCheckedGradientBegin
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color ButtonCheckedGradientEnd
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color ButtonCheckedGradientMiddle
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color ButtonCheckedHighlight
			{
				get { return m_clrItemActiveBack; } // Untested
			}

			public override Color ButtonCheckedHighlightBorder
			{
				get { return m_clrItemActiveBorder; } // Untested
			}

			public override Color ButtonPressedBorder
			{
				get { return m_clrItemActiveBorder; }
			}

			public override Color ButtonPressedGradientBegin
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color ButtonPressedGradientEnd
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color ButtonPressedGradientMiddle
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color ButtonPressedHighlight
			{
				get { return m_clrItemActiveBack; } // Untested
			}

			public override Color ButtonPressedHighlightBorder
			{
				get { return m_clrItemActiveBorder; } // Untested
			}

			public override Color ButtonSelectedBorder
			{
				get { return m_clrItemSelBorder; }
			}

			public override Color ButtonSelectedGradientBegin
			{
				get { return m_clrItemSelBack; }
			}

			public override Color ButtonSelectedGradientEnd
			{
				get { return m_clrItemSelBack; }
			}

			public override Color ButtonSelectedGradientMiddle
			{
				get { return m_clrItemSelBack; }
			}

			public override Color ButtonSelectedHighlight
			{
				get { return m_clrItemSelBack; } // Untested
			}

			public override Color ButtonSelectedHighlightBorder
			{
				get { return m_clrItemSelBorder; }
			}

			public override Color CheckBackground
			{
				get { return Color.FromArgb(192, 221, 235); }
			}

			public override Color CheckPressedBackground
			{
				get { return Color.FromArgb(168, 210, 236); }
			}

			public override Color CheckSelectedBackground
			{
				get { return Color.FromArgb(168, 210, 236); }
			}

			public override Color GripDark
			{
				get { return Color.FromArgb(187, 188, 189); }
			}

			public override Color GripLight
			{
				get { return Color.FromArgb(252, 252, 253); }
			}

			public override Color ImageMarginGradientBegin
			{
				get { return m_clrImageBack; }
			}

			public override Color ImageMarginGradientEnd
			{
				get { return m_clrImageBack; }
			}

			public override Color ImageMarginGradientMiddle
			{
				get { return m_clrImageBack; }
			}

			public override Color ImageMarginRevealedGradientBegin
			{
				get { return m_clrImageBack; }
			}

			public override Color ImageMarginRevealedGradientEnd
			{
				get { return m_clrImageBack; }
			}

			public override Color ImageMarginRevealedGradientMiddle
			{
				get { return m_clrImageBack; }
			}

			public override Color MenuBorder
			{
				get { return Color.FromArgb(151, 151, 151); }
			}

			public override Color MenuItemBorder
			{
				get { return m_clrSubItemSelBorder; }
			}

			public override Color MenuItemSelected
			{
				get { return m_clrSubItemSelBack; }
			}

			public override Color MenuItemPressedGradientBegin
			{
				// Used by pressed root menu items and inactive drop-down
				// arrow buttons in toolbar comboboxes (?!)
				get { return m_clrItemActiveBack; }
			}

			public override Color MenuItemPressedGradientEnd
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color MenuItemPressedGradientMiddle
			{
				get { return m_clrItemActiveBack; }
			}

			public override Color MenuItemSelectedGradientBegin
			{
				get { return m_clrItemSelBack; }
			}

			public override Color MenuItemSelectedGradientEnd
			{
				get { return m_clrItemSelBack; }
			}

			public override Color MenuStripGradientBegin
			{
				get { return m_clrBarBack; }
			}

			public override Color MenuStripGradientEnd
			{
				get { return m_clrBarBack; }
			}

			public override Color OverflowButtonGradientBegin
			{
				get { return Color.FromArgb(245, 245, 245); }
			}

			public override Color OverflowButtonGradientEnd
			{
				get { return Color.FromArgb(229, 229, 229); }
			}

			public override Color OverflowButtonGradientMiddle
			{
				get { return Color.FromArgb(237, 237, 237); }
			}

			public override Color RaftingContainerGradientBegin
			{
				get { return m_clrMenuBack; } // Untested
			}

			public override Color RaftingContainerGradientEnd
			{
				get { return m_clrMenuBack; } // Untested
			}

			public override Color SeparatorDark
			{
				// Menu separators are (215, 215, 215),
				// toolbar separators are (135, 135, 136);
				// SeparatorDark is used for both the menu and the toolbar
				get { return Color.FromArgb(175, 175, 175); }
			}

			public override Color SeparatorLight
			{
				get { return Color.FromArgb(248, 249, 249); }
			}

			public override Color StatusStripGradientBegin
			{
				get { return m_clrMenuBack; }
			}

			public override Color StatusStripGradientEnd
			{
				get { return m_clrMenuBack; }
			}

			public override Color ToolStripBorder
			{
				get { return m_clrMenuBack; }
			}

			public override Color ToolStripContentPanelGradientBegin
			{
				get { return m_clrMenuBack; } // Untested
			}

			public override Color ToolStripContentPanelGradientEnd
			{
				get { return m_clrMenuBack; } // Untested
			}

			public override Color ToolStripDropDownBackground
			{
				get { return m_clrMenuBack; }
			}

			public override Color ToolStripGradientBegin
			{
				get { return m_clrBarBack; }
			}

			public override Color ToolStripGradientEnd
			{
				get { return m_clrBarBack; }
			}

			public override Color ToolStripGradientMiddle
			{
				get { return m_clrBarBack; }
			}

			public override Color ToolStripPanelGradientBegin
			{
				get { return m_clrBarBack; } // Untested
			}

			public override Color ToolStripPanelGradientEnd
			{
				get { return m_clrBarBack; } // Untested
			}
		}

		public Win81Tsr() : base(new Win81TsrColorTable())
		{
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			ToolStripItem tsi = ((e != null) ? e.Item : null);

			if((tsi != null) && ((tsi.Owner is ContextMenuStrip) ||
				(tsi.OwnerItem != null)) && tsi.Selected && !tsi.Enabled)
			{
				Rectangle rect = tsi.ContentRectangle;
				rect.Offset(0, -1);
				rect.Height += 1;

				Color clrBack = Color.FromArgb(225, 225, 225);
				Color clrBorder = Color.FromArgb(174, 174, 174);

				SolidBrush br = new SolidBrush(clrBack);
				Pen p = new Pen(clrBorder);

				Graphics g = e.Graphics;
				if(g != null)
				{
					g.FillRectangle(br, rect);
					g.DrawRectangle(p, rect);
				}
				else { Debug.Assert(false); }

				p.Dispose();
				br.Dispose();
			}
			else base.OnRenderMenuItemBackground(e);
		}
	}
}
