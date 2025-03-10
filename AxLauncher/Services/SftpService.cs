// Services/SftpService.cs
using Renci.SshNet;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using AxLauncher.Utilities;

namespace AxLauncher.Services
{
    /// <summary>
    /// Сервис для загрузки файлов с SFTP-сервера
    /// </summary>
    public class SftpService : ISftpService
    {
        private string sftpHost;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SftpService"/>
        /// </summary>
        public SftpService()
        {
            Logger.Info("Инициализирован SftpService");
        }

        /// <summary>
        /// Инициализирует хост SFTP на основе текущего IP-адреса
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию инициализации</returns>
        public async Task InitializeSftpHostAsync()
        {
            try
            {
                Logger.Info("Инициализация SFTP хоста...");
                
                if (string.IsNullOrEmpty(AppConfig.PrimaryServerIp) || string.IsNullOrEmpty(AppConfig.FallbackServerIp))
                {
                    throw new InvalidOperationException("Не настроены IP-адреса серверов. Пожалуйста, настройте конфигурационный файл.");
                }
                
                string publicIP = await GetPublicIPAsync();
                sftpHost = publicIP == AppConfig.PrimaryServerIp ? AppConfig.FallbackServerIp : AppConfig.PrimaryServerIp;
                
                Logger.Info($"SFTP хост инициализирован: {sftpHost}");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при инициализации SFTP хоста");
                throw;
            }
        }

        /// <summary>
        /// Получает публичный IP-адрес
        /// </summary>
        /// <returns>IP-адрес в виде строки</returns>
        private async Task<string> GetPublicIPAsync()
        {
            string url = "https://api.ipify.org?format=text";
            using HttpClient client = new HttpClient();
            try
            {
                string publicIP = await client.GetStringAsync(url);
                Logger.Info("Ваш глобальный IP-адрес: " + publicIP);
                return publicIP;
            }
            catch (HttpRequestException ex)
            {
                Logger.Exception(ex, "Ошибка при получении IP-адреса");
                return null;
            }
        }

        /// <summary>
        /// Загружает все необходимые файлы с SFTP-сервера
        /// </summary>
        /// <param name="progress">Объект для отслеживания прогресса загрузки</param>
        /// <returns>Task, представляющий асинхронную операцию загрузки</returns>
        public async Task DownloadAllFilesAsync(IProgress<double> progress)
        {
            try
            {
                Logger.Info("Начинаем загрузку файлов...");
                if (string.IsNullOrEmpty(sftpHost))
                {
                    Logger.Info("SFTP хост не инициализирован, выполняем инициализацию...");
                    await InitializeSftpHostAsync();
                }

                string localRootPath = AppConfig.GameDirectory;
                Logger.Info($"Локальный путь: {localRootPath}");

                // Создаем директорию, если она не существует
                if (!Directory.Exists(localRootPath))
                {
                    Logger.Info($"Создаем директорию: {localRootPath}");
                    Directory.CreateDirectory(localRootPath);
                }

                using var sftp = new SftpClient(sftpHost, AppConfig.SftpPort, AppConfig.SftpUsername, AppConfig.SftpPassword);
                Logger.Info($"Подключаемся к SFTP серверу: {sftpHost}:{AppConfig.SftpPort} с пользователем: {AppConfig.SftpUsername}");
                
                try
                {
                    sftp.Connect();
                    Logger.Info("Подключение к SFTP серверу установлено");
                    
                    await DownloadAllFilesFromSftpAsync(sftp, AppConfig.SftpRootPath, localRootPath, true, progress);
                    
                    sftp.Disconnect();
                    Logger.Info("Отключение от SFTP сервера");
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Ошибка при работе с SFTP");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при загрузке файлов");
                throw;
            }
        }

        /// <summary>
        /// Рекурсивно загружает файлы из указанной директории на SFTP-сервере
        /// </summary>
        /// <param name="sftp">Клиент SFTP</param>
        /// <param name="remotePath">Удаленный путь на сервере</param>
        /// <param name="localPath">Локальный путь для сохранения</param>
        /// <param name="isRoot">Является ли директория корневой</param>
        /// <param name="progress">Объект для отслеживания прогресса</param>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        private async Task DownloadAllFilesFromSftpAsync(SftpClient sftp, string remotePath, string localPath, bool isRoot, IProgress<double> progress)
        {
            try
            {
                Logger.Info($"Обработка директории: {remotePath} -> {localPath}");
                var remoteFiles = sftp.ListDirectory(remotePath);

                var remoteFileNames = new HashSet<string>();
                double totalFiles = 0;
                foreach (var remoteFile in remoteFiles)
                {
                    if (remoteFile.Name != "." && remoteFile.Name != "..")
                    {
                        totalFiles++;
                    }
                }

                double processedFiles = 0;

                foreach (var remoteFile in remoteFiles)
                {
                    if (remoteFile.Name == "." || remoteFile.Name == "..")
                        continue;

                    remoteFileNames.Add(remoteFile.Name);

                    string localFilePath = Path.Combine(localPath, remoteFile.Name);

                    if (remoteFile.IsDirectory)
                    {
                        if (!Directory.Exists(localFilePath))
                        {
                            Logger.Info($"Создаем директорию: {localFilePath}");
                            Directory.CreateDirectory(localFilePath);
                        }
                        await DownloadAllFilesFromSftpAsync(sftp, remoteFile.FullName, localFilePath, false, progress);
                    }
                    else
                    {
                        bool needDownload = true;
                        
                        if (File.Exists(localFilePath))
                        {
                            // Получаем размер локального файла и сравниваем с удаленным
                            var localFileInfo = new FileInfo(localFilePath);
                            if (localFileInfo.Length == remoteFile.Length)
                            {
                                Logger.Info($"Пропущено (файл совпадает по размеру): {remoteFile.Name}");
                                needDownload = false;
                            }
                        }
                        
                        if (needDownload)
                        {
                            try
                            {
                                Logger.Info($"Загрузка файла: {remoteFile.Name} ({remoteFile.Length} байт)");
                                using var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write);
                                var asyncResult = sftp.BeginDownloadFile(remoteFile.FullName, fileStream, null, null);
                                await Task.Factory.FromAsync(asyncResult, sftp.EndDownloadFile);
                                Logger.Info($"Загружено: {remoteFile.Name}");
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, $"Ошибка при загрузке файла {remoteFile.Name}");
                            }
                        }
                    }

                    processedFiles++;
                    double percentage = (processedFiles / totalFiles) * 100 * 0.5;
                    progress?.Report(percentage);
                }

                // Удаляем лишние файлы в локальной директории, которых нет на сервере
                if (!isRoot) // Не удаляем файлы из корневой директории
                {
                    var localEntries = Directory.GetFileSystemEntries(localPath);
                    foreach (var localEntry in localEntries)
                    {
                        var localName = Path.GetFileName(localEntry);
                        if (!remoteFileNames.Contains(localName))
                        {
                            try
                            {
                                if (Directory.Exists(localEntry))
                                {
                                    Logger.Info($"Удаляем директорию: {localEntry}");
                                    Directory.Delete(localEntry, true);
                                }
                                else if (File.Exists(localEntry))
                                {
                                    Logger.Info($"Удаляем файл: {localEntry}");
                                    File.Delete(localEntry);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Exception(ex, $"Ошибка при удалении {localEntry}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, $"Ошибка при обработке директории {remotePath}");
                throw;
            }
        }
    }
}
