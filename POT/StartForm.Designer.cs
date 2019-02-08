namespace POT
{
    partial class LoginFR
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginFR));
            this.OkBT = new System.Windows.Forms.Button();
            this.UsernameBX = new System.Windows.Forms.TextBox();
            this.UserenameLB = new System.Windows.Forms.Label();
            this.PasswordLB = new System.Windows.Forms.Label();
            this.PasswordBX = new System.Windows.Forms.TextBox();
            this.CancelBT = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.rbHrv = new System.Windows.Forms.RadioButton();
            this.rbEng = new System.Windows.Forms.RadioButton();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // OkBT
            // 
            this.OkBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkBT.Location = new System.Drawing.Point(329, 340);
            this.OkBT.Name = "OkBT";
            this.OkBT.Size = new System.Drawing.Size(160, 56);
            this.OkBT.TabIndex = 5;
            this.OkBT.Text = "OK";
            this.OkBT.UseVisualStyleBackColor = true;
            this.OkBT.Click += new System.EventHandler(this.OkBT_Click_1);
            // 
            // UsernameBX
            // 
            this.UsernameBX.AcceptsTab = true;
            this.UsernameBX.Location = new System.Drawing.Point(127, 156);
            this.UsernameBX.Name = "UsernameBX";
            this.UsernameBX.Size = new System.Drawing.Size(263, 26);
            this.UsernameBX.TabIndex = 1;
            // 
            // UserenameLB
            // 
            this.UserenameLB.AutoSize = true;
            this.UserenameLB.Location = new System.Drawing.Point(38, 162);
            this.UserenameLB.Name = "UserenameLB";
            this.UserenameLB.Size = new System.Drawing.Size(83, 20);
            this.UserenameLB.TabIndex = 5;
            this.UserenameLB.Text = "Username";
            // 
            // PasswordLB
            // 
            this.PasswordLB.AutoSize = true;
            this.PasswordLB.Location = new System.Drawing.Point(38, 201);
            this.PasswordLB.Name = "PasswordLB";
            this.PasswordLB.Size = new System.Drawing.Size(78, 20);
            this.PasswordLB.TabIndex = 0;
            this.PasswordLB.Text = "Password";
            // 
            // PasswordBX
            // 
            this.PasswordBX.AcceptsTab = true;
            this.PasswordBX.Location = new System.Drawing.Point(127, 195);
            this.PasswordBX.Name = "PasswordBX";
            this.PasswordBX.PasswordChar = '*';
            this.PasswordBX.Size = new System.Drawing.Size(263, 26);
            this.PasswordBX.TabIndex = 2;
            // 
            // CancelBT
            // 
            this.CancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBT.Location = new System.Drawing.Point(12, 340);
            this.CancelBT.Name = "CancelBT";
            this.CancelBT.Size = new System.Drawing.Size(160, 56);
            this.CancelBT.TabIndex = 6;
            this.CancelBT.Text = "Cancel";
            this.CancelBT.UseVisualStyleBackColor = true;
            this.CancelBT.Click += new System.EventHandler(this.CancelBT_Click_1);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(127, 228);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(114, 24);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Remember";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-7, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(535, 144);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(319, 228);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(146, 20);
            this.linkLabel1.TabIndex = 15;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Set DB Connection";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // rbHrv
            // 
            this.rbHrv.AutoSize = true;
            this.rbHrv.Enabled = false;
            this.rbHrv.ForeColor = System.Drawing.Color.Red;
            this.rbHrv.Location = new System.Drawing.Point(439, 156);
            this.rbHrv.Name = "rbHrv";
            this.rbHrv.Size = new System.Drawing.Size(58, 24);
            this.rbHrv.TabIndex = 16;
            this.rbHrv.Text = "Hrv";
            this.rbHrv.UseVisualStyleBackColor = true;
            // 
            // rbEng
            // 
            this.rbEng.AutoSize = true;
            this.rbEng.Checked = true;
            this.rbEng.Enabled = false;
            this.rbEng.ForeColor = System.Drawing.Color.Red;
            this.rbEng.Location = new System.Drawing.Point(439, 195);
            this.rbEng.Name = "rbEng";
            this.rbEng.Size = new System.Drawing.Size(63, 24);
            this.rbEng.TabIndex = 17;
            this.rbEng.TabStop = true;
            this.rbEng.Text = "Eng";
            this.rbEng.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(127, 258);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(174, 24);
            this.checkBox2.TabIndex = 18;
            this.checkBox2.Text = "Auto login next time";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // LoginFR
            // 
            this.AcceptButton = this.OkBT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.CancelBT;
            this.ClientSize = new System.Drawing.Size(521, 408);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.rbEng);
            this.Controls.Add(this.rbHrv);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.CancelBT);
            this.Controls.Add(this.PasswordBX);
            this.Controls.Add(this.PasswordLB);
            this.Controls.Add(this.UserenameLB);
            this.Controls.Add(this.UsernameBX);
            this.Controls.Add(this.OkBT);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginFR";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Credential window POT";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkBT;
        private System.Windows.Forms.TextBox UsernameBX;
        private System.Windows.Forms.Label UserenameLB;
        private System.Windows.Forms.Label PasswordLB;
        private System.Windows.Forms.TextBox PasswordBX;
        private System.Windows.Forms.Button CancelBT;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.RadioButton rbHrv;
        private System.Windows.Forms.RadioButton rbEng;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}

