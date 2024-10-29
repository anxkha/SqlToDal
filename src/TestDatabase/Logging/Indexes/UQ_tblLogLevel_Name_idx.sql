CREATE UNIQUE NONCLUSTERED INDEX [UQ_tblLogLevel_Name_idx]
ON [Logging].[tblLogLevel] ([Name])
INCLUDE ([LogLevelID]);
