
namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public interface IAuthFssTokenProvider
    {
        Task<string> GenerateADAccessToken(string correlationId);
    }
}
