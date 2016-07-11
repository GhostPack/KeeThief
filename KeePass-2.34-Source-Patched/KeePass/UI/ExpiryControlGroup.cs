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
using System.Globalization;
using System.Diagnostics;

using KeePassLib;

namespace KeePass.UI
{
	public sealed class ExpiryControlGroup
	{
		private CheckBox m_cb = null;
		private DateTimePicker m_dtp = null;

		public bool Checked
		{
			get
			{
				if(m_cb == null) { Debug.Assert(false); return false; }
				return m_cb.Checked;
			}

			set { UIUtil.SetChecked(m_cb, value); }
		}

		public DateTime Value
		{
			get
			{
				if(m_dtp == null) { Debug.Assert(false); return DateTime.Now; }

				// Force validation/update of incomplete edit
				// (workaround for KPB 3505269)
				if(m_dtp.Focused && m_dtp.Visible)
				{
					m_dtp.Visible = false;
					m_dtp.Visible = true;
				}

				return m_dtp.Value;
			}

			set
			{
				if(m_dtp == null) { Debug.Assert(false); return; }
				m_dtp.Value = value;
			}
		}

		public ExpiryControlGroup()
		{
		}

#if DEBUG
		~ExpiryControlGroup()
		{
			Debug.Assert(m_cb == null); // Owner should call Release()
		}
#endif

		public void Attach(CheckBox cb, DateTimePicker dtp)
		{
			if(cb == null) throw new ArgumentNullException("cb");
			if(dtp == null) throw new ArgumentNullException("dtp");

			m_cb = cb;
			m_dtp = dtp;

			// m_dtp.ShowUpDown = true;
			m_dtp.CustomFormat = DateTimeFormatInfo.CurrentInfo.ShortDatePattern +
				" " + DateTimeFormatInfo.CurrentInfo.LongTimePattern;

			m_dtp.ValueChanged += this.OnExpiryValueChanged;
			// Also handle key press event (workaround for KPB 3505269)
			m_dtp.KeyPress += this.OnExpiryKeyPress;
		}

		public void Release()
		{
			if(m_cb == null) return;

			m_dtp.ValueChanged -= this.OnExpiryValueChanged;
			m_dtp.KeyPress -= this.OnExpiryKeyPress;

			m_cb = null;
			m_dtp = null;
		}

		private void UpdateUI(bool? pbSetCheck)
		{
			if(pbSetCheck.HasValue)
				UIUtil.SetChecked(m_cb, pbSetCheck.Value);

			UIUtil.SetEnabled(m_dtp, m_cb.Enabled);
		}

		private void OnExpiryValueChanged(object sender, EventArgs e)
		{
			UpdateUI(true);
		}

		private void OnExpiryKeyPress(object sender, KeyPressEventArgs e)
		{
			if(char.IsDigit(e.KeyChar)) UpdateUI(true);
		}
	}
}
