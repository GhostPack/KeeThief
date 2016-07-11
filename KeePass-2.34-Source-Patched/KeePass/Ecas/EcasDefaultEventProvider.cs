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

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Utility;
using KeePassLib.Serialization;

namespace KeePass.Ecas
{
	internal static class EcasEventIDs
	{
		public static readonly PwUuid OpenedDatabaseFile = new PwUuid(new byte[] {
			0xE5, 0xFF, 0x13, 0x06, 0x85, 0xB8, 0x41, 0x89,
			0xB9, 0x06, 0xF6, 0x9E, 0x2B, 0x3B, 0x40, 0xA7
		});
		public static readonly PwUuid SavingDatabaseFile = new PwUuid(new byte[] {
			0x95, 0xC1, 0xA6, 0xFD, 0x72, 0x7C, 0x40, 0xC7,
			0xA2, 0xF9, 0x5B, 0x0F, 0xA0, 0x99, 0x63, 0x1C
		});
		public static readonly PwUuid SavedDatabaseFile = new PwUuid(new byte[] {
			0xB3, 0xA8, 0xFD, 0xFE, 0x78, 0x13, 0x4A, 0x6A,
			0x9C, 0x5D, 0xD5, 0xBA, 0x84, 0x3A, 0x9B, 0x8E
		});
		public static readonly PwUuid ClosingDatabaseFilePre = new PwUuid(new byte[] {
			0x8C, 0xEA, 0xDE, 0x9A, 0xA8, 0x17, 0x49, 0x19,
			0xA3, 0x2F, 0xF4, 0x1E, 0x3B, 0x1D, 0xEC, 0x49
		});
		public static readonly PwUuid ClosingDatabaseFilePost = new PwUuid(new byte[] {
			0x94, 0xFA, 0x70, 0xE5, 0xB1, 0x3F, 0x41, 0x26,
			0xA6, 0x4E, 0x06, 0x4F, 0xD8, 0xC3, 0x6C, 0x95
		});
		public static readonly PwUuid CopiedEntryInfo = new PwUuid(new byte[] {
			0x3F, 0x7E, 0x5E, 0xC6, 0x2A, 0x54, 0x4C, 0x58,
			0x95, 0x44, 0x85, 0xFB, 0xF2, 0x6F, 0x56, 0xDC
		});
		public static readonly PwUuid AppInitPost = new PwUuid(new byte[] {
			0xD4, 0xCE, 0xCD, 0xB5, 0x4B, 0x98, 0x4F, 0xF2,
			0xA6, 0xA9, 0xE2, 0x55, 0x26, 0x1E, 0xC8, 0xE8
		});
		public static readonly PwUuid AppLoadPost = new PwUuid(new byte[] {
			0xD8, 0xF3, 0x1E, 0xE9, 0xCC, 0x69, 0x48, 0x1B,
			0x89, 0xC5, 0xFC, 0xE2, 0xEA, 0x4B, 0x6A, 0x97
		});
		public static readonly PwUuid AppExit = new PwUuid(new byte[] {
			0x82, 0x8A, 0xB7, 0xAB, 0xB1, 0x1C, 0x4E, 0xBF,
			0x80, 0x39, 0x36, 0x3F, 0x91, 0x71, 0x97, 0x78
		});
		public static readonly PwUuid CustomTbButtonClicked = new PwUuid(new byte[] {
			0x47, 0x47, 0x59, 0x92, 0x97, 0xA7, 0x43, 0xA2,
			0xB9, 0x68, 0x1F, 0x1F, 0xC2, 0xF7, 0x9B, 0x92
		});
		public static readonly PwUuid UpdatedUIState = new PwUuid(new byte[] {
			0x8D, 0x12, 0xD4, 0x9A, 0xF2, 0xCB, 0x4F, 0xF7,
			0xA8, 0xEF, 0xCF, 0xDA, 0xAC, 0x62, 0x68, 0x99
		});
	}

	internal sealed class EcasDefaultEventProvider : EcasEventProvider
	{
		public EcasDefaultEventProvider()
		{
			EcasParameter[] epFileFilter = new EcasParameter[] {
				new EcasParameter(KPRes.FileOrUrl + " - " + KPRes.Comparison,
					EcasValueType.EnumStrings, EcasUtil.StdStringCompare),
				new EcasParameter(KPRes.FileOrUrl + " - " + KPRes.Filter,
					EcasValueType.String, null) };
			EcasParameter[] epValueFilter = new EcasParameter[] {
				new EcasParameter(KPRes.Value + " - " + KPRes.Comparison,
					EcasValueType.EnumStrings, EcasUtil.StdStringCompare),
				new EcasParameter(KPRes.Value + " - " + KPRes.Filter,
					EcasValueType.String, null) };

			m_events.Add(new EcasEventType(EcasEventIDs.AppInitPost,
				KPRes.ApplicationInitialized, PwIcon.ProgramIcons, null, null));
			m_events.Add(new EcasEventType(EcasEventIDs.AppLoadPost,
				KPRes.ApplicationStarted, PwIcon.ProgramIcons, null, null));
			m_events.Add(new EcasEventType(EcasEventIDs.AppExit,
				KPRes.ApplicationExit, PwIcon.ProgramIcons, null, null));
			m_events.Add(new EcasEventType(EcasEventIDs.OpenedDatabaseFile,
				KPRes.OpenedDatabaseFile, PwIcon.FolderOpen, epFileFilter,
				IsMatchIocDbEvent));
			m_events.Add(new EcasEventType(EcasEventIDs.SavingDatabaseFile,
				KPRes.SavingDatabaseFile, PwIcon.Disk, epFileFilter,
				IsMatchIocDbEvent));
			m_events.Add(new EcasEventType(EcasEventIDs.SavedDatabaseFile,
				KPRes.SavedDatabaseFile, PwIcon.Disk, epFileFilter,
				IsMatchIocDbEvent));
			m_events.Add(new EcasEventType(EcasEventIDs.ClosingDatabaseFilePre,
				KPRes.ClosingDatabaseFile + " (" + KPRes.SavingPre + ")",
				PwIcon.PaperQ, epFileFilter, IsMatchIocDbEvent));
			m_events.Add(new EcasEventType(EcasEventIDs.ClosingDatabaseFilePost,
				KPRes.ClosingDatabaseFile + " (" + KPRes.SavingPost + ")",
				PwIcon.PaperQ, epFileFilter, IsMatchIocDbEvent));
			m_events.Add(new EcasEventType(EcasEventIDs.CopiedEntryInfo,
				KPRes.CopiedEntryData, PwIcon.ClipboardReady, epValueFilter,
				IsMatchTextEvent));
			m_events.Add(new EcasEventType(EcasEventIDs.UpdatedUIState,
				KPRes.UpdatedUIState, PwIcon.PaperReady, null, null));
			m_events.Add(new EcasEventType(EcasEventIDs.CustomTbButtonClicked,
				KPRes.CustomTbButtonClicked, PwIcon.Star, new EcasParameter[] {
					new EcasParameter(KPRes.Id, EcasValueType.String, null) },
				IsMatchIdEvent));
		}

		private static bool IsMatchIocDbEvent(EcasEvent e, EcasContext ctx)
		{
			uint uCompareType = EcasUtil.GetParamEnum(e.Parameters, 0,
				EcasUtil.StdStringCompareEquals, EcasUtil.StdStringCompare);

			string strFilter = EcasUtil.GetParamString(e.Parameters, 1, true);
			if(string.IsNullOrEmpty(strFilter)) return true;

			// Must prefer IOC (e.g. for SavingDatabaseFile)
			IOConnectionInfo ioc = ctx.Properties.Get<IOConnectionInfo>(
				EcasProperty.IOConnectionInfo);
			if(ioc == null)
			{
				PwDatabase pd = ctx.Properties.Get<PwDatabase>(EcasProperty.Database);
				if(pd == null) { Debug.Assert(false); return false; }

				ioc = pd.IOConnectionInfo;
			}
			if(ioc == null) { Debug.Assert(false); return false; }
			string strCurFile = ioc.Path;
			if(string.IsNullOrEmpty(strCurFile)) return false;

			return EcasUtil.CompareStrings(strCurFile, strFilter, uCompareType);
		}

		private static bool IsMatchTextEvent(EcasEvent e, EcasContext ctx)
		{
			uint uCompareType = EcasUtil.GetParamEnum(e.Parameters, 0,
				EcasUtil.StdStringCompareEquals, EcasUtil.StdStringCompare);

			string strFilter = EcasUtil.GetParamString(e.Parameters, 1, true);
			if(string.IsNullOrEmpty(strFilter)) return true;

			string str = ctx.Properties.Get<string>(EcasProperty.Text);
			if(str == null) { Debug.Assert(false); return false; }

			return EcasUtil.CompareStrings(str, strFilter, uCompareType);
		}

		private static bool IsMatchIdEvent(EcasEvent e, EcasContext ctx)
		{
			string strIdRef = EcasUtil.GetParamString(e.Parameters, 0, true);
			if(string.IsNullOrEmpty(strIdRef)) return true;

			string strIdCur = ctx.Properties.Get<string>(EcasProperty.CommandID);
			if(string.IsNullOrEmpty(strIdCur)) return false;

			return strIdRef.Equals(strIdCur, StrUtil.CaseIgnoreCmp);
		}
	}
}
