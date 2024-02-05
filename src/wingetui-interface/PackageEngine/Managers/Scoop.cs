﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ABI.System.Collections.Generic;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Windows.Graphics.Display;

namespace ModernWindow.PackageEngine.Managers;

public class Scoop : PackageManagerWithSources
{
    new public static string[] FALSE_PACKAGE_NAMES = new string[] { "" };
    new public static string[] FALSE_PACKAGE_IDS = new string[] { "No" };
    new public static string[] FALSE_PACKAGE_VERSIONS = new string[] { "Matches" };
    protected override async Task<Package[]> FindPackages_UnSafe(string query)
    {
        var Packages = new List<Package>();

        string path = await bindings.Which("scoop-search.exe");
        if(!File.Exists(path))
            {
                Process proc = new Process() {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = path,
                        Arguments = Properties.ExecutableCallArgs + " install main/scoop-search",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                await proc.WaitForExitAsync();
                path = "scoop-search.exe";
            }

        Process p = new Process() {
            StartInfo = new ProcessStartInfo()
            {
                FileName = path,
                Arguments = query,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        p.Start();

        string line;
        ManagerSource source = GetMainSource();
        while((line = await p.StandardOutput.ReadLineAsync()) != null)
        {
            if(line.StartsWith("'"))
            {
                var sourceName = line.Split(" ")[0].Replace("'", "");
                if(SourceReference.ContainsKey(sourceName))
                    source = SourceReference[sourceName];
                else
                {
                    Console.WriteLine("Unknown source!");
                    source = new ManagerSource(this, sourceName, new Uri("https://scoop.sh/"), 0, "Unknown");
                    SourceReference.Add(sourceName, source);
                }
            }
            else if (line.Trim() != "")
            {
                var elements = line.Trim().Split(" ");
                if(elements.Length < 2)
                    continue;

                for (int i = 0; i < elements.Length; i++) elements[i] = elements[i].Trim();

                if (FALSE_PACKAGE_IDS.Contains(elements[0]) || FALSE_PACKAGE_VERSIONS.Contains(elements[1]))
                    continue;

                Packages.Add(new Package(bindings.FormatAsName(elements[0]), elements[0], elements[1].Replace("(", "").Replace(")", ""), source, this));
            }
        }
        return Packages.ToArray();
    }

    protected override async Task<UpgradablePackage[]> GetAvailableUpdates_UnSafe()
    {
        var InstalledPackages = new Dictionary<string, Package>();
        foreach(var InstalledPackage in await GetInstalledPackages())
        {
            if (!InstalledPackages.ContainsKey(InstalledPackage.Id + "." + InstalledPackage.Version))
                InstalledPackages.Add(InstalledPackage.Id + "." + InstalledPackage.Version, InstalledPackage);
        }

        var Packages = new List<UpgradablePackage>();

        Process p = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = Status.ExecutablePath,
                Arguments = Properties.ExecutableCallArgs + " status",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        p.Start();

        string line;
        bool DashesPassed = false;
        while ((line = await p.StandardOutput.ReadLineAsync()) != null)
        {
            if (!DashesPassed)
            {
                if(line.Contains("---"))
                    DashesPassed = true;
            }
            else if (line.Trim() != "")
            {
                var elements = Regex.Replace(line, " {2,}", " ").Trim().Split(" ");
                if (elements.Length < 3)
                    continue;

                for (int i = 0; i < elements.Length; i++) elements[i] = elements[i].Trim();

                if (FALSE_PACKAGE_IDS.Contains(elements[0]) || FALSE_PACKAGE_VERSIONS.Contains(elements[1]))
                    continue;

                if(!InstalledPackages.ContainsKey(elements[0] + "." + elements[1]))
                {
                    Console.WriteLine("Upgradable scoop package not listed on installed packages - id=" + elements[0]);
                    continue;
                }
                
                Packages.Add(new UpgradablePackage(bindings.FormatAsName(elements[0]), elements[0], elements[1], elements[2], InstalledPackages[elements[0] + "." + elements[1]].Source, this, InstalledPackages[elements[0] + "." + elements[1]].Scope));
            }
        }
        return Packages.ToArray();
    }

    protected override async Task<Package[]> GetInstalledPackages_UnSafe()
    {
        var Packages = new List<Package>();

        Process p = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = Status.ExecutablePath,
                Arguments = Properties.ExecutableCallArgs + " list",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        p.Start();

        string line;
        bool DashesPassed = false;
        while ((line = await p.StandardOutput.ReadLineAsync()) != null)
        {

            if (!DashesPassed)
            {
                if (line.Contains("---"))
                    DashesPassed = true;
            }
            else if (line.Trim() != "")
            {
                var elements = Regex.Replace(line, " {2,}", " ").Trim().Split(" ");
                if (elements.Length < 3)
                    continue;

                for (int i = 0; i < elements.Length; i++) elements[i] = elements[i].Trim();

                if (FALSE_PACKAGE_IDS.Contains(elements[0]) || FALSE_PACKAGE_VERSIONS.Contains(elements[1]))
                    continue;

                ManagerSource source;
                var sourceName = elements[2];
                if (SourceReference.ContainsKey(sourceName))
                    source = SourceReference[sourceName];
                else
                {
                    Console.WriteLine("Unknown source!");
                    source = new ManagerSource(this, sourceName, new Uri("https://scoop.sh/"), 0, "Unknown");
                    SourceReference.Add(sourceName, source);
                }

                var scope = PackageScope.User;
                if (line.Contains("Global install"))
                    scope = PackageScope.Global;

                Packages.Add(new Package(bindings.FormatAsName(elements[0]), elements[0], elements[1], source, this, scope));
            }
        }
        return Packages.ToArray();
    }


    public override ManagerSource GetMainSource()
    {
        return new ManagerSource(this, "main", new Uri("https://github.com/ScoopInstaller/Main"), 0, "");
    }

    public override Task<PackageDetails> GetPackageDetails_UnSafe(Package package)
    {
        throw new NotImplementedException();
    }

    protected override async Task<ManagerSource[]> GetSources_UnSafe()
    {
        Console.WriteLine("🔵 Starting " + Name + " source search...");
        using (Process process = new Process())
        {
            process.StartInfo.FileName = Status.ExecutablePath;
            process.StartInfo.Arguments = Properties.ExecutableCallArgs + " bucket list";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            List<string> output = new List<string>();
            List<ManagerSource> sources = new List<ManagerSource>();

            process.Start();

            string line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                try
                {
                    string[] elements = Regex.Replace(line.Trim(), " {2,}", " ").Split(' ');
                    if (elements.Length >= 5)
                        sources.Add(new ManagerSource(this, elements[0].Trim(), new Uri(elements[1].Trim()), int.Parse(elements[4].Trim()), elements[2].Trim() + " " + elements[3].Trim()));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            
            await process.WaitForExitAsync();
            Debug.WriteLine("🔵 " + Name + " source search finished.");
            return sources.ToArray();
        }
    }

    public override OperationVeredict GetUninstallOperationVeredict(Package package, InstallationOptions options, int ReturnCode, string[] Output)
    {
        var output_string = string.Join("\n", Output);
        if ((output_string.Contains("Try again with the --global (or -g) flag instead") && package.Scope == PackageScope.Local))
        {
            package.Scope = PackageScope.Global;
            return OperationVeredict.AutoRetry;
        }
        if (output_string.Contains("requires admin rights") || output_string.Contains("requires administrator rights") || output_string.Contains("you need admin rights to install global apps"))
        {
            options.RunAsAdministrator = true;
            return OperationVeredict.AutoRetry;
        }
        if (output_string.Contains("was uninstalled"))
            return OperationVeredict.Succeeded;
        return OperationVeredict.Failed;
    }
    public override OperationVeredict GetInstallOperationVeredict(Package package, InstallationOptions options, int ReturnCode, string[] Output)
    {
        var output_string = string.Join("\n", Output);
        if((output_string.Contains("Try again with the --global (or -g) flag instead") && package.Scope == PackageScope.Local))
        {
            package.Scope = PackageScope.Global;
            return OperationVeredict.AutoRetry;
        }
        if (output_string.Contains("requires admin rights") || output_string.Contains("requires administrator rights") || output_string.Contains("you need admin rights to install global apps"))
        {
            options.RunAsAdministrator = true;
            return OperationVeredict.AutoRetry;
        }
        if (output_string.Contains("Latest versions for all apps are installed") || output_string.Contains("is already installed") || output_string.Contains("was installed successfully"))
            return OperationVeredict.Succeeded;
        return OperationVeredict.Failed;
    }
    public override OperationVeredict GetUpdateOperationVeredict(Package package, InstallationOptions options, int ReturnCode, string[] Output)
    {
        return GetInstallOperationVeredict(package, options, ReturnCode, Output);
    }

    public override string[] GetUninstallParameters(Package package, InstallationOptions options)
    {
        List<string> parameters = new List<string>();

        parameters.Add(Properties.UninstallVerb);
        parameters.Add(package.Source.Name + "/" + package.Id);

        if (package.Scope == PackageScope.Global)
            parameters.Add("--global");

        if (options.CustomParameters != null)
            parameters.AddRange(options.CustomParameters);

        if (options.RemoveDataOnUninstall)
            parameters.Add("--purge");

        return parameters.ToArray();
    }
    public override string[] GetInstallParameters(Package package, InstallationOptions options)
    {
        var parameters = GetUpdateParameters(package, options);
        parameters[0] = Properties.InstallVerb;
        return parameters;
    }
    public override string[] GetUpdateParameters(Package package, InstallationOptions options)
    {
        var parameters = GetUninstallParameters(package, options).ToList();
        parameters[0] = Properties.UpdateVerb;

        parameters.Remove("--purge");

        switch(options.Architecture)
        {
            case null:
                break;
            case Architecture.X64:
                parameters.Add("--arch");
                parameters.Add("64bit");
                break;
            case Architecture.X86:
                parameters.Add("--arch");
                parameters.Add("32bit");
                break;
            case Architecture.Arm64:
                parameters.Add("--arch");
                parameters.Add("arm64");
                break;
        }

        if(options.SkipHashCheck)
        {
            parameters.Add("--skip");
        }

        return parameters.ToArray();
    }

    public override async Task RefreshSources()
    {
        Process process = new Process();
        ProcessStartInfo StartInfo = new ProcessStartInfo()
        {
            FileName = Properties.ExecutableFriendlyName,
            Arguments = Properties.ExecutableCallArgs + " update"
        };
        process.StartInfo = StartInfo;
        process.Start();
        await process.WaitForExitAsync();
    }

    protected override ManagerCapabilities GetCapabilities()
    {
        return new ManagerCapabilities()
        {
            CanRunAsAdmin = true,
            CanSkipIntegrityChecks = true,
            CanRemoveDataOnUninstall = true,
            SupportsCustomArchitectures = true,
            SupportsCustomScopes = true,
            SupportsCustomSources = true,
            Sources = new ManagerSource.Capabilities()
            {
                KnowsPackageCount = true,
                KnowsUpdateDate = true
            }
        };
    }

    protected override ManagerProperties GetProperties()
    {
        return new ManagerProperties()
        {
            Name = "Scoop",
            Description = bindings.Translate("Great repository of unknown but useful utilities and other interesting packages.<br>Contains: <b>Utilities, Command-line programs, General Software (extras bucket required)</b>"),
            IconId = "scoop",
            ColorIconId = "scoop_color",
            ExecutableCallArgs = "-NoProfile -ExecutionPolicy Bypass -Command scoop",
            ExecutableFriendlyName = "scoop",
            InstallVerb = "install",
            UpdateVerb = "update",
            UninstallVerb = "uninstall"
        };
    }

    protected override async Task<ManagerStatus> LoadManager()
    {
        var status = new ManagerStatus
        {
            ExecutablePath = Path.Join(Environment.SystemDirectory, "windowspowershell\\v1.0\\powershell.exe")
        };

        Process process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = status.ExecutablePath,
                Arguments = Properties.ExecutableCallArgs + " --version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        status.Version = (await process.StandardOutput.ReadToEndAsync()).Trim();
        status.Found = process.ExitCode == 0;
        

        if (status.Found && IsEnabled())
            _ = RefreshSources();

        return status;
    }
}