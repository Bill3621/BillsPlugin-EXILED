using System;
using System.IO;
using System.Net;
using BillsPlugin.Core.Handlers;
using Exiled.Loader;

namespace BillsPlugin;

public class Updater
{
    public static Version NewestVersion;
    public static string downloadUrl = "";
    public static bool UpdateAvailable;
    public static bool InstalledAutomatically;

    public static void CheckForUpdate()
    {
        if (!Plugin.Instance.Config.CheckForUpdates) return;

        if (UpdateAvailable)
        {
            PrintUpdateMessage();
            return;
        }

        var httpWebRequest =
            (HttpWebRequest)WebRequest.Create(
                "https://api.github.com/repos/Bill3621/BillsPlugin-EXILED/releases/latest");
        httpWebRequest.Method = "GET";
        httpWebRequest.Accept = "application/json";
        httpWebRequest.ContentType = "application/json; charset=utf-8;";
        httpWebRequest.UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";



        try
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseStream = httpResponse.GetResponseStream();
            if (responseStream == null)
            {
                Log.Error($"Response from GitHub is null. Response status: {httpResponse.StatusDescription}");
                return;
            }

            using var streamReader = new StreamReader(responseStream);

            var result = streamReader.ReadToEnd().Replace(" ", "");
            Log.Debug("Parsing github result...");
            NewestVersion = Version.Parse(Between(result, "tag_name\":\"", "\"").Replace("v", ""));

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

            // New version
            UpdateAvailable = true;
            PrintUpdateMessage();

            // Update

            downloadUrl = Between(result, "browser_download_url\":\"", "\"");

        }
        catch (Exception ex)
        {
            Log.Error("Error at checking for updates: " + ex.Message);
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
        using WebClient webClient = new();

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
                Log.Warn("Update will apply next server restart.");
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

    public static string Between(string str, string firstString, string lastString)
    {
        var pos1 = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;
        var pos2 = str.IndexOf(lastString, pos1, StringComparison.Ordinal);
        return str.Substring(pos1, pos2 - pos1);
    }
}