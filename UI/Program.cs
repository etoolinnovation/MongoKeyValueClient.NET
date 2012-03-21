using System;
using System.Threading;
using System.Windows.Forms;

namespace EtoolTech.Mongo.KeyValueClient.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(ApplicationThreadException);
            Application.Run(new MongoCacheStats());
        }

        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Mongo Cache Client");
        }
    }
}
