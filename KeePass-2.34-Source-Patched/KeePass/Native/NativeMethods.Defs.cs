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
using System.Diagnostics;
using System.Windows.Forms;

namespace KeePass.Native
{
	internal static partial class NativeMethods
	{
		internal const int WM_SETFOCUS = 0x0007;
		internal const int WM_KILLFOCUS = 0x0008;
		internal const int WM_KEYDOWN = 0x0100;
		internal const int WM_KEYUP = 0x0101;
		internal const int WM_DRAWCLIPBOARD = 0x0308;
		internal const int WM_CHANGECBCHAIN = 0x030D;
		internal const int WM_HOTKEY = 0x0312;
		internal const int WM_USER = 0x0400;
		internal const int WM_SYSCOMMAND = 0x0112;
		internal const int WM_POWERBROADCAST = 0x0218;
		internal const int WM_COPYDATA = 0x004A;
		// internal const int WM_MOUSEMOVE = 0x0200;
		internal const int WM_LBUTTONDOWN = 0x0201;
		internal const int WM_RBUTTONDOWN = 0x0204;
		internal const int WM_MBUTTONDOWN = 0x0207;
		// internal const int WM_MOUSEWHEEL = 0x020A;
		// internal const int WM_ERASEBKGND = 0x0014;

		internal const int WM_NOTIFY = 0x004E;

		// See Control.ReflectMessageInternal;
		// http://msdn.microsoft.com/en-us/library/eeah46xd.aspx
		internal const int WM_REFLECT = 0x2000;

		internal const int WM_NOTIFY_REFLECT = (WM_NOTIFY + WM_REFLECT);

		internal const int WM_GETTEXTLENGTH = 0x000E;
		internal const int WM_GETICON = 0x007F;

		internal const int HWND_BROADCAST = 0xFFFF;

		internal const uint SMTO_NORMAL = 0x0000;
		internal const uint SMTO_BLOCK = 0x0001;
		internal const uint SMTO_ABORTIFHUNG = 0x0002;
		internal const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

		internal const uint INPUT_MOUSE = 0;
		internal const uint INPUT_KEYBOARD = 1;
		internal const uint INPUT_HARDWARE = 2;

		// internal const int VK_RETURN = 0x0D;
		internal const int VK_SHIFT = 0x10;
		internal const int VK_CONTROL = 0x11;
		internal const int VK_MENU = 0x12;
		internal const int VK_CAPITAL = 0x14;
		// internal const int VK_ESCAPE = 0x1B;
		// internal const int VK_SPACE = 0x20;
		internal const int VK_LSHIFT = 0xA0;
		internal const int VK_RSHIFT = 0xA1;
		internal const int VK_LCONTROL = 0xA2;
		internal const int VK_RCONTROL = 0xA3;
		internal const int VK_LMENU = 0xA4;
		internal const int VK_RMENU = 0xA5;
		internal const int VK_LWIN = 0x5B;
		internal const int VK_RWIN = 0x5C;
		internal const int VK_SNAPSHOT = 0x2C;

		// internal const int VK_F5 = 0x74;
		// internal const int VK_F6 = 0x75;
		// internal const int VK_F7 = 0x76;
		// internal const int VK_F8 = 0x77;

		internal const uint KEYEVENTF_EXTENDEDKEY = 1;
		internal const uint KEYEVENTF_KEYUP = 2;
		internal const uint KEYEVENTF_UNICODE = 4;

		// internal const uint GW_CHILD = 5;
		internal const uint GW_HWNDNEXT = 2;

		internal const int GWL_STYLE = -16;
		internal const int GWL_EXSTYLE = -20;

		internal const int WS_VISIBLE = 0x10000000;

		// internal const int WS_EX_COMPOSITED = 0x02000000;

		internal const int SW_SHOW = 5;

		internal const int GCLP_HICON = -14;
		internal const int GCLP_HICONSM = -34;

		internal const int ICON_SMALL = 0;
		internal const int ICON_BIG = 1;
		internal const int ICON_SMALL2 = 2;

		internal const int EM_GETCHARFORMAT = WM_USER + 58;
		internal const int EM_SETCHARFORMAT = WM_USER + 68;

		internal const int ES_WANTRETURN = 0x1000;

		internal const int SCF_SELECTION = 0x0001;

		internal const uint CFM_LINK = 0x00000020;
		internal const uint CFE_LINK = 0x00000020;

		internal const int SC_MINIMIZE = 0xF020;
		internal const int SC_MAXIMIZE = 0xF030;

		internal const int IDANI_CAPTION = 3;

		internal const int PBT_APMQUERYSUSPEND = 0x0000;
		internal const int PBT_APMSUSPEND = 0x0004;

		internal const int ECM_FIRST = 0x1500;
		internal const int EM_SETCUEBANNER = ECM_FIRST + 1;

		internal const uint INVALID_FILE_ATTRIBUTES = 0xFFFFFFFFU;

		internal const uint FSCTL_LOCK_VOLUME = 589848;
		internal const uint FSCTL_UNLOCK_VOLUME = 589852;

		internal const int LVM_FIRST = 0x1000;
		// internal const int LVM_ENSUREVISIBLE = LVM_FIRST + 19;
		internal const int LVM_SCROLL = LVM_FIRST + 20;
		// internal const int LVM_SETGROUPINFO = LVM_FIRST + 147; // >= Vista
		// internal const int LVM_GETGROUPINFOBYINDEX = LVM_FIRST + 153; // >= Vista

		internal const int TV_FIRST = 0x1100;
		internal const int TVM_GETTOOLTIPS = TV_FIRST + 25;
		internal const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;

		internal const int TTM_SETDELAYTIME = WM_USER + 3;
		internal const int TTM_GETDELAYTIME = WM_USER + 21;
		internal const int TTDT_AUTOPOP = 2;

		internal const int WM_MOUSEACTIVATE = 0x21;
		internal const int MA_ACTIVATE = 1;
		internal const int MA_ACTIVATEANDEAT = 2;
		internal const int MA_NOACTIVATE = 3;
		internal const int MA_NOACTIVATEANDEAT = 4;

		internal const int BCM_SETSHIELD = 0x160C;

		internal const int SHCNE_ASSOCCHANGED = 0x08000000;
		internal const uint SHCNF_IDLIST = 0x0000;

		// internal const uint SW_INVALIDATE = 0x0002;

		internal const uint TVS_FULLROWSELECT = 0x1000;
		internal const uint TVS_NONEVENHEIGHT = 0x4000;
		internal const uint TVS_EX_DOUBLEBUFFER = 0x0004;
		internal const uint TVS_EX_FADEINOUTEXPANDOS = 0x0040;

		internal const int HDI_FORMAT = 0x0004;
		internal const int HDF_SORTUP = 0x0400;
		internal const int HDF_SORTDOWN = 0x0200;
		internal const int LVM_GETHEADER = LVM_FIRST + 31;
		internal const int HDM_FIRST = 0x1200;
		internal const int HDM_GETITEMA = HDM_FIRST + 3;
		internal const int HDM_GETITEMW = HDM_FIRST + 11;
		internal const int HDM_SETITEMA = HDM_FIRST + 4;
		internal const int HDM_SETITEMW = HDM_FIRST + 12;

		internal const int OFN_DONTADDTORECENT = 0x02000000;

		internal const uint SHGFI_DISPLAYNAME = 0x000000200;
		internal const uint SHGFI_ICON = 0x000000100;
		internal const uint SHGFI_TYPENAME = 0x000000400;
		internal const uint SHGFI_SMALLICON = 0x000000001;

		internal const uint MOD_ALT = 1;
		internal const uint MOD_CONTROL = 2;
		internal const uint MOD_SHIFT = 4;
		internal const uint MOD_WIN = 8;

		internal const int IDHOT_SNAPDESKTOP = -2;
		internal const int IDHOT_SNAPWINDOW = -1;

		internal const uint GHND = 0x0042;
		internal const uint GMEM_MOVEABLE = 0x0002;
		internal const uint GMEM_ZEROINIT = 0x0040;

		internal const uint CF_TEXT = 1;
		internal const uint CF_UNICODETEXT = 13;

		internal const uint SND_ASYNC = 0x0001;
		internal const uint SND_FILENAME = 0x00020000;
		internal const uint SND_NODEFAULT = 0x0002;

		internal const int LOGPIXELSX = 88;
		internal const int LOGPIXELSY = 90;

		// internal const int SM_CXSMICON = 49;
		// internal const int SM_CYSMICON = 50;

		// internal const uint PROCESS_QUERY_INFORMATION = 0x0400;

		internal const uint ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x04;

		internal const int INFOTIPSIZE = 1024;

		// internal const uint DI_NORMAL = 0x0003;

		// internal const int LVN_FIRST = -100;
		// internal const int LVN_LINKCLICK = LVN_FIRST - 84;

		// internal const uint LVGF_NONE = 0x00000000;
		// internal const uint LVGF_HEADER = 0x00000001;
		// internal const uint LVGF_FOOTER = 0x00000002;
		// internal const uint LVGF_STATE = 0x00000004;
		// internal const uint LVGF_ALIGN = 0x00000008;
		// internal const uint LVGF_GROUPID = 0x00000010;
		// internal const uint LVGF_SUBTITLE = 0x00000100;
		// internal const uint LVGF_TASK = 0x00000200;
		// internal const uint LVGF_DESCRIPTIONTOP = 0x00000400;
		// internal const uint LVGF_DESCRIPTIONBOTTOM = 0x00000800;
		// internal const uint LVGF_TITLEIMAGE = 0x00001000;
		// internal const uint LVGF_EXTENDEDIMAGE = 0x00002000;
		// internal const uint LVGF_ITEMS = 0x00004000;
		// internal const uint LVGF_SUBSET = 0x00008000;
		// internal const uint LVGF_SUBSETITEMS = 0x00010000;

		// internal const uint LVGS_NORMAL = 0x00000000;
		// internal const uint LVGS_COLLAPSED = 0x00000001;
		// internal const uint LVGS_HIDDEN = 0x00000002;
		// internal const uint LVGS_NOHEADER = 0x00000004;
		// internal const uint LVGS_COLLAPSIBLE = 0x00000008;
		// internal const uint LVGS_FOCUSED = 0x00000010;
		// internal const uint LVGS_SELECTED = 0x00000020;
		// internal const uint LVGS_SUBSETED = 0x00000040;
		// internal const uint LVGS_SUBSETLINKFOCUSED = 0x00000080;

		// private const int TTN_FIRST = -520;
		// internal const int TTN_NEEDTEXTA = TTN_FIRST;
		// internal const int TTN_NEEDTEXTW = TTN_FIRST - 10;

		[return: MarshalAs(UnmanagedType.Bool)]
		internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		internal enum ComboBoxButtonState : uint
		{
			STATE_SYSTEM_NONE = 0,
			STATE_SYSTEM_INVISIBLE = 0x00008000,
			STATE_SYSTEM_PRESSED = 0x00000008
		}

		[Flags]
		internal enum DesktopFlags : uint
		{
			ReadObjects = 0x0001,
			CreateWindow = 0x0002,
			CreateMenu = 0x0004,
			HookControl = 0x0008,
			JournalRecord = 0x0010,
			JournalPlayback = 0x0020,
			Enumerate = 0x0040,
			WriteObjects = 0x0080,
			SwitchDesktop = 0x0100,
		}

		[Flags]
		internal enum EFileAccess : uint
		{
			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
			GenericExecute = 0x20000000,
			GenericAll = 0x10000000
		}

		[Flags]
		internal enum EFileShare : uint
		{
			None = 0x00000000,
			Read = 0x00000001,
			Write = 0x00000002,
			Delete = 0x00000004
		}

		internal enum ECreationDisposition : uint
		{
			CreateNew = 1,
			CreateAlways = 2,
			OpenExisting = 3,
			OpenAlways = 4,
			TruncateExisting = 5
		}

		private enum ScrollBarDirection : int
		{
			SB_HORZ = 0,
			SB_VERT = 1,
			SB_CTL = 2,
			SB_BOTH = 3
		}

		private enum ScrollInfoMask : uint
		{
			SIF_RANGE = 0x1,
			SIF_PAGE = 0x2,
			SIF_POS = 0x4,
			SIF_DISABLENOSCROLL = 0x8,
			SIF_TRACKPOS = 0x10,
			SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
		}

		[Flags]
		internal enum MessageBoxFlags : uint
		{
			// Buttons
			MB_ABORTRETRYIGNORE = 0x00000002,
			MB_CANCELTRYCONTINUE = 0x00000006,
			MB_HELP = 0x00004000,
			MB_OK = 0,
			MB_OKCANCEL = 0x00000001,
			MB_RETRYCANCEL = 0x00000005,
			MB_YESNO = 0x00000004,
			MB_YESNOCANCEL = 0x00000003,

			// Icons
			MB_ICONEXCLAMATION = 0x00000030,
			MB_ICONWARNING = 0x00000030,
			MB_ICONINFORMATION = 0x00000040,
			MB_ICONASTERISK = 0x00000040,
			MB_ICONQUESTION = 0x00000020,
			MB_ICONSTOP = 0x00000010,
			MB_ICONERROR = 0x00000010,
			MB_ICONHAND = 0x00000010,

			// Default buttons
			MB_DEFBUTTON1 = 0,
			MB_DEFBUTTON2 = 0x00000100,
			MB_DEFBUTTON3 = 0x00000200,
			MB_DEFBUTTON4 = 0x00000300,

			// Modality
			MB_APPLMODAL = 0,
			MB_SYSTEMMODAL = 0x00001000,
			MB_TASKMODAL = 0x00002000,

			// Other options
			MB_DEFAULT_DESKTOP_ONLY = 0x00020000,
			MB_RIGHT = 0x00080000,
			MB_RTLREADING = 0x00100000,
			MB_SETFOREGROUND = 0x00010000,
			MB_TOPMOST = 0x00040000,
			MB_SERVICE_NOTIFICATION = 0x00200000
		}

		// See DialogResult
		/* internal enum CommandID
		{
			None = 0,
			OK = 1, // IDOK
			Cancel = 2, // IDCANCEL
			Abort = 3, // IDABORT
			Retry = 4, // IDRETRY
			Ignore = 5, // IDIGNORE
			Yes = 6, // IDYES
			No = 7, // IDNO
			Close = 8, // IDCLOSE
			Help = 9, // IDHELP
			TryAgain = 10, // IDTRYAGAIN
			Continue = 11, // IDCONTINUE
			TimeOut = 32000 // IDTIMEOUT
		} */

		[Flags]
		internal enum ToolHelpFlags : uint
		{
			SnapHeapList = 0x00000001,
			SnapProcess = 0x00000002,
			SnapThread = 0x00000004,
			SnapModule = 0x00000008,
			SnapModule32 = 0x00000010,

			SnapAll = (SnapHeapList | SnapProcess | SnapThread | SnapModule),

			Inherit = 0x80000000U
		}

		// https://msdn.microsoft.com/en-us/library/windows/desktop/aa380337.aspx
		[Flags]
		internal enum STGM : uint
		{
			Read = 0x00000000,
			Write = 0x00000001,
			ReadWrite = 0x00000002,

			ShareDenyNone = 0x00000040,
			ShareDenyRead = 0x00000030,
			ShareDenyWrite = 0x00000020,
			ShareExclusive = 0x00000010
		}

		internal enum ProcessDpiAwareness : uint
		{
			Unaware = 0,
			SystemAware = 1,
			PerMonitorAware = 2
		}
	}
}
