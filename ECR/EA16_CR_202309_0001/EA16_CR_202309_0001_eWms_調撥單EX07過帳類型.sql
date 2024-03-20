USE [iexp]
GO

/****** Object:  StoredProcedure [dbo].[EDI_SAPXsdGroup_Qry]    Script Date: 2023/10/23 下午 02:42:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






/************************************************************* 
-- 提供XSD介面 的資料來源 、含狀態異動
-- XSD  執行 傳入FMT: 接口ID=@Infid , 主參數: @wkey(檔名/odkey) , 
								次參數:@Extkey(G貨主,otherkey,貨主,其他....) , 執行Tag: @USID
取 次參數() 方式 :  dbo.StrArgIndex(@ExtkeyS ,id)  ；
								如果要依貨主查詢 請取 dbo.StrArgIndex(@ExtkeyS ,3) 
-- <201900610>    

TEST CODE
exec [EDI_SAPXsdGroup_Qry] 'EX2MSM','EX2MSMB063220119.Txt','',''
/*20230727 by JackyHsu 更改EX07SO自製品1110貨主若出貨對象為F001則351否則為951 (請使用者提ECR後再修改)*/
************************************************************ */
CREATE procedure  [dbo].[EDI_SAPXsdGroup_Qry](@Infmod varchar(30) , @wkey varchar(100) ,@ExtkeyS  varchar(300),@USID varchar(30) )
as
--  2F 通過型 SAP 正常 EX01 (有作業量)，暫不使用 @ExtkeyS
Declare @Srt NvarChar(Max)
DECLARE @MM NVARCHAR(2),@BUDAT NVARCHAR(8) , @par1  VARCHAR(30), @par2  VARCHAR(30)
IF (@Infmod='EX01')  
BEGIN
	SET @par2=dbo.StrArgIndex(@ExtkeyS ,2)

	SELECT  RBLNRB=left(C_CONTACT1,10),RBPOSB=left(LOTTABLE04,6),
		BTYPB=left(WKMRKA,1),BLNRB=XSOD.STO,BPOSB=right(XSOD.STOLINENO,6),
		BTYPA=left(WKMRKB,1),BLNRA=XSOHost.BUYERPO,BPOSA=left(LOTTABLE05,6),MATNR=left(SKU,18),
		IMNGA=cast(SHIPPEDQTY as varchar(15)),IEINA=iexp.dbo.GetUOM(UOM),
		WERKA=case when XSOHost.STORERKEY='01DBSDB99' then 'F001' 
								when XSOHost.STORERKEY='DBE1' then 'C001' 
								else right(rtrim(XSOHost.STORERKEY),4) end,
		WERKS=right(rtrim(door),4),ORDERDATE=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd')
	From XSOHost with(nolock) Inner join XSOD with(nolock)
		ON XSOHost.STORERKEY=XSOD.STORERKEY and XSOHost.BUYERPO=XSOD.BUYERPO and XSOHost.EXTERNORDERKEY=XSOD.EXTERNORDERKEY  
		where  somsg!='Z'  and XSOHost.BUYERPO=@par2   -- and XSOHost.STORERKEY= dbo.StrArgIndex(@ExtkeyS ,3)
	--<換行>
	SELECT * from (select  wrline='' ) as Newline1
	--<EX07: 來源 PO   : 專用格式 EX_WMMBID02.fmt>
	SELECT BLDAT=iexp.dbo.FmtDateStr(isnull(cdate,getdate()),'yyyymmdd'),BUDAT=iexp.dbo.FmtDateStr(rdate,'yyyymmdd'),
		XBLNR=isnull(VPO,''),BKTXT=cpcs,FRBNR=isnull(left(IFPO,16),''),TCODE='MB01',MATNR=left(SKU,18),
		WERKS=case when POHost.STORERKEY='01DBSDB99' then 'F001' when POHost.STORERKEY='FK01' then 'F002'  
								when POHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(POHost.STORERKEY),4) end ,
		LGORT=' ',BWART='101', ERFMG=QTYRECEIVED, ERFME='EA',
		EBELN=case when pomsg='ZEL' then ' ' else left(POHost.EXTERNPOKEY,10) end,EBELP=case when pomsg='ZEL' then ' ' else left(EXTERNLINENO,6) end,
		SGTXT=left(POHost.EXTERNPOKEY,10)+'     '+cast(isnull(dpqty,0) as varchar(5))+'/'+cast(isnull(caqty,0) as varchar(4))+'  ',KZBEW='B',
		VLIEF_AVIS=case when pomsg='ZEL' then left(POHost.EXTERNPOKEY,10) else ' ' end,VBELP_AVIS=case when pomsg='ZEL' then left(EXTERNLINENO,6) else '      ' end									
		--LGORT=' ',BWART='101', ERFMG=QTYRECEIVED,	ERFME='EA',
		--EBELN=case when pomsg='ZEL' then '' else left(POHost.EXTERNPOKEY,10) end,EBELP=case when pomsg='ZEL' then '' else left(EXTERNLINENO,6) end,
		--SGTXT=left(POHost.EXTERNPOKEY,10)+'     '+cast(isnull(dpqty,0) as varchar(5))+'/'+cast(isnull(caqty,0) as varchar(4))+'',KZBEW='B',
		--VLIEF_AVIS=case when pomsg='ZEL' then left(POHost.EXTERNPOKEY,10) else '' end,VBELP_AVIS=case when pomsg='ZEL' then left(EXTERNLINENO,6) else '' end
	FROM  POHost with(nolock) Inner join POD with(nolock)
		ON POHost.STORERKEY=POD.STORERKEY and POHost.EXTERNPOKEY=POD.EXTERNPOKEY 
	WHERE  QTYRECEIVED!=0   and  POHost.EXTERNPOKEY=@par2  --and POHost.STORERKEY=dbo.StrArgIndex(@ExtkeyS ,3)
 	--<換行>
	SELECT * from (select  wrline=''  ) as Newline2
	--< 來源 XSOHost  : 專用格式  WMM02.fmt
	SELECT  PARTNER_ID=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,TANUM='',
		KODAT=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd'),KOMUE='X',WABUC='X',
		WADAT_IST=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd'),VBELN_VL=left(XSOD.STO,10),POSNR_VL=left(XSOD.STOLINENO,6),
		VBELN=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,POSNN=left(EXTERNLINENO,6),
		PIKMG=cast(SHIPPEDQTY as varchar(15)),MATNR=left(SKU,18),APDa=right(rtrim(door),4)
	From XSOHost with(nolock) Inner join XSOD with(nolock) 
		ON XSOHost.STORERKEY=XSOD.STORERKEY and XSOHost.BUYERPO=XSOD.BUYERPO and XSOHost.EXTERNORDERKEY=XSOD.EXTERNORDERKEY 
		where somsg!='Z' and SHIPPEDQTY!=0 and   XSOHost.door <> 'F001'
			 and  XSOHost.BUYERPO=@par2  --and XSOHost.STORERKEY=dbo.StrArgIndex(@ExtkeyS ,3)
  
END 

IF (@Infmod='EX31')  
BEGIN
	--< ?? 來源 XSOHost 格式 MM43.fmt( SP18)>
	SELECT  RBLNRB=left(C_CONTACT1,10),RBPOSB=left(LOTTABLE04,6),
		BTYPB=left(WKMRKA,1),BLNRB=XSOD.STO,BPOSB=right(XSOD.STOLINENO,6),
		BTYPA=left(WKMRKB,1),BLNRA=XSOHost.BUYERPO,BPOSA=left(LOTTABLE05,6),MATNR=left(SKU,18),
		IMNGA=cast(SHIPPEDQTY as varchar(15)),IEINA=iexp.dbo.GetUOM(UOM),
		WERKA=case when XSOHost.STORERKEY='01DBSDB99' then 'F001' 
								when XSOHost.STORERKEY='DBE1' then 'C001' 
								else right(rtrim(XSOHost.STORERKEY),4) end,
		WERKS=right(rtrim(door),4),ORDERDATE=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd')
	From XSOHost with(nolock) Inner join XSOD with(nolock)
		ON XSOHost.STORERKEY=XSOD.STORERKEY and XSOHost.BUYERPO=XSOD.BUYERPO and XSOHost.EXTERNORDERKEY=XSOD.EXTERNORDERKEY  
		where   XSOHost.BUYERPO=dbo.StrArgIndex(@ExtkeyS ,2)
	--<換行>
	SELECT * from (select  wrline='' ) as Newline1
	--<EX07: 來源 PO   : 專用格式 EX_WMMBID02.fmt>
	SELECT BLDAT=iexp.dbo.FmtDateStr(isnull(cdate,getdate()),'yyyymmdd'),BUDAT=iexp.dbo.FmtDateStr(rdate,'yyyymmdd'),
		XBLNR=isnull(VPO,''),BKTXT=cpcs,FRBNR=isnull(left(IFPO,16),''),TCODE='MB01',MATNR=left(SKU,18),
		WERKS=case when POHost.STORERKEY='01DBSDB99' then 'F001' when POHost.STORERKEY='FK01' then 'F002'  
								when POHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(POHost.STORERKEY),4) end ,
		LGORT=' ',BWART='101', ERFMG=QTYRECEIVED,	ERFME='EA',
		EBELN=  left(POHost.EXTERNPOKEY,10)  ,EBELP= left(EXTERNLINENO,6)  ,
		SGTXT=left(POHost.EXTERNPOKEY,10)+'     '+cast(isnull(dpqty,0) as varchar(5))+'/'+cast(isnull(caqty,0) as varchar(4))+'  ',KZBEW='',
		VLIEF_AVIS='' ,VBELP_AVIS= ''  
	FROM  POHost with(nolock) Inner join POD with(nolock)
		ON POHost.STORERKEY=POD.STORERKEY and POHost.EXTERNPOKEY=POD.EXTERNPOKEY 
	WHERE    POHost.EXTERNPOKEY= dbo.StrArgIndex(@ExtkeyS ,2)
 	--<換行>
	SELECT * from (select  wrline=''  ) as Newline2
	--< 來源 XSOHost  : 專用格式  WMM02.fmt
	SELECT  PARTNER_ID=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,TANUM='',
		KODAT=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd'),KOMUE='X',WABUC='X',
		WADAT_IST=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd'),VBELN_VL=left(XSOD.STO,10),POSNR_VL=left(XSOD.STOLINENO,6),
		VBELN=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,POSNN=left(EXTERNLINENO,6),
		PIKMG=cast(SHIPPEDQTY as varchar(15)),MATNR=left(SKU,18),APDa=right(rtrim(door),4)
	From XSOHost with(nolock) Inner join XSOD with(nolock) 
		ON XSOHost.STORERKEY=XSOD.STORERKEY and XSOHost.BUYERPO=XSOD.BUYERPO and XSOHost.EXTERNORDERKEY=XSOD.EXTERNORDERKEY 
		where    XSOHost.BUYERPO=dbo.StrArgIndex(@ExtkeyS ,2)
  
END 

--  EX_PORDCR05.fmt  /  expRSO_EX05 
IF (@Infmod='EX05')  
BEGIN
	SELECT DOC_DATE=iexp.dbo.FmtDateStr(oddate,'yyyymmdd'),
		DOC_TYPE='ZR02',DOC_CAT='F',
		PURCH_ORG=case when SOHost.STORERKEY in ('01DBSDB99','ESL' ) then 'EADB' when SOHost.STORERKEY in ('FK01','F002') then 'EAEX'  else 'EADB' end   ,
		PUR_GROUP='X01',  VENDOR=left(HOSTREFERENCE,10),
		PO_NUMBER=left(SOHost.EXTERNORDERKEY,10),
		PO_ITEM=dbo.FmtNumStr(EXTERNLINENO,5,0 ) ,  
		PUR_MAT=left(SKU,18), STORE_LOC='0004'  ,
		PLANT=case when SOHost.STORERKEY in ('01DBSDB99' ,'ESL' ) then 'F001' when SOHost.STORERKEY in ('FK01') then 'F002' when SOHost.STORERKEY='DBE1' then 'C001' else left(SOHost.STORERKEY,4) end,
		RET_ITEM='X',DELIV_DATE=iexp.dbo.FmtDateStr(oddate,'yyyymmdd'),
		QUANTITY=cast(oqty as varchar(15)),
		PARTNERDESC='PI',LANGU='M', BUSPARTNO=left(HOSTREFERENCE,10)
	from SOHost with(nolock) inner join SOD with(nolock) ON SOHost.STORERKEY=SOD.STORERKEY and SOHost.EXTERNORDERKEY=SOD.EXTERNORDERKEY 
		where  oqty!=0  and WKMRKB is null and  SOHost.EXTERNORDERKEY=dbo.StrArgIndex(@ExtkeyS ,2)
		 --(SOHost.EXTERNORDERKEY=@wkey or SOHost.cfile=@wkey)  cctyps BETWEEN 30 and 39 and SOHost.door!='RETURN' and SOHost.etyps=8 and 
END 
 
--  EX_SDPIID01.fm / (OLD:expSC_EX08)
IF (@Infmod='EX08')  
BEGIN
	select PARTNER_ID=right(rtrim(door),4)+SPdate,TANUM='1',KODAT=
		ISNULL(iexp.dbo.FmtDateStr(SOHost.cdate,'yyyymmdd'),iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd')) , 
		KOMUE='X',WABUC='X', WADAT_IST=iexp.dbo.FmtDateStr(SOHost.edate,'yyyymmdd'),
		VBELN_VL=left(SOD.EXTERNORDERKEY,10),POSNR_VL=right(EXTERNLINENO,6),
		VBELN=case when SOHost.STORERKEY in ('01DBSDB99','ESL' ) then  right(rtrim(door),4)+SPdate else left(SOD.EXTERNORDERKEY,10) end ,
		POSNN=right(EXTERNLINENO,6),PIKMG=cast(SHIPPEDQTY as varchar(15)),MATNR=left(SKU,18),APDa='ZRD1'
	from SOHost with(nolock) inner join SOD with(nolock) ON SOHost.STORERKEY=SOD.STORERKEY and SOHost.EXTERNORDERKEY=SOD.EXTERNORDERKEY 
		where  cctyps=10 and (SOHost.EXTERNORDERKEY=@wkey or SOHost.cfile=@wkey)  and door is not null 
		--and SPdate is not null
		--and    SOHost.EXTERNORDERKEY  like '08[5,6]%' 
END 
-- BSO (086、085)   測試 SAP 是否可以全吃
IF (@Infmod='EX08A')  
BEGIN
	select PARTNER_ID=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,TANUM='1',
		KODAT=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd'),KOMUE='X',WABUC='X',
		WADAT_IST=iexp.dbo.FmtDateStr(XSOHost.edate,'yyyymmdd'),VBELN_VL=left(XSOD.EXTERNORDERKEY,10),POSNR_VL=right(EXTERNLINENO,6),
		VBELN=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,POSNN=right(EXTERNLINENO,6),
		PIKMG=cast(SHIPPEDQTY as varchar(15)),MATNR=left(SKU,18),APDa='ZRD1'
	from XSOD with(nolock) inner join  XSOHost with(nolock) ON XSOHost.STORERKEY=XSOD.STORERKEY and XSOHost.EXTERNORDERKEY=XSOD.EXTERNORDERKEY 
	where  XSOHost.ctyps>0 and (XSOHost.BUYERPO=@wkey or XSOHost.cfile=@wkey)  
END 
--  EX07 來源 SO  轉單  : 專用格式 EX_WMMBID02.fmt 
IF (@Infmod in ('EX07SO')  )
BEGIN
	SELECT @MM = SUBSTRING(iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd'),5,2)
	SELECT @BUDAT = iexp.dbo.FmtDateStr(ISNULL(MAX(cdate),GETDATE()),'yyyymmdd') FROM SOHost WHERE EXTERNORDERKEY = @wkey OR cfile = @wkey
	IF @MM <> SUBSTRING(@BUDAT,5,2)
		BEGIN
			SELECT @BUDAT = iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd')
		END
	SELECT 
	BLDAT=iexp.dbo.FmtDateStr(SOHost.edate,'yyyymmdd')
	,BUDAT=@BUDAT
	,XBLNR=' '
	,BKTXT=cast(cpcs as varchar(25))
	,FRBNR=isnull(SOHost.BUYERPO,' '),
		TCODE=case when cctyps=30 then 'MB01' else 'MB1B' END ,
		MATNR=left(SKU,18),
		WERKS=case when SOHost.STORERKEY  in ('01DBSDB99','ESL' )  then 'F001' when SOHost.STORERKEY='DBE1' then 'C001' when SOHost.STORERKEY='FK01' then 'F002' else right(SOHost.STORERKEY,4) end,
		--WERKS=case when cctyps=30 then (case when SOHost.STORERKEY='01DBSDB99' then 'F001' when SOHost.STORERKEY='DBE1' then 'C001' when SOHost.STORERKEY='FK01' then 'F002' else right(SOHost.STORERKEY,4) end) else  left(door,4 ) end,
		--	LGORT=case when cctyps=30 then ' ' else 'PU01' END , 20180530_CALVIN 1300依照CCTYPE提供倉別（PU01），其餘使用0001倉
		LGORT=case SOHost.storerkey WHEN '01DBS1300' THEN case when cctyps=30 then ' ' else 'PU01' END  ELSE '0001' END,
		--20230922 Raines alter by ECR EA16_CR_202308_0001_eWMS_調撥單EX07過帳類型_修改V1.0
		BWART = case 
		when cctyps = 10 and door = 'F001' then '351'
		else '951' end,
		--20230922 Raines alter by ECR EA16_CR_202308_0001_eWMS_調撥單EX07過帳類型_修改V1.0
		--BWART=case when cctyps=30 then '101' else (case when  SOHost.STORERKEY in ('F100','FC02','1110') then '351' else  '951' end) END ,
		--BWART=case when cctyps=30 then '101' else (case when  (SOHost.STORERKEY in ('F100','FC02') or (SOHost.STORERKEY = '1110' and sod.HOSTREFERENCE = 'F001')) then '351' else  '951' end) END , --JH01
		ERFMG=SHIPPEDQTY,ERFME='EA',
		EBELN=left(SOHost.EXTERNORDERKEY,10),EBELP=dbo.FmtNumStr(EXTERNLINENO,5,0 ) ,ELIKZ=' ',
		SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=' ' , SOD.Oqty - SOD.SHIPPEDQTY  --REPLICATE(' ',6)
	from SOD with(nolock) INNER JOIN SOHost with(nolock) ON SOHost.STORERKEY=SOD.STORERKEY and SOHost.EXTERNORDERKEY=SOD.EXTERNORDERKEY 
		where SOHost.EXTERNORDERKEY like '[5,4]%'  and  SHIPPEDQTY!=0 and SOHost.ctyps>0  and 
		 (SOHost.EXTERNORDERKEY=@wkey or SOHost.cfile=@wkey)
END
--  EX06 來源 RSO   : 專用格式 EX_WMMBID02.fmt
IF (@Infmod in ('EX06' )  )
BEGIN
	SELECT @MM = SUBSTRING(iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd'),5,2)
	SELECT @BUDAT = iexp.dbo.FmtDateStr(ISNULL(MAX(cdate),GETDATE()),'yyyymmdd') FROM SOHost WHERE EXTERNORDERKEY = @wkey OR cfile = @wkey
	IF @MM <> SUBSTRING(@BUDAT,5,2)
		BEGIN
			SELECT @BUDAT = iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd')
		END
	SELECT BLDAT=iexp.dbo.FmtDateStr(SOHost.edate,'yyyymmdd'),BUDAT=@BUDAT,
		XBLNR=' ',BKTXT=cast(cpcs as varchar(25)),FRBNR=isnull(SOHost.BUYERPO,' '),
		TCODE='MB01' , MATNR=left(SKU,18),
		WERKS=case when SOHost.STORERKEY  in ('01DBSDB99','ESL' )  then 'F001' when SOHost.STORERKEY='FK01' then 'F002'  
								when SOHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(SOHost.STORERKEY),4) end ,
--		LGORT=case when cctyps=30 then ' ' else 'PU01' END , 20180530_CALVIN 1300依照CCTYPE提供倉別（PU01），其餘使用0001倉
		LGORT=case when SOHost.STORERKEY  in ('01DBSDB99','ESL' ) then (
							case when SOHost.EXTERNORDERKEY like  '461%' then '0006'  when SOHost.EXTERNORDERKEY like    '401%' then '0001'  else  '0004'   end )
						when SOHost.STORERKEY='DBE1' then '0004' else   '0004'  end , 
		BWART=  '101'  ,
		ERFMG=SHIPPEDQTY,	ERFME='EA',
		EBELN=left(SOHost.EXTERNORDERKEY,10),EBELP=dbo.FmtNumStr(EXTERNLINENO,5,0 ) ,ELIKZ=' ',
		SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=REPLICATE(' ',6)
	from SOD with(nolock) INNER JOIN SOHost with(nolock) ON SOHost.STORERKEY=SOD.STORERKEY and SOHost.EXTERNORDERKEY=SOD.EXTERNORDERKEY 
		where SOHost.EXTERNORDERKEY like '[5,4]%'  and  SHIPPEDQTY!=0 and SOHost.ctyps>0  and 
		 (SOHost.EXTERNORDERKEY=@wkey or SOHost.cfile=@wkey)
END

IF (@Infmod in ('EX06BankPO' ) )
BEGIN
	SELECT BLDAT=iexp.dbo.FmtDateStr(Pohost.EditDate ,'yyyymmdd'),BUDAT=iexp.dbo.FmtDateStr(isnull(Pohost.cdate,getdate()),'yyyymmdd'),
		XBLNR=' ',BKTXT=cast(cpcs as varchar(25)),FRBNR = ' ',
		TCODE='MB01' , MATNR=left(SKU,18),
		WERKS=case when Pohost.STORERKEY  in ('01DBSDB99','ESL' )  then 'F001' when Pohost.STORERKEY='FK01' then 'F002'  
								when Pohost.STORERKEY='DBE1' then 'C001'  else right(rtrim(Pohost.STORERKEY),4) end ,
--		LGORT=case when cctyps=30 then ' ' else 'PU01' END , 20180530_CALVIN 1300依照CCTYPE提供倉別（PU01），其餘使用0001倉
		LGORT=  '0001'  ,  BWART=  '101'  ,
		ERFMG=QTYRECEIVED ,	ERFME='EA',
		EBELN=left(Pohost.Externpokey ,10),EBELP=dbo.FmtNumStr(EXTERNLINENO,5,0 ) ,ELIKZ=' ',
		SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=REPLICATE(' ',6)
	FROM  Sapi..Bank_POH As Pohost with(nolock) INNER JOIN Sapi..Bank_POD As Pod with(nolock)
		ON POHost.STORERKEY=POD.STORERKEY and POHost.EXTERNPOKEY=POD.EXTERNPOKEY 
	WHERE   POHost.Cfile=@wkey --and QTYRECEIVED!=0     
END   
-- SAP [402]單回轉 
 IF (@Infmod in ('EX06R')  )
BEGIN
	SELECT @MM = SUBSTRING(iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd'),5,2)
	SELECT @BUDAT = iexp.dbo.FmtDateStr(ISNULL(MAX(cdate),GETDATE()),'yyyymmdd') FROM SOHost WHERE EXTERNORDERKEY = @wkey OR cfile = @wkey
	IF @MM <> SUBSTRING(@BUDAT,5,2)
		BEGIN
			SELECT @BUDAT = iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd')
		END
	SELECT BLDAT=iexp.dbo.FmtDateStr(SOHost.edate,'yyyymmdd'),BUDAT=@BUDAT,
		XBLNR=' ',BKTXT=cast(cpcs as varchar(25)),FRBNR=isnull(SOHost.BUYERPO,' '),
		TCODE='MB01' , MATNR=left(SKU,18),
		WERKS=case when SOHost.STORERKEY  in ('01DBSDB99','ESL' ) then 'F001' when SOHost.STORERKEY='FK01' then 'F002'  
								when SOHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(SOHost.STORERKEY),4) end ,
		LGORT=  ' '  ,  BWART= '102'  ,
		ERFMG=SOD.TAX_C , ERFME='EA',
		EBELN=left(SOHost.EXTERNORDERKEY,10),EBELP=dbo.FmtNumStr(EXTERNLINENO,5,0 ) ,ELIKZ=' ',
		SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=REPLICATE(' ',6)
	from SOD with(nolock) INNER JOIN SOHost with(nolock) ON SOHost.STORERKEY=SOD.STORERKEY and SOHost.EXTERNORDERKEY=SOD.EXTERNORDERKEY 
--		where SOHost.EXTERNORDERKEY like '[5,4]%'  and  SHIPPEDQTY!=0 and SOHost.ctyps>0  and 
		where SOHost.EXTERNORDERKEY like '[5,4]%'  and 
		SOD.TAX_C is not null   and   (SOHost.EXTERNORDERKEY=@wkey or SOHost.cfile=@wkey)
END  
-- SAP PO、STO 強制 零結單
IF (@Infmod in ('EX70')  )
BEGIN
	SELECT EBELN=left(WIHost.EXTERNRECEIPTKEY,10) from WIHost with(nolock)
		where   EXTERNRECEIPTKEY=@wkey  or  cfile=@wkey
END  
--  EX07 來源PO (內/外購轉單)  : 專用格式 EX_WMMBID02.fmt
IF (@Infmod in ('EX07PO')  )
BEGIN
	SELECT @MM = SUBSTRING(iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd'),5,2)
	SELECT top 1  @BUDAT = iexp.dbo.FmtDateStr(ISNULL(MAX(rdate),GETDATE()),'yyyymmdd') FROM POHost WHERE EXTERNPOKEY = @wkey 
	IF @MM <> SUBSTRING(@BUDAT,5,2)
		BEGIN
			SELECT @BUDAT = iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd')
		END
	SELECT BLDAT=iexp.dbo.FmtDateStr(isnull(cdate,getdate()),'yyyymmdd'),BUDAT=@BUDAT ,
		XBLNR=isnull(VPO,' '),BKTXT=cpcs,FRBNR=isnull(left(IFPO,16),' '),TCODE='MB01',MATNR=left(SKU,18),
		WERKS=case when POHost.STORERKEY='01DBSDB99' then 'F001' when POHost.STORERKEY='FK01' then 'F002'  
								when POHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(POHost.STORERKEY),4) end ,
		LGORT=' ',BWART='101', ERFMG=QTYRECEIVED,	ERFME='EA',
		EBELN=case when pomsg='ZEL' then '' else left(POHost.EXTERNPOKEY,10) end,EBELP=case when pomsg='ZEL' then '' else left(EXTERNLINENO,6) end, ELIKZ = '' , 
		SGTXT=  left(POHost.EXTERNPOKEY,10)+' '+cast(isnull(dpqty,0) as varchar(5))+'/'+cast(isnull(caqty,0) as varchar(4))  , KZBEW='B',
		VLIEF_AVIS=case when pomsg in ('ZEL','Z68') then left(POHost.EXTERNPOKEY,10) else '' end,VBELP_AVIS=case when pomsg in ('ZEL','Z68') then left(EXTERNLINENO,6) 
		else '' end
		, POD.oqty - POD.QTYRECEIVED 
	FROM  POHost  with(nolock) INNER JOIN POD with(nolock)
		ON POHost.STORERKEY=POD.STORERKEY and POHost.EXTERNPOKEY=POD.EXTERNPOKEY 
	WHERE  QTYRECEIVED!=0    and  (POHost.EXTERNPOKEY=@wkey or POHost.cfile=@wkey) 
	and POHost.STORERKEY= dbo.StrArgIndex(@ExtkeyS ,3)  
 
END
--  EX07 Vendor->CP->F003->CD , Vendor->CP->C001 : 專用格式 EX_WMMBID02.fmt
IF (@Infmod in ('EX07BankPO')  )
BEGIN
	SELECT 	BLDAT= dbo.FmtDateStr(edate,'yyyymmdd'), BUDAT= dbo.FmtDateStr(edate,'yyyymmdd'),
			XBLNR= '' , BKTXT=' '  ,			
			FRBNR= '' ,
			TCODE='MB01', MATNR='00000'+Right(SKU,13),
			WERKS= Case Pohost.Storerkey When 'DBE1' Then 'C001' Else Replace(Pohost.Storerkey,'01DBS','') End   ,
			LGORT='0001',BWART='101', ERFMG= Case when CQTY>STOQTY then   cast(STOQTY as varchar(6)) else cast(CQTY as varchar(6)) end    , 
			ERFME='EA',
			EBELN=left(Pod.Externpokey ,10), EBELP= right(Pod.EXTERNLINENO ,5)  ,   
			ELIKZ = '' , SGTXT=' ' ,KZBEW='B',
			VLIEF_AVIS=' ' ,  VBELP_AVIS=' '
			, Pod.oqty - Pod.CQTY
	FROM  Sapi..Bank_POH As Pohost with(nolock) INNER JOIN Sapi..Bank_POD As Pod with(nolock)
		ON POHost.STORERKEY=POD.STORERKEY and POHost.EXTERNPOKEY=POD.EXTERNPOKEY 
	WHERE  QTYRECEIVED!=0    and  POHost.Cfile=@wkey 
END

--  EX07 來源WI (內/外購轉單)  : 專用格式 EX_WMMBID02.fmt
IF (@Infmod in ('EX07WI')  )
BEGIN
	SELECT @MM = SUBSTRING(iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd'),5,2)
	SELECT @BUDAT = iexp.dbo.FmtDateStr(ISNULL(MAX(rdate),GETDATE()),'yyyymmdd') FROM WIHost WHERE EXTERNRECEIPTKEY = @wkey OR cfile = @wkey
	IF @MM <> SUBSTRING(@BUDAT,5,2)
		BEGIN
			SELECT @BUDAT = iexp.dbo.FmtDateStr(GETDATE(),'yyyymmdd')
		END
	select BLDAT=iexp.dbo.FmtDateStr(WIHost.edate,'yyyymmdd'),BUDAT=@BUDAT,
		XBLNR=' ',BKTXT=cast(rpcs as varchar(25)),FRBNR=' ',
		TCODE='MB01',
		MATNR=left(SKU,18),
		WERKS=case when WIHost.STORERKEY  in ('01DBSDB99','ESL' )  then 'F001' when WIHost.STORERKEY='FK01' then 'F002'  
								when WIHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(WIHost.STORERKEY),4) end ,
		LGORT=' ', BWART='101',
		ERFMG=WID.QTYRECEIVED,ERFME='EA',
		EBELN=left(WIHost.EXTERNRECEIPTKEY,10),
		EBELP=dbo.FmtNumStr(EXTERNLINENO,5,0 )   ,ELIKZ=' ',
		SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=REPLICATE(' ',6)
		, WID.opqty - WID.QTYRECEIVED
	from WID with(nolock) INNER JOIN WIHost with(nolock)
		ON WIHost.STORERKEY=WID.STORERKEY and WIHost.EXTERNRECEIPTKEY=WID.EXTERNRECEIPTKEY 
		where  WID.QTYRECEIVED>0  and  (WIHost.EXTERNRECEIPTKEY=@wkey or cfile=@wkey )
		and left(WID.EXTERNRECEIPTKEY,1) not in ('A','B','C','D','E','X','Y','Z')
		--and WIHost.rdate is not null
END
--  EX07 來源WI 門市調撥GI單
IF (@Infmod in ('EX07GI')  )
BEGIN
	select BLDAT=iexp.dbo.FmtDateStr(WIHost.edate,'yyyymmdd'),BUDAT=iexp.dbo.FmtDateStr(WIHost.rdate,'yyyymmdd'),
		XBLNR=' ',BKTXT=cast(rpcs as varchar(25)),FRBNR=' ',
		TCODE='MB01',
		MATNR=left(SKU,18),
		WERKS=case when WIHost.STORERKEY  in ('01DBSDB99','ESL' )  then 'F001' when WIHost.STORERKEY='FK01' then 'F002'  
								when WIHost.STORERKEY='DBE1' then 'C001'  else right(rtrim(WIHost.STORERKEY),4) end ,
		LGORT=' ', BWART='101',
		ERFMG=WID.QTYRECEIVED,ERFME='EA',
		EBELN=left(WID.DCKEY ,10),
		EBELP=dbo.FmtNumStr(DCLINENO ,5,0 )   ,ELIKZ=' ',
		SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=REPLICATE(' ',6)
		, WID.opqty - WID.QTYRECEIVED
	from WID with(nolock) INNER JOIN WIHost with(nolock)
		ON WIHost.STORERKEY=WID.STORERKEY and WIHost.EXTERNRECEIPTKEY=WID.EXTERNRECEIPTKEY 
		where  WID.QTYRECEIVED>0  and  (WIHost.EXTERNRECEIPTKEY=@wkey or cfile=@wkey )
END 
--=========== SOInvoics   
-- @wkey單號 + @ExtkeyS select CONVERT(varchar(16), GetDATE(), 121)  
IF (@Infmod='SO90Fmt')    
BEGIN
	IF EXISTS(select * from SOInvoice with(nolock) where cfile=@wkey   and ctyps>=9)
	BEGIN
		SELECT GUINO,GUIDATE=CONVERT(varchar(16), GUIDATE, 121)  ,CarrierType,CarrierId1,CarrierId2, inv.PrintMark ,InvoiceType,CustomsClearanceMark=ClearanceMark,DONATION,EmailAddress=C_mail,NPOBAN,KUNDEUINR,TITLE,MEM_NO,CARD_NO,ZCRCARD,
				CARD_TYPE ,  VSART=ExcType,NAME,STRAS=C_address ,STATE=C_STATE ,ORT01=C_city,PSTLZ=C_zip ,LAND1=C_country ,TELF1=C_telfax,MOBILE=C_phone1,LKOND,KEEP,KEEPPY,TYPE='C001',CURCY,DATE=oddate ,TOTAL,NUMBER,SHIPPING_NUMBER, REMARK,RandomNumber=isnull(RandomNumber,'0000'),ERPCategory,  ContactNotes
			from iexp..SOInvoice as INV with(nolock),iexp..SOHost as H with(nolock) where INV.EXTERNORDERKEY=H.EXTERNORDERKEY and  INV.cfile=@wkey    
		SELECT ORDER_DETAIL_Id='1';
		SELECT SHIPPING_NUMBER=INV.EXTERNORDERKEY,FlowNumber=EXTERNLINENO,TaxType,Quantity=oqty,UnitPrice=PRICE_C ,Unit=UOM,ProductType,SequenceNumber=substring(D.SKU,6,13) ,
				ProductName=isnull( (Select descr from SKUD with(nolock) where SKUD.SKU= D.SKU and SKUD.STORERKEY='01DB' )  ,'測試/待建商品')  ,
				Tax=TAX_C ,WithoutTax   , Amount, Discount=isnull(DISCOUNT_C,0) ,ORDER_DETAIL_Id='1'
			from  iexp..SOInvoice as INV with(nolock),iexp..SOD as D with(nolock) where  INV.EXTERNORDERKEY=D.EXTERNORDERKEY and INV.cfile=@wkey    
				--  D.EXTERNORDERKEY=@wkey and INV.GUINO=dbo.StrArgIndex(@ExtkeyS ,3) ;
	END
END
--=========== SOInvoics  銷退
-- WMS Receipt closed   ;  RECEIPT.EXTERNASNTYPE='ReturnSale'
IF (@Infmod='SO98Fmt')    
BEGIN
		IF exists(select * from tempdb.dbo.sysobjects where id = object_id('tempdb..#Rso'))
			drop table #Rso
		Select ReceiptKey, ExternReceiptKey,ExternLineNo,SKU='00000'+SKU,QtyReceived into #Rsod  from [EDLWMS].PROD.dbo.RECEIPTDETAIL with(nolock) where  Status='9'  and  ExternReceiptKey=@wkey
		--select ExternReceiptKey,ExternLineNo,SKU,QtyReceived  from #Rsod
		-- 發票頭不動 
		SELECT GUINO,GUIDATE=CONVERT(varchar(16), GUIDATE, 121),CarrierType,CarrierId1,CarrierId2, inv.PrintMark ,InvoiceType,CustomsClearanceMark=ClearanceMark,DONATION,EmailAddress=C_mail,NPOBAN,KUNDEUINR,TITLE,MEM_NO,CARD_NO,ZCRCARD,
				CARD_TYPE ,  VSART=ExcType,NAME,STRAS=C_address ,STATE=C_STATE ,ORT01=C_city,PSTLZ=C_zip ,LAND1=C_country ,TELF1=C_telfax,MOBILE=C_phone1,LKOND,KEEP,KEEPPY,TYPE,CURCY,DATE=oddate ,TOTAL,NUMBER,SHIPPING_NUMBER, REMARK, RandomNumber=isnull(RandomNumber,'0000'), ERPCategory,ContactNotes=Extkey05
			from iexp..SOInvoice as INV with(nolock),iexp..SOHost as H with(nolock) where  INV.EXTERNORDERKEY=H.EXTERNORDERKEY  and INV.GUINO in (Select GUINO from iexp..SOD as D with(nolock),#Rsod where D.EXTERNORDERKEY=#Rsod.ExternReceiptKey and  cast(D.EXTERNLINENO as INT )=cast(#Rsod.ExternLineNo as INT ) and  D.SKU= #Rsod.SKU ) 
 		SELECT ORDER_DETAIL_Id='98';
		SELECT SHIPPING_NUMBER=INV.EXTERNORDERKEY,FlowNumber=D.EXTERNLINENO,TaxType,Quantity=QtyReceived ,UnitPrice=PRICE_C ,Unit=UOM,ProductType, SequenceNumber=substring(D.SKU,6,13) ,
			 ProductName=isnull( (Select descr from SKUD with(nolock) where SKUD.SKU=D.SKU and SKUD.STORERKEY='01DB' )  ,'測試/待建商品')  ,
			Tax=TAX_C ,WithoutTax , Amount, Discount=isnull(DISCOUNT_C,0)  ,ORDER_DETAIL_Id='98'
			from  iexp..SOInvoice as INV with(nolock) JOIN iexp..SOD as D with(nolock) ON  INV.EXTERNORDERKEY=D.EXTERNORDERKEY
				JOIN #Rsod ON D.EXTERNORDERKEY=#Rsod.ExternReceiptKey and  cast(D.EXTERNLINENO as INT )=cast(#Rsod.ExternLineNo as INT ) and  D.SKU=#Rsod.SKU 
			where  D.EXTERNORDERKEY=@wkey	
			--and D.GUINO=@ExtkeyS;
END

--MSM發運
IF (@Infmod='EX2MSM')   
BEGIN
	Select 
		LH.LogisticsShipNumber As Shipnumber , LD.CaseID As Casenumber , CD.CaseID_Original As Packnumber , 
		LD.Caits_Check ,
		Case CH.CaseType When 'PO' Then 'Ship' When 'SO' Then 'Ship' Else CH.CaseType End As Casetype ,
		Case CD.CaseType When 'PO' Then 'Ship' When 'SO' Then 'Ship' Else CH.CaseType End As PackType ,
		LH.Route As Hostreference , Case CD.Storerkey When 'F001' Then '01DBSDB99' Else CD.Storerkey End As Storerkey , 
		isnull(Case CD.Pokey When '' Then '0000000000' Else CD.Pokey End , '0000000000' )  As Pokey , 
		isnull(Case CD.orderkey When '' Then '0000000000' Else CD.orderkey End , '0000000000') As Orderkey , 
		isnull(Case CD.OrderLineNumber When '' Then '000000' Else iexp.dbo.FmtNumStr(CD.OrderLineNumber,6,0) End, '000000') As OrderLineNo , 
		isnull(Case CD.Pdct_Code When '' Then '000000000000000000' Else CD.Pdct_Code End,'000000000000000000') As Sku ,
		isnull(Case CD.Ori_Code When '' Then '000000000000000000' Else CD.Ori_Code End,'000000000000000000') As Ean ,  
		'' As Descr , isnull(Sum(CD.Act_qty),'0') As Aqty , isnull(CD.Receoptnumber , '') As Receoptnumber , '' As Note
		From Prod..LogisticsShipHead As LH with(nolock)
			Left Join Prod..LogisticsShipDetail As LD with(nolock) On LH.LogisticsShipNumber = LD.LogisticsShipNumber 
			Left Join Prod..CaseHead As CH with(nolock) On LD.CaseID = CH.CaseID 
			Left Join Prod..CaseDetail As CD with(nolock) On LD.CaseID = CD.CaseID And CH.CaseID_Original = CD.CaseID_Original 	
		Where LH.EX1  = @wkey And CH.CaseType in ('PO','SO') and CD.Storerkey <> 'ESL'
		Group By 
			LH.LogisticsShipNumber , LD.CaseID , CD.CaseID_Original , CH.CaseType , LH.Route , CD.Storerkey , CD.Pokey ,
			CD.orderkey , CD.OrderLineNumber , CD.Pdct_Code , CD.Ori_Code , CD.Receoptnumber , LD.Caits_Check , CD.CaseType , 
			CD.CaseID_Original

--union

--	Select 
--		CH.shipment_id As Shipnumber , CH.container_id As Casenumber , CD.container_id_original As Packnumber , 
--		CH.actual_container_count As Caits_Check ,
--		Case CH.container_type When 'PO' Then 'Ship' When 'SO' Then 'Ship' When 'ZP' Then 'Ship' When 'ZS' Then 'Ship' Else CH.container_type End As Casetype ,
--		Case CH.container_type When 'PO' Then 'Ship' When 'SO' Then 'Ship' When 'ZP' Then 'Ship' When 'ZS' Then 'Ship' Else CH.container_type End As PackType ,
--		CH.ship_to_code As Hostreference , Case CD.erp_site When 'F001' Then '01DBSDB99' Else CD.erp_site End As Storerkey , 
--		isnull(Case CD.po_number When '' Then '0000000000' Else CD.po_number End , '0000000000' )  As Pokey , 
--		isnull(Case CD.order_number When '' Then '0000000000' Else CD.order_number End , '0000000000') As Orderkey , 
--		isnull(Case CD.order_line_number When '' Then '000000' Else iexp.dbo.FmtNumStr(CD.order_line_number,6,0) End, '000000') As OrderLineNo , 
--		isnull(Case CD.display_item_number When '' Then '000000000000000000' Else Right(CD.display_item_number,13) End,'000000000000000000') As Sku ,
--		isnull(Case Sk.Ean When '' Then '000000000000000000' Else Sk.Ean End,'000000000000000000') As Ean ,  
--		'' As Descr , isnull(Sum(CD.qty_packed ),'0') As Aqty , isnull(CD.EX1 , '') As Receoptnumber , '' As Note
--		From [192.168.1.52].iexp.dbo.w_Case_S As CH 
--            Left Join [192.168.1.52].iexp.dbo.w_Cased_S As CD On CH.client_code = CD.client_code And CH.container_id = CD.container_id
--            Left Join [192.168.1.52].iexp.dbo.m_Sku As Sk On CD.client_code = Sk.client_code And CD.erp_site = Sk.erp_site And CD.display_item_number = Sk.display_item_number 
--		Where CH.client_code = 'ESL' And CH.shipment_id = right(left(@wkey,16),10)  And CH.container_type  in ('ZS','ZP','PO','SO') And CH.erp_site in('F001','F002')
--		Group By 
--			CH.shipment_id , CH.container_id , CD.container_id_original , CH.container_type , CH.ship_to_code , CD.erp_site , CD.po_number ,
--			CD.order_number , CD.order_line_number , CD.display_item_number , Sk.Ean , CD.EX1 , CH.actual_container_count 
UNION
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
		    From[192.168.1.52].iexp.dbo.w_Case_S As CH with(nolock)
                Left Join [192.168.1.52].iexp.dbo.w_Cased_S As CD with(nolock) On CH.client_code = CD.client_code And CH.container_id = CD.container_id
                Left Join [192.168.1.52].iexp.dbo.m_Sku As Sk with(nolock) On CD.client_code = Sk.client_code And CD.erp_site = Sk.erp_site And CD.display_item_number = Sk.display_item_number 
		    Where CH.client_code = 'ESL'And CH.shipment_id = right(left(@wkey,16),10)  And CH.container_type  in ('ZS','ZP','PO','SO') And CH.erp_site in('F001','F002')
				  And LEFT(CH.container_id,1) = 'W' --AND CH.erp_site = 'F001'
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
			From [192.168.1.52].iexp.dbo.w_Case_S As CH with(nolock)
				Left Join [192.168.1.52].iexp.dbo.w_Cased_S As CD with(nolock) On CH.client_code = CD.client_code And CH.container_id = CD.container_id
				Left Join [192.168.1.52].iexp.dbo.m_Sku As Sk with(nolock) On CD.client_code = Sk.client_code And CD.erp_site = Sk.erp_site And CD.display_item_number = Sk.display_item_number 
			Where CH.client_code = 'ESL' And CH.shipment_id = right(left(@wkey,16),10)  And CH.container_type  in ('ZS','ZP','PO','SO') And CH.erp_site in('F001','F002')
				  And LEFT(CH.container_id,1) <> 'W' --AND CH.erp_site = 'F001'
			Group By 
				CH.shipment_id , CH.container_id , CD.container_id , CH.container_type , CH.ship_to_code , CD.erp_site , CD.po_number ,
				CD.order_number , CD.order_line_number , CD.display_item_number , Sk.Ean , CD.EX1 , CH.actual_container_count 
End
-- 全網COD_  :  開放 Z7 ，
--IF (@Infmod='C001xSP19')  
--BEGIN
--	--Calvin：expDate若無值先用getdate()代替，後續尋找問題。
--	SELECT  Site=CASE WHEN LEFT(SOHost.door, 2) = 'Z4' THEN substring(SOHost.door, 3, 4) ELSE 'C001' END , EXTERNORDERKEY=RIGHT(RTRIM(SOHost.EXTERNORDERKEY), 10)  , 
--		Status='0'  ,expDate= CONVERT(varchar(16), isnull(SOHost.fdate,getdate()), 121)  ,
--		CaseID= ISNULL(ETSeven_XML.Sevenkey, '') , 
--		ROUTE=left(SOHost.door,6)
--	FROM    SOHost LEFT OUTER JOIN E_CAN..ETSeven_XML ON SOHost.EXTERNORDERKEY = E_CAN.dbo.ETSeven_XML.Odkey
--	WHERE   SOHost.door like 'Z%'  and  somsg='C001xSP19' and SOHost.ctyps>0 and   SOHost.cfile=@wkey    
--END 
---<20191008>委使用 轉用CODX
IF (@Infmod='C001xSP19')  
BEGIN
	SELECT  Site=CASE 
				WHEN LEFT(SOHost.door, 2) = 'Z4' THEN substring(SOHost.door, 3, 4) 
				ELSE 'C001' END , 
		EXTERNORDERKEY=CASE
						WHEN SOHost.Storerkey = 'G016' THEN RIGHT(RTRIM(SOHost.Extkey01), 10) 
						ELSE RIGHT(RTRIM(SOHost.EXTERNORDERKEY), 10) END, 
		Status='0'  ,expDate= CONVERT(varchar(16), isnull(max(SOHost.fdate),getdate()), 121)  ,
		 --CaseID=MAX( ISNULL(C.CaseID,'')  ,         
		CaseID=Case when ShipMode in ('Z7')   then MAX(C.CaseID) else  ISNULL(MAX(C.CaseID),'')   end ,   
		ROUTE=left(SOHost.door,6)
	FROM    SOHost with(nolock) LEFT OUTER JOIN PROD..CaseHead as C with(nolock) ON SOHost.EXTERNORDERKEY =C.EX1 
		WHERE   SOHost.door like 'Z%'  and  somsg='C001xSP19'   and SOHost.ctyps>0 and  
		 SOHost.cfile=@wkey 
	GROUP BY EXTERNORDERKEY, SOHost.door,ShipMode,SOHost.Storerkey,SOHost.Extkey01
END 
-- 全網COD_  :   (OLD:expSC_DBE1Z4) 
IF (@Infmod='SO01SO')  
BEGIN
		--<SHIPMENT><HEAD>
		SELECT [DATE]=CONVERT(varchar(16), SOHost.fdate, 121),
						NUMBER=Extkey02 ,	 SHIPPING_NUMBER=EXTERNORDERKEY , 	
						TRACKING_NUMBER=CH.CaseID , VSART=ShipMode   ,SHIPMENT_Id=ROW_NUMBER() OVER( order by EXTERNORDERKEY+CaseID )
		FROM    SOHost  with(nolock) left outer join PROD..CaseHead as CH with(nolock) ON SOHost.EXTERNORDERKEY=CH.EX1    	
			WHERE   SOHost.door like 'Z%'  and  somsg='SO01SO' and SOHost.ctyps>0 
				and SOHost.cfile=@wkey  ;
		-- <SHIPMENT_ID>
		SELECT  SHIPMENT_Id=ROW_NUMBER() OVER(  order by EXTERNORDERKEY+CaseID )
			FROM    SOHost  with(nolock) left outer join PROD..CaseHead as CH with(nolock) ON SOHost.EXTERNORDERKEY=CH.EX1   where cfile=@wkey  ;
		--<ITEM_ID>
		SELECT   ITEMS_Id=ROW_NUMBER()  OVER( order by EXTERNORDERKEY+CaseID ),
						SHIPMENT_Id=DENSE_RANK() OVER(ORDER BY EXTERNORDERKEY+CaseID)   --,EXTERNORDERKEY
				FROM  SOHost with(nolock) left outer join PROD..CaseDetail as Cs with(nolock) ON SOHost.EXTERNORDERKEY=Cs.Pokey  
				WHERE   SOHost.door like 'Z%'  and  somsg='SO01SO' and SOHost.ctyps>0 
								and Pdct_Code not in ('2000000023007','2000000029009') and cfile=@wkey  ;
		 --<ITEM> 
		SELECT  FlowNumber=PokeyLineNumber ,
						NUMBER=Extkey02 ,	 SHIPPING_NUMBER=Pokey , 	
						TRACKING_NUMBER=Cs.CaseID,
						Quantity=Act_qty , SequenceNumber =Pdct_Code , ProductName=isnull( (Select descr from SKUD with(nolock) where SKUD.SKU='00000'+Cs.Pdct_Code and SKUD.STORERKEY='01DB' )  ,'測試/待建商品'),
						 ITEMS_Id=ROW_NUMBER()  OVER( order by EXTERNORDERKEY+CaseID )
		FROM    SOHost with(nolock)  left outer join PROD..CaseDetail as Cs with(nolock) ON SOHost.EXTERNORDERKEY=Cs.Pokey    	
			WHERE   SOHost.door like 'Z%'  and  somsg='SO01SO' and SOHost.ctyps>0 
				and Pdct_Code not in ('2000000023007','2000000029009')
				and SOHost.cfile=@wkey  
 
END
-- 全網 發貨包件 清單 
 
IF (@Infmod='CODX')  
BEGIN
		DECLARE @MBox VARCHAR(20)  ,@odkey VARCHAR(20)
		 select    @odkey=MAX(EXTERNORDERKEY)   From Sohost   Where  cfile = @wkey 
		 select  @MBox=MAX(CaseID)   From Prod..Casedetail where Pokey=@odkey  group by CaseID
		 DELETE  Prod..Casedetail  where Pokey=@odkey and  CaseID<>@MBox 
		--IF isnull(@MBox,'')
		--BEGIN
        Set @Srt = '<?xml version="1.0" encoding="UTF-8"?>' + (
        Select 
            Cdate As [Date] , 
			--<20211030 調Externorderkey>
            Case Storerkey When 'DBE1' Then (
					Case when Sohost.Extkey01 like '202%'  then Externorderkey else Right(Rtrim(Externorderkey),10 ) end) Else Externorderkey End As Shipping_Number , 
			Mon As Order_Number , Case SOHost.Storerkey When 'DBE1' Then 'C001' Else SOHost.Storerkey End As Site ,
            Door As VSART , 
            ( 
            Select 
                Case Sohost.Storerkey When 'DBE1' Then (
					Case when Sohost.Extkey01 like '202%'  then Externorderkey else Right(Rtrim(Externorderkey),10 ) end) Else Externorderkey End As Shipping_Number ,
                dbo.FmtNumStr(Cast(Sod.EXTERNLINENO As Int),6,0) As FlowNumber ,
                CD.CaseID As Tracking_Number , Sum(CD.Act_qty) As Qty , Sod.amount As Price , 
                Case Sod.Storerkey When 'DBE1' Then 'C001' Else Sod.Storerkey End As Stock_Site , 
                Sod.Sku As SequenceNumber , Skud.Descr As ProductName
            From Prod..Casedetail  As CD 
                Left Join Sod On Sod.Externorderkey  = CD.Pokey And Sod.EXTERNLINENO = CD.PokeyLineNumber  
                Left Join Skud On '00000'+ CD.Pdct_Code = Skud.Sku And Skud.STORERKEY = '01DB'
            Where Sohost.Storerkey  = Sod.Storerkey And Sohost.Externorderkey  = Sod.Externorderkey 
                And Sod.Sku Not in ('000002000000023007' , '000002000000029009')  and  CD.CaseID=@MBox
			Group By  Sod.EXTERNLINENO , CD.CaseID , Sod.amount , Sod.Storerkey , Sod.EXTERNORDERKEY ,
				Sod.Sku , Skud.Descr --And Sohost.cfile  = 'COD_DBE1201128184503347.TXT' 
            For Xml Path ('Item')  , TYPE ) As Items       
        From Sohost 
            Where Sohost.cfile = @wkey  
        For Xml Path ('Shipment') , root('Shipments') )
        Select Replace(@Srt,'&#x20;',' ') 

		--END
   --     Select 
   --         Cdate As [Date] , 
   --         Case Storerkey When 'DBE1' Then Right(Rtrim(Externorderkey),10) Else Externorderkey End As Shipping_Number , Mon As Order_Number , Case SOHost.Storerkey When 'DBE1' Then 'C001' Else SOHost.Storerkey End As Site ,
   --         Door As VSART ,         
   --         ( 
   --         Select 
   --             Case Sohost.Storerkey When 'DBE1' Then Right(Rtrim(Externorderkey),10) Else Externorderkey End As Shipping_Number ,
   --             dbo.FmtNumStr(Cast(Sod.EXTERNLINENO As Int),6,0) As FlowNumber ,
   --             CD.CaseID As Tracking_Number , Sum(CD.Act_qty) As Qty , Sod.amount As Price , 
   --             Case Sod.Storerkey When 'DBE1' Then 'C001' Else Sod.Storerkey End As Stock_Site , 
   --             Sod.Sku As SequenceNumber , Skud.Descr As ProductName
   --         From Prod..Casedetail As CD 
   --             Left Join Sod On Sod.Externorderkey  = CD.Pokey And Sod.EXTERNLINENO = CD.PokeyLineNumber  
   --             Left Join Skud On '00000'+ CD.Pdct_Code = Skud.Sku And Skud.STORERKEY = '01DB'
   --         Where Sohost.Storerkey  = Sod.Storerkey And Sohost.Externorderkey  = Sod.Externorderkey 
   --             And Sod.Sku Not in ('000002000000023007' , '000002000000029009')
			--Group By  Sod.EXTERNLINENO , CD.CaseID , Sod.amount , Sod.Storerkey , Sod.EXTERNORDERKEY ,
			--	Sod.Sku , Skud.Descr --And Sohost.cfile  = 'COD_DBE1201128184503347.TXT' 
   --         For Xml Path ('Item')  , TYPE ) As Items       
   --     From Sohost 
   --         Where Sohost.cfile = @wkey
   --     For Xml Path ('Shipment') , root('Shipments') )
   --     Select Replace(@Srt,'&#x20;',' ') 
 
END

-- EC進貨庫存查詢 
--更新到 PLACEOFISSUE=@wkey 
IF (@Infmod in ('SBEX')  )
BEGIN
	--PRINT dbo.StrArgIndex(@ExtkeyS ,2)+'~' + dbo.StrArgIndex(@ExtkeyS ,3) 
	select  D.SKU, QTYRECEIVED=sum(D.QTYRECEIVED)     --,  CONVERT(varchar(16),max(H.ACTDATE) , 120) 
	from [EDLWMS].PROD.dbo.RECEIPT as H with(nolock) INNER JOIN [EDLWMS].PROD.dbo.RECEIPTDETAIL as D with(nolock)
		ON H.STORERKEY=D.STORERKEY and H.EXTERNRECEIPTKEY=D.EXTERNRECEIPTKEY 
		where H.STORERKEY in ('DBE1') and H.STATUS='9' and  D.QTYRECEIVED>0 and EXTERNASNTYPE in ('PO','10','30') 
		and H.ACTDATE>=cast(dbo.StrArgIndex(@ExtkeyS ,2) as datetime)
		and  H.ACTDATE<  cast(dbo.StrArgIndex(@ExtkeyS ,3) as datetime)
		Group by D.SKU
END 
-- 出口 發運(打版批號) <20190702>
IF (@Infmod='EX08_CaseShip')  
BEGIN
 	select 
 				PARTNER_ID=LogisticsShipNumber ,
 				TANUM= '3PstoDN' ,
				KODAT=iexp.dbo.FmtDateStr( getdate() ,'yyyymmdd') ,  KOMUE='X',WABUC='X',WADAT_IST=iexp.dbo.FmtDateStr( getdate() ,'yyyymmdd') ,  
				VBELN_VL=orderkey,   
				POSNR_VL= right(OrderLineNumber,6)   ,   
				VBELN= LogisticsShipNumber ,
				POSNN=  dbo.FmtNumStr((ROW_NUMBER() OVER(order by max(pokey)) )  ,6,0)    ,    
				PIKMG=Act_qty  , 
				MATNR='00000'+left(Pdct_Code,13) ,
				 APDa =right(rtrim(D.Storerkey) ,4)   --調整為F003
	from PROD..LogisticsShipDetail as H with(nolock) left  outer join PROD..CaseDetail as D with(nolock) ON H.CaseID=D.CaseID where LogisticsShipNumber=@wkey
	Group by  D.Storerkey, Door , Pdct_Code ,orderkey ,OrderLineNumber ,Act_qty ,LogisticsShipNumber
	order by orderkey,  OrderLineNumber
END 
-- XSOD.STO  轉銷售 
IF (@Infmod='EX08_Sale')  
BEGIN
	select PARTNER_ID=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,TANUM='1',
		KODAT=iexp.dbo.FmtDateStr(XSOHost.cdate,'yyyymmdd'),KOMUE='X',WABUC='X',
		WADAT_IST=iexp.dbo.FmtDateStr(XSOHost.edate,'yyyymmdd'),
		VBELN_VL=left(XSOD.EXTERNORDERKEY,10),POSNR_VL=right(EXTERNLINENO,6),
		VBELN=right(rtrim(door),4)+case when SPdate is null then iexp.dbo.FmtDateStr(XSOHost.cdate,'yymmdd') else SPdate end,POSNN=right(EXTERNLINENO,6),
		PIKMG=cast(SHIPPEDQTY as varchar(15)),MATNR=left(SKU,18),APDa='ZRD1'
	from XSOD with(nolock) inner join  XSOHost with(nolock) ON XSOHost.STORERKEY=XSOD.STORERKEY and XSOHost.EXTERNORDERKEY=XSOD.EXTERNORDERKEY 
	where  XSOHost.ctyps>0 and (XSOHost.BUYERPO=@wkey or XSOHost.cfile=@wkey)  
END 

-- 貨態回告。
If (@Infmod = 'OrderStatus' )
Begin
	SET @par1=dbo.StrArgIndex(@ExtkeyS ,2)
    Print @InfMod +' / '+  @wkey +' / '+ @par1																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																					
		Set @Srt =  '<?xml version="1.0" encoding="UTF-8"?>' +   (
		Select  top 1
				[DATE]=left(dbo.FmtDateS(Pack_Date,'TimeKey') ,12) ,
				OrderNumber= case when SOHOST.Storerkey='G016' then Extkey01 else EXTERNORDERKEY end  ,   
				Site=Case  SOHOST.Storerkey When 'DBE1' Then 'C001' Else  SOHOST.Storerkey End ,
				VSART=ShipMode , 
				BoxNumber=CaseID , 
				ShipDate=left(dbo.FmtDateS(getdate(),'TimeKey') ,12)  , 
				Status='5' , 
				Err_Status = NULL   ,
				Err_Msg =    NULL  
		From   SOHOST left outer join PROD.dbo.CaseHead(NOLOCK)  as H  On  EXTERNORDERKEY=EX1
				Where   EX1=@par1	--and EXTERNSOSTATUS>=9
		For XML Path('Order') , Root('Orders') )
		 Select Replace(@Srt,'&#x20;',' ') 
 
END

If (@Infmod = 'OrderStatus1' )
Begin
	SET @par1=dbo.StrArgIndex(@ExtkeyS ,2)
	SET @par2=dbo.StrArgIndex(@ExtkeyS ,3)
    Print @InfMod +' / '+  @wkey +' / '+ @par2
 		Set @Srt =  '<?xml version="1.0" encoding="UTF-8"?>' +   (
		 Select 
				[DATE]=left(dbo.FmtDateS(getdate(),'TimeKey') ,12)  ,  
				OrderNumber= case when  Storerkey='G016' then Extkey01 else EXTERNORDERKEY end  ,   
				Site=Case  Storerkey When 'DBE1' Then 'C001' Else  Storerkey End ,
				VSART=C_ROUTE , 
				BoxNumber=null , 
				ShipDate=left(dbo.FmtDateS(getdate(),'TimeKey')  ,12)  ,  
				Status=@par2 , 
				Err_Status =  NULL    ,
				Err_Msg =   NULL  
		From  SOHost with(nolock)    
				Where   EXTERNORDERKEY=@par1
		For XML Path('Order') , Root('Orders') )
		
		Select Replace(@Srt,'&#x20;',' ') 
 
END
--===========================================
--IF (@Infmod in ('WEX07')  )
--BEGIN
--	select   
--		BLDAT=iexp.dbo.FmtDateStr(D.ADDDATE,'yyyymmdd'),BUDAT=CONVERT(varchar(16),H.ACTDATE, 120)  ,
--		XBLNR=' ',BKTXT=cast(ACTQTY as varchar(6)),FRBNR=' ',
--		TCODE='MB01', MATNR=left(SKU,18),
--		WERKS=case when H.STORERKEY='DBE1' then 'C001'  else  right(H.STORERKEY,4) end,
--		LGORT= case when EXTERNASNTYPE='30' then ' ' else '0001' END , BWART='101',
--		ERFMG=D.QTYRECEIVED,ERFME='EA',
--		EBELN=left(H.EXTERNRECEIPTKEY,10), EBELP=dbo.FmtNumStr(EXTERNLINENO,5,0 )   ,
--		ELIKZ=' ', SGTXT=' ',KZBEW='B',VLIEF_AVIS=' ',VBELP_AVIS=' '
--	from [EDLWMS].PROD.dbo.RECEIPT as H INNER JOIN [EDLWMS].PROD.dbo.RECEIPTDETAIL as D
--		ON H.STORERKEY=D.STORERKEY and H.EXTERNRECEIPTKEY=D.EXTERNRECEIPTKEY 
--		where H.STORERKEY in ('DBE1','G011') and H.STATUS='9' and  D.QTYRECEIVED>0 and EXTERNASNTYPE in ('PO','10','30') 
		--and H.ACTDATE>=case when  DATEPART(hh,GETDATE())<=12 then  CONVERT(varchar(10) ,cast(@wkey AS datetime)-1 ,112 ) +' 18:00' else  @wkey+' 12:00'  end  
		--and  H.ACTDATE<  case when  DATEPART(hh,GETDATE())>=18 then   @wkey +' 18:00' else  @wkey+' 12:00'  end  
--END 
---<20191008> 修改  C001xSP19
   --         Select DISTINCT
   --             Case Sohost.Storerkey When 'DBE1' Then Right(Rtrim(Externorderkey),10) Else Externorderkey End As Shipping_Number ,
   --             dbo.FmtNumStr(Cast(Sod.EXTERNLINENO As Int),6,0) As FlowNumber ,
   --             CD.CaseID As Tracking_Number ,  CD.Act_qty  As Qty , Sod.amount As Price , 
   --             Case Sod.Storerkey When 'DBE1' Then 'C001' Else Sod.Storerkey End As Stock_Site , 
   --             Sod.Sku As SequenceNumber , Skud.Descr As ProductName
   --         From Prod..Casedetail As CD with(nolock) 
   --             Left Join Sod On Sod.Externorderkey  = CD.Pokey And Sod.EXTERNLINENO = CD.PokeyLineNumber  
   --             Left   Join Skud On '00000'+ CD.Pdct_Code = Skud.Sku And Skud.STORERKEY = '01DB'
   --         Where Sohost.Storerkey  = Sod.Storerkey And Sohost.Externorderkey  = Sod.Externorderkey 
   --             And Sod.Sku Not in ('000002000000023007' , '000002000000029009')
			--Group By  Sod.EXTERNLINENO , CD.CaseID , Sod.amount , Sod.Storerkey , Sod.EXTERNORDERKEY ,
			--	Sod.Sku , Skud.Descr ,CD.Act_qty --And Sohost.cfile  = 'COD_DBE1201128184503347.TXT' 
 --select * From PROD..CaseHead  where AddDate>'211016' and ShipMode like 'Z%'





GO


