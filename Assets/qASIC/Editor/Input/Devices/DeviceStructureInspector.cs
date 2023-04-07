using UnityEditor;
using UnityEngine;
using qASIC.Tools;
using System.Linq;
using System;
using qASIC.EditorTools;
using qASIC.Input.Map;

namespace qASIC.Input.Devices.Internal
{
    [CustomEditor(typeof(DeviceStructure))]
    public class DeviceStructureInspector : Editor
    {
        DeviceStructure _structure;

        SerializedProperty p_handlers;

        bool _reloadDeviceManager;

        private void OnEnable()
        {
            _structure = (DeviceStructure)target;
            p_handlers = serializedObject.FindProperty("providers");
        }
        
        public override void OnInspectorGUI()
        {
            for (int i = 0; i < p_handlers.arraySize; i++)
            {
                SerializedProperty item = p_handlers.GetArrayElementAtIndex(i);
                bool foldout = true;
                Rect rect = CreateFoldout(ref foldout);
                Rect labelRect = new Rect(rect).BorderRight(18f);
                Rect menuRect = new Rect(rect).ResizeToRight(16f);
                GUI.Label(labelRect, item.FindPropertyRelative("name").stringValue);

                if (GUI.Button(menuRect, GUIContent.none, EditorStyles.foldoutHeaderIcon))
                    OpenProviderMenu(i);

                if (foldout)
                {
                    qGUIEditorUtility.DrawPropertyLayout(item);
                    //int startIndent = EditorGUI.indentLevel;
                    //int startDepth = item.depth;
                    //while (item.NextVisible(true) && item.depth > startDepth)
                    //{
                    //    EditorGUI.indentLevel = item.depth - startDepth + startIndent;
                    //    EditorGUILayout.PropertyField(item);
                    //}
                    //EditorGUI.indentLevel = startIndent;
                }

                GUI.Box(StreachRectToSides(GUILayoutUtility.GetRect(GUIContent.none, Styles.LineStyle)), GUIContent.none, Styles.LineStyle);
            }

            if (GUILayout.Button("Add provider"))
                OpenAddGenericMenu();

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();

            if (_reloadDeviceManager)
            {
                _reloadDeviceManager = false;
                DeviceManager.Reload();
            }
        }

        private static Rect CreateFoldout(ref bool foldout)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, Styles.FoldoutBackgroundStyle);
            var backgroundRect = StreachRectToSides(rect).Border(0f, -1f);
            EditorGUI.LabelField(backgroundRect, GUIContent.none, Styles.FoldoutBackgroundStyle);
            var foldoutRect = rect;
            foldoutRect.xMax -= 20;
            foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, true, Styles.FoldoutStyle);

            return rect;
        }

        private static Rect StreachRectToSides(Rect r) =>
            r.Border(-18f, -4f, 0f, 0f);

        void OpenAddGenericMenu()
        {
            GenericMenu menu = new GenericMenu();

            var types = TypeFinder.FindAllTypes<DeviceProvider>()
                .Where(x => !x.ContainsGenericParameters);

            var addedProviderTypes = _structure.Providers
                .Select(x => x.GetType())
                .ToList();

            foreach (var type in types)
            {
                menu.AddToggableItem(type.Name.Split('.').Last(), false, () =>
                {
                    _structure.AddHandler(type);

                    serializedObject.Update();
                    SaveAssetDatabase();
                    _reloadDeviceManager = true;
                }, !addedProviderTypes.Contains(type));
            }

            menu.ShowAsContext();
        }

        void OpenProviderMenu(int index)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddToggableItem("Move Up", false, () => { p_handlers.MoveArrayElement(index, index - 1); SaveAssetDatabase(); }, index > 0);
            menu.AddToggableItem("Move Down", false, () => { p_handlers.MoveArrayElement(index, index + 1); SaveAssetDatabase(); }, index < p_handlers.arraySize - 1);
            menu.AddSeparator("");
            menu.AddItem("Delete", false, () =>
            {
                p_handlers.DeleteArrayElementAtIndex(index);
                _reloadDeviceManager = true;
                SaveAssetDatabase();
            });

            menu.ShowAsContext();
        }

        void SaveAssetDatabase(bool applySerializableObject = true)
        {
            if (applySerializableObject)
                serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(_structure);
            AssetDatabase.SaveAssets();
        }

        private static class Styles
        {
            public static GUIStyle FoldoutBackgroundStyle => new GUIStyle("Label")
                .WithBackground(qGUIUtility.GenerateColorTexture(EditorGUIUtility.isProSkin ? new Color(0.161f, 0.161f, 0.161f) : new Color(0.824f, 0.824f, 0.824f)));
            public static GUIStyle FoldoutStyle => new GUIStyle("foldout");
            public static GUIStyle LineStyle => new GUIStyle()
            {
                fixedHeight = 1f,
                stretchWidth = true,
            }
            .WithBackground(qGUIEditorUtility.BorderTexture);
        }
    }
}