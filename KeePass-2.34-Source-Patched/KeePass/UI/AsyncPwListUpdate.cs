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
using System.Threading;
using System.Diagnostics;

using KeePass.Forms;
using KeePass.Util.Spr;

using KeePassLib;

namespace KeePass.UI
{
	public sealed class PwListItem
	{
		private static long m_idNext = 1;

		private readonly PwEntry m_pe;
		public PwEntry Entry
		{
			get { return m_pe; }
		}

		private readonly long m_id;
		public long ListViewItemID
		{
			get { return m_id; }
		}

		public PwListItem(PwEntry pe)
		{
			if(pe == null) throw new ArgumentNullException("pe");

			m_pe = pe;

			m_id = m_idNext;
			unchecked { ++m_idNext; }
		}
	}

	public delegate string PwTextUpdateDelegate(string strText, PwListItem li);

	public sealed class AsyncPwListUpdate
	{
		private readonly ListView m_lv;

		private object m_objListEditSync = new object();
		public object ListEditSyncObject
		{
			get { return m_objListEditSync; }
		}

		private Dictionary<long, bool> m_dValidIDs = new Dictionary<long, bool>();
		private object m_objValidIDsSync = new object();

		private sealed class LviUpdInfo
		{
			public ListView ListView { get; set; }

			public long UpdateID { get; set; }

			public string Text { get; set; }
			public PwListItem ListItem { get; set; }
			public int IndexHint { get; set; }
			public int SubItem { get; set; }

			public PwTextUpdateDelegate Function { get; set; }

			public object ListEditSyncObject { get; set; }

			public object ValidIDsSyncObject { get; set; }
			public Dictionary<long, bool> ValidIDs { get; set; }
		}

		public AsyncPwListUpdate(ListView lv)
		{
			if(lv == null) throw new ArgumentNullException("lv");

			m_lv = lv;
		}

		public void Queue(string strText, PwListItem li, int iIndexHint,
			int iSubItem, PwTextUpdateDelegate f)
		{
			if(strText == null) { Debug.Assert(false); return; }
			if(li == null) { Debug.Assert(false); return; }
			if(iSubItem < 0) { Debug.Assert(false); return; }
			if(f == null) { Debug.Assert(false); return; }

			LviUpdInfo state = new LviUpdInfo();
			state.ListView = m_lv;
			state.UpdateID = unchecked((li.ListViewItemID << 6) + iSubItem);
			state.Text = strText;
			state.ListItem = li;
			state.IndexHint = ((iIndexHint >= 0) ? iIndexHint : 0);
			state.SubItem = iSubItem;
			state.Function = f;
			state.ListEditSyncObject = m_objListEditSync;
			state.ValidIDsSyncObject = m_objValidIDsSync;
			state.ValidIDs = m_dValidIDs;

			lock(m_objValidIDsSync)
			{
				Debug.Assert(!m_dValidIDs.ContainsKey(state.UpdateID));
				m_dValidIDs[state.UpdateID] = true;
			}

			try
			{
				if(!ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateItemFn),
					state)) throw new InvalidOperationException();
			}
			catch(Exception)
			{
				Debug.Assert(false);
				lock(m_objValidIDsSync) { m_dValidIDs.Remove(state.UpdateID); }
			}
		}

		/// <summary>
		/// Cancel all pending updates. This method is asynchronous,
		/// i.e. it returns immediately and the number of queued
		/// updates will decrease continually.
		/// </summary>
		public void CancelPendingUpdatesAsync()
		{
			lock(m_objValidIDsSync)
			{
				List<long> vKeys = new List<long>(m_dValidIDs.Keys);
				foreach(long lKey in vKeys)
				{
					m_dValidIDs[lKey] = false;
				}
			}
		}

		public void WaitAll()
		{
			while(true)
			{
				lock(m_objValidIDsSync)
				{
					if(m_dValidIDs.Count == 0) break;
				}

				Thread.Sleep(4);
				Application.DoEvents();
			}
		}

		private static void UpdateItemFn(object state)
		{
			LviUpdInfo lui = (state as LviUpdInfo);
			if(lui == null) { Debug.Assert(false); return; }

			try // Avoid cross-thread exceptions
			{
				bool bWork;
				lock(lui.ValidIDsSyncObject)
				{
					if(!lui.ValidIDs.TryGetValue(lui.UpdateID,
						out bWork)) { Debug.Assert(false); return; }
				}

				if(bWork)
				{
					string strNew = lui.Function(lui.Text, lui.ListItem);
					if(strNew == null) { Debug.Assert(false); return; }
					if(strNew == lui.Text) return;

					// if(lui.ListView.InvokeRequired)
					lui.ListView.Invoke(new SetItemTextDelegate(
						SetItemText), new object[] { strNew, lui });
					// else SetItemText(strNew, lui);
				}
			}
			catch(Exception) { Debug.Assert(false); }
			finally
			{
				try // Avoid cross-thread exceptions
				{
					lock(lui.ValidIDsSyncObject)
					{
						if(!lui.ValidIDs.Remove(lui.UpdateID)) { Debug.Assert(false); }
					}
				}
				catch(Exception) { Debug.Assert(false); }
			}
		}

		private delegate void SetItemTextDelegate(string strText, LviUpdInfo lui);

		private static void SetItemText(string strText, LviUpdInfo lui)
		{
			try // Avoid cross-thread exceptions
			{
				long lTargetID = lui.ListItem.ListViewItemID;
				int iIndexHint = lui.IndexHint;

				lock(lui.ListEditSyncObject)
				{
					ListView.ListViewItemCollection lvic = lui.ListView.Items;
					int nCount = lvic.Count;

					// for(int i = 0; i < nCount; ++i)
					for(int i = nCount; i > 0; --i)
					{
						int j = ((iIndexHint + i) % nCount);
						ListViewItem lvi = lvic[j];

						PwListItem li = (lvi.Tag as PwListItem);
						if(li == null) { Debug.Assert(false); continue; }

						if(li.ListViewItemID != lTargetID) continue;

						lvi.SubItems[lui.SubItem].Text = strText;
						break;
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		internal static string SprCompileFn(string strText, PwListItem li)
		{
			string strCmp = null;
			while(strCmp == null)
			{
				try
				{
					strCmp = SprEngine.Compile(strText, MainForm.GetEntryListSprContext(
						li.Entry, Program.MainForm.DocumentManager.SafeFindContainerOf(
						li.Entry)));
				}
				catch(InvalidOperationException) { } // Probably collection changed
				catch(NullReferenceException) { } // Objects disposed already
				catch(Exception) { Debug.Assert(false); }
			}

			if(strCmp == strText) return strText;

			return (Program.Config.MainWindow.EntryListShowDerefDataAndRefs ?
				(strCmp + " - " + strText) : strCmp);
		}
	}
}
