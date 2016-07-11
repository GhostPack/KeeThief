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
using System.Drawing;
using System.Threading;
using System.Diagnostics;

using KeePass.Native;
using KeePass.Resources;
using KeePass.Util;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Utility;

using NativeLib = KeePassLib.Native.NativeLib;

namespace KeePass.DataExchange.Formats
{
	internal sealed class WinFavorites10 : FileFormatProvider
	{
		private readonly bool m_bInRoot;

		private const string IniTypeKey = "Type";
		private const string IniTypeValue = "WinFav-Export 1.0";

		private const string LnkDescSuffix = (@" [" + PwDefs.ShortProductName + @"]");

		public override bool SupportsImport { get { return false; } }
		public override bool SupportsExport { get { return true; } }

		public override string FormatName
		{
			get
			{
				return (KPRes.WindowsFavorites + " (" + (m_bInRoot ?
					KPRes.RootDirectory : (KPRes.Folder + @" '" +
					GetFolderName(true, null, null) + @"'")) + ")");
			}
		}
		public override bool RequiresFile { get { return false; } }
		public override string ApplicationGroup { get { return KPRes.General; } }

		public override Image SmallIcon
		{
			get { return Properties.Resources.B16x16_WinFavs; }
		}

		public WinFavorites10(bool bInRoot) : base()
		{
			m_bInRoot = bInRoot;
		}

		private static string GetFolderName(bool bForceRoot, PwExportInfo pwExportInfo,
			PwGroup pg)
		{
			string strBaseName = UrlUtil.FilterFileName(string.IsNullOrEmpty(
				Program.Config.Defaults.WinFavsBaseFolderName) ? PwDefs.ShortProductName :
				Program.Config.Defaults.WinFavsBaseFolderName);
			if(bForceRoot || (pwExportInfo == null) || (pg == null))
				return strBaseName;

			string strGroup = UrlUtil.FilterFileName(pg.Name);
			string strRootName = strBaseName;
			if(strGroup.Length > 0) strRootName += (" - " + strGroup);

			if(pwExportInfo.ContextDatabase != null)
			{
				if(pg == pwExportInfo.ContextDatabase.RootGroup)
					strRootName = strBaseName;
			}

			return strRootName;
		}

		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput,
			IStatusLogger slLogger)
		{
			PwGroup pg = pwExportInfo.DataGroup;
			if(pg == null) { Debug.Assert(false); return true; }

			string strFavsRoot = Environment.GetFolderPath(
				Environment.SpecialFolder.Favorites);
			if(string.IsNullOrEmpty(strFavsRoot)) return false;

			uint uTotalGroups, uTotalEntries, uEntriesProcessed = 0;
			pwExportInfo.DataGroup.GetCounts(true, out uTotalGroups, out uTotalEntries);

			if(!m_bInRoot) // In folder
			{
				string strRootName = GetFolderName(false, pwExportInfo, pg);

				string strFavsSub = UrlUtil.EnsureTerminatingSeparator(
					strFavsRoot, false) + strRootName;
				if(Directory.Exists(strFavsSub))
				{
					Directory.Delete(strFavsSub, true);
					WaitForDirCommit(strFavsSub, false);
				}

				ExportGroup(pwExportInfo.DataGroup, strFavsSub, slLogger,
					uTotalEntries, ref uEntriesProcessed, pwExportInfo);
			}
			else // In root
			{
				DeletePreviousExport(strFavsRoot, slLogger);
				ExportGroup(pwExportInfo.DataGroup, strFavsRoot, slLogger,
					uTotalEntries, ref uEntriesProcessed, pwExportInfo);
			}

			Debug.Assert(uEntriesProcessed == uTotalEntries);
			return true;
		}

		private static void ExportGroup(PwGroup pg, string strDir, IStatusLogger slLogger,
			uint uTotalEntries, ref uint uEntriesProcessed, PwExportInfo pxi)
		{
			foreach(PwEntry pe in pg.Entries)
			{
				ExportEntry(pe, strDir, pxi);

				++uEntriesProcessed;
				if(slLogger != null)
					slLogger.SetProgress(((uEntriesProcessed * 50U) /
						uTotalEntries) + 50U);
			}

			foreach(PwGroup pgSub in pg.Groups)
			{
				string strGroup = UrlUtil.FilterFileName(pgSub.Name);
				string strSub = (UrlUtil.EnsureTerminatingSeparator(strDir, false) +
					(!string.IsNullOrEmpty(strGroup) ? strGroup : KPRes.Group));

				ExportGroup(pgSub, strSub, slLogger, uTotalEntries,
					ref uEntriesProcessed, pxi);
			}
		}

		private static void ExportEntry(PwEntry pe, string strDir, PwExportInfo pxi)
		{
			PwDatabase pd = ((pxi != null) ? pxi.ContextDatabase : null);
			SprContext ctx = new SprContext(pe, pd, SprCompileFlags.NonActive, false, false);

			KeyValuePair<string, string>? okvpCmd = null;
			string strUrl = SprEngine.Compile(pe.Strings.ReadSafe(PwDefs.UrlField), ctx);
			if(WinUtil.IsCommandLineUrl(strUrl))
			{
				strUrl = WinUtil.GetCommandLineFromUrl(strUrl);

				if(!NativeLib.IsUnix()) // LNKs only supported on Windows
				{
					string strApp, strArgs;
					StrUtil.SplitCommandLine(strUrl, out strApp, out strArgs);

					if(!string.IsNullOrEmpty(strApp))
						okvpCmd = new KeyValuePair<string, string>(strApp, strArgs);
				}
			}
			if(string.IsNullOrEmpty(strUrl)) return;
			bool bLnk = okvpCmd.HasValue;

			string strTitleCmp = SprEngine.Compile(pe.Strings.ReadSafe(PwDefs.TitleField), ctx);
			if(string.IsNullOrEmpty(strTitleCmp)) strTitleCmp = KPRes.Entry;
			string strTitle = Program.Config.Defaults.WinFavsFileNamePrefix + strTitleCmp;

			string strSuffix = Program.Config.Defaults.WinFavsFileNameSuffix +
				(bLnk ? ".lnk" : ".url");
			strSuffix = UrlUtil.FilterFileName(strSuffix);

			string strFileBase = (UrlUtil.EnsureTerminatingSeparator(strDir,
				false) + UrlUtil.FilterFileName(strTitle));
			string strFile = strFileBase + strSuffix;
			int iFind = 2;
			while(File.Exists(strFile))
			{
				strFile = strFileBase + " (" + iFind.ToString() + ")" + strSuffix;
				++iFind;
			}

			if(!Directory.Exists(strDir))
			{
				try { Directory.CreateDirectory(strDir); }
				catch(Exception exDir)
				{
					throw new Exception(strDir + MessageService.NewParagraph + exDir.Message);
				}

				WaitForDirCommit(strDir, true);
			}

			try
			{
				if(bLnk)
				{
					int ccMaxDesc = NativeMethods.INFOTIPSIZE - 1 - LnkDescSuffix.Length;
					string strDesc = StrUtil.CompactString3Dots(strUrl, ccMaxDesc) +
						LnkDescSuffix;

					ShellLinkEx sl = new ShellLinkEx(okvpCmd.Value.Key,
						okvpCmd.Value.Value, strDesc);
					sl.Save(strFile);
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine(@"[InternetShortcut]");
					sb.AppendLine(@"URL=" + strUrl); // No additional line break
					sb.AppendLine(@"[" + PwDefs.ShortProductName + @"]");
					sb.AppendLine(IniTypeKey + @"=" + IniTypeValue);
					// Terminating line break is important

					File.WriteAllText(strFile, sb.ToString(), Encoding.Default);
				}
			}
			catch(Exception exWrite)
			{
				throw new Exception(strFile + MessageService.NewParagraph + exWrite.Message);
			}
		}

		private static void WaitForDirCommit(string strDir, bool bRequireExists)
		{
			for(int i = 0; i < 20; ++i)
			{
				bool bExists = Directory.Exists(strDir);
				if(bExists && bRequireExists) return;
				if(!bExists && !bRequireExists) return;

				Thread.Sleep(50);
			}
		}

		private static void DeletePreviousExport(string strDir, IStatusLogger slLogger)
		{
			List<string> vDirsToDelete = new List<string>();

			try
			{
				List<string> lUrlFiles = UrlUtil.GetFilePaths(strDir, "*.url",
					SearchOption.AllDirectories);
				List<string> lLnkFiles = UrlUtil.GetFilePaths(strDir, "*.lnk",
					SearchOption.AllDirectories);

				List<string> lFiles = new List<string>();
				lFiles.AddRange(lUrlFiles);
				lFiles.AddRange(lLnkFiles);

				for(int iFile = 0; iFile < lFiles.Count; ++iFile)
				{
					string strFile = lFiles[iFile];
					try
					{
						bool bDelete = false;

						if(strFile.EndsWith(".url", StrUtil.CaseIgnoreCmp))
						{
							IniFile ini = IniFile.Read(strFile, Encoding.Default);
							string strType = ini.Get(PwDefs.ShortProductName, IniTypeKey);
							bDelete = ((strType != null) && (strType == IniTypeValue));
						}
						else if(strFile.EndsWith(".lnk", StrUtil.CaseIgnoreCmp))
						{
							ShellLinkEx sl = ShellLinkEx.Load(strFile);
							if(sl != null)
								bDelete = ((sl.Description != null) &&
									sl.Description.EndsWith(LnkDescSuffix));
						}
						else { Debug.Assert(false); }

						if(bDelete)
						{
							File.Delete(strFile);

							string strCont = UrlUtil.GetFileDirectory(strFile, false, true);
							if(vDirsToDelete.IndexOf(strCont) < 0)
								vDirsToDelete.Add(strCont);
						}
					}
					catch(Exception) { Debug.Assert(false); }

					if(slLogger != null)
						slLogger.SetProgress(((uint)iFile * 50U) / (uint)lFiles.Count);
				}

				bool bDeleted = true;
				while(bDeleted)
				{
					bDeleted = false;

					for(int i = (vDirsToDelete.Count - 1); i >= 0; --i)
					{
						try
						{
							Directory.Delete(vDirsToDelete[i], false);
							WaitForDirCommit(vDirsToDelete[i], false);

							vDirsToDelete.RemoveAt(i);
							bDeleted = true;
						}
						catch(Exception) { } // E.g. not empty
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}
	}
}
