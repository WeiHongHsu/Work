select * from _tv_order_master (nolocK) where display_order_number = '202212135839202'
--WV2307250001
select waybill_number,* from _tv_order_master (nolocK) where display_order_number = '202212135839202'
select * from _tv_order_detail (nolocK) where display_order_number = '202212135839202'
select * from _tv_order_detail (nolocK) where display_order_number = '202212139410502'
WV2307250004
select waybill_number,* from _t_order_carton_master (nolocK) where display_order_number = '202212135839202'


_sp_batch_order_pick_hu_validate
--update _tv_order_master set customer_code = 'ZA' where display_order_number = '202212135839202'

select * from _t_order_carton_master (nolocK) where display_order_number = '202212135839202'
select * from _t_order_carton_master (nolocK) where wave_id = 'WV2307250001'

select waybill_number,* from _tv_order_master (nolocK) where display_order_number = '202212139410502'
select waybill_number,* from _t_order_carton_master (nolocK) where display_order_number = '202212139410502'
select waybill_number,* from _t_order_carton_master (nolocK) where wave_id = 'WV2307250002'

--exec _sp_order_reset 'PZ1','ESL-202212135839202'


select waybill_number,* from _tv_order_master (nolocK) where display_order_number = '202212160246701'
select * from _tv_order_detail (nolocK) where display_order_number = '202212160246701'
select waybill_number,* from _t_order_carton_master (nolocK) where display_order_number = '202212160246701'

