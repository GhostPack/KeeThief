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
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using KeePass.Util;

using KeePassLib;
using KeePassLib.Native;

namespace KeePass.UI.ToolStripRendering
{
	public static class TsrPool
	{
		private static List<TsrFactory> g_lFacs = null;
		private static int g_nStdFac = 0;

		internal static List<TsrFactory> Factories
		{
			get
			{
				EnsureFactories();
				return g_lFacs;
			}
		}

		private static void EnsureFactories()
		{
			if(g_lFacs != null) return;

			TsrFactory fKP = new KeePassTsrFactory();
			TsrFactory f81 = new Win81TsrFactory();
			TsrFactory f10 = new Win10TsrFactory();
			TsrFactory fP = new ProExtTsrFactory();

			TsrFactory fS;
			try { fS = new SystemTsrFactory(); }
			catch(Exception) { Debug.Assert(false); fS = fP; }

			// https://sourceforge.net/p/keepass/discussion/329220/thread/fab85f1d/
			// http://keepass.info/help/kb/tsrstyles_survey.html
			TsrFactory[] vPref;
			if(WinUtil.IsAtLeastWindows10)
				vPref = new TsrFactory[] { f10, f81, fKP, fP, fS };
			else if(WinUtil.IsAtLeastWindows8)
				vPref = new TsrFactory[] { f81, f10, fKP, fP, fS };
			else if(NativeLib.IsUnix())
				vPref = new TsrFactory[] { f81, f10, fKP, fP, fS };
			else // Older Windows systems
				vPref = new TsrFactory[] { fKP, f10, f81, fP, fS };

			List<TsrFactory> l = new List<TsrFactory>(vPref);

#if DEBUG
			for(int i = 0; i < l.Count; ++i)
			{
				TsrFactory f1 = l[i];
				if(f1 == null) { Debug.Assert(false); continue; }
				if(f1.Uuid == null) { Debug.Assert(false); continue; }

				for(int j = i + 1; j < l.Count; ++j)
				{
					TsrFactory f2 = l[j];
					if(f2 == null) { Debug.Assert(false); continue; }
					if(f2.Uuid == null) { Debug.Assert(false); continue; }

					Debug.Assert(!f1.Uuid.Equals(f2.Uuid));
				}
			}
#endif

			g_lFacs = l;
			g_nStdFac = l.Count;
		}

		private static TsrFactory GetFactory(PwUuid u)
		{
			if(u == null) { Debug.Assert(false); return null; }

			foreach(TsrFactory f in TsrPool.Factories)
			{
				if(u.Equals(f.Uuid)) return f;
			}

			return null;
		}

		public static bool AddFactory(TsrFactory f)
		{
			if(f == null) { Debug.Assert(false); return false; }

			TsrFactory fEx = GetFactory(f.Uuid);
			if(fEx != null) return false; // Exists already

			TsrPool.Factories.Add(f);
			return true;
		}

		public static bool RemoveFactory(PwUuid u)
		{
			if(u == null) { Debug.Assert(false); return false; }

			List<TsrFactory> l = TsrPool.Factories;
			int cInitial = l.Count;

			for(int i = l.Count - 1; i >= g_nStdFac; --i)
			{
				if(u.Equals(l[i].Uuid)) l.RemoveAt(i);
			}

			return (l.Count != cInitial);
		}

		internal static ToolStripRenderer GetBestRenderer(string strUuid)
		{
			PwUuid u = PwUuid.Zero;
			try
			{
				if(!string.IsNullOrEmpty(strUuid))
					u = new PwUuid(Convert.FromBase64String(strUuid));
			}
			catch(Exception) { Debug.Assert(false); }

			return GetBestRenderer(u);
		}

		internal static ToolStripRenderer GetBestRenderer(PwUuid u)
		{
			TsrFactory fPref = null;
			if((u == null) || PwUuid.Zero.Equals(u)) { }
			else fPref = GetFactory(u);

			List<TsrFactory> lPref = new List<TsrFactory>();
			if(fPref != null) lPref.Add(fPref);
			lPref.AddRange(TsrPool.Factories);

			foreach(TsrFactory fCand in lPref)
			{
				if((fCand != null) && fCand.IsSupported())
				{
					try
					{
						ToolStripRenderer tsr = fCand.CreateInstance();
						if(tsr != null) return tsr;
					}
					catch(Exception) { Debug.Assert(false); }
				}
			}

			return null;
		}
	}
}
