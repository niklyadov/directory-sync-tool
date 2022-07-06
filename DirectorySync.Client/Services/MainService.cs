using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectorySync.Client.Services
{
    public class MainService : BackgroundService
    {
        private readonly ILogger<MainService> _logger;

        public MainService(ILogger<MainService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                ushort port = 6800;

                var client = new DirectorySyncClient(port);

                Console.WriteLine("Connecting...");

                var directoryInfo = await client.GetDirectoryInfoAsync();

                if (directoryInfo.FileInfos == null)
                    throw new Exception("Failed to get directory infos");

                var directory = @"";
                var directoryActions = DirectoryInfo.Scan(directory).CompareDirectories(directoryInfo);

                foreach (var action in directoryActions)
                {
                    switch (action.FileAction)
                    {
                        case FileAction.Download:
                            {

                                var rp = action.FileInfo.RelativePath;
                                var file = client.GetFileStreamAsync(rp);

                                var destPath = Path.Combine(directory, rp);

                                var stream = await client.GetFileStreamAsync(rp);

                                using var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write);

                                stream.CopyTo(fileStream);

                                _logger.LogInformation($"downloading file to ${destPath}");
                            }
                            break;
                        case FileAction.Delete:
                            {
                                var rp = action.FileInfo.RelativePath;
                                var destPath = Path.Combine(directory, rp);

                                File.Delete(destPath);

                                _logger.LogInformation($"deleting file from ${destPath}");
                            }
                            break;

                        case FileAction.Rename:
                            {

                            }
                            break;
                    }
                }

                Console.Read();
            });
        }
    }
}
