# UKHO.MaritimeSafetyInformation (Aspire conversion)

A .NET 9 MVC solution orchestrated with Aspire.

## Changes required to run locally and retrieve telemetry

## 1. Introduce minimum hosting model, this involves removing startup.cs and rewriting that startup code in program.cs thus replacing IHostBuilder with WebApplicationBuilder which is needed for aspire to work properly.

## 2. Add AppHost and ServiceDefaults projects. 

### AppHost.cs defines the following resources:
   - Mock APi using ADDS.Mocks 
   - Azure TableStorage emulator for caching
   - SQL Server emulator to hold Radio Navigation Warnings database
   - MSI-Web and MSI-Admin-Web (Rnw)
   - A SeedData method, called at startup, if necessary, to populate a new database.
        
        
### The AppHost project contains two extensions that provide Aspire dashboard interactions.
   - SqlServerResourceBuilderExtensions : Reset the local database.
   - ProjectResourceBuilderExtensions : Change a flag in mockconfig.json which simulates user logins.

### Note: The SeedData method and the SqlServerResourceBuilderExtensions extension replicate the code used to populate the database.


### Updated projects
 
- UKHO.MaritimeSafetyInformation.Common
    - Configuration
        - CacheConfiguration.cs 
            + Added LocalConnectionString property which is populated when running locally
        - UkhoHeaderNames.cs (New)
            + This removed the need to have CorrelationIdMiddleware make public the XCorrelationIdHeaderKey property
    - Extensions
        - CustomLoggingExtensions.cs (New)
        - MockAuthHandler.cs (New)
        - MockAuthSelectionMiddleware.cs (New)
        - MockTokenAqusition.cs (New)
        - RequestPipelineConfigExtensions.cs (New)
    - Helpers
        - MockAuthTokenProvider.cs (New)   

- UKHO.MaritimeSafetyInformation.Web
    - Controllers
        - BaseController.cs 
            + use UkhoHeaderNames.XCorrelationId
    - Filters
        - CorrelationIdMiddleware.cs
            + use UkhoHeaderNames.XCorrelationId
    - Program.cs
        + override some configurations and use Mock Middleware and Handlers if in develop.  
    - Services
        - FileShareServiceCache.cs
            + use LocalConnectionString if assigned
        - MSIBannerNotificationService.cs 
            + use LocalConnectionString if assigned   
    - ViewComponents
        - BaseViewComponent.cs
            + use UkhoHeaderNames.XCorrelationId
    - Views/Home
        - Index.chtml 
            + add data-testid="NMs" for testing.            

- UKHO.MaritimeSafetyInformationAdmin.Web
    - Controllers
      - BaseController.cs
        + use UkhoHeaderNames.XCorrelationId
    - Program.cs
        + allow anonymous access for development purposes  




### Added projects
- UKHO.MaritimeSafetyInformation.Web.AppHost
- UKHO.MaritimeSafetyInformation.Web.ServiceDefaults
- UKHO.ADDS.Mocks
- UKHO.ADDS.Mocks.MSI
- UKHO.MaritimeSafetyInformation.PlaywrightTests
    + these replace Tests/AutoTests/FunctionalTest written in typescript.
