using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace POT
{
    public partial class LoginFR : Form
    {
        private int loginCNt = 1;

        public LoginFR()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.checkBox1.Checked = Properties.Settings.Default.Remember;
            this.UsernameBX.Text = Properties.Settings.Default.Username;
            //this.PasswordBX.Text = Properties.Settings.Default.Password;

            if (Properties.Settings.Default.LanguageStt.Equals("hrv"))
            {
                rbHrv.Checked = true;
                rbEng.Checked = false;
            }
            else if (Properties.Settings.Default.LanguageStt.Equals("eng"))
            {
                rbHrv.Checked = false;
                rbEng.Checked = true;
            }
            else
            {
                rbHrv.Checked = true;
                rbEng.Checked = false;
            }


            PasswordBX.SelectAll();

            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.AutoLogin == true)
            {
                checkBox2.Checked = true;
                //OkBT_Click_1(sender, e);
                OkBT.PerformClick();
            }
        }

        private void OkBT_Click_1(object sender, EventArgs e)
        {
            label1.Visible = true;
            this.Refresh();

            QueryCommands qc = new QueryCommands();
            List<String> arr = new List<string>();

            if ( ( this.UsernameBX.Text.Equals("") || this.PasswordBX.Text.Equals("") ) && Properties.Settings.Default.AutoLogin == false )
            {
                MessageBox.Show("Please enter both, username and password!");
                label1.Visible = false;
                this.Refresh();
            }
            else
            {
                try
                {
                    if (Properties.Settings.Default.AutoLogin == false)
                    {
                        Properties.Settings.Default.Username = this.UsernameBX.Text;
                        Properties.Settings.Default.Password = this.PasswordBX.Text;
                    }

                    arr = qc.SetWorkingDBUser(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
                    //arr = qc.Qcommands(this.UsernameBX.Text, this.PasswordBX.Text, arr, "DBUser");

                    if (!arr.Any())
                    {
                        if (loginCNt == 3)
                        {
                            MessageBox.Show("To many attempts, I will close now!");
                            Application.Exit();
                        }
                        else
                        {
                            MessageBox.Show("Wrong login data, please check again, attempts left: " + (3 - loginCNt));
                            loginCNt++;
                            this.UsernameBX.ResetText();
                            this.PasswordBX.ResetText();
                            this.UsernameBX.Focus();
                        }
                    }
                    else
                    {
                        if (this.checkBox1.Checked)
                        {
                            if (Properties.Settings.Default.AutoLogin == false)
                            {
                                Properties.Settings.Default.Remember = true;
                                Properties.Settings.Default.Username = this.UsernameBX.Text;
                                Properties.Settings.Default.Password = this.PasswordBX.Text;
                            }

                            Properties.Settings.Default.AutoLogin = checkBox2.Checked ? true : false;

                            //Properties.Settings.Default.DefaultLogoName = "DefaultLogoPOT";

                            try
                            {
                                if (rbHrv.Checked)
                                {
                                    Properties.Settings.Default.LanguageStt = "hrv";
                                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("hr-HR");
                                }
                                else if (rbEng.Checked)
                                {
                                    Properties.Settings.Default.LanguageStt = "eng";
                                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                                }
                            }
                            catch (Exception e1)
                            {
                                new LogWriter(e1);
                                Properties.Settings.Default.LanguageStt = "other";
                                Properties.Settings.Default.Save();
                            }

                            if (Properties.Settings.Default.AutoLogin == false)
                                Properties.Settings.Default.Save();
                        }

                        loginCNt = 1;
                        Hide();
                        Form MF = new MainFR();
                        MF.ShowDialog();
                        Close();
                    }
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    MessageBox.Show(e1.Message);
                }
                finally
                {
                    label1.Visible = false;
                    this.Refresh();

                    if (!arr.Any())
                    {
                        if (loginCNt == 3)
                        {
                            MessageBox.Show("To many attempts, I will close now!");
                            Application.Exit();
                        }
                        else
                        {
                            MessageBox.Show("Wrong login data, please check again, attempts left: " + (3 - loginCNt));
                            loginCNt++;
                            this.UsernameBX.ResetText();
                            this.PasswordBX.ResetText();
                            this.UsernameBX.Focus();
                        }
                    }
                }
            }
        }

        private void CancelBT_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form sDB = new SetDBConnection();
            sDB.ShowDialog();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(!UsernameBX.Equals("") && !PasswordBX.Equals(""))
            {
                if (checkBox2.Checked)
                    checkBox1.Checked = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
                checkBox2.Checked = false;
        }
    }
}
