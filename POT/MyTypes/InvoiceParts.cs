using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.MyTypes
{
    class InvoiceParts : Invoice
    {
        private long racunID;
        private long partCode;
        private String vrijemeRada;
        private decimal rabat;
        private int kolicina;

        public InvoiceParts(){}

        public InvoiceParts(long racunID, long partCode, string vrijemeRada, decimal rabat, int kolicina)
        {
            this.racunID = racunID;
            this.partCode = partCode;
            this.vrijemeRada = vrijemeRada;
            this.rabat = rabat;
            this.kolicina = kolicina;

            //decimal koeficijent = (decimal)100 / (decimal)60;
        }

        public decimal workTimeToNumber(String wrk)
        {
            decimal rez = 1;

            if ( !wrk.Equals("00:00") )
            {
                try
                {
                    decimal obrJed = Properties.Settings.Default.ObracunskaJedinica;

                    var spl = wrk.Split(':');
                    int sati = int.Parse(spl[0]);
                    int min = int.Parse(spl[1]);
                    decimal mm = (int)(min / obrJed);

                    if (min % obrJed > 0)
                        mm++;
                    mm = mm * obrJed / 60;

                    rez = sati + (mm);
                }
                catch
                {
                    return 1;
                }
            }

            return rez;
        }

        public decimal PartTotalPrice(decimal price, decimal popust, String wrk)
        {
            decimal rez = 0;

            try
            {
                decimal vrijemeIndex = workTimeToNumber(wrk);
                //vrijemeIndex -= (int)vrijemeIndex;

                rez = vrijemeIndex * price - (vrijemeIndex * price * (popust / 100));
            }
            catch
            {
                return 0;
            }

            return rez;
        }

        public long RacunID { get => racunID; set => racunID = value; }
        public long PartCode { get => partCode; set => partCode = value; }
        public string VrijemeRada { get => vrijemeRada; set => vrijemeRada = value; }
        public decimal Rabat { get => rabat; set => rabat = value; }
        public int Kolicina { get => kolicina; set => kolicina = value; }
    }
}
