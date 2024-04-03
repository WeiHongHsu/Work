Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Collections
Imports System.Text
Imports Microsoft.VisualBasic

Imports System.Xml.Linq
Imports System.IO
Imports System.Xml

'Public Class INIReader
'	Public Shared Function GIniValue(ByVal sSectionName As String, ByVal sKeyName As String, ByVal sFilename As String) As String
'		If sFilename = "" Then sFilename = "D:\SJOB\SQLCLR\Eslite.ini"
'		Dim sbKeyValue As New StringBuilder(1024)
'		Dim NumRet As Integer = GetPrivateProfileString(sSectionName, sKeyName, "", sbKeyValue, 1024, sFilename)
'		Return (sbKeyValue.ToString().Trim())
'	End Function
'	<DllImport("kernel32")>
'	Private Shared Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
'	End Function
'End Class
'===================================
'目的資料庫的數據處理層及連線 
' 前端指訂定: EDIconstr 一次後 其他Function 即共用 。
'Data Source=192.168.1.33;Persist Security Info=True;Password=zaq1;User ID=sa;Initial Catalog=iexp
'====================================
Public NotInheritable Class DBConCls
	'-- 中間庫位置@Password    pstG)MM*NYJcVh*FQ830!A!j

	'Public Shared EDIconstr As String = "Data Source=192.168.1.52;Persist Security Info=True;Password=@Password;User ID=CLR_Executor;Initial Catalog=iexp"
	Public Shared EDIconstr As String = "Initial Catalog=iexp;Data Source=localhost;Integrated Security=SSPI"
	'Public Shared CONNs As String = "Data Source=192.168.1.52;Persist Security Info=True;Password=@Password;User ID=CLR_Executor;Initial Catalog=SAPi"
	'TODO 20220412 Raines Change conns from password to ad
	Public Shared CONNs As String = "Initial Catalog=iexp;Data Source=localhost;Integrated Security=SSPI;"
	'--EDI JobQ 位置
	'Public Shared JobQConstr As String = "Data Source=192.168.1.52;Persist Security Info=True;Password=@Password;User ID=CLR_Executor;Initial Catalog=SJOB"
	Public Shared JobQConstr As String = "Initial Catalog=SJOB;Data Source=localhost;Integrated Security=SSPI;"
	Public Shared Xdpath As String = "D\SJOB\fmt\xsd"	 '尚未使用
	Public Shared APPLog As String = "D:\SJOB\fmt\XsdBulkCopy.log"
	Public Shared RERBackUp As String = "D:\SJOB\BulkCopyErr\"	  ' 錯誤的檔案存放位置
	Public Shared ExcMag As String = ""
	'Public Shared Function GIniValue(ByVal sSectionName As String, ByVal sKeyName As String, ByVal sFilename As String) As String
	'	If sFilename = "" Then sFilename = "D:\SJOB\SQLCLR\Eslite.ini"
	'	Dim sbKeyValue As New StringBuilder(1024)
	'	Dim NumRet As Integer = GetPrivateProfileString(sSectionName, sKeyName, "", sbKeyValue, 1024, sFilename)
	'	Return (sbKeyValue.ToString().Trim())
	'End Function
	'<DllImport("kernel32")>
	'Private Shared Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
	'End Function


	'==================================
	' 傳回DataTable 表內容 
	'===================================
	Public Overloads Shared Function sqlTmpData(ByVal ProName As String) As DataTable
		Dim Ts As New DataTable
		If ProName.IndexOf("SELECT") = -1 And ProName.IndexOf("EXEC") = -1 Then ProName = "EXEC " + ProName
		Using con As New SqlClient.SqlConnection(EDIconstr)
			Using cmd As New SqlCommand(ProName)
				cmd.Connection = con
				cmd.CommandTimeout = 18000
				Try
					Using Adapter As New SqlDataAdapter(cmd)
						Adapter.Fill(Ts)
						con.Dispose()
					End Using
				Catch ex As DataException
					ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpDs-B] ex.Message: " + ex.Message
					FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + vbCrLf, True)
				End Try
			End Using
		End Using
		Return Ts
	End Function
	'================
	' 使用 proc 傳回 Result's 給Datatables
	' sqlstr : 程序名稱
	'<修改> Select command 與 exec 共用
	'================
	Public Overloads Shared Function sqlTmpDs(ByVal ProName As String) As DataSet
		Dim Ds As New DataSet
		If ProName.IndexOf("SELECT") = -1 And ProName.IndexOf("EXEC") = -1 Then ProName = "EXEC " + ProName
		Using con As New SqlClient.SqlConnection(EDIconstr)
			Using cmd As New SqlCommand(ProName)
				cmd.Connection = con
				cmd.CommandTimeout = 18000
				Try
					Using Adapter As New SqlDataAdapter(cmd)
						Adapter.Fill(Ds)
						con.Dispose()
					End Using
				Catch ex As DataException
					ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpDs-B] ex.Message: " + ex.Message
					FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + vbCrLf, True)

				End Try
			End Using
		End Using
		Return Ds
	End Function

	Public Overloads Shared Function sqlTmpDs(ByVal ProName As String, ByRef Ds As DataSet) As Integer
		sqlTmpDs = 0
		If ProName.IndexOf("SELECT") = -1 And ProName.IndexOf("EXEC") = -1 Then ProName = "EXEC " + ProName
		Using con As New SqlClient.SqlConnection(EDIconstr)
			Using cmd As New SqlCommand(ProName)
				cmd.Connection = con
				cmd.CommandTimeout = 30000
				Try
					Using Adapter As New SqlDataAdapter(cmd)
						Adapter.Fill(Ds)
						For i = 0 To Ds.Tables.Count - 1
							sqlTmpDs += Ds.Tables(i).Rows.Count
						Next
						con.Dispose()
					End Using
				Catch ex As DataException
					ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpDs] ex.Message: " + ex.Message
					FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + vbCrLf, True)
					Exit Try
				End Try
			End Using
		End Using
		Return sqlTmpDs
	End Function
	'Public Class ParamSqls
	'	Public Value As Integer
	'	Public SqlType As Integer
	'	Public Name As String
	'	Public Sub New(Value As Integer, SqlType As SqlDbType, Name As String)
	'		Me.Value = Value
	'		Me.SqlType = SqlType
	'		Me.Name = Name
	'	End Sub
	'End Class
	'================
	' 程序名稱 : _Edi_OutPut_Fmt  @InfMod , @Wkey ,  @Extkeys  , @UserID , @Msg   OutPut  (4 in參數+out 參數)
	' sqlstr : 程序名稱
	'<修改> Select command 與 exec 共用
	'================
	Public Shared Function ExpOKSP(ByVal ProName As String, ByVal Par3Val As String) As Integer
		ExpOKSP = -1
		Dim ValNs As String() = Par3Val.Split(","c)
		Try
			Using con As New SqlClient.SqlConnection(CONNs)
				Using objcmd As New SqlClient.SqlCommand(ProName, con)
					con.Open()
					For i As Integer = 0 To UBound(ValNs, 1)
						'Dim SqlParn As New SqlParameter
						objcmd.Parameters.Add(New SqlParameter("@Par" & i + 1, ValNs(i)))
					Next
					objcmd.CommandTimeout = 1800
					ExpOKSP = objcmd.ExecuteNonQuery()
				End Using
			End Using
		Catch ex As DataException
			ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.ExpOKSP] ex.Message: " + ex.Message
			FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + vbCrLf, True)
		End Try
	End Function

	'==================================
	' 傳回DataReader 內容於前端DATASET 使用
	'===================================
	Public Shared Function selReader(ByVal sqlstr As String) As SqlDataReader
		Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand()
		Dim reader As SqlClient.SqlDataReader = Nothing
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
		cmd.CommandTimeout = 1800
		Try
			reader = cmd.ExecuteReader()
			con.Dispose()
		Catch ex As Exception
			FileIO.FileSystem.WriteAllText(APPLog, String.Format(Now.ToString("yyyy/MM/dd hh:mm:ss") & "[DBConCls.selReader] ERR: {0}   ", ex.Message) + vbCrLf, True)
			Exit Try
		End Try
		Return reader
	End Function
	'==================================
	'  ExecuteXmlReader 傳回XMLReader 內容 轉成STRING 給前端 使用
	'===================================
	Public Shared Function sqlTmp2Xml(ByVal ProName As String, ByRef its As Integer) As String
		'sqlTmp2Xml = "<?xml version='1.0' encoding='UTF-8'?>"
		Dim Ts As New DataTable
		Using con As New SqlClient.SqlConnection(EDIconstr)
			Using cmd As New SqlCommand(ProName)
				cmd.Connection = con
				cmd.CommandTimeout = 18000
				Try
					Using Adapter As New SqlDataAdapter(cmd)
						Adapter.Fill(Ts)
						sqlTmp2Xml += Ts.Rows(0)(0).ToString
						its = 1
						con.Dispose()
					End Using
				Catch ex As DataException
					'Dim Pr() As String = {"DBConCls.sqlTmp2Xml", ProName, sqlTmp2Xml, "<SQL XML產生錯誤>", ex.Message}
					'ErrFmtLog(Pr)
					ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpDs-B] ex.Message: " + ex.Message
					FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + vbCrLf, True)
				End Try
			End Using
		End Using

	End Function

	Public Shared Function SQLXmlRW(ByVal sqlstr As String, ByVal expPath As String, ByRef its As Integer) As String
		SQLXmlRW = "-1"
		'Dim wstr As String
		Dim ds As New DataSet
		Dim doc As New Xml.XmlDataDocument()
		Try
			Using con As New SqlConnection(EDIconstr)
				Dim command As New SqlClient.SqlCommand(sqlstr, con)
				con.Open()
				Dim RXml As Xml.XmlReader = command.ExecuteXmlReader()
				If RXml.EOF = False Then RXml.Read()
				doc.Load(RXml)

				If doc.LastChild.ChildNodes.Count > 0 Then
					its = doc.ChildNodes.Count
					doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "UTF-8", Nothing), doc.DocumentElement)
					doc.Save(expPath)
					SQLXmlRW = "OK"
				Else
					its = 0
				End If
				RXml.Close()
				con.Dispose()
			End Using
		Catch ex As Exception
			Dim Pr() As String = {"DBConCls.SQLXmlRW(...)", sqlstr, expPath, "< doc.Load匯出驗證>", ex.Message}
			SQLXmlRW = ErrFmtLog(Pr)

			Exit Try
		End Try

	End Function
	Public Shared Function SQLXmlzpRW(ByVal sqlstr As String, ByVal expPath As String, ByRef its As Integer, ByVal zpLf As Boolean) As String
		SQLXmlzpRW = "-1"
		'Dim wstr As String
		Dim ds As New DataSet
		Dim doc As New Xml.XmlDataDocument()
		Try
			Using con As New SqlConnection(EDIconstr)
				Dim command As New SqlClient.SqlCommand(sqlstr, con)
				con.Open()
				Dim RXml As Xml.XmlReader = command.ExecuteXmlReader()
				If RXml.EOF = False Then RXml.Read()
				If zpLf = True Then
					doc.PreserveWhitespace = False
				End If
				doc.Load(RXml)

				If doc.LastChild.ChildNodes.Count > 0 Then
					its = doc.ChildNodes.Count
					doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "UTF-8", Nothing), doc.DocumentElement)
					If zpLf = True Then
						doc.PreserveWhitespace = False
					End If
					doc.Save(expPath)
					SQLXmlzpRW = "OK"
				Else
					its = 0
				End If
				RXml.Close()
				con.Dispose()
			End Using
		Catch ex As Exception
			Dim Pr() As String = {"DBConCls.SQLXmlRW(...)", sqlstr, expPath, "< doc.Load匯出驗證>", ex.Message}
			SQLXmlzpRW = ErrFmtLog(Pr)

			Exit Try
		End Try

	End Function
	' var xn = xp.CreateNavigator();
	'XmlNode root = xmlDocument.CreateElement("YourFavouriteRootElementName");
	'root.InnerXml = xn.OuterXml;
	'xmlDocument.AppendChild(root);
	'==================================
	' 傳回 SQL 執行結果 於前端 確認數量、處理
	'<20200608> 修改錯誤格式
	'===================================
	Public Shared Function ExcuteCount(ByVal sqlstr As String) As Integer
		'Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(CONNs)
		'Dim objcmd As SqlClient.SqlCommand
		ExcuteCount = 0
		Try
			Using con As New SqlClient.SqlConnection(CONNs)
				Using objcmd As New SqlClient.SqlCommand(sqlstr, con)
					con.Open()
					objcmd.CommandTimeout = 1800
					ExcuteCount = CType(objcmd.ExecuteScalar, Integer)
				End Using
			End Using

		Catch ex As Exception
			ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0}[{1}	,{2}	...]" & vbTab & "錯誤Message:" + ex.Message & vbCrLf
			FileIO.FileSystem.WriteAllText(APPLog, String.Format(ExcMag, "DB.ExcuteCount]", sqlstr, CONNs), True)
			ExcuteCount = -1
		End Try
	End Function

	'=====================ExcuteSql======
	' 傳回 Excute SQL命令 執行處理
	'===================================
	Public Overloads Shared Function ExcuteSql(ByVal sqlstr As String) As Integer
		'Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
		'Dim objcmd As SqlClient.SqlCommand
		ExcuteSql = 0
		Try
			Using con As New SqlClient.SqlConnection(EDIconstr)
				Using objcmd As New SqlClient.SqlCommand(sqlstr, con)
					con.Open()
					objcmd.CommandTimeout = 1800
					ExcuteSql = objcmd.ExecuteNonQuery()
				End Using
			End Using
		Catch ex As Exception
			ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0}[{1}	,{2}	...]" & vbTab & "錯誤Message:" + ex.Message & vbCrLf
			FileIO.FileSystem.WriteAllText(APPLog, String.Format(ExcMag, "ExcuteSql-A", sqlstr, ""), True)
			ExcuteSql = -1
		End Try
	End Function
	Public Overloads Shared Function ExcuteSql(ByVal sqlstr As String, ByVal constr As String) As Integer
		ExcuteSql = -1
		Try
			Using con As New SqlClient.SqlConnection(constr)
				Using objcmd As New SqlClient.SqlCommand(sqlstr, con)
					con.Open()
					objcmd.CommandTimeout = 1800
					ExcuteSql = objcmd.ExecuteNonQuery()
				End Using
			End Using
		Catch ex As Exception
			ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0}[{1}	,{2}	...]" & vbTab & "錯誤Message:" + ex.Message & vbCrLf
			FileIO.FileSystem.WriteAllText(APPLog, String.Format(ExcMag, "ExcuteSql-B", sqlstr, constr), True)
			ExcuteSql = -1
		End Try
	End Function




	''----------------------------- 以上為於SQL 執行傳會回前端UI  ----------------------------------------------

	''==================SqlBulkCopy 使用 大量新增速度快===========================
	'' mp: 當欄位不同時，需使使用 ColumnMap ，且中介數據欄位NAME需存在目的數據表(標準版將取消)
	'' 傳回字傳: N 筆OK  , 1 : 執行中， -1 錯誤及內容
	Public Shared Function TableCopyFun(ByVal MidTab As DataTable, ByVal des As String, Optional ByVal mp As Integer = 0) As String
		'Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(CONNs)
		'Dim SqlCopy As SqlBulkCopy
		con.Open()
		Dim SqlCopy = New SqlBulkCopy(con)
		SqlCopy.BatchSize = 10000
		SqlCopy.BulkCopyTimeout = 1800 '30分
		SqlCopy.DestinationTableName = des
		TableCopyFun = 0
		'處理完後丟出一個事件, 或是說處理幾筆後就丟出事件
		'SqlCopy.NotifyAfter = MidTab.Rows.Count
		'SqlCopy.SqlRowsCopied= New SqlRowsCopiedEventHandler(事件_SqlRowsCopied)
		Try
			If mp = 1 Then
				For i As Integer = 0 To MidTab.Columns.Count - 1
					Dim colnameMap As String = MidTab.Columns(i).ColumnName
					SqlCopy.ColumnMappings.Add(MidTab.Columns(i).ColumnName, MidTab.Columns(i).ColumnName)
				Next
			End If
			' 一次匯入，如果目的表長度不足無法Debug
			Try
				SqlCopy.WriteToServer(MidTab)
				TableCopyFun = "OK !"
			Catch ex As Exception
				TableCopyFun = "Err:" & ex.Message.ToString
			End Try
		Catch ex As SqlException
			TableCopyFun = "Err:" & ex.Message.ToString
		End Try
		SqlCopy.Close()
		con.Close()
		con.Dispose()
	End Function

	'======================SqlBulkCopy 單筆執行 速度變慢: 因SP01 SP09或追查原因好用==============================
	'' 當欄位不同時，需使使用 ColumnMap ，且中介數據欄位NAME需存在目的數據表
	'' 傳回字傳: N 筆OK  , 1 : 執行中， -1 錯誤及內容
	Public Shared Function RowsCopyFun(ByVal MidTab As DataTable, ByVal des As String, Optional ByVal mp As Integer = 0) As String
		Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
		Dim SqlCopy As SqlBulkCopy
		Dim Copytis As Integer = 0
		Dim rits As Integer = 0
		con.Open()
		SqlCopy = New SqlBulkCopy(con)
		SqlCopy.BatchSize = 1
		SqlCopy.DestinationTableName = des
		RowsCopyFun = -1
		Try
			If mp = 1 Then
				For i As Integer = 0 To MidTab.Columns.Count - 1
					Dim colnameMap As String = MidTab.Columns(i).ColumnName
					SqlCopy.ColumnMappings.Add(MidTab.Columns(i).ColumnName, MidTab.Columns(i).ColumnName)
				Next
			End If
		Catch ex As SqlException
			Return "MAP Err:" & ex.Message.ToString
			Exit Function
		End Try
		'1筆的方式 While
		'Dim TabRd As New DataTableReader(MidTab)
		Dim ixRow() As DataRow
		For i As Integer = 0 To MidTab.Rows.Count - 1
			MidTab.Rows(i)("etyps") = 2
			ixRow = MidTab.Select("etyps=2")
			Try
				If ixRow.Length > 0 Then
					SqlCopy.WriteToServer(ixRow)
					MidTab.Rows(i)("etyps") = 9
					Copytis += 1
				End If
			Catch ex As Exception
				MidTab.Rows(i)("etyps") = -1
				MidTab.Rows(i)("sfile") = ex.Message.ToString
			End Try

			MidTab.AcceptChanges()
		Next

		RowsCopyFun = CStr(Copytis)
	End Function


	Public Shared Function ErrFmtLog(ByVal Pr() As String) As String
		ErrFmtLog = ""
		Try
			Dim ErrFmt As String = Now.ToString("yyyy/MM/dd hh:mm:ss") & vbTab & "{0,-25}	{1,-20}	{2,-20}	ErrMessage:  {4,-50}" & vbCrLf
			ErrFmtLog = String.Format(ErrFmt, Pr)
			FileIO.FileSystem.WriteAllText(APPLog, ErrFmtLog, True)
		Catch ex As Exception
			FileIO.FileSystem.WriteAllText(APPLog, "ErrFmtLog()錯誤Log 本身" + ex.Message, True)
		End Try
	End Function



End Class


'Public Class EDIInf
'	Public Inf_ As String = Nothing													 '介面ID
'	Public STKEY_ As String = Nothing												 '貨主	
'	Public FPath_ As String = Nothing													'檔案本機路目錄
'	Public EDIDs As New DataSet														 '對表內容
'	Public Const MidHostStr As String = "context connection=true"	'指定本機DBbase Or 一般指資料庫
'	Public sqlstr As String = ""
'	'===========================================================================
'	' 中介數據文件資訊
'	'select * from EDIInf WHERE INF='SP01'
'	'文件格式範例 EDIInf
'	'Stkey,Infid,InfCls,Subfun,Batpath,DescrImp,Bulkin,SServerAcc,DServerAcc,Fmtname,Ftype,logfile,IsUpdate, >> 來源目地
'	'DoMark,Iseff, >> 控制
'	'EAIMod,EAIpath,Sourcefix,did,fixfile,fix2,back_one,back_two,back_thr,back_fou,  >> scan file
'	'impBkTab,
'	'Expmod,DescrExp,BuName     >> EXP file
'	'===========================================================================
'	Public Sub New(ByVal iInf As String, ByVal iStkey As String)
'		Inf_ = iInf				'設到共用變數
'		'InfID = iInf			'設到屬性
'		'FPath_ = iFpath
'		STKEY_ = iStkey
'		sqlstr = "select * from EDIFMap WHERE INF='" & Inf_ & "' and Bxlen>0 order by cast(SN as int);select top 1 * from EDIInf WHERE INFID='" & Inf_ & "' and STKEY='" & STKEY_ & "' ;"	   ' and infCls='InfMap'
'		'====取得:表 EDIInf,EDIFMap
'		Try
'			Dim Adapter As SqlDataAdapter = New SqlDataAdapter(sqlstr, MidHostStr)
'			Adapter.Fill(EDIDs)
'			EDIDs.Tables(0).TableName = "EDIFMap"
'			EDIDs.Tables(1).TableName = "EDIInf"
'			Adapter.Dispose()
'		Catch ex As DataException
'			'Console.Write("錯誤描述：" & ex.Message)
'			FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("SAPInf 錯誤描述  ERR: {0}   ", ex.Message) + vbCrLf, True)
'			Exit Try
'		End Try
'		'EDIDs = CInfData(sqlstr, MidHostStr)
'	End Sub
'	''屬性建立 設定\取得
'	Public Property InfID() As String
'		Get
'			Return Me.Inf_
'		End Get
'		Set(ByVal Value As String)
'			Me.Inf_ = Value
'		End Set
'	End Property
'End Class

'===========================================================================
'文件資訊 (此為暫時使用) , EAIInf 的
'select * from ImpInf WHERE INF='?' and STKEY='?'    ; select * from FRows where 1<>1
'===========================================================================
'Public Class EAIInf
'		Public EDIDs As New DataSet														 '對表內容
'		Private Const MidHostStr As String = "context connection=true" '指定本機DBbase Or 一般指資料庫
'		'Public sqlstr As String = ""

'		Public Sub New(ByVal iInf As String, ByVal iStkey As String)
'        Dim sqlstr As String = "select * from EDIInf WHERE INFID='" + iInf + "' and STKEY='" & iStkey & "' and infCls='imp_SAP';select * from FRows where 1<>1"
'			'====取得:表 ImpInf
'			Try
'					Dim Adapter As SqlDataAdapter = New SqlDataAdapter(sqlstr, MidHostStr)
'					Adapter.Fill(EDIDs)
'					EDIDs.Tables(0).TableName = "EAIInf"
'					EDIDs.Tables(1).TableName = "FRows"
'					Adapter.Dispose()
'			Catch ex As DataException
'					'Console.Write("錯誤描述：" & ex.Message)
'					FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("SAPInf 錯誤描述  ERR: {0}   ", ex.Message) + vbCrLf, True)
'					Exit Try
'			End Try

'		End Sub
'End Class