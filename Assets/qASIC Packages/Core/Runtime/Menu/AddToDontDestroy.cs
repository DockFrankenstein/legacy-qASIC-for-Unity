using UnityEngine;

namespace qASIC
{
    [AddComponentMenu("qASIC/Other/Add To Dont Destroy")]
    public class AddToDontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}