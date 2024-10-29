CREATE UNIQUE NONCLUSTERED INDEX [UQ_tblEmployee_ObjectID_idx]
ON [dbo].[tblEmployee] ([ObjectID])
INCLUDE ([EmployeeID]);
