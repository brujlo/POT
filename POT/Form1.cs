using System;
using System.Windows.Forms;

namespace POT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void setText(String value)
        {
            this.textBox1.Text = value;
        }
    }
}
