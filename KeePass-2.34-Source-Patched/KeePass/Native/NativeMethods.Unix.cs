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

using KeePass.Forms;
using KeePass.UI;

using KeePassLib.Utility;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass.Native
{
	internal static partial class NativeMethods
	{
		/* private const string PathLibDo = "/usr/lib/gnome-do/libdo";
		private const UnmanagedType NtvStringType = UnmanagedType.LPStr;

		[DllImport(PathLibDo)]
		internal static extern void gnomedo_keybinder_init();

		[DllImport(PathLibDo)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool gnomedo_keybinder_bind(
			[MarshalAs(NtvStringType)] string strKey,
			BindKeyHandler lpfnHandler);

		[DllImport(PathLibDo)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool gnomedo_keybinder_unbind(
			[MarshalAs(NtvStringType)] string strKey,
			BindKeyHandler lpfnHandler);

		internal delegate void BindKeyHandler([MarshalAs(NtvStringType)]
			string strKey, IntPtr lpUserData); */

		/* private const string PathLibTomBoy = "/usr/lib/tomboy/libtomboy.so";

		[DllImport(PathLibTomBoy)]
		internal static extern void tomboy_keybinder_init();

		[DllImport(PathLibTomBoy)]
		internal static extern void tomboy_keybinder_bind(string strKey,
			BindKeyHandler lpHandler);

		[DllImport(PathLibTomBoy)]
		internal static extern void tomboy_keybinder_unbind(string strKey,
			BindKeyHandler lpHandler);

		internal delegate void BindKeyHandler(string strKey, IntPtr lpUserData); */

		private static bool LoseFocusUnix(Form fCurrent)
		{
			if(fCurrent == null) { Debug.Assert(false); return true; }

			try
			{
				string strCurrent = RunXDoTool("getwindowfocus -f");
				long lCurrent;
				long.TryParse(strCurrent.Trim(), out lCurrent);

				MainForm mf = Program.MainForm;
				Debug.Assert(mf == fCurrent);
				if(mf != null) mf.UIBlockWindowStateAuto(true);

				UIUtil.SetWindowState(fCurrent, FormWindowState.Minimized);

				int nStart = Environment.TickCount;
				while((Environment.TickCount - nStart) < 1000)
				{
					Application.DoEvents();

					string strActive = RunXDoTool("getwindowfocus -f");
					long lActive;
					long.TryParse(strActive.Trim(), out lActive);

					if(lActive != lCurrent) break;
				}

				if(mf != null) mf.UIBlockWindowStateAuto(false);

				return true;
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		internal static bool TryXDoTool()
		{
			return !string.IsNullOrEmpty(RunXDoTool("help"));
		}

		internal static bool TryXDoTool(bool bRequireWindowNameSupport)
		{
			if(!bRequireWindowNameSupport) return TryXDoTool();

			string str = RunXDoTool("getactivewindow getwindowname");
			if(string.IsNullOrEmpty(str)) return false;

			return !(str.Trim().Equals("usage: getactivewindow", StrUtil.CaseIgnoreCmp));
		}

		internal static string RunXDoTool(string strParams)
		{
			try
			{
				Application.DoEvents(); // E.g. for clipboard updates
				string strOutput = NativeLib.RunConsoleApp("xdotool", strParams);
				Application.DoEvents(); // E.g. for clipboard updates
				return (strOutput ?? string.Empty);
			}
			catch(Exception) { Debug.Assert(false); }

			return string.Empty;
		}

		/* private static Dictionary<string, Assembly> m_dAsms = null;
		internal static Assembly LoadAssembly(string strAsmName, string strFileName)
		{
			if(m_dAsms == null) m_dAsms = new Dictionary<string, Assembly>();

			Assembly asm;
			if(m_dAsms.TryGetValue(strAsmName, out asm))
				return asm;

			try
			{
				asm = Assembly.Load(strAsmName);
				if(asm != null) { m_dAsms[strAsmName] = asm; return asm; }
			}
			catch(Exception) { }

			try
			{
				asm = Assembly.LoadFrom(strFileName);
				if(asm != null) { m_dAsms[strAsmName] = asm; return asm; }
			}
			catch(Exception) { }

			for(int d = 0; d < 4; ++d)
			{
				string strDir;
				switch(d)
				{
					case 0: strDir = "/usr/lib/mono/gac"; break;
					case 1: strDir = "/usr/lib/cli"; break;
					case 2: strDir = "/usr/lib/mono"; break;
					case 3: strDir = "/lib/mono"; break;
					default: strDir = null; break;
				}
				if(string.IsNullOrEmpty(strDir)) { Debug.Assert(false); continue; }

				try
				{
					string[] vFiles = Directory.GetFiles(strDir, strFileName,
						SearchOption.AllDirectories);
					if(vFiles == null) continue;

					for(int i = vFiles.Length - 1; i >= 0; --i)
					{
						string strFoundName = UrlUtil.GetFileName(vFiles[i]);
						if(!strFileName.Equals(strFoundName, StrUtil.CaseIgnoreCmp))
							continue;

						try
						{
							asm = Assembly.LoadFrom(vFiles[i]);
							if(asm != null) { m_dAsms[strAsmName] = asm; return asm; }
						}
						catch(Exception) { }
					}
				}
				catch(Exception) { }
			}

			m_dAsms[strAsmName] = null;
			return null;
		}

		private static bool m_bGtkInitialized = false;
		internal static bool GtkEnsureInit()
		{
			if(m_bGtkInitialized) return true;

			try
			{
				Assembly asm = LoadAssembly("gtk-sharp", "gtk-sharp.dll");
				if(asm == null) return false;

				Type tApp = asm.GetType("Gtk.Application", true);
				MethodInfo miInitCheck = tApp.GetMethod("InitCheck",
					BindingFlags.Public | BindingFlags.Static);
				if(miInitCheck == null) { Debug.Assert(false); return false; }

				string[] vArgs = new string[0];
				bool bResult = (bool)miInitCheck.Invoke(null, new object[] {
					PwDefs.ShortProductName, vArgs });
				if(!bResult) return false;

				m_bGtkInitialized = true;
				return true;
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		} */
	}
}
