using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    class PrintMeRN
    {
        //Fontovi
        //static string fontName = "Myriad Pro Light";
        static string fontName = "Calibri Light";

        static Font fntA7;
        static Font fntA8;
        static Font fntA9;
        static Font fntA10;
        static Font fntA11;
        static Font fntA12;
        static Font fntA18;

        static Font fntA7b;
        static Font fntA8b;
        static Font fntA9b;
        static Font fntA10b;
        static Font fntA11b;
        static Font fntA12b;
        static Font fntA18b;

        public List<String> UsersInfo = new List<String>();

        int kursorVer;
        int kursorHor;
        int kursorHorPodaci;

        //varijable za novu stranicu
        public static int pgNumber = 1;
        public static int partsCount = 0;

        static TIDs tidRN = new TIDs();
        //static RadniNalog RN;

        Image img = null;
        double imgScale;
        int imgH = 0;  //75
        int imgW = 300; //300
        QueryCommands qc = new QueryCommands();

        public PrintMeRN() { }

        public PrintMeRN(long mID) //TODO primiti broj ponude
        {
            tidRN.GetTIDByID(mID);
            //RN = new RadniNalog(mID);

            CLogo logoImage = new CLogo();
            img = logoImage.GetImage();

            imgScale = (double)img.Width / img.Height;

            imgH = (int)((double)imgW / imgScale);

            fntA7 = getFont(7);
            fntA8 = getFont(8);
            fntA9 = getFont(9);
            fntA10 = getFont(10);
            fntA11 = getFont(11);
            fntA12 = getFont(12);
            fntA18 = getFont(18);

            fntA7b = getFontBold(7);
            fntA8b = getFontBold(8);
            fntA9b = getFontBold(9);
            fntA10b = getFontBold(10);
            fntA11b = getFontBold(11);
            fntA12b = getFontBold(12);
            fntA18b = getFontBold(18);

            UsersInfo = qc.UsersInfo(WorkingUser.Username, WorkingUser.Password);
        }

        public void Print(PrintPageEventArgs e)
        {
            e.HasMorePages = false;

            Font fontID32 = new Font("IDAutomationHC39M", 11, FontStyle.Regular);

            int sredina = 0;
            int kocka = 200;

            int pozicija1 = 0;
            int pozicija2 = 0;
            int pozicija3 = 0;

            long userIDinfo = 0;

            int moveBy = 5;

            PageSettings page = GetPrinterPageInfo();

            RectangleF area = page.PrintableArea;
            Rectangle bounds = page.Bounds;
            Margins margins = page.Margins;

            //inace po defaultu 100
            margins.Left = 75;
            margins.Right = 75;
            margins.Top = 50;
            margins.Bottom = 75;

            kursorVer = margins.Top;
            kursorHor = bounds.Right - margins.Right;

            try
            {
                string workingStr = "";
                float measureStr = 0;
                float measureField = 0;
                
                //Color cl = ColorTranslator.FromHtml("#1B75BC");
                Color cl = ColorTranslator.FromHtml("#0174AA");

                Brush brushBlue = new SolidBrush(cl);
                Brush brushBlack = Brushes.Black;
                Brush brushWhite = Brushes.White;
                Brush brushGrey = Brushes.Gray;

                using (img)
                {
                    
                    Pen bijeliPen = new Pen(Brushes.White);
                    Pen crniPen = new Pen(Brushes.Black);

                    //TICKET ID
                    workingStr = "SERVICE REPORT";
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA18).Width);
                    //kursorHor -= (int)measureStr -50;
                    kursorHor = bounds.Right - margins.Right - (int)measureStr;
                    e.Graphics.DrawString(workingStr, fntA18, brushBlue, kursorHor, kursorVer);

                    moveBy = fntA18.Height;
                    kursorHor = margins.Left;
                    Rectangle rect = new Rectangle( kursorHor, kursorVer, kocka, moveBy);
                    e.Graphics.FillRectangle(brushBlue, rect);
                    e.Graphics.DrawRectangle(crniPen, rect);

                    workingStr = tidRN.TicketID.ToString() + "_" + tidRN.CCN + "_" + tidRN.CID;
                    Font tmp = fitFontSize(e, workingStr, 12, kocka);
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, tmp).Width);
                    sredina = (margins.Left + (kocka / 2) - (int)(measureStr / 2));
                    kursorHor -= (int)measureStr - 50;
                    e.Graphics.DrawString(workingStr, tmp, brushWhite, sredina, kursorVer + 5);
                    kursorVer += (moveBy);

                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, kursorVer), new Point(bounds.Right - margins.Right, kursorVer));



                    //PRVI DETALJI
                    moveBy = fntA10b.Height;
                    kursorVer += (moveBy);
                    kursorHor = pozicija1 = margins.Left + 5;
                    kursorHorPodaci = margins.Left + 80;

                    ///////////////////////////////////////
                    workingStr = "Tech.:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, kursorHor, kursorVer);

                    if (tidRN.UserIDUnio != 0)
                        userIDinfo = tidRN.UserIDUnio;
                    else if (tidRN.UserIDZavrsio != 0)
                        userIDinfo = tidRN.UserIDZavrsio;
                    else if (tidRN.UserIDPoceo != 0)
                        userIDinfo = tidRN.UserIDPoceo;
                    else if (tidRN.UserIDDrive != 0)
                        userIDinfo = tidRN.UserIDDrive;
                    else if (tidRN.UserIDPreuzeo != 0)
                        userIDinfo = tidRN.UserIDPreuzeo;
                    else
                        userIDinfo = tidRN.UserIDSastavio;

                    workingStr = userIDinfo.ToString();
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, kursorHorPodaci, kursorVer);

                    workingStr = "CONTACT DATA";
                    e.Graphics.DrawString(workingStr, fntA10b, brushGrey, 480, kursorVer);
                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Date:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, kursorHor, kursorVer);

                    if(tidRN.DatReport.Equals(""))
                        workingStr = DateTime.Now.ToString("dd.MM.yy."); 
                    else
                        workingStr = tidRN.DatReport;

                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, kursorHorPodaci, kursorVer);

                    workingStr = "Phone:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushGrey, 500, kursorVer);
                    workingStr = Properties.Settings.Default.CmpPhone;
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, 560, kursorVer);
                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Time:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, kursorHor, kursorVer);

                    if (tidRN.VriReport.Equals(""))
                        workingStr = DateTime.Now.ToString("HH:mm");
                    else
                        workingStr = tidRN.VriReport;

                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, kursorHorPodaci, kursorVer);

                    workingStr = "Support:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushGrey, 500, kursorVer);
                    workingStr = Properties.Settings.Default.SupportEmail;
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, 560, kursorVer);
                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Region:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, kursorHor, kursorVer);

                    if (tidRN.UserIDUnio != 0)
                        userIDinfo = tidRN.UserIDUnio;
                    else if(tidRN.UserIDPreuzeo != 0)
                        userIDinfo = tidRN.UserIDPreuzeo;
                    else
                        userIDinfo = tidRN.UserIDSastavio;

                    workingStr = qc.RegionInfoByUserID(userIDinfo)[2];
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, kursorHorPodaci, kursorVer);

                    workingStr = "Info:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushGrey, 500, kursorVer);
                    workingStr = Properties.Settings.Default.CmpEmail;
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, 560, kursorVer);
                    kursorVer += (moveBy * 2);
                    ///////////////////////////////////////


                    //DRUGI KVADRAT
                    moveBy = fntA18.Height;
                    kursorHor = margins.Left;
                    rect = new Rectangle(kursorHor, kursorVer, kocka, moveBy);
                    e.Graphics.FillRectangle(brushBlue, rect);
                    e.Graphics.DrawRectangle(crniPen, rect);

                    workingStr = "CUSTOMER INQURY";
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA12).Width);
                    sredina = (margins.Left + (kocka / 2) - (int)(measureStr / 2));
                    kursorHor -= (int)measureStr - 50;
                    e.Graphics.DrawString(workingStr, fntA12, brushWhite, sredina, kursorVer + 5);
                    kursorVer += (moveBy);

                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, kursorVer), new Point(bounds.Right - margins.Right, kursorVer));



                    //DRUGI DETALJI
                    moveBy = fntA10b.Height;
                    kursorVer += (moveBy);
                    kursorHor = margins.Left + 5;
                    kursorHorPodaci = margins.Left + 80;

                    int triPlaces = (bounds.Right - margins.Left - margins.Right) / 3;
                    //pozicija1 = 0; //vec postavljeno gore
                    pozicija2 = margins.Left + triPlaces;
                    pozicija3 = margins.Left + (triPlaces * 2);

                    ///////////////////////////////////////
                    workingStr = "Date:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.DatPrijave;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    workingStr = "Contact:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    workingStr = tidRN.Prijavio;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);

                    workingStr = "Time SLA:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija3, kursorVer);
                    workingStr = tidRN.VriSLA;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija3 + 70, kursorVer);

                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Time:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.VriPrijave;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    workingStr = "Priority:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    workingStr = tidRN.Prio.ToString();
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);

                    workingStr = "Date SLA:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija3, kursorVer);
                    workingStr = tidRN.DatSLA;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija3 + 70, kursorVer);

                    kursorVer += (moveBy);
                    ///////////////////////////////////////
                        
                    ///////////////////////////////////////
                    workingStr = "Branch:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.Filijala;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    workingStr = "Cst.ID:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    workingStr = tidRN.TvrtkeID.ToString();
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);

                    workingStr = "Cst.Name:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija3, kursorVer);
                    workingStr = qc.CompanyInfoByID(WorkingUser.Username, WorkingUser.Password, tidRN.TvrtkeID)[1];
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija3 + 70, kursorVer);

                    kursorVer += (moveBy * 2);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Problem:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.NazivUredaja;

                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    measureField = bounds.Right - margins.Right - (pozicija1 + 80);
                    workingStr = ReziMe(measureStr, measureField, workingStr, e, fntA10);
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 80, kursorVer);

                    kursorVer += (moveBy * 3);

                    workingStr = "Description:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.OpisKvara;

                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    measureField = bounds.Right - margins.Right - (pozicija1 + 80);
                    workingStr = ReziMe(measureStr, measureField, workingStr, e, fntA10);
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 80, kursorVer);

                    kursorVer += (moveBy * 3);
                    ///////////////////////////////////////



                    //TRECI KVADRAT
                    moveBy = fntA18.Height;
                    kursorHor = margins.Left;
                    rect = new Rectangle(kursorHor, kursorVer, kocka, moveBy);
                    e.Graphics.FillRectangle(brushBlue, rect);
                    e.Graphics.DrawRectangle(crniPen, rect);

                    workingStr = "SERVICE DATA";
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA12).Width);
                    sredina = (margins.Left + (kocka / 2) - (int)(measureStr / 2));
                    kursorHor -= (int)measureStr - 50;
                    e.Graphics.DrawString(workingStr, fntA12, brushWhite, sredina, kursorVer + 5);
                    kursorVer += (moveBy);

                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, kursorVer), new Point(bounds.Right - margins.Right, kursorVer));

                    //TRECI DETALJI
                    moveBy = fntA10b.Height;
                    kursorVer += (moveBy);
                    kursorHor = margins.Left + 5;
                    kursorHorPodaci = margins.Left + 80;

                    ///////////////////////////////////////
                    workingStr = "Date:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.DatZavrsio;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    workingStr = "Work start:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    workingStr = tidRN.VriPoceo;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 75, kursorVer);

                    workingStr = "Error Code:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija3, kursorVer);
                    workingStr = tidRN.Rn.ErrorCode;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija3 + 75, kursorVer);

                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Start time:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.VriDrive;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    workingStr = "Work end:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    workingStr = tidRN.VriZavrsio;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 75, kursorVer);

                    workingStr = "ID number:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija3, kursorVer);
                    workingStr = tidRN.TicketID.ToString();
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija3 + 75, kursorVer);

                    kursorVer += (moveBy * 2);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Description:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.Rn.DescriptionRN;

                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    measureField = bounds.Right - margins.Right - (pozicija1 + 80);
                    workingStr = ReziMe(measureStr, measureField, workingStr, e, fntA10);
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 80, kursorVer);

                    kursorVer += (moveBy * 3);
                    ///////////////////////////////////////


                    //CETVRTI KVADRAT
                    moveBy = fntA18.Height;
                    kursorHor = margins.Left;
                    rect = new Rectangle(kursorHor, kursorVer, kocka, moveBy);
                    e.Graphics.FillRectangle(brushBlue, rect);
                    e.Graphics.DrawRectangle(crniPen, rect);

                    workingStr = "SERVICE DATA";
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA12).Width);
                    sredina = (margins.Left + (kocka / 2) - (int)(measureStr / 2));
                    kursorHor -= (int)measureStr - 50;
                    e.Graphics.DrawString(workingStr, fntA12, brushWhite, sredina, kursorVer + 5);
                    kursorVer += (moveBy);

                    e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, kursorVer), new Point(bounds.Right - margins.Right, kursorVer));

                    //CETVRTI DETALJI
                    moveBy = fntA10b.Height;
                    kursorVer += (moveBy);
                    kursorHor = margins.Left + 5;
                    kursorHorPodaci = margins.Left + 80;

                    if (tidRN.Rn.PartListNew.Count > 1 || tidRN.Rn.PartListOld.Count > 1) //TODO vjerovatno treba druga provjera, odnosno pamtiti na kojem smo jer ovako ide u beskonacnost
                    {
                        e.HasMorePages = true;
                    }

                    pozicija2 = bounds.Right / 2;

                    ///////////////////////////////////////
                    workingStr = "S/N new:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    if (tidRN.Rn.PartListNew.Count > 0)
                    {
                        workingStr = tidRN.Rn.PartListNew[partsCount].SN;
                        e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);
                    }

                    workingStr = "S/N old:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    if (tidRN.Rn.PartListOld.Count > 0)
                    {
                        workingStr = tidRN.Rn.PartListOld[partsCount].SN;
                        e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);
                    }

                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    String kodNew = "";
                    String kodOld = "";

                    ///////////////////////////////////////
                    workingStr = "Code new:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    if (tidRN.Rn.PartListNew.Count > 0)
                    {
                        kodNew = String.Format("{0000000000000:0}", tidRN.Rn.PartListNew[partsCount].CodePartFull.ToString());

                        workingStr = kodNew;
                        e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);
                    }

                    workingStr = "Code old:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    if (tidRN.Rn.PartListOld.Count > 0)
                    {
                        kodOld = String.Format("{0000000000000:0}", tidRN.Rn.PartListOld[partsCount].CodePartFull.ToString());

                        workingStr = kodOld;
                        e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);
                    }

                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "Name new:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);

                    String fullNameNew = "";

                    if (tidRN.Rn.PartListNew.Count > 0)
                    {
                        PartSifrarnik prNew = qc.PartInfoByFullCodeSifrarnik(tidRN.Rn.PartListNew[partsCount].PartialCode);
                        workingStr = prNew.SubPartName;
                        fullNameNew = prNew.CategoryName + " " + prNew.PartName;
                        e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);
                    }

                    workingStr = "Name old:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    if (tidRN.Rn.PartListOld.Count > 0)
                    {
                        PartSifrarnik prOld = qc.PartInfoByFullCodeSifrarnik(tidRN.Rn.PartListOld[partsCount].PartialCode);
                        workingStr = prOld.SubPartName;
                        e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);
                    }

                    kursorVer += (moveBy);
                    ///////////////////////////////////////

                    ///////////////////////////////////////
                    workingStr = "CCN:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = tidRN.CCN;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    workingStr = "CID:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija2, kursorVer);
                    workingStr = tidRN.CID;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija2 + 70, kursorVer);

                    kursorVer += (moveBy);
                    ///////////////////////////////////////
                        
                    ///////////////////////////////////////
                    workingStr = "Part name:";
                    e.Graphics.DrawString(workingStr, fntA10b, brushBlack, pozicija1, kursorVer);
                    workingStr = fullNameNew;
                    e.Graphics.DrawString(workingStr, fntA10, brushBlack, pozicija1 + 70, kursorVer);

                    kursorVer += (moveBy * 2);
                    ///////////////////////////////////////

                    //BARKODOVI
                    ///////////////////////////////////////
                    workingStr = "S/N new";
                    e.Graphics.DrawString(workingStr, fntA8, brushGrey, pozicija1, kursorVer);

                    workingStr = "S/N old";
                    e.Graphics.DrawString(workingStr, fntA8, brushGrey, pozicija2, kursorVer);


                    kursorVer += 14;

                    if (tidRN.Rn.PartListNew.Count > 0)
                    {
                        workingStr = "*" + tidRN.Rn.PartListNew[partsCount].SN.ToString() + "*";
                        e.Graphics.DrawString(workingStr, fontID32, brushBlack, pozicija1, kursorVer);
                    }

                    if (tidRN.Rn.PartListOld.Count > 0)
                    {
                        workingStr = "*" + tidRN.Rn.PartListOld[partsCount].SN.ToString() + "*";
                        e.Graphics.DrawString(workingStr, fontID32, brushBlack, pozicija2, kursorVer);
                    }
                    ///////////////////////////////////////

                    kursorVer += 80;

                    ///////////////////////////////////////
                    workingStr = "Device code new";
                    e.Graphics.DrawString(workingStr, fntA8, brushGrey, pozicija1, kursorVer);

                    workingStr = "Device code old";
                    e.Graphics.DrawString(workingStr, fntA8, brushGrey, pozicija2, kursorVer);


                    kursorVer += 14;

                    if (tidRN.Rn.PartListNew.Count > 0)
                    {
                        workingStr = "*" + kodNew + "*";
                        e.Graphics.DrawString(workingStr, fontID32, brushBlack, pozicija1, kursorVer);
                    }

                    if (tidRN.Rn.PartListOld.Count > 0)
                    {
                        workingStr = "*" + kodOld + "*";
                        e.Graphics.DrawString(workingStr, fontID32, brushBlack, pozicija2, kursorVer);
                    }
                    ///////////////////////////////////////

                    kursorVer += 80;

                    ///////////////////////////////////////
                    workingStr = "Customer Call Nr.";
                    e.Graphics.DrawString(workingStr, fntA8, brushGrey, pozicija1, kursorVer);

                    workingStr = "Call ID";
                    e.Graphics.DrawString(workingStr, fntA8, brushGrey, pozicija2, kursorVer);


                    kursorVer += 14;

                    workingStr = "*" + tidRN.CCN.ToString() + "*";
                    e.Graphics.DrawString(workingStr, fontID32, brushBlack, pozicija1, kursorVer);

                    workingStr = "*" + tidRN.CID.ToString() + "*";
                    e.Graphics.DrawString(workingStr, fontID32, brushBlack, pozicija2, kursorVer);
                    ///////////////////////////////////////


                    //FOOTER
                    ///////////////////////////////////////
                    kursorVer = bounds.Bottom - (margins.Bottom + 25);

                    e.Graphics.DrawLine(new Pen(Brushes.Black, 1.5f), new Point(bounds.Right - margins.Right - 250, kursorVer), new Point(bounds.Right - margins.Right, kursorVer));
                    workingStr = "Signature";
                    e.Graphics.DrawString(workingStr, fntA9, brushGrey, bounds.Right - margins.Right - 246, kursorVer - fntA9.Height);

                    kursorVer += 25;

                    imgW = imgW / 4; //80
                    imgH = (int)((double)imgW / imgScale); //75 //40
                    e.Graphics.DrawImage(img, margins.Left, kursorVer, imgW, imgH);

                    sredina = bounds.Right / 2;

                    workingStr = "RadniNalogReport";
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, sredina - (measureStr / 2), kursorVer);

                    workingStr = DateTime.Now.ToString("HH:mm dd.MM.yy.");
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, bounds.Right - margins.Right - measureStr, kursorVer);

                    kursorVer += fntA10.Height;

                    workingStr = Properties.Settings.Default.CmpWWW;
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, sredina - (measureStr / 2), kursorVer);

                    workingStr = "Page " + pgNumber;
                    measureStr = (float)Math.Ceiling(e.Graphics.MeasureString(workingStr, fntA10).Width);
                    e.Graphics.DrawString(workingStr, fntA10, brushGrey, bounds.Right - margins.Right - measureStr, kursorVer);                 
                }

                if (e.HasMorePages)
                {
                    partsCount += 1;
                    pgNumber += 1;
                    return;
                }
               
                partsCount = 0;
                pgNumber = 1;

                return;
            }
            catch (Exception e1)
            {
                //TODO: Treba logirati gresku
                MessageBox.Show(e1.Message);

                partsCount = 0;
                pgNumber = 1;
            }
        }

        //Razne funkcionalnosti
        /////////////////////////////////////////////////////////
        
            
        private String ReziMe(float measureStr, float measureField, String workingStr, PrintPageEventArgs e, Font fnt)
        {
            String ws1 = "";
            String result = workingStr;

            while (measureStr > measureField)
            {
                result = "";

                ws1 = workingStr;
                int ws1Lenght = (int)e.Graphics.MeasureString(ws1, fnt).Width;
                while (ws1Lenght > measureField)
                {
                    ws1 = ws1.Substring(0, ws1.Length - 1);
                    ws1Lenght = (int)e.Graphics.MeasureString(ws1, fnt).Width;
                }

                if (result.Equals(""))
                {
                    result = ws1;
                }
                else
                {
                    result = result + Environment.NewLine + ws1;
                }

                //secondLine = wsNAPOMENAw;
                workingStr = workingStr.Substring(ws1.Length);
                measureStr = e.Graphics.MeasureString(workingStr, fnt).Width;
            }

            return result;
        }

        public static PageSettings GetPrinterPageInfo(string printerName)
        {
            PrinterSettings settings;

            // If printer name is not set, look for default printer
            if (string.IsNullOrEmpty(printerName))
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
        private static PageSettings GetPrinterPageInfo()
        {
            return GetPrinterPageInfo(null);
        }

        private Font getFont(int _FontSize)
        {
            return new Font(fontName, _FontSize, FontStyle.Regular);
        }

        private Font getFontBold(int _FontSize)
        {
            return new Font(fontName, _FontSize, FontStyle.Bold);
        }

        private Font fitFontSize(PrintPageEventArgs e, string _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font(fontName, velFont, FontStyle.Regular);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font(fontName, velFont, FontStyle.Regular);
        }

        private Font fitFontSizeBold(PrintPageEventArgs e, string _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font(fontName, velFont, FontStyle.Bold);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font(fontName, _FontSize, FontStyle.Bold);
        }

        private Font fitFontSizeItalic(PrintPageEventArgs e, string _WorkingStr, int _FontSize, float _FieldWith)
        {
            float _FonthWith = e.Graphics.MeasureString(_WorkingStr, getFont(_FontSize)).Width;
            int velFont = _FontSize;
            while (_FonthWith > _FieldWith)
            {
                velFont--;
                Font fnt = new Font(fontName, velFont, FontStyle.Italic);
                _FonthWith = e.Graphics.MeasureString(_WorkingStr, fnt).Width;
            }
            return new Font(fontName, _FontSize, FontStyle.Italic); ;

        }

        private Font fitFontSizeIDAutomation(PrintPageEventArgs e, string _WorkingStr, int _FontSize, float _FieldWith)
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
