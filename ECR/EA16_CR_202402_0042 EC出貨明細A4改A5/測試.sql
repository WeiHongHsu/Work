select carton_number ,count(*)
from _t_order_carton_detail (nolocK)
where erp_site = 'C001'
and create_dt >= '20240310'
group by carton_number 
having count(*) = 10


202403139571201 --7
202403139237801 --8
202403110927401
202403116178201

2403021J0K7SEN --7
2403130JVBS7XJ--8
24031304CB2DAC --9
240310MK2YRGPG --10

select * from _t_printer_mapping (nolocK) where workstation_id = 'LNWOC210009'
and report_name in ('HJ_RPT_ShipList_EC','HJ_RPT_ShipList_EC_SP')