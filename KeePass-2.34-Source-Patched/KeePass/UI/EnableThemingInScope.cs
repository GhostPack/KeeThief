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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Native;
using KeePass.Util;

namespace KeePass.UI
{
	// Code derived from http://support.microsoft.com/kb/830033/
	public sealed class EnableThemingInScope : IDisposable
	{
		private UIntPtr? m_nuCookie = null;

		private static object m_oSync = new object();
		private static IntPtr? m_nhCtx = null;

		public EnableThemingInScope(bool bEnable)
		{
			if(!bEnable) return;
			if(KeePassLib.Native.NativeLib.IsUnix()) return;

			try
			{
				if(OSFeature.Feature.IsPresent(OSFeature.Themes))
				{
					if(EnsureActCtxCreated())
					{
						UIntPtr u = UIntPtr.Zero;
						if(NativeMethods.ActivateActCtx(m_nhCtx.Value, ref u))
							m_nuCookie = u;
						else { Debug.Assert(false); }
					}
					else { Debug.Assert(false); }
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		~EnableThemingInScope()
		{
			Debug.Assert(!m_nuCookie.HasValue);
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(!m_nuCookie.HasValue) return;

			try
			{
				if(NativeMethods.DeactivateActCtx(0, m_nuCookie.Value))
					m_nuCookie = null;
				else { Debug.Assert(false); }
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void StaticDispose()
		{
			if(!m_nhCtx.HasValue) return;

			try
			{
				NativeMethods.ReleaseActCtx(m_nhCtx.Value);
				m_nhCtx = null;
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private static bool EnsureActCtxCreated()
		{
			lock(m_oSync)
			{
				if(m_nhCtx.HasValue) return true;

				string strAsmLoc;
				FileIOPermission p = new FileIOPermission(PermissionState.None);
				p.AllFiles = FileIOPermissionAccess.PathDiscovery;
				p.Assert();
				try { strAsmLoc = typeof(object).Assembly.Location; }
				finally { CodeAccessPermission.RevertAssert(); }
				if(string.IsNullOrEmpty(strAsmLoc)) { Debug.Assert(false); return false; }

				string strInstDir = Path.GetDirectoryName(strAsmLoc);
				string strMfLoc = Path.Combine(strInstDir, "XPThemes.manifest");

				NativeMethods.ACTCTX ctx = new NativeMethods.ACTCTX();
				ctx.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.ACTCTX));
				Debug.Assert(((IntPtr.Size == 4) && (ctx.cbSize ==
					NativeMethods.ACTCTXSize32)) || ((IntPtr.Size == 8) &&
					(ctx.cbSize == NativeMethods.ACTCTXSize64)));

				ctx.lpSource = strMfLoc;
				ctx.lpAssemblyDirectory = strInstDir;
				ctx.dwFlags = NativeMethods.ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID;

				m_nhCtx = NativeMethods.CreateActCtx(ref ctx);
				if(NativeMethods.IsInvalidHandleValue(m_nhCtx.Value))
				{
					Debug.Assert(false);
					m_nhCtx = null;
					return false;
				}
			}

			return true;
		}
	}
}
