SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
-- =============================================
-- Author:		JackyHsu
-- Create date: 20230830
-- Description:	e2e專案全家API修改為呼叫EC Team
 Test:
 Declare @apiMessage  NVARCHAR(500)
 exec EDI_FamilyApi_EC 'ESL-202201195764901',@apiMessage
 select @apiMessage 
-- =============================================
*/
Alter PROCEDURE EDI_FamilyApi_EC @wKey nvarchar(200) ,@apiMessage as NVARCHAR(500) out
	AS
BEGIN

	SET NOCOUNT ON;
		
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
		,@col21 nvarchar(400)
		,@col22 nvarchar(400)
		,@col23 nvarchar(400)
		,@col24 nvarchar(400)
		,@col25 nvarchar(400)

			SELECT  
			@col01 = '281' , --ParentId
			@col02 = '0001' , --EshopId
			@col03 = '1' , --OPMode
			@col04 = OD.display_order_number , --BuyerPO 
			@col05 = CONVERT(varchar(10), isnull(OD.adddate ,getdate()) ,120)   , --OrderDate
			@col06 = case when OD.cod_flag='Y' then '1' else '3' end , --ServiceType
			@col07 = '誠品線上' , --SenderName
			@col08 = '0287898921' , --SendMobilePhone
			@col09 = OD.ship_to_name, --ReceiverName
			@col10 = OD.ship_to_phone  , --ReceiverMobilePhone
			@col11 = Cast(OD.total_amount as int )  , --OrderAmount
			@col12 = container_id, --EcOrderNo
			@col13 = CONVERT(varchar(10),Getdate() ,120)   , --ShipDate
			@col14 = cast(OD.cod_amount as int )  , --AgencyFee
			@col15 = OD.ship_to_code  , --StoreId
			@col16 = '0'  , --ProdNm
			@col17 = ISNULL(ST.route_code,'') , --Route 
			@col18 = ISNULL(ST.route_code_2,'')   , --Step
			@col19 = ISNULL(OD.Labx02,'') , --EcBarCode9
			@col20 = ISNULL(OD.CaseLable,'') , --EcBarCode16
			@col21 = substring( ISNULL(OD.Labx01,''), 1,17) , --EcBarCode17
			@col22 = substring(ISNULL(OD.Labx01,'') ,18,1)  , --EcBarCode1Chk
			@col23 = Cs.container_id , --OrderNo
			@col24 = '' , --Remark1
			@col25 = ''  --Remark2
			from iexp..W_Case_S as Cs 
			inner join iexp..w_So  as OD  
			ON container_id_original='ESL-'+OD.display_order_number 
			and  Cs.client_code=OD.client_code 
			left outer join  Iexp..m_Storer_S as ST  
			ON  OD.customer_code+OD.ship_to_code= ST.customer_code+ST.site_code 
			where   ST.customer_code='REY'  -- and Cs.Etyps=9 
			--and  Cs.container_id= '02717480005'
			and cs.container_id_original = @wKey


	SET @Json_Result = (
	  SELECT 
		@col01 as parent_id
	   ,@col02 as eshop_id
	   ,@col03 as op_mode
	   ,@col04 as buyer_po
	   ,@col05 as order_date
	   ,@col06 as service_type
	   ,@col07 as sender_name
	   ,@col08 as send_mobile_phone
	   ,@col09 as receiver_name
	   ,@col10 as receiver_mobile_phone
	   ,@col11 as order_amount
	   ,@col12 as ec_order_no
	   ,@col13 as ship_date
	   ,@col14 as agency_fee
	   ,@col15 as store_id
	   ,@col16 as prod_nm
	   ,@col17 as route
	   ,@col18 as step
	   ,@col19 as ec_barcode9
	   ,@col20 as ec_barcode16
	   ,@col21 as ec_barcode17
	   ,@col22 as ec_barcode1_chk
	   ,@col23 as order_no
	   ,@col24 as remark1
	   ,@col25 as remark2
	  FOR JSON PATH -- Json 
	)
	select @Json_Result

	Declare @outputJson as NVARCHAR(MAX)
	Declare @apiStatus as INT

	Exec iexp..SP_GetShip_FAMI @Json_Result,@outputJson output,@apiStatus output,@apiMessage output
	print 'THIS IS outputJson :' + @outputJson
	print @apiStatus
	print @apiMessage 

	
END
GO
