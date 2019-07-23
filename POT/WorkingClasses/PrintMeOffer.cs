using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    class PrintMeOffer
    {
        int partRows;
        int pageNbr;

        Company cmpR = new Company();
        QueryCommands qc = new QueryCommands();
        List<OfferParts> invPrtList = new List<OfferParts>();
        Offer off = new Offer();
        int storno;
        Boolean hrv = true;
        String brojPonude;
        decimal taxBase;
        decimal totalTax;

        public String datumIzrade = "";
        public String datumIspisa = "";
        public String izradioUser = "";
        public String izradioRegija = "";
        decimal eurDjelitelj = 0;

        int headerpointVer;
        int headerpointHor;
        int imgH = 0;  //75
        int imgW = 300 / Properties.Settings.Default.LogoSize; //150
        int moveBy = 12;
        int fontSizeR = 8;
        int fontSizeS = 10;
        double imgScale = 0;

        Image img = null;

        PrintMeOffer() { }

        public PrintMeOffer(List<OfferParts> _invPrtList, Offer _off, int _storno, Boolean _hrvJezik, decimal _taxBase, decimal _totalTax)
        {
            invPrtList = _invPrtList;
            off = _off;
            storno = _storno;
            hrv = !_hrvJezik;

            taxBase = _taxBase;
            totalTax = _totalTax;

            brojPonude = off.Id.ToString();

            if (brojPonude.Length < 9)
                brojPonude = "00" + String.Format("{0:0-000-00000}", long.Parse(brojPonude));
            else if (brojPonude.Length < 10)
                brojPonude = "0" + String.Format("{0:00-000-00000}", long.Parse(brojPonude));
            else
                brojPonude = String.Format("{0:000-000-00000}", long.Parse(brojPonude));

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
            if (!cmpR.GetCompanyInfoByID(off.CustomerID))
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
                eurDjelitelj = 1;
            }
            else
            {
                Properties.Settings.Default.LanguageStt = "eng";
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                oznakaValute = "€";
                eurDjelitelj = off.Eur;
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
            //margins.Bottom = margins.Bottom / 2;
            margins.Top = margins.Top / 2;

            headerpointVer = margins.Top;
            headerpointHor = bounds.Right - margins.Right;

            try
            {
                String workingStr = "";
                float measureStr = 0;
                float measureField = 0;

                using (img)
                {
                    int dodatak = 15;
                    int rowHeight = 20;
                    int total = bounds.Right - margins.Right - margins.Left;

                    int kraj = bounds.Right - margins.Right;
                    int polje = total / 30;
                    int pocetak = margins.Left - (polje * 1);
                    int rb = pocetak + (polje * 1);
                    int name = margins.Left + (polje * 10); //8
                    int code = margins.Left + (polje * 14); //4

                    int price = margins.Left + (polje * 17); //3
                    int workTime = margins.Left + (polje * 19); //3
                    int rebate = margins.Left + (polje * 21); //3
                    int amount = margins.Left + (polje * 23); //2
                    int rebatePrice = margins.Left + (polje * 27); //3

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
                        e.Graphics.DrawString(Properties.strings.WorkHour + ": " + String.Format("{0:N2}", cmpR.KN / eurDjelitelj) + " " + oznakaValute, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 7)));
                        razmakZaCustomera += 2;
                        e.Graphics.DrawString(Properties.strings.MinWorkTime + ": " + Properties.Settings.Default.ObracunskaJedinica.ToString() + " min", new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 8)));
                        razmakZaCustomera += 2;
                        e.Graphics.DrawString(Environment.NewLine, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, razmakZaCustomera + margins.Top + (moveBy * 9 / 2)));

                        headerpointVer = headerpointVer + (moveBy * 9);

                        //NAPOMENA
                        if (!off.Napomena.Equals(""))
                        {
                            if (hrv)
                                workingStr = "Napomena: ";
                            else
                                workingStr = "Note: ";

                            headerpointVer = headerpointVer + (moveBy * 2);

                            Font ft1 = getFont(8);
                            measureStr = e.Graphics.MeasureString(workingStr, ft1).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft1.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            measureField = workTime - (rb + (int)measureStr + 4);

                            workingStr = off.Napomena;
                            measureStr = e.Graphics.MeasureString(workingStr, ft1).Width;

                            int ii = 1;
                            int secondLine = 0;
                            int wsNAPOMENAw = (int)e.Graphics.MeasureString(Properties.strings.NOTE + ":", ft1).Width;
                            String ws1 = "";

                            while (measureStr > measureField)
                            {
                                ws1 = workingStr;
                                int ws1Lenght = (int)e.Graphics.MeasureString(ws1, ft1).Width;
                                while (ws1Lenght > measureField)
                                {
                                    ws1 = ws1.Substring(0, ws1.Length - 1);
                                    ws1Lenght = (int)e.Graphics.MeasureString(ws1, ft1).Width;
                                }

                                if (headerpointVer + moveBy * ii < margins.Top + imgH + (moveBy * 7) + 75)
                                {
                                    e.Graphics.DrawString(ws1, new Font("Calibri light", ft1.Size, FontStyle.Regular), Brushes.Black, new Point(rb + wsNAPOMENAw + 4, headerpointVer + moveBy * ii));
                                    ii++;
                                    secondLine = wsNAPOMENAw;
                                    workingStr = workingStr.Substring(ws1.Length);
                                    measureStr = e.Graphics.MeasureString(workingStr, ft1).Width;
                                    measureField = workTime - rb - (secondLine);
                                }
                            }
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft1.Size, FontStyle.Regular), Brushes.Black, new Point(rb + wsNAPOMENAw + 4, headerpointVer + moveBy * ii));
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

                        String umjestoPlatite;

                        if (hrv)
                        {
                            workingStr = "Pounda br.";
                            umjestoPlatite = "Ponuda vrijedi";
                        }
                        else
                        {
                            workingStr = "Offer nbr.";
                            umjestoPlatite = "Offer is valid";
                        }

                        e.Graphics.DrawString(workingStr + "  " + brojPonude, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, headerpointVer + (moveBy * 2)));

                        workingStr = umjestoPlatite;
                        measureStr = e.Graphics.MeasureString(workingStr, getFont(10)).Width;
                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Left - (int)measureStr, headerpointVer + moveBy - 4));

                        workingStr = off.Valuta.ToString() + " " + Properties.strings.Days;
                        measureStr = e.Graphics.MeasureString(workingStr, getFont(10)).Width;
                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Left - (int)measureStr, headerpointVer + (moveBy * 2)));

                        headerpointVer = headerpointVer + (moveBy * 6);
                    }
                    else
                    {
                        imgW = imgW / 2; //80
                        imgH = (int)((double)imgW / imgScale); //75 //40
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);
                        headerpointVer = bounds.Top + margins.Top + imgH + 50;
                    }

                    Font fnt = getFont(fontSizeR);

                    if (partRows < invPrtList.Count)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.DeepSkyBlue), rb, headerpointVer, total, rowHeight + dodatak);
                        e.Graphics.DrawRectangle(new Pen(Brushes.Black), rb, headerpointVer, total, rowHeight + dodatak);

                        float[] dashValues = { 1, 1, 1, 1 };
                        Pen blackPen = new Pen(Color.Black, 1);
                        //blackPen.DashPattern = dashValues;
                        blackPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                        //GRID
                        e.Graphics.DrawLine(blackPen, new Point(name, headerpointVer), new Point(name, headerpointVer + rowHeight + dodatak));
                        e.Graphics.DrawLine(blackPen, new Point(code, headerpointVer), new Point(code, headerpointVer + rowHeight + dodatak));
                        e.Graphics.DrawLine(blackPen, new Point(price, headerpointVer), new Point(price, headerpointVer + rowHeight + dodatak));

                        e.Graphics.DrawLine(blackPen, new Point(workTime, headerpointVer), new Point(workTime, headerpointVer + rowHeight + dodatak));
                        e.Graphics.DrawLine(blackPen, new Point(rebate, headerpointVer), new Point(rebate, headerpointVer + rowHeight + dodatak));
                        e.Graphics.DrawLine(blackPen, new Point(amount, headerpointVer), new Point(amount, headerpointVer + rowHeight + dodatak));
                        e.Graphics.DrawLine(blackPen, new Point(rebatePrice, headerpointVer), new Point(rebatePrice, headerpointVer + rowHeight + dodatak)); //new Pen(Brushes.Black)

                        workingStr = "RB";
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, polje);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = polje;
                        e.Graphics.DrawString(workingStr, getFont(6), Brushes.Black, new Point(pocetak + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight));

                        workingStr = Properties.strings.NAME;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, name - rb);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = name - rb;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rb + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                        workingStr = Properties.strings.CODE;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - name);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = code - name;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(name + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                        workingStr = Properties.strings.PRICE;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, price - code);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = price - code;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(code + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                        /////////////////////
                        workingStr = Properties.strings.WORKTIME1;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, workTime - price);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = workTime - price;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(price + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                        workingStr = Properties.strings.WORKTIME2;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, workTime - price);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = workTime - price;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(price + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight));
                        /////////////////////

                        workingStr = Properties.strings.REBATE;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, rebate - workTime);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = rebate - workTime;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(workTime + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                        workingStr = Properties.strings.QUA + ".";
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, amount - rebate);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = amount - rebate;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rebate + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));

                        /////////////////////
                        workingStr = Properties.strings.REBATEPRICE1;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, rebatePrice - amount);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = rebatePrice - amount;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(amount + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                        workingStr = Properties.strings.REBATEPRICE2;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, rebatePrice - amount);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = rebatePrice - amount;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(amount + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight));
                        /////////////////////

                        workingStr = Properties.strings.TOTAL;
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, kraj - rebatePrice);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = kraj - rebatePrice;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rebatePrice + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 2));
                    }

                    headerpointVer += moveBy;

                    for (; partRows < invPrtList.Count; partRows++)
                    {

                        if (headerpointVer + (moveBy * 4) + 20 > bounds.Bottom - margins.Bottom - 20)
                        {
                            e.HasMorePages = true;
                            break;
                        }

                        String partCode = String.Format("{0:00}", long.Parse(Properties.Settings.Default.CmpCode)) + String.Format("{0:00}", cmpR.Code) + Decoder.GetFullPartCodeStr(invPrtList[partRows].PartCode);
                        PartSifrarnik tmpPart = qc.PartInfoByFullCodeSifrarnik(invPrtList[partRows].PartCode);

                        headerpointVer = headerpointVer + (moveBy * 2);

                        workingStr = (partRows + 1).ToString();//tu
                        measureField = polje;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(pocetak + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = tmpPart.FullName;//tu partRows
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = name - rb;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rb, headerpointVer + moveBy));

                        workingStr = partCode;//tu partRows
                        measureField = code - name;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(name + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = String.Format("{0:N2}", decimal.Parse(invPrtList[partRows].IznosPart) / eurDjelitelj) + " " + oznakaValute;
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
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(workTime + ((int)measureField - (int)measureStr), headerpointVer + moveBy));

                        workingStr = invPrtList[partRows].Kolicina.ToString();
                        measureField = amount - rebate;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rebate + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = String.Format("{0:N2}", decimal.Parse(invPrtList[partRows].IznosRabat) / eurDjelitelj) + " " + oznakaValute;
                        measureField = rebatePrice - amount;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(rebatePrice - (int)measureStr, headerpointVer + moveBy));

                        workingStr = String.Format("{0:N2}", decimal.Parse(invPrtList[partRows].IznosTotal) / eurDjelitelj) + " " + oznakaValute;
                        measureField = kraj - rebatePrice;
                        fnt = fitFontSize(e, workingStr, fontSizeR, measureField); ; //tu
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(kraj - (int)measureStr, headerpointVer + moveBy));

                        if (partRows >= invPrtList.Count - 1)
                        {
                            headerpointVer = headerpointVer + (moveBy * 2);
                            e.Graphics.DrawLine(new Pen(Brushes.Black), rb, headerpointVer + moveBy, kraj, headerpointVer + moveBy);
                        }

                    }

                    Properties.Settings.Default.partRows = partRows;

                    if (invPrtList.Count > partRows)
                    {
                        e.HasMorePages = true;
                    }
                    else
                    {
                        Font ft = getFont(10); //tu

                        if (headerpointVer + 120 > bounds.Bottom - margins.Bottom - 20)
                        {
                            e.HasMorePages = true;
                        }
                        else
                        {
                            headerpointVer = headerpointVer + (moveBy * 2);
                            workingStr = Properties.strings.TaxBase + ":";
                            measureField = rebatePrice - workTime;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, ft, Brushes.Black, new Point(amount + ((rebatePrice - amount) / 2) - (int)measureStr, headerpointVer + moveBy));

                            workingStr = String.Format("{0:N2}", taxBase / eurDjelitelj) + " " + oznakaValute;
                            measureField = total - rebatePrice;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, ft, Brushes.Black, new Point(kraj - (int)measureStr, headerpointVer + moveBy));


                            headerpointVer = headerpointVer + (moveBy * 2);
                            workingStr = Properties.strings.TAX + "(" + (hrv ? Properties.Settings.Default.TAX1.ToString() : Properties.Settings.Default.TAX2.ToString()) + "%):";
                            measureField = rebatePrice - workTime;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, ft, Brushes.Black, new Point(amount + ((rebatePrice - amount) / 2) - (int)measureStr, headerpointVer + moveBy));

                            workingStr = String.Format("{0:N2}", totalTax / eurDjelitelj) + " " + oznakaValute;
                            measureField = total - rebatePrice;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, ft, Brushes.Black, new Point(kraj - (int)measureStr, headerpointVer + moveBy));


                            headerpointVer = headerpointVer + (moveBy * 2);
                            workingStr = Properties.strings.TotalSum + ":";
                            measureField = rebatePrice - workTime;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(amount + ((rebatePrice - amount) / 2) - (int)measureStr, headerpointVer + moveBy));

                            workingStr = String.Format("{0:N2}", off.Iznos / eurDjelitelj) + " " + oznakaValute;
                            measureField = total - rebatePrice;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(kraj - (int)measureStr, headerpointVer + moveBy));

                            if (!hrv)
                            {
                                headerpointVer = headerpointVer + (moveBy * 2);
                                workingStr = Properties.strings.TotalSum + ":";
                                measureField = rebatePrice - workTime;
                                measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                                e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(amount + ((rebatePrice - amount) / 2) - (int)measureStr, headerpointVer + moveBy));

                                workingStr = String.Format("{0:N2}", off.Iznos) + " kn";
                                measureField = total - rebatePrice;
                                measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                                e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(kraj - (int)measureStr, headerpointVer + moveBy));
                            }

                            headerpointVer = headerpointVer - (moveBy * 2);
                        }

                        ///////////////////////////////////////////////////////

                        if (headerpointVer + 200 > bounds.Bottom - margins.Bottom - 20 || e.HasMorePages)
                        {
                            e.HasMorePages = true;
                        }
                        else
                        {
                            ft = getFont(8);

                            headerpointVer = headerpointVer + (moveBy * 3);
                            workingStr = Properties.strings.DateTime + ":";
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            workingStr = off.DatumIzdano + " " + off.VrijemeIzdano;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rb + (int)measureStr + 25, headerpointVer + moveBy));


                            headerpointVer = headerpointVer + moveBy + 4;
                            workingStr = Properties.strings.PaymentForm + ":";
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            workingStr = off.NacinPlacanja;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rb + (int)measureStr + 25, headerpointVer + moveBy));


                            headerpointVer = headerpointVer + moveBy + 4;
                            workingStr = Properties.strings.OfferValid + ":";
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            workingStr = off.Valuta.ToString() + " " + Properties.strings.Days;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rb + (int)measureStr + 25, headerpointVer + moveBy));


                            headerpointVer = headerpointVer + moveBy + 4;
                            workingStr = Properties.strings.ExcRate + ":";
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            workingStr = off.Eur.ToString() + " kn " + Properties.strings.OnDay + " " + off.DanTecaja;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rb + (int)measureStr + 25, headerpointVer + moveBy));


                            headerpointVer = headerpointVer + moveBy + 4;
                            workingStr = Properties.strings.Operater + ":";
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            workingStr = WorkingUser.Name[0].ToString() + WorkingUser.Surename[0].ToString() + WorkingUser.UserID.ToString();
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rb + (int)measureStr + 25, headerpointVer + moveBy));


                            String workingStrOsoba = Properties.strings.ResPersone;
                            float measureStrOsoba = e.Graphics.MeasureString(workingStrOsoba, ft).Width;
                            e.Graphics.DrawString(workingStrOsoba, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rebatePrice - ((int)measureStrOsoba / 2), headerpointVer + moveBy));

                            workingStrOsoba = Properties.Settings.Default.odgovornaOsoba;
                            measureStrOsoba = e.Graphics.MeasureString(workingStrOsoba, ft).Width;
                            e.Graphics.DrawString(workingStrOsoba, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rebatePrice - ((int)measureStrOsoba / 2), headerpointVer + (moveBy * 2)));


                            headerpointVer = headerpointVer + moveBy + 4;
                            workingStr = Properties.strings.OfferConn + ":";
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Bold), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            workingStr = off.RacunID.ToString();
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size, FontStyle.Regular), Brushes.Black, new Point(rb + (int)measureStr + 25, headerpointVer + moveBy));

                            headerpointVer = headerpointVer + (moveBy * 4);
                            if (hrv)
                                workingStr = Properties.Settings.Default.extraLine1HRTB;
                            else
                                workingStr = Properties.Settings.Default.extraLine1ENGTB;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size - 2, FontStyle.Regular), Brushes.Black, new Point(rb, headerpointVer + moveBy));

                            headerpointVer = headerpointVer + moveBy + 4;
                            if (hrv)
                                workingStr = Properties.Settings.Default.extraLine2HRTB;
                            else
                                workingStr = Properties.Settings.Default.extraLine2ENGTB;
                            measureStr = e.Graphics.MeasureString(workingStr, ft).Width;
                            e.Graphics.DrawString(workingStr, new Font("Calibri light", ft.Size - 2, FontStyle.Regular), Brushes.Black, new Point(rb, headerpointVer + moveBy));
                        }
                    }

                    e.Graphics.DrawString(Properties.strings.Page + " : " + pageNbr, getFont(8), Brushes.Black, new Point(margins.Left, bounds.Bottom - margins.Bottom ));

                    if (e.HasMorePages)
                    {
                        Properties.Settings.Default.pageNbr = pageNbr = pageNbr + 1;
                    }
                    else
                    {
                        Properties.Settings.Default.pageNbr = 1;
                        Properties.Settings.Default.partRows = partRows = 0;
                    }


                    if (hrv)
                        workingStr = Properties.Settings.Default.thx1HRTB;
                    else
                        workingStr = Properties.Settings.Default.thx1ENGTB;

                    Font fntT = new Font("Ink Free", 14, FontStyle.Regular);
                    measureStr = e.Graphics.MeasureString(workingStr, fntT).Width;
                    measureField = bounds.Right - margins.Right - margins.Left;
                    e.Graphics.DrawString(workingStr, fntT, Brushes.DeepSkyBlue, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), bounds.Bottom - margins.Bottom - 20));

                    fnt = fitFontSize(e, (partRows + 1).ToString(), fontSizeR, code - pocetak);

                    workingStr = Properties.Settings.Default.CmpWWW;
                    fnt = fitFontSize(e, workingStr, fontSizeR, code - pocetak);
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
