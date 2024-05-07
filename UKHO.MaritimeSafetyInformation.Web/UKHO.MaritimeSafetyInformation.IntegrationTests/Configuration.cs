using Microsoft.Extensions.Configuration;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests
{
    public class Configuration
    {
        protected IConfigurationRoot ConfigurationRoot;

        public string BusinessUnit { get; set; }
        public string ProductType { get; set; }
        public string MockBaseUrl { get; set; }
        public string FssClientId { get; set; }
        public int MaxAttributeValuesCount { get; set; }
        public int PageSize { get; set; }
        public int Start { get; set; }

        public Configuration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            BusinessUnit = ConfigurationRoot.GetValue<string>("FileShareService:BusinessUnit");
            ProductType = ConfigurationRoot.GetValue<string>("FileShareService:ProductType");
            MockBaseUrl = ConfigurationRoot.GetValue<string>("FileShareService:MockBaseUrl");
            FssClientId = ConfigurationRoot.GetValue<string>("FileShareService:FssClientId");
            MaxAttributeValuesCount = ConfigurationRoot.GetValue<int>("FileShareService:MaxAttributeValuesCount");
            PageSize = ConfigurationRoot.GetValue<int>("FileShareService:PageSize");
            Start = ConfigurationRoot.GetValue<int>("FileShareService:Start");
        }
    }
}
