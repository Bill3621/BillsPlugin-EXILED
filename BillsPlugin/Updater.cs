using Exiled.API.Features;
using System;
using System.IO;
using System.Net;

namespace BillsPlugin
{
    public class Updater
    {
        public static string NewestVersion;
        public static bool UpdateAvailable;

        public static void CheckForUpdate()
        {

            if (!BillsPlugin.Instance.Config.CheckForUpdates)
            {
                return;
            }

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

                using (var streamReader = new StreamReader(responseStream))
                {
                    var result = streamReader.ReadToEnd().Replace(" ", "");
                    Log.Debug("Parsing github result...");
                    NewestVersion = Between(result, "tag_name\":\"", "\"");
                    if (NewestVersion.Equals($"v{BillsPlugin.Instance.Version}"))
                    {
                        Log.Debug("Plugin is up to date.");

                        return;
                    }

                    // New version
                    UpdateAvailable = true;
                    PrintUpdateMessage();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error at checking for updates: " + ex.Message);
            }
        }

        public static void PrintUpdateMessage()
        {
            Log.Warn($"New version available: {NewestVersion}");
            Log.Warn($"Current version: v{BillsPlugin.Instance.Version}");
            Log.Warn($"Download it from here: https://github.com/Bill3621/BillsPlugin-EXILED/releases/latest");
        }

        public static string Between(string str, string firstString, string lastString)
        {
            var pos1 = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;
            var pos2 = str.IndexOf(lastString, pos1, StringComparison.Ordinal);
            return str.Substring(pos1, pos2 - pos1);
        }
    }
}