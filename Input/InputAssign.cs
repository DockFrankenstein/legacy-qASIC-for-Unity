using UnityEngine;
using UnityEngine.Events;

namespace qASIC.InputManagment.Menu
{
    public class InputAssign : MonoBehaviour
    {
        public string KeyName;
        public TMPro.TextMeshProUGUI keyText;

        public InputListiner Listiner;
        public UnityEvent OnStartListening = new UnityEvent();
        public UnityEvent OnAssign = new UnityEvent();

        private UnityAction<KeyCode> listinerAction;

        private void Awake() => listinerAction = Assign;

        private void Update()
        {
            if (keyText != null && InputManager.GlobalKeys.Presets.ContainsKey(KeyName)) 
                keyText.text = InputManager.GlobalKeys.Presets[KeyName].ToString();
        }

        public void StartListening()
        {
            if (Listiner == null)
            {
                qDebug.LogError("Listiner is not assigned!");
                return;
            }
            OnStartListening.Invoke();
            Listiner.onInputRecived.AddListener(listinerAction);
            Listiner.StartListening(true, false);
        }

        public void Assign(KeyCode key)
        {
            InputManager.ChangeInput(KeyName, key);
            OnAssign.Invoke();

            if (Listiner != null) Listiner.onInputRecived.RemoveListener(listinerAction);
        }
    }
}