using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Decoder = POT.WorkingClasses.Decoder;

namespace POT.Documents
{
    public partial class ISS : Form
    {
        long h = 0;
        long m = 0;
        long s = 0;

        int rb = 0;
        int doNotRepeatMsg = 0;

        long ISSid = 0;

        DateTime startDate = DateTime.Now;

        Boolean timerPaused = false;
        Boolean timerStarted = false;
        int stopClicked = 0;

        QueryCommands qc = new QueryCommands();
        List<Part> partList = new List<Part>();
        List<PartSifrarnik> allParts = new List<PartSifrarnik>();
        List<String> sifrarnikArr = new List<String>();
        Boolean dataLoaded = false;
        List<List<Part>> groupedGoodPartsCode = new List<List<Part>>();
        List<ISSparts> listIssPartsOld = new List<ISSparts>();
        List<long> ISSids = new List<long>();
        Part newSendPart = new Part();
        Part mainPart = new Part();

        int partIndex = -1; //sluzi za grupiranu listu podataka da izvucem podpodatak za po SN da dobijem cn


        public ISS()
        {
            InitializeComponent();
        }

        private void ISS_Load(object sender, EventArgs e)
        {
            partList = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "sng");

            List<Part> goodParts = new List<Part>();
            goodParts = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "g");

            allParts = qc.GetPartsAllSifrarnik();

            if (partList.Count > 0)
            {
                for (int i = 0; i < partList.Count(); i++)
                {
                    PartCb.Items.Add(partList[i].CodePartFull);
                }

            }

            fillSifrarnik();

            groupedGoodPartsCode = goodParts.GroupBy(c => c.CodePartFull).Select(grp => grp.ToList()).ToList();

            if (groupedGoodPartsCode.Count > 0)
            {
                for (int i = 0; i < groupedGoodPartsCode.Count(); i++)
                {
                    NewPartCb.Items.Add(Decoder.ConnectCodeName(sifrarnikArr, groupedGoodPartsCode[i][0]));
                }

            }

            if (allParts.Count > 0)
            {
                for (int i = 0; i < allParts.Count(); i++)
                {
                    OldPartCb.Items.Add(allParts[i].FullName);
                }

            }

            listView1.View = View.Details;

            listView1.Columns.Add("RB");
            listView1.Columns.Add("Name");
            listView1.Columns.Add("CodeO");
            listView1.Columns.Add("SNO");
            listView1.Columns.Add("CNO");
            listView1.Columns.Add("CodeN");
            listView1.Columns.Add("SNN");
            listView1.Columns.Add("CNN");
            listView1.Columns.Add("Date");
            listView1.Columns.Add("Time");
            listView1.Columns.Add("Work done");
            listView1.Columns.Add("Comment");

            for (int i = 0; i < 12; i++)
            {
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            ISSids = qc.GetAllISSOpenClose(0);

            for (int i = 0; i < ISSids.Count; i++)
            {
                ISSSelectorCb.Items.Add(ISSids[i]);
            }

            STARTbt_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            s++;
            if (s > 59)
                calculateTime(s);
            this.TIMERlb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
        }

        public void AppendTextBox(String value)
        {
            this.textBox1.AppendText(value + System.Environment.NewLine);
            new LogWriter(value);
        }

        private void PAUSEbt_Click(object sender, EventArgs e)
        {
            if (!dataLoaded)
            {
                PAUSEbt.Enabled = false;
                STARTbt.Enabled = false;
                STOPbt.Enabled = false;
                return;
            }
            //stopClicked = 0;
            if (stopClicked == 0)
            {
                if (timerPaused)
                {
                    timer1.Start();
                    PAUSEbt.BackColor = Color.AliceBlue;
                    STARTbt.BackColor = Color.Yellow;
                    STOPbt.BackColor = Color.AliceBlue;
                    timerPaused = false;
                }
                else
                {
                    timer1.Stop();
                    PAUSEbt.BackColor = Color.Yellow;
                    STARTbt.BackColor = Color.Yellow;
                    STOPbt.BackColor = Color.AliceBlue;
                    timerPaused = true;
                }
            }
        }

        private void STARTbt_Click(object sender, EventArgs e)
        {
            if (!dataLoaded)
            {
                PAUSEbt.Enabled = false;
                STARTbt.Enabled = false;
                STOPbt.Enabled = false;
                return;
            }

            if (!timerStarted)
            {
                stopClicked = 0;    
                timer1.Start();
                PAUSEbt.BackColor = Color.AliceBlue;
                STARTbt.BackColor = Color.Yellow;
                STOPbt.BackColor = Color.AliceBlue;
                timerStarted = true;
            }
        }

        private void STOPbt_Click(object sender, EventArgs e)
        {
            if (!dataLoaded)
            {
                PAUSEbt.Enabled = false;
                STARTbt.Enabled = false;
                STOPbt.Enabled = false;
                return;
            }

            if (timerStarted)
            {
                switch (stopClicked)
                {
                    case 0:
                        timer1.Stop();
                        stopClicked++;
                        STOPbt.Text = "ADD TO WORKLIST";
                        PAUSEbt.BackColor = Color.AliceBlue;
                        STARTbt.BackColor = Color.AliceBlue;
                        STOPbt.BackColor = Color.Yellow;
                        CANCELBt.Enabled = true;
                        break;
                    case 1:
                        stopClicked = 0;
                        timerStarted = false;
                        STOPbt.Text = "STOP";
                        PAUSEbt.BackColor = Color.AliceBlue;
                        STARTbt.BackColor = Color.AliceBlue;
                        STOPbt.BackColor = Color.AliceBlue;
                        AddToWorkList();
                        h = 0; m = 0; s = 0;
                        this.TIMERlb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
                        break;
                }
            }
        }

        private void CANCELBt_Click(object sender, EventArgs e)
        {
            h = 0; m = 0; s = 0;
            this.TIMERlb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
            PAUSEbt.BackColor = Color.AliceBlue;
            STARTbt.BackColor = Color.AliceBlue;
            STOPbt.BackColor = Color.AliceBlue;
            CANCELBt.Enabled = false;
            stopClicked = 0;
            timerStarted = false;
            STOPbt.Text = "STOP";
        }

        private void calculateTime(long pS)
        {
            if (pS % 60 == 0)
            {
                s = 0;
                m++;
                if(m % 60 == 0)
                {
                    m = 0;
                    h++;
                }
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
                    tresultArr = qc.SelectNameCodeFromSifrarnik(WorkingUser.Username, WorkingUser.Password);

                    if (stop == 100)
                    {
                        MessageBox.Show("Cant load 'sifrarnik'.");
                        String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        String usedQC = "Loading sifrarnik";
                        String data = "Break limit reached, arr cnt = " + tresultArr.Count;
                        String Result = "";
                        LogWriter lw = new LogWriter();

                        ChangeColor("Red");

                        Result = "Cant load 'sifrarnik'.";
                        lw.LogMe(function, usedQC, data, Result);

                        dataLoaded = false;
                        return;
                    }
                }
                if (stop < 100)
                    ChangeColor("Green");

                sifrarnikArr = tresultArr;

                dataLoaded = true;
            }
            catch (Exception e1)
            {
                ChangeColor("Red");
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
                dataLoaded = false;
            }
        }

        public void ChangeColor(string color)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(ChangeColor), new object[] { color });
                return;
            }

            if (color.Equals("Green"))
                this.button4.BackColor = Color.Green;
            else
                this.button4.BackColor = Color.Red;
        }

        private void PartCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            NameTb.Text = Decoder.ConnectCodeName(sifrarnikArr, partList[PartCb.SelectedIndex]);
            SNTb.Text = partList[PartCb.SelectedIndex].SN;
            CNTb.Text = partList[PartCb.SelectedIndex].CN;
            DateInTb.Text = partList[PartCb.SelectedIndex].DateIn;
            DateSentTb.Text = partList[PartCb.SelectedIndex].DateSend;
            IDTb.Text = partList[PartCb.SelectedIndex].PartID.ToString();
            mainPart = partList[PartCb.SelectedIndex];
        }

        private void OldPartCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OldPartCb.SelectedIndex > -1)
            {
                if (PartCb.Text.Equals(""))
                {
                    OldPartCodeTb.ResetText();
                    OldPartCb.ResetText();
                    OldPartCb.SelectedIndex = -1;
                    if (doNotRepeatMsg == 0)
                    {
                        AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "Please enter the part you are repairing.");
                        doNotRepeatMsg = 1;
                    }
                    else
                    {
                        doNotRepeatMsg = 0;
                    }
                }
                else
                {
                    OldPartCodeTb.Text = Decoder.GetOwnerCode(PartCb.Text) + Decoder.GetCustomerCode(PartCb.Text) + Decoder.GetFullPartCodeStr(allParts[OldPartCb.SelectedIndex].FullCode); 
                }
            }
        }

        private void NewPartCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(NewPartCb.SelectedIndex > -1)
            {
                partIndex = NewPartCb.SelectedIndex;

                NewPartCodeTb.Text = groupedGoodPartsCode[partIndex][0].CodePartFull.ToString();

                newSendPart = new Part();
                newSendPart = groupedGoodPartsCode[partIndex][0];

                NewPartCNTb.ResetText();
                NewPartSNCb.ResetText();
                NewPartSNCb.Items.Clear();

                for (int i = 0; i < groupedGoodPartsCode[partIndex].Count(); i++)
                {
                    if (!groupedGoodPartsCode[partIndex][i].SN.Equals(""))
                        NewPartSNCb.Items.Add(groupedGoodPartsCode[partIndex][i].SN);
                }

                if (NewPartSNCb.Items.Count == 0)
                {
                    NewPartCNTb.Text = groupedGoodPartsCode[partIndex][0].CN;
                    NewPartSNCb.Items.Add("");
                }
            }
        }

        private void NewPartSNCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            newSendPart = new Part();

            int snIndex = NewPartSNCb.SelectedIndex;
            NewPartCNTb.ResetText();
            if (!NewPartSNCb.Items[0].Equals(""))
            {
                NewPartCNTb.Text = groupedGoodPartsCode[partIndex][snIndex].CN.ToString();
                newSendPart = groupedGoodPartsCode[partIndex][snIndex];
            }
            else
            {
                NewPartCNTb.Text = groupedGoodPartsCode[partIndex][0].CN.ToString();
                newSendPart = groupedGoodPartsCode[partIndex][snIndex];
            }
        }

        private void AddToWorkList()
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Adding to list";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                rb = listView1.Items.Count + 1;

                String name = NameTb.Text.Trim();
                String partCode = PartCb.Text.Trim();
                String CodeO = OldPartCodeTb.Text.Trim();
                String SNO = OLDPartSNTb.Text.Trim().ToUpper();
                String CNO = OldPartCNTb.Text.Trim().ToUpper();
                String CodeN = NewPartCodeTb.Text.Trim();
                String SNN = NewPartSNCb.Text.Trim().ToUpper();

                int obrJed = Properties.Settings.Default.ObracunskaJedinica;
                String CNN = NewPartCNTb.Text.Trim().ToUpper();

                DateConverter dt = new DateConverter();
                String date = dt.ConvertDDMMYY(DateTime.Now.ToString());

                m = m < obrJed ? m = obrJed : m = (int)(m / obrJed) * obrJed + obrJed;
                if(m >= 60)
                {
                    m = 0;
                    h++;
                }
                String time = string.Format("{0:00}", h) + ":" + string.Format("{0:00}", m);

                String work = WorkDoneCb.Text.Trim();
                String koment = ComentTb.Text.Trim();

                //lvi1.SubItems.Add(sifrarnikArr[sifrarnikArr.IndexOf((long.Parse((textBox1.Text).Substring(4)).ToString())) - 1]); //DecoderBB

                ListViewItem lvi1 = new ListViewItem(rb.ToString());

                lvi1.SubItems.Add(name);
                lvi1.SubItems.Add(CodeO);
                lvi1.SubItems.Add(SNO);
                lvi1.SubItems.Add(CNO);
                lvi1.SubItems.Add(CodeN);
                lvi1.SubItems.Add(SNN);
                lvi1.SubItems.Add(CNN);
                lvi1.SubItems.Add(date);
                lvi1.SubItems.Add(time);
                lvi1.SubItems.Add(work);
                lvi1.SubItems.Add(koment);

                if (listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                listView1.Items.Add(lvi1);
                /*
                partsArr.Add(textBox1.Text);
                partsArr.Add(textBox2.Text);
                partsArr.Add(textBox3.Text);
                partsArr.Add(radioButton1.Checked ? "g" : "ng");
                */
                
                rb = listView1.Items.Count + 1;

                data = name + ", " + CodeO + ", " + SNO + ", " + CNO + ", " + CodeN + ", " + SNN + ", " + CNN + ", " + date + ", " + time + ", " + work + ", " + koment;

                for (int i = 0; i < 12; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                Result = "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "Added to list.";
                lw.LogMe(function, usedQC, data, Result);

                AppendTextBox(Result);

                WorkDoneCb.ResetText();
                OldPartCodeTb.ResetText();
                OLDPartSNTb.ResetText();
                OldPartCNTb.ResetText();
                NewPartCodeTb.ResetText();
                WorkDoneCb.ResetText();
                ComentTb.ResetText();

                NewPartSNCb.ResetText();
                NewPartSNCb.Items.Clear();
                NewPartSNCb.Items.Add("");
                NewPartSNCb.SelectedIndex = -1;

                NewPartCNTb.ResetText();

                OldPartCb.ResetText();
                OldPartCb.SelectedIndex = -1;

                NewPartCb.ResetText();
                NewPartCb.SelectedIndex = -1;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
                //numericUpDown1.Value = 1;
            }
        }

        private void ComentTb_TextChanged(object sender, EventArgs e)
        {
            label17.Text = "Letters left " + (200 - ComentTb.TextLength).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Save to db";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            DateConverter dt = new DateConverter();
            String _date = dt.ConvertDDMMYY(DateTime.Now.ToString());
            Boolean issExist = false;
            Boolean allDone = checkBox1.Checked;
            Company cmpCust = new Company();

            listIssPartsOld.Clear();

            try
            {
                ISSid = qc.ISSExistIfNotReturnNewID(ISSid);
            
                if (ISSid == 0)
                {
                    issExist = true;
                }

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    long CodeO = 0;

                    if (!listView1.Items[i].SubItems[2].Text.Trim().Equals(""))
                        CodeO = long.Parse(listView1.Items[i].SubItems[2].Text.Trim());
                        //CodeO = 0101023003001;


                    ISSparts issp = new ISSparts(
                        ISSid,
                        long.Parse(listView1.Items[i].SubItems[0].Text.Trim()),
                        CodeO, 
                        listView1.Items[i].SubItems[3].Text, 
                        listView1.Items[i].SubItems[4].Text, 
                        newSendPart, 
                        listView1.Items[i].SubItems[10].Text, 
                        listView1.Items[i].SubItems[11].Text, 
                        listView1.Items[i].SubItems[9].Text);

                    listIssPartsOld.Add(issp);

                    data = data + listView1.Items[i].SubItems[0].Text + ". / " + listView1.Items[i].SubItems[1].Text + " / " + listView1.Items[i].SubItems[2].Text + " / " + listView1.Items[i].SubItems[3].Text + " / " +
                        listView1.Items[i].SubItems[4].Text + " / " + listView1.Items[i].SubItems[5].Text + " / " + listView1.Items[i].SubItems[6].Text + " / " + listView1.Items[i].SubItems[7].Text + " / " +
                        listView1.Items[i].SubItems[8].Text + "h / " + listView1.Items[i].SubItems[9].Text + " / " + listView1.Items[i].SubItems[10].Text + Environment.NewLine;
                }

                cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));

                if ( qc.ISSUnesiISS(issExist, allDone, ISSid, _date, cmpCust, mainPart, listIssPartsOld) )
                {
                    Result = "ISS saved with ID " + ISSid;
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Result = "ISS not saved";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show("Error" + Environment.NewLine + Environment.NewLine + e1.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //TODO 
            //UBACITI POVIJEST LOG
            //Ponistiti sve upise
        }

        private void ISSSelectorCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            long issID = long.Parse(ISSSelectorCb.SelectedItem.ToString());
            String date = "";
            long userID = 0;
            long customerID = 0;
            long mainPartID = 0;

            List<ISSparts> iSSparts = new List<ISSparts>();

            iSSparts = qc.GetAllISSPartsByISSid(issID);

            //TODO ubaciti vrijednosti u polja
        }
    }
}