using System;
using Decoder = POT.WorkingClasses.Decoder;
using POT.WorkingClasses;

namespace POT.MyTypes
{
    class ISSparts
    {
        private long issID = 0;
        private long rb = 0;
        private Part prtO = new Part();
        private Part prtN = new Part();
        private String work = "";
        private String comment = "";
        private String time = "";
        private long userID = 0;
        private String date;

        private Part tempPrt = new Part();

        private long pCodeO;
        private String pSNO;
        private String pCNO;

        public ISSparts() { }

        public ISSparts(long mISSid, long mRB, long mCodeO, String mSNO, String mCNO, Part mPrtN, String mWork, String mComment, String mTime, long mUserID, String mDate)
        {
            ISSid = mISSid;
            RB = mRB;

            PCodeO = mCodeO;
            PSNO = mSNO;
            PCNO = mCNO;
            if (mCodeO != 0)
            {
                TempPrt = new Part();
                PrtO = TempPrt;
                PrtN = mPrtN;
            }
            Work = mWork;
            Comment = mComment;
            UserID = mUserID;
            Date = mDate;
            Time = mTime;
        }

        public ISSparts(long mISSid, long mRB, Part mPrtO, Part mPrtN, String mWork, String mComment, String mTime, long mUserID, String mDate)
        {
            ISSid = mISSid;
            RB = mRB;

            PrtO = mPrtO;
            PrtN = mPrtN;

            Work = mWork;
            Comment = mComment;
            UserID = mUserID;
            Date = mDate;
            Time = mTime;
        }

        public long ISSid
        {
            get => issID;

            set
            {
                issID = value;
            }
        }

        public long RB
        {
            get => rb;

            set
            {
                rb = value;
            }
        }

        public String Work
        {
            get => work;

            set
            {
                work = value.Trim();
            }
        }

        public String Comment
        {
            get => comment;

            set
            {
                comment = value.Trim();
            }
        }

        public String Time
        {
            get => time;

            set
            {
                time = value.Trim();
            } 
        }

        public Part PrtO
        {
            get => prtO;

            set
            {
                DateConverter dt = new DateConverter();

                prtO.PartID = value.PartID;
                prtO.CodePartFull = value.CodePartFull;
                prtO.SN = value.SN;
                prtO.CN = value.CN;
                prtO.DateIn = dt.ConvertDDMMYY(DateTime.Now.ToString());
                prtO.CompanyO = value.CompanyO;
                prtO.CompanyC = value.CompanyC;
                prtO.PartialCode = value.PartialCode;
                prtO.State = "ng";
                prtO.StorageID = WorkingUser.RegionID;
            }
        }

        public Part PrtN
        {
            get => prtN;

            set
            {
                prtN = value;
            }
        }

        public long PCodeO { set => pCodeO = value; }

        public String PSNO
        {
            set
            {
                if (value.Equals(""))
                    pSNO = "";
                else
                    pSNO = value.Trim();
            }
        }

        public String PCNO
        {
            set
            {
                if (value.Equals(""))
                    pCNO = "";
                else
                    pCNO = value.Trim();
            }
        }

        //public String TotalTime
        //{
        //    get => totalTime;

        //    set
        //    {
        //        int h = 0;
        //        int m = 0;
        //        int s = 0;

        //        try
        //        {
        //            h = int.Parse(value.Split(':')[0]);
        //            m = int.Parse(value.Split(':')[1]);
        //            s = int.Parse(value.Split(':')[2]);
        //        }
        //        catch{}

        //        if (h == 0 && m == 0)
        //            totalTime = "00:00";
        //        else
        //            totalTime = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m);
        //    }
        //}

        public long UserID
        {
            set
            {
                userID = value;
            }

            get => userID;
        }

        public String Date
        {
            set
            {
                if (value.Equals(""))
                    date = DateTime.Now.ToString("dd.MM.yy.");
                else
                    date = value;
            }

            get => date;
        }

        internal Part TempPrt
        {
            get => tempPrt;

            set
            {
                DateConverter dt = new DateConverter();

                tempPrt.SN = pSNO;
                tempPrt.CN = pCNO;
                tempPrt.DateIn = dt.ConvertDDMMYY(DateTime.Now.ToString());
                tempPrt.CompanyO = Decoder.GetOwnerCode(pCodeO);
                tempPrt.CompanyC = Decoder.GetCustomerCode(pCodeO);
                tempPrt.PartialCode = Decoder.GetFullPartCodeLng(pCodeO);
                tempPrt.State = "sng";
                tempPrt.StorageID = WorkingUser.RegionID;
                tempPrt.PartID = 0;

                /*
                try
                {
                    QueryCommands qc = new QueryCommands();
                    tempPrt.PartID = qc.AddPartToParts(tempPrt);
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    throw;
                }*/
            }
        }
    }
}
