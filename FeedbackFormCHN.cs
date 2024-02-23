using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace START_Tool
{
    public partial class FeedbackFormCHN : Form
    {
        string hostName = "xyz";
        Boolean check = false;
        public bool submited = false;
        string password = "8R@13#s34Af";
       // HomeForm category1 =new HomeForm();
       public List<string> subCatList = new List<string>();
        List<string> catList = new List<string>();
        // Create sha256 hash
        SHA256 mySHA256 = SHA256Managed.Create();
        DataTable dtCategory=new DataTable();
        // Create secret IV
        byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        //string encrypted = "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";
        string catdata;
        string catstr = "";
        string substr = "";
        public FeedbackFormCHN(List<string> lst,DataTable dt)
        {
            dtCategory = dt;
             InitializeComponent();
            subCatList = lst;
            createCatList();

        }


        public void createCatList()
        {
            bool isNotEmpty = subCatList.Any();
            if (isNotEmpty)
            {
                try
                {
                    foreach (var cat in subCatList)
                    {
                        //string c = cat;
                        DataRow[] row = dtCategory.Select("[sub_category]='" + cat + "'");
                        //MessageBox.Show(row[0]["category_id"].ToString() + " - " + row[0]["sub category"].ToString());
                        catList.Add(row[0]["category_id"].ToString());

                    }
                    string  delim = ";";

                     substr = string.Join(delim, subCatList);
                     catstr = string.Join(delim, catList);
                    
                }
                catch (IndexOutOfRangeException iob)
                {

                }
                // 0 - item 0
            } // 1 - item 1
        }
        FeedbackFormCHN() { }
        public static DataTable jsonToDataTable()
        {// string jsonString argument
            FeedbackFormCHN ff = new FeedbackFormCHN();
           byte[] key = ff.mySHA256.ComputeHash(Encoding.ASCII.GetBytes(ff.password));

            string encrypted = "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";
            var jsonString = DecryptString(encrypted, key, ff.iv);
            //MessageBox.Show(jsonString.ToString());
            var jsonLinq = JArray.Parse(jsonString);
            
            // Find the first array using Linq  
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types  
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }
                trgArray.Add(cleanRow);
            }

            //MessageBox.Show(trgArray.ToString());

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }
       
        public string GetCatData()
        {
            string response="";
            try
            {
                string data;
                using (var wb = new WebClient())
                {
                    data = Environment.UserName; ;

       
                     response = wb.DownloadString("https://quickitsupport.gdn.accenture.com/starttool_china/getcat.php");
                    //string responseInString = Encoding.UTF8.GetString(response);
                  

                }
                JsonStringToDataTable(response);
            }
            catch (System.Net.WebException wbex)
            {
                MessageBox.Show("Connection failed! please check your internet connection.");
            }
            return response;
        }


        
        public  void JsonStringToDataTable(string encrypted)
        {
           // FeedbackForm ff = new FeedbackForm();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            var jsonString = DecryptString(encrypted, key, iv);
           DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            dtCategory = dt;
           // return dt;
        }


        private void FeedbackForm_Load(object sender, EventArgs e)
        {
            hostName = Environment.UserName;
            lblUser.Text = hostName;
            lblsubcategory.Text = substr;
            lblCat.Text = catstr;
        }

        private void rdbtnNo_CheckedChanged(object sender, EventArgs e)
        {




            if (rdbtnNo.Checked == true)
            {
                panelafterno.Visible = true;
              
            }
            if(rdbtnYes.Checked==true)
            {
                panelafterno.Visible = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo solution = new DirectoryInfo("C:\\temp\\");


            int i = 0;
            if (!checkData())
            {
               /* try
                {
                    foreach (FileInfo file in solution.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in solution.GetDirectories())
                    {
                        dir.Delete(true); //delete subdirectories and files
                    }
                }
                catch(Exception ex)
                {
                    //MessageBox.Show("Execution is in progress please wait!");
                    i = 1;
                }
               */
                string rating = "average";
                string resolve = "yes";
                if (rdbtnExc.Checked)
                {
                    rating = "excellent";
                }
                else if (rdbtnFair.Checked) { rating = "fair"; }
                else if (rdbtnAvg.Checked) { rating = "average"; }
                else if (rdbtnGood.Checked) { rating = "good"; }
                else if (rdbtnPoor.Checked) { rating = "poor"; }
                if (rdbtnYes.Checked)
                {
                    resolve = "yes";

                }
                else
                {
                    resolve = "no";
                }
                
                try
                {
                    using (var wb = new WebClient())
                    {

                        string data="";

                            string txt = "";
                            if(txtComments.Text=="")
                            {
                                txt = "no comments";
                            }
                            else
                            {
                                txt=txtComments.Text;
                            }
                            data = "username:" + hostName + ",category :" + lblCat.Text.ToString() + ", sbcategory :" + lblsubcategory.Text.ToString() +
                                ", rating :" + rating + ", comments :" +txt + ",resolve:" + resolve + ",location:" + cbLocation.SelectedItem.ToString() + ", version:1.5";
                        //}




                        /* var dataserialise = new JavaScriptSerializer().Serialize(data);
                         MessageBox.Show(dataserialise);*/
                        byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                        string encrypted = this.EncryptString(data, key, iv);
                        // MessageBox.Show(encrypted);

                        var senddata = new NameValueCollection();
                        senddata.Add("d1", encrypted);

                        var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/savefeedback.php", "POST", senddata);
                        //var response = wb.UploadValues("http://localhost:8080/starttool/savefeedback.php", "POST", senddata);

                        string responseInString = Encoding.UTF8.GetString(response);
                    }

                }
                catch (System.Net.WebException wbex)
                {
                    MessageBox.Show("Connection failed! please check your internet connection.");
                }

               
                submited = true;
                MessageBox.Show("Thank you for your valuable Feedback");

                this.Close();
                Environment.Exit(0);
            }

        }

        /*  private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
          {
              if (cbCategory.SelectedItem.ToString() == "Popular Solutions")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                  //panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("Citrix issue");
                  cbSubCategory.Items.Add("Unsecure Device");
                  cbSubCategory.Items.Add("Free Disk Space");
                  cbSubCategory.Items.Add("Azure MFA");
                  cbSubCategory.Items.Add("Admin Rights");
                  cbSubCategory.Items.Add("Patching Issue Fix");
                  cbSubCategory.Items.Add("VPN Issue");
                  cbSubCategory.Items.Add("Reset Password");
                  cbSubCategory.Items.Add("Change Password");

                  cbSubCategory.Items.Add("Check Compliance Status");



              }
              if (cbCategory.SelectedItem.ToString() == "Quick Solutions")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                  //panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("Update Ms office");

                  cbSubCategory.Items.Add("Admin Rights");
                  cbSubCategory.Items.Add("Remove unauthorised ID/ Avecto");
                  cbSubCategory.Items.Add("Citrix Reciever reinstallation");
                  cbSubCategory.Items.Add("Free Disk Space");
                  cbSubCategory.Items.Add("Reset Password");
                  cbSubCategory.Items.Add("Patching Issue Fix");
                  cbSubCategory.Items.Add("MS Teams Meeting Addin Issue");
                  cbSubCategory.Items.Add("Unsecure Device");
                  cbSubCategory.Items.Add("Azure MFA");
                  cbSubCategory.Items.Add("Frequently Faced Issue");
                  cbSubCategory.Items.Add("Enterprise ID Change request");
                  cbSubCategory.Items.Add("MS teams Display Name Change");
              }
              if (cbCategory.SelectedItem.ToString() == "System Issue")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                 // panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("Free Disk Space");

                  cbSubCategory.Items.Add("Unsecure Device fix");
                  cbSubCategory.Items.Add("Remove Unauthorised id/Avecto ");
                  cbSubCategory.Items.Add("Delete Saved Passwords");

                  cbSubCategory.Items.Add("Reset and Unlock Password");
                  cbSubCategory.Items.Add("Change Password");
                  cbSubCategory.Items.Add("Azure MFA");


              }
              if (cbCategory.SelectedItem.ToString() == "Application")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                  //panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("Outlook Fix");
                  cbSubCategory.Items.Add("MS Team Fix");
                  cbSubCategory.Items.Add("Citrix Fix");
                  cbSubCategory.Items.Add("VPN Fix");
                  cbSubCategory.Items.Add("GPH Fix");
                  cbSubCategory.Items.Add("Microsoft Application Related Links");
                  cbSubCategory.Items.Add("Onedrive Solutions");
                  cbSubCategory.Items.Add("Mobile Device(MDM)");

                  //cbSubCategory.Items.Add("Free disk space");
              }
              if (cbCategory.SelectedItem.ToString() == "Network Issue")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                 // panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("Wifi - Guest Access Registration");
                  cbSubCategory.Items.Add("Pulse Secure Reinstallation");

              }
              if (cbCategory.SelectedItem.ToString() == "Compliance")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                 // panel1.Visible = false;
                  lblSubCat.Visible = true;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("Overall NC Remediation");
                  cbSubCategory.Items.Add("Patching Fix"); 
                  cbSubCategory.Items.Add("Polling Fix");
                  cbSubCategory.Items.Add("Encryption Fix");
                  cbSubCategory.Items.Add("Chrome Fix");
                  cbSubCategory.Items.Add("Secure Web Fix");
                  cbSubCategory.Items.Add("Tanium Fix");
                  cbSubCategory.Items.Add("Eeva Fix");
                  cbSubCategory.Items.Add("Office 365 Fix");
                  cbSubCategory.Items.Add("Check Java NC Remediation");
                  cbSubCategory.Items.Add("Check FishMe/Cofense Reporter Check");
                  cbSubCategory.Items.Add("Check BeyondTrust PM Fix");
                  cbSubCategory.Items.Add("Antivirus Fix");

                  cbSubCategory.Items.Add("Firewall check");
                  cbSubCategory.Items.Add("DLP check");
                  cbSubCategory.Items.Add("Adobe Reader Check");
                  cbSubCategory.Items.Add("EVTA Check");
                  cbSubCategory.Items.Add("Adobe Creative Cloud Suite Check");
                  cbSubCategory.Items.Add("Domain Membership Check");
                  cbSubCategory.Items.Add("Python Software Check");
                  cbSubCategory.Items.Add("Browser Check");
                  cbCategory.Items.Add("Unauthorized Software Check");
                  cbCategory.Items.Add("Operating System Check");




                  cbSubCategory.Items.Add("Check Device Complience Status");
              }

              if (cbCategory.SelectedItem.ToString() == "Asset Related Issue")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                //  panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("System Enablement");
                  cbSubCategory.Items.Add("Asset Decommission");
                  cbSubCategory.Items.Add("Asset Name Transfer");
                  cbSubCategory.Items.Add("Asset Return Request");
                  cbSubCategory.Items.Add("Asset Return Guidlines");

              }
              if (cbCategory.SelectedItem.ToString() == "Virtual Conference")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                 // panel1.Visible = false;
                  cbSubCategory.Items.Clear();
                  cbSubCategory.Items.Add("VC Registration Instruction");
                  cbSubCategory.Items.Add("VC Booking & Cost");
                  cbSubCategory.Items.Add("Live Event Support Request");


              }
              if (cbCategory.SelectedItem.ToString() == "Quick Links and Help")
              {
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  cbSubCategory.Visible = true;
                  lblSubCat.Visible = true;
                //  panel1.Visible = false;
                  cbSubCategory.Items.Clear();

                  cbSubCategory.Items.Add("My Asset");
                  cbSubCategory.Items.Add("Check Compliance");
                  cbSubCategory.Items.Add("Persons with Disablilities (PwD) Reasonable Accommodations");
                  cbSubCategory.Items.Add("Report Stolen Hardware");
                  cbSubCategory.Items.Add("Purchase Asset");
                  cbSubCategory.Items.Add("Protecting Accenture");
                  cbSubCategory.Items.Add("Information Security Exception Portal");
                  cbSubCategory.Items.Add("Application Information repository");
                  cbSubCategory.Items.Add("Software Catalogue");
                  cbSubCategory.Items.Add("My Learning Support");
                  cbSubCategory.Items.Add("ESO Related");
                  cbSubCategory.Items.Add("Frequently Faced Issue");
                  cbSubCategory.Items.Add("Accenture Laptop Configuration");
                  cbSubCategory.Items.Add("Enterprise ID Change request");
                  cbCategory.Items.Add("Third Party Risk Assessment (TPRA)");
                  cbCategory.Items.Add("Non-Standard Software Request (NSSR)");
                  cbCategory.Items.Add("New Joiner Laptop Support Desk");
                  cbCategory.Items.Add("Dedicated Support");
              }
              if (cbCategory.SelectedItem.ToString() == "Solution Not Available")
              {
                  cbSubCategory.Items.Clear();
                  panelsubCat.Visible = false;
                  panelMessage.Visible = true;
                  //panel1.Visible = false;

              }
              if (cbCategory.SelectedItem.ToString() == "Multiple Solutions/ Issues")
              {
                  cbSubCategory.Visible = false;
                  panelsubCat.Visible = true;
                  panelMessage.Visible = false;
                  lblSubCat.Visible = false;
                  // panelsubCat.Visible = false;
                  //panelMessage.Visible = true;
                  //panel1.Visible = true;

              }

          }*/
        private bool checkData()
        {
            check = false;
            /* if (cbCategory.Text == "")
             {
                 check = true;
             }
             else
                 {
                 if (cbCategory.SelectedItem.ToString() == "Solution Not Available")
                 {
                     if (cbCategory.Text == "" || txtComments.Text == "" || cbLocation.Text == "")
                     {
                         check = true;
                     }

                     if (rdbtnYes.Checked == false && rdbtnNo.Checked == false)
                     {
                         check = true;

                     }
                     if (check)
                     {

                         MessageBox.Show("Please fill feedback form completely !", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }
                 }

                 else if (cbCategory.Text == "Multiple Solutions/ Issues")
                 {
                     if (cbCategory.Text == "" || txtComments.Text == "" || cbLocation.Text == "")
                     {
                         check = true;
                     }
                     if (rdbtnExc.Checked == false && rdbtnAvg.Checked == false && rdbtnFair.Checked == false && rdbtnGood.Checked == false && rdbtnPoor.Checked == false)
                     {
                         check = true;

                     }

                     if (rdbtnYes.Checked == false && rdbtnNo.Checked == false)
                     {
                         check = true;

                     }
                     if (check)
                     {

                         MessageBox.Show("Please fill feedback form completely !", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }


                 }

                 else
                 {*/
            if (cbLocation.Text == "")
            {
                check = true;
            }
            if (rdbtnExc.Checked == false && rdbtnAvg.Checked == false && rdbtnFair.Checked == false && rdbtnGood.Checked == false && rdbtnPoor.Checked == false)
            {
                check = true;

            }
            if (rdbtnYes.Checked == false && rdbtnNo.Checked == false)
            {
                check = true;

            }
            if (check)
            {

                MessageBox.Show("Please fill feedback form completely !", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        //}

               
          //  }
            return check;
        } 
        public static string DecryptString(string cipherText, byte[] key, byte[] iv)
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
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new  CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            
            return plainText;
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
       
      
        private void txtComments_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\s]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        
        

        private void linkLabelloginc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            try
            {
                //check = false;
                //if(cbLocation.Text=="")
                //{

                //}
                using (var wb = new WebClient())
                {
                    //string locations = "";
                    if (cbLocation.Text == "")
                    {
                        MessageBox.Show("Please Fill the location!", "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        //locations = cbLocation.Text;
                    }
                    
                    
                    string incidentdata = "EnterpriseName:" + hostName + ",location:" + cbLocation.SelectedItem.ToString()+", version:1.5";

                    byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                    string encrypted = this.EncryptString(incidentdata, key, iv);
                  

                    var sendData = new NameValueCollection();
                    sendData.Add("d1", encrypted);
                    var response = wb.UploadValues("https://quickitsupport.gdn.accenture.com/starttool_china/incident.php", "POST", sendData);
                    //var response = wb.UploadValues("http://localhost:8080/starttool/incident.php", "POST", sendData);
                    string responseString = Encoding.UTF8.GetString(response);

                }
            }
            catch (System.Net.WebException wbex)
            {
                MessageBox.Show("Connection failed! please check your internet connection.");
            }
            Process.Start("https://support.accenture.com/support_portal?id=acn_service_catalog_dp&sys_id=c9856641139a6600380ddbf18144b05f");


        }

        private void linkLabelchatbot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://go.accenture.com/MyTechHelp");
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {

        }

        private void rdbtnExc_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FeedbackFormCHN_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
