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
using System.Diagnostics;

using KeePass.Native;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass.Util
{
	public sealed class ShutdownBlocker : IDisposable
	{
		private static ShutdownBlocker g_sdbPrimary = null;
#if DEBUG
		internal static ShutdownBlocker Instance
		{
			get { return g_sdbPrimary; }
		}
#endif

		private readonly IntPtr m_hWnd;

		public ShutdownBlocker(IntPtr hWnd, string strReason)
		{
			Debug.Assert(hWnd != IntPtr.Zero);
			m_hWnd = hWnd;

			if(g_sdbPrimary != null) return; // We're not the first
			if(!WinUtil.IsAtLeastWindowsVista) return;
			if(NativeLib.IsUnix()) return;

			string str = strReason;
			if(string.IsNullOrEmpty(str)) { Debug.Assert(false); str = "..."; }

			try
			{
				if(NativeMethods.ShutdownBlockReasonCreate(hWnd, str))
					g_sdbPrimary = this;
				else { Debug.Assert(false); }
			}
			catch(Exception) { Debug.Assert(false); }
		}

		~ShutdownBlocker()
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
			if(object.ReferenceEquals(this, g_sdbPrimary))
			{
				try
				{
					if(!NativeMethods.ShutdownBlockReasonDestroy(m_hWnd))
					{
						Debug.Assert(false);
					}
				}
				catch(Exception) { Debug.Assert(false); }

				g_sdbPrimary = null;
			}
		}
	}
}
