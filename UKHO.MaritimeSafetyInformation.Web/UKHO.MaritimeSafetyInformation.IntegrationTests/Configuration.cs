using Microsoft.Extensions.Configuration;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests
{
    public class Configuration
    {
        public string BusinessUnit { get; }
        public string ProductType { get; }
        public string FssClientId { get; }
        public int MaxAttributeValuesCount { get; }
        public int PageSize { get; }
        public int Start { get; }

        public Configuration()
        {
            var configurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            BusinessUnit = configurationRoot.GetValue<string>("FileShareService:BusinessUnit");
            ProductType = configurationRoot.GetValue<string>("FileShareService:ProductType");
            FssClientId = configurationRoot.GetValue<string>("FileShareService:FssClientId");
            MaxAttributeValuesCount = configurationRoot.GetValue<int>("FileShareService:MaxAttributeValuesCount");
            PageSize = configurationRoot.GetValue<int>("FileShareService:PageSize");
            Start = configurationRoot.GetValue<int>("FileShareService:Start");
        }
    }
}
