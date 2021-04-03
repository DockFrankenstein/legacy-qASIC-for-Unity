using UnityEngine;
using UnityEngine.Events;

namespace qASIC.InputManagement.Menu
{
    public class InputAssign : MonoBehaviour
    {
        public string KeyName;
        public TMPro.TextMeshProUGUI KeyText;

        public InputListener Listener;
        public UnityEvent OnStartListening = new UnityEvent();
        public UnityEvent OnAssign = new UnityEvent();

        UnityAction<KeyCode> listinerAction;

        private void Awake() => listinerAction = Assign;

        private void Update()
        {
            if (KeyText != null && InputManager.GlobalKeys.Presets.ContainsKey(KeyName)) 
                KeyText.text = InputManager.GlobalKeys.Presets[KeyName].ToString();
        }

        public void StartListening()
        {
            if (Listener == null)
            {
                qDebug.LogError("Listiner is not assigned!");
                return;
            }
            OnStartListening.Invoke();
            Listener.onInputRecived.AddListener(listinerAction);
            Listener.StartListening(true, false);
        }

        public void Assign(KeyCode key)
        {
            InputManager.ChangeInput(KeyName, key);
            OnAssign.Invoke();

            if (Listener != null) Listener.onInputRecived.RemoveListener(listinerAction);
        }
    }
}