using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace POT.MyTypes
{
    class Branch
    {
        private long filID;
        private String tvrtkeCode = "";
        private String filNumber = "";
        private long regionID;
        private String address = "";
        private String city = "";
        private String pb = "";
        private String phone = "";
        private String country = "";


        public void SetFilByFilNumber(String mFilNumber)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.FilByFilNumber(mFilNumber);

                if (!resultArr[0].Equals("nok"))
                {
                    FilID = long.Parse(resultArr[0]);
                    TvrtkeCode = resultArr[1].Trim();
                    FilNumber = resultArr[2].Trim();
                    RegionID = long.Parse(resultArr[3].Trim());
                    Address = resultArr[4].Trim();
                    City = resultArr[5].Trim();
                    Pb = resultArr[6].Trim();
                    Phone = resultArr[7].Trim();
                    Country = resultArr[8];
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public void SetFilByTvrtkeCodeFilNumber(String mTvrtkeCode, String mFilNumber)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();

            try
            {
                resultArr = qc.FilByTvrtkeCodeFilNumber(mTvrtkeCode, mFilNumber);

                if (!resultArr[0].Equals("nok"))
                {
                    FilID = long.Parse(resultArr[0]);
                    TvrtkeCode = resultArr[1].Trim();
                    FilNumber = resultArr[2].Trim();
                    RegionID = long.Parse(resultArr[3].Trim());
                    Address = resultArr[4].Trim();
                    City = resultArr[5].Trim();
                    Pb = resultArr[6].Trim();
                    Phone = resultArr[7].Trim();
                    Country = resultArr[8];
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public List<Branch> GetFilByAddress(String mAddress)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Branch> resultArrB = new List<Branch>();

            try
            {
                resultArr = qc.FilByAddress(mAddress);
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 9)
                    {
                        Branch tempB = new Branch();

                        tempB.filID = long.Parse(resultArr[0 + i]);
                        tempB.tvrtkeCode = resultArr[1 + i].Trim();
                        tempB.filNumber = resultArr[2 + i].Trim();
                        tempB.regionID = long.Parse(resultArr[3 + i].Trim());
                        tempB.address = resultArr[4 + i].Trim();
                        tempB.city = resultArr[5 + i].Trim();
                        tempB.pb = resultArr[6 + i].Trim();
                        tempB.phone = resultArr[7 + i].Trim();
                        tempB.country = resultArr[8 + i];

                        resultArrB.Add(tempB);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }

            return resultArrB;
        }

        public List<Branch> GetFilByCity(String mCity)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Branch> resultArrB = new List<Branch>();

            try
            {
                resultArr = qc.FilByCity(mCity);
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 9)
                    {
                        Branch tempB = new Branch();

                        tempB.filID = long.Parse(resultArr[0 + i]);
                        tempB.tvrtkeCode = resultArr[1 + i].Trim();
                        tempB.filNumber = resultArr[2 + i].Trim();
                        tempB.regionID = long.Parse(resultArr[3 + i].Trim());
                        tempB.address = resultArr[4 + i].Trim();
                        tempB.city = resultArr[5 + i].Trim();
                        tempB.pb = resultArr[6 + i].Trim();
                        tempB.phone = resultArr[7 + i].Trim();
                        tempB.country = resultArr[8 + i];

                        resultArrB.Add(tempB);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }

            return resultArrB;
        }

        public List<Branch> GetFilByCountry(String mCountry)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Branch> resultArrB = new List<Branch>();

            try
            {
                resultArr = qc.FilByCountry(mCountry);
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 9)
                    {
                        Branch tempB = new Branch();

                        tempB.filID = long.Parse(resultArr[0 + i]);
                        tempB.tvrtkeCode = resultArr[1 + i].Trim();
                        tempB.filNumber = resultArr[2 + i].Trim();
                        tempB.regionID = long.Parse(resultArr[3 + i].Trim());
                        tempB.address = resultArr[4 + i].Trim();
                        tempB.city = resultArr[5 + i].Trim();
                        tempB.pb = resultArr[6 + i].Trim();
                        tempB.phone = resultArr[7 + i].Trim();
                        tempB.country = resultArr[8 + i];

                        resultArrB.Add(tempB);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }

            return resultArrB;
        }

        public List<Branch> GetFilByRegionID(long mRegionID)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Branch> resultArrB = new List<Branch>();

            try
            {
                resultArr = qc.FilByRegionID(mRegionID);
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 9)
                    {
                        Branch tempB = new Branch();

                        tempB.filID = long.Parse(resultArr[0 + i]);
                        tempB.tvrtkeCode = resultArr[1 + i].Trim();
                        tempB.filNumber = resultArr[2 + i].Trim();
                        tempB.regionID = long.Parse(resultArr[3 + i].Trim());
                        tempB.address = resultArr[4 + i].Trim();
                        tempB.city = resultArr[5 + i].Trim();
                        tempB.pb = resultArr[6 + i].Trim();
                        tempB.phone = resultArr[7 + i].Trim();
                        tempB.country = resultArr[8 + i];

                        resultArrB.Add(tempB);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }

            return resultArrB;
        }

        public List<Branch> GetAllFilInfoSortByFilNumber()
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<Branch> resultArrB = new List<Branch>();

            try
            {
                resultArr = qc.AllFilInfoSortByFilNumber();
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 9)
                    {
                        Branch tempB = new Branch();

                        tempB.filID = long.Parse(resultArr[0 + i]);
                        tempB.tvrtkeCode = resultArr[1 + i].Trim();
                        tempB.filNumber = resultArr[2 + i].Trim();
                        tempB.regionID = long.Parse(resultArr[3 + i].Trim());
                        tempB.address = resultArr[4 + i].Trim();
                        tempB.city = resultArr[5 + i].Trim();
                        tempB.pb = resultArr[6 + i].Trim();
                        tempB.phone = resultArr[7 + i].Trim();
                        tempB.country = resultArr[8 + i];

                        resultArrB.Add(tempB);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter(ex);
                MessageBox.Show(ex.Message);
            }

            return resultArrB;
        }

        public void Clear()
        {
            filID = 0;
            tvrtkeCode = "";
            filNumber = "";
            regionID = 0;
            address = "";
            city = "";
            pb = "";
            phone = "";
            country = "";
        }

        public long FilID
        {
            get
            {
                return filID;
            }
            set
            {
                if (value > 0)
                    filID = value;
            }
        }
        public String TvrtkeCode
        {
            get
            {
                return tvrtkeCode;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 5)
                    tvrtkeCode = value;
                else
                    tvrtkeCode = "";
            }
        }

        public String FilNumber
        {
            get
            {
                return filNumber;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    filNumber = value.ToString().ToUpper();
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
                if (value > 0)
                    regionID = value;
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
                else
                    address = "";

            }
        }

        public String Pb
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

        public String Phone
        {
            get
            {
                return phone;
            }
            set
            {
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    phone = value.ToString().ToUpper();
                else
                    phone = "";
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
                if (value.ToString().Length > 0 && value.ToString().Length < 51)
                    country = value.ToUpper();
                else
                    country = "";
            }
        }     
    }
}
