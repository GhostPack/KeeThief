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
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Native;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public static partial class ClipboardUtil
	{
		private const int CntUnmanagedRetries = 10;
		private const int CntUnmanagedDelay = 100;

		private static bool OpenW(IntPtr hOwner, bool bEmpty)
		{
			IntPtr h = hOwner;
			if(h == IntPtr.Zero)
			{
				try
				{
					Form f = GlobalWindowManager.TopWindow;
					h = ((f != null) ? f.Handle : IntPtr.Zero);

					if(h == IntPtr.Zero) h = Program.MainForm.Handle;
				}
				catch(Exception) { Debug.Assert(false); }
			}

			for(int i = 0; i < CntUnmanagedRetries; ++i)
			{
				if(NativeMethods.OpenClipboard(h))
				{
					if(bEmpty)
					{
						if(!NativeMethods.EmptyClipboard()) { Debug.Assert(false); }
					}

					return true;
				}

				Thread.Sleep(CntUnmanagedDelay);
			}

			return false;
		}

		private static byte[] GetDataW(uint uFormat)
		{
			IntPtr h = NativeMethods.GetClipboardData(uFormat);
			if(h == IntPtr.Zero) return null;

			UIntPtr pSize = NativeMethods.GlobalSize(h);
			if(pSize == UIntPtr.Zero) return new byte[0];

			IntPtr hMem = NativeMethods.GlobalLock(h);
			if(hMem == IntPtr.Zero) { Debug.Assert(false); return null; }

			byte[] pbMem = new byte[pSize.ToUInt64()];
			Marshal.Copy(hMem, pbMem, 0, pbMem.Length);

			NativeMethods.GlobalUnlock(h); // May return false on success

			return pbMem;
		}

		private static string GetStringW(string strFormat, bool? bForceUni)
		{
			bool bUni = (bForceUni.HasValue ? bForceUni.Value : WinUtil.IsAtLeastWindows2000);

			uint uFormat = (bUni ? NativeMethods.CF_UNICODETEXT : NativeMethods.CF_TEXT);
			if(!string.IsNullOrEmpty(strFormat))
				uFormat = NativeMethods.RegisterClipboardFormat(strFormat);

			byte[] pb = GetDataW(uFormat);
			if(pb == null) { Debug.Assert(false); return null; }

			int nBytes = 0;
			for(int i = 0; i < pb.Length; i += (bUni ? 2 : 1))
			{
				if(bUni && (i == (pb.Length - 1))) { Debug.Assert(false); return null; }

				ushort uValue = (bUni ? (ushort)(((ushort)pb[i] << 8) |
					(ushort)pb[i + 1]) : (ushort)pb[i]);
				if(uValue == 0) break;

				nBytes += (bUni ? 2 : 1);
			}

			byte[] pbCharsOnly = new byte[nBytes];
			Array.Copy(pb, pbCharsOnly, nBytes);

			Encoding enc = (bUni ? new UnicodeEncoding(false, false) : Encoding.Default);
			return enc.GetString(pbCharsOnly);
		}

		private static bool SetDataW(uint uFormat, byte[] pbData)
		{
			UIntPtr pSize = new UIntPtr((uint)pbData.Length);
			IntPtr h = NativeMethods.GlobalAlloc(NativeMethods.GHND, pSize);
			if(h == IntPtr.Zero) { Debug.Assert(false); return false; }

			Debug.Assert(NativeMethods.GlobalSize(h).ToUInt64() >=
				(ulong)pbData.Length); // Might be larger

			IntPtr hMem = NativeMethods.GlobalLock(h);
			if(hMem == IntPtr.Zero)
			{
				Debug.Assert(false);
				NativeMethods.GlobalFree(h);
				return false;
			}

			Marshal.Copy(pbData, 0, hMem, pbData.Length);
			NativeMethods.GlobalUnlock(h); // May return false on success

			if(NativeMethods.SetClipboardData(uFormat, h) == IntPtr.Zero)
			{
				Debug.Assert(false);
				NativeMethods.GlobalFree(h);
				return false;
			}

			return true;
		}

		private static bool SetDataW(uint? uFormat, string strData, bool? bForceUni)
		{
			if(strData == null) { Debug.Assert(false); return false; }

			bool bUni = (bForceUni.HasValue ? bForceUni.Value : WinUtil.IsAtLeastWindows2000);

			uint uFmt = (uFormat.HasValue ? uFormat.Value : (bUni ?
				NativeMethods.CF_UNICODETEXT : NativeMethods.CF_TEXT));
			Encoding enc = (bUni ? new UnicodeEncoding(false, false) : Encoding.Default);

			byte[] pb = enc.GetBytes(strData);
			byte[] pbWithZero = new byte[pb.Length + (bUni ? 2 : 1)];
			Array.Copy(pb, pbWithZero, pb.Length);
			pbWithZero[pb.Length] = 0;
			if(bUni) pbWithZero[pb.Length + 1] = 0;

			return SetDataW(uFmt, pbWithZero);
		}

		/* private static bool SetDataWithLengthW(string strFormat, byte[] pbData)
		{
			uint uFormat = NativeMethods.RegisterClipboardFormat(strFormat);
			uint uLenFormat = NativeMethods.RegisterClipboardFormat(strFormat +
				"_Length");

			byte[] pbLen = MemUtil.UInt64ToBytes((ulong)pbData.LongLength);

			bool bResult = true;
			if(!SetDataW(uLenFormat, pbLen)) bResult = false;
			if(!SetDataW(uFormat, pbData)) bResult = false;
			return bResult;
		} */

		private static void CloseW()
		{
			if(!NativeMethods.CloseClipboard()) { Debug.Assert(false); }
		}

		private static bool AttachIgnoreFormatW()
		{
			if(!Program.Config.Security.UseClipboardViewerIgnoreFormat) return true;

			uint uIgnoreFmt = NativeMethods.RegisterClipboardFormat(
				ClipboardIgnoreFormatName);
			if(uIgnoreFmt == 0) { Debug.Assert(false); return false; }

			return SetDataW(uIgnoreFmt, PwDefs.ProductName, null);
		}
	}
}
