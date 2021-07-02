using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;

namespace qASIC.FileManagement
{
    public static class FileManager
    {
        public static string GetCustomFolderPath(Environment.SpecialFolder type) { return Environment.GetFolderPath(type); }

        #region Trim
        /// <summary>Trims paths end by specified amount of folders</summary>
        /// <param name="amount">Amount of folders to trim</param>
        public static string TrimPathEnd(string path, int amount) 
        {
            path = path.Replace('\\', '/');
            amount = Mathf.Abs(amount);
            string[] folders = path.Split('/');

            string trimmedPath = string.Empty;
            for (int i = 0; i < folders.Length - amount; i++) trimmedPath += $"{folders[i]}/";
            return trimmedPath.TrimEnd('/');
        }

        /// <summary>Trims paths start by specified amount of folders</summary>
        /// <param name="amount">Amount of folders to trim</param>
        public static string TrimPathStart(string path, int amount)
        {
            path = path.Replace('\\', '/');
            amount = Mathf.Abs(amount);
            string[] folders = path.Split('/');

            string trimmedPath = string.Empty;
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

        #region Binary
        private static void SaveBinary(string path, object data)
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

        private static void LoadBinary(string path, out object data)
        {
            FileStream fileStream = File.Open(path, FileMode.Open);
            BinaryFormatter formater = new BinaryFormatter();
            data = formater.Deserialize(fileStream);
            fileStream.Close();
        }


        public static void SaveFileBinary(string path, object data)
        {
            try
            {
                SaveBinary(path, data);
            }
            catch (Exception e)
            {
                qDebug.LogError($"Couldn't save file. Exception: {e}");
            }
        }

        public static bool TrySaveFileBinary(string path, object data)
        {
            try
            {
                SaveBinary(path, data);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static object LoadFileBinary(string path)
        {
            object data = new object();
            try
            {
                LoadBinary(path, out data);
            }
            catch (Exception e)
            {
                qDebug.LogError($"Couldn't load file. Exception: {e}");
            }
            return data;
        }

        public static bool TryLoadFileBinary(string path, out object data)
        {
            data = new object();
            try
            {
                LoadBinary(path, out data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region JSON
        private static void SaveJSON(string path, object data, bool preattyPrint)
        {
            SaveWriter(path, JsonUtility.ToJson(data, preattyPrint));
        }

        private static void LoadJSON(string path, object objectToOverride)
        {
            JsonUtility.FromJsonOverwrite(LoadWriter(path), objectToOverride);
        }


        /// <param name="preattyPrint">If the file should be saved for readability or minimum size</param>
        public static void SaveFileJSON(string path, object data, bool preattyPrint = false)
        {
            try
            {
                SaveJSON(path, data, preattyPrint);
            }
            catch (Exception e)
            {
                qDebug.LogError($"Couldn't save file: {e}");
            }
        }

        /// <param name="preattyPrint">If the file should be saved for readability or minimum size</param>
        public static bool TrySaveFileJSON(string path, object data, bool preattyPrint = false)
        {
            try
            {
                SaveJSON(path, data, preattyPrint);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static void ReadFileJSON(string path, object objectToOverride)
        {
            try
            {
                LoadJSON(path, objectToOverride);
            }
            catch (Exception e)
            {
                qDebug.LogError($"Couldn't load file: {e}");
            }
        }

        public static bool TryReadFileJSON(string path, object objectToOverride)
        {
            try
            {
                LoadJSON(path, objectToOverride);
            }
            catch
            {
                return false;
            }
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

        #region Writer
        private static void SaveWriter(string path, string data)
        {
            string directory = TrimPathEnd(path, 1);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            StreamWriter writer = new StreamWriter(path);
            writer.Write(data);
            writer.Flush();
            writer.Close();
        }

        private static string LoadWriter(string path)
        {
            StreamReader reader = new StreamReader(path);
            string data = reader.ReadToEnd();
            reader.Close();
            if (data == null) data = string.Empty;
            return data;
        }


        public static void SaveFileWriter(string path, string data)
        {
            try
            {
                SaveWriter(path, data);
            }
            catch (Exception e)
            {
                qDebug.LogError($"Couldn't save file. Exception: {e}");
            }
        }

        public static bool TrySaveFileWriter(string path, string data)
        {
            try
            {
                SaveWriter(path, data);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static string LoadFileWriter(string path)
        {
            string data = string.Empty;
            try
            {
                data = LoadWriter(path);
            }
            catch (Exception e)
            {
                qDebug.LogError($"Couldn't save file. Exception: {e}");
            }
            return data;
        }

        public static bool TryLoadFileWriter(string path, out string data)
        {
            data = string.Empty;
            try
            {
                data = LoadWriter(path);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}