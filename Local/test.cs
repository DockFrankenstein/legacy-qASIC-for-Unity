using System.Collections;
using qASIC;
using UnityEngine;
using qASIC.AudioManagment;
using qASIC.FileManagement;

public class test : MonoBehaviour
{
    public AudioData testAudio;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
            AudioManager.Play("test", testAudio);
        if (Input.GetKeyDown(KeyCode.I))
            AudioManager.Pause("test");
        if (Input.GetKeyDown(KeyCode.O))
            AudioManager.UnPause("test");
        if (Input.GetKeyDown(KeyCode.P))
            AudioManager.Stop("test");
        if (Input.GetKeyDown(KeyCode.T))
            FileManager.SaveFileJSON($"{FileManager.GetCustomFolderPath(System.Environment.SpecialFolder.Desktop)}/test.a", new TestSave() { text = "I am beautifull", value = 0f }, true);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestSave s = new TestSave();
            FileManager.ReadFileJSON($"{FileManager.GetCustomFolderPath(System.Environment.SpecialFolder.Desktop)}/test.a", s);
            Debug.Log(s?.text);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) qDebug.Log("Filler message");
    }
}

[System.Serializable]
public class TestSave
{
    public string text;
    public float value;
}