using System;
using System.IO;
using System.Xml.Serialization;

namespace AxLauncher.Utilities
{
    /// <summary>
    /// Класс, содержащий конфигурационные параметры приложения
    /// </summary>
    public static class AppConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "axlauncher_config.xml");
        
        // Параметры SFTP сервера
        public static string SftpUsername { get; private set; } = "anonymous";
        public static string SftpPassword { get; private set; } = "";
        public static int SftpPort { get; private set; } = 22;
        public static string SftpRootPath { get; private set; } = "/";
        
        // Параметры Minecraft
        public static string MinecraftVersion { get; private set; } = "1.20.1";
        public static string ForgeVersion { get; private set; } = "47.3.11";
        
        // Пути к директориям
        public static string ApplicationDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string GameDirectory { get; private set; } = Path.Combine(ApplicationDataPath, ".axcraft");
        
        // Значения по умолчанию
        public static int DefaultRamMB { get; private set; } = 4096;
        
        // Настройки подключения
        public static string PrimaryServerIp { get; private set; } = "";
        public static string FallbackServerIp { get; private set; } = "";

        /// <summary>
        /// Загружает конфигурацию из файла
        /// </summary>
        public static void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    var serializer = new XmlSerializer(typeof(ConfigData));
                    using var reader = new StreamReader(ConfigFilePath);
                    var config = (ConfigData)serializer.Deserialize(reader);
                    
                    // Применяем загруженные настройки
                    SftpUsername = config.SftpUsername;
                    SftpPassword = config.SftpPassword;
                    SftpPort = config.SftpPort;
                    SftpRootPath = config.SftpRootPath;
                    MinecraftVersion = config.MinecraftVersion;
                    ForgeVersion = config.ForgeVersion;
                    GameDirectory = config.GameDirectory ?? Path.Combine(ApplicationDataPath, ".axcraft");
                    DefaultRamMB = config.DefaultRamMB;
                    PrimaryServerIp = config.PrimaryServerIp;
                    FallbackServerIp = config.FallbackServerIp;
                    
                    Logger.Info("Конфигурация успешно загружена");
                }
                else
                {
                    CreateDefaultConfig();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при загрузке конфигурации");
            }
        }
        
        /// <summary>
        /// Создает файл конфигурации по умолчанию
        /// </summary>
        private static void CreateDefaultConfig()
        {
            try
            {
                var config = new ConfigData
                {
                    SftpUsername = SftpUsername,
                    SftpPassword = SftpPassword,
                    SftpPort = SftpPort,
                    SftpRootPath = SftpRootPath,
                    MinecraftVersion = MinecraftVersion,
                    ForgeVersion = ForgeVersion,
                    GameDirectory = GameDirectory,
                    DefaultRamMB = DefaultRamMB,
                    PrimaryServerIp = PrimaryServerIp,
                    FallbackServerIp = FallbackServerIp
                };
                
                var serializer = new XmlSerializer(typeof(ConfigData));
                using var writer = new StreamWriter(ConfigFilePath);
                serializer.Serialize(writer, config);
                
                Logger.Info("Создан файл конфигурации по умолчанию");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при создании файла конфигурации");
            }
        }
        
        /// <summary>
        /// Класс для сериализации/десериализации настроек
        /// </summary>
        [Serializable]
        public class ConfigData
        {
            public string SftpUsername { get; set; } = "anonymous";
            public string SftpPassword { get; set; } = "";
            public int SftpPort { get; set; } = 22;
            public string SftpRootPath { get; set; } = "/";
            public string MinecraftVersion { get; set; } = "1.20.1";
            public string ForgeVersion { get; set; } = "47.3.11";
            public string GameDirectory { get; set; }
            public int DefaultRamMB { get; set; } = 4096;
            public string PrimaryServerIp { get; set; } = "";
            public string FallbackServerIp { get; set; } = "";
        }
    }
} 