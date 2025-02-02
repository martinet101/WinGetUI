using UniGetUI.Core.Data;
using UniGetUI.Core.Language;
using UniGetUI.Core.Logging;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine;
using UniGetUI.PackageEngine.Interfaces;

namespace UniGetUI.Interface.Telemetry;

public static class TelemetryHandler
{
    private const string HOST = "https://marticliment.com/unigetui/statistics";

    private static string[] SettingsToSend =
    {
        "DisableAutoUpdateWingetUI",
        "EnableUniGetUIBeta",
        "DisableSystemTray",
        "DisableNotifications",
        "DisableAutoCheckforUpdates",
        "AutomaticallyUpdatePackages",
        "AskToDeleteNewDesktopShortcuts",
        "EnablePackageBackup",
        "DoCacheAdminRights",
        "DoCacheAdminRightsForBatches",
        "ForceLegacyBundledWinGet",
        "UseSystemChocolatey",
        "SP1", // UniGetUI is portable
        "SP2" // UniGetUI was started as daemon
    };

    public static async void Initialize()
    {
        try
        {
            if (Settings.Get("DisableTelemetry")) return;
            await CoreTools.WaitForInternetConnection();

            if (Settings.GetValue("TelemetryClientToken").Length != 64)
            {
                Settings.SetValue("TelemetryClientToken", CoreTools.RandomString(64));
            }

            string ID = Settings.GetValue("TelemetryClientToken");

            int mask = 0x1;
            int ManagerMagicValue = 0;

            foreach (var manager in PEInterface.Managers)
            {
                if (manager.IsEnabled()) ManagerMagicValue |= mask;
                mask = mask << 1;
                if (manager.IsEnabled() && manager.Status.Found) ManagerMagicValue |= mask;
                mask = mask << 1;

                if (mask == 0x1)
                    throw new OverflowException();
            }

            int SettingsMagicValue = 0;
            mask = 0x1;
            foreach (var setting in SettingsToSend)
            {
                bool enabled;
                if (setting == "SP1") enabled = File.Exists("ForceUniGetUIPortable");
                else if (setting == "SP2") enabled = CoreData.WasDaemon;
                else if (setting.StartsWith("Disable")) enabled = !Settings.Get(setting);
                else enabled = Settings.Get(setting);

                if (enabled) SettingsMagicValue |= mask;
                mask = mask << 1;

                if (mask == 0x1)
                    throw new OverflowException();
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"{HOST}/activity");

            request.Headers.Add("clientId", ID);
            request.Headers.Add("clientVersion", CoreData.VersionName);
            request.Headers.Add("activeManagers", ManagerMagicValue.ToString());
            request.Headers.Add("activeSettings", SettingsMagicValue.ToString());
            request.Headers.Add("language", LanguageEngine.SelectedLocale);

            HttpClient _httpClient = new(CoreData.GenericHttpClientParameters);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Logger.Debug("[Telemetry] Call to /activity succeeded");
            }
            else
            {
                Logger.Warn($"[Telemetry] Call to /activity failed with error code {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error("[Telemetry] Hard crash when calling /activity");
            Logger.Error(ex);
        }
    }

    public static async void PackageInstalled(IPackage package)
    {
        try
        {
            if (Settings.Get("DisableTelemetry")) return;
            await CoreTools.WaitForInternetConnection();

            if (Settings.GetValue("TelemetryClientToken").Length != 64)
            {
                Settings.SetValue("TelemetryClientToken", CoreTools.RandomString(64));
            }

            string ID = Settings.GetValue("TelemetryClientToken");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{HOST}/install");

            request.Headers.Add("clientId", ID);
            request.Headers.Add("packageId", package.Id);
            request.Headers.Add("managerName", package.Manager.Name);
            request.Headers.Add("sourceName", package.Source.Name);

            HttpClient _httpClient = new(CoreData.GenericHttpClientParameters);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Logger.Debug("[Telemetry] Call to /install succeeded");
            }
            else
            {
                Logger.Warn($"[Telemetry] Call to /install failed with error code {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error("[Telemetry] Hard crash when calling /install");
            Logger.Error(ex);
        }
    }
}
