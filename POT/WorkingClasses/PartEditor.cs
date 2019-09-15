using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    public partial class PartEditor : Form
    {
        Part workingPart = new Part();
        PartSifrarnik workingPartSifrarnik = new PartSifrarnik();
        QueryCommands qc = new QueryCommands();

        List<Part> partList = new List<Part>();
        List<PartSifrarnik> partListSifrarnik = new List<PartSifrarnik>();
        List<Company> cmp = new List<Company>();
        List<PovijestLog> plList = new List<PovijestLog>();

        List<String> whatList1 = new List<String>();

        Company cmpC = new Company();
        Company cmpO = new Company();
        PovijestLog pl = new PovijestLog();
        int selectedIndex;
        
        public PartEditor()
        {
            InitializeComponent();
        }

        private void PartEditor_Load(object sender, EventArgs e)
        {
            CntLbl.Text = "Cnt: 0";

            partList = qc.GetAllParts();
            partListSifrarnik = qc.GetPartsAllSifrarnikSortByFullCode();
            Company tempCmp = new Company();
            cmp = tempCmp.GetAllCompanyInfoSortCode();

            long i = 0;
            foreach (Part part in partList)
            {
                IDsearch.Items.Add(i + ". " + part.PartID);

                //if(!part.SN.Equals(""))
                    SNsearch.Items.Add(i + ". " + part.SN);

                //if (!part.CN.Equals(""))
                    CNsearch.Items.Add(i + ". " + part.CN);

                i++;
            }

            whatList1 = qc.GetTableColumnNames("PovijestLog");
            foreach (String name in whatList1)
            {
                if(!name.ToLower().Equals("logid"))
                    What1.Items.Add(name);
            }

            listView1.View = View.Details;

            listView1.Columns.Add("LogID");
            listView1.Columns.Add("DatumUpisa");
            listView1.Columns.Add("DatumRada");
            listView1.Columns.Add("Sifra novi");
            listView1.Columns.Add("Naziv novi");
            listView1.Columns.Add("Opis");
            listView1.Columns.Add("SN novi");
            listView1.Columns.Add("SN stari");
            listView1.Columns.Add("CN");
            listView1.Columns.Add("User ID");
            listView1.Columns.Add("Customer");
            listView1.Columns.Add("CCN");
            listView1.Columns.Add("CI");
            listView1.Columns.Add("Dokument");
            listView1.Columns.Add("Sifra stari");
            listView1.Columns.Add("Naziv stari");
            listView1.Columns.Add("GNG");

            Program.LoadStop();
        }

        private void IDsearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = IDsearch.SelectedIndex;
            workingPart = partList[selectedIndex];
            workingPartSifrarnik = partListSifrarnik.Find(x => x.FullCode == workingPart.PartialCode);
            cmpO = cmp.Find(x => x.Code == workingPart.CompanyO);
            cmpC = cmp.Find(x => x.Code == workingPart.CompanyC);
            popuniPolja(workingPart, workingPartSifrarnik);
        }

        private void SNsearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = SNsearch.SelectedIndex;
            workingPart = partList[selectedIndex];
            workingPartSifrarnik = partListSifrarnik.Find(x => x.FullCode == workingPart.PartialCode);
            cmpO = cmp.Find(x => x.Code == workingPart.CompanyO);
            cmpC = cmp.Find(x => x.Code == workingPart.CompanyC);
            popuniPolja(workingPart, workingPartSifrarnik);
        }

        private void CNsearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = CNsearch.SelectedIndex;
            workingPart = partList[selectedIndex];
            workingPartSifrarnik = partListSifrarnik.Find(x => x.FullCode == workingPart.PartialCode);
            cmpO = cmp.Find(x => x.Code == workingPart.CompanyO);
            cmpC = cmp.Find(x => x.Code == workingPart.CompanyC);
            popuniPolja(workingPart, workingPartSifrarnik);
        }

        private void popuniPolja(Part part, PartSifrarnik partSf)
        {
            CategoryNameTB.Text = workingPartSifrarnik.CategoryName;
            PartNameTB.Text = workingPartSifrarnik.PartName;
            SubPartNameTB.Text = workingPartSifrarnik.SubPartName;
            PartIDTB.Text = part.PartID.ToString();
            PartSNTB.Text = part.SN;
            PartCNTB.Text = part.CN;
            PartName.Text = partSf.FullName;
            DateIn.Text = part.DateIn;
            DateOut.Text = part.DateOut;
            DateSend.Text = part.DateSend;
            StorageID.Text = part.StorageID.ToString();
            State.Text = part.State;
            CompanyOwner.Text = part.CompanyO;
            CompanyOwnerName.Text = cmpO.Name;
            CompanyCustomer.Text = part.CompanyC;
            CompanyCustomerName.Text = cmpC.Name;
            PartCode.Text = string.Format( "{0: 000 000 000}", workingPartSifrarnik.FullCode);
            FullCode.Text = string.Format("{0: 00 00 000 000 000}", workingPart.CodePartFull);
            PartNumber.Text = workingPartSifrarnik.PartNumber;
            InKN.Text = workingPartSifrarnik.PriceInKn.ToString();
            OutKN.Text = workingPartSifrarnik.PriceOutKn.ToString();
            InEUR.Text = workingPartSifrarnik.PriceInEur.ToString();
            OutERU.Text = workingPartSifrarnik.PriceOutEur.ToString();
            Packaging.Text = workingPartSifrarnik.Packing;
        }

        private void PretraziBT_Click(object sender, EventArgs e)
        {
            Program.LoadStart();

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "PovijsetLog search";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                plList.Clear();

                listView1.Clear();

                CntLbl.Text = "Cnt: 0";

                listView1.View = View.Details;

                listView1.Columns.Add("LogID");
                listView1.Columns.Add("DatumUpisa");
                listView1.Columns.Add("DatumRada");
                listView1.Columns.Add("Sifra novi");
                listView1.Columns.Add("Naziv novi");
                listView1.Columns.Add("Opis");
                listView1.Columns.Add("SN novi");
                listView1.Columns.Add("SN stari");
                listView1.Columns.Add("CN");
                listView1.Columns.Add("User ID");
                listView1.Columns.Add("Customer");
                listView1.Columns.Add("CCN");
                listView1.Columns.Add("CI");
                listView1.Columns.Add("Dokument");
                listView1.Columns.Add("Sifra stari");
                listView1.Columns.Add("Naziv stari");
                listView1.Columns.Add("GNG");

                String wh1 = What1.Text;
                String wh2 = What2.Text;
                String wh3 = What3.Text;

                Object value1;
                Object value2;
                Object value3;

                if (dateTimePicker1.Visible)
                    value1 = dateTimePicker1.Value.ToString("dd.MM.yy.");
                else
                    value1 = selectValue(wh1, Value1.Text);

                if (dateTimePicker2.Visible)
                    value2 = dateTimePicker2.Value.ToString("dd.MM.yy.");
                else
                    value2 = selectValue(wh2, Value2.Text);

                if (dateTimePicker3.Visible)
                    value3 = dateTimePicker3.Value.ToString("dd.MM.yy.");
                else
                    value3 = selectValue(wh3, Value3.Text);

                String AndOr = "";
                if (AndRB.Checked)
                    AndOr = "And";
                else
                    AndOr = "Or";

                String reportV1 = "";
                String reportV2 = "";
                String reportV3 = "";

                if (value1 == null)
                    reportV1 = "null";
                else
                    reportV1 = value1.ToString();

                if (value2 == null)
                    reportV2 = "null";
                else
                    reportV2 = value2.ToString();

                if (value3 == null)
                    reportV3 = "null";
                else
                    reportV3 = value3.ToString();

                data = wh1.ToString() + "; " + wh2.ToString() + "; " + wh3.ToString() + "; " + reportV1 + "; " + reportV2 + "; " + reportV3 + "; " + AndOr;

                if (wh1.ToString().Equals("") && wh2.ToString().Equals("") && wh3.ToString().Equals("") && value1.ToString().Equals("") && value2.ToString().Equals("") && value3.ToString().Equals(""))
                {
                    wh1 = "";
                    wh2 = "";
                    wh3 = "";

                    value1 = selectValue(wh1, workingPart.CodePartFull.ToString());
                    value2 = selectValue(wh2, workingPart.SN.ToString());
                    value3 = "";

                    plList = pl.PretraziPL(wh1, wh2, wh3, value1, value2, value3, AndOr, false);
                }
                else
                {
                    plList = pl.PretraziPL(wh1, wh2, wh3, value1, value2, value3, AndOr, true);
                }

                foreach (PovijestLog pov in plList)
                {
                    ListViewItem lvi1 = new ListViewItem(pov.LogID.ToString());

                    lvi1.SubItems.Add(pov.DatumUpisa);
                    lvi1.SubItems.Add(pov.DatumRada);
                    lvi1.SubItems.Add(pov.SifraNovi.ToString());
                    lvi1.SubItems.Add(pov.NazivNovi);
                    lvi1.SubItems.Add(pov.Opis);
                    lvi1.SubItems.Add(pov.SNNovi);
                    lvi1.SubItems.Add(pov.SNStari);
                    lvi1.SubItems.Add(pov.CN);
                    lvi1.SubItems.Add(pov.UserID.ToString());
                    lvi1.SubItems.Add(pov.CustomerName);
                    lvi1.SubItems.Add(pov.CCN);
                    lvi1.SubItems.Add(pov.CI);
                    lvi1.SubItems.Add(pov.Dokument);
                    lvi1.SubItems.Add(pov.SifraStari.ToString());
                    lvi1.SubItems.Add(pov.NazivStari);
                    lvi1.SubItems.Add(pov.GNg);

                    listView1.Items.Add(lvi1);
                }

                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                CntLbl.Text = "Cnt: " + plList.Count().ToString();

                data = wh1.ToString() + "; " + wh2.ToString() + "; " + wh3.ToString() + "; " + value1.ToString() + "; " + value2.ToString() + "; " + value3.ToString();

                Result = "Query data";

                lw.LogMe(function, usedQC, data, Result);
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                this.Focus();

                lw.LogMe(function, usedQC, data, Result);

                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
            finally
            {
                Program.LoadStop();
                this.Focus();
            }

        }

        private Object selectValue(String what, String value)
        {
            Object retValue;

            //if (value.Equals(""))
            //    return null;

            switch (what.ToLower())
            {
                case "logid":
                    retValue = long.Parse(value);
                    break;
                case "sifranovi":
                    retValue = long.Parse(value);
                    break;
                case "sifrastari":
                    retValue = long.Parse(value);
                    break;
                case "userid":
                    retValue = long.Parse(value);
                    break;
                default:
                    retValue = value.ToString();
                    break;
            }

            return retValue;
        }

        private void What1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AndRB.Checked = true;

            dateTimePicker2.Visible = false;
            Value2.Visible = true;

            dateTimePicker3.Visible = false;
            Value3.Visible = true;

            What2.ResetText();
            What3.ResetText();

            Value2.ResetText();
            Value3.ResetText();

            What2.Items.Clear();
            What3.Items.Clear();
            
            if (!What1.Text.Equals(""))
            {
                foreach (String name in whatList1)
                {
                    if (!name.ToLower().Equals("logid") && !name.ToLower().Equals(What1.Text.ToLower()))
                        What2.Items.Add(name);
                }
            }

            if (What1.Text.ToLower().Equals("datumrada") || What1.Text.ToLower().Equals("datumupisa"))
            {
                dateTimePicker1.Visible = true;
                Value1.Visible = false;
            }
            else
            {
                dateTimePicker1.Visible = false;
                Value1.Visible = true;
            }
        }

        private void What2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dateTimePicker3.Visible = false;
            Value3.Visible = true;

            What3.ResetText();

            Value3.ResetText();

            What3.Items.Clear();

            if (!What1.Text.Equals(""))
            {
                foreach (String name in whatList1)
                {
                    if (!name.ToLower().Equals("logid") && !name.ToLower().Equals(What1.Text.ToLower()) && !name.ToLower().Equals(What2.Text.ToLower()))
                        What3.Items.Add(name);
                }
            }

            if (What2.Text.ToLower().Equals("datumrada") || What2.Text.ToLower().Equals("datumupisa"))
            {
                dateTimePicker2.Visible = true;
                Value2.Visible = false;
            }
            else
            {
                dateTimePicker2.Visible = false;
                Value2.Visible = true;
            }
        }

        private void What3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (What3.Text.ToLower().Equals("datumrada") || What3.Text.ToLower().Equals("datumupisa"))
            {
                dateTimePicker3.Visible = true;
                Value3.Visible = false;
            }
            else
            {
                dateTimePicker3.Visible = false;
                Value3.Visible = true;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            var itemIndx = listView1.SelectedIndices[0];

            String document;
            String id;

            try
            {
                var arr = listView1.Items[itemIndx].SubItems[13].Text.Split(' ');

                document = arr[0];
                id = arr[1];

            }
            catch
            {
                MessageBox.Show("I cant find requested document!");
                return;
            }

            //String filePath = Properties.Settings.Default.DefaultFolder + "\\RAC\\" + document + " " + id + ".pdf";
            String filePath = "";

            try
            {
                var allFiles = Directory.GetFiles(Properties.Settings.Default.DefaultFolder, "*.pdf", SearchOption.AllDirectories);

                foreach(String dir in allFiles)
                {
                    if(dir.Contains(document + " " + id + ".pdf"))
                    {
                        filePath = dir;
                        break;
                    }
                }
                if(!filePath.Equals(""))
                    Process.Start(filePath);
                else
                    MessageBox.Show("I cant find requested document!");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
