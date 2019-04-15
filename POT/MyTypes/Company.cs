using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace POT.MyTypes
{
    class Company
    {
        private long id;
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
        private long count;


        public Boolean GetCompanyByName(String mName)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.CompanyInfoByName(mName);

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8]);
                EUR = decimal.Parse(resultArr[9]);
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                RegionID = long.Parse(resultArr[12]);
                Email = resultArr[13].Trim();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                return false;
            }

            return true;
        }

        public Boolean GetCompanyInfoByCode(String mCode)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.CompanyInfoByCode(WorkingUser.Username, WorkingUser.Password, mCode);

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8]);
                EUR = decimal.Parse(resultArr[9]);
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                RegionID = long.Parse(resultArr[12]);
                Email = resultArr[13].Trim();

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

        public Boolean GetCompanyInfoByID(long mID)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.CompanyInfoByID(WorkingUser.Username, WorkingUser.Password, mID);

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8]);
                EUR = decimal.Parse(resultArr[9]);
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                RegionID = long.Parse(resultArr[12]);
                Email = resultArr[13].Trim();

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

        public Boolean GetCompanyInfoByRegionID(String mRegionID)
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.CompanyInfoByRegionID(WorkingUser.Username, WorkingUser.Password, long.Parse(mRegionID));

                ID = long.Parse(resultArr[0]);
                Name = resultArr[1].Trim();
                Address = resultArr[2].Trim();
                City = resultArr[3].Trim();
                PB = resultArr[4].Trim();
                OIB = resultArr[5].Trim();
                Contact = resultArr[6].Trim();
                BIC = resultArr[7].Trim();
                KN = decimal.Parse(resultArr[8]);
                EUR = decimal.Parse(resultArr[9]);
                Code = resultArr[10].Trim();
                Country = resultArr[11].Trim();
                RegionID = long.Parse(resultArr[12]);
                Email = resultArr[13].Trim();

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

        public List<Company> GetAllCompanyInfoSortCode()
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Company> resultArrC = new List<Company>();

            try
            {
                resultArr = qc.AllCompanyInfoSortCode(WorkingUser.Username, WorkingUser.Password);

                for(int i = 0; i < resultArr.Count(); i = i + 14)
                {
                    Company tempC = new Company();

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
                    tempC.RegionID = long.Parse(resultArr[i + 12]);
                    tempC.Email = resultArr[i + 13].Trim();

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

        public List<Company> GetAllCompanyInfoSortByName()
        {
            //Part mPart = new Part();

            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Company> resultArrC = new List<Company>();

            try
            {
                resultArr = qc.AllCompanyInfoSortCode(WorkingUser.Username, WorkingUser.Password);

                for (int i = 0; i < resultArr.Count(); i = i + 14)
                {
                    Company tempC = new Company();
                    if (i == 140)
                        i = 140;
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
                    tempC.RegionID = long.Parse(resultArr[i + 12]);
                    tempC.Email = resultArr[i + 13].Trim();

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

        public void Clear()
        {
            id = 0;
            name = "";
            address = "";
            city = "";
            pb = "";
            oib = "";
            contact = "";
            bic = "";
            kn = 0;
            eur = 0;
            code = "";
            country = "";
            regionID = 0;
            email = "";
            count = 0;
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

        public long Count
        {
            get
            {
                return count;
            }
        }
    }
}