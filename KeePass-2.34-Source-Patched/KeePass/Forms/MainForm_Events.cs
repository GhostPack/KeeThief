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

using KeePass.App;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Serialization;

namespace KeePass.Forms
{
	public sealed class FileCreatedEventArgs : EventArgs
	{
		private PwDatabase m_pwDatabase;

		public PwDatabase Database { get { return m_pwDatabase; } }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public FileCreatedEventArgs(PwDatabase pwDatabase)
		{
			m_pwDatabase = pwDatabase;
		}
	}

	public sealed class FileOpenedEventArgs : EventArgs
	{
		private PwDatabase m_pwDatabase;

		public PwDatabase Database { get { return m_pwDatabase; } }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public FileOpenedEventArgs(PwDatabase pwDatabase)
		{
			m_pwDatabase = pwDatabase;
		}
	}

	/// <summary>
	/// Event arguments structure for the file-saving event.
	/// </summary>
	public sealed class FileSavingEventArgs : CancellableOperationEventArgs
	{
		private bool m_bSaveAs;
		private bool m_bCopy;
		private PwDatabase m_pwDatabase;
		private Guid m_eventGuid;

		/// <summary>
		/// Flag that determines if the user is performing a 'Save As' operation.
		/// If this flag is <c>false</c>, the operation is a standard 'Save' operation.
		/// </summary>
		public bool SaveAs { get { return m_bSaveAs; } }

		public bool Copy { get { return m_bCopy; } }

		public PwDatabase Database { get { return m_pwDatabase; } }

		public Guid EventGuid { get { return m_eventGuid; } }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="bSaveAs">See <c>SaveAs</c> property.</param>
		public FileSavingEventArgs(bool bSaveAs, bool bCopy, PwDatabase pwDatabase,
			Guid eventGuid)
		{
			m_bSaveAs = bSaveAs;
			m_bCopy = bCopy;
			m_pwDatabase = pwDatabase;
			m_eventGuid = eventGuid;
		}
	}

	/// <summary>
	/// Event arguments structure for the file-saved event.
	/// </summary>
	public sealed class FileSavedEventArgs : EventArgs
	{
		private bool m_bResult;
		private PwDatabase m_pwDatabase;
		private Guid m_eventGuid;

		/// <summary>
		/// Specifies the result of the attempt to save the database. If
		/// this property is <c>true</c>, the database has been saved
		/// successfully.
		/// </summary>
		public bool Success { get { return m_bResult; } }

		public PwDatabase Database { get { return m_pwDatabase; } }

		public Guid EventGuid { get { return m_eventGuid; } }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="bResult">See <c>Result</c> property.</param>
		public FileSavedEventArgs(bool bSuccess, PwDatabase pwDatabase, Guid eventGuid)
		{
			m_bResult = bSuccess;
			m_pwDatabase = pwDatabase;
			m_eventGuid = eventGuid;
		}
	}

	/// <summary>
	/// Event arguments structure for file-closing events.
	/// </summary>
	public sealed class FileClosingEventArgs : CancellableOperationEventArgs
	{
		private PwDatabase m_pwDatabase;

		public PwDatabase Database { get { return m_pwDatabase; } }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public FileClosingEventArgs(PwDatabase pwDatabase)
		{
			m_pwDatabase = pwDatabase;
		}
	}

	/// <summary>
	/// Event arguments structure for the file-closed event.
	/// </summary>
	public sealed class FileClosedEventArgs : EventArgs
	{
		private IOConnectionInfo m_ioClosed;

		public IOConnectionInfo IOConnectionInfo { get { return m_ioClosed; } }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public FileClosedEventArgs(IOConnectionInfo ioClosed)
		{
			m_ioClosed = ioClosed;
		}
	}

	public sealed class CancelEntryEventArgs : CancellableOperationEventArgs
	{
		private PwEntry m_pwEntry;
		private int m_nColumn;

		public PwEntry Entry { get { return m_pwEntry; } }
		public int ColumnId { get { return m_nColumn; } }

		public CancelEntryEventArgs(PwEntry pe, int colId)
		{
			m_pwEntry = pe;
			m_nColumn = colId;
		}
	}

	public sealed class FocusEventArgs : CancellableOperationEventArgs
	{
		private Control m_cNewRequested;
		private Control m_cNewFocusing;

		public Control RequestedControl { get { return m_cNewRequested; } }
		public Control FocusingControl { get { return m_cNewFocusing; } }

		public FocusEventArgs(Control cRequested, Control cFocusing)
		{
			m_cNewRequested = cRequested;
			m_cNewFocusing = cFocusing;
		}
	}

	public partial class MainForm : Form
	{
		/// <summary>
		/// Event that is fired after a database has been created.
		/// </summary>
		public event EventHandler<FileCreatedEventArgs> FileCreated;

		/// <summary>
		/// Event that is fired after a database has been opened.
		/// </summary>
		public event EventHandler<FileOpenedEventArgs> FileOpened;

		public event EventHandler<FileClosingEventArgs> FileClosingPre;
		public event EventHandler<FileClosingEventArgs> FileClosingPost;

		/// <summary>
		/// Event that is fired after a database has been closed.
		/// </summary>
		public event EventHandler<FileClosedEventArgs> FileClosed;

		/// <summary>
		/// Event that is fired before a database is being saved. By handling this
		/// event, you can abort the file-saving operation.
		/// </summary>
		public event EventHandler<FileSavingEventArgs> FileSaving;

		/// <summary>
		/// Event that is fired after a database has been saved.
		/// </summary>
		public event EventHandler<FileSavedEventArgs> FileSaved;

		public event EventHandler FormLoadPost;

		public event EventHandler<CancelEntryEventArgs> DefaultEntryAction;

		public event EventHandler UIStateUpdated;

		public event EventHandler<FocusEventArgs> FocusChanging;

		public event EventHandler UserActivityPost;
	}
}
