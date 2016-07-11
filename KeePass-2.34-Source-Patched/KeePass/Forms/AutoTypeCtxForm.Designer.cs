namespace KeePass.Forms
{
	partial class AutoTypeCtxForm
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
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_lblText = new System.Windows.Forms.Label();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_lvItems = new KeePass.UI.CustomListViewEx();
			this.m_pnlTop = new System.Windows.Forms.Panel();
			this.m_pnlBottom = new System.Windows.Forms.Panel();
			this.m_btnTools = new System.Windows.Forms.Button();
			this.m_pnlMiddle = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_pnlTop.SuspendLayout();
			this.m_pnlBottom.SuspendLayout();
			this.m_pnlMiddle.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(579, 60);
			this.m_bannerImage.TabIndex = 0;
			this.m_bannerImage.TabStop = false;
			// 
			// m_lblText
			// 
			this.m_lblText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_lblText.Location = new System.Drawing.Point(9, 11);
			this.m_lblText.Name = "m_lblText";
			this.m_lblText.Size = new System.Drawing.Size(561, 30);
			this.m_lblText.TabIndex = 0;
			this.m_lblText.Text = "<>";
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.m_btnCancel.Location = new System.Drawing.Point(492, 6);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 0;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_lvItems
			// 
			this.m_lvItems.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.m_lvItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_lvItems.FullRowSelect = true;
			this.m_lvItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvItems.HideSelection = false;
			this.m_lvItems.HotTracking = true;
			this.m_lvItems.HoverSelection = true;
			this.m_lvItems.Location = new System.Drawing.Point(12, 0);
			this.m_lvItems.MultiSelect = false;
			this.m_lvItems.Name = "m_lvItems";
			this.m_lvItems.ShowItemToolTips = true;
			this.m_lvItems.Size = new System.Drawing.Size(555, 219);
			this.m_lvItems.TabIndex = 0;
			this.m_lvItems.UseCompatibleStateImageBehavior = false;
			this.m_lvItems.View = System.Windows.Forms.View.Details;
			this.m_lvItems.ItemActivate += new System.EventHandler(this.OnListItemActivate);
			this.m_lvItems.Click += new System.EventHandler(this.OnListClick);
			// 
			// m_pnlTop
			// 
			this.m_pnlTop.Controls.Add(this.m_lblText);
			this.m_pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_pnlTop.Location = new System.Drawing.Point(0, 60);
			this.m_pnlTop.Name = "m_pnlTop";
			this.m_pnlTop.Padding = new System.Windows.Forms.Padding(9, 11, 9, 3);
			this.m_pnlTop.Size = new System.Drawing.Size(579, 44);
			this.m_pnlTop.TabIndex = 1;
			// 
			// m_pnlBottom
			// 
			this.m_pnlBottom.Controls.Add(this.m_btnTools);
			this.m_pnlBottom.Controls.Add(this.m_btnCancel);
			this.m_pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_pnlBottom.Location = new System.Drawing.Point(0, 323);
			this.m_pnlBottom.Name = "m_pnlBottom";
			this.m_pnlBottom.Padding = new System.Windows.Forms.Padding(12, 6, 12, 12);
			this.m_pnlBottom.Size = new System.Drawing.Size(579, 41);
			this.m_pnlBottom.TabIndex = 2;
			// 
			// m_btnTools
			// 
			this.m_btnTools.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_btnTools.Location = new System.Drawing.Point(12, 6);
			this.m_btnTools.Name = "m_btnTools";
			this.m_btnTools.Size = new System.Drawing.Size(75, 23);
			this.m_btnTools.TabIndex = 1;
			this.m_btnTools.Text = "&Options";
			this.m_btnTools.UseVisualStyleBackColor = true;
			this.m_btnTools.Click += new System.EventHandler(this.OnBtnTools);
			// 
			// m_pnlMiddle
			// 
			this.m_pnlMiddle.Controls.Add(this.m_lvItems);
			this.m_pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_pnlMiddle.Location = new System.Drawing.Point(0, 104);
			this.m_pnlMiddle.Name = "m_pnlMiddle";
			this.m_pnlMiddle.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
			this.m_pnlMiddle.Size = new System.Drawing.Size(579, 219);
			this.m_pnlMiddle.TabIndex = 0;
			// 
			// AutoTypeCtxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(579, 364);
			this.Controls.Add(this.m_pnlMiddle);
			this.Controls.Add(this.m_pnlBottom);
			this.Controls.Add(this.m_pnlTop);
			this.Controls.Add(this.m_bannerImage);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AutoTypeCtxForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.Resize += new System.EventHandler(this.OnFormResize);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_pnlTop.ResumeLayout(false);
			this.m_pnlBottom.ResumeLayout(false);
			this.m_pnlMiddle.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Label m_lblText;
		private KeePass.UI.CustomListViewEx m_lvItems;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Panel m_pnlTop;
		private System.Windows.Forms.Panel m_pnlBottom;
		private System.Windows.Forms.Panel m_pnlMiddle;
		private System.Windows.Forms.Button m_btnTools;
	}
}