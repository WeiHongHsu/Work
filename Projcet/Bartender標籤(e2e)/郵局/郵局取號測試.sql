/*��������*/
DECLARE
	  @next_number						NVARCHAR(30) 
	, @rtn_code							NVARCHAR(10) 
	, @rtn_message						NVARCHAR(500) 
	, @ww_result						NVARCHAR(10) 

exec _sp_get_next_number 
'WBLPF'
,@next_number out
,@rtn_code
,@rtn_message
,@ww_result

select @next_number
005001324001
/*�����W�h
ZA : 18 @@@@@@32400118
ZD : 78 @@@@@@32400178
*/

select * from t_control (nolock) where control_type in ('_NO_WBLPF')

/*�����]�w*/
--insert into t_control (control_type,description,next_value,config_display,allow_edit,c1,f1)
select '_NO_WBLPF','�l���B�渹','0','SHOW_VA','1','@@@@@@[324001]','0' 
--select '_NO_WBLZD','ZD�l���B�渹','0','SHOW_VA','1','@@@@@@[32400178]','0'

--delete t_control  where control_type in ('_NO_WBLZA', '_NO_WBLZD')



					

