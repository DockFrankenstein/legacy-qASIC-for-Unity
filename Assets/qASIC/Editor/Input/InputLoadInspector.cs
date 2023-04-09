#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools.Internal;

using static UnityEditor.EditorGUILayout;

namespace qASIC.Input.Internal
{
    [CustomEditor(typeof(InputLoad))]
    public class InputLoadInspector : Editor
    {
        SerializedProperty mapProperty;

        private void OnEnable()
        {
            mapProperty = serializedObject.FindProperty("map");
        }

        public override void OnInspectorGUI()
        {
            qGUIInternalUtility.DrawqASICBanner(docs: "https://docs.qasictools.com/docs/input");

            PropertyField(mapProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif