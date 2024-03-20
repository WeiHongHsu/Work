select * from _tv_order_master (nolocK) where display_order_number = '5040806114'
select * from _tv_order_detail (nolocK) where display_order_number = '5040806114'

select * from _t_order_carton_master (nolocK) where display_order_number = '5040806114'
select * from _t_order_carton_detail (nolocK) where display_order_number = '5040806114'

select comment_text,* from int.iexp.dbo.[w_Case_S] with (nolocK) where ex1 = '5040806114'
select * from int.iexp.dbo.[w_CaseD_S] with (nolocK) where ex1 = '5040806114'

select * from int.iexp.dbo.[W_SO] with (nolocK) where display_order_number = '5040806114'
select * from int.iexp.dbo.[W_SOD] with (nolocK) where display_order_number = '5040806114'
