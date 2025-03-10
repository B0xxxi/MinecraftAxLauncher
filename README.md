# AxLauncher

AxLauncher - это лаунчер для Minecraft с поддержкой Forge, который автоматически загружает и обновляет файлы с SFTP-сервера.

## Особенности

- Автоматическое обновление файлов с SFTP-сервера
- Поддержка Forge для Minecraft
- Настройка объема оперативной памяти
- Сохранение пользовательских настроек
- Подробное логирование всех операций

## Структура проекта

- **Models** - модели данных (UserSettings)
- **ViewModels** - модели представлений (MVVM)
- **Views** - пользовательский интерфейс
- **Services** - сервисы для работы с SFTP и запуска Minecraft
- **Utilities** - вспомогательные классы (команды, логгер, конфигурация)

## Технологии

- .NET 8.0
- WPF (Windows Presentation Foundation)
- MVVM (Model-View-ViewModel)
- CmlLib.Core для работы с Minecraft
- SSH.NET для работы с SFTP

## Настройка

### Конфигурационный файл

При первом запуске приложение создаст конфигурационный файл `axlauncher_config.xml` в директории `%APPDATA%`. Вы можете использовать файл `axlauncher_config.example.xml` из репозитория как шаблон для настройки.

В конфигурационном файле необходимо указать следующие параметры:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ConfigData xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <!-- Учетные данные для SFTP подключения -->
  <SftpUsername>your_username</SftpUsername>
  <SftpPassword>your_password</SftpPassword>
  <SftpPort>22</SftpPort>
  <SftpRootPath>/your/remote/path</SftpRootPath>
  
  <!-- Версии Minecraft и Forge -->
  <MinecraftVersion>1.20.1</MinecraftVersion>
  <ForgeVersion>47.3.11</ForgeVersion>
  
  <!-- Локальная директория для игры (если не указано, будет использоваться %APPDATA%\.axcraft) -->
  <GameDirectory>C:\your\game\directory</GameDirectory>
  
  <!-- Предпочтительный объем памяти в МБ (по умолчанию 4096) -->
  <DefaultRamMB>4096</DefaultRamMB>
  
  <!-- IP-адреса серверов (для автоматического выбора) -->
  <PrimaryServerIp>xxx.xxx.xxx.xxx</PrimaryServerIp>
  <FallbackServerIp>yyy.yyy.yyy.yyy</FallbackServerIp>
</ConfigData>
```

### Minecraft и Forge

Проект по умолчанию настроен для работы со следующими версиями:
- Minecraft: 1.20.1
- Forge: 47.3.11

Для изменения этих версий отредактируйте конфигурационный файл.

## Улучшения в архитектуре

- Добавлены интерфейсы для сервисов, что улучшает тестируемость
- Централизованная конфигурация в AppConfig с поддержкой внешнего конфигурационного файла
- Логирование через специальный класс Logger
- Сохранение и загрузка пользовательских настроек
- Обработка ошибок на всех уровнях приложения


## Разработка и вклад в проект

1. Форкните репозиторий
2. Создайте ветку для своей функциональности (`git checkout -b feature/amazing-feature`)
3. Зафиксируйте изменения (`git commit -m 'Add some amazing feature'`)
4. Отправьте изменения в свой форк (`git push origin feature/amazing-feature`)
5. Создайте Pull Request

## Лицензия

Этот проект распространяется под лицензией MIT. Подробности смотрите в файле [LICENSE](LICENSE). 
