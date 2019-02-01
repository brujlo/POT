using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace POT
{
    public partial class PartsList : Form
    {
        List<String> resultArr = new List<String>();

        public PartsList(List<String> mResultArr)
        {
            resultArr = mResultArr;
            InitializeComponent();
        }

        private void PartsList_Load(object sender, EventArgs e)
        {
            this.Text = "PartList";
            listView1.View = View.Details;
 
            listView1.Columns.Add("PardID");
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
                if (!resultArr[0].Equals("nok"))
                {
                    for (int i = 0; i < resultArr.Count(); i = i + 12)
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
                        listView1.Items.Add(lvi1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            for(int i = 0; i < 12; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
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
    }
}
