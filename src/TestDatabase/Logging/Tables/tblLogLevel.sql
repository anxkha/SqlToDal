﻿CREATE TABLE [Logging].[tblLogLevel]
(
	[LogLevelID] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(64) NOT NULL

	CONSTRAINT [PK_tblLogLevel_LogLevelID] PRIMARY KEY CLUSTERED ([LogLevelID])
);
