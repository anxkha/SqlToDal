CREATE TABLE [Logging].[tblLog]
(
	[LogID] INT NOT NULL IDENTITY(1, 1),
	[LogLevelID] INT NOT NULL,
	[Message] NVARCHAR(MAX) NOT NULL,
	[Timestamp] DATETIMEOFFSET NOT NULL

	CONSTRAINT [PK_tblLog_LogID] PRIMARY KEY CLUSTERED ([LogID])

	CONSTRAINT [FK_tblLog_LogLevelID_tblLogLevel_LogLevelID]
		FOREIGN KEY ([LogLevelID])
		REFERENCES [Logging].[tblLogLevel]([LogLevelID])
);
