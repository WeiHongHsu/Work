select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
order by udf01

/*
https://bartender4.eslite.com/Integration/Json_PDF/Execute
https://bartender4.eslite.com/Integration/Json_STD/Execute
https://bartender4.eslite.com/Integration/CSV_Table/Execute
https://bartender4.eslite.com/Integration/CSV_STD/Execute

https://bartender.eslite.com/Integration/Json_PDF/Execute
https://bartender.eslite.com/Integration/Json_STD/Execute
https://bartender.eslite.com/Integration/CSV_Table/Execute
https://bartender.eslite.com/Integration/CSV_STD/Execute
*/

/*CSV_STD CB*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/CSV_STD/Execute'
and storerkey = 'esl'
order by udf01

/*CSV_STD EC*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/CSV_STD/Execute'
and storerkey in ('C001','G016')
order by udf01

/*CSV_Table CB*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/CSV_Table/Execute'
and storerkey = 'esl'
order by udf01

/*CSV_Table EC*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/CSV_Table/Execute'
and storerkey in ('C001','G016')
order by udf01

/*Json_PDF EC*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'https://bartender.eslite.com/Integration/Json_PDF/Execute'
and storerkey in ('C001','G016')
order by udf01

--begin tran
--update _t_Codelkup 
--set udf01 = 'https://bartender.eslite.com/Integration/Json_PDF/Execute'
--,udf02 = 'https://bartender4.eslite.com/Integration/Json_PDF/Execute'
--where listname = 'btwapiconfig'
--and udf01 = 'http://192.168.1.100/Integration/Json_PDF/Execute'
--rollback tran


/*Json_STD EC*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/Json_STD/Execute'
and storerkey = 'ESL'
order by udf01

/*Json_STD CB*/
select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/Json_STD/Execute'
and storerkey in ('C001','G016')
order by udf01

select * 
from _t_Codelkup (nolock) 
where listname = 'btwapiconfig'
and udf01 = 'http://192.168.1.100/Integration/CSV_Table/Execute'
and storerkey in ('C001','G016')
order by udf01


begin tran
update _t_Codelkup
set udf01 = 'http://192.168.1.100/Integration/Json_STD/Execute'
,udf02 = 'http://192.168.1.104/Integration/Json_STD/Execute'
where listname = 'btwapiconfig'
and udf01 = 'https://bartender.eslite.com/Integration/Json_STD/Execute'
and storerkey in ('C001','G016')
rollback tran

begin tran
update _t_Codelkup 
set udf01 = 'http://192.168.1.100/Integration/CSV_Table/Execute'
,udf02 = 'http://192.168.1.104/Integration/CSV_Table/Execute'
where listname = 'btwapiconfig'
and udf01 = 'https://bartender.eslite.com/Integration/CSV_Table/Execute'
and storerkey in ('C001','G016')
rollback tran

select top 100 * from _t_Bartender_API_LOG (nolock) order by id desc
select top 100 * from [_t_Error_Log] (nolock) 
where userid = 'LPWPACK0003'
order by pid desc