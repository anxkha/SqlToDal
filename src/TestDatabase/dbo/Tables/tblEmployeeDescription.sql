CREATE TABLE [dbo].[tblEmployeeDescription]
(
	[EmployeeDescriptionID] INT NOT NULL IDENTITY(1, 1),
	[EmployeeID] INT NOT NULL,
	[FirstName] NVARCHAR(64) NOT NULL,
	[LastName] NVARCHAR(64) NOT NULL,
	[CreatedTimestamp] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_tblEmployeeDescription_CreatedTimestamp] DEFAULT SYSUTCDATETIME(),
	[CreatedByUserID] INT NOT NULL

	CONSTRAINT [PK_tblEmployeeDescription_EmployeeDescriptionID] PRIMARY KEY CLUSTERED ([EmployeeDescriptionID])

	CONSTRAINT [FK_tblEmployeeDescription_EmployeeID_tblEmployee_EmployeeID]
		FOREIGN KEY ([EmployeeID])
		REFERENCES [dbo].[tblEmployee]([EmployeeID])
	CONSTRAINT [FK_tblEmployeeDescription_CreatedByUserID_tblUser_UserID]
		FOREIGN KEY ([CreatedByUserID])
		REFERENCES [dbo].[tblUser]([UserID])
);
