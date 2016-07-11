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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using KeePass.App;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public enum UpdateComponentStatus
	{
		Unknown = 0,
		UpToDate,
		NewVerAvailable,
		PreRelease,
		DownloadFailed
	}

	public sealed class UpdateComponentInfo
	{
		private readonly string m_strName; // Never null
		public string Name
		{
			get { return m_strName; }
		}

		private readonly ulong m_uVerInstalled;
		public ulong VerInstalled
		{
			get { return m_uVerInstalled; }
		}

		private ulong m_uVerAvailable = 0;
		public ulong VerAvailable
		{
			get { return m_uVerAvailable; }
			set { m_uVerAvailable = value; }
		}

		private UpdateComponentStatus m_status = UpdateComponentStatus.Unknown;
		public UpdateComponentStatus Status
		{
			get { return m_status; }
			set { m_status = value; }
		}

		private readonly string m_strUpdateUrl; // Never null
		public string UpdateUrl
		{
			get { return m_strUpdateUrl; }
		}

		private readonly string m_strCat; // Never null
		public string Category
		{
			get { return m_strCat; }
		}

		public UpdateComponentInfo(string strName, ulong uVerInstalled,
			string strUpdateUrl, string strCategory)
		{
			if(strName == null) throw new ArgumentNullException("strName");
			if(strUpdateUrl == null) throw new ArgumentNullException("strUpdateUrl");
			if(strCategory == null) throw new ArgumentNullException("strCategory");

			m_strName = strName;
			m_uVerInstalled = uVerInstalled;
			m_strUpdateUrl = strUpdateUrl;
			m_strCat = strCategory;
		}
	}

	public static class UpdateCheckEx
	{
		private static Dictionary<string, string> g_dFileSigKeys =
			new Dictionary<string, string>();

		private sealed class UpdateCheckParams
		{
			public readonly bool ForceUI;
			public readonly Form Parent; // May be null

			public UpdateCheckParams(bool bForceUI, Form fOptParent)
			{
				this.ForceUI = bForceUI;
				this.Parent = fOptParent;
			}
		}

		public static void Run(bool bForceUI, Form fOptParent)
		{
			DateTime dtNow = DateTime.Now, dtLast;
			string strLast = Program.Config.Application.LastUpdateCheck;
			if(!bForceUI && (strLast.Length > 0) && TimeUtil.TryDeserializeUtc(
				strLast, out dtLast))
			{
				if(CompareDates(dtLast, dtNow) == 0) return; // Checked today already
			}
			Program.Config.Application.LastUpdateCheck = TimeUtil.SerializeUtc(dtNow);

			UpdateCheckParams p = new UpdateCheckParams(bForceUI, fOptParent);
			if(!bForceUI) // Async
			{
				// // Local, but thread will continue to run anyway
				// Thread th = new Thread(new ParameterizedThreadStart(
				//	UpdateCheckEx.RunPriv));
				// th.Start(p);

				try
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(
						UpdateCheckEx.RunPriv), p);
				}
				catch(Exception) { Debug.Assert(false); }
			}
			else RunPriv(p);
		}

		private static int CompareDates(DateTime a, DateTime b)
		{
			if(a.Year != b.Year) return ((a.Year < b.Year) ? -1 : 1);
			if(a.Month != b.Month) return ((a.Month < b.Month) ? -1 : 1);
			if(a.Day != b.Day) return ((a.Day < b.Day) ? -1 : 1);
			return 0;
		}

		private static void RunPriv(object o)
		{
			UpdateCheckParams p = (o as UpdateCheckParams);
			if(p == null) { Debug.Assert(false); return; }

			IStatusLogger sl = null;
			try
			{
				if(p.ForceUI)
				{
					Form fStatusDialog;
					sl = StatusUtil.CreateStatusDialog(p.Parent, out fStatusDialog,
						KPRes.UpdateCheck, KPRes.CheckingForUpd + "...", true, true);
				}

				List<UpdateComponentInfo> lInst = GetInstalledComponents();
				List<string> lUrls = GetUrls(lInst);
				Dictionary<string, List<UpdateComponentInfo>> dictAvail =
					DownloadInfoFiles(lUrls, sl);
				if(dictAvail == null) return; // User cancelled

				MergeInfo(lInst, dictAvail);

				bool bUpdAvail = false;
				foreach(UpdateComponentInfo uc in lInst)
				{
					if(uc.Status == UpdateComponentStatus.NewVerAvailable)
					{
						bUpdAvail = true;
						break;
					}
				}

				if(sl != null) { sl.EndLogging(); sl = null; }

				if(bUpdAvail || p.ForceUI)
					ShowUpdateDialogAsync(lInst, p.ForceUI);
			}
			catch(Exception) { Debug.Assert(false); }
			finally
			{
				try { if(sl != null) sl.EndLogging(); }
				catch(Exception) { Debug.Assert(false); }
			}
		}

		private static void ShowUpdateDialogAsync(List<UpdateComponentInfo> lInst,
			bool bModal)
		{
			try
			{
				MainForm mf = Program.MainForm;
				if((mf != null) && mf.InvokeRequired)
					mf.BeginInvoke(new UceShDlgDelegate(ShowUpdateDialogPriv),
						lInst, bModal);
				else ShowUpdateDialogPriv(lInst, bModal);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private delegate void UceShDlgDelegate(List<UpdateComponentInfo> lInst,
			bool bModal);
		private static void ShowUpdateDialogPriv(List<UpdateComponentInfo> lInst,
			bool bModal)
		{
			try
			{
				// Do not show the update dialog while auto-typing;
				// https://sourceforge.net/p/keepass/bugs/1265/
				if(SendInputEx.IsSending) return;

				UpdateCheckForm dlg = new UpdateCheckForm();
				dlg.InitEx(lInst, bModal);
				UIUtil.ShowDialogAndDestroy(dlg);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private sealed class UpdateDownloadInfo
		{
			public readonly string Url; // Never null
			public object SyncObj = new object();
			public bool Ready = false;
			public List<UpdateComponentInfo> ComponentInfo = null;

			public UpdateDownloadInfo(string strUrl)
			{
				if(strUrl == null) throw new ArgumentNullException("strUrl");

				this.Url = strUrl;
			}
		}

		private static Dictionary<string, List<UpdateComponentInfo>>
			DownloadInfoFiles(List<string> lUrls, IStatusLogger sl)
		{
			List<UpdateDownloadInfo> lDl = new List<UpdateDownloadInfo>();
			foreach(string strUrl in lUrls)
			{
				if(string.IsNullOrEmpty(strUrl)) { Debug.Assert(false); continue; }

				UpdateDownloadInfo dl = new UpdateDownloadInfo(strUrl);
				lDl.Add(dl);

				ThreadPool.QueueUserWorkItem(new WaitCallback(
					UpdateCheckEx.DownloadInfoFile), dl);
			}

			while(true)
			{
				bool bReady = true;
				foreach(UpdateDownloadInfo dl in lDl)
				{
					lock(dl.SyncObj) { bReady &= dl.Ready; }
				}

				if(bReady) break;
				Thread.Sleep(40);

				if(sl != null)
				{
					if(!sl.ContinueWork()) return null;
				}
			}

			Dictionary<string, List<UpdateComponentInfo>> dict =
				new Dictionary<string, List<UpdateComponentInfo>>();
			foreach(UpdateDownloadInfo dl in lDl)
			{
				dict[dl.Url.ToLower()] = dl.ComponentInfo;
			}
			return dict;
		}

		private static void DownloadInfoFile(object o)
		{
			UpdateDownloadInfo dl = (o as UpdateDownloadInfo);
			if(dl == null) { Debug.Assert(false); return; }

			dl.ComponentInfo = LoadInfoFile(dl.Url);
			lock(dl.SyncObj) { dl.Ready = true; }
		}

		private static List<string> GetUrls(List<UpdateComponentInfo> l)
		{
			List<string> lUrls = new List<string>();
			foreach(UpdateComponentInfo uc in l)
			{
				string strUrl = uc.UpdateUrl;
				if(string.IsNullOrEmpty(strUrl)) continue;

				bool bFound = false;
				for(int i = 0; i < lUrls.Count; ++i)
				{
					if(lUrls[i].Equals(strUrl, StrUtil.CaseIgnoreCmp))
					{
						bFound = true;
						break;
					}
				}

				if(!bFound) lUrls.Add(strUrl);
			}

			return lUrls;
		}

		private static List<UpdateComponentInfo> LoadInfoFile(string strUrl)
		{
			try
			{
				IOConnectionInfo ioc = IOConnectionInfo.FromPath(strUrl.Trim());

				Stream s = IOConnection.OpenRead(ioc);
				if(s == null) throw new InvalidOperationException();
				MemoryStream ms = new MemoryStream();
				MemUtil.CopyStream(s, ms);
				s.Close();
				byte[] pb = ms.ToArray();
				ms.Close();

				if(ioc.Path.EndsWith(".gz", StrUtil.CaseIgnoreCmp))
				{
					// Decompress in try-catch, because some web filters
					// incorrectly pre-decompress the returned data
					// https://sourceforge.net/projects/keepass/forums/forum/329221/topic/4915083
					try
					{
						byte[] pbDec = MemUtil.Decompress(pb);
						List<UpdateComponentInfo> l = LoadInfoFilePriv(pbDec, ioc);
						if(l != null) return l;
					}
					catch(Exception) { }
				}

				return LoadInfoFilePriv(pb, ioc);
			}
			catch(Exception) { }

			return null;
		}

		private static List<UpdateComponentInfo> LoadInfoFilePriv(byte[] pbData,
			IOConnectionInfo iocSource)
		{
			if((pbData == null) || (pbData.Length == 0)) return null;

			int iOffset = 0;
			StrEncodingInfo sei = StrUtil.GetEncoding(StrEncodingType.Utf8);
			byte[] pbBom = sei.StartSignature;
			if((pbData.Length >= pbBom.Length) && MemUtil.ArraysEqual(pbBom,
				MemUtil.Mid(pbData, 0, pbBom.Length)))
				iOffset += pbBom.Length;

			string strData = sei.Encoding.GetString(pbData, iOffset, pbData.Length - iOffset);
			strData = StrUtil.NormalizeNewLines(strData, false);
			string[] vLines = strData.Split('\n');

			string strSigKey;
			g_dFileSigKeys.TryGetValue(iocSource.Path.ToLowerInvariant(), out strSigKey);
			string strLdSig = null;
			StringBuilder sbToVerify = ((strSigKey != null) ? new StringBuilder() : null);

			List<UpdateComponentInfo> l = new List<UpdateComponentInfo>();
			bool bHeader = true, bFooterFound = false;
			char chSep = ':'; // Modified by header
			for(int i = 0; i < vLines.Length; ++i)
			{
				string str = vLines[i].Trim();
				if(str.Length == 0) continue;

				if(bHeader)
				{
					chSep = str[0];
					bHeader = false;

					string[] vHdr = str.Split(chSep);
					if(vHdr.Length >= 2) strLdSig = vHdr[1];
				}
				else if(str[0] == chSep)
				{
					bFooterFound = true;
					break;
				}
				else // Component info
				{
					if(sbToVerify != null)
					{
						sbToVerify.Append(str);
						sbToVerify.Append('\n');
					}

					string[] vInfo = str.Split(chSep);
					if(vInfo.Length >= 2)
					{
						UpdateComponentInfo c = new UpdateComponentInfo(
							vInfo[0].Trim(), 0, iocSource.Path, string.Empty);
						c.VerAvailable = StrUtil.ParseVersion(vInfo[1]);

						AddComponent(l, c);
					}
				}
			}
			if(!bFooterFound) { Debug.Assert(false); return null; }

			if(sbToVerify != null)
			{
				if(!VerifySignature(sbToVerify.ToString(), strLdSig, strSigKey))
					return null;
			}

			return l;
		}

		private static void AddComponent(List<UpdateComponentInfo> l,
			UpdateComponentInfo c)
		{
			if((l == null) || (c == null)) { Debug.Assert(false); return; }

			for(int i = l.Count - 1; i >= 0; --i)
			{
				if(l[i].Name.Equals(c.Name, StrUtil.CaseIgnoreCmp))
					l.RemoveAt(i);
			}

			l.Add(c);
		}

		private static List<UpdateComponentInfo> GetInstalledComponents()
		{
			List<UpdateComponentInfo> l = new List<UpdateComponentInfo>();

			foreach(PluginInfo pi in Program.MainForm.PluginManager)
			{
				Plugin p = pi.Interface;
				string strUrl = ((p != null) ? (p.UpdateUrl ?? string.Empty) :
					string.Empty);

				AddComponent(l, new UpdateComponentInfo(pi.Name.Trim(),
					StrUtil.ParseVersion(pi.FileVersion), strUrl.Trim(),
					KPRes.Plugins));
			}

			// Add KeePass at the end to override any buggy plugin names
			AddComponent(l, new UpdateComponentInfo(PwDefs.ShortProductName,
				PwDefs.FileVersion64, PwDefs.VersionUrl, PwDefs.ShortProductName));
			
			l.Sort(UpdateCheckEx.CompareComponents);
			return l;
		}

		private static int CompareComponents(UpdateComponentInfo a,
			UpdateComponentInfo b)
		{
			if(a.Name == b.Name) return 0;
			if(a.Name == PwDefs.ShortProductName) return -1;
			if(b.Name == PwDefs.ShortProductName) return 1;

			return a.Name.CompareTo(b.Name);
		}

		private static void MergeInfo(List<UpdateComponentInfo> lInst,
			Dictionary<string, List<UpdateComponentInfo>> dictAvail)
		{
			string strOvrId = PwDefs.VersionUrl.ToLower();
			List<UpdateComponentInfo> lOvr;
			dictAvail.TryGetValue(strOvrId, out lOvr);

			foreach(UpdateComponentInfo uc in lInst)
			{
				string strUrlId = uc.UpdateUrl.ToLower();
				List<UpdateComponentInfo> lAvail;
				dictAvail.TryGetValue(strUrlId, out lAvail);

				if(SetComponentAvail(uc, lOvr)) { }
				else if(SetComponentAvail(uc, lAvail)) { }
				else if((strUrlId.Length > 0) && (lAvail == null))
					uc.Status = UpdateComponentStatus.DownloadFailed;
				else uc.Status = UpdateComponentStatus.Unknown;
			}
		}

		private static bool SetComponentAvail(UpdateComponentInfo uc,
			List<UpdateComponentInfo> lAvail)
		{
			if(uc == null) { Debug.Assert(false); return false; }
			if(lAvail == null) return false; // No assert

			foreach(UpdateComponentInfo ucAvail in lAvail)
			{
				if(ucAvail.Name.Equals(uc.Name, StrUtil.CaseIgnoreCmp))
				{
					uc.VerAvailable = ucAvail.VerAvailable;

					if(uc.VerInstalled == uc.VerAvailable)
						uc.Status = UpdateComponentStatus.UpToDate;
					else if(uc.VerInstalled < uc.VerAvailable)
						uc.Status = UpdateComponentStatus.NewVerAvailable;
					else uc.Status = UpdateComponentStatus.PreRelease;

					return true;
				}
			}

			return false;
		}

		private static bool VerifySignature(string strContent, string strSig,
			string strKey)
		{
			if(string.IsNullOrEmpty(strSig)) { Debug.Assert(false); return false; }

			try
			{
				byte[] pbMsg = StrUtil.Utf8.GetBytes(strContent);
				byte[] pbSig = Convert.FromBase64String(strSig);

				using(SHA512Managed sha = new SHA512Managed())
				{
					using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
					{
						// Watching this code in the debugger may result in a
						// CryptographicException when disposing the object
						rsa.PersistKeyInCsp = false; // Default key
						rsa.FromXmlString(strKey);
						rsa.PersistKeyInCsp = false; // Loaded key

						if(!rsa.VerifyData(pbMsg, sha, pbSig))
						{
							Debug.Assert(false);
							return false;
						}

						rsa.PersistKeyInCsp = false;
					}
				}
			}
			catch(Exception) { Debug.Assert(false); return false; }

			return true;
		}

		public static void SetFileSigKey(string strUrl, string strKey)
		{
			if(string.IsNullOrEmpty(strUrl)) { Debug.Assert(false); return; }
			if(string.IsNullOrEmpty(strKey)) { Debug.Assert(false); return; }

			g_dFileSigKeys[strUrl.ToLowerInvariant()] = strKey;
		}

		public static void EnsureConfigured(Form fParent)
		{
			SetFileSigKey(PwDefs.VersionUrl, AppDefs.Rsa4096PublicKeyXml);

			if(Program.Config.Application.Start.CheckForUpdateConfigured) return;

			// If the user has manually enabled the automatic update check
			// before, there's no need to ask him again
			if(!Program.Config.Application.Start.CheckForUpdate &&
				!Program.IsDevelopmentSnapshot())
			{
				string strHdr = KPRes.UpdateCheckInfo;
				string strSub = KPRes.UpdateCheckInfoRes + MessageService.NewParagraph +
					KPRes.UpdateCheckInfoPriv;

				VistaTaskDialog dlg = new VistaTaskDialog();
				dlg.CommandLinks = true;
				dlg.Content = strHdr;
				dlg.MainInstruction = KPRes.UpdateCheckEnableQ;
				dlg.WindowTitle = PwDefs.ShortProductName;
				dlg.AddButton((int)DialogResult.Yes, KPRes.Enable +
					" (" + KPRes.Recommended + ")", null);
				dlg.AddButton((int)DialogResult.No, KPRes.Disable, null);
				dlg.SetIcon(VtdCustomIcon.Question);
				dlg.FooterText = strSub;
				dlg.SetFooterIcon(VtdIcon.Information);

				int iResult;
				if(dlg.ShowDialog(fParent)) iResult = dlg.Result;
				else
				{
					string strMain = strHdr + MessageService.NewParagraph + strSub;
					iResult = (MessageService.AskYesNo(strMain + MessageService.NewParagraph +
						KPRes.UpdateCheckEnableQ, PwDefs.ShortProductName) ?
						(int)DialogResult.Yes : (int)DialogResult.No);
				}

				Program.Config.Application.Start.CheckForUpdate = ((iResult ==
					(int)DialogResult.OK) || (iResult == (int)DialogResult.Yes));
			}

			Program.Config.Application.Start.CheckForUpdateConfigured = true;
		}
	}
}
