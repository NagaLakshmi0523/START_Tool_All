using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Windows.Forms;

namespace START_Tool
{
    public class JSonConverterCHN
    {
        public DataTable catdt=new DataTable();

       public JSonConverterCHN()
        {
            GetCatData();

        }
        public string GetCatData()
        {
            string response = "";
            try
            {
                string data;
                using (var wb = new WebClient())
                {
                    data = Environment.UserName; ;


                    response = wb.DownloadString("https://quickitsupport.gdn.accenture.com/starttool_china/getcat.php");
                    //string responseInString = Encoding.UTF8.GetString(response);


                }
                 catdt= JsonStringToDataTable(response);
            }
            catch (System.Net.WebException wbex)
            {
                MessageBox.Show("Connection failed! please check your internet connection.");
            }
            return response;
        }
        public static DataTable JsonStringToDataTable(string encrypted)
        {
            CipherConverter cconvert = new CipherConverter();

            // FeedbackForm ff = new FeedbackForm();
            //byte[] key = ff.mySHA256.ComputeHash(Encoding.ASCII.GetBytes(ff.password));
            var jsonString = cconvert.DecryptString(encrypted);
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
            return dt;
        }

    }
}
