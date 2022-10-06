using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using qASIC.EditorTools;
using qASIC.EditorTools.Internal;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class InputMapItemInspector
    {
        public virtual Type ItemType { get; }
        public virtual bool AutoSave { get => true; }

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
            IMapItem mapItem = context.item as IMapItem;

            if (AutoSave)
                EditorChangeChecker.BeginChangeCheck(() => window.SetMapDirty());

            if (mapItem != null)
                mapItem.ItemName = EditorGUILayout.DelayedTextField("Name", mapItem.ItemName);

            OnGUI(context);

            EditorGUILayout.Space();

            if (mapItem != null)
                defaultGUIData.delete = DeleteButton(defaultGUIData.delete);

            //Debug
            if (context.debug)
            {
                EditorGUILayout.Space();
                qGUIInternalUtility.BeginGroup();

                OnDebugGUI(context);

                qGUIInternalUtility.EndGroup(false);
            }

            if (AutoSave)
                EditorChangeChecker.EndChangeCheckAndCleanup();
        }

        protected virtual void OnGUI(OnGUIContext context)
        {
            
        }

        protected virtual void OnDebugGUI(OnGUIContext context)
        {
            IMapItem mapItem = context.item as IMapItem;

            if (mapItem == null) return;
            mapItem.Guid = EditorGUILayout.DelayedTextField("GUID", mapItem.Guid);
            if (GUILayout.Button("Generate new GUID"))
                mapItem.Guid = Guid.NewGuid().ToString();
        }

        protected virtual void HandleDeletion(OnGUIContext context)
        {
            
        }

        protected bool DeleteButton(bool state, OnGUIContext context, string text = "Delete") =>
            DeleteButton(state, OnDelete:() =>
            {
                HandleDeletion(context);
                window.SelectInInspector(null);
                window.ReloadTrees();
                SetMapDirty();
            });

        protected static bool DeleteButton(bool state, string text = "Delete", Action OnDelete = null)
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
                    }
                    break;
                case false:
                    if (GUILayout.Button(text))
                        state = true;
                    break;
            }

            EditorGUILayout.EndHorizontal();
            return state;
        }

        protected void SetMapDirty() =>
            window.SetMapDirty();

        public struct OnInitializeContext
        {
            public object item;
            public SerializedObject serializedObject;
        }

        public struct OnGUIContext
        {
            public object item;
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