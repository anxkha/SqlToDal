﻿CREATE TABLE [dbo].[tblEmployee]
(
	[EmployeeID] INT NOT NULL IDENTITY(1, 1),
	[ObjectID] UNIQUEIDENTIFIER NOT NULL

	CONSTRAINT [PK_tblEmployee_EmployeeID] PRIMARY KEY CLUSTERED ([EmployeeID])
);
