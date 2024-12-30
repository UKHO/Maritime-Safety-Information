using System.Threading.Tasks;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public class MockAuthFssTokenProvider : IAuthFssTokenProvider
    {
        public Task<string> GenerateADAccessToken(bool isDistributorUser, string correlationId)
        {
            // Return a mock token for testing purposes
            return Task.FromResult("mock-token");
        }
    }
}
