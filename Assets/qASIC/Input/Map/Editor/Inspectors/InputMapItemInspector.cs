using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using qASIC.EditorTools;

namespace qASIC.InputManagement.Map.Internal.Inspectors
{
    public class InputMapItemInspector
    {
        public virtual Type ItemType { get; }

        public InputMap map;
        public InputMapWindow window;

        DefaultGUIData defaultGUIData;

        public virtual void Initialize(OnInitializeContext context)
        {
            defaultGUIData = new DefaultGUIData()
            {
                map = map,
                window = window,
            };
        }

        public virtual void DrawGUI(OnGUIContext context)
        {
            EditorChangeChecker.BeginChangeCheck(() => window.SetMapDirty());
            OnGUI(context);
            EditorChangeChecker.EndChangeCheckAndCleanup();
        }

        public virtual void OnGUI(OnGUIContext context)
        {
            DrawDefault(context, defaultGUIData);
        }

        public static DefaultGUIData DrawDefault(OnGUIContext context, DefaultGUIData guiData)
        {
            DrawBase(context);

            guiData.delete = DeleteButton(guiData.delete, guiData.window);

            return guiData;
        }

        protected virtual void HandleDeletion(OnGUIContext context)
        {
            Debug.Log("Bro, this doesn't work");
        }

        protected static void DrawBase(OnGUIContext context)
        {
            context.serializedObject.Update();

            var isExpanded = true;
            var property = context.serializedObject.GetIterator();
            if (property.NextVisible(isExpanded))
            {
                isExpanded = false;
                while (property.NextVisible(isExpanded))
                {
                    EditorGUILayout.PropertyField(property.Copy());
                }
            }

            context.serializedObject.ApplyModifiedProperties();
        }

        protected bool DeleteButton(bool state, OnGUIContext context) =>
            DeleteButton(state, window, () => HandleDeletion(context));

        protected static bool DeleteButton(bool state, InputMapWindow window, Action OnDelete = null)
        {
            EditorGUILayout.BeginHorizontal();
            switch (state)
            {
                case true:
                    if (GUILayout.Button("Cancel"))
                        state = false;

                    if (GUILayout.Button("Confirm"))
                    {
                        state = false;
                        OnDelete?.Invoke();
                        window.SelectInInspector(null);
                        window.ReloadTrees();
                        SetMapDirty(window);
                    }
                    break;
                case false:
                    if (GUILayout.Button("Delete"))
                        state = true;
                    break;
            }

            EditorGUILayout.EndHorizontal();
            return state;
        }

        protected void SetMapDirty() =>
            SetMapDirty(window);

        protected static void SetMapDirty(InputMapWindow window)
        {
            window.SetMapDirty();
        }

        public struct OnInitializeContext
        {
            public InputMapItem item;
            public SerializedObject serializedObject;
        }

        public struct OnGUIContext
        {
            public InputMapItem item;
            public SerializedObject serializedObject;
            public bool debug;
        }

        public struct DefaultGUIData
        {
            public bool delete;
            public InputMap map;
            public InputMapWindow window;
        }
    }
}