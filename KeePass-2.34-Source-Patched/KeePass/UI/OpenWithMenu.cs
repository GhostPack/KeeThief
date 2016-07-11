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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

using Microsoft.Win32;

using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Native;
using KeePassLib.Utility;

namespace KeePass.UI
{
	internal enum OwFilePathType
	{
		/// <summary>
		/// Path to an executable file that is invoked with
		/// the target URI as command line parameter.
		/// </summary>
		Executable = 0,

		/// <summary>
		/// Shell path (e.g. URI) in which a placeholder is
		/// replaced by the target URI.
		/// </summary>
		ShellExpand
	}

	internal sealed class OpenWithItem
	{
		private string m_strPath;
		public string FilePath { get { return m_strPath; } }

		private OwFilePathType m_tPath;
		public OwFilePathType FilePathType { get { return m_tPath; } }

		private string m_strMenuText;
		// public string MenuText { get { return m_strMenuText; } }

		private Image m_imgIcon;
		public Image Image { get { return m_imgIcon; } }

		private ToolStripMenuItem m_tsmi;
		public ToolStripMenuItem MenuItem { get { return m_tsmi; } }

		public OpenWithItem(string strFilePath, OwFilePathType tPath,
			string strMenuText, Image imgIcon, DynamicMenu dynMenu)
		{
			m_strPath = strFilePath;
			m_tPath = tPath;
			m_strMenuText = strMenuText;
			m_imgIcon = imgIcon;

			m_tsmi = dynMenu.AddItem(m_strMenuText, m_imgIcon, this);

			try { m_tsmi.ToolTipText = m_strPath; }
			catch(Exception) { } // Too long?
		}
	}

	public sealed class OpenWithMenu
	{
		private ToolStripDropDownItem m_tsmiHost;
		private DynamicMenu m_dynMenu;

		private List<OpenWithItem> m_lOpenWith = null;

		private const string PlhTargetUri = @"{OW_URI}";

		public OpenWithMenu(ToolStripDropDownItem tsmiHost)
		{
			if(tsmiHost == null) { Debug.Assert(false); return; }

			m_tsmiHost = tsmiHost;
			m_dynMenu = new DynamicMenu(m_tsmiHost);
			m_dynMenu.MenuClick += this.OnOpenUrl;

			m_tsmiHost.DropDownOpening += this.OnMenuOpening;
		}

		~OpenWithMenu()
		{
			Destroy();
		}

		public void Destroy()
		{
			if(m_dynMenu != null)
			{
				m_dynMenu.Clear();
				m_dynMenu.MenuClick -= this.OnOpenUrl;
				m_dynMenu = null;

				m_tsmiHost.DropDownOpening -= this.OnMenuOpening;
				m_tsmiHost = null;

				// After the menu has been destroyed:
				ReleaseOpenWithList(); // Release icons, ...
			}
		}

		private void OnMenuOpening(object sender, EventArgs e)
		{
			if(m_dynMenu == null) { Debug.Assert(false); return; }

			if(m_lOpenWith == null) CreateOpenWithList();

			PwEntry[] v = Program.MainForm.GetSelectedEntries();
			if(v == null) v = new PwEntry[0];

			bool bCanOpenWith = true;
			uint uValidUrls = 0;
			foreach(PwEntry pe in v)
			{
				string strUrl = pe.Strings.ReadSafe(PwDefs.UrlField);
				if(string.IsNullOrEmpty(strUrl)) continue;

				++uValidUrls;
				bCanOpenWith &= !WinUtil.IsCommandLineUrl(strUrl);
			}
			if((v.Length == 0) || (uValidUrls == 0)) bCanOpenWith = false;

			foreach(OpenWithItem it in m_lOpenWith)
				it.MenuItem.Enabled = bCanOpenWith;
		}

		private void CreateOpenWithList()
		{
			ReleaseOpenWithList();

			m_dynMenu.Clear();
			m_dynMenu.AddSeparator();

			m_lOpenWith = new List<OpenWithItem>();
			FindAppsByKnown();
			FindAppsByRegistry();

			if(m_lOpenWith.Count == 0) m_dynMenu.Clear(); // Remove separator
		}

		private void ReleaseOpenWithList()
		{
			if(m_lOpenWith == null) return;

			foreach(OpenWithItem it in m_lOpenWith)
			{
				if(it.Image != null) it.Image.Dispose();
			}

			m_lOpenWith = null;
		}

		private void OnOpenUrl(object sender, DynamicMenuEventArgs e)
		{
			if(e == null) { Debug.Assert(false); return; }

			OpenWithItem it = (e.Tag as OpenWithItem);
			if(it == null) { Debug.Assert(false); return; }

			string strApp = it.FilePath;

			PwEntry[] v = Program.MainForm.GetSelectedEntries();
			if(v == null) { Debug.Assert(false); return; }

			foreach(PwEntry pe in v)
			{
				string strUrl = pe.Strings.ReadSafe(PwDefs.UrlField);
				if(string.IsNullOrEmpty(strUrl)) continue;

				if(it.FilePathType == OwFilePathType.Executable)
					WinUtil.OpenUrlWithApp(strUrl, pe, strApp);
				else if(it.FilePathType == OwFilePathType.ShellExpand)
				{
					string str = strApp.Replace(PlhTargetUri, strUrl);
					WinUtil.OpenUrl(str, pe, false);
				}
				else { Debug.Assert(false); }
			}
		}

		private bool AddAppByFile(string strAppCmdLine, string strName)
		{
			if(string.IsNullOrEmpty(strAppCmdLine)) return false; // No assert

			string strPath = UrlUtil.GetShortestAbsolutePath(
				UrlUtil.GetQuotedAppPath(strAppCmdLine).Trim());
			if(strPath.Length == 0) { Debug.Assert(false); return false; }

			foreach(OpenWithItem it in m_lOpenWith)
			{
				if(it.FilePath.Equals(strPath, StrUtil.CaseIgnoreCmp))
					return false; // Already have an item for this
			}

			// Filter non-existing/legacy applications
			try { if(!File.Exists(strPath)) return false; }
			catch(Exception) { Debug.Assert(false); return false; }

			if(string.IsNullOrEmpty(strName))
				strName = UrlUtil.StripExtension(UrlUtil.GetFileName(strPath));

			Image img = UIUtil.GetFileIcon(strPath, DpiUtil.ScaleIntX(16),
				DpiUtil.ScaleIntY(16));

			string strMenuText = KPRes.OpenWith.Replace(@"{PARAM}", strName);
			OpenWithItem owi = new OpenWithItem(strPath, OwFilePathType.Executable,
				strMenuText, img, m_dynMenu);
			m_lOpenWith.Add(owi);
			return true;
		}

		private void AddAppByShellExpand(string strShell, string strName,
			string strIconExe)
		{
			if(string.IsNullOrEmpty(strShell)) return;

			if(string.IsNullOrEmpty(strName))
				strName = strShell;

			Image img = null;
			if(!string.IsNullOrEmpty(strIconExe))
				img = UIUtil.GetFileIcon(strIconExe, DpiUtil.ScaleIntX(16),
					DpiUtil.ScaleIntY(16));

			string strMenuText = KPRes.OpenWith.Replace(@"{PARAM}", strName);
			OpenWithItem owi = new OpenWithItem(strShell, OwFilePathType.ShellExpand,
				strMenuText, img, m_dynMenu);
			m_lOpenWith.Add(owi);
		}

		private void FindAppsByKnown()
		{
			string strIE = AppLocator.InternetExplorerPath;
			if(AddAppByFile(strIE, @"&Internet Explorer"))
			{
				// https://msdn.microsoft.com/en-us/library/hh826025.aspx
				AddAppByShellExpand("cmd://\"" + strIE + "\" -private \"" +
					PlhTargetUri + "\"", "Internet Explorer (" + KPRes.Private + ")", strIE);
			}

			if(AppLocator.EdgeProtocolSupported)
				AddAppByShellExpand("microsoft-edge:" + PlhTargetUri, @"&Edge",
					AppLocator.EdgePath);

			string strFF = AppLocator.FirefoxPath;
			if(AddAppByFile(strFF, @"&Firefox"))
			{
				// The command line options -private and -private-window do not work;
				// https://developer.mozilla.org/en-US/docs/Mozilla/Command_Line_Options
				// https://bugzilla.mozilla.org/show_bug.cgi?id=856839
				// https://bugzilla.mozilla.org/show_bug.cgi?id=829180
				// AddAppByShellExpand("cmd://\"" + strFF + "\" -private-window \"" +
				//	PlhTargetUri + "\"", "Firefox (" + KPRes.Private + ")", strFF);
			}

			string strCh = AppLocator.ChromePath;
			if(AddAppByFile(strCh, @"&Google Chrome"))
			{
				// https://www.chromium.org/developers/how-tos/run-chromium-with-flags
				// http://peter.sh/examples/?/chromium-switches.html
				AddAppByShellExpand("cmd://\"" + strCh + "\" --incognito \"" +
					PlhTargetUri + "\"", "Google Chrome (" + KPRes.Private + ")", strCh);
			}

			string strOp = AppLocator.OperaPath;
			if(AddAppByFile(strOp, @"O&pera"))
			{
				// Doesn't work with Opera 34.0.2036.25:
				// AddAppByShellExpand("cmd://\"" + strOp + "\" -newprivatetab \"" +
				//	PlhTargetUri + "\"", "Opera (" + KPRes.Private + ")", strOp);

				// Doesn't work with Opera 36.0.2130.65:
				// AddAppByShellExpand("cmd://\"" + strOp + "\" --incognito \"" +
				//	PlhTargetUri + "\"", "Opera (" + KPRes.Private + ")", strOp);
			}

			AddAppByFile(AppLocator.SafariPath, @"&Safari");

			if(NativeLib.IsUnix())
			{
				AddAppByFile(AppLocator.FindAppUnix("epiphany-browser"), @"&Epiphany");
				AddAppByFile(AppLocator.FindAppUnix("galeon"), @"Ga&leon");
				AddAppByFile(AppLocator.FindAppUnix("konqueror"), @"&Konqueror");
				AddAppByFile(AppLocator.FindAppUnix("rekonq"), @"&Rekonq");
				AddAppByFile(AppLocator.FindAppUnix("arora"), @"&Arora");
				AddAppByFile(AppLocator.FindAppUnix("midori"), @"&Midori");
				AddAppByFile(AppLocator.FindAppUnix("Dooble"), @"&Dooble"); // Upper-case
			}
		}

		private void FindAppsByRegistry()
		{
			const string strSmiDef = "SOFTWARE\\Clients\\StartMenuInternet";
			const string strSmiWow = "SOFTWARE\\Wow6432Node\\Clients\\StartMenuInternet";

			// https://msdn.microsoft.com/en-us/library/windows/desktop/dd203067.aspx
			try { FindAppsByRegistryPriv(Registry.CurrentUser, strSmiDef); }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
			try { FindAppsByRegistryPriv(Registry.CurrentUser, strSmiWow); }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
			try { FindAppsByRegistryPriv(Registry.LocalMachine, strSmiDef); }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
			try { FindAppsByRegistryPriv(Registry.LocalMachine, strSmiWow); }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); }
		}

		private void FindAppsByRegistryPriv(RegistryKey kBase, string strRootSubKey)
		{
			RegistryKey kRoot = kBase.OpenSubKey(strRootSubKey, false);
			if(kRoot == null) return; // No assert, key might not exist
			string[] vAppSubKeys = kRoot.GetSubKeyNames();

			foreach(string strAppSubKey in vAppSubKeys)
			{
				RegistryKey kApp = kRoot.OpenSubKey(strAppSubKey, false);
				string strName = (kApp.GetValue(string.Empty) as string);
				string strAltName = null;

				RegistryKey kCmd = kApp.OpenSubKey("shell\\open\\command", false);
				if(kCmd == null) { kApp.Close(); continue; } // No assert (XP)
				string strCmdLine = (kCmd.GetValue(string.Empty) as string);
				kCmd.Close();

				RegistryKey kCaps = kApp.OpenSubKey("Capabilities", false);
				if(kCaps != null)
				{
					strAltName = (kCaps.GetValue("ApplicationName") as string);
					kCaps.Close();
				}

				kApp.Close();

				string strDisplayName = string.Empty;
				if(strName != null) strDisplayName = strName;
				if((strAltName != null) && (strAltName.Length <= strDisplayName.Length))
					strDisplayName = strAltName;

				AddAppByFile(strCmdLine, strDisplayName);
			}

			kRoot.Close();
		}
	}
}
