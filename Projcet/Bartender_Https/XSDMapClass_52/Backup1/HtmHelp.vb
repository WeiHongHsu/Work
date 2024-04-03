Imports System
Imports System.Linq
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.Net.Security
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Services.WebService


 Public Class HtmHelp
 'SQL QUERY to HTML table
 ' Encrypt 加密
	 <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
	 Public Shared Function EDI_Encrypt(ByVal pToEncrypt As String, ByVal sKey As String) As String
		 Dim des As New DESCryptoServiceProvider()
		 Dim inputByteArray() As Byte
		 inputByteArray = Encoding.Default.GetBytes(pToEncrypt)
		 '建立加密對象的密鑰和偏移量
		 '原文使用ASCIIEncoding.ASCII方法的GetBytes方法
		 '使得輸入密碼必須輸入英文文本
		 des.Key = ASCIIEncoding.ASCII.GetBytes(sKey)
		 des.IV = ASCIIEncoding.ASCII.GetBytes(sKey)
		 '寫二進制數組到加密流
		 '(把內存流中的內容全部寫入)
		 Dim ms As New System.IO.MemoryStream()
		 Dim cs As New CryptoStream(ms, des.CreateEncryptor, CryptoStreamMode.Write)
		 '寫二進制數組到加密流
		 '(把內存流中的內容全部寫入)
		 cs.Write(inputByteArray, 0, inputByteArray.Length)
		 cs.FlushFinalBlock()

		 '建立輸出字符串     
		 Dim ret As New StringBuilder()
		 Dim b As Byte
		 For Each b In ms.ToArray()
			 ret.AppendFormat("{0:X2}", b)
		 Next

		 Return ret.ToString()
	End Function

		'解密方法
	 <Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
	Public Shared Function EDI_Decrypt(ByVal pToDecrypt As String, ByVal sKey As String) As String
		 Dim des As New DESCryptoServiceProvider()
		 '把字符串放入byte數組
		 Dim len As Integer
		 len = pToDecrypt.Length / 2 - 1
		 Dim inputByteArray(len) As Byte
		 Dim x, i As Integer
		 For x = 0 To len
			 i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16)
			 inputByteArray(x) = CType(i, Byte)
		 Next
		 '建立加密對象的密鑰和偏移量，此值重要，不能修改
		 des.Key = ASCIIEncoding.ASCII.GetBytes(sKey)
		 des.IV = ASCIIEncoding.ASCII.GetBytes(sKey)
		 Dim ms As New System.IO.MemoryStream()
		 Dim cs As New CryptoStream(ms, des.CreateDecryptor, CryptoStreamMode.Write)
		 cs.Write(inputByteArray, 0, inputByteArray.Length)
		 cs.FlushFinalBlock()
		 Return Encoding.Default.GetString(ms.ToArray)

	End Function
'=================HTML===========
	<Microsoft.SqlServer.Server.SqlFunction(DataAccess:=Microsoft.SqlServer.Server.DataAccessKind.Read)> _
	Public Shared Function CLR_Qhtm(ByVal QueryCommand As SqlString, ByVal Summary As SqlString) As SqlString
		Dim str As String = "{0}"
		'by using the context connection, we have access to whatever CTEs, tables, temp tables,table variables and even linked servers that the caller has access to!
		Dim MyConnection As New SqlConnection("context connection=true")
		Try
			str = String.Format(str, QueryCommand)
			MyConnection.Open()

			Dim myCommand As New SqlCommand(str, MyConnection)
			Dim myDataReader As SqlDataReader
			myDataReader = myCommand.ExecuteReader()
			Dim MyDataTable As New DataTable("Results")
			MyDataTable.Load(myDataReader)
			str = FmtTableQhtm(MyDataTable, Summary.Value)
			Return str
		Catch sqlex As SqlException
			'Throw New Exception("FileName: " & _sFileNameWithPath, sqlex)
			Return sqlex.ErrorCode & sqlex.Message
		Catch ex As Exception
			'Throw New Exception("FileName: " & _sFileNameWithPath, ex)
			Return ex.Message
		End Try

	End Function

	Friend Shared Function FmtTableQhtm(ByRef dt As DataTable, ByRef Summary As String) As String
		Dim results As String = ""
		Dim i As Integer
		results = results & "<table style='border:3px #FFAC55 solid;padding:5px;' rules='all' cellpadding='5' summary=' & Summary & " '>"
		results = results & "<caption>" & Summary & "</caption>"
		'headers:
		results = results & "<tr>"
		For Each col As DataColumn In dt.Columns
			results = results & "<th>" & col.ColumnName & "</th>"
		Next
		results = results & "</tr>"
		'--now spew the results
		i = 1
		If dt.Rows.Count = 0 Then
			results = results & "<tr><td colspan=""" & dt.Columns.Count & """>No Data Found.</td></tr> "
		End If
		For Each dr As DataRow In dt.Rows
			results = results & "<tr>"
			For Each col As DataColumn In dt.Columns
					results = results & "<td>" & dr(col).ToString.Replace(vbCrLf, "") & "</td>"
			Next
			results = results & "</tr>"
			i = i + 1
		Next
		results = Left(results & "</table> ", 7990)
		Return results
	End Function
	'LINQ 
	'Friend Shared Function FmtTableQhtm(ByVal data As DataTable, ByRef Summary As String) As String
	'	Dim table As String() = New String(data.Rows.Count) {}
	'	Dim counter As Long = 1
	'	Dim results As String = "<table style='border:3px #FFAC55 solid;padding:5px;' rules='all' cellpadding='5' summary=' & Summary & " '>"
	'	results = results & "<caption>" & Summary & "</caption>" & vbCrLf
	'	'headers:
	'	'results = results & "<tr>"
	'	'For Each col As DataColumn In data.Columns
	'	'	results = results & "<th>" & col.ColumnName & "</th>"
	'	'Next
	'	'results = results & "</tr>"
	'	Dim head = From o In data.Columns Select o.ColumnName
	'	table(0) = "<tr><th>" & String.Join("</th><th>", head.Select(Function(c) c.ToString.TrimEnd(" ")).ToArray) & "</th></tr>"

	'	For Each row As DataRow In data.Rows
	'		table(counter) = "<tr><td>" & String.Join("</td><td>", row.ItemArray.Select(Function(c) c.ToString.TrimEnd(" ")).ToArray) & "</td></tr>"
	'		counter += 1
	'	Next
	'	results += String.Join("", table) & "</table>"
	'	Dim strBytes() As Byte
	'	strBytes = Encoding.Default.GetBytes(results)
	'	results = Encoding.GetEncoding(950).GetString(strBytes)
	'	Return Left(results, 7990)
	'End Function


End Class
