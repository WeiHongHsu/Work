/*印表機清單*/
select left(b.device_description,2)as N'樓層',
right(b.device_description,2) as N'工作站編號'
,a.workstation_id as N'工作站名稱'
,b.device_ip as N'IP'
,a.report_name N'報表名稱'
,BTW_Label
--,*
from _t_printer_mapping (nolocK) a
inner join [dbo].[_t_device] (nolocK) b
on a.workstation_id = b.device_id
where left(a.workstation_id ,7) = 'LPWPACK'
and left(a.printer_id,2) not in ( 'CB','PR')
and (left(a.printer_id,2) in ( 'EC','A4')
or left(a.printer_id,3) in ( 'E16'))
--and report_name not in ( 'HJ_RPT_WavePickList_A4L','')
and  left(b.device_description ,2) in ('3F','4F' )
and b.device_type = 'PC'
and left(b.device_id ,7) = 'LPWPACK'
and a.report_name 
in 
('HJ_RPT_ShipList_EC_FEDEX','HJ_RPT_ShipList_EC','HJ_RPT_ShipList_EC_SP','HJ_WBL_CSO_711'
,'HJ_WBL_CSO_ESL','HJ_WBL_CSO_REY','HJ_WBL_CSO_TCAT','HJ_WBL_CSO_ZA','HJ_WBL_CSO_ZD')

order by 1,3,5
--order by printer_id 


