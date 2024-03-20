USE [Iexp]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetECinfor_711]    Script Date: 8/17/2023 2:40:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











/***********************************************************
 Create Date	: 2023.07.04
 Description	: e2e-->
 Test SQL		: 
	Declare @outputJson as NVARCHAR(MAX)
	Declare @apiStatus as INT
	Declare @apiMessage as NVARCHAR(500)
	Exec SP_GetECinfor_711 N'
[{
"cod_flag": "*不用檢視證件*",
"ship_to_name": "***",
"waybill_number": "78510764706",
"Shop_name": "富友門市194549",
"Tracking_No": "19454978500110764706",
"order_date": "2023/01/11",
"actual_delivery_date": "2023/01/18",
"display_order_number": "202308093985701",
"printer_id": "Ec_192.168.19.2",
"FilePath_bak": "D:\\Bartender\\LogFiles\\711_ShippingLabel_202301098792101_20230118172333.Json",
"BTW_Label": "D:\\Bartender\\Format\\711_ShippingLabel_V1.0.btw"
}]
	',@outputJson output,@apiStatus output,@apiMessage output
	print 'THIS IS outputJson :' + @outputJson
	print @apiStatus
	print @apiMessage 
 Description	:
	取得EC黑貓面單資料

************************************************************/

ALTER PROCEDURE [dbo].[SP_GetECinfor_711]
    @jsonParam VARCHAR(8000),
    @outputJson VARCHAR(8000) OUTPUT,
    @apiStatus INT OUTPUT,
    @apiMessage NVARCHAR(500) OUTPUT
AS
BEGIN TRY
BEGIN
	
    -- 建立變數
    DECLARE @url NVARCHAR(500)				-- 外部 API 的 URL
	DECLARE @address_version NVARCHAR(500)	-- 黑貓郵遞區號版本
	DECLARE @authHeader NVARCHAR(500)
	DECLARE @response NVARCHAR(4000)
	
	-- 設定 API 網址
	SET @url = 'http://invisible-man-uat.eslite.com/v1/bartender/labels/convert'
	SET @url = 'http://invisible-man-dev.eslite.com/v1/bartender/labels/convert'
	SET @url = 'http://invisible-man-dev.eslite.com/v1/bartender/waybill/revert'

	--SET @url = 'http://invisible-man-uat.eslite.com/v1/bartender/waybill/revert'
	PRINT @url
	-- 設定 送出參數
	SET @jsonParam = '{"label_type": "711","data": '+@jsonParam+'}'
	PRINT @jsonParam
	-- 設定授權標頭
	SET @authHeader = 'Basic bG9naXN0aWNzX25pbmphOjc4dTFQNzJGMWhrQQ=='
	
	-- 建立 COM 物件
	DECLARE @http INT
	EXEC sp_OACreate 'MSXML2.ServerXMLHTTP', @http OUTPUT
	
	-- 設定 HTTP 請求
	DECLARE @Content AS NVARCHAR(100) = N'application/json'
	DECLARE @RequestMethod AS NVARCHAR(100) = N'POST'
	
	EXEC sp_OAMethod @http, 'Open', NULL, @RequestMethod, @url, 'false'
	EXEC sp_OAMethod @http, 'setRequestHeader', NULL, 'Authorization', @authHeader
	EXEC sp_OAMethod @http, 'setRequestHeader', NULL, 'Content-Type',  @Content	-- 添加 Content-Type 標頭
	EXEC sp_OAMethod @http, 'send', NULL, @jsonParam;
	
	DECLARE @RequestTime AS DATETIME = GETDATE()
	-- 傳回的狀態碼
	EXEC sp_OAGetProperty @http, 'status', @apiStatus OUTPUT
	

	-- 檢查回應狀態碼
	IF @apiStatus = 200
	BEGIN
	    -- 取得回應內容
	    EXEC sp_OAGetProperty @http, 'responseText', @response OUTPUT
		SET @apiMessage = 'API 回覆成功'
		--PRINT @response

		SET @outputJson = REPLACE(@response, '{"data":', '')
		SET @outputJson = REPLACE(@outputJson, '}]}', '}]')
		--PRINT @outputJson
		---- 在另一個 SELECT 陳述式中使用該變數
		--SELECT @outputPostalCode AS [OutputPostalCode]
		
	    -- 解析 JSON 回應內容
	    
	END
	ELSE
	BEGIN
		EXEC sp_OAGetProperty @http, 'responseText', @response OUTPUT
		SET @apiMessage = @response + ' API 回覆失敗'
		PRINT @response
		SELECT
		JSON_VALUE(@response, '$.status') AS [Status],
		JSON_VALUE(@response, '$.type') AS [Type],
		JSON_VALUE(@response, '$.title') AS [Title],
		JSON_VALUE(@response, '$.detail') AS [Detail];
	END

	--print '@apiMessage: ' + @apiMessage
	print '@response: ' + @response
	print '@status: '+cast(@apiStatus as nvarchar(100))
	DECLARE @ResponseTime AS DATETIME = GETDATE()

	-- 釋放 COM 物件
	EXEC sp_OADestroy @http
	

    -- 返回結果


    DECLARE @IPAddress VARCHAR(50) = CAST(CONNECTIONPROPERTY('client_net_address') AS VARCHAR(50))
	DECLARE @LoginName VARCHAR(50) = ORIGINAL_LOGIN()
	
	EXEC SP_EC_API_Log
	@RequestTime,
	@apiStatus,
	@ResponseTime,
	@Content,
	@url,
	@UserID = @LoginName,
	@SourceIP = @IPAddress,
	@RequestMethod = @RequestMethod,
	@RequestHeaders = @authHeader,
	@RequestParameters = @jsonParam,
	@ExceptionMessage = @response,
	@ServerInfo = 'SP_GetECinfor_711',
	@ResponseHeaders = @Content,
	@ExceptionStackTrace = ''

	print 'try catch in'
END

END TRY
BEGIN CATCH
	print 'try catch out'
    -- 捕捉異常並顯示錯誤訊息
    PRINT 'API呼叫失敗: ' + ERROR_MESSAGE()
END CATCH
