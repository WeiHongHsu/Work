USE [AAD]
GO
/****** Object:  StoredProcedure [dbo].[_sp_HJ_BTW_RPT_ShipList_EC]    Script Date: 10/29/2023 9:54:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

    
/*************************************************************************************/                            
/* Store procedure: _sp_HJ_BTW_RPT_ShipList_EC                                       */                            
/* Creation Date: 01-Feb-2023                                                        */              
/* Written by: LeoHuang                                                              */              
/*                                                                                   */              
/* Purpose: Post API to bartender Server for EC ShipList Printing                    */               
/*                                                                                   */              
/* Input Parameters:   @wh_id         NVARCHAR(20),    - 'pz1'                       */              
/*                     @storerkey     NVARCHAR(20),    - @erp_site                   */              
/*                     @carton_number NVARCHAR(50),    - @carton_number              */              
/*                     @user_id       NVARCHAR(20),    - @user_id                    */            
/*                     @rtn_code      NVARCHAR(10)     - ''                          */            
/*                     @rtn_message   NVARCHAR(4000)   - ''                          */            
/*                     @ww_result     NVARCHAR(10)     - ''                          */            
/*                     @Parameter1    NVARCHAR(200)    - ''                          */            
/*                     @Parameter2    NVARCHAR(200)    - ''                          */            
/*                     @Parameter3    NVARCHAR(200)    - ''                          */            
/*                        - 0                                                        */              
/*                                                                                   */              
/* Output Parameters:                                                                */                      
/*                                                                                   */              
/* Called By: SQL Agent Job                                                          */              
/*                                                                                   */              
/*                                                                                   */              
/* Version: 1.0                                                                      */              
/*                                                                                   */              
/* Data Modifications:                                                               */              
/*                                                                                   */              
/* Updates:                                                                          */              
/* Date        Author   Purposes                                                     */            
/* 2023-02-01  LeoH     Post API to bartender for EC ShipList.                       */    
/* 2023-02-07  LeoH     Remove display_order_number condition.  <LH01>               */
/* 2023-02-16 JackyHsu  新增ErrorLog機制												 */
/* 2023-08-24 JackyHsu	修改出貨明細圖檔由_T_Codelkup.UDF06,UDF07指定來源	<JH01>		 */
/*************************************************************************************/      
/************************************************************************  
Testing Scenarios  
DECLARE @rtn_code      NVARCHAR(10)    
       ,@rtn_message   NVARCHAR(4000)  
       ,@ww_result     NVARCHAR(10)    
EXEC _sp_HJ_BTW_RPT_ShipList_EC 'pz1','C001','ESL-202112200657801','JackyHsu','','',@rtn_code OUTPUT,@rtn_message OUTPUT ,@ww_result OUTPUT,'','',''  
************************************************************************/  
  
ALTER PROCEDURE [dbo].[_sp_HJ_BTW_RPT_ShipList_EC]    
 /*固定參數*/    
 @wh_id         NVARCHAR(20),    
 @storerkey     NVARCHAR(20),    
 @carton_number NVARCHAR(50),    
 @userid       NVARCHAR(20),    
 @Sdate         DATETIME, --未使用    
 @Edate         DATETIME, --未使用    
 /*Page Editor固定參數*/    
 @rtn_code      NVARCHAR(10)   OUTPUT,    
 @rtn_message   NVARCHAR(4000) OUTPUT,    
 @ww_result     NVARCHAR(10)   OUTPUT,    
 /*預留參數*/    
 @Parameter1    NVARCHAR(200) ,    
 @Parameter2    NVARCHAR(200) ,    
 @Parameter3    NVARCHAR(200)     
AS    
BEGIN    


DECLARE 
 @Body NVARCHAR(max) 
,@INOUT NVARCHAR(10)
,@ApiURL NVARCHAR(400)  --主服務
,@ApiURL2 NVARCHAR(400) --備援服務
,@headers NVARCHAR(400) 
,@workstationid nvarchar(50)
,@status int
,@printer_id nvarchar(400)
,@FilePath_bak nvarchar(400)
,@BTW_Label nvarchar(400)
,@reportname nvarchar(100) = 'HJ_RPT_ShipList_EC' --報表名稱
/*LOG*/
,@Module nvarchar(100) = 'ShipList'
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
	 ,LogFilename)  
    SELECT row_number() over (partition by om.display_order_number order by om.display_order_number) as id  
     , QUOTENAME(ROW_NUMBER() over (partition by om.display_order_number order by om.display_order_number),'"')   
     + ',' + QUOTENAME(ISNULL(om.wh_id,''),'"')  
     + ',' + QUOTENAME(ISNULL(om.order_number,''),'"')   
     + ',' + QUOTENAME(ISNULL(om.display_order_number,''),'"')  
     + ',' + QUOTENAME(ISNULL(om.waybill_number,''),'"')  
     + ',' + QUOTENAME(ISNULL(om.cod_flag,''),'"')  
     --+ ',' + QUOTENAME(ISNULL(om.ship_to_name,''),'"') 
	 + ',' + QUOTENAME(ISNULL(LEFT(om.ship_to_name, 1) + REPLICATE('*', LEN(om.ship_to_name) - 1) , '') ,'"') --<JH01>
     + ',' + QUOTENAME(ISNULL(om.ship_to_code,''),'"')  
     + ',' + QUOTENAME(ISNULL(om.ship_to_description,''),'"')  
     + ',' + CASE  
                --WHEN UPPER(om.erp_site) = N'G016' THEN N'蝦皮'  
                WHEN UPPER(om.customer_code) = N'FEDEX' THEN QUOTENAME(N'FedEx','"')  
                WHEN UPPER(om.customer_code) = N'7-11'  THEN QUOTENAME(N'7-11','"')  
                WHEN UPPER(om.customer_code) = N'CVS'   THEN QUOTENAME(N'CVS全家','"')  
                WHEN UPPER(om.customer_code) = N'T-CAT' THEN QUOTENAME(N'宅配','"')  
                WHEN UPPER(om.customer_code) = N'REY'   THEN QUOTENAME(N'全家','"')  
                WHEN UPPER(om.customer_code) = N'ESL'   THEN QUOTENAME(N'門市','"')  
                WHEN UPPER(om.customer_code) = N'Z8'    THEN QUOTENAME(N'基金會','"')  
				when UPPER(om.customer_code) in (N'ZA',N'ZD') then QUOTENAME(N'郵局','"')
                ELSE QUOTENAME(ISNULL(UPPER(om.customer_code),''),'"') END   
     + ',' + QUOTENAME(ISNULL(CONVERT(NVARCHAR(20), oo.order_date, 111),''),'"') 
     + ',' + QUOTENAME(ISNULL(Cast(od.line_number as Nvarchar),''),'"')
     + ',' + QUOTENAME(ISNULL(substring(od.item_number ,5,15),''),'"')  
     + ',' + QUOTENAME(ISNULL(sku.item_description,''),'"')  
     + ',' + QUOTENAME(ISNULL(od.qty_packed,''),'"')  
     + ',' + QUOTENAME(ISNULL(od.qty_shipped,''),'"')
     + ',' + QUOTENAME(ISNULL(od.ref_order_number,''),'"')   
     + ',' + QUOTENAME(ISNULL(od2.qty_packed,''),'"')  
	 + ',' + QUOTENAME(ISNULL(cl.udf06,''),'"')  --<JH01>
	 + ',' + QUOTENAME(ISNULL(cl.udf07,''),'"')  --<JH01>
     + ',' + QUOTENAME(pm.printer_id,'"')    
     + ',' + QUOTENAME(PM.FilePath_bak 
	       + Replace(cl.UDF03,'*',om.display_order_number +'_'+ convert(char(8),getdate(),112) 
	       + Replace(convert(char(9),getdate(),114),':','')),'"')  --as FilePath_bak    
     + ',' + QUOTENAME(Pm.BTW_Label,'"')   
     , cl2.short    
     , cl2.long    
     , cl2.UDF01    
     , cl2.UDF02    
     , cl2.udf04    
     , pm.workstation_id   
	 , pm.FilePath_bak + Replace(cl.UDF03,'*',om.carton_number +'_'+ convert(char(8),getdate(),112) + Replace(convert(char(9),getdate(),114),':',''))
    FROM  
    _t_order_carton_master AS om WITH (NOLOCK)   
    INNER JOIN _t_order_carton_detail AS od WITH (NOLOCK)  
    ON ( om.carton_number = od.carton_number AND om.wh_id = od.wh_id )  
    INNER JOIN _tv_item_master AS sku WITH (NOLOCK)  
    ON (od.wh_id = sku.wh_id AND od.item_number = sku.item_number)  
    INNER JOIN _tv_order_master AS oo WITH (NOLOCK)  
    ON (om.wh_id = oo.wh_id and om.order_number = oo.order_number ) 
    INNER JOIN (SELECT wh_id,carton_number,sum(qty_packed) qty_packed from _t_order_carton_detail   
    WHERE carton_number = @carton_number group by wh_id,carton_number) as od2  
    ON ( om.wh_id = od2.wh_id and om.carton_number = od2.carton_number )  
    LEFT JOIN _t_Codelkup as cl with (nolocK)    
    on ( om.erp_site = cl.storerkey and cl.listname = @Module and Code=@code)  
	left join #printer_mapping as pm with (nolocK) 
	on pm.report_name = cl.udf05 
    LEFT JOIN _t_Codelkup as cl2 with (nolocK)    
    on om.erp_site = cl2.storerkey AND cl2.code = @code  
    AND cl2.listname = 'BTWAPIConfig'  
       WHERE  
    om.wh_id = @wh_id  
    AND om.erp_site = @storerkey  
    --AND om.display_order_number= @Parameter1   -- LH01
    AND od.carton_number = @carton_number  
  


    --API Parameter  
    select top 1 @INOUT = short ,@ApiURL = udf01,@ApiURL2 = udf02 ,@headers = Long 
	,@colno = udf04 ,@workstationid = workstation_id, @LogFilename = LogFilename 
	,@printer_id = printer_id ,@BTW_Label = BTW_Label from @VALUES    
 
  	/*_t_printer_mapping and _t_Codelkup.listname = Shippinglabel設定檢核*/
	if (isnull(@LogFilename,'') = '' ) 
		begin 
		
			select @LogFilename
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
	/*Bartender自動列印API*/

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
  
  
