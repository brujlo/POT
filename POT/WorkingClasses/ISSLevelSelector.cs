using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    public partial class ISSLevelSelector : Form
    {
        List<String> lvlLst = new List<String> { "Close", "Level 1", "Level 2" };
        int res;

        public ISSLevelSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lvlLst.Exists(x => x == comboBox1.Text))
            {
                res = lvlLst.IndexOf(comboBox1.Text);
                this.Close();
            }
            else
            {
                comboBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ISSLevelSelector_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            res = 9999;

            foreach(String item in lvlLst)
            {
                comboBox1.Items.Add(item);
            }
        }

        public int getRes()
        {
            return res;
        }

        public void setCBTekst(long br)
        {
            int brr = (int)br;
            comboBox1.Text = lvlLst[brr];
        }
    }
}
