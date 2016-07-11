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

/*
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;

namespace KeePass.Native
{
	[Flags]
	public enum ProgDlgFlags
	{
		Normal = 0x0,
		Modal = 0x1,
		AutoTime = 0x2,
		NoTime = 0x4,
		NoMinimize = 0x8,
		NoProgressBar = 0x10,
		MarqueeProgress = 0x20,
		NoCancel = 0x40
	}

	public sealed class NativeProgressDialog : IStatusLogger
	{
		private IProgressDialog m_p;
		private uint m_uFlags;
		private IntPtr m_hWndParent;
		private uint m_uLastPercent = 0;

		private const uint PDTIMER_RESET = 0x1;
		private const uint PDTIMER_PAUSE = 0x2;
		private const uint PDTIMER_RESUME = 0x3;

		public static bool IsSupported
		{
			get
			{
				return (WinUtil.IsAtLeastWindowsVista &&
					!KeePassLib.Native.NativeLib.IsUnix());
			}
		}

		public NativeProgressDialog(IntPtr hWndParent, ProgDlgFlags fl)
		{
			m_hWndParent = hWndParent;
			m_uFlags = (uint)fl;

			try { m_p = (IProgressDialog)(new Win32ProgressDialog()); }
			catch(Exception) { Debug.Assert(false); m_p = null; }
		}

		~NativeProgressDialog()
		{
			EndLogging();
		}

		public void StartLogging(string strOperation, bool bWriteOperationToLog)
		{
			if(m_p == null) { Debug.Assert(false); return; }

			m_p.SetTitle(PwDefs.ShortProductName);
			m_p.StartProgressDialog(m_hWndParent, IntPtr.Zero, m_uFlags, IntPtr.Zero);
			m_p.Timer(PDTIMER_RESET, IntPtr.Zero);
			m_p.SetLine(1, strOperation, false, IntPtr.Zero);

			m_p.SetProgress(0, 100);
			m_uLastPercent = 0;
		}

		public void EndLogging()
		{
			if(m_p == null) return; // Might be freed/null already, don't assert

			m_p.StopProgressDialog();
			try { Marshal.ReleaseComObject(m_p); }
			catch(Exception) { Debug.Assert(false); }
			m_p = null;
		}

		public bool SetProgress(uint uPercent)
		{
			if(m_p == null) { Debug.Assert(false); return true; }

			if(uPercent != m_uLastPercent)
			{
				m_p.SetProgress(uPercent, 100);
				m_uLastPercent = uPercent;
			}

			return !m_p.HasUserCancelled();
		}

		public bool SetText(string strNewText, LogStatusType lsType)
		{
			if(m_p == null) { Debug.Assert(false); return true; }

			m_p.SetLine(2, strNewText, false, IntPtr.Zero);
			return !m_p.HasUserCancelled();
		}

		public bool ContinueWork()
		{
			if(m_p == null) { Debug.Assert(false); return true; }

			return !m_p.HasUserCancelled();
		}
	}

	[ComImport]
	[Guid("EBBC7C04-315E-11D2-B62F-006097DF5BD4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IProgressDialog
	{
		void StartProgressDialog(IntPtr hwndParent, IntPtr punkEnableModless,
			uint dwFlags, IntPtr pvReserved);
		void StopProgressDialog();

		void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pwzTitle);
		void SetAnimation(IntPtr hInstAnimation, uint idAnimation);

		[PreserveSig]
		[return: MarshalAs(UnmanagedType.Bool)]
		bool HasUserCancelled();

		void SetProgress(uint dwCompleted, uint dwTotal);
		void SetProgress64(ulong ullCompleted, ulong ullTotal);

		void SetLine(uint dwLineNum, [MarshalAs(UnmanagedType.LPWStr)] string pwzString,
			[MarshalAs(UnmanagedType.Bool)] bool fCompactPath, IntPtr pvReserved);
		void SetCancelMsg([MarshalAs(UnmanagedType.LPWStr)] string pwzCancelMsg,
			IntPtr pvReserved);

		void Timer(uint dwTimerAction, IntPtr pvReserved);
	}

	[ComImport]
	[Guid("F8383852-FCD3-11D1-A6B9-006097DF5BD4")]
	internal class Win32ProgressDialog { }
}
*/
