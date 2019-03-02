using POT.BuildingClasses;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace POT
{
    public partial class MainFR : Form
    {
        private int loginCNt = 1;
        Image img = null;

        int timerCnt = 0;
        int timerRB = 0;
        TimeSpan duration = new TimeSpan(0, 0, 0);
        DateTime dt = DateTime.Now;
        static QueryCommands qc = new QueryCommands();
        ConnectionHelper cn = new ConnectionHelper();
        static List<String> labResultArr = new List<string>();

        public MainFR()
        {
            //For Testing DB BUILD

            Properties.Settings.Default.DBTabelsBuilded = false;
            Properties.Settings.Default.Save();
            
            ///

            InitializeComponent();
            // 1 - admin 
            // 2 - superuser
            if (WorkingUser.AdminRights.ToString().Contains("1"))
            {
                this.linkLabel1.Enabled = true;
                this.linkLabel2.Enabled = true;
                this.linkLabel5.Enabled = true;
                this.linkLabel7.Enabled = true;
                this.linkLabel9.Enabled = true;
            }
            else
            {
                this.linkLabel1.Enabled = false;
                this.linkLabel2.Enabled = false;
                this.linkLabel5.Enabled = false;
                this.linkLabel5.Enabled = false;
                this.linkLabel9.Enabled = false;
            }

            if (Properties.Settings.Default.DBTabelsBuilded) linkLabel11.Enabled = false;

            this.label16.Text = DateTime.Now.ToString("dd.MM.yyyy.");
            
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.CurrentExchangeRate(WorkingUser.Username, WorkingUser.Password);
                this.label15.Text = DateTime.Now.ToString(resultArr[2] + " kn");
                this.label12.Text = DateTime.Now.ToString(resultArr[3] + " kn");
                this.label11.Text = DateTime.Now.ToString(resultArr[4] + " kn");

                try
                {
                    resultArr.Clear();
                    resultArr = qc.GetAllRegions(WorkingUser.Username, WorkingUser.Password);

                    if(resultArr[0] != "nok")
                    {
                        Properties.Settings.Default.Remember = true;
                        Properties.Settings.Default.ServisIDRegion = int.Parse(resultArr[0].ToString());
                        Properties.Settings.Default.TransportIDRegion = int.Parse(resultArr[3].ToString());
                        Properties.Settings.Default.OstaliIDRegion = int.Parse(resultArr[6].ToString());
                        //Properties.Settings.Default.MainCompanyCode = "00";
                        Properties.Settings.Default.Save();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception)
            {
                this.label15.Text = DateTime.Now.ToString("Error");
                this.label12.Text = DateTime.Now.ToString("Error");
                this.label11.Text = DateTime.Now.ToString("Error");
            }

            this.label16.Font = new Font(label16.Font.FontFamily, label16.Font.SizeInPoints, FontStyle.Bold);
            this.label15.Font = new Font(label15.Font.FontFamily, label15.Font.SizeInPoints, FontStyle.Bold);
            this.label12.Font = new Font(label12.Font.FontFamily, label12.Font.SizeInPoints, FontStyle.Bold);
            this.label11.Font = new Font(label11.Font.FontFamily, label11.Font.SizeInPoints, FontStyle.Bold);
            this.label17.Font = new Font(label17.Font.FontFamily, label17.Font.SizeInPoints, FontStyle.Bold);

            adresa.Text = Properties.Settings.Default.CmpAddress;
            oib.Text = Properties.Settings.Default.CmpVAT;
            www.Text = Properties.Settings.Default.CmpWWW;
            tel.Text = Properties.Settings.Default.CmpPhone;
            infomail.Text = Properties.Settings.Default.CmpEmail;

            setText();

            CLogo logoImage = new CLogo();
            img = logoImage.GetImage();
            pictureBox3.Image = img;
        }

        private void MainFR_Load(object sender, EventArgs e)
        {
            if (!CheckIDs())
            {
                foreach(Control ctr in this.Controls)
                {
                    if(ctr is LinkLabel)
                        ((LinkLabel)ctr).Enabled = false;
                }
                linkLabel5.Enabled = true;
                linkLabel9.Enabled = true;
            }
            this.label4.Text = WorkingUser.Name;
            this.label5.Text = WorkingUser.Surename;
            this.label6.Text = WorkingUser.Username;
            this.label9.Text = WorkingUser.UserID.ToString();
            this.label10.Text = WorkingUser.RegionID.ToString();
            this.label24.Text = Properties.Settings.Default.Catalog;
            this.label26.Text = Properties.Settings.Default.MainCompanyCode;
            this.label28.Text = Properties.Settings.Default.CmpName;

            version.Text = "Version: " + Application.ProductVersion;

            timer2.Start();
            Thread myThread = new Thread(getOpenedTasks);
            myThread.Start();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form ndb = new NewDBUser();
            ndb.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form ndb = new DeleteDBUser();
            ndb.ShowDialog();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form st = new State();
            st.ShowDialog();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            LoginForm fr = new LoginForm();
            fr.ShowDialog();

            if(loginCNt == 1)
            {
                MainFR MF = new MainFR();
                MF.ShowDialog();
                Close();
                return;
            }
            if (loginCNt == 3 && !fr.isLoged)
            {
                MessageBox.Show("To many attempts, I will close now!");
                Application.Exit();
            }
            else if (!fr.isLoged)
            {
                MessageBox.Show("Wrong login data, please check again, attempts left: " + (3 - loginCNt));
                loginCNt++;
            }

            if (fr.isLoged)
            {
                if (!CheckIDs())
                {
                    foreach (Control ctr in this.Controls)
                    {
                        if (ctr is LinkLabel)
                            ((LinkLabel)ctr).Enabled = false;
                    }
                    linkLabel5.Enabled = true;
                    linkLabel9.Enabled = true;
                }
                MainFR MF = new MainFR();
                MF.ShowDialog();
                Close();
            }
            else
            {
                Show();
            }
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form re = new AddRegion();
            re.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label17.Text = DateTime.Now.ToString("HH:mm:ss");
            duration = DateTime.Now - dt;
            this.label31.Text = duration.ToString(@"dd\.hh\:mm\:ss");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Primka pr = new Primka();
            pr.ShowDialog();
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CompanyInfo cmpInfo = new CompanyInfo();
            cmpInfo.ShowDialog();

            adresa.Text = Properties.Settings.Default.CmpAddress;
            oib.Text = Properties.Settings.Default.CmpVAT;
            www.Text = Properties.Settings.Default.CmpWWW;
            tel.Text = Properties.Settings.Default.CmpPhone;
            infomail.Text = Properties.Settings.Default.CmpEmail;

            try
            {
                using (ResXResourceSet resxLoad = new ResXResourceSet(@".\Logo.resx"))
                {
                    pictureBox3.Image = (Image)resxLoad.GetObject("LogoPicture", true);
                }
            }
            catch (Exception)
            {

            }
        }

        private Boolean CheckIDs()
        {
            Boolean isIn = false;

            if(Properties.Settings.Default.TransportIDRegion != 0 && Properties.Settings.Default.ServisIDRegion != 0 && Properties.Settings.Default.OstaliIDRegion != 0 && Properties.Settings.Default.MainCompanyCode == "01")
            {
                isIn = true;
            }
            else
            {
                if (Properties.Settings.Default.TransportIDRegion == 0)
                    MessageBox.Show("Please define Transport Region, else program will not work correctly!");
                if (Properties.Settings.Default.ServisIDRegion == 0)
                    MessageBox.Show("Please define Ostali Region, else program will not work correctly!");
                if (Properties.Settings.Default.OstaliIDRegion == 0)
                    MessageBox.Show("Please define Service Region, else program will not work correctly!");
                if (Properties.Settings.Default.MainCompanyCode != "01")
                {
                    MessageBox.Show("Please define MainRegion data, else program will not work correctly!");
                    /*QueryCommands qc = new QueryCommands();
                    List<String> sendArr = new List<string>();
                    sendArr.Add("Tvrtke");
                    qc.ResetAutoIcrement(WorkingUser.Username, WorkingUser.Password, sendArr);*/
                }
                isIn = false;
            }

            this.Refresh();
            return isIn;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            int pHgth = pictureBox4.Height; //90,90);
            do
            {
                pHgth = pHgth - 10;

                pictureBox4.Height = pHgth; 
                pictureBox4.Width = pHgth;

                this.Refresh();
                //System.Threading.Thread.Sleep(10);
            } while (pictureBox4.Height > 0);

            Application.Exit();
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainCmpSelector mfs = new MainCmpSelector();
            mfs.ShowDialog();

            if (!CheckIDs())
            {
                foreach (Control ctr in this.Controls)
                {
                    if (ctr is LinkLabel)
                        ((LinkLabel)ctr).Enabled = false;
                }
                linkLabel5.Enabled = true;
                linkLabel9.Enabled = true;
            }
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Otpremnica otp = new Otpremnica();
            otp.Show();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if(labResultArr.Count > 0 && !labResultArr[0].Equals("nok"))
            { 
                if (timerCnt >= this.Size.Width + label30.Width)
                {
                    if (timerRB >= labResultArr.Count)
                    {
                        timerRB = 0;
                        try
                        {
                            labResultArr.Clear();
                            labResultArr = qc.openedTickets(WorkingUser.Username, WorkingUser.Password);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    timerCnt = 0;
                    label30.Text = labResultArr.Count + " / " + labResultArr[timerRB++];
                }
                else
                {
                    label30.Location = new Point(this.Size.Width - timerCnt, label30.Height / 2);
                }
            }
            timerCnt++;
        }

        static void getOpenedTasks()
        {
            try
            {
                labResultArr = qc.openedTickets(WorkingUser.Username, WorkingUser.Password);
            }
            catch (Exception)
            {
                
            }
        }

        private void setText()
        {
            this.label21.Text = Properties.strings.Set;
            this.label22.Text = Properties.strings.Check;
            this.label23.Text = Properties.strings.Do;

            this.linkLabel1.Text = Properties.strings.SetDBUser;
            this.linkLabel2.Text = Properties.strings.DeleteDBUser;
            this.linkLabel5.Text = Properties.strings.EditRegion;
            this.linkLabel7.Text = Properties.strings.CompanyInfo;
            this.linkLabel9.Text = Properties.strings.SelectMainRegion;
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenTicketList opl = new OpenTicketList(qc.openedTicketsList(WorkingUser.Username, WorkingUser.Password));
            opl.Show();
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            while (backgroundWorker1.IsBusy && !backgroundWorker1.CancellationPending)
            {
                if (Properties.Settings.Default.DBTabelsBuilded)
                {
                    linkLabel11.Enabled = false;
                    this.Refresh();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            MakeDataBase mdb = new MakeDataBase();
            try
            {
                String newDBName = Properties.Settings.Default.CmpName.Replace(".", "");
                newDBName = newDBName.Replace(",", "");
                newDBName = newDBName.Replace("-", "");
                newDBName = newDBName.Replace("/", "");
                var ndbnArr = newDBName.Split();
                newDBName = ndbnArr[0];

                if (mdb.MakeDB(newDBName))
                    if (mdb.MakeTables(newDBName))
                        MessageBox.Show("I am done with buildibng the DB! \r\n Your catalog name is: " + Properties.Settings.Default.Catalog + ".");
                    else
                        MessageBox.Show("I am done with buildibng the DB, but tabels are not added! \r\n Your catalog name is: " + Properties.Settings.Default.Catalog + ".");
                else
                    MessageBox.Show("Nothing done!");
                
                backgroundWorker1.CancelAsync();
                e.Cancel = true;
            }
            catch (Exception es)
            {
                MessageBox.Show("There was a error, ErrMsg: " + es.Message);
                backgroundWorker1.CancelAsync();
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled == true)
            //{
            //    MessageBox.Show("Canceled!");
            //}
            //else if (e.Error != null)
            //{
            //    MessageBox.Show("Error: " + e.Error.Message);
            //}
            //else
            //{
            //    MessageBox.Show("Done!");
            //}

            if (e.Error != null)
            {
                MessageBox.Show("BCKP Workrer Error: " + e.Error.Message);
            }
        }
    }
}
