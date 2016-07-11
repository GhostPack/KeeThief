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
using System.Drawing;
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Utility;
using KeePassLib.Security;

namespace KeePass.DataExchange.Formats
{
	// 3.3.0+
	internal sealed class VisKeeperTxt3 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "VisKeeper TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_VisKeeper; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default, true);
			string strData = sr.ReadToEnd();
			sr.Close();

			strData = StrUtil.NormalizeNewLines(strData, false);

			const string strInitGroup = "\n\nOrdner:";
			const string strInitTemplate = "\n\nVorlage:";
			const string strInitEntry = "\n\nEintrag:";
			const string strInitNote = "\n\nNotiz:";
			string[] vSeps = new string[] { strInitGroup, strInitTemplate,
				strInitEntry, strInitNote };

			List<string> lData = StrUtil.SplitWithSep(strData, vSeps, true);
			Debug.Assert((lData.Count & 1) == 1);

			PwGroup pgRoot = pwStorage.RootGroup;

			PwGroup pgTemplates = new PwGroup(true, true, "Vorlagen",
				PwIcon.MarkedDirectory);
			pgRoot.AddGroup(pgTemplates, true);			

			for(int i = 1; (i + 1) < lData.Count; i += 2)
			{
				string strInit = lData[i];
				string strPart = lData[i + 1];

				if(strInit == strInitGroup) { }
				else if(strInit == strInitTemplate)
					ImportEntry(strPart, pgTemplates, pwStorage, false);
				else if(strInit == strInitEntry)
					ImportEntry(strPart, pgRoot, pwStorage, false);
				else if(strInit == strInitNote)
					ImportEntry(strPart, pgRoot, pwStorage, true);
				else { Debug.Assert(false); }
			}
		}

		private static void ImportEntry(string strData, PwGroup pg, PwDatabase pd,
			bool bForceNotes)
		{
			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			string[] v = strData.Split('\n');

			if(v.Length == 0) { Debug.Assert(false); return; }
			ImportUtil.AppendToField(pe, PwDefs.TitleField, v[0].Trim(), pd);

			int n = v.Length;
			for(int j = n - 1; j >= 0; --j)
			{
				if(v[j].Length > 0) break;
				--n;
			}

			bool bInNotes = bForceNotes;
			for(int i = 1; i < n; ++i)
			{
				string str = v[i];

				int iSep = str.IndexOf(':');
				if(iSep <= 0) bInNotes = true;

				if(bInNotes)
				{
					if(str.Length == 0)
						ImportUtil.AppendToField(pe, PwDefs.NotesField,
							Environment.NewLine, pd, string.Empty, false);
					else
						ImportUtil.AppendToField(pe, PwDefs.NotesField,
							str, pd, Environment.NewLine, false);
				}
				else
				{
					string strRawKey = str.Substring(0, iSep);
					string strValue = str.Substring(iSep + 1).Trim();

					string strKey = ImportUtil.MapNameToStandardField(strRawKey, false);
					if(string.IsNullOrEmpty(strKey)) strKey = strRawKey;

					bool bMultiLine = ((strKey == PwDefs.NotesField) ||
						!PwDefs.IsStandardField(strKey));
					ImportUtil.AppendToField(pe, strKey, strValue, pd,
						(bMultiLine ? Environment.NewLine : ", "), false);
				}
			}
		}
	}
}
