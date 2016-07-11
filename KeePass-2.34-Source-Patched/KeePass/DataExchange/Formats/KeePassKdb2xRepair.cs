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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using KeePass.App;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	internal sealed class KeePassKdb2xRepair : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName
		{
			get { return "KeePass KDBX (2.x) (" + KPRes.RepairMode + ")"; }
		}
		public override string DefaultExtension { get { return AppDefs.FileExtension.FileExt; } }
		public override string ApplicationGroup { get { return PwDefs.ShortProductName; } }

		public override bool SupportsUuids { get { return true; } }
		public override bool RequiresKey { get { return true; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_KeePass; }
		}

		public override bool TryBeginImport()
		{
			string strTitle = KPRes.Warning + "!";
			string strMsg = KPRes.RepairModeInt + MessageService.NewParagraph +
				KPRes.RepairModeUse + MessageService.NewParagraph +
				KPRes.RepairModeQ;

			int iYes = (int)DialogResult.Yes;
			int iNo = (int)DialogResult.No;
			int r = VistaTaskDialog.ShowMessageBoxEx(strMsg, strTitle,
				PwDefs.ShortProductName, VtdIcon.Warning, null,
				KPRes.YesCmd, iYes, KPRes.NoCmd, iNo);
			if(r < 0)
				r = (MessageService.AskYesNo(strTitle + MessageService.NewParagraph +
					strMsg, PwDefs.ShortProductName, false, MessageBoxIcon.Warning) ?
					iYes : iNo);

			return ((r == iYes) || (r == (int)DialogResult.OK));
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			KdbxFile kdbx = new KdbxFile(pwStorage);
			// CappedByteStream s = new CappedByteStream(sInput, 64);

			kdbx.RepairMode = true;

			try { kdbx.Load(sInput, KdbxFormat.Default, slLogger); }
			catch(Exception) { }
		}

		/* private sealed class CappedByteStream : Stream
		{
			private Stream m_sBase;
			private int m_nMaxRead;

			public override bool CanRead { get { return m_sBase.CanRead; } }
			public override bool CanSeek { get { return m_sBase.CanSeek; } }
			public override bool CanWrite { get { return m_sBase.CanWrite; } }
			public override long Length { get { return m_sBase.Length; } }
			public override long Position
			{
				get { return m_sBase.Position; }
				set { m_sBase.Position = value; }
			}

			public CappedByteStream(Stream sBase, int nMaxRead)
			{
				if(sBase == null) throw new ArgumentNullException("sBase");
				if(nMaxRead <= 0) throw new ArgumentException();

				m_sBase = sBase;
				m_nMaxRead = nMaxRead;
			}

			public override void Flush() { m_sBase.Flush(); }

			public override int Read(byte[] buffer, int offset, int count)
			{
				if(count > m_nMaxRead) count = m_nMaxRead;
				return m_sBase.Read(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return m_sBase.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				m_sBase.SetLength(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				m_sBase.Write(buffer, offset, count);
			}
		} */
	}
}
