using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using qASIC.Input.Devices;
using qASIC.Input.Map;
using System.Linq;
using System.Collections.Generic;

namespace qASIC.Input.Menu
{
    [AddComponentMenu("qASIC/Options/Input Asign")]
    public class InputAssign : MonoBehaviour
    {
        [Header("Updating name")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] string optionLabelName;
        [SerializeField] string listeningForKeyText = "Listening for key";

        [Header("Options")]
        [SerializeField] int playerIndex;
        [SerializeField] InputMapItemReference inputAction;
        [SerializeField] int keyIndex;
        [SerializeField] string keyRootPath = "key_keyboard";

        [Header("Events")]
        [SerializeField] UnityEvent OnStartListening;
        [SerializeField] UnityEvent OnAssign;

        bool isListening = false;

        private void Reset()
        {
            Button button = GetComponent<Button>();

            if (button != null)
                UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, StartListening);

            nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            if (nameText != null)
                nameText.text = isListening ? listeningForKeyText : GetLabel();

            if (isListening)
                ListenForKey();
        }

        private void ListenForKey()
        {
            List<IInputDevice> devices = InputManager.Players[playerIndex].CurrentDevices;

            foreach (var device in devices)
            {
                string key = device.GetAnyKeyDown();
                if (string.IsNullOrEmpty(key)) continue;
                if (key.StartsWith(keyRootPath)) continue;

                Assign(key);
                break;
            }
        }

        public string GetLabel()
        {
            string currentKey = "UNKNOWN";
            //FIXME
            //if (InputManager.TryGetPlayer(playerIndex, out Players.InputPlayer player) &&
            //    player.MapData.(inputAction.Guid, out InputMapItem item) &&
            //    item is InputBinding binding)
            //    currentKey = binding.keys[keyIndex].Split('/').Last();

            return $"{optionLabelName}{currentKey}";
        }

        public void StartListening()
        {
            isListening = true;
            OnStartListening.Invoke();
        }

        public void Assign(string key)
        {
            InputManager.ChangeInput(inputAction.GetGroupName(), inputAction.GetItemName(), keyIndex, key);
            isListening = false;
            OnAssign.Invoke();
        }
    }
}