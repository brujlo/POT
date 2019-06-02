using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.Documents
{
    public partial class Racun : Form
    {
        Boolean pictureOn = false;
        double ech = 0;
        String echDate = "";
        int indexPartCB = -1;
        int valuta = 15;
        long racunID = 0;
        int rb = 0;
        decimal TOTAL = 0;

        QueryCommands qc = new QueryCommands();
        ConnectionHelper cn = new ConnectionHelper();

        List<Company> cmpList = new List<Company>();
        List<PartSifrarnik> sifrarnikList = new List<PartSifrarnik>();
        PartSifrarnik tempSifPart = new PartSifrarnik();
        Company customerCmp = new Company();
        MainCmp mainCmp = new MainCmp();
        List<String> exchng = new List<string>();

        Invoice invoice = new Invoice();
        List<InvoiceParts> invoicePartsList = new List<InvoiceParts>();

        public Racun()
        {
            InitializeComponent();
        }

        private void Racun_Load(object sender, EventArgs e)
        {
            racunID = invoice.GetNewInvoiceNumber();
            InvNbrLB.Text = invoice.IDLongtoString(racunID);
            obrJedLB.Text = Properties.Settings.Default.ObracunskaJedinica.ToString();
            QuantityTB.Text = "1";

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            
            mainCmp.GetMainCmpInfoByID(Properties.Settings.Default.CmpID);

            exchng = qc.CurrentExchangeRate();

            ech =double.Parse(exchng[3]);
            echDate = exchng[1];


            ExchangeLB.Text = String.Format("{0,000:N3}", ech);
            EchDateLB.Text = echDate;

            DateTime dt = DateTime.Today.AddDays(valuta);
            ValutaLB.Text = valuta.ToString() + " (" + dt.ToString("dd.MM.yy.") + ")";

            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("PART NAME");
            listView1.Columns.Add("PART CODE");
            listView1.Columns.Add("PRICE");
            listView1.Columns.Add("WORK TIME");
            listView1.Columns.Add("REBATE");
            listView1.Columns.Add("AMOUNT");
            listView1.Columns.Add("REBATE PRICE");
            listView1.Columns.Add("TOTAL");

            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            try
            {
                Company tempCmp = new Company();
                cmpList = tempCmp.GetAllCompanyInfoSortByName();

                if (cmpList.Count > 0)
                {
                    foreach (Company cmp in cmpList)
                    {
                        CustomerCB.Items.Add(cmp.Name);
                    }
                }

                PartSifrarnik tempSifPart = new PartSifrarnik();
                sifrarnikList = tempSifPart.GetPartsAllSifrarnikSortByFullName();

                if (sifrarnikList.Count > 0)
                {
                    foreach (PartSifrarnik sif in sifrarnikList)
                    {
                        PartNameCB.Items.Add(sif.FullName);
                    }
                }
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                new LogWriter(e1);
            }
            Program.LoadStop();
        }

        private void CancelBT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
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
                            pictureBox1.Image = Properties.Resources.LoadDataOn;
                            pictureOn = true;
                        }

                        Thread.Sleep(60000);
                    }
                    else
                    {
                        if (pictureOn)
                        {
                            pictureBox1.Image = Properties.Resources.LoadDataOff;
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

        private void PartNameCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            indexPartCB = PartNameCB.SelectedIndex;

            tempSifPart = sifrarnikList.ElementAt(indexPartCB);

            String prtCod = String.Format("{0:000 000 000}", sifrarnikList.ElementAt(indexPartCB).FullCode);
            PartCodeTB.Text = String.Format("{0:00}", mainCmp.Code) + String.Format("{0:00}" + " ", customerCmp.Code) + prtCod;

            if (radioButtonHRV.Checked)
                PriceTB.Text = CheckIfKNZero(sifrarnikList.ElementAt(indexPartCB));
            else
                PriceTB.Text = CheckIfKNZero(sifrarnikList.ElementAt(indexPartCB));
        }

        private void CustomerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            customerCmp = cmpList.ElementAt(CustomerCB.SelectedIndex);
        }

        private void radioButtonENG_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonENG.Checked)
            {
                CurencyLB.Text = "€";
                CurrencyLB.Text = "€";
                ExchangeLB.Text = String.Format( "{0:N3}",(1 / ech));
            }
            else
            {
                CurencyLB.Text = "KN";
                CurrencyLB.Text = "KN";
                ExchangeLB.Text = String.Format("{0:N3}", ech);
            }

            if (indexPartCB != -1)
            {
                if (radioButtonHRV.Checked)
                    PriceTB.Text = CheckIfKNZero(sifrarnikList.ElementAt(indexPartCB));
                else
                    PriceTB.Text = CheckIfKNZero(sifrarnikList.ElementAt(indexPartCB));
            }
        }

        private String CheckIfKNZero(PartSifrarnik sprt)
        {
            decimal kn = sprt.PriceOutKn;
            decimal eur = sprt.PriceOutEur;

            if (radioButtonENG.Checked)
                return String.Format("{0:N2}", eur);
            else
                return kn <= 0 ? String.Format("{0:N2}", eur * (decimal)ech) : String.Format("{0:N2}", kn);
        }

        private void WorkTimeTB_Leave(object sender, EventArgs e)
        {
            try
            {
                String time = WorkTimeTB.Text;
                long h = 0;
                long m = 0;
                int length = WorkTimeTB.Text.Length;

                switch (length)
                {
                    case 1:
                        h = 0;
                        m = long.Parse(time);
                        WorkTimeTB.Text = String.Format("{0:00}" + ":" + "{1:00}", h, m);
                        break;
                    case 2:
                        h = 0;
                        m = long.Parse(time);
                        WorkTimeTB.Text = String.Format("{0:00}" + ":" + "{1:00}", h, m);
                        break;
                    case 3:
                        h = long.Parse(time.Substring(0,1)); 
                        m = long.Parse(time.Substring(1, 2));
                        WorkTimeTB.Text = String.Format("{0:00}" + ":" + "{1:00}", h, m);
                        break;
                    case 4:
                        h = long.Parse(time.Substring(0, 2));
                        m = long.Parse(time.Substring(2, 2));
                        WorkTimeTB.Text = String.Format("{0:00}" + ":" + "{1:00}", h, m);
                        break;
                    default:

                        h = long.Parse(time.Substring(2, length - 2));
                        m = long.Parse(time.Substring(length - 2, 2));
                        WorkTimeTB.Text = String.Format("{0:00}" + ":" + "{1:00}", h, m);
                        break;
                }
            }
            catch 
            {
                if (WorkTimeTB.Text.Equals(""))
                    WorkTimeTB.Text = "01:00";
                else
                    WorkTimeTB.Text = WorkTimeTB.Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String vrijemeRada = WorkTimeTB.Text;
            decimal rabat = decimal.Parse(RebateTB.Text);
            int kolicina = int.Parse(QuantityTB.Text);
            
            try
            {
                InvoiceParts invoicePart = new InvoiceParts(racunID, tempSifPart.FullCode, vrijemeRada, rabat, kolicina);

                invoicePartsList.Add(invoicePart);

                addToList(true, invoicePart);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        private void addToList(Boolean clear, InvoiceParts invPrt)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Add to list";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            decimal cijena = decimal.Parse(CheckIfKNZero(tempSifPart));
            decimal popust = invPrt.Rabat;
           

            decimal partTotal = invPrt.PartTotalPrice(cijena, popust, invPrt.VrijemeRada);
            decimal partsTotal = partTotal * invPrt.Kolicina;

            TOTAL += partsTotal;

            try
            {
                ListViewItem lvi1 = new ListViewItem();
                rb = listView1.Items.Count + 1;
                lvi1.Text = rb.ToString();
                
                lvi1.SubItems.Add(tempSifPart.FullName);
                lvi1.SubItems.Add(mainCmp.Code.ToString() + customerCmp.Code.ToString() + Decoder.GetFullPartCodeStr(tempSifPart.FullCode.ToString()));
                lvi1.SubItems.Add(String.Format("{0:0.00}", cijena));
                lvi1.SubItems.Add(invPrt.VrijemeRada);
                lvi1.SubItems.Add(String.Format("{0:0.00}", invPrt.Rabat));
                lvi1.SubItems.Add(String.Format("{0:#0}", invPrt.Kolicina));
                lvi1.SubItems.Add(String.Format("{0:0.00}", partTotal));
                lvi1.SubItems.Add(String.Format("{0:0.00}", partsTotal));

                if (listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                listView1.Items.Add(lvi1);

                String dd = tempSifPart.FullName + ", " + Decoder.GetFullPartCodeStr(tempSifPart.FullCode.ToString()) + ", " + tempSifPart.PriceInKn.ToString() + ", " + invPrt.VrijemeRada + ", " + invPrt.Rabat.ToString() + ", " + invPrt.Kolicina.ToString() + ", " + partTotal + ", " + partsTotal + ", " + TOTAL.ToString();

                if (radioButtonENG.Checked)
                {
                    TotalEURLB.Text = String.Format("{0:0.00}", TOTAL);
                    TotalKNLB.Text = String.Format("{0:0.00}", (TOTAL * (decimal)ech));
                }
                else
                {
                    TotalEURLB.Text = String.Format("{0:0.00}", TOTAL / (decimal)ech);
                    TotalKNLB.Text = String.Format("{0:0.00}", TOTAL);
                }

                if (data.Equals(""))
                    data = dd;
                else
                    data = data + Environment.NewLine + "             " + dd;
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

            //if (clear)
            //{
            //    textBox1.Clear();
            //    textBox2.Clear();
            //    textBox3.Clear();
            //}

            //textBox1.SelectAll();
            //isFocused = true;
            //textBox1.Focus();
            //isFocused = false;

            Result = "Added";
            lw.LogMe(function, usedQC, data, Result);

            SystemSounds.Hand.Play();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void RebateTB_Leave(object sender, EventArgs e)
        {
            if (RebateTB.Text.Equals(""))
                RebateTB.Text = "0"; 
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime endDate = monthCalendar1.SelectionRange.Start;
            DateTime startDate = DateTime.Now;
            valuta = (int)(endDate - startDate).TotalDays + 1;

            DateTime dt = DateTime.Today.AddDays(valuta);
            ValutaLB.Text = valuta.ToString() + " (" + dt.ToString("dd.MM.yy.") + ")";
        }
    }
}
