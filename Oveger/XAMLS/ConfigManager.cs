using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Oveger.XAMLS
{
    internal static class ConfigManager
    {
        public static void Save(string PathToSave)
        {
            string json = File.ReadAllText("itens.json");
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach (string Path in jsonobj.Paths)
                PathsSTR.Add(Path);
            PathsSTR.Add(PathToSave);
            jsonobj.Paths = PathsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText("itens.json", output);
        }

        public static void RemoveifNotExist()
        {
            string json = File.ReadAllText("itens.json");
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach (string path in jsonobj.Paths)
                if (File.Exists(path))
                    PathsSTR.Add(path);
            jsonobj.Paths = PathsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText("itens.json", output);
        }

        public static void ChangePath(string oldPath, string newPath)
        {
            string json = File.ReadAllText("itens.json");
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach (string path in jsonobj.Paths)
                if (oldPath == path)
                    PathsSTR.Add(newPath);
                else
                    PathsSTR.Add(path);
            jsonobj.Paths = PathsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText("itens.json", output);
        }

        public static void LoadOrCreate(MainWindow mainWindow)
        {
            if (!File.Exists("itens.json"))
            {
                File.Create("itens.json").Dispose();
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartObject();
                    writer.WritePropertyName("Paths");
                    writer.WriteStartArray();
                    writer.WriteEndArray();
                    writer.WriteEndObject();

                    using (StreamWriter file = File.CreateText("itens.json"))
                    {
                        JObject JOBe = JObject.Parse(sb.ToString());
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Formatting.Indented;
                        serializer.Serialize(file, JOBe);
                        file.Dispose();
                    }
                }
            }
            else
            {
                StreamReader r = new StreamReader("itens.json");
                string json = r.ReadToEnd();
                dynamic data = JObject.Parse(json);
                dynamic Paths = data.Paths;
                foreach (string path in Paths)
                    if(File.Exists(path))
                        mainWindow.SetConfig(path);
                r.Dispose();
            }
        }
        public static void verifyPaths(bool warn)
        {
            string json = File.ReadAllText("itens.json");
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach(string path in jsonobj.Paths)
                if (File.Exists(path))
                    PathsSTR.Add(path);
                else
                    if (warn)
                    {
                        MessageBox.Show($"[{Path.GetFileName(path)}] NÃO ENCONTRADO\n-- {path} ", "LOCAL NÃO ENCONTRADO");
                    }
            jsonobj.Paths = PathsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText("itens.json", output);
        }
    }
}
