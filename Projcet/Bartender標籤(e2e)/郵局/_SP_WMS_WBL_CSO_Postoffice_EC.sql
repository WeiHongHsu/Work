USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_SP_WMS_WBL_CSO_Postoffice_EC]    Script Date: 9/12/2023 3:47:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<JackyHsu>
-- Create date: <2023/01/17>
-- Description:	<EA16_CR_202212_0046_郵局驗放完成自動列印面單>
-- exec [dbo].[_SP_WMS_WBL_CSO_Postoffice_EC] 'LNWOC210009' , '202302028590201'
-- =============================================
ALTER PROCEDURE [dbo].[_SP_WMS_WBL_CSO_Postoffice_EC] @hostName nvarchar(30),@Externorderkey nvarchar(30)
	
AS
BEGIN
	
	SET NOCOUNT ON;

	
	/*API變數*/
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
	,@reportname nvarchar(100) = 'HJ_WBL_CSO_Postoffice' --報表名稱
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


	/*Body欄位變數*/
	Declare @Json_Result nvarchar(2000) = ''
	,@col01    nvarchar(400)
	,@col02    nvarchar(400)
	,@col03    nvarchar(400)
	,@col04    nvarchar(400)
	,@col05    nvarchar(400)
	,@col06    nvarchar(400)
	,@col07    nvarchar(400)
	,@col08    nvarchar(400)


	/*資料來源*/
	select 
	  @col01 = o.EXTERNORDERKEY  --訂單編號
	, @col02 = Replace(o.CASELABLE,' ','')  --查貨編號
	, @col03 = ISNULL(o.C_CONTACT1, '') --收件人名稱
	, @col04 = o.C_ADDRESS1  --收件人地址
	, @col05 = o.C_ZIP  --收件人郵遞區號
	, @col06 = isnull(case when left(o.C_PHONE1,3) = '886' then substring(o.C_PHONE1,4,len(rtrim(o.C_PHONE1))) else o.C_PHONE1 end,'0900-000-000') --收件人電話
	, @col07 = o.C_CITY  --縣市
	, @col08 = case when o.route = 'ZA' then N'大宗郵資已付國內小包'  --大小包裹判斷
					when o.route = 'ZD' then N'大宗郵資已付國內包裹' 
					else N'大宗郵資已付國內包裹'
					end

	, @printer_id = max(pm.printer_id) --as printer_id
	, @FilePath_bak = max(PM.FilePath_bak + Replace(cl.UDF03,'*',o.externorderkey COLLATE Chinese_Taiwan_Stroke_CI_AS +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')))  --as FilePath_bak
	, @BTW_Label = max(Pm.BTW_Label)  --as BTW_Label
	, @ApiURL = max(cl2.udf01) --as APIURL
	, @ApiURL2 = max(cl2.udf02) --as APIURL
	, @INOUT = max(cl2.short) --as INOUT
	, @headers = max(cl2.long) --as headers
	, @workstationid = max(pm.workstation_id)

	from [192.168.1.11].prod.dbo.ORDERS as o with (nolocK)
	inner join [192.168.1.11].prod.dbo.CASEDETAIL as cd with (nolocK) 
	on o.ORDERKEY = cd.orderkey
	and o.route in ('ZA','ZD')
	left join aad.dbo._t_Codelkup as cl with (nolocK)
	on (case when o.storerkey = 'DBE1' then 'C001' end COLLATE Chinese_Taiwan_Stroke_CI_AS) = cl.storerkey
	and o.route COLLATE Chinese_Taiwan_Stroke_CI_AS = cl.code
	and cl.listname = 'Shippinglabel' 
	inner join aad.dbo._t_printer_mapping as pm with (nolocK) 
	on pm.report_name = cl.udf05 
	and pm.workstation_id = @hostname
	left join aad.dbo._t_Codelkup as cl2 with (nolock)
	on (case when o.storerkey = 'DBE1' then 'C001' end COLLATE Chinese_Taiwan_Stroke_CI_AS) = cl2.storerkey
	and o.route COLLATE Chinese_Taiwan_Stroke_CI_AS = cl2.code
	and cl2.listname = 'BTWAPIConfig' 

	where cd.EXTERNORDERKEY =  @Externorderkey

	group by o.EXTERNORDERKEY 
	, Replace(o.CASELABLE,' ','')   
	, o.CASELABLE
	, ISNULL(o.C_CONTACT1, '')
	, o.C_ADDRESS1
	, o.C_ZIP
	, o.C_PHONE1
	, o.C_CITY 
	, o.route


	SET @Json_Result = (
	  SELECT 
		@col01 as EXTERNORDERKEY
	   ,@col02 as CASELALBLE
	   ,@col03 as C_CONTACT1
	   ,@col04 as C_ADDRESS1
	   ,@col05 as C_ZIP
	   ,@col06 as C_PHONE1
	   ,@col07 as C_CITY
	   ,@col08 as Flag
	   ,@printer_id as printer_id
	   ,@FilePath_bak as FilePath_bak
	   ,@BTW_Label as BTW_Label

	  FOR JSON PATH       
	)
	
	/*呼叫EC API接收個資 Start*/
	Declare @outputJson as NVARCHAR(4000)--資料可能大於資料量
	Declare @apiStatus as INT
	Declare @apiMessage as NVARCHAR(500)
	Exec [INT].iexp.[dbo].[SP_GetECinfor_POSTAL]
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
		set @rtn_message = @apiMessage + N' EC API 異常，請聯繫EC Team確認。'
		goto LOGMSG
	end
	
	set @Json_Result = @outputJson
	/*呼叫EC API接收個資 End*/

	set @Body = @Json_Result

	select @INOUT,@ApiURL ,@Headers,@Body 


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
end



