using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace START_Tool
{
    public partial class Download : Form
    {
        public Download()
        {
            InitializeComponent();
        }

        private void Download_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Uri url = new Uri("https://quickitsupport.gdn.accenture.com//solution/TaniumCheck.exe");
            RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.Reload);
            WebClient myWebClient = new WebClient();
            myWebClient.CachePolicy = policy;
            //myWebClient.DownloadProgressChanged += MyWebClient_DownloadProgressChanged;
            myWebClient.DownloadFileCompleted += MyWebClient_DownloadFileCompleted;
            myWebClient.DownloadFileAsync(url, "C:\\temp\\" + DateTime.Now.ToString("HH_mm_ss"));

        }

        private void MyWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            this.Close();
        }
    }
}
