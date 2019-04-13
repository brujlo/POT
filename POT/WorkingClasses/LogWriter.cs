using System;
using System.IO;

namespace POT.WorkingClasses
{
    class LogWriter
    {
        public LogWriter(){}

        public LogWriter(String value)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                path = path + @"\POTLog.txt";

                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(value + Environment.NewLine + "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** ");
                tw.Close();

                Properties.Settings.Default.Path = path;
                Properties.Settings.Default.Save();
            }
            catch(Exception eInner)
            {
                WriteLog(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm") + "Error writing log error message in LogWriter constructor with string value" + Environment.NewLine + "    - ErrorMSg:" + eInner.Message + Environment.NewLine + "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** " + Environment.NewLine);
            }
        }

        public LogWriter(Exception e)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                path = path + @"\POTLog.txt";

                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm") +
                    Environment.NewLine + "    - BaseEcp: " + e.GetBaseException() +
                    Environment.NewLine + "    - InnerEcp: " + e.InnerException +
                    Environment.NewLine + "    - MessageEcp: " + e.Message +
                    Environment.NewLine + "    - SourceEcp: " + e.Source +
                    Environment.NewLine + "    - StackTrqceEcp: " + e.StackTrace +
                    Environment.NewLine + "    - TypeNameEcp: " + e.GetType().FullName + Environment.NewLine 
                    + "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** " + Environment.NewLine);
                tw.Close();

                Properties.Settings.Default.Path = path;
                Properties.Settings.Default.Save();
            }
            catch (Exception eInner1)
            {
                WriteLog(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm") + "Error writing log error message in LogWriter constructor with exception value" + Environment.NewLine + "    - ErrorMSg:" + eInner1.Message + Environment.NewLine + "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** " + Environment.NewLine);
            }
        }

        public void WriteLog(String value)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                path = path + @"\POTLog.txt";

                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(value + Environment.NewLine + "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** " + Environment.NewLine);
                tw.Close();

                Properties.Settings.Default.Path = path;
                Properties.Settings.Default.Save();
            }
            catch (Exception eInner2)
            {
                WriteLog(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm") + "Error writing log error message in LogWriter method" + Environment.NewLine + "    - ErrorMSg:" + eInner2.Message + Environment.NewLine + "    **** " + WorkingUser.Name + WorkingUser.Surename + " **** " + Environment.NewLine);
            }
        }
         
        public void LogMe(String function, String usedQC, String data, String Result)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                path = path + @"\POTLog.txt";

                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm") +
                    Environment.NewLine + "    - LOGED change: " + function + Environment.NewLine +
                    "     - Used: " + usedQC + Environment.NewLine +
                    "     - Data: " + data + Environment.NewLine +
                    "     - Result: " + Result + Environment.NewLine +
                    "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** " + Environment.NewLine);
                tw.Close();

                Properties.Settings.Default.Path = path;
                Properties.Settings.Default.Save();
            }
            catch (Exception eInner1)
            {
                WriteLog(Environment.NewLine + "- " + DateTime.Now.ToString("dd.MM.yy. HH:mm") + "Error writing log error message in LogWriter constructor with exception value" + Environment.NewLine + "    - ErrorMSg:" + eInner1.Message + Environment.NewLine + "    *** " + WorkingUser.Name + WorkingUser.Surename + " *** " + Environment.NewLine);
            }
        }
    }
}
