using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.ViewComponents;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.ViewComponents
{
    [TestFixture]
    public class BannerNotificationViewComponentTest
    {
        private IMSIBannerNotificationService _fakeMSIBannerNotificationService;
        private ILogger<BannerNotificationViewComponent> _fakeLogger;
        private IHttpContextAccessor _fakeContextAccessor;
        private BannerNotificationViewComponent _viewComponent;

        [SetUp]
        public void Setup()
        {
            _fakeMSIBannerNotificationService = A.Fake<IMSIBannerNotificationService>();
            _fakeLogger = A.Fake<ILogger<BannerNotificationViewComponent>>();
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();

            _viewComponent = new BannerNotificationViewComponent(_fakeContextAccessor, _fakeLogger, _fakeMSIBannerNotificationService);
        }

        [Test]
        public async Task WhenICallIndexView_ThenReturnView()
        {
            A.CallTo(() => _fakeMSIBannerNotificationService.GetBannerNotification(A<string>.Ignored)).Returns("test");

            IViewComponentResult result = await _viewComponent.InvokeAsync();

            Assert.IsInstanceOf<ViewViewComponentResult>(result);
        }
    }
}
