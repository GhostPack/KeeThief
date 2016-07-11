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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using KeePass.Native;
using KeePass.Resources;
using KeePass.UI;
using KeePass.Util;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class FileBrowserForm : Form
	{
		private bool m_bSaveMode = false;
		private string m_strTitle = PwDefs.ShortProductName;
		private string m_strHint = string.Empty;
		private string m_strContext = null;

		private ImageList m_ilFolders = null;
		private List<Image> m_vFolderImages = new List<Image>();
		private ImageList m_ilFiles = null;
		private List<Image> m_vFileImages = new List<Image>();

		private int m_nIconDim = DpiUtil.ScaleIntY(16);

		private const string StrDummyNode = "66913D76EA3F4F2A8B1A0899B7322EC3";

		private sealed class FbfPrivTviComparer : IComparer<TreeNode>
		{
			public int Compare(TreeNode x, TreeNode y)
			{
				Debug.Assert((x != null) && (y != null));
				return StrUtil.CompareNaturally(x.Text, y.Text);
			}
		}

		private sealed class FbfPrivLviComparer : IComparer<ListViewItem>
		{
			public int Compare(ListViewItem x, ListViewItem y)
			{
				Debug.Assert((x != null) && (y != null));
				return StrUtil.CompareNaturally(x.Text, y.Text);
			}
		}

		private string m_strSelectedFile = null;
		public string SelectedFile
		{
			get { return m_strSelectedFile; }
		}

		public void InitEx(bool bSaveMode, string strTitle, string strHint,
			string strContext)
		{
			m_bSaveMode = bSaveMode;
			if(strTitle != null) m_strTitle = strTitle;
			if(strHint != null) m_strHint = strHint;
			m_strContext = strContext;
		}

		public FileBrowserForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;
			this.Text = m_strTitle;

			m_nIconDim = m_tvFolders.ItemHeight;

			if(UIUtil.VistaStyleListsSupported)
			{
				m_tvFolders.ShowLines = false;

				UIUtil.SetExplorerTheme(m_tvFolders, true);
				UIUtil.SetExplorerTheme(m_lvFiles, true);
			}

			m_btnOK.Text = (m_bSaveMode ? KPRes.SaveCmd : KPRes.OpenCmd);
			m_lblHint.Text = m_strHint;

			if(UIUtil.ColorsEqual(m_lblHint.ForeColor, Color.Black))
				m_lblHint.ForeColor = Color.FromArgb(96, 96, 96);

			int nWidth = m_lvFiles.ClientSize.Width - UIUtil.GetVScrollBarWidth();
			m_lvFiles.Columns.Add(KPRes.Name, (nWidth * 3) / 4);
			m_lvFiles.Columns.Add(KPRes.Size, nWidth / 4, HorizontalAlignment.Right);

			InitialPopulateFolders();

			string strWorkDir = Program.Config.Application.GetWorkingDirectory(m_strContext);
			if(string.IsNullOrEmpty(strWorkDir))
				strWorkDir = WinUtil.GetHomeDirectory();
			BrowseToFolder(strWorkDir);

			EnableControlsEx();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			m_tvFolders.Nodes.Clear();
			m_lvFiles.Items.Clear();
			m_tvFolders.ImageList = null;
			m_lvFiles.SmallImageList = null;

			if(m_ilFolders != null) { m_ilFolders.Dispose(); m_ilFolders = null; }
			if(m_ilFiles != null) { m_ilFiles.Dispose(); m_ilFiles = null; }

			foreach(Image imgFld in m_vFolderImages) imgFld.Dispose();
			m_vFolderImages.Clear();
			foreach(Image imgFile in m_vFileImages) imgFile.Dispose();
			m_vFileImages.Clear();

			GlobalWindowManager.RemoveWindow(this);
		}

		private void EnableControlsEx()
		{
			m_btnOK.Enabled = (m_lvFiles.SelectedIndices.Count == 1);
		}

		private void InitialPopulateFolders()
		{
			List<TreeNode> l = new List<TreeNode>();

			string str = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			if(!string.IsNullOrEmpty(str))
			{
				TreeNode tn = CreateFolderNode(str, false, null);
				if(tn != null) l.Add(tn);
			}

			str = Environment.GetEnvironmentVariable("USERPROFILE");
			if(!string.IsNullOrEmpty(str))
			{
				TreeNode tn = CreateFolderNode(str, false, null);
				if(tn != null) l.Add(tn);
			}

			DriveInfo[] vDrives = DriveInfo.GetDrives();
			foreach(DriveInfo drv in vDrives)
			{
				try
				{
					DirectoryInfo diDrive = drv.RootDirectory;
					TreeNode tn = CreateFolderNode(diDrive.FullName, true, drv);
					if(tn != null) l.Add(tn);
				}
				catch(Exception) { Debug.Assert(false); }
			}

			RebuildFolderImageList();
			m_tvFolders.Nodes.AddRange(l.ToArray());
		}

		private void GetObjectProps(string strPath, DriveInfo drvHint,
			out Image img, ref string strDisplayName)
		{
			GetObjectPropsUnscaled(strPath, drvHint, out img, ref strDisplayName);

			if(img != null)
			{
				if((img.Width != m_nIconDim) || (img.Height != m_nIconDim))
				{
					Image imgScaled = GfxUtil.ScaleImage(img, m_nIconDim,
						m_nIconDim, ScaleTransformFlags.UIIcon);
					img.Dispose(); // Dispose unscaled version
					img = imgScaled;
				}
			}
		}

		private void GetObjectPropsUnscaled(string strPath, DriveInfo drvHint,
			out Image img, ref string strDisplayName)
		{
			img = null;

			try
			{
				string strName;
				NativeMethods.SHGetFileInfo(strPath, m_nIconDim, out img,
					out strName);

				if(!string.IsNullOrEmpty(strName) && (strName.IndexOf(
					Path.DirectorySeparatorChar) < 0))
					strDisplayName = strName;

				if(img != null) return;
			}
			catch(Exception) { Debug.Assert(false); }

			ImageList.ImageCollection icons = Program.MainForm.ClientIcons.Images;

			if((strPath.Length <= 3) && (drvHint != null))
			{
				switch(drvHint.DriveType)
				{
					case DriveType.Fixed:
						img = new Bitmap(icons[(int)PwIcon.Drive]);
						break;
					case DriveType.CDRom:
						img = new Bitmap(icons[(int)PwIcon.CDRom]);
						break;
					case DriveType.Network:
						img = new Bitmap(icons[(int)PwIcon.NetworkServer]);
						break;
					case DriveType.Ram:
						img = new Bitmap(icons[(int)PwIcon.Memory]);
						break;
					case DriveType.Removable:
						img = new Bitmap(icons[(int)PwIcon.Disk]);
						break;
					default:
						img = new Bitmap(icons[(int)PwIcon.Folder]);
						break;
				}

				return;
			}

			img = UIUtil.GetFileIcon(strPath, m_nIconDim, m_nIconDim);
			if(img != null) return;

			if(Directory.Exists(strPath))
			{
				img = new Bitmap(icons[(int)PwIcon.Folder]);
				return;
			}
			if(File.Exists(strPath))
			{
				img = new Bitmap(icons[(int)PwIcon.PaperNew]);
				return;
			}

			Debug.Assert(false);
			img = new Bitmap(icons[(int)PwIcon.Star]);
		}

		private TreeNode CreateFolderNode(string strDir, bool bForcePlusMinus,
			DriveInfo drvHint)
		{
			try
			{
				DirectoryInfo di = new DirectoryInfo(strDir);

				Image img;
				string strText = di.Name;
				GetObjectProps(di.FullName, drvHint, out img, ref strText);

				m_vFolderImages.Add(img);

				TreeNode tn = new TreeNode(strText, m_vFolderImages.Count - 1,
					m_vFolderImages.Count - 1);
				tn.Tag = di.FullName;

				InitNodePlusMinus(tn, di, bForcePlusMinus);
				return tn;
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		private static void InitNodePlusMinus(TreeNode tn, DirectoryInfo di,
			bool bForce)
		{
			bool bMark = true;

			if(!bForce)
			{
				try
				{
					DirectoryInfo[] vDirs = di.GetDirectories();
					bool bFoundDir = false;
					foreach(DirectoryInfo diSub in vDirs)
					{
						if(!IsValidFileSystemObject(diSub)) continue;

						bFoundDir = true;
						break;
					}

					if(!bFoundDir) bMark = false;
				}
				catch(Exception) { bMark = false; } // Usually unauthorized
			}

			if(bMark)
			{
				tn.Nodes.Add(StrDummyNode);
				tn.Collapse();
			}
		}

		private void RebuildFolderImageList()
		{
			ImageList imgNew = UIUtil.BuildImageListUnscaled(
				m_vFolderImages, m_nIconDim, m_nIconDim);
			m_tvFolders.ImageList = imgNew;

			if(m_ilFolders != null) m_ilFolders.Dispose();
			m_ilFolders = imgNew;
		}

		private void BuildFilesList(DirectoryInfo di)
		{
			m_lvFiles.BeginUpdate();
			m_lvFiles.Items.Clear();

			DirectoryInfo[] vDirs;
			FileInfo[] vFiles;
			try
			{
				vDirs = di.GetDirectories();
				vFiles = di.GetFiles();
			}
			catch(Exception) { m_lvFiles.EndUpdate(); return; } // Unauthorized

			foreach(Image imgFile in m_vFileImages) imgFile.Dispose();
			m_vFileImages.Clear();

			List<ListViewItem> lDirItems = new List<ListViewItem>();
			List<ListViewItem> lFileItems = new List<ListViewItem>();

			foreach(DirectoryInfo diSub in vDirs)
			{
				AddFileItem(diSub, m_vFileImages, lDirItems, -1);
			}
			foreach(FileInfo fi in vFiles)
			{
				AddFileItem(fi, m_vFileImages, lFileItems, fi.Length);
			}

			m_lvFiles.SmallImageList = null;
			if(m_ilFiles != null) m_ilFiles.Dispose();
			m_ilFiles = UIUtil.BuildImageListUnscaled(m_vFileImages, m_nIconDim, m_nIconDim);
			m_lvFiles.SmallImageList = m_ilFiles;

			lDirItems.Sort(new FbfPrivLviComparer());
			m_lvFiles.Items.AddRange(lDirItems.ToArray());
			lFileItems.Sort(new FbfPrivLviComparer());
			m_lvFiles.Items.AddRange(lFileItems.ToArray());
			m_lvFiles.EndUpdate();

			EnableControlsEx();
		}

		private static bool IsValidFileSystemObject(FileSystemInfo fsi)
		{
			if(fsi == null) { Debug.Assert(false); return false; }

			string strName = fsi.Name;
			if(string.IsNullOrEmpty(strName) || (strName == ".") ||
				(strName == "..")) return false;
			if(strName.EndsWith(".lnk", StrUtil.CaseIgnoreCmp)) return false;
			if(strName.EndsWith(".url", StrUtil.CaseIgnoreCmp)) return false;

			FileAttributes fa = fsi.Attributes;
			if((long)(fa & FileAttributes.ReparsePoint) != 0) return false;
			if(((long)(fa & FileAttributes.System) != 0) &&
				((long)(fa & FileAttributes.Hidden) != 0)) return false;

			return true;
		}

		private void AddFileItem(FileSystemInfo fsi, List<Image> lImages,
			List<ListViewItem> lItems, long lFileLength)
		{
			if(!IsValidFileSystemObject(fsi)) return;

			Image img;
			string strText = fsi.Name;
			GetObjectProps(fsi.FullName, null, out img, ref strText);

			lImages.Add(img);

			ListViewItem lvi = new ListViewItem(strText, lImages.Count - 1);
			lvi.Tag = fsi.FullName;

			if(lFileLength < 0) lvi.SubItems.Add(string.Empty);
			else lvi.SubItems.Add(StrUtil.FormatDataSizeKB((ulong)lFileLength));

			lItems.Add(lvi);
		}

		private bool PerformFileSelection()
		{
			ListView.SelectedListViewItemCollection lvsic = m_lvFiles.SelectedItems;
			if((lvsic == null) || (lvsic.Count != 1)) { Debug.Assert(false); return false; }

			string str = (lvsic[0].Tag as string);
			if(string.IsNullOrEmpty(str)) { Debug.Assert(false); return false; }

			try
			{
				if(Directory.Exists(str))
				{
					TreeNode tn = m_tvFolders.SelectedNode;
					if(tn == null) { Debug.Assert(false); return false; }

					if(!tn.IsExpanded) tn.Expand();

					foreach(TreeNode tnSub in tn.Nodes)
					{
						string strSub = (tnSub.Tag as string);
						if(string.IsNullOrEmpty(strSub)) { Debug.Assert(false); continue; }

						if(strSub.Equals(str, StrUtil.CaseIgnoreCmp))
						{
							m_tvFolders.SelectedNode = tnSub;
							tnSub.EnsureVisible();
							return false; // Success, but not a file selection!
						}
					}

					Debug.Assert(false);
				}
				else if(File.Exists(str))
				{
					m_strSelectedFile = str;

					Program.Config.Application.SetWorkingDirectory(m_strContext,
						UrlUtil.GetFileDirectory(str, false, true));

					return true;
				}
				else { Debug.Assert(false); }
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			if(!PerformFileSelection()) this.DialogResult = DialogResult.None;
		}

		private void OnFilesItemActivate(object sender, EventArgs e)
		{
			if(PerformFileSelection()) this.DialogResult = DialogResult.OK;
		}

		private void OnFoldersBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			TreeNode tn = e.Node;
			if(tn == null) { Debug.Assert(false); e.Cancel = true; return; }

			if((tn.Nodes.Count == 1) && (tn.Nodes[0].Text == StrDummyNode))
			{
				tn.Nodes.Clear();
				List<TreeNode> lNodes = new List<TreeNode>();

				try
				{
					DirectoryInfo di = new DirectoryInfo(tn.Tag as string);
					DirectoryInfo[] vSubDirs = di.GetDirectories();
					foreach(DirectoryInfo diSub in vSubDirs)
					{
						if(!IsValidFileSystemObject(diSub)) continue;

						TreeNode tnSub = CreateFolderNode(diSub.FullName, false, null);
						if(tnSub != null) lNodes.Add(tnSub);
					}
				}
				catch(Exception) { Debug.Assert(false); }

				RebuildFolderImageList();
				lNodes.Sort(new FbfPrivTviComparer());
				tn.Nodes.AddRange(lNodes.ToArray());
			}
		}

		private void BrowseToFolder(string strPath)
		{
			try
			{
				DirectoryInfo di = new DirectoryInfo(strPath);
				string[] vPath = di.FullName.Split(new char[]{ Path.DirectorySeparatorChar });
				if((vPath == null) || (vPath.Length == 0)) { Debug.Assert(false); return; }

				TreeNode tn = null;
				string str = string.Empty;
				for(int i = 0; i < vPath.Length; ++i)
				{
					if(i > 0) str = UrlUtil.EnsureTerminatingSeparator(str, false);
					str += vPath[i];
					if(i == 0) str = UrlUtil.EnsureTerminatingSeparator(str, false);

					TreeNodeCollection tnc = ((tn != null) ? tn.Nodes : m_tvFolders.Nodes);
					tn = null;

					foreach(TreeNode tnSub in tnc)
					{
						string strSub = (tnSub.Tag as string);
						if(string.IsNullOrEmpty(strSub)) { Debug.Assert(false); continue; }

						if(strSub.Equals(str, StrUtil.CaseIgnoreCmp))
						{
							tn = tnSub;
							break;
						}
					}

					if(tn == null) { Debug.Assert(false); break; }

					if((i != (vPath.Length - 1)) && !tn.IsExpanded) tn.Expand();
				}

				if(tn != null)
				{
					m_tvFolders.SelectedNode = tn;
					tn.EnsureVisible();
				}
				else { Debug.Assert(false); }
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private void OnFoldersAfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNode tn = e.Node;
			string strPath = (tn.Tag as string);
			if(strPath == null) { Debug.Assert(false); return; }

			try
			{
				DirectoryInfo di = new DirectoryInfo(strPath);
				BuildFilesList(di);
			}
			catch(Exception) { Debug.Assert(false); }
		}

		private void OnFilesSelectedIndexChanged(object sender, EventArgs e)
		{
			EnableControlsEx();
		}
	}
}
