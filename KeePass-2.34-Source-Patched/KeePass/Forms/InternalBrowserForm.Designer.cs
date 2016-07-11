namespace KeePass.Forms
{
	partial class InternalBrowserForm
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
			this.m_menuMain = new KeePass.UI.CustomMenuStripEx();
			this.m_menuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.m_menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_statusMain = new System.Windows.Forms.StatusStrip();
			this.m_lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.m_webBrowser = new System.Windows.Forms.WebBrowser();
			this.m_toolNav = new KeePass.UI.CustomToolStripEx();
			this.m_btnBack = new System.Windows.Forms.ToolStripButton();
			this.m_btnForward = new System.Windows.Forms.ToolStripButton();
			this.m_btnReload = new System.Windows.Forms.ToolStripButton();
			this.m_btnStop = new System.Windows.Forms.ToolStripButton();
			this.m_tssSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tbUrl = new System.Windows.Forms.ToolStripTextBox();
			this.m_btnGo = new System.Windows.Forms.ToolStripButton();
			this.m_menuMain.SuspendLayout();
			this.m_statusMain.SuspendLayout();
			this.m_toolNav.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_menuMain
			// 
			this.m_menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFile});
			this.m_menuMain.Location = new System.Drawing.Point(0, 0);
			this.m_menuMain.Name = "m_menuMain";
			this.m_menuMain.Size = new System.Drawing.Size(779, 24);
			this.m_menuMain.TabIndex = 0;
			this.m_menuMain.TabStop = true;
			// 
			// m_menuFile
			// 
			this.m_menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFileExit});
			this.m_menuFile.Name = "m_menuFile";
			this.m_menuFile.Size = new System.Drawing.Size(35, 20);
			this.m_menuFile.Text = "&File";
			// 
			// m_menuFileExit
			// 
			this.m_menuFileExit.Name = "m_menuFileExit";
			this.m_menuFileExit.Size = new System.Drawing.Size(91, 22);
			this.m_menuFileExit.Text = "E&xit";
			// 
			// m_statusMain
			// 
			this.m_statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblStatus});
			this.m_statusMain.Location = new System.Drawing.Point(0, 506);
			this.m_statusMain.Name = "m_statusMain";
			this.m_statusMain.Size = new System.Drawing.Size(779, 22);
			this.m_statusMain.TabIndex = 1;
			// 
			// m_lblStatus
			// 
			this.m_lblStatus.Name = "m_lblStatus";
			this.m_lblStatus.Size = new System.Drawing.Size(764, 17);
			this.m_lblStatus.Spring = true;
			this.m_lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_webBrowser
			// 
			this.m_webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_webBrowser.Location = new System.Drawing.Point(0, 49);
			this.m_webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.m_webBrowser.Name = "m_webBrowser";
			this.m_webBrowser.ScriptErrorsSuppressed = true;
			this.m_webBrowser.Size = new System.Drawing.Size(779, 457);
			this.m_webBrowser.TabIndex = 2;
			this.m_webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.OnWbDocumentCompleted);
			this.m_webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.OnWbNavigated);
			// 
			// m_toolNav
			// 
			this.m_toolNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnBack,
            this.m_btnForward,
            this.m_btnReload,
            this.m_btnStop,
            this.m_tssSep0,
            this.m_tbUrl,
            this.m_btnGo});
			this.m_toolNav.Location = new System.Drawing.Point(0, 24);
			this.m_toolNav.Name = "m_toolNav";
			this.m_toolNav.Size = new System.Drawing.Size(779, 25);
			this.m_toolNav.TabIndex = 3;
			this.m_toolNav.TabStop = true;
			// 
			// m_btnBack
			// 
			this.m_btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_btnBack.Image = global::KeePass.Properties.Resources.B16x16_1UpArrow;
			this.m_btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnBack.Name = "m_btnBack";
			this.m_btnBack.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_btnBack.Size = new System.Drawing.Size(23, 22);
			this.m_btnBack.Text = "&Back";
			this.m_btnBack.Click += new System.EventHandler(this.OnBtnBack);
			// 
			// m_btnForward
			// 
			this.m_btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_btnForward.Image = global::KeePass.Properties.Resources.B16x16_1DownArrow;
			this.m_btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnForward.Name = "m_btnForward";
			this.m_btnForward.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_btnForward.Size = new System.Drawing.Size(23, 22);
			this.m_btnForward.Text = "&Forward";
			this.m_btnForward.Click += new System.EventHandler(this.OnBtnForward);
			// 
			// m_btnReload
			// 
			this.m_btnReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_btnReload.Image = global::KeePass.Properties.Resources.B16x16_Reload_Page;
			this.m_btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnReload.Name = "m_btnReload";
			this.m_btnReload.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_btnReload.Size = new System.Drawing.Size(23, 22);
			this.m_btnReload.Text = "&Reload";
			this.m_btnReload.Click += new System.EventHandler(this.OnBtnReload);
			// 
			// m_btnStop
			// 
			this.m_btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_btnStop.Image = global::KeePass.Properties.Resources.B16x16_Exit;
			this.m_btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnStop.Name = "m_btnStop";
			this.m_btnStop.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_btnStop.Size = new System.Drawing.Size(23, 22);
			this.m_btnStop.Text = "&Stop";
			this.m_btnStop.Click += new System.EventHandler(this.OnBtnStop);
			// 
			// m_tssSep0
			// 
			this.m_tssSep0.Name = "m_tssSep0";
			this.m_tssSep0.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_tssSep0.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tbUrl
			// 
			this.m_tbUrl.Name = "m_tbUrl";
			this.m_tbUrl.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_tbUrl.Size = new System.Drawing.Size(200, 25);
			this.m_tbUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnTbUrlKeyDown);
			// 
			// m_btnGo
			// 
			this.m_btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.m_btnGo.Image = global::KeePass.Properties.Resources.B16x16_FTP;
			this.m_btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnGo.Name = "m_btnGo";
			this.m_btnGo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.m_btnGo.Size = new System.Drawing.Size(23, 22);
			this.m_btnGo.Text = "&Go";
			this.m_btnGo.Click += new System.EventHandler(this.OnBtnGo);
			// 
			// InternalBrowserForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(779, 528);
			this.Controls.Add(this.m_webBrowser);
			this.Controls.Add(this.m_toolNav);
			this.Controls.Add(this.m_statusMain);
			this.Controls.Add(this.m_menuMain);
			this.MainMenuStrip = this.m_menuMain;
			this.MinimizeBox = false;
			this.Name = "InternalBrowserForm";
			this.ShowInTaskbar = false;
			this.Text = "<DYN>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.SizeChanged += new System.EventHandler(this.OnFormSizeChanged);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			this.m_menuMain.ResumeLayout(false);
			this.m_menuMain.PerformLayout();
			this.m_statusMain.ResumeLayout(false);
			this.m_statusMain.PerformLayout();
			this.m_toolNav.ResumeLayout(false);
			this.m_toolNav.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private KeePass.UI.CustomMenuStripEx m_menuMain;
		private System.Windows.Forms.ToolStripMenuItem m_menuFile;
		private System.Windows.Forms.ToolStripMenuItem m_menuFileExit;
		private System.Windows.Forms.StatusStrip m_statusMain;
		private System.Windows.Forms.ToolStripStatusLabel m_lblStatus;
		private System.Windows.Forms.WebBrowser m_webBrowser;
		private KeePass.UI.CustomToolStripEx m_toolNav;
		private System.Windows.Forms.ToolStripTextBox m_tbUrl;
		private System.Windows.Forms.ToolStripButton m_btnBack;
		private System.Windows.Forms.ToolStripButton m_btnForward;
		private System.Windows.Forms.ToolStripButton m_btnReload;
		private System.Windows.Forms.ToolStripButton m_btnStop;
		private System.Windows.Forms.ToolStripSeparator m_tssSep0;
		private System.Windows.Forms.ToolStripButton m_btnGo;
	}
}