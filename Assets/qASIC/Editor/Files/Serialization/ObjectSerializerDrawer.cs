using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using qASIC.Tools;
using qASIC.EditorTools;
using qASIC.EditorTools.Internal;

namespace qASIC.Files.Serialization.Internal
{
    [CustomPropertyDrawer(typeof(ObjectSerializer))]
    internal class ObjectSerializerDrawer : PropertyDrawer
    {
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

            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 6f + _providerHeight + _pathHeight;
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

            GUI.Box(position, string.Empty, EditorStyles.helpBox);

            position = position.Border(EditorGUIUtility.standardVerticalSpacing * 2f);

            Rect typeSelectorRect = position.SetHeight(EditorGUIUtility.singleLineHeight);

            Rect filePathRect = typeSelectorRect
                .NextLine()
                .MoveY(EditorGUIUtility.singleLineHeight)
                .SetHeight(_pathHeight);

            Rect providerRect = filePathRect
                .NextLine()
                .MoveY(EditorGUIUtility.standardVerticalSpacing)
                .SetHeight(_providerHeight);

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
    }
}