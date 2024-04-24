﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using UniGetUI.Core;
using UniGetUI.Core.Data;

namespace UniGetUI.PackageEngine.Classes
{
    /// <summary>
    /// This class represents the options in which a package must be installed, updated or uninstalled.
    /// </summary>
    public class InstallationOptions
    {
        private ILogger AppLogger => Core.AppLogger.Instance;

        public bool SkipHashCheck { get; set; } = false;
        public bool InteractiveInstallation { get; set; } = false;
        public bool RunAsAdministrator { get; set; } = false;
        public string Version { get; set; } = "";
        public Architecture? Architecture { get; set; } = null;
        public PackageScope? InstallationScope { get; set; } = null;
        public List<string> CustomParameters { get; set; } = new List<string>();
        public bool RemoveDataOnUninstall { get; set; } = false;
        public bool PreRelease { get; set; } = false;
        public string CustomInstallLocation { get; set; } = "";

        public Package Package { get; }

        private string _saveFileName = "Unknown.Unknown.InstallationOptions";

        /// <summary>
        /// Construct a new InstallationOptions object for a given package. The options will be 
        /// loaded from disk unless the reset parameter is set to true, in which case the options
        /// will be the default ones.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="reset"></param>
        public InstallationOptions(Package package, bool reset = false)
        {
            Package = package;
            _saveFileName = Package.Manager.Name.Replace(" ", "").Replace(".", "") + "." + Package.Id;
            if (!reset)
            {
                LoadOptionsFromDisk();
            }
        }

        /// <summary>
        /// Returns a new InstallationOptions object from a given package. The options will be
        /// loaded from the disk asynchronously.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static async Task<InstallationOptions> FromPackageAsync(Package package)
        {
            var options = new InstallationOptions(package, reset: true);
            await options.LoadOptionsFromDiskAsync();
            return options;
        }

        /// <summary>
        /// Overload of the constructor that accepts an UpgradablePackage object.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="reset"></param>
        public InstallationOptions(UpgradablePackage package, bool reset = false) : this((Package)package, reset)
        { }

        /// <summary>
        /// Returns a new InstallationOptions object from a given SerializableInstallationOptions_v1 and a package.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        public static InstallationOptions FromSerialized(SerializableInstallationOptions_v1 options, Package package)
        {
            InstallationOptions opt = new(package, reset: true);
            opt.FromSerialized(options);
            return opt;
        }

        /// <summary>
        /// Loads and applies the options from the given SerializableInstallationOptions_v1 object to the current object.
        /// </summary>
        /// <param name="options"></param>
        public void FromSerialized(SerializableInstallationOptions_v1 options)
        {
            SkipHashCheck = options.SkipHashCheck;
            InteractiveInstallation = options.InteractiveInstallation;
            RunAsAdministrator = options.RunAsAdministrator;
            CustomInstallLocation = options.CustomInstallLocation;
            Version = options.Version;
            PreRelease = options.PreRelease;
            if (options.Architecture != "" && CommonTranslations.InvertedArchNames.ContainsKey(options.Architecture))
                Architecture = CommonTranslations.InvertedArchNames[options.Architecture];
            if (options.InstallationScope != "" && CommonTranslations.InvertedScopeNames_NonLang.ContainsKey(options.InstallationScope))
                InstallationScope = CommonTranslations.InvertedScopeNames_NonLang[options.InstallationScope];
            CustomParameters = options.CustomParameters;
        }

        /// <summary>
        /// Returns a SerializableInstallationOptions_v1 object containing the options of the current instance.
        /// </summary>
        /// <returns></returns>
        public SerializableInstallationOptions_v1 Serialized()
        {
            SerializableInstallationOptions_v1 options = new();
            options.SkipHashCheck = SkipHashCheck;
            options.InteractiveInstallation = InteractiveInstallation;
            options.RunAsAdministrator = RunAsAdministrator;
            options.CustomInstallLocation = CustomInstallLocation;
            options.PreRelease = PreRelease;
            options.Version = Version;
            if (Architecture != null)
                options.Architecture = CommonTranslations.ArchNames[Architecture.Value];
            if (InstallationScope != null)
                options.InstallationScope = CommonTranslations.ScopeNames_NonLang[InstallationScope.Value];
            options.CustomParameters = CustomParameters;
            return options;
        }

        private FileInfo GetPackageOptionsFile()
        {
            var optionsFileName = Package.Manager.Name + "." + Package.Id + ".json";
            return new FileInfo(Path.Join(CoreData.UniGetUIInstallationOptionsDirectory, optionsFileName));
        }

        /// <summary>
        /// Saves the current options to disk.
        /// </summary>
        public void SaveOptionsToDisk()
        {
            try
            {
                var optionsFile = GetPackageOptionsFile();
                if (optionsFile.Directory?.Exists == false)
                    optionsFile.Directory.Create();
                
                using var outputStream = optionsFile.OpenWrite();
                JsonSerializer.Serialize(outputStream, Serialized());
            }
            catch (Exception ex)
            {
                AppLogger.Log(ex);
            }
        }

        /// <summary>
        /// Saves the current options to disk, asynchronously.
        /// </summary>
        public async Task SaveOptionsToDiskAsync()
        {
            try
            {
                var optionsFile = GetPackageOptionsFile();
                if (optionsFile.Directory?.Exists == false)
                    optionsFile.Directory.Create();

                await using var outputStream = optionsFile.OpenWrite();
                await JsonSerializer.SerializeAsync(outputStream, Serialized());
            }
            catch (Exception ex)
            {
                AppLogger.Log(ex);
            }
        }

        /// <summary>
        /// Loads the options from disk, asynchronously.
        /// </summary>
        public void LoadOptionsFromDisk()
        {
            try
            {
                var optionsFile = GetPackageOptionsFile();
                if (!optionsFile.Exists)
                    return;

                using var inputStream = optionsFile.OpenRead();
                var options = JsonSerializer.Deserialize<SerializableInstallationOptions_v1>(inputStream);
                FromSerialized(options);
            }
            catch (Exception e)
            {
                AppLogger.Log(e);
            }
        }

        /// <summary>
        /// Loads the options from disk.
        /// </summary>
        public async Task LoadOptionsFromDiskAsync()
        {
            var optionsFile = GetPackageOptionsFile();
            try
            {
                if (!optionsFile.Exists)
                    return;
                

                await using var inputStream = optionsFile.OpenRead();
                var options = await JsonSerializer.DeserializeAsync<SerializableInstallationOptions_v1>(inputStream);
                FromSerialized(options);
            }
            catch (JsonException)
            {
                AppLogger.Log("An error occurred while parsing package " + optionsFile + ". The file will be overwritten");
                await File.WriteAllTextAsync(optionsFile.FullName, "{}");
            }
            catch (Exception e)
            {
                AppLogger.Log("Loading installation options for file " + optionsFile  + " have failed: ");
                AppLogger.Log(e);
            }
        }

        /// <summary>
        /// Returns a string representation of the current options.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string customparams = CustomParameters != null ? string.Join(",", CustomParameters) : "[]";
            return $"<InstallationOptions: SkipHashCheck={SkipHashCheck};" +
                   $"InteractiveInstallation={InteractiveInstallation};" +
                   $"RunAsAdministrator={RunAsAdministrator};" +
                   $"Version={Version};" +
                   $"Architecture={Architecture};" +
                   $"InstallationScope={InstallationScope};" +
                   $"InstallationScope={CustomInstallLocation};" +
                   $"CustomParameters={customparams};" +
                   $"RemoveDataOnUninstall={RemoveDataOnUninstall}>";
        }
    }
}
