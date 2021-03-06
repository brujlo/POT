﻿using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data;
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

        public Boolean CheckConnection()
        {
            Boolean cnnExist = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Username from Users where UserID = 1";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            try
            {
                if (cn.TestConnection(cnn))
                {
                    if (dataReader.HasRows)
                    {
                        if (dataReader.GetString(0).ToUpper().Equals("BAZA"))
                            cnnExist = true;
                    }
                    else
                    {
                        cnnExist = false;
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                return cnnExist;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            dataReader.Close();
            cnn.Close();
            return cnnExist;
        }

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

            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Users where UserID = " + int.Parse(mUserID);
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (cn.TestConnection(cnn))
            {
                if (dataReader.HasRows)
                {
                    arr.Add(dataReader.GetDecimal(0).ToString("0").Replace(".00", String.Empty));
                    arr.Add(dataReader.GetString(1));
                    arr.Add(dataReader.GetString(2));
                    arr.Add(dataReader.GetString(3));
                    arr.Add(dataReader.GetString(4));
                    arr.Add(dataReader.GetString(5));
                    arr.Add(dataReader.GetString(6));
                    arr.Add(dataReader.GetDecimal(7).ToString("0").Replace(".00", String.Empty));
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        cnn.Close();
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        command.CommandText = "ALTER DATABASE [CP] SET MULTI_USER";
                        command.ExecuteNonQuery();
                        cnn.Close();
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public List<String> RegionInfoByUserID(long mUserID)
        {
            List<String> arr = new List<string>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select RegionID from Users where UserID = " + mUserID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();
            long id = 0;

            if (dataReader.HasRows)
            {
                id = long.Parse(dataReader["RegionID"].ToString());
                dataReader.Close();

                query = "Select * from Regija where RegionID = " + id;
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();
                dataReader = command.ExecuteReader();
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

        public List<String> GetTableColumnNames(String tableName)
        {
            List<String> arr = new List<string>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader.GetString(0));
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

        public PartSifrarnik PartInfoByFullCodeSifrarnik(long mFullCode)
        {
            PartSifrarnik tempPart = new PartSifrarnik();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
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
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return tempPart;
        }

        public List<PartSifrarnik> GetPartsAllSifrarnik()
        {
            List<PartSifrarnik> prs = new List<PartSifrarnik>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select * from Sifrarnik";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            PartSifrarnik tempPart = new PartSifrarnik();

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
                            prs.Add(tempPart);
                        } while (dataReader.Read()) ;
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public List<PartSifrarnik> GetPartsAllSifrarnikSortByFullName()
        {
            List<PartSifrarnik> prs = new List<PartSifrarnik>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select * from Sifrarnik order by FullName asc";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            PartSifrarnik tempPart = new PartSifrarnik();

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
                            prs.Add(tempPart);
                        } while (dataReader.Read());
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public List<PartSifrarnik> GetPartsAllSifrarnikSortByFullCode()
        {
            List<PartSifrarnik> prs = new List<PartSifrarnik>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select * from Sifrarnik order by FullCode asc";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            PartSifrarnik tempPart = new PartSifrarnik();

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
                            prs.Add(tempPart);
                        } while (dataReader.Read());
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public Dictionary<long, String> GetCategoryNamesAllSifrarnikSortByName()
        {
            Dictionary<long, String> prs = new Dictionary<long, String>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select DISTINCT CategoryCode, CategoryName from Sifrarnik ORDER BY CategoryName asc";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            if ( !prs.ContainsKey(long.Parse(dataReader["CategoryCode"].ToString())) )
                                prs.Add(long.Parse(dataReader["CategoryCode"].ToString()), dataReader["CategoryName"].ToString());
                        } while (dataReader.Read());
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                //throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public Dictionary<long, String> GetPartNamesAllSifrarnikSortByName(String mNaziv)
        {
            Dictionary<long, String> prs = new Dictionary<long, String>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select DISTINCT PartCode, PartName  from Sifrarnik where CategoryName = '" + mNaziv + "' ORDER BY PartName asc";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            if (!prs.ContainsKey(long.Parse(dataReader["PartCode"].ToString())))
                                prs.Add(long.Parse(dataReader["PartCode"].ToString()), dataReader["PartName"].ToString());
                        } while (dataReader.Read());
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public Dictionary<long, String> GetSubPartNamesAllSifrarnikSortByName(String mCategoryNaziv, String mPartNaziv)
        {
            Dictionary<long, String> prs = new Dictionary<long, String>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select DISTINCT SubPartName, SubPartCode from Sifrarnik where CategoryName = '" + mCategoryNaziv + "' and PartName = '" + mPartNaziv + "' ORDER BY SubPartName asc";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            if (!prs.ContainsKey(long.Parse(dataReader["SubPartCode"].ToString())))
                                prs.Add(long.Parse(dataReader["SubPartCode"].ToString()), dataReader["SubPartName"].ToString());
                        } while (dataReader.Read());
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public List<PartSifrarnik> GetPartsAllSifrarnikSortByFullCodeDistinct()
        {
            List<PartSifrarnik> prs = new List<PartSifrarnik>();

            try
            {
                //SqlConnection cnn = cn.Connect(Uname, Pass);
                cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
                query = "Select * from Sifrarnik order by FullCode asc";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    dataReader.Read();

                    if (dataReader.HasRows)
                    {
                        do
                        {
                            PartSifrarnik tempPart = new PartSifrarnik();

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
                            prs.Add(tempPart);
                        } while (dataReader.Read());
                    }
                }
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            return prs;
        }

        public List<String> ListPartsByCodeRegionStateS(long mCodePartFull, long mStorageID, String mState)
        {
            List<String> arr = new List<String>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);

            if ( mCodePartFull == 0 && mStorageID == 0 && mState.Equals("") )
            {
                query = "Select * from Parts";
            }
            else
            {
                if (mStorageID == 1 || mStorageID == 2)
                    query = "Select Distinct * from Parts where CodePartFull = " + mCodePartFull + " and (State = '" + mState + "' or State = 'wng' or State = 'wg')";
                else
                    query = "Select Distinct * from Parts where CodePartFull = " + mCodePartFull + " and StorageID = " + mStorageID + " and State = '" + mState + "'";
            }

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

        public List<Part> ListPartsByRegionStateP(long mStorageID, String mState)
        {
            List<Part> arr = new List<Part>();
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Parts where StorageID = " + mStorageID + " and State = '" + mState + "'";
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
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<Part> ListPartsByRegionStatesP(long mStorageID, List<String> mStates)
        {
            List<Part> arr = new List<Part>();
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);

            query = "Select * from Parts where StorageID = " + mStorageID + " and State = '" + mStates[0] + "'";

            if (mStates.Count > 0)
            {
                for(int i = 1; i < mStates.Count; i++)
                {
                    query += " or State = '" + mStates[i] + "'";

                }

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
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    throw;
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
                dataReader.Close();
            }
            cnn.Close();
            return arr;
        }

        public List<String> GetListPartsByPartIDFromParts(long mPartID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
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

        public List<String> GetListPartsByPartIDFromPartsPoslano(long mPartID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from PartsPoslano where PartID = " + mPartID;
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

        public List<String> GetListPartsByPartIDFromPartsZamijenjeno(long mPartID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from PartsZamijenjeno where PartID = " + mPartID;
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

        public List<Part> SearchPartsInAllTablesBYPartID(long mPartID)
        {
            List<Part> arr = new List<Part>();
            
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Parts where PartID = " + mPartID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
            {
                dataReader.Close();
                query = "Select * from PartsPoslano where PartID = " + mPartID;
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();
                dataReader = command.ExecuteReader();
                dataReader.Read();

                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    query = "Select * from PartsZamijenjeno where PartID = " + mPartID;
                    command = new SqlCommand(query, cnn);
                    command.ExecuteNonQuery();
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                }
            }

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
            else
            {
                arr.Add(new Part());
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<Part> GetAllParts()
        {
            List<Part> arr = new List<Part>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Parts";
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
            SqlCommand commandBT; //back thread
            String queryBT;
            ConnectionHelper cnBT = new ConnectionHelper();
            SqlConnection cnnBT = new SqlConnection();

            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnnBT = cnBT.Connect(Uname, Pass);
            queryBT = "Select FullName, FullCode from Sifrarnik order by FullName asc";
            commandBT = new SqlCommand(queryBT, cnnBT);
            commandBT.ExecuteNonQuery();
            SqlDataReader dataReaderBT = commandBT.ExecuteReader();
            dataReaderBT.Read();

            if (dataReaderBT.HasRows)
            {
                do
                {
                    arr.Add(dataReaderBT["FullName"].ToString());
                    arr.Add(dataReaderBT["FullCode"].ToString());
                } while (dataReaderBT.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReaderBT.Close();
            cnnBT.Close();
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

        public List<String> CompanyInfoByID(String Uname, String Pass, long mID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "Select * from Tvrtke where ID = " + mID;
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

        public List<String> CompanyInfoByName(String mName)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Tvrtke where Name = '" + mName.Trim() + "'";
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

        public List<String> GetAllRegions()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
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

        public List<String> GetAllRegionsEditable()
        {
            List<String> arr = new List<string>();
            
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Regija where RegionID > 3";
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
                    catch (Exception)
                    {
                        //new LogWriter(e1);
                        try
                        {
                            transaction.Rollback();
                            executed = false;
                            throw;
                        }
                        catch (Exception)
                        {
                            //new LogWriter(e2);
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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
            query = "select Count(CodePartFull) from Parts p where p.CodePartFull = " + mCodePartFull + " and (p.State = 'sng' or p.State = 'wng' or p.State = 'wg')";
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

        public List<String> CurrentExchangeRate()
        {
            List<String> arr = new List<string>();
            
            cnn = cn.Connect("admin", "0000");
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

        public List<long> GetAllOTPID()
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct otpID from OTP order by otpID asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["otpID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }

            SortLong(ref arr, "desc");

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<long> GetAllOTPcustomerID()
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct customerID from OTP order by customerID asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["customerID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllOTPdateCreated()
        {
            List<String> arr = new List<String>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct dateCreated from OTP";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["dateCreated"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }

            try
            {
                SortArrStringDatesDesc(ref arr);
            }
            catch(Exception)
            {
                //new LogWriter(e1);
                throw;
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllInfoOTPBy(String what, String value)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            if (what.Equals("dateCreated"))
                query = "Select * from OTP where " + what + " = '" + value + "' order by otpID";
            else
                query = "Select * from OTP where " + what + " = " + value + " order by otpID";

            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["otpID"].ToString());
                    arr.Add(dataReader["customerID"].ToString());
                    arr.Add(dataReader["dateCreated"].ToString());
                    arr.Add(dataReader["napomena"].ToString());
                    arr.Add(dataReader["primID"].ToString());
                    arr.Add(dataReader["userID"].ToString());
                    arr.Add(dataReader["branchID"].ToString());
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

        public List<long> GetAllpartIDByOtpID(long pOtpID)
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select distinct partID from OTPparts where otpID = " + pOtpID  + " order by partID";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["partID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<long> GetAllPRIMID(Boolean desc)
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);

            if (desc)
                query = "Select Distinct primID from PRIM order by primID desc";
            else
                query = "Select Distinct primID from PRIM order by primID asc";

            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["primID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<long> GetAllPRIMcustomerID()
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct customerID from PRIM order by customerID asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["customerID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllPRIMdateCreated(Boolean desc)
        {
            List<String> arr = new List<string>();
            List<DateTime> arrSort = new List<DateTime>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct dateCreated from PRIM";

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
                        arrSort.Add(Convert.ToDateTime(dataReader["dateCreated"].ToString()));
                    
                        if(desc)
                            arrSort.Sort((x, y) => y.CompareTo(x));
                        else
                            arrSort.Sort((x, y) => x.CompareTo(y));

                    } while (dataReader.Read());

                    foreach(DateTime dt in arrSort)
                    {
                        arr.Add(dt.ToString("dd.MM.yy."));
                    }
                }
                else
                {
                    arr.Add("nok");
                }

                //List<DateTime> dates = new List<DateTime>();

                //foreach (String a in arr)
                //{
                //    dates.Add(DateTime.Parse(a));
                //}

                //dates.Sort();
                //arr.Clear();
                //DateConverter dc = new DateConverter();

                //foreach (DateTime a in dates)
                //{
                //    arr.Add(dc.ConvertDDMMYY(a.ToShortDateString()));
                //}
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllInfoPRIMBy(String what, String value)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            if (what.Equals("dateCreated"))
                query = "Select * from PRIM where " + what + " = '" + value + "' order by primID";
            else
                query = "Select * from PRIM where " + what + " = " + value + " order by primID";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["primID"].ToString());
                    arr.Add(dataReader["customerID"].ToString());
                    arr.Add(dataReader["dateCreated"].ToString());
                    arr.Add(dataReader["napomena"].ToString());
                    arr.Add(dataReader["userID"].ToString());
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

        public List<long> GetAllpartIDByPrimID(long pPrimID)
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select distinct partID from PRIMparts where primID = " + pPrimID + " order by partID";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["partID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public long AddPartsToParts(List<Part> ListOfParts)
        {
            long newPartID = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                for (int i = 0; i < ListOfParts.Count; i++)
                {
                    command.CommandText = "INSERT INTO Parts (PartialCode, SN, CN, DateIn, DateOut, DateSend, StorageID, State, CompanyO, CompanyC) output INSERTED.PartID VALUES (" + ListOfParts[i].PartialCode + ", '" + ListOfParts[i].SN + "', '" + ListOfParts[i].CN + "', '" + DateTime.Now.ToString("dd.MM.yy.") + "', '', '', " + WorkingUser.RegionID + ", '" + ListOfParts[i].State + "', '" + ListOfParts[i].CompanyO + "', '" + ListOfParts[i].CompanyC + "')";
                    var retVal = command.ExecuteScalar();
                    newPartID = (long)(retVal);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    newPartID = 0;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            
            cnn.Close();
            return newPartID;
        }

        public long AddPartToParts(Part prt)
        {
            long newPartID = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "INSERT INTO Parts (PartialCode, SN, CN, DateIn, DateOut, DateSend, StorageID, State, CompanyO, CompanyC) output INSERTED.PartID VALUES (" +
                    prt.PartialCode + ", '" + prt.SN + "', '" + prt.CN + "', '" + DateTime.Now.ToString("dd.MM.yy.") + "', '', '', " + WorkingUser.RegionID + ", '" + prt.State + "', '" + prt.CompanyO + "', '" + prt.CompanyC + "')";
                var retVal = command.ExecuteScalar();
                newPartID = long.Parse(retVal.ToString());

                transaction.Commit();
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    newPartID = 0;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return newPartID;
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public String OTPUnesiUredajeDaSuPrimljeni(String Uname, String Pass, List<Part> ListOfParts, Company cmpR, Company cmpS, String napomena, long mBranchID)
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
                    command.CommandText = "INSERT INTO OTP (otpID, customerID, dateCreated, napomena, userID, branchID) VALUES (" + otpCnt + ", " + cmpR.ID + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', '" + napomena + "', " +WorkingUser.UserID + ", " + mBranchID + ")";
                    command.ExecuteNonQuery();

                    for (int i = 0; i < ListOfParts.Count; i++)
                    {
                        command.CommandText = "INSERT INTO PartsPoslano SELECT* FROM Parts p WHERE p.partID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE PartsPoslano SET DateOut = '" + DateTime.Now.ToString("dd.MM.yy.") + "' WHERE PartID = " + ListOfParts[i].PartID;
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO OTPparts (otpID, partID) VALUES (" + otpCnt + ", " + ListOfParts[i].PartID + ")";
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE Parts SET StorageID = 3, DateOut = '" + DateTime.Now.ToString("dd.MM.yy.") + "' WHERE PartID = " + ListOfParts[i].PartID;
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public String OTPUnesiUredajeDaSuPrimljeniInner(String Uname, String Pass, List<Part> ListOfParts, Company cmpR, Company cmpS, String napomena, long mBranchID)
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

                    command.CommandText = "INSERT INTO OTP (otpID, customerID, dateCreated, napomena, userID, branchID) VALUES (" + otpCnt + ", " + cmpR.ID + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', '" + napomena + "', " + WorkingUser.UserID + ", " + mBranchID + ")";
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = "nok";
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

            IUSCntFull = long.Parse(DateTime.Now.ToString("yy")) * 1000000 + (WorkingUser.UserID * 1000) + IUSCnt;

            Properties.Settings.Default.ShareDocumentName = (IUSCntFull).ToString();

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

                    command.CommandText = "UPDATE Parts SET DateSend = " + DateTime.Now.ToString("dd.MM.yy.") + " WHERE PartID = " + ListOfParts[i].PartID;
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO IUSparts (iusID, partID, date, rb, customerID, napomena) VALUES (" + IUSCntFull + ", " + ListOfParts[i].PartID
                        + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', " + IUSCnt + ", " + CustomerID + ", '" + mNapomenaIUS + "')";

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                executed = string.Format("{0:00/000/000}", IUSCntFull);
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    executed = "nok";
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
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

        public Boolean ISSPrebaciPartUWorking(Part _part)
        {
            Boolean executed = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                    command.CommandText = "UPDATE Parts SET State = 'wng' WHERE PartID = " + _part.PartID;
                    command.ExecuteNonQuery();

                transaction.Commit();
                executed = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    executed = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
 
            cnn.Close();
            return executed;
        }

        public String IISPrebaciIzServisa(String Uname, String Pass, List<Part> ListOfParts, long RegionIDSender, long CustomerID, String mNapomenaIUS)
        {
            String executed = "nok";
            long IISCnt = 0;
            long IISCntFull = 0;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "select distinct top 1 rb from IISparts where iisID LIKE '" + DateTime.Now.ToString("yy") + "%' ORDER BY rb desc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
                IISCnt = 1;
            else
                IISCnt = long.Parse(dataReader.GetValue(0).ToString()) + (IISCnt + 1);

            IISCntFull = long.Parse(DateTime.Now.ToString("yy")) * 1000000 + (WorkingUser.UserID * 1000) + IISCnt;

            Properties.Settings.Default.ShareDocumentName = (IISCntFull).ToString();

            dataReader.Close();
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {


                for (int i = 0; i < ListOfParts.Count; i++)
                {
                    command.CommandText = "UPDATE Parts SET State = 'g' WHERE PartID = " + ListOfParts[i].PartID;
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE Parts SET DateSend = '" + DateTime.Now.ToString("dd.MM.yy.") + "' where PartID = " + ListOfParts[i].PartID;
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO IISparts (iisID, partID, date, rb, customerID, napomena) VALUES (" + IISCntFull + ", " + ListOfParts[i].PartID
                        + ", '" + DateTime.Now.ToString("dd.MM.yy.") + "', " + IISCnt + ", " + CustomerID + ", '" + mNapomenaIUS + "')";

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                executed = string.Format("{0:00/000/000}", IISCntFull);
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    executed = "nok";
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
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

        public List<TIDs> getAllTickets()
        {
            List<TIDs> arr = new List<TIDs>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "SELECT * FROM Ticket Order By TicketID desc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                try
                {
                    do
                    {
                        TIDs tc = new TIDs(
                            dataReader["TicketID"] == DBNull.Value ? 0 : long.Parse(dataReader["TicketID"].ToString()),
                            dataReader["TvrtkeID"] == DBNull.Value ? 0 : long.Parse(dataReader["TvrtkeID"].ToString()),
                            dataReader["Prio"] == DBNull.Value ? 0 : long.Parse(dataReader["Prio"].ToString()),
                            dataReader["Filijala"].ToString(),
                            dataReader["CCN"].ToString(),
                            dataReader["CID"].ToString(),
                            dataReader["DatPrijave"].ToString(),
                            dataReader["VriPrijave"].ToString(),
                            dataReader["DatSLA"].ToString(),
                            dataReader["VriSLA"].ToString(),
                            dataReader["Drive"] == DBNull.Value ? 0 : long.Parse(dataReader["Drive"].ToString()),
                            dataReader["NazivUredaja"].ToString(),
                            dataReader["OpisKvara"].ToString(),
                            dataReader["Prijavio"].ToString(),
                            dataReader["UserIDPreuzeo"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDPreuzeo"].ToString()),
                            dataReader["DatPreuzeto"].ToString(),
                            dataReader["VriPreuzeto"].ToString(),
                            dataReader["UserIDDrive"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDDrive"].ToString()),
                            dataReader["DatDrive"].ToString(),
                            dataReader["VriDrive"].ToString(),
                            dataReader["UserIDPoceo"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDPoceo"].ToString()),
                            dataReader["DatPoceo"].ToString(),
                            dataReader["VriPoceo"].ToString(),
                            dataReader["UserIDZavrsio"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDZavrsio"].ToString()),
                            dataReader["DatZavrsio"].ToString(),
                            dataReader["VriZavrsio"].ToString(),
                            dataReader["UserIDUnio"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDUnio"].ToString()),
                            dataReader["DatReport"].ToString(),
                            dataReader["VriReport"].ToString(),
                            dataReader["RNID"].ToString(),
                            dataReader["UserIDSastavio"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDSastavio"].ToString())
                            );

                        arr.Add(tc);


                    } while (dataReader.Read());

                    cnn.Close();
                    return arr;
                }
                catch (Exception e2)
                {
                    new LogWriter(e2);
                    return arr;
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }

            cnn.Close();
            return arr;
        }

        public TIDs getTicketByID(double mID)
        {
            List<TIDs> arr = new List<TIDs>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "SELECT * FROM Ticket where TicketID = " + mID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            TIDs tmp = new TIDs();

            if (dataReader.HasRows)
            {
                try
                {
                    TIDs tc = new TIDs(
                        dataReader["TicketID"] == DBNull.Value ? 0 : long.Parse(dataReader["TicketID"].ToString()),
                        dataReader["TvrtkeID"] == DBNull.Value ? 0 : long.Parse(dataReader["TvrtkeID"].ToString()),
                        dataReader["Prio"] == DBNull.Value ? 0 : long.Parse(dataReader["Prio"].ToString()),
                        dataReader["Filijala"].ToString(),
                        dataReader["CCN"].ToString(),
                        dataReader["CID"].ToString(),
                        dataReader["DatPrijave"].ToString(),
                        dataReader["VriPrijave"].ToString(),
                        dataReader["DatSLA"].ToString(),
                        dataReader["VriSLA"].ToString(),
                        dataReader["Drive"] == DBNull.Value ? 0 : long.Parse(dataReader["Drive"].ToString()),
                        dataReader["NazivUredaja"].ToString(),
                        dataReader["OpisKvara"].ToString(),
                        dataReader["Prijavio"].ToString(),
                        dataReader["UserIDPreuzeo"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDPreuzeo"].ToString()),
                        dataReader["DatPreuzeto"].ToString(),
                        dataReader["VriPreuzeto"].ToString(),
                        dataReader["UserIDDrive"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDDrive"].ToString()),
                        dataReader["DatDrive"].ToString(),
                        dataReader["VriDrive"].ToString(),
                        dataReader["UserIDPoceo"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDPoceo"].ToString()),
                        dataReader["DatPoceo"].ToString(),
                        dataReader["VriPoceo"].ToString(),
                        dataReader["UserIDZavrsio"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDZavrsio"].ToString()),
                        dataReader["DatZavrsio"].ToString(),
                        dataReader["VriZavrsio"].ToString(),
                        dataReader["UserIDUnio"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDUnio"].ToString()),
                        dataReader["DatReport"].ToString(),
                        dataReader["VriReport"].ToString(),
                        dataReader["RNID"].ToString(),
                        dataReader["UserIDSastavio"] == DBNull.Value ? 0 : long.Parse(dataReader["UserIDSastavio"].ToString())
                        );

                    tc.CopyTo(tmp);

                    cnn.Close();
                    return tc;
                }
                catch (Exception e2)
                {
                    new LogWriter(e2);
                    return tmp;
                }
                finally
                {
                    if (cnn.State.ToString().Equals("Open"))
                        cnn.Close();
                }
            }

            cnn.Close();
            return tmp;
        }

        public List<String> openedTicketsList(String Uname, String Pass)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(Uname, Pass);
            query = "SELECT * FROM Ticket WHERE (UserIDUnio <> 2 or UserIDUnio is NULL) and VriZavrsio is NULL order by TicketID asc";
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
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    cnn.Close();
                    isDone = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
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
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    cnn.Close();
                    isDone = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
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
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    cnn.Close();
                    isDone = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
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
            query = "Select * from Filijale where TvrtkeCode = '" + mTvrtkeCode + "' order by filNumber asc";
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

        public List<String> GetFillByID(long mFillID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Filijale where FilID = " + mFillID;
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
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public Boolean UpdateFilToDB(String mTvrtkeCode, String mFillNumber, long mRegionID, String mAddress, String mCity, String mPB, String mPhone, String mCountry)
        {
            Boolean executed = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(FilNumber) from Filijale f where f.TvrtkeCode = '" + mTvrtkeCode + "' and FilNumber = '" + mFillNumber + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows && dataReader.GetValue(0).Equals(0))
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
                    command.CommandText = "UPDATE Filijale Set TvrtkeCode = '" + mTvrtkeCode + "', FilNumber = '" + mFillNumber + "', RegionID = " + mRegionID +
                        ", Address = '" + mAddress + "', City = '" + mCity + "', PB = '" + mPB + "', Phone = '" + mPhone + "', Country =  '" + mCountry + "' where TvrtkeCode = '" + mTvrtkeCode + "' and FilNumber = '" + mFillNumber + "'";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    executed = true;
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public Boolean UpdateTvrtke(long mID, String mName, String mAddress, String mCity, String mPB, String mOIB, String mContact, String mBIC, String mKN, String mEUR, String mCode, String mCountry, long mRegionID, String mEmail)
        {
            Boolean isExecuted = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Tvrtke where ID = " + mID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.HasRows)
            {
                isExecuted = false;
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
                    command.CommandText = "UPDATE Tvrtke SET Name = '" + mName +"', Address = '" + mAddress + "', City = '" + mCity + "', PB = '" + mPB + "', OIB = '" + mOIB +"', Contact = '" + mContact + "', BIC = '" + mBIC + "', KN = " + mKN + 
                        ", Eur = " + mEUR + ", Code = '" + mCode + "', Country = '" + mCountry + "', RegionID = " + mRegionID + ", email = '" + mEmail + "' where ID = " + mID;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    isExecuted = true;
                }
                catch (Exception)
                {
                    isExecuted = false;
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        isExecuted = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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
            return isExecuted;
        }

        public int CountMainCmp()
        {
            int value;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from MainCmp";
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

        public Boolean MainCmpChangeSelected(String mName)
        {
            Boolean executed = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "UPDATE MainCmp SET Selected = CASE WHEN CmpName <> '" + mName + "' THEN 0 ELSE 1 END";
                command.ExecuteNonQuery();

                transaction.Commit();
                executed = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    executed = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return executed;
        }

        public MainCmp MainCmpInfoSelected()
        {
            MainCmp cmp = new MainCmp();
            
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp where Selected = 1";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            try
            {
                cmp.ID = long.Parse(dataReader["ID"].ToString());
                cmp.Name = dataReader["CmpName"].ToString();
                cmp.Address = dataReader["Address"].ToString();
                cmp.City = dataReader["City"].ToString();
                cmp.PB = dataReader["PB"].ToString();
                cmp.OIB = dataReader["OIB"].ToString();
                cmp.Contact = dataReader["Contact"].ToString();
                cmp.BIC = dataReader["BIC"].ToString();
                cmp.KN = decimal.Parse(dataReader["KN"].ToString());
                cmp.EUR = decimal.Parse(dataReader["EUR"].ToString());
                cmp.Code = dataReader["Code"].ToString();
                cmp.Country = dataReader["Country"].ToString();
                cmp.RegionID = long.Parse(dataReader["regionID"].ToString());
                cmp.Email = dataReader["Email"].ToString();
                cmp.Phone = dataReader["Phone"].ToString();
                cmp.WWW = dataReader["WWW"].ToString();
                cmp.MB = dataReader["MB"].ToString();
                cmp.IBAN = dataReader["IBAN"].ToString();
                cmp.SupportEmail = dataReader["SupportEmail"].ToString();
            }
            catch (Exception)
            {
                //new LogWriter(e1);    
                throw;
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return cmp;
        }

        public List<String> MainCmpInfoByCode(String mCode)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp where CompanyCode = '" + mCode + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["CmpName"].ToString());
                arr.Add(dataReader["Address"].ToString());
                arr.Add(dataReader["City"].ToString());
                arr.Add(dataReader["PB"].ToString());
                arr.Add(dataReader["OIB"].ToString());
                arr.Add(dataReader["Contact"].ToString());
                arr.Add(dataReader["BIC"].ToString());
                arr.Add(dataReader["KN"].ToString());
                arr.Add(dataReader["EUR"].ToString());
                arr.Add(dataReader["Code"].ToString());
                arr.Add(dataReader["Country"].ToString());
                arr.Add(dataReader["regionID"].ToString());
                arr.Add(dataReader["Email"].ToString());
                arr.Add(dataReader["Phone"].ToString());
                arr.Add(dataReader["WWW"].ToString());
                arr.Add(dataReader["MB"].ToString());
                arr.Add(dataReader["IBAN"].ToString());
                arr.Add(dataReader["SupportEmail"].ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> MainCmpInfoByID(long mID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp where ID = " + mID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["CmpName"].ToString());
                arr.Add(dataReader["Address"].ToString());
                arr.Add(dataReader["City"].ToString());
                arr.Add(dataReader["PB"].ToString());
                arr.Add(dataReader["OIB"].ToString());
                arr.Add(dataReader["Contact"].ToString());
                arr.Add(dataReader["BIC"].ToString());
                arr.Add(dataReader["KN"].ToString());
                arr.Add(dataReader["EUR"].ToString());
                arr.Add(dataReader["Code"].ToString());
                arr.Add(dataReader["Country"].ToString());
                arr.Add(dataReader["regionID"].ToString());
                arr.Add(dataReader["Email"].ToString());
                arr.Add(dataReader["Phone"].ToString());
                arr.Add(dataReader["WWW"].ToString());
                arr.Add(dataReader["MB"].ToString());
                arr.Add(dataReader["IBAN"].ToString());
                arr.Add(dataReader["SupportEmail"].ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> MainCmpInfoByName(String mName)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp where CmpName = '" + mName + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["CmpName"].ToString());
                arr.Add(dataReader["Address"].ToString());
                arr.Add(dataReader["City"].ToString());
                arr.Add(dataReader["PB"].ToString());
                arr.Add(dataReader["OIB"].ToString());
                arr.Add(dataReader["Contact"].ToString());
                arr.Add(dataReader["BIC"].ToString());
                arr.Add(dataReader["KN"].ToString());
                arr.Add(dataReader["EUR"].ToString());
                arr.Add(dataReader["Code"].ToString());
                arr.Add(dataReader["Country"].ToString());
                arr.Add(dataReader["regionID"].ToString());
                arr.Add(dataReader["Email"].ToString());
                arr.Add(dataReader["Phone"].ToString());
                arr.Add(dataReader["WWW"].ToString());
                arr.Add(dataReader["MB"].ToString());
                arr.Add(dataReader["IBAN"].ToString());
                arr.Add(dataReader["SupportEmail"].ToString());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> MainCmpInfoByRegionID(long mRegionID)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp where RegionID = " + mRegionID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["CmpName"].ToString());
                    arr.Add(dataReader["Address"].ToString());
                    arr.Add(dataReader["City"].ToString());
                    arr.Add(dataReader["PB"].ToString());
                    arr.Add(dataReader["OIB"].ToString());
                    arr.Add(dataReader["Contact"].ToString());
                    arr.Add(dataReader["BIC"].ToString());
                    arr.Add(dataReader["KN"].ToString());
                    arr.Add(dataReader["EUR"].ToString());
                    arr.Add(dataReader["Code"].ToString());
                    arr.Add(dataReader["Country"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["Email"].ToString());
                    arr.Add(dataReader["Phone"].ToString());
                    arr.Add(dataReader["WWW"].ToString());
                    arr.Add(dataReader["MB"].ToString());
                    arr.Add(dataReader["IBAN"].ToString());
                    arr.Add(dataReader["SupportEmail"].ToString());
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

        public List<String> AllMainCmpInfoSortCode()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp order by Code asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["CmpName"].ToString());
                    arr.Add(dataReader["Address"].ToString());
                    arr.Add(dataReader["City"].ToString());
                    arr.Add(dataReader["PB"].ToString());
                    arr.Add(dataReader["OIB"].ToString());
                    arr.Add(dataReader["Contact"].ToString());
                    arr.Add(dataReader["BIC"].ToString());
                    arr.Add(dataReader["KN"].ToString());
                    arr.Add(dataReader["EUR"].ToString());
                    arr.Add(dataReader["Code"].ToString());
                    arr.Add(dataReader["Country"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["Email"].ToString());
                    arr.Add(dataReader["Phone"].ToString());
                    arr.Add(dataReader["WWW"].ToString());
                    arr.Add(dataReader["MB"].ToString());
                    arr.Add(dataReader["IBAN"].ToString());
                    arr.Add(dataReader["SupportEmail"].ToString());
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

        public List<String> AllMainCmpInfoSortByName()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from MainCmp order by CmpName asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["CmpName"].ToString());
                    arr.Add(dataReader["Address"].ToString());
                    arr.Add(dataReader["City"].ToString());
                    arr.Add(dataReader["PB"].ToString());
                    arr.Add(dataReader["OIB"].ToString());
                    arr.Add(dataReader["Contact"].ToString());
                    arr.Add(dataReader["BIC"].ToString());
                    arr.Add(dataReader["KN"].ToString());
                    arr.Add(dataReader["EUR"].ToString());
                    arr.Add(dataReader["Code"].ToString());
                    arr.Add(dataReader["Country"].ToString());
                    arr.Add(dataReader["regionID"].ToString());
                    arr.Add(dataReader["Email"].ToString());
                    arr.Add(dataReader["Phone"].ToString());
                    arr.Add(dataReader["WWW"].ToString());
                    arr.Add(dataReader["MB"].ToString());
                    arr.Add(dataReader["IBAN"].ToString());
                    arr.Add(dataReader["SupportEmail"].ToString());
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


        public Boolean AddMainCmp(long mID, String mCmpName, String mAddress, String mCity, String mPB, String mVAT, String mContact,String mBIC, decimal mKN, decimal mEUR, String mCode, String mCountry, String mRegionID, String mEmail, String mPhone, String mWWW, String mMB, String mIBAN, String mSupportEmail, Boolean mSaveToDB)
        {

            Boolean executed = false;
            String code = "";
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);

            if (mID != 0)
            {
                query = "select Code from MainCmp t where t.ID =" + mID;
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();
                SqlDataReader dataReader = command.ExecuteReader();
                dataReader.Read();
                if (!dataReader.HasRows && !dataReader.GetValue(0).Equals(0))
                {
                    executed = false;
                    dataReader.Close();
                }
                else
                {
                    code = dataReader["Code"].ToString();
                    dataReader.Close(); 
                }
            }

            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                if (mID != 0 && mSaveToDB)
                {
                    command.CommandText = "UPDATE MainCmp SET CmpName = '" + mCmpName + "', Address = '" + mAddress + "', City = '" + mCity + "', PB = '" + mPB + "', OIB = '" + mVAT + "', " +
                        "Contact = '" + mContact + "', BIC = '" + mBIC + "', KN = " + mKN + ", EUR = " + mEUR + ", Code = '" + mCode + "', Country = '" + mCountry + "', RegionID = " + mRegionID
                            + ", Email = '" + mEmail + "', Phone = '" + mPhone + "', www = '" + mWWW + "', MB = '" + mMB + "', IBAN = '" + mIBAN + "', SupportEmail = '" + mSupportEmail + 
                        "' WHERE ID = " + mID;
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = "INSERT INTO MainCmp (CmpName, Address, City, PB, BIC, Contact, OIB, KN, EUR, Code, Country, RegionID, Email, Phone, WWW, MB, IBAN, SupportEmail)" +
                            " values " 
                            + "('" + mCmpName + "', '" + mAddress + "', '" + mCity + "', '" + mPB + "', '" + mVAT + "', '" + mContact + "', '" + mBIC + "', " + mKN + ", " + mEUR + ", '" + mCode +
                            "', '" + mCountry + "', " + mRegionID + ", '" + mEmail + "', '" + mPhone + "', '" + mWWW + "', '" + mMB + "', '" + mIBAN + "', '" + mSupportEmail + "')";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                executed = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    executed = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }
            
            cnn.Close();
            return executed;
        }
        
        public List<long> ISSExistReturnPartsIDs(long mISSid)
        {
            List<long> listOfIDs = new List<long>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select OldPartID from ISSparts where ISSid = " + mISSid + " ORDER BY RB";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    listOfIDs.Add(long.Parse(dataReader.GetValue(0).ToString()));
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return listOfIDs;
        }

        public long ISSExistIfNotReturnNewID(long mISSid)
        {
            long ISSCnt = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from ISS where ID = " + mISSid;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();
            
            if (dataReader.GetValue(0).ToString().Equals("0"))
            {
                dataReader.Close();
                query = "select Count(ID) from ISS where ID LIKE '" + DateTime.Now.ToString("yy") + "%'";
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();
                dataReader = command.ExecuteReader();
                dataReader.Read();

                ISSCnt = long.Parse(dataReader.GetValue(0).ToString());
                ISSCnt = long.Parse(DateTime.Now.ToString("yy") + string.Format("{0:0000}", (ISSCnt + 1)));
            }
            else
            {
                ISSCnt = mISSid;
            }
           
            dataReader.Close();
            cnn.Close();
            return ISSCnt;
        }

        public Boolean ISSRBExist(long mISSid, long mRB)
        {
            Boolean exist = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(RB) from ISSparts where ISSid = " + mISSid + " and RB = " + mRB;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.GetValue(0).ToString().Equals("0"))
                exist = false;
            else
                exist = true;

            dataReader.Close();
            cnn.Close();
            return exist;
        }

        public List<long> GetISSRBByISSid(long mISSid)
        {
            List<long> arr = new List<long>();
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select RB from ISSparts where ISSid = " + mISSid;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["RB"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(0);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public Boolean ISSUnesiISS(Boolean mISSExist, Boolean mAllDone, long mISSid, String mDate, Company mCmpCustomer, Part mMainPart, List<ISSparts> listIssParts, long mUserID, String mTotalTIme, long mLevel)
        {
            Boolean isExecuted = false;
            int AllDone = mAllDone ? 1 : 0;
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction("T1");
            command.Connection = cnn;
            command.Transaction = transaction;
            
            try
            {
                if (mISSExist)
                {
                    command.CommandText = "UPDATE ISS SET TotalTime = '" + mTotalTIme + "' where ID = " + mISSid;
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE ISS SET Level = '" + mLevel + "' where ID = " + mISSid;
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = "INSERT INTO ISS (ID, Date, UserID, CustomerID, PartID, Closed, TotalTime, Level) VALUES (" + mISSid + ", '" + mDate + "', " + WorkingUser.UserID + ", " + mCmpCustomer.ID + ", " + mMainPart.PartID + ", " + AllDone + ", '" + mTotalTIme + "', " + mLevel + ")";
                    command.ExecuteNonQuery();

                }
                
                if (mAllDone)
                {
                    command.CommandText = "UPDATE Parts SET State = 'sg' where PartID = " + mMainPart.PartID; 
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE Parts SET DateSend = '" + DateTime.Now.ToString("dd.MM.yy.") + "' where PartID = " + mMainPart.PartID;
                    command.ExecuteNonQuery();
                }

                if (mISSExist && mAllDone)
                {
                    command.CommandText = "UPDATE ISS SET Closed = 1, Level = 0 where ID = " + mISSid;
                    command.ExecuteNonQuery();
                }

                QueryCommands qc1 = new QueryCommands();
                List<long> arr = new List<long>();

                arr = qc1.GetISSRBByISSid(mISSid);
                for (int i = 0; i < listIssParts.Count; i++)
                {
                    if (!arr.Contains(listIssParts[i].RB))
                    {
                        if (listIssParts[i].PrtO.PartialCode != 0)
                        {
                            command.CommandText = "INSERT INTO Parts(PartialCode, SN, CN, DateIn, DateOut, DateSend, StorageID, State, CompanyO, CompanyC) VALUES(" +
                                listIssParts[i].PrtO.PartialCode + ", '" + listIssParts[i].PrtO.SN + "', '" + listIssParts[i].PrtO.CN + "', '" + mDate +
                                "', '', '', " + WorkingUser.RegionID + ", 'ng', '" + listIssParts[i].PrtO.CompanyO + "', '" + listIssParts[i].PrtO.CompanyC + "'); SELECT SCOPE_IDENTITY()";

                            var retVal = command.ExecuteScalar();
                            listIssParts[i].PrtO.PartID = long.Parse(retVal.ToString());
                        }

                        command.CommandText = "INSERT INTO ISSparts (ISSid, RB, oldPartID, newPartID, Work, Comment, Time, UserID, Date) " +
                            "VALUES (" +
                            mISSid + ", " + listIssParts[i].RB + ", " + listIssParts[i].PrtO.PartID + ", " + listIssParts[i].PrtN.PartID + ", '" + listIssParts[i].Work + "', '" + listIssParts[i].Comment + "', '" + listIssParts[i].Time + "', " + mUserID + ", '" + listIssParts[i].Date + "')";
                        command.ExecuteNonQuery();
                    
                        if (listIssParts[i].PrtN.PartialCode != 0)
                        {
                            listIssParts[i].PrtN.DateOut = DateTime.Now.ToString("dd.MM.yy.");
                            command.CommandText = "UPDATE Parts SET DateOut = '" + DateTime.Now.ToString("dd.MM.yy.") + "' where PartID = " + listIssParts[i].PrtN.PartID;
                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO PartsZamijenjeno SELECT * FROM Parts p WHERE p.partID = " + listIssParts[i].PrtN.PartID;
                            command.ExecuteNonQuery();

                            command.CommandText = "DELETE FROM Parts WHERE partID = " + listIssParts[i].PrtN.PartID;
                            command.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                isExecuted = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback("T1");
                    isExecuted = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return isExecuted;
        }

        public List<long> GetAllISSOpenClose(long mDone)
        {
            List<long> arr = new List<long>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select ID from ISS where Closed = " + mDone;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["ID"].ToString()));
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<ISSreport> GetAllISSInfoOpenClose(long mDone)
        {
            List<ISSreport> arr = new List<ISSreport>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from ISS where Closed = " + mDone;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    ISSreport tmpissr = new ISSreport();

                    tmpissr.ISSid = long.Parse(dataReader["ID"].ToString());
                    tmpissr.Date = dataReader["Date"].ToString();
                    tmpissr.UserIDmaked = long.Parse(dataReader["UserID"].ToString());
                    tmpissr.CustomerID = long.Parse(dataReader["CustomerID"].ToString());
                    tmpissr.PartID = long.Parse(dataReader["PartID"].ToString());
                    tmpissr.Closed = long.Parse(dataReader["Closed"].ToString());
                    tmpissr.TotalTIme = dataReader["TotalTIme"].ToString();
                    tmpissr.Level = long.Parse(dataReader["Level"].ToString());

                    arr.Add(tmpissr);

                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<ISSparts> GetAllISSStillWorkingOn(List<long> mISSids)
        {
            List<ISSparts> arr = new List<ISSparts>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);

            if(mISSids.Count > 0)
            {
                query = "select * from ISSparts where ISSid = " + mISSids[0];

                for (int i = 1; i  < mISSids.Count; i++)
                {
                    query += " or ISSid = " + mISSids[i];
                }

                query += " order by ISSid asc";
                
                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();
                SqlDataReader dataReader = command.ExecuteReader();
                dataReader.Read();

                if (dataReader.HasRows)
                {
                    do
                    {
                        ISSparts tempPrt = new ISSparts();
                        Part prt = new Part();

                        tempPrt.ISSid = long.Parse(dataReader["ISSid"].ToString());
                        tempPrt.RB = long.Parse(dataReader["RB"].ToString());

                        tempPrt.PrtO = prt.GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(long.Parse(dataReader["oldPartID"].ToString()));
                        tempPrt.PrtN = prt.GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(long.Parse(dataReader["newPartID"].ToString()));

                        tempPrt.Comment = dataReader["Comment"].ToString();
                        tempPrt.Work = dataReader["Work"].ToString();
                        tempPrt.Time = dataReader["Time"].ToString();
                        tempPrt.UserID = long.Parse(dataReader["UserID"].ToString());
                        tempPrt.Date = dataReader["Date"].ToString();

                        arr.Add(tempPrt);
                    } while (dataReader.Read());
                }

                dataReader.Close();
            }

            cnn.Close();
            return arr;
        }

        public List<ISSparts> GetAllISSPartsByISSid(long mISSid)
        {
            List<ISSparts> arr = new List<ISSparts>();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from ISSparts where ISSid = " + mISSid + " order by RB asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    ISSparts tempPrt = new ISSparts();
                    Part prt = new Part();

                    tempPrt.ISSid = long.Parse(dataReader["ISSid"].ToString());
                    tempPrt.RB = long.Parse(dataReader["RB"].ToString());

                    tempPrt.PrtO = prt.GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(long.Parse(dataReader["oldPartID"].ToString()));
                    tempPrt.PrtN = prt.GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(long.Parse(dataReader["newPartID"].ToString()));

                    tempPrt.Comment = dataReader["Comment"].ToString();
                    tempPrt.Work = dataReader["Work"].ToString();
                    tempPrt.Time = dataReader["Time"].ToString();
                    tempPrt.UserID = long.Parse(dataReader["UserID"].ToString());
                    tempPrt.Date = dataReader["Date"].ToString();

                    arr.Add(tempPrt);
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public long GetMainPartIDISSById(long mISSid)
        {
            long partID = 0;
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select PartID from ISS where ID = " + mISSid;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                partID = long.Parse(dataReader["PartID"].ToString());
            }

            dataReader.Close();
            cnn.Close();
            return partID;
        }

        public long GetISSidByPartID(long mPartID)
        {
            long issID= 0;
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select ID from ISS where PartID = " + mPartID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                issID = long.Parse(dataReader["ID"].ToString());
            }

            dataReader.Close();
            cnn.Close();
            return issID;
        }

        public List<String> GetAllISSInfoById(long mISSid)
        {
            List<String> arr = new List<String>();
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from ISS where ID = " + mISSid;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                arr.Add(dataReader["ID"].ToString());
                arr.Add(dataReader["Date"].ToString());
                arr.Add(dataReader["UserID"].ToString());
                arr.Add(dataReader["CustomerID"].ToString());
                arr.Add(dataReader["PartID"].ToString());
                arr.Add(dataReader["Closed"].ToString());
                arr.Add(dataReader["TotalTime"].ToString());
                arr.Add(dataReader["Level"].ToString());
            }
            else
            {
                arr.Add("nok");
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public Boolean RemoveAllISSParts(long mISSid)
        {
            Boolean isExecuted = false;
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction("T1");
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "DELETE FROM ISSparts WHERE  ISSid = " + mISSid;
                command.ExecuteNonQuery();

                transaction.Commit();
                isExecuted = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback("T1");
                    isExecuted = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return isExecuted;
        }

        public List<String> GetAllFromWorkHR()
        {
            List<String> arr = new List<String>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Odradeno from Work order by Value";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            //dataReader.Read();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    arr.Add(dataReader["Odradeno"].ToString());
                }
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllFromWorkENG()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select WorkDone from Work order by Value";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["WorkDone"].ToString());

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

        public List<WorksISS> GetAllFromWork()
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select * from Work order by Value";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<WorksISS> tmpLst = new List<WorksISS>();

            if (dataReader.HasRows)
            {
                do
                {
                    WorksISS tmp = new WorksISS();
                    
                    tmp.Hrv = dataReader["Odradeno"].ToString();
                    tmp.Eng = dataReader["WorkDone"].ToString();
                    tmp.Id = dataReader["Value"].ToString();

                    tmpLst.Add(tmp);

                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }
            dataReader.Close();
            cnn.Close();
            return tmpLst;
        }

        public Boolean SaveToWork(List<WorksISS> _lst)
        {
            Boolean isExecuted = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "DELETE FROM Work";

                command.ExecuteNonQuery();

                foreach (WorksISS work in _lst)
                {
                    command.CommandText = "INSERT INTO Work (Odradeno, WorkDone, Value) VALUES ('" + work.Hrv + "', '" + work.Eng + "', '" + work.Id + "')";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                isExecuted = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    isExecuted = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return isExecuted;
        }

        public List<String> GetAllInfoISSBy(String what, String value, String orderBy)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            if (what.Equals("Date"))
                query = "Select * from ISS where " + what + " = '" + value + "' order by " + orderBy;
            else
                query = "Select * from ISS where " + what + " = " + value + "' order by " + orderBy;

            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["ID"].ToString());
                    arr.Add(dataReader["Date"].ToString());
                    arr.Add(dataReader["UserID"].ToString());
                    arr.Add(dataReader["CustomerID"].ToString());
                    arr.Add(dataReader["PartID"].ToString());
                    arr.Add(dataReader["Closed"].ToString());
                    arr.Add(dataReader["TotalTime"].ToString());
                    arr.Add(dataReader["Level"].ToString());
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

        public List<ISSreport> GetAllInfoISSByClosed(String what, String value)
        {
            List<ISSreport> arr = new List<ISSreport>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            if (what.Equals("Date"))
                query = "Select * from ISS where " + what + " = '" + value + "' and Closed = 1 order by ID";
            else
                query = "Select * from ISS where " + what + " = " + value + " and Closed = 1 order by ID";

            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();
            
            if (dataReader.HasRows)
            {
                do
                {
                    ISSreport rpt = new ISSreport();

                    rpt.ISSid = long.Parse(dataReader["ID"].ToString());
                    rpt.Date = dataReader["Date"].ToString();
                    rpt.UserIDmaked = long.Parse(dataReader["UserID"].ToString());
                    rpt.CustomerID = long.Parse(dataReader["CustomerID"].ToString());
                    rpt.PartID = long.Parse(dataReader["PartID"].ToString());
                    rpt.Closed = long.Parse(dataReader["Closed"].ToString());
                    rpt.TotalTIme = dataReader["TotalTime"].ToString();
                    rpt.Level = long.Parse(dataReader["Level"].ToString());

                    arr.Add(rpt);
                    
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<long> GetAllISScustomerID(long mClosed)
        {
            List<long> arr = new List<long>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct CustomerID from ISS where Closed =" + mClosed + " order by CustomerID asc";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(long.Parse(dataReader["CustomerID"].ToString()));
                } while (dataReader.Read());
            }
            else
            {
                arr.Add(-1);
            }
            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public List<String> GetAllISSdateCreated(long mClosed)
        {
            List<String> arr = new List<string>();
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "Select Distinct Date from ISS where Closed =" + mClosed;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                do
                {
                    arr.Add(dataReader["Date"].ToString());
                } while (dataReader.Read());
            }
            else
            {
                arr.Add("nok");
            }

            try
            {
                List<DateTime> dates = new List<DateTime>();

                if (!arr[0].Equals("nok"))
                {
                    foreach (String a in arr)
                    {
                        dates.Add(DateTime.Parse(a));
                    }

                    dates.Sort();
                    arr.Clear();
                    DateConverter dc = new DateConverter();

                    foreach (DateTime a in dates)
                    {
                        arr.Add(dc.ConvertDDMMYY(a.ToShortDateString()));
                    }
                }

            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }

            dataReader.Close();
            cnn.Close();
            return arr;
        }

        public long GetNewInvoiceID()
        {
            long invID = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Racun where ID LIKE '" + "%" + DateTime.Now.ToString("yy") + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.GetValue(0).ToString().Equals("0"))
                invID = long.Parse(dataReader.GetValue(0).ToString()) + 1;
            else
                invID = 1;

            dataReader.Close();
            cnn.Close();
            return invID;
        }

        public long GetLastInvoiceID()
        {
            long invID = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Racun where ID LIKE '" + DateTime.Now.ToString("yy") + "%'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.GetValue(0).ToString().Equals("0"))
                invID = long.Parse(dataReader.GetValue(0).ToString());
            else
                invID = 1;

            dataReader.Close();
            cnn.Close();
            return invID;
        }

        public Boolean SaveInvoice(List<InvoiceParts> mInvoiceList, Invoice mInvoice, int mStorno)
        {
            Boolean isExecuted = false;
            long newInvId = mInvoice.GetNewInvoiceNumber();
            //long newInvId = mInvoice.Id;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            String eur = String.Format("{0:N4}", (double)mInvoice.Eur);
            String iznos = String.Format("{0:N2}", (double)mInvoice.Iznos);
            try
            {
                command.CommandText = "INSERT INTO Racun (ID, PonudaID, DatumIzdano, Iznos, DatumNaplaceno, Naplaceno, CustomerID, EUR, Napomena, VrijemeIzdano, Valuta, Operater, DanTecaja, NacinPlacanja, Storno, Konverzija) VALUES ("
                    + newInvId + ", " + mInvoice.PonudaID + ", '" + mInvoice.DatumIzdano + "', '" + iznos + "', '" + mInvoice.DatumNaplaceno + "', " 
                    + mInvoice.Naplaceno + ", " + mInvoice.CustomerID + ", '" + eur + "', '" + mInvoice.Napomena + "', '" + mInvoice.VrijemeIzdano + "', " 
                    + mInvoice.Valuta + ", '" + mInvoice.Operater + "', '" + mInvoice.DanTecaja + "', '" + mInvoice.NacinPlacanja + "', " + mStorno + ", " + mInvoice.Konverzija + ")";

                command.ExecuteNonQuery();

                foreach (InvoiceParts prt in mInvoiceList)
                {
                    command.CommandText = "INSERT INTO RacunParts (RacunID, PartCode, VrijemeRada, Rabat, Kolicina, iznosRabat, iznosTotal, iznosPart) VALUES (" + newInvId + ", " + prt.PartCode + ", '" + prt.VrijemeRada + "', " + prt.Rabat.ToString().Replace(',','.') + ", " + prt.Kolicina + ", '" + prt.IznosRabat + "', '" + prt.IznosTotal + "', '" + prt.IznosPart + "')";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                isExecuted = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    isExecuted = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return isExecuted;
        }

        public Boolean IfInvoiceExist(long _InvocieID)
        {
            Boolean exist = true;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Racun where ID = " + _InvocieID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            try
            {

                if (!dataReader.GetValue(0).ToString().Equals("0"))
                    exist = true;
                else
                    exist = false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dataReader.Close();
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            dataReader.Close();
            cnn.Close();
            return exist;
        }

        public List<Invoice> GetAllInvoices()
        {
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from Racun";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<Invoice> tempList = new List<Invoice>();

            if (dataReader.HasRows)
            {
                do
                {
                    Invoice tempInv = new Invoice(
                    long.Parse(dataReader["ID"].ToString()),
                    long.Parse(dataReader["PonudaID"].ToString()),
                    dataReader["DatumIzdano"].ToString(),
                    decimal.Parse(dataReader["Iznos"].ToString()),
                    dataReader["DatumNaplaceno"].ToString(),
                    decimal.Parse(dataReader["Naplaceno"].ToString()),
                    long.Parse(dataReader["CustomerID"].ToString()),
                    decimal.Parse(dataReader["Eur"].ToString()),
                    dataReader["Napomena"].ToString(),
                    dataReader["VrijemeIzdano"].ToString(),
                    int.Parse(dataReader["Valuta"].ToString()),
                    dataReader["Operater"].ToString(),
                    dataReader["DanTecaja"].ToString(),
                    dataReader["NacinPlacanja"].ToString(),
                    int.Parse(dataReader["Storno"].ToString()),
                    int.Parse(dataReader["Konverzija"].ToString()));

                    tempList.Add(tempInv);

                } while (dataReader.Read());
            }

            SortInvoiceByDateIzdano(ref tempList, "desc");

            dataReader.Close();
            cnn.Close();
            return tempList;
        }

        ////////////////////////////////////////////////////
        ///

        public long GetNewOfferID()
        {
            long offID = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Ponuda where ID LIKE '" + "%" + DateTime.Now.ToString("yy") + "'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.GetValue(0).ToString().Equals("0"))
                offID = long.Parse(dataReader.GetValue(0).ToString()) + 1;
            else
                offID = 1;

            dataReader.Close();
            cnn.Close();
            return offID;
        }

        public long GetLastOfferID()
        {
            long offID = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Ponuda where ID LIKE '" + DateTime.Now.ToString("yy") + "%'";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (!dataReader.GetValue(0).ToString().Equals("0"))
                offID = long.Parse(dataReader.GetValue(0).ToString());
            else
                offID = 1;

            dataReader.Close();
            cnn.Close();
            return offID;
        }

        public Boolean SaveOffer(List<OfferParts> mOfferList, Offer mOffer, int mStorno)
        {
            Boolean isExecuted = false;
            long newInvId = mOffer.GetNewOfferNumber();

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            command = cnn.CreateCommand();
            SqlTransaction transaction = cnn.BeginTransaction();
            command.Connection = cnn;
            command.Transaction = transaction;

            String eur = String.Format("{0:N4}", (double)mOffer.Eur);
            String iznos = String.Format("{0:N2}", (double)mOffer.Iznos);
            try
            {
                command.CommandText = "INSERT INTO Ponuda (ID, RacunID, DatumIzdano, Iznos, DatumNaplaceno, Naplaceno, CustomerID, EUR, Napomena, VrijemeIzdano, Valuta, Operater, DanTecaja, NacinPlacanja, Storno, Konverzija) VALUES ("
                    + newInvId + ", " + mOffer.RacunID + ", '" + mOffer.DatumIzdano + "', '" + iznos + "', '" + mOffer.DatumNaplaceno + "', "
                    + mOffer.Naplaceno + ", " + mOffer.CustomerID + ", '" + eur + "', '" + mOffer.Napomena + "', '" + mOffer.VrijemeIzdano + "', "
                    + mOffer.Valuta + ", '" + mOffer.Operater + "', '" + mOffer.DanTecaja + "', '" + mOffer.NacinPlacanja + "', " + mStorno + ", " + mOffer.Konverzija + ")";
                command.ExecuteNonQuery();

                foreach (OfferParts prt in mOfferList)
                {
                    command.CommandText = "INSERT INTO PonudaParts (PonudaID, PartCode, VrijemeRada, Rabat, Kolicina, iznosRabat, iznosTotal, iznosPart) VALUES (" + newInvId + ", " + prt.PartCode + ", '" + prt.VrijemeRada + "', " + prt.Rabat.ToString().Replace(',', '.') + ", " + prt.Kolicina + ", '" + prt.IznosRabat + "', '" + prt.IznosTotal + "', '" + prt.IznosPart + "')";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                isExecuted = true;
            }
            catch (Exception)
            {
                //new LogWriter(e1);
                try
                {
                    transaction.Rollback();
                    isExecuted = false;
                    throw;
                }
                catch (Exception)
                {
                    //new LogWriter(e2);
                    throw;
                }
            }
            finally
            {
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            cnn.Close();
            return isExecuted;
        }

        public Boolean IfOfferExist(long _InvocieID)
        {
            Boolean exist = true;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Ponuda where ID = " + _InvocieID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            try
            {

                if (!dataReader.GetValue(0).ToString().Equals("0"))
                    exist = true;
                else
                    exist = false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dataReader.Close();
                if (cnn.State.ToString().Equals("Open"))
                    cnn.Close();
            }

            dataReader.Close();
            cnn.Close();
            return exist;
        }

        public List<Offer> GetAllOffers()
        {
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from Ponuda";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<Offer> tempList = new List<Offer>();

            if (dataReader.HasRows)
            {
                do
                {
                    Offer tempOff = new Offer(
                    long.Parse(dataReader["ID"].ToString()),
                    long.Parse(dataReader["RacunID"].ToString()),
                    dataReader["DatumIzdano"].ToString(),
                    decimal.Parse(dataReader["Iznos"].ToString()),
                    dataReader["DatumNaplaceno"].ToString(),
                    decimal.Parse(dataReader["Naplaceno"].ToString()),
                    long.Parse(dataReader["CustomerID"].ToString()),
                    decimal.Parse(dataReader["Eur"].ToString()),
                    dataReader["Napomena"].ToString(),
                    dataReader["VrijemeIzdano"].ToString(),
                    int.Parse(dataReader["Valuta"].ToString()),
                    dataReader["Operater"].ToString(),
                    dataReader["DanTecaja"].ToString(),
                    dataReader["NacinPlacanja"].ToString(),
                    int.Parse(dataReader["Storno"].ToString()),
                    int.Parse(dataReader["Konverzija"].ToString()));

                    tempList.Add(tempOff);

                } while (dataReader.Read()) ;
            }

            dataReader.Close();
            cnn.Close();
            return tempList;
        }

        public Boolean UpdateInvoicePaid(long m_invID, decimal m_Iznos, string m_date)
        {
            Boolean executed = false;
            
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Racun";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "UPDATE Racun SET Naplaceno = '" + m_Iznos + "', DatumNaplaceno = '" + m_date + "' where ID = " + m_invID;
                    command.ExecuteNonQuery();
                    
                    transaction.Commit();
                    executed = true;
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public Boolean UpdateInvoicePaidTo0(long m_invID)
        {
            Boolean executed = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Racun";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "UPDATE Racun SET Naplaceno = '0' , DatumNaplaceno = '01.01.01.' where ID = " + m_invID;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    executed = true;
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public Boolean SetStornoInvoice0Or1(Invoice inv)
        {
            Boolean executed = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(ID) from Racun";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "UPDATE Racun SET Storno = " + inv.Storno + ", DatumNaplaceno = '" + DateTime.Now.ToString("dd.MM.yy.") + "' where ID = " + inv.Id;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    executed = true;
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public List<InvoiceParts> GetAllPartsByInvoiceId(long _invID)
        {
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from RacunParts where RacunID = " + _invID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<InvoiceParts> tempList = new List<InvoiceParts>();

            if (dataReader.HasRows)
            {
                do
                {
                    InvoiceParts tmpInvoiceParts = new InvoiceParts();

                    tmpInvoiceParts.RacunID = long.Parse(dataReader["RacunID"].ToString());
                    tmpInvoiceParts.PartCode = long.Parse(dataReader["PartCode"].ToString());
                    tmpInvoiceParts.VrijemeRada = dataReader["VrijemeRada"].ToString();
                    tmpInvoiceParts.Rabat = decimal.Parse(dataReader["Rabat"].ToString());
                    tmpInvoiceParts.Kolicina = int.Parse(dataReader["Kolicina"].ToString());
                    tmpInvoiceParts.IznosRabat = dataReader["IznosRabat"].ToString();
                    tmpInvoiceParts.IznosTotal = dataReader["IznosTotal"].ToString();
                    tmpInvoiceParts.IznosPart = dataReader["IznosPart"].ToString();

                    tempList.Add(tmpInvoiceParts);

                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return tempList;
        }

        public List<long> GetAllNewPartsIdByTidIDFromRN(long _tidID)
        {
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select PartIDNew from RN where PartIDNew <> 0 and TicketIDRN = " + _tidID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<long> tempList = new List<long>();

            if (dataReader.HasRows)
            {
                do
                {
                    tempList.Add(long.Parse(dataReader["PartIDNew"].ToString()));
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return tempList;
        }

        public List<long> GetAllOldPartsIdByTidIDFromRN(long _tidID)
        {
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select PartIDOld from RN where PartIDOld <> 0 and TicketIDRN = " + _tidID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<long> tempList = new List<long>();

            if (dataReader.HasRows)
            {
                do
                {
                    tempList.Add(long.Parse(dataReader["PartIDOld"].ToString()));
                } while (dataReader.Read());
            }

            dataReader.Close();
            cnn.Close();
            return tempList;
        }

        public List<String> GetAllInfoByTidIDFromRN(long _tidID)
        {
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select * from RN where TicketIDRN = " + _tidID;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            List<String> tempList = new List<String>();

            if (dataReader.HasRows)
            {
                tempList.Add(dataReader["TicketIDRN"].ToString());
                tempList.Add(dataReader["ErrorCode"].ToString());
                tempList.Add(dataReader["Description"].ToString());
                tempList.Add(dataReader["RB"].ToString());
            }
            else
            {
                tempList.Add("nok");
            }

            dataReader.Close();
            cnn.Close();
            return tempList;
        }

        public Boolean AddPartToSifrarnik(long mCategoryCode, String mCategoryName, long mPartCode, String mPartName, long mSubPartCode, String mSubPartName, String mPartNumber, double mPriceInKn, double mPriceOutKn, double mPriceInEur, double mPriceOutEur, String mPacking)
        {
            Boolean executed = false;
            //SqlConnection cnn = cn.Connect(Uname, Pass);
            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(FullName) from Sifrarnik s where s.CategoryCode = " + mCategoryCode + " and PartCode = " + mPartCode + " and SubPartCode = " + mSubPartCode;
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
                    command.CommandText = "INSERT INTO Sifrarnik (CategoryCode, CategoryName, PartCode, PartName, SubPartCode, SubPartName, PartNumber, PriceInKn, PriceOutKn, PriceInEur, PriceOutEur, Packing) " +
                        "values (" 
                        + mCategoryCode + ", '" 
                        + mCategoryName + "', " 
                        + mPartCode + ", '" 
                        + mPartName + "', " 
                        + mSubPartCode + ", '" 
                        + mSubPartName + "', '" 
                        + mPartNumber + "', " 
                        + mPriceInKn.ToString().Replace(',', '.') + ", " 
                        + mPriceOutKn.ToString().Replace(',', '.') + ", " 
                        + mPriceInEur.ToString().Replace(',', '.') + ", " 
                        + mPriceOutEur.ToString().Replace(',', '.') + ", '" 
                        + mPacking + "')";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    executed = true;
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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

        public Boolean UpdatePartSifrarnik(long mCategoryCode, String mCategoryName, long mPartCode, String mPartName, long mSubPartCode, String mSubPartName, String mPartNumber, double mPriceInKn, double mPriceOutKn, double mPriceInEur, double mPriceOutEur, String mPacking)
        {
            Boolean executed = false;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(CategoryCode) from Sifrarnik";
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                dataReader.Close();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "UPDATE Sifrarnik SET " +
                        "CategoryCode = " + mCategoryCode +
                        ", CategoryName = '" + mCategoryName +
                        "', PartCode = " + mPartCode +
                        ", PartName = '" + mPartName +
                        "', SubPartCode = " + mSubPartCode +
                        ", SubPartName = '" + mSubPartName +
                        "', PartNumber = '" + mPartNumber +
                        "', PriceInKn = " + mPriceInKn.ToString().Replace(',','.') +
                        ", PriceOutKn = " + mPriceOutKn.ToString().Replace(',', '.') +
                        ", PriceInEur = " + mPriceInEur.ToString().Replace(',', '.') +
                        ", PriceOutEur = " + mPriceOutEur.ToString().Replace(',', '.') +
                        ", Packing = '" + mPacking + "' where FullCode = " + (mCategoryCode + mPartCode + mSubPartCode);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                    executed = true;
                }
                catch (Exception)
                {
                    //new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        executed = false;
                        throw;
                    }
                    catch (Exception)
                    {
                        //new LogWriter(e2);
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


        ////////////////////////////////  FUNCTIONS  //////////////////////////////////////////////////


        private void SortArrStringDatesAsc(ref List<String> arr)
        {
            try
            {
                List<DateTime> dates = new List<DateTime>();

                if (!arr[0].Equals("nok"))
                {
                    foreach (String a in arr)
                    {
                        dates.Add(DateTime.Parse(a));
                    }

                    dates.Sort();
                    arr.Clear();
                    DateConverter dc = new DateConverter();

                    foreach (DateTime a in dates)
                    {
                        arr.Add(dc.ConvertDDMMYY(a.ToShortDateString()));
                    }
                }

            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
        }

        private void SortArrStringDatesDesc(ref List<String> arr)
        {
            try
            {
                List<DateTime> dates = new List<DateTime>();

                if (!arr[0].Equals("nok"))
                {
                    foreach (String a in arr)
                    {
                        dates.Add(DateTime.Parse(a));
                    }

                    dates.Sort((a, b) => b.CompareTo(a));
                    arr.Clear();
                    DateConverter dc = new DateConverter();

                    foreach (DateTime a in dates)
                    {
                        arr.Add(dc.ConvertDDMMYY(a.ToShortDateString()));
                    }
                }

            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
        }
        
        private void SortInvoiceByDateIzdano(ref List<Invoice> t, String AscDesc)
        {
            try
            {
                if (t.Count > 0)
                    if(AscDesc.ToUpper().Equals("ASC"))
                        t.Sort((a, b) => DateTime.Parse(a.DatumIzdano).CompareTo(DateTime.Parse(b.DatumIzdano)));
                    else
                        t.Sort((a, b) => DateTime.Parse(b.DatumIzdano).CompareTo(DateTime.Parse(a.DatumIzdano)));

            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
        }

        private void SortLong(ref List<long> t, String AscDesc)
        {
            try
            {
                if (t.Count > 0)
                    if (AscDesc.ToUpper().Equals("ASC"))
                        t.Sort((a, b) => a.CompareTo(b));
                    else
                        t.Sort((a, b) => b.CompareTo(a));

            }
            catch (Exception)
            {
                //new LogWriter(e1);
                throw;
            }
        }

        public int GetFullCount(String tableName)
        {
            int value = 0;

            cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
            query = "select Count(0) from " + tableName;
            command = new SqlCommand(query, cnn);
            command.ExecuteNonQuery();
            SqlDataReader dataReader = command.ExecuteReader();
            dataReader.Read();

            if (dataReader.HasRows)
            {
                value = int.Parse(dataReader.GetValue(0).ToString());
            }

            dataReader.Close();
            cnn.Close();
            return value;
        }


        //KORISTIO ZA NOGOMET

        //public void NewUser(String Uname, String Pass)
        //{
        //    //SqlConnection cnn = cn.Connect(Uname, Pass);
        //    cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password);
        //    command = cnn.CreateCommand();
        //    SqlTransaction transaction = cnn.BeginTransaction();
        //    command.Connection = cnn;
        //    command.Transaction = transaction;

        //    try
        //    {
        //        //CriptMe cm = new CriptMe();
        //        //String hashPswd = cm.Cript(value[3]);

        //        //command.CommandText = "CREATE LOGIN " + value[2] + " WITH PASSWORD = '" + value[3] + "'";
        //        //command.ExecuteNonQuery();
        //        //command.CommandText = "CREATE USER " + value[2] + " FOR LOGIN " + value[2];
        //        //command.ExecuteNonQuery();
        //        //command.CommandText = "GRANT INSERT,UPDATE,DELETE,SELECT,EXECUTE ON SCHEMA :: dbo TO " + value[2];
        //        //command.ExecuteNonQuery();
        //        command.CommandText = "Insert into Users(Name, Pass, Joined) values('" + Uname + "', '" + Pass + "','" + DateTime.Now.ToString("dd.MM.yy. hh:mm") + "')";
        //        command.ExecuteNonQuery();
        //        transaction.Commit();
        //    }
        //    catch (Exception)
        //    {
        //        //new LogWriter(e1);
        //        try
        //        {
        //            transaction.Rollback();
        //            cnn.Close();
        //            throw;
        //        }
        //        catch (Exception)
        //        {
        //            //new LogWriter(e2);
        //            throw;
        //        }
        //    }
        //    finally
        //    {
        //        if (cnn.State.ToString().Equals("Open"))
        //            cnn.Close();
        //    }

        //    cnn.Close();
        //}
    }
}