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
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Threading;

using KeePass.App;
using KeePass.DataExchange.Formats;
using KeePass.Forms;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Resources;
using KeePassLib.Security;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.DataExchange
{
	public static class ImportUtil
	{
		public static bool? Import(PwDatabase pwStorage, out bool bAppendedToRootOnly,
			Form fParent)
		{
			bAppendedToRootOnly = false;

			if(pwStorage == null) throw new ArgumentNullException("pwStorage");
			if(!pwStorage.IsOpen) return null;
			if(!AppPolicy.Try(AppPolicyId.Import)) return null;

			ExchangeDataForm dlgFmt = new ExchangeDataForm();
			dlgFmt.InitEx(false, pwStorage, pwStorage.RootGroup);

			if(UIUtil.ShowDialogNotValue(dlgFmt, DialogResult.OK)) return null;

			Debug.Assert(dlgFmt.ResultFormat != null);
			if(dlgFmt.ResultFormat == null)
			{
				MessageService.ShowWarning(KPRes.ImportFailed);
				UIUtil.DestroyForm(dlgFmt);
				return false;
			}

			bAppendedToRootOnly = dlgFmt.ResultFormat.ImportAppendsToRootGroupOnly;
			FileFormatProvider ffp = dlgFmt.ResultFormat;

			List<IOConnectionInfo> lConnections = new List<IOConnectionInfo>();
			foreach(string strSelFile in dlgFmt.ResultFiles)
				lConnections.Add(IOConnectionInfo.FromPath(strSelFile));

			UIUtil.DestroyForm(dlgFmt);
			return Import(pwStorage, ffp, lConnections.ToArray(),
				false, null, false, fParent);
		}

		public static bool? Import(PwDatabase pwDatabase, FileFormatProvider fmtImp,
			IOConnectionInfo[] vConnections, bool bSynchronize, IUIOperations uiOps,
			bool bForceSave, Form fParent)
		{
			if(pwDatabase == null) throw new ArgumentNullException("pwDatabase");
			if(!pwDatabase.IsOpen) return null;
			if(fmtImp == null) throw new ArgumentNullException("fmtImp");
			if(vConnections == null) throw new ArgumentNullException("vConnections");

			if(!AppPolicy.Try(AppPolicyId.Import)) return false;
			if(!fmtImp.TryBeginImport()) return false;

			bool bUseTempDb = (fmtImp.SupportsUuids || fmtImp.RequiresKey);
			bool bAllSuccess = true;

			// if(bSynchronize) { Debug.Assert(vFiles.Length == 1); }

			IStatusLogger dlgStatus;
			if(Program.Config.UI.ShowImportStatusDialog)
				dlgStatus = new OnDemandStatusDialog(false, fParent);
			else dlgStatus = new UIBlockerStatusLogger(fParent);

			dlgStatus.StartLogging(PwDefs.ShortProductName + " - " + (bSynchronize ?
				KPRes.Synchronizing : KPRes.ImportingStatusMsg), false);
			dlgStatus.SetText(bSynchronize ? KPRes.Synchronizing :
				KPRes.ImportingStatusMsg, LogStatusType.Info);

			if(vConnections.Length == 0)
			{
				try { fmtImp.Import(pwDatabase, null, dlgStatus); }
				catch(Exception exSingular)
				{
					if((exSingular.Message != null) && (exSingular.Message.Length > 0))
					{
						// slf.SetText(exSingular.Message, LogStatusType.Warning);
						MessageService.ShowWarning(exSingular);
					}
				}

				dlgStatus.EndLogging();
				return true;
			}

			foreach(IOConnectionInfo iocIn in vConnections)
			{
				Stream s = null;

				try { s = IOConnection.OpenRead(iocIn); }
				catch(Exception exFile)
				{
					MessageService.ShowWarning(iocIn.GetDisplayName(), exFile);
					bAllSuccess = false;
					continue;
				}
				if(s == null) { Debug.Assert(false); bAllSuccess = false; continue; }

				PwDatabase pwImp;
				if(bUseTempDb)
				{
					pwImp = new PwDatabase();
					pwImp.New(new IOConnectionInfo(), pwDatabase.MasterKey);
					pwImp.MemoryProtection = pwDatabase.MemoryProtection.CloneDeep();
				}
				else pwImp = pwDatabase;

				if(fmtImp.RequiresKey && !bSynchronize)
				{
					KeyPromptForm kpf = new KeyPromptForm();
					kpf.InitEx(iocIn, false, true);

					if(UIUtil.ShowDialogNotValue(kpf, DialogResult.OK)) { s.Close(); continue; }

					pwImp.MasterKey = kpf.CompositeKey;
					UIUtil.DestroyForm(kpf);
				}
				else if(bSynchronize) pwImp.MasterKey = pwDatabase.MasterKey;

				dlgStatus.SetText((bSynchronize ? KPRes.Synchronizing :
					KPRes.ImportingStatusMsg) + " (" + iocIn.GetDisplayName() +
					")", LogStatusType.Info);

				try { fmtImp.Import(pwImp, s, dlgStatus); }
				catch(Exception excpFmt)
				{
					string strMsgEx = excpFmt.Message;
					if(bSynchronize && (excpFmt is InvalidCompositeKeyException))
						strMsgEx = KLRes.InvalidCompositeKey + MessageService.NewParagraph +
							KPRes.SynchronizingHint;

					MessageService.ShowWarning(strMsgEx);

					s.Close();
					bAllSuccess = false;
					continue;
				}

				s.Close();

				if(bUseTempDb)
				{
					PwMergeMethod mm;
					if(!fmtImp.SupportsUuids) mm = PwMergeMethod.CreateNewUuids;
					else if(bSynchronize) mm = PwMergeMethod.Synchronize;
					else
					{
						ImportMethodForm imf = new ImportMethodForm();
						if(UIUtil.ShowDialogNotValue(imf, DialogResult.OK)) continue;
						mm = imf.MergeMethod;
						UIUtil.DestroyForm(imf);
					}

					try { pwDatabase.MergeIn(pwImp, mm, dlgStatus); }
					catch(Exception exMerge)
					{
						MessageService.ShowWarning(iocIn.GetDisplayName(),
							KPRes.ImportFailed, exMerge);

						bAllSuccess = false;
						continue;
					}
				}
			}

			if(bSynchronize && bAllSuccess)
			{
				Debug.Assert(uiOps != null);
				if(uiOps == null) throw new ArgumentNullException("uiOps");

				dlgStatus.SetText(KPRes.Synchronizing + " (" +
					KPRes.SavingDatabase + ")", LogStatusType.Info);

				MainForm mf = Program.MainForm; // Null for KPScript
				if(mf != null)
				{
					try { mf.DocumentManager.ActiveDatabase = pwDatabase; }
					catch(Exception) { Debug.Assert(false); }
				}

				if(uiOps.UIFileSave(bForceSave))
				{
					foreach(IOConnectionInfo ioc in vConnections)
					{
						try
						{
							// dlgStatus.SetText(KPRes.Synchronizing + " (" +
							//	KPRes.SavingDatabase + " " + ioc.GetDisplayName() +
							//	")", LogStatusType.Info);

							string strSource = pwDatabase.IOConnectionInfo.Path;
							if(ioc.Path != strSource)
							{
								bool bSaveAs = true;

								if(pwDatabase.IOConnectionInfo.IsLocalFile() &&
									ioc.IsLocalFile())
								{
									// Do not try to copy an encrypted file;
									// https://sourceforge.net/p/keepass/discussion/329220/thread/9c9eb989/
									// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363851.aspx
									if((long)(File.GetAttributes(strSource) &
										FileAttributes.Encrypted) == 0)
									{
										File.Copy(strSource, ioc.Path, true);
										bSaveAs = false;
									}
								}

								if(bSaveAs) pwDatabase.SaveAs(ioc, false, null);
							}
							// else { } // No assert (sync on save)

							if(mf != null)
								mf.FileMruList.AddItem(ioc.GetDisplayName(),
									ioc.CloneDeep());
						}
						catch(Exception exSync)
						{
							MessageService.ShowWarning(KPRes.SyncFailed,
								pwDatabase.IOConnectionInfo.GetDisplayName() +
								MessageService.NewLine + ioc.GetDisplayName(), exSync);

							bAllSuccess = false;
							continue;
						}
					}
				}
				else
				{
					MessageService.ShowWarning(KPRes.SyncFailed,
						pwDatabase.IOConnectionInfo.GetDisplayName());

					bAllSuccess = false;
				}
			}

			dlgStatus.EndLogging();
			return bAllSuccess;
		}

		public static bool? Import(PwDatabase pd, FileFormatProvider fmtImp,
			IOConnectionInfo iocImp, PwMergeMethod mm, CompositeKey cmpKey)
		{
			if(pd == null) { Debug.Assert(false); return false; }
			if(fmtImp == null) { Debug.Assert(false); return false; }
			if(iocImp == null) { Debug.Assert(false); return false; }
			if(cmpKey == null) cmpKey = new CompositeKey();

			if(!AppPolicy.Try(AppPolicyId.Import)) return false;
			if(!fmtImp.TryBeginImport()) return false;

			PwDatabase pdImp = new PwDatabase();
			pdImp.New(new IOConnectionInfo(), cmpKey);
			pdImp.MemoryProtection = pd.MemoryProtection.CloneDeep();

			Stream s = IOConnection.OpenRead(iocImp);
			if(s == null)
				throw new FileNotFoundException(iocImp.GetDisplayName() +
					MessageService.NewLine + KPRes.FileNotFoundError);

			try { fmtImp.Import(pdImp, s, null); }
			finally { s.Close(); }

			pd.MergeIn(pdImp, mm);
			return true;
		}

		public static bool? Synchronize(PwDatabase pwStorage, IUIOperations uiOps,
			bool bOpenFromUrl, Form fParent)
		{
			if(pwStorage == null) throw new ArgumentNullException("pwStorage");
			if(!pwStorage.IsOpen) return null;
			if(!AppPolicy.Try(AppPolicyId.Import)) return null;

			List<IOConnectionInfo> vConnections = new List<IOConnectionInfo>();
			if(bOpenFromUrl == false)
			{
				OpenFileDialogEx ofd = UIUtil.CreateOpenFileDialog(KPRes.Synchronize,
					UIUtil.CreateFileTypeFilter(AppDefs.FileExtension.FileExt,
					KPRes.KdbxFiles, true), 1, null, true,
					AppDefs.FileDialogContext.Sync);

				if(ofd.ShowDialog() != DialogResult.OK) return null;

				foreach(string strSelFile in ofd.FileNames)
					vConnections.Add(IOConnectionInfo.FromPath(strSelFile));
			}
			else // Open URL
			{
				IOConnectionForm iocf = new IOConnectionForm();
				iocf.InitEx(false, null, true, true);

				if(UIUtil.ShowDialogNotValue(iocf, DialogResult.OK)) return null;

				vConnections.Add(iocf.IOConnectionInfo);
				UIUtil.DestroyForm(iocf);
			}

			return Import(pwStorage, new KeePassKdb2x(), vConnections.ToArray(),
				true, uiOps, false, fParent);
		}

		public static bool? Synchronize(PwDatabase pwStorage, IUIOperations uiOps,
			IOConnectionInfo iocSyncWith, bool bForceSave, Form fParent)
		{
			if(pwStorage == null) throw new ArgumentNullException("pwStorage");
			if(!pwStorage.IsOpen) return null; // No assert or throw
			if(iocSyncWith == null) throw new ArgumentNullException("iocSyncWith");
			if(!AppPolicy.Try(AppPolicyId.Import)) return null;

			List<IOConnectionInfo> vConnections = new List<IOConnectionInfo>();
			vConnections.Add(iocSyncWith);

			return Import(pwStorage, new KeePassKdb2x(), vConnections.ToArray(),
				true, uiOps, bForceSave, fParent);
		}

		public static int CountQuotes(string str, int posMax)
		{
			int i = 0, n = 0;

			while(true)
			{
				i = str.IndexOf('\"', i);
				if(i < 0) return n;

				++i;
				if(i > posMax) return n;

				++n;
			}
		}

		public static List<string> SplitCsvLine(string strLine, string strDelimiter)
		{
			List<string> list = new List<string>();

			int nOffset = 0;
			while(true)
			{
				int i = strLine.IndexOf(strDelimiter, nOffset);
				if(i < 0) break;

				int nQuotes = CountQuotes(strLine, i);
				if((nQuotes & 1) == 0)
				{
					list.Add(strLine.Substring(0, i));
					strLine = strLine.Remove(0, i + strDelimiter.Length);
					nOffset = 0;
				}
				else
				{
					nOffset = i + strDelimiter.Length;
					if(nOffset >= strLine.Length) break;
				}
			}

			list.Add(strLine);
			return list;
		}

		public static bool SetStatus(IStatusLogger slLogger, uint uPercent)
		{
			if(slLogger != null) return slLogger.SetProgress(uPercent);
			return true;
		}

		private static readonly string[] m_vTitles = {
			"title", "system", "account", "entry",
			"item", "itemname", "item name", "subject",
			"service", "servicename", "service name",
			"head", "heading", "card", "product", "provider", "bank",
			"type",

			// Non-English names
			"seite"
		};

		private static readonly string[] m_vUserNames = {
			"user", "name", "user name", "username", "login name",
			"email", "e-mail", "id", "userid", "user id",
			"login", "form_loginname", "wpname", "mail",
			"loginid", "login id", "log",
			"first name", "last name", "card#", "account #",
			"member", "member #",

			// Non-English names
			"nom", "benutzername"
		};

		private static readonly string[] m_vPasswords = {
			"password", "pass word", "passphrase", "pass phrase",
			"pass", "code", "code word", "codeword",
			"secret", "secret word",
			"key", "keyword", "key word", "keyphrase", "key phrase",
			"form_pw", "wppassword", "pin", "pwd", "pw", "pword",
			"p", "serial", "serial#", "license key", "reg #",

			// Non-English names
			"passwort"
		};

		private static readonly string[] m_vUrls = {
			"url", "hyper link", "hyperlink", "link",
			"host", "hostname", "host name", "server", "address",
			"hyper ref", "href", "web", "website", "web site", "site",
			"web-site",

			// Non-English names
			"ort", "adresse"
		};

		private static readonly string[] m_vNotes = {
			"note", "notes", "comment", "comments", "memo",
			"description", "free form", "freeform",
			"free text", "freetext", "free",

			// Non-English names
			"kommentar"
		};

		private static readonly string[] m_vSubstrTitles = {
			"title", "system", "account", "entry",
			"item", "subject", "service", "head"
		};

		private static readonly string[] m_vSubstrUserNames = {
			"user", "name", "id", "login", "mail"
		};

		private static readonly string[] m_vSubstrPasswords = {
			"pass", "code",	"secret", "key", "pw", "pin"
		};

		private static readonly string[] m_vSubstrUrls = {
			"url", "link", "host", "address", "hyper ref", "href",
			"web", "site"
		};

		private static readonly string[] m_vSubstrNotes = { 
			"note", "comment", "memo", "description", "free"
		};

		public static string MapNameToStandardField(string strName, bool bAllowFuzzy)
		{
			Debug.Assert(strName != null);
			if(strName == null) throw new ArgumentNullException("strName");

			string strFind = strName.ToLower();

			if(Array.IndexOf<string>(m_vTitles, strFind) >= 0)
				return PwDefs.TitleField;
			if(Array.IndexOf<string>(m_vUserNames, strFind) >= 0)
				return PwDefs.UserNameField;
			if(Array.IndexOf<string>(m_vPasswords, strFind) >= 0)
				return PwDefs.PasswordField;
			if(Array.IndexOf<string>(m_vUrls, strFind) >= 0)
				return PwDefs.UrlField;
			if(Array.IndexOf<string>(m_vNotes, strFind) >= 0)
				return PwDefs.NotesField;

			return (bAllowFuzzy ? MapNameSubstringToStandardField(strName) : string.Empty);
		}

		private static string MapNameSubstringToStandardField(string strName)
		{
			Debug.Assert(strName != null);
			if(strName == null) throw new ArgumentNullException("strName");

			string strFind = strName.ToLower();

			// Check for passwords first, then user names ('vb_login_password')
			foreach(string strPassword in m_vSubstrPasswords)
			{
				if(strFind.Contains(strPassword))
					return PwDefs.PasswordField;
			}
			foreach(string strUserName in m_vSubstrUserNames)
			{
				if(strFind.Contains(strUserName))
					return PwDefs.UserNameField;
			}
			foreach(string strTitle in m_vSubstrTitles)
			{
				if(strFind.Contains(strTitle))
					return PwDefs.TitleField;
			}
			foreach(string strUrl in m_vSubstrUrls)
			{
				if(strFind.Contains(strUrl))
					return PwDefs.UrlField;
			}
			foreach(string strNotes in m_vSubstrNotes)
			{
				if(strFind.Contains(strNotes))
					return PwDefs.NotesField;
			}

			return string.Empty;
		}

		public static void AppendToField(PwEntry pe, string strName, string strValue,
			PwDatabase pdContext)
		{
			AppendToField(pe, strName, strValue, pdContext, null, false);
		}

		public static void AppendToField(PwEntry pe, string strName, string strValue,
			PwDatabase pdContext, string strSeparator, bool bOnlyIfNotDup)
		{
			// Default separator must be single-line compatible
			if(strSeparator == null) strSeparator = ", ";

			bool bProtect = ((pdContext == null) ? false :
				pdContext.MemoryProtection.GetProtection(strName));

			ProtectedString ps = pe.Strings.Get(strName);
			string strPrev = ((ps != null) ? ps.ReadString() : null);
			if(ps != null) bProtect = ps.IsProtected;

			strValue = (strValue ?? string.Empty);
			if(string.IsNullOrEmpty(strPrev))
				pe.Strings.Set(strName, new ProtectedString(bProtect, strValue));
			else if(strValue.Length > 0)
			{
				bool bAppend = true;

				if(bOnlyIfNotDup)
					bAppend &= (strPrev != strValue);

				if(bAppend)
					pe.Strings.Set(strName, new ProtectedString(bProtect,
						strPrev + strSeparator + strValue));
			}
		}

		public static bool EntryEquals(PwEntry pe1, PwEntry pe2)
		{
			if(pe1.ParentGroup == null) return false;
			if(pe2.ParentGroup == null) return false;

			if(pe1.ParentGroup.Name != pe2.ParentGroup.Name)
				return false;

			if(pe1.Strings.ReadSafe(PwDefs.TitleField) !=
				pe2.Strings.ReadSafe(PwDefs.TitleField))
			{
				return false;
			}

			if(pe1.Strings.ReadSafe(PwDefs.UserNameField) !=
				pe2.Strings.ReadSafe(PwDefs.UserNameField))
			{
				return false;
			}

			if(pe1.Strings.ReadSafe(PwDefs.PasswordField) !=
				pe2.Strings.ReadSafe(PwDefs.PasswordField))
			{
				return false;
			}

			if(pe1.Strings.ReadSafe(PwDefs.UrlField) !=
				pe2.Strings.ReadSafe(PwDefs.UrlField))
			{
				return false;
			}

			if(pe1.Strings.ReadSafe(PwDefs.NotesField) !=
				pe2.Strings.ReadSafe(PwDefs.NotesField))
			{
				return false;
			}

			return true;
		}

		internal static string GuiSendRetrieve(string strSendPrefix)
		{
			if(strSendPrefix.Length > 0)
				GuiSendKeysPrc(strSendPrefix);

			return GuiRetrieveDataField();
		}

		private static string GuiRetrieveDataField()
		{
			ClipboardUtil.Clear();
			Application.DoEvents();

			GuiSendKeysPrc(@"^c");

			try
			{
				if(ClipboardUtil.ContainsText()) return ClipboardUtil.GetText();
			}
			catch(Exception) { Debug.Assert(false); } // Opened by other process

			return string.Empty;
		}

		internal static void GuiSendKeysPrc(string strSend)
		{
			if(strSend.Length > 0)
				SendInputEx.SendKeysWait(strSend, false);

			Application.DoEvents();
			Thread.Sleep(100);
			Application.DoEvents();
		}
		
		internal static void GuiSendWaitWindowChange(string strSend)
		{
			IntPtr ptrCur = NativeMethods.GetForegroundWindowHandle();

			ImportUtil.GuiSendKeysPrc(strSend);

			int nRound = 0;
			while(true)
			{
				Application.DoEvents();

				IntPtr ptr = NativeMethods.GetForegroundWindowHandle();
				if(ptr != ptrCur) break;

				++nRound;
				if(nRound > 1000)
					throw new InvalidOperationException();

				Thread.Sleep(50);
			}

			Thread.Sleep(100);
			Application.DoEvents();
		}

		internal static string FixUrl(string strUrl)
		{
			strUrl = strUrl.Trim();

			if((strUrl.Length > 0) && (strUrl.IndexOf(':') < 0) &&
				(strUrl.IndexOf('@') < 0))
			{
				string strNew = ("http://" + strUrl.ToLower());
				if(strUrl.IndexOf('/') < 0) strNew += "/";
				return strNew;
			}

			return strUrl;
		}
	}
}
