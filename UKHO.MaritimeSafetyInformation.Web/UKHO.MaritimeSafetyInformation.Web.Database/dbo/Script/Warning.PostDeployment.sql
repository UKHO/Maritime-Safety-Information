/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET IDENTITY_INSERT [dbo].[WarningType] ON;
MERGE INTO [dbo].[WarningType] AS Target
USING (
VALUES
(1,'NAVAREA 1')
, (2, 'UK Coastal')



) AS Source ([Id], [Name])
ON Target.[Id] = Source.[Id]
WHEN MATCHED THEN
UPDATE SET
Target.[Name] = Source.[Name]



WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (Source.[Id], Source.[Name])



WHEN NOT MATCHED BY SOURCE THEN
DELETE;
SET IDENTITY_INSERT [dbo].[WarningType] OFF;



GO
