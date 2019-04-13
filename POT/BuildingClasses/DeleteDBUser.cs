using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT
{
    public partial class DeleteDBUser : Form
    {
        List<String> cboxArr = new List<string>();
        List<String> cboxArrName = new List<string>();
        List<String> cboxArrSurename = new List<string>();
        List<String> cboxArrRegion = new List<string>();
        List<String> cboxArrID = new List<string>();

        public DeleteDBUser()
        {
            InitializeComponent();
        }

        private void DeleteDBUser_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            this.comboBox1.Items.Clear();
            this.comboBox1.ResetText();

            try
            {
                resultArr = qc.UsersInfo(WorkingUser.Username, WorkingUser.Password);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return;
            }

            for (int i = 0; i < resultArr.Count; i = i + 8)
            {
                cboxArr.Add(resultArr[i + 3]);
                cboxArrName.Add(resultArr[i + 1]);
                cboxArrSurename.Add(resultArr[i + 2]);
                cboxArrRegion.Add(resultArr[i + 7]);
                cboxArrID.Add(resultArr[i]);
            }

            for (int i = 0; i < cboxArr.Count; i++)
            {
                this.comboBox1.Items.Add(cboxArr[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "qc.DeleteDBUser";
            String data = this.comboBox1.Text + ", " + this.label9.Text;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///
            QueryCommands qc = new QueryCommands();

            if (!this.label9.Text.Equals("UserID"))
            {
                try
                {
                    if(qc.DeleteDBUser(WorkingUser.Username, WorkingUser.Password, this.comboBox1.Text, this.label9.Text))
                    {
                        Result = "User deleted.";
                        lw.LogMe(function, usedQC, data, Result);
                        MessageBox.Show(Result);

                        this.label6.Text = "";
                        this.label7.Text = "";
                        this.label8.Text = "";
                        this.label9.Text = "";

                        cboxArr.Clear();
                        cboxArrName.Clear();
                        cboxArrSurename.Clear();
                        cboxArrRegion.Clear();
                        cboxArrID.Clear();

                        DeleteDBUser_Load(sender, e);
                    }
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    Result = "User NOT deleted." + "\n\n" + e1.Message;
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }
            }
            else
            {
                Result = "Please select user to delete.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.label6.Text = cboxArrName[this.comboBox1.SelectedIndex];
            this.label7.Text = cboxArrSurename[this.comboBox1.SelectedIndex];
            this.label8.Text = cboxArrRegion[this.comboBox1.SelectedIndex];
            this.label9.Text = cboxArrID[this.comboBox1.SelectedIndex];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
