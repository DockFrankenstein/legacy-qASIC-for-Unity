using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace qASIC.FileManaging
{
    public static class FileManager
    {
        #region GetPath
        /// <return>Returns the desktop folder path</return>
        public static string GetDesktopPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop); }
        /// <return>Returns the profile folder path</return>
        public static string GetProfilePath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile); }
        /// <return>Returns the favourites folder path</return>
        public static string GetFavouritesPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Favorites); }
        /// <return>Returns the documents folder path</return>
        public static string GetDocumentsPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); }
        /// <return>Returns the music folder path</return>
        public static string GetMusicsPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic); }
        /// <return>Returns the pictures folder path</return>
        public static string GetPicturesPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures); }
        /// <return>Returns the videos folder path</return>
        public static string GetVideosPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos); }
        /// <return>Returns the specified folder</return>
        public static string GetCustomFolder(System.Environment.SpecialFolder type) { return System.Environment.GetFolderPath(type); }
        #endregion

        #region Trim
        /// <summary>Trims paths end by specified amount of folders</summary>
        /// <param name="amount">Amount of folders to trim</param>
        public static string TrimPathEnd(string path, int amount) 
        {
            path = path.Replace('\\', '/');
            amount = Mathf.Abs(amount);
            string[] folders = path.Split('/');

            string trimmedPath = "";
            for (int i = 0; i < folders.Length - amount; i++)
            {
                trimmedPath += $"{folders[i]}/";
            }
            return trimmedPath.TrimEnd('/');
        }

        /// <summary>Trims paths start by specified amount of folders</summary>
        /// <param name="amount">Amount of folders to trim</param>
        public static string TrimPathStart(string path, int amount)
        {
            path = path.Replace('\\', '/');
            amount = Mathf.Abs(amount);
            string[] folders = path.Split('/');

            string trimmedPath = "";
            for (int i = 0; i < folders.Length - amount; i++)
            {
                trimmedPath += $"{folders[i + amount]}/";
            }
            return trimmedPath.TrimEnd('/');
        }
        #endregion

        #region Exists
        public static bool DirectoryExists(string path) { return Directory.Exists(path.Replace('\\', '/')); }
        public static bool FileExists(string path) { return File.Exists(path.Replace('\\', '/')); }
        #endregion

        #region Save and Load
        /// <param name="data">Data to serielize and save</param>
        public static void SaveFile(string path, object data)
        {
            path = path.Replace('\\', '/');
            if (!DirectoryExists(TrimPathEnd(path, 1))) Directory.CreateDirectory(TrimPathEnd(path, 1));
            FileStream fileStream = File.Create(path);
            if (data != null)
            {
                BinaryFormatter binFormater = new BinaryFormatter();
                binFormater.Serialize(fileStream, data);
            }
            fileStream.Close();
        }

        public static object LoadFile(string path)
        {
            FileStream fileStream = File.Open(path, FileMode.Open);
            BinaryFormatter formater = new BinaryFormatter();
            object data = formater.Deserialize(fileStream);
            fileStream.Close();
            return data;
        }

        /// <param name="data">The deserielized object</param>
        /// <returns>Returns if the file exists</returns>
        public static bool TryLoadingFile(string path, out object data)
        {
            data = new object();
            if (FileExists(path)) return false;
            data = LoadFile(path);
            return true;
        }

        #endregion

        #region Delete
        public static void DeleteFile(string path) => File.Delete(path);

        public static void DeleteFolder(string path)
        {
            path = path.Replace('\\', '/');
            if (!Directory.Exists(path)) return;
            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++) DeleteFolder(directories[i]);
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++) File.Delete(files[i]);
            Directory.Delete(path);
        }
        #endregion

        #region Directory
        public static void DeleteDirectory(string path) => Directory.Delete(path);
        public static void CreateDirectory(string path) => Directory.CreateDirectory(path);
        #endregion

        #region Txt
        public static void SaveTxtFile(string path, string data)
        {
            string directory = TrimPathEnd(path, 1);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            StreamWriter writer = new StreamWriter(path);
            writer.Write(data);
            writer.Flush();
            writer.Close();
        }

        public static bool TryLoadTxtFile(string path, out string data)
        {
            data = "";
            if (!File.Exists(path)) return false;
            StreamReader reader = new StreamReader(path);
            data = reader.ReadToEnd();
            reader.Close();
            if (data == null) data = "";
            return true;
        }
        #endregion
    }
}