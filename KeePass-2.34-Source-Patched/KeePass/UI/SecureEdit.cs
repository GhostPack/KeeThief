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
using System.Windows.Forms;
using System.Security;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.UI
{
	/// <summary>
	/// Secure edit control class. Supports storing passwords in an encrypted
	/// form in the process memory.
	/// </summary>
	public sealed class SecureEdit
	{
		private static char? m_ochPasswordChar = null;
		internal static char PasswordChar
		{
			get
			{
				if(m_ochPasswordChar.HasValue) return m_ochPasswordChar.Value;

				// On Windows 98 / ME, an ANSI character must be used as
				// password char
				m_ochPasswordChar = ((Environment.OSVersion.Platform ==
					PlatformID.Win32Windows) ? '\u00D7' : '\u25CF');

				return m_ochPasswordChar.Value;
			}
		}

		private TextBox m_tbPassword = null;
		private EventHandler m_evTextChanged = null;

		private static readonly ProtectedString g_psEmpty = new ProtectedString(
			true, new byte[0]);

		// With ProtectedString.Empty no incremental protection (Remove/Insert)
		private ProtectedString m_psText = g_psEmpty;

		private bool m_bBlockTextChanged = false;

		private bool m_bFirstGotFocus = true;

		private bool m_bSecureDesktop = false;
		public bool SecureDesktopMode
		{
			get { return m_bSecureDesktop; }
			set { m_bSecureDesktop = value; }
		}

		public uint TextLength
		{
			get { return (uint)m_psText.Length; }
		}

		/// <summary>
		/// Construct a new <c>SecureEdit</c> object. You must call the
		/// <c>Attach</c> member function to associate the secure edit control
		/// with a text box.
		/// </summary>
		public SecureEdit()
		{
		}

		~SecureEdit()
		{
			Detach();
		}

		/// <summary>
		/// Associate the current secure edit object with a text box.
		/// </summary>
		/// <param name="tbPasswordBox">Text box to link to.</param>
		/// <param name="bHidePassword">Initial protection flag.</param>
		public void Attach(TextBox tbPasswordBox, EventHandler evTextChanged,
			bool bHidePassword)
		{
			Debug.Assert(tbPasswordBox != null);
			if(tbPasswordBox == null) throw new ArgumentNullException("tbPasswordBox");

			Detach();

			m_tbPassword = tbPasswordBox;
			m_evTextChanged = evTextChanged;

			// Initialize to zero-length string
			m_tbPassword.Text = string.Empty;
			Debug.Assert(m_tbPassword.SelectionStart == 0);
			Debug.Assert(m_tbPassword.SelectionLength == 0);

			m_psText = g_psEmpty;

			EnableProtection(bHidePassword);

			if(m_evTextChanged != null) m_evTextChanged(m_tbPassword, EventArgs.Empty);

			if(!m_bSecureDesktop) m_tbPassword.AllowDrop = true;

			// Register event handler
			m_tbPassword.TextChanged += this.OnPasswordTextChanged;
			m_tbPassword.GotFocus += this.OnGotFocus;
			if(!m_bSecureDesktop)
			{
				m_tbPassword.DragEnter += this.OnDragCheck;
				m_tbPassword.DragOver += this.OnDragCheck;
				m_tbPassword.DragDrop += this.OnDragDrop;
			}
		}

		/// <summary>
		/// Remove the current association. You should call this before the
		/// text box is destroyed.
		/// </summary>
		public void Detach()
		{
			if(m_tbPassword != null)
			{
				m_tbPassword.TextChanged -= this.OnPasswordTextChanged;
				m_tbPassword.GotFocus -= this.OnGotFocus;
				if(!m_bSecureDesktop)
				{
					m_tbPassword.DragEnter -= this.OnDragCheck;
					m_tbPassword.DragOver -= this.OnDragCheck;
					m_tbPassword.DragDrop -= this.OnDragDrop;
				}

				m_tbPassword = null;
			}
		}

		public void EnableProtection(bool bEnable)
		{
			if(m_tbPassword == null) { Debug.Assert(false); return; }

			if(!MonoWorkarounds.IsRequired(5795))
			{
				if(bEnable) FontUtil.AssignDefault(m_tbPassword);
				else
				{
					FontUtil.SetDefaultFont(m_tbPassword);
					FontUtil.AssignDefaultMono(m_tbPassword, true);
				}
			}

			if(m_tbPassword.UseSystemPasswordChar == bEnable) return;
			m_tbPassword.UseSystemPasswordChar = bEnable;

			ShowCurrentPassword(-1, -1);
		}

		private void OnPasswordTextChanged(object sender, EventArgs e)
		{
			if(m_tbPassword == null) { Debug.Assert(false); return; }

			if(m_bBlockTextChanged) return;

			int nSelPos = m_tbPassword.SelectionStart;
			int nSelLen = m_tbPassword.SelectionLength;

			if(!m_tbPassword.UseSystemPasswordChar)
			{
				RemoveInsert(0, 0, m_tbPassword.Text);
				ShowCurrentPassword(nSelPos, nSelLen);
				return;
			}

			string strText = m_tbPassword.Text;

			int inxLeft = -1, inxRight = 0;
			StringBuilder sbNewPart = new StringBuilder();

			char chPasswordChar = SecureEdit.PasswordChar;
			for(int i = 0; i < strText.Length; ++i)
			{
				if(strText[i] != chPasswordChar)
				{
					if(inxLeft == -1) inxLeft = i;
					inxRight = i;

					sbNewPart.Append(strText[i]);
				}
			}

			if(inxLeft < 0)
				RemoveInsert(nSelPos, strText.Length - nSelPos, string.Empty);
			else
				RemoveInsert(inxLeft, strText.Length - inxRight - 1,
					sbNewPart.ToString());

			ShowCurrentPassword(nSelPos, nSelLen);

			// Check for m_tbPassword being null from on now; the
			// control might be disposed already (by the user handler
			// triggered by the ShowCurrentPassword call)
			if(m_tbPassword != null)
				m_tbPassword.ClearUndo(); // Would need special undo buffer
		}

		private void ShowCurrentPassword(int nSelStart, int nSelLength)
		{
			if(m_tbPassword == null) { Debug.Assert(false); return; }

			if(nSelStart < 0) nSelStart = m_tbPassword.SelectionStart;
			if(nSelLength < 0) nSelLength = m_tbPassword.SelectionLength;

			m_bBlockTextChanged = true;
			if(!m_tbPassword.UseSystemPasswordChar)
				m_tbPassword.Text = GetAsString();
			else
				m_tbPassword.Text = new string(SecureEdit.PasswordChar,
					m_psText.Length);
			m_bBlockTextChanged = false;

			int nNewTextLen = m_tbPassword.TextLength;
			if(nSelStart < 0) { Debug.Assert(false); nSelStart = 0; }
			if(nSelStart > nNewTextLen) nSelStart = nNewTextLen; // Behind last char
			if(nSelLength < 0) { Debug.Assert(false); nSelLength = 0; }
			if((nSelStart + nSelLength) > nNewTextLen)
				nSelLength = nNewTextLen - nSelStart;

			m_tbPassword.SelectionStart = nSelStart;
			m_tbPassword.SelectionLength = nSelLength;

			if(m_evTextChanged != null) m_evTextChanged(m_tbPassword, EventArgs.Empty);
		}

		public byte[] ToUtf8()
		{
			// Debug.Assert(sizeof(char) == 2);
			// if(m_secString != null)
			// {
			//	char[] vChars = new char[m_secString.Length];
			//	IntPtr p = Marshal.SecureStringToGlobalAllocUnicode(m_secString);
			//	for(int i = 0; i < m_secString.Length; ++i)
			//		vChars[i] = (char)Marshal.ReadInt16(p, i * 2);
			//	Marshal.ZeroFreeGlobalAllocUnicode(p);
			//	byte[] pb = StrUtil.Utf8.GetBytes(vChars);
			//	Array.Clear(vChars, 0, vChars.Length);
			//	return pb;
			// }
			// else return StrUtil.Utf8.GetBytes(m_strAlternativeSecString);

			return m_psText.ReadUtf8();
		}

		private string GetAsString()
		{
			// if(m_secString != null)
			// {
			//	IntPtr p = Marshal.SecureStringToGlobalAllocUnicode(m_secString);
			//	string str = Marshal.PtrToStringUni(p);
			//	Marshal.ZeroFreeGlobalAllocUnicode(p);
			//	return str;
			// }
			// else return m_strAlternativeSecString;

			return m_psText.ReadString();
		}

		private void RemoveInsert(int nLeftRem, int nRightRem, string strInsert)
		{
			Debug.Assert(nLeftRem >= 0);

			// if(m_secString != null)
			// {
			//	while(m_secString.Length > (nLeftRem + nRightRem))
			//		m_secString.RemoveAt(nLeftRem);
			//	for(int i = 0; i < strInsert.Length; ++i)
			//		m_secString.InsertAt(nLeftRem + i, strInsert[i]);
			// }
			// else
			// {
			//	StringBuilder sb = new StringBuilder(m_strAlternativeSecString);
			//	while(sb.Length > (nLeftRem + nRightRem))
			//		sb.Remove(nLeftRem, 1);
			//	sb.Insert(nLeftRem, strInsert);
			//	m_strAlternativeSecString = sb.ToString();
			// }

			try
			{
				int cr = m_psText.Length - (nLeftRem + nRightRem);
				if(cr >= 0) m_psText = m_psText.Remove(nLeftRem, cr);
				else { Debug.Assert(false); }
				Debug.Assert(m_psText.Length == (nLeftRem + nRightRem));
			}
			catch(Exception) { Debug.Assert(false); }

			try { m_psText = m_psText.Insert(nLeftRem, strInsert); }
			catch(Exception) { Debug.Assert(false); }
		}

		public bool ContentsEqualTo(SecureEdit secOther)
		{
			Debug.Assert(secOther != null); if(secOther == null) return false;

			byte[] pbThis = ToUtf8();
			byte[] pbOther = secOther.ToUtf8();

			bool bEqual = MemUtil.ArraysEqual(pbThis, pbOther);

			MemUtil.ZeroByteArray(pbThis);
			MemUtil.ZeroByteArray(pbOther);
			return bEqual;
		}

		public void SetPassword(byte[] pbUtf8)
		{
			Debug.Assert(pbUtf8 != null);
			if(pbUtf8 == null) throw new ArgumentNullException("pbUtf8");

			// if(m_secString != null)
			// {
			//	m_secString.Clear();
			//	char[] vChars = StrUtil.Utf8.GetChars(pbUtf8);
			//	for(int i = 0; i < vChars.Length; ++i)
			//	{
			//		m_secString.AppendChar(vChars[i]);
			//		vChars[i] = char.MinValue;
			//	}
			// }
			// else m_strAlternativeSecString = StrUtil.Utf8.GetString(pbUtf8);

			m_psText = new ProtectedString(true, pbUtf8);

			ShowCurrentPassword(0, 0);
		}

		private void OnGotFocus(object sender, EventArgs e)
		{
			if(m_tbPassword == null) { Debug.Assert(false); return; }

			if(m_bFirstGotFocus)
			{
				m_bFirstGotFocus = false;

				// OnGotFocus is not called when the box initially has the
				// focus; the user can select characters without triggering
				// OnGotFocus, thus we select all characters only if the
				// selection is in its original state (0, 0), otherwise
				// e.g. the selection restoration when hiding/unhiding does
				// not work the first time (because after restoring the
				// selection, we would override it here by selecting all)
				if((m_tbPassword.SelectionStart <= 0) &&
					(m_tbPassword.SelectionLength <= 0))
					m_tbPassword.SelectAll();
			}
		}

		private void OnDragCheck(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(typeof(string)))
				e.Effect = DragDropEffects.Copy;
			else e.Effect = DragDropEffects.None;
		}

		private void OnDragDrop(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(typeof(string)))
			{
				string strData = (e.Data.GetData(typeof(string)) as string);
				if(strData == null) { Debug.Assert(false); return; }

				if(m_tbPassword != null) m_tbPassword.Paste(strData);
			}
		}
	}
}
