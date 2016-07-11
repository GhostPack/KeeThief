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

namespace KeePass.Util
{
	/// <summary>
	/// Structure for a cancellable event.
	/// The difference between this class and the .NET <c>CancelEventArgs</c>
	/// class is that in <c>CancellableOperationEventArgs</c> once the
	/// <c>Cancel</c> property has been set to <c>true</c>, it remains
	/// <c>true</c>, even when trying to set it to <c>false</c> afterwards;
	/// this allows passing an instance to multiple recipients and cancel
	/// if at least one of them wants to (the others can't reset the vote).
	/// </summary>
	public class CancellableOperationEventArgs : EventArgs
	{
		private bool m_bCancel = false;

		public CancellableOperationEventArgs()
		{
		}

		public bool Cancel
		{
			get { return m_bCancel; }
			set { m_bCancel |= value; }
		}
	}
}
