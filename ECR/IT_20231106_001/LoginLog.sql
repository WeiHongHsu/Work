USE [master]
GO

/****** Object:  Table [dbo].[LoginLog]    Script Date: 11/20/2023 11:13:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoginLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EventTime] [datetime2](7) NOT NULL,
	[IPAddress] [varchar](50) NOT NULL,
	[LoginName] [nvarchar](128) NOT NULL,
	[HostName] [nvarchar](128) NOT NULL,
	[ProgramName] [nvarchar](128) NOT NULL,
	[DatabaseName] [nvarchar](128) NOT NULL,
	[LoginType] [nvarchar](128) NOT NULL,
	[LoginFlag] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


