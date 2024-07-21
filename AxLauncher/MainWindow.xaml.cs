using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Renci.SshNet;
using System.Threading;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using Renci.SshNet.Common;
using System.Net.Http;
using System.Activities.DurableInstancing;
using System.Workflow.Activities;
using System.Speech.Recognition.SrgsGrammar;

namespace AxLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // Инициализация окна
        }
        private async void InitializeAsync()
        {
            await DownloadAllFilesFromSftpAsync();
        }

        public async Task<string> GetPublicIPAsync()
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

        public async Task LocalOrGlobal()
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

        public string sftpHost;
        private const int sftpPort = 22; // Обычно SFTP использует порт 22
        private const string sftpUsername = "sftpuser";
        public const string sftpPassword = "Olezja";
        private const string sftpRootPath = "/test"; // Путь к корневой папке на сервере
        private const string localFilePath = @"C:\Users\grafs\AppData\Roaming\.axcraft\mods";

        public async Task DownloadAllFilesFromSftpAsync()
        {
            if (string.IsNullOrEmpty(sftpHost))
            {
                await LocalOrGlobal();
            }

            using (var sftp = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                sftp.Connect(); // Подключение к SFTP-серверу

                foreach (var file in sftp.ListDirectory(sftpRootPath)) // Создание экземпляра SftpClient с указанными хостом, портом, именем пользователя и паролем
                {
                    if (!file.IsDirectory) // Проверка, что текущий элемент не является директорией.
                    {
                        string remoteFilePath = file.FullName; // Полный путь к файлу на SFTP-сервере.
                        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // Получение пути к директории ApplicationData для текущего пользователя.
                        string modsPath = System.IO.Path.Combine(userProfile, ".axcraft", "mods");  // Создание полного пути к директории ".axcraft/mods" в директории ApplicationData.

                        // Создание директории, если она не существует
                        if (!Directory.Exists(modsPath))
                        {
                            Directory.CreateDirectory(modsPath);
                        }

                        string localFilePath = System.IO.Path.Combine(modsPath, file.Name); // Создание полного пути к локальному файлу, используя имя файла с SFTP-сервера.

                        using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write)) // Создание FileStream для записи файла на локальный диск.
                        {
                            var asyncResult = sftp.BeginDownloadFile(remoteFilePath, fileStream, null, null); // Начало асинхронного скачивания файла с SFTP-сервера в fileStream.
                            await Task.Run(() => sftp.EndDownloadFile(asyncResult)); // Ожидание завершения асинхронного скачивания файла.
                        }

                        Console.WriteLine($"Downloaded: {file.Name}"); // Вывод сообщения о завершении скачивания файла.
                    }
                }

                sftp.Disconnect(); // Отключение от SFTP-сервера.
            }
        }


        private async Task CheckAndDownloadFile()
        {
            // Асинхронное скачивание всех файлов
            await DownloadAllFilesFromSftpAsync();
            Console.WriteLine("Files downloaded from SFTP server.");
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
                await CheckAndDownloadFile(); // Теперь можно использовать await
                Console.WriteLine("Files downloaded successfully!");
                Console.WriteLine("File download completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string minePath = System.IO.Path.Combine(userProfile, ".axcraft");
            string modsPath = System.IO.Path.Combine(userProfile, ".axcraft", "mods");

            var path = new MinecraftPath(minePath); // Стандартная директория
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