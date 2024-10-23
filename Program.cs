using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace MyNamespace
{
    class MyClassCS {
        static string path = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;

        static void Main() {
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
            Console.WriteLine(File.ReadAllText("bannerE.txt"));
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
            Console.WriteLine($"{DateTime.Now} Saved: {e.FullPath}");

            string xmlPath = e.FullPath.Replace(".als", ".xml").Replace("ave","output");
            string args = $"gzip -cd '{e.FullPath}' > '{xmlPath}' -encoding utf8"; //-encoding utf8 makes empty file

            //TODO: NEED TO HANDLE ERRORS ON CMD

            ProcessStartInfo ProcessInfo;
            ProcessInfo = new ProcessStartInfo("powershell.exe", "/C " + args);
            if (ProcessInfo != null){
                ProcessInfo.CreateNoWindow = true;
                ProcessInfo.UseShellExecute = false;

                Process.Start(ProcessInfo);

                Console.WriteLine($"{DateTime.Now} Unzipped XML: {xmlPath}");

                //GZIP generates UTF-16 LE BOM encoding and git cant see it
                //probably need to read file, change encoding, and write to new file
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
            if(!(e.OldFullPath+e.FullPath).Contains("Tmp")){
                Console.WriteLine($"Renamed:");
                Console.WriteLine($"    Old: {e.OldFullPath}");
                Console.WriteLine($"    New: {e.FullPath}");
            }
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
