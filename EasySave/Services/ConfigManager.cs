using EasyLog;
using System.Text.Json;

namespace EasySave.Services
{
    public class AppConfig
    {
        public LogFormat LogFormat { get; set; } = LogFormat.Json;
    }

    public class ConfigManager
    {
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public AppConfig Config { get; private set; }

        public ConfigManager()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    Config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                    return;
                }
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"[WARNING] Invalid or unreadable configuration ({ex.Message}). Using default values.");
            }

            Config = new AppConfig();
            SaveConfig();
        }

        public void SaveConfig()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(_configPath, JsonSerializer.Serialize(Config, options));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"[ERROR] Failed to save configuration: {ex.Message}");
            }
        }
    }
}