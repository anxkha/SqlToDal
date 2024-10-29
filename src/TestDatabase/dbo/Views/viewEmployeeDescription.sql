CREATE VIEW [dbo].[viewEmployeeDescription]
AS

WITH Descriptions AS (
	SELECT
		[EmployeeID]
		,[FirstName]
		,[LastName]
		,[CreatedTimestamp]
		,[CreatedByUserID]
		,ROW_NUMBER() OVER (PARTITION BY [EmployeeID] ORDER BY [CreatedTimestamp] DESC, [EmployeeID] DESC) [RowNumber]
	FROM [dbo].[tblEmployeeDescription]
)

SELECT
	[EmployeeID]
	,[FirstName]
	,[LastName]
	,[CreatedTimestamp]
	,[CreatedByUserID]
FROM Descriptions
WHERE [RowNumber] = 1;
