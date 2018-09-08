IF NOT EXISTS(SELECT * FROM Company)
BEGIN
INSERT INTO [dbo].[Company]
           ([CompanyId]
           ,[Name]
           ,[Income]
           ,[DateCreated]
           ,[City]
           ,[State]
           ,[RowVersion])
     VALUES
           ('7B634BAE-FDD9-4E9F-A62E-6E793A96418C'
           ,'Alpha'
           ,1000
           ,'2001-1-2'
           ,'Atlanta'
           ,'GA'
           ,1)

	INSERT INTO [dbo].[Company]
           ([CompanyId]
           ,[Name]
           ,[Income]
           ,[DateCreated]
           ,[City]
           ,[State]
           ,[RowVersion])
     VALUES
           ('5E31A678-C449-4E15-8C81-FEC8B020A309'
           ,'Beta'
           ,2000
           ,'2002-1-2'
           ,'Macon'
           ,'GA'
           ,2)
END
