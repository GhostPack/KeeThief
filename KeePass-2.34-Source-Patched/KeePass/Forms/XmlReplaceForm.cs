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
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class XmlReplaceForm : Form
	{
		private PwDatabase m_pd = null;

		private Image m_imgWarning = null;

		public void InitEx(PwDatabase pd)
		{
			m_pd = pd;
		}

		public XmlReplaceForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if(m_pd == null) { Debug.Assert(false); throw new InvalidOperationException(); }

			GlobalWindowManager.AddWindow(this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				KeePass.Properties.Resources.B48x48_Binary, KPRes.XmlReplace,
				KPRes.XmlReplaceDesc);

			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.XmlReplace;

			Bitmap bmpBig = SystemIcons.Warning.ToBitmap();
			m_imgWarning = GfxUtil.ScaleImage(bmpBig, DpiUtil.ScaleIntX(16),
				DpiUtil.ScaleIntY(16), ScaleTransformFlags.UIIcon);
			bmpBig.Dispose();
			m_picWarning.Image = m_imgWarning;

			FontUtil.AssignDefaultBold(m_rbRemove);
			FontUtil.AssignDefaultBold(m_rbReplace);

			m_rbReplace.Checked = true;
			m_rbInnerText.Checked = true;

			EnableControlsEx();
		}

		private void CleanUpEx()
		{
			if(m_imgWarning != null)
			{
				m_picWarning.Image = null;
				m_imgWarning.Dispose();
				m_imgWarning = null;
			}
			else { Debug.Assert(false); }
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}

		private void EnableControlsEx()
		{
			m_pnlReplace.Enabled = m_rbReplace.Checked;
		}

		private void OnRemoveCheckedChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			this.Enabled = false;

			try
			{
				XmlReplaceOptions opt = new XmlReplaceOptions();
				XmlReplaceFlags f = XmlReplaceFlags.StatusUI;
				opt.ParentForm = this;

				opt.SelectNodesXPath = m_tbSelNodes.Text;

				if(m_rbRemove.Checked) opt.Operation = XmlReplaceOp.RemoveNodes;
				else if(m_rbReplace.Checked) opt.Operation = XmlReplaceOp.ReplaceData;
				else { Debug.Assert(false); }

				if(m_rbInnerText.Checked) opt.Data = XmlReplaceData.InnerText;
				else if(m_rbInnerXml.Checked) opt.Data = XmlReplaceData.InnerXml;
				else if(m_rbOuterXml.Checked) opt.Data = XmlReplaceData.OuterXml;
				else { Debug.Assert(false); }

				if(m_cbCase.Checked) f |= XmlReplaceFlags.CaseSensitive;
				if(m_cbRegex.Checked) f |= XmlReplaceFlags.Regex;

				opt.FindText = m_tbMatch.Text;
				opt.ReplaceText = m_tbReplace.Text;

				opt.Flags = f;
				XmlUtil.Replace(m_pd, opt);
				this.Enabled = true;
			}
			catch(Exception ex)
			{
				this.Enabled = true;
				MessageService.ShowWarning(ex.Message);
				this.DialogResult = DialogResult.None;
			}
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void OnBtnHelp(object sender, EventArgs e)
		{
			AppHelp.ShowHelp(AppDefs.HelpTopics.XmlReplace, null);
		}
	}
}
