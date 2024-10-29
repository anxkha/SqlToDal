CREATE VIEW [Logging].[viewLog]
AS

SELECT
	ll.[Name] AS [LogLevel]
	,l.[Message]
	,l.[Timestamp]
FROM [Logging].[tblLog] l
	INNER JOIN [Logging].[tblLogLevel] ll ON l.[LogLevelID] = ll.[LogLevelID];
