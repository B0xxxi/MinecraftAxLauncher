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
        private const string localFilePath = @"C:\Users\ivan0\AppData\Roaming\.axcraft";

        public MainWindow()
        {
            InitializeComponent(); // Инициализация окна
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
            string AxPath = @"C:\Users\ivan0\AppData\Roaming\.axcraft";
            if (System.IO.Directory.Exists(AxPath))
            {
                LogMessage("Directory already exists.");
            }
            else
            {
                System.IO.Directory.CreateDirectory(AxPath);
                LogMessage("Directory created.");
            }
        }

        private void CreateScriptFile()
        {
            string scriptFilePath = @"C:\Users\ivan0\AppData\Roaming\.axcraft\script.bat";
            if (!System.IO.File.Exists(scriptFilePath))
            {
                using (StreamWriter writer = new StreamWriter(scriptFilePath))
                {
                    writer.WriteLine("@echo off");
                    writer.WriteLine(":: Ваши скрипты идут здесь");
                    writer.WriteLine("pause");
                }
                LogMessage("Script file created.");
            }
            else
            {
                LogMessage("Script file already exists.");
            }
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e) // Login
        {
            string login = Login.Text; // Задача переменной login
            LogMessage($"Login changed: {login}");
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LogMessage($"Slider value changed: {e.NewValue}");
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
                        string localFilePath = System.IO.Path.Combine(@"C:\Users\ivan0\AppData\Roaming\.axcraft", file.Name);

                        using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                        {
                            // Асинхронная загрузка с BeginDownloadFile и EndDownloadFile
                            var asyncResult = sftp.BeginDownloadFile(remoteFilePath, fileStream, null, null);
                            await Task.Run(() => sftp.EndDownloadFile(asyncResult)); // Ожидание завершения
                        }

                        LogMessage($"Downloaded: {file.Name}");
                    }
                }

                sftp.Disconnect();
            }
        }

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
            LogMessage("Files downloaded from SFTP server.");
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreatePath(); // Добавь эту строку
                await CheckAndDownloadFile(); // Теперь можно использовать await
                MessageBox.Show("Files downloaded successfully!");
                LogMessage("File download completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                LogMessage($"Error: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            LogTextBox.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
            LogTextBox.ScrollToEnd();
        }
    }
}