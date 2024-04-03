USE [Iexp]
GO

/****** Object:  StoredProcedure [dbo].[EDI_B2CxShipM_DFC]    Script Date: 8/24/2023 6:40:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





 

/***************************************************
-- ���Ѥ����t�e�Ӫ� ��ƬyData Flow Contral ���� (�M�w�ӷ��ƾ��޿����� ) 
-- ����y�{ : Call me >>  XSD�ӷ� QUERY >> �����ɮ� >> �ƤJJOBQ�A����^�ǰt�e��
-- ��m:    XSD��m :D:\SJOB\fmt\xsd\exp 
 =============================================================================================
  Author		: ken
 Alter Label	: 20220216_Ken
 Alter Date   : 20220216
 Description	: add REY api error retry again ,if still error update table JOBQ_API status = 99

 =============================================================================================

*****************************************************/
CREATE procedure [dbo].[EDI_B2CxShipM_DFC](@Infmod varchar(30), @ExtkeyS  varchar(100),@USID varchar(30) )
AS
declare  @bwkey   varchar(30),@rcnt int , @jcnt int , @MailMsg varchar(800)=''
declare  @stkey     Varchar(30) ,  @EcBar17 varchar(30)
 /********************�¿ߵo�fFTP ( 18:00 ����) 
-- FTP   ESL/E2018S08L22  (�|�h�b�Ƚs)
--- :ESLYYYYMMDDNN.eod : getdate()-0.708 ( 18:00 ����) �@��h���W�ǫ��sType ESLYYYYMMDDNN.eod ( �o���Y��)
*****************************************/
IF (@Infmod='t-cat_Eod')   
BEGIN
	-- �h�Ȥ�ϥ�
	--�p�GOldID�����󤵤Ѫ����
	--�hNcount�q75�}�l�p��BOldID���󤵤Ѥ��
	DECLARE @SPDATE NVARCHAR(20)
	SELECT @SPDATE = Convert(varchar(6),getdate() ,12)
	--Select Convert(varchar(6),getdate() ,12)
	--select Ncount,* from SJOB..EDIinf where Subfun= 'XsdQueryExp'  and Infid='t-cat_Eod'  
	UPDATE SJOB..EDIinf SET OldID = @SPDATE,Ncount = '75' WHERE Subfun= 'XsdQueryExp'  and Infid='t-cat_Eod' AND OldID <> @SPDATE
	UPDATE   SJOB..EDIinf  set @bwkey='ESL'+CONVERT(varchar(8),getdate()+0.25 ,112)+ dbo.FmtNumStr(Ncount,2,0) +'.EOD'  ,Ncount=Ncount+1   
		where   Subfun= 'XsdQueryExp'  and Infid='t-cat_Eod'  

    UPDATE Iexp..w_Case_S  set etyps=10 , SFile=@bwkey , actual_export_date= getdate() , actual_ship_date=CONVERT(varchar(10),getdate()+0.25 ,112) 
         where customer_code = 't-cat'  and   etyps=9
	SET @rcnt=@@ROWCOUNT

	IF @rcnt>0   
	BEGIN
		INSERT  SJOB..JobQ(Stkey , Wkey , Infid ,Jtyps ,sfile      ,EAIpath    ,Fileback     , Para1 ,Para2, Para3,Para4, LotKey1 ,LotKey2, LotKey3 ,rdo ,rcount ,tscount , EDIStep2, EDIStep3  ,utyps)
			SELECT  Stkey=St  ,Wkey=Sfile  ,Infid  ,Jtyps=infCls , sfile=EDILocDr , EAIpath ,  back_one  ,  Infid  ,Sfile, Para3=client_code+','+erp_site ,Para4=Fmtname, 
                            LotKey1='t-cat' , LotKey2='Z2', LotKey3=Subfun , rDo=2 ,rcount=@rcnt ,tscount=0 ,  EDIStep2=SubFun ,EDIStep3=Expmod ,utyps=9 
				from  (select St=client_code+erp_site ,its=count(*) ,client_code ,erp_site ,Sfile  from  Iexp..w_Case_S where customer_code = 't-cat'  and   etyps=10  group by client_code ,erp_site ,Sfile)  as Avt	 inner join 
						(select * from SJOB..EDIinf  where Subfun= 'XsdQueryExp'  and Infid='t-cat_Eod'  ) as A
							ON A.Stkey=Avt.St


		SET @jcnt=@@ROWCOUNT
		IF @jcnt>0   
		BEGIN
		UPDATE Iexp..w_Case_S  set etyps=11
			where customer_code = 't-cat'  and Sfile IN (SELECT WKEY FROM SJOB..JOBQ WHERE adate> GETDATE()-7 and  Infid='t-cat_Eod' )  and etyps=10
		END
		PRINT 'Z2�w���ץX:['+@bwkey+']  '+ cast(@rcnt as varchar(6)) +' Rows'
		SET @Infmod='CheckLog>Shiped_Rp' 
	END
END

 /******************** 7-11�o�f************
�@��h���W��  785001 YYYYMMDD NN . xml �ɦW���򴫤��I	 (�W�ǫ��sETyps )
�����ɦW75 �}�l
�����I: 11:35 ������F �W�L�u�����j��q��C
**���n��F�j���q�i�窺�]�q�q���ƽаȥ���05:00AM�e�W�ǡA�_�h�L�k���Q�禬�C** 
*************************************************/ 
IF (@Infmod='Seven_Sin')   
BEGIN
	UPDATE   Iexp..w_Case_S  set etyps=20 ,SFile='���^��' where  customer_code='7-11' and  erp_site in ('G016' ) and Etyps=9
	UPDATE SJOB.dbo.EDIinf Set @bwkey='785001'+CONVERT(varchar(8),getdate()  ,112)+dbo.FmtNumStr(Ncount,2,0)+'.XML' ,  Ncount= Ncount+1 
		 where Stkey=@ExtkeyS and Infid='Seven_Sin'  
	UPDATE   Iexp..w_Case_S set etyps=10 ,SFile=@bwkey , actual_export_date= getdate() , actual_ship_date=CONVERT(varchar(10),getdate()+0.25 ,112) 	  ,
			actual_delivery_date=isnull(actual_delivery_date, getdate()+1.46)
		where  customer_code='7-11' and  Etyps=9 and  client_code+erp_site=@ExtkeyS
	SET @rcnt=@@ROWCOUNT 

	IF  @rcnt>0
	BEGIN
		INSERT  SJOB..JobQ(Stkey , Wkey , Infid     ,Jtyps   ,sfile         ,EAIpath    ,Fileback     , Para1 ,Para2, Para3,Para4, LotKey1 ,LotKey2, LotKey3 ,rdo ,rcount ,tscount , EDIStep2, EDIStep3 ,utyps  )
			SELECT  Stkey  ,Wkey=@bwkey  ,Infid    , infCls ,EDILocDr, EAIpath ,  back_one  ,  Infid  ,@bwkey, Para3=Stkey ,Para4=Fmtname, 
                            LotKey1='7-11' , LotKey2=@bwkey , LotKey3='Z7' , rDo=2 ,rcount=@rcnt ,tscount=0 ,  EDIStep2=SubFun ,EDIStep3=Expmod ,utyps=9  
				from  SJOB..EDIinf  where Stkey=@ExtkeyS and Infid='Seven_Sin'  
 		SET @jcnt=@@ROWCOUNT
		UPDATE  Iexp..w_Case_S  set   etyps=18  where SFile=@bwkey and  customer_code='7-11'  and @jcnt>0
		PRINT '7_11�w���ץX:['+@bwkey+']  '+ cast(@rcnt as varchar(6)) +' Rows'
		SET @Infmod='CheckLog12>Shiped_Rp' 
	END	
END
 /*******************REY �s���a�o�f: API************
�@��h���W��: API �W��  
�����I: 11:35 (Now-0.458) ������ (�̳f�B���D)�C
*************************************************/ 
IF (@Infmod='REY_OrderAdd')   
BEGIN
		UPDATE   Iexp..w_Case_S  set etyps=20 ,SFile='���^��' where  customer_code='CVS' and  erp_site in ('G016'  ) and Etyps=9
		SELECT  @bwkey='REY_OrderAdd' + substring(iexp.dbo.FmtDateS(getdate(),'TimeKey') ,1,10)   
 
		UPDATE   Iexp..w_Case_S   set etyps=10 ,SFile=@bwkey , actual_export_date= getdate() , actual_ship_date=CONVERT(varchar(10),getdate()+0.25 ,112) 	  ,
			actual_delivery_date=isnull(actual_delivery_date, getdate()+1.46)
		where  customer_code='REY' and  Etyps=9  
		--and  client_code+erp_site=@ExtkeyS
		SET @rcnt=@@ROWCOUNT
		IF @rcnt=0
			RETURN
	--�����^��   select container_id_original from  Iexp..w_Case_S  where customer_code='REY' 
		--UPDATE   Iexp..w_Case_S  set EX1=container_id_original where SFile=@bwkey 
		DECLARE  @CaseID nvarchar(30) ,@wKey varchar(20) , @soKey varchar(20) , @shipmKey varchar(20) ,@GetVal varchar(200) , @cnt int=1
		DECLARE DoList_Cursor CURSOR FOR   
			select    container_id , EX1, shipment_id from Iexp..w_Case_S  with(nolock) where customer_code='REY'   and  SFile=@bwkey  order by shipment_id  

		OPEN DoList_Cursor
		FETCH NEXT FROM DoList_Cursor INTO @CaseID ,@wKey , @shipmKey
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC  SJOB..FmtCLableREY_Sp @wKey , @CaseID, @EcBar17 out
			EXEC SJOB..EDI_FamilyApi 'IBAB2C_ORDERADD' , @wKey ,'ExtkeyS'  ,@GetVal out 
			IF exists(select * from SJOB..JOBQ_Api with(nolock) where Wkey=@wKey and  StatusMsg like '%���\%')
			BEGIN
				UPDATE   Iexp..w_Case_S  set Etyps=19 ,SFile =@bwkey , actual_export_date=CONVERT(varchar(8),getdate()+0.25,112)
					where customer_code='REY'    and EX1=@wKey
				INSERT  SJOB..JobQ(Stkey , Wkey , Infid ,Jtyps ,sfile     ,EAIpath    ,Fileback     , Para1 ,Para2, Para3,Para4, LotKey1 ,LotKey2, LotKey3 ,rdo ,rcount ,tscount , EDIStep2, EDIStep3 ,LotKey4  ,utyps)
					SELECT  Stkey='C001'  ,Wkey=@wKey  ,Infid='IBAB2C_ORDERADD'   ,Jtyps='Api' , sfile=@bwkey , EAIpath='' ,  Fileback=''  , 'IBAB2C_ORDERADD'  , @wKey, Para3='' ,Para4='IBAB2C_ORDERADD' , 
									LotKey1=@wKey, LotKey2=@CaseID , LotKey3=@shipmKey , rDo=9 ,rcount=1 ,tscount=1 ,  EDIStep2='EDI_FamilyApi' ,EDIStep3='' ,LotKey4='' ,utyps=9 
			    UPDATE SJOB..JOBQ_API set status = '9' where wkey = @wKey
            
            END
            -- 20220216_ken �ɤW���`���s�A���Ǥ@���åB��W�T��
            ELSE
            BEGIN
                   --Wait for 2 seconds and retry for Steve 
                   WAITFOR DELAY '00:00:02';
                   
                   EXEC  SJOB..FmtCLableREY_Sp @wKey , @CaseID, @EcBar17 out
			       EXEC SJOB..EDI_FamilyApi 'IBAB2C_ORDERADD' , @wKey ,'ExtkeyS'  ,@GetVal out 
                   IF exists(select * from SJOB..JOBQ_Api with(nolock) where Wkey=@wKey and  StatusMsg like '%���\%')
			       BEGIN
			       	        UPDATE  Iexp..w_Case_S  set Etyps=19 ,SFile =@bwkey , actual_export_date=CONVERT(varchar(8),getdate()+0.25,112)
			       		        where  customer_code='REY'    and EX1=@wKey
			       	         INSERT  SJOB..JobQ(Stkey , Wkey , Infid ,Jtyps ,sfile     ,EAIpath    ,Fileback     , Para1 ,Para2, Para3,Para4, LotKey1 ,LotKey2, LotKey3 ,rdo ,rcount ,tscount , EDIStep2, EDIStep3 ,LotKey4  ,utyps)
			       		    SELECT  Stkey='C001'  ,Wkey=@wKey  ,Infid='IBAB2C_ORDERADD'   ,Jtyps='Api' , sfile=@bwkey , EAIpath='' ,  Fileback=''  , 'IBAB2C_ORDERADD'  , @wKey, Para3='' ,Para4='IBAB2C_ORDERADD' , 
			       						    LotKey1=@wKey, LotKey2=@CaseID , LotKey3=@shipmKey , rDo=9 ,rcount=1 ,tscount=1 ,  EDIStep2='EDI_FamilyApi' ,EDIStep3='' ,LotKey4='' ,utyps=9 
			               UPDATE SJOB..JOBQ_API set status = '9' where wkey = @wKey
                  END
                  ELSE
                  BEGIN
                           UPDATE SJOB..JOBQ_API set status = '99',erlog = @GetVal where wkey = @wKey
                  END
          
            END
			FETCH NEXT FROM DoList_Cursor INTO  @CaseID ,@wKey , @soKey  
		END --END WHILE
		CLOSE DoList_Cursor
		DEALLOCATE DoList_Cursor
			SET @Infmod='CheckLog12>Shiped_Rp' 
END 

 /********************  CVS �o�f:  SOAP  web servics ************
�@��h���W��: webapi �W��  �ܺC
�����I: 11:35 (Now-0.458) ������ (�̳f�B���D)�C
*************************************************/ 
IF (@Infmod='CVS_F10' )   
BEGIN
	UPDATE   Iexp..w_Case_S  set etyps=20 ,SFile='���^��' where  customer_code='CVS' and  erp_site in ('G016' ,'QQQQ') and Etyps=9
	UPDATE SJOB.dbo.EDIinf Set  @bwkey='F10CVS169' + substring(iexp.dbo.FmtDateS(getdate(),'TimeKey') ,1,10)   
			where Stkey=@ExtkeyS and Infid='CVS_F10'  
	UPDATE   Iexp..w_Case_S set etyps=10 ,SFile=@bwkey , actual_export_date= getdate() , actual_ship_date=CONVERT(varchar(10),getdate()+0.25 ,112) 	  ,
			actual_delivery_date=isnull(actual_delivery_date, getdate()+1.46)
		where  customer_code='CVS' and  Etyps=9 and  client_code+erp_site=@ExtkeyS
 	 -- ���t�W�ǧ妸 20��
	UPDATE   Iexp..w_Case_S set etyps=11 ,SFile = @bwkey +'_'+ dbo.FmtNumStr(Pfid,3,0)  +'.XML'  , bill_to_fax=Pfid ,  
			actual_export_date= getdate() , actual_ship_date=CONVERT(varchar(10),getdate()+0.25 ,112)  
		FROM (select Storerkey=erp_site , Csid=container_id, Pfid=(ROW_NUMBER() OVER(order by  EX1, container_id  )-1 )/20 +1 from Iexp..w_Case_S  where  customer_code='CVS' and  Etyps=10  ) as B
			where   Iexp..w_Case_S.erp_site=B.Storerkey and Iexp..w_Case_S.container_id=B.Csid 	 

	-- �`�`����( ��� (�B�z���ѧO�X 65) �b ��w �귽�W�Q�t�@�ӳB�z���ꦺ�äw�Q��ܧ@���������묹��)
		INSERT  SJOB..JobQ(Stkey , Wkey , Infid ,Jtyps ,sfile     ,EAIpath    ,Fileback     , Para1 ,Para2, Para3,Para4, LotKey1 ,LotKey2, LotKey3 ,rdo ,rcount ,tscount , EDIStep2, EDIStep3 ,LotKey4  ,utyps)
			SELECT  Stkey=St  ,Wkey=Sfile  ,Infid  ,Jtyps=infCls , sfile=EDILocDr , EAIpath ,  back_one  ,  Infid  ,Sfile, Para3=client_code+','+erp_site ,Para4=Fmtname, 
                            LotKey1='CVS' , LotKey2='Z1', LotKey3=Subfun , rDo=2 ,rcount=@rcnt ,tscount=0 ,  EDIStep2=SubFun ,EDIStep3=Expmod ,LotKey4=cast(its as char(6)) ,utyps=9 
				from  (select St=client_code+erp_site ,its=count(*) ,client_code ,erp_site ,Sfile  from  Iexp..w_Case_S where customer_code = 'CVS'  and   Sfile like @bwkey+'%'  group by client_code ,erp_site ,Sfile)  as Avt  inner join 
						(select * from SJOB..EDIinf  where  Stkey=@ExtkeyS and Infid='CVS_F10' ) as A
							ON A.Stkey=Avt.St
			SET @jcnt=@@ROWCOUNT
	PRINT 'CVS�w���ץX:['+@bwkey+']  '+ cast(@rcnt as varchar(6)) +' Rows'
	IF @jcnt>0	
	BEGIN
		UPDATE  Iexp..w_Case_S  set  etyps=18  where SFile like @bwkey+'%'  and  customer_code='CVS'  
		SET @Infmod='CheckLog12>Shiped_Rp' 
	END	
END
 /******************** FedEx �o�f:   �L�W��************
--- FedEx�o�f: �A�@��h���W�� (SOAP) �F��ۦ�W��  
*************************************************/ 
IF (@Infmod='FedEx')   
BEGIN
	SELECT @bwkey='FedEx'+ CONVERT(varchar(8),getdate()-0.25 ,112)  +'.XML'  
	UPDATE   Iexp..w_Case_S set etyps=10 ,SFile=@bwkey , actual_export_date= getdate() , actual_ship_date=CONVERT(varchar(10),getdate()+0.25 ,112) 	  ,
			actual_delivery_date=isnull(actual_delivery_date, getdate() )
		where  customer_code='FedEx' and  Etyps=9 and  client_code+erp_site=@ExtkeyS
	SET @rcnt=@@ROWCOUNT
	IF @rcnt>0	
	BEGIN
		INSERT  SJOB..JobQ(Stkey , Wkey , Infid     ,Jtyps   ,sfile         ,EAIpath    ,Fileback     , Para1 ,Para2, Para3,Para4, LotKey1 ,LotKey2, LotKey3 ,rdo ,rcount ,tscount , EDIStep2, EDIStep3 ,utyps  )
			SELECT  Stkey  ,Wkey=@bwkey  ,Infid    , infCls ,EDILocDr, EAIpath ,  back_one  ,  Infid  ,@bwkey, Para3=Stkey ,Para4=Fmtname, 
                            LotKey1='FedEx' , LotKey2='Z3', LotKey3=Subfun , rDo=2 ,rcount=@rcnt ,tscount=0 ,  EDIStep2=SubFun ,EDIStep3=Expmod ,utyps=9  
				from  SJOB..EDIinf  where Stkey=@ExtkeyS and Infid='FedEx' 
				SET @jcnt=@@ROWCOUNT
		UPDATE  Iexp..w_Case_S  set   etyps=18  where SFile=@bwkey and  customer_code='FedEx'  and @jcnt>0
		SET @Infmod='CheckLog' 
	END	
END
 
 -------------------------------------------
--- �H�U��[�t�e��]�ݭn�ˬd���`  ��
--IF (isnull(@bwkey,'')<>'' and @Infmod like 'CheckLog%')   
--BEGIN
--	--���s 
--		UPDATE Iexp..Vc_CaseLog set  SFile=  SFile+'_' +CONVERT(varchar(10),ExportDate ,12)   where SFile=@bwkey and  ShipMode='Z9' 

--		INSERT Iexp..Vc_CaseLog(ORDERKEY	,ECNO	,PayType,
--			OrderAmt		,CaseID	,Pack_date		,ShipDate		,ExportDate		,ATotal,	CKEEPPY,
--			CAmount	,LastCase	,Route	,DocName		,Sfile	,ShipMode )   
--		SELECT   
--			 ORDERKEY= H.display_order_number  , ECNO=''   , PayType=CASE H.cod_type WHEN '0' THEN '3' ELSE '1' END  , 
--			 OrderAmt=H.total_amount  , CaseID=container_id  ,   Cs.Pack_date  ,ShipDate=Cs.actual_ship_date    , ExportDate=Cs.actual_export_date ,    ATotal =0,  C_KEEPPY=0  ,
--			CAmount =cast((select sum(Csd.qty_packed*SOD.list_price)  from iexp..w_Sod as SOD, iexp..w_Cased_S  as Csd where Csd.order_number=SOD.order_number and SOD.order_line_number= Csd.order_line_number and   Csd.display_item_number=SOD.item_number   and Csd.container_id=Cs.container_id  ) as int) , 
--			LastCase=Cs.last_container_flag , Route=Cs.customer_code ,  DocName =case when Cs.customer_code='7-11' then REPLACE(Cs.Sfile,'.XML','') else '' end ,  Cs.Sfile,ShipMode='ZZZZZ'
--		FROM  Iexp..w_Case_S  as Cs left outer join  Iexp..w_So as H ON Cs.Ex1=display_order_number and Cs.client_code=H.client_code
--			where    Cs.SFile like  @bwkey+'%'  

--		-- �f�줣�糬��
--		UPDATE  Iexp..Vc_CaseLog set  Ctyps=9  where ShipMode in ('Z2','Z3','Z4')  and  SFile=@bwkey
--		-- �]����ȽT:�{���B�F UPDATE ��left outer join �L��
--		IF  @Infmod like 'Check%1%'  
--		BEGIN
--			UPDATE Iexp..Vc_CaseLog set  CkAmt=Case when OrderAmt=CAmt then 9 else -9 end    ,Cits=Cs, Ext2=Case when OrderAmt=CAmt then  '' else '[�]����B���`]' end  
--				from (Select  Sfile ,EXTERNORDERKEY,Cs=COUNT(*) ,CAmt=SUM(CaseAmt) from PROD..CheckCaseLog  where Sfile like @bwkey+'%'   group by Sfile ,EXTERNORDERKEY  ) as A
--				Where  CheckCaseLog.Sfile=A.Sfile and CheckCaseLog.EXTERNORDERKEY=A.EXTERNORDERKEY 
--			IF   EXISTS(select * from Iexp..Vc_CaseLog where CkAmt=-9 and Sfile like @bwkey+'%' )
--			BEGIN				
--				SET @MailMsg='select	 ShipMode, EXTERNORDERKEY , ECNO ,CaseID  ,����=Route ,Ext2="�]����Ȳ��`", OrderAmt ,CaseAmt ,�I��=C_KEEPPY  from [LOCALHOST].PROD.dbo.CheckCaseLog where CkAmt=-9 and Sfile like "' +@bwkey+'%" '
--	 			exec    SJOB.dbo.TsSJOBMAIL  'jimmyshen@edl.com.tw','ben@edl.com.tw', @MailMsg,
--						'���˵����]�{��  [�]�����-���`] ', '���Ҩӷ�:PROD..CheckCaseLog  ! Upload_Ttyps=1(���W��) '
--				UPdate  Iexp..Vc_CaseLog set  CkAmt=CkAmt-10 WHERE CkAmt=-9 and Sfile like @bwkey+'%' 
--			END
--		END

--		-- �糬��  : Ctyps=0 �������eFTP�� MAIL�q���C
--		IF  @Infmod like 'Check%2%' 
--		BEGIN
--			UPDATE Iexp..Vc_CaseLog set Ext1=S.Storerkey  , Ctyps=9 from iexp..STORER_S as S   
--				where  S.Storerkey=CheckCaseLog.Route  and  S.Route=ShipMode  and CheckCaseLog.Sfile like @bwkey+'%' 

--			IF  EXISTS(select * from Iexp..Vc_CaseLog where Ctyps=0 and ROUTE in ('CVS','7-11','CK') and Sfile like @bwkey+'%'   )  
--			BEGIN
--			   UPDATE  Iexp..w_Case_S  set  etyps=-9 , Sfile='����'+Iexp..w_Case_S.Sfile  from   Iexp..Vc_CaseLog  as S   
--					where  S.ORDERKEY=Iexp..w_Case_S.EX1 and  Iexp..w_Case_S.Sfile=S.Sfile and S.Ctyps=0  and S.ROUTE in ('CVS','7-11','CK')  and  Iexp..w_Case_S.Sfile like @bwkey+'%' 

--				SET @MailMsg='select ROUTE,  ORDERKEY , ECNO ,CaseID  ,����=Route ,Ext2="���N�����`"  from Iexp.dbo.Vc_CaseLog where Ctyps=0 and Sfile like "' +@bwkey+'%" '
--	 			exec    SJOB.dbo.TsSJOBMAIL  '�t�e��UPload','', @MailMsg,
--						'�t�e�� [���N�����`] ', '���N�����`:�ݭn�״_�ƾ�;���f���s�K�� ! Upload_Ttyps=-9(�d�I) '
--			END
--			UPdate  Iexp..Vc_CaseLog  set  Ctyps=Ctyps+19 WHERE Ctyps=0 and Sfile like @bwkey+'%' 
--		END
--	-- 7-11 �W�L 19:30 �����|����XML�W��
--		IF  ( (DATEPART(hh,GETDATE() )=22 and DATEPART(mi,GETDATE()) BETWEEN 0 and 39)  or  
--				(DATEPART(hh,GETDATE() )=11 and DATEPART(mi,GETDATE()) BETWEEN 30 and 59) ) and EXISTS(select * from Iexp..Vc_CaseLog where ShipMode = 'Z7' and Sfile=@bwkey)
--		BEGIN		
--			SET @MailMsg='select	 ShipMode, EXTERNORDERKEY , ECNO ,CaseID  ,����=Route ,Ext2="7-11-�W�ǹO��"  from iexp.dbo.Vc_CaseLog where ShipMode = "Z7" and Sfile="' +@bwkey+'" '
--	 		exec    SJOB.dbo.TsSJOBMAIL  '�t�e��UPload','', @MailMsg  ,
--					' 7-11 [�W�ǹO��-����] ', '7-11�W�L�W�Ǯɶ�(11:30/22:00) ! �гq���w�Ф���X�f;�Ω��ѥX�f Upload_Ttyps=1(���W��) '
--			UPdate  Iexp..Vc_CaseLog  set  Ctyps=Ctyps-10 WHERE   Sfile=@bwkey
--			print  ' 7-11 [�W�ǹO��-���`] '  +@bwkey 
--		END
--		--CVS 
--		IF   (DATEPART(hh,GETDATE() )=11 and DATEPART(mi,GETDATE()) BETWEEN 10 and 35)  and EXISTS(select * from PROD..CheckCaseLog where ShipMode = 'Z1' and Sfile like @bwkey+'%')
--		BEGIN		
--			SET @MailMsg='select	 ShipMode, EXTERNORDERKEY , ECNO ,CaseID  ,����=Route ,Ext2="7-11-�W�ǹO��"  from iexp.dbo.CheckCaseLog where ShipMode = "Z1" and Sfile like "' +@bwkey+'%" '
--	 		exec    SJOB.dbo.TsSJOBMAIL  '�t�e��UPload','', @MailMsg  ,
--					' CVS[�W�ǹO��-����] ', 'CVS�W�L�W�Ǯɶ�(11:00 ) ! �гq���w�Ф���X�f;�Ω��ѥX�f Upload_Ttyps=1(���W��) '
--			UPdate  Iexp..Vc_CaseLog  set  Ctyps=Ctyps-10 WHERE   Sfile like @bwkey+'%' 
--			print  'CVS [�W�ǹO��-���`] '  +@bwkey 
--		END

--END
-- �qCaseHead �d�� 
--IF (isnull(@bwkey,'')<>'' and @Infmod like '%Shiped_Rp%')   
--BEGIN
--	Declare @MailMsg2 Varchar(1000) , @count Varchar(6) , @Ds Varchar(10)
--	Select @count = COUNT(*), @Ds=min(ShipMode ) From Iexp..Vc_CaseLog Where Sfile like @bwkey+'%' 
--	Set @MailMsg2 =@Ds + ' �w�p�W�ǲM��G['+@bwkey+']��ơG'+@count
--	SET @MailMsg='select	�t�e�O=ShipMode, ECNO= ORDERKEY  ,CaseID  ,����=Route   from iexp.dbo.Vc_CaseLog where Sfile like "' +@bwkey+'%" '
--	exec    SJOB.dbo.TsSJOBMAIL  '�t�e��UPload' ,'' , @MailMsg , @MailMsg2 , '�t�e��LIST '
--	UPdate iexp..Vc_CaseLog  set  Ctyps=Ctyps-10 WHERE   Sfile like @bwkey+'%' 
-- END
GO


