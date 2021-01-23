using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace qASIC.FileManaging
{
    public static class FileManager
    {
        public static void SaveFile(string path, object data)
        {
            path = path.Replace('\\', '/');

            if (!Directory.Exists(TrimPathEnd(path, 1)))
                Directory.CreateDirectory(TrimPathEnd(path, 1));

            FileStream fileStream = File.Create(path);

            if (data != null)
            {
                BinaryFormatter binFormater = new BinaryFormatter();
                binFormater.Serialize(fileStream, data);
            }

            fileStream.Close();
        }

        public static string TrimPathEnd(string path, int amount) 
        {
            path = path.Replace('\\', '/');
            amount = Mathf.Abs(amount);
            string[] folders = path.Split('/');

            string trimmedPath = "";
            for (int i = 0; i < folders.Length - amount; i++)
            {
                trimmedPath += folders[i] + "/";
            }
            return trimmedPath.TrimEnd('/');
        }

        public static string TrimPathStart(string path, int amount)
        {
            path = path.Replace('\\', '/');
            amount = Mathf.Abs(amount);
            string[] folders = path.Split('/');

            string trimmedPath = "";
            for (int i = 0; i < folders.Length - amount; i++)
            {
                trimmedPath += folders[i + amount] + "/";
            }
            return trimmedPath.TrimEnd('/');
        }

        public static bool DirectoryExists(string path)
        { return Directory.Exists(path.Replace('\\', '/')); }

        public static bool FileExists(string path)
        { return File.Exists(path.Replace('\\', '/')); }

        public static object LoadFile(string path)
        {
            object data = new object();

            FileStream fileStream = File.Open(path, FileMode.Open);
            BinaryFormatter formater = new BinaryFormatter();
            data = formater.Deserialize(fileStream);
            fileStream.Close();

            return data;
        }

        public static bool TryLoadingFile(string path, out object data)
        {
            data = new object();
            if (FileExists(path))
            {
                data = LoadFile(path);
                return true;
            }

            return false;
        }

        public static void DeleteFile(string path) => File.Delete(path);

        public static void DeleteFolder(string path)
        {
            path = path.Replace('\\', '/');

            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                for (int i = 0; i < directories.Length; i++)
                    DeleteFolder(directories[i]);

                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < files.Length; i++)
                    File.Delete(files[i]);

                Directory.Delete(path);
            }
        }

        public static void SaveTxtFile(string path, string data)
        {
            string directory = TrimPathEnd(path, 1);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            StreamWriter writer = new StreamWriter(path);
            writer.Write(data);
            writer.Flush();
            writer.Close();
        }

        public static bool TryLoadTxtFile(string path, out string data)
        {
            data = "";

            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                data = reader.ReadToEnd();
                reader.Close();
                if (data == null)
                    data = "";

                return true;
            }

            return false;
        }
    }
}