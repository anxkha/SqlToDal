CREATE NONCLUSTERED INDEX [NU_tblEmployeeDescription_EmployeeID_CreatedTimestamp_idx]
ON [dbo].[tblEmployeeDescription]
(
	[EmployeeID]
	,[CreatedTimestamp] DESC
)
INCLUDE
(
	[EmployeeDescriptionID]
	,[FirstName]
	,[LastName]
	,[CreatedByUserID]
);
