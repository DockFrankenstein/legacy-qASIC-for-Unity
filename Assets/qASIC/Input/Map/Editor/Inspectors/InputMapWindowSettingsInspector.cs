using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace qASIC.InputManagement.Map.Internal.Inspectors
{
    public class InputMapWindowSettingsInspector : InputMapItemInspector
    {
        public override bool AutoSave => false;

        bool _delete;

        protected override void OnGUI(OnGUIContext context)
        {
            GUILayout.Label("Settings", EditorStyles.whiteLargeLabel);

            InputMapWindow.Prefs_AutoSave = Toggle("Auto Save", InputMapWindow.Prefs_AutoSave);
            InputMapWindow.Prefs_AutoSaveTimeLimit = FloatField("Auto Save Time Limit", InputMapWindow.Prefs_AutoSaveTimeLimit);
            Space();
            InputMapWindow.Prefs_DefaultGroupColor = ColorField("Default Group Color", InputMapWindow.Prefs_DefaultGroupColor);
            Space();
            InputMapWindow.Prefs_ShowItemIcons = Toggle("Show Item Icons", InputMapWindow.Prefs_ShowItemIcons);

            Space();

            _delete = DeleteButton(_delete, "Reset preferences", InputMapWindow.ResetPreferences);
        }

        protected override void OnDebugGUI(OnGUIContext context)
        {
            base.OnDebugGUI(context);
            InputMapWindow.Prefs_InspectorWidth = FloatField("Inspector Width", InputMapWindow.Prefs_InspectorWidth);
        }
    }
}
