using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Oveger.XAMLS
{
    internal class ConfigManager
    {
        public enum TypeFile
        {
            Create,Load,Save,Remove
        }

        public void LoadOrCreateConfigs(TypeFile typeFile, MainWindow mainWindow, string PathToSave = null)
        {
            if(typeFile == TypeFile.Create)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartObject();

                    writer.WritePropertyName("Paths");
                    writer.WriteStartArray();

                    //VALUES PATHS HERE

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
            else if (typeFile == TypeFile.Load)
            {
                StreamReader r = new StreamReader("itens.json");

                string json = r.ReadToEnd();

                dynamic data = JObject.Parse(json);
                dynamic Paths = data.Paths;


                //GETVALUES

                //SETVALUES
                

                foreach (string path in Paths)
                {
                    mainWindow.ButtonSettings(path);
                }
                    


                r.Dispose();
            }
            else if(typeFile == TypeFile.Save)
            {
                string json = File.ReadAllText("itens.json");

                dynamic jsonobj = JsonConvert.DeserializeObject(json);

                JArray PathsSTR = new JArray();
                foreach(string Path in jsonobj.Paths)
                    PathsSTR.Add(Path);

                PathsSTR.Add(PathToSave);

                //SETVALUES
                
             
                jsonobj.Paths = PathsSTR;

                //var pro = JsonConvert.DeserializeObject<List<string>>(jsonobj.Paths[0]);
                //Console.WriteLine(pro);

                Console.WriteLine("COLOCAR PATHS");
                

                string output = JsonConvert.SerializeObject(jsonobj, Formatting.Indented);
                File.WriteAllText("itens.json", output);
            }
        }

    }
}
