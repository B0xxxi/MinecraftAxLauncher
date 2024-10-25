// Services/SftpService.cs
using Renci.SshNet;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AxLauncher.Services
{
    public class SftpService
    {
        private const string sftpUsername = "sftpuser";
        private const string sftpPassword = "Olezja";
        private const int sftpPort = 22;
        private const string sftpRootPath = "/test";
        private string sftpHost;
        private readonly string userProfile;

        public SftpService()
        {
            userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public async Task InitializeSftpHostAsync()
        {
            string publicIP = await GetPublicIPAsync();
            sftpHost = publicIP == "136.169.223.12" ? "192.168.1.69" : "136.169.223.12";
        }

        private async Task<string> GetPublicIPAsync()
        {
            string url = "https://api.ipify.org?format=text";
            using HttpClient client = new HttpClient();
            try
            {
                string publicIP = await client.GetStringAsync(url);
                Console.WriteLine("Ваш глобальный IP-адрес: " + publicIP);
                return publicIP;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Ошибка при получении IP-адреса: " + e.Message);
                return null;
            }
        }

        public async Task DownloadAllFilesAsync()
        {
            if (string.IsNullOrEmpty(sftpHost))
            {
                await InitializeSftpHostAsync();
            }

            string localRootPath = Path.Combine(userProfile, ".axcraft");

            using var sftp = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword);
            sftp.Connect();
            await DownloadAllFilesFromSftpAsync(sftp, sftpRootPath, localRootPath, true);
            sftp.Disconnect();
        }

        private async Task DownloadAllFilesFromSftpAsync(SftpClient sftp, string remotePath, string localPath, bool isRoot)
        {
            var remoteFiles = sftp.ListDirectory(remotePath);

            // Собираем имена файлов и папок на сервере
            var remoteFileNames = new HashSet<string>();
            foreach (var remoteFile in remoteFiles)
            {
                if (remoteFile.Name == "." || remoteFile.Name == "..")
                    continue;

                remoteFileNames.Add(remoteFile.Name);

                string localFilePath = Path.Combine(localPath, remoteFile.Name);

                if (remoteFile.IsDirectory)
                {
                    Directory.CreateDirectory(localFilePath);
                    await DownloadAllFilesFromSftpAsync(sftp, remoteFile.FullName, localFilePath, false);
                }
                else
                {
                    if (!File.Exists(localFilePath))
                    {
                        using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write);
                        var asyncResult = sftp.BeginDownloadFile(remoteFile.FullName, fileStream, null, null);
                        await Task.Run(() => sftp.EndDownloadFile(asyncResult));
                        Console.WriteLine($"Загружено: {remoteFile.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Пропущено (файл существует): {remoteFile.Name}");
                    }
                }
            }

            // Удаляем ненужные файлы в подкаталогах
            var localEntries = Directory.GetFileSystemEntries(localPath);
            foreach (var localEntry in localEntries)
            {
                var localName = Path.GetFileName(localEntry);
                if (!remoteFileNames.Contains(localName))
                {
                    if (!isRoot) // Только в подкаталогах
                    {
                        try
                        {
                            if (Directory.Exists(localEntry))
                            {
                                Directory.Delete(localEntry, true);
                                Console.WriteLine($"Удалена папка: {localEntry}");
                            }
                            else if (File.Exists(localEntry))
                            {
                                File.Delete(localEntry);
                                Console.WriteLine($"Удален файл: {localEntry}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при удалении {localEntry}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
