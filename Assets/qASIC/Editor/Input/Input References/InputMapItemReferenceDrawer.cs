#if UNITY_EDITOR
using UnityEditor;
using qASIC.Input.Internal.ReferenceExplorers;
using UnityEngine;
using System.Linq;
using qASIC.Input.Map;

using Manager = qASIC.Input.Internal.EditorInputManager;

using static UnityEditor.EditorGUIUtility;

namespace qASIC.Input.Internal
{
    [CustomPropertyDrawer(typeof(InputMapItemReference))]
    public class InputMapItemReferenceDrawer : PropertyDrawer
    {
        const string ITEM_NOT_FOUND_TEXT = "ITEM NOT FOUND";

        int ButtonWidth => 60;
        float Spacing => 4f;

        public void OnChangePressed(SerializedProperty guidProperty)
        {
            InputItemReferenceExplorer.OpenSelectWindow(guidProperty.stringValue, guid =>
            {
                guidProperty.stringValue = guid;
                guidProperty.serializedObject.ApplyModifiedProperties();
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            singleLineHeight * 5f + standardVerticalSpacing * 6f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Properties
            SerializedProperty guidProperty = property.FindPropertyRelative("guid");
            string guid = guidProperty.stringValue;

            //Rects
            Rect baseRect = new Rect(position);

            position = position.Border(standardVerticalSpacing + Spacing, 0f);
            position.height = singleLineHeight + standardVerticalSpacing;

            Rect backgroundRect = new Rect(baseRect.x, baseRect.y + singleLineHeight - 1f, baseRect.width, baseRect.height - singleLineHeight + 1f);
            Rect labelRect = new Rect(baseRect.position, new Vector2(baseRect.width, singleLineHeight));

            position.y += singleLineHeight + standardVerticalSpacing;

            Rect groupRect = new Rect(position);
            Rect nameRect = groupRect
                .MoveY(singleLineHeight + standardVerticalSpacing);
            Rect typeRect = nameRect
                .MoveY(singleLineHeight + standardVerticalSpacing);

            Rect guidRect = typeRect
                .MoveY(singleLineHeight + standardVerticalSpacing)
                .SetWidth(position.width - ButtonWidth - standardVerticalSpacing);

            Rect buttonRect = typeRect
                .MoveY(singleLineHeight + standardVerticalSpacing)
                .ResizeToRight(ButtonWidth);

            //Style
            GUIStyle labelStyle = new GUIStyle("Tab onlyOne")
            {
                padding = new RectOffset((int)Spacing * 2, (int)Spacing * 2, 0, 0),
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
            };

            //Drawing
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            GUI.Box(backgroundRect, GUIContent.none, Styles.Background);
            GUI.Label(labelRect, label, labelStyle);

            InputMapItem item = GetItem(guid);

            GUI.Label(groupRect, $"Group: {GetGroupName(item)}");
            GUI.Label(nameRect, $"Name: {item?.ItemName ?? ITEM_NOT_FOUND_TEXT}");
            GUI.Label(typeRect, $"Type: {item?.GetType()?.Name ?? ITEM_NOT_FOUND_TEXT}");
            EditorGUI.PropertyField(guidRect, guidProperty);

            if (GUI.Button(buttonRect, "Change"))
                OnChangePressed(guidProperty);

            EditorGUI.indentLevel = indent;
        }

        InputMapItem GetItem(string guid)
        {
            if (!Manager.Map)
                return null;

            return Manager.Map.ItemsDictionary.TryGetValue(guid, out InputMapItem item) ? item : null;
        }

        string GetGroupName(InputMapItem item)
        {
            if (!Manager.Map)
                return "MAP NOT LOADED";

            if (!Manager.Map.ItemsDictionary.ContainsValue(item))
                return ITEM_NOT_FOUND_TEXT;

            string target = Manager.Map.groups
                .Where(x => x.items.Contains(item))
                .Select(x => x.ItemName)
                .FirstOrDefault();

            return target;
        }

        protected class Styles
        {
            public static GUIStyle DefaultGroupField => new GUIStyle(EditorStyles.textField) { fontStyle = FontStyle.Italic };
            public static GUIStyle Background => new GUIStyle("HelpBox");
        }
    }
}
#endif