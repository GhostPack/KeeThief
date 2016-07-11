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
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;

using KeePassLib.Utility;

namespace KeePass.Native
{
	internal sealed class ShellLinkEx
	{
		private string m_strPath = null;
		public string Path
		{
			get { return m_strPath; }
			set { m_strPath = value; }
		}

		private string m_strArgs = null;
		public string Arguments
		{
			get { return m_strArgs; }
			set { m_strArgs = value; }
		}

		private string m_strDesc = null;
		public string Description
		{
			get { return m_strDesc; }
			set
			{
				if((value != null) && (value.Length >= NativeMethods.INFOTIPSIZE))
				{
					Debug.Assert(false);
					m_strDesc = StrUtil.CompactString3Dots(value,
						NativeMethods.INFOTIPSIZE - 1);
				}
				else m_strDesc = value;
			}
		}

		public ShellLinkEx()
		{
		}

		public ShellLinkEx(string strPath, string strArgs, string strDesc)
		{
			m_strPath = strPath;
			m_strArgs = strArgs;
			this.Description = strDesc; // Shortens description if necessary
		}

		public static ShellLinkEx Load(string strLnkFilePath)
		{
			try
			{
				CShellLink csl = new CShellLink();

				IShellLinkW sl = (csl as IShellLinkW);
				if(sl == null) { Debug.Assert(false); return null; }
				IPersistFile pf = (csl as IPersistFile);
				if(pf == null) { Debug.Assert(false); return null; }

				pf.Load(strLnkFilePath, (int)(NativeMethods.STGM.Read |
					NativeMethods.STGM.ShareDenyWrite));

				const int ccMaxPath = KeePassLib.Native.NativeMethods.MAX_PATH;
				const int ccInfoTip = NativeMethods.INFOTIPSIZE;

				ShellLinkEx r = new ShellLinkEx();

				StringBuilder sb = new StringBuilder(ccMaxPath + 1);
				sl.GetPath(sb, sb.Capacity, IntPtr.Zero, 0);
				r.Path = sb.ToString();

				sb = new StringBuilder(ccInfoTip + 1);
				sl.GetArguments(sb, sb.Capacity);
				r.Arguments = sb.ToString();

				sb = new StringBuilder(ccInfoTip + 1);
				sl.GetDescription(sb, sb.Capacity);
				r.Description = sb.ToString();

				return r;
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		public bool Save(string strLnkFilePath)
		{
			try
			{
				CShellLink csl = new CShellLink();

				IShellLinkW sl = (csl as IShellLinkW);
				if(sl == null) { Debug.Assert(false); return false; }
				IPersistFile pf = (csl as IPersistFile);
				if(pf == null) { Debug.Assert(false); return false; }

				if(!string.IsNullOrEmpty(m_strPath))
					sl.SetPath(m_strPath);
				if(!string.IsNullOrEmpty(m_strArgs))
					sl.SetArguments(m_strArgs);
				if(!string.IsNullOrEmpty(m_strDesc))
					sl.SetDescription(m_strDesc);

				pf.Save(strLnkFilePath, true);
				return true;
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}
	}

	[ComImport]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IShellLinkW
	{
		void GetPath([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
			int cch, IntPtr pfd, uint fFlags);

		void GetIDList(out IntPtr ppidl);
		void SetIDList(IntPtr pidl);

		void GetDescription([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
			int cch);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

		void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
			int cch);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

		void GetArguments([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

		void GetHotkey(out ushort pwHotkey);
		void SetHotkey(ushort wHotkey);

		void GetShowCmd(out int piShowCmd);
		void SetShowCmd(int iShowCmd);

		void GetIconLocation([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
			int cch, out int piIcon);
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
			int iIcon);

		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
			uint dwReserved);

		void Resolve(IntPtr hwnd, uint fFlags);

		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	internal class CShellLink { }
}
