using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CLR_AGV_RECEIPT
{
    class Context
    {
        //抓取中間庫TEMP TABLE 內容
        public static DataSet AGV_Put_View(String Mod, String owner_code, String KEY, ref string dstype)
        {
            SqlConnection connection = new SqlConnection(config.str_dataConn);
            DataSet ds = new DataSet();
            try
            {
                connection.Open();

                String version = connection.ServerVersion;

                SqlCommand command = new SqlCommand("AGV_Put_View", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameterMod = new SqlParameter("@Mod", Mod);
                command.Parameters.Add(parameterMod);

                SqlParameter parameterStorekey = new SqlParameter("@owner_code", owner_code);
                command.Parameters.Add(parameterStorekey);

                SqlParameter parametersku = new SqlParameter("@sku", "");
                command.Parameters.Add(parametersku);

                SqlParameter parameterkey1 = new SqlParameter("@key1", KEY);
                command.Parameters.Add(parameterkey1);

                SqlParameter parameterkey2 = new SqlParameter("@key2", "");
                command.Parameters.Add(parameterkey2);

                SqlParameter parameterkey3 = new SqlParameter("@key3", "");
                command.Parameters.Add(parameterkey3);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                adapter.Fill(ds);

            }
            catch (SqlException ex)
            {
                dstype = "ERROR";
                ds.Merge(Libs.GetErrorTable(ex.Message));
            }
            catch (Exception ex)
            {
                dstype = "ERROR";
                ds.Merge(Libs.GetErrorTable(ex.Message));
            }
            finally
            {
                if (connection != null)
                { connection.Close(); }
            }
            return ds;
        }

        //如果街口有問題,將資料狀態更新成99
        public static DataSet AGV_Err_roldback(String Mod, String owner_code, String KEY)
        {
            SqlConnection connection = new SqlConnection(config.str_dataConn);
            DataSet ds = new DataSet();
            try
            {
                connection.Open();

                String version = connection.ServerVersion;

                SqlCommand command = new SqlCommand("AGV_Err_roldback", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameterMod = new SqlParameter("@Mod", Mod);
                command.Parameters.Add(parameterMod);

                SqlParameter parameterStorekey = new SqlParameter("@owner_code", owner_code);
                command.Parameters.Add(parameterStorekey);

                SqlParameter parametersku = new SqlParameter("@sku", "");
                command.Parameters.Add(parametersku);

                SqlParameter parameterkey1 = new SqlParameter("@key1", KEY);
                command.Parameters.Add(parameterkey1);

                SqlParameter parameterkey2 = new SqlParameter("@key2", "");
                command.Parameters.Add(parameterkey2);

                SqlParameter parameterkey3 = new SqlParameter("@key3", "");
                command.Parameters.Add(parameterkey3);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                adapter.Fill(ds);

            }
            catch (SqlException ex)
            {
                ds.Merge(Libs.GetErrorTable(ex.Message));
            }
            catch (Exception ex)
            {
                ds.Merge(Libs.GetErrorTable(ex.Message));
            }
            finally
            {
                if (connection != null)
                { connection.Close(); }
            }
            return ds;
        }

        public static string ExecSqlLog(string api, string strvalue, string hmsgCode, string hmessage, string errcode, string errmessage, string tb_Result, string jsonstring)
        {
            //寫入LOG   錯誤或成功都會寫
            using (SqlConnection connection = new SqlConnection(config.str_logConn))
            {
                connection.Open();
                string sqlstr = "insert into CLR_API_LOG";
                sqlstr += " (name,Code1,mail,adddate,resultcode,resultmsg,errorcode,errormsg,bodyresultmsg,jsonstring)";
                sqlstr += " values (";
                sqlstr += "N'" + api + "',";
                sqlstr += "N'" + strvalue + "',";
                sqlstr += "0,";
                sqlstr += "getdate(),";
                sqlstr += "N'" + hmsgCode + "',";
                sqlstr += "N'" + hmessage + "',";
                sqlstr += "N'" + errcode + "',";
                sqlstr += "@errmessage,";
                sqlstr += "@tb_Result,";
                sqlstr += "@jsonstring)";
                using (SqlCommand cmdlog = new SqlCommand(sqlstr, connection))
                {
                    cmdlog.Parameters.Add("@errmessage", SqlDbType.NVarChar);
                    cmdlog.Parameters["@errmessage"].Value = errmessage;

                    cmdlog.Parameters.Add("@tb_Result", SqlDbType.NVarChar);
                    cmdlog.Parameters["@tb_Result"].Value = tb_Result;

                    cmdlog.Parameters.Add("@jsonstring", SqlDbType.NVarChar);
                    cmdlog.Parameters["@jsonstring"].Value = jsonstring;

                    cmdlog.ExecuteNonQuery();
                }

                return sqlstr;

            }
        }

    }
}
