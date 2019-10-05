using POT.Documents;
using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POT.WorkingClasses
{
    public partial class ISSSelector : Form
    {
        public static int selectedIndex;

        public ISSSelector()
        {
            InitializeComponent();
        }

        private void ISSSelector_Load(object sender, EventArgs e)
        {
            foreach (Part part in ISS.partList)
            {
                PartSelectorCB.Items.Add(part.CodePartFull + " # " + part.SN + " # " + part.CN);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedIndex = -1;
            selectedIndex = PartSelectorCB.SelectedIndex;

            this.Close();
        }

        private void ISSSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(PartSelectorCB.SelectedIndex < 0)
                selectedIndex = -1;
        }
    }
}
