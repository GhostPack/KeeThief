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
using KeePassLib.Utility;

namespace KeePass.UI.ToolStripRendering
{
	internal sealed class KeePassTsrFactory : TsrFactory
	{
		private PwUuid m_uuid = new PwUuid(new byte[] {
			0x05, 0x0A, 0x57, 0xF0, 0x7B, 0xBC, 0x34, 0xAF,
			0x5B, 0x8F, 0xA1, 0x31, 0xDB, 0xBF, 0x2B, 0xEC
		});

		public override PwUuid Uuid
		{
			get { return m_uuid; }
		}

		public override string Name
		{
			get { return (PwDefs.ShortProductName + " - " + KPRes.Gradient); }
		}

		public override bool IsSupported()
		{
			// bool bVisualStyles = true;
			// try { bVisualStyles = VisualStyleRenderer.IsSupported; }
			// catch(Exception) { Debug.Assert(false); bVisualStyles = false; }

			// Various drawing bugs under Mono (gradients too light, incorrect
			// painting of popup menus, paint method not invoked for disabled
			// items, ...)
			bool bMono = MonoWorkarounds.IsRequired();

			return (!UIUtil.IsHighContrast && !bMono);
		}

		public override ToolStripRenderer CreateInstance()
		{
			return new KeePassTsr();
		}
	}

	internal sealed class KeePassTsr : ProExtTsr
	{
		private sealed class KeePassTsrColorTable : ProfessionalColorTable
		{
			private const double m_dblLight = 0.75;
			private const double m_dblDark = 0.05;

			internal static Color StartGradient(Color clr)
			{
				return UIUtil.LightenColor(clr, m_dblLight);
			}

			internal static Color EndGradient(Color clr)
			{
				return UIUtil.DarkenColor(clr, m_dblDark);
			}

			public override Color ButtonPressedGradientBegin
			{
				get { return StartGradient(this.ButtonPressedGradientMiddle); }
			}

			public override Color ButtonPressedGradientEnd
			{
				get { return EndGradient(this.ButtonPressedGradientMiddle); }
			}

			public override Color ButtonSelectedGradientBegin
			{
				get { return StartGradient(this.ButtonSelectedGradientMiddle); }
			}

			public override Color ButtonSelectedGradientEnd
			{
				get { return EndGradient(this.ButtonSelectedGradientMiddle); }
			}

			public override Color ImageMarginGradientBegin
			{
				get { return StartGradient(this.ImageMarginGradientMiddle); }
			}

			public override Color ImageMarginGradientEnd
			{
				get { return EndGradient(this.ImageMarginGradientMiddle); }
			}

			/* public override Color MenuItemPressedGradientBegin
			{
				get { return StartGradient(this.MenuItemPressedGradientMiddle); }
			}

			public override Color MenuItemPressedGradientEnd
			{
				get { return EndGradient(this.MenuItemPressedGradientMiddle); }
			} */

			public override Color MenuItemSelectedGradientBegin
			{
				get { return StartGradient(this.MenuItemSelected); }
			}

			public override Color MenuItemSelectedGradientEnd
			{
				get { return EndGradient(this.MenuItemSelected); }
			}
		}

		public KeePassTsr() : base(new KeePassTsrColorTable())
		{
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			ToolStripItem tsi = ((e != null) ? e.Item : null);

			if((tsi != null) && ((tsi.Owner is ContextMenuStrip) ||
				(tsi.OwnerItem != null)) && tsi.Selected)
			{
				Rectangle rect = tsi.ContentRectangle;
				rect.Offset(0, -1);
				rect.Height += 1;

				Color clrStart = KeePassTsrColorTable.StartGradient(this.ColorTable.MenuItemSelected);
				Color clrEnd = KeePassTsrColorTable.EndGradient(this.ColorTable.MenuItemSelected);
				Color clrBorder = this.ColorTable.MenuItemBorder;

				if(!tsi.Enabled)
				{
					Color clrBase = this.ColorTable.MenuStripGradientEnd;
					clrStart = UIUtil.ColorTowardsGrayscale(clrStart, clrBase, 0.5);
					clrEnd = UIUtil.ColorTowardsGrayscale(clrEnd, clrBase, 0.2);
					clrBorder = UIUtil.ColorTowardsGrayscale(clrBorder, clrBase, 0.2);
				}

				Graphics g = e.Graphics;
				if(g != null)
				{
					LinearGradientBrush br = new LinearGradientBrush(rect,
						clrStart, clrEnd, LinearGradientMode.Vertical);
					Pen p = new Pen(clrBorder);

					SmoothingMode smOrg = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.HighQuality;

					GraphicsPath gp = UIUtil.CreateRoundedRectangle(rect.X, rect.Y,
						rect.Width, rect.Height, DpiUtil.ScaleIntY(2));
					if(gp != null)
					{
						g.FillPath(br, gp);
						g.DrawPath(p, gp);

						gp.Dispose();
					}
					else // Shouldn't ever happen...
					{
						Debug.Assert(false);
						g.FillRectangle(br, rect);
						g.DrawRectangle(p, rect);
					}

					g.SmoothingMode = smOrg;

					p.Dispose();
					br.Dispose();
					return;
				}
				else { Debug.Assert(false); }
			}

			base.OnRenderMenuItemBackground(e);
		}
	}
}
