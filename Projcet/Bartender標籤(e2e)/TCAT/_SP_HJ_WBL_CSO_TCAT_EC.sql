USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_SP_HJ_WBL_CSO_TCAT_EC]    Script Date: 8/29/2023 3:03:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
-- Author:		JackyHsu
-- Create date: 2023/07/06
-- Description: 黑貓標籤列印

	DECLARE
	@rtn_code NVARCHAR(10)   ,
	@rtn_message NVARCHAR(4000)  ,
	@ww_result NVARCHAR(10)
	exec _SP_HJ_WBL_CSO_TCAT_EC 
		 'pz1'  --倉別
		,'C001' --貨主
		,'ESL-202308299203701' --單號
		,'JackyHsu' --使用者帳號
		,'' --日期參數(未使用)
		,'' --日期參數(未使用)
		,@rtn_code out --Error Flag
		,@rtn_message out -- Error Msg
		,@ww_result out --Error Flag
		,'' --預留參數(未使用)
		,'' --預留參數(未使用)
		,'' --預留參數(未使用)

 --exec _SP_HJ_AutoPrinter 'Shippinglabel','t-cat','STD','PZ1','G016','ESL-2301113TJ2KY9X','JackyHsu','','',@rtn_code out,@rtn_message out,@ww_result out,'','',''
select @rtn_code,@rtn_message,@ww_result

 select * 
 from _t_order_carton_master (nolocK) 
 where erp_site = 'C001' and customer_code = 't-Cat'
 and create_dt >= '20221201'
  
 =============================================*/
ALTER PROCEDURE [dbo].[_SP_HJ_WBL_CSO_TCAT_EC] 
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
--,@rtn_message nvarchar(4000)
,@reportname nvarchar(100) = 'HJ_WBL_CSO_TCAT' --報表名稱
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


Declare  @Json_Result nvarchar(2000) = ''
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
		,@col15 nvarchar(400)
		,@col16 nvarchar(400)
		,@col17 nvarchar(400)
		,@col18 nvarchar(400)
		,@col19 nvarchar(400)
		,@col20 nvarchar(400)


DECLARE	
	  @zip_7 NVARCHAR(20) = ''
	, @db_version NVARCHAR(20) = ''


/*20230704 e2e專案取消呼叫API 改由EC Team提供 Start*/
--		SELECT TOP 1
--			@zip_7 = JSON_VALUE(waybill_data, '$[0].zip_7')
--			, @db_version = JSON_VALUE(waybill_data, '$[0].db_version')
--		FROM
--			_t_order_carton_master WITH (NOLOCK)
--		WHERE
--			wh_id = @wh_id
--			AND carton_number = @carton_number
--			and erp_site = @storerkey
/*20230704 e2e專案取消呼叫API 改由EC Team提供 End*/

		SELECT TOP 1
			  @col01 = ctnm.wh_id
			, @col02 = ctnm.carton_number
			, @col03 = ctnm.carton_number --AS carton_label
			, @col04 = ctnm.waybill_number
			, @col05 = '*' + ctnm.waybill_number + '*' --AS waybill_number_barcode
			, @col06 = ctnm.order_number
			, @col07 = '*' + ctnm.order_number + '*' --AS order_number_barcode
			, @col08 = ctnm.display_order_number
			, @col09 = ctnm.ship_to_name
			, @col10 = ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address --AS ship_to_address
			, @col11 = ctnm.ship_to_phone
			, @col12 = REPLACE(CONVERT(NVARCHAR(20), GETDATE(), 111), '/', '-') --AS ship_date
			, @col13 = REPLACE(CONVERT(NVARCHAR(20), GETDATE() + 1, 111), '/', '-') --AS delivery_date
			, @col14 = case when ctnm.cod_flag = 'Y' then cast (ctnm.cod_amount as nvarchar) + N'元' else N'不收款' end --cod_flag
			, @col15 = cast (DATEPART(mm, DATEADD(d,2,GETDATE())) as nvarchar) + N'月' + cast (DATEPART(DD, DATEADD(d,2,GETDATE())) as nvarchar) + N'日' --AS hope_delivery_date
			, @col16 = JSON_VALUE(waybill_data, '$[0].zip_7') --AS zip_7
			, @col17 = LEFT(REPLACE(JSON_VALUE(waybill_data, '$[0].zip_7'), '-', ''), 7) --AS zip_7_2
			, @col18 = @db_version --AS db_version
			, @col19 = N'01|' + ctnm.waybill_number +'|10|231555313100|N|0|01|01|02|' + SUBSTRING(REPLACE(@zip_7, '-', ''), 3, 10) + '|' + CONVERT(NVARCHAR(20), GETDATE() + 1, 112) + '|01||0|||||||||||' --AS qr_code
			, @col20 = Case when CHARINDEX('路',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('路',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
							when CHARINDEX('街',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('街',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
							when CHARINDEX('鄉',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('鄉',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
							when CHARINDEX('鎮',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('鎮',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
							when CHARINDEX('區',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('區',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
							when CHARINDEX('市',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('市',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
							when CHARINDEX('縣',rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)) > 0 then Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),CHARINDEX('縣',Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address)))
					 else Right('**************'+SUBSTRING(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address),10,len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))),len(Rtrim(ctnm.ship_to_city + ctnm.ship_to_state + ctnm.ship_to_address))) end

			, @printer_id = pm.printer_id
			, @FilePath_bak = PM.FilePath_bak + Replace(cl.UDF03,'*',ctnm.display_order_number +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')) --as FilePath_bak
			, @BTW_Label = Pm.BTW_Label

			, @INOUT = cl2.short
			, @headers = cl2.long
			, @ApiURL = cl2.UDF01
			, @ApiURL2 = cl2.UDF02
			, @workstationid = pm.workstation_id

		FROM
			_t_order_carton_master AS ctnm WITH (NOLOCK)
			left join _t_Codelkup as cl with (nolocK)
			on ctnm.erp_site = cl.storerkey
			and ctnm.customer_code = cl.code
			and ctnm.customer_code = 'T-CAT'
			and cl.listname = @Module 
			left join #printer_mapping as pm with (nolocK) 
			on pm.report_name = cl.udf05 
			left join _t_Codelkup as cl2 with (nolocK)
			on ctnm.erp_site = cl2.storerkey
			and ctnm.customer_code = cl2.code
			and cl2.listname = 'BTWAPIConfig' 
		WHERE
			ctnm.wh_id = @wh_id
			and erp_site = @storerkey
			AND ctnm.carton_number = @carton_number

			/*設定API參數*/


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

	/*設定API參數*/


SET @Json_Result = (
  SELECT 
    @col01 as wh_id
   ,@col02 as carton_number
   ,@col03 as carton_label
   ,@col04 as waybill_number
   ,@col05 as waybill_number_barcode
   ,@col06 as order_number
   ,@col07 as order_number_barcode
   ,@col08 as display_order_number
   ,@col09 as ship_to_name
   ,@col10 as ship_to_address
   ,@col11 as ship_to_phone
   ,@col12 as ship_date
   ,@col13 as delivery_date
   ,@col14 as cod_flag
   ,@col15 as hope_delivery_date
   ,@col16 as zip_7
   ,@col17 as zip_7_2
   ,@col18 as db_version
   ,@col19 as qr_code
   ,@col20 as ship_to_address2
   ,@printer_id as printer_id
   ,@FilePath_bak as FilePath_bak
   ,@BTW_Label as BTW_Label

  FOR JSON PATH -- 蝦皮          
)
select @Json_Result

	/*呼叫EC API接收個資 Start*/
	Declare @outputJson as NVARCHAR(4000)--資料可能大於資料量
	Declare @apiStatus as INT
	Declare @apiMessage as NVARCHAR(500)
	Exec [INT].iexp.dbo.SP_GetECinfor_TCAT 
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

	set @Json_Result = @outputJson

	/*呼叫EC API接收個資 End*/


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

