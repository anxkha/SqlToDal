CREATE PROCEDURE [Logging].[usp_InsertLog]
	@LogLevelName UNIQUEIDENTIFIER,
	@Message NVARCHAR(MAX),
	@Timestamp DATETIMEOFFSET
WITH EXECUTE AS OWNER
AS

SET NOCOUNT ON;

DECLARE @LogLevelID INT = (SELECT [LogLevelID] FROM [Logging].[tblLogLevel] WHERE [Name] = @LogLevelName);

INSERT INTO [Logging].[tblLog]
(
	[LogLevelID]
	,[Message]
	,[Timestamp]
)
VALUES
(
	@LogLevelID
	,@Message
	,@Timestamp
);

RETURN 0;
