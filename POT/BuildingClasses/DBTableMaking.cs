using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.BuildingClasses
{
    class MakeDataBase
    {
        public Boolean MakeDB(String _DBName)
        {
            ConnectionHelper cn = new ConnectionHelper();
            SqlCommand command = new SqlCommand();
            Boolean executed = false;
            int result = 0;

            using (SqlConnection cnn = cn.Connect("admin", "0000"))
            {
                if (!_DBName.Equals(""))
                {
                    try
                    {
                        command.Connection = cnn;
                        command.CommandText = "SELECT database_id FROM sys.databases WHERE Name = '" + _DBName + "'";
                        result = Convert.ToInt32(command.ExecuteScalar());

                        if (result <= 0)
                        {
                            command.CommandText = "CREATE DATABASE " + _DBName;
                            command.ExecuteNonQuery();
                        }

                        executed = true;
                        Properties.Settings.Default.DBBuilded = true;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            return executed;
        }

        public Boolean MakeTables(String _DBName)
        {
            ConnectionHelper cn = new ConnectionHelper();
            SqlCommand command = new SqlCommand();
            Boolean executed = false;
            int i = 0;
            using (SqlConnection cnn = cn.Connect("admin", "0000"))
            {
                SqlTransaction transaction = null;

                if (Properties.Settings.Default.DBBuilded)
                {
                    try
                    {
                        command.Connection = cnn;

                        transaction = cnn.BeginTransaction();
                        command.Transaction = transaction;

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ErrorCode (Item nchar(50), Value nchar(50))";
                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ErrorCodeOther (ErrorValue nvarchar(50), Item nvarchar(50), Value nvarchar(50), Category varchar(50))";
                        command.ExecuteNonQuery();

                        transaction.Commit();

                        executed = true;
                        Properties.Settings.Default.DBTabelsBuilded = true;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            if (transaction != null)
                                transaction.Rollback();
                            throw;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            return executed;
        }
    }
}
