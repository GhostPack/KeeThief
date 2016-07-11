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
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.UI;

namespace KeePass.Native
{
	internal static partial class NativeMethods
	{
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetDllDirectory(string lpPathName);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindow(IntPtr hWnd);

		[DllImport("User32.dll")]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int nMsg,
			IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		internal static extern IntPtr SendMessageHDItem(IntPtr hWnd, int nMsg,
			IntPtr wParam, ref HDITEM hdItem);

		// [DllImport("User32.dll", EntryPoint = "SendMessage")]
		// private static extern IntPtr SendMessageLVGroup(IntPtr hWnd, int nMsg,
		//	IntPtr wParam, ref LVGROUP lvGroup);

		[DllImport("User32.dll", SetLastError = true)]
		internal static extern IntPtr SendMessageTimeout(IntPtr hWnd, int nMsg,
			IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout,
			ref IntPtr lpdwResult);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PostMessage(IntPtr hWnd, int nMsg,
			IntPtr wParam, IntPtr lParam);

		// [DllImport("User32.dll")]
		// internal static extern uint GetMessagePos();

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int RegisterWindowMessage(string lpString);

		// [DllImport("User32.dll")]
		// internal static extern IntPtr GetDesktopWindow();

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr FindWindowEx(IntPtr hwndParent,
			IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("User32.dll")]
		internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("User32.dll")]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("User32.dll", SetLastError = true)]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("User32.dll", EntryPoint = "GetClassLong")]
		private static extern IntPtr GetClassLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("User32.dll", EntryPoint = "GetClassLongPtr")]
		private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

		// [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		// private static extern int GetClassName(IntPtr hWnd,
		//	StringBuilder lpClassName, int nMaxCount);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IsIconic(IntPtr hWnd);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IsZoomed(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true)]
		private static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int GetWindowText(IntPtr hWnd,
			StringBuilder lpString, int nMaxCount);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

		// [DllImport("User32.dll")]
		// internal static extern IntPtr GetActiveWindow();

		[DllImport("User32.dll")]
		private static extern IntPtr GetForegroundWindow(); // Private, is wrapped

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc,
			IntPtr lParam);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RegisterHotKey(IntPtr hWnd, int id,
			uint fsModifiers, uint vk);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("User32.dll", EntryPoint = "SendInput", SetLastError = true)]
		internal static extern uint SendInput32(uint nInputs, INPUT32[] pInputs,
			int cbSize);

		[DllImport("User32.dll", EntryPoint = "SendInput", SetLastError = true)]
		internal static extern uint SendInput64Special(uint nInputs,
			SpecializedKeyboardINPUT64[] pInputs, int cbSize);

		[DllImport("User32.dll")]
		internal static extern IntPtr GetMessageExtraInfo();

		// [DllImport("User32.dll")]
		// internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
		//	IntPtr dwExtraInfo);

		[DllImport("User32.dll")]
		internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		internal static extern ushort VkKeyScan(char ch);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		internal static extern ushort VkKeyScanEx(char ch, IntPtr hKL);

		[DllImport("User32.dll")]
		internal static extern ushort GetKeyState(int vKey);

		[DllImport("User32.dll")]
		internal static extern ushort GetAsyncKeyState(int vKey);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)]
			bool fBlockIt);

		// [DllImport("User32.dll")]
		// [return: MarshalAs(UnmanagedType.Bool)]
		// internal static extern bool AttachThreadInput(uint idAttach,
		//	uint idAttachTo, [MarshalAs(UnmanagedType.Bool)] bool fAttach);

		[DllImport("User32.dll")]
		internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ChangeClipboardChain(IntPtr hWndRemove,
			IntPtr hWndNewNext);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EmptyClipboard();

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClipboard();

		[DllImport("User32.dll", SetLastError = true)]
		internal static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

		[DllImport("User32.dll", SetLastError = true)]
		internal static extern IntPtr GetClipboardData(uint uFormat);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern uint RegisterClipboardFormat(string lpszFormat);

		[DllImport("User32.dll")]
		internal static extern uint GetClipboardSequenceNumber();

		[DllImport("Kernel32.dll")]
		internal static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

		[DllImport("Kernel32.dll")]
		internal static extern IntPtr GlobalFree(IntPtr hMem);

		[DllImport("Kernel32.dll")]
		internal static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GlobalUnlock(IntPtr hMem);

		[DllImport("Kernel32.dll")]
		internal static extern UIntPtr GlobalSize(IntPtr hMem);

		[DllImport("ShlWApi.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PathCompactPathEx(StringBuilder pszOut,
			string szPath, uint cchMax, uint dwFlags);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DrawAnimatedRects(IntPtr hWnd,
			int idAni, [In] ref RECT lprcFrom, [In] ref RECT lprcTo);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetComboBoxInfo(IntPtr hWnd,
			ref COMBOBOXINFO pcbi);

		[DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateDesktop(string lpszDesktop,
			string lpszDevice, IntPtr pDevMode, UInt32 dwFlags,
			[MarshalAs(UnmanagedType.U4)] DesktopFlags dwDesiredAccess,
			IntPtr lpSecurityAttributes);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseDesktop(IntPtr hDesktop);

		// [DllImport("User32.dll", SetLastError = true)]
		// internal static extern IntPtr OpenDesktop(string lpszDesktop,
		//	UInt32 dwFlags, [MarshalAs(UnmanagedType.Bool)] bool fInherit,
		//	[MarshalAs(UnmanagedType.U4)] DesktopFlags dwDesiredAccess);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SwitchDesktop(IntPtr hDesktop);

		[DllImport("User32.dll", SetLastError = true)]
		internal static extern IntPtr OpenInputDesktop(uint dwFlags,
			[MarshalAs(UnmanagedType.Bool)] bool fInherit,
			[MarshalAs(UnmanagedType.U4)] DesktopFlags dwDesiredAccess);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetUserObjectInformation(IntPtr hObj,
			int nIndex, IntPtr pvInfo, uint nLength, ref uint lpnLengthNeeded);

		[DllImport("User32.dll", SetLastError = true)]
		internal static extern IntPtr GetThreadDesktop(uint dwThreadId);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetThreadDesktop(IntPtr hDesktop);

		[DllImport("Kernel32.dll")]
		internal static extern uint GetCurrentThreadId();

		// [DllImport("Imm32.dll")]
		// [return: MarshalAs(UnmanagedType.Bool)]
		// internal static extern bool ImmDisableIME(uint idThread);

		// [DllImport("Imm32.dll")]
		// internal static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateFile(string lpFileName,
			[MarshalAs(UnmanagedType.U4)] EFileAccess dwDesiredAccess,
			[MarshalAs(UnmanagedType.U4)] EFileShare dwShareMode,
			IntPtr lpSecurityAttributes,
			[MarshalAs(UnmanagedType.U4)] ECreationDisposition dwCreationDisposition,
			[MarshalAs(UnmanagedType.U4)] uint dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr hObject);

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern uint GetFileAttributes(string lpFileName);

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteFile(string lpFileName);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
			IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize,
			out uint lpBytesReturned, IntPtr lpOverlapped);

		[DllImport("ComCtl32.dll", CharSet = CharSet.Auto)]
		internal static extern Int32 TaskDialogIndirect([In] ref VtdConfig pTaskConfig,
			[Out] out int pnButton, [Out] out int pnRadioButton,
			[Out] [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);

		[DllImport("UxTheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
			string pszSubIdList);

		[DllImport("Shell32.dll")]
		internal static extern void SHChangeNotify(int wEventId, uint uFlags,
			IntPtr dwItem1, IntPtr dwItem2);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar,
			ref SCROLLINFO lpsi);

		// [DllImport("User32.dll")]
		// private static extern int SetScrollInfo(IntPtr hwnd, int fnBar,
		//	[In] ref SCROLLINFO lpsi, [MarshalAs(UnmanagedType.Bool)] bool fRedraw);

		// [DllImport("User32.dll")]
		// private static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy,
		//	IntPtr prcScroll, IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate,
		//	uint flags);

		[DllImport("User32.dll")]
		internal static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("User32.dll")]
		internal static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint uFlags);

		[DllImport("User32.dll")]
		internal static extern uint GetWindowThreadProcessId(IntPtr hWnd,
			[Out] out uint lpdwProcessId);

		// [DllImport("UxTheme.dll")]
		// internal static extern IntPtr OpenThemeData(IntPtr hWnd,
		//	[MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

		// [DllImport("UxTheme.dll")]
		// internal static extern uint CloseThemeData(IntPtr hTheme);

		// [DllImport("UxTheme.dll")]
		// internal extern static uint DrawThemeBackground(IntPtr hTheme, IntPtr hdc,
		//	int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect);	

		[DllImport("Gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteObject(IntPtr hObject);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		[DllImport("Shell32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SHGetFileInfo(string pszPath,
			uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo,
			uint uFlags);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DestroyIcon(IntPtr hIcon);

		// [DllImport("User32.dll", SetLastError = true)]
		// [return: MarshalAs(UnmanagedType.Bool)]
		// internal static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop,
		//	IntPtr hIcon, int cxWidth, int cyWidth, uint istepIfAniCur,
		//	IntPtr hbrFlickerFreeDraw, uint diFlags);

		[DllImport("User32.dll")]
		internal static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("User32.dll")]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("Gdi32.dll")]
		internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		[DllImport("WinMM.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PlaySound(string pszSound, IntPtr hmod,
			uint fdwSound);

		[DllImport("Shell32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr ShellExecute(IntPtr hwnd,
			string lpOperation, string lpFile, string lpParameters,
			string lpDirectory, int nShowCmd);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		internal static extern int MessageBox(IntPtr hWnd, string lpText,
			string lpCaption, [MarshalAs(UnmanagedType.U4)] MessageBoxFlags uType);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetProcessDPIAware();

		[DllImport("ShCore.dll")]
		internal static extern int SetProcessDpiAwareness(
			[MarshalAs(UnmanagedType.U4)] ProcessDpiAwareness a);

		[DllImport("Kernel32.dll")]
		internal static extern IntPtr CreateToolhelp32Snapshot(
			[MarshalAs(UnmanagedType.U4)] ToolHelpFlags dwFlags, uint th32ProcessID);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Process32First(IntPtr hSnapshot,
			ref PROCESSENTRY32 lppe);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Process32Next(IntPtr hSnapshot,
			ref PROCESSENTRY32 lppe);

		// [DllImport("Kernel32.dll", SetLastError = true)]
		// internal static extern IntPtr OpenProcess(uint dwDesiredAccess,
		//	[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

		// [DllImport("User32.dll")]
		// [return: MarshalAs(UnmanagedType.Bool)]
		// internal static extern bool IsImmersiveProcess(IntPtr hProcess);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateActCtx(ref ACTCTX pActCtx);

		[DllImport("Kernel32.dll")]
		internal static extern void ReleaseActCtx(IntPtr hActCtx);

		[DllImport("Kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ActivateActCtx(IntPtr hActCtx,
			ref UIntPtr lpCookie);

		[DllImport("Kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeactivateActCtx(uint dwFlags,
			UIntPtr ulCookie);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ShutdownBlockReasonCreate(IntPtr hWnd,
			[MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

		// [DllImport("User32.dll")]
		// internal static extern int GetSystemMetrics(int nIndex);
	}
}
