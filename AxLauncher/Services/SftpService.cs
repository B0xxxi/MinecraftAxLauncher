// Services/SftpService.cs
using Renci.SshNet;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
            await DownloadAllFilesFromSftpAsync(sftp, sftpRootPath, localRootPath);
            sftp.Disconnect();
        }

        private async Task DownloadAllFilesFromSftpAsync(SftpClient sftp, string remotePath, string localPath)
        {
            var files = sftp.ListDirectory(remotePath);
            foreach (var file in files)
            {
                string localFilePath = Path.Combine(localPath, file.Name);
                if (file.IsDirectory)
                {
                    if (file.Name != "." && file.Name != "..")
                    {
                        Directory.CreateDirectory(localFilePath);
                        await DownloadAllFilesFromSftpAsync(sftp, file.FullName, localFilePath);
                    }
                }
                else
                {
                    using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write);
                    var asyncResult = sftp.BeginDownloadFile(file.FullName, fileStream, null, null);
                    await Task.Run(() => sftp.EndDownloadFile(asyncResult));
                    Console.WriteLine($"Downloaded: {file.Name}");
                }
            }
        }
    }
}
