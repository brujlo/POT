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
            Properties.Settings.Default.DataSource = this.textBox1.Text.Trim();
            Properties.Settings.Default.Catalog = this.textBox2.Text.Trim();
            Properties.Settings.Default.Save();
        }
    }
}
