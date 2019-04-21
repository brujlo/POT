using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT
{
    public partial class MainCmpSelector : Form
    {
        MainCmp cmp = new MainCmp();
        List<MainCmp> resultList = new List<MainCmp>();

        public MainCmpSelector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Main cmp select";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            int index = comboBox1.SelectedIndex;

            Properties.Settings.Default.CmpName = resultList[index].Name;
            Properties.Settings.Default.CmpAddress = resultList[index].Address;
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

            Result = "Main cmp selected, please relog to see changes.";
            lw.LogMe(function, usedQC, data, Result);
            MessageBox.Show(Result);

            Close();
        }

        private void MainCmpSelector_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            resultList = cmp.GetAllMainCmpInfoSortCode();

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
