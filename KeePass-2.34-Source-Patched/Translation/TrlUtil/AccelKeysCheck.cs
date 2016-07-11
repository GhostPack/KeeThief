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
using System.Windows.Forms;
using System.Diagnostics;

using KeePassLib.Translation;
using KeePassLib.Utility;

namespace TrlUtil
{
	public static class AccelKeysCheck
	{
		private static char GetAccelKey(string strText)
		{
			if(strText == null) { Debug.Assert(false); return char.MinValue; }
			if(strText.Length == 0) return char.MinValue;

			strText = strText.Replace(@"&&", string.Empty);

			Debug.Assert(strText.IndexOf('&') == strText.LastIndexOf('&'));

			int nIndex = strText.IndexOf('&');
			Debug.Assert(nIndex != (strText.Length - 1));

			if((nIndex >= 0) && (nIndex < (strText.Length - 1)))
				return char.ToUpper(strText[nIndex + 1]);

			return char.MinValue;
		}

		public static string Validate(KPTranslation trl)
		{
			if(trl == null) { Debug.Assert(false); return null; }

			foreach(KPFormCustomization kpfc in trl.Forms)
			{
				string str = Validate(kpfc);
				if(str != null) return str;
			}

			return null;
		}

		private static string Validate(KPFormCustomization kpfc)
		{
			if(kpfc == null) { Debug.Assert(false); return null; }
			if(kpfc.FormEnglish == null) { Debug.Assert(false); return null; }

			string str = Validate(kpfc, kpfc.FormEnglish, null);
			if(str != null) return str;

			return null;
		}

		private static string Validate(KPFormCustomization kpfc, Control c,
			Dictionary<char, string> dictParent)
		{
			if(kpfc == null) { Debug.Assert(false); return null; }
			if(kpfc.FormEnglish == null) { Debug.Assert(false); return null; }
			if(c == null) { Debug.Assert(false); return null; }

			Dictionary<char, string> dictAccel = new Dictionary<char, string>();

			foreach(Control cSub in c.Controls)
			{
				string strText = Translate(kpfc, cSub);
				char chKey = GetAccelKey(strText);
				if(chKey == char.MinValue) continue;

				string strId = kpfc.FullName + "." + cSub.Name + " - \"" +
					strText + "\"";

				bool bCollides = dictAccel.ContainsKey(chKey);
				bool bCollidesParent = ((dictParent != null) ?
					dictParent.ContainsKey(chKey) : false);

				if(bCollides || bCollidesParent)
				{
					string strMsg = "Key " + chKey.ToString() + ":";
					strMsg += MessageService.NewLine;
					strMsg += (bCollides ? dictAccel[chKey] : dictParent[chKey]);
					strMsg += MessageService.NewLine + strId;
					return strMsg;
				}

				dictAccel.Add(chKey, strId);
			}

			Dictionary<char, string> dictSub = MergeDictionaries(dictParent, dictAccel);
			foreach(Control cSub in c.Controls)
			{
				string str = Validate(kpfc, cSub, dictSub);
				if(str != null) return str;
			}

			return null;
		}

		private static string Translate(KPFormCustomization kpfc, Control c)
		{
			string strName = c.Name;
			if(string.IsNullOrEmpty(strName)) return string.Empty;

			foreach(KPControlCustomization cc in kpfc.Controls)
			{
				if(cc.Name == strName)
				{
					if(!string.IsNullOrEmpty(cc.TextEnglish))
					{
						Debug.Assert(c.Text == cc.TextEnglish);
					}

					return cc.Text;
				}
			}

			return c.Text;
		}

		private static Dictionary<char, string> MergeDictionaries(
			Dictionary<char, string> x, Dictionary<char, string> y)
		{
			Dictionary<char, string> d = new Dictionary<char, string>();

			if(x != null)
			{
				foreach(KeyValuePair<char, string> kvp in x)
					d[kvp.Key] = kvp.Value;
			}

			if(y != null)
			{
				foreach(KeyValuePair<char, string> kvp in y)
					d[kvp.Key] = kvp.Value;
			}

			return d;
		}
	}
}
