using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT
{
    public partial class AddRegion : Form
    {
        List<String> RegionID = new List<String>();
        List<String> RegionShort = new List<String>();
        List<String> FullRegion = new List<String>();
        List<long> biggestID = new List<long>();

        public AddRegion()
        {
            InitializeComponent();
        }

        private void AddRegion_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            this.comboBox1.Items.Clear();
            this.comboBox1.ResetText();

            try
            {
                resultArr = qc.GetAllRegions(WorkingUser.Username, WorkingUser.Password);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return;
            }

            for (int i = 0; i < resultArr.Count; i = i + 3)
            {
                RegionID.Add(resultArr[i]);
                biggestID.Add(long.Parse(resultArr[i]));
                RegionShort.Add(resultArr[i + 1]);
                FullRegion.Add(resultArr[i + 2]);
            }
            
            biggestID.Sort((x,y) => x.CompareTo(y));
            label6.Text = biggestID[biggestID.Count - 1].ToString();

            for (int i = 0; i < RegionShort.Count; i++)
            {
                this.comboBox1.Items.Add(RegionID[i] + " - " + RegionShort[i] + " - " + FullRegion[i]) ;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = RegionShort[comboBox1.SelectedIndex];
            textBox2.Text = FullRegion[comboBox1.SelectedIndex];
            textBox3.Text = RegionID[comboBox1.SelectedIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "qc.AddRegion";
            String data = comboBox1.Text + ", " + textBox3.Text + ", " + textBox1.Text + ", " + textBox2.Text + ", " + addressTB.Text + ", " + CityTB.Text + ", " + PBTB.Text + ", " + OIBTB.Text + ", " +
                        ContactTB.Text + ", " + CountryTB.Text + ", " + BICTB.Text + ", " + EmailTB.Text + ", " + Properties.Settings.Default.MainCompanyCode;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            
            QueryCommands qc = new QueryCommands();
            Boolean result = false;
            String CompanyCode;

            if(textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && addressTB.Text != "" && CityTB.Text != "" && PBTB.Text != "" && OIBTB.Text != "" && ContactTB.Text != "" && CountryTB.Text != "")
            {
                try
                {
                    if (int.Parse(label6.Text) < 4)
                    {
                        //Properties.Settings.Default.Remember = true;
                        Properties.Settings.Default.MainCompanyCode = "01";

                        Properties.Settings.Default.CmpName = textBox2.Text.Trim();
                        Properties.Settings.Default.CmpAddress = addressTB.Text.Trim() + ", " + CountryTB.Text.Trim() + " - " + PBTB.Text.Trim() + " " + CityTB.Text.Trim();
                        Properties.Settings.Default.CmpVAT = this.OIBTB.Text.Trim();
                        Properties.Settings.Default.CmpWWW = "www";
                        Properties.Settings.Default.CmpPhone = "Phone";
                        if (EmailTB.Text.Equals(""))
                            Properties.Settings.Default.CmpEmail = "email";
                        else
                            Properties.Settings.Default.CmpEmail = EmailTB.Text;
                        CompanyCode = Properties.Settings.Default.MainCompanyCode;
                    }
                    else
                    {
                        CompanyCode = string.Format("{0:00}", qc.CountCompany(WorkingUser.Username, WorkingUser.Password) + 1);
                    }
                    result = qc.AddRegion(WorkingUser.Username, WorkingUser.Password, long.Parse(textBox3.Text.Trim()), textBox1.Text.Trim(), textBox2.Text.Trim(), addressTB.Text, CityTB.Text, PBTB.Text, OIBTB.Text, ContactTB.Text, CountryTB.Text, BICTB.Text, EmailTB.Text, CompanyCode);
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    MessageBox.Show(e1.Message);
                    return;
                }
                if (result)
                {
                    Result = "Region added.";
                    lw.LogMe(function, usedQC, data, Result);

                    MessageBox.Show(Result);
                    Properties.Settings.Default.Remember = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Result = "Region NOT added.";
                    lw.LogMe(function, usedQC, data, Result);

                    MessageBox.Show(Result);
                }
            }
            else
            {
                MessageBox.Show("Please fill in all fields, not added.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            Boolean result = false;

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "qc.DeleteRegion";
            String data = comboBox1.Text + ", " + textBox3.Text + ", " + textBox1.Text + ", " + textBox2.Text + ", " + addressTB.Text + ", " + CityTB.Text + ", " + PBTB.Text + ", " + OIBTB.Text + ", " +
                        ContactTB.Text + ", " + CountryTB.Text + ", " + BICTB.Text + ", " + EmailTB.Text + ", " + Properties.Settings.Default.MainCompanyCode;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            
            if (textBox3.Text.Equals("1") || textBox3.Text.Equals("2") || textBox3.Text.Equals("3") || textBox3.Text.Equals("4"))
            {
                Result = "Not deleted, regions 01, 02, 03 and 04 cant be deleted.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result);
                return;
            }

            try
            {
                result = qc.DeleteRegion(WorkingUser.Username, WorkingUser.Password, int.Parse(textBox3.Text.Trim()));
                if (result)
                {
                    Result = "Deleted.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
                else
                {
                    Result = "NOT Deleted.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show("Please check that RegionID is correct NUMBER." + "\n\n" + e1.Message);
            }
        }
    }
}
