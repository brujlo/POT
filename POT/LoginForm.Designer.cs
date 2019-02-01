namespace POT
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.CancelBT = new System.Windows.Forms.Button();
            this.PasswordBX = new System.Windows.Forms.TextBox();
            this.PasswordLB = new System.Windows.Forms.Label();
            this.UserenameLB = new System.Windows.Forms.Label();
            this.UsernameBX = new System.Windows.Forms.TextBox();
            this.OkBT = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(126, 228);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(114, 24);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Remember";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // CancelBT
            // 
            this.CancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBT.Location = new System.Drawing.Point(12, 276);
            this.CancelBT.Name = "CancelBT";
            this.CancelBT.Size = new System.Drawing.Size(160, 56);
            this.CancelBT.TabIndex = 11;
            this.CancelBT.Text = "Cancel";
            this.CancelBT.UseVisualStyleBackColor = true;
            this.CancelBT.Click += new System.EventHandler(this.CancelBT_Click);
            // 
            // PasswordBX
            // 
            this.PasswordBX.AcceptsTab = true;
            this.PasswordBX.Location = new System.Drawing.Point(126, 195);
            this.PasswordBX.Name = "PasswordBX";
            this.PasswordBX.PasswordChar = '*';
            this.PasswordBX.Size = new System.Drawing.Size(338, 26);
            this.PasswordBX.TabIndex = 8;
            // 
            // PasswordLB
            // 
            this.PasswordLB.AutoSize = true;
            this.PasswordLB.Location = new System.Drawing.Point(37, 201);
            this.PasswordLB.Name = "PasswordLB";
            this.PasswordLB.Size = new System.Drawing.Size(78, 20);
            this.PasswordLB.TabIndex = 6;
            this.PasswordLB.Text = "Password";
            // 
            // UserenameLB
            // 
            this.UserenameLB.AutoSize = true;
            this.UserenameLB.Location = new System.Drawing.Point(37, 162);
            this.UserenameLB.Name = "UserenameLB";
            this.UserenameLB.Size = new System.Drawing.Size(83, 20);
            this.UserenameLB.TabIndex = 12;
            this.UserenameLB.Text = "Username";
            // 
            // UsernameBX
            // 
            this.UsernameBX.AcceptsTab = true;
            this.UsernameBX.Location = new System.Drawing.Point(126, 156);
            this.UsernameBX.Name = "UsernameBX";
            this.UsernameBX.Size = new System.Drawing.Size(338, 26);
            this.UsernameBX.TabIndex = 7;
            // 
            // OkBT
            // 
            this.OkBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkBT.Location = new System.Drawing.Point(329, 276);
            this.OkBT.Name = "OkBT";
            this.OkBT.Size = new System.Drawing.Size(160, 56);
            this.OkBT.TabIndex = 10;
            this.OkBT.Text = "OK";
            this.OkBT.UseVisualStyleBackColor = true;
            this.OkBT.Click += new System.EventHandler(this.OkBT_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-5, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(535, 144);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(318, 228);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(146, 20);
            this.linkLabel1.TabIndex = 14;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Set DB Connection";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(523, 348);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.CancelBT);
            this.Controls.Add(this.PasswordBX);
            this.Controls.Add(this.PasswordLB);
            this.Controls.Add(this.UserenameLB);
            this.Controls.Add(this.UsernameBX);
            this.Controls.Add(this.OkBT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Credential window POT";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button CancelBT;
        private System.Windows.Forms.TextBox PasswordBX;
        private System.Windows.Forms.Label PasswordLB;
        private System.Windows.Forms.Label UserenameLB;
        private System.Windows.Forms.TextBox UsernameBX;
        private System.Windows.Forms.Button OkBT;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}