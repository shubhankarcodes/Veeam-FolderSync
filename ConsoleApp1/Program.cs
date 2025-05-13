using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;

class FolderSync
{
    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSync <source> <replica> <interval_in_seconds> <log_file>");
            return;
        }

        string sourcePath = args[0];
        string replicaPath = args[1];
        if (!int.TryParse(args[2], out int intervalSeconds) || intervalSeconds <= 0)
        {
            Console.WriteLine("Invalid synchronization interval.");
            return;
        }
        string logFile = args[3];

        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine($"Source folder does not exist: {sourcePath}");
            return;
        }

        Directory.CreateDirectory(replicaPath); 

        while (true)
        {
            try
            {
                SyncFolders(sourcePath, replicaPath, logFile);
            }
            catch (Exception ex)
            {
                Log(logFile, $"[ERROR] {ex.Message}");
            }

            Thread.Sleep(intervalSeconds * 1000);
        }
    }

    static void SyncFolders(string sourceDir, string replicaDir, string logFile)
    {
        // Sync files and directories
        foreach (string srcFilePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDir, srcFilePath);
            string replicaFilePath = Path.Combine(replicaDir, relativePath);
            string replicaDirPath = Path.GetDirectoryName(replicaFilePath);
            if (!Directory.Exists(replicaDirPath))
            {
                Directory.CreateDirectory(replicaDirPath);
                Log(logFile, $"[CREATE FOLDER] {replicaDirPath}");
            }

            if (!File.Exists(replicaFilePath) || !FilesAreEqual(srcFilePath, replicaFilePath))
            {
                File.Copy(srcFilePath, replicaFilePath, true);
                Log(logFile, $"[COPY] {srcFilePath} -> {replicaFilePath}");
            }
        }

        // Delete files not present in source
        foreach (string repFilePath in Directory.GetFiles(replicaDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(replicaDir, repFilePath);
            string correspondingSourcePath = Path.Combine(sourceDir, relativePath);
            if (!File.Exists(correspondingSourcePath))
            {
                File.Delete(repFilePath);
                Log(logFile, $"[DELETE FILE] {repFilePath}");
            }
        }

        // Delete directories not present in source
        foreach (string repSubDir in Directory.GetDirectories(replicaDir, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
        {
            string relativePath = Path.GetRelativePath(replicaDir, repSubDir);
            string correspondingSourceDir = Path.Combine(sourceDir, relativePath);
            if (!Directory.Exists(correspondingSourceDir))
            {
                Directory.Delete(repSubDir, true);
                Log(logFile, $"[DELETE FOLDER] {repSubDir}");
            }
        }
    }

    static bool FilesAreEqual(string file1, string file2)
    {
        using (var hashAlgo = MD5.Create())
        {
            using (var fs1 = File.OpenRead(file1))
            using (var fs2 = File.OpenRead(file2))
            {
                byte[] hash1 = hashAlgo.ComputeHash(fs1);
                byte[] hash2 = hashAlgo.ComputeHash(fs2);
                return hash1.SequenceEqual(hash2);
            }
        }
    }

    static void Log(string logFile, string message)
    {
        string fullMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
        Console.WriteLine(fullMessage);
        File.AppendAllText(logFile, fullMessage + Environment.NewLine);
    }
}
