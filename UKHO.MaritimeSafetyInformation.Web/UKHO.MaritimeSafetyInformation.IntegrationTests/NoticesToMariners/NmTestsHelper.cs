using System;
using System.Configuration;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Filters;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    public class NmTestsHelper
    {
        public ILogger<NoticesToMarinersController> _fakeLogger;
        public ILogger<NMDataService> _fakeLoggerNMDataService;
        public IHttpContextAccessor _fakeHttpContextAccessor;
        public IFileShareService _fakeFileShareService;
        public IAuthFssTokenProvider _fakeAuthFssTokenProvider;

        private Configuration Config { get; set; }

        public NmTestsHelper()
        {
            Config = new Configuration();
            /////// var serviceProvider = new ServiceCollection()
            ////// .BuildServiceProvider();

            //////////_fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            //////////_fakeLogger = A.Fake<ILogger<NoticesToMarinersController>>();
            //////////_fakeLoggerNMDataService = A.Fake<ILogger<NMDataService>>();
            //////////_fakeFileShareService = A.Fake<IFileShareService>();
            //////////_fakeAuthFssTokenProvider = A.Fake<IAuthFssTokenProvider>();          
        }


        protected static IServiceCollection BuildDefaultServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            ////////////serviceCollection.AddSingleton(A.Fake<ILogger<NMDataService>>());
            ////////////serviceCollection.AddSingleton(A.Fake<ILogger<FileShareService>>());
            ////////////serviceCollection.AddScoped(A.Fake<ILogger<AuthFssTokenProvider>>());

            serviceCollection.AddScoped<INMDataService, NMDataService>();
            serviceCollection.AddScoped<IFileShareService, FileShareService>();
            serviceCollection.AddScoped<IAuthFssTokenProvider, AuthFssTokenProvider>();
            serviceCollection.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddHeaderPropagation(options =>
            {
                options.Headers.Add(CorrelationIdMiddleware.XCorrelationIdHeaderKey);
            });
            serviceCollection.AddHttpClient();
            return serviceCollection;
        }
    }
}
