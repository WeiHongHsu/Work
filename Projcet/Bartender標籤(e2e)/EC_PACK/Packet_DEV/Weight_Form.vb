Imports System.Data.SqlClient

Public Class Weight_Form
    Public Stationid As String
    Public Checker As String
    Public Packer1 As String
    Public Packer2 As String
    Public Order As String
    Public Deliver As String
    Public Origin_Weight As Integer
    Public Package_Weight As Integer
    Dim Wrapping_Weight As Integer
    Dim Package_ratio As String
    Public approved As Boolean



    Private Sub Weight_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        init()
    End Sub

    Sub main()
        Try
            weight_check()
            approved_check()
            set_view()
            InsertData(Stationid, Checker, Packer1, Packer2, Order, Deliver, Origin_Weight, Package_Weight, Wrapping_Weight, Package_ratio)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Sub init()
        Station_Label.Text = ""
        Checker_Label.Text = ""
        Packer1_Label.Text = ""
        Packer2_Label.Text = ""
        Order_Label.Text = ""
        Deliver_Label.Text = ""
        Origin_Weight_Label.Text = ""
        Packer1_Label.Text = ""
        Packer2_Label.Text = ""
        Package_Weight_Label.Text = ""
        Wrapping_Weight_Label.Text = ""
        Package_ratio_Label.Text = ""
        Approved_Label.Text = ""

        Stationid = ""
        Checker = ""
        Packer1 = ""
        Packer2 = ""
        Order = ""
        Deliver = ""
        Origin_Weight = 0
        Package_Weight = 0
        Wrapping_Weight = 0
        Package_ratio = ""
        approved = False
    End Sub
    Sub approved_check()
        If approved = True Then
            Approved_Label.Text = "合格"
            Approved_Label.BackColor = Color.Lime
        Else
            Approved_Label.Text = "不合格"
            Approved_Label.BackColor = Color.Red
        End If
    End Sub
    Sub set_view()
        If String.IsNullOrEmpty(Stationid) = False Then Station_Label.Text = Stationid
        If String.IsNullOrEmpty(Checker) = False Then Checker_Label.Text = Checker
        If String.IsNullOrEmpty(Packer1) = False Then Packer1_Label.Text = Packer1
        If String.IsNullOrEmpty(Packer2) = False Then Packer2_Label.Text = Packer2
        If String.IsNullOrEmpty(Order) = False Then Order_Label.Text = Order
        If String.IsNullOrEmpty(Deliver) = False Then Deliver_Label.Text = Deliver
        If String.IsNullOrEmpty(Origin_Weight.ToString) = False Then Origin_Weight_Label.Text = Origin_Weight
        If String.IsNullOrEmpty(Package_Weight.ToString) = False Then Package_Weight_Label.Text = Package_Weight
        If String.IsNullOrEmpty(Wrapping_Weight.ToString) = False Then Wrapping_Weight_Label.Text = Wrapping_Weight
        If String.IsNullOrEmpty(Package_ratio) = False Then Package_ratio_Label.Text = Package_ratio
    End Sub
    Sub weight_check()

        Dim final_scale As Single
        Dim limit_scale As Single

        If 250 <= Origin_Weight And Origin_Weight < 1000 Then
            Wrapping_Weight = Package_Weight - Origin_Weight
            final_scale = Math.Round(Wrapping_Weight / Package_Weight, 5)
            limit_scale = 0.4
        ElseIf 1000 <= Origin_Weight And Origin_Weight < 3000 Then
            Wrapping_Weight = Package_Weight - Origin_Weight
            final_scale = Math.Round(Wrapping_Weight / Package_Weight, 5)
            limit_scale = 0.3
        ElseIf 3000 <= Origin_Weight Then
            Wrapping_Weight = Package_Weight - Origin_Weight
            final_scale = Math.Round(Wrapping_Weight / Package_Weight, 5)
            limit_scale = 0.15
        ElseIf Origin_Weight <= 250 Then
            final_scale = Math.Round(Wrapping_Weight / Package_Weight, 5)
            limit_scale = 100
        End If

        Debug.Print(final_scale)
        Package_ratio = (final_scale * 100).ToString + "%"
        If final_scale > limit_scale Then
            approved = False
            Debug.Print("不合格")
        Else
            approved = True
            Debug.Print("合格")
        End If

    End Sub

    Public Sub InsertData(stationId As String, checker As String, packer1 As String, packer2 As String, orderNo As String, deliver As String, originWeight As Integer, packageWeight As Integer, wrappingWeight As Integer, packageRatio As String)
        Dim connectionString As String = "Data Source=192.168.1.137\hjwms;Initial Catalog=AAD;User ID=eWMS_Owner;Password=(eUwPWWfz2Gz5drm(lSKwC5L"

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO _t_carton_weight (WMS, StationId, Checker, Packer1, Packer2, OrderNo, Deliver, OriginWeight, PackageWeight, WrappingWeight, PackageRatio, approved) " &
                                  "VALUES (@WMS, @StationId, @Checker, @Packer1, @Packer2, @OrderNo, @Deliver, @OriginWeight, @PackageWeight, @WrappingWeight, @PackageRatio, @approved)"

            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@WMS", "eWMS")
                command.Parameters.AddWithValue("@StationId", stationId)
                command.Parameters.AddWithValue("@Checker", checker)
                command.Parameters.AddWithValue("@Packer1", packer1)
                command.Parameters.AddWithValue("@Packer2", packer2)
                command.Parameters.AddWithValue("@OrderNo", orderNo)
                command.Parameters.AddWithValue("@Deliver", deliver)
                command.Parameters.AddWithValue("@OriginWeight", originWeight)
                command.Parameters.AddWithValue("@PackageWeight", packageWeight)
                command.Parameters.AddWithValue("@WrappingWeight", wrappingWeight)
                command.Parameters.AddWithValue("@PackageRatio", packageRatio)
                command.Parameters.AddWithValue("@approved", approved.ToString)
                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub


End Class