delete [master].[dbo].[whitelist] 

/*例外*/
insert into [master].[dbo].[whitelist]([FLAG],[LoginName],[IP],[Hostname],[Group],[Note])
select 'Y','SA','<local machine>','LocalLogin','Local','Local登入'



/*IT*/
insert into [master].[dbo].[whitelist]([FLAG],[LoginName],[IP],[Hostname],[Group],[Note])
select 'Y','SA','192.168.101.38','LNWOC210009','IT','Jacky' union
select 'Y','SA','192.168.101.37','LNWOC210002','IT','Kevin' union
select 'Y','SA','192.168.101.42','LNWOC210007','IT','Leo' union
select 'Y','SA','192.168.101.43','LNWOC210004','IT','Raines' union
select 'Y','SA','192.168.101.44','LNWOC210008','IT','Emy' union
select 'Y','SA','192.168.101.45','LNWOC210003','IT','Ken' union
select 'Y','SA','192.168.101.46','LNWOC210005','IT','Kye' union
select 'Y','SA','192.168.101.210','LNWOC210017','IT','Weilun' union
select 'Y','SA','192.168.101.209','LNWOC210016','IT','Johnson' union
select 'Y','SA','192.168.101.207','LNWOC210001','IT','Jun' union
select 'Y','SA','192.168.101.227','LNWOC210018','IT','Mark' union
select 'Y','SA','192.168.101.208','LNWOC210015','IT','Vicky' union
select 'Y','SA','192.168.101.201','LNWOC210014','IT','Penny' union
select 'Y','SA','192.168.101.41','LNWOC210011','IT','Emma' union
select 'N','SA','192.168.101.221','LNWOC200017','IT','Ben' 

/*Server*/
insert into [master].[dbo].[whitelist]([FLAG],[LoginName],[IP],[Hostname],[Group],[Note])
select 'Y','SA','192.168.16.171','CHUTE-PC','SQLServer','Sorter'				union
select 'Y','SA','192.168.1.134','EDL_WES','SQLServer','EDL_WES'					union
select 'Y','SA','192.168.1.121','EDL_GatewayTest','SQLServer','EDL_GatewayTest'		union
select 'Y','SA','192.168.1.127','HjData_TEST','SQLServer','HjData_TEST'				union
select 'Y','SA','192.168.1.11','EDLWMS','SQLServer','WMS'						union
select 'Y','SA','192.168.1.52','EDL_Gateway','SQLServer','EDL_Gateway'				union
select 'Y','SA','192.168.1.33','GATEWAYA','SQLServer','GATEWAYA'						union
select 'Y','SA','192.168.1.137','HJDATA','SQLServer','HJDATA'					union
select 'Y','SA','192.168.1.133','HJ_ARCHIVE','SQLServer','AlterDB'						union
select 'Y','SA','192.168.1.41','LSWCAPS0001','eCapsDB','eCaps1'			union
select 'Y','SA','192.168.1.42','LSWCAPS0002','eCapsDB','eCaps2'			union
select 'Y','SA','192.168.1.43','LSWCAPS0003','eCapsDB','eCaps3'			union
select 'Y','SA','192.168.1.44','LSWCAPS0004','eCapsDB','eCaps4'			union
select 'Y','SA','192.168.16.42','LSWCAPS0005','eCapsDB','eCaps5'			union
select 'Y','SA','192.168.16.8','LSWCAPS0006','eCapsDB','eCaps6'			union
select 'Y','SA','192.168.1.57','LSWCAPS0007','eCapsDB','eCaps7'			union
select 'Y','SA','192.168.1.58','LSWCAPS0008','eCapsDB','eCaps8'			

/*Workstation*/
insert into [master].[dbo].[whitelist]([FLAG],[LoginName],[IP],[Hostname],[Group],[Note])
select 'Y','SA','192.168.101.135','LNWOC010014','NB','郭慶莊'		union
select 'N','SA','192.168.101.143','LNWOC200007','NB','賴夢如'		Union
select 'Y','SA','192.168.101.148','LNWOC010004','NB','黃照能'	union	
select 'Y','SA','192.168.16.180','LPWOC010002','Workstation','陳群尉'	union	


select 'Y','SA','192.168.16.44','1F-SORTERPC01','Workstation',''			union
select 'Y','SA','192.168.16.1','LPWCAPS0001','Workstation','eCaps01'		union
select 'Y','SA','192.168.16.4','LPWCAPS0002','Workstation','eCaps04'		union
select 'Y','SA','192.168.16.7','LPWCAPS0003','Workstation','eCaps07'		union
select 'Y','SA','192.168.16.10','LPWCAPS0004','Workstation','eCaps10'		union
select 'Y','SA','192.168.16.2','LPWCAPS0005','Workstation','eCaps02'		union
select 'Y','SA','192.168.16.5','LPWCAPS0006','Workstation','eCaps05'		union
select 'Y','SA','192.168.16.9','LPWCAPS0007','Workstation','eCaps09'		union
select 'Y','SA','192.168.16.11','EcapsLine4PPC1','Workstation','eCaps11'		union
select 'Y','SA','192.168.16.15','LPWCAPS0008','Workstation','eCaps15'		union
select 'Y','SA','192.168.16.14','LPWCAPS0009','Workstation','eCaps14'		union
select 'Y','SA','192.168.16.16','LPWCAPS0010','Workstation','eCaps16'		union
select 'Y','SA','192.168.16.45','LPWCAPS0012','Workstation','eCasp08'		union
select 'Y','SA','192.168.16.46','LPWCAPS0013','Workstation','eCaps8-B 箱標籤'			union
select 'Y','SA','192.168.16.13','LPWOC010038','Workstation','eCaps結案PC'		union
select 'Y','SA','192.168.16.17','LPWCAPS0011','Workstation','eCaps11'			union
select 'Y','SA','192.168.16.3','LPWCAPS0003','Workstation','eCaps03'		union
select 'Y','SA','192.168.16.6','LPWCAPS0006','Workstation','eCaps06'		union
select 'Y','SA','192.168.16.12','未定義','Workstation','eCaps12(暫無工作站)'	union

select 'Y','SA','192.168.16.18','LPWPACK0025','Workstation','1F驗放-內01'	union 
select 'Y','SA','192.168.16.19','LPWPACK0021','Workstation','1F驗放-內02'	union 
select 'Y','SA','192.168.16.20','LPWPACK0023','Workstation','1F驗放-內03'	union 
select 'Y','SA','192.168.16.21','LPWPACK0024','Workstation','1F驗放-外01'	union 
select 'Y','SA','192.168.16.22','LPWPACK0026','Workstation','1F驗放-外02'	union 
select 'Y','SA','192.168.16.23','LPWPACK0022','Workstation','1F驗放-外03'	union 

select 'Y','SA','192.168.16.105','LPWFOOD0002','Workstation','食品電腦'		union
select 'Y','SA','192.168.16.167','LPWWORK0002','Workstation','OA-進貨'		union
select 'Y','SA','192.168.16.183','LPWOC010022','Workstation','劉俊廷'	union
select 'Y','SA','192.168.18.251','LPWDIFF007','Workstation','1F-進貨差異劃單'	union
select 'Y','SA','192.168.16.241','LPWOC010027','Workstation','蔡汪霖'	union
select 'Y','SA','192.168.16.244','1F-OFFICE04','Workstation','劉家正'	union
select 'Y','SA','192.168.16.250','1F-OFFICE10','Workstation','陳芷芸'	union
select 'Y','SA','192.168.16.90','LPWOC010013','Workstation','許雁棠'	union


select 'Y','SA','192.168.16.191','LPWPACK0033','Workstation','5F-PACKPC01' union
select 'Y','SA','192.168.16.192','LPWPACK0034','Workstation','5F-PACKPC02' union
select 'Y','SA','192.168.16.193','LPWPACK0035','Workstation','5F-PACKPC03' union
select 'Y','SA','192.168.16.194','LPWPACK0036','Workstation','5F-PACKPC04' union
select 'Y','SA','192.168.16.195','LPWPACK0037','Workstation','5F-PACKPC05' union
select 'Y','SA','192.168.16.196','LPWPACK0027','Workstation','5F-PACKPC06' union
select 'Y','SA','192.168.16.197','LPWPACK0028','Workstation','5F-PACKPC07' union
select 'Y','SA','192.168.16.198','LPWPACK0038','Workstation','5F-PACKPC08' union
select 'Y','SA','192.168.16.200','LPWPACK0029','Workstation','5F-PACKPC11' union
select 'Y','SA','192.168.16.201','LPWPACK0032','Workstation','5F-PACKPC12' union
select 'Y','SA','192.168.16.202','LPWPACK0030','Workstation','5F-PACKPC13' union
select 'Y','SA','192.168.16.203','LPWPACK0031','Workstation','5F-PACKPC14'


/*VPN*/
insert into [master].[dbo].[whitelist]([FLAG],[LoginName],[IP],[Hostname],[Group],[Note])
select 'Y','SA','172.20.0.101','LNWOC210016','IT_VPN','Johnson' union
select 'Y','SA','172.20.0.102','LNWOC210002','IT_VPN','Kevin' union
select 'Y','SA','172.20.0.113','LNWOC210002','IT_VPN','Kevin' union
select 'Y','SA','172.20.0.103','LNWOC210005','IT_VPN','Kye' union
select 'Y','SA','172.20.0.104','LNWOC210007','IT_VPN','Leo' union
select 'Y','SA','172.20.0.105','LNWOC210001','IT_VPN','Jun' union
select 'Y','SA','172.20.0.106','LNWOC210003','IT_VPN','Ken' union
select 'Y','SA','172.20.0.107','LNWOC210004','IT_VPN','Raines' union
select 'Y','SA','172.20.0.108','LNWOC210015','IT_VPN','Vicky' union
select 'Y','SA','172.20.0.109','LNWOC210009','IT_VPN','Jacky' union
select 'Y','SA','172.20.0.110','LNWOC210008','IT_VPN','Emy' union
select 'Y','SA','172.20.0.111','LNWOC210014','IT_VPN','Penny' union
select 'N','SA','172.20.0.112','LNWOC200017','IT_VPN','Ben' union
select 'Y','SA','172.20.0.114','LNWOC210011','IT_VPN','Rmma' union	
select 'Y','SA','172.20.0.115','LNWOC210018','IT_VPN','Mark' union
select 'Y','SA','172.20.0.116','LNWOC210017','IT_VPN','WeiLun' union
select 'Y','SA','172.20.0.17','LNWOC010014','OP_VPN','郭慶莊' 

/*Sorter*/
insert into [master].[dbo].[whitelist]([FLAG],[LoginName],[IP],[Hostname],[Group],[Note])
select 'Y','SA','192.168.100.100','MPC','Sorter','MPC' union
select 'Y','SA','192.168.100.101','CPC','Sorter','CPC' union
select 'Y','SA','192.168.100.109','滑道PC','Sorter','滑道PC' 


SELECT *
FROM [master].[dbo].[whitelist] (nolock)
order by 3
