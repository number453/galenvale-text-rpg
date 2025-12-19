using System;
using System.IO;
using System.Text.Json;

namespace Galenvale
{
    public static class SaveSystem
    {
        private const string FileName = "save.json";

        // Save next to the executable by default (works great for console apps)
        private static string SavePath => Path.Combine(AppContext.BaseDirectory, FileName);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static bool SaveExists() => File.Exists(SavePath);

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }

        public static void Write(SaveData data)
        {
            data.SavedAtUtc = DateTime.UtcNow;
            string json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(SavePath, json);
        }

        public static bool TryLoad(out SaveData? data)
        {
            data = null;

            if (!File.Exists(SavePath))
                return false;

            try
            {
                string json = File.ReadAllText(SavePath);
                data = JsonSerializer.Deserialize<SaveData>(json, JsonOptions);
                return data != null;
            }
            catch
            {
                // If save is corrupted, treat as no save (you can also auto-delete here if you want)
                data = null;
                return false;
            }
        }
    }
}

