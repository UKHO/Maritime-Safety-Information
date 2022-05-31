using Microsoft.Extensions.Configuration;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests
{
    public class Configuration
    {
        protected IConfigurationRoot ConfigurationRoot;

        public string BusinessUnit { get; set; }
        public string ProductType { get; set; }
        public string BaseUrl { get; set; }
        public string FssClientId { get; set; }
        public int PageSize { get; set; }
        public string Start { get; set; }

        public Configuration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            BusinessUnit = ConfigurationRoot.GetValue<string>("FileShareService:BusinessUnit");
            ProductType = ConfigurationRoot.GetValue<string>("FileShareService:ProductType");
            BaseUrl = ConfigurationRoot.GetValue<string>("FileShareService:BaseUrl");
            FssClientId = ConfigurationRoot.GetValue<string>("FileShareService:FssClientId");
            PageSize = ConfigurationRoot.GetValue<int>("FileShareService:PageSize");
            Start = ConfigurationRoot.GetValue<string>("FileShareService:Start");
        }
    }
}
