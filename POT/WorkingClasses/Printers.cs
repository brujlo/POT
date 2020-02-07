using System;
using System.Collections.Generic;

namespace POT.WorkingClasses
{
    class Printers
    {
        private List<String> prtLst = new List<String>();

        public Printers()
        {
            foreach (string prtName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                prtLst.Add(prtName);
            }
        }

        public Boolean PrinterExist(String pName)
        {
            return prtLst.Exists(name => name == pName);
        }

        
    }
}
