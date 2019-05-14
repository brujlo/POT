using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
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
        Boolean timerEnabled = true;
        int stopClicked = 0;

        QueryCommands qc = new QueryCommands();
        List<Part> partList = new List<Part>();
        List<PartSifrarnik> allParts = new List<PartSifrarnik>();
        List<String> sifrarnikArr = new List<String>();
        List<String> workDone = new List<String>();
        Boolean dataLoaded = false;
        List<List<Part>> groupedGoodPartsCode = new List<List<Part>>();
        List<ISSparts> listIssParts = new List<ISSparts>();
        List<long> ISSids = new List<long>();
        Part newSendPart = new Part();
        Part mainPart = new Part();

        Company cmpCust = new Company();
        Company cmpM = new Company();
        MainCmp mm = new MainCmp();

        Boolean onlyOneTime = true;
        //Boolean itemRemoved = false;

        Boolean pictureOn = false;

        int partIndex = -1; //sluzi za grupiranu listu podataka da izvucem podpodatak za po SN da dobijem cn


        public ISS()
        {
            InitializeComponent();
        }

        private void ISS_Load(object sender, EventArgs e)
        {
            mm.GetMainCmpInfoByID(Properties.Settings.Default.CmpID);
            cmpM = mm.MainCmpToCompany();

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

            if (Properties.Settings.Default.LanguageStt.Equals("hrv"))
                workDone = qc.GetAllFromWorkHR();
            else if (Properties.Settings.Default.LanguageStt.Equals("eng"))
                workDone = qc.GetAllFromWorkENG();

            if (workDone.Count > 0)
            {
                for (int i = 0; i < workDone.Count(); i++)
                {
                    WorkDoneCb.Items.Add(workDone[i]);
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
            //TODO
            //PRINTbt.Enabled = false;
            //SelectPrinterbt.Enabled = false;

            STARTbt_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            s++;
            if (s > 59)
                calculateTime(s);
            this.TIMERtb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
        }

        public void AppendTextBox(String value)
        {
            this.textBox1.AppendText(value + System.Environment.NewLine);
            //new LogWriter(value);
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
                timerEnabled = true;
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
                        timerEnabled = false;
                        STOPbt.Text = "STOP";
                        PAUSEbt.BackColor = Color.AliceBlue;
                        STARTbt.BackColor = Color.AliceBlue;
                        STOPbt.BackColor = Color.AliceBlue;
                        if (!PartCb.Equals(""))
                            AddToWorkList();
                        h = 0; m = 0; s = 0;
                        this.TIMERtb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
                        break;
                }
            }
        }

        private void CANCELBt_Click(object sender, EventArgs e)
        {
            h = 0; m = 0; s = 0;
            this.TIMERtb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
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
                        //MessageBox.Show("Cant load 'sifrarnik'.");
                        String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        String usedQC = "Loading sifrarnik";
                        String data = "Break limit reached, arr cnt = " + tresultArr.Count;
                        String Result = "";
                        LogWriter lw = new LogWriter();

                        Result = "Cant load 'sifrarnik'.";
                        lw.LogMe(function, usedQC, data, Result);

                        pictureBox1.Image = Properties.Resources.LoadDataOff;
                        pictureOn = false;

                        this.Refresh();

                        break;
                    }
                }

                sifrarnikArr = tresultArr;

                if (sifrarnikArr.Count > 0 && stop < 100)
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOn;
                    dataLoaded = true;
                    STARTbt_Click(null, null);
                    pictureOn = true;
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOff;
                    dataLoaded = false;
                    STARTbt_Click(null, null);
                    pictureOn = false;
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }

        /*
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
        */

        private void CleanMe()
        {
            ISSid = 0;

            if(checkBox1.Checked == true)
            {
                checkBox1.Checked = false;
                PartCb.ResetText();
                listView1.Clear();
                
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

                NameTb.ResetText();
                SNTb.ResetText();
                CNTb.ResetText();
                DateInTb.ResetText();
                DateSentTb.ResetText();
                IDTb.ResetText();
            }

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
                if (!NewPartSNCb.Text.Equals("") && NewPartSNCb.SelectedIndex != -1)
                {
                    NewPartCNTb.Text = groupedGoodPartsCode[partIndex][0].CN.ToString();
                    newSendPart = groupedGoodPartsCode[partIndex][snIndex];
                }
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

            DialogResult result = MessageBox.Show("Close ISS?", "ISS", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
            //TODO
            //PRINTbt.Enabled = checkBox1.Checked;
            //SelectPrinterbt.Enabled = checkBox1.Checked;

            DateConverter dt = new DateConverter();
            String _date = dt.ConvertDDMMYY(DateTime.Now.ToString());
            Boolean issExist = false;
            Boolean allDone = checkBox1.Checked;

            listIssParts.Clear();

            try
            {
                long testMe = qc.ISSExistIfNotReturnNewID(ISSid);
                
                if (ISSid == testMe)
                {
                    issExist = true;
                }
                else
                {
                    ISSid = testMe;
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
                        listView1.Items[i].SubItems[9].Text,
                        WorkingUser.UserID);
                    
                    listIssParts.Add(issp);
                    
                    data = data + listView1.Items[i].SubItems[0].Text + ". / " + listView1.Items[i].SubItems[1].Text + " / " + listView1.Items[i].SubItems[2].Text + " / " + listView1.Items[i].SubItems[3].Text + " / " +
                        listView1.Items[i].SubItems[4].Text + " / " + listView1.Items[i].SubItems[5].Text + " / " + listView1.Items[i].SubItems[6].Text + " / " + listView1.Items[i].SubItems[7].Text + " / " +
                        listView1.Items[i].SubItems[8].Text + "h / " + listView1.Items[i].SubItems[9].Text + " / " + listView1.Items[i].SubItems[10].Text + Environment.NewLine;
                }

                cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));

                /*
                if (itemRemoved)
                {
                    if (!qc.RemoveAllISSParts(ISSid))
                        return;
                    itemRemoved = false;
                }*/

                if ( qc.ISSUnesiISS(issExist, allDone, ISSid, _date, cmpCust, mainPart, listIssParts, WorkingUser.UserID) )
                {
                    Result = "ISS saved with ID " + ISSid;
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ISSSelectorCb.Items.Clear();
                    ISSSelectorCb.ResetText();

                    ISSids = qc.GetAllISSOpenClose(0);

                    for (int i = 0; i < ISSids.Count; i++)
                    {
                        ISSSelectorCb.Items.Add(ISSids[i]);
                    }

                    ISSid = 0;
                }
                else
                {
                    Result = "ISS not saved";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                doNotRepeatMsg = 1;

                PovijestLog pl = new PovijestLog();
                List<Part> plParts = new List<Part>();

                plParts.Add(listIssParts[0].PrtN);
                plParts.Add(listIssParts[0].PrtO);

                if (!pl.SaveToPovijestLog(plParts, DateTime.Now.ToString("dd.MM.yy."), WorkDoneCb.Text, cmpCust.Name, "", "", ISSid.ToString(), "gng"))
                {
                    Result = "Povijest log is not saved.";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                    doNotRepeatMsg = 1;
                }

            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show("Error" + Environment.NewLine + Environment.NewLine + e1.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            CleanMe();
        }

        private void ISSSelectorCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "ISS loaded";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                CleanMe();
                listView1.Clear();

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

                long issID = long.Parse(ISSSelectorCb.SelectedItem.ToString());
                Part mainPr = new Part();
                List<String> allISSInfo = new List<String>();

                ISSid = issID;

                allISSInfo = qc.GetAllISSInfoById(issID);
            
                if (allISSInfo[0].Equals("nok"))
                    return;

                mainPart = qc.SearchPartsInAllTablesBYPartID(long.Parse(allISSInfo[4]))[0];

                PartCb.Text = mainPart.CodePartFull.ToString();

                cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));

                listIssParts.Clear();
                listIssParts = qc.GetAllISSPartsByISSid(issID);

                for (int i = 0; i < listIssParts.Count; i++)
                {
                    ListViewItem lvi1 = new ListViewItem(listIssParts[i].RB.ToString().ToString());

                    lvi1.SubItems.Add(qc.PartInfoByFullCodeSifrarnik(mainPart.PartialCode).FullName);

                    //////////////////////////////////////////////////////////////
                
                    if (listIssParts[i].PrtO.CodePartFull == 0)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(listIssParts[i].PrtO.CodePartFull.ToString());

                    if (listIssParts[i].PrtO.SN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(listIssParts[i].PrtO.SN.ToString());

                    if (listIssParts[i].PrtO.CN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(listIssParts[i].PrtO.CN.ToString());

                    //////////////////////////////////////////////////////////////
                
                    if (listIssParts[i].PrtN.CodePartFull == 0)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(listIssParts[i].PrtN.CodePartFull.ToString());

                    if (listIssParts[i].PrtN.SN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(listIssParts[i].PrtN.SN.ToString());

                    if (listIssParts[i].PrtN.CN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(listIssParts[i].PrtN.CN.ToString());

                    //////////////////////////////////////////////////////////////

                    lvi1.SubItems.Add(allISSInfo[1]);

                    lvi1.SubItems.Add(listIssParts[i].Time.ToString());
                    lvi1.SubItems.Add(listIssParts[i].Work.ToString());
                    lvi1.SubItems.Add(listIssParts[i].Comment.ToString());

                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);

                    listView1.Items.Add(lvi1);
                }

                for (int i = 0; i < 12; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                rb = listView1.Items.Count + 1;

                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + listIssParts.Count + ", " + ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "ISS selected " + ISSid;
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                lw.LogMe(function, usedQC, data, Result);
            }
            catch(Exception e1)
            {
                data = ISSid + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CleanMe();
        }
        //TODO popraviti
        private void button3_Click(object sender, EventArgs e)
        {
            if (!timerEnabled)
            {
                if(TIMERtb.ReadOnly == false)
                {
                    TIMERtb.ReadOnly = true;
                    SAVEbt.Enabled = true;
                    button3.BackColor = Color.AliceBlue;

                    try
                    {
                        var list = TIMERtb.Text.Split(':');
                        int h = 0;
                        int m = 0;
                        int s = 0;

                        switch (list.Count())
                        {
                            case 1:
                                m = int.Parse(list[0]);
                                break;
                            case 2:
                                h = int.Parse(list[0]);
                                m = int.Parse(list[1]);
                                break;
                           default:
                                h = int.Parse(list[0]);
                                m = int.Parse(list[1]);
                                s = int.Parse(list[2]);
                                break;
                        }

                        TIMERtb.Text = String.Format("{0:00}:{1:00}:{2:00}", h, m, s);
                    }
                    catch 
                    {
                        TIMERtb.Text = "00:00:00";
                    }

                }
                else
                {
                    TIMERtb.ReadOnly = false;
                    SAVEbt.Enabled = false;
                    button3.BackColor = Color.Yellow;
                }
            }
        }

        private void SelectPrinterbt_Click(object sender, EventArgs e)
        {
            onlyOneTime = true;

            PrintDialog printDialog1 = new PrintDialog();
            printDialog1.Document = printDocument1;
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                PRINTbt_Click(sender, e);
                //printDocumentPrim.Print();
            }
        }

        private void PRINTbt_Click(object sender, EventArgs e)
        {
            onlyOneTime = true;

            Properties.Settings.Default.pageNbr = 1;
            Properties.Settings.Default.partRows = 0;
            Properties.Settings.Default.printingSN = false;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.Size = new System.Drawing.Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
            printPreviewDialog1.ShowDialog();

            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Print";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            PrintMeISS pr = new PrintMeISS(cmpCust, cmpM, sifrarnikArr, mainPart, listIssParts, ISSid.ToString(), Properties.strings.ServiceReport, Properties.strings.customer, false, "");
            pr.Print(e);

            if (onlyOneTime)
            {
                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + listIssParts.Count + ", " + ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "Print page called";
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                lw.LogMe(function, usedQC, data, Result);

                onlyOneTime = false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Remove selected";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            String showItem = "";
            var items = listView1.SelectedItems;
            int k = 1;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
                showItem = item.SubItems[0].Text;
                if (data.Equals(""))
                    data = item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5];
                else
                    data = data + Environment.NewLine + "             " + item.SubItems[0] + ", " + item.SubItems[1] + ", " + item.SubItems[2] + ", " + item.SubItems[3] + ", " + item.SubItems[4] + ", " + item.SubItems[5];
            }

            foreach (ListViewItem item in listView1.Items)
            {
                if (listView1.SelectedItems != null && listView1.Items.Count != 0)
                {
                    item.SubItems[0].Text = k.ToString();
                    k++;
                }
            }

            Result = "Line " + showItem + " removed";
            lw.LogMe(function, usedQC, data, Result);
            AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);

            rb = listView1.Items.Count + 1;

            //itemRemoved = true;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "ISS loaded";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                var items = listView1.SelectedItems;

                long issID = long.Parse(ISSSelectorCb.SelectedItem.ToString());
                Part mainPr = new Part();
                List<String> allISSInfo = new List<String>();

                ISSid = issID;

                allISSInfo = qc.GetAllISSInfoById(issID);

                if (allISSInfo[0].Equals("nok"))
                    return;

                if (items[0].SubItems[2].Text.Equals(""))
                    OldPartCb.Text = "";
                else
                    OldPartCb.Text = sifrarnikArr.ElementAt(sifrarnikArr.IndexOf(Decoder.GetFullPartCodeStr(long.Parse(items[0].SubItems[2].Text))) - 1);

                OldPartCodeTb.Text = items[0].SubItems[2].Text;
                OLDPartSNTb.Text = items[0].SubItems[3].Text;
                OldPartCNTb.Text = items[0].SubItems[4].Text;

                if (items[0].SubItems[5].Text.Equals(""))
                    NewPartCb.Text = "";
                else
                    NewPartCb.Text = sifrarnikArr.ElementAt(sifrarnikArr.IndexOf(Decoder.GetFullPartCodeStr(long.Parse(items[0].SubItems[5].Text))) - 1);

                NewPartCodeTb.Text = items[0].SubItems[5].Text;
                NewPartSNCb.Text = items[0].SubItems[6].Text;
                NewPartCNTb.Text = items[0].SubItems[7].Text;

                WorkDoneCb.Text = items[0].SubItems[10].Text;
                ComentTb.Text = items[0].SubItems[11].Text;

                mainPart = qc.SearchPartsInAllTablesBYPartID(long.Parse(allISSInfo[4]))[0];

                PartCb.Text = mainPart.CodePartFull.ToString();

                cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));

                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + listIssParts.Count + ", " + ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "ISS selected " + ISSid;
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                lw.LogMe(function, usedQC, data, Result);
            }
            catch (Exception e1)
            {
                data = ISSid + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}