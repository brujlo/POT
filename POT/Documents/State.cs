using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace POT
{ 
    public partial class State : Form
    {

        private int regionCnt;
        private int positionX;// = 20;
        private int positionY;// = 100;
        private int positionPlusY;// = 100;
        private int totalG = 0;
        private int totalNG = 0;
        List<String> resultArrSearchName = new List<string>();
        List<String> resultArrSearchCode = new List<string>();

        public State()
        {
            InitializeComponent();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            this.textBox1.ResetText();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            EnterClicked(sender, e);
        }

        private void EnterClicked(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar.GetHashCode().ToString().Equals("851981"))
            //{

            //    //MessageBox.Show("Enter");
            //    e.Handled = true;
            //    FindPart();
            //    this.textBox1.ResetText();
            //}
        }

        private void FindPart()
        {
            QueryCommands qc = new QueryCommands();
            QueryCommands qc1 = new QueryCommands();
            QueryCommands qc2 = new QueryCommands();
            List<String> sendArr = new List<string>();
            List<String> resultArr = new List<string>();
            List<String> resultArr1 = new List<string>();
            List<String> resultArr2 = new List<string>();
            List<String> resultArr3 = new List<string>();
            List<String> resultArrG = new List<string>();
            List<String> resultArrNG = new List<string>();

            for (int i = 1; i <= regionCnt; i++)
            {
                Control ctn1 = this.Controls["lblAdd_" + i];
                Control ctn2 = this.Controls["txtBoxAddNG_" + i];
                Control ctn3 = this.Controls["txtBoxAddG_" + i];

                this.Controls.Remove(ctn1);
                this.Controls.Remove(ctn2);
                this.Controls.Remove(ctn3);
            }

            positionX = 20;
            //positionY = 100;
            positionPlusY = 0;

            if (this.textBox1.TextLength > 12)
            {
                ConnectionHelper cn = new ConnectionHelper();

                try
                {

                    resultArr3 = qc.GetAllRegions();

                    PartSifrarnik prt = new PartSifrarnik();
                    this.label7.Text = prt.PartCode.ToString();
                    Company cmpO = new Company();
                    Company cmpC = new Company();
                    Boolean cmpOb = false;
                    Boolean cmpCb = false;

                    if (Decoder.GetOwnerCode(textBox1.Text).Equals("01"))
                    {
                        MainCmp mpc = new MainCmp();
                        mpc.GetMainCmpByName(Properties.Settings.Default.CmpName);
                        cmpO = mpc.MainCmpToCompany();
                        cmpOb = true;
                    }
                    else
                    {
                        cmpOb = cmpO.GetCompanyInfoByCode(Decoder.GetOwnerCode(textBox1.Text));
                    }

                    if (Decoder.GetOwnerCode(textBox1.Text).Equals("01"))
                    {
                        MainCmp mpc = new MainCmp();
                        mpc.GetMainCmpByName(Properties.Settings.Default.CmpName);
                        cmpC = mpc.MainCmpToCompany();
                        cmpCb = true;
                    }
                    else
                    {
                        cmpCb = cmpC.GetCompanyInfoByCode(Decoder.GetCustomerCode(textBox1.Text));
                    }


                    //if (prt.GetPart(textBox1.Text.Substring(4, 9)) && cmpO.GetCompanyInfoByCode(textBox1.Text.Substring(0, 2)) && cmpC.GetCompanyInfoByCode(textBox1.Text.Substring(2, 2))) //DecoderBB
                    if (prt.GetPart(Decoder.GetFullPartCodeStr(textBox1.Text)) && cmpOb && cmpCb)
                    {
                        this.textBox2.ResetText();
                        this.textBox2.Text = prt.FullName;
                        ////this.label18.Text = string.Format("{0:0000 000 000 000}", long.Parse(this.textBox1.Text));
                        this.label18.Text = this.textBox1.Text;
                        this.label7.Text = cmpO.Name;
                        this.label8.Text = cmpC.Name;
                        this.label9.Text = cmpO.Code;
                        this.label10.Text = cmpC.Code;
                        this.label11.Text = prt.CategoryName;
                        this.label12.Text = prt.PartName;
                        this.label13.Text = string.Format("{0:000}", (prt.CategoryCode / 1000000));
                        this.label14.Text = string.Format("{0:000}", (prt.PartCode / 1000));
                        this.label15.Text = prt.SubPartName;
                        this.label16.Text = string.Format("{0:000}", prt.SubPartCode);

                        this.label21.Text = string.Format("{0:C}", prt.PriceInKn);
                        this.label22.Text = string.Format("{0:C}", prt.PriceOutKn);
                        var culture = new CultureInfo("de-DE");
                        this.label25.Text = string.Format(culture, "{0:C}", prt.PriceInEur);
                        this.label26.Text = string.Format(culture, "{0:C}", prt.PriceOutEur);
                        this.label29.Text = prt.PartNumber;
                        sendArr.Clear();
                        resultArr = qc.InTransport(WorkingUser.Username, WorkingUser.Password, long.Parse(label18.Text.Trim()));
                        this.label30.Text = resultArr[0] == "nok" ? "0" : resultArr[0];
                        this.label32.Text = prt.Packing;
                        this.textBox3.Text = "*" + this.label18.Text + "*";

                        regionCnt = (resultArr3.Count() / 3);
                        int j = 1;
                        int jj = 0;

                        for (int i = 1; i <= regionCnt; i++)
                        {
                            if (!resultArr3[j].Equals("O"))
                            {
                                Label lblAdd = new Label();
                                this.Controls.Add(lblAdd);
                                lblAdd.Size = new Size(50, 20);
                                positionY = this.panel21.Bounds.Bottom + 40 + positionPlusY;// + (40 * i);
                                lblAdd.Location = new Point(positionX, positionY);
                                lblAdd.Font = new Font(lblAdd.Font, FontStyle.Bold);
                                lblAdd.Name = "lblAdd_" + i;
                                lblAdd.Text = resultArr3[j] + " - " + resultArr3[j - 1];

                                TextBox txtBoxAddG = new TextBox();
                                this.Controls.Add(txtBoxAddG);

                                txtBoxAddG.Size = new Size(60, 60);
                                positionY = this.panel21.Bounds.Bottom + 60 + positionPlusY;// + (40 * i);
                                txtBoxAddG.Location = new Point(positionX, positionY);
                                txtBoxAddG.BackColor = Color.LightGreen;
                                txtBoxAddG.TextAlign = HorizontalAlignment.Center;
                                txtBoxAddG.ReadOnly = true;
                                resultArrG.Clear();
                                sendArr.Clear();

                                if (resultArr3[jj + 1].Equals("S"))
                                    resultArrG = qc.PartsCntGS(WorkingUser.Username, WorkingUser.Password, long.Parse(label18.Text.Trim()));
                                else
                                    resultArrG = qc.PartsCntG(WorkingUser.Username, WorkingUser.Password, long.Parse(label18.Text.Trim()), long.Parse(resultArr3[jj].Trim()));

                                txtBoxAddG.Name = "txtBoxAddG_" + i;
                                txtBoxAddG.Text = resultArrG[0] == "nok" ? "" : resultArrG[0].Equals("0") ? "" : resultArrG[0];
                                if (!txtBoxAddG.Text.Equals("")) totalG = totalG + int.Parse(txtBoxAddG.Text);

                                TextBox txtBoxAddNG = new TextBox();
                                this.Controls.Add(txtBoxAddNG);

                                txtBoxAddNG.Size = new Size(60, 60);
                                positionX = txtBoxAddG.Bounds.Right + 10;
                                txtBoxAddNG.Location = new Point(positionX, positionY);
                                txtBoxAddNG.BackColor = Color.LightCoral;
                                txtBoxAddNG.TextAlign = HorizontalAlignment.Center;
                                txtBoxAddNG.ReadOnly = true;
                                resultArrNG.Clear();
                                sendArr.Clear();

                                if(resultArr3[jj + 1].Equals("S"))
                                    resultArrNG = qc.PartsCntNGS(WorkingUser.Username, WorkingUser.Password, long.Parse(label18.Text.Trim()));
                                else
                                    resultArrNG = qc.PartsCntNG(WorkingUser.Username, WorkingUser.Password, long.Parse(label18.Text.Trim()), long.Parse(resultArr3[jj].Trim()));

                                txtBoxAddNG.Name = "txtBoxAddNG_" + i;
                                txtBoxAddNG.Text = resultArrNG[0] == "nok" ? "" : resultArrNG[0].Equals("0") ? "" : resultArrNG[0];
                                if (!txtBoxAddNG.Text.Equals("")) totalNG = totalNG + int.Parse(txtBoxAddNG.Text);

                                txtBoxAddNG.Click += new EventHandler(showList);
                                txtBoxAddG.Click += new EventHandler(showList);

                                positionX = positionX + 80;

                                if((positionX + 150) >= ClientRectangle.Width)
                                {
                                    positionPlusY = positionPlusY + 50;
                                    positionX = 20;
                                }
                            }
                            j = j + 3;
                            jj = jj + 3;
                        }
                        textBox4.Text = totalG.ToString();
                        textBox5.Text = totalNG.ToString();
                    }
                    else
                    {
                        clearME();
                    }
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    MessageBox.Show(e1.Message);
                    clearME();
                    return;
                }
            }
        }

        private void showList(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> resultArr = new List<string>();
            List<PartSifrarnik> partArr = new List<PartSifrarnik>();

            ConnectionHelper cn = new ConnectionHelper();

            Control regionIdcontrol = this.Controls["lblAdd_" + ((TextBox)sender).Name.Split('_')[1]];

            try
            {
                resultArr = qc.ListPartsByCodeRegionStateS(WorkingUser.Username, WorkingUser.Password, long.Parse(label18.Text), long.Parse(regionIdcontrol.Text.Split('-')[1].Trim().ToString()), ((TextBox)sender).Name.Contains("txtBoxAddNG") ? "ng" : "g");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            PartsList pl = new PartsList(resultArr);
            pl.Show();
            textBox1.Focus();
            pl.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                totalG = 0;
                totalNG = 0;
                //MessageBox.Show("Enter");
                e.Handled = true;
                FindPart();
                this.textBox1.ResetText();
            }
        }

        private void clearME()
        {
            this.label18.Text = "";
            this.textBox2.ResetText();
            this.textBox2.Text = "No match found";
            this.label7.ResetText();
            this.label8.ResetText();
            this.label9.ResetText();
            this.label10.ResetText();
            this.label11.ResetText();
            this.label12.ResetText();
            this.label13.ResetText();
            this.label14.ResetText();
            this.label15.ResetText();
            this.label16.ResetText();

            this.label21.ResetText();
            this.label22.ResetText();
            this.label25.ResetText();
            this.label26.ResetText();
            this.label29.ResetText();
            this.label30.ResetText();
            this.label32.ResetText();
            this.textBox3.ResetText();
            this.comboBox1.ResetText();
            comboBox2.Text = Properties.Settings.Default.MainCompanyCode;
            comboBox3.Text = Properties.Settings.Default.MainCompanyCode;

            textBox4.ResetText();
            textBox5.ResetText();
            totalG = 0;
            totalNG = 0;
        }

        private void State_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            List<String> sendArr = new List<string>();
            List<String> resultArr = new List<string>();
            List<Company> resultArrC = new List<Company>();
            ConnectionHelper cn = new ConnectionHelper();

            comboBox2.Text = Properties.Settings.Default.MainCompanyCode;
            comboBox3.Text = Properties.Settings.Default.MainCompanyCode;

            try
            {
                Company cmpList = new Company();
                resultArrC = cmpList.GetAllCompanyInfoSortCode();

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.comboBox2.Items.Add(resultArrC[i].Code);
                        this.comboBox3.Items.Add(resultArrC[i].Code);
                    }

                }

                resultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);
                sendArr.Clear();

                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 2)
                    {
                        this.comboBox1.Items.Add(resultArr[i]);
                        //resultArrSearchName.Add(resultArr[i]);
                        resultArrSearchCode.Add(resultArr[i + 1]);
                    }

                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                clearME();
                return;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.Text = comboBox2.Text + comboBox3.Text + string.Format("{0:000000000}", int.Parse(resultArrSearchCode.ElementAt(comboBox1.SelectedIndex)));
            SendKeys.Send("{ENTER}");
        }
    }
}
