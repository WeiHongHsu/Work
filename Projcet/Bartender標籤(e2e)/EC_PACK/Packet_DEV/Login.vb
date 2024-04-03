Public Class Login
    Dim packer1_bool As Boolean = False
    Dim packer2_bool As Boolean = False
    Dim packer3_bool As Boolean = False

    Private Sub Login_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Function GetName(ByVal BADGENUMBER As String)
        Dim str As String = "FAIL"
        If BADGENUMBER.Length < 2 Then Return str
        Dim CONSTR As String = "Data Source=192.168.1.81;Initial Catalog=A2000;User ID=sa;Password=zaq1"
        Dim SQLSTR As String = "SELECT NAME FROM [A2000].[dbo].[USERINFO] With (nolock) where BADGENUMBER = '" + BADGENUMBER + "'"
        Dim DT As DataTable = New DataTable
        Dim ADAPTER As SqlClient.SqlDataAdapter
        Dim CON As SqlClient.SqlConnection = New SqlClient.SqlConnection(CONSTR)
        Dim CMD As SqlClient.SqlCommand = New SqlClient.SqlCommand(SQLSTR, CON)
        ADAPTER = New SqlClient.SqlDataAdapter(CMD)
        DT = New DataTable
        ADAPTER.Fill(DT)
        If DT.Rows.Count > 0 Then
            str = DT.Rows(0).Item(0).ToString
        End If
        Return str
    End Function

    Private Sub TextBox1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode <> Keys.Enter Then Exit Sub
        Dim BADGENUMBER As String = TextBox1.Text.Trim
        Dim USERNAME As String = GetName(BADGENUMBER)
        If USERNAME = "FAIL" Then
            Packer1.Text = "FAIL"
            Panel1.BackColor = Color.Red
            packer1_bool = False
        Else
            Packer1.Text = USERNAME
            Panel1.BackColor = Color.Green
            TextBox1.Enabled = False
            packer1_bool = True
        End If
    End Sub

    Private Sub TextBox2_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode <> Keys.Enter Then Exit Sub
        Dim BADGENUMBER As String = TextBox2.Text.Trim
        Dim USERNAME As String = GetName(BADGENUMBER)
        If USERNAME = "FAIL" Then
            Packer2.Text = "FAIL"
            Panel2.BackColor = Color.Red
            packer2_bool = False
        Else
            Packer2.Text = USERNAME
            Panel2.BackColor = Color.Green
            TextBox2.Enabled = False
            packer2_bool = True
        End If
    End Sub

    Private Sub TextBox3_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode <> Keys.Enter Then Exit Sub
        Dim BADGENUMBER As String = TextBox3.Text.Trim
        Dim USERNAME As String = GetName(BADGENUMBER)
        If USERNAME = "FAIL" Then
            Packer3.Text = "FAIL"
            Panel3.BackColor = Color.Red
            packer3_bool = False
        Else
            Packer3.Text = USERNAME
            Panel3.BackColor = Color.Green
            TextBox3.Enabled = False
            packer3_bool = True
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If packer1_bool = False Then Exit Sub
        If packer2_bool = False Then Exit Sub
        If packer1_bool = True Then Packet.packer1 = Packer1.Text.Trim
        If packer2_bool = True Then Packet.packer2 = Packer2.Text.Trim
        If packer3_bool = True Then Packet.packer3 = Packer3.Text.Trim

        Packet.Enabled = True
        Packet.Show()
        Me.Close()
        'Packet.test()

    End Sub

    Private Sub Login_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        'Me.Close()
        If packer1_bool = False Then Packet.Close()
        If packer2_bool = False Then Packet.Close()
        Packet.Show()
        Packet.Enabled = True

    End Sub
End Class