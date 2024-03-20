Declare @outputPostalCode as NVARCHAR(200)
Declare @apiStatus as INT
Declare @apiMessage as NVARCHAR(500)
Exec [INT].iexp.dbo.SP_GetPostalCode_TCAT '202308098923101',@outputPostalCode output,@apiStatus output,@apiMessage output
print '@outputPostalCode' + @outputPostalCode
print '@apiStatus' + cast(@apiStatus as nvarchar(20))
print '@apiMessage ' + @apiMessage

202308099107501(錯誤地址)
202308098923101(正確地址)

SELECT * FROM [INT].[Iexp].[dbo].[EC_API_Log]

202308099107501(錯誤地址)
202308098923101(正確地址)

