CREATE TABLE [dbo].[RadioNavigationalWarnings] (
 [Id] INT IDENTITY (1, 1), 
 [WarningType] INT NOT NULL, 
 [Reference] VARCHAR(32) NOT NULL, 
 [DateTimeGroup] DATETIME NOT NULL, 
 [Summary] VARCHAR(256) NOT NULL, 
 [Content] VARCHAR(MAX) NOT NULL, 
 [ExpiryDate] DATETIME NULL, 
 [IsDeleted] BIT NOT NULL DEFAULT 0, 
 CONSTRAINT [PK_RadioNavigationalWarnings] PRIMARY KEY CLUSTERED ([Id] ASC) ,
 CONSTRAINT [FK_WarningType] FOREIGN KEY ([WarningType]) REFERENCES [dbo].[WarningType]([Id]) ON DELETE CASCADE );