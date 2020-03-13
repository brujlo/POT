namespace POT.Documents
{
    partial class WorkListISS
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
            this.saveBT = new System.Windows.Forms.Button();
            this.cancelBT = new System.Windows.Forms.Button();
            this.addBT = new System.Windows.Forms.Button();
            this.deletBT = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.editBT = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // saveBT
            // 
            this.saveBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBT.Location = new System.Drawing.Point(245, 350);
            this.saveBT.Name = "saveBT";
            this.saveBT.Size = new System.Drawing.Size(75, 23);
            this.saveBT.TabIndex = 1;
            this.saveBT.Text = "Save";
            this.saveBT.UseVisualStyleBackColor = true;
            this.saveBT.Click += new System.EventHandler(this.saveBT_Click);
            // 
            // cancelBT
            // 
            this.cancelBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBT.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBT.Location = new System.Drawing.Point(164, 350);
            this.cancelBT.Name = "cancelBT";
            this.cancelBT.Size = new System.Drawing.Size(75, 23);
            this.cancelBT.TabIndex = 2;
            this.cancelBT.Text = "Cancel";
            this.cancelBT.UseVisualStyleBackColor = true;
            // 
            // addBT
            // 
            this.addBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addBT.Location = new System.Drawing.Point(175, 309);
            this.addBT.Name = "addBT";
            this.addBT.Size = new System.Drawing.Size(75, 23);
            this.addBT.TabIndex = 3;
            this.addBT.TabStop = false;
            this.addBT.Text = "Add";
            this.addBT.UseVisualStyleBackColor = true;
            this.addBT.Click += new System.EventHandler(this.addBT_Click);
            // 
            // deletBT
            // 
            this.deletBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deletBT.Location = new System.Drawing.Point(13, 309);
            this.deletBT.Name = "deletBT";
            this.deletBT.Size = new System.Drawing.Size(75, 23);
            this.deletBT.TabIndex = 4;
            this.deletBT.TabStop = false;
            this.deletBT.Text = "Delete";
            this.deletBT.UseVisualStyleBackColor = true;
            this.deletBT.Click += new System.EventHandler(this.deletBT_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(13, 13);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(307, 290);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // editBT
            // 
            this.editBT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editBT.Location = new System.Drawing.Point(94, 309);
            this.editBT.Name = "editBT";
            this.editBT.Size = new System.Drawing.Size(75, 23);
            this.editBT.TabIndex = 6;
            this.editBT.TabStop = false;
            this.editBT.Text = "Edit";
            this.editBT.UseVisualStyleBackColor = true;
            this.editBT.Click += new System.EventHandler(this.editBT_Click);
            // 
            // WorkListISS
            // 
            this.AcceptButton = this.saveBT;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cancelBT;
            this.ClientSize = new System.Drawing.Size(332, 379);
            this.Controls.Add(this.editBT);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.deletBT);
            this.Controls.Add(this.addBT);
            this.Controls.Add(this.cancelBT);
            this.Controls.Add(this.saveBT);
            this.Name = "WorkListISS";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WorkListISS";
            this.Load += new System.EventHandler(this.WorkListISS_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button saveBT;
        private System.Windows.Forms.Button cancelBT;
        private System.Windows.Forms.Button addBT;
        private System.Windows.Forms.Button deletBT;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button editBT;
    }
}