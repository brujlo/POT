using POT.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace POT.WorkingClasses
{
    class CLogo
    {
        Image img = null;
        ResourceManager rm = Resources.ResourceManager;

        public Image GetImage()
        {
            try
            {
                using (ResXResourceSet resxLoad = new ResXResourceSet(@".\Logo.resx"))
                {
                    img = (Image)resxLoad.GetObject("LogoPicture", true);

                    if(img == null)
                        img = (Image)rm.GetObject("DefaultLogoPOT");
                }
            }
            catch (Exception)
            {
                img = (Image)rm.GetObject("DefaultLogoPOT");
            }

            return img;
        }
    }
}
