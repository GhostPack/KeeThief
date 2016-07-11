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
using System.Diagnostics;

using KeePass.Native;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Utility;

using Microsoft.Win32;

namespace KeePass.Util
{
	public static class ShellUtil
	{
		public static void RegisterExtension(string strFileExt, string strExtId,
			string strFullExtName, string strAppPath, string strAppName,
			bool bShowSuccessMessage)
		{
			try
			{
				RegistryKey kClassesRoot = Registry.ClassesRoot;

				try { kClassesRoot.CreateSubKey("." + strFileExt); }
				catch(Exception) { }
				RegistryKey kFileExt = kClassesRoot.OpenSubKey("." + strFileExt, true);
				kFileExt.SetValue(string.Empty, strExtId, RegistryValueKind.String);
				kFileExt.Close();

				try { kClassesRoot.CreateSubKey(strExtId); }
				catch(Exception) { }
				RegistryKey kExtInfo = kClassesRoot.OpenSubKey(strExtId, true);

				kExtInfo.SetValue(string.Empty, strFullExtName, RegistryValueKind.String);

				try { kExtInfo.CreateSubKey("DefaultIcon"); }
				catch(Exception) { }
				RegistryKey kIcon = kExtInfo.OpenSubKey("DefaultIcon", true);
				if(strAppPath.IndexOfAny(new char[]{ ' ', '\t' }) < 0)
					kIcon.SetValue(string.Empty, strAppPath + ",0", RegistryValueKind.String);
				else
					kIcon.SetValue(string.Empty, "\"" + strAppPath + "\",0", RegistryValueKind.String);
				kIcon.Close();

				try { kExtInfo.CreateSubKey("shell"); }
				catch(Exception) { }
				RegistryKey kShell = kExtInfo.OpenSubKey("shell", true);

				try { kShell.CreateSubKey("open"); }
				catch(Exception) { }
				RegistryKey kShellOpen = kShell.OpenSubKey("open", true);

				kShellOpen.SetValue(string.Empty, @"&Open with " + strAppName, RegistryValueKind.String);

				try { kShellOpen.CreateSubKey("command"); }
				catch(Exception) { }
				RegistryKey kShellCommand = kShellOpen.OpenSubKey("command", true);
				kShellCommand.SetValue(string.Empty, "\"" + strAppPath + "\" \"%1\"", RegistryValueKind.String);
				kShellCommand.Close();

				kShellOpen.Close();
				kShell.Close();
				kExtInfo.Close();

				ShChangeNotify();

				if(bShowSuccessMessage)
					MessageService.ShowInfo(KPRes.FileExtInstallSuccess);
			}
			catch(Exception)
			{
				MessageService.ShowWarning(KPRes.FileExtInstallFailed);
			}
		}

		public static void UnregisterExtension(string strFileExt, string strExtId)
		{
			try
			{
				RegistryKey kClassesRoot = Registry.ClassesRoot;

				kClassesRoot.DeleteSubKeyTree("." + strFileExt);
				kClassesRoot.DeleteSubKeyTree(strExtId);

				ShChangeNotify();
			}
			catch(Exception) { }
		}

		private static void ShChangeNotify()
		{
			try
			{
				NativeMethods.SHChangeNotify(NativeMethods.SHCNE_ASSOCCHANGED,
					NativeMethods.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private const string AutoRunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
		public static void SetStartWithWindows(string strAppName, string strAppPath,
			bool bAutoStart)
		{
			try
			{
				if(bAutoStart)
					Registry.SetValue("HKEY_CURRENT_USER\\" + AutoRunKey, strAppName,
						strAppPath, RegistryValueKind.String);
				else
				{
					RegistryKey kRun = Registry.CurrentUser.OpenSubKey(AutoRunKey, true);
					kRun.DeleteValue(strAppName);
					kRun.Close();
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static bool GetStartWithWindows(string strAppName)
		{
			try
			{
				string strNotFound = Guid.NewGuid().ToString();
				string strResult = (Registry.GetValue("HKEY_CURRENT_USER\\" + AutoRunKey,
					strAppName, strNotFound) as string);

				if((strResult != null) && (strResult != strNotFound) &&
					(strResult.Length > 0))
				{
					return true;
				}
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		/* private const string PreLoadKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
		public static void RegisterPreLoad(string strAppName, string strAppPath,
			string strCmdLineOptions, bool bPreLoad)
		{
			try
			{
				if(bPreLoad)
				{
					string strValue = strAppPath;
					if(!string.IsNullOrEmpty(strCmdLineOptions))
						strValue += " " + strCmdLineOptions;

					Registry.SetValue("HKEY_LOCAL_MACHINE\\" + PreLoadKey, strAppName,
						strValue, RegistryValueKind.String);
				}
				else
				{
					RegistryKey kRun = Registry.LocalMachine.OpenSubKey(PreLoadKey, true);
					kRun.DeleteValue(strAppName);
					kRun.Close();
				}
			}
			catch(Exception) { Debug.Assert(false); }
		} */
	}
}
