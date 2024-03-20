USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_SP_WMS_WBL_CSO_TCAT_EC]    Script Date: 8/17/2023 12:01:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
-- Author:		JackyHsu
-- Create date: 2023/07/05
-- Description: 黑貓標籤列印

exec _SP_WMS_WBL_CSO_TCAT_EC 
'LNWOC210009' --hostname
,'230510CK3RD7M8' --單號

 =============================================*/
ALTER PROCEDURE [dbo].[_SP_WMS_WBL_CSO_TCAT_EC] 
@Hostname nvarchar(50) ,	
@externorderkey nvarchar(50) 

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
,@reportname nvarchar(100) = 'HJ_WBL_CSO_TCAT' --報表名稱
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
,@col15 nvarchar(400)
,@col16 nvarchar(400)
,@col17 nvarchar(400)
,@col18 nvarchar(400)
,@col19 nvarchar(400)
,@col20 nvarchar(400)


		SELECT TOP 1
			  @col01 = 'PZ1' 
			, @col02 = o.EXTERNORDERKEY
			, @col03 = o.EXTERNORDERKEY
			, @col04 = o.CASELABLE 
			, @col05 = '*' + o.CASELABLE + '*'
			, @col06 = o.EXTERNORDERKEY
			, @col07 = '*' + o.EXTERNORDERKEY + '*'
			, @col08 = o.EXTERNORDERKEY
			, @col09 = o.C_CONTACT1
			, @col10 = o.C_ADDRESS1
			, @col11 = o.C_PHONE1
			, @col12 = REPLACE(CONVERT(NVARCHAR(20),o.SHIPDATE, 111), '/', '-')
			, @col13 = REPLACE(CONVERT(NVARCHAR(20),DATEADD(d,1,o.SHIPDATE), 111), '/', '-')
			, @col14 = case when o.C_KEEPPY > 0 then cast (o.C_KEEPPY as nvarchar) + N'元' else N'不收款' end --cod_flag
			, @col15 = cast (DATEPART(mm, DATEADD(d,1,o.SHIPDATE)) as nvarchar) + N'月' + cast (DATEPART(DD, DATEADD(d,1,o.SHIPDATE)) as nvarchar) + N'日' --AS hope_delivery_date
			, @col16 = o.C_ZIP
			, @col17 = LEFT(REPLACE(o.C_ZIP, '-', ''), 7)
			, @col18 = o.SUSR5
			, @col19 = N'01|' + o.CASELABLE +'|10|279529660100|N|0|01|01|02|' + SUBSTRING(REPLACE(o.C_ZIP, '-', ''), 3, 10) + '|' + CONVERT(NVARCHAR(20), DATEADD(d,1,o.SHIPDATE), 112) + '|01||0|||||||||||' --AS qr_code
			, @col20 = Case when CHARINDEX('路',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('路',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
				 when CHARINDEX('街',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('街',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
				 when CHARINDEX('鄉',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('鄉',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
				 when CHARINDEX('鎮',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('鎮',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
				 when CHARINDEX('區',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('區',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
				 when CHARINDEX('市',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('市',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
				 when CHARINDEX('縣',rtrim(o.C_ADDRESS1)) > 0 then Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),CHARINDEX('縣',Rtrim(o.C_ADDRESS1)),len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1)))
			else Right('**************'+SUBSTRING(Rtrim(o.C_ADDRESS1),10,len(Rtrim(o.C_ADDRESS1))),len(Rtrim(o.C_ADDRESS1))) end


			, @printer_id = pm.printer_id
			, @FilePath_bak = PM.FilePath_bak + Replace(cl.UDF03,'*',o.EXTERNORDERKEY COLLATE Chinese_Taiwan_Stroke_CI_AS +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')) --as FilePath_bak
			, @BTW_Label = Pm.BTW_Label

			, @INOUT = cl2.short
			, @headers = cl2.long
			, @ApiURL = cl2.UDF01
			, @ApiURL2 = cl2.UDF02
			, @workstationid = pm.workstation_id
			, @storerkey = Case when o.storerkey = 'DBE1' then 'C001' else o.storerkey end 

		FROM
			[192.168.1.11].[PROD].[dbo].[Vw_Case_Addr_Invoice] AS O WITH (NOLOCK)
			left join aad.dbo._t_Codelkup as cl with (nolocK)
			on (case when o.storerkey = 'DBE1' then 'C001' else o.storerkey end COLLATE Chinese_Taiwan_Stroke_CI_AS) = cl.storerkey
			and case when o.route = 'Z2' then 'T-CAT' else '' end COLLATE Chinese_Taiwan_Stroke_CI_AS = cl.code
			and cl.listname = 'Shippinglabel' 
			inner join aad.dbo._t_printer_mapping as pm with (nolocK) 
			on pm.report_name = cl.udf05 
			and pm.workstation_id = @Hostname
			left join aad.dbo._t_Codelkup as cl2 with (nolock)
			on (case when o.storerkey = 'DBE1' then 'C001' else o.storerkey end COLLATE Chinese_Taiwan_Stroke_CI_AS) = cl2.storerkey
			and case when o.route = 'Z2' then 'T-CAT' else '' end COLLATE Chinese_Taiwan_Stroke_CI_AS = cl2.code
			and cl2.listname = 'BTWAPIConfig' 

		where o.EXTERNORDERKEY = @externorderkey

			/*設定API參數*/


/*_t_printer_mapping and _t_Codelkup.listname = Shippinglabel設定檢核*/
if (isnull(@FilePath_bak,'') = '' ) 
	begin 
		
		select @FilePath_bak
		Print N'_t_Codelkup.listname = Shippinglabel 設定未完成，請確認是否設定。'
		set @rtn_message = N'_t_Codelkup.listname = Shippinglabel 設定未完成，請確認是否設定。'
		goto LOGMSG

	end

/*_t_Codelkup.listname = BTWAPIConfig 設定檢核*/
if (isnull(@INOUT,'') = '' or isnull(@headers,'') = '' or isnull(@ApiURL,'') = '' or isnull(@ApiURL2,'') = '' )
begin 
		select @INOUT,@headers,@ApiURL,@ApiURL2
		Print N'_t_Codelkup.listname = BTWAPIConfig 設定未完成，請確認是否設定。'
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
		
	/*呼叫EC API接收個資 End*/

	set @Json_Result = @outputJson

	select @Body = @Json_Result

	select @INOUT,@ApiURL ,@Headers,@Body,@Hostname,@workstationid,@FilePath_bak

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

