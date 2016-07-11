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
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.UI;
using KeePass.Util;
using KeePass.Resources;

using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class TextEncodingForm : Form
	{
		private string m_strContext = string.Empty;
		private byte[] m_pbData = null;
		private bool m_bInitializing = false;
		private Encoding m_encSel = null;
		private uint m_uStartOffset = 0;

		public Encoding SelectedEncoding
		{
			get { return m_encSel; }
		}

		public void InitEx(string strContext, byte[] pbData)
		{
			m_strContext = (strContext ?? string.Empty);
			m_pbData = pbData;
		}

		public TextEncodingForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			m_bInitializing = true;
			FontUtil.AssignDefaultBold(m_lblContext);
			m_lblContext.Text = m_strContext;

			m_cmbEnc.Items.Add(KPRes.BinaryNoConv);
			foreach(StrEncodingInfo sei in StrUtil.Encodings)
				m_cmbEnc.Items.Add(sei.Name);

			StrEncodingInfo seiGuess = BinaryDataClassifier.GetStringEncoding(
				m_pbData, out m_uStartOffset);

			int iSel = 0;
			if(seiGuess != null)
				iSel = Math.Max(m_cmbEnc.FindStringExact(seiGuess.Name), 0);
			m_cmbEnc.SelectedIndex = iSel;

			m_bInitializing = false;
			UpdateTextPreview();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private Encoding GetSelEnc()
		{
			StrEncodingInfo sei = StrUtil.GetEncoding(m_cmbEnc.Text);
			return ((sei != null) ? sei.Encoding : null);
		}

		private void UpdateTextPreview()
		{
			if(m_bInitializing) return;

			m_rtbPreview.Clear(); // Clear formatting
			try
			{
				Encoding enc = GetSelEnc();
				if(enc == null) throw new InvalidOperationException();

				m_rtbPreview.Text = enc.GetString(m_pbData, (int)m_uStartOffset,
					m_pbData.Length - (int)m_uStartOffset);
			}
			catch(Exception) { m_rtbPreview.Text = string.Empty; }
		}

		private void OnEncSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateTextPreview();
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			m_encSel = GetSelEnc();
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}
	}
}
