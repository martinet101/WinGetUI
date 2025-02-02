using System.Runtime.InteropServices;
using Microsoft.Management.Deployment;
using UniGetUI.Core.Logging;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine.Classes.Manager.BaseProviders;
using UniGetUI.PackageEngine.Enums;
using UniGetUI.PackageEngine.Interfaces;

namespace UniGetUI.PackageEngine.Managers.WingetManager;
internal sealed class WinGetPkgOperationHelper : PackagePkgOperationHelper
{
    public static string GetIdNamePiece(IPackage package)
    {
        if(!package.Id.EndsWith("…"))
            return $"--id \"{package.Id.TrimEnd('…')}\" --exact";

        if (!package.Name.EndsWith("…"))
            return $"--name \"{package.Name}\" --exact";

        return $"--id \"{package.Id.TrimEnd('…')}\"";
    }

    public WinGetPkgOperationHelper(WinGet manager) : base(manager) { }

    protected override IEnumerable<string> _getOperationParameters(IPackage package, IInstallationOptions options, OperationType operation)
    {
        List<string> parameters = [operation switch {
            OperationType.Install => Manager.Properties.InstallVerb,
            OperationType.Update => Manager.Properties.UpdateVerb,
            OperationType.Uninstall => Manager.Properties.UninstallVerb,
            _ => throw new InvalidDataException("Invalid package operation")
        }];

        parameters.AddRange(GetIdNamePiece(package).Split(" "));
        parameters.AddRange(["--source", package.Source.IsVirtualManager ? "winget" : package.Source.Name]);
        parameters.AddRange(["--accept-source-agreements", "--disable-interactivity"]);

        // package.OverridenInstallationOptions.Scope is meaningless in WinGet packages. Default is unspecified, hence the _ => [].
        parameters.AddRange((package.OverridenOptions.Scope ?? options.InstallationScope) switch {
            PackageScope.User => ["--scope", "user"],
            PackageScope.Machine => ["--scope", "machine"],
            _ => []
        });

        if (operation is OperationType.Uninstall && package.Version != "Unknown")
        {
            parameters.AddRange(["--version", $"\"{package.Version}\""]);
        }
        else if (operation is OperationType.Install && options.Version != "")
        {
            parameters.AddRange(["--version", $"\"{options.Version}\""]);
        }

        parameters.Add(options.InteractiveInstallation ? "--interactive" : "--silent");
        parameters.AddRange(options.CustomParameters);

        if(operation is OperationType.Update)
        {
            if (package.Name.Contains("64-bit") || package.Id.ToLower().Contains("x64"))
            {
                options.Architecture = Architecture.X64;
            }
            else if (package.Name.Contains("32-bit") || package.Id.ToLower().Contains("x86"))
            {
                options.Architecture = Architecture.X86;
            }
            parameters.Add("--include-unknown");
        }

        if(operation is not OperationType.Uninstall)
        {
            parameters.AddRange(["--accept-package-agreements", "--force"]);

            if (options.SkipHashCheck)
                parameters.Add("--ignore-security-hash");

            if (options.CustomInstallLocation != "")
                parameters.AddRange(["--location", $"\"{options.CustomInstallLocation}\""]);

            parameters.AddRange(options.Architecture switch
            {
                Architecture.X86 => ["--architecture", "x86"],
                Architecture.X64 => ["--architecture", "x64"],
                Architecture.Arm64 => ["--architecture", "arm64"],
                _ => []
            });
        }

        try
        {
            var installOptions = NativePackageHandler.GetInstallationOptions(package, operation);
            if (installOptions?.ElevationRequirement is ElevationRequirement.ElevationRequired
                or ElevationRequirement.ElevatesSelf)
            {
                package.OverridenOptions.RunAsAdministrator = true;
            }
            else if (installOptions?.ElevationRequirement is ElevationRequirement.ElevationProhibited)
            {
                if (CoreTools.IsAdministrator())
                    throw new UnauthorizedAccessException(
                        CoreTools.Translate("This package cannot be installed from an elevated context.")
                        + CoreTools.Translate("Please run UniGetUI as a regular user and try again."));

                if (options.RunAsAdministrator)
                    throw new UnauthorizedAccessException(
                        CoreTools.Translate("This package cannot be installed from an elevated context.")
                        + CoreTools.Translate("Please check the installation options for this package and try again"));
                package.OverridenOptions.RunAsAdministrator = false;
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Recovered from fatal WinGet exception:");
            Logger.Error(ex);
        }

        return parameters;
    }

    protected override OperationVeredict _getOperationResult(
        IPackage package,
        OperationType operation,
        IEnumerable<string> processOutput,
        int returnCode)
    {
        // See https://github.com/microsoft/winget-cli/blob/master/doc/windows/package-manager/winget/returnCodes.md for reference
        uint uintCode = (uint)returnCode;

        if (uintCode == 0x8A150109)
        {
            // If the user is required to restart the system to complete the installation
            if(operation is OperationType.Update) MarkUpgradeAsDone(package);
            //return OperationVeredict.RestartRequired;
            return OperationVeredict.Success;
        }

        if (uintCode == 0x8A150077 || uintCode == 0x8A15010C || uintCode == 0x8A150005)
        {
            return OperationVeredict.Canceled;
        }

        if (uintCode == 0x8A150011)
        {
            // TODO: Needs skip checksum
            return OperationVeredict.Failure;
        }

		if (uintCode == 0x8A15002B)
		{
			return OperationVeredict.Failure;
		}

        if (uintCode == 0x8A15010D || uintCode == 0x8A15004F || uintCode == 0x8A15010E)
        {
            // Application is already installed
            if(operation is OperationType.Update) MarkUpgradeAsDone(package);
            return OperationVeredict.Success;
        }

        if (returnCode == 0)
        {
            // Operation succeeded
            if(operation is OperationType.Update) MarkUpgradeAsDone(package);
            return OperationVeredict.Success;
        }

        if(uintCode == 0x8A150056 && package.OverridenOptions.RunAsAdministrator != false && !CoreTools.IsAdministrator())
        {
            // Installer can't run elevated
            package.OverridenOptions.RunAsAdministrator = false;
            return OperationVeredict.AutoRetry;
        }

        if ((uintCode is 0x8A150019 or 0x80073D28) && package.OverridenOptions.RunAsAdministrator != true)
        {
            // Code 0x80073D28 was added after https://github.com/marticliment/UniGetUI/issues/3093
            // Installer needs to run elevated
            package.OverridenOptions.RunAsAdministrator = true;
            return OperationVeredict.AutoRetry;
        }

        return OperationVeredict.Failure;
    }

    private static void MarkUpgradeAsDone(IPackage package)
    {
        Settings.SetDictionaryItem<string, string>("WinGetAlreadyUpgradedPackages", package.Id, package.NewVersion);
    }

    public static bool UpdateAlreadyInstalled(IPackage package)
    {
        return Settings.GetDictionaryItem<string, string>("WinGetAlreadyUpgradedPackages", package.Id) == package.NewVersion;
    }
}
