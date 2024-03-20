USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_sp_add_carton_by_order]    Script Date: 7/27/2023 2:46:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





ALTER PROCEDURE [dbo].[_sp_add_carton_by_order]
	@wh_id								NVARCHAR(10)
	, @order_number						NVARCHAR(30)
	----
	, @carton_number					NVARCHAR(50) OUTPUT
	----
	, @user_id							NVARCHAR(30) = N'HJS'
	, @rtn_code							NVARCHAR(10) = NULL OUTPUT
	, @rtn_message						NVARCHAR(500) = NULL OUTPUT
	, @ww_result						NVARCHAR(10) = NULL OUTPUT
AS

/***
================================================================================
[Name]
	_sp_add_carton_by_order

[Description]
	Get waybill number and add a new carton to the specified order.

[Parameters]
	@wh_id						Warehouse ID.
	@order_number				Order Number.
	@carton_number				New carton number added.
	@user_id					Login user ID.
	@rtn_code					Return code. ('PASS', 'FAIL')
	@rtn_message				Return message.
	@ww_result					Return code for WebWise. (= @rtn_code)

Pointek Co.
--------------------------------------------------------------------------------
2020/10/10 - [Steve] First release.
2022/05/16 - [Steve] Dealing Shopee canceled orders.
2022/07/11 - [Steve] Auto-print waybill and ship list of the closed carton.
================================================================================
***/

/*
================================================================================
 Author			: ken
 Alter Label	: 20220519_ken
 Alter Date		: 20220519
 Description	: add SPD 
 test script	: 
 ===============================================================================
 Author			: ken
 Alter Label	: 20220526_ken
 Alter Date		: 20220526
 Description	: add SPD scope change 
 test script	: 
 ===============================================================================
 Author			: ken
 Alter Label	: 20221022_ken
 Alter Date		: 20221022
 Description	: package auto in Staged 
 test script	: 
 ===============================================================================
 Author			: ken
 Alter Label	: 20221212_ken
 Alter Date		: 20221212
 Description	: add ZA ZD 郵局 
 test script	: 
 ===============================================================================
 ================================================================================  
 Author			: kye
 Alter Label	: 20221215_kye
 Alter Date		: 20221215
 Description	: modify Shopee logic for Shopee API 2.0 
 test script	: 
===============================================================================
 ===============================================================================
 Author			: kye
 Alter Label	: 20230117_kye
 Alter Date		: 20230117
 Description	: 7-11 & REY  Storer validation for OP-029
 test script	: 
===============================================================================

 ===============================================================================
 Author			: JackyHsu
 Alter Label	: 20230727(JH01)
 Alter Date		: 20230727
 Description	: 回歸郵局(ZA,ZD)取號機制
 test script	: 
===============================================================================

*/

BEGIN
    SET NOCOUNT ON;

	DECLARE
		---- Error handling variables
		@sp_name						NVARCHAR(30) = N'_sp_add_carton_by_order'
		, @error_number					INT
		, @retry_count					INT = 3
		, @debug						BIT = 1
		, @dt							DATETIME = GETDATE()
		----
		, @client_code					NVARCHAR(30)
		, @display_order_number			NVARCHAR(30)
		, @erp_site						NVARCHAR(15)
		----
		, @type_text					NVARCHAR(20)
		, @last_carton_seq				INT
		, @last_carton_number			NVARCHAR(50)
		, @carton_seq					INT
		, @carton_label					NVARCHAR(50)
		----
		, @waybill_type					NVARCHAR(10)
		, @waybill_number				NVARCHAR(30)
		, @waybill_key					NVARCHAR(4000)
		, @waybill_data					NVARCHAR(4000)
		----
		, @barcode_1					NVARCHAR(30)
		, @barcode_2					NVARCHAR(30)
		, @barcode_qr					NVARCHAR(600)
		----
		, @full_address					NVARCHAR(2000)
		, @ship_to_code					NVARCHAR(30)
		, @ship_to_name					NVARCHAR(30)
		, @ship_to_description			NVARCHAR(100)
		, @ship_to_phone				NVARCHAR(30)
		, @branch_code					NVARCHAR(100) --20220526_ken
		, @branch_name					NVARCHAR(2000)--20220526_ken
		, @customer_code                NVARCHAR(30)  --20220526_ken
		----
		, @report_name					NVARCHAR(60)
		, @report_params				NVARCHAR(500)
		----
		, @note_message					NVARCHAR(500)
		----

	BEGIN TRY
		SET @rtn_code = N'PASS';
		SET @rtn_message = NULL;

-- >>>>> Customization codes <<<<< --

		IF @debug = 1
			PRINT N'[' + @sp_name + N']:' +  NCHAR(10)
				+ N'@wh_id: ' + ISNULL(@wh_id, N'(null)') + NCHAR(10)
				+ N'@order_number: ' + ISNULL(@order_number, N'(null)') + NCHAR(10)
				
		WHILE @retry_count > 0
		BEGIN
			SET @rtn_code = N'PASS';
			SET @rtn_message = NULL;

			BEGIN TRY

				---- Check if the order exists and get information for getting wayill number
				SELECT TOP 1
					@client_code = om.client_code
					, @display_order_number = om.display_order_number
					, @erp_site = om.erp_site
					, @type_text = om.type_text
					, @waybill_type = 
						CASE
							WHEN (om.type_text = N'CSO') AND (om.erp_site = 'G016') AND (customer_code <> 'T-CAT') THEN 'WBLSOP' -- 蝦皮
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'FEDEX') THEN 'WBLFDX' -- FedEx
							WHEN (om.type_text = N'CSO') AND (om.customer_code = '7-11') THEN 'WBL711' -- 7-11
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'CVS') THEN 'WBLCVS' -- CVS
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'T-CAT') THEN 'WBLTCAT' -- 黑貓
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'REY') THEN 'WBLREY' -- 全家日翊
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'ESL') THEN 'WBLESL' -- 誠品線上
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'CK') THEN 'WBLCK' -- 香港 CK
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'Z8') THEN 'WBLZ8' -- 誠品基金會
                            WHEN (om.type_text = N'CSO') AND (om.customer_code = 'ZA') THEN 'WBLZA' -- 郵局ZA
                            WHEN (om.type_text = N'CSO') AND (om.customer_code = 'ZD') THEN 'WBLZD' -- 郵局ZD
							WHEN (om.type_text = N'SO') THEN 'WMSSHPCTN' -- 門市出貨
							WHEN (om.type_text IN (N'RSO', N'RSO_M1')) THEN 'WMSRTVCTN' -- 供應商退貨
							ELSE N''
						END
					, @waybill_key = 
						CASE
							WHEN (om.type_text = N'CSO') AND (om.erp_site = 'G016') AND (customer_code <> 'T-CAT') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number, om.erp_site AS erp_site FOR JSON PATH) -- 蝦皮
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'FEDEX') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- FedEx
							WHEN (om.type_text = N'CSO') AND (om.customer_code = '7-11') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 7-11
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'CVS') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- CVS
							--WHEN (om.type_text = N'CSO') AND (om.customer_code = 'T-CAT') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number, IIF(om.cod_flag = N'N', N'A', N'B') AS type, om.ship_to_city + om.ship_to_state + om.ship_to_address AS ship_to_address FOR JSON PATH) -- 黑貓
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'T-CAT') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number, IIF(om.cod_flag = N'N', N'A', N'B') AS type, om.ship_to_city + om.ship_to_state + om.ship_to_address AS ship_to_address , om.erp_site AS erp_site FOR JSON PATH) -- 黑貓   --20221215_kye
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'REY') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 全家日翊
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'ESL') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 誠品線上
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'CK') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 香港 CK
							WHEN (om.type_text = N'CSO') AND (om.customer_code = 'Z8') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 誠品基金會
                            WHEN (om.type_text = N'CSO') AND (om.customer_code = 'ZA') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 郵局ZA 20221212_ken 
                            WHEN (om.type_text = N'CSO') AND (om.customer_code = 'ZC') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 郵局ZD 20221212_ken
							WHEN (om.type_text = N'SO') THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 門市出貨
							WHEN (om.type_text IN (N'RSO', N'RSO_M1')) THEN (SELECT om.client_code AS client_code, om.display_order_number AS display_order_number FOR JSON PATH) -- 供應商退貨
							ELSE NULL
						END
				FROM
					_tv_order_master AS om WITH (NOLOCK)
				WHERE
					om.wh_id = @wh_id
					AND om.order_number = @order_number

				IF @@ROWCOUNT = 0
				BEGIN
					SET @rtn_code = N'FAIL';
					SET @rtn_message = N'倉庫 [' + @wh_id + N'] 系統出庫單號 [' + @order_number + N'] 不存在！';
				END

				IF @rtn_code = N'FAIL'
					THROW 50000, @rtn_message, 1;

				---- Get last carton information
				SELECT TOP 1
					@last_carton_seq = ctnm.carton_seq
					, @last_carton_number = ctnm.carton_number
				FROM
					_t_order_carton_master AS ctnm WITH (NOLOCK)
				WHERE
					ctnm.wh_id = @wh_id
					AND ctnm.order_number = @order_number
				ORDER BY
					ctnm.wh_id
					, ctnm.order_number
					, ctnm.carton_seq DESC

				IF @@ROWCOUNT = 0
				BEGIN
					SET @last_carton_seq = 0
					SET @last_carton_number = NULL
				END

				---- Check if the last carton is empty (nothing been packed in it's detail).
				IF @last_carton_number IS NOT NULL AND NOT EXISTS
				(
					SELECT
						1
					FROM
						_t_order_carton_detail AS ctnd WITH (NOLOCK)
					WHERE
						wh_id = @wh_id
						AND carton_number = @last_carton_number
						AND qty_packed > 0
				)
				BEGIN
					SET @rtn_code = N'FAIL';
					SET @rtn_message = N'倉庫 [' + @wh_id + N'] 系統出庫箱號 [' + @last_carton_number + N'] 未執行包裝，不可加箱！';
				END

				IF @rtn_code = N'FAIL'
					THROW 50000, @rtn_message, 1;

				---- Set new carton number and sequence
				SET @carton_seq = @last_carton_seq + 1
				SET @carton_number = @order_number + IIF(@last_carton_seq = 0, N'', N'_' + FORMAT(@carton_seq, N'000'))

				IF @debug = 1
					PRINT N'[' + @sp_name + N']:' +  NCHAR(10)
						+ N'@client_code: ' + ISNULL(@client_code, N'(null)') + NCHAR(10)
						+ N'@display_order_number: ' + ISNULL(@display_order_number, N'(null)') + NCHAR(10)
						+ N'@erp_site: ' + ISNULL(@erp_site, N'(null)') + NCHAR(10)
						+ N'@type_text: ' + ISNULL(@type_text, N'(null)') + NCHAR(10)
						+ N'@waybill_type: ' + ISNULL(@waybill_type, N'(null)') + NCHAR(10)
						+ N'@waybill_key: ' + ISNULL(@waybill_key, N'(null)') + NCHAR(10)

				---- Get waybill number and data
				SET @ship_to_code = NULL
				SET @ship_to_name = NULL
				SET @ship_to_description = NULL
				SET @ship_to_phone = NULL

				IF @waybill_type IN ('WBLCVS', 'WBLCK')
				BEGIN
					SET @waybill_number = @carton_number;
					SET @waybill_data = NULL;
					SET @carton_label = @carton_number;
				END
				ELSE IF @waybill_type IN ('WBLZ8')
				BEGIN
					SET @waybill_number = @display_order_number;
					SET @waybill_data = NULL;
					SET @carton_label = @display_order_number;
				END
				/*20230727 取消郵局判斷加入HJ取號機制 (JH01)*/
    --            ELSE IF @waybill_type IN ('WBLZA','WBLZD') --20221212_ken
				--BEGIN
				--	SET @waybill_number = @display_order_number;
				--	SET @waybill_data = NULL;
				--	SET @carton_label = @display_order_number;
				--END
				ELSE
				BEGIN
					---- Get waybill number and data
					EXEC _sp_get_waybill_number
						@waybill_type				= @waybill_type
						, @waybill_key				= @waybill_key
						, @waybill_number			= @waybill_number OUTPUT
						, @waybill_data				= @waybill_data OUTPUT
						----
						, @rtn_code					= @rtn_code OUTPUT
						, @rtn_message				= @rtn_message OUTPUT

					IF ISNULL(@waybill_number, N'') = N''
					BEGIN
-- >>>>> By Steve, 2022/05/16 -> For Shopee canceled orders.
						-- [Now]
						SET @waybill_number = NULL

						-- [Was]
						/*
						SET @rtn_code = N'FAIL';
						SET @rtn_message = N'出庫單號 [' + JSON_VALUE(@waybill_key, '$[0].display_order_number') + N'] 取號失敗！';

						IF @rtn_code = N'FAIL'
							THROW 50000, @rtn_message, 1;
						*/
-- <<<<<
					END

					SET @carton_label = @waybill_number
				END


				--Start (20230117_kye)  
				IF @waybill_type IN ( 'WBL711' ,'WBLREY')
				BEGIN  
				 SELECT 1 
				 FROM 
				  dbo._tv_Order_master AS om WITH (NOLOCK) 
				 INNER JOIN dbo._v_customer_site AS cs WITH (NOLOCK) 
				  ON om.ship_to_code = cs.site_code
				  AND om.customer_code = cs.customer_code
				 WHERE
				  om.wh_id = @wh_id  
				  AND om.order_number = @order_number  
				
				 IF @@ROWCOUNT < 1
				 BEGIN

				   UPDATE  
				    om
				   SET  
				    errlog = 'S-無店鋪主檔' 
				   FROM  
				    dbo._tv_Order_master AS om  
				   WHERE  
				    om.wh_id = @wh_id  
				    AND om.order_number = @order_number  

				   SET @rtn_code = N'FAIL';  
				   SET @rtn_message = N'出庫單號 [' + JSON_VALUE(@waybill_key, '$[0].display_order_number') + N'] 無店鋪主檔！';  
				 END
				END
				--End (20230117_kye)

				---- Special carton lebel barcode encoding
				IF @waybill_number IS NOT NULL
				BEGIN
					IF @waybill_type = 'WBL711'
					BEGIN
						SELECT TOP 1
							@carton_label = om.ship_to_code + N'785001' + @waybill_number
						FROM
							_tv_order_master AS om WITH (NOLOCK)
						WHERE
							om.wh_id = @wh_id
							AND om.order_number = @order_number
					END
					ELSE IF @waybill_type = 'WBLREY'
					BEGIN
						SELECT TOP 1
							@barcode_1 = -- 門店一段條碼
								N'281' -- EC廠商代號3碼
								+ LEFT(RIGHT(REPLICATE(N'0', 11) + @waybill_number, 11), 3) -- 訂單編號前3碼
								+ N'762' -- 代收代號3碼
							, @barcode_2 = -- 門店二段條碼
								RIGHT(REPLICATE(N'0', 11) + @waybill_number, 8) -- 訂單編號後8碼
								+ IIF(om.cod_flag = N'Y', N'1', N'3') -- 交易方式1碼 (1:取貨付款, 3:取貨不付款)
								+ RIGHT(REPLICATE(N'0', 5) + CONVERT(NVARCHAR(5), om.cod_amount), 5) -- 代收金額5碼
						FROM
							_tv_order_master AS om WITH (NOLOCK)
						WHERE
							om.wh_id = @wh_id
							AND om.order_number = @order_number
				
						SELECT @carton_label = @barcode_2 + dbo._fn_get_cvs_barcode_checksum(N'2', @barcode_1 + N'0' + @barcode_2)
					END
					ELSE IF @waybill_type = 'WBLSOP'
					BEGIN
						SELECT TOP 1
							@carton_label =  
							CASE  
							  WHEN om.customer_code = N'7-11' THEN JSON_VALUE(@waybill_data, '$[0].BarCode')  
							  WHEN om.customer_code = N'CVS' THEN JSON_VALUE(@waybill_data, '$[0].ec_bar_code16') 
							  -- 20220519_ken add SPD   
							  WHEN om.customer_code = N'SPD' THEN JSON_VALUE(@waybill_data, '$[0].TrackingNo') 
							END        
							, @ship_to_name = JSON_VALUE(@waybill_data, '$[0].c_Name')  
							, @ship_to_phone = JSON_VALUE(@waybill_data, '$[0].c_Phone')  
							--@carton_label =
							--CASE
							--	WHEN om.customer_code = N'7-11' THEN JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.barcode')
							--	WHEN om.customer_code = N'CVS' THEN JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.ec_bar_code16')
							--	-- 20220519_ken add SPD 
							--	WHEN om.customer_code = N'SPD' THEN JSON_VALUE(@waybill_data, '$.shopee_tracking_no')
							--END						
							--, @ship_to_name = JSON_VALUE(JSON_QUERY(@waybill_data, '$.recipient_address'), '$.name')
							--, @ship_to_phone = JSON_VALUE(JSON_QUERY(@waybill_data, '$.recipient_address'), '$.phone')
							--, @full_address = JSON_VALUE(JSON_QUERY(@waybill_data, '$.recipient_address'), '$.full_address')
							-- 20220526_ken
							, @customer_code = om.customer_code
							, @branch_code = 
							CASE 
								--WHEN om.customer_code = N'SPD' THEN IIF(CHARINDEX('--', JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.branch_code')) = 0
								--										, JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.branch_code')
								--										, SUBSTRING(JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.branch_code'), 0, CHARINDEX('--', JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.branch_code'))))
							    WHEN om.customer_code = N'SPD' THEN IIF(CHARINDEX('--', JSON_VALUE(@waybill_data, '$[0].branch_code')) = 0  
							              , JSON_VALUE(@waybill_data, '$[0].branch_code') 
							              , Substring(JSON_VALUE(@waybill_data, '$[0].branch_code') , 1,
							                  CHARINDEX(dbo.StrArgIndex(Replace(JSON_VALUE(@waybill_data, '$[0].branch_code') ,'-',','),4),JSON_VALUE(@waybill_data, '$[0].branch_code') ) 
							                  + LEN(dbo.StrArgIndex(Replace(JSON_VALUE(@waybill_data, '$[0].branch_code') ,'-',','),4)) -1
							                )
										  )  
							ELSE
								''
							END
							, @branch_name = 
							CASE 
								--WHEN om.customer_code = N'SPD' THEN JSON_VALUE(JSON_QUERY(@waybill_data, '$.third_party_logistic_info'), '$.branch_name')
								WHEN om.customer_code = N'SPD' THEN JSON_VALUE(@waybill_data, '$[0].branch_name')  --20221215_kye
							ELSE
								''
							END
						FROM
							_tv_order_master AS om WITH (NOLOCK)
						WHERE
							om.wh_id = @wh_id
							AND om.order_number = @order_number

						-- 20220526_ken
						IF @customer_code = 'SPD'
						BEGIN
						    SET @ship_to_code = @branch_code
							SET @ship_to_description = @branch_name
						END
						ELSE
						BEGIN --20221215_kye
							--SET @ship_to_code = SUBSTRING(@full_address, CHARINDEX('店號', @full_address) + 2, 100)
							--SET @ship_to_description = LEFT(SUBSTRING(@full_address, CHARINDEX(' ', @full_address) + 1, 100), CHARINDEX(' ', SUBSTRING(@full_address, CHARINDEX(' ', @full_address) + 1, 100)))
							SET @ship_to_code = NULL
							SET @ship_to_description = NULL
						END
						
						--SET @ship_to_code = SUBSTRING(@full_address, CHARINDEX('店號', @full_address) + 2, 100)
						--SET @ship_to_description = LEFT(SUBSTRING(@full_address, CHARINDEX(' ', @full_address) + 1, 100), CHARINDEX(' ', SUBSTRING(@full_address, CHARINDEX(' ', @full_address) + 1, 100)))
					END
					ELSE IF @waybill_type = 'WBLFDX'
					BEGIN
						SET @carton_label = @waybill_number + N'0430' -- FedEx Tracking Form ID
					END

				END

				IF @debug = 1
					PRINT N'[' + @sp_name + N']:' + NCHAR(10)
						+ N'@waybill_number: ' + ISNULL(@waybill_number, N'(null)') + NCHAR(10)
						+ N'@waybill_data: ' + ISNULL(@waybill_data, N'(null)') + NCHAR(10)
						+ N'@carton_label: ' + ISNULL(@carton_label, N'(null)') + NCHAR(10)

				---- Start transaction
				BEGIN TRANSACTION
						
				---- Insert carton master
				INSERT INTO
					_t_order_carton_master
					(
						--id --bigint IDENTITY(1,1) NOT NULL,
						wh_id -- NVARCHAR(10) NOT NULL,
						, carton_number -- NVARCHAR(50) NOT NULL,
						, client_code -- NVARCHAR(30) NOT NULL,
						, erp_site -- NVARCHAR(15) NOT NULL,
						, carton_label -- NVARCHAR(50) NULL,
						, carton_type -- NVARCHAR(20) NOT NULL,
						, carton_seq --int NULL,
						, master_carton_number -- NVARCHAR(50) NULL,
						, waybill_number -- NVARCHAR(50) NULL,
						, waybill_data -- NVARCHAR(4000) NULL,
						, status -- NVARCHAR(10) NOT NULL,
						, location_id -- NVARCHAR(50) NULL,
						, hu_id -- NVARCHAR(22) NULL,
						, reg_number -- NVARCHAR(30) NULL,
						, reg_datetime --datetime NULL,
						, po_number -- NVARCHAR(30) NULL,
						, display_po_number -- NVARCHAR(30) NULL,
						, ref_po_number -- NVARCHAR(30) NULL,
						, order_number -- NVARCHAR(30) NULL,
						, display_order_number -- NVARCHAR(30) NULL,
						, ref_order_number -- NVARCHAR(30) NULL,
						, wave_id -- NVARCHAR(20) NULL,
						, batch_id -- NVARCHAR(30) NULL,
						, customer_code -- NVARCHAR(30) NULL,
						, ship_to_email -- NVARCHAR(80) NULL,
						, ship_to_code -- NVARCHAR(30) NULL,
						, ship_to_description -- NVARCHAR(100) NULL,
						, ship_to_code_2 -- NVARCHAR(30) NULL,
						, ship_to_description_2 -- NVARCHAR(100) NULL,
						, ship_to_name -- NVARCHAR(30) NULL,
						, ship_to_address -- NVARCHAR(1000) NULL,
						, ship_to_country_name -- NVARCHAR(30) NULL,
						, ship_to_state -- NVARCHAR(50) NULL,
						, ship_to_city -- NVARCHAR(30) NULL,
						, ship_to_zip -- NVARCHAR(30) NULL,
						, ship_to_fax -- NVARCHAR(30) NULL,
						, ship_to_phone -- NVARCHAR(30) NULL,
						, cod_type -- NVARCHAR(30) NULL,
						, cod_flag -- NVARCHAR(30) NULL,
						, cod_amount --float NULL,
						, total_items --int NULL,
						, total_qty --float NULL,
						, total_amount --float NULL,
						, print_invoice -- NVARCHAR(2) NULL,
						, bill_to_email -- NVARCHAR(80) NULL,
						, bill_to_code -- NVARCHAR(30) NULL,
						, bill_to_description -- NVARCHAR(100) NULL,
						, bill_to_name -- NVARCHAR(30) NULL,
						, bill_to_address -- NVARCHAR(1000) NULL,
						, bill_to_country_name -- NVARCHAR(30) NULL,
						, bill_to_state -- NVARCHAR(50) NULL,
						, bill_to_city -- NVARCHAR(30) NULL,
						, bill_to_zip -- NVARCHAR(30) NULL,
						, bill_to_fax -- NVARCHAR(30) NULL,
						, bill_to_phone -- NVARCHAR(30) NULL,
						, exp_container_type -- NVARCHAR(10) NULL,
						, container_type -- NVARCHAR(10) NULL,
						, exp_carton_volume --float NULL,
						, carton_volume --float NULL,
						, exp_carton_weight --float NULL,
						, carton_weight --float NULL,
						, expect_carton_count --int NULL,
						, carton_count --int NULL,
						, last_carton_flag -- NVARCHAR(10) NULL,
						, pick_user -- NVARCHAR(30) NULL,
						, pick_dt --datetime NULL,
						, pack_user -- NVARCHAR(30) NULL,
						, pack_dt --datetime NULL,
						, ship_user -- NVARCHAR(30) NULL,
						, ship_dt --datetime NULL,
						, delivery_user -- NVARCHAR(30) NULL,
						, delivery_dt --datetime NULL,
						, export_id -- NVARCHAR(20) NULL,
						, export_dt --datetime NULL,
						, create_dt --datetime NULL,
						, update_dt --datetime NULL,
						, comment -- NVARCHAR(2000) NULL
					)
-- >>>>> By Steve, 2022/05/16 -> For Shopee canceled orders.
				-- [Now]
				SELECT 
					--id --bigint IDENTITY(1,1) NOT NULL,
					@wh_id AS wh_id -- NVARCHAR(10) NOT NULL,
					, @carton_number -- NVARCHAR(50) NOT NULL,
					, @client_code AS client_code -- NVARCHAR(30) NOT NULL,
					, @erp_site AS erp_site -- NVARCHAR(15) NOT NULL,
					, @carton_label AS carton_label -- NVARCHAR(50) NULL,
					, @type_text AS carton_type -- NVARCHAR(20) NOT NULL,
					, @carton_seq AS carton_seq -- int NULL,
					, @carton_number AS master_carton_number -- NVARCHAR(50) NULL,
					, @waybill_number AS waybill_number -- NVARCHAR(50) NULL,
					, @waybill_data AS waybill_data -- NVARCHAR(4000) NULL,
					, N'N' AS status -- NVARCHAR(10) NOT NULL,
					, NULL AS location_id -- NVARCHAR(50) NULL,
					, NULL AS hu_id -- NVARCHAR(22) NULL,
					, NULL AS reg_number -- NVARCHAR(30) NULL,
					, NULL AS reg_datetime -- datetime NULL,
					, NULL AS po_number -- NVARCHAR(30) NULL,
					, NULL AS display_po_number -- NVARCHAR(30) NULL,
					, NULL AS ref_po_number -- NVARCHAR(30) NULL,
					, om.order_number AS order_number -- NVARCHAR(30) NULL,
					, om.display_order_number AS display_order_number -- NVARCHAR(30) NULL,
					, NULL AS ref_order_number -- NVARCHAR(30) NULL,
					, wd.wave_id AS wave_id -- NVARCHAR(20) NULL,
					, wb.batch_id AS batch_id -- NVARCHAR(30) NULL,
					, om.customer_code AS customer_code -- NVARCHAR(30) NULL,
					, NULL AS ship_to_email -- NVARCHAR(80) NULL,
					, ISNULL(@ship_to_code, om.ship_to_code) AS ship_to_code -- NVARCHAR(30) NULL,
					, ISNULL(@ship_to_description, om.ship_to_description) AS ship_to_description -- NVARCHAR(100) NULL,
					, NULL AS ship_to_code_2 -- NVARCHAR(30) NULL,
					, NULL AS ship_to_description_2 -- NVARCHAR(100) NULL,
					, ISNULL(@ship_to_name, om.ship_to_name) AS ship_to_name -- NVARCHAR(30) NULL,
					, om.ship_to_address AS ship_to_address -- NVARCHAR(1000) NULL,
					, om.ship_to_country_name AS ship_to_country_name -- NVARCHAR(30) NULL,
					, om.ship_to_state AS ship_to_state -- NVARCHAR(50) NULL,
					, om.ship_to_city AS ship_to_city -- NVARCHAR(30) NULL,
					, om.ship_to_zip AS ship_to_zip -- NVARCHAR(30) NULL,
					, om.ship_to_fax AS ship_to_fax -- NVARCHAR(30) NULL,
					, ISNULL(@ship_to_phone, om.ship_to_phone) AS ship_to_phone -- NVARCHAR(30) NULL,
					, om.cod_type AS cod_type -- NVARCHAR(30) NULL,
					, om.cod_flag AS cod_flag -- NVARCHAR(30) NULL,
					, om.cod_amount AS cod_amount -- float NULL,
					, NULL AS total_items --(SELECT COUNT(DISTINCT item_number) FROM _t_order_carton_detail WITH (NOLOCK) WHERE wh_id = om.wh_id AND carton_number = om.order_number ) AS total_items -- int NULL,
					, NULL AS total_qty --(SELECT ISNULL(SUM(ISNULL(qty, 0)), 0) FROM _t_order_carton_detail WITH (NOLOCK) WHERE wh_id = om.wh_id AND carton_number = om.order_number ) AS total_qty -- float NULL,
					, NULL AS total_amount -- float NULL,
					, NULL AS print_invoice -- NVARCHAR(2) NULL,
					, NULL AS bill_to_email -- NVARCHAR(80) NULL,
					, om.bill_to_code AS bill_to_code -- NVARCHAR(30) NULL,
					, om.bill_to_description AS bill_to_description -- NVARCHAR(100) NULL,
					, NULL AS bill_to_name -- NVARCHAR(30) NULL,
					, NULL AS bill_to_address -- NVARCHAR(1000) NULL,
					, NULL AS bill_to_country_name -- NVARCHAR(30) NULL,
					, NULL AS bill_to_state -- NVARCHAR(50) NULL,
					, NULL AS bill_to_city -- NVARCHAR(30) NULL,
					, NULL AS bill_to_zip -- NVARCHAR(30) NULL,
					, NULL AS bill_to_fax -- NVARCHAR(30) NULL,
					, NULL AS bill_to_phone -- NVARCHAR(30) NULL,
					, NULL AS exp_container_type -- NVARCHAR(10) NULL,
					, NULL AS container_type -- NVARCHAR(10) NULL,
					, NULL AS exp_carton_volume -- float NULL,
					, NULL AS carton_volume -- float NULL,
					, NULL AS exp_carton_weight -- float NULL,
					, NULL AS carton_weight -- float NULL,
					, 1 AS expect_carton_count -- int NULL,
					, 1 AS carton_count -- int NULL,
					, N'Y' AS last_carton_flag -- NVARCHAR(10) NULL,
					, @user_id AS pick_user -- NVARCHAR(30) NULL,
					, @dt AS pick_dt -- datetime NULL,
					, @user_id AS pack_user -- NVARCHAR(30) NULL,
					, @dt AS pack_dt -- datetime NULL,
					, NULL AS ship_user -- NVARCHAR(30) NULL,
					, NULL AS ship_dt -- datetime NULL,
					, NULL AS delivery_user -- NVARCHAR(30) NULL,
					, NULL AS delivery_dt -- datetime NULL,
					, NULL AS export_id -- NVARCHAR(20) NULL,
					, NULL AS export_dt -- datetime NULL,
					, @dt AS create_dt -- datetime NULL,
					, @dt AS update_dt -- datetime NULL,
					, NULL AS comment -- NVARCHAR(2000) NULL
				FROM
					_tv_order_master AS om WITH (NOLOCK)
					LEFT JOIN t_afo_wave_detail AS wd WITH (NOLOCK)
						ON om.wh_id = wd.wh_id
						AND om.order_number = wd.order_number
					LEFT JOIN _t_wave_batch AS wb WITH (NOLOCK)
						ON om.wh_id = wb.wh_id
						AND om.order_number = wb.order_number

				-- [Was]
				/*
				SELECT 
					--id --bigint IDENTITY(1,1) NOT NULL,
					@wh_id AS wh_id -- NVARCHAR(10) NOT NULL,
					, @carton_number -- NVARCHAR(50) NOT NULL,
					, @client_code AS client_code -- NVARCHAR(30) NOT NULL,
					, @erp_site AS erp_site -- NVARCHAR(15) NOT NULL,
					, @carton_label AS carton_label -- NVARCHAR(50) NULL,
					, @type_text AS carton_type -- NVARCHAR(20) NOT NULL,
					, @carton_seq AS carton_seq -- int NULL,
					, @carton_number AS master_carton_number -- NVARCHAR(50) NULL,
					, @waybill_number AS waybill_number -- NVARCHAR(50) NULL,
					, @waybill_data AS waybill_data -- NVARCHAR(4000) NULL,
					, N'N' AS status -- NVARCHAR(10) NOT NULL,
					, NULL AS location_id -- NVARCHAR(50) NULL,
					, NULL AS hu_id -- NVARCHAR(22) NULL,
					, NULL AS reg_number -- NVARCHAR(30) NULL,
					, NULL AS reg_datetime -- datetime NULL,
					, NULL AS po_number -- NVARCHAR(30) NULL,
					, NULL AS display_po_number -- NVARCHAR(30) NULL,
					, NULL AS ref_po_number -- NVARCHAR(30) NULL,
					, om.order_number AS order_number -- NVARCHAR(30) NULL,
					, om.display_order_number AS display_order_number -- NVARCHAR(30) NULL,
					, NULL AS ref_order_number -- NVARCHAR(30) NULL,
					, wb.wave_id AS wave_id -- NVARCHAR(20) NULL,
					, wb.batch_id AS batch_id -- NVARCHAR(30) NULL,
					, om.customer_code AS customer_code -- NVARCHAR(30) NULL,
					, NULL AS ship_to_email -- NVARCHAR(80) NULL,
					, ISNULL(@ship_to_code, om.ship_to_code) AS ship_to_code -- NVARCHAR(30) NULL,
					, ISNULL(@ship_to_description, om.ship_to_description) AS ship_to_description -- NVARCHAR(100) NULL,
					, NULL AS ship_to_code_2 -- NVARCHAR(30) NULL,
					, NULL AS ship_to_description_2 -- NVARCHAR(100) NULL,
					, ISNULL(@ship_to_name, om.ship_to_name) AS ship_to_name -- NVARCHAR(30) NULL,
					, om.ship_to_address AS ship_to_address -- NVARCHAR(1000) NULL,
					, om.ship_to_country_name AS ship_to_country_name -- NVARCHAR(30) NULL,
					, om.ship_to_state AS ship_to_state -- NVARCHAR(50) NULL,
					, om.ship_to_city AS ship_to_city -- NVARCHAR(30) NULL,
					, om.ship_to_zip AS ship_to_zip -- NVARCHAR(30) NULL,
					, om.ship_to_fax AS ship_to_fax -- NVARCHAR(30) NULL,
					, ISNULL(@ship_to_phone, om.ship_to_phone) AS ship_to_phone -- NVARCHAR(30) NULL,
					, om.cod_type AS cod_type -- NVARCHAR(30) NULL,
					, om.cod_flag AS cod_flag -- NVARCHAR(30) NULL,
					, om.cod_amount AS cod_amount -- float NULL,
					, NULL --(SELECT COUNT(DISTINCT item_number) FROM _t_order_carton_detail WITH (NOLOCK) WHERE wh_id = om.wh_id AND carton_number = om.order_number ) AS total_items -- int NULL,
					, NULL --(SELECT ISNULL(SUM(ISNULL(qty, 0)), 0) FROM _t_order_carton_detail WITH (NOLOCK) WHERE wh_id = om.wh_id AND carton_number = om.order_number ) AS total_qty -- float NULL,
					, NULL AS total_amount -- float NULL,
					, NULL AS print_invoice -- NVARCHAR(2) NULL,
					, NULL AS bill_to_email -- NVARCHAR(80) NULL,
					, om.bill_to_code AS bill_to_code -- NVARCHAR(30) NULL,
					, om.bill_to_description AS bill_to_description -- NVARCHAR(100) NULL,
					, NULL AS bill_to_name -- NVARCHAR(30) NULL,
					, NULL AS bill_to_address -- NVARCHAR(1000) NULL,
					, NULL AS bill_to_country_name -- NVARCHAR(30) NULL,
					, NULL AS bill_to_state -- NVARCHAR(50) NULL,
					, NULL AS bill_to_city -- NVARCHAR(30) NULL,
					, NULL AS bill_to_zip -- NVARCHAR(30) NULL,
					, NULL AS bill_to_fax -- NVARCHAR(30) NULL,
					, NULL AS bill_to_phone -- NVARCHAR(30) NULL,
					, NULL AS exp_container_type -- NVARCHAR(10) NULL,
					, NULL AS container_type -- NVARCHAR(10) NULL,
					, NULL AS exp_carton_volume -- float NULL,
					, NULL AS carton_volume -- float NULL,
					, NULL AS exp_carton_weight -- float NULL,
					, NULL AS carton_weight -- float NULL,
					, 1 AS expect_carton_count -- int NULL,
					, 1 AS carton_count -- int NULL,
					, N'Y' AS last_carton_flag -- NVARCHAR(10) NULL,
					, @user_id AS pick_user -- NVARCHAR(30) NULL,
					, @dt AS pick_dt -- datetime NULL,
					, @user_id AS pack_user -- NVARCHAR(30) NULL,
					, @dt AS pack_dt -- datetime NULL,
					, NULL AS ship_user -- NVARCHAR(30) NULL,
					, NULL AS ship_dt -- datetime NULL,
					, NULL AS delivery_user -- NVARCHAR(30) NULL,
					, NULL AS delivery_dt -- datetime NULL,
					, NULL AS export_id -- NVARCHAR(20) NULL,
					, NULL AS export_dt -- datetime NULL,
					, @dt AS create_dt -- datetime NULL,
					, @dt AS update_dt -- datetime NULL,
					, NULL AS comment -- NVARCHAR(2000) NULL
				FROM
					_tv_order_master AS om WITH (NOLOCK)
					LEFT JOIN _t_wave_batch AS wb WITH (NOLOCK)
						ON wb.wh_id = om.wh_id
						AND wb.order_number = om.order_number
				*/
-- <<<<<
				WHERE
					om.wh_id = @wh_id
					AND om.order_number = @order_number

				---- The 1st carton of the order
				IF @carton_seq = 1
				BEGIN
					---- Insert carton detail
					INSERT INTO
						_t_order_carton_detail
						(
							--id -- BIGINT IDENTITY(1,1) NOT NULL,
							wh_id -- NVARCHAR(10) NOT NULL,
							, carton_number -- NVARCHAR(50) NOT NULL,
							, client_code -- NVARCHAR(30) NOT NULL,
							, erp_site -- NVARCHAR(15) NOT NULL,
							, line_number -- NVARCHAR(10) NOT NULL,
							, item_number -- NVARCHAR(60) NOT NULL,
							, display_item_number -- NVARCHAR(30) NULL,
							, ean -- NVARCHAR(20) NULL,
							, isbn -- NVARCHAR(20) NULL,
							, item_description_full -- NVARCHAR(200) NULL,
							, po_number -- NVARCHAR(30) NULL,
							, display_po_number -- NVARCHAR(30) NULL,
							, po_line_number -- NVARCHAR(10) NULL,
							, ref_po_number-- NVARCHAR(30) NULL,
							, ref_po_line_number -- NVARCHAR(10) NULL,
							, order_number -- NVARCHAR(30) NULL,
							, display_order_number -- NVARCHAR(30) NULL,
							, order_line_number -- NVARCHAR(10) NULL,
							, ref_order_number-- NVARCHAR(30) NULL,
							, ref_order_line_number -- NVARCHAR(10) NULL,
							, qty -- float NULL,
							, qty_picked -- float NULL,
							, qty_packed -- float NULL,
							, qty_unpack -- float NULL,
							, qty_shipped -- float NULL,
							, pick_location -- NVARCHAR(50) NULL,
							, comment -- NVARCHAR(2000) NULL,
						)
					SELECT
						--id -- bigint IDENTITY(1,1) NOT NULL,
						od.wh_id AS wh_id -- NVARCHAR(10) NOT NULL,
						, od.order_number AS carton_number -- NVARCHAR(50) NOT NULL,
						, od.client_code AS client_code -- NVARCHAR(30) NOT NULL,
						, od.erp_site AS erp_site -- NVARCHAR(15) NOT NULL,
						, FORMAT(ROW_NUMBER() OVER(ORDER BY od.order_number, od.line_number), N'0000000000') AS line_number -- NVARCHAR(10) NOT NULL,
						, od.item_number AS item_number -- NVARCHAR(60) NOT NULL,
						, im.display_item_number AS display_item_number -- NVARCHAR(30) NULL,
						, im.ean AS ean -- NVARCHAR(20) NULL,
						, im.isbn AS isbn -- NVARCHAR(20) NULL,
						, im.item_description AS item_description_full -- NVARCHAR(200) NULL,
						, NULL AS po_number -- NVARCHAR(30) NULL,
						, NULL AS display_po_number -- NVARCHAR(30) NULL,
						, NULL AS po_line_number -- NVARCHAR(10) NULL,
						, od.ref_po_number AS ref_po_number -- NVARCHAR(30) NULL,
						, od.ref_po_line_number AS ref_po_line_number -- NVARCHAR(10) NULL,
						, od.order_number AS order_number -- NVARCHAR(30) NULL,
						, om.display_order_number AS display_order_number -- NVARCHAR(30) NULL,
						, od.line_number AS order_line_number -- NVARCHAR(10) NULL,
						, od.ref_order_number AS ref_order_number -- NVARCHAR(30) NULL,
						, od.ref_order_line_number AS ref_order_line_number -- NVARCHAR(10) NULL,
						, od.qty AS qty -- FLOAT NULL,
						, 0 AS qty_picked -- FLOAT NULL,
						, IIF(od.item_number IN (N'ESL-2000000023007', N'ESL-2000000029009'), od.qty, 0) AS qty_packed -- FLOAT NULL, -- 排除運費及處理費
						, 0 AS qty_unpack -- FLOAT NULL,
						, 0 AS qty_shipped -- FLOAT NULL,
						, NULL AS pick_location -- NVARCHAR(50) NULL,
						, NULL AS comment -- NVARCHAR(2000) NULL,
					FROM
						_tv_order_detail AS od WITH (NOLOCK)
						INNER JOIN _tv_order_master AS om WITH (NOLOCK)
							ON om.wh_id = od.wh_id
							AND om.order_number = od.order_number
						INNER JOIN _tv_item_master AS im WITH (NOLOCK)
							ON im.wh_id = od.wh_id
							AND im.item_number = od.item_number
					WHERE
						od.wh_id = @wh_id
						AND od.order_number = @order_number
				END -- @carton_seq = 1
				ELSE
				---- Not the 1st carton of the order
				BEGIN
					---- Insert carton detail from last carton's unpacked detail
					INSERT INTO
						_t_order_carton_detail
						(
							--id -- BIGINT IDENTITY(1,1) NOT NULL,
							wh_id -- NVARCHAR(10) NOT NULL,
							, carton_number -- NVARCHAR(50) NOT NULL,
							, client_code -- NVARCHAR(30) NOT NULL,
							, erp_site -- NVARCHAR(15) NOT NULL,
							, line_number -- NVARCHAR(10) NOT NULL,
							, item_number -- NVARCHAR(60) NOT NULL,
							, display_item_number -- NVARCHAR(30) NULL,
							, ean -- NVARCHAR(20) NULL,
							, isbn -- NVARCHAR(20) NULL,
							, item_description_full -- NVARCHAR(200) NULL,
							, po_number -- NVARCHAR(30) NULL,
							, po_line_number -- NVARCHAR(10) NULL,
							, ref_po_number-- NVARCHAR(30) NULL,
							, ref_po_line_number -- NVARCHAR(10) NULL,
							, order_number -- NVARCHAR(30) NULL,
							, display_order_number -- NVARCHAR(30) NULL,
							, order_line_number -- NVARCHAR(10) NULL,
							, ref_order_number-- NVARCHAR(30) NULL,
							, ref_order_line_number -- NVARCHAR(10) NULL,
							, qty -- FLOAT NULL,
							, qty_picked -- FLOAT NULL,
							, qty_packed -- FLOAT NULL,
							, qty_unpack -- FLOAT NULL,
							, qty_shipped -- FLOAT NULL,
							, pick_location -- NVARCHAR(50) NULL,
							, create_dt -- DATETIME
							, update_dt -- DATETIME
							, comment -- NVARCHAR(2000) NULL,
						)
					SELECT
						--id -- BIGINT IDENTITY(1,1) NOT NULL,
						@wh_id AS wh_id-- NVARCHAR(10) NOT NULL,
						, @carton_number AS carton_number -- NVARCHAR(50) NOT NULL,
						, client_code -- NVARCHAR(30) NOT NULL,
						, erp_site -- NVARCHAR(15) NOT NULL,
						, FORMAT(ROW_NUMBER() OVER(ORDER BY ctnd.order_number, ctnd.line_number), N'0000000000') AS line_number -- NVARCHAR(10) NOT NULL,
						, item_number -- NVARCHAR(60) NOT NULL,
						, display_item_number -- NVARCHAR(30) NULL,
						, ean -- NVARCHAR(20) NULL,
						, isbn -- NVARCHAR(20) NULL,
						, item_description_full -- NVARCHAR(200) NULL,
						, po_number -- NVARCHAR(30) NULL,
						, po_line_number -- NVARCHAR(10) NULL,
						, ref_po_number-- NVARCHAR(30) NULL,
						, ref_po_line_number -- NVARCHAR(10) NULL,
						, order_number -- NVARCHAR(30) NULL,
						, display_order_number -- NVARCHAR(30) NULL,
						, order_line_number -- NVARCHAR(10) NULL,
						, ref_order_number-- NVARCHAR(30) NULL,
						, ref_order_line_number -- NVARCHAR(10) NULL,
						, qty - qty_packed AS qty -- FLOAT NULL,
						, qty_picked - qty_packed -- FLOAT NULL,
						, 0 AS qty_packed -- FLOAT NULL,
						, qty_unpack -- FLOAT NULL,
						, qty_shipped -- FLOAT NULL,
						, pick_location -- NVARCHAR(50) NULL,
						, @dt AS create_dt -- DATETIME
						, @dt AS update_dt -- DATETIME
						, comment -- NVARCHAR(2000) NULL,
					FROM
						_t_order_carton_detail AS ctnd WITH (NOLOCK)
					WHERE
						ctnd.wh_id = @wh_id
						AND ctnd.order_number = @order_number
						AND ctnd.carton_number = @last_carton_number
						AND ctnd.qty_packed < ctnd.qty

					---- Clear last carton's unpacked detail
					DELETE
						ctnd
					FROM
						_t_order_carton_detail AS ctnd
					WHERE
						ctnd.wh_id = @wh_id
						AND ctnd.order_number = @order_number
						AND ctnd.carton_number = @last_carton_number
						AND ctnd.qty_packed = 0

					---- Update last carton's partial packed detail rows
					UPDATE
						ctnd
					SET
						qty = qty_packed
						, qty_picked = qty_packed
						, update_dt = @dt
					FROM
						_t_order_carton_detail AS ctnd
					WHERE
						ctnd.wh_id = @wh_id
						AND ctnd.order_number = @order_number
						AND ctnd.carton_number = @last_carton_number
						AND ctnd.qty_packed < ctnd.qty

					---- Close last carton
					UPDATE
						ctnm
					SET
						ctnm.total_items = (SELECT COUNT(DISTINCT item_number) FROM _t_order_carton_detail WITH (NOLOCK) WHERE wh_id = @wh_id AND carton_number = @last_carton_number)
						, ctnm.total_qty = (SELECT ISNULL(SUM(ISNULL(qty, 0)), 0) FROM _t_order_carton_detail WITH (NOLOCK) WHERE wh_id = @wh_id AND carton_number = @last_carton_number)
						, ctnm.last_carton_flag = N'N' -- Not the last carton
                        --, ctnm.status = N'Q' -- Q:Packed
						, ctnm.status = N'R' -- Staged 20221022_ken
						, ctnm.update_dt = @dt
					FROM
						_t_order_carton_master AS ctnm
					WHERE
						ctnm.wh_id = @wh_id
						AND ctnm.order_number = @order_number
						AND ctnm.carton_number = @last_carton_number
				END

				---- Commit transaction
				COMMIT;

-- >>>>> By Steve, 2022/07/11 -> Auto-print waybill and ship list of the closed carton
				---- Not the first carton
				IF @carton_seq > 1
				BEGIN
					---- Auto-print waybill
					SELECT TOP 1
						@report_name = 
							CASE
								WHEN (erp_site = N'G016') AND (customer_code <> N'T-CAT') THEN N'HJ_WBL_' + ctnm.carton_type + N'_' + REPLACE(UPPER(ctnm.customer_code), '-', '') + N'_SP' 
								WHEN customer_code = N'FedEx' THEN N'HJ_WBL_' + ctnm.carton_type + N'_' + REPLACE(UPPER(ctnm.customer_code), '-', '')
								ELSE N'HJ_WBL_' + ctnm.carton_type + N'_' + REPLACE(UPPER(ctnm.customer_code), '-', '')
							END
						, @report_params = (SELECT ctnm.wh_id AS 'wh_id', ctnm.carton_number AS 'carton_number' FOR JSON PATH)
						--, @note_message = IIF(erp_site = N'G016', N'★★★★★ 蝦皮訂單，請列印發票 ★★★★★', N'')
					FROM 
						_t_order_carton_master AS ctnm WITH (NOLOCK)
					WHERE 
						wh_id = @wh_id 
						AND carton_number = @last_carton_number

					EXEC _sp_auto_print
						@wh_id								= @wh_id
						, @report_name						= @report_name
						, @report_params					= @report_params
						--
						, @user_id							= @user_id
						--, @rtn_code						= @rtn_code OUTPUT
						--, @rtn_message					= @rtn_message OUTPUT
						
					---- Wait for 1 seconds to secure the correct print-out sequence.
					WAITFOR DELAY '00:00:01';

					---- Auto-print ship list
					SELECT TOP 1
						@report_name = 
							CASE
								WHEN customer_code = N'FedEx' THEN N'HJ_RPT_ShipList_EC_FEDEX'
								ELSE N'HJ_RPT_ShipList_EC'
							END
						, @report_params = (SELECT ctnm.display_order_number AS 'display_order_number', ctnm.carton_number AS 'carton_number' FOR JSON PATH)
					FROM 
						_t_order_carton_master AS ctnm WITH (NOLOCK)
					WHERE 
						wh_id = @wh_id 
						AND carton_number = @last_carton_number

					EXEC _sp_auto_print
						@wh_id								= @wh_id
						, @report_name						= @report_name
						, @report_params					= @report_params
						--
						, @user_id							= @user_id
						--, @rtn_code						= @rtn_code OUTPUT
						--, @rtn_message					= @rtn_message OUTPUT			
				END
-- <<<<<

				BREAK; -- WHILE @retry_count > 0
			END TRY
			BEGIN CATCH
				---- Get Error Informatons
				SET @error_number = ERROR_NUMBER();
				SET @rtn_code = N'FAIL';
				SET @rtn_message = ERROR_MESSAGE() + NCHAR(10) + N'(' + @sp_name + N', ' + CONVERT(NVARCHAR(10), @error_number) + N')';

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

					---- Wait for 2 seconds and retry
					WAITFOR DELAY '00:00:02';
				END -- IF (@retry_count > 0 AND @error_number = 1205)
				ELSE
					BREAK; -- WHILE @retry_count > 0
			END CATCH

			IF @debug = 1
				PRINT N'[' + @sp_name + N']:' +  NCHAR(10)
					+ N'@rtn_code: ' + ISNULL(@rtn_code, N'(null)') + NCHAR(10)
					+ N'@rtn_message: ' + ISNULL(@rtn_message, N'(null)') + NCHAR(10)

		END -- WHILE @retry_count > 0
	END TRY
	BEGIN CATCH
		---- Get Error Informatons
		SET @error_number = ERROR_NUMBER();
		SET @rtn_code = N'FAIL';
		SET @rtn_message = ERROR_MESSAGE() + NCHAR(10) + N'(' + @sp_name + N', ' + CONVERT(NVARCHAR(10), @error_number) + N')';
	END CATCH

	IF @debug = 1
		PRINT N'[' + @sp_name + N']:' +  NCHAR(10)
			+ N'@rtn_code: ' + ISNULL(@rtn_code, N'(null)') + NCHAR(10)
			+ N'@rtn_message: ' + ISNULL(@rtn_message, N'(null)') + NCHAR(10)

	SET @ww_result = @rtn_code;
	RETURN;
END;

/***
SELECT status, * FROM _tv_order_master WITH (NOLOCK) WHERE order_number='ESL-202110040536201'
SELECT * FROM _tv_order_detail WITH (NOLOCK) WHERE order_number='ESL-202110040536201'
SELECT * FROM _t_order_carton_master WITH (NOLOCK) WHERE order_number='ESL-202110040536201'
--DELETE _t_order_carton_master WHERE order_number='ESL-0802421772'
SELECT * FROM _t_order_carton_detail WITH (NOLOCK) WHERE order_number='ESL-202110040536201' ORDER BY carton_number, line_number
--DELETE _t_order_carton_detail WHERE order_number='ESL-0802421772'
--UPDATE _t_order_carton_detail SET qty_packed = qty_packed + 1 WHERE carton_number='ESL-0802421772' AND line_number < '0000000011'
SELECT * FROM _t_wave_batch WHERE order_number='ESL-0802421772'

DECLARE	@rtn_code NVARCHAR(10), @rtn_message NVARCHAR(500), @carton_number NVARCHAR(50)
EXEC _sp_add_carton_by_order
	@wh_id								= N'pz1'
	, @order_number						= N'ESL-220124NBBHRSTS'
	, @carton_number					= @carton_number OUTPUT
	--
	, @user_id							= N'steve'
	, @rtn_code							= @rtn_code OUTPUT
	, @rtn_message						= @rtn_message OUTPUT
***/
