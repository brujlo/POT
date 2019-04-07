﻿using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POT
{
    class QueryCommands
    {
        SqlCommand command;
        String query;
        ConnectionHelper cn = new ConnectionHelper();
        SqlConnection cnn = new SqlConnection();

        public Boolean ResetAutoIcrement(String Uname, String Pass, List<String> TableArr)
        {
            Boolean executed = false;
            using (SqlConnection cnn1 = cn.Connect(Uname, Pass))
            {

                command = cnn1.CreateCommand();
                SqlTransaction transaction = cnn1.BeginTransaction();
                command.Connection = cnn1;
                command.Transaction = transaction;

                if (TableArr.Count > 0)
                {
                    try
                    {
                        for (int i = 0; i < TableArr.Count; i++)
                        {
                            command.CommandText = "DBCC CHECKIDENT ('[" + TableArr[i] + "]', RESEED, 0)";
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        executed = true;
                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        try
                        {
                            transaction.Rollback();
                            executed = false;
                            throw;
                        }
                        catch (Exception e2)
                        {
                            new LogWriter(e2);
                            MessageBox.Show(e2.Message);
                        }
                    }
                    finally
                    {
                        if (cnn1.State.ToString().Equals("Open"))
                            cnn1.Close();
                    }
                }
                return executed;
            }
        }

        public List<String> SetWorkingDBUser(String Uname, String Pass)
        {
            List<String> arr = new List<string>();

            CriptMe cm = new CriptMe();
            String hashPswd = cm.Cript(Pass);
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Users where Username = '" + Uname + "' and HashPswd = '" + hashPswd + "'";
            //query = "Select * from Users where Username = '" + Uname + "' and Password = '" + Pass + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();

            if (cn.TestConnection(cnn))
            {    
                dataReader.Read();

                if (dataReader.HasRows)
                {
                    //WorkingUser.UserID = Int32.Parse(dataReader["UserID"].ToString());
                    WorkingUser.UserID = (long) dataReader.GetDecimal(0);
                    WorkingUser.Name = dataReader.GetString(1);
                    WorkingUser.Surename = dataReader.GetString(2);
                    WorkingUser.Username = dataReader.GetString(3);
                    WorkingUser.Password = dataReader.GetString(4);
                    WorkingUser.Phone = dataReader.GetString(5);
                    WorkingUser.Email = dataReader.GetString(6);
                    WorkingUser.RegionID = (long) dataReader.GetDecimal(7);
                    WorkingUser.AdminRights = dataReader.GetDecimal(8);
                }
                arr.Add(dataReader["UserID"].ToString());
                dataReader.Close();
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> User(String Uname, String Pass, String mUserID)
        {
            List<String> arr = new List<string>();

            query = "Select * from Users where UserID = " + int.Parse(mUserID);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = new SqlCommand(query, cnn);
            dataReader.Read();

            if (cn.TestConnection(cnn))
            {
                if (dataReader.HasRows)
                {
                    arr.Add(dataReader.GetDecimal(0).ToString("0.00").Replace(".00", String.Empty));
                    arr.Add(dataReader.GetString(1));
                    arr.Add(dataReader.GetString(2));
                    arr.Add(dataReader.GetString(3));
                    arr.Add(dataReader.GetString(4));
                    arr.Add(dataReader.GetString(5));
                    arr.Add(dataReader.GetString(6));
                    arr.Add(dataReader.GetDecimal(7).ToString("0.00").Replace(".00", String.Empty));
                }
                else
                {
                    arr.Add("nok");
                }
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public void NewDBUser(String Uname, String Pass, List<String> value)
        {
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            if (value[2] != "")
            {
                try
                {
                    CriptMe cm = new CriptMe();
                    String hashPswd = cm.Cript(value[3]);

                    command.CommandText = "CREATE LOGIN " + value[2] + " WITH PASSWORD = '" + value[3] + "'";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE USER " + value[2] + " FOR LOGIN " + value[2];
                    command.ExecuteNonQuery();
                    command.CommandText = "GRANT INSERT,UPDATE,DELETE,SELECT,EXECUTE ON SCHEMA :: dbo TO " + value[2];
                    command.ExecuteNonQuery();
                    command.CommandText = "Insert into Users(Name, Surename, Username, Password, Phone, Email, RegionID, AdminRights, HashPswd) values('" + value[0] + "', '" + value[1] + "', '" + value[2] + "', '" + value[3] + "', '" + value[4] + "', '" + value[5] + "', " + value[6] + ", " + value[7] + ", '" + hashPswd + "')";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        cnn.Close();
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            cnn.Close();
        }

        public Boolean DeleteDBUser(String Uname, String Pass, String mUserName, String mUserId)
        {
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = cnn.CreateCommand();
            command.Connection = cnn;

            if (!mUserName.Equals("") && !mUserId.Equals(""))
            {
                try
                {
                    command.CommandText = "ALTER DATABASE [CP] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    command.ExecuteNonQuery();

                    command.CommandText = "EXEC sp_dropuser '" + mUserName + "'";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP LOGIN " + mUserName;
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM Users WHERE UserID='" + mUserId + "'";
                    command.ExecuteNonQuery();

                    command.CommandText = "ALTER DATABASE [CP] SET MULTI_USER";
                    command.ExecuteNonQuery();

                    cnn.Close();
                    return true;
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        command.CommandText = "ALTER DATABASE [CP] SET MULTI_USER";
                        command.ExecuteNonQuery();
                        cnn.Close();
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        command.CommandText = "ALTER DATABASE [CP] SET MULTI_USER";
                        command.ExecuteNonQuery();
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            cnn.Close();
            return false;
        }

        public List<String> RegionInfo(String Uname, String Pass)
        {
            List<String> arr = new List<string>();

            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Regija";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["RegionID"].ToString());
                    arr.Add(dataReader["Region"].ToString());
                    arr.Add(dataReader["FullRegion"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> UsersInfo(String Uname, String Pass)
        {
            List<String> arr = new List<string>();

            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Users";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["UserID"].ToString());
                    arr.Add(dataReader["Name"].ToString());
                    arr.Add(dataReader["Surename"].ToString());
                    arr.Add(dataReader["Username"].ToString());
                    arr.Add(dataReader["Password"].ToString());
                    arr.Add(dataReader["Phone"].ToString());
                    arr.Add(dataReader["Email"].ToString());
                    arr.Add(dataReader["RegionID"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public PartSifrarnik PartInfoByFullCodeSifrarnik(String Uname, String Pass, long mFullCode)
        {
            PartSifrarnik tempPart = new PartSifrarnik();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(Uname, Pass);
                query = "Select * from Sifrarnik where FullCode = " + mFullCode;
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {  
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        tempPart.CategoryCode = long.Parse(dataReader["CategoryCode"].ToString());
                        tempPart.CategoryName = dataReader["CategoryName"].ToString();
                        tempPart.PartCode = long.Parse(dataReader["PartCode"].ToString());
                        tempPart.PartName = dataReader["PartName"].ToString();
                        tempPart.SubPartCode = long.Parse(dataReader["SubPartCode"].ToString());
                        tempPart.SubPartName = dataReader["SubPartName"].ToString();
                        tempPart.PartNumber = dataReader["PartNumber"].ToString();
                        tempPart.PriceInKn = decimal.Parse(dataReader["PriceInKn"].ToString());
                        tempPart.PriceOutKn = decimal.Parse(dataReader["PriceOutKn"].ToString());
                        tempPart.PriceInEur = decimal.Parse(dataReader["PriceInEur"].ToString());
                        tempPart.PriceOutEur = decimal.Parse(dataReader["PriceOutEur"].ToString());
                        tempPart.FullCode = long.Parse(dataReader["FullCode"].ToString());
                        tempPart.FullName = dataReader["FullName"].ToString();
                        tempPart.Packing = dataReader["Packing"].ToString();
                        tempPart.IsInitialized = true;
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return tempPart;
        }

        public List<String> ListPartsByCodeRegionStateS(String Uname, String Pass, long mCodePartFull, long mStorageID, String mState)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Parts where CodePartFull = " + mCodePartFull + " and StorageID = " + mStorageID + " and State = '" + mState + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["PartID"].ToString());
                    arr.Add(dataReader["CodePartFull"].ToString());
                    arr.Add(dataReader["PartialCode"].ToString());
                    arr.Add(dataReader["SN"].ToString());
                    arr.Add(dataReader["CN"].ToString());
                    arr.Add(dataReader["DateIn"].ToString());
                    arr.Add(dataReader["DateOut"].ToString());
                    arr.Add(dataReader["DateSend"].ToString());
                    arr.Add(dataReader["StorageID"].ToString());
                    arr.Add(dataReader["State"].ToString());
                    arr.Add(dataReader["CompanyO"].ToString());
                    arr.Add(dataReader["CompanyC"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<Part> ListPartsByCodeRegionStateP(String Uname, String Pass, long mCodePartFull, String mSN, String mCN, String mState, long mStorageID)
        {
            List<Part> arr = new List<Part>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Parts where CodePartFull = " + mCodePartFull + " and SN = '" + mSN + "' and CN = '" + mCN + "' and StorageID = " + mStorageID + " and State = '" + mState + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    Part tempPart = new Part();

                    tempPart.PartID = long.Parse(dataReader["PartID"].ToString());
                    tempPart.CodePartFull = long.Parse(dataReader["CodePartFull"].ToString());
                    tempPart.PartialCode = long.Parse(dataReader["PartialCode"].ToString());
                    tempPart.SN = dataReader["SN"].ToString();
                    tempPart.CN = dataReader["CN"].ToString();
                    tempPart.DateIn = dataReader["DateIn"].ToString();
                    tempPart.DateOut = dataReader["DateOut"].ToString();
                    tempPart.DateSend = dataReader["DateSend"].ToString();
                    tempPart.StorageID = long.Parse(dataReader["StorageID"].ToString());
                    tempPart.State = dataReader["State"].ToString();
                    tempPart.CompanyO = dataReader["CompanyO"].ToString();
                    tempPart.CompanyC = dataReader["CompanyC"].ToString();

                    arr.Add(tempPart);
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetPartsByOTPID(String Uname, String Pass, long mPartID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Parts where PartID = " + mPartID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["PartID"].ToString());
                    arr.Add(dataReader["CodePartFull"].ToString());
                    arr.Add(dataReader["PartialCode"].ToString());
                    arr.Add(dataReader["SN"].ToString());
                    arr.Add(dataReader["CN"].ToString());
                    arr.Add(dataReader["DateIn"].ToString());
                    arr.Add(dataReader["DateOut"].ToString());
                    arr.Add(dataReader["DateSend"].ToString());
                    arr.Add(dataReader["StorageID"].ToString());
                    arr.Add(dataReader["State"].ToString());
                    arr.Add(dataReader["CompanyO"].ToString());
                    arr.Add(dataReader["CompanyC"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetPartsIDSbyOpenedOTP(String Uname, String Pass, long otpID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select partID from OTPparts where otpID = " + otpID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["PartID"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> SelectNameCodeFromSifrarnik(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select FullName, FullCode from Sifrarnik order by FullName asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["FullName"].ToString());
                    arr.Add(dataReader["FullCode"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public int CountCompany(String Uname, String Pass)
        {
            int value;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(ID) from Tvrtke";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                var cnt = dataReader.GetValue(0);
                value = (int)cnt; ;
            }
            else
            {
                value = 0;
            }
            dataReader.Close();
            cnn.Close();
            return value;
        }

        public List<String> CompanyInfoByCode(String Uname, String Pass, String mCode)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Tvrtke where Code = '" + mCode + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["Name"].ToString());
                arr.Add(dataReader["Address"].ToString());
                arr.Add(dataReader["City"].ToString());
                arr.Add(dataReader["PB"].ToString());
                arr.Add(dataReader["OIB"].ToString());
                arr.Add(dataReader["Contact"].ToString());
                arr.Add(dataReader["BIC"].ToString());
                arr.Add(dataReader["KN"].ToString());
                arr.Add(dataReader["Eur"].ToString());
                arr.Add(dataReader["Code"].ToString());
                arr.Add(dataReader["Country"].ToString());
                arr.Add(dataReader["RegionID"].ToString());
                arr.Add(dataReader["email"].ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> CompanyInfoByName(String Uname, String Pass, String mName)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Tvrtke where Name = '" + mName + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["Name"].ToString());
                arr.Add(dataReader["Address"].ToString());
                arr.Add(dataReader["City"].ToString());
                arr.Add(dataReader["PB"].ToString());
                arr.Add(dataReader["OIB"].ToString());
                arr.Add(dataReader["Contact"].ToString());
                arr.Add(dataReader["BIC"].ToString());
                arr.Add(dataReader["KN"].ToString());
                arr.Add(dataReader["Eur"].ToString());
                arr.Add(dataReader["Code"].ToString());
                arr.Add(dataReader["Country"].ToString());
                arr.Add(dataReader["RegionID"].ToString());
                arr.Add(dataReader["email"].ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> CompanyInfoByRegionID(String Uname, String Pass, long mRegionID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Tvrtke where RegionID = " + mRegionID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["Name"].ToString());
                    arr.Add(dataReader["Address"].ToString());
                    arr.Add(dataReader["City"].ToString());
                    arr.Add(dataReader["PB"].ToString());
                    arr.Add(dataReader["OIB"].ToString());
                    arr.Add(dataReader["Contact"].ToString());
                    arr.Add(dataReader["BIC"].ToString());
                    arr.Add(dataReader["KN"].ToString());
                    arr.Add(dataReader["Eur"].ToString());
                    arr.Add(dataReader["Code"].ToString());
                    arr.Add(dataReader["Country"].ToString());
                    arr.Add(dataReader["RegionID"].ToString());
                    arr.Add(dataReader["email"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> AllCompanyInfoSortCode(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Tvrtke order by Code asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["Name"].ToString());
                    arr.Add(dataReader["Address"].ToString());
                    arr.Add(dataReader["City"].ToString());
                    arr.Add(dataReader["PB"].ToString());
                    arr.Add(dataReader["OIB"].ToString());
                    arr.Add(dataReader["Contact"].ToString());
                    arr.Add(dataReader["BIC"].ToString());
                    arr.Add(dataReader["KN"].ToString());
                    arr.Add(dataReader["Eur"].ToString());
                    arr.Add(dataReader["Code"].ToString());
                    arr.Add(dataReader["Country"].ToString());
                    arr.Add(dataReader["RegionID"].ToString());
                    arr.Add(dataReader["email"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> AllCompanyInfoSortByName(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Tvrtke order by Name asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["Name"].ToString());
                    arr.Add(dataReader["Address"].ToString());
                    arr.Add(dataReader["City"].ToString());
                    arr.Add(dataReader["PB"].ToString());
                    arr.Add(dataReader["OIB"].ToString());
                    arr.Add(dataReader["Contact"].ToString());
                    arr.Add(dataReader["BIC"].ToString());
                    arr.Add(dataReader["KN"].ToString());
                    arr.Add(dataReader["Eur"].ToString());
                    arr.Add(dataReader["Code"].ToString());
                    arr.Add(dataReader["Country"].ToString());
                    arr.Add(dataReader["RegionID"].ToString());
                    arr.Add(dataReader["email"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllRegions(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Regija";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["RegionID"].ToString());
                    arr.Add(dataReader["Region"].ToString());
                    arr.Add(dataReader["FullRegion"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> InTransport(String Uname, String Pass, long mCodePartFull)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(CodePartFull) from Parts p where p.CodePartFull =" + mCodePartFull + " and p.StorageID = " + Properties.Settings.Default.TransportIDRegion;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                var cnt = dataReader.GetValue(0);
                arr.Add(cnt.ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public Boolean AddRegion(String Uname, String Pass, long mRegionID, String mRegion, String mFullRegion, String mAddressTB, String mCityTB, String mPBTB, String mOIBTB, String mContactTB, String mCountryTB, String mBICTB, String mEmailTB, String mCompanyCode)
        {

            Boolean executed = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(RegionID) from Regija r where r.RegionID =" + mRegionID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows && !dataReader.GetValue(0).Equals(0))
            {
                executed = false;
                dataReader.Close();
            }
            else
            {
                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                if (mRegionID != 0 && mRegion != "" && mFullRegion != "")
                {
                    try
                    {
                        command.CommandText = "INSERT INTO Regija (RegionID, Region, FullRegion) values (" + mRegionID + ", '" + mRegion + "', '" + mFullRegion + "')";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO Tvrtke (Name, Address, City, PB, OIB, Contact, BIC, KN, Eur, Code, Country, RegionID, email) values ('" + mFullRegion + "', '" + mAddressTB + "', '" + mCityTB + "', '" + mPBTB + "', '" + mOIBTB + "', '" + mContactTB + "', '" + mBICTB + "', 0, 0, '" + mCompanyCode + "', '" + mCountryTB + "', " + mRegionID + ", '" + mEmailTB  + "')";
                        command.ExecuteNonQuery();

                        transaction.Commit();

                        executed = true;
                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        try
                        {
                            transaction.Rollback();
                            executed = false;
                            throw;
                        }
                        catch (Exception e2)
                        {
                            new LogWriter(e2);
                            throw;
                        }
                    }
                    finally
                    {
                        if (cnn.State.ToString().Equals("Open"))
                            cnn.Close();
                    }
                }
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }

        public Boolean DeleteRegion(String Uname, String Pass, int mRegionID)
        {
            Boolean executed = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;
            int result = 0;

            if (mRegionID != 0)
            {
                try
                {
                    command.CommandText = "DELETE FROM Regija WHERE RegionID =  @word";
                    command.Parameters.AddWithValue("@word", mRegionID);
                    result = command.ExecuteNonQuery();
                    transaction.Commit();

                    if (result != 0)
                        executed = true;
                    else
                    {
                        executed = false;
                    }
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            cnn.Close();
            return executed;
        }

        public List<String> PartsCntG(String Uname, String Pass, long mCodePartFull, long mStorageID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(CodePartFull) from Parts p where p.CodePartFull = " + mCodePartFull + " and p.StorageID = " + mStorageID + " and p.State = 'g'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                var cnt = dataReader.GetValue(0);
                arr.Add(cnt.ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> PartsCntNG(String Uname, String Pass, long mCodePartFull, long mStorageID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(CodePartFull) from Parts p where p.CodePartFull = " + mCodePartFull + " and p.StorageID = " + mStorageID + " and p.State = 'ng'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                var cnt = dataReader.GetValue(0);
                arr.Add(cnt.ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> PartsCntNGS(String Uname, String Pass, long mCodePartFull)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(CodePartFull) from Parts p where p.CodePartFull = " + mCodePartFull + " and p.State = 'sng'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                var cnt = dataReader.GetValue(0);
                arr.Add(cnt.ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> PartsCntGS(String Uname, String Pass, long mCodePartFull)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(CodePartFull) from Parts p where p.CodePartFull = " + mCodePartFull + " and p.State = 'sg'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                var cnt = dataReader.GetValue(0);
                arr.Add(cnt.ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> CurrentExchangeRate(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "SELECT * FROM TecajnaLista WHERE id = (SELECT MAX(id) FROM TecajnaLista)";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["DateUpdated"].ToString());
                arr.Add(dataReader["KupovniE"].ToString());
                arr.Add(dataReader["SrednjiE"].ToString());
                arr.Add(dataReader["ProdajniE"].ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetPartIDCompareCodeSNCNStorage(String Uname, String Pass, long mCodePartFull, String mSN, String mCN, long mStorageID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select PartID from Parts where CodePartFull = " + mCodePartFull + " and SN = '" + mSN + "' and CN = '" + mCN + "' and StorageID = " + mStorageID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["PartID"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public int GetPartCountByCodeSNCNStateStorage(String Uname, String Pass, long mCodePartFull, String mSN, String mCN, String mState, long mStorageID)
        {
            int retValue = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select Count(PartID) from Parts where CodePartFull = " + mCodePartFull + " and SN = '" + mSN + "' and CN = '" + mCN + "' and StorageID = " + mStorageID + " and State = '" + mState + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                if (dataReader.HasRows)
                {
                    retValue = dataReader.GetInt32(0);
                }

            }

            dataReader.Close();
            cnn.Close();
            return retValue;
        }

        public List<String> GetPartIDCompareCodeSNCNStorageState(String Uname, String Pass, long mCodePartFull, String mSN, String mCN, long mStorageID, String mState)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select PartID from Parts where CodePartFull = " + mCodePartFull + " and SN = '" + mSN + "' and CN = '" + mCN + "' and StorageID = " + mStorageID + "and State = '" + mState + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["PartID"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllOpenedOTP(String Uname, String Pass, long mReciever)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select otpID from OTP where customerID = " + mReciever + " and primID is NULL";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["otpID"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public String PRIMUnesiUredajeDaSuPrimljeniInnner(String Uname, String Pass, List<Part> PartsID, long RegionS, long RegionR, long otpID, String napomena)
        {
            String executed = "nok";
            long primCnt = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(primID) from PRIM where primID LIKE '" + DateTime.Now.ToString("yy") + "%'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
            {
                executed = "nok";
                dataReader.Close();
            }
            else
            {
                primCnt = long.Parse(dataReader.GetValue(0).ToString());
                primCnt = long.Parse(DateTime.Now.ToString("yy") + string.Format("{0:000}", (primCnt + 1)));
                Properties.Settings.Default.ShareDocumentName = primCnt.ToString();

                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO PRIM (primID, customerID, dateCreated, napomena, userID) values (" + primCnt + ", " + RegionS + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', '" + napomena + "', " + WorkingUser.UserID + ")";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE OTP SET primID = " + primCnt + " where otpID = " + otpID;
                    command.ExecuteNonQuery();

                    for(int i = 0; i < PartsID.Count; i++)
                    {
                        command.CommandText = "UPDATE Parts SET StorageID = " + RegionR + " where PartID = " + PartsID[i].PartID;
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Transport SET TransportDateIn = '" + DateTime.Now.ToString("dd.MM.yy.") + "', UsersUserIDIn =  " + WorkingUser.UserID + " WHERE TransactionID = " + otpID;
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO PRIMParts (primID, partID) VALUES (" + primCnt + ", " + PartsID[i].PartID + ")";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    executed = string.Format("{0:00/000}", primCnt);
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }

        public String PRIMUnesiUredajeDaSuPrimljeni(String Uname, String Pass, List<Part> ListOfParts, long RegionIDReciever, long CustomerID, String napomena)
        {
            String executed = "nok";
            long primCnt = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(primID) from PRIM where primID LIKE '" + DateTime.Now.ToString("yy") + "%'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
            {
                executed = "nok";
                dataReader.Close();
            }
            else
            {
                primCnt = long.Parse(dataReader.GetValue(0).ToString());
                primCnt = long.Parse(DateTime.Now.ToString("yy") + string.Format("{0:000}", (primCnt + 1)));
                Properties.Settings.Default.ShareDocumentName = primCnt.ToString();

                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO PRIM (primID, customerID, dateCreated, napomena, userID) values (" + primCnt + ", " + CustomerID + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', '" + napomena + "', " + WorkingUser.UserID + ")";
                    command.ExecuteNonQuery();

                    for (int i = 0; i < ListOfParts.Count; i++)
                    {
                        command.CommandText = "INSERT INTO Parts (PartialCode, SN, CN, DateIn, DateOut, DateSend, StorageID, State, CompanyO, CompanyC) output INSERTED.PartID VALUES (" + ListOfParts[i].PartialCode + ", '" + ListOfParts[i].SN + "', '" + ListOfParts[i].CN + "', '" + DateTime.Now.ToString("dd.MM.yy.") + "', '', '', " + WorkingUser.RegionID + ", '" + ListOfParts[i].State + "', '" + ListOfParts[i].CompanyO + "', '" + ListOfParts[i].CompanyC + "')";
                        var newPartID = command.ExecuteScalar();
                        
                        command.CommandText = "INSERT INTO PRIMParts (primID, partID) VALUES (" + primCnt + ", " + newPartID + ")";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    executed = string.Format("{0:00/000}", primCnt);
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }

        public String OTPUnesiUredajeDaSuPrimljeni(String Uname, String Pass, List<Part> ListOfParts, Company cmpR, Company cmpS, String napomena)
        {
            String executed = "nok";
            long otpCnt = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(otpID) from OTP where otpID LIKE '" + DateTime.Now.ToString("yy") + "%'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
            {
                executed = "nok";
                dataReader.Close();
            }
            else
            {
                otpCnt = long.Parse(dataReader.GetValue(0).ToString());
                otpCnt = long.Parse(DateTime.Now.ToString("yy") + string.Format("{0:000}", (otpCnt + 1)));
                Properties.Settings.Default.ShareDocumentName = otpCnt.ToString();

                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO OTP (otpID, customerID, dateCreated, napomena, userID) VALUES (" + otpCnt + ", " + cmpR.ID + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', '" + napomena + "', " +WorkingUser.UserID + ")";
                    command.ExecuteNonQuery();

                    for (int i = 0; i < ListOfParts.Count; i++)
                    {
                        command.CommandText = "INSERT INTO PartsPoslano SELECT* FROM Parts p WHERE p.partID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE PartsPoslano SET DateOut = '" + DateTime.Now.ToString("dd.MM.yy.") + "' WHERE PartID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO OTPparts (otpID, partID) VALUES (" + otpCnt + ", " + ListOfParts[i].PartID + ")";
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Parts SET StorageID = 3, DateSend = '" + DateTime.Now.ToString("dd.MM.yy.") + "' WHERE PartID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM Parts WHERE partID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = "INSERT INTO Transport (TransactionID, TransportDateOut, RegionIDOut, RegionIDIn, UsersUserIDOut, haveTrackingNumbers) VALUES (" 
                        + otpCnt + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', " + cmpS.ID + ", " + cmpR.ID + ", " + WorkingUser.UserID + ", " + 0 + ")";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    executed = string.Format("{0:00/000}", otpCnt);
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }

        public String OTPUnesiUredajeDaSuPrimljeniInner(String Uname, String Pass, List<Part> ListOfParts, Company cmpR, Company cmpS, String napomena)
        {
            String executed = "nok";
            long otpCnt = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select Count(otpID) from OTP where otpID LIKE '" + DateTime.Now.ToString("yy") + "%'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
            {
                executed = "nok";
                dataReader.Close();
            }
            else
            {
                otpCnt = long.Parse(dataReader.GetValue(0).ToString());
                otpCnt = long.Parse(DateTime.Now.ToString("yy") + string.Format("{0:000}", (otpCnt + 1)));
                Properties.Settings.Default.ShareDocumentName = otpCnt.ToString();

                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO OTP (otpID, customerID, dateCreated, napomena, userID) VALUES (" + otpCnt + ", " + cmpR.ID + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', '" + napomena + "', " + WorkingUser.UserID + ")";
                    command.ExecuteNonQuery();

                    for (int i = 0; i < ListOfParts.Count; i++)
                    {
                        command.CommandText = "INSERT INTO OTPparts (otpID, partID) VALUES (" + otpCnt + ", " + ListOfParts[i].PartID + ")";
                        //command.CommandText = "INSERT INTO OTPparts (otpID, partID, trackingNumber) VALUES (" + otpCnt + ", " + ListOfParts[i].PartID + ", '" + PrenesiTN(i) + "')";
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Parts SET StorageID = 2, DateSend = '" + DateTime.Now.ToString("dd.MM.yy.") + "' WHERE PartID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = "INSERT INTO Transport (TransactionID, TransportDateOut, RegionIDOut, RegionIDIn, UsersUserIDOut, haveTrackingNumbers) VALUES ("
                        + otpCnt + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', " + cmpS.ID + ", " + cmpR.ID + ", " + WorkingUser.UserID + ", " + 0 + ")";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    executed = string.Format("{0:00/000}", otpCnt);
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }

        public String IUSPrebaciUServis(String Uname, String Pass, List<Part> ListOfParts, long RegionIDReciever, long CustomerID, String mNapomenaIUS)
        {
            String executed = "nok";
            long IUSCnt = 0;
            long IUSCntFull = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select distinct top 1 rb from IUSparts where iusID LIKE '" + DateTime.Now.ToString("yy") + "%' ORDER BY rb desc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
                IUSCnt = 1;
            else
                IUSCnt = long.Parse(dataReader.GetValue(0).ToString()) + (IUSCnt + 1);

            IUSCntFull = long.Parse(DateTime.Now.ToString("yy")) * 1000000 + (WorkingUser.UserID * 1000);

            Properties.Settings.Default.ShareDocumentName = (IUSCntFull + IUSCnt).ToString();

            dataReader.Close();
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                    

                for (int i = 0; i < ListOfParts.Count; i++)
                {
                    command.CommandText = "UPDATE Parts SET State = 'sng' WHERE PartID = " + ListOfParts[i].PartID;
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO IUSparts (iusID, partID, date, rb, customerID, napomena) VALUES (" + IUSCntFull + ", " + ListOfParts[i].PartID
                        + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', " + IUSCnt + ", " + CustomerID + ", '" + mNapomenaIUS + "')";

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                executed = string.Format("{0:00/000/000}", IUSCntFull + IUSCnt);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    executed = "nok";
                    throw;
                }
                catch (Exception e2)
                {
                    new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }

        public List<String> openedTickets(String Uname, String Pass)
        {
            int cnt = 1;
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "SELECT * FROM Ticket WHERE (UserIDUnio <> 2 or UserIDUnio is NULL) and VriZavrsio is NULL ";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    String wrkStr = "";

                    if (dataReader["DatPreuzeto"].ToString().Equals(""))
                    {
                        wrkStr = "Need action";
                    }
                    else if (dataReader["DatDrive"].ToString().Equals(""))
                    {
                        wrkStr = "Taken " + dataReader["DatPreuzeto"].ToString() + " - " + dataReader["VriPreuzeto"].ToString() + "h";
                    }
                    else if (dataReader["DatPoceo"].ToString().Equals(""))
                    {
                        wrkStr = "Driving " + dataReader["DatDrive"].ToString() + " - " + dataReader["VriDrive"].ToString() + "h";
                    }
                    else if (dataReader["DatZavrsio"].ToString().Equals(""))
                    {
                        wrkStr = "Working " + dataReader["DatPoceo"].ToString() + " - " + dataReader["VriPoceo"].ToString() + "h";
                    }

                    arr.Add(cnt + " =>     ID: " + dataReader["TicketID"].ToString() + "  #  CUST: " + dataReader["TvrtkeID"].ToString() + " - F" + dataReader["Filijala"].ToString() + "  #  CCN: " + dataReader["CCN"].ToString() + " - " + dataReader["CID"].ToString() + "  #  PLACED: " +
                    dataReader["DatPrijave"].ToString() + " - " + dataReader["VriPrijave"].ToString() + "h  #  SLA: " + dataReader["DatSLA"].ToString() + " - " + dataReader["VriSLA"].ToString() + "h  #  STATUS: " + wrkStr);

                    cnt++;
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> openedTicketsList(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "SELECT * FROM Ticket WHERE (UserIDUnio <> 2 or UserIDUnio is NULL) and VriZavrsio is NULL ";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["TicketID"].ToString());
                    arr.Add(dataReader["TvrtkeID"].ToString());
                    arr.Add(dataReader["Prio"].ToString());
                    arr.Add(dataReader["Filijala"].ToString());
                    arr.Add(dataReader["CCN"].ToString());
                    arr.Add(dataReader["CID"].ToString());
                    arr.Add(dataReader["DatPrijave"].ToString());
                    arr.Add(dataReader["VriPrijave"].ToString());
                    arr.Add(dataReader["DatSLA"].ToString());
                    arr.Add(dataReader["VriSla"].ToString());
                    arr.Add(dataReader["Drive"].ToString());
                    arr.Add(dataReader["NazivUredaja"].ToString());
                    arr.Add(dataReader["Prijavio"].ToString());
                    arr.Add(dataReader["UserIDPreuzeo"].ToString());
                    arr.Add(dataReader["DatPreuzeto"].ToString());
                    arr.Add(dataReader["VriPreuzeto"].ToString());
                    arr.Add(dataReader["UserIDDrive"].ToString());
                    arr.Add(dataReader["DatDrive"].ToString());
                    arr.Add(dataReader["VriDrive"].ToString());
                    arr.Add(dataReader["UserIDPoceo"].ToString());
                    arr.Add(dataReader["DatPoceo"].ToString());
                    arr.Add(dataReader["VriPoceo"].ToString());
                    arr.Add(dataReader["UserIDZavrsio"].ToString());
                    arr.Add(dataReader["DatZavrsio"].ToString());
                    arr.Add(dataReader["VriZavrsio"].ToString());
                    arr.Add(dataReader["UserIDSastavio"].ToString());
                    arr.Add(dataReader["OpisKvara"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public int GetLastTicketID(String Uname, String Pass)
        {
            int retValue = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select count(TicketID) from ticket";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                retValue = dataReader.GetInt32(0);
            }

            dataReader.Close();
            cnn.Close();
            return retValue;
        }

        public List<String> SentToUsers()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Email, UserID from Users where UserID > 3 and Email is NOT NULL";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["Email"].ToString());
                    arr.Add(dataReader["UserID"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public Boolean SendTIDStorno(String Uname, String Pass, long mTicketID, String mNaslov, String mPosaljiPoruku, String mSendList)
        {
            Boolean isDone = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "UPDATE Ticket SET UserIDUnio = 1, DatReport = '" + DateTime.Now.ToString("dd.MM.yy.") + "', VriReport = '" + DateTime.Now.ToString("HH:mm") + "' WHERE TicketID = " + mTicketID;
                command.ExecuteNonQuery();

                command.CommandText = "exec msdb.dbo.sp_send_dbmail @profile_name = 'CES-POS ticket' , @recipients = '" + mSendList + ";', @subject = '" + mNaslov + "', @body = '" + mPosaljiPoruku + "', @body_format = 'HTML'";
                command.ExecuteNonQuery();

                transaction.Commit();
                isDone = true;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    cnn.Close();
                    isDone = false;
                    throw;
                }
                catch (Exception e2)
                {
                    new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            cnn.Close();
            return isDone;
        }

        public Boolean SendTID(String Uname, String Pass, long mTicketID, String mNaslov, String mPosaljiPoruku, String mSendList)
        {
            Boolean isDone = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                //command.CommandText = "UPDATE Ticket SET UserIDUnio = 1, DatReport = '" + DateTime.Now.ToString("dd.MM.yy.") + "', VriReport = '" + DateTime.Now.ToString("HH:mm") + "' WHERE TicketID = " + mTicketID;
                //command.ExecuteNonQuery();

                command.CommandText = "exec msdb.dbo.sp_send_dbmail @profile_name = 'CES-POS ticket' , @recipients = '" + mSendList + ";', @subject = '" + mNaslov + "', @body = '" + mPosaljiPoruku + "', @body_format = 'HTML'";
                command.ExecuteNonQuery();

                transaction.Commit();
                isDone = true;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    cnn.Close();
                    isDone = false;
                    throw;
                }
                catch (Exception e2)
                {
                    new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            cnn.Close();
            return isDone;
        }

        public Boolean AddTID(String Uname, String Pass, long mTicketID, Company mCmp, long mPrio, String mFilijala, String mCCN, String mCID, String mDatPrijave, String mVriPrijave, String mDatSla, String mVriSla, long mDrive, String mNazivUredaja, String mOpis, String mPrijavio)
        {
            Boolean isDone = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "INSERT INTO Ticket (TicketID, TvrtkeID, Prio, Filijala, CCN, CID, DatPrijave, VriPrijave, DatSLA, VriSLA, Drive, NazivUredaja, OpisKvara, Prijavio, UserIDSastavio)"
                                    + "VALUES"
                                    + "(" + mTicketID + ", " + mCmp.ID + ", " + mPrio + ", '" + mFilijala + "', '" + mCCN + "', '" + mCID + "', '" + mDatPrijave + "', '" + mVriPrijave + "', '" + mDatSla + "', '" + mVriSla
                                    + "', " + mDrive + ", '" + mNazivUredaja + "', '" + mOpis + "', '" + mPrijavio + "', 1)";
                command.ExecuteNonQuery();

                transaction.Commit();
                isDone = true;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    cnn.Close();
                    isDone = false;
                    throw;
                }
                catch (Exception e2)
                {
                    new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            cnn.Close();
            return isDone;
        }

        public Boolean IsTIDExistByIDCCNCID(long mID, String mCCN, String mCID)
        {
            Boolean exist = false;
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Ticket where TicketID = " + mID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            try
            {
                if (dataReader.HasRows)
                {
                    exist = true;
                }
                else if(!exist)
                {
                    dataReader.Close();
                    query = "Select * from Ticket where CCN = '" + mCCN + "'";
                    command = new SqlCommand(query, cnn);
                    command.ExecuteNonQuery();
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    if (dataReader.HasRows)
                        exist = true;
                }
                else if (!exist)
                {
                    dataReader.Close();
                    query = "Select * from Ticket where CID = '" + mCID + "'";
                    command = new SqlCommand(query, cnn);
                    command.ExecuteNonQuery();
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    if (dataReader.HasRows)
                        exist = true;
                }
                dataReader.Close();
                cnn.Close();
                return exist;
            }
            catch (Exception e2)
            {
                new LogWriter(e2);
                return exist;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
        }

        public List<String> FilByFilNumber(String mFilNumber)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where FilNumber = '" + mFilNumber + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            try
            {
                if (dataReader.HasRows)
                {
                    do
                    {
                        arr.Add(dataReader["filID"].ToString());
                        arr.Add(dataReader["tvrtkeCode"].ToString());
                        arr.Add(dataReader["filNumber"].ToString());
                        arr.Add(dataReader["regionID"].ToString());
                        arr.Add(dataReader["address"].ToString());
                        arr.Add(dataReader["city"].ToString());
                        arr.Add(dataReader["pb"].ToString());
                        arr.Add(dataReader["phone"].ToString());
                        arr.Add(dataReader["country"].ToString());
                    } while (dataReader.Read());
                }
                else
                {
                    arr.Add("nok");
                }
                dataReader.Close();
                cnn.Close();
                return arr;
            }
            catch (Exception e2)
            {
                arr.Add("nok");
                new LogWriter(e2);
                return arr;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

        }

        public List<String> FilByTvrtkeCodeFilNumber(String mTvrtkeCode, String mFilNumber)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where TvrtkeCode = '" + mTvrtkeCode + "' and FilNumber = '" + mFilNumber + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> FilByAddress(String mAddress)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where Address = '" + mAddress + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> FilByCity(String mCity)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where City = '" + mCity + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> FilByCountry(String mCountry)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where Country = '" + mCountry + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> FilByRegionID(long mRegionID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where RegionID = " + mRegionID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> AllFilByTvrtkeCode(string mTvrtkeCode)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where TvrtkeCode = '" + mTvrtkeCode + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> AllFilInfoSortByFilNumber()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale Order by FilNumber ASC";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["filID"].ToString());
                    arr.Add(dataReader["tvrtkeCode"].ToString());
                    arr.Add(dataReader["filNumber"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["address"].ToString());
                    arr.Add(dataReader["city"].ToString());
                    arr.Add(dataReader["pb"].ToString());
                    arr.Add(dataReader["phone"].ToString());
                    arr.Add(dataReader["country"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public Boolean AddFilToDB(String mTvrtkeCode, String mFillNumber, long mRegionID, String mAddress, String mCity, String mPB, String mPhone, String mCountry)
        {
            String Uname = WorkingUser.Username;
            String Pass = WorkingUser.Password;
            Boolean executed = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(FilNumber) from Filijale f where f.TvrtkeCode = '" + mTvrtkeCode + "' and FilNumber = '" + mFillNumber + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows && !dataReader.GetValue(0).Equals(0))
            {
                executed = false;
                dataReader.Close();
            }
            else
            {
                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO Filijale (TvrtkeCode, FilNumber, RegionID, Address, City, PB, Phone, Country) " +
                        "values ('" + mTvrtkeCode + "', '" + mFillNumber + "', " + mRegionID + ", '" + mAddress + "', '" + mCity + "', '" + mPB + "', '" + mPhone + "', '" + mCountry + "')";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    executed = true;
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        throw;
                    }
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }
            dataReader.Close();
            cnn.Close();
            return executed;
        }
    }
}