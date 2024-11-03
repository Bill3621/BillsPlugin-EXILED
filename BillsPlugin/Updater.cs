using System;
using System.IO;
using System.Net.Http;
using BillsPlugin.Core.Handlers;
using Exiled.Loader;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using ServerOutput;

namespace BillsPlugin;

public class Updater
{
    public static Version NewestVersion;
    public static string downloadUrl = "";
    public static bool UpdateAvailable;
    public static bool InstalledAutomatically;

    public static async void CheckForUpdate()
    {
        if (!Plugin.Instance.Config.CheckForUpdates) return;

        if (UpdateAvailable)
        {
            PrintUpdateMessage();
            return;
        }

        var url = "https://api.github.com/repos/Bill3621/BillsPlugin-EXILED/releases/latest";

        using (HttpClient client = new())
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("updater");
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                var tagName = json["tag_name"].ToString();
                NewestVersion = Version.Parse(tagName.ToLower().Replace("v", ""));

                int compare = NewestVersion.CompareTo(Plugin.Instance.Version);
                Log.Debug($"Current version: {Plugin.Instance.Version}");
                Log.Debug($"Newest version: {NewestVersion}");
                Log.Debug($"Compare result: {compare}");
                if (compare == 0)
                {
                    Log.Debug("Plugin is up to date.");
                    return;
                }
                else if (compare < 0)
                {
                    Log.Debug("Plugin is development build.");
                    return;
                }

                UpdateAvailable = true;
                PrintUpdateMessage();

                downloadUrl = json["assets"][0]["browser_download_url"].ToString();
            }
            catch (Exception ex)
            {
                Log.Error("Error at checking for updates: ");
                Log.Error(ex);
            }
        }

        if (!UpdateAvailable) return;
        if (InstalledAutomatically) return;
        if (!Plugin.Instance.Config.AutoUpdate) return;

        string pluginPath = PathExtensions.GetPath(Plugin.Instance);

        try
        {
            Log.Debug($"Current plugin path: {pluginPath}");
            Log.Debug($"Latest release download: {downloadUrl}");

            DownloadAndReplaceFile(downloadUrl, pluginPath);
            Log.Info("Plugin successfully updated!");
            InstalledAutomatically = true;
            if (Plugin.Instance.Config.AutoRestart)
            {
                ServerEventHandlers.BroadcastStaff("BillsPlugin: An update has been installed. Applied next round.");
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                ServerConsole.AddOutputEntry(default(ExitActionRestartEntry));
                return;
            }
            ServerEventHandlers.BroadcastStaff("BillsPlugin: An update has been installed. Applied on next restart.");
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while trying to update the plugin: {ex.Message}");
            ServerEventHandlers.BroadcastStaff("BillsPlugin: Error at installing update.");
        }
    }

    private static void DownloadAndReplaceFile(string url, string filePath)
    {
        using System.Net.WebClient webClient = new();
        string tempFilePath = Path.GetTempFileName();

        webClient.DownloadFile(url, tempFilePath);

        File.Copy(tempFilePath, filePath, true);

        File.Delete(tempFilePath);
    }

    public static void PrintUpdateMessage()
    {
        if (Plugin.Instance.Config.AutoUpdate)
        {
            Log.Warn($"New version available: {NewestVersion}");
            Log.Warn($"Current version: v{Plugin.Instance.Version}");
            if (InstalledAutomatically)
            {
                if (Plugin.Instance.Config.AutoRestart)
                {
                    Log.Warn("Update will apply next round.");
                }
                else
                {

                    Log.Warn("Update will apply next server restart.");
                }
            }
            else
            {

                Log.Warn("Update will be automatically installed.");
            }
            return;
        }

        Log.Warn($"New version available: {NewestVersion}");
        Log.Warn($"Current version: v{Plugin.Instance.Version}");
        Log.Warn($"Download it here: {downloadUrl}");
    }
}