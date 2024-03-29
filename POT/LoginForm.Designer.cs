﻿namespace POT
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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(112, 182);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(99, 21);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Remember";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // CancelBT
            // 
            this.CancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBT.Location = new System.Drawing.Point(11, 272);
            this.CancelBT.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CancelBT.Name = "CancelBT";
            this.CancelBT.Size = new System.Drawing.Size(142, 45);
            this.CancelBT.TabIndex = 6;
            this.CancelBT.Text = "Cancel";
            this.CancelBT.UseVisualStyleBackColor = true;
            this.CancelBT.Click += new System.EventHandler(this.CancelBT_Click);
            // 
            // PasswordBX
            // 
            this.PasswordBX.AcceptsTab = true;
            this.PasswordBX.Location = new System.Drawing.Point(112, 156);
            this.PasswordBX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PasswordBX.Name = "PasswordBX";
            this.PasswordBX.PasswordChar = '*';
            this.PasswordBX.Size = new System.Drawing.Size(301, 22);
            this.PasswordBX.TabIndex = 2;
            // 
            // PasswordLB
            // 
            this.PasswordLB.AutoSize = true;
            this.PasswordLB.Location = new System.Drawing.Point(33, 161);
            this.PasswordLB.Name = "PasswordLB";
            this.PasswordLB.Size = new System.Drawing.Size(69, 17);
            this.PasswordLB.TabIndex = 6;
            this.PasswordLB.Text = "Password";
            // 
            // UserenameLB
            // 
            this.UserenameLB.AutoSize = true;
            this.UserenameLB.Location = new System.Drawing.Point(33, 130);
            this.UserenameLB.Name = "UserenameLB";
            this.UserenameLB.Size = new System.Drawing.Size(73, 17);
            this.UserenameLB.TabIndex = 12;
            this.UserenameLB.Text = "Username";
            // 
            // UsernameBX
            // 
            this.UsernameBX.AcceptsTab = true;
            this.UsernameBX.Location = new System.Drawing.Point(112, 125);
            this.UsernameBX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.UsernameBX.Name = "UsernameBX";
            this.UsernameBX.Size = new System.Drawing.Size(301, 22);
            this.UsernameBX.TabIndex = 1;
            // 
            // OkBT
            // 
            this.OkBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkBT.Location = new System.Drawing.Point(292, 272);
            this.OkBT.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OkBT.Name = "OkBT";
            this.OkBT.Size = new System.Drawing.Size(142, 45);
            this.OkBT.TabIndex = 5;
            this.OkBT.Text = "OK";
            this.OkBT.UseVisualStyleBackColor = true;
            this.OkBT.Click += new System.EventHandler(this.OkBT_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(283, 182);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(127, 17);
            this.linkLabel1.TabIndex = 14;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Set DB Connection";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(112, 206);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(153, 21);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "Auto login next time";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-4, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(476, 115);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(139, 247);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 23);
            this.label1.TabIndex = 15;
            this.label1.Text = "Credentials checking ...";
            this.label1.Visible = false;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.OkBT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.CancelBT;
            this.ClientSize = new System.Drawing.Size(463, 326);
            this.Controls.Add(this.OkBT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.CancelBT);
            this.Controls.Add(this.PasswordBX);
            this.Controls.Add(this.PasswordLB);
            this.Controls.Add(this.UserenameLB);
            this.Controls.Add(this.UsernameBX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
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
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label1;
    }
}