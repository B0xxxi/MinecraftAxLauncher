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

namespace AxLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // Инициализация окна
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
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var path = new MinecraftPath(); // Стандартная директория
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