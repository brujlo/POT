using System;
using System.Threading;
using System.Windows.Forms;



namespace POT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static Boolean SaveDocumentsPDF;
        public static Loading load;
        public static Saving save;

        public static Thread saveTh;
        //public static Thread loadTh;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            load = new Loading();
            save = new Saving();

            Application.Run(new LoginFR());
        }

        public static void LoadStart()
        {
            try
            {
                new Thread(() => load.ShowDialog()).Start();
            }
            catch { }
        }

        public static void LoadStop()
        {
            try
            {
                load.Invoke((MethodInvoker)delegate
                {
                    load.Close();
                });

            }
            catch { }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SaveStart()
        {
            try
            {
                // new Thread(() => load.ShowDialog()).Start();

                if (saveTh == null)
                {
                    saveTh = new Thread(new ThreadStart(metodSaveShow));
                    String name = saveTh.Name = "SaveThName";
                    saveTh.Start();
                }
                else if (saveTh.Name.Equals("SaveThName"))
                {
                    saveTh.Abort();
                    saveTh = null;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
        public static void metodSaveShow()
        {
            if (saveTh != null)
            {
                save.ShowDialog();
            }
        }


        public static void SaveStop()
        {
            try
            {
                if (saveTh != null && saveTh.Name.Equals("SaveThName"))
                {
                    save.Invoke((MethodInvoker)delegate
                    {
                        save.Close();
                    });

                    saveTh.Abort();
                    saveTh = null;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
    }
}
