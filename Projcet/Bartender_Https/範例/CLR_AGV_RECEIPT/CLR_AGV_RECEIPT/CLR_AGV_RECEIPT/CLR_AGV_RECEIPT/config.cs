using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace CLR_AGV_RECEIPT
{
    class config
    {
        //20200811改成實體INI檔讀取
        #region 寫死在CLR路徑 MARK CODE
        ////取得資料DB
        //public static string str_dataConn = "Data Source=192.168.1.11;Initial Catalog=PROD;Persist Security Info=True;User ID=sa;Password=zaq1";
        ////寫入LOGDB
        //public static string str_logConn = "Data Source=192.168.1.11;Initial Catalog=PROD;Persist Security Info=True;User ID=sa;Password=zaq1";

        ////WEBAPI呼叫路徑
        //static string Url = "http://192.168.1.197"; //依據實際IP修改
        //static string Url1 = "/geekplus/api/artemis/pushJson/"; //固定Url 
        //static string Url2 = "?warehouse_code=ESLITE&owner_code=ESLITE"; //依據實際warehouse_code，owner_code 修改       
        //static string methodName = "receiptNoteImport";//API路徑名稱
        ////傳出路徑
        //public static string strURL = Url + Url1 + methodName + Url2;

        ////GEEK+ JSON表頭
        //public static string JsonHead(out string Requestid)
        //{

        //    StringBuilder jsonString = new StringBuilder();
        //    jsonString.Append("\"header\":{");
        //    jsonString.Append("\"warehouse_code\":\"ESLITE\","); //依據實際warehouse_code，owner_code 修改
        //    jsonString.Append("\"user_id\":\"ESLITE\","); //依據實際user_id 修改
        //    jsonString.Append("\"user_key\":\"111111\""); //依據實際user_key 修改

        //    jsonString.Append("},");
        //    Requestid = jsonString.ToString();
        //    return Requestid;
        //}
        #endregion
        static string strinipatch = "D://AGVCLR//AGV_CLR_CONFIG//" + "CLR_AGV_RECEIPT" + ".ini";
        //取得資料DB
        public static string str_dataConn = IniManager.ReadIniData("DB", "str_dataConn ", "", strinipatch);

        //寫入LOGDB
        public static string str_logConn = IniManager.ReadIniData("DB", "str_logConn ", "", strinipatch);

        //WEBAPI呼叫路徑
        public static string StrURL()
        {
            string Url = "http://" + IniManager.ReadIniData("WEBAPIurl", "IP ", "", strinipatch) + ""; //依據實際IP修改
            string Url1 = "/geekplus/api/artemis/pushJson/"; //固定Url 
            string Url2 = "?warehouse_code=" + StoredProcedures.warehouse_code + "&owner_code=" + StoredProcedures.owner_code + ""; //依據實際warehouse_code，owner_code 修改       
            string methodName = "receiptNoteImport"; //API路徑名稱
            //傳出路徑
            return Url + Url1 + methodName + Url2;

        }

        //GEEK+ JSON表頭
        public static string JsonHead(out string Requestid)
        {

            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("\"header\":{");
            jsonString.Append("\"warehouse_code\":\"" + StoredProcedures.warehouse_code + "\","); //依據實際warehouse_code，owner_code 修改
            jsonString.Append("\"user_id\":\"" + IniManager.ReadIniData("WEBAPIurl", "user_id ", "", strinipatch) + "\","); //依據實際user_id 修改
            jsonString.Append("\"user_key\":\"" + IniManager.ReadIniData("WEBAPIurl", "user_key ", "", strinipatch) + "\""); //依據實際user_key 修改

            jsonString.Append("},");
            Requestid = jsonString.ToString();
            return Requestid;
        }

        public class IniManager
        {
            #region API函式宣告
            [DllImport("kernel32")]//返回0表示失敗，非0為成功
            private static extern long WritePrivateProfileString(string section, string key,
              string val, string filePath);
            [DllImport("kernel32")]//返回取得字串緩衝區的長度
            private static extern long GetPrivateProfileString(string section, string key,
              string def, StringBuilder retVal, int size, string filePath);
            #endregion
            #region 讀Ini檔案
            public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
            {
                if (File.Exists(iniFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024);
                    GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                    return temp.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
            #endregion
        }
    }
}
