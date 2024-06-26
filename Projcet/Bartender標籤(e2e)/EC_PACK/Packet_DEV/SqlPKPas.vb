Imports System.Data.SqlClient

Public Class SqlPKPas
    'Data Source=127.0.0.1;Initial Catalog=TFPK;Persist Security Info=True;User ID=sa;Password=zaq1
    Public Shared TPKconstr As String = "Data Source=192.168.1.11;Initial Catalog=PROD;Persist Security Info=True;User ID=sa;Password=zaq1"

    Public Shared Function Getkey(ByVal KeyName As String, ByVal Wkey As String) As String
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim cmd As SqlClient.SqlCommand
        Dim sqls As String = ""
        Dim fgStr As String = ""
        ' GetKey @KeyName   , @UserID  , @Returnkey
        Try
            con.Open()
            cmd = New SqlClient.SqlCommand("Prod..Getkey", con)
            cmd.CommandTimeout = 1800
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.Add("@KeyName", SqlDbType.VarChar, 30).Value = KeyName
            cmd.Parameters.Add("@UserID ", SqlDbType.VarChar, 30).Value = Wkey
            cmd.Parameters.Add("@Returnkey", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output
            cmd.ExecuteNonQuery()
            Getkey = cmd.Parameters("@Returnkey").Value.ToString

        Catch ex As Exception
            Getkey = "DB function::Getkey Return :" & ex.Message

        Finally
        End Try
    End Function

    ' 共同StoredProcedure  VarChar:20,20,500 ;Input,Input,Output
    Public Shared Function PKSqlLname(ByVal PrcName As String, ByVal LsValue As String) As Object
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim OUT As String = ""
        Try
            con.Open()
            Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand(PrcName, con)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 1800
            Dim Ls() As String = LsValue.Split(",")
            For i As Integer = 0 To Ls.Length - 1
                If Ls(i).IndexOf("=") > 0 Then
                    Dim LsVal() As String = Ls(i).Split("=")
                    cmd.Parameters.Add(LsVal(0), SqlDbType.VarChar, 20).Value = LsVal(1)
                    cmd.Parameters(i).Direction = ParameterDirection.Input
                End If
                If Ls(i).IndexOf("OUT") > 0 Then
                    Ls(i).Replace(" OUT", "")
                    cmd.Parameters.Add(Ls(i).Replace(" OUT", ""), SqlDbType.VarChar, 500)
                    cmd.Parameters(i).Direction = ParameterDirection.Output
                    OUT = cmd.Parameters(i).ParameterName
                End If
            Next
            'Debug.Print(cmd.CommandText & " " & cmd.Parameters(0).SqlValue & cmd.Parameters(2).SqlValue & cmd.Parameters(2).Value.ToString)
            cmd.ExecuteNonQuery()
            PKSqlLname = cmd.Parameters(OUT).SqlValue.ToString

            con.Close()
        Catch ex As Exception

            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
            PKSqlLname = "ERR：APP " & Err.Number & "错误描述：" & Err.Description
        End Try
    End Function
    'GetCountRd
    ''' <summary>
    ''' 返回查詢后的 筆輸
    ''' </summary>
    ''' <param name="sqlstr">sql語句</param>
    ''' <returns></returns>
    Public Shared Function GetCountRd(ByVal sqlstr As String) As Long
        GetCountRd = Nothing
        Dim Sconn As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim Scmd As SqlClient.SqlCommand = New SqlClient.SqlCommand(sqlstr, Sconn)
        Scmd.CommandTimeout = 1800
        Try
            If Sconn.State = ConnectionState.Closed Then Sconn.Open()
            Dim LsDr As SqlClient.SqlDataReader
            LsDr = Scmd.ExecuteReader()
            If LsDr.Read() = True Then
                GetCountRd = LsDr.GetInt64(0).ToString()
            Else
                GetCountRd = 0
            End If
            Sconn.Close()
            LsDr.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Function

    ''' <summary>
    '''回傳一兩列陣列( Row2:  filed ,Values)
    ''' </summary>
    ''' <param name="selectCmd">sql語句</param>
    ''' <returns></returns>

    Public Shared Function ReadDBInt(ByVal selectCmd As String) As String(,)
        ReadDBInt = Nothing
        Dim Sconn As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim Scmd As SqlClient.SqlCommand = New SqlClient.SqlCommand(selectCmd, Sconn)
        Dim ALstr(-1, 2) As String
        Try
            If Sconn.State = ConnectionState.Closed Then Sconn.Open()
            Dim LsDr As SqlClient.SqlDataReader
            LsDr = Scmd.ExecuteReader()
            If LsDr.Read() = True Then
                ReDim ALstr(LsDr.FieldCount - 1, 1)
                For i As Integer = 0 To LsDr.FieldCount - 1
                    ALstr(i, 0) = LsDr.GetName(i)
                    ALstr(i, 1) = LsDr.GetString(i).ToString
                Next
            End If
            ReadDBInt = ALstr
            Sconn.Close()
            LsDr.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Function

    Public Shared Function SqlSelectData(ByVal SQLstr As String) As DataSet
        Dim Ds As New DataSet

        Using con As New SqlClient.SqlConnection(TPKconstr)
            Using cmd As New SqlCommand(SQLstr)
                cmd.Connection = con
                cmd.CommandTimeout = 18000
                Try
                    Using Adapter As New SqlDataAdapter(cmd)
                        Adapter.Fill(Ds)
                        con.Dispose()
                    End Using
                Catch ex As DataException
                    MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)

                End Try
            End Using
        End Using
        Return Ds
    End Function

    ''' <summary>
    ''' 返回查詢sql中帶參數的datareader
    ''' </summary>
    ''' <param name="sqlstr">sql語句</param>
    ''' <param name="pra">參數</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function sqlsel(ByVal sqlstr As String, ByVal pra() As SqlClient.SqlParameter) As SqlClient.SqlDataReader
        Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand()
        Dim reader As SqlClient.SqlDataReader = Nothing
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        cmd.CommandTimeout = 1800
        Try
            If Not (pra Is Nothing) Then
                PrepareCommand(cmd, con, Nothing, sqlstr, pra)
            Else
                PrepareCommand(cmd, con, Nothing, sqlstr, Nothing)
            End If
            reader = cmd.ExecuteReader()
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
            Exit Try
        End Try
        Return reader
    End Function
    ''' <summary>
    ''' 返回查詢sql中不帶參數的datareader
    ''' </summary>
    ''' <param name="sqlstr">sql語句</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function sqlsel(ByVal sqlstr As String) As SqlClient.SqlDataReader
        Return sqlsel(sqlstr, Nothing)
    End Function

    Public Shared Function SqlSelectData(ByVal sqlstr As String, ByVal pra() As SqlClient.SqlParameter) As DataSet
        Dim objdataset As New DataSet()
        Dim Adapter As SqlClient.SqlDataAdapter
        Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand()
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        cmd.CommandTimeout = 1800
        Try
            PrepareCommand(cmd, con, Nothing, sqlstr, pra)
            Adapter = New SqlClient.SqlDataAdapter(cmd)
            objdataset = New DataSet
            Adapter.Fill(objdataset)
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
            Exit Try
        End Try
        Return objdataset
    End Function


    Public Shared Function sqlTable(ByVal sqlstr As String) As DataTable
        Dim boxlab As New DataTable()

        Try
            Using conn As New System.Data.SqlClient.SqlConnection(TPKconstr)
                'conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand(sqlstr, conn)
                    Using Adapter = New SqlClient.SqlDataAdapter(cmd)
                        Adapter.Fill(boxlab)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
            'End
            Exit Try
        End Try
        Return boxlab
    End Function


    Public Shared Function ExcuteSql(ByVal sqlstr As String) As Boolean
        Dim objoraclecon As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim objcmd As SqlClient.SqlCommand

        Try
            objoraclecon.Open()
            objcmd = New SqlClient.SqlCommand(sqlstr, objoraclecon)
            objcmd.CommandTimeout = 1800
            objcmd.ExecuteNonQuery()

            objoraclecon.Close()
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description & Chr(10) & sqlstr)
            Return False
        End Try
        Return True
    End Function

    Public Shared Function SqlQueryVal(ByVal sqlstr As String) As String
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
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

    Public Shared Function ExcuteSqlRows(ByVal sqlstr As String) As Boolean
        Dim objoraclecon As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim objcmd As SqlClient.SqlCommand
        Dim r As Integer

        Try
            objoraclecon.Open()
            objcmd = New SqlClient.SqlCommand(sqlstr, objoraclecon)
            objcmd.CommandTimeout = 1800
            r = objcmd.ExecuteNonQuery()
            If r = 0 Then
                ExcuteSqlRows = False
            Else
                ExcuteSqlRows = True
            End If

            objoraclecon.Close()
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description & Chr(10) & sqlstr)
            ExcuteSqlRows = False
        End Try

    End Function

    Public Shared Sub PrepareCommand(ByVal cmd As SqlClient.SqlCommand, ByVal conn As SqlClient.SqlConnection, ByVal tran As SqlClient.SqlTransaction, ByVal cmdtext As String, ByVal pra() As SqlClient.SqlParameter)
        If (conn.State <> ConnectionState.Open) Then
            conn.Open()
        End If
        cmd.Connection = conn
        cmd.CommandText = cmdtext
        If Not (tran Is Nothing) Then
            cmd.Transaction = tran
        End If
        cmd.CommandType = CommandType.Text
        If Not (pra Is Nothing) Then
            Dim orapara As SqlClient.SqlParameter
            For Each orapara In pra
                cmd.Parameters.Add(orapara)
            Next
        End If
    End Sub
    ''' <summary>
    ''' 執行帶參數的insert,update,delete操作
    ''' </summary>
    ''' <param name="sqlstr">sql語句</param>
    ''' <param name="pra">參數</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExcuteSql(ByVal sqlstr As String, ByVal pra() As SqlClient.SqlParameter) As Boolean
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand()
        Try
            cmd.CommandTimeout = 1800

            PrepareCommand(cmd, con, Nothing, sqlstr, pra)
            cmd.ExecuteNonQuery()
            con.Close()
        Catch ex As Exception
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
            Return False
            'End
        End Try
        Return True
    End Function

    Public Shared Function ExcuteSql(ByVal sqlstr() As String) As Boolean
        Return ExcuteSql(sqlstr, Nothing)
    End Function
    ''' <summary>
    ''' 執行帶參數的批量的sql語句并且進行事務處理
    ''' </summary>
    ''' <param name="sqlstr">sql語句</param>
    ''' <param name="pra">參數</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExcuteSql(ByVal sqlstr() As String, ByVal pra()() As SqlClient.SqlParameter) As Boolean
        Dim sql As String
        Dim count As Integer = 0
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(TPKconstr)
        con.Open()
        Dim tran As SqlClient.SqlTransaction = con.BeginTransaction()
        Dim cmd As SqlClient.SqlCommand
        Try
            For Each sql In sqlstr
                cmd = New SqlClient.SqlCommand()
                cmd.CommandTimeout = 1800
                If (pra Is Nothing) Then
                    PrepareCommand(cmd, con, tran, sql, Nothing)
                Else
                    PrepareCommand(cmd, con, tran, sql, pra(count))
                End If
                cmd.ExecuteNonQuery()
                count = count + 1
            Next
            tran.Commit()
            Return True
        Catch ex As Exception
            tran.Rollback()
            MsgBox("错误号：" & Err.Number & "错误描述：" & Err.Description)
            Return False
            'End
        Finally
            con.Dispose()
        End Try
    End Function



    Public Shared Function exists(ByVal sqlstr As String) As Boolean
        Dim data As New Object()
        Dim con As New SqlClient.SqlConnection(TPKconstr)
        Dim cmd As New SqlClient.SqlCommand()
        Try
            PrepareCommand(cmd, con, Nothing, sqlstr, Nothing)
            data = cmd.ExecuteScalar()
            con.Close()
        Catch ex As Exception
            'MsgBox("错误号：" & ex.Message)
            'Return Nothing
            data = Nothing
            Exit Try
        End Try

        If (data Is Nothing) Then
            Return False
        Else
            Return True
        End If
    End Function




End Class
