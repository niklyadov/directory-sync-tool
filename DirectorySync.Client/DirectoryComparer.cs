namespace DirectorySync.Client
{
    public class DirectoryComparer
    {
        private DirectoryInfo _clientDirectoryInfo { get; set; }
        private DirectoryInfo _serverDirectoryInfo { get; set; }

        public DirectoryComparer(DirectoryInfo clientDirectoryInfo, DirectoryInfo serverDirectoryInfo)
        {
            _clientDirectoryInfo = clientDirectoryInfo;
            _serverDirectoryInfo = serverDirectoryInfo;
        }

        public ICollection<FileInfo> GetFileInfosToDownload() =>
            _serverDirectoryInfo.FileInfos.Where(sfi =>
            {
                var fileInfosWithSameNames = _clientDirectoryInfo.FileInfos
                    .Where(clientFI => sfi.RelativePath.Equals(clientFI.RelativePath));

                var fileInfosWithSameHashes = _clientDirectoryInfo.FileInfos
                    .Where(clientFI => sfi.Hash.Equals(clientFI.Hash));

                return !fileInfosWithSameNames.Any() && !fileInfosWithSameHashes.Any() ||
                     fileInfosWithSameNames.Any() && !fileInfosWithSameHashes.Any();
            }).ToList();

        public ICollection<FileInfo> GetFileInfosToDelete() =>
            _clientDirectoryInfo.FileInfos.Where(cfi =>
                !_serverDirectoryInfo.FileInfos.Contains(cfi))
            .ToList();
    }
}
