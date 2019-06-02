using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.MyTypes
{
    class Invoice
    {
        private QueryCommands qc = new QueryCommands();

        private long id;
        private long ponudaID;
        private String datumIzdano;
        private decimal iznos;
        private String datumNaplaceno;
        private decimal naplaceno;
        private long customerID;
        private decimal eur;
        private String napomena;
        private String vrijemeIzdano;
        private int valuta;
        private String operater;
        private String danTecaja;
        private String nacinPlacanja;
        private int storno;

        public Invoice(){}

        public Invoice(long id, long ponudaID, string datumIzdano, decimal iznos, string datumNaplaceno, decimal naplaceno, long customerID, decimal eur, string napomena, string vrijemeIzdano, int valuta, string operater, string danTecaja, string nacinPlacanja, int storno)
        {
            this.Id = id;
            this.PonudaID = ponudaID;
            this.DatumIzdano = datumIzdano;
            this.Iznos = iznos;
            this.DatumNaplaceno = datumNaplaceno;
            this.Naplaceno = naplaceno;
            this.CustomerID = customerID;
            this.Eur = eur;
            this.Napomena = napomena;
            this.VrijemeIzdano = vrijemeIzdano;
            this.Valuta = valuta;
            this.Operater = operater;
            this.DanTecaja = danTecaja;
            this.NacinPlacanja = nacinPlacanja;
            this.Storno = storno;
        }

        public long GetNewInvoiceNumber()
        {
            long retValue = 0;

            String region = String.Format("{0:000}", WorkingUser.RegionID);
            String user = String.Format("{0:000}", WorkingUser.UserID);
            String invNbr = String.Format("{0:000}", qc.GetNewInvoiceID());
            String year = String.Format("{0:00}", DateTime.Now.ToString("yy"));

            try
            {
                retValue = long.Parse(region + user + invNbr + year);
                Id = retValue;
            }
            catch
            {
                retValue = 0;
            }

            return retValue;
        }

        public String IDLongtoString(long value)
        {
            String retValue = "000-000-00000";

            try
            {
                String temp = value.ToString();

                if (temp.Length < 10)
                    temp = "00" + value;
                else if (temp.Length < 11)
                    temp = "0" + value;

                String region = temp.Substring(0,3);
                String user = temp.Substring(3, 3);
                String invNbr = temp.Substring(6, 3);
                String year = temp.Substring(9, 2);

                retValue = region + "-" + user + "-" + invNbr + year;
            }
            catch
            {
                retValue = "000-000-00000";
            }

            return retValue;
        }

        public long Id { get => id; set => id = value; }
        public long PonudaID { get => ponudaID; set => ponudaID = value; }
        public string DatumIzdano { get => datumIzdano; set => datumIzdano = value; }
        public decimal Iznos { get => iznos; set => iznos = value; }
        public string DatumNaplaceno { get => datumNaplaceno; set => datumNaplaceno = value; }
        public decimal Naplaceno { get => naplaceno; set => naplaceno = value; }
        public long CustomerID { get => customerID; set => customerID = value; }
        public decimal Eur { get => eur; set => eur = value; }
        public string Napomena { get => napomena; set => napomena = value; }
        public string VrijemeIzdano { get => vrijemeIzdano; set => vrijemeIzdano = value; }
        public int Valuta { get => valuta; set => valuta = value; }
        public string Operater { get => operater; set => operater = value; }
        public string DanTecaja { get => danTecaja; set => danTecaja = value; }
        public string NacinPlacanja { get => nacinPlacanja; set => nacinPlacanja = value; }
        public int Storno { get => storno; set => storno = value; }
    }
}
