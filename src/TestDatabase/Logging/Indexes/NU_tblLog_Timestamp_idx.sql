CREATE NONCLUSTERED INDEX [NU_tblLog_Timestamp_idx]
ON [Logging].[tblLog] ([Timestamp] DESC)
INCLUDE
(
	[LogID]
	,[LogLevelID]
	,[Message]
);
