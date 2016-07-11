namespace KeePass.Forms
{
	partial class ProxyForm
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
			this.m_grpServer = new System.Windows.Forms.GroupBox();
			this.m_tbPort = new System.Windows.Forms.TextBox();
			this.m_lblPort = new System.Windows.Forms.Label();
			this.m_tbAddress = new System.Windows.Forms.TextBox();
			this.m_lblAddress = new System.Windows.Forms.Label();
			this.m_rbManualProxy = new System.Windows.Forms.RadioButton();
			this.m_rbSystemProxy = new System.Windows.Forms.RadioButton();
			this.m_rbNoProxy = new System.Windows.Forms.RadioButton();
			this.m_grpAuth = new System.Windows.Forms.GroupBox();
			this.m_rbAuthManual = new System.Windows.Forms.RadioButton();
			this.m_rbAuthDefault = new System.Windows.Forms.RadioButton();
			this.m_rbAuthNone = new System.Windows.Forms.RadioButton();
			this.m_tbPassword = new System.Windows.Forms.TextBox();
			this.m_lblPassword = new System.Windows.Forms.Label();
			this.m_tbUser = new System.Windows.Forms.TextBox();
			this.m_lblUser = new System.Windows.Forms.Label();
			this.m_grpServer.SuspendLayout();
			this.m_grpAuth.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(183, 297);
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
			this.m_btnCancel.Location = new System.Drawing.Point(264, 297);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 1;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			// 
			// m_grpServer
			// 
			this.m_grpServer.Controls.Add(this.m_tbPort);
			this.m_grpServer.Controls.Add(this.m_lblPort);
			this.m_grpServer.Controls.Add(this.m_tbAddress);
			this.m_grpServer.Controls.Add(this.m_lblAddress);
			this.m_grpServer.Controls.Add(this.m_rbManualProxy);
			this.m_grpServer.Controls.Add(this.m_rbSystemProxy);
			this.m_grpServer.Controls.Add(this.m_rbNoProxy);
			this.m_grpServer.Location = new System.Drawing.Point(12, 12);
			this.m_grpServer.Name = "m_grpServer";
			this.m_grpServer.Size = new System.Drawing.Size(327, 120);
			this.m_grpServer.TabIndex = 2;
			this.m_grpServer.TabStop = false;
			this.m_grpServer.Text = "Server";
			// 
			// m_tbPort
			// 
			this.m_tbPort.Location = new System.Drawing.Point(266, 88);
			this.m_tbPort.Name = "m_tbPort";
			this.m_tbPort.Size = new System.Drawing.Size(50, 20);
			this.m_tbPort.TabIndex = 6;
			// 
			// m_lblPort
			// 
			this.m_lblPort.AutoSize = true;
			this.m_lblPort.Location = new System.Drawing.Point(231, 91);
			this.m_lblPort.Name = "m_lblPort";
			this.m_lblPort.Size = new System.Drawing.Size(29, 13);
			this.m_lblPort.TabIndex = 5;
			this.m_lblPort.Text = "Port:";
			// 
			// m_tbAddress
			// 
			this.m_tbAddress.Location = new System.Drawing.Point(79, 88);
			this.m_tbAddress.Name = "m_tbAddress";
			this.m_tbAddress.Size = new System.Drawing.Size(145, 20);
			this.m_tbAddress.TabIndex = 4;
			this.m_tbAddress.TextChanged += new System.EventHandler(this.OnAddressTextChanged);
			// 
			// m_lblAddress
			// 
			this.m_lblAddress.AutoSize = true;
			this.m_lblAddress.Location = new System.Drawing.Point(25, 91);
			this.m_lblAddress.Name = "m_lblAddress";
			this.m_lblAddress.Size = new System.Drawing.Size(48, 13);
			this.m_lblAddress.TabIndex = 3;
			this.m_lblAddress.Text = "Address:";
			// 
			// m_rbManualProxy
			// 
			this.m_rbManualProxy.AutoSize = true;
			this.m_rbManualProxy.Location = new System.Drawing.Point(9, 65);
			this.m_rbManualProxy.Name = "m_rbManualProxy";
			this.m_rbManualProxy.Size = new System.Drawing.Size(155, 17);
			this.m_rbManualProxy.TabIndex = 2;
			this.m_rbManualProxy.TabStop = true;
			this.m_rbManualProxy.Text = "&Manual proxy configuration:";
			this.m_rbManualProxy.UseVisualStyleBackColor = true;
			this.m_rbManualProxy.CheckedChanged += new System.EventHandler(this.OnManualProxyCheckedChanged);
			// 
			// m_rbSystemProxy
			// 
			this.m_rbSystemProxy.AutoSize = true;
			this.m_rbSystemProxy.Location = new System.Drawing.Point(9, 42);
			this.m_rbSystemProxy.Name = "m_rbSystemProxy";
			this.m_rbSystemProxy.Size = new System.Drawing.Size(146, 17);
			this.m_rbSystemProxy.TabIndex = 1;
			this.m_rbSystemProxy.TabStop = true;
			this.m_rbSystemProxy.Text = "Use &system proxy settings";
			this.m_rbSystemProxy.UseVisualStyleBackColor = true;
			this.m_rbSystemProxy.CheckedChanged += new System.EventHandler(this.OnSystemProxyCheckedChanged);
			// 
			// m_rbNoProxy
			// 
			this.m_rbNoProxy.AutoSize = true;
			this.m_rbNoProxy.Location = new System.Drawing.Point(9, 19);
			this.m_rbNoProxy.Name = "m_rbNoProxy";
			this.m_rbNoProxy.Size = new System.Drawing.Size(67, 17);
			this.m_rbNoProxy.TabIndex = 0;
			this.m_rbNoProxy.TabStop = true;
			this.m_rbNoProxy.Text = "&No proxy";
			this.m_rbNoProxy.UseVisualStyleBackColor = true;
			this.m_rbNoProxy.CheckedChanged += new System.EventHandler(this.OnNoProxyCheckedChanged);
			// 
			// m_grpAuth
			// 
			this.m_grpAuth.Controls.Add(this.m_rbAuthManual);
			this.m_grpAuth.Controls.Add(this.m_rbAuthDefault);
			this.m_grpAuth.Controls.Add(this.m_rbAuthNone);
			this.m_grpAuth.Controls.Add(this.m_tbPassword);
			this.m_grpAuth.Controls.Add(this.m_lblPassword);
			this.m_grpAuth.Controls.Add(this.m_tbUser);
			this.m_grpAuth.Controls.Add(this.m_lblUser);
			this.m_grpAuth.Location = new System.Drawing.Point(12, 138);
			this.m_grpAuth.Name = "m_grpAuth";
			this.m_grpAuth.Size = new System.Drawing.Size(327, 146);
			this.m_grpAuth.TabIndex = 3;
			this.m_grpAuth.TabStop = false;
			this.m_grpAuth.Text = "Authentication";
			// 
			// m_rbAuthManual
			// 
			this.m_rbAuthManual.AutoSize = true;
			this.m_rbAuthManual.Location = new System.Drawing.Point(9, 65);
			this.m_rbAuthManual.Name = "m_rbAuthManual";
			this.m_rbAuthManual.Size = new System.Drawing.Size(163, 17);
			this.m_rbAuthManual.TabIndex = 2;
			this.m_rbAuthManual.TabStop = true;
			this.m_rbAuthManual.Text = "Use the following &credentials:";
			this.m_rbAuthManual.UseVisualStyleBackColor = true;
			this.m_rbAuthManual.CheckedChanged += new System.EventHandler(this.OnAuthManualCheckedChanged);
			// 
			// m_rbAuthDefault
			// 
			this.m_rbAuthDefault.AutoSize = true;
			this.m_rbAuthDefault.Location = new System.Drawing.Point(9, 42);
			this.m_rbAuthDefault.Name = "m_rbAuthDefault";
			this.m_rbAuthDefault.Size = new System.Drawing.Size(113, 17);
			this.m_rbAuthDefault.TabIndex = 1;
			this.m_rbAuthDefault.TabStop = true;
			this.m_rbAuthDefault.Text = "&Default credentials";
			this.m_rbAuthDefault.UseVisualStyleBackColor = true;
			this.m_rbAuthDefault.CheckedChanged += new System.EventHandler(this.OnAuthDefaultCheckedChanged);
			// 
			// m_rbAuthNone
			// 
			this.m_rbAuthNone.AutoSize = true;
			this.m_rbAuthNone.Location = new System.Drawing.Point(9, 19);
			this.m_rbAuthNone.Name = "m_rbAuthNone";
			this.m_rbAuthNone.Size = new System.Drawing.Size(93, 17);
			this.m_rbAuthNone.TabIndex = 0;
			this.m_rbAuthNone.TabStop = true;
			this.m_rbAuthNone.Text = "N&o credentials";
			this.m_rbAuthNone.UseVisualStyleBackColor = true;
			this.m_rbAuthNone.CheckedChanged += new System.EventHandler(this.OnAuthNoneCheckedChanged);
			// 
			// m_tbPassword
			// 
			this.m_tbPassword.Location = new System.Drawing.Point(92, 114);
			this.m_tbPassword.Name = "m_tbPassword";
			this.m_tbPassword.Size = new System.Drawing.Size(224, 20);
			this.m_tbPassword.TabIndex = 6;
			this.m_tbPassword.UseSystemPasswordChar = true;
			// 
			// m_lblPassword
			// 
			this.m_lblPassword.AutoSize = true;
			this.m_lblPassword.Location = new System.Drawing.Point(25, 117);
			this.m_lblPassword.Name = "m_lblPassword";
			this.m_lblPassword.Size = new System.Drawing.Size(56, 13);
			this.m_lblPassword.TabIndex = 5;
			this.m_lblPassword.Text = "Password:";
			// 
			// m_tbUser
			// 
			this.m_tbUser.Location = new System.Drawing.Point(92, 88);
			this.m_tbUser.Name = "m_tbUser";
			this.m_tbUser.Size = new System.Drawing.Size(224, 20);
			this.m_tbUser.TabIndex = 4;
			// 
			// m_lblUser
			// 
			this.m_lblUser.AutoSize = true;
			this.m_lblUser.Location = new System.Drawing.Point(25, 91);
			this.m_lblUser.Name = "m_lblUser";
			this.m_lblUser.Size = new System.Drawing.Size(61, 13);
			this.m_lblUser.TabIndex = 3;
			this.m_lblUser.Text = "User name:";
			// 
			// ProxyForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(351, 332);
			this.Controls.Add(this.m_grpAuth);
			this.Controls.Add(this.m_grpServer);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProxyForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Proxy Settings";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.m_grpServer.ResumeLayout(false);
			this.m_grpServer.PerformLayout();
			this.m_grpAuth.ResumeLayout(false);
			this.m_grpAuth.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.GroupBox m_grpServer;
		private System.Windows.Forms.RadioButton m_rbSystemProxy;
		private System.Windows.Forms.RadioButton m_rbNoProxy;
		private System.Windows.Forms.GroupBox m_grpAuth;
		private System.Windows.Forms.TextBox m_tbPassword;
		private System.Windows.Forms.Label m_lblPassword;
		private System.Windows.Forms.TextBox m_tbUser;
		private System.Windows.Forms.Label m_lblUser;
		private System.Windows.Forms.TextBox m_tbPort;
		private System.Windows.Forms.Label m_lblPort;
		private System.Windows.Forms.TextBox m_tbAddress;
		private System.Windows.Forms.Label m_lblAddress;
		private System.Windows.Forms.RadioButton m_rbManualProxy;
		private System.Windows.Forms.RadioButton m_rbAuthDefault;
		private System.Windows.Forms.RadioButton m_rbAuthNone;
		private System.Windows.Forms.RadioButton m_rbAuthManual;
	}
}