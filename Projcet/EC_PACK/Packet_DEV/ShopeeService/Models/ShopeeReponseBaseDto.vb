Imports Newtonsoft.Json

Public Class ShopeeReponseBaseDto

    <JsonProperty("error")>
    Public Property [Error] As String

    <JsonProperty("msg")>
    Public Property Message As String

    <JsonProperty("request_id")>
    Public Property RequestId As String

End Class