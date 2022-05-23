using Microsoft.Extensions.Configuration;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    public class Configuration
    {       
        protected IConfigurationRoot ConfigurationRoot;

        public string BusinessUnit;
        public string ProductType;
        public string BaseUrl;

        public Configuration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            BusinessUnit = ConfigurationRoot.GetValue<string>("FileShareService:BusinessUnit");
            ProductType = ConfigurationRoot.GetValue<string>("FileShareService:ProductType");
            BaseUrl = ConfigurationRoot.GetValue<string>("FileShareService:BaseUrl");
        }
    }
}
