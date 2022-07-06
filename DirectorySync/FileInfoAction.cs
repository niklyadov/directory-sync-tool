namespace DirectorySync
{
    public class FileInfoAction
    {
        public FileInfo FileInfo { get; set; }

        public FileAction FileAction { get; set; }

        public FileInfoAction(FileInfo fileInfo, FileAction fileAction)
        {
            FileInfo = fileInfo;
            FileAction = fileAction;
        }
    }
}