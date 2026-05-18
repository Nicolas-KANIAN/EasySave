using EasyLog;
using System.Text.Json;
using System.Xml.Linq;

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
                    if (string.IsNullOrWhiteSpace(xml)) return null;

                    XDocument doc = XDocument.Parse(xml);

                    foreach (var element in doc.Descendants("StateEntry"))
                    {
                        string? name = element.Element("Name")?.Value;
                        string? state = element.Element("State")?.Value;

                        if (string.Equals(name, jobName, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(state, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                        {
                            string? progStr = element.Element("Progression")?.Value;
                            if (int.TryParse(progStr, out int progress))
                            {
                                return progress;
                            }
                        }
                    }
                    return null;
                }

                // Format JSON
                string json = ReadFileSafely(statePath);
                if (string.IsNullOrWhiteSpace(json)) return null;

                using JsonDocument document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement element in document.RootElement.EnumerateArray())
                    {
                        if (element.TryGetProperty("Name", out JsonElement name) &&
                            element.TryGetProperty("State", out JsonElement state) &&
                            element.TryGetProperty("Progression", out JsonElement progression))
                        {
                            string? stateJobName = name.GetString();
                            string? stateValue = state.GetString();

                            if (string.Equals(stateJobName, jobName, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(stateValue, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                            {
                                if (progression.TryGetInt32(out int progress))
                                {
                                    return progress;
                                }
                            }
                        }
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