using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace POT.BuildingClasses
{
    public partial class CmpRegEditcs : Form
    {
        List<Company> resultArrC = new List<Company>();

        static List<String> codeArr = new List<string>();
        static List<String> regionArr = new List<string>();
        static List<String> regionFullNameArr = new List<string>();

        String Address;
        String City;
        String Phone;
        String Zip;
        String Filijala;
        String Country;
        String tvrtCode;
        long RegionID;

        Branch br = new Branch();
        List<Branch> brList = new List<Branch>();

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
                        this.comboBox4.Items.Add(resultArrC[i].Name);
                        codeArr.Add(int.Parse(resultArrC[i].Code).ToString());
                    }

                }

                List<String> testresultArr1 = new List<string>();

                testresultArr1 = qc.GetAllRegions();

                if (!testresultArr1[0].Equals("nok"))
                {
                    for (int i = 6; i < testresultArr1.Count(); i = i + 3)
                    {
                        comboBox3.Items.Add(testresultArr1[i].ToString());
                        comboBox5.Items.Add(testresultArr1[i].ToString());
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

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4_Leave(sender, e);
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            tvrtCode = "";
            comboBox4_Leave(sender, e);
        }

        private void comboBox4_Leave(object sender, EventArgs e)
        {
            comboBox6.Items.Clear();
            comboBox6.ResetText();
            comboBox5.ResetText();
            this.textBox12.Text = "";
            this.textBox15.Text = "";
            this.textBox16.Text = "";
            this.textBox17.Text = "";
            this.textBox10.Text = "";

            int index = comboBox4.SelectedIndex;
            if(index >= 0)
            {
                brList = br.GetAllFilByTvrtkeCode(resultArrC.ElementAt(index).Code);
                tvrtCode = resultArrC.ElementAt(index).Code;

                for (int i = 0; i < brList.Count(); i++)
                {
                    comboBox6.Items.Add(brList[i].FilNumber);
                }
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox6_Leave(sender, e);
        }

        private void comboBox6_Leave(object sender, EventArgs e)
        {
            String brFIl;

            brFIl = this.comboBox6.Text;
            if (this.comboBox6.Text.Length > 0 && this.comboBox6.Text.Length < 4)
            {
                if (this.comboBox6.Text.Length < 2) brFIl = "00" + this.comboBox6.Text;
                else if (this.comboBox6.Text.Length < 3) brFIl = "0" + this.comboBox6.Text;
            }

            this.comboBox6.Text = brFIl;

            int i = 0;
            for (; i < brList.Count(); i++)
            {
                if (brList[i].FilNumber.Contains(brFIl))
                {
                    break;
                }
            }

            if (i < brList.Count())
            {
                this.comboBox5.Text = brList[i].RegionID.ToString();
                RegionID = brList[i].RegionID;
                this.textBox12.Text = brList[i].Address;
                this.textBox15.Text = brList[i].Phone;
                this.textBox16.Text = brList[i].City;
                this.textBox17.Text = brList[i].Pb;
                this.textBox10.Text = brList[i].Country;
            }
            else
            {
                this.comboBox5.Text = "";
                this.textBox12.Text = "";
                this.textBox15.Text = "";
                this.textBox16.Text = "";
                this.textBox17.Text = "";
                this.textBox10.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "qc.AddFilToDB";
            String data = tvrtCode + ", " + Filijala + ", " + RegionID + ", " + Address + ", " + City + ", " + Zip + ", " + Phone + ", " + Country;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            int index;
            index = comboBox5.FindStringExact(comboBox5.Text);

            if (index < 0)
                return;

            try
            {
                if (comboBox4.Text.Equals("") || comboBox5.Text.Equals("") || comboBox6.Text.Equals("")
                    || textBox12.Text.Equals("") || textBox15.Text.Equals("") || textBox16.Text.Equals("") || textBox17.Text.Equals("") || textBox10.Text.Equals(""))
                {
                    Result = "All fields fileds must be filled in.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }
                zamjeniZabranjeneZnakove(textBox12.Text, textBox16.Text, textBox17.Text, textBox15.Text, textBox10.Text, comboBox6.Text, comboBox5.Text);

                QueryCommands qc = new QueryCommands();
                if (qc.AddFilToDB(tvrtCode, Filijala, RegionID, Address, City, Zip, Phone, Country))
                {
                    Result = "Branch added.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
                else
                {
                    Result = "Branch NOT added.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                new LogWriter(e1);
            }
        }

        private void zamjeniZabranjeneZnakove(String mAddress, String mCity, String mZip, String mPhone, String mCountry, String mFilijala, String mRegijaID)
        {
            Address = mAddress.Replace("'", "*").Trim();
            Address = Address.Replace("\"", "*").Trim();
            Address = Address.Replace("#", "*").Trim();

            Phone = mPhone.Replace("'", "*").Trim();
            Phone = Phone.Replace("\"", "*").Trim();
            Phone = Phone.Replace("#", "*").Trim();

            City = mCity.Replace("'", "*").Trim();
            City = City.Replace("\"", "*").Trim();
            City = City.Replace("#", "*").Trim();

            Zip = mZip.Replace("'", "*").Trim();
            Zip = Zip.Replace("\"", "*").Trim();
            Zip = Zip.Replace("#", "*").Trim();

            Country = mCountry.Replace("'", "*").Trim();
            Country = Country.Replace("\"", "*").Trim();
            Country = Country.Replace("#", "*").Trim().ToUpper();

            Filijala = mFilijala.Replace("'", "*").Trim();
            Filijala = Filijala.Replace("\"", "*").Trim();
            Filijala = Filijala.Replace("#", "*").Trim().ToUpper();

            mRegijaID = mRegijaID.Replace("'", "*").Trim();
            mRegijaID = mRegijaID.Replace("\"", "*").Trim();
            RegionID = long.Parse(mRegijaID.Replace("#", "*").Trim().ToUpper());
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            String test = comboBox5.Text;

            if(test.Length > 0)
            {
                var isNumeric = int.TryParse(test.Substring(test.Length - 1, 1), out int n);

                if (!isNumeric)
                {
                    comboBox5.Text = comboBox5.Text.Remove(comboBox5.Text.Length - 1);
                }
            }

            comboBox5.SelectionStart = comboBox5.Text.Length;

            int index;
            index = comboBox5.FindStringExact(comboBox5.Text);

            if (index < 0)
                button3.Enabled = false;
            else
                button3.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "qc.UpdateFilToDB";
            String data = tvrtCode + ", " + Filijala + ", " + RegionID + ", " + Address + ", " + City + ", " + Zip + ", " + Phone + ", " + Country;
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///
            try
            {
                if (comboBox4.Text.Equals("") || comboBox5.Text.Equals("") || comboBox6.Text.Equals("")
                    || textBox12.Text.Equals("") || textBox15.Text.Equals("") || textBox16.Text.Equals("") || textBox17.Text.Equals("") || textBox10.Text.Equals(""))
                {
                    Result = "All fields fileds must be filled in.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                    return;
                }
                zamjeniZabranjeneZnakove(textBox12.Text, textBox16.Text, textBox17.Text, textBox15.Text, textBox10.Text, comboBox6.Text, comboBox5.Text);

                QueryCommands qc = new QueryCommands();
                if (qc.UpdateFilToDB(tvrtCode, Filijala, RegionID, Address, City, Zip, Phone, Country))
                {
                    Result = "Branch updated.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
                else
                {
                    Result = "Branch NOT updated.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result);
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                new LogWriter(e1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox3_Leave(sender, e);

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "UpdateTvrtke";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///
            try
            {
                long regID = long.Parse(comboBox3.Text);
                String eur = textBox9.Text.Replace(',', '.');
                String kn = textBox8.Text.Replace(',', '.');

                QueryCommands qc = new QueryCommands();
                qc.UpdateTvrtke(long.Parse(textBox14.Text), comboBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, 
                        textBox6.Text, textBox7.Text, kn, eur, comboBox2.Text, textBox11.Text, regID, textBox13.Text);

                data = long.Parse(textBox14.Text) + ", " + comboBox1.Text + ", " + textBox2.Text + ", " + textBox3.Text + ", " + textBox4.Text + 
                        ", " + textBox5.Text + ", " + textBox6.Text + ", " + textBox7.Text + ", " + kn + ", " + eur + ", " + comboBox2.Text + ", " + textBox11.Text + ", " + regID + ", " + textBox13.Text;
            }
            catch(Exception e1)
            {
                data = comboBox3.Text + ", " + textBox9.Text + ", " + textBox8.Text;
                Result = "Cmp not updated." + Environment.NewLine + e1.ToString();
                lw.LogMe(function, usedQC, data, Result);
                new LogWriter(e1);
                MessageBox.Show(Result);
                return;
            }

            Result = "Cmp updated.";
            lw.LogMe(function, usedQC, data, Result);
            MessageBox.Show(Result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox3_Leave(sender, e);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> arr = qc.GetAllRegions();

            for (int i = 0; i < arr.Count; i = i + 3)
            {
                if (arr[i].Equals(comboBox3.Text))
                {
                    textBox1.Text = arr[i + 2];
                    break;
                }
            }
        }

        private void comboBox3_Leave(object sender, EventArgs e)
        {
            Boolean itIs = false;
            foreach (var item in comboBox3.Items)
            {
                if (item.Equals(comboBox3.Text))
                {
                    itIs = true;
                    break;
                }
            }

            if (!itIs)
                textBox1.ResetText();
        }
    }
}
