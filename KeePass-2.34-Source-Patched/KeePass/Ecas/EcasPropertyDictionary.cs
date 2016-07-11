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

namespace KeePass.Ecas
{
	public static class EcasProperty
	{
		// Triggering objects
		public const string Database = "Database"; // PwDatabase
		public const string IOConnectionInfo = "IOConnectionInfo"; // IOConnectionInfo
		public const string Text = "Text"; // String
		public const string CommandID = "CommandID"; // String
	}

	public sealed class EcasPropertyDictionary
	{
		private Dictionary<string, object> m_dict = new Dictionary<string, object>();

		public EcasPropertyDictionary()
		{
		}

		public void Set(string strKey, object oValue)
		{
			if(string.IsNullOrEmpty(strKey)) { Debug.Assert(false); return; }

			m_dict[strKey] = oValue;
		}

		public T Get<T>(string strKey)
			where T : class
		{
			if(string.IsNullOrEmpty(strKey)) { Debug.Assert(false); return null; }

			object o;
			if(m_dict.TryGetValue(strKey, out o))
			{
				if(o == null) return null;

				T p = (o as T);
				Debug.Assert(p != null);
				return p;
			}

			return null;
		}
	}
}
