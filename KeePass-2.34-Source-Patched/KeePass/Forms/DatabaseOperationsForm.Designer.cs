namespace KeePass.Forms
{
	partial class DatabaseOperationsForm
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
			this.m_btnClose = new System.Windows.Forms.Button();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_grpHistoryDelete = new System.Windows.Forms.GroupBox();
			this.m_lblEntryHistoryWarning = new System.Windows.Forms.Label();
			this.m_btnHistoryEntriesDelete = new System.Windows.Forms.Button();
			this.m_numHistoryDays = new System.Windows.Forms.NumericUpDown();
			this.m_lblHistoryEntriesDays = new System.Windows.Forms.Label();
			this.m_lblDeleteHistoryEntries = new System.Windows.Forms.Label();
			this.m_lblTrashIcon = new System.Windows.Forms.Label();
			this.m_tabMain = new System.Windows.Forms.TabControl();
			this.m_tabCleanUp = new System.Windows.Forms.TabPage();
			this.m_grpDeletedObjectsInfo = new System.Windows.Forms.GroupBox();
			this.m_lblDelObjInfoWarning = new System.Windows.Forms.Label();
			this.m_btnRemoveDelObjInfo = new System.Windows.Forms.Button();
			this.m_lblTrashIcon2 = new System.Windows.Forms.Label();
			this.m_lblDelObjInfoIntro = new System.Windows.Forms.Label();
			this.m_pbStatus = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_grpHistoryDelete.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numHistoryDays)).BeginInit();
			this.m_tabMain.SuspendLayout();
			this.m_tabCleanUp.SuspendLayout();
			this.m_grpDeletedObjectsInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnClose
			// 
			this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnClose.Location = new System.Drawing.Point(376, 354);
			this.m_btnClose.Name = "m_btnClose";
			this.m_btnClose.Size = new System.Drawing.Size(75, 23);
			this.m_btnClose.TabIndex = 0;
			this.m_btnClose.Text = "&Close";
			this.m_btnClose.UseVisualStyleBackColor = true;
			this.m_btnClose.Click += new System.EventHandler(this.OnBtnClose);
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(463, 60);
			this.m_bannerImage.TabIndex = 1;
			this.m_bannerImage.TabStop = false;
			// 
			// m_grpHistoryDelete
			// 
			this.m_grpHistoryDelete.Controls.Add(this.m_lblEntryHistoryWarning);
			this.m_grpHistoryDelete.Controls.Add(this.m_btnHistoryEntriesDelete);
			this.m_grpHistoryDelete.Controls.Add(this.m_numHistoryDays);
			this.m_grpHistoryDelete.Controls.Add(this.m_lblHistoryEntriesDays);
			this.m_grpHistoryDelete.Controls.Add(this.m_lblDeleteHistoryEntries);
			this.m_grpHistoryDelete.Controls.Add(this.m_lblTrashIcon);
			this.m_grpHistoryDelete.Location = new System.Drawing.Point(6, 6);
			this.m_grpHistoryDelete.Name = "m_grpHistoryDelete";
			this.m_grpHistoryDelete.Size = new System.Drawing.Size(417, 118);
			this.m_grpHistoryDelete.TabIndex = 0;
			this.m_grpHistoryDelete.TabStop = false;
			this.m_grpHistoryDelete.Text = "Entry history";
			// 
			// m_lblEntryHistoryWarning
			// 
			this.m_lblEntryHistoryWarning.Location = new System.Drawing.Point(44, 81);
			this.m_lblEntryHistoryWarning.Name = "m_lblEntryHistoryWarning";
			this.m_lblEntryHistoryWarning.Size = new System.Drawing.Size(367, 29);
			this.m_lblEntryHistoryWarning.TabIndex = 5;
			this.m_lblEntryHistoryWarning.Text = "Clicking the \'Delete\' button will remove all history entries older than the speci" +
				"fied number of days. There\'s no way to get them back.";
			// 
			// m_btnHistoryEntriesDelete
			// 
			this.m_btnHistoryEntriesDelete.Location = new System.Drawing.Point(335, 52);
			this.m_btnHistoryEntriesDelete.Name = "m_btnHistoryEntriesDelete";
			this.m_btnHistoryEntriesDelete.Size = new System.Drawing.Size(75, 23);
			this.m_btnHistoryEntriesDelete.TabIndex = 4;
			this.m_btnHistoryEntriesDelete.Text = "&Delete";
			this.m_btnHistoryEntriesDelete.UseVisualStyleBackColor = true;
			this.m_btnHistoryEntriesDelete.Click += new System.EventHandler(this.OnBtnDelete);
			// 
			// m_numHistoryDays
			// 
			this.m_numHistoryDays.Location = new System.Drawing.Point(239, 54);
			this.m_numHistoryDays.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
			this.m_numHistoryDays.Name = "m_numHistoryDays";
			this.m_numHistoryDays.Size = new System.Drawing.Size(59, 20);
			this.m_numHistoryDays.TabIndex = 3;
			this.m_numHistoryDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.m_numHistoryDays.Value = new decimal(new int[] {
            365,
            0,
            0,
            0});
			// 
			// m_lblHistoryEntriesDays
			// 
			this.m_lblHistoryEntriesDays.AutoSize = true;
			this.m_lblHistoryEntriesDays.Location = new System.Drawing.Point(44, 56);
			this.m_lblHistoryEntriesDays.Name = "m_lblHistoryEntriesDays";
			this.m_lblHistoryEntriesDays.Size = new System.Drawing.Size(189, 13);
			this.m_lblHistoryEntriesDays.TabIndex = 2;
			this.m_lblHistoryEntriesDays.Text = "Delete history entries older than (days):";
			// 
			// m_lblDeleteHistoryEntries
			// 
			this.m_lblDeleteHistoryEntries.Location = new System.Drawing.Point(44, 17);
			this.m_lblDeleteHistoryEntries.Name = "m_lblDeleteHistoryEntries";
			this.m_lblDeleteHistoryEntries.Size = new System.Drawing.Size(367, 32);
			this.m_lblDeleteHistoryEntries.TabIndex = 1;
			this.m_lblDeleteHistoryEntries.Text = "Old history entries (items shown on the \'History\' tab page in the entries dialog)" +
				" can be deleted. This will decrease the database size a bit.";
			// 
			// m_lblTrashIcon
			// 
			this.m_lblTrashIcon.Image = global::KeePass.Properties.Resources.B32x32_Trashcan_Full;
			this.m_lblTrashIcon.Location = new System.Drawing.Point(6, 16);
			this.m_lblTrashIcon.Name = "m_lblTrashIcon";
			this.m_lblTrashIcon.Size = new System.Drawing.Size(32, 32);
			this.m_lblTrashIcon.TabIndex = 0;
			// 
			// m_tabMain
			// 
			this.m_tabMain.Controls.Add(this.m_tabCleanUp);
			this.m_tabMain.Location = new System.Drawing.Point(12, 66);
			this.m_tabMain.Name = "m_tabMain";
			this.m_tabMain.SelectedIndex = 0;
			this.m_tabMain.Size = new System.Drawing.Size(439, 276);
			this.m_tabMain.TabIndex = 1;
			// 
			// m_tabCleanUp
			// 
			this.m_tabCleanUp.Controls.Add(this.m_grpDeletedObjectsInfo);
			this.m_tabCleanUp.Controls.Add(this.m_grpHistoryDelete);
			this.m_tabCleanUp.Location = new System.Drawing.Point(4, 22);
			this.m_tabCleanUp.Name = "m_tabCleanUp";
			this.m_tabCleanUp.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabCleanUp.Size = new System.Drawing.Size(431, 250);
			this.m_tabCleanUp.TabIndex = 0;
			this.m_tabCleanUp.Text = "Clean Up";
			this.m_tabCleanUp.UseVisualStyleBackColor = true;
			// 
			// m_grpDeletedObjectsInfo
			// 
			this.m_grpDeletedObjectsInfo.Controls.Add(this.m_lblDelObjInfoWarning);
			this.m_grpDeletedObjectsInfo.Controls.Add(this.m_btnRemoveDelObjInfo);
			this.m_grpDeletedObjectsInfo.Controls.Add(this.m_lblTrashIcon2);
			this.m_grpDeletedObjectsInfo.Controls.Add(this.m_lblDelObjInfoIntro);
			this.m_grpDeletedObjectsInfo.Location = new System.Drawing.Point(6, 130);
			this.m_grpDeletedObjectsInfo.Name = "m_grpDeletedObjectsInfo";
			this.m_grpDeletedObjectsInfo.Size = new System.Drawing.Size(417, 114);
			this.m_grpDeletedObjectsInfo.TabIndex = 1;
			this.m_grpDeletedObjectsInfo.TabStop = false;
			this.m_grpDeletedObjectsInfo.Text = "Deleted objects information";
			// 
			// m_lblDelObjInfoWarning
			// 
			this.m_lblDelObjInfoWarning.Location = new System.Drawing.Point(44, 67);
			this.m_lblDelObjInfoWarning.Name = "m_lblDelObjInfoWarning";
			this.m_lblDelObjInfoWarning.Size = new System.Drawing.Size(367, 41);
			this.m_lblDelObjInfoWarning.TabIndex = 3;
			this.m_lblDelObjInfoWarning.Text = "Warning! After removing this information, deleted objects (groups, entries, ...) " +
				"may reappear when synchronizing the current database with another one (which sti" +
				"ll contains the objects).";
			// 
			// m_btnRemoveDelObjInfo
			// 
			this.m_btnRemoveDelObjInfo.Location = new System.Drawing.Point(335, 16);
			this.m_btnRemoveDelObjInfo.Name = "m_btnRemoveDelObjInfo";
			this.m_btnRemoveDelObjInfo.Size = new System.Drawing.Size(75, 23);
			this.m_btnRemoveDelObjInfo.TabIndex = 2;
			this.m_btnRemoveDelObjInfo.Text = "D&elete";
			this.m_btnRemoveDelObjInfo.UseVisualStyleBackColor = true;
			this.m_btnRemoveDelObjInfo.Click += new System.EventHandler(this.OnBtnRemoveDelObjInfo);
			// 
			// m_lblTrashIcon2
			// 
			this.m_lblTrashIcon2.Image = global::KeePass.Properties.Resources.B32x32_Trashcan_Full;
			this.m_lblTrashIcon2.Location = new System.Drawing.Point(6, 16);
			this.m_lblTrashIcon2.Name = "m_lblTrashIcon2";
			this.m_lblTrashIcon2.Size = new System.Drawing.Size(32, 32);
			this.m_lblTrashIcon2.TabIndex = 0;
			// 
			// m_lblDelObjInfoIntro
			// 
			this.m_lblDelObjInfoIntro.Location = new System.Drawing.Point(44, 17);
			this.m_lblDelObjInfoIntro.Name = "m_lblDelObjInfoIntro";
			this.m_lblDelObjInfoIntro.Size = new System.Drawing.Size(285, 41);
			this.m_lblDelObjInfoIntro.TabIndex = 1;
			this.m_lblDelObjInfoIntro.Text = "KeePass keeps some information about deleted objects. This information can be rem" +
				"oved in order to reduce the size of the database.";
			// 
			// m_pbStatus
			// 
			this.m_pbStatus.Location = new System.Drawing.Point(12, 359);
			this.m_pbStatus.Name = "m_pbStatus";
			this.m_pbStatus.Size = new System.Drawing.Size(341, 13);
			this.m_pbStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.m_pbStatus.TabIndex = 2;
			// 
			// DatabaseOperationsForm
			// 
			this.AcceptButton = this.m_btnClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnClose;
			this.ClientSize = new System.Drawing.Size(463, 389);
			this.Controls.Add(this.m_pbStatus);
			this.Controls.Add(this.m_tabMain);
			this.Controls.Add(this.m_bannerImage);
			this.Controls.Add(this.m_btnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DatabaseOperationsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<DYN>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_grpHistoryDelete.ResumeLayout(false);
			this.m_grpHistoryDelete.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_numHistoryDays)).EndInit();
			this.m_tabMain.ResumeLayout(false);
			this.m_tabCleanUp.ResumeLayout(false);
			this.m_grpDeletedObjectsInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnClose;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.GroupBox m_grpHistoryDelete;
		private System.Windows.Forms.Label m_lblTrashIcon;
		private System.Windows.Forms.NumericUpDown m_numHistoryDays;
		private System.Windows.Forms.Label m_lblHistoryEntriesDays;
		private System.Windows.Forms.Label m_lblDeleteHistoryEntries;
		private System.Windows.Forms.Button m_btnHistoryEntriesDelete;
		private System.Windows.Forms.TabControl m_tabMain;
		private System.Windows.Forms.TabPage m_tabCleanUp;
		private System.Windows.Forms.Label m_lblEntryHistoryWarning;
		private System.Windows.Forms.ProgressBar m_pbStatus;
		private System.Windows.Forms.GroupBox m_grpDeletedObjectsInfo;
		private System.Windows.Forms.Label m_lblTrashIcon2;
		private System.Windows.Forms.Label m_lblDelObjInfoIntro;
		private System.Windows.Forms.Label m_lblDelObjInfoWarning;
		private System.Windows.Forms.Button m_btnRemoveDelObjInfo;
	}
}