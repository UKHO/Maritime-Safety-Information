using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Web.Models
{

    [ExcludeFromCodeCoverage]
    public class AccessTokenItem
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
    }
}
