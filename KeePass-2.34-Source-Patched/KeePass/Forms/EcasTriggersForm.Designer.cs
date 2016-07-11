namespace KeePass.Forms
{
	partial class EcasTriggersForm
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
			this.components = new System.ComponentModel.Container();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_lvTriggers = new KeePass.UI.CustomListViewEx();
			this.m_btnAdd = new System.Windows.Forms.Button();
			this.m_btnEdit = new System.Windows.Forms.Button();
			this.m_btnDelete = new System.Windows.Forms.Button();
			this.m_lblSep = new System.Windows.Forms.Label();
			this.m_cbEnableTriggers = new System.Windows.Forms.CheckBox();
			this.m_btnMoveUp = new System.Windows.Forms.Button();
			this.m_btnMoveDown = new System.Windows.Forms.Button();
			this.m_btnTools = new System.Windows.Forms.Button();
			this.m_ctxTools = new KeePass.UI.CustomContextMenuStripEx(this.components);
			this.m_ctxToolsHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.m_ctxToolsSep0 = new System.Windows.Forms.ToolStripSeparator();
			this.m_ctxToolsCopyTriggers = new System.Windows.Forms.ToolStripMenuItem();
			this.m_ctxToolsCopySelectedTriggers = new System.Windows.Forms.ToolStripMenuItem();
			this.m_ctxToolsPasteTriggers = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_ctxTools.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(626, 60);
			this.m_bannerImage.TabIndex = 0;
			this.m_bannerImage.TabStop = false;
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(458, 389);
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
			this.m_btnCancel.Location = new System.Drawing.Point(539, 389);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 1;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_lvTriggers
			// 
			this.m_lvTriggers.FullRowSelect = true;
			this.m_lvTriggers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvTriggers.HideSelection = false;
			this.m_lvTriggers.Location = new System.Drawing.Point(12, 96);
			this.m_lvTriggers.Name = "m_lvTriggers";
			this.m_lvTriggers.Size = new System.Drawing.Size(521, 268);
			this.m_lvTriggers.TabIndex = 3;
			this.m_lvTriggers.UseCompatibleStateImageBehavior = false;
			this.m_lvTriggers.View = System.Windows.Forms.View.Details;
			this.m_lvTriggers.ItemActivate += new System.EventHandler(this.OnTriggersItemActivate);
			this.m_lvTriggers.SelectedIndexChanged += new System.EventHandler(this.OnTriggersSelectedIndexChanged);
			// 
			// m_btnAdd
			// 
			this.m_btnAdd.Location = new System.Drawing.Point(539, 95);
			this.m_btnAdd.Name = "m_btnAdd";
			this.m_btnAdd.Size = new System.Drawing.Size(75, 23);
			this.m_btnAdd.TabIndex = 4;
			this.m_btnAdd.Text = "&Add...";
			this.m_btnAdd.UseVisualStyleBackColor = true;
			this.m_btnAdd.Click += new System.EventHandler(this.OnBtnAdd);
			// 
			// m_btnEdit
			// 
			this.m_btnEdit.Location = new System.Drawing.Point(539, 124);
			this.m_btnEdit.Name = "m_btnEdit";
			this.m_btnEdit.Size = new System.Drawing.Size(75, 23);
			this.m_btnEdit.TabIndex = 5;
			this.m_btnEdit.Text = "&Edit...";
			this.m_btnEdit.UseVisualStyleBackColor = true;
			this.m_btnEdit.Click += new System.EventHandler(this.OnBtnEdit);
			// 
			// m_btnDelete
			// 
			this.m_btnDelete.Location = new System.Drawing.Point(539, 153);
			this.m_btnDelete.Name = "m_btnDelete";
			this.m_btnDelete.Size = new System.Drawing.Size(75, 23);
			this.m_btnDelete.TabIndex = 6;
			this.m_btnDelete.Text = "&Delete";
			this.m_btnDelete.UseVisualStyleBackColor = true;
			this.m_btnDelete.Click += new System.EventHandler(this.OnBtnDelete);
			// 
			// m_lblSep
			// 
			this.m_lblSep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSep.Location = new System.Drawing.Point(0, 377);
			this.m_lblSep.Name = "m_lblSep";
			this.m_lblSep.Size = new System.Drawing.Size(626, 2);
			this.m_lblSep.TabIndex = 9;
			// 
			// m_cbEnableTriggers
			// 
			this.m_cbEnableTriggers.AutoSize = true;
			this.m_cbEnableTriggers.Location = new System.Drawing.Point(12, 73);
			this.m_cbEnableTriggers.Name = "m_cbEnableTriggers";
			this.m_cbEnableTriggers.Size = new System.Drawing.Size(126, 17);
			this.m_cbEnableTriggers.TabIndex = 2;
			this.m_cbEnableTriggers.Text = "Enable trigger system";
			this.m_cbEnableTriggers.UseVisualStyleBackColor = true;
			// 
			// m_btnMoveUp
			// 
			this.m_btnMoveUp.Image = global::KeePass.Properties.Resources.B16x16_1UpArrow;
			this.m_btnMoveUp.Location = new System.Drawing.Point(539, 199);
			this.m_btnMoveUp.Name = "m_btnMoveUp";
			this.m_btnMoveUp.Size = new System.Drawing.Size(75, 23);
			this.m_btnMoveUp.TabIndex = 7;
			this.m_btnMoveUp.UseVisualStyleBackColor = true;
			this.m_btnMoveUp.Click += new System.EventHandler(this.OnBtnMoveUp);
			// 
			// m_btnMoveDown
			// 
			this.m_btnMoveDown.Image = global::KeePass.Properties.Resources.B16x16_1DownArrow;
			this.m_btnMoveDown.Location = new System.Drawing.Point(539, 228);
			this.m_btnMoveDown.Name = "m_btnMoveDown";
			this.m_btnMoveDown.Size = new System.Drawing.Size(75, 23);
			this.m_btnMoveDown.TabIndex = 8;
			this.m_btnMoveDown.UseVisualStyleBackColor = true;
			this.m_btnMoveDown.Click += new System.EventHandler(this.OnBtnMoveDown);
			// 
			// m_btnTools
			// 
			this.m_btnTools.Location = new System.Drawing.Point(12, 389);
			this.m_btnTools.Name = "m_btnTools";
			this.m_btnTools.Size = new System.Drawing.Size(75, 23);
			this.m_btnTools.TabIndex = 10;
			this.m_btnTools.Text = "&Tools";
			this.m_btnTools.UseVisualStyleBackColor = true;
			this.m_btnTools.Click += new System.EventHandler(this.OnBtnTools);
			// 
			// m_ctxTools
			// 
			this.m_ctxTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_ctxToolsHelp,
            this.m_ctxToolsSep0,
            this.m_ctxToolsCopyTriggers,
            this.m_ctxToolsCopySelectedTriggers,
            this.m_ctxToolsPasteTriggers});
			this.m_ctxTools.Name = "m_ctxTools";
			this.m_ctxTools.Size = new System.Drawing.Size(270, 120);
			// 
			// m_ctxToolsHelp
			// 
			this.m_ctxToolsHelp.Image = global::KeePass.Properties.Resources.B16x16_Help;
			this.m_ctxToolsHelp.Name = "m_ctxToolsHelp";
			this.m_ctxToolsHelp.Size = new System.Drawing.Size(269, 22);
			this.m_ctxToolsHelp.Text = "&Help";
			this.m_ctxToolsHelp.Click += new System.EventHandler(this.OnCtxToolsHelp);
			// 
			// m_ctxToolsSep0
			// 
			this.m_ctxToolsSep0.Name = "m_ctxToolsSep0";
			this.m_ctxToolsSep0.Size = new System.Drawing.Size(266, 6);
			// 
			// m_ctxToolsCopyTriggers
			// 
			this.m_ctxToolsCopyTriggers.Image = global::KeePass.Properties.Resources.B16x16_EditCopy;
			this.m_ctxToolsCopyTriggers.Name = "m_ctxToolsCopyTriggers";
			this.m_ctxToolsCopyTriggers.Size = new System.Drawing.Size(269, 22);
			this.m_ctxToolsCopyTriggers.Text = "&Copy Triggers to Clipboard";
			this.m_ctxToolsCopyTriggers.Click += new System.EventHandler(this.OnCtxToolsCopyTriggers);
			// 
			// m_ctxToolsCopySelectedTriggers
			// 
			this.m_ctxToolsCopySelectedTriggers.Image = global::KeePass.Properties.Resources.B16x16_EditCopy;
			this.m_ctxToolsCopySelectedTriggers.Name = "m_ctxToolsCopySelectedTriggers";
			this.m_ctxToolsCopySelectedTriggers.Size = new System.Drawing.Size(269, 22);
			this.m_ctxToolsCopySelectedTriggers.Text = "Copy &Selected Triggers to Clipboard";
			this.m_ctxToolsCopySelectedTriggers.Click += new System.EventHandler(this.OnCtxToolsCopySelectedTriggers);
			// 
			// m_ctxToolsPasteTriggers
			// 
			this.m_ctxToolsPasteTriggers.Image = global::KeePass.Properties.Resources.B16x16_EditPaste;
			this.m_ctxToolsPasteTriggers.Name = "m_ctxToolsPasteTriggers";
			this.m_ctxToolsPasteTriggers.Size = new System.Drawing.Size(269, 22);
			this.m_ctxToolsPasteTriggers.Text = "&Paste Triggers from Clipboard";
			this.m_ctxToolsPasteTriggers.Click += new System.EventHandler(this.OnCtxToolsPasteTriggers);
			// 
			// EcasTriggersForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(626, 424);
			this.Controls.Add(this.m_btnTools);
			this.Controls.Add(this.m_btnMoveDown);
			this.Controls.Add(this.m_btnMoveUp);
			this.Controls.Add(this.m_cbEnableTriggers);
			this.Controls.Add(this.m_lblSep);
			this.Controls.Add(this.m_btnDelete);
			this.Controls.Add(this.m_btnEdit);
			this.Controls.Add(this.m_btnAdd);
			this.Controls.Add(this.m_lvTriggers);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_bannerImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EcasTriggersForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_ctxTools.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private KeePass.UI.CustomListViewEx m_lvTriggers;
		private System.Windows.Forms.Button m_btnAdd;
		private System.Windows.Forms.Button m_btnEdit;
		private System.Windows.Forms.Button m_btnDelete;
		private System.Windows.Forms.Label m_lblSep;
		private System.Windows.Forms.CheckBox m_cbEnableTriggers;
		private System.Windows.Forms.Button m_btnMoveUp;
		private System.Windows.Forms.Button m_btnMoveDown;
		private System.Windows.Forms.Button m_btnTools;
		private KeePass.UI.CustomContextMenuStripEx m_ctxTools;
		private System.Windows.Forms.ToolStripMenuItem m_ctxToolsHelp;
		private System.Windows.Forms.ToolStripSeparator m_ctxToolsSep0;
		private System.Windows.Forms.ToolStripMenuItem m_ctxToolsCopyTriggers;
		private System.Windows.Forms.ToolStripMenuItem m_ctxToolsCopySelectedTriggers;
		private System.Windows.Forms.ToolStripMenuItem m_ctxToolsPasteTriggers;
	}
}