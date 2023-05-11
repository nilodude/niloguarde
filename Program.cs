using System;
using System.IO;
using System.Diagnostics;

namespace MyNamespace
{
    class MyClassCS
    {
        static string path = @"D:\MUSICA\ableton\sinbeat\stealin Project";
        static string currDir = Directory.GetCurrentDirectory();
        static void Main()
        {
            // THE IDEA IS TO RUN THIS APP FROM ABLETON PROJECT DIRECTORY, AND USE CURRENT DIRECTORY PATH
            // TODO: NEED TO INFER PATH FROM CURRENT DIRECTORY 
            // Console.WriteLine(currDir);
            
            using var watcher = new FileSystemWatcher(path);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.als";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            string banner = File.ReadAllText("banner.txt");
            Console.WriteLine(banner);
            Console.WriteLine($"Watching changes at {path}");
            Console.WriteLine("\nPress ENTER anytime to exit.");
            Console.WriteLine("\nPlease make honest music.\n");
            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Saved: {e.FullPath}");

            string[] fullPath = e.FullPath.Split('\\');
            string file =  fullPath.Last();
            string fileName = file.Split('.').First();
            string fileType = file.Split('.').Last();
            
            if(!"lacopia".Equals(fileName)){

                string xmlPath = e.FullPath.Replace(".als", ".xml");
                // string args = "gzip -cd '" + e.FullPath + "' > '" + xmlPath + "'";
                string args = $"gzip -cd '{e.FullPath}' > '{xmlPath}'";  
                //TODO: NEED TO HANDLE ERRORS ON CMD

                ProcessStartInfo ProcessInfo;
                ProcessInfo = new ProcessStartInfo("powershell.exe", "/C " + args);
                if (ProcessInfo != null){
                    ProcessInfo.CreateNoWindow = true;
                    ProcessInfo.UseShellExecute = false;

                    Process.Start(ProcessInfo);
                    
                    Console.WriteLine($"Unzipped XML: {xmlPath}");
                }
            }
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
