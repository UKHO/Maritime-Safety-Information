using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests
{
    public class Configuration
    {
        protected IConfigurationRoot ConfigurationRoot;

        public string BusinessUnit;
        public string ProductType;
        public string BaseUrl;
        public string PageSize;
        public string Start;

        public Configuration()
        {
            ConfigurationRoot = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            BusinessUnit = ConfigurationRoot.GetValue<string>("FileShareService:BusinessUnit");
            ProductType = ConfigurationRoot.GetValue<string>("FileShareService:ProductType");
            BaseUrl = ConfigurationRoot.GetValue<string>("FileShareService:BaseUrl");
            PageSize = ConfigurationRoot.GetValue<string>("FileShareService:PageSize");
            Start = ConfigurationRoot.GetValue<string>("FileShareService:Start");
        }
    }
}
