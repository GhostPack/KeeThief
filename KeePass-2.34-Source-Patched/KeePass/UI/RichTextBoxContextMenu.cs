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
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.Resources;
using KeePass.Util;

namespace KeePass.UI
{
	public sealed class RichTextBoxContextMenu
	{
		private RichTextBox m_rtb = null;
		private Form m_form = null;
		private ContextMenuStrip m_ctx = null;
		private ToolStripItem[] m_vMenuItems =
			new ToolStripItem[(int)RtbCtxCommands.Count];
		private string m_strCurLink = string.Empty;

		private enum RtbCtxCommands
		{
			Undo = 0,
			Cut,
			Copy,
			Paste,
			Delete,
			CopyLink,
			CopyAll,
			SelectAll,
			Count
		}

		public RichTextBoxContextMenu()
		{
		}

		~RichTextBoxContextMenu()
		{
			Detach();
		}

		[Obsolete]
		public void Attach(RichTextBox rtb)
		{
			Attach(rtb, null);
		}

		public void Attach(RichTextBox rtb, Form fParent)
		{
			Detach();

			m_rtb = rtb;
			m_form = fParent;

			m_ctx = CreateContextMenu();
			m_ctx.Opening += this.OnMenuOpening;

			GlobalWindowManager.CustomizeControl(m_ctx);

			m_rtb.ContextMenuStrip = m_ctx;
		}

		public void Detach()
		{
			if(m_rtb != null)
			{
				m_rtb.ContextMenuStrip = null;

				m_ctx.Opening -= this.OnMenuOpening;
				m_ctx = null;

				for(int i = 0; i < (int)RtbCtxCommands.Count; ++i)
					m_vMenuItems[i] = null;

				m_rtb = null;
				m_form = null;
			}
		}

		private ContextMenuStrip CreateContextMenu()
		{
			CustomContextMenuStripEx ctx = new CustomContextMenuStripEx();
			int iPos = -1;

			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.Undo,
				Properties.Resources.B16x16_Undo, this.OnUndoCommand);
			ctx.Items.Add(new ToolStripSeparator());

			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.Cut,
				Properties.Resources.B16x16_Cut, this.OnCutCommand);
			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.Copy,
				Properties.Resources.B16x16_EditCopy, this.OnCopyCommand);
			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.Paste,
				Properties.Resources.B16x16_EditPaste, this.OnPasteCommand);
			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.Delete,
				Properties.Resources.B16x16_EditDelete, this.OnDeleteCommand);
			ctx.Items.Add(new ToolStripSeparator());

			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.CopyLink,
				Properties.Resources.B16x16_EditCopyLink, this.OnCopyLinkCommand);
			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.CopyAll,
				Properties.Resources.B16x16_EditShred, this.OnCopyAllCommand);
			m_vMenuItems[++iPos] = ctx.Items.Add(KPRes.SelectAll,
				Properties.Resources.B16x16_Edit, this.OnSelectAllCommand);
			
			Debug.Assert(iPos == ((int)RtbCtxCommands.Count - 1));
			return ctx;
		}

		private void OnMenuOpening(object sender, EventArgs e)
		{
			bool bHasText = (m_rtb.TextLength > 0);
			bool bHasSel = (m_rtb.SelectionLength > 0);

			if(m_rtb.ReadOnly)
			{
				m_vMenuItems[(int)RtbCtxCommands.Undo].Enabled = false;
				m_vMenuItems[(int)RtbCtxCommands.Cut].Enabled = false;
				m_vMenuItems[(int)RtbCtxCommands.Paste].Enabled = false;
				m_vMenuItems[(int)RtbCtxCommands.Delete].Enabled = false;
			}
			else // Editable
			{
				m_vMenuItems[(int)RtbCtxCommands.Undo].Enabled = m_rtb.CanUndo;
				m_vMenuItems[(int)RtbCtxCommands.Cut].Enabled = bHasSel;
				m_vMenuItems[(int)RtbCtxCommands.Paste].Enabled = true; // Optimistic
				m_vMenuItems[(int)RtbCtxCommands.Delete].Enabled = bHasSel;
			}

			m_vMenuItems[(int)RtbCtxCommands.Copy].Enabled = bHasSel;
			m_vMenuItems[(int)RtbCtxCommands.CopyAll].Enabled = bHasText;
			m_vMenuItems[(int)RtbCtxCommands.SelectAll].Enabled = bHasText;

			m_strCurLink = GetLinkBelowMouse();
			m_vMenuItems[(int)RtbCtxCommands.CopyLink].Enabled = (m_strCurLink.Length > 0);
		}

		private string GetLinkBelowMouse()
		{
			// Save selection
			int iSelStart = m_rtb.SelectionStart, iSelLength = m_rtb.SelectionLength;

			string strLink = string.Empty;
			try
			{
				int p = m_rtb.GetCharIndexFromPosition(m_rtb.PointToClient(
					Cursor.Position));
				m_rtb.Select(p, 1);
				if(UIUtil.RtfIsFirstCharLink(m_rtb))
				{
					int l = p;
					while((l - 1) >= 0)
					{
						m_rtb.Select(l - 1, 1);
						if(!UIUtil.RtfIsFirstCharLink(m_rtb)) break;
						--l;
					}

					int r = p, n = m_rtb.TextLength;
					while((r + 1) < n)
					{
						m_rtb.Select(r + 1, 1);
						if(!UIUtil.RtfIsFirstCharLink(m_rtb)) break;
						++r;
					}

					strLink = m_rtb.Text.Substring(l, r - l + 1);
				}
			}
			catch(Exception) { Debug.Assert(false); }

			m_rtb.Select(iSelStart, iSelLength); // Restore selection
			return strLink;
		}

		private void OnUndoCommand(object sender, EventArgs e)
		{
			m_rtb.Undo();
		}

		private void OnCutCommand(object sender, EventArgs e)
		{
			m_rtb.Cut();
		}

		private void OnCopyCommand(object sender, EventArgs e)
		{
			m_rtb.Copy();
		}

		private void OnPasteCommand(object sender, EventArgs e)
		{
			CustomRichTextBoxEx crtb = (m_rtb as CustomRichTextBoxEx);
			if(crtb != null) crtb.PasteAcceptable();
			else m_rtb.Paste();
		}

		private void OnDeleteCommand(object sender, EventArgs e)
		{
			int nStart = m_rtb.SelectionStart, nLength = m_rtb.SelectionLength;
			if((nStart < 0) || (nLength <= 0)) return;
			
			string strText = m_rtb.Text;
			strText = strText.Remove(nStart, nLength);
			m_rtb.Text = strText;

			m_rtb.Select(nStart, 0);
		}

		private void OnCopyLinkCommand(object sender, EventArgs e)
		{
			ClipboardUtil.Copy(m_strCurLink, false, false, null, null,
				(m_form != null) ? m_form.Handle : IntPtr.Zero);
		}

		private void OnCopyAllCommand(object sender, EventArgs e)
		{
			int nStart = m_rtb.SelectionStart, nLength = m_rtb.SelectionLength;
			m_rtb.SelectAll();
			m_rtb.Copy();
			m_rtb.Select(nStart, nLength);
		}

		private void OnSelectAllCommand(object sender, EventArgs e)
		{
			m_rtb.SelectAll();
		}
	}
}
