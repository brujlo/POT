using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using POT.WorkingClasses;
using System.Media;

namespace POT
{
    public partial class Primka : Form
    {
        int rb = 1;
        List<String> partsArr = new List<string>();
        List<String> resultArr = new List<string>();
        List<String> resultArrSearchCode = new List<string>();
        List<String> openedOTP = new List<string>();
        List<String> openedTransactionSenderRegions = new List<string>();
        static List<String> sifrarnikArr = new List<string>();
        Boolean isPrimkaSaved = false;
        Company cmpS = new Company();
        Company cmpR = new Company();
        String PrimkaNumber;
        List<Part> partListPrint = new List<Part>();
        String napomenaPRIMPrint;

        List<Company> resultArrC = new List<Company>();

        public Primka()
        {
            InitializeComponent();
        }

        private void Primka_Load(object sender, EventArgs e)
        {
            comboBox3.Text = Properties.Settings.Default.MainCompanyCode;
            comboBox4.Text = Properties.Settings.Default.MainCompanyCode;
            this.printPrewBT.Enabled = false;

            Thread myThread = new Thread(fillComboBoxes);

            myThread.Start();

            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("Name");
            listView1.Columns.Add("Code");
            listView1.Columns.Add("SN");
            listView1.Columns.Add("CN");
            listView1.Columns.Add("Condition");

            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();

            try
            {
                Company cmpList = new Company();
                //List<Company> resultArrC = new List<Company>();

                resultArrC = cmpList.GetAllCompanyInfoSortByName();

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.comboBox1.Items.Add(resultArrC[i].Name);
                    }

                }
            }
            catch (Exception)
            {

            }

            resultArr.Clear();
            try
            {
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
            }
            catch (Exception)
            {
                
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label10.Text = "Letters left " + (200 - textBox4.TextLength).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvi1 = new ListViewItem();
                rb = listView1.Items.Count + 1;
                lvi1.Text = rb.ToString();

                if ((sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString()))) < 0)
                {
                    MessageBox.Show("Selected code does not exist in DB.");
                    textBox1.SelectAll();
                    return;
                }
                lvi1.SubItems.Add(sifrarnikArr[sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString())) - 1]);
                lvi1.SubItems.Add(textBox1.Text);
                lvi1.SubItems.Add(textBox2.Text);
                lvi1.SubItems.Add(textBox3.Text);
                lvi1.SubItems.Add(radioButton1.Checked ? "g" : "ng");

                if(listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);
                
                listView1.Items.Add(lvi1);
                partsArr.Add(textBox1.Text);
                partsArr.Add(textBox2.Text);
                partsArr.Add(textBox3.Text);
                partsArr.Add(radioButton1.Checked ? "g" : "ng");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            for (int i = 0; i < 6; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();

            textBox1.SelectAll();
            textBox1.Focus();

            SystemSounds.Hand.Play();
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
                textBox1.Focus();
                
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Focus();

            label2.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).Name.Trim();
            label3.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).Address.Trim();
            label4.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).OIB.Trim();
            label5.Text = resultArrC.ElementAt(comboBox1.SelectedIndex).Contact.Trim();

            textBox1.Focus();
        }

        static void fillComboBoxes()
        {
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            List<String> tsendArr = new List<string>();
            List<String> tresultArr = new List<string>();

            try
            {
                tresultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);
                tsendArr.Clear();

                if (!tresultArr[0].Equals("nok"))
                {
                    sifrarnikArr = tresultArr;
                }
            }
            catch (Exception)
            {
                sifrarnikArr = tresultArr;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = comboBox4.Text + comboBox3.Text + string.Format("{0:000000000}", int.Parse(resultArrSearchCode.ElementAt(comboBox2.SelectedIndex)));
                textBox1.SelectAll();
                textBox1.Focus();
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var items = listView1.SelectedItems;
            int k = 1;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
            }

            foreach (ListViewItem item in listView1.Items)
            {
                if (listView1.SelectedItems != null && listView1.Items.Count != 0)
                {
                    item.SubItems[0].Text = k.ToString();
                    k++;
                }
            }
            rb = listView1.Items.Count + 1;
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PrimkaNumber = "";

            if (this.label2.Text.Equals("Name"))
            {
                MessageBox.Show("Please select company, nothing done.");
                textBox1.SelectAll();
                textBox1.Focus();
            }
            else if (this.listView1.Items.Count == 0)
            {
                MessageBox.Show("There is no items in list, nothing done.");
                textBox1.SelectAll();
                textBox1.Focus();
            }
            else
            {
                List<Part> listOfOtpPartsFromOTP = new List<Part>();
                //List<Part> listOfOtpPartsPrimka = new List<Part>();

                if (cmpS.GetCompanyByName(label2.Text.Trim()) && cmpR.GetCompanyInfoByRegionID(WorkingUser.RegionID.ToString()))
                {
                    try
                    {
                        List<String> arr = new List<string>();
                        SqlCommand commandPR;
                        String queryPR;
                        ConnectionHelper cn = new ConnectionHelper();
                        using (SqlConnection cnnPR = cn.Connect(WorkingUser.Username, WorkingUser.Password))
                        {
                            if (cn.TestConnection(cnnPR))
                            {
                                //Provjera da li se dijelovi nalaze u mom skladistu
                                QueryCommands qc = new QueryCommands();

                                for (int i = 0; i < listView1.Items.Count; i++) // vec imam provjeru gore kod unosa ali neka ostane(tamo je po imenu)
                                {
                                    if (!qc.GetPartIDCompareCodeSNCNStorage(WorkingUser.Username, WorkingUser.Password,
                                        long.Parse(listView1.Items[i].SubItems[2].Text),
                                        listView1.Items[i].SubItems[3].Text,
                                        listView1.Items[i].SubItems[4].Text,
                                        WorkingUser.RegionID)[0].Equals("nok"))
                                    {
                                        MessageBox.Show("In your storage, part with: \n\n Code: " + listView1.Items[i].SubItems[2].Text + "\n on position " + (listView1.Items[i].Index + 1) + "\n, already exist in DB.\n\nNothing Done.");
                                        textBox1.SelectAll();
                                        textBox1.Focus();
                                        return;
                                    }
                                }

                                //Provjera i spremanje u bazu

                                List<String> allRegions = new List<string>();
                                allRegions = qc.GetAllRegions(WorkingUser.Username, WorkingUser.Password);

                                int index = resultArrC.FindIndex(resultArrC => resultArrC.Name.Equals(label2.Text));

                                if (resultArrC[index].RegionID != Properties.Settings.Default.OstaliIDRegion &&
                                    resultArrC[index].RegionID != Properties.Settings.Default.TransportIDRegion)
                                {
                                    queryPR = "Select RegionIDOut from Transport where RegionIDIn = " + WorkingUser.RegionID + " and UsersUserIDIn is NULL";
                                    commandPR = new SqlCommand(queryPR, cnnPR);
                                    commandPR.ExecuteNonQuery();
                                    SqlDataReader dataReader = commandPR.ExecuteReader();
                                    dataReader.Read();

                                    if (dataReader.HasRows)
                                    {
                                        do
                                        {
                                            openedTransactionSenderRegions.Add(dataReader["RegionIDOut"].ToString());
                                        } while (dataReader.Read());
                                        dataReader.Close();
                                    }
                                    else
                                    {
                                        MessageBox.Show("From selected region nothing is sent to you. \n\n Nothing Done.");
                                        textBox1.SelectAll();
                                        textBox1.Focus();
                                        return;
                                    }

                                    for (int i = 0; i < listView1.Items.Count; i++) // vec imam provjeru gore kod unosa ali neka ostane(tamo je po imenu)
                                    {
                                        long test = long.Parse(listView1.Items[i].SubItems[2].Text.Substring(4));
                                        if (!resultArrSearchCode.Contains(test.ToString()))
                                        {
                                            MessageBox.Show("There is no part in 'Sifrarnik' with code, = " + test.ToString() + "\n" + "on position " + (listView1.Items[i].Index + 1) + "  \n\n Nothing Done.");
                                            textBox1.SelectAll();
                                            textBox1.Focus();
                                            return;
                                        }
                                    }

                                    //Provjera da li sam odabrao dobru tvrtku koja salje
                                    Boolean exist = false;
                                    for (int j = 0; j < openedTransactionSenderRegions.Count; j++)
                                    {
                                        if (cmpS.RegionID.ToString().Equals(openedTransactionSenderRegions[j]))
                                        {
                                            exist = true;
                                            break;
                                        }
                                    }

                                    if (!exist)
                                    {
                                        MessageBox.Show("Please select right company! \n\n Nothing done.");
                                        textBox1.SelectAll();
                                        textBox1.Focus();
                                        return;
                                    }

                                    openedOTP = qc.GetAllOpenedOTP(WorkingUser.Username, WorkingUser.Password, WorkingUser.RegionID);
                                    long selectedOTP = 0;

                                    if (openedOTP[0].Equals("nok"))
                                    {
                                        MessageBox.Show("There is no opened documents for you!");
                                        textBox1.SelectAll();
                                        textBox1.Focus();
                                        return;
                                    }

                                    using (Selector selector = new Selector())
                                    {
                                        selector.SetLabelText = "Please select receiving document";
                                        selector.SetComboBoxStringList = openedOTP;
                                        selector.ShowDialog();

                                        if (selector.GetOTPValue > 0)
                                        {
                                            selectedOTP = selector.GetOTPValue;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Please select valid receiving document! \n\n Nothing done.");
                                            textBox1.SelectAll();
                                            textBox1.Focus();
                                            return;
                                        }
                                    }

                                    Part otpParts = new Part();
                                    listOfOtpPartsFromOTP = otpParts.GetListOfPartsOTPParts(selectedOTP);

                                    int counterLP = listOfOtpPartsFromOTP.Count;
                                    int counterLV = listView1.Items.Count;
                                    Boolean same = false;
                                    for (int i = 0; i < counterLP; i++)
                                    {
                                        for (int ii = 0; ii < counterLV; ii++)
                                        {
                                            if (listOfOtpPartsFromOTP[i].CodePartFull == long.Parse(listView1.Items[ii].SubItems[2].Text)
                                                && listOfOtpPartsFromOTP[i].SN.Equals(listView1.Items[ii].SubItems[3].Text)
                                                && listOfOtpPartsFromOTP[i].CN.Equals(listView1.Items[ii].SubItems[4].Text))
                                            {
                                                same = true;
                                                break;
                                            }
                                            else
                                            {
                                                same = false;
                                            }
                                        }
                                        if (!same)
                                            break;
                                    }

                                    if (!same || counterLP != counterLV)
                                    {
                                        MessageBox.Show("Receiving document and sending document items do not match ! \n\n Nothing done.");
                                        textBox1.SelectAll();
                                        textBox1.Focus();
                                        return;
                                    }

                                    PrimkaNumber = qc.PRIMUnesiUredajeDaSuPrimljeniInnner(WorkingUser.Username, WorkingUser.Password, listOfOtpPartsFromOTP, cmpS.RegionID, cmpR.RegionID, selectedOTP, textBox4.Text);
                                    if (!PrimkaNumber.Equals("nok"))
                                    {
                                        PovijestLog pl = new PovijestLog();
                                        Boolean saved = false;
                                        for (int k = 0; k < listOfOtpPartsFromOTP.Count; k++)
                                        {
                                            List<Part> tempPart = new List<Part>();
                                            tempPart.Clear();
                                            tempPart.Add(listOfOtpPartsFromOTP[k]);
                                            if (pl.SaveToPovijestLog(tempPart, DateTime.Now.ToString("dd.MM.yy."), textBox4.Text, cmpS.Name, "", "", "PRIM " + Properties.Settings.Default.ShareDocumentName, tempPart[0].State))
                                            {
                                                saved = true;
                                                Properties.Settings.Default.ShareDocumentName = "";
                                            }
                                            else
                                            {
                                                saved = false;
                                                break;
                                            }
                                        }

                                        if (saved)
                                        {
                                            MessageBox.Show("DONE, document nbr. 'PRIM " + PrimkaNumber + "'.");
                                            isPrimkaSaved = true;
                                            listView1.Clear();
                                            listView1.View = View.Details;

                                            partListPrint.Clear();
                                            partListPrint = listOfOtpPartsFromOTP;
                                            //partListPrint = listOfOtpPartsPrimka;

                                            listView1.Columns.Add("RB");
                                            listView1.Columns.Add("Name");
                                            listView1.Columns.Add("Code");
                                            listView1.Columns.Add("SN");
                                            listView1.Columns.Add("CN");
                                            listView1.Columns.Add("Condition");
                                            textBox4.Clear();
                                        }
                                        else
                                        {
                                            MessageBox.Show("DONE, document nbr. 'PRIM " + PrimkaNumber + "', but not saved in PL.");
                                            listView1.Clear();
                                            listView1.View = View.Details;

                                            partListPrint.Clear();
                                            partListPrint = listOfOtpPartsFromOTP;
                                            //partListPrint = listOfOtpPartsPrimka;

                                            listView1.Columns.Add("RB");
                                            listView1.Columns.Add("Name");
                                            listView1.Columns.Add("Code");
                                            listView1.Columns.Add("SN");
                                            listView1.Columns.Add("CN");
                                            listView1.Columns.Add("Condition");
                                            textBox4.Clear();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Unknown error in QUERYinner.");
                                        isPrimkaSaved = false;
                                    }
                                }
                                else if (resultArrC[index].RegionID == Properties.Settings.Default.OstaliIDRegion)
                                {
                                    List<Part> partList = new List<Part>();
                                    String napomenaPRIM = textBox4.Text;

                                    for (int i = 0; i < listView1.Items.Count; i++)
                                    {
                                        PartSifrarnik tempSifPart = new PartSifrarnik();
                                        Part tempPart = new Part();

                                        tempSifPart.GetPart(listView1.Items[i].SubItems[2].Text.Substring(4));

                                        tempPart.PartialCode = tempSifPart.FullCode;
                                        tempPart.SN = listView1.Items[i].SubItems[3].Text;
                                        tempPart.CN = listView1.Items[i].SubItems[4].Text;
                                        tempPart.DateIn = DateTime.Now.ToString("dd.MM.yy.");
                                        tempPart.StorageID = WorkingUser.RegionID;
                                        tempPart.State = listView1.Items[i].SubItems[5].Text;
                                        tempPart.CompanyO = listView1.Items[i].SubItems[2].Text.Substring(0, 2);
                                        tempPart.CompanyC = listView1.Items[i].SubItems[2].Text.Substring(2, 2);

                                        partList.Add(tempPart);
                                    }

                                    PrimkaNumber = qc.PRIMUnesiUredajeDaSuPrimljeni(WorkingUser.Username, WorkingUser.Password, partList, WorkingUser.RegionID, cmpS.ID, napomenaPRIM);
                                    if (!PrimkaNumber.Equals("nok"))
                                    {
                                        PovijestLog pl = new PovijestLog();
                                        Boolean saved = false;
                                        for (int k = 0; k < partList.Count; k++)
                                        {
                                            List<Part> tempPart = new List<Part>();
                                            tempPart.Clear();
                                            tempPart.Add(partList[k]);
                                            if (pl.SaveToPovijestLog(tempPart, DateTime.Now.ToString("dd.MM.yy."), napomenaPRIM, cmpS.Name, "", "", "PRIM " + Properties.Settings.Default.ShareDocumentName, tempPart[0].State))
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

                                        if (saved)
                                        {
                                            MessageBox.Show("DONE, document nbr. PRIM '" + PrimkaNumber + "'.");

                                            partListPrint.Clear();
                                            partListPrint = partList;

                                            isPrimkaSaved = true;
                                            listView1.Clear();
                                            listView1.View = View.Details;

                                            listView1.Columns.Add("RB");
                                            listView1.Columns.Add("Name");
                                            listView1.Columns.Add("Code");
                                            listView1.Columns.Add("SN");
                                            listView1.Columns.Add("CN");
                                            listView1.Columns.Add("Condition");
                                            textBox4.Clear();
                                            napomenaPRIMPrint = napomenaPRIM;
                                        }
                                        else
                                        {
                                            MessageBox.Show("DONE, document nbr. 'PRIM " + PrimkaNumber + "', but not saved in PL.");

                                            partListPrint.Clear();
                                            partListPrint = partList;

                                            isPrimkaSaved = true;
                                            listView1.Clear();
                                            listView1.View = View.Details;

                                            listView1.Columns.Add("RB");
                                            listView1.Columns.Add("Name");
                                            listView1.Columns.Add("Code");
                                            listView1.Columns.Add("SN");
                                            listView1.Columns.Add("CN");
                                            listView1.Columns.Add("Condition");
                                            textBox4.Clear();
                                            napomenaPRIMPrint = napomenaPRIM;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Unknown error in QUERY.");
                                        napomenaPRIMPrint = "";
                                        isPrimkaSaved = false;
                                    }
                                }
                            }
                            cnnPR.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        textBox1.SelectAll();
                        textBox1.Focus();
                    }
                }
                this.printPrewBT.Enabled = isPrimkaSaved;
            }
        }

        private void printPrewBT_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            printPreviewDialogPrim.Document = printDocumentPrim;
            printPreviewDialogPrim.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialogPrim.ShowDialog();

            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void printDocumentPrim_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintMe pr = new PrintMe(cmpS, cmpR, sifrarnikArr, partListPrint, PrimkaNumber, napomenaPRIMPrint, "Receipt", "customer", false);
            //PrintMe pr = new PrintMe(cmpS, cmpR, sifrarnikArr, partListPrint, PrimkaNumber);
            pr.Print(e);
        }
    }
}