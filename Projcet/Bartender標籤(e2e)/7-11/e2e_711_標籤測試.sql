/*
1.	202308159178401(¨ú³f¥I´Ú)
2.	202308151024301(¨ú³f¥I´Ú)
3.	202308153801601(¨ú³f¤£¥I´Ú)
4.	202308151568701(¨ú³f¤£¥I´Ú)

1.	202308180874301 (¨ú³f¥I´Ú)
2.	202308180563901 (¨ú³f¥I´Ú)
3.	202308181259001 (¨ú³f¤£¥I´Ú)
4.	202308184791201 (¨ú³f¤£¥I´Ú)


'202308180874301',
'202308180563901',
'202308181259001',
'202308184791201'

*/

select * 
from _tv_order_master (nolock) 
where display_order_number 
in ('202308159178401'
,'202308151024301'
,'202308153801601'
,'202308151568701')

select * 
from _tv_order_detail (nolock) 
where display_order_number 
in ('202308159178401'
,'202308151024301'
,'202308153801601'
,'202308151568701')



--WV2308170001


select * from _t_allocation_snapshot (nolock) where wave_id = 'WV2308170001'
order by order_number ,line_number

select * from _t_order_carton_master (nolocK)
where display_order_number 
in ('202308159178401'
,'202308151024301'
,'202308153801601'
,'202308151568701')



select * from _t_Codelkup (nolocK) where listname = 'BTWAPIConfig' and storerkey = 'C001' and code = '7-11'

select * from _t_Codelkup (nolocK) where listname = 'shippinglabel' and storerkey = 'C001' and code = '7-11'
--update _t_Codelkup set long = '_SP_HJ_WBL_CSO_711_EC' Where listname = 'shippinglabel' and storerkey = 'C001' and code = '7-11'
select * from _t_Codelkup (nolocK) where listname = 'shippinglabel' and storerkey = 'C001' 
--lib://711_ShippingLabel.btw
select * from _t_printer_mapping (nolocK) where workstation_id = 'LNWOC210009' and report_name = 'HJ_WBL_CSO_711'
--update _t_printer_mapping set BTW_Label = 'lib://711_ShippingLabel.btw' where workstation_id = 'LNWOC210009' and report_name = 'HJ_WBL_CSO_711'

select * from _t_Error_Log (nolocK) order by 1 desc



WV2308180004

select * 
from _tv_order_master (nolock) 
where display_order_number 
in ('202308180874301',
'202308180563901',
'202308181259001',
'202308184791201')

select * 
from _tv_order_detail (nolock) 
where display_order_number 
in ('202308180874301',
'202308180563901',
'202308181259001',
'202308184791201')


select * from _t_allocation_snapshot (nolock) where wave_id = 'WV2308180004'
order by order_number ,line_number

select * from _t_order_carton_master (nolocK)
where display_order_number 
in ('202308180874301',
'202308180563901',
'202308181259001',
'202308184791201')
