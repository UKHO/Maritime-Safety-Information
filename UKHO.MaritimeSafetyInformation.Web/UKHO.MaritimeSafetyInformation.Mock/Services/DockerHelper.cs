using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;

namespace UKHO.MaritimeSafetyInformation.Local.Services
{
    public static class DockerHelper
    {
        private static readonly DockerClient _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();

        public static async Task PullDockerImageAsync(string imageName)
        {
            if (await ImageAlreadyExists(imageName)) return;

            var parameters = new ImagesCreateParameters
            {
                FromImage = imageName,
                Tag = "latest"
            };

            var progress = new Progress<JSONMessage>(message => Console.WriteLine(message.Stream));

            try
            {
                await _dockerClient.Images.CreateImageAsync(parameters, null, progress);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public static async Task BuildDockerImageAsync(string imageName, string dockerfilePath)
        {
            if (await ImageAlreadyExists(imageName)) return;

            using (var tarStream = new MemoryStream())
            {
                using (var tar = new TarOutputStream(tarStream, Encoding.UTF8))
                {
                    tar.IsStreamOwner = false;
                    var entry = TarEntry.CreateTarEntry(Path.GetFileName(dockerfilePath));
                    entry.Size = new FileInfo(dockerfilePath).Length;
                    tar.PutNextEntry(entry);
                    using (var fileStream = File.OpenRead(dockerfilePath))
                    {
                        await fileStream.CopyToAsync(tar);
                    }
                    tar.CloseEntry();
                }
                tarStream.Seek(0, SeekOrigin.Begin);

                var parameters = new ImageBuildParameters
                {
                    Tags = new[] { imageName }
                };

                var progress = new Progress<JSONMessage>(message =>
                {
                    Console.WriteLine(message.Stream);
                });

                try
                {
                    await _dockerClient.Images.BuildImageFromDockerfileAsync(parameters, tarStream, null, null, progress, CancellationToken.None);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        public static async Task RunDockerContainerAsync(string imageName, Config containerConfig, HostConfig hostConfig)
        {
            if (await IsContainerRunning(imageName)) return;

            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters(containerConfig)
            {
                HostConfig = hostConfig
            });

            try
            {
                await _dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting container: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public static async Task StopDockerContainerAsync(string imageName)
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var container = containers.FirstOrDefault(c => c.Image == imageName && c.State == "running");

            if (container != null)
            {
                try
                {
                    await _dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());
                    Console.WriteLine($"Container with image {imageName} has been stopped.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping container: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
            else
            {
                Console.WriteLine($"No running container found with image {imageName}.");
            }
        }

        private static async Task<bool> ImageAlreadyExists(string imageName)
        {
            var listImagesParameters = new ImagesListParameters { All = true };
            var images = await _dockerClient.Images.ListImagesAsync(listImagesParameters);
            return images.Any(image => image.RepoTags.Any(tag => tag.Contains(imageName)));
        }

        private static async Task<bool> IsContainerRunning(string imageName)
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var existingContainer = containers.FirstOrDefault(c => c.Image == imageName && c.State == "running");

            if (existingContainer != null)
            {
                Console.WriteLine($"Container with image {imageName} is already running.");
                return true;
            }

            return false;
        }
    }
}
