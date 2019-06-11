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

        public static Loading load;
        public static Saving save;

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

        public static void SaveStart()
        {
            try
            {
                new Thread(() => save.ShowDialog()).Start();
            }
            catch { }
        }

        public static void SaveStop()
        {
            try
            {
                save.Invoke((MethodInvoker)delegate
                {
                    save.Close();
                });
            }
            catch { }
        }
    }
}
