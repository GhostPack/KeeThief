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
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

// using KeePass.Native;

namespace KeePass.UI
{
	public static class TaskbarList
	{
		private static bool m_bInitialized = false;
		private static ITaskbarList3 m_tbList = null;

		private static bool EnsureInitialized()
		{
			if(m_bInitialized) return (m_tbList != null);

			try
			{
				m_tbList = (ITaskbarList3)(new CTaskbarList());
				m_tbList.HrInit();
			}
			catch(Exception) { m_tbList = null; }

			m_bInitialized = true;
			return (m_tbList != null);
		}

		public static void SetProgressValue(Form fWindow, UInt64 ullCompleted,
			UInt64 ullTotal)
		{
			if(!EnsureInitialized()) return;

			try { m_tbList.SetProgressValue(fWindow.Handle, ullCompleted, ullTotal); }
			catch(Exception) { Debug.Assert(false); }
		}

		public static void SetProgressState(Form fWindow, TbpFlag tbpFlags)
		{
			if(!EnsureInitialized()) return;

			try { m_tbList.SetProgressState(fWindow.Handle, tbpFlags); }
			catch(Exception) { Debug.Assert(false); }
		}

		public static void SetOverlayIcon(Form fWindow, Icon iconOverlay,
			string strDescription)
		{
			if(!EnsureInitialized()) return;

			try
			{
				m_tbList.SetOverlayIcon(fWindow.Handle, ((iconOverlay == null) ?
					IntPtr.Zero : iconOverlay.Handle), strDescription);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		/* public static void SetThumbnailClip(IntPtr hWnd, Rectangle? rect)
		{
			if(!EnsureInitialized()) return;

			try
			{
				if(!rect.HasValue)
					m_tbList.SetThumbnailClip(hWnd, IntPtr.Zero);
				else
				{
					NativeMethods.RECT rc = new NativeMethods.RECT(rect.Value);
					IntPtr prc = Marshal.AllocCoTaskMem(Marshal.SizeOf(
						typeof(NativeMethods.RECT)));
					Marshal.StructureToPtr(rc, prc, false);

					m_tbList.SetThumbnailClip(hWnd, prc);

					Marshal.DestroyStructure(prc, typeof(NativeMethods.RECT));
					Marshal.FreeCoTaskMem(prc);
				}
			}
			catch(Exception) { Debug.Assert(false); }
		} */
	}

	// States are mutually exclusive, see MSDN
	public enum TbpFlag
	{
		NoProgress = 0x0,
		Indeterminate = 0x1,
		Normal = 0x2,
		Error = 0x4,
		Paused = 0x8
	}

	internal enum TbatFlag
	{
		None = 0x0,
		UseMdiThumbnail = 0x1,
		UseMdiLivePreview = 0x2
	}

	[ComImport]
	[Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEFAF")]
	// [Guid("C43DC798-95D1-4BEA-9030-BB99E2983A1A")] // 4
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ITaskbarList3
	{
		// ITaskbarList
		void HrInit();
		void AddTab(IntPtr hwnd);
		void DeleteTab(IntPtr hwnd);
		void ActivateTab(IntPtr hwnd);
		void SetActiveAlt(IntPtr hwnd);

		// ITaskbarList2
		void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)]
			bool fFullscreen);

		// ITaskbarList3
		void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
		void SetProgressState(IntPtr hwnd, TbpFlag tbpFlags);
		void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
		void UnregisterTab(IntPtr hwndTab);
		void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
		void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, TbatFlag tbatFlags);
		void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);
		void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);
		void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
		void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)]
			string pszDescription);
		void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)]
			string pszTip);
		void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
	}

	[ComImport]
	[Guid("56FDF344-FD6D-11D0-958A-006097C9A090")]
	[ClassInterface(ClassInterfaceType.None)]
	internal class CTaskbarList { }
}
