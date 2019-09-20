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
        public static Thread loadTh;

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
                // new Thread(() => load.ShowDialog()).Start();

                if (loadTh == null)
                {
                    loadTh = new Thread(new ThreadStart(metodLoadShow));
                    String name = loadTh.Name = "LoadThName";
                    loadTh.Start();
                }
                else if (loadTh.Name.Equals("LoadThName"))
                {
                    loadTh.Abort();
                    loadTh = null;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }

            //try
            //{
            //    new Thread(() => load.ShowDialog()).Start();
            //}
            //catch { }
        }
        public static void metodLoadShow()
        {
            // Aborted              256     The thread state includes AbortRequested and the thread is now dead, but its state has not yet changed to Stopped.
            // AbortRequested       128     The Abort(Object) method has been invoked on the thread, but the thread has not yet received the pending ThreadAbortException that will attempt to terminate it.
            // Background           4       The thread is being executed as a background thread, as opposed to a foreground thread. This state is controlled by setting the IsBackground property.
            // Running              0       The thread has been started and not yet stopped.
            // Stopped              16      The thread has stopped.
            // StopRequested        1       The thread is being requested to stop. This is for internal use only.
            // Suspended            64      The thread has been suspended.
            // SuspendRequested     2       The thread is being requested to suspend.
            // Unstarted            8       The Start() method has not been invoked on the thread.
            // WaitSleepJoin        32      The thread is blocked. This could be the result of calling Sleep(Int32) or Join(), of requesting a lock - for example, by calling Enter(Object) or Wait(Object, Int32, Boolean) - or of waiting on a thread synchronization object such as ManualResetEvent.
            
            // loadTh.ThreadState.HasFlag(ThreadState.Running)

            try
            {
                if (loadTh != null)
                {
                    load.ShowDialog();
                }
            }
            catch { }
        }

        public static void LoadStop()
        {
            try
            {
                if (loadTh != null && loadTh.Name.Equals("LoadThName"))
                {
                    load.Invoke((MethodInvoker)delegate
                    {
                        load.Close();
                    });

                    loadTh.Abort();
                    loadTh = null;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }

            //try
            //{
            //    load.Invoke((MethodInvoker)delegate
            //    {
            //        load.Close();
            //    });

            //}
            //catch { }
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
