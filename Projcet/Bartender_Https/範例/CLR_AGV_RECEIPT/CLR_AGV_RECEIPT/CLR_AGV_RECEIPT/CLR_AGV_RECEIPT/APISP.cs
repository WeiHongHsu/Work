//------------------------------------------------------------------------------
// <copyright file="CSSqlStoredProcedure.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CLR_AGV_RECEIPT;

public partial class StoredProcedures
{
    public static string warehouse_code { get; set; }
    public static string owner_code { get; set; }

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void APISP ()
    {

    }

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void CLR_AGV_RECEIPT(SqlString _Mod, SqlString _warehouse_code, SqlString _Storekey, SqlString _KEY, out SqlInt32 value)
    {
        // 
        value = 0;
        string Mod = _Mod.ToString();
        string KEY = _KEY.ToString();
        string dstype = "";
        string tb_JSON = "";
        warehouse_code = _warehouse_code.ToString();
        owner_code = _Storekey.ToString();

        DataSet ds = new DataSet();
        try
        {
            ds =  Context.AGV_Put_View(Mod, owner_code, KEY, ref dstype);

            //dstype 判斷SP是否有正常執行
            if (dstype == "ERROR")
            {
                //寫入LOG   錯誤或成功都會寫 並且在SQL CLIENT也顯示insert 內容
                SqlContext.Pipe.Send(Context.ExecSqlLog("CLR_AGV_RECEIPT", KEY, "400", "", "SP", ds.Tables[0].Rows[0][0].ToString(), "", tb_JSON));
            }
            else
            {
                
                SqlContext.Pipe.Send("1");
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "receipt_list";
                    ds.Tables[1].TableName = "sku_list";
                    string Requestid = "";
                    string head = config.JsonHead(out Requestid);
                    string body = "\"body\":{" + "" + "\"receipt_amount\":" + ds.Tables[0].Rows.Count + "," + "\"receipt_list\":[{";
                    SqlContext.Pipe.Send("2");
                    DataTable dt = ds.Tables["receipt_list"];
                    //當有需要組合下一層LIST的時候要加上,
                    body = body + Libs.Data_Json(dt) + ",";

                    body = body + "\"sku_list\":[{";
                    dt = ds.Tables["sku_list"]; 
                    body = body + Libs.Data_Json(dt) + "}]";

                    //sku_list的結尾
                    body = body + "}]";
                    //body的結尾
                    body = body + "}";

                    string Json = "{" + head + body + "}";
                    
                    tb_JSON = Json;
                    SqlContext.Pipe.Send("3");
                    //SqlContext.Pipe.Send(Json.ToString());
                    SqlContext.Pipe.Send("3.1");
                    //SqlContext.Pipe.Send(tb_JSON);
                    SqlContext.Pipe.Send("3.2");
                    string tb_Result = Libs.Post(Json);
                    SqlContext.Pipe.Send("3.3");
                    // show 结果
                    SqlContext.Pipe.Send("4");
                    //SqlContext.Pipe.Send(tb_Result);

                    // 回傳
                    JObject jo = (JObject)JsonConvert.DeserializeObject(tb_Result);
                    string hmsgCode = jo["header"]["msgCode"].ToString();
                    string hmessage = jo["header"]["message"].ToString();

                    string errcode = "";
                    string errmessage = "";

                    //SQL介面顯示錯誤訊息
                    if (hmsgCode == "400")
                    {
                        //SqlContext.Pipe.Send(jo.ToString());
                        value = -1;
                    }
                    // TODO AGV回傳錯誤訊息是動態的,還沒研究怎麼解析,所以錯誤JSON全部塞到tb_Result
                    //SqlContext.Pipe.Send(hmsgCode);
                    //SqlContext.Pipe.Send(hmessage);
                    //SqlContext.Pipe.Send(errcode);
                    //SqlContext.Pipe.Send(errmessage);
                    //更新log
                    //sql
                    if (hmsgCode == "200")
                    {
                        value = 1;
                    }

                    //寫入LOG   錯誤或成功都會寫 並且在SQL CLIENT也顯示insert 內容
                    //SqlContext.Pipe.Send(Context.ExecSqlLog("AGV_RECEIPT", POKEY, hmsgCode, hmessage, errcode, errmessage, tb_Result));
                    Context.ExecSqlLog("CLR_AGV_RECEIPT", KEY, hmsgCode, hmessage, errcode, errmessage, tb_Result, tb_JSON);

                }
                else
                {
                    SqlContext.Pipe.Send(Context.ExecSqlLog("CLR_AGV_RECEIPT", KEY, "400", "", "SP", "沒有資料", "", tb_JSON));
                    value = -1;
                }
            }

            
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send("EX");
            SqlContext.Pipe.Send("KEY:"+ KEY);
            SqlContext.Pipe.Send(ex.Message);
            //寫入LOG   錯誤或成功都會寫 並且在SQL CLIENT也顯示insert 內容
            //SqlContext.Pipe.Send(Context.ExecSqlLog("CLR_AGV_RECEIPT", POKEY, "400", "", "CLR", ex.Message, "", tb_JSON));
            Context.ExecSqlLog("CLR_AGV_RECEIPT", KEY, "400", "", "CLR", ex.Message, "", tb_JSON);
            value = -1;

            //更新有問題的資料狀態為99          
            ds = Context.AGV_Err_roldback(Mod, owner_code, KEY);
        }
        finally
        {
            ds.Dispose();
        }
    }

     

}
     
