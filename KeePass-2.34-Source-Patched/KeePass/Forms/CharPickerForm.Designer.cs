namespace KeePass.Forms
{
	partial class CharPickerForm
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
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_lblChars = new System.Windows.Forms.Label();
			this.m_lblIndex = new System.Windows.Forms.Label();
			this.m_lblSelChars = new System.Windows.Forms.Label();
			this.m_tbSelected = new System.Windows.Forms.TextBox();
			this.m_cbHideChars = new System.Windows.Forms.CheckBox();
			this.m_pnlSelect = new System.Windows.Forms.Panel();
			this.m_pnlBottom = new System.Windows.Forms.Panel();
			this.m_pnlBottomRight = new System.Windows.Forms.Panel();
			this.m_pnlTop = new System.Windows.Forms.Panel();
			this.m_pnlTopLeft = new System.Windows.Forms.Panel();
			this.m_pnlLeft = new System.Windows.Forms.Panel();
			this.m_pnlRight = new System.Windows.Forms.Panel();
			this.m_pnlMiddleTopSpacer = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_pnlBottom.SuspendLayout();
			this.m_pnlBottomRight.SuspendLayout();
			this.m_pnlTop.SuspendLayout();
			this.m_pnlTopLeft.SuspendLayout();
			this.m_pnlLeft.SuspendLayout();
			this.m_pnlRight.SuspendLayout();
			this.m_pnlMiddleTopSpacer.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(8, 5);
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
			this.m_btnCancel.Location = new System.Drawing.Point(89, 5);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 1;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(628, 60);
			this.m_bannerImage.TabIndex = 3;
			this.m_bannerImage.TabStop = false;
			// 
			// m_lblChars
			// 
			this.m_lblChars.AutoSize = true;
			this.m_lblChars.Location = new System.Drawing.Point(12, 6);
			this.m_lblChars.Name = "m_lblChars";
			this.m_lblChars.Size = new System.Drawing.Size(56, 13);
			this.m_lblChars.TabIndex = 0;
			this.m_lblChars.Text = "Character:";
			// 
			// m_lblIndex
			// 
			this.m_lblIndex.AutoSize = true;
			this.m_lblIndex.Location = new System.Drawing.Point(12, 30);
			this.m_lblIndex.Name = "m_lblIndex";
			this.m_lblIndex.Size = new System.Drawing.Size(47, 13);
			this.m_lblIndex.TabIndex = 1;
			this.m_lblIndex.Text = "Position:";
			// 
			// m_lblSelChars
			// 
			this.m_lblSelChars.AutoSize = true;
			this.m_lblSelChars.Location = new System.Drawing.Point(12, 15);
			this.m_lblSelChars.Name = "m_lblSelChars";
			this.m_lblSelChars.Size = new System.Drawing.Size(36, 13);
			this.m_lblSelChars.TabIndex = 0;
			this.m_lblSelChars.Text = "Word:";
			// 
			// m_tbSelected
			// 
			this.m_tbSelected.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_tbSelected.Location = new System.Drawing.Point(0, 12);
			this.m_tbSelected.Name = "m_tbSelected";
			this.m_tbSelected.Size = new System.Drawing.Size(508, 20);
			this.m_tbSelected.TabIndex = 0;
			// 
			// m_cbHideChars
			// 
			this.m_cbHideChars.Appearance = System.Windows.Forms.Appearance.Button;
			this.m_cbHideChars.Location = new System.Drawing.Point(5, 10);
			this.m_cbHideChars.Name = "m_cbHideChars";
			this.m_cbHideChars.Size = new System.Drawing.Size(32, 23);
			this.m_cbHideChars.TabIndex = 0;
			this.m_cbHideChars.Text = "***";
			this.m_cbHideChars.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.m_cbHideChars.UseVisualStyleBackColor = true;
			this.m_cbHideChars.CheckedChanged += new System.EventHandler(this.OnHideCharsCheckedChanged);
			// 
			// m_pnlSelect
			// 
			this.m_pnlSelect.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_pnlSelect.Location = new System.Drawing.Point(71, 8);
			this.m_pnlSelect.Name = "m_pnlSelect";
			this.m_pnlSelect.Size = new System.Drawing.Size(545, 52);
			this.m_pnlSelect.TabIndex = 1;
			// 
			// m_pnlBottom
			// 
			this.m_pnlBottom.Controls.Add(this.m_pnlBottomRight);
			this.m_pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_pnlBottom.Location = new System.Drawing.Point(0, 175);
			this.m_pnlBottom.Name = "m_pnlBottom";
			this.m_pnlBottom.Size = new System.Drawing.Size(628, 40);
			this.m_pnlBottom.TabIndex = 2;
			// 
			// m_pnlBottomRight
			// 
			this.m_pnlBottomRight.Controls.Add(this.m_btnCancel);
			this.m_pnlBottomRight.Controls.Add(this.m_btnOK);
			this.m_pnlBottomRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.m_pnlBottomRight.Location = new System.Drawing.Point(452, 0);
			this.m_pnlBottomRight.Name = "m_pnlBottomRight";
			this.m_pnlBottomRight.Size = new System.Drawing.Size(176, 40);
			this.m_pnlBottomRight.TabIndex = 0;
			// 
			// m_pnlTop
			// 
			this.m_pnlTop.Controls.Add(this.m_pnlSelect);
			this.m_pnlTop.Controls.Add(this.m_pnlTopLeft);
			this.m_pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_pnlTop.Location = new System.Drawing.Point(0, 60);
			this.m_pnlTop.Name = "m_pnlTop";
			this.m_pnlTop.Padding = new System.Windows.Forms.Padding(0, 8, 12, 8);
			this.m_pnlTop.Size = new System.Drawing.Size(628, 68);
			this.m_pnlTop.TabIndex = 3;
			// 
			// m_pnlTopLeft
			// 
			this.m_pnlTopLeft.Controls.Add(this.m_lblChars);
			this.m_pnlTopLeft.Controls.Add(this.m_lblIndex);
			this.m_pnlTopLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_pnlTopLeft.Location = new System.Drawing.Point(0, 8);
			this.m_pnlTopLeft.Name = "m_pnlTopLeft";
			this.m_pnlTopLeft.Size = new System.Drawing.Size(71, 52);
			this.m_pnlTopLeft.TabIndex = 0;
			// 
			// m_pnlLeft
			// 
			this.m_pnlLeft.Controls.Add(this.m_lblSelChars);
			this.m_pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_pnlLeft.Location = new System.Drawing.Point(0, 128);
			this.m_pnlLeft.Name = "m_pnlLeft";
			this.m_pnlLeft.Size = new System.Drawing.Size(71, 47);
			this.m_pnlLeft.TabIndex = 4;
			// 
			// m_pnlRight
			// 
			this.m_pnlRight.Controls.Add(this.m_cbHideChars);
			this.m_pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.m_pnlRight.Location = new System.Drawing.Point(579, 128);
			this.m_pnlRight.Name = "m_pnlRight";
			this.m_pnlRight.Size = new System.Drawing.Size(49, 47);
			this.m_pnlRight.TabIndex = 1;
			// 
			// m_pnlMiddleTopSpacer
			// 
			this.m_pnlMiddleTopSpacer.Controls.Add(this.m_tbSelected);
			this.m_pnlMiddleTopSpacer.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_pnlMiddleTopSpacer.Location = new System.Drawing.Point(71, 128);
			this.m_pnlMiddleTopSpacer.Name = "m_pnlMiddleTopSpacer";
			this.m_pnlMiddleTopSpacer.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this.m_pnlMiddleTopSpacer.Size = new System.Drawing.Size(508, 41);
			this.m_pnlMiddleTopSpacer.TabIndex = 0;
			// 
			// CharPickerForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(628, 215);
			this.Controls.Add(this.m_pnlMiddleTopSpacer);
			this.Controls.Add(this.m_pnlRight);
			this.Controls.Add(this.m_pnlLeft);
			this.Controls.Add(this.m_pnlTop);
			this.Controls.Add(this.m_pnlBottom);
			this.Controls.Add(this.m_bannerImage);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CharPickerForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.SizeChanged += new System.EventHandler(this.OnFormSizeChanged);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.Resize += new System.EventHandler(this.OnFormResize);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_pnlBottom.ResumeLayout(false);
			this.m_pnlBottomRight.ResumeLayout(false);
			this.m_pnlTop.ResumeLayout(false);
			this.m_pnlTopLeft.ResumeLayout(false);
			this.m_pnlTopLeft.PerformLayout();
			this.m_pnlLeft.ResumeLayout(false);
			this.m_pnlLeft.PerformLayout();
			this.m_pnlRight.ResumeLayout(false);
			this.m_pnlMiddleTopSpacer.ResumeLayout(false);
			this.m_pnlMiddleTopSpacer.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Label m_lblChars;
		private System.Windows.Forms.Label m_lblIndex;
		private System.Windows.Forms.Label m_lblSelChars;
		private System.Windows.Forms.TextBox m_tbSelected;
		private System.Windows.Forms.CheckBox m_cbHideChars;
		private System.Windows.Forms.Panel m_pnlSelect;
		private System.Windows.Forms.Panel m_pnlBottom;
		private System.Windows.Forms.Panel m_pnlBottomRight;
		private System.Windows.Forms.Panel m_pnlTop;
		private System.Windows.Forms.Panel m_pnlTopLeft;
		private System.Windows.Forms.Panel m_pnlLeft;
		private System.Windows.Forms.Panel m_pnlRight;
		private System.Windows.Forms.Panel m_pnlMiddleTopSpacer;
	}
}