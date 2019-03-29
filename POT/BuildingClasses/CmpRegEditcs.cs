using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POT.BuildingClasses
{
    public partial class CmpRegEditcs : Form
    {
        List<Company> resultArrC = new List<Company>();

        static List<String> codeArr = new List<string>();
        static List<String> regionArr = new List<string>();
        static List<String> regionFullNameArr = new List<string>();

        public CmpRegEditcs()
        {
            InitializeComponent();
        }

        private void CmpRegEditcs_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();

            try
            {
                Company cmpGetList = new Company();
                resultArrC = cmpGetList.GetAllCompanyInfoSortByName();

                button1.Enabled = false;

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.comboBox1.Items.Add(resultArrC[i].Name);
                        codeArr.Add(int.Parse(resultArrC[i].Code).ToString());
                    }

                }

                List<String> testresultArr1 = new List<string>();

                testresultArr1 = qc.GetAllRegions(WorkingUser.Username, WorkingUser.Password);

                if (!testresultArr1[0].Equals("nok"))
                {
                    for (int i = 6; i < testresultArr1.Count(); i = i + 3)
                    {
                        comboBox3.Items.Add(testresultArr1[i].ToString());
                        regionArr.Add(testresultArr1[i].ToString());
                        regionFullNameArr.Add(testresultArr1[i + 2].ToString());
                    }   
                }


                if (!resultArrC[0].Equals("nok"))
                {
                    for (int i = 1; i < 100; i++)
                    {
                        if(!codeArr.Contains(i.ToString()))
                            comboBox2.Items.Add(i);
                    }
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;

            if (index >= 0)
            {
                button1.Enabled = true;
                textBox2.Text = resultArrC.ElementAt(index).Address;
                textBox3.Text = resultArrC.ElementAt(index).City;
                textBox4.Text = resultArrC.ElementAt(index).PB;
                textBox5.Text = resultArrC.ElementAt(index).OIB;
                textBox6.Text = resultArrC.ElementAt(index).Contact;
                textBox7.Text = resultArrC.ElementAt(index).BIC;
                textBox8.Text = resultArrC.ElementAt(index).KN.ToString();
                textBox9.Text = resultArrC.ElementAt(index).EUR.ToString();
                comboBox2.Text = resultArrC.ElementAt(index).Code;
                textBox11.Text = resultArrC.ElementAt(index).Country;
                comboBox3.Text = resultArrC.ElementAt(index).RegionID.ToString();
                textBox13.Text = resultArrC.ElementAt(index).Email;
                textBox14.Text = resultArrC.ElementAt(index).ID.ToString();
            }
            else
            {
                button1.Enabled = false;
                textBox2.ResetText();
                textBox3.ResetText();
                textBox4.ResetText();
                textBox5.ResetText();
                textBox6.ResetText();
                textBox7.ResetText();
                textBox8.ResetText();
                textBox9.ResetText();
                comboBox2.ResetText();
                textBox11.ResetText();
                comboBox3.ResetText();
                textBox13.ResetText();
                textBox14.ResetText();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1_Leave(sender, e);
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            comboBox1_Leave(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
