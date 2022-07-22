
namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public interface IAuthFssTokenProvider
    {
        Task<string> GenerateADAccessToken(bool isDistributorUser, string correlationId);
    }
}
