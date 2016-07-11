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

namespace KeePass.Ecas
{
	public sealed class EcasContext
	{
		private EcasTriggerSystem m_coll;
		public EcasTriggerSystem TriggerSystem
		{
			get { return m_coll; }
		}

		private EcasTrigger m_trigger;
		public EcasTrigger Trigger
		{
			get { return m_trigger; }
		}

		private EcasEvent m_eOccured;
		public EcasEvent Event
		{
			get { return m_eOccured; }
		}

		private EcasPropertyDictionary m_props;
		public EcasPropertyDictionary Properties
		{
			get { return m_props; }
		}

		private bool m_bCancel = false;
		public bool Cancel
		{
			get { return m_bCancel; }
			set { m_bCancel = value; }
		}

		public EcasContext(EcasTriggerSystem coll, EcasTrigger trigger,
			EcasEvent e, EcasPropertyDictionary props)
		{
			if(coll == null) throw new ArgumentNullException("coll");
			if(trigger == null) throw new ArgumentNullException("trigger");
			if(e == null) throw new ArgumentNullException("e");
			if(props == null) throw new ArgumentNullException("props");

			m_coll = coll;
			m_trigger = trigger;
			m_eOccured = e;
			m_props = props;
		}
	}
}
