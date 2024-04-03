Imports System.Data.SqlClient

Public Class NEDISqlH
    Public Shared NEDIconstr As String = "Data Source=192.168.1.52;Initial Catalog=SJOB;Persist Security Info=True;User ID=EC_PACK_Executor;Password=uF8tEFy7xRzS9UkP"

    'EXEC [NEDI].SJOB.dbo.EDIQry_ApiBuildStrSTD 'tracking' , @UserID,'G016' , ''
    'EXEC [NEDI].SJOB.dbo.EDIApiBus_DFlow 'tracking' , @UserID ,'', @GetVal out
    Public Shared Function SqlApi_Shoppe(ByVal SPName As String, ByVal Mods As String, ByVal Wkey As String, ByVal Extkeys As String) As String
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(NEDIconstr)
        Dim cmd As SqlClient.SqlCommand
        Dim sqls As String = ""
        Dim fgStr As String = ""

        Try

            con.Open()
            cmd = New SqlClient.SqlCommand(SPName, con)
            cmd.CommandTimeout = 1800
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.Add("@Infmod", SqlDbType.VarChar, 30).Value = Mods
            cmd.Parameters.Add("@Wkey", SqlDbType.VarChar, 30).Value = Wkey
            cmd.Parameters.Add("@Extkeys", SqlDbType.VarChar, 50).Value = Extkeys
            cmd.Parameters.Add("@USID", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            SqlApi_Shoppe = cmd.Parameters("@USID").Value.ToString

        Catch ex As Exception
            SqlApi_Shoppe = "DB function::" & SPName & "Return :" & ex.Message

        Finally
        End Try
    End Function

    Public Shared Function ExcuteSql(ByVal sqlstr As String) As Boolean
        Dim objoraclecon As SqlClient.SqlConnection = New SqlClient.SqlConnection(NEDIconstr)
        Dim objcmd As SqlClient.SqlCommand

        Try
            objoraclecon.Open()
            objcmd = New SqlClient.SqlCommand(sqlstr, objoraclecon)
            objcmd.CommandTimeout = 1800
            objcmd.ExecuteNonQuery()
            objcmd.ExecuteNonQuery()
            objoraclecon.Close()
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description & Chr(10) & sqlstr)
            Return False
        End Try
        Return True
    End Function

    Public Shared Function SqlQueryVal(ByVal sqlstr As String) As String
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(NEDIconstr)
        Try
            con.Open()
            Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand(sqlstr, con)
            Dim Rt As Object = cmd.ExecuteScalar()
            If Rt Is Nothing Then
                SqlQueryVal = ""
            Else
                SqlQueryVal = Rt.ToString
            End If
            con.Close()
        Catch ex As Exception
            SqlQueryVal = -9
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
        End Try
    End Function



End Class