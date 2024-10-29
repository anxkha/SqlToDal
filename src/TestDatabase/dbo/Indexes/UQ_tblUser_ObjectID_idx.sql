CREATE UNIQUE NONCLUSTERED INDEX [UQ_tblUser_ObjectID_idx]
ON [dbo].[tblUser] ([ObjectID])
INCLUDE ([UserID]);
