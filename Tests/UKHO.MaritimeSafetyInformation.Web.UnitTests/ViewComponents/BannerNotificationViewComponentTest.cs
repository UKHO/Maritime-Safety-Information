using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
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
        private BannerNotificationViewComponent _viewComponent;

        [SetUp]
        public void Setup()
        {
            _fakeMSIBannerNotificationService = A.Fake<IMSIBannerNotificationService>();

            _viewComponent = new BannerNotificationViewComponent(_fakeMSIBannerNotificationService);
        }

        [Test]
        public async Task WhenICallIndexView_ThenReturnView()
        {
            A.CallTo(() => _fakeMSIBannerNotificationService.GetBannerNotification()).Returns("test");

            IViewComponentResult result = await _viewComponent.InvokeAsync();

            Assert.IsInstanceOf<ViewViewComponentResult>(result);
        }
    }
}
