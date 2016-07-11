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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using KeePass.DataExchange.Formats;

using KeePassLib.Utility;

namespace KeePass.DataExchange
{
	public sealed class FileFormatPool : IEnumerable<FileFormatProvider>
	{
		private List<FileFormatProvider> m_vFormats = null;

		public IEnumerable<FileFormatProvider> Importers
		{
			get
			{
				EnsurePoolInitialized();

				List<FileFormatProvider> v = new List<FileFormatProvider>();
				foreach(FileFormatProvider prov in m_vFormats)
				{
					if(prov.SupportsImport) v.Add(prov);
				}

				return v;
			}
		}

		public IEnumerable<FileFormatProvider> Exporters
		{
			get
			{
				EnsurePoolInitialized();

				List<FileFormatProvider> v = new List<FileFormatProvider>();
				foreach(FileFormatProvider prov in m_vFormats)
				{
					if(prov.SupportsExport) v.Add(prov);
				}

				return v;
			}
		}

		public int Count
		{
			get
			{
				EnsurePoolInitialized();
				return m_vFormats.Count;
			}
		}

		public FileFormatPool() { }

		IEnumerator IEnumerable.GetEnumerator()
		{
			EnsurePoolInitialized();
			return m_vFormats.GetEnumerator();
		}

		public IEnumerator<FileFormatProvider> GetEnumerator()
		{
			EnsurePoolInitialized();
			return m_vFormats.GetEnumerator();
		}

		private void EnsurePoolInitialized()
		{
			if(m_vFormats != null) return;

			InitializePool();
		}

		private void InitializePool()
		{
			Debug.Assert(m_vFormats == null);
			m_vFormats = new List<FileFormatProvider>();

			m_vFormats.Add(new KeePassCsv1x());
			m_vFormats.Add(new KeePassKdb1x());
			m_vFormats.Add(new KeePassKdb2x());
			m_vFormats.Add(new KeePassKdb2xRepair());
			m_vFormats.Add(new KeePassXml1x());
			m_vFormats.Add(new KeePassXml2x());

			m_vFormats.Add(new GenericCsv());

			m_vFormats.Add(new KeePassHtml2x());
			m_vFormats.Add(new XslTransform2x());
			m_vFormats.Add(new WinFavorites10(false));
			m_vFormats.Add(new WinFavorites10(true));

			m_vFormats.Add(new OnePwProCsv599());
			m_vFormats.Add(new AmpXml250());
			m_vFormats.Add(new AnyPwCsv144());
			m_vFormats.Add(new CodeWalletTxt605());
			m_vFormats.Add(new DashlaneCsv2());
			m_vFormats.Add(new DataVaultCsv47());
			m_vFormats.Add(new DesktopKnox32());
			m_vFormats.Add(new FlexWalletXml17());
			m_vFormats.Add(new HandySafeTxt512());
			m_vFormats.Add(new HandySafeProXml12());
			m_vFormats.Add(new KasperskyPwMgrXml50());
			m_vFormats.Add(new KeePassXXml041());
			m_vFormats.Add(new LastPassCsv2());
			m_vFormats.Add(new NetworkPwMgrCsv4());
			m_vFormats.Add(new NortonIdSafeCsv2013());
			m_vFormats.Add(new NPasswordNpw102());
			m_vFormats.Add(new PassKeeper12());
			m_vFormats.Add(new PpKeeperHtml270());
			m_vFormats.Add(new PwAgentXml234());
			m_vFormats.Add(new PwDepotXml26());
			m_vFormats.Add(new PwKeeperCsv70());
			m_vFormats.Add(new PwMemory2008Xml104());
			m_vFormats.Add(new PwPrompterDat12());
			m_vFormats.Add(new PwSafeXml302());
			m_vFormats.Add(new PwsPlusCsv1007());
			m_vFormats.Add(new PwTresorXml100());
			m_vFormats.Add(new PVaultTxt14());
			m_vFormats.Add(new PinsTxt450());
			m_vFormats.Add(new RevelationXml04());
			m_vFormats.Add(new RoboFormHtml69());
			m_vFormats.Add(new SafeWalletXml3());
			m_vFormats.Add(new SecurityTxt12());
			m_vFormats.Add(new SplashIdCsv402());
			m_vFormats.Add(new SteganosPwManager2007());
			m_vFormats.Add(new StickyPwXml50());
			m_vFormats.Add(new TurboPwsCsv5());
			m_vFormats.Add(new VisKeeperTxt3());
			m_vFormats.Add(new Whisper32Csv116());
			m_vFormats.Add(new ZdnPwProTxt314());

			m_vFormats.Add(new MozillaBookmarksHtml100());
			m_vFormats.Add(new MozillaBookmarksJson100());
			m_vFormats.Add(new PwExporterXml105());

			m_vFormats.Add(new Spamex20070328());

#if DEBUG
			// Ensure name uniqueness
			for(int i = 0; i < m_vFormats.Count; ++i)
			{
				FileFormatProvider pi = m_vFormats[i];
				for(int j = i + 1; j < m_vFormats.Count; ++j)
				{
					FileFormatProvider pj = m_vFormats[j];
					Debug.Assert(!string.Equals(pi.FormatName, pj.FormatName, StrUtil.CaseIgnoreCmp));
					Debug.Assert(!string.Equals(pi.FormatName, pj.DisplayName, StrUtil.CaseIgnoreCmp));
					Debug.Assert(!string.Equals(pi.DisplayName, pj.FormatName, StrUtil.CaseIgnoreCmp));
					Debug.Assert(!string.Equals(pi.DisplayName, pj.DisplayName, StrUtil.CaseIgnoreCmp));
				}
			}
#endif
		}

		public void Add(FileFormatProvider prov)
		{
			Debug.Assert(prov != null);
			if(prov == null) throw new ArgumentNullException("prov");

			EnsurePoolInitialized();

			m_vFormats.Add(prov);
		}

		public bool Remove(FileFormatProvider prov)
		{
			Debug.Assert(prov != null);
			if(prov == null) throw new ArgumentNullException("prov");

			EnsurePoolInitialized();

			return m_vFormats.Remove(prov);
		}

		public FileFormatProvider Find(string strFormatName)
		{
			if(strFormatName == null) return null;

			EnsurePoolInitialized();

			// Format and display names may differ (e.g. the Generic
			// CSV Importer has a different format name)

			foreach(FileFormatProvider f in m_vFormats)
			{
				if(string.Equals(strFormatName, f.DisplayName, StrUtil.CaseIgnoreCmp))
					return f;
			}

			foreach(FileFormatProvider f in m_vFormats)
			{
				if(string.Equals(strFormatName, f.FormatName, StrUtil.CaseIgnoreCmp))
					return f;
			}

			return null;
		}
	}
}
