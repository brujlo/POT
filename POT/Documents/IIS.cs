using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.Documents
{
    public partial class IIS : Form
    {
        int rb = 1;
        List<String> resultArr = new List<String>();
        List<String> resultArrSearchCode = new List<String>();
        static List<String> sifrarnikArr = new List<String>();

        Boolean isIISSaved = false;
        Company cmpS = new Company();
        Company cmpR = new Company();
        String IISNumber;
        List<Part> partListPrint = new List<Part>();
        List<Part> partsArr = new List<Part>();
        List<Part> partsArrCB5 = new List<Part>();
        String napomenaIISPrint;

        List<Company> resultArrC = new List<Company>();
        QueryCommands qc = new QueryCommands();
        ConnectionHelper cn = new ConnectionHelper();

        Boolean isFocused = false;

        public IIS()
        {
            InitializeComponent();
        }

        private void IIS_Load(object sender, EventArgs e)
        {
            comboBox3.Text = Properties.Settings.Default.MainCompanyCode;
            comboBox4.Text = Properties.Settings.Default.MainCompanyCode;
            //this.printPrewBT.Enabled = false;

            Thread myThread = new Thread(fillComboBoxes);
            myThread.Start();

            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("Name");
            listView1.Columns.Add("Code");
            listView1.Columns.Add("SN");
            listView1.Columns.Add("CN");
            listView1.Columns.Add("Condition");

            try
            {
                Company cmpList = new Company();

                resultArrC = cmpList.GetAllCompanyInfoSortByName();

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.comboBox1.Items.Add(resultArrC[i].Name);
                    }

                }

                resultArr.Clear();

                resultArr = qc.AllCompanyInfoSortCode(WorkingUser.Username, WorkingUser.Password);

                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 10; i < resultArr.Count(); i = i + 14)
                    {
                        comboBox4.Items.Add(resultArr[i]);
                        comboBox3.Items.Add(resultArr[i]);
                    }

                }

                resultArr.Clear();
                resultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);

                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 2)
                    {
                        comboBox2.Items.Add(resultArr[i]);
                        //resultArrSearchName.Add(resultArr[i]);
                        resultArrSearchCode.Add(resultArr[i + 1]);
                    }

                }

                partsArr = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "sg");
                partsArrCB5 = new List<Part>(partsArr);
                if (partsArr.Count > 0)
                {
                    for (int i = 0; i < partsArr.Count; i++)
                    {
                        comboBox5.Items.Add(partsArr[i].CodePartFull + " # " + partsArr[i].SN + " # " + partsArr[i].CN);
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);

                Program.LoadStop();
                this.Focus();
            }
            Program.LoadStop();
            this.Focus();
        }

        static void fillComboBoxes()
        {
            QueryCommands qc1 = new QueryCommands();

            List<String> tsendArr = new List<string>();
            List<String> tresultArr = new List<string>();

            try
            {
                tresultArr = qc1.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);
                tsendArr.Clear();

                if (!tresultArr[0].Equals("nok"))
                {
                    sifrarnikArr = tresultArr;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isFocused)
            {
                int indx = comboBox5.SelectedIndex;
                Part tmpPart = partsArrCB5[indx];
                addToList(false, tmpPart.CodePartFull, tmpPart.SN, tmpPart.CN);
                partsArrCB5.RemoveAt(indx);

                comboBox5.Items.RemoveAt(indx);
            }
        }

        private void addToList(Boolean clear, long mCode, String mSN, String mCN)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Add to list";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                ListViewItem lvi1 = new ListViewItem();
                rb = listView1.Items.Count + 1;
                lvi1.Text = rb.ToString();

                //if ((sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString()))) < 0) //DecoderBB
                if ((sifrarnikArr.IndexOf(Decoder.GetFullPartCodeStr(mCode.ToString()))) < 0)
                {
                    data = mCode.ToString();
                    Result = "Selected code does not exist in DB.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    textBox1.SelectAll();
                    return;
                }
                //lvi1.SubItems.Add(sifrarnikArr[sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString())) - 1]); //DecoderBB
                lvi1.SubItems.Add(sifrarnikArr[sifrarnikArr.IndexOf(Decoder.GetFullPartCodeStr(mCode.ToString())) - 1]);
                lvi1.SubItems.Add(mCode.ToString());
                lvi1.SubItems.Add(mSN);
                lvi1.SubItems.Add(mCN);
                lvi1.SubItems.Add("sg");

                if (listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                listView1.Items.Add(lvi1);

                if (data.Equals(""))
                    data = mCode.ToString() + ", " + mSN + ", " + mCN + ", g";
                else
                    data = data + Environment.NewLine + "             " + mCode.ToString() + ", " + mSN + ", " + mCN + ", g";
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            if(clear)
            {
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }

            textBox1.SelectAll();
            isFocused = true;
            textBox1.Focus();
            isFocused = false;

            Result = "Added";
            lw.LogMe(function, usedQC, data, Result);

            SystemSounds.Hand.Play();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label10.Text = "Letters left " + (200 - textBox4.TextLength).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Add to list";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                ListViewItem lvi1 = new ListViewItem();
                rb = listView1.Items.Count + 1;
                lvi1.Text = rb.ToString();

                //if ((sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString()))) < 0) //DecoderBB
                if ((sifrarnikArr.IndexOf(Decoder.GetFullPartCodeStr(textBox1.Text))) < 0)
                {
                    data = textBox1.Text;
                    Result = "Selected code does not exist in DB.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    textBox1.SelectAll();
                    return;
                }

                addToList(true, long.Parse(textBox1.Text), textBox2.Text, textBox3.Text);
                //lvi1.SubItems.Add(sifrarnikArr[sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString())) - 1]); //DecoderBB

                if (data.Equals(""))
                    data = textBox1.Text + ", " + textBox2.Text + ", " + textBox3.Text + ", ng";
                else
                    data = data + Environment.NewLine + "             " + textBox1.Text + ", " + textBox2.Text + ", " + textBox3.Text + ", ng";
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                textBox2.SelectAll();
                textBox2.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                textBox3.SelectAll();
                textBox3.Focus();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                SystemSounds.Hand.Play();
                e.Handled = true;
                button1_Click(sender, e);
                textBox1.SelectAll();
                isFocused = true;
                textBox1.Focus();
                isFocused = false;

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.SelectAll();

            label2.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).Name.Trim();
            label3.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).Address.Trim();
            label4.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).OIB.Trim();
            label5.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).Contact.Trim();

            isFocused = true;
            textBox1.Focus();
            isFocused = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = comboBox4.Text + comboBox3.Text + string.Format("{0:000000000}", int.Parse(resultArrSearchCode.ElementAt(comboBox2.SelectedIndex)));
                textBox1.SelectAll();
                isFocused = true;
                textBox1.Focus();
                isFocused = false;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            isFocused = true;
            textBox1.Focus();
            isFocused = false;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            isFocused = true;
            textBox1.Focus();
            isFocused = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Remove selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            var items = listView1.SelectedItems;
            int k = 1;
            String fCode = "";
            String fSN = "";
            String fCN = "";

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
                fCode = item.SubItems[2].Text;
                fSN = item.SubItems[3].Text;
                fCN = item.SubItems[4].Text;

                if (data.Equals(""))
                    data = item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5];
                else
                    data = data + Environment.NewLine + "             " + item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5];
            }

            foreach (ListViewItem item in listView1.Items)
            {
                if (listView1.SelectedItems != null && listView1.Items.Count != 0)
                {
                    item.SubItems[0].Text = k.ToString();
                    k++;
                }
            }

            Result = "Removed";
            lw.LogMe(function, usedQC, data, Result);

            foreach (Part part in partsArr)
            {
                if (part.CodePartFull == long.Parse(fCode) && part.SN.ToUpper().Equals(fSN.ToUpper()) && part.CN.ToUpper().Equals(fCN.ToUpper()))
                {
                    comboBox5.Items.Add(part.CodePartFull.ToString() + " # " + part.SN + " # " + part.CN);
                    partsArrCB5.Add(part);
                    break;
                }
            }

            rb = listView1.Items.Count + 1;
            textBox1.SelectAll();
            isFocused = true;
            textBox1.Focus();
            isFocused = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Save to db";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            IISNumber = "";

            if (this.label2.Text.Equals("Name"))
            {
                Result = "Please select company, nothing done.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result);
                textBox1.SelectAll();
                isFocused = true;
                textBox1.Focus();
                isFocused = false;
            }
            else if (this.listView1.Items.Count == 0)
            {
                Result = "There is no items in list, nothing done.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result);
                textBox1.SelectAll();
                isFocused = true;
                textBox1.Focus();
                isFocused = false;
            }
            else
            {
                //List<Part> listOfOtpPartsPrimka = new List<Part>();
                MainCmp mpc = new MainCmp();
                mpc.GetMainCmpByName(Properties.Settings.Default.CmpName);
                cmpS.Clear();
                cmpS = mpc.MainCmpToCompany();

                if (cmpR.GetCompanyByName(label2.Text.Trim()))
                {
                    try
                    {
                        List<String> arr = new List<string>();
                        ConnectionHelper cn = new ConnectionHelper();
                        using (SqlConnection cnnIIS = cn.Connect(WorkingUser.Username, WorkingUser.Password))
                        {
                            if (cn.TestConnection(cnnIIS))
                            {
                                //Provjera da li se dijelovi nalaze u mom skladistu
                                QueryCommands qc = new QueryCommands();

                                for (int i = 0; i < listView1.Items.Count; i++) // vec imam provjeru gore kod unosa ali neka ostane(tamo je po imenu)
                                {
                                    if (qc.GetPartIDCompareCodeSNCNStorageState(WorkingUser.Username, WorkingUser.Password,
                                        long.Parse(listView1.Items[i].SubItems[2].Text),
                                        listView1.Items[i].SubItems[3].Text,
                                        listView1.Items[i].SubItems[4].Text,
                                        WorkingUser.RegionID, "sg")[0].Equals("nok"))
                                    {
                                        data = listView1.Items[i].SubItems[2].Text + "\n on position " + (listView1.Items[i].Index + 1);
                                        Result = "In your storage does not exist SG part with: \n\n Code: " + listView1.Items[i].SubItems[2].Text + "\n on position " + (listView1.Items[i].Index + 1) + "\n\nNothing Done.";
                                        lw.LogMe(function, usedQC, data, Result);
                                        MessageBox.Show(Result);
                                        textBox1.SelectAll();
                                        isFocused = true;
                                        textBox1.Focus();
                                        isFocused = false;
                                        return;
                                    }
                                }

                                //Provjera i spremanje u bazu
                                List<String> allRegions = new List<string>();
                                allRegions = qc.GetAllRegions();

                                int index = resultArrC.FindIndex(resultArrC => resultArrC.Name.Equals(label2.Text));

                                List<Part> partList = new List<Part>();
                                String napomenaIIS = textBox4.Text;

                                for (int i = 0; i < listView1.Items.Count; i++)
                                {
                                    PartSifrarnik tempSifPart = new PartSifrarnik();
                                    Part tempPart = new Part();

                                    foreach (Part part in partsArr)
                                    {
                                        tempSifPart.GetPart(Decoder.GetFullPartCodeStr(listView1.Items[i].SubItems[2].Text));
                                        
                                        if (part.PartialCode == tempSifPart.FullCode && part.SN.ToUpper().Equals(listView1.Items[i].SubItems[3].Text.ToUpper()) && part.CN.ToUpper().Equals(listView1.Items[i].SubItems[4].Text.ToUpper()) && !partList.Contains(part))
                                        {
                                            tempPart = part;
                                            tempPart.StorageID = WorkingUser.RegionID;
                                            tempPart.State = "sg";
                                            tempPart.CompanyO = Decoder.GetOwnerCode(listView1.Items[i].SubItems[2].Text);
                                            tempPart.CompanyC = Decoder.GetCustomerCode(listView1.Items[i].SubItems[2].Text);
                                            break;
                                        }
                                    }

                                    String tmpResult = qc.GetListPartsByPartIDFromParts(tempPart.PartID)[0];

                                    if (tmpResult.Equals("nok"))
                                    {
                                        data = listView1.Items[i].SubItems[2].Text + "\n on position " + (listView1.Items[i].Index + 1);
                                        Result = "There is no SG part in your storage with: \n\n Code: " + listView1.Items[i].SubItems[2].Text + "\n on position " + (listView1.Items[i].Index + 1) + ". \n\nNothing Done.";
                                        lw.LogMe(function, usedQC, data, Result);
                                        MessageBox.Show(Result);
                                        textBox1.SelectAll();
                                        isFocused = true;
                                        textBox1.Focus();
                                        isFocused = false;
                                        return;
                                    }
                                    else
                                    {
                                        tempPart.PartID = long.Parse(tmpResult);
                                        partList.Add(tempPart);
                                    }
                                }

                                IISNumber = qc.IISPrebaciIzServisa(WorkingUser.Username, WorkingUser.Password, partList, WorkingUser.RegionID, cmpS.ID, IISNumber);
                                if (!IISNumber.Equals("nok"))
                                {
                                    PovijestLog pl = new PovijestLog();
                                    Boolean saved = false;
                                    for (int k = 0; k < partList.Count; k++)
                                    {
                                        List<Part> tempPart = new List<Part>();
                                        tempPart.Clear();
                                        tempPart.Add(partList[k]);
                                        if (pl.SaveToPovijestLog(tempPart, DateTime.Now.ToString("dd.MM.yy."), napomenaIIS, cmpS.Name, "", "", "IIS " + Properties.Settings.Default.ShareDocumentName, "g"))
                                        {
                                            saved = true;
                                        }
                                        else
                                        {
                                            saved = false;
                                            Properties.Settings.Default.ShareDocumentName = "";
                                            break;
                                        }
                                    }

                                    if (Program.SaveDocumentsPDF) saveToPDF();

                                    if (saved)
                                    {
                                        Result = "DONE, document nbr. IIS '" + IISNumber + "'.";
                                        lw.LogMe(function, usedQC, data, Result);
                                        MessageBox.Show(Result);

                                        partListPrint.Clear();
                                        partListPrint = partList;

                                        isIISSaved = true;
                                        listView1.Clear();
                                        listView1.View = View.Details;

                                        listView1.Columns.Add("RB");
                                        listView1.Columns.Add("Name");
                                        listView1.Columns.Add("Code");
                                        listView1.Columns.Add("SN");
                                        listView1.Columns.Add("CN");
                                        listView1.Columns.Add("Condition");
                                        textBox4.Clear();
                                        napomenaIISPrint = napomenaIIS;
                                    }
                                    else
                                    {
                                        Result = "DONE, document nbr. 'IIS " + IISNumber + "', but not saved in PL.";
                                        lw.LogMe(function, usedQC, data, Result);
                                        MessageBox.Show(Result);

                                        partListPrint.Clear();
                                        partListPrint = partList;

                                        isIISSaved = true;
                                        listView1.Clear();
                                        listView1.View = View.Details;

                                        listView1.Columns.Add("RB");
                                        listView1.Columns.Add("Name");
                                        listView1.Columns.Add("Code");
                                        listView1.Columns.Add("SN");
                                        listView1.Columns.Add("CN");
                                        listView1.Columns.Add("Condition");
                                        textBox4.Clear();
                                        napomenaIISPrint = napomenaIIS;
                                    }
                                }
                                else
                                {
                                    Result = "Unknown error in QUERY.";
                                    lw.LogMe(function, usedQC, data, Result);
                                    MessageBox.Show(Result);
                                    napomenaIISPrint = "";
                                    isIISSaved = false;
                                }
                                //}
                            }
                            cnnIIS.Close();
                        }
                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        MessageBox.Show(e1.Message);
                        textBox1.SelectAll();
                        isFocused = true;
                        textBox1.Focus();
                        isFocused = false;
                    }
                }
                this.printPrewBT.Enabled = isIISSaved;
            }
        }

        private void printDocumentIIS_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Print";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            PrintMe pr = new PrintMe(cmpR, cmpS, sifrarnikArr, partListPrint, IISNumber, napomenaIISPrint, "IIS", "customer", false);
            pr.Print(e);

            data = cmpS + ", " + cmpR + ", " + sifrarnikArr + ", " + partListPrint + ", " + IISNumber + ", " + napomenaIISPrint + ", IIS, customer, false";
            Result = "Print page called";
            lw.LogMe(function, usedQC, data, Result);
        }

        private void printPrewBT_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            printPreviewDialogIIS.Document = printDocumentIIS;
            printPreviewDialogIIS.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialogIIS.ShowDialog();

            textBox1.SelectAll();
            isFocused = true;
            textBox1.Focus();
            isFocused = false;
        }

        private void selectPrinterPrintBtn_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = printDocumentIIS;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog pdfSaveDialog = new SaveFileDialog();

                if (printDialog1.PrinterSettings.PrinterName == "Microsoft Print to PDF")
                {   // force a reasonable filename
                    string basename = Path.GetFileNameWithoutExtension("IIS " + IISNumber.ToString());
                    string directory = Path.GetDirectoryName("IIS " + IISNumber.ToString());
                    printDocumentIIS.PrinterSettings.PrintToFile = true;
                    // confirm the user wants to use that name
                    pdfSaveDialog.InitialDirectory = directory;
                    pdfSaveDialog.FileName = basename + ".pdf";
                    pdfSaveDialog.Filter = "PDF File|*.pdf";
                    result = pdfSaveDialog.ShowDialog();
                    if (result != DialogResult.Cancel)
                        printDocumentIIS.PrinterSettings.PrintFileName = pdfSaveDialog.FileName;
                }

                if (result != DialogResult.Cancel)  // in case they canceled the save as dialog
                {
                    printDocumentIIS.Print();
                    MessageBox.Show("Saved to location: " + Environment.NewLine + pdfSaveDialog.FileName, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    printPrewBT_Click(sender, e);
                }
            }
        }

        private void saveToPDF()
        {
            String printerName = printDialog1.PrinterSettings.PrinterName;

            try
            {
                PrintDialog printDialog1 = new PrintDialog();
                printDialog1.Document = printDocumentIIS;

                printDialog1.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                if (!printDialog1.PrinterSettings.IsValid) return;

                if (!Directory.Exists(Properties.Settings.Default.DefaultFolder + "\\IIS"))
                    return;

                string fileName = "\\IIS " + IISNumber.ToString().Replace("/", "") + ".pdf";
                string directory = Properties.Settings.Default.DefaultFolder + "\\IIS";

                printDialog1.PrinterSettings.PrintToFile = true;
                printDocumentIIS.PrinterSettings.PrintFileName = directory + fileName;
                printDocumentIIS.PrinterSettings.PrintToFile = true;
                printDocumentIIS.Print();

                printDialog1.PrinterSettings.PrintToFile = false;
                printDocumentIIS.PrinterSettings.PrintToFile = false;
                printDialog1.PrinterSettings.PrinterName = printerName;
                printDocumentIIS.PrinterSettings.PrinterName = printerName;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message + Environment.NewLine + "PDF file not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
