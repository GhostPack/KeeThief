namespace KeePass.Forms
{
	partial class EditAutoTypeItemForm
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
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_lblTargetWindow = new System.Windows.Forms.Label();
			this.m_lblKeySeqInsertInfo = new System.Windows.Forms.Label();
			this.m_lblSeparator = new System.Windows.Forms.Label();
			this.m_lblOpenHint = new System.Windows.Forms.Label();
			this.m_lnkWildcardRegexHint = new System.Windows.Forms.LinkLabel();
			this.m_rbSeqDefault = new System.Windows.Forms.RadioButton();
			this.m_rbSeqCustom = new System.Windows.Forms.RadioButton();
			this.m_cmbWindow = new KeePass.UI.ImageComboBoxEx();
			this.m_rtbPlaceholders = new KeePass.UI.CustomRichTextBoxEx();
			this.m_rbKeySeq = new KeePass.UI.CustomRichTextBoxEx();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.SuspendLayout();
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(511, 60);
			this.m_bannerImage.TabIndex = 0;
			this.m_bannerImage.TabStop = false;
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(343, 388);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 8;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(424, 388);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 9;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Location = new System.Drawing.Point(12, 388);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
			this.m_btnHelp.TabIndex = 10;
			this.m_btnHelp.Text = "&Help";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.OnBtnHelp);
			// 
			// m_lblTargetWindow
			// 
			this.m_lblTargetWindow.AutoSize = true;
			this.m_lblTargetWindow.Location = new System.Drawing.Point(9, 75);
			this.m_lblTargetWindow.Name = "m_lblTargetWindow";
			this.m_lblTargetWindow.Size = new System.Drawing.Size(80, 13);
			this.m_lblTargetWindow.TabIndex = 12;
			this.m_lblTargetWindow.Text = "Target window:";
			// 
			// m_lblKeySeqInsertInfo
			// 
			this.m_lblKeySeqInsertInfo.AutoSize = true;
			this.m_lblKeySeqInsertInfo.Location = new System.Drawing.Point(29, 214);
			this.m_lblKeySeqInsertInfo.Name = "m_lblKeySeqInsertInfo";
			this.m_lblKeySeqInsertInfo.Size = new System.Drawing.Size(94, 13);
			this.m_lblKeySeqInsertInfo.TabIndex = 6;
			this.m_lblKeySeqInsertInfo.Text = "Insert placeholder:";
			// 
			// m_lblSeparator
			// 
			this.m_lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSeparator.Location = new System.Drawing.Point(0, 379);
			this.m_lblSeparator.Name = "m_lblSeparator";
			this.m_lblSeparator.Size = new System.Drawing.Size(511, 2);
			this.m_lblSeparator.TabIndex = 11;
			// 
			// m_lblOpenHint
			// 
			this.m_lblOpenHint.AutoSize = true;
			this.m_lblOpenHint.Location = new System.Drawing.Point(92, 99);
			this.m_lblOpenHint.Name = "m_lblOpenHint";
			this.m_lblOpenHint.Size = new System.Drawing.Size(351, 13);
			this.m_lblOpenHint.TabIndex = 1;
			this.m_lblOpenHint.Text = "Click the drop-down button on the right to see currently opened windows.";
			// 
			// m_lnkWildcardRegexHint
			// 
			this.m_lnkWildcardRegexHint.AutoSize = true;
			this.m_lnkWildcardRegexHint.Location = new System.Drawing.Point(92, 116);
			this.m_lnkWildcardRegexHint.Name = "m_lnkWildcardRegexHint";
			this.m_lnkWildcardRegexHint.Size = new System.Drawing.Size(270, 13);
			this.m_lnkWildcardRegexHint.TabIndex = 2;
			this.m_lnkWildcardRegexHint.TabStop = true;
			this.m_lnkWildcardRegexHint.Text = "Simple wildcards and regular expressions are supported.";
			this.m_lnkWildcardRegexHint.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnWildcardRegexLinkClicked);
			// 
			// m_rbSeqDefault
			// 
			this.m_rbSeqDefault.AutoSize = true;
			this.m_rbSeqDefault.Location = new System.Drawing.Point(12, 142);
			this.m_rbSeqDefault.Name = "m_rbSeqDefault";
			this.m_rbSeqDefault.Size = new System.Drawing.Size(234, 17);
			this.m_rbSeqDefault.TabIndex = 3;
			this.m_rbSeqDefault.TabStop = true;
			this.m_rbSeqDefault.Text = "Use default keystroke sequence of the entry";
			this.m_rbSeqDefault.UseVisualStyleBackColor = true;
			this.m_rbSeqDefault.CheckedChanged += new System.EventHandler(this.OnSeqDefaultCheckedChanged);
			// 
			// m_rbSeqCustom
			// 
			this.m_rbSeqCustom.AutoSize = true;
			this.m_rbSeqCustom.Location = new System.Drawing.Point(12, 164);
			this.m_rbSeqCustom.Name = "m_rbSeqCustom";
			this.m_rbSeqCustom.Size = new System.Drawing.Size(183, 17);
			this.m_rbSeqCustom.TabIndex = 4;
			this.m_rbSeqCustom.TabStop = true;
			this.m_rbSeqCustom.Text = "Use custom keystroke sequence:";
			this.m_rbSeqCustom.UseVisualStyleBackColor = true;
			this.m_rbSeqCustom.CheckedChanged += new System.EventHandler(this.OnSeqCustomCheckedChanged);
			// 
			// m_cmbWindow
			// 
			this.m_cmbWindow.IntegralHeight = false;
			this.m_cmbWindow.Location = new System.Drawing.Point(95, 72);
			this.m_cmbWindow.Name = "m_cmbWindow";
			this.m_cmbWindow.Size = new System.Drawing.Size(404, 21);
			this.m_cmbWindow.TabIndex = 0;
			this.m_cmbWindow.SelectedIndexChanged += new System.EventHandler(this.OnWindowSelectedIndexChanged);
			this.m_cmbWindow.TextUpdate += new System.EventHandler(this.OnWindowTextUpdate);
			// 
			// m_rtbPlaceholders
			// 
			this.m_rtbPlaceholders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_rtbPlaceholders.Location = new System.Drawing.Point(32, 230);
			this.m_rtbPlaceholders.Name = "m_rtbPlaceholders";
			this.m_rtbPlaceholders.ReadOnly = true;
			this.m_rtbPlaceholders.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.m_rtbPlaceholders.Size = new System.Drawing.Size(467, 136);
			this.m_rtbPlaceholders.TabIndex = 7;
			this.m_rtbPlaceholders.Text = "";
			this.m_rtbPlaceholders.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.OnPlaceholdersLinkClicked);
			// 
			// m_rbKeySeq
			// 
			this.m_rbKeySeq.DetectUrls = false;
			this.m_rbKeySeq.Font = new System.Drawing.Font("Courier New", 8.25F);
			this.m_rbKeySeq.HideSelection = false;
			this.m_rbKeySeq.Location = new System.Drawing.Point(32, 187);
			this.m_rbKeySeq.Multiline = false;
			this.m_rbKeySeq.Name = "m_rbKeySeq";
			this.m_rbKeySeq.Size = new System.Drawing.Size(467, 21);
			this.m_rbKeySeq.TabIndex = 5;
			this.m_rbKeySeq.Text = "";
			this.m_rbKeySeq.TextChanged += new System.EventHandler(this.OnTextChangedKeySeq);
			// 
			// EditAutoTypeItemForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(511, 423);
			this.Controls.Add(this.m_rbSeqCustom);
			this.Controls.Add(this.m_rbSeqDefault);
			this.Controls.Add(this.m_lnkWildcardRegexHint);
			this.Controls.Add(this.m_lblOpenHint);
			this.Controls.Add(this.m_cmbWindow);
			this.Controls.Add(this.m_rtbPlaceholders);
			this.Controls.Add(this.m_rbKeySeq);
			this.Controls.Add(this.m_lblSeparator);
			this.Controls.Add(this.m_lblKeySeqInsertInfo);
			this.Controls.Add(this.m_lblTargetWindow);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_bannerImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditAutoTypeItemForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Auto-Type Item";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.Shown += new System.EventHandler(this.OnFormShown);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Label m_lblTargetWindow;
		private System.Windows.Forms.Label m_lblKeySeqInsertInfo;
		private System.Windows.Forms.Label m_lblSeparator;
		private KeePass.UI.CustomRichTextBoxEx m_rbKeySeq;
		private KeePass.UI.CustomRichTextBoxEx m_rtbPlaceholders;
		private KeePass.UI.ImageComboBoxEx m_cmbWindow;
		private System.Windows.Forms.Label m_lblOpenHint;
		private System.Windows.Forms.LinkLabel m_lnkWildcardRegexHint;
		private System.Windows.Forms.RadioButton m_rbSeqDefault;
		private System.Windows.Forms.RadioButton m_rbSeqCustom;
	}
}