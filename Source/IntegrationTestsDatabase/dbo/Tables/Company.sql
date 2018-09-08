CREATE TABLE [dbo].[Company]
(
	[CompanyId] CHAR(36) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Income] MONEY NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [City] NVARCHAR(50) NULL, 
    [State] NVARCHAR(50) NULL, 
    [RowVersion] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Company] PRIMARY KEY NONCLUSTERED ([CompanyId]) 
)
