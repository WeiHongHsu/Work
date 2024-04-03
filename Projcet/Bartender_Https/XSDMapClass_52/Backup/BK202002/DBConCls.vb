Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server

Imports System.Runtime.InteropServices
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Security.Principal

'Public Class INIReader
'		Public Shared Function GIniValue(ByVal sSectionName As String, ByVal sKeyName As String, ByVal sFilename As String) As String
'			If sFilename = "" Then sFilename = "D:\SQLCLR\Eslite.ini"
'			Dim sbKeyValue As New StringBuilder(1024)
'			Dim NumRet As Integer = GetPrivateProfileString(sSectionName, sKeyName, "", sbKeyValue, 1024, sFilename)
'			Return (sbKeyValue.ToString().Trim())
'		End Function
'		<DllImport("kernel32")> _
'		Private Shared Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
'		End Function
'End Class
'===================================
'目的資料庫的數據處理層及連線 
' 前端指訂定: EDIconstr 一次後 其他Function 即共用 。
'Data Source=192.168.1.33;Persist Security Info=True;Password=zaq1;User ID=sa;Initial Catalog=iexp
'====================================
Public NotInheritable Class DBConCls
	Public Shared EDIconstr As String = "Data Source=192.168.1.52;Persist Security Info=True;Password=zaq1;User ID=sa;Initial Catalog=iexp"
	Public Shared CONNs As String = "Data Source=192.168.1.52;Persist Security Info=True;Password=zaq1;User ID=sa;Initial Catalog=SAPi"

	'Data Source=LOCALHOST\SJOB;Initial Catalog=SJOB;User ID=sa    \ Data Source=192.198.1.33\SJOB;Initial Catalog=iexp;Persist Security Info=True;User ID=sa;Password=zaq1
	Public Shared JobQConstr As String = "Data Source=192.168.1.33\SJOB;Persist Security Info=True;Password=zaq1;User ID=sa;Initial Catalog=SJOB"
	Public Shared Xdpath As String = "\\192.168.1.33\SJOB\fmt\xsd"	'尚未使用
	Public Shared APPLog As String = "D:\SJOB\fmt\XsdBulkCopy.log"
	Public Shared RERBackUp As String = "D:\SJOB\BulkCopyErr\"	' 錯誤的檔案存放位置
	Public Shared ExcMag As String = ""

  '==================================
  ' 傳回DataTable 表內容於 UI 檢視
  '===================================
	Public Shared Function sqlTmpData(ByVal sqlstr As String) As DataTable
			Dim objdatastable As New DataTable()
			Dim Adapter As SqlClient.SqlDataAdapter
			Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)

			Dim cmd As SqlClient.SqlCommand = New SqlClient.SqlCommand(sqlstr, con)
			cmd.CommandTimeout = 1800
			Try
					Adapter = New SqlClient.SqlDataAdapter(cmd)
					objdatastable = New DataTable
          Adapter.Fill(objdatastable)
          con.Close()
					con.Dispose()
			Catch ex As DataException
					  ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpData] ex.Message: " + ex.Message
					  FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + Chr(10), True)
					Exit Try
			End Try
			Return objdatastable
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
			  cmd.CommandTimeout = 180
				Try
				  Using Adapter As New SqlDataAdapter(cmd)
					Adapter.Fill(Ds)
					con.Dispose()
				   End Using
				Catch ex As DataException
					  ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpDs-B] ex.Message: " + ex.Message
					  FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + Chr(10), True)

				End Try
			End Using
		  End Using
		  Return Ds
	End Function
	Public Overloads Shared Function sqlTmpDs(ByVal ProName As String, ByRef Ds As DataSet) As Integer
		sqlTmpDs = -1
		   If ProName.IndexOf("SELECT") = -1 And ProName.IndexOf("EXEC") = -1 Then ProName = "EXEC " + ProName
		  Using con As New SqlClient.SqlConnection(EDIconstr)
			Using cmd As New SqlCommand(ProName)
			  cmd.Connection = con
			  cmd.CommandTimeout = 1800
				Try
				  Using Adapter As New SqlDataAdapter(cmd)
					sqlTmpDs = Adapter.Fill(Ds)
					con.Dispose()
				   End Using
				Catch ex As DataException
					  ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.sqlTmpDs] ex.Message: " + ex.Message
					  FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + Chr(10), True)
					Exit Try
				End Try
			End Using
		  End Using
		  Return sqlTmpDs
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
					FileIO.FileSystem.WriteAllText(APPLog, String.Format(Now.ToString("yyyy/MM/dd hh:mm:ss") & "[DBConCls.selReader] ERR: {0}   ", ex.Message) + Chr(10), True)
					Exit Try
			End Try
			Return reader
	End Function
  '==================================
  ' 傳回 SQL 執行結果 於前端 確認數量、處理
  '===================================
  Public Shared Function ExcuteCount(ByVal sqlstr As String) As Integer
	Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
	Dim objcmd As SqlClient.SqlCommand
	ExcuteCount = 0
	Try
		con.Open()
		objcmd = New SqlClient.SqlCommand(sqlstr, con)
		objcmd.CommandTimeout = 180
		ExcuteCount = objcmd.ExecuteScalar
	Catch ex As Exception
	  ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.ExcuteCount] ex.Message: " + ex.Message
    FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述: {0}  cmdAry:{1}", ExcMag, sqlstr) + Chr(10), True)
	  ExcuteCount = -1
	End Try
  End Function
  '=====================ExcuteSql======
  ' 傳回 Excute SQL命令 執行處理
  '===================================
  Public Shared Function ExcuteSql(ByVal sqlstr As String) As Integer
	Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
	Dim objcmd As SqlClient.SqlCommand
	ExcuteSql = 0
	Try
		con.Open()
		objcmd = New SqlClient.SqlCommand(sqlstr, con)
		objcmd.CommandTimeout = 1800
		ExcuteSql = objcmd.ExecuteNonQuery()
		con.Close()
		con.Dispose()
	Catch ex As Exception
		ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "	" & "[DBConCls.ExcuteSql] ex.Message: " + ex.Message
		FileIO.FileSystem.WriteAllText(APPLog, String.Format("錯誤描述 : {0}", ExcMag) + Chr(10), True)
		con.Close()
		con.Dispose()
	  ExcuteSql = -1
	End Try
  End Function

  '=====================ExcuteSql======
  ' 傳回 Excute SQL命令 執行處理; 改自定義 SqlConnection 傳入
  '===================================
  Public Shared Function ExcuteSql(ByVal sqlstr As String, iconn As String) As Integer
	Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(iconn)
	Dim objcmd As SqlClient.SqlCommand
	ExcuteSql = 0
	Try
		con.Open()
		objcmd = New SqlClient.SqlCommand(sqlstr, con)
		objcmd.CommandTimeout = 1800
		ExcuteSql = objcmd.ExecuteNonQuery()
		con.Close()
		con.Dispose()
	Catch ex As Exception
		ExcMag = Now.ToString("yyyy/MM/dd hh:mm:ss") & "[DBConCls.ExcuteSql-B] ERR:' " + ex.Message
		FileIO.FileSystem.WriteAllText(APPLog, String.Format(Now.ToString("yyyy/MM/dd hh:mm:ss") & "[DBConCls.ExcuteSql] ERR: {0}", ex.Message) + Chr(10) + con.ConnectionString + Chr(10), True)
		con.Close()
		con.Dispose()
	  ExcuteSql = -1
	End Try
  End Function


  ''----------------------------- 以上為於SQL 執行傳會回前端UI  ----------------------------------------------

''==================SqlBulkCopy 使用 大量新增速度快===========================
'' mp: 當欄位不同時，需使使用 ColumnMap ，且中介數據欄位NAME需存在目的數據表(標準版將取消)
'' 傳回字傳: N 筆OK  , 1 : 執行中， -1 錯誤及內容
  Public Shared Function TableCopyFun(ByVal MidTab As DataTable, ByVal des As String, Optional ByVal mp As Integer = 0) As String
	Dim con As SqlClient.SqlConnection = New SqlClient.SqlConnection(EDIconstr)
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






End Class


Public Class EDIInf
		Public Inf_ As String = Nothing													 '介面ID
		Public STKEY_ As String = Nothing												 '貨主	
		Public FPath_ As String = Nothing													'檔案本機路目錄
		Public EDIDs As New DataSet														 '對表內容
		Public Const MidHostStr As String = "context connection=true"	'指定本機DBbase Or 一般指資料庫
		Public sqlstr As String = ""
'===========================================================================
' 中介數據文件資訊
'select * from EDIInf WHERE INF='SP01'
'文件格式範例 EDIInf
'Stkey,Infid,InfCls,Subfun,Batpath,DescrImp,Bulkin,SServerAcc,DServerAcc,Fmtname,Ftype,logfile,IsUpdate, >> 來源目地
'DoMark,Iseff, >> 控制
'EAIMod,EAIpath,Sourcefix,did,fixfile,fix2,back_one,back_two,back_thr,back_fou,  >> scan file
'impBkTab,
'Expmod,DescrExp,BuName     >> EXP file
'===========================================================================
		Public Sub New(ByVal iInf As String, ByVal iStkey As String)
			Inf_ = iInf				'設到共用變數
			'InfID = iInf			'設到屬性
			'FPath_ = iFpath
			STKEY_ = iStkey
	  sqlstr = "select * from EDIFMap WHERE INF='" & Inf_ & "' and Bxlen>0 order by cast(SN as int);select top 1 * from EDIInf WHERE INFID='" & Inf_ & "' and STKEY='" & STKEY_ & "' ;"	   ' and infCls='InfMap'
			'====取得:表 EDIInf,EDIFMap
			Try
					Dim Adapter As SqlDataAdapter = New SqlDataAdapter(sqlstr, MidHostStr)
					Adapter.Fill(EDIDs)
					EDIDs.Tables(0).TableName = "EDIFMap"
					EDIDs.Tables(1).TableName = "EDIInf"
					Adapter.Dispose()
			Catch ex As DataException
					'Console.Write("錯誤描述：" & ex.Message)
					FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("SAPInf 錯誤描述  ERR: {0}   ", ex.Message) + Chr(10), True)
					Exit Try
			End Try
			'EDIDs = CInfData(sqlstr, MidHostStr)
		End Sub
		''屬性建立 設定\取得
		Public Property InfID() As String
				Get
						Return Me.Inf_
				End Get
				Set(ByVal Value As String)
						Me.Inf_ = Value
				End Set
		End Property
End Class

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
'					FileIO.FileSystem.WriteAllText(DBConCls.APPLog, String.Format("SAPInf 錯誤描述  ERR: {0}   ", ex.Message) + Chr(10), True)
'					Exit Try
'			End Try

'		End Sub
'End Class