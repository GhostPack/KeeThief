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
using System.Windows.Forms;
using System.Drawing;

using KeePass.Ecas;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util.Spr;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Ecas
{
	public enum EcasTypeDxMode // Type data exchange modes
	{
		None = 0,
		Selection, // DX with the type UI control (combobox)
		ParamsTag // Get type from the parameters control
	}

	public static class EcasUtil
	{
		public const uint StdCompareEqual = 0;
		public const uint StdCompareNotEqual = 1;
		public const uint StdCompareLesser = 2;
		public const uint StdCompareLesserEqual = 3;
		public const uint StdCompareGreater = 4;
		public const uint StdCompareGreaterEqual = 5;

		private static EcasEnum m_enumCompare = null;
		public static EcasEnum StdCompare
		{
			get
			{
				if(m_enumCompare == null)
					m_enumCompare = new EcasEnum(new EcasEnumItem[] {
						new EcasEnumItem(StdCompareEqual, "="),
						new EcasEnumItem(StdCompareNotEqual, "<>"),
						new EcasEnumItem(StdCompareLesser, "<"),
						new EcasEnumItem(StdCompareLesserEqual, "<="),
						new EcasEnumItem(StdCompareGreater, ">"),
						new EcasEnumItem(StdCompareGreaterEqual, ">=") });

				return m_enumCompare;
			}
		}

		public const uint StdStringCompareEquals = 0;
		public const uint StdStringCompareContains = 1;
		public const uint StdStringCompareStartsWith = 2;
		public const uint StdStringCompareEndsWith = 3;

		private static EcasEnum m_enumStringCompare = null;
		public static EcasEnum StdStringCompare
		{
			get
			{
				if(m_enumStringCompare == null)
					m_enumStringCompare = new EcasEnum(new EcasEnumItem[] {
						new EcasEnumItem(StdStringCompareEquals, KPRes.EqualsOp),
						new EcasEnumItem(StdStringCompareContains, KPRes.ContainsOp),
						new EcasEnumItem(StdStringCompareStartsWith, KPRes.StartsWith),
						new EcasEnumItem(StdStringCompareEndsWith, KPRes.EndsWith) });

				return m_enumStringCompare;
			}
		}

		public static string GetParamString(List<string> vParams, int iIndex)
		{
			return GetParamString(vParams, iIndex, string.Empty);
		}

		public static string GetParamString(List<string> vParams, int iIndex,
			bool bSprCompile)
		{
			return GetParamString(vParams, iIndex, bSprCompile, false);
		}

		public static string GetParamString(List<string> vParams, int iIndex,
			bool bSprCompile, bool bSprForCommandLine)
		{
			string str = GetParamString(vParams, iIndex, string.Empty);

			if(bSprCompile && !string.IsNullOrEmpty(str))
			{
				PwEntry pe = null;
				try { pe = Program.MainForm.GetSelectedEntry(false); }
				catch(Exception) { Debug.Assert(false); }

				PwDatabase pd = Program.MainForm.DocumentManager.SafeFindContainerOf(pe);

				str = SprEngine.Compile(str, new SprContext(pe, pd,
					SprCompileFlags.All, false, bSprForCommandLine));
			}

			return str;
		}

		public static string GetParamString(List<string> vParams, int iIndex,
			string strDefault)
		{
			if(vParams == null) { Debug.Assert(false); return strDefault; }
			if(iIndex < 0) { Debug.Assert(false); return strDefault; }
			if(iIndex >= vParams.Count) return strDefault; // No assert

			return vParams[iIndex];
		}

		public static uint GetParamUInt(List<string> vParams, int iIndex)
		{
			return GetParamUInt(vParams, iIndex, 0);
		}

		public static uint GetParamUInt(List<string> vParams, int iIndex,
			uint uDefault)
		{
			string str = GetParamString(vParams, iIndex, string.Empty);
			uint u;
			if(uint.TryParse(str, out u)) return u;
			return uDefault;
		}

		public static uint GetParamEnum(List<string> vParams, int iIndex,
			uint uDefault, EcasEnum enumItems)
		{
			if(enumItems == null) { Debug.Assert(false); return uDefault; }

			string str = GetParamString(vParams, iIndex, null);
			if(string.IsNullOrEmpty(str)) { Debug.Assert(false); return uDefault; }

			uint uID;
			if(!uint.TryParse(str, out uID)) { Debug.Assert(false); return uDefault; }

			// Make sure the enumeration contains the value
			if(enumItems.GetItemString(uID, null) == null) { Debug.Assert(false); return uDefault; }

			return uID;
		}

		public static void ParametersToDataGridView(DataGridView dg,
			IEcasParameterized p, IEcasObject objDefaults)
		{
			if(dg == null) throw new ArgumentNullException("dg");
			if(p == null) throw new ArgumentNullException("p");
			if(p.Parameters == null) throw new ArgumentException();
			if(objDefaults == null) throw new ArgumentNullException("objDefaults");
			if(objDefaults.Parameters == null) throw new ArgumentException();

			dg.Rows.Clear();
			dg.Columns.Clear();

			Color clrBack = dg.DefaultCellStyle.BackColor;
			Color clrValueBack = dg.DefaultCellStyle.BackColor;
			if(clrValueBack.GetBrightness() >= 0.5)
				clrValueBack = UIUtil.DarkenColor(clrValueBack, 0.075);
			else clrValueBack = UIUtil.LightenColor(clrValueBack, 0.075);

			dg.ColumnHeadersVisible = false;
			dg.RowHeadersVisible = false;
			dg.GridColor = clrBack;
			dg.BackgroundColor = clrBack;
			dg.DefaultCellStyle.SelectionBackColor = clrBack;
			dg.DefaultCellStyle.SelectionForeColor = dg.DefaultCellStyle.ForeColor;
			dg.AllowDrop = false;
			dg.AllowUserToAddRows = false;
			dg.AllowUserToDeleteRows = false;
			dg.AllowUserToOrderColumns = false;
			dg.AllowUserToResizeColumns = false;
			dg.AllowUserToResizeRows = false;
			// dg.EditMode: see below
			dg.Tag = p;

			int nWidth = (dg.ClientSize.Width - UIUtil.GetVScrollBarWidth());
			dg.Columns.Add("Name", KPRes.FieldName);
			dg.Columns.Add("Value", KPRes.FieldValue);
			dg.Columns[0].Width = (nWidth / 2);
			dg.Columns[1].Width = (nWidth / 2);

			bool bUseDefaults = true;
			if(objDefaults.Type == null) { Debug.Assert(false); } // Optimistic
			else if(p.Type == null) { Debug.Assert(false); } // Optimistic
			else if(!objDefaults.Type.Equals(p.Type)) bUseDefaults = false;

			for(int i = 0; i < p.Parameters.Length; ++i)
			{
				EcasParameter ep = p.Parameters[i];

				dg.Rows.Add();
				DataGridViewRow row = dg.Rows[dg.Rows.Count - 1];
				DataGridViewCellCollection cc = row.Cells;

				Debug.Assert(cc.Count == 2);
				cc[0].Value = ep.Name;
				cc[0].ReadOnly = true;

				string strParam = (bUseDefaults ? EcasUtil.GetParamString(
					objDefaults.Parameters, i) : string.Empty);

				DataGridViewCell c = null;
				switch(ep.Type)
				{
					case EcasValueType.String:
						c = new DataGridViewTextBoxCell();
						c.Value = strParam;
						break;

					case EcasValueType.Bool:
						c = new DataGridViewCheckBoxCell(false);
						(c as DataGridViewCheckBoxCell).Value =
							StrUtil.StringToBool(strParam);
						break;

					case EcasValueType.EnumStrings:
						DataGridViewComboBoxCell cmb = new DataGridViewComboBoxCell();
						cmb.Sorted = false;
						cmb.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
						int iFound = -1;
						for(int e = 0; e < ep.EnumValues.ItemCount; ++e)
						{
							EcasEnumItem eei = ep.EnumValues.Items[e];
							cmb.Items.Add(eei.Name);
							if(eei.ID.ToString() == strParam) iFound = e;
						}
						if(iFound >= 0) cmb.Value = ep.EnumValues.Items[iFound].Name;
						else if(ep.EnumValues.ItemCount > 0) cmb.Value = ep.EnumValues.Items[0].Name;
						else { Debug.Assert(false); }
						c = cmb;
						break;

					case EcasValueType.Int64:
						c = new DataGridViewTextBoxCell();
						c.Value = FilterTypeI64(strParam);
						break;

					case EcasValueType.UInt64:
						c = new DataGridViewTextBoxCell();
						c.Value = FilterTypeU64(strParam);
						break;

					default:
						Debug.Assert(false);
						break;
				}

				if(c != null) cc[1] = c;
				cc[1].ReadOnly = false;
				cc[1].Style.BackColor = clrValueBack;
				cc[1].Style.SelectionBackColor = clrValueBack;
			}

			// Perform postponed setting of EditMode (cannot set it earlier
			// due to a Mono bug on FreeBSD);
			// https://sourceforge.net/p/keepass/discussion/329220/thread/cb8270e2/
			dg.EditMode = DataGridViewEditMode.EditOnEnter;
		}

		public static void DataGridViewToParameters(DataGridView dg, IEcasObject objOut,
			IEcasParameterized eTypeInfo)
		{
			if(dg == null) throw new ArgumentNullException("dg");
			if(objOut == null) throw new ArgumentNullException("objOut");
			// if(vParamDesc == null) throw new ArgumentNullException("vParamDesc");
			// if(dg.Rows.Count != vParamDesc.Length) { Debug.Assert(false); return; }

			objOut.Parameters.Clear();

			bool bTypeInfoValid = ((eTypeInfo != null) && (eTypeInfo.Parameters.Length ==
				dg.Rows.Count));

			for(int i = 0; i < dg.RowCount; ++i)
			{
				DataGridViewCell c = dg.Rows[i].Cells[1];
				object oValue = c.Value;
				string strValue = ((oValue != null) ? oValue.ToString() : string.Empty);

				if(bTypeInfoValid && (eTypeInfo.Parameters[i].EnumValues != null) &&
					(c is DataGridViewComboBoxCell))
				{
					objOut.Parameters.Add(eTypeInfo.Parameters[i].EnumValues.GetItemID(
						strValue, 0).ToString());
				}
				else objOut.Parameters.Add(strValue);
			}
		}

		public static bool UpdateDialog(EcasObjectType objType, ComboBox cmbTypes,
			DataGridView dgvParams, IEcasObject o, bool bGuiToInternal,
			EcasTypeDxMode dxType)
		{
			bool bResult = true;

			try
			{
				if(bGuiToInternal)
				{
					IEcasParameterized eTypeInfo = null;

					if(dxType == EcasTypeDxMode.Selection)
					{
						string strSel = (cmbTypes.SelectedItem as string);
						if(!string.IsNullOrEmpty(strSel))
						{
							if(objType == EcasObjectType.Event)
							{
								eTypeInfo = Program.EcasPool.FindEvent(strSel);
								o.Type = eTypeInfo.Type;
							}
							else if(objType == EcasObjectType.Condition)
							{
								eTypeInfo = Program.EcasPool.FindCondition(strSel);
								o.Type = eTypeInfo.Type;
							}
							else if(objType == EcasObjectType.Action)
							{
								eTypeInfo = Program.EcasPool.FindAction(strSel);
								o.Type = eTypeInfo.Type;
							}
							else { Debug.Assert(false); }
						}
					}
					else if(dxType == EcasTypeDxMode.ParamsTag)
					{
						IEcasParameterized p = (dgvParams.Tag as IEcasParameterized);
						if((p != null) && (p.Type != null))
						{
							eTypeInfo = p;
							o.Type = eTypeInfo.Type;
						}
						else { Debug.Assert(false); }
					}

					EcasUtil.DataGridViewToParameters(dgvParams, o, eTypeInfo);
				}
				else // Internal to GUI
				{
					if(dxType == EcasTypeDxMode.Selection)
					{
						if(o.Type.Equals(PwUuid.Zero))
							cmbTypes.SelectedIndex = 0;
						else
						{
							int i = -1;
							if(objType == EcasObjectType.Event)
								i = cmbTypes.FindString(Program.EcasPool.FindEvent(o.Type).Name);
							else if(objType == EcasObjectType.Condition)
								i = cmbTypes.FindString(Program.EcasPool.FindCondition(o.Type).Name);
							else if(objType == EcasObjectType.Action)
								i = cmbTypes.FindString(Program.EcasPool.FindAction(o.Type).Name);
							else { Debug.Assert(false); }

							if(i >= 0) cmbTypes.SelectedIndex = i;
							else { Debug.Assert(false); }
						}
					}
					else { Debug.Assert(dxType != EcasTypeDxMode.ParamsTag); }

					IEcasParameterized t = null;
					if(objType == EcasObjectType.Event)
						t = Program.EcasPool.FindEvent(cmbTypes.SelectedItem as string);
					else if(objType == EcasObjectType.Condition)
						t = Program.EcasPool.FindCondition(cmbTypes.SelectedItem as string);
					else if(objType == EcasObjectType.Action)
						t = Program.EcasPool.FindAction(cmbTypes.SelectedItem as string);
					else { Debug.Assert(false); }

					if(t != null) EcasUtil.ParametersToDataGridView(dgvParams, t, o);
				}
			}
			catch(Exception e) { MessageService.ShowWarning(e); bResult = false; }

			return bResult;
		}

		public static string ParametersToString(IEcasObject ecasObj,
			EcasParameter[] vParamInfo)
		{
			if(ecasObj == null) throw new ArgumentNullException("ecasObj");
			if(ecasObj.Parameters == null) throw new ArgumentException();

			bool bParamInfoValid = true;
			if((vParamInfo == null) || (ecasObj.Parameters.Count > vParamInfo.Length))
			{
				Debug.Assert(false);
				bParamInfoValid = false;
			}

			StringBuilder sb = new StringBuilder();

			EcasCondition eCond = (ecasObj as EcasCondition);
			if(eCond != null)
			{
				if(eCond.Negate) sb.Append(KPRes.Not);
			}

			for(int i = 0; i < ecasObj.Parameters.Count; ++i)
			{
				string strParam = ecasObj.Parameters[i];
				string strAppend;

				if(bParamInfoValid)
				{
					EcasValueType t = vParamInfo[i].Type;
					if(t == EcasValueType.String)
						strAppend = strParam;
					else if(t == EcasValueType.Bool)
						strAppend = (StrUtil.StringToBool(strParam) ? KPRes.Yes : KPRes.No);
					else if(t == EcasValueType.EnumStrings)
					{
						uint uEnumID;
						if(uint.TryParse(strParam, out uEnumID) == false) { Debug.Assert(false); }
						EcasEnum ee = vParamInfo[i].EnumValues;
						if(ee != null) strAppend = ee.GetItemString(uEnumID, string.Empty);
						else { Debug.Assert(false); strAppend = strParam; }
					}
					else if(t == EcasValueType.Int64)
						strAppend = FilterTypeI64(strParam);
					else if(t == EcasValueType.UInt64)
						strAppend = FilterTypeU64(strParam);
					else { Debug.Assert(false); strAppend = strParam; }
				}
				else strAppend = strParam;

				if(string.IsNullOrEmpty(strAppend)) continue;
				string strAppTrimmed = strAppend.Trim();
				if(strAppTrimmed.Length == 0) continue;
				if(sb.Length > 0) sb.Append(", ");
				sb.Append(strAppTrimmed);
			}

			return sb.ToString();
		}

		public static bool CompareStrings(string x, string y, uint uCompareType)
		{
			if(x == null) { Debug.Assert(false); return false; }
			if(y == null) { Debug.Assert(false); return false; }

			if(uCompareType == EcasUtil.StdStringCompareEquals)
				return x.Equals(y, StrUtil.CaseIgnoreCmp);
			if(uCompareType == EcasUtil.StdStringCompareStartsWith)
				return x.StartsWith(y, StrUtil.CaseIgnoreCmp);
			if(uCompareType == EcasUtil.StdStringCompareEndsWith)
				return x.EndsWith(y, StrUtil.CaseIgnoreCmp);

			Debug.Assert(uCompareType == EcasUtil.StdStringCompareContains);
			return (x.IndexOf(y, StrUtil.CaseIgnoreCmp) >= 0);
		}

		private static string FilterTypeI64(string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;

			long i64;
			if(long.TryParse(str, out i64)) return i64.ToString();

			return string.Empty;
		}

		private static string FilterTypeU64(string str)
		{
			if(string.IsNullOrEmpty(str)) return string.Empty;

			ulong u64;
			if(ulong.TryParse(str, out u64)) return u64.ToString();

			return string.Empty;
		}
	}
}
