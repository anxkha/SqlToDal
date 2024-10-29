CREATE PROCEDURE [Logging].[usp_InsertLogs]
	@Logs [Logging].[LogType] READONLY
WITH EXECUTE AS OWNER
AS

SET NOCOUNT ON;

INSERT INTO [Logging].[tblLog]
(
	[LogLevelID]
	,[Message]
	,[Timestamp]
)
SELECT
	ll.[LogLevelID]
	,[Message]
	,[Timestamp]
FROM @Logs l
	INNER JOIN [Logging].[tblLogLevel] ll ON l.[LogLevelID] = ll.[LogLevelID]
WHERE ll.[Name] = l.[LogLevelName];

RETURN 0;
