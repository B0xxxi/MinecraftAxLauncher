using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AxLauncher.Utilities;

namespace AxLauncher
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Инициализируем обработку необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            
            Logger.Info("Приложение запущено");
            
            // Загружаем конфигурацию
            AppConfig.LoadConfig();
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info("Приложение завершает работу");
            Logger.FlushLog();
            base.OnExit(e);
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Logger.Exception(exception, "Необработанное исключение в домене приложения");
            Logger.FlushLog();
            
            MessageBox.Show(
                $"Произошла непредвиденная ошибка: {exception?.Message}\n\nПриложение будет закрыто.",
                "Критическая ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Exception(e.Exception, "Необработанное исключение в UI потоке");
            Logger.FlushLog();
            
            MessageBox.Show(
                $"Произошла ошибка: {e.Exception.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                
            e.Handled = true;
        }
    }
}
