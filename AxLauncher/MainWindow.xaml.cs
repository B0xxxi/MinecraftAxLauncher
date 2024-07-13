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
using MaterialDesignColors;
using MaterialDesignThemes;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Renci.SshNet;
using System.Threading;

namespace AxLauncher
{
    public partial class MainWindow : Window
    {
        private const string sftpHost = "192.168.1.69";
        private const int sftpPort = 22; // Обычно SFTP использует порт 22
        private const string sftpUsername = "sftpuser";
        private const string sftpPassword = "Olezja";
        private const string sftpRootPath = "/test"; // Путь к корневой папке на сервере
        private const string localFilePath = @"C:\Users\grafs\AppData\Roaming\.axcraft";

        public MainWindow()
        {
            InitializeComponent(); // Инициализация окна
            CreateScriptFile();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Перенос окна
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Это закроет текущее окно
        }

        private void CreatePath()
        {
            string AxPath = @"C:\Users\grafs\AppData\Roaming\.axcraft";
            if (System.IO.Directory.Exists(AxPath))
            {
                Console.WriteLine("Directory already exists.");
            }
            else
            {
                System.IO.Directory.CreateDirectory(AxPath);
                Console.WriteLine("Directory created.");
            }
        }

        //Создание бат файла
        private void CreateScriptFile()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //Поиск папки User
            string scriptFilePath = System.IO.Path.Combine(userProfile, ".axcraft", "start.bat"); //Объединение строк в путь до bat файла
            if (!System.IO.File.Exists(scriptFilePath))
            {
                //Создание файла и запись критериев
                using (StreamWriter sw = File.CreateText(scriptFilePath))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("java -Dos.name=Windows10 -Dos.version=10.0 -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xss1M -Djava.library.path=\\natives -Djna.tmpdir=.\\versions\\Forge-1.20.4\\natives -Dorg.lwjgl.system.SharedLibraryExtractPath=.\\versions\\Forge-1.20.4\\natives -Dio.netty.native.workdir=.\\versions\\Forge-1.20.4\\natives -Dminecraft.launcher.brand=minecraft-launcher -Dminecraft.launcher.version=2.3.173 -cp .\\libraries\\net\\minecraftforge\\forge\\1.20.4-49.0.38\\forge-1.20.4-49.0.38-universal.jar;.\\libraries\\net\\minecraftforge\\forge\\1.20.4-49.0.38\\forge-1.20.4-49.0.38-client.jar;.\\libraries\\net\\minecraftforge\\JarJarFileSystems\\0.3.26\\JarJarFileSystems-0.3.26.jar;.\\libraries\\net\\minecraftforge\\securemodules\\2.2.10\\securemodules-2.2.10.jar;.\\libraries\\net\\minecraftforge\\unsafe\\0.9.2\\unsafe-0.9.2.jar;.\\libraries\\org\\ow2\\asm\\asm\\9.6\\asm-9.6.jar;.\\libraries\\org\\ow2\\asm\\asm-tree\\9.6\\asm-tree-9.6.jar;.\\libraries\\org\\ow2\\asm\\asm-util\\9.6\\asm-util-9.6.jar;.\\libraries\\org\\ow2\\asm\\asm-commons\\9.6\\asm-commons-9.6.jar;.\\libraries\\org\\ow2\\asm\\asm-analysis\\9.6\\asm-analysis-9.6.jar;.\\libraries\\net\\minecraftforge\\bootstrap\\2.1.0\\bootstrap-2.1.0.jar;.\\libraries\\net\\minecraftforge\\bootstrap-api\\2.1.0\\bootstrap-api-2.1.0.jar;.\\libraries\\net\\minecraftforge\\accesstransformers\\8.1.1\\accesstransformers-8.1.1.jar;.\\libraries\\org\\antlr\\antlr4-runtime\\4.9.1\\antlr4-runtime-4.9.1.jar;.\\libraries\\net\\minecraftforge\\eventbus\\6.2.0\\eventbus-6.2.0.jar;.\\libraries\\net\\jodah\\typetools\\0.6.3\\typetools-0.6.3.jar;.\\libraries\\net\\minecraftforge\\forgespi\\7.1.3\\forgespi-7.1.3.jar;.\\libraries\\net\\minecraftforge\\coremods\\5.1.0\\coremods-5.1.0.jar;.\\libraries\\org\\openjdk\\nashorn\\nashorn-core\\15.3\\nashorn-core-15.3.jar;.\\libraries\\net\\minecraftforge\\modlauncher\\10.1.2\\modlauncher-10.1.2.jar;.\\libraries\\net\\minecraftforge\\mergetool-api\\1.0\\mergetool-api-1.0.jar;.\\libraries\\com\\electronwill\\night-config\\toml\\3.6.4\\toml-3.6.4.jar;.\\libraries\\com\\electronwill\\night-config\\core\\3.6.4\\core-3.6.4.jar;.\\libraries\\org\\apache\\maven\\maven-artifact\\3.8.5\\maven-artifact-3.8.5.jar;.\\libraries\\net\\minecrell\\terminalconsoleappender\\1.2.0\\terminalconsoleappender-1.2.0.jar;.\\libraries\\org\\jline\\jline-reader\\3.12.1\\jline-reader-3.12.1.jar;.\\libraries\\org\\jline\\jline-terminal\\3.12.1\\jline-terminal-3.12.1.jar;.\\libraries\\org\\jline\\jline-terminal-jna\\3.12.1\\jline-terminal-jna-3.12.1.jar;.\\libraries\\org\\spongepowered\\mixin\\0.8.5\\mixin-0.8.5.jar;.\\libraries\\net\\minecraftforge\\JarJarSelector\\0.3.26\\JarJarSelector-0.3.26.jar;.\\libraries\\net\\minecraftforge\\JarJarMetadata\\0.3.26\\JarJarMetadata-0.3.26.jar;.\\libraries\\net\\minecraftforge\\fmlcore\\1.20.4-49.0.38\\fmlcore-1.20.4-49.0.38.jar;.\\libraries\\net\\minecraftforge\\fmlloader\\1.20.4-49.0.38\\fmlloader-1.20.4-49.0.38.jar;.\\libraries\\net\\minecraftforge\\fmlearlydisplay\\1.20.4-49.0.38\\fmlearlydisplay-1.20.4-49.0.38.jar;.\\libraries\\net\\minecraftforge\\javafmllanguage\\1.20.4-49.0.38\\javafmllanguage-1.20.4-49.0.38.jar;.\\libraries\\net\\minecraftforge\\lowcodelanguage\\1.20.4-49.0.38\\lowcodelanguage-1.20.4-49.0.38.jar;.\\libraries\\net\\minecraftforge\\mclanguage\\1.20.4-49.0.38\\mclanguage-1.20.4-49.0.38.jar;.\\libraries\\com\\github\\oshi\\oshi-core\\6.4.5\\oshi-core-6.4.5.jar;.\\libraries\\com\\google\\code\\gson\\gson\\2.10.1\\gson-2.10.1.jar;.\\libraries\\com\\google\\guava\\failureaccess\\1.0.1\\failureaccess-1.0.1.jar;.\\libraries\\com\\google\\guava\\guava\\32.1.2-jre\\guava-32.1.2-jre.jar;.\\libraries\\com\\ibm\\icu\\icu4j\\73.2\\icu4j-73.2.jar;.\\libraries\\org\\tlauncher\\authlib\\6.0.522\\authlib-6.0.522.jar;.\\libraries\\com\\mojang\\blocklist\\1.0.10\\blocklist-1.0.10.jar;.\\libraries\\com\\mojang\\brigadier\\1.2.9\\brigadier-1.2.9.jar;.\\libraries\\com\\mojang\\datafixerupper\\6.0.8\\datafixerupper-6.0.8.jar;.\\libraries\\com\\mojang\\logging\\1.1.1\\logging-1.1.1.jar;.\\libraries\\org\\tlauncher\\patchy\\2.2.10\\patchy-2.2.10.jar;.\\libraries\\com\\mojang\\text2speech\\1.17.9\\text2speech-1.17.9.jar;.\\libraries\\commons-codec\\commons-codec\\1.16.0\\commons-codec-1.16.0.jar;.\\libraries\\commons-io\\commons-io\\2.13.0\\commons-io-2.13.0.jar;.\\libraries\\commons-logging\\commons-logging\\1.2\\commons-logging-1.2.jar;.\\libraries\\io\\netty\\netty-buffer\\4.1.97.Final\\netty-buffer-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-codec\\4.1.97.Final\\netty-codec-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-common\\4.1.97.Final\\netty-common-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-handler\\4.1.97.Final\\netty-handler-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-resolver\\4.1.97.Final\\netty-resolver-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-transport-classes-epoll\\4.1.97.Final\\netty-transport-classes-epoll-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-transport-native-unix-common\\4.1.97.Final\\netty-transport-native-unix-common-4.1.97.Final.jar;.\\libraries\\io\\netty\\netty-transport\\4.1.97.Final\\netty-transport-4.1.97.Final.jar;.\\libraries\\it\\unimi\\dsi\\fastutil\\8.5.12\\fastutil-8.5.12.jar;.\\libraries\\net\\java\\dev\\jna\\jna-platform\\5.13.0\\jna-platform-5.13.0.jar;.\\libraries\\net\\java\\dev\\jna\\jna\\5.13.0\\jna-5.13.0.jar;.\\libraries\\net\\sf\\jopt-simple\\jopt-simple\\5.0.4\\jopt-simple-5.0.4.jar;.\\libraries\\org\\apache\\commons\\commons-compress\\1.22\\commons-compress-1.22.jar;.\\libraries\\org\\apache\\commons\\commons-lang3\\3.13.0\\commons-lang3-3.13.0.jar;.\\libraries\\org\\apache\\httpcomponents\\httpclient\\4.5.13\\httpclient-4.5.13.jar;.\\libraries\\org\\apache\\httpcomponents\\httpcore\\4.4.16\\httpcore-4.4.16.jar;.\\libraries\\org\\apache\\logging\\log4j\\log4j-api\\2.19.0\\log4j-api-2.19.0.jar;.\\libraries\\org\\apache\\logging\\log4j\\log4j-core\\2.19.0\\log4j-core-2.19.0.jar;.\\libraries\\org\\apache\\logging\\log4j\\log4j-slf4j2-impl\\2.19.0\\log4j-slf4j2-impl-2.19.0.jar;.\\libraries\\org\\joml\\joml\\1.10.5\\joml-1.10.5.jar;.\\libraries\\org\\lwjgl\\lwjgl-glfw\\3.3.2\\lwjgl-glfw-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl-glfw\\3.3.2\\lwjgl-glfw-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl-glfw\\3.3.2\\lwjgl-glfw-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl-glfw\\3.3.2\\lwjgl-glfw-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\lwjgl\\lwjgl-jemalloc\\3.3.2\\lwjgl-jemalloc-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl-jemalloc\\3.3.2\\lwjgl-jemalloc-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl-jemalloc\\3.3.2\\lwjgl-jemalloc-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl-jemalloc\\3.3.2\\lwjgl-jemalloc-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\lwjgl\\lwjgl-openal\\3.3.2\\lwjgl-openal-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl-openal\\3.3.2\\lwjgl-openal-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl-openal\\3.3.2\\lwjgl-openal-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl-openal\\3.3.2\\lwjgl-openal-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\lwjgl\\lwjgl-opengl\\3.3.2\\lwjgl-opengl-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl-opengl\\3.3.2\\lwjgl-opengl-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl-opengl\\3.3.2\\lwjgl-opengl-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl-opengl\\3.3.2\\lwjgl-opengl-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\lwjgl\\lwjgl-stb\\3.3.2\\lwjgl-stb-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl-stb\\3.3.2\\lwjgl-stb-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl-stb\\3.3.2\\lwjgl-stb-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl-stb\\3.3.2\\lwjgl-stb-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\lwjgl\\lwjgl-tinyfd\\3.3.2\\lwjgl-tinyfd-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl-tinyfd\\3.3.2\\lwjgl-tinyfd-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl-tinyfd\\3.3.2\\lwjgl-tinyfd-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl-tinyfd\\3.3.2\\lwjgl-tinyfd-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\lwjgl\\lwjgl\\3.3.2\\lwjgl-3.3.2.jar;.\\libraries\\org\\lwjgl\\lwjgl\\3.3.2\\lwjgl-3.3.2-natives-windows.jar;.\\libraries\\org\\lwjgl\\lwjgl\\3.3.2\\lwjgl-3.3.2-natives-windows-arm64.jar;.\\libraries\\org\\lwjgl\\lwjgl\\3.3.2\\lwjgl-3.3.2-natives-windows-x86.jar;.\\libraries\\org\\slf4j\\slf4j-api\\2.0.7\\slf4j-api-2.0.7.jar;.\\versions\\Forge-1.20.4\\Forge-1.20.4.jar -Djava.net.preferIPv6Addresses=system -Xmx13308M -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Djava.net.preferIPv4Stack=true -Dminecraft.applet.TargetDirectory=. -DlibraryDirectory=.\\libraries -Dlog4j.configurationFile=.\\assets\\log_configs\\client-1.12.xml net.minecraftforge.bootstrap.ForgeBootstrap --username B0xxxxi --version Forge 1.20.4 --gameDir . --assetsDir .\\assets --assetIndex 12 --uuid 1bf16c534b8343fb92ee8c3b8b544532 --accessToken null --clientId null --xuid null --userType mojang --versionType modified --width 1920 --height 1080 --launchTarget forge_client");
                    sw.WriteLine("pause");
                }
                Console.WriteLine("Script file created.");
                System.IO.File.Move(@"C:\Users\grafs\AppData\Roaming\.axcraft\start.txt", @"C:\Users\grafs\AppData\Roaming\.axcraft\start.bat");
            }
            else
            {
                Console.WriteLine("Script file already exists.");
            }
        }

        // Login
        private void Login_TextChanged(object sender, TextChangedEventArgs e) 
        {
            string login = Login.Text; // Задача переменной login
            Console.WriteLine($"Login changed: {login}");
        }

        // Password
        private void Pass_TextChanged(object sender, TextChangedEventArgs e)
        {
            string login = Login.Text; // Задача переменной login
            Console.WriteLine($"Login changed: {login}");
        }

        //Выбор оперативы
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Console.WriteLine($"Slider value changed: {e.NewValue}");
            int operdata = (int)e.NewValue;
            Console.WriteLine(operdata);
        }


        // Асинхронное скачивание всех файлов из директории
        private async Task DownloadAllFilesFromSftpAsync()
        {
            using (var sftp = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword))
            {
                await sftp.ConnectAsync(CancellationToken.None);

                foreach (var file in sftp.ListDirectory(sftpRootPath))
                {
                    if (!file.IsDirectory)
                    {
                        string remoteFilePath = file.FullName;
                        string localFilePath = System.IO.Path.Combine(@"C:\Users\grafs\AppData\Roaming\.axcraft", file.Name);

                        using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                        {
                            // Асинхронная загрузка с BeginDownloadFile и EndDownloadFile
                            var asyncResult = sftp.BeginDownloadFile(remoteFilePath, fileStream, null, null);
                            await Task.Run(() => sftp.EndDownloadFile(asyncResult)); // Ожидание завершения
                        }

                        Console.WriteLine($"Downloaded: {file.Name}");
                    }
                }

                sftp.Disconnect();
            }
        }

        //Проверка наличия файлов у клиента
        private bool CompareFiles(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);

            if (file1.Length != file2.Length)
                return false;

            for (int i = 0; i < file1.Length; i++)
            {
                if (file1[i] != file2[i])
                    return false;
            }

            return true;
        }

        private async Task CheckAndDownloadFile()
        {
            // Асинхронное скачивание всех файлов
            await DownloadAllFilesFromSftpAsync();
            Console.WriteLine("Files downloaded from SFTP server.");
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreatePath(); // Добавь эту строку
                await CheckAndDownloadFile(); // Теперь можно использовать await
                Console.WriteLine("Files downloaded successfully!");
                Console.WriteLine("File download completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            /*
            LogTextBox.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
            LogTextBox.ScrollToEnd();
            */
        }
    }
}