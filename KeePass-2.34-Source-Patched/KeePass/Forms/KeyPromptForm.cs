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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Keys;
using KeePassLib.Native;
using KeePassLib.Utility;
using KeePassLib.Serialization;

namespace KeePass.Forms
{
	public partial class KeyPromptForm : Form
	{
		private CompositeKey m_pKey = null;
		private IOConnectionInfo m_ioInfo = new IOConnectionInfo();
		private string m_strCustomTitle = null;

		// private bool m_bRedirectActivation = false;
		private bool m_bCanExit = false;
		private bool m_bHasExited = false;

		private SecureEdit m_secPassword = new SecureEdit();

		private bool m_bInitializing = true;
		private bool m_bDisposed = false;

		private AceKeyAssoc m_aKeyAssoc = null;
		private bool m_bCanModKeyFile = false;
		private List<string> m_lKeyFileNames = new List<string>();
		// private List<Image> m_lKeyFileImages = new List<Image>();
		private bool m_bUaStatePreset = false;

		public CompositeKey CompositeKey
		{
			get { return m_pKey; }
		}

		public bool HasClosedWithExit
		{
			get { return m_bHasExited; }
		}

		private bool m_bSecureDesktop = false;
		public bool SecureDesktopMode
		{
			get { return m_bSecureDesktop; }
			set { m_bSecureDesktop = value; }
		}

		private bool m_bShowHelpAfterClose = false;
		public bool ShowHelpAfterClose
		{
			get { return m_bShowHelpAfterClose; }
		}

		public KeyPromptForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		public void InitEx(IOConnectionInfo ioInfo, bool bCanExit,
			bool bRedirectActivation)
		{
			InitEx(ioInfo, bCanExit, bRedirectActivation, null);
		}

		public void InitEx(IOConnectionInfo ioInfo, bool bCanExit,
			bool bRedirectActivation, string strCustomTitle)
		{
			if(ioInfo != null) m_ioInfo = ioInfo;

			m_bCanExit = bCanExit;
			// m_bRedirectActivation = bRedirectActivation;
			m_strCustomTitle = strCustomTitle;
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			m_bInitializing = true;

			GlobalWindowManager.AddWindow(this);
			// if(m_bRedirectActivation) Program.MainForm.RedirectActivationPush(this);

			string strBannerTitle = (!string.IsNullOrEmpty(m_strCustomTitle) ?
				m_strCustomTitle : KPRes.EnterCompositeKey);
			string strBannerDesc = WinUtil.CompactPath(m_ioInfo.Path, 45);
			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_KGPG_Key2, strBannerTitle, strBannerDesc);
			this.Icon = Properties.Resources.KeePass;

			FontUtil.SetDefaultFont(m_cbPassword);
			FontUtil.AssignDefaultBold(m_cbPassword);
			FontUtil.AssignDefaultBold(m_cbKeyFile);
			FontUtil.AssignDefaultBold(m_cbUserAccount);

			// m_ttRect.SetToolTip(m_cbHidePassword, KPRes.TogglePasswordAsterisks);
			m_ttRect.SetToolTip(m_btnOpenKeyFile, KPRes.KeyFileSelect);

			PwInputControlGroup.ConfigureHideButton(m_cbHidePassword, m_ttRect);

			string strStart = (!string.IsNullOrEmpty(m_strCustomTitle) ?
				m_strCustomTitle : KPRes.OpenDatabase);
			string strNameEx = UrlUtil.GetFileName(m_ioInfo.Path);
			if(!string.IsNullOrEmpty(strNameEx))
				this.Text = strStart + " - " + strNameEx;
			else this.Text = strStart;

			m_tbPassword.Text = string.Empty;
			m_secPassword.SecureDesktopMode = m_bSecureDesktop;
			//m_secPassword.Attach(m_tbPassword, ProcessTextChangedPassword, true);

			// m_cmbKeyFile.OrderedImageList = m_lKeyFileImages;
			AddKeyFileSuggPriv(KPRes.NoKeyFileSpecifiedMeta, true);

			// Do not directly compare with Program.CommandLineArgs.FileName,
			// because this may be a relative path instead of an absolute one
			string strCmdLineFile = Program.CommandLineArgs.FileName;
			if((strCmdLineFile != null) && (Program.MainForm != null))
				strCmdLineFile = Program.MainForm.IocFromCommandLine().Path;
			if((strCmdLineFile != null) && strCmdLineFile.Equals(m_ioInfo.Path,
				StrUtil.CaseIgnoreCmp))
			{
				string str;

				str = Program.CommandLineArgs[AppDefs.CommandLineOptions.Password];
				if(str != null)
				{
					m_cbPassword.Checked = true;
					m_tbPassword.Text = str;
				}

				str = Program.CommandLineArgs[AppDefs.CommandLineOptions.PasswordEncrypted];
				if(str != null)
				{
					m_cbPassword.Checked = true;
					m_tbPassword.Text = StrUtil.DecryptString(str);
				}

				str = Program.CommandLineArgs[AppDefs.CommandLineOptions.PasswordStdIn];
				if(str != null)
				{
					KcpPassword kcpPw = KeyUtil.ReadPasswordStdIn(true);
					if(kcpPw != null)
					{
						m_cbPassword.Checked = true;
						m_tbPassword.Text = kcpPw.Password.ReadString();
					}
				}

				str = Program.CommandLineArgs[AppDefs.CommandLineOptions.KeyFile];
				if(str != null)
				{
					m_cbKeyFile.Checked = true;
					AddKeyFileSuggPriv(str, true);
				}

				str = Program.CommandLineArgs[AppDefs.CommandLineOptions.PreSelect];
				if(str != null)
				{
					m_cbKeyFile.Checked = true;
					AddKeyFileSuggPriv(str, true);
				}
			}

			m_cbHidePassword.Checked = true;
			//OnCheckedHidePassword(sender, e);

			Debug.Assert(m_cmbKeyFile.Text.Length != 0);

			m_btnExit.Enabled = m_bCanExit;
			m_btnExit.Visible = m_bCanExit;

			ulong uKpf = Program.Config.UI.KeyPromptFlags;
			UIUtil.ApplyKeyUIFlags(uKpf, m_cbPassword, m_cbKeyFile,
				m_cbUserAccount, m_cbHidePassword);

			if((uKpf & (ulong)AceKeyUIFlags.DisableKeyFile) != 0)
			{
				UIUtil.SetEnabled(m_cmbKeyFile, m_cbKeyFile.Checked);
				UIUtil.SetEnabled(m_btnOpenKeyFile, m_cbKeyFile.Checked);
			}

			if(((uKpf & (ulong)AceKeyUIFlags.CheckUserAccount) != 0) ||
				((uKpf & (ulong)AceKeyUIFlags.UncheckUserAccount) != 0))
				m_bUaStatePreset = true;

			CustomizeForScreenReader();
			EnableUserControls();

			m_bInitializing = false;

			// E.g. command line options have higher priority
			m_bCanModKeyFile = (m_cmbKeyFile.SelectedIndex == 0);

			m_aKeyAssoc = Program.Config.Defaults.GetKeySources(m_ioInfo);
			if(m_aKeyAssoc != null)
			{
				if(m_aKeyAssoc.KeyFilePath.Length > 0)
					AddKeyFileSuggPriv(m_aKeyAssoc.KeyFilePath, null);

				if(m_aKeyAssoc.UserAccount && !m_bUaStatePreset)
					m_cbUserAccount.Checked = true;
			}

			foreach(KeyProvider prov in Program.KeyProviderPool)
				AddKeyFileSuggPriv(prov.Name, null);

			// Local, but thread will continue to run anyway
			Thread th = new Thread(new ThreadStart(this.AsyncFormLoad));
			th.Start();
			// ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsyncFormLoad));

			this.BringToFront();
			this.Activate();
			// UIUtil.SetFocus(m_tbPassword, this); // See OnFormShown
		}

		private void OnFormShown(object sender, EventArgs e)
		{
			// Focusing doesn't always work in OnFormLoad;
			// https://sourceforge.net/p/keepass/feature-requests/1735/
			if(m_tbPassword.CanFocus) UIUtil.ResetFocus(m_tbPassword, this);
			else if(m_cmbKeyFile.CanFocus) UIUtil.SetFocus(m_cmbKeyFile, this);
			else if(m_btnOK.CanFocus) UIUtil.SetFocus(m_btnOK, this);
			else { Debug.Assert(false); }
		}

		private void CustomizeForScreenReader()
		{
			if(!Program.Config.UI.OptimizeForScreenReader) return;

			m_cbHidePassword.Text = KPRes.HideUsingAsterisks;
			m_btnOpenKeyFile.Text = KPRes.SelectFile;
		}

		private void CleanUpEx()
		{
			Debug.Assert(m_cmbKeyFile.Items.Count == m_lKeyFileNames.Count);

			if(m_bDisposed) { Debug.Assert(false); return; }
			m_bDisposed = true;

			// m_cmbKeyFile.OrderedImageList = null;
			// m_lKeyFileImages.Clear();

			m_secPassword.Detach();
		}

		private bool CreateCompositeKey()
		{
			m_pKey = new CompositeKey();

			if(m_cbPassword.Checked && m_tbPassword.Text != "") // Use a password
			{
                m_pKey.AddUserKey(new KcpPassword(m_tbPassword.Text));
			}

            if(m_cbKeyFile.Checked && m_keyFileBase64.Text != "") // Use the base64 representation of a keyfile
            {
                byte[] pb = Convert.FromBase64String(m_keyFileBase64.Text);
                m_pKey.AddUserKey(new KcpKeyFile(pb));
                Array.Clear(pb, 0, pb.Length);
            }

            if (m_cbUserAccount.Checked && m_userBase64.Text != "") // Use the base64 representation of a Windows User Account
            {
                byte[] pb = Convert.FromBase64String(m_userBase64.Text);

                //MessageBox.Show(m_userBase64.Text, "m_userBase64.Text", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                //StringBuilder hex = new StringBuilder(pb.Length * 2);
                //foreach (byte b in pb)
                //    hex.AppendFormat("{0:x2}", b);
                //MessageBox.Show(hex.ToString(), "m_userBase64 hex", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

                m_pKey.AddUserKey(new KcpUserAccount(pb));
                Array.Clear(pb, 0, pb.Length);
            }


            //string strKeyFile = m_cmbKeyFile.Text;
            //Debug.Assert(strKeyFile != null); if(strKeyFile == null) strKeyFile = string.Empty;
            //bool bIsProvKey = Program.KeyProviderPool.IsKeyProvider(strKeyFile);

            //if(m_cbKeyFile.Checked && !strKeyFile.Equals(KPRes.NoKeyFileSpecifiedMeta) &&
            //	!bIsProvKey)
            //{
            //	if(!ValidateKeyFile()) return false;

            //	try { m_pKey.AddUserKey(new KcpKeyFile(strKeyFile)); }
            //	catch(Exception)
            //	{
            //		MessageService.ShowWarning(strKeyFile, KPRes.KeyFileError);
            //		return false;
            //	}
            //}
            //else if(m_cbKeyFile.Checked && !strKeyFile.Equals(KPRes.NoKeyFileSpecifiedMeta) &&
            //	bIsProvKey)
            //{
            //	KeyProvider kp = Program.KeyProviderPool.Get(strKeyFile);
            //	if((kp != null) && m_bSecureDesktop)
            //	{
            //		if(!kp.SecureDesktopCompatible)
            //		{
            //			MessageService.ShowWarning(KPRes.KeyProvIncmpWithSD,
            //				KPRes.KeyProvIncmpWithSDHint);
            //			return false;
            //		}
            //	}

            //	KeyProviderQueryContext ctxKP = new KeyProviderQueryContext(
            //		m_ioInfo, false, m_bSecureDesktop);

            //	bool bPerformHash;
            //	byte[] pbProvKey = Program.KeyProviderPool.GetKey(strKeyFile, ctxKP,
            //		out bPerformHash);
            //	if((pbProvKey != null) && (pbProvKey.Length > 0))
            //	{
            //		try { m_pKey.AddUserKey(new KcpCustomKey(strKeyFile, pbProvKey, bPerformHash)); }
            //		catch(Exception exCKP)
            //		{
            //			MessageService.ShowWarning(exCKP);
            //			return false;
            //		}

            //		Array.Clear(pbProvKey, 0, pbProvKey.Length);
            //	}
            //	else return false; // Provider has shown error message
            //}

   //         if (m_cbUserAccount.Checked)
			//{
			//	try { m_pKey.AddUserKey(new KcpUserAccount()); }
			//	catch(Exception exUA)
			//	{
			//		MessageService.ShowWarning(exUA);
			//		return false;
			//	}
			//}

			return true;
		}

		private bool ValidateKeyFile()
		{
			string strKeyFile = m_cmbKeyFile.Text;
			Debug.Assert(strKeyFile != null); if(strKeyFile == null) strKeyFile = string.Empty;
			if(strKeyFile.Equals(KPRes.NoKeyFileSpecifiedMeta)) return true;

			if(Program.KeyProviderPool.IsKeyProvider(strKeyFile)) return true;

			bool bSuccess = true;

			IOConnectionInfo ioc = IOConnectionInfo.FromPath(strKeyFile);
			if(!IOConnection.FileExists(ioc))
			{
				MessageService.ShowWarning(strKeyFile, KPRes.FileNotFoundError);
				bSuccess = false;
			}

			// if(!bSuccess)
			// {
			//	int nPos = m_cmbKeyFile.Items.IndexOf(strKeyFile);
			//	if(nPos >= 0)
			//	{
			//		m_cmbKeyFile.Items.RemoveAt(nPos);
			//		m_lKeyFileNames.RemoveAt(nPos);
			//	}
			//	m_cmbKeyFile.SelectedIndex = 0;
			// }

			return bSuccess;
		}

		private void EnableUserControls()
		{
			string strKeyFile = m_cmbKeyFile.Text;
			Debug.Assert(strKeyFile != null); if(strKeyFile == null) strKeyFile = string.Empty;
			if(m_cbKeyFile.Checked && strKeyFile.Equals(KPRes.NoKeyFileSpecifiedMeta))
				UIUtil.SetEnabled(m_btnOK, false);
			else UIUtil.SetEnabled(m_btnOK, true);

			bool bExclusiveProv = false;
			KeyProvider prov = Program.KeyProviderPool.Get(strKeyFile);
			if(prov != null) bExclusiveProv = prov.Exclusive;

			if(bExclusiveProv)
			{
				m_tbPassword.Text = string.Empty;
				UIUtil.SetChecked(m_cbPassword, false);
				UIUtil.SetChecked(m_cbUserAccount, false);
			}

			bool bPwAllowed = ((Program.Config.UI.KeyPromptFlags &
				(ulong)AceKeyUIFlags.DisablePassword) == 0);
			bool bPwInput = (bPwAllowed || m_cbPassword.Checked);
			UIUtil.SetEnabled(m_cbPassword, !bExclusiveProv && bPwAllowed);
			UIUtil.SetEnabled(m_tbPassword, !bExclusiveProv && bPwInput);
			if((Program.Config.UI.KeyPromptFlags &
				(ulong)AceKeyUIFlags.DisableHidePassword) == 0)
				UIUtil.SetEnabled(m_cbHidePassword, !bExclusiveProv && bPwInput);

			bool bUaAllowed = ((Program.Config.UI.KeyPromptFlags &
				(ulong)AceKeyUIFlags.DisableUserAccount) == 0);
			bUaAllowed &= !(WinUtil.IsWindows9x || NativeLib.IsUnix());
			UIUtil.SetEnabled(m_cbUserAccount, !bExclusiveProv && bUaAllowed);
		}

		private void OnCheckedPassword(object sender, EventArgs e)
		{
			if(m_cbPassword.Checked) UIUtil.SetFocus(m_tbPassword, this);
		}

		private void OnCheckedKeyFile(object sender, EventArgs e)
		{
			if(m_bInitializing) return;

			if(!m_cbKeyFile.Checked) UIUtil.SetFocus(m_keyFileBase64, this);

            //EnableUserControls();
        }

		private void ProcessTextChangedPassword(object sender, EventArgs e)
		{
			if(((Program.Config.UI.KeyPromptFlags & (ulong)AceKeyUIFlags.CheckPassword) == 0) &&
				((Program.Config.UI.KeyPromptFlags & (ulong)AceKeyUIFlags.UncheckPassword) == 0))
				UIUtil.SetChecked(m_cbPassword, m_tbPassword.TextLength > 0);
		}

        private void ProcessTextChangedKeyFileBase64(object sender, EventArgs e)
        {
            if (((Program.Config.UI.KeyPromptFlags & (ulong)AceKeyUIFlags.CheckPassword) == 0) &&
                ((Program.Config.UI.KeyPromptFlags & (ulong)AceKeyUIFlags.UncheckPassword) == 0))
                UIUtil.SetChecked(m_cbKeyFile, m_keyFileBase64.TextLength > 0);
        }

        private void ProcessTextChangedUserBase64(object sender, EventArgs e)
        {
            if (((Program.Config.UI.KeyPromptFlags & (ulong)AceKeyUIFlags.CheckPassword) == 0) &&
                ((Program.Config.UI.KeyPromptFlags & (ulong)AceKeyUIFlags.UncheckPassword) == 0))
                UIUtil.SetChecked(m_cbUserAccount, m_userBase64.TextLength > 0);
        }

        //private void OnCheckedHidePassword(object sender, EventArgs e)
        //{
        //	bool bHide = m_cbHidePassword.Checked;
        //	if(!bHide && !AppPolicy.Try(AppPolicyId.UnhidePasswords))
        //	{
        //		m_cbHidePassword.Checked = true;
        //		return;
        //	}

        //	m_secPassword.EnableProtection(bHide);

        //	if(!m_bInitializing) UIUtil.SetFocus(m_tbPassword, this);
        //}

        private void OnBtnOK(object sender, EventArgs e)
		{
			if(!CreateCompositeKey()) this.DialogResult = DialogResult.None;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
			m_pKey = null;
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			if(m_bSecureDesktop)
			{
				m_bShowHelpAfterClose = true;
				this.DialogResult = DialogResult.Cancel;
			}
			else AppHelp.ShowHelp(AppDefs.HelpTopics.KeySources, null);
		}

		private void OnClickKeyFileBrowse(object sender, EventArgs e)
		{
			string strFile = null;
			if(m_bSecureDesktop)
			{
				FileBrowserForm dlg = new FileBrowserForm();
				dlg.InitEx(false, KPRes.KeyFileSelect, KPRes.SecDeskFileDialogHint,
					AppDefs.FileDialogContext.KeyFile);
				if(dlg.ShowDialog() == DialogResult.OK)
					strFile = dlg.SelectedFile;
				UIUtil.DestroyForm(dlg);
			}
			else
			{
				string strFilter = UIUtil.CreateFileTypeFilter("key", KPRes.KeyFiles, true);
				OpenFileDialogEx ofd = UIUtil.CreateOpenFileDialog(KPRes.KeyFileSelect,
					strFilter, 2, null, false, AppDefs.FileDialogContext.KeyFile);

				if(ofd.ShowDialog() == DialogResult.OK)
					strFile = ofd.FileName;
			}

			if(!string.IsNullOrEmpty(strFile))
			{
				if((Program.Config.UI.KeyPromptFlags &
					(ulong)AceKeyUIFlags.UncheckKeyFile) == 0)
					UIUtil.SetChecked(m_cbKeyFile, true);

				AddKeyFileSuggPriv(strFile, true);
			}

			EnableUserControls();
		}

		private void OnKeyFileSelectedIndexChanged(object sender, EventArgs e)
		{
			if(m_bInitializing) return;

			string strKeyFile = m_cmbKeyFile.Text;
			Debug.Assert(strKeyFile != null); if(strKeyFile == null) strKeyFile = string.Empty;
			if(!strKeyFile.Equals(KPRes.NoKeyFileSpecifiedMeta))
			{
				// if(ValidateKeyFile())
				// {
					if((Program.Config.UI.KeyPromptFlags &
						(ulong)AceKeyUIFlags.UncheckKeyFile) == 0)
						UIUtil.SetChecked(m_cbKeyFile, true);
				// }
			}
			else if((Program.Config.UI.KeyPromptFlags &
				(ulong)AceKeyUIFlags.CheckKeyFile) == 0)
				UIUtil.SetChecked(m_cbKeyFile, false);

			EnableUserControls();
		}

		private void AsyncFormLoad()
		{
			try { PopulateKeyFileSuggestions(); }
			catch(Exception) { Debug.Assert(false); }
		}

		private void PopulateKeyFileSuggestions()
		{
			if(Program.Config.Integration.SearchKeyFiles)
			{
				bool bSearchOnRemovable = Program.Config.Integration.SearchKeyFilesOnRemovableMedia;

				DriveInfo[] vDrives = DriveInfo.GetDrives();
				foreach(DriveInfo di in vDrives)
				{
					if(di.DriveType == DriveType.NoRootDirectory)
						continue;
					else if((di.DriveType == DriveType.Removable) && !bSearchOnRemovable)
						continue;
					else if(di.DriveType == DriveType.CDRom)
						continue;

					ThreadPool.QueueUserWorkItem(new WaitCallback(
						this.AddKeyDriveSuggAsync), di);
				}
			}
		}

		private void AddKeyDriveSuggAsync(object oDriveInfo)
		{
			try
			{
				DriveInfo di = (oDriveInfo as DriveInfo);
				if(di == null) { Debug.Assert(false); return; }

				if(!di.IsReady) return;

				List<FileInfo> lFiles = UrlUtil.GetFileInfos(di.RootDirectory,
					"*." + AppDefs.FileExtension.KeyFile,
					SearchOption.TopDirectoryOnly);

				foreach(FileInfo fi in lFiles)
					AddKeyFileSuggAsync(fi.FullName, null);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private void AddKeyFileSuggAsync(string str, bool? obSelect)
		{
			if(m_cmbKeyFile.InvokeRequired)
				m_cmbKeyFile.BeginInvoke(new AkfsDelegate(
					this.AddKeyFileSuggPriv), str, obSelect);
			else AddKeyFileSuggPriv(str, obSelect);
		}

		private delegate void AkfsDelegate(string str, bool? obSelect);
		private void AddKeyFileSuggPriv(string str, bool? obSelect)
		{
			try
			{
				if(m_bDisposed) return; // Slow drive
				if(string.IsNullOrEmpty(str)) { Debug.Assert(false); return; }

				int iIndex = m_lKeyFileNames.IndexOf(str);
				if(iIndex < 0)
				{
					iIndex = m_lKeyFileNames.Count;
					m_lKeyFileNames.Add(str);
					// m_lKeyFileImages.Add(img);
					if(m_cmbKeyFile.Items.Add(str) != iIndex) { Debug.Assert(false); }
				}

				if(obSelect.HasValue)
				{
					if(obSelect.Value) m_cmbKeyFile.SelectedIndex = iIndex;
				}
				else if((m_aKeyAssoc != null) && m_bCanModKeyFile)
				{
					if(str.Equals(m_aKeyAssoc.KeyFilePath, StrUtil.CaseIgnoreCmp) ||
						str.Equals(m_aKeyAssoc.KeyProvider, StrUtil.CaseIgnoreCmp))
					{
						m_cmbKeyFile.SelectedIndex = iIndex;
						m_bCanModKeyFile = false;
					}
				}
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnBtnExit(object sender, EventArgs e)
		{
			if(!m_bCanExit)
			{
				Debug.Assert(false);
				this.DialogResult = DialogResult.None;
				return;
			}

			m_bHasExited = true;
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			// if(m_bRedirectActivation) Program.MainForm.RedirectActivationPop();
			CleanUpEx();
		}
	}
}
