using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    class PrintMe
    {
        int partRows;
        int pageNbr;
        String PrimkaNumber;
        List<Part> partListPrint = new List<Part>();
        Company cmpS = new Company();
        Company cmpR = new Company();
        List<String> sifrarnikArr = new List<string>();
        String napomenaPRIMPrint;
        String documentName;
        String recipientSender;
        Boolean signature = false;

        int headerpointVer;
        int headerpointHor;
        int imgH = 75;
        int imgW = 150;
        int moveBy = 12;
        int fontSizeR = 8;
        int fontSizeS = 10;

        Image img = null;

        PrintMe() { }

        public PrintMe(Company _cmpS, Company _cmpR, List<String> _sifrarnikArr, List<Part> _partListPrint, String _PrimkaNumber, String _napomenaPRIMPrint, String _DocumentName, String _recipientSender, Boolean _Signature)
        {
            cmpR = _cmpR;
            cmpS = _cmpS;
            sifrarnikArr = _sifrarnikArr;
            partListPrint = _partListPrint;
            PrimkaNumber = _PrimkaNumber;
            napomenaPRIMPrint = _napomenaPRIMPrint;
            documentName = _DocumentName.ToUpper();
            recipientSender = _recipientSender.ToUpper();
            signature = _Signature;

            CLogo logoImage = new CLogo();
            img = logoImage.GetImage();
        }

        public void Print(PrintPageEventArgs e)
        {
            if (!Properties.Settings.Default.printingSN)
            {
                printParts(e);
                return;
            }
            else
            {
                printSN(e);
            }

            if (!Properties.Settings.Default.printingSN)
            {
                Properties.Settings.Default.pageNbr = pageNbr = 1;
                Properties.Settings.Default.partRows = partRows = 0;
            }
        }

        void printParts(PrintPageEventArgs e)
        {
            partRows = Properties.Settings.Default.partRows;
            pageNbr = Properties.Settings.Default.pageNbr;
            
            

            e.HasMorePages = false;

            PageSettings page = GetPrinterPageInfo();

            RectangleF area = page.PrintableArea;
            Rectangle bounds = page.Bounds;
            Margins margins = page.Margins;

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
                        //Sendere Company Info
                        e.Graphics.DrawString(recipientSender + ": ", new Font("Calibri light", fontSizeS - 1, FontStyle.Underline), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy / 2)));
                        e.Graphics.DrawString(cmpS.Name, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 2)));
                        e.Graphics.DrawString(cmpS.Address, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 3)));
                        e.Graphics.DrawString(cmpS.Country + " - " + cmpS.City + ", " + cmpS.PB, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 4)));
                        e.Graphics.DrawString(Environment.NewLine, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 5 / 2)));
                        e.Graphics.DrawString("Date: " + DateTime.Now.ToString("dd.MM.yyyy."), new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 6)));
                        e.Graphics.DrawString("Time: " + DateTime.Now.ToString("hh:mm"), new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 7)));
                        e.Graphics.DrawString("UserID: " + WorkingUser.UserID, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 8)));
                        e.Graphics.DrawString("RegionID: " + WorkingUser.RegionID, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 9)));
                        e.Graphics.DrawString("Made by: " + WorkingUser.Name + " " + WorkingUser.Surename, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 10)));

                        //MyCompany Info
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);

                        e.Graphics.DrawString(Properties.Settings.Default.CmpName, new Font("Calibri light", fontSizeR, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy)));
                        e.Graphics.DrawString(Properties.Settings.Default.CmpAddress, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 2)));
                        e.Graphics.DrawString("MB: " + Properties.Settings.Default.CmpMB, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 3)));
                        e.Graphics.DrawString("VAT: " + Properties.Settings.Default.CmpVAT, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 4)));
                        e.Graphics.DrawString("Tel: " + Properties.Settings.Default.CmpPhone, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 5)));
                        e.Graphics.DrawString("IBAN: " + Properties.Settings.Default.CmpIBAN, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 6)));
                        e.Graphics.DrawString("SWIFT: " + Properties.Settings.Default.CmpSWIFT, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 7)));

                        headerpointVer = margins.Top + imgH + (moveBy * 7) + 50;
                        headerpointHor = bounds.Right - margins.Right - imgW;

                        workingStr = documentName;
                        measureStr = e.Graphics.MeasureString(workingStr, new Font("Calibri light", fontSizeS + 4, FontStyle.Bold)).Width;

                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS + 2, FontStyle.Bold), Brushes.Black, new Point((bounds.Right / 2) - ((int)measureStr / 2), headerpointVer));
                        e.Graphics.DrawString("Document nbr.  " + PrimkaNumber, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, headerpointVer + (moveBy * 2)));

                        headerpointVer = headerpointVer + (moveBy * 4);
                    }
                    else
                    {
                        imgH = 40;
                        imgW = 80;
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);
                        headerpointVer = bounds.Top + margins.Top + imgH + 50;
                    }

                    e.Graphics.DrawRectangle(new Pen(Brushes.Black), margins.Left, headerpointVer, bounds.Right - margins.Right - margins.Left, 20);
                    e.Graphics.FillRectangle(new SolidBrush(Color.AliceBlue), margins.Left + 1, headerpointVer + 1, bounds.Right - margins.Right - margins.Left - 2, 18);

                    int total = bounds.Right - margins.Left - margins.Right;
                    int rb = margins.Left + total / 17;
                    int code = rb + total / 5;
                    int name = code + total / 2;
                    int mes = name + total / 9;
                    int rowHeight = 20;

                    //GRID
                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(rb, headerpointVer), new Point(rb, headerpointVer + rowHeight));
                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(code, headerpointVer), new Point(code, headerpointVer + rowHeight));
                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(name, headerpointVer), new Point(name, headerpointVer + rowHeight));
                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(mes, headerpointVer), new Point(mes, headerpointVer + rowHeight));

                    Font fnt = getFont(fontSizeR);

                    workingStr = "RB";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = rb - margins.Left;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    workingStr = "CODE";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = code - rb;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(rb + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    workingStr = "NAME";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = name - code;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(code + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    workingStr = "PACK.";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = mes - name;
                    e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(name + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    workingStr = "QUA.";
                    fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = bounds.Right - margins.Right - mes;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(mes + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                    var groupedPartsList = partListPrint.GroupBy(c => c.PartialCode).Select(grp => grp.ToList()).ToList();
                    //for (; partRows < 35; partRows++)
                    for (; partRows < groupedPartsList.Count; partRows++)
                    {

                        if (headerpointVer + (moveBy * 4) + 20 > napomenaHeight && napomenaHeight != 0)
                        {
                            e.HasMorePages = true;
                            Properties.Settings.Default.printingSN = false;
                            break;

                        }
                        else
                        {
                            Properties.Settings.Default.printingSN = true;
                        }

                        headerpointVer = headerpointVer + (moveBy * 2);
                        //e.Graphics.DrawRectangle(new Pen(Brushes.Black), margins.Left, headerpointVer, bounds.Right - margins.Right - margins.Left, 20);
                        //e.Graphics.DrawLine(new Pen(Brushes.Black), margins.Left, headerpointVer + (moveBy * 2), bounds.Right - margins.Right, headerpointVer + (moveBy * 2));
                        float[] dashValues = { 2, 2, 2, 2 };
                        Pen blackPen = new Pen(Color.Black, 1);
                        blackPen.DashPattern = dashValues;
                        e.Graphics.DrawLine(blackPen, margins.Left, headerpointVer + (moveBy * 2), bounds.Right - margins.Right, headerpointVer + (moveBy * 2));

                        workingStr = string.Format("{0:000}", groupedPartsList[0][0].CompanyO) + string.Format("{0:00}", groupedPartsList[partRows][0].CompanyC) + " " + string.Format("{0:000 000 000}", groupedPartsList[0][0].PartialCode);//tu partRows
                        fnt = fitFontSize(e, workingStr, fontSizeR, code - rb);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = code - rb;
                        e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(rb + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = sifrarnikArr[(sifrarnikArr.IndexOf((groupedPartsList[partRows][0].PartialCode).ToString())) - 1];//tu
                        fnt = fitFontSize(e, workingStr, fontSizeR, code - rb);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = name - code;
                        e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(code + 3, headerpointVer + moveBy));

                        workingStr = "kom";
                        fnt = fitFontSize(e, workingStr, fontSizeR, code - rb);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = mes - name;
                        e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(name + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        workingStr = groupedPartsList[partRows].Count.ToString();//tu
                        fnt = fitFontSize(e, workingStr, fontSizeR, code - rb);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = bounds.Right - margins.Right - mes;
                        e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(mes + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));

                        fnt = fitFontSize(e, (partRows + 1).ToString(), fontSizeR, code - rb); //tu
                        workingStr = (partRows + 1).ToString();//tu
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = rb - margins.Left;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy));
                    }

                    Properties.Settings.Default.partRows = partRows;
                    
                    if (groupedPartsList.Count == 0)
                        e.HasMorePages = false;
                    else
                        e.HasMorePages = true;

                    //NAPOMENA
                    if (pageNbr == 1)
                    {
                        napomenaHeight = 0;
                        workingStr = "NAPOMENA: " + napomenaPRIMPrint;
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = bounds.Right - margins.Right - margins.Left;

                        int ii = 5;
                        int secondLine = 0;
                        int wsNAPOMENAw = (int)e.Graphics.MeasureString("NAPOMENA :", fnt).Width;
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

                    e.Graphics.DrawString("Page : " + pageNbr, getFont(8), Brushes.Black, new Point(margins.Left, bounds.Bottom - margins.Bottom));

                    Properties.Settings.Default.pageNbr = pageNbr = pageNbr + 1;

                    fnt = fitFontSize(e, (partRows + 1).ToString(), fontSizeR, code - rb);
                    workingStr = "Document: " + PrimkaNumber;
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    measureField = bounds.Right - margins.Right - margins.Left;
                    e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), bounds.Bottom - margins.Bottom));

                    workingStr = Properties.Settings.Default.CmpWWW;
                    fnt = fitFontSize(e, workingStr, fontSizeR, code - rb);
                    measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                    e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(bounds.Right - margins.Right - (int)measureStr, bounds.Bottom - margins.Bottom));
                }
                return;
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        void printSN(PrintPageEventArgs e)
        {
            partRows = Properties.Settings.Default.partRows;
            pageNbr = Properties.Settings.Default.pageNbr;

            e.HasMorePages = false;

            PageSettings page = GetPrinterPageInfo();

            RectangleF area = page.PrintableArea;
            Rectangle bounds = page.Bounds;
            Margins margins = page.Margins;

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
                    partRows = Properties.Settings.Default.partRows;
                    var groupedPartsListSN = partListPrint.GroupBy(c => c.PartialCode).Select(grp => grp.ToList()).ToList();

                    if (partRows >= groupedPartsListSN.Count)
                    {
                        Properties.Settings.Default.printingSN = true;
                        Properties.Settings.Default.partRows = partRows = 0;
                    }

                    //if (partRows >= 35) //Ovo je test
                    if (partRows <= groupedPartsListSN.Count) //Ovo je test
                    {
                        headerpointVer = bounds.Bottom + margins.Top;

                        imgH = 40;
                        imgW = 80;

                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);
                        headerpointVer = bounds.Top + margins.Top + imgH + 50;


                        e.Graphics.DrawRectangle(new Pen(Brushes.Black), margins.Left, headerpointVer, bounds.Right - margins.Right - margins.Left, 20);
                        e.Graphics.FillRectangle(new SolidBrush(Color.AliceBlue), margins.Left + 1, headerpointVer + 1, bounds.Right - margins.Right - margins.Left - 2, 18);

                        int total = bounds.Right - margins.Left - margins.Right;
                        int rbSN = margins.Left + total / 17;
                        int field = (total - (rbSN - margins.Left)) / 3;
                        int codeSN = rbSN + field;
                        int snSN = rbSN + field * 2;
                        int rowHeight = 20;

                        workingStr = "PART LIST";
                        Font fnt = fitFontSizeBold(e, workingStr, fontSizeS, rbSN - margins.Left);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS + 2, FontStyle.Bold), Brushes.Black, new Point((bounds.Right / 2) - ((int)measureStr / 2), headerpointVer - 25));

                        //GRID
                        e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(rbSN, headerpointVer), new Point(rbSN, headerpointVer + rowHeight));
                        e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(codeSN, headerpointVer), new Point(codeSN, headerpointVer + rowHeight));
                        e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(snSN, headerpointVer), new Point(snSN, headerpointVer + rowHeight));

                        workingStr = "RB";
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, rbSN - margins.Left);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = rbSN - margins.Left;
                        e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, rbSN - margins.Left), Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                        workingStr = "CODE";
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, codeSN - rbSN);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = codeSN - rbSN;
                        e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, codeSN - rbSN), Brushes.Black, new Point(rbSN + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                        workingStr = "SERIAL NBR.";
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, snSN - codeSN);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = snSN - codeSN;
                        e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, snSN - codeSN), Brushes.Black, new Point(codeSN + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));

                        workingStr = "CUSTOMER NBR.";
                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, bounds.Right - margins.Right - snSN);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = bounds.Right - margins.Right - snSN;
                        e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(snSN + (((int)measureField - (int)measureStr) / 2), headerpointVer + rowHeight / 4));


                        //Doc INFO
                        e.Graphics.DrawString("Page : " + pageNbr, getFont(8), Brushes.Black, new Point(margins.Left, bounds.Bottom - margins.Bottom));

                        Properties.Settings.Default.pageNbr = pageNbr = pageNbr + 1;
                        Properties.Settings.Default.Save();

                        fnt = fitFontSize(e, (partRows + 1).ToString(), fontSizeR, codeSN - rbSN);
                        workingStr = "Document: " + PrimkaNumber;
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = bounds.Right - margins.Right - margins.Left;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), bounds.Bottom - margins.Bottom));

                        workingStr = Properties.Settings.Default.CmpWWW;
                        fnt = fitFontSize(e, workingStr, fontSizeR, codeSN - rbSN);
                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        e.Graphics.DrawString(workingStr, getFont(8), Brushes.Black, new Point(bounds.Right - margins.Right - (int)measureStr, bounds.Bottom - margins.Bottom));

                        //var groupedPartsList = partListPrint.GroupBy(c => c.PartialCode).Select(grp => grp.ToList()).ToList();
                        //for (; partRows < 35; partRows++)
                        for (; partRows < groupedPartsListSN.Count; partRows++)
                        {
                            int rbInner = 1;
                            for (int partRowsInner = 0; partRowsInner < groupedPartsListSN[partRows].Count; partRowsInner++)
                            {
                                if (headerpointVer + (moveBy * 4) + 20 > (bounds.Bottom - margins.Bottom))
                                {
                                    //printingSN = true;
                                    e.HasMorePages = true;
                                    Properties.Settings.Default.printingSN = true;
                                    Properties.Settings.Default.pageNbr = pageNbr;
                                    Properties.Settings.Default.partRows = partRows;
                                    return;

                                }

                                headerpointVer = headerpointVer + (moveBy * 3);
                                //e.Graphics.DrawRectangle(new Pen(Brushes.Black), margins.Left, headerpointVer, bounds.Right - margins.Right - margins.Left, 20);
                                //e.Graphics.DrawLine(new Pen(Brushes.Black), margins.Left, headerpointVer + (moveBy * 2), bounds.Right - margins.Right, headerpointVer + (moveBy * 2));
                                float[] dashValues = { 2, 2, 2, 2 };
                                Pen blackPen = new Pen(Color.Black, 1);
                                blackPen.DashPattern = dashValues;
                                e.Graphics.DrawLine(blackPen, margins.Left, headerpointVer + (moveBy * 3), bounds.Right - margins.Right, headerpointVer + (moveBy * 3));

                                workingStr = (partRows + 1).ToString()+ " - " + rbInner;//tu partRows
                                fnt = fitFontSize(e, workingStr, fontSizeR, rbSN - margins.Left);
                                measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                                measureField = rbSN - margins.Left;
                                e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, rbSN - margins.Left), Brushes.Black, new Point(margins.Left + (((int)measureField - (int)measureStr) / 2), headerpointVer + moveBy * 2)); //  + moveBy

                                workingStr = string.Format("{0:000}", groupedPartsListSN[partRows][partRowsInner].CompanyO) + string.Format("{0:00}", groupedPartsListSN[partRows][partRowsInner].CompanyC) + string.Format("{0:000000000}", groupedPartsListSN[partRows][partRowsInner].PartialCode);//tu partRows
                                fnt = fitFontSizeIDAutomation(e, workingStr, fontSizeR, codeSN - rbSN);
                                measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                                measureField = codeSN - rbSN;
                                e.Graphics.DrawString("*" + workingStr + "*", fitFontSizeIDAutomation(e, workingStr, 6, codeSN - rbSN), Brushes.Black, new Point(rbSN + (((int)measureField - (int)measureStr) / 2), headerpointVer)); //  + moveBy

                                workingStr = groupedPartsListSN[partRows][partRowsInner].SN.ToString();//tu
                                fnt = fitFontSizeIDAutomation(e, workingStr, fontSizeR, snSN - codeSN);
                                measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                                measureField = snSN - codeSN;
                                e.Graphics.DrawString("*" + workingStr + "*", fitFontSizeIDAutomation(e, workingStr, 6, snSN - codeSN), Brushes.Black, new Point(codeSN + (((int)measureField - (int)measureStr) / 2), headerpointVer)); //  + moveBy

                                workingStr = groupedPartsListSN[partRows][partRowsInner].CN.ToString();//tu
                                fnt = fitFontSizeIDAutomation(e, (partRows + 1).ToString(), fontSizeR, bounds.Right - margins.Right - snSN); //tu
                                measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                                measureField = bounds.Right - margins.Right - snSN;
                                e.Graphics.DrawString("*" + workingStr + "*", fitFontSizeIDAutomation(e, workingStr, 6, bounds.Right - margins.Right - snSN), Brushes.Black, new Point(snSN + (((int)measureField - (int)measureStr) / 2), headerpointVer)); //  + moveBy
                                rbInner++;
                            }
                        }

                        if (signature)
                        {
                            headerpointVer = headerpointVer + (moveBy * 12);

                            if (headerpointVer + (moveBy * 16) + 20 > (bounds.Bottom - margins.Bottom))
                            {
                                //printingSN = true;
                                e.HasMorePages = true;
                                Properties.Settings.Default.printingSN = true;
                                Properties.Settings.Default.pageNbr = pageNbr;
                                Properties.Settings.Default.partRows = partRows;
                                return;

                            }

                            int lineLength = (bounds.Right - margins.Right - margins.Left) / 4;
                            e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, headerpointVer), new Point(margins.Left + lineLength, headerpointVer));
                            headerpointVer = headerpointVer + (moveBy / 2);

                            workingStr = "Date";
                            fnt = fitFontSize(e, workingStr, 7, lineLength);
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + ((lineLength - (int)measureStr) / 2), headerpointVer));
                            headerpointVer = headerpointVer + (moveBy * 4);

                            e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, headerpointVer), new Point(margins.Left + lineLength, headerpointVer));
                            headerpointVer = headerpointVer + (moveBy / 2);

                            workingStr = "Signature";
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + ((lineLength - (int)measureStr) / 2), headerpointVer));
                        }

                        Properties.Settings.Default.printingSN = false;
                        e.HasMorePages = false;

                        return;
                    }
                }
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
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
            return new Font("Calibri light", _FontSize, FontStyle.Regular); ;

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
