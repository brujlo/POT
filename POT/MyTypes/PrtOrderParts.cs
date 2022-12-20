using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.MyTypes
{
    public class PrtOrderParts
    {
        PartSifrarnik pt;
        int kol;

        public PartSifrarnik Pt { get => pt; set => pt = value; }
        public int Kol { get => kol; set => kol = value; }

        public PrtOrderParts() { }

        public PrtOrderParts(PartSifrarnik pt, int kol)
        {
            this.pt = pt;
            this.kol = kol;
        }
    }
}
