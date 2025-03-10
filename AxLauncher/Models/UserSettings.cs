// Models/UserSettings.cs
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using AxLauncher.Utilities;

namespace AxLauncher.Models
{
    /// <summary>
    /// Класс, представляющий пользовательские настройки приложения
    /// </summary>
    public class UserSettings
    {
        private static readonly string SettingsPath = Path.Combine(AppConfig.ApplicationDataPath, "axlauncher_settings.xml");
        private string login = "";
        private int ram = AppConfig.DefaultRamMB;

        /// <summary>
        /// Логин пользователя (только английские буквы и цифры)
        /// </summary>
        public string Login
        {
            get => login;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    login = "";
                    return;
                }
                
                if (Regex.IsMatch(value, "^[a-zA-Z0-9]*$"))
                {
                    login = value;
                }
                else
                {
                    throw new ArgumentException("Логин может содержать только английские буквы и цифры.");
                }
            }
        }

        /// <summary>
        /// Объем оперативной памяти для игры (в МБ)
        /// </summary>
        public int RAM
        {
            get => ram;
            set
            {
                if (value >= 1024 && value <= 16384)
                {
                    ram = value;
                }
                else
                {
                    throw new ArgumentException("Объем оперативной памяти должен быть от 1024 МБ до 16384 МБ (16 ГБ).");
                }
            }
        }

        /// <summary>
        /// Загружает настройки из файла
        /// </summary>
        /// <returns>Загруженные настройки или новый экземпляр, если файл не существует</returns>
        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var serializer = new XmlSerializer(typeof(UserSettings));
                    using var reader = new StreamReader(SettingsPath);
                    var settings = (UserSettings)serializer.Deserialize(reader);
                    Logger.Info("Настройки успешно загружены");
                    return settings;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при загрузке настроек");
            }
            
            Logger.Info("Создаем новые настройки по умолчанию");
            return new UserSettings();
        }

        /// <summary>
        /// Сохраняет настройки в файл
        /// </summary>
        public void Save()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(UserSettings));
                using var writer = new StreamWriter(SettingsPath);
                serializer.Serialize(writer, this);
                Logger.Info("Настройки успешно сохранены");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Ошибка при сохранении настроек");
            }
        }
    }
}
