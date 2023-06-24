using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using qASIC.EditorTools;

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
                Rect labelRect = new Rect(rect).BorderRight(36f);
                Rect menuRect = new Rect(rect).ResizeToRight(16f);
                Rect platformRect = new Rect(menuRect).MoveX(-18f);
                GUI.Label(labelRect, item.FindPropertyRelative("name").stringValue);

                if (GUI.Button(platformRect, EditorGUIUtility.IconContent("d_BuildSettings.Standalone"), EditorStyles.label))
                    OpenPlatformMenu(i);

                if (GUI.Button(menuRect, GUIContent.none, EditorStyles.foldoutHeaderIcon))
                    OpenProviderMenu(i);

                if (foldout)
                    qGUIEditorUtility.DrawPropertyLayout(item);

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
            foldoutRect.xMax -= 38;
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
                SaveAssetDatabase();
            });

            menu.ShowAsContext();
        }

        void OpenPlatformMenu(int index)
        {
            var provider = _structure.Providers[index];

            var allFlagValues = (RuntimePlatformFlags[])Enum.GetValues(typeof(RuntimePlatformFlags));

            var supportedPlatforms = allFlagValues
                .Where(x => provider.SupportedPlatforms.HasFlag(x))
                .ToList();

            var everythingPlatform = (RuntimePlatformFlags)supportedPlatforms
                .Cast<int>()
                .Sum();

            GenericMenu menu = new GenericMenu();

            menu.AddItem("None", provider.platforms == RuntimePlatformFlags.None, () =>
            {
                provider.platforms = RuntimePlatformFlags.None;
                SaveAssetDatabase();
            });

            menu.AddItem("Everything", provider.platforms.HasFlag(everythingPlatform), () =>
            {
                provider.platforms = everythingPlatform;
                SaveAssetDatabase();
            });

            foreach (var item in supportedPlatforms)
            {
                if (item == RuntimePlatformFlags.None || item == RuntimePlatformFlags.Everything)
                    continue;

                bool hasFlag = provider.platforms.HasFlag(item);

                menu.AddItem(item.ToString(), hasFlag, () =>
                {
                    switch (hasFlag)
                    {
                        case true:
                            provider.platforms &= ~item;
                            break;
                        case false:
                            provider.platforms |= item;
                            break;
                    }

                    SaveAssetDatabase();
                });
            }

            menu.ShowAsContext();
        }

        void SaveAssetDatabase(bool applySerializableObject = true, bool reloadDeviceManager = true)
        {
            if (applySerializableObject)
                serializedObject.ApplyModifiedProperties();

            if (reloadDeviceManager)
                _reloadDeviceManager = true;

            EditorUtility.SetDirty(_structure);
            AssetDatabase.SaveAssets();

            serializedObject.Update();
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