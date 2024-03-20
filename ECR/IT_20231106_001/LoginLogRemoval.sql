
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jacky
-- Create date: 20231129
-- Description:	刪除設備登入日誌紀錄
-- 刪除範圍： EDL_WES、CHUTEPC、ECaps1~4、ECaps7~8
-- TEST：exec LoginLogRemoval
-- =============================================
alter PROCEDURE LoginLogRemoval

AS
BEGIN
	SET NOCOUNT ON;


	DECLARE  @link table ( ID Int ,eCapsLink nvarchar(50))
	insert into @link (ID,eCapsLink) Values(1,'[EDL_WES]')
	insert into @link (ID,eCapsLink) Values(2,'[ECaps1]')
	insert into @link (ID,eCapsLink) Values(3,'[ECaps2]')
	insert into @link (ID,eCapsLink) Values(4,'[ECaps3]')
	insert into @link (ID,eCapsLink) Values(5,'[ECaps4]')
	insert into @link (ID,eCapsLink) Values(6,'[ECaps7]')
	insert into @link (ID,eCapsLink) Values(7,'[ECaps8]')
	insert into @link (ID,eCapsLink) Values(8,'[CHUTEPC]')


	DECLARE @sql nvarchar(2000),
			@eCapsLink nvarchar(50),
			@id int

	While (1=1)
		begin
			if not EXISTS (select top 1 * from @link)
			begin 
				break
			end

			select top 1  @eCapsLink = eCapsLink, @id = id from @link

			/*清除LoginLog*/
			set @sql = 'if EXISTS(select top 1 * from '+@eCapsLink+'.[master].[dbo].LoginLog with (nolocK) where convert(char(8),EventTime,112) < convert(char(8),getdate()-7,112)) '
					 + ' begin '
					 + ' delete '+@eCapsLink+'.[master].[dbo].LoginLog where convert(char(8),EventTime,112) < convert(char(8),getdate()-7,112) '
					 + ' end '
					 + ' else '
					 + ' begin '
					 + ' Print (N'''+@eCapsLink+'無資料需刪除'') ' 
					 + ' end '
			--Print @sql
			exec sp_executesql @sql

			delete @link where id = @id
	end

END
GO
