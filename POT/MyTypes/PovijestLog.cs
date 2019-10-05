using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POT.MyTypes
{
    class PovijestLog
    {
        private long logID;
        private String datumUpisa;
        private String datumRada;
        private long sifraNovi;
        private String nazivNovi;
        private String opis;
        private String sNNovi;
        private String sNStari;
        private String cN;
        private long userID;
        private String customerName;
        private String cCN;
        private String cI;
        private String dokument;
        private long sifraStari;
        private string nazivStari;
        private string gNg;

        public Boolean SaveToPovijestLog(List<Part> mNewOldPart, String mDatumRada, String mOpis, String mCustomerName, String mCCN, String mCI, String mDokument, String mGNG)
        {
            Boolean saved = false;

            SqlCommand command;
            ConnectionHelper cn = new ConnectionHelper();
            QueryCommands qc = new QueryCommands();
            using (SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password))
            {
                datumRada = mDatumRada;
                DatumUpisa = DateTime.Now.ToString("dd.MM.yy.");

                try
                {
                    if (mNewOldPart[0].CodePartFull == 0)
                    {
                        SifraNovi = long.Parse(string.Format("{0:00}", mNewOldPart[0].CompanyO) + string.Format("{0:00}", mNewOldPart[0].CompanyC) + string.Format("{0:000000000}", mNewOldPart[0].PartialCode));
                    }
                    else
                    {
                        SifraNovi = mNewOldPart[0].CodePartFull;
                    }

                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    SifraNovi = 0;
                }

                if (mNewOldPart[0].PartialCode != 0)
                    NazivNovi = qc.PartInfoByFullCodeSifrarnik(mNewOldPart[0].PartialCode).FullName;
                else
                    NazivNovi = "";
                opis = mOpis;
                SNNovi = mNewOldPart[0].SN;
                CN = mNewOldPart[0].CN;
                UserID = WorkingUser.UserID;
                customerName = mCustomerName;
                cCN = mCCN;
                cI = mCI;
                dokument = mDokument;

                if (mNewOldPart.Count > 1)
                {
                    if (mNewOldPart[1].PartialCode != 0)
                        NazivStari = qc.PartInfoByFullCodeSifrarnik(mNewOldPart[1].PartialCode).FullName;
                    else
                        NazivStari = "";
                    try
                    {
                        if (mNewOldPart[1].CodePartFull == 0)
                        {
                            SifraStari = long.Parse(string.Format("{0:00}", mNewOldPart[1].CompanyO) + string.Format("{0:00}", mNewOldPart[1].CompanyC) + string.Format("{0:000000000}", mNewOldPart[1].PartialCode));
                        }
                        else
                        {
                            SifraStari = mNewOldPart[1].CodePartFull;
                        }

                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        SifraStari = 0;
                    }
                    SNStari = mNewOldPart[1].SN;
                }

                gNg = mGNG;

                List<String> resultArr = new List<string>();
                command = cnn.CreateCommand();
                SqlTransaction transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    int partCount = mNewOldPart.Count;
                    partCount = SifraNovi == 0 ? partCount / 2 : partCount;
                    for (int i = 0; i < partCount; i++)
                    {
                        command.CommandText = "insert into PovijestLog " +
                            "(DatumUpisa, DatumRada, SifraNovi, NazivNovi, Opis, SNNovi, SNStari, CN, UserID, Customer, CCN, CI, " +
                            "Dokument, SifraStari, NazivStari, gNg)" +
                            " values ('" + DatumUpisa + "', '" + DatumRada + "', " + SifraNovi + ", '" + NazivNovi +
                            "', '" + Opis + "', '" + SNNovi + "', '" + SNStari + "', '" + CN + "', " + UserID +
                            ", '" + CustomerName + "', '" + CCN + "', '" + CI + "', '" + Dokument + "', " + SifraStari +
                            ", '" + NazivStari + "', '" + GNg + "')";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    saved = true;
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    try
                    {
                        transaction.Rollback();
                        saved = false;
                        cnn.Close();
                        throw;
                    }
                    catch (Exception e2)
                    {
                        new LogWriter(e2);
                        saved = false;
                        cnn.Close();
                        //throw;
                    }
                }
                cnn.Close();
                return saved;
            }
        }

        private String izgradiQueryValue(Object value)
        {
            String partQuery = "";

            switch (value.GetType().ToString())
            {
                case "System.Double":
                    partQuery = "'" + String.Format("{0:N2}", (double)value) + "'";
                    break;
                case "System.Decimal":
                    partQuery = value.ToString();
                    break;
                case "System.Int32":
                    partQuery = value.ToString();
                    break;
                case "System.String":
                    partQuery = "'" + value + "'";
                    break;
                case "System.Int64":
                    partQuery = value.ToString();
                    break;
            }

            return partQuery;
        }

        public List<PovijestLog> PretraziPL(String byWhat1, String byWhat2, String byWhat3, Object value1, Object value2, Object value3, String AndOr, Boolean selection)
        {
            String query = "";

            if (selection)
            {
                if (!byWhat1.Equals("") && !byWhat2.Equals("") && !byWhat3.Equals("") && !value1.ToString().Equals("") && !value2.ToString().Equals("") && !value3.ToString().Equals(""))
                    query = "Select * from PovijestLog where " + byWhat1 + " = " + izgradiQueryValue(value1).ToUpper() + " " + AndOr + " " + byWhat2 + " = " + izgradiQueryValue(value2).ToUpper() + " " + AndOr + " " + byWhat3 + " = " + izgradiQueryValue(value3).ToUpper() + " Order By logID";
                else if (!byWhat1.Equals("") && !byWhat2.Equals("") && !value1.ToString().Equals("") && !value2.ToString().Equals(""))
                    query = "Select * from PovijestLog where " + byWhat1 + " = " + izgradiQueryValue(value1).ToUpper() + " " + AndOr + " " + byWhat2 + " = " + izgradiQueryValue(value2).ToUpper() + " Order By logID";
                else
                    query = "Select * from PovijestLog where " + byWhat1 + " = " + izgradiQueryValue(value1).ToUpper() + " Order By logID";
            }
            else
            {
                query = "Select * from PovijestLog where (SifraNovi = " + izgradiQueryValue(value1).ToUpper() + " and SNNovi = " + izgradiQueryValue(value2).ToUpper() + ") Or " +
                    "(SifraStari = " + izgradiQueryValue(value1).ToUpper() + " and SNStari = " + izgradiQueryValue(value2).ToUpper() + ")" + " Order By logID";
            }

            SqlCommand command;
            ConnectionHelper cn = new ConnectionHelper();
            QueryCommands qc = new QueryCommands();

            using (SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password))
            {
                List<PovijestLog> arr = new List<PovijestLog>();

                command = new SqlCommand(query, cnn);
                command.ExecuteNonQuery();
                SqlDataReader dataReader = command.ExecuteReader();
                dataReader.Read();

                if (dataReader.HasRows)
                {
                    do
                    {
                        PovijestLog tmp = new PovijestLog();

                        tmp.LogID = long.Parse(dataReader["logID"].ToString());
                        tmp.DatumUpisa = dataReader["DatumUpisa"].ToString();
                        tmp.DatumRada = dataReader["DatumRada"].ToString();
                        tmp.SifraNovi = long.Parse(dataReader["SifraNovi"].ToString());
                        tmp.NazivNovi = dataReader["NazivNovi"].ToString();
                        tmp.Opis = dataReader["Opis"].ToString();
                        tmp.SNNovi = dataReader["SNNovi"].ToString();
                        tmp.SNStari = dataReader["SNStari"].ToString();
                        tmp.CN = dataReader["CN"].ToString();
                        tmp.UserID = long.Parse(dataReader["UserID"].ToString());
                        tmp.CustomerName = dataReader["Customer"].ToString();
                        tmp.CCN = dataReader["CCN"].ToString();
                        tmp.Dokument = dataReader["Dokument"].ToString();
                        tmp.SifraStari = long.Parse(dataReader["SifraStari"].ToString());
                        tmp.NazivStari = dataReader["NazivStari"].ToString();
                        tmp.GNg = dataReader["gNg"].ToString();

                        arr.Add(tmp);
                    } while (dataReader.Read());
                }

                dataReader.Close();
                cnn.Close();
                return arr;
            }
        }

        public String DatumRada
        {
            get
            {
                return datumRada;
            }
            set
            {
                datumRada = string.Format("{0:dd.MM.yy.}", value.ToUpper());
            }

        }

        public String GNg
        {
            get
            {
                return gNg;
            }
            set
            {
                gNg = value.ToLower().Trim();
            }
        }

        public String Opis
        {
            get
            {
                return opis;
            }
            set
            {
                if (value.ToString().Length > 400)
                {
                    opis = value.Substring(0, 400).Trim();
                }
                else
                {
                    opis = value.Trim();
                }
            }
        }

        public String CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                if (value.ToString().Length > 50)
                {
                    customerName = value.Substring(0, 50).Trim();
                }
                else
                {
                    customerName = value.Trim();
                }
            }
        }

        public String CI
        {
            get
            {
                return cI;
            }
            set
            {
                if (value.ToString().Length > 50)
                {
                    cI = value.ToUpper().Substring(0, 50).Trim();
                }
                else
                {
                    cI = value.ToUpper().Trim();
                }
            }
        }

        public String CCN
        {
            get
            {
                return cCN;
            }
            set
            {
                if (value.ToString().Length > 50)
                {
                    cCN = value.ToUpper().Substring(0, 50).Trim();
                }
                else
                {
                    cCN = value.ToUpper().Trim();
                }
            }
        }

        public String Dokument
        {
            get
            {
                return dokument;
            }
            set
            {
                if (value.ToString().Length > 50)
                {
                    dokument = value.ToUpper().Substring(0, 50).Trim();
                }
                else
                {
                    dokument = value.ToUpper().Trim();
                }
            }
        }

        public String DatumUpisa
        {
            get
            {
                return datumUpisa;
            }
            set
            {
                if (value.ToString().Equals(""))
                {
                    datumUpisa = DateTime.Now.ToString("dd.MM.yy.");
                }
                else
                {
                    datumUpisa = value;
                }
            }
        }

        public long LogID { get => logID; set => logID = value; }
        public long SifraNovi { get => sifraNovi; set => sifraNovi = value; }
        public long SifraStari { get => sifraStari; set => sifraStari = value; }
        public string NazivNovi { get => nazivNovi; set => nazivNovi = value; }
        public string SNNovi { get => sNNovi; set => sNNovi = value; }
        public string SNStari { get => sNStari; set => sNStari = value; }
        public string CN { get => cN; set => cN = value; }
        public long UserID { get => userID; set => userID = value; }
        public string NazivStari { get => nazivStari; set => nazivStari = value; }
    }
}
