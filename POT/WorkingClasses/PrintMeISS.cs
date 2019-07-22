using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    class PrintMeISS
    {
        int partRows;
        int pageNbr;
        Company cmpCust = new Company();
        Company cmpM = new Company();
        List<String> sifrarnikArr = new List<string>();
        String documentName;
        String recipientSender;
        Boolean signature = false;
        Part mainPart;
        String totalTIme;
        String ISSid = "";
        String date = "";
        List<ISSparts> listIssParts = new List<ISSparts>();

        public String datumIzrade = "";
        public String datumIspisa = "";
        public String izradioUser = "";
        public String izradioRegija = "";

        int headerpointVer;
        int headerpointHor;
        int imgH = 0;  //75
        int imgW = 150 / Properties.Settings.Default.LogoSize; //150
        int moveBy = 12;
        int fontSizeR = 8;
        int fontSizeS = 10;
        double imgScale = 0;
        float spaceSize = 0;

        PageSettings page;
        RectangleF area;
        Rectangle bounds;
        Margins margins;

        Image img = null;

        PrintMeISS() { }
        
        public PrintMeISS(Company _cmpCust, Company _cmpM, List<String>  _sifrarnikArr, Part _mainPart, List<ISSparts> _listIssParts, String _ISSid, String _DocumentName, String _recipientSender, Boolean _Signature, String _date, String _totalTIme)
        {
            cmpCust = _cmpCust;
            cmpM = _cmpM;
            sifrarnikArr = _sifrarnikArr;
            mainPart = _mainPart;
            ISSid = _ISSid;
            documentName = _DocumentName.ToUpper();
            listIssParts = _listIssParts;
            recipientSender = _recipientSender.ToUpper();
            signature = _Signature;
            date = _date;
            totalTIme = _totalTIme;

            CLogo logoImage = new CLogo();
            img = logoImage.GetImage();

            imgScale = (double)img.Width / img.Height;

            imgH = (int)((double)imgW / imgScale);
        }

        public void Print(PrintPageEventArgs e)
        {
            printParts(e);
            return;
        }

        void printParts(PrintPageEventArgs e)
        {
            Font fnt = getFont(fontSizeR);
            spaceSize = e.Graphics.MeasureString("COMMENT: ", fnt).Width + 15;

            partRows = Properties.Settings.Default.partRows;
            pageNbr = Properties.Settings.Default.pageNbr;

            datumIzrade = date;

            if (datumIzrade.Equals(""))
                datumIzrade = DateTime.Now.ToString("dd.MM.yy.");
            if (datumIspisa.Equals(""))
                datumIspisa = DateTime.Now.ToString("dd.MM.yy.");
            if (izradioUser.Equals(""))
                izradioUser = WorkingUser.UserID.ToString();
            if (izradioRegija.Equals(""))
                izradioRegija = WorkingUser.RegionID.ToString();

            e.HasMorePages = false;

            page = GetPrinterPageInfo();

            area = page.PrintableArea;
            bounds = page.Bounds;
            margins = page.Margins;

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
                    if (pageNbr == 1)
                    {
                        //Sender/Receiver Company Info
                        e.Graphics.DrawString(recipientSender + ": ", new Font("Calibri light", fontSizeS - 1, FontStyle.Underline | FontStyle.Italic), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy / 2)));
                        e.Graphics.DrawString(cmpCust.Name, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 2)));
                        e.Graphics.DrawString(cmpCust.Address, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 3)));
                        e.Graphics.DrawString(cmpCust.Country + " - " + cmpCust.City + ", " + cmpCust.PB, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 4)));
                        e.Graphics.DrawString(Properties.strings.VAT + ": " + cmpCust.OIB, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 5)));
                        e.Graphics.DrawString(Environment.NewLine, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 6 / 2)));

                        int pomak = 6;

                        //PRVI RED JE NASLOV ZA IZRADIO
                        e.Graphics.DrawString(Properties.strings.MadeBy.ToUpper() + ": ", new Font("Calibri light", fontSizeS - 1, FontStyle.Underline | FontStyle.Italic), Brushes.Black, new Point(margins.Left, margins.Top - 5 + (moveBy * (pomak + 3))));
                        e.Graphics.DrawString(Properties.strings.Date + ": " + datumIzrade, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * (pomak + 4))));
                        e.Graphics.DrawString(Properties.strings.Time + ": " + datumIspisa, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * (pomak + 5))));
                        e.Graphics.DrawString(Properties.strings.UserID + ": " + izradioUser, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * (pomak + 6))));
                        e.Graphics.DrawString(Properties.strings.RegionID + ": " + izradioRegija, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * (pomak + 7))));

                        //KORISNIK - da se ne vidi ime i prezime, odkomentiraj da se vidi
                        //e.Graphics.DrawString(Properties.strings.MadeBy + ": " + WorkingUser.Name + " " + WorkingUser.Surename, new Font("Calibri light", fontSizeS, FontStyle.Regular), Brushes.Black, new Point(margins.Left, margins.Top + (moveBy * 13)));

                        
                        //MyCompany Info
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);
                        
                        String MainCmpPrintAddress = Properties.Settings.Default.CmpAddress + ", " + Properties.Settings.Default.CmpCountry + " - " + Properties.Settings.Default.CmpPB + " " + Properties.Settings.Default.CmpCity;
                        
                        e.Graphics.DrawString(Properties.Settings.Default.CmpName, new Font("Calibri light", fontSizeR, FontStyle.Bold), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy)));
                        e.Graphics.DrawString(MainCmpPrintAddress, new Font("Calibri light", fontSizeR, FontStyle.Regular), Brushes.Black, new Point(bounds.Right - margins.Right - (imgW - imgW / 7), margins.Top + imgH + (moveBy * 2)));

                        headerpointVer = margins.Top + imgH + (moveBy * 7) + 100;
                        headerpointHor = bounds.Right - margins.Right - imgW;
                        
                        workingStr = documentName;
                        measureStr = e.Graphics.MeasureString(workingStr, new Font("Calibri light", fontSizeS + 4, FontStyle.Bold)).Width;

                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS + 2, FontStyle.Bold), Brushes.Black, new Point((bounds.Right / 2) - ((int)measureStr / 2), headerpointVer));
                        e.Graphics.DrawString(Properties.strings.DocumentNbr + "  " + ISSid, new Font("Calibri light", fontSizeS, FontStyle.Bold), Brushes.Black, new Point(margins.Left, headerpointVer + (moveBy * 2)));

                        headerpointVer = headerpointVer + (moveBy * 4);


                        //MAIN PART INFO
                        String mainPartName = sifrarnikArr[(sifrarnikArr.IndexOf((mainPart.PartialCode).ToString())) - 1];
                        Font fntBold = new Font("Calibri light", fontSizeS, FontStyle.Bold);
                        Font fntRegular = new Font("Calibri light", fontSizeS, FontStyle.Regular);
                        float pamtiUdaljenost = 0;

                        headerpointVer = headerpointVer + moveBy;
                        workingStr = "PART INFO";
                        e.Graphics.DrawString(workingStr, new Font("Calibri light", fontSizeS, FontStyle.Bold | FontStyle.Underline), Brushes.Black, new Point(margins.Left, headerpointVer));
                        headerpointVer = headerpointVer + moveBy * 2;

                        workingStr = "Name: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left, headerpointVer));
                        measureStr = e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        workingStr = mainPartName;
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));

                        moveBy += 2;

                        headerpointVer = headerpointVer + moveBy;

                        ///////////////////////////////////////////////////////////////
                        ///

                        workingStr = "Code: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left + (int)pamtiUdaljenost, headerpointVer));
                        measureStr = e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        if(mainPart.CodePartFull.ToString().Length == 12)
                            workingStr = "0" + mainPart.CodePartFull.ToString();
                        else
                            workingStr = mainPart.CodePartFull.ToString();
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));

                        pamtiUdaljenost = measureStr + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;

                        workingStr = "SN: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left + (int)pamtiUdaljenost, headerpointVer));
                        measureStr = pamtiUdaljenost + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        workingStr = mainPart.SN;
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));

                        pamtiUdaljenost = measureStr + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;

                        workingStr = "CN: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left + (int)pamtiUdaljenost, headerpointVer));
                        measureStr = pamtiUdaljenost + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        workingStr = mainPart.CN;
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));
                        headerpointVer = headerpointVer + moveBy;

                        ///////////////////////////////////////////////////////////////
                        ///

                        pamtiUdaljenost = 0;

                        workingStr = "Date in: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left + (int)pamtiUdaljenost, headerpointVer));
                        measureStr = e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        workingStr = workingStr = mainPart.DateIn;
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));

                        pamtiUdaljenost = measureStr + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;

                        workingStr = "Storage: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left + (int)pamtiUdaljenost, headerpointVer));
                        measureStr = pamtiUdaljenost + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        workingStr = mainPart.StorageID.ToString();
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));

                        pamtiUdaljenost = measureStr + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;

                        workingStr = "Total time: ";
                        e.Graphics.DrawString(workingStr, fntBold, Brushes.Black, new Point(margins.Left + (int)pamtiUdaljenost, headerpointVer));
                        measureStr = pamtiUdaljenost + e.Graphics.MeasureString(workingStr, fntBold).Width + 10;
                        workingStr = workingStr = totalTIme;
                        e.Graphics.DrawString(workingStr, fntRegular, Brushes.Black, new Point(margins.Left + (int)measureStr, headerpointVer));

                        headerpointVer = headerpointVer + moveBy;

                        moveBy -= 2;
                    }
                    else
                    {
                        imgW = imgW / 1; //80
                        imgH = (int)((double)imgW / imgScale); //75 //40
                        e.Graphics.DrawImage(img, bounds.Right - imgW - margins.Right, bounds.Top + margins.Top, imgW, imgH);
                        headerpointVer = bounds.Top + margins.Top + imgH + 50;
                    }


                    int total = bounds.Right - margins.Left - margins.Right;
                    int rb = margins.Left + total / 17;
                    int code = rb + total / 5;
                    int name = code + total / 2;
                    int mes = name + total / 9;
                    
                    //for (; partRows < 35; partRows++)
                    for (; partRows < listIssParts.Count; partRows++)
                    {

                        if (headerpointVer + (moveBy * 6) + 20 > napomenaHeight && napomenaHeight != 0)
                        {
                            e.HasMorePages = true;
                            Properties.Settings.Default.partRows = partRows;
                            break;
                        }
                        else
                        {
                            e.HasMorePages = false;
                        }



                        headerpointVer = headerpointVer + (moveBy * 2);
                        
                        float[] dashValues = { 2, 2, 2, 2 };
                        Pen blackPen = new Pen(Color.Black, 1);
                        blackPen.DashPattern = dashValues;
                        e.Graphics.DrawLine(blackPen, margins.Left, headerpointVer + (moveBy * 2), bounds.Right - margins.Right, headerpointVer + (moveBy * 2));

                        String partOldName = "";
                        String partNewName = "";
                        String columnName = "";
                        int saljiSize = (int)spaceSize + margins.Left + 20;

                        measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                        measureField = code - rb;

                        fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                        columnName = listIssParts[partRows].RB.ToString() + ".";
                        workingStr = columnName;
                        e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left, headerpointVer + moveBy));
                        headerpointVer = headerpointVer + moveBy + 5;


                        if (listIssParts[partRows].PrtO.PartialCode != 0)
                        {
                            columnName = "PART OLD: ";
                            workingStr = columnName;
                            fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + 20, headerpointVer + moveBy));

                            partOldName = sifrarnikArr[(sifrarnikArr.IndexOf((listIssParts[partRows].PrtO.PartialCode).ToString())) - 1];
                            workingStr = listIssParts[partRows].PrtO.PartID.ToString() + " - " + partOldName;
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            toBig(saljiSize, measureStr, workingStr, e, fnt, code - rb);

                            //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(saljiSize, headerpointVer + moveBy));
                            //headerpointVer = headerpointVer + moveBy;
                        }


                        if (listIssParts[partRows].PrtN.PartialCode != 0)
                        {
                            columnName = "PART NEW: ";
                            workingStr = columnName;
                            fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + 20, headerpointVer + moveBy));

                            partNewName = sifrarnikArr[(sifrarnikArr.IndexOf((listIssParts[partRows].PrtO.PartialCode).ToString())) - 1];
                            workingStr = listIssParts[partRows].PrtN.PartID.ToString() + " - " + partNewName;
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            toBig(saljiSize, measureStr, workingStr, e, fnt, code - rb);

                            //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(saljiSize, headerpointVer + moveBy));
                            //headerpointVer = headerpointVer + moveBy;
                        }


                        if (!listIssParts[partRows].Work.Equals(""))
                        {
                            columnName = "WORK DONE: ";
                            workingStr = columnName;
                            fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + 20, headerpointVer + moveBy));

                            workingStr = listIssParts[partRows].Work;
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            toBig(saljiSize, measureStr, workingStr, e, fnt, code - rb);

                            //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(saljiSize, headerpointVer + moveBy));
                            //headerpointVer = headerpointVer + moveBy;
                        }


                        if (!listIssParts[partRows].Comment.Equals(""))
                        {
                            columnName = "COMMENT: ";
                            workingStr = columnName;
                            fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + 20, headerpointVer + moveBy));
                            
                            workingStr = listIssParts[partRows].Comment;
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            toBig(saljiSize, measureStr, workingStr, e, fnt, code - rb);
                            
                            //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(saljiSize, headerpointVer + moveBy));
                            //headerpointVer = headerpointVer + moveBy;
                        }


                        if (!listIssParts[partRows].Time.Equals(""))
                        {
                            columnName = "TIME: ";
                            workingStr = columnName;
                            fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + 20, headerpointVer + moveBy));

                            workingStr = listIssParts[partRows].Time;
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            toBig(saljiSize, measureStr, workingStr, e, fnt, code - rb);

                            //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(saljiSize, headerpointVer + moveBy));
                            //headerpointVer = headerpointVer + moveBy;
                        }

                        if (listIssParts[partRows].UserID != 0)
                        {
                            columnName = "USER ID: ";
                            workingStr = columnName;
                            fnt = fitFontSizeBold(e, workingStr, fontSizeR, code - rb);
                            e.Graphics.DrawString(workingStr, fnt, Brushes.Black, new Point(margins.Left + 20, headerpointVer + moveBy));

                            workingStr = listIssParts[partRows].UserID.ToString();
                            measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
                            toBig(saljiSize, measureStr, workingStr, e, fnt, code - rb);

                            //e.Graphics.DrawString(workingStr, fitFontSize(e, workingStr, fontSizeR, code - rb), Brushes.Black, new Point(saljiSize, headerpointVer + moveBy));
                            //headerpointVer = headerpointVer + moveBy;
                        }
                    }

                    

                    e.Graphics.DrawString(Properties.strings.Page + " : " + pageNbr, getFont(8), Brushes.Black, new Point(margins.Left, bounds.Bottom - margins.Bottom));

                    if (e.HasMorePages)
                        Properties.Settings.Default.pageNbr = pageNbr = pageNbr + 1;
                    else
                    {
                        Properties.Settings.Default.pageNbr = 1;
                        Properties.Settings.Default.partRows = 0;
                    }

                    fnt = fitFontSize(e, (partRows + 1).ToString(), fontSizeR, code - rb);
                    workingStr = Properties.strings.Document + ": " + ISSid;
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

        private void toBig(int space, float size, String wString, PrintPageEventArgs e, Font fnt, int FieldWith)
        {
            String workingStrInner = wString;
            float measureStr = size;
            int wsLenght = (int)e.Graphics.MeasureString(workingStrInner, fnt).Width;
            float measureField = bounds.Right - (margins.Left + space);

            String ws1 = "";
            String ws2 = "";
            Boolean zavrsavaSa = true;

            if (measureStr <= measureField)
            {
                e.Graphics.DrawString(workingStrInner, fitFontSize(e, wString, fontSizeR, FieldWith), Brushes.Black, new Point(space, headerpointVer + moveBy));
                headerpointVer = headerpointVer + moveBy;
            }
            else
            {
                ws1 = workingStrInner; 

                while (measureStr > measureField)
                {
                    while (wsLenght > measureField || zavrsavaSa)
                    {
                        ws2 = ws1.Substring(ws1.Length - 1, 1) + ws2; 
                        ws1 = ws1.Substring(0, ws1.Length - 1);
                        wsLenght = (int)e.Graphics.MeasureString(ws1, fnt).Width;

                        if (wsLenght <= measureField && ws1.EndsWith(" "))
                            zavrsavaSa = false;
                    }

                    zavrsavaSa = true;
                    measureStr = wsLenght = (int)e.Graphics.MeasureString(ws2, fnt).Width;
                
                    if (wsLenght > measureField)
                    {
                        e.Graphics.DrawString(ws1, fitFontSize(e, "ws1", fontSizeR, FieldWith), Brushes.Black, new Point(space, headerpointVer + moveBy));
                        headerpointVer = headerpointVer + moveBy;

                        ws1 = ws2;
                        ws2 = "";
                    }
                    else
                    {
                        e.Graphics.DrawString(ws1, fitFontSize(e, "ws1", fontSizeR, FieldWith), Brushes.Black, new Point(space, headerpointVer + moveBy));
                        headerpointVer = headerpointVer + moveBy;

                        e.Graphics.DrawString(ws2, fitFontSize(e, "ws2", fontSizeR, FieldWith), Brushes.Black, new Point(space, headerpointVer + moveBy));
                        headerpointVer = headerpointVer + moveBy;
                    }
                }
            }
        }
    }
}
