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
using System.Diagnostics;
using System.Reflection;

using KeePass.UI;

namespace TrlUtil
{
	public sealed class PreviewForm : Form
	{
		private static Random m_rand = new Random();

		public PreviewForm()
		{
			try { this.DoubleBuffered = true; }
			catch(Exception) { Debug.Assert(false); }
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape) UIUtil.SetHandled(e, true);
			else base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape) UIUtil.SetHandled(e, true);
			else base.OnKeyUp(e);
		}

		public void CopyForm(Form f)
		{
			this.SuspendLayout();

			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;

			this.Controls.Clear();

			this.ClientSize = f.ClientSize;
			this.Text = f.Text;

			CopyChildControls(this, f);

			this.ResumeLayout(true);
		}

		private void CopyChildControls(Control cDest, Control cSource)
		{
			if((cDest == null) || (cSource == null)) return;

			foreach(Control c in cSource.Controls)
			{
				Type t = c.GetType();
				Control cCopy;
				bool bCopyChilds = true;

				if(t == typeof(Button)) cCopy = new Button();
				else if(t == typeof(Label))
				{
					cCopy = new Label();
					(cCopy as Label).AutoSize = (c as Label).AutoSize;
				}
				else if(t == typeof(CheckBox)) cCopy = new CheckBox();
				else if(t == typeof(RadioButton)) cCopy = new RadioButton();
				else if(t == typeof(GroupBox)) cCopy = new GroupBox();
				// NumericUpDown leads to GDI objects leak
				// else if(t == typeof(NumericUpDown)) cCopy = new NumericUpDown();
				else if(t == typeof(Panel)) cCopy = new Panel();
				else if(t == typeof(TabControl)) cCopy = new TabControl();
				else if(t == typeof(TabPage)) cCopy = new TabPage();
				else if(t == typeof(ComboBox))
				{
					cCopy = new ComboBox();
					(cCopy as ComboBox).DropDownStyle = (c as ComboBox).DropDownStyle;
				}
				else if(t == typeof(PromptedTextBox))
				{
					cCopy = new PromptedTextBox();
					(cCopy as PromptedTextBox).Multiline = (c as PromptedTextBox).Multiline;
				}
				else if(t == typeof(TextBox))
				{
					cCopy = new TextBox();
					(cCopy as TextBox).Multiline = (c as TextBox).Multiline;
				}
				else if((t == typeof(RichTextBox)) || // RTB leads to GDI objects leak
					(t == typeof(CustomRichTextBoxEx)))
				{
					cCopy = new TextBox();
					(cCopy as TextBox).Multiline = true;
				}
				else
				{
					cCopy = new Label();
					bCopyChilds = false;
				}

				Color clr = Color.FromArgb(128 + m_rand.Next(0, 128),
					128 + m_rand.Next(0, 128), 128 + m_rand.Next(0, 128));

				cCopy.Name = c.Name;
				cCopy.Font = c.Font;
				cCopy.BackColor = clr;
				cCopy.Text = c.Text;

				Type tCopy = cCopy.GetType();
				PropertyInfo piAutoSizeSrc = t.GetProperty("AutoSize", typeof(bool));
				PropertyInfo piAutoSizeDst = tCopy.GetProperty("AutoSize", typeof(bool));
				if((piAutoSizeSrc != null) && (piAutoSizeDst != null))
				{
					MethodInfo miSrc = piAutoSizeSrc.GetGetMethod();
					MethodInfo miDst = piAutoSizeDst.GetSetMethod();
					miDst.Invoke(cCopy, new object[] { miSrc.Invoke(c, null) });
				}

				try
				{
					cDest.Controls.Add(cCopy);

					if(bCopyChilds) CopyChildControls(cCopy, c);
				}
				catch(Exception) { Debug.Assert(false); }

				cCopy.Left = c.Left;
				cCopy.Top = c.Top;
				cCopy.Width = c.Width;
				cCopy.Height = c.Height;
				cCopy.ClientSize = c.ClientSize;
				if(c.Dock != DockStyle.None) cCopy.Dock = c.Dock;
			}
		}
	}
}
