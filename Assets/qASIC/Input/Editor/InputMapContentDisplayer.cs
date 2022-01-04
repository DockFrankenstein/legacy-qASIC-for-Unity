using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.IMGUI.Controls;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputMapContentDisplayer
    {
        public InputGroup group;

        //InputMapContentTree actionTree;
        //TreeViewState actionTreeState;

        Vector2 scroll;

        public event Action<object> OnItemSelect;

        public InputMapContentDisplayer()
        {
            //if (actionTreeState == null)
            //    actionTreeState = new TreeViewState();

            //actionTree = new InputMapContentTree(actionTreeState);
        }

        public void OnGUI()
        {
            if (group == null) return;

            scroll = BeginScrollView(scroll);

            DisplayActions();
            EndScrollView();
        }

        void DisplayActions()
        {
            for (int i = 0; i < group.actions.Count; i++)
                if (Toggle(group.currentEditorSelectedAction == i, group.actions[i].actionName, EditorStyles.miniButton) != (group.currentEditorSelectedAction == i))
                    Select(i);
        }

        public void Select(int i)
        {
            group.currentEditorSelectedAction = i;
            OnItemSelect?.Invoke(new InputMapInspectorDisplayer.InspectorInputAction(group, group.actions[i]));
        }
    }
}