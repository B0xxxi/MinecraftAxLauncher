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
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;

namespace AxLauncher
{
    public partial class MainWindow : Window

    {
        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            var path = new MinecraftPath(); // use default directory
            var launcher = new MinecraftLauncher(path);

            // show launch progress to console
            var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(e =>
                Console.WriteLine($"[{e.EventType}][{e.ProgressedTasks}/{e.TotalTasks}] {e.Name}"));
            var byteProgress = new SyncProgress<ByteProgress>(e =>
                Console.WriteLine(e.ToRatio() * 100 + "%"));
            var installerOutput = new SyncProgress<string>(e =>
                Console.WriteLine(e));

            //Initialize variables with the Minecraft version and the Forge version
            var mcVersion = "1.20.1";
            var forgeVersion = "47.2.32";

            //Initialize MForge
            var forge = new ForgeInstaller(launcher);

            var version_name = await forge.Install(mcVersion, forgeVersion, new ForgeInstallOptions
            {
                FileProgress = fileProgress,
                ByteProgress = byteProgress,
                InstallerOutput = installerOutput,
            });
            //var version_name = await forge.Install(mcVersion); // install the recommended forge version for mcVersion
            //OR var version_name = forge.Install(mcVersion, forgeVersion).GetAwaiter().GetResult();

            //Start Minecraft
            var launchOption = new MLaunchOption
            {
                MaximumRamMb = 1024,
                Session = MSession.CreateOfflineSession("Gamer123"),
            };

            var process = await launcher.InstallAndBuildProcessAsync(version_name, launchOption);

            // print game logs
            var processUtil = new ProcessWrapper(process);
            processUtil.OutputReceived += (s, e) => Console.WriteLine(e);
            processUtil.StartWithEvents();
            await processUtil.WaitForExitTaskAsync();
        }
    }
}