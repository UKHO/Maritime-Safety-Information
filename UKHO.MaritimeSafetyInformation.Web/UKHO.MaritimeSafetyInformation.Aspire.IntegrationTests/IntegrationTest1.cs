using System.Text;
using System.Text.Json;
using Aspire.Hosting;
using Azure;
using Azure.Identity;
using Google.Protobuf.WellKnownTypes;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Transaction;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Aspire.IntegrationTests
{
    public class IntegrationTest1
    {
        // Instructions:
        // 1. Add a project reference to the target AppHost project, e.g.:
        //
        //    <ItemGroup>
        //        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
        //    </ItemGroup>
        //
        // 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' to match your AppHost project:
        //
        private HttpClient? _httpClient;
        private DistributedApplication _app;

        private FormUrlEncodedContent GetWeeklyFormData(int year, int week)
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Year", year.ToString()),
                new KeyValuePair<string, string>("Week", week.ToString()),
            });
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UKHO_MaritimeSafetyInformation_Web_AppHost>();
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });
            _app = await appHost.BuildAsync();
            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync();

            _httpClient = _app.CreateHttpClient("ukho-msi-web");
            await resourceNotificationService.WaitForResourceAsync("ukho-msi-web", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));

        }


        [OneTimeTearDown]
        public async Task Teardown()
        {
            _httpClient?.Dispose();
            await _app.DisposeAsync();
        }


        [Test]
        public async Task Initial_Load_of_NoticeToMariners_Weekly_Returns_200()
        {
            var response = _httpClient!.GetAsync("/NoticesToMariners/Weekly").Result;
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var result = await response.Content.ReadAsStringAsync();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task A_ValidCall_To_NoticeToMariners_Weekly_Returns_200()
        {

            var response = _httpClient!.PostAsync("/NoticesToMariners/Weekly", GetWeeklyFormData(year: 2024, week: 4)).Result;
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var result = await response.Content.ReadAsStringAsync();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task A_Call_To_NoticeToMariners_Daily_Returns_200()
        {
            var response = _httpClient!.GetAsync("/NoticesToMariners/Daily").Result;
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var result = await response.Content.ReadAsStringAsync();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task A_Call_To_NoticeToMariners_Cumulative_Returns_200()
        {
            var response = _httpClient!.GetAsync("/NoticesToMariners/Cumulative").Result;
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var result = await response.Content.ReadAsStringAsync();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task A_Call_To_NoticeToMariners_Annual_Returns_200()
        {
            var response = _httpClient!.GetAsync("/NoticesToMariners/Annual").Result;
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var result = await response.Content.ReadAsStringAsync();
            Assert.That(result, Is.Not.Null);
        }

        //todo
        //var response3 = _httpClient!.PostAsync("/NoticesToMariners/Weekly", GetWeeklyFormData(year: 2024, week: 5)).Result;
        //var result = await response.Content.ReadAsStringAsync();
        //var result1 = await response1.Content.ReadAsStringAsync();
    }
}
