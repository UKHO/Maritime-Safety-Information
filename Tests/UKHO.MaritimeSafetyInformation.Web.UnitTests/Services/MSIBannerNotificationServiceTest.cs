using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class MSIBannerNotificationServiceTest
    {
        private IOptions<CacheConfiguration> fakeCacheConfiguration;
        private IAzureStorageService fakeAzureStorageService;
        private IAzureTableStorageClient fakeAzureTableStorageClient;
        private IOptions<BannerNotificationConfiguration> fakeBannerNotificationConfiguration;
        private ILogger<MSIBannerNotificationService> fakeLogger;

        private MSIBannerNotificationService msiBannerNotificationService;

        [SetUp]
        public void Setup()
        {
            fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
            fakeAzureStorageService = A.Fake<IAzureStorageService>();
            fakeAzureTableStorageClient = A.Fake<IAzureTableStorageClient>();
            fakeBannerNotificationConfiguration = A.Fake<IOptions<BannerNotificationConfiguration>>();
            fakeLogger = A.Fake<ILogger<MSIBannerNotificationService>>();

            fakeBannerNotificationConfiguration.Value.IsBannerNotificationEnabled = true;

            msiBannerNotificationService = new MSIBannerNotificationService(fakeCacheConfiguration, fakeAzureStorageService, fakeAzureTableStorageClient, fakeBannerNotificationConfiguration, fakeLogger);
        }

        [Test]
        public async Task WhenIsBannerNotificationDisabled_ThenNullValueForBannerNotificationMessageIsAssigned()
        {
            fakeBannerNotificationConfiguration.Value.IsBannerNotificationEnabled = false;

            string result = await msiBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task WhenExceptionThrownByService_ThenNullValueForBannerNotificationMessageIsAssigned()
        {
            A.CallTo(() => fakeAzureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Throws(new Exception());

            string result = await msiBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task WhenBannerNotificationNotFound_ThenEmptyValueForBannerNotificationMessageIsAssigned()
        {
            A.CallTo(() => fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            string result = await msiBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task WhenBannerNotificationFoundWithXSS_ThenValueExcludingXSSForBannerNotificationMessageIsAssigned()
        {
            string message = @"This system will be under maintenance. For more details <a href=""https://www.test.com"" target=""_blank"">click here<script>alert('XSS attack!')</script>";
            string expectedMessage = @"This system will be under maintenance. For more details <a href=""https://www.test.com"" target=""_blank"">click here</a>";

            A.CallTo(() => fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).Returns(new MsiBannerNotificationEntity() { Message = message });

            string result = await msiBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.That(expectedMessage, Is.EqualTo(result));
        }

        [Test]
        public async Task WhenBannerNotificationFound_ThenValueForBannerNotificationMessageIsAssigned()
        {
            A.CallTo(() => fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).Returns(new MsiBannerNotificationEntity() { Message = "test" });

            string result = await msiBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.That("test", Is.EqualTo(result));
        }
    }
}
