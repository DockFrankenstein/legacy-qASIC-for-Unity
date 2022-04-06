using UnityEngine;

namespace qASIC
{
    public class AddToDontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}