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
	public sealed class EcasParameter
	{
		private static EcasParameter[] m_epvNone = null;
		public static EcasParameter[] EmptyArray
		{
			get
			{
				if(m_epvNone == null) m_epvNone = new EcasParameter[0];
				return m_epvNone;
			}
		}

		private string m_strName;
		public string Name
		{
			get { return m_strName; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strName = value;
			}
		}

		private EcasValueType m_type;
		public EcasValueType Type
		{
			get { return m_type; }
			set { m_type = value; }
		}

		private EcasEnum m_vEnumValues;
		public EcasEnum EnumValues
		{
			get { return m_vEnumValues; }
			set { m_vEnumValues = value; } // May be null
		}

		public EcasParameter(string strName, EcasValueType t, EcasEnum eEnumValues)
		{
			if(strName == null) throw new ArgumentNullException("strName");

			m_strName = strName;
			m_type = t;
			m_vEnumValues = eEnumValues;
		}
	}
}
