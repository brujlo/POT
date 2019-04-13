using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT
{
    public partial class Selector : Form
    {
        private long OTPNumber;

        public Selector()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem.ToString().Equals(""))
                    OTPNumber = 0;
                else
                    OTPNumber = long.Parse(comboBox1.SelectedItem.ToString());
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                OTPNumber = 0;
            }
            finally
            {
                this.Close();
            }
        }

        public string SetLabelText
        {
            get
            {
                return this.label1.Text;
            }
            set
            {
                this.label1.Text = value;
            }
        }

        public List<String> SetComboBoxStringList
        {
            get
            {
                List<String> backArr = new List<string>();
                if(comboBox1.Items.Count > 0)
                {
                    for(int i = 0; i < comboBox1.Items.Count; i++)
                    {
                        backArr.Add(comboBox1.Items[i].ToString());
                    }
                }
                return backArr;
            }

            set
            {
                if(value.Count > 0)
                {
                    for(int i = 0; i < value.Count; i++)
                    {
                        comboBox1.Items.Add(value[i]);
                    }
                }
            }
        }

        public long GetOTPValue
        {
            get
            {
                return OTPNumber;
            }
        }
    }
}
