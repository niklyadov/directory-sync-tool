using Newtonsoft.Json;

namespace DirectorySync.Client
{
    public class DirectorySyncClient
    {
        private readonly Uri _apiBaseUrl;
        private readonly Uri _apiDirectoryInfoUrl;
        private readonly Uri _apiDownloadFileUrl;

        public DirectorySyncClient(ushort port)
        {
            _apiBaseUrl = new Uri($"http://localhost:{port}/");
            _apiDirectoryInfoUrl = new Uri(_apiBaseUrl, "/directoryInfo/");
            _apiDownloadFileUrl = new Uri(_apiBaseUrl, "/s/");
        }

        public async Task<DirectoryInfo> GetDirectoryInfoAsync()
        {
            var fullUri = new Uri(_apiBaseUrl, _apiDirectoryInfoUrl);
            var downloadedString = await DownloadStringAsync(fullUri);
            var result = JsonConvert.DeserializeObject<DirectoryInfo>(downloadedString);

            if (result == null)
                throw new InvalidDataException("Couldn't to parse JSON");

            return result;
        }

        public async Task<Stream> GetFileStreamAsync(string filename)
        {
            filename = filename.Trim().Replace('\\', '/');

            var fullUri = new Uri(_apiBaseUrl, _apiDownloadFileUrl);
                fullUri = new Uri($"{fullUri}/{filename}");

            return await GetStreamFromUriAsync(fullUri);
        }

        private static async Task<string> DownloadStringAsync(Uri downloadUri)
        {
            using var hc = new HttpClient();
            return await hc.GetStringAsync(downloadUri);
        }

        private static async Task<Stream> GetStreamFromUriAsync(Uri downloadUri)
        {
            using var hc = new HttpClient();
            return await hc.GetStreamAsync(downloadUri);
        }
    }
}
