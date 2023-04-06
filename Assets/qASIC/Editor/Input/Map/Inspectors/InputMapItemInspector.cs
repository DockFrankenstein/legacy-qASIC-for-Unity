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
            {
                EditorChangeChecker.BeginChangeCheck(() =>
                {
                    window.SetMapDirty();
                    window.ReloadTrees();
                });
            }

            if (mapItem != null)
            {
                mapItem.ItemName = EditorGUILayout.DelayedTextField("Name", mapItem.ItemName);               
            }

            OnGUI(context);

            EditorGUILayout.Space();

            if (mapItem != null)
            {
                bool canDelete = CanDelete(context);

                using (new EditorGUI.DisabledScope(!canDelete))
                {
                    defaultGUIData.delete = DeleteButton(defaultGUIData.delete, context) && canDelete;
                }
            }

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
            if (context.item is IMapItem genericItem)
            {
                genericItem.Guid = EditorGUILayout.DelayedTextField("GUID", genericItem.Guid);
                if (GUILayout.Button("Generate new GUID"))
                    genericItem.Guid = Guid.NewGuid().ToString();
            }

            if (context.item is InputMapItem mapItem)
            {
                EditorGUILayout.LabelField("Map loaded", mapItem.MapLoaded.ToString());
            }
        }

        protected virtual void HandleDeletion(OnGUIContext context)
        {
            
        }

        protected virtual bool CanDelete(OnGUIContext context) =>
            true;

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
            using (new EditorChangeChecker.ChangeCheckPause())
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
            }

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