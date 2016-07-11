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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using KeePass.App;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Native;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class IOConnectionForm : Form
	{
		private bool m_bSave = false;
		private IOConnectionInfo m_ioc = new IOConnectionInfo();
		private bool m_bCanRememberCred = true;
		private bool m_bTestConnection = false;

		private List<KeyValuePair<IocPropertyInfo, Control>> m_lProps =
			new List<KeyValuePair<IocPropertyInfo, Control>>();

		public IOConnectionInfo IOConnectionInfo
		{
			get { return m_ioc; }
		}

		public void InitEx(bool bSave, IOConnectionInfo ioc, bool bCanRememberCred,
			bool bTestConnection)
		{
			m_bSave = bSave;
			if(ioc != null) m_ioc = ioc.CloneDeep();
			m_bCanRememberCred = bCanRememberCred;
			m_bTestConnection = bTestConnection;
		}

		public IOConnectionForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			// Must work without a parent window
			Debug.Assert(this.StartPosition == FormStartPosition.CenterScreen);

			InitAdvancedTab(); // After translation, before resize

			GlobalWindowManager.AddWindow(this);

			string strTitle = (m_bSave ? KPRes.UrlSaveTitle : KPRes.UrlOpenTitle);
			string strDesc = (m_bSave ? KPRes.UrlSaveDesc : KPRes.UrlOpenDesc);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				KeePass.Properties.Resources.B48x48_WWW, strTitle, strDesc);
			this.Icon = Properties.Resources.KeePass;
			this.Text = strTitle;

			FontUtil.AssignDefaultBold(m_lblUrl);
			FontUtil.AssignDefaultBold(m_lblUserName);
			FontUtil.AssignDefaultBold(m_lblPassword);
			FontUtil.AssignDefaultBold(m_lblRemember);

			m_tbUrl.Text = (m_ioc.IsLocalFile() ? string.Empty : m_ioc.Path);
			m_tbUserName.Text = m_ioc.UserName;
			m_tbPassword.Text = m_ioc.Password;

			m_cmbCredSaveMode.Items.Add(KPRes.CredSaveNone);
			m_cmbCredSaveMode.Items.Add(KPRes.CredSaveUserOnly);
			m_cmbCredSaveMode.Items.Add(KPRes.CredSaveAll);

			if(m_ioc.CredSaveMode == IOCredSaveMode.UserNameOnly)
				m_cmbCredSaveMode.SelectedIndex = 1;
			else if(m_ioc.CredSaveMode == IOCredSaveMode.SaveCred)
				m_cmbCredSaveMode.SelectedIndex = 2;
			else
				m_cmbCredSaveMode.SelectedIndex = 0;

			if(!m_bCanRememberCred)
			{
				m_cmbCredSaveMode.SelectedIndex = 0;
				m_cmbCredSaveMode.Enabled = false;
			}

			if((m_tbUrl.TextLength > 0) && (m_tbUserName.TextLength > 0))
				UIUtil.SetFocus(m_tbPassword, this);
			else if(m_tbUrl.TextLength > 0)
				UIUtil.SetFocus(m_tbUserName, this);
			else UIUtil.SetFocus(m_tbUrl, this);
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			string strUrl = m_tbUrl.Text;
			if(strUrl.IndexOf("://") < 0)
			{
				// m_ttInvalidUrl.Show(KPRes.InvalidUrl, m_tbUrl);
				MessageService.ShowWarning(strUrl, KPRes.InvalidUrl);
				this.DialogResult = DialogResult.None;
				return;
			}

			m_ioc.Path = strUrl;
			m_ioc.UserName = m_tbUserName.Text;
			m_ioc.Password = m_tbPassword.Text;

			if(m_cmbCredSaveMode.SelectedIndex == 1)
				m_ioc.CredSaveMode = IOCredSaveMode.UserNameOnly;
			else if(m_cmbCredSaveMode.SelectedIndex == 2)
				m_ioc.CredSaveMode = IOCredSaveMode.SaveCred;
			else
				m_ioc.CredSaveMode = IOCredSaveMode.NoSave;

			IocProperties p = m_ioc.Properties;
			foreach(KeyValuePair<IocPropertyInfo, Control> kvp in m_lProps)
			{
				IocPropertyInfo pi = kvp.Key;
				string strName = pi.Name;
				Type t = pi.Type;
				Control c = kvp.Value;

				try
				{
					if(t == typeof(string))
					{
						TextBox tb = (c as TextBox);
						if(tb != null) p.Set(strName, tb.Text.Trim());
						else { Debug.Assert(false); }
					}
					else if(t == typeof(bool))
					{
						ComboBox cmb = (c as ComboBox);
						if(cmb != null)
						{
							int iSel = cmb.SelectedIndex;
							if(iSel == 0) p.SetBool(strName, null);
							else p.SetBool(strName, (iSel == 1));
						}
						else { Debug.Assert(false); }
					}
					else if(t == typeof(long))
					{
						TextBox tb = (c as TextBox);
						if(tb != null)
						{
							string str = tb.Text.Trim();
							if(str.Length == 0) p.SetLong(strName, null);
							else
							{
								// Validate and store number
								long l = long.Parse(str, NumberFormatInfo.InvariantInfo);
								p.SetLong(strName, l);
							}
						}
						else { Debug.Assert(false); }
					}
					else { Debug.Assert(false); }
				}
				catch(Exception ex)
				{
					string strMsg = KPRes.Field + @" '" + pi.DisplayName +
						@"':" + MessageService.NewLine + ex.Message;
					// if(!VistaTaskDialog.ShowMessageBox(strMsg, KPRes.ValidationFailed,
					//	PwDefs.ShortProductName, VtdIcon.Warning, this))
					MessageService.ShowWarning(strMsg);

					this.DialogResult = DialogResult.None;
					return;
				}
			}

			if(m_bTestConnection && !m_bSave)
			{
				if(!TestConnectionEx())
					this.DialogResult = DialogResult.None;
			}
		}

		private bool TestConnectionEx()
		{
			bool bResult = true;
			bool bOK = m_btnOK.Enabled, bCancel = m_btnCancel.Enabled;
			bool bCombo = m_cmbCredSaveMode.Enabled;

			m_btnOK.Enabled = m_btnCancel.Enabled = m_tbUrl.Enabled =
				m_tbUserName.Enabled = m_tbPassword.Enabled =
				m_btnHelp.Enabled = m_cmbCredSaveMode.Enabled = false;

			Application.DoEvents();

			try
			{
				if(!IOConnection.FileExists(m_ioc, true))
					throw new FileNotFoundException();
			}
			catch(Exception exTest)
			{
				if(Program.CommandLineArgs[AppDefs.CommandLineOptions.Debug] != null)
					MessageService.ShowWarningExcp(m_ioc.GetDisplayName(), exTest);
				else
				{
					string strError = exTest.Message;
					if((exTest.InnerException != null) &&
						!string.IsNullOrEmpty(exTest.InnerException.Message))
						strError += MessageService.NewParagraph +
							exTest.InnerException.Message;

					MessageService.ShowWarning(m_ioc.GetDisplayName(), strError);
				}

				bResult = false;
			}

			m_btnOK.Enabled = bOK;
			m_btnCancel.Enabled = bCancel;
			m_cmbCredSaveMode.Enabled = bCombo;
			m_btnHelp.Enabled = m_tbUserName.Enabled = m_tbUrl.Enabled =
				m_tbPassword.Enabled = true;
			return bResult;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.IOConnections, null);
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			GlobalWindowManager.RemoveWindow(this);
		}

		private static Dictionary<string, List<IocPropertyInfo>> GetPropertyInfos()
		{
			Dictionary<string, List<IocPropertyInfo>> d =
				new Dictionary<string, List<IocPropertyInfo>>(StrUtil.CaseIgnoreComparer);

			foreach(IocPropertyInfo pi in IocPropertyInfoPool.PropertyInfos)
			{
				if(pi == null) { Debug.Assert(false); continue; }

				List<string> lPrt = new List<string>(pi.Protocols);
				lPrt.Sort(StrUtil.CaseIgnoreComparer);
				StringBuilder sbPrt = new StringBuilder();
				foreach(string str in lPrt)
				{
					if(sbPrt.Length > 0) sbPrt.Append(" / ");
					sbPrt.Append(str);
				}
				string strPrt = sbPrt.ToString();

				List<IocPropertyInfo> l;
				if(d.TryGetValue(strPrt, out l)) l.Add(pi);
				else
				{
					l = new List<IocPropertyInfo>();
					l.Add(pi);
					d[strPrt] = l;
				}
			}

			return d;
		}

		private void InitAdvancedTab()
		{
			IocProperties p = m_ioc.Properties;
			Dictionary<string, List<IocPropertyInfo>> dProps = GetPropertyInfos();

			const int d = 7;
			int hLabel = m_lblUrl.Height;
			int hTextBox = m_tbUrl.Height;
			int hComboBox = m_cmbCredSaveMode.Height;
			Font f = m_lblUrl.Font;

			Debug.Assert(m_pnlAdv.AutoScroll);
			m_pnlAdv.AutoScrollMargin = new Size(1, d);

			int wPanel = m_pnlAdv.ClientSize.Width - UIUtil.GetVScrollBarWidth() - 1;
			int wGroup = wPanel - (2 * d);
			int wCell = (wPanel - (3 * d)) / 2;
			int xText = d - 1;
			int xInput = d + wCell + d - 1;

			int y = 1;
			int iID = 0;

			m_pnlAdv.SuspendLayout();

			foreach(KeyValuePair<string, List<IocPropertyInfo>> kvp in dProps)
			{
				string strGroup = kvp.Key;
				y += d;

				Label lblGroup = new Label();
				lblGroup.Name = "cGroup" + iID.ToString(NumberFormatInfo.InvariantInfo);
				++iID;
				lblGroup.AutoEllipsis = true;
				lblGroup.AutoSize = false;
				lblGroup.Dock = DockStyle.None;
				lblGroup.Location = new Point(xText, y);
				lblGroup.Size = new Size(wGroup, hLabel);
				lblGroup.Text = strGroup;
				lblGroup.TextAlign = ContentAlignment.MiddleLeft;
				// lblGroup.BackColor = Color.Red;
				FontUtil.AssignDefaultBold(lblGroup);

				m_pnlAdv.Controls.Add(lblGroup);
				y += hLabel;

				foreach(IocPropertyInfo pi in kvp.Value)
				{
					string strName = pi.Name;
					string strText = pi.DisplayName + ":";
					Type t = pi.Type;
					y += d;

					int hText = hLabel;
					int wRem = TextRenderer.MeasureText(strText, f).Width;
					while(wRem >= wCell)
					{
						hText += (hLabel + 1);
						wRem -= wCell;
					}

					Label lblText = new Label();
					lblText.Name = "cText" + iID.ToString(NumberFormatInfo.InvariantInfo);
					++iID;
					lblText.AutoEllipsis = true;
					lblText.AutoSize = false;
					lblText.Dock = DockStyle.None;
					lblText.Size = new Size(wCell, hText);
					lblText.Text = strText;
					lblText.TextAlign = ContentAlignment.MiddleLeft;
					// lblText.BackColor = Color.Green;

					Control cInput = null;
					if((t == typeof(string)) || (t == typeof(long)))
					{
						TextBox tb = new TextBox();
						tb.Size = new Size(wCell, hTextBox);

						tb.Text = (p.Get(strName) ?? string.Empty);

						cInput = tb;
					}
					else if(t == typeof(bool))
					{
						ComboBox cmb = new ComboBox();
						cmb.DropDownStyle = ComboBoxStyle.DropDownList;
						cmb.Size = new Size(wCell, hComboBox);

						cmb.Items.Add(KPRes.Auto);
						cmb.Items.Add(KPRes.Yes);
						cmb.Items.Add(KPRes.No);

						bool? ob = p.GetBool(strName);
						if(ob.HasValue) cmb.SelectedIndex = (ob.Value ? 1 : 2);
						else cmb.SelectedIndex = 0;

						cInput = cmb;
					}
					else { Debug.Assert(false); continue; }

					cInput.Dock = DockStyle.None;
					cInput.Name = "cInput" + iID.ToString(NumberFormatInfo.InvariantInfo);
					++iID;

					int hDiff = lblText.Height - cInput.Height;
					if(hDiff >= 0)
					{
						lblText.Location = new Point(xText, y);
						cInput.Location = new Point(xInput, y + (hDiff / 2));

						y += lblText.Height;
					}
					else
					{
						lblText.Location = new Point(xText, y - (hDiff / 2));
						cInput.Location = new Point(xInput, y);

						y += cInput.Height;
					}

					m_pnlAdv.Controls.Add(lblText);
					m_pnlAdv.Controls.Add(cInput);

					m_lProps.Add(new KeyValuePair<IocPropertyInfo, Control>(
						pi, cInput));
				}
			}

			m_pnlAdv.ResumeLayout(true);
		}
	}
}
