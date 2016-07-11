namespace KeePass.Forms
{
	partial class AboutForm
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
			this.m_lblCopyright = new System.Windows.Forms.Label();
			this.m_lblOsi = new System.Windows.Forms.Label();
			this.m_lblGpl = new System.Windows.Forms.Label();
			this.m_linkHomepage = new System.Windows.Forms.LinkLabel();
			this.m_linkHelp = new System.Windows.Forms.LinkLabel();
			this.m_linkLicense = new System.Windows.Forms.LinkLabel();
			this.m_linkAcknowledgements = new System.Windows.Forms.LinkLabel();
			this.m_linkDonate = new System.Windows.Forms.LinkLabel();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_lvComponents = new KeePass.UI.CustomListViewEx();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.SuspendLayout();
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(424, 60);
			this.m_bannerImage.TabIndex = 0;
			this.m_bannerImage.TabStop = false;
			// 
			// m_lblCopyright
			// 
			this.m_lblCopyright.Location = new System.Drawing.Point(10, 72);
			this.m_lblCopyright.Name = "m_lblCopyright";
			this.m_lblCopyright.Size = new System.Drawing.Size(402, 15);
			this.m_lblCopyright.TabIndex = 1;
			// 
			// m_lblOsi
			// 
			this.m_lblOsi.Location = new System.Drawing.Point(10, 96);
			this.m_lblOsi.Name = "m_lblOsi";
			this.m_lblOsi.Size = new System.Drawing.Size(402, 14);
			this.m_lblOsi.TabIndex = 2;
			this.m_lblOsi.Text = "KeePass is OSI Certified Open Source Software.";
			// 
			// m_lblGpl
			// 
			this.m_lblGpl.Location = new System.Drawing.Point(10, 119);
			this.m_lblGpl.Name = "m_lblGpl";
			this.m_lblGpl.Size = new System.Drawing.Size(402, 27);
			this.m_lblGpl.TabIndex = 3;
			this.m_lblGpl.Text = "The program is distributed under the terms of the GNU General Public License v2 o" +
				"r later.";
			// 
			// m_linkHomepage
			// 
			this.m_linkHomepage.AutoSize = true;
			this.m_linkHomepage.Location = new System.Drawing.Point(10, 155);
			this.m_linkHomepage.Name = "m_linkHomepage";
			this.m_linkHomepage.Size = new System.Drawing.Size(91, 13);
			this.m_linkHomepage.TabIndex = 4;
			this.m_linkHomepage.TabStop = true;
			this.m_linkHomepage.Text = "KeePass Website";
			this.m_linkHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkHomepage);
			// 
			// m_linkHelp
			// 
			this.m_linkHelp.AutoSize = true;
			this.m_linkHelp.Location = new System.Drawing.Point(213, 155);
			this.m_linkHelp.Name = "m_linkHelp";
			this.m_linkHelp.Size = new System.Drawing.Size(29, 13);
			this.m_linkHelp.TabIndex = 6;
			this.m_linkHelp.TabStop = true;
			this.m_linkHelp.Text = "Help";
			this.m_linkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkHelpFile);
			// 
			// m_linkLicense
			// 
			this.m_linkLicense.AutoSize = true;
			this.m_linkLicense.Location = new System.Drawing.Point(10, 177);
			this.m_linkLicense.Name = "m_linkLicense";
			this.m_linkLicense.Size = new System.Drawing.Size(44, 13);
			this.m_linkLicense.TabIndex = 7;
			this.m_linkLicense.TabStop = true;
			this.m_linkLicense.Text = "License";
			this.m_linkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkLicenseFile);
			// 
			// m_linkAcknowledgements
			// 
			this.m_linkAcknowledgements.AutoSize = true;
			this.m_linkAcknowledgements.Location = new System.Drawing.Point(107, 155);
			this.m_linkAcknowledgements.Name = "m_linkAcknowledgements";
			this.m_linkAcknowledgements.Size = new System.Drawing.Size(100, 13);
			this.m_linkAcknowledgements.TabIndex = 5;
			this.m_linkAcknowledgements.TabStop = true;
			this.m_linkAcknowledgements.Text = "Acknowledgements";
			this.m_linkAcknowledgements.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkAcknowledgements);
			// 
			// m_linkDonate
			// 
			this.m_linkDonate.AutoSize = true;
			this.m_linkDonate.Location = new System.Drawing.Point(107, 177);
			this.m_linkDonate.Name = "m_linkDonate";
			this.m_linkDonate.Size = new System.Drawing.Size(42, 13);
			this.m_linkDonate.TabIndex = 8;
			this.m_linkDonate.TabStop = true;
			this.m_linkDonate.Text = "Donate";
			this.m_linkDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkDonate);
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(337, 315);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 0;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			// 
			// m_lvComponents
			// 
			this.m_lvComponents.FullRowSelect = true;
			this.m_lvComponents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvComponents.Location = new System.Drawing.Point(13, 203);
			this.m_lvComponents.Name = "m_lvComponents";
			this.m_lvComponents.Size = new System.Drawing.Size(398, 101);
			this.m_lvComponents.TabIndex = 9;
			this.m_lvComponents.UseCompatibleStateImageBehavior = false;
			this.m_lvComponents.View = System.Windows.Forms.View.Details;
			// 
			// AboutForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnOK;
			this.ClientSize = new System.Drawing.Size(424, 350);
			this.Controls.Add(this.m_lvComponents);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_linkDonate);
			this.Controls.Add(this.m_linkAcknowledgements);
			this.Controls.Add(this.m_linkLicense);
			this.Controls.Add(this.m_linkHelp);
			this.Controls.Add(this.m_linkHomepage);
			this.Controls.Add(this.m_lblGpl);
			this.Controls.Add(this.m_lblOsi);
			this.Controls.Add(this.m_lblCopyright);
			this.Controls.Add(this.m_bannerImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About KeePass";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Label m_lblCopyright;
		private System.Windows.Forms.Label m_lblOsi;
		private System.Windows.Forms.Label m_lblGpl;
		private System.Windows.Forms.LinkLabel m_linkHomepage;
		private System.Windows.Forms.LinkLabel m_linkHelp;
		private System.Windows.Forms.LinkLabel m_linkLicense;
		private System.Windows.Forms.LinkLabel m_linkAcknowledgements;
		private System.Windows.Forms.LinkLabel m_linkDonate;
		private System.Windows.Forms.Button m_btnOK;
		private KeePass.UI.CustomListViewEx m_lvComponents;
	}
}