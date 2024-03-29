﻿CREATE TABLE [dbo].[RadioNavigationalWarnings] (
 [Id] INT IDENTITY (1, 1), 
 [WarningType] INT NOT NULL, 
 [Reference] VARCHAR(32) NOT NULL, 
 [DateTimeGroup] DATETIME NOT NULL, 
 [Summary] VARCHAR(256) NOT NULL, 
 [Content] VARCHAR(MAX) NOT NULL, 
 [ExpiryDate] DATETIME NULL, 
 [IsDeleted] BIT NOT NULL DEFAULT 0, 
 [LastModified] DATETIME NOT NULL DEFAULT GETUTCDATE() INDEX [IDX_RadioNavigationalWarnings_LastModified] NONCLUSTERED, 
 CONSTRAINT [PK_RadioNavigationalWarnings] PRIMARY KEY CLUSTERED ([Id] ASC) ,
 CONSTRAINT [FK_WarningType] FOREIGN KEY ([WarningType]) REFERENCES [dbo].[WarningType]([Id]) ON DELETE NO ACTION );
