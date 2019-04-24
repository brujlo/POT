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

        private Part tempPrt = new Part();

        private long pCodeO;
        private String pSNO;
        private String pCNO;

        public ISSparts() { }

        public ISSparts(long mISSid, long mRB, long mCodeO, String mSNO, String mCNO, Part mPrtN, String mWork, String mComment, String mTime)
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
            Time = mTime;
        }

        public ISSparts(long mISSid, long mRB, Part mPrtO, Part mPrtN, String mWork, String mComment, String mTime)
        {
            ISSid = mISSid;
            RB = mRB;

            PrtO = mPrtO;
            PrtN = mPrtN;

            Work = mWork;
            Comment = mComment;
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

        public string Work
        {
            get => work;

            set
            {
                work = value.Trim();
            }
        }

        public string Comment
        {
            get => comment;

            set
            {
                comment = value.Trim();
            }
        }

        public string Time
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

        public string PSNO
        {
            set
            {
                if (value.Equals(""))
                    pSNO = "";
                else
                    pSNO = value.Trim();
            }
        }

        public string PCNO
        {
            set
            {
                if (value.Equals(""))
                    pCNO = "";
                else
                    pCNO = value.Trim();
            }
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
