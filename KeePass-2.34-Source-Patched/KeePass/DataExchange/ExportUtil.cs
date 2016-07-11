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
using System.IO;

using KeePass.App;
using KeePass.Forms;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.DataExchange
{
	public static class ExportUtil
	{
		public static bool Export(PwExportInfo pwExportInfo, IStatusLogger slLogger)
		{
			if(pwExportInfo == null) throw new ArgumentNullException("pwExportInfo");
			if(pwExportInfo.DataGroup == null) throw new ArgumentException();

			if(!AppPolicy.Try(AppPolicyId.Export)) return false;

			ExchangeDataForm dlg = new ExchangeDataForm();
			dlg.InitEx(true, pwExportInfo.ContextDatabase, pwExportInfo.DataGroup);

			bool bResult = false;
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				FileFormatProvider ffp = dlg.ResultFormat;
				if(ffp == null) { Debug.Assert(false); goto ExpZRet; }
				if(ffp.RequiresFile)
				{
					if(dlg.ResultFiles.Length != 1) { Debug.Assert(false); goto ExpZRet; }
					if(dlg.ResultFiles[0] == null) { Debug.Assert(false); goto ExpZRet; }
					if(dlg.ResultFiles[0].Length == 0) { Debug.Assert(false); goto ExpZRet; }
				}

				Application.DoEvents(); // Redraw parent window

				IOConnectionInfo iocOutput = (ffp.RequiresFile ? IOConnectionInfo.FromPath(
					dlg.ResultFiles[0]) : null);

				try
				{
					bResult = Export(pwExportInfo, dlg.ResultFormat, iocOutput, slLogger);
				}
				catch(Exception ex) { MessageService.ShowWarning(ex); }
			}

		ExpZRet:
			UIUtil.DestroyForm(dlg);
			return bResult;
		}

		public static bool Export(PwExportInfo pwExportInfo, string strFormatName,
			IOConnectionInfo iocOutput)
		{
			if(strFormatName == null) throw new ArgumentNullException("strFormatName");
			// iocOutput may be null

			FileFormatProvider prov = Program.FileFormatPool.Find(strFormatName);
			if(prov == null) return false;

			NullStatusLogger slLogger = new NullStatusLogger();
			return Export(pwExportInfo, prov, iocOutput, slLogger);
		}

		public static bool Export(PwExportInfo pwExportInfo, FileFormatProvider
			fileFormat, IOConnectionInfo iocOutput, IStatusLogger slLogger)
		{
			if(pwExportInfo == null) throw new ArgumentNullException("pwExportInfo");
			if(pwExportInfo.DataGroup == null) throw new ArgumentException();
			if(fileFormat == null) throw new ArgumentNullException("fileFormat");
			if(fileFormat.RequiresFile && (iocOutput == null))
				throw new ArgumentNullException("iocOutput");

			if(!AppPolicy.Try(AppPolicyId.Export)) return false;
			if(!fileFormat.SupportsExport) return false;
			if(!fileFormat.TryBeginExport()) return false;

			// bool bExistedAlready = File.Exists(strOutputFile);
			bool bExistedAlready = (fileFormat.RequiresFile ? IOConnection.FileExists(
				iocOutput) : false);

			// FileStream fsOut = new FileStream(strOutputFile, FileMode.Create,
			//	FileAccess.Write, FileShare.None);
			Stream sOut = (fileFormat.RequiresFile ? IOConnection.OpenWrite(
				iocOutput) : null);

			bool bResult = false;
			try { bResult = fileFormat.Export(pwExportInfo, sOut, slLogger); }
			catch(Exception ex) { MessageService.ShowWarning(ex); }

			if(sOut != null) sOut.Close();

			if(fileFormat.RequiresFile && (bResult == false) && (bExistedAlready == false))
			{
				try { IOConnection.DeleteFile(iocOutput); }
				catch(Exception) { }
			}

			return bResult;
		}
	}
}
