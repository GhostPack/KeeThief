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
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.Ecas
{
	internal sealed class EcasDefaultConditionProvider : EcasConditionProvider
	{
		public EcasDefaultConditionProvider()
		{
			m_conditions.Add(new EcasConditionType(new PwUuid(new byte[] {
				0x9F, 0x11, 0xD0, 0xBD, 0xEC, 0xE9, 0x45, 0x3B,
				0xA5, 0x45, 0x26, 0x1F, 0xF7, 0xA4, 0xFF, 0x1F }),
				KPRes.EnvironmentVariable, PwIcon.Console, new EcasParameter[] {
					new EcasParameter(KPRes.Name, EcasValueType.String, null),
					new EcasParameter(KPRes.Value + " - " + KPRes.Comparison,
						EcasValueType.EnumStrings, EcasUtil.StdStringCompare),
					new EcasParameter(KPRes.Value, EcasValueType.String, null) },
				IsMatchEnvironmentVar));

			m_conditions.Add(new EcasConditionType(new PwUuid(new byte[] {
				0xB9, 0x0F, 0xF8, 0x07, 0x73, 0x38, 0x4F, 0xEA,
				0xBB, 0x2E, 0xBC, 0x0B, 0xEA, 0x3B, 0x98, 0xC3 }),
				KPRes.String, PwIcon.Configuration, new EcasParameter[] {
					new EcasParameter(KPRes.String, EcasValueType.String, null),
					new EcasParameter(KPRes.Value + " - " + KPRes.Comparison,
						EcasValueType.EnumStrings, EcasUtil.StdStringCompare),
					new EcasParameter(KPRes.Value, EcasValueType.String, null) },
				IsMatchString));

			m_conditions.Add(new EcasConditionType(new PwUuid(new byte[] {
				0xCB, 0x4A, 0x9E, 0x34, 0x56, 0x8C, 0x4C, 0x95,
				0xAD, 0x67, 0x4D, 0x1C, 0xA1, 0x04, 0x19, 0xBC }),
				KPRes.FileExists, PwIcon.PaperReady, new EcasParameter[] {
					new EcasParameter(KPRes.FileOrUrl, EcasValueType.String, null) },
				IsMatchFileExists));

			m_conditions.Add(new EcasConditionType(new PwUuid(new byte[] {
				0x2A, 0x22, 0x83, 0xA8, 0x9D, 0x13, 0x41, 0xE8,
				0x99, 0x87, 0x8B, 0xAC, 0x21, 0x8D, 0x81, 0xF4 }),
				KPRes.RemoteHostReachable, PwIcon.NetworkServer, new EcasParameter[] {
					new EcasParameter(KPRes.Host, EcasValueType.String, null) },
				IsHostReachable));

			m_conditions.Add(new EcasConditionType(new PwUuid(new byte[] {
				0xD3, 0xCA, 0xFA, 0xEF, 0x28, 0x2A, 0x46, 0x4A,
				0x99, 0x90, 0xD8, 0x65, 0xFC, 0xE0, 0x16, 0xED }),
				KPRes.DatabaseHasUnsavedChanges, PwIcon.PaperFlag, new EcasParameter[] {
					new EcasParameter(KPRes.Database, EcasValueType.EnumStrings,
						new EcasEnum(new EcasEnumItem[] {
							new EcasEnumItem(0, KPRes.Active),
							new EcasEnumItem(1, KPRes.Triggering) })) },
				IsDatabaseModified));
		}

		private static bool IsMatchEnvironmentVar(EcasCondition c, EcasContext ctx)
		{
			string strName = EcasUtil.GetParamString(c.Parameters, 0, true);
			uint uCompareType = EcasUtil.GetParamEnum(c.Parameters, 1,
				EcasUtil.StdStringCompareEquals, EcasUtil.StdStringCompare);
			string strValue = EcasUtil.GetParamString(c.Parameters, 2, true);

			if(string.IsNullOrEmpty(strName) || (strValue == null))
				return false;

			try
			{
				string strVar = Environment.GetEnvironmentVariable(strName);
				if(strVar == null) return false;

				return EcasUtil.CompareStrings(strVar, strValue, uCompareType);
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		private static bool IsMatchString(EcasCondition c, EcasContext ctx)
		{
			string str = EcasUtil.GetParamString(c.Parameters, 0, true);
			uint uCompareType = EcasUtil.GetParamEnum(c.Parameters, 1,
				EcasUtil.StdStringCompareEquals, EcasUtil.StdStringCompare);
			string strValue = EcasUtil.GetParamString(c.Parameters, 2, true);

			if((str == null) || (strValue == null)) return false;

			return EcasUtil.CompareStrings(str, strValue, uCompareType);
		}

		private static bool IsMatchFileExists(EcasCondition c, EcasContext ctx)
		{
			string strFile = EcasUtil.GetParamString(c.Parameters, 0, true);
			if(string.IsNullOrEmpty(strFile)) return true;

			try
			{
				// return File.Exists(strFile);

				IOConnectionInfo ioc = IOConnectionInfo.FromPath(strFile);
				return IOConnection.FileExists(ioc);
			}
			catch(Exception) { }

			return false;
		}

		private static bool IsHostReachable(EcasCondition c, EcasContext ctx)
		{
			string strHost = EcasUtil.GetParamString(c.Parameters, 0, true);
			if(string.IsNullOrEmpty(strHost)) return true;

			int[] vTimeOuts = { 250, 1250 };
			const string strBuffer = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			byte[] pbBuffer = Encoding.ASCII.GetBytes(strBuffer);

			try
			{
				Ping ping = new Ping(); // We have sufficient privileges?
				PingOptions options = new PingOptions(64, true);

				foreach(int nTimeOut in vTimeOuts)
				{
					PingReply reply = ping.Send(strHost, nTimeOut, pbBuffer, options);
					if(reply.Status == IPStatus.Success) return true;
				}

				return false;
			}
			catch(Exception) { }

			return false;
		}

		private static bool IsDatabaseModified(EcasCondition c, EcasContext ctx)
		{
			PwDatabase pd = null;

			uint uSel = EcasUtil.GetParamUInt(c.Parameters, 0, 0);
			if(uSel == 0)
				pd = Program.MainForm.ActiveDatabase;
			else if(uSel == 1)
				pd = ctx.Properties.Get<PwDatabase>(EcasProperty.Database);
			else { Debug.Assert(false); }

			if((pd == null) || !pd.IsOpen) return false;
			return pd.Modified;
		}
	}
}
