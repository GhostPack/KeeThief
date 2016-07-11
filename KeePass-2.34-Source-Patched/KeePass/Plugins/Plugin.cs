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
using System.Diagnostics;
using System.Drawing;

namespace KeePass.Plugins
{
	/// <summary>
	/// KeePass plugin base class. All KeePass plugins must derive from
	/// this class.
	/// </summary>
	public abstract class Plugin
	{
		/// <summary>
		/// The <c>Initialize</c> function is called by KeePass when
		/// you should initialize your plugin (create menu items, etc.).
		/// </summary>
		/// <param name="host">Plugin host interface. By using this
		/// interface, you can access the KeePass main window and the
		/// currently opened database.</param>
		/// <returns>You must return <c>true</c> in order to signal
		/// successful initialization. If you return <c>false</c>,
		/// KeePass unloads your plugin (without calling the
		/// <c>Terminate</c> function of your plugin).</returns>
		public virtual bool Initialize(IPluginHost host)
		{
			return (host != null);
		}

		/// <summary>
		/// The <c>Terminate</c> function is called by KeePass when
		/// you should free all resources, close open files/streams,
		/// etc. It is also recommended that you remove all your
		/// plugin menu items from the KeePass menu.
		/// </summary>
		public virtual void Terminate()
		{
		}

		/// <summary>
		/// Get a handle to a 16x16 icon representing the plugin.
		/// This icon is shown in the plugin management window of
		/// KeePass for example.
		/// </summary>
		public virtual Image SmallIcon
		{
			get { return null; }
		}

		/// <summary>
		/// URL of a version information file. See
		/// http://keepass.info/help/v2_dev/plg_index.html#upd
		/// </summary>
		public virtual string UpdateUrl
		{
			get { return null; }
		}
	}
}
