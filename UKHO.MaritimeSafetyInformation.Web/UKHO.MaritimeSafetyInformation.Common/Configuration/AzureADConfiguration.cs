using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{

    public class AzureADConfiguration
    {
        public string MicrosoftOnlineLoginUrl { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
    }
}



