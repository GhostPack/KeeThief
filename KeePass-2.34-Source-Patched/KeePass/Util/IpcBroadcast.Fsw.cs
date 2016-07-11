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
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;

using KeePassLib.Cryptography;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public static partial class IpcBroadcast
	{
		private static string m_strMsgFilePath = null;
		private static string m_strMsgFileName = null;
		private static FileSystemWatcher m_fsw = null;
		private static CriticalSectionEx m_csProcess = new CriticalSectionEx();

		private static List<IpcMessage> m_vProcessedMsgs = new List<IpcMessage>();

		private const double IpcMsgValidSecs = 4.0;
		private const int IpcComRetryCount = 30;
		private const int IpcComRetryDelay = 10;
		private const ulong IpcFileSig = 0x038248CB851D7A7CUL;
		private static readonly byte[] IpcOptEnt = { 0xC7, 0x97, 0x39, 0x74 };

		private static void FswEnsurePaths()
		{
			if(m_strMsgFilePath != null) return;

			string strDir = UrlUtil.EnsureTerminatingSeparator(
				UrlUtil.GetTempPath(), false);
			m_strMsgFileName = IpcUtilEx.IpcMsgFilePreID + GetUserID() +
				IpcUtilEx.IpcMsgFilePostID;
			m_strMsgFilePath = strDir + m_strMsgFileName;
		}

		private static void FswSend(Program.AppMessage msg, int lParam)
		{
			FswEnsurePaths();

			IpcMessage ipcMsg = new IpcMessage();
			byte[] pbID = CryptoRandom.Instance.GetRandomBytes(8);
			ipcMsg.ID = MemUtil.BytesToUInt64(pbID);
			ipcMsg.Time = DateTime.UtcNow.ToBinary();
			ipcMsg.Message = msg;
			ipcMsg.LParam = lParam;

			// Send just to others, not to own
			m_vProcessedMsgs.Add(ipcMsg);

			for(int r = 0; r < IpcComRetryCount; ++r)
			{
				try
				{
					List<IpcMessage> l = ReadMessagesPriv();
					CleanOldMessages(l);
					l.Add(ipcMsg);

					MemoryStream ms = new MemoryStream();
					BinaryWriter bw = new BinaryWriter(ms);
					bw.Write(IpcFileSig);
					bw.Write((uint)l.Count);
					for(int j = 0; j < l.Count; ++j)
						IpcMessage.Serialize(bw, l[j]);
					byte[] pbPlain = ms.ToArray();
					bw.Close();
					ms.Close();

					byte[] pbFile = ProtectedData.Protect(pbPlain, IpcOptEnt,
						DataProtectionScope.CurrentUser);

					FileStream fsWrite = new FileStream(m_strMsgFilePath,
						FileMode.Create, FileAccess.Write, FileShare.None);
					fsWrite.Write(pbFile, 0, pbFile.Length);
					fsWrite.Close();

					break;
				}
				catch(Exception) { }

				Thread.Sleep(IpcComRetryDelay);
			}

			CleanOldMessages(m_vProcessedMsgs);
		}

		private static void FswStartServer()
		{
			FswEnsurePaths();

			try
			{
				m_fsw = new FileSystemWatcher(UrlUtil.GetTempPath(),
					m_strMsgFileName);
			}
			catch(Exception) { Debug.Assert(false); return; } // Access denied

			m_fsw.IncludeSubdirectories = false;
			m_fsw.NotifyFilter = (NotifyFilters.CreationTime | NotifyFilters.LastWrite);

			m_fsw.Created += OnCreated;
			m_fsw.Changed += OnChanged;

			m_fsw.EnableRaisingEvents = true;
		}

		private static void FswStopServer()
		{
			if(m_fsw != null)
			{
				m_fsw.EnableRaisingEvents = false;

				m_fsw.Changed -= OnChanged;
				m_fsw.Created -= OnCreated;

				m_fsw.Dispose();
				m_fsw = null;
			}
		}

		private static void OnCreated(object sender, FileSystemEventArgs e)
		{
			ProcessIpcMessage(e);
		}

		private static void OnChanged(object sender, FileSystemEventArgs e)
		{
			ProcessIpcMessage(e);
		}

		private static void ProcessIpcMessage(FileSystemEventArgs e)
		{
			if((e == null) || (e.FullPath == null) || !m_strMsgFilePath.Equals(
				e.FullPath, StrUtil.CaseIgnoreCmp))
			{
				Debug.Assert(false);
				return;
			}

			if(!m_csProcess.TryEnter()) return;

			for(int r = 0; r < IpcComRetryCount; ++r)
			{
				try { ProcessIpcMessagesPriv(); break; }
				catch(Exception) { }

				Thread.Sleep(IpcComRetryDelay);
			}
			CleanOldMessages(m_vProcessedMsgs);

			m_csProcess.Exit();
		}

		private static void ProcessIpcMessagesPriv()
		{
			List<IpcMessage> l = ReadMessagesPriv();
			CleanOldMessages(l);

			foreach(IpcMessage msg in l)
			{
				bool bProcessed = false;
				foreach(IpcMessage ipcMsg in m_vProcessedMsgs)
				{
					if(ipcMsg.ID == msg.ID) { bProcessed = true; break; }
				}
				if(bProcessed) continue;

				m_vProcessedMsgs.Add(msg);

				Program.MainForm.Invoke(new CallPrivDelegate(CallPriv),
					(int)msg.Message, msg.LParam);
			}
		}

		public delegate void CallPrivDelegate(int msg, int lParam);
		private static void CallPriv(int msg, int lParam)
		{
			Program.MainForm.ProcessAppMessage(new IntPtr(msg), new IntPtr(lParam));
		}

		private static List<IpcMessage> ReadMessagesPriv()
		{
			List<IpcMessage> l = new List<IpcMessage>();
			if(!File.Exists(m_strMsgFilePath)) return l;

			byte[] pbEnc = File.ReadAllBytes(m_strMsgFilePath);
			byte[] pb = ProtectedData.Unprotect(pbEnc, IpcOptEnt,
				DataProtectionScope.CurrentUser);

			MemoryStream ms = new MemoryStream(pb, false);
			BinaryReader br = new BinaryReader(ms);
			ulong uSig = br.ReadUInt64();
			if(uSig != IpcFileSig) { Debug.Assert(false); return l; }
			uint uMessages = br.ReadUInt32();

			for(uint u = 0; u < uMessages; ++u)
				l.Add(IpcMessage.Deserialize(br));

			br.Close();
			ms.Close();
			return l;
		}

		private static void CleanOldMessages(List<IpcMessage> l)
		{
			DateTime dtNow = DateTime.UtcNow;
			for(int i = l.Count - 1; i >= 0; --i)
			{
				DateTime dtEvent = DateTime.FromBinary(l[i].Time);

				if((dtNow - dtEvent).TotalSeconds > IpcMsgValidSecs)
					l.RemoveAt(i);
			}
		}

		private sealed class IpcMessage
		{
			public ulong ID;
			public long Time;
			public Program.AppMessage Message;
			public int LParam;

			public static void Serialize(BinaryWriter bw, IpcMessage msg)
			{
				if((bw == null) || (msg == null)) { Debug.Assert(false); return; }

				bw.Write(msg.ID);
				bw.Write(msg.Time);
				bw.Write((int)msg.Message);
				bw.Write(msg.LParam);
			}

			public static IpcMessage Deserialize(BinaryReader br)
			{
				if(br == null) { Debug.Assert(false); return null; }

				IpcMessage msg = new IpcMessage();

				msg.ID = br.ReadUInt64();
				msg.Time = br.ReadInt64();
				msg.Message = (Program.AppMessage)br.ReadInt32();
				msg.LParam = br.ReadInt32();

				return msg;
			}
		}
	}
}
