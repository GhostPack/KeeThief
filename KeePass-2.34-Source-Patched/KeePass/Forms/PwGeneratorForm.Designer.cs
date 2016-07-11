namespace KeePass.Forms
{
	partial class PwGeneratorForm
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
			this.components = new System.ComponentModel.Container();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_rbStandardCharSet = new System.Windows.Forms.RadioButton();
			this.m_lblNumGenChars = new System.Windows.Forms.Label();
			this.m_numGenChars = new System.Windows.Forms.NumericUpDown();
			this.m_bannerImage = new System.Windows.Forms.PictureBox();
			this.m_lblProfile = new System.Windows.Forms.Label();
			this.m_cmbProfiles = new System.Windows.Forms.ComboBox();
			this.m_btnProfileAdd = new System.Windows.Forms.Button();
			this.m_btnProfileRemove = new System.Windows.Forms.Button();
			this.m_ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.m_grpCurOpt = new System.Windows.Forms.GroupBox();
			this.m_btnCustomOpt = new System.Windows.Forms.Button();
			this.m_cmbCustomAlgo = new System.Windows.Forms.ComboBox();
			this.m_rbCustom = new System.Windows.Forms.RadioButton();
			this.m_cbPatternPermute = new System.Windows.Forms.CheckBox();
			this.m_lblCustomChars = new System.Windows.Forms.Label();
			this.m_tbCustomChars = new System.Windows.Forms.TextBox();
			this.m_cbHighAnsi = new System.Windows.Forms.CheckBox();
			this.m_cbBrackets = new System.Windows.Forms.CheckBox();
			this.m_cbSpecial = new System.Windows.Forms.CheckBox();
			this.m_cbSpace = new System.Windows.Forms.CheckBox();
			this.m_cbUnderline = new System.Windows.Forms.CheckBox();
			this.m_cbMinus = new System.Windows.Forms.CheckBox();
			this.m_cbDigits = new System.Windows.Forms.CheckBox();
			this.m_cbLowerCase = new System.Windows.Forms.CheckBox();
			this.m_cbUpperCase = new System.Windows.Forms.CheckBox();
			this.m_cbEntropy = new System.Windows.Forms.CheckBox();
			this.m_tbPattern = new System.Windows.Forms.TextBox();
			this.m_rbPattern = new System.Windows.Forms.RadioButton();
			this.m_cbExcludeLookAlike = new System.Windows.Forms.CheckBox();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_tabMain = new System.Windows.Forms.TabControl();
			this.m_tabSettings = new System.Windows.Forms.TabPage();
			this.m_tabAdvanced = new System.Windows.Forms.TabPage();
			this.m_tbExcludeChars = new System.Windows.Forms.TextBox();
			this.m_lblExcludeChars = new System.Windows.Forms.Label();
			this.m_cbNoRepeat = new System.Windows.Forms.CheckBox();
			this.m_lblSecRedInfo = new System.Windows.Forms.Label();
			this.m_tabPreview = new System.Windows.Forms.TabPage();
			this.m_pbPreview = new System.Windows.Forms.ProgressBar();
			this.m_tbPreview = new System.Windows.Forms.TextBox();
			this.m_lblPreview = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_numGenChars)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_grpCurOpt.SuspendLayout();
			this.m_tabMain.SuspendLayout();
			this.m_tabSettings.SuspendLayout();
			this.m_tabAdvanced.SuspendLayout();
			this.m_tabPreview.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(321, 515);
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
			this.m_btnCancel.Location = new System.Drawing.Point(402, 515);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 1;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_rbStandardCharSet
			// 
			this.m_rbStandardCharSet.AutoSize = true;
			this.m_rbStandardCharSet.Checked = true;
			this.m_rbStandardCharSet.Location = new System.Drawing.Point(9, 20);
			this.m_rbStandardCharSet.Name = "m_rbStandardCharSet";
			this.m_rbStandardCharSet.Size = new System.Drawing.Size(165, 17);
			this.m_rbStandardCharSet.TabIndex = 0;
			this.m_rbStandardCharSet.TabStop = true;
			this.m_rbStandardCharSet.Text = "Generate using character set:";
			this.m_rbStandardCharSet.UseVisualStyleBackColor = true;
			// 
			// m_lblNumGenChars
			// 
			this.m_lblNumGenChars.AutoSize = true;
			this.m_lblNumGenChars.Location = new System.Drawing.Point(26, 45);
			this.m_lblNumGenChars.Name = "m_lblNumGenChars";
			this.m_lblNumGenChars.Size = new System.Drawing.Size(154, 13);
			this.m_lblNumGenChars.TabIndex = 1;
			this.m_lblNumGenChars.Text = "Length of generated password:";
			// 
			// m_numGenChars
			// 
			this.m_numGenChars.Location = new System.Drawing.Point(228, 43);
			this.m_numGenChars.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
			this.m_numGenChars.Name = "m_numGenChars";
			this.m_numGenChars.Size = new System.Drawing.Size(69, 20);
			this.m_numGenChars.TabIndex = 2;
			this.m_numGenChars.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(489, 60);
			this.m_bannerImage.TabIndex = 10;
			this.m_bannerImage.TabStop = false;
			// 
			// m_lblProfile
			// 
			this.m_lblProfile.AutoSize = true;
			this.m_lblProfile.Location = new System.Drawing.Point(6, 9);
			this.m_lblProfile.Name = "m_lblProfile";
			this.m_lblProfile.Size = new System.Drawing.Size(39, 13);
			this.m_lblProfile.TabIndex = 0;
			this.m_lblProfile.Text = "Profile:";
			// 
			// m_cmbProfiles
			// 
			this.m_cmbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cmbProfiles.FormattingEnabled = true;
			this.m_cmbProfiles.Location = new System.Drawing.Point(51, 6);
			this.m_cmbProfiles.MaxDropDownItems = 16;
			this.m_cmbProfiles.Name = "m_cmbProfiles";
			this.m_cmbProfiles.Size = new System.Drawing.Size(341, 21);
			this.m_cmbProfiles.TabIndex = 1;
			this.m_cmbProfiles.SelectedIndexChanged += new System.EventHandler(this.OnProfilesSelectedIndexChanged);
			// 
			// m_btnProfileAdd
			// 
			this.m_btnProfileAdd.Location = new System.Drawing.Point(398, 5);
			this.m_btnProfileAdd.Name = "m_btnProfileAdd";
			this.m_btnProfileAdd.Size = new System.Drawing.Size(25, 23);
			this.m_btnProfileAdd.TabIndex = 2;
			this.m_btnProfileAdd.UseVisualStyleBackColor = true;
			this.m_btnProfileAdd.Click += new System.EventHandler(this.OnBtnProfileSave);
			// 
			// m_btnProfileRemove
			// 
			this.m_btnProfileRemove.Location = new System.Drawing.Point(426, 5);
			this.m_btnProfileRemove.Name = "m_btnProfileRemove";
			this.m_btnProfileRemove.Size = new System.Drawing.Size(25, 23);
			this.m_btnProfileRemove.TabIndex = 3;
			this.m_btnProfileRemove.UseVisualStyleBackColor = true;
			this.m_btnProfileRemove.Click += new System.EventHandler(this.OnBtnProfileRemove);
			// 
			// m_grpCurOpt
			// 
			this.m_grpCurOpt.Controls.Add(this.m_btnCustomOpt);
			this.m_grpCurOpt.Controls.Add(this.m_cmbCustomAlgo);
			this.m_grpCurOpt.Controls.Add(this.m_rbCustom);
			this.m_grpCurOpt.Controls.Add(this.m_cbPatternPermute);
			this.m_grpCurOpt.Controls.Add(this.m_lblCustomChars);
			this.m_grpCurOpt.Controls.Add(this.m_tbCustomChars);
			this.m_grpCurOpt.Controls.Add(this.m_cbHighAnsi);
			this.m_grpCurOpt.Controls.Add(this.m_cbBrackets);
			this.m_grpCurOpt.Controls.Add(this.m_cbSpecial);
			this.m_grpCurOpt.Controls.Add(this.m_cbSpace);
			this.m_grpCurOpt.Controls.Add(this.m_cbUnderline);
			this.m_grpCurOpt.Controls.Add(this.m_cbMinus);
			this.m_grpCurOpt.Controls.Add(this.m_cbDigits);
			this.m_grpCurOpt.Controls.Add(this.m_cbLowerCase);
			this.m_grpCurOpt.Controls.Add(this.m_cbUpperCase);
			this.m_grpCurOpt.Controls.Add(this.m_cbEntropy);
			this.m_grpCurOpt.Controls.Add(this.m_tbPattern);
			this.m_grpCurOpt.Controls.Add(this.m_rbPattern);
			this.m_grpCurOpt.Controls.Add(this.m_lblNumGenChars);
			this.m_grpCurOpt.Controls.Add(this.m_rbStandardCharSet);
			this.m_grpCurOpt.Controls.Add(this.m_numGenChars);
			this.m_grpCurOpt.Location = new System.Drawing.Point(6, 31);
			this.m_grpCurOpt.Name = "m_grpCurOpt";
			this.m_grpCurOpt.Size = new System.Drawing.Size(445, 377);
			this.m_grpCurOpt.TabIndex = 4;
			this.m_grpCurOpt.TabStop = false;
			this.m_grpCurOpt.Text = "Current settings";
			// 
			// m_btnCustomOpt
			// 
			this.m_btnCustomOpt.Image = global::KeePass.Properties.Resources.B16x16_Misc;
			this.m_btnCustomOpt.Location = new System.Drawing.Point(414, 321);
			this.m_btnCustomOpt.Name = "m_btnCustomOpt";
			this.m_btnCustomOpt.Size = new System.Drawing.Size(25, 23);
			this.m_btnCustomOpt.TabIndex = 19;
			this.m_btnCustomOpt.UseVisualStyleBackColor = true;
			this.m_btnCustomOpt.Click += new System.EventHandler(this.OnBtnCustomOpt);
			// 
			// m_cmbCustomAlgo
			// 
			this.m_cmbCustomAlgo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cmbCustomAlgo.FormattingEnabled = true;
			this.m_cmbCustomAlgo.Location = new System.Drawing.Point(29, 322);
			this.m_cmbCustomAlgo.Name = "m_cmbCustomAlgo";
			this.m_cmbCustomAlgo.Size = new System.Drawing.Size(379, 21);
			this.m_cmbCustomAlgo.TabIndex = 18;
			// 
			// m_rbCustom
			// 
			this.m_rbCustom.AutoSize = true;
			this.m_rbCustom.Location = new System.Drawing.Point(9, 299);
			this.m_rbCustom.Name = "m_rbCustom";
			this.m_rbCustom.Size = new System.Drawing.Size(182, 17);
			this.m_rbCustom.TabIndex = 17;
			this.m_rbCustom.TabStop = true;
			this.m_rbCustom.Text = "Generate using custom algorithm:";
			this.m_rbCustom.UseVisualStyleBackColor = true;
			// 
			// m_cbPatternPermute
			// 
			this.m_cbPatternPermute.AutoSize = true;
			this.m_cbPatternPermute.Location = new System.Drawing.Point(29, 276);
			this.m_cbPatternPermute.Name = "m_cbPatternPermute";
			this.m_cbPatternPermute.Size = new System.Drawing.Size(227, 17);
			this.m_cbPatternPermute.TabIndex = 16;
			this.m_cbPatternPermute.Text = "Randomly permute characters of password";
			this.m_cbPatternPermute.UseVisualStyleBackColor = true;
			// 
			// m_lblCustomChars
			// 
			this.m_lblCustomChars.AutoSize = true;
			this.m_lblCustomChars.Location = new System.Drawing.Point(26, 183);
			this.m_lblCustomChars.Name = "m_lblCustomChars";
			this.m_lblCustomChars.Size = new System.Drawing.Size(182, 13);
			this.m_lblCustomChars.TabIndex = 12;
			this.m_lblCustomChars.Text = "Also include the following characters:";
			// 
			// m_tbCustomChars
			// 
			this.m_tbCustomChars.Location = new System.Drawing.Point(29, 199);
			this.m_tbCustomChars.Name = "m_tbCustomChars";
			this.m_tbCustomChars.Size = new System.Drawing.Size(410, 20);
			this.m_tbCustomChars.TabIndex = 13;
			// 
			// m_cbHighAnsi
			// 
			this.m_cbHighAnsi.AutoSize = true;
			this.m_cbHighAnsi.Location = new System.Drawing.Point(228, 137);
			this.m_cbHighAnsi.Name = "m_cbHighAnsi";
			this.m_cbHighAnsi.Size = new System.Drawing.Size(129, 17);
			this.m_cbHighAnsi.TabIndex = 10;
			this.m_cbHighAnsi.Text = "High ANSI characters";
			this.m_cbHighAnsi.UseVisualStyleBackColor = true;
			// 
			// m_cbBrackets
			// 
			this.m_cbBrackets.AutoSize = true;
			this.m_cbBrackets.Location = new System.Drawing.Point(228, 115);
			this.m_cbBrackets.Name = "m_cbBrackets";
			this.m_cbBrackets.Size = new System.Drawing.Size(68, 17);
			this.m_cbBrackets.TabIndex = 8;
			this.m_cbBrackets.Text = "Brackets";
			this.m_cbBrackets.UseVisualStyleBackColor = true;
			// 
			// m_cbSpecial
			// 
			this.m_cbSpecial.AutoSize = true;
			this.m_cbSpecial.Location = new System.Drawing.Point(228, 93);
			this.m_cbSpecial.Name = "m_cbSpecial";
			this.m_cbSpecial.Size = new System.Drawing.Size(61, 17);
			this.m_cbSpecial.TabIndex = 6;
			this.m_cbSpecial.Text = "Special";
			this.m_cbSpecial.UseVisualStyleBackColor = true;
			// 
			// m_cbSpace
			// 
			this.m_cbSpace.AutoSize = true;
			this.m_cbSpace.Location = new System.Drawing.Point(228, 71);
			this.m_cbSpace.Name = "m_cbSpace";
			this.m_cbSpace.Size = new System.Drawing.Size(57, 17);
			this.m_cbSpace.TabIndex = 4;
			this.m_cbSpace.Text = "Space";
			this.m_cbSpace.UseVisualStyleBackColor = true;
			// 
			// m_cbUnderline
			// 
			this.m_cbUnderline.AutoSize = true;
			this.m_cbUnderline.Location = new System.Drawing.Point(29, 159);
			this.m_cbUnderline.Name = "m_cbUnderline";
			this.m_cbUnderline.Size = new System.Drawing.Size(71, 17);
			this.m_cbUnderline.TabIndex = 11;
			this.m_cbUnderline.Text = "Underline";
			this.m_cbUnderline.UseVisualStyleBackColor = true;
			// 
			// m_cbMinus
			// 
			this.m_cbMinus.AutoSize = true;
			this.m_cbMinus.Location = new System.Drawing.Point(29, 137);
			this.m_cbMinus.Name = "m_cbMinus";
			this.m_cbMinus.Size = new System.Drawing.Size(54, 17);
			this.m_cbMinus.TabIndex = 9;
			this.m_cbMinus.Text = "Minus";
			this.m_cbMinus.UseVisualStyleBackColor = true;
			// 
			// m_cbDigits
			// 
			this.m_cbDigits.AutoSize = true;
			this.m_cbDigits.Location = new System.Drawing.Point(29, 115);
			this.m_cbDigits.Name = "m_cbDigits";
			this.m_cbDigits.Size = new System.Drawing.Size(52, 17);
			this.m_cbDigits.TabIndex = 7;
			this.m_cbDigits.Text = "Digits";
			this.m_cbDigits.UseVisualStyleBackColor = true;
			// 
			// m_cbLowerCase
			// 
			this.m_cbLowerCase.AutoSize = true;
			this.m_cbLowerCase.Location = new System.Drawing.Point(29, 93);
			this.m_cbLowerCase.Name = "m_cbLowerCase";
			this.m_cbLowerCase.Size = new System.Drawing.Size(81, 17);
			this.m_cbLowerCase.TabIndex = 5;
			this.m_cbLowerCase.Text = "Lower-case";
			this.m_cbLowerCase.UseVisualStyleBackColor = true;
			// 
			// m_cbUpperCase
			// 
			this.m_cbUpperCase.AutoSize = true;
			this.m_cbUpperCase.Location = new System.Drawing.Point(29, 71);
			this.m_cbUpperCase.Name = "m_cbUpperCase";
			this.m_cbUpperCase.Size = new System.Drawing.Size(81, 17);
			this.m_cbUpperCase.TabIndex = 3;
			this.m_cbUpperCase.Text = "Upper-case";
			this.m_cbUpperCase.UseVisualStyleBackColor = true;
			// 
			// m_cbEntropy
			// 
			this.m_cbEntropy.AutoSize = true;
			this.m_cbEntropy.Location = new System.Drawing.Point(9, 352);
			this.m_cbEntropy.Name = "m_cbEntropy";
			this.m_cbEntropy.Size = new System.Drawing.Size(144, 17);
			this.m_cbEntropy.TabIndex = 20;
			this.m_cbEntropy.Text = "Collect additional entropy";
			this.m_cbEntropy.UseVisualStyleBackColor = true;
			// 
			// m_tbPattern
			// 
			this.m_tbPattern.Location = new System.Drawing.Point(29, 250);
			this.m_tbPattern.Name = "m_tbPattern";
			this.m_tbPattern.Size = new System.Drawing.Size(410, 20);
			this.m_tbPattern.TabIndex = 15;
			// 
			// m_rbPattern
			// 
			this.m_rbPattern.AutoSize = true;
			this.m_rbPattern.Location = new System.Drawing.Point(9, 227);
			this.m_rbPattern.Name = "m_rbPattern";
			this.m_rbPattern.Size = new System.Drawing.Size(136, 17);
			this.m_rbPattern.TabIndex = 14;
			this.m_rbPattern.TabStop = true;
			this.m_rbPattern.Text = "Generate using pattern:";
			this.m_rbPattern.UseVisualStyleBackColor = true;
			// 
			// m_cbExcludeLookAlike
			// 
			this.m_cbExcludeLookAlike.AutoSize = true;
			this.m_cbExcludeLookAlike.Location = new System.Drawing.Point(12, 39);
			this.m_cbExcludeLookAlike.Name = "m_cbExcludeLookAlike";
			this.m_cbExcludeLookAlike.Size = new System.Drawing.Size(165, 17);
			this.m_cbExcludeLookAlike.TabIndex = 1;
			this.m_cbExcludeLookAlike.Text = "Exclude look-alike characters";
			this.m_cbExcludeLookAlike.UseVisualStyleBackColor = true;
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Location = new System.Drawing.Point(12, 515);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
			this.m_btnHelp.TabIndex = 3;
			this.m_btnHelp.Text = "&Help";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.OnBtnHelp);
			// 
			// m_tabMain
			// 
			this.m_tabMain.Controls.Add(this.m_tabSettings);
			this.m_tabMain.Controls.Add(this.m_tabAdvanced);
			this.m_tabMain.Controls.Add(this.m_tabPreview);
			this.m_tabMain.Location = new System.Drawing.Point(12, 66);
			this.m_tabMain.Name = "m_tabMain";
			this.m_tabMain.SelectedIndex = 0;
			this.m_tabMain.Size = new System.Drawing.Size(465, 439);
			this.m_tabMain.TabIndex = 2;
			this.m_tabMain.SelectedIndexChanged += new System.EventHandler(this.OnTabMainSelectedIndexChanged);
			// 
			// m_tabSettings
			// 
			this.m_tabSettings.Controls.Add(this.m_lblProfile);
			this.m_tabSettings.Controls.Add(this.m_cmbProfiles);
			this.m_tabSettings.Controls.Add(this.m_grpCurOpt);
			this.m_tabSettings.Controls.Add(this.m_btnProfileAdd);
			this.m_tabSettings.Controls.Add(this.m_btnProfileRemove);
			this.m_tabSettings.Location = new System.Drawing.Point(4, 22);
			this.m_tabSettings.Name = "m_tabSettings";
			this.m_tabSettings.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabSettings.Size = new System.Drawing.Size(457, 413);
			this.m_tabSettings.TabIndex = 0;
			this.m_tabSettings.Text = "Settings";
			this.m_tabSettings.UseVisualStyleBackColor = true;
			// 
			// m_tabAdvanced
			// 
			this.m_tabAdvanced.Controls.Add(this.m_tbExcludeChars);
			this.m_tabAdvanced.Controls.Add(this.m_lblExcludeChars);
			this.m_tabAdvanced.Controls.Add(this.m_cbExcludeLookAlike);
			this.m_tabAdvanced.Controls.Add(this.m_cbNoRepeat);
			this.m_tabAdvanced.Controls.Add(this.m_lblSecRedInfo);
			this.m_tabAdvanced.Location = new System.Drawing.Point(4, 22);
			this.m_tabAdvanced.Name = "m_tabAdvanced";
			this.m_tabAdvanced.Size = new System.Drawing.Size(457, 413);
			this.m_tabAdvanced.TabIndex = 2;
			this.m_tabAdvanced.Text = "Advanced";
			this.m_tabAdvanced.UseVisualStyleBackColor = true;
			// 
			// m_tbExcludeChars
			// 
			this.m_tbExcludeChars.Location = new System.Drawing.Point(12, 84);
			this.m_tbExcludeChars.Name = "m_tbExcludeChars";
			this.m_tbExcludeChars.Size = new System.Drawing.Size(432, 20);
			this.m_tbExcludeChars.TabIndex = 3;
			// 
			// m_lblExcludeChars
			// 
			this.m_lblExcludeChars.AutoSize = true;
			this.m_lblExcludeChars.Location = new System.Drawing.Point(9, 68);
			this.m_lblExcludeChars.Name = "m_lblExcludeChars";
			this.m_lblExcludeChars.Size = new System.Drawing.Size(163, 13);
			this.m_lblExcludeChars.TabIndex = 2;
			this.m_lblExcludeChars.Text = "Exclude the following characters:";
			// 
			// m_cbNoRepeat
			// 
			this.m_cbNoRepeat.AutoSize = true;
			this.m_cbNoRepeat.Location = new System.Drawing.Point(12, 16);
			this.m_cbNoRepeat.Name = "m_cbNoRepeat";
			this.m_cbNoRepeat.Size = new System.Drawing.Size(218, 17);
			this.m_cbNoRepeat.TabIndex = 0;
			this.m_cbNoRepeat.Text = "Each character must occur at most once";
			this.m_cbNoRepeat.UseVisualStyleBackColor = true;
			// 
			// m_lblSecRedInfo
			// 
			this.m_lblSecRedInfo.Location = new System.Drawing.Point(9, 362);
			this.m_lblSecRedInfo.Name = "m_lblSecRedInfo";
			this.m_lblSecRedInfo.Size = new System.Drawing.Size(445, 41);
			this.m_lblSecRedInfo.TabIndex = 4;
			this.m_lblSecRedInfo.Text = "Options/rules marked with an asterisk reduce the security of generated passwords." +
				" Only enable them if you are forced to follow such rules by the website, applica" +
				"tion or password policy.";
			// 
			// m_tabPreview
			// 
			this.m_tabPreview.Controls.Add(this.m_pbPreview);
			this.m_tabPreview.Controls.Add(this.m_tbPreview);
			this.m_tabPreview.Controls.Add(this.m_lblPreview);
			this.m_tabPreview.Location = new System.Drawing.Point(4, 22);
			this.m_tabPreview.Name = "m_tabPreview";
			this.m_tabPreview.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabPreview.Size = new System.Drawing.Size(457, 413);
			this.m_tabPreview.TabIndex = 1;
			this.m_tabPreview.Text = "Preview";
			this.m_tabPreview.UseVisualStyleBackColor = true;
			// 
			// m_pbPreview
			// 
			this.m_pbPreview.Location = new System.Drawing.Point(9, 30);
			this.m_pbPreview.Name = "m_pbPreview";
			this.m_pbPreview.Size = new System.Drawing.Size(437, 15);
			this.m_pbPreview.TabIndex = 1;
			// 
			// m_tbPreview
			// 
			this.m_tbPreview.AcceptsReturn = true;
			this.m_tbPreview.Location = new System.Drawing.Point(9, 51);
			this.m_tbPreview.Multiline = true;
			this.m_tbPreview.Name = "m_tbPreview";
			this.m_tbPreview.ReadOnly = true;
			this.m_tbPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.m_tbPreview.Size = new System.Drawing.Size(437, 350);
			this.m_tbPreview.TabIndex = 2;
			this.m_tbPreview.WordWrap = false;
			// 
			// m_lblPreview
			// 
			this.m_lblPreview.Location = new System.Drawing.Point(6, 12);
			this.m_lblPreview.Name = "m_lblPreview";
			this.m_lblPreview.Size = new System.Drawing.Size(445, 15);
			this.m_lblPreview.TabIndex = 0;
			this.m_lblPreview.Text = "Here you see a few sample passwords matching the rules specified on the first tab" +
				" pages.";
			// 
			// PwGeneratorForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(489, 550);
			this.Controls.Add(this.m_tabMain);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_bannerImage);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PwGeneratorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Password Generator";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.m_numGenChars)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_grpCurOpt.ResumeLayout(false);
			this.m_grpCurOpt.PerformLayout();
			this.m_tabMain.ResumeLayout(false);
			this.m_tabSettings.ResumeLayout(false);
			this.m_tabSettings.PerformLayout();
			this.m_tabAdvanced.ResumeLayout(false);
			this.m_tabAdvanced.PerformLayout();
			this.m_tabPreview.ResumeLayout(false);
			this.m_tabPreview.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.RadioButton m_rbStandardCharSet;
		private System.Windows.Forms.Label m_lblNumGenChars;
		private System.Windows.Forms.NumericUpDown m_numGenChars;
		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Label m_lblProfile;
		private System.Windows.Forms.ComboBox m_cmbProfiles;
		private System.Windows.Forms.Button m_btnProfileAdd;
		private System.Windows.Forms.ToolTip m_ttMain;
		private System.Windows.Forms.Button m_btnProfileRemove;
		private System.Windows.Forms.GroupBox m_grpCurOpt;
		private System.Windows.Forms.TextBox m_tbPattern;
		private System.Windows.Forms.RadioButton m_rbPattern;
		private System.Windows.Forms.CheckBox m_cbEntropy;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.TabControl m_tabMain;
		private System.Windows.Forms.TabPage m_tabSettings;
		private System.Windows.Forms.TabPage m_tabPreview;
		private System.Windows.Forms.Label m_lblPreview;
		private System.Windows.Forms.TextBox m_tbPreview;
		private System.Windows.Forms.ProgressBar m_pbPreview;
		private System.Windows.Forms.CheckBox m_cbHighAnsi;
		private System.Windows.Forms.CheckBox m_cbBrackets;
		private System.Windows.Forms.CheckBox m_cbSpecial;
		private System.Windows.Forms.CheckBox m_cbSpace;
		private System.Windows.Forms.CheckBox m_cbUnderline;
		private System.Windows.Forms.CheckBox m_cbMinus;
		private System.Windows.Forms.CheckBox m_cbDigits;
		private System.Windows.Forms.CheckBox m_cbLowerCase;
		private System.Windows.Forms.CheckBox m_cbUpperCase;
		private System.Windows.Forms.CheckBox m_cbExcludeLookAlike;
		private System.Windows.Forms.Label m_lblCustomChars;
		private System.Windows.Forms.TextBox m_tbCustomChars;
		private System.Windows.Forms.CheckBox m_cbPatternPermute;
		private System.Windows.Forms.TabPage m_tabAdvanced;
		private System.Windows.Forms.CheckBox m_cbNoRepeat;
		private System.Windows.Forms.Label m_lblSecRedInfo;
		private System.Windows.Forms.TextBox m_tbExcludeChars;
		private System.Windows.Forms.Label m_lblExcludeChars;
		private System.Windows.Forms.ComboBox m_cmbCustomAlgo;
		private System.Windows.Forms.RadioButton m_rbCustom;
		private System.Windows.Forms.Button m_btnCustomOpt;
	}
}