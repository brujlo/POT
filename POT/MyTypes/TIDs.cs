using System;
using System.Collections.Generic;

namespace POT.MyTypes
{
    class TIDs : RadniNalog
    {
        private long ticketID;
        private long tvrtkeID;
        private long prio;
        private String filijala;
        private String cCN;
        private String cID;
        private String datPrijave;
        private String vriPrijave;
        private String datSLA;
        private String vriSLA;
        private long drive;
        private String nazivUredaja;
        private String opisKvara;
        private String prijavio;
        private long userIDPreuzeo;
        private String datPreuzeto;
        private String vriPreuzeto;
        private long userIDDrive;
        private String datDrive;
        private String vriDrive;
        private long userIDPoceo;
        private String datPoceo;
        private String vriPoceo;
        private long userIDZavrsio;
        private String datZavrsio;
        private String vriZavrsio;
        private long userIDUnio;
        private String datReport;
        private String vriReport;
        private String rNID;
        private long userIDSastavio;
        private RadniNalog rn;

        public List<TIDs> allTickets = new List<TIDs>();
        public List<TIDs> workingTickets = new List<TIDs>();
        public List<TIDs> openedTickets = new List<TIDs>();
        public List<TIDs> closedTickets = new List<TIDs>();
        public List<TIDs> stornoTickets = new List<TIDs>();

        public TIDs() { }

        public TIDs(Boolean fillAll) { fillTickets(); }

        public TIDs(long _TicketID, long _TvrtkeID, long _Prio, String _Filijala, String _CCN, String _CID,
            String _DatPrijave, String _VriPrijave, String _DatSLA, String _VriSLA, long _Drive, String _NazivUredaja, String _OpisKvara, String _Prijavio,
            long _UserIDPreuzeo, String _DatPreuzeto, String _VriPreuzeto, long _UserIDDrive, String _DatDrive, String _VriDrive, long _UserIDPoceo,
            String _DatPoceo, String _VriPoceo, long _UserIDZavrsio, String _DatZavrsio, String _VriZavrsio, long _UserIDUnio, String _DatReport, String _VriReport, String _RNID, long _UserIDSastavio)
        {
            try
            {
                TicketID = _TicketID;
                TvrtkeID = _TvrtkeID;
                Prio = _Prio;
                Filijala = _Filijala;
                CCN = _CCN;
                CID = _CID;
                DatPrijave = _DatPrijave;
                VriPrijave = _VriPrijave;
                DatSLA = _DatSLA;
                VriSLA = _VriSLA;
                Drive = _Drive;
                NazivUredaja = _NazivUredaja;
                OpisKvara = _OpisKvara;
                Prijavio = _Prijavio;
                UserIDPreuzeo = _UserIDPreuzeo;
                DatPreuzeto = _DatPreuzeto;
                VriPreuzeto = _VriPreuzeto;
                UserIDDrive = _UserIDDrive;
                DatDrive = _DatDrive;
                VriDrive = _VriDrive;
                UserIDPoceo = _UserIDPoceo;
                DatPoceo = _DatPoceo;
                VriPoceo = _VriPoceo;
                UserIDZavrsio = _UserIDZavrsio;
                DatZavrsio = _DatZavrsio;
                VriZavrsio = _VriZavrsio;
                UserIDUnio = _UserIDUnio;
                DatReport = _DatReport;
                VriReport = _VriReport;
                RNID = _RNID;
                UserIDSastavio = _UserIDSastavio;

                Rn = new RadniNalog(TicketID);
            }
            catch { }
        }

        public void GetTIDByID(double mID)
        {
            QueryCommands qc = new QueryCommands();

            try
            {
                TIDs tmp = qc.getTicketByID(mID);
                /*
                TicketID = tmp.TicketID;
                TvrtkeID = tmp.TvrtkeID;
                Prio = tmp.Prio;
                Filijala = tmp.Filijala;
                CCN = tmp.CCN;
                CID = tmp.CID;
                DatPrijave = tmp.DatPrijave;
                VriPrijave = tmp.VriPrijave;
                DatSLA = tmp.DatSLA;
                VriSLA = tmp.VriSLA;
                Drive = tmp.Drive;
                NazivUredaja = tmp.NazivUredaja;
                OpisKvara = tmp.OpisKvara;
                Prijavio = tmp.Prijavio;
                UserIDPreuzeo = tmp.UserIDPreuzeo;
                DatPreuzeto = tmp.DatPreuzeto;
                VriPreuzeto = tmp.VriPreuzeto;
                UserIDDrive = tmp.UserIDDrive;
                DatDrive = tmp.DatDrive;
                VriDrive = tmp.VriDrive;
                UserIDPoceo = tmp.UserIDPoceo;
                DatPoceo = tmp.DatPoceo;
                VriPoceo = tmp.VriPoceo;
                UserIDZavrsio = tmp.UserIDZavrsio;
                DatZavrsio = tmp.DatZavrsio;
                VriZavrsio = tmp.VriZavrsio;
                UserIDUnio = tmp.UserIDUnio;
                DatReport = tmp.DatReport;
                VriReport = tmp.VriReport;
                RNID = tmp.RNID;
                UserIDSastavio = tmp.UserIDSastavio;

                Rn = new RadniNalog(TicketID);
                */

                tmp.CopyTo(this);

            }
            catch { }
        }

        public void CopyTo(TIDs tmp)
        {
            QueryCommands qc = new QueryCommands();

            try
            {
                tmp.TicketID = TicketID;
                tmp.TvrtkeID = TvrtkeID;
                tmp.Prio = Prio;
                tmp.Filijala = Filijala;
                tmp.CCN = CCN;
                tmp.CID = CID;
                tmp.DatPrijave = DatPrijave;
                tmp.VriPrijave = VriPrijave;
                tmp.DatSLA = DatSLA;
                tmp.VriSLA = VriSLA;
                tmp.Drive = Drive;
                tmp.NazivUredaja = NazivUredaja;
                tmp.OpisKvara = OpisKvara;
                tmp.Prijavio = Prijavio;
                tmp.UserIDPreuzeo = UserIDPreuzeo;
                tmp.DatPreuzeto = DatPreuzeto;
                tmp.VriPreuzeto = VriPreuzeto;
                tmp.UserIDDrive = UserIDDrive;
                tmp.DatDrive = DatDrive;
                tmp.VriDrive = VriDrive;
                tmp.UserIDPoceo = UserIDPoceo;
                tmp.DatPoceo = DatPoceo;
                tmp.VriPoceo = VriPoceo;
                tmp.UserIDZavrsio = UserIDZavrsio;
                tmp.DatZavrsio = DatZavrsio;
                tmp.VriZavrsio = VriZavrsio;
                tmp.UserIDUnio = UserIDUnio;
                tmp.DatReport = DatReport;
                tmp.VriReport = VriReport;
                tmp.RNID = RNID;
                tmp.UserIDSastavio = UserIDSastavio;

                tmp.Rn = Rn;
            }
            catch { }
        }

        private void fillTickets()
        {
            try
            {
                QueryCommands qc = new QueryCommands();

                allTickets = qc.getAllTickets();

                foreach (TIDs tid in allTickets)
                {
                    if(tid.UserIDUnio == 0)
                    {
                        openedTickets.Add(tid);
                    }
                    else if (tid.UserIDUnio != 0)
                    {
                        closedTickets.Add(tid);
                    }

                    if (tid.UserIDUnio == 2)
                    {
                        stornoTickets.Add(tid);
                    }

                    if (tid.UserIDPreuzeo != 0 && tid.UserIDZavrsio == 0 && tid.UserIDUnio != 2 && tid.userIDPoceo != 0)
                    {
                        workingTickets.Add(tid);
                    }
                }
            }
            catch { }
        }


        public long TicketID { get => ticketID; set => ticketID = value; }
        public long TvrtkeID { get => tvrtkeID; set => tvrtkeID = value; }
        public long Prio { get => prio; set => prio = value; }
        public string Filijala { get => filijala; set => filijala = value; }
        public string CCN { get => cCN; set => cCN = value; }
        public string CID { get => cID; set => cID = value; }
        public string DatPrijave { get => datPrijave; set => datPrijave = value; }
        public string VriPrijave { get => vriPrijave; set => vriPrijave = value; }
        public string DatSLA { get => datSLA; set => datSLA = value; }
        public string VriSLA { get => vriSLA; set => vriSLA = value; }
        public long Drive { get => drive; set => drive = value; }
        public string NazivUredaja { get => nazivUredaja; set => nazivUredaja = value; }
        public string OpisKvara { get => opisKvara; set => opisKvara = value; }
        public string Prijavio { get => prijavio; set => prijavio = value; }
        public long UserIDPreuzeo { get => userIDPreuzeo; set => userIDPreuzeo = value; }
        public string DatPreuzeto { get => datPreuzeto; set => datPreuzeto = value; }
        public string VriPreuzeto { get => vriPreuzeto; set => vriPreuzeto = value; }
        public long UserIDDrive { get => userIDDrive; set => userIDDrive = value; }
        public string DatDrive { get => datDrive; set => datDrive = value; }
        public string VriDrive { get => vriDrive; set => vriDrive = value; }
        public long UserIDPoceo { get => userIDPoceo; set => userIDPoceo = value; }
        public string DatPoceo { get => datPoceo; set => datPoceo = value; }
        public string VriPoceo { get => vriPoceo; set => vriPoceo = value; }
        public long UserIDZavrsio { get => userIDZavrsio; set => userIDZavrsio = value; }
        public string DatZavrsio { get => datZavrsio; set => datZavrsio = value; }
        public string VriZavrsio { get => vriZavrsio; set => vriZavrsio = value; }
        public long UserIDUnio { get => userIDUnio; set => userIDUnio = value; }
        public string DatReport { get => datReport; set => datReport = value; }
        public string VriReport { get => vriReport; set => vriReport = value; }
        public string RNID { get => rNID; set => rNID = value; }
        public long UserIDSastavio { get => userIDSastavio; set => userIDSastavio = value; }
        internal RadniNalog Rn { get => rn; set => rn = value; }
    }
}
