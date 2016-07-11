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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.UI;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Native;
using KeePassLib.Security;
using KeePassLib.Delegates;
using KeePassLib.Resources;
using KeePassLib.Translation;
using KeePassLib.Utility;

namespace KeePass.Forms
{
	public partial class PrintForm : Form
	{
		private PwGroup m_pgDataSource = null;
		private bool m_bPrintMode = true;
		private int m_nDefaultSortColumn = -1;

		private bool m_bBlockPreviewRefresh = false;
		private string m_strGeneratedHtml = string.Empty;

		private ImageList m_ilTabIcons = null;

		public string GeneratedHtml
		{
			get { return m_strGeneratedHtml; }
		}

		public void InitEx(PwGroup pgDataSource, bool bPrintMode, int nDefaultSortColumn)
		{
			Debug.Assert(pgDataSource != null); if(pgDataSource == null) throw new ArgumentNullException("pgDataSource");

			m_pgDataSource = pgDataSource;
			m_bPrintMode = bPrintMode;
			m_nDefaultSortColumn = nDefaultSortColumn;
		}

		public PrintForm()
		{
			InitializeComponent();
			Program.Translation.ApplyTo(this);
		}

		private void CreateDialogBanner()
		{
			string strTitle, strDesc;

			if(m_bPrintMode)
			{
				strTitle = KPRes.Print;
				strDesc = KPRes.PrintDesc;
			}
			else // HTML export mode
			{
				strTitle = KPRes.ExportHtml;
				strDesc = KPRes.ExportHtmlDesc;
			}

			BannerFactory.CreateBannerEx(this, m_bannerImage,
				Properties.Resources.B48x48_FilePrint, strTitle, strDesc);

			this.Text = strTitle;
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			Debug.Assert(m_pgDataSource != null); if(m_pgDataSource == null) throw new ArgumentException();

			GlobalWindowManager.AddWindow(this);

			this.Icon = Properties.Resources.KeePass;
			CreateDialogBanner();

			List<Image> lTabImg = new List<Image>();
			lTabImg.Add(Properties.Resources.B16x16_XMag);
			lTabImg.Add(Properties.Resources.B16x16_Configure);

			m_ilTabIcons = UIUtil.BuildImageListUnscaled(lTabImg,
				DpiUtil.ScaleIntX(16), DpiUtil.ScaleIntY(16));
			m_tabMain.ImageList = m_ilTabIcons;

			m_tabPreview.ImageIndex = 0;
			m_tabDataLayout.ImageIndex = 1;

			UIUtil.SetButtonImage(m_btnConfigPrinter,
				Properties.Resources.B16x16_EditCopy, true);
			UIUtil.SetButtonImage(m_btnPrintPreview,
				Properties.Resources.B16x16_FileQuickPrint, true);

			FontUtil.AssignDefaultBold(m_rbTabular);
			FontUtil.AssignDefaultBold(m_rbDetails);

			if(!m_bPrintMode) m_btnOK.Text = KPRes.Export;

			m_bBlockPreviewRefresh = true;
			m_rbTabular.Checked = true;

			m_cmbSortEntries.Items.Add("(" + KPRes.None + ")");
			m_cmbSortEntries.Items.Add(KPRes.Title);
			m_cmbSortEntries.Items.Add(KPRes.UserName);
			m_cmbSortEntries.Items.Add(KPRes.Password);
			m_cmbSortEntries.Items.Add(KPRes.Url);
			m_cmbSortEntries.Items.Add(KPRes.Notes);

			AceColumnType colType = AceColumnType.Count;
			List<AceColumn> vCols = Program.Config.MainWindow.EntryListColumns;
			if((m_nDefaultSortColumn >= 0) && (m_nDefaultSortColumn < vCols.Count))
				colType = vCols[m_nDefaultSortColumn].Type;

			int nSortSel = 0;
			if(colType == AceColumnType.Title) nSortSel = 1;
			else if(colType == AceColumnType.UserName) nSortSel = 2;
			else if(colType == AceColumnType.Password) nSortSel = 3;
			else if(colType == AceColumnType.Url) nSortSel = 4;
			else if(colType == AceColumnType.Notes) nSortSel = 5;
			m_cmbSortEntries.SelectedIndex = nSortSel;
			m_bBlockPreviewRefresh = false;

			if(!m_bPrintMode) // Export to HTML
			{
				m_btnConfigPrinter.Visible = m_btnPrintPreview.Visible = false;
				m_lblPreviewHint.Visible = false;
			}

			if(!NativeLib.IsUnix())
			{
				// MSHTML may create and forget temporary files under
				// C:\\Users\\USER\\AppData\\Local\\Temp\\*.htm
				// (e.g. when printing fails); we delete these later
				Program.TempFilesPool.AddContent("*.htm", false);
			}

			UpdateHtmlDocument();
			UpdateUIState();
		}

		private void OnBtnOK(object sender, EventArgs e)
		{
			UpdateHtmlDocument();

			if(m_bPrintMode)
			{
				try { m_wbMain.ShowPrintDialog(); } // Throws in Mono 1.2.6+
				catch(NotImplementedException)
				{
					MessageService.ShowWarning(KLRes.FrameworkNotImplExcp);
				}
				catch(Exception ex) { MessageService.ShowWarning(ex); }
			}
			else m_strGeneratedHtml = UIUtil.GetWebBrowserDocument(m_wbMain);

			if(m_strGeneratedHtml == null) m_strGeneratedHtml = string.Empty;
		}

		private void OnBtnCancel(object sender, EventArgs e)
		{
		}

		private void UpdateUIState()
		{
			m_cbAutoType.Enabled = m_cbCustomStrings.Enabled = m_rbDetails.Checked;

			if(m_rbTabular.Checked)
			{
				m_cbAutoType.Checked = false;
				m_cbCustomStrings.Checked = false;
			}
		}

		private void UpdateHtmlDocument()
		{
			if(m_bBlockPreviewRefresh) return;

			m_bBlockPreviewRefresh = true;

			PwGroup pgDataSource = m_pgDataSource.CloneDeep();

			int nSortEntries = m_cmbSortEntries.SelectedIndex;
			string strSortFieldName = null;
			if(nSortEntries == 0) { } // No sort
			else if(nSortEntries == 1) strSortFieldName = PwDefs.TitleField;
			else if(nSortEntries == 2) strSortFieldName = PwDefs.UserNameField;
			else if(nSortEntries == 3) strSortFieldName = PwDefs.PasswordField;
			else if(nSortEntries == 4) strSortFieldName = PwDefs.UrlField;
			else if(nSortEntries == 5) strSortFieldName = PwDefs.NotesField;
			else { Debug.Assert(false); }
			if(strSortFieldName != null)
				SortGroupEntriesRecursive(pgDataSource, strSortFieldName);

			StringBuilder sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"");
			sb.AppendLine("\t\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");

			sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" ");
			string strLang = Program.Translation.Properties.Iso6391Code;
			if(string.IsNullOrEmpty(strLang)) strLang = "en";
			sb.AppendLine("lang=\"" + strLang + "\" xml:lang=\"" + strLang + "\">");

			sb.AppendLine("<head>");
			sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />");
			sb.Append("<title>");
			sb.Append(StrUtil.StringToHtml(pgDataSource.Name));
			sb.AppendLine("</title>");
			sb.AppendLine("<meta http-equiv=\"expires\" content=\"0\" />");
			sb.AppendLine("<meta http-equiv=\"cache-control\" content=\"no-cache\" />");
			sb.AppendLine("<meta http-equiv=\"pragma\" content=\"no-cache\" />");

			sb.AppendLine("<style type=\"text/css\"><!--");

			sb.AppendLine("body, p, div, h1, h2, h3, h4, h5, h6, ol, ul, li, td, th, dd, dt, a {");
			sb.AppendLine("\tfont-family: \"Tahoma\", \"MS Sans Serif\", \"Sans Serif\", \"Verdana\", sans-serif;");
			sb.AppendLine("\tfont-size: 10pt;");
			sb.AppendLine("}");

			sb.AppendLine("span.fserif {");
			sb.AppendLine("\tfont-family: \"Times New Roman\", serif;");
			sb.AppendLine("}");

			sb.AppendLine("h1 { font-size: 2em; }");
			sb.AppendLine("h2 {");
			sb.AppendLine("\tfont-size: 1.5em;");
			sb.AppendLine("\tcolor: #000000;");
			sb.AppendLine("\tbackground-color: #D0D0D0;");
			sb.AppendLine("\tpadding-left: 2pt;");
			sb.AppendLine("}");
			sb.AppendLine("h3 {");
			sb.AppendLine("\tfont-size: 1.2em;");
			sb.AppendLine("\tcolor: #000000;");
			sb.AppendLine("\tbackground-color: #D0D0D0;");
			sb.AppendLine("\tpadding-left: 2pt;");
			sb.AppendLine("}");
			sb.AppendLine("h4 { font-size: 1em; }");
			sb.AppendLine("h5 { font-size: 0.89em; }");
			sb.AppendLine("h6 { font-size: 0.6em; }");

			sb.AppendLine("table {");
			sb.AppendLine("\twidth: 100%;");
			sb.AppendLine("\ttable-layout: fixed;");
			sb.AppendLine("}");

			sb.AppendLine("th {");
			sb.AppendLine("\ttext-align: left;");
			sb.AppendLine("\tvertical-align: top;");
			sb.AppendLine("\tfont-weight: bold;");
			sb.AppendLine("}");

			sb.AppendLine("td {");
			sb.AppendLine("\ttext-align: left;");
			sb.AppendLine("\tvertical-align: top;");
			sb.AppendLine("}");

			sb.AppendLine("a:visited {");
			sb.AppendLine("\ttext-decoration: none;");
			sb.AppendLine("\tcolor: #0000DD;");
			sb.AppendLine("}");
			sb.AppendLine("a:active {");
			sb.AppendLine("\ttext-decoration: none;");
			sb.AppendLine("\tcolor: #6699FF;");
			sb.AppendLine("}");
			sb.AppendLine("a:link {");
			sb.AppendLine("\ttext-decoration: none;");
			sb.AppendLine("\tcolor: #0000DD;");
			sb.AppendLine("}");
			sb.AppendLine("a:hover {");
			sb.AppendLine("\ttext-decoration: underline;");
			sb.AppendLine("\tcolor: #6699FF;");
			sb.AppendLine("}");

			sb.AppendLine(".field_name {");
			sb.AppendLine("\t-webkit-hyphens: auto;");
			sb.AppendLine("\t-moz-hyphens: auto;");
			sb.AppendLine("\t-ms-hyphens: auto;");
			sb.AppendLine("\thyphens: auto;");
			sb.AppendLine("}");
			sb.AppendLine(".field_data {");
			// sb.AppendLine("\tword-break: break-all;");
			sb.AppendLine("\toverflow-wrap: break-word;");
			sb.AppendLine("\tword-wrap: break-word;");
			sb.AppendLine("}");

			// Add the temporary content identifier
			sb.AppendLine("." + Program.TempFilesPool.TempContentTag + " {");
			sb.AppendLine("\tfont-size: 10pt;");
			sb.AppendLine("}");

			sb.AppendLine("--></style>");
			sb.AppendLine("</head><body>");

			sb.AppendLine("<h2>" + StrUtil.StringToHtml(pgDataSource.Name) + "</h2>");

			GroupHandler gh = null;
			EntryHandler eh = null;

			bool bGroup = m_cbGroups.Checked;
			bool bTitle = m_cbTitle.Checked, bUserName = m_cbUser.Checked;
			bool bPassword = m_cbPassword.Checked, bURL = m_cbUrl.Checked;
			bool bNotes = m_cbNotes.Checked;
			bool bCreation = m_cbCreation.Checked, bLastMod = m_cbLastMod.Checked;
			// bool bLastAcc = m_cbLastAccess.Checked;
			bool bExpire = m_cbExpire.Checked;
			bool bAutoType = m_cbAutoType.Checked;
			bool bTags = m_cbTags.Checked;
			bool bCustomStrings = m_cbCustomStrings.Checked;
			bool bUuid = m_cbUuid.Checked;

			bool bMonoPasswords = m_cbMonospaceForPasswords.Checked;
			if(m_rbMonospace.Checked) bMonoPasswords = false; // Monospaced anyway

			bool bSmallMono = m_cbSmallMono.Checked;

			string strFontInit = string.Empty, strFontExit = string.Empty;

			if(m_rbSerif.Checked)
			{
				strFontInit = "<span class=\"fserif\">";
				strFontExit = "</span>";
			}
			else if(m_rbSansSerif.Checked)
			{
				strFontInit = string.Empty;
				strFontExit = string.Empty;
			}
			else if(m_rbMonospace.Checked)
			{
				strFontInit = (bSmallMono ? "<code><small>" : "<code>");
				strFontExit = (bSmallMono ? "</small></code>" : "</code>");
			}
			else { Debug.Assert(false); }

			string strTableInit = "<table>";
			PwGroup pgLast = null;

			if(m_rbTabular.Checked)
			{
				int nEquiCols = 0;
				if(bGroup) ++nEquiCols;
				if(bTitle) ++nEquiCols;
				if(bUserName) ++nEquiCols;
				if(bPassword) ++nEquiCols;
				if(bURL) ++nEquiCols;
				if(bNotes) nEquiCols += 2;
				if(bCreation) ++nEquiCols;
				// if(bLastAcc) ++nEquiCols;
				if(bLastMod) ++nEquiCols;
				if(bExpire) ++nEquiCols;
				if(bTags) ++nEquiCols;
				if(bUuid) ++nEquiCols;
				if(nEquiCols == 0) nEquiCols = 1;

				string strColWidth = (100.0f / (float)nEquiCols).ToString(
					"F2", NumberFormatInfo.InvariantInfo);
				string strColWidth2 = (200.0f / (float)nEquiCols).ToString(
					"F2", NumberFormatInfo.InvariantInfo);

				string strHTdInit = "<th class=\"field_name\" style=\"width: " +
					strColWidth + "%;\">";
				string strHTdInit2 = "<th class=\"field_name\" style=\"width: " +
					strColWidth2 + "%;\">";
				string strHTdExit = "</th>";

				string strCellPre = "<td class=\"field_data\">" + strFontInit;
				string strCellPost = strFontExit + "</td>";

				StringBuilder sbH = new StringBuilder();
				sbH.AppendLine();
				sbH.Append("<tr>");
				if(bGroup) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.Group) + strHTdExit);
				if(bTitle) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.Title) + strHTdExit);
				if(bUserName) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.UserName) + strHTdExit);
				if(bPassword) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.Password) + strHTdExit);
				if(bURL) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.Url) + strHTdExit);
				if(bNotes) sbH.AppendLine(strHTdInit2 + StrUtil.StringToHtml(KPRes.Notes) + strHTdExit);
				if(bCreation) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.CreationTime) + strHTdExit);
				// if(bLastAcc) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.LastAccessTime) + strHTdExit);
				if(bLastMod) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.LastModificationTime) + strHTdExit);
				if(bExpire) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.ExpiryTime) + strHTdExit);
				if(bTags) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.Tags) + strHTdExit);
				if(bUuid) sbH.AppendLine(strHTdInit + StrUtil.StringToHtml(KPRes.Uuid) + strHTdExit);
				sbH.Append("</tr>"); // No terminating \r\n

				strTableInit += sbH.ToString();
				sb.AppendLine(strTableInit);

				eh = delegate(PwEntry pe)
				{
					sb.AppendLine("<tr>");

					WriteTabularIf(bGroup, sb, StrUtil.StringToHtml(pe.ParentGroup.Name), strCellPre, strCellPost);
					WriteTabularIf(bTitle, sb, pe, PwDefs.TitleField, strCellPre, strCellPost);
					WriteTabularIf(bUserName, sb, pe, PwDefs.UserNameField, strCellPre, strCellPost);

					if(bPassword)
					{
						if(bMonoPasswords)
							sb.Append("<td class=\"field_data\">" + (bSmallMono ?
								"<code><small>" : "<code>"));
						else sb.Append(strCellPre);

						string strInner = StrUtil.StringToHtml(pe.Strings.ReadSafe(PwDefs.PasswordField));
						if(strInner.Length > 0) sb.Append(strInner);
						else sb.Append(@"&nbsp;");

						if(bMonoPasswords)
							sb.AppendLine(bSmallMono ? "</small></code></td>" : "</code></td>");
						else sb.AppendLine(strCellPost);
					}

					// WriteTabularIf(bURL, sb, pe, PwDefs.UrlField, strCellPre, strCellPost);
					WriteTabularIf(bURL, sb, MakeUrlLink(pe.Strings.ReadSafe(PwDefs.UrlField),
						strFontInit, strFontExit), strCellPre, strCellPost);

					WriteTabularIf(bNotes, sb, pe, PwDefs.NotesField, strCellPre, strCellPost);

					WriteTabularIf(bCreation, sb, StrUtil.StringToHtml(
						TimeUtil.ToDisplayString(pe.CreationTime)), strCellPre, strCellPost);
					// WriteTabularIf(bLastAcc, sb, StrUtil.StringToHtml(
					//	TimeUtil.ToDisplayString(pe.LastAccessTime)), strCellPre, strCellPost);
					WriteTabularIf(bLastMod, sb, StrUtil.StringToHtml(
						TimeUtil.ToDisplayString(pe.LastModificationTime)), strCellPre, strCellPost);
					WriteTabularIf(bExpire, sb, StrUtil.StringToHtml(pe.Expires ?
						TimeUtil.ToDisplayString(pe.ExpiryTime) : KPRes.NeverExpires),
						strCellPre, strCellPost);

					WriteTabularIf(bTags, sb, StrUtil.StringToHtml(StrUtil.TagsToString(
						pe.Tags, true)), strCellPre, strCellPost);

					WriteTabularIf(bUuid, sb, pe.Uuid.ToHexString(), strCellPre, strCellPost);

					sb.AppendLine("</tr>");
					return true;
				};
			}
			else if(m_rbDetails.Checked)
			{
				sb.AppendLine(strTableInit);

				if(pgDataSource.Entries.UCount == 0)
					sb.AppendLine(@"<tr><td>&nbsp;</td></tr>");

				eh = delegate(PwEntry pe)
				{
					if((pgLast != null) && (pgLast == pe.ParentGroup))
						sb.AppendLine("<tr><td colspan=\"2\"><hr /></td></tr>");

					if(bGroup) WriteDetailsLine(sb, KPRes.Group, pe.ParentGroup.Name, bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bTitle) WriteDetailsLine(sb, KPRes.Title, pe.Strings.ReadSafe(PwDefs.TitleField), bSmallMono, bMonoPasswords, strFontInit + "<b>", "</b>" + strFontExit);
					if(bUserName) WriteDetailsLine(sb, KPRes.UserName, pe.Strings.ReadSafe(PwDefs.UserNameField), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bPassword) WriteDetailsLine(sb, KPRes.Password, pe.Strings.ReadSafe(PwDefs.PasswordField), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bURL) WriteDetailsLine(sb, KPRes.Url, pe.Strings.ReadSafe(PwDefs.UrlField), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bNotes) WriteDetailsLine(sb, KPRes.Notes, pe.Strings.ReadSafe(PwDefs.NotesField), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bCreation) WriteDetailsLine(sb, KPRes.CreationTime, TimeUtil.ToDisplayString(
						pe.CreationTime), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					// if(bLastAcc) WriteDetailsLine(sb, KPRes.LastAccessTime, TimeUtil.ToDisplayString(
					//	pe.LastAccessTime), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bLastMod) WriteDetailsLine(sb, KPRes.LastModificationTime, TimeUtil.ToDisplayString(
						pe.LastModificationTime), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bExpire) WriteDetailsLine(sb, KPRes.ExpiryTime, (pe.Expires ? TimeUtil.ToDisplayString(
						pe.ExpiryTime) : KPRes.NeverExpires), bSmallMono, bMonoPasswords, strFontInit, strFontExit);

					if(bAutoType)
					{
						foreach(AutoTypeAssociation a in pe.AutoType.Associations)
							WriteDetailsLine(sb, KPRes.AutoType, a.WindowName +
								": " + a.Sequence, bSmallMono, bMonoPasswords,
								strFontInit, strFontExit);
					}

					if(bTags) WriteDetailsLine(sb, KPRes.Tags, StrUtil.TagsToString(
						pe.Tags, true), bSmallMono, bMonoPasswords, strFontInit, strFontExit);
					if(bUuid) WriteDetailsLine(sb, KPRes.Uuid, pe.Uuid.ToHexString(),
						bSmallMono, bMonoPasswords, strFontInit, strFontExit);

					foreach(KeyValuePair<string, ProtectedString> kvp in pe.Strings)
					{
						if(bCustomStrings && !PwDefs.IsStandardField(kvp.Key))
							WriteDetailsLine(sb, kvp, bSmallMono, bMonoPasswords,
								strFontInit, strFontExit);
					}

					pgLast = pe.ParentGroup;
					return true;
				};
			}
			else { Debug.Assert(false); }

			gh = delegate(PwGroup pg)
			{
				if(pg.Entries.UCount == 0) return true;

				sb.Append("</table><br /><br /><h3>"); // "</table><br /><hr /><h3>"
				sb.Append(StrUtil.StringToHtml(pg.GetFullPath(" - ", false)));
				sb.AppendLine("</h3>");
				sb.AppendLine(strTableInit);

				return true;
			};

			pgDataSource.TraverseTree(TraversalMethod.PreOrder, gh, eh);

			if(m_rbTabular.Checked)
				sb.AppendLine("</table>");
			else if(m_rbDetails.Checked)
				sb.AppendLine("</table><br />");

			sb.AppendLine("</body></html>");

			try { UIUtil.SetWebBrowserDocument(m_wbMain, sb.ToString()); }
			catch(Exception) { Debug.Assert(NativeLib.IsUnix()); } // Throws in Mono 2.0+
			try { m_wbMain.AllowNavigation = false; }
			catch(Exception) { Debug.Assert(false); }

			m_bBlockPreviewRefresh = false;
		}

		private static void WriteTabularIf(bool bCondition, StringBuilder sb,
			PwEntry pe, string strIndex, string strCellPre, string strCellPost)
		{
			if(!bCondition) return;

			sb.Append(strCellPre);

			string strInner = StrUtil.StringToHtml(pe.Strings.ReadSafe(strIndex));
			if(strInner.Length > 0) sb.Append(strInner);
			else sb.Append(@"&nbsp;");

			sb.AppendLine(strCellPost);
		}

		private static void WriteTabularIf(bool bCondition, StringBuilder sb,
			string strValue, string strCellPre, string strCellPost)
		{
			if(!bCondition) return;

			sb.Append(strCellPre);

			if(strValue.Length > 0) sb.Append(strValue); // Don't HTML-encode
			else sb.Append(@"&nbsp;");

			sb.AppendLine(strCellPost);
		}

		private static void WriteDetailsLine(StringBuilder sb,
			KeyValuePair<string, ProtectedString> kvp, bool bSmallMono,
			bool bMonoPasswords, string strFontInit, string strFontExit)
		{
			sb.Append("<tr><td class=\"field_name\" style=\"width: 20%;\"><i>");
			sb.Append(StrUtil.StringToHtml(kvp.Key));
			sb.AppendLine(":</i></td>");

			sb.Append("<td class=\"field_data\" style=\"width: 80%;\">");

			if(bMonoPasswords && (kvp.Key == PwDefs.PasswordField))
				sb.Append(bSmallMono ? "<code><small>" : "<code>");
			else sb.Append(strFontInit);

			if((kvp.Key == PwDefs.UrlField) && !kvp.Value.IsEmpty)
				sb.Append(MakeUrlLink(kvp.Value.ReadString(), strFontInit, strFontExit));
			else
			{
				string strInner = StrUtil.StringToHtml(kvp.Value.ReadString());
				if(strInner.Length > 0) sb.Append(strInner);
				else sb.Append("&nbsp;");
			}

			if(kvp.Key == PwDefs.PasswordField)
				sb.Append(bSmallMono ? "</small></code>" : "</code>");
			else sb.Append(strFontExit);

			sb.AppendLine("</td></tr>");
		}

		private static void WriteDetailsLine(StringBuilder sb, string strIndex,
			string strValue, bool bSmallMono, bool bMonoPasswords, string strFontInit,
			string strFontExit)
		{
			if(string.IsNullOrEmpty(strValue)) return;

			KeyValuePair<string, ProtectedString> kvp = new KeyValuePair<string, ProtectedString>(strIndex,
				new ProtectedString(false, strValue));

			WriteDetailsLine(sb, kvp, bSmallMono, bMonoPasswords, strFontInit, strFontExit);
		}

		private static string MakeUrlLink(string strRawUrl, string strFontInit,
			string strFontExit)
		{
			if(string.IsNullOrEmpty(strRawUrl)) return string.Empty;

			string strUrl = StrUtil.StringToHtml(strRawUrl);
			strUrl = "<a href=\"" + strUrl + "\">" + strFontInit + strUrl +
				strFontExit + "</a>";
			return strUrl;
		}

		private void SortGroupEntriesRecursive(PwGroup pg, string strFieldName)
		{
			PwEntryComparer cmp = new PwEntryComparer(strFieldName, true, true);
			pg.Entries.Sort(cmp);

			foreach(PwGroup pgSub in pg.Groups)
			{
				SortGroupEntriesRecursive(pgSub, strFieldName);
			}
		}

		private void OnBtnConfigPage(object sender, EventArgs e)
		{
			UpdateHtmlDocument();

			try { m_wbMain.ShowPageSetupDialog(); } // Throws in Mono 1.2.6+
			catch(NotImplementedException)
			{
				MessageService.ShowWarning(KLRes.FrameworkNotImplExcp);
			}
			catch(Exception ex) { MessageService.ShowWarning(ex); }
		}

		private void OnBtnPrintPreview(object sender, EventArgs e)
		{
			UpdateHtmlDocument();

			try { m_wbMain.ShowPrintPreviewDialog(); } // Throws in Mono 1.2.6+
			catch(NotImplementedException)
			{
				MessageService.ShowWarning(KLRes.FrameworkNotImplExcp);
			}
			catch(Exception ex) { MessageService.ShowWarning(ex); }
		}

		private void OnTabSelectedIndexChanged(object sender, EventArgs e)
		{
			if(m_tabMain.SelectedIndex == 0) UpdateHtmlDocument();
		}

		private void OnTabularCheckedChanged(object sender, EventArgs e)
		{
			UpdateUIState();
		}

		private CheckBox[] GetAllFields()
		{
			return new CheckBox[] {
				m_cbTitle, m_cbUser, m_cbPassword, m_cbUrl, m_cbNotes,
				m_cbCreation, m_cbLastMod, m_cbExpire, // m_cbLastAccess
				m_cbAutoType, m_cbTags,
				m_cbGroups, m_cbCustomStrings, m_cbUuid
			};
		}

		private void OnLinkSelectAllFields(object sender, LinkLabelLinkClickedEventArgs e)
		{
			foreach(CheckBox cb in GetAllFields()) { cb.Checked = true; }
			UpdateUIState();
		}

		private void OnLinkDeselectAllFields(object sender, LinkLabelLinkClickedEventArgs e)
		{
			foreach(CheckBox cb in GetAllFields()) { cb.Checked = false; }
			UpdateUIState();
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			if(m_ilTabIcons != null)
			{
				m_tabMain.ImageList = null;
				m_ilTabIcons.Dispose();
				m_ilTabIcons = null;
			}
			else { Debug.Assert(false); }

			GlobalWindowManager.RemoveWindow(this);
		}
	}
}