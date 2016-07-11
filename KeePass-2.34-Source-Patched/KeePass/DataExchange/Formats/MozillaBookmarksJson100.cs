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
using System.Diagnostics;
using System.Drawing;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 1.00
	internal sealed class MozillaBookmarksJson100 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Mozilla Bookmarks JSON"; } }
		public override string DefaultExtension { get { return "json"; } }
		public override string ApplicationGroup { get { return KPRes.Browser; } }

		private const string m_strGroup = "children";

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_ASCII; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, StrUtil.Utf8);
			string strContent = sr.ReadToEnd();
			sr.Close();

			if(string.IsNullOrEmpty(strContent)) return;

			CharStream cs = new CharStream(strContent);

			Dictionary<string, List<string>> dTags =
				new Dictionary<string, List<string>>();
			List<PwEntry> lCreatedEntries = new List<PwEntry>();

			JsonObject jRoot = new JsonObject(cs);
			AddObject(pwStorage.RootGroup, jRoot, pwStorage, false, dTags,
				lCreatedEntries);
			Debug.Assert(cs.PeekChar(true) == char.MinValue);

			// Assign tags
			foreach(PwEntry pe in lCreatedEntries)
			{
				string strUri = pe.Strings.ReadSafe(PwDefs.UrlField);
				if(strUri.Length == 0) continue;

				foreach(KeyValuePair<string, List<string>> kvp in dTags)
				{
					foreach(string strTagUri in kvp.Value)
					{
						if(strUri.Equals(strTagUri, StrUtil.CaseIgnoreCmp))
							pe.AddTag(kvp.Key);
					}
				}
			}
		}

		private static void AddObject(PwGroup pgStorage, JsonObject jObject,
			PwDatabase pwContext, bool bCreateSubGroups,
			Dictionary<string, List<string>> dTags, List<PwEntry> lCreatedEntries)
		{
			JsonValue jvRoot;
			jObject.Items.TryGetValue("root", out jvRoot);
			string strRoot = (((jvRoot != null) ? jvRoot.ToString() : null) ?? string.Empty);
			if(strRoot.Equals("tagsFolder", StrUtil.CaseIgnoreCmp))
			{
				ImportTags(jObject, dTags);
				return;
			}

			if(jObject.Items.ContainsKey(m_strGroup))
			{
				JsonArray jArray = (jObject.Items[m_strGroup].Value as JsonArray);
				if(jArray == null) { Debug.Assert(false); return; }

				PwGroup pgNew;
				if(bCreateSubGroups)
				{
					pgNew = new PwGroup(true, true);
					pgStorage.AddGroup(pgNew, true);

					if(jObject.Items.ContainsKey("title"))
						pgNew.Name = ((jObject.Items["title"].Value != null) ?
							jObject.Items["title"].Value.ToString() : string.Empty);
				}
				else pgNew = pgStorage;

				foreach(JsonValue jValue in jArray.Values)
				{
					JsonObject objSub = (jValue.Value as JsonObject);
					if(objSub != null)
						AddObject(pgNew, objSub, pwContext, true, dTags, lCreatedEntries);
					else { Debug.Assert(false); }
				}

				return;
			}

			PwEntry pe = new PwEntry(true, true);

			SetString(pe, "Index", false, jObject, "index");
			SetString(pe, PwDefs.TitleField, pwContext.MemoryProtection.ProtectTitle,
				jObject, "title");
			SetString(pe, "ID", false, jObject, "id");
			SetString(pe, PwDefs.UrlField, pwContext.MemoryProtection.ProtectUrl,
				jObject, "uri");
			SetString(pe, "CharSet", false, jObject, "charset");

			if(jObject.Items.ContainsKey("annos"))
			{
				JsonArray vAnnos = (jObject.Items["annos"].Value as JsonArray);
				if(vAnnos != null)
				{
					foreach(JsonValue jv in vAnnos.Values)
					{
						if(jv == null) { Debug.Assert(false); continue; }
						JsonObject jo = (jv.Value as JsonObject);
						if(jo == null) { Debug.Assert(false); continue; }

						JsonValue jvAnnoName, jvAnnoValue;
						jo.Items.TryGetValue("name", out jvAnnoName);
						jo.Items.TryGetValue("value", out jvAnnoValue);
						if((jvAnnoName == null) || (jvAnnoValue == null)) continue;

						string strAnnoName = jvAnnoName.ToString();
						string strAnnoValue = jvAnnoValue.ToString();
						if((strAnnoName == null) || (strAnnoValue == null)) continue;

						if(strAnnoName == "bookmarkProperties/description")
							pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
								pwContext.MemoryProtection.ProtectNotes, strAnnoValue));
					}
				}
			}

			if((pe.Strings.ReadSafe(PwDefs.TitleField).Length > 0) ||
				(pe.Strings.ReadSafe(PwDefs.UrlField).Length > 0))
			{
				pgStorage.AddEntry(pe, true);
				lCreatedEntries.Add(pe);
			}
		}

		private static void SetString(PwEntry pe, string strEntryKey, bool bProtect,
			JsonObject jObject, string strObjectKey)
		{
			if(!jObject.Items.ContainsKey(strObjectKey)) return;

			object obj = jObject.Items[strObjectKey].Value;
			if(obj == null) return;

			pe.Strings.Set(strEntryKey, new ProtectedString(bProtect, obj.ToString()));
		}

		private static void ImportTags(JsonObject jTagsRoot,
			Dictionary<string, List<string>> dTags)
		{
			try
			{
				JsonValue jTags = jTagsRoot.Items["children"];
				if(jTags == null) { Debug.Assert(false); return; }
				JsonArray arTags = (jTags.Value as JsonArray);
				if(arTags == null) { Debug.Assert(false); return; }

				foreach(JsonValue jvTag in arTags.Values)
				{
					JsonObject jTag = (jvTag.Value as JsonObject);
					if(jTag == null) { Debug.Assert(false); continue; }

					JsonValue jvName = jTag.Items["title"];
					if(jvName == null) { Debug.Assert(false); continue; }
					string strName = jvName.ToString();
					if(string.IsNullOrEmpty(strName)) { Debug.Assert(false); continue; }

					List<string> lUris;
					dTags.TryGetValue(strName, out lUris);
					if(lUris == null)
					{
						lUris = new List<string>();
						dTags[strName] = lUris;
					}

					JsonValue jvUrls = jTag.Items["children"];
					if(jvUrls == null) { Debug.Assert(false); continue; }
					JsonArray arUrls = (jvUrls.Value as JsonArray);
					if(arUrls == null) { Debug.Assert(false); continue; }

					foreach(JsonValue jvPlace in arUrls.Values)
					{
						JsonObject jUrl = (jvPlace.Value as JsonObject);
						if(jUrl == null) { Debug.Assert(false); continue; }

						JsonValue jvUri = jUrl.Items["uri"];
						if(jvUri == null) { Debug.Assert(false); continue; }

						string strUri = jvUri.ToString();
						if(!string.IsNullOrEmpty(strUri))
							lUris.Add(strUri);
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}
	}
}
