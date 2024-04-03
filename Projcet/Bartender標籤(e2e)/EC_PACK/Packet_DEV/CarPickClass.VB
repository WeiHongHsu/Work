Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Data.SqlClient


Namespace CarPickClass

    Public Class CarPickClass
        Public CarNo As String
        Public CarData As DataTable = New DataTable
        Public dicAGVBOX As Dictionary(Of String, String) = setdic()
        Public dicPOKEY As Dictionary(Of String, String) = setdic()
        Public status As String

        Public Sub addCarNo(ByVal CarNo As String)
            CarNo = checkCarNO(CarNo)
        End Sub

        Public Sub addAGVBOX(ByVal CarLoc As String, ByVal agvbox As String)
            If agvbox.Trim.ToString = "" Then
                dicAGVBOX(CarLoc) = ""
                dicPOKEY(CarLoc) = ""
            End If
            Dim pokey As String = checkAGVBOX(agvbox)
            If pokey = "69" Then Exit Sub
            dicAGVBOX(CarLoc) = agvbox
            dicPOKEY(CarLoc) = pokey
        End Sub

        Public Sub getCarData(ByVal str As String)
            CarData = getCarNoData(str)
            filldic()
        End Sub

        Public Sub filldic()
            If CarData.Rows.Count > 0 Then
                For i As Integer = 0 To CarData.Rows.Count - 1
                    dicPOKEY(CarData.Rows(i).Item("Carpkid").trim.ToString) = CarData.Rows(i).Item("sokey").trim.ToString
                Next
            End If
        End Sub

        Public Sub uploadCarData()
            Try
                Dim constr As String = "Data Source=192.168.1.11;Initial Catalog=Prod;User ID=eWMS_Owner;Password=(eUwPWWfz2Gz5drm(lSKwC5L"
                Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(constr)
                Dim cmd As SqlClient.SqlCommand = con.CreateCommand
                con.Open()
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandText = "AGV_Car_pick"
                '呼叫預存程序參數
                cmd.Parameters.Add(New SqlParameter("@CarNo", CarNo))
                cmd.Parameters.Add(New SqlParameter("@A", dicPOKEY("A")))
                cmd.Parameters.Add(New SqlParameter("@B", dicPOKEY("B")))
                cmd.Parameters.Add(New SqlParameter("@C", dicPOKEY("C")))
                cmd.Parameters.Add(New SqlParameter("@D", dicPOKEY("D")))
                cmd.Parameters.Add(New SqlParameter("@E", dicPOKEY("E")))
                cmd.Parameters.Add(New SqlParameter("@F", dicPOKEY("F")))
                cmd.Parameters.Add(New SqlParameter("@G", dicPOKEY("G")))
                cmd.Parameters.Add(New SqlParameter("@H", dicPOKEY("H")))
                cmd.Parameters.Add(New SqlParameter("@I", dicPOKEY("I")))
                cmd.Parameters.Add(New SqlParameter("@J", dicPOKEY("J")))
                cmd.Parameters.Add(New SqlParameter("@K", dicPOKEY("K")))
                cmd.Parameters.Add(New SqlParameter("@L", dicPOKEY("L")))
                '宣告預存程序回傳值
                cmd.Parameters.Add(New SqlParameter("@MSG", Data.SqlDbType.NVarChar, 2))
                cmd.Parameters("@MSG").Direction = Data.ParameterDirection.Output
                cmd.ExecuteNonQuery()

                Dim str As String = cmd.Parameters("@MSG").Value
                Select Case str
                    Case "00"
                        MsgBox("資料異常請洽資訊人員")
                    Case "69"
                        MsgBox("該車次已存在")
                    Case "79"
                        MsgBox("資料上傳異常已刪除,請重新封車")
                    Case "99"
                        MsgBox("資料上傳成功")
                End Select
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        Function setdic()
            Dim dicdefu As Dictionary(Of String, String) = New Dictionary(Of String, String)
            dicdefu.Add("A", "")
            dicdefu.Add("B", "")
            dicdefu.Add("C", "")
            dicdefu.Add("D", "")
            dicdefu.Add("E", "")
            dicdefu.Add("F", "")
            dicdefu.Add("G", "")
            dicdefu.Add("H", "")
            dicdefu.Add("I", "")
            dicdefu.Add("J", "")
            dicdefu.Add("K", "")
            dicdefu.Add("L", "")
            Return dicdefu
        End Function

        Function getPoByCarLoc(ByVal CarLoc As String)
            Dim str As String = CarLoc
            If dicPOKEY(CarLoc) <> "" Then str = CarLoc + " : " + dicPOKEY(CarLoc)
            If dicAGVBOX(CarLoc) <> "" Then str = CarLoc + " : " + dicAGVBOX(CarLoc)
            Return str
        End Function

        Function getCarNoData(ByVal CarNo As String) As DataTable
            Dim sqlstr As String = "select * from FWK..AGV_PickCarList where CarNo = '" + CarNo + "'"
            Dim dt As DataTable = New DataTable
            dt = Sqlselect(sqlstr)
            Return dt
        End Function

        Function checkCarNO(ByVal CarNo As String)

            Dim sqlstr As String = "select count(*) from FWK..AGV_PickCarList where CarNo = '" + CarNo + "'"
            Dim dt As DataTable = New DataTable
            dt = Sqlselect(sqlstr)
            status = dt.Rows(0).Item(0).ToString
            Select Case status
                Case "0"
                    CarNo = CarNo
                    status = "0"
                Case Else
                    status = "69"
            End Select
            Return CarNo
        End Function

        Function checkAGVBOX(ByVal agvbox As String)
            Dim pokey As String = "69"
            Dim sqlstr As String = "select ISNULL(COUNT(distinct EXTERNORDERKEY),0) from PROD..ORDERS where AgvBox = '" + agvbox + "'"
            Dim dt As DataTable = New DataTable
            dt = Sqlselect(sqlstr)
            Dim status As String = dt.Rows(0).Item(0).ToString
            Select Case status
                Case "0"
                    MsgBox("AGV密盆: " + agvbox + " 無對應單號")
                Case "1"
                    sqlstr = "select distinct EXTERNORDERKEY from PROD..ORDERS where AgvBox = '" + agvbox + "'"
                    dt = Sqlselect(sqlstr)
                    pokey = dt.Rows(0).Item(0).ToString
                Case Else
                    MsgBox("AGV密盆: " + agvbox + " 對應多筆單號")
            End Select

            Return pokey
        End Function

        Function Sqlselect(ByVal sqlstr As String)
            Dim Adapter As SqlClient.SqlDataAdapter
            Dim constr As String = "Data Source=192.168.1.11;Initial Catalog=PROD;User ID=eWMS_Owner;Password=(eUwPWWfz2Gz5drm(lSKwC5L"
            Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(constr)
            Dim dt As DataTable = New DataTable
            dt.Clear()
            If con.State <> ConnectionState.Open Then con.Open()
            Try
                Adapter = New SqlClient.SqlDataAdapter(sqlstr, con)
                Adapter.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            Return dt
        End Function

    End Class

End Namespace
