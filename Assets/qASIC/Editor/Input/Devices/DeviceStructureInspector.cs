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
            p_handlers = serializedObject.FindProperty(nameof(DeviceStructure.providers));
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
                    int startIndent = EditorGUI.indentLevel;
                    int startDepth = item.depth;
                    while (item.NextVisible(true) && item.depth > startDepth)
                    {
                        EditorGUI.indentLevel = item.depth - startDepth + startIndent;
                        EditorGUILayout.PropertyField(item);
                    }
                    EditorGUI.indentLevel = startIndent;
                }

                GUI.Box(StreachRectToSides(GUILayoutUtility.GetRect(GUIContent.none, Styles.s_lineStyle)), GUIContent.none, Styles.s_lineStyle);
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
            var rect = GUILayoutUtility.GetRect(GUIContent.none, Styles.s_foldoutBackgroundStyle);
            var backgroundRect = StreachRectToSides(rect).Border(0f, -1f);
            EditorGUI.LabelField(backgroundRect, GUIContent.none, Styles.s_foldoutBackgroundStyle);
            var foldoutRect = rect;
            foldoutRect.xMax -= 20;
            foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, true, Styles.s_foldoutStyle);

            return rect;
        }

        private static Rect StreachRectToSides(Rect r) =>
            r.Border(-18f, -4f, 0f, 0f);

        void OpenAddGenericMenu()
        {
            GenericMenu menu = new GenericMenu();

            var types = TypeFinder.FindAllTypes<DeviceProvider>()
                .Where(x => !x.ContainsGenericParameters);

            foreach (var type in types)
            {
                menu.AddItem(type.Name.Split('.').Last(), false, () =>
                {
                    AddHandler(type);
                    _reloadDeviceManager = true;
                });
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

        void AddHandler(Type type)
        {
            var provider = (DeviceProvider)Activator.CreateInstance(type, new object[] { });
            provider.Name = provider.DefaultItemName;

            _structure.providers.Add(provider);
            serializedObject.UpdateIfRequiredOrScript();
            SaveAssetDatabase(false);
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
            public static GUIStyle s_foldoutBackgroundStyle = new GUIStyle("Label")
                .WithBackground(qGUIUtility.GenerateColorTexture(EditorGUIUtility.isProSkin ? new Color(0.161f, 0.161f, 0.161f) : new Color(0.824f, 0.824f, 0.824f)));
            public static GUIStyle s_foldoutStyle = new GUIStyle("foldout");
            public static GUIStyle s_lineStyle = new GUIStyle()
            {
                fixedHeight = 1f,
                stretchWidth = true,
            }
            .WithBackground(qGUIEditorUtility.BorderTexture);
        }
    }
}