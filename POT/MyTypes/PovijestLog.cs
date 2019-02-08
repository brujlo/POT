using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.MyTypes
{
    class PovijestLog
    {
        private String  datumUpisa = DateTime.Now.ToString("dd.MM.yy.");
        private String  datumRada;
        private long    sifraNovi;
        private String  nazivNovi;
        private String  opis;
        private String  sNNovi;
        private String  sNStari;
        private String  cN;
        private long    userID;
        private String  customerName;
        private String  cCN;
        private String  cI;
        private String  dokument;
        private long    sifraStari;
        private string  nazivStari;
        private string  gNg;

        public Boolean SaveToPovijestLog(List<Part> mNewOldPart, String mDatumRada, String mOpis, String mCustomerName, String mCCN, String mCI, String mDokument, String mGNG)
        {
            Boolean saved = false;

            SqlCommand command;
            ConnectionHelper cn = new ConnectionHelper();
            QueryCommands qc = new QueryCommands();
            using (SqlConnection cnn = cn.Connect(WorkingUser.Username, WorkingUser.Password))
            {
                datumRada = mDatumRada;

                try
                {
                    if (mNewOldPart[0].CodePartFull == 0)
                    {
                        sifraNovi = long.Parse(string.Format("{0:00}", mNewOldPart[0].CompanyO) + string.Format("{0:00}", mNewOldPart[0].CompanyC) + string.Format("{0:000000000}", mNewOldPart[0].PartialCode));
                    }
                    else
                    {
                        sifraNovi = mNewOldPart[0].CodePartFull;
                    }

                }
                catch
                {
                    sifraNovi = 0;
                }

                if (mNewOldPart[0].PartialCode != 0)
                    nazivNovi = qc.PartInfoByFullCodeSifrarnik(WorkingUser.Username, WorkingUser.Password, mNewOldPart[0].PartialCode).FullName;
                else
                    nazivNovi = "";
                opis = mOpis;
                sNNovi = mNewOldPart[0].SN;
                cN = mNewOldPart[0].CN;
                userID = WorkingUser.UserID;
                customerName = mCustomerName;
                cCN = mCCN;
                cI = mCI;
                dokument = mDokument;
                
                if(mNewOldPart.Count > 1)
                {
                    if (mNewOldPart[1].PartialCode != 0)
                        nazivStari = qc.PartInfoByFullCodeSifrarnik(WorkingUser.Username, WorkingUser.Password, mNewOldPart[1].PartialCode).FullName;
                    else
                        nazivStari = "";
                    try
                    {
                        if (mNewOldPart[1].CodePartFull == 0)
                        {
                            sifraStari = long.Parse(string.Format("{0:00}", mNewOldPart[1].CompanyO) + string.Format("{0:00}", mNewOldPart[1].CompanyC) + string.Format("{0:000000000}", mNewOldPart[1].PartialCode));
                        }
                        else
                        {
                            sifraStari = mNewOldPart[1].CodePartFull;
                        }

                    }
                    catch
                    {
                        sifraStari = 0;
                    }
                    sNStari = mNewOldPart[1].SN;
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
                    partCount = sifraNovi == 0 ? partCount / 2 : partCount;
                    for (int i = 0; i < partCount; i++)
                    {
                        command.CommandText = "insert into PovijestLog " +
                            "(DatumUpisa, DatumRada, SifraNovi, NazivNovi, Opis, SNNovi, SNStari, CN, UserID, Customer, CCN, CI, " +
                            "Dokument, SifraStari, NazivStari, gNg)" +
                            " values ('" + datumUpisa + "', '" + DatumRada + "', " + sifraNovi + ", '" + nazivNovi +
                            "', '" + Opis + "', '" + sNNovi + "', '" + sNStari + "', '" + cN + "', " + userID +
                            ", '" + CustomerName + "', '" + CCN + "', '" + CI + "', '" + Dokument + "', " + sifraStari +
                            ", '" + nazivStari + "', '" + GNg + "')";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    saved = true;
                }
                catch (Exception)
                {
                    try
                    {
                        transaction.Rollback();
                        saved = false;
                        cnn.Close();
                        throw;
                    }
                    catch (Exception)
                    {
                        saved = false;
                        cnn.Close();
                        //throw;
                    }
                }
                cnn.Close();
                return saved;
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
                if(value.ToString().Length > 400)
                {
                    opis = value.Substring(0,400).Trim();
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
    }
}
