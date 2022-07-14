using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AzureAdB2C
    {
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Domain { get; set; }
        public string RedirectBaseUrl { get; set; }
        public string Scope { get; set; }
    }
}
