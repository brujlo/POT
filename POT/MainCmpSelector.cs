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

namespace POT
{
    public partial class MainCmpSelector : Form
    {
        Company cmp = new Company();
        List<Company> resultList = new List<Company>();

        public MainCmpSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;

            Properties.Settings.Default.CmpName = resultList[index].Name;
            Properties.Settings.Default.CmpAddress = resultList[index].Address + ", " + resultList[index].Country + " - " + resultList[index].PB + " " + resultList[index].City;
            Properties.Settings.Default.CmpVAT = resultList[index].OIB;
            if(Properties.Settings.Default.CmpWWW == "")
                Properties.Settings.Default.CmpWWW = "www";
            if(Properties.Settings.Default.CmpPhone == "")
                Properties.Settings.Default.CmpPhone = "Phone";
            Properties.Settings.Default.CmpEmail = resultList[index].Email;

            Properties.Settings.Default.MainCompanyCode = resultList[index].Code;
            Properties.Settings.Default.CmpName = resultList[index].Name;
            Properties.Settings.Default.Remember = true;
            Properties.Settings.Default.Save();
            Close();
        }

        private void MainCmpSelector_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            resultList = cmp.GetAllCompanyInfoSortCode();

            if (resultList.Count != 0)
            {
                for(int i = 0; i < resultList.Count; i++)
                {
                    comboBox1.Items.Add(resultList[i].Name);
                }
            }
        }
    }
}
