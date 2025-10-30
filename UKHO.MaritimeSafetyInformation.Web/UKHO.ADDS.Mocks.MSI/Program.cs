using UKHO.ADDS.Mocks.Configuration;
using UKHO.ADDS.Mocks.Domain.Configuration;

namespace UKHO.ADDS.Mocks.MSI
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            MockServices.AddServices();
            ServiceRegistry.AddDefinition(new ServiceDefinition("fssmsi", "FileShare Service (MSI)", []));

            await MockServer.RunAsync(args);
        }
    }
}
