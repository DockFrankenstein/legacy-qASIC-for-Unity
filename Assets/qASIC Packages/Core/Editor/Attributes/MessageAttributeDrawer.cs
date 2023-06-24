#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(MessageAttribute))]
    public class MessageAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            position.height -= EditorGUIUtility.standardVerticalSpacing;
            //EditorGUI.HelpBox(position, target.Message, ConvertToMessageType(target.IconType));

            GUI.Label(position, GetContent(), EditorStyles.helpBox);
        }

        GUIContent GetContent()
        {
            MessageAttribute target = attribute as MessageAttribute;
            return new GUIContent(target.Message, ConvertTypeToIcon(target.IconType));
        }

        static Texture ConvertTypeToIcon(InspectorMessageIconType iconType)
        {
            switch (iconType)
            {
                case InspectorMessageIconType.error:
                    return qGUIEditorUtility.ErrorIcon;
                case InspectorMessageIconType.warning:
                    return qGUIEditorUtility.WarningIcon;
                case InspectorMessageIconType.notification:
                    return qGUIEditorUtility.InfoIcon;
                default:
                    return null;
            }
        }

        public override float GetHeight()
        {
            float minHeight = (attribute as MessageAttribute).IconType == InspectorMessageIconType.none ?
                EditorGUIUtility.singleLineHeight: 
                EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

            return Mathf.Max(minHeight, EditorStyles.helpBox.CalcHeight(GetContent(), EditorGUIUtility.currentViewWidth)) + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
#endif