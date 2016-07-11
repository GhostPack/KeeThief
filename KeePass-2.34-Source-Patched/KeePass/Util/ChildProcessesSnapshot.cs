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
using System.Threading;
using System.Diagnostics;

using KeePass.Native;
using KeePass.Util;

using KeePassLib.Utility;

namespace KeePass.Util
{
	public sealed class ChildProcessesSnapshot
	{
		private readonly string m_strChildExeName; // May be null
		private readonly List<uint> m_lPids;

		public ChildProcessesSnapshot(string strChildExeName)
		{
			m_strChildExeName = (string.IsNullOrEmpty(strChildExeName) ?
				string.Empty : GetExeName(strChildExeName));

			m_lPids = GetChildPids();
		}

		private List<uint> GetChildPids()
		{
			List<uint> lPids = new List<uint>();
			if(KeePassLib.Native.NativeLib.IsUnix()) return lPids;

			try
			{
				uint pidThis = (uint)Process.GetCurrentProcess().Id;

				uint uEntrySize = (uint)Marshal.SizeOf(typeof(
					NativeMethods.PROCESSENTRY32));
				if(WinUtil.IsAtLeastWindows2000) // Unicode
				{
					int p = Marshal.SizeOf(typeof(IntPtr));
					if(p == 4)
					{
						Debug.Assert(uEntrySize ==
							NativeMethods.PROCESSENTRY32SizeUni32);
					}
					else if(p == 8)
					{
						Debug.Assert(uEntrySize ==
							NativeMethods.PROCESSENTRY32SizeUni64);
					}
				}

				IntPtr hSnap = NativeMethods.CreateToolhelp32Snapshot(
					NativeMethods.ToolHelpFlags.SnapProcess, 0);
				if(NativeMethods.IsInvalidHandleValue(hSnap))
				{
					Debug.Assert(false);
					return lPids;
				}

				for(int i = 0; i < int.MaxValue; ++i)
				{
					NativeMethods.PROCESSENTRY32 pe = new NativeMethods.PROCESSENTRY32();
					pe.dwSize = uEntrySize;

					bool b;
					if(i == 0) b = NativeMethods.Process32First(hSnap, ref pe);
					else b = NativeMethods.Process32Next(hSnap, ref pe);
					if(!b) break;

					if(pe.th32ProcessID == pidThis) continue;
					if(pe.th32ParentProcessID != pidThis) continue;

					if(!string.IsNullOrEmpty(m_strChildExeName))
					{
						if(pe.szExeFile == null) { Debug.Assert(false); continue; }

						string str = GetExeName(pe.szExeFile);
						if(!str.Equals(m_strChildExeName, StrUtil.CaseIgnoreCmp))
							continue;
					}

					lPids.Add(pe.th32ProcessID);
				}

				if(!NativeMethods.CloseHandle(hSnap)) { Debug.Assert(false); }
			}
			catch(Exception) { Debug.Assert(false); }

			return lPids;
		}

		private static char[] m_vTrimChars = null;
		private static string GetExeName(string strPath)
		{
			if(strPath == null) { Debug.Assert(false); return string.Empty; }

			if(m_vTrimChars == null)
				m_vTrimChars = new char[] { '\r', '\n', ' ', '\t', '\"', '\'' };

			string str = strPath.Trim(m_vTrimChars);
			str = UrlUtil.GetFileName(str);

			return str;
		}

		public void TerminateNewChildsAsync(int nDelayMs)
		{
			List<uint> lPids = GetChildPids();

			foreach(uint uPid in lPids)
			{
				if(m_lPids.IndexOf(uPid) < 0)
				{
					CpsTermInfo ti = new CpsTermInfo(uPid, m_strChildExeName,
						nDelayMs);

					ParameterizedThreadStart pts = new ParameterizedThreadStart(
						ChildProcessesSnapshot.DelayedTerminatePid);
					Thread th = new Thread(pts);
					th.Start(ti);
				}
			}
		}

		private sealed class CpsTermInfo
		{
			private readonly uint m_uPid;
			public uint ProcessId { get { return m_uPid; } }

			private readonly string m_strExeName;
			public string ExeName { get { return m_strExeName; } }

			private readonly int m_nDelayMs;
			public int Delay { get { return m_nDelayMs; } }

			public CpsTermInfo(uint uPid, string strExeName, int nDelayMs)
			{
				m_uPid = uPid;
				m_strExeName = strExeName;
				m_nDelayMs = nDelayMs;
			}
		}

		private static void DelayedTerminatePid(object oTermInfo)
		{
			try
			{
				CpsTermInfo ti = (oTermInfo as CpsTermInfo);
				if(ti == null) { Debug.Assert(false); return; }

				if(ti.Delay > 0) Thread.Sleep(ti.Delay);

				Process p = Process.GetProcessById((int)ti.ProcessId);
				if(p == null) { Debug.Assert(false); return; }

				// Verify that likely it's indeed the correct process
				if(!string.IsNullOrEmpty(ti.ExeName))
				{
					string str = GetExeName(p.MainModule.FileName);
					if(!str.Equals(ti.ExeName, StrUtil.CaseIgnoreCmp))
					{
						Debug.Assert(false);
						return;
					}
				}

				p.Kill();
				p.Close();
			}
			catch(ArgumentException) { } // Not running
			catch(Exception) { Debug.Assert(false); }
		}
	}
}
