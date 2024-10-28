CREATE PROCEDURE [Platform].[usp_InsertLog]
	@ObjectID UNIQUEIDENTIFIER,
	@Message NVARCHAR(200)
WITH EXECUTE AS OWNER
AS

SET NOCOUNT ON;

INSERT INTO [Platform].[tblLog]
(
	[ObjectID]
	,[Message]
)
VALUES
(
	@ObjectID
	,@Message
);

RETURN 0;
