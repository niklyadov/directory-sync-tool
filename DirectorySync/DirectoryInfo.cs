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
            FileInfos = ScanDirectory(wokringPath);
        }

        public static DirectoryInfo Scan(string workingPath)
        {
            return new DirectoryInfo(workingPath);
        }

        private static List<FileInfo> ScanDirectory(string workingPath)
        {
            var fileNamesInDirectory = Directory.EnumerateFiles(workingPath, "*.*", SearchOption.AllDirectories);
            var fileInfosInDirectory = fileNamesInDirectory.Select(filePath =>
            {
                var fullPath = Path.Combine(workingPath, filePath);
                var relativePath = Path.GetRelativePath(workingPath, fullPath);

                return new FileInfo(fullPath, relativePath);
            });

            return fileInfosInDirectory.ToList();
        }
    }
}