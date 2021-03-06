﻿using POT.WorkingClasses;
using System;
using System.Collections.Generic;

namespace POT.MyTypes
{
    public class PartSifrarnik
    {
        private long categoryCode;
        private String categoryName;
        private long partCode;
        private String partName;
        private long subPartCode;
        private String subPartName;
        private String partNumber;
        private decimal priceInKn;
        private decimal priceOutKn;
        private decimal priceInEur;
        private decimal priceOutEur;
        private long fullCode;
        private String fullName;
        private String packing;
        private Boolean isInitialized;

        public Boolean GetPart(String mCode)
        {
            try
            {
                QueryCommands qc = new QueryCommands();
                PartSifrarnik dr = qc.PartInfoByFullCodeSifrarnik(long.Parse(mCode));
                
                if (dr.FullName != null)
                {
                    CategoryCode = dr.CategoryCode;
                    CategoryName = dr.CategoryName;
                    PartCode = dr.PartCode;
                    PartName = dr.PartName;
                    SubPartCode = dr.SubPartCode;
                    SubPartName = dr.SubPartName;
                    PartNumber = dr.PartNumber;
                    PriceInKn = dr.PriceInKn;
                    PriceOutKn = dr.PriceOutKn;
                    PriceInEur = dr.PriceInEur;
                    PriceOutEur = dr.PriceOutEur;
                    FullCode = dr.FullCode;
                    FullName = dr.FullName;
                    Packing = dr.Packing;
                    IsInitialized = dr.IsInitialized;
                }
                else
                {
                    IsInitialized = false;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            return IsInitialized;
        }

        public List<PartSifrarnik> GetPartsFullSifrarnik()
        {
            try
            {
                QueryCommands qc = new QueryCommands();
                List<PartSifrarnik> prs = qc.GetPartsAllSifrarnik();
                return prs;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
        }

        public List<PartSifrarnik> GetPartsAllSifrarnikSortByFullName()
        {
            try
            {
                QueryCommands qc = new QueryCommands();
                List<PartSifrarnik> prs = qc.GetPartsAllSifrarnikSortByFullName();
                return prs;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
        }


        public long CategoryCode
        {
            get
            {
                return categoryCode;
            }
            set
            {
                if(value.ToString().Length > 6 && value.ToString().Length < 10)
                {
                    categoryCode = value;
                }
            }
        }
        public String CategoryName
        {
            get
            {
                return categoryName;
            }
            set
            {
                categoryName = value.ToUpper();
            }

        }

        public long PartCode
        {
            get
            {
                return partCode;
            }
            set
            {
                if (value.ToString().Length > 3 && value.ToString().Length < 7)
                {
                    partCode = value;
                }
            }
        }
        public String PartName
        {
            get
            {
                return partName;
            }
            set
            {
                if (!value.Equals(""))
                {
                    partName = value;
                }
                
            }

        }

        public long SubPartCode
        {
            get
            {
                return subPartCode;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 4)
                {
                    subPartCode = value;
                }
            }
        }

        public String PartNumber
        {
            get
            {
                return partNumber;
            }
            set
            {
                partNumber = value.ToUpper();
            }

        }

        public String Packing
        {
            get
            {
                return packing;
            }
            set
            {
                if (value.Equals("kom") || value.Equals("pak") || value.Equals("kut") || value.Equals("sat") || value.Equals("dan") || value.Equals("mje") || value.Equals("god"))
                {
                    packing = value;
                }

            }

        }

        public String SubPartName
        {
            get
            {
                return subPartName;
            }
            set
            {
                subPartName = value;
            }

        }

        public decimal PriceInKn
        {
            get
            {
                return priceInKn;
            }
            set
            {
                priceInKn = value;
            }
        }

        public decimal PriceOutKn
        {
            get
            {
                return priceOutKn;
            }
            set
            {
                priceOutKn = value;
            }
        }

        public decimal PriceInEur
        {
            get
            {
                return priceInEur;
            }
            set
            {
                priceInEur = value;
            }
        }

        public decimal PriceOutEur
        {
            get
            {
                return priceOutEur;
            }
            set
            {
                priceOutEur = value;
            }
        }

        public long FullCode
        {
            get
            {
                return fullCode;
            }
            set
            {
                fullCode = value;
            }
        }

        public String FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
            }

        }

        public Boolean IsInitialized
        {
            get
            {
                return isInitialized;
            }
            set
            {
                isInitialized = value;
            }
        }
    }
}

//public Boolean GetPart(String mCode)
//{
//    //Part mPart = new Part();

//    QueryCommands qc = new QueryCommands();
//    List<String> resultArr = new List<string>();
//    List<String> sendArr = new List<string>();

//    ConnectionHelper cn = new ConnectionHelper();
//    sendArr.Add(mCode);

//    try
//    {
//        resultArr = qc.Qcommands(WorkingUser.Username, WorkingUser.Password, sendArr, "PartInfo");
//        sendArr.Clear();

//        CategoryCode    = long.Parse(resultArr[0]);
//        CategoryName    = resultArr[1];
//        PartCode        = long.Parse(resultArr[2]);
//        PartName        = resultArr[3];
//        SubPartCode     = long.Parse(resultArr[4]);
//        SubPartName     = resultArr[5];
//        PartNumber      = resultArr[6];
//        PriceInKn       = decimal.Parse(resultArr[7]);
//        PriceOutKn      = decimal.Parse(resultArr[8]);
//        PriceInEur      = decimal.Parse(resultArr[9]);
//        PriceOutEur     = decimal.Parse(resultArr[10]);
//        FullCode        = long.Parse(resultArr[11]);
//        FullName        = resultArr[12];
//        Packing         = resultArr[13];
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show(ex.Message);
//        return false;
//    }

//    return true;
//}