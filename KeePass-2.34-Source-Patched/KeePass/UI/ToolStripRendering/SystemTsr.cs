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

using KeePass.Resources;

using KeePassLib;

namespace KeePass.UI.ToolStripRendering
{
	internal sealed class SystemTsrFactory : TsrFactory
	{
		private PwUuid m_uuid = new PwUuid(new byte[] {
			0x6B, 0xCD, 0x45, 0xFA, 0xA1, 0x3F, 0x71, 0xEC,
			0x7B, 0x5E, 0x97, 0x38, 0x8D, 0xB1, 0xCB, 0x09
		});

		public override PwUuid Uuid
		{
			get { return m_uuid; }
		}

		public override string Name
		{
			get { return (KPRes.System + " - " + KPRes.ClassicAdj); }
		}

		public override ToolStripRenderer CreateInstance()
		{
			return new SystemTsr();
		}
	}

	// Checkboxes are rendered incorrectly
	/* internal sealed class SystemTsrFactory : TsrFactory
	{
		private PwUuid m_uuid = new PwUuid(new byte[] {
			0x03, 0xF8, 0x67, 0xAB, 0x21, 0x96, 0x43, 0xED,
			0xA5, 0xFE, 0x9E, 0x43, 0x4A, 0x35, 0x89, 0xAA
		});

		public override PwUuid Uuid
		{
			get { return m_uuid; }
		}

		public override string Name
		{
			get { return (KPRes.System + " - " + KPRes.ClassicAdj); }
		}

		public override ToolStripRenderer CreateInstance()
		{
			return new ToolStripSystemRenderer();
		}
	} */

	internal sealed class SystemTsr : ProExtTsr
	{
		private sealed class SystemTsrColorTable : ProfessionalColorTable
		{
			private Color m_clrItemActiveBack = SystemColors.Control;
			private Color m_clrItemActiveBorder = SystemColors.ControlDarkDark;
			private Color m_clrItemSelBack = SystemColors.Control;
			private Color m_clrItemSelBorder = SystemColors.ControlDark;

			private Color m_clrBarBack = SystemColors.MenuBar;
			private Color m_clrMenuBack = SystemColors.Menu;
			private Color m_clrImageBack = SystemColors.Menu;

			private Color m_clrSubItemSelBack = SystemColors.MenuHighlight;
			private Color m_clrSubItemSelBorder = SystemColors.MenuHighlight;

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
				get { return m_clrMenuBack; }
			}

			public override Color CheckPressedBackground
			{
				get { return m_clrMenuBack; }
			}

			public override Color CheckSelectedBackground
			{
				get { return m_clrMenuBack; }
			}

			public override Color GripDark
			{
				get { return SystemColors.ControlDark; }
			}

			public override Color GripLight
			{
				get { return SystemColors.ControlLight; }
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
				get { return SystemColors.ActiveBorder; }
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
				get { return m_clrMenuBack; }
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
				get { return SystemColors.ControlLight; }
			}

			public override Color OverflowButtonGradientEnd
			{
				get { return SystemColors.ControlDark; }
			}

			public override Color OverflowButtonGradientMiddle
			{
				get { return SystemColors.Control; }
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
				// SeparatorDark is used for both the menu and the toolbar
				get { return SystemColors.ControlDark; }
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

		protected override bool EnsureTextContrast
		{
			get { return false; } // Prevent color override by base class
		}

		public SystemTsr() : base(new SystemTsrColorTable())
		{
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if(e != null)
			{
				ToolStripItem tsi = e.Item;
				if(tsi != null)
				{
					bool bDropDown = (tsi.OwnerItem != null);
					bool bCtxMenu = (tsi.Owner is ContextMenuStrip);

					Color clr = tsi.ForeColor;

					if(!tsi.Enabled && !tsi.Selected)
					{
						if(!UIUtil.IsHighContrast)
						{
							// Draw light "shadow"
							Rectangle r = e.TextRectangle;
							int dx = DpiUtil.ScaleIntX(128) / 128; // Force floor
							int dy = DpiUtil.ScaleIntY(128) / 128; // Force floor
							r.Offset(dx, dy);
							TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont,
								r, SystemColors.HighlightText, e.TextFormat);
						}

						clr = SystemColors.GrayText;
					}
					else if(tsi.Selected && (bDropDown || bCtxMenu))
						clr = SystemColors.HighlightText;
					else if(clr.ToArgb() == Control.DefaultForeColor.ToArgb())
						clr = SystemColors.MenuText;
					else
					{
						bool bDarkBack = this.IsDarkStyle;
						bool bDarkText = UIUtil.IsDarkColor(clr);

						if((bDarkBack && bDarkText) || (!bDarkBack && !bDarkText))
						{
							Debug.Assert(false);
							clr = SystemColors.MenuText;
						}
					}

					e.TextColor = clr;
				}
			}
			else { Debug.Assert(false); }

			base.OnRenderItemText(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			ToolStripItem tsi = ((e != null) ? e.Item : null);
			bool bCtxItem = ((tsi != null) ? (tsi.Owner is ContextMenuStrip) : false);

			if((tsi != null) && (bCtxItem || (tsi.OwnerItem != null)) &&
				tsi.Selected && !tsi.Enabled)
			{
				Rectangle rect = tsi.ContentRectangle;
				rect.Offset(0, -1);
				rect.Height += 1;

				Color clrBack = this.ColorTable.MenuItemSelected;
				Color clrBorder = clrBack;

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
			/* else if((tsi != null) && !bCtxItem && (tsi.OwnerItem == null) &&
				(tsi.Selected || tsi.Pressed))
			{
				Rectangle rect = tsi.ContentRectangle;
				rect.Offset(0, -1);
				rect.Height += 1;

				// ButtonState bs = (tsi.Pressed ? ButtonState.Pushed : ButtonState.Normal);
				// if(!tsi.Enabled) bs = ButtonState.Inactive;
				// ControlPaint.DrawButton(e.Graphics, rect, bs);
				DrawButton(e.Graphics, rect, tsi.Pressed);
			} */
			else base.OnRenderMenuItemBackground(e);
		}

		/* private static void DrawButton(Graphics g, Rectangle r, bool bPressed)
		{
			using(SolidBrush brBack = new SolidBrush(SystemColors.Control))
			{
				g.FillRectangle(brBack, r);
			}

			Color clrTL = (bPressed ? SystemColors.ButtonShadow : SystemColors.ButtonHighlight);
			Color clrBR = (bPressed ? SystemColors.ButtonHighlight : SystemColors.ButtonShadow);
			int x1 = r.Left + 2, y1 = r.Top;
			int x2 = r.Right - 3, y2 = r.Bottom;

			using(Pen pTL = new Pen(clrTL))
			{
				g.DrawLine(pTL, x1, y1, x2, y1);
				g.DrawLine(pTL, x1, y1, x1, y2);
			}

			using(Pen pBR = new Pen(clrBR))
			{
				g.DrawLine(pBR, x1, y2, x2, y2);
				g.DrawLine(pBR, x2, y1, x2, y2);
			}
		} */
	}
}
