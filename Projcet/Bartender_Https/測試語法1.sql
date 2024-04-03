USE [AAD]
GO

DECLARE	@return_value int,
		@status int,
		@ResponseText nvarchar(4000)

EXEC	@return_value = [dbo].[_sp_BartenderAPI]
		@INOUT = N'POST',
		@ApiURL = N'https://bartender.eslite.com/Integration/Json_STD/Execute',
		@headers = 'application/text' ,
		@Body = N'[{"display_order_number":"202311234893201","waybill_number":"03716880097","cod_flag":"請收款結帳","ship_to_phone":"168","ship_to_name":"誠品顧客","ship_to_description":"龜山德明店","route_seq":"GZ04","region":"北","channel_code":"1","barcode_1":"281037762","barcode_2":"1688009710124666","barcode_qr":"B1||                  ||         ||                  ||               ||12810003716880097I||2||017529||1GZ04|| 4||0||281037762||1688009710124666||             ||          ","Black_Flag":"Y","equipment_id":"4","printer_id":"Ec_192.168.19.2","FilePath_bak":"D:\\Bartender\\LogFiles\\REY_ShippingLabel_202311234893201_20231123211557.Json","BTW_Label":"lib:\/\/REY_ShippingLabel.btw"}]',
		@userid = N'LPWPACK0003',
		@workstationid = N'LPWPACK0003',
		@LogFileName = N'D:\\Bartender\\LogFiles\\REY_ShippingLabel_202311234893201_20231123210834.Json',
		@page = 1,
		@status = @status OUTPUT,
		@ResponseText = @ResponseText OUTPUT

SELECT	@status as N'@status',
		@ResponseText as N'@ResponseText'

SELECT	'Return Value' = @return_value

GO


