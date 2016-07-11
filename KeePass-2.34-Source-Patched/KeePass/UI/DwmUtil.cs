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
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;
using KeePass.Forms;
using KeePass.Native;
using KeePass.Util;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public static class DwmUtil
	{
		private const uint DWMWA_FORCE_ICONIC_REPRESENTATION = 7;
		// private const uint DWMWA_FLIP3D_POLICY = 8;
		private const uint DWMWA_HAS_ICONIC_BITMAP = 10;
		// private const uint DWMWA_DISALLOW_PEEK = 11;

		private const uint DWM_SIT_DISPLAYFRAME = 1;

		public const int WM_DWMSENDICONICTHUMBNAIL = 0x0323;
		public const int WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326;

		// private const int DWMFLIP3D_DEFAULT = 0;
		// private const int DWMFLIP3D_EXCLUDEBELOW = 1;

		// [DllImport("DwmApi.dll")]
		// private static extern Int32 DwmExtendFrameIntoClientArea(IntPtr hWnd,
		//	ref MARGINS pMarInset);

		// [DllImport("DwmApi.dll")]
		// private static extern Int32 DwmIsCompositionEnabled(ref Int32 pfEnabled);

		[DllImport("DwmApi.dll")]
		private static extern int DwmInvalidateIconicBitmaps(IntPtr hWnd);

		[DllImport("DwmApi.dll", EntryPoint = "DwmSetWindowAttribute")]
		private static extern int DwmSetWindowAttributeInt(IntPtr hWnd,
			uint dwAttribute, [In] ref int pvAttribute, uint cbAttribute);

		[DllImport("DwmApi.dll")]
		private static extern int DwmSetIconicThumbnail(IntPtr hWnd,
			IntPtr hBmp, uint dwSITFlags);

		[DllImport("DwmApi.dll")]
		private static extern int DwmSetIconicLivePreviewBitmap(IntPtr hWnd,
			IntPtr hBmp, IntPtr pptClient, uint dwSITFlags);

		internal static void EnableWindowPeekPreview(MainForm mf)
		{
			if(mf == null) { Debug.Assert(false); return; }

			FormWindowState ws = mf.WindowState;
			bool bPeek = ((ws == FormWindowState.Normal) ||
				(ws == FormWindowState.Maximized) || !mf.IsFileLocked(null));

			EnableWindowPeekPreview(mf, bPeek);
		}

		public static void EnableWindowPeekPreview(Form f, bool bEnable)
		{
			try
			{
				// Static iconic bitmaps only supported by Windows >= 7
				if(!WinUtil.IsAtLeastWindows7) return;

				IntPtr h = f.Handle;
				if(h == IntPtr.Zero) { Debug.Assert(false); return; }

				int s = (bEnable ? 0 : 1);

				// DwmSetWindowAttributeInt(h, DWMWA_DISALLOW_PEEK, ref iNoPeek, 4);

				// int iFlip3D = (bEnable ? DWMFLIP3D_DEFAULT :
				//	DWMFLIP3D_EXCLUDEBELOW);
				// DwmSetWindowAttributeInt(h, DWMWA_FLIP3D_POLICY, ref iFlip3D, 4);

				DwmSetWindowAttributeInt(h, DWMWA_HAS_ICONIC_BITMAP, ref s, 4);
				DwmSetWindowAttributeInt(h, DWMWA_FORCE_ICONIC_REPRESENTATION, ref s, 4);

				DwmInvalidateIconicBitmaps(h);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void SetIconicThumbnail(Form f, Icon ico, ref Message m)
		{
			int iInfo = (int)m.LParam.ToInt64();
			Size sz = new Size((iInfo >> 16) & 0xFFFF, iInfo & 0xFFFF);

			SetIconicBitmap(f, ico, sz, true);

			m.Result = IntPtr.Zero;
		}

		public static void SetIconicPreview(Form f, Icon ico, ref Message m)
		{
			if(f == null) { Debug.Assert(false); return; }

			Size sz = new Size(0, 0);
			MainForm mf = (f as MainForm);

			if(mf != null) sz = mf.LastClientSize;
			if((sz.Width <= 0) || (sz.Height <= 0)) sz = f.ClientSize;

			SetIconicBitmap(f, ico, sz, false);

			m.Result = IntPtr.Zero;
		}

		private static void SetIconicBitmap(Form f, Icon ico, Size sz,
			bool bThumbnail)
		{
			Image img = null;
			Bitmap bmp = null;
			IntPtr hBmp = IntPtr.Zero;
			try
			{
				IntPtr hWnd = f.Handle;
				if(hWnd == IntPtr.Zero) { Debug.Assert(false); return; }

				img = UIUtil.ExtractVistaIcon(ico);
				if(img == null) img = ico.ToBitmap();
				if(img == null) { Debug.Assert(false); return; }

				int sw = sz.Width, sh = sz.Height;
				if(sw <= 0) { Debug.Assert(false); sw = 200; } // Default Windows 7
				if(sh <= 0) { Debug.Assert(false); sh = 109; } // Default Windows 7

				int iImgW = Math.Min(img.Width, 128);
				int iImgH = Math.Min(img.Height, 128);
				int iImgWMax = (sw * 4) / 6;
				int iImgHMax = (sh * 4) / 6;
				if(iImgW > iImgWMax)
				{
					float fRatio = (float)iImgWMax / (float)iImgW;
					iImgW = iImgWMax;
					iImgH = (int)((float)iImgH * fRatio);
				}
				if(iImgH > iImgHMax)
				{
					float fRatio = (float)iImgHMax / (float)iImgH;
					iImgW = (int)((float)iImgW * fRatio);
					iImgH = iImgHMax;
				}
				if((iImgW <= 0) || (iImgH <= 0)) { Debug.Assert(false); return; }
				if(iImgW > sw) { Debug.Assert(false); iImgW = sw; }
				if(iImgH > sh) { Debug.Assert(false); iImgH = sh; }

				int iImgX = (sw - iImgW) / 2;
				int iImgY = (sh - iImgH) / 2;

				// 32-bit color depth required by API
				bmp = new Bitmap(sw, sh, PixelFormat.Format32bppArgb);
				using(Graphics g = Graphics.FromImage(bmp))
				{
					Color clr = AppDefs.ColorControlDisabled;
					g.Clear(clr);

					using(LinearGradientBrush br = new LinearGradientBrush(
						new Point(0, 0), new Point(sw, sh), UIUtil.LightenColor(
						clr, 0.25), UIUtil.DarkenColor(clr, 0.25)))
					{
						g.FillRectangle(br, 0, 0, sw, sh);
					}

					// *After* drawing the gradient (otherwise border bug)
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.SmoothingMode = SmoothingMode.HighQuality;

					RectangleF rSource = new RectangleF(0.0f, 0.0f,
						img.Width, img.Height);
					RectangleF rDest = new RectangleF(iImgX, iImgY, iImgW, iImgH);
					GfxUtil.AdjustScaleRects(ref rSource, ref rDest);

					// g.DrawImage(img, iImgX, iImgY, iImgW, iImgH);
					g.DrawImage(img, rDest, rSource, GraphicsUnit.Pixel);
				}

				hBmp = bmp.GetHbitmap();

				if(bThumbnail)
					DwmSetIconicThumbnail(hWnd, hBmp, DWM_SIT_DISPLAYFRAME);
				else
					DwmSetIconicLivePreviewBitmap(hWnd, hBmp, IntPtr.Zero,
						DWM_SIT_DISPLAYFRAME);
			}
			catch(Exception) { Debug.Assert(!WinUtil.IsAtLeastWindows7); }
			finally
			{
				if(hBmp != IntPtr.Zero)
				{
					try { NativeMethods.DeleteObject(hBmp); }
					catch(Exception) { Debug.Assert(false); }
				}
				if(bmp != null) bmp.Dispose();
				if(img != null) img.Dispose();
			}
		}
	}
}
