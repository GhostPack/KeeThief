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
using System.Xml;

using KeePass.Util;

using KeePassLib.Native;
using KeePassLib.Utility;

namespace KeePass.UI
{
	public static class UISystemFonts
	{
		private static bool m_bInitialized = false;

		private static Font m_fontUI = null;
		public static Font DefaultFont
		{
			get { EnsureInitialized(); return m_fontUI; }
		}

		private static Font m_fontList = null;
		public static Font ListFont
		{
			get { EnsureInitialized(); return m_fontList; }
		}

		private static void EnsureInitialized()
		{
			if(m_bInitialized) return;

			if(NativeLib.IsUnix())
			{
				try { UnixLoadFonts(); }
				catch(Exception) { Debug.Assert(false); }
			}

			if(m_fontUI == null) m_fontUI = SystemFonts.DefaultFont;

			if(m_fontList == null)
			{
				if(UIUtil.VistaStyleListsSupported)
				{
					string str1 = SystemFonts.IconTitleFont.ToString();
					string str2 = SystemFonts.StatusFont.ToString();
					if(str1 == str2) m_fontList = SystemFonts.StatusFont;
					else m_fontList = m_fontUI;
				}
				else m_fontList = m_fontUI;
			}

			m_bInitialized = true;
		}

		private static void UnixLoadFonts()
		{
			// string strSession = Environment.GetEnvironmentVariable("DESKTOP_SESSION");
			// "Default", "KDE", "Gnome", "Ubuntu", ...
			// string strKde = Environment.GetEnvironmentVariable("KDE_FULL_SESSION");

			string strHome = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if(string.IsNullOrEmpty(strHome)) { Debug.Assert(false); return; }
			strHome = UrlUtil.EnsureTerminatingSeparator(strHome, false);

			KdeLoadFonts(strHome);
			if(m_fontUI == null) GnomeLoadFonts(strHome);
			if(m_fontUI == null) UbuntuLoadFonts();
		}

		private static void KdeLoadFonts(string strHome)
		{
			string strKdeConfig = strHome + ".kde/share/config/kdeglobals";
			if(!File.Exists(strKdeConfig))
			{
				strKdeConfig = strHome + ".kde4/share/config/kdeglobals";
				if(!File.Exists(strKdeConfig))
				{
					strKdeConfig = strHome + ".kde3/share/config/kdeglobals";
					if(!File.Exists(strKdeConfig)) return;
				}
			}

			IniFile ini = IniFile.Read(strKdeConfig, Encoding.UTF8);

			string strFont = ini.Get("General", "font");
			if(string.IsNullOrEmpty(strFont)) { Debug.Assert(false); return; }

			m_fontUI = KdeCreateFont(strFont);
		}

		private static Font KdeCreateFont(string strDef)
		{
			string[] v = strDef.Split(new char[] { ',' });
			if((v == null) || (v.Length < 6)) { Debug.Assert(false); return null; }

			for(int i = 0; i < v.Length; ++i)
				v[i] = v[i].Trim();

			float fSize;
			if(!float.TryParse(v[1], out fSize)) { Debug.Assert(false); return null; }

			FontStyle fs = FontStyle.Regular;
			if(v[4] == "75") fs |= FontStyle.Bold;
			if(v[5] == "2") fs |= FontStyle.Italic;

			return FontUtil.CreateFont(v[0], fSize, fs);
		}

		private static void GnomeLoadFonts(string strHome)
		{
			string strConfig = strHome + @".gconf/desktop/gnome/interface/%gconf.xml";
			if(!File.Exists(strConfig)) return;

			XmlDocument doc = new XmlDocument();
			doc.Load(strConfig);

			foreach(XmlNode xn in doc.DocumentElement.ChildNodes)
			{
				if(string.Equals(xn.Name, "entry") &&
					string.Equals(xn.Attributes.GetNamedItem("name").Value, "font_name"))
				{
					m_fontUI = GnomeCreateFont(xn.FirstChild.InnerText);
					break;
				}
			}
		}

		private static Font GnomeCreateFont(string strDef)
		{
			int iSep = strDef.LastIndexOf(' ');
			if(iSep < 0) { Debug.Assert(false); return null; }

			string strName = strDef.Substring(0, iSep);

			float fSize = float.Parse(strDef.Substring(iSep + 1));

			FontStyle fs = FontStyle.Regular;
			// Name can end with "Bold", "Italic", "Bold Italic", ...
			if(strName.EndsWith(" Oblique", StrUtil.CaseIgnoreCmp)) // Gnome
			{
				fs |= FontStyle.Italic;
				strName = strName.Substring(0, strName.Length - 8);
			}
			if(strName.EndsWith(" Italic", StrUtil.CaseIgnoreCmp)) // Ubuntu
			{
				fs |= FontStyle.Italic;
				strName = strName.Substring(0, strName.Length - 7);
			}
			if(strName.EndsWith(" Bold", StrUtil.CaseIgnoreCmp))
			{
				fs |= FontStyle.Bold;
				strName = strName.Substring(0, strName.Length - 5);
			}

			return FontUtil.CreateFont(strName, fSize, fs);
		}

		private static void UbuntuLoadFonts()
		{
			string strDef = NativeLib.RunConsoleApp("gsettings",
				"get org.gnome.desktop.interface font-name");
			if(strDef == null) return;

			strDef = strDef.Trim(new char[] { ' ', '\t', '\r', '\n', '\'', '\"' });
			if(strDef.Length == 0) return;

			m_fontUI = GnomeCreateFont(strDef);
		}
	}
}
