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
using System.Threading;
using System.Diagnostics;

using KeePass.Forms;
using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Cryptography;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public enum BinaryDataHandler
	{
		Default = 0,
		InternalViewer = 1,
		InternalEditor = 2,
		ExternalApp = 3
	}

	public sealed class BinaryDataOpenOptions : IDeepCloneable<BinaryDataOpenOptions>
	{
		private BinaryDataHandler m_h = BinaryDataHandler.Default;
		public BinaryDataHandler Handler
		{
			get { return m_h; }
			set { m_h = value; }
		}

		private bool m_bReadOnly = false;
		public bool ReadOnly
		{
			get { return m_bReadOnly; }
			set { m_bReadOnly = value; }
		}

		public BinaryDataOpenOptions CloneDeep()
		{
			BinaryDataOpenOptions opt = new BinaryDataOpenOptions();

			opt.m_h = m_h;
			opt.m_bReadOnly = m_bReadOnly;

			return opt;
		}
	}

	public sealed class EntryBinaryDataContext
	{
		private PwEntry m_pe = null;
		public PwEntry Entry
		{
			get { return m_pe; }
			set { m_pe = value; }
		}

		private string m_strDataName = null;
		public string Name
		{
			get { return m_strDataName; }
			set { m_strDataName = value; }
		}

		private BinaryDataOpenOptions m_oo = null;
		public BinaryDataOpenOptions Options
		{
			get { return m_oo; }
			set { m_oo = value; }
		}
	}

	public static class BinaryDataUtil
	{
		public static ProtectedBinary Open(string strName, ProtectedBinary pb,
			BinaryDataOpenOptions opt)
		{
			if(string.IsNullOrEmpty(strName)) { Debug.Assert(false); return null; }
			if(pb == null) { Debug.Assert(false); return null; }
			if(opt == null) opt = new BinaryDataOpenOptions();

			byte[] pbData = pb.ReadData();
			if(pbData == null) { Debug.Assert(false); return null; }

			BinaryDataHandler h = opt.Handler;
			if(h == BinaryDataHandler.Default)
				h = ChooseHandler(strName, pbData, opt);

			byte[] pbModData = null;
			if(h == BinaryDataHandler.InternalViewer)
			{
				DataViewerForm dvf = new DataViewerForm();
				dvf.InitEx(strName, pbData);
				UIUtil.ShowDialogAndDestroy(dvf);
			}
			else if(h == BinaryDataHandler.InternalEditor)
			{
				DataEditorForm def = new DataEditorForm();
				def.InitEx(strName, pbData);
				def.ShowDialog();

				if(def.EditedBinaryData != null)
					pbModData = def.EditedBinaryData;

				UIUtil.DestroyForm(def);
			}
			else if(h == BinaryDataHandler.ExternalApp)
				pbModData = OpenExternal(strName, pbData, opt);
			else { Debug.Assert(false); }

			if((pbModData != null) && !MemUtil.ArraysEqual(pbData, pbModData) &&
				!opt.ReadOnly)
				return new ProtectedBinary(pb.IsProtected, pbModData);
			return null;
		}

		private static BinaryDataHandler ChooseHandler(string strName,
			byte[] pbData, BinaryDataOpenOptions opt)
		{
			BinaryDataClass bdc = BinaryDataClassifier.Classify(strName, pbData);

			if(DataEditorForm.SupportsDataType(bdc) && !opt.ReadOnly)
				return BinaryDataHandler.InternalEditor;

			if(DataViewerForm.SupportsDataType(bdc))
				return BinaryDataHandler.InternalViewer;

			return BinaryDataHandler.ExternalApp;
		}

		private static byte[] OpenExternal(string strName, byte[] pbData,
			BinaryDataOpenOptions opt)
		{
			byte[] pbResult = null;

			try
			{
				string strBaseTempDir = UrlUtil.EnsureTerminatingSeparator(
					UrlUtil.GetTempPath(), false);

				string strTempID, strTempDir;
				while(true)
				{
					byte[] pbRandomID = CryptoRandom.Instance.GetRandomBytes(8);
					strTempID = Convert.ToBase64String(pbRandomID);
					strTempID = StrUtil.AlphaNumericOnly(strTempID);
					if(strTempID.Length == 0) { Debug.Assert(false); continue; }

					strTempDir = strBaseTempDir + strTempID;
					if(!Directory.Exists(strTempDir))
					{
						Directory.CreateDirectory(strTempDir);

						strTempDir = UrlUtil.EnsureTerminatingSeparator(
							strTempDir, false);

						// Mark directory as encrypted, such that new files
						// are encrypted automatically; there exists no
						// Directory.Encrypt method, but we can use File.Encrypt,
						// because this internally uses the EncryptFile API
						// function, which works with directories
						try { File.Encrypt(strTempDir); }
						catch(Exception) { Debug.Assert(false); }

						break;
					}
				}

				string strFile = strTempDir + strName;
				File.WriteAllBytes(strFile, pbData);

				// Encrypt again, in case the directory encryption above failed
				try { File.Encrypt(strFile); }
				catch(Exception) { Debug.Assert(false); }

				ProcessStartInfo psi = new ProcessStartInfo();
				psi.FileName = strFile;
				psi.UseShellExecute = true;
				psi.WorkingDirectory = strTempDir;

				ParameterizedThreadStart pts = new ParameterizedThreadStart(
					BinaryDataUtil.ShellOpenFn);
				Thread th = new Thread(pts);
				th.Start(psi);

				string strMsgMain = KPRes.AttachExtOpened + MessageService.NewParagraph +
					KPRes.AttachExtOpenedPost + ":";
				string strMsgImp = KPRes.Import;
				string strMsgImpDesc = KPRes.AttachExtImportDesc;
				string strMsgCancel = KPRes.DiscardChangesCmd;
				string strMsgCancelDesc = KPRes.AttachExtDiscardDesc;

				VistaTaskDialog vtd = new VistaTaskDialog();
				vtd.CommandLinks = true;
				vtd.Content = strMsgMain;
				vtd.MainInstruction = strName;
				vtd.WindowTitle = PwDefs.ShortProductName;
				vtd.SetIcon(VtdCustomIcon.Question);

				vtd.AddButton((int)DialogResult.OK, strMsgImp, strMsgImpDesc);
				vtd.AddButton((int)DialogResult.Cancel, strMsgCancel, strMsgCancelDesc);

				vtd.FooterText = KPRes.AttachExtSecDel;
				vtd.SetFooterIcon(VtdIcon.Information);

				bool bImport;
				if(vtd.ShowDialog())
					bImport = ((vtd.Result == (int)DialogResult.OK) ||
						(vtd.Result == (int)DialogResult.Yes));
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine(strName);
					sb.AppendLine();
					sb.AppendLine(strMsgMain);
					sb.AppendLine();
					sb.AppendLine("[" + KPRes.Yes + "]:");
					sb.AppendLine(StrUtil.RemoveAccelerator(strMsgImp) +
						" - " + strMsgImpDesc);
					sb.AppendLine();
					sb.AppendLine("[" + KPRes.No + "]:");
					sb.AppendLine(StrUtil.RemoveAccelerator(strMsgCancel) +
						" - " + strMsgCancelDesc);
					sb.AppendLine();
					sb.AppendLine(KPRes.AttachExtSecDel);

					bImport = MessageService.AskYesNo(sb.ToString(),
						PwDefs.ShortProductName);
				}

				if(bImport && !opt.ReadOnly)
				{
					while(true)
					{
						try
						{
							pbResult = File.ReadAllBytes(strFile);
							break;
						}
						catch(Exception exRead)
						{
							if(!AskForRetry(strFile, exRead.Message)) break;
						}
					}
				}

				string strReportObj = null;
				while(true)
				{
					try
					{
						strReportObj = strFile;
						if(File.Exists(strFile))
						{
							FileInfo fiTemp = new FileInfo(strFile);
							long cb = fiTemp.Length;
							if(cb > 0)
							{
								byte[] pbOvr = new byte[cb];
								Program.GlobalRandom.NextBytes(pbOvr);
								File.WriteAllBytes(strFile, pbOvr);
							}

							File.Delete(strFile);
						}

						strReportObj = strTempDir;
						if(Directory.Exists(strTempDir))
							Directory.Delete(strTempDir, true);

						break;
					}
					catch(Exception exDel)
					{
						if(!AskForRetry(strReportObj, exDel.Message)) break;
					}
				}
			}
			catch(Exception ex)
			{
				MessageService.ShowWarning(ex.Message);
			}

			return pbResult;
		}

		private static bool AskForRetry(string strObj, string strText)
		{
			string strContent = strObj + MessageService.NewParagraph + strText;

			int i = VistaTaskDialog.ShowMessageBoxEx(strContent, null,
				PwDefs.ShortProductName, VtdIcon.Warning, null,
				KPRes.RetryCmd, (int)DialogResult.Retry,
				KPRes.Cancel, (int)DialogResult.Cancel);
			if(i < 0)
				i = (int)MessageService.Ask(strContent, PwDefs.ShortProductName,
					MessageBoxButtons.RetryCancel);

			return ((i == (int)DialogResult.Retry) || (i == (int)DialogResult.Yes) ||
				(i == (int)DialogResult.OK));
		}

		private static void ShellOpenFn(object o)
		{
			if(o == null) { Debug.Assert(false); return; }

			try
			{
				ProcessStartInfo psi = (o as ProcessStartInfo);
				if(psi == null) { Debug.Assert(false); return; }

				// Let the main thread finish showing the message box
				Thread.Sleep(200);

				Process.Start(psi);
			}
			catch(Exception ex)
			{
				try { MessageService.ShowWarning(ex.Message); }
				catch(Exception) { Debug.Assert(false); }
			}
		}

		internal static void BuildOpenWithMenu(DynamicMenu dm, string strItem,
			ProtectedBinary pb, bool bReadOnly)
		{
			if(dm == null) { Debug.Assert(false); return; }
			dm.Clear();

			if(string.IsNullOrEmpty(strItem)) { Debug.Assert(false); return; }
			if(pb == null) { Debug.Assert(false); return; }

			byte[] pbData = pb.ReadData();
			if(pbData == null) { Debug.Assert(false); return; }

			BinaryDataClass bdc = BinaryDataClassifier.Classify(strItem, pbData);

			BinaryDataOpenOptions oo = new BinaryDataOpenOptions();
			oo.Handler = BinaryDataHandler.InternalViewer;
			dm.AddItem(KPRes.InternalViewer, Properties.Resources.B16x16_View_Detailed, oo);

			oo = new BinaryDataOpenOptions();
			oo.Handler = BinaryDataHandler.InternalEditor;
			ToolStripMenuItem tsmiIntEditor = dm.AddItem(KPRes.InternalEditor,
				Properties.Resources.B16x16_View_Detailed, oo);

			oo = new BinaryDataOpenOptions();
			oo.Handler = BinaryDataHandler.ExternalApp;
			ToolStripMenuItem tsmiExt = dm.AddItem(KPRes.ExternalApp,
				Properties.Resources.B16x16_Make_KDevelop, oo);

			if(!DataEditorForm.SupportsDataType(bdc) || bReadOnly)
				tsmiIntEditor.Enabled = false;
			if(bReadOnly) tsmiExt.Enabled = false;
		}
	}
}
