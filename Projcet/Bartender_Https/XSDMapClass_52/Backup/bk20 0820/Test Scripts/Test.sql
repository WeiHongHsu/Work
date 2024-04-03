-- Examples for queries that exercise different SQL objects implemented by this assembly
 
select SJOB.dbo.XsdBulkCopy('SP01','SP010106000524.PUB','F001','D:\sjob\fmt\xsd\imp\SP01_TEST-len.XSD')



--select dbo.XsdQueryExp('EC01','D:\DEV\SQLCLR\XSD\DataSet-XSD\XSD\EC9098.xsd' ,'C001' ,'SO00030000160001') s
--select SJOB.dbo.XsdQueryExp('F03','D6920190118','C001-Z9,D6920190105,D:\SJOB\fmt\xsd\exp\DsoFmtCK.Xsd','D:\SJOB\fmt\xsd\exp\DsoFmtCK.Xsd')
-- select SJOB.dbo.XsdQueryExp(  'COXD','COXD_SO01SO00041005720001.XML','DBE2,00041005060001, '    ,'\\192.168.1.33\sjob\fmt\XSTest\COXD_Hd.xsd')
--select SJOB.dbo.XsdQueryExp(  'EX01','1119161876','01DBSDB99','\\192.168.1.33\sjob\fmt\xsd\exp\SAP_EX01XSOFmt.xsd')
-- select SJOB.dbo.XsdQueryExp('EX07PO','0880081127','1116,DB,GG','\\192.168.1.33\SJOB\fmt\xsd\exp\SAP_EX07POFmt.xsd')
-- select SJOB.dbo.XsdQueryExp('EX07WI','EX07_DB99_5059019003.PUB','FXXX,5059019003','\\192.168.1.33\SJOB\fmt\xsd\exp\TesT\SAP_EX07WITest.xsd')
 --select SJOB.dbo.XsdBulkCopy('CVSF01','F01ALLCVS20190602.xml','Z1','\\192.168.1.33\sjob\fmt\xsd\imp\CVSF01.xsd')
--   select SJOB.dbo.XsdBulkUType('CVSF01','F01ALLCVS20190602.xml','Z1','\\192.168.1.33\sjob\fmt\xsd\imp\CVSF01.xsd')
--DECLARE  @qy varchar(2000)  =	'select	 top 5 貨主別=Stkey, Infid, 程序=Jtyps , WKEY,轉入=rcount ,轉出=tscount ,狀態= rdo ,參考=Rfkey1+Rfkey2,錯誤= SERR  from SJOB.dbo.JOBQ  WHERE (rdo=119 or rdo is null or rdo<=1 )'
-- SELECT  dbo.CLR_Qhtm(@qy,'Head' )
--select SJOB.dbo.XsdBulkCopy('SO01','D:\SJOB\IMP\SO0100030000180001.xml','C001','D:\sjob\fmt\xsd\imp\SO01-2TRef.xsd')
--select SJOB.dbo.XsdQueryExp('SBEX','SBEX_201810101446.PUB','DBE2,20181009 18:00,20181010 12:00','D:\SJOB\fmt\xsd\exp\SB_WMSPOQtyFmt.xsd')
--select SJOB.dbo.XsdQueryExp(  'SO98Fmt','00040000140001','DBE2,00040000140001, ','D:\SJOB\fmt\xsd\exp\EC_SO98Fmt_2TRF.xsd')

-----------------------------------------------------------------------------------------
-- User defined type
-----------------------------------------------------------------------------------------

-----------------------------------------------------------------------------------------
-- User defined type
-----------------------------------------------------------------------------------------
-- select dbo.AggregateName(Column1) from Table1


--select 'To run your project, please edit the Test.sql file in your project. This file is located in the Test Scripts folder in the Solution Explorer.'
