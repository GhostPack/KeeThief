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
using System.IO;
using System.Xml.Serialization;

using KeePassLib.Native;

namespace KeePass.Util
{
	public sealed class CommandLineArgs
	{
		private List<string> m_vFileNames = new List<string>();
		private SortedDictionary<string, string> m_vParams =
			new SortedDictionary<string, string>();

		/// <summary>
		/// Get the primary file name.
		/// </summary>
		public string FileName
		{
			get
			{
				if(m_vFileNames.Count < 1) return null;
				return m_vFileNames[0];
			}
		}

		/// <summary>
		/// All file names.
		/// </summary>
		public IEnumerable<string> FileNames
		{
			get { return m_vFileNames; }
		}

		public IEnumerable<KeyValuePair<string, string>> Parameters
		{
			get { return m_vParams; }
		}

		public CommandLineArgs(string[] vArgs)
		{
			if(vArgs == null) return; // No throw

			foreach(string str in vArgs)
			{
				if((str == null) || (str.Length < 1)) continue;

				KeyValuePair<string, string> kvp = GetParameter(str);

				if(kvp.Key.Length == 0) m_vFileNames.Add(kvp.Value);
				else m_vParams[kvp.Key] = kvp.Value;
			}
		}

		/// <summary>
		/// Get the value of a command line parameter. Returns <c>null</c>
		/// if no key-value pair with the specified key exists.
		/// </summary>
		public string this[string strKey]
		{
			get
			{
				if(string.IsNullOrEmpty(strKey))
					return this.FileName;
				else
				{
					string strValue;
					if(m_vParams.TryGetValue(strKey.ToLower(), out strValue))
						return strValue;
				}

				return null;
			}
		}

		internal static KeyValuePair<string, string> GetParameter(string strCompiled)
		{
			string str = strCompiled;

			if(str.StartsWith("--")) str = str.Remove(0, 2);
			else if(str.StartsWith("-")) str = str.Remove(0, 1);
			else if(str.StartsWith("/") && !NativeLib.IsUnix())
				str = str.Remove(0, 1);
			else return new KeyValuePair<string, string>(string.Empty, str);

			int posDbl = str.IndexOf(':');
			int posEq = str.IndexOf('=');

			if((posDbl < 0) && (posEq < 0))
				return new KeyValuePair<string, string>(str.ToLower(), string.Empty);

			int posMin = Math.Min(posDbl, posEq);
			if(posMin < 0) posMin = ((posDbl < 0) ? posEq : posDbl);

			if(posMin <= 0)
				return new KeyValuePair<string, string>(str.ToLower(), string.Empty);

			string strKey = str.Substring(0, posMin).ToLower();
			string strValue = str.Remove(0, posMin + 1);
			return new KeyValuePair<string, string>(strKey, strValue);
		}

		public bool Remove(string strParamName)
		{
			if(strParamName == null) { Debug.Assert(false); return false; }

			string strKey = strParamName.ToLower();
			if(m_vParams.ContainsKey(strKey))
			{
				if(m_vParams.Remove(strKey)) return true;
				else { Debug.Assert(false); }
			}

			return false; // No assert when not found
		}

		public static string SafeSerialize(string[] args)
		{
			if(args == null) throw new ArgumentNullException("args");

			MemoryStream ms = new MemoryStream();

			XmlSerializer xml = new XmlSerializer(typeof(string[]));
			xml.Serialize(ms, args);

			string strSerialized = Convert.ToBase64String(ms.ToArray(),
				Base64FormattingOptions.None);
			ms.Close();
			return strSerialized;
		}

		public static string[] SafeDeserialize(string str)
		{
			if(str == null) throw new ArgumentNullException("str");

			byte[] pb = Convert.FromBase64String(str);
			MemoryStream ms = new MemoryStream(pb, false);
			XmlSerializer xml = new XmlSerializer(typeof(string[]));

			try { return (string[])xml.Deserialize(ms); }
			catch(Exception) { Debug.Assert(false); }
			finally { ms.Close(); }

			return null;
		}

		internal void CopyFrom(CommandLineArgs args)
		{
			if(args == null) throw new ArgumentNullException("args");

			m_vFileNames.Clear();
			foreach(string strFile in args.FileNames)
			{
				m_vFileNames.Add(strFile);
			}

			m_vParams.Clear();
			foreach(KeyValuePair<string, string> kvp in args.Parameters)
			{
				if(!string.IsNullOrEmpty(kvp.Key))
					m_vParams[kvp.Key] = kvp.Value;
			}
		}
	}
}
