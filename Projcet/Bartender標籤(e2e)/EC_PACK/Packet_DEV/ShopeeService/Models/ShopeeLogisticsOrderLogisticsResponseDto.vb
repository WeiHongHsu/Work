Imports Newtonsoft.Json

Public Class ShopeeLogisticsOrderLogisticsResponseDto
    Inherits ShopeeReponseBaseDto

    <JsonProperty("logistics")>
    Public Property Logistics As ShopeeLogisticsOrderLogisticsResponseLogisticDto

End Class