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
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

using KeePass.App.Configuration;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Native;
using KeePassLib.Utility;

namespace KeePass.Plugins
{
	public static class PlgxCache
	{
		private const string CacheDirName = "PluginCache";

		private static string m_strAppEnvID = null;

		private static string GetAppEnvID()
		{
			if(m_strAppEnvID != null) return m_strAppEnvID;

			StringBuilder sb = new StringBuilder();

			Assembly asm = null;
			AssemblyName asmName = null;
			try
			{
				asm = Assembly.GetExecutingAssembly();
				asmName = asm.GetName();
			}
			catch(Exception) { Debug.Assert(false); }

			try { sb.Append(asmName.Version.ToString(4)); }
			catch(Exception) { Debug.Assert(false); sb.Append(PwDefs.VersionString); }

#if DEBUG
			sb.Append("d");
#endif

			sb.Append(",PK=");
			try
			{
				byte[] pk = asmName.GetPublicKeyToken();
				sb.Append(Convert.ToBase64String(pk, Base64FormattingOptions.None));
			}
			catch(Exception) { Debug.Assert(false); sb.Append('?'); }

			sb.Append(",CLR=");
			sb.Append(Environment.Version.ToString(4));
			sb.Append(",Ptr=");
			sb.Append(IntPtr.Size.ToString());

			sb.Append(",OS=");
			PlatformID p = NativeLib.GetPlatformID();
			if((p == PlatformID.Win32NT) || (p == PlatformID.Win32S) ||
				(p == PlatformID.Win32Windows))
				sb.Append("Win");
			else if(p == PlatformID.WinCE) sb.Append("WinCE");
			else if(p == PlatformID.Xbox) sb.Append("Xbox");
			else if(p == PlatformID.Unix) sb.Append("Unix");
			else if(p == PlatformID.MacOSX) sb.Append("MacOSX");
			else sb.Append('?');

			m_strAppEnvID = sb.ToString();
			return m_strAppEnvID;
		}

		public static string GetCacheRoot()
		{
			if(Program.Config.Application.PluginCachePath.Length > 0)
			{
				string strRoot = SprEngine.Compile(Program.Config.Application.PluginCachePath,
					null);
				if(!string.IsNullOrEmpty(strRoot))
				{
					if(strRoot.EndsWith(new string(Path.DirectorySeparatorChar, 1)))
						strRoot = strRoot.Substring(0, strRoot.Length - 1);
					return strRoot;
				}
			}

			string strDataDir = AppConfigSerializer.LocalAppDataDirectory;
			// try
			// {
			//	DirectoryInfo diAppData = new DirectoryInfo(strDataDir);
			//	DirectoryInfo diRoot = diAppData.Root;
			//	DriveInfo di = new DriveInfo(diRoot.FullName);
			//	if(di.DriveType == DriveType.Network)
			//	{
			//		strDataDir = UrlUtil.EnsureTerminatingSeparator(
			//			UrlUtil.GetTempPath(), false);
			//		strDataDir = strDataDir.Substring(0, strDataDir.Length - 1);
			//	}
			// }
			// catch(Exception) { Debug.Assert(false); }

			return (strDataDir + Path.DirectorySeparatorChar + CacheDirName);
		}

		public static string GetCacheDirectory(PlgxPluginInfo plgx, bool bEnsureExists)
		{
			if(plgx == null) { Debug.Assert(false); return null; }

			StringBuilder sb = new StringBuilder();
			sb.Append(plgx.BaseFileName);
			sb.Append(':');
			sb.Append(Convert.ToBase64String(plgx.FileUuid.UuidBytes,
				Base64FormattingOptions.None));
			sb.Append(';');
			sb.Append(GetAppEnvID());

			byte[] pbID = StrUtil.Utf8.GetBytes(sb.ToString());
			SHA256Managed sha256 = new SHA256Managed();
			byte[] pbHash = sha256.ComputeHash(pbID);

			string strHash = Convert.ToBase64String(pbHash, Base64FormattingOptions.None);
			strHash = StrUtil.AlphaNumericOnly(strHash);
			if(strHash.Length > 20) strHash = strHash.Substring(0, 20);

			string strDir = GetCacheRoot() + Path.DirectorySeparatorChar + strHash;

			if(bEnsureExists && !Directory.Exists(strDir))
				Directory.CreateDirectory(strDir);

			return strDir;
		}

		public static string GetCacheFile(PlgxPluginInfo plgx, bool bMustExist,
			bool bCreateDirectory)
		{
			if(plgx == null) { Debug.Assert(false); return null; }

			// byte[] pbID = new byte[(int)PwUuid.UuidSize];
			// Array.Copy(pwPluginUuid.UuidBytes, 0, pbID, 0, pbID.Length);
			// Array.Reverse(pbID);
			// string strID = Convert.ToBase64String(pbID, Base64FormattingOptions.None);
			// strID = StrUtil.AlphaNumericOnly(strID);
			// if(strID.Length > 8) strID = strID.Substring(0, 8);

			string strFileName = StrUtil.AlphaNumericOnly(plgx.BaseFileName);
			if(strFileName.Length == 0) strFileName = "Plugin";
			strFileName += ".dll";

			string strDir = GetCacheDirectory(plgx, bCreateDirectory);
			string strPath = strDir + Path.DirectorySeparatorChar + strFileName;
			bool bExists = File.Exists(strPath);

			if(bMustExist && bExists)
			{
				try { File.SetLastAccessTime(strPath, DateTime.Now); }
				catch(Exception) { } // Might be locked by other KeePass instance
			}

			if(!bMustExist || bExists) return strPath;
			return null;
		}

		public static string AddCacheAssembly(string strAssemblyPath, PlgxPluginInfo plgx)
		{
			if(string.IsNullOrEmpty(strAssemblyPath)) { Debug.Assert(false); return null; }

			string strNewFile = GetCacheFile(plgx, false, true);
			File.Copy(strAssemblyPath, strNewFile, true);

			return strNewFile;
		}

		public static string AddCacheFile(string strNormalFile, PlgxPluginInfo plgx)
		{
			if(string.IsNullOrEmpty(strNormalFile)) { Debug.Assert(false); return null; }

			string strNewFile = UrlUtil.EnsureTerminatingSeparator(GetCacheDirectory(
				plgx, true), false) + UrlUtil.GetFileName(strNormalFile);
			File.Copy(strNormalFile, strNewFile, true);

			return strNewFile;
		}

		public static ulong GetUsedCacheSize()
		{
			string strRoot = GetCacheRoot();
			if(!Directory.Exists(strRoot)) return 0;

			DirectoryInfo di = new DirectoryInfo(strRoot);
			List<FileInfo> lFiles = UrlUtil.GetFileInfos(di, "*",
				SearchOption.AllDirectories);

			ulong uSize = 0;
			foreach(FileInfo fi in lFiles) { uSize += (ulong)fi.Length; }

			return uSize;
		}

		public static void Clear()
		{
			try
			{
				string strRoot = GetCacheRoot();
				if(!Directory.Exists(strRoot)) return;

				Directory.Delete(strRoot, true);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public static void DeleteOldFilesAsync()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(PlgxCache.DeleteOldFilesSafe));
		}

		private static void DeleteOldFilesSafe(object stateInfo)
		{
			try { DeleteOldFilesFunc(); }
			catch(Exception) { Debug.Assert(false); }
		}

		private static void DeleteOldFilesFunc()
		{
			string strRoot = GetCacheRoot();
			if(!Directory.Exists(strRoot)) return;
			
			DirectoryInfo di = new DirectoryInfo(strRoot);
			foreach(DirectoryInfo diSub in di.GetDirectories("*",
				SearchOption.TopDirectoryOnly))
			{
				try
				{
					if(ContainsOnlyOldFiles(diSub))
						Directory.Delete(diSub.FullName, true);
				}
				catch(Exception) { Debug.Assert(false); }
			}
		}

		private static bool ContainsOnlyOldFiles(DirectoryInfo di)
		{
			if((di.Name == ".") || (di.Name == "..")) return false;

			List<FileInfo> lFiles = UrlUtil.GetFileInfos(di, "*.dll",
				SearchOption.TopDirectoryOnly);
			bool bNew = false;
			foreach(FileInfo fi in lFiles)
			{
				bNew |= ((DateTime.Now - fi.LastAccessTime).TotalDays < 62.0);
				if(bNew) break;
			}

			return !bNew;
		}
	}
}
