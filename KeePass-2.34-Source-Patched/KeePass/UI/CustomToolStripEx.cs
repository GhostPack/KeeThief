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
using System.Diagnostics;

using KeePass.Native;
using KeePass.Util;

namespace KeePass.UI
{
	public sealed class CustomToolStripEx : ToolStrip
	{
		private CriticalSectionEx m_csSizeAuto = new CriticalSectionEx();
		private int m_iLockedHeight = 0;

		public CustomToolStripEx() : base()
		{
			// ThemeToolStripRenderer.AttachTo(this);

			UIUtil.Configure(this);
		}

#if DEBUG
		~CustomToolStripEx()
		{
			if(m_csSizeAuto.TryEnter()) m_csSizeAuto.Exit();
			else { Debug.Assert(false); } // Should have been unlocked
		}
#endif

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			// Enable 'click through' behavior
			if((m.Msg == NativeMethods.WM_MOUSEACTIVATE) &&
				(m.Result == (IntPtr)NativeMethods.MA_ACTIVATEANDEAT))
			{
				m.Result = (IntPtr)NativeMethods.MA_ACTIVATE;
			}
		}

		public void LockHeight(bool bLock)
		{
			Debug.Assert(this.Height > 0);
			m_iLockedHeight = (bLock ? this.Height : 0);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if(m_csSizeAuto.TryEnter())
			{
				try
				{
					Size sz = this.Size;
					// Ignore zero-size events (which can occur e.g. when
					// the ToolStrip is being hidden)
					if((sz.Width > 0) && (sz.Height > 0))
					{
						if((m_iLockedHeight > 0) && (sz.Height != m_iLockedHeight))
						{
							base.OnSizeChanged(e);
							this.Height = m_iLockedHeight;
							Debug.Assert(this.Size.Height == m_iLockedHeight);
							return;
						}
					}
				}
				catch(Exception) { Debug.Assert(false); }
				finally { m_csSizeAuto.Exit(); }
			}

			base.OnSizeChanged(e);
		}
	}
}
