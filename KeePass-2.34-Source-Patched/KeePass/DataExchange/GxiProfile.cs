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

using KeePassLib.Utility;

namespace KeePass.DataExchange
{
	public enum GxiBinaryEncoding
	{
		Base64 = 0,
		Hex,

		Count // Virtual
	}

	public sealed class GxiProfile
	{
		private StrEncodingType m_tEnc = StrEncodingType.Unknown;
		public StrEncodingType Encoding
		{
			get { return m_tEnc; }
			set { m_tEnc = value; }
		}

		private bool m_bRemoveInvChars = true;
		[DefaultValue(true)]
		public bool RemoveInvalidChars
		{
			get { return m_bRemoveInvChars; }
			set { m_bRemoveInvChars = value; }
		}

		private bool m_bDecodeHtmlEnt = true;
		[DefaultValue(true)]
		public bool DecodeHtmlEntities
		{
			get { return m_bDecodeHtmlEnt; }
			set { m_bDecodeHtmlEnt = value; }
		}

		private string m_strRootXPath = string.Empty;
		[DefaultValue("")]
		public string RootXPath
		{
			get { return m_strRootXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strRootXPath = value;
			}
		}

		private string m_strGroupXPath = string.Empty;
		[DefaultValue("")]
		public string GroupXPath
		{
			get { return m_strGroupXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strGroupXPath = value;
			}
		}

		private string m_strGroupNameXPath = string.Empty;
		[DefaultValue("")]
		public string GroupNameXPath
		{
			get { return m_strGroupNameXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strGroupNameXPath = value;
			}
		}

		private string m_strEntryXPath = string.Empty;
		[DefaultValue("")]
		public string EntryXPath
		{
			get { return m_strEntryXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strEntryXPath = value;
			}
		}

		private string m_strStringKvpXPath = string.Empty;
		[DefaultValue("")]
		public string StringKvpXPath
		{
			get { return m_strStringKvpXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringKvpXPath = value;
			}
		}

		private string m_strStringKeyXPath = string.Empty;
		[DefaultValue("")]
		public string StringKeyXPath
		{
			get { return m_strStringKeyXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringKeyXPath = value;
			}
		}

		private bool m_bStringKeyUseName = false;
		[DefaultValue(false)]
		public bool StringKeyUseName
		{
			get { return m_bStringKeyUseName; }
			set { m_bStringKeyUseName = value; }
		}

		private string m_strStringKeyRepl = string.Empty;
		[DefaultValue("")]
		public string StringKeyRepl
		{
			get { return m_strStringKeyRepl; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringKeyRepl = value;
			}
		}

		private string m_strStringValueXPath = string.Empty;
		[DefaultValue("")]
		public string StringValueXPath
		{
			get { return m_strStringValueXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringValueXPath = value;
			}
		}

		private string m_strStringValueRepl = string.Empty;
		[DefaultValue("")]
		public string StringValueRepl
		{
			get { return m_strStringValueRepl; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringValueRepl = value;
			}
		}

		private string m_strStringKvpXPath2 = string.Empty;
		[DefaultValue("")]
		public string StringKvpXPath2
		{
			get { return m_strStringKvpXPath2; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringKvpXPath2 = value;
			}
		}

		private string m_strStringKeyXPath2 = string.Empty;
		[DefaultValue("")]
		public string StringKeyXPath2
		{
			get { return m_strStringKeyXPath2; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringKeyXPath2 = value;
			}
		}

		private bool m_bStringKeyUseName2 = false;
		[DefaultValue(false)]
		public bool StringKeyUseName2
		{
			get { return m_bStringKeyUseName2; }
			set { m_bStringKeyUseName2 = value; }
		}

		private string m_strStringKeyRepl2 = string.Empty;
		[DefaultValue("")]
		public string StringKeyRepl2
		{
			get { return m_strStringKeyRepl2; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringKeyRepl2 = value;
			}
		}

		private string m_strStringValueXPath2 = string.Empty;
		[DefaultValue("")]
		public string StringValueXPath2
		{
			get { return m_strStringValueXPath2; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringValueXPath2 = value;
			}
		}

		private string m_strStringValueRepl2 = string.Empty;
		[DefaultValue("")]
		public string StringValueRepl2
		{
			get { return m_strStringValueRepl2; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strStringValueRepl2 = value;
			}
		}

		private string m_strBinaryKvpXPath = string.Empty;
		[DefaultValue("")]
		public string BinaryKvpXPath
		{
			get { return m_strBinaryKvpXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strBinaryKvpXPath = value;
			}
		}

		private string m_strBinaryKeyXPath = string.Empty;
		[DefaultValue("")]
		public string BinaryKeyXPath
		{
			get { return m_strBinaryKeyXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strBinaryKeyXPath = value;
			}
		}

		private bool m_bBinaryKeyUseName = false;
		[DefaultValue(false)]
		public bool BinaryKeyUseName
		{
			get { return m_bBinaryKeyUseName; }
			set { m_bBinaryKeyUseName = value; }
		}

		private string m_strBinaryKeyRepl = string.Empty;
		[DefaultValue("")]
		public string BinaryKeyRepl
		{
			get { return m_strBinaryKeyRepl; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strBinaryKeyRepl = value;
			}
		}

		private string m_strBinaryValueXPath = string.Empty;
		[DefaultValue("")]
		public string BinaryValueXPath
		{
			get { return m_strBinaryValueXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strBinaryValueXPath = value;
			}
		}

		private GxiBinaryEncoding m_beBinValue = GxiBinaryEncoding.Base64;
		public GxiBinaryEncoding BinaryValueEncoding
		{
			get { return m_beBinValue; }
			set { m_beBinValue = value; }
		}

		private string m_strEntryGroupXPath = string.Empty;
		[DefaultValue("")]
		public string EntryGroupXPath
		{
			get { return m_strEntryGroupXPath; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strEntryGroupXPath = value;
			}
		}

		private string m_strEntryGroupXPath2 = string.Empty;
		[DefaultValue("")]
		public string EntryGroupXPath2
		{
			get { return m_strEntryGroupXPath2; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strEntryGroupXPath2 = value;
			}
		}

		private string m_strEntryGroupSep = string.Empty;
		[DefaultValue("")]
		public string EntryGroupSep
		{
			get { return m_strEntryGroupSep; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strEntryGroupSep = value;
			}
		}

		private bool m_bEntriesInRoot = true;
		[DefaultValue(true)]
		public bool EntriesInRoot
		{
			get { return m_bEntriesInRoot; }
			set { m_bEntriesInRoot = value; }
		}

		private bool m_bEntriesInGroup = true;
		[DefaultValue(true)]
		public bool EntriesInGroup
		{
			get { return m_bEntriesInGroup; }
			set { m_bEntriesInGroup = value; }
		}

		private bool m_bGroupsInGroup = true;
		[DefaultValue(true)]
		public bool GroupsInGroup
		{
			get { return m_bGroupsInGroup; }
			set { m_bGroupsInGroup = value; }
		}

		private bool m_bStringKeyToStd = true;
		[DefaultValue(true)]
		public bool StringKeyToStd
		{
			get { return m_bStringKeyToStd; }
			set { m_bStringKeyToStd = value; }
		}

		private bool m_bStringKeyToStdFuzzy = false;
		[DefaultValue(false)]
		public bool StringKeyToStdFuzzy
		{
			get { return m_bStringKeyToStdFuzzy; }
			set { m_bStringKeyToStdFuzzy = value; }
		}
	}
}
