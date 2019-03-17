using POT.MyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using POT.WorkingClasses;
using System.Media;

namespace POT.Documents
{
    public partial class Tickets : Form
    {
        ConnectionHelper cn = new ConnectionHelper();
        List<Company> resultArrC = new List<Company>();
        static List<String> sifrarnikArr = new List<string>();

        AddTIDDB at = new AddTIDDB();
        Company cmp = new Company();

        public Tickets()
        {
            InitializeComponent();
            AppendTextBox("- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "Waiting");
        }

        private void Tickets_Load(object sender, EventArgs e)
        {
            QueryCommands qc = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            Company cmpList = new Company();

            this.nextID.Text = (qc.GetLastTicketID(WorkingUser.Username, WorkingUser.Password) + 1).ToString();

            try
            {
                resultArrC = cmpList.GetAllCompanyInfoSortByName();

                if (!resultArrC[0].Name.Equals(""))
                {
                    for (int i = 0; i < resultArrC.Count(); i++)
                    {
                        this.companyCB.Items.Add(resultArrC[i].Name);
                    }

                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void datPrijave_Enter(object sender, EventArgs e)
        {
            try
            {
                datPrijave_Leave(sender, e);
                this.datPrijave.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
            
            

        }

        private void datPrijave_Leave(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                if (this.datPrijave.Text.Equals(""))
                    this.datPrijave.Text = DateTime.Now.ToString("dd.MM.yy.");
                else
                    this.datPrijave.Text = dc.ConvertDDMMYY(datPrijave.Text);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void vriPrijave_Enter(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                this.vriPrijave.Text = dc.ConvertTimeHHMM(this.vriPrijave.Text);

                this.vriPrijave.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
        }

        private void vriPrijave_Leave(object sender, EventArgs e)
        {
            try
            {
                vriPrijave_Enter(sender, e);
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void SLAdat_Enter(object sender, EventArgs e)
        {
            try
            {
                SLAdat_Leave(sender, e);
                this.SLAdat.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
        }

        private void SLAdat_Leave(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                if(this.prio.Text.Equals("6"))
                    this.SLAdat.Text = dc.CalculatePrio(this.prio.Text, this.SLAdat.Text, this.SLAvri.Text).ToString("dd.MM.yy.");
                else
                    this.SLAdat.Text = dc.CalculatePrio(this.prio.Text, this.datPrijave.Text, this.vriPrijave.Text).ToString("dd.MM.yy.");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void SLAvri_Enter(object sender, EventArgs e)
        {
            try
            {
                SLAvri_Leave(sender, e);
                this.datPrijave.SelectAll();
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                this.datPrijave.SelectAll();
            }
        }

        private void SLAvri_Leave(object sender, EventArgs e)
        {
            try
            {
                DateConverter dc = new DateConverter();

                if (this.prio.Text.Equals("6"))
                    this.SLAvri.Text = dc.CalculatePrio(this.prio.Text, this.SLAdat.Text, this.SLAvri.Text).ToString("HH:mm");
                else
                    this.SLAvri.Text = dc.CalculatePrio(this.prio.Text, this.datPrijave.Text, this.vriPrijave.Text).ToString("HH:mm");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
            }
        }

        ///////////////////////////////////////////////////////////

        private void naziv_TextChanged(object sender, EventArgs e)
        {
            label10.Text = "Letters left " + (400 - naziv.TextLength).ToString();
        }

        private void opis_TextChanged(object sender, EventArgs e)
        {
            label11.Text = "Letters left " + (400 - opis.TextLength).ToString();
        }

        private void drive_Enter(object sender, EventArgs e)
        {
            this.drive.SelectAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.companyCB.Text.Equals("") || this.companyCB.Text.Equals("Search by company name") || this.datPrijave.Text.Equals("") || this.vriPrijave.Text.Equals("") || this.drive.Text.Equals("") 
                || this.SLAdat.Text.Equals("") || this.SLAvri.Text.Equals("") || this.label30.Text.Equals("") || this.filijala.Text.Equals("") 
                || this.ccn.Text.Equals("") || this.cid.Text.Equals("") || this.prio.Text.Equals("") || this.opis.Text.Equals("") || this.naziv.Text.Equals(""))
            {
                AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "All fields must filed in" + System.Environment.NewLine + "    - Nothing done");
                return;
            }

            Branch br = new Branch();
            QueryCommands qc = new QueryCommands();

            long TIDid = qc.GetLastTicketID(WorkingUser.Username, WorkingUser.Password) + 1;
            br.SetFilByTvrtkeCodeFilNumber(cmp.Code, this.filijala.Text.Trim());

            if (!qc.IsTIDExistByIDCCNCID(TIDid, ccn.Text, cid.Text))
            {
                if (at.sendIntervention(cmp, datPrijave.Text, vriPrijave.Text, prio.Text, SLAdat.Text, SLAvri.Text, filijala.Text, ccn.Text, cid.Text, TIDid, drive.Text, InqUser.Text, naziv.Text, opis.Text, "", br.City, br.Address, br.Phone, this))
                    AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + " Added and sent");
            }
            else
            {
                String msg = System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + " TID already exist";
                AppendTextBox(msg);
                new LogWriter(msg);
            }

            SystemSounds.Hand.Play();
        }

        private void companyCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.companyCB.SelectedIndex;

            cmp.Clear();

            cmp.Address = resultArrC[index].Address;
            cmp.BIC = resultArrC[index].BIC;
            cmp.City = resultArrC[index].City;
            cmp.Code = resultArrC[index].Code;
            cmp.Contact = resultArrC[index].Contact;
            cmp.Country = resultArrC[index].Country;
            cmp.EUR = resultArrC[index].EUR;
            cmp.Email = resultArrC[index].Email;
            cmp.ID = resultArrC[index].ID;
            cmp.KN = resultArrC[index].KN;
            cmp.Name = resultArrC[index].Name;
            cmp.OIB = resultArrC[index].OIB;
            cmp.PB = resultArrC[index].PB;
            cmp.RegionID = resultArrC[index].RegionID;


            this.cmpID.Text = resultArrC.ElementAt(companyCB.SelectedIndex).ID.ToString().Trim();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.companyCB.ResetText();
            this.datPrijave.ResetText();
            this.vriPrijave.ResetText();
            this.drive.ResetText();
            this.SLAdat.ResetText();
            this.SLAvri.ResetText();
            this.label30.ResetText();
            this.filijala.ResetText();
            this.ccn.ResetText();
            this.cid.ResetText();
            this.prio.ResetText();
            this.opis.ResetText();
            this.naziv.ResetText();

            AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "All reseted" + System.Environment.NewLine + "    - Waiting");
        }

        private void filijala_Leave(object sender, EventArgs e)
        {
            this.filijala.Text = this.filijala.Text.ToUpper();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            this.label30.Text = this.label30.Text.ToUpper();
        }

        private void ccn_Leave(object sender, EventArgs e)
        {
            this.ccn.Text = this.ccn.Text.ToUpper();
        }

        private void cid_Leave(object sender, EventArgs e)
        {
            this.cid.Text = this.cid.Text.ToUpper();
        }

        ///////////////////////////////////////////////////////////

        public void AppendTextBox(String value)
        {
            this.textBox3.AppendText(value + System.Environment.NewLine);
            new LogWriter(value);
        }
    }
}
