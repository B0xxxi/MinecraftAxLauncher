// Services/ISftpService.cs
using System;
using System.Threading.Tasks;

namespace AxLauncher.Services
{
    /// <summary>
    /// Интерфейс сервиса для работы с SFTP и обновления файлов
    /// </summary>
    public interface ISftpService
    {
        /// <summary>
        /// Инициализирует хост SFTP
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию инициализации</returns>
        Task InitializeSftpHostAsync();

        /// <summary>
        /// Загружает все необходимые файлы с SFTP-сервера
        /// </summary>
        /// <param name="progress">Объект для отслеживания прогресса загрузки</param>
        /// <returns>Task, представляющий асинхронную операцию загрузки</returns>
        Task DownloadAllFilesAsync(IProgress<double> progress);
    }
} 