using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT
{
    public partial class PartsList : Form
    {
        List<String> resultArr = new List<String>();
        List<String> sifrarnikArr = new List<String>();

        public PartsList(List<String> mResultArr)
        {
            resultArr = mResultArr;
            InitializeComponent();
        }

        private void PartsList_Load(object sender, EventArgs e)
        {
            fillSifrarnik();

            this.Text = "PartList";
            listView1.View = View.Details;
 
            listView1.Columns.Add("PardID");
            listView1.Columns.Add("Name");
            listView1.Columns.Add("CodePartFull");
            listView1.Columns.Add("PartialCode");
            listView1.Columns.Add("SN");
            listView1.Columns.Add("CN");
            listView1.Columns.Add("DateIn");
            listView1.Columns.Add("DateOut");
            listView1.Columns.Add("DateSent");
            listView1.Columns.Add("Storage");
            listView1.Columns.Add("State");
            listView1.Columns.Add("CompanyO");
            listView1.Columns.Add("CompanyC");

            try
            {
                int i = 0;

                if (!resultArr[0].Equals("nok"))
                {
                    for (i = 0; i < resultArr.Count(); i = i + 12)
                    {
                        ListViewItem lvi1 = new ListViewItem();

                        lvi1.Text = resultArr[i];
                        lvi1.SubItems.Add(resultArr[i + 1]);
                        lvi1.SubItems.Add(Decoder.ConnectCodeName(sifrarnikArr, long.Parse(resultArr[i + 1])));
                        lvi1.SubItems.Add(resultArr[i + 2]);
                        lvi1.SubItems.Add(resultArr[i + 3]);
                        lvi1.SubItems.Add(resultArr[i + 4]);
                        lvi1.SubItems.Add(resultArr[i + 5]);
                        lvi1.SubItems.Add(resultArr[i + 6]);
                        lvi1.SubItems.Add(resultArr[i + 7]);
                        lvi1.SubItems.Add(resultArr[i + 8]);
                        lvi1.SubItems.Add(resultArr[i + 9]);
                        lvi1.SubItems.Add(resultArr[i + 10]);
                        lvi1.SubItems.Add(resultArr[i + 11]);
                        listView1.Items.Add(lvi1);
                    }
                }

                this.label2.Text = (i / 12).ToString();
            }
            catch (Exception e1)
            {
                Program.LoadStop();
                this.Focus();

                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }

            for(int i = 0; i < 13; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            Program.LoadStop();
            this.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var builder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        if (item.SubItems[i].Text.Equals(""))
                            builder.Append("" + ",");
                        else
                            builder.Append(item.SubItems[i].Text + ",");
                    }
                    builder.AppendLine();
                }
                //PROVJERITI STO AKO JE PRAZAN
                Clipboard.SetText(builder.ToString());

                MessageBox.Show("Copied.");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var builder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        if (item.SubItems[i].Text.Equals(""))
                            builder.Append(" " + " ");
                        else
                            builder.Append(item.SubItems[i].Text + " ");
                    }
                    builder.AppendLine();
                }

                Clipboard.SetText(builder.ToString());

                MessageBox.Show("Copied.");
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message.ToString());
            }
        }

        private void fillSifrarnik()
        {
            QueryCommands qc2 = new QueryCommands();
            ConnectionHelper cn = new ConnectionHelper();
            List<String> tresultArr = new List<string>();
            int stop = 0;

            try
            {
                while (tresultArr.Count == 0 || tresultArr[0].Equals("nok"))
                {
                    stop++;
                    sifrarnikArr.Clear();
                    tresultArr.Clear();
                    tresultArr = qc2.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);

                    if (stop == 100)
                    {
                        String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        String usedQC = "Loading sifrarnik";
                        String data = "Break limit reached, arr cnt = " + tresultArr.Count;
                        String Result = "";
                        LogWriter lw = new LogWriter();

                        Result = "Cant load 'sifrarnik'.";
                        lw.LogMe(function, usedQC, data, Result);

                        MessageBox.Show(Result);

                        this.Refresh();

                        break;
                    }
                }

                sifrarnikArr = tresultArr;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }
    }
}
