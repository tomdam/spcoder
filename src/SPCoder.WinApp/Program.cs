using System;
using System.Windows.Forms;

namespace SPCoder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmSplashScreen());
            }
            catch (Exception exc)
            {
                SPCoderForm.MainForm.LogException(exc);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                SPCoderForm.MainForm.LogError("unhandled error");
                SPCoderForm.MainForm.LogError(e.ExceptionObject.ToString());
                MessageBox.Show(SPCoderForm.MainForm, e.ExceptionObject.ToString(), "Application Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                SPCoderForm.MainForm.LogException(e.Exception);
                MessageBox.Show(SPCoderForm.MainForm, e.Exception.Message, "Application Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
        }
    }
}