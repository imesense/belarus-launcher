using System;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.IO;

namespace StalkerBelarus.Launcher.ViewModels.Manager;
    //Беларусь никогда не выйдет
    public class MyDownloadManager : IDisposable
    {
        private string[] Folders = { "binaries", "resources", "patches" };
        private JsonDocument? jsonDocument;
        public MyDownloadManager()
        {
            string response = "{}";
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                var Url = "https://api.github.com/repos/Belarus-Mod/Mod-Data/releases/latest";
                response = client.GetStringAsync(Url).Result;

                DebugOutput("Подключение к сети!");
            }
            catch
            {
                response = "{}";
                DebugOutput("Невозможно загрузить релиз!");
            }
            jsonDocument = JsonDocument.Parse(response);
        }
        public void DebugOutput(string log)
        {
            Console.WriteLine(log);
        }
        private string GetNewsFile()
        {
            using var client = new HttpClient();
            var root = jsonDocument.RootElement;
            var element = FindFileByName("news.json");
            return client.GetStringAsync(element.GetProperty("browser_download_url").ToString()).Result;
        }
        public IList<NewsViewModel> GetNewsList() {
            var newsList = new List<NewsViewModel>();
            try {
                var newsFile = GetNewsFile();
                var news = JsonDocument.Parse(newsFile).RootElement;
                foreach (var _news in news.EnumerateObject())
                {
                    newsList.Add(new NewsViewModel(_news.Name, _news.Value.ToString()));
                }
            } 
            catch 
            {
                newsList.Add(new NewsViewModel("Ошибка!", "Ошибка загрузки новостей. Возможно интернет-соединение отсутствует."));
            }
            newsList.Reverse();
            return newsList;
        }
        private JsonElement FindFileByName(string name)
        {
            JsonElement root = jsonDocument.RootElement;
            var Assets = root.GetProperty("assets");
            foreach (var Item in Assets.EnumerateArray())
            {
                if (Item.GetProperty("name").ToString().ToLower() == name.ToLower())
                {
                    return Item;
                }
            }
            return root;
        }
        private void CalculateMD5(string[] filepath, Utf8JsonWriter writer)
        {
            foreach (var folder in filepath)
            {
                writer.WriteStartObject(folder);
                try
                {
                    var Dir = Directory.GetCurrentDirectory() + "\\" + folder;
                    foreach (string file in Directory.EnumerateFiles(Dir, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        using var Compute = MD5.Create();
                        using var Stream = File.OpenRead(file);
                        var Hash = Compute.ComputeHash(Stream);
                        var MD5Hash = Convert.ToHexString(Hash);
                        writer.WriteString(Path.GetFileName(file).ToLower(), MD5Hash);
                        DebugOutput("Hash " + Path.GetFileName(file) + " - " + MD5Hash);
                    }
                }
                catch {
                }
                writer.WriteEndObject();
            }
        }
        private void LoadFile(string FilePath, string FileName)
        {
            try
            {
                DebugOutput("Load " + FilePath + FileName);
                var Adress = FindFileByName(FileName).GetProperty("browser_download_url").ToString();
                using var Client = new WebClient();
                var dirInfo = new DirectoryInfo(FilePath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                Client.DownloadFile(Adress, FilePath + FileName);
                DebugOutput("Adress " + Adress);
            }
            catch
            {
            }
        }
        private void LoadMissedFiles(JsonElement local, JsonElement server)
        {
            foreach (var folder in server.EnumerateObject())
            {
                foreach (var file in folder.Value.EnumerateObject())
                {
                    var Path = Directory.GetCurrentDirectory() + "\\" + folder.Name + "\\";
                    try
                    {
                        if (file.Value.ToString() != local.GetProperty(folder.Name).GetProperty(file.Name).ToString())
                        {
                            LoadFile(Path, file.Name);
                        }
                    }
                    catch
                    {
                        LoadFile(Path, file.Name);
                    }
                }
            }
        }
        private void DeleteExtraFiles(JsonElement local, JsonElement server)
        {
            foreach (var folder in local.EnumerateObject())
            {
                foreach (var file in folder.Value.EnumerateObject())
                {
                    var Path = Directory.GetCurrentDirectory() + "\\" + folder.Name + "\\" + file.Name;
                    try
                    {
                        server.GetProperty(folder.Name).GetProperty(file.Name);
                    }
                    catch
                    {
                        try
                        {
                            File.Delete(Path);
                            DebugOutput("Delete " + Path);
                        }
                        catch
                        {
                            DebugOutput("Error Delete " + Path);
                        }
                    }
                }
            }
        }
        public string GetLocalHash()
        {
            try
            {
                JsonWriterOptions Options = new JsonWriterOptions
                {
                    Indented = true
                };
                using (var Stream = new MemoryStream())
                {
                    using (var Writer = new Utf8JsonWriter(Stream, Options))
                    {
                        Writer.WriteStartObject();
                        CalculateMD5(Folders, Writer);
                        Writer.WriteEndObject();
                    }
                    string Json = Encoding.UTF8.GetString(Stream.ToArray());
                    return Json;
                }
            }
            catch
            {
            }
            return "{}";
        }
        private string GetServerHash()
        {
            try
            {
                using var client = new HttpClient();
                var root = jsonDocument.RootElement;
                var element = FindFileByName("hash.json");
                return client.GetStringAsync(element.GetProperty("browser_download_url").ToString()).Result;
            }
            catch
            {
            }
            return "{}";
        }
        public bool CheckFiles(bool update = false)
        {
            try
            {
                var ServerHash = GetServerHash();
                var LocalHash = GetLocalHash();

                if (ServerHash != LocalHash)
                {
                    DebugOutput(ServerHash);
                    DebugOutput(LocalHash);

                    var Server = JsonDocument.Parse(ServerHash).RootElement;
                    var Local = JsonDocument.Parse(LocalHash).RootElement;

                    if (update) {
                        DeleteExtraFiles(Local, Server);
                        LoadMissedFiles(Local, Server);
                    }

                    DebugOutput("Loading End");
                    return true;
                }
            }
            catch
            {
                DebugOutput("Loading Error");
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            jsonDocument.Dispose();
        }
    }
