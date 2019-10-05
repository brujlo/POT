using POT.MyTypes;
using System;
using System.Collections.Generic;

namespace POT.WorkingClasses
{
    class ISSreport
    {
        private long issID;
        private String date;
        private long userIDmaked;
        private long customerID;
        private long partID;
        private long closed;
        private String totalTIme;

        private Part mainPart;

        List<ISSparts> listIssParts = new List<ISSparts>();

        public long ISSid { get => issID; set => issID = value; }
        public string Date { get => date; set => date = value; }
        public long UserIDmaked { get => userIDmaked; set => userIDmaked = value; }
        public long CustomerID { get => customerID; set => customerID = value; }
        public long PartID { get => partID; set => partID = value; }
        public long Closed { get => closed; set => closed = value; }
        public string TotalTIme { get => totalTIme; set => totalTIme = value; }
        public Part MainPart { get => mainPart; set => mainPart = value; }
        internal List<ISSparts> ListIssParts { get => listIssParts; set => listIssParts = value; }

        public List<ISSreport> GetAllIIS()
        {
            QueryCommands qc = new QueryCommands();

            List<ISSreport> tmparr = qc.GetAllISSInfoOpenClose(0);

            foreach(ISSreport iss in tmparr)
            {
                iss.ListIssParts = qc.GetAllISSPartsByISSid(iss.ISSid);
                iss.MainPart = qc.SearchPartsInAllTablesBYPartID(iss.partID)[0];

            }

            return tmparr;
        }

        //long h = 0;
        //long m = 0;
        //long s = 0;

        //int rb = 0;

        //private long ISSid;

        //DateTime startDate = DateTime.Now;

        //QueryCommands qc = new QueryCommands();
        //List<Part> partList = new List<Part>();

        //List<String> sifrarnikArr = new List<String>();
        //List<String> workDone = new List<String>();

        //List<List<Part>> groupedGoodPartsCode = new List<List<Part>>();
        //List<ISSparts> listIssParts = new List<ISSparts>();

        //static List<ISSparts> listIssBckpParts = new List<ISSparts>();
        //static Boolean isBckpFilled = false;

        //List<long> ISSids = new List<long>();
        //List<Part> PartsInService = new List<Part>();
        //Part newSendPart = new Part();
        //List<Part> newSendPartList = new List<Part>();
        //Part mainPart = new Part();

        //Company cmpCust = new Company();
        //Company cmpM = new Company();
        //MainCmp mm = new MainCmp();

        //int uvecajGroupPart = 0;

    }
}
