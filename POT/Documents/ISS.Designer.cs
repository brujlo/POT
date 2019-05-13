namespace POT.Documents
{
    partial class ISS
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ISS));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.PAUSEbt = new System.Windows.Forms.Button();
            this.STARTbt = new System.Windows.Forms.Button();
            this.STOPbt = new System.Windows.Forms.Button();
            this.PartCb = new System.Windows.Forms.ComboBox();
            this.NameTb = new System.Windows.Forms.TextBox();
            this.DateInTb = new System.Windows.Forms.TextBox();
            this.DateSentTb = new System.Windows.Forms.TextBox();
            this.SNTb = new System.Windows.Forms.TextBox();
            this.CNTb = new System.Windows.Forms.TextBox();
            this.IDTb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.WorkDoneCb = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.OldPartCodeTb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.OldPartCb = new System.Windows.Forms.ComboBox();
            this.label55 = new System.Windows.Forms.Label();
            this.NewPartCb = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.NewPartCodeTb = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.NewPartCNTb = new System.Windows.Forms.TextBox();
            this.NewPartSNCb = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.OldPartCNTb = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.OLDPartSNTb = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ComentTb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.ISSSelectorCb = new System.Windows.Forms.ComboBox();
            this.SAVEbt = new System.Windows.Forms.Button();
            this.CANCELBt = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.TIMERtb = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.PRINTbt = new System.Windows.Forms.Button();
            this.SelectPrinterbt = new System.Windows.Forms.Button();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PAUSEbt
            // 
            this.PAUSEbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PAUSEbt.BackColor = System.Drawing.Color.AliceBlue;
            this.PAUSEbt.Location = new System.Drawing.Point(713, 78);
            this.PAUSEbt.Name = "PAUSEbt";
            this.PAUSEbt.Size = new System.Drawing.Size(112, 44);
            this.PAUSEbt.TabIndex = 155;
            this.PAUSEbt.TabStop = false;
            this.PAUSEbt.Text = "PAUSE";
            this.PAUSEbt.UseVisualStyleBackColor = false;
            this.PAUSEbt.Click += new System.EventHandler(this.PAUSEbt_Click);
            // 
            // STARTbt
            // 
            this.STARTbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.STARTbt.BackColor = System.Drawing.Color.AliceBlue;
            this.STARTbt.Location = new System.Drawing.Point(854, 78);
            this.STARTbt.Name = "STARTbt";
            this.STARTbt.Size = new System.Drawing.Size(112, 44);
            this.STARTbt.TabIndex = 2;
            this.STARTbt.TabStop = false;
            this.STARTbt.Text = "START";
            this.STARTbt.UseVisualStyleBackColor = false;
            this.STARTbt.Click += new System.EventHandler(this.STARTbt_Click);
            // 
            // STOPbt
            // 
            this.STOPbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.STOPbt.BackColor = System.Drawing.Color.AliceBlue;
            this.STOPbt.Location = new System.Drawing.Point(854, 146);
            this.STOPbt.Name = "STOPbt";
            this.STOPbt.Size = new System.Drawing.Size(112, 44);
            this.STOPbt.TabIndex = 3;
            this.STOPbt.TabStop = false;
            this.STOPbt.Text = "STOP";
            this.STOPbt.UseVisualStyleBackColor = false;
            this.STOPbt.Click += new System.EventHandler(this.STOPbt_Click);
            // 
            // PartCb
            // 
            this.PartCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PartCb.FormattingEnabled = true;
            this.PartCb.Location = new System.Drawing.Point(279, 23);
            this.PartCb.Name = "PartCb";
            this.PartCb.Size = new System.Drawing.Size(238, 28);
            this.PartCb.TabIndex = 0;
            this.PartCb.SelectedIndexChanged += new System.EventHandler(this.PartCb_SelectedIndexChanged);
            // 
            // NameTb
            // 
            this.NameTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTb.BackColor = System.Drawing.Color.PapayaWhip;
            this.NameTb.Location = new System.Drawing.Point(279, 78);
            this.NameTb.Name = "NameTb";
            this.NameTb.ReadOnly = true;
            this.NameTb.Size = new System.Drawing.Size(243, 26);
            this.NameTb.TabIndex = 106;
            this.NameTb.TabStop = false;
            // 
            // DateInTb
            // 
            this.DateInTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DateInTb.BackColor = System.Drawing.Color.PapayaWhip;
            this.DateInTb.Location = new System.Drawing.Point(558, 78);
            this.DateInTb.Name = "DateInTb";
            this.DateInTb.ReadOnly = true;
            this.DateInTb.Size = new System.Drawing.Size(96, 26);
            this.DateInTb.TabIndex = 107;
            this.DateInTb.TabStop = false;
            // 
            // DateSentTb
            // 
            this.DateSentTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DateSentTb.BackColor = System.Drawing.Color.PapayaWhip;
            this.DateSentTb.Location = new System.Drawing.Point(558, 140);
            this.DateSentTb.Name = "DateSentTb";
            this.DateSentTb.ReadOnly = true;
            this.DateSentTb.Size = new System.Drawing.Size(96, 26);
            this.DateSentTb.TabIndex = 108;
            this.DateSentTb.TabStop = false;
            // 
            // SNTb
            // 
            this.SNTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SNTb.BackColor = System.Drawing.Color.PapayaWhip;
            this.SNTb.Location = new System.Drawing.Point(279, 140);
            this.SNTb.Name = "SNTb";
            this.SNTb.ReadOnly = true;
            this.SNTb.Size = new System.Drawing.Size(243, 26);
            this.SNTb.TabIndex = 109;
            this.SNTb.TabStop = false;
            // 
            // CNTb
            // 
            this.CNTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CNTb.BackColor = System.Drawing.Color.PapayaWhip;
            this.CNTb.Location = new System.Drawing.Point(279, 202);
            this.CNTb.Name = "CNTb";
            this.CNTb.ReadOnly = true;
            this.CNTb.Size = new System.Drawing.Size(243, 26);
            this.CNTb.TabIndex = 110;
            this.CNTb.TabStop = false;
            // 
            // IDTb
            // 
            this.IDTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IDTb.BackColor = System.Drawing.Color.PapayaWhip;
            this.IDTb.Location = new System.Drawing.Point(558, 202);
            this.IDTb.Name = "IDTb";
            this.IDTb.ReadOnly = true;
            this.IDTb.Size = new System.Drawing.Size(96, 26);
            this.IDTb.TabIndex = 111;
            this.IDTb.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(275, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 112;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(554, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 20);
            this.label2.TabIndex = 113;
            this.label2.Text = "Date in";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(275, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 20);
            this.label3.TabIndex = 114;
            this.label3.Text = "SN";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Location = new System.Drawing.Point(275, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 20);
            this.label4.TabIndex = 115;
            this.label4.Text = "CN";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(554, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 20);
            this.label5.TabIndex = 116;
            this.label5.Text = "Date sent";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Location = new System.Drawing.Point(554, 183);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 20);
            this.label6.TabIndex = 117;
            this.label6.Text = "ID";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(275, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 20);
            this.label7.TabIndex = 118;
            this.label7.Text = "Part code";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 252);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 20);
            this.label11.TabIndex = 120;
            this.label11.Text = "Work list";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BackColor = System.Drawing.Color.Pink;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.listView1.Location = new System.Drawing.Point(13, 286);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(944, 147);
            this.listView1.TabIndex = 119;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // WorkDoneCb
            // 
            this.WorkDoneCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WorkDoneCb.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.WorkDoneCb.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.WorkDoneCb.BackColor = System.Drawing.Color.White;
            this.WorkDoneCb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.WorkDoneCb.FormattingEnabled = true;
            this.WorkDoneCb.Location = new System.Drawing.Point(87, 245);
            this.WorkDoneCb.Margin = new System.Windows.Forms.Padding(4);
            this.WorkDoneCb.Name = "WorkDoneCb";
            this.WorkDoneCb.Size = new System.Drawing.Size(567, 33);
            this.WorkDoneCb.TabIndex = 1;
            this.WorkDoneCb.TabStop = false;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Gray;
            this.label9.Location = new System.Drawing.Point(432, 448);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 20);
            this.label9.TabIndex = 125;
            this.label9.Text = "Code";
            // 
            // OldPartCodeTb
            // 
            this.OldPartCodeTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OldPartCodeTb.BackColor = System.Drawing.Color.PaleTurquoise;
            this.OldPartCodeTb.Location = new System.Drawing.Point(437, 467);
            this.OldPartCodeTb.Name = "OldPartCodeTb";
            this.OldPartCodeTb.ReadOnly = true;
            this.OldPartCodeTb.Size = new System.Drawing.Size(100, 26);
            this.OldPartCodeTb.TabIndex = 124;
            this.OldPartCodeTb.TabStop = false;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Gray;
            this.label10.Location = new System.Drawing.Point(10, 446);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 20);
            this.label10.TabIndex = 127;
            this.label10.Text = "Old part name";
            // 
            // OldPartCb
            // 
            this.OldPartCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OldPartCb.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.OldPartCb.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.OldPartCb.FormattingEnabled = true;
            this.OldPartCb.Location = new System.Drawing.Point(14, 465);
            this.OldPartCb.Name = "OldPartCb";
            this.OldPartCb.Size = new System.Drawing.Size(413, 28);
            this.OldPartCb.TabIndex = 2;
            this.OldPartCb.SelectedIndexChanged += new System.EventHandler(this.OldPartCb_SelectedIndexChanged);
            // 
            // label55
            // 
            this.label55.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label55.AutoSize = true;
            this.label55.ForeColor = System.Drawing.Color.Gray;
            this.label55.Location = new System.Drawing.Point(10, 505);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(116, 20);
            this.label55.TabIndex = 131;
            this.label55.Text = "New part name";
            // 
            // NewPartCb
            // 
            this.NewPartCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NewPartCb.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.NewPartCb.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.NewPartCb.FormattingEnabled = true;
            this.NewPartCb.Location = new System.Drawing.Point(14, 524);
            this.NewPartCb.Name = "NewPartCb";
            this.NewPartCb.Size = new System.Drawing.Size(413, 28);
            this.NewPartCb.TabIndex = 5;
            this.NewPartCb.SelectedIndexChanged += new System.EventHandler(this.NewPartCb_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Gray;
            this.label13.Location = new System.Drawing.Point(433, 507);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 20);
            this.label13.TabIndex = 129;
            this.label13.Text = "Code";
            // 
            // NewPartCodeTb
            // 
            this.NewPartCodeTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NewPartCodeTb.BackColor = System.Drawing.Color.PaleTurquoise;
            this.NewPartCodeTb.Location = new System.Drawing.Point(437, 526);
            this.NewPartCodeTb.Name = "NewPartCodeTb";
            this.NewPartCodeTb.ReadOnly = true;
            this.NewPartCodeTb.Size = new System.Drawing.Size(100, 26);
            this.NewPartCodeTb.TabIndex = 128;
            this.NewPartCodeTb.TabStop = false;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.Gray;
            this.label12.Location = new System.Drawing.Point(540, 507);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 20);
            this.label12.TabIndex = 133;
            this.label12.Text = "SN";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.Gray;
            this.label14.Location = new System.Drawing.Point(766, 507);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 20);
            this.label14.TabIndex = 135;
            this.label14.Text = "CN";
            // 
            // NewPartCNTb
            // 
            this.NewPartCNTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NewPartCNTb.BackColor = System.Drawing.Color.PaleTurquoise;
            this.NewPartCNTb.Location = new System.Drawing.Point(770, 526);
            this.NewPartCNTb.Name = "NewPartCNTb";
            this.NewPartCNTb.ReadOnly = true;
            this.NewPartCNTb.Size = new System.Drawing.Size(200, 26);
            this.NewPartCNTb.TabIndex = 134;
            this.NewPartCNTb.TabStop = false;
            // 
            // NewPartSNCb
            // 
            this.NewPartSNCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NewPartSNCb.BackColor = System.Drawing.Color.White;
            this.NewPartSNCb.FormattingEnabled = true;
            this.NewPartSNCb.Location = new System.Drawing.Point(543, 524);
            this.NewPartSNCb.Name = "NewPartSNCb";
            this.NewPartSNCb.Size = new System.Drawing.Size(215, 28);
            this.NewPartSNCb.TabIndex = 6;
            this.NewPartSNCb.SelectedIndexChanged += new System.EventHandler(this.NewPartSNCb_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.Gray;
            this.label15.Location = new System.Drawing.Point(766, 448);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(31, 20);
            this.label15.TabIndex = 139;
            this.label15.Text = "CN";
            // 
            // OldPartCNTb
            // 
            this.OldPartCNTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OldPartCNTb.BackColor = System.Drawing.Color.White;
            this.OldPartCNTb.Location = new System.Drawing.Point(770, 467);
            this.OldPartCNTb.Name = "OldPartCNTb";
            this.OldPartCNTb.Size = new System.Drawing.Size(200, 26);
            this.OldPartCNTb.TabIndex = 4;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.Color.Gray;
            this.label16.Location = new System.Drawing.Point(543, 448);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(31, 20);
            this.label16.TabIndex = 137;
            this.label16.Text = "SN";
            // 
            // OLDPartSNTb
            // 
            this.OLDPartSNTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OLDPartSNTb.BackColor = System.Drawing.Color.White;
            this.OLDPartSNTb.Location = new System.Drawing.Point(544, 467);
            this.OLDPartSNTb.Name = "OLDPartSNTb";
            this.OLDPartSNTb.Size = new System.Drawing.Size(214, 26);
            this.OLDPartSNTb.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.DimGray;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(3, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(237, 107);
            this.textBox1.TabIndex = 141;
            this.textBox1.TabStop = false;
            // 
            // ComentTb
            // 
            this.ComentTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ComentTb.BackColor = System.Drawing.Color.White;
            this.ComentTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ComentTb.ForeColor = System.Drawing.Color.Black;
            this.ComentTb.Location = new System.Drawing.Point(15, 587);
            this.ComentTb.Multiline = true;
            this.ComentTb.Name = "ComentTb";
            this.ComentTb.Size = new System.Drawing.Size(448, 65);
            this.ComentTb.TabIndex = 7;
            this.ComentTb.TabStop = false;
            this.ComentTb.TextChanged += new System.EventHandler(this.ComentTb_TextChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(11, 568);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 20);
            this.label8.TabIndex = 157;
            this.label8.Text = "Comment";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.Color.Crimson;
            this.label17.Location = new System.Drawing.Point(351, 568);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(116, 20);
            this.label17.TabIndex = 158;
            this.label17.Text = "Letters left 200";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.Color.Gray;
            this.label18.Location = new System.Drawing.Point(-1, 121);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(85, 20);
            this.label18.TabIndex = 160;
            this.label18.Text = "Select ISS";
            // 
            // ISSSelectorCb
            // 
            this.ISSSelectorCb.FormattingEnabled = true;
            this.ISSSelectorCb.Location = new System.Drawing.Point(3, 140);
            this.ISSSelectorCb.Name = "ISSSelectorCb";
            this.ISSSelectorCb.Size = new System.Drawing.Size(238, 28);
            this.ISSSelectorCb.TabIndex = 159;
            this.ISSSelectorCb.TabStop = false;
            this.ISSSelectorCb.SelectedIndexChanged += new System.EventHandler(this.ISSSelectorCb_SelectedIndexChanged);
            // 
            // SAVEbt
            // 
            this.SAVEbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SAVEbt.BackColor = System.Drawing.Color.LightSkyBlue;
            this.SAVEbt.Location = new System.Drawing.Point(843, 613);
            this.SAVEbt.Name = "SAVEbt";
            this.SAVEbt.Size = new System.Drawing.Size(112, 44);
            this.SAVEbt.TabIndex = 8;
            this.SAVEbt.Text = "Save";
            this.SAVEbt.UseVisualStyleBackColor = false;
            this.SAVEbt.Click += new System.EventHandler(this.button1_Click);
            // 
            // CANCELBt
            // 
            this.CANCELBt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CANCELBt.BackColor = System.Drawing.Color.AliceBlue;
            this.CANCELBt.Enabled = false;
            this.CANCELBt.Location = new System.Drawing.Point(713, 146);
            this.CANCELBt.Name = "CANCELBt";
            this.CANCELBt.Size = new System.Drawing.Size(112, 44);
            this.CANCELBt.TabIndex = 162;
            this.CANCELBt.TabStop = false;
            this.CANCELBt.Text = "CANCEL";
            this.CANCELBt.UseVisualStyleBackColor = false;
            this.CANCELBt.Click += new System.EventHandler(this.CANCELBt_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(679, 254);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(278, 24);
            this.checkBox1.TabIndex = 163;
            this.checkBox1.TabStop = false;
            this.checkBox1.Text = "Check this if you want to close ISS";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.button2.Location = new System.Drawing.Point(3, 183);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 35);
            this.button2.TabIndex = 164;
            this.button2.Text = "Clear all";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TIMERtb
            // 
            this.TIMERtb.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TIMERtb.Font = new System.Drawing.Font("Candara", 20F);
            this.TIMERtb.ForeColor = System.Drawing.Color.Yellow;
            this.TIMERtb.Location = new System.Drawing.Point(713, 16);
            this.TIMERtb.Name = "TIMERtb";
            this.TIMERtb.ReadOnly = true;
            this.TIMERtb.Size = new System.Drawing.Size(253, 56);
            this.TIMERtb.TabIndex = 165;
            this.TIMERtb.TabStop = false;
            this.TIMERtb.Text = "00:00:00";
            this.TIMERtb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.AliceBlue;
            this.button3.Location = new System.Drawing.Point(679, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(28, 24);
            this.button3.TabIndex = 166;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // PRINTbt
            // 
            this.PRINTbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PRINTbt.BackColor = System.Drawing.Color.LightSkyBlue;
            this.PRINTbt.Location = new System.Drawing.Point(711, 613);
            this.PRINTbt.Name = "PRINTbt";
            this.PRINTbt.Size = new System.Drawing.Size(112, 44);
            this.PRINTbt.TabIndex = 9;
            this.PRINTbt.Text = "Print";
            this.PRINTbt.UseVisualStyleBackColor = false;
            this.PRINTbt.Click += new System.EventHandler(this.PRINTbt_Click);
            // 
            // SelectPrinterbt
            // 
            this.SelectPrinterbt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectPrinterbt.BackColor = System.Drawing.Color.LightSkyBlue;
            this.SelectPrinterbt.Location = new System.Drawing.Point(711, 566);
            this.SelectPrinterbt.Name = "SelectPrinterbt";
            this.SelectPrinterbt.Size = new System.Drawing.Size(112, 44);
            this.SelectPrinterbt.TabIndex = 10;
            this.SelectPrinterbt.Text = "Select printer";
            this.SelectPrinterbt.UseVisualStyleBackColor = false;
            this.SelectPrinterbt.Click += new System.EventHandler(this.SelectPrinterbt_Click);
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // printDialog1
            // 
            this.printDialog1.AllowSomePages = true;
            this.printDialog1.Document = this.printDocument1;
            this.printDialog1.PrintToFile = true;
            this.printDialog1.UseEXDialog = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.LightGreen;
            this.button1.Location = new System.Drawing.Point(843, 566);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 44);
            this.button1.TabIndex = 11;
            this.button1.Text = "Remove from list";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.button1.UseCompatibleTextRendering = true;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::POT.Properties.Resources.LoadDataOff;
            this.pictureBox1.Location = new System.Drawing.Point(496, 585);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(137, 65);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 167;
            this.pictureBox1.TabStop = false;
            // 
            // ISS
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(978, 662);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SelectPrinterbt);
            this.Controls.Add(this.PRINTbt);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.TIMERtb);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.CANCELBt);
            this.Controls.Add(this.SAVEbt);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.ISSSelectorCb);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ComentTb);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.OLDPartSNTb);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.OldPartCNTb);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.NewPartSNCb);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.NewPartCNTb);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label55);
            this.Controls.Add(this.NewPartCb);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.NewPartCodeTb);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.OldPartCb);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.OldPartCodeTb);
            this.Controls.Add(this.WorkDoneCb);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.IDTb);
            this.Controls.Add(this.CNTb);
            this.Controls.Add(this.SNTb);
            this.Controls.Add(this.DateSentTb);
            this.Controls.Add(this.DateInTb);
            this.Controls.Add(this.NameTb);
            this.Controls.Add(this.PartCb);
            this.Controls.Add(this.STOPbt);
            this.Controls.Add(this.STARTbt);
            this.Controls.Add(this.PAUSEbt);
            this.Controls.Add(this.pictureBox1);
            this.MinimumSize = new System.Drawing.Size(1000, 718);
            this.Name = "ISS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ISS";
            this.Load += new System.EventHandler(this.ISS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button PAUSEbt;
        private System.Windows.Forms.Button STARTbt;
        private System.Windows.Forms.Button STOPbt;
        private System.Windows.Forms.ComboBox PartCb;
        private System.Windows.Forms.TextBox NameTb;
        private System.Windows.Forms.TextBox DateInTb;
        private System.Windows.Forms.TextBox DateSentTb;
        private System.Windows.Forms.TextBox SNTb;
        private System.Windows.Forms.TextBox CNTb;
        private System.Windows.Forms.TextBox IDTb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ComboBox WorkDoneCb;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox OldPartCodeTb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox OldPartCb;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.ComboBox NewPartCb;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox NewPartCodeTb;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox NewPartCNTb;
        private System.Windows.Forms.ComboBox NewPartSNCb;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox OldPartCNTb;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox OLDPartSNTb;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox ComentTb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox ISSSelectorCb;
        private System.Windows.Forms.Button SAVEbt;
        private System.Windows.Forms.Button CANCELBt;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox TIMERtb;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button PRINTbt;
        private System.Windows.Forms.Button SelectPrinterbt;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.Button button1;
    }
}