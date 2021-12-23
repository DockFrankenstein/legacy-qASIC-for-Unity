using System;

namespace qASIC.FileManagement
{
    [Serializable]
    public class GenericFilePath
    {
        public Environment.SpecialFolder specialFolder = Environment.SpecialFolder.ApplicationData;
        public string filePath;

        public GenericFilePath() { }

        public GenericFilePath(string filePath)
        {
            this.filePath = filePath;
        }

        public GenericFilePath(Environment.SpecialFolder specialFolder, string filePath)
        {
            this.specialFolder = specialFolder;
            this.filePath = filePath;
        }

        public string GetFullPath() =>
            GenerateFullPath(specialFolder, filePath);

        public static string GenerateFullPath(Environment.SpecialFolder specialFolder, string filePath) =>
            $@"{FileManager.GetCustomFolderPath(specialFolder)}\{filePath}";

        public override string ToString() =>
            GetFullPath();
    }
}