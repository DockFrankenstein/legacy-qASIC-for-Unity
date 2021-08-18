using qASIC.InputManagement;
using UnityEngine;
using qASIC.Demo.ColorZones;

namespace qASIC.Demo.Dialogue
{
	public class Interactable : MonoBehaviour
	{
		public TMPro.TextMeshPro text;
		public string[] dialogue;

		public int colorZoneIndex;

		bool isActive = false;

		SpriteRenderer spriteRenderer;

        private void Awake()
        {
			spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void ChangeState(bool state)
        {
			text.gameObject.SetActive(state);
			isActive = state;
		}

        private void Update()
        {
			ChangeColor();
			if (!isActive || DialogueController.Active) return;
			if (InputManager.GetInputDown("Interact")) OnInteract();
		}

		void ChangeColor()
        {
			if (ColorZoneManager.Singleton == null) return;
			if (text != null) text.color = ColorZoneManager.Singleton.current.textColor;
			if (spriteRenderer != null) spriteRenderer.color = ColorZoneManager.Singleton.current.interactableColor;
		}

		void OnInteract()
        {
			DialogueController.singleton?.DisplayDialogue(dialogue);
			ColorZoneManager.Singleton?.ChangeColorZone(colorZoneIndex);
        }
    }
}