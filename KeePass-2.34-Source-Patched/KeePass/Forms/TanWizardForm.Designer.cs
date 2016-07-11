namespace KeePass.Forms
{
	partial class TanWizardForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TanWizardForm));
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_lblIntro = new System.Windows.Forms.Label();
			this.m_tbTANs = new System.Windows.Forms.TextBox();
			this.m_cbNumberTans = new System.Windows.Forms.CheckBox();
			this.m_lblSeparator = new System.Windows.Forms.Label();
			this.m_numTANsIndex = new System.Windows.Forms.NumericUpDown();
			this.m_lblTanChars = new System.Windows.Forms.Label();
			this.m_tbTanChars = new System.Windows.Forms.TextBox();
			this.m_lblToGroup = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_numTANsIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(403, 423);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 3;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(484, 423);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 4;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(571, 60);
			this.m_bannerImage.TabIndex = 2;
			this.m_bannerImage.TabStop = false;
			// 
			// m_lblIntro
			// 
			this.m_lblIntro.Location = new System.Drawing.Point(9, 91);
			this.m_lblIntro.Name = "m_lblIntro";
			this.m_lblIntro.Size = new System.Drawing.Size(550, 29);
			this.m_lblIntro.TabIndex = 5;
			this.m_lblIntro.Text = resources.GetString("m_lblIntro.Text");
			// 
			// m_tbTANs
			// 
			this.m_tbTANs.AcceptsReturn = true;
			this.m_tbTANs.Location = new System.Drawing.Point(12, 124);
			this.m_tbTANs.Multiline = true;
			this.m_tbTANs.Name = "m_tbTANs";
			this.m_tbTANs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbTANs.Size = new System.Drawing.Size(547, 208);
			this.m_tbTANs.TabIndex = 0;
			// 
			// m_cbNumberTans
			// 
			this.m_cbNumberTans.AutoSize = true;
			this.m_cbNumberTans.Location = new System.Drawing.Point(12, 344);
			this.m_cbNumberTans.Name = "m_cbNumberTans";
			this.m_cbNumberTans.Size = new System.Drawing.Size(275, 17);
			this.m_cbNumberTans.TabIndex = 1;
			this.m_cbNumberTans.Text = "Number TANs consecutively, starting from this value:";
			this.m_cbNumberTans.UseVisualStyleBackColor = true;
			this.m_cbNumberTans.CheckedChanged += new System.EventHandler(this.OnNumberTANsCheckedChanged);
			// 
			// m_lblSeparator
			// 
			this.m_lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSeparator.Location = new System.Drawing.Point(0, 416);
			this.m_lblSeparator.Name = "m_lblSeparator";
			this.m_lblSeparator.Size = new System.Drawing.Size(571, 2);
			this.m_lblSeparator.TabIndex = 6;
			// 
			// m_numTANsIndex
			// 
			this.m_numTANsIndex.Location = new System.Drawing.Point(293, 343);
			this.m_numTANsIndex.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.m_numTANsIndex.Name = "m_numTANsIndex";
			this.m_numTANsIndex.Size = new System.Drawing.Size(72, 20);
			this.m_numTANsIndex.TabIndex = 2;
			this.m_numTANsIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// m_lblTanChars
			// 
			this.m_lblTanChars.AutoSize = true;
			this.m_lblTanChars.Location = new System.Drawing.Point(9, 368);
			this.m_lblTanChars.Name = "m_lblTanChars";
			this.m_lblTanChars.Size = new System.Drawing.Size(200, 13);
			this.m_lblTanChars.TabIndex = 7;
			this.m_lblTanChars.Text = "TANs consist of the following characters:";
			// 
			// m_tbTanChars
			// 
			this.m_tbTanChars.Location = new System.Drawing.Point(12, 384);
			this.m_tbTanChars.Name = "m_tbTanChars";
			this.m_tbTanChars.Size = new System.Drawing.Size(547, 20);
			this.m_tbTanChars.TabIndex = 8;
			// 
			// m_lblToGroup
			// 
			this.m_lblToGroup.Location = new System.Drawing.Point(9, 71);
			this.m_lblToGroup.Name = "m_lblToGroup";
			this.m_lblToGroup.Size = new System.Drawing.Size(550, 15);
			this.m_lblToGroup.TabIndex = 9;
			this.m_lblToGroup.Text = "TANs are imported into the currently selected group";
			// 
			// TanWizardForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(571, 458);
			this.Controls.Add(this.m_lblToGroup);
			this.Controls.Add(this.m_tbTanChars);
			this.Controls.Add(this.m_lblTanChars);
			this.Controls.Add(this.m_numTANsIndex);
			this.Controls.Add(this.m_lblSeparator);
			this.Controls.Add(this.m_cbNumberTans);
			this.Controls.Add(this.m_tbTANs);
			this.Controls.Add(this.m_lblIntro);
			this.Controls.Add(this.m_bannerImage);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TanWizardForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_numTANsIndex)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Label m_lblIntro;
		private System.Windows.Forms.TextBox m_tbTANs;
		private System.Windows.Forms.CheckBox m_cbNumberTans;
		private System.Windows.Forms.Label m_lblSeparator;
		private System.Windows.Forms.NumericUpDown m_numTANsIndex;
		private System.Windows.Forms.Label m_lblTanChars;
		private System.Windows.Forms.TextBox m_tbTanChars;
		private System.Windows.Forms.Label m_lblToGroup;
	}
}