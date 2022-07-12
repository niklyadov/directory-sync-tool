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
                do
                {
                    Console.Write("Enter connection port: ");
                } while (!ushort.TryParse(Console.ReadLine(), out port));

                var client = new DirectorySyncClient(port);

                var serverDirectoryInfo = await client.GetDirectoryInfoAsync();

                if (serverDirectoryInfo.FileInfos == null)
                    throw new InvalidDataException("Failed to get directory infos");

    
                var localDirectoryInfo = DirectoryInfo.Scan(Props.LOCALPATH);

                var directoryComparer = new DirectoryComparer(localDirectoryInfo, serverDirectoryInfo);

                foreach (var fileInfo in directoryComparer.GetFileInfosToDelete())
                {
                    var relativePath = fileInfo.RelativePath;
                    var destPath = Path.Combine(Props.LOCALPATH, relativePath);


                    _logger.LogInformation($"DONE Delete: file from ${destPath}");

                    File.Delete(destPath);
                }

                foreach (var fileInfos in directoryComparer.GetFileInfosPairToRename())
                {
                    var clientFileInfo = fileInfos.clientFi;
                    var serverFileInfo = fileInfos.serverFi;

                    var oldFilePath = Path.Combine(Props.LOCALPATH, clientFileInfo.RelativePath);
                    var newFilePath = Path.Combine(Props.LOCALPATH, serverFileInfo.RelativePath);

                    _logger.LogInformation($"DONE Rename: file from {oldFilePath} to {newFilePath}");

                    CreateDirectoryForFileIfNotExists(newFilePath);

                    File.Move(oldFilePath, newFilePath);
                }

                await Parallel.ForEachAsync(directoryComparer.GetFileInfosToDownload(), async (fileInfo, d) =>
                {
                    var relativePath = fileInfo.RelativePath;
                    var destPath = Path.Combine(Props.LOCALPATH, relativePath);

                    var downloadedFileStream = await client.GetFileStreamAsync(relativePath);

                    CreateDirectoryForFileIfNotExists(destPath);
                    using var writeFilestream = new FileStream(destPath, FileMode.Create, FileAccess.Write);
                    downloadedFileStream.CopyTo(writeFilestream);

                    _logger.LogInformation($"DONE Download: file ${destPath}");
                });

            });
        }

        private void CreateDirectoryForFileIfNotExists(string filePath)
        {
            var newFilePathDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(newFilePathDirectory) && newFilePathDirectory != null)
                Directory.CreateDirectory(newFilePathDirectory);
        }
    }
}
