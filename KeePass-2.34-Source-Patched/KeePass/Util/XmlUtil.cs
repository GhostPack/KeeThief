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
using System.Xml.Serialization;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Util.XmlSerialization;

using KeePassLib.Utility;

namespace KeePass.Util
{
	public static partial class XmlUtil
	{
		public static string SafeInnerText(XmlNode xmlNode)
		{
			Debug.Assert(xmlNode != null); if(xmlNode == null) return string.Empty;

			return (xmlNode.InnerText ?? string.Empty);
		}

		public static string SafeInnerText(XmlNode xmlNode, string strNewLineCode)
		{
			if(string.IsNullOrEmpty(strNewLineCode))
				return SafeInnerText(xmlNode);

			string strInner = SafeInnerText(xmlNode);
			return strInner.Replace(strNewLineCode, "\r\n");
		}

		public static string SafeInnerXml(XmlNode xmlNode)
		{
			Debug.Assert(xmlNode != null); if(xmlNode == null) return string.Empty;

			return (xmlNode.InnerXml ?? string.Empty);
		}

		public static string SafeInnerText(HtmlElement htmlNode)
		{
			Debug.Assert(htmlNode != null); if(htmlNode == null) return string.Empty;

			return (htmlNode.InnerText ?? string.Empty);
		}

		public static string SafeAttribute(HtmlElement htmlNode, string strName)
		{
			if(htmlNode == null) { Debug.Assert(false); return string.Empty; }
			if(string.IsNullOrEmpty(strName)) { Debug.Assert(false); return string.Empty; }

			string strValue = (htmlNode.GetAttribute(strName) ?? string.Empty);

			// http://msdn.microsoft.com/en-us/library/ie/ms536429.aspx
			if((strValue.Length == 0) && strName.Equals("class", StrUtil.CaseIgnoreCmp))
				strValue = (htmlNode.GetAttribute("className") ?? string.Empty);

			return strValue;
		}

		private static Dictionary<string, char> m_dHtmlEntities = null;
		/// <summary>
		/// Decode common HTML entities except the XML ones (&lt;, &gt;, etc.).
		/// </summary>
		/// <param name="strXml">String to decode.</param>
		/// <returns>String without non-standard entities.</returns>
		public static string DecodeHtmlEntities(string strXml)
		{
			if(strXml == null) { Debug.Assert(false); return string.Empty; }

			if(m_dHtmlEntities == null)
			{
				Dictionary<string, char> d = new Dictionary<string, char>();

				d["nbsp"] = '\u00A0'; d["iexcl"] = '\u00A1';
				d["cent"] = '\u00A2'; d["pound"] = '\u00A3';
				d["curren"] = '\u00A4'; d["yen"] = '\u00A5';
				d["brvbar"] = '\u00A6'; d["sect"] = '\u00A7';
				d["uml"] = '\u00A8'; d["copy"] = '\u00A9';
				d["ordf"] = '\u00AA'; d["laquo"] = '\u00AB';
				d["not"] = '\u00AC'; d["shy"] = '\u00AD';
				d["reg"] = '\u00AE'; d["macr"] = '\u00AF';
				d["deg"] = '\u00B0'; d["plusmn"] = '\u00B1';
				d["sup2"] = '\u00B2'; d["sup3"] = '\u00B3';
				d["acute"] = '\u00B4'; d["micro"] = '\u00B5';
				d["para"] = '\u00B6'; d["middot"] = '\u00B7';
				d["cedil"] = '\u00B8'; d["sup1"] = '\u00B9';
				d["ordm"] = '\u00BA'; d["raquo"] = '\u00BB';
				d["frac14"] = '\u00BC'; d["frac12"] = '\u00BD';
				d["frac34"] = '\u00BE'; d["iquest"] = '\u00BF';
				d["Agrave"] = '\u00C0'; d["Aacute"] = '\u00C1';
				d["Acirc"] = '\u00C2'; d["Atilde"] = '\u00C3';
				d["Auml"] = '\u00C4'; d["Aring"] = '\u00C5';
				d["AElig"] = '\u00C6'; d["Ccedil"] = '\u00C7';
				d["Egrave"] = '\u00C8'; d["Eacute"] = '\u00C9';
				d["Ecirc"] = '\u00CA'; d["Euml"] = '\u00CB';
				d["Igrave"] = '\u00CC'; d["Iacute"] = '\u00CD';
				d["Icirc"] = '\u00CE'; d["Iuml"] = '\u00CF';
				d["ETH"] = '\u00D0'; d["Ntilde"] = '\u00D1';
				d["Ograve"] = '\u00D2'; d["Oacute"] = '\u00D3';
				d["Ocirc"] = '\u00D4'; d["Otilde"] = '\u00D5';
				d["Ouml"] = '\u00D6'; d["times"] = '\u00D7';
				d["Oslash"] = '\u00D8'; d["Ugrave"] = '\u00D9';
				d["Uacute"] = '\u00DA'; d["Ucirc"] = '\u00DB';
				d["Uuml"] = '\u00DC'; d["Yacute"] = '\u00DD';
				d["THORN"] = '\u00DE'; d["szlig"] = '\u00DF';
				d["agrave"] = '\u00E0'; d["aacute"] = '\u00E1';
				d["acirc"] = '\u00E2'; d["atilde"] = '\u00E3';
				d["auml"] = '\u00E4'; d["aring"] = '\u00E5';
				d["aelig"] = '\u00E6'; d["ccedil"] = '\u00E7';
				d["egrave"] = '\u00E8'; d["eacute"] = '\u00E9';
				d["ecirc"] = '\u00EA'; d["euml"] = '\u00EB';
				d["igrave"] = '\u00EC'; d["iacute"] = '\u00ED';
				d["icirc"] = '\u00EE'; d["iuml"] = '\u00EF';
				d["eth"] = '\u00F0'; d["ntilde"] = '\u00F1';
				d["ograve"] = '\u00F2'; d["oacute"] = '\u00F3';
				d["ocirc"] = '\u00F4'; d["otilde"] = '\u00F5';
				d["ouml"] = '\u00F6'; d["divide"] = '\u00F7';
				d["oslash"] = '\u00F8'; d["ugrave"] = '\u00F9';
				d["uacute"] = '\u00FA'; d["ucirc"] = '\u00FB';
				d["uuml"] = '\u00FC'; d["yacute"] = '\u00FD';
				d["thorn"] = '\u00FE'; d["yuml"] = '\u00FF';
				Debug.Assert(d.Count == (0xFF - 0xA0 + 1));
				d["circ"] = '\u02C6';
				d["tilde"] = '\u02DC';
				d["euro"] = '\u20AC';

				m_dHtmlEntities = d;

#if DEBUG
				Dictionary<char, bool> dChars = new Dictionary<char, bool>();
				foreach(KeyValuePair<string, char> kvp in d)
				{
					Debug.Assert((kvp.Key.IndexOf('&') < 0) &&
						(kvp.Key.IndexOf(';') < 0));
					dChars[kvp.Value] = true;
				}
				Debug.Assert(dChars.Count == d.Count); // Injective

				Debug.Assert(DecodeHtmlEntities("&euro; A &ecircZ; B &copy;") ==
					"\u20AC A &ecircZ; B \u00A9");
				Debug.Assert(DecodeHtmlEntities(@"&lt;&gt;&amp;&apos;&quot;") ==
					"&lt;&gt;&amp;&apos;&quot;"); // Do not decode XML entities
				Debug.Assert(DecodeHtmlEntities(@"<![CDATA[&euro;]]>&euro;<![CDATA[&euro;]]>") ==
					"<![CDATA[&euro;]]>\u20AC<![CDATA[&euro;]]>");
#endif
			}

			StringBuilder sb = new StringBuilder();
			List<string> lParts = SplitByCData(strXml);

			foreach(string str in lParts)
			{
				if(str.StartsWith(@"<![CDATA["))
				{
					sb.Append(str);
					continue;
				}

				int iOffset = 0;
				while(iOffset < str.Length)
				{
					int pAmp = str.IndexOf('&', iOffset);

					if(pAmp < 0)
					{
						sb.Append(str, iOffset, str.Length - iOffset);
						break;
					}

					if(pAmp > iOffset)
						sb.Append(str, iOffset, pAmp - iOffset);

					int pTerm = str.IndexOf(';', pAmp);
					if(pTerm < 0)
					{
						Debug.Assert(false); // At least one entity not terminated
						sb.Append(str, pAmp, str.Length - pAmp);
						break;
					}

					string strEntity = str.Substring(pAmp + 1, pTerm - pAmp - 1);
					char ch;
					if(m_dHtmlEntities.TryGetValue(strEntity, out ch))
						sb.Append(ch);
					else sb.Append(str, pAmp, pTerm - pAmp + 1);

					iOffset = pTerm + 1;
				}
			}

			return sb.ToString();
		}

		public static List<string> SplitByCData(string str)
		{
			List<string> l = new List<string>();
			if(str == null) { Debug.Assert(false); return l; }

			int iOffset = 0;
			while(iOffset < str.Length)
			{
				int pStart = str.IndexOf(@"<![CDATA[", iOffset);

				if(pStart < 0)
				{
					l.Add(str.Substring(iOffset, str.Length - iOffset));
					break;
				}

				if(pStart > iOffset)
					l.Add(str.Substring(iOffset, pStart - iOffset));

				const string strTerm = @"]]>";
				int pTerm = str.IndexOf(strTerm, pStart);
				if(pTerm < 0)
				{
					Debug.Assert(false); // At least one CDATA not terminated
					l.Add(str.Substring(pStart, str.Length - pStart));
					break;
				}

				l.Add(str.Substring(pStart, pTerm - pStart + strTerm.Length));

				iOffset = pTerm + strTerm.Length;
			}

			Debug.Assert(l.IndexOf(string.Empty) < 0);
			return l;
		}

		public static int GetMultiChildIndex(XmlNodeList xlList, XmlNode xnFind)
		{
			if(xlList == null) throw new ArgumentNullException("xlList");
			if(xnFind == null) throw new ArgumentNullException("xnFind");

			string strChildName = xnFind.Name;
			int nChildIndex = 0;
			for(int i = 0; i < xlList.Count; ++i)
			{
				if(xlList[i] == xnFind) return nChildIndex;
				else if(xlList[i].Name == strChildName) ++nChildIndex;
			}

			return -1;
		}

		public static XmlNode FindMultiChild(XmlNodeList xlList, string strChildName,
			int nMultiChildIndex)
		{
			if(xlList == null) throw new ArgumentNullException("xlList");
			if(strChildName == null) throw new ArgumentNullException("strChildName");

			int nChildIndex = 0;
			for(int i = 0; i < xlList.Count; ++i)
			{
				if(xlList[i].Name == strChildName)
				{
					if(nChildIndex == nMultiChildIndex) return xlList[i];
					else ++nChildIndex;
				}
			}

			return null;
		}

		public static void MergeNodes(XmlDocument xd, XmlNode xn, XmlNode xnOverride)
		{
			if(xd == null) throw new ArgumentNullException("xd");
			if(xn == null) throw new ArgumentNullException("xn");
			if(xnOverride == null) throw new ArgumentNullException("xnOverride");
			Debug.Assert(xn.Name == xnOverride.Name);

			foreach(XmlNode xnOvrChild in xnOverride.ChildNodes)
			{
				if(xnOvrChild.NodeType == XmlNodeType.Comment) continue;
				Debug.Assert(xnOvrChild.NodeType == XmlNodeType.Element);

				int nOvrIndex = GetMultiChildIndex(xnOverride.ChildNodes, xnOvrChild);
				if(nOvrIndex < 0) { Debug.Assert(false); continue; }

				XmlNode xnRep = FindMultiChild(xn.ChildNodes, xnOvrChild.Name, nOvrIndex);
				bool bHasSub = (XmlUtil.SafeInnerXml(xnOvrChild).IndexOf('>') >= 0);

				if(xnRep == null)
				{
					if(xnOvrChild.NodeType != XmlNodeType.Element)
					{
						Debug.Assert(false);
						continue;
					}

					XmlNode xnNew = xd.CreateElement(xnOvrChild.Name);
					xn.AppendChild(xnNew);

					if(!bHasSub) xnNew.InnerText = XmlUtil.SafeInnerText(xnOvrChild);
					else MergeNodes(xd, xnNew, xnOvrChild);
				}
				else if(bHasSub) MergeNodes(xd, xnRep, xnOvrChild);
				else xnRep.InnerText = XmlUtil.SafeInnerText(xnOvrChild);
			}
		}

		private sealed class XuopContainer
		{
			private object m_o;
			public object Object { get { return m_o; } }

			public XuopContainer(object o)
			{
				m_o = o;
			}
		}

		private const string GoxpSep = "/";
		public static string GetObjectXmlPath(object oContainer, object oNeedle)
		{
			if(oContainer == null) { Debug.Assert(false); return null; }

			XuopContainer c = new XuopContainer(oContainer);
			string strXPath = GetObjectXmlPathRec(c, typeof(XuopContainer),
				oNeedle, string.Empty);
			if(string.IsNullOrEmpty(strXPath)) return strXPath;

			if(!strXPath.StartsWith("/Object")) { Debug.Assert(false); return null; }
			strXPath = strXPath.Substring(7);

			Type tRoot = oContainer.GetType();
			string strRoot = XmlSerializerEx.GetXmlName(tRoot);

			return (GoxpSep + strRoot + strXPath);
		}

		private static string GetObjectXmlPathRec(object oContainer, Type tContainer,
			object oNeedle, string strCurPath)
		{
			if(oContainer == null) { Debug.Assert(false); return null; }
			if(oNeedle == null) { Debug.Assert(false); return null; }
			Debug.Assert(oContainer.GetType() == tContainer);

			PropertyInfo[] vProps = tContainer.GetProperties();
			foreach(PropertyInfo pi in vProps)
			{
				if((pi == null) || !pi.CanRead) continue;

				object[] vPropAttribs = pi.GetCustomAttributes(true);
				if(XmlSerializerEx.GetAttribute<XmlIgnoreAttribute>(
					vPropAttribs) != null) continue;

				object oSub = pi.GetValue(oContainer, null);
				if(oSub == null) continue;

				string strPropName = XmlSerializerEx.GetXmlName(pi);
				string strSubPath = strCurPath + GoxpSep + strPropName;

				if(oSub == oNeedle) return strSubPath;

				Type tSub = oSub.GetType();
				string strPrimarySubType;
				string strPropTypeCS = XmlSerializerEx.GetFullTypeNameCS(tSub,
					out strPrimarySubType);
				if(XmlSerializerEx.TypeIsArray(strPropTypeCS) ||
					XmlSerializerEx.TypeIsList(strPropTypeCS))
					continue;
				if(strPropTypeCS.StartsWith("System.")) continue;

				string strSubFound = GetObjectXmlPathRec(oSub, tSub, oNeedle,
					strSubPath);
				if(strSubFound != null) return strSubFound;
			}

			return null;
		}
	}
}
