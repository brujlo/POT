namespace POT.WorkingClasses
{
    partial class ISSSelector
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
            this.PartSelectorCB = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PartSelectorCB
            // 
            this.PartSelectorCB.FormattingEnabled = true;
            this.PartSelectorCB.Location = new System.Drawing.Point(12, 34);
            this.PartSelectorCB.Name = "PartSelectorCB";
            this.PartSelectorCB.Size = new System.Drawing.Size(379, 28);
            this.PartSelectorCB.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(265, 79);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 41);
            this.button1.TabIndex = 1;
            this.button1.Text = "Select";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ISSSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 132);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PartSelectorCB);
            this.Name = "ISSSelector";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select part";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ISSSelector_FormClosing);
            this.Load += new System.EventHandler(this.ISSSelector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox PartSelectorCB;
        private System.Windows.Forms.Button button1;
    }
}