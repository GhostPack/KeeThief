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
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	// 5.12
	internal sealed class HandySafeTxt512 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Handy Safe TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_HandySafe; }
		}

		private const string StrGroupStart = "[Category: ";
		private const string StrGroupEnd = "]";
		private const string StrEntryStart = "[Card, ";
		private const string StrEntryEnd = "]";
		private const string StrNotesBegin = "Note:";
		private const string StrFieldSplit = ": ";

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);
			string strData = sr.ReadToEnd();
			sr.Close();

			strData = strData.Replace("\r", string.Empty);
			string[] vLines = strData.Split(new char[] { '\n' });

			PwGroup pg = pwStorage.RootGroup;
			Dictionary<string, string> dItems = new Dictionary<string, string>();
			bool bInNotes = false;

			foreach(string strLine in vLines)
			{
				if(strLine.StartsWith(StrGroupStart) && strLine.EndsWith(StrGroupEnd))
				{
					AddEntry(pg, dItems, ref bInNotes);
					dItems.Clear();

					pg = new PwGroup(true, true);
					pg.Name = strLine.Substring(StrGroupStart.Length, strLine.Length -
						StrGroupStart.Length - StrGroupEnd.Length);

					pwStorage.RootGroup.AddGroup(pg, true);
				}
				else if(strLine.StartsWith(StrEntryStart) && strLine.EndsWith(StrEntryEnd))
				{
					AddEntry(pg, dItems, ref bInNotes);
					dItems.Clear();
				}
				else if(strLine == StrNotesBegin) bInNotes = true;
				else if(bInNotes)
				{
					if(dItems.ContainsKey(PwDefs.NotesField))
						dItems[PwDefs.NotesField] += MessageService.NewLine + strLine;
					else dItems[PwDefs.NotesField] = strLine;
				}
				else
				{
					int nSplitPos = strLine.IndexOf(StrFieldSplit);
					if(nSplitPos < 0) { Debug.Assert(false); }
					else
					{
						AddField(dItems, strLine.Substring(0, nSplitPos),
							strLine.Substring(nSplitPos + StrFieldSplit.Length));
					}
				}
			}

			AddEntry(pg, dItems, ref bInNotes);
		}

		private static void AddField(Dictionary<string, string> dItems,
			string strKey, string strValue)
		{
			string strKeyTrl = ImportUtil.MapNameToStandardField(strKey, true);
			if(string.IsNullOrEmpty(strKeyTrl)) strKeyTrl = strKey;

			if(!dItems.ContainsKey(strKeyTrl))
			{
				dItems[strKeyTrl] = strValue;
				return;
			}

			string strPreValue = dItems[strKeyTrl];
			if((strPreValue.Length > 0) && (strValue.Length > 0))
				strPreValue += ", ";

			dItems[strKeyTrl] = strPreValue + strValue;
		}

		private static void AddEntry(PwGroup pg, Dictionary<string, string> dItems,
			ref bool bInNotes)
		{
			if(dItems.Count == 0) return;

			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			foreach(KeyValuePair<string, string> kvp in dItems)
				pe.Strings.Set(kvp.Key, new ProtectedString(false, kvp.Value));

			bInNotes = false;
		}
	}
}
