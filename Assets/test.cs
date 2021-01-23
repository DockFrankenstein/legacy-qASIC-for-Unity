using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC;
using qASIC.FileManaging;

public class test : MonoBehaviour
{
    private void Awake()
    {
        FileManager.SaveFile(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/test/test2/test3/lol.test", new object());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            FileManager.DeleteFolder(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/test");
    }
}
