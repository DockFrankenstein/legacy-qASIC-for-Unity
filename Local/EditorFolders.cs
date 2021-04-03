#if UNITY_EDITOR
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

public class EditorFolders : MonoBehaviour
{
    [MenuItem("Folder/Data")]
    static void OpenApplicationDataPath()
    {
        OpenFolder(Application.persistentDataPath);
    }

    [MenuItem("Folder/Project")]
    static void OpenProjectDataPath()
    {
        OpenFolder(qASIC.FileManagement.FileManager.TrimPathEnd(Application.dataPath, 1));
    }

    private static void OpenFolder(string path)
    {
        Process.Start(path);
    }
}
#endif