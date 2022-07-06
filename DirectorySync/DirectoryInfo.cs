namespace DirectorySync
{
    public class DirectoryInfo
    {
        public string? WorkingPath { get; set; }
        public List<FileInfo> FileInfos { get; set; } = new List<FileInfo>();

        public DirectoryInfo()
        {
        }

        public DirectoryInfo(string wokringPath)
        {
            WorkingPath = wokringPath;
            FileInfos = ScanFileInfoss(wokringPath);
        }

        public static DirectoryInfo Scan(string workingPath)
        {
            return new DirectoryInfo(workingPath);
        }

        public List<FileInfoAction> CompareDirectories(DirectoryInfo otherDirectory)
        {
            if (otherDirectory == null)
                throw new ArgumentNullException(nameof(otherDirectory));

            if (otherDirectory.FileInfos == null)
                throw new InvalidOperationException("File infos List for the other DirectoryInfo is null");

            var fileActions = new List<FileInfoAction>();

            foreach (var serverFileInfo in otherDirectory.FileInfos)
            {
                var fileInfosWithSameNames = FileInfos.Where(clientFI => serverFileInfo.RelativePath.Equals(clientFI.RelativePath));
                var fileInfosWithSameHashes = FileInfos.Where(clientFI => serverFileInfo.Hash.Equals(clientFI.Hash));

                if (!fileInfosWithSameNames.Any() && !fileInfosWithSameHashes.Any() ||
                     fileInfosWithSameNames.Any() && !fileInfosWithSameHashes.Any())
                    fileActions.Add(new FileInfoAction(serverFileInfo, FileAction.Download));

                if (!fileInfosWithSameNames.Any() && fileInfosWithSameHashes.Any())
                    fileActions.Add(new FileInfoAction(serverFileInfo, FileAction.Rename));
            }

            var fileActionsToDelete = FileInfos.Where(client => !otherDirectory.FileInfos.Contains(client))
                .Select(toDelete => new FileInfoAction(toDelete, FileAction.Delete)).ToList();

            fileActions.AddRange(fileActionsToDelete);

            return fileActions;
        }

        private static List<FileInfo> ScanFileInfoss(string workingPath)
        {
            var fileNamesInDirectory = Directory.EnumerateFiles(workingPath, "*.*", SearchOption.AllDirectories);
            var fileInfosInDirectory = fileNamesInDirectory.Select(filePath =>
            {
                var fullPath = Path.Combine(workingPath, filePath);
                var filename = Path.GetFileName(fullPath);

                return new FileInfo(fullPath, filename);
            });

            return fileInfosInDirectory.ToList();
        }
    }
}