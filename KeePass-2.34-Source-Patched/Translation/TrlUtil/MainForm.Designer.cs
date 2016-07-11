namespace TrlUtil
{
	partial class MainForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.m_tabMain = new System.Windows.Forms.TabControl();
			this.m_tabProps = new System.Windows.Forms.TabPage();
			this.m_cbRtl = new System.Windows.Forms.CheckBox();
			this.m_lblAuthorContact = new System.Windows.Forms.Label();
			this.m_tbAuthorContact = new System.Windows.Forms.TextBox();
			this.m_linkLangCode = new System.Windows.Forms.LinkLabel();
			this.m_lblLangIDSample = new System.Windows.Forms.Label();
			this.m_tbLangID = new System.Windows.Forms.TextBox();
			this.m_lblLangID = new System.Windows.Forms.Label();
			this.m_lblAuthorName = new System.Windows.Forms.Label();
			this.m_tbAuthorName = new System.Windows.Forms.TextBox();
			this.m_lblNameLclSample = new System.Windows.Forms.Label();
			this.m_tbNameLcl = new System.Windows.Forms.TextBox();
			this.m_lblNameLcl = new System.Windows.Forms.Label();
			this.m_lblNameEngSample = new System.Windows.Forms.Label();
			this.m_lblNameEng = new System.Windows.Forms.Label();
			this.m_tbNameEng = new System.Windows.Forms.TextBox();
			this.m_tabStrings = new System.Windows.Forms.TabPage();
			this.m_lblStrSaveHint = new System.Windows.Forms.Label();
			this.m_tbStrTrl = new System.Windows.Forms.TextBox();
			this.m_tbStrEng = new System.Windows.Forms.TextBox();
			this.m_lblStrTrl = new System.Windows.Forms.Label();
			this.m_lblStrEng = new System.Windows.Forms.Label();
			this.m_lvStrings = new KeePass.UI.CustomListViewEx();
			this.m_tabDialogs = new System.Windows.Forms.TabPage();
			this.m_lblIconColorHint = new System.Windows.Forms.Label();
			this.m_grpControl = new System.Windows.Forms.GroupBox();
			this.m_grpControlLayout = new System.Windows.Forms.GroupBox();
			this.m_lblLayoutHint2 = new System.Windows.Forms.Label();
			this.m_lblLayoutHint = new System.Windows.Forms.Label();
			this.m_tbLayoutH = new System.Windows.Forms.TextBox();
			this.m_tbLayoutW = new System.Windows.Forms.TextBox();
			this.m_tbLayoutY = new System.Windows.Forms.TextBox();
			this.m_tbLayoutX = new System.Windows.Forms.TextBox();
			this.m_lblLayoutH = new System.Windows.Forms.Label();
			this.m_lblLayoutW = new System.Windows.Forms.Label();
			this.m_lblLayoutY = new System.Windows.Forms.Label();
			this.m_lblLayoutX = new System.Windows.Forms.Label();
			this.m_tbCtrlTrlText = new System.Windows.Forms.TextBox();
			this.m_tbCtrlEngText = new System.Windows.Forms.TextBox();
			this.m_lblCtrlTrlText = new System.Windows.Forms.Label();
			this.m_lblCtrlEngText = new System.Windows.Forms.Label();
			this.m_tvControls = new System.Windows.Forms.TreeView();
			this.m_tabUnusedText = new System.Windows.Forms.TabPage();
			this.m_btnClearUnusedText = new System.Windows.Forms.Button();
			this.m_rtbUnusedText = new KeePass.UI.CustomRichTextBoxEx();
			this.m_menuMain = new KeePass.UI.CustomMenuStripEx();
			this.m_menuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_menuFileImport = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileImportLng = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileImportPo = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileImportSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_menuFileImport2xNoChecks = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuEditNextUntrl = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsMain = new KeePass.UI.CustomToolStripEx();
			this.m_tbOpen = new System.Windows.Forms.ToolStripButton();
			this.m_tbSave = new System.Windows.Forms.ToolStripButton();
			this.m_tbSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbNextUntrl = new System.Windows.Forms.ToolStripButton();
			this.m_tbSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbFind = new System.Windows.Forms.ToolStripTextBox();
			this.m_tabMain.SuspendLayout();
			this.m_tabProps.SuspendLayout();
			this.m_tabStrings.SuspendLayout();
			this.m_tabDialogs.SuspendLayout();
			this.m_grpControl.SuspendLayout();
			this.m_grpControlLayout.SuspendLayout();
			this.m_tabUnusedText.SuspendLayout();
			this.m_menuMain.SuspendLayout();
			this.m_tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_tabMain
			// 
			this.m_tabMain.Controls.Add(this.m_tabProps);
			this.m_tabMain.Controls.Add(this.m_tabStrings);
			this.m_tabMain.Controls.Add(this.m_tabDialogs);
			this.m_tabMain.Controls.Add(this.m_tabUnusedText);
			this.m_tabMain.Location = new System.Drawing.Point(12, 53);
			this.m_tabMain.Name = "m_tabMain";
			this.m_tabMain.SelectedIndex = 0;
			this.m_tabMain.Size = new System.Drawing.Size(605, 461);
			this.m_tabMain.TabIndex = 0;
			this.m_tabMain.SelectedIndexChanged += new System.EventHandler(this.OnTabMainSelectedIndexChanged);
			// 
			// m_tabProps
			// 
			this.m_tabProps.Controls.Add(this.m_cbRtl);
			this.m_tabProps.Controls.Add(this.m_lblAuthorContact);
			this.m_tabProps.Controls.Add(this.m_tbAuthorContact);
			this.m_tabProps.Controls.Add(this.m_linkLangCode);
			this.m_tabProps.Controls.Add(this.m_lblLangIDSample);
			this.m_tabProps.Controls.Add(this.m_tbLangID);
			this.m_tabProps.Controls.Add(this.m_lblLangID);
			this.m_tabProps.Controls.Add(this.m_lblAuthorName);
			this.m_tabProps.Controls.Add(this.m_tbAuthorName);
			this.m_tabProps.Controls.Add(this.m_lblNameLclSample);
			this.m_tabProps.Controls.Add(this.m_tbNameLcl);
			this.m_tabProps.Controls.Add(this.m_lblNameLcl);
			this.m_tabProps.Controls.Add(this.m_lblNameEngSample);
			this.m_tabProps.Controls.Add(this.m_lblNameEng);
			this.m_tabProps.Controls.Add(this.m_tbNameEng);
			this.m_tabProps.Location = new System.Drawing.Point(4, 22);
			this.m_tabProps.Name = "m_tabProps";
			this.m_tabProps.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabProps.Size = new System.Drawing.Size(597, 435);
			this.m_tabProps.TabIndex = 0;
			this.m_tabProps.Text = "Properties";
			this.m_tabProps.UseVisualStyleBackColor = true;
			// 
			// m_cbRtl
			// 
			this.m_cbRtl.AutoSize = true;
			this.m_cbRtl.Location = new System.Drawing.Point(9, 238);
			this.m_cbRtl.Name = "m_cbRtl";
			this.m_cbRtl.Size = new System.Drawing.Size(149, 17);
			this.m_cbRtl.TabIndex = 14;
			this.m_cbRtl.Text = "Script is written right-to-left";
			this.m_cbRtl.UseVisualStyleBackColor = true;
			// 
			// m_lblAuthorContact
			// 
			this.m_lblAuthorContact.AutoSize = true;
			this.m_lblAuthorContact.Location = new System.Drawing.Point(6, 206);
			this.m_lblAuthorContact.Name = "m_lblAuthorContact";
			this.m_lblAuthorContact.Size = new System.Drawing.Size(80, 13);
			this.m_lblAuthorContact.TabIndex = 12;
			this.m_lblAuthorContact.Text = "Author contact:";
			// 
			// m_tbAuthorContact
			// 
			this.m_tbAuthorContact.Location = new System.Drawing.Point(147, 203);
			this.m_tbAuthorContact.Name = "m_tbAuthorContact";
			this.m_tbAuthorContact.Size = new System.Drawing.Size(435, 20);
			this.m_tbAuthorContact.TabIndex = 13;
			// 
			// m_linkLangCode
			// 
			this.m_linkLangCode.AutoSize = true;
			this.m_linkLangCode.Location = new System.Drawing.Point(144, 151);
			this.m_linkLangCode.Name = "m_linkLangCode";
			this.m_linkLangCode.Size = new System.Drawing.Size(156, 13);
			this.m_linkLangCode.TabIndex = 9;
			this.m_linkLangCode.TabStop = true;
			this.m_linkLangCode.Text = "See ISO 639-1 language codes";
			this.m_linkLangCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkLangCodeClicked);
			// 
			// m_lblLangIDSample
			// 
			this.m_lblLangIDSample.AutoSize = true;
			this.m_lblLangIDSample.Location = new System.Drawing.Point(144, 135);
			this.m_lblLangIDSample.Name = "m_lblLangIDSample";
			this.m_lblLangIDSample.Size = new System.Drawing.Size(65, 13);
			this.m_lblLangIDSample.TabIndex = 8;
			this.m_lblLangIDSample.Text = "Example: de";
			// 
			// m_tbLangID
			// 
			this.m_tbLangID.Location = new System.Drawing.Point(147, 112);
			this.m_tbLangID.Name = "m_tbLangID";
			this.m_tbLangID.Size = new System.Drawing.Size(435, 20);
			this.m_tbLangID.TabIndex = 7;
			// 
			// m_lblLangID
			// 
			this.m_lblLangID.AutoSize = true;
			this.m_lblLangID.Location = new System.Drawing.Point(6, 115);
			this.m_lblLangID.Name = "m_lblLangID";
			this.m_lblLangID.Size = new System.Drawing.Size(132, 13);
			this.m_lblLangID.TabIndex = 6;
			this.m_lblLangID.Text = "ISO 639-1 language code:";
			// 
			// m_lblAuthorName
			// 
			this.m_lblAuthorName.AutoSize = true;
			this.m_lblAuthorName.Location = new System.Drawing.Point(6, 180);
			this.m_lblAuthorName.Name = "m_lblAuthorName";
			this.m_lblAuthorName.Size = new System.Drawing.Size(70, 13);
			this.m_lblAuthorName.TabIndex = 10;
			this.m_lblAuthorName.Text = "Author name:";
			// 
			// m_tbAuthorName
			// 
			this.m_tbAuthorName.Location = new System.Drawing.Point(147, 177);
			this.m_tbAuthorName.Name = "m_tbAuthorName";
			this.m_tbAuthorName.Size = new System.Drawing.Size(435, 20);
			this.m_tbAuthorName.TabIndex = 11;
			// 
			// m_lblNameLclSample
			// 
			this.m_lblNameLclSample.AutoSize = true;
			this.m_lblNameLclSample.Location = new System.Drawing.Point(144, 86);
			this.m_lblNameLclSample.Name = "m_lblNameLclSample";
			this.m_lblNameLclSample.Size = new System.Drawing.Size(93, 13);
			this.m_lblNameLclSample.TabIndex = 5;
			this.m_lblNameLclSample.Text = "Example: Deutsch";
			// 
			// m_tbNameLcl
			// 
			this.m_tbNameLcl.Location = new System.Drawing.Point(147, 63);
			this.m_tbNameLcl.Name = "m_tbNameLcl";
			this.m_tbNameLcl.Size = new System.Drawing.Size(435, 20);
			this.m_tbNameLcl.TabIndex = 4;
			// 
			// m_lblNameLcl
			// 
			this.m_lblNameLcl.AutoSize = true;
			this.m_lblNameLcl.Location = new System.Drawing.Point(6, 66);
			this.m_lblNameLcl.Name = "m_lblNameLcl";
			this.m_lblNameLcl.Size = new System.Drawing.Size(117, 13);
			this.m_lblNameLcl.TabIndex = 3;
			this.m_lblNameLcl.Text = "Native language name:";
			// 
			// m_lblNameEngSample
			// 
			this.m_lblNameEngSample.AutoSize = true;
			this.m_lblNameEngSample.Location = new System.Drawing.Point(144, 38);
			this.m_lblNameEngSample.Name = "m_lblNameEngSample";
			this.m_lblNameEngSample.Size = new System.Drawing.Size(90, 13);
			this.m_lblNameEngSample.TabIndex = 2;
			this.m_lblNameEngSample.Text = "Example: German";
			// 
			// m_lblNameEng
			// 
			this.m_lblNameEng.AutoSize = true;
			this.m_lblNameEng.Location = new System.Drawing.Point(6, 18);
			this.m_lblNameEng.Name = "m_lblNameEng";
			this.m_lblNameEng.Size = new System.Drawing.Size(120, 13);
			this.m_lblNameEng.TabIndex = 0;
			this.m_lblNameEng.Text = "English language name:";
			// 
			// m_tbNameEng
			// 
			this.m_tbNameEng.Location = new System.Drawing.Point(147, 15);
			this.m_tbNameEng.Name = "m_tbNameEng";
			this.m_tbNameEng.Size = new System.Drawing.Size(435, 20);
			this.m_tbNameEng.TabIndex = 1;
			// 
			// m_tabStrings
			// 
			this.m_tabStrings.Controls.Add(this.m_lblStrSaveHint);
			this.m_tabStrings.Controls.Add(this.m_tbStrTrl);
			this.m_tabStrings.Controls.Add(this.m_tbStrEng);
			this.m_tabStrings.Controls.Add(this.m_lblStrTrl);
			this.m_tabStrings.Controls.Add(this.m_lblStrEng);
			this.m_tabStrings.Controls.Add(this.m_lvStrings);
			this.m_tabStrings.Location = new System.Drawing.Point(4, 22);
			this.m_tabStrings.Name = "m_tabStrings";
			this.m_tabStrings.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabStrings.Size = new System.Drawing.Size(597, 435);
			this.m_tabStrings.TabIndex = 1;
			this.m_tabStrings.Text = "String Tables";
			this.m_tabStrings.UseVisualStyleBackColor = true;
			// 
			// m_lblStrSaveHint
			// 
			this.m_lblStrSaveHint.AutoSize = true;
			this.m_lblStrSaveHint.Location = new System.Drawing.Point(66, 410);
			this.m_lblStrSaveHint.Name = "m_lblStrSaveHint";
			this.m_lblStrSaveHint.Size = new System.Drawing.Size(248, 13);
			this.m_lblStrSaveHint.TabIndex = 5;
			this.m_lblStrSaveHint.Text = "Press [Return] to save/accept the translated string.";
			// 
			// m_tbStrTrl
			// 
			this.m_tbStrTrl.Location = new System.Drawing.Point(69, 387);
			this.m_tbStrTrl.Name = "m_tbStrTrl";
			this.m_tbStrTrl.Size = new System.Drawing.Size(522, 20);
			this.m_tbStrTrl.TabIndex = 4;
			this.m_tbStrTrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnStrKeyDown);
			this.m_tbStrTrl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnStrKeyUp);
			// 
			// m_tbStrEng
			// 
			this.m_tbStrEng.Location = new System.Drawing.Point(69, 328);
			this.m_tbStrEng.Multiline = true;
			this.m_tbStrEng.Name = "m_tbStrEng";
			this.m_tbStrEng.ReadOnly = true;
			this.m_tbStrEng.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbStrEng.Size = new System.Drawing.Size(522, 53);
			this.m_tbStrEng.TabIndex = 2;
			// 
			// m_lblStrTrl
			// 
			this.m_lblStrTrl.AutoSize = true;
			this.m_lblStrTrl.Location = new System.Drawing.Point(3, 390);
			this.m_lblStrTrl.Name = "m_lblStrTrl";
			this.m_lblStrTrl.Size = new System.Drawing.Size(60, 13);
			this.m_lblStrTrl.TabIndex = 3;
			this.m_lblStrTrl.Text = "Translated:";
			// 
			// m_lblStrEng
			// 
			this.m_lblStrEng.AutoSize = true;
			this.m_lblStrEng.Location = new System.Drawing.Point(3, 331);
			this.m_lblStrEng.Name = "m_lblStrEng";
			this.m_lblStrEng.Size = new System.Drawing.Size(44, 13);
			this.m_lblStrEng.TabIndex = 1;
			this.m_lblStrEng.Text = "English:";
			// 
			// m_lvStrings
			// 
			this.m_lvStrings.FullRowSelect = true;
			this.m_lvStrings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvStrings.HideSelection = false;
			this.m_lvStrings.Location = new System.Drawing.Point(6, 6);
			this.m_lvStrings.MultiSelect = false;
			this.m_lvStrings.Name = "m_lvStrings";
			this.m_lvStrings.ShowItemToolTips = true;
			this.m_lvStrings.Size = new System.Drawing.Size(585, 316);
			this.m_lvStrings.TabIndex = 0;
			this.m_lvStrings.UseCompatibleStateImageBehavior = false;
			this.m_lvStrings.View = System.Windows.Forms.View.Details;
			this.m_lvStrings.SelectedIndexChanged += new System.EventHandler(this.OnStringsSelectedIndexChanged);
			this.m_lvStrings.DoubleClick += new System.EventHandler(this.OnStrDoubleClick);
			// 
			// m_tabDialogs
			// 
			this.m_tabDialogs.Controls.Add(this.m_lblIconColorHint);
			this.m_tabDialogs.Controls.Add(this.m_grpControl);
			this.m_tabDialogs.Controls.Add(this.m_tvControls);
			this.m_tabDialogs.Location = new System.Drawing.Point(4, 22);
			this.m_tabDialogs.Name = "m_tabDialogs";
			this.m_tabDialogs.Size = new System.Drawing.Size(597, 435);
			this.m_tabDialogs.TabIndex = 2;
			this.m_tabDialogs.Text = "Dialogs";
			this.m_tabDialogs.UseVisualStyleBackColor = true;
			// 
			// m_lblIconColorHint
			// 
			this.m_lblIconColorHint.AutoSize = true;
			this.m_lblIconColorHint.Location = new System.Drawing.Point(221, 407);
			this.m_lblIconColorHint.Name = "m_lblIconColorHint";
			this.m_lblIconColorHint.Size = new System.Drawing.Size(307, 13);
			this.m_lblIconColorHint.TabIndex = 5;
			this.m_lblIconColorHint.Text = "In a correct translation, all icons in the tree on the left are green.";
			// 
			// m_grpControl
			// 
			this.m_grpControl.Controls.Add(this.m_grpControlLayout);
			this.m_grpControl.Controls.Add(this.m_tbCtrlTrlText);
			this.m_grpControl.Controls.Add(this.m_tbCtrlEngText);
			this.m_grpControl.Controls.Add(this.m_lblCtrlTrlText);
			this.m_grpControl.Controls.Add(this.m_lblCtrlEngText);
			this.m_grpControl.Location = new System.Drawing.Point(224, 6);
			this.m_grpControl.Name = "m_grpControl";
			this.m_grpControl.Size = new System.Drawing.Size(367, 357);
			this.m_grpControl.TabIndex = 1;
			this.m_grpControl.TabStop = false;
			this.m_grpControl.Text = "Selected Control";
			// 
			// m_grpControlLayout
			// 
			this.m_grpControlLayout.Controls.Add(this.m_lblLayoutHint2);
			this.m_grpControlLayout.Controls.Add(this.m_lblLayoutHint);
			this.m_grpControlLayout.Controls.Add(this.m_tbLayoutH);
			this.m_grpControlLayout.Controls.Add(this.m_tbLayoutW);
			this.m_grpControlLayout.Controls.Add(this.m_tbLayoutY);
			this.m_grpControlLayout.Controls.Add(this.m_tbLayoutX);
			this.m_grpControlLayout.Controls.Add(this.m_lblLayoutH);
			this.m_grpControlLayout.Controls.Add(this.m_lblLayoutW);
			this.m_grpControlLayout.Controls.Add(this.m_lblLayoutY);
			this.m_grpControlLayout.Controls.Add(this.m_lblLayoutX);
			this.m_grpControlLayout.Location = new System.Drawing.Point(9, 174);
			this.m_grpControlLayout.Name = "m_grpControlLayout";
			this.m_grpControlLayout.Size = new System.Drawing.Size(352, 174);
			this.m_grpControlLayout.TabIndex = 4;
			this.m_grpControlLayout.TabStop = false;
			this.m_grpControlLayout.Text = "Layout";
			// 
			// m_lblLayoutHint2
			// 
			this.m_lblLayoutHint2.Location = new System.Drawing.Point(6, 127);
			this.m_lblLayoutHint2.Name = "m_lblLayoutHint2";
			this.m_lblLayoutHint2.Size = new System.Drawing.Size(340, 40);
			this.m_lblLayoutHint2.TabIndex = 9;
			this.m_lblLayoutHint2.Text = "The values must be entered according to the \"en-US\" culture, i.e. the decimal poi" +
				"nt is represented by a dot (.), the negative sign is leading the number.";
			// 
			// m_lblLayoutHint
			// 
			this.m_lblLayoutHint.Location = new System.Drawing.Point(6, 77);
			this.m_lblLayoutHint.Name = "m_lblLayoutHint";
			this.m_lblLayoutHint.Size = new System.Drawing.Size(340, 41);
			this.m_lblLayoutHint.TabIndex = 8;
			this.m_lblLayoutHint.Text = "All values need be entered in % relative to the current position/size. For exampl" +
				"e, entering -50 in the width field will shrink the control\'s width to half of it" +
				"s previous value.";
			// 
			// m_tbLayoutH
			// 
			this.m_tbLayoutH.Location = new System.Drawing.Point(246, 45);
			this.m_tbLayoutH.Name = "m_tbLayoutH";
			this.m_tbLayoutH.Size = new System.Drawing.Size(100, 20);
			this.m_tbLayoutH.TabIndex = 7;
			this.m_tbLayoutH.TextChanged += new System.EventHandler(this.OnLayoutHeightTextChanged);
			// 
			// m_tbLayoutW
			// 
			this.m_tbLayoutW.Location = new System.Drawing.Point(53, 45);
			this.m_tbLayoutW.Name = "m_tbLayoutW";
			this.m_tbLayoutW.Size = new System.Drawing.Size(100, 20);
			this.m_tbLayoutW.TabIndex = 5;
			this.m_tbLayoutW.TextChanged += new System.EventHandler(this.OnLayoutWidthTextChanged);
			// 
			// m_tbLayoutY
			// 
			this.m_tbLayoutY.Location = new System.Drawing.Point(246, 19);
			this.m_tbLayoutY.Name = "m_tbLayoutY";
			this.m_tbLayoutY.Size = new System.Drawing.Size(100, 20);
			this.m_tbLayoutY.TabIndex = 3;
			this.m_tbLayoutY.TextChanged += new System.EventHandler(this.OnLayoutYTextChanged);
			// 
			// m_tbLayoutX
			// 
			this.m_tbLayoutX.Location = new System.Drawing.Point(53, 19);
			this.m_tbLayoutX.Name = "m_tbLayoutX";
			this.m_tbLayoutX.Size = new System.Drawing.Size(100, 20);
			this.m_tbLayoutX.TabIndex = 1;
			this.m_tbLayoutX.TextChanged += new System.EventHandler(this.OnLayoutXTextChanged);
			// 
			// m_lblLayoutH
			// 
			this.m_lblLayoutH.AutoSize = true;
			this.m_lblLayoutH.Location = new System.Drawing.Point(199, 48);
			this.m_lblLayoutH.Name = "m_lblLayoutH";
			this.m_lblLayoutH.Size = new System.Drawing.Size(41, 13);
			this.m_lblLayoutH.TabIndex = 6;
			this.m_lblLayoutH.Text = "Height:";
			// 
			// m_lblLayoutW
			// 
			this.m_lblLayoutW.AutoSize = true;
			this.m_lblLayoutW.Location = new System.Drawing.Point(6, 48);
			this.m_lblLayoutW.Name = "m_lblLayoutW";
			this.m_lblLayoutW.Size = new System.Drawing.Size(38, 13);
			this.m_lblLayoutW.TabIndex = 4;
			this.m_lblLayoutW.Text = "Width:";
			// 
			// m_lblLayoutY
			// 
			this.m_lblLayoutY.AutoSize = true;
			this.m_lblLayoutY.Location = new System.Drawing.Point(199, 22);
			this.m_lblLayoutY.Name = "m_lblLayoutY";
			this.m_lblLayoutY.Size = new System.Drawing.Size(17, 13);
			this.m_lblLayoutY.TabIndex = 2;
			this.m_lblLayoutY.Text = "Y:";
			// 
			// m_lblLayoutX
			// 
			this.m_lblLayoutX.AutoSize = true;
			this.m_lblLayoutX.Location = new System.Drawing.Point(6, 22);
			this.m_lblLayoutX.Name = "m_lblLayoutX";
			this.m_lblLayoutX.Size = new System.Drawing.Size(17, 13);
			this.m_lblLayoutX.TabIndex = 0;
			this.m_lblLayoutX.Text = "X:";
			// 
			// m_tbCtrlTrlText
			// 
			this.m_tbCtrlTrlText.Location = new System.Drawing.Point(9, 113);
			this.m_tbCtrlTrlText.Name = "m_tbCtrlTrlText";
			this.m_tbCtrlTrlText.Size = new System.Drawing.Size(352, 20);
			this.m_tbCtrlTrlText.TabIndex = 3;
			this.m_tbCtrlTrlText.TextChanged += new System.EventHandler(this.OnCtrlTrlTextChanged);
			this.m_tbCtrlTrlText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnCtrlTrlTextKeyDown);
			this.m_tbCtrlTrlText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnCtrlTrlTextKeyUp);
			// 
			// m_tbCtrlEngText
			// 
			this.m_tbCtrlEngText.Location = new System.Drawing.Point(9, 41);
			this.m_tbCtrlEngText.Multiline = true;
			this.m_tbCtrlEngText.Name = "m_tbCtrlEngText";
			this.m_tbCtrlEngText.ReadOnly = true;
			this.m_tbCtrlEngText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbCtrlEngText.Size = new System.Drawing.Size(352, 53);
			this.m_tbCtrlEngText.TabIndex = 1;
			// 
			// m_lblCtrlTrlText
			// 
			this.m_lblCtrlTrlText.AutoSize = true;
			this.m_lblCtrlTrlText.Location = new System.Drawing.Point(6, 97);
			this.m_lblCtrlTrlText.Name = "m_lblCtrlTrlText";
			this.m_lblCtrlTrlText.Size = new System.Drawing.Size(60, 13);
			this.m_lblCtrlTrlText.TabIndex = 2;
			this.m_lblCtrlTrlText.Text = "Translated:";
			// 
			// m_lblCtrlEngText
			// 
			this.m_lblCtrlEngText.AutoSize = true;
			this.m_lblCtrlEngText.Location = new System.Drawing.Point(6, 25);
			this.m_lblCtrlEngText.Name = "m_lblCtrlEngText";
			this.m_lblCtrlEngText.Size = new System.Drawing.Size(44, 13);
			this.m_lblCtrlEngText.TabIndex = 0;
			this.m_lblCtrlEngText.Text = "English:";
			// 
			// m_tvControls
			// 
			this.m_tvControls.HideSelection = false;
			this.m_tvControls.Location = new System.Drawing.Point(6, 6);
			this.m_tvControls.Name = "m_tvControls";
			this.m_tvControls.ShowNodeToolTips = true;
			this.m_tvControls.ShowRootLines = false;
			this.m_tvControls.Size = new System.Drawing.Size(200, 423);
			this.m_tvControls.TabIndex = 0;
			this.m_tvControls.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnCustomControlsAfterSelect);
			// 
			// m_tabUnusedText
			// 
			this.m_tabUnusedText.Controls.Add(this.m_btnClearUnusedText);
			this.m_tabUnusedText.Controls.Add(this.m_rtbUnusedText);
			this.m_tabUnusedText.Location = new System.Drawing.Point(4, 22);
			this.m_tabUnusedText.Name = "m_tabUnusedText";
			this.m_tabUnusedText.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabUnusedText.Size = new System.Drawing.Size(597, 435);
			this.m_tabUnusedText.TabIndex = 3;
			this.m_tabUnusedText.Text = "Unused Text";
			this.m_tabUnusedText.UseVisualStyleBackColor = true;
			// 
			// m_btnClearUnusedText
			// 
			this.m_btnClearUnusedText.Location = new System.Drawing.Point(516, 406);
			this.m_btnClearUnusedText.Name = "m_btnClearUnusedText";
			this.m_btnClearUnusedText.Size = new System.Drawing.Size(75, 23);
			this.m_btnClearUnusedText.TabIndex = 1;
			this.m_btnClearUnusedText.Text = "&Clear";
			this.m_btnClearUnusedText.UseVisualStyleBackColor = true;
			this.m_btnClearUnusedText.Click += new System.EventHandler(this.OnBtnClearUnusedText);
			// 
			// m_rtbUnusedText
			// 
			this.m_rtbUnusedText.Location = new System.Drawing.Point(6, 6);
			this.m_rtbUnusedText.Name = "m_rtbUnusedText";
			this.m_rtbUnusedText.Size = new System.Drawing.Size(585, 394);
			this.m_rtbUnusedText.TabIndex = 0;
			this.m_rtbUnusedText.Text = "";
			// 
			// m_menuMain
			// 
			this.m_menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFile,
            this.m_menuEdit});
			this.m_menuMain.Location = new System.Drawing.Point(0, 0);
			this.m_menuMain.Name = "m_menuMain";
			this.m_menuMain.Size = new System.Drawing.Size(629, 24);
			this.m_menuMain.TabIndex = 1;
			this.m_menuMain.TabStop = true;
			// 
			// m_menuFile
			// 
			this.m_menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFileOpen,
            this.m_menuFileSave,
            this.m_menuFileSaveAs,
            this.m_menuFileSep0,
            this.m_menuFileImport,
            this.m_menuFileSep1,
            this.m_menuFileExit});
			this.m_menuFile.Name = "m_menuFile";
			this.m_menuFile.Size = new System.Drawing.Size(39, 20);
			this.m_menuFile.Text = "&File";
			// 
			// m_menuFileOpen
			// 
			this.m_menuFileOpen.Image = global::TrlUtil.Properties.Resources.B16x16_Folder_Yellow_Open;
			this.m_menuFileOpen.Name = "m_menuFileOpen";
			this.m_menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.m_menuFileOpen.Size = new System.Drawing.Size(154, 22);
			this.m_menuFileOpen.Text = "&Open...";
			this.m_menuFileOpen.Click += new System.EventHandler(this.OnFileOpen);
			// 
			// m_menuFileSave
			// 
			this.m_menuFileSave.Image = global::TrlUtil.Properties.Resources.B16x16_FileSave;
			this.m_menuFileSave.Name = "m_menuFileSave";
			this.m_menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.m_menuFileSave.Size = new System.Drawing.Size(154, 22);
			this.m_menuFileSave.Text = "&Save";
			this.m_menuFileSave.Click += new System.EventHandler(this.OnFileSave);
			// 
			// m_menuFileSaveAs
			// 
			this.m_menuFileSaveAs.Image = global::TrlUtil.Properties.Resources.B16x16_FileSaveAs;
			this.m_menuFileSaveAs.Name = "m_menuFileSaveAs";
			this.m_menuFileSaveAs.Size = new System.Drawing.Size(154, 22);
			this.m_menuFileSaveAs.Text = "Save &As...";
			this.m_menuFileSaveAs.Click += new System.EventHandler(this.OnFileSaveAs);
			// 
			// m_menuFileSep0
			// 
			this.m_menuFileSep0.Name = "m_menuFileSep0";
			this.m_menuFileSep0.Size = new System.Drawing.Size(151, 6);
			// 
			// m_menuFileImport
			// 
			this.m_menuFileImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFileImportLng,
            this.m_menuFileImportPo,
            this.m_menuFileImportSep0,
            this.m_menuFileImport2xNoChecks});
			this.m_menuFileImport.Name = "m_menuFileImport";
			this.m_menuFileImport.Size = new System.Drawing.Size(154, 22);
			this.m_menuFileImport.Text = "&Import";
			// 
			// m_menuFileImportLng
			// 
			this.m_menuFileImportLng.Name = "m_menuFileImportLng";
			this.m_menuFileImportLng.Size = new System.Drawing.Size(311, 22);
			this.m_menuFileImportLng.Text = "KeePass &1.x LNG File...";
			this.m_menuFileImportLng.Click += new System.EventHandler(this.OnImport1xLng);
			// 
			// m_menuFileImportPo
			// 
			this.m_menuFileImportPo.Name = "m_menuFileImportPo";
			this.m_menuFileImportPo.Size = new System.Drawing.Size(311, 22);
			this.m_menuFileImportPo.Text = "&PO File...";
			this.m_menuFileImportPo.Click += new System.EventHandler(this.OnImportPo);
			// 
			// m_menuFileImportSep0
			// 
			this.m_menuFileImportSep0.Name = "m_menuFileImportSep0";
			this.m_menuFileImportSep0.Size = new System.Drawing.Size(308, 6);
			// 
			// m_menuFileImport2xNoChecks
			// 
			this.m_menuFileImport2xNoChecks.Name = "m_menuFileImport2xNoChecks";
			this.m_menuFileImport2xNoChecks.Size = new System.Drawing.Size(311, 22);
			this.m_menuFileImport2xNoChecks.Text = "KeePass &2.x LNGX File (No Base Checks)...";
			this.m_menuFileImport2xNoChecks.Click += new System.EventHandler(this.OnImport2xNoChecks);
			// 
			// m_menuFileSep1
			// 
			this.m_menuFileSep1.Name = "m_menuFileSep1";
			this.m_menuFileSep1.Size = new System.Drawing.Size(151, 6);
			// 
			// m_menuFileExit
			// 
			this.m_menuFileExit.Name = "m_menuFileExit";
			this.m_menuFileExit.Size = new System.Drawing.Size(154, 22);
			this.m_menuFileExit.Text = "&Exit";
			this.m_menuFileExit.Click += new System.EventHandler(this.OnFileExit);
			// 
			// m_menuEdit
			// 
			this.m_menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuEditNextUntrl});
			this.m_menuEdit.Name = "m_menuEdit";
			this.m_menuEdit.Size = new System.Drawing.Size(40, 20);
			this.m_menuEdit.Text = "&Edit";
			// 
			// m_menuEditNextUntrl
			// 
			this.m_menuEditNextUntrl.Image = global::TrlUtil.Properties.Resources.B16x16_Down;
			this.m_menuEditNextUntrl.Name = "m_menuEditNextUntrl";
			this.m_menuEditNextUntrl.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
			this.m_menuEditNextUntrl.Size = new System.Drawing.Size(245, 22);
			this.m_menuEditNextUntrl.Text = "Go to Next &Untranslated";
			this.m_menuEditNextUntrl.Click += new System.EventHandler(this.OnEditNextUntrl);
			// 
			// m_tsMain
			// 
			this.m_tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tbOpen,
            this.m_tbSave,
            this.m_tbSep0,
            this.m_tbNextUntrl,
            this.m_tbSep1,
            this.m_tbFind});
			this.m_tsMain.Location = new System.Drawing.Point(0, 24);
			this.m_tsMain.Name = "m_tsMain";
			this.m_tsMain.Size = new System.Drawing.Size(629, 25);
			this.m_tsMain.TabIndex = 2;
			this.m_tsMain.TabStop = true;
			// 
			// m_tbOpen
			// 
			this.m_tbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbOpen.Image = global::TrlUtil.Properties.Resources.B16x16_Folder_Yellow_Open;
			this.m_tbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbOpen.Name = "m_tbOpen";
			this.m_tbOpen.Size = new System.Drawing.Size(23, 22);
			this.m_tbOpen.Text = "Open... (Ctrl+O)";
			this.m_tbOpen.Click += new System.EventHandler(this.OnFileOpen);
			// 
			// m_tbSave
			// 
			this.m_tbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbSave.Image = global::TrlUtil.Properties.Resources.B16x16_FileSave;
			this.m_tbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbSave.Name = "m_tbSave";
			this.m_tbSave.Size = new System.Drawing.Size(23, 22);
			this.m_tbSave.Text = "Save (Ctrl+S)";
			this.m_tbSave.Click += new System.EventHandler(this.OnFileSave);
			// 
			// m_tbSep0
			// 
			this.m_tbSep0.Name = "m_tbSep0";
			this.m_tbSep0.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbNextUntrl
			// 
			this.m_tbNextUntrl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbNextUntrl.Image = global::TrlUtil.Properties.Resources.B16x16_Down;
			this.m_tbNextUntrl.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbNextUntrl.Name = "m_tbNextUntrl";
			this.m_tbNextUntrl.Size = new System.Drawing.Size(23, 22);
			this.m_tbNextUntrl.Text = "Go to Next Untranslated (Ctrl+U)";
			this.m_tbNextUntrl.Click += new System.EventHandler(this.OnEditNextUntrl);
			// 
			// m_tbSep1
			// 
			this.m_tbSep1.Name = "m_tbSep1";
			this.m_tbSep1.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbFind
			// 
			this.m_tbFind.AcceptsReturn = true;
			this.m_tbFind.Name = "m_tbFind";
			this.m_tbFind.Size = new System.Drawing.Size(180, 25);
			this.m_tbFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFindKeyDown);
			this.m_tbFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnFindKeyUp);
			this.m_tbFind.TextChanged += new System.EventHandler(this.OnFindTextChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(629, 526);
			this.Controls.Add(this.m_tsMain);
			this.Controls.Add(this.m_tabMain);
			this.Controls.Add(this.m_menuMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.m_menuMain;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "KeePass Translation Utility";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.m_tabMain.ResumeLayout(false);
			this.m_tabProps.ResumeLayout(false);
			this.m_tabProps.PerformLayout();
			this.m_tabStrings.ResumeLayout(false);
			this.m_tabStrings.PerformLayout();
			this.m_tabDialogs.ResumeLayout(false);
			this.m_tabDialogs.PerformLayout();
			this.m_grpControl.ResumeLayout(false);
			this.m_grpControl.PerformLayout();
			this.m_grpControlLayout.ResumeLayout(false);
			this.m_grpControlLayout.PerformLayout();
			this.m_tabUnusedText.ResumeLayout(false);
			this.m_menuMain.ResumeLayout(false);
			this.m_menuMain.PerformLayout();
			this.m_tsMain.ResumeLayout(false);
			this.m_tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl m_tabMain;
		private System.Windows.Forms.TabPage m_tabProps;
		private System.Windows.Forms.TabPage m_tabStrings;
		private System.Windows.Forms.TabPage m_tabDialogs;
		private System.Windows.Forms.Label m_lblNameLclSample;
		private System.Windows.Forms.TextBox m_tbNameLcl;
		private System.Windows.Forms.Label m_lblNameLcl;
		private System.Windows.Forms.Label m_lblNameEngSample;
		private System.Windows.Forms.Label m_lblNameEng;
		private System.Windows.Forms.TextBox m_tbNameEng;
		private System.Windows.Forms.Label m_lblLangIDSample;
		private System.Windows.Forms.TextBox m_tbLangID;
		private System.Windows.Forms.Label m_lblLangID;
		private System.Windows.Forms.Label m_lblAuthorName;
		private System.Windows.Forms.TextBox m_tbAuthorName;
		private System.Windows.Forms.LinkLabel m_linkLangCode;
		private System.Windows.Forms.Label m_lblAuthorContact;
		private System.Windows.Forms.TextBox m_tbAuthorContact;
		private KeePass.UI.CustomMenuStripEx m_menuMain;
		private System.Windows.Forms.ToolStripMenuItem m_menuFile;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileOpen;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileSave;
		private System.Windows.Forms.ToolStripSeparator m_menuFileSep0;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileExit;
		private KeePass.UI.CustomListViewEx m_lvStrings;
		private System.Windows.Forms.TextBox m_tbStrTrl;
		private System.Windows.Forms.TextBox m_tbStrEng;
		private System.Windows.Forms.Label m_lblStrTrl;
		private System.Windows.Forms.Label m_lblStrEng;
		private System.Windows.Forms.Label m_lblStrSaveHint;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileSaveAs;
		private System.Windows.Forms.TreeView m_tvControls;
		private System.Windows.Forms.GroupBox m_grpControl;
		private System.Windows.Forms.Label m_lblCtrlEngText;
		private System.Windows.Forms.TextBox m_tbCtrlTrlText;
		private System.Windows.Forms.TextBox m_tbCtrlEngText;
		private System.Windows.Forms.Label m_lblCtrlTrlText;
		private System.Windows.Forms.GroupBox m_grpControlLayout;
		private System.Windows.Forms.TextBox m_tbLayoutH;
		private System.Windows.Forms.TextBox m_tbLayoutW;
		private System.Windows.Forms.TextBox m_tbLayoutY;
		private System.Windows.Forms.TextBox m_tbLayoutX;
		private System.Windows.Forms.Label m_lblLayoutH;
		private System.Windows.Forms.Label m_lblLayoutW;
		private System.Windows.Forms.Label m_lblLayoutY;
		private System.Windows.Forms.Label m_lblLayoutX;
		private System.Windows.Forms.Label m_lblLayoutHint2;
		private System.Windows.Forms.Label m_lblLayoutHint;
		private System.Windows.Forms.Label m_lblIconColorHint;
		private System.Windows.Forms.TabPage m_tabUnusedText;
		private System.Windows.Forms.Button m_btnClearUnusedText;
		private KeePass.UI.CustomRichTextBoxEx m_rtbUnusedText;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileImport;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileImportLng;
		private System.Windows.Forms.ToolStripSeparator m_menuFileSep1;
		private KeePass.UI.CustomToolStripEx m_tsMain;
		private System.Windows.Forms.ToolStripButton m_tbOpen;
		private System.Windows.Forms.ToolStripButton m_tbSave;
		private System.Windows.Forms.ToolStripSeparator m_tbSep0;
		private System.Windows.Forms.ToolStripTextBox m_tbFind;
		private System.Windows.Forms.ToolStripSeparator m_menuFileImportSep0;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileImport2xNoChecks;
		private System.Windows.Forms.CheckBox m_cbRtl;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileImportPo;
		private System.Windows.Forms.ToolStripSeparator m_tbSep1;
		private System.Windows.Forms.ToolStripButton m_tbNextUntrl;
		private System.Windows.Forms.ToolStripMenuItem m_menuEdit;
		private System.Windows.Forms.ToolStripMenuItem m_menuEditNextUntrl;
	}
}

