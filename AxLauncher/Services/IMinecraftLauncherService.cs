using System;
using System.Threading.Tasks;

namespace AxLauncher.Services
{
    /// <summary>
    /// Интерфейс сервиса для запуска Minecraft с настройками пользователя
    /// </summary>
    public interface IMinecraftLauncherService
    {
        /// <summary>
        /// Запускает Minecraft с указанными настройками
        /// </summary>
        /// <param name="progress">Объект для отслеживания прогресса запуска</param>
        /// <returns>Task, представляющий асинхронную операцию запуска</returns>
        Task LaunchMinecraftAsync(IProgress<double> progress);
    }
} 