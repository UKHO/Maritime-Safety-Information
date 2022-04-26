using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace UKHO.MaritimeSafetyInformation.Common
{
    public interface IAuthFssTokenProvider
    {
        public Task<AuthenticationResult> GetAuthTokenAsync();
    }
}
