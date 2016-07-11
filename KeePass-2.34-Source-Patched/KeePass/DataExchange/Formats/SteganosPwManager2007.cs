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
using System.IO;
using System.Drawing;
using System.Threading;

using KeePass.App;
using KeePass.Native;
using KeePass.Resources;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	internal sealed class SteganosPwManager2007 : FileFormatProvider
	{
		public override bool SupportsImport { get { return true; } }
		public override bool SupportsExport { get { return false; } }

		public override string FormatName { get { return "Steganos Password Manager 2007"; } }
		public override string ApplicationGroup { get { return KPRes.PasswordManagers; } }

		public override bool RequiresFile { get { return false; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_Imp_Steganos; }
		}

		public override void Import(PwDatabase pwStorage, Stream sInput,
			IStatusLogger slLogger)
		{
			if(MessageService.AskYesNo(KPRes.ImportMustRead + MessageService.NewParagraph +
				KPRes.ImportMustReadQuestion) == false)
			{
				AppHelp.ShowHelp(AppDefs.HelpTopics.ImportExport,
					AppDefs.HelpTopics.ImportExportSteganos);
				return;
			}

			PwEntry pePrev = new PwEntry(true, true);

			for(int i = 0; i < 20; ++i)
			{
				Thread.Sleep(500);
				Application.DoEvents();
			}

			try
			{
				while(true)
				{
					PwEntry pe = ImportEntry(pwStorage);

					if(ImportUtil.EntryEquals(pe, pePrev))
					{
						if(pe.ParentGroup != null) // Remove duplicate
							pe.ParentGroup.Entries.Remove(pe);
						break;
					}

					ImportUtil.GuiSendKeysPrc(@"{DOWN}");
					pePrev = pe;
				}

				MessageService.ShowInfo(KPRes.ImportFinished);
			}
			catch(Exception exImp) { MessageService.ShowWarning(exImp); }
		}

		private static PwEntry ImportEntry(PwDatabase pwDb)
		{
			ImportUtil.GuiSendWaitWindowChange(@"{ENTER}");
			Thread.Sleep(1000);
			ImportUtil.GuiSendKeysPrc(string.Empty); // Process messages

			string strTitle = ImportUtil.GuiSendRetrieve(string.Empty);
			string strGroup = ImportUtil.GuiSendRetrieve(@"{TAB}");
			string strUserName = ImportUtil.GuiSendRetrieve(@"{TAB}");
			ImportUtil.GuiSendKeysPrc(@"{TAB}{TAB}");
			ImportUtil.GuiSendKeysPrc(@" ");
			ImportUtil.GuiSendKeysPrc(@"+({TAB})");
			string strPassword = ImportUtil.GuiSendRetrieve(string.Empty);
			ImportUtil.GuiSendKeysPrc(@"{TAB} ");
			string strNotes = ImportUtil.GuiSendRetrieve(@"{TAB}{TAB}");

			string strUrl = ImportUtil.GuiSendRetrieve(@"{TAB}");
			string strUrl2 = ImportUtil.GuiSendRetrieve(@"{TAB}");

			ImportUtil.GuiSendWaitWindowChange(@"{ESC}");

			if(strGroup.Length == 0) strGroup = "Steganos";

			PwGroup pg = pwDb.RootGroup.FindCreateGroup(strGroup, true);
			PwEntry pe = new PwEntry(true, true);
			pg.AddEntry(pe, true);

			pe.Strings.Set(PwDefs.TitleField, new ProtectedString(
				pwDb.MemoryProtection.ProtectTitle, strTitle));
			pe.Strings.Set(PwDefs.UserNameField, new ProtectedString(
				pwDb.MemoryProtection.ProtectUserName, strUserName));
			pe.Strings.Set(PwDefs.PasswordField, new ProtectedString(
				pwDb.MemoryProtection.ProtectPassword, strPassword));
			pe.Strings.Set(PwDefs.NotesField, new ProtectedString(
				pwDb.MemoryProtection.ProtectNotes, strNotes));

			if(strUrl.Length > 0)
				pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
					pwDb.MemoryProtection.ProtectUrl, strUrl));
			else
				pe.Strings.Set(PwDefs.UrlField, new ProtectedString(
					pwDb.MemoryProtection.ProtectUrl, strUrl2));

			return pe;
		}
	}
}
