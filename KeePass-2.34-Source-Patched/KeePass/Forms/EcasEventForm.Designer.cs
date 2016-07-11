namespace KeePass.Forms
{
	partial class EcasEventForm
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
			this.m_lblEvent = new System.Windows.Forms.Label();
			this.m_cmbEvents = new System.Windows.Forms.ComboBox();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_dgvParams = new System.Windows.Forms.DataGridView();
			this.m_lblParamHint = new System.Windows.Forms.Label();
			this.m_lblSep = new System.Windows.Forms.Label();
			this.m_btnHelp = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.m_dgvParams)).BeginInit();
			this.SuspendLayout();
			// 
			// m_lblEvent
			// 
			this.m_lblEvent.AutoSize = true;
			this.m_lblEvent.Location = new System.Drawing.Point(9, 14);
			this.m_lblEvent.Name = "m_lblEvent";
			this.m_lblEvent.Size = new System.Drawing.Size(38, 13);
			this.m_lblEvent.TabIndex = 7;
			this.m_lblEvent.Text = "Event:";
			// 
			// m_cmbEvents
			// 
			this.m_cmbEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cmbEvents.FormattingEnabled = true;
			this.m_cmbEvents.Location = new System.Drawing.Point(12, 30);
			this.m_cmbEvents.Name = "m_cmbEvents";
			this.m_cmbEvents.Size = new System.Drawing.Size(510, 21);
			this.m_cmbEvents.TabIndex = 0;
			this.m_cmbEvents.SelectedIndexChanged += new System.EventHandler(this.OnEventsSelectedIndexChanged);
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(366, 289);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 5;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(447, 289);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 6;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_dgvParams
			// 
			this.m_dgvParams.AllowUserToAddRows = false;
			this.m_dgvParams.AllowUserToDeleteRows = false;
			this.m_dgvParams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_dgvParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_dgvParams.Location = new System.Drawing.Point(12, 57);
			this.m_dgvParams.Name = "m_dgvParams";
			this.m_dgvParams.Size = new System.Drawing.Size(510, 180);
			this.m_dgvParams.TabIndex = 1;
			// 
			// m_lblParamHint
			// 
			this.m_lblParamHint.Location = new System.Drawing.Point(9, 249);
			this.m_lblParamHint.Name = "m_lblParamHint";
			this.m_lblParamHint.Size = new System.Drawing.Size(513, 15);
			this.m_lblParamHint.TabIndex = 2;
			this.m_lblParamHint.Text = "<>";
			// 
			// m_lblSep
			// 
			this.m_lblSep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSep.Location = new System.Drawing.Point(0, 280);
			this.m_lblSep.Name = "m_lblSep";
			this.m_lblSep.Size = new System.Drawing.Size(535, 2);
			this.m_lblSep.TabIndex = 3;
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Location = new System.Drawing.Point(12, 289);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
			this.m_btnHelp.TabIndex = 4;
			this.m_btnHelp.Text = "&Help";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.OnBtnHelp);
			// 
			// EcasEventForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(534, 324);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_lblSep);
			this.Controls.Add(this.m_lblParamHint);
			this.Controls.Add(this.m_dgvParams);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_lblEvent);
			this.Controls.Add(this.m_cmbEvents);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EcasEventForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_dgvParams)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label m_lblEvent;
		private System.Windows.Forms.ComboBox m_cmbEvents;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.DataGridView m_dgvParams;
		private System.Windows.Forms.Label m_lblParamHint;
		private System.Windows.Forms.Label m_lblSep;
		private System.Windows.Forms.Button m_btnHelp;
	}
}