'==================================================================
' 使用XSD 檔取代 EDIFMap 的 屬性定義及功能擴充方式
' 依SOAP 的xsd:element 的基礎 如需要架構Web SOAP 時XSD基層一致 。  
' 1 .來說明中繼表的Maping及目的數據表位置
' 2. 中繼表與目的表defail ckeck; 擴充性功能
'版本:<20180720>
' 可將CLR  建至在測試環境 ，環境設訂於DBConCls 
' 
'================================================================
  
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.IO
Imports System.Text
'Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.FileIO
Imports System.Xml
Imports System.Xml.Linq
Imports System.Collections.Generic
Imports System.Linq
Imports <xmlns:xsd="http://www.w3.org/2001/XMLSchema">
Imports <xmlns:xs="http://www.w3.org/2001/XMLSchema">
Imports <xmlns:sql="urn:schemas-microsoft-com:mapping-schema">


<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")> _
Public  Class infXsdFun
	Shared Fcoding, FieldParser, INFType, FileType, BuName, ReOK_Value, impsFile As String
	Shared CommandText, CheckFun, SQLBulkCk, JOBQsvr, Stkey, InfKey, EDLLocDr As String

  'Shared rcount, tscount As Integer
	Shared XDoc As XDocument = New XDocument()
	Shared XApi As IEnumerable(Of XElement)
	Shared Vxsd As IEnumerable(Of XElement)
	Shared MapInf As IEnumerable(Of MapingBox)
	'Public Shared Mds As New DataSet
'*********************** SQL CLR 進入點*************



 '======匯入數據 : INFType=import  ======== 
 ' XML/TXT 轉入DataSet >> 依 XSD Mapping SQL  
 ' ikey=接口ID，iFpath=wkey
 '==============================
 <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
 Public Shared Function XsdBulkCopy(ByVal ikey As SqlString, ByVal iwkey As SqlString, ByVal iStkey As SqlString, ByVal InfXSD As SqlString) As SqlString
	Dim Mds As New DataSet
   Dim Rtcode As String = ""
   Dim tscount As Integer = 0
   Dim rcount As Integer = 0
	Try
		XDoc = XDocument.Load(InfXSD.Value)
		XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el
	'====取得: xsd:appinf 參數
		Stkey = iStkey.Value
		InfKey = ikey.Value

		DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
		EDLLocDr = XApi.<EDIinf>.@EDLLocDr
		SQLBulkCk = XApi.<EDIinf>.@SQLBulkCk
		JOBQsvr = XApi.<EDIinf>.@JOBQsvr
		INFType = XApi.<EDIinf>.@INFType
		FileType = XApi.<FileMap>.@FileType
		FieldParser = XApi.<FileMap>.@FieldParser
		impsFile = EDLLocDr + iwkey.Value
		'===參數值檢查
		If FileType = "" Or INFType = "" Then
			ReOK_Value = "<<XSD設定 appinfo: 設定未完成>> " + InfXSD.Value
			Return CType(ReOK_Value, SqlString)
		End If
		If FileType = "txt" And INFType = "Import" And (FieldParser = "" Or SQLBulkCk = "") Then
			ReOK_Value = String.Format("<<XSD設定 appinfo: 設定未完成 >>{0},{1},{2}。", FieldParser, SQLBulkCk, InfXSD.Value)
			Return CType(ReOK_Value, SqlString)
		End If
	Catch ex As Exception
		Return CType("<<讀取XSD Load失敗" + InfXSD.Value + ">>" + ex.Message, SqlString)
		Exit Function
	End Try
	' 是否設定Schema element
	If XDoc.Descendants().<xsd:element>.Elements.Count <= 0 Then
		Return CType("<<XSD 設定Not Elements Mapping  !>> ", SqlString)
		Exit Function
	End If
	If FileIO.FileSystem.FileExists(impsFile) = False Then
	Return CType("<<讀取失敗 XML檔不存在:" + impsFile, SqlString)
		Exit Function
	End If

  '====建表及讀檔
  Try
	If FileType.ToUpper = "TXT" Then
	  Rtcode = CrEdiDsFmt(XDoc, Mds)
	  If Rtcode = "Schema OK!" Then
		rcount = readTxtFile(Mds.Tables(0), impsFile)
	  Else
		  XsdBulkCopy = CType(ReOK_Value + Rtcode, SqlString)
	  End If
	End If
	If FileType.ToUpper = "XML" Then
	  If FileIO.FileSystem.FileExists(impsFile) Then
	  rcount = readXmlFile(impsFile, "", Mds)
	End If
	End If
  Catch ex As Exception
	ReOK_Value += "<數據結構解析錯誤:" + ex.Message + " !>"
	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	Return CType("<<讀取XML Load失敗" + impsFile + ">>" + ex.Message, SqlString)
  End Try
  ' 回傳檔案
  If rcount > 0 Then
	Try
	  Rtcode = XsdMapCopy(XDoc, Mds)
	  ReOK_Value = "<<XSD讀取 \N 筆，新增 \X 筆 TO " + SQLBulkCk + " !>>" + Rtcode
	Catch ex As Exception
	  ReOK_Value += " [新增]:0 row ,錯誤 Message:" & ex.Message.ToString
	  FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	End Try
  End If
  '' 確認 impsFile 完成筆數
  Try
	tscount = DBConCls.ExcuteCount("SELECT count(*) from " + SQLBulkCk + " where sfile='" + impsFile + "'")
	ReOK_Value = ReOK_Value.Replace("\N", rcount)
	ReOK_Value = ReOK_Value.Replace("\X", tscount)
	Dim setV As String = ""
	'回傳 錯誤內容 暫無法傳到前端，改使用XML 記錄錯誤 
	If tscount > 0 Then
    'setV = "Rdo=" + IIf(rcount > 0, "4", "-1") + ",SYS_USER='XsdBulkCopy', rcount=" + rcount.ToString
    setV = "Rdo=" + IIf(tscount > 0, "9", "-1") + ",SYS_USER='XsdBulkCopy', rcount=" + rcount.ToString + ", tscount=" + tscount.ToString
	Else
	  Mds.Tables(0).Rows(0)("sfile") = Rtcode
	  Mds.WriteXml(impsFile.ToString.Replace(".", "") & "-內容錯誤協助.XML")
	  ReOK_Value += "<2. 大量CopyToSQL 筆數錯誤:" & tscount & ">保留來源檔:" & impsFile
	  setV = "Rdo=-4" + ",SERR='" + ReOK_Value + "',SYS_USER='XsdBulkCopy',rcount=" + rcount.ToString
	End If
	'如需提供<JOBQsvr> 提供順便寫回JOBQ結果 
	If JOBQsvr = "UPDATE" Then JobQSvrCmd("UPDATE", impsFile, setV, "and Rdo<9")
  Catch ex As DataException
	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("<3.JobQSvrCmd 更新JOBQ 發生錯誤描述ERR:{0} / ReOK_Value {1} ", ex.Message.ToString, ReOK_Value) + Chr(10), True)
  End Try

   XsdBulkCopy = CType(ReOK_Value, SqlString)
  End Function




 '======匯出數據 : INFType=export ===============
'來源依 XSD 內CommandText  ，目的類型: txt、xml、SQLDB
 '依 XSD 的CommandText 產生來源數據 
 '轉入並依類型 : XML/TXT 轉出檔案
 '============================ 
 <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
Public Shared Function XsdQueryExp(ByVal ikey As SqlString, ByVal wkey As SqlString, ByVal ExtkeyS As SqlString, ByVal InfXSD As SqlString) As SqlString
  Dim Mds As New DataSet
  Dim Filename As String = ""
  Dim Despath As String = ""
  Dim Rtcode As String = ""
  Dim tscount As Integer = 0
  Dim rcount As Integer = 0
  Dim Fencoding As String
  Try
    XDoc = XDocument.Load(InfXSD.Value)
    XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el

    DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
    CommandText = XApi.<CommandText>.FirstOrDefault.Value
    Despath = XApi.<EDIinf>.@EDLLocDr
    INFType = XApi.<EDIinf>.@INFType
    FileType = XApi.<FileMap>.@FileType
    FieldParser = XApi.<FileMap>.@FieldParser
    Fencoding = XApi.<FileMap>.@Encoding
    If INFType.ToUpper <> "EXPORT" Or FileType = "" Or Despath = "" Or CommandText = "" Or DBConCls.EDIconstr = "" Then
      ReOK_Value = InfXSD.Value + "<<XSD appinfo 錯誤>> 必要設定未完成!  " + XApi.Value
      Return CType(ReOK_Value, SqlString)
	End If
	''檔名規則
	Filename = FileFmtname(XApi, ikey.Value, ExtkeyS.Value, wkey.Value)

	If (FileType.ToUpper = "TXT" And FieldParser = "") Then
	  ReOK_Value = InfXSD.Value + "<<XSD appinfo 錯誤>> TXT 類型 FieldParser 組合設定錯誤 !  " + XApi.Value
	  Return CType(ReOK_Value, SqlString)
	End If
  Catch ex As Exception
	Return CType("<<XSD.appinfo Load 失敗 !>> " + InfXSD.Value + " ErrMsg:" + ex.Message, SqlString)
  End Try
  ' 是否設定Schema element FileType.ToUpper = "TXT" Or
  If (FileType.ToUpper = "XML" Or FileType.ToUpper = "SQLDB") And XDoc.Descendants().<xsd:element>.Elements.Count <= 0 Then
	Return CType("<<XSD 未設定xsd:element >> FileType=" + FileType + "  類型 需設定 ! ", SqlString)
  End If

  Try
    ' 統一格式參數，如需自定請於XSD 指示sqlobject及參數
    CommandText = CommandText.Replace("@wkey", wkey.Value).Replace("@ExtkeyS", ExtkeyS.Value)
    CommandText = CommandText.Replace("@UID", "EDIApi")
  '' TXT
    If FileType.ToUpper = "TXT" Then
      ' Mds.ReadXmlSchema(InfXSD)
      'Mds = DBConCls.sqlTmpDs(CommandText)
      rcount = DBConCls.sqlTmpDs(CommandText, Mds)
      If rcount > 0 Then
		  FieldParser = IIf(IsNumeric(FieldParser) = True, ChrW(FieldParser), FieldParser)
		  tscount = DatasetWrTxtFile(Despath + Filename, FieldParser, Fencoding, Mds)
      End If

    End If
    '' DB to DB
    If FileType.ToUpper = "SQLDB" Then
      Try
      Mds = DBConCls.sqlTmpDs(CommandText)
       Rtcode = XsdMapCopy(XDoc, Mds)
      Catch ex As Exception
      ReOK_Value += " [SQLDB] 交易:" & Rtcode & " 錯誤 Message:" & ex.Message.ToString
      FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
      End Try
    End If
    If FileType.ToUpper = "XML" Then
      Mds.ReadXmlSchema(InfXSD)
      Dim TDs As DataSet = DBConCls.sqlTmpDs(CommandText)
      For Each T As DataTable In TDs.Tables
		  rcount += T.Rows.Count
      Next
       tscount = DstoDs(XDoc, TDs, Mds)
      'sqlXSDs(CmdText, XDoc, Mds)
       If tscount > 0 Then
		  Mds.WriteXml(Despath + Filename)
       End If
    End If
    ReOK_Value = "<<讀取 \R 筆，匯出 \X 筆 TO " + Filename + " !>>"
    '==========客製轉出檔案
    ' 7-11 ; cvs
    If FileType = "DsoFmtCK" Then
      ReOK_Value = DsoFmtCK(ikey, wkey, ExtkeyS, InfXSD)
    End If
    If FileType = "DsoFmt7_11" Then
      ReOK_Value = DsoFmt7_11(ikey, wkey, ExtkeyS, InfXSD)
    End If
    If FileType = "DsoFmtCVS" Then
      ReOK_Value = DsoFmtCVS(ikey, wkey, ExtkeyS, InfXSD)
    End If
  Catch ex As Exception
    ReOK_Value += "<解析錯誤:" + ex.Message + " !>"
    FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
  End Try

  ReOK_Value = ReOK_Value.Replace("\R", rcount).Replace("\X ", tscount)
  'JOBQ 排程協助
  JOBQsvr = XApi.<EDIinf>.@JOBQsvr
  'If JOBQsvr = "INSERT" And FileType.ToUpper <> "SQLDB" Then
  '  Dim ParArray() As String = {ikey.Value, "", ReOK_Value}
  '  JobQSvrCmd(JOBQsvr, Filename, "", "", ParArray)
  'End If
  Dim setV As String

  Try
    If JOBQsvr = "UPDATE" And rcount > 0 Then
      setV = "Rdo=" + IIf(rcount > 0, "4", "-1") + ",SYS_USER='XsdQueryExp', rcount=" + rcount.ToString + ", tscount=" + tscount.ToString
      If FileIO.FileSystem.FileExists(Despath + Filename) Then JobQSvrCmd("UPDATE", Despath + Filename, setV, "and Rdo<9")
    End If
  Catch ex As Exception
  End Try
  XsdQueryExp = CType(ReOK_Value, SqlString)

 End Function

 '========建立接口檔案 :命名方式======== 
 ' exkey: SQL CLR 傳入的次參數(STORERKEY,odkey,其他) 
 ' wkey: SQL CLR 傳入的主參數: 為檔案名 / 單號 
Public Shared Function FileFmtname(ByVal Apinfo As IEnumerable(Of XElement), ByVal infkey As String, ByVal exkey As String, ByVal wkey As String) As String
	Dim stkey As String() = exkey.Split(",")
	Dim Filename As String = Apinfo.<EDIinf>.@EDLLocDr
	Dim Fmt As String = Apinfo.<FileMap>.@FilenameFmt
	If (Fmt = "" Or String.IsNullOrEmpty(Fmt)) Or wkey.IndexOf(".") = -1 Then
	  Fmt = infkey + "_" + stkey(0) + stkey(1) + "_" + Now.ToString("HHmmssff") + ".PUB"
	End If
	' 如wkey 已是檔案名直接引用 
	If wkey.IndexOf(".") > -1 Then Fmt = Fmt.Replace("@wkey", wkey)
	If Fmt.IndexOf("@") > 0 Then
	  If Fmt.IndexOf("@stkey") > 0 Then Fmt = Fmt.Replace("@stkey", stkey(0))
	  If Fmt.IndexOf("@odkey") >= 0 Then Fmt = Fmt.Replace("@odkey", stkey(1))
	  If Fmt.IndexOf("@yymmdd") >= 0 Then Fmt = Fmt.Replace("@yymmdd", Now.ToString("yyMMdd"))
	  If Fmt.IndexOf("@hhmmss") >= 0 Then Fmt = Fmt.Replace("@hhmmss", Now.ToString("HHmmss"))
	  'If Fmt.IndexOf("mmssff") > 0 Then Fmt = Fmt.Replace("@mmssff", Now.ToString("HHmmssff"))
	 ' If Fmt.IndexOf("@yyyymmdd") >= 0 Then Fmt = Fmt.Replace("@yyyymmdd", Now.ToString("yyyyMMdd"))
	 ' If Fmt.IndexOf("@yyMMddHHmm") >= 0 Then Fmt = Fmt.Replace("@yyMMddHHmm", Now.ToString("yyMMddHHmm"))
	  'If Fmt.IndexOf("@yyyyMMddhhmm") >= 0 Then Fmt = Fmt.Replace("@yyyyMMddhhmm", Now.ToString("yyyyMMddHHmm"))
	  'If Fmt.IndexOf("@HHmmssff") > 0 Then Fmt = Fmt.Replace("@HHmmssff", Now.ToString("HHmmssff"))

	End If
	  Return Fmt
 End Function


 '***********************************
 Public Class MapingBox
		Public Tfield As String
		Public Sqlfield As String
		Public item As Integer
		Public Length As Integer
		Public SourceDt As String
		Public DecDt As String
End Class
' 分析XSD 與Query DataSet 差異   
 Public Class XsdAnysDs
		Public Dname As String
		Public Sname As String
		Public Qname As String
		Public xdIts As Integer
		Public MapCk As String
		Public refkey As String
		Public recnt As Integer
End Class
'========建立 Mapinf  ========================= 
 '依XSD文件建立 Mapinf 集合資訊 (未Maping)
 '======================================== 
Sub Create_Mapinf(ByVal XDoc1 As XDocument)
		Dim colid As Integer = 0
		Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
		For Each vt In Vxsd
			colid = 1
			MapInf = From el2 In (From el In XDoc1.Descendants().<xsd:element> Where el.@name = vt.name
								Select el).Descendants().<xsd:element>
								Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
						Select New MapingBox With {.Tfield = el2.@name, .Sqlfield = IIf(el2.@sql:field Is Nothing, vt.name, el2.@sql:field), .Length = IIf(el2.@length Is Nothing, 0, el2.@length),
												.item = colid, .SourceDt = vt.name, .DecDt = vt.relation}
			For Each elx In MapInf
				colid += 1
				'Console.WriteLine("{0},{1},{2}", elx.Tfield, elx.Sqlfield, elx.item)
			Next
		Next
	End Sub

'============================
'msgKey()順序 :  LotKey1,LotKey2,LotKey3,SERR
'在SQLFunction 中UPDATE 不可使用同一連線連線 ""
'============================
Public Shared Function JobQSvrCmd(ByVal JobType As String, ByVal sfile As String, ByVal setValue As String, ByVal WhereStr As String, Optional ByVal msgKey() As String = Nothing) As Integer
	'JobQSvrCmd = ""
	Dim CmdJobq As String = ""
	If JobType = "UPDATE" Then
		'CmdJobq = "UPDATE SJOB..JobQ set rdo=@rdo, udate=getdate(),rcount=" + rcount + ",LotKey1='" + msgKey(0) + "'   where rdo<3 and sfile=''" + sfile + "';"
	CmdJobq = "UPDATE SJOB..JobQ set udate=getdate(), " + setValue + " where sfile='" + sfile + "' " + WhereStr + ";"
	End If
	If JobType = "INSERT" And InfKey <> "" Then
	CmdJobq = "INSERT INTO SJOB..JobQ(STKEY, Wkey, InfID, Jtyps, sfile, Rfkey1, Rfkey2, lotkey1, lotkey2, rdo, exfile, tscount) " _
		  + "SELECT  Stkey,Wkey='@Wkey',Infid ,Jtyps='@Jtyps',  sfile=EDILocDr , Rfkey1=back_one , Rfkey2=EAIpath , LotKey1='@LotKey1', LotKey2='@LotKey2',rDo=null ,exfile=EDILocDr+ '@Wkey',tscount=@tscount" _
		  + "from  EDIinf  where Infid='" + InfKey + "' and Stkey='" + Stkey + "'; "
		CmdJobq.Replace("@Jtyps", msgKey(0).ToString)
		CmdJobq.Replace("@LotKey1", msgKey(1).ToString)
		CmdJobq.Replace("@LotKey2", msgKey(2).ToString)
		CmdJobq.Replace("@Wkey", sfile)
	End If
		Try
			Using conJob As New SqlClient.SqlConnection("Data Source=192.168.1.33\SJOB;Initial Catalog=SJOB;Persist Security Info=True;User ID=sa;Password=zaq1")
					Using JOBM As New SqlClient.SqlCommand(CmdJobq, conJob)
					conJob.Open()
		   JOBM.ExecuteNonQuery()
		  'FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("EDI.JOBQ command:{0}   ", CmdJobq) + Chr(10), True)
					End Using
			End Using
		Catch ex As Exception
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("EDI.JOBQ_Cmd: 描述  ERR: {0} command:{1}   ", ex.Message, CmdJobq) + Chr(10), True)
		End Try
End Function


'========  讀取文字檔方式 ============= 
' 暫時只處理單一格式文字檔
'  tabN : 將DataSet Table 傳入 , sfile :  文字檔位置，ws : 
'=============================== 
Public Shared Function readTxtFile(ByRef tabN As DataTable, sfile As String) As Integer
	Dim lno As Integer = 0
	Dim ws As Integer()
	Dim Filecode As Encoding
	'readTxtFile = 0
	' XDoc.Descendants.<xsd:appinfo>.<FileMap>.@Encoding.FirstOrDefault.Value()
	FieldParser = ChrW(XApi.<FileMap>.@FieldParser)
	Fcoding = XApi.<FileMap>.@Encoding
	If Fcoding = "BIG5" Then
		Filecode = Encoding.GetEncoding(950)
	Else
		Filecode = Encoding.UTF8
	End If
	Try
		' xsd..<EDIinf>.@FieldParser設定 取檔方式 
		Using myReader As New TextFieldParser(sfile, Filecode)
			If FieldParser = "FixedWidth" Then
				myReader.TextFieldType = FieldType.FixedWidth
				ws = (From el In MapInf Where el.Length > 0 Select el.Length)
				myReader.SetFieldWidths(ws)
			Else
				'FieldParser = ChrW(XApi.Elements("EDI").ElementAt(0).Attribute("FieldParser").Value)
				myReader.TextFieldType = FieldType.Delimited
				myReader.SetDelimiters(FieldParser)
			End If
			myReader.TrimWhiteSpace = True
			myReader.HasFieldsEnclosedInQuotes = False
			While Not myReader.EndOfData
				Dim TxtVal As String()
				TxtVal = myReader.ReadFields
				tabN.Rows.Add(TxtVal)
				lno += 1
			End While
		End Using
		readTxtFile = lno
	  Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
		ReOK_Value += "ReadTxtFile<讀取 \N 筆發生錯誤 ! " & ex.Message & ">"
		FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	  End Try
End Function
Public Shared Function readXmlFile(sfile As String, ByRef infxsd As String, ByRef MidDs As DataSet) As Integer
  Try
    If infxsd <> "" Then
      MidDs.WriteXmlSchema(infxsd)
    End If
    MidDs.ReadXml(sfile)
    ' 系統需求欄補建
    If MidDs.Tables(0).Columns.Contains("sfile") = False Then
      MidDs.Tables(0).Columns.Add("edate", Type.GetType("System.DateTime"))
      MidDs.Tables(0).Columns.Add("sfile", Type.GetType("System.String"))
      MidDs.Tables(0).Columns.Add("etyps", Type.GetType("System.Int32"))
      For Each dr As DataRow In MidDs.Tables(0).Rows
        dr("edate") = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
        dr("sfile") = sfile
        'dr("etyps") = 0
      Next
      MidDs.AcceptChanges()
    End If
    readXmlFile = MidDs.Tables(0).Rows.Count
  Catch ex As Exception
          readXmlFile = 0
          'ReOK_Value = "<數據結構解析錯誤:" + ex.Message + " !>"
          FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", "readXmlFile:<數據Maping解析錯誤:" + ex.Message + " !>") + Chr(10), True)
  End Try

End Function

'========建立接口資料表 DataSet 表格=========== 
' 依XSD 的 DATA TYPEXElement  自行建立單純結構 DataSet 表格
'================================== 
Public Overloads Shared Function CrEdiDsFmt(ByVal XDoc1 As XDocument, ByRef MidDs As DataSet) As String
		Dim colid As Integer = 0
		Dim Ctable As New DataTable
		'====建表及讀檔
		Try
			Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
			For Each vt In Vxsd
				'Console.WriteLine(vt)
				MidDs.Tables.Add(vt.name)
				Dim cInf = From el2 In (From el In XDoc1.Descendants().<xsd:element> Where el.@name = vt.name
									Select el).Descendants().<xsd:element>
									Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
							Select el2.@name
				For Each elx In cInf
					colid += 1
					Dim colname As String = elx.ToString
						MidDs.Tables(vt.name).Columns.Add(colname, GetType(String))
				Next
				'Ctable.TableName = vt.name.ToString
				'MidDs.Tables.Add(Ctable)
			Next
			If MidDs.Tables(0).Columns.Contains("sfile") = False Then
				MidDs.Tables(0).Columns.Add("edate", Type.GetType("System.DateTime"))
				MidDs.Tables(0).Columns.Add("sfile", Type.GetType("System.String"))
				MidDs.Tables(0).Columns.Add("etyps", Type.GetType("System.Int32"))
				MidDs.Tables(0).Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
				MidDs.Tables(0).Columns("sfile").DefaultValue = impsFile
				'MidDs.Tables(0).Columns("etyps").DefaultValue = 0
			End If
			If MidDs.Tables.Count = Vxsd.Count Then
				Return "Schema OK!"
			Else
				Return "CrEdiDsFmt:<數據結構解析錯誤!> "
			End If

		Catch ex As DataException
			CrEdiDsFmt += "<CrEdiDsFmt:<數據結構解析錯誤:" + ex.Message + " !>"
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", CrEdiDsFmt) + Chr(10), True)
		End Try
		Return CrEdiDsFmt
	End Function
Public Overloads Shared Function CrEdiDsFmt(ByVal XMaps As IEnumerable(Of XElement), ByRef MidDs As DataSet) As String
		 If XMaps.Elements.Count <= 0 Then CrEdiDsFmt = "<<XSD 讀取Mapping設定Maped 錯誤  !>> "
		'====建表及讀檔
		Dim Ctable As New DataTable
		Try
			For Each el As XElement In XMaps
				If el.Attributes.Where(Function(a) a.Name.LocalName = "relation").ToArray.Length > 0 Then
					' 使用Xneme  MidDs.Tables.Add(el.Attribute("name").Value.TrimEnd)
					Ctable = MidDs.Tables.Add(el.Attributes.Where(Function(a) a.Name.LocalName = "relation").ToArray(0).Value)
				Else
					Dim colname As String = el.Attribute("name").Value.TrimEnd
					Ctable.Columns.Add(colname, GetType(String))
				End If
			Next
			'基本必要欄位 
			CrEdiDsFmt = "<<讀取 \N 筆，新增 \X 筆 TO " + MidDs.Tables(0).TableName + " !>>"
			MidDs.Tables(0).Columns.Add("edate", Type.GetType("System.DateTime"))
			MidDs.Tables(0).Columns.Add("sfile", Type.GetType("System.String"))
			MidDs.Tables(0).Columns.Add("etyps", Type.GetType("System.Int32"))
			MidDs.Tables(0).Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
			MidDs.Tables(0).Columns("sfile").DefaultValue = impsFile
			MidDs.Tables(0).Columns("etyps").DefaultValue = 0
		Catch ex As DataException
			CrEdiDsFmt += "<數據結構解析錯誤:" + ex.Message + " !>"
			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", CrEdiDsFmt) + Chr(10), True)
		End Try
	End Function






'================================='*********** 轉出文件區塊 ********************
'========= 依XSD 進行數據轉換 (矩陣格式)=====
'範例 : OK  配送單文件檔 'CirCleK
'================================== 
''  <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
Public Shared Function DsoFmtCK(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String) As String
	Dim DT As DataTable
	Dim FileName, FilePath, Head, Body, Tail As String
	Dim FileNameP, FilePathP, HeadP, BodyP, TailP As String
	Dim count As String
  Dim its As Integer
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
	JOBQsvr = XApi.<EDIinf>.@JOBQsvr
	FilePath = XApi.<EDIinf>.@EDLLocDr
	FilePathP = XApi.<EDIinf>.@EDLLocDr

  FileName = "CVS" + sfile + ".F03"
  FileNameP = "DOK" + sfile + ".P03"
	Dim PCname As String

	DT = DBConCls.sqlTmpData(CommandText)
	count = DT.Rows.Count
	If count = 0 Then
	  DsoFmtCK = ReOK_Value.Replace("\X", count)
	  Exit Function
	End If
	' F03
	' @Head 
	Try
	  Head = "1"  'RDFMT
	  Head += "D69"	'SNCD
	  HeadP = Head + DateTime.Now.ToString("yyyyMMdd").PadRight(60, "0")  'RDFMT+SNCD+PRDT+P/H
	  Head += DateTime.Now.ToString("yyyyMMdd").PadRight(162, "0")	'PRDT+FIL

	  Head += vbCrLf  'CRLF
	  HeadP += vbCrLf	'Crlf

    For i As Integer = 0 To count - 1
    ' F03   Body
      Body += "2"    'RDFMT
      Body += "169" 'ECNO
      Body += "TOK" 'CNNO
      Body += RTrim(DT.Rows(i).Item("STNO").ToString) 'STNO
      Body += RTrim(DT.Rows(i).Item("STNM").ToString).PadRight(18 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("STNM").ToString)).Length - RTrim(DT.Rows(i).Item("STNM").ToString).Length)) 'STNM
      Body += RTrim(DT.Rows(i).Item("odkey").ToString)  'ODNO
      Body += RTrim(DT.Rows(i).Item("dpay").ToString).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("dpay").ToString)).Length - RTrim(DT.Rows(i).Item("dpay").ToString).Length), "0") 'AMT
      Body += RTrim(DT.Rows(i).Item("down").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("down").ToString)).Length - RTrim(DT.Rows(i).Item("down").ToString).Length)) 'CUTKNM
      Body += " ".PadLeft(20) 'CUTKTL
      Body += "     " 'DCRONO
      Body += "D69" 'EDCNO
      Body += "0"   'PRODNM

      Body += "誠品網路書店 www.eslite.com             " 'ECWEB
      Body += "(02)8789-8921       " 'ECSERTEL
      Body += RTrim(DT.Rows(i).Item("tots")).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("tots").ToString)).Length - RTrim(DT.Rows(i).Item("tots").ToString).Length), "0") 'REALAMT
      Body += "3"  'TRADETYPE
      Body += "963" 'SERCODE
      Body += vbCrLf  'CRLF
      ' P03   Body
      BodyP += "2"  'RDFMT
      BodyP += "169"    'ECNO
      BodyP += "TOK"    'CNNO
      BodyP += RTrim(DT.Rows(i).Item("odkey").ToString)  'ODNO
      BodyP += RTrim(DT.Rows(i).Item("down").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("down").ToString)).Length - RTrim(DT.Rows(i).Item("down").ToString).Length)) 'CUTKNM
      BodyP += RTrim(DT.Rows(i).Item("dtel").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("dtel").ToString)).Length - RTrim(DT.Rows(i).Item("dtel").ToString).Length)) 'CUTKTL
      BodyP += RTrim(Left(DT.Rows(i).Item("Weight").ToString, 5)).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("Weight").ToString)).Length - RTrim(DT.Rows(i).Item("Weight").ToString).Length), "0") 'Weight
      BodyP += RTrim(DT.Rows(i).Item("PackType").ToString).PadRight(1 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("PackType").ToString)).Length - RTrim(DT.Rows(i).Item("PackType").ToString).Length)) 'Packtype
      BodyP += vbCrLf
      its += 1
    Next i

	  ' @Tail
	  Tail = "3"	'RDFMT
	  Tail += count.PadLeft(8, "0")	'RDCNT
	  'Tail += DT.Compute("SUM(dpay)", "").PadRight(9 - System.Text.Encoding.Default.GetBytes(DT.Compute("SUM(dpay)", "")).Length - RTrim(DT.Compute("SUM(dpay)", "").ToString.Length)) 'AMT
	  TailP += Tail.PadRight(64, "0")

	  Tail += DT.Compute("SUM(dpay)", "").ToString.PadRight(7, "0")	'AMT
	  Tail += "".PadRight(150, "0")	 ' FIL2
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
  Public Shared Function DsoFmtCVS(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String) As String
  Dim FileName, FilePath
  Dim count As String
  Dim i As Integer
  ReOK_Value = "<<轉出 \N 筆 Form  DsoFmtCVS !" + ikey.ToString + ">>"
  '*** 指定目的表的[連接]及[表][欄位資訊]
  Try
     'XDoc = XDocument.Load(infXSD)
     'XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el
  '====取得: xsd:appinf 參數
	'DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
	'CommandText = XApi.<CommandText>.FirstOrDefault.Value
    If DBConCls.EDIconstr = "" Then
      ReOK_Value += "<<XSD設定 appinfo: 設定未完成CONNs>> " + infXSD
      Return ReOK_Value
    End If
  Catch ex As Exception
    Return "<<讀取XSD Load失敗" + infXSD + ">>" + ex.Message
    Exit Function
  End Try
  FilePath = XApi.<EDIinf>.@EDLLocDr
    Try
      Dim XMLfileR As New Xml.XmlDocument
      'Dim aaaa As New tw.com.cvs.cvsweb.Service
      Dim response As String
      'aaaa.Timeout = 3600000
      Dim Wdt As New DataTable
      '   "select * from dbo.ETCVSxml where etyps=7"
	  Wdt = DBConCls.sqlTmpData(CommandText)

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
          XmlString = XmlString + "<CUTKNM><![CDATA[" & RTrim(Wdt.Rows(i).Item("CUTKNM").ToString) & "]]></CUTKNM>"     ''取貨人中文姓名
          XmlString = XmlString + "<CUTKTL>" & RTrim(Wdt.Rows(i).Item("CUTKTL").ToString) & "</CUTKTL>" ''收貨人手機號碼
          XmlString = XmlString + "<PRODNM>0</PRODNM>"                             ''固定=0 RTrim(Wdt.Rows(i).Item("PRODNM").ToString)
          XmlString = XmlString + "<ECWEB><![CDATA[" & RTrim(Wdt.Rows(i).Item("ECNM").ToString) & "]]></ECWEB>"      ''固定
          XmlString = XmlString + "<ECSERTEL>" & RTrim(Wdt.Rows(i).Item("ECSERTEL").ToString) & "</ECSERTEL>"        ''固定
          XmlString = XmlString + "<REALAMT>" & RTrim(Wdt.Rows(i).Item("REALAMT").ToString) & "</REALAMT>"
          XmlString = XmlString + "<TRADETYPE>" & RTrim(Wdt.Rows(i).Item("TRADETYPE").ToString) & "</TRADETYPE>"
          XmlString = XmlString + "<SERCODE>963</SERCODE>"                         ''固定
          XmlString = XmlString + "<EDCNO>D07</EDCNO>" + "</ORDER>" '東拓D05改日翊D07
          cnt = cnt + 1
          If (cnt Mod 50 = 0) Then
            XmlString = XmlString + "<ORDERCOUNT><TOTALS>" & CStr(cnt) & "</TOTALS></ORDERCOUNT>"
            XmlString = XmlString + "</ORDER_DOC> "
            XMLfileR.LoadXml(XmlString)
            XMLfileR.Save(FilePath + sfile)
            'response = aaaa.ORDERS_ADD(XmlString)
            'If (response.Contains("退0筆") = True) Then
            '	Console.WriteLine(Now() + " 上傳成功")
            'Else
            '	Console.WriteLine(Now() + " ★★★★★上傳成功但有踢退資料★★★★★")
            'End If

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
          '  response = aaaa.ORDERS_ADD(XmlString)
          'If (response.Contains("退0筆") = True) Then
          '	Console.WriteLine(Now() + " 上傳成功")
          'Else
          '	Console.WriteLine(Now() + " ★★★★★上傳成功但有踢退資料★★★★★")
          'End If
        End If
      End If    'Wdt.Rows.Count>0

      ReOK_Value = ReOK_Value.Replace("\N", Wdt.Rows.Count) + " Export:" + sfile
    Catch ex As Exception
      ReOK_Value += " [寫入XML檔案]: " & i & "row ,錯誤 Message:" & ex.Message.ToString
      FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
    End Try

  DsoFmtCVS = ReOK_Value
  End Function

  '========= 依XSD 進行數據轉換 ====== "【版本為17.02.22】 " 
'範例 : 7-11 ;   ''711用FTP正式地址''ftp://785001:785PRUztX390a001@202.168.204.211/SIN/
'舊版 :  exec E_CAN.dbo.ETSeven_ErrCheckSTOP  > cmd.CommandText = "select toMail from iexp..rpini where rpname = '7-11 SIN上傳'"
'		>  select * from dbo.ETSeven_XMLView order by Order_EshopOrderNO  
'================================== 
Public Shared Function DsoFmt7_11(ByVal ikey As String, ByVal sfile As String, ByVal iStkey As String, ByVal infXSD As String) As String
  Dim DevStr, DocName, ToM, Body, SubJect As String
  Dim XMLfileR As New Xml.XmlDocument
  Dim FileName, FilePath
  Dim Wdt As New DataTable
  Dim i, cnt As Integer
  ReOK_Value = "<<轉出 \N 筆 Form  CKDsoFmt!" + ikey.ToString + ">>"
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
  FilePath = XApi.<EDIinf>.@EDLLocDr
  Try

    '抓取欲上傳的訂單
	  Wdt = DBConCls.sqlTmpData(CommandText)
      Dim XmlString As String
      XmlString = "<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "UTF-8" & Chr(34) & " ?>" 'XML固定需要的首

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

 '=================
 '將查詢結果轉入中繼DataSet
 '簡易版: 需依 XSD 結構取得: 對等的QUERY 集合 含虛擬表
 '=================
 Public Shared Function DstoDs(ByVal adpinf As XDocument, ByVal SDs As DataSet, ByRef DDs As DataSet) As Integer
		'Dim Vxsd = From el In adpinf.Descendants().<xs:element> Where (el.@sql:relation IsNot Nothing)
		'		Select New With {.Dname = el.@name, .Sname = el.@sql:relation, .Qname = "N", .xdIts = 0, .MapCk = "-1", .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H"), .recnt = 0}
	Dim rcnt As Long
	Dim STableID As Integer = 0
	For Each T In DDs.Tables
		If SDs.Tables.Count = DDs.Tables.Count Then
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

Public Shared Function DatasetWrTxtFile(ByVal efilePayh As String, ByVal FieldTag As String, ByVal Fcode As String, ByRef MidDs As DataSet) As Integer
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
      For i As Integer = 0 To MidDs.Tables.Count - 1
        For Each drow As DataRow In MidDs.Tables(i).Rows
          'For c As Integer = 0 To drow.Table.Columns.Count - 1
          '	Xstr.Append(drow(c).ToString)
          '	If c < drow.Table.Columns.Count Then
          '		Xstr.Append(FieldTag)
          '	End If
          'Next c
          ''Dim array() As Object = row.ItemArray
          ''For i = 0 To array.Length - 2
          ''	stm.Write(array(i).ToString() + ControlChars.Tab)
          ''Next i
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

 '*********** 轉入SQL區塊 ********************
'======== 資料表 DATASET 依Mapinf  ========================== 
 ' 使用DATASET + XsdMaping (sql:field)來源數據 至 SQL server   
'  XMaping(sql:field) 為主 ，如DataTable 查無攔欄位,亦不使用 
 '=================================================
	Public Shared Function XsdMapCopy(ByVal XDoc1 As XDocument, ByRef MidDs As DataSet) As String
		Dim sql As XNamespace = XDoc1.Root.GetNamespaceOfPrefix("sql")
		DBConCls.EDIconstr = XDoc1.Descendants.<xsd:appinfo>.<CONNs>.FirstOrDefault.Value
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(DBConCls.EDIconstr)
		Dim colid As Integer = 0
		con.Open()
		Dim SqlCopy = New SqlBulkCopy(con)
		SqlCopy.BatchSize = 10000
	SqlCopy.BulkCopyTimeout = 180 '30分
		'XsdMapCopy = 0
		'Try
		Dim Vxsd = From el In XDoc1.Descendants().<xsd:element> Where el.@sql:relation IsNot Nothing Select New With {.name = el.@name, .relation = el.@sql:relation, .refkey = IIf(el.@sql:relationship IsNot Nothing, el.@sql:relationship, "H")}
		For Each vt In Vxsd
			colid = 1
			MapInf = From el2 In (From el In XDoc1.Descendants().<xsd:element> Where el.@name = vt.name
				Select el).Descendants().<xsd:element>
				Take While el2.@sql:relationship Is Nothing Or el2.@sql:relationship = vt.refkey
					Select New MapingBox With {.Tfield = el2.@name, .Sqlfield = IIf(el2.@sql:field Is Nothing, vt.name, el2.@sql:field), .Length = IIf(el2.@fixed Is Nothing, 0, el2.@fixed),
						.item = colid, .SourceDt = vt.name, .DecDt = vt.relation}
			For Each elx In MapInf
				colid += 1
				'Console.WriteLine("{0},{1},{2}", elx.Tfield, elx.Sqlfield, elx.item)
				If MidDs.Tables(vt.name).Columns.Contains(elx.Tfield) = True Then
						Dim mapcol As SqlBulkCopyColumnMapping = SqlCopy.ColumnMappings.Add(elx.Tfield, elx.Sqlfield)
				End If
			Next
			' 資料表確認是否有料 sfile,edate,etyps 改XSD內指示
			If MidDs.Tables(vt.name).Rows.Count > 0 And SqlCopy.ColumnMappings.Count > 0 Then
 
				SqlCopy.DestinationTableName = vt.relation
		'  ' 一次匯入，如果目的表長度不足無法Debug
				Try
					SqlCopy.WriteToServer(MidDs.Tables(vt.name))
					SqlCopy.ColumnMappings.Clear()
				Catch ex As SqlException
					'Mds.WriteXml(DBConCls.APPLog)
					XsdMapCopy = "XsdMapCopy:<數據Maping解析錯誤:" + ex.Message + " !>"
					FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 错误描述  ERR: {0}   ", XsdMapCopy) + Chr(10), True)
				End Try
			End If
		Next
		SqlCopy.Close()
		con.Close()
		con.Dispose()
		 XsdMapCopy = "OK !"

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

<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="System.ArgumentException.#ctor(System.String)")> _
	Private Shared Function InvalidDataTypeCode(ByVal code As TypeCode) As Exception
		Return New ArgumentException("Invalid type: " & code.ToString())
	End Function

	<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="System.ArgumentException.#ctor(System.String)")> _
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

'=================依XSD 進行數據轉換 =====
' 只有一個表 (未分割)
'==================================
 ' <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
 'Public Shared Function XsdBlkCopy1T(ByVal iInf As SqlString, ByVal iFpath As SqlString, ByVal iStkey As SqlString) As SqlString
	'Dim TxtTagC, tabN, INFType, FileType, BuName, ReOK_Value As String
	'Dim CommandText, CheckFun As String
	'Dim XDoc As XDocument = XDocument.Load(DBConCls.Xdpath & "\" & iInf.ToString & "_" & iStkey.ToString & ".xsd")
	'Dim XApi As IEnumerable(Of XElement) = From el In XDoc.Descendants().<xsd:appinfo> Select el
	''Dim sqlstr As SqlString = ""
	'Dim lno As Integer = 0
	'Dim cklin As Integer = 0
	'Dim Mapdata As New DataTable
	'Try
	''====取得:<EDIinf INFType="Import" EDLLocDr="D:\SJOB\IMP\" FileType="txt" FieldParser="1" SQLBulkCk="SAPi..SP34" BuName=""/>
	'	DBConCls.EDIconstr = XApi.Elements("CONNs").Value
	'	FieldParser = ChrW(XApi.Elements("EDI").ElementAt(0).Attribute("FieldParser").Value)
	'	SQLBulkCk = XApi.Elements("EDI").ElementAt(0).Attribute("SQLBulkCk").Value
	'	INFType = XApi.Elements("EDI").ElementAt(0).Attribute("INFType").Value
	'	FileType = XApi.Elements("EDI").ElementAt(0).Attribute("FileType").Value
	'	If FieldParser = "" Or SQLBulkCk = "" Or INFType <> "Import" Or FileType <> "txt" Then
	'		ReOK_Value = "<<讀取XSD設定 appinfo失敗   !>>FieldParser:" + TxtTagC & ",SQLBulkCk:" & tabN & ".xsd"
	'		Return CType(ReOK_Value, SqlString)
	'	End If
	'Catch ex As Exception
	'	ReOK_Value = "<<讀取XSD Load失敗 !>>" + DBConCls.Xdpath & "\" & iInf.ToString & ".xsd"
	'End Try
	'	'===是否有上下表的關連 <sql:relationship 
	'	'Dim TabNRef As IEnumerable(Of XElement) = XApi.<sql:relationship>.ElementAt(0).Attribute("EDLLocDr")
	''**檔案欄位的分隔符號字元位置 及map table 建立
	'Dim XMaps As IEnumerable(Of XElement) = From el In XDoc.Descendants().<xsd:element> Select el
	'If XMaps.Elements.Count <= 0 Then Return CType("<<讀取XSD 設定Maped   !>> ", SqlString)
	''====建表及讀檔
	'Try
	'	If FileType = "txt" Then
	'		For Each el As XElement In XMaps
	'			'If el.Attributes.Count >= 3 Then
	'			If el.Attributes.Where(Function(a) a.Name.LocalName = "relation").ToArray.Length > 0 Then
	'				Mapdata.TableName = el.Attribute("name").Value.TrimEnd
	'			Else
	'				Dim colname As String = el.Attribute("name").Value.TrimEnd
	'				Mapdata.Columns.Add(colname, GetType(String))
	'			End If
	'		Next
	'	End If
	'	If FileType = "XML" Then
	'		Mapdata.ReadXml(iFpath)
	'	End If

	'	'基本必要欄位 

	'	ReOK_Value = "<<讀取 \N 筆，新增 \X 筆 TO " + tabN + " !>>"
	'	Mapdata.Columns.Add("edate", Type.GetType("System.DateTime"))
	'	Mapdata.Columns.Add("sfile", Type.GetType("System.String"))
	'	Mapdata.Columns.Add("etyps", Type.GetType("System.Int32"))
	'	Mapdata.Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
	'	Mapdata.Columns("sfile").DefaultValue = iFpath.ToString
	'	Mapdata.Columns("etyps").DefaultValue = 0
	'Catch ex As DataException
	'	ReOK_Value += "<數據結構解析錯誤:" + ex.Message + " !>"
	'	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'End Try

	'  Try
	'	Using myReader As New TextFieldParser(iFpath)
	'	  myReader.TextFieldType = FieldType.Delimited
	'	  myReader.SetDelimiters(FieldParser)
	'	  'myReader.TrimWhiteSpace = True
	'	  myReader.HasFieldsEnclosedInQuotes = False
	'	  'lno = myReader.LineNumber

	'	  While Not myReader.EndOfData
	'		Dim TxtVal As String()
	'		TxtVal = myReader.ReadFields
	'		Mapdata.Rows.Add(TxtVal)
	'		'Mapdata.Rows(lno)("sfile") = iFpath
	'		'Mapdata.Rows(lno)("edate") = Now.ToString("yyyy/MM/dd HH:mm:ss")
	'		'Mapdata.Rows(lno)("etyps") = 1
	'		lno += 1
	'	  End While
	'	End Using
	'  Catch ex As Exception
	'	ReOK_Value += "<讀取 \N 筆發生錯誤 ! " & ex.Message & ">"
	'	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'  End Try
	'  ReOK_Value = ReOK_Value.Replace("\N", lno)
	'  ' 回傳數據
	'  Dim Rtcode As String = ""
	'  Dim Ckcount As Long = 0

	'  Try
	'	'讀取大於10 才能執行大量
	'	If Mapdata.Rows.Count <= 10 Then
	'	'If iInf = "SP01" Or iInf = "SP07" Or iInf = "SP09" Or iInf = "SP10" Then
	'	  Rtcode = DBConCls.RowsCopyFun(Mapdata, tabN, 1)
	'	Else
	'	  Rtcode = DBConCls.TableCopyFun(Mapdata, tabN, 1)
	'	End If
	'  Catch ex As Exception
	'	ReOK_Value += " [新增]:" & Rtcode & "row ,錯誤 Message:" & ex.Message.ToString
	'	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'  End Try

	'  '' 確認 iFpath 完成筆數
	'  Ckcount = DBConCls.ExcuteCount("SELECT count(*) from " + tabN + " where sfile='" + iFpath + "'")
	'	ReOK_Value = ReOK_Value.Replace("\X", Ckcount)
	'  '回傳 錯誤內容 暫無法傳到前端，改使用XML 記錄錯誤 
	'  If Ckcount = Mapdata.Rows.Count Then
	'	ReOK_Value += Rtcode
	'	'來源檔刪除
	'	'FileIO.FileSystem.DeleteFile(iFpath.ToString)
	'  ElseIf Mapdata.Rows.Count <= 10 Then
	'	Mapdata.DefaultView.RowFilter = "etyps<2"
	'	Dim Wxml As DataTable = Mapdata.DefaultView.ToTable(iInf.ToString)
	'	Wxml.WriteXml(iFpath.ToString.Replace(".", "") & "-內容錯誤協助.XML")
	'  Else
	'	ReOK_Value += "<2. 大量CopyToSQL 筆數錯誤:" & Rtcode & ">保留來源檔:" & iFpath.ToString
	'  End If
	'  XsdBlkCopy1T = CType(ReOK_Value, SqlString)
 ' End Function

'Private Shared Sub JobQSvrCmd(p1 As String, p2 As String())
'Throw New NotImplementedException
' End Sub


End Class

 '<Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
 ' Public Shared Function CKDsoFmt(ByVal ikey As SqlString, ByVal sfile As SqlString, ByVal iStkey As SqlString, ByVal infXSD As SqlString) As SqlString
	'Dim DT As DataTable
	'Dim FileName, FilePath, Head, Body, Tail As String
	'Dim FileNameP, FilePathP, HeadP, BodyP, TailP As String
	'Dim count As String
	'Dim i As Integer
	'ReOK_Value = "<<轉出 \N 筆 Form  CKDsoFmt!" + ikey.ToString + ">>"
	'*** 指定目的表的[連接]及[表][欄位資訊]
	'Try
	'   XDoc = XDocument.Load(infXSD.Value)
	'   XApi = From el In XDoc.Descendants().<xsd:appinfo> Select el
	'====取得: xsd:appinf 參數
	'  DBConCls.EDIconstr = XApi.<CONNs>.FirstOrDefault.Value
	'  CommandText = XApi.<CommandText>.FirstOrDefault.Value
	'  If DBConCls.EDIconstr = "" Then
	'	  ReOK_Value += "<<XSD設定 appinfo: 設定未完成CONNs>> " + infXSD.Value
	'	  Return CType(ReOK_Value, SqlString)
	'  End If
	'Catch ex As Exception
	'  Return CType("<<讀取XSD Load失敗" + infXSD.Value + ">>" + ex.Message, SqlString)
	'  Exit Function
	'End Try
	'JOBQsvr = XApi.<EDIinf>.@JOBQsvr
	'FilePath = XApi.<EDIinf>.@EDLLocDr
	'FilePathP = XApi.<EDIinf>.@EDLLocDr

	'FileName = "CVSD69" + DateTime.Now.ToString("yyyyMMdd") + ".F03"
	'FileNameP = "DOKD69" + DateTime.Now.ToString("yyyyMMdd") + ".P03"
	'Dim PCname As String
	''【0】.  排除店號異常的訂單 +【1】. 下載欲上傳的訂單
	'User_Function.DAC.ExcuteSql("Exec ETUpLoadCheck '','','Z9'") +[ETCirCle_UpLoadView] 
	'   EC_XsdGroup_Qry() 'CKDsoFmt','@wkey','@any','@UID'

	'DT = DBConCls.sqlTmpData(CommandText)
	'count = DT.Rows.Count
	'If count = 0 Then
	'  CKDsoFmt = CType(ReOK_Value.Replace("\X", count), SqlString)
	'  Exit Function
	'End If
	' F03()
	' @Head 
	'Try
	'  Head = "1"  'RDFMT
	'  Head += "D69"	'SNCD
	'  HeadP = Head + DateTime.Now.ToString("yyyyMMdd").PadRight(60, "0")  'RDFMT+SNCD+PRDT+P/H
	'  Head += DateTime.Now.ToString("yyyyMMdd").PadRight(162, "0")	'PRDT+FIL

	'  Head += vbCrLf  'CRLF
	'  HeadP += vbCrLf	'Crlf
	'   @Body 

	'  For i = 0 To count - 1
	'	 F03(Body)
	'	Body += "2"	   'RDFMT
	'	Body += "169" 'ECNO
	'	Body += "TOK" 'CNNO
	'	Body += RTrim(DT.Rows(i).Item("STNO").ToString)	'STNO
	'	Body += RTrim(DT.Rows(i).Item("STNM").ToString).PadRight(18 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("STNM").ToString)).Length - RTrim(DT.Rows(i).Item("STNM").ToString).Length))	'STNM
	'	Body += RTrim(DT.Rows(i).Item("odkey").ToString)  'ODNO
	'	Body += RTrim(DT.Rows(i).Item("dpay").ToString).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("dpay").ToString)).Length - RTrim(DT.Rows(i).Item("dpay").ToString).Length), "0") 'AMT
	'	Body += RTrim(DT.Rows(i).Item("down").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("down").ToString)).Length - RTrim(DT.Rows(i).Item("down").ToString).Length))	'CUTKNM
	'	Body += " ".PadLeft(20)	'CUTKTL
	'	Body += "     "	'DCRONO
	'	Body += "D69" 'EDCNO
	'	Body += "0"	  'PRODNM

	'	Body += "誠品網路書店 www.eslite.com             " 'ECWEB
	'	Body += "(02)8789-8921       " 'ECSERTEL
	'	Body += RTrim(DT.Rows(i).Item("tots")).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("tots").ToString)).Length - RTrim(DT.Rows(i).Item("tots").ToString).Length), "0") 'REALAMT
	'	Body += "3"	 'TRADETYPE
	'	Body += "963" 'SERCODE
	'	Body += vbCrLf	'CRLF

	'	 P03(Body)
	'	BodyP += "2"  'RDFMT
	'	BodyP += "169"	  'ECNO
	'	BodyP += "TOK"	  'CNNO
	'	BodyP += RTrim(DT.Rows(i).Item("odkey").ToString)  'ODNO
	'	BodyP += RTrim(DT.Rows(i).Item("down").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("down").ToString)).Length - RTrim(DT.Rows(i).Item("down").ToString).Length)) 'CUTKNM
	'	BodyP += RTrim(DT.Rows(i).Item("dtel").ToString).PadRight(20 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("dtel").ToString)).Length - RTrim(DT.Rows(i).Item("dtel").ToString).Length)) 'CUTKTL
	'	BodyP += RTrim(Left(DT.Rows(i).Item("Weight").ToString, 5)).PadLeft(5 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("Weight").ToString)).Length - RTrim(DT.Rows(i).Item("Weight").ToString).Length), "0") 'Weight
	'	BodyP += RTrim(DT.Rows(i).Item("PackType").ToString).PadRight(1 - (System.Text.Encoding.Default.GetBytes(RTrim(DT.Rows(i).Item("PackType").ToString)).Length - RTrim(DT.Rows(i).Item("PackType").ToString).Length))	'Packtype
	'	BodyP += vbCrLf
	'	i += 1
	'  Next

	'   @Tail
	'  Tail = "3"	'RDFMT
	'  Tail += count.PadLeft(8, "0")	'RDCNT
	'  Tail += DT.Compute("SUM(dpay)", "").PadRight(9 - System.Text.Encoding.Default.GetBytes(DT.Compute("SUM(dpay)", "")).Length - RTrim(DT.Compute("SUM(dpay)", "").ToString.Length)) 'AMT
	'  TailP += Tail.PadRight(64, "0")

	'  Tail += DT.Compute("SUM(dpay)", "").ToString.PadRight(7, "0")	'AMT
	'  Tail += "".PadRight(150, "0")	 ' FIL2
	'  Tail += vbCrLf  'CRLF

	'  TailP += vbCrLf
	'  Head = Head + Body + Tail
	'  HeadP = HeadP + BodyP + TailP
	'   【2】. 將步驟1.的訂單資料寫入F03 , P03檔案
	'   將資料塞入txt檔()
	'  FileIO.FileSystem.WriteAllText((FilePath + FileName), Head, False, Encoding.GetEncoding(950))
	'  FileIO.FileSystem.WriteAllText((FilePathP + FileNameP), HeadP, False, Encoding.GetEncoding(950))
	'  ReOK_Value = ReOK_Value.Replace("\N", i) + " Export:" + FileName + "/" + FileNameP

	'Catch ex As Exception
	'  ReOK_Value += " [寫入F03 , P03檔案]: " & i & "row ,錯誤 Message:" & ex.Message.ToString
	'  FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'End Try
	'如需提供<JOBQsvr> 提供順便寫回JOBQ結果
	'Dim setV As String
	'Try
	'  If JOBQsvr = "UPDATE" And count > 0 Then
	'  setV = "Rdo=" + IIf(count > 0, "4", "-1") + ",SYS_USER='CKDsoFmt', rcount=" + count.ToString + ", tscount=" + i.ToString
	'  If FileIO.FileSystem.FileExists(FilePath + FileName) Then JobQSvrCmd("UPDATE", FilePath + FileName, setV, "and Rdo<9")
	'  If FileIO.FileSystem.FileExists(FilePathP + FileNameP) Then JobQSvrCmd("UPDATE", FilePathP + FileNameP, setV, "and Rdo<9")
	'  End If
	'Catch ex As Exception
	'End Try

	'CKDsoFmt = CType(ReOK_Value, SqlString)

 ' End Function

'Public Class XSDinf
'		Public Fcoding, FieldParser, tabN, INFType, FileType, BuName, ReOK_Value, impsFile As String
'		Public CommandText, CheckFun As String
'		Public XDoc As XDocument = New XDocument()
'		Public XApi As IEnumerable(Of XElement)
'		Public Vxsd As IEnumerable(Of XElement)
'		Public MapInf As IEnumerable(Of MapingBox)
'		Public Mds As New DataSet
'		Public Infxsd_ As String = Nothing													 '介面ID
'		'Public STKEY_ As String = Nothing												 '貨主	
'		'Public FPath_ As String = Nothing													'檔案本機路目錄
'		'Public EDIDs As New DataSet														 '對表內容
'		'Public Const MidHostStr As String = "context connection=true"	'指定本機DBbase Or 一般指資料庫
'		'Public sqlstr As String = ""


' 		Public Sub New(ByVal iInf As String, ByVal iStkey As String)
'			Infxsd_ = iInf				'設到共用變數
'			STKEY_ = iStkey
' 			'====取得:表 EDIInf,EDIFMap
'			Try

'			Catch ex As DataException

'					FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("SAPInf 錯誤描述  ERR: {0}   ", ex.Message) + Chr(10), True)
'					Exit Try
'			End Try

'		End Sub
'		''屬性建立 設定\取得
'		Public Property Infxsd() As String
'				Get
'						Return Me.Infxsd_
'				End Get
'				Set(ByVal Value As String)
'						Me.Infxsd_ = Value
'				End Set
'		End Property

'	 Public Class MapingBox
'			Public Tfield As String
'			Public Sqlfield As String
'			Public item As Integer
'			Public Length As Integer
'			Public SourceDt As String
'			Public DecDt As String
'	End Class
'	' 分析XSD 與Query DataSet 差異   
'	 Public Class XsdAnysDs
'			Public Dname As String
'			Public Sname As String
'			Public Qname As String
'					Public xdIts As Integer
'			Public MapCk As String
'					Public refkey As String
'			Public recnt As Integer
'	End Class

'End Class




 ' <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
 ' Public Shared Function MapBulkCopy(ByVal iInf As SqlString, ByVal iFpath As SqlString, ByVal iStkey As SqlString) As SqlString
	'Dim Inf As String = CType(iInf, String)		 '介面ID
	'Dim STKEY As String = CType(iStkey, String)		' "B2B" 貨主	
	'Dim UserCmd As String = "JobQ"
	'Dim rDo As Integer = 0

	'Dim oSAPMap As SAPiInf = New SAPiInf(Inf, STKEY)
	''預設 
	'  'Dim oSAPMap As EDIInf = New EDIInf(CType(iInf, String), CType(iStkey, String))
	'  Dim sqlstr As SqlString = ""
	'  '====取得:表 EDIInf,FEDIMap
	'  Dim lno As Integer = 0
	'  Dim cklin As Integer = 0
	'  '*** 指定目的表的[連接]及[表][欄位資訊]SServerAcc
	'  'DBConCls.EDIconstr = oSAPMap.MidHostStr 
	'  Dim MapInf As DataTable = oSAPMap.EDIDs.Tables("EDIFMap")
	'  Dim Mapdata As New DataTable
	'  Dim tabN As String = oSAPMap.Bulkin
	'  Dim TxtTagC As String = ChrW(oSAPMap.TagDelim.ToString)

	'  Dim ReOK_Value As String = "<<讀取 \N 筆，新增 \X 筆 TO " + tabN + " !>>"
	'  '**檔案欄位的分隔符號字元位置 及map table 建立

	'  Try
	'	For i As Integer = 0 To MapInf.Rows.Count - 1
	'	  Dim colname As String = MapInf.Rows(i)("EDIField").ToString.TrimEnd(" ")
	'	  Mapdata.Columns.Add(colname, GetType(String))
	'	Next
	'  '基本必要欄位 
	'  Mapdata.Columns.Add("edate", Type.GetType("System.DateTime"))
	'  Mapdata.Columns.Add("sfile", Type.GetType("System.String"))
	'  Mapdata.Columns.Add("etyps", Type.GetType("System.Int32"))
	'  Mapdata.Columns("edate").DefaultValue = CDate(Now.ToString("yyyy/MM/dd HH:mm:ss"))
	'  Mapdata.Columns("sfile").DefaultValue = iFpath.ToString
	'  Mapdata.Columns("etyps").DefaultValue = 1
	'  Catch ex As DataException
	'	ReOK_Value += "<MIDTABLE數據結構解析錯誤:" + ex.Message + " !>"
	'	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'  End Try

	'  Try
	'	Using myReader As New TextFieldParser(iFpath)
	'	  myReader.TextFieldType = FieldType.Delimited
	'	  myReader.SetDelimiters(TxtTagC)
	'	  'myReader.TrimWhiteSpace = True
	'	  myReader.HasFieldsEnclosedInQuotes = False

	'	  While Not myReader.EndOfData
	'		Dim TxtVal As String()
	'		TxtVal = myReader.ReadFields
	'		Mapdata.Rows.Add(TxtVal)
	'		lno += 1
	'	  End While
	'	End Using
	'  Catch ex As Exception
	'	ReOK_Value += "<讀取 \N 筆發生錯誤 ! " & ex.Message & ">"
	'	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'  End Try
	'  ReOK_Value = ReOK_Value.Replace("\N", lno)
	'  ' 回傳數據
	'  Dim Rtcode As String = ""
	'  Dim Ckcount As Long = 0

	'  Try
	'	'讀取大於10 才能執行大量
	'	'If iInf = "SP01" Or iInf = "SP07" Or iInf = "SP09" Or iInf = "SP10" Then
	'	If Mapdata.Rows.Count <= 10 Then
	'	  Rtcode = DBConCls.RowsCopyFun(Mapdata, tabN, 1)
	'	Else
	'	  Rtcode = DBConCls.TableCopyFun(Mapdata, tabN, 1)
	'	End If
	'  Catch ex As Exception
	'	ReOK_Value += " [新增]:" & Rtcode & "row ,錯誤 Message:" & ex.Message.ToString
	'	FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format(" 錯誤描述  ERR: {0}   ", ReOK_Value) + Chr(10), True)
	'  End Try

	'  '' 確認 iFpath 完成筆數
	'  Ckcount = DBConCls.ExcuteCount("SELECT count(*) from " + tabN + " where sfile='" + iFpath.ToString + "'")
	'  ReOK_Value = ReOK_Value.Replace("\X", Ckcount)

	'  '回傳 錯誤內容 暫無法傳到前端，改使用XML 記錄錯誤 
	'  If Ckcount = Mapdata.Rows.Count Then
	'	ReOK_Value += Rtcode
	'	If oSAPMap.JOBQmod = "ON" Then oSAPMap.JOBQ_Rdof(iFpath.ToString, {lno.ToString, Ckcount.ToString})
	'	'來源檔刪除
	'	  'File.Delete(iFpath.ToString)
	'  ElseIf Mapdata.Rows.Count <= 10 Then
	'	'RERBackUp
	'	FileIO.FileSystem.CopyFile(iFpath.ToString, DBConCls.RERBackUp & FileIO.FileSystem.GetFileInfo(iFpath.ToString).Name, True)
	'	Mapdata.DefaultView.RowFilter = "etyps<1"
	'	Dim Wxml As DataTable = Mapdata.DefaultView.ToTable(iInf.ToString)
	'	Wxml.WriteXml(iFpath.ToString.Replace(".", "") & "-內容錯誤協助.XML")
	'  Else
	'	FileIO.FileSystem.CopyFile(iFpath.ToString, DBConCls.RERBackUp & FileIO.FileSystem.GetFileInfo(iFpath.ToString).Name, True)
	'	ReOK_Value += "<2. 大量CopyToSQL 筆數錯誤:" & Rtcode & ">保留來源檔:" & iFpath.ToString
	'  End If
	'  If Not oSAPMap Is Nothing Then oSAPMap.Dispose()
	'  If oSAPMap Is Nothing Then ReOK_Value += "obj未清除!"
	'  MapBulkCopy = CType(ReOK_Value, SqlString)
 ' End Function



'Public Class XsdOut
'	Private Inf_ As String = Nothing		'介面ID
'	Private STKEY_ As String = Nothing	 '貨主	
'	Public EDIDs As New DataSet	'對表內容
'	Public Const MidHostStr As String = "context connection=true"	'指定本機DBbase Or 一般指資料庫
'	Public DServerAcc As String
'	'Public SServerAcc As String
'	Public JOBQmod As String
'	Public OutPath As String	'檔案EDI本機路目錄
'	Public EdiPath As String	' 客戶目錄
'	Public Bulkin As String		'共同PROC 查詢 
'	Public FFmtName As String	 '檔案格式
'	'Public Fmtname As String   '格式NAME
'	Public TagDelim As String	 '是否使用分隔符號
'	Public ExpTagC As String	'是否使用檔案分區
'	Public eRows As Long		 ' 無法在類別變動Public給函數中取用(待驗證如何取得)
'	''屬性建立 設定\取得
'	Public Property InfID() As String
'		Get
'			Return Me.Inf_
'		End Get
'		Set(ByVal Value As String)
'			Me.Inf_ = Value
'		End Set
'	End Property
'	Public Property STKEY() As String
'		Get
'			Return Me.STKEY_
'		End Get
'		Set(ByVal Value As String)
'			Me.STKEY_ = Value
'		End Set
'  End Property

'	'===========================================================================
'	' 中介數據文件資訊 
'	'EDIFMap :需要的欄位MAP ，EDIInf 轉介的程式方式
'	'select * from EDIInf WHERE INF='SP01'
'	'Stkey,Infid,InfCls,Subfun,Batpath,DescrImp,Bulkin,SServerAcc,DServerAcc,Fmtname,Ftype,logfile,IsUpdate, Fmtname ,ExpTagC>> 來源目地
'	'DoMark,Iseff, >> 控制
'	'EAIMod,EAIpath,Sourcefix,did,fixfile,fix2,back_one,back_two,back_thr,back_fou,  >> scan file
'	'Expmod,DescrExp,BuName     >> EXP file
'	'===========================================================================
'	Public Sub New(ByVal iInf As String, ByVal iStkey As String)
'		Inf_ = iInf				'設到共用變數
'		STKEY_ = iStkey

'		'sqlstr = "select * from EDIFMap WHERE INF='" & Inf_ & "' and Bxlen>0 order by cast(SN as int);select top 1 * from EDIInf WHERE InfCls='OutResTxt' and INFID='" & Inf_ & "' and STKEY='" & STKEY_ & "' ;"
'		Dim sqlstr As String = "select * from EDIFMap WHERE INF='" & Inf_ & "' and Bxlen>0 order by cast(SN as int);select top 1 * from EDIInf WHERE  INFID='" & Inf_ & "' and STKEY='" & STKEY_ & "' ;"

'		'====取得:表 EDIInf,EDIFMap
'		Try
'			Dim Adapter As SqlDataAdapter = New SqlDataAdapter(sqlstr, MidHostStr)
'			Adapter.Fill(EDIDs)
'			EDIDs.Tables(0).TableName = "EDIFMap"
'			EDIDs.Tables(1).TableName = "EDIInf"

'			OutPath = EDIDs.Tables("EDIInf").Rows(0)("EDILocDr").ToString
'			EdiPath = EDIDs.Tables("EDIInf").Rows(0)("EAIpath").ToString
'			Bulkin = EDIDs.Tables("EDIInf").Rows(0)("Bulkin").ToString

'			TagDelim = EDIDs.Tables("EDIInf").Rows(0)("TxtTagC").ToString
'			ExpTagC = EDIDs.Tables("EDIInf").Rows(0)("ExpTagC").ToString
'			JOBQmod = EDIDs.Tables("EDIInf").Rows(0)("JOBQmod").ToString
'			DBConCls.EDIconstr = EDIDs.Tables("EDIInf").Rows(0)("DServerAcc").ToString
'			'DBConCls.EDIconstr = EDIDs.Tables("EDIInf").Rows(0)("SServerAcc").ToString
'			Adapter.Dispose()
'		Catch ex As Exception
'			'Console.Write("錯誤描述：" & ex.Message)
'			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("SAPInf 錯誤描述  ERR: {0}   ", ex.Message) + Chr(10), True)
'			Exit Try
'		End Try

'		'EDIDs = CInfData(sqlstr, MidHostStr)
'	End Sub


''執行 建立前端讀取用 Table 及SqlCopy.ColumnMappings
'	'Public Function MidTable(ByVal cmdstr As String) As DataTable

'	'End Function

''=========JOBQmod ON 模式 : 接口Infid 
''EDIT:  
'	'來源接口檔Infid SCAN後的處理記錄
'	'使於匯出檔案(成功或失敗),後續給MoveFile 備份用
'	'生成rdo=NULL ，成功rdo=0 ,失敗rdo=-1
''==========================
'	Public Sub JOBQ_Newf(ByVal Wkey As String, ByVal LotKeys() As String)

'		Dim CmdJobq As New StringBuilder("INSERT INTO SJOB..JobQ(STKEY, Wkey, InfID, Jtyps, sfile, Rfkey1, Rfkey2, lotkey1, lotkey2, rdo, exfile, tscount) ")
'		CmdJobq.Append("SELECT  Stkey,Wkey='@Wkey',Infid ,Jtyps='@Jtyps',  sfile=EDILocDr , Rfkey1=back_one , Rfkey2=EAIpath , LotKey1='@LotKey1', LotKey2='@LotKey2',rDo=null ,exfile=EDILocDr+ '@Wkey',tscount=@its ")
'		CmdJobq.Append("from  EDIinf  where Infid='@Infid' and Stkey='@Stkey' ")
'		Try
'			CmdJobq.Replace("@Infid", Me.InfID)
'			CmdJobq.Replace("@Stkey", Me.STKEY)
'			CmdJobq.Replace("@Wkey", Wkey)
'			CmdJobq.Replace("@Jtyps", LotKeys(0).ToString)
'			CmdJobq.Replace("@LotKey1", LotKeys(1).ToString)
'			CmdJobq.Replace("@LotKey2", LotKeys(2).ToString)
'			CmdJobq.Replace("@its", Me.eRows)
'			JOBQ_Cmd(CmdJobq.ToString)
'			'Console.WriteLine("匯出文字、XML: " & Wkey & "--查詢:" & Me.QueryProc)
'		Catch ex As DataException
'			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("JOBQ log 產生錯誤: {0}   ", ex.Message) + Chr(10), True)
'			Console.WriteLine(CmdJobq)
'		End Try
'	End Sub
''=========JOBQmod ON 模式 : 讀取後的記錄更新
''EDIT:  
''JOBQ Rdo  讀取後的記錄更新
''==========================
'	Public Sub JOBQ_Rdof(ByVal Wkey As String, ByVal LotKeys() As String)
'			Dim CmdJobq As New StringBuilder("UPDATE SJOB..JobQ set rdo=3, udate=getdate(),rcount=@LotKey1,LotKey1='@LotKey2' where rdo<3 and Wkey='@Wkey'")
'			CmdJobq.Replace("@LotKey1", LotKeys(1).ToString)
'			CmdJobq.Replace("@LotKey2", "讀/寫:" + LotKeys(0).ToString + "/" + LotKeys(1).ToString)
'			Wkey = IIf(Wkey.LastIndexOf("\") > 0, Wkey.Substring(Wkey.LastIndexOf("\") + 1), Wkey)
'			Wkey = IIf(Wkey.LastIndexOf("/") > 0, Wkey.Substring(Wkey.LastIndexOf("/") + 1), Wkey)
'			CmdJobq.Replace("@Wkey", Wkey)
'		Try
'			JOBQ_Cmd(CmdJobq.ToString)
'		Catch ex As Exception
'			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("Sub JOBQ_Rdof 產生錯誤: {0}   ", ex.Message) + Chr(10), True)
'		End Try
'	End Sub


''=========執行有關JOBQ Command 
'	'MidHostStr="context connection=true" 在一個連線訊息 sqlfunction 禁止使用
'	'需要另外連線 Data Source=LOCALHOST\SJOB;Initial Catalog=SJOB;Persist Security Info=True;User ID=sa;Password=zaq1
''===================================
'	Public Function JOBQ_Cmd(ByVal cmdstr As String) As Integer
'		JOBQ_Cmd = -1
'		If Me.JOBQmod <> "ON" Then
'			JOBQ_Cmd = "停用 JOBQ 服務 !"
'			Exit Function
'		End If
'		Try
'			Using conJob As New SqlClient.SqlConnection("Data Source=LOCALHOST\SJOB;Initial Catalog=SJOB;Persist Security Info=True;User ID=sa;Password=zaq1")
'					Using JOBM As New SqlClient.SqlCommand(cmdstr, conJob)
'					conJob.Open()
'					JOBQ_Cmd = JOBM.ExecuteNonQuery()
'					End Using
'			End Using
'		Catch ex As Exception
'			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("EDI.JOBQ_Cmd: 錯誤描述  ERR: {0}   ", ex.Message) + Chr(10) + cmdstr, True)
'		End Try

'	End Function


'End Class