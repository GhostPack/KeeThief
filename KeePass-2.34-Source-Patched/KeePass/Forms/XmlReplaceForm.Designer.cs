namespace KeePass.Forms
{
	partial class XmlReplaceForm
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
			this.m_lblSelNodes = new System.Windows.Forms.Label();
			this.m_tbSelNodes = new System.Windows.Forms.TextBox();
			this.m_lblData = new System.Windows.Forms.Label();
			this.m_rbRemove = new System.Windows.Forms.RadioButton();
			this.m_rbReplace = new System.Windows.Forms.RadioButton();
			this.m_pnlReplace = new System.Windows.Forms.Panel();
			this.m_rbOuterXml = new System.Windows.Forms.RadioButton();
			this.m_cbRegex = new System.Windows.Forms.CheckBox();
			this.m_cbCase = new System.Windows.Forms.CheckBox();
			this.m_tbReplace = new System.Windows.Forms.TextBox();
			this.m_lblReplace = new System.Windows.Forms.Label();
			this.m_tbMatch = new System.Windows.Forms.TextBox();
			this.m_lblMatch = new System.Windows.Forms.Label();
			this.m_rbInnerText = new System.Windows.Forms.RadioButton();
			this.m_rbInnerXml = new System.Windows.Forms.RadioButton();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_lblAction = new System.Windows.Forms.Label();
			this.m_picWarning = new System.Windows.Forms.PictureBox();
			this.m_lblWarning = new System.Windows.Forms.Label();
			this.m_lblSeparator = new System.Windows.Forms.Label();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_pnlReplace.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_picWarning)).BeginInit();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(316, 388);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 7;
			this.m_btnOK.Text = "OK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(397, 388);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 8;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_lblSelNodes
			// 
			this.m_lblSelNodes.AutoSize = true;
			this.m_lblSelNodes.Location = new System.Drawing.Point(9, 98);
			this.m_lblSelNodes.Name = "m_lblSelNodes";
			this.m_lblSelNodes.Size = new System.Drawing.Size(110, 13);
			this.m_lblSelNodes.TabIndex = 10;
			this.m_lblSelNodes.Text = "Select nodes (XPath):";
			// 
			// m_tbSelNodes
			// 
			this.m_tbSelNodes.Location = new System.Drawing.Point(12, 114);
			this.m_tbSelNodes.Name = "m_tbSelNodes";
			this.m_tbSelNodes.Size = new System.Drawing.Size(460, 20);
			this.m_tbSelNodes.TabIndex = 0;
			// 
			// m_lblData
			// 
			this.m_lblData.AutoSize = true;
			this.m_lblData.Location = new System.Drawing.Point(3, 5);
			this.m_lblData.Name = "m_lblData";
			this.m_lblData.Size = new System.Drawing.Size(33, 13);
			this.m_lblData.TabIndex = 0;
			this.m_lblData.Text = "Data:";
			// 
			// m_rbRemove
			// 
			this.m_rbRemove.AutoSize = true;
			this.m_rbRemove.Location = new System.Drawing.Point(12, 164);
			this.m_rbRemove.Name = "m_rbRemove";
			this.m_rbRemove.Size = new System.Drawing.Size(97, 17);
			this.m_rbRemove.TabIndex = 2;
			this.m_rbRemove.TabStop = true;
			this.m_rbRemove.Text = "&Remove nodes";
			this.m_rbRemove.UseVisualStyleBackColor = true;
			this.m_rbRemove.CheckedChanged += new System.EventHandler(this.OnRemoveCheckedChanged);
			// 
			// m_rbReplace
			// 
			this.m_rbReplace.AutoSize = true;
			this.m_rbReplace.Location = new System.Drawing.Point(12, 187);
			this.m_rbReplace.Name = "m_rbReplace";
			this.m_rbReplace.Size = new System.Drawing.Size(89, 17);
			this.m_rbReplace.TabIndex = 3;
			this.m_rbReplace.TabStop = true;
			this.m_rbReplace.Text = "Re&place data";
			this.m_rbReplace.UseVisualStyleBackColor = true;
			// 
			// m_pnlReplace
			// 
			this.m_pnlReplace.Controls.Add(this.m_rbOuterXml);
			this.m_pnlReplace.Controls.Add(this.m_cbRegex);
			this.m_pnlReplace.Controls.Add(this.m_cbCase);
			this.m_pnlReplace.Controls.Add(this.m_tbReplace);
			this.m_pnlReplace.Controls.Add(this.m_lblReplace);
			this.m_pnlReplace.Controls.Add(this.m_tbMatch);
			this.m_pnlReplace.Controls.Add(this.m_lblMatch);
			this.m_pnlReplace.Controls.Add(this.m_rbInnerText);
			this.m_pnlReplace.Controls.Add(this.m_rbInnerXml);
			this.m_pnlReplace.Controls.Add(this.m_lblData);
			this.m_pnlReplace.Location = new System.Drawing.Point(25, 207);
			this.m_pnlReplace.Name = "m_pnlReplace";
			this.m_pnlReplace.Size = new System.Drawing.Size(459, 168);
			this.m_pnlReplace.TabIndex = 4;
			// 
			// m_rbOuterXml
			// 
			this.m_rbOuterXml.AutoSize = true;
			this.m_rbOuterXml.Location = new System.Drawing.Point(197, 3);
			this.m_rbOuterXml.Name = "m_rbOuterXml";
			this.m_rbOuterXml.Size = new System.Drawing.Size(76, 17);
			this.m_rbOuterXml.TabIndex = 3;
			this.m_rbOuterXml.TabStop = true;
			this.m_rbOuterXml.Text = "O&uter XML";
			this.m_rbOuterXml.UseVisualStyleBackColor = true;
			// 
			// m_cbRegex
			// 
			this.m_cbRegex.AutoSize = true;
			this.m_cbRegex.Location = new System.Drawing.Point(6, 143);
			this.m_cbRegex.Name = "m_cbRegex";
			this.m_cbRegex.Size = new System.Drawing.Size(121, 17);
			this.m_cbRegex.TabIndex = 9;
			this.m_cbRegex.Text = "Regular &expressions";
			this.m_cbRegex.UseVisualStyleBackColor = true;
			// 
			// m_cbCase
			// 
			this.m_cbCase.AutoSize = true;
			this.m_cbCase.Location = new System.Drawing.Point(6, 120);
			this.m_cbCase.Name = "m_cbCase";
			this.m_cbCase.Size = new System.Drawing.Size(94, 17);
			this.m_cbCase.TabIndex = 8;
			this.m_cbCase.Text = "Case-&sensitive";
			this.m_cbCase.UseVisualStyleBackColor = true;
			// 
			// m_tbReplace
			// 
			this.m_tbReplace.Location = new System.Drawing.Point(6, 90);
			this.m_tbReplace.Name = "m_tbReplace";
			this.m_tbReplace.Size = new System.Drawing.Size(441, 20);
			this.m_tbReplace.TabIndex = 7;
			// 
			// m_lblReplace
			// 
			this.m_lblReplace.AutoSize = true;
			this.m_lblReplace.Location = new System.Drawing.Point(3, 74);
			this.m_lblReplace.Name = "m_lblReplace";
			this.m_lblReplace.Size = new System.Drawing.Size(72, 13);
			this.m_lblReplace.TabIndex = 6;
			this.m_lblReplace.Text = "Replace with:";
			// 
			// m_tbMatch
			// 
			this.m_tbMatch.Location = new System.Drawing.Point(6, 44);
			this.m_tbMatch.Name = "m_tbMatch";
			this.m_tbMatch.Size = new System.Drawing.Size(441, 20);
			this.m_tbMatch.TabIndex = 5;
			// 
			// m_lblMatch
			// 
			this.m_lblMatch.AutoSize = true;
			this.m_lblMatch.Location = new System.Drawing.Point(3, 28);
			this.m_lblMatch.Name = "m_lblMatch";
			this.m_lblMatch.Size = new System.Drawing.Size(56, 13);
			this.m_lblMatch.TabIndex = 4;
			this.m_lblMatch.Text = "Find what:";
			// 
			// m_rbInnerText
			// 
			this.m_rbInnerText.AutoSize = true;
			this.m_rbInnerText.Location = new System.Drawing.Point(42, 3);
			this.m_rbInnerText.Name = "m_rbInnerText";
			this.m_rbInnerText.Size = new System.Drawing.Size(69, 17);
			this.m_rbInnerText.TabIndex = 1;
			this.m_rbInnerText.TabStop = true;
			this.m_rbInnerText.Text = "Inner &text";
			this.m_rbInnerText.UseVisualStyleBackColor = true;
			// 
			// m_rbInnerXml
			// 
			this.m_rbInnerXml.AutoSize = true;
			this.m_rbInnerXml.Location = new System.Drawing.Point(117, 3);
			this.m_rbInnerXml.Name = "m_rbInnerXml";
			this.m_rbInnerXml.Size = new System.Drawing.Size(74, 17);
			this.m_rbInnerXml.TabIndex = 2;
			this.m_rbInnerXml.TabStop = true;
			this.m_rbInnerXml.Text = "Inner &XML";
			this.m_rbInnerXml.UseVisualStyleBackColor = true;
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(484, 60);
			this.m_bannerImage.TabIndex = 8;
			this.m_bannerImage.TabStop = false;
			// 
			// m_lblAction
			// 
			this.m_lblAction.AutoSize = true;
			this.m_lblAction.Location = new System.Drawing.Point(9, 146);
			this.m_lblAction.Name = "m_lblAction";
			this.m_lblAction.Size = new System.Drawing.Size(40, 13);
			this.m_lblAction.TabIndex = 1;
			this.m_lblAction.Text = "Action:";
			// 
			// m_picWarning
			// 
			this.m_picWarning.Location = new System.Drawing.Point(9, 71);
			this.m_picWarning.Name = "m_picWarning";
			this.m_picWarning.Size = new System.Drawing.Size(16, 16);
			this.m_picWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.m_picWarning.TabIndex = 10;
			this.m_picWarning.TabStop = false;
			// 
			// m_lblWarning
			// 
			this.m_lblWarning.AutoSize = true;
			this.m_lblWarning.Location = new System.Drawing.Point(28, 72);
			this.m_lblWarning.Name = "m_lblWarning";
			this.m_lblWarning.Size = new System.Drawing.Size(267, 13);
			this.m_lblWarning.TabIndex = 9;
			this.m_lblWarning.Text = "XML Replace is a feature for experts. Use with caution!";
			// 
			// m_lblSeparator
			// 
			this.m_lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_lblSeparator.Location = new System.Drawing.Point(0, 380);
			this.m_lblSeparator.Name = "m_lblSeparator";
			this.m_lblSeparator.Size = new System.Drawing.Size(484, 2);
			this.m_lblSeparator.TabIndex = 5;
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Location = new System.Drawing.Point(12, 388);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
			this.m_btnHelp.TabIndex = 6;
			this.m_btnHelp.Text = "&Help";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.OnBtnHelp);
			// 
			// XmlReplaceForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(484, 423);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_lblSeparator);
			this.Controls.Add(this.m_lblWarning);
			this.Controls.Add(this.m_picWarning);
			this.Controls.Add(this.m_lblAction);
			this.Controls.Add(this.m_bannerImage);
			this.Controls.Add(this.m_pnlReplace);
			this.Controls.Add(this.m_rbReplace);
			this.Controls.Add(this.m_rbRemove);
			this.Controls.Add(this.m_tbSelNodes);
			this.Controls.Add(this.m_lblSelNodes);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "XmlReplaceForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<DYN>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.m_pnlReplace.ResumeLayout(false);
			this.m_pnlReplace.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_picWarning)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Label m_lblSelNodes;
		private System.Windows.Forms.TextBox m_tbSelNodes;
		private System.Windows.Forms.Label m_lblData;
		private System.Windows.Forms.RadioButton m_rbRemove;
		private System.Windows.Forms.RadioButton m_rbReplace;
		private System.Windows.Forms.Panel m_pnlReplace;
		private System.Windows.Forms.TextBox m_tbReplace;
		private System.Windows.Forms.Label m_lblReplace;
		private System.Windows.Forms.TextBox m_tbMatch;
		private System.Windows.Forms.Label m_lblMatch;
		private System.Windows.Forms.RadioButton m_rbInnerText;
		private System.Windows.Forms.RadioButton m_rbInnerXml;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Label m_lblAction;
		private System.Windows.Forms.PictureBox m_picWarning;
		private System.Windows.Forms.Label m_lblWarning;
		private System.Windows.Forms.Label m_lblSeparator;
		private System.Windows.Forms.CheckBox m_cbRegex;
		private System.Windows.Forms.CheckBox m_cbCase;
		private System.Windows.Forms.RadioButton m_rbOuterXml;
		private System.Windows.Forms.Button m_btnHelp;
	}
}