using qASIC.InputManagement;
using UnityEngine;

namespace qASIC.Demo.Dialogue
{
	public class Interactable : MonoBehaviour
	{
		public GameObject Toggable;
		public string[] dialogue;

		bool isActive = false;

		public void ChangeState(bool state)
        {
			Toggable.SetActive(state);
			isActive = state;
		}

        private void Update()
        {
			if (!isActive || DialogueController.Active) return;
			if (InputManager.GetInputDown("Interact")) OnInteract();
		}

		void OnInteract()
        {
			DialogueController.singleton?.DisplayDialogue(dialogue);
        }
    }
}