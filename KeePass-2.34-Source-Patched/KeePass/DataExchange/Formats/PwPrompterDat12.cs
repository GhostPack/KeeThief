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
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

using KeePass.Forms;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Resources;
using KeePassLib.Security;

namespace KeePass.DataExchange.Formats
{
	internal sealed class PwPrompterDat12 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Password Prompter DAT"; } }
		public override string DefaultExtension { get { return "dat"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool ImportAppendsToRootGroupOnly { get { return true; } }

		public override Image SmallIcon
		{
			get { return Properties.Resources.B16x16_Imp_PwPrompter; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			SingleLineEditForm dlg = new SingleLineEditForm();
			dlg.InitEx(KPRes.Password, KPRes.Import + ": " + this.FormatName,
				KPRes.PasswordPrompt, Properties.Resources.B48x48_KGPG_Key2,
				string.Empty, null);
			if(UIUtil.ShowDialogNotValue(dlg, DialogResult.OK)) return;
			string strPassword = dlg.ResultString;
			UIUtil.DestroyForm(dlg);

			byte[] pbPassword = Encoding.Default.GetBytes(strPassword);

			BinaryReader br = new BinaryReader(sInput, Encoding.Default);

			ushort usFileVersion = br.ReadUInt16();
			if(usFileVersion != 0x0100)
				throw new Exception(KLRes.FileVersionUnsupported);

			uint uEntries = br.ReadUInt32();
			uint uKeySize = br.ReadUInt32();
			Debug.Assert(uKeySize == 50); // It's a constant
			
			byte btKeyArrayLen = br.ReadByte();
			byte[] pbKey = br.ReadBytes(btKeyArrayLen);

			byte btValidArrayLen = br.ReadByte();
			byte[] pbValid = br.ReadBytes(btValidArrayLen);

			if(pbPassword.Length > 0)
			{
				MangleSetKey(pbPassword);
				MangleDecode(pbKey);
			}

			MangleSetKey(pbKey);
			MangleDecode(pbValid);
			string strValid = Encoding.Default.GetString(pbValid);
			if(strValid != "aacaaaadaaeabaacyuioqaqqaaaaaertaaajkadaadaaxywqea")
				throw new Exception(KLRes.InvalidCompositeKey);

			for(uint uEntry = 0; uEntry < uEntries; ++uEntry)
			{
				PwEntry pe = new PwEntry(true, true);
				pwStorage.RootGroup.AddEntry(pe, true);

				pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectTitle, ReadString(br)));
				pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectUserName, ReadString(br)));
				pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectPassword, ReadString(br)));
				pe.Strings.Set("Hint", new ProtectedString(false, ReadString(br)));
				pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectNotes, ReadString(br)));
				pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
					pwStorage.MemoryProtection.ProtectUrl, ReadString(br)));
			}

			br.Close();
			sInput.Close();
		}

		private string ReadString(BinaryReader br)
		{
			byte btLen = br.ReadByte();
			byte[] pbData = br.ReadBytes(btLen);

			MangleDecode(pbData);

			return Encoding.Default.GetString(pbData);
		}

		byte[] m_pbMangleKey = null;
		private void MangleSetKey(byte[] pbKey)
		{
			if(pbKey == null) { Debug.Assert(false); return; }

			m_pbMangleKey = new byte[pbKey.Length];
			Array.Copy(pbKey, m_pbMangleKey, pbKey.Length);
		}

		private void MangleDecode(byte[] pbData)
		{
			if(m_pbMangleKey == null) { Debug.Assert(false); return; }

			int nKeyIndex = 0, nIndex = 0, nRemLen = pbData.Length;
			bool bUp = true;

			while(nRemLen > 0)
			{
				if(nKeyIndex > (m_pbMangleKey.Length - 1))
				{
					nKeyIndex = m_pbMangleKey.Length - 1;
					bUp = false;
				}
				else if(nKeyIndex < 0)
				{
					nKeyIndex = 0;
					bUp = true;
				}

				pbData[nIndex] ^= m_pbMangleKey[nKeyIndex];

				nKeyIndex += (bUp ? 1 : -1);

				++nIndex;
				--nRemLen;
			}
		}
	}
}
