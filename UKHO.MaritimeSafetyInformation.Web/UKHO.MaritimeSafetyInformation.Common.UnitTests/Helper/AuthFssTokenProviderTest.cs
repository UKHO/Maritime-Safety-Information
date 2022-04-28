using FakeItEasy;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helper
{
    public class AuthFssTokenProviderTest
    {
        private IOptions<AzureADConfiguration> _fakeAzureADConfiguration;
        private AuthFssTokenProvider _fakeAuthFssTokenProvider;

        [SetUp]
        public void Setup()
        {
            _fakeAzureADConfiguration = A.Fake<IOptions<AzureADConfiguration>>();

            _fakeAuthFssTokenProvider = new AuthFssTokenProvider(_fakeAzureADConfiguration);
        }

    }
}
