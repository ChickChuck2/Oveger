using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Oveger.XAMLS
{
    internal static class ConfigManager
    {
        private static readonly string pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\CyWoodsDev\Oveger\";
        private static readonly string fileName    = "config.json";
        private static readonly string FileName   = Path.Combine(pathFolder, fileName);

        // ════════════════════════════════════════════════════════
        //  RESILIENCE LAYER
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// Último motivo de falha registrado (para diagnóstico).
        /// </summary>
        public static string LastError { get; private set; } = string.Empty;

        /// <summary>
        /// Tenta ler e fazer parse do config.json.
        /// Retorna null se o arquivo não existir, estiver vazio ou corrompido.
        /// </summary>
        private static JObject SafeRead()
        {
            try
            {
                if (!File.Exists(FileName))
                    return null;

                string json = File.ReadAllText(FileName, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(json))
                {
                    LastError = "Arquivo de configuração está vazio.";
                    return null;
                }

                JObject obj = JObject.Parse(json);
                return obj;
            }
            catch (JsonReaderException ex)
            {
                LastError = $"JSON inválido/corrompido: {ex.Message}";
                return null;
            }
            catch (IOException ex)
            {
                LastError = $"Erro de acesso ao arquivo: {ex.Message}";
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                LastError = $"Sem permissão de leitura: {ex.Message}";
                return null;
            }
            catch (Exception ex)
            {
                LastError = $"Erro inesperado: {ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Move o arquivo corrompido para um backup com timestamp e cria um novo config padrão.
        /// </summary>
        private static void BackupCorruptedAndReset()
        {
            if (File.Exists(FileName))
            {
                string timestamp  = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupPath = Path.Combine(pathFolder, $"config.corrupted_{timestamp}.json");
                try
                {
                    File.Move(FileName, backupPath);
                    MessageBox.Show(
                        $"O arquivo de configuração estava corrompido ou ilegível.\n\n" +
                        $"Motivo: {LastError}\n\n" +
                        $"Um backup foi salvo em:\n{backupPath}\n\n" +
                        $"O Oveger criará um novo arquivo de configuração.",
                        "Oveger — Configuração corrompida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                catch
                {
                    // Se não conseguir mover, tenta deletar para poder recriar
                    try { File.Delete(FileName); } catch { /* melhor esforço */ }
                }
            }

            WriteDefaultConfig();
        }

        /// <summary>
        /// Garante que o arquivo de configuração existe e é válido.
        /// Se não existir → cria. Se corrompido/vazio → faz backup e recria.
        /// Se campos obrigatórios faltam → os adiciona com valores padrão.
        /// </summary>
        private static void EnsureValidConfig()
        {
            if (!Directory.Exists(pathFolder))
            {
                try { Directory.CreateDirectory(pathFolder); }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Não foi possível criar a pasta de configuração:\n{pathFolder}\n\nErro: {ex.Message}",
                        "Oveger — Falha crítica",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.GetCurrentProcess().Kill();
                }
            }

            // Arquivo não existe → criar do zero
            if (!File.Exists(FileName))
            {
                WriteDefaultConfig();
                return;
            }

            // Tenta ler → se falhar → backup + recriar
            JObject obj = SafeRead();
            if (obj == null)
            {
                BackupCorruptedAndReset();
                return;
            }

            // Arquivo existe e parseou, mas pode ter campos faltando (versão antiga)
            bool dirty = false;

            if (obj["startWithWindows"] == null)  { obj["startWithWindows"] = false; dirty = true; }
            if (obj["modkeys"] == null)
            {
                obj["modkeys"] = new JArray
                {
                    MainWindow.Modifiers.Ctrl.ToString(),
                    MainWindow.Modifiers.Alt.ToString()
                };
                dirty = true;
            }
            if (obj["KEY"] == null)               { obj["KEY"] = Keys.S.ToString(); dirty = true; }
            if (obj["Paths"] == null)             { obj["Paths"] = new JArray(); dirty = true; }
            if (obj["Groups"] == null || obj["Groups"] is JArray)
            {
                obj["Groups"] = new JObject();
                dirty = true;
            }
            if (obj["labelsname"] == null)
            {
                obj["labelsname"] = new JArray { new JObject() };
                dirty = true;
            }

            if (dirty)
            {
                try { File.WriteAllText(FileName, obj.ToString(Formatting.Indented), Encoding.UTF8); }
                catch { /* não crítico — carrega o que tem */ }
            }
        }

        /// <summary>
        /// Escreve um config.json padrão do zero.
        /// </summary>
        private static void WriteDefaultConfig()
        {
            var defaultObj = new JObject
            {
                ["startWithWindows"] = false,
                ["modkeys"] = new JArray
                {
                    MainWindow.Modifiers.Ctrl.ToString(),
                    MainWindow.Modifiers.Alt.ToString()
                },
                ["KEY"]        = Keys.S.ToString(),
                ["Paths"]      = new JArray(),
                ["Groups"]     = new JObject(),
                ["labelsname"] = new JArray { new JObject() }
            };

            try
            {
                File.WriteAllText(FileName, defaultObj.ToString(Formatting.Indented), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível criar o arquivo de configuração:\n{FileName}\n\nErro: {ex.Message}",
                    "Oveger — Falha crítica",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// Tenta gravar JSON no arquivo de config com proteção contra falha de escrita.
        /// </summary>
        private static bool SafeWrite(JObject obj)
        {
            try
            {
                File.WriteAllText(FileName, obj.ToString(Formatting.Indented), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Falha ao salvar configuração: {ex.Message}";
                MessageBox.Show(
                    $"Não foi possível salvar a configuração.\n\nErro: {ex.Message}",
                    "Oveger — Erro ao salvar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ════════════════════════════════════════════════════════
        //  PUBLIC API
        // ════════════════════════════════════════════════════════

        public static MainWindow.Modifiers GetMODKey(int index)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            List<MainWindow.Modifiers> MODKey = new List<MainWindow.Modifiers>();
            foreach (JToken key in jsonobj["modkeys"])
                MODKey.Add((MainWindow.Modifiers)Enum.Parse(typeof(MainWindow.Modifiers), key.ToString()));
            return MODKey[index];
        }

        public static object GetKey()
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            return Enum.Parse(typeof(Keys), (string)jsonobj["KEY"]);
        }

        public static void ChangeHotkeys(Keys key, MainWindow.Modifiers mod1, MainWindow.Modifiers mod2 = MainWindow.Modifiers.NoMod)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();

            jsonobj["modkeys"] = new JArray
            {
                Enum.GetName(typeof(MainWindow.Modifiers), mod1),
                Enum.GetName(typeof(MainWindow.Modifiers), mod2)
            };
            jsonobj["KEY"] = Enum.GetName(typeof(Keys), key);

            SafeWrite(jsonobj);
        }

        [Obsolete("Use SavePath() , ChangeStartWithWindows() instead")]
        public static void Save(bool startwithwindows, string PathToSave = null, List<string> groups = null)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();

            if (PathToSave != null)
            {
                JArray PathsSTR = new JArray();
                foreach (string p in jsonobj["Paths"])
                    PathsSTR.Add(p);
                PathsSTR.Add(PathToSave);
                jsonobj["Paths"] = PathsSTR;
            }
            jsonobj["startWithWindows"] = startwithwindows;
            try   { jsonobj["Groups"] = groups != null ? JArray.FromObject(groups) : jsonobj["Groups"]; }
            catch { jsonobj["Groups"] = null; }

            SafeWrite(jsonobj);
        }

        public static void SavePath(string pathToSave)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();

            JArray PathsSTR = new JArray();
            foreach (string p in jsonobj["Paths"])
                PathsSTR.Add(p);
            PathsSTR.Add(pathToSave);
            jsonobj["Paths"] = PathsSTR;

            SafeWrite(jsonobj);
        }

        public static void ChangePath(string oldPath, string newPath)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();

            JArray PathsSTR = new JArray();
            foreach (string p in jsonobj["Paths"])
                PathsSTR.Add(oldPath == p ? newPath : p);
            jsonobj["Paths"] = PathsSTR;

            SafeWrite(jsonobj);
        }

        public static void AddGroups(string[] groups)
        {
            EnsureValidConfig();
            JObject jsonobj  = SafeRead();
            JObject jgroups  = (JObject)jsonobj["Groups"] ?? new JObject();
            JObject curr     = new JObject();

            foreach (var g in jgroups)
                curr.Add(g.Key, g.Value);

            foreach (string g in groups)
            {
                if (!GetGroups().Contains(g))
                    curr.Add(g, new JArray());
                else
                    MessageBox.Show($"Já existe um grupo chamado '{g}'. Pulado.", "Oveger", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            jsonobj["Groups"] = curr;
            SafeWrite(jsonobj);
        }

        public static void AddPathOnGroup(string path, string group)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JObject jgroups = (JObject)jsonobj["Groups"];

            foreach (var v in jgroups)
            {
                if (v.Key.Equals(group))
                {
                    var target = (JArray)jgroups[group];
                    target.Add(path);
                    jsonobj["Groups"][group] = target;
                }
            }
            SafeWrite(jsonobj);
        }

        public static void RemovePathOnGroup(string path, string group)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JObject jgroups = (JObject)jsonobj["Groups"];
            JArray newPaths = new JArray();

            foreach (var v in jgroups)
                if (v.Key.Equals(group))
                {
                    foreach (var v2 in (JArray)v.Value)
                        if ((string)v2 != path)
                            newPaths.Add(v2);
                    break;
                }

            jsonobj["Groups"][group] = newPaths;
            SafeWrite(jsonobj);
        }

        public static string[] GetGroups()
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JObject groups  = (JObject)jsonobj["Groups"];
            List<string> result = new List<string>();
            foreach (var g in groups)
                result.Add(g.Key);
            return result.ToArray();
        }

        public static Dictionary<string, int> GetCountOfGroupsPaths()
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JObject groups  = (JObject)jsonobj["Groups"];
            var result = new Dictionary<string, int>();
            foreach (var g in groups)
                result.Add(g.Key, g.Value.Count());
            return result;
        }

        public static string GetGroupByPath(string path)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JObject groups  = (JObject)jsonobj["Groups"];
            string result   = string.Empty;

            foreach (var item in groups)
                foreach (JValue v in item.Value)
                    if (v.ToString().Equals(path))
                    {
                        result = item.Key;
                        break;
                    }
                    else
                        result = string.Empty;

            return result;
        }

        public static void RemoveGroup(string group)
        {
            EnsureValidConfig();
            JObject jsonobj  = SafeRead();
            JObject curr = new JObject();

            foreach (var g in (JObject)jsonobj["Groups"])
                if (g.Key != group)
                    curr.Add(g.Key, g.Value);

            jsonobj["Groups"] = curr;
            SafeWrite(jsonobj);
        }

        public static Dictionary<string, string[]> GetPathAndGroup()
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JObject groups  = (JObject)jsonobj["Groups"];
            var result = new Dictionary<string, string[]>();

            foreach (var g in groups)
            {
                List<string> paths = new List<string>();
                foreach (var p in g.Value)
                    paths.Add(p.ToString());
                result.Add(g.Key, paths.ToArray());
            }
            return result;
        }

        [Obsolete]
        private static string GetKeyName(Enum typer, Enum key) => Enum.GetName(typer.GetType(), key);

        public static void LoadOrCreate(MainWindow mainWindow)
        {
            EnsureValidConfig();

            JObject data;
            try
            {
                data = SafeRead();
                if (data == null)
                {
                    // EnsureValidConfig deveria ter resolvido, mas tenta novamente
                    WriteDefaultConfig();
                    data = SafeRead();
                    if (data == null) return; // desiste se ainda falhar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro inesperado ao carregar configuração:\n{ex.Message}",
                    "Oveger", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Corrige legado: "Groups" como JArray → converte para JObject vazio
            if (data["Groups"] is JArray)
            {
                data["Groups"] = new JObject();
                SafeWrite(data);
            }

            // Carrega os atalhos salvos
            JArray paths = data["Paths"] as JArray ?? new JArray();
            foreach (string path in paths)
            {
                try
                {
                    if (File.Exists(path))
                        mainWindow.SetConfig(path);
                }
                catch (Exception ex)
                {
                    // Item inválido — apenas ignora e continua
                    Console.WriteLine($"[ConfigManager] Falha ao carregar item '{path}': {ex.Message}");
                }
            }
        }

        public static string[] GetPaths()
        {
            EnsureValidConfig();
            JObject data  = SafeRead();
            JArray Paths  = data["Paths"] as JArray ?? new JArray();
            List<string> result = new List<string>();
            foreach (string p in Paths)
                if (File.Exists(p))
                    result.Add(p);
            return result.ToArray();
        }

        public static void ChangeStartWithWindows()
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            jsonobj["startWithWindows"] = !(bool)jsonobj["startWithWindows"];
            SafeWrite(jsonobj);
        }

        public static bool GetBool()
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            return jsonobj["startWithWindows"]?.Value<bool>() ?? false;
        }

        public static void VerifyPaths(bool warn)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JArray PathsSTR = new JArray();

            foreach (string path in jsonobj["Paths"])
            {
                if (File.Exists(path))
                {
                    PathsSTR.Add(path);
                }
                else
                {
                    bool found = false;
                    try
                    {
                        string dir = Path.GetDirectoryName(path);
                        if (dir != null && Directory.Exists(dir))
                        {
                            var subs = Directory.GetDirectories(dir);
                            foreach (var sub in subs)
                            {
                                string filename = Path.GetFileName(path);
                                string candidate = Path.Combine(sub, filename);
                                if (File.Exists(candidate))
                                {
                                    MessageBox.Show($"'{filename}' foi encontrado em:\n{candidate}", "Arquivo movido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    PathsSTR.Add(candidate);
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch { /* continua */ }

                    if (warn && !found)
                        System.Windows.MessageBox.Show(
                            $"[{Path.GetFileName(path)}] NÃO ENCONTRADO\n-- {path}",
                            "LOCAL NÃO ENCONTRADO");
                }
            }

            if (PathsSTR.Count > 0)
            {
                jsonobj["Paths"] = PathsSTR;
                SafeWrite(jsonobj);
            }
        }

        public static void Remove(string path)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            JArray PathsSTR = new JArray();

            foreach (string p in jsonobj["Paths"])
                if (p != path)
                    PathsSTR.Add(p);

            jsonobj["Paths"] = PathsSTR;
            SafeWrite(jsonobj);
        }

        public static int RenameLabel(string path, string labelName)
        {
            EnsureValidConfig();
            JObject jsonobj    = SafeRead();
            JArray labelsArray = jsonobj["labelsname"] as JArray ?? new JArray(new JObject());
            JObject jo         = labelsArray[0] as JObject ?? new JObject();

            JArray newLabels = new JArray { jo };
            int i = 0;

            foreach (string savedPath in jsonobj["Paths"])
            {
                if (jo[savedPath] != null && i == 0) { jo.Add(savedPath, jsonobj["labelsname"][0][savedPath]); i++; }
                else if (i >= 1 && jo[savedPath] != null) i++;
            }

            try   { jo.Add(path, labelName); }
            catch { jo.Remove(path); jo.Add(path, labelName); }

            jsonobj["labelsname"] = newLabels;
            SafeWrite(jsonobj);
            return i;
        }

        public static string GetLabelName(string path, string defaultName)
        {
            EnsureValidConfig();
            JObject jsonobj = SafeRead();
            try
            {
                var label = jsonobj["labelsname"]?[0]?[path];
                return label != null ? (string)label : defaultName;
            }
            catch
            {
                return defaultName;
            }
        }
    }
}