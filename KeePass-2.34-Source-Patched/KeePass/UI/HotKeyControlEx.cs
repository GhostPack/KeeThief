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

// This control is based on the following article:
// http://www.codeproject.com/useritems/hotkeycontrol.asp

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

using KeePass.Resources;

namespace KeePass.UI
{
	public sealed class HotKeyControlEx : TextBox
	{
		private Keys m_kHotKey = Keys.None;
		private Keys m_kModifiers = Keys.None;

		private static List<Keys> m_vNeedNonShiftModifier = new List<Keys>();
		private static List<Keys> m_vNeedNonAltGrModifier = new List<Keys>();

		private ContextMenu m_ctxNone = new ContextMenu();

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Keys HotKey
		{
			get { return m_kHotKey; }
			set { m_kHotKey = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Keys HotKeyModifiers
		{
			get { return m_kModifiers; }
			set { m_kModifiers = value; }
		}

		private bool m_bNoRightModKeys = false;
		[DefaultValue(false)]
		public bool NoRightModKeys
		{
			get { return m_bNoRightModKeys; }
			set { m_bNoRightModKeys = value; }
		}

		private string m_strTextNone = KPRes.None;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TextNone
		{
			get { return m_strTextNone; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strTextNone = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ContextMenu ContextMenu
		{
			get { return m_ctxNone; }
			set { base.ContextMenu = m_ctxNone; }
		}

		// Hot key control is single-line
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Multiline
		{
			get { return base.Multiline; }
			set { base.Multiline = false; }
		}

		public HotKeyControlEx()
		{
			this.ContextMenu = m_ctxNone; // No context menu available
			this.Text = m_strTextNone;

			this.KeyPress += new KeyPressEventHandler(this.OnKeyPressEx);
			this.KeyUp += new KeyEventHandler(this.OnKeyUpEx);
			this.KeyDown += new KeyEventHandler(this.OnKeyDownEx);

			if(m_vNeedNonShiftModifier.Count == 0)
				PopulateModifierLists();
		}

		private static void PopulateModifierLists()
		{
			for(Keys k = Keys.D0; k <= Keys.Z; ++k)
				m_vNeedNonShiftModifier.Add(k);

			for(Keys k = Keys.NumPad0; k <= Keys.NumPad9; ++k)
				m_vNeedNonShiftModifier.Add(k);

			for(Keys k = Keys.Oem1; k <= Keys.OemBackslash; ++k)
				m_vNeedNonShiftModifier.Add(k);

			for(Keys k = Keys.Space; k <= Keys.Home; ++k)
				m_vNeedNonShiftModifier.Add(k);

			m_vNeedNonShiftModifier.Add(Keys.Insert);
			m_vNeedNonShiftModifier.Add(Keys.Help);
			m_vNeedNonShiftModifier.Add(Keys.Multiply);
			m_vNeedNonShiftModifier.Add(Keys.Add);
			m_vNeedNonShiftModifier.Add(Keys.Subtract);
			m_vNeedNonShiftModifier.Add(Keys.Divide);
			m_vNeedNonShiftModifier.Add(Keys.Decimal);
			m_vNeedNonShiftModifier.Add(Keys.Return);
			m_vNeedNonShiftModifier.Add(Keys.Escape);
			m_vNeedNonShiftModifier.Add(Keys.NumLock);
			m_vNeedNonShiftModifier.Add(Keys.Scroll);
			m_vNeedNonShiftModifier.Add(Keys.Pause);

			for(Keys k = Keys.D0; k <= Keys.D9; ++k)
				m_vNeedNonAltGrModifier.Add(k);
		}

		public new void Clear()
		{
			m_kHotKey = Keys.None;
			m_kModifiers = Keys.None;
		}

		private void OnKeyDownEx(object sender, KeyEventArgs e)
		{
			if((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
				ResetHotKey();
			else
			{
				m_kHotKey = e.KeyCode;
				m_kModifiers = e.Modifiers;
				RenderHotKey();
			}
		}

		private void OnKeyUpEx(object sender, KeyEventArgs e)
		{
			if((m_kHotKey == Keys.None) && (Control.ModifierKeys == Keys.None))
				ResetHotKey();
		}

		private void OnKeyPressEx(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if((keyData == Keys.Delete) || (keyData == (Keys.Control | Keys.Delete)))
			{
				ResetHotKey();
				return true;
			}

			if(keyData == (Keys.Shift | Keys.Insert)) // Paste command
				return true; // Not allowed

			if((keyData & Keys.KeyCode) == Keys.F12)
				return true; // Reserved for debugger, see MSDN

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void ResetHotKey()
		{
			m_kHotKey = Keys.None;
			m_kModifiers = Keys.None;
			RenderHotKey();
		}

		public void ResetIfModifierOnly()
		{
			if((m_kHotKey == Keys.None) && (m_kModifiers != Keys.None))
				ResetHotKey();
		}

		public void RenderHotKey()
		{
			if(m_kHotKey == Keys.None)
			{
				this.Text = m_strTextNone;
				return;
			}

			if((m_kHotKey == Keys.LWin) || (m_kHotKey == Keys.RWin))
			{
				ResetHotKey();
				return;
			}

			if(((m_kModifiers == Keys.Shift) || (m_kModifiers == Keys.None)) &&
				m_vNeedNonShiftModifier.Contains(m_kHotKey))
			{
				if(m_kModifiers == Keys.None)
				{
					if(m_vNeedNonAltGrModifier.Contains(m_kHotKey) == false)
						m_kModifiers = (Keys.Alt | Keys.Control);
					else
						m_kModifiers = (Keys.Alt | Keys.Shift);
				}
				else
				{
					m_kHotKey = Keys.None;
					this.Text = ModifiersToString(m_kModifiers) + " + " + KPRes.InvalidKey;
					return;
				}
			}

			if((m_kModifiers == (Keys.Alt | Keys.Control)) &&
				m_vNeedNonAltGrModifier.Contains(m_kHotKey))
			{
				m_kHotKey = Keys.None;
				this.Text = ModifiersToString(m_kModifiers) + " + " + KPRes.InvalidKey;
				return;
			}

			if(m_kModifiers == Keys.None)
			{
				if(m_kHotKey == Keys.None)
				{
					this.Text = m_strTextNone;
					return;
				}
				else
				{
					this.Text = m_kHotKey.ToString();
					return;
				}
			}

			if((m_kHotKey == Keys.Menu) || (m_kHotKey == Keys.ShiftKey) ||
				(m_kHotKey == Keys.ControlKey))
			{
				m_kHotKey = Keys.None;
			}

			this.Text = ModifiersToString(m_kModifiers) + " + " + m_kHotKey.ToString();
		}

		private string ModifiersToString(Keys kModifiers)
		{
			string str = string.Empty;

			if((kModifiers & Keys.Shift) != Keys.None)
				str = (m_bNoRightModKeys ? KPRes.KeyboardKeyShiftLeft : KPRes.KeyboardKeyShift);
			if((kModifiers & Keys.Control) != Keys.None)
			{
				if(str.Length > 0) str += ", ";
				str += (m_bNoRightModKeys ? KPRes.KeyboardKeyCtrlLeft : KPRes.KeyboardKeyCtrl);
			}
			if((kModifiers & Keys.Alt) != Keys.None)
			{
				if(str.Length > 0) str += ", ";
				str += KPRes.KeyboardKeyAlt;
			}

			return str;
		}

		[Obsolete]
		public static HotKeyControlEx ReplaceTextBox(Control cContainer, TextBox tb)
		{
			return ReplaceTextBox(cContainer, tb, false);
		}

		public static HotKeyControlEx ReplaceTextBox(Control cContainer, TextBox tb,
			bool bNoRightModKeys)
		{
			Debug.Assert(tb != null); if(tb == null) throw new ArgumentNullException("tb");
			tb.Enabled = false;
			tb.Visible = false;
			cContainer.Controls.Remove(tb);

			HotKeyControlEx hk = new HotKeyControlEx();
			hk.Location = tb.Location;
			hk.Size = tb.Size;
			hk.NoRightModKeys = bNoRightModKeys;

			cContainer.Controls.Add(hk);
			cContainer.PerformLayout();

			return hk;
		}
	}
}
