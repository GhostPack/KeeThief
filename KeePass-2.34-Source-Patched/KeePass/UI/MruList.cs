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
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.UI
{
	/// <summary>
	/// MRU handler interface. An MRU handler must support executing an MRU
	/// item and clearing the MRU list.
	/// </summary>
	public interface IMruExecuteHandler
	{
		/// <summary>
		/// Function that is called when an MRU item is executed (i.e. when
		/// the user has clicked the menu item).
		/// </summary>
		void OnMruExecute(string strDisplayName, object oTag,
			ToolStripMenuItem tsmiParent);

		/// <summary>
		/// Function to clear the MRU (for example all menu items must be
		/// removed from the menu).
		/// </summary>
		void OnMruClear();
	}

	public sealed class MruList
	{
		private List<KeyValuePair<string, object>> m_vItems =
			new List<KeyValuePair<string, object>>();

		private IMruExecuteHandler m_handler = null;
		private List<ToolStripMenuItem> m_lContainers =
			new List<ToolStripMenuItem>();

		private ToolStripMenuItem m_tsmiClear = null;
		private List<ToolStripMenuItem> m_lMruMenuItems =
			new List<ToolStripMenuItem>();

		private enum MruMenuItemType
		{
			None = 0,
			Item,
			Clear
		}

		// private Font m_fItalic = null;

		private uint m_uMaxItemCount = 0;
		public uint MaxItemCount
		{
			get { return m_uMaxItemCount; }
			set { m_uMaxItemCount = value; }
		}

		private bool m_bMarkOpened = false;
		public bool MarkOpened
		{
			get { return m_bMarkOpened; }
			set { m_bMarkOpened = value; }
		}

		public uint ItemCount
		{
			get { return (uint)m_vItems.Count; }
		}

		public bool IsValid
		{
			get { return (m_handler != null); }
		}

		public MruList()
		{
		}

		public void Initialize(IMruExecuteHandler handler,
			params ToolStripMenuItem[] vContainers)
		{
			Release();

			Debug.Assert(handler != null); // No throw
			m_handler = handler;

			if(vContainers != null)
			{
				foreach(ToolStripMenuItem tsmi in vContainers)
				{
					if(tsmi != null)
					{
						m_lContainers.Add(tsmi);

						tsmi.DropDownOpening += this.OnDropDownOpening;
					}
				}
			}
		}

		public void Release()
		{
			ReleaseMenuItems();

			foreach(ToolStripMenuItem tsmi in m_lContainers)
			{
				tsmi.DropDownOpening -= this.OnDropDownOpening;
			}
			m_lContainers.Clear();

			m_handler = null;
		}

		private void ReleaseMenuItems()
		{
			if(m_tsmiClear != null)
			{
				m_tsmiClear.Click -= this.ClearHandler;
				m_tsmiClear = null;
			}

			foreach(ToolStripMenuItem tsmi in m_lMruMenuItems)
			{
				tsmi.Click -= this.ClickHandler;
			}
			m_lMruMenuItems.Clear();
		}

		public void Clear()
		{
			m_vItems.Clear();
		}

		[Obsolete]
		public void AddItem(string strDisplayName, object oTag, bool bUpdateMenu)
		{
			AddItem(strDisplayName, oTag);
		}

		public void AddItem(string strDisplayName, object oTag)
		{
			Debug.Assert(strDisplayName != null);
			if(strDisplayName == null) throw new ArgumentNullException("strDisplayName");
			// oTag may be null

			bool bExists = false;
			foreach(KeyValuePair<string, object> kvp in m_vItems)
			{
				Debug.Assert(kvp.Key != null);
				if(kvp.Key.Equals(strDisplayName, StrUtil.CaseIgnoreCmp))
				{
					bExists = true;
					break;
				}
			}

			if(bExists) MoveItemToTop(strDisplayName, oTag);
			else
			{
				m_vItems.Insert(0, new KeyValuePair<string, object>(
					strDisplayName, oTag));

				if(m_vItems.Count > m_uMaxItemCount)
					m_vItems.RemoveAt(m_vItems.Count - 1);
			}

			// if(bUpdateMenu) UpdateMenu();
		}

		private void MoveItemToTop(string strName, object oNewTag)
		{
			for(int i = 0; i < m_vItems.Count; ++i)
			{
				if(m_vItems[i].Key.Equals(strName, StrUtil.CaseIgnoreCmp))
				{
					KeyValuePair<string, object> t =
						new KeyValuePair<string, object>(strName, oNewTag);

					m_vItems.RemoveAt(i);
					m_vItems.Insert(0, t);
					return;
				}
			}

			Debug.Assert(false);
		}

		[Obsolete]
		public void UpdateMenu()
		{
		}

		private void UpdateMenu(object oContainer)
		{
			ToolStripMenuItem tsmiContainer = (oContainer as ToolStripMenuItem);
			if(tsmiContainer == null) { Debug.Assert(false); return; }
			if(!m_lContainers.Contains(tsmiContainer)) { Debug.Assert(false); return; }

			tsmiContainer.DropDown.SuspendLayout();

			// Verify that the popup arrow has been drawn (i.e. items existed)
			Debug.Assert(tsmiContainer.DropDownItems.Count > 0);

			ReleaseMenuItems();
			tsmiContainer.DropDownItems.Clear();

			uint uAccessKey = 1, uNull = 0;
			if(m_vItems.Count > 0)
			{
				foreach(KeyValuePair<string, object> kvp in m_vItems)
				{
					AddMenuItem(tsmiContainer, MruMenuItemType.Item,
						StrUtil.EncodeMenuText(kvp.Key), null, kvp.Value,
						true, ref uAccessKey);
				}

				tsmiContainer.DropDownItems.Add(new ToolStripSeparator());

				AddMenuItem(tsmiContainer, MruMenuItemType.Clear, KPRes.ClearMru,
					Properties.Resources.B16x16_EditDelete, null, true, ref uNull);
			}
			else
			{
				AddMenuItem(tsmiContainer, MruMenuItemType.None, "(" +
					KPRes.Empty + ")", null, null, false, ref uNull);
			}

			tsmiContainer.DropDown.ResumeLayout(true);
		}

		private void AddMenuItem(ToolStripMenuItem tsmiParent, MruMenuItemType t,
			string strText, Image img, object oTag, bool bEnabled,
			ref uint uAccessKey)
		{
			ToolStripMenuItem tsmi = CreateMenuItem(t, strText, img, oTag,
				bEnabled, uAccessKey);
			tsmiParent.DropDownItems.Add(tsmi);

			if(t == MruMenuItemType.Item)
				m_lMruMenuItems.Add(tsmi);
			else if(t == MruMenuItemType.Clear)
			{
				Debug.Assert(m_tsmiClear == null);
				m_tsmiClear = tsmi;
			}

			if(uAccessKey != 0) ++uAccessKey;
		}

		private ToolStripMenuItem CreateMenuItem(MruMenuItemType t, string strText,
			Image img, object oTag, bool bEnabled, uint uAccessKey)
		{
			string strItem = strText;
			if(uAccessKey >= 1)
			{
				NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
				if(uAccessKey < 10)
					strItem = @"&" + uAccessKey.ToString(nfi) + " " + strItem;
				else if(uAccessKey == 10)
					strItem = @"1&0 " + strItem;
				else strItem = uAccessKey.ToString(nfi) + " " + strItem;
			}

			ToolStripMenuItem tsmi = new ToolStripMenuItem(strItem);
			if(img != null) tsmi.Image = img;
			if(oTag != null) tsmi.Tag = oTag;

			IOConnectionInfo ioc = (oTag as IOConnectionInfo);
			if(m_bMarkOpened && (ioc != null) && (Program.MainForm != null))
			{
				foreach(PwDatabase pd in Program.MainForm.DocumentManager.GetOpenDatabases())
				{
					if(pd.IOConnectionInfo.GetDisplayName().Equals(
						ioc.GetDisplayName(), StrUtil.CaseIgnoreCmp))
					{
						// if(m_fItalic == null)
						// {
						//	Font f = tsi.Font;
						//	if(f != null)
						//		m_fItalic = FontUtil.CreateFont(f, FontStyle.Italic);
						//	else { Debug.Assert(false); }
						// }

						// if(m_fItalic != null) tsmi.Font = m_fItalic;
						// 153, 51, 153
						tsmi.ForeColor = Color.FromArgb(64, 64, 255);
						tsmi.Text += " [" + KPRes.Opened + "]";
						break;
					}
				}
			}

			if(t == MruMenuItemType.Item)
				tsmi.Click += this.ClickHandler;
			else if(t == MruMenuItemType.Clear)
				tsmi.Click += this.ClearHandler;
			// t == MruMenuItemType.None needs no handler

			if(!bEnabled) tsmi.Enabled = false;

			return tsmi;
		}

		public KeyValuePair<string, object> GetItem(uint uIndex)
		{
			Debug.Assert(uIndex < (uint)m_vItems.Count);
			if(uIndex >= (uint)m_vItems.Count) throw new ArgumentException();

			return m_vItems[(int)uIndex];
		}

		public bool RemoveItem(string strDisplayName)
		{
			Debug.Assert(strDisplayName != null);
			if(strDisplayName == null) throw new ArgumentNullException("strDisplayName");

			for(int i = 0; i < m_vItems.Count; ++i)
			{
				KeyValuePair<string, object> kvp = m_vItems[i];
				if(kvp.Key.Equals(strDisplayName, StrUtil.CaseIgnoreCmp))
				{
					m_vItems.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		private void ClickHandler(object sender, EventArgs args)
		{
			ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
			if(tsmi == null) { Debug.Assert(false); return; }
			Debug.Assert(m_lMruMenuItems.Contains(tsmi));

			ToolStripMenuItem tsmiParent = (tsmi.OwnerItem as ToolStripMenuItem);
			if(tsmiParent == null) { Debug.Assert(false); return; }
			if(!m_lContainers.Contains(tsmiParent)) { Debug.Assert(false); return; }

			if(m_handler == null) { Debug.Assert(false); return; }

			string strName = tsmi.Text;
			object oTag = tsmi.Tag;

			m_handler.OnMruExecute(strName, oTag, tsmiParent);

			// MoveItemToTop(strName);
		}

		private void ClearHandler(object sender, EventArgs e)
		{
			if(m_handler != null) m_handler.OnMruClear();
			else { Debug.Assert(false); }
		}

		private void OnDropDownOpening(object sender, EventArgs e)
		{
			UpdateMenu(sender);
		}
	}
}
