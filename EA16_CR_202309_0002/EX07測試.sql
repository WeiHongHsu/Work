select rdo,* from sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' order by adate desc
select * from sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' and LotKey2 = '5050047001'
select * from sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' and LotKey2 = '5040593353'
select * from sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' and LotKey2 = '5150224643'


select logkey2,* from sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' order by adate desc

--ZS04¡BZS05¡BZS15
select * from W_SO (nolocK) where display_order_number = '5050047001'
select * from W_SO (nolocK) where display_order_number = '5040593353'
select * from W_SO (nolocK) where display_order_number = '5150224654'

select Etyps,* from W_SO (nolocK) where ship_to_code = 'F001' and cfile is not null order by Adddate desc

--update sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' and LotKey2 = '5050047001'
--update sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' and LotKey2 = '5040593178'
--update sjob.dbo.jobq (nolocK) where infid = 'EX07_SO' and LotKey2 = '5150224643'

select SJOB.dbo.XsdQueryExp('EX07_SO','EX07_SO_G0165050047001_230323165700.Pub','ESL,5050047001' ,'D:\SJOB\fmt\xsd\exp\SAP_EX07SO_Fmt.xsd' )
select SJOB.dbo.XsdQueryExp('EX07_SO','EX07_SO_C0015040593353_220302153001.Pub','ESL,5040593353' ,'D:\SJOB\fmt\xsd\exp\SAP_EX07SO_Fmt.xsd' )
select SJOB.dbo.XsdQueryExp('EX07_SO','EX07_SO_C0015150224643_211202175001.Pub','ESL,5150224643' ,'D:\SJOB\fmt\xsd\exp\SAP_EX07SO_Fmt.xsd' )