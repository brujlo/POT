using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.MyTypes
{
    class Narudzba
    {
        long id;
        long userId;
        string dateMaked;
        int otpID;
        int customerID;
        int branchID;
        string brNarudzbe;
        string datePlaced;
        List<PrtOrderParts> parts = new List<PrtOrderParts>();
        string dateDelivery;
        string dateDelivered;

        public long Id { get => id; set => id = value; }
        public long UserId { get => userId; set => userId = value; }
        public string DateMaked { get => dateMaked; set => dateMaked = value; }

        public int OtpID
        {
            get { return otpID; }
            set { otpID = value; }
        }

        //{ get => otpID; set => otpID = value; }
        public int CustomerID { get => customerID; set => customerID = value; }
        public int BranchID { get => branchID; set => branchID = value; }
        public string BrNarudzbe { get => brNarudzbe; set => brNarudzbe = value; }
        public string DatePlaced { get => datePlaced; set => datePlaced = value; }
        public string DateDelivery { get => dateDelivery; set => dateDelivery = value; }
        public string DateDelivered { get => dateDelivered; set => dateDelivered = value; }
        public List<PrtOrderParts> Parts { get => parts; set => parts = value; }

        public void addPart(PrtOrderParts pt)
        {
            if (Parts.Exists(x => x.Pt.FullCode == pt.Pt.FullCode))
            {
                int indeks = Parts.FindIndex(x => x.Pt.FullCode == pt.Pt.FullCode);

                Parts[indeks].Kol += pt.Kol;
            }
            else
            {
                Parts.Add(pt);
            }
        }

        public PrtOrderParts getPart(Part pt)
        {
            if (Parts.Exists(x => x.Pt.FullCode == pt.PartialCode))
                return Parts.Find(x => x.Pt.FullCode == pt.PartialCode);
            else
                return null;
        }

        public Narudzba() { }

        public Narudzba(
            long p_id, 
            long p_userId,
            string p_dateMaked,
            int? p_otpID,
            int p_customerID,
            int? p_branchID,
            string p_brNarudzbe,
            string p_datePlaced,
            List<PrtOrderParts> p_parts,
            string p_dateDelivery = "",
            string p_dateDelivered = "",
            Boolean save = false)
        {
            Id = p_id;
            UserId = p_userId;
            DateMaked = p_dateMaked;
            OtpID = p_otpID ?? 0;
            CustomerID = p_customerID;
            BranchID = p_branchID ?? 0;
            BrNarudzbe = p_brNarudzbe;
            DatePlaced = p_datePlaced;
            Parts = p_parts;
            DateDelivery = p_dateDelivery;
            DateDelivered = p_dateDelivered;

            if (save)
                SaveNarudzba();
        }

        public Boolean SaveNarudzba()
        {


            return true;
        }
    }
}
