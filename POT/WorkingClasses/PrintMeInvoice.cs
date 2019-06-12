using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    class PrintMeInvoice
    {
        int partRows;
        int pageNbr;
        static int rbInner = 1;
        static int partRowsInner = 0;
        static Boolean signatureInitiated = false;
        List<Part> partListPrint = new List<Part>();
        
        Company cmpR = new Company();
        QueryCommands qc = new QueryCommands();
        List<InvoiceParts> invPrtList = new List<InvoiceParts>();
        Invoice inv = new Invoice();
        int storno;
        Boolean hrv = true;
        String brojRacuna;

        public String datumIzrade = "";
        public String datumIspisa = "";
        public String izradioUser = "";
        public String izradioRegija = "";

        int headerpointVer;
        int headerpointHor;
        int imgH = 0;  //75
        int imgW = 300 / Properties.Settings.Default.LogoSize; //150
        int moveBy = 12;
        int fontSizeR = 8;
        int fontSizeS = 10;
        double imgScale = 0;

        Image img = null;

        PrintMeInvoice() { }

        public PrintMeInvoice(List<InvoiceParts> _invPrtList, Invoice _inv, int _storno, Boolean _hrvJezik)
        {
            invPrtList = _invPrtList;
            inv = _inv;
            storno = _storno;
            hrv = _hrvJezik;
            
            brojRacuna = inv.Id.ToString();

            if (brojRacuna.Length < 9)
                brojRacuna = "00" + String.Format("{0:0-000-00000}", long.Parse(brojRacuna));
            else if (brojRacuna.Length < 10)
                brojRacuna = "0" + String.Format("{0:00-000-00000}", long.Parse(brojRacuna));
            else
                brojRacuna = String.Format("{0:000-000-00000}", long.Parse(brojRacuna));

            CLogo logoImage = new CLogo();
            img = logoImage.GetImage();

            imgScale = (double)img.Width / img.Height;

            imgH = (int)((double)imgW / imgScale);
        }

        public void Print(PrintPageEventArgs e)
        {
            if (!Properties.Settings.Default.printingSN)
            {
                printParts(e);
                return;
            }

            //Properties.Settings.Default.pageNbr = pageNbr = 1;
            //Properties.Settings.Default.partRows = partRows = 0;
        }

        void printParts(PrintPageEventArgs e)
        {
            if (!cmpR.GetCompanyInfoByID(inv.CustomerID))
            {
                MessageBox.Show("I cant find company.");
                return;
            }

            String oznakaValute = "";
            if (hrv)
            {
                Properties.Settings.Default.LanguageStt = "hrv";
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("hr-HR");
                oznakaValute = "kn";
            }
            else
            {
                Properties.Settings.Default.LanguageStt = "eng";
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                oznakaValute = "€";
            }

            partRows = Properties.Settings.Default.partRows;
            pageNbr = Properties.Settings.Default.pageNbr;

            if (datumIzrade.Equals(""))
                datumIzrade = DateTime.Now.ToString("dd.MM.yy.");
            if (datumIspisa.Equals(""))
                datumIspisa = DateTime.Now.ToString("dd.MM.yy.");
            if (izradioUser.Equals(""))
                izradioUser = WorkingUser.UserID.ToString();
            if (izradioRegija.Equals(""))
                izradioRegija = WorkingUser.RegionID.ToString();

            e.HasMorePages = false;

            PageSettings page = GetPrinterPageInfo();

            RectangleF area = page.PrintableArea;
            Rectangle bounds = page.Bounds;
            Margins margins = page.Margins;

            //Podesavanje pocetka ispisa za prvi list od vrha (default = 100)
            margins.Bottom = margins.Bottom / 2;
            margins.Top = margins.Top / 2;

            headerpointVer = margins.Top;
            headerpointHor = bounds.Right - margins.Right;
            int napomenaHeight = bounds.Bottom - margins.Bottom - moveBy * 5;

            try
            {
                String workingStr = "";
                float measureStr = 0;
                float measureField = 0;

                using (img)
                {
                    //var groupedPartsList = partListPrint.GroupBy(c => c.PartialCode).Select(grp => grp.ToList()).ToList();
                    if (pageNbr == 1)
                    {
                        //Sender/Receiver Company Info
                        int razmakZaCustomera = 0;
                        e.Graphics.DrawString(Properties.strings.customer + ": ", new Font("Calibri light", fontSizeS - 1, FontStyle.Underline | FontStyle.Italic), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy / 2)));
                        e.Graphics.DrawString(cmpR.Name, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 2)));
                        razmakZaCustomera += 6;
                        e.Graphics.DrawString(cmpR.Address, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 3)));
                        razmakZaCustomera += 2;
                        e.Graphics.DrawString(cmpR.Country + " - " + cmpR.City + ", " + cmpR.PB, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 4)));
                        razmakZaCustomera += 4;
                        e.Graphics.DrawString(Properties.strings.VAT + ": " + cmpR.OIB, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 5)));
                        razmakZaCustomera += 2;
                        e.Graphics.DrawString(Properties.strings.SWIFT + ": " + cmpR.BIC, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 6)));
                        razmakZaCustomera += 4;
                        e.Graphics.DrawString(Properties.strings.WorkHour + ": " + cmpR.KN + " " + oznakaValute, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 7)));
                        razmakZaCustomera += 2;
                        e.Graphics.DrawString(Properties.strings.MinWorkTime + ": " + Properties.Settings.Default.RadniSat, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 8)));
                        razmakZaCustomera += 2;
                        e.Graphics.DrawString(Environment.NewLine, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 9 / 2)));
                        
                        if (!inv.Napomena.Equals(""))
                        {
                            if (hrv)
                                workingStr = "Napomena: ";
                            else
                                workingStr = "Note: ";
                            
                            measureStr = e.Graphics.MeasureString(workingStr, getFont(10)).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 11)));
                            e.Graphics.DrawString(inv.Napomena, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left + (int)measureStr + 10, margins.Top + (moveBy * 11)));
                        }

                        //MyCompany Info
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);

                        String MainCmpPrintAddress = Properties.Settings.Default.CmpAddress + ", " + Properties.Settings.Default.CmpCountry + " - " + Properties.Settings.Default.CmpPB + " " + Properties.Settings.Default.CmpCity;

                        e.Graphics.DrawString(Properties.Settings.Default.CmpName, new Font("Calibri light", fontSizeR, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy)));
                        e.Graphics.DrawString(MainCmpPrintAddress, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 2)));
                        e.Graphics.DrawString("MB: " + Properties.Settings.Default.CmpMB, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 3)));
                        e.Graphics.DrawString(Properties.strings.VAT + ": " + Properties.Settings.Default.CmpVAT, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 4)));
                        e.Graphics.DrawString("Tel: " + Properties.Settings.Default.CmpPhone, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 5)));
                        e.Graphics.DrawString("IBAN: " + Properties.Settings.Default.CmpIBAN, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 6)));
                        e.Graphics.DrawString(Properties.strings.SWIFT + ": " + Properties.Settings.Default.CmpSWIFT, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 7)));

                        headerpointVer = margins.Top + imgH + (moveBy * 7) + 100;
                        headerpointHor = bounds.Right - margins.Right - imgW;

                        if (hrv)
                            workingStr = "Račun br.";
                        else
                            workingStr = "Invoice nbr.";

                        //measureStr = e.Graphics.MeasureString(workingStr, new Font("Calibri light", fontSizeS + 5, FontStyle.Bold)).Width;
                        //e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS + 2, FontStyle.Bold), Brushes.Black, new Point((bounds.Right / 2) - ((int)measureStr / 2), headerpointVer));

                        //headerpointVer += (moveBy * 2);
                        //e.Graphics.DrawString(Properties.strings.DocumentNbr + "  " + brojRacuna, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, headerpointVer + (moveBy * 2)));
                        e.Graphics.DrawString(workingStr + "  " + brojRacuna, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, headerpointVer + (moveBy * 2)));

                        workingStr = Properties.strings.ACCOUNTUSE;
                        measureStr = e.Graphics.MeasureString(workingStr, getFont(10)).Width;
                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Left - (int)measureStr, headerpointVer + moveBy - 4));

                        workingStr = Properties.Settings.Default.CmpIBAN;
                        measureStr = e.Graphics.MeasureString(workingStr, getFont(10)).Width;
                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Left - (int)measureStr, headerpointVer + (moveBy * 2)));


                        //(((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy)
                        //measureStr = e.Graphics.MeasureString(workingStr, getFont(8)).Width;
                        //measureField = amount - rebate;
                        //e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(rebate + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight));

                        headerpointVer = headerpointVer + (moveBy * 4);
                    }
                    else
                    {
                        imgW = imgW / 2; //80
                        imgH = (int)((double)imgW / imgScale); //75 //40
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);
                        headerpointVer = bounds.Top + margins.Top + imgH + 50;
                    }

                    //int rb = margins.Left + 40;
                    //int name = margins.Left + 200;
                    //int code = margins.Left + 280;

                    //int price = margins.Left + 340;
                    //int workTime = margins.Left + 400;
                    //int rebate = margins.Left + 460;
                    //int amount = margins.Left + 500;
                    //int rebatePrice = margins.Left + 560;
                    int dodatak = 15;
                    int rowHeight = 20;

                    //e.Graphics.DrawRectangle(new Pen(Brushes.Black), margins.Left, headerpointVer, bounds.Right - margins.Right - margins.Left, rowHeight + dodatak);
                    e.Graphics.FillRectangle(new SolidBrush(Color.AliceBlue), margins.Left + 1, headerpointVer + 1, bounds.Right - margins.Right - margins.Left - 2, 33);
                    e.Graphics.DrawLine(new Pen(Brushes.Black), margins.Left, headerpointVer + rowHeight + dodatak, bounds.Right - margins.Right, headerpointVer + rowHeight + dodatak);
                    
                    int total = bounds.Right - margins.Right - margins.Left;
                    int polje = total / 30;
                    int rb = margins.Left + (polje * 1);
                    int name = margins.Left + (polje * 12); //8
                    int code = margins.Left + (polje * 16); //4

                    int price = margins.Left + (polje * 19); //3
                    int workTime = margins.Left + (polje * 21); //3
                    int rebate = margins.Left + (polje * 23); //3
                    int amount = margins.Left + (polje * 25); //2
                    int rebatePrice = margins.Left + (polje * 28); //3
                    //int totalPart = margins.Left + 620;

                    int mes = name + total / 9;

                    //GRID
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(rb, headerpointVer), new Point(rb, headerpointVer + rowHeight + dodatak));
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(name, headerpointVer), new Point(name, headerpointVer + rowHeight + dodatak));
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(code, headerpointVer), new Point(code, headerpointVer + rowHeight + dodatak));
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(price, headerpointVer), new Point(price, headerpointVer + rowHeight + dodatak));

                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(workTime, headerpointVer), new Point(workTime, headerpointVer + rowHeight + dodatak));
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(rebate, headerpointVer), new Point(rebate, headerpointVer + rowHeight + dodatak));
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(amount, headerpointVer), new Point(amount, headerpointVer + rowHeight + dodatak));
                    //e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(rebatePrice, headerpointVer), new Point(rebatePrice, headerpointVer + rowHeight + dodatak));

                    Font fnt = getFont(fontSizeR);

                    workingStr = "RB";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, rb - margins.Left);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = rb - margins.Left;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    workingStr = Properties.strings.NAME;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, name - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = name - rb;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(rb + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    workingStr = Properties.strings.CODE;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - name);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = code - name;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(name + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    workingStr = Properties.strings.PRICE;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, price - code);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = price - code;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(code + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    /////////////////////
                    workingStr = Properties.strings.WORKTIME1;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, workTime - price);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = workTime - price;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(price + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    workingStr = Properties.strings.WORKTIME2;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, workTime - price);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = workTime - price;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(price + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight));
                    /////////////////////

                    workingStr = Properties.strings.REBATE;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, rebate - workTime);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = rebate - workTime;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(workTime + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    workingStr = Properties.strings.QUA + ".";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, amount - rebate);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = amount - rebate;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(rebate + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    /////////////////////
                    workingStr = Properties.strings.REBATEPRICE1;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, rebatePrice - amount);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = rebatePrice - amount;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(amount + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    workingStr = Properties.strings.REBATEPRICE2;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, rebatePrice - amount);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = rebatePrice - amount;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(amount + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight));
                    /////////////////////

                    workingStr = Properties.strings.TOTAL;
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, total + margins.Left - rebatePrice);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = total + margins.Left - rebatePrice;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(rebatePrice + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                    //var groupedPartsList = partListPrint.GroupBy(c => c.PartialCode).Select(grp => grp.ToList()).ToList();
                    //for (; partRows < 35; partRows++)
                    headerpointVer += moveBy;

                    for (; partRows < invPrtList.Count; partRows++)
                    {

                        if (headerpointVer + (moveBy * 4) + 20 > napomenaHeight)
                        {
                            e.HasMorePages = true;
                            break;
                        }
                        
                        String partCode = String.Format("{0:00}", long.Parse(Properties.Settings.Default.CmpCode)) + String.Format("{0:00}", cmpR.Code) + Decoder.GetFullPartCodeStr(invPrtList[partRows].PartCode);
                        PartSifrarnik tmpPart = qc.PartInfoByFullCodeSifrarnik(invPrtList[partRows].PartCode);

                        headerpointVer = headerpointVer + (moveBy * 2);

                        workingStr = (partRows + 1).ToString();//tu
                        measureField = rb - margins.Left;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = tmpPart.FullName;//tu partRows
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = name - rb;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(rb, headerpointVer + moveBy));
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rb, headerpointVer + moveBy));

                        workingStr = partCode;//tu partRows
                        measureField = code - name;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(name + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = invPrtList[partRows].IznosPart + " " + oznakaValute;
                        measureField = price - code;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(price - (int)measureStr, headerpointVer + moveBy));

                        workingStr = invPrtList[partRows].VrijemeRada;
                        measureField = workTime - price;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(price + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = String.Format("{0:N2}", invPrtList[partRows].Rabat) + " %";
                        measureField = rebate - workTime;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(workTime + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = invPrtList[partRows].Kolicina.ToString();
                        measureField = amount - rebate;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rebate + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = invPrtList[partRows].IznosRabat + " " + oznakaValute;
                        measureField = rebatePrice - amount;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rebatePrice - (int)measureStr, headerpointVer + moveBy));

                        workingStr = invPrtList[partRows].IznosTotal + " " + oznakaValute;
                        measureField = total + margins.Left - rebatePrice;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField); ; //tu
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(total + margins.Left - (int)measureStr, headerpointVer + moveBy));
                    }

                    Properties.Settings.Default.partRows = partRows;

                    if (invPrtList.Count > partRows)
                        e.HasMorePages = true;
                    else
                        e.HasMorePages = false;

                    //NAPOMENA
                    if (pageNbr == 1)
                    {
                        napomenaHeight = 0;
                        workingStr = Properties.strings.NOTE + ":  " + inv.Napomena;//Bruno TEST
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = bounds.Right - margins.Right - margins.Left;

                        int ii = 5;
                        int secondLine = 0;
                        int wsNAPOMENAw = (int)e.Graphics.MeasureString(Properties.strings.NOTE + ":", fnt).Width;
                        String ws1 = "";

                        while (measureStr > measureField)
                        {
                            ws1 = workingStr;
                            int ws1Lenght = (int)e.Graphics.MeasureString(ws1, fnt).Width;
                            while (ws1Lenght > measureField)
                            {
                                ws1 = ws1.Substring(0, ws1.Length - 1);
                                ws1Lenght = (int)e.Graphics.MeasureString(ws1, fnt).Width;
                            }

                            e.Graphics.DrawString(ws1, fnt, Brushes.Black, new Point(margins.Left + secondLine, bounds.Bottom - margins.Bottom - moveBy * ii--));
                            secondLine = wsNAPOMENAw;
                            workingStr = workingStr.Substring(ws1.Length);
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            measureField = bounds.Right - margins.Right - margins.Left - wsNAPOMENAw;
                        }
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + secondLine, bounds.Bottom - margins.Bottom - moveBy * ii--));
                    }

                    e.Graphics.DrawString(Properties.strings.Page + " : " + pageNbr, getFont(8), Brushes.Black, new Point(margins.Left, bounds.Bottom - margins.Bottom));

                    if( e.HasMorePages)
                    {
                        Properties.Settings.Default.pageNbr = pageNbr = pageNbr + 1;
                    }
                    else
                    {
                        Properties.Settings.Default.pageNbr = 1;
                        Properties.Settings.Default.partRows = partRows = 0;
                    }

                    fnt = fitFontSize(e, (partRows + 1).ToString(), fontSizeR, code - rb);
                    
                    workingStr = "Hvala na ukazanom povjerenju";
                    Font fntT = new Font("Ink Free", 14, FontStyle.Regular);
                    measureStr = e.Graphics.MeasureString(workingStr, fntT).Width;
                    measureField = bounds.Right - margins.Right - margins.Left;
                    e.Graphics.DrawString(workingStr, fntT, Brushes.DodgerBlue, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), bounds.Bottom - margins.Bottom));

                    workingStr = Properties.Settings.Default.CmpWWW;
                    fnt = fitFontSize(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(bounds.Right - margins.Right - (int)measureStr, bounds.Bottom - margins.Bottom));
                }

                return;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        public static PageSettings GetPrinterPageInfo(String printerName)
        {
            PrinterSettings settings;

            // If printer name is not set, look for default printer
            if (String.IsNullOrEmpty(printerName))
            {
                foreach (var printer in PrinterSettings.InstalledPrinters)
                {
                    settings = new PrinterSettings();

                    settings.PrinterName = printer.ToString();

                    if (settings.IsDefaultPrinter)
                        return settings.DefaultPageSettings;
                }

                return null; // <- No default printer  
            }

            // printer by its name 
            settings = new PrinterSettings();

            settings.PrinterName = printerName;

            return settings.DefaultPageSettings;
        }

        // Default printer default page info
        public static PageSettings GetPrinterPageInfo()
        {
            return GetPrinterPageInfo(null);
        }

        private Font getFont(int _FontSize)
        {
            return new Font("Calibri light", _FontSize, FontStyle.Regular);
        }

        private Font fitFontSize(PrintPageEventArgs e, String _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font("Calibri light", velFont, FontStyle.Regular);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font("Calibri light", velFont, FontStyle.Regular); ;

        }

        private Font fitFontSizeBold(PrintPageEventArgs e, String _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font("Calibri light", velFont, FontStyle.Bold);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font("Calibri light", _FontSize, FontStyle.Bold); ;

        }

        private Font fitFontSizeItalic(PrintPageEventArgs e, String _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font("Calibri light", velFont, FontStyle.Italic);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font("Calibri light", _FontSize, FontStyle.Italic); ;

        }

        private Font fitFontSizeIDAutomation(PrintPageEventArgs e, String _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font("IDAutomationHC39M", velFont, FontStyle.Regular);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font("IDAutomationHC39M", _FontSize, FontStyle.Regular);
        }
    }
}
