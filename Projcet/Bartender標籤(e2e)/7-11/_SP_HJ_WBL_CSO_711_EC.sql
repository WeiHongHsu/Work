USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_SP_HJ_WBL_CSO_711_EC]    Script Date: 8/17/2023 11:30:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
-- Author:		JackyHsu
-- Create date: 2023/07/21
-- Description: 7-11本站標籤列印
DECLARE
@rtn_code NVARCHAR(10)   ,
@rtn_message NVARCHAR(4000)  ,
@ww_result NVARCHAR(10)
--exec _SP_HJ_WBL_CSO_711_EC 'pz1','C001','ESL-202308159178401','JackyHsu','','',@rtn_code out,@rtn_message out,@ww_result out,'','',''
 exec _SP_HJ_AutoPrinter 'Shippinglabel','7-11','STD','PZ1','C001','ESL-202308151568701','JackyHsu','','',@rtn_code out,@rtn_message out,@ww_result out,'','',''
 select @rtn_code,@rtn_message,@ww_result

 ============================================= */

ALTER PROCEDURE [dbo].[_SP_HJ_WBL_CSO_711_EC] 
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
--,@Err_Msg nvarchar(4000)
,@reportname nvarchar(100) = 'HJ_WBL_CSO_711' --報表名稱
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

Declare 
 @Json_Result nvarchar(2000) = ''
,@col01 nvarchar(400)
,@col02 nvarchar(400)
,@col03 nvarchar(400)
,@col04 nvarchar(400)
,@col05 nvarchar(400)
,@col06 nvarchar(400)
,@col07 nvarchar(400)
,@col08 nvarchar(400)


	SELECT 
		  @col01 = Case when ctnm.cod_flag  = 'Y' then N'*不用檢視證件*' else N'*商品已付款，請檢視證件*' end  --as cod_flag
		, @col02 = left(ctnm.ship_to_name,5) --as ship_to_name
		, @col03 = '785' + ctnm.waybill_number --as waybill_number
		, @col04 = ltrim(rtrim((ctnm.ship_to_description)))+right(ctnm.ship_to_code,6) --as Shop_name
		, @col05 = right(ctnm.ship_to_code,6) + '785' + '001' + ctnm.waybill_number --as Tracking_No
		, @col06 = convert(char(10),Dateadd(D,1,ctnm.create_dt),111) --as order_date
		, @col07 = convert(char(10),Dateadd(D,8,ctnm.create_dt),111)  --as actual_delivery_date
		, @col08 = ctnm.display_order_number 

		, @printer_id = pm.printer_id
		, @FilePath_bak = PM.FilePath_bak + Replace(cl.UDF03,'*',ctnm.display_order_number +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')) --as FilePath_bak
		, @BTW_Label = Pm.BTW_Label

		, @INOUT = cl2.short
		, @headers = cl2.long
		, @ApiURL = cl2.UDF01 --主要
		, @ApiURL2 = cl2.UDF02 --備援
		, @workstationid = Pm.workstation_id
	FROM
		_t_order_carton_master AS ctnm WITH (NOLOCK)
		LEFT JOIN _tv_order_master AS om WITH (NOLOCK)
		ON om.wh_id = ctnm.wh_id
		AND om.order_number = ctnm.order_number
		and om.erp_site = 'C001'
		left join _t_Codelkup as cl with (nolocK)
		on ctnm.erp_site = cl.storerkey
		and om.customer_code = cl.code
		and cl.listname = @Module
		left join #printer_mapping as pm with (nolocK) 
		on pm.report_name = cl.udf05 
		left join _t_Codelkup as cl2 with (nolocK)
		on ctnm.erp_site = cl2.storerkey
		and om.customer_code = cl2.code
		and cl2.listname = 'BTWAPIConfig' 

		WHERE
		ctnm.wh_id = @wh_id
		and ctnm.erp_site = @storerkey
		AND ctnm.carton_number = @carton_number



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
		set @rtn_code = N'FAIL' --JH01
		set @rtn_message = N'_t_Codelkup.listname = BTWAPIConfig 設定未完成，請確認是否設定。'
		goto LOGMSG

end
	

	/*設定API參數*/

	SET @Json_Result = (
	  SELECT 
		@col01 as cod_flag
	   ,@col02 as ship_to_name
	   ,@col03 as waybill_number
	   ,@col04 as Shop_name
	   ,@col05 as Tracking_No
	   ,@col06 as order_date
	   ,@col07 as actual_delivery_date
	   ,@col08 as display_order_number
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
	--Exec [INT].iexp.dbo.SP_GetECinfor_TEST
	Exec [192.168.1.52].iexp.dbo.SP_GetECinfor_711

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
