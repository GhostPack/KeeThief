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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

using KeePass.Native;
using KeePass.Util;

namespace KeePass.UI
{
	public sealed class SplitButtonEx : Button
	{
		private const int BS_SPLITBUTTON = 0x0000000C;

		private const uint BCN_FIRST = unchecked((uint)(-1250));
		private const uint BCN_DROPDOWN = (BCN_FIRST + 0x0002);

		private readonly bool m_bSupported;

		private ContextMenuStrip m_ctx = null;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ContextMenuStrip SplitDropDownMenu
		{
			get { return m_ctx; }
			set { m_ctx = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				if(m_bSupported) cp.Style |= BS_SPLITBUTTON;

				return cp;
			}
		}

		public SplitButtonEx() : base()
		{
			m_bSupported = (WinUtil.IsAtLeastWindowsVista &&
				!KeePassLib.Native.NativeLib.IsUnix() && !Program.DesignMode);

			if(m_bSupported)
			{
				this.FlatStyle = FlatStyle.System;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if((m.Msg == NativeMethods.WM_NOTIFY_REFLECT) && m_bSupported)
			{
				try
				{
					NativeMethods.NMHDR nm = (NativeMethods.NMHDR)m.GetLParam(
						typeof(NativeMethods.NMHDR));
					if(nm.code == BCN_DROPDOWN)
					{
						if(m_ctx != null)
						{
							m_ctx.Show(this, new Point(0, this.Height));
							return; // We handled it
						}
						else { Debug.Assert(false); }
					}
				}
				catch(Exception) { Debug.Assert(false); }
			}

			base.WndProc(ref m);
		}
	}
}
