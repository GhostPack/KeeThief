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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Diagnostics;

using KeePass.App.Configuration;

using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib.Translation;
using KeePassLib.Utility;

namespace KeePass.Util.XmlSerialization
{
	public sealed partial class XmlSerializerEx : IXmlSerializerEx
	{
		private readonly Type m_t;

		public XmlSerializerEx(Type t)
		{
			m_t = t;
		}

		// AppConfigEx with KeePass.XmlSerializers.dll: 811 ms
		// AppConfigEx with own deserializer: 312 ms
		/* public object Deserialize(Stream s)
		{
			int tStart = Environment.TickCount;
			object o = Deserialize_(s);
			MessageService.ShowInfo(Environment.TickCount - tStart);
			return o;
		} */

		public object Deserialize(Stream s)
		{
			object oResult = null;
			if((m_t == typeof(AppConfigEx)) || (m_t == typeof(KPTranslation)))
			{
				XmlReaderSettings xrs = KdbxFile.CreateStdXmlReaderSettings();
				XmlReader xr = XmlReader.Create(s, xrs);

				string strRootName = GetXmlName(m_t);
				bool bRootFound = SkipToRoot(xr, strRootName);

				if(!bRootFound) { Debug.Assert(false); }
				else if(m_t == typeof(AppConfigEx))
					oResult = ReadAppConfigEx(xr);
				else if(m_t == typeof(KPTranslation))
					oResult = ReadKPTranslation(xr);
				else { Debug.Assert(false); } // See top-level 'if'

				xr.Close();
			}
			if(oResult != null) return oResult;

			XmlSerializer xs = new XmlSerializer(m_t);
			return xs.Deserialize(s);
		}

		private static bool SkipToRoot(XmlReader xr, string strRootName)
		{
			xr.Read(); // Initialize reader

			bool bRootFound = false;
			while(true)
			{
				if((xr.NodeType == XmlNodeType.Document) ||
					(xr.NodeType == XmlNodeType.DocumentFragment) ||
					(xr.NodeType == XmlNodeType.Element))
				{
					if(xr.Name == strRootName) { bRootFound = true; break; }
					xr.Skip();
				}
				else if(!xr.Read()) { Debug.Assert(false); break; }
			}

			return bRootFound;
		}

		public void Serialize(XmlWriter xmlWriter, object o)
		{
			XmlSerializer xs = new XmlSerializer(m_t);
			xs.Serialize(xmlWriter, o);
		}

		internal static T GetAttribute<T>(object[] vAttribs)
			where T : Attribute
		{
			if(vAttribs == null) { Debug.Assert(false); return null; }

			foreach(object o in vAttribs)
			{
				if(o == null) { Debug.Assert(false); continue; }
				if(o.GetType() == typeof(T)) return (o as T);
			}

			return null;
		}

		internal static string GetXmlName(MemberInfo mi)
		{
			object[] vAttribs = mi.GetCustomAttributes(true);
			string strXmlName = mi.Name;

			XmlTypeAttribute xmlType = GetAttribute<XmlTypeAttribute>(vAttribs);
			if(xmlType != null) strXmlName = xmlType.TypeName;
			XmlRootAttribute xmlRoot = GetAttribute<XmlRootAttribute>(vAttribs);
			if(xmlRoot != null) strXmlName = xmlRoot.ElementName;
			XmlArrayAttribute xmlArray = GetAttribute<XmlArrayAttribute>(vAttribs);
			if(xmlArray != null) strXmlName = xmlArray.ElementName;
			XmlElementAttribute xmlElement = GetAttribute<XmlElementAttribute>(vAttribs);
			if(xmlElement != null) strXmlName = xmlElement.ElementName;

			return strXmlName;
		}

		private sealed class XmlsTypeInfo
		{
			public bool HasInfo { get { return (m_strReadCode.Length > 0); } }

			private readonly Type m_t;
			public Type Type { get { return m_t; } }

			private readonly string m_strReadCode;
			public string ReadCode { get { return m_strReadCode; } }

			// private readonly string m_strWriteCode;
			// public string WriteCode { get { return m_strWriteCode; } }

			public XmlsTypeInfo(Type t)
			{
				m_t = t;
				m_strReadCode = string.Empty;
				// m_strWriteCode = string.Empty;
			}

			public XmlsTypeInfo(Type t, string strReadCode, string strWriteCode)
			{
				m_t = t;
				m_strReadCode = (strReadCode ?? string.Empty);
				// m_strWriteCode = (strWriteCode ?? string.Empty);
			}
		}

		internal static void GenerateSerializers(CommandLineArgs cl)
		{
			StringBuilder sb = new StringBuilder();
			int t = 0;

			AppendLine(sb, "// This is a generated file!", ref t);
			AppendLine(sb, "// Do not edit manually, changes will be overwritten.", ref t);
			AppendLine(sb);
			AppendLine(sb, "using System;", ref t);
			AppendLine(sb, "using System.Collections.Generic;", ref t);
			AppendLine(sb, "using System.Xml;", ref t);
			AppendLine(sb, "using System.Diagnostics;", ref t);
			AppendLine(sb);
			AppendLine(sb, "using KeePassLib.Interfaces;", ref t);
			AppendLine(sb);
			AppendLine(sb, "namespace KeePass.Util.XmlSerialization", ref t);
			AppendLine(sb, "{", ref t, 0, 1);
			AppendLine(sb, "public sealed partial class XmlSerializerEx : IXmlSerializerEx", ref t);
			AppendLine(sb, "{", ref t, 0, 1);
			AppendLine(sb, "private static char[] m_vEnumSeps = new char[] {", ref t, 0, 1);
			AppendLine(sb, "' ', '\\t', '\\r', '\\n', '|', ',', ';', ':'", ref t);
			AppendLine(sb, "};", ref t, -1, 0);

			Dictionary<string, XmlsTypeInfo> d =
				new Dictionary<string, XmlsTypeInfo>();
			d[typeof(AppConfigEx).FullName] = new XmlsTypeInfo(typeof(AppConfigEx));
			d[typeof(KPTranslation).FullName] = new XmlsTypeInfo(typeof(KPTranslation));

			bool bTypeCreated = true;
			while(bTypeCreated)
			{
				bTypeCreated = false;
				foreach(KeyValuePair<string, XmlsTypeInfo> kvp in d)
				{
					if(!kvp.Value.HasInfo)
					{
						d[kvp.Key] = GenerateSerializer(kvp.Value.Type, d, t);
						bTypeCreated = true;
						break; // Iterator might be invalid
					}
				}
			}

			foreach(KeyValuePair<string, XmlsTypeInfo> kvp in d)
			{
				AppendLine(sb);
				sb.Append(kvp.Value.ReadCode);
			}

			AppendLine(sb, "}", ref t, -1, 0);
			AppendLine(sb, "}", ref t, -1, 0);
			Debug.Assert(t == 0);

			string strFileData = StrUtil.NormalizeNewLines(sb.ToString(), true);

			string strFile = cl["out"];
			if(!string.IsNullOrEmpty(strFile))
			{
				strFile = UrlUtil.MakeAbsolutePath(WinUtil.GetExecutable(), strFile);
				File.WriteAllText(strFile, strFileData, StrUtil.Utf8);
				MessageService.ShowInfo("Saved XmlSerializerEx to:", strFile);
			}
		}

		private static bool IsXmlConvertibleType(Type t)
		{
			return ((t == typeof(Boolean)) || (t == typeof(Byte)) ||
				(t == typeof(Char)) || (t == typeof(DateTime)) ||
				(t == typeof(DateTimeOffset)) || (t == typeof(Decimal)) ||
				(t == typeof(Double)) || (t == typeof(Guid)) ||
				(t == typeof(Int16)) || (t == typeof(Int32)) ||
				(t == typeof(Int64)) || (t == typeof(SByte)) ||
				(t == typeof(Single)) || (t == typeof(TimeSpan)) ||
				(t == typeof(UInt16)) || (t == typeof(UInt32)) ||
				(t == typeof(UInt64)));
		}

		private static void Append(StringBuilder sb, string strAppend,
			ref int iIndent, int iIndentChangePre, int iIndentChangePost)
		{
			iIndent += iIndentChangePre;
			Debug.Assert(iIndent >= 0);
			sb.Append(new string('\t', Math.Max(iIndent, 0)));
			sb.Append(strAppend);
			iIndent += iIndentChangePost;
		}

		private static void AppendLine(StringBuilder sb)
		{
			sb.AppendLine();
		}

		private static void AppendLine(StringBuilder sb, string strAppend,
			ref int iIndent)
		{
			AppendLine(sb, strAppend, ref iIndent, 0, 0);
		}

		private static void AppendLine(StringBuilder sb, string strAppend,
			ref int iIndent, int iIndentChangePre, int iIndentChangePost)
		{
			Append(sb, strAppend + MessageService.NewLine, ref iIndent,
				iIndentChangePre, iIndentChangePost);
		}

		internal static string GetFullTypeNameCS(Type t, out string strPrimarySubType)
		{
			string str = t.FullName;

			if(str.StartsWith(@"System.Collections.Generic.List`1"))
			{
				int iElemTypeOffset = str.IndexOf("[[");
				int iElemTypeEnd = str.IndexOfAny(new char[] {
					',', ' ', '\t', '\r', '\n', ';', ':', ']' }, iElemTypeOffset);
				strPrimarySubType = str.Substring(iElemTypeOffset + 2,
					iElemTypeEnd - iElemTypeOffset - 2);

				str = "System.Collections.Generic.List<" + strPrimarySubType + ">";
			}
			else if(str.EndsWith("[]"))
				strPrimarySubType = str.Substring(0, str.Length - 2);
			else strPrimarySubType = null;

			return str;
		}

		internal static string GetTypeDesc(string strFullTypeNameCS)
		{
			string str = strFullTypeNameCS;

			int iBackOffset = str.IndexOf('<');
			if(iBackOffset < 0) iBackOffset = str.Length - 1;

			int i = str.LastIndexOf('.', iBackOffset);
			if(i >= 0) str = str.Substring(i + 1);

			if(str.StartsWith("List<"))
			{
				string strSubType = str.Substring(5, str.Length - 6);
				return "ListOf" + GetTypeDesc(strSubType);
			}
			if(str.EndsWith("[]"))
			{
				string strSubType = str.Substring(0, str.Length - 2);
				return "ArrayOf" + GetTypeDesc(strSubType);
			}

			Debug.Assert(str.IndexOfAny(new char[] { '<', '>', '[', ']',
				'`', ':', ' ', '\t', '\r', '\n' }) < 0);
			return str;
		}

		internal static bool TypeIsList(string strTypeFullCS)
		{
			return strTypeFullCS.StartsWith("System.Collections.Generic.List<");
		}

		internal static bool TypeIsArray(string strTypeFullCS)
		{
			return strTypeFullCS.EndsWith("[]");
		}

		private static XmlsTypeInfo GenerateSerializer(Type t,
			Dictionary<string, XmlsTypeInfo> dTypes, int iIndent)
		{
			StringBuilder sbr = new StringBuilder();
			StringBuilder sbw = new StringBuilder();

			string strSubTypeFull;
			string strTypeFull = GetFullTypeNameCS(t, out strSubTypeFull);
			string strTypeDesc = GetTypeDesc(strTypeFull);

			string strSubTypeDesc = null;
			if(strSubTypeFull != null)
				strSubTypeDesc = GetTypeDesc(strSubTypeFull);

			bool bIsList = TypeIsList(strTypeFull);
			bool bIsArray = TypeIsArray(strTypeFull);
			int ir = iIndent;

			if(t.IsEnum)
				AppendLine(sbr, "private static Dictionary<string, " + strTypeFull +
					"> m_dict" + strTypeDesc + " = null;", ref ir);

			AppendLine(sbr, "private static " + strTypeFull + " Read" +
				strTypeDesc + "(XmlReader xr" +
				// ((bIsList || bIsArray) ? ", string strItemName" : string.Empty) +
				")", ref ir);
			AppendLine(sbr, "{", ref ir, 0, 1);

			if(t == typeof(string))
			{
				AppendLine(sbr, "return xr.ReadElementString();", ref ir);
			}
			else if(IsXmlConvertibleType(t))
			{
				AppendLine(sbr, "string strValue = xr.ReadElementString();", ref ir);
				AppendLine(sbr, "return XmlConvert.To" + strTypeDesc + "(strValue);", ref ir);
			}
			else if(t.IsEnum)
			{
				AppendLine(sbr, "if(m_dict" + strTypeDesc + " == null)", ref ir);
				AppendLine(sbr, "{", ref ir, 0, 1);
				AppendLine(sbr, "m_dict" + strTypeDesc + " = new Dictionary<string, " +
					strTypeFull + ">();", ref ir);

				string[] vEnumNames = Enum.GetNames(t);
				foreach(string strEnumName in vEnumNames)
				{
					AppendLine(sbr, "m_dict" + strTypeDesc + "[\"" + strEnumName +
						"\"] = " + strTypeFull + "." + strEnumName + ";", ref ir);
				}

				AppendLine(sbr, "}", ref ir, -1, 0);
				AppendLine(sbr);

				AppendLine(sbr, "string strValue = xr.ReadElementString();", ref ir);

				// AppendLine(sbr, "return Enum.Parse(typeof(" + strTypeFull + "), strValue);", ref ir);
				object[] vAttribs = t.GetCustomAttributes(true);
				if(GetAttribute<FlagsAttribute>(vAttribs) != null)
				{
					AppendLine(sbr, strTypeFull + " eResult = (" + strTypeFull + ")0;", ref ir);
					AppendLine(sbr, "string[] vValues = strValue.Split(m_vEnumSeps, StringSplitOptions.RemoveEmptyEntries);", ref ir);
					AppendLine(sbr, "foreach(string strPart in vValues)", ref ir);
					AppendLine(sbr, "{", ref ir, 0, 1);
					AppendLine(sbr, strTypeFull + " ePart;", ref ir);
					AppendLine(sbr, "if(m_dict" + strTypeDesc + ".TryGetValue(strPart, out ePart))", ref ir);
					AppendLine(sbr, "eResult |= ePart;", ref ir, 1, -1);
					AppendLine(sbr, "else { Debug.Assert(false); }", ref ir);
					AppendLine(sbr, "}", ref ir, -1, 0);
				}
				else
				{
					AppendLine(sbr, strTypeFull + " eResult;", ref ir);
					AppendLine(sbr, "if(!m_dict" + strTypeDesc + ".TryGetValue(strValue, out eResult))", ref ir);
					AppendLine(sbr, "{ Debug.Assert(false); }", ref ir, 1, -1);
				}

				AppendLine(sbr, "return eResult;", ref ir);
			}
			else
			{
				if(!bIsArray)
					AppendLine(sbr, strTypeFull + " o = new " + strTypeFull + "();", ref ir);
				else
					AppendLine(sbr, "List<" + strSubTypeFull + "> l = new List<" +
						strSubTypeFull + ">();", ref ir);
				AppendLine(sbr);

				byte[] pbInsAttribs = new byte[16];
				Program.GlobalRandom.NextBytes(pbInsAttribs);
				string strInsAttribs = Convert.ToBase64String(pbInsAttribs,
					Base64FormattingOptions.None);
				sbr.Append(strInsAttribs);
				StringBuilder sbAttribs = new StringBuilder();

				AppendLine(sbr, "if(SkipEmptyElement(xr)) return " +
					(bIsArray ? "l.ToArray();" : "o;"), ref ir);
				AppendLine(sbr);

				AppendLine(sbr, "Debug.Assert(xr.NodeType == XmlNodeType.Element);", ref ir);
				AppendLine(sbr, "xr.ReadStartElement();", ref ir);
				AppendLine(sbr, "xr.MoveToContent();", ref ir);
				AppendLine(sbr);
				AppendLine(sbr, "while(true)", ref ir);
				AppendLine(sbr, "{", ref ir, 0, 1);
				AppendLine(sbr, "XmlNodeType nt = xr.NodeType;", ref ir);
				AppendLine(sbr, "if((nt == XmlNodeType.EndElement) || (nt == XmlNodeType.None)) break;", ref ir);
				AppendLine(sbr, "if(nt != XmlNodeType.Element) { Debug.Assert(false); continue; }", ref ir);
				AppendLine(sbr);

				if(bIsList || bIsArray)
				{
					AppendLine(sbr, strSubTypeFull + " oElem = Read" + strSubTypeDesc +
						"(xr);", ref ir);
					AppendLine(sbr, (bIsArray ? "l" : "o") + ".Add(oElem);", ref ir);

					if(!dTypes.ContainsKey(strSubTypeFull))
						dTypes[strSubTypeFull] = new XmlsTypeInfo(Type.GetType(strSubTypeFull));
				}
				else
				{
					StringBuilder sbs = new StringBuilder();
					uint uElements = 0;

					AppendLine(sbs, "switch(xr.LocalName)", ref ir);
					AppendLine(sbs, "{", ref ir, 0, 1);

					PropertyInfo[] vProps = t.GetProperties();
					foreach(PropertyInfo pi in vProps)
					{
						object[] vAttribs = pi.GetCustomAttributes(true);
						if(GetAttribute<XmlIgnoreAttribute>(vAttribs) != null) continue;

						if(!pi.CanRead || !pi.CanWrite) { Debug.Assert(false); continue; }

						Type tProp = pi.PropertyType;
						string strPropSubTypeFull;
						string strPropTypeFull = GetFullTypeNameCS(tProp, out strPropSubTypeFull);
						string strPropTypeDesc = GetTypeDesc(strPropTypeFull);

						string strXmlName = GetXmlName(pi);

						if(!dTypes.ContainsKey(strPropTypeFull))
							dTypes[strPropTypeFull] = new XmlsTypeInfo(tProp);

						if(GetAttribute<XmlAttributeAttribute>(vAttribs) == null)
						{
							AppendLine(sbs, "case \"" + strXmlName + "\":", ref ir, 0, 1);
							AppendLine(sbs, "o." + pi.Name + " = Read" + strPropTypeDesc +
								"(xr);", ref ir);
							AppendLine(sbs, "break;", ref ir, 0, -1);

							++uElements;
						}
						else
						{
							Debug.Assert(tProp == typeof(string));

							AppendLine(sbAttribs, "case \"" + strXmlName + "\":", ref ir, 0, 1);
							AppendLine(sbAttribs, "o." + pi.Name + " = xr.Value;", ref ir);
							AppendLine(sbAttribs, "break;", ref ir, 0, -1);
						}
					}

					AppendLine(sbs, "default:", ref ir, 0, 1);
					AppendLine(sbs, "Debug.Assert(false);", ref ir);
					AppendLine(sbs, "xr.Skip();", ref ir);
					AppendLine(sbs, "break;", ref ir, 0, -1);
					AppendLine(sbs, "}", ref ir, -1, 0); // switch

					if(uElements > 0) sbr.Append(sbs.ToString());
					else
					{
						AppendLine(sbr, "Debug.Assert(false);", ref ir);
						AppendLine(sbr, "xr.Skip();", ref ir);
					}
				}

				AppendLine(sbr);
				AppendLine(sbr, "xr.MoveToContent();", ref ir);
				AppendLine(sbr, "}", ref ir, -1, 0);
				AppendLine(sbr);
				AppendLine(sbr, "Debug.Assert(xr.NodeType == XmlNodeType.EndElement);", ref ir);
				AppendLine(sbr, "xr.ReadEndElement();", ref ir);

				AppendLine(sbr, (bIsArray ? "return l.ToArray();" : "return o;"), ref ir);

				if(sbAttribs.Length == 0)
					sbr.Replace(strInsAttribs, string.Empty);
				else
				{
					StringBuilder sba = new StringBuilder();
					AppendLine(sba, "while(xr.MoveToNextAttribute())", ref ir);
					AppendLine(sba, "{", ref ir, 0, 1);
					AppendLine(sba, "switch(xr.LocalName)", ref ir);
					AppendLine(sba, "{", ref ir, 0, 1);
					sba.Append(sbAttribs.ToString());
					AppendLine(sba, "default:", ref ir, 0, 1);
					AppendLine(sba, "Debug.Assert(false);", ref ir);
					AppendLine(sba, "break;", ref ir, 0, -1);
					AppendLine(sba, "}", ref ir, -1, 0); // switch
					AppendLine(sba, "}", ref ir, -1, 0); // while
					sba.AppendLine();

					sbr.Replace(strInsAttribs, sba.ToString());
				}
			}

			AppendLine(sbr, "}", ref ir, -1, 0);

			Debug.Assert(ir == iIndent);

			XmlsTypeInfo xti = new XmlsTypeInfo(t, sbr.ToString(), sbw.ToString());
			return xti;
		}

		private static bool SkipEmptyElement(XmlReader xr)
		{
			xr.MoveToElement();
			if(xr.IsEmptyElement)
			{
				xr.Skip();
				return true;
			}

			return false;
		}
	}
}
