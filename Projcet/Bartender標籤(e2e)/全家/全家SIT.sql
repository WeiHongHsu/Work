--全家
--202308308602401 取貨付款
--202308301892701 取貨不付款

202309040785601
202309040693101
WV2309060001

/*全家*/
select * 
from [dbo].[_tv_order_master](nolocK) 
where display_order_number in
(
'202308301892701'
,'202308308602401'
)


select * 
from [dbo].[_tv_order_detail](nolocK) 
where display_order_number in
(
 '202308301892701'
,'202308308602401'
)

select * from _t_allocation_snapshot (nolock) where wave_id = 'WV2309060001'

select * 
from _t_order_carton_master (nolocK) 
where display_order_number in
(
 '202308301892701'
,'202308308602401'
)





S202308300001