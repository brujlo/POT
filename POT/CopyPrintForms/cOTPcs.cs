using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.CopyPrintForms
{
    public partial class cOTPcs : Form
    {
        List<long> otpID = new List<long>();
        List<long> customerID = new List<long>();
        List<String> dateCreated = new List<String>();
        List<Company> cmpList = new List<Company>();
        List<String> sifrarnikArr = new List<String>();
        List<String> userArr = new List<string>();
        Company cmpR = new Company();
        Company cmpS = new Company();

        String datumIzradeM;
        String izradioUserM;
        String izradioRegijaM;

        String OTPNumber;
        List<Part> partListPrint = new List<Part>();
        String napomenaOTPPrint;
        Branch br = new Branch();

        QueryCommands qc = new QueryCommands();

        Boolean pictureOn = true;

        public cOTPcs()
        {
            InitializeComponent();
            new Thread(fillSifrarnik).Start();
        }

        private void cOTPcs_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            
            listView1.Columns.Add("RB");
            listView1.Columns.Add("otpID");
            listView1.Columns.Add("customerID");
            listView1.Columns.Add("dateCreated");
            listView1.Columns.Add("napomena");
            listView1.Columns.Add("primID");
            listView1.Columns.Add("userID");
            listView1.Columns.Add("fillID");

            listView2.View = View.Details;

            listView2.Columns.Add("RB");
            listView2.Columns.Add("Name");
            listView2.Columns.Add("CodePartFull");
            listView2.Columns.Add("SN");
            listView2.Columns.Add("CN");

            //Thread myThread = new Thread(fillSifrarnik);
            //myThread.Start();

            otpID = qc.GetAllOTPID();
            if (otpID[0] != -1)
            {
                for (int i = 0; i < otpID.Count(); i++)
                {
                    this.comboBox1.Items.Add(otpID[i]);
                }
            }

            customerID = qc.GetAllOTPcustomerID();
            if (customerID[0] != -1)
            {
                for (int i = 0; i < customerID.Count(); i++)
                {
                    this.comboBox2.Items.Add(customerID[i]);
                }

            }

            dateCreated = qc.GetAllOTPdateCreated();
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

            Program.LoadStop();
            this.Focus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            getOTPList(comboBox1.Text, "ID");

            comboBox2.ResetText();
            comboBox3.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            getOTPList(comboBox2.Text, "CMP");

            comboBox1.ResetText();
            comboBox3.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            getOTPList(comboBox3.Text, "DATE");

            comboBox1.ResetText();
            comboBox2.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<String> arr = new List<string>();
            arr = qc.CompanyInfoByName(comboBox4.Text);
            getOTPList(arr[0], "NAME");

            comboBox1.ResetText();
            comboBox2.ResetText();
            comboBox3.ResetText();
        }

        private void getOTPList(String value, String tipe)
        {
            datumIzradeM = "";
            izradioUserM = "";
            izradioRegijaM = "";

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = tipe;
            String data = value;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            listView1.Items.Clear();

            List<String> arr = new List<string>();
            int rb;
            QueryCommands qc1 = new QueryCommands();

            switch (tipe)
            {
                case ("ID"):
                    arr = qc1.GetAllInfoOTPBy("otpID", value);
                    break;
                case ("CMP"):
                    arr = qc1.GetAllInfoOTPBy("customerID", value);
                    break;
                case ("DATE"):
                    arr = qc1.GetAllInfoOTPBy("dateCreated", value);
                    break;
                case ("NAME"):
                    arr = qc1.GetAllInfoOTPBy("customerID", value);
                    break;

            }

            userArr = arr;

            try
            {
                rb = listView1.Items.Count + 1;

                if (arr[0].Equals("nok"))
                {
                    data = value;
                    Result = "Selected OTP does not exist.";
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

            for (int i = 0; i < 7; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            Result = "Added";
            lw.LogMe(function, usedQC, data, Result);

            SystemSounds.Hand.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
        }

        private void printPrewBT_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            printPreviewDialogOtp.Document = printDocumentOtp;
            printPreviewDialogOtp.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialogOtp.ShowDialog();
        }

        private void selectPrinterPrintBtn_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = printDocumentOtp;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog pdfSaveDialog = new SaveFileDialog();

                if (printDialog1.PrinterSettings.PrinterName == "Microsoft Print to PDF")
                {   // force a reasonable filename
                    string basename = System.IO.Path.GetFileNameWithoutExtension("OTP " + OTPNumber.ToString());
                    string directory = System.IO.Path.GetDirectoryName("OTP " + OTPNumber.ToString());
                    printDocumentOtp.PrinterSettings.PrintToFile = true;
                    // confirm the user wants to use that name
                    pdfSaveDialog.InitialDirectory = directory;
                    pdfSaveDialog.FileName = basename + ".pdf";
                    pdfSaveDialog.Filter = "PDF File|*.pdf";
                    result = pdfSaveDialog.ShowDialog();
                    if (result != DialogResult.Cancel)
                        printDocumentOtp.PrinterSettings.PrintFileName = pdfSaveDialog.FileName;
                }

                if (result != DialogResult.Cancel)  // in case they canceled the save as dialog
                {
                    printDocumentOtp.Print();
                    MessageBox.Show("Saved to location: " + Environment.NewLine + pdfSaveDialog.FileName, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    printPrewBT_Click(sender, e);
                }
            }
        }

        private void printDocumentOtp_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Print";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            if (br.FilID != 0)
            {
                PrintMe pr = new PrintMe(cmpR, cmpS, sifrarnikArr, partListPrint, OTPNumber, napomenaOTPPrint, Properties.strings.DELIVERY, Properties.strings.customer, true, br);
                pr.datumIzrade = datumIzradeM;
                pr.izradioUser = izradioUserM;
                pr.izradioRegija = izradioRegijaM;
                pr.Print(e);
                data = cmpR + ", " + cmpS + ", " + sifrarnikArr + ", " + partListPrint + ", " + OTPNumber + ", " + napomenaOTPPrint + ", " + Properties.strings.DELIVERY + ", " + Properties.strings.customer + ", true, " + br;
            }
            else
            {
                PrintMe pr = new PrintMe(cmpR, cmpS, sifrarnikArr, partListPrint, OTPNumber, napomenaOTPPrint, Properties.strings.DELIVERY, Properties.strings.customer, true);
                pr.datumIzrade = datumIzradeM;
                pr.izradioUser = izradioUserM;
                pr.izradioRegija = izradioRegijaM;
                pr.Print(e);
                data = cmpR + ", " + cmpS + ", " + sifrarnikArr + ", " + partListPrint + ", " + OTPNumber + ", " + napomenaOTPPrint + ", " + Properties.strings.DELIVERY + ", " + Properties.strings.customer + ", true";
            }
            
            Result = "Print page called";
            lw.LogMe(function, usedQC, data, Result);

            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.Save();
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
                    tresultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);

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
                        this.label7.ResetText();
                        pictureOn = false;

                        this.Refresh();

                        break;
                    }
                }

                sifrarnikArr = tresultArr;

                if (sifrarnikArr.Count > 0 && stop < 100)
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOn;
                    this.label7.ResetText();
                    pictureOn = true;
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOff;
                    this.label7.ResetText();
                    pictureOn = false;
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

            if(color.Equals("Green"))
                this.button4.BackColor = Color.Green;
            else
                this.button4.BackColor = Color.Red;
        }
        */

        private void fillCmp(long code)
        {
            try
            {
                cmpR.Clear();
                cmpS.Clear();
                cmpR.GetCompanyInfoByID(code);

                MainCmp mpc = new MainCmp();
                mpc.GetMainCmpByName(Properties.Settings.Default.CmpName);
                cmpS.Clear();
                cmpS = mpc.MainCmpToCompany();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
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

            this.label7.Text = "Working...";
            this.Refresh();

            br.Clear();

            if (!pictureOn)
            {
                Thread myThread = new Thread(fillSifrarnik);
                myThread.Start();
            }

            listView2.Items.Clear();
            listView2.Refresh();
            this.label5.Text = "";

            List<long> prtList = new List<long>();
            List<Part> parts = new List<Part>();
            long otpID;
            int rb;
            Part prt = new Part();

            var item = listView1.SelectedItems;
            if (item.Count == 0)
            {
                this.label7.Text = "";
                return;
            }

            int index = item[0].Index;

            long threadCmpCode = long.Parse(item[0].SubItems[2].Text);
            Thread myThreadCmp = new Thread(() => fillCmp(threadCmpCode));
            myThreadCmp.Start();

            otpID = long.Parse(item[0].SubItems[1].Text);
            prtList = qc.GetAllpartIDByOtpID(otpID);
            this.label5.Text = "Selected OTP: " + otpID.ToString();

            datumIzradeM = userArr[(7 * index) + 2];
            izradioUserM = userArr[(7 * index) + 5];
            QueryCommands qc4 = new QueryCommands();
            izradioRegijaM = qc4.User(WorkingUser.Username, WorkingUser.Password, izradioUserM)[7];

            if (prtList[0] == -1)
            {
                data = otpID.ToString();
                Result = "Can not find parts by given otpID.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result);
                return;
            }


            try
            {
                rb = listView2.Items.Count + 1;

                parts = prt.GetListOfPartsFromPartsPartsPoslanoByID(prtList);

                if (parts.Count == 0)
                {
                    data = otpID.ToString();
                    Result = "Selected OTP (" + otpID.ToString() + ") does not exist or do not have parts.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }

                var groupedPartsListSN = parts.GroupBy(c => c.CodePartFull).Select(grp => grp.ToList()).ToList();

                int i = 0;
                for (int k = 0; k < groupedPartsListSN.Count; k++)
                {
                    String name = (sifrarnikArr[sifrarnikArr.IndexOf(Decoder.GetFullPartCodeStr(parts[i].PartialCode.ToString())) - 1]);

                    for (i = 0; i < groupedPartsListSN[k].Count; i++)
                    {
                        ListViewItem lvi2 = new ListViewItem(rb.ToString());
                        lvi2.SubItems.Add(name);
                        lvi2.SubItems.Add(groupedPartsListSN[k][i].CodePartFull.ToString());
                        lvi2.SubItems.Add(groupedPartsListSN[k][i].SN);
                        lvi2.SubItems.Add(groupedPartsListSN[k][i].CN);

                        if (listView2.Items.Count > 1)
                            listView2.EnsureVisible(listView2.Items.Count - 1);

                        listView2.Items.Add(lvi2);

                        rb = listView2.Items.Count + 1;

                        if (data.Equals(""))
                            data = groupedPartsListSN[k][i].CodePartFull.ToString() + ", " + groupedPartsListSN[k][i].SN + ", " + groupedPartsListSN[k][i].CN;
                        else
                            data = data + Environment.NewLine + "             " + groupedPartsListSN[k][i].CodePartFull.ToString() + ", " + groupedPartsListSN[k][i].SN + ", " + groupedPartsListSN[k][i].CN;
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            for (int i = 0; i < 5; i++)
            {
                listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            OTPNumber = string.Format("{0:00/000}", otpID); 
            partListPrint = parts;
            napomenaOTPPrint = item[0].SubItems[4].Text;
            if (!item[0].SubItems[7].Text.Equals(""))
                br.GetFilByID(long.Parse(item[0].SubItems[7].Text));

            if (!cmpR.Name.Equals(""))
                this.label8.Text = "cmpR: OK";
            else
                this.label8.Text = "cmpR: NOK";

            if (!cmpS.Name.Equals(""))
                this.label9.Text = "cmpS: OK";
            else
                this.label9.Text = "cmpS: NOK";

            if (br.FilID != 0)
                this.label10.Text = "br: OK";
            else
                this.label10.Text = "br: NOK";

            Result = "Added";
            lw.LogMe(function, usedQC, data, Result);

            SystemSounds.Hand.Play();

            this.label7.ResetText();
        }
    }
}
