using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AzureADConfiguration
    {
        public string Instance { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string CallbackPath { get; set; }

        public string RedirectBaseUrl { get; set; }
    }
}
