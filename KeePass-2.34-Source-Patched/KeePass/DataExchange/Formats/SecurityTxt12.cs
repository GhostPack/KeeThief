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

namespace KeePass.DataExchange.Formats
{
	// 1.2
	internal sealed class SecurityTxt12 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Security TXT"; } }
		public override string DefaultExtension { get { return "txt"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_Security; }
		}

		internal sealed class SecLine
		{
			public string Text = string.Empty;
			public List<SecLine> SubLines = new List<SecLine>();
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			StreamReader sr = new StreamReader(sInput, Encoding.Default);

			Stack<SecLine> vGroups = new Stack<SecLine>();
			SecLine secRoot = new SecLine();
			vGroups.Push(secRoot);

			char[] vTrim = new char[]{ '\t', '\n', '\r', ' ' };

			while(true)
			{
				string str = sr.ReadLine();
				if(str == null) break;
				if(str.Length == 0) continue;

				SecLine line = new SecLine();
				line.Text = str.Trim(vTrim);

				int nTabs = CountTabs(str);

				if(nTabs == vGroups.Count)
				{
					vGroups.Peek().SubLines.Add(line);
					vGroups.Push(line);
				}
				else
				{
					while(nTabs < (vGroups.Count - 1))
						vGroups.Pop();

					vGroups.Peek().SubLines.Add(line);
					vGroups.Push(line);
				}
			}

			AddSecLine(pwStorage.RootGroup, secRoot, true, pwStorage);

			sr.Close();
		}

		private static int CountTabs(string str)
		{
			Debug.Assert(str != null); if(str == null) return 0;

			int nTabs = 0;

			for(int i = 0; i < str.Length; ++i)
			{
				if(str[i] != '\t') break;
				++nTabs;
			}

			return nTabs;
		}

		private void AddSecLine(PwGroup pgContainer, SecLine line, bool bIsContainer,
			PwDatabase pwParent)
		{
			if(!bIsContainer)
			{
				if(line.SubLines.Count > 0)
				{
					PwGroup pg = new PwGroup(true, true);
					pg.Name = line.Text;

					pgContainer.AddGroup(pg, true);

					pgContainer = pg;
				}
				else
				{
					PwEntry pe = new PwEntry(true, true);
					pgContainer.AddEntry(pe, true);

					pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
						pwParent.MemoryProtection.ProtectTitle, line.Text));
				}
			}

			foreach(SecLine subLine in line.SubLines)
				AddSecLine(pgContainer, subLine, false, pwParent);
		}
	}
}
