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

namespace KeePass.Util
{
	public sealed class ClipboardContents
	{
		private string m_strText = null;

		private List<KeyValuePair<string, object>> m_vContents = null;

		public ClipboardContents(bool bGetFromCurrent, bool bSimpleOnly)
		{
			if(bGetFromCurrent) GetData(bSimpleOnly);
		}

		/// <summary>
		/// Create a backup of the current clipboard contents.
		/// </summary>
		/// <param name="bSimpleOnly">Create a simplified backup.
		/// The advanced mode might crash in conjunction with
		/// applications that use clipboard tricks like delay
		/// rendering.</param>
		public void GetData(bool bSimpleOnly)
		{
			try { GetDataPriv(bSimpleOnly); }
			catch(Exception) { Debug.Assert(false); }
		}

		private void GetDataPriv(bool bSimpleOnly)
		{
			if(bSimpleOnly)
			{
				if(ClipboardUtil.ContainsText())
					m_strText = ClipboardUtil.GetText();
			}
			else // Advanced backup
			{
				m_vContents = new List<KeyValuePair<string, object>>();

				IDataObject idoClip = Clipboard.GetDataObject();
				foreach(string strFormat in idoClip.GetFormats())
				{
					KeyValuePair<string, object> kvp =
						new KeyValuePair<string, object>(strFormat,
						idoClip.GetData(strFormat));

					m_vContents.Add(kvp);
				}
			}
		}

		public void SetData()
		{
			try { SetDataPriv(); }
			catch(Exception) { Debug.Assert(false); }
		}

		private void SetDataPriv()
		{
			if(m_strText != null)
				ClipboardUtil.Copy(m_strText, false, false, null, null, IntPtr.Zero);
			else if(m_vContents != null)
			{
				DataObject dObj = new DataObject();
				foreach(KeyValuePair<string, object> kvp in m_vContents)
					dObj.SetData(kvp.Key, kvp.Value);

				ClipboardUtil.Clear();
				Clipboard.SetDataObject(dObj);
			}
		}
	}
}
