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
using System.Xml.Serialization;
using System.ComponentModel;
using System.Diagnostics;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;

namespace KeePass.Ecas
{
	[DebuggerDisplay("Name = {m_strName}")]
	public sealed class EcasTrigger : IDeepCloneable<EcasTrigger>
	{
		private PwUuid m_uuid = PwUuid.Zero;
		[XmlIgnore]
		public PwUuid Uuid
		{
			get { return m_uuid; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");

				m_uuid = value;
			}
		}

		[XmlElement("Guid")]
		public string UuidString
		{
			get { return Convert.ToBase64String(m_uuid.UuidBytes, Base64FormattingOptions.None); }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_uuid = new PwUuid(Convert.FromBase64String(value));
			}
		}

		private string m_strName = string.Empty;
		[DefaultValue("")]
		public string Name
		{
			get { return m_strName; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strName = value;
			}
		}

		private string m_strComments = string.Empty;
		[DefaultValue("")]
		public string Comments
		{
			get { return m_strComments; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strComments = value;
			}
		}

		private bool m_bEnabled = true;
		[DefaultValue(true)]
		public bool Enabled
		{
			get { return m_bEnabled; }
			set { m_bEnabled = value; }
		}

		private bool m_bInitiallyOn = true;
		[DefaultValue(true)]
		public bool InitiallyOn
		{
			get { return m_bInitiallyOn; }
			set { m_bInitiallyOn = value; }
		}

		private bool m_bOn = true;
		[XmlIgnore]
		public bool On
		{
			get { return m_bOn; }
			set { m_bOn = value; }
		}

		private bool m_bTurnOffAfterAction = false;
		[DefaultValue(false)]
		public bool TurnOffAfterAction
		{
			get { return m_bTurnOffAfterAction; }
			set { m_bTurnOffAfterAction = value; }
		}

		private PwObjectList<EcasEvent> m_events = new PwObjectList<EcasEvent>();
		[XmlIgnore]
		public PwObjectList<EcasEvent> EventCollection
		{
			get { return m_events; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_events = value;
			}
		}

		[XmlArray("Events")]
		[XmlArrayItem("Event")]
		public EcasEvent[] EventArrayForSerialization
		{
			get { return m_events.CloneShallowToList().ToArray(); }
			set { m_events = PwObjectList<EcasEvent>.FromArray(value); }
		}

		private PwObjectList<EcasCondition> m_conds = new PwObjectList<EcasCondition>();
		[XmlIgnore]
		public PwObjectList<EcasCondition> ConditionCollection
		{
			get { return m_conds; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_conds = value;
			}
		}

		[XmlArray("Conditions")]
		[XmlArrayItem("Condition")]
		public EcasCondition[] ConditionsArrayForSerialization
		{
			get { return m_conds.CloneShallowToList().ToArray(); }
			set { m_conds = PwObjectList<EcasCondition>.FromArray(value); }
		}

		private PwObjectList<EcasAction> m_acts = new PwObjectList<EcasAction>();
		[XmlIgnore]
		public PwObjectList<EcasAction> ActionCollection
		{
			get { return m_acts; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_acts = value;
			}
		}

		[XmlArray("Actions")]
		[XmlArrayItem("Action")]
		public EcasAction[] ActionArrayForSerialization
		{
			get { return m_acts.CloneShallowToList().ToArray(); }
			set { m_acts = PwObjectList<EcasAction>.FromArray(value); }
		}

		public EcasTrigger()
		{
		}

		public EcasTrigger(bool bCreateNewUuid)
		{
			if(bCreateNewUuid) m_uuid = new PwUuid(true);
		}

		public EcasTrigger CloneDeep()
		{
			EcasTrigger e = new EcasTrigger(false);

			e.m_uuid = m_uuid; // PwUuid is immutable
			e.m_strName = m_strName;
			e.m_strComments = m_strComments;
			e.m_bEnabled = m_bEnabled;
			e.m_bInitiallyOn = m_bInitiallyOn;
			e.m_bOn = m_bOn;
			e.m_bTurnOffAfterAction = m_bTurnOffAfterAction;

			for(uint i = 0; i < m_events.UCount; ++i)
				e.m_events.Add(m_events.GetAt(i).CloneDeep());

			for(uint j = 0; j < m_conds.UCount; ++j)
				e.m_conds.Add(m_conds.GetAt(j).CloneDeep());

			for(uint k = 0; k < m_acts.UCount; ++k)
				e.m_acts.Add(m_acts.GetAt(k).CloneDeep());

			return e;
		}

		internal void SetToInitialState()
		{
			m_bOn = m_bInitiallyOn;
		}

		public void RunIfMatching(EcasEvent ctxOccured, EcasPropertyDictionary props)
		{
			if(!m_bEnabled || !m_bOn) return;

			EcasContext ctx = new EcasContext(Program.TriggerSystem, this,
				ctxOccured, props);

			bool bEventMatches = false;
			foreach(EcasEvent e in m_events)
			{
				if(Program.EcasPool.CompareEvents(e, ctx))
				{
					bEventMatches = true;
					break;
				}
			}
			if(!bEventMatches) return;

			foreach(EcasCondition c in m_conds)
			{
				if(Program.EcasPool.EvaluateCondition(c, ctx) == false)
					return;
			}

			for(uint iAction = 0; iAction < m_acts.UCount; ++iAction)
			{
				if(ctx.Cancel) break;

				Program.EcasPool.ExecuteAction(m_acts.GetAt(iAction), ctx);
			}

			if(m_bTurnOffAfterAction) m_bOn = false;
		}
	}
}
