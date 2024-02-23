using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;

namespace START_Tool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static Mutex mutex = null;

        [STAThread]
        public static void Main()
        {


            string folder_path = @"C:\\temp";

            if (!System.IO.Directory.Exists(folder_path))
            {
                System.IO.Directory.CreateDirectory(folder_path);
            }
            const string appName = "START_Tool";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            JSonConverterIND jc = new JSonConverterIND();
            
           
            Application.Run(new SelectRole());
        }

       
    }
}