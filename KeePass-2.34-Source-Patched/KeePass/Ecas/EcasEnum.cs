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

using KeePass.Resources;

using KeePassLib.Utility;

namespace KeePass.Ecas
{
	public sealed class EcasEnum
	{
		private EcasEnumItem[] m_vItems;
		public EcasEnumItem[] Items
		{
			get { return m_vItems; }
		}

		public int ItemCount
		{
			get { return m_vItems.Length; }
		}

		public EcasEnum(EcasEnumItem[] vItems)
		{
			if(vItems == null) throw new ArgumentNullException("vItems");

			m_vItems = vItems;
		}

		public uint GetItemID(string strName, uint uDefaultIfNotFound)
		{
			if(strName == null) throw new ArgumentNullException("strName");

			foreach(EcasEnumItem e in m_vItems)
			{
				if(e.Name == strName) return e.ID;
			}

			return uDefaultIfNotFound;
		}

		/// <summary>
		/// Get the localized descriptive text of an enumeration item.
		/// </summary>
		/// <param name="uID">ID of the enumeration item.</param>
		/// <param name="strDefaultIfNotFound">Default value, may be <c>null</c>.</param>
		/// <returns>Localized enumeration item text or the default value.</returns>
		public string GetItemString(uint uID, string strDefaultIfNotFound)
		{
			foreach(EcasEnumItem e in m_vItems)
			{
				if(e.ID == uID) return e.Name;
			}

			return strDefaultIfNotFound;
		}
	}

	public sealed class EcasEnumItem
	{
		private uint m_id;
		public uint ID
		{
			get { return m_id; }
		}

		private string m_name;
		public string Name
		{
			get { return m_name; }
		}

		public EcasEnumItem(uint uID, string strName)
		{
			if(strName == null) throw new ArgumentNullException("strName");

			m_id = uID;
			m_name = strName;
		}
	}
}
