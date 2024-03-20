USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_SP_HJ_WBL_CSO_REY_EC]    Script Date: 8/17/2023 12:00:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* =============================================
-- Author:		JackyHsu
-- Create date: 2023/01/29
-- Description: 全家本站標籤列印
-- Test: exec _SP_HJ_WBL_CSO_REY 'pz1','C001','ESL-202204259478501','JackyHsu'
-- Test: exec _SP_HJ_AutoPrinter 'Shippinglabel','REY','STD','C001','PZ1','ESL-202204259478501','JackyHsu'

	DECLARE
	@rtn_code NVARCHAR(10)   ,
	@rtn_message NVARCHAR(4000)  ,
	@ww_result NVARCHAR(10)
	exec _SP_HJ_WBL_CSO_REY 'pz1','C001','ESL-202202157986201','JackyHsu','','',@rtn_code out,@rtn_message out,@ww_result out,'','',''
	--exec _SP_HJ_AutoPrinter 'Shippinglabel','REY','STD','PZ1','C001','ESL-202202157986201','JackyHsu','','',@rtn_code out,@rtn_message out,@ww_result out,'','',''
	select @rtn_code,@rtn_message,@ww_result

=============================================*/
ALTER PROCEDURE [dbo].[_SP_HJ_WBL_CSO_REY_EC] 
	@wh_id nvarchar(20),
	@storerkey  nvarchar(20),
	@carton_number nvarchar(50) ,
	@userid nvarchar(50) ,	

	@Sdate datetime , --未使用
	@Edate datetime , --未使用
	/*Page Editor固定參數*/
	@rtn_code         NVARCHAR(10)  OUTPUT ,
	@rtn_message     NVARCHAR(4000) OUTPUT ,
	@ww_result         NVARCHAR(10) OUTPUT ,
	/*預留參數*/
	@Parameter1 nvarchar(200) , --未使用
	@Parameter2 Nvarchar(200) , --未使用
	@Parameter3 Nvarchar(200)  --未使用

AS
BEGIN

	SET NOCOUNT ON;


DECLARE 
 @Body NVARCHAR(4000) 
,@INOUT NVARCHAR(10)
,@ApiURL NVARCHAR(400)  --主服務
,@ApiURL2 NVARCHAR(400) --備援服務
,@headers NVARCHAR(400) 
,@workstationid nvarchar(50)
,@status int
,@printer_id nvarchar(400)
,@FilePath_bak nvarchar(400)
,@BTW_Label nvarchar(400)
,@reportname nvarchar(100) = 'HJ_WBL_CSO_REY' --報表名稱
/*LOG*/
,@Module nvarchar(100) = 'Shippinglabel'
,@SPName nvarchar(50) = OBJECT_NAME(@@PROCID)


 --Check Connection   
 IF ISNULL([dbo].[_fn_get_hj1_logon_ip](@userid),'')=''  
 BEGIN  
	PRINT 'You have been Disconnected. Please try to re-login HJ.' 
	set @rtn_code  = 'FAIL' --JH01
	set @rtn_message = N'You have been Disconnected. Please try to re-login HJ.' 
	goto LOGMSG
 END  

 

	/*_t_printer_mapping 設定抓取*/
	IF OBJECT_ID('tempdb.dbo.#printer_mapping') IS NOT NULL
	BEGIN
		DROP TABLE #printer_mapping
	END

	select 
	workstation_id , 
	report_name,
	printer_id,
	FilePath_bak,
	BTW_Label 
	into #printer_mapping
	from _t_printer_mapping as pm with (nolocK)
	inner join _t_device as TD with (nolocK)
	on TD.wh_id = @wh_id
	and TD.device_type = 'PC'
	and TD.device_ip = [dbo].[_fn_get_hj1_logon_ip](@userid)
	and TD.device_id = PM.workstation_id
	where pm.report_name= @reportname

	if @@ROWCOUNT = 0
		begin 
			Print N'_t_printer_mapping 設定未完成，請確認設定資訊。'
			set @rtn_code  = 'FAIL' --JH01
			set @rtn_message = N'_t_printer_mapping 設定未完成，請確認設定資訊。'
			goto LOGMSG
		end


/*JSON*/
Declare @Json_Result nvarchar(2000) = ''
,@col01 nvarchar(400)
,@col02 nvarchar(400)
,@col03 nvarchar(400)
,@col04 nvarchar(400)
,@col05 nvarchar(400)
,@col06 nvarchar(400)
,@col07 nvarchar(400)
,@col08 nvarchar(400)
,@col09 nvarchar(400)
,@col10 nvarchar(400)
,@col11 nvarchar(400)
,@col12 nvarchar(400)
,@col13 nvarchar(400)
,@col14 nvarchar(400)





		/*資料內容*/
		---- GL Barcode
		DECLARE
			@channel_code		NVARCHAR(10)
			, @rchannel_code		NVARCHAR(10)
			, @ship_to_code		NVARCHAR(10)
			, @route_seq		NVARCHAR(10)
			, @equipment_id		NVARCHAR(10)
			, @region			NVARCHAR(40)
			, @barcode_gl		NVARCHAR(30)
			, @barcode_1		NVARCHAR(30)
			, @barcode_2		NVARCHAR(30)
			, @barcode_qr		NVARCHAR(600)
			, @route_code_2		NVARCHAR(10)


		SELECT TOP 1
			@channel_code =
			CASE
				WHEN ctnm.customer_code = N'REY' THEN N'1'
				ELSE N''
			END
			, @rchannel_code =
			CASE
				WHEN ctnm.customer_code = N'REY' THEN N'2'
				ELSE N''
			END
			, @ship_to_code = ctnm.ship_to_code
			, @route_seq = LEFT(cs.route_code, 2) + RIGHT(cs.route_code_2, 2)
			, @route_code_2 = left(RIGHT(cs.route_code_2, 2),1)
			, @equipment_id = cs.equipment_id
			, @region =
			CASE
				WHEN cs.region = N'01' THEN N'北'
				WHEN cs.region = N'02' THEN N'中'
				WHEN cs.region = N'03' THEN N'南'
				ELSE cs.region
			END
			, @barcode_gl = 
				N'281' 
				+ N'00' 
				+ RIGHT(REPLICATE(N'0', 11) + ctnm.waybill_number, 11)
			, @barcode_1 = 
				N'281' 
				+ LEFT(RIGHT(REPLICATE(N'0', 11) + ctnm.waybill_number, 11), 3) 
				+ N'762'
			, @barcode_2 = 
				RIGHT(REPLICATE(N'0', 11) + ctnm.waybill_number, 8) 
				+ IIF(ctnm.cod_flag = N'Y', N'1', N'3')
				+ RIGHT(REPLICATE(N'0', 5) + CONVERT(NVARCHAR(5), ctnm.cod_amount), 5)
		FROM
			_t_order_carton_master AS ctnm WITH (NOLOCK)
			LEFT JOIN _v_customer_site AS cs WITH (NOLOCK)
				ON cs.customer_code = ctnm.customer_code
				AND cs.site_code = ctnm.ship_to_code
			WHERE
			ctnm.wh_id = @wh_id
			and ctnm.erp_site = @storerkey
			AND ctnm.carton_number = @carton_number
	

	   
		SET @barcode_gl = @channel_code + @barcode_gl + dbo._fn_get_cvs_barcode_checksum(N'GL', @channel_code + @barcode_gl)

		SET @barcode_2 = @barcode_2 + dbo._fn_get_cvs_barcode_checksum(N'2', @barcode_1 + N'0' + @barcode_2)

		SELECT @barcode_qr = 
			N'B1'
			+ N'||' + REPLICATE(N' ', 18)
			+ N'||' + REPLICATE(N' ', 9)
			+ N'||' + REPLICATE(N' ', 18)
			+ N'||' + REPLICATE(N' ', 15) 
			+ N'||' + @barcode_gl
			+ N'||' + @rchannel_code
			+ N'||' + RIGHT(@ship_to_code, 6)
			+ N'||' + @channel_code + @route_seq
			+ N'||' + right('  '+ ltrim(@equipment_id),2)
			+ N'||' + N'0'
			+ N'||' + @barcode_1
			+ N'||' + @barcode_2
			+ N'||' + REPLICATE(N' ', 13)
			+ N'||' + REPLICATE(N' ', 10)

		SELECT 
			  @col01 = ctnm.display_order_number
			, @col02 = ctnm.waybill_number
			, @col03 = case when ctnm.cod_flag = 'Y' then N'請收款結帳' else N'請核對證件' end -- as cod_flag
			, @col04 = right(rtrim(ctnm.ship_to_phone),3)  --as ship_to_phone
			, @col05 = ctnm.ship_to_name
			, @col06 = ctnm.ship_to_description
			, @col07 = @route_seq --AS route_seq
			, @col08 = @region --AS region
			, @col09 = @channel_code --as channel_code
			, @col10 = @barcode_1 --AS barcode_1
			, @col11 = @barcode_2 --AS barcode_2
			, @col12 = @barcode_qr --AS barcode_qr
			, @col13 = case when cast(right( rtrim(@route_seq),2) as int) % 2 = 0 then 'Y' else 'N' end --Black_Flag
			, @col14 = @equipment_id
			, @printer_id = pm.printer_id
			, @FilePath_bak = PM.FilePath_bak + Replace(cl.UDF03,'*',ctnm.display_order_number +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')) -- as FilePath_bak
			, @BTW_Label = Pm.BTW_Label
			, @INOUT = cl2.short
			, @headers = cl2.long
			, @ApiURL = cl2.UDF01
			, @ApiURL2 = cl2.UDF02
			, @workstationid = pm.workstation_id



		FROM
			_t_order_carton_master AS ctnm WITH (NOLOCK)
			inner join aad.dbo._t_Codelkup as cl with (nolocK)
			on ctnm.erp_site = cl.storerkey
			and ctnm.customer_code = cl.code
			and ctnm.customer_code = 'REY'
			and cl.listname = @Module
			left join #printer_mapping as pm with (nolocK) 
			on pm.report_name = cl.udf05 
			left join _t_Codelkup as cl2 with (nolocK)
			on ctnm.erp_site = cl2.storerkey
			and ctnm.customer_code = cl2.code
			and cl2.listname = 'BTWAPIConfig' 
		WHERE
			ctnm.wh_id = @wh_id
			and ctnm.erp_site = @storerkey
			AND ctnm.carton_number = @carton_number

/*資料內容*/

/*_t_printer_mapping and _t_Codelkup.listname = Shippinglabel設定檢核*/
if (isnull(@FilePath_bak,'') = '' ) 
	begin 
		
		select @FilePath_bak
		Print N'_t_Codelkup.listname = Shippinglabel 設定未完成，請確認是否設定。'
		set @rtn_code  = 'FAIL' --JH01
		set @rtn_message = N'_t_Codelkup.listname = Shippinglabel 設定未完成，請確認是否設定。'
		goto LOGMSG

	end

/*_t_Codelkup.listname = BTWAPIConfig 設定檢核*/
if (isnull(@INOUT,'') = '' or isnull(@headers,'') = '' or isnull(@ApiURL,'') = '' or isnull(@ApiURL2,'') = '' )
begin 
		select @INOUT,@headers,@ApiURL,@ApiURL2
		Print N'_t_Codelkup.listname = BTWAPIConfig 設定未完成，請確認是否設定。'
		set @rtn_code  = 'FAIL' --JH01
		set @rtn_message = N'_t_Codelkup.listname = BTWAPIConfig 設定未完成，請確認是否設定。'
		goto LOGMSG

end



	SET @Json_Result = (
	  SELECT 
		@col01 as display_order_number
	   ,@col02 as waybill_number
	   ,@col03 as cod_flag
	   ,@col04 as ship_to_phone
	   ,@col05 as ship_to_name
	   ,@col06 as ship_to_description
	   ,@col07 as route_seq
	   ,@col08 as region
	   ,@col09 as channel_code
	   ,@col10 as barcode_1
	   ,@col11 as barcode_2
	   ,@col12 as barcode_qr
	   ,@col13 as Black_Flag
	   ,@col14 as equipment_id
	   ,@printer_id as printer_id
	   ,@FilePath_bak as FilePath_bak
	   ,@BTW_Label as BTW_Label
   
	  FOR JSON PATH -- 蝦皮          
	)
	select @Json_Result


	/*呼叫EC API接收個資 Start 待修改成REY*/
	Declare @outputJson as NVARCHAR(4000)--資料可能大於資料量
	Declare @apiStatus as INT
	Declare @apiMessage as NVARCHAR(500)
	Exec [INT].iexp.dbo.SP_GetECinfor_TEST 
	 @Json_Result --產生Json
	,@outputJson output --覆蓋個資後Json
	,@apiStatus output --API 狀態
	,@apiMessage output --Msg
	--print @outputJson
	--print @apiStatus
	--print @apiMessage

	if @apiStatus <> '200'
	begin
		set @rtn_code  = 'FAIL'
		set @rtn_message =  N' EC API 異常，請與EC Team聯繫。' + char(13)+char(10) + @apiMessage
		goto LOGMSG
	end
	select @outputJson as outputJson
	set @Json_Result = @outputJson

	/*呼叫EC API接收個資 End*/



	/*JSON*/

	select @Body = @Json_Result

	select @INOUT,@ApiURL ,@Headers,@Body 


	select @Body = @Json_Result

	select @INOUT,@ApiURL ,@Headers,@Body,@userid,@workstationid,@FilePath_bak

	DECLARE @LogFilename nvarchar(100)

	Set @LogFilename = @FilePath_bak

	/****Bartender自動列印API Start****/
	/*主要API呼叫*/
	exec _sp_BartenderAPI @INOUT,@ApiURL ,@Headers,@Body ,@userid,@workstationid,@LogFilename,1,@status out ,@rtn_message out

	if @status <> 200
		begin 
		
			set @rtn_code  = 'FAIL'

			select @INOUT,@ApiURL2 ,@Headers,@Body ,@userid,@workstationid,@LogFilename,1,@rtn_code
			/*備援API呼叫*/
			exec _sp_BartenderAPI @INOUT,@ApiURL2 ,@Headers,@Body ,@userid,@workstationid,@LogFilename,1,@status out ,@rtn_message out
			if @status = 200
				begin
					/*JH01*/
					if CHARINDEX(@rtn_message,'Response="OK"')>0
						begin 
							set @rtn_code  = 'PASS'	
						end 
					else
						begin 
							set @rtn_code  = 'FAIL'	
						end
					/*JH01*/
					Print N'Bartender Server(192.168.1.100) 服務異常，請檢查主機狀況。'
					set @rtn_message = @rtn_message + N'Bartender Server(192.168.1.100) 服務異常，請檢查主機狀況。'
					goto LOGMSG

				end
			else
				begin
					set @rtn_code  = 'FAIL' --JH01
					Print N'備援Bartender Server(192.168.1.104) 服務異常，請檢查主機狀況。'
					set @rtn_message = @rtn_message + N'備援Bartender Server(192.168.1.104) 服務異常，請檢查主機狀況。' --JH01
					goto LOGMSG
				end

		end
		/*JH01*/
		else
		begin

			if CHARINDEX('Response="OK"',@rtn_message) > 0
				begin 
					set @rtn_code  = 'PASS'	
				end 
			else
				begin 
					set @rtn_code  = 'FAIL'	
				end
		end
		/*JH01*/

	set @ww_result = @rtn_code
	/****Bartender自動列印API End****/


/*****Log紀錄 Start*****/
LOGMSG:
	if isnull(@rtn_message,'') <> '' --JH01
		begin 

			set @ww_result = @rtn_code --JH01

			exec [AAD].dbo._sp_logerror 
			   @Module --[Module]
			  ,@SPName --[SPName]
			  ,@userid --[USERID]
			  ,N'' --[ERRORID]
			  ,@rtn_message --[ErrorMsg]
			  ,@storerkey --[Storerkey]
			  ,@wh_id --[Key1]
			  ,@carton_number --[Key2]
			  ,@reportname --[Key3]
			  ,N'' --[Key4]
			  ,N'' --[Key5]
		end
/*****Log紀錄 END*****/


END
