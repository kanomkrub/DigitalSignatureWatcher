using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestDigitalSignatureServiceApi;

namespace DigitalSignatureWatcher
{
    public class Worker
    {
        string servicePath, templateId, folderIn, folderOut;
        FileSystemWatcher watcher;
        public Worker(string servicePath, string templateId, string folderIn, string folderOut)
        {
            this.servicePath = servicePath;
            this.templateId = templateId;
            this.folderIn = folderIn;
            this.folderOut = folderOut;
            this.watcher = new FileSystemWatcher(folderIn)
            {
                IncludeSubdirectories = false,
                Filter = "",
                EnableRaisingEvents = false
            };
            watcher.Created += new FileSystemEventHandler(file_created);
        }
        public void Start()
        {
            foreach (var filePath in Directory.GetFiles(folderIn))
            {
                Task.Run(() => { DoWork(filePath); });
            }
            watcher.EnableRaisingEvents = true;
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        private void file_created(object sender, FileSystemEventArgs e)
        {
            Task.Run(() => { DoWork(e.FullPath); });
        }
        private void DoWork(string filePath)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    System.Threading.Thread.Sleep(5000);
                    var outPath = Path.Combine(folderOut, Path.GetFileName(filePath));
                    var client = new DigitalSignatureClientExample(servicePath);
                    var pdfOut = client.BurnFields(File.ReadAllBytes(filePath), templateId);
                    File.WriteAllBytes(outPath, pdfOut);
                    File.Delete(filePath);
                    Console.Out.WriteLine($"watched success {Path.GetFileName(filePath)}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine($"watched error {Path.GetFileName(filePath)}");
                    Console.Out.WriteLine($"error round:{i} file:{filePath} : {ex}");
                    //log here
                }
            }
        }
    }
}
