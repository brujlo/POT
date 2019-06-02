using POT.WorkingClasses;
using System;
using System.Windows.Forms;

namespace POT
{
    public partial class SetDBConnection : Form
    {
        public SetDBConnection()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "DB connection address and name save.";
            String data = this.textBox1.Text.Trim() + ", " + this.textBox2.Text.Trim();
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            Properties.Settings.Default.DataSource = this.textBox1.Text.Trim();
            Properties.Settings.Default.Catalog = this.textBox2.Text.Trim();
            Properties.Settings.Default.Save();

            Result = "Saved, no msgToUser";
            lw.LogMe(function, usedQC, data, Result);
        }

        private void SetDBConnection_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.DataSource;
            textBox2.Text = Properties.Settings.Default.Catalog;
        }
    }
}
