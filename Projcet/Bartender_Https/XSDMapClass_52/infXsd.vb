'==================================================================
' 使用XSD 檔取代 EDIFMap 的 屬性定義及功能擴充方式
' 依SOAP 的xsd:element 的基礎 如需要架構Web SOAP 時XSD基層一致 。  
' 1 .來說明中繼表的Maping及目的數據表位置
' 2. 中繼表與目的表defail ckeck; 擴充性功能
'版本:<20200725-SQL2016>1750
' 可將CLR  建至在測試環境 ，環境設訂於DBConCls 
' 
'================================================================
Option Strict Off
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.IO
Imports System.Text
'Imports System.Security
'Imports System.Security.Cryptography
Imports Microsoft.VisualBasic.FileIO
Imports System.Xml
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Linq
Imports <xmlns:xsd="http://www.w3.org/2001/XMLSchema">
Imports <xmlns:xs="http://www.w3.org/2001/XMLSchema">
Imports <xmlns:sql="urn:schemas-microsoft-com:mapping-schema">


<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")>
Public Class infXsdFun
	'Shared Fcoding, FieldParser, INFType, FileType, ReOK_Value, impsFile As String
	'Shared CommandText, CheckFun, SQLBulkCk, JOBQsvr, Stkey, InfKey, EDLLocDr As String
	'Shared Extkeys() As String
	'Shared tscount As Integer = 0
	'Shared XDoc As XDocument = New XDocument()
	'Shared XApi As IEnumerable(Of XElement)
	'Shared Vxsd As IEnumerable(Of XElement)
	'Shared MapInf As IEnumerable(Of MapingBox)
	'Public Shared Mds As New DataSet
	'*********************** SQL CLR 進入點*************
	'======匯入數據 : INFType(檔案類型) import  ======== 
	' XML/TXT 轉入DataSet >> 依 XSD Mapping SQL  
	' ikey=接口ID，iFpath=wkey
	'==============================
	<Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)>
	Public Shared Function XsdBulkCopy(ByVal infkey As SqlString, ByVal iwkey As SqlString, ByVal Stkeys As SqlString, ByVal InfXSD As SqlString) As SqlString
		Dim Inxsd As XsdCls = New XsdCls(infkey.Value, iwkey.Value, Stkeys.Value, InfXSD.Value)
		Dim impsFile As String = Inxsd.impsFile

		Dim Rtcode As String = ""
		Dim rcount As Integer = 0
		Dim tscountCk As Integer = 0
 
		Dim setV As String = ""
		Dim tscount As Integer = 0
		Dim ReOK_Value As String = ""
 
		If Inxsd.ClStatus <> "OK" Then
			Return CType("<XSD 設定> XsdCls 建立失敗 !>> ", SqlString)
			Exit Function
		End If

		If Inxsd.MapInf Is Nothing Then
			Return CType("<<XSD 設定>>Not Elements Mapping  !>> ", SqlString)
			Exit Function
		End If
		If FileIO.FileSystem.FileExists(Inxsd.impsFile) = False Then
			Return CType("<<讀取失敗>> 來源檔不存在:" + Inxsd.impsFile, SqlString)
			Exit Function
		End If
 
		'====建表 +讀檔
		Try
			If Inxsd.FileType.ToUpper = "TXT" Then
				Rtcode = Inxsd.CrEdiDsFmt
				If Rtcode = "Schema OK!" Then
					If Inxsd.Mds.Tables.Count = 1 Then rcount = Inxsd.readTxtFile2Table(impsFile)
					If Inxsd.Mds.Tables.Count > 1 Then rcount = Inxsd.readTxtFile2Ds(impsFile)
				Else
					XsdBulkCopy = CType(Inxsd.ClStatus, SqlString)
				End If
			End If
			If Inxsd.FileType.ToUpper = "XML" Then
				If Inxsd.INFType.ToUpper = "IMPORT" Then rcount = Inxsd.readXmlFile()
				If Inxsd.INFType.ToUpper = "TABLEMERGE" Then rcount = Inxsd.readXmlFile(1)
			End If

		Catch ex As Exception
			Dim Pr() As String = {"XsdBulkCopy(...)", impsFile, Inxsd.InfKey, "<1.數據結構解析錯誤>", ex.Message}
			ReOK_Value = Inxsd.ErrFmtLog(Pr)
			Return CType(ReOK_Value, SqlString)
		End Try

		' 回傳 dataTable 內容
		If rcount > 0 Then
			Try
				If Inxsd.INFType.ToUpper = "IMPORT" Then
					tscount = Inxsd.XsdMapCopy()
					ReOK_Value = "<<XSD讀取 \N 筆，新增 \X 筆 TO " & Inxsd.SQLBulkCk & " !>>OK :"
				End If
				' 直接傳 dataSet(XML) 回SQLDB 使用UDT 收數據  
				If Inxsd.INFType.ToUpper = "TABLEMERGE" And Inxsd.CommandText <> "" Then
					Inxsd.CommandText = Inxsd.CommandText.Replace("@Extkey", infkey.Value & "," & Inxsd.StKey & "," & Inxsd.impsFile)
					tscount = Inxsd.XsdMergeUDT(Inxsd.CommandText, Inxsd.InfKey, 1, Inxsd.Mds)
					If tscount > 0 Then Rtcode = "OK !"
					ReOK_Value = "<<XSD讀取 \N 筆，新增 \X 筆 TO " & Inxsd.SQLBulkCk & " !>>OK :"
				End If
			Catch ex As Exception
				Dim Pr() As String = {"XsdBulkCopy(...)", impsFile, Inxsd.InfKey, "<2.[新增] 錯誤Raines test>", ex.Message}
				ReOK_Value = Inxsd.ErrFmtLog(Pr)
				Return CType(ReOK_Value, SqlString)

			End Try
		Else
			ReOK_Value += "	<讀取 readFile> & <數據Maping解析> 錯誤 "
		End If
		'' 確認 impsFile 完成筆數在暫存
		Try
			' 如 ODMTyps 有質指示更新，但未回處理筆數時再補查一次 
			If Inxsd.SQLBulkCk <> "" Then
				tscountCk = DBConCls.ExcuteCount("SELECT count(*) from " + Inxsd.SQLBulkCk + " where etyps=0 and sfile='" + impsFile + "'")
				ReOK_Value += tscountCk.ToString
			Else
				ReOK_Value += tscount.ToString
			End If

			' 依ODMTyps Commmand 要更新暫存表狀態
			If Inxsd.ODMTyps <> "" And Inxsd.ODMTyps <> "NA" Then
				If Inxsd.ODMTyps.IndexOf("@wkey") > -1 Then Inxsd.ODMTyps = Inxsd.ODMTyps.Replace("@wkey", impsFile).Replace("@tscount", tscount)
				DBConCls.ExcuteSql(Inxsd.ODMTyps, DBConCls.CONNs)
			End If

			''20200911 取消 If tscountCk > tscount Then tscount = tscountCk
			ReOK_Value = ReOK_Value.Replace("\N", rcount)
			ReOK_Value = ReOK_Value.Replace("\X", tscount)

			''回傳 錯誤內容 暫無法傳到前端，改使用XML 記錄錯誤 
			'If tscount <= 0 Then
			'	Mds.Tables(0).Rows(0)("sfile") = ReOK_Value
			'	Mds.WriteXml(impsFile.ToString.Replace(".XML", "") & "-內容錯誤協助.XML")
			'	ReOK_Value += "<2. 大量CopyToSQL 筆數錯誤:" & tscount & ">保留來源檔:" & impsFile
			'End If
			'如需提供<JOBQTyps> 提供順便寫回JOBQ結果 
			If Inxsd.JOBQTyps <> "" Then
				If tscount > 0 Then
					setV = ",Rdo=7 ,SYS_USER='XsdBulkCopy', rcount=" + rcount.ToString
				Else
					setV = ",Rdo=-4" + ",SERR='" + ReOK_Value + "',SYS_USER='XsdBulkCopy',rcount=" + rcount.ToString
				End If
				Inxsd.JobQSvrCmd(Inxsd.JOBQTyps, iwkey.Value, setV, "and Rdo<9")
			End If
			''接口匯入指定執行    BuName
			If Inxsd.BuName <> "" And Inxsd.BuName <> "NA" Then
				DBConCls.ExcuteSql(Inxsd.BuName, DBConCls.CONNs)
			End If
			' 補執行
			If Inxsd.CommandText <> "" And Inxsd.CommandText <> "NA" Then
				DBConCls.ExcuteSql(Inxsd.CommandText, DBConCls.CONNs)
			End If
		Catch ex As Exception
			Dim Pr() As String = {"XsdBulkCopy(...)", Stkeys.Value, ReOK_Value, "<3.XsdBulkCopy(登記)>", ex.Message}
			ReOK_Value = Inxsd.ErrFmtLog(Pr)
			'DBConCls.ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0,-15}	{1,-20}	{2,20}	{3,50}	\ErrMessage:" + ex.Message & vbCrLf
			'FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(DBConCls.ExcMag, "3.XsdBulkCopy(登記)", iwkey.Value, "", ReOK_Value), True)
		End Try

		XsdBulkCopy = CType(ReOK_Value, SqlString)
	End Function

	'======匯出數據 : INFType=export ===============
	'來源依 XSD 內CommandText  ，目的類型: txt、xml、SQLDB
	' <CONNs> 來源 ; 依 <CommandText> 產生來源數據 
	'轉入並依類型 : XML/TXT 轉出檔案
	' 如有XSD有設 Schema 則使用(MapInf)；無則用 query 來源
	'============================ 
	<Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)>
	Public Shared Function XsdQueryExp(ByVal ikey As SqlString, ByVal wkey As SqlString, ByVal Extkeys As SqlString, ByVal InfXSD As SqlString) As SqlString
		Dim Outxsd As XsdCls = New XsdCls(ikey.Value, wkey.Value, Extkeys.Value, InfXSD.Value)
		'Dim Mds As New DataSet
		Dim Filename As String = Outxsd.FileFmtname(ikey.Value, Extkeys.Value, wkey.Value)
		Dim Despath As String = Outxsd.EDLLocDr
		Dim Rtcode As String = ""
		Dim rcount As Integer = 0
		Dim ReOK_Value As String = ""
		Dim tscount As Integer = 0
		Try
			' 是否設定 "OK"
			If Outxsd.ClStatus <> "OK" Then
				ReOK_Value = InfXSD.Value + "<<infxsd New 設定錯誤>> " + Outxsd.ClStatus
				Return CType(ReOK_Value, SqlString)
				Outxsd.Dispose()
				Exit Function
			End If
			''檔名規則
			If Filename.IndexOf("\") > 0 Then Despath = ""
			'' TXT 格式  
			If Outxsd.FileType.ToUpper = "TXT" Then
				If (Outxsd.FieldParser = "") Then
					ReOK_Value = InfXSD.Value + "<<XSD appinfo 錯誤>> TXT 類型 FieldParser 組合設定錯誤 !  "
					Return CType(ReOK_Value, SqlString)
				End If
			End If

			'And  MapInf Is Nothing
			'If (Outxsd.FileType.ToUpper = "XML" And Outxsd.MapInf Is Nothing) Then
			'	ReOK_Value = InfXSD.Value + "<<XSD appinfo 錯誤>> XML 類型 xsd:schema element 組合設定錯誤 !  " + Outxsd.XApi.Value
			'	Return CType(ReOK_Value, SqlString)
			'End If
		Catch ex As Exception
			Return CType("<<XSD.appinfo Load 失敗 !>> " + InfXSD.Value + " ErrMsg:" + ex.Message, SqlString)
		End Try

		Try
			' 統一格式參數，如需自定請於XSD 指示sqlobject及參數
			Outxsd.CommandText = Outxsd.CommandText.Replace("@wkey", wkey.Value).Replace("@ExtkeyS", Extkeys.Value.ToString)

			ReOK_Value = "<<讀取 \R 筆，匯出 \X 筆 TO " + Filename + " !>>Tag:\X "
			' TXT
			If Outxsd.FileType.ToUpper = "TXT" Then
				' Mds.ReadXmlSchema(InfXSD) 
				'Mds = DBConCls.sqlTmpDs(CommandText)
				rcount = DBConCls.sqlTmpDs(Outxsd.CommandText, Outxsd.Mds)

				If rcount > 0 Then
					If Outxsd.FieldParser = "FixedWidth" Then
						tscount = Outxsd.WrFileFixWidth(Despath + Filename, Outxsd.Fcoding)
					Else
						Outxsd.FieldParser = IIf(IsNumeric(Outxsd.FieldParser) = True, ChrW(Outxsd.FieldParser), Outxsd.FieldParser)
						tscount = Outxsd.DatasetWrTxtFile(Despath + Filename, Outxsd.FieldParser, Outxsd.Fcoding)
					End If
				End If

			End If
			'' DBtoDB 直接Call SP 執行
			If Outxsd.FileType.ToUpper = "SQLSP" Then
				Try
					Outxsd.Mds = DBConCls.sqlTmpDs(Outxsd.CommandText)
					Rtcode = Outxsd.XsdMapCopy()
				Catch ex As Exception
					ReOK_Value += " [SQLDB] 交易:" & Rtcode & " 錯誤 Message:" & ex.Message.ToString
					FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
				End Try
			End If
			' 匯入 DataSet 再轉出XML
			' 讀取XSD 建置暫存結構 ； SQL Query 欄位數需求要精準、避免DS重建結構結構
			If Outxsd.FileType.ToUpper = "XML" Then
				Outxsd.Mds.ReadXmlSchema(InfXSD)

				Dim TDs As DataSet = DBConCls.sqlTmpDs(Outxsd.CommandText)
				For Each T As DataTable In TDs.Tables
					rcount += T.Rows.Count
				Next
				tscount = Outxsd.DstoDs(TDs)
				'sqlXSDs(CmdText, XDoc, Mds)
				If tscount > 0 Then
					Outxsd.Mds.WriteXml(Despath + Filename)
				End If
			End If
			' SQL ForXml  格式轉存:
			' 驗證SQLXML 是否格式正常；筆數 
			If Outxsd.FileType.ToUpper = "FORXML" Then
				ReOK_Value = "<<讀取 \R 筆，ForXml 匯出 \X 筆 TO " + Filename + "!>>Tag:\X "
				Dim XmlRW As String = DBConCls.SQLXmlRW(Outxsd.CommandText, Despath + Filename, rcount)
				If rcount > 0 Then
					tscount = rcount
				Else
					ReOK_Value = XmlRW
				End If
			End If
			''' ===========
			If Outxsd.FileType.ToUpper = "FORXMLzp" Then
				ReOK_Value = "<<讀取 \R 筆，ForXml 匯出 \X 筆 TO " + Filename + "!>>Tag:\X "
				Dim XmlRW As String = DBConCls.SQLXmlzpRW(Outxsd.CommandText, Despath + Filename, rcount, True)
				If rcount > 0 Then
					tscount = rcount
				Else
					ReOK_Value = XmlRW
				End If
			End If
			'ReOK_Value = "<<讀取 \R 筆，匯出 \X 筆 TO " + Filename + " !>>Tag:\X "
			'==========客製轉出檔案
			' 7-11 ; cvs
			If Outxsd.FileType = "DsoFmtCK" Then
				ReOK_Value = DsoFmtCK(ikey, wkey, Outxsd.StKey, InfXSD, Outxsd)
			End If
			If Outxsd.FileType = "DsoFmt7_11" Then
				ReOK_Value = DsoFmt7_11(ikey, wkey, Outxsd.StKey, InfXSD, Outxsd)
			End If
			If Outxsd.FileType = "DsoFmtCVS" Then
				ReOK_Value = DsoFmtCVS(ikey, wkey, Outxsd.StKey, InfXSD, Outxsd)
			End If
		Catch ex As Exception
			Dim Pr() As String = {"XsdQueryExp(...)", ikey.Value & wkey.Value, ReOK_Value, "<2.EXP Data to File 錯誤>", ex.Message}
			ReOK_Value = Outxsd.ErrFmtLog(Pr)
		End Try

		ReOK_Value = ReOK_Value.Replace("\R", rcount).Replace("\X ", tscount).Replace("Tag", "OK")
		If tscount > 0 Then ReOK_Value = ReOK_Value.Replace("Tag", "OK")


		' ======XSD inf 排程Wkey 狀態更新，ODM 訂單TYPE 更新
		Try
			'JOBQ 排程協助 JOBQTyps直接給指令
			If Outxsd.JOBQTyps <> "" And tscount > 0 Then
				If wkey.Value.IndexOf(".") > -1 Then
					Outxsd.JobQSvrCmd(Outxsd.JOBQTyps, Filename, "", "")
				Else
					Outxsd.JobQSvrCmd(Outxsd.JOBQTyps, wkey.Value, "", "")
				End If
			End If
			''================
			''  更新匯出來源資料表的狀態值 
			''================
			If Outxsd.ODMTyps <> "" And tscount > 0 Then
				'If ODMTyps.IndexOf("@wkey") > -1 Then ODMTyps = ODMTyps.Replace("@wkey", wkey.Value)

				If Outxsd.Extkeys.Length > 0 Then
					If Outxsd.ODMTyps.IndexOf("@Par1") > -1 Then Outxsd.ODMTyps = Outxsd.ODMTyps.Replace("@Client_code", Outxsd.Extkeys(0))
					If Outxsd.Extkeys.Length > 2 Then Outxsd.ODMTyps = Outxsd.ODMTyps.Replace("@odkey", Outxsd.Extkeys(1))
				End If

				DBConCls.ExpOKSP(Outxsd.ODMTyps, Extkeys)
			End If
		Catch ex As Exception
			Dim Pr() As String = {"XsdQueryExp(...)", ikey.Value & wkey.Value, "<3.XsdBulkCopy(登記JOBQ)>", ex.Message}
			ReOK_Value = Outxsd.ErrFmtLog(Pr)
		End Try

		XsdQueryExp = CType(ReOK_Value, SqlString)
	End Function

	'==================================
	' 傳回表集內容於SQL server 檢視
	' SqlContext.Pipe 無法參考設定為物件的執行個體  ByVal MidTab As DataTable
	'===================================
	<Microsoft.SqlServer.Server.SqlProcedure()>
	Public Shared Sub EDIinfxSDView(ByVal ikey As SqlString, ByVal iwkey As SqlString, ByVal iStkey As SqlString, ByVal InfXSD As SqlString)
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(DBConCls.EDIconstr)
		Dim Inxsd As XsdCls = New XsdCls(ikey.Value, iwkey.Value, iStkey.Value, InfXSD.Value)
		Dim impsFile As String = Inxsd.impsFile

		Dim Rtcode As String = ""
		Dim rcount As Integer = 0
		Dim ReOK_Value As String = ""
		Try
			'===參數值檢查
			If Inxsd.FileType = "txt" And Inxsd.INFType = "Import" And (Inxsd.FieldParser = "" Or Inxsd.SQLBulkCk = "") Then
				ReOK_Value = String.Format("<<XSD設定 appinfo: 設定未完成 >>{0},{1},{2}。", Inxsd.FieldParser, Inxsd.SQLBulkCk, InfXSD.Value)
			End If

			' 是否設定Schema element
			If Inxsd.XApi.Elements.Count <= 0 Then
				ReOK_Value = "<<XSD 設定Not Elements Mapping  !>> "
				'Exit Sub
			End If
			If FileIO.FileSystem.FileExists(InfXSD.Value) = False Then
				ReOK_Value = "<<XSD檔 不存在:" + InfXSD.Value
				'Exit Sub
			End If
			If FileIO.FileSystem.FileExists(impsFile) = False Then
				ReOK_Value = "<<讀取來源檔失敗 !不存在:" + impsFile
				'Exit Sub
			End If
			If Inxsd.ClStatus <> "OK" Then
				ReOK_Value = "<XSD 設定> XsdCls 建立失敗 !>> "
			End If
		Catch ex As Exception
			ReOK_Value += "<<讀取XSD Load失敗" + InfXSD.Value + ">>" + ex.Message
		End Try


		'====建表 +讀檔
		Try
			If Inxsd.FileType.ToUpper = "TXT" Then
				Rtcode = Inxsd.CrEdiDsFmt()
				If Rtcode = "Schema OK!" Then
					If Inxsd.Mds.Tables.Count = 1 Then rcount = Inxsd.readTxtFile2Table(impsFile)
					If Inxsd.Mds.Tables.Count > 1 Then rcount = Inxsd.readTxtFile2Ds(impsFile)
				Else
					ReOK_Value += Rtcode
				End If
			End If
			If Inxsd.FileType.ToUpper = "XML" Then
				If FileIO.FileSystem.FileExists(impsFile) Then
					If Inxsd.INFType.ToUpper = "IMPORT" Then rcount = Inxsd.readXmlFile()
					If Inxsd.INFType.ToUpper = "TABLEMERGE" Then rcount = Inxsd.readXmlFile(1)
				End If
			End If

		Catch ex As Exception
			Dim Pr() As String = {"EDIinfxSDView(...)", iwkey.Value + "\" + ikey.Value, ReOK_Value, "<3.EDIinfxSDView >", ex.Message}
			ReOK_Value = Inxsd.ErrFmtLog(Pr)
			If Inxsd.Mds.Tables Is Nothing Then
				Inxsd.Mds.Tables.Add("ERROR")
				Inxsd.Mds.Tables(0).Columns.Add("ErrMSg", Type.GetType("System.String"))
			End If
			Inxsd.Mds.Tables(0).Rows.Add({ReOK_Value})

		End Try

		If rcount > 0 Then ReOK_Value = rcount.ToString + " 筆 XSD 結構傳送 SQL 測試 !"
		For i As Integer = 0 To Inxsd.Mds.Tables.Count - 1
			XsdTabView(Inxsd.Mds.Tables(i), ReOK_Value)
		Next
		''''<0201122> 192.168.1.121測試卸載
		Inxsd.Dispose()



	End Sub

	'==================================
	' 傳回表集內容於SQL server 檢視
	' SqlContext.Pipe 無法參考設定為物件的執行個體  (可以不移動)
	'===================================
	Public Shared Sub XsdTabView(ByVal MidTab As DataTable, ByRef Okstr As String)
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(DBConCls.EDIconstr)
		Dim TmpResult(MidTab.Columns.Count - 1) As SqlMetaData
		Dim pipe As SqlPipe = SqlContext.Pipe

		Try
			'Using conn As New SqlConnection("context connection=true")
			'  conn.Open()
			'  Dim Hcmd As SqlCommand = New SqlCommand("select 'Client Err Table List'")
			For idx As Integer = 0 To MidTab.Columns.Count - 1
				Dim column As DataColumn = MidTab.Columns(idx)
				TmpResult(idx) = SqlColumnType(column)
			Next
			Dim record As New SqlDataRecord(TmpResult)
			pipe.SendResultsStart(record)

			Dim rowX As DataRow = Nothing
			Dim colVal As Object = Nothing
			For r As Integer = 0 To MidTab.Rows.Count - 1
				rowX = MidTab.Rows(r)
				For f As Integer = 0 To record.FieldCount - 1
					colVal = rowX(f)
					record.SetValue(f, colVal)
				Next
				pipe.SendResultsRow(record)
			Next
			'End Using
		Catch ex As Exception
			Okstr += String.Format(" 來源參考[EDIinf.XsdTabView] :{0} ", ex.Message)
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, Okstr + vbCrLf, True)

			con.Close()
			con.Dispose()
		Finally
			pipe.SendResultsEnd()
			pipe.Send(Okstr)
			con.Close()
			con.Dispose()
		End Try
	End Sub


	'***********************************
	Public Class MapingBox
		Public Tfield As String             ''''XSD 文件結構欄位名稱
		Public Sqlfield As String           ''''T-SQL Query  欄位名稱
		Public item As Integer              ''''順序
		Public fixed As Integer     '''' 文件 inBound/outBound 欄位長。 fixed
		Public SourceDt As String       '''' 來源表DataTable 
		Public DecDt As String          '''' SQL 表 Table 
		Public PadRL As String          ''''''''' Text 靠左靠右 。 
	End Class
	' 分析自訂 XSD 與 Query DataTable  差異   分析
	'Public Class XsdAnysDs
	'	Public Dname As String
	'	Public Sname As String
	'	Public Qname As String
	'	Public xdIts As Integer
	'	Public MapCk As String
	'	Public refkey As String
	'	Public recnt As Integer
	'End Class
	'========建立 Mapinf  ========================= 
	'依XSD文件建立 Mapinf 集合資訊 (未Maping)
	'======================================== 
	'Public Shared Sub Create_Mapinf(ByVal XDoc1 As XDocument)
	'	Dim colid As Integer = 0
	'	Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
	'	'Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing AndAlso el.@sql:relation <> ""
	'	'		   Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}

	'	For Each vt In Vxsd
	'		colid = 1
	'		MapInf = From el2 In (From el In XDoc1.Descendants().<xsd:element> Where el.@name = vt.name
	'							  Select el).Descendants().<xsd:element>
	'				 Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
	'				 Select New MapingBox With {.Tfield = el2.@name, .Sqlfield = IIf(el2.@sql:field Is Nothing, vt.name, el2.@sql:field), .fixed = IIf(el2.@fixed Is Nothing, 0, el2.@fixed),
	'		   .item = colid, .SourceDt = vt.name, .DecDt = vt.relation, .PadRL = IIf(el2.@PadRL Is Nothing, 0, el2.@PadRL)}
	'		For Each elx In MapInf
	'			colid += 1
	'			'Console.WriteLine("{0},{1},{2}", elx.Tfield, elx.Sqlfield, elx.item)
	'		Next
	'	Next
	'End Sub





	'====XmlDocument 轉DataSet==== 
	'Public Overloads Shared Function DsReadXmldoc(ByVal sfile As String, ByRef MidDs As DataSet) As Integer
	'	Dim rcnt As Integer = 0
	'	Try
	'		''新建XML文件類別
	'		Dim Xmldoc As New XmlDocument()
	'		''從指定的字串載入XML文件
	'		Xmldoc.LoadXml(sfile)
	'		'建立此物件，並輸入透過StringReader讀取Xmldoc中的Xmldoc字串輸出
	'		Dim Xrd As XmlReader = XmlReader.Create(New System.IO.StringReader(Xmldoc.OuterXml))
	'		MidDs.ReadXml(Xrd)
	'		' 系統需求欄補建
	'		For Each cTab As DataTable In MidDs.Tables
	'			If cTab.Columns.Contains("sfile") = False Then cTab.Columns.Add("sfile", Type.GetType("System.String"))
	'			If cTab.Columns.Contains("edate") = False Then cTab.Columns.Add("edate", Type.GetType("System.DateTime"))
	'			If cTab.Columns.Contains("etyps") = False Then cTab.Columns.Add("etyps", Type.GetType("System.Int32"))
	'			If cTab.Columns.Contains("clientcode") = False Then cTab.Columns.Add("clientcode", Type.GetType("System.String"))
	'			For Each dr As DataRow In cTab.Rows
	'				rcnt += 1
	'				dr("sfile") = sfile
	'				dr("edate") = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
	'				dr("etyps") = 0
	'				dr("clientcode") = Stkey
	'			Next
	'		Next

	'		DsReadXmldoc = rcnt
	'	Catch ex As Exception
	'		DsReadXmldoc = 0
	'		DBConCls.ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0,-20}	{1,-20}	{2,20}	{3,50}	\ErrMessage:" + ex.Message & vbCrLf
	'		FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(DBConCls.ExcMag, "DsReadXml", sfile, "", "<Ds數據Read XmlDoc解析錯誤>"), True)
	'	End Try

	'End Function
	''==== 序列轉DataSet====
	'Public Shared Function DsXmlSerializ(ByVal sfile As String, ByRef MidDs As DataSet) As Integer
	'	Dim rcnt As Integer = 0
	'	''新建XML 類別
	'	Dim xmlSerializer As XmlSerializer = New XmlSerializer(GetType(DataSet))
	'	Dim readStream As FileStream = New FileStream(sfile, FileMode.Open)
	'	Try
	'		MidDs = CType(xmlSerializer.Deserialize(readStream), DataSet)
	'		readStream.Close()
	'		' 系統需求欄補建
	'		For Each cTab As DataTable In MidDs.Tables
	'			If cTab.Columns.Contains("sfile") = False Then cTab.Columns.Add("sfile", Type.GetType("System.String"))
	'			If cTab.Columns.Contains("edate") = False Then cTab.Columns.Add("edate", Type.GetType("System.DateTime"))
	'			If cTab.Columns.Contains("etyps") = False Then cTab.Columns.Add("etyps", Type.GetType("System.Int32"))
	'			If cTab.Columns.Contains("clientcode") = False Then cTab.Columns.Add("clientcode", Type.GetType("System.String"))
	'			For Each dr As DataRow In cTab.Rows
	'				rcnt += 1
	'				dr("sfile") = sfile
	'				dr("edate") = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
	'				dr("etyps") = 0
	'				dr("clientcode") = Stkey
	'			Next
	'		Next
	'		DsXmlSerializ = rcnt
	'	Catch ex As Exception
	'		DsXmlSerializ = 0
	'		DBConCls.ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0,-20}	{1,-20}	{2,20}	{3,50}	\ErrMessage:" + ex.Message & vbCrLf
	'		FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(DBConCls.ExcMag, "readXmlFile1", sfile, "", "<Xml Deserialize 反序列解析DataSet錯誤>"), True)
	'	End Try
	'End Function







	'================================='*********** 轉出文件區塊 ********************
	'========= 依XSD 進行數據轉換 (矩陣格式)=====
	'範例 : OK  配送單文件檔 'CirCleK
	'================================== 
	''  <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
	'Public Shared Function DsoFmtCK(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String, ByVal XApi As IEnumerable(Of XElement)) As String
	Public Shared Function DsoFmtCK(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String, ByVal Xsd As XsdCls) As String
		Dim DT As DataTable
		Dim FileName, FilePath, Head, Body, Tail As String
		Dim FileNameP, FilePathP, HeadP, BodyP, TailP As String
		Dim count As String
		Dim its As Integer
		Dim ReOK_Value As String
		ReOK_Value = "<<轉出 \N 筆 Form  CKDsoFmt!" + ikey.ToString + ">>"
		'*** 指定目的表的[連接]及[表][欄位資訊]
		Try
			'XDoc = XDocument.Load(infXSD)
			'XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el
			'====取得: xsd:appinf 參數
			'DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
			'CommandText = XApi.<CommandText>.FirstOrDefault.Value
			If DBConCls.EDIconstr = "" Then
				ReOK_Value += "<<XSD設定 appinfo: 設定未完成CONNs>> " + infXSD
				Return CType(ReOK_Value, SqlString)
			End If
		Catch ex As Exception
			Return "<<讀取XSD Load失敗" + infXSD + ">>" + ex.Message
			Exit Function
		End Try
		'JOBQsvr = XApi.<EDIinf>.@JOBQsvr
		FilePath = Xsd.EDLLocDr
		FilePathP = Xsd.EDLLocDr

		FileName = "CVS" + sfile + ".F03"
		FileNameP = "DOK" + sfile + ".P03"
		'Dim PCname As String

		DT = DBConCls.sqlTmpData(Xsd.CommandText)
		count = DT.Rows.Count
		If count = 0 Then
			DsoFmtCK = ReOK_Value.Replace("\X", count)
			Exit Function
		End If
		' F03
		' @Head 
		Try
			Head = "1"  'RDFMT
			Head += "D69" 'SNCD
			HeadP = Head + DateTime.Now.ToString("yyyyMMdd").PadRight(60, "0")  'RDFMT+SNCD+PRDT+P/H
			Head += DateTime.Now.ToString("yyyyMMdd").PadRight(162, "0")  'PRDT+FIL

			Head += vbCrLf  'CRLF
			HeadP += vbCrLf 'Crlf

			For i As Integer = 0 To count - 1
				' F03   Body
				Body += "2"  'RDFMT
				Body += "169"   'ECNO
				Body += "TOK"   'CNNO
				Body += RTrim(DT.Rows(i).Item("STNO").ToString) 'STNO
				Body += RTrim(DT.Rows(i).Item("STNM").ToString).PadRight(18 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("STNM").ToString)).Length - RTrim(DT.Rows(i).Item("STNM").ToString).Length)) 'STNM
				Body += RTrim(DT.Rows(i).Item("odkey").ToString)    'ODNO
				Body += RTrim(DT.Rows(i).Item("dpay").ToString).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("dpay").ToString)).Length - RTrim(DT.Rows(i).Item("dpay").ToString).Length), "0") 'AMT
				Body += RTrim(DT.Rows(i).Item("down").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("down").ToString)).Length - RTrim(DT.Rows(i).Item("down").ToString).Length)) 'CUTKNM
				Body += " ".PadLeft(20) 'CUTKTL
				Body += "     " 'DCRONO
				Body += "D69"   'EDCNO
				Body += "0" 'PRODNM

				Body += "誠品網路書店 www.eslite.com             " 'ECWEB
				Body += "(02)8789-8921       " 'ECSERTEL
				Body += RTrim(DT.Rows(i).Item("tots")).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("tots").ToString)).Length - RTrim(DT.Rows(i).Item("tots").ToString).Length), "0")   'REALAMT
				Body += "3"  'TRADETYPE
				Body += "963"   'SERCODE
				Body += vbCrLf  'CRLF
				' P03   Body
				BodyP += "2"    'RDFMT
				BodyP += "169"  'ECNO
				BodyP += "TOK"  'CNNO
				BodyP += RTrim(DT.Rows(i).Item("odkey").ToString)    'ODNO
				BodyP += RTrim(DT.Rows(i).Item("down").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("down").ToString)).Length - RTrim(DT.Rows(i).Item("down").ToString).Length)) 'CUTKNM
				BodyP += RTrim(DT.Rows(i).Item("dtel").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("dtel").ToString)).Length - RTrim(DT.Rows(i).Item("dtel").ToString).Length)) 'CUTKTL
				BodyP += RTrim(Left(DT.Rows(i).Item("Weight").ToString, 5)).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("Weight").ToString)).Length - RTrim(DT.Rows(i).Item("Weight").ToString).Length), "0") 'Weight
				BodyP += RTrim(DT.Rows(i).Item("PackType").ToString).PadRight(1 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("PackType").ToString)).Length - RTrim(DT.Rows(i).Item("PackType").ToString).Length)) 'Packtype
				BodyP += vbCrLf
				its += 1
			Next i

			' @Tail
			Tail = "3"  'RDFMT
			Tail += count.PadLeft(8, "0") 'RDCNT
			'Tail += DT.Compute("SUM(dpay)", "").PadRight(9 - System.Text.Encoding.Default.GetBytes(DT.Compute("SUM(dpay)", "")).Length - RTrim(DT.Compute("SUM(dpay)", "").ToString.Length)) 'AMT
			TailP += Tail.PadRight(64, "0")

			Tail += DT.Compute("SUM(dpay)", "").ToString.PadRight(7, "0") 'AMT
			Tail += "".PadRight(150, "0")  ' FIL2
			Tail += vbCrLf  'CRLF

			TailP += vbCrLf
			Head = Head + Body + Tail
			HeadP = HeadP + BodyP + TailP
			' 【2】. 將步驟1.的訂單資料寫入F03 , P03檔案
			FileIO.FileSystem.WriteAllText((FilePath + FileName), Head, False, Encoding.GetEncoding(950))
			FileIO.FileSystem.WriteAllText((FilePathP + FileNameP), HeadP, False, Encoding.GetEncoding(950))
			ReOK_Value = ReOK_Value.Replace("\N", its) + " Export:" + FileName + "/" + FileNameP

		Catch ex As Exception
			ReOK_Value += " [寫入F03 , P03檔案]: " & its & "row ,錯誤 Message:" & ex.Message.ToString
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
		End Try

		DsoFmtCK = ReOK_Value
	End Function

	'========= 依XSD 進行數據轉換 =====
	'範例 : 7-11 ; DsoFmtCVS
	'================================== 
	Public Shared Function DsoFmtCVS(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String, ByVal XApi As XsdCls) As String
		Dim FilePath As String  'FileName, count
		Dim ReOK_Value As String
		Dim i As Integer
		ReOK_Value = "<<轉出 \N 筆 Form  DsoFmtCVS !" + ikey.ToString + ">>"
		'*** 指定目的表的[連接]及[表][欄位資訊]
		Try

			If DBConCls.EDIconstr = "" Then
				ReOK_Value += "<<XSD設定 appinfo: 設定未完成CONNs>> " + infXSD
				Return ReOK_Value
			End If
		Catch ex As Exception
			Return "<<讀取XSD Load失敗" + infXSD + ">>" + ex.Message
			Exit Function
		End Try
		FilePath = XApi.EDLLocDr
		Try
			Dim XMLfileR As New Xml.XmlDocument
			'Dim response As String
			Dim Wdt As New DataTable
			Wdt = DBConCls.sqlTmpData(XApi.CommandText)

			If Wdt.Rows.Count > 0 Then
				Dim XmlString As String
				XmlString = "<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "UTF-8" & Chr(34) & " ?>"
				XmlString = XmlString + "<ORDER_DOC>"
				Dim cnt As Integer
				cnt = 0
				For i = 0 To Wdt.Rows.Count - 1
					XmlString = XmlString + "<ORDER>"
					XmlString = XmlString + "<ECNO>" & RTrim(Wdt.Rows(i).Item("ECNO").ToString) & "</ECNO>"
					XmlString = XmlString + "<ODNO>" & RTrim(Wdt.Rows(i).Item("ODNO").ToString) & "</ODNO>"
					XmlString = XmlString + "<STNO>" & RTrim(Wdt.Rows(i).Item("STNO").ToString) & "</STNO>"
					XmlString = XmlString + "<AMT>" & RTrim(Wdt.Rows(i).Item("AMT").ToString) & "</AMT>"
					XmlString = XmlString + "<CUTKNM><![CDATA[" & RTrim(Wdt.Rows(i).Item("CUTKNM").ToString) & "]]></CUTKNM>"       ''取貨人中文姓名
					XmlString = XmlString + "<CUTKTL>" & RTrim(Wdt.Rows(i).Item("CUTKTL").ToString) & "</CUTKTL>"   ''收貨人手機號碼
					XmlString = XmlString + "<PRODNM>0</PRODNM>"                               ''固定=0 RTrim(Wdt.Rows(i).Item("PRODNM").ToString)
					XmlString = XmlString + "<ECWEB><![CDATA[" & RTrim(Wdt.Rows(i).Item("ECNM").ToString) & "]]></ECWEB>"        ''固定
					XmlString = XmlString + "<ECSERTEL>" & RTrim(Wdt.Rows(i).Item("ECSERTEL").ToString) & "</ECSERTEL>"      ''固定
					XmlString = XmlString + "<REALAMT>" & RTrim(Wdt.Rows(i).Item("REALAMT").ToString) & "</REALAMT>"
					XmlString = XmlString + "<TRADETYPE>" & RTrim(Wdt.Rows(i).Item("TRADETYPE").ToString) & "</TRADETYPE>"
					XmlString = XmlString + "<SERCODE>963</SERCODE>"                           ''固定
					XmlString = XmlString + "<EDCNO>D07</EDCNO>" + "</ORDER>"   '東拓D05改日翊D07
					cnt = cnt + 1
					If (cnt Mod 50 = 0) Then
						XmlString = XmlString + "<ORDERCOUNT><TOTALS>" & CStr(cnt) & "</TOTALS></ORDERCOUNT>"
						XmlString = XmlString + "</ORDER_DOC> "
						XMLfileR.LoadXml(XmlString)
						XMLfileR.Save(FilePath + sfile)
						XmlString = "<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "UTF-8" & Chr(34) & " ?>"
						XmlString = XmlString + "<ORDER_DOC>"
						cnt = 0
					End If
				Next
				If (Wdt.Rows.Count Mod 50 > 0) Then
					XmlString = XmlString + "<ORDERCOUNT><TOTALS>" & CStr(cnt) & "</TOTALS></ORDERCOUNT>"
					XmlString = XmlString + "</ORDER_DOC> "
					XMLfileR.LoadXml(XmlString)
					XMLfileR.Save(FilePath + sfile)
				End If
			End If  'Wdt.Rows.Count>0

			ReOK_Value = ReOK_Value.Replace("\N", Wdt.Rows.Count) + " Export:" + sfile
		Catch ex As Exception
			ReOK_Value += " [寫入XML檔案]: " & i & "row ,錯誤 Message:" & ex.Message.ToString
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
		End Try

		DsoFmtCVS = ReOK_Value
	End Function

	'========= 依XSD 進行數據轉換 ====== "【版本為17.02.22】 " 
	'範例 : 7-11 ;   ''711用FTP正式地址''ftp://785001:785PRUztX390a001@202.168.204.211/SIN/
	'舊版 :  exec E_CAN.dbo.ETSeven_ErrCheckSTOP  > cmd.CommandText = "select toMail from iexp..rpini where rpname = '7-11 SIN上傳'" XsdCls
	'		>  select * from dbo.ETSeven_XMLView order by Order_EshopOrderNO  
	'================================== 
	'Public Shared Function DsoFmt7_11(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String, ByVal XApi As IEnumerable(Of XElement)) As String
	Public Shared Function DsoFmt7_11(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String, ByVal XApi As XsdCls) As String
		Dim DocName As String    ''  DevStr,Body, , ToM,  SubJect
		Dim XMLfileR As New Xml.XmlDocument
		Dim FileName As String = ""
		Dim FilePath As String = ""
		Dim Wdt As New DataTable
		Dim i, cnt As Integer
		Dim ReOK_Value As String = "<<轉出 \N 筆 Form  DsoFmt7_11!" + ikey.ToString + ">>"
		'*** 指定目的表的[連接]及[表][欄位資訊]
		Try
			'XDoc = XDocument.Load(infXSD)
			'XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el
			'====取得: xsd:appinf 參數
			'DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
			'CommandText = XApi.<CommandText>.FirstOrDefault.Value
			DocName = sfile.Replace(".XML", "")
			If DBConCls.EDIconstr = "" Then
				ReOK_Value += "<<XSD設定 appinfo: 設定未完成CONNs>> " + infXSD
				Return ReOK_Value
			End If
		Catch ex As Exception
			Return "<<讀取XSD Load失敗" + infXSD + ">>" + ex.Message
			Exit Function
		End Try
		FilePath = XApi.EDLLocDr
		Try

			'抓取欲上傳的訂單 <CommandText>.Value

			Wdt = DBConCls.sqlTmpData(XApi.CommandText)
			Dim XmlString As String
			XmlString = "<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "UTF-8" & Chr(34) & " ?>"   'XML固定需要的首

			XmlString = XmlString + "<OrderDoc>"
			XmlString = XmlString + "<DocHead>"
			XmlString = XmlString + "<DocNo>" + DocName + "</DocNo>"
			XmlString = XmlString + "<DocDate>" + Now.ToString("yyyy-MM-dd") + "</DocDate>"
			XmlString = XmlString + "<ParentId>" + "785" + "</ParentId>"
			XmlString = XmlString + "</DocHead>"
			XmlString = XmlString + "<DocContent>"
			cnt = 0
			For i = 0 To Wdt.Rows.Count - 1
				XmlString = XmlString + "<Order>"
				XmlString = XmlString + "<EshopId>" & "001" & "</EshopId>"
				XmlString = XmlString + "<OPMode>" & RTrim(Wdt.Rows(i).Item("Order_OPMode").ToString) & "</OPMode>"
				XmlString = XmlString + "<EshopOrderNo>" & RTrim(Wdt.Rows(i).Item("Order_EshopOrderNo").ToString) & "</EshopOrderNo>"
				XmlString = XmlString + "<EshopOrderDate>" & RTrim(Wdt.Rows(i).Item("EshopOrderDate").ToString) & "</EshopOrderDate>"
				XmlString = XmlString + "<ServiceType>" & RTrim(Wdt.Rows(i).Item("Order_ServiceType").ToString) & "</ServiceType>"
				XmlString = XmlString + "<ShopperName>" & RTrim(Wdt.Rows(i).Item("Order_ShopperName").ToString) & "</ShopperName>"
				XmlString = XmlString + "<ShopperPhone> </ShopperPhone>" ''''''''''''''''''''''
				XmlString = XmlString + "<ShopperMobilPhone> </ShopperMobilPhone>" ''''''''''''''''''''''
				XmlString = XmlString + "<ShopperEmail> </ShopperEmail>" ''''''''''''''''''''''
				XmlString = XmlString + "<ReceiverName>" & RTrim(Wdt.Rows(i).Item("Order_ReceiverName").ToString) & "</ReceiverName>"
				XmlString = XmlString + "<ReceiverPhone> </ReceiverPhone>" ''''''''''''''''''''''
				XmlString = XmlString + "<ReceiverMobilPhone>" & RTrim(Wdt.Rows(i).Item("Order_ReceiverMobilPhone").ToString) & "</ReceiverMobilPhone>"
				XmlString = XmlString + "<ReceiverEmail> </ReceiverEmail>" ''''''''''''''''''''''
				XmlString = XmlString + "<ReceiverIDNumber> </ReceiverIDNumber>" ''''''''''''''''''''''
				XmlString = XmlString + "<OrderAmount>" & RTrim(Wdt.Rows(i).Item("Order_OrderAmount").ToString) & "</OrderAmount>"
				XmlString = XmlString + "<OrderDetail>"
				XmlString = XmlString + "<ProductId> </ProductId>" ''''''''''''''''''''''
				XmlString = XmlString + "<ProductName> </ProductName>" ''''''''''''''''''''''
				XmlString = XmlString + "<Quantity> </Quantity>" ''''''''''''''''''''''
				XmlString = XmlString + "<Unit> </Unit>" ''''''''''''''''''''''
				XmlString = XmlString + "<UnitPrice> </UnitPrice>" ''''''''''''''''''''''
				XmlString = XmlString + "</OrderDetail>"
				XmlString = XmlString + "<ShipmentDetail>"
				XmlString = XmlString + "<ShipmentNo>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_ShipmentNo").ToString) & "</ShipmentNo>"
				XmlString = XmlString + "<ShipDate>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_ShipDate").ToString) & "</ShipDate>"
				XmlString = XmlString + "<ReturnDate>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_ReturnDate").ToString) & "</ReturnDate>"
				XmlString = XmlString + "<LastShipment>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_LstShipment").ToString) & "</LastShipment>"
				XmlString = XmlString + "<ShipmentAmount>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_ShipmentAmount").ToString) & "</ShipmentAmount>"
				XmlString = XmlString + "<StoreId>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_StoreID").ToString) & "</StoreId>"
				XmlString = XmlString + "<EshopType>" & RTrim(Wdt.Rows(i).Item("ShipmentDetail_Eshop").ToString) & "</EshopType>"
				XmlString = XmlString + "</ShipmentDetail>"
				XmlString = XmlString + "</Order>"
				cnt = cnt + 1
			Next
			XmlString = XmlString + "</DocContent>"
			XmlString = XmlString + "</OrderDoc> "
			XMLfileR.PreserveWhitespace = True
			XMLfileR.LoadXml(XmlString)
			XMLfileR.Save(FilePath + sfile)

			ReOK_Value = ReOK_Value.Replace("\N", Wdt.Rows.Count) + " Export:" + sfile
		Catch ex As Exception
			ReOK_Value += " [寫入XML檔案]: " & i & "row ,錯誤 Message:" & ex.Message.ToString
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
		End Try

		DsoFmt7_11 = ReOK_Value
	End Function





	'***********       類型區塊 ********************
	'================================================
	'來源數據類型轉至SQL server 的DATA TYPE
	'=================================================
	Private Shared Function SqlColumnType(ByVal column As DataColumn) As SqlMetaData
		Dim smd As SqlMetaData = Nothing
		Dim clrType As Type = column.DataType
		Dim name As String = column.ColumnName
		Select Case Type.GetTypeCode(clrType)
			Case TypeCode.Boolean
				smd = New SqlMetaData(name, SqlDbType.Bit)
			Case TypeCode.Byte
				smd = New SqlMetaData(name, SqlDbType.TinyInt)
			Case TypeCode.Char
				smd = New SqlMetaData(name, SqlDbType.NVarChar, 1)
			Case TypeCode.DateTime
				smd = New SqlMetaData(name, SqlDbType.DateTime)
			Case TypeCode.DBNull
				Throw InvalidDataTypeCode(TypeCode.DBNull)
			Case TypeCode.Decimal
				smd = New SqlMetaData(name, SqlDbType.Decimal)
			Case TypeCode.Double
				smd = New SqlMetaData(name, SqlDbType.Float)
			Case TypeCode.Empty
				Throw InvalidDataTypeCode(TypeCode.Empty)
			Case TypeCode.Int16
				smd = New SqlMetaData(name, SqlDbType.SmallInt)
			Case TypeCode.Int32
				smd = New SqlMetaData(name, SqlDbType.Int)
			Case TypeCode.Int64
				smd = New SqlMetaData(name, SqlDbType.BigInt)
			Case TypeCode.SByte
				Throw InvalidDataTypeCode(TypeCode.SByte)
			Case TypeCode.Single
				smd = New SqlMetaData(name, SqlDbType.Real)
			Case TypeCode.String
				smd = New SqlMetaData(name, SqlDbType.NVarChar, column.MaxLength)
			Case TypeCode.UInt16
				Throw InvalidDataTypeCode(TypeCode.UInt16)
			Case TypeCode.UInt32
				Throw InvalidDataTypeCode(TypeCode.UInt32)
			Case TypeCode.UInt64
				Throw InvalidDataTypeCode(TypeCode.UInt64)
			Case TypeCode.Object
				smd = New SqlMetaData(name, SqlDbType.NVarChar, column.MaxLength)
			Case Else
				Throw UnknownDataType(clrType)
		End Select

		Return smd
	End Function
	<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="System.ArgumentException.#ctor(System.String)")>
	Private Shared Function InvalidDataTypeCode(ByVal code As TypeCode) As Exception
		Return New ArgumentException("Invalid type: " & code.ToString())
	End Function

	<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="System.ArgumentException.#ctor(System.String)")>
	Private Shared Function UnknownDataType(ByVal clrType As Type) As Exception
		Return New ArgumentException("Unknown type: " & clrType.ToString())
	End Function

	''======錯誤轉為 Record 傳回SQL =============
	Private Shared Sub ErrMsgRows(ErrMsg As SqlString)
		Dim Erecord As New SqlDataRecord(New SqlMetaData("cErrmsg", SqlDbType.NVarChar, 512))
		Erecord.SetString(0, ErrMsg)
		SqlContext.Pipe.SendResultsRow(Erecord)
		SqlContext.Pipe.SendResultsEnd()
	End Sub

	'===================================== 
	'SQL 數據類型轉至中介數據類型 
	'===================================== 
	Private Shared Function TableType(ByVal sqltype As String) As Type
		Dim smd As Type = Nothing
		Select Case sqltype
			Case "char" Or "varchar" Or "nvarchar"
				smd = GetType(String)
			Case "int" Or "bigint" Or "Long"
				smd = GetType(Int32)
			Case "DateTime" Or "DateTime"
				smd = GetType(DateTime)
			Case "Double"
				smd = GetType(System.Double)
			Case Else
				smd = GetType(String)
		End Select
		Return smd
	End Function

End Class

