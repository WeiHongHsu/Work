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
        //20200811�令����INI��Ū��
        #region �g���bCLR���| MARK CODE
        ////���o���DB
        //public static string str_dataConn = "Data Source=192.168.1.11;Initial Catalog=PROD;Persist Security Info=True;User ID=sa;Password=zaq1";
        ////�g�JLOGDB
        //public static string str_logConn = "Data Source=192.168.1.11;Initial Catalog=PROD;Persist Security Info=True;User ID=sa;Password=zaq1";

        ////WEBAPI�I�s���|
        //static string Url = "http://192.168.1.197"; //�̾ڹ��IP�ק�
        //static string Url1 = "/geekplus/api/artemis/pushJson/"; //�T�wUrl 
        //static string Url2 = "?warehouse_code=ESLITE&owner_code=ESLITE"; //�̾ڹ��warehouse_code�Aowner_code �ק�       
        //static string methodName = "receiptNoteImport";//API���|�W��
        ////�ǥX���|
        //public static string strURL = Url + Url1 + methodName + Url2;

        ////GEEK+ JSON���Y
        //public static string JsonHead(out string Requestid)
        //{

        //    StringBuilder jsonString = new StringBuilder();
        //    jsonString.Append("\"header\":{");
        //    jsonString.Append("\"warehouse_code\":\"ESLITE\","); //�̾ڹ��warehouse_code�Aowner_code �ק�
        //    jsonString.Append("\"user_id\":\"ESLITE\","); //�̾ڹ��user_id �ק�
        //    jsonString.Append("\"user_key\":\"111111\""); //�̾ڹ��user_key �ק�

        //    jsonString.Append("},");
        //    Requestid = jsonString.ToString();
        //    return Requestid;
        //}
        #endregion
        static string strinipatch = "D://AGVCLR//AGV_CLR_CONFIG//" + "CLR_AGV_RECEIPT" + ".ini";
        //���o���DB
        public static string str_dataConn = IniManager.ReadIniData("DB", "str_dataConn ", "", strinipatch);

        //�g�JLOGDB
        public static string str_logConn = IniManager.ReadIniData("DB", "str_logConn ", "", strinipatch);

        //WEBAPI�I�s���|
        public static string StrURL()
        {
            string Url = "http://" + IniManager.ReadIniData("WEBAPIurl", "IP ", "", strinipatch) + ""; //�̾ڹ��IP�ק�
            string Url1 = "/geekplus/api/artemis/pushJson/"; //�T�wUrl 
            string Url2 = "?warehouse_code=" + StoredProcedures.warehouse_code + "&owner_code=" + StoredProcedures.owner_code + ""; //�̾ڹ��warehouse_code�Aowner_code �ק�       
            string methodName = "receiptNoteImport"; //API���|�W��
            //�ǥX���|
            return Url + Url1 + methodName + Url2;

        }

        //GEEK+ JSON���Y
        public static string JsonHead(out string Requestid)
        {

            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("\"header\":{");
            jsonString.Append("\"warehouse_code\":\"" + StoredProcedures.warehouse_code + "\","); //�̾ڹ��warehouse_code�Aowner_code �ק�
            jsonString.Append("\"user_id\":\"" + IniManager.ReadIniData("WEBAPIurl", "user_id ", "", strinipatch) + "\","); //�̾ڹ��user_id �ק�
            jsonString.Append("\"user_key\":\"" + IniManager.ReadIniData("WEBAPIurl", "user_key ", "", strinipatch) + "\""); //�̾ڹ��user_key �ק�

            jsonString.Append("},");
            Requestid = jsonString.ToString();
            return Requestid;
        }

        public class IniManager
        {
            #region API�禡�ŧi
            [DllImport("kernel32")]//��^0��ܥ��ѡA�D0�����\
            private static extern long WritePrivateProfileString(string section, string key,
              string val, string filePath);
            [DllImport("kernel32")]//��^���o�r��w�İϪ�����
            private static extern long GetPrivateProfileString(string section, string key,
              string def, StringBuilder retVal, int size, string filePath);
            #endregion
            #region ŪIni�ɮ�
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
