namespace KeePass.Forms
{
	partial class StatusLoggerForm
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
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_lvMessages = new KeePass.UI.CustomListViewEx();
			this.m_pbProgress = new System.Windows.Forms.ProgressBar();
			this.m_tbDetails = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(392, 276);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 0;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_lvMessages
			// 
			this.m_lvMessages.FullRowSelect = true;
			this.m_lvMessages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.m_lvMessages.Location = new System.Drawing.Point(12, 12);
			this.m_lvMessages.MultiSelect = false;
			this.m_lvMessages.Name = "m_lvMessages";
			this.m_lvMessages.Size = new System.Drawing.Size(455, 134);
			this.m_lvMessages.TabIndex = 1;
			this.m_lvMessages.UseCompatibleStateImageBehavior = false;
			this.m_lvMessages.View = System.Windows.Forms.View.Details;
			this.m_lvMessages.SelectedIndexChanged += new System.EventHandler(this.OnMessagesSelectedIndexChanged);
			// 
			// m_pbProgress
			// 
			this.m_pbProgress.Location = new System.Drawing.Point(12, 152);
			this.m_pbProgress.Name = "m_pbProgress";
			this.m_pbProgress.Size = new System.Drawing.Size(455, 17);
			this.m_pbProgress.TabIndex = 2;
			// 
			// m_tbDetails
			// 
			this.m_tbDetails.Location = new System.Drawing.Point(12, 175);
			this.m_tbDetails.Multiline = true;
			this.m_tbDetails.Name = "m_tbDetails";
			this.m_tbDetails.ReadOnly = true;
			this.m_tbDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbDetails.Size = new System.Drawing.Size(455, 95);
			this.m_tbDetails.TabIndex = 3;
			// 
			// StatusLoggerForm
			// 
			this.AcceptButton = this.m_btnCancel;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(479, 311);
			this.Controls.Add(this.m_tbDetails);
			this.Controls.Add(this.m_pbProgress);
			this.Controls.Add(this.m_lvMessages);
			this.Controls.Add(this.m_btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StatusLoggerForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_btnCancel;
		private KeePass.UI.CustomListViewEx m_lvMessages;
		private System.Windows.Forms.ProgressBar m_pbProgress;
		private System.Windows.Forms.TextBox m_tbDetails;
	}
}