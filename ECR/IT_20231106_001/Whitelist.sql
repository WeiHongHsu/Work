USE [master]
GO

/****** Object:  Table [dbo].[Whitelist]    Script Date: 11/20/2023 11:13:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Whitelist](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LoginName] [nvarchar](50) NOT NULL,
	[Hostname] [nvarchar](50) NOT NULL,
	[IP] [nvarchar](50) NOT NULL,
	[Group] [nvarchar](50) NOT NULL,
	[Flag] [nvarchar](50) NOT NULL,
	[Note] [nvarchar](2000) NULL,
	[Adddate] [datetime] NOT NULL,
	[AddWho] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_whitelist] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_LoginName]  DEFAULT ('') FOR [LoginName]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_Hostname]  DEFAULT ('') FOR [Hostname]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_IP]  DEFAULT ('') FOR [IP]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_Group]  DEFAULT ('') FOR [Group]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_Flag]  DEFAULT ('Y') FOR [Flag]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_Note]  DEFAULT ('') FOR [Note]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_Adddate]  DEFAULT (getdate()) FOR [Adddate]
GO

ALTER TABLE [dbo].[whitelist] ADD  CONSTRAINT [DF_whitelist_AddWho]  DEFAULT (suser_sname(suser_sid())) FOR [AddWho]
GO


