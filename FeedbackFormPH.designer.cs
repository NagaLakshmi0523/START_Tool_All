
namespace START_Tool
{
    partial class FeedbackFormPH
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
            if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedbackFormPH));
            this.label1 = new System.Windows.Forms.Label();
            this.lblSubCat = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.RichTextBox();
            this.rdbtnYes = new System.Windows.Forms.RadioButton();
            this.rdbtnNo = new System.Windows.Forms.RadioButton();
            this.lblUser = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbLocation = new System.Windows.Forms.ComboBox();
            this.panelafterno = new System.Windows.Forms.Panel();
            this.linkLabelchatbot = new System.Windows.Forms.LinkLabel();
            this.linkLabelloginc = new System.Windows.Forms.LinkLabel();
            this.lblCat = new System.Windows.Forms.Label();
            this.lblsubcategory = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdbtnExc = new System.Windows.Forms.RadioButton();
            this.rdbtnGood = new System.Windows.Forms.RadioButton();
            this.rdbtnFair = new System.Windows.Forms.RadioButton();
            this.rdbtnAvg = new System.Windows.Forms.RadioButton();
            this.rdbtnPoor = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panelafterno.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(76, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name";
            // 
            // lblSubCat
            // 
            this.lblSubCat.AutoSize = true;
            this.lblSubCat.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubCat.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblSubCat.Location = new System.Drawing.Point(1088, 472);
            this.lblSubCat.Name = "lblSubCat";
            this.lblSubCat.Size = new System.Drawing.Size(160, 29);
            this.lblSubCat.TabIndex = 1;
            this.lblSubCat.Text = "Sub-Category";
            this.lblSubCat.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(1126, 412);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 29);
            this.label3.TabIndex = 2;
            this.label3.Text = "Category";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(0, 458);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(221, 58);
            this.label4.TabIndex = 3;
            this.label4.Text = "Provide Comments/\r\n remarks if any\r\n";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label6.Location = new System.Drawing.Point(28, 285);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(181, 29);
            this.label6.TabIndex = 5;
            this.label6.Text = "Issue Resolved?";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnSave.FlatAppearance.BorderSize = 10;
            this.btnSave.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSave.Location = new System.Drawing.Point(363, 632);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(270, 60);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Submit Feedback";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtComments
            // 
            this.txtComments.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComments.Location = new System.Drawing.Point(267, 440);
            this.txtComments.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtComments.MaxLength = 100;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(538, 146);
            this.txtComments.TabIndex = 14;
            this.txtComments.Text = "";
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            this.txtComments.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtComments_KeyPress);
            // 
            // rdbtnYes
            // 
            this.rdbtnYes.AutoSize = true;
            this.rdbtnYes.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnYes.Location = new System.Drawing.Point(24, 0);
            this.rdbtnYes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnYes.Name = "rdbtnYes";
            this.rdbtnYes.Size = new System.Drawing.Size(74, 33);
            this.rdbtnYes.TabIndex = 15;
            this.rdbtnYes.TabStop = true;
            this.rdbtnYes.Text = "Yes";
            this.rdbtnYes.UseVisualStyleBackColor = true;
            // 
            // rdbtnNo
            // 
            this.rdbtnNo.AutoSize = true;
            this.rdbtnNo.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnNo.Location = new System.Drawing.Point(219, 0);
            this.rdbtnNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnNo.Name = "rdbtnNo";
            this.rdbtnNo.Size = new System.Drawing.Size(67, 33);
            this.rdbtnNo.TabIndex = 16;
            this.rdbtnNo.TabStop = true;
            this.rdbtnNo.Text = "No";
            this.rdbtnNo.UseVisualStyleBackColor = true;
            this.rdbtnNo.CheckedChanged += new System.EventHandler(this.rdbtnNo_CheckedChanged);
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblUser.Location = new System.Drawing.Point(262, 31);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(0, 29);
            this.lblUser.TabIndex = 19;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbtnYes);
            this.groupBox1.Controls.Add(this.rdbtnNo);
            this.groupBox1.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(267, 285);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(327, 52);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label7.Location = new System.Drawing.Point(18, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(185, 29);
            this.label7.TabIndex = 21;
            this.label7.Text = "Location/Facility";
            // 
            // cbLocation
            // 
            this.cbLocation.BackColor = System.Drawing.Color.White;
            this.cbLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLocation.Font = new System.Drawing.Font("Graphik Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLocation.FormattingEnabled = true;
            this.cbLocation.Items.AddRange(new object[] {
            "Philippines-Cebu-Ebloc 2",
            "Philippines-Cebu-Cyberzone 1",
            "Philippines-Cebu-Cyberzone 2",
            "Philippines-Cebu-Cybergate",
            "Philippines-Cebu-Pioneer House",
            "Philippines-Manila-EW Global 1",
            "Philippines-Manila-Gateway 2",
            "Philippines-Manila-Cyberpark 1",
            "Philippines-Manila-Axis 1",
            "Philippines-Manila-Science Hub",
            "Philippines-Manila-Uptown 2",
            "Philippines-Manila-Uptown 3",
            "Philippines-Manila-Cybergate 1",
            "Philippines-Manila-Cybergate 2",
            "Philippines-Manila-Cybergate 3",
            "Philippines-Ilocos-San Nicolas",
            "Others"});
            this.cbLocation.Location = new System.Drawing.Point(267, 102);
            this.cbLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbLocation.Name = "cbLocation";
            this.cbLocation.Size = new System.Drawing.Size(345, 30);
            this.cbLocation.TabIndex = 22;
            this.cbLocation.SelectedIndexChanged += new System.EventHandler(this.cbLocation_SelectedIndexChanged);
            // 
            // panelafterno
            // 
            this.panelafterno.Controls.Add(this.linkLabelchatbot);
            this.panelafterno.Controls.Add(this.linkLabelloginc);
            this.panelafterno.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelafterno.Location = new System.Drawing.Point(600, 285);
            this.panelafterno.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelafterno.Name = "panelafterno";
            this.panelafterno.Size = new System.Drawing.Size(313, 61);
            this.panelafterno.TabIndex = 26;
            this.panelafterno.Visible = false;
            // 
            // linkLabelchatbot
            // 
            this.linkLabelchatbot.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkLabelchatbot.AutoSize = true;
            this.linkLabelchatbot.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.linkLabelchatbot.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelchatbot.LinkColor = System.Drawing.Color.DodgerBlue;
            this.linkLabelchatbot.Location = new System.Drawing.Point(21, 85);
            this.linkLabelchatbot.Name = "linkLabelchatbot";
            this.linkLabelchatbot.Size = new System.Drawing.Size(288, 58);
            this.linkLabelchatbot.TabIndex = 1;
            this.linkLabelchatbot.TabStop = true;
            this.linkLabelchatbot.Text = "TALK TO MyTechHelp BOT \r\n\r\n";
            this.linkLabelchatbot.Visible = false;
            this.linkLabelchatbot.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelchatbot.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelchatbot_LinkClicked);
            // 
            // linkLabelloginc
            // 
            this.linkLabelloginc.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkLabelloginc.AutoSize = true;
            this.linkLabelloginc.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.linkLabelloginc.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelloginc.LinkColor = System.Drawing.Color.DodgerBlue;
            this.linkLabelloginc.Location = new System.Drawing.Point(21, 28);
            this.linkLabelloginc.Name = "linkLabelloginc";
            this.linkLabelloginc.Size = new System.Drawing.Size(244, 29);
            this.linkLabelloginc.TabIndex = 0;
            this.linkLabelloginc.TabStop = true;
            this.linkLabelloginc.Text = "CREATE AN INCIDENT";
            this.linkLabelloginc.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelloginc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelloginc_LinkClicked);
            // 
            // lblCat
            // 
            this.lblCat.AutoSize = true;
            this.lblCat.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCat.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCat.Location = new System.Drawing.Point(1310, 429);
            this.lblCat.Name = "lblCat";
            this.lblCat.Size = new System.Drawing.Size(0, 29);
            this.lblCat.TabIndex = 27;
            this.lblCat.Visible = false;
            // 
            // lblsubcategory
            // 
            this.lblsubcategory.AutoSize = true;
            this.lblsubcategory.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblsubcategory.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblsubcategory.Location = new System.Drawing.Point(1297, 478);
            this.lblsubcategory.Name = "lblsubcategory";
            this.lblsubcategory.Size = new System.Drawing.Size(0, 29);
            this.lblsubcategory.TabIndex = 28;
            this.lblsubcategory.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(122, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 29);
            this.label2.TabIndex = 29;
            this.label2.Text = "Rating ";
            // 
            // rdbtnExc
            // 
            this.rdbtnExc.AutoSize = true;
            this.rdbtnExc.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnExc.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rdbtnExc.Location = new System.Drawing.Point(267, 199);
            this.rdbtnExc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnExc.Name = "rdbtnExc";
            this.rdbtnExc.Size = new System.Drawing.Size(134, 33);
            this.rdbtnExc.TabIndex = 30;
            this.rdbtnExc.TabStop = true;
            this.rdbtnExc.Text = "Excellent";
            this.rdbtnExc.UseVisualStyleBackColor = true;
            this.rdbtnExc.CheckedChanged += new System.EventHandler(this.rdbtnExc_CheckedChanged);
            // 
            // rdbtnGood
            // 
            this.rdbtnGood.AutoSize = true;
            this.rdbtnGood.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnGood.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rdbtnGood.Location = new System.Drawing.Point(410, 200);
            this.rdbtnGood.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnGood.Name = "rdbtnGood";
            this.rdbtnGood.Size = new System.Drawing.Size(95, 33);
            this.rdbtnGood.TabIndex = 31;
            this.rdbtnGood.TabStop = true;
            this.rdbtnGood.Text = "Good";
            this.rdbtnGood.UseVisualStyleBackColor = true;
            // 
            // rdbtnFair
            // 
            this.rdbtnFair.AutoSize = true;
            this.rdbtnFair.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnFair.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rdbtnFair.Location = new System.Drawing.Point(657, 200);
            this.rdbtnFair.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnFair.Name = "rdbtnFair";
            this.rdbtnFair.Size = new System.Drawing.Size(76, 33);
            this.rdbtnFair.TabIndex = 32;
            this.rdbtnFair.TabStop = true;
            this.rdbtnFair.Text = "Fair";
            this.rdbtnFair.UseVisualStyleBackColor = true;
            // 
            // rdbtnAvg
            // 
            this.rdbtnAvg.AutoSize = true;
            this.rdbtnAvg.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnAvg.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rdbtnAvg.Location = new System.Drawing.Point(519, 199);
            this.rdbtnAvg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnAvg.Name = "rdbtnAvg";
            this.rdbtnAvg.Size = new System.Drawing.Size(126, 33);
            this.rdbtnAvg.TabIndex = 33;
            this.rdbtnAvg.TabStop = true;
            this.rdbtnAvg.Text = "Average";
            this.rdbtnAvg.UseVisualStyleBackColor = true;
            // 
            // rdbtnPoor
            // 
            this.rdbtnPoor.AutoSize = true;
            this.rdbtnPoor.Font = new System.Drawing.Font("Graphik", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbtnPoor.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rdbtnPoor.Location = new System.Drawing.Point(744, 199);
            this.rdbtnPoor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdbtnPoor.Name = "rdbtnPoor";
            this.rdbtnPoor.Size = new System.Drawing.Size(87, 33);
            this.rdbtnPoor.TabIndex = 34;
            this.rdbtnPoor.TabStop = true;
            this.rdbtnPoor.Text = "Poor";
            this.rdbtnPoor.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Font = new System.Drawing.Font("Graphik", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Yellow;
            this.label5.Location = new System.Drawing.Point(29, 370);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(904, 42);
            this.label5.TabIndex = 37;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // FeedbackFormPH
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1329, 805);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rdbtnPoor);
            this.Controls.Add(this.rdbtnAvg);
            this.Controls.Add(this.rdbtnFair);
            this.Controls.Add(this.rdbtnGood);
            this.Controls.Add(this.rdbtnExc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblsubcategory);
            this.Controls.Add(this.lblCat);
            this.Controls.Add(this.panelafterno);
            this.Controls.Add(this.cbLocation);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblSubCat);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FeedbackFormPH";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = resources.GetString("$this.Text");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FeedbackFormPH_FormClosing);
            this.Load += new System.EventHandler(this.FeedbackForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelafterno.ResumeLayout(false);
            this.panelafterno.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSubCat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RichTextBox txtComments;
        private System.Windows.Forms.RadioButton rdbtnYes;
        private System.Windows.Forms.RadioButton rdbtnNo;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbLocation;
        private System.Windows.Forms.Panel panelafterno;
        private System.Windows.Forms.LinkLabel linkLabelchatbot;
        private System.Windows.Forms.LinkLabel linkLabelloginc;
        private System.Windows.Forms.Label lblsubcategory;
        private System.Windows.Forms.Label lblCat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdbtnExc;
        private System.Windows.Forms.RadioButton rdbtnGood;
        private System.Windows.Forms.RadioButton rdbtnFair;
        private System.Windows.Forms.RadioButton rdbtnAvg;
        private System.Windows.Forms.RadioButton rdbtnPoor;
        private System.Windows.Forms.Label label5;
    }
}