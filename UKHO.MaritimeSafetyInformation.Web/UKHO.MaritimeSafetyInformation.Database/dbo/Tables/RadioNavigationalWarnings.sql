CREATE TABLE [dbo].[RadioNavigationalWarnings] (
    [id]             INT           NOT NULL,
    [WarningType]    VARCHAR (32)  NULL,
    [Reference]      VARCHAR (32)  NULL,
    [DateTimeGroup]  DATETIME      NULL,
    [Description]    VARCHAR (256) NULL,
    [Text]           VARCHAR (MAX) NULL,
    [ExpiryDate]     DATETIME      NULL,
    [Approvalstatus] BIT           NULL,
    [IsDeleted]      BIT           NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

