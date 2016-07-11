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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;
using KeePass.Util;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public enum BannerStyle
	{
		Default = 0,

		WinXPLogin = 1,
		WinVistaBlack = 2,
		KeePassWin32 = 3,
		BlueCarbon = 4
	}

	public sealed class BfBannerInfo
	{
		public int Width { get; private set; }
		public int Height { get; private set; }
		public BannerStyle Style { get; private set; }
		public Image Icon { get; private set; }
		public string TitleText { get; private set; }
		public string InfoText { get; private set; }

		public BfBannerInfo(int nWidth, int nHeight, BannerStyle bs,
			Image imgIcon, string strTitle, string strLine)
		{
			this.Width = nWidth;
			this.Height = nHeight;
			this.Style = bs;
			this.Icon = imgIcon;
			this.TitleText = strTitle;
			this.InfoText = strLine;
		}
	}

	public delegate Image BfBannerGenerator(BfBannerInfo bannerInfo);

	public static class BannerFactory
	{
		private const int StdHeight = 60; // Standard height for 96 DPI
		private const int StdIconDim = 48;

		private static Dictionary<string, Image> m_vImageCache =
			new Dictionary<string, Image>();
		private const int MaxCachedImages = 32;

		private static BfBannerGenerator m_pCustomGen = null;
		public static BfBannerGenerator CustomGenerator
		{
			get { return m_pCustomGen; }
			set { m_pCustomGen = value; }
		}

		public static Image CreateBanner(int nWidth, int nHeight, BannerStyle bs,
			Image imgIcon, string strTitle, string strLine)
		{
			return CreateBanner(nWidth, nHeight, bs, imgIcon, strTitle, strLine, false);
		}

		public static Image CreateBanner(int nWidth, int nHeight, BannerStyle bs,
			Image imgIcon, string strTitle, string strLine, bool bNoCache)
		{
			// imgIcon may be null
			Debug.Assert(strTitle != null); if(strTitle == null) throw new ArgumentNullException("strTitle");
			Debug.Assert(strLine != null); if(strLine == null) throw new ArgumentNullException("strLine");

			Debug.Assert((nHeight == StdHeight) || DpiUtil.ScalingRequired);

			if(MonoWorkarounds.IsRequired(12525) && (nHeight > 0))
				--nHeight;

			string strImageID = nWidth.ToString() + "x" + nHeight.ToString() + ":";
			if(strTitle != null) strImageID += strTitle;
			strImageID += ":";
			if(strLine != null) strImageID += strLine;

			if(bs == BannerStyle.Default) bs = Program.Config.UI.BannerStyle;
			if(bs == BannerStyle.Default)
			{
				Debug.Assert(false);
				bs = BannerStyle.WinVistaBlack;
			}

			strImageID += ":" + ((uint)bs).ToString();

			// Try getting the banner from the banner cache
			Image img = null;
			if(!bNoCache && m_vImageCache.TryGetValue(strImageID, out img))
				return img;

			if(m_pCustomGen != null)
				img = m_pCustomGen(new BfBannerInfo(nWidth, nHeight, bs, imgIcon,
					strTitle, strLine));

			const float fHorz = 0.90f;
			const float fVert = 90.0f;

			if(img == null)
			{
				img = new Bitmap(nWidth, nHeight, PixelFormat.Format24bppRgb);
				Graphics g = Graphics.FromImage(img);
				int xIcon = DpiScaleInt(10, nHeight);

				bool bRtl = Program.Translation.Properties.RightToLeft;
				Matrix mxTrfOrg = g.Transform;
				if(bRtl)
				{
					g.TranslateTransform(nWidth, 0.0f);
					g.ScaleTransform(-1.0f, 1.0f);
				}

				Color clrStart = Color.White;
				Color clrEnd = Color.LightBlue;
				float fAngle = fHorz;

				if(bs == BannerStyle.BlueCarbon)
				{
					fAngle = fVert;

					g.Clear(Color.Black); // Area from 3/8 to 1/2 height

					clrStart = Color.LightGray;
					clrEnd = Color.Black;

					Rectangle rect = new Rectangle(0, 0, nWidth, (nHeight * 3) / 8);
					using(LinearGradientBrush brCarbonT = new LinearGradientBrush(
						rect, clrStart, clrEnd, fAngle, true))
					{
						g.FillRectangle(brCarbonT, rect);
					}

					// clrStart = Color.FromArgb(0, 0, 32);
					clrStart = Color.FromArgb(0, 0, 28);
					// clrEnd = Color.FromArgb(192, 192, 255);
					clrEnd = Color.FromArgb(155, 155, 214);

					// rect = new Rectangle(0, nHeight / 2, nWidth, (nHeight * 5) / 8);
					int hMid = nHeight / 2;
					rect = new Rectangle(0, hMid - 1, nWidth, nHeight - hMid);
					using(LinearGradientBrush brCarbonB = new LinearGradientBrush(
						rect, clrStart, clrEnd, fAngle, true))
					{
						g.FillRectangle(brCarbonB, rect);
					}

					// Workaround gradient drawing bug (e.g. occuring on
					// Windows 8.1 with 150% DPI)
					using(Pen pen = new Pen(Color.Black))
					{
						g.DrawLine(pen, 0, hMid - 1, nWidth - 1, hMid - 1);
					}
				}
				else
				{
					if(bs == BannerStyle.WinXPLogin)
					{
						clrStart = Color.FromArgb(200, 208, 248);
						clrEnd = Color.FromArgb(40, 64, 216);
					}
					else if(bs == BannerStyle.WinVistaBlack)
					{
						clrStart = Color.FromArgb(151, 154, 173);
						clrEnd = Color.FromArgb(27, 27, 37);

						fAngle = fVert;
					}
					else if(bs == BannerStyle.KeePassWin32)
					{
						clrStart = Color.FromArgb(235, 235, 255);
						clrEnd = Color.FromArgb(192, 192, 255);
					}

					Rectangle rect = new Rectangle(0, 0, nWidth, nHeight);
					using(LinearGradientBrush brBack = new LinearGradientBrush(
						rect, clrStart, clrEnd, fAngle, true))
					{
						g.FillRectangle(brBack, rect);
					}
				}

				int wIconScaled = StdIconDim;
				int hIconScaled = StdIconDim;
				if(imgIcon != null)
				{
					float fIconRel = (float)imgIcon.Width / (float)imgIcon.Height;
					wIconScaled = (int)Math.Round(DpiScaleFloat(fIconRel *
						(float)StdIconDim, nHeight));
					hIconScaled = DpiScaleInt(StdIconDim, nHeight);

					int yIcon = (nHeight - hIconScaled) / 2;
					if(hIconScaled == imgIcon.Height)
						g.DrawImageUnscaled(imgIcon, xIcon, yIcon);
					else
						g.DrawImage(imgIcon, xIcon, yIcon, wIconScaled, hIconScaled);

					ColorMatrix cm = new ColorMatrix();
					cm.Matrix33 = 0.1f;
					ImageAttributes ia = new ImageAttributes();
					ia.SetColorMatrix(cm);

					int w = wIconScaled * 3, h = hIconScaled * 3;
					int x = nWidth - w - xIcon, y = (nHeight - h) / 2;
					Rectangle rectDest = new Rectangle(x, y, w, h);
					g.DrawImage(imgIcon, rectDest, 0, 0, imgIcon.Width, imgIcon.Height,
						GraphicsUnit.Pixel, ia);
				}

				if((bs == BannerStyle.WinXPLogin) || (bs == BannerStyle.WinVistaBlack) ||
					(bs == BannerStyle.BlueCarbon))
				{
					int sh = DpiUtil.ScaleIntY(20) / 10; // Force floor

					Rectangle rect = new Rectangle(0, nHeight - sh, 0, sh);

					rect.Width = nWidth / 2 + 1;
					rect.X = nWidth / 2;
					clrStart = Color.FromArgb(248, 136, 24);
					clrEnd = Color.White;
					using(LinearGradientBrush brushOrangeWhite = new LinearGradientBrush(
						rect, clrStart, clrEnd, fHorz, true))
					{
						g.FillRectangle(brushOrangeWhite, rect);
					}

					rect.Width = nWidth / 2 + 1;
					rect.X = 0;
					clrStart = Color.White;
					clrEnd = Color.FromArgb(248, 136, 24);
					using(LinearGradientBrush brushWhiteOrange = new LinearGradientBrush(
						rect, clrStart, clrEnd, fHorz, true))
					{
						g.FillRectangle(brushWhiteOrange, rect);
					}
				}
				else if(bs == BannerStyle.KeePassWin32)
				{
					int sh = DpiUtil.ScaleIntY(10) / 10; // Force floor

					// Black separator line
					using(Pen penBlack = new Pen(Color.Black))
					{
						for(int i = 0; i < sh; ++i)
							g.DrawLine(penBlack, 0, nHeight - i - 1,
								nWidth - 1, nHeight - i - 1);
					}
				}

				if(bRtl) g.Transform = mxTrfOrg;

				// Brush brush;
				Color clrText;
				if(bs == BannerStyle.KeePassWin32)
				{
					// brush = Brushes.Black;
					clrText = Color.Black;
				}
				else
				{
					// brush = Brushes.White;
					clrText = Color.White;
				}

				// float fx = 2 * xIcon, fy = 9.0f;
				int tx = 2 * xIcon, ty = DpiScaleInt(9, nHeight);
				if(imgIcon != null) tx += wIconScaled; // fx

				// TextFormatFlags tff = (TextFormatFlags.PreserveGraphicsClipping |
				//	TextFormatFlags.NoPrefix);
				// if(bRtl) tff |= TextFormatFlags.RightToLeft;

				float fFontSize = DpiScaleFloat((12.0f * 96.0f) / g.DpiY, nHeight);
				Font font = FontUtil.CreateFont(FontFamily.GenericSansSerif,
					fFontSize, FontStyle.Bold);
				int txs = (!bRtl ? tx : (nWidth - tx));
					// - TextRenderer.MeasureText(g, strTitle, font).Width));
				// g.DrawString(strTitle, font, brush, fx, fy);
				BannerFactory.DrawText(g, strTitle, txs, ty, font, clrText, bRtl);
				font.Dispose();

				tx += xIcon; // fx
				ty += xIcon * 2 + 2; // fy

				float fFontSizeSm = DpiScaleFloat((9.0f * 96.0f) / g.DpiY, nHeight);
				Font fontSmall = FontUtil.CreateFont(FontFamily.GenericSansSerif,
					fFontSizeSm, FontStyle.Regular);
				int txl = (!bRtl ? tx : (nWidth - tx));
					// - TextRenderer.MeasureText(g, strLine, fontSmall).Width));
				// g.DrawString(strLine, fontSmall, brush, fx, fy);
				BannerFactory.DrawText(g, strLine, txl, ty, fontSmall, clrText, bRtl);
				fontSmall.Dispose();

				g.Dispose();
			}

			if(!bNoCache)
			{
				while(m_vImageCache.Count >= MaxCachedImages)
				{
					foreach(string strKey in m_vImageCache.Keys)
					{
						m_vImageCache.Remove(strKey);
						break; // Remove first item only
					}
				}

				// Save in cache
				m_vImageCache[strImageID] = img;
			}

			return img;
		}

		private static void DrawText(Graphics g, string strText, int x,
			int y, Font font, Color clrForeground, bool bRtl)
		{
			// With ClearType on, text drawn using Graphics.DrawString
			// looks better than TextRenderer.DrawText;
			// https://sourceforge.net/p/keepass/discussion/329220/thread/06ef4466/

			/* // On Windows 2000 the DrawText method taking a Point doesn't
			// work by design, see MSDN:
			// http://msdn.microsoft.com/en-us/library/ms160657.aspx
			if(WinUtil.IsWindows2000)
				TextRenderer.DrawText(g, strText, font, new Rectangle(pt.X, pt.Y,
					nWidth - pt.X - 1, nHeight - pt.Y - 1), clrForeground, tff);
			else
				TextRenderer.DrawText(g, strText, font, pt, clrForeground, tff); */

			using(SolidBrush br = new SolidBrush(clrForeground))
			{
				StringFormatFlags sff = (StringFormatFlags.FitBlackBox |
					StringFormatFlags.NoClip);
				if(bRtl) sff |= StringFormatFlags.DirectionRightToLeft;

				using(StringFormat sf = new StringFormat(sff))
				{
					g.DrawString(strText, font, br, x, y, sf);
				}
			}
		}

		private static int DpiScaleInt(int x, int nBaseHeight)
		{
			return (int)Math.Round((double)(x * nBaseHeight) / (double)StdHeight);
		}

		private static float DpiScaleFloat(float x, int nBaseHeight)
		{
			return ((x * (float)nBaseHeight) / (float)StdHeight);
		}

		public static void CreateBannerEx(Form f, PictureBox picBox, Image imgIcon,
			string strTitle, string strLine)
		{
			CreateBannerEx(f, picBox, imgIcon, strTitle, strLine, false);
		}

		public static void CreateBannerEx(Form f, PictureBox picBox, Image imgIcon,
			string strTitle, string strLine, bool bNoCache)
		{
			if(picBox == null) { Debug.Assert(false); return; }

			try
			{
				picBox.Image = CreateBanner(picBox.Width, picBox.Height,
					BannerStyle.Default, imgIcon, strTitle, strLine, bNoCache);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		/// <summary>
		/// Update/create a dialog banner. This method is intended for
		/// updating banners in resizable dialogs. The created banner
		/// images bypass the cache of the factory and are disposed
		/// when the dialog is resized (i.e. the caller shouldn't do
		/// anything with the banner images).
		/// </summary>
		public static void UpdateBanner(Form f, PictureBox picBox, Image imgIcon,
			string strTitle, string strLine, ref int nOldWidth)
		{
			int nWidth = picBox.Width;
			if(nWidth != nOldWidth)
			{
				Image imgPrev = null;
				if(nOldWidth >= 0) imgPrev = picBox.Image;

				BannerFactory.CreateBannerEx(f, picBox, imgIcon, strTitle,
					strLine, true);

				if(imgPrev != null) imgPrev.Dispose(); // Release old banner

				nOldWidth = nWidth;
			}
		}
	}
}
