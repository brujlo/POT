using POT.WorkingClasses;
using System;
using System.Windows.Forms;

namespace POT.BuildingClasses
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            tax1TB.Text = Properties.Settings.Default.TAX1.ToString();
            tax2TB.Text = Properties.Settings.Default.TAX2.ToString();

            obrJedTB.Text = Properties.Settings.Default.ObracunskaJedinica.ToString();

            Properties.Settings.Default.Save();

            extraLine1ENGTB.Text = Properties.Settings.Default.extraLine1ENGTB.ToString();
            extraLine2ENGTB.Text = Properties.Settings.Default.extraLine2ENGTB.ToString();
            thx1ENGTB.Text = Properties.Settings.Default.thx1ENGTB.ToString();

            extraLine1HRTB.Text = Properties.Settings.Default.extraLine1HRTB.ToString();
            extraLine2HRTB.Text = Properties.Settings.Default.extraLine2HRTB.ToString();
            thx1HRTB.Text = Properties.Settings.Default.thx1HRTB.ToString();
        }

        private void SaveBT_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Print";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                Properties.Settings.Default.TAX1 = int.Parse(tax1TB.Text);
                Properties.Settings.Default.TAX2 = int.Parse(tax2TB.Text);

                Properties.Settings.Default.ObracunskaJedinica = int.Parse(obrJedTB.Text);

                Properties.Settings.Default.extraLine1ENGTB = extraLine1ENGTB.Text;
                Properties.Settings.Default.extraLine2ENGTB = extraLine2ENGTB.Text;
                Properties.Settings.Default.thx1ENGTB = thx1ENGTB.Text;

                Properties.Settings.Default.extraLine1HRTB = extraLine1HRTB.Text;
                Properties.Settings.Default.extraLine2HRTB = extraLine2HRTB.Text;
                Properties.Settings.Default.thx1HRTB = thx1HRTB.Text;

                Properties.Settings.Default.Save();

                data = tax1TB.Text + "; " + tax2TB.Text + "; " + obrJedTB.Text
                     + "; " + extraLine1ENGTB.Text + "; " + extraLine2ENGTB.Text + "; " + thx1ENGTB.Text
                      + "; " + extraLine1HRTB.Text + "; " + extraLine2HRTB.Text + "; " + thx1HRTB.Text;
                lw.LogMe(function, usedQC, data, Result);

                MessageBox.Show("Saved.");
                this.Close();
            }
            catch(Exception e1)
            {
                data = tax1TB.Text + "; " + tax2TB.Text + "; " + obrJedTB.Text
                     + "; " + extraLine1ENGTB.Text + "; " + extraLine2ENGTB.Text + "; " + thx1ENGTB.Text
                      + "; " + extraLine1HRTB.Text + "; " + extraLine2HRTB.Text + "; " + thx1HRTB.Text;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }

        }

        private void CancelBT_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}