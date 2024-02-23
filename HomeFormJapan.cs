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

namespace START_Tool
{
    public partial class HomeFormJapan : Form
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

        public HomeFormJapan(DataTable catdt )
        {
            categoryData = catdt;   
            InitializeComponent();
           // this.DoubleBuffered = true;
        }
        public HomeFormJapan()
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

                        var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_japan/savelog.php", "POST", senddata);
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


        }

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
                     /*   using (Download downloadObj = new Download()) 
                        {
                            downloadObj.ShowDialog(this);
                        }
                        //Uri url = new Uri(remoteUri);*/
                        RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.Reload);
                        WebClient myWebClient = new WebClient();
                        myWebClient.CachePolicy = policy;
                        myWebClient.DownloadProgressChanged += MyWebClient_DownloadProgressChanged;
                        myWebClient.DownloadFileCompleted += MyWebClient_DownloadFileCompleted;

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
                        /*
                                                ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
                                                si.CreateNoWindow = true;
                                                si.FileName = Application.StartupPath + "//script//test.bat";
                                                si.Arguments = fileName;
                                                if (scriptname != "Avecto_Admin_Access_Automation")
                                                {
                                                    si.Verb = "runas";
                                                }
                                                //si.UseShellExecute = true;
                                                Process process = System.Diagnostics.Process.Start(si);


                                                si = new System.Diagnostics.ProcessStartInfo();
                                                si.CreateNoWindow = true;
                                                si.FileName = Application.StartupPath + "//script//end.bat";
                                                si.Arguments = fileName;

                                                Process process1 = Process.Start(si);
                                                process1.EnableRaisingEvents = true;
                                                process1.Exited += (sender, e) => {
                                                    if (scriptname == "DiskSpace_Cleanup")
                                                    {
                                                        showMessage("Disk Space Solution Execute in the background," + Environment.NewLine + " Check Disk space after 15 mins");
                                                    }

                                                    showMessage("Execution started, Please wait " + Environment.NewLine + "till the execution complete successfully.");

                                                    // MessageBox.Show("Execution started, Please wait till the execution complete successfully.", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                                    fileName = null;

                                                };*/

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


                  /*  if (scriptStr != null)
                    {
                        if (isbat)
                        {

                            ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
                            si.CreateNoWindow = true;
                            si.FileName = Application.StartupPath + "//script//Wifi-enablement.bat";
                            si.Verb = "runas";
                            //si.Arguments = FileName;
                            //si.UseShellExecute = true;
                            Process process = System.Diagnostics.Process.Start(si);
                            process.EnableRaisingEvents = true;
                            process.Exited += (sender, e) =>
                            {
                                showMessage("Execution started, Please wait " + Environment.NewLine + "till the execution complete successfully.");
                                // MessageBox.Show("Execution started, Please wait till the execution complete successfully", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                fileName = null;
                                // panel1.Visible = false;



                            };
                            executed = true;
                        }
                        else
                        {
                            MessageBox.Show("Alert! Solution is getting executed please be patient. You will get notification once execution completed.");
                            PowerShell ps = PowerShell.Create();
                            string scriptStr = File.ReadAllText(@path);
                            ps.AddScript(scriptStr).Invoke();
                            path = null; scriptStr = null;
                            fileName = null;
                            // MessageBox.Show("Execution started, Please wait till the execution complete successfully.");
                            showMessage("Execution started, Please wait " + Environment.NewLine + "till the execution complete successfully.");
                            executed = true;
                            

                        }

                    }*/
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
            txtDetails.Text = "一時ファイルやプリフェッチ、ごみ箱やキャッシュをクリアすることで、ディスクの空き容量を増やすことができます。\"Prepare Solution\" クリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+
                "If you are facing problem regarding full disk space. You can remediate this issue just by clearing space from temp, prefetch, recycle bin, windows cache. If you wish to free up disk space, click on button below. AvectoRights must be needed to execute the below solution.";
            scriptname = "Free Disk Space";

            fileName = "C:\\temp\\DiskSpace_Cleanup.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/DiskSpace_Cleanup.exe";
            string myStringWebResource = null;

            myStringWebResource = remoteUri;
            //  myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

       

        private void unsecureDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "If you are facing problem regarding unsecure device as if it is showing on screen of your machine. You can remediate this issue just by running a script.";
            scriptname = "Unsecure Device";
            fileName = "C:\\temp\\Unsecured_device_fix.exe"; 
             remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Unsecured_device_fix.exe";
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
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Removal_of_Unauthorised_user_EID_from_Avecto_Group.exe";
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
            txtDetails.Text = "保存されたパスワードを資格情報やブラウザの履歴から削除するには、\"Prepare Solution\"クリックしてください。実行には、管理者権限が必要です。" + Environment.NewLine+Environment.NewLine+

                "Do you want to clear saved passwords from credential manager and browser history? Click on the button below to continue. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Delete Saved Password";

            fileName = "C:\\temp\\Delete_Saved_Passwords.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Clear_Saved_Passwords.exe";
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

            txtDetails.Text = "お使いのPCに最新のパッチを適用するには、\"Prepare Solution\" をクリックし、管理者権限で実行してください。\r\n※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n※ポータル上のコンプライアンス状態への反映には48～72時間かかります。" + Environment.NewLine +Environment.NewLine+
                
                "To make your device compliant with patching parameter, please click on button below. Please save all your work, system will restart after execution. AvectoRights must be needed to execute the below solution." + Environment.NewLine +Environment.NewLine+
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";
//" if Script is not executed, follow the below Instructions."+Environment.NewLine+

            //"Go to “Windows Search” and type “Software Center”"+Environment.NewLine+
            //"and then Select “Applications”"+ Environment.NewLine+
            //"and Run “Zero Touch Solutions - Patch NC Remediation”." +Environment.NewLine+
//Environment.NewLine+ "Replication on portal takes 48 to 72 business hours post execution of the solution.";
           // txtDetails.AppendText()
            //Replication on portal takes 48 to 72hrs post execution of the script
            scriptname = "Patching Issue Fix";

            fileName = "C:\\temp\\Patch_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Patch_NC_Remediation.exe";
            string myStringWebResource = null;

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "To make your device compliant with polling paramater, please click on button below."
                + Environment.NewLine +
                "Please save all your work, system will restart after execution. " + Environment.NewLine
                +Environment.NewLine+Environment.NewLine+
                "Replication on portal takes 48 to 72 business hours post execution of the solution." + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Polling Fix";

            fileName = "C:\\temp\\Patch_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Patch_NC_Remediation.exe";
            string myStringWebResource = null;

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Red;
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
            txtDetails.Text = "暗号化のコンプライアンス違反を解消するには、下のボタンをクリックしてください。\r\n　※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n　※実行するには、管理者権限が必要です。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine+Environment.NewLine+

                "If you want to fix encryption Noncompliance issue, please click on the button below." +Environment.NewLine +
                "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + 
               Environment.NewLine+ Environment.NewLine+ "Replication on portal takes 48 to 72 business hours post execution of the solution.";
       // https://mytechhelp.accenture.com/downloadExe/EncryptionRemediation/EnterpriseId
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            scriptname = "Encryption Fix";
            panel1.Visible = false;
            fileName = "C:\\temp\\Encryption_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Encryption_NC_Remediation.exe";
            string myStringWebResource = null;

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
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
           // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "ウェブブラウザ Google Chrome を更新しますか?下のボタンをクリックしてください。以下のソリューションを実行するには、Avecto Rights が必要です。" + Environment.NewLine + Environment.NewLine +
                "ポータルでのレプリケーションには、ソリューションの実行後 48 ～ 72 営業時間かかります。" + Environment.NewLine + Environment.NewLine +
            "Do you want to update web browser google chrome? Click on the button below. Avecto Rights must be needed to execute the below solution." + Environment.NewLine+Environment.NewLine+
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            scriptname = "Chrome Fix";

            fileName = "C:\\temp\\GoogleChrome_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/GoogleChrome_NC_Remediation.exe";


            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
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
            txtDetails.Text = "Forcepointのコンプライアンス違反を解消するには、\"Prepare Solution\"をクリックしてください。\r\n※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n※実行するには、管理者権限が必要です。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine +Environment.NewLine+

                "If you want to update Forcepoint(web secure)/fix noncompliance, click on the button below." +Environment.NewLine+
                "Please save all your work, system will restart after execution. AvectoRights must be needed to execute the below solution." +Environment.NewLine+Environment.NewLine+
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            txtDetails.Visible = true;
            fileName = "C:\\temp\\ForcePoint_Compliance_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/ForcePoint_Compliance_Check.exe";
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
            txtDetails.Text = "Taniumのコンプライアンス違反を解消するには、\"Prepare Solution\"をクリックしてください。\r\n※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n※実行するには、管理者権限が必要です。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine + Environment.NewLine +

                "If you wish to update Tanium Noncompliance issue, click on button below." +Environment.NewLine+ "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine+ Environment.NewLine+
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            scriptname = "Tanium Fix";

            fileName = "C:\\temp\\TaniumCheck.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/TaniumCheck.exe";
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
            txtDetails.Text = "EVTA/非準拠EVTAを修正するには、下のボタンをクリックしてください。\r\n　※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n　※実行するには、管理者権限が必要です。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine+ Environment.NewLine +

                "Do you want to update EVTA/fix Noncompliance EVTA?" +
                "Please click on button below."+Environment.NewLine+ "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine+ Environment.NewLine +
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            scriptname = "EVTA Fix";

            fileName = "C:\\temp\\EEVA_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/EVTA_NC_Remediation.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void office365FixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnAzure.Visible = false;
            txtDetails.Visible = true;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "Office365を更新し、コンプライアンス違反を修正するには\"Prepare solution\"をクリックし、管理者権限で実行してください。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine + Environment.NewLine +
                "Do you want to update Office365/fix Noncompliance? please click on button below. AvectoRights must be needed to execute the below solution." + Environment.NewLine +
                "Replication on portal takes 48 to 72 business hours post execution of the solution.";


            //txtDetails.Text = "Do you want to update Office365/fix Noncompliance? please click on button below. Avecto Rights must be needed to execute the below solution." + Environment.NewLine+ Environment.NewLine+
              //  "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            scriptname = "Office 365 Fix";

            fileName = "C:\\temp\\Office365_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Office365_NC_Remediation.exe";
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
            txtDetails.Text = "Outlookアプリに不具合があり、プロファイルを再作成するには\"Prepare Solution\"をクリックします。実行には管理者権限が必要です。" + Environment.NewLine+Environment.NewLine+
                "Alert! Do you want to delete your outlook profile and recreate it again? To recreate outlook profile, click on the button below to continue. AvectoRights must be needed to execute the below solution.";
            scriptname = "Reconfigure Outlook";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            fileName = "C:\\temp\\Outlook_Profile_Deletion_Creation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Outlook_Profile_Deletion_Creation.exe";
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
            txtDetails.Text = "Office 365を再インストールする場合は\"Prepare Solution\"をクリックしてください。実行するには、管理者権限が必要です。" + Environment.NewLine+Environment.NewLine+
                "If you want to reinstall Office 365 click on the button below. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Office 365 Reinstallation";

            fileName = "C:\\temp\\Office365_Reinstall.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Office365_Reinstall.exe";
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
            if (executed)
            {
                // subcategory.Add("text");
                FeedbackFormJapan feedback = new FeedbackFormJapan(subcategory, categoryData);
                feedback.ShowDialog();
                if (feedback.submited == false && checkdwnld == false)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Environment.Exit(0);
            }

        }

        private void resetPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("Reset-Unlock Password");
            saveExecutionLog("Reset-Unlock Password");
            //Process.Start("https://passwordreset.microsoftonline.com/?whr=accenture.com");
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs20101225/");
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
            btnInstructions.Visible = true;
            //txtDetails.Text = "Are you from ATCI ? If Yes continue to run script If From AO then raise request for necessary approval.If solution does" +
            //    " not work for you then follow the given instructions."+Environment.NewLine+
            //    "Follow the following steps to execute solution from software center"+Environment.NewLine+ "1. Go to Software center. " + Environment.NewLine
            //    + "2.Search applications and then execute zero touch Solution-Avecto admin rights." + Environment.NewLine + "3. Avecto admin access automation";
            txtDetails.Text = "Avecto Admin Rights Solution which is available here is only for “ATCI Users”, Other Business units must follow the Security guidelines to get admin Access, Reach out to Project AMT for More details.";
            // tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            
            //txtDetails.Text = "If  you want to fix issue regarding Protect my Tech(PMT) tool click on the button below";
            scriptname = "Admin Rights";
            //remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Avecto_Admin_Access_Automation_V1.01.exe";
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
            Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/MS_Teams_Booking_Procedure_1.2.pdf");
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
            //Process.Start("https://amrau1101.accenture.com/guest/guest_index.php");
            saveExecutionLog(sender.ToString());
            executed = true;
        }

        private void registrationLink2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://apaau1102.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            //Process.Start("https://amrau1102.accenture.com/guest/guest_index.php");
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            executed = true;

        }

        private void registrationLink3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://amrau1101.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            //Process.Start("https://amrau1103.accenture.com/guest/guest_index.php");
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            executed = true;

        }

        private void registrationLink4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://amrau1102.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            //Process.Start("https://amrau1104.accenture.com/guest/guest_index.php");
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            executed = true;

        }

        private void registrationLink5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
            Process.Start("https://emeau1101.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            //Process.Start("https://amrau1105.accenture.com/guest/guest_index.php");
            executed = true;
        }

        private void registrationLink6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://emeau1102.accenture.com/guest/Restricted_Guest_Registration.php?_browser=1");
            //Process.Start("https://amrau1106.accenture.com/guest/guest_index.php");
            executed = true;

        }

        private void vPNAccessRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs2009070201/");
            //Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0075158&sys_id=1affe8ad37690a00812bd5c543990e32");
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
            txtDetails.Text = "Power BIリクエストを更新する方法" + Environment.NewLine +
                "① https://support.accenture.com/support_portal を開きます。" + Environment.NewLine +
                "②Technology Supportをクリックします・" + Environment.NewLine +
                "③左メニューから'Browse All Services'を選択します。" + Environment.NewLine +
                "④Productivity Toolsの下の‘Reporting & Analytics’を選択します。" + Environment.NewLine +
                "⑤‘Microsoft Power BI Premium’ を選択します。" + Environment.NewLine +
                "⑥'Request Action'をクリックし、'Upgrade exisiting subscription'を選択します。" + Environment.NewLine +
                "⑦'Executive Sponcer'や'charge code'を埋め、’Submit'をクリックし申請フォームを送信します。" + Environment.NewLine + Environment.NewLine +

                "Steps on how to upgrade Power BI request:" + Environment.NewLine +

           "1.Visit Accenture Support Portal - https://support.accenture.com/support_portal" + Environment.NewLine +

"2.Click the Technology Support." + Environment.NewLine +
"3.Select 'Browse All Services'(found at the left side of the site)." + Environment.NewLine +
"4.Under the Productivity Tools, Click the ‘Reporting & Analytics’ and" + Environment.NewLine +
"5.Select the ‘Microsoft Power BI premium'." + Environment.NewLine +
"6.At the 'Request Action' select 'Upgrade existing subscription'." + Environment.NewLine +
"7.Fill up the 'Executive Sponsor' and 'Charge Code'." + Environment.NewLine +
"8.Click 'Submit'.";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            executed = true;
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("Change Password");
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs20101225/");
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
                //Process.Start("https://ts.accenture.com/sites/Japan_LTS_Projects/Media/Library/Win10_MFA_SSPR_Registration.pdf");
                executed = true;
            }
            if(scriptname== "Azure MS Portal Access-Tenant related")
            {
                saveExecutionLog("Azure MS Portal Access-Tenant related");
                subcategory.Add("Azure MS Portal Access-Tenant related");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Accessing_Azure_or_MS_Portal_Tenant.pdf");
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
                saveExecutionLog("MS Teams");
                subcategory.Add("MS Teams");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/VPN-Access.pdf");
                executed = true;
            }
            if(scriptname== "CORA Tool")
            {
                subcategory.Add("CORA Tool");
                saveExecutionLog("CORA Tool");
                executed = true;

                Process.Start("https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=8eead6ae1b2348d0ae30dd77cc4bcb4b");
            }
            if (scriptname == "Smart CardLogon Fix")
            {
                subcategory.Add("Smart CardLogon Fix");
                saveExecutionLog("Smart CardLogon Fix");
                executed = true;
                Process.Start("https://mypasswordless.accenture.com/gopasswordless");
            }
            if(scriptname== "FIDO2 Set-up and Registration")
            {
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                executed = true;
                Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20230215/");
            }
            if(scriptname == "Ordering a FIDO2 Token")
            {
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                executed = true;
                Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20230215/");
            }
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
            Process.Start("https://in.accenture.com/uki/policies/exiting-accenture-uk/?customize_changeset_uuid=#last-day");
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
            txtDetails.Text = "Microsoft Teamsのアドイン機能を修正したい場合は、\"Prepare Solution\"クリックし、管理者権限で実行してください。" + Environment.NewLine +Environment.NewLine+
                "If you want to fix MS Team meeting addin option, please click on button below. AvectoRights must be needed to execute the below solution.";
            scriptname = "MS Teams Meeting Addin Issue";
            fileName = "C:\\temp\\Teams_Meeting_Addin_issue.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Teams_Meeting_Addin_issue.exe";
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20191016/");
            executed = true;
        }

        private void mobileDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20170801/");
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20170801/");
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
            txtDetails.Visible = true;
            
            txtDetails.Text = "If you wish to reinstall/ fix Citrix issue, please click on the button below."+Environment.NewLine+"Please save all your work, system will restart after execution of solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Citrix Receiver Fix";

            fileName = "C:\\temp\\Citrix_WorkSpace_Reinstallation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Citrix_Receiver.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

        }

       

        private void vPNFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible=false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Please access the below link to check the Status of Accenture VPN access" +
                Environment.NewLine + "https://quickitsupport.gdn.accenture.com/japan_doc/VPN-Access.pdf"
                + Environment.NewLine+ "Please execute the VPN Solution if you have approved Subscription.";
            scriptname = "Accenture VPN Fix";

            fileName = "C:\\temp\\Basic_VPN_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Basic_VPN_Fix.exe";
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
            subcategory.Add("Recreating Corrupt Outlook Profile");
            saveExecutionLog("Recreating Corrupt Outlook Profile");
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20230509/");
            executed = true;
        }

        private void enterpriseAccountLockoutsAfterAPasswordChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            executed = true;
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs20101225/");
            //Process.Start("https://in.accenture.com/connectivitysecurity/how-to-avoid-enterprise-account-lock-outs-after-a-password-change/");
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs20091014/");
        }

        private void accessWebmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20211101-2/");
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs2010082001/");
        }

        private void addingOrRemovingASharedMailboxProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs2005121914/");
           // Process.Start("https://in.accenture.com/teamingsharingconferencing/adding-or-removing-a-shared-mailbox-profile-in-your-microsoft-outlook/");
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20211101-2/");
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
            Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/software_not_found.pdf");
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
            btnAzure.Visible = true;
            btnInstructions.Visible = false;
            panel1.Visible = false;
            btnFix.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            scriptname = "Azure MFA Registration";
            txtDetails.ForeColor = System.Drawing.Color.Black;
            txtDetails.Text = "① https://mysignins.microsoft.com/security-info を開きます。" + Environment.NewLine+
                "②「サインイン方法の追加」をクリックし、ドロップダウンから追加する方法を選択し「追加」をクリックします。" + Environment.NewLine +
                "③画面指示に従い登録します。" + Environment.NewLine+ "※手順の詳細は、\"Details\"をクリックします。" + Environment.NewLine+Environment.NewLine+




                "Click on link - https://mysignins.microsoft.com/security-info" + Environment.NewLine+Environment.NewLine +


                "* Click on “Add Method”." + Environment.NewLine+


                "* Choose method would you like to add from the drop Down." + Environment.NewLine+

                "* Click on Add & Enter Details and then follow the instructions.";
        }

        private void azureMFaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20131107/");
            //Process.Start("https://in.accenture.com/connectivitysecurity/azure-mfa-and-self-service-password-reset-sspr/?referrer=mailer");
            executed = true;
        }

        private void vCCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());

            Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/VC_Booking_Cost_and_Other_Details.pdf");
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
            txtDetails.Text = "PC上のすべてのシステムコンプライアンスを修正するには、\"Prepare Solution\" をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+
                "Do you want to fix all compliance issues for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Overall NC Remediation";

            fileName = "C:\\temp\\Overall_NC_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Overall_NC_Remediation.exe";
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
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/ForcePoint_Compliance_Check.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Red;
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
            txtDetails.Text = "Javaのコンプライアンス違反を解消するには、下のボタンをクリックしてください。実行するには、管理者権限が必要です。" + Environment.NewLine+Environment.NewLine+
                "Do you want to fix Java compliance issue? click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Java NC  Remediation";

            fileName = "C:\\temp\\Java_Compliance_Checkv1.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Java_Compliance_Checkv1.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;
        }

        private void phishMeCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            scriptname = "Cofense Reporter Check";
            panel1.Visible = false;
            txtDetails.Text =
                            " If you wish to update Cofense reporter fix Noncompliance issue click on button below." + Environment.NewLine + "Please save all your work, system will restart after execution." + Environment.NewLine + Environment.NewLine +
                            "Replication on portal takes 48 to 72 business hours post execution of the solution." + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            fileName = "C:\\temp\\PhishMe_Check.exe ";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/PhishMe_Check.exe";
            string myStringWebResource = null;
            btnFix.Visible = true;
            txtDetails.ForeColor= System.Drawing.Color.Red;
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
            //txtDetails.Visible = true;
            //tabControl1.Visible = false;
            scriptname = "MDfE Antivirus Fix";
            panel1.Visible = false;
            txtDetails.Text = "Microsoft Defender for Endopointのコンプライアンス状態を更新または修正するには、\"Prepare Solution\"をクリックしてください。\r\n※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n※実行するには、管理者権限が必要です。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine +Environment.NewLine+
                
                            " If you wish to update or Fix MDfE Antivirus compliance issue, click on button below.Please save all your work, system will restart after execution. AvectoRights must be needed to execute the below solution." + Environment.NewLine +
                            //"Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
                            "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            fileName = "C:\\temp\\MDfE_AVFW_NC_Remediation.exe ";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/MDfE_AVFW_NC_Remediation.exe";

            string myStringWebResource = null;
            btnFix.Visible = true;
            txtDetails.ForeColor=System.Drawing.Color.Black;
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
            txtDetails.Text = "BeyondTrust PMチェックのコンプライアンスの問題を更新または修正する場合は、下のボタンをクリックしてください。\r\n　※実行後PCは再起動されるので、すべての作業を保存してから行ってください。\r\n　※実行するには、管理者権限が必要です。ポータルへの反映には、 48～72時間かかる場合があります。" + Environment.NewLine + Environment.NewLine +

                            " If you wish to update or Fix BeyondTrust PM Check compliance issue, click on button below." + Environment.NewLine + "Please save all your work, system will restart after execution. Avecto Rights must be needed to execute the below solution." + Environment.NewLine + Environment.NewLine +
                            "Replication on portal takes 48 to 72 business hours post execution of the solution.";
            fileName = "C:\\temp\\BeyondTrust_PM_compliance.exe ";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/BeyondTrust_PM_compliance.exe";
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
            txtDetails.Text = "① https://wd3.myworkday.com/accenture を開き、自身のロールを選択します。" + Environment.NewLine + 
                "②画面右上の「プロファイル」をクリックし、「プロファイルの表示」を選択します。" + Environment.NewLine+
                "③左のメニュー一覧から「個人情報」を選択します。" + Environment.NewLine+
                "④「通称氏名」の「編集」より、登録情報を変更後「送信」をクリックします。" + Environment.NewLine+ "※Teamsアカウントへの反映は約48時間かかります。" + Environment.NewLine + Environment.NewLine +
                //"* [優先名] に移動し、編集し、詳細を入力して送信します。"+Environment.NewLine+
                //"* Teams でのレプリケーションには最大 48 時間かかります" + Environment.NewLine+ Environment.NewLine +
                "https://wd3.myworkday.com/accenture" +Environment.NewLine+ Environment.NewLine+
                                "* Access above(myworkday) portal and Select Role."+Environment.NewLine +
                                "* Go to profile Picture and Select View Profile."+ Environment.NewLine +
                                "* Select Personal towards left-hand side."+Environment.NewLine+
                                "* Go to Preferred Name, Edit, Provide the Details and Submit."+Environment.NewLine+
                                "* Replication on Teams takes ~48Hrs";
        }

        private void pulseSecureReinstallationexeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Pulse Secure issue on your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Pulse Secure Reinstallation";

            fileName = "C:\\temp\\PulseSecure_Reinstallation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/PulseSecure_Reinstallation.exe";
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
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            //tabControl1.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Citrix workspace issue for your system. click a button below to run solution. Avecto Rights must be needed to execute the below solution.";
            scriptname = "Citrix Workspace Fix";

            fileName = "C:\\temp\\Citrix_WorkSpace.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Citrix_WorkSpace.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            //myWebClient.DownloadFile(myStringWebResource, @fileName);
            btnFix.Visible = true;
            txtDetails.ForeColor = System.Drawing.Color.Black;

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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Firewall issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Firewall Check";

            fileName = "C:\\temp\\Firewall_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Firewall_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix DLP issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "DLP Check";

            fileName = "C:\\temp\\DLP_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/DLP_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix adobe reader issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Adobe Reader Check";

            fileName = "C:\\temp\\Adobe_Reader_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Adobe_Reader_Check.exe";
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
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/EVTA_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix adobe creative cloud suite issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Adobe Creative Cloud Suite Check";

            fileName = "C:\\temp\\adobeCreativeCloudSuite_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/adobeCreativeCloudSuite_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Domain membership issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Domain Membership Check";

            fileName = "C:\\temp\\domainMembership_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/domainMembership_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix python software issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Python Software Check";

            fileName = "C:\\temp\\pythonSoftware_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/pythonSoftware_Check.exe";
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
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Chrome_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Firefox issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Firefox Check";

            fileName = "C:\\temp\\firefox_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/firefox_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Edge issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Edge Check";

            fileName = "C:\\temp\\edge_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/edge_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix Internet Explorer issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "internet Explorer Check";

            fileName = "C:\\temp\\internetExplorer_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/internetExplorer_Check.exe";
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
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Opera_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix unauthorized software issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "unauthorizedSoftware_Check.exe";

            fileName = "C:\\temp\\unauthorizedSoftware_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/unauthorizedSoftware_Check.exe";
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
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Do you want to fix operating system issue for your system. click a button below to run solution. " + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Operating System Check";

            fileName = "C:\\temp\\os_Check.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/os_Check.exe";
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
           Process.Start("https://quickitsupport.gdn.accenture.com//japan_doc/newJoinerSupportDesk.pdf");
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
            Process.Start("https://quickitsupport.gdn.accenture.com//japan_doc/dedicatedSupport.pdf");
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
            executed = true;
            scriptname = "Admin Rights";
            subcategory.Add("Admin Rights");
            saveExecutionLog("Admin Rights");
            MessageBox.Show("1) Go to Windows Search and type “Software center”." + Environment.NewLine + "2) Click on “Application” folder." + Environment.NewLine + "3) Choose “Zero Touch Solution – Avecto Automation” Solution."
                    + Environment.NewLine + "4) Install the Solution and then Restart the Laptop.", "message");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/ts20171108/");
            //Process.Start("https://in.accenture.com/mycomputer/delegate-access-troubleshooting-steps-how-to-remove-and-re-add-delegate-access/");
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

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal/?id=acn_my_devices");
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal/?id=pc_hardware_other_pc_requests");
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            subcategory.Add("HFB Manual Enrollment");
            saveExecutionLog("HFB Manual Enrollment");
            Process.Start("https://kxdocuments.accenture.com/contribution/1e472cba-8375-4f4c-afdd-63156e2e6fee");
            executed = true;
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            subcategory.Add("HFB Automated Enrollment");
            saveExecutionLog("HFB Automated Enrollment");
            Process.Start("https://kxdocuments.accenture.com/contribution/246809d6-39ca-4a45-91f8-5021f9f89d80");
            executed = true;
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add("Tap Request");
            saveExecutionLog("Tap Request");
            Process.Start("https://ts.accenture.com/sites/Japan_LTS_Projects/Media/Library/Passwordless/Temporary%20Access%20Pass%20(TAP)%E5%88%A9%E7%94%A8%E6%96%B9%E6%B3%95.pdf");
        }

        private void toolStripMenuItem66_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/power-bi-provisioning-support-site/");
            executed = true;
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/windows-11-homepage/");
            executed = true;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

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
            txtDetails.Text = "Software Centerでアプリやツールをインストールや更新ができない場合は、\"Prepare Solution\"をクリックし、管理者権限で実行します。" + Environment.NewLine+Environment.NewLine+
                "This Script solution will fix unable to install or keeps failing updates/applications on software center. Use this solution to fix those unable to install apps/updates on software center." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Software Center Update Fix";
            fileName = "C:\\temp\\Software_Center_Update_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Software_Center_Update_Fix.exe";
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
            txtDetails.Text = "コントロールパネル＞Configuration Managerを開き、タブが７つ表示されていない場合は、アクセンチュアのドメインとの通信に問題があり、PCがコンプライアンスに準拠していないことを意味します。SCCM(system center configuration manager)を修正する必要がある場合は、\"Prepare Solution\"をクリックします。" + Environment.NewLine +Environment.NewLine+

                "This Script solution will try to fix System Center Configuration Manager focusing on Non-Pooling compliance that is unable to communicate with our Domain Accenture causing our workstation to have a non-compliant if it's not healthy. Control Panel > Configuration Manager, if it shows 6 tabs, that means your SCCM is not healthy. Use this script to try to fix non-healthy SCCM to fully healthy SCCM."
                           + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "SCCM Fix";
            fileName = "C:\\temp\\SCCM_Not_Working_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/SCCM_Not_Working_Fix.exe";
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
            txtDetails.Text = "このスクリプトソリューションは、表示されない、または破損しているファイルエクスプローラーリボンを修正します。このソリューションを使用してその機能を復元することで、リボンが再表示されます。\"Prepare Solution\"をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+

                "This Script solution will try to fix File Explorer Ribbon that's not showing or corrupted. Use this solution to restore its functionality and ribbon will be visible." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows Files Explorer Ribbon Fix";
            fileName = "C:\\temp\\Windows_File_Explorer_Ribbon_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Windows_File_Explorer_Ribbon_Fix.exe";
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
            txtDetails.Text = "このスクリプトソリューションは、システムファイルの破損修復、イメージが修復可能かどうかを判断し、破損が検出された場合は、見つかった問題を修復します。\"Prepare Solution\"をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+

                "This Script solution will try to fix system file corruption, determines if the image is repairable, checks if there are any corruptions detected, and repairs problems that it finds with the operating system you are logged into. Use this solution to try to fix it." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Restore and Repair Corrupted System Files Fix";
            fileName = "C:\\temp\\Restore_and_Repair_Corrupted_System_File_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Restore_and_Repair_Corrupted_System_File_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void label8_Click(object sender, EventArgs e)
        {

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
            txtDetails.Text = "Windowsライセンスの認証のエラーが出ている場合は、\"Prepare Solution\"をクリックします。" + Environment.NewLine + Environment.NewLine +

                "This Script solution will fix our Windows that shows a blue watermark that says Activate Windows. Use this solution to remediate Activation issue on desktop."
                           + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows Activation Fix";
            fileName = "C:\\temp\\Windows_Activation_fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Windows_Activation_fix.exe";
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
            txtDetails.Text = "「PINは現在利用できません」と表示される場合、\"Prepare Solution\"をクリックします。実行には、管理者権限が必要です。" + Environment.NewLine+Environment.NewLine+
                "This Script solution will fix the HFB PIN that has an error 'PIN is currently Unavailable'. Use this solution to resolve this issue." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "HFB Pin Fix";
            fileName = "C:\\temp\\HFB_Pin_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/HFB_Pin_Fix.exe";
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
            txtDetails.Text = "Microsoft Defender によって開くことができないアプリケーションがある場合は、\"Prepare Solution\"をクリックし管理者権限で実行します。" + Environment.NewLine+Environment.NewLine+
                "This Script solution will fix for the application Microsoft Defender is unable to open. Use this solution to fix Unable to open Microsoft Defender." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Defender NotOpening Fix";
            fileName = "C:\\temp\\Defender_NotOpening.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Defender_NotOpening.exe";
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
            txtDetails.Text = "パスワードレスの方で、ご自身の設定したPINや生体認証でログオンができない場合は、\"Prepare Solution\"をクリックし、手順に沿ってパスワードを発行してください。その後、発行したパスワードでログオンしてください。" + Environment.NewLine +Environment.NewLine+

                "This Script solution will fix the Smart Card Logon on startup where your password was setup to passwordless and unable to login to your workstation. Use this script to follow the instructions how to enable back password setup." +
                            Environment.NewLine + "For more info, visit : https://mypasswordless.accenture.com/gopasswordless " + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Smart CardLogon Fix";
            fileName = "C:\\temp\\Smart_CardLogon_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Smart_CardLogon_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void NetworkProblemRepairtoolStripMenu_Click(object sender, EventArgs e)
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
            txtDetails.Text = "このスクリプト ソリューションでは、次の詳細を選択できます。" + Environment.NewLine + "オプション 1: TCP/IPOption にバインドされている各アダプターのすべての構成情報を表示します。"
                 + Environment.NewLine + "オプション2: 考えられるネットワークの問題を表示します。" + Environment.NewLine + "オプション3:ネットワーク情報のキャッシュをクリアし、新しいIPを付与します。"
                 + Environment.NewLine + "オプション4: ネットワーク上のプロキシ設定をすべて削除します。実行するには、’prepare solution'をクリックし、管理者権限で実行してください" + Environment.NewLine+Environment.NewLine+

                "This Script solution will let you select the following details." + Environment.NewLine + "Option 1: Displays all configuration information for each adapter bound to TCP/IP"
                            + Environment.NewLine + "Option 2: Displays possible network problems data." + Environment.NewLine + "Option 3: Clears network Cache, Release and renew an IP to your network."
                            + Environment.NewLine + "Option 4: Remove any proxy setup on your network." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Network Problem Diagnose Repair";
            fileName = "C:\\temp\\Network_Problem_Diagnose_Repair.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Network_Problem_Diagnose_Repair.exe";
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
            txtDetails.Text = "このスクリプトソリューションは、応答しない、スタックする、または動作が不安定なWindowsタスクバーを修正します。このソリューションを使用して、その機能をリセットします。\"Prepare Solution\"をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+

                "This Script solution will fix windows taskbar that are not responding, stuck, or unstable. Use this solution to reset back its functionality." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Taskbar Fix";
            fileName = "C:\\temp\\Taskbar_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Taskbar_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void Windows11ClassicRightClickFixtoolStripMenu_Click(object sender, EventArgs e)
        {
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "If you want to fix the Windows11 Classic RightClick issue, please click on button below." + Environment.NewLine + Environment.NewLine + "Note: Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows11 Classic Right Click Fix";
            fileName = "C:\\temp\\Windows11_Classic_right_click_feature.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Windows11_Classic_right_click_feature.exe";
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
            txtDetails.Text = "ご自身のPCにWindowsポリシーが正しく反映されていない場合、グループポリシーのキャッシュをクリアもしくは削除し、再更新します。\"Prepare Solution\"をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+

                "This Script solution will clear or remove any group policy cache on your machine. Use this solution that windows policies were not reflecting properly and perform group policy update afterwards." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Clear Group Policy Cache";
            fileName = "C:\\temp\\Clear_Group_PolicyCache.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Clear_Group_PolicyCache.exe";
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
            btnAzure.Visible = false;
            btnHfbPin.Visible = false;
            btnHfbT2.Visible = false;
            btnHfbT3.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "WindowsOSのアップグレードに失敗する場合には、スクリプトを実行します。\"Prepare Solution\"をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+

                "This Script solution will fix those In-Place Upgrade for Windows installation doesn't proceed. Use this solution to fix and proceed its installation." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "IPU Install Fix";
            fileName = "C:\\temp\\IPU_Unable_to_install_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/IPU_Unable_to_install_Fix.exe";
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
            txtDetails.Text = "このスクリプト ソリューションは、SAP などの.net Framework の前提条件を必要とするアプリケーションを修正します。このソリューションを使用して、.net Framework を正しくインストールします。\"Prepare Solution\"をクリックし、管理者権限で実行してください。" + Environment.NewLine+Environment.NewLine+

                "This Script solution will fix those applications that require .Net Framework pre requirements such as SAP. Use this solution to install .net Framework properly." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Net Framework Fix ";
            fileName = "C:\\temp\\Net_Framework_Fix.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Net_Framework_Fix.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void AvectoPrivilegeAccesstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal/support_portal?id=iframe_catalog_request&sys_id=44eeeee213443a00e24d30128144b0a7");
            executed = true;
        }

        private void DeviceControlPmtNonCompliancetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mycomputer/device-control-pmt-non-compliance-remediation/");
            executed = true;
        }

        private void GroupPolicyManagementtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071902&sys_id=54bfc914dba77200f748fdb61d961931");
            executed = true;
        }

        private void NonStandardSoftwareReviewtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0072221&sys_id=aed365d1dbbd07003cacfb051d96199a");
            executed = true;
        }

        private void RequestForAssistancetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=iframe_catalog_request&sys_id=ce87487f13f76200e24d30128144b08f");
            executed = true;
        }

        private void RequestForReImagingWorkstationtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071813&sys_id=07e3dc06db5a760057c7ff971d961936");
            executed = true;
        }

        private void RoundTableConferencetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0072142&sys_id=a235f46f1315b600e24d30128144b038");
            executed = true;
        }

        private void SCCMSoftwareDeploymenttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071865&sys_id=405cb553dbb96f0425cd9c41ba961900");
            executed = true;
        }

        private void SoftwareInstallationAssistancetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0075308&sys_id=538c9c9f135b2e40380ddbf18144b07d");
            executed = true;
        }

        private void VoiceNonChargeableServicestoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071839&sys_id=cfcdbca8db62ba0057c7ff971d961980&catID=null");
            executed = true;
        }

        private void IOSIntuneRegistrationtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://cdn.in.accenture.com/prod/wp-content/uploads/sites/274/2021/10/08213407/iOS-MAM_July_2021_azmfa.pdf");
            executed = true;
        }

        private void AndroidIntuneRegistrationtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://cdn.in.accenture.com/prod/wp-content/uploads/sites/274/2021/10/08213405/Android-MAM_July_2021_azmfa.pdf");
            executed = true;
        }

        private void MSTeamsandYammerLiveEventstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0075399&sys_id=99b35a5a1358f60099f8dbf18144b0b6");
            executed = true;
        }

        private void MSTeamsCallQueuestoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0075379&sys_id=23fbf2d613522fc88c72d7028144b088");
            executed = true;
        }

        private void MSTeamsMobilePolicyChangetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0087081&sys_id=807dd552dbc8ec144ef1ea7b039619ce");
            executed = true;
        }

        private void MSTeamsPhoneEditiontoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071232&sys_id=46dd671813e9f28099f8dbf18144b0a1");
            executed = true;
        }

        private void MSTeamsRecordingEnablementtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0087899&sys_id=973aed7e1bef0414dc492fc02a4bcb22");
            executed = true;
        }

        private void MSTeamsRequesttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal/?id=iframe_catalog_request&sys_id=b4e6136e13453600e24d30128144b080");
            executed = true;
        }

        private void MSTeamsRoomtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services&articleNumber=KB0071970");
            executed = true;
        }

        private void DesktopRequesttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071744&sys_id=9f894a5e13c9f200e24d30128144b085");
            executed = true;
        }

        private void HardwareInstallationorUninstallationtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071819&sys_id=de620837dbc2720057c7ff971d9619e0&catID=null");
            executed = true;
        }

        private void LaptopReequesttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=iframe_catalog_request&sys_id=ab5eb1521309ba4099f8dbf18144b0c0");
            executed = true;
        }

        private void RequestForAssetConsumablePeripheralstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071818&sys_id=21fbd3bf134d3200e24d30128144b005&catID=null");
            executed = true;
        }

        private void MobileApplicationtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/mobiledevicesandtelephony/mobile-productivity/");
            executed = true;
        }

        private void RequestforMobilePhoneandPlanstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=iframe_catalog_request&sys_id=0aa6fdf013213a00e24d30128144b04a&categoryId=5223e818db62cb00f945f9a41d96192c");
            executed = true;
        }

        private void RequestforAssetMovementtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services_iframe&articleNumber=KB0071824&sys_id=026073261385fa4099f8dbf18144b017&catID=null");
            executed = true;
        }

        private void orsRequesttoRecevieITAssetfromVendtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071828&sys_id=4ab5748e13736a00380ddbf18144b00a");
            executed = true;
        }

        private void ReturnaPCtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071810&sys_id=dd084ad61309ba4099f8dbf18144b067");
            executed = true;
        }

        private void VoiceChargeableRequesttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services_iframe&articleNumber=KB0072186&sys_id=672a148adb1efe00f748fdb61d96192e&catID=6383ac98db62cb00f945f9a41d961904");
            executed = true;
        }

        private void BandwidthIncreasetoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=iframe_catalog_request&sys_id=a8fa648edb5a0f40ec2638f0ad961913");
            executed = true;
        }

        private void DataPortEnableorDisablementtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services_iframe&articleNumber=KB0071792&sys_id=ea9d48a1db86f600f748fdb61d961998");
            executed = true;
        }

        private void RASVPNRequesttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071256&sys_id=66d1ed5a1385ba4099f8dbf18144b077");
            executed = true;
        }

        private void RequestForFixedhomeInternetConnectiontoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0077350&sys_id=78e91bd9dbc118144ef1ea7b03961987");
            executed = true;
        }

        private void SharedFolderAccesstoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0071843&sys_id=0f2c60badb02320057c7ff971d9619f2");
            executed = true;
        }

        private void WIFIEnaqbleorDisablementtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0088550&sys_id=6a8769cedb423a0057c7ff971d96199d");
            executed = true;
        }

        private void RequestForFixedhomeInternetConnectiontoolStripMenu_Click_1(object sender, EventArgs e)
        {

        }

        private void RequestforHomeinternetconnectiontoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://support.accenture.com/support_portal?id=it_services2&articleNumber=KB0077350&sys_id=78e91bd9dbc118144ef1ea7b03961987");
            executed = true;
        }

        private void NewJoinerGuideLinktoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://mediaexchange.accenture.com/channel/ISA%2BGuide%2BMe/250570533");
            executed = true;
        }

        private void toolStripMenuWorkStationIssue_Click(object sender, EventArgs e)
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
            txtDetails.Text = "アクセンチュア社内のWi-FiまたはLANの接続中に問題が発生した場合は、wifi-guest/外部ネットワークに接続し'prepare solution'をクリックしてください。実行後にPCを再起動が必要です。" + Environment.NewLine+Environment.NewLine+

                "Follow the On-Screen Instructions in case of any issues while connecting Accenture Wi-Fi or LAN." + Environment.NewLine + "Ensure to connect Wi-Fi Internet/External Network and Restart the Laptop Post Execution of the solution." + Environment.NewLine + "Avecto Rights must be needed to execute the below solution.";
            scriptname = "Windows Workstation Network Access Fix";
            fileName = "C:\\temp\\Windows_Workstation_Network_Access_Issue.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/Windows_Workstation_Network_Access_Issue.exe";
            string myStringWebResource = null;
            // Create a new WebClient instance.

            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri;
            // myWebClient.DownloadFile(myStringWebResource, @fileName);
            txtDetails.ForeColor = System.Drawing.Color.Black;
            btnFix.Visible = true;
        }

        private void txtDetails_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAzure_Click(object sender, EventArgs e)
        {
            if(scriptname == "Azure MFA Registration")
            {
                executed = true;
                scriptname = "Azure MFA Registration";
                subcategory.Add("Azure MFA Registration");
                saveExecutionLog("Azure MFA Registration");
                Process.Start("https://ts.accenture.com/sites/Japan_LTS_Projects/Media/Library/Win10_MFA_SSPR_Registration.pdf");
            }
            if (scriptname == "Azure MS Portal Access-Tenant related")
            {
                executed = true;
                scriptname = "Azure MS Portal Access-Tenant related";
                subcategory.Add("Azure MS Portal Access-Tenant related");
                saveExecutionLog("Azure MS Portal Access-Tenant related");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Accessing_Azure_or_MS_Portal_Tenant.pdf");
            }
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
            txtDetails.Text = "ポータルサイトへのアクセスに問題がある場合は、\"Details' をご確認ください。\r\n(トラブル例）\r\n・AzureやMicrosoftサービスポータルにアクセスした際に、MFA認証が聞かれない、もしくは認証に失敗する場合\r\n・アクセンチュア社内ポータルサイト以外のサイトにアクセスできない場合\r\n" + Environment.NewLine +Environment.NewLine+
               
               
                "Use Cases when users are accessing Azure or other MS Service Portals" + Environment.NewLine +
                "*For Private, Project, Customer Portals, but can access other Accenture Sites." + Environment.NewLine +
                "*Users are prompted MFA but the authentication is failing or timing out." + Environment.NewLine +
                "*Users are not receiving the expected MFA prompt during the sign in process." + Environment.NewLine +
                "For more information click on details button";
        }

        private void toolStripMenuItem144_Click(object sender, EventArgs e)
        {
            subcategory.Add("Windows Hello for Business");
            saveExecutionLog("Windows Hello for Business");
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ws20200511/");
            //Process.Start("https://in.accenture.com/mycomputer/how-to-enable-windows-10-hello/");
            executed = true;
        }

        private void PasswordlessSupporttoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ws20220912/?customize_changeset_uuid=");
            executed = true;
        }

        private void PasswordlessSupportFAQtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ws20220912/?customize_changeset_uuid=");
            executed = true;
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void buttonHFBmanual_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://kxdocuments.accenture.com/contribution/1e472cba-8375-4f4c-afdd-63156e2e6fee");
            executed = true;
        }

        private void buttonHFBAutomated_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://kxdocuments.accenture.com/contribution/246809d6-39ca-4a45-91f8-5021f9f89d80");
            executed = true;
        }

        private void toolStripMenuGoPasswordlessRequest_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://ts.accenture.com/sites/Japan_LTS_Projects/Media/Library/Passwordless/Guide%20for%20Passwordless%20enablement.pdf");
            executed = true;
        }

        private void toolStripMenuMSAuthAppEndUser_Click(object sender, EventArgs e)
        {
            subcategory.Add("MS Authenticator App End User Guid");
            saveExecutionLog("MS Authenticator App End User Guid");
            Process.Start("https://ts.accenture.com/sites/Japan_LTS_Projects/Media/Library/Win10_MFA_SSPR_Registration.pdf");
            executed = true;
        }

        private void toolStripMenuPasswordless_Click(object sender, EventArgs e)
        {
            subcategory.Add("Passwordless Support");
            saveExecutionLog("Passwordless Support");
            Process.Start("https://in.accenture.com/connectivitysecurity/passwordless-support/");
            executed = true;
        }

        private void toolStripMenuManagingSigninMethod_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20131107/");
            executed = true;
        }

        private void NewJoinerNeedtoKnowtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/yearone/");
            executed = true;
        }

        private void CreateEnterpriseIDPasswordtoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://mediaexchange.accenture.com/playlist/dedicated/250570533/1_x25bakor/1_pg09xvdp");
            executed = true;
        }

        private void HowToLoginAccentureLaptoptoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://mediaexchange.accenture.com/playlist/dedicated/250570533/1_x25bakor/1_28r6fysn");
            executed = true;
        }

        private void HowToSetUpBitLockerPintoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20210506/");
            executed = true;
        }

        private void SettingUpMultitoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20131107/");
            executed = true;
        }

        private void WindowHelloForBussinesstoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ws20200511/");
            executed = true;
        }

        private void UnlockedPasswordtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs20101225/");
            executed = true;
        }

        private void EnterpriseIdandPasswordtoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/enterprise-id-and-password/");
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
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs20101225//");
            executed = true;
        }

        private void SelfServerPasswordResettoolStripMenu_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/connectivitysecurity/self-service-password-reset-web-application/");
            executed = true;
        }

        private void MacInternetEnrolltoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://mediaexchange.accenture.com/playlist/dedicated/250570533/1_bt4exu6x/1_ho3rk9y3");
            executed = true;
        }

        private void GlobalProtectVPNtoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_cs2009070201/");
            executed = true;
        }

        private void toolStripMenuWirelessContentSharing_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/videoconferencing/wireless-content-sharing/");
        }

        private void toolStripMenuSurfaceHub_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/surface-hub/");
        }

        private void toolStripMenuVideoConference_Click(object sender, EventArgs e)
        {
            executed = true;
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_nw2009072101/");
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
            txtDetails.Text = "FIDO2の購入方法や・使用手順については、下記サイトを参照してください。https://in.accenture.com/japan/lts_home/lts_ts20230215/" + Environment.NewLine +Environment.NewLine+
      //"表 1: Windows (OS) の FIDO2 トークンのセットアップと登録"+Environment.NewLine+
      //"表 2: FIDO2 トークンのセットアップと MAC (OS) の登録"+Environment.NewLine+
      //"表 3: 特権アカウントの FIDO2 トークン キーの登録" + Environment.NewLine + Environment.NewLine +
            "This guide highlights support material that the user can use to set-up and register a FIDO2. Resources are also shared to learn more about FIDO2 and how to seek support." + Environment.NewLine + Environment.NewLine +
                "Table 1: FIDO2 Token Setup and Registration for Windows (OS)" + Environment.NewLine + "Table 2: FIDO2 Token Setup and Registration for MAC (OS)" + Environment.NewLine + "Table 3: Registering a FIDO2 Token Key for Privileged Accounts";
        }

        private void btnHfbPin_Click(object sender, EventArgs e)
        {
            if (scriptname == "HfB PIN Reset Options")
            {
                executed = true;
                subcategory.Add("HfB PIN Reset Options");
                saveExecutionLog("HfB PIN Reset Options");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Using_the_system_setting_if_your_desktop_is_in_unlocked_screen_mode.pdf");
            }
            if (scriptname == "FIDO2 Set-up and Registration")
            {
                executed = true;
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/FIDO2_Token_Setup_and_Registration_for_Windows_(OS).pdf");
            }
            if (scriptname == "Ordering a FIDO2 Token")
            {
                executed = true;
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Recommended_Tokens.pdf");
            }
        }

        private void btnHfbT2_Click(object sender, EventArgs e)
        {
            if (scriptname == "HfB PIN Reset Options")
            {
                executed = true;
                subcategory.Add("HfB PIN Reset Options");
                saveExecutionLog("HfB PIN Reset Options");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Passwordless_Enabled_Using_the_system_settings_from_the_unlocked_screen.pdf");
            }
            if (scriptname == "FIDO2 Set-up and Registration")
            {
                executed = true;
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/FIDO2_Token_Setup_and_Registration_for_MAC_(OS).pdf");
            }
            if (scriptname == "Ordering a FIDO2 Token")
            {
                executed = true;
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Users_unable_to_locate_their_shipping_address_during_profile_setup.pdf");
            }
        }

        private void btnHfbT3_Click(object sender, EventArgs e)
        {
            if (scriptname == "HfB PIN Reset Options")
            {
                executed = true;
                subcategory.Add("HfB PIN Reset Options");
                saveExecutionLog("HfB PIN Reset Options");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Passwordless_Enabled_Pin_Reset_from_the_locked_screen.pdf");
            }
            if (scriptname == "FIDO2 Set-up and Registration")
            {
                executed = true;
                subcategory.Add("FIDO2 Set-up and Registration");
                saveExecutionLog("FIDO2 Set-up and Registration");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Registering_a_FIDO2_Token_Key_for_Privileged_Accounts.pdf");
            }
            if (scriptname == "Ordering a FIDO2 Token")
            {
                executed = true;
                subcategory.Add("Ordering a FIDO2 Token");
                saveExecutionLog("Ordering a FIDO2 Token");
                Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/Search_results_indicate_that_the_Feitian_products_are_not_found.pdf");
            }
        }

        private void toolStripMenuItem147_Click(object sender, EventArgs e)
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
            txtDetails.Text = "FIDO2の購入方法や・使用手順については、下記サイトを参照してください。https://in.accenture.com/japan/lts_home/lts_ts20230215/" + Environment.NewLine + Environment.NewLine+
      //"表 1: 推奨されるトークン"+Environment.NewLine+
      //"表 2: ユーザーがプロファイルのセットアップ中に配送先住所を見つけることができない"+Environment.NewLine+
      // "表 3: 検索結果は、Feitian 製品が見つからないことを示しています" + Environment.NewLine + Environment.NewLine +
            "This guide highlights support material that the user can use to order a FIDO2 token on the BuyNow (Ariba) site. Resources are also shared to learn more about FIDO2 and how to seek support." + Environment.NewLine + Environment.NewLine +
                "Table 1: Recommended Tokens" + Environment.NewLine + "Table 2: Users unable to locate their shipping address during profile setup" + Environment.NewLine + "Table 3: Search results indicate that the Feitian products are not found";
        }

        private void HfbPinResetoptionstoolStrip_Click(object sender, EventArgs e)
        {
            btndownload.Visible = false;
            btndownloadruntime.Visible = false;
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.ForeColor = System.Drawing.Color.Black;
            // tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Visible = true;
            txtDetails.Text = "下記手順に沿って、Hello for Business PINを変更できます。" + Environment.NewLine+ "①Ctrlキー+Altキー+Deleteキーを同時に押す"
                + Environment.NewLine + "②「パスワードの変更」をクリック" + Environment.NewLine + "③「サインインオプション」をクリック" + Environment.NewLine + "④PINアイコンをクリック" + Environment.NewLine + "⑤画面に従い、新しいPINと利用中のPINを入力" + Environment.NewLine+Environment.NewLine+
                "The HfB PIN reset options help guide offers different ways user can reset their hello for Business PIN: This document also support methods for both Passwordless and Non passwordless users."
                + Environment.NewLine + Environment.NewLine + "Table1: Using the System Settings if your desktop is in unlocked screen mode" + Environment.NewLine + "Table2: Passwordless is Enabled: Using System Settings from the Unlocked screen" + Environment.NewLine + "Table3: Passwordless is Enabled: PIN Reset from the Locked Screen.";
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

        private void HFBManualEnrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subcategory.Add("HFB Manual Enrollment");
            saveExecutionLog("HFB Manual Enrollment");
            Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/ACN_Hfb_Manual_Enrollment_utility_Instructions.pdf");
            executed = true;
        }

        private void toolStripHfbAutomatedEnroll_Click(object sender, EventArgs e)
        {
            subcategory.Add("HFB Automated Enrollment");
            saveExecutionLog("HFB Automated Enrollment");
            Process.Start("https://quickitsupport.gdn.accenture.com/japan_doc/ACN_Hfb_Automated_Enrollment_utility_Instructions.pdf");
            executed = true;
        }

        private void NewJoinerSupportDesktoolStrip_Click(object sender, EventArgs e)
        {
            Process.Start("https://quickitsupport.gdn.accenture.com//japan_doc/newJoinerSupportDesk.pdf");
            executed = true;
            saveExecutionLog(sender.ToString());
            subcategory.Add(sender.ToString());
        }

        private void toolStripMenuReachTool_Click(object sender, EventArgs e)
        {
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            panel1.Visible = false;
            txtDetails.Text = "Accenture Inhouse Remote Tool (REACH) を以下からダウンロードします。" + Environment.NewLine + Environment.NewLine + "オプション 1: REACH パッケージ。" + Environment.NewLine + "オプション 2: Windows ランタイムを含む REACH パッケージ。"+Environment.NewLine+Environment.NewLine+
                "Download Accenture Inhouse Remote Tool (REACH) from the below." + Environment.NewLine + Environment.NewLine + "Option 1: REACH Package." + Environment.NewLine + "Option 2: REACH Package with Windows Runtime.";
            scriptname = "REACH Tool";
            fileName = "C:\\temp\\REACH.zip";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/REACH.zip";

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
                    remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/REACH_runtime.zip";

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

        private void PasswordlessfaqstoolStrip_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ws20220912/?customize_changeset_uuid=");
            executed = true;
        }

        private void OrderingFIDO2TokenLink_Click(object sender, EventArgs e)
        {
            subcategory.Add(sender.ToString());
            saveExecutionLog(sender.ToString());
            Process.Start("https://in.accenture.com/japan/lts_home/lts_ts20230215/#Eng");
            executed = true;
        }

        private void browserCheckToolStripMenuItem_Click(object sender, EventArgs e)
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
            txtDetails.Text = "Microsoft Teamsに問題がある場合、下記リンクからCORAツールをダウンロードしてください。\r\n https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=8eead6ae1b2348d0ae30dd77cc4bcb4b " + Environment.NewLine+ Environment.NewLine +
                
                "Please download Communicator Optimization and Remediation Assistant (CORA) tool from below SAC Portal." +Environment.NewLine+
                "https://support.accenture.com/support_portal?id=acn_sac&spa=1&page=details&category=&sc_cat_id=8eead6ae1b2348d0ae30dd77cc4bcb4b"+Environment.NewLine+
                "CORA will help to fix all basic MS team issues.";
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
            txtDetails.Text = "*パスワードを変更するには、キーボード上で「Ctrl + Alt + Delete」を同時に押します。  " + Environment.NewLine +
                "*パスワード同期の問題を回避するために、アクセンチュアVPNに接続することをお勧めします。" + Environment.NewLine+Environment.NewLine+

                "* Click on “Ctrl+Alt+Delete” on your laptop to change the password." + Environment.NewLine +
                                  "* Recommended to connect Accenture VPN to avoid password sync issues.";
            
              
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
            btnAzure.Visible = false;
            btnInstructions.Visible = false;
            txtDetails.Visible = true;
            //tabControl1.Visible = false;
            panel1.Visible = false;
            txtDetails.Text = "GPH の問題を解決したい場合は、下のボタンをクリックしてください。以下のソリューションを実行するには、Avecto Rights が必要です。" + Environment.NewLine+Environment.NewLine+
                "If you want to fix GPH issue, click on the button below. Avecto Rights must be needed to execute the below solution.";
            scriptname = "GPH Fix";

            fileName = "C:\\temp\\GPH_Remediation.exe";
            remoteUri = "https://quickitsupport.gdn.accenture.com//solution/japan_solutions/GPH_Remediation.exe";

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
