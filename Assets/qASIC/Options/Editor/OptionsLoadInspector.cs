#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools.Internal;
using qASIC.FileManagement;

using static UnityEditor.EditorGUILayout;

namespace qASIC.Options.Internal
{
    [CustomEditor(typeof(OptionsLoad))]
    public class OptionsLoadInspector : Editor
    {
        SerializedProperty serializationTypeProperty;
        SerializedProperty pathProperty;

        private void OnEnable()
        {
            serializationTypeProperty = serializedObject.FindProperty("serializationType");
            pathProperty = serializedObject.FindProperty("filePath");
        }

        public override void OnInspectorGUI()
        {
            qGUIInternalUtility.DrawqASICBanner(docs: "https://docs.qasictools.com/docs/options");

            PropertyField(serializationTypeProperty);
            if ((SerializationType)serializationTypeProperty.intValue == SerializationType.config)
            {
                Space();
                PropertyField(pathProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif