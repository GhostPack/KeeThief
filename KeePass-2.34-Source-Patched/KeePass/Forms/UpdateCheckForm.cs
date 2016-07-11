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

using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class UpdateCheckForm : Form, IGwmWindow
	{
		private List<UpdateComponentInfo> m_lInfo = null;
		private ImageList m_ilIcons = null;

		public bool CanCloseWithoutDataLoss { get { return true; } }

		public void InitEx(List<UpdateComponentInfo> lInfo, bool bModal)
		{
			m_lInfo = lInfo;

			if(!bModal) this.ShowInTaskbar = true;
		}

		public UpdateCheckForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if(m_lInfo == null) throw new InvalidOperationException();

			GlobalWindowManager.AddWindow(this, this);

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_WWW, KPRes.UpdateCheck,
				KPRes.UpdateCheckResults);
			this.Icon = Properties.Resources.KeePass;
			this.Text = KPRes.UpdateCheck + " - " + PwDefs.ShortProductName;

			UIUtil.SetExplorerTheme(m_lvInfo, true);

			m_lvInfo.Columns.Add(KPRes.Component);
			m_lvInfo.Columns.Add(KPRes.Status);
			m_lvInfo.Columns.Add(KPRes.Installed);
			m_lvInfo.Columns.Add(KPRes.Available);

			List<Image> lImages = new List<Image>();
			lImages.Add(Properties.Resources.B16x16_Help);
			lImages.Add(Properties.Resources.B16x16_Apply);
			lImages.Add(Properties.Resources.B16x16_Redo);
			lImages.Add(Properties.Resources.B16x16_History);
			lImages.Add(Properties.Resources.B16x16_Error);
			m_ilIcons = UIUtil.BuildImageListUnscaled(lImages,
				DpiUtil.ScaleIntX(16), DpiUtil.ScaleIntY(16));

			m_lvInfo.SmallImageList = m_ilIcons;

			string strCat = string.Empty;
			ListViewGroup lvg = null;
			const uint uMinComp = 2;

			foreach(UpdateComponentInfo uc in m_lInfo)
			{
				if(uc.Category != strCat)
				{
					lvg = new ListViewGroup(uc.Category);
					m_lvInfo.Groups.Add(lvg);
					strCat = uc.Category;
				}

				ListViewItem lvi = new ListViewItem(uc.Name);

				string strStatus = KPRes.Unknown + ".";
				if(uc.Status == UpdateComponentStatus.UpToDate)
				{
					strStatus = KPRes.UpToDate + ".";
					lvi.ImageIndex = 1;
				}
				else if(uc.Status == UpdateComponentStatus.NewVerAvailable)
				{
					strStatus = KPRes.NewVersionAvailable + "!";
					lvi.ImageIndex = 2;
				}
				else if(uc.Status == UpdateComponentStatus.PreRelease)
				{
					strStatus = KPRes.PreReleaseVersion + ".";
					lvi.ImageIndex = 3;
				}
				else if(uc.Status == UpdateComponentStatus.DownloadFailed)
				{
					strStatus = KPRes.UpdateCheckFailedNoDl;
					lvi.ImageIndex = 4;
				}
				else lvi.ImageIndex = 0;

				lvi.SubItems.Add(strStatus);
				lvi.SubItems.Add(StrUtil.VersionToString(uc.VerInstalled, uMinComp));

				if((uc.Status == UpdateComponentStatus.UpToDate) ||
					(uc.Status == UpdateComponentStatus.NewVerAvailable) ||
					(uc.Status == UpdateComponentStatus.PreRelease))
					lvi.SubItems.Add(StrUtil.VersionToString(uc.VerAvailable, uMinComp));
				else lvi.SubItems.Add("?");

				if(lvg != null) lvi.Group = lvg;
				m_lvInfo.Items.Add(lvi);
			}

			UIUtil.ResizeColumns(m_lvInfo, new int[] { 2, 2, 1, 1 }, true);
		}

		private void CleanUpEx()
		{
			if(m_ilIcons != null)
			{
				m_lvInfo.SmallImageList = null; // Detach event handlers
				m_ilIcons.Dispose();
				m_ilIcons = null;
			}
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			CleanUpEx();
			GlobalWindowManager.RemoveWindow(this);
		}

		private void OnLinkWeb(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenUrl(PwDefs.HomepageUrl);
		}

		private void OnLinkPlugins(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenUrl(PwDefs.PluginsUrl);
		}

		private void OpenUrl(string strUrl)
		{
			if(!KeePassLib.Native.NativeLib.IsUnix())
			{
				// Process.Start has a considerable delay when opening URLs
				// here (different thread, etc.), therefore try the native
				// ShellExecute first (which doesn't have any delay)
				try
				{
					IntPtr h = NativeMethods.ShellExecute(this.Handle,
						null, strUrl, null, null, NativeMethods.SW_SHOW);
					long l = h.ToInt64();
					if((l < 0) || (l > 32)) return;
					else { Debug.Assert(false); }
				}
				catch(Exception) { Debug.Assert(false); }
			}

			try { Process.Start(strUrl); }
			catch(Exception) { Debug.Assert(false); }
		}

		private void OnInfoItemActivate(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvInfo.SelectedItems;
			if((lvsic == null) || (lvsic.Count != 1)) { Debug.Assert(false); return; }
			ListViewItem lvi = lvsic[0];
			if((lvi == null) || (lvi.Group == null)) { Debug.Assert(false); return; }

			string strGroup = (lvi.Group.Header ?? string.Empty);
			if(strGroup == PwDefs.ShortProductName)
				OpenUrl(PwDefs.HomepageUrl);
			else if(strGroup == KPRes.Plugins)
				OpenUrl(PwDefs.PluginsUrl);
			else { Debug.Assert(false); }
		}
	}
}
