DECLARE
@INOUT nvarchar(50) = 'POST',
@ApiURL nvarchar(2000) = 'https://bartender.eslite.com/Integration/Json_STD/Execute', 
--@ApiURL nvarchar(2000) = 'https://bartender.eslite.com/Integration/CSV_STD/Execute', 
--@ApiURL nvarchar(2000) = 'https://bartender.eslite.com/Integration/CSV_Table/Execute', 
--@ApiURL nvarchar(2000) = 'https://bartender.eslite.com/Integration/Json_PDF/Execute', 
--@ApiURL nvarchar(200) = 'http://192.168.1.104/Integration/Json_STD/Execute',
@Headers nvarchar(100) = 'application/json', 
@Body nvarchar(max) = N'
[
{
"display_order_number": "202112208694501",
"waybill_number": "02702100032",
"cod_flag": "請收款結帳",
"ship_to_phone": "189",
"ship_to_name": "測試人",
"ship_to_description": "全家岡山岡燕店",
"route_seq": "TM04",
"region": "南",
"channel_code": "1",
"barcode_1": "281027762",
"barcode_2": "0210003210262513",
"barcode_qr": "B1||                  ||         ||                  ||               ||12810002702100032T||2||019693||1TM04|| 6||0||281027762||0210003210262513||             ||          ",
"printer_id": "A16_192.168.19.1",
"FilePath_bak": "D:\\Bartender\\Log Files\\REY_ShippingLabel_202112208694501_20230202142208.Json",
"BTW_Label": "lib://REY_ShippingLabel.btw",
"Black_Flag": "Y"
}
]
',
@userid nvarchar(50) ='jackyhsu', 
@workstationid nvarchar(50) =  'LNWOC210009',
@LogFileName nvarchar(100) = 'TEST.Json',
@page int = 1, --列印張數
@status int , 
@ResponseText NVARCHAR(4000) 
DECLARE @hr  int

DECLARE @Object INT

EXEC @hr = sp_OACreate 'MSXML2.ServerXMLHTTP.6.0', @Object OUT;
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object 
select '1',getdate()
EXEC @hr = sp_OAMethod @Object, 'open', NULL, @INOUT,@ApiURL, false
select '2',getdate()
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object
EXEC @hr = sp_OAMethod @Object, 'setRequestHeader', null, 'Content-Type', @Headers
---- 设置代理
--EXEC sp_OAMethod @Object, 'setProperty', NULL, 'onreadystatechange', 'SetProxy'
---- 设置代理服务器地址和端口
--EXEC sp_OAMethod @Object, 'setProperty', NULL, 'ProxyServer', 'bartender.eslite.com'
--EXEC sp_OAMethod @Object, 'setProperty', NULL, 'ProxyPort', 443
select '3',getdate()
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object
EXEC @hr = sp_OAMethod @Object, 'send', null, @Body
select '4',getdate()
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object
EXEC @hr = sp_OAGetProperty @Object, 'status', @status OUT
select '5',getdate()
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object
EXEC @hr = sp_OAMethod @Object, 'ResponseText', @ResponseText OUTput
select '6',getdate()
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object
--刪除已建立的 OLE 物件
EXEC @hr = sp_OADestroy @Object
IF @hr <> 0 EXEC sp_OAGetErrorInfo @Object
select @Body,@status,@ResponseText

