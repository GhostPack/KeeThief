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
using System.Diagnostics;

using KeePass.Util.Spr;

using KeePassLib;

namespace KeePass.Util
{
	/// <summary>
	/// Auto-type candidate context.
	/// </summary>
	public sealed class AutoTypeCtx
	{
		private string m_strSeq = string.Empty;
		public string Sequence
		{
			get { return m_strSeq; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strSeq = value;
			}
		}

		private PwEntry m_pe = null;
		public PwEntry Entry
		{
			get { return m_pe; }
			set { m_pe = value; }
		}

		private PwDatabase m_pd = null;
		public PwDatabase Database
		{
			get { return m_pd; }
			set { m_pd = value; }
		}

		public AutoTypeCtx() { }

		public AutoTypeCtx(string strSequence, PwEntry pe, PwDatabase pd)
		{
			if(strSequence == null) throw new ArgumentNullException("strSequence");

			m_strSeq = strSequence;
			m_pe = pe;
			m_pd = pd;
		}

		public AutoTypeCtx Clone()
		{
			return (AutoTypeCtx)this.MemberwiseClone();
		}
	}

	public sealed class SequenceQueriesEventArgs : EventArgs
	{
		private readonly int m_iEventID;
		public int EventID
		{
			get { return m_iEventID; }
		}

		private readonly IntPtr m_h;
		public IntPtr TargetWindowHandle
		{
			get { return m_h; }
		}

		private readonly string m_strWnd;
		public string TargetWindowTitle
		{
			get { return m_strWnd; }
		}

		public SequenceQueriesEventArgs(int iEventID, IntPtr hWnd,
			string strWnd)
		{
			m_iEventID = iEventID;
			m_h = hWnd;
			m_strWnd = strWnd;
		}
	}

	public sealed class SequenceQueryEventArgs : EventArgs
	{
		private readonly int m_iEventID;
		public int EventID
		{
			get { return m_iEventID; }
		}

		private readonly IntPtr m_h;
		public IntPtr TargetWindowHandle
		{
			get { return m_h; }
		}

		private readonly string m_strWnd;
		public string TargetWindowTitle
		{
			get { return m_strWnd; }
		}

		private readonly PwEntry m_pe;
		public PwEntry Entry
		{
			get { return m_pe; }
		}

		private readonly PwDatabase m_pd;
		public PwDatabase Database
		{
			get { return m_pd; }
		}

		private List<string> m_lSeqs = new List<string>();
		internal IEnumerable<string> Sequences
		{
			get { return m_lSeqs; }
		}

		public SequenceQueryEventArgs(int iEventID, IntPtr hWnd, string strWnd,
			PwEntry pe, PwDatabase pd)
		{
			m_iEventID = iEventID;
			m_h = hWnd;
			m_strWnd = strWnd;
			m_pe = pe;
			m_pd = pd;
		}

		public void AddSequence(string strSeq)
		{
			if(strSeq == null) { Debug.Assert(false); return; }

			m_lSeqs.Add(strSeq);
		}
	}
}
