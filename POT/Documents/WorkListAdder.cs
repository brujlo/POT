using POT.WorkingClasses;
using System;
using System.Windows.Forms;

namespace POT.Documents
{
    public partial class WorkListAdder : Form
    {
        private WorksISS wss = new WorksISS();
        private WorkListISS wli;

        public WorkListAdder()
        {
            InitializeComponent();
        }
        private void WorkListAdder_Load(object sender, EventArgs e)
        {
            this.Focus();
        }

        public void SendValues(WorksISS _wss, WorkListISS form, Boolean idChange, int lastNbr)
        {
            //if(_hrv.Equals("") || _eng.Equals("") || _id.Equals(""))
            //{

            //}

            //textBox3.Enabled = idChange;

            wli = form;
            this.wss.CopyFrom(_wss);

            textBox1.Text = wss.Hrv;
            textBox2.Text = wss.Eng;
            textBox3.Text = idChange ?  (lastNbr + 1).ToString() : wss.Id;
        }

        private void okBT_Click(object sender, EventArgs e)
        {
            wss.Hrv = textBox1.Text;
            wss.Eng = textBox2.Text;
            wss.Id = textBox3.Text;

            wli.GetValues(wss,true);

            this.Close();
        }

        private void cancelBT_Click(object sender, EventArgs e)
        {
            wli.GetValues(wss, false);
        }
    }
}
