using qASIC.InputManagement;
using UnityEngine;
using qASIC.Displayer;
using qASIC.Demo.Dialogue;

namespace qASIC.Demo
{
	public class PlayerController : MonoBehaviour
	{
        public static bool Freeze = false;

        public float Speed = 6f;

		Rigidbody2D rb;

        public static float SpeedMultiplier { get; set; } = 1f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (Freeze)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            rb.velocity = new Vector2(InputManager.GetAxis("Right", "Left"), InputManager.GetAxis("Up", "Down")) * Speed * SpeedMultiplier;
            InfoDisplayer.DisplayValue("pos", VectorText.ToText(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        }

        Interactable currentInteractable;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.name.StartsWith("INTER")) return;
            Interactable interactable = collision.GetComponent<Interactable>();
            if (interactable == null) return;
            if (currentInteractable != null) currentInteractable.ChangeState(false);
            currentInteractable = interactable;
            currentInteractable.ChangeState(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.name.StartsWith("INTER")) return;
            Interactable interactable = collision.GetComponent<Interactable>();
            interactable.ChangeState(false);
            if (interactable == currentInteractable) currentInteractable = null;
        }
    }
}