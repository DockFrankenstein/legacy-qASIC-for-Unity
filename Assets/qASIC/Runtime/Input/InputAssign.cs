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
        public TextMeshProUGUI nameText;
        public bool autoLabel = true;
        public string optionLabelName;
        public string listeningForKeyText = "Press any key";

        [Header("Target")]
        public int playerIndex;
        public InputMapItemReference inputAction;
        public int keyIndex;

        [Header("Options")]
        public string keyRootPath = "key_keyboard";
        [InputKey]
        public List<string> stopListeningKeys = new List<string>()
        {
            "key_keyboard/Escape",
        };

        [Header("Events")]
        public UnityEvent OnStartListening;
        public UnityEvent OnAssign;

        bool isListening = false;

        private void Reset()
        {
#if UNITY_EDITOR
            Button button = GetComponent<Button>();

            if (button != null)
                UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, StartListening);
#endif

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
                key = key.ToLower();

                if (!key.StartsWith(keyRootPath.ToLower())) continue;

                if (stopListeningKeys.Select(x => x.ToLower()).Contains(key))
                {
                    StopListening();
                    break;
                }

                Assign(key);
                break;
            }
        }

        public string GetLabel()
        {
            string currentKey = "UNKNOWN";
            if (InputManager.TryGetPlayer(playerIndex, out Players.InputPlayer player))
            {
                var binding = player.MapData.GetItemData<Map.ItemData.InputBindingData>(inputAction.Guid);

                if (binding != null && binding.keys.IndexInRange(keyIndex))
                {
                    currentKey = binding.keys[keyIndex].Split('/').Last();

                    if (autoLabel)
                        return $"{player.Map.GetItem<InputBinding>(inputAction.Guid).ItemName}: {currentKey}";
                }
            }

            return $"{optionLabelName}{currentKey}";
        }

        public void StartListening()
        {
            isListening = true;
            OnStartListening.Invoke();
        }

        public void StopListening()
        {
            isListening = false;
        }

        public void Assign(string key)
        {
            InputManager.ChangeInput(inputAction.GetGroupName(), inputAction.GetItemName(), keyIndex, key);
            StopListening();
            OnAssign.Invoke();
        }
    }
}