using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using Renci.SshNet;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using System.Net.Http;

namespace AxLauncher
{
    public partial class MainWindow : Window
    {
        private string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private bool filesChecked = false; // Добавим переменную для отслеживания состояния проверки файлов
        private const string sftpUsername = "sftpuser";
        private const string sftpPassword = "Olezja";
        private string sftpHost;
        private const int sftpPort = 22;
        private const string sftpRootPath = "/test";
        private readonly string[] foldersToDownload = { "mods", "kubejs", "packmenu" };

        public MainWindow()
        {
            InitializeComponent(); // Инициализация окна
        }

        private async Task<string> GetPublicIPAsync()
        {
            string url = "https://api.ipify.org?format=text";

            using (HttpClient client = new HttpClient())
            {
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
        }

        private async Task LocalOrGlobal()
        {
            string publicIP = await GetPublicIPAsync();
            if (publicIP == "136.169.223.12")
            {
                sftpHost = "192.168.1.69";
            }
            else
            {
                sftpHost = "136.169.223.12";
            }
        }
        private async Task DownloadAllFilesFromSftpAsync(string remotePath, string localPath)
        {
            if (string.IsNullOrEmpty(sftpHost))
            {
                await LocalOrGlobal();
            }

            string localRootPath = System.IO.Path.Combine(userProfile, ".axcraft");

            using (var sftp = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                sftp.Connect();

                var files = sftp.ListDirectory(remotePath);

                foreach (var file in files)
                {
                    string localFilePath = Path.Combine(localPath, file.Name);

                    if (file.IsDirectory)
                    {
                        if (file.Name != "." && file.Name != "..")
                        {
                            if (!Directory.Exists(localFilePath))
                            {
                                Directory.CreateDirectory(localFilePath);
                            }
                            await DownloadAllFilesFromSftpAsync(file.FullName, localFilePath);
                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                        {
                            var asyncResult = sftp.BeginDownloadFile(file.FullName, fileStream, null, null);
                            await Task.Run(() => sftp.EndDownloadFile(asyncResult));
                        }
                        Console.WriteLine($"Downloaded: {file.Name}");
                    }
                }

                sftp.Disconnect();
            }
        }

        private async Task CheckAndDownloadFile()
        {

            string localRootPath = System.IO.Path.Combine(userProfile, ".axcraft");

            if (!filesChecked)
            {
                await DownloadAllFilesFromSftpAsync(sftpRootPath, localRootPath);
                filesChecked = true;
                Console.WriteLine("Files downloaded from SFTP server.");
            }
        }

        void Border_MouseLeftButtonDown(object localSender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Перенос окна
        }

        void CloseButton_Click(object localSender, RoutedEventArgs e)
        {
            this.Close(); // Это закроет текущее окно
        }

        // Логин\Никнейм
        string login = ""; // Создаём переменную login
        void Login_TextChanged(object localSender, TextChangedEventArgs e)
        {
            login = Login.Text; // Задача переменной login слайдером
            if (!Regex.IsMatch(login, "^[a-zA-Z0-9]*$"))
            {
                MessageBox.Show("Логин может содержать только английские буквы и цифры.");
                Login.Text = Regex.Replace(login, "[^a-zA-Z0-9]", "");
            }
            Console.WriteLine($"Login changed: {login}");
        }

        // Пароль (Может вообще убрать его и сделать авторизацию прямо на сервере через мод? Надо обсудить)
        void Pass_TextChanged(object localSender, TextChangedEventArgs e)
        {

        }

        // Оператива
        int operdata = 4096; // Создаём переменную с оперативой (Пусть по умолчанию будет 4096)
        void Slider_ValueChanged(object localSender, RoutedPropertyChangedEventArgs<double> e)
        {
            operdata = (int)e.NewValue; // Задаем значение переменной operdata
            Console.WriteLine($"RAM: {operdata}"); // Выводим в консоль кол-во выбраной оперативки
        }

        // Кнопка играть (Запуск игры)
        public async void Play_Click(object localSender, RoutedEventArgs e)
        {
            try
            {
                await CheckAndDownloadFile();
                Console.WriteLine("Files downloaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            string minePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".axcraft");
            var path = new MinecraftPath(minePath);
            var launcher = new MinecraftLauncher(path);

            // Лог загрузки в консоль
            var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(x =>
                Console.WriteLine($"[{x.EventType}][{x.ProgressedTasks}/{x.TotalTasks}] {x.Name}"));
            var byteProgress = new SyncProgress<ByteProgress>(x =>
                Console.WriteLine(x.ToRatio() * 100 + "%"));
            var installerOutput = new SyncProgress<string>(x =>
                Console.WriteLine(x));

            //Задаём версию майна и форджа
            var mcVersion = "1.20.1";
            var forgeVersion = "47.3.0";

            //Инициализируем MForge
            var forge = new ForgeInstaller(launcher);

            var version_name = await forge.Install(mcVersion, forgeVersion, new ForgeInstallOptions
            {
                FileProgress = fileProgress,
                ByteProgress = byteProgress,
                InstallerOutput = installerOutput,
            });

            login = Login.Text;

            //Запуск майна
            var launchOption = new MLaunchOption
            {
                MaximumRamMb = operdata,
                Session = MSession.CreateOfflineSession(login),
            };

            Process process = await launcher.InstallAndBuildProcessAsync(version_name, launchOption);

            // Лооооооги
            var processUtil = new ProcessWrapper(process);
            processUtil.OutputReceived += (s, x) => Console.WriteLine(x);
            processUtil.StartWithEvents();
            await processUtil.WaitForExitTaskAsync();
        }
    }
}
