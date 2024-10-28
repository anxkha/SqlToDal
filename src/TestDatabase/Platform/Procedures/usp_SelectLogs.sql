CREATE PROCEDURE [Platform].[usp_SelectLogs]
WITH EXECUTE AS OWNER
AS

SET NOCOUNT ON;

SELECT
	[ObjectID]
	,[Message]
	,[CreatedTimestamp]
FROM [Platform].[tblLog];

RETURN 0;
