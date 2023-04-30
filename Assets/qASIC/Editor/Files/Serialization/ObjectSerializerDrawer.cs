using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using qASIC.EditorTools;

namespace qASIC.Files.Serialization.Internal
{
    [CustomPropertyDrawer(typeof(ObjectSerializer))]
    internal class ObjectSerializerDrawer : PropertyDrawer
    {
        const float _LABEL_HEIGHT = 24f;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_init)
            {
                Initialize(property);
                _init = true;
            }

            _pathHeight = _provider?.SavesToFile == true ?
                EditorGUI.GetPropertyHeight(p_filePath) :
                0f;

            return EditorGUIUtility.singleLineHeight * 1f + EditorGUIUtility.standardVerticalSpacing * 8f + _providerHeight + _pathHeight + _LABEL_HEIGHT;
        }

        bool _init = false;

        string[] _selectorOptions;
        int _selectorIndex;
        List<Type> _providerTypes;
        SerializationProvider _provider;

        SerializedProperty p_provider;
        SerializedProperty p_filePath;

        float _providerHeight;
        float _pathHeight;

        void Initialize(SerializedProperty property)
        {
            p_provider = property.FindPropertyRelative("provider");
            p_filePath = property.FindPropertyRelative(nameof(ObjectSerializer.filepath));

            _providerTypes = ObjectSerializer.GetAvaliableProviderTypes();
            _providerTypes.Insert(0, null);

            _selectorOptions = TypeFinder.CreateConstructorsFromTypes<SerializationProvider>(_providerTypes)
                .Select(x => x == null ? "None" : x.DisplayName)
                .ToArray();

            string[] providerTypData = p_provider.managedReferenceFullTypename.Split(' ');
            string typeName = providerTypData.Length == 2 ?
                $"{providerTypData[1]}, {providerTypData[0]}"
                : typeof(SerializationProvider).ToString();

            _selectorIndex = _providerTypes.IndexOf(Type.GetType(typeName));

            CreateProvider();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_init)
            {
                Initialize(property);
                _init = true;
            }

            Rect backgroundRect = new Rect(position);

            Rect labelBackgroundRect = position
                .SetHeight(_LABEL_HEIGHT);

            Rect labelSeparatorRect = labelBackgroundRect
                .ResizeToBottom(0f);

            Rect labelRect = labelBackgroundRect
                .BorderLeft(EditorGUIUtility.standardVerticalSpacing * 2f);

            position = position
                .Border(EditorGUIUtility.standardVerticalSpacing * 2f)
                .MoveY(_LABEL_HEIGHT + EditorGUIUtility.standardVerticalSpacing)
                .SetHeight(EditorGUIUtility.singleLineHeight);

            Rect typeSelectorRect = position;

            Rect filePathRect = typeSelectorRect
                .NextLine()
                .MoveY(EditorGUIUtility.standardVerticalSpacing)
                .SetHeight(_pathHeight);

            Rect providerRect = filePathRect
                .NextLine()
                .MoveY(EditorGUIUtility.standardVerticalSpacing)
                .SetHeight(_providerHeight);

            GUI.Box(backgroundRect, string.Empty, Styles.Background);

            GUI.DrawTexture(labelBackgroundRect, qGUIEditorUtility.DarkBackgroundTexture);

            EditorGUI.LabelField(labelRect, label, Styles.Label);

            qGUIEditorUtility.BorderAround(backgroundRect);
            qGUIEditorUtility.BorderAround(labelSeparatorRect);

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                _selectorIndex = EditorGUI.Popup(typeSelectorRect, "Type", _selectorIndex, _selectorOptions);

                if (scope.changed)
                {
                    p_provider.managedReferenceValue = _selectorIndex == 0 ? 
                        null : 
                        TypeFinder.CreateConstructorFromType(_providerTypes[_selectorIndex]);

                    CreateProvider();
                }
            }

            if (_pathHeight != 0f)
                EditorGUI.PropertyField(filePathRect, p_filePath);

            providerRect = qGUIEditorUtility.DrawProperty(providerRect, p_provider);

            _providerHeight = providerRect.height;
            

            if (property.serializedObject.hasModifiedProperties)
                property.serializedObject.ApplyModifiedProperties();
        }

        void CreateProvider()
        {
            _provider = _selectorIndex == 0 ?
                null :
                (SerializationProvider)TypeFinder.CreateConstructorFromType(_providerTypes[_selectorIndex]);
        }

        private static class Styles
        {
            public static GUIStyle Background => new GUIStyle()
                .WithBackground(qGUIEditorUtility.ButtonColorTexture);

            public static GUIStyle Label => new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                normal = new GUIStyleState()
                {
                    textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
                }
            };
        }
    }
}