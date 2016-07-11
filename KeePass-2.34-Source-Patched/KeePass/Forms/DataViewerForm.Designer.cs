namespace KeePass.Forms
{
	partial class DataViewerForm
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
			this.m_picBox = new System.Windows.Forms.PictureBox();
			this.m_webBrowser = new System.Windows.Forms.WebBrowser();
			this.m_pnlImageViewer = new System.Windows.Forms.Panel();
			this.m_statusMain = new System.Windows.Forms.StatusStrip();
			this.m_tssStatusMain = new System.Windows.Forms.ToolStripStatusLabel();
			this.m_rtbText = new KeePass.UI.CustomRichTextBoxEx();
			this.m_toolMain = new KeePass.UI.CustomToolStripEx();
			this.m_tslViewer = new System.Windows.Forms.ToolStripLabel();
			this.m_tscViewers = new System.Windows.Forms.ToolStripComboBox();
			this.m_tssSeparator0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tslEncoding = new System.Windows.Forms.ToolStripLabel();
			this.m_tscEncoding = new System.Windows.Forms.ToolStripComboBox();
			this.m_tslZoom = new System.Windows.Forms.ToolStripLabel();
			this.m_tscZoom = new System.Windows.Forms.ToolStripComboBox();
			((System.ComponentModel.ISupportInitialize)(this.m_picBox)).BeginInit();
			this.m_pnlImageViewer.SuspendLayout();
			this.m_statusMain.SuspendLayout();
			this.m_toolMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_picBox
			// 
			this.m_picBox.Location = new System.Drawing.Point(31, 26);
			this.m_picBox.Name = "m_picBox";
			this.m_picBox.Size = new System.Drawing.Size(174, 130);
			this.m_picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.m_picBox.TabIndex = 1;
			this.m_picBox.TabStop = false;
			// 
			// m_webBrowser
			// 
			this.m_webBrowser.AllowWebBrowserDrop = false;
			this.m_webBrowser.Location = new System.Drawing.Point(23, 160);
			this.m_webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.m_webBrowser.Name = "m_webBrowser";
			this.m_webBrowser.ScriptErrorsSuppressed = true;
			this.m_webBrowser.Size = new System.Drawing.Size(193, 158);
			this.m_webBrowser.TabIndex = 2;
			// 
			// m_pnlImageViewer
			// 
			this.m_pnlImageViewer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_pnlImageViewer.Controls.Add(this.m_picBox);
			this.m_pnlImageViewer.Location = new System.Drawing.Point(234, 70);
			this.m_pnlImageViewer.Name = "m_pnlImageViewer";
			this.m_pnlImageViewer.Size = new System.Drawing.Size(264, 212);
			this.m_pnlImageViewer.TabIndex = 1;
			// 
			// m_statusMain
			// 
			this.m_statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tssStatusMain});
			this.m_statusMain.Location = new System.Drawing.Point(0, 366);
			this.m_statusMain.Name = "m_statusMain";
			this.m_statusMain.Size = new System.Drawing.Size(524, 22);
			this.m_statusMain.TabIndex = 4;
			// 
			// m_tssStatusMain
			// 
			this.m_tssStatusMain.Name = "m_tssStatusMain";
			this.m_tssStatusMain.Size = new System.Drawing.Size(509, 17);
			this.m_tssStatusMain.Spring = true;
			this.m_tssStatusMain.Text = "<>";
			this.m_tssStatusMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_rtbText
			// 
			this.m_rtbText.Location = new System.Drawing.Point(23, 40);
			this.m_rtbText.Name = "m_rtbText";
			this.m_rtbText.ReadOnly = true;
			this.m_rtbText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.m_rtbText.Size = new System.Drawing.Size(190, 114);
			this.m_rtbText.TabIndex = 0;
			this.m_rtbText.Text = "";
			this.m_rtbText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.OnRichTextBoxLinkClicked);
			// 
			// m_toolMain
			// 
			this.m_toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tslViewer,
            this.m_tscViewers,
            this.m_tssSeparator0,
            this.m_tslEncoding,
            this.m_tscEncoding,
            this.m_tslZoom,
            this.m_tscZoom});
			this.m_toolMain.Location = new System.Drawing.Point(0, 0);
			this.m_toolMain.Name = "m_toolMain";
			this.m_toolMain.Size = new System.Drawing.Size(524, 25);
			this.m_toolMain.TabIndex = 3;
			this.m_toolMain.TabStop = true;
			// 
			// m_tslViewer
			// 
			this.m_tslViewer.Name = "m_tslViewer";
			this.m_tslViewer.Size = new System.Drawing.Size(21, 22);
			this.m_tslViewer.Text = "<>";
			// 
			// m_tscViewers
			// 
			this.m_tscViewers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_tscViewers.Name = "m_tscViewers";
			this.m_tscViewers.Size = new System.Drawing.Size(140, 25);
			this.m_tscViewers.SelectedIndexChanged += new System.EventHandler(this.OnViewersSelectedIndexChanged);
			// 
			// m_tssSeparator0
			// 
			this.m_tssSeparator0.Name = "m_tssSeparator0";
			this.m_tssSeparator0.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tslEncoding
			// 
			this.m_tslEncoding.Name = "m_tslEncoding";
			this.m_tslEncoding.Size = new System.Drawing.Size(21, 22);
			this.m_tslEncoding.Text = "<>";
			// 
			// m_tscEncoding
			// 
			this.m_tscEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_tscEncoding.Name = "m_tscEncoding";
			this.m_tscEncoding.Size = new System.Drawing.Size(200, 25);
			this.m_tscEncoding.SelectedIndexChanged += new System.EventHandler(this.OnEncodingSelectedIndexChanged);
			// 
			// m_tslZoom
			// 
			this.m_tslZoom.Name = "m_tslZoom";
			this.m_tslZoom.Size = new System.Drawing.Size(21, 22);
			this.m_tslZoom.Text = "<>";
			// 
			// m_tscZoom
			// 
			this.m_tscZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_tscZoom.Name = "m_tscZoom";
			this.m_tscZoom.Size = new System.Drawing.Size(75, 25);
			this.m_tscZoom.SelectedIndexChanged += new System.EventHandler(this.OnZoomSelectedIndexChanged);
			// 
			// DataViewerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(524, 388);
			this.Controls.Add(this.m_webBrowser);
			this.Controls.Add(this.m_pnlImageViewer);
			this.Controls.Add(this.m_rtbText);
			this.Controls.Add(this.m_toolMain);
			this.Controls.Add(this.m_statusMain);
			this.MinimizeBox = false;
			this.Name = "DataViewerForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.SizeChanged += new System.EventHandler(this.OnFormSizeChanged);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.m_picBox)).EndInit();
			this.m_pnlImageViewer.ResumeLayout(false);
			this.m_statusMain.ResumeLayout(false);
			this.m_statusMain.PerformLayout();
			this.m_toolMain.ResumeLayout(false);
			this.m_toolMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private KeePass.UI.CustomRichTextBoxEx m_rtbText;
		private System.Windows.Forms.PictureBox m_picBox;
		private System.Windows.Forms.WebBrowser m_webBrowser;
		private KeePass.UI.CustomToolStripEx m_toolMain;
		private System.Windows.Forms.ToolStripLabel m_tslViewer;
		private System.Windows.Forms.ToolStripComboBox m_tscViewers;
		private System.Windows.Forms.ToolStripSeparator m_tssSeparator0;
		private System.Windows.Forms.ToolStripLabel m_tslEncoding;
		private System.Windows.Forms.ToolStripComboBox m_tscEncoding;
		private System.Windows.Forms.Panel m_pnlImageViewer;
		private System.Windows.Forms.StatusStrip m_statusMain;
		private System.Windows.Forms.ToolStripStatusLabel m_tssStatusMain;
		private System.Windows.Forms.ToolStripLabel m_tslZoom;
		private System.Windows.Forms.ToolStripComboBox m_tscZoom;
	}
}