Imports Microsoft.Reporting.WinForms
Imports System
Imports System.IO
Imports System.Net
Imports System.Data
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.Collections.Generic
Imports System.Data.SqlClient

Public Class Frm_pkPrint
    Dim A4_Print As New Printing.PrintDocument
    Dim ReportViewer1 As New ReportViewer
    Dim CASEID As String
    Dim Exkey As String
    Private m_currentPageIndex As Integer

    'Dim T_GUINODetail As List(Of DataRow)
    'Dim T_CASEDETAIL As List(Of PRODDataSet.CASEDETAILRow)
    Private m_streams As IList(Of IO.Stream)



    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Packet.Show()
        Me.Close()
    End Sub

    Private Sub pokey_Text_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles pokey_Text.KeyPress
        If Asc(e.KeyChar) = 13 Then
            GetSonoData(pokey_Text.Text.Trim)
        End If
    End Sub
    Sub AutoPrint(ByVal printtype As String, ByVal Rport As LocalReport)

        A4_Print.DocumentName = "A4_Print"
        A4_Print.PrinterSettings.PrinterName = "A4_Print"

        Dim deviceInfo As String
        deviceInfo =
             "<DeviceInfo>" +
             "  <OutputFormat>EMF</OutputFormat>" +
             "  <PageWidth>29.7cm</PageWidth>" +
             "  <PageHeight>21cm</PageHeight>" +
             "  <MarginTop>1cm</MarginTop>" +
             "  <MarginLeft>0cm</MarginLeft>" +
             "  <MarginRight>0cm</MarginRight>" +
             "  <MarginBottom>1cm</MarginBottom>" +
             "</DeviceInfo>"

            '將報表轉為圖形串流
            Dim warnings() As Warning = Nothing
            m_streams = New List(Of Stream)()
            m_streams.Clear()

        Rport.Render("Image", deviceInfo, AddressOf CreateStream, warnings)
            '設定串流的起始位置
            For Each stream As Stream In m_streams
                stream.Position = 0
            Next

            If m_streams Is Nothing Or m_streams.Count = 0 Then
                Exit Sub
            End If

        If Not PrintDocument1.PrinterSettings.IsValid Then
            Throw New Exception("Error: cannot find the default printer.")
        Else
            'AddHandler PrintDocument1.PrintPage, AddressOf PrintPage
            m_currentPageIndex = 0

            PrintDocument1.Print()
        End If
 

    End Sub
    Private Function CreateStream(ByVal name As String,
ByVal fileNameExtension As String,
ByVal encoding As System.Text.Encoding, ByVal mimeType As String,
ByVal willSeek As Boolean) As Stream

        Dim stream As Stream = New FileStream(name + "." _
         + fileNameExtension, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)
        m_streams.Add(stream)
        Return stream

    End Function

    Private Sub pokey_Text_TextChanged(sender As System.Object, e As System.EventArgs) Handles pokey_Text.TextChanged

    End Sub

    Private Sub Frm_pkPrint_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'TODO: 這行程式碼會將資料載入 'PRODDataSet.Vw_Rpt_PickList' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_Rpt_PickListTableAdapter.Fill(Me.PRODDataSet.Vw_Rpt_PickList)
        'Me.ReportViewer2.RefreshReport()
        'GetSonoData("Rainestest")
        'Me.TopMost = True
    End Sub

    Private Sub Btn_Close_Click(sender As System.Object, e As System.EventArgs) Handles Btn_Close.Click

    End Sub
    Private Sub GetSonoData(ByVal sono As String)
        sono = Me.Vw_Rpt_PickListTableAdapter.GetOrderkey(sono)
        If sono.Trim = "" Then Exit Sub
        Me.Vw_Rpt_PickListTableAdapter.FillBy_ORDERKEY(Me.PRODDataSet.Vw_Rpt_PickList, sono)
        Me.ReportViewer2.Visible = True
        Me.ReportViewer2.RefreshReport()
        AutoPrint("A4_Print", ReportViewer2.LocalReport)
    End Sub

    Private Sub PrintDocument1_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim pageImage As New Imaging.Metafile(m_streams(m_currentPageIndex)) '取得資料流
        '啟動版面設定對話方塊
        Dim PageSetupDialog1 As New PageSetupDialog
        PageSetupDialog1.PageSettings = New System.Drawing.Printing.PageSettings
        'PageSetupDialog1.PrinterSettings = New System.Drawing.Printing.PrinterSettings
        PageSetupDialog1.ShowNetwork = False
        '設為預設之左邊界
        PageSetupDialog1.PageSettings.Margins.Left = 0
        '設為預設之右邊界
        PageSetupDialog1.PageSettings.Margins.Right = 0
        '設為預設之上邊界
        PageSetupDialog1.PageSettings.Margins.Top = 0
        '設為預設之下邊界
        PageSetupDialog1.PageSettings.Margins.Bottom = 0

        '設定紙張名稱,寬與高
        'Dim pkCustomSize1 As New System.Drawing.Printing.PaperSize("711", "80", "110")
        '套入格式給PrintDocument
        'PrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize = pkCustomSize1

        'PrintDocument1.DefaultPageSettings.PaperSize = PageSetupDialog1.PageSettings.PaperSize
        'MsgBox(PrintDocument1.PrinterSettings.DefaultPageSettings.ToString)

        e.Graphics.DrawImage(pageImage, e.PageBounds) '繪製(將資料送至印表機)
        m_currentPageIndex += 1
        e.HasMorePages = (m_currentPageIndex < m_streams.Count)
    End Sub
End Class