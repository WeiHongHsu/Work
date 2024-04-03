Imports Newtonsoft.Json

Public Class ShopeeBaseDto

    <JsonProperty("partner_id")>
    Public ReadOnly Property PartnerId As Long
        Get
            Return 844801
        End Get
    End Property

    <JsonProperty("shopid")>
    Public ReadOnly Property ShopId As Long
        Get
            Return 211208801
        End Get
    End Property

    <JsonProperty("timestamp")>
    Public Property TimeStamp As Long

End Class