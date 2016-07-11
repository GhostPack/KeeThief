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
using System.Threading;
using System.Diagnostics;

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public delegate void AceColumnDelegate(AceColumn c);
	// public delegate void UpdateUIDelegate(bool bGuiToInternal);

	public partial class ColumnsForm : Form
	{
		private bool m_bIgnoreHideCheckEvent = false;

		public ColumnsForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_View_Detailed,
				KPRes.ConfigureColumns, KPRes.ConfigureColumnsDesc);
			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.ConfigureColumns;

			float fWidth = (float)(m_lvColumns.ClientRectangle.Width -
				UIUtil.GetVScrollBarWidth()) / 5.0f;
			m_lvColumns.Columns.Add(KPRes.Column, (int)(fWidth * 3.0f));
			m_lvColumns.Columns.Add(KPRes.Asterisks + " ***", (int)fWidth);
			m_lvColumns.Columns.Add(KPRes.Toggle + " ***", (int)fWidth);

			UIUtil.SetExplorerTheme(m_lvColumns, false);

			UpdateColumnPropInfo();

			ThreadPool.QueueUserWorkItem(new WaitCallback(FillColumnsList));
		}

		private void AddAceColumnTh(AceColumn c)
		{
			string strLvgName = KPRes.StandardFields;
			if(c.Type == AceColumnType.CustomString) strLvgName = KPRes.CustomFields;
			else if(c.Type == AceColumnType.PluginExt) strLvgName = KPRes.PluginProvided;
			else if((int)c.Type > (int)AceColumnType.PluginExt)
				strLvgName = KPRes.More;

			ListViewGroup lvgContainer = null;
			foreach(ListViewGroup lvg in m_lvColumns.Groups)
			{
				if(lvg.Header == strLvgName) { lvgContainer = lvg; break; }
			}
			if(lvgContainer == null)
			{
				lvgContainer = new ListViewGroup(strLvgName);
				m_lvColumns.Groups.Add(lvgContainer);
			}

			ListViewItem lvi = new ListViewItem(c.GetDisplayName());
			lvi.Tag = c;

			lvi.SubItems.Add(c.HideWithAsterisks ? KPRes.Yes : KPRes.No);

			if(c.Type == AceColumnType.Password)
				lvi.SubItems.Add(KPRes.KeyboardKeyCtrl + "+H");
			else if(c.Type == AceColumnType.UserName)
				lvi.SubItems.Add(KPRes.KeyboardKeyCtrl + "+J");
			else lvi.SubItems.Add(string.Empty);

			bool bChecked = false;
			List<AceColumn> lCur = Program.Config.MainWindow.EntryListColumns;
			foreach(AceColumn cCur in lCur)
			{
				if(cCur.Type != c.Type) continue;

				if((c.Type != AceColumnType.CustomString) &&
					(c.Type != AceColumnType.PluginExt))
				{
					bChecked = true;
					break;
				}
				else if((c.Type == AceColumnType.CustomString) &&
					(cCur.CustomName == c.CustomName))
				{
					bChecked = true;
					break;
				}
				else if((c.Type == AceColumnType.PluginExt) &&
					(cCur.CustomName == c.CustomName))
				{
					bChecked = true;
					break;
				}
			}
			lvi.Checked = bChecked;

			lvi.Group = lvgContainer;
			m_lvColumns.Items.Add(lvi);
		}

		private void AddAceColumn(List<AceColumn> lContainer, AceColumn c)
		{
			m_lvColumns.Invoke(new AceColumnDelegate(AddAceColumnTh), c);
			lContainer.Add(c);
		}

		private void AddStdAceColumn(List<AceColumn> lContainer, AceColumnType colType)
		{
			bool bHide = (colType == AceColumnType.Password); // Passwords hidden by default
			int nWidth = -1;
			List<AceColumn> lCur = Program.Config.MainWindow.EntryListColumns;
			foreach(AceColumn cCur in lCur)
			{
				if(cCur.Type == colType)
				{
					bHide = cCur.HideWithAsterisks;
					nWidth = cCur.Width;
					break;
				}
			}

			AddAceColumn(lContainer, new AceColumn(colType, string.Empty, bHide, nWidth));
		}

		private void FillColumnsList(object state)
		{
			List<AceColumn> l = new List<AceColumn>();

			AddStdAceColumn(l, AceColumnType.Title);
			AddStdAceColumn(l, AceColumnType.UserName);
			AddStdAceColumn(l, AceColumnType.Password);
			AddStdAceColumn(l, AceColumnType.Url);
			AddStdAceColumn(l, AceColumnType.Notes);
			AddStdAceColumn(l, AceColumnType.CreationTime);
			
			if((Program.Config.UI.UIFlags & (ulong)AceUIFlags.ShowLastAccessTime) != 0)
				AddStdAceColumn(l, AceColumnType.LastAccessTime);

			AddStdAceColumn(l, AceColumnType.LastModificationTime);
			AddStdAceColumn(l, AceColumnType.ExpiryTime);
			AddStdAceColumn(l, AceColumnType.Uuid);
			AddStdAceColumn(l, AceColumnType.Attachment);

			SortedDictionary<string, AceColumn> d =
				new SortedDictionary<string, AceColumn>(StrUtil.CaseIgnoreComparer);
			List<AceColumn> lCur = Program.Config.MainWindow.EntryListColumns;
			foreach(AceColumn cCur in lCur)
			{
				if((cCur.Type == AceColumnType.CustomString) &&
					!d.ContainsKey(cCur.CustomName))
				{
					d[cCur.CustomName] = new AceColumn(AceColumnType.CustomString,
						cCur.CustomName, cCur.HideWithAsterisks, cCur.Width);
				}
			}

			foreach(PwDocument pwDoc in Program.MainForm.DocumentManager.Documents)
			{
				if(string.IsNullOrEmpty(pwDoc.LockedIoc.Path) && pwDoc.Database.IsOpen)
				{
					EntryHandler eh = delegate(PwEntry pe)
					{
						foreach(KeyValuePair<string, ProtectedString> kvp in pe.Strings)
						{
							if(PwDefs.IsStandardField(kvp.Key)) continue;
							if(d.ContainsKey(kvp.Key)) continue;

							d[kvp.Key] = new AceColumn(AceColumnType.CustomString,
								kvp.Key, kvp.Value.IsProtected, -1);
						}

						return true;
					};

					pwDoc.Database.RootGroup.TraverseTree(TraversalMethod.PreOrder, null, eh);
				}
			}

			foreach(KeyValuePair<string, AceColumn> kvpCustom in d)
			{
				AddAceColumn(l, kvpCustom.Value);
			}

			AddStdAceColumn(l, AceColumnType.Size);
			AddStdAceColumn(l, AceColumnType.AttachmentCount);
			AddStdAceColumn(l, AceColumnType.HistoryCount);
			AddStdAceColumn(l, AceColumnType.OverrideUrl);
			AddStdAceColumn(l, AceColumnType.Tags);
			AddStdAceColumn(l, AceColumnType.ExpiryTimeDateOnly);

			d.Clear();
			// Add active plugin columns (including those of uninstalled plugins)
			foreach(AceColumn cCur in lCur)
			{
				if(cCur.Type != AceColumnType.PluginExt) continue;
				if(d.ContainsKey(cCur.CustomName)) { Debug.Assert(false); continue; }

				d[cCur.CustomName] = new AceColumn(AceColumnType.PluginExt,
					cCur.CustomName, cCur.HideWithAsterisks, cCur.Width);
			}

			// Add unused plugin columns
			string[] vPlgExtNames = Program.ColumnProviderPool.GetColumnNames();
			foreach(string strPlgName in vPlgExtNames)
			{
				if(d.ContainsKey(strPlgName)) continue; // Do not overwrite

				d[strPlgName] = new AceColumn(AceColumnType.PluginExt, strPlgName,
					false, -1);
			}

			foreach(KeyValuePair<string, AceColumn> kvpExt in d)
			{
				AddAceColumn(l, kvpExt.Value);
			}

			// m_lvColumns.Invoke(new UpdateUIDelegate(UpdateListEx), false);
		}

		private void UpdateListEx(bool bGuiToInternal)
		{
			if(bGuiToInternal)
			{
			}
			else // Internal to GUI
			{
				foreach(ListViewItem lvi in m_lvColumns.Items)
				{
					AceColumn c = (lvi.Tag as AceColumn);
					if(c == null) { Debug.Assert(false); continue; }

					string str = (c.HideWithAsterisks ? KPRes.Yes : KPRes.No);
					lvi.SubItems[1].Text = str;
				}
			}
		}

		private void UpdateColumnPropInfo()
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvColumns.SelectedItems;
			if((lvsic == null) || (lvsic.Count != 1) || (lvsic[0] == null))
			{
				m_grpColumn.Text = KPRes.SelectedColumn + ": (" + KPRes.None + ")";
				m_cbHide.Checked = false;
				m_cbHide.Enabled = false;
			}
			else
			{
				ListViewItem lvi = lvsic[0];
				AceColumn c = (lvi.Tag as AceColumn);
				if(c == null) { Debug.Assert(false); return; }

				m_grpColumn.Text = KPRes.SelectedColumn + ": " + c.GetDisplayName();
				m_cbHide.Enabled = true;

				m_bIgnoreHideCheckEvent = true;
				UIUtil.SetChecked(m_cbHide, c.HideWithAsterisks);
				m_bIgnoreHideCheckEvent = false;
			}
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			List<AceColumn> l = Program.Config.MainWindow.EntryListColumns;
			l.Clear();

			foreach(ListViewItem lvi in m_lvColumns.Items)
			{
				if(!lvi.Checked) continue;

				AceColumn c = (lvi.Tag as AceColumn);
				if(c == null) { Debug.Assert(false); continue; }

				l.Add(c);
			}
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnColumnsSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateColumnPropInfo();
		}

		private void OnHideCheckedChanged(object sender, EventArgs e)
		{
			if(m_bIgnoreHideCheckEvent) return;

			bool bChecked = m_cbHide.Checked;
			foreach(ListViewItem lvi in m_lvColumns.SelectedItems)
			{
				AceColumn c = (lvi.Tag as AceColumn);
				if(c == null) { Debug.Assert(false); continue; }

				if((c.Type == AceColumnType.Password) && c.HideWithAsterisks &&
					!AppPolicy.Try(AppPolicyId.UnhidePasswords))
				{
					// Do not change c.HideWithAsterisks
				}
				else c.HideWithAsterisks = bChecked;
			}

			UpdateListEx(false);
		}
	}
}
