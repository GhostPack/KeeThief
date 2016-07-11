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

using KeePass.Util;

namespace KeePass.Ecas
{
	public sealed class EcasRaisingEventArgs : CancellableOperationEventArgs
	{
		private EcasEvent m_evt;
		public EcasEvent Event
		{
			get { return m_evt; }
		}

		private EcasPropertyDictionary m_props;
		public EcasPropertyDictionary Properties
		{
			get { return m_props; }
		}

		public EcasRaisingEventArgs(EcasEvent evt, EcasPropertyDictionary props)
		{
			m_evt = evt;
			m_props = props;
		}
	}
}
