USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_sp_EC_ShipList_WMS]    Script Date: 11/7/2023 9:40:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<JackyHsu>
-- Create date: <2023/02/18>
-- Description:	<EA16_CR_202212_0046_EC驗放完成自動列印出貨單>
--exec [dbo].[_sp_EC_ShipList_WMS] 'PZ1','G016','2209208WTXWSFR','LNWOC210009'
/* 2023-09-09 JackyHsu  緊急修改收貨對象姓名隱碼邏輯<JH01>	*/
/* 2023-10-29 JackyHsu	隱藏收件人姓名	<JH02> */

/*
DECLARE @rtn_code      NVARCHAR(10)    
       ,@rtn_message   NVARCHAR(4000)  
       ,@ww_result     NVARCHAR(10)    
EXEC [_sp_EC_ShipList_WMS] 'pz1','c001','202308270763501','LNWOC210009','','',@rtn_code OUTPUT,@rtn_message OUTPUT ,@ww_result OUTPUT,'','',''  

DBE1 202302185274001
G016 230218B31UYN2D

*/
-- =============================================
ALTER PROCEDURE [dbo].[_sp_EC_ShipList_WMS] 
	@wh_id nvarchar(20),
	@storerkey  nvarchar(20),
	@Externorderkey nvarchar(50) ,
	@hostname nvarchar(50),
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
	
	/*WMS貨主轉換*/
	set @storerkey = Case when @storerkey = 'DBE1' then 'C001' else @storerkey end
						  

	DECLARE 
	 @Body NVARCHAR(MAX) 
	,@INOUT NVARCHAR(10)
	,@ApiURL NVARCHAR(400)  --主服務
	,@ApiURL2 NVARCHAR(400) --備援服務
	,@headers NVARCHAR(400) 
	,@workstationid nvarchar(50)
	,@status int
	,@printer_id nvarchar(400)
	,@FilePath_bak nvarchar(400)
	,@BTW_Label nvarchar(400)
	,@Err_Msg nvarchar(4000)
	,@reportname nvarchar(100) = case when @storerkey = 'G016' then 'HJ_RPT_ShipList_EC_SP' 
										else 'HJ_RPT_ShipList_EC' end --報表名稱
	/*LOG*/
	,@Module nvarchar(100) = 'ShipList'
	,@SPName nvarchar(50) = OBJECT_NAME(@@PROCID)

  

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
	where pm.report_name= @reportname 
	and workstation_id = @hostname

	if @@ROWCOUNT = 0
		begin 
			Print N'_t_printer_mapping 設定未完成，請確認設定資訊。'
			set @Err_Msg = N'_t_printer_mapping 設定未完成，請確認設定資訊。'
			goto LOGMSG
		end

	DECLARE  @VALUES TABLE  
	 (  
	  ID      INT  
	  ,Col    NVARCHAR(MAX)  
	  ,Short  NVARCHAR(50)  
	  ,long   NVARCHAR(200)  
	  ,UDF01  NVARCHAR(100)  
	  ,UDF02  NVARCHAR(100)  
	  ,UDF04  INT  
	  ,Workstation_id NVARCHAR(50)  
	  ,LogFilename    NVARCHAR(100)
	  ,BTW_Label  nvarchar(100)
	  ,printer_id  nvarchar(100)
	 )  


	DECLARE @code  NVARCHAR(20)  
		,@colno INT    
		,@page  INT  
		,@LogFilename NVARCHAR(100)

	/*資料來源 Strat*/
	SET @code = 'ECShipList'  
    SET @page = 0    
  

 --Data Source  
    INSERT INTO @VALUES  
    (ID       
     ,Col     
     ,Short   
     ,long    
     ,UDF01   
     ,UDF02   
     ,UDF04  
     ,Workstation_id
	 ,LogFilename
	 ,BTW_Label
	 ,printer_id
	 )

	SELECT   top 100
	   row_number() over (partition by cd.EXTERNORDERKEY order by cd.EXTERNORDERKEY) as id  
	, QUOTENAME(ROW_NUMBER() over (partition by cd.EXTERNORDERKEY order by cd.EXTERNORDERKEY),'"')   --01
	+','+ QUOTENAME('PZ1' ,'"')    --wh_id  -02 未使用
	+','+ QUOTENAME('ESL-' + cd.EXTERNORDERKEY ,'"')    --order_number --03 未使用
	+','+ QUOTENAME(cd.EXTERNORDERKEY  ,'"')    --display_order_number --04
	+','+ QUOTENAME(o.DeliveryKey ,'"')    --waybill_number --05 未使用
	+','+ QUOTENAME('N' ,'"')    --cod_flag --06 未使用
	--+','+ QUOTENAME(ISNULL(o.C_CONTACT1, '') ,'"')  --ship_to_name --07
	--+','+ QUOTENAME(ISNULL(LEFT(o.C_CONTACT1, 1) + REPLICATE('*', LEN(o.C_CONTACT1) - 1) , '') ,'"') --ship_to_name --07 <JH01>
	+','+ QUOTENAME('***' ,'"')  --ship_to_name --07 --<JH02>
	+','+ QUOTENAME(o.CONSIGNEEKEY ,'"') --ship_to_code --08 未使用
	+','+ QUOTENAME('DDDDD' ,'"') --ship_to_code  --待確認 --09 未使用
	+','+ case when cd.ROUTE = 'Z2' then QUOTENAME(N'宅配','"')
		   when cd.ROUTE = 'Z7' then QUOTENAME(N'7-11','"')
		   when cd.ROUTE = 'Z9' then QUOTENAME(N'CVS全家','"')
		   when cd.ROUTE = 'Z3' then QUOTENAME(N'Fedex','"')
		   when cd.ROUTE = 'Z4' then QUOTENAME(N'門市','"')
		   when cd.ROUTE = 'Z8' then QUOTENAME(N'基金會','"')
		   when cd.ROUTE = 'ZA' then QUOTENAME(N'郵局','"')
		   when cd.ROUTE = 'ZB' then QUOTENAME(N'全家' ,'"')
		   else QUOTENAME( cd.ROUTE ,'"') end   --10
	+','+QUOTENAME(convert(char(8),o.EFFECTIVEDATE,112) ,'"')  --11
	+','+QUOTENAME(cd.ORDERLINENUMBER ,'"') --12 未使用
	+','+QUOTENAME(ISNULL(sku.BUSR2,sku.SKU) ,'"')  --13 
	+','+QUOTENAME(sku.DESCR ,'"') --14
	+','+QUOTENAME(cd.QTY ,'"')  --15
	+','+QUOTENAME(od.SHIPPEDQTY ,'"') --16 未使用
	+','+QUOTENAME(o.BillingKey ,'"') --17
	+','+QUOTENAME(CD2.qty_packed ,'"') --18
	+ ',' + QUOTENAME(ISNULL(cl.udf06,''),'"')  
	+ ',' + QUOTENAME(ISNULL(cl.udf07,''),'"')  
	+','+QUOTENAME(pm.printer_id,'"')  
	+','+QUOTENAME(PM.FilePath_bak 
		+ Replace(cl.UDF03,'*',o.externorderkey COLLATE Chinese_Taiwan_Stroke_CI_AS +'_'+ convert(char(8),getdate(),112) 
		+ Replace(convert(char(9),getdate(),114),':','')),'"')  --as FilePath_bak    
	+','+QUOTENAME(Pm.BTW_Label,'"')   col
	 
	 , cl2.short    
     , cl2.long    
     , cl2.UDF01    
     , cl2.UDF02    
     , cl2.udf04    
     , pm.workstation_id   
	 , pm.FilePath_bak + Replace(cl.UDF03,'*',o.externorderkey COLLATE Chinese_Taiwan_Stroke_CI_AS +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':','')) 
	 , Pm.BTW_Label 
	 , pm.printer_id

	FROM [192.168.1.11].prod.dbo.CASEDETAIL AS cd with (nolocK) 
	LEFT OUTER JOIN [192.168.1.11].prod.dbo.Exchange_Rate as ER with (nolocK) 
		ON cd.CURCY = ER.CURRENCY 
	inner join [192.168.1.11].prod.dbo.ORDERS AS o with (nolocK) 
		ON cd.ORDERKEY = o.ORDERKEY 
		and cd.storerkey = o.storerkey
	inner join [192.168.1.11].prod.dbo.orderdetail as od with (nolocK)
		on o.ORDERKEY = od.ORDERKEY
		and o.STORERKEY = od.STORERKEY
		and cd.ORDERLINENUMBER = od.ORDERLINENUMBER
	LEFT OUTER JOIN [192.168.1.11].prod.dbo.SKU AS sku with (nolocK) 
		on cd.SKU = sku.SKU 
		And Cd.STORERKEY = sku.STORERKEY
	INNER JOIN (SELECT orderkey,CASELABLE,sum(QTY) qty_packed from [192.168.1.11].prod.dbo.CASEDETAIL with (nolocK)
				where EXTERNORDERKEY = @Externorderkey group by orderkey,CASELABLE) as CD2  
		ON ( cd.orderkey = CD2.orderkey and CD.CASELABLE = CD2.CASELABLE )  

	left join aad.dbo._t_Codelkup as cl with (nolocK)
		on cl.storerkey = @storerkey
		and cl.listname = @Module 
		and cl.Code= @code
	inner join #printer_mapping as pm with (nolocK) 
		on pm.report_name = cl.udf05 
	left join aad.dbo._t_Codelkup as cl2 with (nolock)
		on cl2.storerkey = @storerkey
		and cl2.code = @code
		and cl2.listname = 'BTWAPIConfig' 

	where cd.Externorderkey = @Externorderkey


    select top 1 
	@INOUT = short 
	,@ApiURL = udf01
	,@ApiURL2 = udf02 
	,@headers = Long 
	,@colno = udf04 
	,@workstationid = workstation_id
	,@LogFilename = LogFilename 
	,@printer_id = printer_id 
	,@BTW_Label = BTW_Label
	from @VALUES  



    --CSV Column  
    DECLARE @col NVARCHAR(max) = ''    
           ,@T   NVARCHAR(3) = 'COL'    
           ,@R   INT = 1    
           ,@j   INT = 1    
    
    while @j <= @colno    
    begin     
     set @col = @col + @T + right('0'+cast(@R as nvarchar(2)),2) + ','    
     set @j = @j + 1    
     set @R = @R +1    
    end    
    
    set @col = @col + 'printer_id,FilePath_bak,BTW_Label'    
    select @col    
  
    --CSV VALUES  
    DECLARE @id NVARCHAR(10)    
    
    While (1=1)    
    begin    
       if not EXISTS (select top 1 * from @VALUES)    
       begin     
          break    
       end    
        
       select top 1  @id = id from @VALUES    
             
       select @col = @col + char(13)+char(10) + Col from @VALUES where id = @id     
        
       delete @VALUES where id = @id    
        
    end    
  
    select @Body = @col    
    
    --POST API to Bartender  
    print '@Col : '+@col   


	/*主要API呼叫*/
	exec _sp_BartenderAPI @INOUT,@ApiURL ,@Headers,@Body ,@hostname,@workstationid,@LogFilename,1,@status out ,@rtn_message out

	if @status <> 200
		begin 
		
			set @rtn_code  = 'FAIL'

			
			select @INOUT,@ApiURL2 ,@Headers,@Body ,@hostname,@workstationid,@LogFilename,1,@rtn_code
			/*備援API呼叫*/
			exec _sp_BartenderAPI @INOUT,@ApiURL2 ,@Headers,@Body ,@hostname,@workstationid,@LogFilename,1,@status out ,@rtn_message out
			if @status = 200
				begin
					set @rtn_code  = 'PASS'

					Print N'Bartender Server(192.168.1.100) 服務異常，請檢查主機狀況。'
					set @Err_Msg = N'Bartender Server(192.168.1.100) 服務異常，請檢查主機狀況。'
					goto LOGMSG

				end
			else
				begin
					set @rtn_code  = 'FAIL'
					Print N'備援Bartender Server(192.168.1.104) 服務異常，請檢查主機狀況。'
					set @Err_Msg = N'備援Bartender Server(192.168.1.104) 服務異常，請檢查主機狀況。'
					goto LOGMSG
				end



		end
		else
		begin
			set @rtn_code  = 'PASS'
		end
	set @ww_result = @rtn_code



LOGMSG:
	if isnull(@Err_Msg,'') <> ''
		begin 
			exec [AAD].dbo._sp_logerror 
			   @Module --[Module]
			  ,@SPName --[SPName]
			  ,@hostname --[USERID]
			  ,N'' --[ERRORID]
			  ,@Err_Msg --[ErrorMsg]
			  ,@storerkey --[Storerkey]
			  ,@wh_id --[Key1]
			  ,@Externorderkey --[Key2]
			  ,@reportname --[Key3]
			  ,N'' --[Key4]
			  ,N'' --[Key5]
		end
end



