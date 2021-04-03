using UnityEngine;

namespace qASIC
{
    public class Toggler : MonoBehaviour
    {
        [HideInInspector] public bool state;

        public KeyCode Key = KeyCode.F2;
        public GameObject ToggleObject;

        public UnityEventBool OnChangeState;

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
            OnChangeState.Invoke(state);
        }
    }
}