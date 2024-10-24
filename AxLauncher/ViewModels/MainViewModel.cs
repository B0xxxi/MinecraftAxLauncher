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
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly UserSettings userSettings;
        private readonly SftpService sftpService;
        private readonly MinecraftLauncherService minecraftLauncherService;
        private bool filesChecked = false;

        public MainViewModel()
        {
            userSettings = new UserSettings();
            sftpService = new SftpService();
            minecraftLauncherService = new MinecraftLauncherService(userSettings);

            // Используем AsyncRelayCommand для асинхронной команды
            PlayCommand = new AsyncRelayCommand(async _ => await PlayAsync());

            // Используем RelayCommand для синхронной команды
            CloseCommand = new RelayCommand(_ => CloseApplication());
        }

        private string login;
        public string Login
        {
            get => login;
            set
            {
                if (login != value)
                {
                    login = value;
                    userSettings.Login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }

        private int ram = 4096; // Значение по умолчанию
        public int RAM
        {
            get => ram;
            set
            {
                if (ram != value)
                {
                    ram = value;
                    userSettings.RAM = value;
                    OnPropertyChanged(nameof(RAM));
                }
            }
        }

        public ICommand PlayCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task PlayAsync()
        {
            if (!filesChecked)
            {
                try
                {
                    await sftpService.DownloadAllFilesAsync();
                    filesChecked = true;
                    Console.WriteLine("Files downloaded successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось загрузить файлы с SFTP-сервера: {ex.Message}");
                    MessageBox.Show("Не удалось обновить файлы с сервера. Игра будет запущена без обновлений.", "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Продолжаем без обновления файлов
                }
            }

            try
            {
                await minecraftLauncherService.LaunchMinecraftAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при запуске игры: {ex.Message}");
                MessageBox.Show($"Произошла ошибка при запуске игры: {ex.Message}", "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}