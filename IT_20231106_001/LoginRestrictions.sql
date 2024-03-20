USE [master]
GO

/****** Object:  DdlTrigger [LoginRestrictions]    Script Date: 11/20/2023 10:33:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/**EVENTDATA()**/

--<EVENT_INSTANCE>
--  <EventType>LOGON</EventType>
--  <PostTime>2022-05-13T12:33:59.773</PostTime>
--  <SPID>53</SPID>
--  <ServerName>JERRY\JIGNESH</ServerName>
--  <LoginName>sa</LoginName>
--  <LoginType>Windows (NT)Login</LoginType>
--  <SID>AQ==</SID>
--  <ClientHost>140.123.1.1</ClientHost>
--  <IsPooled>0</IsPooled>
--</EVENT_INSTANCE>


alter TRIGGER [LoginRestrictions]
ON ALL SERVER 
FOR LOGON
AS
    BEGIN
        
		DECLARE @data xml
		DECLARE @ClientHost varchar(50)
		DECLARE @LoginName varchar(50)
		DECLARE @LoginType varchar(50)
		DECLARE @PostTime datetime


		SET @data = EVENTDATA()
		SET @ClientHost = @data.value('(/EVENT_INSTANCE/ClientHost)[1]', 'varchar(50)')
		SET @LoginName = @data.value('(/EVENT_INSTANCE/LoginName)[1]', 'varchar(50)')
		SET @LoginType = @data.value('(/EVENT_INSTANCE/LoginType)[1]', 'varchar(50)')
		SET @PostTime = @data.value('(/EVENT_INSTANCE/PostTime)[1]', 'datetime')

		DECLARE @HostName NVARCHAR(128);
		DECLARE @ProgramName NVARCHAR(128);
		DECLARE @DatabaseName NVARCHAR(128);
		SET @HostName = HOST_NAME();
		SET @ProgramName = APP_NAME();
		SET @DatabaseName = DB_NAME();

	  --白名單
      IF @LoginName in (select distinct loginname FROM [master].[dbo].[Whitelist] (nolocK))
	  and @ClientHost not in (select [IP] FROM [master].[dbo].[Whitelist] (nolocK) where loginname = @LoginName and flag = 'Y' )
		begin 
			ROLLBACK; -- 不允許登入
			--登入LOG
			INSERT INTO [master].[dbo].LoginLog (EventTime, IPAddress, LoginName, HostName, ProgramName, DatabaseName, LoginType,LoginFlag)
			VALUES (@PostTime, @ClientHost, @LoginName, @HostName, @ProgramName, @DatabaseName,@LoginType,'False');
		end
	else
		begin
			--登入LOG
			INSERT INTO [master].[dbo].LoginLog (EventTime, IPAddress, LoginName, HostName, ProgramName, DatabaseName, LoginType,LoginFlag)
			VALUES (@PostTime, @ClientHost, @LoginName, @HostName, @ProgramName, @DatabaseName,@LoginType,'True');
		end
	
    END

GO

ENABLE TRIGGER [LoginRestrictions] ON ALL SERVER --啟用
--DISABLE TRIGGER [LoginRestrictions] ON ALL SERVER --停用


GO


