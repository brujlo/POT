using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT
{
    public partial class NewDBUser : Form
    {
        List<String> cboxArr = new List<string>();
        List<String> cboxArrID = new List<string>();
        List<String> cboxArrFull = new List<string>();

        public NewDBUser()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> sendArr = new List<string>();

            sendArr.Add(this.textBox1.Text.Trim());
            sendArr.Add(this.textBox2.Text.Trim());
            sendArr.Add(this.textBox3.Text.Trim());
            sendArr.Add(this.textBox4.Text.Trim());
            sendArr.Add(this.textBox5.Text.Trim());
            sendArr.Add(this.textBox6.Text.Trim());
            sendArr.Add(this.textBox8.Text.Trim());

            if (this.checkBox1.Checked && this.checkBox2.Checked)
                sendArr.Add("12");
            else if (this.checkBox1.Checked)
                sendArr.Add("1");
            else if (this.checkBox2.Checked)
                sendArr.Add("2");
            else
                sendArr.Add("0");

            try
            {
                qc.NewDBUser(WorkingUser.Username, WorkingUser.Password, sendArr);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show("User NOT added." + "\n\n" + e1.Message);
                return;
            }
            MessageBox.Show("User added.");
        }

        private void NewDBUser_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> sendArr = new List<string>();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.RegionInfo(WorkingUser.Username, WorkingUser.Password);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return;
            }

            for (int i = 0; i < resultArr.Count; i = i + 3)
            {
                cboxArr.Add(resultArr[i+1]);
                cboxArrID.Add(resultArr[i]);
                cboxArrFull.Add(resultArr[i+2]);
            }
            
            for (int i = 0; i < cboxArr.Count; i++)
            {
                this.comboBox1.Items.Add(cboxArr[i]);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox7.Text = cboxArrFull[this.comboBox1.SelectedIndex];
            this.textBox8.Text = cboxArrID[this.comboBox1.SelectedIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
