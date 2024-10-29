CREATE PROCEDURE [Logging].[usp_SelectLogs]
WITH EXECUTE AS OWNER
AS

SET NOCOUNT ON;

SELECT
	[LogLevel]
	,[Message]
	,[Timestamp]
FROM [Logging].[viewLog]
ORDER BY [Timestamp] DESC;

RETURN 0;
