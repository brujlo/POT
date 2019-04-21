using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace POT.MyTypes
{
    class MainCmp
    {
        private long   id;
        private String name = "";
        private String address = "";
        private String city = "";
        private String pb = "";
        private String oib = "";
        private String contact = "";
        private String bic = "";
        private decimal kn;
        private decimal eur;
        private String code = "";
        private String country = "";
        private long regionID;
        private String email = "";
        private String phone = "";
        private String www = "";
        private String mb = "";
        private String iban = "";
        private String supportEmail = "";
        
        private long count;

        public Boolean GetMainCmpByName(String mName)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.MainCmpInfoByName(mName);

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8].Trim());
                EUR = decimal.Parse(resultArr[9].Trim());
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                regionID = long.Parse(resultArr[12].Trim());
                Email = resultArr[13].Trim();
                Phone = resultArr[14].Trim();
                WWW = resultArr[15].Trim();
                MB = resultArr[16].Trim();
                IBAN = resultArr[17];
                SupportEmail = resultArr[18].Trim();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return false;
            }

            return true;
        }

        public Boolean GetMainCmpInfoByCode(String mCode)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.MainCmpInfoByCode(mCode);

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8].Trim());
                EUR = decimal.Parse(resultArr[9].Trim());
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                regionID = long.Parse(resultArr[12].Trim());
                Email = resultArr[13].Trim();
                Phone = resultArr[14].Trim();
                WWW = resultArr[15].Trim();
                MB = resultArr[16].Trim();
                IBAN = resultArr[17];
                SupportEmail = resultArr[18].Trim();

                count++;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return false;
            }

            return true;
        }

        public Boolean GetMainCmpInfoByID(long mID)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.MainCmpInfoByID(mID);

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8].Trim());
                EUR = decimal.Parse(resultArr[9].Trim());
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                regionID = long.Parse(resultArr[12].Trim());
                Email = resultArr[13].Trim();
                Phone = resultArr[14].Trim();
                WWW = resultArr[15].Trim();
                MB = resultArr[16].Trim();
                IBAN = resultArr[17];
                SupportEmail = resultArr[18].Trim();

                count++;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return false;
            }

            return true;
        }

        public Boolean GetMainCmpInfoByRegionID(String mRegionID)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.MainCmpInfoByRegionID(long.Parse(mRegionID));

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8].Trim());
                EUR = decimal.Parse(resultArr[9].Trim());
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                regionID = long.Parse(resultArr[12].Trim());
                Email = resultArr[13].Trim();
                Phone = resultArr[14].Trim();
                WWW = resultArr[15].Trim();
                MB = resultArr[16].Trim();
                IBAN = resultArr[17];
                SupportEmail = resultArr[18].Trim();

                count++;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return false;
            }

            return true;
        }

        public List<MainCmp> GetAllMainCmpInfoSortCode()
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<MainCmp> resultArrC = new List<MainCmp>();

            try
            {
                resultArr = qc.AllMainCmpInfoSortCode();

                for (int i = 0; i < resultArr.Count(); i = i + 19)
                {
                    MainCmp tempC = new MainCmp();

                    tempC.ID            = long.Parse(resultArr[i + 0]);
                    tempC.Name          = resultArr[i + 1].Trim();
                    tempC.Address       = resultArr[i + 2].Trim();
                    tempC.City          = resultArr[i + 3].Trim();
                    tempC.PB            = resultArr[i + 4].Trim();
                    tempC.OIB           = resultArr[i + 5].Trim();
                    tempC.Contact       = resultArr[i + 6].Trim();
                    tempC.BIC           = resultArr[i + 7].Trim();
                    tempC.KN            = decimal.Parse(resultArr[i + 8]);
                    tempC.EUR           = decimal.Parse(resultArr[i + 9]);
                    tempC.Code          = resultArr[i + 10].Trim();
                    tempC.Country       = resultArr[i + 11].Trim();
                    tempC.RegionID      = long.Parse(resultArr[i + 12].Trim());
                    tempC.Email         = resultArr[i + 13].Trim();
                    tempC.Phone         = resultArr[i + 14].Trim();
                    tempC.WWW           = resultArr[i + 15].Trim();
                    tempC.MB            = resultArr[i + 16].Trim();
                    tempC.IBAN          = resultArr[i + 17].Trim();
                    tempC.SupportEmail  = resultArr[i + 18].Trim();

                    resultArrC.Add(tempC);
                    count++;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            return resultArrC;
        }

        public List<MainCmp> GetAllMainCmpInfoSortByName()
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<MainCmp> resultArrC = new List<MainCmp>();

            try
            {
                resultArr = qc.AllMainCmpInfoSortCode();

                for (int i = 0; i < resultArr.Count(); i = i + 19)
                {
                    MainCmp tempC = new MainCmp();
                    tempC.ID = long.Parse(resultArr[i + 0]);
                    tempC.Name = resultArr[i + 1].Trim();
                    tempC.Address = resultArr[i + 2].Trim();
                    tempC.City = resultArr[i + 3].Trim();
                    tempC.PB = resultArr[i + 4].Trim();
                    tempC.OIB = resultArr[i + 5].Trim();
                    tempC.Contact = resultArr[i + 6].Trim();
                    tempC.BIC = resultArr[i + 7].Trim();
                    tempC.KN = decimal.Parse(resultArr[i + 8]);
                    tempC.EUR = decimal.Parse(resultArr[i + 9]);
                    tempC.Code = resultArr[i + 10].Trim();
                    tempC.Country = resultArr[i + 11].Trim();
                    tempC.RegionID = long.Parse(resultArr[i + 12].Trim());
                    tempC.Email = resultArr[i + 13].Trim();
                    tempC.Phone = resultArr[i + 14].Trim();
                    tempC.WWW = resultArr[i + 15].Trim();
                    tempC.MB = resultArr[i + 16].Trim();
                    tempC.IBAN = resultArr[i + 17].Trim();
                    tempC.SupportEmail = resultArr[i + 18].Trim();

                    resultArrC.Add(tempC);
                    count++;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            return resultArrC;
        }

        public Company MainCmpToCompany()
        {
            //Part mPart = new Part();

            Company cmp = new Company();

            try
            {
                cmp.ID = ID;
                cmp.Name = Name;
                cmp.Address = Address;
                cmp.City = City;
                cmp.PB = PB;
                cmp.OIB = OIB;
                cmp.Contact = Contact;
                cmp.BIC = BIC;
                cmp.KN = KN;
                cmp.EUR = EUR;
                cmp.Code = Code;
                cmp.Country = Country;
                cmp.RegionID = RegionID;
                cmp.Email = Email;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            return cmp;
        }

        public void Clear()
        {
            ID = 0;
            Name = "";
            Address = "";
            City = "";
            PB = "";
            OIB = "";
            Contact = "";
            BIC = "";
            KN = 0;
            EUR = 0;
            Code = "";
            Country = "";
            regionID = 0;
            Email = "";
            Phone = "";
            WWW = "";
            MB = "";
            IBAN = "";
            SupportEmail = "";
        }

        public Boolean HasItems()
        {
            if (count > 0)
                return true;
            else
                return false;
        }

        public long ID
        {
            get
            {
                return id;
            }
            set
            {
                if (value > 0)
                    id = value;
            }
        }
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    name = value;
                else
                    name = "";
            }

        }

        public String Address
        {
            get
            {
                return address;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    address = value.First().ToString().ToUpper() + value.Substring(1);
            }
        }

        public String WWW
        {
            get
            {
                return www;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    www = value;
                else
                    www = "";
            }
        }

        public String Phone
        {
            get
            {
                return phone;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    phone = value;
                else
                    phone = "";
            }

        }

        public String Email
        {
            get
            {
                return email;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    email = value;
                else
                    email = "";
            }

        }

        public String MB
        {
            get
            {
                return mb;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    mb = value.ToUpper();
                else
                    mb = "";
            }

        }

        public String IBAN
        {
            get
            {
                return iban;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    iban = value.ToUpper();
                else
                    iban = "";
            }
        }

        public String SupportEmail
        {
            get
            {
                return supportEmail;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    supportEmail = value;
                else
                    supportEmail = "";
            }
        }

        public String City
        {
            get
            {
                return city;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    city = value.First().ToString().ToUpper() + value.Substring(1);
                else
                    city = "";

            }

        }

        public String PB
        {
            get
            {
                return pb;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    pb = value.ToUpper();
                else
                    pb = "";
            }
        }

        public String OIB
        {
            get
            {
                return oib;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    oib = value.ToUpper();
                else
                    oib = "";
            }

        }

        public String Contact
        {
            get
            {
                return contact;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    contact = value.First().ToString().ToUpper() + value.Substring(1);
                else
                    contact = "";
            }

        }

        public String BIC
        {
            get
            {
                return bic;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    bic = value.ToUpper();
                else
                    bic = "";
            }

        }

        public decimal KN
        {
            get
            {
                return kn;
            }
            set
            {
                kn = value;
            }
        }

        public decimal EUR
        {
            get
            {
                return eur;
            }
            set
            {
                eur = value;
            }
        }

        public String Code
        {
            get
            {
                return code;
            }
            set
            {
                if (value.Length > 0 && value.Length < 3)
                {
                    if (value.ToString().Length == 1)
                        code = "0" + value;
                    else
                        code = value;
                }
            }
        }

        public String Country
        {
            get
            {
                return country;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 11)
                    country = value.ToUpper();
                else
                    country = "";
            }
        }

        public long RegionID
        {
            get
            {
                return regionID;
            }
            set
            {
                regionID = value;
            }
        }

        public long Count
        {
            get
            {
                return count;
            }
        }
    }
}
