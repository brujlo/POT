using System;
using System.Collections.Generic;

namespace POT.MyTypes
{
    class Part
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

                resultArr = qc.ListPartsByCodeRegionState(WorkingUser.Username, WorkingUser.Password, mCodePartFull, mStorageID, mState);

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
            catch (Exception)
            {
                throw;
            }
            return pr;
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
            catch (Exception)
            {
                throw;
            }
            return pr;
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
                sN = value.ToUpper().Trim();
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
                cN = value.ToUpper().Trim();
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
