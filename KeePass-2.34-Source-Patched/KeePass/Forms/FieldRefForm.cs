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
using System.Diagnostics;

using KeePass.App;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class FieldRefForm : Form
	{
		private PwGroup m_pgEntrySource = null;
		private ImageList m_ilIcons = null;
		private string m_strDefaultRef = string.Empty;

		private List<KeyValuePair<string, string>> m_vColumns =
			new List<KeyValuePair<string, string>>();

		private string m_strResultRef = string.Empty;
		public string ResultReference
		{
			get { return m_strResultRef; }
		}

		public void InitEx(PwGroup pgEntrySource, ImageList ilClientIcons,
			string strDefaultRef)
		{
			m_pgEntrySource = pgEntrySource;
			m_ilIcons = ilClientIcons;
			m_strDefaultRef = (strDefaultRef ?? string.Empty);
		}

		public FieldRefForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if(m_pgEntrySource == null) { Debug.Assert(false); return; }
			if(m_ilIcons == null) { Debug.Assert(false); return; }

			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;

			UIUtil.SetExplorerTheme(m_lvEntries, true);

			m_vColumns.Add(new KeyValuePair<string, string>(PwDefs.TitleField, KPRes.Title));
			m_vColumns.Add(new KeyValuePair<string, string>(PwDefs.UserNameField, KPRes.UserName));
			m_vColumns.Add(new KeyValuePair<string, string>(PwDefs.UrlField, KPRes.Url));
			m_vColumns.Add(new KeyValuePair<string, string>(PwDefs.NotesField, KPRes.Notes));

			PwObjectList<PwEntry> vEntries = m_pgEntrySource.GetEntries(true);
			UIUtil.CreateEntryList(m_lvEntries, vEntries, m_vColumns, m_ilIcons);

			m_radioIdUuid.Checked = true;

			if(m_strDefaultRef == PwDefs.TitleField)
				m_radioRefTitle.Checked = true;
			else if(m_strDefaultRef == PwDefs.UserNameField)
				m_radioRefUserName.Checked = true;
			// else if(m_strDefaultRef == PwDefs.PasswordField)
			//	m_radioRefPassword.Checked = true;
			else if(m_strDefaultRef == PwDefs.UrlField)
				m_radioRefUrl.Checked = true;
			else if(m_strDefaultRef == PwDefs.NotesField)
				m_radioRefNotes.Checked = true;
			else m_radioRefPassword.Checked = true;
		}

		private void CleanUpEx()
		{
			m_lvEntries.SmallImageList = null; // Detach event handlers
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}

		private PwEntry GetSelectedEntry()
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvEntries.SelectedItems;
			if((lvsic == null) || (lvsic.Count != 1)) return null;

			return (lvsic[0].Tag as PwEntry);
		}

		private bool CreateResultRef()
		{
			PwEntry pe = this.GetSelectedEntry();
			if(pe == null) return false;

			string str = @"{REF:";
			if(m_radioRefTitle.Checked) str += "T";
			else if(m_radioRefUserName.Checked) str += "U";
			else if(m_radioRefPassword.Checked) str += "P";
			else if(m_radioRefUrl.Checked) str += "A";
			else if(m_radioRefNotes.Checked) str += "N";
			else { Debug.Assert(false); return false; }

			str += @"@";

			string strId;
			if(m_radioIdTitle.Checked)
				strId = @"T:" + pe.Strings.ReadSafe(PwDefs.TitleField);
			else if(m_radioIdUserName.Checked)
				strId = @"U:" + pe.Strings.ReadSafe(PwDefs.UserNameField);
			else if(m_radioIdPassword.Checked)
				strId = @"P:" + pe.Strings.ReadSafe(PwDefs.PasswordField);
			else if(m_radioIdUrl.Checked)
				strId = @"A:" + pe.Strings.ReadSafe(PwDefs.UrlField);
			else if(m_radioIdNotes.Checked)
				strId = @"N:" + pe.Strings.ReadSafe(PwDefs.NotesField);
			else if(m_radioIdUuid.Checked)
				strId = @"I:" + pe.Uuid.ToHexString();
			else { Debug.Assert(false); return false; }

			char[] vInvalidChars = new char[] { '{', '}', '\r', '\n' };
			if(strId.IndexOfAny(vInvalidChars) >= 0)
			{
				MessageService.ShowWarning(KPRes.FieldRefInvalidChars);
				return false;
			}

			string strIdData = strId.Substring(2, strId.Length - 2);
			if(IdMatchesMultipleTimes(strIdData, strId[0]))
			{
				MessageService.ShowWarning(KPRes.FieldRefMultiMatch,
					KPRes.FieldRefMultiMatchHint);
				return false;
			}

			str += strId + @"}";

			m_strResultRef = str;
			return true;
		}

		private bool IdMatchesMultipleTimes(string strSearch, char tchField)
		{
			if(m_pgEntrySource == null) { Debug.Assert(false); return false; }

			SearchParameters sp = SearchParameters.None;
			sp.SearchString = strSearch;
			sp.RespectEntrySearchingDisabled = false;

			if(tchField == 'T') sp.SearchInTitles = true;
			else if(tchField == 'U') sp.SearchInUserNames = true;
			else if(tchField == 'P') sp.SearchInPasswords = true;
			else if(tchField == 'A') sp.SearchInUrls = true;
			else if(tchField == 'N') sp.SearchInNotes = true;
			else if(tchField == 'I') sp.SearchInUuids = true;
			else { Debug.Assert(false); return true; }

			PwObjectList<PwEntry> l = new PwObjectList<PwEntry>();
			m_pgEntrySource.SearchEntries(sp, l);

			if(l.UCount == 0) { Debug.Assert(false); return false; }
			else if(l.UCount == 1) return false;

			return true;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(!CreateResultRef()) this.DialogResult = DialogResult.None;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void EnableChildControls()
		{
			m_btnOK.Enabled = (GetSelectedEntry() != null);
		}

		private void OnEntriesSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableChildControls();
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.FieldRefs, null);
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(((keyData == Keys.Return) || (keyData == Keys.Enter)) && m_tbFilter.Focused)
				return false; // Forward to TextBox

			return base.ProcessDialogKey(keyData);
		}

		private void OnFilterKeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
			{
				e.SuppressKeyPress = true;

				SearchParameters sp = new SearchParameters();
				sp.SearchString = m_tbFilter.Text;
				sp.SearchInPasswords = true;

				PwObjectList<PwEntry> lResults = new PwObjectList<PwEntry>();
				m_pgEntrySource.SearchEntries(sp, lResults);

				UIUtil.CreateEntryList(m_lvEntries, lResults, m_vColumns, m_ilIcons);
			}
		}

		private void OnFilterKeyUp(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
				e.SuppressKeyPress = true;
		}
	}
}
