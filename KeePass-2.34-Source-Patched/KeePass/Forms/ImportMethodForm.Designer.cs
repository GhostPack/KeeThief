namespace KeePass.Forms
{
	partial class ImportMethodForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportMethodForm));
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_lblIntro = new System.Windows.Forms.Label();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_radioCreateNew = new System.Windows.Forms.RadioButton();
			this.m_lblCreateNewHint = new System.Windows.Forms.Label();
			this.m_radioKeepExisting = new System.Windows.Forms.RadioButton();
			this.m_lblExistingHint = new System.Windows.Forms.Label();
			this.m_radioOverwrite = new System.Windows.Forms.RadioButton();
			this.m_lblOverwriteHint = new System.Windows.Forms.Label();
			this.m_radioOverwriteIfNewer = new System.Windows.Forms.RadioButton();
			this.m_lblOverwriteIfNewerHint = new System.Windows.Forms.Label();
			this.m_radioSynchronize = new System.Windows.Forms.RadioButton();
			this.m_lblSynchronizeHint = new System.Windows.Forms.Label();
			this.m_lblSeparator = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(345, 431);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 11;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(426, 431);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 12;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_lblIntro
			// 
			this.m_lblIntro.Location = new System.Drawing.Point(12, 72);
			this.m_lblIntro.Name = "m_lblIntro";
			this.m_lblIntro.Size = new System.Drawing.Size(489, 28);
			this.m_lblIntro.TabIndex = 1;
			this.m_lblIntro.Text = "The file format that you have selected to import supports group and/or entry IDs." +
				" Please choose an import behavior.";
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(513, 60);
			this.m_bannerImage.TabIndex = 3;
			this.m_bannerImage.TabStop = false;
			// 
			// m_radioCreateNew
			// 
			this.m_radioCreateNew.AutoSize = true;
			this.m_radioCreateNew.Location = new System.Drawing.Point(15, 114);
			this.m_radioCreateNew.Name = "m_radioCreateNew";
			this.m_radioCreateNew.Size = new System.Drawing.Size(60, 17);
			this.m_radioCreateNew.TabIndex = 0;
			this.m_radioCreateNew.TabStop = true;
			this.m_radioCreateNew.Text = "<DYN>";
			this.m_radioCreateNew.UseVisualStyleBackColor = true;
			// 
			// m_lblCreateNewHint
			// 
			this.m_lblCreateNewHint.Location = new System.Drawing.Point(32, 134);
			this.m_lblCreateNewHint.Name = "m_lblCreateNewHint";
			this.m_lblCreateNewHint.Size = new System.Drawing.Size(469, 28);
			this.m_lblCreateNewHint.TabIndex = 2;
			this.m_lblCreateNewHint.Text = "If you select this option, KeePass will create new IDs for all groups and entries" +
				". Consequently no existing groups and entries of the currently opened database w" +
				"ill be modified or overwritten.";
			// 
			// m_radioKeepExisting
			// 
			this.m_radioKeepExisting.AutoSize = true;
			this.m_radioKeepExisting.Location = new System.Drawing.Point(15, 174);
			this.m_radioKeepExisting.Name = "m_radioKeepExisting";
			this.m_radioKeepExisting.Size = new System.Drawing.Size(60, 17);
			this.m_radioKeepExisting.TabIndex = 3;
			this.m_radioKeepExisting.TabStop = true;
			this.m_radioKeepExisting.Text = "<DYN>";
			this.m_radioKeepExisting.UseVisualStyleBackColor = true;
			// 
			// m_lblExistingHint
			// 
			this.m_lblExistingHint.Location = new System.Drawing.Point(32, 194);
			this.m_lblExistingHint.Name = "m_lblExistingHint";
			this.m_lblExistingHint.Size = new System.Drawing.Size(469, 27);
			this.m_lblExistingHint.TabIndex = 4;
			this.m_lblExistingHint.Text = "Existing entries of the currently opened database will not be modified. Only new " +
				"entries will be added.";
			// 
			// m_radioOverwrite
			// 
			this.m_radioOverwrite.AutoSize = true;
			this.m_radioOverwrite.Location = new System.Drawing.Point(15, 233);
			this.m_radioOverwrite.Name = "m_radioOverwrite";
			this.m_radioOverwrite.Size = new System.Drawing.Size(60, 17);
			this.m_radioOverwrite.TabIndex = 5;
			this.m_radioOverwrite.TabStop = true;
			this.m_radioOverwrite.Text = "<DYN>";
			this.m_radioOverwrite.UseVisualStyleBackColor = true;
			// 
			// m_lblOverwriteHint
			// 
			this.m_lblOverwriteHint.Location = new System.Drawing.Point(32, 253);
			this.m_lblOverwriteHint.Name = "m_lblOverwriteHint";
			this.m_lblOverwriteHint.Size = new System.Drawing.Size(469, 28);
			this.m_lblOverwriteHint.TabIndex = 6;
			this.m_lblOverwriteHint.Text = "KeePass will replace all existing groups and entries by the ones in the file to i" +
				"mport, if they have the same ID.";
			// 
			// m_radioOverwriteIfNewer
			// 
			this.m_radioOverwriteIfNewer.AutoSize = true;
			this.m_radioOverwriteIfNewer.Location = new System.Drawing.Point(15, 293);
			this.m_radioOverwriteIfNewer.Name = "m_radioOverwriteIfNewer";
			this.m_radioOverwriteIfNewer.Size = new System.Drawing.Size(60, 17);
			this.m_radioOverwriteIfNewer.TabIndex = 7;
			this.m_radioOverwriteIfNewer.TabStop = true;
			this.m_radioOverwriteIfNewer.Text = "<DYN>";
			this.m_radioOverwriteIfNewer.UseVisualStyleBackColor = true;
			// 
			// m_lblOverwriteIfNewerHint
			// 
			this.m_lblOverwriteIfNewerHint.Location = new System.Drawing.Point(32, 313);
			this.m_lblOverwriteIfNewerHint.Name = "m_lblOverwriteIfNewerHint";
			this.m_lblOverwriteIfNewerHint.Size = new System.Drawing.Size(469, 27);
			this.m_lblOverwriteIfNewerHint.TabIndex = 8;
			this.m_lblOverwriteIfNewerHint.Text = "KeePass will compare the last-modified times of the groups/entries and replace th" +
				"e existing ones only if the ones to import are newer.";
			// 
			// m_radioSynchronize
			// 
			this.m_radioSynchronize.AutoSize = true;
			this.m_radioSynchronize.Location = new System.Drawing.Point(15, 352);
			this.m_radioSynchronize.Name = "m_radioSynchronize";
			this.m_radioSynchronize.Size = new System.Drawing.Size(60, 17);
			this.m_radioSynchronize.TabIndex = 9;
			this.m_radioSynchronize.TabStop = true;
			this.m_radioSynchronize.Text = "<DYN>";
			this.m_radioSynchronize.UseVisualStyleBackColor = true;
			// 
			// m_lblSynchronizeHint
			// 
			this.m_lblSynchronizeHint.Location = new System.Drawing.Point(32, 372);
			this.m_lblSynchronizeHint.Name = "m_lblSynchronizeHint";
			this.m_lblSynchronizeHint.Size = new System.Drawing.Size(469, 40);
			this.m_lblSynchronizeHint.TabIndex = 10;
			this.m_lblSynchronizeHint.Text = resources.GetString("m_lblSynchronizeHint.Text");
			// 
			// m_lblSeparator
			// 
			this.m_lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSeparator.Location = new System.Drawing.Point(0, 425);
			this.m_lblSeparator.Name = "m_lblSeparator";
			this.m_lblSeparator.Size = new System.Drawing.Size(513, 2);
			this.m_lblSeparator.TabIndex = 13;
			// 
			// ImportMethodForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(513, 466);
			this.Controls.Add(this.m_lblSeparator);
			this.Controls.Add(this.m_lblSynchronizeHint);
			this.Controls.Add(this.m_radioSynchronize);
			this.Controls.Add(this.m_lblOverwriteIfNewerHint);
			this.Controls.Add(this.m_radioOverwriteIfNewer);
			this.Controls.Add(this.m_lblOverwriteHint);
			this.Controls.Add(this.m_radioOverwrite);
			this.Controls.Add(this.m_lblExistingHint);
			this.Controls.Add(this.m_radioKeepExisting);
			this.Controls.Add(this.m_lblCreateNewHint);
			this.Controls.Add(this.m_radioCreateNew);
			this.Controls.Add(this.m_bannerImage);
			this.Controls.Add(this.m_lblIntro);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImportMethodForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<DYN>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Label m_lblIntro;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.RadioButton m_radioCreateNew;
		private System.Windows.Forms.Label m_lblCreateNewHint;
		private System.Windows.Forms.RadioButton m_radioKeepExisting;
		private System.Windows.Forms.Label m_lblExistingHint;
		private System.Windows.Forms.RadioButton m_radioOverwrite;
		private System.Windows.Forms.Label m_lblOverwriteHint;
		private System.Windows.Forms.RadioButton m_radioOverwriteIfNewer;
		private System.Windows.Forms.Label m_lblOverwriteIfNewerHint;
		private System.Windows.Forms.RadioButton m_radioSynchronize;
		private System.Windows.Forms.Label m_lblSynchronizeHint;
		private System.Windows.Forms.Label m_lblSeparator;
	}
}