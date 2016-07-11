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
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;

using KeePass.Util;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public sealed class CustomRichTextBoxEx : RichTextBox
	{
		private static bool? m_bForceRedrawOnScroll = null;

		private bool m_bSimpleTextOnly = false;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(false)]
		public bool SimpleTextOnly
		{
			get { return m_bSimpleTextOnly; }
			set { m_bSimpleTextOnly = value; }
		}

		private bool m_bCtrlEnterAccepts = false;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(false)]
		public bool CtrlEnterAccepts
		{
			get { return m_bCtrlEnterAccepts; }
			set { m_bCtrlEnterAccepts = value; }
		}

		public CustomRichTextBoxEx() : base()
		{
			// We cannot use EnableAutoDragDrop, because moving some text
			// using drag&drop can remove the selected text from the box
			// (even when read-only is enabled!), which is usually not a
			// good behavior in the case of KeePass;
			// reproducible e.g. with LibreOffice Writer, not WordPad
			// this.EnableAutoDragDrop = true;

			// this.AllowDrop = true;

			// The following is intended to set a default value;
			// also see OnHandleCreated
			this.AutoWordSelection = false;
		}

		private CriticalSectionEx m_csAutoProps = new CriticalSectionEx();
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			// The following operations should not recreate the handle
			if(m_csAutoProps.TryEnter())
			{
				// Workaround for .NET bug:
				// AutoWordSelection remembers the value of the property
				// correctly, but incorrectly sends a message as follows:
				//   SendMessage(EM_SETOPTIONS, value ? ECOOP_OR : ECOOP_XOR,
				//       ECO_AUTOWORDSELECTION);
				// So, when setting AutoWordSelection to false, the control
				// style is toggled instead of turned off (the internal value
				// is updated correctly)
				bool bAutoWord = this.AutoWordSelection; // Internal, no message
				if(!bAutoWord) // Only 'false' needs workaround
				{
					// Ensure control style is on (currently we're in a
					// random state, as it could be set to false multiple
					// times; e.g. base.OnHandleCreated does the following:
					// 'this.AutoWordSelection = this.AutoWordSelection;')
					this.AutoWordSelection = true;

					// Toggle control style to false
					this.AutoWordSelection = false;
				}

				m_csAutoProps.Exit();
			}
			else { Debug.Assert(false); }
		}

		private static bool IsPasteCommand(KeyEventArgs e)
		{
			if(e == null) { Debug.Assert(false); return false; }

			// Also check for modifier keys being up;
			// https://sourceforge.net/p/keepass/bugs/1185/
			if((e.KeyCode == Keys.V) && e.Control && !e.Alt) // e.Shift arb.
				return true;
			if((e.KeyCode == Keys.Insert) && e.Shift && !e.Alt) // e.Control arb.
				return true;

			return false;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(UIUtil.HandleCommonKeyEvent(e, true, this)) return;

			if(m_bSimpleTextOnly && IsPasteCommand(e))
			{
				UIUtil.SetHandled(e, true);

				PasteAcceptable();
				return;
			}

			if(m_bCtrlEnterAccepts && e.Control && ((e.KeyCode == Keys.Return) ||
				(e.KeyCode == Keys.Enter)))
			{
				UIUtil.SetHandled(e, true);
				Debug.Assert(this.Multiline);

				Control p = this;
				Form f;
				while(true)
				{
					f = (p as Form);
					if(f != null) break;

					Control pParent = p.Parent;
					if((pParent == null) || (pParent == p)) break;
					p = pParent;
				}
				if(f != null)
				{
					IButtonControl btn = f.AcceptButton;
					if(btn != null) btn.PerformClick();
					else { Debug.Assert(false); }
				}
				else { Debug.Assert(false); }

				return;
			}

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if(UIUtil.HandleCommonKeyEvent(e, false, this)) return;

			if(m_bSimpleTextOnly && IsPasteCommand(e))
			{
				UIUtil.SetHandled(e, true);
				return;
			}

			if(m_bCtrlEnterAccepts && e.Control && ((e.KeyCode == Keys.Return) ||
				(e.KeyCode == Keys.Enter)))
			{
				UIUtil.SetHandled(e, true);
				return;
			}

			base.OnKeyUp(e);
		}

		public void PasteAcceptable()
		{
			try
			{
				if(!m_bSimpleTextOnly) Paste();
				else if(ClipboardUtil.ContainsData(DataFormats.UnicodeText))
					Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
				else if(ClipboardUtil.ContainsData(DataFormats.Text))
					Paste(DataFormats.GetFormat(DataFormats.Text));
			}
			catch(Exception) { Debug.Assert(false); }
		}

		// //////////////////////////////////////////////////////////////////
		// Drag&Drop Source Support

		/* private Rectangle m_rectDragBox = Rectangle.Empty;
		private bool m_bCurDragSource = false;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if(((e.Button & MouseButtons.Left) == MouseButtons.Left) &&
				(this.SelectionLength > 0))
			{
				Size szDrag = SystemInformation.DragSize;
				m_rectDragBox = new Rectangle(new Point(e.X - (szDrag.Width / 2),
					e.Y - (szDrag.Height / 2)), szDrag);
			}
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			m_rectDragBox = Rectangle.Empty;

			base.OnMouseUp(mevent);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(m_bCurDragSource) { base.OnMouseMove(e); return; }

			if(((e.Button & MouseButtons.Left) == MouseButtons.Left) &&
				!m_rectDragBox.IsEmpty)
			{
				if(!m_rectDragBox.Contains(e.X, e.Y))
				{
					m_rectDragBox = Rectangle.Empty;

					string strText = this.SelectedText;
					// int iSelStart = this.SelectionStart;
					// int iSelLen = this.SelectionLength;

					if(!string.IsNullOrEmpty(strText))
					{
						m_bCurDragSource = true;
						DoDragDrop(strText, (DragDropEffects.Move |
							DragDropEffects.Copy | DragDropEffects.Scroll));
						m_bCurDragSource = false;

						// Select(iSelStart, iSelLen);
						return;
					}
					else { Debug.Assert(false); }
				}
			}

			base.OnMouseMove(e);
		} */

		// //////////////////////////////////////////////////////////////////
		// Drag&Drop Target Support

		/* private static bool ObjectHasText(DragEventArgs e)
		{
			if(e == null) { Debug.Assert(false); return false; }

			IDataObject ido = e.Data;
			if(ido == null)
			{
				Debug.Assert(false);
				return false;
			}

			return (ido.GetDataPresent(DataFormats.Text) ||
				ido.GetDataPresent(DataFormats.UnicodeText));
		}

		private bool IsDropAcceptable(DragEventArgs e)
		{
			if(e == null) { Debug.Assert(false); return false; }

			if(this.ReadOnly) return false;

			// Currently, in-control drag&drop is unsupported
			if(m_bCurDragSource) return false;

			return ObjectHasText(e);
		}

		private bool CustomizeDropEffect(DragEventArgs e)
		{
			if(e == null) { Debug.Assert(false); return false; }

			if(!IsDropAcceptable(e))
			{
				e.Effect = DragDropEffects.None;
				return true;
			}

			return false;
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			base.OnDragEnter(drgevent);
			CustomizeDropEffect(drgevent);
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			base.OnDragOver(drgevent);
			CustomizeDropEffect(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if(drgevent == null) { Debug.Assert(false); return; }
			if(!IsDropAcceptable(drgevent))
			{
				drgevent.Effect = DragDropEffects.None;
				return;
			}

			if(m_bSimpleTextOnly)
			{
				IDataObject d = drgevent.Data;
				string str = null;
				if(d.GetDataPresent(DataFormats.UnicodeText))
					str = (d.GetData(DataFormats.UnicodeText) as string);
				if((str == null) && d.GetDataPresent(DataFormats.Text))
					str = (d.GetData(DataFormats.Text) as string);
				if(str == null)
				{
					Debug.Assert(false);
					drgevent.Effect = DragDropEffects.None;
					return;
				}

				Point pt = new Point(drgevent.X, drgevent.Y);
				pt = PointToClient(pt);

				drgevent.Effect = DragDropEffects.None;

				int pIns = GetCharIndexFromPosition(pt);
				InsertTextAt(str, pIns);
			}
			else base.OnDragDrop(drgevent);
		}

		private void InsertTextAt(string str, int pIns)
		{
			if(string.IsNullOrEmpty(str)) return;

			string strText = this.Text;

			if((pIns < 0) || (pIns > strText.Length)) { Debug.Assert(false); return; }

			this.Text = strText.Insert(pIns, str);

			Select(pIns + str.Length, 0); // Data was inserted, not moved
			ScrollToCaret();
		} */

		// //////////////////////////////////////////////////////////////////
		// Simple Selection Support

		// The following selection code is not required, because
		// AutoWordSelection can be used with a workaround

		// private int? m_oiMouseSelStart = null;

		/* protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if(((e.Button & MouseButtons.Left) != MouseButtons.None) &&
				(this.SelectionLength == 0))
				m_oiMouseSelStart = this.SelectionStart;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(UpdateSelectionEx(e)) return;

			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			if((mevent.Button & MouseButtons.Left) != MouseButtons.None)
				m_oiMouseSelStart = null;

			base.OnMouseUp(mevent);
		}

		private bool UpdateSelectionEx(MouseEventArgs e)
		{
			if(!m_oiMouseSelStart.HasValue) return false;
			if((e.Button & MouseButtons.Left) == MouseButtons.None) return false;

			try
			{
				int iSelS = m_oiMouseSelStart.Value;
				int iSelE = Math.Max(GetCharIndexFromPosition(e.Location), 0);

				if(iSelE == (this.TextLength - 1))
				{
					Font f = this.SelectionFont;
					if(f == null)
					{
						Select(iSelE, 1);
						f = (this.SelectionFont ?? this.Font);
					}

					// Measuring a single character is imprecise (padding, ...)
					Size szLastChar = TextRenderer.MeasureText(new string(
						this.Text[iSelE], 20), f);
					int wLastChar = szLastChar.Width / 20;

					Point ptLastChar = GetPositionFromCharIndex(iSelE);

					if(e.X >= (ptLastChar.X + (wLastChar / 2))) ++iSelE;
				}

				if(iSelS <= iSelE) Select(iSelS, iSelE - iSelS);
				else Select(iSelE, iSelS - iSelE);
			}
			catch(Exception) { Debug.Assert(false); return false; }

			return true;
		} */

		protected override void OnHScroll(EventArgs e)
		{
			base.OnHScroll(e);

			MonoRedrawOnScroll();
		}

		protected override void OnVScroll(EventArgs e)
		{
			base.OnVScroll(e);

			MonoRedrawOnScroll();
		}

		private void MonoRedrawOnScroll()
		{
			if(!m_bForceRedrawOnScroll.HasValue)
				m_bForceRedrawOnScroll = MonoWorkarounds.IsRequired(1366);

			if(m_bForceRedrawOnScroll.Value) Invalidate();
		}
	}
}
