using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace POT
{
    class ConnectionHelper
    {
        public SqlConnection Connect(String Username, String Password)
        {
            string connectionString = "Data Source=" + Properties.Settings.Default.DataSource + ";Initial Catalog=" + Properties.Settings.Default.Catalog  + ";User ID='" + Username + "';Password='" + Password + "';Timeout=2";
            SqlConnection cnn = new SqlConnection(connectionString);

            try
            {
                cnn.Open();
                return cnn;
            }
            catch (Exception)
            {
                //return cnn;
                throw;
            }
        }

        public Boolean TestConnection(SqlConnection cnn)
        {
            try
            {
                return cnn.State.ToString().Equals("Open") ? true : false;
            }
            catch
            {
                throw ;
            }
        }
    }

}