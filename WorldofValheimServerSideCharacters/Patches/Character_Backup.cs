using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace WorldofValheimServerSideCharacters
{
    public class Character_Backup
    {
        // Lets create the BackupCharacter Thread!
        public static Thread BackupCharacter;
        // Here is the CopyFolder
        public static void CopyCharacters(string SavePath, string BackupPath)
        {
            if (!Directory.Exists(BackupPath))
                Directory.CreateDirectory(BackupPath);
            string[] files = Directory.GetFiles(SavePath);
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(BackupPath, Path.GetFileName(file)));
            }
            string[] folders = Directory.GetDirectories(SavePath);
            foreach (string folder in folders)
            {
                CopyCharacters(folder, Path.Combine(BackupPath, Path.GetFileName(folder)));
            }
        }
        public static void BackupScanner()
        {
            Debug.Log("Backup: Backup thread has been started. Initiating backup sequence.");
            Backup_Characters(DateTime.Now.ToString("yyyy-MM-dd_HH-mm"));
            while (true)
            {
                // Sleep the thread for (X) minutes.
                Thread.Sleep(WorldofValheimServerSideCharacters.BackupInterval.Value * 60 * 1000);
                Backup_Characters(DateTime.Now.ToString("yyyy-MM-dd_HH"));
            }
        }
        public static void Backup_Characters(string CurrentTime)
        {
            var SavePath = WorldofValheimServerSideCharacters.CharacterSavePath.Value;
            var BackupLocation = Path.Combine(SavePath + "-bak");
            var BackupPath = Path.Combine(BackupLocation, CurrentTime);
            Debug.Log("Backup: Attempting to backup all Server-Side Characters..");
            if (!Directory.Exists(BackupLocation))
                Directory.CreateDirectory(BackupLocation);
            IOrderedEnumerable<FileSystemInfo> BackupDirectory = new DirectoryInfo(SavePath + "-bak").GetFileSystemInfos().OrderByDescending(fi => fi.CreationTime);
            if (BackupDirectory.Count() > WorldofValheimServerSideCharacters.MaxBackups.Value - 1)
            {
                Directory.Delete(BackupDirectory.Last().FullName, true);
            }
            CopyCharacters(SavePath, BackupPath);
            Debug.Log("Backup: All Server-Side Characters have been backed up.");
        }
    }
}