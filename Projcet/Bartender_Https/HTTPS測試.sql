DECLARE 
@ResponseText NVARCHAR(4000)
,@status int
 exec [dbo].[_sp_BartenderAPI] 
 'POST',
 N'https://bartender4/Integration/Json_STD_TEST/Execute',
 null
 ,'adadf'
,'JackyHsu'
 , 'LNWOC210009'
 ,'REY_ShippingLabel_202112208694501_20230202142208.Json'
 , 1
 , @status output
 ,@ResponseText output
 select @status,@ResponseText