using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.CopyPrintForms
{
    public partial class cISS : Form
    {
        List<long> ISSids = new List<long>();
        List<long> customerID = new List<long>();
        List<String> dateCreated = new List<String>();
        List<Company> cmpList = new List<Company>();

        QueryCommands qc = new QueryCommands();

        int rb = 0;
        long ISSid = 0;

        DateTime startDate = DateTime.Now;

        List<Part> partList = new List<Part>();
        List<PartSifrarnik> allParts = new List<PartSifrarnik>();
        List<String> sifrarnikArr = new List<String>();
        List<String> workDone = new List<String>();
        List<List<Part>> groupedGoodPartsCode = new List<List<Part>>();
        List<ISSparts> listIssParts = new List<ISSparts>();
        List<String> allISSInfo = new List<String>();

        Part newSendPart = new Part();
        Part mainPart = new Part();
        String totalTime;

        Company cmpCust = new Company();
        Company cmpM = new Company();
        MainCmp mm = new MainCmp();

        Object obj;
        Boolean onlyOneTime = true;
        Boolean oneTimeISSSelectorCb = true;

        int obrJed = Properties.Settings.Default.ObracunskaJedinica;
        //Boolean pictureOn = false;

        public cISS()
        {
            InitializeComponent();
        }

        private void cISS_Load(object sender, EventArgs e)
        {
            fillSifrarnik();

            mm.GetMainCmpInfoByID(Properties.Settings.Default.CmpID);
            cmpM = mm.MainCmpToCompany();

            partList = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "sng");

            List<Part> goodParts = new List<Part>();
            goodParts = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "g");

            allParts = qc.GetPartsAllSifrarnik();

            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("ID");
            listView1.Columns.Add("Date");
            listView1.Columns.Add("UserID");
            listView1.Columns.Add("CustomerID");
            listView1.Columns.Add("PartID");
            listView1.Columns.Add("Closed");
            listView1.Columns.Add("TotalTime");

            listView2.View = View.Details;

            listView2.Columns.Add("RB");
            listView2.Columns.Add("Name");
            listView2.Columns.Add("CodeO");
            listView2.Columns.Add("SNO");
            listView2.Columns.Add("CNO");
            listView2.Columns.Add("CodeN");
            listView2.Columns.Add("SNN");
            listView2.Columns.Add("CNN");
            listView2.Columns.Add("Date");
            listView2.Columns.Add("Time");
            listView2.Columns.Add("Work done");
            listView2.Columns.Add("Comment");

            customerID = qc.GetAllISScustomerID(1);
            if (customerID[0] != -1)
            {
                for (int i = 0; i < customerID.Count(); i++)
                {
                    this.comboBox2.Items.Add(customerID[i]);
                }

            }

            dateCreated = qc.GetAllISSdateCreated(1);
            if (!dateCreated[0].Equals("nok"))
            {
                for (int i = 0; i < dateCreated.Count(); i++)
                {
                    this.comboBox3.Items.Add(dateCreated[i]);
                }

            }

            Company temCmp = new Company();
            cmpList = temCmp.GetAllCompanyInfoSortByName();

            if (cmpList.Count != 0)
            {
                for (int i = 0; i < temCmp.Count; i++)
                {
                    this.comboBox4.Items.Add(cmpList[i].Name);
                }

            }

            ISSids = qc.GetAllISSOpenClose(1);

            for (int i = 0; i < ISSids.Count; i++)
            {
                ISSSelectorCb.Items.Add(ISSids[i]);
            }

            for (int i = 0; i < 12; i++)
            {
                listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            Program.LoadStop();
            this.Focus();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "OTP Selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            if (sifrarnikArr.Count <= 0)
            {
                Thread myThread = new Thread(fillSifrarnik);
                myThread.Start();

                lw.LogMe(function, usedQC, data, Result);

                return;
            }

            listView2.Items.Clear();
            listView2.Refresh();

            var item = listView1.SelectedItems;
            if (item.Count == 0)
            {
                pictureBox1.Image = Properties.Resources.LoadDataOff;
                return;
            }

            obj = sender;
            ISSSelectorCb.Text = item[0].SubItems[1].Text;

            ISSSelectorCb_SelectedIndexChanged(sender, e);
            oneTimeISSSelectorCb = true;

            Result = "Added";
            lw.LogMe(function, usedQC, data, Result);

            //SystemSounds.Hand.Play();
        }

        private void ISSSelectorCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oneTimeISSSelectorCb)
            {
                if (obj != null)
                {
                    obj = null;
                    oneTimeISSSelectorCb = false;
                }


                ///////////////// LogMe ////////////////////////
                String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                String usedQC = "ISS loaded";
                String data = "";
                String Result = "";
                LogWriter lw = new LogWriter();
                ////////////////////////////////////////////////
                ///

                try
                {
                    CleanMe();
                    listView2.Clear();

                    listView2.View = View.Details;

                    listView2.Columns.Add("RB");
                    listView2.Columns.Add("Name");
                    listView2.Columns.Add("CodeO");
                    listView2.Columns.Add("SNO");
                    listView2.Columns.Add("CNO");
                    listView2.Columns.Add("CodeN");
                    listView2.Columns.Add("SNN");
                    listView2.Columns.Add("CNN");
                    listView2.Columns.Add("Date");
                    listView2.Columns.Add("Time");
                    listView2.Columns.Add("Work done");
                    listView2.Columns.Add("Comment");

                    long issID = long.Parse(ISSSelectorCb.SelectedItem.ToString());
                    Part mainPr = new Part();
                    //List<String> allISSInfo = new List<String>();

                    ISSid = issID;

                    allISSInfo = qc.GetAllISSInfoById(issID);

                    if (allISSInfo[0].Equals("nok"))
                        return;

                    totalTime = allISSInfo[6] + ":00";
                    
                    mainPart = qc.SearchPartsInAllTablesBYPartID(long.Parse(allISSInfo[4]))[0];

                    PartTb.Text = mainPart.CodePartFull.ToString();

                    cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));

                    listIssParts.Clear();
                    listIssParts = qc.GetAllISSPartsByISSid(issID);

                    NameTb.Text = Decoder.ConnectCodeName(sifrarnikArr, mainPart);
                    SNTb.Text = mainPart.SN;
                    CNTb.Text = mainPart.CN;
                    DateInTb.Text = mainPart.DateIn;
                    DateSentTb.Text = mainPart.DateSend;
                    IDTb.Text = mainPart.PartID.ToString();

                    for (int i = 0; i < listIssParts.Count; i++)
                    {
                        ListViewItem lvi1 = new ListViewItem(listIssParts[i].RB.ToString().ToString());

                        lvi1.SubItems.Add(qc.PartInfoByFullCodeSifrarnik(mainPart.PartialCode).FullName);

                        //////////////////////////////////////////////////////////////

                        if (listIssParts[i].PrtO.CodePartFull == 0)
                            lvi1.SubItems.Add("");
                        else
                            lvi1.SubItems.Add(listIssParts[i].PrtO.CodePartFull.ToString());

                        if (listIssParts[i].PrtO.SN == null)
                            lvi1.SubItems.Add("");
                        else
                            lvi1.SubItems.Add(listIssParts[i].PrtO.SN.ToString());

                        if (listIssParts[i].PrtO.CN == null)
                            lvi1.SubItems.Add("");
                        else
                            lvi1.SubItems.Add(listIssParts[i].PrtO.CN.ToString());

                        //////////////////////////////////////////////////////////////

                        if (listIssParts[i].PrtN.CodePartFull == 0)
                            lvi1.SubItems.Add("");
                        else
                            lvi1.SubItems.Add(listIssParts[i].PrtN.CodePartFull.ToString());

                        if (listIssParts[i].PrtN.SN == null)
                            lvi1.SubItems.Add("");
                        else
                            lvi1.SubItems.Add(listIssParts[i].PrtN.SN.ToString());

                        if (listIssParts[i].PrtN.CN == null)
                            lvi1.SubItems.Add("");
                        else
                            lvi1.SubItems.Add(listIssParts[i].PrtN.CN.ToString());

                        //////////////////////////////////////////////////////////////

                        lvi1.SubItems.Add(allISSInfo[1]);

                        lvi1.SubItems.Add(listIssParts[i].Time.ToString());
                        lvi1.SubItems.Add(listIssParts[i].Work.ToString());
                        lvi1.SubItems.Add(listIssParts[i].Comment.ToString());

                        if (listView2.Items.Count > 1)
                            listView2.EnsureVisible(listView2.Items.Count - 1);

                        listView2.Items.Add(lvi1);
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                        listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                    }

                    rb = listView2.Items.Count + 1;

                    data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + listIssParts.Count + ", " + ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                    Result = "ISS selected " + ISSid;
                    lw.LogMe(function, usedQC, data, Result);

                    SystemSounds.Hand.Play();
                }
                catch (Exception e1)
                {
                    data = ISSid + Environment.NewLine;
                    Result = e1.Message;
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                oneTimeISSSelectorCb = true;
            }
        }

        private void CleanMe()
        {
            ISSid = 0;

            PartTb.ResetText();
            listView2.Clear();

            listView2.View = View.Details;

            listView2.Columns.Add("RB");
            listView2.Columns.Add("Name");
            listView2.Columns.Add("CodeO");
            listView2.Columns.Add("SNO");
            listView2.Columns.Add("CNO");
            listView2.Columns.Add("CodeN");
            listView2.Columns.Add("SNN");
            listView2.Columns.Add("CNN");
            listView2.Columns.Add("Date");
            listView2.Columns.Add("Time");
            listView2.Columns.Add("Work done");
            listView2.Columns.Add("Comment");

            for (int i = 0; i < 12; i++)
            {
                listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            NameTb.ResetText();
            SNTb.ResetText();
            CNTb.ResetText();
            DateInTb.ResetText();
            DateSentTb.ResetText();
            IDTb.ResetText();
        }

        private void fillSifrarnik()
        {
            QueryCommands qc2 = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            List<String> tresultArr = new List<string>();
            int stop = 0;

            try
            {
                while (tresultArr.Count == 0 || tresultArr[0].Equals("nok"))
                {
                    stop++;
                    sifrarnikArr.Clear();
                    tresultArr.Clear();
                    tresultArr = qc2.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);

                    if (stop == 100)
                    {
                        //MessageBox.Show("Cant load 'sifrarnik'.");
                        String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        String usedQC = "Loading sifrarnik";
                        String data = "Break limit reached, arr cnt = " + tresultArr.Count;
                        String Result = "";
                        LogWriter lw = new LogWriter();

                        Result = "Cant load 'sifrarnik'.";
                        lw.LogMe(function, usedQC, data, Result);

                        pictureBox1.Image = Properties.Resources.LoadDataOff;
                        //pictureOn = false;

                        this.Refresh();

                        break;
                    }
                }

                sifrarnikArr = tresultArr;

                if (sifrarnikArr.Count > 0 && stop < 100)
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOn;
                    //pictureOn = true;
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOff;
                    //pictureOn = false;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }

        /*
        public void ChangeColor(string color)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(ChangeColor), new object[] { color });
                return;
            }
        }
        */

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            getISSList(comboBox2.Text, "CMP");

            ISSSelectorCb.ResetText();
            comboBox3.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            getISSList(comboBox3.Text, "DATE");

            ISSSelectorCb.ResetText();
            comboBox2.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<String> arr = new List<string>();
            arr = qc.CompanyInfoByName(comboBox4.Text);
            getISSList(arr[0], "NAME");

            ISSSelectorCb.ResetText();
            comboBox2.ResetText();
            comboBox3.ResetText();
        }

        private void getISSList(String value, String tipe)
        {

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = tipe;
            String data = value;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            listView1.Items.Clear();
            listView2.Items.Clear();

            List<String> arr = new List<string>();
            int rb;
            QueryCommands qc1 = new QueryCommands();

            switch (tipe)
            {
                case ("ID"):
                    arr = qc1.GetAllInfoISSByClosed("ID", value);
                    break;
                case ("CMP"):
                    arr = qc1.GetAllInfoISSByClosed("CustomerID", value);
                    break;
                case ("DATE"):
                    arr = qc1.GetAllInfoISSByClosed("Date", value);
                    break;
                case ("NAME"):
                    arr = qc1.GetAllInfoISSByClosed("CustomerID", value); //Name
                    break;

            }

            try
            {
                rb = listView1.Items.Count + 1;

                if (arr[0].Equals("nok"))
                {
                    data = value;
                    Result = "Selected ISS does not exist.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }

                for (int i = 0; i < arr.Count; i = i + 7)
                {
                    ListViewItem lvi1 = new ListViewItem(rb.ToString());

                    lvi1.SubItems.Add(arr[i]);
                    lvi1.SubItems.Add(arr[i + 1]);
                    lvi1.SubItems.Add(arr[i + 2]);
                    lvi1.SubItems.Add(arr[i + 3]);
                    lvi1.SubItems.Add(arr[i + 4]);
                    lvi1.SubItems.Add(arr[i + 5]);
                    lvi1.SubItems.Add(arr[i + 6]);

                    listView1.Items.Add(lvi1);

                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);

                    rb = listView1.Items.Count + 1;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            for (int i = 0; i < 6; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            Result = "Added";
            lw.LogMe(function, usedQC, data, Result);

            //SystemSounds.Hand.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            comboBox2.ResetText();
            comboBox3.ResetText();
            comboBox4.ResetText();
            ISSSelectorCb.ResetText();

            PartTb.ResetText();
            NameTb.ResetText();
            SNTb.ResetText();
            CNTb.ResetText();
            DateInTb.ResetText();
            DateSentTb.ResetText();
            IDTb.ResetText();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Print";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            int h = int.Parse(totalTime.Split(':')[0]);
            int m = int.Parse(totalTime.Split(':')[1]);

            totalTime = String.Format("{0:00}:{1:00}", h, m);
           
            PrintMeISS pr = new PrintMeISS(cmpCust, cmpM, sifrarnikArr, mainPart, listIssParts, ISSid.ToString(), Properties.strings.ServiceReport, Properties.strings.customer, false, allISSInfo[1], totalTime, true);
            pr.Print(e);

            if (onlyOneTime)
            {
                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + listIssParts.Count + ", " + ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "Print page called";
                lw.LogMe(function, usedQC, data, Result);

                onlyOneTime = false;
            }

            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.Save();
        }

        private void printPrewBT_Click(object sender, EventArgs e)
        {
            onlyOneTime = true;

            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialog1.ShowDialog();
        }

        private void selectPrinterPrintBtn_Click(object sender, EventArgs e)
        {
            onlyOneTime = true;
            
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = printDocument1;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog pdfSaveDialog = new SaveFileDialog();

                if (printDialog1.PrinterSettings.PrinterName == "Microsoft Print to PDF")
                {   // force a reasonable filename
                    string basename = System.IO.Path.GetFileNameWithoutExtension("ISS " + ISSid.ToString());
                    string directory = System.IO.Path.GetDirectoryName("ISS " + ISSid.ToString());
                    printDocument1.PrinterSettings.PrintToFile = true;
                    // confirm the user wants to use that name
                    pdfSaveDialog.InitialDirectory = directory;
                    pdfSaveDialog.FileName = basename + ".pdf";
                    pdfSaveDialog.Filter = "PDF File|*.pdf";
                    result = pdfSaveDialog.ShowDialog();
                    if (result != DialogResult.Cancel)
                        printDocument1.PrinterSettings.PrintFileName = pdfSaveDialog.FileName;
                }

                if (result != DialogResult.Cancel)  // in case they canceled the save as dialog
                {
                    printDocument1.Print();
                    MessageBox.Show("Saved to location: " + Environment.NewLine + pdfSaveDialog.FileName, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    printPrewBT_Click(sender, e);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
        }
    }
}
