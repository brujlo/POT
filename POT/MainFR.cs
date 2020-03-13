using POT.BuildingClasses;
using POT.CopyPrintForms;
using POT.Documents;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace POT
{
    public partial class MainFR : Form
    {
        private int loginCNt = 1;
        Image img = null;
        Boolean isClosePicClicked = false;
        Boolean logOutClicked = false;

        int timerCnt = 0;
        int timerRB = 0;
        TimeSpan duration = new TimeSpan(0, 0, 0);
        DateTime dt = DateTime.Now;
        static QueryCommands qc = new QueryCommands();
        ConnectionHelper cn = new ConnectionHelper();
        static List<String> labResultArr = new List<string>();

        Boolean pictureOn = false;

        System.Windows.Forms.Timer t1 = new System.Windows.Forms.Timer();

        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 1)
                t1.Stop();   //this stops the timer if the form is completely displayed
            else
                Opacity += 0.02;
        }

        public MainFR()
        {
            //For Testing DB BUILD
            Properties.Settings.Default.DBTabelsBuilded = false;
            Properties.Settings.Default.Save();

            Program.SaveDocumentsPDF = Properties.Settings.Default.SaveToPDF;

            Opacity = 0;      //first the opacity is 0

            t1.Interval = 10;  //we'll increase the opacity every 10ms
            t1.Tick += new EventHandler(fadeIn);  //this calls the function that changes opacity 
            t1.Start();
            ///

            InitializeComponent();
            // 1 - admin 
            // 2 - superuser
            if (WorkingUser.AdminRights.ToString().Contains("1"))
            {
                this.linkLabel1.Enabled = true;
                this.linkLabel2.Enabled = true;
                this.linkLabel7.Enabled = true;
                this.linkLabel9.Enabled = true;
                this.linkLabel11.Enabled = true;
                this.linkLabel14.Enabled = true;
                this.linkLabel15.Enabled = true;
                this.linkLabel16.Enabled = true;
                this.linkLabel20.Enabled = true;
                this.linkLabel23.Enabled = true;
            }
            else
            {
                this.linkLabel1.Enabled = false;
                this.linkLabel2.Enabled = false;
                this.linkLabel7.Enabled = false;
                this.linkLabel9.Enabled = false;
                this.linkLabel11.Enabled = false;
                this.linkLabel14.Enabled = false;
                this.linkLabel15.Enabled = false;
                this.linkLabel16.Enabled = false;
                this.linkLabel20.Enabled = false;
                this.linkLabel23.Enabled = false;
            }

            if (WorkingUser.AdminRights.ToString().Contains("2") || WorkingUser.AdminRights.ToString().Contains("1"))
            {
                this.linkLabel9.Enabled = true;
                this.linkLabel16.Enabled = true;
                this.linkLabel20.Enabled = true;
            }
            else
            {
                this.linkLabel9.Enabled = false;
                this.linkLabel16.Enabled = false;
                this.linkLabel20.Enabled = false;
            }

            if (Properties.Settings.Default.DBTabelsBuilded) linkLabel11.Enabled = false;

            this.label16.Text = DateTime.Now.ToString("dd.MM.yyyy.");
            
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.CurrentExchangeRate();
                this.label15.Text = resultArr[2] + " kn";
                this.label12.Text = resultArr[3] + " kn";
                this.label11.Text = resultArr[4] + " kn";
                this.label35.Text = resultArr[1];

                try
                {
                    resultArr.Clear();
                    resultArr = qc.GetAllRegions();

                    if (resultArr[0] != "nok")
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
                    new LogWriter(ex);
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
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

            //// Jezik ////
            setText();

            CLogo logoImage = new CLogo();
            img = logoImage.GetImage();
            pictureBox3.Image = img;
        }

        private void MainFR_Load(object sender, EventArgs e)
        {
            int kk = 0;
            foreach (var screen in Screen.AllScreens)
            {
                kk++;
            }

            if (kk < 2 && Properties.Settings.Default.DisplayPoint.X > Screen.PrimaryScreen.Bounds.Width)
            {
                this.Location = new Point(0, 0);
                this.Size = new Size(1008, 720);
            }
            else
            {
                this.Location = Properties.Settings.Default.DisplayPoint; //Pamti zadnju poziciju forme
                this.Size = Properties.Settings.Default.MainFrSize;
            }


            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync();
            }

            if (!CheckIDs())
            {
                foreach(Control ctr in this.Controls)
                {
                    if(ctr is LinkLabel)
                        ((LinkLabel)ctr).Enabled = false;
                }
                linkLabel16.Enabled = true;
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

            //version.Text = "Version: " + Application.ProductVersion;
            if (Debugger.IsAttached)
                version.Text = "Debug Mode";
            else
                version.Text = "Version: " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

            timer2.Start();
            Thread myThread = new Thread(getOpenedTasks);
            myThread.Start();

            new LogWriter(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "App started");

            //linkLabel5.UseMnemonic = true;
            //linkLabel5.Text = "&" + linkLabel5.Text;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                QueryCommands wkQ = new QueryCommands();
                
                while (true)
                {
                    if (wkQ.CheckConnection())
                    {
                        if (!pictureOn)
                        {
                            pictureBox9.Image = Properties.Resources.MainConnectionOn;
                            pictureOn = true;
                        }

                        Thread.Sleep(60000);
                    }
                    else
                    {
                        if (pictureOn)
                        {
                            pictureBox9.Image = Properties.Resources.MainConnectionOff;
                            pictureOn = false;
                        }

                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Form ndb = new NewDBUser();
                ndb.ShowDialog();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Form ndb = new DeleteDBUser();
                ndb.ShowDialog();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            try
            {
                Form st = new State();
                //st.ShowDialog();
                st.Show();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }        
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            { 
                int cnt = 0;

                foreach (Form frm in Application.OpenForms)
                {
                    if (!frm.Name.Equals("MainFR") && !frm.Name.Equals("LoginFR") && !frm.Name.Equals("StartForm"))
                        cnt++;
                }

                if (cnt == 0)
                    linkLabel4.Enabled = true;
                else
                {
                    linkLabel4.Enabled = false;
                    return;
                }

                logOutClicked = true;
                new LogWriter(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "LogOut clicked" + System.Environment.NewLine);
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
                        linkLabel16.Enabled = true;
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
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label17.Text = DateTime.Now.ToString("HH:mm:ss");
            duration = DateTime.Now - dt;
            this.label31.Text = duration.ToString(@"dd\.hh\:mm\:ss");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Program.LoadStart();

                Primka pr = new Primka();
                pr.Show();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

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
            catch (Exception e1)
            {
                Program.LoadStop();
                this.Focus();

                new LogWriter(e1);
            }
        }

        private Boolean CheckIDs()
        {
            Boolean isIn = false;

            try
            {
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
            catch (Exception e1)
            {
                new LogWriter(e1);
                return isIn;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //this.Location = Properties.Settings.Default.DisplayPoint; //Pamti zadnju poziciju forme
            //this.Size = Properties.Settings.Default.MainFrSize;

            try
            {
                int pHgth = pictureBox4.Height; //(90,90);
                do
                {
                    pHgth = pHgth - 10;

                    pictureBox4.Height = pHgth; 
                    pictureBox4.Width = pHgth;

                    this.Refresh();
                    //System.Threading.Thread.Sleep(10);
                } while (pictureBox4.Height > 0);


                Properties.Settings.Default.DisplayPoint = Screen.FromControl(this).WorkingArea.Location;

                Properties.Settings.Default.MainFrSize = this.Size;
                Properties.Settings.Default.Save();

                isClosePicClicked = true;
                new LogWriter(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "App turned off");
                Application.Exit();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //Program.LoadStart();

                MainCmpSelector mfs = new MainCmpSelector();
                mfs.ShowDialog();

                if (!CheckIDs())
                {
                    foreach (Control ctr in this.Controls)
                    {
                        if (ctr is LinkLabel)
                            ((LinkLabel)ctr).Enabled = false;
                    }
                    linkLabel16.Enabled = true;
                    linkLabel9.Enabled = true;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Program.LoadStart();

                Otpremnica otp = new Otpremnica();
                otp.Show();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
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
                        catch (Exception e1)
                        {
                            new LogWriter(e1);
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
            else
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
                        catch (Exception e1)
                        {
                            new LogWriter(e1);
                        }
                    }

                    timerCnt = 0;
                    label30.Text = Properties.Settings.Default.CmpName + "   -   " + Properties.Settings.Default.CmpWWW + "   -   " + Properties.Settings.Default.CmpPhone;
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
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void setText()
        {
            //// Jezik ////
            
            this.label21.Text = Properties.strings.Set;
            this.label22.Text = Properties.strings.Check;
            this.label23.Text = Properties.strings.Do;
            this.label33.Text = Properties.strings.Service;
            this.label37.Text = Properties.strings.Edit;

            this.linkLabel1.Text = Properties.strings.SetDBUser;
            this.linkLabel2.Text = Properties.strings.DeleteDBUser;
            this.linkLabel6.Text = Properties.strings.ReceiptMain;
            this.linkLabel7.Text = Properties.strings.CompanyInfo;
            this.linkLabel8.Text = Properties.strings.DeliveryMain;
            this.linkLabel9.Text = Properties.strings.SelectMainRegion;
            this.linkLabel16.Text = Properties.strings.EditRegion;
            this.linkLabel17.Text = Properties.strings.DeliveryMain;
            this.linkLabel18.Text = Properties.strings.ReceiptMain;

        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Program.LoadStart();

                OpenTicketList opl = new OpenTicketList(qc.openedTicketsList(WorkingUser.Username, WorkingUser.Password));
                opl.Show();
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                this.Focus();

                new LogWriter(e1);
            }
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
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
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            MakeDataBase mdb = new MakeDataBase();
            try
            {
                ///////////////// LogMe ////////////////////////
                String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                String usedQC = "DB making";
                String data = "";
                String Result = "";
                LogWriter lw = new LogWriter();
                ////////////////////////////////////////////////
                ///

                String newDBName = Properties.Settings.Default.CmpName.Replace(".", "");
                if (newDBName.Equals(""))
                {
                    Result = "First you must insert main company. \r\n Nothing done.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }
                newDBName = newDBName.Replace(",", "");
                newDBName = newDBName.Replace("-", "");
                newDBName = newDBName.Replace("/", "");
                var ndbnArr = newDBName.Split();
                newDBName = ndbnArr[0];
                data = newDBName;

                if (mdb.MakeDB(newDBName))
                    if (mdb.MakeTables(newDBName))
                    {
                        Result = "I am done with buildibng the DB! \r\n Your catalog name is: " + Properties.Settings.Default.Catalog;
                        lw.LogMe(function, usedQC, data, Result);
                        MessageBox.Show(Result);
                    }
                    else
                    {
                        Result = "I am done with buildibng the DB, but tabels are not added! \r\n Your catalog name is: " + Properties.Settings.Default.Catalog;
                        lw.LogMe(function, usedQC, data, Result);
                        MessageBox.Show(Result);
                    }
                else
                {
                    Result = "Nothing done!";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
                
                backgroundWorker1.CancelAsync();
                e.Cancel = true;
            }
            catch (Exception es)
            {
                new LogWriter(es);
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

        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Program.LoadStart();

                IUS ius = new IUS();
                ius.Show();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void MainFR_Click(object sender, EventArgs e)
        {
            try
            {
                int cnt = 0; 

                foreach (Form frm in Application.OpenForms)
                {
                    if (!frm.Name.Equals("MainFR") && !frm.Name.Equals("LoginFR") && !frm.Name.Equals("StartForm"))
                        cnt++;
                }

                if(cnt == 0)
                    linkLabel4.Enabled = true;
                else
                    linkLabel4.Enabled = false;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Program.LoadStart();

                Tickets tck = new Tickets();
                tck.Show();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.Path);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show("File does not exist!", "Important Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(Properties.Settings.Default.Path, "Log file path", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (result == DialogResult.Cancel)
                {
                    try
                    {
                        var builder = new StringBuilder();
                        builder.Append(Properties.Settings.Default.Path);
                        builder.AppendLine();

                        //// Jezik ////
                        Clipboard.SetText(builder.ToString());
                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        MessageBox.Show(e1.Message.ToString());
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void MainFR_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClosePicClicked && !logOutClicked)
                new LogWriter(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "App turned off");

            Properties.Settings.Default.DisplayPoint = Screen.FromControl(this).WorkingArea.Location;
            Properties.Settings.Default.MainFrSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Program.LoadStart();

                CmpRegEditcs cre = new CmpRegEditcs();
                cre.Show();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            cOTPcs doc = new cOTPcs();
            doc.Show();

        }

        private void linkLabel18_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            cPRIM doc = new cPRIM();
            doc.Show();
        }

        private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            ISS iss = new ISS();
            iss.Show();
        }

        private void linkLabel21_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            cISS ciss = new cISS();
            ciss.Show();
        }

        private void linkLabel20_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            WorkListISS wli = new WorkListISS();
            wli.ShowDialog();
        }

        private void linkLabel22_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            IIS iis = new IIS();
            iis.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            Racun rc = new Racun();
            rc.Show();
        }

        private void linkLabel23_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SettingsForm().ShowDialog();
        }

        private void linkLabel24_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            Ponuda p = new Ponuda();
            p.Show();
        }

        private void linkLabel25_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.LoadStart();

            PartEditor p = new PartEditor();
            p.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            TestForm tf = new TestForm();
            tf.Show();
        }

        private void linkLabel26_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            printPreviewDialogPrim.Document = printDocumentPrim;
            printPreviewDialogPrim.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialogPrim.ShowDialog();

        }

        private void printDocumentPrim_PrintPage(object sender, PrintPageEventArgs e)
        {

            PrintMeRN pr = new PrintMeRN(18);
            //PrintMe pr = new PrintMe(cmpS, cmpR, sifrarnikArr, partListPrint, PrimkaNumber);
            pr.Print(e);

            if (!e.HasMorePages)
            {
                Properties.Settings.Default.pageNbr = 1;
                Properties.Settings.Default.Save();
            }
        }

    }
}