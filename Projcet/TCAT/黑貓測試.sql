select * 
from _tv_order_master (nolocK)
where customer_code = 't-cat'

select * 
from _tv_order_master (nolocK)
where customer_code = 't-cat'

select * from _tv_order_master (nolocK)
where display_order_number in (
'202308092401801'
,'202308095932701'
,'202308097159601'
,'202308091537201'  
)

202308099107501(¿ù»~¦a§})
202308098923101(¥¿½T¦a§})
WV2308100003

select * 
from _tv_order_detail (nolocK)
where display_order_number in ('202308099107501','202308098923101')

ESL-2681593233006
ESL-2681759319001
ESL-2681759319001
3CH0483
select * 
from _t_allocation_snapshot (nolocK)
where wave_id = 'WV2308100004'

select * from t_wave_master (nolocK) where wave_id = 'WV2308100001'

ESL-2681593233006
3CH0473

select waybill_number,waybill_data,* 
from _tv_order_master (nolocK)
where display_order_number in ('202308099107501')

select waybill_number,waybill_data,* 
from  _t_order_carton_master (nolock)
where display_order_number in ('202308098923101','202308099107501')

select waybill_number,waybill_data,* 
from _tv_order_master (nolocK)
where display_order_number in ('202308098923101')
select * 
from _tv_order_detail (nolocK)
where display_order_number in ('202308098923101')


select * 
from _tv_order_detail (nolocK)
where display_order_number in ('202308098923101')

select waybill_number,waybill_data,* 
from _tv_order_master (nolocK)
where display_order_number in ('202308098923101')

select * 
from _tv_order_detail (nolocK)
where display_order_number in ('202308098923101')



--exec _sp_order_reset 'PZ1','ESL-202308099107501','NO','','','',''

--exec _sp_order_reset 'PZ1','ESL-202308098923101','NO','','','',''


select * from _t_Codelkup (nolocK) where listname = 'SHIPPINGLABEL' and code = 'T-CAT'



