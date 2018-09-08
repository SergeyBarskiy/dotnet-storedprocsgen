CREATE TABLE [dbo].[Phone]
(
	[PhoneId] INT NOT NULL IDENTITY, 
	[PersonId] INT NOT NULL,
    [PhoneNumber] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_Phone] PRIMARY KEY CLUSTERED ([PhoneId]), 
    CONSTRAINT [FK_Phone_Person] FOREIGN KEY ([PersonId]) REFERENCES [Person]([PersonId]) 
)
