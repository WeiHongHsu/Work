Module ShopeeService

    ' 取蝦皮全家物, 代入參數(蝦皮訂單編號)
    Function GetShopeeFamilyLogistic(shopeeOrderId As String) As ShopeeLogisticsOrderLogisticsResponseDto

        Dim request As ShopeeOrderSnDto = New ShopeeOrderSnDto() With {
                .Sn = shopeeOrderId,
                .TimeStamp = DateTimeExtension.GetTimeStampNow()
            }

        Dim response As ShopeeLogisticsOrderLogisticsResponseDto = ShopeeLogisticsApi.GetOrderLogistics(request)

        If Not String.IsNullOrEmpty(response.Error) Then
            Throw New Exception(response.Message)
        End If

        Return response

    End Function

End Module