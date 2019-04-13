using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using POT.WorkingClasses;

namespace POT
{
    public partial class CompanyInfo : Form
    {
        QueryCommands qc = new QueryCommands();
        ConnectionHelper cn = new ConnectionHelper();
        int cnt;
        Image img = null;

        public CompanyInfo()
        {
            InitializeComponent();
        }

        private void CompanyInfo_Load(object sender, EventArgs e)
        {
            CmpNameTB.Text = Properties.Settings.Default.CmpName;
            CmpAddressTB.Text = Properties.Settings.Default.CmpAddress;
            CmpOIBTB.Text = Properties.Settings.Default.CmpVAT;
            CmpWWWTB.Text = Properties.Settings.Default.CmpWWW;
            CmpPhoneTB.Text = Properties.Settings.Default.CmpPhone;
            CmpEmail.Text = Properties.Settings.Default.CmpEmail;
            SupportEmail.Text = Properties.Settings.Default.SupportEmail;
            
            CmpIBAN.Text = Properties.Settings.Default.CmpIBAN;
            CmpSwift.Text = Properties.Settings.Default.CmpSWIFT;
            CmpMB.Text = Properties.Settings.Default.CmpMB;
            LogoSize.Value = Properties.Settings.Default.LogoSize;

            try
            {
                CLogo logoImage = new CLogo();
                img = logoImage.GetImage();
                pictureBox1.Image = img;

                cnt = qc.CountCompany(WorkingUser.Username, WorkingUser.Password);
                if (cnt != 0)
                    label8.Text = "Last Code - " + cnt;
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
            String data = this.CmpNameTB.Text.Trim() + ", " + this.CmpAddressTB.Text.Trim() + ", " + this.CmpOIBTB.Text.Trim() + ", " + this.CmpWWWTB.Text.Trim() + 
                ", " + this.CmpPhoneTB.Text.Trim() + ", " + this.CmpEmail.Text.Trim() + ", " + this.CmpIBAN.Text.Trim() + ", " + this.CmpSwift.Text.Trim() + ", " + 
                this.CmpMB.Text.Trim() + ", " + int.Parse(this.LogoSize.Value.ToString()) + ", " + this.SupportEmail.Text.Trim(); 
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            if (!this.cmpCode.Text.Equals("") && long.Parse(this.cmpCode.Text.Trim()) == 1)
            {
                Properties.Settings.Default.Remember = true;
                Properties.Settings.Default.CmpName = this.CmpNameTB.Text.Trim();
                Properties.Settings.Default.CmpAddress = this.CmpAddressTB.Text.Trim();
                Properties.Settings.Default.CmpVAT = this.CmpOIBTB.Text.Trim();
                Properties.Settings.Default.CmpWWW = this.CmpWWWTB.Text.Trim();
                Properties.Settings.Default.CmpPhone = this.CmpPhoneTB.Text.Trim();
                Properties.Settings.Default.CmpEmail = this.CmpEmail.Text.Trim();
                Properties.Settings.Default.CmpIBAN = this.CmpIBAN.Text.Trim();
                Properties.Settings.Default.CmpSWIFT = this.CmpSwift.Text.Trim();
                Properties.Settings.Default.CmpMB = this.CmpMB.Text.Trim();
                Properties.Settings.Default.LogoSize = int.Parse(this.LogoSize.Value.ToString());
                Properties.Settings.Default.SupportEmail = this.SupportEmail.Text.Trim(); 

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
    }
}
