using EasyLog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Services
{
    public class AppConfig
    {
        public LogFormat LogFormat { get; set; } = LogFormat.Json;

        public List<string> ExtensionsToEncrypt { get; set; } = new List<string>();

        public string BusinessSoftware { get; set; } = string.Empty;

        public string CryptoSoftPath { get; set; } = "CryptoSoft.exe";
        public string CryptoKey { get; set; } = "EasySave2026";
    }

    public class ConfigManager
    {
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public AppConfig Config { get; private set; } = new AppConfig();

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public ConfigManager()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (!File.Exists(_configPath))
            {
                Config = new AppConfig();
                SaveConfig();
                return;
            }

            try
            {
                string json = File.ReadAllText(_configPath);
                Config = JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions) ?? new AppConfig();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[WARNING] Invalid JSON configuration ({ex.Message}). Using default values.");
                Config = new AppConfig();
                SaveConfig();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"[ERROR] Failed to read configuration ({ex.Message}). Using current memory configuration.");
            }
        }

        public void SaveConfig()
        {
            try
            {
                File.WriteAllText(_configPath, JsonSerializer.Serialize(Config, _jsonOptions));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"[ERROR] Failed to save configuration: {ex.Message}");
            }
        }
    }
}