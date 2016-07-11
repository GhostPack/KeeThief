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

using KeePassLib.Translation;
using KeePassLib.Utility;

namespace TrlUtil
{
	public static class TrlImport
	{
		public static void Import1xLng(KPTranslation kpInto, string strFile)
		{
			if((strFile == null) || (strFile.Length == 0)) { Debug.Assert(false); return; }

			string strData = File.ReadAllText(strFile, StrUtil.Utf8);

			Dictionary<string, string> dict = new Dictionary<string, string>();

			const int nStatePreEn = 0;
			const int nStateInEn = 1;
			const int nStateBetween = 2;
			const int nStateInTrl = 3;

			StringBuilder sbEn = new StringBuilder();
			StringBuilder sbTrl = new StringBuilder();
			int nState = nStatePreEn;

			for(int i = 0; i < strData.Length; ++i)
			{
				char ch = strData[i];

				if(ch == '|')
				{
					if(nState == nStatePreEn) nState = nStateInEn;
					else if(nState == nStateInEn) nState = nStateBetween;
					else if(nState == nStateBetween) nState = nStateInTrl;
					else if(nState == nStateInTrl)
					{
						dict[sbEn.ToString()] = sbTrl.ToString();

						sbEn = new StringBuilder();
						sbTrl = new StringBuilder();

						nState = nStatePreEn;
					}
				}
				else if(nState == nStateInEn) sbEn.Append(ch);
				else if(nState == nStateInTrl) sbTrl.Append(ch);
			}

			Debug.Assert(nState == nStatePreEn);

			dict[string.Empty] = string.Empty;

			MergeDict(kpInto, dict);
		}

		private static void MergeDict(KPTranslation kpInto, Dictionary<string, string> dict)
		{
			if(kpInto == null) { Debug.Assert(false); return; }
			if(dict == null) { Debug.Assert(false); return; }

			foreach(KPStringTable kpst in kpInto.StringTables)
			{
				foreach(KPStringTableItem kpsti in kpst.Strings)
				{
					string strTrl;
					if(dict.TryGetValue(kpsti.ValueEnglish, out strTrl))
						kpsti.Value = strTrl;
				}
			}

			foreach(KPFormCustomization kpfc in kpInto.Forms)
			{
				string strTrlWnd;
				if(dict.TryGetValue(kpfc.Window.TextEnglish, out strTrlWnd))
					kpfc.Window.Text = strTrlWnd;

				foreach(KPControlCustomization kpcc in kpfc.Controls)
				{
					string strTrlCtrl;
					if(dict.TryGetValue(kpcc.TextEnglish, out strTrlCtrl))
						kpcc.Text = strTrlCtrl;
				}
			}
		}

		public static void ImportPo(KPTranslation kpInto, string strFile)
		{
			if((strFile == null) || (strFile.Length == 0)) { Debug.Assert(false); return; }

			string strData = File.ReadAllText(strFile, StrUtil.Utf8);
			strData = StrUtil.NormalizeNewLines(strData, false);
			string[] vData = strData.Split('\n');

			Dictionary<string, string> dict = new Dictionary<string, string>();
			string strID = string.Empty;
			foreach(string strLine in vData)
			{
				string str = strLine.Trim();
				if(str.StartsWith("msgid ", StrUtil.CaseIgnoreCmp))
					strID = FilterPoValue(str.Substring(6));
				else if(str.StartsWith("msgstr ", StrUtil.CaseIgnoreCmp))
				{
					if(strID.Length > 0)
					{
						dict[strID] = FilterPoValue(str.Substring(7));
						strID = string.Empty;
					}
				}
			}

			MergeDict(kpInto, dict);
		}

		private static string FilterPoValue(string str)
		{
			if(str == null) { Debug.Assert(false); return string.Empty; }

			if(str.StartsWith("\"") && str.EndsWith("\"") && (str.Length >= 2))
				str = str.Substring(1, str.Length - 2);
			else { Debug.Assert(false); }

			str = str.Replace("\\\"", "\"");

			return str;
		}
	}
}
