using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace UKHO.MaritimeSafetyInformation.Local.Services
{
    internal static class AzuriteDocker
    {
        private const string ImageName = @"azurite-local";

        public static async Task StartContainerAsync()
        {
            var dockerfilePath = Path.Combine(Directory.GetCurrentDirectory(), "Docker", "Azurite", "Dockerfile");

            await DockerHelper.BuildDockerImageAsync(ImageName, dockerfilePath);

            var containerConfig = new Config
            {
                Image = ImageName,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "10000/tcp", new EmptyStruct() },
                    { "10001/tcp", new EmptyStruct() },
                    { "10002/tcp", new EmptyStruct() }
                }
            };

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "10000/tcp", new List<PortBinding> { new PortBinding { HostPort = "10000" } } },
                    { "10001/tcp", new List<PortBinding> { new PortBinding { HostPort = "10001" } } },
                    { "10002/tcp", new List<PortBinding> { new PortBinding { HostPort = "10002" } } }
                },
                AutoRemove = true
            };

            await DockerHelper.RunDockerContainerAsync(ImageName, containerConfig, hostConfig);
        }
        public static async Task StopContainerAsync()
        {
            await DockerHelper.StopDockerContainerAsync(ImageName);
        }
    }
}
