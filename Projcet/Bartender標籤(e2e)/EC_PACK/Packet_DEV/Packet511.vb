Imports User_Function.User_Function
Imports Neodynamic.WinControls.BarcodeProfessional
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
Imports System.Windows.Forms

Public Class Packet
    Public Shared TotalWeight As String = ""
    Dim Tax As Double = GetTax()
    Dim actQty As Integer = 0
    Dim CASEID As String
    Dim AGV_Box As String
    Dim Exkey As String
    Private m_currentPageIndex As Integer

    'Dim T_GUINODetail As List(Of DataRow)
    'Dim T_CASEDETAIL As List(Of PRODDataSet.CASEDETAILRow)
    Private m_streams As IList(Of IO.Stream)

    'Dim msg As New ObjectParameter("msg", GetType(String))
    'Dim NewCASEID As New ObjectParameter("Returnkey", GetType(String))
    Dim msg As String
    Dim QTmp As DataTable
    Dim NewCASEID As String
    Dim ROUTE As String
    Dim T_GUINO As IEnumerable(Of String)

    'Dim T_PickDetail_Rate As DataTable
    Dim T_PickDetail_Rate As List(Of PRODDataSet.Vw_PICKDETAIL_RateRow)

    Dim T_SKUDetail As PRODDataSet.Vw_PICKDETAIL_RateRow
    '儲存圖片資料流

    Dim readini As vs.com.Readini.INIReader = New vs.com.Readini.INIReader()
    Public Shared AppPatch As String = System.Environment.CurrentDirectory
    Dim MonitorStation As String
    Dim username As String = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString
    Dim MSAPI1 As String = "http://172.16.1.101/MonitoringService/videocalls.asmx?op=SendCommand"
    'ini讀取
    Function GetTax() As Double
        Try
            Dim API As String = "https://tw.rter.info/currency/USD/"
            Dim json As String = ""
            Dim request As HttpWebRequest
            Dim SoapByte() As Byte
            Dim response As HttpWebResponse
            Dim Datastream As Stream
            Dim reader As StreamReader
            request = DirectCast(WebRequest.Create(API), HttpWebRequest)
            request.Method = "POST"
            request.Timeout = 6000
            request.ContentType = "application/ld+json"
            request.ContentLength = json.Length
            SoapByte = System.Text.Encoding.UTF8.GetBytes(json)
            Datastream = request.GetRequestStream()
            Datastream.Write(SoapByte, 0, json.Length)
            Datastream.Close()
            response = request.GetResponse()
            Datastream = response.GetResponseStream()
            reader = New StreamReader(Datastream)
            Dim SD2Request As String = reader.ReadToEnd
            Console.WriteLine("call API by json" + SD2Request)

            Dim str As String = SD2Request.Substring(SD2Request.IndexOf("美金＝"), 10)
            Dim tax As Double = 0
            tax = CDbl(str.Substring(3, 5))
            If tax <= 0 Then
                tax = 1
            End If
            Return tax
        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function

    Public Shared Function sqlTmpData(ByVal sqlstr As String) As DataTable
        Dim objdatastable As New DataTable()
        Dim Adapter As SqlClient.SqlDataAdapter
        Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(My.Settings.PRODConnectionString)

        Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand(sqlstr, con)
        cmd.CommandTimeout = 1800
        Try
            Adapter = New SqlClient.SqlDataAdapter(cmd)
            objdatastable = New DataTable
            Adapter.Fill(objdatastable)
        Catch ex As Exception
            MsgBox("NO：" & Err.Number & "描述：" & Err.Description)
            'End
            Exit Try
        End Try
        Return objdatastable
    End Function

    Shared Function transfer(ByVal inputNumber As String) As String
        Dim w(10), z(10), r(10)
        Dim result As String
        'word = Text1.Text
        For i As Integer = 1 To Len(inputNumber)
            w(i) = Mid(inputNumber, i, 1)
            If w(i) = 0 Then r(i) = "零"
            If w(i) = 1 Then r(i) = "壹"
            If w(i) = 2 Then r(i) = "貳"
            If w(i) = 3 Then r(i) = "參"
            If w(i) = 4 Then r(i) = "肆"
            If w(i) = 5 Then r(i) = "伍"
            If w(i) = 6 Then r(i) = "陸"
            If w(i) = 7 Then r(i) = "柒"
            If w(i) = 8 Then r(i) = "捌"
            If w(i) = 9 Then r(i) = "玖"
        Next i
        z(8) = ("仟萬")
        z(7) = ("佰萬")
        z(6) = ("拾萬")
        z(5) = ("萬")
        z(4) = ("仟")
        z(3) = ("佰")
        z(2) = ("拾")
        z(1) = ("")
        result = ""
        Dim L = Len(inputNumber)
        For i = 1 To Len(inputNumber)
            result = result & r(i).ToString & z(L).ToString
            L = L - 1
        Next i

        result = result + "元整"
        Return result

    End Function

    Public Sub allprint()

        Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_PickFinish("sa", "CASEFINISH", Exkey, ROUTE, CASEID, "Y", msg)
        If msg = 1 Then

            '列印遞紙條,發票
            print_Addr_invoice(Exkey, CASEID, ROUTE)
            init()
        End If

    End Sub

    Sub AutoPrint(ByVal printtype As String, ByVal Rport As ReportViewer)

        PrintDocument1.DocumentName = "CirCle K_Print"
        PrintDocument1.PrinterSettings.PrinterName = "TSC TTP-247"
        PrintDocument2.DocumentName = "A4_Print"
        PrintDocument2.PrinterSettings.PrinterName = "A4_Print"

        'PrintDocument2.PrinterSettings.PrinterName = "HP LaserJet P3015"
        'PrintDocument2.PrinterSettings.PrinterName = "HP M607"
        Dim deviceInfo As String
        If printtype = "addr" Then
            '設定EMF參數
            deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>EMF</OutputFormat>" +
            "  <PageWidth>8cm</PageWidth>" +
            "  <PageHeight>11cm</PageHeight>" +
            "  <MarginTop>0cm</MarginTop>" +
            "  <MarginLeft>0cm</MarginLeft>" +
            "  <MarginRight>0cm</MarginRight>" +
            "  <MarginBottom>0cm</MarginBottom>" +
            "</DeviceInfo>"

            '將報表轉為圖形串流
            Dim warnings() As Warning = Nothing
            m_streams = New List(Of Stream)()
            m_streams.Clear()

            'Dim viewer As New ReportViewer()
            'viewer.ProcessingMode = ProcessingMode.Local
            ''外部目錄
            ''viewer.LocalReport.ReportPath = "C:\RDLC\Report_FEDEXinvoice.rdlc"
            ''專案目錄
            'viewer.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_FEDEXinvoice.rdlc"
            'viewer.LocalReport.DataSources.Add(rds)
            'Dim params() As ReportParameter = {New ReportParameter("Tkey", Tkey)}
            'viewer.LocalReport.SetParameters(params)
            'Dim bytes As Byte() = viewer.LocalReport.Render("PDF", deviceInfo, AddressOf CreateStream, warnings)
            'Dim fs As New FileStream(Path, FileMode.Create)
            'fs.Write(bytes, 0, bytes.Length)
            'fs.Close()



            Rport.LocalReport.Render("Image", deviceInfo, AddressOf CreateStream, warnings)
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
        Else
            If ROUTE = "Z2" Then
                '設定EMF參數()
                'deviceInfo =
                ' "<DeviceInfo>" +
                ' "  <OutputFormat>EMF</OutputFormat>" +
                ' "</DeviceInfo>"
                deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>EMF</OutputFormat>" +
                "  <PageWidth>21cm</PageWidth>" +
                "  <PageHeight>29.7cm</PageHeight>" +
                "  <MarginTop>-1cm</MarginTop>" +
                "  <MarginLeft>0cm</MarginLeft>" +
                "  <MarginRight>0cm</MarginRight>" +
                "  <MarginBottom>0cm</MarginBottom>" +
                "</DeviceInfo>"

            Else
                deviceInfo =
                 "<DeviceInfo>" +
                 "  <OutputFormat>EMF</OutputFormat>" +
                 "  <PageWidth>21cm</PageWidth>" +
                 "  <PageHeight>29.7cm</PageHeight>" +
                 "  <MarginTop>1cm</MarginTop>" +
                 "  <MarginLeft>0cm</MarginLeft>" +
                 "  <MarginRight>0cm</MarginRight>" +
                 "  <MarginBottom>1cm</MarginBottom>" +
                 "</DeviceInfo>"
            End If




            '將報表轉為圖形串流
            Dim warnings() As Warning = Nothing
            m_streams = New List(Of Stream)()
            m_streams.Clear()

            Rport.LocalReport.Render("Image", deviceInfo, AddressOf CreateStream, warnings)
            '設定串流的起始位置
            For Each stream As Stream In m_streams
                stream.Position = 0
            Next

            If m_streams Is Nothing Or m_streams.Count = 0 Then
                Exit Sub
            End If

            If Not PrintDocument2.PrinterSettings.IsValid Then
                Throw New Exception("Error: cannot find the default printer.")
            Else
                'AddHandler PrintDocument1.PrintPage, AddressOf PrintPage
                m_currentPageIndex = 0

                PrintDocument2.Print()
            End If

        End If

    End Sub
    Sub AutoPrint(ByVal printtype As String, ByVal Rport As LocalReport)

        PrintDocument1.DocumentName = "CirCle K_Print"
        PrintDocument1.PrinterSettings.PrinterName = "TSC TTP-247"
        PrintDocument2.DocumentName = "A4_Print"
        'PrintDocument2.PrinterSettings.PrinterName = "HP LaserJet P3015"
        PrintDocument2.PrinterSettings.PrinterName = "A4_Print"
        'PrintDocument2.PrinterSettings.PrinterName = "HP M607"
        Dim deviceInfo As String
        If printtype = "addr" Then
            '設定EMF參數
            deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>EMF</OutputFormat>" +
            "  <PageWidth>8cm</PageWidth>" +
            "  <PageHeight>11cm</PageHeight>" +
            "  <MarginTop>0cm</MarginTop>" +
            "  <MarginLeft>0cm</MarginLeft>" +
            "  <MarginRight>0cm</MarginRight>" +
            "  <MarginBottom>0cm</MarginBottom>" +
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
        Else

            '設定EMF參數
            'deviceInfo =
            ' "<DeviceInfo>" +
            ' "  <OutputFormat>EMF</OutputFormat>" +
            ' "</DeviceInfo>"

            deviceInfo =
             "<DeviceInfo>" +
             "  <OutputFormat>EMF</OutputFormat>" +
             "  <PageWidth>21cm</PageWidth>" +
             "  <PageHeight>29.7cm</PageHeight>" +
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

            If Not PrintDocument2.PrinterSettings.IsValid Then
                Throw New Exception("Error: cannot find the default printer.")
            Else
                'AddHandler PrintDocument1.PrintPage, AddressOf PrintPage
                m_currentPageIndex = 0

                PrintDocument2.Print()
            End If

        End If

    End Sub

    'Public Function ClientApi_HJ(ByVal Mods As String, ByVal Wkey As String, ByVal Extkeys As String) As String
    '    Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(SqlPKPas.TPKconstr)
    '    Dim cmd As SqlClient.SqlCommand
    '    Dim sqls As String = ""
    '    Dim fgStr As String = ""
    '    Try

    '        con.Open()
    '        cmd = New SqlClient.SqlCommand("ClientApi_HJ", con)
    '        cmd.CommandTimeout = 1800
    '        cmd.CommandType = CommandType.StoredProcedure

    '        cmd.Parameters.Add("@Infmod", SqlDbType.VarChar, 30).Value = Mods
    '        cmd.Parameters.Add("@Wkey", SqlDbType.VarChar, 30).Value = Wkey
    '        cmd.Parameters.Add("@Extkeys", SqlDbType.VarChar, 50).Value = Extkeys
    '        'cmd.Parameters.Add("@lino", SqlDbType.VarChar, 20).Value = LINE_NO

    '        cmd.Parameters.Add("@GetVal", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output
    '        cmd.ExecuteNonQuery()
    '        ClientApi_HJ = cmd.Parameters(3).Value.ToString
    '    Catch ex As Exception
    '        ClientApi_HJ = "DB::function(ClientApi_HJ) :" & ex.Message

    '    Finally
    '    End Try
    'End Function










    Private Sub AddrReportSet(ByVal caseid As String, ByVal route As String)
        Dim bcp As New Neodynamic.WinControls.BarcodeProfessional.BarcodeProfessional()
        Dim errsx As String = ""
        Dim ApiMsg As String = ""
        Dim reportDate = PRODDataSet.Vw_Case_Addr_Invoice.First(Function(s) s.CASEID = caseid)
        Dim reportTable As List(Of PRODDataSet.Vw_Case_Addr_InvoiceRow) = New List(Of PRODDataSet.Vw_Case_Addr_InvoiceRow)
        'Dim rds As New ReportDataSource("DataSet1", Vw_Case_Addr_InvoiceBindingSource)
        reportTable.Add(reportDate)
        Dim OrderNo As String = reportTable(0).EXTERNORDERKEY.ToString
        Dim STORERKEY As String = reportTable(0).STORERKEY.ToString
        If STORERKEY = "G016" And (route = "Z1" Or route = "Z7") Then
            Dim shopeeApi As ShopeeLogisticsOrderLogisticsResponseDto
            Try
                shopeeApi = ShopeeService.GetShopeeFamilyLogistic(OrderNo)
                If shopeeApi.Logistics Is Nothing Then
                    MsgBox("SHOPEE 無回應:" & shopeeApi.Error)
                    Exit Sub
                End If
                If caseid = "" Then
                    MsgBox("SHOPEE 無回應:" & shopeeApi.Error)
                    Exit Sub
                End If
            Catch ex As Exception
                'If MsgBox("SHOPEE 備貨失敗(缺面單)；是否再執行一次出貨通知[Yes]!或煩請通知帳務人員確認單據狀況[No]", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                '    'SqlPKPas.ExcuteSql("EXEC PROD..GetKey '" + STORERKEY + route + "','" + OrderNo + "','';")
                'End If
                NEDISqlH.SqlApi_Shoppe("SJOB..EDIQry_ApiBuildStrSTD", "tracking", OrderNo, "G016")
                apiMsg = NEDISqlH.SqlApi_Shoppe("SJOB..EDIApiBus_DFlow", "tracking", OrderNo, "G016")
                If IsNumeric(ApiMsg) Then
                    MsgBox("Shoppe 出貨單成立: " & ApiMsg)
                    AddrReportSet(caseid, route)
                Else
                    If MsgBox(ApiMsg & "；是否再執行一次出貨通知[Yes]!或煩請通知帳務人員確認單據狀況[" & OrderNo & "]", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        NEDISqlH.SqlApi_Shoppe("SJOB..EDIQry_ApiBuildStrSTD", "tracking", OrderNo, "G016")
                        ApiMsg = NEDISqlH.SqlApi_Shoppe("SJOB..EDIApiBus_DFlow", "tracking", OrderNo, "G016")
                    End If

                End If
                Exit Sub
            End Try

            If STORERKEY = "G016" And route = "Z7" Then
                If MsgBox(ApiMsg & "；是否再執行一次出貨通知[Yes]!或煩請通知帳務人員確認單據狀況[" & OrderNo & "]", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    NEDISqlH.SqlApi_Shoppe("SJOB..EDIQry_ApiBuildStrSTD", "tracking", OrderNo, "G016")
                    ApiMsg = NEDISqlH.SqlApi_Shoppe("SJOB..EDIApiBus_DFlow", "tracking", OrderNo, "G016")
                End If
                '20210204 增加店號更改更新33的casehead 與 casedetail 的欄位CASELABLE
                If reportTable(0).DOOR <> shopeeApi.Logistics.ThirdPartyLogisticInfo.BarCode.Substring(0, 6) Then
                    Dim objoraclecon As SqlClient.SqlConnection = New SqlClient.SqlConnection("Data Source=192.168.1.33;Initial Catalog=prod;Persist Security Info=True;User ID=sa;Password=zaq1")
                    Dim objcmd As SqlClient.SqlCommand
                    Dim sqlstr As String
                    sqlstr = ""
                    sqlstr = "update CaseHead Set  CaseLable = '" + shopeeApi.Logistics.ThirdPartyLogisticInfo.BarCode + "' Where CaseID = '" + shopeeApi.Logistics.TrackingNo + "';"
                    sqlstr = sqlstr + " update CaseDetail Set  CaseLable = '" + shopeeApi.Logistics.ThirdPartyLogisticInfo.BarCode + "' Where CaseID = '" + shopeeApi.Logistics.TrackingNo + "';"
                    Try
                        objoraclecon.Open()
                        objcmd = New SqlClient.SqlCommand(sqlstr, objoraclecon)
                        objcmd.CommandTimeout = 1800
                        objcmd.ExecuteNonQuery()
                        objoraclecon.Close()
                    Catch ex As Exception
                        MsgBox("错误号：店號未更新")
                    End Try
                End If

                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.Code128
                reportTable(0).CASEID = shopeeApi.Logistics.TrackingNo
                reportTable(0).DESCR = shopeeApi.Logistics.ThirdPartyLogisticInfo.ServiceDescription
                reportTable(0).C_CONTACT1 = shopeeApi.Logistics.RecipientAddress.Name
                Dim addrsAr() As String = shopeeApi.Logistics.RecipientAddress.ToString.Split(Chr(20))
                Dim addrs As String = CType(shopeeApi.Logistics.RecipientAddress.FullAddress, String)
                reportTable(0).Company = addrs.Substring(5, 4) + " " + addrs.Substring(addrs.Length - 6, 6)
                reportTable(0).CASELABLE = shopeeApi.Logistics.ThirdPartyLogisticInfo.BarCode



                bcp.Symbology = Symbology.Code128
                bcp.DisplayChecksum = False
                bcp.DisplayCode = False   ' 取消條碼下方的文字
                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)
                ''1970-01-01 08:00:00',
                ' Dim timeL As Long = shopeeApi.Logistics.ThirdPartyLogisticInfo.PurchaseTime
                Dim mydt As DateTime = New DateTime(1970, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(shopeeApi.Logistics.ThirdPartyLogisticInfo.PurchaseTime)
                reportTable(0).EDITWHO = mydt.ToString("yyyy/MM/dd")
                reportTable(0).SPSHIPDATE = mydt.AddDays(7).ToString("yyyy/MM/dd")
                ReportViewer1.Reset()
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_711_SP.rdlc"
                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))
                PrintDocument1.DefaultPageSettings.Landscape = False
            End If
            If route = "Z1" Then
                reportTable(0).CONSIGNEEKEY = shopeeApi.Logistics.ThirdPartyLogisticInfo.OrderNo

                reportTable(0).SOURCEKEY = shopeeApi.Logistics.TrackingNo
                reportTable(0).StoreType2 = CInt(reportTable(0).StoreType2)
                Dim TrackNo As String = reportTable(0).SOURCEKEY
                bcp.Code = shopeeApi.Logistics.TrackingNo
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(ImageFormat.Png)

                reportTable(0).EQID = shopeeApi.Logistics.ThirdPartyLogisticInfo.EquipmentId.ToString()
                reportTable(0).AREA = shopeeApi.Logistics.ThirdPartyLogisticInfo.RecipientArea.ToString()
                reportTable(0).RSNO = shopeeApi.Logistics.ThirdPartyLogisticInfo.RouteStep.ToString()
                reportTable(0).INCOTERM = shopeeApi.Logistics.ThirdPartyLogisticInfo.Prompt.ToString()
                reportTable(0).C_CONTACT1 = shopeeApi.Logistics.RecipientAddress.Name
                reportTable(0).C_PHONE1 = shopeeApi.Logistics.RecipientAddress.Phone
                bcp.DisplayChecksum = False
                bcp.DisplayCode = False
                bcp.AddChecksum = False
                bcp.DisplayStartStopChar = False
                '' TOP BARCODE
                '' Image3
                bcp.Symbology = Symbology.Code128
                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(ImageFormat.Png)
                '' Image2理貨條碼
                bcp.Code = shopeeApi.Logistics.ThirdPartyLogisticInfo.BarCode
                reportTable(0).Barcode_image2 = bcp.GetBarcodeImage(ImageFormat.Png)
                reportTable(0).CASEID = shopeeApi.Logistics.ThirdPartyLogisticInfo.BarCode
                bcp.Code = shopeeApi.Logistics.ThirdPartyLogisticInfo.EcBarCode9.ToString()
                reportTable(0).Barcode_image3 = bcp.GetBarcodeImage(ImageFormat.Png)

                reportTable(0).DESCR = shopeeApi.Logistics.ThirdPartyLogisticInfo.EcBarCode16.ToString()
                bcp.Code = shopeeApi.Logistics.ThirdPartyLogisticInfo.EcBarCode16.ToString()
                reportTable(0).Barcode_image4 = bcp.GetBarcodeImage(ImageFormat.Png)

                ''QRCODE
                bcp.Symbology = Symbology.QRCode
                bcp.Code = shopeeApi.Logistics.ThirdPartyLogisticInfo.QrCode.ToString()
                reportTable(0).Barcode_image1 = bcp.GetBarcodeImage(ImageFormat.Png)

                Me.ReportViewer1.Reset()
                Me.ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_CVS_SP.rdlc"
                Me.ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))
                Me.PrintDocument1.DefaultPageSettings.Landscape = False
                ' SqlPKPas.ExcuteSql("update ORDERS set BUYERPO='" + TrackNo + "' where EXTERNORDERKEY='" + reportTable(0).EXTERNORDERKEY.ToString() + "'")
                ' 修改條碼讀取值
            End If

        Else
            ''---------------------- 面單範圍 
            If route = "Z7" Then

                If reportTable(0).Company = "" Then
                    MsgBox("無門市名稱")
                    Return
                End If

                If reportTable(0).C_CONTACT1 = "" Then
                    MsgBox("提貨人姓名空白")
                    Return
                End If

                bcp.Symbology = Symbology.Code128
                'bcp.Code128CharSet = Code128.Auto
                'bcp.TextAlignment = Alignment.AboveCenter
                'bcp.BarcodePadding.Left = 0.5
                'bcp.BarcodePadding.Right = 0
                'bcp.BarcodePadding.Top = 0
                'bcp.BarcodePadding.Bottom = 0
                bcp.DisplayChecksum = False '
                bcp.DisplayCode = False   ' 取消條碼下方的文字
                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                ReportViewer1.Reset()
                'ReportViewer1.LocalReport.ReportPath = "C:\RDLC\Report_711.rdlc"
                'ReportViewer1.LocalReport.DataSources.Add(rds)
                'If (reportTable(0).STORERKEY.Equals("G016") Or reportTable(0).STORERKEY.Equals("QQQQ")) Then
                '    ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_711_SP.rdlc"
                'Else
                '    ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_711.rdlc"
                'End If
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_711.rdlc"
                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))

                PrintDocument1.DefaultPageSettings.Landscape = False
                'For Each row In PRODDataSet.Vw_Address.Rows
                '    bcp.Code = row.CASELABLE
                '    row.Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)
                'Next

            ElseIf route = "Z4" Then
                'MsgBox("Z4")
                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.Code128
                bcp.DisplayChecksum = False '
                bcp.DisplayCode = False   ' 取消條碼下方的文字
                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                ReportViewer1.Reset()
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_Door.rdlc"
                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))

                PrintDocument1.DefaultPageSettings.Landscape = False

            ElseIf route = "Z9" Then
                'MsgBox("Z9")

                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.Code39
                bcp.DisplayCode = False
                bcp.AddChecksum = False
                bcp.Extended = False

                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)
                bcp.Code = reportTable(0).EXTERNORDERKEY.ToString
                reportTable(0).Barcode_image2 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                ReportViewer1.Reset()
                'ReportViewer1.LocalReport.ReportPath = "C:\RDLC\Report_CirCle.rdlc"
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_CirCle.rdlc"
                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))
                PrintDocument1.DefaultPageSettings.Landscape = False









                '600 line--<20210314 增加 日翊全家>--------------------------------
            ElseIf route = "ZB" Then
                If reportTable(0).StoreType2 = "01" And reportTable(0).EQID = "" Then
                    MsgBox("全家設備代碼空白")
                    Return
                End If
                If reportTable(0).CASELABLE = "" Or reportTable(0).Labx03 = "" Or reportTable(0).Labx01 = "" Then
                    MsgBox("全家Qrcode\2段條碼空白")
                    Return
                End If
                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.Code128
                bcp.DisplayChecksum = False '
                'bcp.BarHeight = 0.55F
                bcp.DisplayCode = False   ' 取消條碼下方的文字
                bcp.AddChecksum = False
                bcp.DisplayStartStopChar = False
                'TOP 條碼 :CaseID =配送號
                bcp.Code = reportTable(0).CASEID
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '貨路線 = "1RA02"
                bcp.Code = reportTable(0).StoreType2.Substring(1, 1) + reportTable(0).RSNO + reportTable(0).STEP2
                reportTable(0).Barcode_image2 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '第一段條碼
                bcp.Code = reportTable(0).Labx02
                reportTable(0).Barcode_image3 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '第二段條碼 (CaseLable) 也可刷
                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image4 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'QRcode
                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.QRCode
                bcp.Code = reportTable(0).Labx03
                reportTable(0).Barcode_image1 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                ReportViewer1.Reset()
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_ERY_ZB.rdlc"

                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))
                PrintDocument1.DefaultPageSettings.Landscape = False

            ElseIf route = "Z1" Then
                'MsgBox("Z1")

                If reportTable(0).StoreType2 = "01" And reportTable(0).EQID = "" Then
                    MsgBox("設備代碼空白")
                    Return
                End If

                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.Code128
                bcp.DisplayChecksum = False '
                'bcp.BarHeight = 0.55F
                bcp.DisplayCode = False   ' 取消條碼下方的文字
                bcp.AddChecksum = False
                bcp.DisplayStartStopChar = False

                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                Dim TFline, RTFline
                If reportTable(0).StoreType2 = "01" Then
                    TFline = "1"
                    RTFline = "2"
                ElseIf reportTable(0).StoreType2 = "02" Then
                    TFline = "2"
                    RTFline = "5"
                Else
                    TFline = "3"
                    RTFline = "6"
                End If

                'bcp.Code = "1RA02"
                bcp.Code = TFline + reportTable(0).RSNO
                reportTable(0).Barcode_image2 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '第一段條碼
                Dim code1
                If (reportTable(0).STORERKEY.Equals("G016") Or reportTable(0).STORERKEY.Equals("QQQQ")) Then
                    code1 = "727" + Microsoft.VisualBasic.Left(reportTable(0).CASEID, 3) + "950"
                Else
                    code1 = "169" + Microsoft.VisualBasic.Left(reportTable(0).CASEID, 3) + "963"
                End If
                bcp.Code = code1
                reportTable(0).Barcode_image3 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '第二段條碼
                Dim TFtype
                If reportTable(0).INCOTERM = "1" Then
                    TFtype = "1"
                Else
                    TFtype = "3"
                End If

                Dim code2 = Microsoft.VisualBasic.Right(reportTable(0).CASEID, 8) + TFtype + (CInt(reportTable(0).C_KEEPPY)).ToString().PadLeft(5, "0")

                Dim A = 0, B = 0

                For pos As Integer = 1 To code1.ToString().Length
                    If (pos Mod 2 = 0) Then
                        B = B + Microsoft.VisualBasic.Mid(code1, pos, 1)
                    Else
                        A = A + Microsoft.VisualBasic.Mid(code1, pos, 1)
                    End If
                Next

                For pos As Integer = 1 To code2.ToString().Length
                    If (pos Mod 2 = 0) Then
                        B = B + Microsoft.VisualBasic.Mid(code2, pos, 1)
                    Else
                        A = A + Microsoft.VisualBasic.Mid(code2, pos, 1)
                    End If
                Next

                If A Mod 11 = 10 Then
                    A = 1
                Else
                    A = A Mod 11
                End If

                If B Mod 11 = 0 Then
                    B = 8
                ElseIf B Mod 11 = 10 Then
                    B = 9
                Else
                    B = B Mod 11
                End If

                '暫存
                code2 = code2 + A.ToString() + B.ToString()
                reportTable(0).DESCR = code2
                bcp.Code = reportTable(0).DESCR
                reportTable(0).Barcode_image4 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'QRcode
                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.QRCode
                bcp.Code = "B1||                  ||         ||                  ||               ||" + reportTable(0).CASELABLE + "||" + RTFline + "||" + Microsoft.VisualBasic.Right(reportTable(0).DOOR, 6) + "||" + TFline + reportTable(0).RSNO + "||" + reportTable(0).EQID.PadLeft(2, " ") + "||0||" + code1 + "||" + code2 + "||             ||          "
                'bcp.Code = "B1||                  ||         ||                  ||               ||" + reportTable(0).CASELABLE + "||2||" + Microsoft.VisualBasic.Right(reportTable(0).DOOR, 6) + "||1" + "RA02" + "||" + " 1" + "||0||" + code1 + "||" + code2 + "||             ||          "
                reportTable(0).Barcode_image1 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                ReportViewer1.Reset()
                If (reportTable(0).STORERKEY.Equals("G016") Or reportTable(0).STORERKEY.Equals("QQQQ")) Then
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_CVS_SP.rdlc"
                Else
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_CVS_new.rdlc"
                End If

                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))
                PrintDocument1.DefaultPageSettings.Landscape = False

            ElseIf route = "Z2" Then
                'MsgBox("Z2")
                If reportDate.C_ZIP = "" Then
                    MsgBox("郵碼錯誤")
                    reportDate.C_ZIP = " - - - "
                    'Return
                End If

                If reportDate.CASELABLE = "" Then
                    MsgBox("配送號錯誤")
                    Return
                End If

                ReportViewer1.Reset()
                'ReportViewer1.LocalReport.ReportPath = "C:\RDLC\Report_eZCat.rdlc"
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_eZCat.rdlc"

                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.Code39
                bcp.BarHeight = 0.25F
                bcp.AddChecksum = False
                bcp.Extended = False
                'bcp.DisplayCode = True

                Dim params() As ReportParameter =
                {
                    New ReportParameter("catKey1", reportDate.CASELABLE.ToString),
                    New ReportParameter("SPdate", reportDate.SHIPDATE.ToString),
                    New ReportParameter("Dtime1", "Non"),
                    New ReportParameter("down1", reportDate.C_CONTACT1.ToString),
                    New ReportParameter("BOOK1", "書籍"),
                    New ReportParameter("odkey1", reportDate.EXTERNORDERKEY.ToString),
                    New ReportParameter("notes1", "BOOK"),
                    New ReportParameter("addr1", reportDate.C_ADDRESS1.ToString),
                    New ReportParameter("dtel1", reportDate.C_PHONE1.ToString),
                    New ReportParameter("ZIP1", reportDate.C_ZIP.ToString),
                    New ReportParameter("KEEPPY1", reportDate.C_KEEPPY.ToString),
                    New ReportParameter("version", reportDate.SUSR5.ToString)
                }

                'For Each row In PRODDataSet.Vw_Case_Addr_Invoice.Rows

                '    Dim params() As ReportParameter =
                '    {
                '        New ReportParameter("catKey1", row.CASEID.ToString),
                '        New ReportParameter("SPdate", row.SHIPDATE.ToString),
                '        New ReportParameter("Dtime1", "Non"),
                '        New ReportParameter("down1", row.C_CONTACT1.ToString),
                '        New ReportParameter("BOOK1", "書籍"),
                '        New ReportParameter("odkey1", row.EXTERNORDERKEY.ToString),
                '        New ReportParameter("notes1", "BOOK"),
                '        New ReportParameter("addr1", row.C_ADDRESS1.ToString),
                '        New ReportParameter("dtel1", row.C_PHONE1.ToString),
                '        New ReportParameter("ZIP1", row.C_ZIP.ToString),
                '        New ReportParameter("KEEPPY1", row.C_KEEPPY)
                '    }

                'New ReportParameter("compid1", row.compid.ToString),
                'New ReportParameter("pdsend1", row.pdsend.ToString),

                '传递报表中的参数集合
                ReportViewer1.LocalReport.SetParameters(params)

                bcp.DisplayCode = False   ' 取消條碼下方的文字
                '   包裹查詢號碼
                bcp.Code = reportTable(0).CASELABLE
                reportTable(0).Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'bcp.Code = row.CASEID
                'row.Barcode_image = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '   誠品訂單編號
                bcp.Code = reportTable(0).EXTERNORDERKEY
                reportTable(0).Barcode_image1 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'bcp.Code = row.EXTERNORDERKEY
                'row.Barcode_image1 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '   客訂代號
                '   20210328更新 Raines
                'bcp.Code = "2315553131"    '20210402舊的
                bcp.Code = "2795296601"
                reportTable(0).Barcode_image3 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'bcp.Code = "2315553131"
                'row.Barcode_image3 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                '   轉運站碼及理貨區碼
                bcp.Code = "+" + Microsoft.VisualBasic.Left(reportTable(0).C_ZIP.Replace("-", ""), 7)
                reportTable(0).Barcode_image2 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'QRcode
                bcp.Symbology = Neodynamic.WinControls.BarcodeProfessional.Symbology.QRCode
                bcp.Code = "01|" + reportTable(0).CASELABLE + "|10|279529660100|N|0|01|01|02|" + reportTable(0).C_ZIP.Replace("-", "").Substring(2) + "|" + Format(DateAdd("d", +1, reportTable(0).SHIPDATE), "yyyyMMdd") + "|01||0|||||||||||"
                '20210402舊的
                'bcp.Code = "01|" + reportTable(0).CASELABLE + "|10|231555313100|N|0|01|01|02|" + reportTable(0).C_ZIP.Replace("-", "").Substring(2) + "|" + Format(DateAdd("d", +1, reportTable(0).SHIPDATE), "yyyyMMdd") + "|01||0|||||||||||"
                reportTable(0).Barcode_image4 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                'bcp.DisplayCode = False   ' 取消條碼下方的文字
                'bcp.Code = "+" + row.C_ZIP.Replace("-", "")
                'row.Barcode_image2 = bcp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Png)

                ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportTable.CopyToDataTable))
                PrintDocument2.DefaultPageSettings.Landscape = False
            End If
        End If

        Me.ReportViewer1.RefreshReport()
        m_currentPageIndex = 0

        '測試先關
        '列印
        Application.DoEvents()
        If route = "Z2" Then
            AutoPrint("A4", ReportViewer1)

        Else
            AutoPrint("addr", ReportViewer1)
        End If
        m_streams.Clear() '測試時不列印請註解本行------------------------------!!!!!!

    End Sub

    Private Sub Btn_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Close_test.Click
        '   系統判斷「此單商品開啟量與作業量一致」才可以結束單據」
        '   刷書號時已判斷
        '   test

        'Dim Tt_GUINO = CInt(Me.T_PickDetail_Rate.AsEnumerable().Where(Function(f) f.Field(Of String)("Externorderkey") = pokey_Text.Text).Select(Function(s) s.Field(Of String)("STATUS")).Min)
        'MsgBox(Tt_GUINO)

        'U_PickDetail = PRODContext.Vw_Combination_PickDetail.Where(Function(s) s.Externorderkey = "10031000170002")
        ' PrintFedexInvice("00031000110001")
        'T_GUINO = T_PickDetail_Rate.AsEnumerable.Where(Function(f) f.Field(Of String)("EXTERNORDERKEY") = "00031000110001").Select(Function(s) s.Field(Of String)("GUINO")).Distinct

        'For Each guino As String In T_GUINO
        '    'PrintInvoice(guino)
        '    MsgBox(guino)
        'Next

        'MsgBox(transfer(10200))

        'U_PickDetail = PRODContext.Vw_Combination_PickDetail.Where(Function(s) s.Externorderkey = "10031000170002")

        'MsgBox("U_PickDetail" + U_PickDetail.Count.ToString)

        'U_GUINO = U_PickDetail.Where(Function(f) f.KUNDEUINR = "").Select(Function(s) s.GUINO).Distinct

        'MsgBox("U_GUINO" + U_PickDetail.Count.ToString)

        'For Each guino As String In U_GUINO
        '    PrintInvoice(guino)
        'Next
        'If T_SKUDetail Is Nothing Then
        '    MsgBox("yy")
        'End If

        PrintOrderDetail("Agv2002112VRV4SXF")

        'Me.Vw_Case_Addr_InvoiceTableAdapter.FillByExkey(PRODDataSet.Vw_Case_Addr_Invoice, "Agv2002112VRV4SXF")
        'Me.ReportViewer3.RefreshReport()

        'AddrReportSet(Txt_SKU.Text.ToString, "Z1")
        'PrintFedexInvice("00041000370001")
        'Application.DoEvents()
        'If ROUTE = "Z2" Then
        '    AutoPrint("A4", ReportViewer1)
        'Else
        '    AutoPrint("addr", ReportViewer1)
        'End If
        'm_streams.Clear() '測試時不列印請註解本行------------------------------!!!!!!
        'Application.DoEvents()
        'AutoPrint("A4", ReportViewer1)
        'm_streams.Clear() '測試時不列印請註解本行------------------------------!!!!!!

        'PrintInvoiceA4("")

        'Dim dd
        'dd = PRODContext.Vw_Combination_PickDetail.Where(Function(s) s.Externorderkey = pokey_Text.Text.ToString).Select(Function(f) f.PART.Distinct)
        'MsgBox(dd.Item(0).ToString)

        'PRODContext.Database.ExecuteSqlCommand("update Combination_PickDetail Set STATUS = 0 where Externorderkey = '00801517465'")

        'U_SKUDetail = U_PickDetail.Where(Function(s) s.SKU Or s.BUSR2 Or s.BUSR3 = "2681495646003")
        'U_SKUDetail.First.PACKQTY = 0
        'U_PickDetail.Where(Function(s) s.Externorderkey = "00801517465").ToList.ForEach(Function(t) t.STATUS = 9 = True)

        ''------------------------------------------
        'For Each tt As Vw_Combination_PickDetail In U_PickDetail.Where(Function(s) s.Externorderkey = "00801517465")
        '    tt.STATUS = 9
        'Next

        'Try
        '    PRODContext.SaveChanges()
        'Catch ex As Exception
        '    Throw
        'End Try
        '-------------------------------------------

        'Me.Vw_AddressTableAdapter.FillByCaseID(PRODDataSet.Vw_Address, "Z200172561")

        'Dim yy As Integer
        'yy = PRODDataSet.CASEDETAIL.Count

        'BarcodeSet("Z7")

        'Me.Vw_AddressTableAdapter.FillByCaseID(PRODDataSet.Vw_Address, "K00176")

        'BarcodeSet("Z9")

        'Application.DoEvents()
        'AutoPrint("addr")
        'm_streams.Clear() '測試時不列印請註解本行------------------------------!!!!!!

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Close.Click
        '   系統判斷此單據類型(VSART)是否為FedEx & CirCleK，若條件成立則依照Txt_Weight.Text內容填入。
        '20210209將功能取消
        Exit Sub
        If pokey_Text.Text.ToString = "" Then
            MsgBox("請輸入單號")
            Return
        End If

        'If (T_SKUDetail.Item("PART") = "N") Then
        '    Me.Vw_PICKDETAIL_RateTableAdapter.GetKey("Sa", "DBE1" + route, NewCASEID)
        '    CASEID = NewCASEID
        '    Me.LAB_CASEID.Text = CASEID
        'End If

        'If (U_SKUDetail.PART = "N") Then
        '    PRODContext.GetKey("Sa", "DBE1" + route, NewCASEID)
        '    CASEID = NewCASEID.Value
        '    Me.LAB_CASEID.Text = CASEID
        'End If

        Try
            If (T_SKUDetail IsNot Nothing And T_SKUDetail.PART = "N") Then

                Me.Vw_PICKDETAIL_RateTableAdapter.GetKey(T_SKUDetail.STORERKEY + ROUTE, "sa", NewCASEID)
                CASEID = NewCASEID
                Me.LAB_CASEID.Text = CASEID

                Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_PickFinish("sa", "PART", Exkey, ROUTE, CASEID, "N", msg)

                'For Each tt As Vw_Combination_PickDetail In U_PickDetail.Where(Function(s) s.Externorderkey = pokey_Text.Text.ToString)
                '    tt.PART = "Y"
                'Next

                'PRODContext.SaveChanges()
            Else
                Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_PickFinish("sa", "CASEFINISH", Exkey, ROUTE, CASEID, "Y", msg)
            End If

            T_PickDetail_Rate = Me.Vw_PICKDETAIL_RateTableAdapter.GetDataByExkey(Exkey).ToList
        Catch ex As Exception
            Throw
        End Try

        If msg = 1 Then
            MsgBox("封箱成功")

            Me.Vw_Case_Addr_InvoiceTableAdapter.FillByExkey(PRODDataSet.Vw_Case_Addr_Invoice, pokey_Text.Text.ToString)
            '列印暫關
            'AddrReportSet(CASEID, ROUTE)

            Me.Vw_PICKDETAIL_RateTableAdapter.GetKey("DBE1" + ROUTE, pokey_Text.Text.ToString, NewCASEID)
            CASEID = NewCASEID
            Me.LAB_CASEID.Text = CASEID

            'Me.Txt_SKU.Text = ""
            Me.Txt_SKU.Focus()
            Me.Txt_SKU.SelectAll()
        Else
            MsgBox("封箱失敗")
        End If

        'PRODDataSet.CASEDETAIL.Clear()

    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Txt_Pcs.Text = 1
            Txt_Pcs.Enabled = False
        Else
            Txt_Pcs.Text = 2
            Txt_Pcs.Enabled = True

        End If
    End Sub

    '產生串流函式
    Private Function CreateStream(ByVal name As String,
    ByVal fileNameExtension As String,
    ByVal encoding As System.Text.Encoding, ByVal mimeType As String,
    ByVal willSeek As Boolean) As Stream

        Dim stream As Stream = New FileStream(name + "." _
         + fileNameExtension, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)
        m_streams.Add(stream)
        Return stream

    End Function

    '   CaseDetail 回收鈕點擊
    Private Sub DGV_CaseDetail_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGV_CaseDetail.CellContentClick

        If e.ColumnIndex = 0 Then

            If MsgBox("確定要回收商品：" & DGV_CaseDetail.CurrentRow.Cells("DescrDataGridViewTextBoxColumn1").Value, vbYesNo) = MsgBoxResult.Yes Then
                '   回收

                Try

                    'Dim deleteItem = PRODContext.Combination_CaseDetail.Where(Function(s) s.Item_No = DGV_CaseDetail.Rows(e.RowIndex).Cells("Item_No").Value)

                    'Dim deleteItem = PRODContext.Combination_CaseDetail.ToList.FirstOrDefault(Function(s) s.Item_No = DGV_CaseDetail.CurrentRow.Cells("Item_No").Value)

                    'Combination_CaseDetailTableAdapter.DeleteQuery(DGV_CaseDetail.CurrentRow.Cells("Item_No").Value)

                    Me.CASEDETAILTableAdapter.CASEDETAIL_RePick("sa", "CB_CANCEL", DGV_CaseDetail.CurrentRow.Cells("ItemNoDataGridViewTextBoxColumn").Value.ToString, "", msg)
                    If msg = 1 Then

                        DGV_CaseDetail.Rows.RemoveAt(e.RowIndex)
                        setPickDetailData()
                        MsgBox("回收成功。")
                    Else
                        MsgBox("回收商品失敗、請聯絡資訊部。")
                    End If
                Catch ex As Exception
                    MsgBox(ex.ToString)
                    MsgBox("回收商品失敗、請聯絡資訊部。")
                End Try
            Else
                '   不回收
            End If
        End If

    End Sub

    Private Sub DGV_PickDetail_RowHeaderMouseDoubleClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles DGV_PickDetail.RowHeaderMouseDoubleClick
        Txt_SKU.Text = DGV_PickDetail.CurrentRow.Cells("SKUDataGridViewTextBoxColumn").Value
        Txt_SKU.Focus()
    End Sub

    Private Sub DGV_reAddr_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DGV_reAddr.CellContentClick

        If (ROUTE = "Z3") Then

            PrintFedexInvice(repokey_Text.Text.ToString)
            Return
        End If

        If e.ColumnIndex = 0 Then
            AddrReportSet(DGV_reAddr.Rows(e.RowIndex).Cells("CASEID").Value, ROUTE)

        End If

    End Sub

    Private Sub DGV_reInvoice_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles DGV_reInvoice.CellContentClick

        If e.ColumnIndex = 0 Then
            Dim invoicData = PRODDataSet.Vw_Case_Addr_Invoice.First(Function(s) s.GUINO = DGV_reInvoice.Rows(e.RowIndex).Cells("GUINO").Value)

            If invoicData.BILLTOKEY = "0000000000" Or invoicData.BILLTOKEY = "" Then
                PrintInvoice(invoicData.GUINO)
            Else
                PrintInvoiceA4(invoicData.GUINO)
            End If

        End If

    End Sub

    Private Sub init()

        actQty = 0
        Me.poInfo_lb.Text = ""
        Me.pokey_Text.Text = ""
        Me.pokey_Text.Focus()
        Me.Txt_SKU.Text = ""
        Me.Txt_SKU.Enabled = False
        Me.LAB_CASEID.Text = ""
        Me.Label10.Text = "－－"
        Me.Label11.Text = "－－"
        Me.Label12.Text = "－－"
        CASEID = ""
        msg = ""
        Exkey = ""

        DGV_PickDetail.DataSource = Nothing
        DGV_CaseDetail.DataSource = Nothing

        MonitorStation = vs.com.Readini.INIReader.getValue(AppPatch.ToString + "\Packet.ini", "ALL", "MonitorStation")
        ComboBox1.Text = MonitorStation
        ComboBox2.Text = username
        MonitorStation_API1("")
    End Sub

    Private Sub InvoiceDataSet()

        '列印發票//0000000000
        T_GUINO = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) (f.BILLTOKEY = "0000000000" Or f.BILLTOKEY = "") And f.PrintMark = "Y").Select(Function(s) s.GUINO).Distinct

        For Each guino As String In T_GUINO
            PrintInvoice(guino)
        Next

        '列印A4發票
        T_GUINO = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.BILLTOKEY <> "0000000000" And f.PrintMark = "Y").Select(Function(s) s.GUINO).Distinct

        For Each guino As String In T_GUINO
            PrintInvoiceA4(guino)
        Next

    End Sub

    Private Sub Packet_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Frm_pkPrint.Show()
        'TODO: 這行程式碼會將資料載入 'PRODDataSet.Vw_Case_Addr_Invoice' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_Case_Addr_InvoiceTableAdapter.Fill(Me.PRODDataSet.Vw_Case_Addr_Invoice)
        'TODO: 這行程式碼會將資料載入 'PRODDataSet.Vw_Case_Addr_Invoice' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_Case_Addr_InvoiceTableAdapter.Fill(Me.PRODDataSet.Vw_Case_Addr_Invoice)
        'TODO: 這行程式碼會將資料載入 'PRODDataSet.Vw_Case_Addr_Invoice' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_Case_Addr_InvoiceTableAdapter.Fill(Me.PRODDataSet.Vw_Case_Addr_Invoice)

        'TODO: 這行程式碼會將資料載入 'PRODDataSet1.Vw_Address' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_AddressTableAdapter.Fill(Me.PRODDataSet1.Vw_Address)
        ''TODO: 這行程式碼會將資料載入 'PRODDataSet1.Vw_Address' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_AddressTableAdapter.Fill(Me.PRODDataSet1.Vw_Address)
        ''TODO: 這行程式碼會將資料載入 'PRODDataSet.Vw_Invoice' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_InvoiceTableAdapter.Fill(Me.PRODDataSet.Vw_Invoice)
        ''TODO: 這行程式碼會將資料載入 'PRODDataSet.Vw_Combination_PickDetail' 資料表。您可以視需要進行移動或移除。
        'Me.Vw_Combination_PickDetailTableAdapter.Fill(Me.PRODDataSet.Vw_Combination_PickDetail)

        Me.CASEDETAILTableAdapter.ClearBeforeFill = True
        Me.Vw_PICKDETAIL_RateTableAdapter.ClearBeforeFill = True
        Me.Vw_Case_Addr_InvoiceTableAdapter.ClearBeforeFill = True

        BarcodeProfessional.LicenseOwner = "Eslite Logistics Co. Ltd.-Ultimate Edition-Developer License"
        BarcodeProfessional.LicenseKey = "8DRV93WC4PYM4SU4HJD9ETG9J9GKRT4HT47GEDW2GVXCY5F6LYCQ"




        '   測試用 待刪↑
        Me.ReportViewer1.RefreshReport()
        Me.ReportViewer2.RefreshReport()
        Me.ReportViewer3.RefreshReport()

        init()
        '   測試用 待刪↓
        'pokey_Text.Text = "00801517465"
        'Txt_SKU.Text = "2681495646003"
        'MonitorStation_API1("9999999999")
        'MonitorStation_API2("9999999999", "26code", "descr", "69")
    End Sub
    Private Sub MonitorStation_API1(ByVal pokey As String)
        If MonitorStation.Substring(0, 1) <> "I" Then Exit Sub
        Try
            ' 標頭,鏡頭編號,使用者編號,客戶名稱,投入口,時間
            Dim WebAPI As WebReference.videocalls
            WebAPI = New WebReference.videocalls()
            WebAPI.SendCommand("E," + MonitorStation, 0)
            If pokey = "" Then Exit Sub
            WebAPI.SendCommand("S," + MonitorStation + "," + username + "," + pokey + "," + "" + "," + Now.Date.ToString("yyyy-MM-dd"), 1)
            'MessageBox.Show(WebAPI.SendCommand("S," + MonitorStation + "," + username + "," + pokey + "," + " " + "," + Now.Date.ToString, 1))
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub MonitorStation_API1(ByVal pokey As String, ByVal str As String)
        If MonitorStation.Substring(0, 1) <> "I" Then Exit Sub
        Try
            ' 標頭,鏡頭編號,使用者編號,客戶名稱,投入口,時間
            Dim WebAPI As WebReference.videocalls
            WebAPI = New WebReference.videocalls()
            WebAPI.SendCommand("E," + MonitorStation, 0)
            If pokey = "" Then Exit Sub
            WebAPI.SendCommand("S," + MonitorStation + "," + username + "," + pokey + "," + str + "," + Now.Date.ToString("yyyy-MM-dd"), 1)
            'MessageBox.Show(WebAPI.SendCommand("S," + MonitorStation + "," + username + "," + pokey + "," + " " + "," + Now.Date.ToString, 1))
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub MonitorStation_API2(ByVal pokey As String, ByVal SKU As String, ByVal isbn As String, ByVal DESCR As String, ByVal num As String)
        If MonitorStation.Substring(0, 1) <> "I" Then Exit Sub
        Try
            ' 標頭,鏡頭編號,使用者編號,店號,投入口,時間
            Dim WebAPI As WebReference.videocalls
            WebAPI = New WebReference.videocalls()
            MsgBox(WebAPI.SendCommand("M," + MonitorStation + "," + pokey + "," + SKU + "," + isbn + "," + DESCR + "," + num, 1))
            'MessageBox.Show(WebAPI.SendCommand("S," + MonitorStation + "," + username + "," + pokey + "," + " " + "," + Now.Date.ToString, 1))
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub pokey_Text_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles pokey_Text.KeyPress
        Try
            If Asc(e.KeyChar) = 13 Then
                If pokey_Text.Text.Length < 8 Then
                    MsgBox("單據號碼長度不正確，請確認單據輸入是否正確。")
                    init()
                    Exit Sub
                Else
                    Exkey = pokey_Text.Text.ToString.Trim

                    Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_UpdateCheck("sa", "EX", Exkey, msg)
                    If msg.Equals("1") Then
                        poInfo_lb.Text = ""
                    ElseIf msg.Equals("-1") Then
                        init()
                        poInfo_lb.Text = "資料未上傳，請稍後作業"
                        Exit Sub
                    ElseIf msg.Equals("-3") Then
                        init()
                        poInfo_lb.Text = "密盆綁定多號，請通知資訊人員"
                        Exit Sub
                    ElseIf msg.Equals("-2") Then
                        poInfo_lb.Text = "資料上傳逾時，請跟下一批出貨"
                    Else
                        poInfo_lb.Text = ""
                    End If
                    setPickDetailData()

                End If
            End If
        Catch ex As InvalidOperationException
            System.Console.Beep(500, 200)
            MsgBox("查無此單數據。")
            init()
            'DGV_PickDetail.DataSource = Nothing
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub print_Addr_invoice(ByVal exkey As String, ByVal caseid As String, ByVal route As String)

        '列印遞紙條
        'Me.Vw_AddressTableAdapter.FillTOP1ByCaseID(PRODDataSet.Vw_Address, caseid)

        Me.Vw_Case_Addr_InvoiceTableAdapter.FillByExkey(PRODDataSet.Vw_Case_Addr_Invoice, exkey)

        If PRODDataSet.Vw_Case_Addr_Invoice.Count = 0 Then
            MsgBox("查無此單號")
            Exit Sub
        End If

        If (route = "Z3") Then

            PrintFedexInvice(exkey)
        Else
            '地址條
            AddrReportSet(caseid, route)
            '發票
            InvoiceDataSet()
            '出貨明細
            PrintOrderDetail(exkey)

        End If

        ''列印發票
        'T_GUINO = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.BILLTOKEY = "0000000000" And f.PrintMark = "Y").Select(Function(s) s.GUINO).Distinct

        'For Each guino As String In T_GUINO
        '    PrintInvoice(guino)
        'Next

        ''列印A4發票
        'T_GUINO = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.BILLTOKEY <> "0000000000" And f.PrintMark = "Y").Select(Function(s) s.GUINO).Distinct

        'For Each guino As String In T_GUINO
        '    PrintInvoiceA4(guino)
        'Next

    End Sub

    'PrintDocument1.Print()會呼叫此函式↓
    Private Sub PrintDocument1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
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

    'PrintDocument2.Print()會呼叫此函式↓
    Private Sub PrintDocument2_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument2.PrintPage
        Dim pageImage As New Imaging.Metafile(m_streams(m_currentPageIndex)) '取得資料流
        '啟動版面設定對話方塊
        Dim PageSetupDialog2 As New PageSetupDialog
        PageSetupDialog2.PageSettings = New System.Drawing.Printing.PageSettings
        'PageSetupDialog1.PrinterSettings = New System.Drawing.Printing.PrinterSettings
        PageSetupDialog2.ShowNetwork = False
        '設為預設之左邊界
        PageSetupDialog2.PageSettings.Margins.Left = 0
        '設為預設之右邊界
        PageSetupDialog2.PageSettings.Margins.Right = 0
        '設為預設之上邊界
        PageSetupDialog2.PageSettings.Margins.Top = 0
        '設為預設之下邊界
        PageSetupDialog2.PageSettings.Margins.Bottom = 0

        '設定紙張名稱,寬與高
        'Dim pkCustomSize2 As New System.Drawing.Printing.PaperSize("711", "85", "85")
        '套入格式給PrintDocument
        'PrintDocument2.PrinterSettings.DefaultPageSettings.PaperSize = pkCustomSize2

        'PrintDocument1.DefaultPageSettings.PaperSize = PageSetupDialog1.PageSettings.PaperSize
        'MsgBox(PrintDocument1.PrinterSettings.DefaultPageSettings.ToString)

        e.Graphics.DrawImage(pageImage, e.PageBounds) '繪製(將資料送至印表機)
        m_currentPageIndex += 1
        e.HasMorePages = (m_currentPageIndex < m_streams.Count)

    End Sub

    Private Sub PrintFedexInvice(ByVal exkey As String)

        Dim Tkey, Path As String

        Tkey = ""
        Path = "C:\FEDEX\print\invoice.pdf"

        'Me.PICKDETAILTableAdapter.FillByExkey(PRODDataSet.PICKDETAIL, exkey)
        'Me.PICKDETAILTableAdapter.GetKey("DBE2Z3N", "sa", Tkey)

        'Me.Vw_PICKDETAIL_RateTableAdapter.FillByExkey(PRODDataSet.Vw_PICKDETAIL_Rate, exkey)
        'Me.Vw_PICKDETAIL_RateTableAdapter.GetKey("DBE2Z3N", "sa", Tkey)
        Tkey = PRODDataSet.Vw_Case_Addr_Invoice(0).CASEID
        TotalWeight = PRODDataSet.Vw_Case_Addr_Invoice(0).Weight
        Dim Vw_FedexAI = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.SKU <> "2000000029009")

        Dim rds As New ReportDataSource("DataSet1", Vw_FedexAI.CopyToDataTable)

        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing

        ' Setup the report viewer object and get the array of bytes
        Dim viewer As New ReportViewer()
        viewer.ProcessingMode = ProcessingMode.Local
        '外部目錄
        'viewer.LocalReport.ReportPath = "C:\RDLC\Report_FEDEXinvoice.rdlc"
        '專案目錄
        viewer.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_FEDEXinvoice.rdlc"
        viewer.LocalReport.DataSources.Add(rds)
        Dim params() As ReportParameter = {New ReportParameter("Tkey", Tkey)}
        Dim params1() As ReportParameter = {New ReportParameter("Tax", Tax)}
        viewer.LocalReport.SetParameters(params)
        viewer.LocalReport.SetParameters(params1)
        'viewer.RefreshReport()
        '轉PDF
        Dim bytes As Byte() = viewer.LocalReport.Render("PDF", Nothing, mimeType, encoding, extension, streamids, warnings)
        'save file
        Dim fs As New FileStream(Path, FileMode.Create)
        fs.Write(bytes, 0, bytes.Length)
        fs.Close()
        AutoPrint("A4", viewer.LocalReport)
        setFedexInData(exkey, Tkey, Path)

    End Sub

    Private Sub PrintInvoice(ByVal Guino As String)
        Dim TempPath, FileName, Path As String
        '   檔案暫存位址
        TempPath = "C:\Program Files\eInvoicePrint_EA01\temp\"
        '   等待列印位址
        Path = "C:\Program Files\eInvoicePrint_EA01\print\"
        FileName = "PI_" & Guino & Format(Now, "_yyyyMMddhhmss.TXT")

        Dim objwriter As New System.IO.StreamWriter(TempPath & FileName)
        Dim randomCode As String
        Dim TType As String
        Dim ADDRESS1 As String
        Dim CONTACT1 As String
        Dim KUNDEUINR As String
        Dim TAX01 As String
        Dim total As Integer
        Dim NOTES As String
        total = 0

        'T_GUINODetail = T_PickDetail_Rate.Where(Function(f) f.GUINO = Guino).ToList
        Dim T_GUINODetail = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.GUINO = Guino).ToList

        'total = T_GUINODetail.AsEnumerable().Sum(Function(s) (s.Field(Of Decimal)("QTY") * s.Field(Of Decimal)("PRICE")))
        total = T_GUINODetail.AsEnumerable().Sum(Function(s) (s.QTY * s.PRICE))
        TAX01 = T_GUINODetail.AsEnumerable().Sum(Function(s) s.TAX01)

        TType = T_GUINODetail(0).TaxType
        KUNDEUINR = T_GUINODetail(0).BILLTOKEY
        ADDRESS1 = T_GUINODetail(0).C_ADDRESS1
        CONTACT1 = T_GUINODetail(0).C_CONTACT1
        NOTES = T_GUINODetail(0).NOTES
        randomCode = T_GUINODetail(0).RandomNumber

        objwriter.Write("H1^")
        objwriter.Write(Guino)
        objwriter.Write("^")
        objwriter.Write(Format(Now, "yyyyMMdd")) '要改Guidate 在pickdetail
        objwriter.Write("^")
        objwriter.Write(Format(Now, "HH:mm:ss")) '要改Guidate 在pickdetail
        objwriter.Write("^")
        objwriter.Write(randomCode)
        objwriter.Write("^")
        objwriter.Write("23155531")
        objwriter.Write("^")
        objwriter.Write("誠品股份有限公司")
        objwriter.Write("^")
        objwriter.Write("台北市信義區松德路196號B1,B1-1至B1-6")
        objwriter.Write("^")
        objwriter.Write(KUNDEUINR)
        objwriter.Write("^")
        objwriter.Write(CONTACT1)
        objwriter.Write("^")
        objwriter.Write(ADDRESS1)
        objwriter.Write("^")

        If TType = "1" Then '01應稅
            TType = "T"
            objwriter.Write(total.ToString + "^0^0^")
        ElseIf TType = "3" Then '03免稅
            TType = "O"
            objwriter.Write("0^" + total.ToString + "^0^")
        ElseIf TType = "2" Then '02零稅
            TType = "Z"
            objwriter.Write("0^0^" + total.ToString + "^")
        End If

        objwriter.Write(TAX01)
        objwriter.Write("^")
        objwriter.Write(total.ToString)
        objwriter.Write("^")
        objwriter.Write("BP:G909     TEL:02-66385168")
        objwriter.Write("^D^")
        objwriter.Write("^^^^^")
        objwriter.WriteLine("")

        'Dim fff = T_PickDetail_Rate.AsEnumerable().Where(Function(f) f.Field(Of String)("GUINO") = Guino).ToList

        Dim no = 1
        For Each DD In T_GUINODetail

            total = CInt(DD.PRICE) * CInt(DD.QTY)
            objwriter.WriteLine(
            "D1^" + no.ToString + "^" +
            DD.DESCR.ToString + "^" +
            DD.QTY.ToString + "^" +
            CInt(DD.PRICE).ToString + "^^" +
            total.ToString + "^" +
            TType)
            no = no + 1
        Next

        'For Each DD As Vw_Combination_PickDetail In U_GUINODetail

        '    total = CInt(DD.PRICE) * CInt(DD.QTY)
        '    objwriter.WriteLine(
        '    "D1^" + DD.OrderkeyLineNumber.ToString + "^" +
        '    DD.DESCR.ToString + "^" +
        '    DD.QTY.ToString + "^" +
        '    DD.PRICE.ToString + "^^" +
        '    total.ToString + "^" +
        '    TType)
        'Next

        objwriter.Close()

        '   檔案搬移
        Try
            My.Computer.FileSystem.MoveFile(TempPath & FileName, Path & FileName)
        Catch ex As Exception
            MsgBox(ex.ToString)
            Exit Sub
        End Try

    End Sub

    Private Sub PrintInvoiceA4(ByVal Guino As String)

        'Me.Vw_Combination_PickDetailTableAdapter.FillByGUINO(PRODDataSet.Vw_Combination_PickDetail, Guino)
        'Me.Vw_PICKDETAIL_RateTableAdapter.FillByGUINO(PRODDataSet.Vw_PICKDETAIL_Rate, Guino)
        Dim reportData = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(s) s.GUINO = Guino).ToList

        Me.ReportViewer2.LocalReport.DataSources.Clear()
        Me.ReportViewer2.LocalReport.ReportEmbeddedResource = "Packet_DEV.Report_invoiceA4.rdlc"
        Me.ReportViewer2.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", reportData.CopyToDataTable))

        'Dim Rpt_TASKKEY As New ReportParameter("TASKKEY", Trim(Me.THDataGridView.CurrentRow.Cells(0).Value.ToString()))
        'Me.ReportViewer1.LocalReport.SetParameters(New ReportParameter() {Rpt_TASKKEY})

        Me.ReportViewer2.RefreshReport()

        Application.DoEvents()
        AutoPrint("A4", ReportViewer2)
        m_streams.Clear() '測試時不列印請註解本行------------------------------!!!!!!

    End Sub

    Private Sub PrintOrderDetail(ByVal exkey As String)

        If (PRODDataSet.Vw_Case_Addr_Invoice.Count = 0) Then
            Me.Vw_Case_Addr_InvoiceTableAdapter.FillByExkey(PRODDataSet.Vw_Case_Addr_Invoice, exkey)
        End If
        '設定列印橫式
        'PrintDocument2.DefaultPageSettings.Landscape = True
        Me.ReportViewer3.RefreshReport()

        Application.DoEvents()
        AutoPrint("A4", ReportViewer3)
        m_streams.Clear()

    End Sub



    Private Sub rePack_Btn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles rePack_Btn.Click

        If (pokey_Text.Text.ToString = "" Or DGV_PickDetail.RowCount = 0) Then
            MsgBox("請輸入單號或先查詢")
            Return
        End If

        Me.CASEDETAILTableAdapter.CASEDETAIL_RePick("sa", "CD_RESTART", Exkey, "", msg)
        If msg = 1 Then
            MsgBox("重新打包成功")
            init()
        Else
            MsgBox("重新打包失敗")
        End If

    End Sub

    Private Sub repokey_Text_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles repokey_Text.KeyPress

        If Asc(e.KeyChar) = 13 Then
            If repokey_Text.Text.Length = 0 Then
                MsgBox("請確認單據輸入是否正確。")
                Exit Sub
            Else

                Me.Vw_Case_Addr_InvoiceTableAdapter.FillByExkey(PRODDataSet.Vw_Case_Addr_Invoice, repokey_Text.Text.ToString)
                If PRODDataSet.Vw_Case_Addr_Invoice.Count = 0 Then
                    MsgBox("查無此單號")
                    Exit Sub
                End If

                ROUTE = PRODDataSet.Vw_Case_Addr_Invoice.First.ROUTE.ToString

                'T_PickDetail_Rate.AsEnumerable.Where(Function(f) f.KUNDEUINR = "0000000000" And f.PrintMark = "Y").Select(Function(s) s.GUINO).Distinct
                'Dim reAddrData As List(Of PRODDataSet.Vw_Case_Addr_InvoiceRow)

                'Dim DGVaddrdata = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.STATUS <= 13 And f.STATUS > 0).Select(Function(f) New With {.CASEID = f.CASEID, .C_CONTACT1 = f.C_CONTACT1, .C_ADDRESS1 = f.C_ADDRESS1})
                Dim DGVaddrdata = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.STATUS > 0).Select(Function(f) New With {.CASEID = f.CASEID, .提貨人 = f.C_CONTACT1, .地址 = f.C_ADDRESS1})
                Dim DGVinvoicedata = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.STATUS <= 13 And f.STATUS > 0).Select(Function(f) New With {.GUINO = f.GUINO})

                DGV_reAddr.DataSource = DGVaddrdata.GroupBy(Function(f) f.CASEID).Select(Function(f) f.First).ToList
                DGV_reInvoice.DataSource = DGVinvoicedata.GroupBy(Function(f) f.GUINO).Select(Function(f) f.First).ToList
                'MsgBox(reAddrData(0))
                Me.ReportViewer3.RefreshReport()

            End If
        End If

    End Sub

    Private Sub ReportViewer2_Load(ByVal sender As Object, ByVal e As EventArgs) Handles ReportViewer2.Load

    End Sub

    Private Sub setFedexInData(ByVal exkey As String, ByVal Tkey As String, ByVal invicePath As String)

        Dim TempPath, FileName, Path As String

        'Me.PICKDETAILTableAdapter.GetKey("DBE2Z3N", "sa", Tkey)

        '   檔案暫存位址
        TempPath = "C:\FEDEX\temp\"
        '   等待列印位址
        Path = "C:\FEDEX\"
        FileName = Tkey & "_" & exkey & ".in"

        'U_GUINODetail = U_PickDetail.First(Function(s) s.Externorderkey = exkey)
        'U_PickDetail = PRODContext.Vw_Combination_PickDetail.Where(Function(s) s.Externorderkey = exkey)
        'T_PickDetail_Rate = Me.Vw_PICKDETAIL_RateTableAdapter.GetDataByExkey(exkey).ToList
        'Dim PickDetail_Rate = Vw_Case_Addr_InvoiceTableAdapter.GetDataByExkey(exkey).ToList
        Dim T_CaseAI = PRODDataSet.Vw_Case_Addr_Invoice.Where(Function(f) f.SKU <> "2000000029009").ToList
        Dim total As Double '= T_CaseAI.Sum(Function(s) Math.Round((s.QTY * s.PRICE_RATE) / Tax))
        total = T_CaseAI.Sum(Function(s) (s.QTY * s.PRICE_RATE) / Tax)
        total = Math.Round(total, 2)

        Dim objwriter As New System.IO.StreamWriter(TempPath & FileName)

        Dim dfdf = T_CaseAI(0).C_ZIP
        objwriter.WriteLine("0,""20""")
        objwriter.WriteLine("1222,""" + Tkey + """") '提單號
        objwriter.WriteLine("25,""" + exkey + """")   '誠品訂單號
        objwriter.WriteLine("12,""" + T_CaseAI(0).C_CONTACT1 + """")   '收件人
        objwriter.WriteLine("13,""" + T_CaseAI(0).C_ADDRESS1 + """")   '收件地址1
        objwriter.WriteLine("14,""""")   '收件地址2
        objwriter.WriteLine("11,""""")
        objwriter.WriteLine("15,""" + T_CaseAI(0).C_CITY + """")   '收件城市
        objwriter.WriteLine("50,""" + T_CaseAI(0).C_COUNTRY + """") '收件國家
        objwriter.WriteLine("17,""" + T_CaseAI(0).C_ZIP + """")   '郵遞區號
        objwriter.WriteLine("18,""" + T_CaseAI(0).C_PHONE1 + """")   '收件人電話
        objwriter.WriteLine("1202,""""") '收件人EMAIL
        objwriter.WriteLine("1274,""3""")
        objwriter.WriteLine("1273,""1""")
        objwriter.WriteLine("68,""USD""")
        objwriter.WriteLine("23,""1""")
        objwriter.WriteLine("20,""184835673""") '誠品的FEDEX 帳號
        objwriter.WriteLine("70,""2""")
        objwriter.WriteLine("71,""""")
        objwriter.WriteLine("116,""1""")
        objwriter.WriteLine("112,""" + TotalWeight + """") '重量
        'TODO 價錢除以美金 
        objwriter.WriteLine("119,""" + total.ToString + """") '總價格
        objwriter.WriteLine("1,""" + T_CaseAI(0).SALESPERSON + """") '追蹤ID
        objwriter.WriteLine("75,""KGS""")
        objwriter.WriteLine("79-1,""Books.""")
        objwriter.WriteLine("80-1,""TW""")
        '上傳FEDEX------
        'objwriter.WriteLine("2806,""Y""")
        'objwriter.WriteLine("2818,""1""")
        'objwriter.WriteLine("2819,""" + invicePath + """")
        '---------------
        objwriter.WriteLine("99,""""")

        objwriter.Close()

        '   檔案搬移
        Try
            My.Computer.FileSystem.MoveFile(TempPath & FileName, Path & FileName)
        Catch ex As Exception
            MsgBox(ex.ToString)
            Exit Sub
        End Try

    End Sub

    '<20210426>蝦皮單重送單補確認 !
    Private Sub setPickDetailData()

        DGV_PickDetail.DataSource = Nothing
        DGV_CaseDetail.DataSource = Nothing

        T_PickDetail_Rate = Me.Vw_PICKDETAIL_RateTableAdapter.GetDataByExkey(Exkey).ToList
        Dim vCaseID As String = T_PickDetail_Rate.First.DeliveryKey.ToString
        Dim shopee As String = T_PickDetail_Rate.First.STORERKEY.ToString
        If vCaseID = "" And shopee = "G016" Then
            MsgBox("蝦皮單-配送號未取!請再刷1次單號。")
            shopee += T_PickDetail_Rate.First.ROUTE.ToString
            vCaseID = SqlPKPas.Getkey(shopee, Exkey)
            If vCaseID <> "" Then
                MsgBox("蝦皮單-配送號:" & vCaseID)
            End If
            'SqlPKPas.ExcuteSql("EXEC PROD..GetKey '" + shopee + ROUTE + "','" + Exkey + "','';")
            init()
            Exit Sub
        End If

        If (Me.T_PickDetail_Rate.Select(Function(s) s.STATUS).Min = 9) Then
            MsgBox("此單已SHIP。")
            init()
            Exit Sub
        End If

        '   總品項數
        Dim pickdetailItemCount = T_PickDetail_Rate.Count()
        '   總PCS數
        Dim PackqtyTotal_Pickdetail = T_PickDetail_Rate.Sum(Function(s) s.QTY)
        '   總已驗數
        actQty = T_PickDetail_Rate.Sum(Function(s) s.PACKQTY)

        Label10.Text = pickdetailItemCount
        Label11.Text = PackqtyTotal_Pickdetail
        Label12.Text = actQty
        'DGV_PickDetail.DataSource = T_PickDetail_Rate.CopyToDataTable
        VwPICKDETAILRateBindingSource.DataSource = T_PickDetail_Rate.CopyToDataTable
        Dim str As String = T_PickDetail_Rate.Item(0).C_CONTACT1.ToString
        MonitorStation_API1(Exkey, str)
        DGV_PickDetail.DataSource = VwPICKDETAILRateBindingSource

        Me.Txt_SKU.Enabled = True
        Me.Txt_SKU.Text = ""
        Me.Txt_SKU.Focus()
        Me.LAB_CASEID.Text = ""
        CASEID = ""

        TabControl2.SelectedIndex = 0
        '   查詢單筆欄位
        'Dim londonCusts = From cust In PD.Combination_PickDetail
        '                  Where cust.Exter = "5160011660"
        '                  Select New Info With {.Pcs = cust.Pcs}

    End Sub

    Private Sub Txt_SKU_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Txt_SKU.KeyPress, Txt_Pcs.KeyPress
        '   ENTER後將Txt_SKU.Text的商品號碼與DGV_PickDetail中相同商品號碼的資訊寫入DGV_CaseDetail，數量依照Txt_Pcs.Text填入。
        Try
            If Asc(e.KeyChar) = 13 Then
                'If Txt_Ext.Text.Length < 10 Then
                '    MsgBox("單據號碼不足十碼、請確認單據輸入是否正確。")
                '    Exit Sub
                'End If

                If (Txt_SKU.Text = "" Or Txt_SKU.Enabled = False) Then
                    For i As Integer = 0 To 2
                        System.Console.Beep(500, 200)
                        System.Console.Beep(700, 200)
                    Next i
                    MsgBox("無此資料")
                    'Me.Txt_SKU.Text = ""
                    Me.Txt_SKU.Focus()
                    Me.Txt_SKU.SelectAll()
                    Exit Sub
                End If

                If T_PickDetail_Rate.Where(Function(s) (s.SKU.Trim = Txt_SKU.Text.ToString.Trim Or s.BUSR2.Trim = Txt_SKU.Text.ToString.Trim Or s.BUSR3.Trim = Txt_SKU.Text.ToString.Trim)).Count = 0 Then
                    For i As Integer = 0 To 2
                        System.Console.Beep(500, 200)
                        System.Console.Beep(700, 200)
                    Next i
                    MsgBox("本單無此商品")
                    Me.Txt_SKU.Focus()
                    Me.Txt_SKU.SelectAll()
                    Exit Sub
                End If

                '   單據明細清單
                T_SKUDetail = T_PickDetail_Rate.First(Function(s) (s.SKU.Trim = Txt_SKU.Text.ToString.Trim Or s.BUSR2.Trim = Txt_SKU.Text.ToString.Trim Or s.BUSR3.Trim = Txt_SKU.Text.ToString.Trim) And (s.PACKQTY < s.PICKQTY))
                ROUTE = T_SKUDetail.ROUTE

                If T_SKUDetail.PACKQTY + CInt(Txt_Pcs.Text) > T_SKUDetail.PICKQTY Then
                    For i As Integer = 0 To 2
                        System.Console.Beep(500, 200)
                        System.Console.Beep(700, 200)
                    Next i
                    MsgBox("數量已超過")
                    'Me.Txt_SKU.Text = ""
                    Me.Txt_SKU.Focus()
                    Me.Txt_SKU.SelectAll()
                    Exit Sub
                End If

                If (CASEID = "") Then
                    CASEID = T_SKUDetail.DeliveryKey
                    Me.LAB_CASEID.Text = CASEID
                End If

                ' 建立caseRow
                Dim caseRow As DataRow = PRODDataSet.Combination_CaseDetail.NewRow()

                caseRow("Storerkey") = T_SKUDetail.STORERKEY
                caseRow("Orderkey") = T_SKUDetail.ORDERKEY
                caseRow("OrderkeyLineNumber") = T_SKUDetail.ORDERLINENUMBER
                caseRow("Externorderkey") = T_SKUDetail.EXTERNORDERKEY
                caseRow("ROUTE") = T_SKUDetail.ROUTE
                caseRow("DOOR") = T_SKUDetail.DOOR
                caseRow("DESCR") = T_SKUDetail.DESCR
                caseRow("Sku") = T_SKUDetail.SKU
                caseRow("LOC") = T_SKUDetail.LOC
                caseRow("QTY") = Txt_Pcs.Text
                caseRow("GUINO") = T_SKUDetail.GUINO
                caseRow("PACKKEY") = CASEID
                caseRow("PICKDETAILKEY") = T_SKUDetail.PICKDETAILKEY
                caseRow("EDITDATE") = Now()
                caseRow("CURCY") = T_SKUDetail.CURCY
                caseRow("EDITWHO") = Environment.MachineName
                caseRow("TaxType") = T_SKUDetail.TaxType
                caseRow("TAX01") = T_SKUDetail.TAX01
                caseRow("UNITPRICE") = T_SKUDetail.UNITPRICE
                caseRow("PRICE") = T_SKUDetail.PRICE
                caseRow("Amount") = T_SKUDetail.Amount
                caseRow("RandomNumber") = T_SKUDetail.RandomNumber
                caseRow("WAVEKEY") = T_SKUDetail.WAVEKEY
                MonitorStation_API2(T_SKUDetail.EXTERNORDERKEY, T_SKUDetail.BUSR2, T_SKUDetail.DESCR, Txt_Pcs.Text, "")
                'caseRow("C_CONTACT1") = T_SKUDetail.KUNDEUINR
                'caseRow("C_CONTACT1") = T_SKUDetail.TAX01
                'caseRow("C_CONTACT1") = T_SKUDetail.PRICE
                'caseRow("C_CONTACT1") = T_SKUDetail.UNITPRICE
                Me.PRODDataSet.Combination_CaseDetail.Rows.Add(caseRow)

                '   將資料列插入DataContext中的DataTable()
                'PRODContext.Combination_CaseDetail.Add(U_CaseDetil)
                '   更新數量
                T_SKUDetail.PACKQTY = T_SKUDetail.PACKQTY + CInt(Txt_Pcs.Text)
                actQty = actQty + CInt(Txt_Pcs.Text)
                Label12.Text = actQty

                '   設定DGV_PickDetail位置
                'Dim pos As Integer = T_PickDetail_Rate.FindIndex(Function(s) s.SKU = T_SKUDetail.SKU And s.LOC = T_SKUDetail.LOC And s.ORDERLINENUMBER = T_SKUDetail.ORDERLINENUMBER)
                Dim pos As Integer = VwPICKDETAILRateBindingSource.Find("PICKDETAILKEY", T_SKUDetail.PICKDETAILKEY.ToString)
                DGV_PickDetail.Rows(pos).Cells("PACKQTYDataGridViewTextBoxColumn").Value = T_SKUDetail.PACKQTY

                If T_SKUDetail.PACKQTY.Equals(T_SKUDetail.PICKQTY) Then
                    'T_SKUDetail.Item("STATUS") = 9
                    Me.Vw_PICKDETAIL_RateTableAdapter.UpdatePICKDETAIL(T_SKUDetail.PACKQTY, 5, T_SKUDetail.EXTERNORDERKEY, T_SKUDetail.SKU, T_SKUDetail.LOC, T_SKUDetail.ORDERLINENUMBER)
                    DGV_PickDetail.Rows(pos).Cells("STATUSDataGridViewTextBoxColumn").Value = 5
                Else
                    Me.Vw_PICKDETAIL_RateTableAdapter.UpdatePICKDETAIL(T_SKUDetail.PACKQTY, T_SKUDetail.STATUS, T_SKUDetail.EXTERNORDERKEY, T_SKUDetail.SKU, T_SKUDetail.LOC, T_SKUDetail.ORDERLINENUMBER)
                    DGV_PickDetail.Rows(pos).Cells("STATUSDataGridViewTextBoxColumn").Value = T_SKUDetail.STATUS
                End If

                DGV_PickDetail.CurrentCell = DGV_PickDetail.Rows(pos).Cells(0)
                DGV_PickDetail.Rows(pos).Selected = True

                '   確定寫入Combination_CaseDetail資料庫。

                'PRODContext.SaveChanges()
                Me.Combination_CaseDetailTableAdapter.Update(caseRow)
                '顯示U_CaseDetail內容至DGV_CaseDetail
                DGV_CaseDetail.DataSource = Me.Combination_CaseDetailTableAdapter.GetDataByExkey(T_SKUDetail.EXTERNORDERKEY)
                'TabControl2.SelectedIndex = 1
                'Txt_SKU.Text = ""
                'End If

                T_PickDetail_Rate = Me.Vw_PICKDETAIL_RateTableAdapter.GetDataByExkey(Exkey).ToList

                If (Me.T_PickDetail_Rate.Select(Function(s) s.STATUS).Min = 5) Then
                    '整箱完成印地址條
                    If MsgBox("整單完成", vbYesNo) = MsgBoxResult.Yes Then
                        MonitorStation_API1("")
                        If (ROUTE = "Z9" Or ROUTE = "Z3") Then

                            Dim f2 As New Form1
                            f2.f1 = Me
                            f2.wpokey_Lb.Text = Exkey
                            f2.wcaseID_Lb.Text = LAB_CASEID.Text.ToString
                            f2.route = ROUTE
                            f2.complete = "Y"
                            f2.Txt_Weight.Clear()
                            f2.packmod = ""
                            f2.Cb_packmod.Text = ""
                            f2.ShowDialog()
                            allprint()
                        Else
                            allprint()
                        End If
                    Else
                        '重新打包
                        rePack_Btn.PerformClick()
                    End If

                End If

                'Me.Txt_SKU.Text = ""
                Me.Txt_SKU.Focus()
                Me.Txt_SKU.SelectAll()

            End If
        Catch ex As InvalidOperationException
            For i As Integer = 0 To 2
                System.Console.Beep(500, 200)
                System.Console.Beep(700, 200)
            Next i
            MsgBox("商品已經滿足。")
            'Me.Txt_SKU.Text = ""
            Me.Txt_SKU.Focus()
            Me.Txt_SKU.SelectAll()
        Catch ex As Exception
            Throw
        End Try

        'Dim Cust As New Combination_CaseDetail With _
        '    {
        '        .Descr = "1",
        '        .PKNO=
        '    }

        '將資料列插入DataContext中的DataTable()
        'PD.Combination_CaseDetail.InsertOnSubmit(Cust)

        'Try
        '    '   確定寫入資料庫。
        '    PD.SubmitChanges()
        'Catch ex As Exception
        '    MsgBox(ex.ToString)
        'End Try

    End Sub

    Private Sub Txt_SKU_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Txt_SKU.TextChanged
        '   刷入商品號碼。
        'If Txt_SKU.Text.Length > 0 Then
        '    Btn_Close.Enabled = False
        'Else
        '    Btn_Close.Enabled = True
        'End If
    End Sub

    Private Sub UploadCase_txt_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles UploadCase_txt.KeyPress
        If Asc(e.KeyChar) = 13 Then
            If UploadCase_txt.Text.Length = 0 Then
                MsgBox("請確認輸入是否正確。")
                Exit Sub
            Else
                Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_UpdateCheck("sa", "CASE", UploadCase_txt.Text.ToString.Trim, msg)
                If msg.Equals("1") Then
                    MsgBox("已上傳可以出貨")
                ElseIf msg.Equals("-1") Then
                    MsgBox("資料未上傳，出貨前請再查詢")
                ElseIf msg.Equals("-2") Then
                    MsgBox("資料上傳逾時，請跟下一批出貨")
                Else
                    MsgBox("查無資料或非Z7單據")
                End If

            End If
        End If
    End Sub

    Private Sub UploadPo_txt_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles UploadPo_txt.KeyPress

        If Asc(e.KeyChar) = 13 Then
            If UploadPo_txt.Text.Length = 0 Then
                MsgBox("請確認輸入是否正確。")
                Exit Sub
            Else
                Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_UpdateCheck("sa", "EX", UploadPo_txt.Text.ToString.Trim, msg)
                If msg.Equals("1") Then
                    MsgBox("已上傳可以出貨")
                ElseIf msg.Equals("-1") Then
                    MsgBox("資料未上傳，出貨前請再查詢")
                ElseIf msg.Equals("-2") Then
                    MsgBox("資料上傳逾時，請跟下一批出貨")
                Else
                    MsgBox("查無資料或非Z7單據")
                End If

            End If
        End If

    End Sub

    Private Sub wavekey_Text_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles wavekey_Text.KeyPress
        If Asc(e.KeyChar) = 13 Then
            If wavekey_Text.Text.Length = 0 Then
                MsgBox("請確認輸入是否正確。")
                Exit Sub
            Else
                Me.Vw_Case_Addr_InvoiceTableAdapter.FillByWAVEKEY(PRODDataSet.Vw_Case_Addr_Invoice, wavekey_Text.Text.ToString)
                If PRODDataSet.Vw_Case_Addr_Invoice.Count = 0 Then
                    MsgBox("查無資料")
                    Exit Sub
                End If

                ROUTE = PRODDataSet.Vw_Case_Addr_Invoice.First.ROUTE.ToString
                Dim DGVWAVEdata = PRODDataSet.Vw_Case_Addr_Invoice.Select(Function(f) New With {.單號 = f.EXTERNORDERKEY, .CASEID = f.CASEID, .路線 = f.ROUTE, .提貨人 = f.C_CONTACT1, .地址 = f.C_ADDRESS1, .發運時間 = f.SHIPDATE}).GroupBy(Function(f) f.CASEID)
                DGV_WAVE.DataSource = DGVWAVEdata.Select(Function(f) f.First).OrderBy(Function(f) f.單號).ToList

                If MsgBox("是否列印" + CStr(DGVWAVEdata.Count) + "單？", vbYesNo) = MsgBoxResult.Yes Then
                    '列印
                    For i As Integer = 0 To DGVWAVEdata.Count - 1
                        AddrReportSet(DGV_WAVE.Rows(i).Cells("CASEID").Value, ROUTE)
                        'MsgBox(DGV_WAVE.Rows(i).Cells("CASEID").Value)
                    Next
                Else
                    wavekey_Text.Text = ""
                    'DGV_WAVE.DataSource = Nothing
                End If

            End If
        End If
    End Sub

    Private Sub weight(ByVal exkey As String)

        Dim f2 As New Form1

        f2.f1 = Me

        f2.wpokey_Lb.Text = exkey
        f2.ShowDialog()

    End Sub
    Private Function CVSAsc43(ByVal odkey As String, ByVal StoreType2 As String) As String
        odkey = StoreType2 + odkey.PadLeft(11, "0")
        Dim Vsum As Integer = 0
        ' The following expression was wrapped in a checked-statement
        Dim num As Integer = odkey.ToString().Length - 1
        For pos As Integer = 1 To num
            ' The following expression was wrapped in a unchecked-expression
            Vsum += odkey.Substring(pos, 1)
        Next
        Dim oik As Integer = Vsum Mod 43
        Dim num2 As Integer = oik
        If num2 < 10 Then
            odkey += Chr(oik + 48)
        Else
            If num2 >= 10 AndAlso num2 <= 35 Then
                odkey += Chr(oik + 55)
            Else
                If num2 = 36 Then odkey += "-"
                If num2 = 37 Then odkey += "."
                If num2 = 38 Then odkey += " "
                If num2 = 38 Then odkey += "$"
                If num2 = 40 Then odkey += "/"
                If num2 = 41 Then odkey += "+"
                If num2 = 42 Then odkey += "%"
            End If
        End If
        Return odkey.Substring(0, 13)
    End Function

    Private Sub CheckBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.Click
        'TODO 20210121 Raines 新增密碼鎖定
        If CheckBox1.Checked = False Then
            Dim password As String = InputBox("Please Input your Password")
            Dim TMD As String = (Date.Today.Year + Date.Today.Month + Date.Today.Day).ToString
            If password <> TMD Then MsgBox("輸入錯誤")
            If password <> TMD Then CheckBox1.Checked = True
            If password <> TMD Then Exit Sub
            If password = TMD Then CheckBox1.Checked = False
        End If


        If CheckBox1.Checked = True Then
            Txt_Pcs.Text = 1
            Txt_Pcs.Enabled = False
        Else
            Txt_Pcs.Text = 2
            Txt_Pcs.Enabled = True

        End If
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            vs.com.Readini.INIReader.setValue(AppPatch.ToString + "\Packet.ini", "ALL", "MonitorStation", ComboBox1.Text.ToString.Trim)
            vs.com.Readini.INIReader.setValue(AppPatch.ToString + "\Packet.ini", "ALL", "MonitorStationUser", ComboBox2.Text.ToString.Trim)
            MsgBox("設置成功")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub


    '====================================================
    ''  庫轉修改 20210430 
    ' 選擇 單據作業/AGV
    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged, RadioButton1.CheckedChanged, RadioButton3.CheckedChanged
        Select Case CType(sender, RadioButton).Checked
            Case Me.RadioButton1.Checked        '單據 224, 224, 224
                pokey_Text.Enabled = True
                AGV_Txt.Enabled = False
                AGV_Txt.BackColor = Color.FromArgb(224, 224, 224)
                pokey_Text.BackColor = Color.FromArgb(255, 224, 192)
            Case Me.RadioButton2.Checked
                pokey_Text.Enabled = False
                AGV_Txt.Enabled = True
                AGV_Txt.BackColor = Color.FromArgb(255, 224, 192)
                pokey_Text.BackColor = Color.FromArgb(224, 224, 224)
            Case Me.RadioButton2.Checked
                Frm_pkPrint.Show()
                Me.Close()
        End Select
        'If RadioButton2.Checked = False Then
        '    pokey_Text.Enabled = False
        '    Txt_Pcs.Enabled = False
        '    Txt_SKU.Enabled = False
        '    CheckBox1.Enabled = False
        'Else
        '    pokey_Text.Enabled = True
        '    Txt_Pcs.Enabled = True
        '    Txt_SKU.Enabled = True
        '    CheckBox1.Enabled = True
        'End If
    End Sub
                               
    ''' 庫轉修改 20210430 
 
    Private Sub AGV_Txt_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles AGV_Txt.KeyPress
        If Asc(e.KeyChar) = 13 Then
            AGV_Box = AGV_Txt.Text.ToString.Trim
            'QTmp = SqlPKPas.sqlTable("select AGV_Box,EXTERNORDERKEY,SUSR from ORDERS where AGV_Box='" + AGV_Txt.Text + "'")
            'If QTmp Is Nothing And QTmp.Rows.Count = 1 Then
            '    AGV_Txt.Text = QTmp.Rows(0)("EXTERNORDERKEY").ToString
            'Else
 
            AGV_Txt.Text = ""
            Me.Vw_PICKDETAIL_RateTableAdapter.CASEDETAIL_UpdateCheck("Packapp", "AGV_Ck", AGV_Box, msg)

            If msg Like "1:*" Then
                poInfo_lb.Text = ""
                pokey_Text.Text = msg.Split(":")(1) + Chr(13)
                Call pokey_Text_KeyPress(Me, e)
            ElseIf msg Like "-#:*" Then
                poInfo_lb.Text = msg.Split(":")(1)
                'poInfo_lb.Text = "密盆綁定多號，請通知資訊人員"
                Exit Sub
                'ElseIf msg Like "0:*" Then
                '    poInfo_lb.Text = msg.Split(":")(1)
                '    Exit Sub
            Else
                poInfo_lb.Text = ""
            End If
 
        End If
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If repokey_Text.Text.Trim = "" Then Exit Sub
        PrintOrderDetail(repokey_Text.Text)
    End Sub
    '====================================================
 
End Class