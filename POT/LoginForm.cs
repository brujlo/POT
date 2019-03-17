using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace POT
{
    public partial class LoginForm : Form
    {
        private Boolean IsLoged;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void OkBT_Click(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> arr = new List<string>();

            try
            {
                arr = qc.SetWorkingDBUser(this.UsernameBX.Text, this.PasswordBX.Text);

                if (arr.Any())
                {
                    if (this.checkBox1.Checked)
                    {
                        Properties.Settings.Default.Remember = true;
                        Properties.Settings.Default.Username = this.UsernameBX.Text;
                        Properties.Settings.Default.Password = this.PasswordBX.Text;
                        Properties.Settings.Default.AutoLogin = checkBox2.Checked ? true : false;
                        Properties.Settings.Default.Save();
                    }

                    IsLoged = true;
                    Hide();
                    Close();
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        public Boolean isLoged
        {
            get { return IsLoged; }
        }

        private void CancelBT_Click(object sender, EventArgs e)
        {
            this.Close();

            //Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.checkBox1.Checked = false;
            this.UsernameBX.Text = Properties.Settings.Default.Username;
            this.PasswordBX.Text = Properties.Settings.Default.Password;

            Properties.Settings.Default.Remember = true;
            checkBox2.Checked = Properties.Settings.Default.AutoLogin ? true : false;
            Properties.Settings.Default.Save();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form sDB = new SetDBConnection();
            sDB.ShowDialog();
        }
    }
}
