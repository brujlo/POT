using POT.Properties;
using System;
using System.Drawing;
using System.Resources;


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
            catch (Exception e1)
            {
                new LogWriter(e1);
                img = (Image)rm.GetObject("DefaultLogoPOT");
            }

            return img;
        }
    }
}
