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
using System.Runtime.InteropServices;
using System.Diagnostics;

using KeePass.Native;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass.Util
{
	public sealed class ClipboardEventChainBlocker : IDisposable
	{
		private ClipboardBlockerForm m_form = null;
		private IntPtr m_hChain = IntPtr.Zero;

		public ClipboardEventChainBlocker()
		{
			if(NativeLib.IsUnix()) return; // Unsupported

			m_form = new ClipboardBlockerForm();

			try
			{
				m_hChain = NativeMethods.SetClipboardViewer(m_form.Handle);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		~ClipboardEventChainBlocker()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(bDisposing && (m_form != null))
			{
				try
				{
					// Ignore return value (no assert); see documentation
					// of ChangeClipboardChain
					NativeMethods.ChangeClipboardChain(m_form.Handle, m_hChain);
				}
				catch(Exception) { Debug.Assert(false); }

				m_form.Dispose();
				m_form = null;
			}

			m_hChain = IntPtr.Zero;
		}

		[Obsolete]
		public void Release()
		{
			Dispose(true);
		}

		private sealed class ClipboardBlockerForm : Form
		{
			public ClipboardBlockerForm() : base()
			{
				this.Visible = false;
				this.ShowInTaskbar = false;
				this.ShowIcon = false;
			}

			public override bool PreProcessMessage(ref Message msg)
			{
				if(msg.Msg == NativeMethods.WM_DRAWCLIPBOARD)
					return true; // Block message

				return base.PreProcessMessage(ref msg);
			}
		}
	}
}
