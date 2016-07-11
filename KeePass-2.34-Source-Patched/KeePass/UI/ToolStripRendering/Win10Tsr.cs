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
	internal sealed class Win10TsrFactory : TsrFactory
	{
		private PwUuid m_uuid = new PwUuid(new byte[] {
			0x39, 0xE5, 0x05, 0x04, 0xB6, 0x56, 0x14, 0xE7,
			0x4F, 0x13, 0x68, 0x51, 0x85, 0xB3, 0x87, 0xC6
		});

		public override PwUuid Uuid
		{
			get { return m_uuid; }
		}

		public override string Name
		{
			get { return "Windows 10"; } // Version 1511
		}

		public override bool IsSupported()
		{
			return !UIUtil.IsHighContrast;
		}

		public override ToolStripRenderer CreateInstance()
		{
			return new Win10Tsr();
		}
	}

	internal sealed class Win10Tsr : ProExtTsr
	{
		private sealed class Win10TsrColorTable : ProfessionalColorTable
		{
			private Color m_clrItemActiveBack = Color.FromArgb(204, 232, 255);
			private Color m_clrItemActiveBorder = Color.FromArgb(153, 209, 255);
			private Color m_clrItemSelBack = Color.FromArgb(229, 243, 255);
			private Color m_clrItemSelBorder = Color.FromArgb(204, 232, 255);

			private Color m_clrBarBack = Color.FromArgb(255, 255, 255);
			private Color m_clrMenuBack = Color.FromArgb(242, 242, 242);
			private Color m_clrImageBack = Color.FromArgb(240, 240, 240);

			private Color m_clrSubItemSelBack = Color.FromArgb(145, 201, 247);
			private Color m_clrSubItemSelBorder = Color.FromArgb(145, 201, 247);

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
				get { return Color.FromArgb(144, 200, 246); }
			}

			public override Color CheckPressedBackground
			{
				get { return Color.FromArgb(86, 176, 250); }
			}

			public override Color CheckSelectedBackground
			{
				get { return Color.FromArgb(86, 176, 250); }
			}

			public override Color GripDark
			{
				get { return Color.FromArgb(195, 195, 195); }
			}

			public override Color GripLight
			{
				get { return Color.FromArgb(228, 228, 228); }
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
				get { return Color.FromArgb(204, 204, 204); }
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
				// toolbar separators are (140, 140, 140);
				// SeparatorDark is used for both the menu and the toolbar
				get { return Color.FromArgb(177, 177, 177); }
			}

			public override Color SeparatorLight
			{
				get { return m_clrBarBack; }
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

		public Win10Tsr() : base(new Win10TsrColorTable())
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

				// In image area (228, 228, 228), in text area (230, 230, 230)
				Color clrBack = Color.FromArgb(229, 229, 229);
				Color clrBorder = Color.FromArgb(229, 229, 229);

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
