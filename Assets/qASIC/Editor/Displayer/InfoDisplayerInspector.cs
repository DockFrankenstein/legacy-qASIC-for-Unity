//TODO: make this work
//using UnityEngine;
//using UnityEditor;
//using System.Text;

//namespace qASIC.Displayer.Internal
//{
//    [CustomEditor(typeof(InfoDisplayer))]
//    public class InfoDisplayerInspector : Editor
//    {
//        bool _init = false;

//        InfoDisplayer displayer;


//        void InitializeIfRequired()
//        {
//            if (!_init) return;
//            Initialize();
//        }

//        void Initialize()
//        {
//            _init = true;
//            displayer = (InfoDisplayer)target;
//        }

//        public override void OnInspectorGUI()
//        {
//            InitializeIfRequired();

//            DrawProperty(nameof(InfoDisplayer.displayerName), "Name");
//            DrawProperty(nameof(InfoDisplayer.text), "Text");

//            EditorGUILayout.Space();
//            GUILayout.Label("Text Style", EditorStyles.boldLabel);
//            DrawProperty(nameof(InfoDisplayer.separator), "Separator");
//            EditorGUILayout.Space();
//            DrawProperty(nameof(InfoDisplayer.startText), "Start Text");
//            DrawProperty(nameof(InfoDisplayer.endText), "End Text");

//            EditorGUILayout.Space();
//            GUILayout.Label("Hiding Entries", EditorStyles.boldLabel);
//            DrawProperty(nameof(InfoDisplayer.removeSeparatorText), "Remove Separator Text");
//            DrawProperty(nameof(InfoDisplayer.removeSeparatorValue), "Remove Separator Value");
//            DrawProperty(nameof(InfoDisplayer.acceptUnknown), "Accept Unknown");

//            var defaultLines = serializedObject.FindProperty(nameof(InfoDisplayer.defaultLines));
//            for (int i = 0; i < defaultLines.arraySize; i++)
//                EditorGUILayout.PropertyField(defaultLines.GetArrayElementAtIndex(i));

//            DrawPreview();
//        }

//        void DrawPreview()
//        {
//            GUIStyle textStyle = new GUIStyle(EditorStyles.label);

//            switch (displayer.text?.horizontalAlignment)
//            {
//                case TMPro.HorizontalAlignmentOptions.Left:
//                    textStyle.alignment = TextAnchor.UpperLeft;
//                    break;
//                case TMPro.HorizontalAlignmentOptions.Center:
//                    textStyle.alignment = TextAnchor.UpperCenter;
//                    break;
//                case TMPro.HorizontalAlignmentOptions.Right:
//                    textStyle.alignment = TextAnchor.UpperRight;
//                    break;
//            }

//            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
//            {
//                GUILayout.Label(GeneratePreviewText(), textStyle);
//            }
//        }

//        string GeneratePreviewText()
//        {
//            StringBuilder builder = new StringBuilder(displayer.startText);

//            builder.Append(displayer.endText);
//            return builder.ToString();
//        }

//        void DrawProperty(string propertyPath, string label) =>
//            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyPath), new GUIContent(label));
//    }
//}