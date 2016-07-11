namespace KeePass.Forms
{
	partial class HelpSourceForm
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
			this.m_radioLocal = new System.Windows.Forms.RadioButton();
			this.m_radioOnline = new System.Windows.Forms.RadioButton();
			this.m_lblLocal = new System.Windows.Forms.Label();
			this.m_lblOnline = new System.Windows.Forms.Label();
			this.m_lblIntro = new System.Windows.Forms.Label();
			this.m_lblSeparator = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.SuspendLayout();
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(418, 60);
			this.m_bannerImage.TabIndex = 0;
			this.m_bannerImage.TabStop = false;
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(250, 240);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 0;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(331, 240);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 1;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_radioLocal
			// 
			this.m_radioLocal.AutoSize = true;
			this.m_radioLocal.Location = new System.Drawing.Point(12, 101);
			this.m_radioLocal.Name = "m_radioLocal";
			this.m_radioLocal.Size = new System.Drawing.Size(90, 17);
			this.m_radioLocal.TabIndex = 3;
			this.m_radioLocal.TabStop = true;
			this.m_radioLocal.Text = "&Local help file";
			this.m_radioLocal.UseVisualStyleBackColor = true;
			// 
			// m_radioOnline
			// 
			this.m_radioOnline.AutoSize = true;
			this.m_radioOnline.Location = new System.Drawing.Point(12, 162);
			this.m_radioOnline.Name = "m_radioOnline";
			this.m_radioOnline.Size = new System.Drawing.Size(111, 17);
			this.m_radioOnline.TabIndex = 5;
			this.m_radioOnline.TabStop = true;
			this.m_radioOnline.Text = "&Online help center";
			this.m_radioOnline.UseVisualStyleBackColor = true;
			// 
			// m_lblLocal
			// 
			this.m_lblLocal.Location = new System.Drawing.Point(28, 121);
			this.m_lblLocal.Name = "m_lblLocal";
			this.m_lblLocal.Size = new System.Drawing.Size(378, 28);
			this.m_lblLocal.TabIndex = 4;
			this.m_lblLocal.Text = "The local help file is a snapshot of the product documentation at the point when " +
				"this product version was published. It is not updated automatically.";
			// 
			// m_lblOnline
			// 
			this.m_lblOnline.Location = new System.Drawing.Point(28, 182);
			this.m_lblOnline.Name = "m_lblOnline";
			this.m_lblOnline.Size = new System.Drawing.Size(378, 29);
			this.m_lblOnline.TabIndex = 6;
			this.m_lblOnline.Text = "The online help center always contains the latest version of the product document" +
				"ation. Internet connection is required.";
			// 
			// m_lblIntro
			// 
			this.m_lblIntro.AutoSize = true;
			this.m_lblIntro.Location = new System.Drawing.Point(9, 72);
			this.m_lblIntro.Name = "m_lblIntro";
			this.m_lblIntro.Size = new System.Drawing.Size(377, 13);
			this.m_lblIntro.TabIndex = 2;
			this.m_lblIntro.Text = "Use the following help source when a \"Help\" button within KeePass is clicked:";
			// 
			// m_lblSeparator
			// 
			this.m_lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSeparator.Location = new System.Drawing.Point(0, 230);
			this.m_lblSeparator.Name = "m_lblSeparator";
			this.m_lblSeparator.Size = new System.Drawing.Size(418, 2);
			this.m_lblSeparator.TabIndex = 7;
			// 
			// HelpSourceForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(418, 275);
			this.Controls.Add(this.m_lblSeparator);
			this.Controls.Add(this.m_lblIntro);
			this.Controls.Add(this.m_lblOnline);
			this.Controls.Add(this.m_lblLocal);
			this.Controls.Add(this.m_radioOnline);
			this.Controls.Add(this.m_radioLocal);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_bannerImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HelpSourceForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<DYN>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.RadioButton m_radioLocal;
		private System.Windows.Forms.RadioButton m_radioOnline;
		private System.Windows.Forms.Label m_lblLocal;
		private System.Windows.Forms.Label m_lblOnline;
		private System.Windows.Forms.Label m_lblIntro;
		private System.Windows.Forms.Label m_lblSeparator;
	}
}