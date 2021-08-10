using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace qASIC.InputManagement.Menu
{
    public class InputAssign : MonoBehaviour
    {
        [Header("Updating name")]
        public TextMeshProUGUI nameText;
        public string optionLabelName;

        [Header("Options")]
        public string keyName;

        [Header("Listener")]
        public InputListener listener;
        public UnityEvent OnStartListening = new UnityEvent();
        public UnityEvent OnAssign = new UnityEvent();

        UnityAction<KeyCode> listinerAction;

        private void Awake()
        {
            listinerAction = Assign;

            Button button = GetComponent<Button>();
            if (button == null) return;
            button.onClick.AddListener(StartListening);
        }

        private void Update()
        {
            if (nameText != null)
                nameText.text = GetLabel();
        }

        public string GetLabel()
        {
            string currentKey = "none";
            if (InputManager.GlobalKeys.Presets.ContainsKey(keyName)) 
                currentKey = InputManager.GlobalKeys.Presets[keyName].ToString();
            return $"{optionLabelName}{currentKey}";
        }

        public void StartListening()
        {
            if (listener == null)
            {
                qDebug.LogError("Listiner is not assigned!");
                return;
            }
            OnStartListening.Invoke();
            listener.OnInputRecived.AddListener(listinerAction);
            listener.StartListening(true, false);
        }

        public void Assign(KeyCode key)
        {
            InputManager.ChangeInput(keyName, key);
            OnAssign.Invoke();

            if (listener != null) listener.OnInputRecived.RemoveListener(listinerAction);
        }
    }
}