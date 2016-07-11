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
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using KeePass.App;
using KeePass.DataExchange;
using KeePass.Forms;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Delegates;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.Ecas
{
	internal sealed class EcasDefaultActionProvider : EcasActionProvider
	{
		private const uint IdTriggerOff = 0;
		private const uint IdTriggerOn = 1;
		private const uint IdTriggerToggle = 2;

		private const uint IdMbcY = 0;
		private const uint IdMbcN = 1;

		private const uint IdMbaNone = 0;
		private const uint IdMbaAbort = 1;
		private const uint IdMbaCmd = 2;

		public EcasDefaultActionProvider()
		{
			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0xDA, 0xE5, 0xF8, 0x3B, 0x07, 0x30, 0x4C, 0x13,
				0x9E, 0xEF, 0x2E, 0xBA, 0xCB, 0x6E, 0xE4, 0xC7 }),
				KPRes.ExecuteCmdLineUrl, PwIcon.Console, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null),
					new EcasParameter(KPRes.Arguments, EcasValueType.String, null),
					new EcasParameter(KPRes.WaitForExit, EcasValueType.Bool, null) },
				ExecuteShellCmd));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0xB6, 0x46, 0xA6, 0x9F, 0xDE, 0x94, 0x4B, 0xB9,
				0x9B, 0xAE, 0x3C, 0xA4, 0x7E, 0xCC, 0x10, 0xEA }),
				KPRes.TriggerStateChange, PwIcon.Run, new EcasParameter[] {
					new EcasParameter(KPRes.TriggerName, EcasValueType.String, null),
					new EcasParameter(KPRes.NewState, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem(IdTriggerOn, KPRes.On),
							new EcasEnumItem(IdTriggerOff, KPRes.Off),
							new EcasEnumItem(IdTriggerToggle, KPRes.Toggle) })) },
				ChangeTriggerOnOff));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0xFD, 0x41, 0x55, 0xD5, 0x79, 0x8F, 0x44, 0xFA,
				0xAB, 0x89, 0xF2, 0xF8, 0x70, 0xEF, 0x94, 0xB8 }),
				KPRes.OpenDatabaseFileStc, PwIcon.FolderOpen, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null),
					new EcasParameter(KPRes.IOConnection + " - " + KPRes.UserName,
						EcasValueType.String, null),
					new EcasParameter(KPRes.IOConnection + " - " + KPRes.Password,
						EcasValueType.String, null),
					new EcasParameter(KPRes.Password, EcasValueType.String, null),
					new EcasParameter(KPRes.KeyFile, EcasValueType.String, null),
					new EcasParameter(KPRes.UserAccount, EcasValueType.Bool, null) },
				OpenDatabaseFile));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0xF5, 0x57, 0x61, 0x4B, 0xF8, 0x4C, 0x41, 0x5D,
				0xA9, 0x13, 0x7A, 0x39, 0xCD, 0x10, 0xF0, 0xBD }),
				KPRes.SaveDatabaseStc, PwIcon.Disk, null,
				SaveDatabaseFile));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x22, 0xAD, 0x77, 0xE4, 0x17, 0x78, 0x4E, 0xED,
				0x99, 0xB4, 0x57, 0x1D, 0x02, 0xB3, 0xAD, 0x4D }),
				KPRes.SynchronizeStc, PwIcon.PaperReady, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null),
					new EcasParameter(KPRes.IOConnection + " - " + KPRes.UserName,
						EcasValueType.String, null),
					new EcasParameter(KPRes.IOConnection + " - " + KPRes.Password,
						EcasValueType.String, null) },
				SyncDatabaseFile));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x80, 0xE6, 0x7F, 0x4E, 0x72, 0xF1, 0x40, 0x45,
				0x91, 0x76, 0x1F, 0x2C, 0x23, 0xD8, 0xEC, 0xBE }),
				KPRes.ImportStc, PwIcon.PaperReady, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null),
					new EcasParameter(KPRes.FileFormatStc, EcasValueType.String, null),
					new EcasParameter(KPRes.Method, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem((uint)PwMergeMethod.None, KPRes.Default),
							new EcasEnumItem((uint)PwMergeMethod.CreateNewUuids,
								StrUtil.RemoveAccelerator(KPRes.CreateNewIDs)),
							new EcasEnumItem((uint)PwMergeMethod.KeepExisting,
								StrUtil.RemoveAccelerator(KPRes.KeepExisting)),
							new EcasEnumItem((uint)PwMergeMethod.OverwriteExisting,
								StrUtil.RemoveAccelerator(KPRes.OverwriteExisting)),
							new EcasEnumItem((uint)PwMergeMethod.OverwriteIfNewer,
								StrUtil.RemoveAccelerator(KPRes.OverwriteIfNewer)),
							new EcasEnumItem((uint)PwMergeMethod.Synchronize,
								StrUtil.RemoveAccelerator(KPRes.OverwriteIfNewerAndApplyDel)) })),
					new EcasParameter(KPRes.Password, EcasValueType.String, null),
					new EcasParameter(KPRes.KeyFile, EcasValueType.String, null),
					new EcasParameter(KPRes.UserAccount, EcasValueType.Bool, null) },
				ImportIntoCurrentDatabase));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x0F, 0x9A, 0x6B, 0x5B, 0xCE, 0xD5, 0x46, 0xBE,
				0xB9, 0x34, 0xED, 0xB1, 0x3F, 0x94, 0x48, 0x22 }),
				KPRes.ExportStc, PwIcon.Disk, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null),
					new EcasParameter(KPRes.FileFormatStc, EcasValueType.String, null),
					new EcasParameter(KPRes.Filter + " - " + KPRes.Group, EcasValueType.String, null),
					new EcasParameter(KPRes.Filter + " - " + KPRes.Tag, EcasValueType.String, null) },
				ExportDatabaseFile));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x5B, 0xBF, 0x45, 0x9D, 0x54, 0xBF, 0x49, 0xBD,
				0x97, 0xFB, 0x2C, 0xEE, 0x5F, 0x99, 0x0A, 0x67 }),
				KPRes.CloseActiveDatabase, PwIcon.PaperReady, null,
				CloseDatabaseFile));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x3F, 0xB8, 0x33, 0x2D, 0xD6, 0x16, 0x4E, 0x87,
				0x99, 0x05, 0x64, 0xDB, 0x16, 0x4C, 0xD6, 0x26 }),
				KPRes.ActivateDatabaseTab, PwIcon.List, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null),
					new EcasParameter(KPRes.Filter, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem(0, KPRes.All),
							new EcasEnumItem(1, KPRes.Triggering) })) },
				ActivateDatabaseTab));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x3B, 0x3D, 0x3E, 0x31, 0xE4, 0xB3, 0x42, 0xA6,
				0xBA, 0xCC, 0xD5, 0xC0, 0x3B, 0xAC, 0xA9, 0x69 }),
				KPRes.Wait, PwIcon.Clock, new EcasParameter[] {
					new EcasParameter(KPRes.TimeSpan + @" [ms]", EcasValueType.UInt64, null) },
				ExecuteSleep));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x09, 0xF7, 0x8F, 0x73, 0x24, 0xEC, 0x4F, 0xEC,
				0x88, 0xB6, 0x25, 0xD5, 0x30, 0xF4, 0x34, 0x6E }),
				KPRes.ShowMessageBox, PwIcon.UserCommunication, new EcasParameter[] {
					new EcasParameter(KPRes.MainInstruction, EcasValueType.String, null),
					new EcasParameter(KPRes.Text, EcasValueType.String, null),
					new EcasParameter(KPRes.Icon, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem((uint)MessageBoxIcon.None, KPRes.None),
							new EcasEnumItem((uint)MessageBoxIcon.Information, "i"),
							new EcasEnumItem((uint)MessageBoxIcon.Question, "?"),
							new EcasEnumItem((uint)MessageBoxIcon.Warning, KPRes.Warning),
							new EcasEnumItem((uint)MessageBoxIcon.Error, KPRes.Error) })),
					new EcasParameter(KPRes.Buttons, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem((uint)MessageBoxButtons.OK,
								KPRes.Ok),
							new EcasEnumItem((uint)MessageBoxButtons.OKCancel,
								KPRes.Ok + "/" + KPRes.Cancel),
							new EcasEnumItem((uint)MessageBoxButtons.YesNo,
								KPRes.Yes + "/" + KPRes.No) })),
					new EcasParameter(KPRes.ButtonDefault, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem(0, KPRes.Button + " 1"),
							new EcasEnumItem(1, KPRes.Button + " 2") })),
					new EcasParameter(KPRes.Action + " - " + KPRes.Condition, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem(IdMbcY, KPRes.Button + " " +
								KPRes.Ok + "/" + KPRes.Yes),
							new EcasEnumItem(IdMbcN, KPRes.Button + " " +
								KPRes.Cancel + "/" + KPRes.No) })),
					new EcasParameter(KPRes.Action, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem(IdMbaNone, KPRes.None),
							new EcasEnumItem(IdMbaAbort, KPRes.AbortTrigger),
							new EcasEnumItem(IdMbaCmd, KPRes.ExecuteCmdLineUrl) })),
					new EcasParameter(KPRes.Action + " - " + KPRes.Parameters,
						EcasValueType.String, null) },
				ShowMessageBox));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x40, 0x69, 0xA5, 0x36, 0x57, 0x1B, 0x47, 0x92,
				0xA9, 0xB3, 0x73, 0x65, 0x30, 0xE0, 0xCF, 0xC3 }),
				KPRes.PerformGlobalAutoType, PwIcon.Run, null, ExecuteGlobalAutoType));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x31, 0x70, 0x8F, 0xAD, 0x64, 0x93, 0x43, 0xF5,
				0x94, 0xEE, 0xC8, 0x1A, 0x23, 0x6E, 0x32, 0x4D }),
				KPRes.PerformSelectedAutoType, PwIcon.Run, new EcasParameter[] {
					new EcasParameter(KPRes.Sequence, EcasValueType.String, null) },
				ExecuteSelectedAutoType));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x42, 0xE8, 0x37, 0x81, 0x73, 0xD3, 0x4E, 0xEC,
				0x81, 0x48, 0x9E, 0x3B, 0x36, 0xAC, 0x83, 0x84 }),
				KPRes.ShowEntriesByTag, PwIcon.List, new EcasParameter[] {
					new EcasParameter(KPRes.Tag, EcasValueType.String, null) },
				ShowEntriesByTag));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0x95, 0x81, 0x8F, 0x45, 0x99, 0x66, 0x49, 0x88,
				0xAB, 0x3E, 0x86, 0xE8, 0x1A, 0x96, 0x68, 0x36 }),
				KPRes.CustomTbButtonAdd, PwIcon.Screen, new EcasParameter[] {
					new EcasParameter(KPRes.Id, EcasValueType.String, null),
					new EcasParameter(KPRes.Name, EcasValueType.String, null),
					new EcasParameter(KPRes.Description, EcasValueType.String, null) },
				AddToolBarButton));

			m_actions.Add(new EcasActionType(new PwUuid(new byte[] {
				0xD6, 0x6D, 0x41, 0xA2, 0x6C, 0xB2, 0x44, 0xBA,
				0xA4, 0x48, 0x0A, 0x41, 0xFA, 0x09, 0x48, 0x79 }),
				KPRes.CustomTbButtonRemove, PwIcon.Screen, new EcasParameter[] {
					new EcasParameter(KPRes.Id, EcasValueType.String, null) },
				RemoveToolBarButton));
		}

		private static void ExecuteShellCmd(EcasAction a, EcasContext ctx)
		{
			string strCmd = EcasUtil.GetParamString(a.Parameters, 0, true, true);
			string strArgs = EcasUtil.GetParamString(a.Parameters, 1, true, true);
			bool bWait = StrUtil.StringToBool(EcasUtil.GetParamString(a.Parameters,
				2, string.Empty));

			if(string.IsNullOrEmpty(strCmd)) return;

			try
			{
				Process p;
				if(string.IsNullOrEmpty(strArgs)) p = Process.Start(strCmd);
				else p = Process.Start(strCmd, strArgs);

				if((p != null) && bWait)
				{
					Program.MainForm.UIBlockInteraction(true);
					MessageService.ExternalIncrementMessageCount();

					try { p.WaitForExit(); }
					catch(Exception) { Debug.Assert(false); }

					MessageService.ExternalDecrementMessageCount();
					Program.MainForm.UIBlockInteraction(false);
				}
			}
			catch(Exception e)
			{
				throw new Exception(strCmd + MessageService.NewParagraph + e.Message);
			}
		}

		private static void ChangeTriggerOnOff(EcasAction a, EcasContext ctx)
		{
			string strName = EcasUtil.GetParamString(a.Parameters, 0, true);
			uint uState = EcasUtil.GetParamUInt(a.Parameters, 1);

			EcasTrigger t = null;
			if(strName.Length == 0) t = ctx.Trigger;
			else
			{
				foreach(EcasTrigger trg in ctx.TriggerSystem.TriggerCollection)
				{
					if(trg.Name == strName) { t = trg; break; }
				}
			}

			if(t == null) throw new Exception(KPRes.ObjectNotFound +
				MessageService.NewParagraph + KPRes.TriggerName + ": " + strName + ".");

			if(uState == IdTriggerOn) t.On = true;
			else if(uState == IdTriggerOff) t.On = false;
			else if(uState == IdTriggerToggle) t.On = !t.On;
			else { Debug.Assert(false); }
		}

		private static void OpenDatabaseFile(EcasAction a, EcasContext ctx)
		{
			string strPath = EcasUtil.GetParamString(a.Parameters, 0, true);
			if(string.IsNullOrEmpty(strPath)) return;

			string strIOUserName = EcasUtil.GetParamString(a.Parameters, 1, true);
			string strIOPassword = EcasUtil.GetParamString(a.Parameters, 2, true);

			IOConnectionInfo ioc = IOFromParameters(strPath, strIOUserName, strIOPassword);
			if(ioc == null) return;

			CompositeKey cmpKey = KeyFromParams(a, 3, 4, 5);

			Program.MainForm.OpenDatabase(ioc, cmpKey, ioc.IsLocalFile());
		}

		private static CompositeKey KeyFromParams(EcasAction a, int iPassword,
			int iKeyFile, int iUserAccount)
		{
			string strPassword = EcasUtil.GetParamString(a.Parameters, iPassword, true);
			string strKeyFile = EcasUtil.GetParamString(a.Parameters, iKeyFile, true);
			bool bUserAccount = StrUtil.StringToBool(EcasUtil.GetParamString(
				a.Parameters, iUserAccount, true));

			CompositeKey cmpKey = null;
			if(!string.IsNullOrEmpty(strPassword) || !string.IsNullOrEmpty(strKeyFile) ||
				bUserAccount)
			{
				List<string> vArgs = new List<string>();
				if(!string.IsNullOrEmpty(strPassword))
					vArgs.Add("-" + AppDefs.CommandLineOptions.Password + ":" + strPassword);
				if(!string.IsNullOrEmpty(strKeyFile))
					vArgs.Add("-" + AppDefs.CommandLineOptions.KeyFile + ":" + strKeyFile);
				if(bUserAccount)
					vArgs.Add("-" + AppDefs.CommandLineOptions.UserAccount);

				CommandLineArgs cmdArgs = new CommandLineArgs(vArgs.ToArray());
				cmpKey = KeyUtil.KeyFromCommandLine(cmdArgs);
			}

			return cmpKey;
		}

		private static void SaveDatabaseFile(EcasAction a, EcasContext ctx)
		{
			Program.MainForm.UIFileSave(false);
		}

		private static void SyncDatabaseFile(EcasAction a, EcasContext ctx)
		{
			string strPath = EcasUtil.GetParamString(a.Parameters, 0, true);
			if(string.IsNullOrEmpty(strPath)) return;

			string strIOUserName = EcasUtil.GetParamString(a.Parameters, 1, true);
			string strIOPassword = EcasUtil.GetParamString(a.Parameters, 2, true);

			IOConnectionInfo ioc = IOFromParameters(strPath, strIOUserName, strIOPassword);
			if(ioc == null) return;

			PwDatabase pd = Program.MainForm.ActiveDatabase;
			if((pd == null) || !pd.IsOpen) return;

			bool? b = ImportUtil.Synchronize(pd, Program.MainForm, ioc, false,
				Program.MainForm);
			Program.MainForm.UpdateUI(false, null, true, null, true, null, false);
			if(b.HasValue) Program.MainForm.SetStatusEx(b.Value ? KPRes.SyncSuccess : KPRes.SyncFailed);
		}

		private static IOConnectionInfo IOFromParameters(string strPath,
			string strUser, string strPassword)
		{
			IOConnectionInfo ioc = IOConnectionInfo.FromPath(strPath);

			// Set the user name, which acts as a filter for the MRU items
			if(!string.IsNullOrEmpty(strUser)) ioc.UserName = strUser;

			// Try to complete it using the MRU list; this will especially
			// retrieve the CredSaveMode of the MRU item (if one exists)
			ioc = Program.MainForm.CompleteConnectionInfoUsingMru(ioc);

			// Override the password using the trigger value; do not change
			// the CredSaveMode anymore (otherwise e.g. values retrieved
			// using field references would be stored in the MRU list)
			if(!string.IsNullOrEmpty(strPassword)) ioc.Password = strPassword;

			if(ioc.Password.Length > 0) ioc.IsComplete = true;

			return MainForm.CompleteConnectionInfo(ioc, false, true, true, false);
		}

		private static void ImportIntoCurrentDatabase(EcasAction a, EcasContext ctx)
		{
			PwDatabase pd = Program.MainForm.ActiveDatabase;
			if((pd == null) || !pd.IsOpen) return;

			string strPath = EcasUtil.GetParamString(a.Parameters, 0, true);
			if(string.IsNullOrEmpty(strPath)) return;
			IOConnectionInfo ioc = IOConnectionInfo.FromPath(strPath);

			string strFormat = EcasUtil.GetParamString(a.Parameters, 1, true);
			if(string.IsNullOrEmpty(strFormat)) return;
			FileFormatProvider ff = Program.FileFormatPool.Find(strFormat);
			if(ff == null)
				throw new Exception(KPRes.Unknown + ": " + strFormat);

			uint uMethod = EcasUtil.GetParamUInt(a.Parameters, 2);
			Type tMM = Enum.GetUnderlyingType(typeof(PwMergeMethod));
			object oMethod = Convert.ChangeType(uMethod, tMM);
			PwMergeMethod mm = PwMergeMethod.None;
			if(Enum.IsDefined(typeof(PwMergeMethod), oMethod))
				mm = (PwMergeMethod)oMethod;
			else { Debug.Assert(false); }
			if(mm == PwMergeMethod.None) mm = PwMergeMethod.CreateNewUuids;

			CompositeKey cmpKey = KeyFromParams(a, 3, 4, 5);
			if((cmpKey == null) && ff.RequiresKey)
			{
				KeyPromptForm kpf = new KeyPromptForm();
				kpf.InitEx(ioc, false, true);

				if(UIUtil.ShowDialogNotValue(kpf, DialogResult.OK)) return;

				cmpKey = kpf.CompositeKey;
				UIUtil.DestroyForm(kpf);
			}

			bool? b = true;
			try { b = ImportUtil.Import(pd, ff, ioc, mm, cmpKey); }
			finally
			{
				if(b.GetValueOrDefault(false))
					Program.MainForm.UpdateUI(false, null, true, null, true, null, true);
			}
		}

		private static void ExportDatabaseFile(EcasAction a, EcasContext ctx)
		{
			string strPath = EcasUtil.GetParamString(a.Parameters, 0, true);
			// if(string.IsNullOrEmpty(strPath)) return; // Allow no-file exports
			string strFormat = EcasUtil.GetParamString(a.Parameters, 1, true);
			if(string.IsNullOrEmpty(strFormat)) return;
			string strGroup = EcasUtil.GetParamString(a.Parameters, 2, true);
			string strTag = EcasUtil.GetParamString(a.Parameters, 3, true);

			PwDatabase pd = Program.MainForm.ActiveDatabase;
			if((pd == null) || !pd.IsOpen) return;

			PwGroup pg = pd.RootGroup;
			if(!string.IsNullOrEmpty(strGroup))
			{
				char chSep = strGroup[0];
				PwGroup pgSub = pg.FindCreateSubTree(strGroup.Substring(1),
					new char[] { chSep }, false);
				pg = (pgSub ?? (new PwGroup(true, true, KPRes.Group, PwIcon.Folder)));
			}

			if(!string.IsNullOrEmpty(strTag))
			{
				// Do not use pg.Duplicate, because this method
				// creates new UUIDs
				pg = pg.CloneDeep();
				pg.TakeOwnership(true, true, true);

				GroupHandler gh = delegate(PwGroup pgSub)
				{
					PwObjectList<PwEntry> l = pgSub.Entries;
					long n = (long)l.UCount;
					for(long i = n - 1; i >= 0; --i)
					{
						if(!l.GetAt((uint)i).HasTag(strTag))
							l.RemoveAt((uint)i);
					}

					return true;
				};

				gh(pg);
				pg.TraverseTree(TraversalMethod.PreOrder, gh, null);
			}

			PwExportInfo pei = new PwExportInfo(pg, pd, true);
			IOConnectionInfo ioc = (!string.IsNullOrEmpty(strPath) ?
				IOConnectionInfo.FromPath(strPath) : null);
			ExportUtil.Export(pei, strFormat, ioc);
		}

		private static void CloseDatabaseFile(EcasAction a, EcasContext ctx)
		{
			Program.MainForm.CloseDocument(null, false, false, true);
		}

		private static void ActivateDatabaseTab(EcasAction a, EcasContext ctx)
		{
			string strName = EcasUtil.GetParamString(a.Parameters, 0, true);
			bool bEmptyName = string.IsNullOrEmpty(strName);

			uint uSel = EcasUtil.GetParamUInt(a.Parameters, 1, 0);
			PwDatabase pdSel = ctx.Properties.Get<PwDatabase>(EcasProperty.Database);

			DocumentManagerEx dm = Program.MainForm.DocumentManager;
			foreach(PwDocument doc in dm.Documents)
			{
				if(doc.Database == null) { Debug.Assert(false); continue; }

				if(uSel == 0) // Select from all
				{
					if(bEmptyName) continue; // Name required in this case
				}
				else if(uSel == 1) // Triggering only
				{
					if(!object.ReferenceEquals(doc.Database, pdSel)) continue;
				}
				else { Debug.Assert(false); continue; }

				IOConnectionInfo ioc = null;
				if((doc.LockedIoc != null) && !string.IsNullOrEmpty(doc.LockedIoc.Path))
					ioc = doc.LockedIoc;
				else if((doc.Database.IOConnectionInfo != null) &&
					!string.IsNullOrEmpty(doc.Database.IOConnectionInfo.Path))
					ioc = doc.Database.IOConnectionInfo;

				if(bEmptyName || ((ioc != null) && (ioc.Path.IndexOf(strName,
					StrUtil.CaseIgnoreCmp) >= 0)))
				{
					Program.MainForm.MakeDocumentActive(doc);
					break;
				}
			}
		}

		private static void ExecuteSleep(EcasAction a, EcasContext ctx)
		{
			uint uTimeSpan = EcasUtil.GetParamUInt(a.Parameters, 0);

			if((uTimeSpan != 0) && (uTimeSpan <= (uint)int.MaxValue))
				Thread.Sleep((int)uTimeSpan);
		}

		private static void ExecuteGlobalAutoType(EcasAction a, EcasContext ctx)
		{
			Program.MainForm.ExecuteGlobalAutoType();
		}

		private static void ExecuteSelectedAutoType(EcasAction a, EcasContext ctx)
		{
			try
			{
				// Do not Spr-compile the sequence here; it'll be compiled by
				// the auto-type engine (and this expects an auto-type sequence
				// as input, not a data string; compiling it here would e.g.
				// result in broken '%' characters in passwords)
				string strSeq = EcasUtil.GetParamString(a.Parameters, 0, false);
				if(string.IsNullOrEmpty(strSeq)) strSeq = null;

				PwEntry pe = Program.MainForm.GetSelectedEntry(true);
				if(pe == null) return;
				PwDatabase pd = Program.MainForm.DocumentManager.SafeFindContainerOf(pe);

				IntPtr hFg = NativeMethods.GetForegroundWindowHandle();
				if(AutoType.IsOwnWindow(hFg))
					AutoType.PerformIntoPreviousWindow(Program.MainForm, pe,
						pd, strSeq);
				else AutoType.PerformIntoCurrentWindow(pe, pd, strSeq);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private static void ShowEntriesByTag(EcasAction a, EcasContext ctx)
		{
			string strTag = EcasUtil.GetParamString(a.Parameters, 0, true);
			Program.MainForm.ShowEntriesByTag(strTag);
		}

		private static void AddToolBarButton(EcasAction a, EcasContext ctx)
		{
			string strID = EcasUtil.GetParamString(a.Parameters, 0, true);
			string strName = EcasUtil.GetParamString(a.Parameters, 1, true);
			string strDesc = EcasUtil.GetParamString(a.Parameters, 2, true);

			Program.MainForm.AddCustomToolBarButton(strID, strName, strDesc);
		}

		private static void RemoveToolBarButton(EcasAction a, EcasContext ctx)
		{
			string strID = EcasUtil.GetParamString(a.Parameters, 0, true);
			Program.MainForm.RemoveCustomToolBarButton(strID);
		}

		private static void ShowMessageBox(EcasAction a, EcasContext ctx)
		{
			VistaTaskDialog vtd = new VistaTaskDialog();

			string strMain = EcasUtil.GetParamString(a.Parameters, 0, true);
			if(!string.IsNullOrEmpty(strMain)) vtd.MainInstruction = strMain;

			string strText = EcasUtil.GetParamString(a.Parameters, 1, true);
			if(!string.IsNullOrEmpty(strText)) vtd.Content = strText;

			uint uIcon = EcasUtil.GetParamUInt(a.Parameters, 2, 0);
			if(uIcon == (uint)MessageBoxIcon.Information)
				vtd.SetIcon(VtdIcon.Information);
			else if(uIcon == (uint)MessageBoxIcon.Question)
				vtd.SetIcon(VtdCustomIcon.Question);
			else if(uIcon == (uint)MessageBoxIcon.Warning)
				vtd.SetIcon(VtdIcon.Warning);
			else if(uIcon == (uint)MessageBoxIcon.Error)
				vtd.SetIcon(VtdIcon.Error);
			else { Debug.Assert(uIcon == (uint)MessageBoxIcon.None); }

			vtd.CommandLinks = false;

			uint uBtns = EcasUtil.GetParamUInt(a.Parameters, 3, 0);
			bool bCanCancel = false;
			if(uBtns == (uint)MessageBoxButtons.OKCancel)
			{
				vtd.AddButton((int)DialogResult.OK, KPRes.Ok, null);
				vtd.AddButton((int)DialogResult.Cancel, KPRes.Cancel, null);
				bCanCancel = true;
			}
			else if(uBtns == (uint)MessageBoxButtons.YesNo)
			{
				vtd.AddButton((int)DialogResult.OK, KPRes.YesCmd, null);
				vtd.AddButton((int)DialogResult.Cancel, KPRes.NoCmd, null);
				bCanCancel = true;
			}
			else vtd.AddButton((int)DialogResult.OK, KPRes.Ok, null);

			uint uDef = EcasUtil.GetParamUInt(a.Parameters, 4, 0);
			ReadOnlyCollection<VtdButton> lButtons = vtd.Buttons;
			if(uDef < (uint)lButtons.Count)
				vtd.DefaultButtonID = lButtons[(int)uDef].ID;

			vtd.WindowTitle = PwDefs.ShortProductName;

			string strTrg = ctx.Trigger.Name;
			if(!string.IsNullOrEmpty(strTrg))
			{
				vtd.FooterText = KPRes.Trigger + @": '" + strTrg + @"'.";
				vtd.SetFooterIcon(VtdIcon.Information);
			}

			int dr;
			if(vtd.ShowDialog()) dr = vtd.Result;
			else
			{
				string str = (strMain ?? string.Empty);
				if(!string.IsNullOrEmpty(strText))
				{
					if(str.Length > 0) str += MessageService.NewParagraph;
					str += strText;
				}

				MessageBoxDefaultButton mbdb = MessageBoxDefaultButton.Button1;
				if(uDef == 1) mbdb = MessageBoxDefaultButton.Button2;
				else if(uDef == 2) mbdb = MessageBoxDefaultButton.Button3;

				MessageService.ExternalIncrementMessageCount();
				try
				{
					dr = (int)MessageService.SafeShowMessageBox(str,
						PwDefs.ShortProductName, (MessageBoxButtons)uBtns,
						(MessageBoxIcon)uIcon, mbdb);
				}
				finally { MessageService.ExternalDecrementMessageCount(); }
			}

			uint uActCondID = EcasUtil.GetParamUInt(a.Parameters, 5, 0);

			bool bDrY = ((dr == (int)DialogResult.OK) ||
				(dr == (int)DialogResult.Yes));
			bool bDrN = ((dr == (int)DialogResult.Cancel) ||
				(dr == (int)DialogResult.No));

			bool bPerformAction = (((uActCondID == IdMbcY) && bDrY) ||
				((uActCondID == IdMbcN) && bDrN));
			if(!bPerformAction) return;

			uint uActID = EcasUtil.GetParamUInt(a.Parameters, 6, 0);
			string strActionParam = EcasUtil.GetParamString(a.Parameters, 7, true);

			if(uActID == IdMbaNone) { }
			else if(uActID == IdMbaAbort)
			{
				if(bCanCancel) ctx.Cancel = true;
			}
			else if(uActID == IdMbaCmd)
			{
				if(!string.IsNullOrEmpty(strActionParam))
					WinUtil.OpenUrl(strActionParam, null);
			}
			else { Debug.Assert(false); }
		}
	}
}
