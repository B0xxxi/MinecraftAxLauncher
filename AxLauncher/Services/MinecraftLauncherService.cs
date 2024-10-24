// Services/MinecraftLauncherService.cs
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AxLauncher.Models;

namespace AxLauncher.Services
{
    public class MinecraftLauncherService
    {
        private readonly MinecraftLauncher launcher;
        private readonly UserSettings userSettings;
        private const string mcVersion = "1.20.1";
        private const string forgeVersion = "47.3.11";

        public MinecraftLauncherService(UserSettings settings)
        {
            userSettings = settings;
            string minePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".axcraft");
            var path = new MinecraftPath(minePath);
            launcher = new MinecraftLauncher(path);
        }

        public async Task LaunchMinecraftAsync()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(x =>
                Console.WriteLine($"[{x.EventType}][{x.ProgressedTasks}/{x.TotalTasks}] {x.Name}"));
            var byteProgress = new SyncProgress<ByteProgress>(x =>
                Console.WriteLine(x.ToRatio() * 100 + "%"));
            var installerOutput = new SyncProgress<string>(x =>
                Console.WriteLine(x));

            var forge = new ForgeInstaller(launcher);

            var versionName = await forge.Install(mcVersion, forgeVersion, new ForgeInstallOptions
            {
                FileProgress = fileProgress,
                ByteProgress = byteProgress,
                InstallerOutput = installerOutput,
            });

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = userSettings.RAM,
                Session = MSession.CreateOfflineSession(userSettings.Login),
            };

            Process process = await launcher.InstallAndBuildProcessAsync(versionName, launchOption);

            var processUtil = new ProcessWrapper(process);
            processUtil.OutputReceived += (s, x) => Console.WriteLine(x);
            processUtil.StartWithEvents();
            await processUtil.WaitForExitTaskAsync();
        }
    }
}
