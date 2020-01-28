using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.Documents
{
    public partial class Racun : Form
    {
        Boolean pictureOn = false;
        Boolean storno = false;
        
        double ech = 0;
        String echDate = "";
        int indexPartCB = -1;
        int valuta = 15;
        long racunID = 0;
        int rb = 0;

        decimal vat;
        decimal TOTAL = 0;
        decimal TOTALTAXBASE = 0;
        decimal TOTALTAX = 0;

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
        InvoiceParts recalculateInvPart = new InvoiceParts();

        Invoice stornoInvoice = new Invoice();
        List<InvoiceParts> stornoPartsList = new List<InvoiceParts>();
        Invoice newInvoice = new Invoice();

        List<Offer> ponudeList = new List<Offer>();
        List<Invoice> invList = new List<Invoice>();

        public Racun()
        {
            InitializeComponent();
        }
        
        private void Racun_Load(object sender, EventArgs e)
        {  
            invoice.Naplaceno = 0;
            invoice.Operater = WorkingUser.UserID.ToString();
            invoice.PonudaID = 0;
            invoice.Storno = 0;
            invoice.Konverzija = 1;

            if (radioButtonENG.Checked)
            {
                invoice.NacinPlacanja = Properties.Settings.Default.PaymentForm;
                vat = Properties.Settings.Default.TAX2 / 100;
            }
            else{
                invoice.NacinPlacanja = Properties.Settings.Default.NacinPlacanja;
                vat = Properties.Settings.Default.TAX1 / 100;
            }


            invoice.Id = racunID = invoice.GetNewInvoiceNumber();
            InvNbrLB.Text = invoice.IDLongtoString(racunID);
            obrJedLB.Text = Properties.Settings.Default.ObracunskaJedinica.ToString();
            QuantityTB.Text = "1";

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            
            mainCmp.GetMainCmpInfoByID(Properties.Settings.Default.CmpID);

            exchng = qc.CurrentExchangeRate();

            ech = double.Parse(exchng[3]);
            invoice.Eur = (decimal)ech;
            echDate = exchng[1];


            ExchangeLB.Text = String.Format("{0,000:N3}", ech);
            invoice.DanTecaja = EchDateLB.Text = echDate;
            invoice.DatumIzdano = DateTime.Now.ToString("dd.MM.yy.");
            invoice.DatumNaplaceno = "01.01.01.";

            DateTime dt = DateTime.Today.AddDays(valuta);
            ValutaLB.Text = valuta.ToString() + " (" + dt.ToString("dd.MM.yy.") + ")";
            invoice.Valuta = valuta;

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

            listView2.View = View.Details;

            listView2.Columns.Add("RB");
            listView2.Columns.Add("ID");
            listView2.Columns.Add("PonudaID");
            listView2.Columns.Add("DatumIzdano");
            listView2.Columns.Add("Iznos");
            listView2.Columns.Add("DatumNaplaceno");
            listView2.Columns.Add("Naplaceno");
            listView2.Columns.Add("CustomerID");
            listView2.Columns.Add("EUR");
            listView2.Columns.Add("Napomena");
            listView2.Columns.Add("VrijemeIzdano");
            listView2.Columns.Add("Valuta");
            listView2.Columns.Add("Operater");
            listView2.Columns.Add("DanTecaja");
            listView2.Columns.Add("NacinPlacanja");
            listView2.Columns.Add("Storno");

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


                ponudeList = qc.GetAllOffers();

                if (ponudeList.Count > 0)
                {
                    foreach (Offer off in ponudeList)
                    {
                        OfferCB.Items.Add(off.Id);
                    }
                }
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                new LogWriter(e1);
            }
            finally
            {
                Program.LoadStop();
                this.Focus();
            }
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
            tempSifPart = null;

            if (CustomerCB.Text.Equals("") || CustomerCB.Text.Equals("Customer"))
            {
                MessageBox.Show("Please, select company first.","Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                indexPartCB = PartNameCB.SelectedIndex;

                tempSifPart = sifrarnikList.ElementAt(indexPartCB);

                String prtCod = String.Format("{0:000 000 000}", tempSifPart.FullCode);
                PartCodeTB.Text = String.Format("{0:00}", mainCmp.Code) + String.Format("{0:00}" + " ", customerCmp.Code) + prtCod;

                if (radioButtonHRV.Checked)
                    PriceTB.Text = CheckIfKNZero(tempSifPart);
                else
                    PriceTB.Text = CheckIfKNZero(tempSifPart);

                PriceINEURTB.Text = String.Format("{0:N2}", tempSifPart.PriceInEur);
                PriceINKNTB.Text = String.Format("{0:N2}", tempSifPart.PriceInKn);
                PriceOUTEURTB.Text = String.Format("{0:N2}", tempSifPart.PriceOutEur);
                PriceOUTKNTB.Text = String.Format("{0:N2}", tempSifPart.PriceOutKn);
            }
        }

        private void CustomerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            customerCmp = cmpList.ElementAt(CustomerCB.SelectedIndex);
            invoice.CustomerID = customerCmp.ID;
        }

        private void radioButtonENG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonENG.Checked)
                {
                    vat = Properties.Settings.Default.TAX2 / 100;

                    invoice.NacinPlacanja = Properties.Settings.Default.PaymentForm;
                    invoice.Konverzija = 0;

                    CurencyLB.Text = "€";
                    CurrencyLB.Text = "€";
                    ExchangeLB.Text = String.Format("{0:N3}", (1 / ech));

                    TaxBaseLB.Text = String.Format("{0:0.00}", TOTALTAXBASE / (decimal)ech);
                    VATLB.Text = String.Format("{0:0.00}", TOTALTAX / (decimal)ech);
                }
                else
                {
                    vat = Properties.Settings.Default.TAX1 / 100;

                    invoice.NacinPlacanja = Properties.Settings.Default.NacinPlacanja;
                    invoice.Konverzija = 1;

                    CurencyLB.Text = "KN";
                    CurrencyLB.Text = "KN";
                    ExchangeLB.Text = String.Format("{0:N3}", ech);

                    TaxBaseLB.Text = String.Format("{0:0.00}", TOTALTAXBASE);
                    VATLB.Text = String.Format("{0:0.00}", TOTALTAX);
                }

                if (indexPartCB != -1)
                {
                    decimal kn = tempSifPart.PriceOutKn;
                    decimal eur = tempSifPart.PriceOutEur;

                    if (radioButtonENG.Checked)
                    {
                        PriceTB.Text = eur > 0 ? String.Format("{0:N2}", eur) : String.Format("{0:N2}", kn / (decimal)ech);
                    }
                    else
                    {
                        PriceTB.Text = kn > 0 ? String.Format("{0:N2}", kn) : String.Format("{0:N2}", eur * (decimal)ech);
                    }
                }
            }
            catch { }
        }

        private String CheckIfKNZero(PartSifrarnik sprt)
        {
            decimal kn = sprt.PriceOutKn;
            decimal eur = sprt.PriceOutEur;

            if (radioButtonENG.Checked)
                return String.Format("{0:N2}", eur <= 0 ? kn / (decimal)ech : eur);
            else
                return String.Format("{0:N2}", kn <= 0 ? eur * (decimal)ech : kn);
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

            radioButtonENG.Enabled = false;
            radioButtonHRV.Enabled = false;

            try
            {
                InvoiceParts invoicePart = new InvoiceParts(racunID, tempSifPart.FullCode, vrijemeRada, rabat, kolicina);

                int indx = invoicePart.Compare(invoicePartsList, invoicePart);
                if ( indx >= 0 )
                {
                    listView1.Items[indx].SubItems[6].Text = (invoicePartsList.ElementAt(indx).Kolicina += invoicePart.Kolicina).ToString();
                    invoicePartsList[indx].IznosTotal = String.Format("{0:N2}", decimal.Parse(invoicePartsList[indx].IznosTotal) + invoicePart.Kolicina * decimal.Parse(invoicePartsList[indx].IznosRabat));
                    listView1.Items[indx].SubItems[8].Text = invoicePartsList.ElementAt(indx).IznosTotal;
                    addToList(false, true, invoicePart);

                    SystemSounds.Hand.Play();
                }
                else
                {
                    invoicePartsList.Add(invoicePart);

                    addToList(false, false, invoicePart);

                    recalculateInvPart = invoicePart;
                }

                PartNameCB.Focus();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        private void addToList(Boolean remove, Boolean update, InvoiceParts invPrt)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Add to list";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            decimal cijena = 0;
            decimal popust = 0;


            decimal partTotal = 0;
            decimal partsTotal = 0;

            //decimal cijena = decimal.Parse(CheckIfKNZero(tempSifPart));
            if (!remove)
            {

                decimal kn = tempSifPart.PriceOutKn;
                decimal eur = tempSifPart.PriceOutEur;

                if (radioButtonENG.Checked)
                {
                    cijena = eur > 0 ? eur : kn / (decimal)ech;
                }
                else
                {
                    cijena = kn > 0 ? kn : eur * (decimal)ech;
                }

                popust = invPrt.Rabat;
           

                partTotal = invPrt.PartTotalPrice(cijena, popust, invPrt.VrijemeRada);
                partsTotal = partTotal * invPrt.Kolicina;

                //TOTAL += partsTotal;
                TOTALTAXBASE += partsTotal;
                TOTALTAX = TOTALTAXBASE * vat;
                TOTAL = TOTALTAX + TOTALTAXBASE;

                invoice.Iznos = TOTAL;
            }
            else
            {
                cijena = decimal.Parse(invPrt.IznosTotal);

                //TOTAL += partsTotal;
                TOTALTAXBASE -= cijena;
                TOTALTAX = TOTALTAXBASE * vat;
                TOTAL = TOTALTAX + TOTALTAXBASE;

                invoice.Iznos = TOTAL;
            }

            try
            {
                String dd;

                if (!remove && !update)
                {
                    ListViewItem lvi1 = new ListViewItem();
                    rb = listView1.Items.Count + 1;
                    lvi1.Text = rb.ToString();
                
                    lvi1.SubItems.Add(tempSifPart.FullName);
                    lvi1.SubItems.Add(mainCmp.Code.ToString() + customerCmp.Code.ToString() + Decoder.GetFullPartCodeStr(tempSifPart.FullCode));
                    lvi1.SubItems.Add(String.Format("{0:0.00}", cijena));
                    lvi1.SubItems.Add(invPrt.VrijemeRada);
                    lvi1.SubItems.Add(String.Format("{0:0.00}", invPrt.Rabat));
                    lvi1.SubItems.Add(String.Format("{0:#0}", invPrt.Kolicina));
                    lvi1.SubItems.Add(String.Format("{0:0.00}", partTotal));
                    lvi1.SubItems.Add(String.Format("{0:0.00}", partsTotal));

                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);

                    listView1.Items.Add(lvi1);

                    dd = tempSifPart.FullName + ", " + Decoder.GetFullPartCodeStr(tempSifPart.FullCode.ToString()) + ", " + tempSifPart.PriceInKn.ToString() + ", " + invPrt.VrijemeRada + ", " + invPrt.Rabat.ToString() + ", " + invPrt.Kolicina.ToString() + ", " + partTotal + ", " + partsTotal + ", " + TOTAL.ToString();
                    invPrt.IznosPart = String.Format("{0:N2}", cijena);
                    invPrt.IznosRabat = String.Format("{0:N2}", partTotal);
                    invPrt.IznosTotal = String.Format("{0:N2}", partsTotal);

                    if (data.Equals(""))
                        data = dd;
                    else
                        data = data + Environment.NewLine + "             " + dd;
                }
                else
                {
                    dd = tempSifPart.FullName + ", " + Decoder.GetFullPartCodeStr(tempSifPart.FullCode.ToString()) + ", " + tempSifPart.PriceInKn.ToString() + ", " + invPrt.VrijemeRada + ", " + invPrt.Rabat.ToString() + ", " + invPrt.Kolicina.ToString() + ", " + partTotal + ", " + partsTotal + ", " + TOTAL.ToString();
                    if (data.Equals(""))
                        data = dd;
                    else
                        data = data + Environment.NewLine + "             " + dd;
                }

                if (radioButtonENG.Checked)
                {
                    TaxBaseLB.Text = String.Format("{0:0.00}", TOTALTAXBASE * (decimal)ech);
                    VATLB.Text = String.Format("{0:0.00}", TOTALTAX * (decimal)ech);

                    TotalEURLB.Text = String.Format("{0:0.00}", TOTAL);
                    TotalKNLB.Text = String.Format("{0:0.00}", (TOTAL * (decimal)ech));
                }
                else
                {
                    TaxBaseLB.Text = String.Format("{0:0.00}", TOTALTAXBASE);
                    VATLB.Text = String.Format("{0:0.00}", TOTALTAX);

                    TotalEURLB.Text = String.Format("{0:0.00}", TOTAL / (decimal)ech);
                    TotalKNLB.Text = String.Format("{0:0.00}", TOTAL);
                }

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
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Remove selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///
            
            if (listView1.SelectedItems.Count <= 0)
                return;

            var itemIndx = listView1.SelectedIndices[0];
            int k = 1;
            
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
                if (data.Equals(""))
                    data = item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5] + ", " + item.SubItems[6] + ", " + item.SubItems[7] + ", " + item.SubItems[8];
                else
                    data = data + Environment.NewLine + "             " + item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5] + ", " + item.SubItems[6] + ", " + item.SubItems[7] + ", " + item.SubItems[8];
            }

            foreach (ListViewItem item in listView1.Items)
            {
                if (listView1.SelectedItems != null && listView1.Items.Count != 0)
                {
                    item.SubItems[0].Text = k.ToString();
                    k++;
                }
            }
            
            addToList(true, false, invoicePartsList[itemIndx]);
            invoicePartsList.RemoveAt(itemIndx);

            Result = "Removed";
            lw.LogMe(function, usedQC, data, Result);

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

        private void WorkTimeTB_Enter(object sender, EventArgs e)
        {
            WorkTimeTB.Focus();
            WorkTimeTB.SelectAll();
        }

        private void RebateTB_Enter(object sender, EventArgs e)
        {
            RebateTB.Focus();
            RebateTB.SelectAll();
        }

        private void QuantityTB_Enter(object sender, EventArgs e)
        {
            QuantityTB.Focus();
            QuantityTB.SelectAll();
        }

        private void QuantityTB_MouseClick(object sender, MouseEventArgs e)
        {
            QuantityTB.Focus();
            QuantityTB.SelectAll();
        }

        private void RebateTB_MouseClick(object sender, MouseEventArgs e)
        {
            RebateTB.Focus();
            RebateTB.SelectAll();
        }

        private void WorkTimeTB_MouseClick(object sender, MouseEventArgs e)
        {
            WorkTimeTB.Focus();
            WorkTimeTB.SelectAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "SaveInvoice";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            if (qc.IfInvoiceExist(invoice.Id))
            {
                data = invoice.Id.ToString() + Environment.NewLine;
                Result = "Invoice is already saved in DB, please make new one.";
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOTHING DONE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Program.SaveStart();


            invoice.VrijemeIzdano = DateTime.Now.ToString("HH:mm");
            invoice.Napomena = textBox4.Text;

            invoice.PonudaID = (OfferCB.Text.Equals("") || OfferCB.Text.Equals("Offer")) ? 0 : long.Parse(OfferCB.Text);

            foreach (InvoiceParts prt in invoicePartsList)
            {
                prt.AddInvoiceToPart(invoice);
            }
            try
            {
                //invoice.Id = 400401419; //treba i u queryu promjeniti newID
                //invoice.DatumIzdano = "30.12.19";
                //invoice.VrijemeIzdano = "12:34";
                //invoice.Eur = (decimal)7.4391;
                //invoice.DanTecaja = "30.12.19";

                if (qc.SaveInvoice(invoicePartsList, invoice, invoice.Storno))
                {
                    if (Program.SaveDocumentsPDF) saveToPDF();

                    Program.SaveStop();
                    MessageBox.Show("SAVED");
                }
                else
                {
                    Program.SaveStop();
                    MessageBox.Show("NOT SAVED");
                }
            }
            catch(Exception e1)
            {
                Program.SaveStop();
                data = invoice.Id.ToString() + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label23.Text = "Letters left " + (200 - textBox4.TextLength).ToString();
        }

        private void printDocumentInvoice_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            invoice.VrijemeIzdano = DateTime.Now.ToString("HH:mm"); //TODO Delete provjeri da li se u racunu sprema
            invoice.Napomena = textBox4.Text;

            foreach (InvoiceParts prt in invoicePartsList)
            {
                prt.AddInvoiceToPart(invoice);
            }

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Print";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            List<InvoiceParts> tempLst = new List<InvoiceParts>();
            Invoice tmpInv = new Invoice();

            if ( storno)
            {
                tempLst = stornoPartsList;
                tmpInv = newInvoice;
            }
            else
            {
                tempLst = invoicePartsList;
                tmpInv = invoice;
            }

            PrintMeInvoice pr = new PrintMeInvoice(tempLst, tmpInv, storno ? 1 : 0, radioButtonENG.Checked, TOTALTAXBASE, TOTALTAX);
            pr.Print(e);
            
            //data = cmpS + ", " + cmpR + ", " + sifrarnikArr + ", " + partListPrint + ", " + IISNumber + ", " + napomenaIISPrint + ", IIS, customer, false";
            Result = "Print page called";
            lw.LogMe(function, usedQC, data, Result);

            if (!e.HasMorePages)
            {
                Properties.Settings.Default.pageNbr = 1;
                Properties.Settings.Default.Save();
            }
        }

        private void PrintPrewBT_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            printPreviewDialogInvoice.Document = printDocumentInvoice;

            printPreviewDialogInvoice.Size = new Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialogInvoice.ShowDialog();

            //textBox1.SelectAll();
            //isFocused = true;
            //textBox1.Focus();
            //isFocused = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog1 = new PrintDialog();

            printDialog1.Document = printDocumentInvoice;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog pdfSaveDialog = new SaveFileDialog();

                if (printDialog1.PrinterSettings.PrinterName == "Microsoft Print to PDF")
                {   // force a reasonable filename
                    string fileName = invoice.IDLongtoString(invoice.Id).Replace("-", "");
                    string basename = Path.GetFileNameWithoutExtension("EXE " + fileName);
                    string directory = Path.GetDirectoryName("EXE " + fileName);
                    printDocumentInvoice.PrinterSettings.PrintToFile = true;
                    // confirm the user wants to use that name
                    pdfSaveDialog.InitialDirectory = directory;
                    pdfSaveDialog.FileName = basename + ".pdf";
                    pdfSaveDialog.Filter = "PDF File|*.pdf";
                    result = pdfSaveDialog.ShowDialog();
                    if (result != DialogResult.Cancel)
                        printDocumentInvoice.PrinterSettings.PrintFileName = pdfSaveDialog.FileName;
                }

                if (result != DialogResult.Cancel)  // in case they canceled the save as dialog
                {
                    printDocumentInvoice.Print();
                    MessageBox.Show("Saved to location: " + Environment.NewLine + pdfSaveDialog.FileName, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    PrintPrewBT_Click(sender, e);
                }
            }
        }

        private void WorkTimeTB_TextChanged(object sender, EventArgs e)
        {
            cijneaPrimjer();
        }

        private void RebateTB_TextChanged(object sender, EventArgs e)
        {
            cijneaPrimjer();
        }

        private void QuantityTB_TextChanged(object sender, EventArgs e)
        {
            cijneaPrimjer();
        }

        private void PriceTB_TextChanged(object sender, EventArgs e)
        {
            cijneaPrimjer();
        }

        private void cijneaPrimjer()
        {
            try
            {
                decimal priceTime = 0;

                if (radioButtonHRV.Checked)
                    priceTime = ( (tempSifPart.PriceOutKn == 0 ? tempSifPart.PriceOutEur * (decimal)ech : tempSifPart.PriceOutKn) * workTimeToNumber(WorkTimeTB.Text));
                else
                    priceTime = ((tempSifPart.PriceOutEur == 0 ? tempSifPart.PriceOutKn / (decimal)ech : tempSifPart.PriceOutEur) * workTimeToNumber(WorkTimeTB.Text));

                decimal rebatePrice = (priceTime * (decimal.Parse(RebateTB.Text) / 100));
                int qnt = int.Parse(QuantityTB.Text);

                decimal partPrice = (priceTime - rebatePrice) * qnt;
                PriceInfoLB.Text = String.Format("{0:N2}", partPrice);
                FullPriceInfoLB.Text = String.Format("{0:N2}", partPrice * vat + partPrice);
            }
            catch { }
        }

        private decimal workTimeToNumber(String wrk)
        {
            decimal rez = 1;

            if (!wrk.Equals("00:00"))
            {
                try
                {
                    decimal obrJed = Properties.Settings.Default.ObracunskaJedinica;

                    var spl = wrk.Split(':');
                    int sati = int.Parse(spl[0]);
                    int min = int.Parse(spl[1]);
                    decimal mm = (int)(min / obrJed);

                    if (min % obrJed > 0)
                        mm++;
                    mm = mm * obrJed / 60;

                    rez = sati + (mm);
                }
                catch
                {
                    return 1;
                }
            }

            return rez;
        }

        private void saveToPDF()
        {
            String printerName = printDialog1.PrinterSettings.PrinterName;

            try
            {
                PrintDialog printDialog1 = new PrintDialog();
                printDialog1.Document = printDocumentInvoice;

                printDialog1.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                if (!printDialog1.PrinterSettings.IsValid) return;

                if (!Directory.Exists(Properties.Settings.Default.DefaultFolder + "\\RAC"))
                    return;

                string fileName;
                if ( storno )
                    fileName = "\\EXE " + newInvoice.IDLongtoString(newInvoice.Id).Replace("-", "") + ".pdf";
                else
                    fileName = "\\EXE " + invoice.IDLongtoString(invoice.Id).Replace("-", "") + ".pdf";

                string directory = Properties.Settings.Default.DefaultFolder + "\\RAC";

                printDialog1.PrinterSettings.PrintToFile = true;
                printDocumentInvoice.PrinterSettings.PrintFileName = directory + fileName;
                printDocumentInvoice.PrinterSettings.PrintToFile = true;
                printDocumentInvoice.Print();
                
                printDialog1.PrinterSettings.PrintToFile = false;
                printDocumentInvoice.PrinterSettings.PrintToFile = false;
                printDialog1.PrinterSettings.PrinterName = printerName;
                printDocumentInvoice.PrinterSettings.PrinterName = printerName;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message + Environment.NewLine + "PDF file not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Program.LoadStop();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();

            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage2"])
            {
                Program.LoadStart();

                invList = qc.GetAllInvoices();

                if (invList.Count > 0)
                {
                    foreach (Invoice inv in invList)
                    {

                        ListViewItem lvi2 = new ListViewItem();
                        rb = listView2.Items.Count + 1;

                        lvi2.Text = rb.ToString();

                        if (inv.Naplaceno > 0)
                            lvi2.ForeColor = Color.Green;
                        else
                            lvi2.ForeColor = Color.Red;

                           
                        lvi2.SubItems.Add(inv.Id.ToString());
                        lvi2.SubItems.Add(inv.PonudaID.ToString());
                        lvi2.SubItems.Add(inv.DatumIzdano.ToString());
                        lvi2.SubItems.Add(inv.Iznos.ToString());
                        lvi2.SubItems.Add(inv.DatumNaplaceno.ToString());
                        lvi2.SubItems.Add(inv.Naplaceno.ToString());
                        lvi2.SubItems.Add(inv.CustomerID.ToString());
                        lvi2.SubItems.Add(inv.Eur.ToString());
                        lvi2.SubItems.Add(inv.Napomena.ToString());
                        lvi2.SubItems.Add(inv.VrijemeIzdano.ToString());
                        lvi2.SubItems.Add(inv.Valuta.ToString());
                        lvi2.SubItems.Add(inv.Operater.ToString());
                        lvi2.SubItems.Add(inv.DanTecaja.ToString());
                        lvi2.SubItems.Add(inv.NacinPlacanja.ToString());
                        lvi2.SubItems.Add(inv.Storno.ToString());

                        if (listView2.Items.Count > 0)
                            listView2.EnsureVisible(listView2.Items.Count - 1);

                        listView2.Items.Add(lvi2);
                    }
                }

                for (int i = 0; i < listView2.Columns.Count; i++)
                {
                    listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView2.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                Program.LoadStop();
                this.Focus();
            }
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Chage payed - selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                Program.SaveStart();

                if (listView2.SelectedItems == null)
                    return;

                var itemIndx = listView2.SelectedIndices[0];

                ListViewItem item = listView2.SelectedItems[0];

                if (decimal.Parse(item.SubItems[6].Text) > 0)
                {
                    qc.UpdateInvoicePaidTo0(invList[itemIndx].Id);
                    item.SubItems[5].Text = "01.01.01.";
                    item.SubItems[6].Text = "0";
                    item.ForeColor = Color.Red;
                }
                else
                {
                    qc.UpdateInvoicePaid(invList[itemIndx].Id, invList[itemIndx].Iznos, DateTime.Now.ToString("dd.MM.yy."));
                    item.SubItems[5].Text = DateTime.Now.ToString("dd.MM.yy.");
                    item.SubItems[6].Text = invList[itemIndx].Iznos.ToString();
                    item.ForeColor = Color.Green;
                }

                data = item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5] + ", " + item.SubItems[6] + ", " + item.SubItems[7] + ", " + item.SubItems[8];

                Result = "Changed";
                lw.LogMe(function, usedQC, data, Result);

                Program.SaveStop();
            }
            catch(Exception e1)
            {
                Program.SaveStop();
                data = invoice.Id.ToString() + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOT CHANGED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PDFOpen_Click(object sender, EventArgs e)
        {
            var itemIndx = listView2.SelectedIndices[0];

            String id = String.Format( "{0:00000000000}", invList[itemIndx].Id);


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
            }catch(Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StornoBT_Click(object sender, EventArgs e)
        {
            storno = true;

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Storno invoice selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            stornoInvoice = new Invoice();
            newInvoice = new Invoice();
            stornoPartsList.Clear();

            try
            {
                Program.LoadStart();

                var itemIndx = listView2.SelectedIndices[0];

                long stornoId = invList[itemIndx].Id;

                
                if (invList.Exists( x => x.Id == stornoId) && invList.Exists(x => x.Storno != 1))
                {
                    stornoInvoice = invList.First(x => x.Id == stornoId);

                    
                    newInvoice.GetNewInvoiceNumber();
                    if (qc.IfInvoiceExist(newInvoice.Id))
                    {
                        data = invoice.Id.ToString() + Environment.NewLine;
                        Result = "Invoice is already saved in DB, please make new one.";
                        lw.LogMe(function, usedQC, data, Result);
                        MessageBox.Show(Result, "NOTHING DONE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    stornoInvoice.Storno = 1;
                    stornoInvoice.Storno = 0;

                    stornoPartsList = newInvoice.StornoGetAllParts(stornoInvoice);

                    if ( qc.SetStornoInvoice0Or1(stornoInvoice) )
                    {
                        Properties.Settings.Default.StornoInvoiceNumber = String.Format("{0:00000000000}", stornoId);

                        TOTAL = stornoInvoice.Iznos;
                        TOTALTAX = vat == 0 ? 0 : TOTAL * vat;
                        TOTALTAXBASE = TOTAL - TOTALTAX;

                        invoice.Iznos = TOTAL;

                        try
                        {
                            String poruka = "";
                            if (qc.SaveInvoice(stornoPartsList, newInvoice, newInvoice.Storno))
                            {
                                if (Program.SaveDocumentsPDF) saveToPDF();

                                poruka = "Invoice " + stornoId + " is storned, new storno invoice made with number " + newInvoice.Id;
                            }
                            else
                            {
                                poruka = "NOTHING DONE";
                            }

                            Properties.Settings.Default.StornoInvoiceNumber = "";

                            Program.SaveStop();
                            MessageBox.Show(poruka);
                        }
                        catch (Exception e1)
                        {
                            Program.SaveStop();
                            data = newInvoice.Id.ToString() + Environment.NewLine;
                            Result = e1.Message;
                            lw.LogMe(function, usedQC, data, Result);
                            MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        finally
                        {
                            Properties.Settings.Default.StornoInvoiceNumber = "";
                            Program.LoadStop();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Already storned.");
                    }
                }
            }
            catch(Exception e1)
            {
                storno = false;
                Program.SaveStop();
                MessageBox.Show(e1.Message);
            }
            finally
            {
                storno = false;
                Program.SaveStop();
            }
            
        }
    }
}
