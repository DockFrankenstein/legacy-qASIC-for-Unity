using UnityEngine;
using qASIC.Input.Map;

namespace qASIC.Input
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [AddComponentMenu("qASIC/Input/Input Load")]
    public class InputLoad : MonoBehaviour
    {
        [Message("This feature has not been reimplemented yet :/", InspectorMessageIconType.error)]
        [SerializeField] InputMap map;

        private static bool init = false;

#if UNITY_EDITOR
        private static InputMap _editorMap = null;
        public static InputMap EditorMap { get; private set; }
        public static System.Action<InputMap> OnEditorMapChange;
#endif

        private void Start()
        {
            if (init) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                InitializeEditor();
                return;
            }
#endif
            InitializeRuntime();

            init = true;
        }

        void InitializeRuntime()
        {
            if (!map)
            {
                qDebug.LogError("Cannot load Input Map: Input Map has not been assigned!");
                return;
            }
        }

#if UNITY_EDITOR
        void InitializeEditor() =>
            AssignEditorMap();

        private void OnValidate() =>
            AssignEditorMap();

        void AssignEditorMap()
        {
            EditorMap = map;
            OnEditorMapChange?.Invoke(map);
        }
#endif
    }
}