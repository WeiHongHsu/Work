/*
SJOB 的部署指令碼

這段程式碼由工具產生。
變更這個檔案可能導致不正確的行為，而且如果重新產生程式碼，
變更將會遺失。
*/

GO
SET ANSI_S, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT__YIELDS_, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "SJOB"
:setvar DefaultFilePrefix "SJOB"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
偵測 SQLCMD 模式，如果不支援 SQLCMD 模式，則停用指令碼執行。
若要在啟用 SQLCMD 模式後重新啟用指令碼，請執行以下:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'必須啟用 SQLCMD 模式才能成功執行此指令碼。';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'正在建立 [dbo].[EDI_Decrypt]...';


GO
CREATE FUNCTION [dbo].[EDI_Decrypt]
(@pToDecrypt NVARCHAR (MAX) , @sKey NVARCHAR (MAX) )
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [infXSD].[infXSD.HtmHelp].[EDI_Decrypt]


GO
PRINT N'正在建立 [dbo].[EDI_Encrypt]...';


GO
CREATE FUNCTION [dbo].[EDI_Encrypt]
(@pToEncrypt NVARCHAR (MAX) , @sKey NVARCHAR (MAX) )
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [infXSD].[infXSD.HtmHelp].[EDI_Encrypt]


GO
PRINT N'正在建立 [dbo].[XsdBulkCopy]...';


GO
CREATE FUNCTION [dbo].[XsdBulkCopy]
(@infkey NVARCHAR (MAX) , @iwkey NVARCHAR (MAX) , @Stkeys NVARCHAR (MAX) , @InfXSD NVARCHAR (MAX) )
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [infXSD].[infXSD.infXsdFun].[XsdBulkCopy]


GO
PRINT N'正在建立 [dbo].[XsdQueryExp]...';


GO
CREATE FUNCTION [dbo].[XsdQueryExp]
(@ikey NVARCHAR (MAX) , @wkey NVARCHAR (MAX) , @Extkeys NVARCHAR (MAX) , @InfXSD NVARCHAR (MAX) )
RETURNS NVARCHAR (MAX)
AS
 EXTERNAL NAME [infXSD].[infXSD.infXsdFun].[XsdQueryExp]


GO
PRINT N'正在建立 [dbo].[EDIinfxSDView]...';


GO
CREATE PROCEDURE [dbo].[EDIinfxSDView]
@ikey NVARCHAR (MAX) , @iwkey NVARCHAR (MAX) , @iStkey NVARCHAR (MAX) , @InfXSD NVARCHAR (MAX) 
AS EXTERNAL NAME [infXSD].[infXSD.infXsdFun].[EDIinfxSDView]


GO
PRINT N'更新完成。';


GO
