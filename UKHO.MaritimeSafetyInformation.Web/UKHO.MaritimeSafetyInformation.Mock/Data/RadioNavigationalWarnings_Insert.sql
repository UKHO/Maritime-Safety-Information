
INSERT INTO [dbo].[RadioNavigationalWarnings]
           ([WarningType]
           ,[Reference]
           ,[DateTimeGroup]
           ,[Summary]
           ,[Content]
           ,[IsDeleted]
           ,[LastModified])
     VALUES
           (1
           ,'NAVAREA I 240/24'
           ,GETDATE() - 2
           ,'SPACE WEATHER. SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.'
           ,'NAVAREA 1
			NAVAREA I 240/24
			301040 UTC Dec 24
			SPACE WEATHER.
			SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.
			RADIO AND SATELLITE NAVIGATION SERVICES MAY BE AFFECTED.'
           ,0
           ,GETDATE())
GO


INSERT INTO [dbo].[RadioNavigationalWarnings]
           ([WarningType]
           ,[Reference]
           ,[DateTimeGroup]
           ,[Summary]
           ,[Content]
           ,[IsDeleted]
           ,[LastModified])
     VALUES
           (2
           ,'WZ 897/24'
           ,GETDATE() - 2
           ,'HUMBER. HORNSEA 1 AND 2 WINDFARMS. TURBINE FOG SIGNALS INOPERATIVE.'
           ,'UK Coastal
WZ 897/24
301510 UTC Dec 24
HUMBER.
HORNSEA 1 AND 2 WINDFARMS.
1. TURBINES T25 54-00.3N 001-36.7E, A16 53-50.0N 001-58.7E AND S16 53-59.4N 001-48.3E, FOG SIGNALS INOPERATIVE.
2. CANCEL WZ 895.'
           ,0
           ,GETDATE())
GO