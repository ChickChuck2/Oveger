﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Oveger.XAMLS
{
    internal static class ConfigManager
    {
        private static readonly string pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\CyWoodsDev\Oveger\";
        private static readonly string fileName = "config.json";
        private static readonly string FileName = Path.Combine(pathFolder, fileName);
        public static MainWindow.Modifiers GetMODKey(int index)
        {
            List<MainWindow.Modifiers> MODKey = new List<MainWindow.Modifiers>();
            JObject jsonobj = JsonConvert.DeserializeObject(File.ReadAllText(FileName)) as JObject;
            foreach (JToken key in jsonobj["modkeys"])
                MODKey.Add((MainWindow.Modifiers)Enum.Parse(typeof(MainWindow.Modifiers), key.ToString()));
            return MODKey[index];
        }
        public static object GetKey()
        {
            JObject jsonobj = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(FileName));
            return Enum.Parse(typeof(Keys), (string)jsonobj["KEY"]);
        }

        public static void ChangeHotkeys(Keys key, MainWindow.Modifiers mod1, MainWindow.Modifiers mod2 = MainWindow.Modifiers.NoMod)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray modkeys = new JArray
            {
                Enum.GetName(typeof(MainWindow.Modifiers), mod1),
                Enum.GetName(typeof(MainWindow.Modifiers), mod2),
            };

            jsonobj.modkeys = modkeys;
            jsonobj.KEY = Enum.GetName(typeof(Keys), key);
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText(FileName, output);
        }
        public static void Save(bool startwithwindows, string PathToSave = null)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            if(PathToSave != null)
            {
                JArray PathsSTR = new JArray();
                foreach (string Path in jsonobj.Paths)
                    PathsSTR.Add(Path);
                PathsSTR.Add(PathToSave);
                jsonobj.Paths = PathsSTR;
            }
            jsonobj.startWithWindows = startwithwindows;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText(FileName, output);
        }

        public static void ChangePath(string oldPath, string newPath)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach (string path in jsonobj.Paths)
                if (oldPath == path)
                    PathsSTR.Add(newPath);
                else
                    PathsSTR.Add(path);
            jsonobj.Paths = PathsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText(FileName, output);
        }

        private static string GetKeyName(Enum typer, Enum key)
        {
            return Enum.GetName(typer.GetType(), key);
        }

        public static void LoadOrCreate(MainWindow mainWindow)
        {
            if (!File.Exists(FileName))
            {
                if (!Directory.Exists(pathFolder))
                    try
                    {
                        Directory.CreateDirectory(pathFolder);
                    }catch(FileNotFoundException ex)
                    {
                        DialogResult d = MessageBox.Show($"Seu Windows Defender bloqueou o acesso a OneDrive' Precisamos que você dê acesso a essa pasta para que possamos" +
                            $" dar inicio a criação da pasta {pathFolder}", $"Oveger - Acesso bloqueado {ex.Message}", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        Process.GetCurrentProcess().Kill();
                        Application.Exit();
                    }
                File.Create(FileName).Dispose();
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartObject();
                    writer.WritePropertyName("startWithWindows");
                    writer.WriteValue(false);
                    writer.WritePropertyName("modkeys");
                    writer.WriteStartArray();
                    writer.WriteValue(MainWindow.Modifiers.Ctrl.ToString());
                    writer.WriteValue(MainWindow.Modifiers.Alt.ToString());
                    writer.WriteEndArray();
                    writer.WritePropertyName("KEY");
                    writer.WriteValue(Keys.S.ToString());
                    writer.WritePropertyName("Paths");
                    writer.WriteStartArray();
                    writer.WriteEndArray();

                    writer.WritePropertyName("labelsname");
                    writer.WriteStartArray();
                    writer.WriteStartObject();

                    writer.WriteEndObject();
                    writer.WriteEnd();

                    writer.WriteEndObject();

                    using (StreamWriter file = File.CreateText(FileName))
                    {
                        JObject JOBe = JObject.Parse(sb.ToString());
                        JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
                        serializer.Serialize(file, JOBe);
                        file.Dispose();
                    }
                }
            }
            else
            {
                StreamReader r = new StreamReader(FileName);
                string json = r.ReadToEnd();
                dynamic data = JObject.Parse(json);
                dynamic Paths = data.Paths;
                foreach (string path in Paths)
                    if (File.Exists(path))
                        mainWindow.SetConfig(path);
                r.Dispose();
            }
        }

        public static bool GetBool()
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);

            return (bool)jsonobj.startWithWindows;
        }

        public static void VerifyPaths(bool warn)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach(string path in jsonobj.Paths)
                if (File.Exists(path))
                    PathsSTR.Add(path);
                else
                    if(warn)
                        System.Windows.MessageBox.Show($"[{Path.GetFileName(path)}] NÃO ENCONTRADO\n-- {path} ", "LOCAL NÃO ENCONTRADO");

            if(PathsSTR.Count != jsonobj.Paths.Count)
            {
                jsonobj.Paths = PathsSTR;
                string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
                File.WriteAllText(FileName, output);
            }
        }
        public static void Remove(string path)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            JArray PathsSTR = new JArray();
            foreach (string paths in jsonobj.Paths)
                if (paths != path)
                    PathsSTR.Add(paths);
            jsonobj.Paths = PathsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText(FileName, output);
        }
        public static int RenameLabel(string path, string labelName)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            dynamic labelsname = jsonobj.labelsname[0];
            dynamic paths = jsonobj.Paths;

            JArray LabelsSTR = new JArray();
            JObject jo = new JObject();
            LabelsSTR.Add(jo);

            int i = 0;

            foreach (string pathssaved in paths)
            {
                if (labelsname[pathssaved] != null && i == 0)
                {
                    jo.Add(pathssaved, jsonobj.labelsname[0][pathssaved]);
                    i++;
                }
                if (i >= 1 && labelsname[pathssaved] != null)
                    i++;
            }
                

            try { jo.Add(path, labelName); }
            catch
            {
                jo.Remove(path);
                jo.Add(path, labelName);
            }

            jsonobj.labelsname = LabelsSTR;
            string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
            File.WriteAllText(FileName, output);
            return i;
        }
        public static string GetLabelName(string path, string defaultName)
        {
            string json = File.ReadAllText(FileName);
            dynamic jsonobj = JsonConvert.DeserializeObject(json);
            if (jsonobj.labelsname[0][path] != null)
                return jsonobj.labelsname[0][path];
            else
                return defaultName;
        }
    }
}
