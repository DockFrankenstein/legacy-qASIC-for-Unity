#if UNITY_EDITOR
using UnityEditor;
using qASIC.FileManagement;
using qASIC.EditorTools.Internal;

using static UnityEditor.EditorGUILayout;

namespace qASIC.InputManagement.Internal
{
    [CustomEditor(typeof(InputLoad))]
    public class InputLoadInspector : Editor
    {
        SerializedProperty mapProperty;
        SerializedProperty serializationTypeProperty;
        SerializedProperty pathProperty;

        private void OnEnable()
        {
            mapProperty = serializedObject.FindProperty("map");
            serializationTypeProperty = serializedObject.FindProperty("serializationType");
            pathProperty = serializedObject.FindProperty("filePath");
        }

        public override void OnInspectorGUI()
        {
            //TODO: change docs link to point to input system
            qGUIInternalUtility.DrawqASICBanner(docs: "https://docs.qasictools.com/docs/input");

            PropertyField(mapProperty);
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