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

using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;
using KeePassLib.Utility;

namespace KeePass.Ecas
{
	public sealed class EcasTriggerSystem : IDeepCloneable<EcasTriggerSystem>
	{
		private bool m_bEnabled = true;
		[DefaultValue(true)]
		public bool Enabled
		{
			get { return m_bEnabled; }
			set { m_bEnabled = value; }
		}

		private PwObjectList<EcasTrigger> m_vTriggers = new PwObjectList<EcasTrigger>();
		[XmlIgnore]
		public PwObjectList<EcasTrigger> TriggerCollection
		{
			get { return m_vTriggers; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vTriggers = value;
			}
		}

		[XmlArray("Triggers")]
		[XmlArrayItem("Trigger")]
		public EcasTrigger[] TriggerArrayForSerialization
		{
			get { return m_vTriggers.CloneShallowToList().ToArray(); }
			set { m_vTriggers = PwObjectList<EcasTrigger>.FromArray(value); }
		}

		public event EventHandler<EcasRaisingEventArgs> RaisingEvent;

		public EcasTriggerSystem()
		{
		}

		public EcasTriggerSystem CloneDeep()
		{
			EcasTriggerSystem c = new EcasTriggerSystem();

			c.m_bEnabled = m_bEnabled;

			for(uint i = 0; i < m_vTriggers.UCount; ++i)
				c.m_vTriggers.Add(m_vTriggers.GetAt(i).CloneDeep());

			return c;
		}

		internal void SetToInitialState()
		{
			for(uint i = 0; i < m_vTriggers.UCount; ++i)
				m_vTriggers.GetAt(i).SetToInitialState();
		}

		public object FindObjectByUuid(PwUuid pwUuid)
		{
			if(pwUuid == null) throw new ArgumentNullException("pwUuid");

			foreach(EcasTrigger t in m_vTriggers)
			{
				if(t.Uuid.Equals(pwUuid)) return t;

				foreach(EcasEvent e in t.EventCollection)
				{
					if(e.Type.Equals(pwUuid)) return e;
				}

				foreach(EcasCondition c in t.ConditionCollection)
				{
					if(c.Type.Equals(pwUuid)) return c;
				}

				foreach(EcasAction a in t.ActionCollection)
				{
					if(a.Type.Equals(pwUuid)) return a;
				}
			}

			return null;
		}

		public void RaiseEvent(PwUuid eventType)
		{
			RaiseEvent(eventType, null);
		}

		internal void RaiseEvent(PwUuid eventType, string strPropKey,
			object oPropValue)
		{
			EcasPropertyDictionary d = new EcasPropertyDictionary();
			d.Set(strPropKey, oPropValue);

			RaiseEvent(eventType, d);
		}

		public void RaiseEvent(PwUuid eventType, EcasPropertyDictionary props)
		{
			if(eventType == null) throw new ArgumentNullException("eventType");
			// if(props == null) throw new ArgumentNullException("props");

			if(!m_bEnabled) return;

			EcasEvent e = new EcasEvent();
			e.Type = eventType;

			RaiseEventObj(e, (props ?? new EcasPropertyDictionary()));
		}

		private void RaiseEventObj(EcasEvent e, EcasPropertyDictionary props)
		{
			// if(e == null) throw new ArgumentNullException("e");
			// if(m_bEnabled == false) return;

			if(this.RaisingEvent != null)
			{
				EcasRaisingEventArgs args = new EcasRaisingEventArgs(e, props);
				this.RaisingEvent(this, args);
				if(args.Cancel) return;
			}

			try
			{
				foreach(EcasTrigger t in m_vTriggers)
					t.RunIfMatching(e, props);
			}
			catch(Exception ex)
			{
				if(!VistaTaskDialog.ShowMessageBox(ex.Message, KPRes.TriggerExecutionFailed,
					PwDefs.ShortProductName, VtdIcon.Warning, null))
				{
					MessageService.ShowWarning(KPRes.TriggerExecutionFailed + ".", ex);
				}
			}
		}
	}

	[XmlRoot("TriggerCollection")]
	public sealed class EcasTriggerContainer
	{
		private List<EcasTrigger> m_vTriggers = new List<EcasTrigger>();
		[XmlArrayItem("Trigger")]
		public List<EcasTrigger> Triggers
		{
			get { return m_vTriggers; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vTriggers = value;
			}
		}

		public EcasTriggerContainer() { }
	}
}
