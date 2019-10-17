using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using POT.WorkingClasses;
using System.Media;
using System.Diagnostics;

namespace POT.Documents
{
    public partial class Tickets : Form
    {
        ConnectionHelper cn = new ConnectionHelper();
        List<Company> resultArrC = new List<Company>();
        static List<String> sifrarnikArr = new List<string>();

        AddTIDDB at = new AddTIDDB();
        Company cmp = new Company();

        TIDs tids = new TIDs();
        String tidsListType;

        public Tickets()
        {
            InitializeComponent();
            AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "Waiting");
        }

        private void Tickets_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            Company cmpList = new Company();

            this.nextID.Text = (qc.GetLastTicketID(WorkingUser.Username, WorkingUser.Password) + 1).ToString();

            try
            {
                resultArrC = cmpList.GetAllCompanyInfoSortByName();

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.companyCB.Items.Add(resultArrC[i].Name);
                    }

                }
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                this.Focus();

                new LogWriter(e1);
            }

            Program.LoadStop();
            this.Focus();
        }

        ///////////////////////////////////////////////////////////

        private void datPrijave_Enter(object sender, EventArgs e)
        {
            try
            {
                datPrijave_Leave(sender, e);
                this.datPrijave.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
            
            

        }

        private void datPrijave_Leave(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                if (this.datPrijave.Text.Equals(""))
                    this.datPrijave.Text = DateTime.Now.ToString("dd.MM.yy.");
                else
                    this.datPrijave.Text = dc.ConvertDDMMYY(datPrijave.Text);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void vriPrijave_Enter(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                this.vriPrijave.Text = dc.ConvertTimeHHMM(this.vriPrijave.Text);

                this.vriPrijave.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
        }

        private void vriPrijave_Leave(object sender, EventArgs e)
        {
            try
            {
                vriPrijave_Enter(sender, e);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void SLAdat_Enter(object sender, EventArgs e)
        {
            try
            {
                SLAdat_Leave(sender, e);
                this.SLAdat.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
        }

        private void SLAdat_Leave(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                if(this.prio.Text.Equals("6"))
                    this.SLAdat.Text = dc.CalculatePrio(this.prio.Text, dc.ConvertDDMMYY(this.SLAdat.Text), dc.ConvertTimeHHMM(this.SLAvri.Text)).ToString("dd.MM.yy.");
                else
                    this.SLAdat.Text = dc.CalculatePrio(this.prio.Text, this.datPrijave.Text, this.vriPrijave.Text).ToString("dd.MM.yy.");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void SLAvri_Enter(object sender, EventArgs e)
        {
            try
            {
                SLAvri_Leave(sender, e);
                this.datPrijave.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
        }

        private void SLAvri_Leave(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                

                if (this.prio.Text.Equals("6"))
                    this.SLAvri.Text = dc.CalculatePrio(this.prio.Text, dc.ConvertDDMMYY(this.SLAdat.Text), dc.ConvertTimeHHMM(this.SLAvri.Text)).ToString("HH:mm");
                else
                    this.SLAvri.Text = dc.CalculatePrio(this.prio.Text, this.datPrijave.Text, this.vriPrijave.Text).ToString("HH:mm");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void naziv_TextChanged(object sender, EventArgs e)
        {
            label10.Text = "Letters left " + (400 - naziv.TextLength).ToString();
        }

        private void opis_TextChanged(object sender, EventArgs e)
        {
            label11.Text = "Letters left " + (400 - opis.TextLength).ToString();
        }

        private void drive_Enter(object sender, EventArgs e)
        {
            this.drive.SelectAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.companyCB.Text.Equals("") || this.companyCB.Text.Equals("Search by company name") || this.datPrijave.Text.Equals("") || this.vriPrijave.Text.Equals("") || this.drive.Text.Equals("") 
                || this.SLAdat.Text.Equals("") || this.SLAvri.Text.Equals("") || this.label30.Text.Equals("") || this.filijala.Text.Equals("") 
                || this.ccn.Text.Equals("") || this.cid.Text.Equals("") || this.prio.Text.Equals("") || this.opis.Text.Equals("") || this.naziv.Text.Equals(""))
            {
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "All fields must filed in" + Environment.NewLine + "    - Nothing done");
                return;
            }

            Branch br = new Branch();
            QueryCommands qc = new QueryCommands();

            long TIDid = qc.GetLastTicketID(WorkingUser.Username, WorkingUser.Password) + 1;
            br.SetFilByTvrtkeCodeFilNumber(cmp.Code, this.filijala.Text.Trim());

            if (!qc.IsTIDExistByIDCCNCID(TIDid, ccn.Text, cid.Text))
            {
                Boolean storno = false;
                if (at.sendIntervention(cmp, datPrijave.Text, vriPrijave.Text, prio.Text, SLAdat.Text, SLAvri.Text, filijala.Text, ccn.Text, cid.Text, TIDid, drive.Text, InqUser.Text, naziv.Text, opis.Text, storno, br.City, br.Address, br.Phone, this))
                    AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + " Added and sent");
            }
            else
            {
                String msg = Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + " TID already exist";
                AppendTextBox(msg);
            }

            SystemSounds.Hand.Play();
        }

        private void companyCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.companyCB.SelectedIndex;

            cmp.Clear();

            cmp.Address = resultArrC[index].Address;
            cmp.BIC = resultArrC[index].BIC;
            cmp.City = resultArrC[index].City;
            cmp.Code = resultArrC[index].Code;
            cmp.Contact = resultArrC[index].Contact;
            cmp.Country = resultArrC[index].Country;
            cmp.EUR = resultArrC[index].EUR;
            cmp.Email = resultArrC[index].Email;
            cmp.ID = resultArrC[index].ID;
            cmp.KN = resultArrC[index].KN;
            cmp.Name = resultArrC[index].Name;
            cmp.OIB = resultArrC[index].OIB;
            cmp.PB = resultArrC[index].PB;
            cmp.RegionID = resultArrC[index].RegionID;


            this.cmpID.Text = resultArrC.ElementAt(companyCB.SelectedIndex).ID.ToString().Trim();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.companyCB.ResetText();
            this.datPrijave.ResetText();
            this.vriPrijave.ResetText();
            this.drive.ResetText();
            this.SLAdat.ResetText();
            this.SLAvri.ResetText();
            this.InqUser.ResetText();
            this.filijala.ResetText();
            this.ccn.ResetText();
            this.cid.ResetText();
            this.prio.ResetText();
            this.opis.ResetText();
            this.naziv.ResetText();

            AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "All reseted" + System.Environment.NewLine + "    - Waiting");
        }

        private void filijala_Leave(object sender, EventArgs e)
        {
            try
            {
                this.filijala.Text = filijala.Text.ToUpper();
                if(long.TryParse(filijala.Text,out long n))
                    filijala.Text = long.Parse(filijala.Text).ToString("000");
            }catch(Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void InqUser_Leave(object sender, EventArgs e)
        {
            InqUser.Text = InqUser.Text.ToUpper();
        }

        private void ccn_Leave(object sender, EventArgs e)
        {
            this.ccn.Text = this.ccn.Text.ToUpper();
        }

        private void cid_Leave(object sender, EventArgs e)
        {
            this.cid.Text = this.cid.Text.ToUpper();
        }

        ///////////////////////////////////////////////////////////

        public void AppendTextBox(String value)
        {
            this.textBox3.AppendText(value + System.Environment.NewLine);
            new LogWriter(value);
        }

        ///////////////////////////////////////////////////////////
        //   TAB CONTROL  //
        ///////////////////////////////////////////////////////////

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    
                    break;
                case "tabPage2":
                    TIDs tmpTids = new TIDs(true);
                    tids = tmpTids;

                    AllRB.Checked = true;

                    tidsListType = "allTickets";

                    ClearComboboxes();
                    FillComboboxes(tids.allTickets);

                    OpenCntLB.Text = tmpTids.openedTickets.Count().ToString();
                    ClosedCntLB.Text = tmpTids.closedTickets.Count().ToString();
                    WorkingCntLB.Text = tmpTids.workingTickets.Count().ToString();
                    StornoCntLB.Text = tmpTids.stornoTickets.Count().ToString();

                    break;
                case "tabPage3":
                    break;
                case "tabPage4":
                    break;
            }
        }


        ///////////////////////////////////////////////////////////
        //   TID CHECK  //
        ///////////////////////////////////////////////////////////


        private void AllRB_Click(object sender, EventArgs e)
        {
            ClearComboboxes();

            tidsListType = "";
            tidsListType = "allTickets";

            FillComboboxes(tids.allTickets);
        }

        private void StornoRB_Click(object sender, EventArgs e)
        {
            ClearComboboxes();

            tidsListType = "";
            tidsListType = "stornoTickets";

            FillComboboxes(tids.stornoTickets);
        }

        private void WorkingRB_Click(object sender, EventArgs e)
        {
            ClearComboboxes();

            tidsListType = "";
            tidsListType = "workingTickets";

            FillComboboxes(tids.workingTickets);
        }

        private void ClosedRB_Click(object sender, EventArgs e)
        {
            ClearComboboxes();

            tidsListType = "";
            tidsListType = "closedTickets";

            FillComboboxes(tids.closedTickets);
        }

        private void OpenRB_Click(object sender, EventArgs e)
        {
            ClearComboboxes();

            tidsListType = "";
            tidsListType = "openedTickets";

            FillComboboxes(tids.openedTickets);
        }

        private void ClearComboboxes()
        {
            TIDidCB.ResetText();
            TIDccnCB.ResetText();
            TIDcidCB.ResetText();

            TIDidCB.Items.Clear();
            TIDccnCB.Items.Clear();
            TIDcidCB .Items.Clear();

            TIDidTB.Text = "";
            TvrtkaIDTB.Text = "";
            PrioTB.Text = "";
            FilijalaTB.Text = "";
            CCNTB.Text = "";
            CIDTB.Text = "";
            DatPrijaveTB.Text = "";
            VriPrijaveTB.Text = "";
            DatSLATB.Text = "";
            VriSLATB.Text = "";
            DriveTB.Text = "";
            NazivUredajaTB.Text = "";
            OpisKvaraTB.Text = "";
            PrijavioTB.Text = "";
            UserIDPreuzeoTB.Text = "";
            DatPreuzetoTB.Text = "";
            VriPreuzetoTB.Text = "";
            UserIDDriveTB.Text = "";
            DatDriveTB.Text = "";
            VriDriveTB.Text = "";
            UserIDPoceoTB.Text = "";
            DatPoceoTB.Text = "";
            VriPoceoTB.Text = "";
            UserIDZavrsioTB.Text = "";
            DatZavrsioTB.Text = "";
            VriZavrsioTB.Text = "";
            UserIDUnioTB.Text = "";
            DatReportTB.Text = "";
            VriReportTB.Text = "";
            RNIDTB.Text = "";
            UserIDSastavioTB.Text = "";
        }

        private void FillComboboxes(List<TIDs> tidLst)
        {
            foreach(TIDs item in tidLst)
            {
                TIDidCB.Items.Add(item.TicketID.ToString());
                TIDccnCB.Items.Add(item.CCN);
                TIDcidCB.Items.Add(item.CID);
            }
        }

        private void TIDidCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendList(TIDidCB.SelectedIndex);
        }

        private void TIDccnCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendList(TIDccnCB.SelectedIndex);
        }

        private void TIDcidCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendList(TIDcidCB.SelectedIndex);
        }

        private void SendList(int index)
        {
            switch (tidsListType)
            {
                case "allTickets":
                    FillTIDFields(index, tids.allTickets);
                    break;
                case "stornoTickets":
                    FillTIDFields(index, tids.stornoTickets);
                    break;
                case "workingTickets":
                    FillTIDFields(index, tids.workingTickets);
                    break;
                case "closedTickets":
                    FillTIDFields(index, tids.closedTickets);
                    break;
                case "openedTickets":
                    FillTIDFields(index, tids.openedTickets);
                    break;

            }
        }

        private void FillTIDFields(int index, List<TIDs> tidLst)
        {
            TIDidTB.Text = tidLst[index].TicketID.ToString();
            TvrtkaIDTB.Text = tidLst[index].TvrtkeID.ToString();
            PrioTB.Text = tidLst[index].Prio.ToString();
            FilijalaTB.Text = tidLst[index].Filijala.ToString();
            CCNTB.Text = tidLst[index].CCN.ToString();
            CIDTB.Text = tidLst[index].CID.ToString();
            DatPrijaveTB.Text = tidLst[index].DatPrijave.ToString();
            VriPrijaveTB.Text = tidLst[index].VriPrijave.ToString();
            DatSLATB.Text = tidLst[index].DatSLA.ToString();
            VriSLATB.Text = tidLst[index].VriSLA.ToString();
            DriveTB.Text = tidLst[index].Drive.ToString();
            NazivUredajaTB.Text = tidLst[index].NazivUredaja.ToString();
            OpisKvaraTB.Text = tidLst[index].OpisKvara.ToString();
            PrijavioTB.Text = tidLst[index].Prijavio.ToString();
            UserIDPreuzeoTB.Text = tidLst[index].UserIDPreuzeo.ToString();
            DatPreuzetoTB.Text = tidLst[index].DatPreuzeto.ToString();
            VriPreuzetoTB.Text = tidLst[index].VriPreuzeto.ToString();
            UserIDDriveTB.Text = tidLst[index].UserIDDrive.ToString();
            DatDriveTB.Text = tidLst[index].DatDrive.ToString();
            VriDriveTB.Text = tidLst[index].VriDrive.ToString();
            UserIDPoceoTB.Text = tidLst[index].UserIDPoceo.ToString();
            DatPoceoTB.Text = tidLst[index].DatPoceo.ToString();
            VriPoceoTB.Text = tidLst[index].VriPoceo.ToString();
            UserIDZavrsioTB.Text = tidLst[index].UserIDZavrsio.ToString();
            DatZavrsioTB.Text = tidLst[index].DatZavrsio.ToString();
            VriZavrsioTB.Text = tidLst[index].VriZavrsio.ToString();
            UserIDUnioTB.Text = tidLst[index].UserIDUnio.ToString();
            DatReportTB.Text = tidLst[index].DatReport.ToString();
            VriReportTB.Text = tidLst[index].VriReport.ToString();
            RNIDTB.Text = tidLst[index].RNID.ToString();
            UserIDSastavioTB.Text = tidLst[index].UserIDSastavio.ToString();
        }

        private void PrintRNBT_Click(object sender, EventArgs e)
        {
            //String printerName = printDialog1.PrinterSettings.PrinterName;

            //try
            //{
            //    PrintDialog printDialog1 = new PrintDialog();
            //    printDialog1.Document = printDocumentOtp;

            //    printDialog1.PrinterSettings.PrinterName = "Microsoft Print to PDF";

            //    if (!printDialog1.PrinterSettings.IsValid) return;

            //    if (!Directory.Exists(Properties.Settings.Default.DefaultFolder + "\\OTP"))
            //        return;

            //    string fileName = "\\OTP " + OTPNumber.ToString().Replace("/", "") + ".pdf";
            //    string directory = Properties.Settings.Default.DefaultFolder + "\\OTP";

            //    printDialog1.PrinterSettings.PrintToFile = true;
            //    printDocumentOtp.PrinterSettings.PrintFileName = directory + fileName;
            //    printDocumentOtp.PrinterSettings.PrintToFile = true;
            //    printDocumentOtp.Print();

            //    printDialog1.PrinterSettings.PrintToFile = false;
            //    printDocumentOtp.PrinterSettings.PrintToFile = false;
            //    printDialog1.PrinterSettings.PrinterName = printerName;
            //    printDocumentOtp.PrinterSettings.PrinterName = printerName;
            //}
            //catch (Exception e1)
            //{
            //    new LogWriter(e1);
            //    MessageBox.Show(e1.Message + Environment.NewLine + "PDF file not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void OpenRNBT_Click(object sender, EventArgs e)
        {
            long id = 0;

            if (!RNIDTB.Text.Equals(""))
                id = long.Parse(RNIDTB.Text);
            else
                return;

            String filePath = Properties.Settings.Default.DefaultFolder + "\\RAC\\EXE " + id + ".pdf";

            try
            {
                Process.Start(filePath);

                /*
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = "acroRd32.exe"; //not the full application path
                myProcess.StartInfo.Arguments = "/A \"page=2=OpenActions\" " + filePath;
                myProcess.Start();
                */
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }

        private void StornoBT_Click(object sender, EventArgs e)
        {

        }
    }
}
