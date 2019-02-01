﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            this.PasswordBX.Text = Properties.Settings.Default.Password;
        }

        private void OkBT_Click_1(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> arr = new List<string>();

            try
            {
                arr = qc.SetWorkingDBUser(this.UsernameBX.Text, this.PasswordBX.Text);
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
                        Properties.Settings.Default.Remember = true;
                        Properties.Settings.Default.Username = this.UsernameBX.Text;
                        Properties.Settings.Default.Password = this.PasswordBX.Text;
                        Properties.Settings.Default.Save();
                    }

                    loginCNt = 1;
                    Hide();
                    Form MF = new MainFR();
                    MF.ShowDialog();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
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
    }
}
