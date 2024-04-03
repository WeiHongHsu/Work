USE [Iexp]
GO
/****** Object:  StoredProcedure [dbo].[_Edi_OutPut_Fmt]    Script Date: 9/14/2023 4:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





 
/****
 =============================================================================================
 Author			: Calvin
 Create Date	: 2020.04.09
 Description	: 序號取得預存程序
 Test SQL		: Exec _Edi_OutPut_Fmt @Extkeys , @OutPutKey , @UserID , @Msg

 =============================================================================================
 Author			: ken
 Alter Label	: 20211217_ken
 Alter Date   : 20211217
 Description	: 1.修改產出EDL_RTV W_CASED_S 的條件， w_Cased_S.po_number  change to  w_Cased_S.order_number 
                      2.修改撈出資料時間格式，避免產檔有上下午
 =============================================================================================
 Author			: ken
 Alter Label	: 20211224_ken
 Alter Date   : 20211224
 Description	: 1.增加EX11邏輯
                      2.增加BX11邏輯
 =============================================================================================
 Author			: Kevin
 Alter Label	: 20220104_Kevin
 Alter Date   : 20220104
 Description	: For SP11 exprt EX07_SO, copy h_Sod_Imp.lot_number_3 and FC02SC = 351
 =============================================================================================
 Author			: Raines
 Alter Label	: 20220105_Raines
 Alter Date   : 20220105
 Description	: add union to conver w case
 test script	: EXEC iexp.._Edi_OutPut_Fmt 'EX2MSM','ESL,B049220103','EDIapi',''
 =============================================================================================
  Author		: ken
 Alter Label	: 20220109_Ken
 Alter Date   : 20220109
 Description	: add REY_XML
 test script	: EXEC iexp.._Edi_OutPut_Fmt 'REY_XML','ESL,GARY','EDIapi',''
 =============================================================================================
  =============================================================================================
  Author		: ben
 Alter Label	: 20220119_ben
 Alter Date   : 20220119
 Description	: <20220119_ben>修改地址、代收金額、契客代號   
 test script	:  EXEC iexp.._Edi_OutPut_Fmt 't-cat_Eod','ESL,TEXTFILE.EOD','EDIapi',''
 =============================================================================================
   Author		: ken
 Alter Label	: 20220318_Ken
 Alter Date   : 20220318
 Description	: alter mod 'EDL_PO'  vendor_name change to vendor_name_full
 test script	: EXEC iexp.._Edi_OutPut_Fmt 'REY_XML','ESL,GARY','EDIapi',''
 =============================================================================================
 Author			: Raines
 Alter Label	: 20220511_Raines
 Alter Date		: 20220511
 Description	: LGORT change by Julian
 test script	: EXEC iexp.._Edi_OutPut_Fmt 'EX07_GI','ESL,5040674876','EDIapi',''
 =============================================================================================
 Author			: ken
 Alter Label	: 20220513_ken
 Alter Date		: 20220513
 Description	: alter mod EDL_PO substring sku descr for 40 size(substring(m_Sku.item_description,0,39))，and add table with nolock
 test script	: 
 =============================================================================================
 Author		　　: ken
 Alter Label	: 20220520_ken
 Alter Date		: 20220520
 Description	: alter mod EDL_PO with no sku 
 test script	: 
 =============================================================================================
 Author		　　: ken
 Alter Label	: 20220531_ken
 Alter Date		: 20220531
 Description	: seven_sin add AwardAmount v2.86 版本的調整
 test script	: 
 =============================================================================================
 Author			: Raines
 Alter Label	: 20220614_Raines 
 Alter Date		: 20220614
 Description	: add ex07 gi logic for 493 orders
 test script	: EXEC iexp.._Edi_OutPut_Fmt 'EX07_GI','ESL,4931596451','EDIapi',''
 =============================================================================================
 Author			: JackyHsu(HJ01)
 Alter Label	: 20230223 
 Alter Date		: 20230223
 Description	: 修正資料存在於不同箱未顯示的問題
 test script	: 
 =============================================================================================
 Author			: KyeLi
 Alter Label	: KL01
 Alter Date		: 20230801
 Description	: Added EX07_GPO for export global orders
 test script	: 
 =============================================================================================
 Author			: LeoHuang
 Alter Label	: LH01
 Alter Date		: 20230801
 Description	: 修正ctyps資料為19 (根據B2B信件需求修正:RE: 40283000294 無法上傳B2B)
 test script	: 
 =============================================================================================
 Author			: LeoHuang
 Alter Label	: LH02
 Alter Date		: 20230802
 Description	: 修正cdate 和 rdate 順序 (根據B2B信件進行Bug Fix)
 test script	: 
 =============================================================================================
  =============================================================================================
 Author			: JackyHsu
 Alter Label	: JH02
 Alter Date		: 20230807
 Description	: 修改EX08 VBELN_VL與POSNR_VL資料來源
 test script	: 
 =============================================================================================
 =============================================================================================
 Author			: JackyHsu
 Alter Label	: JH03
 Alter Date		: 20230814
 Description	: 修改BX11 F003補齊商品主檔後送出
 test script	: 
 =============================================================================================

****/

ALTER PROCEDURE [dbo].[_Edi_OutPut_Fmt] ( 
	@InfMod NVarchar(50) ,  @Extkeys NVarchar(500) , @UserID NVarchar(200) , @Msg Nvarchar(200) OutPut ) 
AS
Begin
	SET NOCOUNT ON;
    Declare @Client_Code Nvarchar(20) , @OutPutKey NVarchar(500) , @RowCount Int , @Srt NVarchar(Max)
	IF CHARINDEX(  ','  ,@Extkeys )>1
		Select @Client_Code = SJOB.dbo.StrArgIdxTag(@Extkeys,1,',') , @OutPutKey = SJOB.dbo.StrArgIdxTag(@Extkeys,2,',') , @RowCount = 0 , @Msg = ''
	else
		Select @OutPutKey=@Extkeys
	--IF CHARINDEX(  ','  ,@UserID )>1
	--	Select @Client_Code = SJOB.dbo.StrArgIdxTag(@UserID,1,',') , @OutPutKey = SJOB.dbo.StrArgIdxTag(@UserID ,2,',') , @RowCount = 0 , @Msg = ''

    -- 資料取得區塊    
    If @InfMod = 'EX01' --通過型EX01回傳（內外購）
    Begin
        Print @InfMod
        Select 
            w_Xsod.order_number As RBLNRB , Right(w_Xsod.order_line_number,6) As RBPOSB , 1 As BTYPB , 
            w_Xsod.display_order_number As BLNRB , Right(w_Xsod.Display_order_line_number,6) As BPOSB , 1 As BTYPA ,
            w_Xsod.cust_po_number As BLNRA , Right('00000'+w_Xsod.po_line_number,6) As BPOSA , Right(w_Xsod.item_number , 18) As MATNR ,
            w_Xsod.qty_shipped As IMNGA , w_Xsod.order_uom As IEINA , w_Xso.erp_site As WERKA , w_Xso.customer_code As WERKS , 
            Convert(varchar(8) , Isnull(w_Xso.Cdate,Getdate()-0.25) , 112) As ORDERDATE
        From w_Xso Left Join w_Xsod On 
                w_Xso.client_code = w_Xsod.client_code And w_Xso.erp_site = w_Xsod.erp_site And 
                w_Xso.cust_po_number = w_Xsod.cust_po_number And w_Xso.display_order_number = w_Xsod.Display_order_number 
        Where w_Xso.client_code = @Client_Code And w_Xso.cust_po_number = @OutPutKey
	    
        Select * From (Select wrline='' ) As Newline1

        Select 
            Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()-0.25), 112) As BLDAT , Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()-0.25), 112) As BUDAT , 
            IsNull(left(vendor_so_number,16),'') As XBLNR , w_Po.po_total_qty As BKTXT , isnull(left(w_Po.vendor_so_number_2,16),' ') As FRBNR , 
            'MB01' As TCODE , Right('00000'+w_Pod.item_number ,18) As MATNR , w_Po.erp_site As WERKS , ' ' As LGORT , '101' As BWART ,
            w_Pod.qty_received As ERFMG , 'EA' As ERFME , case when w_Po.Erp_type_text = 'ZEL' then ' ' else left(w_Po.display_po_number ,10) end As EBELN , 
            case when w_Po.Erp_type_text = 'ZEL' then ' ' else Right(w_Pod.line_number ,5) end As EBELP ,
            left(w_Po.display_po_number,10)+'     '+cast(isnull(w_Po.vendor_total_qty,0) as varchar(5))+'/'+cast(isnull(w_Po.vendor_case_count ,0) as varchar(4))+'  ' As SGTXT , 
            'B' As KZBEW , case when w_Po.Erp_type_text = 'ZEL' then left(w_Po.display_po_number,10) else ' ' end As VLIEF_AVIS , 
            case when w_Po.Erp_type_text = 'ZEL' then Right(w_Pod.line_number,6) else '' end As VBELP_AVIS
        From w_Po Left Join w_Pod On
                w_Po.client_code = w_Pod.client_code And w_Po.erp_site = w_Pod.erp_site And
                w_Po.display_po_number = w_Pod.display_po_number
        where w_Po.client_code = @Client_Code And w_Po.display_po_number = @OutPutKey And w_Pod.qty_received <> 0
      
        Select * From (Select wrline='' ) As Newline2

        Select
            w_Xso.customer_code + Convert(varchar(6) , Isnull(w_Xso.Cdate,Getdate()-0.25) , 12) As PARTNER_ID , ' ' As TANUM , Convert(varchar(8) , Isnull(w_Xso.Cdate,Getdate()-0.25) , 112) As KODAT  , 'X' As KOMUE , 'X' As WABUC , 
            Convert(varchar(8) , Isnull(w_Xso.Cdate,Getdate()-0.25) , 112) As WADAT_IST , w_Xsod.Display_order_number As VBELN_VL , Right(w_Xsod.Display_order_line_number ,6) As POSNR_VL , 
            w_Xso.customer_code + Convert(varchar(6) , Isnull(w_Xso.Cdate,Getdate()-0.25) , 12) As VBELN , Right(w_Xsod.po_line_number,6) As POSNN , w_Xsod.qty_shipped As PIKMG , 
            Right('00000'+w_Xsod.item_number , 18) As MATNR , w_Xso.customer_code As APDa
        From w_Xso Left Join w_Xsod On 
                w_Xso.client_code = w_Xsod.client_code And w_Xso.erp_site = w_Xsod.erp_site And 
                w_Xso.cust_po_number = w_Xsod.cust_po_number And w_Xso.display_order_number = w_Xsod.Display_order_number 
        Where w_Xso.client_code = @Client_Code And w_Xso.cust_po_number = @OutPutKey And w_Xsod.qty_shipped <> 0
            And w_Xso.ship_to_code Not Like 'F%'
        Set @RowCount = @RowCount + @@ROWCOUNT       
    End
	--  EX07 來源 SO  轉單  : 專用格式 EX_WMMBID02.fmt 
    If @InfMod = 'EX07_SO' -- 在庫調除單建立
    Begin
        Print @InfMod

        Select 
            Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BLDAT 
			, Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BUDAT 
			, ' ' As XBLNR 
			, w_So.so_total_fulfill_qty As BKTXT 
			, ' ' As FRBNR 
			, case when type_text='RSO' then 'MB01' else 'MB1B' END As TCODE 
			, left( w_Sod.item_number ,18) As MATNR 
			, w_So.erp_site As WERKS 
			-- If h_Sod_Imp.lot_number_3 = '0001', copy it to LGORT
			, case when w_So.erp_site='1300' and type_text in ('PO','WPO','SO') then 'PU01' 
			       when Isnull(w_Sod.lot_number_3,'') = '' and Isnull(h_Sod_Imp.lot_number_3,'') <> '' Then h_Sod_Imp.lot_number_3
				   else w_Sod.lot_number_3 end  As LGORT 
			
			--w_SO.erp_site (Storerkey)+ w_So.ship_to_code (Ship to location) = From and to location
			, case when type_text='RSO' then '101' 
				when w_So.erp_site+w_So.ship_to_code = '1110F001' then '351' 
				when w_So.erp_site+w_So.ship_to_code = '1116F001' then '351'
				when w_So.erp_site+w_So.ship_to_code = '1300F001' then '351'
				when w_So.erp_site+w_So.ship_to_code = 'C001F001' then '351'
				when w_So.erp_site+w_So.ship_to_code = 'FC02F001' then '351'
				when w_So.erp_site+w_So.ship_to_code = 'G011F001' then '351'
				when w_So.erp_site+w_So.ship_to_code = 'G016F001' then '351'
				
				when w_So.erp_site+LEFT(w_So.ship_to_code,2) = 'FC02SS' then '351' --FC02 to coffee store
				when w_So.erp_site+LEFT(w_So.ship_to_code,2) = 'FC02SC' then '351' --FC02 to coffee store
				
				else '951' end as BWART
			
			, w_Sod.qty_shipped As ERFMG , 'EA' As ERFME 
			, left(w_So.display_order_number ,10) As EBELN 
			, dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As EBELP 
			, ' ' As ELIKZ 
			, ' ' As SGTXT 
			, 'B' As KZBEW 
			, ' ' As VLIEF_AVIS 
			, REPLICATE(' ',6) As VBELP_AVIS
			
			From w_So with (nolock)
			Left join w_Sod with (nolock)
			ON w_So.client_code = w_Sod.client_code 
			And w_So.erp_site = w_Sod.erp_site 
			And w_So.display_order_number = w_Sod.display_order_number 
			
			Left join h_Sod_Imp with (nolock) -- Add join h_Sod_Imp from SP11, h_Sod_Imp.lot_number_3 = '0001'
			ON w_Sod.client_code = h_Sod_Imp.client_code 
			And w_Sod.erp_site = h_Sod_Imp.erp_site 
			And w_Sod.display_order_number = h_Sod_Imp.display_order_number
			And w_Sod.Display_order_line_number = h_Sod_Imp.Display_order_line_number
			
		    where w_So.client_code = @Client_Code 
			And w_So.display_order_number = @OutPutKey 
			And w_Sod.qty_shipped !=0
			
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
	--KL01 Start
    If @InfMod = 'EX07_GPO' --海外調撥補產生PO
    Begin
        Print @InfMod

        Select 
            Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BLDAT 
			, Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BUDAT 
			, ' ' As XBLNR 
			, w_So.so_total_fulfill_qty As BKTXT 
			, ' ' As FRBNR 
			--, case when type_text='RSO' then 'MB01' else 'MB1B' END As TCODE 
			, 'MB01' As TCODE 
			, left( w_Sod.item_number ,18) As MATNR 
			, w_So.bill_to_code As WERKS 
			, '0001' As LGORT 
			, '101' as BWART
			, w_Sod.qty_shipped As ERFMG , 'EA' As ERFME 
			, left(w_So.display_order_number ,10) As EBELN 
			, dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As EBELP 
			, ' ' As ELIKZ 
			, ' ' As SGTXT 
			, 'B' As KZBEW 
			, ' ' As VLIEF_AVIS 
			, REPLICATE(' ',6) As VBELP_AVIS
			From w_So with (nolock)
			Left join w_Sod with (nolock)
			ON w_So.client_code = w_Sod.client_code 
			And w_So.erp_site = w_Sod.erp_site 
			And w_So.display_order_number = w_Sod.display_order_number 
			Left join h_Sod_Imp with (nolock)
			ON w_Sod.client_code = h_Sod_Imp.client_code 
			And w_Sod.erp_site = h_Sod_Imp.erp_site 
			And w_Sod.display_order_number = h_Sod_Imp.display_order_number
			And w_Sod.Display_order_line_number = h_Sod_Imp.Display_order_line_number
		    where w_So.client_code = @Client_Code 
			And w_So.display_order_number = @OutPutKey 
			And w_Sod.qty_shipped !=0
			
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
	--KL01 End
    If @InfMod = 'EX07_PO' --在庫型EX07收貨
    Begin
        Print @InfMod
        Select 
            Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()), 112) As BLDAT , Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()), 112) As BUDAT , 
            IsNull(left(vendor_so_number,16),'') As XBLNR , w_Po.po_total_qty As BKTXT , isnull(left(w_Po.vendor_so_number_2,16),' ') As FRBNR , 
            'MB01' As TCODE , Right('00000'+w_Pod.item_number  ,18) As MATNR , w_Po.erp_site As WERKS , lot_number_3 As LGORT , '101' As BWART ,
            w_Pod.qty_received As ERFMG , 'EA' As ERFME , case when w_Po.Erp_type_text = 'ZEL' then ' ' else left(w_Po.display_po_number ,10) end As EBELN , 
            case when w_Po.Erp_type_text = 'ZEL' then ' ' else Right(w_Pod.line_number ,5) end As EBELP , ' ' As ELIKZ ,
            left(w_Po.display_po_number,10)+'     '+cast(isnull(w_Po.vendor_total_qty,0) as varchar(5))+'/'+cast(isnull(w_Po.vendor_case_count ,0) as varchar(4))+'  ' As SGTXT , 
            'B' As KZBEW , case when w_Po.Erp_type_text in ('ZEL','Z68') then left(w_Po.display_po_number,10) else ' ' end As VLIEF_AVIS , 
            case when w_Po.Erp_type_text in ('ZEL','Z68') then Right(w_Pod.line_number,6) else '' end As VBELP_AVIS , qty - qty_received As MENGE
        From w_Po Left Join w_Pod On
                w_Po.client_code = w_Pod.client_code And w_Po.erp_site = w_Pod.erp_site And
                w_Po.display_po_number = w_Pod.display_po_number
        where w_Po.client_code = @Client_Code And w_Po.display_po_number = @OutPutKey And w_Pod.qty_received <> 0        
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX07_GI' --在庫型EX07收貨
    Begin
        Print @InfMod
        Select 
            Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()), 112) As BLDAT , Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()), 112) As BUDAT , 
            ' ' As XBLNR , w_Po.po_total_qty As BKTXT , ' ' As FRBNR , 
            'MB01' As TCODE , Right('00000'+w_Pod.item_number  ,18) As MATNR , w_Po.erp_site As WERKS , 
			CASE 
			WHEN w_Po.erp_site = '1300' THEN ' ' 
			ELSE lot_number_3
			END As LGORT , 
			--20220511_Raines
			--lot_number_3 As LGORT , 
			'101' As BWART ,
            w_Pod.qty_received As ERFMG , 'EA' As ERFME , 
			--20220614_Raines 
			CASE When left(w_Pod.display_po_number,3) = '493' then w_Pod.ref_po_number
			ELSE w_Pod.display_po_number
			END As EBELN , 
			Right(w_Pod.ref_line_number,5)  As EBELP ,
            ' ' As ELIKZ , ' ' As SGTXT , 'B' As KZBEW , ' ' As VLIEF_AVIS , ' ' As VBELP_AVIS , qty - qty_received As MENGE
        From w_Po Left Join w_Pod On
                w_Po.client_code = w_Pod.client_code And w_Po.erp_site = w_Pod.erp_site And
                w_Po.display_po_number = w_Pod.display_po_number
        where w_Po.client_code = @Client_Code And w_Po.display_po_number = @OutPutKey And w_Pod.qty_received <> 0        
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX07_SN' --SN收貨
    Begin
        Print @InfMod
        Select 
            Convert(varchar(8) , Isnull(w_Po.Cdate,Getdate()), 112) As BLDAT , Convert(varchar(8) , Isnull(w_Po.Rdate,Getdate()),112) As BUDAT , 
            ' ' As XBLNR , w_Po.po_total_qty As BKTXT , ' ' As FRBNR , 
            'MB01' As TCODE , Right('00000'+w_Pod.item_number  ,18) As MATNR , w_Po.erp_site As WERKS , lot_number_3 As LGORT , '101' As BWART ,
            w_Pod.qty_received As ERFMG , 'EA' As ERFME , w_Pod.ref_po_number  As EBELN , Right(w_Pod.ref_line_number,5)  As EBELP ,
            ' ' As ELIKZ , ' ' As SGTXT , 'B' As KZBEW , ' ' As VLIEF_AVIS , ' ' As VBELP_AVIS , qty - qty_received As MENGE
        From w_Po Left Join w_Pod On
                w_Po.client_code = w_Pod.client_code And w_Po.erp_site = w_Pod.erp_site And
                w_Po.display_po_number = w_Pod.display_po_number
        where w_Po.client_code = @Client_Code And w_Po.display_po_number = @OutPutKey And w_Pod.qty_received <> 0        
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX05' --EX05退供單建立
    Begin
        Print @InfMod
        Select 
            Convert(varchar(8) , Isnull( w_So.Adddate ,Getdate()), 112) As DOC_DATE , 'ZR02' As DOC_TYPE , 'F' As DOC_CAT ,
		    Case When w_So.erp_site in ('F001') then 'EADB' when w_So.erp_site in ('FK01','F002') Then 'EAEX'  Else 'EADB' End As PURCH_ORG ,
		    'X01' As PUR_GROUP , left( ship_to_code ,10) As VENDOR , left(w_So.display_order_number ,10) As PO_NUMBER ,
		    dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As PO_ITEM , left( item_number ,18) As  PUR_MAT , '0004' As STORE_LOC ,
		    w_So.erp_site As PLANT , 'X' As RET_ITEM , Convert(varchar(8) , Isnull( w_So.Adddate ,Getdate()), 112) As DELIV_DATE ,
		    cast(qty as varchar(15)) As QUANTITY , 'PI' As PARTNERDESC , 'M' As LANGU , left(ship_to_code,10) As BUSPARTNO
        From w_So Left join w_Sod ON w_So.client_code = w_Sod.client_code And w_So.erp_site = w_Sod.erp_site And w_So.display_order_number = w_Sod.display_order_number 
		    where w_So.client_code = @Client_Code And w_So.display_order_number = @OutPutKey And qty !=0
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX06' --EX05退供單建立
    Begin
        Print @InfMod
        -- TODO 倉別要調整 401（0001）、415（0001）、402（0004）不一樣
		IF exists(select *   From w_So where display_order_number = @OutPutKey )
		Begin
			Select 
				Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BLDAT , Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BUDAT ,
				' ' As XBLNR , w_So.so_total_fulfill_qty As BKTXT , ' ' As FRBNR , 'MB01' As TCODE , left( item_number ,18) As MATNR ,
				w_So.erp_site As WERKS ,
				case when w_Sod.lot_number_3='' then '0001' else w_Sod.lot_number_3 end  As LGORT ,
				'101' As BWART , w_Sod.qty_shipped As ERFMG , 'EA' As ERFME , 
				left(w_So.display_order_number ,10) As EBELN , dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As EBELP , ' ' As ELIKZ , 
				' ' As SGTXT , 'B' As KZBEW , ' ' As VLIEF_AVIS , REPLICATE(' ',6) As VBELP_AVIS
			From w_So Left join w_Sod ON w_So.client_code = w_Sod.client_code And w_So.erp_site = w_Sod.erp_site And w_So.display_order_number = w_Sod.display_order_number 
				where w_So.client_code = @Client_Code And w_So.display_order_number = @OutPutKey And qty_shipped !=0
		end
		ELSE
		Begin
			Select 
				Convert(varchar(8) , Isnull( w_Po.Cdate ,Getdate()), 112) As BLDAT , Convert(varchar(8) , Isnull( w_Po.Cdate ,Getdate()), 112) As BUDAT ,
				' ' As XBLNR , w_Po.rcv_total_qty As BKTXT , ' ' As FRBNR , 'MB01' As TCODE , left( item_number ,18) As MATNR ,
				w_Po.erp_site As WERKS ,
				case when w_Pod.lot_number_3='' then '0001' else w_Pod.lot_number_3 end  As LGORT ,
				'101' As BWART , w_Pod.qty_received As ERFMG , 'EA' As ERFME , 
				left(w_Po.display_po_number ,10) As EBELN , dbo.FmtNumStr( w_Pod.line_number ,5,0 ) As EBELP , ' ' As ELIKZ , 
				' ' As SGTXT , 'B' As KZBEW , ' ' As VLIEF_AVIS , REPLICATE(' ',6) As VBELP_AVIS
			From w_Po Left Join w_Pod  ON   w_Po.client_code = w_Pod.client_code And w_Po.erp_site = w_Pod.erp_site And
                w_Po.display_po_number = w_Pod.display_po_number
			where w_Po.client_code = @Client_Code And w_Po.display_po_number = @OutPutKey And w_Pod.qty_received <> 0        
  
		end
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX06R' --EX06RB2B迴轉
    Begin
        Print @InfMod
        Select 
            Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BLDAT , Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As BUDAT ,
            ' ' As XBLNR , w_So.so_total_fulfill_qty As BKTXT , ' ' As FRBNR , 'MB01' As TCODE , left( item_number ,18) As MATNR ,
            w_So.erp_site As WERKS , ' ' As LGORT , '102' As BWART , w_Sod.qty_diff As ERFMG , 'EA' As ERFME , 
            left(w_So.display_order_number ,10) As EBELN , dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As EBELP , ' ' As ELIKZ , 
            ' ' As SGTXT , 'B' As KZBEW , ' ' As VLIEF_AVIS , REPLICATE(' ',6) As VBELP_AVIS
        From w_So Left join w_Sod ON w_So.client_code = w_Sod.client_code And w_So.erp_site = w_Sod.erp_site And w_So.display_order_number = w_Sod.display_order_number 
		    where w_So.client_code = @Client_Code And w_So.display_order_number = @OutPutKey 
			And qty_diff !=0
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX08' --EX08DN單過帳
    Begin
        Print @InfMod
        Select 
            w_So.ship_to_code + Convert(varchar(6) , Isnull( w_So.Cdate ,Getdate()), 12) As PARTNER_ID , '1' As TANUM  ,
            Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As KODAT , 'X' As KOMUE , 'X' As WABUC , Convert(varchar(8) , Isnull( w_So.Cdate ,Getdate()), 112) As WADAT_IST , 
            left(w_So.display_order_number ,10) As VBELN_VL , dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As POSNR_VL ,
            Case when w_So.erp_site = 'F001' Then w_So.ship_to_code + Convert(varchar(6) , Isnull( w_So.Cdate ,Getdate()), 12) else left(w_So.display_order_number ,10) end  As VBELN ,
            dbo.FmtNumStr( w_sod.Display_order_line_number ,5,0 ) As POSNN , qty_shipped As PIKMG , left( item_number ,18) As MATNR , 'ZRD1' As APDa
        From w_So inner join w_Sod ON w_So.client_code = w_Sod.client_code And w_So.erp_site = w_Sod.erp_site And w_So.display_order_number = w_Sod.display_order_number 
		    where w_So.client_code = @Client_Code And w_So.display_order_number = @OutPutKey --And qty_shipped !=0
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX08_Case' --EX08出口過帳
    Begin
        Print @InfMod
		Declare @erpsite nvarchar(20)
		Select top 1 @erpsite = erp_site from w_Case_S where w_Case_S.client_code = @Client_Code And w_Case_S.shipment_id = @OutPutKey 

		If @erpsite = 'F003'
			Begin
				If Left(@OutPutKey,2) in ('JP','CN')
					Begin
						print 'F003 JP or CN'

						/*20230807 by JackyHsu (JH02)*/
						Select 
							shipment_id As PARTNER_ID 
							, '3PstoDN' As TANUM 
							, Convert(varchar(8) , Isnull( w_Case_S.actual_ship_date ,Getdate()), 112) As KODAT 
							, 'X' As KOMUE 
							, 'X' As WABUC 
							, Convert(varchar(8) , Isnull( w_Case_S.actual_ship_date ,Getdate()), 112) As WADAT_IST 
							--, w_Cased_S.po_number As VBELN_VL --(JH02)
							--, dbo.FmtNumStr( w_Cased_S.po_line_number ,6,0 ) As POSNR_VL --(JH02)
							, w_pod.ref_po_number As VBELN_VL --書
							, dbo.FmtNumStr(  w_pod.ref_line_number ,6,0 ) As POSNR_VL --書
							--, w_Xsod.order_number As VBELN_VL  --非書
							--, dbo.FmtNumStr(  w_Xsod.order_line_number ,6,0 ) As POSNR_VL  --非書
							, shipment_id As VBELN 
							, dbo.FmtNumStr((ROW_NUMBER() OVER(order by max(po_number)) )  ,6,0) As POSNN 
							, sum(w_Cased_S.qty_packed) As PIKMG 
							, '00000'+w_Cased_S.display_item_number  As MATNR 
							, w_Case_S.erp_site  As APDa
						From w_Case_S 
						Left Join w_Cased_S 
						On w_Case_S.client_code = w_Cased_S.client_code 
						And w_Case_S.container_id = w_Cased_S.container_id 
						inner Join w_pod (nolock) 
						on w_Cased_S.client_code = w_pod.client_code 
						and w_Cased_S.po_number = w_pod.display_po_number
						and w_Cased_S.po_line_number = w_pod.line_number
						left join W_Xsod (nolocK)
						on w_pod.client_code = W_XSOD.client_code
						and w_Pod.erp_site = W_XSOD.erp_site
						and w_Pod.line_number = w_Xsod.po_line_number
						and w_Pod.display_po_number = w_Xsod.cust_po_number

						Where w_Case_S.client_code = @Client_Code 
						And w_Case_S.shipment_id = @OutPutKey
						--Group By  shipment_id , actual_ship_date , po_number , po_line_number , w_Case_S.erp_site , w_Cased_S.display_item_number  --(HJ010
						--Group By  shipment_id , actual_ship_date , po_number , po_line_number , w_Case_S.erp_site , w_Cased_S.display_item_number  --(JH02
						Group By  shipment_id , actual_ship_date ,w_pod.ref_po_number ,w_pod.ref_line_number , w_Case_S.erp_site , w_Cased_S.display_item_number   --書
						--Group By  shipment_id , actual_ship_date ,w_Xsod.order_number ,w_Xsod.order_line_number , w_Case_S.erp_site , w_Cased_S.display_item_number   --非書

						Set @RowCount = @RowCount + @@ROWCOUNT 
					End
				Else
					Begin
						Select 
							shipment_id As PARTNER_ID , '3PstoDN' As TANUM , 
							Convert(varchar(8) , Isnull( w_Case_S.actual_ship_date ,Getdate()), 112) As KODAT , 'X' As KOMUE , 'X' As WABUC , 
							Convert(varchar(8) , Isnull( w_Case_S.actual_ship_date ,Getdate()), 112) As WADAT_IST , w_Cased_S.po_number As VBELN_VL , 
							dbo.FmtNumStr( w_Cased_S.po_line_number ,6,0 ) As POSNR_VL , shipment_id As VBELN ,
							dbo.FmtNumStr((ROW_NUMBER() OVER(order by max(po_number)) )  ,6,0) As POSNN , w_Cased_S.qty_packed As PIKMG , 
							'00000'+w_Cased_S.display_item_number  As MATNR , w_Case_S.erp_site  As APDa
						From w_Case_S Left Join w_Cased_S On w_Case_S.client_code = w_Cased_S.client_code And w_Case_S.container_id = w_Cased_S.container_id 
							Where w_Case_S.client_code = @Client_Code And w_Case_S.shipment_id = @OutPutKey
						--Group By shipment_id , actual_ship_date , po_number , po_line_number , w_Case_S.erp_site , w_Cased_S.qty_packed , w_Cased_S.display_item_number 
						Group By  w_Case_S.container_id,shipment_id , actual_ship_date , po_number , po_line_number , w_Case_S.erp_site , w_Cased_S.qty_packed , w_Cased_S.display_item_number  --(HJ010
						Set @RowCount = @RowCount + @@ROWCOUNT 
					End
			End
		Else
			Begin
				Select 
					shipment_id As PARTNER_ID , '3PstoDN' As TANUM , 
					Convert(varchar(8) , Isnull( w_Case_S.actual_ship_date ,Getdate()), 112) As KODAT , 'X' As KOMUE , 'X' As WABUC , 
					Convert(varchar(8) , Isnull( w_Case_S.actual_ship_date ,Getdate()), 112) As WADAT_IST , w_Cased_S.order_number As VBELN_VL , 
					dbo.FmtNumStr( w_Cased_S.order_line_number ,6,0 ) As POSNR_VL , shipment_id As VBELN ,
					dbo.FmtNumStr((ROW_NUMBER() OVER(order by max(order_number)) )  ,6,0) As POSNN , w_Cased_S.qty_packed As PIKMG , 
					'00000'+w_Cased_S.display_item_number  As MATNR , w_Case_S.erp_site  As APDa
				From w_Case_S Left Join w_Cased_S On w_Case_S.client_code = w_Cased_S.client_code And w_Case_S.container_id = w_Cased_S.container_id 
					Where w_Case_S.client_code = @Client_Code And w_Case_S.shipment_id = @OutPutKey
				Group By shipment_id , actual_ship_date , order_number , order_line_number , w_Case_S.erp_site , w_Cased_S.qty_packed , w_Cased_S.display_item_number 
				Set @RowCount = @RowCount + @@ROWCOUNT 
			End

    End
    If @InfMod = 'BX11' --20211224_ken
    Begin
        Print @InfMod

			/*補商品主檔20230814 by JackyHsu start JH03*/

			IF OBJECT_ID('tempdb.dbo.#temp') IS NOT NULL
			BEGIN
				DROP TABLE #temp
			END


			select distinct   po_number,erp_site
			into #temp
			from w_cased_s (nolock) 
			where container_id in (select distinct container_id from w_case_s (nolocK) where shipment_id = @OutPutKey)
			and isnull(po_number,'') <> ''



			DECLARE @po_number nvarchar(30)
				,@erp_site nvarchar(10)
				,@rtn_code NVarchar(100) , @rtn_message NVarchar(100) 

			While (1=1)
				begin
					if not EXISTS (select top 1 * from #temp)
					begin 
						break
					end

					select top 1  @po_number = po_number,@erp_site = erp_site from #temp
				
					/*處理*/
					Exec WMS_Inbound_Cheack_Sku_Sub 
					@erp_site , 
					'ESL' , 
					'ESL' , 
					@po_number , 
					'BX11_補檔' , 
					@rtn_code OutPut , 
					@rtn_message OutPut
					delete #temp where po_number = @po_number
			end

			/*補商品主檔20230814 by JackyHsu end*/

            IF OBJECT_ID('tempdb.dbo.#_bx11') IS NOT NULL
            BEGIN
                    DROP TABLE #_bx11
            END

           SELECT
		               BSART='ZPCN', 
                       BELNR= ch.container_id ,
		               ORGUID1= ch.shipment_id  ,  						 
		               ORGUID2=ch.erp_site , 
		               DATUM=convert(nvarchar(10),ch.actual_export_date,112) ,
		               PARVW='CN' ,						 
		               PARTN= sku.preferred_Y ,--串供應商代碼	M_sku		 
		               POSEX=  dbo.FmtNumStr((ROW_NUMBER() OVER(order by cd.container_id )),5,0)   ,
		               MENGE=  cd.qty_packed  ,
		               MENEE='EA',   
		               NETWR='0.0',
		               MATKL=  RTRIM(sku.category_code)   ,  --串商品分類
		               WERKS=LEFT(ch.ship_to_code,4)  ,						 
		               ABTNR=RTRIM(sku.category_code)   ,--串商品分類
		               --ABRVW= cat.site_zone_code ,	 --串櫃組
		               WMENG=  cd.qty_packed  ,
		               EDATU=convert(varchar(8),getdate(),112),
		               IDTNR= left(cd.display_item_number,18),
                       NOTUSE1=NULL    ,  
		               SOcus_id  =xh.ship_to_code,  --?
                       Scategory = sku.category_code,--串商品分類
                  --     cate_name =cat.site_zone_name,--串櫃組名稱
		               TWPO =xd.cust_po_number ,       --需確認 xd.cust_po_number 
                       POLineNO =right(xd.po_line_number,5),--需確認 xd.po_line_number right 5碼
                       ECMYSTO=xd.order_number ,--需確認   xd.order_number 
                       MYLineNO =right(xd.order_line_number,5),--需確認  xd.order_line_number  right 5碼
                       ECHDSTO =xd.order_number ,--需確認   xd.order_number 
                       HDLineNO =right( xd.order_line_number,5),--需確認   xd.order_line_number  right 5碼
		               SSTO =xd.display_order_number,         --需確認  xd.display_order_number
                       STOLineNO =right( xd.display_order_line_number,5), --需確認  xd.display_order_line_number		
			           QTY=cd.qty_packed,
			           DCKEY1  = xd.sold_to_code,
                       DN = ch.ship_to_code,
                       DESCR =LEFT(cd.item_description_full,30) ,
                       client_code = ch.client_code
                into #_bx11
               from w_case_s(nolock) as ch, w_cased_s(nolock) as cd,m_Sku(nolock) as sku,w_Xso(nolock) as xh,w_Xsod(nolock) xd 
             where ch.container_id = cd.container_id
                 and ch.client_code = cd.client_code
                 and ch.erp_site = cd.erp_site
                 and '00000'+ cd.display_item_number = sku.display_item_number
                 and cd.client_code = sku.client_code
                 and cd.erp_site = sku.erp_site
                 and cd.client_code = xd.client_code
                 and cd.erp_site = xd.erp_site
                 and cd.po_number = xd.cust_po_number
                 and cd.po_line_number = xd.po_line_number
                 and xh.client_code = xd.client_code
                 and xh.display_order_number = xd.Display_order_number
                 and xh.erp_site = xd.erp_site
                 and ch.erp_site in ('F002','F003')
                 and ch.client_code = 'ESL'
                 and ch.shipment_id = @OutPutKey

             Select BSART,BELNR,ORGUID1,ORGUID2,DATUM,PARVW,PARTN,POSEX,MENGE,MENEE,NETWR,MATKL,WERKS,ABTNR,
                        ABRVW = cat.site_zone_code,
                        WMENG,EDATU,IDTNR,NOTUSE1,SOcus_id,Scategory,
                        cate_name =cat.site_zone_name,
                        TWPO,POLineNO,ECMYSTO,MYLineNO,ECHDSTO,HDLineNO,SSTO,STOLineNO,QTY,DCKEY1,DN,DESCR
                from #_bx11
            　   left outer join _t_customer_site_zone(nolock) as cat on cat.customer_code = #_bx11.client_code and cat.site_code = #_bx11.SOcus_id and cat.category_code = #_bx11.MATKL

        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EX11' --20211224_ken修改中
    Begin
        Print @InfMod
		SELECT N'板號	箱號	26碼	書名	ISBN	數量	據點	PO單號	POitem	SN單號	SNitem	箱重量	櫃號	STO單號	STOitem	出口日期	到店日	TXTONLY	海空運'
       
		SELECT 
		 
		 ctnm.hu_id AS '板號'
		, ctnm.carton_number AS '箱號'
		, ctnd.display_item_number AS '[26碼]'
		, ctnd.item_description_full AS '書名'
		, ISNULL(ISNULL(ctnd.isbn,ctnd.ean),ctnd.display_item_number) AS 'ISBN'
		, ISNULL(ctnd.qty_packed, 0) - ISNULL(ctnd.qty_unpack, 0) AS '數量'
		, ctnm.ship_to_code AS '據點'
		, ctnd.display_po_number AS 'PO單號'
		, RIGHT(REPLICATE('0', 5) + CAST(ctnd.po_line_number as NVARCHAR), 5) as 'POitem'
		--2022/03/30 Raines
		--SN單號 for 蘇州 >088外購單號
		--SN單號 for 日本 >1995誠生單號
		--SN單號 for 香港 >等於PO單
		, ctnd.display_po_number AS 'SN單號' 
		, RIGHT(REPLICATE('0', 5) + CAST(ctnd.po_line_number as NVARCHAR), 5) as 'SNitem'

		, ISNULL(ctnm.carton_weight, 0) AS '箱重量'
		, N'' AS '櫃號'
		, case when left(ctnm.ship_to_code,1) = 'E' and left(ctnd.display_po_number,3) = '504' then ctnd.display_po_number
		else ctnd.display_order_number end AS 'STO單號' 
		, case when left(ctnm.ship_to_code,1) = 'E' and left(ctnd.display_po_number,3) = '504' then ctnd.po_line_number
		else ctnd.order_line_number end AS 'STOitem'
		, CONVERT(NVARCHAR(20), ctnm.export_dt, 112) AS '出口日期'
		, CONVERT(NVARCHAR(20), ctnm.delivery_dt, 112) AS '到店日'
		, IIF(ISNULL(ctnd.display_item_number, N'') = N'', N'X', N'') AS txt_only
		, ctnm.export_via AS '海空運'
		FROM  [HJWMS].AAD.DBO._t_order_carton_detail AS ctnd WITH (NOLOCK), [HJWMS].AAD.DBO._t_order_carton_master AS ctnm WITH (NOLOCK)
		WHERE ctnm.wh_id = N'pz1'
		and ctnm.wh_id = ctnd.wh_id  
		and ctnm.carton_number = ctnd.carton_number
		AND ctnm.export_id = @OutPutKey
		AND ISNULL(ctnd.qty_packed, 0) - ISNULL(ctnd.qty_unpack, 0) > 0
		ORDER BY  ctnm.hu_id , ctnm.carton_number , ctnd.line_number , ctnd.item_number
	   
	   --Mark by Raines 20220329
	   -- SELECT
       --             ctnm.hu_id AS '板號'
       --            , ctnm.carton_number AS '箱號'
       --            , ctnd.display_item_number AS '[26碼]'
       --            , ctnd.item_description_full AS '書名'
       --            , ctnd.isbn AS 'ISN'
       --            , ISNULL(ctnd.qty_packed, 0) - ISNULL(ctnd.qty_unpack, 0) AS '數量'
       --            , ctnm.ship_to_code AS '據點'
       --            , od.ref_order_number AS 'PO單號' --w_xsod.order_number 
				   --, RIGHT(REPLICATE('0', 5) + CAST(od.ref_order_line_number as NVARCHAR), 5) as 'POitem'
       --            --, od.ref_order_line_number AS 'POitem'
       --            , od.ref_po_number AS 'SN單號' -- w_xsod.cust_po_number 
				   --, RIGHT(REPLICATE('0', 5) + CAST(od.ref_po_line_number as NVARCHAR), 5) as 'SNitem'
       --            --, od.ref_po_line_number AS 'SNitem'
       --            , ISNULL(ctnm.carton_weight, 0) AS '箱重量'
       --            , N'' AS '櫃號'
       --            , od.display_order_number AS 'STO單號' --w_xsod.display_order_number
       --            , od.line_number AS 'STOitem'
       --            , CONVERT(NVARCHAR(20), ctnm.export_dt, 112) AS '出口日期'
       --            , CONVERT(NVARCHAR(20), ctnm.delivery_dt, 112) AS '到店日'
       --            , N'' AS 'TXTONLY'
       --            , ctnm.delivery_user AS '海空運'
       --     FROM [HJWMS].AAD.DBO._t_order_carton_detail AS ctnd WITH (NOLOCK),[HJWMS].AAD.DBO._t_order_carton_master AS ctnm WITH (NOLOCK),
       --                [HJWMS].AAD.DBO._tv_order_detail as od WITH (NOLOCK)
       -- WHERE ctnm.wh_id = N'pz1'
       --       and ctnm.wh_id = ctnd.wh_id  
       --       and ctnm.carton_number = ctnd.carton_number
       --       --and ctnm.display_order_number = ctnd.display_order_number
       --       and ctnm.Display_PO_number = od.ref_po_number
       --       and right(ctnd.line_number,5) =  right(od.ref_po_line_number,5)
       --      AND ctnm.export_id = @OutPutKey
       --       AND ISNULL(ctnd.qty_packed, 0) - ISNULL(ctnd.qty_unpack, 0) > 0
       --  ORDER BY  ctnm.hu_id , ctnm.carton_number , ctnd.line_number , ctnd.item_number

        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'Ex12' --調倉轉出
    Begin
        Print @InfMod
        --Select 
        --    Convert(varchar, Getdate(), 111) As BLDAT , Convert(varchar, Getdate(), 111) As BUDAT , 
        --    Out1.erp_site_New As WERKS , Out1.display_item_number As MATNR , Out2.erp_wh_New AS LGORT1 , Out1.erp_wh_New As LGORT2 , 
        --    Out1.tran_qty As ERFMG , 'EA' As ERFME , 'MB1B' As TCODE , '311' As BWART           
        --From 
        --    h_WMS_Adg_Out As Out1 Left Join h_WMS_Adg_Out As Out2 
        --        On Out1.control_number = Out2.control_number And Out1.location_id = Out2.location_id 
        --            And Out1.display_item_number = Out2.display_item_number And ABS(Out1.tran_qty) = ABS(Out2.tran_qty) 
        --Where Out1.client_code = @Client_Code  And Out1.control_number = @OutPutKey And Out1.tran_qty > 0 And Out2.tran_qty < 0 
        
        --舊格式
        --Select 
        --    Convert(varchar, tran_time , 111) As BLDAT , Convert(varchar, tran_time, 111) As BUDAT , 
        --    erp_site_New As WERKS , display_item_number As MATNR , erp_wh_New AS LGORT1 , erp_wh_Old As LGORT2 , 
        --    tran_qty As ERFMG , 'EA' As ERFME , 'MB1B' As TCODE , '311' As BWART -- 移倉別311，移據點301          
        --From 
        --    h_WMS_Adg_Out 
        --Where client_code = @Client_Code And control_number = @OutPutKey        
        --舊格式

        Select
            Right(control_number,10) As  mblnr , Convert(varchar(4), tran_time , 111) As mjahr , 
            Right(line_number,4) As zeile , 
            Replace(Convert(varchar, tran_time , 111),'/','') As BLDAT , Replace(Convert(varchar, tran_time, 111),'/','') As BUDAT , 
            isnull(comment_text,'') As xblnr , isnull(Ex1,'') As bktxt, display_item_number As MATNR ,--2021/02/23增加 參考文件號碼16、文件表頭內文25
			--'' As xblnr , '' As bktxt , display_item_number As MATNR ,
            erp_site_Old As WERKS ,  erp_wh_Old AS lgort , erp_site_New As umwrk , erp_wh_New As umlgo , 
            -- WERKS 轉出SITE 、 lgort 轉出倉別 ，umwrk轉入SITE、umlgo 轉入倉別
            tran_qty As ERFMG , 'EA' As ERFME           
        From 
            h_WMS_Adg_Out 
        Where client_code = @Client_Code And Sfile = @OutPutKey 
        
        Set @RowCount = @RowCount + @@ROWCOUNT        
    End
    --20220513_ken
	--20220520_ken
	If @InfMod = 'EDL_PO' --Erp_site：F開頭，採購單轉入B2B 狀態:未確認  第一個檔回去建單
    Begin
        Print @InfMod
  　　
　　　　IF OBJECT_ID('tempdb.dbo.#sku') IS NOT NULL
　　　　BEGIN
               DROP TABLE #sku
　　　　END 

　　　　Select Right(w_Pod.item_number , 13) As SKU , m_Sku.item_description ,ean , 
　　　　m_Sku.item_number_origin
　　　　into #sku
　　　　  From w_Pod(nolock), m_Sku(nolock)
　　　  where w_Pod.client_code = m_sku.client_code　
　　　　   And Right(Rtrim(w_Pod.item_number),13) = Right(Rtrim(m_Sku.display_item_number),13) 
　　　　   and w_Pod.client_code = @Client_Code And w_Pod.display_po_number = @OutPutKey 		

		--頭檔 --(LH02)
   --     Select 
   --         '00001' AS 'NO' , 
			--'H1' AS 'H' , 
			--erp_site As Storerkey , 
			--display_po_number As externpokey , 
   --         po_total_qty As Pcs , po_item_count As item , '19' As  etyps , Convert (varchar(30), Adddate , 120) As  adate , Convert (varchar(30), Editdate ,120) As  edate ,
   --         Convert (varchar(30), isnull(rdate,getdate()) , 120) As rdate , Convert (varchar(30), cdate ,120) As cdate , w_Po.vendor_code As vendor , rcv_total_qty As actqty , Convert (varchar(30), create_date ,120) As pdate , 
   --         vendor_so_number As VPO, vendor_case_count As caqty , vendor_total_qty As dpqty , Convert (varchar(30) , rdate ,120) As udate , '19' As ptype ,
   --         Convert (varchar(30), rdate ,120 ) As edvdate , '' As cfdate , '' As ReadType , '' As SP_ReadType ,substring(m_Storer_V.vendor_name_full,0,8) As vendorName ,
   --         '' As cfwho, '' As cf_editwho, '' As cf_editdate, Convert (varchar(30), Getdate(),120) As TransDate
   --     From w_Po(nolock) Left Join m_Storer_V(nolock) On w_Po.client_code = m_Storer_V.client_code And w_Po.vendor_code = m_Storer_V.vendor_code 
   --         Where w_Po.client_code = @Client_Code And display_po_number = @OutPutKey 
        --明細    
        --Select 
        --    '00001' AS 'NO', 'D1' AS 'D' , w_Pod.erp_site As Storerkey , w_Pod.display_po_number As externpokey ,
        --    w_Po.vendor_code As HOSTREFERENCE , Right(w_Pod.line_number , 5) As EXTERNLINENO , Right(w_Pod.item_number , 13) As SKU , 
        --    qty As oqty , Isnull(qty_received,0) As actqty , substring(m_Sku.item_description,0,39) As Descr , ean As BUSR2 , 'S' As E_type ,
        --    0 As ADDs , qty - Isnull(qty_received,0) As ODDs , qty_diff_barcode As Code_err , qty_diff_noean As Non_EAN ,
        --    qty_diff_stain As Water , qty_diff_damaged As Damage , qty_diff_misc As Other , diff_misc_description As Other_Res ,
        --    diff_memo As Notes , qty - Isnull(qty_received,0) As DiffQty , m_Sku.item_number_origin As RETAILSKU
        --From w_Pod(nolock) 
        --    Left Join w_Po(nolock) On w_Pod.client_code = w_Po.client_code And w_Pod.erp_site = w_Po.erp_site And w_Po.display_po_number = w_Pod.display_po_number
        --    Left Join m_Sku(nolock) On w_Pod.client_code = m_sku.client_code And w_Pod.erp_site = m_Sku.erp_site 
        --        And Right(w_Pod.item_number,13) = Right(m_Sku.display_item_number,13) 
        --    Where w_Pod.client_code = @Client_Code And w_Pod.display_po_number = @OutPutKey 
		SELECT  --LH02
/* 1*/           '00001' AS 'NO' 
/* 2*/           , 'H1' AS 'H' 
/* 3*/           , erp_site As Storerkey 
/* 4*/           , display_po_number As externpokey 
/* 5*/           , po_total_qty As Pcs 
/* 6*/           , po_item_count As item 
/* 7*/           , '19' As  etyps 
/* 8*/           , Convert (varchar(30), Adddate , 120) As  adate 
/* 9*/           , Convert (varchar(30), Editdate ,120) As  edate 
/*10*/           , Convert (varchar(30), cdate ,120) As cdate 
/*11*/           , Convert (varchar(30), isnull(rdate,getdate()) , 120) As rdate 
/*12*/           , w_Po.vendor_code As vendor 
/*13*/           , rcv_total_qty As actqty 
/*14*/           , Convert (varchar(30), create_date ,120) As pdate 
/*15*/           , vendor_so_number As VPO
/*16*/           , vendor_case_count As caqty 
/*17*/           , vendor_total_qty As dpqty , Convert (varchar(30) 
/*18*/           , rdate ,120) As udate 
/*19*/           , '19' As ptype 
/*20*/           , Convert (varchar(30), rdate ,120 ) As edvdate 
/*21*/           , '' As cfdate 
/*22*/           , '' As ReadType 
/*23*/           , '' As SP_ReadType 
/*24*/           ,substring(m_Storer_V.vendor_name_full,0,8) As vendorName 
/*25*/           , '' As cfwho
/*26*/           , '' As cf_editwho
/*27*/           , '' As cf_editdate
/*28*/           , Convert (varchar(30), Getdate(),120) As TransDate
			From w_Po(nolock) Left Join m_Storer_V(nolock) On w_Po.client_code = m_Storer_V.client_code And w_Po.vendor_code = m_Storer_V.vendor_code 
            Where w_Po.client_code = @Client_Code And display_po_number = @OutPutKey

Select 
            '00001' AS 'NO', 'D1' AS 'D' , w_Pod.erp_site As Storerkey , w_Pod.display_po_number As externpokey ,
            w_Po.vendor_code As HOSTREFERENCE , Right(w_Pod.line_number , 5) As EXTERNLINENO , Right(w_Pod.item_number , 13) As SKU , 
            qty As oqty , Isnull(qty_received,0) As actqty , 
			substring((select top 1 item_description 
			                                           from #sku(nolock) 
			                                          where Right(w_Pod.item_number,13) = Right(#sku.sku,13) 
													    and #sku.item_description is not null),0,39) As Descr , 
			(select top 1 ean 
			                                           from #sku(nolock) 
			                              　        　where Right(w_Pod.item_number,13) = Right(#sku.sku,13)
										  　　　　　　　and #sku.ean is not null) As BUSR2 , 
			'S' As E_type ,
            0 As ADDs , qty - Isnull(qty_received,0) As ODDs , qty_diff_barcode As Code_err , qty_diff_noean As Non_EAN ,
            qty_diff_stain As Water , qty_diff_damaged As Damage , qty_diff_misc As Other , diff_misc_description As Other_Res ,
            diff_memo As Notes , qty - Isnull(qty_received,0) As DiffQty , 
			(select top 1 item_number_origin 
			                                           from #sku(nolock) 
			                              　          where Right(w_Pod.item_number,13) = Right(#sku.sku,13)
										  　　　　　　　and #sku.item_number_origin is not null) As RETAILSKU
        From w_Pod(nolock) 
            Left Join w_Po(nolock) On w_Pod.client_code = w_Po.client_code And w_Pod.erp_site = w_Po.erp_site And w_Po.display_po_number = w_Pod.display_po_number           
            Where w_Po.client_code = @Client_Code And w_Pod.display_po_number = @OutPutKey         
		
		
		
		
		
		
		Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EDL_PO_C' --Erp_site：F開頭，採購單轉入B2B etyps=20已確認
    Begin
        Print @InfMod
        --頭檔
        Select 
            '00001' AS 'NO' , 'H1' AS 'H' , erp_site As Storerkey , display_po_number As externpokey , 
            po_total_qty As Pcs , po_item_count As item , '20' As  etyps , Convert (varchar(30), Adddate , 120) As  adate , Convert (varchar(30), Editdate ,120) As  edate ,
            Convert (varchar(30), isnull(rdate,getdate()) , 120) As rdate , Convert (varchar(30), cdate ,120) As cdate , w_Po.vendor_code As vendor , rcv_total_qty As actqty , Convert (varchar(30), create_date ,120) As pdate , 
            vendor_so_number As VPO, vendor_case_count As caqty , vendor_total_qty As dpqty , Convert (varchar(30) , rdate ,120) As udate , '19' As ptype ,
            Convert (varchar(30), rdate ,120 ) As edvdate , '' As cfdate , '' As ReadType , '' As SP_ReadType , m_Storer_V.vendor_name As vendorName , 
            '' As cfwho, '' As cf_editwho, '' As cf_editdate, Convert (varchar(30), Getdate(),120) As TransDate
        From w_Po Left Join m_Storer_V On w_Po.client_code = m_Storer_V.client_code And w_Po.vendor_code = m_Storer_V.vendor_code 
            Where w_Po.client_code = @Client_Code And display_po_number = @OutPutKey 
        --明細    
        Select 
            '00001' AS 'NO', 'D1' AS 'D' , w_Pod.erp_site As Storerkey , w_Pod.display_po_number As externpokey ,
            w_Po.vendor_code As HOSTREFERENCE , Right(w_Pod.line_number , 5) As EXTERNLINENO , Right(w_Pod.item_number , 13) As SKU , 
            qty As oqty , Isnull(qty_received,0) As actqty , m_Sku.item_description As Descr , ean As BUSR2 , 'S' As E_type ,
            qty_diff_over As ADDs , qty_diff_under As ODDs , qty_diff_barcode As Code_err , qty_diff_noean As Non_EAN ,
            qty_diff_stain As Water , qty_diff_damaged As Damage , qty_diff_misc As Other , diff_misc_description As Other_Res ,
            diff_memo As Notes , qty_diff As DiffQty , m_Sku.item_number_origin As RETAILSKU
        From w_Pod 
            Left Join w_Po On w_Pod.client_code = w_Po.client_code And w_Pod.erp_site = w_Po.erp_site And w_Po.display_po_number = w_Pod.display_po_number
            Left Join m_Sku On w_Pod.client_code = m_sku.client_code And w_Pod.erp_site = m_Sku.erp_site 
                And Right(w_Pod.item_number,13) = Right(m_Sku.display_item_number,13) 
            Where w_Pod.client_code = @Client_Code And w_Pod.display_po_number = @OutPutKey 
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'EDL_RTV' --Erp_site：F開頭，退貨單轉入B2B
    Begin
        Print @InfMod
        --Select 
        --    '00001' AS 'NO' , 'H1' AS 'H' , w_So.ship_to_code As Cus_ID , display_order_number As EXTERNORDERKEY ,
        --    cust_po_number As BUYERPO , so_total_qty As pcs , so_item_count As item , w_So.Adddate As adate , w_So.Editdate As edate ,
        --    '' As cdate , Cdate As ccdate , Cdate As oddate , Cdate As fdate , Cdate As effdate , m_Storer_V.return_control_code As vdip , w_So.ctyps As ctyps ,
        --    Case When purchase_group_code+substring(industry_code,4,1)+substring(erp_latta ,5,1) IN ('B2B31','B2B11') then '上線;' else '非上線;' End As memo ,
        --    Getdate() As sysdate , '' As Conf_Deadline , 0 As Sup_Check , '' As Sup_Check_Time , 0 As Log_Check , '' AS Log_Check_Time , 1 As CaseSum ,
        --    m_Storer_V.vendor_name As VendorName , erp_site As STORERKEY , '' As WaitRtn_Time
       
       --20211217_ken 修改撈出資料時間格式，避免產檔有上下午
       Select 
            '00001' AS 'NO' , 'H1' AS 'H' , w_So.ship_to_code As Cus_ID , display_order_number As EXTERNORDERKEY ,
            cust_po_number As BUYERPO , so_total_qty As pcs , so_item_count As item , convert(nvarchar(19),w_So.Adddate,121) As adate , convert(nvarchar(19),w_So.Editdate,121) As edate ,
            '' As cdate , convert(nvarchar(19),Cdate,121) As ccdate , convert(nvarchar(19),Cdate,121) As oddate , convert(nvarchar(19),Cdate,121) As fdate , convert(nvarchar(19),Cdate,121) As effdate 
            , m_Storer_V.return_control_code As vdip , '19' As ctyps, -- w_So.ctyps As ctyps ,--<LH01>
            Case When purchase_group_code+substring(industry_code,4,1)+substring(erp_latta ,5,1) IN ('B2B31','B2B11') then '上線;' else '非上線;' End As memo ,
            convert(nvarchar(19),Getdate(),121) As sysdate , '' As Conf_Deadline , 0 As Sup_Check , '' As Sup_Check_Time , 0 As Log_Check , '' AS Log_Check_Time , 1 As CaseSum ,0 as WaitRtn,
            m_Storer_V.vendor_name As VendorName , erp_site As STORERKEY , '' As WaitRtn_Time
        From w_So Left Join m_Storer_V On w_So.client_code = m_Storer_V.client_code And w_So.ship_to_code = m_Storer_V.vendor_code 
       Where w_So.client_code = @Client_Code And w_So.display_order_number = @OutPutKey 
        
        Select 
            '00001' AS 'NO', 'D1' AS 'D' , w_So.ship_to_code As cus_id , w_Cased_S.order_number As externlino , w_Case_S.container_id As pkno ,
            w_Cased_S.display_item_number As pdct_code , w_Cased_S.isbn As ori_code , w_Cased_S.item_description_full As pdct_name ,
            qty_packed As openqty , qty_packed As pack_qty , qty_packed As act_qty , qty_packed As Fix_atqty , '30' As asntype ,
            w_Case_S.shipment_id As ruppult , Cspdate As pkdate , '' As Ustatus , '' As Multi , 0 As Odds , 0 As Damage , 
            0 As Non_Comp , 0 As Non_Sets , 0 As Expired , 0 As Other , '' As Other_Res , '' As Notes , 0 As Short_Qty , 
            '' As Udate , '' As Sup_Inbound , '' As Sup_Return , '' As Rtn_Note_No , '' As type_chk , w_Cased_S.erp_site ,
            w_Cased_S.ID As SN , right(line_number,5) As item_SN
        From w_Case_S(nolock),w_Cased_S(nolock)
            Left Join w_So(nolock) On w_Cased_S.client_code = w_So.client_code And w_Cased_S.erp_site = w_So.erp_site And w_Cased_S.order_number = w_So.display_order_number 
            Left Join m_Sku(nolock) On w_Cased_S.client_code = m_sku.client_code And w_Cased_S.erp_site = m_Sku.erp_site And w_Cased_S.display_item_number = m_Sku.display_item_number 
       Where w_Cased_S.container_id = w_Case_S.container_id
            and w_Cased_S.client_code = @Client_Code
            And w_Cased_S.order_number= @OutPutKey -- 20211217_ken Edit  w_Cased_S.po_number  change to  w_Cased_S.order_number 
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'Ex2Msm' --物流箱明細回傳MSM
    Begin
        Print @InfMod
		--20220105_Raines
	    Select 
		    CH.shipment_id As Shipnumber , CH.container_id As Casenumber , CD.container_id As Packnumber , 
		    CH.actual_container_count As Caits_Check ,
		    Case CH.container_type When 'PO' Then 'Ship' When 'SO' Then 'Ship' When 'ZP' Then 'Ship' When 'ZS' Then 'Ship' Else CH.container_type End As Casetype ,
		    Case CH.container_type When 'PO' Then 'Ship' When 'SO' Then 'Ship' When 'ZP' Then 'Ship' When 'ZS' Then 'Ship' Else CH.container_type End As PackType ,
		    CH.ship_to_code As Hostreference , Case CD.erp_site When 'F001' Then '01DBSDB99' Else CD.erp_site End As Storerkey , 
		    isnull(Case CD.order_number When '' Then '0000000000' Else CD.order_number End , '0000000000' )  As Pokey , 
		    isnull(Case CD.po_number When '' Then '0000000000' Else CD.po_number End , '0000000000') As Orderkey , 
		    isnull(Case CD.po_line_number When '' Then '000000' Else iexp.dbo.FmtNumStr(CD.po_line_number,6,0) End, '000000') As OrderLineNo , 
		    isnull(Case CD.display_item_number When '' Then '000000000000000000' Else Right(CD.display_item_number,13) End,'000000000000000000') As Sku ,
		    isnull(Case Sk.Ean When '' Then '000000000000000000' Else Sk.Ean End,'000000000000000000') As Ean ,  
		    '' As Descr , isnull(Sum(CD.qty_packed ),'0') As Aqty , isnull(CD.EX1 , '') As Receoptnumber , '' As Note
		    From w_Case_S As CH 
                Left Join w_Cased_S As CD On CH.client_code = CD.client_code And CH.container_id = CD.container_id
                Left Join m_Sku As Sk On CD.client_code = Sk.client_code And CD.erp_site = Sk.erp_site And CD.display_item_number = Sk.display_item_number 
		    Where CH.client_code = @Client_Code And CH.shipment_id = @OutPutKey  And CH.container_type  in ('ZS','ZP','PO','SO') And CH.erp_site in('F001','F002')
				  And LEFT(CH.container_id,1) = 'W' --AND CH.erp_site = 'F001'
				  AND CH.EX2MSM_STATUS = 'Ready2ship'
		    Group By 
			    CH.shipment_id , CH.container_id , CD.container_id , CH.container_type , CH.ship_to_code , CD.erp_site , CD.po_number ,
			    CD.order_number , CD.po_line_number , CD.display_item_number , Sk.Ean , CD.EX1 , CH.actual_container_count 
		UNION
		Select 
			CH.shipment_id As Shipnumber , CH.container_id As Casenumber , CD.container_id As Packnumber , 
			CH.actual_container_count As Caits_Check ,
			Case CH.container_type When 'PO' Then 'Ship' When 'SO' Then 'Ship' When 'ZP' Then 'Ship' When 'ZS' Then 'Ship' Else CH.container_type End As Casetype ,
			Case CH.container_type When 'PO' Then 'Ship' When 'SO' Then 'Ship' When 'ZP' Then 'Ship' When 'ZS' Then 'Ship' Else CH.container_type End As PackType ,
			CH.ship_to_code As Hostreference , Case CD.erp_site When 'F001' Then '01DBSDB99' Else CD.erp_site End As Storerkey , 
			isnull(Case CD.po_number When '' Then '0000000000' Else CD.po_number End , '0000000000' )  As Pokey , 
			isnull(Case CD.order_number When '' Then '0000000000' Else CD.order_number End , '0000000000') As Orderkey , 
			isnull(Case CD.order_line_number When '' Then '000000' Else iexp.dbo.FmtNumStr(CD.order_line_number,6,0) End, '000000') As OrderLineNo , 
			isnull(Case CD.display_item_number When '' Then '000000000000000000' Else Right(CD.display_item_number,13) End,'000000000000000000') As Sku ,
			isnull(Case Sk.Ean When '' Then '000000000000000000' Else Sk.Ean End,'000000000000000000') As Ean ,  
			'' As Descr , isnull(Sum(CD.qty_packed ),'0') As Aqty , isnull(CD.EX1 , '') As Receoptnumber , '' As Note
			From w_Case_S As CH 
				Left Join w_Cased_S As CD On CH.client_code = CD.client_code And CH.container_id = CD.container_id
				Left Join m_Sku As Sk On CD.client_code = Sk.client_code And CD.erp_site = Sk.erp_site And CD.display_item_number = Sk.display_item_number 
			Where CH.client_code = @Client_Code And CH.shipment_id = @OutPutKey  And CH.container_type  in ('ZS','ZP','PO','SO') And CH.erp_site in('F001','F002')
				  And LEFT(CH.container_id,1) <> 'W' --AND CH.erp_site = 'F001'
				  AND CH.EX2MSM_STATUS = 'Ready2ship'
			Group By 
				CH.shipment_id , CH.container_id , CD.container_id , CH.container_type , CH.ship_to_code , CD.erp_site , CD.po_number ,
				CD.order_number , CD.order_line_number , CD.display_item_number , Sk.Ean , CD.EX1 , CH.actual_container_count 

        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'RSO_EC_RET' --網路書店RSO介面結案
    Begin
		--  SJOB.dbo.StrArgIdxTag(@Extkeys,1,',') , @OutPutKey = SJOB.dbo.StrArgIdxTag(@Extkeys,2,',')
		PRINT  @Client_Code +'/'+@OutPutKey 
		select 
				ORDER_TYPE='RSO' , 
				E1EDK02_BELNR01= display_po_number ,
				E1EDK02_BELNR02=dbo.StrArgIndex(EX1 ,1)  ,
							--(select   D.ref_po_number  from  iexp..w_Po  as P JOIN  iexp..w_Pod as D  ON P.display_po_number=D.display_po_number) ,
				E1EDK02_BELNR03=dbo.StrArgIndex(EX1 ,2)  ,
				E1EDK02_BELNR10= dbo.StrArgIndex(EX1 ,3)  ,                   --D.EX2 ,
				NOTE1=isnull( comment,'') ,
				NOTE2='' ,
				NOTE3='' , 
				(  select 
						E1EDP02_BELNR01=D.display_po_number ,
						E1EDP02_BELNR02=D.ref_po_number ,
						WERKS=D.erp_site ,
						IDTNR=D.item_number ,
						KTEXT=D.EX5 ,
						POSEX=cast(line_number as int ) ,
						MENGE1=isnull(qty  ,'0') ,
						MENGE2=isnull(qty_received  ,'0') ,
						[STATUS] = case when D.qty_received>0 then '01'  when qty_diff>0 then '02'   when qty_diff_over>0 then '03' when qty_diff_under>0 then '04' else '00' end ,
						ITEMNOTE1 =isnull(D.comment,'') ,
						ITEMNOTE2=isnull(diff_memo,'')  ,
						ITEMNOTE3='' 
					from iexp..w_Po  as P JOIN  iexp..w_Pod as D  ON P.display_po_number=D.display_po_number 
							where   P.client_code = @Client_Code  And   P.display_po_number= @OutPutKey
					FOR XML PATH('ITEM')  , Type
				)  As 'ITEMS'     
		from  iexp..w_Po  where  client_code = @Client_Code  And  display_po_number=@OutPutKey
			FOR XML PATH('ORDER')  
 
        --Select 
        --    So.erp_site + '|' + So.display_order_number + '|' + '0' + '|' + Convert(varchar(16), So.Cdate , 120) + '|' + Case So.customer_code When '7-11' Then Isnull(cs.ex8,'') Else '' End+ '|' + dbo.StrCodeLookup(So.client_code , 'SP19' ,'', So.Erp_type_text , 'VSART' , -1 , So.customer_code )
        --    From Iexp..w_So As So Left Join (Select * From h_So_Out Where Ctyps = 106 And Ex7 = '19') As CS
        --        On So.erp_site = CS.erp_site And So.display_order_number = Replace(CS.display_order_number,'ESL-','') 
        --Where So.Ex8 = @OutPutKey 
    End   



    If @InfMod = 'CODX' -- 網路書店結案回傳。
    Begin
        Print @InfMod

        Select 
            Cdate As [Date] , display_order_number As Shipping_Number , cust_po_number As Order_Number , erp_site As Site ,
            dbo.StrCodeLookup(client_code , 'SP19' ,'', Erp_type_text , 'VSART' , -1 , customer_code ) As VSART ,         
            ( 
            Select 
                w_Sod.display_order_number As Shipping_Number , dbo.FmtNumStr(Cast(w_Sod.order_line_number As Int),6,0) As FlowNumber ,
                w_Cased_S.container_id As Tracking_Number , w_Cased_S.qty_packed As Qty , w_Sod.amount As Price , 
                w_Cased_S.erp_site As Stock_Site , w_Sod.item_number As SequenceNumber , m_Sku.item_description As ProductName
            From w_Cased_S   
                Left Join w_Sod On w_Sod.Display_order_number  = Replace(w_Cased_S.order_number,'ESL-','') And w_Sod.Display_order_line_number   = w_Cased_S.order_line_number 
                Left Join m_Sku On w_Cased_S.client_code = m_Sku.client_code And w_Cased_S.erp_site = m_Sku.erp_site And w_Cased_S.display_item_number = m_Sku.display_item_number  
            Where w_So.client_code = w_Sod.client_code And w_So.display_order_number  = w_Sod.display_order_number
                And w_Sod.item_number Not in ('2000000023007' , '2000000029009') 
            For Xml Path ('Item')  , TYPE ) As Items       
        From w_So 
            Where client_code = @Client_Code  And display_order_number = @OutPutKey 
        For Xml Path ('Shipment') , root('Shipments') 

        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'PickShort' -- 揀缺回告。
    Begin
        Print @InfMod
        Select 
            Right(Replace(Replace(Replace(Replace(Convert(nvarchar(19),Getdate(),120),'-',''),'/',''),':',''),' ',''),12) As [DATE] , 
            display_order_number As OrderNumber , erp_site As Site ,
            ( 
            Select 
                w_Sod.display_order_number As OrderNumber , dbo.FmtNumStr(Cast(w_Sod.order_line_number As Int),6,0) As OrderLineNumber ,
                w_Sod.qty As Oqty , w_Sod.qty_shipped As Aqty , item_number As ItemNumber , m_Sku.item_description As ItemName
            From 
                w_Sod  
                Left Outer Join m_Sku On w_Sod.client_code = m_Sku.client_code And w_Sod.item_number = m_Sku.display_item_number And m_Sku.erp_site = 'F001'
                Where w_Sod.client_code = w_So.client_code And w_Sod.display_order_number  = w_So.display_order_number
                    And w_Sod.item_number Not in ('2000000023007' , '2000000029009') 
            For XML Path('Item') , Type ) As 'Items'              
            From w_So Where client_code = @Client_Code And display_order_number = @OutPutKey 
            For XML Path('Order') , Root('Orders')

        Set @RowCount = @RowCount + @@ROWCOUNT 
    End

	If (@InfMod = 'ECOrderStatus' )
	BEGIN
		Declare @Ctyps  varchar(20)= dbo.StrArgIndex(@Extkeys ,3)
		Declare @orderkey  varchar(20)= dbo.StrArgIndex(@Extkeys ,2)
		PRINT @Ctyps+'/'+ @orderkey
		--IF  @Ctyps in ('90' ,'91')
		--BEGIN
-- 缺DN 要改 imp  ； 結構未拆箱明細  , (待修改)qty_packed AS 'OrderQty'   
			SELECT  CONVERT(VARCHAR(10), h.EDITdate, 112) AS 'DATE'
				,display_order_number AS 'OrderNumber'
				,display_order_number AS 'DnNumber'
				,h.erp_site AS 'Site'
				,type_text AS 'OrderType'
				,COMMENT AS 'Note1' 
				,'' AS 'Note2'
				,'' AS 'Note3'
				,hc.container_id AS 'TrackingNumber'
				,CONVERT(VARCHAR(10), h.EDITdate, 112) AS 'ShipDate'
				,@Ctyps  AS 'Status'
				,'' AS 'TotalPrice'
				,'' AS 'PackagePrice'
				,(
					SELECT display_item_number AS 'SequenceNumber'
						,item_description_full AS 'ProductName'
						,qty_packed AS 'OrderQty'   
						,qty_packed AS 'PackageQty'
						,comment_text AS 'ItemNote1'
						,'' AS 'ItemNote2'
						,'' AS 'ItemNote3'
					FROM h_Cased_S_Out hc
					WHERE hc.order_number = h.display_order_number
					FOR XML Path('Item') ,Type
					) As 'Items'     
			FROM  iexp..w_So h (nolock)  left outer join h_Cased_S_Out (nolock) hc 
				ON h.display_order_number =Replace( hc.order_number ,'ESL-','')  
			-- ON h.display_order_number = hc.order_number    X where h.Ctyps=9 and
				where   h.display_order_number=@orderkey
			group by   display_order_number ,h.erp_site ,type_text ,container_id ,COMMENT , h.EDITdate
			FOR XML PATH('Shipment'), root('Shipments') 

		--end
		--else
		--Begin
		--	SELECT CONVERT(VARCHAR(10), h.Adddate, 112) AS 'DATE'
		--		,display_order_number AS 'OrderNumber'
		--		,display_order_number AS 'DnNumber'
		--		,h.erp_site AS 'Site'
		--		,type_text AS 'OrderType'
		--		,COMMENT AS 'Note1'
		--		,'' AS 'Note2'
		--		,'' AS 'Note3'
		--		,hc.container_label AS 'TrackingNumber'
		--		,CONVERT(VARCHAR(10), h.Adddate, 112) AS 'ShipDate'
		--		,@Ctyps AS 'Status'
		--		,'' AS 'TotalPrice'
		--		,'' AS 'PackagePrice'
		--		,(
		--			SELECT display_item_number AS 'SequenceNumber'
		--				,item_description_full AS 'ProductName'
		--				,'' AS 'OrderQty'
		--				,qty_packed AS 'PackageQty'
		--				,comment_text AS 'ItemNote1'
		--				,'' AS 'ItemNote2'
		--				,'' AS 'ItemNote3'
		--			FROM h_Cased_S_Out hc
		--			WHERE hc.order_number = h.display_order_number
		--			FOR XML Path('Items')
		--				,Type
		--			)
		--	FROM h_so_out h left outer join h_Cased_S_Out hc 
		--	ON h.display_order_number = hc.order_number
		--		where   h.display_order_number=@orderkey
		--	ORDER BY CONVERT(VARCHAR(10), h.Adddate, 112)
		--		,display_order_number
		--	FOR XML PATH('Shipment')
		--end
	END


    If @InfMod = 'OrderStatusOLD' -- 貨態回告。
    Begin
        Print @InfMod
        Select 
            Right(Replace(Replace(Replace(Replace(Convert(nvarchar(19),Getdate(),120),'-',''),'/',''),':',''),' ',''),12) As [DATE] ,
            Case w_So_Status.erp_site When 'G016' Then w_So_Status.Ex1 Else w_So_Status.display_order_number End As OrderNumber , w_So_Status.erp_site As Site ,
            dbo.StrCodeLookup(w_So_Status.client_code , 'SP19' ,'', Erp_type_text , 'VSART' , -1 , customer_code ) As VSART , 
            container_id As BoxNumber , 
            Right(Replace(Replace(Replace(Replace(Convert(nvarchar(19),w_So_Status.Editdate,120),'-',''),'/',''),':',''),' ',''),12) As ShipDate , w_So_Status.Order_Status As Status , 
            Case When w_So_Status.Order_Status = -99 Then Track_Status Else Null End As Err_Status ,
            Case When w_So_Status.Order_Status = -99 Then Track_Status_Note Else Null End As Err_Msg 
        From w_So_Status Left Join w_SoD_Status On w_So_Status.client_code = w_SoD_Status.client_code And w_So_Status.display_order_number = w_SoD_Status.display_order_number 
            Where w_So_Status.client_code  = @Client_Code And w_So_Status.display_order_number = @OutPutKey 
        For XML Path('Order') , Root('Orders')
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
	/****黑貓 Eod出貨檔案。 ********/
	--<20220119_ben>修改地址、代收金額、契客代號
    If @InfMod = 't-cat_Eod'  
    Begin
        Print @InfMod
	    Select     
		    託運類別 = '1' ,
		    託運單號 = container_id ,
		    訂單編號 = H.display_order_number ,
		    契客代號='2795296601' ,						--正式 '2315553131' 2795296601 ,  7076259101
		    溫層 = '0001',			 
		    距離 = Case When H.ship_to_city in('桃園市','台北市','新北市', '基隆市') Then '00' When H.ship_to_city in ('金門縣','連江縣','澎湖縣') Then '02' Else '01' End ,	
		    規格 = '0001',
		    代收 = H.cod_flag , -- N:否  Y:是 EC平台需先付款
		    代收金額 = 
                Case H.cod_flag 
                    When 'N' Then 0 
                    When 'Y' Then Cast (
										(Select sum(Csd.qty_packed*(amount/qty )) From iexp..w_Sod As Sod JOIN iexp..w_Cased_S As Csd 
											ON Sod.display_order_number=Csd.order_number and Sod.Display_order_line_number=Csd.order_line_number 
											And  Csd.container_id = Cs.container_id
										) As Int) 
                    Else 0 End ,   
		    是否到付 = 'N',  
            是否付現 = '01',				 
		    姓名 = '*' +Left(H.ship_to_name,1) ,    
            電話 = '*'   ,
            手機 = '*' +Right(H.ship_to_phone,3) ,
		    郵遞區號 = Case 
			When CHARINDEX('-',  H.ship_to_zip ) >1 Then Right(REPLACE(H.ship_to_zip,'-',''),5) 
			WHEN H.customer_code = 't-cat' Then 
			(select top 1 RIGHT(REPLACE(JSON_VALUE(waybill_data, '$[0].zip_7'),'-',''),6) 
			from [HJWMS].aad.dbo._t_order_carton_master 
			where 1=1 and _t_order_carton_master.display_order_number = H.display_order_number)
			Else H.ship_to_zip End ,
		    --地址 = H.ship_to_address  ,
			--20221208 Raines change
			地址 = H.ship_to_city + H.ship_to_state + H.ship_to_address  ,
		寄件人姓名='誠品線上' , 寄件人電話='03-4907188#321'  ,  
		寄件人手機='' , 
		From區號='37111G' , 
		寄件人地址='桃園市平鎮區雙福路一段65號',
		    P託運單日 = CONVERT(varchar(8),Cs.Adddate,112),
		    預定取件時段 = '4', 
		    預定配達時段 = '4', 
		    會員編號 = '' ,
		    物品名稱 = '' ,
		    易碎物品 = 'N', --前無作用：固定填N	
		    精密儀器 = 'N',
		    備註 = '' , 
            SD路線代碼 = '',
            補column結束=''  --  註2：每個 column結束以│區隔,內容可以為變動長度	 
	    From w_Case_S As Cs Left Outer Join w_So As H On  Cs.client_code = H.client_code  and  Cs.Ex1=H.display_order_number  
		       Where  Cs.customer_code = 't-cat' And Cs.Etyps > 0  And Cs.SFile = @OutPutKey 
			   order by H.display_order_number
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 't-cat_Eod_So' -- 黑貓 Eod出貨檔案。
    Begin
        Print @InfMod
	    Select  
		    託運類別 = '1' ,
		    託運單號 = Rtrim(Hs.Ex8) ,
		    訂單編號 = H.display_order_number ,
		    契客代號='2795296601' ,						--正式 '2315553131' 2795296601 ,  7076259101
		    溫層 = '0001',			 
		    距離 = Case When H.ship_to_city in('桃園市','台北市','新北市', '基隆市') Then '00' When H.ship_to_city in ('金門縣','連江縣','澎湖縣') Then '02' Else '01' End ,	
		    規格 = '0001',
		    代收 = H.cod_flag , -- N:否  Y:是 EC平台需先付款
		    代收金額 = 
                Case H.cod_flag 
                    When 'N' Then 0 
                    When 'Y' Then Cast( h.total_amount As Int) 
                    Else 0 End ,   
		    是否到付 = 'N',  
            是否付現 = '01',				 
		    姓名 = '*' +Left(H.ship_to_name,1) ,    
            電話 = '*'   ,
            手機 = '*' +Right(H.ship_to_phone,3) ,
		    郵遞區號 = Case When CHARINDEX('-',  H.ship_to_zip ) >1 Then Right(REPLACE(H.ship_to_zip,'-',''),5) Else H.ship_to_zip End ,
		    地址 = Isnull(H.ship_to_city,'') + Isnull(H.ship_to_state,'')  + Isnull(H.ship_to_address,'')  ,
		寄件人姓名='誠品線上' , 寄件人電話='03-4907188#321'  ,  
		寄件人手機='' , 
		From區號='324' , 
		寄件人地址='桃園市平鎮區雙福路一段65號',
		    P託運單日 = CONVERT(varchar(8),GetDate(),112),
		    預定取件時段 = '4', 
		    預定配達時段 = '4', 
		    會員編號 = '' ,
		    物品名稱 = '' ,
		    易碎物品 = 'N', --前無作用：固定填N	
		    精密儀器 = 'N',
		    備註 = '' , 
            SD路線代碼 = '',
            補column結束=''  --  註2：每個 column結束以│區隔,內容可以為變動長度	 
	    From h_So_Out As Hs Left Outer Join w_So As H On Hs.display_order_number = H.display_order_number And Hs.client_code = H.client_code 
		       Where  Hs.customer_code = 't-cat' And HS.Efile = @OutPutKey  
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End

    If @InfMod = 'Seven_Sin' -- 711 Sin出貨檔案。
    Begin
        Print @InfMod
        IF OBJECT_ID('tempdb.dbo.#Seven_Sin') IS NOT NULL
       BEGIN
               DROP TABLE #Seven_Sin
       END
       select  so.display_order_number AS 'EshopOrderNo'
                 ,CASE so.cod_flag WHEN 'N' THEN '3' ELSE '1' END as 'ServiceType'
                 ,Replace( so.ship_to_name ,',','') as 'ShopperName'
                  , h.container_id,so.Adddate,h.actual_ship_date,Replace( d.order_number ,'ESL-','') as display_order_number
                  ,so.display_order_number AS 'OrderNumber'
    			  ,so.display_order_number AS 'DnNumber'
                  , Right(Isnull( H.ship_to_phone ,'000'), 3) as 'ReceiverMobilPhone'
                  ,d.line_number
                  ,so.ship_to_code
                  ,h.erp_site AS 'Site'
    		   	  ,'SO' AS 'OrderType'
    			  ,so.comment AS 'Note1' 
                  ,d.item_description_full 
    			  ,d.qty_packed 
                  ,cast(so.cod_amount  as int)AS 'OrderAmount'
                  ,cast(so.total_amount  as int)AS 'ShipmentAmount'
  			      -- ,cast(round(case when sod.invoice_amount = 0 then 0 else sod.invoice_amount/sod.qty end,2 )  as numeric(8,2) ) AS 'PackagePrice'
    			  ,cast(round(case when sod.invoice_amount = 0 then 0 else sod.invoice_amount/sod.qty end,2 )  as INT ) AS 'PackagePrice'
           into #Seven_Sin
           from w_case_s(nolock) h,w_cased_s(nolock)d,w_So(nolock) so,w_sod(nolock) sod
        where h.erp_site = d.erp_site
            and h.client_code = d.client_code
            and h.container_id = d.container_id
            and d.client_code = so.client_code
            and d.erp_site = so.erp_site
            and d.client_code = sod.client_code
            and d.erp_site = sod.erp_site
            and Replace( d.order_number ,'ESL-','')   = sod.Display_order_number
            and d.order_line_number = sod.order_line_number
            and so.display_order_number =Replace( d.order_number ,'ESL-','')  
            and h.Sfile =@OutPutKey
            and h.customer_code = '7-11'
        Set @Srt = '<?xml version="1.0" encoding="UTF-8"?>' + (

        --Select            
        --    (Select
        --        DocNo = Replace(@OutPutKey,'.XML','')  ,
        --        DocDate = Replace(Convert( varchar(10), Getdate() ,111),'/','-') , 
        --        ParentId  = '785'
        --        For XML Path('DocHead') , Type ) ,
        --    (Select 
        --        EshopId = '001' ,
        --        OPMode = 'A' ,
        --        EshopOrderNo = H.display_order_number ,
        --        EshopOrderDate = Convert ( varchar(10), GetDate() , 120) ,
        --        ServiceType = CASE H.cod_flag WHEN 'N' THEN '3' ELSE '1' END  , 
        --        ShopperName = Replace( H.ship_to_name ,',','') , 
        --        ShopperPhone = ' ' ,  
        --        ShopperMobilPhone = ' ' ,  
        --        ShopperEmail = ' ' , 
        --        ReceiverName = Left(Replace( H.ship_to_name ,',',''),10)   , 
        --        ReceiverPhone = ' ' , 
        --        ReceiverMobilPhone = Right(Isnull( H.ship_to_phone ,'000'), 3) ,
        --        ReceiverEmail = ' ' ,  
        --        ReceiverIDNumber = ' ' , 
        --        OrderAmount = Cast(H.total_amount As Int) ,
        --        (
        --            Select 
        --            ProductId = ' ' , 
        --            ProductName = ' ' ,
        --            Quantity = ' ' , 
        --            Unit = ' ' , 
        --            UnitPrice = ' ' For XML Path('OrderDetail') , Type )  ,
        --        (   
        --            Select
        --            ShipmentNo = CS.container_id  ,   
        --            ShipDate = CONVERT(varchar(10),getdate()+0.52  ,121)   , 
        --            ReturnDate = CONVERT(varchar(10),getdate()+8.52  , 121)   , 
        --            LastShipment = 'Y'  , 
        --            --ShipmentAmount = Cast(H.total_amount As Int) , 
        --            ShipmentDetail_ShipmentAmount = cast((select sum(Csd.qty_packed * Sod.amount )  from Iexp..w_Sod As Sod, Iexp..w_Cased_S as Csd where Csd.order_line_number = Sod.Display_order_line_number And Csd.container_id = Cs.container_id  ) as int) , 
        --            StoreId=ISNULL( CS.ship_to_code, ' ')  , 
        --            EshopType='04'
        --            From iexp..w_So Where display_order_number = CS.Ex1 And client_code = CS.client_code 
        --            For XML Path('ShipmentDetail') , Type ) 
        --    From Iexp..w_Case_S as CS left outer join iexp..w_So as H ON CS.Ex1 = H.display_order_number and CS.client_code = H.client_code 
        --Where  CS.customer_code = '7-11' And CS.SFile = @OutPutKey   
        --    For XML Path ('Order') , Type ) As 'DocContent'  For XML Path ('OrderDoc'))

         

Select            
            (Select
                DocNo = Replace(@OutPutKey,'.XML','')  ,
                DocDate = Replace(Convert( varchar(10), Getdate() ,111),'/','-') , 
                ParentId  = '785'
                For XML Path('DocHead') , Type ) ,
            (Select 
                EshopId = '001' ,
                OPMode = 'A' ,
                EshopOrderNo ,
                EshopOrderDate = Convert ( varchar(10), GetDate() , 120) ,
                ServiceType , 
                ShopperName  , 
                ShopperPhone = ' ' ,  
                ShopperMobilPhone = ' ' ,  
                ShopperEmail = ' ' , 
                ReceiverName = Left(Replace( ShopperName ,',',''),10)   , 
                ReceiverPhone = ' ' , 
                ReceiverMobilPhone ,
                ReceiverEmail = ' ' ,  
                ReceiverIDNumber = ' ' , 
               -- OrderAmount  = sum(PackagePrice*qty_packed), --未來拆包在開啟，目前無法考慮拆包
                'OrderAmount' = case when  ServiceType = '1' then ShipmentAmount else OrderAmount end ,--當ServiceType=1 此攔位放賠償金額，當ServiceType=3 此攔位放應附金額
                (
                    Select 
                    ProductId = ' ' , 
                    ProductName = ' ' ,
                    Quantity = ' ' , 
                    Unit = ' ' , 
                    UnitPrice = ' ' For XML Path('OrderDetail') , Type )  ,
                (   
                    Select distinct
                    ShipmentNo = sd.container_id  ,   
                    ShipDate = CONVERT(varchar(10),getdate()+0.52  ,121)   , 
                    ReturnDate = CONVERT(varchar(10),getdate()+8.52  , 121)   , 
                    LastShipment = case when (select top 1  sdd.container_id from #Seven_Sin sdd where sd.OrderNumber = sdd.OrderNumber) = sd.container_id then 'Y'  else 'N' END , 
                    --ShipmentAmount = sum(PackagePrice*qty_packed)  , 
                    'ShipmentAmount' = case when  ServiceType = '1' then OrderAmount else ShipmentAmount end   , --當ServiceType=1 此攔位放應附金額，當ServiceType=3 此攔位放賠償金額
					'AwardAmount' = ShipmentAmount,-- v2.86 版本的調整 20220531_ken
                    StoreId = ISNULL( sd.ship_to_code, ' ')  , 
                    EshopType='04'
                    From #Seven_Sin as sd 
                    where  sd.display_order_number = #Seven_Sin.display_order_number
                    GROUP by  sd.container_id ,sd.ship_to_code,sd.OrderNumber,sd.ShipmentAmount,sd.ServiceType,sd.OrderAmount
                    For XML Path('ShipmentDetail') , Type ) 
            From #Seven_Sin 
            group by  EshopOrderNo , ServiceType ,  ShopperName ,ReceiverMobilPhone,display_order_number,OrderAmount,ShipmentAmount
            For XML Path ('Order') , Type ) As 'DocContent'  For XML Path ('OrderDoc'))

        Select Replace(@Srt,'&#x20;',' ') 

        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'Seven_Sin_So' -- 711 Sin出貨檔案。
    Begin
        Print @InfMod
        Select 
            Order_EshopID = '001' ,  
            Order_OPMode = 'A', 
            Order_EshopOrderNO = H.display_order_number ,  
            EshopOrderDate = Convert ( varchar(10), H.actual_delivery_date , 120) , 
            Order_ServiceType = CASE H.cod_flag WHEN 'N' THEN '3' ELSE '1' END  , 
            Order_ShopperName = Replace( H.ship_to_name ,',','') , 
            Order_ShopperPhone = '' ,  
            Order_ShopperMobilPhone = '' ,  
            Order_ShopperEmail = '' , 
            Order_ReceiverName = Left(Replace( H.ship_to_name ,',',''),10)   , 
            Order_ReceiverPhone = '' , 
            Order_ReceiverMobilPhone = 
                Case When Isnull( H.ship_to_phone ,'' ) = '' Then Right(Isnull( H.ship_to_phone ,'000'), 3) Else  Right(Isnull( H.ship_to_phone ,'000'), 3) End ,
            Order_ReceiverEmail = '' ,  
            Order_ReceiverIDNumber = '' , 
            Order_OrderAmount = Cast(H.total_amount As Int) , 
            OrderDetail_ProductID = '' , 
            OrderDetail_ProductName = '' ,
            OrderDetail_Quantity = '' , 
            OrderDetail_Unit = '' , 
            OrderDetail_UnitPrice = '' , 
            ShipmentDetail_ShipmentNo = ISNULL(HS.EX8 , '') , 
            --ShipmentDetail_ShipDate=CONVERT(varchar(10),Cs.CaseDate, 121)   , 
            --ShipmentDetail_ReturnDate = CONVERT(varchar(10),Cs.CaseDate+8 , 121)   , 
            --<190813 13:50>ShipDate
            ShipmentDetail_ShipDate = CONVERT(varchar(10),getdate()+0.52  ,121)   , 
            ShipmentDetail_ReturnDate = CONVERT(varchar(10),getdate()+8.52  , 121)   , 
            ShipmentDetail_LstShipment = 'Y'  , 
            ShipmentDetail_ShipmentAmount = Cast(H.total_amount As Int) , 
            --ShipmentDetail_ShipmentAmount = cast((select sum(Csd.qty_packed * Sod.amount )  from Iexp..w_Sod As Sod, Iexp..w_Cased_S as Csd where Csd.order_line_number = Sod.Display_order_line_number And Csd.container_id = Cs.container_id  ) as int) , 
            ShipmentDetail_StoreID=ISNULL( HS.ship_to_code, '')  , 
            ShipmentDetail_Eshop='04'  ,
	        DocName =REPLACE(@OutPutKey,'.XML','')
        from Iexp..h_So_Out as HS left outer join iexp..w_So as H ON HS.display_order_number = H.display_order_number and HS.client_code = H.client_code 
        where  HS.customer_code = '7-11' And HS.Efile = @OutPutKey   
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'Seven_Sin_SoN' -- 711 Sin出貨檔案。
    Begin
        Print @InfMod

        Set @Srt = '<?xml version="1.0" encoding="UTF-8"?>' + (
        Select            
            (Select
                DocNo = Replace(@OutPutKey,'.XML','')  ,
                DocDate = Replace(Convert( varchar(10), Getdate() ,111),'/','-') , 
                ParentId  = '785'
                For XML Path('DocHead') , Type ) ,
            (Select 
                EshopId = '001' ,
                OPMode = 'A' ,
                EshopOrderNo = H.display_order_number ,
                EshopOrderDate = Convert ( varchar(10), GetDate() , 120) ,
                ServiceType = CASE H.cod_flag WHEN 'N' THEN '3' ELSE '1' END  , 
                ShopperName = Replace( H.ship_to_name ,',','') , 
                ShopperPhone = ' ' ,  
                ShopperMobilPhone = ' ' ,  
                ShopperEmail = ' ' , 
                ReceiverName = Left(Replace( H.ship_to_name ,',',''),10)   , 
                ReceiverPhone = ' ' , 
                ReceiverMobilPhone = Right(Isnull( H.ship_to_phone ,'000'), 3) ,
                ReceiverEmail = ' ' ,  
                ReceiverIDNumber = ' ' , 
                OrderAmount = Cast(H.total_amount As Int) ,
                (
                    Select 
                    ProductId = ' ' , 
                    ProductName = ' ' ,
                    Quantity = ' ' , 
                    Unit = ' ' , 
                    UnitPrice = ' ' For XML Path('OrderDetail') , Type )  ,
                (   
                    Select
                    ShipmentNo = HS.Ex8 ,   
                    ShipDate = CONVERT(varchar(10),getdate()+0.52  ,121)   , 
                    ReturnDate = CONVERT(varchar(10),getdate()+8.52  , 121)   , 
                    LastShipment = 'Y'  , 
                    ShipmentAmount = Cast(H.total_amount As Int) , 
                    --ShipmentDetail_ShipmentAmount = cast((select sum(Csd.qty_packed * Sod.amount )  from Iexp..w_Sod As Sod, Iexp..w_Cased_S as Csd where Csd.order_line_number = Sod.Display_order_line_number And Csd.container_id = Cs.container_id  ) as int) , 
                    StoreId=ISNULL( HS.ship_to_code, ' ')  , 
                    EshopType='04'
                    From iexp..w_So Where display_order_number = HS.display_order_number And client_code = HS.client_code 
                    For XML Path('ShipmentDetail') , Type ) 
            From Iexp..h_So_Out as HS left outer join iexp..w_So as H ON HS.display_order_number = H.display_order_number and HS.client_code = H.client_code 
        Where  HS.customer_code = '7-11' And HS.Efile = @OutPutKey   
            For XML Path ('Order') , Type ) As 'DocContent'  For XML Path ('OrderDoc'))
        Select Replace(@Srt,'&#x20;',' ') 
        
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'CK_F03P03' -- CK F03、P03出貨檔案。
    Begin
        Print @InfMod
        Select  
            STNO = Cs.ship_to_city , 
            STNM = St.site_name ,  
            odkey = Cs.EX1 , 
            dpay = 0 ,
            down = isnull(H.ship_to_name ,'') , 
            tots = isnull(H.total_amount ,0) , 
            dtel = isnull(H.ship_to_phone ,'') ,
            Weight = Case When CAST( Cs.actual_weight AS Decimal(18,3))> 99 Then 500 Else  CAST((CAST(Cs.actual_weight As Decimal(18,3))* 1000) As Bigint) End , 
            Packtype = Cs.container_type ,
            VSART = Cs.ship_to_city , 
            Etyps = Cs.Etyps
        From Iexp..w_So As H 
            Left Outer Join Iexp..w_Case_S  As Cs On H.display_order_number = Cs.EX1
            Left Outer Join  Iexp..m_Storer_S As ST On Cs.ship_to_code = ST.site_code And Cs.customer_code = ST.customer_code
        Where Cs.customer_code = 'CK' And Cs.Etyps>0 And Cs.actual_weight Is Not Null And cs.container_type <> 0 And Cs.SFile = @OutPutKey    
            Order By Cs.EX1
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'CVS_F10' -- CVS F10出貨檔案。
    Begin
        Print @InfMod
	    Select  
            ECNO = '169' ,
		    ODNO = Right(Cs.container_id,11) ,
		    STNO = Cs.ship_to_code ,    
		    AMT = CASE H.cod_flag WHEN 'N' THEN 0 ELSE H.total_amount END   ,
		    CUTKNM=REPLACE(H.ship_to_name ,',','') , 
		    CUTKTL= CASE ISNULL(H.ship_to_phone ,'') WHEN '' THEN '000' ELSE  RIGHT(H.ship_to_phone , 3) END ,
		    PRODNM='0' ,
		    ECWEB='誠品網路書店' ,
		    ECWEB='http://www.eslite.com' ,
		    ECSERTEL='02-87898921' ,
		    REALAMT= H.total_amount , 
		    --REALAMT=(select sum(Csd.qty_packed * Sod.amount )  from Iexp..w_Sod As Sod , Iexp..w_Cased_S As Csd Where Csd.order_line_number = Sod.Display_order_line_number And Csd.container_id = Cs.container_id)  , 
		    TRADETYPE= CASE H.cod_flag WHEN 'N' THEN '3' ELSE '1' END   ,
		    SERCODE='963' ,
		    EDCNO='D07' ,
		    H_TOTALS = ( Select COUNT(*) From Iexp..w_Case_S Where customer_code = 'CVS' and SFile=@OutPutKey  )
	    From Iexp..w_Case_S as Cs left outer join Iexp..w_So as H ON Cs.Ex1 = H.display_order_number and Cs.erp_site = H.erp_site 
		    Where Cs.customer_code = 'CVS' and Cs.Etyps > 0  and  Cs.SFile = @OutPutKey   
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'CVS_F10_So' -- CVS F10出貨檔案。
    Begin
        Print @InfMod

        Set @Srt = '<?xml version="1.0" encoding="UTF-8"?>' + (	    
        Select 
        (Select 
            ECNO = '169'  ,
		    ODNO = Right('0'+Ltrim(Rtrim(Hs.Ex8)),11) ,
		    STNO = Hs.ship_to_code ,    
		    AMT = CASE H.cod_flag WHEN 'N' THEN 0 ELSE Cast(H.total_amount As Int) END   ,
		    CUTKNM ='<![CDATA['+REPLACE(H.ship_to_name ,',','')+N']]>' , 
		    CUTKTL=CASE  when ISNULL(H.ship_to_phone ,'')='' THEN '000' ELSE  RIGHT(H.ship_to_phone , 3) END ,
		    PRODNM='0' ,
		    ECWEB='<![CDATA['+'誠品網路書店'+']]>' ,
		    --ECWEB='http://www.eslite.com' ,
		    ECSERTEL='02-87898921' ,
		    REALAMT= Cast(H.total_amount As Int), 
		    --REALAMT=(select sum(Csd.qty_packed * Sod.amount )  from Iexp..w_Sod As Sod , Iexp..w_Cased_S As Csd Where Csd.order_line_number = Sod.Display_order_line_number And Csd.container_id = Cs.container_id)  , 
		    TRADETYPE= CASE H.cod_flag WHEN 'N' THEN '3' ELSE '1' END   ,
		    SERCODE='963' ,
		    EDCNO='D07'  
                From Iexp..h_So_Out as Hs left outer join Iexp..w_So as H ON Hs.client_code = H.client_code And Hs.display_order_number = H.display_order_number and Hs.erp_site = H.erp_site 
		        Where Hs.customer_code = 'CVS' and HS.Efile = @OutPutKey     
            For XML Path('ORDER') , Type ),
            (
            Select 
            TOTALS = ( Select COUNT(*) From Iexp..h_So_Out Where customer_code = 'CVS' and Efile = @OutPutKey )
            For XML Path('ORDERCOUNT') , Type)  
            For XML Path('ORDER_DOC')) 
        
        Select Replace(Replace(Replace(@Srt,'&#x20;',' '),'&lt;','<'),'&gt;','>') 
        Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'REY_XML' -- 20220109_Ken 全家FORMAT。
    Begin
        Print @InfMod
       IF OBJECT_ID('tempdb.dbo.#REY_XML') IS NOT NULL
       BEGIN
               DROP TABLE #REY_XML
       END
       
      select  so.display_order_number AS 'EshopOrderNo'
                 ,CASE so.cod_flag WHEN 'N' THEN '3' ELSE '1' END as 'ServiceType'
                 ,Replace( so.ship_to_name ,',','') as 'ShopperName'
                  , h.container_id,so.Adddate,h.actual_ship_date,Replace( d.order_number ,'ESL-','') as display_order_number
                  ,so.display_order_number AS 'OrderNumber'
    			  ,so.display_order_number AS 'DnNumber'
                  , Right(Isnull( H.ship_to_phone ,'000'), 3) as 'ReceiverMobilPhone'
                  ,d.line_number
                  ,so.ship_to_code
                  ,h.erp_site AS 'Site'
    		   	  ,'SO' AS 'OrderType'
    			  ,so.comment AS 'Note1' 
                  ,d.item_description_full 
    			  ,d.qty_packed 
                  ,so.total_amount AS 'OrderAmount'
    			  ,cast(round(case when sod.invoice_amount = 0 then 0 else sod.invoice_amount/sod.qty end,2 )  as numeric(8,2) ) AS 'PackagePrice'
           into #REY_XML
           from w_case_s(nolock) h,w_cased_s(nolock)d,w_So(nolock) so,w_sod(nolock) sod
        where h.erp_site = d.erp_site
            and h.client_code = d.client_code
            and h.container_id = d.container_id
            and d.client_code = so.client_code
            and d.erp_site = so.erp_site
            and d.client_code = sod.client_code
            and d.erp_site = sod.erp_site
            and Replace( d.order_number ,'ESL-','')   = sod.Display_order_number
            and d.order_line_number = sod.order_line_number
            and so.display_order_number =Replace( d.order_number ,'ESL-','')  
          --  and so.display_order_number =@orderkey
            and h.Sfile = 'GARY'
            and h.customer_code = 'REY'

   
        Set @Srt = '<?xml version="1.0" encoding="UTF-8"?>' + (

        Select            
            (Select
                RDFMT = '1' ,
                SNCD  = 'DFM',
                PRDT = Replace(Convert( varchar(10), Getdate() ,111),'/','-') 
                For XML Path('Head') , Type ) ,
            (select 
             (   
                    Select distinct 
                    RDFMT = '2',   
                    ParentId = '未知，先開欄位',--廠商代碼
                    EshopId = '0001',----廠商代碼
                    ShipmentNo =  sd.container_id,
                    DCReceiveDate =Replace(Convert( varchar(10), Getdate() ,111),'/','-') ,
                    DCReceiveStatus = '1',
                    FlowType = 'N'
                    From #REY_XML as sd 
                    For XML Path('R04') , Type ) 
                    For XML Path('BODY') , Type ),
            (Select
                RDFMT = '3' ,
                RDCNT  = (select count(distinct container_id) from #REY_XML)
                For XML Path('FOOTER') , Type ) 
          For XML Path('doc') , Type)

          Select Replace(@Srt,'&#x20;',' ') 

         Set @RowCount = @RowCount + @@ROWCOUNT 
    End
    If @InfMod = 'OMS_Invertory' --庫存傳出
    Begin
        Print @InfMod
        --Select 
        --    Right('0000000000'+Cast(SL.Id As varchar(10)),10) As Id , Right(Ltrim(Rtrim(SL.display_item_number)),13) As Sku , SL.actual_qty - SL.unavailable_qty - Isnull(HD.hold_qty,0) As Qty , SL.erp_site As ErpSite
        --From [HJWMS].AAD.dbo._t_oms_inventory_(X) As SL Left Join ( 
        --Select wh_id , client_code , erp_site , erp_wh , item_number , display_item_number , Sum(hold_qty) As hold_qty 
        --    From [HJWMS].AAD.dbo._t_oms_inventory_hold_(X)
        --Group By 
        --    wh_id , client_code , erp_site , erp_wh , item_number , display_item_number ) As HD
        --On SL.wh_id = HD.wh_id And SL.client_code = HD.client_code And SL.erp_site = HD.erp_site And SL.item_number = HD.item_number 
        --    And SL.display_item_number = HD.display_item_number 
        --Where SL.actual_qty - SL.unavailable_qty - Isnull(HD.hold_qty,0) > 0 And SL.erp_site = @OutPutKey
        --    Order By Id
        Set @RowCount = @RowCount + @@ROWCOUNT
    End
    If @InfMod = 'OMS_Invertorychange' --庫存傳出
    Begin
        Print @InfMod

        Select
            dbo.FmtNumStr(T_ID , 10, 0 ) As OMSNo , Right(Ltrim(Rtrim(display_item_number)),13) As Sku , tran_qty As Qty , actual_qty As StockQty ,
            tran_type As OMSType , erp_site As [Site] , 
            Case When tran_type in ('30','31')  Then 
                Case When employee_id in ('MSM') Then 'MSM' Else 'EC' End 
            Else 'Sap' End  As SOType , control_number As SO , line_number As SOLineNo
        From h_OMS_Tran_Out Where Sfile = @OutPutKey
        
        --Select
        --    dbo.FmtNumStr(T_ID , 10, 0 ) As OMSNo , Right(Ltrim(Rtrim(display_item_number)),13) As Sku , tran_qty As Qty , actual_qty As StockQty ,
        --    '10' As OMSType , erp_site As [Site] , 'Sap' As SOType , control_number As SO , Right('000000'+line_number,6) As SOLineNo
        --From h_OMS_Tran_Out Where erp_site = @OutPutKey 
        Set @RowCount = @RowCount + @@ROWCOUNT
    End

    If @InfMod = 'Rsoreturn' Begin
		select top 1   hpo.Erp_type_text as 'ORDER_TYPE',
				 o.E1EDK02_BELNR01 as 'E1EDK02_BELNR01',
				 o.E1EDK02_BELNR02 as 'E1EDK02_BELNR02',
				 o.E1EDK02_BELNR03 as 'E1EDK02_BELNR03',
				 o.E1EDK02_BELNR10 as 'E1EDK02_BELNR10',
				 isnull(o.NOTE,'') as 'NOTE1',
				 '' as 'NOTE2',
				 '' as 'NOTE3', ( Select  omsod.E1EDP02_BELNR01 as 'E1EDP02_BELNR01',
						  omso.Sokey as 'E1EDP02_BELNR02',
						  hpod.erp_site as 'WERKS',
						  hpod.item_number as 'IDTNR',
						  m_sku.item_description as 'KTEXT',
						  omsod.POSEX as 'POSEX',
						  hpod.qty as 'MENGE1',
						  hpod.qty_received as 'MENGE2',
						  '01' as 'STATUS',
						  hpod.comment as 'ITEMNOTE1',
						  '' as 'ITEMNOTE2',
						  ''as 'ITEMNOTE3'
				 From     h_Pod_Out hpod
				 join     sapi..OMS_ORDER_BK omso   on omso.Sokey = hpod.display_po_number and omso.clientcode = hpod.client_code
				 join     sapi..OMS_ORDERD_BK omsod on omso.Sokey = omsod.E1EDP02_BELNR01
				 Left Join m_Sku       On hpod.client_code = m_Sku.client_code And hpod.erp_site = m_Sku.erp_site And hpod.item_number = m_Sku.display_item_number
				 Where    hpod.display_po_number = hpo.display_po_number
						  and hpod.client_code = hpo.client_code For Xml Path ('ITEMS') ,
						  TYPE ) As ITEMS
		from     h_Po_Out hpo
		join     sapi..OMS_ORDER_BK o on hpo.display_po_number = o.Sokey and hpo.client_code = o.clientcode For Xml Path ('ORDER')

	end



    -- 判斷筆數，並且回傳Msg
    If @RowCount = 0 
    Begin
        Set @Msg = '失敗，類型：'+ @InfMod + '，貨主：'+@Client_Code + '，轉出號：' + @OutPutKey + '查無相關資料'
    End
    Else 
    Begin
        Set @Msg = '成功，類型：'+ @InfMod + '，貨主：'+@Client_Code + '，轉出號：' + @OutPutKey + '取得筆數'+Cast(@RowCount As Nvarchar(10))+ '筆'
    End
End 

