CREATE TABLE [dbo].[RadioNavigationalWarnings] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [WarningType]     VARCHAR(32) NULL,
    [Reference] VARCHAR(32) NULL,
    [DateTimeGroup]   DATETIME NULL,
    [Description] VARCHAR(256) NULL, 
    [Text] VARCHAR(MAX) NULL, 
    [ExpiryDate] DATETIME NULL, 
    [ApprovalStatus] BIT NULL, 
    [IsDeleted] BIT NULL, 
    CONSTRAINT [PK_RadioNavigationalWarnings] PRIMARY KEY CLUSTERED ([Id] ASC)
);
