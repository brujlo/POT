using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace POT.Documents
{
    public partial class WorkListISS : Form
    {
        private WorksISS wss = new WorksISS();
        private int maxValue;
        private int indeks;
        private Boolean okBtn;


        private List<WorksISS> worksLst = new List<WorksISS>();
        private List<WorksISS> initialLst = new List<WorksISS>();
        
        QueryCommands qc = new QueryCommands();


        public WorkListISS()
        {
            InitializeComponent();
        }

        private void WorkListISS_Load(object sender, EventArgs e)
        {
            maxValue = 0;
            wss.Hrv = "";
            wss.Eng = "";
            wss.Id = "";

            try
            {
                initialLst = qc.GetAllFromWork();
                RefillList();

                this.Focus();
            }
            catch(Exception e1)
            {
                
                LogWriter lw = new LogWriter();
                lw.WriteLog(e1.Message);
            }
            finally
            {
                Program.LoadStop();
            }
        }

        private void RefillList()
        {
            listView1.Clear();

            try
            {
                if(worksLst.Count <= 0)
                    worksLst = qc.GetAllFromWork();

                listView1.View = View.Details;

                listView1.Columns.Add("RB");
                listView1.Columns.Add("ODRADENO");
                listView1.Columns.Add("WORK DONE");
                listView1.Columns.Add("VALUE");



                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                if (worksLst.Count == 0)
                    return;

                maxValue = worksLst.Count;

                int rb = 0;
                worksLst.Sort((a, b) => a.Id.CompareTo(b.Id));

                foreach (WorksISS ws in worksLst)
                {
                    ListViewItem lvi1 = new ListViewItem((++rb).ToString());

                    lvi1.SubItems.Add(ws.Hrv);
                    lvi1.SubItems.Add(ws.Eng);
                    lvi1.SubItems.Add(ws.Id);
                    
                    listView1.Items.Add(lvi1);

                    maxValue = Math.Max(maxValue, int.Parse(ws.Id));
                }

                if (listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);


                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }
            }
            catch (Exception e1)
            {

                LogWriter lw = new LogWriter();
                lw.WriteLog(e1.Message);

                MessageBox.Show(e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addBT_Click(object sender, EventArgs e)
        {
            WorkListAdder wla = new WorkListAdder();
            
            String mID = wss.Id;
            wss = new WorksISS();
            wss.Id = mID;

            wla.SendValues(wss, this, true, maxValue);

            wla.ShowDialog();

            if(okBtn)
                RefillList();
        }

        private void deletBT_Click(object sender, EventArgs e)
        {
            int k = 1;
            int mRB;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                mRB = int.Parse(item.SubItems[0].Text);

                WorksISS itm = new WorksISS();
                itm = worksLst.Find(w => w.Id == item.SubItems[3].Text);

                listView1.Items.Remove(item);
                worksLst.Remove(itm);
            }

            RefillList();
        }

        private void editBT_Click(object sender, EventArgs e)
        {
            WorksISS tmp = new WorksISS();

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                tmp.Hrv = item.SubItems[1].Text;
                tmp.Eng = item.SubItems[2].Text;
                tmp.Id = item.SubItems[3].Text;
            }

            WorkListAdder wla = new WorkListAdder();
            wla.SendValues(tmp, this, false, maxValue);
            wla.ShowDialog();

            if (okBtn)
            {
                worksLst.RemoveAll(o => o.Id == tmp.Id);

                worksLst.Add(wss);

                RefillList();
            }
        }

        private void saveBT_Click(object sender, EventArgs e)
        {
            List<WorksISS> saveLst = new List<WorksISS>();

            foreach (ListViewItem item in listView1.Items)
            {
                WorksISS tmp = new WorksISS();

                tmp.Hrv = item.SubItems[1].Text;
                tmp.Eng = item.SubItems[2].Text;
                tmp.Id = item.SubItems[3].Text;

                saveLst.Add(tmp);
            }

            if (qc.SaveToWork(saveLst))
            {
                MessageBox.Show("All data are saved!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Data not saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetValues(WorksISS _wss, Boolean _okBtn)
        {
            okBtn = _okBtn;

            if (okBtn)
            {
                try
                {
                    wss.CopyFrom(_wss);

                    worksLst.Add(_wss);

                    RefillList();
                }
                catch(Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    indeks = item.Index;
                }
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }
    }
}
