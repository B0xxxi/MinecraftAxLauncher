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
        public readonly UserSettings userSettings;
        public readonly SftpService sftpService;
        public readonly MinecraftLauncherService minecraftLauncherService;
        private bool filesChecked = false;

        public MainViewModel()
        {
            userSettings = new UserSettings();
            sftpService = new SftpService();
            minecraftLauncherService = new MinecraftLauncherService(userSettings);

            PlayCommand = new AsyncRelayCommand(async _ => await PlayAsync());
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

        private int ram = 4096;
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

        private double progressValue;
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

        public ICommand PlayCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task PlayAsync()
        {
            var overallProgress = new Progress<double>(value =>
            {
                ProgressValue = value;
            }) as IProgress<double>;

            if (!filesChecked)
            {
                try
                {
                    var sftpProgress = new System.Progress<double>(value =>
                    {
                        overallProgress.Report(value * 0.5);
                    });

                    await sftpService.DownloadAllFilesAsync(sftpProgress);
                    filesChecked = true;
                    Console.WriteLine("Files downloaded successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось загрузить файлы с SFTP-сервера: {ex.Message}");
                    MessageBox.Show("Не удалось обновить файлы с сервера. Игра будет запущена без обновлений.", "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            try
            {
                var launchProgress = new System.Progress<double>(value =>
                {
                    overallProgress.Report(50 + value * 0.5);
                });

                await minecraftLauncherService.LaunchMinecraftAsync(launchProgress);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
