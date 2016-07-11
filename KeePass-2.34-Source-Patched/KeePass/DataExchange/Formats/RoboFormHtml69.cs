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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 6.9.82-7.9.13.5+
	internal sealed class RoboFormHtml69 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "RoboForm HTML (Logins/PassCards)"; } }
		public override string DefaultExtension { get { return @"html|htm"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_RoboForm; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Unicode, true);
			string strData = sr.ReadToEnd();
			sr.Close();

			strData = strData.Replace(@"<WBR>", string.Empty);
			strData = strData.Replace(@"&shy;", string.Empty);

			WebBrowser wb = new WebBrowser();
			try
			{
				wb.Visible = false;
				wb.ScriptErrorsSuppressed = true;

				UIUtil.SetWebBrowserDocument(wb, strData);
				ImportPriv(pwStorage, wb.Document.Body);
			}
			finally { wb.Dispose(); }
		}

		private static string ParseTitle(string strTitle, PwDatabase pd,
			out PwGroup pg)
		{
			pg = pd.RootGroup;

			// In 7.9.5.9 '/' is used; in earlier versions '\\'
			char[] vSeps = new char[] { '/', '\\' };

			int iLastSep = strTitle.LastIndexOfAny(vSeps);
			if(iLastSep >= 0)
			{
				string strTree = strTitle.Substring(0, iLastSep);
				pg = pd.RootGroup.FindCreateSubTree(strTree, vSeps, true);

				return strTitle.Substring(iLastSep + 1);
			}

			return strTitle;
		}

		private static string MapKey(string strKey)
		{
			string s = ImportUtil.MapNameToStandardField(strKey, true);
			if(string.IsNullOrEmpty(s)) return strKey;

			if((s == PwDefs.TitleField) || (s == PwDefs.UrlField))
				return strKey;

			return s;			
		}

		private static List<HtmlElement> GetElements(HtmlElement hRoot,
			string strTagName, string strAttribName, string strAttribValue)
		{
			List<HtmlElement> l = new List<HtmlElement>();
			if(hRoot == null) { Debug.Assert(false); return l; }
			if(string.IsNullOrEmpty(strTagName)) { Debug.Assert(false); return l; }

			foreach(HtmlElement hEl in hRoot.GetElementsByTagName(strTagName))
			{
				if(!string.IsNullOrEmpty(strAttribName) && (strAttribValue != null))
				{
					string strValue = XmlUtil.SafeAttribute(hEl, strAttribName);
					if(!strValue.Equals(strAttribValue, StrUtil.CaseIgnoreCmp))
						continue;
				}

				l.Add(hEl);
			}

			return l;
		}

		private static void ImportPriv(PwDatabase pd, HtmlElement hBody)
		{
#if DEBUG
			bool bHasSpanCaptions = (GetElements(hBody, "SPAN", "class",
				"caption").Count > 0);
#endif

			foreach(HtmlElement hTable in hBody.GetElementsByTagName("TABLE"))
			{
				Debug.Assert(XmlUtil.SafeAttribute(hTable, "width") == "100%");
				string strRules = XmlUtil.SafeAttribute(hTable, "rules");
				string strFrame = XmlUtil.SafeAttribute(hTable, "frame");
				if(strRules.Equals("cols", StrUtil.CaseIgnoreCmp) &&
					strFrame.Equals("void", StrUtil.CaseIgnoreCmp))
					continue;

				PwEntry pe = new PwEntry(true, true);
				PwGroup pg = null;
				bool bNotesHeaderFound = false;

				foreach(HtmlElement hTr in hTable.GetElementsByTagName("TR"))
				{
					// 7.9.1.1+
					List<HtmlElement> lCaption = GetElements(hTr, "SPAN",
						"class", "caption");
					if(lCaption.Count == 0)
						lCaption = GetElements(hTr, "DIV", "class", "caption");
					if(lCaption.Count > 0)
					{
						string strTitle = ParseTitle(XmlUtil.SafeInnerText(
							lCaption[0]), pd, out pg);
						ImportUtil.AppendToField(pe, PwDefs.TitleField, strTitle, pd);
						continue; // Data is in next TR
					}

					// 7.9.1.1+
					if(hTr.GetElementsByTagName("TABLE").Count > 0) continue;

					HtmlElementCollection lTd = hTr.GetElementsByTagName("TD");
					if(lTd.Count == 1)
					{
						HtmlElement e = lTd[0];
						string strText = XmlUtil.SafeInnerText(e);
						string strClass = XmlUtil.SafeAttribute(e, "class");

						if(strClass.Equals("caption", StrUtil.CaseIgnoreCmp))
						{
							Debug.Assert(pg == null);
							strText = ParseTitle(strText, pd, out pg);
							ImportUtil.AppendToField(pe, PwDefs.TitleField, strText, pd);
						}
						else if(strClass.Equals("subcaption", StrUtil.CaseIgnoreCmp))
							ImportUtil.AppendToField(pe, PwDefs.UrlField,
								ImportUtil.FixUrl(strText), pd);
						else if(strClass.Equals("field", StrUtil.CaseIgnoreCmp))
						{
							// 7.9.2.5+
							if(strText.EndsWith(":") && !bNotesHeaderFound)
								bNotesHeaderFound = true;
							else
								ImportUtil.AppendToField(pe, PwDefs.NotesField,
									strText.Trim(), pd, MessageService.NewLine, false);
						}
						else { Debug.Assert(false); }
					}
					else if((lTd.Count == 2) || (lTd.Count == 3))
					{
						string strKey = XmlUtil.SafeInnerText(lTd[0]);
						string strValue = XmlUtil.SafeInnerText(lTd[lTd.Count - 1]);
						if(lTd.Count == 3) { Debug.Assert(string.IsNullOrEmpty(lTd[1].InnerText)); }

						if(strKey.EndsWith(":")) // 7.9.1.1+
							strKey = strKey.Substring(0, strKey.Length - 1);

						if(strKey.Length > 0)
							ImportUtil.AppendToField(pe, MapKey(strKey), strValue, pd);
						else { Debug.Assert(false); }
					}
					else { Debug.Assert(false); }
				}

				if(pg != null) pg.AddEntry(pe, true);
#if DEBUG
				else { Debug.Assert(bHasSpanCaptions); }
#endif
			}
		}
	}
}
