using System;

namespace qASIC.FileManagement
{
    [Serializable]
    public struct AdvancedGenericFilePath
    {
        public GenericFilePath buildPath;

        public bool separateEditorPath;
        public GenericFilePath editorPath;

        public AdvancedGenericFilePath(string path)
        {
            buildPath = new GenericFilePath(GenericFolder.PersistentDataPath, path);
            separateEditorPath = false;
            editorPath = new GenericFilePath(GenericFolder.PersistentDataPath, path);
        }

        public AdvancedGenericFilePath(GenericFolder folder, string path)
        {
            buildPath = new GenericFilePath(folder, path);
            separateEditorPath = false;
            editorPath = new GenericFilePath(folder, path);
        }

        public AdvancedGenericFilePath(string path, string editorPath)
        {
            buildPath = new GenericFilePath(GenericFolder.PersistentDataPath, path);
            separateEditorPath = true;
            this.editorPath = new GenericFilePath(GenericFolder.PersistentDataPath, editorPath);
        }

        public AdvancedGenericFilePath(GenericFolder folder, string path, string editorPath)
        {
            buildPath = new GenericFilePath(folder, path);
            separateEditorPath = true;
            this.editorPath = new GenericFilePath(folder, editorPath);
        }

        public string GetFullPath()
        {
#if UNITY_EDITOR
            return (separateEditorPath ? editorPath : buildPath).GetFullPath();
#else
            return buildPath.GetFullPath();
#endif
        }

        public override string ToString() =>
            GetFullPath();
    }
}