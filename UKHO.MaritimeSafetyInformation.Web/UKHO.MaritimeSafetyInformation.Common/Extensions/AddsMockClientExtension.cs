using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Rhz
/// In Development, this extension is used to configure the mock client url to ADDS.Mock for the MSI service.
/// This,seems
/// </summary>  
namespace UKHO.ADDS.Mocks.MSI.Extensions
{
    public static class AddsMockClientExtension
    {
        public static WebApplicationBuilder AddMockClientConfig(this WebApplicationBuilder builder, string resource, string prefix, string target = null)
        {
            var httpsEndpoint = builder.Configuration.GetSection($"services:{resource}:https:0").Value;
            var httpEndpoint = builder.Configuration.GetSection($"services:{resource}:http:0").Value;

            httpsEndpoint = new UriBuilder(httpsEndpoint) { Path = prefix }.Uri.ToString();
            httpEndpoint = new UriBuilder(httpEndpoint) { Path = prefix }.Uri.ToString();

            if (!string.IsNullOrEmpty(target))
            {
                builder.Configuration[target] = httpEndpoint;
            }

            var configValues = new Dictionary<string, string>
            {
                {$"ConnectionStrings:{resource}-https", httpsEndpoint! },
                {$"ConnectionStrings:{resource}-http", httpEndpoint! },

            };
            builder.Configuration.AddInMemoryCollection(configValues!);
            
            return builder;
        }
    }
}
