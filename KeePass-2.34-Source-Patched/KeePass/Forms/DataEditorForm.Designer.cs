namespace KeePass.Forms
{
	partial class DataEditorForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_menuMain = new KeePass.UI.CustomMenuStripEx();
			this.m_menuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuView = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuViewFont = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuViewSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_menuViewWordWrap = new System.Windows.Forms.ToolStripMenuItem();
			this.m_toolFile = new KeePass.UI.CustomToolStripEx();
			this.m_tbFileSave = new System.Windows.Forms.ToolStripButton();
			this.m_tbFileSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbEditCut = new System.Windows.Forms.ToolStripButton();
			this.m_tbEditCopy = new System.Windows.Forms.ToolStripButton();
			this.m_tbEditPaste = new System.Windows.Forms.ToolStripButton();
			this.m_tbFileSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbEditUndo = new System.Windows.Forms.ToolStripButton();
			this.m_tbEditRedo = new System.Windows.Forms.ToolStripButton();
			this.m_tbFileSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbFind = new System.Windows.Forms.ToolStripTextBox();
			this.m_toolFormat = new KeePass.UI.CustomToolStripEx();
			this.m_tbFontCombo = new System.Windows.Forms.ToolStripComboBox();
			this.m_tbFontSizeCombo = new System.Windows.Forms.ToolStripComboBox();
			this.m_tbFormatSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbFormatBold = new System.Windows.Forms.ToolStripButton();
			this.m_tbFormatItalic = new System.Windows.Forms.ToolStripButton();
			this.m_tbFormatUnderline = new System.Windows.Forms.ToolStripButton();
			this.m_tbFormatStrikeout = new System.Windows.Forms.ToolStripButton();
			this.m_tbFormatSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbColorForeground = new System.Windows.Forms.ToolStripButton();
			this.m_tbColorBackground = new System.Windows.Forms.ToolStripButton();
			this.m_tbFormatSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbAlignLeft = new System.Windows.Forms.ToolStripButton();
			this.m_tbAlignCenter = new System.Windows.Forms.ToolStripButton();
			this.m_tbAlignRight = new System.Windows.Forms.ToolStripButton();
			this.m_statusMain = new System.Windows.Forms.StatusStrip();
			this.m_tssStatusMain = new System.Windows.Forms.ToolStripStatusLabel();
			this.m_rtbText = new KeePass.UI.CustomRichTextBoxEx();
			this.m_menuMain.SuspendLayout();
			this.m_toolFile.SuspendLayout();
			this.m_toolFormat.SuspendLayout();
			this.m_statusMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_menuMain
			// 
			this.m_menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFile,
            this.m_menuView});
			this.m_menuMain.Location = new System.Drawing.Point(0, 0);
			this.m_menuMain.Name = "m_menuMain";
			this.m_menuMain.Size = new System.Drawing.Size(608, 24);
			this.m_menuMain.TabIndex = 1;
			// 
			// m_menuFile
			// 
			this.m_menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFileSave,
            this.m_menuFileSep0,
            this.m_menuFileExit});
			this.m_menuFile.Name = "m_menuFile";
			this.m_menuFile.Size = new System.Drawing.Size(39, 20);
			this.m_menuFile.Text = "&File";
			// 
			// m_menuFileSave
			// 
			this.m_menuFileSave.Image = global::KeePass.Properties.Resources.B16x16_FileSave;
			this.m_menuFileSave.Name = "m_menuFileSave";
			this.m_menuFileSave.Size = new System.Drawing.Size(105, 22);
			this.m_menuFileSave.Text = "&Save";
			this.m_menuFileSave.Click += new System.EventHandler(this.OnFileSave);
			// 
			// m_menuFileSep0
			// 
			this.m_menuFileSep0.Name = "m_menuFileSep0";
			this.m_menuFileSep0.Size = new System.Drawing.Size(102, 6);
			// 
			// m_menuFileExit
			// 
			this.m_menuFileExit.Image = global::KeePass.Properties.Resources.B16x16_Exit;
			this.m_menuFileExit.Name = "m_menuFileExit";
			this.m_menuFileExit.Size = new System.Drawing.Size(105, 22);
			this.m_menuFileExit.Text = "&Close";
			this.m_menuFileExit.Click += new System.EventHandler(this.OnFileExit);
			// 
			// m_menuView
			// 
			this.m_menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuViewFont,
            this.m_menuViewSep0,
            this.m_menuViewWordWrap});
			this.m_menuView.Name = "m_menuView";
			this.m_menuView.Size = new System.Drawing.Size(45, 20);
			this.m_menuView.Text = "&View";
			// 
			// m_menuViewFont
			// 
			this.m_menuViewFont.Name = "m_menuViewFont";
			this.m_menuViewFont.Size = new System.Drawing.Size(152, 22);
			this.m_menuViewFont.Text = "&Font...";
			this.m_menuViewFont.Click += new System.EventHandler(this.OnViewFont);
			// 
			// m_menuViewSep0
			// 
			this.m_menuViewSep0.Name = "m_menuViewSep0";
			this.m_menuViewSep0.Size = new System.Drawing.Size(149, 6);
			// 
			// m_menuViewWordWrap
			// 
			this.m_menuViewWordWrap.Name = "m_menuViewWordWrap";
			this.m_menuViewWordWrap.Size = new System.Drawing.Size(152, 22);
			this.m_menuViewWordWrap.Text = "Word &Wrap";
			this.m_menuViewWordWrap.Click += new System.EventHandler(this.OnViewWordWrap);
			// 
			// m_toolFile
			// 
			this.m_toolFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tbFileSave,
            this.m_tbFileSep0,
            this.m_tbEditCut,
            this.m_tbEditCopy,
            this.m_tbEditPaste,
            this.m_tbFileSep1,
            this.m_tbEditUndo,
            this.m_tbEditRedo,
            this.m_tbFileSep2,
            this.m_tbFind});
			this.m_toolFile.Location = new System.Drawing.Point(0, 24);
			this.m_toolFile.Name = "m_toolFile";
			this.m_toolFile.Size = new System.Drawing.Size(608, 25);
			this.m_toolFile.TabIndex = 2;
			// 
			// m_tbFileSave
			// 
			this.m_tbFileSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbFileSave.Image = global::KeePass.Properties.Resources.B16x16_FileSave;
			this.m_tbFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbFileSave.Name = "m_tbFileSave";
			this.m_tbFileSave.Size = new System.Drawing.Size(23, 22);
			this.m_tbFileSave.Click += new System.EventHandler(this.OnFileSave);
			// 
			// m_tbFileSep0
			// 
			this.m_tbFileSep0.Name = "m_tbFileSep0";
			this.m_tbFileSep0.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbEditCut
			// 
			this.m_tbEditCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbEditCut.Image = global::KeePass.Properties.Resources.B16x16_Cut;
			this.m_tbEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbEditCut.Name = "m_tbEditCut";
			this.m_tbEditCut.Size = new System.Drawing.Size(23, 22);
			this.m_tbEditCut.Click += new System.EventHandler(this.OnEditCut);
			// 
			// m_tbEditCopy
			// 
			this.m_tbEditCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbEditCopy.Image = global::KeePass.Properties.Resources.B16x16_EditCopy;
			this.m_tbEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbEditCopy.Name = "m_tbEditCopy";
			this.m_tbEditCopy.Size = new System.Drawing.Size(23, 22);
			this.m_tbEditCopy.Click += new System.EventHandler(this.OnEditCopy);
			// 
			// m_tbEditPaste
			// 
			this.m_tbEditPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbEditPaste.Image = global::KeePass.Properties.Resources.B16x16_EditPaste;
			this.m_tbEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbEditPaste.Name = "m_tbEditPaste";
			this.m_tbEditPaste.Size = new System.Drawing.Size(23, 22);
			this.m_tbEditPaste.Click += new System.EventHandler(this.OnEditPaste);
			// 
			// m_tbFileSep1
			// 
			this.m_tbFileSep1.Name = "m_tbFileSep1";
			this.m_tbFileSep1.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbEditUndo
			// 
			this.m_tbEditUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbEditUndo.Image = global::KeePass.Properties.Resources.B16x16_Undo;
			this.m_tbEditUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbEditUndo.Name = "m_tbEditUndo";
			this.m_tbEditUndo.Size = new System.Drawing.Size(23, 22);
			this.m_tbEditUndo.Click += new System.EventHandler(this.OnEditUndo);
			// 
			// m_tbEditRedo
			// 
			this.m_tbEditRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbEditRedo.Image = global::KeePass.Properties.Resources.B16x16_Redo;
			this.m_tbEditRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbEditRedo.Name = "m_tbEditRedo";
			this.m_tbEditRedo.Size = new System.Drawing.Size(23, 22);
			this.m_tbEditRedo.Click += new System.EventHandler(this.OnEditRedo);
			// 
			// m_tbFileSep2
			// 
			this.m_tbFileSep2.Name = "m_tbFileSep2";
			this.m_tbFileSep2.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbFind
			// 
			this.m_tbFind.AcceptsReturn = true;
			this.m_tbFind.Name = "m_tbFind";
			this.m_tbFind.Size = new System.Drawing.Size(121, 25);
			this.m_tbFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnTextFindKeyDown);
			this.m_tbFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnTextFindKeyUp);
			// 
			// m_toolFormat
			// 
			this.m_toolFormat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tbFontCombo,
            this.m_tbFontSizeCombo,
            this.m_tbFormatSep0,
            this.m_tbFormatBold,
            this.m_tbFormatItalic,
            this.m_tbFormatUnderline,
            this.m_tbFormatStrikeout,
            this.m_tbFormatSep1,
            this.m_tbColorForeground,
            this.m_tbColorBackground,
            this.m_tbFormatSep2,
            this.m_tbAlignLeft,
            this.m_tbAlignCenter,
            this.m_tbAlignRight});
			this.m_toolFormat.Location = new System.Drawing.Point(0, 49);
			this.m_toolFormat.Name = "m_toolFormat";
			this.m_toolFormat.Size = new System.Drawing.Size(608, 25);
			this.m_toolFormat.TabIndex = 3;
			// 
			// m_tbFontCombo
			// 
			this.m_tbFontCombo.Name = "m_tbFontCombo";
			this.m_tbFontCombo.Size = new System.Drawing.Size(160, 25);
			this.m_tbFontCombo.SelectedIndexChanged += new System.EventHandler(this.OnFontComboSelectedIndexChanged);
			this.m_tbFontCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFontComboKeyDown);
			// 
			// m_tbFontSizeCombo
			// 
			this.m_tbFontSizeCombo.Name = "m_tbFontSizeCombo";
			this.m_tbFontSizeCombo.Size = new System.Drawing.Size(75, 25);
			this.m_tbFontSizeCombo.SelectedIndexChanged += new System.EventHandler(this.OnFontSizeComboSelectedIndexChanged);
			this.m_tbFontSizeCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFontSizeComboKeyDown);
			// 
			// m_tbFormatSep0
			// 
			this.m_tbFormatSep0.Name = "m_tbFormatSep0";
			this.m_tbFormatSep0.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbFormatBold
			// 
			this.m_tbFormatBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbFormatBold.Image = global::KeePass.Properties.Resources.B16x16_FontBold;
			this.m_tbFormatBold.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbFormatBold.Name = "m_tbFormatBold";
			this.m_tbFormatBold.Size = new System.Drawing.Size(23, 22);
			this.m_tbFormatBold.Click += new System.EventHandler(this.OnFormatBoldClicked);
			// 
			// m_tbFormatItalic
			// 
			this.m_tbFormatItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbFormatItalic.Image = global::KeePass.Properties.Resources.B16x16_FontItalic;
			this.m_tbFormatItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbFormatItalic.Name = "m_tbFormatItalic";
			this.m_tbFormatItalic.Size = new System.Drawing.Size(23, 22);
			this.m_tbFormatItalic.Click += new System.EventHandler(this.OnFormatItalicClicked);
			// 
			// m_tbFormatUnderline
			// 
			this.m_tbFormatUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbFormatUnderline.Image = global::KeePass.Properties.Resources.B16x16_FontUnderline;
			this.m_tbFormatUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbFormatUnderline.Name = "m_tbFormatUnderline";
			this.m_tbFormatUnderline.Size = new System.Drawing.Size(23, 22);
			this.m_tbFormatUnderline.Click += new System.EventHandler(this.OnFormatUnderlineClicked);
			// 
			// m_tbFormatStrikeout
			// 
			this.m_tbFormatStrikeout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbFormatStrikeout.Image = global::KeePass.Properties.Resources.B16x16_FontStrikeout;
			this.m_tbFormatStrikeout.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbFormatStrikeout.Name = "m_tbFormatStrikeout";
			this.m_tbFormatStrikeout.Size = new System.Drawing.Size(23, 22);
			this.m_tbFormatStrikeout.Click += new System.EventHandler(this.OnFormatStrikeoutClicked);
			// 
			// m_tbFormatSep1
			// 
			this.m_tbFormatSep1.Name = "m_tbFormatSep1";
			this.m_tbFormatSep1.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbColorForeground
			// 
			this.m_tbColorForeground.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbColorForeground.Image = global::KeePass.Properties.Resources.B16x16_Colorize;
			this.m_tbColorForeground.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbColorForeground.Name = "m_tbColorForeground";
			this.m_tbColorForeground.Size = new System.Drawing.Size(23, 22);
			this.m_tbColorForeground.Click += new System.EventHandler(this.OnColorForegroundClicked);
			// 
			// m_tbColorBackground
			// 
			this.m_tbColorBackground.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbColorBackground.Image = global::KeePass.Properties.Resources.B16x16_Color_Fill;
			this.m_tbColorBackground.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbColorBackground.Name = "m_tbColorBackground";
			this.m_tbColorBackground.Size = new System.Drawing.Size(23, 22);
			this.m_tbColorBackground.Click += new System.EventHandler(this.OnColorBackgroundClicked);
			// 
			// m_tbFormatSep2
			// 
			this.m_tbFormatSep2.Name = "m_tbFormatSep2";
			this.m_tbFormatSep2.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbAlignLeft
			// 
			this.m_tbAlignLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbAlignLeft.Image = global::KeePass.Properties.Resources.B16x16_TextAlignLeft;
			this.m_tbAlignLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbAlignLeft.Name = "m_tbAlignLeft";
			this.m_tbAlignLeft.Size = new System.Drawing.Size(23, 22);
			this.m_tbAlignLeft.Click += new System.EventHandler(this.OnAlignLeftClicked);
			// 
			// m_tbAlignCenter
			// 
			this.m_tbAlignCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbAlignCenter.Image = global::KeePass.Properties.Resources.B16x16_TextAlignCenter;
			this.m_tbAlignCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbAlignCenter.Name = "m_tbAlignCenter";
			this.m_tbAlignCenter.Size = new System.Drawing.Size(23, 22);
			this.m_tbAlignCenter.Click += new System.EventHandler(this.OnAlignCenterClicked);
			// 
			// m_tbAlignRight
			// 
			this.m_tbAlignRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_tbAlignRight.Image = global::KeePass.Properties.Resources.B16x16_TextAlignRight;
			this.m_tbAlignRight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tbAlignRight.Name = "m_tbAlignRight";
			this.m_tbAlignRight.Size = new System.Drawing.Size(23, 22);
			this.m_tbAlignRight.Click += new System.EventHandler(this.OnAlignRightClicked);
			// 
			// m_statusMain
			// 
			this.m_statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tssStatusMain});
			this.m_statusMain.Location = new System.Drawing.Point(0, 394);
			this.m_statusMain.Name = "m_statusMain";
			this.m_statusMain.Size = new System.Drawing.Size(608, 22);
			this.m_statusMain.TabIndex = 4;
			// 
			// m_tssStatusMain
			// 
			this.m_tssStatusMain.Name = "m_tssStatusMain";
			this.m_tssStatusMain.Size = new System.Drawing.Size(593, 17);
			this.m_tssStatusMain.Spring = true;
			this.m_tssStatusMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_rtbText
			// 
			this.m_rtbText.AcceptsTab = true;
			this.m_rtbText.HideSelection = false;
			this.m_rtbText.Location = new System.Drawing.Point(25, 102);
			this.m_rtbText.Name = "m_rtbText";
			this.m_rtbText.Size = new System.Drawing.Size(100, 96);
			this.m_rtbText.TabIndex = 0;
			this.m_rtbText.Text = "";
			this.m_rtbText.SelectionChanged += new System.EventHandler(this.OnTextSelectionChanged);
			this.m_rtbText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.OnTextLinkClicked);
			this.m_rtbText.TextChanged += new System.EventHandler(this.OnTextTextChanged);
			// 
			// DataEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(608, 416);
			this.Controls.Add(this.m_rtbText);
			this.Controls.Add(this.m_statusMain);
			this.Controls.Add(this.m_toolFormat);
			this.Controls.Add(this.m_toolFile);
			this.Controls.Add(this.m_menuMain);
			this.MainMenuStrip = this.m_menuMain;
			this.MinimizeBox = false;
			this.Name = "DataEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.m_menuMain.ResumeLayout(false);
			this.m_menuMain.PerformLayout();
			this.m_toolFile.ResumeLayout(false);
			this.m_toolFile.PerformLayout();
			this.m_toolFormat.ResumeLayout(false);
			this.m_toolFormat.PerformLayout();
			this.m_statusMain.ResumeLayout(false);
			this.m_statusMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private KeePass.UI.CustomRichTextBoxEx m_rtbText;
		private KeePass.UI.CustomMenuStripEx m_menuMain;
		private System.Windows.Forms.ToolStripMenuItem m_menuFile;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileSave;
		private System.Windows.Forms.ToolStripSeparator m_menuFileSep0;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileExit;
		private KeePass.UI.CustomToolStripEx m_toolFile;
		private System.Windows.Forms.ToolStripButton m_tbFileSave;
		private KeePass.UI.CustomToolStripEx m_toolFormat;
		private System.Windows.Forms.ToolStripComboBox m_tbFontCombo;
		private System.Windows.Forms.StatusStrip m_statusMain;
		private System.Windows.Forms.ToolStripComboBox m_tbFontSizeCombo;
		private System.Windows.Forms.ToolStripSeparator m_tbFormatSep0;
		private System.Windows.Forms.ToolStripButton m_tbFormatBold;
		private System.Windows.Forms.ToolStripButton m_tbFormatItalic;
		private System.Windows.Forms.ToolStripButton m_tbFormatUnderline;
		private System.Windows.Forms.ToolStripButton m_tbFormatStrikeout;
		private System.Windows.Forms.ToolStripButton m_tbColorForeground;
		private System.Windows.Forms.ToolStripButton m_tbColorBackground;
		private System.Windows.Forms.ToolStripSeparator m_tbFormatSep1;
		private System.Windows.Forms.ToolStripSeparator m_tbFormatSep2;
		private System.Windows.Forms.ToolStripButton m_tbAlignLeft;
		private System.Windows.Forms.ToolStripButton m_tbAlignCenter;
		private System.Windows.Forms.ToolStripButton m_tbAlignRight;
		private System.Windows.Forms.ToolStripSeparator m_tbFileSep0;
		private System.Windows.Forms.ToolStripButton m_tbEditCut;
		private System.Windows.Forms.ToolStripButton m_tbEditCopy;
		private System.Windows.Forms.ToolStripButton m_tbEditPaste;
		private System.Windows.Forms.ToolStripSeparator m_tbFileSep1;
		private System.Windows.Forms.ToolStripButton m_tbEditUndo;
		private System.Windows.Forms.ToolStripButton m_tbEditRedo;
		private System.Windows.Forms.ToolStripMenuItem m_menuView;
		private System.Windows.Forms.ToolStripMenuItem m_menuViewFont;
		private System.Windows.Forms.ToolStripSeparator m_menuViewSep0;
		private System.Windows.Forms.ToolStripMenuItem m_menuViewWordWrap;
		private System.Windows.Forms.ToolStripStatusLabel m_tssStatusMain;
		private System.Windows.Forms.ToolStripSeparator m_tbFileSep2;
		private System.Windows.Forms.ToolStripTextBox m_tbFind;
	}
}