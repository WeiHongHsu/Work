use vendor_agv
DECLARE @ok int
EXECUTE [dbo].[CLR_TEST]  @ok OUTPUT
select @ok