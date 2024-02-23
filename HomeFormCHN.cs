using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.Management.Automation;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net.Cache;
using System.Collections.Generic;
using System.Data;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Timer = System.Windows.Forms.Timer;

namespace START_Tool
{
    public partial class HomeFormCHN : Form
    {
        Thread thread = null;
        protected string scriptname;
        protected string scriptStr, path, fileName, remoteUri, hostName,substr,catstr;
        protected List<string> category = new List<string>();
        protected List<string> subcategory = new List<string>();
        //WebClient myWebClient;
        string allow = "false";
        string role = "";
        //bool isbat = false;
        bool executed = false;
        bool checkdwnld = false;
        protected DataTable categoryData=new DataTable();
        
        bool isdownloaded = false;

        public HomeFormCHN(DataTable catdt )
        {
            categoryData = catdt;   
            InitializeComponent();
           // this.DoubleBuffered = true;
        }
        public HomeFormCHN()
        {

        }

        public void saveExecutionLog(string solution )
        {
           // bool isNotEmpty = subCatList.Any();
            if (solution!=null)
            {
                try
                {
                   
                        //string c = cat;
                        DataRow[] row = categoryData.Select("[sub_category]='" + solution + "'");
                        //MessageBox.Show(row[0]["category_id"].ToString() + " - " + row[0]["sub category"].ToString());
                        catstr=row[0]["category_id"].ToString();
                        substr = solution;

                    //MessageBox.Show(catstr + substr);


                    using (var wb = new WebClient())
                    {
                        CipherConverter cc = new CipherConverter();

                        string  data = "category:" + catstr + ",sbcategory :" +substr + ",user :"+ Environment.UserName+", version:1.5";
                     
                        string encrypted = cc.EncryptString(data);
                        // MessageBox.Show(encrypted);

                        var senddata = new NameValueCollection();
                        senddata.Add("d1", encrypted);

                        var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/savelog.php", "POST", senddata);
                        //var response = wb.UploadValues("http://localhost:8080/starttool/savelog.php", "POST", senddata);

                        string responseInString = Encoding.UTF8.GetString(response);
                    }


                }
                catch (Exception ex)
                {

                    MessageBox.Show("Connection failed! please check your internet connection.");
                }
                // 0 - item 0
            } // 1 - item 1
        }
        private void HomeForm_Load(object sender, EventArgs e)
        {
            hostName = Environment.UserName;
            //SelectRole sr = new SelectRole();
            //sr.ShowDialog();
            //role = sr.srrole;

            //if(role=="open")
            //{
            //    // this.Close();
            //}
            //else
            //{
            //    this.Close();
            //}
            this.WindowState = FormWindowState.Maximized;

            //Timer MyTimer = new Timer();
            //MyTimer.Interval = (60 * 60 * 1000); // 20 mins
            //MyTimer.Tick += new EventHandler(MyTimer_Tick);
            //MyTimer.Start();

        }
        //private void MyTimer_Tick(object sender, EventArgs e)
        //{
        //    MessageBox.Show(new Form() { TopMost = true }, "Time to close the StartTool.", "Time Out",MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    this.Close();
        //}

        private void MyWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (thread != null && thread.IsAlive) 
            {
                thread.Abort();
            }

            isdownloaded = true;
            allow = "true";
           var result= MessageBox.Show("Solution prepared! Do you want to execute?", "message", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            
            // If the no button was pressed ...
            if (result == DialogResult.No)
            {
                MessageBox.Show("You selected to not execute the solution.");
                enableControl();
            }else
                {
                executeScript();
                // throw new NotImplementedException();
            }
            //panel1.Visible = false;
        }

        private void MyWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //     using (DownloadProgressChangedEventArgs e) {
            try
            {
                Invoke(new MethodInvoker(delegate ()
               {

                   progressBar1.Minimum = 0;
                   double recievedbyte = double.Parse(e.BytesReceived.ToString());

                   double totalbytes = double.Parse(e.TotalBytesToReceive.ToString());
                   double percentrecieved = recievedbyte / totalbytes * 100;
                   lblStatus.Text = $"Prepared  {string.Format("{0:0.##}", percentrecieved)}%";
                   progressBar1.Value = int.Parse(Math.Truncate(percentrecieved).ToString());


               }



              ));
            }catch(Exception ex)
            {

            }
           
        }

        private void btnFix_Click(object sender, EventArgs e)
        {

            bool ifFileExist = true;

            if (fileName != null)
            {
                panel1.Visible = true;
                try
                {

                    HttpWebRequest request = WebRequest.Create(remoteUri) as HttpWebRequest;
                    request.Method = "HEAD";
                    using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                    {
                        ifFileExist = true;
                    }
                    
                }
                catch (WebException we)
                {
                    
                    ifFileExist = false;

                }


                if (ifFileExist)
                {
                    menuStrip2.Enabled = false;
                    btnFix.Enabled = false;
                    panel7.Enabled = false;

                    checkdwnld = true;

                    try
                    {

                        RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.Reload);
                        WebClient myWebClient = new WebClient();
                        myWebClient.CachePolicy = policy;
                        myWebClient.DownloadProgressChanged += MyWebClient_DownloadProgressChanged;
                        myWebClient.DownloadFileCompleted += MyWebClient_DownloadFileCompleted;
                      

                        thread = new Thread(() =>
                        {
                            Uri url = new Uri(remoteUri);

                            myWebClient.DownloadFileAsync(url, @fileName);

                        });
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        enableControl();
                        MessageBox.Show("Please do not try to execute more than one script");
                    }
                    
                }
                else
                {
                    MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                }

            }
            if (path != null)
                {
                enableControl();
                allow = "true";
                executeScript();



                }
            
        }

        public void enableControl()
        {
            //if(isdownloaded)
            {
                if (this.menuStrip2.InvokeRequired && this.btnFix.InvokeRequired)
                {
                    btnFix.Invoke((MethodInvoker)delegate
                    {
                        btnFix.Enabled = true;
                    });

                    menuStrip2.Invoke((MethodInvoker)delegate
                    {
                        menuStrip2.Enabled = true;
                    });
                    panel7.Invoke((MethodInvoker)delegate
                    {
                        panel7.Enabled = true;
                        checkdwnld = false;
                    });

                }
                else
                {
                    menuStrip2.Enabled = true;
                    btnFix.Enabled = true;
                    panel7.Enabled = true;
                }
                
            }
        }


        private void showMessage(string messageText,string btntext)
        {
            using (MessageForm mf = new MessageForm())
            {
                mf.lblMessage.Text = messageText;
               
                mf.btnOk.Text = btntext;
               // mf.btnOk.Text = "";
                if (this.InvokeRequired )
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        mf.ShowDialog(this);
                    });

                }
                // mf.ShowDialog(this);
            }
        }
        

        private void executeScript()
        {
            
            if (allow == "true")
            {
                try
                {
                    if (fileName != null)
                    {

                        ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
                        si.CreateNoWindow = true;
                        si.FileName = fileName;
                        // si.Arguments = fileName;
                        if (scriptname != "Admin Rights")
                        {
                            si.Verb = "runas";
                        }

                        showMessage("Execution getting started, Please wait till the execution complete successfully."
                            +Environment.NewLine+ "You can Close the tool once “Command Prompt” is Closed","Start Execution");
                        executed = true;

                        Process process = System.Diagnostics.Process.Start(si);
                        //add a log entry for downloaded scripts and which are executing


                        if (scriptname == "Free Disk Space")
                        {
                            showMessage("Disk Space Solution Execute in the background," + Environment.NewLine + " Check Disk space after 15 mins","Ok");
                        }
                        saveExecutionLog(scriptname);
                        subcategory.Add(scriptname);
                        
                    }


                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("System not working properly!");
                }
                
            }
            enableControl();
        }


        


        private void freeDiskSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            panel1.Visible = false;
           // tabControl1.Visible = false;
            txtDetails.Visible = true;
            //txtDetails.Text = "If you are facing problem regarding full disk space. You can remediate this issue just by clearing space from temp, prefetch, recycle bin, windows cache. If you wish to free up disk space, click on button below."
            //     + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";

            txtDetails.Text = "如果您遇到有关磁盘空间已满的问题。您可以通过从临时、预取、回收站、Windows 缓存中清除空间来解决此问题。如果您希望释放磁盘空间，请单击下面的按钮。"
                + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";

            scriptname = "Free Disk Space";

            fileName = "C:\\temp\\DiskSpace_Cleanup.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/DiskSpace_Cleanup.exe";
            string myStringWebResource = null;

            myStringWebResource = remoteUri;
            //  myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

       

        private void unsecureDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "If you are facing problem regarding unsecure device as if it is showing on screen of your machine. You can remediate this issue just by running a script."
                 + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Unsecure Device";
            fileName = "C:\\temp\\Unsecured_device_fix.exe"; 
             remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Unsecured_device_fix.exe";
            string myStringWebResource = null;
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void removeUnauthoriseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
           // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "If you are facing problem regarding Unauthorized_ID. You can remediate this issue just by a click." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Remove Unauthorise ID";
            fileName = "C:\\temp\\Removal_of_Unauthorised_user_EID_from_Avecto_Group.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Removal_of_Unauthorised_user_EID_from_Avecto_Group.exe";
            string myStringWebResource = null;
            
            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;

            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void deleteSavedPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            //txtDetails.Text = "Do you want to clear saved passwords from credential manager and browser history? Click on the button below to continue."
            //    + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";

            txtDetails.Text = "您要从凭据管理器和浏览器历史记录中清除已保存的密码吗？单击下面的按钮继续。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";

            scriptname = "Delete Saved Password";

            fileName = "C:\\temp\\Delete_Saved_Passwords.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Clear_Saved_Passwords.exe";
            string myStringWebResource = null;

            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void changeResetPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Process.Start("https://account.activedirectory.windowsazure.com/ChangePassword.aspx");
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
        }

        private void patchingIssueFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
          //  tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;

            txtDetails.Text = "To make your device compliant with patching parameter, please click on button below." + Environment.NewLine +
                "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution. " + Environment.NewLine + Environment.NewLine +
               //" If Script is not executed, follow the below Instructions." + Environment.NewLine +
                "Replication on portal takes 48 to 72 business hours post execution of the solution."
 +Environment.NewLine+Environment.NewLine+ "Alternate Solution:" + Environment.NewLine+
"1) Go to “Windows Search” and type “Software Center”." + Environment.NewLine +
"2) Click on “Applications” folder." + Environment.NewLine +
"3)Choose “Zero Touch Solutions - Patch NC Remediation”."+Environment.NewLine+"4) Install the Solution and then Restart the Laptop.";
            //txtDetails.Text= "To make your device compliant with patching parameter, follow the Instructions Message." + Environment.NewLine +Environment.NewLine+
            //    "Or please click on button below to Run the Solution." + Environment.NewLine + Environment.NewLine + "Replication on portal takes 48 to 72 business hours post execution of the solution."
            //    + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            //MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Application” folder." + Environment.NewLine + "3) Choose “Zero Touch Solutions - Patch NC Remediation"
            //             + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "Message");
            // txtDetails.AppendText()
            //Replication on portal takes 48 to 72hrs post execution of the script
            scriptname = "Patching Issue Fix";

            fileName = "C:\\temp\\Patch_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Patch_NC_Remediation.exe";
            string myStringWebResource = null;

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;

        }

        private void antivirusFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDetails.Visible = true;
           // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "If you want to update SEP or fix SEP non compliance issue please click on the button below.";
            scriptname = "SEP_Remediation";

            fileName = "C:\\temp\\SEP_Remediation.exe";
            remoteUri = "https://mytechhelp.accenture.com/downloadExe/SEP_%20Remediation/" + hostName;
            string myStringWebResource = null;
            // Create a new WebClient instance.

            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void pollingFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            //txtDetails.Text = "To make your device compliant with polling paramater, please click on button below."
            //    + Environment.NewLine +
            //    "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine
            //    +Environment.NewLine+
            //    "Replication on portal takes 48 to 72 business hours post execution of the solution.";

            txtDetails.Text = "要使您的设备符合轮询参数，请单击下面的按钮。" + Environment.NewLine +
                "请保存您的所有工作，系统将在执行后重新启动。必须需要 Avecto Rights 才能执行以下解决方案。" + Environment.NewLine +
                "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";

            scriptname = "Polling Fix";

            fileName = "C:\\temp\\Patch_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Patch_NC_Remediation.exe";
            string myStringWebResource = null;

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void encryptionFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

            txtDetails.Text = "如果您想修复加密不合规问题，请单击下面的按钮。" + Environment.NewLine +
                "请保存您的所有工作，系统将在执行后重新启动。必须需要 Avecto Rights 才能执行以下解决方案。" +
                Environment.NewLine + Environment.NewLine + "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";
                

            //            txtDetails.Text = "If you want to fix Encryption Noncompliance issue, please click on the button below."
            //                + Environment.NewLine +
            //                "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + 
            //               Environment.NewLine + Environment.NewLine + "Replication on portal takes 48 to 72 business hours post execution of the solution."
            

            //txtDetails.Text= "If you want to fix Encryption Noncompliance issue," + Environment.NewLine +"Follow the Instructions Message." + Environment.NewLine +Environment.NewLine+
            //    "Or Please Click On the button Below." + Environment.NewLine +Environment.NewLine+ "Please save all your work, system will restart after execution. " + Environment.NewLine +
            //   Environment.NewLine+ "Replication on portal takes 48 to 72 business hours post execution of the solution." + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            //MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Application” folder." + Environment.NewLine + "3) Choose “Zero Touch Solutions - Encryption NC Remediation"
            //             + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "message");
            // https://mytechhelp.accenture.com/downloadExe/EncryptionRemediation/EnterpriseId
            //tabControl1.Visible = false;
            scriptname = "Encryption Fix";
            panel1.Visible = false;
            fileName = "C:\\temp\\Encryption_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Encryption_NC_Remediation.exe";
            string myStringWebResource = null;

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
        }

        private void chromeFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;

            txtDetails.Text = "您想更新网络浏览器 google chrome 吗？单击下面的按钮。必须需要 Avecto Rights 才能执行以下解决方案。" + Environment.NewLine + Environment.NewLine +
                "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";

            //txtDetails.Text = "Do you want to update web browser google chrome? Click on the button below. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
            //    "Replication on portal takes 48 to 72 business hours post execution of the solution.";

            //txtDetails.Text= "Do you want to update web browser google chrome?"+Environment.NewLine+"Follow the Instructions Message"+Environment.NewLine+ Environment.NewLine +
            //    "Or Please Click on the button below to Run the Solution" +Environment.NewLine+ Environment.NewLine+"Replication on portal takes 48 to 72 business hours post execution of the solution."
            //    + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            //MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Application” folder." + Environment.NewLine + "3) Choose “Chrome"
            //             + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "Message");
            scriptname = "Chrome Fix";

            fileName = "C:\\temp\\GoogleChrome_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/GoogleChrome_NC_Remediation.exe";


            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
        }

        private void secureWebFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "If you want to update Forcepoint(web secure)/fix noncompliance, click on the button below."+Environment.NewLine+
                "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." +Environment.NewLine+Environment.NewLine+
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            txtDetails.Visible = true;
            fileName = "C:\\temp\\ForcePoint_Compliance_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/ForcePoint_Compliance_Check.exe";
            string myStringWebResource = null;
            scriptname = "Secure Web Fix";
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void taniumFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
           // tabControl1.Visible = false;
            panel1.Visible = false;
            //txtDetails.Text = 
            //    " If you wish to update Tanium Noncompliance issue, click on button below."+Environment.NewLine+ "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine+ Environment.NewLine+
            //    "Replication on portal takes 48 to 72 business hours post execution of the solution." ;

            txtDetails.Text = "如果您想更新 Tanium 不合规问题，请单击下面的按钮。" + Environment.NewLine + "请保存您的所有工作，系统将在执行后重新启动。必须需要 Avecto Rights 才能执行以下解决方案。" + Environment.NewLine + Environment.NewLine +
                "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";
            scriptname = "Tanium Fix";

            fileName = "C:\\temp\\TaniumCheck.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/TaniumCheck.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void eEVAFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to update EVTA/fix Noncompliance EVTA?" +
                " Please click on button below." + Environment.NewLine + "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + 
                Environment.NewLine + Environment.NewLine + "Replication on portal takes 48 to 72 business hours post execution of the solution.";
                
            scriptname = "EVTA Fix";

            fileName = "C:\\temp\\EEVA_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/EVTA_NC_Remediation.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black ;
        }

        private void office365FixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible=false;
            btnHfbT3.Visible=false;
            btnAzure.Visible = false;
            txtDetails.Visible = true;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            //txtDetails.Text = "Do you want to update Office365/fix Noncompliance? please click on button below. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
            //    "Replication on portal takes 48 to 72 business hours post execution of the solution.";

            txtDetails.Text = "是否要更新 Office365/修复不合规？请点击下面的按钮。必须需要 Avecto Rights 才能执行以下解决方案。" + Environment.NewLine + Environment.NewLine +
                "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";

            scriptname = "Office 365 Fix";

            fileName = "C:\\temp\\Office365_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Office365_NC_Remediation.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void outlookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            //  tabControl1.Visible = false;
            txtDetails.Text = "警报！您要删除您的 outlook 配置文件并重新创建吗？要重新创建 outlook 配置文件，请单击下面的按钮继续。必须需要 Avecto Rights 才能执行以下解决方案。";

            //txtDetails.Text = "Alert! Do you want to delete your outlook profile and recreate it again? To recreate outlook profile click on the button below to continue. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Reconfigure Outlook";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            fileName = "C:\\temp\\Outlook_Profile_Deletion_Creation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Outlook_Profile_Deletion_Creation.exe";
            string myStringWebResource = null;

            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
           // txtDetails.ForeColor = System.Drawing.Color.Black;

        }

       
        

        private void office365ReinstallationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "如果您想重新安装 Office 365，请单击下面的按钮。必须需要 Avecto Rights 才能执行以下解决方案。";

            //txtDetails.Text = "If you want to reinstall Office 365 click on the button below. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Office 365 Reinstallation";

            fileName = "C:\\temp\\Office365_Reinstall.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Office365_Reinstall.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void resetPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("Reset-Unlock Password");
            saveExecutionLog("Reset-Unlock Password");
            Process.Start("https://passwordreset.microsoftonline.com/?whr=accenture.com");
            executed = true;
        }

        private void updateMSOfficeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDetails.Text = "If you want to  update microsoft office365 please click on the button below.";
          //  tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            path = Application.StartupPath + "\\script\\Office_Update.ps1";
            //txtDetails.Text = "If  you want to fix issue regarding Protect my Tech(PMT) tool click on the button below";
            scriptname = "updatemsoffice";
            fileName = null;
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void adminRightsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = true;
            txtDetails.Text = "对于 Avecto 管理员权限，请按照以下步骤操作。" + Environment.NewLine + "* 访问以下链接并下载“Avecto_Admin_Access_Automation”解决方案。" + Environment.NewLine +
                "* 下载后执行解决方案" + Environment.NewLine + "* 解决方案执行后重启笔记本电脑";

            //txtDetails.Text = "For Avecto Admin Rights follow the below Steps." + Environment.NewLine + "* Access the below link and Download “Avecto_Admin_Access_Automation” Solution." + Environment.NewLine +
            //  "* Execute the Solution once downloaded" + Environment.NewLine + "* Restart the laptop once Solution Executed ";
            txtDetails.Visible = true;
            panel1.Visible = false;
            
            //txtDetails.Text = "If  you want to fix issue regarding Protect my Tech(PMT) tool click on the button below";
            scriptname = "Admin Rights";
            //remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Avecto_Admin_Access_Automation_V1.01.exe";
            //fileName = "C:\\temp\\Avecto_Admin_Access_Automation_V1.01.exe";
            btnFix.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
           // tabControl1.Visible = false;
        }


        

       

        

        private void processDocumetsForKnownIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // tabControl1.Visible = true;
        }

        private void gSSharepointLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0072157&sys_id=2295e1da1389f200e24d30128144b022");
            executed = true;
        }

        private void aTCISharepointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/India_info/Survey/SitePages/Home.aspx");
            executed = true;
        }

        private void gSSharepointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ICF_AMT-PAN_India/SitePages/ASSET-ALLOCATION-REQUEST.aspx");
            executed = true;
        }

        private void aOSharepointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            Process.Start("https://apps.powerapps.com/play/17e07279-263d-4c01-9f5b-4f069f967385?tenantId=e0793d39-0939-496d-b129-198edd916feb&hidenavbar=true");

            executed = true;
        }


        private void aTCISharepointLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0072180&sys_id=ac7da7f6134d3600e24d30128144b0fe");
            executed = true;
        }

      

        private void vCBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/MS_Teams_Booking_Procedure_1.2.pdf");
            executed = true;
        }

       

        private void registrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/SMBLTSSupportTeam");
            executed = true;
        }

        

        private void shareScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0075401&sys_id=b4e6136e13453600e24d30128144b080&catID=8e1fbc47db0aaf0025cd9c41ba9619f8");
            executed = true;
        }

        private void mSFormRelatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/forms-support-page/?section=ms_forms_introduction");
            executed = true;
        }

        private void registrationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            Process.Start("https://apaau1101.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            saveExecutionLog(sender.ToString());
            executed = true;
        }

        private void registrationLink2ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Process.Start("https://apaau1102.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            executed = true;

        }

        private void registrationLink3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://amrau1101.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            executed = true;

        }

        private void registrationLink4ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Process.Start("https://amrau1102.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            executed = true;

        }

        private void registrationLink5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            Process.Start("https://emeau1101.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            executed = true;
        }

        private void registrationLink6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://emeau1102.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            executed = true;

        }

        private void vPNAccessRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("Accenture VPN Access Request Link");
            saveExecutionLog("Accenture VPN Access Request Link");
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0075158&sys_id=1affe8ad37690a00812bd5c543990e32&catID=null");
            executed = true;
        }

   

        private void powerBISubbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
           // tabControl1.Visible = false;
            btnFix.Visible = false;
            panel1.Visible = false;
            scriptname = "Power BI Subscription";
            //            txtDetails.Text = "Steps on how to upgrade Power BI request:" + Environment.NewLine +

            //           " 1.Visit Accenture Support Portal - https://support.accenture.com/support_portal" + Environment.NewLine +

            //"2.Click the Technology Support." + Environment.NewLine +
            //"3.Select 'Browse All Services'(found at the left side of the site)." + Environment.NewLine +
            //"4.Under the Productivity Tools, Click the ‘Reporting & Analytics’ and" + Environment.NewLine +
            //"5.Select the ‘Microsoft Power BI premium'." + Environment.NewLine +
            //"6.At the 'Request Action' select 'Upgrade existing subscription'." + Environment.NewLine +
            //"7.Fill up the 'Executive Sponsor' and 'Charge Code'." + Environment.NewLine +
            //"8.Click 'Submit'." + Environment.NewLine;

            txtDetails.Text = "有关如何升级 Power BI 请求的步骤：" + Environment.NewLine +
                "1.访问埃森哲支持门户 - https://support.accenture.com/support_portal" + Environment.NewLine +
                "2.点击技术支持。" + Environment.NewLine +
                "3.选择“浏览所有服务”（位于站点左侧）。" + Environment.NewLine +
                "4.在生产力工具下，单击“报告和分析”，然后" + Environment.NewLine +
                "5.选择“Microsoft Power BI Premium”。" + Environment.NewLine +
                "6.在“请求操作”中选择“升级现有订阅”。" + Environment.NewLine +
                "7.填写“Executive Sponsor”和“Charge Code”。" + Environment.NewLine +
                "8.点击“提交”。";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            executed = true;
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("Change Password");

            Process.Start("https://account.activedirectory.windowsazure.com/ChangePassword.aspx");
            executed = true;
            saveExecutionLog("Change Password");
        }

        private void txtDetails_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (scriptname == "PowerBI")
            {
                subcategory.Add("Power BI Subscription");
                saveExecutionLog("Power BI Subscription");
                Process.Start("https://support.accenture.com/support_portal");
                executed = true;
            }
            if (scriptname== "Power BI Subscription")
            {
                subcategory.Add("Power BI Subscription");
                saveExecutionLog("Power BI Subscription");
                Process.Start("https://support.accenture.com/support_portal");
                executed = true;

            }
            if(scriptname== "Azure MFA Registration")
            {
                subcategory.Add("Azure MFA Registration");
                saveExecutionLog("Azure MFA Registration");
                Process.Start("https://mysignins.microsoft.com/security-info");
                executed = true;
            }
            if(scriptname== "MS Teams Display Name Change")
            {
                saveExecutionLog("MS Teams");
                subcategory.Add("MS Teams");
                Process.Start("https://wd3.myworkday.com/accenture");
                executed = true;
            }
            if(scriptname== "Accenture VPN Fix")
            {
                executed = true;
                saveExecutionLog("Accenture VPN Fix");
                subcategory.Add("Accenture VPN Fix");
                Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/VPN-Access.pdf");
            }
            if(scriptname== "CORA Tool")
            {
                subcategory.Add("CORA Tool");
                saveExecutionLog("CORA Tool");
                executed = true;

                Process.Start("https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=8eead6ae1b2348d0ae30dd77cc4bcb4b");
            }
            if(scriptname== "Smart CardLogon Fix")
            {
                subcategory.Add("Smart CardLogon Fix");
                saveExecutionLog("Smart CardLogon Fix");
                executed=true;
                Process.Start("https://mypasswordless.accenture.com/gopasswordless");
            }
            //if (scriptname == "Smart CardLogon Fix")
            //{
            //    subcategory.Add("Smart CardLogon Fix");
            //    saveExecutionLog("Smart CardLogon Fix");
            //    executed = true;
            //    Process.Start("https://in.accenture.com/connectivitysecurity/passwordless-support/");
            //}
            // executed = true;
        }

        private void assetReturnRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/India_info/AR/Lists/RPMUL/AllItems.aspx");
            executed = true;
        }

        private void assetReturnGuidlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/Outstation_employee_couriering_to_office_process_V3.pdf");
            executed = true;

        }

        private void mSTeamsMeetingAddinIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
           // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "如果您想修复 MS Team 会议插件选项，请单击下面的按钮。必须需要 Avecto Rights 才能执行以下解决方案。";

            //txtDetails.Text = "If you want to fix MS Team meeting addin option, please click on button below. Avecto Rights must be needed to execute the below solution.";
            scriptname = "MS Teams Meeting Addin Issue";
            fileName = "C:\\temp\\Teams_Meeting_Addin_issue.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Teams_Meeting_Addin_issue.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void checkDeviceComplianceStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.accenture.com/support_portal?id=acn_my_devices");
            executed = true;
            //MessageBox.Show(sender.ToString());
            subcategory.Add("Device Compliance Status Check");
            saveExecutionLog("Device Compliance Status Check");
        }

        private void btnReturnofc_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://forms.office.com/Pages/ResponsePage.aspx?id=OT154DkJbUmxKRmO3ZFv6--dYu3s0NlFgrd7gXjm165UOE1XT1pBOU4zNkFKS1Y3MExHNDhWNFdVTi4u");
            executed = true;
        }

        private void btnCSB_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/945945-basics-of-finance-amp-commercial-services");
            executed = true;
        }

        private void complianceIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mSTeamsInformationLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/microsoftteams/meetings-and-calls/");
            executed = true;
        }

        private void mobileDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/");
            executed = true;
        }

        private void powerBIInformationLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/power-bi-provisioning-support-site/");
            executed = true;
        }

        private void adobAcrobatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/adobe-acrobat-pro/");
            executed = true;
        }

        private void mSProjectInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/project/?section=get_project");
            executed = true;
        }

        private void mSVisioInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/how-do-i-get-access-to-ms-visio/");
            executed = true;
        }

        private void mSPowerAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/microsoft-powerapps/");
            executed = true;
        }

        private void managingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/getting-started-with-onedrive-online/managing-files/");
            executed = true;
        }

        private void pCSyncConfiigurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/onedrive-for-business-sync/?section=sync_installation");
            executed = true;
        }

        private void maanagingFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/getting-started-with-onedrive-online/managing-folders/");
            executed = true;
        }

        private void pCSyncIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("http://in.accenture.com/onedriveforbusiness/onedrive-for-business-sync/?section=PC_synchronization");
            executed = true;
        }

        private void usingSelectiveSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/onedrive-for-business-sync/?section=selective_sync");
            executed = true;
        }

        private void sharingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/getting-started-with-onedrive-online/sharing-management/");
            executed = true;
        }

        private void externalFileSharingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/accenture-external-file-share/");
            executed = true;
        }

        private void knownIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/issues-and-workarounds/?section=known_issues");
            executed = true;
        }

        private void powerAutomateInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/converting-workflows-to-power-automate/");
            executed = true;
        }

        private void basicIssueWorkaroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/cioorganization/office365_downtimes_and_workarounds/");
            executed = true;
        }

        private void checkComplienceOfDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=acn_my_devices");
            executed = true;
        }

        private void purchaseAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://s1.ariba.com/Buyer/Main/aw?awh=r&awssk=kDN6u4AI&realm=accenture&dard=1&sc_cat_item_category=07d57e35dbc837046c7f3318f4961910");
            executed = true;
        }

        private void myAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/asset?id=eam_asset_dashboard&sc_cat_item_category=32dbe9e6db7d9094c3a3ea7b03961926");
            executed = true;
        }

        private void softwareAssetManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/asset?id=eam_receive_assets&active_link=SAM&sc_cat_item_category=c613d065dba3b3002a3e105f689619c3");
            executed = true;
        }

        private void microsoftTeamsRecordingServiceAccentureSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            executed = true;
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0074573&sys_id=1b34d1f3db18c340890af3d51d961973");
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void btnISAService_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/946325-isa-service-overview-ctsdrms");
        }

        private void btnServMgmt_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/961305-itsm-amp-itil-concepts");
            executed = true;
        }

        private void btnDataServices_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/947332-basics-of-data-services");
            executed = true;
        }

        private void btnCloud_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/958529-cloud-overview");
            executed = true;
        }

        private void homePersonsWithDisablilitiesPwDReasonableAccommodationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("PwD reasonable Accommodation");
            saveExecutionLog("PwD reasonable Accommodation");
            Process.Start("https://ts.accenture.com/sites/PwD_Reasonable_Accommodations/default.aspx");
            executed = true;
           // MessageBox.Show(sender.ToString());
        }

        private void reportStolenHardwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://asoc.accenture.com/");
            executed = true;
        }

        private void protectingAccentureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/protectingaccenture/");
            executed = true;
        }

        private void informationSecurityExceptionPortalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://egrc.accenture.com/default.aspx");
            executed = true;
        }

        private void microsoftTeamsRecordingServiceAccentureSupportToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            executed = true;
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0074573&sys_id=1b34d1f3db18c340890af3d51d961973");
        }

        private void signinMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDetails.Text = "Steps on how to select sign-in methos:" + Environment.NewLine +

           " 1. Visit given link - https://mysignins.microsoft.com/security-info" + Environment.NewLine +

"2.	Click on “Add Method”." + Environment.NewLine +
"3. Select Which method would you like to add from the drop Down." + Environment.NewLine +
"4. Click on Add and Enter the Details and then follow the instructions.";

            txtDetails.ForeColor = System.Drawing.Color.Black;
            executed = true;
            btnFix.Visible = false;
            panel1.Visible = false;
           /// tabControl1.Visible = false;
        }

        private void visitSupportSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/azure-mfa-and-self-service-password-reset-sspr/?referrer=mailer");
        }

        private void btnQuickSolution_Click(object sender, EventArgs e)
        {
            //hideMenu();
            //showMenu(panel6);
        }

        private void citrixFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            panel1.Visible = false;
            //tabControl1.Visible = false;
            var citrixFix = MessageBox.Show("对于 Citrix 安装或修复，您必须先批准 NSSR 请求，然后才能继续操作。", "message", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (citrixFix == DialogResult.Yes)
            {
                scriptname = "Citrix Receiver Fix";
                DataRow[] row = categoryData.Select("[sub_category]='" + scriptname + "'");
                catstr = row[0]["category_id"].ToString();
                substr = scriptname;
                using (var wb = new WebClient())
                {
                    CipherConverter cc = new CipherConverter();
                    string data = "user :" + Environment.UserName + ", category:" + catstr + ",sbcategory :" + substr + ", selected : yes" + ", version:1.5";

                    string encrypted = cc.EncryptString(data);
                    // MessageBox.Show(encrypted);

                    var senddata = new NameValueCollection();
                    senddata.Add("d1", encrypted);

                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/citrix_log.php", "POST", senddata);

                    string responseInString = Encoding.UTF8.GetString(response);

                }
                txtDetails.Visible = true;

                //txtDetails.Text = "If you wish to reinstall/ fix Citrix issue, please click on the button below."+Environment.NewLine+ "Please save all your work, system will restart after execution of solution. Avecto Rights must be needed to execute the below solution."

                txtDetails.Text = "如果您想重新安装/修复 Citrix 问题，请单击下面的按钮。" + Environment.NewLine + "请保存您的所有工作，系统将在执行解决方案后重新启动。必须需要 Avecto Rights 才能执行以下解决方案。";


                scriptname = "Citrix Receiver Fix";

                fileName = "C:\\temp\\Citrix_WorkSpace_Reinstallation.exe";
                remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Citrix_Receiver.exe";
                string myStringWebResource = null;
                // Create a new WebClient instance.

                // Concatenate the domain with the Web resource filename.
                myStringWebResource = remoteUri;
                // myWebClient.DownloadFile(myStringWebResource, @fileName);
                btnFix.Visible = true;
                txtDetails.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                scriptname = "Citrix Receiver Fix";
                DataRow[] row = categoryData.Select("[sub_category]='" + scriptname + "'");
                catstr = row[0]["category_id"].ToString();
                substr = scriptname;
                using (var wb = new WebClient())
                {
                    CipherConverter cc = new CipherConverter();
                    string data = "user :" + Environment.UserName + ", category:" + catstr + ",sbcategory :" + substr + ", selected : no" + ", version:1.5";

                    string encrypted = cc.EncryptString(data);
                    // MessageBox.Show(encrypted);

                    var senddata = new NameValueCollection();
                    senddata.Add("d1", encrypted);

                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/citrix_log.php", "POST", senddata);

                    string responseInString = Encoding.UTF8.GetString(response);

                }
                txtDetails.Visible = false;
                btnFix.Visible = false;
                subcategory.Add("Citrix Receiver Fix");
                saveExecutionLog("Citrix Receiver Fix");
                Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0072221&sys_id=aed365d1dbbd07003cacfb051d96199a&catID=null");
                executed = true;


            }

        }

       

        private void vPNFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible=false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Please access the below link to check the Status of Accenture VPN access" +
                Environment.NewLine + "https://quickitsupport.gdn.accenture.com/start_doc/VPN-Access.pdf"
                + Environment.NewLine+ "Please execute the VPN Solution if you have approved Subscription.";
            scriptname = "Accenture VPN Fix";

            fileName = "C:\\temp\\Basic_VPN_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Basic_VPN_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

  
       

        

       

        private void recreatingCurruptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/recreating-corrupt-outlook-profile/");
            executed = true;
        }

        private void enterpriseAccountLockoutsAfterAPasswordChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            executed = true;
            Process.Start("https://in.accenture.com/connectivitysecurity/how-to-avoid-enterprise-account-lock-outs-after-a-password-change/");
        }

        private void assigningArchivePoliciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/using-the-auto-archive-feature/");
        }

        private void checkOutlookStorageQuotaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/check-your-outlook-storage-quota/");
        }

        private void accessWebmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://myemail.accenture.com/mail/");
        }

        private void outlookDataFilePSTDefaultLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/outlook-data-file-pst-default-location/");
        }

        private void createOnlineEMailArchiveFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/create-an-online-e-mail-archive-folder/");
        }

        private void moveMessagesFromOutlookToOnlineEMailArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/move-messages-from-outlook-to-the-online-e-mail-archive/");
        }

        private void moveFoldersFromOutlookToOnlineEMailArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/move-folders-from-outlook-to-the-online-e-mail-archive/");
        }

        private void increaseMailboxSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/how-do-i-increase-my-mailbox-size/");
        }

        private void requestForYToZProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add("Request for Y to Z Profile");
            saveExecutionLog("Request for Y to Z Profile ");
            Process.Start("https://support.accenture.com/support_portal?id=acn_sac&sc_cat_id=37a5e6a2dba71bc02cfe7fc88c9619c3&spa=1&page=details");
        }

        private void requestAdditionToDistributionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/how-to-request-addition-to-a-distribution-list/");

        }

        private void howToDelayOrScheduleSendingEailMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/how-to-set-a-delay-on-outgoing-e-mails/");
        }

        private void addingOrRemovingASharedMailboxProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/teamingsharingconferencing/adding-or-removing-a-shared-mailbox-profile-in-your-microsoft-outlook/");
        }

        private void disablingOutlookDesktopNotificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/disabling-outlook-desktop-notifications/");
        }

        private void clearingOutlookRemindersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/clearing-outlook-reminders/");
        }

        private void readEncryptedEMailsInOWAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/how-to-read-encrypted-e-mails-in-owa/");
        }

        private void accessingTheOWAVersionOfTheSharedMailboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/teamingsharingconferencing/accessing-the-owa-version-of-the-shared-mailbox/");
        }

        private void manageCalenderGroupsInOutlookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/manage-calendar-groups-in-outlook/");
        }

        private void reopeningPSTFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/reopening-pst-files/");
        }

        private void accessingAccentureEMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://myemail.accenture.com/mail//");
        }

        private void oneDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aIRSupportPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/applicationinformationrepository/home-page/");
            executed = true;
            
        }

        private void registerApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/air?id=acn_service_catalog_dp&sys_id=9f07abca13ee8f800931fc04e144b07d");
            executed = true;
        }

        private void approvedSoftwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=softwares&category=top-downloads");
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
        }

        private void supportPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/software-catalog/");
            executed = true;
        }

        private void softwareUnavailableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/software_not_found.pdf");
            executed = true;
        }

        private void myLearningSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            Process.Start("https://mylearningchatbot-prod.accenture.com/");
            executed = true;
        }

        private void azureMFARegistrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            panel1.Visible = false;
            btnFix.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            scriptname = "Azure MFA Registration";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //txtDetails.Text = "Click on link - https://mysignins.microsoft.com/security-info" + Environment.NewLine+Environment.NewLine +


            //    "* Click on “Add Method”." +Environment.NewLine+


            //    "* Choose method would you like to add from the drop Down." +Environment.NewLine+

            //    "* Click on Add & Enter Details and then follow the instructions.";

            txtDetails.Text = "点击链接— https://mysignins.microsoft.com/security-info" + Environment.NewLine + Environment.NewLine +
               "* 点击“添加方法”。" + Environment.NewLine +
               "* 从下拉列表中选择您想添加的方法。" + Environment.NewLine +
               "* 单击“添加并输入详细信息”，然后按照说明进行操作。";
        }

        private void azureMFaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/azure-mfa-and-self-service-password-reset-sspr/?referrer=mailer");
            executed = true;
        }

        private void vCCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/VC_Booking_Cost_and_Other_Details.pdf");
            executed = true;

        }

   
 

        private void linksAndHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

       

        private void applicationIntegrationHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/enterprisesignonintegration/application-integration/");
            executed = true;
        }

        private void gettingStartedWithAppIntegrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/enterprisesignonintegration/getting-started-with-eso-integration/");
            executed = true;
        }

        private void submitTicketWithESOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/enterprisesignonintegration/how-to-submit-a-ticket/");
            executed = true;
        }


        private void btnSecurity_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/945964-basics-of-information-security");
            executed = true;
        }

        private void btnMac_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ACD_Info/Training/NJWBT/SitePages/Home.aspx");
            executed = true;
        }

        private void btnInduction_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ACD_Info/Training/NJWBT/SitePages/Home.aspx");
            executed = true;
        }

        private void btnKnwmgmt_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ACD_Info/Training/NJWBT/SitePages/Home.aspx");
            executed = true;
        }

        private void btnWorkstation_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ACD_Info/Training/NJWBT/SitePages/Home.aspx");
            executed = true;

        }

        private void btnNtwrk_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/946490-basics-of-networking");
            executed = true;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://connectedlearning.accenture.com/learningboard/946517-microsoft-excel-basics");
            executed = true;
        }

        private void btnSoftSkill_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ACD_Info/Training/NJWBT/SitePages/Home.aspx");
            executed = true;
        }

        private void btnCX_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ACD_Info/Training/NJWBT/SitePages/Home.aspx");
            executed = true;
        }

        private void overallNCRemediationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            //txtDetails.Text = "Do you want to fix all compliance issues for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            txtDetails.Text = "您想修复系统的所有合规性问题吗？单击下面的按钮运行解决方案。必须需要 Avecto Rights 才能执行以下解决方案。";
            scriptname = "Overall NC Remediation";

            fileName = "C:\\temp\\Overall_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Overall_NC_Remediation.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

        private void forcePointRemediationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix the issues related to ForcePoint? Click on the button below to continue.";
            scriptname = "ForcePoint_Compliance_Check";

            fileName = "C:\\temp\\ForcePoint_Compliance_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/ForcePoint_Compliance_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void javaNCRemediationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //txtDetails.Text = "Do you want to fix Java compliance issue?" + Environment.NewLine + "Follow the Instructions Message" + Environment.NewLine + Environment.NewLine +
            //    "Or Please Click on the button below to Run the Solution." + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            //MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Application” folder." + Environment.NewLine + "3) Choose “Zero Touch Solutions - Java NC Remediation"
            //             + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "Message");

            //            txtDetails.Text = "Do you want to fix Java compliance issue? click a button below to run solution. Avecto Rights must be needed to execute the below solution." 
            

            txtDetails.Text = "您想解决 Java 合规性问题吗？单击下面的按钮运行解决方案。必须需要 Avecto Rights 才能执行以下解决方案。";
                
            scriptname = "Java NC  Remediation";

            fileName = "C:\\temp\\Java_Compliance_Checkv1.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Java_Compliance_Checkv1.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
        }

        private void phishMeCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            scriptname = "Cofense Reporter Check";
            panel1.Visible = false;
            //txtDetails.Text =" If you wish to update Cofense reporter fix Noncompliance issue click on button below." + Environment.NewLine + "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
            //                "Replication on portal takes 48 to 72 business hours post execution of the solution.";

            txtDetails.Text = "如果您希望更新 Cofense 报告器修复不合规问题，请单击下面的按钮" + Environment.NewLine + "请保存您的所有工作，系统将在执行后重新启动。必须需要 Avecto Rights 才能执行以下解决方案。" + Environment.NewLine + Environment.NewLine +
                "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";

            fileName = "C:\\temp\\PhishMe_Check.exe ";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/PhishMe_Check.exe";
            string myStringWebResource = null;
            btnFix.Visible = true;
            txtDetails.ForeColor= System.Drawing.Color.Black;
        }

        private void antiVirusFixToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //tabControl1.Visible = false;
            scriptname = "MDfE Antivirus Fix";
            panel1.Visible = false;

            txtDetails.Text = "如果您希望更新或修复 MDfE 防病毒合规性问题，请单击下面的按钮。" + Environment.NewLine + "请保存您的所有工作，系统将在执行后重新启动。必须需要 Avecto Rights 才能执行以下解决方案。" + Environment.NewLine + Environment.NewLine +
                "解决方案执行后，门户网站上的复制需要 48 到 72 个工作小时。";
                

            //            txtDetails.Text =
            //                            " If you wish to update or Fix MDfE Antivirus compliance issue, click on button below." + Environment.NewLine + "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
            //                            "Replication on portal takes 48 to 72 business hours post execution of the solution." 
           

            //txtDetails.Text= " If you wish to update or Fix MDfE Antivirus compliance issue,"+Environment.NewLine+"Follow the Instrctions Message"
            //    +Environment.NewLine+Environment.NewLine+"Or Please Click on the button below to run the Solution"
            //    + Environment.NewLine +Environment.NewLine + "Please save all your work, system will restart after execution." + Environment.NewLine + Environment.NewLine + "Replication on portal takes 48 to 72 business hours post execution of the solution."
            //    + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            //MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Application” folder." + Environment.NewLine + "3) Choose “Zero Touch Solutions - MDfE NC Remediation"
            //             + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "Message");
            fileName = "C:\\temp\\MDfE_AVFW_NC_Remediation.exe ";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/MDfE_AVFW_NC_Remediation.exe";

            string myStringWebResource = null;
            btnFix.Visible = true;
        }

        private void beyondTrustPMFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            scriptname = "BeyondTrust PM Fix";
            panel1.Visible = false;
            txtDetails.Text =
                            " If you wish to update or Fix BeyondTrust PM Check compliance issue, click on button below." + Environment.NewLine + "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
                            "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            fileName = "C:\\temp\\BeyondTrust_PM_compliance.exe ";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/BeyondTrust_PM_compliance.exe";
            string myStringWebResource = null;
            btnFix.Visible = true;
            txtDetails.ForeColor= System.Drawing.Color.Black;
        }

        private void rASVPNAccessRequestLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071256&sys_id=66d1ed5a1385ba4099f8dbf18144b077");
            executed = true;
        }

        private void azureMFAToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void enterpriseIDChangeRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://directory.accenture.com/NameChange");
            executed = true;
        }

        private void mSTeamsDisplayNameChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            panel1.Visible = false;
            btnFix.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            scriptname = "MS Teams Display Name Change";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //txtDetails.Text = "https://wd3.myworkday.com/accenture"+Environment.NewLine+
            ////    MessageBox.Show("Access above(myworkday) portal and Select Role." + Environment.NewLine + "Go to profile Picture and Select View Profile." + Environment.NewLine + "Select Personal towards left-hand side." + Environment.NewLine +
            ////"Go to Preferred Name, Edit, Provide the Details and Submit." + Environment.NewLine +
            ////"Replication on Teams takes ~48Hrs", "Message");
            //"* Access above(myworkday) portal and Select Role." + Environment.NewLine +
            //"* Go to profile Picture and Select View Profile." + Environment.NewLine +
            //"* Select Personal towards left-hand side." + Environment.NewLine +
            //"* Go to Preferred Name, Edit, Provide the Details and Submit." + Environment.NewLine +
            //"* Replication on Teams takes ~48Hrs";

            txtDetails.Text = "https://wd3.myworkday.com/accenture" + Environment.NewLine + Environment.NewLine +
                "访问上面的 (myworkday) 门户并选择角色。" + Environment.NewLine +
                "转到个人资料图片并选择查看个人资料。" + Environment.NewLine +
                "选择左侧的个人。" + Environment.NewLine +
                "转到首选名称，编辑，提供详细信息并提交。" + Environment.NewLine +
                "Teams 上的复制需要大约 48 小时";

        }

        private void pulseSecureReinstallationexeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text= "您想修复系统上的 Pulse Secure 问题吗？单击下面的按钮运行解决方案。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "Do you want to fix Pulse Secure issue on your system. click a button below to run solution. " + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Pulse Secure Reinstallation";

            fileName = "C:\\temp\\PulseSecure_Reinstallation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/PulseSecure_Reinstallation.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

        private void citrixWorkspaceFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            var citrixFix = MessageBox.Show("对于 Citrix 安装或修复，您必须先批准 NSSR 请求，然后才能继续操作。", "message", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (citrixFix == DialogResult.Yes)
            {
                scriptname = "Citrix Workspace Fix";
                DataRow[] row = categoryData.Select("[sub_category]='" + scriptname + "'");
                catstr = row[0]["category_id"].ToString();
                substr = scriptname;
                using (var wb = new WebClient())
                {
                    CipherConverter cc = new CipherConverter();
                    string data = "user :" + Environment.UserName + ", category:" + catstr + ",sbcategory :" + substr + ", selected : yes" + ", version:1.5";

                    string encrypted = cc.EncryptString(data);
                    // MessageBox.Show(encrypted);

                    var senddata = new NameValueCollection();
                    senddata.Add("d1", encrypted);

                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/citrix_log.php", "POST", senddata);

                    string responseInString = Encoding.UTF8.GetString(response);

                }
                txtDetails.Visible = true;
                panel1.Visible = false;

                txtDetails.Text = "您想为您的系统修复 Citrix 工作区问题吗？单击下面的按钮运行解决方案。必须需要 Avecto Rights 才能执行以下解决方案。";

                //txtDetails.Text = "Do you want to fix Citrix workspace issue for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution." + 

                scriptname = "Citrix Workspace Fix";

                fileName = "C:\\temp\\Citrix_WorkSpace.exe";
                remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Citrix_WorkSpace.exe";
                string myStringWebResource = null;
                // Create a new WebClient instance.

                // Concatenate the domain with the Web resource filename.
                myStringWebResource = remoteUri;
                //myWebClient.DownloadFile(myStringWebResource, @fileName);
                btnFix.Visible = true;
                txtDetails.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                scriptname = "Citrix Workspace Fix";
                DataRow[] row = categoryData.Select("[sub_category]='" + scriptname + "'");
                catstr = row[0]["category_id"].ToString();
                substr = scriptname;
                using (var wb = new WebClient())
                {
                    CipherConverter cc = new CipherConverter();
                    string data = "user :" + Environment.UserName + ", category:" + catstr + ",sbcategory :" + substr + ", selected : no" + ", version:1.5";

                    string encrypted = cc.EncryptString(data);
                    // MessageBox.Show(encrypted);

                    var senddata = new NameValueCollection();
                    senddata.Add("d1", encrypted);

                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/citrix_log.php", "POST", senddata);

                    string responseInString = Encoding.UTF8.GetString(response);

                }
                txtDetails.Visible = false;
                btnFix.Visible = false;
                subcategory.Add(sender.ToString());
                saveExecutionLog(sender.ToString());
                Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0072221&sys_id=aed365d1dbbd07003cacfb051d96199a&catID=null");
                executed = true;


            }

        }

        private void thirdPartyRiskAssessmentTPRAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071863&sys_id=9bf66cc813957e4099f8dbf18144b0b8");
            executed = true;
        }

        private void nonStandardSoftwareRequestNSSRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services_iframe&articleNumber=KB0072221&sys_id=aed365d1dbbd07003cacfb051d96199a&catID=null");
            executed = true;
        }

        private void firewallCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "你想为你的系统修复防火墙问题吗？单击下面的按钮运行解决方案。" + Environment.NewLine + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "Do you want to fix Firewall issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Firewall Check";

            fileName = "C:\\temp\\Firewall_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Firewall_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void dLPCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix DLP issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "DLP Check";

            fileName = "C:\\temp\\DLP_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/DLP_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

        private void adobeReaderCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Adobe reader issue for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Adobe Reader Check";

            fileName = "C:\\temp\\Adobe_Reader_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Adobe_Reader_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

        private void eVTACheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix EVTA issue for your system. click a button below to run solution. ";
            scriptname = "EVTA_Check.exe";

            fileName = "C:\\temp\\EVTA_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/EVTA_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void adobeCreativeCloudSuiteCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix adobe creative cloud suite issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Adobe Creative Cloud Suite Check";

            fileName = "C:\\temp\\adobeCreativeCloudSuite_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/adobeCreativeCloudSuite_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void domainMembershipCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Domain membership issue for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Domain Membership Check";

            fileName = "C:\\temp\\domainMembership_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/domainMembership_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void unauthorizedSoftwareCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix python software issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Python Software Check";

            fileName = "C:\\temp\\pythonSoftware_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/pythonSoftware_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void chromeCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Google chrome issue for your system. click a button below to run solution. ";
            scriptname = "Chrome_Check.exe";

            fileName = "C:\\temp\\Chrome_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Chrome_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void firefoxCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Firefox issue for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Firefox Check";

            fileName = "C:\\temp\\firefox_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/firefox_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void edgeCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Edge issue for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Edge Check";

            fileName = "C:\\temp\\edge_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/edge_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void internetExplorerCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Internet Explorer issue for your system. click a button below to run solution. " + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "internet Explorer Check";

            fileName = "C:\\temp\\internetExplorer_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/internetExplorer_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void operaCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Opera issue for your system. click a button below to run solution. ";
            scriptname = "Opera_Check.exe";

            fileName = "C:\\temp\\Opera_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/Opera_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void unauthorizedSoftwareCheckToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix unauthorized software issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "unauthorizedSoftware_Check.exe";

            fileName = "C:\\temp\\unauthorizedSoftware_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/unauthorizedSoftware_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void operatingSystemCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix operating system issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Operating System Check";

            fileName = "C:\\temp\\os_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/os_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

        

        private void newJoinerLaptopSupportDeskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.IO.FileInfo fileObj = new System.IO.FileInfo
           Process.Start("https://quickitsupport.gdn.accenture.com//start_doc/newJoinerSupportDesk.pdf");
            executed = true;
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            //fileObj.Attributes = System.IO.FileAttributes.ReadOnly;
            //System.Diagnostics.Process.Start(fileObj.FullName);
        }

        private void dedicatedSupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com//start_doc/dedicatedSupport.pdf");
            executed = true;
        }

        private void instantChatWithTechnicalSupportDeskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://secure.logmeinrescue-enterprise.com/Customer/Download.aspx?EntryID=1644565702");
            executed = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void btnInstructions_Click(object sender, EventArgs e)
        {
            //executed = true;
            //scriptname = "Admin Rights";
            ////scriptname = "Patching Issue Fix";
            //subcategory.Add("Admin Rights");
            //saveExecutionLog("Admin Rights");
            //    MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Applications” folder." + Environment.NewLine + "3) Choose “Zero Touch Solution – Avecto Automation” Solution."
            //            + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "message");

            executed = true;
            scriptname = "Admin Rights";
            subcategory.Add("Admin Rights");
            saveExecutionLog("Admin Rights");
            Process.Start("https://ts.accenture.com/sites/ISAChina/Job%20Aid/Request%20Guildeline/Forms/AllItems.aspx?RootFolder=%2Fsites%2FISAChina%2FJob%20Aid%2FRequest%20Guildeline%2FStart%20Tool&FolderCTID=0x01200098853ABFAFA3774D826317E665BF994F&View=%7B2C9E78F9%2D5E10%2D4BF0%2D8E89%2DCE8E929C0A06%7D");

        }

        private void toolStripMenuItem90_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/how-do-i-connect-to-prisma-access-globalprotect-vpn-solution/");
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=6b9127821b324d50931677b51a4bcb4b");
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/windows-11-homepage/");
            executed = true;
        }

        private void txtDetails_TextChanged(object sender, EventArgs e)
        {

        }

        private void UnlockedPasswordtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/how-to-avoid-enterprise-account-lock-outs-after-a-password-change/");
            executed = true;
        }

        private void EnterpriseIdandPasswordtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/enterprise-id-and-password/");
            executed = true;
        }

        private void PasswordlessSupporttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add("Passwordless Support");
            saveExecutionLog("Passwordless Support");
            Process.Start("https://in.accenture.com/connectivitysecurity/passwordless-support/");
            executed = true;
        }

        private void SelfServerPasswordResettoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/self-service-password-reset-web-application/");
            executed = true;
        }

        private void SecurePasswordAndroidDevicetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/how-to-secure-my-android-device/");
            executed = true;
        }

        private void WifiTroubleshootingOnMobileDevicetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/wi-fi-connectivity-and-troubleshooting-on-your-mobile-device/");
            executed = true;
        }

        private void NetworkAccessSecuritytoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/network-access-security-initiative/");
            executed = true;
        }

        private void HomeNetworkConnectivitytoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/why-is-the-wireless-connection-so-slow/?section=bestpractices");
            executed = true;
        }

        private void OnboardWirelessAdaptertoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/determining-if-onboard-wireless-adapter-is-present/");
            executed = true;
        }

        private void RDPEnablementtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/rdp-enablement-support-site/");
            executed = true;
        }

        private void BitLockertoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/temporarily-suspend-bitlocker/");
            executed = true;
        }

        private void OneDriveforBusinesstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/onedriveforbusiness/");
            executed = true;
        }

        private void ClientandContractorAccesstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/client-contractor-access/");
            executed = true;
        }

        private void NewJoinerNeedtoKnowtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/new-joiners-things-you-need-to-know/");
            executed = true;
        }

        private void EmailProtectionupporttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/proofpoint-advanced-email-protection/");
            executed = true;
        }

        private void RulesforEnterprisePasswordtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/complexity-rules-for-enterprise-password/");
            executed = true;
        }

        private void PasswordResettoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/the-enterprise-directory-password-has-been-lost-or-forgotten/");
            executed = true;
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/delegate-access-troubleshooting-steps-how-to-remove-and-re-add-delegate-access/");
        }

        private void toolStripSoftwareCenterUpdateFix_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "此脚本解决方案将修复无法在软件中心安装或保持失败的更新/应用程序。使用此解决方案修复那些无法在软件中心安装应用程序/更新的问题。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "This Script solution will fix unable to install or keeps failing updates/applications on software center. Use this solution to fix those unable to install apps/updates on software center." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Software Center Update Fix";
            fileName = "C:\\temp\\Software_Center_Update_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Software_Center_Update_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void toolStripMenuSCCMNotWorkingFix_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "This Script solution will try to fix System Center Configuration Manager focusing on Non-Pooling compliance that is unable to communicate with our Domain Accenture causing our workstation to have a non-compliant if it's not healthy. Control Panel > Configuration Manager, if it shows 6 tabs, that means your SCCM is not healthy. Use this script to try to fix non-healthy SCCM to fully healthy SCCM."
                + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "SCCM Fix";
            fileName = "C:\\temp\\SCCM_Not_Working_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/SCCM_Not_Working_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void WindowsActiveFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;

            txtDetails.Text = "此脚本解决方案将修复我们的 Windows，该 Windows 显示一个蓝色水印，上面写着激活 Windows。使用此解决方案修复桌面上的激活问题。"
                + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";

            //txtDetails.Text = "This Script solution will fix our Windows that shows a blue watermark that says Activate Windows. Use this solution to remediate Activation issue on desktop."
            //    + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            //txtDetails.Text = "If you want to fix the Windows Activation issue, please click on button below." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows Activation Fix";
            fileName = "C:\\temp\\Windows_Activation_fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Windows_Activation_fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void HFBPinFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "此脚本解决方案将修复错误“PIN 当前不可用”的 HFB PIN。使用此解决方案来解决此问题。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "This Script solution will fix the HFB PIN that has an error 'PIN is currently Unavailable'. Use this solution to resolve this issue." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "HFB Pin Fix";
            fileName = "C:\\temp\\HFB_Pin_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/HFB_Pin_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void DefenderNotOpeningFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "此脚本解决方案将修复 Microsoft Defender 无法打开的应用程序。使用此解决方案修复无法打开 Microsoft Defender。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "This Script solution will fix for the application Microsoft Defender is unable to open. Use this solution to fix Unable to open Microsoft Defender." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Defender Not Opening Fix";
            fileName = "C:\\temp\\Defender_NotOpening.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Defender_NotOpening.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void SmartCardLogonFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "This Script solution will fix the Smart Card Logon on startup where your password was setup to passwordless and unable to login to your workstation. Use this script to follow the instructions how to enable back password setup."+
                Environment.NewLine+ "For more info, visit : https://mypasswordless.accenture.com/gopasswordless "+ Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Smart CardLogon Fix";
            fileName = "C:\\temp\\Smart_CardLogon_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Smart_CardLogon_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void toolStripMenuWindowsFileExploreFix_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "此脚本解决方案将尝试修复未显示或损坏的文件资源管理器功能区。使用此解决方案恢复其功能，功能区将可见。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "This Script solution will try to fix File Explorer Ribbon that's not showing or corrupted. Use this solution to restore its functionality and ribbon will be visible." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows File Explorer Ribbon Fix";
            fileName = "C:\\temp\\Windows_File_Explorer_Ribbon_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Windows_File_Explorer_Ribbon_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void toolStripRestoreandRepairSystemFix_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "此脚本解决方案将尝试修复系统文件损坏，确定图像是否可修复，检查是否检测到任何损坏，并修复它在您登录的操作系统中发现的问题。使用此解决方案尝试修复它。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "This Script solution will try to fix system file corruption, determines if the image is repairable, checks if there are any corruptions detected, and repairs problems that it finds with the operating system you are logged into. Use this solution to try to fix it." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Restore and Repair Corrupted System Files Fix";
            fileName = "C:\\temp\\Restore_and_Repair_Corrupted_System_File_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Restore_and_Repair_Corrupted_System_File_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void TaskbarFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "此脚本解决方案将修复没有响应、卡住或不稳定的 Windows 任务栏。使用此解决方案重置其功能。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "This Script solution will fix windows taskbar that are not responding, stuck, or unstable. Use this solution to reset back its functionality." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Taskbar Fix";
            fileName = "C:\\temp\\Taskbar_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Taskbar_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void ClearGroupPolicyCachetoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = " This Script solution will clear or remove any group policy cache on your machine. Use this solution that windows policies were not reflecting properly and perform group policy update afterwards." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Clear Group Policy Cache";
            fileName = "C:\\temp\\Clear_Group_PolicyCache.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Clear_Group_PolicyCache.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void IPUInstallFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "This Script solution will fix those In-Place Upgrade for Windows installation doesn't proceed. Use this solution to fix and proceed its installation." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "IPU Install Fix";
            fileName = "C:\\temp\\IPU_Unable_to_install_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/IPU_Unable_to_install_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void NetFrameworkFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "This Script solution will fix those applications that require .Net Framework pre requirements such as SAP. Use this solution to install .net Framework properly." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Net Framework Fix ";
            fileName = "C:\\temp\\Net_Framework_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Net_Framework_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void toolStripMenuWorkstationSystemIssue_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "Follow the On-Screen Instructions in case of any issues while connecting Accenture Wi-Fi or LAN." + Environment.NewLine+ "Ensure to connect Wi-Fi Internet/External Network and Restart the Laptop Post Execution of the solution." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
                //+Environment.NewLine+Environment.NewLine+"Once you Start Execution,"+Environment.NewLine+"*Click on Install button, Next and OK"+Environment.NewLine+
                //"*It will ask to Update, Click on Update Now"+Environment.NewLine+"*After Update click Install, Next and Ok to complete the Installation.";
            scriptname = "Windows Workstation Network Access Fix";
            fileName = "C:\\temp\\Windows_Workstation_Network_Access_Issue.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/Windows_Workstation_Network_Access_Issue.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void toolStripMenuVideoConference_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/videoconferencing/videoconferencing-systems/");
        }

        private void toolStripMenuSurfaceHub_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/videoconferencing/welcome-to-surface-hub-support-site/");
        }

        private void toolStripMenuWirelessContentSharing_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/videoconferencing/wireless-content-sharing/");
        }

        private void toolStripMenuIndicatorsOfAnExternalTenant_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com//start_doc/Indicators_of_an_external_tenant.pdf");
        }

        private void toolStripMenuAccessingAccentureAzureTenant_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com//start_doc/Accessing_Accenture_Azure_Tenants.pdf");
        }

        private void toolStripMenuSwitchingDirectories_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com//start_doc/Azure_MFA_Switching_Directories.pdf");
        }

        private void toolStripMenuSwitchingOrganizations_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com//start_doc/Azure_MFA_Switching_Organizations.pdf");
        }

        private void toolStripMenu_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = true;

            txtDetails.Text = "Access to Accenture Production or Staging tenants "+Environment.NewLine +
                Environment.NewLine + "If accessing Accenture Production or Accenture Staging tenants fails after remediation processes above do not work, users can submit a support request. Users should have all the appropriate evidence attached to the support request.";
                // + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            
            txtDetails.Visible = true;
            panel1.Visible = false;
            scriptname = "Log a request if tenants fails";
            btnFix.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void RASVPNRequesttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071256&sys_id=66d1ed5a1385ba4099f8dbf18144b077");
            executed = true;
        }

        private void NetworkProblemRepairtoolStripMenu_Click(object sender, EventArgs e)
        {
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "This Script solution will let you select the following details"+Environment.NewLine+ "Option 1: Displays all configuration information for each adapter bound to TCP/IP" 
                +Environment.NewLine+ "Option 2: Displays possible network problems data"+Environment.NewLine+ "Option 3: Clears network Cache, Release and renew an IP to your network"
                +Environment.NewLine+ "Option 4: Remove any proxy setup on your network." + Environment.NewLine +"Avecto Rights must be needed to execute the below solution.";
            scriptname = "Network Problem Diagnose Repair";
            fileName = "C:\\temp\\Network_Problem_Diagnose_Repair.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/ph_solutions/Network_Problem_Diagnose_Repair.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void AccessingMSPortalorAZTenanttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/Accessing_Azure_or_MS_Portal_Tenant.pdf");
            executed = true;
        }

        private void AzurePortalTenantRelatestoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = true;
            btnInstructions.Visible = false;
            panel1.Visible = false;
            btnFix.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            scriptname = "Azure MS Portal Access-Tenant related";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            txtDetails.Text = "用户访问 Azure 或其他 MS 服务门户时的用例" + Environment.NewLine +
                "*对于私人、项目、客户门户网站，但可以访问其他埃森哲网站。" + Environment.NewLine +
                "*提示用户 MFA 但身份验证失败或超时。" + Environment.NewLine +
                "*用户在登录过程中没有收到预期的 MFA 提示。" + Environment.NewLine + Environment.NewLine +
                "有关更多信息，请单击详细信息按钮";

            //txtDetails.Text = "Use Cases when users are accessing Azure or other MS Service Portals" + Environment.NewLine +
            //    "*For Private, Project, Customer Portals, but can access other Accenture Sites." + Environment.NewLine +
            //    "*Users are prompted MFA but the authentication is failing or timing out." + Environment.NewLine +
            //    "*Users are not receiving the expected MFA prompt during the sign in process."+Environment.NewLine+Environment.NewLine+
            //    "For more information click on details button";
        }

        private void btnAzure_Click(object sender, EventArgs e)
        {
            executed = true;
            scriptname = "Azure MS Portal Access-Tenant related";
            subcategory.Add("Azure MS Portal Access-Tenant related");
            saveExecutionLog("Azure MS Portal Access-Tenant related");
            Process.Start("https://quickitsupport.gdn.accenture.com/start_doc/Accessing_Azure_or_MS_Portal_Tenant.pdf");
        }

        private void ConfigureOutlookIOStoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/outlook-for-ios/");
            executed = true;
        }

        private void LostorStolenDevicestoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/lost-or-stolen-devices/");
            executed = true;
        }

        private void ConfigureOutlookAndroidDevicetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://cdn.in.accenture.com/prod/wp-content/uploads/sites/274/2021/10/08213405/Android-MAM_July_2021_azmfa.pdf");
            executed = true;
        }

        private void PasswordlessSupportFAQtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add("Passwordless Support FAQs");
            saveExecutionLog("Passwordless Support FAQs");
            Process.Start("https://in.accenture.com/connectivitysecurity/passwordless-faqs/");
            executed = true;
        }

        private void toolStripMenuItem144_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/how-to-enable-windows-10-hello/");
            executed = true;
        }

        private void toolStripMenuItemReturnAnAsset_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            //MessageBox.Show(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/r/sites/ISAChina/Job%20Aid/_layouts/15/Doc.aspx?sourcedoc=%7BEB9F4FEF-0D11-4ED0-B9FF-BD7FCB1EE1E9%7D&file=How%20to%20Return%20an%20Asset.docx&action=default&mobileredirect=true");
            executed = true;
        }

        private void toolStripMenuIDataRestoration_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            //Process.Start("https://ts.accenture.com/sites/ISAChina/JobAid/Request Guildeline/How to Return an Asset.docx")
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EcZM1s2j_UFFsDCbX3zM6-IBPlrL2Xe1TLV3Y2mU8UAG0A");
            executed = true;
        }

        private void toolStripMenuPhoneRequestInSnow_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/Ec9OgiVAnR5PneN4xaFf3QEB9pQU1aJqLhYWG8BPsF8YBQ");
            executed = true;
        }

        private void toolStripMenuWIFIGestSelfRegistration_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EXQSo1753z9Pmv5sYNzM-nUB1SWBumi4pNYwXrV2RohhHw");
            //Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/WIFI_Guest_Self-Registration.pdf");
            executed = true;
        }

        private void toolStripMenuWIFI_IOTSelfRegistration_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/ESAIt3OkZUVEq19ukNN1WIABjgLaCzO9wBvNH1WQ1YQGxA");
            //Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/WIFI_IOT_Self-Registration.pdf");
            executed = true;
        }

        private void toolStripMenuLaptopRequestInSNOW_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EWfB_IDgjLFLmp1NxW7GNbABiqDXQBLWZlzO8kmm1EuFUw");
            //Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Laptop_Request_in_SNOW.pdf");
            executed = true;
        }

        private void toolStripMenuDesktopPullOutReturnRequest_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/ERzU78vkxEpEmzYy8yAx8aAB3dhRAQkPZdD2tpHVzmm3aw");
            //Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Desktop_Pull_Out&Return_Request.pdf");
            executed = true;
        }

        private void toolStripMenuDesktopRequestInSNOW_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/ERHDn4EvHp1OrUAqs5_CzkEBWrv-JbDZBhyaLrkQRoQAXQ");
            executed = true;
        }

        private void toolStripMenuHardwareInstallationServiceRequest_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EWcoOSNiHyxHuaPRnqe9N_wBfdqp3DV3PvC7Oqg92TRWkw");
            executed = true;
        }

        private void toolStripMenuRequestForAssetDevice_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/r/sites/ISAChina/Job%20Aid/_layouts/15/Doc.aspx?sourcedoc=%7B89FCF1FC-15EC-4BCB-9A8E-C033A1995ABC%7D&file=Request%20For%20Asset%20Device.docx&action=default&mobileredirect=true");
            executed = true;
        }

        private void toolStripMenuShareFolderRequestInSNOW_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/ERVyxNwaGv9FlUUeMAO38tMB-b19DwEeyfgYMB5uVcfjeg");
            executed = true;
        }

        private void toolStripMenuTeamsPERequest_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/ET-tHJTCNPlGqu7Sli9MfXsBGC2xvIcBKSdP9k5NpwRDbw");
            executed = true;
        }

        private void toolStripMenuTeamRequestInSnow_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            //Process.Start("https://ts.accenture.com/:w:/r/sites/ISAChina/Job%20Aid/_layouts/15/Doc.aspx?sourcedoc=%7BDD94E499-D63C-4455-8B16-912F865986D4%7D&file=Teams%20Request%20in%20snow.docx&action=default&mobileredirect=true&cid=5fda8084-df6e-4bd9-a13a-1451af12e42d");
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EZnklN081lVEixaRL4ZZhtQBbCygxZt_0Bl5k2jwIpjQcA");
            executed = true;
        }

        private void toolStripMenuITAssets_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EdB26pI2PMxIsOVHsYd5TqEB8ZciMLWlaOO2KJDUYHZjFQ");
            executed = true;
        }

        private void toolStripMenuSoftwareInstallationRequest_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/:w:/s/ISAChina/Job%20Aid/EazYSwKeTxtIrGYRz2FYCZsBEFKBJlY6hUOLBtXKnnqPMA");
            //Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Share_Point_Request_Update.pdf");
            executed = true;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ISAChina/ISA%20Guides/SitePages/AZURE%20MFA.aspx");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ISAChina/ISA%20Guides/SitePages/Password.aspx");
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ISAChina/ISA%20Guides/SitePages/B.aspx");
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ISAChina/ISA%20Guides/SitePages/Awareness-Workstation-Compliance.aspx");
        }

        private void GuideForSubmissionOfRelevantIttoolStripMenu_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ISAChina/ISA%20Guides/SitePages/The-Submission-Of-Relevant-IT-Permission.aspx");
        }

        private void HFBPINtoolStripMenu_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/ISAChina/ISA%20Guides/SitePages/Hello-For-Business-PIN.aspx");
        }

        private void Windows11ClassicRighClickFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "如果您想修复 Windows 11 经典右键单击问题，请单击下面的按钮。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "If you want to Fix the Windows 11 Classic Right Click Issue, PLease Click on the button below." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows 11 Classic Right Click Fix";
            fileName = "C:\\temp\\Windows11_Classic_right_click_feature.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/china_solutions/Windows11_Classic_right_click_feature.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void PulseSecureReinstallationFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "如果您想修复 PulseSecure 重新安装问题，请单击下面的按钮。" + Environment.NewLine + "必须需要 Avecto Rights 才能执行以下解决方案。";
            //txtDetails.Text = "If you want to Fix the PulseSecure Reinstallation Issue, PLease Click on the button below." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "PulseSecure Reinstallation Fix";
            fileName = "C:\\temp\\PulseSecure_Reinstallation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/china_solutions/PulseSecure_Reinstallation.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void HFBManualEnrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("HFB Manual Enrollment");
            saveExecutionLog("HFB Manual Enrollment");
            Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/ACN_Hfb_Manual_Enrollment_utility_Instructions.pdf");
            executed = true;
        }

        private void toolStripMenuHFBAutoEnroll_Click(object sender, EventArgs e)
        {
            subcategory.Add("HFB Automated Enrollment");
            saveExecutionLog("HFB Automated Enrollment");
            Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/ACN_Hfb_Automated_Enrollment_utility_Instructions.pdf");
            executed = true;
        }

        private void toolStripMenuTapReqest_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add("TAP Request");
            saveExecutionLog("TAP Request");
            Process.Start("https://mypasswordless.accenture.com/tap");
        }

        private void toolStripMenuGoPasswordless_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://mypasswordless.accenture.com/gopasswordless/enable-password");
            executed = true;
        }

        private void toolStripMenuMSAuthApp_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/ms-authenticator-app-end-user-guide/");
            executed = true;
        }

        private void toolStripMenuManagingSigninMethod_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/azure-mfa-and-self-service-password-reset-sspr/");
            executed = true;
        }

        private void toolStripMenuMSAuthAppEndUser_Click(object sender, EventArgs e)
        {
            subcategory.Add("MS Authenticator App End User Guid");
            saveExecutionLog("MS Authenticator App End User Guid");
            Process.Start("https://in.accenture.com/connectivitysecurity/ms-authenticator-app-end-user-guide/");
            executed = true;
        }

        private void FIDO2SetupandregistrationtoolStrip_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = true;
            btnHfbT2.Visible = true;
            btnHfbT3.Visible = true;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            panel1.Visible = false;
            btnFix.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            scriptname = "FIDO2 Set-up and Registration";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //txtDetails.Text = "This guide highlights support material that the user can use to set-up and register a FIDO2. Resources are also shared to learn more about FIDO2 and how to seek support." + Environment.NewLine + Environment.NewLine +
            //    "Table 1: FIDO2 Token Setup and Registration for Windows (OS)" + Environment.NewLine + "Table 2: FIDO2 Token Setup and Registration for MAC (OS)" + Environment.NewLine + "Table 3: Registering a FIDO2 Token Key for Privileged Accounts";
            txtDetails.Text = "本指南重点介绍了用户可用于设置和注册 FIDO2 的支持材料。还共享资源以了解有关 FIDO2 的更多信息以及如何寻求支持。" + Environment.NewLine + Environment.NewLine +
               "表 1：Windows（操作系统）的 FIDO2 令牌设置和注册" + Environment.NewLine + "表 2：MAC（操作系统）的 FIDO2 令牌设置和注册" + Environment.NewLine + "表 3：为特权帐户注册 FIDO2 令牌密钥";
        }

        private void OrderingFIDO2TokentoolStrip_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = true;
            btnHfbT2.Visible = true;
            btnHfbT3.Visible = true;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            panel1.Visible = false;
            btnFix.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            scriptname = "Ordering a FIDO2 Token";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //txtDetails.Text = "This guide highlights support material that the user can use to order a FIDO2 token on the BuyNow (Ariba) site. Resources are also shared to learn more about FIDO2 and how to seek support." + Environment.NewLine + Environment.NewLine +
            //    "Table 1: Recommended Tokens" + Environment.NewLine + "Table 2: Users unable to locate their shipping address during profile setup" + Environment.NewLine + "Table 3: Search results indicate that the Feitian products are not found";

            txtDetails.Text= "本指南重点介绍了用户可用于在 BuyNow (Ariba) 网站上订购 FIDO2 令牌的支持材料。还共享资源以了解有关 FIDO2 的更多信息以及如何寻求支持。" + Environment.NewLine + Environment.NewLine +
                "表 1：推荐代币" + Environment.NewLine + "表 2：用户在配置文件设置期间无法找到他们的送货地址" + Environment.NewLine + "表3：搜索结果显示未找到飞天产品";
        }

        private void HfbPinResetoptionstoolStrip_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            //txtDetails.Text = "The HfB PIN reset options help guide offers different ways user can reset their hello for Business PIN: This document also support methods for both Passwordless and Non passwordless users."
              //  + Environment.NewLine + Environment.NewLine + "Table1: Using the System Settings if your desktop is in unlocked screen mode" + Environment.NewLine + "Table2: Passwordless is Enabled: Using System Settings from the Unlocked screen" + Environment.NewLine + "Table3: Passwordless is Enabled: PIN Reset from the Locked Screen.";
            txtDetails.Text = "HfB PIN 重置选项帮助指南提供了用户可以重置其业务 PIN 的不同方式：本文档还支持无密码和非无密码用户的方法。"
                + Environment.NewLine + Environment.NewLine + "表 1：如果您的桌面处于解锁屏幕模式，则使用系统设置" + Environment.NewLine + "表 2：启用无密码：从解锁屏幕使用系统设置" + Environment.NewLine + "表 3：启用无密码：从锁定屏幕重置 PIN。";
            scriptname = "HfB PIN Reset Options";
            btnHfbPin.Visible = true;
            btnHfbT2.Visible = true;
            btnHfbT3.Visible = true;
            //fileName = "C:\\temp\\HFB_Pin_Fix.exe";
            //remoteUri = "https://quickitsupport.gdn.accenture.com//solution/HFB_Pin_Fix.exe";
            //string myStringWebResource = null;
            //// Create a new WebClient instance.

            //// Concatenate the domain with the Web resource filename.
            //myStringWebResource = remoteUri;
            //// myWebClient.DownloadFile(myStringWebResource, @fileName);
            //txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = false;
        }

        private void btnHfbT1(object sender, EventArgs e)
        {
            if (scriptname == "HfB PIN Reset Options")
            {
                executed = true;
                subcategory.Add("HfB PIN Reset Options");
                saveExecutionLog("HfB PIN Reset Options");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Using_the_system_setting_if_your_desktop_is_in_unlocked_screen_mode.pdf");
            }
            if (scriptname == "FIDO2 Set-up and Registration")
            {
                executed = true;
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/FIDO2_Token_Setup_and_Registration_for_Windows_(OS).pdf");
            }
            if (scriptname == "Ordering a FIDO2 Token")
            {
                executed = true;
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Recommended_Tokens.pdf");
            }
        }

        private void btnHfbT2_Click(object sender, EventArgs e)
        {
            if (scriptname == "HfB PIN Reset Options")
            {
                executed = true;
                subcategory.Add("HfB PIN Reset Options");
                saveExecutionLog("HfB PIN Reset Options");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Passwordless_Enabled_Using_the_system_settings_from_the_unlocked_screen.pdf");
            }
            if (scriptname == "FIDO2 Set-up and Registration")
            {
                executed = true;
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/FIDO2_Token_Setup_and_Registration_for_MAC_(OS).pdf");
            }
            if (scriptname == "Ordering a FIDO2 Token")
            {
                executed = true;
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Users_unable_to_locate_their_shipping_address_during_profile_setup.pdf");
            }
        }

        private void btnHfbT3_Click(object sender, EventArgs e)
        {
            if (scriptname == "HfB PIN Reset Options")
            {
                executed = true;
                subcategory.Add("HfB PIN Reset Options");
                saveExecutionLog("HfB PIN Reset Options");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Passwordless_Enabled_Pin_Reset_from_the_locked_screen.pdf");
            }
            if (scriptname == "FIDO2 Set-up and Registration")
            {
                executed = true;
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Registering_a_FIDO2_Token_Key_for_Privileged_Accounts.pdf");
            }
            if (scriptname == "Ordering a FIDO2 Token")
            {
                executed = true;
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                Process.Start("https://quickitsupport.gdn.accenture.com/china_doc/Search_results_indicate_that_the_Feitian_products_are_not_found.pdf");
            }
        }

        private void toolStripMenuItem146_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/");
            executed = true;
        }

        private void HomeFormCHN_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (executed)
            {
                // subcategory.Add("text");
                FeedbackFormCHN feedback = new FeedbackFormCHN(subcategory, categoryData);
                feedback.ShowDialog();
                if (feedback.submited == false && checkdwnld == false)
                {
                    e.Cancel = true;
                    feedback.Close();
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void toolStripMenuReachTool_Click(object sender, EventArgs e)
        {
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "从下面下载埃森哲内部远程工具 (REACH)。" + Environment.NewLine + Environment.NewLine + "选项 1：REACH 套餐。" + Environment.NewLine + "选项 2：带有 Windows 运行时的 REACH 软件包。";
            //txtDetails.Text = "Download Accenture Inhouse Remote Tool (REACH) from the below." + Environment.NewLine + Environment.NewLine + "Option 1: REACH Package." + Environment.NewLine + "Option 2: REACH Package with Windows Runtime.";
            scriptname = "REACH Tool";
            fileName = "C:\\temp\\REACH.zip";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/REACH.zip";

            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //saveExecutionLog(sender.ToString());
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btndownload.Visible = true;
            btndownloadruntime.Visible = true;
        }

        private void MyWebClient_Downloadreach(object sender, AsyncCompletedEventArgs e)
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }

            isdownloaded = true;
            allow = "true";
            var reach = MessageBox.Show("REACH Tool Downloaded", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (reach == DialogResult.OK)
            {
                //executed = true;
                subcategory.Add("REACH Tool");
                saveExecutionLog("REACH Tool");
                Process.Start("C:\\temp");
                enableControl();
            }

        }

        private void btndownload_Click(object sender, EventArgs e)
        {
            bool ifFileExist = true;

            if (fileName != null)
            {
                panel1.Visible = true;
                try
                {

                    HttpWebRequest request = WebRequest.Create(remoteUri) as HttpWebRequest;
                    request.Method = "HEAD";
                    using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                    {
                        ifFileExist = true;
                    }

                }
                catch (WebException we)
                {

                    ifFileExist = false;

                }


                if (ifFileExist)
                {
                    menuStrip2.Enabled = false;
                    btnFix.Enabled = false;
                    panel7.Enabled = false;

                    checkdwnld = true;

                    try
                    {

                        RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.Reload);
                        WebClient myWebClient = new WebClient();
                        myWebClient.CachePolicy = policy;
                        myWebClient.DownloadProgressChanged += MyWebClient_DownloadProgressChanged;
                        myWebClient.DownloadFileCompleted += MyWebClient_Downloadreach;
                        thread = new Thread(() =>
                        {
                            Uri url = new Uri(remoteUri);

                            myWebClient.DownloadFileAsync(url, @fileName);

                        });
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        enableControl();
                        MessageBox.Show("Please do not try to execute more than one script");
                    }

                }
                else
                {
                    MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                }

            }
            if (path != null)
            {
                enableControl();
                allow = "true";
                executeScript();
            }
        }

        private void btndownloadruntime_Click(object sender, EventArgs e)
        {
            bool ifFileExist = true;

            if (fileName != null)
            {
                panel1.Visible = true;
                try
                {

                    HttpWebRequest request = WebRequest.Create(remoteUri) as HttpWebRequest;
                    request.Method = "HEAD";
                    using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                    {
                        ifFileExist = true;
                    }

                }
                catch (WebException we)
                {

                    ifFileExist = false;

                }


                if (ifFileExist)
                {
                    fileName = "C:\\temp\\REACH_runtime.zip";
                    remoteUri = "https://quickitsupport.gdn.accenture.com//solution/china_solutions/REACH_runtime.zip";

                    menuStrip2.Enabled = false;
                    btnFix.Enabled = false;
                    panel7.Enabled = false;

                    checkdwnld = true;

                    try
                    {

                        RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.Reload);
                        WebClient myWebClient = new WebClient();
                        myWebClient.CachePolicy = policy;
                        myWebClient.DownloadProgressChanged += MyWebClient_DownloadProgressChanged;
                        myWebClient.DownloadFileCompleted += MyWebClient_Downloadreach;

                        //myWebClient.DownloadFileAsync(url, @fileName+DateTime.Now.ToString("HH_mm_ss"));*/

                        //send data to api


                        thread = new Thread(() =>
                        {
                            Uri url = new Uri(remoteUri);

                            myWebClient.DownloadFileAsync(url, @fileName);

                        });
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        enableControl();
                        MessageBox.Show("Please do not try to execute more than one script");
                    }

                }
                else
                {
                    MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                }

            }
            if (path != null)
            {
                enableControl();
                allow = "true";
                executeScript();
            }
        }

        private void btnReachtool_Click(object sender, EventArgs e)
        {

        }

        private void cORAToolForTeamsIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            //txtDetails.Text = "Please download Communicator Optimization and Remediation Assistant (CORA) tool from below SAC Portal."+Environment.NewLine+
            //    "https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=8eead6ae1b2348d0ae30dd77cc4bcb4b"+Environment.NewLine+
            //    "CORA will help to fix all basic MS team issues";

            txtDetails.Text = "请从以下 SAC 门户下载 Communicator Optimization and Remediation Assistant (CORA) 工具。" + Environment.NewLine +
                "https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=8eead6ae1b2348d0ae30dd77cc4bcb4b" + Environment.NewLine +
                "CORA 将帮助解决所有基本的 MS 团队问题";

            scriptname = "CORA Tool";

            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //saveExecutionLog(sender.ToString());
        }

        private void fromLaptopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            subcategory.Add("Change Password");
            saveExecutionLog("Change Password");
            panel1.Visible = false;
            btnFix.Visible =  false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
           // scriptname = "AzureMFA";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            //txtDetails.Text = "* Click on “Ctrl+Alt+Delete” on your laptop to change the password." + Environment.NewLine +
            //                      "* Recommended to connect Accenture VPN to avoid password sync issues.";

            txtDetails.Text = "* 在您的笔记本电脑上点击“Ctrl+Alt+Delete”来更改密码。" + Environment.NewLine +
                "* 建议连接 Accenture VPN 以避免密码同步问题。";


        }

        private void frequentlyFacedIssuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString()); 
            saveExecutionLog(sender.ToString());
            Process.Start("https://sway.office.com/E3at8rC5P2rr5FPG");
            executed = true;
        }

        private void accentureLaptopConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://sway.office.com/cCB2IYf0aJa2e4MB?ref=Link");
            executed = true;
        }

       
        private void gPHFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "If you want to fix GPH issue click on the button below. Avecto Rights must be needed to execute the below solution.";
            scriptname = "GPH Fix";

            fileName = "C:\\temp\\GPH_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/GPH_Remediation.exe";

            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

       
      /*  private void hideMenu()
        {
            if (subComplianceIssue.Visible == true)
            {
                subComplianceIssue.Visible=false; 
            }

            if (subQuickSolution.Visible == true) 
            {
                subQuickSolution.Visible=false;
            }

            if (subSystemIssue.Visible == true)
            {
                subSystemIssue.Visible = false;
            }
            if (subApplication.Visible == true)
            {
                subApplication.Visible = false;
            }
            if (subNtwIssue.Visible == true)
            {
                subNtwIssue.Visible = false;
            }
            if(subQuickLinks.Visible==true)
            {
                subQuickLinks.Visible = false;
            }
            string x = subSystemEnablment.Parent.Name;
        }
        private void showMenu(Panel submenu,string parent)
        {
            if (submenu == subApplication)
            {
                if(subApplication.Visible==false)
                {
                    hideMenu();
                    submenu.Visible = true;
                    subMSTeams.Visible = false;
                    subMSApp.Visible = false;
                    subVPN.Visible = false;
                    subOneDrive.Visible = false;
                }
                else
                {
                    submenu.Visible = false;
                }
            }
            if (parent == subApplication.ToString())
            {
                subApplication.Visible = true;

                if (submenu.Visible == true)
                {
                    submenu.Visible = false;
                }
                else
                {
                    submenu.Visible = true;

                }
                if (submenu != subMSTeams)
                { 
                subMSTeams.Visible = false;
                }
                if (submenu != subMSApp)
                {
                    subMSApp.Visible = false;
                }
                if (submenu != subVPN)
                {
                    subVPN.Visible = false;
                }
                if (submenu != subOneDrive)
                {
                    subOneDrive.Visible = false;
                }
            }

            if (submenu ==subNtwIssue)
            {
                if (subNtwIssue.Visible == false)
                {
                    hideMenu();
                    submenu.Visible = true;
                    SubWifiReg.Visible = false;
                   
                }
                else
                {
                    subNtwIssue.Visible = false;
                }
            }
            if (parent == "subNtwIssue")
            {
                subNtwIssue.Visible = true;

                if (submenu.Visible == true)
                {
                    submenu.Visible = false;
                }
                else
                {
                    submenu.Visible = true;

                }
            }

            if (submenu ==SubAssetRelates)
            {
                if (SubAssetRelates.Visible == false)
                {
                    hideMenu();
                    submenu.Visible = true;
                   subAssetDecom.Visible=false;
                    subSystemEnablment.Visible = false;
                }
                else
                {
                    SubAssetRelates.Visible = false;
                }
            }
            if (parent == "SubAssetRelates")
            {
                SubAssetRelates.Visible = true;

                if (submenu.Visible == true)
                {
                    submenu.Visible = false;
                }
                else
                {
                    submenu.Visible = true;

                }
                if (submenu != subAssetDecom)
                {
                    subAssetDecom.Visible = false;
                }
                if (submenu != subSystemEnablment)
                {
                    subSystemEnablment.Visible = false;
                }
              
            }
            if (submenu == subQuickSolution || submenu == subSystemIssue || submenu == subComplianceIssue|| submenu==subVC||submenu==subQuickLinks)
                if (submenu.Visible == false)
                {
                    hideMenu();
                    submenu.Visible = true;
                }
                else
                {
                    submenu.Visible = false;
                }

        }*/
    }
}
