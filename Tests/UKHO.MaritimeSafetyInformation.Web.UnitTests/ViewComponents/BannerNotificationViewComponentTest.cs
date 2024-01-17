using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.ViewComponents;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.ViewComponents
{
    [TestFixture]
    public class BannerNotificationViewComponentTest
    {
        private IMSIBannerNotificationService fakeMSIBannerNotificationService;
        private ILogger<BannerNotificationViewComponent> fakeLogger;
        private IHttpContextAccessor fakeContextAccessor;
        private BannerNotificationViewComponent viewComponent;

        [SetUp]
        public void Setup()
        {
            fakeMSIBannerNotificationService = A.Fake<IMSIBannerNotificationService>();
            fakeLogger = A.Fake<ILogger<BannerNotificationViewComponent>>();
            fakeContextAccessor = A.Fake<IHttpContextAccessor>();

            viewComponent = new BannerNotificationViewComponent(fakeContextAccessor, fakeLogger, fakeMSIBannerNotificationService);
        }

        [Test]
        public async Task WhenICallIndexView_ThenReturnView()
        {
            A.CallTo(() => fakeMSIBannerNotificationService.GetBannerNotification(A<string>.Ignored)).Returns("test");

            IViewComponentResult result = await viewComponent.InvokeAsync();

            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());
        }
    }
}
