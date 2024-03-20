select * from jobq (nolocK) where wkey like '%5040806114%'
select * from jobq (nolocK) where wkey like '%SP8110185780000628153041.PUB%'

select * from iexp..w_PO (nolocK) where display_po_number = '5040806114'

select * from iexp..w_SO (nolocK) where display_order_number = '5040806114'
select * from iexp..w_SOD (nolocK) where display_order_number = '5040806114'


--select  SJOB.dbo.XsdQueryExp('EX07_SO','EX07_SO_13005040806114_231019214949.Pub','ESL,5040806114' ,'D:\SJOB\fmt\xsd\exp\SAP_EX07SO_Fmt.xsd' )




