﻿using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace qASIC.InputManagement.Menu
{
    public class InputAssign : MonoBehaviour
    {
        [Header("Updating name")]
        public TextMeshProUGUI NameText;
        public string OptionLabelName;

        [Header("Options")]
        public string KeyName;

        [Header("Listener")]
        public InputListener Listener;
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
            if (NameText != null)
                NameText.text = GetLabel();
        }

        public string GetLabel()
        {
            string currentKey = "none";
            if (InputManager.GlobalKeys.Presets.ContainsKey(KeyName)) 
                currentKey = InputManager.GlobalKeys.Presets[KeyName].ToString();
            return $"{OptionLabelName}{currentKey}";
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