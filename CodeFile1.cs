using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace START_Tool {
    public class CommonUtility
    {
        public string convertObjectToJson(Object obj)
        {
            StringBuilder jsonString = new StringBuilder();
            try
            {
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                jsonSerializer.MaxJsonLength = Int32.MaxValue;

                jsonSerializer.Serialize(obj, jsonString);
                //logger.Debug("Request = " + jsonString.ToString());
            }
            catch
            {
                // We are Commenting this code because we found circular dependancy , in this method whwn exception is come in Serialize object
                // LogManager.GetLogger().Exception(ex, true);
            }
            return jsonString.ToString();
        }

        public T convertJsonToObject<T>(string json) where T : class
        {
            try
            {
                JavaScriptSerializer jasonSerializer = new JavaScriptSerializer();
                jasonSerializer.MaxJsonLength = Int32.MaxValue;
                //logger.Debug("Response = " + json);
                return jasonSerializer.Deserialize<T>(json);
            }
            catch (Exception )
            {  }
            return null;
        }
    }
}
