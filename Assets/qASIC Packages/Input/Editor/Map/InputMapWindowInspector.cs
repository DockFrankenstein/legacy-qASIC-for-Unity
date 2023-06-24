#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using qASIC.Input.Map.Internal.Inspectors;

using static qASIC.EditorTools.qGUIEditorUtility;
using static UnityEditor.EditorGUILayout;

namespace qASIC.Input.Map.Internal
{
    public class InputMapWindowInspector
    {
        public InputMap map;
        public InputMapWindow window;

        object _inspectionObject;
        bool _displayDeletePrompt;

        Vector2 _scroll;

        public string DefaultText { get; set; }
        public float Width { get; set; }

        private Dictionary<Type, InputMapItemInspector> _inspectors = null;
        public Dictionary<Type, InputMapItemInspector> _Inspectors
        {
            get
            {
                if (_inspectors == null)
                    RebuildInspectors();

                return _inspectors;
            }
        }

        bool _resetNameField = false;
        bool _editingNameField = false;

        InputMapItemInspector _currentInspector;
        InputMapItemInspector _defaultInspector = new InputMapItemInspector();

        string nameFieldValue;

        event Action OnNextRepaint;

        #region GUI
        public void OnGUI()
        {
            if (!map) return;

            GUIStyle inspectorStyle = new GUIStyle()
            {
                padding = new RectOffset(4, 4, 8, 8),
                fixedWidth = Width,
            };

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            _scroll = BeginScrollView(_scroll);

            using (new GUILayout.VerticalScope(inspectorStyle))
            {
                if (_currentInspector != null)
                {
                    InputMapItemInspector.OnGUIContext context = new InputMapItemInspector.OnGUIContext()
                    {
                        item = _inspectionObject,
                        debug = InputMapWindow.DebugMode,
                    };

                    _currentInspector?.DrawGUI(context);
                }
            }

            EndScrollView();

            EditorGUIUtility.labelWidth = labelWidth;

            if (OnNextRepaint != null && Event.current.type == EventType.Repaint)
            {
                OnNextRepaint?.Invoke();
                OnNextRepaint = null;
                InputMapWindow.GetEditorWindow().Repaint();
            }
        }
        #endregion

        #region Control
        public void SetObject(object obj)
        {
            //If the object gets selected right now after the layout event
            //there could be a problem with instance IDs so in order to avoid
            //that we assign the object on next repaint
            OnNextRepaint += () =>
            {
                _inspectionObject = obj;
                _displayDeletePrompt = false;
                if (_editingNameField)
                    GUI.FocusControl(null);
                _resetNameField = true;

                Type type = obj?.GetType();

                var inspector = type != null && _Inspectors.ContainsKey(type) ?
                    _Inspectors[type] :
                    _defaultInspector;

                SetInspector(inspector, _inspectionObject);
            };
        }

        public void SetInspector(InputMapItemInspector inspector, object obj = null)
        {
            _inspectionObject = obj;
            InputMapItemInspector.OnInitializeContext context = new InputMapItemInspector.OnInitializeContext()
            {
                item = _inspectionObject,
            };

            _currentInspector = inspector;
            _currentInspector.window = window;
            _currentInspector.map = map;
            _currentInspector.Initialize(context);
        }

        public void ResetInspector()
        {
            SetObject(null);
            GUI.FocusControl(null);
            _resetNameField = true;
        }
        #endregion

        #region GUI Methods
        string NameField(string name, bool setDirty = true) =>
            NameField(name, _ => { return true; }, setDirty);

        string NameField(string text, Func<string, bool> canRename, bool setDirty = true)
        {
            if (!_editingNameField || _resetNameField || !EditorGUIUtility.editingTextField)
            {
                nameFieldValue = text;
                _resetNameField = false;
                _editingNameField = false;
            }

            nameFieldValue = TextField("Name", nameFieldValue);
            _editingNameField = EditorGUIUtility.editingTextField;
            if (_editingNameField || !canRename.Invoke(nameFieldValue))
                return text;

            if (setDirty && nameFieldValue != text)
            {
                window.SetMapDirty();
                window.ReloadTreesNextRepaint();
            }

            return nameFieldValue;
        }

        string AxisField(InputGroup group, string label, string value)
        {
            GUIContent guiContent = new GUIContent(label);
            if (!group.ItemExists(value))
            {
                guiContent.image = ErrorIcon;
                guiContent.tooltip = "This action doesn't exist";
            }

            return DelayedTextField(guiContent, value);
        }

        bool DeleteButton(string label = "Delete", string confirm = "Confirm", string cancel = "Cancel")
        {
            bool state = false;
            BeginHorizontal();
            switch (_displayDeletePrompt)
            {
                case true:
                    if (GUILayout.Button(cancel))
                        _displayDeletePrompt = false;

                    if (state = GUILayout.Button(confirm))
                    {
                        _displayDeletePrompt = false;
                        ResetInspector();
                        window.SetMapDirty();
                    }
                    break;
                case false:
                    if (GUILayout.Button(label))
                        _displayDeletePrompt = true;
                    break;
            }
            EndHorizontal();
            return state;
        }
        #endregion

        #region Utility
        public void RebuildInspectors()
        {
            _inspectors = TypeFinder.CreateConstructorsFromTypesList<InputMapItemInspector>(TypeFinder.FindAllTypes<InputMapItemInspector>())
                        .Where(x => x.ItemType != null)
                        .ToDictionary(x => x.ItemType);
        }
        #endregion
    }
}
#endif