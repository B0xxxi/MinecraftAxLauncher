using System;
using System.IO;
using System.Text;

namespace AxLauncher.Utilities
{
    /// <summary>
    /// Простой класс для логирования сообщений приложения
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFile = Path.Combine(AppConfig.ApplicationDataPath, "axlauncher.log");
        private static readonly StringBuilder LogBuffer = new StringBuilder();
        
        /// <summary>
        /// Логирует информационное сообщение
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void Info(string message)
        {
            Log("INFO", message);
        }
        
        /// <summary>
        /// Логирует предупреждение
        /// </summary>
        /// <param name="message">Текст предупреждения</param>
        public static void Warning(string message)
        {
            Log("WARNING", message);
        }
        
        /// <summary>
        /// Логирует ошибку
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        public static void Error(string message)
        {
            Log("ERROR", message);
        }
        
        /// <summary>
        /// Логирует исключение
        /// </summary>
        /// <param name="ex">Объект исключения</param>
        /// <param name="message">Дополнительное сообщение</param>
        public static void Exception(Exception ex, string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                Log("EXCEPTION", $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            else
            {
                Log("EXCEPTION", $"{message}\n{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// Записывает лог в буфер и консоль
        /// </summary>
        private static void Log(string level, string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Console.WriteLine(logEntry);
            LogBuffer.AppendLine(logEntry);
            
            // Сохраняем лог при достижении определенного размера буфера
            if (LogBuffer.Length > 4096)
            {
                FlushLog();
            }
        }
        
        /// <summary>
        /// Записывает буфер лога в файл
        /// </summary>
        public static void FlushLog()
        {
            try
            {
                File.AppendAllText(LogFile, LogBuffer.ToString());
                LogBuffer.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи лога: {ex.Message}");
            }
        }
    }
} 