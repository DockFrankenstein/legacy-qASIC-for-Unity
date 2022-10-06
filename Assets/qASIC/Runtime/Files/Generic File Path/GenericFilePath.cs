using System;

namespace qASIC.FileManagement
{
    [Serializable]
    public struct GenericFilePath
    {
        public GenericFolder genericFolder;
        public string filePath;

        public GenericFilePath(string filePath)
        {
            this.filePath = filePath;
            genericFolder = GenericFolder.PersistentDataPath;
        }

        public GenericFilePath(GenericFolder genericFolder, string filePath)
        {
            this.genericFolder = genericFolder;
            this.filePath = filePath;
        }

        public string GetFullPath() =>
            GenerateFullPath(genericFolder, filePath);

        public static string GenerateFullPath(GenericFolder genericFolder, string filePath) =>
            $@"{FileManager.GetGenericFolderPath(genericFolder)}\{filePath}".Replace('/', '\\');

        public override string ToString() =>
            GetFullPath();
    }
}