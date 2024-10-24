// Models/UserSettings.cs
using System;
using System.Text.RegularExpressions;

namespace AxLauncher.Models
{
    public class UserSettings
    {
        private string login = "";
        private int ram = 4096;

        public string Login
        {
            get => login;
            set
            {
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

        public int RAM
        {
            get => ram;
            set
            {
                if (value > 0)
                {
                    ram = value;
                }
                else
                {
                    throw new ArgumentException("Объем оперативной памяти должен быть больше 0.");
                }
            }
        }
    }
}
