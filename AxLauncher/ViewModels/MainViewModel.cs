// ViewModels/MainViewModel.cs
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AxLauncher.Models;
using AxLauncher.Services;
using AxLauncher.Utilities;

namespace AxLauncher.ViewModels
{
    /// <summary>
    /// Основная модель представления приложения AxLauncher
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly UserSettings userSettings;
        private readonly ISftpService sftpService;
        private readonly IMinecraftLauncherService minecraftLauncherService;
        private bool filesChecked = false;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainViewModel"/>
        /// </summary>
        public MainViewModel()
        {
            Logger.Info("Инициализация MainViewModel...");
            
            // Загружаем настройки из файла или создаем новые по умолчанию
            userSettings = UserSettings.Load();
            
            sftpService = new SftpService();
            minecraftLauncherService = new MinecraftLauncherService(userSettings);

            // Загружаем значения из настроек
            Login = userSettings.Login;
            RAM = userSettings.RAM > 0 ? userSettings.RAM : AppConfig.DefaultRamMB;

            PlayCommand = new AsyncRelayCommand(async _ => await PlayAsync());
            CloseCommand = new RelayCommand(_ => CloseApplication());
            
            Logger.Info("MainViewModel инициализирован");
        }

        private string login;
        
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login
        {
            get => login;
            set
            {
                if (login != value)
                {
                    try
                    {
                        login = value;
                        userSettings.Login = value;
                        userSettings.Save();
                        OnPropertyChanged(nameof(Login));
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "Ошибка при установке логина");
                        MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private int ram = AppConfig.DefaultRamMB;
        
        /// <summary>
        /// Объем оперативной памяти для Minecraft (в МБ)
        /// </summary>
        public int RAM
        {
            get => ram;
            set
            {
                if (ram != value)
                {
                    try
                    {
                        ram = value;
                        userSettings.RAM = value;
                        userSettings.Save();
                        OnPropertyChanged(nameof(RAM));
                        Logger.Info($"Изменено значение RAM: {value} MB");
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "Ошибка при установке RAM");
                        MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private double progressValue;
        
        /// <summary>
        /// Значение прогресса выполнения операций (0-100)
        /// </summary>
        public double ProgressValue
        {
            get => progressValue;
            set
            {
                if (progressValue != value)
                {
                    progressValue = value;
                    OnPropertyChanged(nameof(ProgressValue));
                }
            }
        }

        /// <summary>
        /// Команда запуска игры
        /// </summary>
        public ICommand PlayCommand { get; }
        
        /// <summary>
        /// Команда закрытия приложения
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Асинхронно выполняет проверку файлов и запуск игры
        /// </summary>
        private async Task PlayAsync()
        {
            try
            {
                Logger.Info("Запуск игры...");
                
                if (string.IsNullOrWhiteSpace(Login))
                {
                    MessageBox.Show("Необходимо ввести логин", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var overallProgress = new Progress<double>(value =>
                {
                    ProgressValue = value;
                }) as IProgress<double>;

                if (!filesChecked)
                {
                    try
                    {
                        Logger.Info("Начинаем проверку файлов...");
                        var sftpProgress = new System.Progress<double>(value =>
                        {
                            overallProgress.Report(value * 0.5);
                        });

                        await sftpService.DownloadAllFilesAsync(sftpProgress);
                        filesChecked = true;
                        Logger.Info("Файлы успешно загружены!");
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "Не удалось загрузить файлы с SFTP-сервера");
                        MessageBox.Show("Не удалось обновить файлы с сервера. Игра будет запущена без обновлений.", "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                try
                {
                    Logger.Info("Запускаем Minecraft...");
                    var launchProgress = new System.Progress<double>(value =>
                    {
                        overallProgress.Report(50 + value * 0.5);
                    });

                    await minecraftLauncherService.LaunchMinecraftAsync(launchProgress);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Произошла ошибка при запуске игры");
                    MessageBox.Show($"Произошла ошибка при запуске игры: {ex.Message}", "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
                // Сбрасываем прогресс после завершения
                ProgressValue = 0;
            }
            finally
            {
                // Сохраняем лог при любом исходе
                Logger.FlushLog();
            }
        }

        /// <summary>
        /// Закрывает приложение
        /// </summary>
        private void CloseApplication()
        {
            Logger.Info("Завершение работы приложения");
            Logger.FlushLog();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Вызывает событие изменения свойства
        /// </summary>
        /// <param name="name">Имя измененного свойства</param>
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
