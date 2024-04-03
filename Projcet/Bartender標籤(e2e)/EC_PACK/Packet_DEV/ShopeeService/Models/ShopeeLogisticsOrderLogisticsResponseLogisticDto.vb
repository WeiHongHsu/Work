Imports Newtonsoft.Json

Public Class ShopeeLogisticsOrderLogisticsResponseLogisticDto

    <JsonProperty("cod")>
    Public Property Cod As Boolean

    <JsonProperty("first_mile_name")>
    Public Property FirstMileName As String

    <JsonProperty("goods_to_declare")>
    Public Property GoodsToDeclare As Boolean

    <JsonProperty("lane_code")>
    Public Property LaneCode As String

    <JsonProperty("last_mile_name")>
    Public Property LastMileName As String

    <JsonProperty("logistic_id")>
    Public Property LogisticId As Long

    <JsonProperty("recipient_address")>
    Public Property RecipientAddress As ShopeeLogisticsOrderLogisticsResponseLogisticRecipientDto

    <JsonProperty("recipient_sort_code")>
    Public Property RecipientSortCode As ShopeeLogisticsOrderLogisticsResponseLogisticRecipientSortCodeDto

    <JsonProperty("sender_sort_code")>
    Public Property SenderSortCode As ShopeeLogisticsOrderLogisticsResponseLogisticSenderSortCodeDto

    <JsonProperty("service_code")>
    Public Property ServiceCode As String

    <JsonProperty("shipping_carrier")>
    Public Property ShippingCarrier As String

    <JsonProperty("third_party_logistic_info")>
    Public Property ThirdPartyLogisticInfo As ShopeeLogisticsOrderLogisticsResponseLogisticThirdPartyLogisticInfoDto

    <JsonProperty("tracking_no")>
    Public Property TrackingNo As String

    <JsonProperty("warehouse_address")>
    Public Property WarehouseAddress As String

    <JsonProperty("warehouse_id")>
    Public Property WarehouseId As Long

    <JsonProperty("zone")>
    Public Property Zone As String

End Class