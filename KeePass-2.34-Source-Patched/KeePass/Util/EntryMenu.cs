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
using System.Diagnostics;

using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public static class EntryMenu
	{
		private static ContextMenuStrip m_ctx = null;

		public static void Show()
		{
			EntryMenu.Show(Cursor.Position.X, Cursor.Position.Y);
		}

		public static void Show(int iPosX, int iPosY)
		{
			EntryMenu.Destroy();

			m_ctx = EntryMenu.Construct();
			m_ctx.Show(iPosX, iPosY);
		}

		public static void Destroy()
		{
			if(m_ctx != null)
			{
				m_ctx.Close();
				m_ctx.Dispose();
				m_ctx = null;
			}
		}

		private static ContextMenuStrip Construct()
		{
			CustomContextMenuStripEx ctx = new CustomContextMenuStripEx();

			// Clone the image list in order to prevent event handlers
			// from the global client icons list to the context menu
			ctx.ImageList = UIUtil.CloneImageList(Program.MainForm.ClientIcons, true);

			bool bAppendSeparator = false;
			foreach(PwDocument ds in Program.MainForm.DocumentManager.Documents)
			{
				if(ds.Database.IsOpen)
				{
					if(bAppendSeparator)
						ctx.Items.Add(new ToolStripSeparator());

					foreach(PwGroup pg in ds.Database.RootGroup.Groups)
					{
						ToolStripMenuItem tsmi = MenuCreateGroup(ds, pg);
						ctx.Items.Add(tsmi);
						MenuProcessGroup(ds, tsmi, pg);
					}

					bAppendSeparator = true;
				}
			}

			GlobalWindowManager.CustomizeControl(ctx);
			return ctx;
		}

		private static ToolStripMenuItem MenuCreateGroup(PwDocument ds,
			PwGroup pg)
		{
			ToolStripMenuItem tsmi = new ToolStripMenuItem();
			tsmi.Text = pg.Name;
			tsmi.ImageIndex = MenuGetImageIndex(ds, pg.IconId, pg.CustomIconUuid);
			return tsmi;
		}

		private static int MenuGetImageIndex(PwDocument ds, PwIcon pwID,
			PwUuid pwCustomID)
		{
			if(!pwCustomID.Equals(PwUuid.Zero) && (ds ==
				Program.MainForm.DocumentManager.ActiveDocument))
			{
				return (int)PwIcon.Count +
					Program.MainForm.DocumentManager.ActiveDatabase.GetCustomIconIndex(
					pwCustomID);
			}

			if((int)pwID < (int)PwIcon.Count) return (int)pwID;

			return (int)PwIcon.Key;
		}

		private static void MenuAddEntry(PwDocument ds, ToolStripMenuItem tsmiContainer,
			PwEntry pe)
		{
			ToolStripMenuItem tsmiEntry = new ToolStripMenuItem();
			string strTitle = pe.Strings.ReadSafe(PwDefs.TitleField);
			string strUser = pe.Strings.ReadSafe(PwDefs.UserNameField);
			string strText = string.Empty;
			if((strTitle.Length > 0) && (strUser.Length > 0))
				strText = strTitle + ": " + strUser;
			else if(strTitle.Length > 0) strText = strTitle;
			else if(strUser.Length > 0) strText = strUser;
			tsmiEntry.Text = strText;
			tsmiEntry.ImageIndex = MenuGetImageIndex(ds, pe.IconId, pe.CustomIconUuid);
			tsmiContainer.DropDownItems.Add(tsmiEntry);

			ToolStripMenuItem tsmi;

			tsmi = new ToolStripMenuItem(KPRes.AutoType);
			tsmi.ImageIndex = (int)PwIcon.Run;
			tsmi.Tag = pe;
			tsmi.Click += OnAutoType;
			tsmi.Enabled = pe.GetAutoTypeEnabled();
			tsmiEntry.DropDownItems.Add(tsmi);

			tsmiEntry.DropDownItems.Add(new ToolStripSeparator());

			tsmi = new ToolStripMenuItem(KPRes.Copy + " " + KPRes.UserName);
			tsmi.ImageIndex = (int)PwIcon.UserKey;
			tsmi.Tag = pe;
			tsmi.Click += OnCopyUserName;
			tsmiEntry.DropDownItems.Add(tsmi);

			tsmi = new ToolStripMenuItem(KPRes.Copy + " " + KPRes.Password);
			tsmi.ImageIndex = (int)PwIcon.Key;
			tsmi.Tag = pe;
			tsmi.Click += OnCopyPassword;
			tsmiEntry.DropDownItems.Add(tsmi);
		}

		private static void MenuProcessGroup(PwDocument ds,
			ToolStripMenuItem tsmiContainer, PwGroup pgSource)
		{
			if((pgSource.Groups.UCount == 0) && (pgSource.Entries.UCount == 0))
				return;

			foreach(PwGroup pg in pgSource.Groups)
			{
				ToolStripMenuItem tsmi = MenuCreateGroup(ds, pg);
				tsmiContainer.DropDownItems.Add(tsmi);
				MenuProcessGroup(ds, tsmi, pg);
			}

			foreach(PwEntry pe in pgSource.Entries)
				MenuAddEntry(ds, tsmiContainer, pe);
		}

		private static void OnAutoType(object sender, EventArgs e)
		{
			ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
			Debug.Assert(tsmi != null); if(tsmi == null) return;
			PwEntry pe = (tsmi.Tag as PwEntry);
			Debug.Assert(pe != null); if(pe == null) return;

			try
			{
				AutoType.PerformIntoCurrentWindow(pe,
					Program.MainForm.DocumentManager.SafeFindContainerOf(pe));
			}
			catch(Exception ex)
			{
				MessageService.ShowWarning(ex);
			}
		}

		private static void OnCopyField(object sender, string strField)
		{
			ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
			Debug.Assert(tsmi != null); if(tsmi == null) return;
			PwEntry pe = (tsmi.Tag as PwEntry);
			Debug.Assert(pe != null); if(pe == null) return;

			ClipboardUtil.Copy(pe.Strings.ReadSafe(strField), true, true,
				pe, Program.MainForm.DocumentManager.SafeFindContainerOf(pe),
				IntPtr.Zero);
		}

		private static void OnCopyUserName(object sender, EventArgs e)
		{
			OnCopyField(sender, PwDefs.UserNameField);
		}

		private static void OnCopyPassword(object sender, EventArgs e)
		{
			OnCopyField(sender, PwDefs.PasswordField);
		}
	}
}
