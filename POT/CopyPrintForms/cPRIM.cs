using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.CopyPrintForms
{
    public partial class cPRIM : Form
    {
        List<long> primID = new List<long>();
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

        String PRIMNumber;
        List<Part> partListPrint = new List<Part>();
        String napomenaPRIMPrint;
        Branch br = new Branch();

        QueryCommands qc = new QueryCommands();

        public cPRIM()
        {
            InitializeComponent();
            new Thread(fillSifrarnik).Start();
        }

        private void cPRIM_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("primID");
            listView1.Columns.Add("customerID");
            listView1.Columns.Add("dateCreated");
            listView1.Columns.Add("napomena");
            listView1.Columns.Add("userID");

            listView2.View = View.Details;

            listView2.Columns.Add("RB");
            listView2.Columns.Add("Name");
            listView2.Columns.Add("CodePartFull");
            listView2.Columns.Add("SN");
            listView2.Columns.Add("CN");
            
            //Thread myThread = new Thread(fillSifrarnik);
            //myThread.Start();

            primID = qc.GetAllPRIMID();
            if (primID[0] != -1)
            {
                for (int i = 0; i < primID.Count(); i++)
                {
                    this.comboBox1.Items.Add(primID[i]);
                }
            }
            
            customerID = qc.GetAllPRIMcustomerID();
            if (customerID[0] != -1)
            {
                for (int i = 0; i < customerID.Count(); i++)
                {
                    this.comboBox2.Items.Add(customerID[i]);
                }

            }

            dateCreated = qc.GetAllPRIMdateCreated();
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            getPRIMList(comboBox1.Text, "ID");

            comboBox2.ResetText();
            comboBox3.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            getPRIMList(comboBox2.Text, "CMP");

            comboBox1.ResetText();
            comboBox3.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            getPRIMList(comboBox3.Text, "DATE");

            comboBox1.ResetText();
            comboBox2.ResetText();
            comboBox4.ResetText();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<String> arr = new List<string>();
            arr = qc.CompanyInfoByName(comboBox4.Text);
            getPRIMList(arr[0], "NAME");

            comboBox1.ResetText();
            comboBox2.ResetText();
            comboBox3.ResetText();
        }

        private void getPRIMList(String value, String tipe)
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
                    arr = qc1.GetAllInfoPRIMBy("primID", value);
                    break;
                case ("CMP"):
                    arr = qc1.GetAllInfoPRIMBy("customerID", value);
                    break;
                case ("DATE"):
                    arr = qc1.GetAllInfoPRIMBy("dateCreated", value);
                    break;
                case ("NAME"):
                    arr = qc1.GetAllInfoPRIMBy("customerID", value);
                    break;

            }

            userArr = arr;

            try
            {
                rb = listView1.Items.Count + 1;

                if (arr[0].Equals("nok"))
                {
                    data = value;
                    Result = "Selected PRIM does not exist.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }

                for (int i = 0; i < arr.Count; i = i + 5)
                {
                    ListViewItem lvi1 = new ListViewItem(rb.ToString());

                    lvi1.SubItems.Add(arr[i]);
                    lvi1.SubItems.Add(arr[i + 1]);
                    lvi1.SubItems.Add(arr[i + 2]);
                    lvi1.SubItems.Add(arr[i + 3]);
                    lvi1.SubItems.Add(arr[i + 4]);

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

            for (int i = 0; i < 5; i++)
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

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "PRIM Selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            this.label7.Text = "Working...";
            this.Refresh();

            br.Clear();

            if (sifrarnikArr.Count > 0)
                this.label6.Text = "SifrarnikArr: OK";
            else
            {
                Thread myThread = new Thread(fillSifrarnik);
                myThread.Start();

                this.label7.Text = "";

                Result = "SifrarnikArr: NOK";
                lw.LogMe(function, usedQC, data, Result);
                this.label6.Text = "SifrarnikArr: NOK";
                this.label7.ResetText();
                return;
            }

            listView2.Items.Clear();
            listView2.Refresh();
            this.label5.Text = "";

            List<long> prtList = new List<long>();
            List<Part> parts = new List<Part>();
            long primID;
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

            primID = long.Parse(item[0].SubItems[1].Text);
            prtList = qc.GetAllpartIDByPrimID(primID);
            this.label5.Text = "Selected PRIM: " + primID.ToString();

            datumIzradeM = userArr[(5 * index) + 2];
            izradioUserM = userArr[(5 * index) + 4];
            QueryCommands qc4 = new QueryCommands();
            izradioRegijaM = qc4.User(WorkingUser.Username, WorkingUser.Password, izradioUserM)[7];

            if (prtList[0] == -1)
            {
                data = primID.ToString();
                Result = "Can not find parts by given primID.";
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
                    data = primID.ToString();
                    Result = "Selected PRIM (" + primID.ToString() + ") does not exist or do not have parts.";
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

            PRIMNumber = primID.ToString();
            partListPrint = parts;
            napomenaPRIMPrint = item[0].SubItems[4].Text;

            ////////////////// TODO ODKOMENTIRAJ AKO BUDES NA PRIMCI IMAO ISPIS FILIJALE///////////////////
            //if (!item[0].SubItems[6].Text.Equals(""))
            //    br.GetFilByID(long.Parse(item[0].SubItems[7].Text));
            br.FilID = 0;

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

            printPreviewDialogPrim.Document = printDocumentPrim;
            printPreviewDialogPrim.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialogPrim.ShowDialog();
        }

        private void selectPrinterPrintBtn_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = printDocumentPrim;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                printPrewBT_Click(sender, e);
                //printDocumentPrim.Print();
            }
        }

        private void printDocumentPrim_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
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
                PrintMe pr = new PrintMe(cmpS, cmpR, sifrarnikArr, partListPrint, PRIMNumber, napomenaPRIMPrint, Properties.strings.RECEIPT, Properties.strings.customer, false, br);
                pr.datumIzrade = datumIzradeM;
                pr.izradioUser = izradioUserM;
                pr.izradioRegija = izradioRegijaM;
                pr.Print(e);
                data = cmpR + ", " + cmpS + ", " + sifrarnikArr + ", " + partListPrint + ", " + PRIMNumber + ", " + napomenaPRIMPrint + ", " + Properties.strings.DELIVERY + ", " + Properties.strings.customer + ", true, " + br;
            }
            else
            {
                PrintMe pr = new PrintMe(cmpS, cmpR, sifrarnikArr, partListPrint, PRIMNumber, napomenaPRIMPrint, Properties.strings.RECEIPT, Properties.strings.customer, false);
                pr.datumIzrade = datumIzradeM;
                pr.izradioUser = izradioUserM;
                pr.izradioRegija = izradioRegijaM;
                pr.Print(e);
                data = cmpR + ", " + cmpS + ", " + sifrarnikArr + ", " + partListPrint + ", " + PRIMNumber + ", " + napomenaPRIMPrint + ", " + Properties.strings.DELIVERY + ", " + Properties.strings.customer + ", false";
            }

            Result = "Print page called";
            lw.LogMe(function, usedQC, data, Result);
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
                        MessageBox.Show("Cant load 'sifrarnik'.");
                        String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        String usedQC = "Loading sifrarnik";
                        String data = "Break limit reached, arr cnt = " + tresultArr.Count;
                        String Result = "";
                        LogWriter lw = new LogWriter();

                        ChangeColor("Red");

                        Result = "Cant load 'sifrarnik'.";
                        lw.LogMe(function, usedQC, data, Result);

                        break;
                    }
                }
                if (stop < 100)
                    ChangeColor("Green");
                else
                    ChangeColor("Red");
                sifrarnikArr = tresultArr;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }

        public void ChangeColor(string color)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(ChangeColor), new object[] { color });
                return;
            }

            if (color.Equals("Green"))
                this.button4.BackColor = Color.Green;
            else
                this.button4.BackColor = Color.Red;
        }

        private void fillCmp(long code)
        {
            try
            {
                MainCmp mpc = new MainCmp();
                mpc.GetMainCmpByName(Properties.Settings.Default.CmpName);
                cmpR.Clear();
                cmpR = mpc.MainCmpToCompany();

                cmpS.Clear();
                cmpS.GetCompanyInfoByID(code);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }
    }
}
