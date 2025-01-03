using Docker.DotNet.Models;

namespace UKHO.MaritimeSafetyInformation.Local.Services
{
    internal static class SqlLDocker
    {
        private const string ImageName = @"mcr.microsoft.com/mssql/server";

        public static async Task StartContainerAsync()
        {
            var containerConfig = new Config
            {
                Image = ImageName,
                Env = new List<string>
                {
                    "ACCEPT_EULA=Y",
                    "SA_PASSWORD=Passw0rd123"
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "1433/tcp", new EmptyStruct() }
                }
            };
            
            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "1433/tcp", new List<PortBinding> { new PortBinding { HostPort = "1439" } } }
                },
                AutoRemove = true
            };

            await DockerHelper.PullDockerImageAsync(ImageName);
            await DockerHelper.RunDockerContainerAsync(ImageName, containerConfig, hostConfig);
        }

        public static async Task StopContainerAsync()
        {
            await DockerHelper.StopDockerContainerAsync(ImageName);
        }
    }
}
