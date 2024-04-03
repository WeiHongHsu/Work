USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_sp_get_waybill_number]    Script Date: 8/29/2023 11:55:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
        
        
          
Create PROCEDURE [dbo].[_sp_get_waybill_number]          
 @waybill_type      NVARCHAR(10)          
 , @waybill_key      NVARCHAR(4000) = NULL          
 , @waybill_number     NVARCHAR(30) OUTPUT          
 , @waybill_data      NVARCHAR(4000) OUTPUT          
 ----          
 , @rtn_code       NVARCHAR(10) = NULL OUTPUT          
 , @rtn_message      NVARCHAR(500) = NULL OUTPUT          
 , @ww_result      NVARCHAR(10) = NULL OUTPUT          
AS          
          
/***          
================================================================================          
[Name]          
 _sp_get_waybill_number          
          
[Description]          
 Get the waybill number of specified waybill type from numbering system of API.          
          
[Parameters]          
 @waybill_type    Type of the waybill.          
 @waybill_key    Input keys and/or parameters for getting waybill number.          
 @waybill_number    The waybill number returned.          
 @waybill_data    Output data of further waybill information and/or API.          
 @rtn_code     Return code. ('PASS', 'FAIL')          
 @rtn_message    Return message.          
 @ww_result     Return code for WebWise. (= @rtn_code)          
          
Pointek Co.          
--------------------------------------------------------------------------------          
2020/10/10 - [Steve] First release.          
================================================================================          
 Author   : kye        
 Alter Label : 20221215_kye        
 Alter Date  : 20221215        
 Description : modify Shopee logic for Shopee API 2.0         
 test script :         
===============================================================================        
================================================================================          
 Author   : JackyHsu        
 Alter Label : 20230724   (JH01)     
 Alter Date  : 20230724        
 Description : 新增郵局取號機制        
 test script :         
===============================================================================        
================================================================================          
Author   : JackyHsu        
Alter Label : 20230801        
Alter Date  : 20230801        
Description : 更改黑貓API取站別號碼機制(JH02)        
test script :         
===============================================================================        

 
***/          
          
BEGIN          
 SET NOCOUNT ON;          
          
 DECLARE          
  ---- Error handling variables          
  @sp_name      NVARCHAR(30)  = N'_sp_get_waybill_number'          
  , @error_number     INT          
  , @retry_count     INT = 3          
  , @debug      BIT = 1          
  , @production     BIT = 1          
  ---- For API          
  , @api_retry_count    INT = 2          
  , @api_uri      NVARCHAR(4000)          
  , @api_header     NVARCHAR(4000)          
  , @api_data      NVARCHAR(4000)          
  , @api_id      NVARCHAR(30)          
  ---- For synamic SQL          
  , @sql       NVARCHAR(4000)          
  , @parm_01      NVARCHAR(500)          
  , @parm_02      NVARCHAR(500)          
  , @parm_03      NVARCHAR(500)          
  , @parm_04      NVARCHAR(500)          
  , @parm_05      NVARCHAR(500)          
  , @parm_06      NVARCHAR(500)          
  , @parm_07      NVARCHAR(500)          
  ----          
          
 BEGIN TRY          
  SET @rtn_code = N'PASS';          
  SET @rtn_message = NULL;          
          
-- >>>>> Customization codes.          
          
  SET @waybill_type = UPPER(@waybill_type);          
  SET @waybill_number = NULL;          
  SET @waybill_data = NULL;          
          
  IF @debug = 1          
   PRINT N'[' + @sp_name + N']:' + NCHAR(10)          
    + N'@waybill_type: ' + ISNULL(@waybill_type, N'(null)') + NCHAR(10)          
    + N'@waybill_key: ' + ISNULL(@waybill_key, N'(null)') + NCHAR(10)          
          
  WHILE @retry_count > 0          
  BEGIN          
   SET @rtn_code = N'PASS';          
   SET @rtn_message = NULL;          
          
   BEGIN TRY          
          
    ---- Start transaction          
    /*          
    BEGIN TRANSACTION          
    */          
          
    ---- Get waybill information according to @waybill_type          
          
    ---- FedEx          
    IF @waybill_type = N'WBLFDX'          
	BEGIN          
		EXEC _sp_get_next_number          
		@number_type    = @waybill_type          
		, @next_number    = @waybill_number OUTPUT          
		----          
		, @rtn_code     = @rtn_code OUTPUT          
		, @rtn_message    = @rtn_message OUTPUT          
	END          
    ---- 7-11          
    ELSE IF @waybill_type = N'WBL711'          
	BEGIN          
		EXEC _sp_get_next_number          
		@number_type    = @waybill_type          
		, @next_number    = @waybill_number OUTPUT          
		----          
		, @rtn_code     = @rtn_code OUTPUT          
		, @rtn_message    = @rtn_message OUTPUT          
	END          
    ---- CVS          
    ELSE IF @waybill_type = N'WBLCVS'          
    BEGIN            
     EXEC _sp_get_next_number          
      @number_type    = @waybill_type          
      , @next_number    = @waybill_number OUTPUT          
      ----          
      , @rtn_code     = @rtn_code OUTPUT          
      , @rtn_message    = @rtn_message OUTPUT          
          
     ---- Build JSON for waybill_data          
     SELECT TOP 1          
      @waybill_data =           
      (          
       SELECT           
        JSON_VALUE(@waybill_key, '$[0].display_order_number') AS display_order_number          
        , om.ship_to_code AS ship_to_code           
        , CONVERT(NVARCHAR(30), om.cod_amount) AS cod_amount          
        , cs.route_seq AS route_seq           
        , cs.region AS region           
        , cs.equipment_id AS equipment_id           
       FOR JSON PATH          
      )          
     FROM          
      _tv_order_master AS om WITH (NOLOCK)          
      LEFT JOIN _v_customer_site AS cs WITH (NOLOCK)          
       ON cs.customer_code = om.customer_code          
       AND cs.site_code = om.ship_to_code          
     WHERE          
      client_code = JSON_VALUE(@waybill_key, '$[0].client_code')          
      AND display_order_number = JSON_VALUE(@waybill_key, '$[0].display_order_number')                
    END          
    ---- 誠品香港CK          
    ELSE IF @waybill_type = N'WBLCK'          
    BEGIN          
     EXEC _sp_get_next_number          
      @number_type    = @waybill_type          
      , @next_number    = @waybill_number OUTPUT          
      --          
      , @rtn_code     = @rtn_code OUTPUT          
      , @rtn_message    = @rtn_message OUTPUT          
    END          
    ---- 誠品門市          
    ELSE IF @waybill_type = N'WBLESL'          
    BEGIN          
     EXEC _sp_get_next_number          
      @number_type    = @waybill_type          
      , @next_number    = @waybill_number OUTPUT          
      --          
      , @rtn_code     = @rtn_code OUTPUT          
      , @rtn_message    = @rtn_message OUTPUT          
    END          
    ---- T-Cat 黑貓          
    ELSE IF @waybill_type = N'WBLTCAT'          
    BEGIN          
     ---- PROD 
	 /*黑貓取站別號碼*/
	 /*20230801 by JackyHsu (JH02)*/
	--IF @production = 1 
	--SET @sql = 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''''http://192.168.1.52:8800/egs?cmd=query_suda7_dashv2&address_1=' + JSON_VALUE(@waybill_key, '$[0].ship_to_address') + ''''', ''''suda7_1'''') AS zip_7'          
    
	------ TEST          
	--ELSE 
	--SET @sql = 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''''http://192.168.1.121:8800/egs?cmd=query_suda7_dashv2&address_1=' + JSON_VALUE(@waybill_key, '$[0].ship_to_address') + ''''', ''''suda7_1'''') AS zip_7'          
          
	--SET @sql = 'SELECT @zip_7 = zip_7 FROM OPENQUERY([INT],''' + @sql + ''')'          

	--EXEC sp_executesql           
	--@sql          
	--, N'@zip_7 NVARCHAR(20) OUTPUT'          
	--, @zip_7 = @parm_01 OUTPUT    
	  

	Declare @outputPostalCode as NVARCHAR(200)
	Declare @apiStatus as INT
	Declare @apiMessage as NVARCHAR(500)
	Declare @Order nvarchar(100)
	set @order = JSON_VALUE(@waybill_key, '$[0].display_order_number')

	Exec [INT].iexp.dbo.SP_GetPostalCode_TCAT @order,@outputPostalCode output,@apiStatus output,@apiMessage output
	--Exec [192.168.1.52].iexp.dbo.SP_GetPostalCode_TCAT @order,@outputPostalCode output,@apiStatus output,@apiMessage output


	IF ISNULL(@apiStatus,'') <> '200'         
		BEGIN          
			SET @rtn_code = N'FAIL';          
			SET @rtn_message = 'TCAT 郵遞區號API異常 訊息如下： ' + @apiMessage;  
			set @outputPostalCode = @apiMessage
		END          


	/*黑貓郵遞區號*/
	set @parm_01 = @outputPostalCode
   
     ---- PROD          
	 /*黑貓取EGS版本號*/
     IF @production = 1          
      SET @sql = 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''''http://192.168.1.52:8800/egs?cmd=query_egs_info'''', ''''address_db_version'''') AS db_version'          
     ---- TEST          
     ELSE          
      SET @sql = 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''''http://192.168.1.121:8800/egs?cmd=query_egs_info'''', ''''address_db_version'''') AS db_version'          
               
     SET @sql = 'SELECT @db_version = db_version FROM OPENQUERY([INT],''' + @sql + ''')'          
     EXEC sp_executesql          
      @sql          
      , N'@db_version NVARCHAR(20) OUTPUT'          
      , @db_version = @parm_02 OUTPUT          
          
     ---- Build JSON for waybill_data          
     SET @waybill_data =           
     (          
      SELECT           
       @parm_01 AS zip_7          
       , @parm_02 AS db_version           
      FOR JSON PATH          
     )          
          
     ---- COD type = A, 不代收          
     IF JSON_VALUE(@waybill_key, '$[0].type') = N'A'          
     BEGIN          
      ---- PROD
	  /*黑貓取號*/
      IF @production = 1          
       SELECT           
        @waybill_number = waybill_number          
       FROM           
        OPENQUERY(INT, 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''http://192.168.1.52:8800/egs?cmd=query_waybill_id_range&customer_id=2795296601&waybill_type=A&count=1'' , ''waybill_id'') AS waybill_number')          
      ---- TEST          
      ELSE          
       SELECT           
        @waybill_number = waybill_number          
       FROM           
        OPENQUERY(INT, 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''http://192.168.1.121:8800/egs?cmd=query_waybill_id_range&customer_id=7076259101&waybill_type=A&count=1'' , ''waybill_id'') AS waybill_number')          
     END          
     ---- COD type = B, 需代收          
     ELSE IF JSON_VALUE(@waybill_key, '$[0].type') = N'B'          
     BEGIN          
      ---- PROD          
      IF @production = 1          
       SELECT           
        @waybill_number = waybill_number          
       FROM           
        OPENQUERY(INT, 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''http://192.168.1.52:8800/egs?cmd=query_waybill_id_range&customer_id=2795296601&waybill_type=B&count=1'' , ''waybill_id'') AS waybill_number')          
      ---- TEST          
      ELSE          
       SELECT           
        @waybill_number = waybill_number          
       FROM           
        OPENQUERY(INT, 'SELECT SJOB.dbo.EDI_EZcatApi_clr(''http://192.168.1.121:8800/egs?cmd=query_waybill_id_range&customer_id=7076259101&waybill_type=B&count=1'' , ''waybill_id'') AS waybill_number')          
     END          
     ELSE          
     BEGIN          
      SET @rtn_code = N'FAIL';          
      SET @rtn_message = N'運單類別 [' + @waybill_type + N'] 代收類別 [' + JSON_VALUE(@waybill_key, '$[0].type') + N'] 錯誤！';          
     END          
        
  --20221215_kye Start 蝦皮黑貓 更改平台出貨狀態        
  SET @parm_03 = JSON_VALUE(@waybill_key, '$[0].erp_site')        
  SET @parm_04 = JSON_VALUE(@waybill_key, '$[0].display_order_number')        
        
  IF ISNULL(@waybill_number,'') <> '' AND　@parm_03 = 'G016'       
  BEGIN        
  　          
    EXEC [INT].[SJOB].[dbo].[Shopee_ShipNTrackingNo_Retry] @parm_03,@parm_04,'G016Z2',@waybill_number ,@rtn_code OUTPUT,@rtn_message OUTPUT        
        
       IF ISNULL(@rtn_code,'') <> ''         
       BEGIN          
        SET @rtn_code = N'FAIL';          
        SET @rtn_message = N'出庫單號 [' + @parm_04 + N'] 更新平台狀態失敗！';          
       END          
    ELSE    
    BEGIN    
      SET @rtn_code = 'PASS'    
    END     
  END        
  --20221215_kye END        
        
    END          
    ---- REY 日翊          
    ELSE IF @waybill_type = N'WBLREY'          
    BEGIN          
     EXEC _sp_get_next_number          
      @number_type    = @waybill_type          
      , @next_number    = @waybill_number OUTPUT          
      --          
      , @rtn_code     = @rtn_code OUTPUT          
      , @rtn_message    = @rtn_message OUTPUT          
          
     ---- Build JSON for waybill_data          
     SELECT TOP 1          
      @waybill_data =           
      (          
       SELECT           
        JSON_VALUE(@waybill_key, '$[0].display_order_number') AS display_order_number          
        , om.ship_to_code AS ship_to_code       
        , CONVERT(NVARCHAR(30), om.cod_amount) AS cod_amount          
        , cs.route_seq AS route_seq           
        , cs.region AS region           
        , cs.equipment_id AS equipment_id           
       FOR JSON PATH          
      )          
     FROM          
      _tv_order_master AS om WITH (NOLOCK)          
      LEFT JOIN _v_customer_site AS cs WITH (NOLOCK)          
       ON cs.customer_code = om.customer_code          
       AND cs.site_code = om.ship_to_code          
     WHERE          
      client_code = JSON_VALUE(@waybill_key, '$[0].client_code')          
      AND display_order_number = JSON_VALUE(@waybill_key, '$[0].display_order_number')          
    END          
    ---- 誠品基金會          
    ELSE IF @waybill_type = N'WBLZ8'          
    BEGIN          
     SET @waybill_number = JSON_VALUE(@waybill_key, '$[0].display_order_number');          
    END          
    ---- 郵局  20230724 by JackyHsu (JH01)      
    ELSE IF @waybill_type IN (N'WBLZA',N'WBLZD')          
    BEGIN          
		DECLARE @Waybill_type_PF NVARCHAR(10)
		set @Waybill_type_PF = 'WBLPF' --郵局ZA與ZD使用相同取號機制因次統一類別  
		EXEC _sp_get_next_number          
		  @number_type    = @Waybill_type_PF         
		, @next_number    = @waybill_number OUTPUT          
		, @rtn_code     = @rtn_code OUTPUT          
		, @rtn_message    = @rtn_message OUTPUT  
		
		/*WBLZA : 小包裹(18) WBLZD : 大包裹(78)*/
		set @waybill_number = @waybill_number + case when  @waybill_type = N'WBLZA' then '18' else '78' end

    END          
    ---- WMS B2B出貨箱號          
    ELSE IF @waybill_type = N'WMSSHPCTN'          
    BEGIN          
     EXEC _sp_get_next_number          
      @number_type    = @waybill_type          
      , @next_number    = @waybill_number OUTPUT          
      --          
      , @rtn_code     = @rtn_code OUTPUT          
      , @rtn_message    = @rtn_message OUTPUT          
    END          
    ---- WMS B2B退貨箱號          
    ELSE IF @waybill_type = N'WMSRTVCTN'          
    BEGIN          
     EXEC _sp_get_next_number          
      @number_type    = @waybill_type          
      , @next_number    = @waybill_number OUTPUT          
     --          
      , @rtn_code     = @rtn_code OUTPUT          
      , @rtn_message    = @rtn_message OUTPUT          
    END          
    ---- 蝦皮          
    ELSE IF @waybill_type = N'WBLSOP'          
    BEGIN          
  --20221215_kye Start        
      
      SET @parm_01 = JSON_VALUE(@waybill_key, '$[0].display_order_number')          
      SET @parm_02 = JSON_VALUE(@waybill_key, '$[0].erp_site')           
      
      EXEC [INT].[SJOB].[dbo].[Shopee_ShipNTrackingNo_Retry] @parm_02,@parm_01,'',@waybill_number OUTPUT,@rtn_code OUTPUT,@rtn_message OUTPUT        
        
     IF @debug = 1          
      PRINT N'[' + @sp_name + N']:' + NCHAR(10)          
       + N'@parm_01: ' + ISNULL(@parm_01, N'(null)') + NCHAR(10)          
       + N'@parm_02: ' + ISNULL(@parm_02, N'(null)') + NCHAR(10)          
          
      ----          
      IF ISNULL(@rtn_code,'') <> '' --AND ISNULL(@waybill_number,'') = ''        
      BEGIN          
       SET @waybill_number =  NULL          
          
       SET @rtn_code = N'FAIL';          
       SET @rtn_message = N'出庫單號 [' + @parm_01 + N'] 狀態 [' + @rtn_code + N'] 不允許出貨！';          
      END          
      ELSE          
      BEGIN          
        
       --Exec [INT].[SJOB].[dbo].[Shopee_Get_Label_Info] @parm_02,@parm_01,'','','' ,'','','',@waybill_data OUTPUT,@rtn_code OUTPUT,@rtn_message OUTPUT        
	   Exec [INT].[SJOB].[dbo].[Shopee_Get_Label_Info_For_HJ] @parm_02,@parm_01,'','','' ,'','','',@waybill_data OUTPUT,@rtn_code OUTPUT,@rtn_message OUTPUT        
        
       IF ISNULL(@rtn_code,'') <> ''         
       BEGIN          
        SET @waybill_number =  NULL          
          
        SET @rtn_code = N'FAIL';          
        SET @rtn_message = N'出庫單號 [' + @parm_01 + N'] 取訂單資訊失敗！';          
       END    
    ELSE    
    BEGIN    
     SET @rtn_code = 'PASS'    
    END    
        
          
       IF @debug = 1          
        PRINT N'@waybill_data: ' + ISNULL(@waybill_data, N'(null)') + NCHAR(10)          
         + N'@waybill_number: ' + ISNULL(@waybill_number, N'(null)') + NCHAR(10)          
          
       IF ISNULL(@waybill_number, N'') <> N''          
        BREAK          
      END              
 --20221215_kye End        
    END  --蝦皮END        
         
          
    BREAK; -- WHILE @retry_count > 0          
   END TRY          
   BEGIN CATCH          
    ---- Get Error Informatons          
    SET @error_number = ERROR_NUMBER();          
    SET @rtn_code = N'FAIL';          
    SET @rtn_message = ERROR_MESSAGE() + NCHAR(10) + N'(' + @sp_name + N', ' + CONVERT(NVARCHAR (10), @error_number) + N')';          
          
    ---- Nested outer transaction          
    IF @@TRANCOUNT = 1          
     ROLLBACK          
    ---- Nested inner transaction          
    ELSE IF @@TRANCOUNT > 1          
     COMMIT;          
          
    ---- Check for Deadlock and Retry as long as the Retry Counter is greater than zero          
    IF (@retry_count > 0 AND @error_number = 1205)          
    BEGIN          
     SET @retry_count = @retry_count - 1;          
          
     ---- Wait for 1 seconds and retry          
     WAITFOR DELAY '00:00:01';          
    END -- IF (@retry_count > 0 AND @error_number = 1205)          
    ELSE          
    BEGIN          
     BREAK; -- WHILE @retry_count > 0          
    END          
   END CATCH          
          
   IF @debug = 1          
    PRINT N'[' + @sp_name + N']:' + NCHAR(10)          
     + N'@rtn_code: ' + ISNULL(@rtn_code, N'(null)') + NCHAR(10)          
     + N'@rtn_message: ' + ISNULL(@rtn_message, N'(null)') + NCHAR(10)          
          
  END -- WHILE @retry_count > 0          
 END TRY          
 BEGIN CATCH          
  ---- Get Error Informatons          
  SET @error_number = ERROR_NUMBER();          
  SET @rtn_code = N'FAIL';          
  SET @rtn_message = ERROR_MESSAGE() + NCHAR(10) + N'(' + @sp_name + N', ' + CONVERT(NVARCHAR (10), @error_number) + N')';          
 END CATCH          
          
 IF @debug = 1          
  PRINT N'[' + @sp_name + N']:' + NCHAR(10)          
   + N'@rtn_code: ' + ISNULL(@rtn_code, N'(null)') + NCHAR(10)          
   + N'@rtn_message: ' + ISNULL(@rtn_message, N'(null)') + NCHAR(10)          
          
 SET @ww_result = @rtn_code;          
 RETURN;          
END;          
          
/***          
---- WBLTCAT          
DECLARE @rtn_code NVARCHAR(10), @rtn_message NVARCHAR(500), @waybill_number NVARCHAR(30), @waybill_data NVARCHAR(2000)          
EXEC _sp_get_waybill_number          
 @waybill_type      = N'WBLTCAT'          
 , @waybill_key      = N'[{"type":"A","ship_to_address":"台北市信義區松德路204號B1"}]'          
 , @waybill_number     = @waybill_number OUTPUT          
 , @waybill_data      = @waybill_data OUTPUT          
 --          
 , @rtn_code       = @rtn_code OUTPUT          
 , @rtn_message      = @rtn_message OUTPUT          
 , @ww_result      = NULL          
SELECT @waybill_number, @waybill_data          
          
---- WBLCVS          
DECLARE @rtn_code NVARCHAR(10), @rtn_message NVARCHAR(500), @waybill_number NVARCHAR(30), @waybill_data NVARCHAR(2000)          
EXEC _sp_get_waybill_number          
 @waybill_type      = N'WBLCVS'          
 , @waybill_key      = N'[{"client_code":"ESL","display_order_number":"0801680339"}]'          
 , @waybill_number     = @waybill_number OUTPUT          
 , @waybill_data      = @waybill_data OUTPUT          
 --          
 , @rtn_code       = @rtn_code OUTPUT          
 , @rtn_message      = @rtn_message OUTPUT          
 , @ww_result      = NULL          
SELECT @waybill_number, @waybill_data          
          
---- WBLSOP          
DECLARE @rtn_code NVARCHAR(10), @rtn_message NVARCHAR(500), @waybill_number NVARCHAR(30), @waybill_data NVARCHAR(4000)          
EXEC _sp_get_waybill_number          
 @waybill_type      = N'WBLSOP'          
 , @waybill_key      = N'[{"client_code":"ESL","display_order_number":"220425G4CWFPMT","erp_site":"G016"}]'          
 , @waybill_number     = @waybill_number OUTPUT          
 , @waybill_data      = @waybill_data OUTPUT          
 --          
 , @rtn_code       = @rtn_code OUTPUT          
 , @rtn_message      = @rtn_message OUTPUT          
 , @ww_result      = NULL          
SELECT @waybill_number, @waybill_data          
          
UPDATE           
 _om          
SET           
 waybill_number = '625453941502'          
-- , waybill_data = '[{"zip_7":"11-110-42-A","db_version":"20112101"}]'          
FROM          
 _t_order_master AS _om          
 INNER JOIN t_order AS om WITH (NOLOCK)          
  ON om.order_id = _om.order_id          
WHERE          
 om.order_number = 'ESL-0802299785'          
***/ 