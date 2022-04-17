using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace qASIC.InputManagement.Menu
{
    [AddComponentMenu("qASIC/Options/Input Asign")]
    public class InputAssign : MonoBehaviour
    {
        [Header("Updating name")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] string optionLabelName;

        [Header("Options")]
        [SerializeField] InputActionReference inputAction;
        [SerializeField] int keyIndex;

        [Header("Events")]
        [SerializeField] UnityEvent OnStartListening;
        [SerializeField] UnityEvent OnAssign;

        bool isListening = false;

        private void Awake()
        {
            Button button = GetComponent<Button>();
            if (button == null) return;
            button.onClick.AddListener(StartListening);
        }

        private void Update()
        {
            if (nameText != null)
                nameText.text = GetLabel();

            if (isListening)
                ListenForKey();
        }

        private void ListenForKey()
        {
            if (!Input.anyKeyDown) return;
            for (int i = 0; i < KeyboardManager.AllKeyCodes.Length; i++)
            {
                if (!Input.GetKeyDown(KeyboardManager.AllKeyCodes[i])) continue;
                Assign(KeyboardManager.AllKeyCodes[i]);
                return;
            }
        }

        public string GetLabel()
        {
            string currentKey = "UNKNOWN";
            if (InputManager.TryGetKeyCode(inputAction.GroupName, inputAction.ActionName, keyIndex, out KeyCode key, false))
                currentKey = key.ToString();
            return $"{optionLabelName}{currentKey}";
        }

        public void StartListening()
        {
            isListening = true;
            OnStartListening.Invoke();
        }

        public void Assign(KeyCode key)
        {
            InputManager.ChangeInput(inputAction.GroupName, inputAction.ActionName, keyIndex, key);
            isListening = false;
            OnAssign.Invoke();
        }
    }
}