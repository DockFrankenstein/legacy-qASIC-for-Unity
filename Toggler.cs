using UnityEngine;

public class Toggler : MonoBehaviour
{
    [HideInInspector] public bool state;

    public KeyCode Key = KeyCode.F2;
    public GameObject ToggleObject;

    private void Awake() => Toggle(ToggleObject.activeSelf);

    private void Update()
    {
        if (Input.GetKeyDown(Key))
            Toggle();
    }

    public void Toggle() => Toggle(!state);

    public void Toggle(bool state)
    {
        this.state = state;
        ToggleObject?.SetActive(state);
    }
}
