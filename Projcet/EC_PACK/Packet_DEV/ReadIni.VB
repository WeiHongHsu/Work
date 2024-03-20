Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices


Namespace vs.com.Readini

	Public Class INIReader

		'''Description: retrieve section names' list in config file
		Public Shared Function getSectionNames(sFilename As String) As String()
			'StringBuilder sbSectionNames = new StringBuilder(255);            //[NEW]
			Dim baSectionNames As Byte() = New Byte(512) {}
			Dim NumRet As Integer = GetPrivateProfileSectionNames(baSectionNames, 512, sFilename)
			Return System.Text.Encoding.[Default].GetString(baSectionNames, 0, NumRet).Split(ControlChars.NullChar)
		End Function
		'getSectionNames()
		'''Description: retrieve value with specified {sFilename, sSectionName, sKeyName}
		Public Shared Function getValue(sFilename As String, sSectionName As String, sKeyName As String) As String
			Dim sbKeyValue As New StringBuilder(255)
			'[NEW]
			Dim NumRet As Integer = GetPrivateProfileString(sSectionName, sKeyName, "", sbKeyValue, 255, sFilename)
			Return (sbKeyValue.ToString().Trim())
        End Function
        Public Shared Function setValue(ByVal sFilename As String, ByVal sSectionName As String, ByVal sKeyName As String, ByVal strVal As String) As Long
            Dim sbKeyValue As New StringBuilder(255)
            '[NEW]
            Dim NumRet As Long = WritePrivateProfileString(sSectionName, sKeyName, strVal, sFilename)
            Return NumRet
        End Function
		'getValue()
		'''/////////////////
		'''API declaration
		<DllImport("kernel32", EntryPoint := "GetPrivateProfileSectionNamesA")> _
		Private Shared Function GetPrivateProfileSectionNames(<MarshalAs(UnmanagedType.LPArray)> retVal As Byte(), size As Integer, filePath As String) As Integer
		End Function
		<DllImport("kernel32")> _
		Private Shared Function GetPrivateProfileString(section As String, key As String, def As String, retVal As StringBuilder, size As Integer, filePath As String) As Integer
		End Function
		<DllImport("kernel32")> _
		Private Shared Function WritePrivateProfileString(section As String, key As String, val As String, filePath As String) As Long
		End Function

	End Class
	'class-INIReader
End Namespace
