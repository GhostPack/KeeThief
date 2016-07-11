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
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using KeePass.App;
using KeePass.Resources;
using KeePass.Plugins;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Utility;

namespace KeePass.Plugins
{
	internal sealed class PluginManager : IEnumerable<PluginInfo>
	{
		private List<PluginInfo> m_vPlugins = new List<PluginInfo>();
		private IPluginHost m_host = null;

		public void Initialize(IPluginHost host)
		{
			Debug.Assert(host != null);
			m_host = host;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_vPlugins.GetEnumerator();
		}
		
		public IEnumerator<PluginInfo> GetEnumerator()
		{
			return m_vPlugins.GetEnumerator();
		}

		public void LoadAllPlugins(string strDirectory, SearchOption so,
			string[] vExclNames)
		{
			Debug.Assert(m_host != null);

			try
			{
				string strPath = strDirectory;
				if(!Directory.Exists(strPath)) return; // No assert

				DirectoryInfo di = new DirectoryInfo(strPath);

				List<FileInfo> lFiles = UrlUtil.GetFileInfos(di, "*.dll", so);
				FilterList(lFiles, vExclNames);
				LoadPlugins(lFiles, null, null, true);

				lFiles = UrlUtil.GetFileInfos(di, "*.exe", so);
				FilterList(lFiles, vExclNames);
				LoadPlugins(lFiles, null, null, true);

				lFiles = UrlUtil.GetFileInfos(di, "*." + PlgxPlugin.PlgxExtension, so);
				FilterList(lFiles, vExclNames);
				if(lFiles.Count > 0)
				{
					OnDemandStatusDialog dlgStatus = new OnDemandStatusDialog(true, null);
					dlgStatus.StartLogging(PwDefs.ShortProductName, false);

					foreach(FileInfo fi in lFiles)
						PlgxPlugin.Load(fi.FullName, dlgStatus);

					dlgStatus.EndLogging();
				}
			}
			catch(Exception) { Debug.Assert(false); } // Path access violation
		}

		public void LoadPlugin(string strFilePath, string strTypeName,
			string strDisplayFilePath, bool bSkipCacheFile)
		{
			if(strFilePath == null) throw new ArgumentNullException("strFilePath");

			List<FileInfo> l = new List<FileInfo>();
			l.Add(new FileInfo(strFilePath));

			LoadPlugins(l, strTypeName, strDisplayFilePath, bSkipCacheFile);
		}

		private void LoadPlugins(List<FileInfo> lFiles, string strTypeName,
			string strDisplayFilePath, bool bSkipCacheFiles)
		{
			string strCacheRoot = PlgxCache.GetCacheRoot();

			foreach(FileInfo fi in lFiles)
			{
				if(bSkipCacheFiles && fi.FullName.StartsWith(strCacheRoot,
					StrUtil.CaseIgnoreCmp))
					continue;

				FileVersionInfo fvi = null;
				try
				{
					fvi = FileVersionInfo.GetVersionInfo(fi.FullName);

					if((fvi == null) || (fvi.ProductName == null) ||
						(fvi.ProductName != AppDefs.PluginProductName))
					{
						continue;
					}
				}
				catch(Exception) { continue; }

				Exception exShowStd = null;
				try
				{
					PluginInfo pi = new PluginInfo(fi.FullName, fvi, strDisplayFilePath);

					pi.Interface = CreatePluginInstance(pi.FilePath, strTypeName);

					if(!pi.Interface.Initialize(m_host))
						continue; // Fail without error

					m_vPlugins.Add(pi);
				}
				catch(BadImageFormatException exBif)
				{
					if(Is1xPlugin(fi.FullName))
						MessageService.ShowWarning(KPRes.PluginIncompatible +
							MessageService.NewLine + fi.FullName + MessageService.NewParagraph +
							KPRes.Plugin1x + MessageService.NewParagraph + KPRes.Plugin1xHint);
					else exShowStd = exBif;
				}
				catch(Exception exLoad)
				{
					if(Program.CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null)
						MessageService.ShowWarningExcp(fi.FullName, exLoad);
					else exShowStd = exLoad;
				}

				if(exShowStd != null)
					ShowLoadError(fi.FullName, exShowStd, null);
			}
		}

		internal static void ShowLoadError(string strPath, Exception ex,
			IStatusLogger slStatus)
		{
			if(string.IsNullOrEmpty(strPath)) { Debug.Assert(false); return; }

			if(slStatus != null)
				slStatus.SetText(KPRes.PluginLoadFailed, LogStatusType.Info);

			bool bShowExcp = (Program.CommandLineArgs[
				AppDefs.CommandLineOptions.Debug] != null);

			string strMsg = KPRes.PluginIncompatible + MessageService.NewLine +
				strPath + MessageService.NewParagraph + KPRes.PluginUpdateHint;
			string strExcp = ((ex != null) ? StrUtil.FormatException(ex).Trim() : null);

			VistaTaskDialog vtd = new VistaTaskDialog();
			vtd.Content = strMsg;
			vtd.ExpandedByDefault = ((strExcp != null) && bShowExcp);
			vtd.ExpandedInformation = strExcp;
			vtd.WindowTitle = PwDefs.ShortProductName;
			vtd.SetIcon(VtdIcon.Warning);

			if(!vtd.ShowDialog())
			{
				if(!bShowExcp) MessageService.ShowWarning(strMsg);
				else MessageService.ShowWarningExcp(strPath, ex);
			}
		}

		public void UnloadAllPlugins()
		{
			foreach(PluginInfo plugin in m_vPlugins)
			{
				Debug.Assert(plugin.Interface != null);
				if(plugin.Interface != null)
				{
					try { plugin.Interface.Terminate(); }
					catch(Exception) { Debug.Assert(false); }
				}
			}

			m_vPlugins.Clear();
		}

		private static Plugin CreatePluginInstance(string strFilePath,
			string strTypeName)
		{
			Debug.Assert(strFilePath != null);
			if(strFilePath == null) throw new ArgumentNullException("strFilePath");

			string strType;
			if(string.IsNullOrEmpty(strTypeName))
			{
				strType = UrlUtil.GetFileName(strFilePath);
				strType = UrlUtil.StripExtension(strType) + "." +
					UrlUtil.StripExtension(strType) + "Ext";
			}
			else strType = strTypeName + "." + strTypeName + "Ext";

			ObjectHandle oh = Activator.CreateInstanceFrom(strFilePath, strType);

			Plugin plugin = (oh.Unwrap() as Plugin);
			if(plugin == null) throw new FileLoadException();
			return plugin;
		}

		private static bool Is1xPlugin(string strFile)
		{
			try
			{
				byte[] pbFile = File.ReadAllBytes(strFile);
				byte[] pbSig = StrUtil.Utf8.GetBytes("KpCreateInstance");
				string strData = MemUtil.ByteArrayToHexString(pbFile);
				string strSig = MemUtil.ByteArrayToHexString(pbSig);

				return (strData.IndexOf(strSig) >= 0);
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		private static void FilterList(List<FileInfo> l, string[] vExclNames)
		{
			if((l == null) || (vExclNames == null)) { Debug.Assert(false); return; }

			for(int i = l.Count - 1; i >= 0; --i)
			{
				string strName = UrlUtil.GetFileName(l[i].FullName);

				foreach(string strExcl in vExclNames)
				{
					if(string.IsNullOrEmpty(strExcl)) { Debug.Assert(false); continue; }

					if(string.Equals(strName, strExcl, StrUtil.CaseIgnoreCmp))
					{
						l.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
}
