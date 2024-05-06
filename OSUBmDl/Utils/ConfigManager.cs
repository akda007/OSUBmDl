using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Text;

namespace OSUBmDl.Utils
{

    public class ConfigManager
    {
        public static Config Load(string path)
        {
            if (!File.Exists(path))
                Save(path, new Config() { autoInstall = false, firstRun = true });

            string f = File.ReadAllText(path);

            if (f.Contains("autoInstall") && f.Contains("firstRun"))
                return JsonSerializer.Deserialize<Config>(f);


            File.Delete(path);
            Save(path, new Config() { autoInstall = false, firstRun = true });

            return JsonSerializer.Deserialize<Config>(f);
        }

        public static void Save(string path, Config config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions());

            File.WriteAllText(path, json);
        }
    }
}
