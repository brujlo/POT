using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.MyTypes
{
    class RadniNalog
    {
        private long ticketIDRN;
        private String errorCode;
        private String descriptionRN;
        private long rB;
        private long partIDNew;
        private long partIDOld;

        private List<Part> partListNew = new List<Part>();
        private List<Part> partListOld = new List<Part>();

        private List<long> idListNew = new List<long>();
        private List<long> idListOld = new List<long>();

        QueryCommands qc = new QueryCommands();

        public RadniNalog() { }

        public RadniNalog(long mTidID)
        {
            List<String> tmpList = qc.GetAllInfoByTidIDFromRN(mTidID);

            if (!tmpList[0].Equals("nok"))
            {
                TicketIDRN = long.Parse(tmpList[0]);
                ErrorCode = tmpList[1];
                DescriptionRN = tmpList[2];
                RB = long.Parse(tmpList[3]);
            }
            else
            {
                TicketIDRN = 0;
                ErrorCode = "";
                DescriptionRN = "";
                RB = 0;
            }


            idListNew = qc.GetAllNewPartsIdByTidIDFromRN(mTidID);
            idListOld = qc.GetAllOldPartsIdByTidIDFromRN(mTidID);

            Part pr = new Part();
            foreach(long id in idListNew)
            {
                PartListNew.Add(pr.GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(id));
                pr = new Part();
            }

            foreach (long id in idListOld)
            {
                PartListOld.Add(pr.GetPartFromPartsPartsPoslanoPartsZamijenjenoByID(id));
                pr = new Part();

            }

        }

        public long TicketIDRN { get => ticketIDRN; set => ticketIDRN = value; }
        public string ErrorCode { get => errorCode; set => errorCode = value; }
        public string DescriptionRN { get => descriptionRN; set => descriptionRN = value; }
        public long RB { get => rB; set => rB = value; }
        public long PartIDNew { get => partIDNew; set => partIDNew = value; }
        public long PartIDOld { get => partIDOld; set => partIDOld = value; }
        public List<Part> PartListNew { get => partListNew; set => partListNew = value; }
        public List<Part> PartListOld { get => partListOld; set => partListOld = value; }
    }
}
