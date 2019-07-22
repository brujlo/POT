using System;
using System.Collections.Generic;

namespace POT.MyTypes
{
    class OfferParts : Offer
    {
        private long ponudaID;
        private long partCode;
        private String vrijemeRada;
        private decimal rabat;
        private String iznosPart;
        private String iznosRabat;
        private String iznosTotal;
        private int kolicina;

        public OfferParts() { }

        public OfferParts(long ponudaID, long partCode, string vrijemeRada, decimal rabat, int kolicina)
        {
            this.ponudaID = ponudaID;
            this.partCode = partCode;
            this.vrijemeRada = vrijemeRada;
            this.rabat = rabat;
            this.kolicina = kolicina;

            //decimal koeficijent = (decimal)100 / (decimal)60;
        }

        public void AddOfferToPart(Offer off)
        {
            this.Id = off.Id;
            this.RacunID = off.RacunID;
            this.DatumIzdano = off.DatumIzdano;
            this.Iznos = off.Iznos;
            this.DatumNaplaceno = off.DatumNaplaceno;
            this.Naplaceno = off.Naplaceno;
            this.CustomerID = off.CustomerID;
            this.Eur = off.Eur;
            this.Napomena = off.Napomena;
            this.VrijemeIzdano = off.VrijemeIzdano;
            this.Valuta = off.Valuta;
            this.Operater = off.Operater;
            this.DanTecaja = off.DanTecaja;
            this.NacinPlacanja = off.NacinPlacanja;
            this.Storno = off.Storno;
        }

        public int Compare(List<OfferParts> lst, OfferParts prt)
        {
            int i = -1;
            foreach (OfferParts prtL in lst)
            {
                i++;
                if (prt.partCode == prtL.partCode && prt.vrijemeRada == prtL.vrijemeRada && prt.rabat == prtL.rabat)
                    return i;
            }
            return -1;
        }

        public decimal workTimeToNumber(String wrk)
        {
            decimal rez = 1;

            if (!wrk.Equals("00:00"))
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

        public long PonudaID { get => ponudaID; set => ponudaID = value; }
        public long PartCode { get => partCode; set => partCode = value; }
        public string VrijemeRada { get => vrijemeRada; set => vrijemeRada = value; }
        public decimal Rabat { get => rabat; set => rabat = value; }
        public String IznosPart { get => iznosPart; set => iznosPart = value; }
        public String IznosRabat { get => iznosRabat; set => iznosRabat = value; }
        public String IznosTotal { get => iznosTotal; set => iznosTotal = value; }
        public int Kolicina { get => kolicina; set => kolicina = value; }
    }
}
