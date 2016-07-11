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

using KeePass.App.Configuration;

using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Serialization;

namespace KeePass.UI
{
	public sealed class DocumentManagerEx
	{
		private List<PwDocument> m_vDocs = new List<PwDocument>();
		private PwDocument m_dsActive = new PwDocument();

		public event EventHandler ActiveDocumentSelected;

		public DocumentManagerEx()
		{
			Debug.Assert((m_vDocs != null) && (m_dsActive != null));
			m_vDocs.Add(m_dsActive);
		}

		public PwDocument ActiveDocument
		{
			get { return m_dsActive; }
			set
			{
				if(value == null) { Debug.Assert(false); throw new ArgumentNullException("value"); }

				for(int i = 0; i < m_vDocs.Count; ++i)
				{
					if(m_vDocs[i] == value)
					{
						m_dsActive = value;

						NotifyActiveDocumentSelected();
						return;
					}
				}

				throw new ArgumentException();
			}
		}

		public PwDatabase ActiveDatabase
		{
			get { return m_dsActive.Database; }
			set
			{
				if(value == null) { Debug.Assert(false); throw new ArgumentNullException("value"); }

				for(int i = 0; i < m_vDocs.Count; ++i)
				{
					if(m_vDocs[i].Database == value)
					{
						m_dsActive = m_vDocs[i];

						NotifyActiveDocumentSelected();
						return;
					}
				}

				throw new ArgumentException();
			}
		}

		public uint DocumentCount
		{
			get { return (uint)m_vDocs.Count; }
		}

		public List<PwDocument> Documents
		{
			get { return m_vDocs; }
		}

		public PwDocument CreateNewDocument(bool bMakeActive)
		{
			PwDocument ds = new PwDocument();

			if((m_vDocs.Count == 1) && (!m_vDocs[0].Database.IsOpen) &&
				(m_vDocs[0].LockedIoc.Path.Length == 0))
			{
				m_vDocs.RemoveAt(0);
				m_dsActive = ds;
			}

			m_vDocs.Add(ds);
			if(bMakeActive) m_dsActive = ds;

			NotifyActiveDocumentSelected();
			return ds;
		}

		public void CloseDatabase(PwDatabase pwDatabase)
		{
			int iFoundPos = -1;
			for(int i = 0; i < m_vDocs.Count; ++i)
			{
				if(m_vDocs[i].Database == pwDatabase)
				{
					iFoundPos = i;
					break;
				}
			}
			if(iFoundPos < 0) { Debug.Assert(false); return; }

			bool bClosingActive = (m_vDocs[iFoundPos] == m_dsActive);

			m_vDocs.RemoveAt(iFoundPos);
			if(m_vDocs.Count == 0)
				m_vDocs.Add(new PwDocument());

			if(bClosingActive)
			{
				int iNewActive = Math.Min(iFoundPos, m_vDocs.Count - 1);
				m_dsActive = m_vDocs[iNewActive];
				NotifyActiveDocumentSelected();
			}
			else { Debug.Assert(m_vDocs.Contains(m_dsActive)); }
		}

		public List<PwDatabase> GetOpenDatabases()
		{
			List<PwDatabase> list = new List<PwDatabase>();

			foreach(PwDocument ds in m_vDocs)
			{
				if(ds.Database.IsOpen)
					list.Add(ds.Database);
			}

			return list;
		}

		internal List<PwDocument> GetDocuments(int iMoveActive)
		{
			List<PwDocument> lDocs = new List<PwDocument>(m_vDocs);

			if(iMoveActive != 0)
			{
				for(int i = 0; i < lDocs.Count; ++i)
				{
					if(lDocs[i] == m_dsActive)
					{
						lDocs.RemoveAt(i);
						if(iMoveActive < 0) lDocs.Insert(0, m_dsActive);
						else lDocs.Add(m_dsActive);
						break;
					}
				}
			}

			return lDocs;
		}

		private void NotifyActiveDocumentSelected()
		{
			RememberActiveDocument();

			if(this.ActiveDocumentSelected != null)
				this.ActiveDocumentSelected(null, EventArgs.Empty);
		}

		internal void RememberActiveDocument()
		{
			if(m_dsActive == null) { Debug.Assert(false); return; }

			if(m_dsActive.LockedIoc != null)
				SetLastUsedFile(m_dsActive.LockedIoc);
			if(m_dsActive.Database != null)
				SetLastUsedFile(m_dsActive.Database.IOConnectionInfo);
		}

		private static void SetLastUsedFile(IOConnectionInfo ioc)
		{
			if(ioc == null) { Debug.Assert(false); return; }

			AceApplication aceApp = Program.Config.Application;
			if(aceApp.Start.OpenLastFile)
			{
				if(!string.IsNullOrEmpty(ioc.Path))
					aceApp.LastUsedFile = ioc.CloneDeep();
			}
			else aceApp.LastUsedFile = new IOConnectionInfo();
		}

		public PwDocument FindDocument(PwDatabase pwDatabase)
		{
			if(pwDatabase == null) throw new ArgumentNullException("pwDatabase");

			foreach(PwDocument ds in m_vDocs)
			{
				if(ds.Database == pwDatabase) return ds;
			}

			return null;
		}

		/// <summary>
		/// Search for an entry in all opened databases. The
		/// entry is identified by its reference (not its UUID).
		/// </summary>
		/// <param name="peObj">Entry to search for.</param>
		/// <returns>Database containing the entry.</returns>
		public PwDatabase FindContainerOf(PwEntry peObj)
		{
			if(peObj == null) return null; // No assert

			PwGroup pg = peObj.ParentGroup;
			if(pg != null)
			{
				while(pg.ParentGroup != null) { pg = pg.ParentGroup; }

				foreach(PwDocument ds in m_vDocs)
				{
					PwDatabase pd = ds.Database;
					if((pd == null) || !pd.IsOpen) continue;

					if(object.ReferenceEquals(pd.RootGroup, pg))
						return pd;
				}

				Debug.Assert(false);
			}

			return SlowFindContainerOf(peObj);
		}

		private PwDatabase SlowFindContainerOf(PwEntry peObj)
		{
			PwDatabase pdRet = null;
			foreach(PwDocument ds in m_vDocs)
			{
				PwDatabase pd = ds.Database;
				if((pd == null) || !pd.IsOpen) continue;

				EntryHandler eh = delegate(PwEntry pe)
				{
					if(object.ReferenceEquals(pe, peObj))
					{
						pdRet = pd;
						return false; // Stop traversal
					}

					return true;
				};

				pd.RootGroup.TraverseTree(TraversalMethod.PreOrder, null, eh);
				if(pdRet != null) return pdRet;
			}

			return null;
		}

		public PwDatabase SafeFindContainerOf(PwEntry peObj)
		{
			// peObj may be null
			return (FindContainerOf(peObj) ?? m_dsActive.Database);
		}
	}

	public sealed class PwDocument
	{
		private PwDatabase m_pwDb = new PwDatabase();
		private IOConnectionInfo m_ioLockedIoc = new IOConnectionInfo();

		public PwDatabase Database
		{
			get { return m_pwDb; }
		}

		public IOConnectionInfo LockedIoc
		{
			get { return m_ioLockedIoc; }
			set
			{
				Debug.Assert(value != null); if(value == null) throw new ArgumentNullException("value");
				m_ioLockedIoc = value;
			}
		}
	}
}
