namespace KeePass.Forms
{
	partial class EntropyForm
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
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_picRandom = new System.Windows.Forms.PictureBox();
			this.m_lblHint = new System.Windows.Forms.Label();
			this.m_lblGeneratedHint = new System.Windows.Forms.Label();
			this.m_lblStatus = new System.Windows.Forms.Label();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_grpMouse = new System.Windows.Forms.GroupBox();
			this.m_pbGenerated = new KeePass.UI.QualityProgressBar();
			this.m_grpKeyboard = new System.Windows.Forms.GroupBox();
			this.m_lblKeysDesc = new System.Windows.Forms.Label();
			this.m_lblKeysIntro = new System.Windows.Forms.Label();
			this.m_tbEdit = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.m_picRandom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_grpMouse.SuspendLayout();
			this.m_grpKeyboard.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(462, 388);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 1;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(543, 388);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 2;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_picRandom
			// 
			this.m_picRandom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.m_picRandom.Location = new System.Drawing.Point(10, 47);
			this.m_picRandom.Name = "m_picRandom";
			this.m_picRandom.Size = new System.Drawing.Size(282, 233);
			this.m_picRandom.TabIndex = 2;
			this.m_picRandom.TabStop = false;
			this.m_picRandom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnRandomMouseMove);
			// 
			// m_lblHint
			// 
			this.m_lblHint.Location = new System.Drawing.Point(6, 16);
			this.m_lblHint.Name = "m_lblHint";
			this.m_lblHint.Size = new System.Drawing.Size(289, 28);
			this.m_lblHint.TabIndex = 0;
			this.m_lblHint.Text = "Move the mouse randomly in the following field to generate random bits:";
			// 
			// m_lblGeneratedHint
			// 
			this.m_lblGeneratedHint.AutoSize = true;
			this.m_lblGeneratedHint.Location = new System.Drawing.Point(6, 289);
			this.m_lblGeneratedHint.Name = "m_lblGeneratedHint";
			this.m_lblGeneratedHint.Size = new System.Drawing.Size(79, 13);
			this.m_lblGeneratedHint.TabIndex = 1;
			this.m_lblGeneratedHint.Text = "Generated bits:";
			// 
			// m_lblStatus
			// 
			this.m_lblStatus.Location = new System.Drawing.Point(245, 289);
			this.m_lblStatus.Name = "m_lblStatus";
			this.m_lblStatus.Size = new System.Drawing.Size(50, 13);
			this.m_lblStatus.TabIndex = 3;
			this.m_lblStatus.Text = "0 bits";
			this.m_lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(630, 60);
			this.m_bannerImage.TabIndex = 8;
			this.m_bannerImage.TabStop = false;
			// 
			// m_grpMouse
			// 
			this.m_grpMouse.Controls.Add(this.m_lblHint);
			this.m_grpMouse.Controls.Add(this.m_picRandom);
			this.m_grpMouse.Controls.Add(this.m_lblGeneratedHint);
			this.m_grpMouse.Controls.Add(this.m_pbGenerated);
			this.m_grpMouse.Controls.Add(this.m_lblStatus);
			this.m_grpMouse.Location = new System.Drawing.Point(12, 66);
			this.m_grpMouse.Name = "m_grpMouse";
			this.m_grpMouse.Size = new System.Drawing.Size(302, 316);
			this.m_grpMouse.TabIndex = 3;
			this.m_grpMouse.TabStop = false;
			this.m_grpMouse.Text = "Random mouse input";
			// 
			// m_pbGenerated
			// 
			this.m_pbGenerated.Location = new System.Drawing.Point(91, 289);
			this.m_pbGenerated.Name = "m_pbGenerated";
			this.m_pbGenerated.Size = new System.Drawing.Size(148, 13);
			this.m_pbGenerated.TabIndex = 2;
			this.m_pbGenerated.TabStop = false;
			// 
			// m_grpKeyboard
			// 
			this.m_grpKeyboard.Controls.Add(this.m_lblKeysDesc);
			this.m_grpKeyboard.Controls.Add(this.m_lblKeysIntro);
			this.m_grpKeyboard.Controls.Add(this.m_tbEdit);
			this.m_grpKeyboard.Location = new System.Drawing.Point(320, 66);
			this.m_grpKeyboard.Name = "m_grpKeyboard";
			this.m_grpKeyboard.Size = new System.Drawing.Size(298, 316);
			this.m_grpKeyboard.TabIndex = 0;
			this.m_grpKeyboard.TabStop = false;
			this.m_grpKeyboard.Text = "Random keyboard input";
			// 
			// m_lblKeysDesc
			// 
			this.m_lblKeysDesc.Location = new System.Drawing.Point(6, 254);
			this.m_lblKeysDesc.Name = "m_lblKeysDesc";
			this.m_lblKeysDesc.Size = new System.Drawing.Size(286, 56);
			this.m_lblKeysDesc.TabIndex = 2;
			this.m_lblKeysDesc.Text = "Just hack some random keys into the box above. You do not need to remember them. " +
				"They are only used as a random seed for a cryptographically strong random number" +
				" generator.";
			// 
			// m_lblKeysIntro
			// 
			this.m_lblKeysIntro.AutoSize = true;
			this.m_lblKeysIntro.Location = new System.Drawing.Point(6, 16);
			this.m_lblKeysIntro.Name = "m_lblKeysIntro";
			this.m_lblKeysIntro.Size = new System.Drawing.Size(229, 13);
			this.m_lblKeysIntro.TabIndex = 1;
			this.m_lblKeysIntro.Text = "Type random characters into the following field:";
			// 
			// m_tbEdit
			// 
			this.m_tbEdit.AcceptsReturn = true;
			this.m_tbEdit.Location = new System.Drawing.Point(9, 47);
			this.m_tbEdit.Multiline = true;
			this.m_tbEdit.Name = "m_tbEdit";
			this.m_tbEdit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbEdit.Size = new System.Drawing.Size(279, 204);
			this.m_tbEdit.TabIndex = 0;
			// 
			// EntropyForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(630, 423);
			this.Controls.Add(this.m_grpKeyboard);
			this.Controls.Add(this.m_grpMouse);
			this.Controls.Add(this.m_bannerImage);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EntropyForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_picRandom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_grpMouse.ResumeLayout(false);
			this.m_grpMouse.PerformLayout();
			this.m_grpKeyboard.ResumeLayout(false);
			this.m_grpKeyboard.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.PictureBox m_picRandom;
		private System.Windows.Forms.Label m_lblHint;
		private System.Windows.Forms.Label m_lblGeneratedHint;
		private KeePass.UI.QualityProgressBar m_pbGenerated;
		private System.Windows.Forms.Label m_lblStatus;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.GroupBox m_grpMouse;
		private System.Windows.Forms.GroupBox m_grpKeyboard;
		private System.Windows.Forms.TextBox m_tbEdit;
		private System.Windows.Forms.Label m_lblKeysIntro;
		private System.Windows.Forms.Label m_lblKeysDesc;
	}
}