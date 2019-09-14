using POT.WorkingClasses;
using System;
using System.Collections.Generic;

namespace POT.MyTypes
{
    public class Part
    {
        private long partID;
        private long codePartFull;
        private long partialCode;
        private String sN;
        private String cN;
        private String dateIn;
        private String dateOut;
        private String dateSend;
        private long storageID;
        private String state;
        private String companyO;
        private String companyC;

        public List<Part> GetListOfParts(long mCodePartFull, long mStorageID, String mState)
        {

            List<Part> pr = new List<Part>();
            List<String> resultArr = new List<string>();
            QueryCommands qc = new QueryCommands();
            try
            {

                resultArr = qc.ListPartsByCodeRegionStateS(mCodePartFull, mStorageID, mState);

                if (resultArr[0] != "nok")
                {
                    for (int i = 0; i < resultArr.Count; i = i + 12)
                    {
                        Part onlyPart = new Part();

                        onlyPart.PartID = long.Parse(resultArr[i + 0].ToString());
                        onlyPart.CodePartFull = long.Parse(resultArr[i + 1].ToString());
                        onlyPart.PartialCode = long.Parse(resultArr[i + 2].ToString());
                        onlyPart.SN = resultArr[i + 3].ToString();
                        onlyPart.CN = resultArr[i + 4].ToString();
                        onlyPart.DateIn = resultArr[i + 5].ToString();
                        onlyPart.DateOut = resultArr[i + 6].ToString();
                        onlyPart.DateSend = resultArr[i + 7].ToString();
                        onlyPart.StorageID = long.Parse(resultArr[i + 8].ToString());
                        onlyPart.State = resultArr[i + 9].ToString();
                        onlyPart.CompanyO = resultArr[i + 10].ToString();
                        onlyPart.CompanyC = resultArr[i + 11].ToString();

                        pr.Add(onlyPart);
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            return pr;
        }

        public List<Part> GetListOfParts(long mCodePartFull, String mSN, String mCN, String mState, long mStorageID)
        {

            List<Part> pr = new List<Part>();
            QueryCommands qc = new QueryCommands();
            try
            {
                pr = qc.ListPartsByCodeRegionStateP(WorkingUser.Username, WorkingUser.Password, mCodePartFull, mSN, mCN, mState, mStorageID);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            return pr;
        }

        public List<Part> GetListOfPartsFromPartsPartsPoslanoByID(List<long> mPartsID)
        {

            List<Part> pr = new List<Part>();
            List<String> resultArr = new List<string>();
            QueryCommands qc = new QueryCommands();
            try
            {
                for (int k = 0; k < mPartsID.Count; k++)
                {
                    resultArr = qc.GetListPartsByPartIDFromPartsPoslano(mPartsID[k]);
                    if (resultArr[0].Equals("nok"))
                        resultArr = qc.GetListPartsByPartIDFromParts(mPartsID[k]);

                    if (resultArr[0].Equals("nok"))
                    {
                        //pr.Clear(); 
                        return pr;
                    }

                    for (int i = 0; i < resultArr.Count; i = i + 12)
                    {
                        Part onlyPart = new Part();
                        
                        onlyPart.PartID = long.Parse(resultArr[i + 0].ToString());
                        onlyPart.CodePartFull = long.Parse(resultArr[i + 1].ToString());
                        onlyPart.PartialCode = long.Parse(resultArr[i + 2].ToString());
                        onlyPart.SN = resultArr[i + 3].ToString();
                        onlyPart.CN = resultArr[i + 4].ToString();
                        onlyPart.DateIn = resultArr[i + 5].ToString();
                        onlyPart.DateOut = resultArr[i + 6].ToString();
                        onlyPart.DateSend = resultArr[i + 7].ToString();
                        onlyPart.StorageID = long.Parse(resultArr[i + 8].ToString());
                        onlyPart.State = resultArr[i + 9].ToString();
                        onlyPart.CompanyO = resultArr[i + 10].ToString();
                        onlyPart.CompanyC = resultArr[i + 11].ToString();

                        pr.Add(onlyPart);
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            return pr;
        }

        public List<Part> GetListOfPartsFromPartsPartsPoslanoPartsZamijenjenoByID(List<long> mPartsID)
        {

            List<Part> pr = new List<Part>();
            List<String> resultArr = new List<string>();
            QueryCommands qc = new QueryCommands();
            try
            {
                for (int k = 0; k < mPartsID.Count; k++)
                {
                    resultArr = qc.GetListPartsByPartIDFromPartsPoslano(mPartsID[k]);
                    if (resultArr[0].Equals("nok"))
                        resultArr = qc.GetListPartsByPartIDFromParts(mPartsID[k]);
                    else if (resultArr[0].Equals("nok"))
                        resultArr = qc.GetListPartsByPartIDFromPartsZamijenjeno(mPartsID[k]);
                    else if(resultArr[0].Equals("nok"))
                    {
                        pr.Clear();
                        return pr;
                    }

                    for (int i = 0; i < resultArr.Count; i = i + 12)
                    {
                        Part onlyPart = new Part();

                        onlyPart.PartID = long.Parse(resultArr[i + 0].ToString());
                        onlyPart.CodePartFull = long.Parse(resultArr[i + 1].ToString());
                        onlyPart.PartialCode = long.Parse(resultArr[i + 2].ToString());
                        onlyPart.SN = resultArr[i + 3].ToString();
                        onlyPart.CN = resultArr[i + 4].ToString();
                        onlyPart.DateIn = resultArr[i + 5].ToString();
                        onlyPart.DateOut = resultArr[i + 6].ToString();
                        onlyPart.DateSend = resultArr[i + 7].ToString();
                        onlyPart.StorageID = long.Parse(resultArr[i + 8].ToString());
                        onlyPart.State = resultArr[i + 9].ToString();
                        onlyPart.CompanyO = resultArr[i + 10].ToString();
                        onlyPart.CompanyC = resultArr[i + 11].ToString();

                        pr.Add(onlyPart);
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            return pr;
        }

        public Part GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(long mPartID)
        {
            Part onlyPart = new Part();

            if (mPartID != 0)
            {
                List<String> resultArr = new List<string>();
                QueryCommands qc = new QueryCommands();
                try
                {
                    resultArr = qc.GetListPartsByPartIDFromPartsPoslano(mPartID);

                    if (resultArr[0].Equals("nok"))
                    {
                        resultArr.Clear();
                        resultArr = qc.GetListPartsByPartIDFromParts(mPartID);
                    }

                    if (resultArr[0].Equals("nok"))
                    {
                        resultArr.Clear();
                        resultArr = qc.GetListPartsByPartIDFromPartsZamijenjeno(mPartID);
                    }

                    if (resultArr[0].Equals("nok"))
                        return onlyPart;

                    onlyPart.PartID = long.Parse(resultArr[0].ToString());
                    onlyPart.CodePartFull = long.Parse(resultArr[1].ToString());
                    onlyPart.PartialCode = long.Parse(resultArr[2].ToString());
                    onlyPart.SN = resultArr[3].ToString();
                    onlyPart.CN = resultArr[4].ToString();
                    onlyPart.DateIn = resultArr[5].ToString();
                    onlyPart.DateOut = resultArr[6].ToString();
                    onlyPart.DateSend = resultArr[7].ToString();
                    onlyPart.StorageID = long.Parse(resultArr[8].ToString());
                    onlyPart.State = resultArr[9].ToString();
                    onlyPart.CompanyO = resultArr[10].ToString();
                    onlyPart.CompanyC = resultArr[11].ToString();
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    throw;
                }
            }
            return onlyPart;
        }

        public List<Part> GetListOfPartsOTPParts(long mOTPID)
        {
            List<long> backArr = new List<long>();
            List<String> resultArr = new List<string>();
            List<Part> pr = new List<Part>();
            QueryCommands qc = new QueryCommands();
            try
            {
                //for (int i = 0; i < mNumberOfParts; i++)
                int ii = 0;
                do
                {
                    resultArr = qc.GetPartsIDSbyOpenedOTP(WorkingUser.Username, WorkingUser.Password, mOTPID);
                    if (resultArr[0] != "nok")
                        backArr.Add(long.Parse(resultArr[ii].ToString()));
                    ii++;
                } while (ii < resultArr.Count);

                for (int k = 0; k < backArr.Count; k++)
                {
                    resultArr.Clear();
                    resultArr = qc.GetPartsByOTPID(WorkingUser.Username, WorkingUser.Password, backArr[k]);

                    if (resultArr[0] != "nok")
                    {
                        for (int i = 0; i < resultArr.Count; i = i + 12)
                        {
                            Part onlyPart = new Part();
                            onlyPart.PartID = long.Parse(resultArr[0].ToString());
                            onlyPart.CodePartFull = long.Parse(resultArr[1].ToString());
                            onlyPart.PartialCode = long.Parse(resultArr[2].ToString());
                            onlyPart.SN = resultArr[3].ToString();
                            onlyPart.CN = resultArr[4].ToString();
                            onlyPart.DateIn = resultArr[5].ToString();
                            onlyPart.DateOut = resultArr[6].ToString();
                            onlyPart.DateSend = resultArr[7].ToString();
                            onlyPart.StorageID = long.Parse(resultArr[8].ToString());
                            onlyPart.State = resultArr[9].ToString();
                            onlyPart.CompanyO = resultArr[10].ToString();
                            onlyPart.CompanyC = resultArr[11].ToString();
                            pr.Add(onlyPart);
                        }
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            return pr;
        }

        public Boolean Clear()
        {
            Boolean isExecuted = false;
            try
            {
                partID = 0;
                codePartFull = 0;
                partialCode = 0;
                sN = "";
                cN = "";
                dateIn = "";
                dateOut = "";
                dateSend = "";
                storageID = 0;
                state = "";
                companyO = "";
                companyC = "";

                isExecuted = true;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }

            return isExecuted;
        }

        public long PartID
        {
            get
            {
                return partID;
            }
            set
            {
                partID = value;
            }
        }
        public long CodePartFull
        {
            get
            {
                return codePartFull;
            }
            set
            {
                if (value.ToString().Length >= 12 && value.ToString().Length < 14)
                {
                    codePartFull = value;
                }
            }

        }

        public long PartialCode
        {
            get
            {
                return partialCode;
            }
            set
            {
                if (value.ToString().Length >= 7 && value.ToString().Length < 10)
                {
                    partialCode = value;
                }
            }
        }
        public String SN
        {
            get
            {
                return sN;
            }
            set
            {
                try
                {
                    sN = value.ToUpper().Trim();
                }
                catch { sN = ""; }
            }

        }

        public String CN
        {
            get
            {
                return cN;
            }
            set
            {
                try
                {
                    cN = value.ToUpper().Trim();
                }
                catch { cN = ""; }
            }
        }

        public String DateIn
        {
            get
            {
                return dateIn;
            }
            set
            {
                dateIn = string.Format("{0:dd.MM.yy.}", value.ToUpper());
            }

        }

        public String DateOut
        {
            get
            {
                return dateOut;
            }
            set
            {
                dateOut = string.Format("{0:dd.MM.yy.}", value.ToUpper());

            }

        }

        public String DateSend
        {
            get
            {
                return dateSend;
            }
            set
            {
                dateSend = string.Format("{0:dd.MM.yy.}", value.ToUpper());
            }

        }

        public long StorageID
        {
            get
            {
                return storageID;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 100)
                    storageID = value;
            }
        }

        public String State
        {
            get
            {
                return state;
            }
            set
            {
                state = value.ToLower().Trim();
            }
        }

        public String CompanyO
        {
            get
            {
                return companyO;
            }
            set
            {
                companyO = string.Format("{0:00}", value);
            }

        }

        public String CompanyC
        {
            get
            {
                return companyC;
            }
            set
            {
                companyC = string.Format("{0:00}", value);
            }

        }
    }
}
