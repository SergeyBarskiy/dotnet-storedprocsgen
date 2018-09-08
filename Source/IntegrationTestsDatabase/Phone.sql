IF NOT EXISTS(SELECT * FROM Phone)
BEGIN


	INSERT INTO [dbo].[Phone]
			   ([PersonId]
			   ,[PhoneNumber])
	VALUES
			   ((SELECT TOP 1 Person.PersonId FROM Person)
			   ,'111-222-3333')


	INSERT INTO [dbo].[Phone]
			   ([PersonId]
			   ,[PhoneNumber])
	VALUES
			   ((SELECT TOP 1 Person.PersonId FROM Person)
			   ,'444-555-6666')

END