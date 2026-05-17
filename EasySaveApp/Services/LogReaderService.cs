using EasyLog;
using System.Text.Json;

namespace EasySaveApp.Services
{
    public class LogReaderService
    {
        public string ReadFileSafely(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(fileStream);
                return streamReader.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        public int? GetJobProgress(string statePath, string jobName, LogFormat format)
        {
            if (string.IsNullOrWhiteSpace(jobName) || !File.Exists(statePath)) return null;

            try
            {
                if (format == LogFormat.Xml)
                {
                    string xml = ReadFileSafely(statePath);
                    string startTag = "<Progression>";
                    string endTag = "</Progression>";
                    int startIndex = xml.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
                    int endIndex = xml.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        if (!xml.Contains("<Name>" + jobName + "</Name>", StringComparison.OrdinalIgnoreCase)) return null;
                        if (!xml.Contains("<State>ACTIVE</State>", StringComparison.OrdinalIgnoreCase)) return null;

                        startIndex += startTag.Length;
                        string value = xml.Substring(startIndex, endIndex - startIndex);
                        if (int.TryParse(value, out int progress)) return progress;
                    }
                    return null;
                }

                // Format JSON
                string json = ReadFileSafely(statePath);
                using JsonDocument document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind == JsonValueKind.Array &&
                    document.RootElement.GetArrayLength() > 0 &&
                    document.RootElement[0].TryGetProperty("Name", out JsonElement name) &&
                    document.RootElement[0].TryGetProperty("State", out JsonElement state) &&
                    document.RootElement[0].TryGetProperty("Progression", out JsonElement progression))
                {
                    string? stateJobName = name.GetString();
                    string? stateValue = state.GetString();

                    if (string.Equals(stateJobName, jobName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(stateValue, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                    {
                        return progression.GetInt32();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Failed to parse state file: {ex.Message}");
            }
            return null;
        }
    }
}