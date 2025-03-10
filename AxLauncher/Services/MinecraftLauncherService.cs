// Services/MinecraftLauncherService.cs
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AxLauncher.Models;
using AxLauncher.Utilities;

namespace AxLauncher.Services
{
    /// <summary>
    /// Сервис для запуска Minecraft с заданными параметрами
    /// </summary>
    public class MinecraftLauncherService : IMinecraftLauncherService
    {
        private readonly MinecraftLauncher launcher;
        private readonly UserSettings userSettings;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MinecraftLauncherService"/>
        /// </summary>
        /// <param name="settings">Пользовательские настройки</param>
        public MinecraftLauncherService(UserSettings settings)
        {
            userSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            var path = new MinecraftPath(AppConfig.GameDirectory);
            launcher = new MinecraftLauncher(path);
            Logger.Info($"Инициализирован MinecraftLauncherService с путем: {path.BasePath}");
        }

        /// <summary>
        /// Запускает Minecraft с указанными настройками
        /// </summary>
        /// <param name="progress">Объект для отслеживания прогресса запуска</param>
        /// <returns>Task, представляющий асинхронную операцию запуска</returns>
        public async Task LaunchMinecraftAsync(IProgress<double> progress)
        {
            try
            {
                Logger.Info("Начинаем запуск Minecraft...");
                System.Net.ServicePointManager.DefaultConnectionLimit = 256;

                var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(x =>
                {
                    double percentage = ((double)x.ProgressedTasks / x.TotalTasks) * 100 * 0.5;
                    progress?.Report(50 + percentage * 0.5);
                    Logger.Info($"[{x.EventType}][{x.ProgressedTasks}/{x.TotalTasks}] {x.Name}");
                });
                var byteProgress = new SyncProgress<ByteProgress>(x =>
                {
                    Logger.Info($"Загрузка: {x.ToRatio() * 100:F2}%");
                });
                var installerOutput = new SyncProgress<string>(x =>
                    Logger.Info($"Установщик Forge: {x}"));

                var forge = new ForgeInstaller(launcher);

                Logger.Info($"Установка Forge {AppConfig.ForgeVersion} для Minecraft {AppConfig.MinecraftVersion}...");
                var versionName = await forge.Install(AppConfig.MinecraftVersion, AppConfig.ForgeVersion, new ForgeInstallOptions
                {
                    FileProgress = fileProgress,
                    ByteProgress = byteProgress,
                    InstallerOutput = installerOutput,
                });

                progress?.Report(75);
                Logger.Info($"Forge установлен. Версия: {versionName}");

                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = userSettings.RAM,
                    Session = MSession.CreateOfflineSession(userSettings.Login),
                };

                Logger.Info($"Создаем процесс с RAM: {userSettings.RAM} MB, пользователь: {userSettings.Login}");
                Process process = await launcher.CreateProcessAsync(versionName, launchOption);

                progress?.Report(85);
                Logger.Info("Процесс создан, запускаем игру...");

                var processUtil = new ProcessWrapper(process);
                processUtil.OutputReceived += (s, x) => Logger.Info($"Minecraft: {x}");
                processUtil.StartWithEvents();
                progress?.Report(100);
                Logger.Info("Minecraft запущен успешно!");

                await processUtil.WaitForExitTaskAsync();
                Logger.Info("Minecraft завершил работу.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при запуске Minecraft");
                throw;
            }
        }
    }
}

