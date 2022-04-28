
namespace UKHO.MaritimeSafetyInformation.Common
{
    public interface IAuthFssTokenProvider
    {
        public Task<string> GenerateADAccessToken(string correlationId);
    }
}
