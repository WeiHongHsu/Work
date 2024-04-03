Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.IO
Imports System.Text
'Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.FileIO
Imports System.Xml


Public Class strHelp

    <Microsoft.SqlServer.Server.SqlProcedure()> _
  Public Shared Function QueryToHTML_CLR(ByVal QueryCommand As SqlString, ByVal IncludeHeaders As SqlInt32, ByVal Title As SqlString, ByVal Summary As SqlString, ByVal HTMLStyle As SqlInt32) As SqlString
    Dim str As String = "{0}"
    'Dim _sFilePath As String = FormattedPath(FilePath.ToString.Trim)
    'Dim _sFileName As String = FormattedFileName(FileName.ToString.Trim)
    'Dim _sFileNameWithPath As String = _sFilePath & _sFileName
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
      'NOW we can, in theory, do something with the DataTable.
      str = FormatDataTableAsHTML(MyDataTable, GetInteger(IncludeHeaders.Value), GetString(Title), GetString(Summary), GetInteger(HTMLStyle.Value))
      'SaveTextToFile(_sFileNameWithPath, str)
      Return CType(str, SqlString)

    Catch sqlex As SqlException
      Return CType("<ERRMSG>" + sqlex.Message, SqlString)
    Catch ex As Exception
      Return CType("<ERRMSG>" + ex.Message, SqlString)
    End Try
  End Function


  Friend Shared Function FormatDataTableAsHTML(ByRef dt As DataTable, ByVal IncludeHeaders As Integer, ByVal Title As String, ByVal Summary As String, ByVal HTMLStyle As Integer) As String
        Dim results As String = ""
        Dim i As Integer
        'preformatting.
        results = "<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN""> " & vbCrLf
        results = results & "<html> " & vbCrLf
        results = results & "<head> " & vbCrLf
        results = results & "<title> " & Title & " </title> " & vbCrLf
        results = results & "<meta HTTP-EQUIV=""Content-Type"" CONTENT=""text/html; charset=utf-8"">" & vbCrLf
        results = results & "<meta name=""Generator"" content=""CLR_ExportTableToHTML""> " & vbCrLf
        results = results & "<meta name=""Author"" content=""CLR_ExportTableToHTML""> " & vbCrLf
        results = results & "<meta name=""Keywords"" content=""CLR_ExportTableToHTML""> " & vbCrLf
        results = results & "<meta name=""Description"" content=""CLR_ExportTableToHTML""> " & vbCrLf
        results = results & GetHTMStyle(HTMLStyle) & " " & vbCrLf
        results = results & "</head> " & vbCrLf
        results = results & "<body> " & vbCrLf
        results = results & "<table summary=""" & Summary & """> " & vbCrLf
        results = results & "<caption>" & Summary & "</caption> " & vbCrLf

        If IncludeHeaders <> 0 Then
            'headers:
            results = results & "<tr> " & vbCrLf
            For Each col As DataColumn In dt.Columns
                results = results & "<th class=""HeaderRow""> " & col.ColumnName & "</th>" & vbCrLf
            Next
            results = results & "</tr> " & vbCrLf
        End If
        '--now spew the results
        i = 1
        If dt.Rows.Count = 0 Then
            results = results & "<tr><td colspan=""" & dt.Columns.Count & """>No Data Found.</td></tr> " & vbCrLf
        End If
        For Each dr As DataRow In dt.Rows
            results = results & "<tr> " & vbCrLf
            For Each col As DataColumn In dt.Columns
                If col.DataType.ToString = "System.Boolean" Then
                    If GetInteger(dr(col)) = 0 Then
                        results = results & "<td class=""" & IIf(i Mod 2 = 0, "EvenOverFlowRow", "OddOverFlowRow").ToString & """><input type=""checkbox"" DISABLED name=""" & col.ColumnName & i & """>&nbsp;</td>" & vbCrLf
                    Else
                        results = results & "<td class=""" & IIf(i Mod 2 = 0, "EvenOverFlowRow", "OddOverFlowRow").ToString & """><input type=""checkbox"" DISABLED CHECKED name=""" & col.ColumnName & i & """>&nbsp;</td>" & vbCrLf
                    End If
                Else
                    results = results & "<td class=""" & IIf(i Mod 2 = 0, "EvenRow", "OddRow").ToString & """> " & GetString(dr(col)).Replace(vbCrLf, "<br />") & vbCrLf & "&nbsp;</td>" & vbCrLf
                End If
            Next
            results = results & "</tr> " & vbCrLf
            i = i + 1
        Next
        results = results & " " & vbCrLf
        results = results & "</body> " & vbCrLf
        results = results & "</html> " & vbCrLf
        results = results & " " & vbCrLf
        Return results
    End Function
  ''=====================================================
      Friend Shared Function GetInteger(ByVal objValue As Object) As Integer
        Try
            If objValue Is Nothing OrElse Convert.IsDBNull(objValue) OrElse IsNumeric(objValue) = False Then
                Return 0
            Else
                Return Convert.ToInt32(objValue)
            End If
        Catch ex As Exception
            Return 0
        End Try
        'OJO
    End Function
    Friend Shared Function GetString(ByVal objValue As Object) As String
        If objValue Is Nothing OrElse Convert.IsDBNull(objValue) Then
            Return ""
        Else
            Return Convert.ToString(objValue)
        End If
    End Function

    '35 styles of various CSS definitions to change the color of html reports
    Friend Shared Function GetHTMStyle(ByVal WhichStyle As String) As String
        Select Case WhichStyle
            Case "Caramel"
                Return GetHTMStyle(1)
            Case "Money Twins"
                Return GetHTMStyle(2)
            Case "Lilian"
                Return GetHTMStyle(3)
            Case "The Asphalt World"
                Return GetHTMStyle(4)
            Case "iMaginary"
                Return GetHTMStyle(5)
            Case "Black"
                Return GetHTMStyle(6)
            Case "Blue"
                Return GetHTMStyle(7)
            Case "Coffee"
                Return GetHTMStyle(8)
            Case "Liquid Sky"
                Return GetHTMStyle(9)
            Case "London Liquid Sky"
                Return GetHTMStyle(10)
            Case "Glass Oceans"
                Return GetHTMStyle(11)
            Case "Stardust"
                Return GetHTMStyle(12)
            Case "Xmas 2008 Blue"
                Return GetHTMStyle(13)
            Case "Valentine"
                Return GetHTMStyle(14)
            Case "McSkin"
                Return GetHTMStyle(15)
            Case "Summer 2008"
                Return GetHTMStyle(16)
            Case "Pumpkin"
                Return GetHTMStyle(17)
            Case "Dark Side"
                Return GetHTMStyle(18)
            Case "Springtime"
                Return GetHTMStyle(19)
            Case "Darkroom"
                Return GetHTMStyle(20)
            Case "Foggy"
                Return GetHTMStyle(21)
            Case "High Contrast"
                Return GetHTMStyle(22)
            Case "Seven"
                Return GetHTMStyle(23)
            Case "Seven Classic"
                Return GetHTMStyle(24)
            Case "Sharp"
                Return GetHTMStyle(25)
            Case "Sharp Plus"
                Return GetHTMStyle(26)
            Case "DevExpress Style"
                Return GetHTMStyle(27)
            Case "Office 2007 Blue"
                Return GetHTMStyle(28)
            Case "Office 2007 Black"
                Return GetHTMStyle(29)
            Case "Office 2007 Silver"
                Return GetHTMStyle(30)
            Case "Office 2007 Green"
                Return GetHTMStyle(31)
            Case "Office 2007 Pink"
                Return GetHTMStyle(32)
            Case "Office 2010 Blue"
                Return GetHTMStyle(33)
            Case "Office 2010 Black"
                Return GetHTMStyle(34)
            Case "Office 2010 Silver"
                Return GetHTMStyle(35)
            Case Else 'default = "London Liquid Sky"
                Return GetHTMStyle(10)
        End Select
    End Function
End Class
