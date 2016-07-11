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
using System.Diagnostics;
using System.Drawing;
using System.IO;

using KeePass.Resources;

using KeePassLib.Utility;

namespace KeePass.Util
{
	public enum BinaryDataClass
	{
		Unknown = 0,
		Text,
		RichText,
		Image,
		WebDocument
	}

	public static class BinaryDataClassifier
	{
		private static readonly string[] m_vTextExtensions = new string[] {
			"txt", "csv", "c", "cpp", "h", "hpp", "css", "js", "bat"
		};

		private static readonly string[] m_vRichTextExtensions = new string[] {
			"rtf"
		};

		private static readonly string[] m_vImageExtensions = new string[] {
			"bmp", "emf", "exif", "gif", "ico", "jpeg", "jpe", "jpg",
			"png", "tiff", "tif", "wmf"
		};

		private static readonly string[] m_vWebExtensions = new string[] {
			"htm", "html"

			// The following types can be displayed by Internet Explorer,
			// but not by the WebBrowser control
			// "mht", "xml", "xslt"
		};

		public static BinaryDataClass ClassifyUrl(string strUrl)
		{
			Debug.Assert(strUrl != null);
			if(strUrl == null) throw new ArgumentNullException("strUrl");

			string str = strUrl.Trim().ToLower();

			foreach(string strTextExt in m_vTextExtensions)
			{
				if(str.EndsWith("." + strTextExt))
					return BinaryDataClass.Text;
			}

			foreach(string strRichTextExt in m_vRichTextExtensions)
			{
				if(str.EndsWith("." + strRichTextExt))
					return BinaryDataClass.RichText;
			}

			foreach(string strImageExt in m_vImageExtensions)
			{
				if(str.EndsWith("." + strImageExt))
					return BinaryDataClass.Image;
			}

			foreach(string strWebExt in m_vWebExtensions)
			{
				if(str.EndsWith("." + strWebExt))
					return BinaryDataClass.WebDocument;
			}

			return BinaryDataClass.Unknown;
		}

		public static BinaryDataClass ClassifyData(byte[] pbData)
		{
			Debug.Assert(pbData != null);
			if(pbData == null) throw new ArgumentNullException("pbData");

			try
			{
				Image img = GfxUtil.LoadImage(pbData);
				if(img != null)
				{
					img.Dispose();
					return BinaryDataClass.Image;
				}
			}
			catch(Exception) { }

			return BinaryDataClass.Unknown;
		}

		public static BinaryDataClass Classify(string strUrl, byte[] pbData)
		{
			BinaryDataClass bdc = ClassifyUrl(strUrl);
			if(bdc != BinaryDataClass.Unknown) return bdc;

			return ClassifyData(pbData);
		}

		public static StrEncodingInfo GetStringEncoding(byte[] pbData,
			out uint uStartOffset)
		{
			Debug.Assert(pbData != null);
			if(pbData == null) throw new ArgumentNullException("pbData");

			uStartOffset = 0;

			List<StrEncodingInfo> lEncs = new List<StrEncodingInfo>(StrUtil.Encodings);
			lEncs.Sort(BinaryDataClassifier.CompareBySigLengthRev);

			foreach(StrEncodingInfo sei in lEncs)
			{
				byte[] pbSig = sei.StartSignature;
				if((pbSig == null) || (pbSig.Length == 0)) continue;
				if(pbSig.Length > pbData.Length) continue;

				byte[] pbStart = MemUtil.Mid<byte>(pbData, 0, pbSig.Length);
				if(MemUtil.ArraysEqual(pbStart, pbSig))
				{
					uStartOffset = (uint)pbSig.Length;
					return sei;
				}
			}

			if((pbData.Length % 4) == 0)
			{
				byte[] z3 = new byte[] { 0, 0, 0 };
				int i = MemUtil.IndexOf<byte>(pbData, z3);
				if((i >= 0) && (i < (pbData.Length - 4))) // Ignore last zero char
				{
					if((i % 4) == 0) return StrUtil.GetEncoding(StrEncodingType.Utf32BE);
					if((i % 4) == 1) return StrUtil.GetEncoding(StrEncodingType.Utf32LE);
					// Don't assume UTF-32 for other offsets
				}
			}

			if((pbData.Length % 2) == 0)
			{
				int i = Array.IndexOf<byte>(pbData, 0);
				if((i >= 0) && (i < (pbData.Length - 2))) // Ignore last zero char
				{
					if((i % 2) == 0) return StrUtil.GetEncoding(StrEncodingType.Utf16BE);
					return StrUtil.GetEncoding(StrEncodingType.Utf16LE);
				}
			}

			try
			{
				UTF8Encoding utf8Throw = new UTF8Encoding(false, true);
				utf8Throw.GetString(pbData);
				return StrUtil.GetEncoding(StrEncodingType.Utf8);
			}
			catch(Exception) { }

			return StrUtil.GetEncoding(StrEncodingType.Default);
		}

		private static int CompareBySigLengthRev(StrEncodingInfo a, StrEncodingInfo b)
		{
			Debug.Assert((a != null) && (b != null));

			int na = 0, nb = 0;
			if((a != null) && (a.StartSignature != null))
				na = a.StartSignature.Length;
			if((b != null) && (b.StartSignature != null))
				nb = b.StartSignature.Length;

			return -(na.CompareTo(nb));
		}
	}
}
