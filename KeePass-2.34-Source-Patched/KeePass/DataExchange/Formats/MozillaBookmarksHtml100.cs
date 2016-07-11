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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 1.00
	internal sealed class MozillaBookmarksHtml100 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return true; } }

		public override string FormatName { get { return "Mozilla Bookmarks HTML"; } }
		public override string DefaultExtension { get { return @"html|htm"; } }
		public override string ApplicationGroup { get { return KPRes.Browser; } }

		// public override bool ImportAppendsToRootGroupOnly { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_ASCII; }
		}

		// //////////////////////////////////////////////////////////////////
		// Import

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.UTF8);
			string strContent = sr.ReadToEnd();
			sr.Close();

			if(strContent.IndexOf(@"<!DOCTYPE NETSCAPE-Bookmark-file-1>") < 0)
				throw new FormatException("Invalid DOCTYPE!");

			strContent = strContent.Replace(@"<!DOCTYPE NETSCAPE-Bookmark-file-1>", string.Empty);
			strContent = strContent.Replace(@"<HR>", string.Empty);
			strContent = strContent.Replace(@"<p>", string.Empty);
			// strContent = strContent.Replace(@"<DD>", string.Empty);
			// strContent = strContent.Replace(@"<DL>", string.Empty);
			// strContent = strContent.Replace(@"</DL>", string.Empty);
			strContent = strContent.Replace(@"<DT>", string.Empty);

			// int nOffset = strContent.IndexOf('&');
			// while(nOffset >= 0)
			// {
			//	string str4 = strContent.Substring(nOffset, 4);
			//	string str5 = strContent.Substring(nOffset, 5);
			//	string str6 = strContent.Substring(nOffset, 6);
			//	if((str6 != @"&nbsp;") && (str5 != @"&amp;") && (str4 != @"&lt;") &&
			//		(str4 != @"&gt;") && (str5 != @"&#39;") && (str6 != @"&quot;"))
			//	{
			//		strContent = strContent.Remove(nOffset, 1);
			//		strContent = strContent.Insert(nOffset, @"&amp;");
			//	}
			//	else nOffset = strContent.IndexOf('&', nOffset + 1);
			// }

			string[] vPreserve = new string[] { @"&nbsp;", @"&amp;", @"&lt;",
				@"&gt;", @"&#39;", @"&quot;" };
			Dictionary<string, string> dPreserve = new Dictionary<string, string>();
			CryptoRandom cr = CryptoRandom.Instance;
			foreach(string strPreserve in vPreserve)
			{
				string strCode = Convert.ToBase64String(cr.GetRandomBytes(16));
				Debug.Assert(strCode.IndexOf('&') < 0);
				dPreserve[strPreserve] = strCode;

				strContent = strContent.Replace(strPreserve, strCode);
			}
			strContent = strContent.Replace(@"&", @"&amp;");
			foreach(KeyValuePair<string, string> kvpPreserve in dPreserve)
			{
				strContent = strContent.Replace(kvpPreserve.Value, kvpPreserve.Key);
			}

			// Terminate <DD>s
			int iDD = -1;
			while(true)
			{
				iDD = strContent.IndexOf(@"<DD>", iDD + 1);
				if(iDD < 0) break;

				int iNextTag = strContent.IndexOf('<', iDD + 1);
				if(iNextTag <= 0) { Debug.Assert(false); break; }

				strContent = strContent.Insert(iNextTag, @"</DD>");
			}

			strContent = "<RootSentinel>" + strContent + "</META></RootSentinel>";

			byte[] pbFixedData = StrUtil.Utf8.GetBytes(strContent);
			MemoryStream msFixed = new MemoryStream(pbFixedData, false);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(msFixed);
			msFixed.Close();

			XmlNode xmlRoot = xmlDoc.DocumentElement;
			foreach(XmlNode xmlChild in xmlRoot)
			{
				if(xmlChild.Name == "META")
					ImportMeta(xmlChild, pwStorage);
			}
		}

		private static void ImportMeta(XmlNode xmlNode, PwDatabase pwStorage)
		{
			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == "DL")
					ImportGroup(xmlChild, pwStorage, pwStorage.RootGroup);
				else if(xmlChild.Name == "TITLE") { }
				else if(xmlChild.Name == "H1") { }
				else { Debug.Assert(false); }
			}
		}

		private static void ImportGroup(XmlNode xmlNode, PwDatabase pwStorage,
			PwGroup pg)
		{
			PwGroup pgSub = pg;
			PwEntry pe = null;

			foreach(XmlNode xmlChild in xmlNode)
			{
				if(xmlChild.Name == "A")
				{
					pe = new PwEntry(true, true);
					pg.AddEntry(pe, true);

					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pwStorage.MemoryProtection.ProtectTitle,
						XmlUtil.SafeInnerText(xmlChild)));

					XmlNode xnUrl = xmlChild.Attributes.GetNamedItem("HREF");
					if((xnUrl != null) && (xnUrl.Value != null))
						pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
							pwStorage.MemoryProtection.ProtectUrl, xnUrl.Value));
					else { Debug.Assert(false); }

					// pe.Strings.Set("RDF_ID", new ProtectedString(
					//	false, xmlChild.Attributes.GetNamedItem("ID").Value));

					ImportIcon(xmlChild, pe, pwStorage);

					XmlNode xnTags = xmlChild.Attributes.GetNamedItem("TAGS");
					if((xnTags != null) && (xnTags.Value != null))
					{
						string[] vTags = xnTags.Value.Split(',');
						foreach(string strTag in vTags)
						{
							if(string.IsNullOrEmpty(strTag)) continue;
							pe.AddTag(strTag);
						}
					}
				}
				else if(xmlChild.Name == "DD")
				{
					if(pe != null)
						ImportUtil.AppendToField(pe, PwDefs.NotesField,
							XmlUtil.SafeInnerText(xmlChild).Trim(), pwStorage,
							"\r\n", false);
					else { Debug.Assert(false); }
				}
				else if(xmlChild.Name == "H3")
				{
					string strGroup = XmlUtil.SafeInnerText(xmlChild);
					if(strGroup.Length == 0) { Debug.Assert(false); pgSub = pg; }
					else
					{
						pgSub = new PwGroup(true, true, strGroup, PwIcon.Folder);
						pg.AddGroup(pgSub, true);
					}
				}
				else if(xmlChild.Name == "DL")
					ImportGroup(xmlChild, pwStorage, pgSub);
				else { Debug.Assert(false); }
			}
		}

		private static void ImportIcon(XmlNode xn, PwEntry pe, PwDatabase pd)
		{
			XmlNode xnIcon = xn.Attributes.GetNamedItem("ICON");
			if(xnIcon == null) return;

			string strIcon = xnIcon.Value;
			if(!StrUtil.IsDataUri(strIcon)) { Debug.Assert(false); return; }

			try
			{
				byte[] pbImage = StrUtil.DataUriToData(strIcon);
				if((pbImage == null) || (pbImage.Length == 0)) { Debug.Assert(false); return; }

				Image img = GfxUtil.LoadImage(pbImage);
				if(img == null) { Debug.Assert(false); return; }

				byte[] pbPng;
				int wMax = PwCustomIcon.MaxWidth;
				int hMax = PwCustomIcon.MaxHeight;
				if((img.Width <= wMax) && (img.Height <= hMax))
				{
					MemoryStream msPng = new MemoryStream();
					img.Save(msPng, ImageFormat.Png);
					pbPng = msPng.ToArray();
					msPng.Close();
				}
				else
				{
					Image imgSc = GfxUtil.ScaleImage(img, wMax, hMax);

					MemoryStream msPng = new MemoryStream();
					imgSc.Save(msPng, ImageFormat.Png);
					pbPng = msPng.ToArray();
					msPng.Close();

					imgSc.Dispose();
				}
				img.Dispose();

				PwUuid pwUuid = null;
				int iEx = pd.GetCustomIconIndex(pbPng);
				if(iEx >= 0) pwUuid = pd.CustomIcons[iEx].Uuid;
				else
				{
					pwUuid = new PwUuid(true);
					pd.CustomIcons.Add(new PwCustomIcon(pwUuid, pbPng));
					pd.UINeedsIconUpdate = true;
					pd.Modified = true;
				}
				pe.CustomIconUuid = pwUuid;
			}
			catch(Exception) { Debug.Assert(false); }
		}

		// //////////////////////////////////////////////////////////////////
		// Export

		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput,
			IStatusLogger slLogger)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE NETSCAPE-Bookmark-file-1>");
			sb.AppendLine("<!-- This is an automatically generated file.");
			sb.AppendLine("     It will be read and overwritten.");
			sb.AppendLine("     DO NOT EDIT! -->");
			sb.AppendLine("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");
			sb.AppendLine("<TITLE>Bookmarks</TITLE>");
			sb.AppendLine("<H1>Bookmarks</H1>");
			sb.AppendLine();
			sb.AppendLine("<DL><p>");

			ExportGroup(sb, pwExportInfo.DataGroup, 1, pwExportInfo.ContextDatabase);

			sb.AppendLine("</DL>");

			string strData = sb.ToString();
			strData = StrUtil.NormalizeNewLines(strData, false);

			byte[] pbData = StrUtil.Utf8.GetBytes(strData);
			sOutput.Write(pbData, 0, pbData.Length);
			sOutput.Close();
			return true;
		}

		private static void ExportGroup(StringBuilder sb, PwGroup pg, uint uIndent,
			PwDatabase pd)
		{
			ExportData(sb, uIndent, "<DT><H3", null, false, null, false);
			ExportTimes(sb, pg);
			ExportData(sb, 0, ">", pg.Name, true, "</H3>", true);
			ExportData(sb, uIndent, "<DL><p>", null, false, null, true);

			foreach(PwGroup pgSub in pg.Groups)
			{
				ExportGroup(sb, pgSub, uIndent + 1, pd);
			}

#if DEBUG
			List<string> l = new List<string>();
			l.Add("Tag 1");
			l.Add("Tag 2");
			Debug.Assert(StrUtil.TagsToString(l, false) == "Tag 1;Tag 2");
#endif

			foreach(PwEntry pe in pg.Entries)
			{
				string strUrl = pe.Strings.ReadSafe(PwDefs.UrlField);
				if(strUrl.Length == 0) continue;

				// Encode only when really required; '&' does not need
				// to be encoded
				bool bEncUrl = (strUrl.IndexOfAny(new char[] {
					'\"', '<', '>' }) >= 0);

				ExportData(sb, uIndent + 1, "<DT><A HREF=\"", strUrl, bEncUrl,
					"\"", false);

				ExportTimes(sb, pe);

				if(!pe.CustomIconUuid.Equals(PwUuid.Zero) && (pd != null))
				{
					try
					{
						Image imgIcon = pd.GetCustomIcon(pe.CustomIconUuid, 16, 16);
						if(imgIcon != null)
						{
							using(MemoryStream msIcon = new MemoryStream())
							{
								imgIcon.Save(msIcon, ImageFormat.Png);
								byte[] pbIcon = msIcon.ToArray();
								string strIcon = StrUtil.DataToDataUri(pbIcon,
									"image/png");

								ExportData(sb, 0, " ICON=\"", strIcon, false,
									"\"", false);
							}
						}
						else { Debug.Assert(false); }
					}
					catch(Exception) { Debug.Assert(false); }
				}

				if(pe.Tags.Count > 0)
				{
					string strTags = StrUtil.TagsToString(pe.Tags, false);
					strTags = strTags.Replace(';', ','); // Without space

					ExportData(sb, 0, " TAGS=\"", strTags, true, "\"", false);
				}

				string strTitle = pe.Strings.ReadSafe(PwDefs.TitleField);
				if(strTitle.Length == 0) strTitle = strUrl;

				ExportData(sb, 0, ">", strTitle, true, "</A>", true);

				string strNotes = pe.Strings.ReadSafe(PwDefs.NotesField);
				if(strNotes.Length > 0)
					ExportData(sb, uIndent + 1, "<DD>", strNotes, true, null, true);
			}

			ExportData(sb, uIndent, "</DL><p>", null, false, null, true);
		}

		private static void ExportData(StringBuilder sb, uint uIndent,
			string strRawPrefix, string strData, bool bEncodeData,
			string strRawSuffix, bool bNewLine)
		{
			if(uIndent > 0) sb.Append(new string(' ', 4 * (int)uIndent));
			if(strRawPrefix != null) sb.Append(strRawPrefix);

			if(strData != null)
			{
				if(bEncodeData)
				{
					// Apply HTML encodings except '\n' -> "<br />"
					const string strNewLine = "899A13DDD6BA4B24BA2CA6C756E7B936";
					string str = StrUtil.NormalizeNewLines(strData, false);
					str = str.Replace("\n", strNewLine);
					str = StrUtil.StringToHtml(str);
					str = str.Replace(strNewLine, "\n");

					sb.Append(str);
				}
				else sb.Append(strData);
			}

			if(strRawSuffix != null) sb.Append(strRawSuffix);

			if(bNewLine) sb.AppendLine();
		}

		private static void ExportTimes(StringBuilder sb, ITimeLogger tl)
		{
			if(tl == null) { Debug.Assert(false); return; }

			try
			{
				long t = (long)TimeUtil.SerializeUnix(tl.CreationTime);
				ExportData(sb, 0, " ADD_DATE=\"", t.ToString(
					NumberFormatInfo.InvariantInfo), false, "\"", false);

				t = (long)TimeUtil.SerializeUnix(tl.LastModificationTime);
				ExportData(sb, 0, " LAST_MODIFIED=\"", t.ToString(
					NumberFormatInfo.InvariantInfo), false, "\"", false);
			}
			catch(Exception) { Debug.Assert(false); }
		}
	}
}
