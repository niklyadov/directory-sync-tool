using System.Security.Cryptography;

namespace DirectorySync
{
    public class FileInfo
    {
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public string Hash { get; set; }

        public FileInfo(string fullPath, string relativePath)
        {
            FullPath = fullPath;
            RelativePath = relativePath;
            Hash = GetHash();
        }

        public string GetHash()
        {
            var data = File.ReadAllBytes(FullPath);

            using var sha256 = SHA256.Create();

            return string.Concat(sha256.ComputeHash(data).Select(x => x.ToString("X2")));
        }

        public override string ToString()
        {
            return $"{FullPath} {RelativePath} {GetHash()}";
        }

        public bool Equals(FileInfo other)
        {
            var hashEquals = Hash.Equals(other.Hash);
            var relativePathEquals = RelativePath.Equals(other.RelativePath);

            return hashEquals && relativePathEquals;
        }
    }
}
