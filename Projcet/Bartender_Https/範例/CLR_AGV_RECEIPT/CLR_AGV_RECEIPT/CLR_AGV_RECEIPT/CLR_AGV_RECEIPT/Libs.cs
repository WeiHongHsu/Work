using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using CLR_AGV_RECEIPT;

namespace CLR_AGV_RECEIPT
{
    class Libs
    {
        
        public static DataTable GetErrorTable(String errorMessage)
        {
            DataTable dtError = new DataTable();
            dtError.TableName = "Error";
            dtError.Columns.Add("Message");
            DataRow row = dtError.Rows.Add();
            row["Message"] = errorMessage;

            return dtError;
        }


        //針對DB的TABLE資料組成JSON內容
        public static string Data_Json(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            if (dt.Columns.Count > 0)//判?符合表
            {
                DataRowCollection drc = dt.Rows;
                for (int i = 0; i < drc.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string strKey = dt.Columns[j].ColumnName;
                        string strValue = drc[i][j].ToString();
                        if (drc[i][j] == DBNull.Value)
                        { strValue = "null"; }
                        //if (strValue == "") continue;
                        Type type = dt.Columns[j].DataType;
                        jsonString.Append("\"" + strKey + "\":");
                        strValue = String.Format(strValue, type);

                        //修改判斷最後一行不加上,
                        if (j == dt.Columns.Count - 1)
                        {
                            if (strValue == "null")
                            {
                                jsonString.Append("" + strValue + "");
                            }
                            else
                            {
                                if (type.Name == "Char")
                                {
                                    jsonString.Append("\"" + strValue + "\"");
                                }
                                else if (type.Name == "String")
                                {
                                    jsonString.Append("\"" + strValue + "\"");
                                }
                                else
                                {
                                    jsonString.Append("" + strValue + "");
                                }
                            }                       
                        }
                        else
                        {
                            if (strValue == "null")
                            {
                                jsonString.Append("" + strValue + ",");
                            }
                            else
                            {
                                if (type.Name == "Char")
                                {
                                    jsonString.Append("\"" + strValue + "\",");
                                }
                                else if (type.Name == "String")
                                {
                                    jsonString.Append("\"" + strValue + "\",");
                                }
                                else
                                {
                                    jsonString.Append("" + strValue + ",");
                                }
                            }
                                
                        }
                    }

                    //判斷當有多筆資料時,需組合{}
                    if (drc.Count > 1 && i != drc.Count - 1)
                    {
                        jsonString.Append("}, {");
                    }
                }

            }
            // jsonString.Remove(jsonString.Length - 1, 1);
            return jsonString.ToString();
        }

        //JSON發送方式一
        public static string Post(string jsonParas)
        {
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(config.StrURL());
            request.Method = "POST";
            request.Timeout = 1000000;
            request.ReadWriteTimeout = 1000000;
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = jsonParas;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = null;
            try
            {
                writer = request.GetRequestStream();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response = null;
            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string strHtml = sr.ReadToEnd();
                return strHtml;
            }
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

        //JSON發送方式二
        public static string PostKen(string jsonPara)
        {
            using (WebClient webClient = new WebClient())
            {
                // 指定 WebClient 編碼
                webClient.Encoding = Encoding.UTF8;
                // 指定 WebClient 的 Content-Type header
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                // 指定 WebClient 的 authorization header
                //webClient.Headers.Add("authorization", "token {apitoken}");
                
                // 執行 post 動作
                var result = webClient.UploadString(config.StrURL(), jsonPara);

                return result.ToString();
            }
        }
    }


}
