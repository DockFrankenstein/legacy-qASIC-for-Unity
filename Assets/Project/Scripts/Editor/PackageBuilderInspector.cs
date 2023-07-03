using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Project.Internal
{
    [CustomEditor(typeof(PackageBuilder))]
    public class PackageBuilderInspector : Editor
    {
        PackageBuilder script;
        ReorderableList l_components;

        List<PackageBuilder.System> detectedComponents = new List<PackageBuilder.System>();

        [NonSerialized] bool _init = false;

        void Initialize()
        {
            _init = true;
            script = (PackageBuilder)target;

            var componentProperty = serializedObject.FindProperty(nameof(PackageBuilder.systems));
            l_components = new ReorderableList(serializedObject, componentProperty);

            l_components.elementHeightCallback += (int index) =>
            {
                return EditorGUI.GetPropertyHeight(componentProperty.GetArrayElementAtIndex(index));
            };

            l_components.drawHeaderCallback += (Rect rect) =>
            {
                GUI.Label(rect, "Systems");
            };

            l_components.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                EditorGUI.PropertyField(rect, componentProperty.GetArrayElementAtIndex(index), true);
            };

            AutoDetectPackages($"{Application.dataPath}/{script.systemsRootPath}");
        }

        public override void OnInspectorGUI()
        {
            if (!_init)
                Initialize();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Systems Root", EditorStyles.boldLabel);
                using (new GUILayout.HorizontalScope())
                {
                    using (var changeCheck = new EditorGUI.ChangeCheckScope())
                    {
                        script.systemsRootPath = EditorGUILayout.TextField(string.Empty, script.systemsRootPath);

                        if (GUILayout.Button("R", GUILayout.Width(EditorGUIUtility.singleLineHeight)) || changeCheck.changed)
                            AutoDetectPackages($"{Application.dataPath}/{script.systemsRootPath}");
                    }

                    if (GUILayout.Button("Detect", GUILayout.Width(64f)))
                        ApplyDetectedPackages();
                }

                foreach (var component in detectedComponents)
                    GUILayout.Label(component.path);
            }

            EditorGUILayout.Space();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(PackageBuilder.persistentFilesPath)), new GUIContent("Persistent Files"));
                EditorGUILayout.LabelField($"{Application.dataPath}/{script.persistentFilesPath}");
            }

            EditorGUILayout.Space();


            l_components.DoLayoutList();

            GUILayout.Label("Generation result", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(PackageBuilder.generatedCollectionPath)), new GUIContent("Generated Collection"));
                EditorGUILayout.LabelField($"{Application.dataPath}/{script.generatedCollectionPath}");
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Entire Collection", GUILayout.Height(32f)))
                GenerateCollection();

            var defaultGUIColor = GUI.color;

            GUI.color = Color.red;
            if (GUILayout.Button("Revert Collection"))
                RevertCollection();

            GUI.color = defaultGUIColor;

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        public void AutoDetectPackages(string path)
        {
            path = path.Replace('\\', ',');

            detectedComponents.Clear();

            if (!Directory.Exists(path))
                path = $"{path}~";

            if (!Directory.Exists(path))
                return;

            var systemPaths = Directory.GetDirectories(path)
                .Select(x => x.Replace('\\', '/'));

            foreach (var systemPath in systemPaths)
            {
                var component = new PackageBuilder.System()
                {
                    name = systemPath.Split('/').Last(),
                    path = systemPath.Substring(Application.dataPath.Length + 1, systemPath.Length - Application.dataPath.Length - 1),
                };

                detectedComponents.Add(component);
            }
        }

        public void ApplyDetectedPackages()
        {
            foreach (var component in detectedComponents)
            {
                if (!script.systems
                    .Select(x => x.name)
                    .Contains(component.name))
                {
                    script.systems.Add(component);
                    continue;
                }

                switch (EditorUtility.DisplayDialog("System is already registered", $"System of name '{component.name}' already exists in the builder.", "Replace", "Cancel"))
                {
                    //Replace
                    case true:
                        var itemToReplace = script.systems
                            .Where(x => x.name == component.name)
                            .FirstOrDefault();

                        int index = script.systems.IndexOf(itemToReplace);

                        script.systems[index] = component;
                        break;
                    //Cancel
                    case false:
                        break;
                }
            }

            serializedObject.Update();
        }

        public void GenerateCollection()
        {
            var systemsRootPath = $"{Application.dataPath}/{script.systemsRootPath}";
            if (Directory.Exists($"{systemsRootPath}~"))
            {
                Debug.LogError($"Cannot generate collection, {systemsRootPath}~ already exists!");
                return;
            }

            if (!EditorUtility.DisplayDialog("Are you sure?", "Do you want to generate the entire collection?", "Yes", "No"))
                return;

            var path = $"{Application.dataPath}/{script.generatedCollectionPath}";

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);

            if (File.Exists($"{path}.meta~"))
                File.Move($"{path}.meta~", $"{path}.meta");

            var folders = script.systems
                .SelectMany(x => Directory.GetDirectories($"{Application.dataPath}/{x.path}"))
                .Distinct()
                .Select(x => x.Replace('\\', '/').Split('/').Last())
                .ToList();

            foreach (var item in folders)
            {
                Directory.CreateDirectory($"{path}/{item}");
                Debug.Log($"Creating directory `{path}/{item}`");
            }

            var packagesRootPath = script.systemsRootPath.Replace('\\', '/');
            foreach (var system in script.systems)
            {
                var systemPath = $"{Application.dataPath}/{system.path.Replace('\\', '/')}";
                if (!Directory.Exists(systemPath))
                    systemPath = systemPath.Replace(packagesRootPath, $"{packagesRootPath}~");

                if (!Directory.Exists(systemPath))
                {
                    Debug.LogError($"Unable to find system under {system.path}");
                    continue;
                }

                foreach (var directory in Directory.GetDirectories(systemPath))
                {
                    var directoryName = directory.Replace('\\', '/').Split('/').Last();
                    CopyDirectoryContents(directory, $"{path}/{directoryName}/{system.name}", new string[]
                    {
                        ".asmdef",
                        ".asmdef.meta",
                    });
                }
            }

            Directory.Move(systemsRootPath, $"{systemsRootPath}~");

            if (File.Exists($"{systemsRootPath}.meta"))
                File.Move($"{systemsRootPath}.meta", $"{systemsRootPath}.meta~");

            CopyDirectoryContents($"{Application.dataPath}/{script.persistentFilesPath}", path);

            AssetDatabase.Refresh();
        }

        public void RevertCollection()
        {
            if (!EditorUtility.DisplayDialog("Are you sure?", "Do you want to revert the collection?", "Yes", "No"))
                return;

            var collectionPath = $"{Application.dataPath}/{script.generatedCollectionPath}";
            var systemsPath = $"{Application.dataPath}/{script.systemsRootPath}";

            if (Directory.Exists(collectionPath))
                Directory.Delete(collectionPath, true);

            if (File.Exists($"{collectionPath}.meta"))
                File.Move($"{collectionPath}.meta", $"{collectionPath}.meta~");

            if (Directory.Exists($"{systemsPath}~"))
                Directory.Move($"{systemsPath}~", systemsPath);

            if (File.Exists($"{systemsPath}.meta~"))
                File.Move($"{systemsPath}.meta~", $"{systemsPath}.meta");

            AssetDatabase.Refresh();
        }

        static void CopyDirectoryContents(string sourcePath, string targetPath) =>
            CopyDirectoryContents(sourcePath, targetPath, new string[0]);

        static void CopyDirectoryContents(string sourcePath, string targetPath, string[] blacklistedFormats)
        {
            Directory.CreateDirectory(targetPath);

            foreach (var path in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(path.Replace(sourcePath, targetPath));

            foreach (var path in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
            {
                bool ignore = false;
                foreach (var format in blacklistedFormats)
                {
                    if (!path.EndsWith(format)) continue;
                    ignore = true;
                    break;
                }

                if (ignore)
                    continue;

                File.Copy(path, path.Replace(sourcePath, targetPath), true);
            }
        }
    }
}