using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DigitalSignatureWatcher
{
    class Watcher
    {
        static void Main(string[] args)
        {
            var folders = GetConfiguration();

            foreach (var folder in folders.Where(t => t.enable))
            {
                Task.Run(() =>
                {
                    var worker = new Worker(folder.digital_signature_service, folder.template_id, folder.watch_folder_in, folder.watch_folder_out);
                    worker.Start();
                });
                Console.Out.WriteLine($"Start watch for '{folder.watch_folder_in}' desc:{folder.description}");
            }
            Console.Out.WriteLine("please enter to exit");
            Console.ReadLine();
        }
        public class FolderConfig
        {
            public string description { get; set; }
            public string watch_folder_in { get; set; }
            public string watch_folder_out { get; set; }
            public string digital_signature_service { get; set; }
            public string template_id { get; set; }
            public bool enable { get; set; }
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
                    template_id = folder.Single(t => t.Key == "template_id").Value,
                    enable = bool.Parse(folder.Single(t => t.Key == "enable").Value)
                });
            }
            return configs;
        }
    }
}