using POT.MyTypes;
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

namespace POT.WorkingClasses
{
    public partial class Narudzbe : Form
    {
        List<String> resultArr = new List<string>();

        List<Company> resultArrC = new List<Company>();
        List<String> resultArrSearchCode = new List<string>();

        static List<String> sifrarnikArr = new List<string>();

        private string code = "";

        Branch br = new Branch();
        List<Branch> brList = new List<Branch>();
        String tvrtCode;

        public Narudzbe()
        {
            InitializeComponent();
        }

        private void Narudzbe_Load(object sender, EventArgs e)
        {

            dateTimePicker1.Value = DateTime.Today.AddDays(+20);

            comboBox3.Text = Properties.Settings.Default.MainCompanyCode;
            comboBox4.Text = Properties.Settings.Default.MainCompanyCode;
            //this.printPrewBT.Enabled = false;

            Thread myThread = new Thread(fillComboBoxes);

            myThread.Start();

            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();

            try
            {
                Company cmpList = new Company();
                //List<Company> resultArrC = new List<Company>();

                resultArrC = cmpList.GetAllCompanyInfoSortByName();

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.cbTvrtka.Items.Add(resultArrC[i].Name);
                    }

                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }

            resultArr.Clear();
            try
            {
                resultArr = qc.AllCompanyInfoSortCode(WorkingUser.Username, WorkingUser.Password);

                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 10; i < resultArr.Count(); i = i + 14)
                    {
                        comboBox4.Items.Add(resultArr[i]);
                        comboBox3.Items.Add(resultArr[i]);
                    }

                }

                resultArr.Clear();
                resultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);

                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 2)
                    {
                        cbPart.Items.Add(resultArr[i]);
                        //resultArrSearchName.Add(resultArr[i]);
                        resultArrSearchCode.Add(resultArr[i + 1]);
                    }

                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);

                Program.LoadStop();
                this.Focus();
            }

            textBox5.Text = resultArrC.ElementAt(int.Parse(comboBox4.Text) - 1).Name.Trim();
            textBox6.Text = resultArrC.ElementAt(int.Parse(comboBox3.Text) - 1).Name.Trim();

            Program.LoadStop();
            this.Focus();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Text = resultArrC.ElementAt(comboBox4.SelectedIndex).Name.Trim();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = resultArrC.ElementAt(comboBox3.SelectedIndex).Name.Trim();
        }

        static void fillComboBoxes()
        {
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            List<String> tsendArr = new List<string>();
            List<String> tresultArr = new List<string>();

            try
            {
                tresultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);
                tsendArr.Clear();

                if (!tresultArr[0].Equals("nok"))
                {
                    sifrarnikArr = tresultArr;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }

        private void cbTvrtka_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.SelectedIndex = cbTvrtka.SelectedIndex;
            //comboBox3.SelectedIndex = comboBox3.FindStringExact(resultArrC[cbTvrtka.SelectedIndex].Code);


            comboBox5.Items.Clear();
            comboBox5.ResetText();

            label8.Text = resultArrC.ElementAt(cbTvrtka.SelectedIndex).Name.Trim();
            label7.Text = resultArrC.ElementAt(cbTvrtka.SelectedIndex).Address.Trim();
            label6.Text = resultArrC.ElementAt(cbTvrtka.SelectedIndex).OIB.Trim();
            label5.Text = resultArrC.ElementAt(cbTvrtka.SelectedIndex).Contact.Trim();

            int index = cbTvrtka.SelectedIndex;
            if (index >= 0)
            {
                brList = br.GetAllFilByTvrtkeCode(resultArrC.ElementAt(index).Code);
                tvrtCode = resultArrC.ElementAt(index).Code;

                for (int i = 0; i < brList.Count(); i++)
                {
                    comboBox5.Items.Add(brList[i].FilNumber);
                }
            }
        }

        private void cbPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                code = comboBox4.Text + comboBox3.Text + string.Format("{0:000000000}", int.Parse(resultArrSearchCode.ElementAt(cbPart.SelectedIndex)));
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox5.SelectedIndex;
            br = brList[index];
            textBox7.Text = br.City + ", " + br.Address;
        }
    }
}
