namespace KeePass.Forms
{
	partial class UrlOverridesForm
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
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_lvOverrides = new KeePass.UI.CustomListViewEx();
			this.m_btnAdd = new System.Windows.Forms.Button();
			this.m_btnEdit = new System.Windows.Forms.Button();
			this.m_btnDelete = new System.Windows.Forms.Button();
			this.m_lblSep = new System.Windows.Forms.Label();
			this.m_cbOverrideAll = new System.Windows.Forms.CheckBox();
			this.m_tbOverrideAll = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(320, 324);
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
			this.m_btnCancel.Location = new System.Drawing.Point(401, 324);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 1;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_lvOverrides
			// 
			this.m_lvOverrides.CheckBoxes = true;
			this.m_lvOverrides.FullRowSelect = true;
			this.m_lvOverrides.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvOverrides.Location = new System.Drawing.Point(12, 12);
			this.m_lvOverrides.Name = "m_lvOverrides";
			this.m_lvOverrides.ShowItemToolTips = true;
			this.m_lvOverrides.Size = new System.Drawing.Size(383, 256);
			this.m_lvOverrides.TabIndex = 2;
			this.m_lvOverrides.UseCompatibleStateImageBehavior = false;
			this.m_lvOverrides.View = System.Windows.Forms.View.Details;
			this.m_lvOverrides.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnOverridesItemChecked);
			this.m_lvOverrides.SelectedIndexChanged += new System.EventHandler(this.OnOverridesSelectedIndexChanged);
			// 
			// m_btnAdd
			// 
			this.m_btnAdd.Location = new System.Drawing.Point(401, 12);
			this.m_btnAdd.Name = "m_btnAdd";
			this.m_btnAdd.Size = new System.Drawing.Size(75, 23);
			this.m_btnAdd.TabIndex = 3;
			this.m_btnAdd.Text = "&Add...";
			this.m_btnAdd.UseVisualStyleBackColor = true;
			this.m_btnAdd.Click += new System.EventHandler(this.OnBtnAdd);
			// 
			// m_btnEdit
			// 
			this.m_btnEdit.Location = new System.Drawing.Point(401, 41);
			this.m_btnEdit.Name = "m_btnEdit";
			this.m_btnEdit.Size = new System.Drawing.Size(75, 23);
			this.m_btnEdit.TabIndex = 4;
			this.m_btnEdit.Text = "&Edit...";
			this.m_btnEdit.UseVisualStyleBackColor = true;
			this.m_btnEdit.Click += new System.EventHandler(this.OnBtnEdit);
			// 
			// m_btnDelete
			// 
			this.m_btnDelete.Location = new System.Drawing.Point(401, 70);
			this.m_btnDelete.Name = "m_btnDelete";
			this.m_btnDelete.Size = new System.Drawing.Size(75, 23);
			this.m_btnDelete.TabIndex = 5;
			this.m_btnDelete.Text = "&Delete";
			this.m_btnDelete.UseVisualStyleBackColor = true;
			this.m_btnDelete.Click += new System.EventHandler(this.OnBtnDelete);
			// 
			// m_lblSep
			// 
			this.m_lblSep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSep.Location = new System.Drawing.Point(0, 315);
			this.m_lblSep.Name = "m_lblSep";
			this.m_lblSep.Size = new System.Drawing.Size(489, 2);
			this.m_lblSep.TabIndex = 8;
			// 
			// m_cbOverrideAll
			// 
			this.m_cbOverrideAll.AutoSize = true;
			this.m_cbOverrideAll.Location = new System.Drawing.Point(12, 283);
			this.m_cbOverrideAll.Name = "m_cbOverrideAll";
			this.m_cbOverrideAll.Size = new System.Drawing.Size(138, 17);
			this.m_cbOverrideAll.TabIndex = 6;
			this.m_cbOverrideAll.Text = "Override all entry URLs:";
			this.m_cbOverrideAll.UseVisualStyleBackColor = true;
			this.m_cbOverrideAll.CheckedChanged += new System.EventHandler(this.OnOverrideAllCheckedChanged);
			// 
			// m_tbOverrideAll
			// 
			this.m_tbOverrideAll.Location = new System.Drawing.Point(156, 281);
			this.m_tbOverrideAll.Name = "m_tbOverrideAll";
			this.m_tbOverrideAll.Size = new System.Drawing.Size(239, 20);
			this.m_tbOverrideAll.TabIndex = 7;
			// 
			// UrlOverridesForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(488, 359);
			this.Controls.Add(this.m_tbOverrideAll);
			this.Controls.Add(this.m_cbOverrideAll);
			this.Controls.Add(this.m_lblSep);
			this.Controls.Add(this.m_btnDelete);
			this.Controls.Add(this.m_btnEdit);
			this.Controls.Add(this.m_btnAdd);
			this.Controls.Add(this.m_lvOverrides);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UrlOverridesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private KeePass.UI.CustomListViewEx m_lvOverrides;
		private System.Windows.Forms.Button m_btnAdd;
		private System.Windows.Forms.Button m_btnEdit;
		private System.Windows.Forms.Button m_btnDelete;
		private System.Windows.Forms.Label m_lblSep;
		private System.Windows.Forms.CheckBox m_cbOverrideAll;
		private System.Windows.Forms.TextBox m_tbOverrideAll;
	}
}