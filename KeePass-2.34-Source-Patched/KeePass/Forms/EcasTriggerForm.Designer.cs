namespace KeePass.Forms
{
	partial class EcasTriggerForm
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
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnPrev = new System.Windows.Forms.Button();
			this.m_btnNext = new System.Windows.Forms.Button();
			this.m_tabMain = new System.Windows.Forms.TabControl();
			this.m_tabProps = new System.Windows.Forms.TabPage();
			this.m_lblEnabledDesc = new System.Windows.Forms.Label();
			this.m_cbTurnOffAfterAction = new System.Windows.Forms.CheckBox();
			this.m_tbComments = new System.Windows.Forms.TextBox();
			this.m_lblTriggerComments = new System.Windows.Forms.Label();
			this.m_lblInitiallyOnDesc = new System.Windows.Forms.Label();
			this.m_cbInitiallyOn = new System.Windows.Forms.CheckBox();
			this.m_cbEnabled = new System.Windows.Forms.CheckBox();
			this.m_tbName = new System.Windows.Forms.TextBox();
			this.m_lblTriggerName = new System.Windows.Forms.Label();
			this.m_tabEvents = new System.Windows.Forms.TabPage();
			this.m_btnEventMoveDown = new System.Windows.Forms.Button();
			this.m_btnEventMoveUp = new System.Windows.Forms.Button();
			this.m_btnEventEdit = new System.Windows.Forms.Button();
			this.m_btnEventDelete = new System.Windows.Forms.Button();
			this.m_btnEventAdd = new System.Windows.Forms.Button();
			this.m_lblEventsIntro = new System.Windows.Forms.Label();
			this.m_lvEvents = new KeePass.UI.CustomListViewEx();
			this.m_tabConditions = new System.Windows.Forms.TabPage();
			this.m_btnConditionMoveDown = new System.Windows.Forms.Button();
			this.m_btnConditionMoveUp = new System.Windows.Forms.Button();
			this.m_btnConditionEdit = new System.Windows.Forms.Button();
			this.m_lblConditionsEmptyHint = new System.Windows.Forms.Label();
			this.m_btnConditionDelete = new System.Windows.Forms.Button();
			this.m_btnConditionAdd = new System.Windows.Forms.Button();
			this.m_lblConditionsIntro = new System.Windows.Forms.Label();
			this.m_lvConditions = new KeePass.UI.CustomListViewEx();
			this.m_tabActions = new System.Windows.Forms.TabPage();
			this.m_btnActionMoveDown = new System.Windows.Forms.Button();
			this.m_btnActionMoveUp = new System.Windows.Forms.Button();
			this.m_btnActionEdit = new System.Windows.Forms.Button();
			this.m_btnActionDelete = new System.Windows.Forms.Button();
			this.m_btnActionAdd = new System.Windows.Forms.Button();
			this.m_lvActions = new KeePass.UI.CustomListViewEx();
			this.m_lblActionsIntro = new System.Windows.Forms.Label();
			this.m_btnHelp = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
			this.m_tabMain.SuspendLayout();
			this.m_tabProps.SuspendLayout();
			this.m_tabEvents.SuspendLayout();
			this.m_tabConditions.SuspendLayout();
			this.m_tabActions.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_bannerImage
			// 
			this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
			this.m_bannerImage.Name = "m_bannerImage";
			this.m_bannerImage.Size = new System.Drawing.Size(594, 60);
			this.m_bannerImage.TabIndex = 0;
			this.m_bannerImage.TabStop = false;
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(426, 389);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.Size = new System.Drawing.Size(75, 23);
			this.m_btnOK.TabIndex = 4;
			this.m_btnOK.Text = "&Finish";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.OnBtnOK);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(507, 389);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.m_btnCancel.TabIndex = 5;
			this.m_btnCancel.Text = "Cancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.OnBtnCancel);
			// 
			// m_btnPrev
			// 
			this.m_btnPrev.Location = new System.Drawing.Point(258, 389);
			this.m_btnPrev.Name = "m_btnPrev";
			this.m_btnPrev.Size = new System.Drawing.Size(75, 23);
			this.m_btnPrev.TabIndex = 2;
			this.m_btnPrev.Text = "< &Back";
			this.m_btnPrev.UseVisualStyleBackColor = true;
			this.m_btnPrev.Click += new System.EventHandler(this.OnBtnPrev);
			// 
			// m_btnNext
			// 
			this.m_btnNext.Location = new System.Drawing.Point(333, 389);
			this.m_btnNext.Name = "m_btnNext";
			this.m_btnNext.Size = new System.Drawing.Size(75, 23);
			this.m_btnNext.TabIndex = 3;
			this.m_btnNext.Text = "&Next >";
			this.m_btnNext.UseVisualStyleBackColor = true;
			this.m_btnNext.Click += new System.EventHandler(this.OnBtnNext);
			// 
			// m_tabMain
			// 
			this.m_tabMain.Controls.Add(this.m_tabProps);
			this.m_tabMain.Controls.Add(this.m_tabEvents);
			this.m_tabMain.Controls.Add(this.m_tabConditions);
			this.m_tabMain.Controls.Add(this.m_tabActions);
			this.m_tabMain.Location = new System.Drawing.Point(12, 69);
			this.m_tabMain.Name = "m_tabMain";
			this.m_tabMain.SelectedIndex = 0;
			this.m_tabMain.Size = new System.Drawing.Size(570, 314);
			this.m_tabMain.TabIndex = 0;
			this.m_tabMain.SelectedIndexChanged += new System.EventHandler(this.OnTabMainSelectedIndexChanged);
			// 
			// m_tabProps
			// 
			this.m_tabProps.Controls.Add(this.m_lblEnabledDesc);
			this.m_tabProps.Controls.Add(this.m_cbTurnOffAfterAction);
			this.m_tabProps.Controls.Add(this.m_tbComments);
			this.m_tabProps.Controls.Add(this.m_lblTriggerComments);
			this.m_tabProps.Controls.Add(this.m_lblInitiallyOnDesc);
			this.m_tabProps.Controls.Add(this.m_cbInitiallyOn);
			this.m_tabProps.Controls.Add(this.m_cbEnabled);
			this.m_tabProps.Controls.Add(this.m_tbName);
			this.m_tabProps.Controls.Add(this.m_lblTriggerName);
			this.m_tabProps.Location = new System.Drawing.Point(4, 22);
			this.m_tabProps.Name = "m_tabProps";
			this.m_tabProps.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabProps.Size = new System.Drawing.Size(562, 288);
			this.m_tabProps.TabIndex = 0;
			this.m_tabProps.Text = "Properties";
			this.m_tabProps.UseVisualStyleBackColor = true;
			// 
			// m_lblEnabledDesc
			// 
			this.m_lblEnabledDesc.AutoSize = true;
			this.m_lblEnabledDesc.Location = new System.Drawing.Point(26, 81);
			this.m_lblEnabledDesc.Name = "m_lblEnabledDesc";
			this.m_lblEnabledDesc.Size = new System.Drawing.Size(204, 13);
			this.m_lblEnabledDesc.TabIndex = 3;
			this.m_lblEnabledDesc.Text = "If not enabled, the trigger has no function.";
			// 
			// m_cbTurnOffAfterAction
			// 
			this.m_cbTurnOffAfterAction.AutoSize = true;
			this.m_cbTurnOffAfterAction.Location = new System.Drawing.Point(9, 263);
			this.m_cbTurnOffAfterAction.Name = "m_cbTurnOffAfterAction";
			this.m_cbTurnOffAfterAction.Size = new System.Drawing.Size(224, 17);
			this.m_cbTurnOffAfterAction.TabIndex = 8;
			this.m_cbTurnOffAfterAction.Text = "Turn off after executing actions (run once)";
			this.m_cbTurnOffAfterAction.UseVisualStyleBackColor = true;
			// 
			// m_tbComments
			// 
			this.m_tbComments.AcceptsReturn = true;
			this.m_tbComments.Location = new System.Drawing.Point(9, 164);
			this.m_tbComments.Multiline = true;
			this.m_tbComments.Name = "m_tbComments";
			this.m_tbComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_tbComments.Size = new System.Drawing.Size(543, 83);
			this.m_tbComments.TabIndex = 7;
			// 
			// m_lblTriggerComments
			// 
			this.m_lblTriggerComments.AutoSize = true;
			this.m_lblTriggerComments.Location = new System.Drawing.Point(6, 148);
			this.m_lblTriggerComments.Name = "m_lblTriggerComments";
			this.m_lblTriggerComments.Size = new System.Drawing.Size(59, 13);
			this.m_lblTriggerComments.TabIndex = 6;
			this.m_lblTriggerComments.Text = "Comments:";
			// 
			// m_lblInitiallyOnDesc
			// 
			this.m_lblInitiallyOnDesc.AutoSize = true;
			this.m_lblInitiallyOnDesc.Location = new System.Drawing.Point(26, 126);
			this.m_lblInitiallyOnDesc.Name = "m_lblInitiallyOnDesc";
			this.m_lblInitiallyOnDesc.Size = new System.Drawing.Size(319, 13);
			this.m_lblInitiallyOnDesc.TabIndex = 5;
			this.m_lblInitiallyOnDesc.Text = "When checked, the trigger will initially be on when KeePass starts.";
			// 
			// m_cbInitiallyOn
			// 
			this.m_cbInitiallyOn.AutoSize = true;
			this.m_cbInitiallyOn.Location = new System.Drawing.Point(9, 106);
			this.m_cbInitiallyOn.Name = "m_cbInitiallyOn";
			this.m_cbInitiallyOn.Size = new System.Drawing.Size(72, 17);
			this.m_cbInitiallyOn.TabIndex = 4;
			this.m_cbInitiallyOn.Text = "Initially on";
			this.m_cbInitiallyOn.UseVisualStyleBackColor = true;
			// 
			// m_cbEnabled
			// 
			this.m_cbEnabled.AutoSize = true;
			this.m_cbEnabled.Location = new System.Drawing.Point(9, 61);
			this.m_cbEnabled.Name = "m_cbEnabled";
			this.m_cbEnabled.Size = new System.Drawing.Size(65, 17);
			this.m_cbEnabled.TabIndex = 2;
			this.m_cbEnabled.Text = "Enabled";
			this.m_cbEnabled.UseVisualStyleBackColor = true;
			// 
			// m_tbName
			// 
			this.m_tbName.Location = new System.Drawing.Point(9, 26);
			this.m_tbName.Name = "m_tbName";
			this.m_tbName.Size = new System.Drawing.Size(543, 20);
			this.m_tbName.TabIndex = 1;
			// 
			// m_lblTriggerName
			// 
			this.m_lblTriggerName.AutoSize = true;
			this.m_lblTriggerName.Location = new System.Drawing.Point(6, 10);
			this.m_lblTriggerName.Name = "m_lblTriggerName";
			this.m_lblTriggerName.Size = new System.Drawing.Size(38, 13);
			this.m_lblTriggerName.TabIndex = 0;
			this.m_lblTriggerName.Text = "Name:";
			// 
			// m_tabEvents
			// 
			this.m_tabEvents.Controls.Add(this.m_btnEventMoveDown);
			this.m_tabEvents.Controls.Add(this.m_btnEventMoveUp);
			this.m_tabEvents.Controls.Add(this.m_btnEventEdit);
			this.m_tabEvents.Controls.Add(this.m_btnEventDelete);
			this.m_tabEvents.Controls.Add(this.m_btnEventAdd);
			this.m_tabEvents.Controls.Add(this.m_lblEventsIntro);
			this.m_tabEvents.Controls.Add(this.m_lvEvents);
			this.m_tabEvents.Location = new System.Drawing.Point(4, 22);
			this.m_tabEvents.Name = "m_tabEvents";
			this.m_tabEvents.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabEvents.Size = new System.Drawing.Size(562, 288);
			this.m_tabEvents.TabIndex = 1;
			this.m_tabEvents.Text = "Events";
			this.m_tabEvents.UseVisualStyleBackColor = true;
			// 
			// m_btnEventMoveDown
			// 
			this.m_btnEventMoveDown.Image = global::KeePass.Properties.Resources.B16x16_1DownArrow;
			this.m_btnEventMoveDown.Location = new System.Drawing.Point(478, 161);
			this.m_btnEventMoveDown.Name = "m_btnEventMoveDown";
			this.m_btnEventMoveDown.Size = new System.Drawing.Size(75, 23);
			this.m_btnEventMoveDown.TabIndex = 6;
			this.m_btnEventMoveDown.UseVisualStyleBackColor = true;
			this.m_btnEventMoveDown.Click += new System.EventHandler(this.OnBtnEventMoveDown);
			// 
			// m_btnEventMoveUp
			// 
			this.m_btnEventMoveUp.Image = global::KeePass.Properties.Resources.B16x16_1UpArrow;
			this.m_btnEventMoveUp.Location = new System.Drawing.Point(478, 132);
			this.m_btnEventMoveUp.Name = "m_btnEventMoveUp";
			this.m_btnEventMoveUp.Size = new System.Drawing.Size(75, 23);
			this.m_btnEventMoveUp.TabIndex = 5;
			this.m_btnEventMoveUp.UseVisualStyleBackColor = true;
			this.m_btnEventMoveUp.Click += new System.EventHandler(this.OnBtnEventMoveUp);
			// 
			// m_btnEventEdit
			// 
			this.m_btnEventEdit.Location = new System.Drawing.Point(478, 55);
			this.m_btnEventEdit.Name = "m_btnEventEdit";
			this.m_btnEventEdit.Size = new System.Drawing.Size(75, 23);
			this.m_btnEventEdit.TabIndex = 3;
			this.m_btnEventEdit.Text = "&Edit...";
			this.m_btnEventEdit.UseVisualStyleBackColor = true;
			this.m_btnEventEdit.Click += new System.EventHandler(this.OnEventEdit);
			// 
			// m_btnEventDelete
			// 
			this.m_btnEventDelete.Location = new System.Drawing.Point(478, 84);
			this.m_btnEventDelete.Name = "m_btnEventDelete";
			this.m_btnEventDelete.Size = new System.Drawing.Size(75, 23);
			this.m_btnEventDelete.TabIndex = 4;
			this.m_btnEventDelete.Text = "&Delete";
			this.m_btnEventDelete.UseVisualStyleBackColor = true;
			this.m_btnEventDelete.Click += new System.EventHandler(this.OnEventDelete);
			// 
			// m_btnEventAdd
			// 
			this.m_btnEventAdd.Location = new System.Drawing.Point(478, 26);
			this.m_btnEventAdd.Name = "m_btnEventAdd";
			this.m_btnEventAdd.Size = new System.Drawing.Size(75, 23);
			this.m_btnEventAdd.TabIndex = 2;
			this.m_btnEventAdd.Text = "&Add...";
			this.m_btnEventAdd.UseVisualStyleBackColor = true;
			this.m_btnEventAdd.Click += new System.EventHandler(this.OnEventAdd);
			// 
			// m_lblEventsIntro
			// 
			this.m_lblEventsIntro.AutoSize = true;
			this.m_lblEventsIntro.Location = new System.Drawing.Point(6, 10);
			this.m_lblEventsIntro.Name = "m_lblEventsIntro";
			this.m_lblEventsIntro.Size = new System.Drawing.Size(297, 13);
			this.m_lblEventsIntro.TabIndex = 0;
			this.m_lblEventsIntro.Text = "The trigger will fire when any of the events listed below occur.";
			// 
			// m_lvEvents
			// 
			this.m_lvEvents.FullRowSelect = true;
			this.m_lvEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvEvents.HideSelection = false;
			this.m_lvEvents.Location = new System.Drawing.Point(9, 26);
			this.m_lvEvents.Name = "m_lvEvents";
			this.m_lvEvents.ShowItemToolTips = true;
			this.m_lvEvents.Size = new System.Drawing.Size(463, 253);
			this.m_lvEvents.TabIndex = 1;
			this.m_lvEvents.UseCompatibleStateImageBehavior = false;
			this.m_lvEvents.View = System.Windows.Forms.View.Details;
			this.m_lvEvents.ItemActivate += new System.EventHandler(this.OnEventsItemActivate);
			this.m_lvEvents.SelectedIndexChanged += new System.EventHandler(this.OnEventsSelectedIndexChanged);
			// 
			// m_tabConditions
			// 
			this.m_tabConditions.Controls.Add(this.m_btnConditionMoveDown);
			this.m_tabConditions.Controls.Add(this.m_btnConditionMoveUp);
			this.m_tabConditions.Controls.Add(this.m_btnConditionEdit);
			this.m_tabConditions.Controls.Add(this.m_lblConditionsEmptyHint);
			this.m_tabConditions.Controls.Add(this.m_btnConditionDelete);
			this.m_tabConditions.Controls.Add(this.m_btnConditionAdd);
			this.m_tabConditions.Controls.Add(this.m_lblConditionsIntro);
			this.m_tabConditions.Controls.Add(this.m_lvConditions);
			this.m_tabConditions.Location = new System.Drawing.Point(4, 22);
			this.m_tabConditions.Name = "m_tabConditions";
			this.m_tabConditions.Size = new System.Drawing.Size(562, 288);
			this.m_tabConditions.TabIndex = 2;
			this.m_tabConditions.Text = "Conditions";
			this.m_tabConditions.UseVisualStyleBackColor = true;
			// 
			// m_btnConditionMoveDown
			// 
			this.m_btnConditionMoveDown.Image = global::KeePass.Properties.Resources.B16x16_1DownArrow;
			this.m_btnConditionMoveDown.Location = new System.Drawing.Point(478, 161);
			this.m_btnConditionMoveDown.Name = "m_btnConditionMoveDown";
			this.m_btnConditionMoveDown.Size = new System.Drawing.Size(75, 23);
			this.m_btnConditionMoveDown.TabIndex = 6;
			this.m_btnConditionMoveDown.UseVisualStyleBackColor = true;
			this.m_btnConditionMoveDown.Click += new System.EventHandler(this.OnBtnConditionMoveDown);
			// 
			// m_btnConditionMoveUp
			// 
			this.m_btnConditionMoveUp.Image = global::KeePass.Properties.Resources.B16x16_1UpArrow;
			this.m_btnConditionMoveUp.Location = new System.Drawing.Point(478, 132);
			this.m_btnConditionMoveUp.Name = "m_btnConditionMoveUp";
			this.m_btnConditionMoveUp.Size = new System.Drawing.Size(75, 23);
			this.m_btnConditionMoveUp.TabIndex = 5;
			this.m_btnConditionMoveUp.UseVisualStyleBackColor = true;
			this.m_btnConditionMoveUp.Click += new System.EventHandler(this.OnBtnConditionMoveUp);
			// 
			// m_btnConditionEdit
			// 
			this.m_btnConditionEdit.Location = new System.Drawing.Point(478, 55);
			this.m_btnConditionEdit.Name = "m_btnConditionEdit";
			this.m_btnConditionEdit.Size = new System.Drawing.Size(75, 23);
			this.m_btnConditionEdit.TabIndex = 3;
			this.m_btnConditionEdit.Text = "&Edit...";
			this.m_btnConditionEdit.UseVisualStyleBackColor = true;
			this.m_btnConditionEdit.Click += new System.EventHandler(this.OnConditionEdit);
			// 
			// m_lblConditionsEmptyHint
			// 
			this.m_lblConditionsEmptyHint.AutoSize = true;
			this.m_lblConditionsEmptyHint.Location = new System.Drawing.Point(6, 266);
			this.m_lblConditionsEmptyHint.Name = "m_lblConditionsEmptyHint";
			this.m_lblConditionsEmptyHint.Size = new System.Drawing.Size(306, 13);
			this.m_lblConditionsEmptyHint.TabIndex = 7;
			this.m_lblConditionsEmptyHint.Text = "If no conditions are specified, the actions are always performed.";
			// 
			// m_btnConditionDelete
			// 
			this.m_btnConditionDelete.Location = new System.Drawing.Point(478, 84);
			this.m_btnConditionDelete.Name = "m_btnConditionDelete";
			this.m_btnConditionDelete.Size = new System.Drawing.Size(75, 23);
			this.m_btnConditionDelete.TabIndex = 4;
			this.m_btnConditionDelete.Text = "&Delete";
			this.m_btnConditionDelete.UseVisualStyleBackColor = true;
			this.m_btnConditionDelete.Click += new System.EventHandler(this.OnConditionDelete);
			// 
			// m_btnConditionAdd
			// 
			this.m_btnConditionAdd.Location = new System.Drawing.Point(478, 26);
			this.m_btnConditionAdd.Name = "m_btnConditionAdd";
			this.m_btnConditionAdd.Size = new System.Drawing.Size(75, 23);
			this.m_btnConditionAdd.TabIndex = 2;
			this.m_btnConditionAdd.Text = "&Add...";
			this.m_btnConditionAdd.UseVisualStyleBackColor = true;
			this.m_btnConditionAdd.Click += new System.EventHandler(this.OnConditionAdd);
			// 
			// m_lblConditionsIntro
			// 
			this.m_lblConditionsIntro.AutoSize = true;
			this.m_lblConditionsIntro.Location = new System.Drawing.Point(6, 10);
			this.m_lblConditionsIntro.Name = "m_lblConditionsIntro";
			this.m_lblConditionsIntro.Size = new System.Drawing.Size(329, 13);
			this.m_lblConditionsIntro.TabIndex = 0;
			this.m_lblConditionsIntro.Text = "The trigger actions are only performed if all conditions below are met.";
			// 
			// m_lvConditions
			// 
			this.m_lvConditions.FullRowSelect = true;
			this.m_lvConditions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvConditions.HideSelection = false;
			this.m_lvConditions.Location = new System.Drawing.Point(9, 26);
			this.m_lvConditions.Name = "m_lvConditions";
			this.m_lvConditions.ShowItemToolTips = true;
			this.m_lvConditions.Size = new System.Drawing.Size(463, 237);
			this.m_lvConditions.TabIndex = 1;
			this.m_lvConditions.UseCompatibleStateImageBehavior = false;
			this.m_lvConditions.View = System.Windows.Forms.View.Details;
			this.m_lvConditions.ItemActivate += new System.EventHandler(this.OnConditionsItemActivate);
			this.m_lvConditions.SelectedIndexChanged += new System.EventHandler(this.OnConditionsSelectedIndexChanged);
			// 
			// m_tabActions
			// 
			this.m_tabActions.Controls.Add(this.m_btnActionMoveDown);
			this.m_tabActions.Controls.Add(this.m_btnActionMoveUp);
			this.m_tabActions.Controls.Add(this.m_btnActionEdit);
			this.m_tabActions.Controls.Add(this.m_btnActionDelete);
			this.m_tabActions.Controls.Add(this.m_btnActionAdd);
			this.m_tabActions.Controls.Add(this.m_lvActions);
			this.m_tabActions.Controls.Add(this.m_lblActionsIntro);
			this.m_tabActions.Location = new System.Drawing.Point(4, 22);
			this.m_tabActions.Name = "m_tabActions";
			this.m_tabActions.Size = new System.Drawing.Size(562, 288);
			this.m_tabActions.TabIndex = 3;
			this.m_tabActions.Text = "Actions";
			this.m_tabActions.UseVisualStyleBackColor = true;
			// 
			// m_btnActionMoveDown
			// 
			this.m_btnActionMoveDown.Image = global::KeePass.Properties.Resources.B16x16_1DownArrow;
			this.m_btnActionMoveDown.Location = new System.Drawing.Point(478, 161);
			this.m_btnActionMoveDown.Name = "m_btnActionMoveDown";
			this.m_btnActionMoveDown.Size = new System.Drawing.Size(75, 23);
			this.m_btnActionMoveDown.TabIndex = 6;
			this.m_btnActionMoveDown.UseVisualStyleBackColor = true;
			this.m_btnActionMoveDown.Click += new System.EventHandler(this.OnBtnActionMoveDown);
			// 
			// m_btnActionMoveUp
			// 
			this.m_btnActionMoveUp.Image = global::KeePass.Properties.Resources.B16x16_1UpArrow;
			this.m_btnActionMoveUp.Location = new System.Drawing.Point(478, 132);
			this.m_btnActionMoveUp.Name = "m_btnActionMoveUp";
			this.m_btnActionMoveUp.Size = new System.Drawing.Size(75, 23);
			this.m_btnActionMoveUp.TabIndex = 5;
			this.m_btnActionMoveUp.UseVisualStyleBackColor = true;
			this.m_btnActionMoveUp.Click += new System.EventHandler(this.OnBtnActionMoveUp);
			// 
			// m_btnActionEdit
			// 
			this.m_btnActionEdit.Location = new System.Drawing.Point(478, 55);
			this.m_btnActionEdit.Name = "m_btnActionEdit";
			this.m_btnActionEdit.Size = new System.Drawing.Size(75, 23);
			this.m_btnActionEdit.TabIndex = 3;
			this.m_btnActionEdit.Text = "&Edit...";
			this.m_btnActionEdit.UseVisualStyleBackColor = true;
			this.m_btnActionEdit.Click += new System.EventHandler(this.OnActionEdit);
			// 
			// m_btnActionDelete
			// 
			this.m_btnActionDelete.Location = new System.Drawing.Point(478, 84);
			this.m_btnActionDelete.Name = "m_btnActionDelete";
			this.m_btnActionDelete.Size = new System.Drawing.Size(75, 23);
			this.m_btnActionDelete.TabIndex = 4;
			this.m_btnActionDelete.Text = "&Delete";
			this.m_btnActionDelete.UseVisualStyleBackColor = true;
			this.m_btnActionDelete.Click += new System.EventHandler(this.OnActionDelete);
			// 
			// m_btnActionAdd
			// 
			this.m_btnActionAdd.Location = new System.Drawing.Point(478, 26);
			this.m_btnActionAdd.Name = "m_btnActionAdd";
			this.m_btnActionAdd.Size = new System.Drawing.Size(75, 23);
			this.m_btnActionAdd.TabIndex = 2;
			this.m_btnActionAdd.Text = "&Add...";
			this.m_btnActionAdd.UseVisualStyleBackColor = true;
			this.m_btnActionAdd.Click += new System.EventHandler(this.OnActionAdd);
			// 
			// m_lvActions
			// 
			this.m_lvActions.FullRowSelect = true;
			this.m_lvActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_lvActions.HideSelection = false;
			this.m_lvActions.Location = new System.Drawing.Point(9, 26);
			this.m_lvActions.Name = "m_lvActions";
			this.m_lvActions.ShowItemToolTips = true;
			this.m_lvActions.Size = new System.Drawing.Size(463, 253);
			this.m_lvActions.TabIndex = 1;
			this.m_lvActions.UseCompatibleStateImageBehavior = false;
			this.m_lvActions.View = System.Windows.Forms.View.Details;
			this.m_lvActions.ItemActivate += new System.EventHandler(this.OnActionsItemActivate);
			this.m_lvActions.SelectedIndexChanged += new System.EventHandler(this.OnActionsSelectedIndexChanged);
			// 
			// m_lblActionsIntro
			// 
			this.m_lblActionsIntro.AutoSize = true;
			this.m_lblActionsIntro.Location = new System.Drawing.Point(6, 10);
			this.m_lblActionsIntro.Name = "m_lblActionsIntro";
			this.m_lblActionsIntro.Size = new System.Drawing.Size(224, 13);
			this.m_lblActionsIntro.TabIndex = 0;
			this.m_lblActionsIntro.Text = "The trigger will perform all actions listed below.";
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Location = new System.Drawing.Point(12, 389);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
			this.m_btnHelp.TabIndex = 1;
			this.m_btnHelp.Text = "&Help";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.OnBtnHelp);
			// 
			// EcasTriggerForm
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(594, 424);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_tabMain);
			this.Controls.Add(this.m_btnNext);
			this.Controls.Add(this.m_btnPrev);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_bannerImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EcasTriggerForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<>";
			this.Load += new System.EventHandler(this.OnFormLoad);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
			this.m_tabMain.ResumeLayout(false);
			this.m_tabProps.ResumeLayout(false);
			this.m_tabProps.PerformLayout();
			this.m_tabEvents.ResumeLayout(false);
			this.m_tabEvents.PerformLayout();
			this.m_tabConditions.ResumeLayout(false);
			this.m_tabConditions.PerformLayout();
			this.m_tabActions.ResumeLayout(false);
			this.m_tabActions.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox m_bannerImage;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnPrev;
		private System.Windows.Forms.Button m_btnNext;
		private System.Windows.Forms.TabControl m_tabMain;
		private System.Windows.Forms.TabPage m_tabProps;
		private System.Windows.Forms.TabPage m_tabEvents;
		private System.Windows.Forms.TabPage m_tabConditions;
		private System.Windows.Forms.TabPage m_tabActions;
		private System.Windows.Forms.CheckBox m_cbEnabled;
		private System.Windows.Forms.TextBox m_tbName;
		private System.Windows.Forms.Label m_lblTriggerName;
		private System.Windows.Forms.Label m_lblTriggerComments;
		private System.Windows.Forms.Label m_lblInitiallyOnDesc;
		private System.Windows.Forms.CheckBox m_cbInitiallyOn;
		private System.Windows.Forms.TextBox m_tbComments;
		private System.Windows.Forms.Label m_lblEnabledDesc;
		private System.Windows.Forms.CheckBox m_cbTurnOffAfterAction;
		private System.Windows.Forms.Label m_lblEventsIntro;
		private KeePass.UI.CustomListViewEx m_lvEvents;
		private System.Windows.Forms.Button m_btnEventDelete;
		private System.Windows.Forms.Button m_btnEventAdd;
		private KeePass.UI.CustomListViewEx m_lvConditions;
		private System.Windows.Forms.Label m_lblConditionsIntro;
		private System.Windows.Forms.Label m_lblConditionsEmptyHint;
		private System.Windows.Forms.Button m_btnConditionDelete;
		private System.Windows.Forms.Button m_btnConditionAdd;
		private System.Windows.Forms.Button m_btnActionDelete;
		private System.Windows.Forms.Button m_btnActionAdd;
		private KeePass.UI.CustomListViewEx m_lvActions;
		private System.Windows.Forms.Label m_lblActionsIntro;
		private System.Windows.Forms.Button m_btnEventEdit;
		private System.Windows.Forms.Button m_btnConditionEdit;
		private System.Windows.Forms.Button m_btnActionEdit;
		private System.Windows.Forms.Button m_btnEventMoveDown;
		private System.Windows.Forms.Button m_btnEventMoveUp;
		private System.Windows.Forms.Button m_btnConditionMoveDown;
		private System.Windows.Forms.Button m_btnConditionMoveUp;
		private System.Windows.Forms.Button m_btnActionMoveDown;
		private System.Windows.Forms.Button m_btnActionMoveUp;
		private System.Windows.Forms.Button m_btnHelp;
	}
}