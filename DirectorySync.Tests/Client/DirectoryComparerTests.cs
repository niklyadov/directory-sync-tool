using System.Collections.Generic;
using System.Linq;
using DirectorySync.Client;
using NUnit.Framework;

namespace DirectorySync.Tests.Client;

public class DirectoryComparerTests
{
    private const string _clientPath = "/test/local";
    private const string _serverPath = "/test/remote";
    
    [Test]
    public void GetFileInfosToDownloadTest()
    {
        var directoryComparer = GetDirectoryComparerInstance();

        var testFilesToDownload = directoryComparer.GetFileInfosToDownload();

        var relativePathToDownload = testFilesToDownload.Single().RelativePath;

        Assert.AreEqual(string.IsNullOrEmpty(relativePathToDownload), false);
        Assert.AreEqual(relativePathToDownload, "only_server_file");
    }
    
    [Test]
    public void GetFileInfosToDeleteTest()
    {
        var directoryComparer = GetDirectoryComparerInstance();

        var testFilesToDelete = directoryComparer.GetFileInfosToDelete();

        var relativePathToDelete = testFilesToDelete.Single().RelativePath;

        Assert.AreEqual(string.IsNullOrEmpty(relativePathToDelete), false);
        Assert.AreEqual(relativePathToDelete, "only_client_file");
    }
    
    [Test]
    public void GetFileInfosPairToRename()
    {
        var directoryComparer = GetDirectoryComparerInstance();

        var testFilesToRename = directoryComparer
            .GetFileInfosPairToRename()
            .Single();

        var clientRelativePathToRename = testFilesToRename.clientFi.RelativePath;
        var serverRelativePathToRename = testFilesToRename.serverFi.RelativePath;
        
        Assert.AreEqual(string.IsNullOrEmpty(clientRelativePathToRename), false);
        Assert.AreEqual(string.IsNullOrEmpty(serverRelativePathToRename), false);
        
        Assert.AreEqual(clientRelativePathToRename, "file_for_rename_client");
        Assert.AreEqual(serverRelativePathToRename, "file_for_rename_server");
    }

    private DirectoryComparer GetDirectoryComparerInstance()
    {
        var testClientDirectoryInfo = new DirectoryInfo
        {
            FileInfos = GetClientFileInfos(), 
            WorkingPath = _clientPath
        };
        var testServerDirectoryInfo = new DirectoryInfo
        {
            FileInfos = GetServerFileInfos(),
            WorkingPath = _serverPath
        };

        return new DirectoryComparer(
            testClientDirectoryInfo, 
            testServerDirectoryInfo);
    }

    private List<FileInfo> GetClientFileInfos() 
        => new ()
        {
            new FileInfo
            {
                FullPath = $"{_clientPath}/only_client_file",
                RelativePath = "only_client_file",
                Hash = "1"
            },
            new FileInfo
            {
                FullPath = $"{_clientPath}/client_server_file",
                RelativePath = "client_server_file",
                Hash = "2"
            },
            new FileInfo
            {
                FullPath = $"{_clientPath}/file_for_rename_client",
                RelativePath = "file_for_rename_client",
                Hash = "3"
            }
        };

    private List<FileInfo> GetServerFileInfos()
        => new()
        {
            new FileInfo
            {
                FullPath = $"{_clientPath}/only_server_file",
                RelativePath = "only_server_file",
                Hash = "-1"
            },
            new FileInfo
            {
                FullPath = $"{_clientPath}/client_server_file",
                RelativePath = "client_server_file",
                Hash = "2"
            },
            new FileInfo
            {
                FullPath = $"{_clientPath}/file_for_rename_server",
                RelativePath = "file_for_rename_server",
                Hash = "3"
            }
        };
}