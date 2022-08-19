using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class MSIBannerNotificationServiceTest
    {
        private IOptions<CacheConfiguration> _fakeCacheConfiguration;
        private IAzureStorageService _fakeAzureStorageService;
        private IAzureTableStorageClient _fakeAzureTableStorageClient;
        private IOptions<BannerNotificationConfiguration> _fakeBannerNotificationConfiguration;
        private ILogger<MSIBannerNotificationService> _fakeLogger;

        private MSIBannerNotificationService _mSIBannerNotificationService;

        [SetUp]
        public void Setup()
        {
            _fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
            _fakeAzureStorageService = A.Fake<IAzureStorageService>();
            _fakeAzureTableStorageClient = A.Fake<IAzureTableStorageClient>();
            _fakeBannerNotificationConfiguration = A.Fake<IOptions<BannerNotificationConfiguration>>();
            _fakeLogger = A.Fake<ILogger<MSIBannerNotificationService>>();

            _fakeBannerNotificationConfiguration.Value.IsBannerNotificationEnabled = true;

            _mSIBannerNotificationService = new MSIBannerNotificationService(_fakeCacheConfiguration, _fakeAzureStorageService, _fakeAzureTableStorageClient, _fakeBannerNotificationConfiguration, _fakeLogger);
        }

        [Test]
        public async Task WhenIsBannerNotificationDisabled_ThenNullValueForBannerNotificationMessageIsAssigned()
        {
            _fakeBannerNotificationConfiguration.Value.IsBannerNotificationEnabled = false;

            string result = await _mSIBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.IsNull(result);
        }

        [Test]
        public async Task WhenExceptionThrownByService_ThenNullValueForBannerNotificationMessageIsAssigned()
        {
            A.CallTo(() => _fakeAzureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Throws(new Exception());

            string result = await _mSIBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.IsNull(result);
        }

        [Test]
        public async Task WhenBannerNotificationNotFound_ThenEmptyValueForBannerNotificationMessageIsAssigned()
        {
            A.CallTo(() => _fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            string result = await _mSIBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task WhenBannerNotificationFoundWithXSS_ThenValueExcludingXSSForBannerNotificationMessageIsAssigned()
        {
            string message = @"This system will be under maintenance. For more details <a href=""https://www.test.com"" target=""_blank"">click here<script>alert('XSS attack!')</script>";
            string expectedMessage = @"This system will be under maintenance. For more details <a href=""https://www.test.com"" target=""_blank"">click here</a>";

            A.CallTo(() => _fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).Returns(new MsiBannerNotificationEntity() { Message = message });

            string result = await _mSIBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.AreEqual(expectedMessage, result);
        }

        [Test]
        public async Task WhenBannerNotificationFound_ThenValueForBannerNotificationMessageIsAssigned()
        {
            A.CallTo(() => _fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).Returns( new MsiBannerNotificationEntity() { Message ="test"});

            string result =  await _mSIBannerNotificationService.GetBannerNotification(string.Empty);

            Assert.AreEqual("test", result);
        }
    }
}
