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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using KeePass.UI;
using KeePass.Resources;

using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class EntropyForm : Form
	{
		private byte[] m_pbEntropy = null;
		private LinkedList<uint> m_llPool = new LinkedList<uint>();

		private Bitmap m_bmpRandom = null;

		public byte[] GeneratedEntropy
		{
			get { return m_pbEntropy; }
		}

		public static byte[] CollectEntropyIfEnabled(PwProfile pp)
		{
			if(!pp.CollectUserEntropy) return null;

			EntropyForm ef = new EntropyForm();
			if(UIUtil.ShowDialogNotValue(ef, DialogResult.OK)) return null;

			byte[] pbGen = ef.GeneratedEntropy;
			UIUtil.DestroyForm(ef);
			return pbGen;
		}

		public EntropyForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			// Can be invoked by tray command; don't use CenterParent
			Debug.Assert(this.StartPosition == FormStartPosition.CenterScreen);

			GlobalWindowManager.AddWindow(this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_Binary, KPRes.EntropyTitle,
				KPRes.EntropyDesc);
			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.EntropyTitle;

			m_bmpRandom = CreateRandomBitmap(m_picRandom.ClientSize);
			m_picRandom.Image = m_bmpRandom;

			UpdateUIState();
		}

		private void UpdateUIState()
		{
			int nBits = m_llPool.Count / 8;
			m_lblStatus.Text = nBits.ToString() + " " + KPRes.BitsStc;

			if(nBits > 256) { Debug.Assert(false); m_pbGenerated.Value = 100; }
			else m_pbGenerated.Value = (nBits * 100) / 256;
		}

		private void OnRandomMouseMove(object sender, MouseEventArgs e)
		{
			if(m_llPool.Count >= 2048) return;

			uint ul = (uint)((e.X << 8) ^ e.Y);
			ul ^= (uint)(Environment.TickCount << 16);

			m_llPool.AddLast(ul);

			UpdateUIState();
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			MemoryStream ms = new MemoryStream();

			// Prevent empty / low entropy buffer
			byte[] pbGuid = Guid.NewGuid().ToByteArray();
			ms.Write(pbGuid, 0, pbGuid.Length);

			foreach(uint ln in m_llPool)
				ms.Write(MemUtil.UInt32ToBytes(ln), 0, 4);

			if(m_tbEdit.Text.Length > 0)
			{
				byte[] pbUTF8 = StrUtil.Utf8.GetBytes(m_tbEdit.Text);
				ms.Write(pbUTF8, 0, pbUTF8.Length);
			}

			byte[] pbColl = ms.ToArray();

			SHA256Managed sha256 = new SHA256Managed();
			m_pbEntropy = sha256.ComputeHash(pbColl);

			CryptoRandom.Instance.AddEntropy(pbColl); // Will be hashed using SHA-512
			ms.Close();
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			if(m_bmpRandom != null)
			{
				m_picRandom.Image = null;

				m_bmpRandom.Dispose();
				m_bmpRandom = null;
			}
			else { Debug.Assert(false); }

			GlobalWindowManager.RemoveWindow(this);
		}

		private static Bitmap CreateRandomBitmap(Size sz)
		{
			int w = sz.Width;
			int h = sz.Height;
			if((w <= 0) || (h <= 0)) { Debug.Assert(false); return null; }

			byte[] pbEndianTest = BitConverter.GetBytes((int)7);
			bool bLittleEndian = (pbEndianTest[0] == 7);

			byte[] pbBmpData = new byte[w * h * 4];
			byte[] pbRandomValues = new byte[w * h];
			Program.GlobalRandom.NextBytes(pbRandomValues);
			int p = 0;
			for(int i = 0; i < pbBmpData.Length; i += 4)
			{
				byte bt = pbRandomValues[p];
				++p;

				if(bLittleEndian)
				{
					pbBmpData[i] = bt;
					pbBmpData[i + 3] = 255;
				}
				else
				{
					pbBmpData[i] = 255;
					pbBmpData[i + 3] = bt;
				}

				pbBmpData[i + 1] = bt;
				pbBmpData[i + 2] = bt;
			}
			Debug.Assert(p == (w * h));

			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);

			Rectangle rect = new Rectangle(0, 0, w, h);
			BitmapData bd = bmp.LockBits(rect, ImageLockMode.WriteOnly,
				PixelFormat.Format32bppArgb);

			if(bd.Stride == (w * 4))
				Marshal.Copy(pbBmpData, 0, bd.Scan0, pbBmpData.Length);
			else
			{
				Debug.Assert(false);

				byte[] pbBlank = new byte[bd.Stride * h];
				for(int i = 0; i < pbBlank.Length; ++i) pbBlank[i] = 255;

				Marshal.Copy(pbBlank, 0, bd.Scan0, pbBlank.Length);
			}

			bmp.UnlockBits(bd);
			return bmp;
		}
	}
}