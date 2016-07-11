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
using System.Drawing;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public sealed class RichTextBuilder
	{
		private StringBuilder m_sb = new StringBuilder();

		private static List<RtfbTag> m_vTags = null;

		private sealed class RtfbTag
		{
			public string IdCode { get; private set; }
			public string RtfCode { get; private set; }
			public bool StartTag { get; private set; }
			public FontStyle Style { get; private set; }

			public RtfbTag(string strId, string strRtf, bool bStartTag, FontStyle fs)
			{
				if(string.IsNullOrEmpty(strId)) strId = GenerateRandomIdCode();
				this.IdCode = strId;

				this.RtfCode = strRtf;
				this.StartTag = bStartTag;
				this.Style = fs;
			}

			internal static string GenerateRandomIdCode()
			{
				StringBuilder sb = new StringBuilder(14);
				for(int i = 0; i < 12; ++i)
				{
					int n = Program.GlobalRandom.Next(62);
					if(n < 26) sb.Append((char)('A' + n));
					else if(n < 52) sb.Append((char)('a' + (n - 26)));
					else sb.Append((char)('0' + (n - 52)));
				}
				return sb.ToString();
			}
		}

		private Font m_fDefault = null;
		public Font DefaultFont
		{
			get { return m_fDefault; }
			set { m_fDefault = value; }
		}

		public RichTextBuilder()
		{
			EnsureInitializedStatic();
		}

		private static void EnsureInitializedStatic()
		{
			if(m_vTags != null) return;

			List<RtfbTag> l = new List<RtfbTag>();
			l.Add(new RtfbTag(null, "\\b ", true, FontStyle.Bold));
			l.Add(new RtfbTag(null, "\\b0 ", false, FontStyle.Bold));
			l.Add(new RtfbTag(null, "\\i ", true, FontStyle.Italic));
			l.Add(new RtfbTag(null, "\\i0 ", false, FontStyle.Italic));
			l.Add(new RtfbTag(null, "\\ul ", true, FontStyle.Underline));
			l.Add(new RtfbTag(null, "\\ul0 ", false, FontStyle.Underline));
			l.Add(new RtfbTag(null, "\\strike ", true, FontStyle.Strikeout));
			l.Add(new RtfbTag(null, "\\strike0 ", false, FontStyle.Strikeout));
			m_vTags = l;
		}

		public static KeyValuePair<string, string> GetStyleIdCodes(FontStyle fs)
		{
			string strL = null, strR = null;

			foreach(RtfbTag rTag in m_vTags)
			{
				if(rTag.Style == fs)
				{
					if(rTag.StartTag) strL = rTag.IdCode;
					else strR = rTag.IdCode;
				}
			}

			return new KeyValuePair<string, string>(strL, strR);
		}

		public void Append(string str)
		{
			m_sb.Append(str);
		}

		public void AppendLine()
		{
			m_sb.AppendLine();
		}

		public void AppendLine(string str)
		{
			m_sb.AppendLine(str);
		}

		public void Append(string str, FontStyle fs)
		{
			Append(str, fs, null, null, null, null);
		}

		public void Append(string str, FontStyle fs, string strOuterPrefix,
			string strInnerPrefix, string strInnerSuffix, string strOuterSuffix)
		{
			KeyValuePair<string, string> kvpTags = GetStyleIdCodes(fs);

			if(!string.IsNullOrEmpty(strOuterPrefix)) m_sb.Append(strOuterPrefix);

			if(!string.IsNullOrEmpty(kvpTags.Key)) m_sb.Append(kvpTags.Key);
			if(!string.IsNullOrEmpty(strInnerPrefix)) m_sb.Append(strInnerPrefix);
			m_sb.Append(str);
			if(!string.IsNullOrEmpty(strInnerSuffix)) m_sb.Append(strInnerSuffix);
			if(!string.IsNullOrEmpty(kvpTags.Value)) m_sb.Append(kvpTags.Value);

			if(!string.IsNullOrEmpty(strOuterSuffix)) m_sb.Append(strOuterSuffix);
		}

		public void AppendLine(string str, FontStyle fs, string strOuterPrefix,
			string strInnerPrefix, string strInnerSuffix, string strOuterSuffix)
		{
			Append(str, fs, strOuterPrefix, strInnerPrefix, strInnerSuffix, strOuterSuffix);
			m_sb.AppendLine();
		}

		public void AppendLine(string str, FontStyle fs)
		{
			Append(str, fs);
			m_sb.AppendLine();
		}

		private static RichTextBox CreateOpRtb()
		{
			RichTextBox rtbOp = new RichTextBox();
			rtbOp.Visible = false; // Ensure invisibility
			rtbOp.DetectUrls = false;
			rtbOp.HideSelection = true;
			rtbOp.Multiline = true;
			rtbOp.WordWrap = false;

			return rtbOp;
		}

		public void Build(RichTextBox rtb)
		{
			if(rtb == null) throw new ArgumentNullException("rtb");

			RichTextBox rtbOp = CreateOpRtb();
			string strText = m_sb.ToString();

			Dictionary<char, string> dEnc = new Dictionary<char, string>();
			if(MonoWorkarounds.IsRequired(586901))
			{
				StringBuilder sbEnc = new StringBuilder();
				for(int i = 0; i < strText.Length; ++i)
				{
					char ch = strText[i];
					if((int)ch <= 255) sbEnc.Append(ch);
					else
					{
						string strCharEnc;
						if(!dEnc.TryGetValue(ch, out strCharEnc))
						{
							strCharEnc = RtfbTag.GenerateRandomIdCode();
							dEnc[ch] = strCharEnc;
						}
						sbEnc.Append(strCharEnc);
					}
				}
				strText = sbEnc.ToString();
			}

			rtbOp.Text = strText;
			Debug.Assert(rtbOp.Text == strText); // Test committed

			if(m_fDefault != null)
			{
				rtbOp.Select(0, rtbOp.TextLength);
				rtbOp.SelectionFont = m_fDefault;
			}

			string strRtf = rtbOp.Rtf;
			rtbOp.Dispose();

			foreach(KeyValuePair<char, string> kvpEnc in dEnc)
			{
				strRtf = strRtf.Replace(kvpEnc.Value,
					StrUtil.RtfEncodeChar(kvpEnc.Key));
			}
			foreach(RtfbTag rTag in m_vTags)
			{
				strRtf = strRtf.Replace(rTag.IdCode, rTag.RtfCode);
			}

			rtb.Rtf = strRtf;
		}
	}
}
