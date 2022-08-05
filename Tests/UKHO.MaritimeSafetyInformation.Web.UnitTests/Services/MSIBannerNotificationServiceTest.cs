using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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

        private MSIBannerNotificationService _mSIBannerNotificationService;

        [SetUp]
        public void Setup()
        {
            _fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
            _fakeAzureStorageService = A.Fake<IAzureStorageService>();
            _fakeAzureTableStorageClient = A.Fake<IAzureTableStorageClient>();
            _fakeBannerNotificationConfiguration = A.Fake<IOptions<BannerNotificationConfiguration>>();
            _fakeBannerNotificationConfiguration.Value.IsBannerNotificationEnabled = true;

            _mSIBannerNotificationService = new MSIBannerNotificationService(_fakeCacheConfiguration, _fakeAzureStorageService, _fakeAzureTableStorageClient, _fakeBannerNotificationConfiguration);
        }

        [Test]
        public void WhenIsBannerNotificationDisabled_ThenNullValueForBannerNotificationMessageIsAssigned()
        {
            Global.BannerNotificationMessage = null;
            _fakeBannerNotificationConfiguration.Value.IsBannerNotificationEnabled = false;

            Task result = _mSIBannerNotificationService.GetBannerNotification();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsNull(Global.BannerNotificationMessage);
        }

        [Test]
        public void WhenBannerNotificationNotFound_ThenNullValueForBannerNotificationMessageIsAssigned()
        {
            Global.BannerNotificationMessage = null;

            A.CallTo(() => _fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            Task result = _mSIBannerNotificationService.GetBannerNotification();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsNull(Global.BannerNotificationMessage);
        }

        [Test]
        public void WhenBannerNotificationFound_ThenValueForBannerNotificationMessageIsAssigned()
        {
            Global.BannerNotificationMessage = null;

            A.CallTo(() => _fakeAzureTableStorageClient.GetSingleEntityAsync(A<string>.Ignored, A<string>.Ignored)).Returns( new MsiBannerNotificationEntity() { Message ="test"});

            Task result =  _mSIBannerNotificationService.GetBannerNotification();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsNotNull(Global.BannerNotificationMessage);
        }
    }
}
