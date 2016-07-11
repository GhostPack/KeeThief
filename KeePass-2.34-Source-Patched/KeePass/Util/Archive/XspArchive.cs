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
using System.Diagnostics;

using KeePassLib.Utility;

namespace KeePass.Util.Archive
{
	internal sealed class XspArchive
	{
		private const ulong g_uSig = 0x562CBA7C4F9AF297UL;
		private const ushort g_uVer = 1;

		private Dictionary<string, byte[]> m_dItems =
			new Dictionary<string, byte[]>();
		public IDictionary<string, byte[]> Items
		{
			get { return m_dItems; }
		}

		public XspArchive()
		{
		}

		internal static void CreateFile(string strXspFile, string strSourceDir)
		{
			try
			{
				string strExe = WinUtil.GetExecutable();

				string strFile = UrlUtil.MakeAbsolutePath(strExe, strXspFile);

				string strDir = UrlUtil.EnsureTerminatingSeparator(
					strSourceDir, false);
				strDir = strDir.Substring(0, strDir.Length - 1);
				strDir = UrlUtil.EnsureTerminatingSeparator(
					UrlUtil.MakeAbsolutePath(strExe, strDir), false);

				CreateFilePriv(strFile, strDir);
			}
			catch(Exception ex)
			{
				MessageService.ShowWarningExcp(ex);
			}
		}

		private static void CreateFilePriv(string strFile, string strSourceDir)
		{
			FileStream fsOut = new FileStream(strFile, FileMode.Create,
				FileAccess.Write, FileShare.None);
			BinaryWriter bwOut = new BinaryWriter(fsOut);
			try
			{
				bwOut.Write(g_uSig);
				bwOut.Write(g_uVer);

				string[] vFiles = Directory.GetFiles(strSourceDir, "*.*",
					SearchOption.AllDirectories);
				foreach(string str in vFiles)
				{
					if(string.IsNullOrEmpty(str)) { Debug.Assert(false); continue; }
					if(str.EndsWith("\"")) { Debug.Assert(false); continue; }
					if(str.EndsWith(".")) { Debug.Assert(false); continue; }

					byte[] pbData = File.ReadAllBytes(str);
					if(pbData.LongLength > int.MaxValue)
						throw new OutOfMemoryException();
					int cbData = pbData.Length;

					string strName = UrlUtil.GetFileName(str);
					byte[] pbName = StrUtil.Utf8.GetBytes(strName);
					if(pbName.LongLength > int.MaxValue)
						throw new OutOfMemoryException();
					int cbName = pbName.Length;

					bwOut.Write(cbName);
					bwOut.Write(pbName);

					bwOut.Write(cbData);
					bwOut.Write(pbData);
				}

				const int iTerm = 0;
				bwOut.Write(iTerm);
			}
			finally { bwOut.Close(); fsOut.Close(); }
		}

		public void Load(byte[] pbFile)
		{
			if(pbFile == null) throw new ArgumentNullException("pbFile");

			MemoryStream ms = new MemoryStream(pbFile, false);
			BinaryReader br = new BinaryReader(ms);
			try
			{
				ulong uSig = br.ReadUInt64();
				if(uSig != g_uSig) throw new FormatException();

				ushort uVer = br.ReadUInt16();
				if(uVer > g_uVer) throw new FormatException();

				while(true)
				{
					int cbName = br.ReadInt32();
					if(cbName == 0) break;

					byte[] pbName = br.ReadBytes(cbName);
					string strName = StrUtil.Utf8.GetString(pbName);

					int cbData = br.ReadInt32();
					byte[] pbData = br.ReadBytes(cbData);

					Debug.Assert(!m_dItems.ContainsKey(strName));
					m_dItems[strName] = pbData;
				}
			}
			finally { br.Close(); ms.Close(); }
		}
	}
}
