using POT.MyTypes;
using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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

        static long IISIDforThread = 0;

        DateTime startDate = DateTime.Now;

        Boolean timerPaused = false;
        Boolean timerStarted = false;
        Boolean timerEnabled = true;
        int stopClicked = 0;

        List<ISSreport> ISSall = new List<ISSreport>();
        ISSreport tmpISS = new ISSreport();

        QueryCommands qc = new QueryCommands();
        public static List<Part> partList = new List<Part>();
        List<PartSifrarnik> allParts = new List<PartSifrarnik>();
        List<String> sifrarnikArr = new List<String>();
        List<String> workDone = new List<String>();
        Boolean dataLoaded = false;
        List<List<Part>> groupedGoodPartsCode = new List<List<Part>>();

        static long ISSPartsCounter = 0;
        static Boolean isSaved = false;

        Part newSendPart = new Part();
        List<Part> newSendPartList = new List<Part>();
        Part mainPart = new Part();

        Company cmpCust = new Company();
        Company cmpM = new Company();

        int uvecajGroupPart = 0;
        Boolean onlyOneTime = true;
        Boolean allDonePrint = false;

        int obrJed = Properties.Settings.Default.ObracunskaJedinica;
        String totalTime;

        int partIndex = -1; //sluzi za grupiranu listu podataka da izvucem podpodatak za po SN da dobijem cn


        public ISS()
        {
            InitializeComponent();
        }

        private void ISS_Load(object sender, EventArgs e)
        {
            try
            {
                ISSreport tmp = new ISSreport();

                ISSall = tmp.GetAllIIS();

                MainCmp mmtmp = new MainCmp();
                mmtmp.GetMainCmpInfoByID(Properties.Settings.Default.CmpID);
                cmpM = mmtmp.MainCmpToCompany();
                
                partList = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "sng");

                List<Part> goodParts = new List<Part>();
                goodParts = qc.ListPartsByRegionStateP(WorkingUser.RegionID, "g");

                allParts = qc.GetPartsAllSifrarnik();

                if (ISSall.Count > 0) //(partList.Count > 0)
                {
                    foreach(ISSreport iss in ISSall)
                    {
                        ISSSelectorCb.Items.Add(iss.ISSid);

                        PartSelectorCb.Items.Add(iss.MainPart.PartID.ToString() + " # " + iss.MainPart.SN.ToUpper().ToString() + " # " + iss.MainPart.CN.ToUpper().ToString() + " # " + iss.MainPart.CodePartFull.ToString());
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
                listView1.Columns.Add("User ID");

                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                STARTbt_Click(sender, e);
            }catch(Exception e1)
            {
                Program.LoadStop();
                MessageBox.Show(e1.Message);
            }
            finally
            {
                Program.LoadStop();
            }

            this.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            s++;
            if (s > 59)
                calculateTimeLong(s);
            this.TIMERtb.Text = String.Format("{0:00}", h) + ":" + String.Format("{0:00}", m) + ":" + String.Format("{0:00}", s);
        }

        public void AppendTextBox(String value)
        {
            this.textBox1.AppendText(value + System.Environment.NewLine);
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
            if (WorkDoneCb.Text == "")
            {
                MessageBox.Show("Please fill in what is done!", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                        timerEnabled = false;
                        break;
                    case 1:
                        stopClicked = 0;
                        timerStarted = false;
                        timerEnabled = false;
                        STOPbt.Text = "STOP";
                        PAUSEbt.BackColor = Color.AliceBlue;
                        STARTbt.BackColor = Color.AliceBlue;
                        STOPbt.BackColor = Color.AliceBlue;
                        if (mainPart != null)
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
                        String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                        String usedQC = "Loading sifrarnik";
                        String data = "Break limit reached, arr cnt = " + tresultArr.Count;
                        String Result = "";
                        LogWriter lw = new LogWriter();

                        Result = "Cant load 'sifrarnik'.";
                        lw.LogMe(function, usedQC, data, Result);

                        pictureBox1.Image = Properties.Resources.LoadDataOff;

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
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.LoadDataOff;
                    dataLoaded = false;
                    STARTbt_Click(null, null);
                }
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                sifrarnikArr = tresultArr;
            }
        }

        private void CleanMe(object sender)
        {
            Button btn = null;
            Boolean btnYN = false;

            try
            {
                btn = (Button)sender;
            }
            catch { }

            if (btn != null && btn.Name.Equals("button2"))
            {
                DialogResult result = MessageBox.Show("Clear list and part also?", "ISS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    btnYN = true;
                else
                    btnYN = false;
            }
            if (btnYN || checkBox1.Checked == true)
            {
                checkBox1.Checked = false;
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

                for (int i = 0; i < listView1.Columns.Count; i++)
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
                ISSIDlb.Text = "0";
                PartSelectorCb.ResetText();
            }

            WorkDoneCb.ResetText();
            OldPartCodeTb.ResetText();
            OLDPartSNTb.ResetText();
            OldPartCNTb.ResetText();
            NewPartCodeTb.ResetText();
            WorkDoneCb.ResetText();
            ComentTb.ResetText();

            ISSSelectorCb.ResetText();
            PartSelectorCb.ResetText();

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

        private void OldPartCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OldPartCb.SelectedIndex > -1)
            {
                if (tmpISS.MainPart.CodePartFull == 0)
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
                    OldPartCodeTb.Text = Decoder.GetOwnerCode(tmpISS.MainPart.CodePartFull.ToString()) + Decoder.GetCustomerCode(tmpISS.MainPart.CodePartFull.ToString()) + Decoder.GetFullPartCodeStr(allParts[OldPartCb.SelectedIndex].FullCode); 
                }
            }
        }

        private void NewPartCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NewPartCb.SelectedIndex > -1)
            {
                partIndex = NewPartCb.SelectedIndex;

                NewPartCodeTb.Text = groupedGoodPartsCode[partIndex][0].CodePartFull.ToString();

                newSendPart = groupedGoodPartsCode[partIndex][0 + uvecajGroupPart];

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
            if (partIndex < 0)
                return;

            int snIndex = groupedGoodPartsCode[partIndex].FindIndex(x => x.SN == NewPartSNCb.Text);
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
                    NewPartCNTb.Text = groupedGoodPartsCode[partIndex][snIndex].CN.ToString();
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
                String partCode = tmpISS.MainPart.CodePartFull.ToString().Trim();// PartCbTT.Text.Trim();
                String CodeO = OldPartCodeTb.Text.Trim();
                String SNO = OLDPartSNTb.Text.Trim().ToUpper();
                String CNO = OldPartCNTb.Text.Trim().ToUpper();
                String CodeN = NewPartCodeTb.Text.Trim();
                String SNN = NewPartSNCb.Text.Trim().ToUpper();
                String wuID = WorkingUser.UserID.ToString();

                String CNN = NewPartCNTb.Text.Trim().ToUpper();

                DateConverter dt = new DateConverter();
                String date = dt.ConvertDDMMYY(DateTime.Now.ToString());
                
                if (CodeN.Equals(""))
                {
                    newSendPartList.Add(new Part());
                }
                else
                {
                    if (!newSendPartList.Contains(newSendPart))
                    {
                        newSendPartList.Add(newSendPart);

                        int indexRemove = groupedGoodPartsCode.FindIndex(x => x.Exists(y => y.PartID == newSendPart.PartID));
                        groupedGoodPartsCode[indexRemove].Remove(newSendPart);
                        //MessageBox.Show("Obrisano: " + groupedGoodPartsCode[indexRemove].Remove(newSendPart));
                        uvecajGroupPart++;
                    }
                    else
                    {
                        int i;
                        for (i = 0; i < groupedGoodPartsCode[partIndex].Count; i++)
                        {
                            if (!newSendPartList.Contains(groupedGoodPartsCode[partIndex][i]))
                            {
                                newSendPartList.Add(groupedGoodPartsCode[partIndex][i]);
                                uvecajGroupPart++;
                                groupedGoodPartsCode.RemoveAt(partIndex);
                                break;
                            }
                        }

                        if (i >= groupedGoodPartsCode[partIndex].Count)
                        {
                            data = "ISSid - " + tmpISS.ISSid.ToString();
                            Result = "There is a error with adding part, I cant find requested part" + Environment.NewLine + "Please contact your administrator.";
                            lw.LogMe(function, usedQC, data, Result);
                            MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (s >= 0)
                {
                    s = 0;
                    m++;
                }
                if (m >= 60)
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
                lvi1.SubItems.Add(wuID);

                if (listView1.Items.Count > 1)
                    listView1.EnsureVisible(listView1.Items.Count - 1);

                listView1.Items.Add(lvi1);

                tmpISS.ListIssParts.Add(AddNewChangedPartTotmpISS(lvi1, newSendPart));

                rb = listView1.Items.Count + 1;

                data = name + ", " + CodeO + ", " + SNO + ", " + CNO + ", " + CodeN + ", " + SNN + ", " + CNN + ", " + date + ", " + time + ", " + work + ", " + koment + ", " + wuID;

                for (int i = 0; i < listView1.Columns.Count; i++)
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

                isSaved = false;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message);
            }
        }

        private String calculateTime(List<ISSparts> listIssParts)
        {
            int hh = 0;
            int mm = 0;

            for(int i = 0; i < listIssParts.Count(); i++)
            {
                hh += int.Parse(listIssParts[i].Time.Split(':')[0]);
                mm += int.Parse(listIssParts[i].Time.Split(':')[1]);
            }

            hh += mm / 60;
            mm = mm % 60;

            return String.Format("{0:00}:{1:00}:{2:00}", hh, mm, 0);
        }

        private void calculateTimeLong(long pS)
        {
            if (pS % 60 == 0)
            {
                s = 0;
                m++;
                if (m % 60 == 0)
                {
                    m = 0;
                    h++;
                }
            }
        }

        private void ComentTb_TextChanged(object sender, EventArgs e)
        {
            label17.Text = "Letters left " + (200 - ComentTb.TextLength).ToString();
        }

        private void SAVEbt_Click(object sender, EventArgs e)
        {
            isSaved = false;

            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "Save to db";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            Boolean allDone = checkBox1.Checked;

            if (!checkBox1.Checked)
            {
                DialogResult result = MessageBox.Show("Close ISS?", "ISS", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    checkBox1.Checked = true;
                    allDone = true;
                }
                else
                {
                    checkBox1.Checked = false;
                    allDone = false;
                }
            }

            this.Refresh();

            //DateConverter dt = new DateConverter();
            //String _date = dt.ConvertDDMMYY(DateTime.Now.ToString());
            String _date = DateTime.Now.ToString("dd.MM.yy.");
            Boolean issExist = false;

            try
            {
                long testMe = qc.ISSExistIfNotReturnNewID(tmpISS == null ? 0 : tmpISS.ISSid);

                List<long> partsIDpokupi = new List<long>();
                if (tmpISS != null && tmpISS.ISSid == testMe)
                {
                    foreach(ISSparts part in tmpISS.ListIssParts)
                    {
                        if (part.PrtO.PartID != 0)
                            partsIDpokupi.Add(part.PrtO.PartID);
                    }

                    issExist = true;
                }
                else
                {
                    tmpISS = null;
                    tmpISS = new ISSreport();
                    tmpISS.ISSid = testMe;
                }

                tmpISS.ListIssParts.Clear();

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    tmpISS.ListIssParts.Add(AddNewChangedPartTotmpISS(listView1.Items[i], newSendPartList[i]));

                    data = data + listView1.Items[i].SubItems[0].Text + ". / " + listView1.Items[i].SubItems[1].Text + " / " + listView1.Items[i].SubItems[2].Text + " / " + listView1.Items[i].SubItems[3].Text + " / " +
                        listView1.Items[i].SubItems[4].Text + " / " + listView1.Items[i].SubItems[5].Text + " / " + listView1.Items[i].SubItems[6].Text + " / " + listView1.Items[i].SubItems[7].Text + " / " +
                        listView1.Items[i].SubItems[8].Text + "h / " + listView1.Items[i].SubItems[9].Text + " / " + listView1.Items[i].SubItems[10].Text + Environment.NewLine;
                }

                if (issExist)
                    cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(tmpISS.MainPart.CodePartFull));
                else
                {
                    cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));
                    tmpISS.CustomerID = cmpCust.ID;
                    tmpISS.Date = DateTime.Now.ToString("dd.mm.yy.");
                    tmpISS.MainPart = mainPart;
                    tmpISS.PartID = mainPart.PartID;
                    tmpISS.UserIDmaked = WorkingUser.UserID;
                }
                

                tmpISS.TotalTIme = totalTime = calculateTime(tmpISS.ListIssParts).Substring(0, 5);

                if (checkBox1.Checked)
                {
                    long th = 0;
                    long tm = 0;

                    th = int.Parse(totalTime.Split(':')[0]);
                    tm = int.Parse(totalTime.Split(':')[1]);

                    tm = tm < obrJed ? tm = obrJed : tm = (int)(tm / obrJed) * obrJed + obrJed;
                    if (tm >= 60)
                    {
                        tm = 0;
                        th++;
                    }

                    totalTime = String.Format("{0:00}:{1:00}", th, tm);
                }

                long ISSidPL = tmpISS.ISSid;

                //da mi ne upisuje nule ako je vec snimljeno
                if( issExist && partsIDpokupi.Count > 0 )
                {
                    for(int i = 0; i < partsIDpokupi.Count; i++)
                    {
                        tmpISS.ListIssParts[i].PrtO.PartID = partsIDpokupi[i];
                    }
                }

                allDonePrint = allDone;

                if (qc.ISSUnesiISS(issExist, allDone, tmpISS.ISSid, _date, cmpCust, mainPart, tmpISS.ListIssParts, WorkingUser.UserID, totalTime))
                {
                    isSaved = true;

                    if (Program.SaveDocumentsPDF) saveToPDF(); 

                    if (allDone)
                    {
                        PovijestLog pl = new PovijestLog();

                        Boolean plGreska = false;

                        for (int i = 0; i < tmpISS.ListIssParts.Count; i++)
                        {
                            List<Part> plParts = new List<Part>();
                            plParts.Add(tmpISS.ListIssParts[i].PrtN);
                            plParts.Add(tmpISS.ListIssParts[i].PrtO);

                            String tekst = tmpISS.ListIssParts[i].Comment;
                            plGreska = !pl.SaveToPovijestLog(plParts, tmpISS.ListIssParts[i].Date, tmpISS.ListIssParts[i].Work + (tekst.Equals("") ? "" : " - " + tekst), cmpCust.Name, "", "", "ISS " + ISSidPL.ToString(), "wsg");
                        }

                        if(plGreska)
                        {
                            Result = "Povijest log is not saved.";
                            lw.LogMe(function, usedQC, data, Result);
                            MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                            doNotRepeatMsg = 1;
                        }

                        ISSall.Remove(tmpISS);

                        partList.Remove(mainPart);
                    }
                    else
                    {
                        if (!ISSall.Exists(x => x.ISSid == tmpISS.ISSid))
                            ISSall.Add(tmpISS);
                    }

                    ISSIDlb.Text = tmpISS.ISSid.ToString();

                    Result = "ISS saved with ID " + tmpISS.ISSid;
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (!ISSall.Exists(x => x.ISSid == tmpISS.ISSid))
                        partList.Remove(mainPart);

                    ISSSelectorCb.Items.Clear();
                    PartSelectorCb.Items.Clear();

                    if (ISSall.Count > 0) //(partList.Count > 0)
                    {
                        foreach (ISSreport iss in ISSall)
                        {
                            ISSSelectorCb.Items.Add(iss.ISSid);

                            PartSelectorCb.Items.Add(iss.MainPart.PartID.ToString() + " # " + iss.MainPart.SN.ToUpper().ToString() + " # " + iss.MainPart.CN.ToUpper().ToString());
                        }
                    }

                    ISSSelectorCb.SelectedIndex = ISSSelectorCb.FindStringExact(tmpISS.ISSid.ToString());
                    ISSSelectorCb.Text = tmpISS.ISSid.ToString();

                    if (checkBox1.Checked)
                        CleanMe(null);
                }
                else
                {
                    isSaved = false;
                    Result = "ISS not saved";
                    lw.LogMe(function, usedQC, data, Result);
                    MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                doNotRepeatMsg = 1;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show("Error" + Environment.NewLine + Environment.NewLine + e1.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isSaved = false;
            }
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

            tmpISS = null;

            try
            {
                Program.LoadStart();

                CleanMe(null);
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
                listView1.Columns.Add("UserID");

                totalTime = "00:00";

                long issID = long.Parse(ISSSelectorCb.SelectedItem.ToString());

                tmpISS = ISSall.Find(x => x.ISSid == issID);

                ISSIDlb.Text = issID.ToString();

                totalTime = tmpISS.TotalTIme + ":00";

                mainPart = tmpISS.MainPart;

                SNTb.Text = tmpISS.MainPart.SN;
                CNTb.Text = tmpISS.MainPart.CN;
                DateInTb.Text = tmpISS.MainPart.DateIn;
                DateSentTb.Text = tmpISS.MainPart.DateSend;
                IDTb.Text = tmpISS.MainPart.PartID.ToString();
                NameTb.Text = Decoder.ConnectCodeName(sifrarnikArr, tmpISS.MainPart.CodePartFull);

                cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(tmpISS.MainPart.CodePartFull));

                newSendPartList.Clear();

                for (int i = 0; i < tmpISS.ListIssParts.Count; i++)
                {
                    newSendPartList.Add(tmpISS.ListIssParts[i].PrtN);

                    ListViewItem lvi1 = new ListViewItem(tmpISS.ListIssParts[i].RB.ToString().ToString());

                    lvi1.SubItems.Add(qc.PartInfoByFullCodeSifrarnik(tmpISS.MainPart.PartialCode).FullName);

                    //////////////////////////////////////////////////////////////

                    if (tmpISS.ListIssParts[i].PrtO.CodePartFull == 0)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtO.CodePartFull.ToString());

                    if (tmpISS.ListIssParts[i].PrtO.SN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtO.SN.ToString());

                    if (tmpISS.ListIssParts[i].PrtO.CN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtO.CN.ToString());

                    //////////////////////////////////////////////////////////////

                    if (tmpISS.ListIssParts[i].PrtN.CodePartFull == 0)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtN.CodePartFull.ToString());

                    if (tmpISS.ListIssParts[i].PrtN.SN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtN.SN.ToString());

                    if (tmpISS.ListIssParts[i].PrtN.CN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtN.CN.ToString());

                    //////////////////////////////////////////////////////////////

                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Date.ToString());

                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Time.ToString());
                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Work.ToString());
                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Comment.ToString());
                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].UserID.ToString());

                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);

                    listView1.Items.Add(lvi1);
                }

                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                rb = listView1.Items.Count + 1;

                ISSPartsCounter = tmpISS.ListIssParts.Count();

                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + tmpISS.ListIssParts.Count + ", " + tmpISS.ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "ISS selected " + tmpISS.ISSid;
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                lw.LogMe(function, usedQC, data, Result);

                Program.LoadStop();
                this.Focus();
            }
            catch(Exception e1)
            {
                data = tmpISS.ISSid + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);

                Program.LoadStop();
                this.Focus();

                MessageBox.Show(Result, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Program.LoadStop();
                this.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CleanMe(sender);
        }
        //TODO eventualno popraviti
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
                        if (list.Count() < 2)
                        {
                            list = TIMERtb.Text.Split(',');
                            list = TIMERtb.Text.Split('.');
                        }
                        h = 0;
                        m = 0;
                        s = 0;

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
                SaveFileDialog pdfSaveDialog = new SaveFileDialog();

                if (printDialog1.PrinterSettings.PrinterName == "Microsoft Print to PDF")
                {   // force a reasonable filename
                    string basename = Path.GetFileNameWithoutExtension("ISS " + tmpISS.ISSid.ToString());
                    string directory = Path.GetDirectoryName("ISS " + tmpISS.ISSid.ToString());
                    printDocument1.PrinterSettings.PrintToFile = true;
                    // confirm the user wants to use that name
                    pdfSaveDialog.InitialDirectory = directory;
                    pdfSaveDialog.FileName = basename + ".pdf";
                    pdfSaveDialog.Filter = "PDF File|*.pdf";
                    result = pdfSaveDialog.ShowDialog();
                    if (result != DialogResult.Cancel)
                        printDocument1.PrinterSettings.PrintFileName = pdfSaveDialog.FileName;
                }

                if (result != DialogResult.Cancel)  // in case they canceled the save as dialog
                {
                    printDocument1.Print();
                    MessageBox.Show("Saved to location: " + Environment.NewLine + pdfSaveDialog.FileName, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    PRINTbt_Click(sender, e);
                }
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
            printPreviewDialog1.Size = new Size(screenWidth - ((screenWidth / 100) * 60), screenHeight - (screenHeight / 100) * 10);
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
            
            PrintMeISS pr = new PrintMeISS(cmpCust, cmpM, sifrarnikArr, mainPart, tmpISS.ListIssParts, tmpISS.ISSid.ToString(), Properties.strings.ServiceReport, Properties.strings.customer, false, "", totalTime, allDonePrint);
            pr.Print(e);

            if (onlyOneTime)
            {
                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + tmpISS.ListIssParts.Count + ", " + tmpISS.ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
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
            int mRB;
            
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                mRB = int.Parse(item.SubItems[0].Text);

                if ( !isSaved && mRB <= ISSPartsCounter )
                {
                    MessageBox.Show("I cant delete the item that have already been recorded!" + Environment.NewLine + "Please contact you administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                else if ( isSaved )
                {
                    MessageBox.Show("Sorry I can check if part can be removed." + Environment.NewLine + "I did not load parts correctly." + Environment.NewLine + "Please try again later or try to reload ISS, if the problem is persistent, contact you administrator.", "Parts error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                
                tmpISS.ListIssParts.RemoveAt(mRB - 1);
                newSendPartList.RemoveAt(mRB - 1);
                uvecajGroupPart--;
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
                //return;

                var items = listView1.SelectedItems;

                long issID = long.Parse(tmpISS.ISSid.ToString());
                Part mainPr = new Part();
                List<String> allISSInfo = new List<String>();

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
                
                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + tmpISS.ListIssParts.Count + ", " + tmpISS.ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "ISS selected " + tmpISS.ISSid;
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                lw.LogMe(function, usedQC, data, Result);
            }
            catch (Exception e1)
            {
                data = tmpISS.ISSid + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);
                MessageBox.Show(Result, "NOT SAVED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void saveToPDF()
        {
            String printerName = printDialog1.PrinterSettings.PrinterName;

            try
            {
                PrintDialog printDialog1 = new PrintDialog();
                printDialog1.Document = printDocument1;

                printDialog1.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                if (!printDialog1.PrinterSettings.IsValid) return;

                if (!Directory.Exists(Properties.Settings.Default.DefaultFolder + "\\ISS"))
                    return;

                string fileName = "\\ISS " + tmpISS.ISSid.ToString().Replace("/", "") + ".pdf";
                string directory = Properties.Settings.Default.DefaultFolder + "\\ISS";

                printDialog1.PrinterSettings.PrintToFile = true;
                printDocument1.PrinterSettings.PrintFileName = directory + fileName;
                printDocument1.PrinterSettings.PrintToFile = true;
                printDocument1.Print();

                printDialog1.PrinterSettings.PrintToFile = false;
                printDocument1.PrinterSettings.PrintToFile = false;
                printDialog1.PrinterSettings.PrinterName = printerName;
                printDocument1.PrinterSettings.PrinterName = printerName;
            }
            catch (Exception e1)
            {
                new LogWriter(e1);
                MessageBox.Show(e1.Message + Environment.NewLine + "PDF file not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PartSelectorCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///////////////// LogMe ////////////////////////
            String function = this.GetType().FullName + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name;
            String usedQC = "ISS part loaded";
            String data = "";
            String Result = "";
            LogWriter lw = new LogWriter();
            ////////////////////////////////////////////////
            ///

            try
            {
                Program.LoadStart();

                CleanMe(null);
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
                listView1.Columns.Add("UserID");

                totalTime = "00:00";

                String splitMe = PartSelectorCb.SelectedItem.ToString();
                var partData = splitMe.Split('#');

                tmpISS = null;
                tmpISS = ISSall.Find(x => x.MainPart.PartID == long.Parse(partData[0].Trim()));

                long issID = tmpISS.ISSid;

                if (issID == 0)
                {
                    if (PartSelectorCb.SelectedIndex < 0)
                    {
                        return;
                    }
                    else
                    {
                        ISSIDlb.Text = "0";

                        NameTb.Text = Decoder.ConnectCodeName(sifrarnikArr, tmpISS.MainPart.PartialCode);

                        SNTb.Text = tmpISS.MainPart.SN;
                        CNTb.Text = tmpISS.MainPart.CN;
                        DateInTb.Text = tmpISS.MainPart.DateIn;
                        DateSentTb.Text = tmpISS.MainPart.DateSend;
                        IDTb.Text = tmpISS.MainPart.PartID.ToString();
                        mainPart = tmpISS.MainPart;
                    }
                }

                ISSIDlb.Text = issID.ToString();

                if (tmpISS == null)
                {
                    data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + tmpISS.ListIssParts.Count + ", " + tmpISS.ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                    Result = "No ISS part selected " + partData[0];
                    AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                    lw.LogMe(function, usedQC, data, Result);
                    return;
                }

                totalTime = tmpISS.TotalTIme + ":00";

                mainPart = tmpISS.MainPart;

                SNTb.Text = tmpISS.MainPart.SN;
                CNTb.Text = tmpISS.MainPart.CN;
                DateInTb.Text = tmpISS.MainPart.DateIn;
                DateSentTb.Text = tmpISS.MainPart.DateSend;
                IDTb.Text = tmpISS.MainPart.PartID.ToString();
                NameTb.Text = Decoder.ConnectCodeName(sifrarnikArr, tmpISS.MainPart.CodePartFull);

                cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(tmpISS.MainPart.CodePartFull));

                newSendPartList.Clear();

                for (int i = 0; i < tmpISS.ListIssParts.Count; i++)
                {
                    newSendPartList.Add(tmpISS.ListIssParts[i].PrtN);

                    ListViewItem lvi1 = new ListViewItem(tmpISS.ListIssParts[i].RB.ToString().ToString());

                    lvi1.SubItems.Add(qc.PartInfoByFullCodeSifrarnik(tmpISS.MainPart.PartialCode).FullName);

                    //////////////////////////////////////////////////////////////

                    if (tmpISS.ListIssParts[i].PrtO.CodePartFull == 0)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtO.CodePartFull.ToString());

                    if (tmpISS.ListIssParts[i].PrtO.SN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtO.SN.ToString());

                    if (tmpISS.ListIssParts[i].PrtO.CN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtO.CN.ToString());

                    //////////////////////////////////////////////////////////////

                    if (tmpISS.ListIssParts[i].PrtN.CodePartFull == 0)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtN.CodePartFull.ToString());

                    if (tmpISS.ListIssParts[i].PrtN.SN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtN.SN.ToString());

                    if (tmpISS.ListIssParts[i].PrtN.CN == null)
                        lvi1.SubItems.Add("");
                    else
                        lvi1.SubItems.Add(tmpISS.ListIssParts[i].PrtN.CN.ToString());

                    //////////////////////////////////////////////////////////////

                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Date.ToString());

                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Time.ToString());
                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Work.ToString());
                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].Comment.ToString());
                    lvi1.SubItems.Add(tmpISS.ListIssParts[i].UserID.ToString());

                    if (listView1.Items.Count > 1)
                        listView1.EnsureVisible(listView1.Items.Count - 1);

                    listView1.Items.Add(lvi1);
                }

                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                    listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                rb = listView1.Items.Count + 1;

                ISSPartsCounter = tmpISS.ListIssParts.Count();

                data = cmpCust.Name + ", " + cmpM.Name + ", " + "Sifrarnik arr cnt " + sifrarnikArr.Count + ", " + mainPart.CodePartFull + ", " + "listIssParts cnt " + tmpISS.ListIssParts.Count + ", " + tmpISS.ISSid.ToString() + ", " + Properties.strings.ServiceReport + ", " + Properties.strings.customer + ", false";
                Result = "ISS part selected " + partData[0];
                AppendTextBox(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + Result);
                lw.LogMe(function, usedQC, data, Result);

                Program.LoadStop();
                this.Focus();
            }
            catch (Exception e1)
            {
                data = tmpISS.ISSid + Environment.NewLine;
                Result = e1.Message;
                lw.LogMe(function, usedQC, data, Result);

                MessageBox.Show(Result, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Program.LoadStop();
                this.Focus();
            }
        }

        private void NewISSBT_Click(object sender, EventArgs e)
        {
            try
            {
                ISSSelector issSelector = new ISSSelector();
                issSelector.ShowDialog();

                if (ISSSelector.selectedIndex < 0)
                {
                    if (ISSall.Count > 0) //(partList.Count > 0)
                    {
                        ISSSelectorCb.Text = "";
                        PartSelectorCb.Text = "";

                        ISSSelectorCb.Items.Clear();
                        PartSelectorCb.Items.Clear();
                        listView1.Items.Clear();

                        foreach (ISSreport iss in ISSall)
                        {
                            ISSSelectorCb.Items.Add(iss.ISSid);

                            PartSelectorCb.Items.Add(iss.MainPart.PartID.ToString() + " # " + iss.MainPart.SN.ToUpper().ToString() + " # " + iss.MainPart.CN.ToUpper().ToString() + " # " + iss.MainPart.CodePartFull.ToString());
                        }
                    }
                    return;
                }
                mainPart = null;
                tmpISS = null;

                FillTmpISSIManiPart(ISSSelector.selectedIndex);

                ISSSelectorCb.Text = "";
                PartSelectorCb.Text = "";

                ISSSelectorCb.Items.Clear();
                PartSelectorCb.Items.Clear();
                listView1.Items.Clear();


                Boolean allDone = checkBox1.Checked;

                if (qc.ISSUnesiISS(false, allDone, tmpISS.ISSid, tmpISS.Date, cmpCust, mainPart, tmpISS.ListIssParts, WorkingUser.UserID, totalTime))
                {
                    if (Program.SaveDocumentsPDF) saveToPDF();
                }

                qc.ISSPrebaciPartUWorking(tmpISS.MainPart);

                partList.RemoveAt(ISSSelector.selectedIndex);

                ISSall.Add(tmpISS);
                if (ISSall.Count > 0) //(partList.Count > 0)
                {
                    foreach (ISSreport iss in ISSall)
                    {
                        ISSSelectorCb.Items.Add(iss.ISSid);

                        PartSelectorCb.Items.Add(iss.MainPart.PartID.ToString() + " # " + iss.MainPart.SN.ToUpper().ToString() + " # " + iss.MainPart.CN.ToUpper().ToString() + " # " + iss.MainPart.CodePartFull.ToString());
                    }
                }

                MessageBox.Show("New ISS is opened, with number, " + tmpISS.ISSid.ToString());

                mainPart = null;
                tmpISS = null;
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }

        private void FillTmpISSIManiPart(int _selectedIndex)
        {
            mainPart = partList[_selectedIndex];

            tmpISS = new ISSreport();
            tmpISS.ISSid = qc.ISSExistIfNotReturnNewID(0);

            cmpCust.GetCompanyInfoByCode(Decoder.GetCustomerCode(mainPart.CodePartFull));
            tmpISS.CustomerID = cmpCust.ID;
            tmpISS.Date = DateTime.Now.ToString("dd.mm.yy.");
            tmpISS.MainPart = mainPart;
            tmpISS.PartID = mainPart.PartID;
            tmpISS.UserIDmaked = WorkingUser.UserID;

            tmpISS.TotalTIme = totalTime = "00:00";
        }

        private ISSparts AddNewChangedPartTotmpISS(ListViewItem _item, Part _part)
        {
            try
            {
                long CodeO = 0;

                if (!_item.SubItems[2].Text.Trim().Equals(""))
                    CodeO = long.Parse(_item.SubItems[2].Text.Trim());

                ISSparts issp = new ISSparts(
                            tmpISS.ISSid,
                            long.Parse(_item.SubItems[0].Text.Trim()),
                            CodeO,
                            _item.SubItems[3].Text,
                            _item.SubItems[4].Text,
                            _part,
                            _item.SubItems[10].Text,
                            _item.SubItems[11].Text,
                            _item.SubItems[9].Text,
                            long.Parse(_item.SubItems[12].Text),
                            _item.SubItems[8].Text);
                return issp;
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
                return null;
            }
            finally
            {
                //Program.LoadStop();
            }
        }
    }
}