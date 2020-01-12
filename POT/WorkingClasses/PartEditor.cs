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

        Dictionary<long, String> categorySifrarnik = new Dictionary<long, String>();
        Dictionary<long, String> partSifrarnik = new Dictionary<long, String>();
        Dictionary<long, String> subpartSifrarnik = new Dictionary<long, String>();
        long[] arr = new long[1000];
        PartSifrarnik prtSif = new PartSifrarnik();

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
            if (WorkingUser.AdminRights == 1 || WorkingUser.AdminRights == 2)
                tabPage2.Enabled = true;
            else
                tabPage2.Enabled = false;

            CntLbl.Text = "Cnt: 0";

            partList = qc.GetAllParts();
            partListSifrarnik = qc.GetPartsAllSifrarnikSortByFullCode();
            categorySifrarnik = qc.GetCategoryNamesAllSifrarnikSortByName();

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

            long bigest = 0;
            foreach (KeyValuePair<long, String> key in categorySifrarnik)
            {
                sCategory.Items.Add(key.Value);

                if (bigest < (key.Key / 1000000))
                    if ((key.Key / 1000000) != 999 && (key.Key / 1000000) != 998)
                        bigest = key.Key / 1000000;
            }
            lastIDCategory.Text = bigest.ToString();

            for (i = 0; i < 1000; i++)
            {
                if( i > bigest)
                    sCategoryCodeNew.Items.Add(String.Format("{0:000}", i));
                sPartNameCodeNew.Items.Add(String.Format("{0:000}", i));
                sSubPartNameCodeNew.Items.Add(String.Format("{0:000}", i));
                arr[i] = i;
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

        private void IDsearch_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            selectedIndex = IDsearch.SelectedIndex;
            workingPart = partList[selectedIndex];
            workingPartSifrarnik = partListSifrarnik.Find(x => x.FullCode == workingPart.PartialCode);
            cmpO = cmp.Find(x => x.Code == workingPart.CompanyO);
            cmpC = cmp.Find(x => x.Code == workingPart.CompanyC);
            popuniPolja(workingPart, workingPartSifrarnik);
        }

        private void SNsearch_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            selectedIndex = SNsearch.SelectedIndex;
            workingPart = partList[selectedIndex];
            workingPartSifrarnik = partListSifrarnik.Find(x => x.FullCode == workingPart.PartialCode);
            cmpO = cmp.Find(x => x.Code == workingPart.CompanyO);
            cmpC = cmp.Find(x => x.Code == workingPart.CompanyC);
            popuniPolja(workingPart, workingPartSifrarnik);
        }

        private void CNsearch_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            selectedIndex = CNsearch.SelectedIndex;
            workingPart = partList[selectedIndex];
            workingPartSifrarnik = partListSifrarnik.Find(x => x.FullCode == workingPart.PartialCode);
            cmpO = cmp.Find(x => x.Code == workingPart.CompanyO);
            cmpC = cmp.Find(x => x.Code == workingPart.CompanyC);
            popuniPolja(workingPart, workingPartSifrarnik);
        }

        private void What1_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void What2_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void What3_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void PretraziBT_Click_1(object sender, EventArgs e)
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

                foreach (String dir in allFiles)
                {
                    if (dir.Contains(document + " " + id + ".pdf"))
                    {
                        filePath = dir;
                        break;
                    }
                }
                if (!filePath.Equals(""))
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

        
        ///////////////////////////////  Pocinje Drugi tab //////////////////////////////////////////////////////////////////

        private void sCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Program.LoadStart();

                prtSif = null;

                sCategoryCode.ResetText();
                lastIDPart.ResetText();

                sCategoryCodeNew.Enabled = true;
                sCategoryNew.Enabled = true;

                //button1_Click(sender, e);
                button1.PerformClick();

                foreach (KeyValuePair<long, String> item in categorySifrarnik)
                {
                    if (item.Value.Equals(sCategory.SelectedItem.ToString()))
                    {
                        sCategoryCode.Text = item.Key.ToString();
                        prtSif = partListSifrarnik.Find(x => x.CategoryCode == item.Key && x.PartCode == 0 && x.SubPartCode == 0);

                        sCategoryCodeNew.Items.Clear();
                        sCategoryCodeNew.ResetText();
                        sCategoryCodeNew.Items.Add(String.Format( "{0:000}", item.Key / 1000000) );
                        sCategoryCodeNew.SelectedIndex = 0;
                        sCategoryCodeNew.Enabled = false;

                        sCategoryNew.Items.Clear();
                        sCategoryNew.ResetText();
                        sCategoryNew.Items.Add(item.Value);
                        sCategoryNew.SelectedIndex = 0;
                        sCategoryNew.Enabled = false;

                        break;
                    }
                }


                partSifrarnik.Clear();
                partSifrarnik = qc.GetPartNamesAllSifrarnikSortByName(sCategory.SelectedItem.ToString());

                sPartName.ResetText();
                sPartName.Items.Clear();
                
                long bigest = 0;
                foreach (KeyValuePair<long, String> key in partSifrarnik)
                {
                    sPartName.Items.Add(key.Value);
                    if (bigest < (key.Key / 1000))
                        bigest = key.Key / 1000;
                }
                lastIDPart.Text = bigest.ToString();

               
                sPartNameCodeNew.Items.Clear();

                lastIDPart.Text = bigest.ToString();
                for (long i = bigest + 1; i < 1000; i++)
                {
                    sPartNameCodeNew.Items.Add(i);
                }

                PackingCB.Items.Add("kom");
                PackingCB.Items.Add("pak");
                PackingCB.Items.Add("sat");
                PackingCB.Items.Add("dan");
                PackingCB.Items.Add("mje");
                PackingCB.Items.Add("god");
                PackingCB.Items.Add("kut");
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                //MessageBox.Show(e1.Message);
            }
            finally
            {
                PostaviFullCode();
                Program.LoadStop();
            }
        }

        private void sPartName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Program.LoadStart();

                sPartNameCode.ResetText();
                lastIDSubPart.ResetText();

                sPartNameCodeNew.Enabled = true;
                sPartNameNew.Enabled = true;

                foreach (KeyValuePair<long, String> item in partSifrarnik)
                {
                    if (item.Value.Equals(sPartName.SelectedItem.ToString()))
                    {
                        sPartNameCode.Text = item.Key.ToString();
                        prtSif = partListSifrarnik.Find(x => x.CategoryCode == long.Parse(sCategoryCode.Text) && x.PartCode == item.Key && x.SubPartCode == 0);

                        sPartNameCodeNew.Items.Clear();
                        sPartNameCodeNew.ResetText();
                        sPartNameCodeNew.Items.Add(String.Format("{0:000}", item.Key / 1000));
                        sPartNameCodeNew.SelectedIndex = 0;
                        sPartNameCodeNew.Enabled = false;

                        sPartNameNew.Items.Clear();
                        sPartNameNew.ResetText();
                        sPartNameNew.Items.Add(item.Value);
                        sPartNameNew.SelectedIndex = 0;
                        sPartNameNew.Enabled = false;

                        break;
                    }
                }

                subpartSifrarnik.Clear();
                subpartSifrarnik = qc.GetSubPartNamesAllSifrarnikSortByName(sPartName.SelectedItem.ToString()); 

                sSubPartName.ResetText();
                sSubPartName.Items.Clear();

                long bigest = 0;
                foreach (KeyValuePair<long, String> key in subpartSifrarnik)
                {
                    sSubPartName.Items.Add(key.Value);
                    if (bigest < key.Key)
                        bigest = key.Key;
                }
                lastIDSubPart.Text = bigest.ToString();

                for (long i = bigest + 1; i < 1000; i++)
                {
                    sSubPartNameCodeNew.Items.Add(i);
                }
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
            finally
            {
                PostaviFullCode();
                Program.LoadStop();
            }
        }

        private void sSubPartName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Program.LoadStart();

                sSubPartNameCode.ResetText();

                sSubPartNameCodeNew.Enabled = true;
                sSubPartNameNew.Enabled = true;

                foreach (KeyValuePair<long, String> item in subpartSifrarnik)
                {
                    if (item.Value.Equals(sSubPartName.SelectedItem.ToString()))
                    {
                        sSubPartNameCode.Text = item.Key.ToString();
                        prtSif = partListSifrarnik.Find(x => x.CategoryCode == long.Parse(sCategoryCode.Text) && x.PartCode == long.Parse(sPartNameCode.Text) && x.SubPartCode == item.Key);

                        sSubPartNameCodeNew.Items.Clear();
                        sSubPartNameCodeNew.ResetText();
                        sSubPartNameCodeNew.Items.Add(String.Format("{0:000}", item.Key));
                        sSubPartNameCodeNew.SelectedIndex = 0;
                        sSubPartNameCodeNew.Enabled = false;

                        sSubPartNameNew.Items.Clear();
                        sSubPartNameNew.ResetText();
                        sSubPartNameNew.Items.Add(item.Value);
                        sSubPartNameNew.SelectedIndex = 0;
                        sSubPartNameNew.Enabled = false;

                        PriceINKNTB.Text = prtSif.PriceInKn.ToString();
                        PriceOUTKNTB.Text = prtSif.PriceOutKn.ToString();
                        PriceINEURTB.Text = prtSif.PriceInEur.ToString();
                        PriceOUTEURTB.Text = prtSif.PriceOutEur.ToString();
                        PackingCB.Text = prtSif.Packing;
                        PartNumberTB.Text = prtSif.PartNumber;

                        break;
                    }
                }
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                //MessageBox.Show(e1.Message);
            }
            finally
            {
                PostaviFullCode();
                Program.LoadStop();
            }
        }

        private void PostaviFullCode()
        {
            try
            {
                //if (!sSubPartNameCode.Text.Equals(""))
                //{
                //    sFullCode.Text = String.Format("{0: 000 000 000}", long.Parse(sCategoryCode.Text) + long.Parse(sPartNameCode.Text) + long.Parse(sSubPartNameCode.Text));
                //}
                //else if (!sPartNameCode.Text.Equals(""))
                //{
                //    sFullCode.Text = String.Format("{0: 000 000 000}", long.Parse(sCategoryCode.Text) + long.Parse(sPartNameCode.Text) );
                //}
                //else
                //{
                //    sFullCode.Text = String.Format("{0: 000 000 000}", long.Parse(sCategoryCode.Text) );
                //}

                sFullCode.Text = String.Format("{0: 000 000 000}", prtSif.FullCode);
                sFullName.Text = prtSif.FullName;
            }
            catch
            {
                sFullCode.Text = "000 000 000";
                sFullName.Text = sCategory.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sPartName.Items.Clear();
            sSubPartName.Items.Clear();


            sCategoryCode.ResetText();
            sCategory.ResetText();
            sPartNameCode.ResetText();
            sPartName.ResetText();
            sSubPartNameCode.ResetText();
            sSubPartName.ResetText();

            sCategoryCodeNew.ResetText();
            sCategoryNew.ResetText();
            sPartNameCodeNew.ResetText();
            sPartNameNew.ResetText();
            sSubPartNameCodeNew.ResetText();
            sSubPartNameNew.ResetText();

            PriceINKNTB.ResetText();
            PriceOUTKNTB.ResetText();
            PriceINEURTB.ResetText();
            PriceOUTEURTB.ResetText();
            PackingCB.ResetText();
            PartNumberTB.ResetText();

            sCategoryCodeNew.Enabled = true;
            sCategoryNew.Enabled = true;
            sPartNameCodeNew.Enabled = true;
            sPartNameNew.Enabled = true;
            sSubPartNameCodeNew.Enabled = true;
            sSubPartNameNew.Enabled = true;

            prtSif = null;
            PostaviFullCode();
        }

        private void AddNewPart_Click(object sender, EventArgs e)
        {
            String CategoryName;
            String PartName;
            String SubPartName;
            String Packing;
            String PartNumber;

            long CategoryCode;
            long PartCode;
            long SubPartCode;

            double PriceInKn;
            double PriceOutKn;
            double PriceInEur;
            double PriceOutEur;

            if (!CheckFields(true))
            {
                MessageBox.Show("Please fill in all required fields." + Environment.NewLine + "Nothing done!");
                return;
            }

            try
            {
                PriceInKn = double.Parse(PriceINKNTB.Text.Replace(',', '.'));
                PriceOutKn = double.Parse(PriceOUTKNTB.Text.Replace(',', '.'));
                PriceInEur = double.Parse(PriceINEURTB.Text.Replace(',', '.'));
                PriceOutEur = double.Parse(PriceOUTEURTB.Text.Replace(',', '.'));

                CategoryCode = long.Parse(sCategoryCodeNew.Text) * 1000000;
                PartCode = long.Parse(sPartNameCodeNew.Text) * 1000;
                SubPartCode = long.Parse(sSubPartNameCodeNew.Text);

                Packing = PackingCB.Text;

                CategoryName = sCategoryNew.Text;
                PartName = sPartNameNew.Text;
                SubPartName = sSubPartNameNew.Text;

                PartNumber = PartNumberTB.Text;

                if (qc.AddPartToSifrarnik(CategoryCode, CategoryName, PartCode, PartName, SubPartCode, SubPartName, PartNumber, PriceInKn, PriceOutKn, PriceInEur, PriceOutEur, Packing))
                {
                    MessageBox.Show("Part added.");
                    button1.PerformClick();
                }
                else
                {
                    MessageBox.Show("Part already exist." + Environment.NewLine + "Nothing done!");
                }
            }
            catch(Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message + Environment.NewLine + "Nothing done!");
            }
        }

        private Boolean CheckFields(Boolean insert)
        {
            try
            {
                if (insert)
                {
                    if (sCategoryNew.Text.Equals("")) return false;
                    if (sCategoryCodeNew.Text.Equals("")) return false;

                    if (sPartNameNew.Text.Equals("")) return false;
                    if (sPartNameCodeNew.Text.Equals("")) return false;

                    if (sSubPartNameNew.Text.Equals("")) return false;
                    if (sSubPartNameCodeNew.Text.Equals("")) return false;
                }
                else //update
                {
                    if (sCategory.Text.Equals("")) return false;

                    if (sPartName.Text.Equals("")) return false;

                    if (sSubPartName.Text.Equals("")) return false;
                }

                if (PriceINKNTB.Text.Equals("")) return false;
                if (PriceOUTKNTB.Text.Equals("")) return false;
                if (PriceINEURTB.Text.Equals("")) return false;
                if (PriceOUTEURTB.Text.Equals("")) return false;
                if (PackingCB.Text.Equals("")) return false;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                return false;
            }

            return true;
        }

        private void UpdatePart_Click(object sender, EventArgs e)
        {
            String CategoryName;
            String PartName;
            String SubPartName;
            String Packing;
            String PartNumber;

            long CategoryCode;
            long PartCode;
            long SubPartCode;

            double PriceInKn;
            double PriceOutKn;
            double PriceInEur;
            double PriceOutEur;

            if (!CheckFields(false))
            {
                MessageBox.Show("Please fill in all required fields." + Environment.NewLine + "Nothing done!");
                return;
            }

            try
            {
                PriceInKn = double.Parse(PriceINKNTB.Text.Replace(',', '.'));
                PriceOutKn = double.Parse(PriceOUTKNTB.Text.Replace(',', '.'));
                PriceInEur = double.Parse(PriceINEURTB.Text.Replace(',', '.'));
                PriceOutEur = double.Parse(PriceOUTEURTB.Text.Replace(',', '.'));

                CategoryCode = long.Parse(sCategoryCodeNew.Text) * 1000000;
                PartCode = long.Parse(sPartNameCodeNew.Text) * 1000;
                SubPartCode = long.Parse(sSubPartNameCodeNew.Text);

                Packing = PackingCB.Text;

                CategoryName = sCategoryNew.Text;
                PartName = sPartNameNew.Text;
                SubPartName = sSubPartNameNew.Text;

                PartNumber = PartNumberTB.Text;

                if (qc.UpdatePartSifrarnik(CategoryCode, CategoryName, PartCode, PartName, SubPartCode, SubPartName, PartNumber, PriceInKn, PriceOutKn, PriceInEur, PriceOutEur, Packing))
                {
                    MessageBox.Show("Part updated.");
                    button1.PerformClick();
                }
                else
                {
                    MessageBox.Show("Part does not exist." + Environment.NewLine + "Nothing done!");
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message + Environment.NewLine + "Nothing done!");
            }
        }
    }
}
