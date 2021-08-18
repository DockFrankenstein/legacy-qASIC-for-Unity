using UnityEngine;
using qASIC.InputManagement;

namespace qASIC.Demo
{
	public class DialogueController : MonoBehaviour
	{
		public static DialogueController singleton;

        string[] currentDialogue;
        int index;

        public static bool Active { get; private set; }

        public GameObject toggleObject;
        public TMPro.TextMeshProUGUI text;

        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
                return;
            }
            qDebug.LogWarning("There are multiple instances of the dialogue controller in the scene!");
            Destroy(gameObject);
        }

        public void DisplayDialogue(string[] dialogue)
        {
            if (dialogue.Length == 0) return;

            Active = true;

            toggleObject.SetActive(true);
            index = -1;
            currentDialogue = dialogue;
            PlayerController.freeze = true;
        }

        private void LateUpdate()
        {
            if (!Active) return;
            if (InputManager.GetInputDown("Interact")) Next();
        }

        public void Next()
        {
            index++;
            if(index >= currentDialogue.Length)
            {
                HideDialogue();
                return;
            }

            text.text = currentDialogue[index];
        }

        public void HideDialogue()
        {
            Active = false;

            toggleObject.SetActive(false);
            index = 0;
            currentDialogue = new string[0];
            text.text = string.Empty;
            PlayerController.freeze = false;
        }
    }
}