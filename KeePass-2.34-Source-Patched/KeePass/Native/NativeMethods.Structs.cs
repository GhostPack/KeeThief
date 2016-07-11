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
using System.Drawing;

namespace KeePass.Native
{
	internal static partial class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MOUSEINPUT32_WithSkip
		{
			public uint __Unused0; // See INPUT32 structure

			public int X;
			public int Y;
			public uint MouseData;
			public uint Flags;
			public uint Time;
			public IntPtr ExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct KEYBDINPUT32_WithSkip
		{
			public uint __Unused0; // See INPUT32 structure

			public ushort VirtualKeyCode;
			public ushort ScanCode;
			public uint Flags;
			public uint Time;
			public IntPtr ExtraInfo;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct INPUT32
		{
			[FieldOffset(0)]
			public uint Type;
			[FieldOffset(0)]
			public MOUSEINPUT32_WithSkip MouseInput;
			[FieldOffset(0)]
			public KEYBDINPUT32_WithSkip KeyboardInput;
		}

		// INPUT.KI (40). vk: 8, sc: 10, fl: 12, t: 16, ex: 24
		[StructLayout(LayoutKind.Explicit, Size = 40)]
		internal struct SpecializedKeyboardINPUT64
		{
			[FieldOffset(0)]
			public uint Type;
			[FieldOffset(8)]
			public ushort VirtualKeyCode;
			[FieldOffset(10)]
			public ushort ScanCode;
			[FieldOffset(12)]
			public uint Flags;
			[FieldOffset(16)]
			public uint Time;
			[FieldOffset(24)]
			public IntPtr ExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct CHARFORMAT2
		{
			public UInt32 cbSize;
			public UInt32 dwMask;
			public UInt32 dwEffects;
			public Int32 yHeight;
			public Int32 yOffset;
			public UInt32 crTextColor;
			public Byte bCharSet;
			public Byte bPitchAndFamily;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szFaceName;

			public UInt16 wWeight;
			public UInt16 sSpacing;
			public Int32 crBackColor;
			public Int32 lcid;
			public UInt32 dwReserved;
			public Int16 sStyle;
			public Int16 wKerning;
			public Byte bUnderlineType;
			public Byte bAnimation;
			public Byte bRevAuthor;
			public Byte bReserved1;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct RECT
		{
			public Int32 Left;
			public Int32 Top;
			public Int32 Right;
			public Int32 Bottom;

			public RECT(Rectangle rect)
			{
				this.Left = rect.Left;
				this.Top = rect.Top;
				this.Right = rect.Right;
				this.Bottom = rect.Bottom;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct COMBOBOXINFO
		{
			public Int32 cbSize;
			public RECT rcItem;
			public RECT rcButton;

			[MarshalAs(UnmanagedType.U4)]
			public ComboBoxButtonState buttonState;

			public IntPtr hwndCombo;
			public IntPtr hwndEdit;
			public IntPtr hwndList;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MARGINS
		{
			public Int32 Left;
			public Int32 Right;
			public Int32 Top;
			public Int32 Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public Int32 cbData;
			public IntPtr lpData;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SCROLLINFO
		{
			public uint cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct HDITEM
		{
			public UInt32 mask;
			public Int32 cxy;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pszText;

			public IntPtr hbm;
			public Int32 cchTextMax;
			public Int32 fmt;
			public IntPtr lParam;
			public Int32 iImage;
			public Int32 iOrder;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct NMHDR
		{
			public IntPtr hwndFrom;
			public IntPtr idFrom;
			public uint code;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst =
				KeePassLib.Native.NativeMethods.MAX_PATH)]
			public string szDisplayName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		/* // LVGROUP for Windows Vista and higher
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct LVGROUP
		{
			public uint cbSize;
			public uint mask;

			// [MarshalAs(UnmanagedType.LPWStr)]
			// public StringBuilder pszHeader;
			public IntPtr pszHeader;
			public int cchHeader;

			// [MarshalAs(UnmanagedType.LPWStr)]
			// public StringBuilder pszFooter;
			public IntPtr pszFooter;
			public int cchFooter;

			public int iGroupId;
			public uint stateMask;
			public uint state;
			public uint uAlign;

			// [MarshalAs(UnmanagedType.LPWStr)]
			// public StringBuilder pszSubtitle;
			public IntPtr pszSubtitle;
			public uint cchSubtitle;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszTask;
			// public StringBuilder pszTask;
			// public IntPtr pszTask;
			public uint cchTask;

			// [MarshalAs(UnmanagedType.LPWStr)]
			// public StringBuilder pszDescriptionTop;
			public IntPtr pszDescriptionTop;
			public uint cchDescriptionTop;

			// [MarshalAs(UnmanagedType.LPWStr)]
			// public StringBuilder pszDescriptionBottom;
			public IntPtr pszDescriptionBottom;
			public uint cchDescriptionBottom;

			public int iTitleImage;
			public int iExtendedImage;
			public int iFirstItem;
			public uint cItems;

			// [MarshalAs(UnmanagedType.LPWStr)]
			// public StringBuilder pszSubsetTitle;
			public IntPtr pszSubsetTitle;
			public uint cchSubsetTitle;

			[Conditional("DEBUG")]
			internal void AssertSize()
			{
				if(IntPtr.Size == 4)
				{
					Debug.Assert(Marshal.SizeOf(this) == 96);
				}
				else if(IntPtr.Size == 8)
				{
					Debug.Assert(Marshal.SizeOf(this) == 152);
				}
				else { Debug.Assert(false); }
			}
		} */

		internal const uint PROCESSENTRY32SizeUni32 = 556;
		internal const uint PROCESSENTRY32SizeUni64 = 568;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct PROCESSENTRY32
		{
			public uint dwSize;
			public uint cntUsage;
			public uint th32ProcessID;
			public UIntPtr th32DefaultHeapID;
			public uint th32ModuleID;
			public uint cntThreads;
			public uint th32ParentProcessID;
			public int pcPriClassBase;
			public uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst =
				KeePassLib.Native.NativeMethods.MAX_PATH)]
			public string szExeFile;
		}

		internal const uint ACTCTXSize32 = 32;
		internal const uint ACTCTXSize64 = 56;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct ACTCTX
		{
			public uint cbSize;
			public uint dwFlags;
			[MarshalAs(UnmanagedType.LPTStr)] // Not LPWStr, see source code
			public string lpSource;
			public ushort wProcessorArchitecture;
			public ushort wLangId;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpAssemblyDirectory;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpResourceName;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpApplicationName;
			public IntPtr hModule;
		}
	}
}
