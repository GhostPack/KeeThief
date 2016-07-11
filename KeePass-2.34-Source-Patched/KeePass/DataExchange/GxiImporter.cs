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
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange
{
	public static class GxiImporter
	{
		private sealed class GxiContext // Immutable
		{
			private PwDatabase m_pd;
			public PwDatabase Database { get { return m_pd; } }

			private PwGroup m_pg;
			public PwGroup Group { get { return m_pg; } }

			private PwEntry m_pe;
			public PwEntry Entry { get { return m_pe; } }

			private Dictionary<string, string> m_dStringKeyRepl;
			public Dictionary<string, string> StringKeyRepl { get { return m_dStringKeyRepl; } }
			private Dictionary<string, string> m_dStringValueRepl;
			public Dictionary<string, string> StringValueRepl { get { return m_dStringValueRepl; } }
			private Dictionary<string, string> m_dStringKeyRepl2;
			public Dictionary<string, string> StringKeyRepl2 { get { return m_dStringKeyRepl2; } }
			private Dictionary<string, string> m_dStringValueRepl2;
			public Dictionary<string, string> StringValueRepl2 { get { return m_dStringValueRepl2; } }
			private Dictionary<string, string> m_dBinaryKeyRepl;
			public Dictionary<string, string> BinaryKeyRepl { get { return m_dBinaryKeyRepl; } }

			public GxiContext(GxiProfile p, PwDatabase pd, PwGroup pg, PwEntry pe)
			{
				m_pd = pd;
				m_pg = pg;
				m_pe = pe;

				m_dStringKeyRepl = GxiImporter.ParseRepl(p.StringKeyRepl);
				m_dStringValueRepl = GxiImporter.ParseRepl(p.StringValueRepl);
				m_dStringKeyRepl2 = GxiImporter.ParseRepl(p.StringKeyRepl2);
				m_dStringValueRepl2 = GxiImporter.ParseRepl(p.StringValueRepl2);
				m_dBinaryKeyRepl = GxiImporter.ParseRepl(p.BinaryKeyRepl);
			}

			public GxiContext ModifyWith(PwGroup pg)
			{
				GxiContext c = (GxiContext)MemberwiseClone();
				Debug.Assert(object.ReferenceEquals(c.m_dStringKeyRepl, m_dStringKeyRepl));

				c.m_pg = pg;
				return c;
			}

			public GxiContext ModifyWith(PwEntry pe)
			{
				GxiContext c = (GxiContext)MemberwiseClone();
				Debug.Assert(object.ReferenceEquals(c.m_dStringKeyRepl, m_dStringKeyRepl));

				c.m_pe = pe;
				return c;
			}
		}

		private delegate void ImportObjectDelegate(XPathNavigator xpRoot,
			GxiProfile p, GxiContext c);

		public static void Import(PwGroup pgStorage, Stream s, GxiProfile p,
			PwDatabase pdContext, IStatusLogger sl)
		{
			if(pgStorage == null) throw new ArgumentNullException("pgStorage");
			if(s == null) throw new ArgumentNullException("s");
			if(p == null) throw new ArgumentNullException("p");
			if(pdContext == null) throw new ArgumentNullException("pdContext");
			// sl may be null

			// Import into virtual group first, in order to realize
			// an all-or-nothing import
			PwGroup pgVirt = new PwGroup(true, true);

			try { ImportPriv(pgVirt, s, p, pdContext, sl); }
			finally { s.Close(); }

			foreach(PwGroup pg in pgVirt.Groups)
				pgStorage.AddGroup(pg, true);
			foreach(PwEntry pe in pgVirt.Entries)
				pgStorage.AddEntry(pe, true);
		}

		private static void ImportPriv(PwGroup pgStorage, Stream s, GxiProfile p,
			PwDatabase pdContext, IStatusLogger sl)
		{
			StrEncodingInfo sei = StrUtil.GetEncoding(p.Encoding);
			StreamReader srRaw;
			if((sei != null) && (sei.Encoding != null))
				srRaw = new StreamReader(s, sei.Encoding, true);
			else srRaw = new StreamReader(s, true);
			string strDoc = srRaw.ReadToEnd();
			srRaw.Close();

			strDoc = Preprocess(strDoc, p);

			StringReader srDoc = new StringReader(strDoc);
			XPathDocument xd = new XPathDocument(srDoc);

			GxiContext c = new GxiContext(p, pdContext, pgStorage, null);

			XPathNavigator xpDoc = xd.CreateNavigator();
			ImportObject(xpDoc, p, p.RootXPath, "/*", GxiImporter.ImportRoot, c);

			srDoc.Close();
		}

		private static string Preprocess(string strDoc, GxiProfile p)
		{
			string str = strDoc;
			if(str == null) { Debug.Assert(false); return string.Empty; }

			if(p.RemoveInvalidChars) str = StrUtil.SafeXmlString(str);
			if(p.DecodeHtmlEntities) str = XmlUtil.DecodeHtmlEntities(str);

			return str;
		}

		private static XPathNodeIterator QueryNodes(XPathNavigator xpBase,
			string strXPath, string strAltXPath)
		{
			if(xpBase == null) { Debug.Assert(false); return null; }

			string strX = (string.IsNullOrEmpty(strXPath) ? strAltXPath : strXPath);
			if(string.IsNullOrEmpty(strX)) return null;

			return xpBase.Select(strX);
		}

		private static string QueryValue(XPathNavigator xpBase, string strXPath,
			bool bQueryName)
		{
			if(xpBase == null) { Debug.Assert(false); return null; }
			if(string.IsNullOrEmpty(strXPath)) return null;

			XPathNavigator xp = xpBase.SelectSingleNode(strXPath);
			if(xp == null) return null;

			return (bQueryName ? xp.Name : xp.Value);
		}

		private static string QueryValueSafe(XPathNavigator xpBase, string strXPath)
		{
			return (QueryValue(xpBase, strXPath, false) ?? string.Empty);
		}

		// private static string QueryValueSafe(XPathNavigator xpBase, string strXPath,
		//	bool bQueryName)
		// {
		//	return (QueryValue(xpBase, strXPath, bQueryName) ?? string.Empty);
		// }

		private static void ImportObject(XPathNavigator xpBase, GxiProfile p,
			string strXPath, string strAltXPath, ImportObjectDelegate f,
			GxiContext c)
		{
			if(f == null) { Debug.Assert(false); return; }

			XPathNodeIterator xi = QueryNodes(xpBase, strXPath, strAltXPath);
			if(xi == null) return; // No assert
			foreach(XPathNavigator xp in xi) { f(xp, p, c); }
		}

		private static void ImportRoot(XPathNavigator xpBase, GxiProfile p,
			GxiContext c)
		{
			ImportObject(xpBase, p, p.GroupXPath, null, GxiImporter.ImportGroup, c);

			if(p.EntriesInRoot)
				ImportObject(xpBase, p, p.EntryXPath, null, GxiImporter.ImportEntry, c);
		}

		private static void ImportGroup(XPathNavigator xpBase, GxiProfile p,
			GxiContext c)
		{
			PwGroup pg = new PwGroup(true, true);
			c.Group.AddGroup(pg, true);

			GxiContext cSub = c.ModifyWith(pg);

			pg.Name = QueryValueSafe(xpBase, p.GroupNameXPath);
			if(pg.Name.Length == 0) pg.Name = KPRes.Group;

			if(p.GroupsInGroup)
				ImportObject(xpBase, p, p.GroupXPath, null, GxiImporter.ImportGroup, cSub);
			if(p.EntriesInGroup)
				ImportObject(xpBase, p, p.EntryXPath, null, GxiImporter.ImportEntry, cSub);
		}

		private static void ImportEntry(XPathNavigator xpBase, GxiProfile p,
			GxiContext c)
		{
			PwEntry pe = new PwEntry(true, true);

			PwGroup pg = c.Group; // Not the database root group
			string strGroupPath = QueryValueSafe(xpBase, p.EntryGroupXPath);
			string strGroupPath2 = QueryValueSafe(xpBase, p.EntryGroupXPath2);
			if((strGroupPath.Length > 0) && (strGroupPath2.Length > 0))
			{
				Debug.Assert(p.EntryGroupSep.Length > 0);
				strGroupPath = strGroupPath + p.EntryGroupSep + strGroupPath2;
			}
			if(strGroupPath.Length > 0)
			{
				if(p.EntryGroupSep.Length == 0)
					pg = pg.FindCreateGroup(strGroupPath, true);
				else
					pg = pg.FindCreateSubTree(strGroupPath, new string[1]{
						p.EntryGroupSep }, true);
			}
			pg.AddEntry(pe, true);

			GxiContext cSub = c.ModifyWith(pe);

			ImportObject(xpBase, p, p.StringKvpXPath, null,
				GxiImporter.ImportStringKvp, cSub);
			ImportObject(xpBase, p, p.StringKvpXPath2, null,
				GxiImporter.ImportStringKvp2, cSub);
			ImportObject(xpBase, p, p.BinaryKvpXPath, null,
				GxiImporter.ImportBinaryKvp, cSub);
		}

		private static void ImportStringKvp(XPathNavigator xpBase, GxiProfile p,
			GxiContext c)
		{
			ImportStringKvpEx(xpBase, p, c, true);
		}

		private static void ImportStringKvp2(XPathNavigator xpBase, GxiProfile p,
			GxiContext c)
		{
			ImportStringKvpEx(xpBase, p, c, false);
		}

		private static void ImportStringKvpEx(XPathNavigator xpBase, GxiProfile p,
			GxiContext c, bool bFirst)
		{
			string strKey = QueryValue(xpBase, (bFirst ? p.StringKeyXPath :
				p.StringKeyXPath2), (bFirst ? p.StringKeyUseName :
				p.StringKeyUseName2));
			if(string.IsNullOrEmpty(strKey)) return;

			strKey = ApplyRepl(strKey, (bFirst ? c.StringKeyRepl : c.StringKeyRepl2));
			if(strKey.Length == 0) return;

			if(p.StringKeyToStd)
			{
				string strMapped = ImportUtil.MapNameToStandardField(strKey,
					p.StringKeyToStdFuzzy);
				if(!string.IsNullOrEmpty(strMapped)) strKey = strMapped;
			}

			string strValue = QueryValueSafe(xpBase, (bFirst ? p.StringValueXPath :
				p.StringValueXPath2));
			strValue = ApplyRepl(strValue, (bFirst ? c.StringValueRepl :
				c.StringValueRepl2));

			ImportUtil.AppendToField(c.Entry, strKey, strValue, c.Database);
		}

		private static void ImportBinaryKvp(XPathNavigator xpBase, GxiProfile p,
			GxiContext c)
		{
			string strKey = QueryValue(xpBase, p.BinaryKeyXPath, p.BinaryKeyUseName);
			if(string.IsNullOrEmpty(strKey)) return;

			strKey = ApplyRepl(strKey, c.BinaryKeyRepl);
			if(strKey.Length == 0) return;

			string strValue = QueryValueSafe(xpBase, p.BinaryValueXPath);

			byte[] pbValue = null;
			if(p.BinaryValueEncoding == GxiBinaryEncoding.Base64)
				pbValue = Convert.FromBase64String(strValue);
			else if(p.BinaryValueEncoding == GxiBinaryEncoding.Hex)
				pbValue = MemUtil.HexStringToByteArray(strValue);
			else { Debug.Assert(false); }

			if(pbValue == null) return;

			c.Entry.Binaries.Set(strKey, new ProtectedBinary(false, pbValue));
		}

		internal static Dictionary<string, string> ParseRepl(string str)
		{
			Dictionary<string, string> d = new Dictionary<string, string>();
			if(str == null) { Debug.Assert(false); return d; }

			CharStream cs = new CharStream(str + ",,");
			StringBuilder sb = new StringBuilder();
			string strKey = string.Empty;
			bool bValue = false;

			while(true)
			{
				char ch = cs.ReadChar();
				if(ch == char.MinValue) break;

				if(ch == ',')
				{
					if(!bValue)
					{
						strKey = sb.ToString();
						sb.Remove(0, sb.Length);
					}

					if(strKey.Length > 0) d[strKey] = sb.ToString();

					sb.Remove(0, sb.Length);
					bValue = false;
				}
				else if(ch == '>')
				{
					strKey = sb.ToString();

					sb.Remove(0, sb.Length);
					bValue = true;
				}
				else if(ch == '\\')
				{
					char chSub = cs.ReadChar();

					if(chSub == 'n') sb.Append('\n');
					else if(chSub == 'r') sb.Append('\r');
					else if(chSub == 't') sb.Append('\t');
					else sb.Append(chSub);
				}
				else sb.Append(ch);
			}

			return d;
		}

		private static string ApplyRepl(string str, Dictionary<string, string> dRepl)
		{
			if(str == null) { Debug.Assert(false); return string.Empty; }
			if(dRepl == null) { Debug.Assert(false); return str; }

			string strOut;
			if(dRepl.TryGetValue(str, out strOut)) return strOut;
			return str;
		}
	}
}
