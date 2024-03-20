Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json

Module ShopeeLogisticsApi
    Dim pokey As String
    Public Function GetOrderLogistics(request As ShopeeOrderSnDto) As ShopeeLogisticsOrderLogisticsResponseDto
        pokey = request.Sn
        Dim response As String = CallApi("logistics/order/get", request)
        Return JsonConvert.DeserializeObject(Of ShopeeLogisticsOrderLogisticsResponseDto)(response)

    End Function

    Sub insertAPI(ByVal Order As String, ByVal postData As String, ByVal response As String)
        Try
            Dim CONSTR As String = "Data Source=192.168.1.11;Initial Catalog=RF;User ID=sa;Password=zaq1"
            Dim SQLSTR As String = "INSERT INTO Shopee_Records" + _
                                    "(POKEY,postData,response,postDate)" + _
                                    "VALUES" + _
                                    "('" + pokey + "','" + postData + "','" + response + "','" + Now.ToString + "')"
            Dim DT As DataTable = New DataTable
            Dim CON As SqlClient.SqlConnection = New SqlClient.SqlConnection(CONSTR)
            Dim CMD As SqlClient.SqlCommand = New SqlClient.SqlCommand(SQLSTR, CON)
            CON.Open()
            CMD.ExecuteNonQuery()
            CON.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub
    '20220519 Raines TODO get API
    Private Function CallApi(apiUrlAttr As String, request As Object) As String

        Dim apiBaseUrl = "https://partner.shopeemobile.com/api/v1/"

        Dim apiUrl As String = apiBaseUrl & apiUrlAttr
        Dim postData As String = JsonConvert.SerializeObject(request)
        Dim auth As String = GetAuthorization(apiUrl, postData)
        Dim response As String = String.Empty

        Using wc As WebClient = New WebClient()
            wc.Encoding = Encoding.UTF8
            wc.Headers.Add("Content-Type", "application/json")
            wc.Headers.Add("Authorization", auth)
            response = wc.UploadString(apiUrl, "post", postData)
        End Using
        insertAPI(pokey, postData, response)
        Return response


    End Function

    Private Function GetAuthorization(apiUrl As String, postData As String) As String

        Dim privateKey = "eacf4273cd6c1eb8cffb76da64478e1f9811b7d8a60698d6ac748d946837b87a"

        Dim authContent As String = apiUrl & "|" & postData
        Return HMACSHA256Encode(authContent, privateKey)

    End Function

    Private Function HMACSHA256Encode(ByVal source As String, ByVal key As String) As String

        Dim encoding As UTF8Encoding = New UTF8Encoding()
        Dim keybytes As Byte() = encoding.GetBytes(key)
        Dim sourcebytes As Byte() = encoding.GetBytes(source)
        Dim result As String

        Using hmac As HMACSHA256 = New HMACSHA256(keybytes)
            Dim resultBytes As Byte() = hmac.ComputeHash(sourcebytes)
            result = BitConverter.ToString(resultBytes).Replace("-", String.Empty)
        End Using

        Return result

    End Function

End Module