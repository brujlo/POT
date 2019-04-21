using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using POT.MyTypes;
using POT.WorkingClasses;

namespace POT
{
    public partial class CompanyInfo : Form
    {
        QueryCommands qc = new QueryCommands();
        ConnectionHelper cn = new ConnectionHelper();
        List<MainCmp> cmpList = new List<MainCmp>();
        List<String> regionsArr = new List<String>();
        int cnt;
        Image img = null;

        public CompanyInfo()
        {
            InitializeComponent();
        }

        private void CompanyInfo_Load(object sender, EventArgs e)
        {
            comboBox1.Text = Properties.Settings.Default.CmpName;
            CmpAddressTB.Text = Properties.Settings.Default.CmpAddress;
            CmpOIBTB.Text = Properties.Settings.Default.CmpVAT;
            CmpWWWTB.Text = Properties.Settings.Default.CmpWWW;
            CmpPhoneTB.Text = Properties.Settings.Default.CmpPhone;
            CmpEmail.Text = Properties.Settings.Default.CmpEmail;
            SupportEmail.Text = Properties.Settings.Default.SupportEmail;
            
            CmpIBAN.Text = Properties.Settings.Default.CmpIBAN;
            CmpSwift.Text = Properties.Settings.Default.CmpSWIFT;
            CmpMB.Text = Properties.Settings.Default.CmpMB;


            CmpCityTB.Text = Properties.Settings.Default.CmpCity;
            CmpPBTB.Text = Properties.Settings.Default.CmpPB;
            CmpContactTB.Text = Properties.Settings.Default.CmpContact;
            CmpKNTB.Text = Properties.Settings.Default.CmpKN.ToString();
            CmpEURTB.Text = Properties.Settings.Default.CmpEUR.ToString();
            cmpCode.Text = Properties.Settings.Default.CmpCode;
            CmpCountryTB.Text = Properties.Settings.Default.CmpCountry;

            LogoSize.Value = Properties.Settings.Default.LogoSize;

            try
            {
                CLogo logoImage = new CLogo();
                img = logoImage.GetImage();
                pictureBox1.Image = img;

                cnt = qc.CountMainCmp();
                if (cnt != 0)
                    label8.Text = "Last Code - " + cnt;
                else
                    label8.Text = "Last Code - " + cnt;

                MainCmp temCmp = new MainCmp();
                cmpList = temCmp.GetAllMainCmpInfoSortByName();
                regionsArr = qc.GetAllRegions();
                if (cmpList.Count != 0)
                {
                    for (int i = 0; i < temCmp.Count; i++)
                    {
                        this.comboBox1.Items.Add(cmpList[i].Name);
                    }

                }
                regionsArr = qc.GetAllRegions();

                if (regionsArr.Count != 0)
                {
                    for (int i = 0; i < regionsArr.Count; i = i + 3)
                    {
                        if (long.Parse(regionsArr[i]) > 3)
                            this.comboBox2.Items.Add(regionsArr[i]);
                    }
                }

                comboBox2.Text = Properties.Settings.Default.CmpRegionID.ToString();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "resx.AddResource(\"LogoPicture\", pictureBox1.Image)";
            String data = comboBox1.Text.Trim() + ", " +
                CmpAddressTB.Text.Trim() + ", " +
                CmpCityTB.Text.Trim() + ", " +
                CmpPBTB.Text.Trim() + ", " +
                CmpCountryTB.Text.Trim() + ", " +
                CmpKNTB.Text.Trim() + ", " +
                CmpEURTB.Text.Trim() + ", " +
                CmpOIBTB.Text.Trim() + ", " +
                CmpSwift.Text.Trim() + ", " +
                CmpMB.Text.Trim() + ", " +
                CmpIBAN.Text.Trim() + ", " +
                comboBox2.Text.Trim() + ", " +
                cmpCode.Text.Trim() + ", " +
                CmpWWWTB.Text.Trim() + ", " +
                CmpPhoneTB.Text.Trim() + ", " +
                CmpContactTB.Text.Trim() + ", " +
                CmpEmail.Text.Trim() + ", " +
                SupportEmail.Text.Trim() + ", " +
                int.Parse(LogoSize.Value.ToString());
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///
            long mID = 0;
            String smID = "";
            try
            {
                List<String> arr = new List<String>();
                arr = qc.MainCmpInfoByName(comboBox1.Text);
                smID = arr[0];

                if (arr[0].Equals("nok"))
                    mID = 0;
                else
                    mID = long.Parse(smID);
            }
            catch { }

            if (!this.cmpCode.Text.Equals(""))
            {
                try
                {
                    usedQC = "Saving MainCmp info";

                    if (qc.AddMainCmp(mID, comboBox1.Text.Trim(), CmpAddressTB.Text.Trim(), CmpCityTB.Text.Trim(), CmpPBTB.Text.Trim(), CmpOIBTB.Text.Trim(),CmpContactTB.Text.Trim(), CmpSwift.Text.Trim(), 
                        decimal.Parse(CmpKNTB.Text.Trim()), decimal.Parse(CmpEURTB.Text.Trim()),
                        cmpCode.Text.Trim(), CmpCountryTB.Text.Trim(), comboBox2.Text.Trim(),CmpEmail.Text.Trim(), CmpPhoneTB.Text.Trim(), CmpWWWTB.Text.Trim(), CmpMB.Text.Trim(), CmpIBAN.Text.Trim(), SupportEmail.Text.Trim()))
                    {
                        Properties.Settings.Default.Remember = true;
                        Properties.Settings.Default.CmpName = this.comboBox1.Text.Trim();
                        Properties.Settings.Default.CmpAddress = this.CmpAddressTB.Text.Trim();
                        Properties.Settings.Default.CmpVAT = this.CmpOIBTB.Text.Trim();
                        Properties.Settings.Default.CmpWWW = this.CmpWWWTB.Text.Trim();
                        Properties.Settings.Default.CmpPhone = this.CmpPhoneTB.Text.Trim();
                        Properties.Settings.Default.CmpEmail = this.CmpEmail.Text.Trim();
                        Properties.Settings.Default.CmpIBAN = this.CmpIBAN.Text.Trim();
                        Properties.Settings.Default.CmpSWIFT = this.CmpSwift.Text.Trim();
                        Properties.Settings.Default.CmpMB = this.CmpMB.Text.Trim();
                        Properties.Settings.Default.CmpCity = this.CmpCityTB.Text.Trim();
                        Properties.Settings.Default.CmpPB = this.CmpPBTB.Text.Trim();
                        Properties.Settings.Default.CmpContact = this.CmpContactTB.Text.Trim();
                        Properties.Settings.Default.CmpKN = decimal.Parse(CmpKNTB.Text.Trim());
                        Properties.Settings.Default.CmpEUR = decimal.Parse(CmpEURTB.Text.Trim());
                        Properties.Settings.Default.CmpCode = this.cmpCode.Text.Trim();
                        Properties.Settings.Default.CmpCountry = this.CmpCountryTB.Text.Trim();
                        Properties.Settings.Default.CmpRegionID = long.Parse(comboBox2.Text.Trim());
                        Properties.Settings.Default.LogoSize = int.Parse(this.LogoSize.Value.ToString());

                        using (ResXResourceWriter resx = new ResXResourceWriter(@".\Logo.resx"))
                        {
                            resx.AddResource("LogoPicture", pictureBox1.Image);
                        }

                        Properties.Settings.Default.Save();

                        Result = "Saved.";
                        lw.LogMe(function, usedQC, data, Result);
                        MessageBox.Show(Result);
                    }
                    else
                    {
                        MessageBox.Show("Company info not saved.");
                        return;
                    }

                }
                catch (Exception e1)
                {
                    Result = "Cmp not updated." + Environment.NewLine + e1.ToString();
                    lw.LogMe(function, usedQC, data, Result);
                    new LogWriter(e1);
                    MessageBox.Show(Result);
                    return;
                }
            }
            else
            {
                Result = "Not saved, new Company Code must be bigger then last Code.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog dlg = new OpenFileDialog())
            {
                ///////////////// LogMe ////////////////////////
                String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                String usedQC = "dlg.ShowDialog()";
                String data = "Change logo";
                String Result = "";
                LogWriter lw = new LogWriter();
                ////////////////////////////////////////////////
                ///

                dlg.Title = "Select image";
                
                {
                    if(dlg.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox1.ImageLocation = dlg.FileName;
                        Result = dlg.FileName.ToString();
                        lw.LogMe(function, usedQC, data, Result);
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox1.SelectedIndex;

            comboBox1.Text = cmpList[i].Name;
            CmpAddressTB.Text = cmpList[i].Address;
            CmpCityTB.Text = cmpList[i].City;
            CmpPBTB.Text = cmpList[i].PB;
            CmpCountryTB.Text = cmpList[i].Country;
            CmpKNTB.Text = cmpList[i].KN.ToString();
            CmpEURTB.Text = cmpList[i].EUR.ToString();
            CmpOIBTB.Text = cmpList[i].OIB;
            CmpSwift.Text = cmpList[i].BIC;
            CmpMB.Text = cmpList[i].MB;
            CmpIBAN.Text = cmpList[i].IBAN;
            comboBox2.Text = cmpList[i].RegionID.ToString();
            CmpWWWTB.Text = cmpList[i].WWW;
            CmpPhoneTB.Text = cmpList[i].Phone;
            CmpEmail.Text = cmpList[i].Email;
            SupportEmail.Text = cmpList[i].SupportEmail;
            LogoSize.Value = Properties.Settings.Default.LogoSize;
        }

        private void CmpKNTB_TextChanged(object sender, EventArgs e)
        {
            CmpKNTB.Text = CmpKNTB.Text.Replace(',', '.');
            CmpKNTB.SelectionStart = CmpKNTB.Text.Length;
        }

        private void CmpEURTB_TextChanged(object sender, EventArgs e)
        {
            CmpEURTB.Text = CmpEURTB.Text.Replace(',', '.');
            CmpEURTB.SelectionStart = CmpEURTB.Text.Length;
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (long.Parse(comboBox2.Text) < 4 || long.Parse(comboBox2.Text) > long.Parse(regionsArr[regionsArr.Count - 3]))
                {
                    comboBox2.ResetText();
                    textBox1.ResetText();
                }

                for (int i = 0; i < regionsArr.Count; i = i + 3)
                {
                    if (comboBox2.Text.Equals(regionsArr[i]))
                    {
                        textBox1.Text = regionsArr[i + 2];
                        break;
                    }
                    else
                    {
                        textBox1.ResetText();
                    }
                }
            }
            catch
            {
                comboBox2.ResetText();
                textBox1.ResetText();
            }
        }
    }
}