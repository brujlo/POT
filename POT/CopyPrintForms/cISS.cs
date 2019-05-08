using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POT.CopyPrintForms
{
    public partial class cISS : Form
    {
        public cISS()
        {
            InitializeComponent();
        }

        private void cISS_Load(object sender, EventArgs e)
        {
            /*
            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("otpID");
            listView1.Columns.Add("customerID");
            listView1.Columns.Add("dateCreated");
            listView1.Columns.Add("napomena");
            listView1.Columns.Add("primID");
            listView1.Columns.Add("userID");
            listView1.Columns.Add("fillID");

            listView2.View = View.Details;

            listView2.Columns.Add("RB");
            listView2.Columns.Add("Name");
            listView2.Columns.Add("CodePartFull");
            listView2.Columns.Add("SN");
            listView2.Columns.Add("CN");

            //Thread myThread = new Thread(fillSifrarnik);
            //myThread.Start();

            otpID = qc.GetAllOTPID();
            if (otpID[0] != -1)
            {
                for (int i = 0; i < otpID.Count(); i++)
                {
                    this.comboBox1.Items.Add(otpID[i]);
                }
            }

            customerID = qc.GetAllOTPcustomerID();
            if (customerID[0] != -1)
            {
                for (int i = 0; i < customerID.Count(); i++)
                {
                    this.comboBox2.Items.Add(customerID[i]);
                }

            }

            dateCreated = qc.GetAllOTPdateCreated();
            if (!dateCreated[0].Equals("nok"))
            {
                for (int i = 0; i < dateCreated.Count(); i++)
                {
                    this.comboBox3.Items.Add(dateCreated[i]);
                }

            }

            Company temCmp = new Company();
            cmpList = temCmp.GetAllCompanyInfoSortByName();

            if (cmpList.Count != 0)
            {
                for (int i = 0; i < temCmp.Count; i++)
                {
                    this.comboBox4.Items.Add(cmpList[i].Name);
                }

            }*/
        }
    }
}
