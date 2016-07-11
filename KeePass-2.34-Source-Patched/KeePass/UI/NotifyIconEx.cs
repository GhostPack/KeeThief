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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using KeePassLib;
using KeePassLib.Native;
using KeePassLib.Utility;

namespace KeePass.UI
{
	/// <summary>
	/// Exception-safe <c>NotifyIcon</c> wrapper class (workaround
	/// for exceptions thrown when running KeePass under Mono on
	/// Mac OS X).
	/// </summary>
	public sealed class NotifyIconEx
	{
		private NotifyIcon m_ntf = null;

		private Icon m_ico = null; // Property value
		private Icon m_icoShell = null; // Private copy

		public NotifyIcon NotifyIcon { get { return m_ntf; } }

		public ContextMenuStrip ContextMenuStrip
		{
			get
			{
				try { if(m_ntf != null) return m_ntf.ContextMenuStrip; }
				catch(Exception) { Debug.Assert(false); }
				return null;
			}
			set
			{
				try { if(m_ntf != null) m_ntf.ContextMenuStrip = value; }
				catch(Exception) { Debug.Assert(false); }
			}
		}

		public bool Visible
		{
			get
			{
				try { if(m_ntf != null) return m_ntf.Visible; }
				catch(Exception) { Debug.Assert(false); }
				return false;
			}
			set
			{
				try { if(m_ntf != null) m_ntf.Visible = value; }
				catch(Exception) { Debug.Assert(false); }
			}
		}

		public Icon Icon
		{
			get { return m_ico; }
			set
			{
				try
				{
					m_ico = value;
					if(m_ntf == null) return;

					Icon icoToDispose = m_icoShell;
					try
					{
						if(m_ico != null)
						{
							Size sz = SystemInformation.SmallIconSize;
							m_icoShell = new Icon(m_ico, sz);

							m_ntf.Icon = m_icoShell;
						}
						else m_ntf.Icon = null;
					}
					catch(Exception)
					{
						Debug.Assert(false);
						m_ntf.Icon = m_ico;
					}

					if(icoToDispose != null) icoToDispose.Dispose();
				}
				catch(Exception) { Debug.Assert(false); }
			}
		}

		public string Text
		{
			get
			{
				try { if(m_ntf != null) return m_ntf.Text; }
				catch(Exception) { Debug.Assert(false); }
				return string.Empty;
			}
			set
			{
				try { if(m_ntf != null) m_ntf.Text = value; }
				catch(Exception) { Debug.Assert(false); }
			}
		}

		public NotifyIconEx(IContainer container)
		{
			try
			{
				bool bNtf = true;
				DesktopType t = NativeLib.GetDesktopType();
				if((t == DesktopType.Unity) || (t == DesktopType.Pantheon))
					bNtf = !MonoWorkarounds.IsRequired(1354);

				if(bNtf) m_ntf = new NotifyIcon(container);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		public void SetHandlers(EventHandler ehClick, EventHandler ehDoubleClick,
			MouseEventHandler ehMouseDown)
		{
			if(m_ntf == null) return;

			try
			{
				if(ehClick != null) m_ntf.Click += ehClick;
				if(ehDoubleClick != null) m_ntf.DoubleClick += ehDoubleClick;
				if(ehMouseDown != null) m_ntf.MouseDown += ehMouseDown;
			}
			catch(Exception) { Debug.Assert(false); }
		}
	}
}
