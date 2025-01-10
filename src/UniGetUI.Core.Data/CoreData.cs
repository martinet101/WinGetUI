﻿using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using UniGetUI.Core.Logging;

namespace UniGetUI.Core.Data
{
    public static class CoreData
    {
        private static int? __code_page;
        public static int CODE_PAGE { get => __code_page ??= GetCodePage(); }

        private static int GetCodePage()
        {
            try
            {
                Process p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "chcp.com",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                p.Start();
                string contents = p.StandardOutput.ReadToEnd();
                string purifiedString = "";

                foreach (var c in contents.Split(':')[^1].Trim())
                {
                    if (c >= '0' && c <= '9')
                    {
                        purifiedString += c;
                    }
                }

                return int.Parse(purifiedString);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return 0;
            }
        }

        public const string VersionName =  "3.1.6-beta1"; // Do not modify this line, use file scripts/apply_versions.py
        public const int BuildNumber =  75; // Do not modify this line, use file scripts/apply_versions.py

        public const string UserAgentString = $"UniGetUI/{VersionName} (https://marticliment.com/unigetui/; contact@marticliment.com)";

        public static HttpClientHandler GenericHttpClientParameters
        {
            get
            {
                return new()
                {
                    AutomaticDecompression = DecompressionMethods.All,
                    AllowAutoRedirect = true,
                };
            }
        }

        /// <summary>
        /// The directory where all the user data is stored. The directory is automatically created if it does not exist.
        /// </summary>
        public static string UniGetUIDataDirectory
        {
            get
            {
                string old_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".wingetui");
                string new_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UniGetUI");
                return GetNewDataDirectoryOrMoveOld(old_path, new_path);
            }
        }

        /// <summary>
        /// The directory where the installation options are stored. The directory is automatically created if it does not exist.
        /// </summary>
        public static string UniGetUIInstallationOptionsDirectory
        {
            get
            {
                string path = Path.Join(UniGetUIDataDirectory, "InstallationOptions");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        /// <summary>
        /// The directory where the metadata cache is stored. The directory is automatically created if it does not exist.
        /// </summary>
        public static string UniGetUICacheDirectory_Data
        {
            get
            {
                string old_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WingetUI", "CachedData");
                string new_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UniGetUI", "CachedMetadata");
                return GetNewDataDirectoryOrMoveOld(old_path, new_path);
            }
        }

        /// <summary>
        /// The directory where the cached icons and screenshots are saved. The directory is automatically created if it does not exist.
        /// </summary>
        public static string UniGetUICacheDirectory_Icons
        {
            get
            {
                string old_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WingetUI", "CachedIcons");
                string new_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UniGetUI", "CachedMedia");
                return GetNewDataDirectoryOrMoveOld(old_path, new_path);
            }
        }

        /// <summary>
        /// The directory where the cached language files are stored. The directory is automatically created if it does not exist.
        /// </summary>
        public static string UniGetUICacheDirectory_Lang
        {
            get
            {
                string old_dir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WingetUI", "CachedLangFiles");
                string new_dir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UniGetUI", "CachedLanguageFiles");
                return GetNewDataDirectoryOrMoveOld(old_dir, new_dir);
            }
        }

        /// <summary>
        /// The directory where package backups will be saved by default.
        /// </summary>
        public static string UniGetUI_DefaultBackupDirectory
        {
            get
            {
                string old_dir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WingetUI");
                string new_dir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UniGetUI");
                return GetNewDataDirectoryOrMoveOld(old_dir, new_dir);
            }
        }

        public static bool IsDaemon;

        /// <summary>
        /// The ID of the notification that is used to inform the user that updates are available
        /// </summary>
        public const int UpdatesAvailableNotificationTag = 1234;
        /// <summary>
        /// The ID of the notification that is used to inform the user that UniGetUI can be updated
        /// </summary>
        public const int UniGetUICanBeUpdated = 1235;
        /// <summary>
        /// The ID of the notification that is used to inform the user that shortcuts are available for deletion
        /// </summary>
        public const int NewShortcutsNotificationTag = 1236;

        /// <summary>
        /// A path pointing to the location where the app is installed
        /// </summary>
        public static string UniGetUIExecutableDirectory
        {
            get
            {
                string? dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (dir is not null)
                {
                    return dir;
                }

                Logger.Error("System.Reflection.Assembly.GetExecutingAssembly().Location returned an empty path");

                return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "UniGetUI");
            }
        }

        /// <summary>
        /// A path pointing to the executable file of the app
        /// </summary>
        public static string UniGetUIExecutableFile
        {
            get
            {
                string? filename = Process.GetCurrentProcess().MainModule?.FileName;
                if (filename is not null)
                {
                    return filename.Replace(".dll", ".exe");
                }

                Logger.Error("System.Reflection.Assembly.GetExecutingAssembly().Location returned an empty path");

                return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "UniGetUI", "UniGetUI.exe");
            }
        }

        public static string GSudoPath = "";

        /// <summary>
        /// This method will return the most appropriate data directory.
        /// If the new directory exists, it will be used.
        /// If the new directory does not exist, but the old directory does, it will be moved to the new location, and the new location will be used.
        /// If none exist, the new directory will be created.
        /// </summary>
        /// <param name="old_path">The old/legacy directory</param>
        /// <param name="new_path">The new directory</param>
        /// <returns>The path to an existing, valid directory</returns>
        private static string GetNewDataDirectoryOrMoveOld(string old_path, string new_path)
        {
            if (Directory.Exists(new_path) && !Directory.Exists(old_path))
            {
                return new_path;
            }

            if (Directory.Exists(new_path) && Directory.Exists(old_path))
            {
                try
                {
                    foreach (string old_subdir in Directory.GetDirectories(old_path, "*", SearchOption.AllDirectories))
                    {
                        string new_subdir = old_subdir.Replace(old_path, new_path);
                        if (!Directory.Exists(new_subdir))
                        {
                            Logger.Debug("New directory: " + new_subdir);
                            Directory.CreateDirectory(new_subdir);
                        }
                        else
                        {
                            Logger.Debug("Directory " + new_subdir + " already exists");
                        }
                    }

                    foreach (string old_file in Directory.GetFiles(old_path, "*", SearchOption.AllDirectories))
                    {
                        string new_file = old_file.Replace(old_path, new_path);
                        if (!File.Exists(new_file))
                        {
                            Logger.Info("Copying " + old_file);
                            File.Move(old_file, new_file);
                        }
                        else
                        {
                            Logger.Debug("File " + new_file + " already exists.");
                            File.Delete(old_file);
                        }
                    }

                    foreach (string old_subdir in Directory.GetDirectories(old_path, "*", SearchOption.AllDirectories).Reverse())
                    {
                        if (!Directory.EnumerateFiles(old_subdir).Any() && !Directory.EnumerateDirectories(old_subdir).Any())
                        {
                            Logger.Debug("Deleting old empty subdirectory " + old_subdir);
                            Directory.Delete(old_subdir);
                        }
                    }

                    if (!Directory.EnumerateFiles(old_path).Any() && !Directory.EnumerateDirectories(old_path).Any())
                    {
                        Logger.Info("Deleting old Chocolatey directory " + old_path);
                        Directory.Delete(old_path);
                    }

                    return new_path;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return new_path;
                }
            }

            if (/*Directory.Exists(new_path)*/Directory.Exists(old_path))
            {
                try
                {
                    Directory.Move(old_path, new_path);
                    Task.Delay(100).Wait();
                    return new_path;
                }
                catch (Exception e)
                {
                    Logger.Error("Cannot move old data directory to new location. Directory to move: " + old_path + ". Destination: " + new_path);
                    Logger.Error(e);
                    return old_path;
                }
            }

            try
            {
                Logger.Debug("Creating non-existing data directory at: " + new_path);
                Directory.CreateDirectory(new_path);
                return new_path;
            }
            catch (Exception e)
            {
                Logger.Error("Could not create new directory. You may perhaps need to disable Controlled Folder Access from Windows Settings or make an exception for UniGetUI.");
                Logger.Error(e);
                return new_path;
            }
        }

        public static JsonSerializerOptions SerializingOptions = new()
        {
            TypeInfoResolverChain = { new DefaultJsonTypeInfoResolver() },
            WriteIndented = true,
        };
    }
}
