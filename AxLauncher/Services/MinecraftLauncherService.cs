﻿// Services/MinecraftLauncherService.cs
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
        private const string forgeVersion = "47.3.0";

        public MinecraftLauncherService(UserSettings settings)
        {
            userSettings = settings;
            string minePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".axcraft");
            var path = new MinecraftPath(minePath);
            launcher = new MinecraftLauncher(path);
        }

        public async Task LaunchMinecraftAsync(IProgress<double> progress)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(x =>
            {
                double percentage = ((double)x.ProgressedTasks / x.TotalTasks) * 100 * 0.5;
                progress?.Report(50 + percentage * 0.5);
                Console.WriteLine($"[{x.EventType}][{x.ProgressedTasks}/{x.TotalTasks}] {x.Name}");
            });
            var byteProgress = new SyncProgress<ByteProgress>(x =>
            {
                Console.WriteLine(x.ToRatio() * 100 + "%");
            });
            var installerOutput = new SyncProgress<string>(x =>
                Console.WriteLine(x));

            var forge = new ForgeInstaller(launcher);

            var versionName = await forge.Install(mcVersion, forgeVersion, new ForgeInstallOptions
            {
                FileProgress = fileProgress,
                ByteProgress = byteProgress,
                InstallerOutput = installerOutput,
            });

            progress?.Report(75);

            var launchOption = new MLaunchOption
            {
                MaximumRamMb = userSettings.RAM,
                Session = MSession.CreateOfflineSession(userSettings.Login),
            };

            Process process = await launcher.CreateProcessAsync(versionName, launchOption);

            progress?.Report(85);

            var processUtil = new ProcessWrapper(process);
            processUtil.OutputReceived += (s, x) => Console.WriteLine(x);
            processUtil.StartWithEvents();
            progress?.Report(100);

            await processUtil.WaitForExitTaskAsync();
        }
    }
}

