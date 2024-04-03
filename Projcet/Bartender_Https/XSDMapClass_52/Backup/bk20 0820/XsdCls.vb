Option Strict Off
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.IO
Imports System.Text
 
Imports Microsoft.VisualBasic.FileIO
Imports System.Xml
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Linq
Imports <xmlns:xsd="http://www.w3.org/2001/XMLSchema">
Imports <xmlns:xs="http://www.w3.org/2001/XMLSchema">
Imports <xmlns:sql="urn:schemas-microsoft-com:mapping-schema">

'===========================================================================
' XsdMaping 類型 : 
'  xsd檔取得 的參數及函式
' XsdMaping 類型 : 需要的欄位 ，EDIInf 轉介的程式方式
'===========================================================================


Public Class XsdCls
	Implements IDisposable
	' NEW初期再傳入值  
	Private _InfKey As String
	Private _StKey As String
	Private _XsdFile As String
	Private _wkey As String
	'========= XSD 共用參數 

	Public INFType As String
	Public SQLBulkCk As String	''  SQLBulkin
	Public EDLLocDr As String
	Public BuName As String
	Public FileType As String
	Public Fcoding As String
	Public FieldParser As String
	Public SearchStr As String
	Private FilenameFmt As String
	Public CommandText As String
	Private CheckFun As String
	Public Extkeys() As String

	Public ODMTyps As String
	Public JOBQTyps As String
	' 取消Stkey, InfKey,
	Public XDoc As XDocument = New XDocument()
	Public XApi As IEnumerable(Of XElement)
	Public impsFile As String
	Public Mds As New DataSet
	Public MapDs As New DataSet
	Public MapInf As IEnumerable(Of MapingBox)
	Public Property ClStatus() As String = ""     ' 狀態說明(open:GET,SET)


	Public Rtcode As String    ' 後期讀取數量 
	Public Tscount As Long      ' 後期目SQL DB 或檔案的 imp處理數量 

	Dim colid As Integer = 0
	Public APPLog As String = DBConCls.APPLog

#Region "New 屬性GET SET"
	Public Property InfKey() As String
		Get
			Return _InfKey
		End Get
		Set(ByVal value As String)
			_InfKey = value
		End Set
	End Property
	Public Property StKey() As String
		Get
			Return _StKey
		End Get
		Set(ByVal value As String)
			_StKey = value
		End Set
	End Property
	Public Property XsdFile() As String
		Get
			Return _XsdFile
		End Get
		Set(ByVal value As String)
			_XsdFile = value
		End Set
	End Property
	Public Property wkey() As String
		Get
			Return _wkey
		End Get
		Set(ByVal value As String)
			_wkey = value
		End Set
	End Property

#End Region
#Region "類別建構式"

	Public Sub New()
	End Sub
	'OverLoading建構子
	Public Sub New(ByVal iInf As String, ByVal iwkey As String, ByVal iStkey As String, ByVal iXsdFile As String)
		'==驗證 參數
		Extkeys = iStkey.Split(New Char() {","})
		_StKey = Extkeys(0)
		_InfKey = iInf
		_wkey = iwkey
		If FileIO.FileSystem.FileExists(iXsdFile) = True Then
			_XsdFile = iXsdFile
		Else
			ClStatus = "<sql : xsd 不存在!>" & iXsdFile
			Exit Sub
		End If
		ClStatus = "XSD 設定讀取....."
		'====取得XSD結構表  >設到共用變數 xsd:appinf 參數
		Try
			XDoc = XDocument.Load(XsdFile)
			XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el
			'' <CONNs>匯入匯出 SERVER   : SQL連線 如未填 用預設中間庫位置 ；SQLBulkCk: 目的資料表
			If XApi.<CONNs>.FirstOrDefault.Value.ToString.TrimEnd(" ") <> "" Then
				DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
				DBConCls.CONNs = DBConCls.EDIconstr
			End If
			' <EDIinf> 接口匯出或匯入 (類型)  =Import/Expport/TABLEMERGE/
			INFType = XApi.<EDIinf>.@INFType
			' 匯出來源或匯入檔案位置
			EDLLocDr = XApi.<EDIinf>.@EDLLocDr
			If iwkey.IndexOf("\") = -1 Then
				impsFile = EDLLocDr + iwkey
			Else
				impsFile = iwkey
			End If

			'' 檔案內容格式
			Fcoding = IIf(XApi.<EDIinf>.@Encoding Is Nothing, XApi.<FileMap>.@Encoding, XApi.<EDIinf>.@Encoding)
			' 匯出來源表或匯入表的    content確認
			SQLBulkCk = IIf(XApi.<EDIinf>.@SQLBulkCk Is Nothing, XApi.<EDIinf>.@SQLBulkin, XApi.<EDIinf>.@SQLBulkCk)
			BuName = XApi.<EDIinf>.@BuName
			FilenameFmt = XApi.<FileMap>.@FilenameFmt
			'' FileType:   舊版設定在 EDIinf 新版分拆 FileMap
			''txt ; xml；json 匯入/ 匯出 檔案格式 及 區隔符號或FixedWidth   
			FileType = IIf(XApi.<EDIinf>.@FileType Is Nothing, XApi.<FileMap>.@FileType, XApi.<EDIinf>.@FileType)
			FieldParser = XApi.<FileMap>.@FieldParser
			SearchStr = IIf(XApi.<FileMap>.@SearchStr Is Nothing, "0", XApi.<FileMap>.@SearchStr)

			CommandText = XApi.<CommandText>.Value
			CheckFun = XApi.<CheckFun>.Value
			' 是否需要維護中間庫: 訂單、主檔的內容
			If XApi.<ODMTyps>.Any = True Then ODMTyps = XApi.<ODMTyps>.FirstOrDefault.Value
			' 是否需要維護EDI庫:  rdo、 
			If XApi.<JOBQTyps>.Any = True Then JOBQTyps = XApi.<JOBQTyps>.FirstOrDefault.Value
			'=================參數值檢查

			If INFType = "" Or FileType = "" Or EDLLocDr = "" Then
				ClStatus += XsdFile + "<XSD設定 appinfo.EDIinf: 設定未完成> INFType/FileType/EDLLocDr 必要值! "
				Exit Sub
			End If
			' 匯入匯入類型 FileType 必要值檢查
			Select Case INFType.ToLower
				Case "import"
					If FileType = "" OrElse FieldParser = "" OrElse SQLBulkCk = "" Then
						ClStatus += XsdFile + "<XSD設定 appinfo :  Import設定未完成>FileType/FieldParser /SQLBulkCk必要值! "
					End If
				Case "export"
					If FileType = "" OrElse FieldParser = "" OrElse CommandText = "" OrElse FilenameFmt = "" Then
						ClStatus += XsdFile + "<XSD設定 appinfo :  Expport設定未完成>FileType/FieldParser /CommandText 必要值! "
					End If
					If Fcoding = "" OrElse FilenameFmt = "" Then
						ClStatus += XsdFile + "<XSD設定 appinfo :  Expport設定未完成>Fcoding/FilenameFmt 必要值! "
					End If
				Case "tablemerge"
					If SQLBulkCk = "" Or CommandText = "" Then
						ClStatus += XsdFile + "<XSD設定 appinfo :  TABLEMERGE設定未完成> "
					End If
				Case "dbtodb"
					If SQLBulkCk = "" Or CommandText = "" Then
						ClStatus += XsdFile + "<XSD設定 appinfo :  DBtoDB設定未完成> "
					End If
			End Select
			'=======分析element 結構
			Dim Vxsd = From el In XDoc.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing AndAlso el.@sql:relation <> ""
					   Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}

			For Each vt In Vxsd
				colid = 1
				MapInf = From el2 In (From el In XDoc.Descendants().<xsd:element> Where el.@name = vt.name
									  Select el).Descendants().<xsd:element>
						 Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
						 Select New MapingBox With {.Tfield = el2.@name, .Sqlfield = IIf(el2.@sql:field Is Nothing, vt.name, el2.@sql:field), .fixed = IIf(el2.@fixed Is Nothing, 0, el2.@fixed),
				.item = colid, .SourceDt = vt.name, .DecDt = vt.relation, .PadRL = IIf(el2.@PadRL Is Nothing, 0, el2.@PadRL)}
				For Each elx In MapInf
					colid += 1
				Next
			Next

			''Create_MapDs(XDoc)

			'===  Import 其他必要條件
			If INFType = "Import" Then
				If XDoc.Descendants().<xsd:element>.Elements.Count <= 0 Then _
				 ClStatus += XsdFile + "<XSD設定 Import :  element 結構設定異常! > "
				'If FileIO.FileSystem.FileExists(Me.EDLLocDr) = False Then _
				'	ClStatus += XsdFile + "<XSD設定 Import :  匯入檔案必需存在> "
			End If
			'' EXPORT 其他必要條件
			'If INFType.ToUpper = "EXPORT" Then
			'	If (FileType.ToUpper = "TXT" Or FileType.ToUpper = "XML") And MapInf.Count() < 1 Then
			'		ClStatus += XsdFile + "<XSD 設定 Expport :  TXT 類型 element 結構設定異常! > "
			'	End If
			'End If
			ClStatus = "OK"
		Catch ex As Exception
			Dim Pr() As String = {"Xsd New(...)", impsFile, XsdFile, "<Ds解析錯誤>", ex.Message}
			ErrFmtLog(Pr)
			'Me.Dispose()
		End Try
	End Sub

	Public Overloads Sub Dispose() Implements IDisposable.Dispose
		Me.Dispose()
	End Sub
#End Region

#Region "XSD 文件結構名稱"
	Public Class MapingBox
		Public Tfield As String             ''''XSD 文件結構欄位名稱
		Public Sqlfield As String           ''''T-SQL Query  欄位名稱
		Public item As Integer              ''''順序
		Public fixed As Integer     '''' 文件 inBound/outBound 欄位長。
		Public SourceDt As String       '''' 來源表DataTable 
		Public DecDt As String          '''' SQL 表 Table 
		Public PadRL As String          ''''''''' Text 靠左靠右 。 
	End Class
	'========建立 Mapinf  ========================= 
	'依XSD文件建立 Mapinf 集合資訊 (未Maping)
	'======================================== 
	Public Sub Create_MapDs(ByVal XDoc1 As XDocument)
		Dim colid As Integer = 1
		Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
		For Each vt In Vxsd
			Dim ctab As DataTable = MapDs.Tables.Add(vt.name)
			'ctab.Columns.Add("SourceDt", GetType(String))
			ctab.Columns.Add("DecDt", GetType(String))
			ctab.Columns.Add("item", GetType(Integer))
			ctab.Columns.Add("Tfield", GetType(String))
			ctab.Columns.Add("Sqlfield", GetType(String))
			ctab.Columns.Add("fixed", GetType(Integer))
			ctab.Columns.Add("PadRL", GetType(String))

			Dim MapInfX = (From el2 In (From el In XDoc.Descendants().<xsd:element> Where el.@name = vt.name
										Select el).Descendants().<xsd:element>
						   Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
						   Select New With {.Tfield = el2.@name, .Sqlfield = IIf(el2.@sql:field Is Nothing, el2.@name, el2.@sql:field), .fixed = IIf(el2.@fixed Is Nothing, 0, el2.@fixed),
							.item = 0, .SourceDt = vt.name, .DecDt = vt.relation, .PadRL = IIf(el2.@PadRL Is Nothing, 0, el2.@PadRL)}).ToList
			For Each elx In MapInfX
				Dim dr As DataRow = ctab.Rows.Add()
				'dr("SourceDt") = elx.SourceDt
				dr("DecDt") = elx.DecDt
				dr("item") = colid
				dr("Tfield") = elx.Tfield
				dr("Sqlfield") = elx.Sqlfield
				dr("fixed") = elx.fixed
				dr("PadRL") = elx.PadRL
				colid += 1
			Next
		Next
	End Sub
#End Region

	'========錯誤 FMT LOG
	'  
	'================= 
	Public Function ErrFmtLog(ByVal Pr() As String) As String
		ErrFmtLog = ""
		Try
			Dim ErrFmt As String = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0,-25}	{1,-20}	{2,-20}	ErrMessage:  {4,-50}" & vbCrLf
			ErrFmtLog = String.Format(ErrFmt, Pr)
			FileIO.FileSystem.WriteAllText(APPLog, ErrFmtLog, True)
		Catch ex As Exception
			FileIO.FileSystem.WriteAllText(APPLog, "ErrFmtLog()錯誤Log 本身" + ex.Message, True)
		End Try
	End Function
	'========建立接口資料表 DataSet 表格=========== 
	' 依XSD 的 DATA TYPEXElement  自行建立單純結構 DataSet 表格
	'基本必要欄位  exdate ?  				Return "Schema OK!"     Return "CrEdiDsFmt:<數據結構解析錯誤!> "
	'================================== 
	Public Overloads Function CrEdiDsFmt() As String
		CrEdiDsFmt = "-1"
		Dim colid As Integer = 0
		'=== 自建表及讀檔 暫不補  Where el2.@sql:field <> ""
		Try
			Dim Vxsd = From el In XDoc.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
			For Each vt In Vxsd
				Mds.Tables.Add(vt.name)
				Dim cInf = From el2 In (From el In XDoc.Descendants().<xsd:element> Where el.@name = vt.name
										Select el).Descendants().<xsd:element>
						   Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
						   Select el2.@name
				For Each elx In cInf
					colid += 1
					Dim colname As String = elx.ToString
					Mds.Tables(vt.name).Columns.Add(colname, GetType(String))
				Next

				If Mds.Tables(vt.name).Columns.Contains("sfile") = False Then
					Mds.Tables(vt.name).Columns.Add("edate", Type.GetType("System.DateTime"))
					Mds.Tables(vt.name).Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					Mds.Tables(vt.name).Columns.Add("sfile", Type.GetType("System.String"))
					Mds.Tables(vt.name).Columns("sfile").DefaultValue = impsFile
					Mds.Tables(vt.name).Columns.Add("etyps", Type.GetType("System.Int32"))
					Mds.Tables(vt.name).Columns("etyps").DefaultValue = 0
					Mds.Tables(vt.name).Columns.Add("clientcode", Type.GetType("System.String"))
					Mds.Tables(vt.name).Columns("clientcode").DefaultValue = StKey
				Else
					Mds.Tables(vt.name).Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					Mds.Tables(vt.name).Columns("sfile").DefaultValue = impsFile
					Mds.Tables(vt.name).Columns("etyps").DefaultValue = 0
					Mds.Tables(vt.name).Columns("clientcode").DefaultValue = StKey
				End If
			Next
			If Mds.Tables.Count = Vxsd.Count Then
				Return "Schema OK!"
			Else
				Return "CrEdiDsFmt:<數據結構解析錯誤!> "
			End If

		Catch ex As DataException
			Dim Pr() As String = {"CrEdiDsFmt()", Mds.DataSetName, "NA", "<自建 DataSet解析錯誤>", ex.Message}
			CrEdiDsFmt = ErrFmtLog(Pr)
			'FileIO.FileSystem.WriteAllText(APPLog, String.Format(ClStatus, "CrEdiDsFmt", Mds.DataSetName, "", "<自建 DataSet解析錯誤>"), True)
		End Try
		Return CrEdiDsFmt
	End Function
	'=============
	'   單表 (TXT)
	' Element 關鍵字relation
	'=============
	Public Overloads Function CrEdiDsFmt(ByRef MidDs As DataSet) As String
		CrEdiDsFmt = "-1"
		Dim colid As Integer = 0
		'====建表及讀檔
		Try
			Dim Vxsd = From el In XDoc.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
			For Each vt In Vxsd
				Mds.Tables.Add(vt.name)
				Dim cInf = From el2 In (From el In XDoc.Descendants().<xsd:element> Where el.@name = vt.name
										Select el).Descendants().<xsd:element>
						   Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
						   Select el2.@name
				For Each elx In cInf
					colid += 1
					Dim colname As String = elx.ToString
					MidDs.Tables(vt.name).Columns.Add(colname, GetType(String))
				Next

				If MidDs.Tables(vt.name).Columns.Contains("sfile") = False Then
					MidDs.Tables(vt.name).Columns.Add("edate", Type.GetType("System.DateTime"))
					MidDs.Tables(vt.name).Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					MidDs.Tables(vt.name).Columns.Add("sfile", Type.GetType("System.String"))
					MidDs.Tables(vt.name).Columns("sfile").DefaultValue = impsFile
					MidDs.Tables(vt.name).Columns.Add("etyps", Type.GetType("System.Int32"))
					MidDs.Tables(vt.name).Columns("etyps").DefaultValue = 0
					MidDs.Tables(vt.name).Columns.Add("clientcode", Type.GetType("System.String"))
					MidDs.Tables(vt.name).Columns("clientcode").DefaultValue = StKey
				Else
					MidDs.Tables(vt.name).Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					MidDs.Tables(vt.name).Columns("sfile").DefaultValue = impsFile
					MidDs.Tables(vt.name).Columns("etyps").DefaultValue = 0
					MidDs.Tables(vt.name).Columns("clientcode").DefaultValue = StKey
				End If

			Next

			If MidDs.Tables.Count = Vxsd.Count Then
				Return "Schema OK!"
			Else
				Return "CrEdiDsFmt:<數據結構解析錯誤!> "
			End If

		Catch ex As DataException
			Dim Pr() As String = {"CrEdiDsFmt(Xdoc,ds)", "行:" & MidDs.DataSetName, "", "<自建 DataSet解析錯誤>", ex.Message}
			ClStatus = ErrFmtLog(Pr)
		End Try
		Return CrEdiDsFmt
	End Function

	'========  讀取XML檔方式 ============= 
	'  tabN : 將DataSet Table 傳入 , sfile :  文字檔位置，ws : 
	'2020/03/07 補SAP表 clientcode由 SP 傳入  
	'=============================== 
	'' ==== 新版版本   指定回傳一個表  =====
	Public Overloads Function readXmlFile(ByRef TblNo As Integer) As Integer
		Try
			If XsdFile <> "" Then
				Mds.ReadXmlSchema(XsdFile)
				For Each cTab As DataTable In Mds.Tables
					For Each c As DataColumn In cTab.Columns
						c.AllowDBNull = True
					Next
				Next
			End If

			Mds.ReadXml(impsFile)
			' 系統需求欄補建 必要欄位 sfile , edate ,etyps
			For Each cTab As DataTable In Mds.Tables
				If cTab.Columns.Contains("sfile") = False Then cTab.Columns.Add("sfile", Type.GetType("System.String"))
				If cTab.Columns.Contains("edate") = False Then cTab.Columns.Add("edate", Type.GetType("System.DateTime"))
				If cTab.Columns.Contains("etyps") = False Then cTab.Columns.Add("etyps", Type.GetType("System.Int32"))
				If cTab.Columns.Contains("clientcode") = False Then cTab.Columns.Add("clientcode", Type.GetType("System.String"))
				For Each dr As DataRow In cTab.Rows
					dr("sfile") = impsFile
					dr("edate") = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					dr("etyps") = 0
					dr("clientcode") = StKey
				Next
			Next
			Mds.AcceptChanges()
			readXmlFile = Mds.Tables(TblNo).Rows.Count

		Catch ex As Exception
			readXmlFile = 0
			Dim Pr() As String = {"readXmlFile(1)", "0", impsFile, "<Ds數據ReadXml解析錯誤>", ex.Message}
			ClStatus = ErrFmtLog(Pr)
		End Try

	End Function

	'' ==== 多表 版本 直接塞入值 =====
	Public Overloads Function readXmlFile() As Integer
		Dim rcnt As Integer = 0
		Try
			If XsdFile <> "" Then
				'先修後讀
				Mds.ReadXmlSchema(XsdFile)
				For Each cTab As DataTable In Mds.Tables
					For Each c As DataColumn In cTab.Columns
						c.AllowDBNull = True
					Next
					'If cTab.Columns.Contains("sfile") = True Then   '''  DefaultValue 無效
					'	cTab.Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					'	cTab.Columns("sfile").DefaultValue = impsFile
					'	cTab.Columns("etyps").DefaultValue = 0
					'	cTab.Columns("clientcode").DefaultValue = Stkey
					'End If
				Next

			End If
			' 直接讀 再補建系統需求欄
			' 7/16 NET Framework 安全性和品質彙總套件 (KB4565613) 後無法再使用 ReadXml
			'   目前依Schema 再 ReadXml(
			Mds.ReadXml(impsFile)
			For Each cTab As DataTable In Mds.Tables
				If cTab.Columns.Contains("sfile") = False Then cTab.Columns.Add("sfile", Type.GetType("System.String"))
				If cTab.Columns.Contains("edate") = False Then cTab.Columns.Add("edate", Type.GetType("System.DateTime"))
				If cTab.Columns.Contains("etyps") = False Then cTab.Columns.Add("etyps", Type.GetType("System.Int32"))
				If cTab.Columns.Contains("clientcode") = False Then cTab.Columns.Add("clientcode", Type.GetType("System.String"))
				For Each dr As DataRow In cTab.Rows
					rcnt += 1
					dr("sfile") = impsFile
					dr("edate") = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
					dr("etyps") = 0
					dr("clientcode") = StKey
				Next
			Next

			Mds.AcceptChanges()
			readXmlFile = rcnt
		Catch ex As Exception
			readXmlFile = 0
			Dim Pr() As String = {"readXmlFile(sfile)", rcnt, impsFile, "<Ds數據ReadXml解析錯誤>", ex.Message}
			ErrFmtLog(Pr)
		End Try

	End Function


	'========  讀取文字檔方式 TXT============= 
	' 暫時只處理單一格式文字檔 
	' FieldType= FixedWidth(指定Bytes寬度) ; Delimited(指定服符號) ;FixedLength(HK 2字節混淆算UTF8  )
	'  tabN : 將DataSet Table 傳入 , sfile :  文字檔位置，ws : 
	'=============================== 
	Public Function readTxtFile2Table(ByVal sfile As String) As Integer
		readTxtFile2Table = -1
		Dim lno As Integer = 0
		Dim ws As Integer()
		Dim LarrWidth As Integer
		Dim Filecode As Encoding
		If IsNumeric(FieldParser) Then FieldParser = ChrW(FieldParser)
		If Fcoding = "BIG5" Then
			Filecode = Encoding.GetEncoding(950)
		Else
			Filecode = Encoding.UTF8
		End If
		Try

			Using myReader As New TextFieldParser(sfile, Filecode)
				Select Case FieldParser
					Case "FixedWords"
						'必需使用Bytes字元寬度 'ReadFields 無法處理 : 中文
						ws = (From el In MapInf Where el.fixed <> 0 Select el.fixed).ToArray
						'Dim myLine As String = myReader.PeekChars(ws.Sum)
						Dim TxtVal As String()
						myReader.TextFieldType = FieldType.FixedWidth
						myReader.SetFieldWidths(ws)
						While Not myReader.EndOfData
							TxtVal = myReader.ReadFields
							lno += 1
						End While
						'Else
					Case "FixedBytesWidth"
						ws = (From el In MapInf Where el.fixed <> 0 Select el.fixed).ToArray
						Dim myLine As String = myReader.PeekChars(ws.Sum)
						myReader.TextFieldType = FieldType.FixedWidth
						While Not myReader.EndOfData
							Dim TxtVal As String()
							Dim TextField As String = ""
							Dim listVal As New List(Of String)
							myLine = myReader.ReadLine()
							For Each w As Integer In ws
								Dim cBytes() As Byte = Filecode.GetBytes(myLine)
								If w > 0 Then
									TextField = Filecode.GetString(cBytes, 0, w)
									myLine = Filecode.GetString(cBytes, w, cBytes.Length - w)
								ElseIf w = 0 Then
									TextField = ""
								ElseIf w = -1 Then    '最後全取
									TextField = Filecode.GetString(cBytes, 0, cBytes.Length)
									listVal.Add(TextField)
									Exit For
								End If
								listVal.Add(TextField)
							Next
							TxtVal = listVal.ToArray()
							Mds.Tables(0).Rows.Add(TxtVal)
							'Try
							'Catch ex As Exception
							'	'Dim Pr() As String = {"myReader", "行:" & myReader.PeekChars(ws.Sum) & "欄:" & ws.Sum.ToString, FieldParser & "", "<讀取錯誤>" & myReader.ErrorLineNumber, ex.Message}
							'	'ErrFmtLog(Pr)
							'	'Exit Function
							'End Try
							lno += 1
						End While
						'End If
					Case "FixedBIG5xUTF8_HK"   '原: 
						'HK 使用2字節計算UTF8 ; 寬度不足且 不到位
						'以下計算中文字數再補足字節數後切斷；再取下一攔(遇多餘空格靠右)
						Dim myLine As String
						ws = (From el In MapInf Where el.fixed <> 0 Select el.fixed).ToArray
						'Dim Tws As Integer() = ws.[Select](Function(s) Integer.Parse(s)).Aggregate(CType(New List(Of Integer)(), IEnumerable(Of Integer)), Function(a, i) a.Concat({a.LastOrDefault() + i})).ToArray()
						While Not myReader.EndOfData
							myLine = myReader.ReadLine()
							Dim TextSlip As String = ""
							Dim TextFieldP As String = ""
							'Dim U8Data As Byte() = System.Text.Encoding.UTF8.GetBytes(myLine)
							'Dim Bg5Data As Byte() = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(950), U8Data)
							Dim x As Integer = 0
							Dim dflen As Integer = 0
							Dim lenSt As Integer = 0
							Dim SubTxtVal() As String   ''Dim SubTxtVal As New List(Of String)()
							'Dim SubTxtVal =
							'  (From i In ws From j In Tws Select Encoding.UTF8.GetString(U8Data, 0, i)).ToArray
							For Each lx As Integer In ws
								dflen = 0
								lenSt = 0
								If myLine.Length >= lx Then
									TextFieldP = myLine.Substring(0, lx)
									If Encoding.UTF8.GetBytes(TextFieldP).Length <> lx Then
										dflen = (Encoding.UTF8.GetBytes(TextFieldP).Length - lx) \ 2
										If TextFieldP.Length - TextFieldP.TrimStart(" "c).Length > 0 And TextFieldP.Trim <> "" Then
											lenSt = (TextFieldP.Length - TextFieldP.TrimStart(" "c).Length)
											'TextFieldP = TextFieldP.Substring(lenSt, lx - dflen)
											TextFieldP = myLine.Substring(lenSt, lx - dflen)
										Else
											TextFieldP = myLine.Substring(0, lx - dflen)
										End If
									End If
									myLine = myLine.Substring(lx + lenSt - dflen)

								End If
								x += 1
								TextSlip += TextFieldP + IIf(ws.Length > x, "|", "")
							Next
							SubTxtVal = TextSlip.Split("|"c)
							Mds.Tables(0).Rows.Add(SubTxtVal)
							' Dim(SubTxtVal = Enumerable.Range(1, ws.Count() - 1) _
							'.Select(Function(i) myLine.Substring(ws(i - 1), ws(i) - ws(i - 1))))
							lno += 1
						End While

					Case Else
						myReader.TrimWhiteSpace = True
						myReader.HasFieldsEnclosedInQuotes = False
						myReader.TextFieldType = FieldType.Delimited
						myReader.SetDelimiters(FieldParser)
						While Not myReader.EndOfData
							Dim TxtVal As String()
							TxtVal = myReader.ReadFields
							Mds.Tables(0).Rows.Add(TxtVal)
							lno += 1
						End While

				End Select

			End Using
			readTxtFile2Table = lno
		Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
			Dim Pr() As String = {"readTxtFile2Table(sfile)", "行:" & lno.ToString & "欄:" & ws.Sum.ToString, FieldParser & "", "<讀取發生錯誤>", ex.Message}
			ClStatus = ErrFmtLog(Pr)
		End Try
	End Function

	Public Function readTxtFile2Ds(ByVal sfile As String) As Integer
		readTxtFile2Ds = -1
		Dim lno As Integer = 0
		Dim wsH() As Integer
		Dim wsD() As Integer
		'Dim wsL() As Integer
		Dim LarrWidth As Integer
		Dim Filecode As Encoding
		Dim SearchFun() As String = SearchStr.ToString.Split(",")
		If IsNumeric(FieldParser) Then FieldParser = ChrW(FieldParser)

		Fcoding = XApi.<FileMap>.@Encoding
		If Fcoding = "BIG5" Then
			Filecode = Encoding.GetEncoding(950)
		Else
			Filecode = Encoding.UTF8
		End If
		Try
			' xsd..<EDIinf>.@FieldParser設定 取檔方式 
			Using myReader As New TextFieldParser(sfile, Filecode)

				Select Case FieldParser
					Case "FixedWidth"
						'必需使用Bytes字元寬度
						myReader.TextFieldType = FieldType.FixedWidth
						For Each vt As DataTable In Mds.Tables
							wsH = (From el As DataRow In vt.Rows Where el("fixed") <> 0 Select CInt(el("fixed"))).ToArray
							wsD = (From el As DataRow In vt Where el("fixed") <> 0 And el("SourceDt") = vt.TableName Select CInt(el("fixed"))).ToArray
							'wsD = (From el In MapInf Where el.fixed <> 0 And el.SourceDt = vt.TableName Select el.fixed).ToArray
						Next

						While Not myReader.EndOfData
							Dim TxtVal As String()
							If lno = 0 Then
								myReader.SetFieldWidths(wsH)
								TxtVal = myReader.ReadFields
								Mds.Tables(0).Rows.Add(TxtVal)
							Else
								myReader.SetFieldWidths(wsD)
								TxtVal = myReader.ReadFields
								Mds.Tables(0).Rows.Add(TxtVal)
							End If
							lno += 1
						End While
					Case Else
						'一律使用 分隔
						myReader.TrimWhiteSpace = True
						myReader.HasFieldsEnclosedInQuotes = False
						myReader.TextFieldType = FieldType.Delimited
						myReader.SetDelimiters(FieldParser)
						If SearchStr = "0" Then
							While Not myReader.EndOfData
								Dim TxtVal() As String
								' 1 HOST  其他為Detail  >   1 
								TxtVal = myReader.ReadFields
								If lno = 0 Then Mds.Tables(0).Rows.Add(TxtVal)
								If lno > 0 Then Mds.Tables(1).Rows.Add(TxtVal)
								If Mds.Tables.Count > 2 And myReader.EndOfData = True Then
									Mds.Tables(2).Rows.Add(TxtVal)
								End If
								lno += 1
							End While
						Else
							While Not myReader.EndOfData
								Dim TxtVal() As String
								If myReader.PeekChars(SearchFun(0)).IndexOf(FieldParser + SearchFun(1)) > 0 Then
									TxtVal = myReader.ReadFields
									Mds.Tables(0).Rows.Add(TxtVal)
								End If
								If myReader.PeekChars(SearchFun(0)).IndexOf(FieldParser + SearchFun(2)) > 0 Then
									TxtVal = myReader.ReadFields
									Mds.Tables(1).Rows.Add(TxtVal)
								End If
								If Mds.Tables.Count > 2 AndAlso myReader.PeekChars(SearchFun(0)).IndexOf(FieldParser + SearchFun(3)) > 0 Then
									TxtVal = myReader.ReadFields
									Mds.Tables(2).Rows.Add(TxtVal)
								End If
								' 此設定未考量舊版未設 SearchStr
								'For i = 1 To SearchFun.Length - 1
								'	'使用每行的WORK key (H；D;L)
								'	If myReader.PeekChars(SearchFun(0)).IndexOf(FieldParser + SearchFun(i)) > 0 Then
								'		TxtVal = myReader.ReadFields
								'		Mds.Tables(i - 1).Rows.Add(TxtVal)
								'		lno += 1
								'	End If
								'Next
								lno += 1
							End While

						End If


				End Select
			End Using
			readTxtFile2Ds = lno
		Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
			Dim Pr() As String = {"readTxtFile2Ds(sfile)", lno.ToString, LarrWidth.ToString, "<讀取 \N 筆發生錯誤>", ex.Message}
			ErrFmtLog(Pr)
			'FileIO.FileSystem.WriteAllText(APPLog, String.Format(" 錯誤描述  ERR: {0} ; readTxtFile  {1}  ", ClStatus, LarrWidth) + Chr(10), True)
		End Try
	End Function



	'=========  寫檔案 =====
	'How to write text file with fixed length from datatable
	'依 XSD 結構取得 fixed length : 對等的QUERY 集合 含虛擬表
	'ByVal CommandNode As System.Xml.Linq.XElement  
	'=================
	Public Function WrFileFixWidth(ByVal efPath As String, ByVal Fcode As String) As Long
		Dim ECodingS As Encoding = Nothing '= New UTF8Encoding(False)
		Dim StartAt As Integer = 0
		Dim r As Long = 0
		Try
			If Fcode.ToUpper = "UTF8" Then
				ECodingS = New UTF8Encoding(False)
			ElseIf Fcode.ToUpper = "BIG5" Then
				ECodingS = Encoding.GetEncoding("BIG5")
			Else
				ECodingS = Encoding.Default
			End If
		Catch ex As Exception
		End Try

		'Dim positions = From c In CommandNode.Descendants(Namespaces.Integration & "Position") Order By Integer.Parse(c.Attribute("Start").Value) Select New With { Key
		'      .Name = c.Attribute("Name").Value, Key
		'      .Start = Integer.Parse(c.Attribute("Start").Value) - StartAt, Key
		'      .Length = Integer.Parse(c.Attribute("Length").Value), Key
		'.Justification = If(c.Attribute("Justification") IsNot Nothing, c.Attribute("Justification").Value.ToLower(), "left")

		'Dim lineLength As Integer = positions.Last().Start + positions.Last().Length
		'lineLength = MapInf.Sum(Function(Length) Length.Length)
		Try
			Dim Table As DataTable = Mds.Tables(0)
			Dim lineLength As Integer = (From c In MapInf Select c.fixed).Sum()
			Using myReadTs As New StreamWriter(efPath, False, ECodingS)
				For Each row As DataRow In Table.Rows
					Dim line As StringBuilder = New StringBuilder(lineLength)
					For Each p In MapInf
						'line.Insert(p.Start, If(p.Justification = "left", (If(row.Field(Of String)(p.Name), "")).PadRight(p.Length, " "c), (If(row.Field(Of String)(p.Name), "")).PadLeft(p.Length, " "c)))
						StartAt += p.fixed
						line.Insert(StartAt, If(p.PadRL = "left", (If(row(p.DecDt), "")).PadRight(p.fixed, " "c), (If(row(p.DecDt), "")).PadLeft(p.fixed, " "c)))
					Next
					r += 1
					myReadTs.WriteLine(line.ToString())
				Next
				myReadTs.Flush()
			End Using
		Catch ex As Exception
			Return -1
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0} / Function位置 {1} ", ex.Message, "WrFileFixWidth") + Chr(10), True)
			FileIO.FileSystem.RenameFile(efPath, efPath & "." & r & "Err")
		End Try
		WrFileFixWidth = r
	End Function

	'=================
	'將查詢結果轉入中繼DataSet
	'簡易版: 需依 XSD 結構取得: 對等的QUERY 集合 含虛擬表
	'=================
	Public Function DstoDs(ByVal SDs As DataSet) As Integer
		'Dim Vxsd = From el In adpinf.Descendants().<xs:element> Where (el.@sql:relation IsNot Nothing)
		'		Select New With {.Dname = el.@name, .Sname = el.@sql:relation, .Qname = "N", .xdIts = 0, .MapCk = "-1", .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H"), .recnt = 0}
		Dim rcnt As Long
		Dim STableID As Integer = 0
		For Each T In Mds.Tables
			If SDs.Tables.Count = Mds.Tables.Count Then
				For Each c As DataColumn In T.Columns
					c.AllowDBNull = True
				Next
				For Each rs As DataRow In SDs.Tables(STableID).Rows
					T.ImportRow(rs)
					rcnt += 1
				Next
				STableID += 1
			End If
		Next
		DstoDs = rcnt
	End Function

	Public Function DatasetWrTxtFile(ByVal efilePayh As String, ByVal FieldTag As String, ByVal Fcode As String) As Long
		'DatasetWrTxtFile = ""
		Dim wrline As Long
		Dim Xstr As StringBuilder = Nothing
		Try
			Dim ECodingS As Encoding = Nothing '= New UTF8Encoding(False)
			If Fcode.ToUpper = "UTF8" Then
				ECodingS = New UTF8Encoding(False)
			ElseIf Fcode.ToUpper = "BIG5" Then
				ECodingS = Encoding.GetEncoding("BIG5")
			Else
				ECodingS = Encoding.Default
			End If

			'Using myReadTs As New StreamWriter(efilePayh, False, Encoding.UTF8)
			Using myReadTs As New StreamWriter(efilePayh, False, ECodingS)
				For i As Integer = 0 To Mds.Tables.Count - 1
					For Each drow As DataRow In Mds.Tables(i).Rows
						Dim linetext = String.Join(FieldTag, drow.ItemArray.Select(Function(s) s.ToString.TrimEnd(" ")).ToArray)
						myReadTs.WriteLine(linetext)
						wrline += 1
					Next
				Next
				myReadTs.Flush()
				myReadTs.Close()
			End Using
			DatasetWrTxtFile = wrline
		Catch ex As Exception
			Return wrline
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0} / Function位置 {1} ", ex.Message, "DatasetWrTxtFile") + Chr(10), True)
			FileIO.FileSystem.RenameFile(efilePayh, efilePayh & "." & wrline & "Err")
		End Try

	End Function



	'============================
	' 維護JOBQ :  LotKey1,LotKey2,LotKey3,SERR
	'XSD JOBQTypes 需傳 "RDo" 取預設
	'============================
	Public Function JobQSvrCmd(ByVal CmdJobq As String, ByVal ewkey As String, ByVal setValue As String, ByVal WhereStr As String, Optional ByVal msgKey() As String = Nothing) As Integer
		JobQSvrCmd = 0
		If CmdJobq.ToLower = "rdo" Or CmdJobq.ToLower = "std" Or CmdJobq.ToLower = "detail" Then
			CmdJobq = "UPDATE SJOB..JobQ set udate=getdate() ,tscount=" + Tscount.ToString + " @setValue where Wkey='@Wkey' @WhereStr;"

		End If
		If CmdJobq.ToLower.IndexOf("update") > -1 Then
			CmdJobq = CmdJobq.Replace("@Wkey", ewkey)               ' where 主key
			CmdJobq = CmdJobq.Replace("@setValue", setValue)    ' 補充其他欄位更新+值
			CmdJobq = CmdJobq.Replace("@WhereStr", WhereStr)    ' 補充 其他條件
		End If
		'' 自建 JOBQ 登記 
		If CmdJobq.IndexOf("INSERT") > -1 Then
			CmdJobq = "INSERT INTO SJOB..JobQ(STKEY, Wkey, InfID, Jtyps, sfile, Rfkey1, Rfkey2, lotkey1, lotkey2, rdo, exfile, tscount) " _
			+ "SELECT  Stkey,Wkey='@Wkey',Infid ,Jtyps='@Jtyps',  sfile=EDILocDr , Rfkey1=back_one , Rfkey2=EAIpath , LotKey1='@LotKey1', LotKey2='@LotKey2',rDo=null ,exfile=EDILocDr+ '@Wkey',tscount=@tscount" _
			+ "from  EDIinf  where Infid='" + InfKey + "' and Stkey='" + StKey + "'; "
			CmdJobq.Replace("@Jtyps", msgKey(0).ToString)
			CmdJobq.Replace("@LotKey1", msgKey(1).ToString)
			CmdJobq.Replace("@LotKey2", msgKey(2).ToString)
			CmdJobq.Replace("@wkey", ewkey)
		End If
		If CmdJobq = "" Then
			JobQSvrCmd = -9
			Exit Function
		End If

		Try
			Using conJob As New SqlClient.SqlConnection(DBConCls.JobQConstr)
				Using JOBM As New SqlClient.SqlCommand(CmdJobq, conJob)
					conJob.Open()
					JOBM.ExecuteNonQuery()
				End Using
			End Using
		Catch ex As DataException
			Dim Pr() As String = {"JobQSvrCmd(...)", ewkey, CmdJobq, "<更新SJOB TSQL錯誤>", ex.Message}
			ErrFmtLog(Pr)
		End Try

	End Function

	'========建立接口檔案 :命名方式======== 
	'  Extkeys(0) : SQL CLR 傳入的次參數(STORERKEY,odkey,其他) 
	' wkey: SQL CLR 傳入的主參數: 為檔案名 / 單號 
	'=========================
	Public Function FileFmtname(ByVal infkey As String, ByVal exkey As String, ByVal wkey As String) As String
		'Dim stkey As String() = exkey.Split(",")
		Dim Filename As String = EDLLocDr
		Dim Fmt As String = FilenameFmt
		'@wkey如果是單號或  FilenameFmt=未指示 既使用標準
		If (Fmt = "" Or String.IsNullOrEmpty(Fmt)) Or (wkey.IndexOf(".") = -1 And Fmt = "@wkey") Then
			Fmt = infkey + "_" + Right(Me.StKey, 4) + "_" + wkey + "_" + Now.ToString("HHmmssff") + ".PUB"
		End If
		' 如wkey 已是檔案名直接引用 
		'If wkey.IndexOf(".") > -1 Then
		'	Fmt = Fmt.Replace("@wkey", wkey)
		'End If
		Fmt = Fmt.Replace("@wkey", wkey)
		If Fmt.IndexOf("@") > 0 Then
			If Fmt.IndexOf("@stkey") > 0 Then Fmt = Fmt.Replace("@stkey", Right(Me.StKey, 4))
			If Fmt.IndexOf("@odkey") >= 0 And Extkeys.Length > 1 Then Fmt = Fmt.Replace("@odkey", Extkeys(1))
			If Fmt.IndexOf("@yymmdd") >= 0 Then Fmt = Fmt.Replace("@yymmdd", Now.ToString("yyMMdd"))
			If Fmt.IndexOf("@hhmmssff") >= 0 Then Fmt = Fmt.Replace("@hhmmssff", Now.ToString("HHmmssff"))
			'If Fmt.IndexOf("mmssff") > 0 Then Fmt = Fmt.Replace("@mmssff", Now.ToString("HHmmssff"))
			If Fmt.IndexOf("@yyyymmdd") >= 0 Then Fmt = Fmt.Replace("@yyyymmdd", Now.ToString("yyyyMMdd"))
			If Fmt.IndexOf("@yyyyMMddhhmm") >= 0 Then Fmt = Fmt.Replace("@yyyyMMddhhmm", Now.ToString("yyyyMMddHHmm"))

		End If
		Return Fmt
	End Function



	'*********** 轉入SQL區塊 ********************
	'======== 資料表 DATASET 依Mapinf  ========================== 
	' 使用DATASET + XsdMaping (sql:field)來源數據 至 SQL server   
	'  XMaping(sql:field) 為主 ，如DataTable 查無攔欄位,亦不使用 
	'=================================================
	Public Function XsdMapCopy() As String
		Dim sql As XNamespace = XDoc.Root.GetNamespaceOfPrefix("sql")
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(DBConCls.CONNs)
		Dim colid As Integer = 0
		Dim Rtrow As Long = 0
		con.Open()
		Dim SqlCopy = New SqlBulkCopy(con)
		SqlCopy.BatchSize = 10000
		SqlCopy.BulkCopyTimeout = 720   '30分
		'XsdMapCopy = 0
		'Try
		Dim Vxsd = From el In XDoc.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
		'Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing AndAlso el.@sql:relation <> ""
		'		   Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}

		For Each vt In Vxsd
			colid = 1
			MapInf = From el2 In (From el In XDoc.Descendants().<xsd:element> Where el.@name = vt.name
								  Select el).Descendants().<xsd:element>
					 Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
					 Select New MapingBox With {.Tfield = el2.@name, .Sqlfield = IIf(el2.@sql:field Is Nothing, el2.@name, el2.@sql:field), .fixed = IIf(el2.@fixed Is Nothing, 0, el2.@fixed),
			.item = colid, .SourceDt = vt.name, .DecDt = vt.relation, .PadRL = IIf(el2.@PadRL Is Nothing, 0, el2.@PadRL)}
			For Each elx In MapInf
				colid += 1
				'Console.WriteLine("{0},{1},{2}", elx.Tfield, elx.Sqlfield, elx.item)
				If Mds.Tables(vt.name).Columns.Contains(elx.Tfield) = True And elx.Sqlfield <> "" Then
					SqlCopy.ColumnMappings.Add(elx.Tfield, elx.Sqlfield)
				End If
			Next
			' 資料表確認是否有料 sfile,edate,etyps 改XSD內指示
			If Mds.Tables(vt.name).Rows.Count > 0 And SqlCopy.ColumnMappings.Count > 0 Then

				SqlCopy.DestinationTableName = vt.relation
				'  ' 一次匯入，如果目的表長度不足無法Debug
				Try
					SqlCopy.WriteToServer(Mds.Tables(vt.name))
					SqlCopy.ColumnMappings.Clear()
					Rtrow += Mds.Tables(vt.name).Rows.Count
				Catch ex As Exception
					'回傳 錯誤內容 暫無法傳到前端，改使用XML 記錄錯誤 
					Mds.Tables(vt.name).Rows(0)("sfile") = ex.Message
					Mds.WriteXml(impsFile.ToString.Replace(".XML", "") & "-內容錯誤協助.XML")
					Dim Pr() As String = {"XsdMapCopy(...)", SqlCopy.DestinationTableName, "欄數:" & SqlCopy.ColumnMappings.Count & "+筆數:" & Mds.Tables(vt.name).Rows.Count, "<SqlCopy: WriteToServer錯誤>", ex.Message}
					Me.ClStatus = ErrFmtLog(Pr)
					SqlCopy.ColumnMappings.Clear()
				End Try
			End If
		Next
		SqlCopy.Close()
		con.Close()
		con.Dispose()
		Tscount = Rtrow
		XsdMapCopy = Rtrow

	End Function

	'======匯入數據 : INFType(檔案類型) import  ======== 
	' XML/TXT 轉入DataSet >> 指定要轉的表名 與USER Defined Table 
	' ikey=接口ID，iFpath=wkey
	'<CommandText> 指定轉檔的 PROC 
	'==============================
	Public Function XsdMergeUDT(ByVal CommText As String, ByVal UDTkey As String, ByVal tblindex As Integer, ByRef Mds As DataSet) As Integer
		'  Table 傳回SQL user Type
		XsdMergeUDT = -1
		Try
			Using MgConn As New SqlClient.SqlConnection(DBConCls.EDIconstr)
				Using MgCmd As New SqlClient.SqlCommand(CommText, MgConn)
					Dim Utyp As SqlParameter = MgCmd.Parameters.Add("@UT_CVSF01", SqlDbType.Structured)
					Utyp.Value = Mds.Tables(tblindex)
					Utyp.TypeName = "UT_" & UDTkey
					MgConn.Open()
					XsdMergeUDT = MgCmd.ExecuteNonQuery()

				End Using
			End Using
			Me.Tscount = XsdMergeUDT
		Catch ex As DataException
			Dim Pr() As String = {"XsdMergeUDT(..)", Tscount.ToString, UDTkey, "<新增:0 row ,錯誤>", ex.Message}
			ErrFmtLog(Pr)
 
		End Try

	End Function


	'Private disposedValue As Boolean = False		' 偵測多餘的呼叫
	'' IDisposable
	'Protected Overridable Sub Dispose(ByVal disposing As Boolean)
	'	If Not Me.disposedValue Then
	'		If disposing Then
	'			' TODO: 釋放大物件位置 (Managed 物件)。

	'		End If
	'		' TODO: 釋放您自己的狀態 (Unmanaged 物件)
	'		' TODO: 將大型欄位設定為 null。
	'	End If
	'	Me.disposedValue = True
	'End Sub

	'#Region " IDisposable Support "
	'	' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
	'	Public Sub Dispose() Implements IDisposable.Dispose
	'		' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (ByVal 視為布林值處置)。
	'		Dispose(True)
	'		GC.SuppressFinalize(Me)
	'	End Sub
	'#End Region


End Class
