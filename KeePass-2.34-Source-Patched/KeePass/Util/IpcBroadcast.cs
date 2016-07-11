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
// using System.Runtime.Remoting;
// using System.Runtime.Remoting.Channels;
// using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Cryptography;

using KeePass.Native;

using KeePassLib.Utility;

namespace KeePass.Util
{
	public static partial class IpcBroadcast
	{
		// private static IpcServerChannel m_chServer = null;
		// private static IpcClientChannel m_chClient = null;

		// private const string IpcServerPortName = "KeePassBroadcastPort";
		// private const string IpcObjectName = "KeePassBroadcastSingleton";

		public static void Send(Program.AppMessage msg, int lParam,
			bool bWaitWithTimeout)
		{
			if(!KeePassLib.Native.NativeLib.IsUnix()) // Windows
			{
				if(bWaitWithTimeout)
				{
					IntPtr pResult = new IntPtr(0);
					NativeMethods.SendMessageTimeout((IntPtr)NativeMethods.HWND_BROADCAST,
						Program.ApplicationMessage, (IntPtr)msg,
						(IntPtr)lParam, NativeMethods.SMTO_ABORTIFHUNG, 5000, ref pResult);
				}
				else
					NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST,
						Program.ApplicationMessage, (IntPtr)msg, (IntPtr)lParam);
			}
			else // Unix
			{
				// if(m_chClient == null)
				// {
				//	m_chClient = new IpcClientChannel();
				//	ChannelServices.RegisterChannel(m_chClient, false);
				// }
				// try
				// {
				//	IpcBroadcastSingleton ipc = (Activator.GetObject(typeof(
				//		IpcBroadcastSingleton), "ipc://" + GetPortName() + "/" +
				//		IpcObjectName) as IpcBroadcastSingleton);
				//	if(ipc != null) ipc.Call((int)msg, lParam);
				// }
				// catch(Exception) { } // Server might not exist

				FswSend(msg, lParam);
			}
		}

		// private static string GetPortName()
		// {
		//	return (IpcServerPortName + "-" + GetUserID());
		// }

		private static string m_strUserID = null;
		internal static string GetUserID()
		{
			if(m_strUserID != null) return m_strUserID;

			string strID = (Environment.UserName ?? string.Empty) + @" @ " +
				(Environment.MachineName ?? string.Empty);
			byte[] pbID = StrUtil.Utf8.GetBytes(strID);

			SHA1Managed sha1 = new SHA1Managed();
			byte[] pbHash = sha1.ComputeHash(pbID);

			string strShort = Convert.ToBase64String(pbHash,
				Base64FormattingOptions.None);
			strShort = strShort.Replace(@"+", string.Empty);
			strShort = strShort.Replace(@"/", string.Empty);
			if(strShort.Length > 8) strShort = strShort.Substring(0, 8);

			m_strUserID = strShort;
			return strShort;
		}

		public static void StartServer()
		{
			StopServer();

			if(!KeePassLib.Native.NativeLib.IsUnix()) return; // Windows

			// IDictionary dOpt = new Hashtable();
			// dOpt["portName"] = GetPortName();
			// dOpt["exclusiveAddressUse"] = false;
			// dOpt["secure"] = false;
			// m_chServer = new IpcServerChannel(dOpt, null);
			// ChannelServices.RegisterChannel(m_chServer, false);
			// RemotingConfiguration.RegisterWellKnownServiceType(typeof(
			//	IpcBroadcastSingleton), IpcObjectName,
			//	WellKnownObjectMode.SingleCall);

			FswStartServer();
		}

		public static void StopServer()
		{
			if(!KeePassLib.Native.NativeLib.IsUnix()) return; // Windows

			// if(m_chClient != null)
			// {
			//	ChannelServices.UnregisterChannel(m_chClient);
			//	m_chClient = null;
			// }
			// if(m_chServer != null)
			// {
			//	ChannelServices.UnregisterChannel(m_chServer);
			//	m_chServer = null;
			// }

			FswStopServer();
		}
	}

	// public sealed class IpcBroadcastSingleton : MarshalByRefObject
	// {
	//	public void Call(int msg, int lParam)
	//	{
	//		Program.MainForm.Invoke(new CallPrivDelegate(CallPriv), msg, lParam);
	//	}
	//	public delegate void CallPrivDelegate(int msg, int lParam);
	//	private void CallPriv(int msg, int lParam)
	//	{
	//		Program.MainForm.ProcessAppMessage(new IntPtr(msg), new IntPtr(lParam));
	//	}
	// }
}
