using POT.Documents;
using POT.MyTypes;
using System;
using System.Collections.Generic;

namespace POT.WorkingClasses
{
    class AddTIDDB
    {
        public Boolean sendIntervention(Company cmp, String datPrijave, String VriPrijave, String Prio, String slaDatum,
                String slaVrijeme, String Filijala, String CCN, String CID, long idNumber,
                String drive, String Prijavio, String NazivUredaja, String Opis, String storno, String grad, String adresa, String telefon, Tickets frm)
        {
            String posaljiPoruku;
            String naslov;
            String GoogleS;

            GoogleS = adresa + "+" + grad;
            GoogleS = GoogleS.Replace(".", "+");
            GoogleS = GoogleS.Replace(",", "+");
            GoogleS = GoogleS.Replace(" ", "+");

            if (adresa.Equals("")) adresa = "Unknown";
            if (grad.Equals("")) grad = "Unknown";
            if (telefon.Equals("")) telefon = "Unknown";

            if (storno.ToUpper().Equals("STORNO"))
            {
                naslov = "STORNO" + " CUST_" + cmp.Code + "_ID_" + idNumber + "_CCN_" + CCN + "_CID_" + CID + "_" + "F" + Filijala;
                
                posaljiPoruku = "<p style=''font-family:calibri;font-size:15''>" +
                    "<u><b>STORNO</b></u>" + "</p>" + "<br>" +
                    "<p style=''font-family:calibri;font-size:12''>" +
                    "ID: " + idNumber + "<br>" +
                    "Klijent: " + cmp.Name + "<br>" +
                    "Filijala: " + Filijala + "<br>" +
                    "CCN: " + CCN + "<br>" +
                    "CID: " + CID + "<br>" + "<br>" + "<br>" + "</p>" +
                    "<p style=''font-family:calibri;font-size:12''>"
                    + "--------------------------------------" + "<br>" +
                    setAutoFooter();

                QueryCommands qc = new QueryCommands();

                List<String> arr = new List<string>();
                arr = qc.SentToUsers();
                String sendList = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    sendList = sendList + arr[i++] + ";";
                }
                sendList = sendList.Trim();

                if (qc.SendTIDStorno(WorkingUser.Username, WorkingUser.Password, idNumber, naslov, posaljiPoruku, sendList))
                {
                    frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "TID " + idNumber + " STORNO sent at " + DateTime.Now);
                    return true;
                }
                else
                {
                    frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "TID " + idNumber + " STORNO NOT sent at " + DateTime.Now);
                    return false;
                }
            }
            else
            {
                naslov = "CUST_" + cmp.Code + "_ID_" + idNumber + "_CCN_" + CCN + "_CID_" + CID + "_" + "F" + Filijala;

                QueryCommands qc = new QueryCommands();

                try
                {
                    if (qc.AddTID(WorkingUser.Username, WorkingUser.Password, idNumber, cmp, long.Parse(Prio), Filijala, CCN, CID, datPrijave, VriPrijave, slaDatum, slaVrijeme, long.Parse(drive), NazivUredaja, Opis, Prijavio))
                        frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "TID " + idNumber + " - " + CCN + " added to DB at " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    new LogWriter(ex);
                    frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "TID " + idNumber + " - " + CCN + " NOT added to DB,");
                    frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "ERROR msg: " + ex.Message + " at " + DateTime.Now);
                    return false;
                }

                List<String> arr = new List<string>();
                arr = qc.SentToUsers();
                String sendList = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    sendList = arr[i];
                    i++;
                    posaljiPoruku = "<p style=''font-family:calibri;font-size:15''>" +
                        "<b>ID:</b> " + idNumber + "<br>" + "<b>Klijent:</b> " + cmp.Name + "<br>" + "<br>" +
                        "<b>Datum prijave:</b> " + datPrijave + "<br>" + "<b>Vrijeme prijave:</b> " + VriPrijave + "<br>" +
                        "<b>Datum SLA:</b> " + slaDatum + "<br>" + "<b>Vrijeme SLA:</b> " + slaVrijeme + "<br>" + "<br>" +
                        "<b>Prioritet:</b> " + Prio + "<br>" +
                        "<b>Filijala:</b> " + Filijala + "<br>" +
                        "<b>Grad:</b> " + grad + "<br>" +
                        "<b>Adresa:</b> " + "<A href=https://www.google.com/maps/search/?api=1&query=" + GoogleS + ">" + adresa + "</A >" + "<br>" +
                        "<b>Telefon:</b> " + telefon + "<br>" + "<br>" +
                        "<b>CCN:</b> " + CCN + "<br>" +
                        "<b>CID:</b> " + CID + "<br>" +
                        "<b>Drive:</b> " + drive + "<br>" + "<br>" +
                        "<b>Prijavio:</b> " + Prijavio + "<br>" +
                        "<b>Uredaj:</b> " + NazivUredaja + "<br>" +
                        "<b>Opis:</b> " + Opis + "<br>" + "<br>" + "<br>" + "</p>" +
                        "<br>TID action: " + "<A href=http://" + Properties.Settings.Default.DataSource + ":9090/takenWF?field1=" + idNumber + "&field2=" + arr[i] + ">Take me</A >" + "<br>" + "<br>" +
                        "<br>TID action: " + "<A href=http://" + Properties.Settings.Default.DataSource + ":9090/driveWF?field1=" + idNumber + "&field2=" + arr[i] + ">Start driving</A >" + "<br>" + "<br>" +
                        "<br>TID action: " + "<A href=http://" + Properties.Settings.Default.DataSource + ":9090/startWF?field1=" + idNumber + "&field2=" + arr[i] + ">Start working</A >" + "<br>" + "<br>" +
                        "<br>TID action: " + "<A href=http://" + Properties.Settings.Default.DataSource + ":9090/endWF?field1=" + idNumber + "&field2=" + arr[i] + ">End working</A >" + "<br>" + "<br>" +
                        "<p style=''font-family:calibri;font-size:12''>" +
                        "--------------------------------------" + "<br>" +
                        setAutoFooter();

                    try
                    {
                        if (qc.SendTID(WorkingUser.Username, WorkingUser.Password, idNumber, naslov, posaljiPoruku, sendList))
                            frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "TID " + idNumber + " - " + CCN + " sent to user " + arr[i - 1] + " at " + DateTime.Now);
                    }
                    catch (Exception ex1)
                    {
                        new LogWriter(ex1);
                        frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "TID " + idNumber + " - " + CCN + " NOT sent,");
                        frm.AppendTextBox(System.Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm - ") + "ERROR msg: " + ex1.Message + " at " + DateTime.Now);
                        break;
                    }
                }
            }

            return false;
        }

        private String setAutoFooter()
        {
            return "This is an automatically generated " + Properties.Settings.Default.Catalog + " status notification." + "<br>" +
                    "If you need any further information, please do not hesitate to contact:" + "<br><a href=mailto:" + Properties.Settings.Default.SupportEmail + ">Support Email</a><br>" +
                    "<A href=tel:" + Properties.Settings.Default.CmpPhone + ">Call us</A ><br><br></p>" +
                    "<p style=''font-family:calibri;font-size:17''>" + 
                    Properties.Settings.Default.CmpWWW + "<br>" + "</p>";
        }
    }
}
