namespace KeePass.Forms
{
	partial class FieldRefForm
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
			this.m_lvEntries = new KeePass.UI.CustomListViewEx();
			this.m_grpIdentify = new System.Windows.Forms.GroupBox();
			this.m_radioIdUuid = new System.Windows.Forms.RadioButton();
			this.m_radioIdNotes = new System.Windows.Forms.RadioButton();
			this.m_radioIdUrl = new System.Windows.Forms.RadioButton();
			this.m_radioIdPassword = new System.Windows.Forms.RadioButton();
			this.m_radioIdUserName = new System.Windows.Forms.RadioButton();
			this.m_radioIdTitle = new System.Windows.Forms.RadioButton();
			this.m_grpRefField = new System.Windows.Forms.GroupBox();
			this.m_radioRefNotes = new System.Windows.Forms.RadioButton();
			this.m_radioRefUrl = new System.Windows.Forms.RadioButton();
			this.m_radioRefPassword = new System.Windows.Forms.RadioButton();
			this.m_radioRefUserName = new System.Windows.Forms.RadioButton();
			this.m_radioRefTitle = new System.Windows.Forms.RadioButton();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_lblFilter = new System.Windows.Forms.Label();
			this.m_tbFilter = new System.Windows.Forms.TextBox();
			this.m_grpIdentify.SuspendLayout();
			this.m_grpRefField.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_lvEntries
			// 
			this.m_lvEntries.FullRowSelect = true;
			this.m_lvEntries.GridLines = true;
			this.m_lvEntries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvEntries.HideSelection = false;
			this.m_lvEntries.Location = new System.Drawing.Point(12, 38);
			this.m_lvEntries.MultiSelect = false;
			this.m_lvEntries.Name = "m_lvEntries";
			this.m_lvEntries.Size = new System.Drawing.Size(608, 280);
			this.m_lvEntries.TabIndex = 0;
			this.m_lvEntries.UseCompatibleStateImageBehavior = false;
			this.m_lvEntries.View = System.Windows.Forms.View.Details;
			this.m_lvEntries.SelectedIndexChanged += new System.EventHandler(this.OnEntriesSelectedIndexChanged);
			// 
			// m_grpIdentify
			// 
			this.m_grpIdentify.Controls.Add(this.m_radioIdUuid);
			this.m_grpIdentify.Controls.Add(this.m_radioIdNotes);
			this.m_grpIdentify.Controls.Add(this.m_radioIdUrl);
			this.m_grpIdentify.Controls.Add(this.m_radioIdPassword);
			this.m_grpIdentify.Controls.Add(this.m_radioIdUserName);
			this.m_grpIdentify.Controls.Add(this.m_radioIdTitle);
			this.m_grpIdentify.Location = new System.Drawing.Point(12, 326);
			this.m_grpIdentify.Name = "m_grpIdentify";
			this.m_grpIdentify.Size = new System.Drawing.Size(301, 69);
			this.m_grpIdentify.TabIndex = 1;
			this.m_grpIdentify.TabStop = false;
			this.m_grpIdentify.Text = "Identify source entry by";
			// 
			// m_radioIdUuid
			// 
			this.m_radioIdUuid.AutoSize = true;
			this.m_radioIdUuid.Location = new System.Drawing.Point(202, 42);
			this.m_radioIdUuid.Name = "m_radioIdUuid";
			this.m_radioIdUuid.Size = new System.Drawing.Size(52, 17);
			this.m_radioIdUuid.TabIndex = 5;
			this.m_radioIdUuid.TabStop = true;
			this.m_radioIdUuid.Text = "UUID";
			this.m_radioIdUuid.UseVisualStyleBackColor = true;
			// 
			// m_radioIdNotes
			// 
			this.m_radioIdNotes.AutoSize = true;
			this.m_radioIdNotes.Location = new System.Drawing.Point(202, 19);
			this.m_radioIdNotes.Name = "m_radioIdNotes";
			this.m_radioIdNotes.Size = new System.Drawing.Size(53, 17);
			this.m_radioIdNotes.TabIndex = 4;
			this.m_radioIdNotes.TabStop = true;
			this.m_radioIdNotes.Text = "Notes";
			this.m_radioIdNotes.UseVisualStyleBackColor = true;
			// 
			// m_radioIdUrl
			// 
			this.m_radioIdUrl.AutoSize = true;
			this.m_radioIdUrl.Location = new System.Drawing.Point(106, 42);
			this.m_radioIdUrl.Name = "m_radioIdUrl";
			this.m_radioIdUrl.Size = new System.Drawing.Size(47, 17);
			this.m_radioIdUrl.TabIndex = 3;
			this.m_radioIdUrl.TabStop = true;
			this.m_radioIdUrl.Text = "URL";
			this.m_radioIdUrl.UseVisualStyleBackColor = true;
			// 
			// m_radioIdPassword
			// 
			this.m_radioIdPassword.AutoSize = true;
			this.m_radioIdPassword.Location = new System.Drawing.Point(106, 19);
			this.m_radioIdPassword.Name = "m_radioIdPassword";
			this.m_radioIdPassword.Size = new System.Drawing.Size(71, 17);
			this.m_radioIdPassword.TabIndex = 2;
			this.m_radioIdPassword.TabStop = true;
			this.m_radioIdPassword.Text = "Password";
			this.m_radioIdPassword.UseVisualStyleBackColor = true;
			// 
			// m_radioIdUserName
			// 
			this.m_radioIdUserName.AutoSize = true;
			this.m_radioIdUserName.Location = new System.Drawing.Point(10, 42);
			this.m_radioIdUserName.Name = "m_radioIdUserName";
			this.m_radioIdUserName.Size = new System.Drawing.Size(76, 17);
			this.m_radioIdUserName.TabIndex = 1;
			this.m_radioIdUserName.TabStop = true;
			this.m_radioIdUserName.Text = "User name";
			this.m_radioIdUserName.UseVisualStyleBackColor = true;
			// 
			// m_radioIdTitle
			// 
			this.m_radioIdTitle.AutoSize = true;
			this.m_radioIdTitle.Location = new System.Drawing.Point(10, 19);
			this.m_radioIdTitle.Name = "m_radioIdTitle";
			this.m_radioIdTitle.Size = new System.Drawing.Size(45, 17);
			this.m_radioIdTitle.TabIndex = 0;
			this.m_radioIdTitle.TabStop = true;
			this.m_radioIdTitle.Text = "Title";
			this.m_radioIdTitle.UseVisualStyleBackColor = true;
			// 
			// m_grpRefField
			// 
			this.m_grpRefField.Controls.Add(this.m_radioRefNotes);
			this.m_grpRefField.Controls.Add(this.m_radioRefUrl);
			this.m_grpRefField.Controls.Add(this.m_radioRefPassword);
			this.m_grpRefField.Controls.Add(this.m_radioRefUserName);
			this.m_grpRefField.Controls.Add(this.m_radioRefTitle);
			this.m_grpRefField.Location = new System.Drawing.Point(319, 326);
			this.m_grpRefField.Name = "m_grpRefField";
			this.m_grpRefField.Size = new System.Drawing.Size(301, 69);
			this.m_grpRefField.TabIndex = 2;
			this.m_grpRefField.TabStop = false;
			this.m_grpRefField.Text = "Source field to reference";
			// 
			// m_radioRefNotes
			// 
			this.m_radioRefNotes.AutoSize = true;
			this.m_radioRefNotes.Location = new System.Drawing.Point(203, 19);
			this.m_radioRefNotes.Name = "m_radioRefNotes";
			this.m_radioRefNotes.Size = new System.Drawing.Size(53, 17);
			this.m_radioRefNotes.TabIndex = 4;
			this.m_radioRefNotes.TabStop = true;
			this.m_radioRefNotes.Text = "Notes";
			this.m_radioRefNotes.UseVisualStyleBackColor = true;
			// 
			// m_radioRefUrl
			// 
			this.m_radioRefUrl.AutoSize = true;
			this.m_radioRefUrl.Location = new System.Drawing.Point(107, 42);
			this.m_radioRefUrl.Name = "m_radioRefUrl";
			this.m_radioRefUrl.Size = new System.Drawing.Size(47, 17);
			this.m_radioRefUrl.TabIndex = 3;
			this.m_radioRefUrl.TabStop = true;
			this.m_radioRefUrl.Text = "URL";
			this.m_radioRefUrl.UseVisualStyleBackColor = true;
			// 
			// m_radioRefPassword
			// 
			this.m_radioRefPassword.AutoSize = true;
			this.m_radioRefPassword.Location = new System.Drawing.Point(107, 19);
			this.m_radioRefPassword.Name = "m_radioRefPassword";
			this.m_radioRefPassword.Size = new System.Drawing.Size(71, 17);
			this.m_radioRefPassword.TabIndex = 2;
			this.m_radioRefPassword.TabStop = true;
			this.m_radioRefPassword.Text = "Password";
			this.m_radioRefPassword.UseVisualStyleBackColor = true;
			// 
			// m_radioRefUserName
			// 
			this.m_radioRefUserName.AutoSize = true;
			this.m_radioRefUserName.Location = new System.Drawing.Point(11, 42);
			this.m_radioRefUserName.Name = "m_radioRefUserName";
			this.m_radioRefUserName.Size = new System.Drawing.Size(76, 17);
			this.m_radioRefUserName.TabIndex = 1;
			this.m_radioRefUserName.TabStop = true;
			this.m_radioRefUserName.Text = "User name";
			this.m_radioRefUserName.UseVisualStyleBackColor = true;
			// 
			// m_radioRefTitle
			// 
			this.m_radioRefTitle.AutoSize = true;
			this.m_radioRefTitle.Location = new System.Drawing.Point(11, 19);
			this.m_radioRefTitle.Name = "m_radioRefTitle";
			this.m_radioRefTitle.Size = new System.Drawing.Size(45, 17);
			this.m_radioRefTitle.TabIndex = 0;
			this.m_radioRefTitle.TabStop = true;
			this.m_radioRefTitle.Text = "Title";
			this.m_radioRefTitle.UseVisualStyleBackColor = true;
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(464, 405);
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
			this.m_btnCancel.Location = new System.Drawing.Point(545, 405);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 4;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Location = new System.Drawing.Point(12, 405);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
			this.m_btnHelp.TabIndex = 5;
			this.m_btnHelp.Text = "&Help";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.OnBtnHelp);
			// 
			// m_lblFilter
			// 
			this.m_lblFilter.AutoSize = true;
			this.m_lblFilter.Location = new System.Drawing.Point(396, 15);
			this.m_lblFilter.Name = "m_lblFilter";
			this.m_lblFilter.Size = new System.Drawing.Size(32, 13);
			this.m_lblFilter.TabIndex = 6;
			this.m_lblFilter.Text = "Filter:";
			// 
			// m_tbFilter
			// 
			this.m_tbFilter.Location = new System.Drawing.Point(434, 12);
			this.m_tbFilter.Name = "m_tbFilter";
			this.m_tbFilter.Size = new System.Drawing.Size(186, 20);
			this.m_tbFilter.TabIndex = 7;
			this.m_tbFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFilterKeyDown);
			this.m_tbFilter.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnFilterKeyUp);
			// 
			// FieldRefForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(632, 440);
			this.Controls.Add(this.m_tbFilter);
			this.Controls.Add(this.m_lblFilter);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_grpRefField);
			this.Controls.Add(this.m_grpIdentify);
			this.Controls.Add(this.m_lvEntries);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FieldRefForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Insert Field Reference";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.m_grpIdentify.ResumeLayout(false);
			this.m_grpIdentify.PerformLayout();
			this.m_grpRefField.ResumeLayout(false);
			this.m_grpRefField.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private KeePass.UI.CustomListViewEx m_lvEntries;
		private System.Windows.Forms.GroupBox m_grpIdentify;
		private System.Windows.Forms.GroupBox m_grpRefField;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.RadioButton m_radioIdNotes;
		private System.Windows.Forms.RadioButton m_radioIdUrl;
		private System.Windows.Forms.RadioButton m_radioIdPassword;
		private System.Windows.Forms.RadioButton m_radioIdUserName;
		private System.Windows.Forms.RadioButton m_radioIdTitle;
		private System.Windows.Forms.RadioButton m_radioIdUuid;
		private System.Windows.Forms.RadioButton m_radioRefNotes;
		private System.Windows.Forms.RadioButton m_radioRefUrl;
		private System.Windows.Forms.RadioButton m_radioRefPassword;
		private System.Windows.Forms.RadioButton m_radioRefUserName;
		private System.Windows.Forms.RadioButton m_radioRefTitle;
		private System.Windows.Forms.Label m_lblFilter;
		private System.Windows.Forms.TextBox m_tbFilter;
	}
}