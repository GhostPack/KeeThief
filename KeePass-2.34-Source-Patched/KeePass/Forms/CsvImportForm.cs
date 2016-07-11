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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

using KeePass.App;
using KeePass.DataExchange;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class CsvImportForm : Form
	{
		private byte[] m_pbData = null;
		private PwDatabase m_pwDatabase = null;

		private bool m_bInitializing = true;
		private uint m_uStartOffset = 0;
		private CsvFieldType m_tLastCsvType = CsvFieldType.Count;

		private readonly string StrCharTab = @"{Tab}";
		private readonly string StrCharNewLine = @"{" + KPRes.NewLine + @"}";

		private enum CsvFieldType
		{
			Ignore = 0,
			Group,
			Title,
			UserName,
			Password,
			Url,
			Notes,
			CustomString,
			CreationTime,
			// LastAccessTime,
			LastModTime,
			ExpiryTime,
			Tags,

			Count, // Last enum item + 1
			First = 0
		}

		private sealed class CsvFieldInfo
		{
			private readonly CsvFieldType m_t;
			public CsvFieldType Type { get { return m_t; } }

			private readonly string m_strName;
			public string Name { get { return m_strName; } }

			private readonly string m_strFormat;
			public string Format { get { return m_strFormat; } }

			public CsvFieldInfo(CsvFieldType t, string strName, string strFormat)
			{
				m_t = t;
				m_strName = strName; // May be null
				m_strFormat = strFormat; // May be null
			}
		}

		public void InitEx(PwDatabase pwStorage, byte[] pbInData)
		{
			m_pwDatabase = pwStorage;
			m_pbData = pbInData;
		}

		public CsvImportForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if((m_pbData == null) || (m_pwDatabase == null))
				throw new InvalidOperationException();

			m_bInitializing = true;

			GlobalWindowManager.AddWindow(this);

			// Callable from KPScript without parent form
			Debug.Assert(this.StartPosition == FormStartPosition.CenterScreen);

			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.GenericCsvImporter + " - " + PwDefs.ShortProductName;

			// FontUtil.AssignDefaultBold(m_grpSyntax);
			// FontUtil.AssignDefaultBold(m_grpSem);

			UIUtil.SetExplorerTheme(m_lvFields, false);
			UIUtil.SetExplorerTheme(m_lvImportPreview, false);

			foreach(StrEncodingInfo seiEnum in StrUtil.Encodings)
			{
				m_cmbEnc.Items.Add(seiEnum.Name);
			}

			StrEncodingInfo seiGuess = BinaryDataClassifier.GetStringEncoding(
				m_pbData, out m_uStartOffset);

			int iSel = 0;
			if(seiGuess != null)
				iSel = Math.Max(m_cmbEnc.FindStringExact(seiGuess.Name), 0);
			m_cmbEnc.SelectedIndex = iSel;

			string[] vChars = new string[] { ",", ";", ".", ":", "\"", @"'",
				StrCharTab, StrCharNewLine };
			foreach(string strChar in vChars)
			{
				m_cmbFieldSep.Items.Add(strChar);
				m_cmbRecSep.Items.Add(strChar);
				m_cmbTextQual.Items.Add(strChar);
			}
			m_cmbFieldSep.SelectedIndex = 0;
			m_cmbRecSep.SelectedIndex = 7;
			m_cmbTextQual.SelectedIndex = 4;

			m_lvFields.Columns.Add(KPRes.Field);

			AddCsvField(CsvFieldType.Title, null, null);
			AddCsvField(CsvFieldType.UserName, null, null);
			AddCsvField(CsvFieldType.Password, null, null);
			AddCsvField(CsvFieldType.Url, null, null);
			AddCsvField(CsvFieldType.Notes, null, null);

			for(int i = (int)CsvFieldType.First; i < (int)CsvFieldType.Count; ++i)
				m_cmbFieldType.Items.Add(CsvFieldToString((CsvFieldType)i));
			m_cmbFieldType.SelectedIndex = (int)CsvFieldType.Group;

			m_cmbFieldFormat.Text = string.Empty;

			m_bInitializing = false;

			UpdateTextPreview();
			UpdateImportPreview();
			GuessCsvStructure();

			ProcessResize();
			EnableControlsEx();

			UIUtil.SetFocus(m_btnTabNext, this);
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void EnableControlsEx()
		{
			if(m_bInitializing) return;

			List<CsvFieldInfo> lFields = GetCsvFieldInfos();

			bool bSelField = (m_lvFields.SelectedIndices.Count >= 1);
			bool bSel1Field = (m_lvFields.SelectedIndices.Count == 1);
			m_btnFieldDel.Enabled = bSelField;
			m_btnFieldMoveUp.Enabled = bSel1Field;
			m_btnFieldMoveDown.Enabled = bSel1Field;

			bool bFieldName, bFieldFormat;
			CsvFieldType t = GetCsvFieldType(out bFieldName, out bFieldFormat);
			m_lblFieldName.Enabled = bFieldName;
			m_tbFieldName.Enabled = bFieldName;
			m_lblFieldFormat.Enabled = bFieldFormat;
			m_cmbFieldFormat.Enabled = bFieldFormat;
			m_linkFieldFormat.Enabled = IsTimeField(t);

			int iTab = m_tabMain.SelectedIndex, nTabs = m_tabMain.TabCount;
			m_btnTabBack.Enabled = (iTab > 0);
			m_btnTabNext.Enabled = (iTab < (nTabs - 1));

			bool bValidFieldSep = (GetCharFromDef(m_cmbFieldSep.Text) != char.MinValue);
			bool bValidRecSep = (GetCharFromDef(m_cmbRecSep.Text) != char.MinValue);
			bool bValidTextQual = (GetCharFromDef(m_cmbTextQual.Text) != char.MinValue);

			if(bValidFieldSep) m_cmbFieldSep.ResetBackColor();
			else m_cmbFieldSep.BackColor = AppDefs.ColorEditError;
			if(bValidRecSep) m_cmbRecSep.ResetBackColor();
			else m_cmbRecSep.BackColor = AppDefs.ColorEditError;
			if(bValidTextQual) m_cmbTextQual.ResetBackColor();
			else m_cmbTextQual.BackColor = AppDefs.ColorEditError;

			bool bOK = true;
			bOK &= (iTab == (nTabs - 1));
			bOK &= (bValidFieldSep && bValidRecSep && bValidTextQual);
			m_btnOK.Enabled = bOK;

			if(t != m_tLastCsvType)
			{
				m_cmbFieldFormat.Items.Clear();

				string[] vItems;
				if(IsTimeField(t))
					vItems = new string[] {
						string.Empty,
						@"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzz",
						@"ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", @"yyyy'-'MM'-'dd'T'HH':'mm':'ss",
						@"yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
						@"yyyy/MM/dd HH:mm:ss",
						@"yyyy/MM/dd", @"MM/dd/yy", @"MMMM dd, yyyy", @"MM/dd/yy H:mm:ss zzz"
					};
				else if(t == CsvFieldType.Group)
					vItems = new string[] { string.Empty, ".", "/", "\\" };
				else vItems = new string[0];

				foreach(string strPre in vItems)
					m_cmbFieldFormat.Items.Add(strPre);

				if(t == CsvFieldType.Group)
					m_lblFieldFormat.Text = KPRes.Separator + ":";
				else m_lblFieldFormat.Text = KPRes.Format + ":";
			}
			m_tLastCsvType = t;

			bool bHasGroup = false;
			foreach(CsvFieldInfo cfi in lFields)
			{
				if(cfi.Type == CsvFieldType.Group) { bHasGroup = true; break; }
			}
			if(!bHasGroup) m_cbMergeGroups.Checked = false;
			m_cbMergeGroups.Enabled = bHasGroup;
		}

		private string GetDecodedText()
		{
			StrEncodingInfo sei = StrUtil.GetEncoding(m_cmbEnc.Text);
			try
			{
				return (sei.Encoding.GetString(m_pbData, (int)m_uStartOffset,
					m_pbData.Length - (int)m_uStartOffset) ?? string.Empty);
			}
			catch(Exception) { }

			return string.Empty;
		}

		private void UpdateTextPreview()
		{
			if(m_bInitializing) return;

			m_rtbEncPreview.Clear(); // Clear formatting
			m_rtbEncPreview.Text = GetDecodedText();
		}

		private void UpdateImportPreview()
		{
			if(m_bInitializing) return;

			PerformImport(new PwGroup(true, true), true);
		}

		private void ProcessResize()
		{
			if(m_bInitializing) return;

			int dx = m_lvFields.ClientRectangle.Width;
			m_lvFields.Columns[0].Width = dx -
				UIUtil.GetVScrollBarWidth(); // Add some space for usability

			UIUtil.ResizeColumns(m_lvImportPreview, true);
		}

		private static string CsvFieldToString(CsvFieldType t)
		{
			string strText;
			if(t == CsvFieldType.Ignore) strText = "(" + KPRes.Ignore + ")";
			else if(t == CsvFieldType.Group) strText = KPRes.Group;
			else if(t == CsvFieldType.Title) strText = KPRes.Title;
			else if(t == CsvFieldType.UserName) strText = KPRes.UserName;
			else if(t == CsvFieldType.Password) strText = KPRes.Password;
			else if(t == CsvFieldType.Url) strText = KPRes.Url;
			else if(t == CsvFieldType.Notes) strText = KPRes.Notes;
			else if(t == CsvFieldType.CustomString)
				strText = KPRes.String;
			else if(t == CsvFieldType.CreationTime)
				strText = KPRes.CreationTime;
			// else if(t == CsvFieldType.LastAccessTime)
			//	strText = KPRes.LastAccessTime;
			else if(t == CsvFieldType.LastModTime)
				strText = KPRes.LastModificationTime;
			else if(t == CsvFieldType.ExpiryTime)
				strText = KPRes.ExpiryTime;
			else if(t == CsvFieldType.Tags)
				strText = KPRes.Tags;
			else { Debug.Assert(false); strText = KPRes.Unknown; }

			return strText;
		}

		private void AddCsvField(CsvFieldType t, string strName, string strFormat)
		{
			string strText = CsvFieldToString(t);

			string strSub = string.Empty;
			if(strName != null) strSub += strName;
			if(!string.IsNullOrEmpty(strFormat))
			{
				if(strSub.Length > 0) strSub += ", ";
				strSub += strFormat;
			}

			if(strSub.Length > 0) strText += " (" + strSub + ")";

			ListViewItem lvi = m_lvFields.Items.Add(strText);
			lvi.Tag = new CsvFieldInfo(t, strName, strFormat);
		}

		private CsvFieldType GetCsvFieldType()
		{
			bool bName, bFormat;
			return GetCsvFieldType(out bName, out bFormat);
		}

		private CsvFieldType GetCsvFieldType(out bool bName, out bool bFormat)
		{
			int i = m_cmbFieldType.SelectedIndex;
			if((i < (int)CsvFieldType.First) || (i >= (int)CsvFieldType.Count))
			{
				Debug.Assert(false);
				bName = false;
				bFormat = false;
				return CsvFieldType.Ignore;
			}

			CsvFieldType t = (CsvFieldType)i;
			bName = (t == CsvFieldType.CustomString);
			bFormat = (IsTimeField(t) || (t == CsvFieldType.Group));
			return t;
		}

		private char GetCharFromDef(string strDef)
		{
			if(strDef.Equals(StrCharTab, StrUtil.CaseIgnoreCmp))
				return '\t';
			if(strDef.Equals(StrCharNewLine, StrUtil.CaseIgnoreCmp))
				return '\n';
			if(strDef.Length == 1) return strDef[0];
			return char.MinValue;
		}

		private void OnEncSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateTextPreview();
		}

		private void OnFieldSepTextUpdate(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnRecSepTextUpdate(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnTextQualTextUpdate(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnBtnFieldDel(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection lvsic = m_lvFields.SelectedIndices;
			for(int i = lvsic.Count - 1; i >= 0; --i)
				m_lvFields.Items.RemoveAt(lvsic[i]);

			EnableControlsEx();
		}

		private void OnFieldsSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnBtnFieldMoveUp(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection lvsic = m_lvFields.SelectedIndices;
			if(lvsic.Count != 1) { Debug.Assert(false); return; }

			int iPos = lvsic[0];
			if(iPos == 0) return;

			ListViewItem lviMove = m_lvFields.Items[iPos];
			m_lvFields.Items.RemoveAt(iPos);
			m_lvFields.Items.Insert(iPos - 1, lviMove);
		}

		private void OnBtnFieldMoveDown(object sender, EventArgs e)
		{
			ListView.SelectedIndexCollection lvsic = m_lvFields.SelectedIndices;
			if(lvsic.Count != 1) { Debug.Assert(false); return; }

			int iPos = lvsic[0];
			if(iPos == (m_lvFields.Items.Count - 1)) return;

			ListViewItem lviMove = m_lvFields.Items[iPos];
			m_lvFields.Items.RemoveAt(iPos);
			m_lvFields.Items.Insert(iPos + 1, lviMove);
		}

		private void OnFieldTypeSelectedIndexChanged(object sender, EventArgs e)
		{
			CsvFieldType t = GetCsvFieldType();
			if(t != m_tLastCsvType) m_cmbFieldFormat.Text = string.Empty;

			EnableControlsEx();
		}

		private void OnFieldFormatLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			CsvFieldType t = GetCsvFieldType();

			string strUrl = null;
			if(IsTimeField(t))
				strUrl = @"http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx";
			// else if(t == CsvFieldType.Group)
			// {
			//	AppHelp.ShowHelp(AppDefs.HelpTopics.ImportExport,
			//		AppDefs.HelpTopics.ImportExportGenericCsv);
			//	return;
			// }
			else { Debug.Assert(false); return; }

			try { Process.Start(strUrl); }
			catch(Exception ex) { MessageService.ShowWarning(strUrl, ex.Message); }
		}

		private void OnBtnFieldAdd(object sender, EventArgs e)
		{
			bool bName, bFormat;
			CsvFieldType t = GetCsvFieldType(out bName, out bFormat);
			string strName = (bName ? m_tbFieldName.Text : null);
			string strFormat = (bFormat ? m_cmbFieldFormat.Text : null);

			AddCsvField(t, strName, strFormat);
			ProcessResize();
			for(int i = 0; i < (m_lvFields.Items.Count - 1); ++i)
				m_lvFields.Items[i].Selected = false;
			m_lvFields.EnsureVisible(m_lvFields.Items.Count - 1);
			UIUtil.SetFocusedItem(m_lvFields, m_lvFields.Items[
				m_lvFields.Items.Count - 1], true);
		}

		private void OnTabMainSelectedIndexChanged(object sender, EventArgs e)
		{
			if(m_tabMain.SelectedTab == m_tabPreview)
				UpdateImportPreview();

			EnableControlsEx();
		}

		private CsvOptions GetCsvOptions()
		{
			CsvOptions opt = new CsvOptions();
			opt.FieldSeparator = GetCharFromDef(m_cmbFieldSep.Text);
			opt.RecordSeparator = GetCharFromDef(m_cmbRecSep.Text);
			opt.TextQualifier = GetCharFromDef(m_cmbTextQual.Text);
			if((opt.FieldSeparator == char.MinValue) || (opt.RecordSeparator == char.MinValue) ||
				(opt.TextQualifier == char.MinValue))
				return null;
			opt.BackslashIsEscape = m_cbBackEscape.Checked;
			opt.TrimFields = m_cbTrim.Checked;

			return opt;
		}

		private List<CsvFieldInfo> GetCsvFieldInfos()
		{
			List<CsvFieldInfo> lFields = new List<CsvFieldInfo>();

			for(int i = 0; i < m_lvFields.Items.Count; ++i)
			{
				CsvFieldInfo cfi = (m_lvFields.Items[i].Tag as CsvFieldInfo);
				if(cfi == null) { Debug.Assert(false); continue; }
				lFields.Add(cfi);
			}

			return lFields;
		}

		private void PerformImport(PwGroup pgStorage, bool bCreatePreview)
		{
			List<CsvFieldInfo> lFields = GetCsvFieldInfos();

			if(bCreatePreview)
			{
				int dx = m_lvImportPreview.ClientRectangle.Width; // Before clearing

				m_lvImportPreview.Items.Clear();
				m_lvImportPreview.Columns.Clear();

				foreach(CsvFieldInfo cfi in lFields)
				{
					string strCol = CsvFieldToString(cfi.Type);
					if(cfi.Type == CsvFieldType.CustomString)
						strCol = (cfi.Name ?? string.Empty);
					m_lvImportPreview.Columns.Add(strCol, dx / lFields.Count);
				}
			}

			CsvOptions opt = GetCsvOptions();
			if(opt == null) { Debug.Assert(bCreatePreview); return; }

			string strData = GetDecodedText();
			CsvStreamReaderEx csr = new CsvStreamReaderEx(strData, opt);

			Dictionary<string, PwGroup> dGroups = new Dictionary<string, PwGroup>();
			dGroups[string.Empty] = pgStorage;

			if(bCreatePreview) m_lvImportPreview.BeginUpdate();

			DateTime dtNow = DateTime.Now;
			DateTime dtNoExpire = KdbTime.NeverExpireTime.ToDateTime();
			bool bIgnoreFirstRow = m_cbIgnoreFirst.Checked;
			bool bIsFirstRow = true;
			bool bMergeGroups = m_cbMergeGroups.Checked;

			while(true)
			{
				string[] v = csr.ReadLine();
				if(v == null) break;
				if(v.Length == 0) continue;
				if((v.Length == 1) && (v[0].Length == 0)) continue;

				if(bIsFirstRow && bIgnoreFirstRow)
				{
					bIsFirstRow = false;
					continue;
				}
				bIsFirstRow = false;

				PwGroup pg = pgStorage;
				PwEntry pe = new PwEntry(true, true);

				ListViewItem lvi = null;
				for(int i = 0; i < Math.Min(v.Length, lFields.Count); ++i)
				{
					string strField = v[i];
					CsvFieldInfo cfi = lFields[i];

					if(cfi.Type == CsvFieldType.Ignore) { }
					else if(cfi.Type == CsvFieldType.Group)
						pg = FindCreateGroup(strField, pgStorage, dGroups,
							cfi.Format, opt, bMergeGroups);
					else if(cfi.Type == CsvFieldType.Title)
						ImportUtil.AppendToField(pe, PwDefs.TitleField,
							strField, m_pwDatabase);
					else if(cfi.Type == CsvFieldType.UserName)
						ImportUtil.AppendToField(pe, PwDefs.UserNameField,
							strField, m_pwDatabase);
					else if(cfi.Type == CsvFieldType.Password)
						ImportUtil.AppendToField(pe, PwDefs.PasswordField,
							strField, m_pwDatabase);
					else if(cfi.Type == CsvFieldType.Url)
						ImportUtil.AppendToField(pe, PwDefs.UrlField,
							strField, m_pwDatabase);
					else if(cfi.Type == CsvFieldType.Notes)
						ImportUtil.AppendToField(pe, PwDefs.NotesField,
							strField, m_pwDatabase);
					else if(cfi.Type == CsvFieldType.CustomString)
						ImportUtil.AppendToField(pe, (string.IsNullOrEmpty(cfi.Name) ?
							PwDefs.NotesField : cfi.Name), strField, m_pwDatabase);
					else if(cfi.Type == CsvFieldType.CreationTime)
						pe.CreationTime = ParseDateTime(ref strField, cfi, dtNow);
					// else if(cfi.Type == CsvFieldType.LastAccessTime)
					//	pe.LastAccessTime = ParseDateTime(ref strField, cfi, dtNow);
					else if(cfi.Type == CsvFieldType.LastModTime)
						pe.LastModificationTime = ParseDateTime(ref strField, cfi, dtNow);
					else if(cfi.Type == CsvFieldType.ExpiryTime)
					{
						bool bParseSuccess;
						pe.ExpiryTime = ParseDateTime(ref strField, cfi, dtNow,
							out bParseSuccess);
						pe.Expires = (bParseSuccess && (pe.ExpiryTime != dtNoExpire));
					}
					else if(cfi.Type == CsvFieldType.Tags)
					{
						List<string> lTags = StrUtil.StringToTags(strField);
						foreach(string strTag in lTags)
							pe.AddTag(strTag);
					}
					else { Debug.Assert(false); }

					if(bCreatePreview)
					{
						strField = StrUtil.MultiToSingleLine(strField);

						if(lvi != null) lvi.SubItems.Add(strField);
						else lvi = m_lvImportPreview.Items.Add(strField);
					}
				}

				if(bCreatePreview)
				{
					// Create remaining subitems
					for(int r = v.Length; r < lFields.Count; ++r)
					{
						if(lvi != null) lvi.SubItems.Add(string.Empty);
						else lvi = m_lvImportPreview.Items.Add(string.Empty);
					}
				}

				pg.AddEntry(pe, true);
			}

			if(bCreatePreview)
			{
				m_lvImportPreview.EndUpdate();
				ProcessResize();
			}
		}

		private static DateTime ParseDateTime(ref string strData, CsvFieldInfo cfi,
			DateTime dtDefault)
		{
			bool bDummy;
			return ParseDateTime(ref strData, cfi, dtDefault, out bDummy);
		}

		private static DateTime ParseDateTime(ref string strData, CsvFieldInfo cfi,
			DateTime dtDefault, out bool bSuccess)
		{
			DateTime? odt = null;

			if(!string.IsNullOrEmpty(cfi.Format))
			{
				const DateTimeStyles dts = (DateTimeStyles.AllowWhiteSpaces |
					DateTimeStyles.AssumeLocal);

				DateTime dtExact;
				if(DateTime.TryParseExact(strData, cfi.Format, null, dts,
					out dtExact))
					odt = dtExact;
			}

			if(!odt.HasValue)
			{
				DateTime dtStd;
				if(DateTime.TryParse(strData, out dtStd))
					odt = dtStd;
			}

			if(odt.HasValue)
			{
				strData = TimeUtil.ToDisplayString(odt.Value);
				bSuccess = true;
			}
			else
			{
				strData = string.Empty;
				bSuccess = false;

				odt = dtDefault;
			}

			return odt.Value;
		}

		private static List<string> SplitGroupPath(string strSpec, string strSep,
			CsvOptions opt)
		{
			List<string> l = new List<string>();

			if(string.IsNullOrEmpty(strSep)) l.Add(strSpec);
			else
			{
				string[] vChain = strSpec.Split(new string[1] { strSep },
					StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < vChain.Length; ++i)
				{
					string strGrp = vChain[i];
					if(opt.TrimFields) strGrp = strGrp.Trim();
					if(strGrp.Length > 0) l.Add(strGrp);
				}
				if(l.Count == 0) l.Add(strSpec);
			}

			return l;
		}

		private static string AssembleGroupPath(List<string> l, int iOffset,
			char chSep)
		{
			StringBuilder sb = new StringBuilder();

			for(int i = iOffset; i < l.Count; ++i)
			{
				if(i > iOffset) sb.Append(chSep);
				sb.Append(l[i]);
			}

			return sb.ToString();
		}

		private static PwGroup FindCreateGroup(string strSpec, PwGroup pgStorage,
			Dictionary<string, PwGroup> dRootGroups, string strSep, CsvOptions opt,
			bool bMergeGroups)
		{
			List<string> l = SplitGroupPath(strSpec, strSep, opt);

			if(bMergeGroups)
			{
				char chSep = StrUtil.GetUnusedChar(strSpec);
				string strPath = AssembleGroupPath(l, 0, chSep);

				return pgStorage.FindCreateSubTree(strPath, new char[1] { chSep });
			}

			string strRootSub = l[0];
			if(!dRootGroups.ContainsKey(strRootSub))
			{
				PwGroup pgNew = new PwGroup(true, true);
				pgNew.Name = strRootSub;
				pgStorage.AddGroup(pgNew, true);
				dRootGroups[strRootSub] = pgNew;
			}
			PwGroup pg = dRootGroups[strRootSub];

			if(l.Count > 1)
			{
				char chSep = StrUtil.GetUnusedChar(strSpec);
				string strSubPath = AssembleGroupPath(l, 1, chSep);

				pg = pg.FindCreateSubTree(strSubPath, new char[1] { chSep });
			}

			return pg;
		}

		private void OnBtnTabBack(object sender, EventArgs e)
		{
			int i = m_tabMain.SelectedIndex;
			if(i > 0) m_tabMain.SelectedIndex = i - 1;
		}

		private void OnBtnTabNext(object sender, EventArgs e)
		{
			int i = m_tabMain.SelectedIndex;
			if(i < (m_tabMain.TabCount - 1)) m_tabMain.SelectedIndex = i + 1;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			PerformImport(m_pwDatabase.RootGroup, false);
		}

		private void OnFieldSepSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnRecSepSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnTextQualSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void GuessCsvStructure()
		{
			bool bFieldsGuessed = GuessFieldTypes();
			m_cbIgnoreFirst.Checked = bFieldsGuessed;
		}

		private bool GuessFieldTypes()
		{
			CsvOptions opt = GetCsvOptions();
			if(opt == null) { Debug.Assert(false); return false; }

			string strData = GetDecodedText();
			CsvStreamReaderEx csv = new CsvStreamReaderEx(strData, opt);

			string[] v;
			while(true)
			{
				v = csv.ReadLine();
				if(v == null) return false;
				if(v.Length == 0) continue;
				if((v.Length == 1) && (v[0].Length == 0)) continue;
				break;
			}
			if(v.Length <= 3) return false;

			CsvFieldInfo[] vFields = new CsvFieldInfo[v.Length];
			int nDetermined = 0;
			for(int i = 0; i < v.Length; ++i)
			{
				CsvFieldInfo fi = GuessFieldType(v[i]);
				if(fi != null) ++nDetermined;
				else fi = new CsvFieldInfo(CsvFieldType.Ignore, null, null);

				vFields[i] = fi;
			}

			// Accept the guesses only if at least half of them are
			// probably correct
			if(nDetermined < (v.Length + 1) / 2) return false;

			m_lvFields.Items.Clear();
			foreach(CsvFieldInfo fi in vFields)
				AddCsvField(fi.Type, fi.Name, fi.Format);

			return true;
		}

		private static CsvFieldInfo GuessFieldType(string strRawName)
		{
			if(strRawName == null) return null;
			string strName = strRawName.Trim();
			if(strName.Length == 0) return null;

			string str = ImportUtil.MapNameToStandardField(strName, false);
			if(str == PwDefs.TitleField)
				return new CsvFieldInfo(CsvFieldType.Title, null, null);
			if(str == PwDefs.UserNameField)
				return new CsvFieldInfo(CsvFieldType.UserName, null, null);
			if(str == PwDefs.PasswordField)
				return new CsvFieldInfo(CsvFieldType.Password, null, null);
			if(str == PwDefs.UrlField)
				return new CsvFieldInfo(CsvFieldType.Url, null, null);
			if(str == PwDefs.NotesField)
				return new CsvFieldInfo(CsvFieldType.Notes, null, null);

			string[] vGroupNames = new string[] {
				"Password Groups", "Group", "Groups", "Group Tree", KPRes.Group
			};
			foreach(string strGroupName in vGroupNames)
			{
				if(strName.Equals(strGroupName, StrUtil.CaseIgnoreCmp))
					return new CsvFieldInfo(CsvFieldType.Group, null, null);
			}

			string[] vCreationNames = new string[] {
				"Creation", "Creation Time",
				KPRes.CreationTime
			};
			foreach(string strCreation in vCreationNames)
			{
				if(strName.Equals(strCreation, StrUtil.CaseIgnoreCmp))
					return new CsvFieldInfo(CsvFieldType.CreationTime, null, null);
			}

			// string[] vLastAccess = new string[] {
			//	"Last Access", "Last Access Time",
			//	KPRes.LastAccessTime
			// };
			// foreach(string strLastAccess in vLastAccess)
			// {
			//	if(strName.Equals(strLastAccess, StrUtil.CaseIgnoreCmp))
			//		return new CsvFieldInfo(CsvFieldType.LastAccessTime, null, null);
			// }

			string[] vLastMod = new string[] {
				"Last Modification", "Last Mod", "Last Modification Time",
				"Last Mod Time",
				KPRes.LastModificationTime
			};
			foreach(string strLastMod in vLastMod)
			{
				if(strName.Equals(strLastMod, StrUtil.CaseIgnoreCmp))
					return new CsvFieldInfo(CsvFieldType.LastModTime, null, null);
			}

			string[] vExpiry = new string[] {
				"Expires", "Expire", "Expiry", "Expiry Time",
				KPRes.ExpiryTime, KPRes.ExpiryTimeDateOnly
			};
			foreach(string strExpire in vExpiry)
			{
				if(strName.Equals(strExpire, StrUtil.CaseIgnoreCmp))
					return new CsvFieldInfo(CsvFieldType.ExpiryTime, null, null);
			}

			string[] vTags = new string[] {
				"Tags", "Tag", KPRes.Tags, KPRes.Tag
			};
			foreach(string strTag in vTags)
			{
				if(strName.Equals(strTag, StrUtil.CaseIgnoreCmp))
					return new CsvFieldInfo(CsvFieldType.Tags, null, null);
			}

			return null;
		}

		private static bool IsTimeField(CsvFieldType t)
		{
			return ((t == CsvFieldType.CreationTime) ||
				// (t == CsvFieldType.LastAccessTime) ||
				(t == CsvFieldType.LastModTime) || (t == CsvFieldType.ExpiryTime));
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.ImportExport,
				AppDefs.HelpTopics.ImportExportGenericCsv);
		}
	}
}
