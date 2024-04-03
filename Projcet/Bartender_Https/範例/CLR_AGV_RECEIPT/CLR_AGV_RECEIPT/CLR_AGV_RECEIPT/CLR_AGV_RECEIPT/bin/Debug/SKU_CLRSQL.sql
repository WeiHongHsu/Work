drop PROCEDURE CallAPI_SKU
drop ASSEMBLY HJWCSAPI


CREATE ASSEMBLY HJWCSAPI
FROM 'D:\Geek+接口\HJWCSAPI20200120\HJWCSAPI\HJWCSAPI\bin\Debug\HJWCSAPI.dll'
WITH PERMISSION_SET=UNSAFE;
GO

--然后执行底下sql，把PROCEDURE挂上去：
--CREATE PROCEDURE [dbo].[CallSKU] 
--WITH EXECUTE AS CALLER 
--AS 
--EXTERNAL NAME [HJWCSAPI].[StoredProcedures].[API_SKU]


CREATE PROCEDURE [dbo].[CallAPI_SKU] 
(@skuno nvarchar(50),@whid nvarchar(10),@ok int OUTPUT 
) 
WITH EXECUTE AS CALLER 
AS 
EXTERNAL NAME [HJWCSAPI].[StoredProcedures].[API_SKU]
GO

DECLARE @aaa int
EXEC	[dbo].[CallAPI_SKU] '2000000085005','01' ,@aaa
select @aaa

select * from int_api_log

FNFG00000
2000000005003

select * from t_item_master
