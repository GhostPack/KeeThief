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
using System.Diagnostics;

using KeePass.UI;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Util.Archive
{
	internal sealed class ImageArchive
	{
		private Dictionary<string, ImageArchiveItem> m_dItems =
			new Dictionary<string, ImageArchiveItem>();

		private sealed class ImageArchiveItem
		{
			private readonly string m_strName;
			public string Name
			{
				get { return m_strName; }
			}

			private byte[] m_pbData;
			public byte[] Data
			{
				get { return m_pbData; }
			}

			private Image m_imgOrg = null;
			public Image Image
			{
				get
				{
					if(m_imgOrg != null) return m_imgOrg;

					try
					{
						Image img = GfxUtil.LoadImage(m_pbData);
						DpiUtil.AssertUIImage(img);

						m_imgOrg = img;
						return img;
					}
					catch(Exception) { Debug.Assert(false); }

					return null;
				}
			}

			public ImageArchiveItem(string strName, byte[] pbData)
			{
				if(strName == null) throw new ArgumentNullException("strName");
				if(pbData == null) throw new ArgumentNullException("pbData");

				m_strName = strName;
				m_pbData = pbData;
			}
		}

		public ImageArchive()
		{
		}

		public void Load(byte[] pbXspFile)
		{
			XspArchive a = new XspArchive();
			a.Load(pbXspFile);

			foreach(KeyValuePair<string, byte[]> kvp in a.Items)
			{
				string str = GetID(kvp.Key);
				ImageArchiveItem it = new ImageArchiveItem(kvp.Key, kvp.Value);

				Debug.Assert(!m_dItems.ContainsKey(str));
				m_dItems[str] = it;
			}
		}

		private static string GetID(string strFileName)
		{
			if(strFileName == null) { Debug.Assert(false); return string.Empty; }

			string str = strFileName;
			if(str.Length == 0) return string.Empty;

			char chFirst = str[0];
			if((chFirst == 'B') || (chFirst == 'b'))
			{
				int pL = str.IndexOf('_');
				int pR = str.LastIndexOf('.');
				if((pL >= 0) && (pR > pL))
					str = str.Substring(pL + 1, pR - pL - 1);
				else if(pL >= 0)
					str = str.Substring(pL + 1);
			}
			else str = UrlUtil.StripExtension(str);

			return str.ToLowerInvariant();
		}

		public Image GetForObject(string strObjectName)
		{
			if(strObjectName == null) { Debug.Assert(false); return null; }

			string str = GetID(strObjectName);

			ImageArchiveItem it;
			if(!m_dItems.TryGetValue(str, out it)) return null;
			if(it == null) { Debug.Assert(false); return null; }

			return it.Image;
		}

		public List<Image> GetImages(int w, int h, bool bSortByName)
		{
			if(w < 0) { Debug.Assert(false); return null; }
			if(h < 0) { Debug.Assert(false); return null; }

			List<ImageArchiveItem> lItems = new List<ImageArchiveItem>(
				m_dItems.Values);
			if(bSortByName) lItems.Sort(ImageArchive.CompareByName);

			List<Image> l = new List<Image>(lItems.Count);

			foreach(ImageArchiveItem it in lItems)
			{
				Image img = it.Image;
				if(img == null)
				{
					Debug.Assert(false);
					img = UIUtil.CreateColorBitmap24(w, h, Color.White);
				}

				if((img.Width == w) && (img.Height == h))
					l.Add(img);
				else l.Add(GfxUtil.ScaleImage(img, w, h, ScaleTransformFlags.UIIcon));
			}

			Debug.Assert(l.Count == lItems.Count);
			return l;
		}

		private static int CompareByName(ImageArchiveItem a, ImageArchiveItem b)
		{
			if(a == null) { Debug.Assert(false); return -1; }
			if(b == null) { Debug.Assert(false); return 1; }

			return string.Compare(a.Name, b.Name, StrUtil.CaseIgnoreCmp);
		}
	}
}
