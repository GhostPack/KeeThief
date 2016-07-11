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

using KeePassLib;

namespace KeePass.Ecas
{
	public abstract class EcasConditionProvider
	{
		protected List<EcasConditionType> m_conditions = new List<EcasConditionType>();

		internal List<EcasConditionType> Conditions
		{
			get { return m_conditions; }
		}

		public bool IsSupported(PwUuid uuidType)
		{
			if(uuidType == null) throw new ArgumentNullException("uuidType");

			foreach(EcasConditionType t in m_conditions)
			{
				if(t.Type.Equals(uuidType))
					return true;
			}

			return false;
		}

		public EcasConditionType Find(string strConditionName)
		{
			if(strConditionName == null) throw new ArgumentNullException("strConditionName");

			foreach(EcasConditionType t in m_conditions)
			{
				if(t.Name == strConditionName) return t;
			}

			return null;
		}

		public EcasConditionType Find(PwUuid uuid)
		{
			if(uuid == null) throw new ArgumentNullException("uuid");

			foreach(EcasConditionType t in m_conditions)
			{
				if(t.Type.Equals(uuid)) return t;
			}

			return null;
		}

		public bool Evaluate(EcasCondition c, EcasContext ctx)
		{
			if(c == null) throw new ArgumentNullException("c");

			foreach(EcasConditionType t in m_conditions)
			{
				if(t.Type.Equals(c.Type))
					return t.EvaluateMethod(c, ctx);
			}

			throw new NotSupportedException();
		}
	}
}
