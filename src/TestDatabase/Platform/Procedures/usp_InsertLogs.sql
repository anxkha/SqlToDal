CREATE PROCEDURE [Platform].[usp_InsertLogs]
	@Logs [Platform].[LogType] READONLY
WITH EXECUTE AS OWNER
AS

SET NOCOUNT ON;

INSERT INTO [Platform].[tblLog]
(
	[ObjectID]
	,[Message]
)
SELECT
	[ObjectID]
	,[Message]
FROM @Logs;

RETURN 0;
