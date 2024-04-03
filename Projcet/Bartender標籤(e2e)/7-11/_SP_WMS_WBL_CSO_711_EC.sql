USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_SP_WMS_WBL_CSO_711_EC]    Script Date: 8/17/2023 12:02:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
-- Author:		JackyHsu
-- Create date: 2023/07/21
-- Description: WMS 7-11本站標籤列印
--exec _SP_WMS_WBL_CSO_711_EC 'LNWOC210009','202307219721501'

 ============================================= */

ALTER PROCEDURE [dbo].[_SP_WMS_WBL_CSO_711_EC] 
	@hostName nvarchar(30),@Externorderkey nvarchar(30)
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
,@reportname nvarchar(100) = 'HJ_WBL_CSO_711' --報表名稱
/*LOG*/
,@Module nvarchar(100) = 'Shippinglabel'
,@SPName nvarchar(50) = OBJECT_NAME(@@PROCID)
,@wh_id nvarchar(20) = 'PZ1'
,@Sdate datetime --未使用
,@Edate datetime  --未使用
 /*Page Editor固定參數*/
,@rtn_code         NVARCHAR(10)   
,@rtn_message     NVARCHAR(4000)  
,@ww_result         NVARCHAR(10)  
,@storerkey nvarchar(20)


 
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


		select  distinct 
			  @col01 = Case  when C_KEEPPY > 0 then N'不用檢視證件' else N'商品已付款，請檢視證件' end --cod_flag
			, @col02 = C_CONTACT1 --ship_to_name
			, @col03 = '785'+CASEID --waybill_number
			, @col04 = rtrim(ltrim(Company)) +' '+DOOR --Shop_name
			, @col05 = rtrim(ltrim(DOOR)) + '785' + '001' + CASEID --Tracking_No
			, @col06 = convert(char(10),dateadd(d,1,SHIPDATE),111) --order_date
			, @col07 = convert(char(10),dateadd(d,8,SHIPDATE),111) --actual_delivery_date
			, @col08 = EXTERNORDERKEY --display_order_number

			, @printer_id = pm.printer_id
			, @FilePath_bak = PM.FilePath_bak + Replace(cl.UDF03,'*',o.EXTERNORDERKEY COLLATE Chinese_Taiwan_Stroke_CI_AS +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')) --as FilePath_bak
			, @BTW_Label = Pm.BTW_Label

			, @INOUT = cl2.short
			, @headers = cl2.long
			, @ApiURL = cl2.UDF01
			, @ApiURL2 = cl2.UDF02
			, @workstationid = pm.workstation_id
			, @storerkey = Case when o.storerkey = 'DBE1' then 'C001' else o.storerkey end 

		FROM [192.168.1.11].[PROD].[dbo].[Vw_Case_Addr_Invoice] as O with (nolock)
		left join aad.dbo._t_Codelkup as cl with (nolocK)
		on (case when o.storerkey = 'DBE1' then 'C001' else o.storerkey end COLLATE Chinese_Taiwan_Stroke_CI_AS) = cl.storerkey
			and case when o.route  = 'Z7' then '7-11' else '' end COLLATE Chinese_Taiwan_Stroke_CI_AS = cl.code
			and cl.listname = 'Shippinglabel' 
		inner join aad.dbo._t_printer_mapping as pm with (nolocK) 
		on pm.report_name = cl.udf05 
			and pm.workstation_id = @hostName
		left join aad.dbo._t_Codelkup as cl2 with (nolock)
		on (case when o.storerkey = 'DBE1' then 'C001' else o.storerkey end COLLATE Chinese_Taiwan_Stroke_CI_AS) = cl2.storerkey
			and case when o.route  = 'Z7' then '7-11' else '' end COLLATE Chinese_Taiwan_Stroke_CI_AS = cl2.code
			and cl2.listname = 'BTWAPIConfig' 
		where o.externorderkey = @Externorderkey



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

	select @INOUT,@ApiURL ,@Headers,@Json_Result 
	


	/*呼叫EC API接收個資 Start*/
	Declare @outputJson as NVARCHAR(4000)--資料可能大於資料量
	Declare @apiStatus as INT
	Declare @apiMessage as NVARCHAR(500)
	--Exec [INT].iexp.dbo.SP_GetECinfor_TEST 
	Exec [INT].iexp.dbo.SP_GetECinfor_711
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


	/*設定API Body*/
	set @Body = @Json_Result
	
	/*設定LogFilename*/
	DECLARE @LogFilename nvarchar(100)

	Set @LogFilename = @FilePath_bak

	/****Bartender自動列印API Start****/
	/*主要API呼叫*/
	exec _sp_BartenderAPI @INOUT,@ApiURL ,@Headers,@Body ,@Hostname,@workstationid,@LogFilename,1,@status out ,@rtn_message out

	if @status <> 200
		begin 
		
			set @rtn_code  = 'FAIL'

			select @INOUT,@ApiURL2 ,@Headers,@Body ,@Hostname,@workstationid,@LogFilename,1,@rtn_code
			/*備援API呼叫*/
			exec _sp_BartenderAPI @INOUT,@ApiURL2 ,@Headers,@Body ,@Hostname,@workstationid,@LogFilename,1,@status out ,@rtn_message out
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



LOGMSG:
	if isnull(@rtn_message,'') <> ''
		begin 

			exec [AAD].dbo._sp_logerror 
			   @Module --[Module]
			  ,@SPName --[SPName]
			  ,@Hostname --[USERID]
			  ,N'' --[ERRORID]
			  ,@rtn_message --[ErrorMsg]
			  ,@storerkey --[Storerkey]
			  ,@wh_id --[Key1]
			  ,@reportname --[Key2]
			  ,@reportname --[Key3]
			  ,N'' --[Key4]
			  ,N'' --[Key5]

		end


END
