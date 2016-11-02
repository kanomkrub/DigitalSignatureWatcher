using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DigitalSignatureWatcher
{
    class Watcher
    {
        static void Main(string[] args)
        {
            var folders = GetConfiguration();

            foreach(var folder in folders)
            {


            }

            //var servicePath = config["digital_signatue_service"];
            //var watcher = new FileSystemWatcher(@"D:\temp\testSignatureService");
            //watcher.IncludeSubdirectories = true;
            //watcher.Filter = "";
            ////watcher.Renamed += new RenamedEventHandler(renamed);
            ////watcher.Deleted += new FileSystemEventHandler(changed);
            ////watcher.Changed += new FileSystemEventHandler(changed);
            //watcher.Created += new FileSystemEventHandler(changed);
            //watcher.EnableRaisingEvents = true;

            Console.ReadKey();
        }

        private static void renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine(DateTime.Now + ": " +
                e.ChangeType + " " + e.FullPath);
        }

        private static void changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(DateTime.Now + ": " +
                e.ChangeType + " " + e.FullPath);
        }

        public class FolderConfig
        {
            public string description { get; set; }
            public string watch_folder_in { get; set; }
            public string watch_folder_out { get; set; }
            public string digital_signature_service { get; set; }
            public string template_id { get; set; }
        }
        public static List<FolderConfig> GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            var config = builder.AddJsonFile("config.json").Build();

            var folders = config.GetSection("watch_folders").GetChildren().Select(t => t.GetChildren());
            var configs = new List<FolderConfig>();
            foreach (var folder in folders)
            {
                configs.Add(new FolderConfig()
                {
                    description = folder.Single(t => t.Key == "description").Value,
                    watch_folder_in = folder.Single(t => t.Key == "watch_folder_in").Value,
                    watch_folder_out = folder.Single(t => t.Key == "watch_folder_out").Value,
                    digital_signature_service = folder.Single(t => t.Key == "digital_signature_service").Value,
                    template_id = folder.Single(t => t.Key == "template_id").Value
                });
            }
            return configs;
        }
    }
}