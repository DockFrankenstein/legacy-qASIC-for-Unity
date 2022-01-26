﻿using UnityEditor;
using UnityEngine;
using static qASIC.EditorTools.qGUIUtility;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(MessageAttribute))]
    public class MessageAttributeDrawer : DecoratorDrawer
    {
        public Texture texture;

        float height;

        public override void OnGUI(Rect position)
        {
            MessageAttribute target = attribute as MessageAttribute;
            Texture2D background = GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f));

            GUIContent content = new GUIContent(target.Message);
            GUIStyle style = new GUIStyle("Label")
            {
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(target.Icon ? 48 : 4, 4, 4, 4),
            };

            style.normal.background = background;
            height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
            if(target.Icon)
                height = Mathf.Clamp(height + 8f, 40f, float.MaxValue) + 8f;

            GUI.Label(new Rect(position.position, new Vector2(position.width, height)), target.Message, style);
            if (target.Icon) GUI.DrawTexture(new Rect(position.position + new Vector2(4f, 4f), new Vector2(40f, 40f)), target.Icon);
        }

        public override float GetHeight() =>
            height + 4f;
    }
}