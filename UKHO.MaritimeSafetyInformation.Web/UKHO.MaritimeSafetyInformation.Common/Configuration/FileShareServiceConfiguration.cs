using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class FileShareServiceConfiguration
    {
        public string BaseUrl { get; set; }
        public string BusinessUnit { get; set; }
        public string ProductType { get; set; }
        public string FssClientId { get; set; }
        public int MaxAttributeValuesCount { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }        
    }
}
