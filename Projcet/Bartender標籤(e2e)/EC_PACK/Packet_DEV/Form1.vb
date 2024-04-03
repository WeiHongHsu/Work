Public Class Form1

    Public f1 As Packet
    Public msg, route, complete, packmod As String

    Private Sub Cb_packmod_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cb_packmod.SelectedIndexChanged
        packmod = CStr(Cb_packmod.SelectedIndex + 1)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles w_btn.Click

        If (Txt_Weight.Text = "" Or packmod = "") Then
            MsgBox("請輸入正確資訊。")
            Return
        End If

        f1.Combination_CaseDetailTableAdapter.UpdateWeightQuery((CDbl(Txt_Weight.Text) + 0.5).ToString(), packmod, wpokey_Lb.Text, wcaseID_Lb.Text)

        If (route = "Z3" And complete = "Y") Then
            Packet.TotalWeight = Txt_Weight.Text
            Dim f2 As New Form2
        End If

        If (route = "Z9") Then
            f1.allprint()
        End If

        Me.Close()

    End Sub
End Class