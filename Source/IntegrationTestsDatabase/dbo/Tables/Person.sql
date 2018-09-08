CREATE TABLE [dbo].[Person]
(
	[PersonId] INT NOT NULL IDENTITY(1,1), 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [BirthDate] DATE NULL, 
    [RowVersion] TIMESTAMP NOT NULL, 
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([PersonId]),
	
)
