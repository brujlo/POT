using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POT
{
    public partial class OpenTicketList : Form
    {
        List<String> resultArr = new List<String>();
        public delegate void CallBack(Boolean isOpened);

        public OpenTicketList(List<String> mResultArr)
        {
            resultArr = mResultArr;
            InitializeComponent();
        }

        private void OpenTicketList_Load(object sender, EventArgs e)
        {
            this.Text = "Open Tickets";
            listView1.View = View.Details;

            listView1.Columns.Add("TID");
            listView1.Columns.Add("CompanyID");
            listView1.Columns.Add("Prio");
            listView1.Columns.Add("Branch");
            listView1.Columns.Add("CCN");
            listView1.Columns.Add("CID");
            listView1.Columns.Add("DatInq");
            listView1.Columns.Add("TimeInq");
            listView1.Columns.Add("DatSLA");
            listView1.Columns.Add("TimeSLA");
            listView1.Columns.Add("Drive");
            listView1.Columns.Add("PartName");
            listView1.Columns.Add("Reported");
            listView1.Columns.Add("UserIDTake");
            listView1.Columns.Add("DatTake");
            listView1.Columns.Add("TimeTake");
            listView1.Columns.Add("UserIDDrive");
            listView1.Columns.Add("DatDrive");
            listView1.Columns.Add("TimeDrive");
            listView1.Columns.Add("UserIDStart");
            listView1.Columns.Add("DatStart");
            listView1.Columns.Add("TimeStart");
            listView1.Columns.Add("UserIDEnd");
            listView1.Columns.Add("DatEnd");
            listView1.Columns.Add("TimeENd");
            listView1.Columns.Add("MadeID");
            listView1.Columns.Add("Description");

            try
            {
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 27)
                    {
                        ListViewItem lvi1 = new ListViewItem();

                        lvi1.Text = resultArr[i];
                        lvi1.SubItems.Add(resultArr[i + 1]);
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
                        lvi1.SubItems.Add(resultArr[i + 12]);
                        lvi1.SubItems.Add(resultArr[i + 13]);
                        lvi1.SubItems.Add(resultArr[i + 14]);
                        lvi1.SubItems.Add(resultArr[i + 15]);
                        lvi1.SubItems.Add(resultArr[i + 16]);
                        lvi1.SubItems.Add(resultArr[i + 17]);
                        lvi1.SubItems.Add(resultArr[i + 18]);
                        lvi1.SubItems.Add(resultArr[i + 19]);
                        lvi1.SubItems.Add(resultArr[i + 20]);
                        lvi1.SubItems.Add(resultArr[i + 21]);
                        lvi1.SubItems.Add(resultArr[i + 22]);
                        lvi1.SubItems.Add(resultArr[i + 23]);
                        lvi1.SubItems.Add(resultArr[i + 24]);
                        lvi1.SubItems.Add(resultArr[i + 25]);
                        lvi1.SubItems.Add(resultArr[i + 26]);
                        listView1.Items.Add(lvi1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            for (int i = 0; i < 27; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                var builder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        builder.Append(item.SubItems[i].Text + " ");
                    }
                    builder.AppendLine();
                }

                Clipboard.SetText(builder.ToString());

                MessageBox.Show("Copied.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var builder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        builder.Append(item.SubItems[i].Text + ",");
                    }
                    builder.AppendLine();
                }

                Clipboard.SetText(builder.ToString());

                MessageBox.Show("Copied.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
