﻿CREATE TYPE [Logging].[LogType] AS TABLE
(
	[LogLevelName] NVARCHAR(64) NOT NULL,
	[Message] NVARCHAR(MAX) NOT NULL,
	[Timestamp] DATETIMEOFFSET NOT NULL
);
