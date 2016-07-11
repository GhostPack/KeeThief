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
using System.Text;
using System.Windows.Forms;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Native;
using KeePassLib.Utility;

namespace KeePass.UI.ToolStripRendering
{
	internal sealed class ProExtTsrFactory : TsrFactory
	{
		private PwUuid m_uuid = new PwUuid(new byte[] {
			0x21, 0xED, 0x54, 0x1A, 0xE2, 0xEB, 0xCB, 0x0C,
			0x57, 0x18, 0x41, 0x32, 0x70, 0xD8, 0xE0, 0xE9
		});

		public override PwUuid Uuid
		{
			get { return m_uuid; }
		}

		public override string Name
		{
			get { return (".NET/Office - " + KPRes.Professional); }
		}

		public override ToolStripRenderer CreateInstance()
		{
			return new ProExtTsr();
		}
	}

	public class ProExtTsr : ToolStripProfessionalRenderer
	{
		private bool m_bCustomColorTable = false;

		protected bool IsDarkStyle
		{
			get
			{
				ProfessionalColorTable ct = this.ColorTable;
				if(ct == null) { Debug.Assert(false); return false; }

				return UIUtil.IsDarkColor(ct.ToolStripDropDownBackground);
			}
		}

		protected virtual bool EnsureTextContrast
		{
			get { return true; }
		}

		public ProExtTsr() : base()
		{
		}

		public ProExtTsr(ProfessionalColorTable ct) : base(ct)
		{
			m_bCustomColorTable = true;
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderButtonBackground(e);

			// .NET incorrectly draws the border using
			// this.ColorTable.ButtonSelectedBorder even when the button
			// is pressed; thus in this case we draw it again using the
			// correct color
			ToolStripItem tsi = ((e != null) ? e.Item : null);
			if((tsi != null) && tsi.Pressed && !NativeLib.IsUnix())
			{
				using(Pen p = new Pen(this.ColorTable.ButtonPressedBorder))
				{
					e.Graphics.DrawRectangle(p, 0, 0, tsi.Width - 1, tsi.Height - 1);
				}
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			Image imgToDispose = null;
			try
			{
				Graphics g = e.Graphics;
				Image imgOrg = e.Image;
				Rectangle rOrg = e.ImageRectangle;
				ToolStripItem tsi = e.Item;

				Image img = imgOrg;
				Rectangle r = rOrg;
				Debug.Assert(r.Width == r.Height);
				Debug.Assert(DpiUtil.ScalingRequired || (r.Size ==
					((img != null) ? img.Size : new Size(3, 5))));

				// Override the .NET checkmark bitmap
				ToolStripMenuItem tsmi = (tsi as ToolStripMenuItem);
				if((tsmi != null) && tsmi.Checked && (tsmi.Image == null))
					img = Properties.Resources.B16x16_MenuCheck;

				if(tsi != null)
				{
					Rectangle rContent = tsi.ContentRectangle;
					Debug.Assert(rContent.Contains(r) || DpiUtil.ScalingRequired);
					r.Intersect(rContent);
					if(r.Height < r.Width) r.Width = r.Height;
				}
				else { Debug.Assert(false); }

				if((img != null) && (r.Size != img.Size))
				{
					img = GfxUtil.ScaleImage(img, r.Width, r.Height,
						ScaleTransformFlags.UIIcon);
					imgToDispose = img;
				}

				if((img != imgOrg) || (r != rOrg))
				{
					ToolStripItemImageRenderEventArgs eNew =
						new ToolStripItemImageRenderEventArgs(g, tsi, img, r);
					base.OnRenderItemCheck(eNew);
					return;
				}

				/* ToolStripMenuItem tsmi = (tsi as ToolStripMenuItem);
				if((tsmi != null) && tsmi.Checked && (r.Width > 0) &&
					(r.Height > 0) && (img != null) &&
					((img.Width != r.Width) || (img.Height != r.Height)))
				{
					Rectangle rContent = tsmi.ContentRectangle;
					r.Intersect(rContent);
					if(r.Height < r.Width)
						r.Width = r.Height;

					ProfessionalColorTable ct = this.ColorTable;

					Color clrBorder = ct.ButtonSelectedBorder;

					Color clrBG = ct.CheckBackground;
					if(tsmi.Selected) clrBG = ct.CheckSelectedBackground;
					if(tsmi.Pressed) clrBG = ct.CheckPressedBackground;

					Color clrFG = ((UIUtil.ColorToGrayscale(clrBG).R >= 128) ?
						Color.Black : Color.White);

					using(SolidBrush sb = new SolidBrush(clrBG))
					{
						g.FillRectangle(sb, r);
					}
					using(Pen p = new Pen(clrBorder))
					{
						g.DrawRectangle(p, r.X, r.Y, r.Width - 1, r.Height - 1);
					}

					ControlPaint.DrawMenuGlyph(g, r, MenuGlyph.Checkmark,
						clrFG, Color.Transparent);

					// if((img.Width == r.Width) && (img.Height == r.Height))
					//	g.DrawImage(img, r);
					// else
					// {
					//	Image imgScaled = GfxUtil.ScaleImage(img,
					//		r.Width, r.Height, ScaleTransformFlags.UIIcon);
					//	g.DrawImage(imgScaled, r);
					//	imgScaled.Dispose();
					// }

					return;
				} */

				/* if((img != null) && (r.Width > 0) && (r.Height > 0) &&
					((img.Width != r.Width) || (img.Height != r.Height)) &&
					(tsi != null))
				{
					// This should only happen on high DPI
					Debug.Assert(DpiUtil.ScalingRequired);

					Image imgScaled = GfxUtil.ScaleImage(img,
						r.Width, r.Height, ScaleTransformFlags.UIIcon);
					// Image imgScaled = new Bitmap(r.Width, r.Height,
					//	PixelFormat.Format32bppArgb);
					// using(Graphics gScaled = Graphics.FromImage(imgScaled))
					// {
					//	gScaled.Clear(Color.Transparent);

					//	Color clrFG = ((UIUtil.ColorToGrayscale(
					//		this.ColorTable.CheckBackground).R >= 128) ?
					//		Color.FromArgb(18, 24, 163) : Color.White);

					//	Rectangle rGlyph = new Rectangle(0, 0, r.Width, r.Height);
					//	// rGlyph.Inflate(-r.Width / 12, -r.Height / 12);

					//	ControlPaint.DrawMenuGlyph(gScaled, rGlyph,
					//		MenuGlyph.Bullet, clrFG, Color.Transparent);
					// }

					ToolStripItemImageRenderEventArgs eMod =
						new ToolStripItemImageRenderEventArgs(g, e.Item,
							imgScaled, r);
					base.OnRenderItemCheck(eMod);

					imgScaled.Dispose();
					return;
				} */
			}
			catch(Exception) { Debug.Assert(false); }
			finally
			{
				if(imgToDispose != null) imgToDispose.Dispose();
			}

			base.OnRenderItemCheck(e);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if(e != null)
			{
				ToolStripItem tsi = e.Item;

				// In high contrast mode, various colors of the default
				// color table are incorrect, thus check m_bCustomColorTable
				if((tsi != null) && this.EnsureTextContrast && m_bCustomColorTable)
				{
					bool bDarkBack = this.IsDarkStyle;
					if(tsi.Selected || tsi.Pressed)
					{
						if((tsi.Owner is ContextMenuStrip) || (tsi.OwnerItem != null))
							bDarkBack = UIUtil.IsDarkColor(this.ColorTable.MenuItemSelected);
						else // Top menu item
						{
							if(tsi.Pressed)
								bDarkBack = UIUtil.IsDarkColor(
									this.ColorTable.MenuItemPressedGradientMiddle);
							else
								bDarkBack = UIUtil.IsDarkColor(UIUtil.ColorMiddle(
									this.ColorTable.MenuItemSelectedGradientBegin,
									this.ColorTable.MenuItemSelectedGradientEnd));
						}
					}

					// e.TextColor might be incorrect, thus use tsi.ForeColor
					bool bDarkText = UIUtil.IsDarkColor(tsi.ForeColor);

					if(bDarkBack && bDarkText)
					{
						Debug.Assert(false);
						e.TextColor = Color.White;
					}
					else if(!bDarkBack && !bDarkText)
					{
						Debug.Assert(false);
						e.TextColor = Color.Black;
					}
				}
			}
			else { Debug.Assert(false); }

			base.OnRenderItemText(e);
		}
	}
}
