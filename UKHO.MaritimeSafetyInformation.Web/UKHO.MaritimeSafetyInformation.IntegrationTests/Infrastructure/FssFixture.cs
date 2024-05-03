using WireMock.Server;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.Infrastructure
{
    public class FssFixture
    {
        private readonly WireMockServer wireMockServer;

        public FssFixture()
        {
            wireMockServer = WireMockServer.Start(50000);
        }

        public void Stop()
        {
            wireMockServer.Stop();
        }

        public void Reset()
        {
            wireMockServer.Reset();
        }
    }
}
