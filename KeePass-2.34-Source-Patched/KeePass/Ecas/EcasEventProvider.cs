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

using KeePassLib;

namespace KeePass.Ecas
{
	public abstract class EcasEventProvider
	{
		protected List<EcasEventType> m_events = new List<EcasEventType>();

		internal List<EcasEventType> Events
		{
			get { return m_events; }
		}

		public bool IsSupported(PwUuid uuidType)
		{
			if(uuidType == null) throw new ArgumentNullException("uuidType");

			foreach(EcasEventType t in m_events)
			{
				if(t.Type.Equals(uuidType))
					return true;
			}

			return false;
		}

		public EcasEventType Find(string strEventName)
		{
			if(strEventName == null) throw new ArgumentNullException("strEventName");

			foreach(EcasEventType t in m_events)
			{
				if(t.Name == strEventName) return t;
			}

			return null;
		}

		public EcasEventType Find(PwUuid uuid)
		{
			if(uuid == null) throw new ArgumentNullException("uuid");

			foreach(EcasEventType t in m_events)
			{
				if(t.Type.Equals(uuid)) return t;
			}

			return null;
		}

		public bool Compare(EcasEvent e, EcasContext ctx)
		{
			if(e == null) throw new ArgumentNullException("e");
			if(ctx == null) throw new ArgumentNullException("ctx");

			Debug.Assert(e.Type.Equals(ctx.Event.Type));

			foreach(EcasEventType t in m_events)
			{
				if(t.Type.Equals(e.Type))
					return t.CompareMethod(e, ctx);
			}

			throw new NotSupportedException();
		}
	}
}
