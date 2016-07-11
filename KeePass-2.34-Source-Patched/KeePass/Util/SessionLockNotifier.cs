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

using Microsoft.Win32;

namespace KeePass.Util
{
	public enum SessionLockReason
	{
		Unknown = 0,
		Ending = 1,
		Lock = 2,
		Suspend = 3,
		RemoteControlChange = 4,
		UserSwitch = 5
	}

	public sealed class SessionLockEventArgs : EventArgs
	{
		private SessionLockReason m_r;

		public SessionLockReason Reason { get { return m_r; } }

		public SessionLockEventArgs(SessionLockReason r)
		{
			m_r = r;
		}
	}

	public sealed class SessionLockNotifier
	{
		private bool m_bEventsRegistered = false;
		private EventHandler<SessionLockEventArgs> m_evHandler = null;

		public SessionLockNotifier()
		{
		}

#if DEBUG
		~SessionLockNotifier()
		{
			Debug.Assert(m_bEventsRegistered == false);
		}
#endif

		public void Install(EventHandler<SessionLockEventArgs> ev)
		{
			this.Uninstall();

			try
			{
				SystemEvents.SessionEnding += this.OnSessionEnding;
				SystemEvents.SessionSwitch += this.OnSessionSwitch;
				SystemEvents.PowerModeChanged += this.OnPowerModeChanged;
			}
			catch(Exception) { Debug.Assert(WinUtil.IsWindows2000); } // 2000 always throws

			m_evHandler = ev;
			m_bEventsRegistered = true;
		}

		public void Uninstall()
		{
			if(m_bEventsRegistered)
			{
				// Unregister event handlers (in the same order as registering,
				// in case one of them throws)
				try
				{
					SystemEvents.SessionEnding -= this.OnSessionEnding;
					SystemEvents.SessionSwitch -= this.OnSessionSwitch;
					SystemEvents.PowerModeChanged -= this.OnPowerModeChanged;
				}
				catch(Exception) { Debug.Assert(WinUtil.IsWindows2000); } // 2000 always throws

				m_evHandler = null;
				m_bEventsRegistered = false;
			}
		}

		private void OnSessionEnding(object sender, SessionEndingEventArgs e)
		{
			if(m_evHandler != null)
				m_evHandler(sender, new SessionLockEventArgs(SessionLockReason.Ending));
		}

		private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if(m_evHandler != null)
			{
				SessionLockReason r = SessionLockReason.Unknown;
				if(e.Reason == SessionSwitchReason.SessionLock)
					r = SessionLockReason.Lock;
				else if(e.Reason == SessionSwitchReason.SessionLogoff)
					r = SessionLockReason.Ending;
				else if(e.Reason == SessionSwitchReason.ConsoleDisconnect)
					r = SessionLockReason.UserSwitch;
				else if((e.Reason == SessionSwitchReason.SessionRemoteControl) ||
					(e.Reason == SessionSwitchReason.RemoteConnect) ||
					(e.Reason == SessionSwitchReason.RemoteDisconnect))
					r = SessionLockReason.RemoteControlChange;

				if(r != SessionLockReason.Unknown)
					m_evHandler(sender, new SessionLockEventArgs(r));
			}
		}

		private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			if((m_evHandler != null) && (e.Mode == PowerModes.Suspend))
				m_evHandler(sender, new SessionLockEventArgs(SessionLockReason.Suspend));
		}
	}
}
