IF NOT EXISTS(SELECT * FROM Person)
BEGIN
	INSERT INTO [dbo].[Person]
           ([FirstName]
           ,[LastName]
           ,[BirthDate])
     VALUES
           ('John'
           ,'Doe'
           ,null)

	INSERT INTO [dbo].[Person]
           ([FirstName]
           ,[LastName]
           ,[BirthDate])
     VALUES
           ('Jane'
           ,'Johnson'
           ,'2002-1-2')

END
