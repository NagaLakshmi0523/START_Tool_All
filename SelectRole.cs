using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Security;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace START_Tool
{
    public partial class SelectRole : Form
    {
        public string srrole = "";      
        bool ifFileExist = true;
        public event EventHandler ComboBoxSelectionChanged;
        public string countryName;
        bool check =false;

        public SelectRole()
        {
            InitializeComponent();         
        }

        private void SelectRole_Load(object sender, EventArgs e)
        {

        }

        private void SelectRole_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            srrole = cmbRole.SelectedItem.ToString();
        }

        private void SaveHits()
        {

            string password = "8R@13#s34Af";

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();


            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            if (checkData())
            {
                //MessageBox.Show("Please select the Region!","Prerequisites", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Show();
            }
           
            else
            {
                if (rdbtnInd.Checked)
                {
                    try
                    {
                        string data;
                        using (var wb = new WebClient())
                        {

                            data = "userName :" + Environment.UserName + ", version:1.5";

                            /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                             MessageBox.Show(dataserialise);*/
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                            string encrypted = this.EncryptString(data, key, iv);
                            // MessageBox.Show(encrypted);

                            var senddata = new NameValueCollection();
                            senddata.Add("d1", encrypted);

                            //if(formIND ==)
                            var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool/starthits_new.php", "POST", senddata);
                            // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

                            string responseInString = Encoding.UTF8.GetString(response);
                        }
                    }
                    catch (System.Net.WebException wbex)
                    {
                        MessageBox.Show("Connection failed! please check your internet connection.");
                    }
                    string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool/CheckStart.txt";
                    try
                    {

                        HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
                        request.Method = "HEAD";
                        using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                        {
                            ifFileExist = true;

                        }

                    }
                    catch (WebException we)
                    {
                        MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                        ifFileExist = false;

                    }

                }
                else if (rdtbtnSea.Checked)
                {
                    try
                    {


                        string data;
                        using (var wb = new WebClient())
                        {

                            data = "userName :" + Environment.UserName + ", version:1.5";

                            /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                             MessageBox.Show(dataserialise);*/
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                            string encrypted = this.EncryptString(data, key, iv);
                            // MessageBox.Show(encrypted);

                            var senddata = new NameValueCollection();
                            senddata.Add("d1", encrypted);

                            //if(formIND ==)
                            var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_sea/starthits_new.php", "POST", senddata);
                            // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

                            string responseInString = Encoding.UTF8.GetString(response);


                        }
                    }
                    catch (System.Net.WebException wbex)
                    {
                        MessageBox.Show("Connection failed! please check your internet connection.");
                    }
                    string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool_sea/CheckStart.txt";
                    try
                    {

                        HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
                        request.Method = "HEAD";
                        using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                        {
                            ifFileExist = true;

                        }

                    }
                    catch (WebException we)
                    {
                        MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                        ifFileExist = false;

                    }
                }
                else if (rbtChina.Checked)
                {
                    try
                    {


                        string data;
                        using (var wb = new WebClient())
                        {

                            data = "userName :" + Environment.UserName + ", version:1.5";

                            /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                             MessageBox.Show(dataserialise);*/
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                            string encrypted = this.EncryptString(data, key, iv);
                            // MessageBox.Show(encrypted);

                            var senddata = new NameValueCollection();
                            senddata.Add("d1", encrypted);

                            //if(formIND ==)
                            var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/starthits.php", "POST", senddata);
                            // var response = wb.UploadValues("http://localhost:8080/starttool_china/starthit.php", "POST", senddata);

                            string responseInString = Encoding.UTF8.GetString(response);


                        }
                    }
                    catch (System.Net.WebException wbex)
                    {
                        MessageBox.Show("Connection failed! please check your internet connection.");
                    }
                    string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool_china/CheckStart.txt";
                    try
                    {

                        HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
                        request.Method = "HEAD";
                        using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                        {
                            ifFileExist = true;

                        }

                    }
                    catch (WebException we)
                    {
                        MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                        ifFileExist = false;

                    }
                }
                else if (rbtPh.Checked)
                {
                    try
                    {


                        string data;
                        using (var wb = new WebClient())
                        {

                            data = "userName :" + Environment.UserName + ", version:1.5";

                            /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                             MessageBox.Show(dataserialise);*/
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                            string encrypted = this.EncryptString(data, key, iv);
                            // MessageBox.Show(encrypted);

                            var senddata = new NameValueCollection();
                            senddata.Add("d1", encrypted);

                            //if(formIND ==)
                            var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_ph/starthits.php", "POST", senddata);
                            // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

                            string responseInString = Encoding.UTF8.GetString(response);


                        }
                    }
                    catch (System.Net.WebException wbex)
                    {
                        MessageBox.Show("Connection failed! please check your internet connection.");
                    }
                    string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool_ph/CheckStart.txt";
                    try
                    {

                        HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
                        request.Method = "HEAD";
                        using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                        {
                            ifFileExist = true;

                        }

                    }
                    catch (WebException we)
                    {
                        MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                        ifFileExist = false;

                    }
                }
                else if (rbnAunz.Checked)
                {
                    try
                    {


                        string data;
                        using (var wb = new WebClient())
                        {

                            data = "userName :" + Environment.UserName + ", version:1.5";

                            /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                             MessageBox.Show(dataserialise);*/
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                            string encrypted = this.EncryptString(data, key, iv);
                            // MessageBox.Show(encrypted);

                            var senddata = new NameValueCollection();
                            senddata.Add("d1", encrypted);

                            //if(formIND ==)
                            var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_anz/starthits.php", "POST", senddata);
                            // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

                            string responseInString = Encoding.UTF8.GetString(response);


                        }

                    }
                    catch (System.Net.WebException wbex)
                    {
                        MessageBox.Show("Connection failed! please check your internet connection.");
                    }
                    string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool_anz/CheckStart.txt";
                    try
                    {

                        HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
                        request.Method = "HEAD";
                        using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                        {
                            ifFileExist = true;

                        }

                    }
                    catch (WebException we)
                    {
                        MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                        ifFileExist = false;

                    }
                }
                else if (rdbJapan.Checked)
                {
                    try
                    {
                        string data;
                        using (var wb = new WebClient())
                        {

                            data = "userName :" + Environment.UserName + ", version:1.5";

                            /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                             MessageBox.Show(dataserialise);*/
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                            string encrypted = this.EncryptString(data, key, iv);
                            // MessageBox.Show(encrypted);

                            var senddata = new NameValueCollection();
                            senddata.Add("d1", encrypted);

                            //if(formIND ==)
                            var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_japan/starthits.php", "POST", senddata);
                            // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

                            string responseInString = Encoding.UTF8.GetString(response);


                        }
                    }
                    catch (System.Net.WebException wbex)
                    {
                        MessageBox.Show("Connection failed! please check your internet connection.");
                    }
                    string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool_japan/CheckStart.txt";
                    try
                    {

                        HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
                        request.Method = "HEAD";
                        using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                        {
                            ifFileExist = true;

                        }

                    }
                    catch (WebException we)
                    {
                        MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
                        ifFileExist = false;

                    }
                }
            }
            //else
            //{
            //    switch ()
            //    {
            //        case "India":
            //            try
            //            {
            //                string data;
            //                using (var wb = new WebClient())
            //                {

            //                    data = "userName :" + Environment.UserName + ", version:1.5";

            //                    /* var dataserialise = new JavaScriptSerializer().Serialize(data);
            //                     MessageBox.Show(dataserialise);*/
            //                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            //                    string encrypted = this.EncryptString(data, key, iv);
            //                    // MessageBox.Show(encrypted);

            //                    var senddata = new NameValueCollection();
            //                    senddata.Add("d1", encrypted);

            //                    //if(formIND ==)
            //                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool/starthits_new.php", "POST", senddata);
            //                    // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

            //                    string responseInString = Encoding.UTF8.GetString(response);


            //                }
            //            }
            //            catch (System.Net.WebException wbex)
            //            {
            //                MessageBox.Show("Connection failed! please check your internet connection.");
            //            }
            //            string checkLinkIND = "https://quickitsupport.gdn.accenture.com//starttool/CheckStart.txt";
            //            try
            //            {

            //                HttpWebRequest request = WebRequest.Create(checkLinkIND) as HttpWebRequest;
            //                request.Method = "HEAD";
            //                using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
            //                {
            //                    ifFileExist = true;

            //                }

            //            }
            //            catch (WebException we)
            //            {
            //                MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
            //                ifFileExist = false;

            //            }
            //            break;
            //        case "China":
            //            try
            //            {
            //                string data;
            //                using (var wb = new WebClient())
            //                {

            //                    data = "userName :" + Environment.UserName + ", version:1.5";

            //                    /* var dataserialise = new JavaScriptSerializer().Serialize(data);
            //                     MessageBox.Show(dataserialise);*/
            //                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            //                    string encrypted = this.EncryptString(data, key, iv);
            //                    // MessageBox.Show(encrypted);

            //                    var senddata = new NameValueCollection();
            //                    senddata.Add("d1", encrypted);

            //                    //if(formIND ==)
            //                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/starthits.php", "POST", senddata);
            //                    // var response = wb.UploadValues("http://localhost:8080/starttool_china/starthit.php", "POST", senddata);

            //                    string responseInString = Encoding.UTF8.GetString(response);


            //                }
            //            }
            //            catch (System.Net.WebException wbex)
            //            {
            //                MessageBox.Show("Connection failed! please check your internet connection.");
            //            }
            //            string checkLinkCHN = "https://quickitsupport.gdn.accenture.com//starttool_china/CheckStart.txt";
            //            try
            //            {

            //                HttpWebRequest request = WebRequest.Create(checkLinkCHN) as HttpWebRequest;
            //                request.Method = "HEAD";
            //                using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
            //                {
            //                    ifFileExist = true;

            //                }

            //            }
            //            catch (WebException we)
            //            {
            //                MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
            //                ifFileExist = false;

            //            }
            //            break;
            //        case "Philippines":
            //            try
            //            {
            //                string data;
            //                using (var wb = new WebClient())
            //                {

            //                    data = "userName :" + Environment.UserName + ", version:1.5";

            //                    /* var dataserialise = new JavaScriptSerializer().Serialize(data);
            //                     MessageBox.Show(dataserialise);*/
            //                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            //                    string encrypted = this.EncryptString(data, key, iv);
            //                    // MessageBox.Show(encrypted);

            //                    var senddata = new NameValueCollection();
            //                    senddata.Add("d1", encrypted);

            //                    //if(formIND ==)
            //                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_ph/starthits.php", "POST", senddata);
            //                    // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

            //                    string responseInString = Encoding.UTF8.GetString(response);


            //                }
            //            }
            //            catch (System.Net.WebException wbex)
            //            {
            //                MessageBox.Show("Connection failed! please check your internet connection.");
            //            }
            //            string checkLinkPH = "https://quickitsupport.gdn.accenture.com//starttool_ph/CheckStart.txt";
            //            try
            //            {

            //                HttpWebRequest request = WebRequest.Create(checkLinkPH) as HttpWebRequest;
            //                request.Method = "HEAD";
            //                using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
            //                {
            //                    ifFileExist = true;

            //                }

            //            }
            //            catch (WebException we)
            //            {
            //                MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
            //                ifFileExist = false;

            //            }
            //            break;
            //        case "SEA":
            //            try
            //            {
            //                string data;
            //                using (var wb = new WebClient())
            //                {

            //                    data = "userName :" + Environment.UserName + ", version:1.5";

            //                    /* var dataserialise = new JavaScriptSerializer().Serialize(data);
            //                     MessageBox.Show(dataserialise);*/
            //                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            //                    string encrypted = this.EncryptString(data, key, iv);
            //                    // MessageBox.Show(encrypted);

            //                    var senddata = new NameValueCollection();
            //                    senddata.Add("d1", encrypted);

            //                    //if(formIND ==)
            //                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_sea/starthits_new.php", "POST", senddata);
            //                    // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

            //                    string responseInString = Encoding.UTF8.GetString(response);


            //                }
            //            }
            //            catch (System.Net.WebException wbex)
            //            {
            //                MessageBox.Show("Connection failed! please check your internet connection.");
            //            }
            //            string checkLinkSEA = "https://quickitsupport.gdn.accenture.com//starttool_sea/CheckStart.txt";
            //            try
            //            {

            //                HttpWebRequest request = WebRequest.Create(checkLinkSEA) as HttpWebRequest;
            //                request.Method = "HEAD";
            //                using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
            //                {
            //                    ifFileExist = true;

            //                }

            //            }
            //            catch (WebException we)
            //            {
            //                MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
            //                ifFileExist = false;

            //            }
            //            break;
            //        case "Australia and NewZealand":
            //            try
            //            {
            //                string data;
            //                using (var wb = new WebClient())
            //                {

            //                    data = "userName :" + Environment.UserName + ", version:1.5";

            //                    /* var dataserialise = new JavaScriptSerializer().Serialize(data);
            //                     MessageBox.Show(dataserialise);*/
            //                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            //                    string encrypted = this.EncryptString(data, key, iv);
            //                    // MessageBox.Show(encrypted);

            //                    var senddata = new NameValueCollection();
            //                    senddata.Add("d1", encrypted);

            //                    //if(formIND ==)
            //                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_anz/starthits.php", "POST", senddata);
            //                    // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

            //                    string responseInString = Encoding.UTF8.GetString(response);


            //                }
            //            }
            //            catch (System.Net.WebException wbex)
            //            {
            //                MessageBox.Show("Connection failed! please check your internet connection.");
            //            }
            //            string checkLinkAUZ = "https://quickitsupport.gdn.accenture.com//starttool_anz/CheckStart.txt";
            //            try
            //            {

            //                HttpWebRequest request = WebRequest.Create(checkLinkAUZ) as HttpWebRequest;
            //                request.Method = "HEAD";
            //                using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
            //                {
            //                    ifFileExist = true;

            //                }

            //            }
            //            catch (WebException we)
            //            {
            //                MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
            //                ifFileExist = false;

            //            }
            //            break;
            //        case "United Kingdom and Ireland":
            //            try
            //            {
            //                string data;
            //                using (var wb = new WebClient())
            //                {

            //                    data = "userName :" + Environment.UserName + ", version:1.5";

            //                    /* var dataserialise = new JavaScriptSerializer().Serialize(data);
            //                     MessageBox.Show(dataserialise);*/
            //                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            //                    string encrypted = this.EncryptString(data, key, iv);
            //                    // MessageBox.Show(encrypted);

            //                    var senddata = new NameValueCollection();
            //                    senddata.Add("d1", encrypted);

            //                    //if(formIND ==)
            //                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_uk/starthits.php", "POST", senddata);
            //                    // var response = wb.UploadValues("http://localhost:8080/starttool/starthit.php", "POST", senddata);

            //                    string responseInString = Encoding.UTF8.GetString(response);


            //                }
            //            }
            //            catch (System.Net.WebException wbex)
            //            {
            //                MessageBox.Show("Connection failed! please check your internet connection.");
            //            }
            //            string checkLinkUKI = "https://quickitsupport.gdn.accenture.com//starttool_uk/CheckStart.txt";
            //            try
            //            {

            //                HttpWebRequest request = WebRequest.Create(checkLinkUKI) as HttpWebRequest;
            //                request.Method = "HEAD";
            //                using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
            //                {
            //                    ifFileExist = true;

            //                }

            //            }
            //            catch (WebException we)
            //            {
            //                MessageBox.Show("We are Working to Deploy the New Solution Soon, Please Check After Sometime");
            //                ifFileExist = false;

            //            }
            //            break;
                
            //    }
            //}
            
        }
        private bool checkData()
        {
            check = false;
            if (rdbtnInd.Checked == false && rdtbtnSea.Checked == false && rbtChina.Checked == false && rbtPh.Checked == false && rbnAunz.Checked==false && rdbJapan.Checked == false)
            {
                check = true;

            }
            if (check)
            {

                //MessageBox.Show("Please Select the Region!", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return check;
        }

        public void btnSubmit_Click(object sender, EventArgs e)
        {
            
            SaveHits();
            if (ifFileExist)
            {
               
                if(checkData())
               // if (CountryNamesComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select the Region!", "Prerequisites", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Show();
                }
                else
                {
                    if (rdbtnInd.Checked)
                    {

                        srrole = "open";
                        this.Hide();
                        JSonConverterIND jc = new JSonConverterIND();
                        var formIND = new HomeFormInd(jc.catdt);
                        formIND.ShowDialog();


                    }
                    else if (rdtbtnSea.Checked)
                    {
                        JSonConverterSEA jSonConverterSEA = new JSonConverterSEA();
                        var formSEA = new HomeFormSEA(jSonConverterSEA.catdt);
                        srrole = "open";
                        this.Hide();
                        formSEA.ShowDialog();

                    }
                    else if (rbtChina.Checked)
                    {
                        JSonConverterCHN jSonConverterCHN = new JSonConverterCHN();
                        var formCHN = new HomeFormCHN(jSonConverterCHN.catdt);
                        srrole = "open";
                        this.Hide();
                        formCHN.ShowDialog();

                    }
                    else if (rbtPh.Checked)
                    {
                        JSonConverterPH jSonConverterPH = new JSonConverterPH();
                        var formPH = new HomeFormPH(jSonConverterPH.catdt);
                        srrole = "open";
                        this.Hide();
                        formPH.ShowDialog();


                    }
                    else if(rbnAunz.Checked)
                    {
                        JSonConverterAUNZ jSonConverterAUNZ = new JSonConverterAUNZ();
                        var formAUNZ = new HomeFormAUNZ(jSonConverterAUNZ.catdt);
                        srrole = "open";
                        this.Hide();
                        formAUNZ.ShowDialog();
                    }
                    else if (rdbJapan.Checked)
                    {
                        JSonConverterJapan jSonConverterJapan = new JSonConverterJapan();
                        var formJapan = new HomeFormJapan(jSonConverterJapan.catdt);
                        srrole = "open";
                        this.Hide();
                        formJapan.ShowDialog();
                    }
                    //else if (selectItem == "United Kingdom and Ireland")
                    //{
                    //    JSonConverterUKI jSonConverterUKI = new JSonConverterUKI();
                    //    var fromUKI = new HomeFormUKI(jSonConverterUKI.catdt);
                    //    srrole = "open";
                    //    this.Hide();
                    //    fromUKI.ShowDialog();

                    //}
                    //else if (selectItem == "Australia and NewZealand")
                    //{
                    //    JSonConverterAUNZ jSonConverterAUNZ = new JSonConverterAUNZ();
                    //    var fromAUNZ = new HomeFormAUNZ(jSonConverterAUNZ.catdt);
                    //    srrole = "open";
                    //    this.Hide();
                    //    fromAUNZ.ShowDialog();


                    //}
                }

            }
            else
            {
                srrole = "";
                this.Close();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        public string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cipherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

            // Return the encrypted data as a string
            return cipherText;
        }

        private void cmbRole_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        

        public void CountryNamesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //countryName = CountryNamesComboBox.SelectedItem.ToString();
        }

        private void linkLblStart_Click(object sender, EventArgs e)
        {
            SaveHits();
            if (ifFileExist)
            {

                if (checkData())
                // if (CountryNamesComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select the Region!", "Prerequisites", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Show();
                }
                else
                {

                    //string checkIND = rdbtnInd.Checked.ToString();
                    //string checkSEA = rdtbtnSea.Checked.ToString();
                    //string checkChina = rbtChina.Checked.ToString();
                    //string checkPH = rbtPh.Checked.ToString();
                    if (rdbtnInd.Checked)
                    {

                        srrole = "open";
                        this.Hide();
                        JSonConverterIND jc = new JSonConverterIND();
                        var formIND = new HomeFormInd(jc.catdt);
                        formIND.ShowDialog();


                    }
                    else if (rdtbtnSea.Checked)
                    {
                        JSonConverterSEA jSonConverterSEA = new JSonConverterSEA();
                        var formSEA = new HomeFormSEA(jSonConverterSEA.catdt);
                        srrole = "open";
                        this.Hide();
                        formSEA.ShowDialog();

                    }
                    else if (rbtChina.Checked)
                    {
                        JSonConverterCHN jSonConverterCHN = new JSonConverterCHN();
                        var formCHN = new HomeFormCHN(jSonConverterCHN.catdt);
                        srrole = "open";
                        this.Hide();
                        formCHN.ShowDialog();

                    }
                    else if (rbtPh.Checked)
                    {
                        JSonConverterPH jSonConverterPH = new JSonConverterPH();
                        var formPH = new HomeFormPH(jSonConverterPH.catdt);
                        srrole = "open";
                        this.Hide();
                        formPH.ShowDialog();


                    }
                    else if (rbnAunz.Checked)
                    {
                        JSonConverterAUNZ jSonConverterAUNZ = new JSonConverterAUNZ();
                        var fromAUNZ = new HomeFormAUNZ(jSonConverterAUNZ.catdt);
                        srrole = "open";
                        this.Hide();
                        fromAUNZ.ShowDialog();
                    }
                    //else if (selectItem == "United Kingdom and Ireland")
                    //{
                    //    JSonConverterUKI jSonConverterUKI = new JSonConverterUKI();
                    //    var fromUKI = new HomeFormUKI(jSonConverterUKI.catdt);
                    //    srrole = "open";
                    //    this.Hide();
                    //    fromUKI.ShowDialog();

                    //}
                    //else if (selectItem == "Australia and NewZealand")
                    //{
                    //    JSonConverterAUNZ jSonConverterAUNZ = new JSonConverterAUNZ();
                    //    var fromAUNZ = new HomeFormAUNZ(jSonConverterAUNZ.catdt);
                    //    srrole = "open";
                    //    this.Hide();
                    //    fromAUNZ.ShowDialog();


                    //}
                }

            }
            else
            {
                srrole = "";
                this.Close();
            }
        }
    }
}
